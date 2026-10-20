using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Hooking;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;

namespace DailyRoutines.ModulesPublic;

public class CustomizeGameObject : ModuleBase
{
	private unsafe delegate void* CharacterUpdateDelegate(Character* character);

	private class CustomizePreset : IEquatable<CustomizePreset>
	{
		public string Note { get; set; } = string.Empty;

		public CustomizeType Type { get; set; }

		public string Value { get; set; } = string.Empty;

		public float Scale { get; set; }

		public bool ScaleVFX { get; set; }

		public bool Enabled { get; set; }

		public bool Equals(CustomizePreset? other)
		{
			if (other == null)
			{
				return false;
			}
			if (Type == other.Type)
			{
				return string.Equals(Value, other.Value, StringComparison.Ordinal);
			}
			return false;
		}

		public override bool Equals(object? obj)
		{
			if (obj is CustomizePreset other)
			{
				return Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Type, Value);
		}

		public static bool operator ==(CustomizePreset left, CustomizePreset right)
		{
			return object.Equals(left, right);
		}

		public static bool operator !=(CustomizePreset left, CustomizePreset right)
		{
			return !object.Equals(left, right);
		}
	}

	private sealed record CustomizeHistoryEntry(CustomizePreset Preset, float OrigScale, float CurrentScale);

	private enum CustomizeType
	{
		Name,
		ModelCharaID,
		ModelSkeletonID,
		DataID,
		ObjectID
	}

	private class Config : ModuleConfig
	{
		public List<CustomizePreset> CustomizePresets = new List<CustomizePreset>();
	}

	private static readonly CompSig CharacterUpdateSig = new CompSig("4C 8B DC 53 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 84 24 ?? ?? ?? ?? 80 89");

	private Hook<CharacterUpdateDelegate>? CharacterUpdateHook;

	private Config config;

	private readonly Dictionary<uint, CustomizePreset> lookupDataID = new Dictionary<uint, CustomizePreset>();

	private readonly Dictionary<ulong, CustomizePreset> lookupObjectID = new Dictionary<ulong, CustomizePreset>();

	private readonly Dictionary<int, CustomizePreset> lookupModelCharaID = new Dictionary<int, CustomizePreset>();

	private readonly Dictionary<int, CustomizePreset> lookupModelSkeletonID = new Dictionary<int, CustomizePreset>();

	private readonly List<(byte[] NameBytes, CustomizePreset Preset)> lookupName = new List<(byte[], CustomizePreset)>();

	private readonly Dictionary<nint, CustomizeHistoryEntry> customizeHistory = new Dictionary<nint, CustomizeHistoryEntry>();

	private readonly Dictionary<nint, (ulong GameObjectID, int LastCheckTick)> failureCache = new Dictionary<nint, (ulong, int)>();

	private CustomizeType typeInput;

	private string noteInput = string.Empty;

	private float scaleInput = 1f;

	private string valueInput = string.Empty;

	private bool scaleVFXInput;

	private CustomizeType typeEditInput;

	private string noteEditInput = string.Empty;

	private float scaleEditInput = 1f;

	private string valueEditInput = string.Empty;

	private bool scaleVFXEditInput;

	private const int THROTTLE_INTERVAL_MS = 2000;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("CustomizeGameObjectTitle"),
		Description = LanguageManager.Get("CustomizeGameObjectDescription"),
		Category = ModuleCategory.System,
		Author = new string[1] { "HSS" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		NeedAuth = true
	};

	protected unsafe override void Init()
	{
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		RebuildLookupCache();
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += OnZoneChanged;
		if (CharacterUpdateHook == null)
		{
			CharacterUpdateHook = CharacterUpdateSig.GetHook<CharacterUpdateDelegate>(UpdateCharacterDetour);
		}
		CharacterUpdateHook.Enable();
	}

	protected override void Uninit()
	{
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChanged;
		CharacterUpdateHook?.Disable();
		if (IOmenDalamudService<IObjectTable>.Instance().LocalPlayer != null)
		{
			ResetAllCustomizeFromHistory();
		}
		customizeHistory.Clear();
		failureCache.Clear();
		ClearLookupCache();
	}

	private unsafe void* UpdateCharacterDetour(Character* character)
	{
		if (config.CustomizePresets.Count == 0)
		{
			return CharacterUpdateHook.Original(character);
		}
		void* result = CharacterUpdateHook.Original(character);
		ProcessCharacter(character);
		return result;
	}

	private void OnZoneChanged(uint u)
	{
		customizeHistory.Clear();
		failureCache.Clear();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe void ProcessCharacter(Character* character)
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Invalid comparison between Unknown and I4
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		if (character == null)
		{
			return;
		}
		ref CustomizeHistoryEntry valueRefOrNullRef = ref CollectionsMarshal.GetValueRefOrNullRef(customizeHistory, (nint)character);
		if (!Unsafe.IsNullRef(in valueRefOrNullRef))
		{
			if (MathF.Abs(((GameObject)(&((Character)character).GameObject)).Scale - valueRefOrNullRef.CurrentScale) > 0.1f)
			{
				customizeHistory.Remove((nint)character);
			}
			return;
		}
		ref(ulong, int) valueRefOrNullRef2 = ref CollectionsMarshal.GetValueRefOrNullRef(failureCache, (nint)character);
		if (!Unsafe.IsNullRef(in valueRefOrNullRef2))
		{
			GameObjectId gameObjectId = ((GameObject)(&((Character)character).GameObject)).GetGameObjectId();
			if (valueRefOrNullRef2.Item1 == GameObjectId.op_Implicit(gameObjectId) && Environment.TickCount64 - valueRefOrNullRef2.Item2 < 2000)
			{
				return;
			}
		}
		CustomizePreset value = null;
		if (lookupObjectID.Count > 0)
		{
			GameObjectId gameObjectId2 = ((GameObject)(&((Character)character).GameObject)).GetGameObjectId();
			if (lookupObjectID.TryGetValue(GameObjectId.op_Implicit(gameObjectId2), out value))
			{
				goto IL_0226;
			}
		}
		if ((lookupDataID.Count <= 0 || !lookupDataID.TryGetValue(((Character)character).BaseId, out value)) && (lookupModelCharaID.Count <= 0 || !lookupModelCharaID.TryGetValue(((ModelContainer)(&((Character)character).ModelContainer)).ModelCharaId, out value)) && (lookupModelSkeletonID.Count <= 0 || !lookupModelSkeletonID.TryGetValue(((ModelContainer)(&((Character)character).ModelContainer)).ModelSkeletonId, out value)))
		{
			if (lookupName.Count > 0 && ((int)((GameObject)(&((Character)character).GameObject)).ObjectKind != 1 || ((GameObject)(&((Character)character).GameObject)).Name[0] != 0))
			{
				Span<byte> name = ((GameObject)(&((Character)character).GameObject)).Name;
				foreach (var (presetNameBytes, customizePreset) in lookupName)
				{
					if (!IsNameEqual(name, presetNameBytes))
					{
						continue;
					}
					value = customizePreset;
					goto IL_0226;
				}
			}
			if (Unsafe.IsNullRef(in valueRefOrNullRef2))
			{
				failureCache[(nint)character] = (GameObjectId.op_Implicit(((GameObject)(&((Character)character).GameObject)).GetGameObjectId()), Environment.TickCount);
				return;
			}
			valueRefOrNullRef2.Item1 = GameObjectId.op_Implicit(((GameObject)(&((Character)character).GameObject)).GetGameObjectId());
			valueRefOrNullRef2.Item2 = Environment.TickCount;
			return;
		}
		goto IL_0226;
		IL_0226:
		if (!Unsafe.IsNullRef(in valueRefOrNullRef2))
		{
			failureCache.Remove((nint)character);
		}
		ApplyPreset(character, (nint)character, value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool IsNameEqual(Span<byte> gameObjectName, byte[] presetNameBytes)
	{
		if (gameObjectName[presetNameBytes.Length] != 0)
		{
			return false;
		}
		return ((ReadOnlySpan<byte>)gameObjectName).SequenceEqual((ReadOnlySpan<byte>)presetNameBytes);
	}

	private unsafe void ApplyPreset(Character* chara, nint addr, CustomizePreset preset)
	{
		float scale = ((GameObject)(&((Character)chara).GameObject)).Scale;
		float num = scale * preset.Scale;
		CustomizeHistoryEntry value = new CustomizeHistoryEntry(preset, scale, num);
		if (customizeHistory.TryAdd(addr, value))
		{
			((GameObject)(&((Character)chara).GameObject)).Scale = num;
			if (preset.ScaleVFX)
			{
				((GameObject)(&((Character)chara).GameObject)).VfxScale = num;
			}
			CustomizeType type = preset.Type;
			if ((uint)(type - 1) <= 1u)
			{
				((CharacterData)(&((Character)chara).CharacterData)).ModelScale = num;
			}
			if (((Character)chara).IsReadyToDraw())
			{
				((GameObject)(&((Character)chara).GameObject)).DisableDraw();
				((GameObject)(&((Character)chara).GameObject)).EnableDraw();
			}
		}
	}

	private void ResetCustomizeFromHistory(nint address)
	{
		if (customizeHistory.Count == 0 || !customizeHistory.TryGetValue(address, out CustomizeHistoryEntry value))
		{
			return;
		}
		if (address != (nint)0)
		{
			((GameObject)address).Scale = value.OrigScale;
			((GameObject)address).VfxScale = value.OrigScale;
			if (((GameObject)address).IsReadyToDraw())
			{
				((GameObject)address).DisableDraw();
				((GameObject)address).EnableDraw();
			}
		}
	}

	private unsafe void ResetAllCustomizeFromHistory()
	{
		if (customizeHistory.Count == 0)
		{
			return;
		}
		foreach (KeyValuePair<nint, CustomizeHistoryEntry> item in customizeHistory)
		{
			item.Deconstruct(out var key, out var value);
			nint num = key;
			CustomizeHistoryEntry customizeHistoryEntry = value;
			GameObject* ptr = (GameObject*)num;
			if (ptr != null)
			{
				((GameObject)ptr).Scale = customizeHistoryEntry.OrigScale;
				((GameObject)ptr).VfxScale = customizeHistoryEntry.OrigScale;
				if (((GameObject)ptr).IsReadyToDraw())
				{
					((GameObject)ptr).DisableDraw();
					((GameObject)ptr).EnableDraw();
				}
			}
		}
	}

	private void RemovePresetHistory(CustomizePreset? preset)
	{
		foreach (nint item in (from x in customizeHistory
			where x.Value.Preset == preset
			select x.Key).ToList())
		{
			ResetCustomizeFromHistory(item);
			customizeHistory.Remove(item);
		}
	}

	private void RebuildLookupCache()
	{
		ClearLookupCache();
		failureCache.Clear();
		foreach (CustomizePreset customizePreset in config.CustomizePresets)
		{
			if (!customizePreset.Enabled)
			{
				continue;
			}
			try
			{
				switch (customizePreset.Type)
				{
				case CustomizeType.Name:
					if (!string.IsNullOrEmpty(customizePreset.Value))
					{
						lookupName.Add((Encoding.UTF8.GetBytes(customizePreset.Value), customizePreset));
					}
					break;
				case CustomizeType.DataID:
				{
					if (uint.TryParse(customizePreset.Value, out var result2))
					{
						lookupDataID.TryAdd(result2, customizePreset);
					}
					break;
				}
				case CustomizeType.ObjectID:
				{
					string text = customizePreset.Value.Trim();
					ulong key = (text.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? ulong.Parse(text.AsSpan(2), NumberStyles.HexNumber) : ulong.Parse(text));
					lookupObjectID.TryAdd(key, customizePreset);
					break;
				}
				case CustomizeType.ModelCharaID:
				{
					if (int.TryParse(customizePreset.Value, out var result3))
					{
						lookupModelCharaID.TryAdd(result3, customizePreset);
					}
					break;
				}
				case CustomizeType.ModelSkeletonID:
				{
					if (int.TryParse(customizePreset.Value, out var result))
					{
						lookupModelSkeletonID.TryAdd(result, customizePreset);
					}
					break;
				}
				}
			}
			catch
			{
			}
		}
	}

	private void ClearLookupCache()
	{
		lookupDataID.Clear();
		lookupObjectID.Clear();
		lookupModelCharaID.Clear();
		lookupModelSkeletonID.Clear();
		lookupName.Clear();
	}

	protected override void ConfigUI()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		TargetInfoPreviewUI(TargetManager.Target);
		Vector2 vector = ImGui.GetContentRegionAvail() - Globals.ScaledVector2(100f);
		vector.Y = 0f;
		Vector2 vector2 = vector;
		TableDisposable val = ImRaii.Table(ImU8String.op_Implicit("###ConfigTable"), 7, (ImGuiTableFlags)640, vector2);
		try
		{
			if (!val)
			{
				return;
			}
			ImGui.TableSetupColumn(ImU8String.op_Implicit("启用"), (ImGuiTableColumnFlags)8, ImGui.GetTextLineHeightWithSpacing(), 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("备注"), (ImGuiTableColumnFlags)4, 20f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("模式"), (ImGuiTableColumnFlags)8, ImGui.CalcTextSize(ImU8String.op_Implicit("ModelSkeletonID"), false, -1f).X, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("值"), (ImGuiTableColumnFlags)4, 30f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("缩放比例"), (ImGuiTableColumnFlags)8, ImGui.CalcTextSize(ImU8String.op_Implicit("99.99"), false, -1f).X, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("缩放特效"), (ImGuiTableColumnFlags)8, ImGui.GetTextLineHeightWithSpacing(), 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("操作"), (ImGuiTableColumnFlags)8, 6f * ImGui.GetTextLineHeightWithSpacing(), 0u);
			ImGui.TableNextRow((ImGuiTableRowFlags)1);
			DrawAddPresetSection();
			ImGui.TableNextColumn();
			ImGuiOm.Text(LanguageManager.Get("Note"));
			ImGui.TableNextColumn();
			ImGuiOm.Text(LanguageManager.Get("CustomizeGameObject-CustomizeType"));
			ImGui.TableNextColumn();
			ImGuiOm.Text(LanguageManager.Get("Value"));
			ImGui.TableNextColumn();
			ImGuiOm.Text(LanguageManager.Get("Scale"));
			ImGui.TableNextColumn();
			ImGui.Dummy(new Vector2(32f));
			ImGuiOm.TooltipHover(LanguageManager.Get("CustomizeGameObject-ScaleVFX"));
			ImGui.TableNextColumn();
			ImGuiOm.Text(LanguageManager.Get("Operation"));
			CustomizePreset[] array = config.CustomizePresets.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				CustomizePreset preset = array[i];
				ImU8String val2 = new ImU8String(7, 1);
				((ImU8String)(ref val2)).AppendLiteral("Preset_");
				((ImU8String)(ref val2)).AppendFormatted<int>(i);
				IdDisposable val3 = ImRaii.PushId(val2, true);
				try
				{
					DrawPresetRow(i, preset);
				}
				finally
				{
					((IDisposable)val3)?.Dispose();
				}
			}
		}
		finally
		{
			((TableDisposable)(ref val)).Dispose();
		}
	}

	private void DrawAddPresetSection()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TableNextColumn();
		if (ImGuiOm.SelectableIconCentered("AddNewPreset", (FontAwesomeIcon)61543, selected: false, (ImGuiSelectableFlags)0))
		{
			ImGui.OpenPopup(ImU8String.op_Implicit("AddNewPresetPopup"), (ImGuiPopupFlags)0);
		}
		PopupDisposable val = ImRaii.Popup(ImU8String.op_Implicit("AddNewPresetPopup"));
		try
		{
			if (PopupDisposable.op_Implicit(val))
			{
				CustomizePresetEditorUI(ref typeInput, ref valueInput, ref scaleInput, ref scaleVFXInput, ref noteInput);
				ImGui.Spacing();
				Vector2 vector = new Vector2(ImGui.GetContentRegionAvail().X, 24f * Globals.GlobalUIScale);
				if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("Add")), vector) && scaleInput > 0f && !string.IsNullOrWhiteSpace(valueInput))
				{
					CustomizePreset item = new CustomizePreset
					{
						Enabled = true,
						Scale = scaleInput,
						Type = typeInput,
						Value = valueInput,
						ScaleVFX = scaleVFXInput,
						Note = noteInput
					};
					config.CustomizePresets.Add(item);
					SaveAndRebuild();
					ImGui.CloseCurrentPopup();
				}
			}
		}
		finally
		{
			((PopupDisposable)(ref val)).Dispose();
		}
	}

	private void DrawPresetRow(int i, CustomizePreset preset)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TableNextRow();
		ImGui.TableNextColumn();
		bool enabled = preset.Enabled;
		if (ImGui.Checkbox(ImU8String.op_Implicit("###IsEnabled"), ref enabled))
		{
			preset.Enabled = enabled;
			RemovePresetHistory(preset);
			SaveAndRebuild();
		}
		ImGui.TableNextColumn();
		ImGuiOm.Text(preset.Note);
		ImGui.TableNextColumn();
		ImGuiOm.Text(preset.Type.ToString());
		ImGui.TableNextColumn();
		ImGuiOm.Text(preset.Value);
		ImGui.TableNextColumn();
		ImGuiOm.Text(preset.Scale.ToString(CultureInfo.InvariantCulture));
		ImGui.TableNextColumn();
		bool scaleVFX = preset.ScaleVFX;
		if (ImGui.Checkbox(ImU8String.op_Implicit("###IsScaleVFX"), ref scaleVFX))
		{
			preset.ScaleVFX = scaleVFX;
			RemovePresetHistory(preset);
			SaveAndRebuild();
		}
		ImGui.TableNextColumn();
		if (ImGuiOm.ButtonIcon($"EditPreset_{i}", (FontAwesomeIcon)61508))
		{
			ImU8String val = default(ImU8String);
			((ImU8String)(ref val))._002Ector(19, 1);
			((ImU8String)(ref val)).AppendLiteral("EditNewPresetPopup_");
			((ImU8String)(ref val)).AppendFormatted<int>(i);
			ImGui.OpenPopup(val, (ImGuiPopupFlags)0);
		}
		ImU8String val2 = default(ImU8String);
		((ImU8String)(ref val2))._002Ector(19, 1);
		((ImU8String)(ref val2)).AppendLiteral("EditNewPresetPopup_");
		((ImU8String)(ref val2)).AppendFormatted<int>(i);
		PopupDisposable val3 = ImRaii.Popup(val2);
		try
		{
			if (PopupDisposable.op_Implicit(val3))
			{
				if (ImGui.IsWindowAppearing())
				{
					typeEditInput = preset.Type;
					noteEditInput = preset.Note;
					scaleEditInput = preset.Scale;
					valueEditInput = preset.Value;
					scaleVFXEditInput = preset.ScaleVFX;
				}
				if (CustomizePresetEditorUI(ref typeEditInput, ref valueEditInput, ref scaleEditInput, ref scaleVFXEditInput, ref noteEditInput))
				{
					preset.Type = typeEditInput;
					preset.Value = valueEditInput;
					preset.Scale = scaleEditInput;
					preset.ScaleVFX = scaleVFXEditInput;
					preset.Note = noteEditInput;
					RemovePresetHistory(preset);
					SaveAndRebuild();
				}
			}
		}
		finally
		{
			((PopupDisposable)(ref val3)).Dispose();
		}
		ImGui.SameLine();
		if (ImGuiOm.ButtonIcon($"DeletePreset_{i}", (FontAwesomeIcon)62189, LanguageManager.Get("HoldCtrlToDelete")) && ImGui.IsKeyDown((ImGuiKey)527))
		{
			RemovePresetHistory(preset);
			config.CustomizePresets.Remove(preset);
			SaveAndRebuild();
		}
		ImGui.SameLine();
		if (ImGuiOm.ButtonIcon($"ExportPreset_{i}", (FontAwesomeIcon)62830, LanguageManager.Get("ExportToClipboard")))
		{
			ModuleBase.ExportToClipboard(preset);
		}
		ImGui.SameLine();
		if (ImGuiOm.ButtonIcon($"ImportPreset_{i}", (FontAwesomeIcon)62831, LanguageManager.Get("ImportFromClipboard")))
		{
			CustomizePreset customizePreset = ModuleBase.ImportFromClipboard<CustomizePreset>();
			if (customizePreset != null && !config.CustomizePresets.Contains(customizePreset))
			{
				config.CustomizePresets.Add(customizePreset);
				SaveAndRebuild();
			}
		}
	}

	private void SaveAndRebuild()
	{
		config.Save(this);
		RebuildLookupCache();
	}

	private static bool CustomizePresetEditorUI(ref CustomizeType typeInput, ref string valueInput, ref float scaleInput, ref bool scaleVFXInput, ref string noteInput)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		TableDisposable val = ImRaii.Table(ImU8String.op_Implicit("CustomizeTable"), 2, (ImGuiTableFlags)0);
		try
		{
			if (!val)
			{
				return false;
			}
			ImGui.TableSetupColumn(ImU8String.op_Implicit("Label"), (ImGuiTableColumnFlags)8, ImGui.CalcTextSize(ImU8String.op_Implicit("真得五个字"), false, -1f).X, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("Input"), (ImGuiTableColumnFlags)8, 300f * Globals.GlobalUIScale, 0u);
			ImGui.TableNextRow();
			ImGui.TableNextColumn();
			ImU8String val2 = new ImU8String(1, 1);
			((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get("CustomizeGameObject-CustomizeType"));
			((ImU8String)(ref val2)).AppendLiteral(":");
			ImGui.TextUnformatted(val2);
			ImGui.TableNextColumn();
			ComboDisposable val3 = ImRaii.Combo(ImU8String.op_Implicit("###CustomizeTypeSelectCombo"), ImU8String.op_Implicit(typeInput.ToString()));
			try
			{
				if (ComboDisposable.op_Implicit(val3))
				{
					CustomizeType[] values = Enum.GetValues<CustomizeType>();
					for (int i = 0; i < values.Length; i++)
					{
						CustomizeType customizeType = values[i];
						if (ImGui.Selectable(ImU8String.op_Implicit(customizeType.ToString()), customizeType == typeInput, (ImGuiSelectableFlags)0, default(Vector2)))
						{
							typeInput = customizeType;
							result = true;
						}
					}
				}
			}
			finally
			{
				((ComboDisposable)(ref val3)).Dispose();
			}
			ImGui.TableNextRow();
			ImGui.TableNextColumn();
			ImU8String val4 = new ImU8String(1, 1);
			((ImU8String)(ref val4)).AppendFormatted<string>(LanguageManager.Get("Value"));
			((ImU8String)(ref val4)).AppendLiteral(":");
			ImGui.TextUnformatted(val4);
			ImGui.TableNextColumn();
			ImGui.InputText(ImU8String.op_Implicit("###CustomizeValueInput"), ref valueInput, 128, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
			if (ImGui.IsItemDeactivatedAfterEdit())
			{
				result = true;
			}
			ImGuiOm.TooltipHover(valueInput);
			ImGui.TableNextRow();
			ImGui.TableNextColumn();
			ImU8String val5 = new ImU8String(1, 1);
			((ImU8String)(ref val5)).AppendFormatted<string>(LanguageManager.Get("Scale"));
			((ImU8String)(ref val5)).AppendLiteral(":");
			ImGui.TextUnformatted(val5);
			ImGui.TableNextColumn();
			ImGui.SliderFloat(ImU8String.op_Implicit("###CustomizeScaleSilder"), ref scaleInput, 0.1f, 10f, ImU8String.op_Implicit("%.1f"), (ImGuiSliderFlags)0);
			if (ImGui.IsItemDeactivatedAfterEdit())
			{
				result = true;
			}
			ImGui.SameLine();
			if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("CustomizeGameObject-ScaleVFX")), ref scaleVFXInput))
			{
				result = true;
			}
			ImGui.TableNextRow();
			ImGui.TableNextColumn();
			ImU8String val6 = new ImU8String(1, 1);
			((ImU8String)(ref val6)).AppendFormatted<string>(LanguageManager.Get("Note"));
			((ImU8String)(ref val6)).AppendLiteral(":");
			ImGui.TextUnformatted(val6);
			ImGui.TableNextColumn();
			ImGui.InputText(ImU8String.op_Implicit("###CustomizeNoteInput"), ref noteInput, 128, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
			if (ImGui.IsItemDeactivatedAfterEdit())
			{
				result = true;
			}
			ImGuiOm.TooltipHover(noteInput);
			return result;
		}
		finally
		{
			((TableDisposable)(ref val)).Dispose();
		}
	}

	private unsafe static void TargetInfoPreviewUI(IGameObject? gameObject)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		if (!(gameObject is ICharacter character))
		{
			ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("CustomizeGameObject-NoTaretNotice")));
			return;
		}
		Vector2 vector = new Vector2(350f * Globals.GlobalUIScale, 0f);
		TableDisposable val = ImRaii.Table(ImU8String.op_Implicit("TargetInfoPreviewTable"), 2, (ImGuiTableFlags)640, vector);
		try
		{
			if (!val)
			{
				return;
			}
			ImGui.TableSetupColumn(ImU8String.op_Implicit("Lable"), (ImGuiTableColumnFlags)8, ImGui.CalcTextSize(ImU8String.op_Implicit("--Model Skeleton ID--"), false, -1f).X, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("Input"), (ImGuiTableColumnFlags)4, 50f, 0u);
			DrawRow(LanguageManager.Get("Name"), character.Name);
			DrawRow("Data ID", character.DataID.ToString());
			DrawRow("Object ID", character.GameObjectID.ToString());
			DrawRow("Model Chara ID", character.ModelCharaID.ToString());
			DrawRow("Model Skeleton ID", character.ModelSkeletonID.ToString());
			if (character is IPlayerCharacter playerCharacter && character.CurrentMount.HasValue)
			{
				Character* mountObject = ((MountContainer)(&((BattleChara)playerCharacter.ToStruct()).Mount)).MountObject;
				if (mountObject != null)
				{
					DrawRow("Mount Object ID", GameObjectId.op_Implicit(((Character)mountObject).GetGameObjectId()).ToString());
				}
			}
		}
		finally
		{
			((TableDisposable)(ref val)).Dispose();
		}
		static void DrawRow(string label, string value)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			ImGui.TableNextRow();
			ImGui.TableNextColumn();
			ImGui.AlignTextToFramePadding();
			ImGui.TextUnformatted(ImU8String.op_Implicit(label));
			ImGui.TableNextColumn();
			ImGui.SetNextItemWidth(-1f);
			ImU8String val2 = default(ImU8String);
			((ImU8String)(ref val2))._002Ector(10, 1);
			((ImU8String)(ref val2)).AppendLiteral("###");
			((ImU8String)(ref val2)).AppendFormatted<string>(label);
			((ImU8String)(ref val2)).AppendLiteral("Preview");
			ImGui.InputText(val2, ref value, 128, (ImGuiInputTextFlags)16384, (ImGuiInputTextCallbackDelegate)null);
		}
	}
}
