using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Hooking;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using Lumina.Excel.Sheets;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;

namespace DailyRoutines.ModulesPublic;

public class AutoChangeCharacterRaceSex : ModuleBase
{
	private unsafe delegate byte UpdateDrawDataDelegate(DrawDataContainer* data);

	private sealed class Config : ModuleConfig
	{
		public List<RaceSexMapping> Mappings = new List<RaceSexMapping>();

		public bool IsSkipLocalPlayer = true;
	}

	private sealed class RaceSexMapping
	{
		public bool Enabled = true;

		public byte SourceRace = 1;

		public byte SourceSex;

		public byte TargetRace = 1;

		public byte TargetSex = 1;
	}

	private sealed class RaceSexLookup
	{
		public static readonly RaceSexLookup Empty = new RaceSexLookup();

		public ushort ActiveMask;

		public PackedTargetTable PackedTargets;

		public bool HasAnyMapping
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return ActiveMask != 0;
			}
		}
	}

	[InlineArray(16)]
	private struct PackedTargetTable
	{
		private byte element0;
	}

	private static readonly CompSig UpdateDrawDataSig = new CompSig("48 89 5C 24 ?? 48 89 6C 24 ?? 56 57 41 56 48 83 EC 40 45 33 F6 48 8D 59 7A");

	private Hook<UpdateDrawDataDelegate>? UpdateDrawDataHook;

	private Config config;

	private RaceSexLookup activeLookup = RaceSexLookup.Empty;

	private byte pendingSourceRace = 1;

	private byte pendingSourceSex;

	private byte pendingTargetRace = 1;

	private byte pendingTargetSex = 1;

	private const byte MIN_RACE = 1;

	private const byte MAX_RACE = 8;

	private const byte MALE_SEX = 0;

	private const byte FEMALE_SEX = 1;

	private const int SEX_COUNT = 2;

	private const int LOOKUP_SIZE = 16;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoChangeCharacterRaceSexTitle"),
		Description = LanguageManager.Get("AutoChangeCharacterRaceSexDescription"),
		Category = ModuleCategory.System
	};

	protected unsafe override void Init()
	{
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		RebuildLookup();
		if (UpdateDrawDataHook == null)
		{
			UpdateDrawDataHook = UpdateDrawDataSig.GetHook<UpdateDrawDataDelegate>(UpdateDrawDataDetour);
		}
		UpdateDrawDataHook.Enable();
		if (GameState.IsLoggedIn)
		{
			RerenderAllPlayers();
		}
	}

	protected override void Uninit()
	{
		activeLookup = RaceSexLookup.Empty;
	}

	protected override void ConfigUI()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("AutoChangeCharacterRaceSex-SkipLocalPlayer")), ref config.IsSkipLocalPlayer))
		{
			SaveConfigAndRefresh();
		}
		ImGuiOm.HelpMarker(LanguageManager.Get("AutoChangeCharacterRaceSex-SkipLocalPlayer-Help"), 40f, (FontAwesomeIcon)61530);
		ImGui.NewLine();
		TableDisposable val = ImRaii.Table(ImU8String.op_Implicit("###AutoChangeCharacterRaceSexEditor"), 2, (ImGuiTableFlags)24576);
		try
		{
			if (TableDisposable.op_Implicit(val))
			{
				ImGui.TableSetupColumn(ImU8String.op_Implicit("Label"), (ImGuiTableColumnFlags)8, 120f * Globals.GlobalUIScale, 0u);
				ImGui.TableSetupColumn(ImU8String.op_Implicit("Input"), (ImGuiTableColumnFlags)4, 0f, 0u);
				ImGui.TableNextRow();
				ImGui.TableNextColumn();
				ImGui.AlignTextToFramePadding();
				ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("AutoChangeCharacterRaceSex-Source")));
				ImGui.TableNextColumn();
				ImGui.SetNextItemWidth(320f * Globals.GlobalUIScale);
				DrawRaceSexCombo("###AutoChangeCharacterRaceSexSource", ref pendingSourceRace, ref pendingSourceSex);
				ImGui.TableNextRow();
				ImGui.TableNextColumn();
				ImGui.AlignTextToFramePadding();
				ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("AutoChangeCharacterRaceSex-Target")));
				ImGui.TableNextColumn();
				ImGui.SetNextItemWidth(320f * Globals.GlobalUIScale);
				DrawRaceSexCombo("###AutoChangeCharacterRaceSexTarget", ref pendingTargetRace, ref pendingTargetSex);
			}
		}
		finally
		{
			((TableDisposable)(ref val)).Dispose();
		}
		if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61543, LanguageManager.Get("AutoChangeCharacterRaceSex-AddOrUpdate")))
		{
			AddOrUpdatePendingRule();
		}
		ImGuiOm.HelpMarker(LanguageManager.Get("AutoChangeCharacterRaceSex-AddOrUpdateHelp"), 40f, (FontAwesomeIcon)61530);
		ImGui.SameLine(0f, 10f * Globals.GlobalUIScale);
		if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)62193, LanguageManager.Get("Refresh")))
		{
			RebuildLookup();
			RerenderAllPlayers();
		}
		ImGui.Spacing();
		if (config.Mappings.Count == 0)
		{
			return;
		}
		TableDisposable val2 = ImRaii.Table(ImU8String.op_Implicit("###AutoChangeCharacterRaceSexRules"), 4, (ImGuiTableFlags)33556416, new Vector2(0f, 240f * Globals.GlobalUIScale));
		try
		{
			if (!TableDisposable.op_Implicit(val2))
			{
				return;
			}
			ImU8String val3 = new ImU8String(2, 1);
			((ImU8String)(ref val3)).AppendLiteral("##");
			((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get("Enabled"));
			ImGui.TableSetupColumn(val3, (ImGuiTableColumnFlags)8, ImGui.GetTextLineHeightWithSpacing(), 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit(LanguageManager.Get("AutoChangeCharacterRaceSex-Source")), (ImGuiTableColumnFlags)4, 1f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit(LanguageManager.Get("AutoChangeCharacterRaceSex-Target")), (ImGuiTableColumnFlags)4, 1f, 0u);
			ImU8String val4 = new ImU8String(2, 1);
			((ImU8String)(ref val4)).AppendLiteral("##");
			((ImU8String)(ref val4)).AppendFormatted<string>(LanguageManager.Get("Operation"));
			ImGui.TableSetupColumn(val4, (ImGuiTableColumnFlags)4, 0f, 0u);
			ImGui.TableHeadersRow();
			for (int i = 0; i < config.Mappings.Count; i++)
			{
				RaceSexMapping raceSexMapping = config.Mappings[i];
				ImGui.TableNextRow();
				ImGui.TableNextColumn();
				bool enabled = raceSexMapping.Enabled;
				ImU8String val5 = new ImU8String(36, 1);
				((ImU8String)(ref val5)).AppendLiteral("###AutoChangeCharacterRaceSexEnabled");
				((ImU8String)(ref val5)).AppendFormatted<int>(i);
				if (ImGui.Checkbox(val5, ref enabled))
				{
					raceSexMapping.Enabled = enabled;
					SaveConfigAndRefresh();
				}
				ImGui.TableNextColumn();
				ImGui.TextUnformatted(ImU8String.op_Implicit(GetRaceSexDisplayName(raceSexMapping.SourceRace, raceSexMapping.SourceSex)));
				ImGui.TableNextColumn();
				ImGui.TextUnformatted(ImU8String.op_Implicit(GetRaceSexDisplayName(raceSexMapping.TargetRace, raceSexMapping.TargetSex)));
				ImGui.TableNextColumn();
				ImU8String val6 = new ImU8String(32, 1);
				((ImU8String)(ref val6)).AppendLiteral("AutoChangeCharacterRaceSexDelete");
				((ImU8String)(ref val6)).AppendFormatted<int>(i);
				IdDisposable val7 = ImRaii.PushId(val6, true);
				try
				{
					if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)62189, LanguageManager.Get("Delete")))
					{
						config.Mappings.RemoveAt(i);
						SaveConfigAndRefresh();
						break;
					}
				}
				finally
				{
					((IDisposable)val7)?.Dispose();
				}
			}
		}
		finally
		{
			((TableDisposable)(ref val2)).Dispose();
		}
	}

	private unsafe byte UpdateDrawDataDetour(DrawDataContainer* data)
	{
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		if (data == null || ((DrawDataContainer)data).OwnerObject == null)
		{
			return UpdateDrawDataHook.Original(data);
		}
		RaceSexLookup raceSexLookup = Volatile.Read(in activeLookup);
		if (!raceSexLookup.HasAnyMapping)
		{
			return UpdateDrawDataHook.Original(data);
		}
		byte race = ((CustomizeData)(&((DrawDataContainer)data).CustomizeData)).Race;
		byte sex = ((CustomizeData)(&((DrawDataContainer)data).CustomizeData)).Sex;
		if (!TryGetTarget(raceSexLookup, race, sex, out var targetRace, out var targetSex) || (targetRace == race && targetSex == sex))
		{
			return UpdateDrawDataHook.Original(data);
		}
		BattleChara* localPlayer = Control.GetLocalPlayer();
		bool flag = config.IsSkipLocalPlayer && localPlayer != null;
		Unsafe.SkipInit<CustomizeData>(out var value);
		if (flag)
		{
			value = ((DrawDataContainer)(&((BattleChara)localPlayer).DrawData)).CustomizeData;
		}
		((CustomizeData)(&((DrawDataContainer)data).CustomizeData)).Race = targetRace;
		((CustomizeData)(&((DrawDataContainer)data).CustomizeData)).Sex = targetSex;
		((CustomizeData)(&((DrawDataContainer)data).CustomizeData)).Normalize(&((DrawDataContainer)data).CustomizeData);
		byte result = UpdateDrawDataHook.Original(data);
		if (flag)
		{
			Unsafe.Write(&((DrawDataContainer)(&((BattleChara)localPlayer).DrawData)).CustomizeData, value);
		}
		return result;
	}

	private void AddOrUpdatePendingRule()
	{
		int num = FindRuleIndex(pendingSourceRace, pendingSourceSex);
		if (num >= 0)
		{
			RaceSexMapping raceSexMapping = config.Mappings[num];
			raceSexMapping.TargetRace = pendingTargetRace;
			raceSexMapping.TargetSex = pendingTargetSex;
			raceSexMapping.Enabled = true;
		}
		else
		{
			config.Mappings.Add(new RaceSexMapping
			{
				Enabled = true,
				SourceRace = pendingSourceRace,
				SourceSex = pendingSourceSex,
				TargetRace = pendingTargetRace,
				TargetSex = pendingTargetSex
			});
		}
		SaveConfigAndRefresh();
	}

	private int FindRuleIndex(byte sourceRace, byte sourceSex)
	{
		Span<RaceSexMapping> span = CollectionsMarshal.AsSpan(config.Mappings);
		for (int i = 0; i < span.Length; i++)
		{
			ref RaceSexMapping reference = ref span[i];
			if (reference.SourceRace == sourceRace && reference.SourceSex == sourceSex)
			{
				return i;
			}
		}
		return -1;
	}

	private void SaveConfigAndRefresh()
	{
		RebuildLookup();
		config.Save(this);
		RerenderAllPlayers();
	}

	private void RebuildLookup()
	{
		RaceSexLookup raceSexLookup = new RaceSexLookup();
		Span<RaceSexMapping> span = CollectionsMarshal.AsSpan(config.Mappings);
		for (int i = 0; i < span.Length; i++)
		{
			ref RaceSexMapping reference = ref span[i];
			if (reference.Enabled)
			{
				int num = TryGetLookupIndex(reference.SourceRace, reference.SourceSex);
				if (num >= 0 && IsValidRace(reference.TargetRace) && IsValidSex(reference.TargetSex))
				{
					raceSexLookup.ActiveMask |= (ushort)(1 << num);
					raceSexLookup.PackedTargets[num] = PackRaceSex(reference.TargetRace, reference.TargetSex);
				}
			}
		}
		Volatile.Write(ref activeLookup, raceSexLookup);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool TryGetTarget(RaceSexLookup lookup, byte sourceRace, byte sourceSex, out byte targetRace, out byte targetSex)
	{
		int num = TryGetLookupIndex(sourceRace, sourceSex);
		if (num >= 0 && (lookup.ActiveMask & (1 << num)) != 0)
		{
			return TryUnpackRaceSex(lookup.PackedTargets[num], out targetRace, out targetSex);
		}
		targetRace = 0;
		targetSex = 0;
		return false;
	}

	private static bool DrawRaceSexCombo(string id, ref byte race, ref byte sex)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		ComboDisposable val = ImRaii.Combo(ImU8String.op_Implicit(id), ImU8String.op_Implicit(GetRaceSexDisplayName(race, sex)));
		try
		{
			if (!val)
			{
				return false;
			}
			for (byte b = 1; b <= 8; b++)
			{
				for (byte b2 = 0; b2 <= 1; b2++)
				{
					bool flag = b == race && b2 == sex;
					if (ImGui.Selectable(ImU8String.op_Implicit($"{GetRaceSexDisplayName(b, b2)}###{id}_{b}_{b2}"), flag, (ImGuiSelectableFlags)0, default(Vector2)))
					{
						race = b;
						sex = b2;
						result = true;
					}
				}
			}
			return result;
		}
		finally
		{
			((ComboDisposable)(ref val)).Dispose();
		}
	}

	private static string GetRaceSexDisplayName(byte race, byte sex)
	{
		return GetRaceName(race, sex) + " / " + GetSexName(sex);
	}

	private static string GetRaceName(byte race, byte sex)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (!LuminaGetter.TryGetRow<Race>((uint)race, out Race item))
		{
			return LanguageManager.Get("Unknown");
		}
		object obj;
		if (sex != 1)
		{
			obj = ((object)((Race)(ref item)).Masculine/*cast due to constrained. prefix*/).ToString();
			if (obj == null)
			{
				return LanguageManager.Get("Unknown");
			}
		}
		else
		{
			obj = ((object)((Race)(ref item)).Feminine/*cast due to constrained. prefix*/).ToString() ?? LanguageManager.Get("Unknown");
		}
		return (string)obj;
	}

	private static string GetSexName(byte sex)
	{
		if (sex != 1)
		{
			return LuminaWrapper.GetAddonText(15608u);
		}
		return LuminaWrapper.GetAddonText(15609u);
	}

	private unsafe static void RerenderAllPlayers()
	{
		foreach (ICharacter item in IOmenDalamudService<IObjectTable>.Instance().OfType<ICharacter>())
		{
			Character* ptr = item.ToStruct();
			if (ptr != null && ((Character)ptr).IsReadyToDraw())
			{
				((Character)ptr).DisableDraw();
				((Character)ptr).EnableDraw();
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int TryGetLookupIndex(byte race, byte sex)
	{
		uint num = (uint)(race - 1);
		if (num > 7 || sex > 1)
		{
			return -1;
		}
		return (int)((num << 1) | sex);
	}

	private static bool IsValidRace(byte race)
	{
		if (race >= 1)
		{
			return race <= 8;
		}
		return false;
	}

	private static bool IsValidSex(byte sex)
	{
		if ((uint)sex <= 1u)
		{
			return true;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static byte PackRaceSex(byte race, byte sex)
	{
		return (byte)(race | (sex << 4));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool TryUnpackRaceSex(byte packed, out byte race, out byte sex)
	{
		if (packed == 0)
		{
			race = 0;
			sex = 0;
			return false;
		}
		race = (byte)(packed & 0xF);
		sex = (byte)(packed >> 4);
		return true;
	}
}
