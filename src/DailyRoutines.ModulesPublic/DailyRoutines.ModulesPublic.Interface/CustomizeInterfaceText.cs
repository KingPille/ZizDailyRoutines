using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Hooking;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;
using InteropGenerator.Runtime;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Interop.Game.Models;

namespace DailyRoutines.ModulesPublic.Interface;

public class CustomizeInterfaceText : ModuleBase
{
	private delegate nint SetPlayerNamePlateDelegate(nint namePlateObjectPtr, bool isPrefixTitle, bool displayTitle, nint titlePtr, nint namePtr, nint fcNamePtr, nint prefix, int iconID);

	private unsafe delegate void TextNodeSetStringDelegate(AtkTextNode* textNode, CStringPointer text);

	private class Config : ModuleConfig
	{
		public List<ReplacePattern> ReplacePatterns = new List<ReplacePattern>();
	}

	private enum ReplaceMode
	{
		部分匹配,
		完全匹配,
		正则
	}

	private class ReplacePattern : IComparable<ReplacePattern>, IEquatable<ReplacePattern>
	{
		public string Key { get; set; } = string.Empty;

		public string Value { get; set; } = string.Empty;

		public ReplaceMode Mode { get; set; }

		public bool Enabled { get; set; }

		public Regex? Regex { get; set; }

		public ReplacePattern()
		{
		}

		public ReplacePattern(string key, string value, ReplaceMode mode, bool enabled)
		{
			Key = key;
			Value = value;
			Mode = mode;
			Enabled = enabled;
			if (mode == ReplaceMode.正则)
			{
				Regex = new Regex(key, RegexOptions.Compiled);
			}
		}

		public int CompareTo(ReplacePattern? other)
		{
			if (other != null)
			{
				return string.Compare(Key, other.Key, StringComparison.Ordinal);
			}
			return 1;
		}

		public bool Equals(ReplacePattern? other)
		{
			if (other != null)
			{
				return Key == other.Key;
			}
			return false;
		}

		public override bool Equals(object? obj)
		{
			return Equals(obj as ReplacePattern);
		}

		public override int GetHashCode()
		{
			return Key.GetHashCode(StringComparison.Ordinal);
		}

		public void Deconstruct(out string key, out string value, out ReplaceMode mode, out bool enabled)
		{
			string key2 = Key;
			string value2 = Value;
			ReplaceMode mode2 = Mode;
			bool enabled2 = Enabled;
			key = key2;
			value = value2;
			mode = mode2;
			enabled = enabled2;
		}
	}

	private struct PinnedMemory(IEnumerable array) : IDisposable
	{
		public readonly nint Pointer = handle.AddrOfPinnedObject();

		private GCHandle handle = GCHandle.Alloc(array, GCHandleType.Pinned);

		public void Dispose()
		{
			handle.Free();
		}
	}

	private static readonly CompSig SetPlayerNamePlateSig = new CompSig("48 89 5C 24 ?? 55 56 57 41 54 41 55 41 56 41 57 48 83 EC ?? 44 0F B6 EA");

	private Hook<SetPlayerNamePlateDelegate>? SetPlayerNamePlateHook;

	private static readonly CompSig TextNodeSetStringSig = new CompSig("E8 ?? ?? ?? ?? 48 83 C4 ?? 5B C3 CC CC CC CC CC CC CC CC CC CC 40 55 56 57 48 81 EC");

	private Hook<TextNodeSetStringDelegate>? TextNodeSetStringHook;

	private Config config;

	private string keyInput = string.Empty;

	private string valueInput = string.Empty;

	private int replaceModeInput;

	private string keyEditInput = string.Empty;

	private string valueEditInput = string.Empty;

	private int replaceModeEditInput;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("CustomizeInterfaceTextTitle"),
		Description = LanguageManager.Get("CustomizeInterfaceTextDescription"),
		Category = ModuleCategory.Interface
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		NeedAuth = true
	};

	protected unsafe override void Init()
	{
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		if (TextNodeSetStringHook == null)
		{
			TextNodeSetStringHook = TextNodeSetStringSig.GetHook<TextNodeSetStringDelegate>(TextNodeSetStringDetour);
		}
		TextNodeSetStringHook.Enable();
		if (SetPlayerNamePlateHook == null)
		{
			SetPlayerNamePlateHook = SetPlayerNamePlateSig.GetHook<SetPlayerNamePlateDelegate>(SetPlayerNamePlayerDetour);
		}
		SetPlayerNamePlateHook.Enable();
	}

	protected override void ConfigUI()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0538: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0409: Unknown result type (might be due to invalid IL or missing references)
		//IL_042a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0567: Unknown result type (might be due to invalid IL or missing references)
		//IL_056c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0588: Unknown result type (might be due to invalid IL or missing references)
		//IL_058a: Unknown result type (might be due to invalid IL or missing references)
		//IL_058f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0591: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0654: Unknown result type (might be due to invalid IL or missing references)
		//IL_0683: Unknown result type (might be due to invalid IL or missing references)
		//IL_0688: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0713: Unknown result type (might be due to invalid IL or missing references)
		GroupDisposable val = ImRaii.Group();
		try
		{
			ImGui.AlignTextToFramePadding();
			ImU8String val2 = new ImU8String(1, 1);
			((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get("Key"));
			((ImU8String)(ref val2)).AppendLiteral(":");
			ImGui.TextUnformatted(val2);
			ImGui.SameLine();
			ImGui.SetNextItemWidth(300f * Globals.GlobalUIScale);
			ImGui.InputText(ImU8String.op_Implicit("###KeyInput"), ref keyInput, 96, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
			ImGui.AlignTextToFramePadding();
			ImU8String val3 = new ImU8String(1, 1);
			((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get("Value"));
			((ImU8String)(ref val3)).AppendLiteral(":");
			ImGui.TextUnformatted(val3);
			ImGui.SameLine();
			ImGui.SetNextItemWidth(300f * Globals.GlobalUIScale);
			ImGui.InputText(ImU8String.op_Implicit("###ValueInput"), ref valueInput, 96, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
			ImGui.AlignTextToFramePadding();
			ImU8String val4 = new ImU8String(1, 1);
			((ImU8String)(ref val4)).AppendFormatted<string>(LanguageManager.Get("CustomizeInterfaceText-ReplaceMode"));
			((ImU8String)(ref val4)).AppendLiteral(":");
			ImGui.TextUnformatted(val4);
			ReplaceMode[] values = Enum.GetValues<ReplaceMode>();
			for (int i = 0; i < values.Length; i++)
			{
				ReplaceMode replaceMode = values[i];
				ImGui.SameLine();
				ImGui.RadioButton<int>(ImU8String.op_Implicit(replaceMode.ToString()), ref replaceModeInput, (int)replaceMode);
			}
		}
		finally
		{
			((GroupDisposable)(ref val)).Dispose();
		}
		ImGui.SameLine();
		if (ImGuiOm.ButtonIconWithTextVertical((FontAwesomeIcon)61543, LanguageManager.Get("Add")) && !string.IsNullOrWhiteSpace(keyInput))
		{
			ReplacePattern replacePattern = new ReplacePattern(keyInput, valueInput, (ReplaceMode)replaceModeInput, enabled: true);
			if (replaceModeEditInput == 2)
			{
				replacePattern.Regex = new Regex(replacePattern.Key, RegexOptions.Compiled);
			}
			if (!config.ReplacePatterns.Contains(replacePattern))
			{
				config.ReplacePatterns.Add(replacePattern);
				keyInput = (valueInput = string.Empty);
				config.Save(this);
			}
		}
		ImGui.NewLine();
		TableDisposable val5 = ImRaii.Table(ImU8String.op_Implicit("###CustomizeInterfaceTextTable"), 4, (ImGuiTableFlags)1920);
		try
		{
			if (!TableDisposable.op_Implicit(val5))
			{
				return;
			}
			ImGui.TableSetupColumn(ImU8String.op_Implicit("启用"), (ImGuiTableColumnFlags)8, 20f * Globals.GlobalUIScale, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("键"), (ImGuiTableColumnFlags)4, 50f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("值"), (ImGuiTableColumnFlags)4, 50f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("匹配模式"), (ImGuiTableColumnFlags)4, 15f, 0u);
			ImGui.TableNextRow((ImGuiTableRowFlags)1);
			ImGui.TableNextColumn();
			ImGui.TextUnformatted(ImU8String.op_Implicit(string.Empty));
			ImGui.TableNextColumn();
			ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("Key")));
			ImGui.TableNextColumn();
			ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("Value")));
			ImGui.TableNextColumn();
			ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("CustomizeInterfaceText-ReplaceMode")));
			ReplacePattern[] array = config.ReplacePatterns.ToArray();
			for (int j = 0; j < config.ReplacePatterns.Count; j++)
			{
				ReplacePattern replacePattern2 = array[j];
				ImGui.TableNextRow();
				ImGui.TableNextColumn();
				bool enabled = replacePattern2.Enabled;
				ImU8String val6 = new ImU8String(13, 1);
				((ImU8String)(ref val6)).AppendLiteral("###");
				((ImU8String)(ref val6)).AppendFormatted<int>(j);
				((ImU8String)(ref val6)).AppendLiteral("_IsEnabled");
				if (ImGui.Checkbox(val6, ref enabled))
				{
					config.ReplacePatterns[j].Enabled = enabled;
					config.Save(this);
				}
				ImGui.TableNextColumn();
				ImGui.Selectable(ImU8String.op_Implicit(replacePattern2.Key), false, (ImGuiSelectableFlags)1, default(Vector2));
				ImU8String val7 = new ImU8String(8, 1);
				((ImU8String)(ref val7)).AppendFormatted<string>(replacePattern2.Key);
				((ImU8String)(ref val7)).AppendLiteral("_KeyEdit");
				PopupDisposable val8 = ImRaii.ContextPopupItem(val7);
				try
				{
					if (PopupDisposable.op_Implicit(val8))
					{
						if (ImGui.IsWindowAppearing())
						{
							keyEditInput = replacePattern2.Key;
						}
						ImGui.AlignTextToFramePadding();
						ImU8String val9 = new ImU8String(1, 1);
						((ImU8String)(ref val9)).AppendFormatted<string>(LanguageManager.Get("Key"));
						((ImU8String)(ref val9)).AppendLiteral(":");
						ImGui.TextUnformatted(val9);
						ImGui.SameLine();
						ImGui.SetNextItemWidth(300f * Globals.GlobalUIScale);
						ImGui.InputText(ImU8String.op_Implicit("###KeyEditInput"), ref keyEditInput, 96, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
						if (ImGui.IsItemDeactivatedAfterEdit() && !string.IsNullOrWhiteSpace(keyEditInput))
						{
							ReplacePattern item = new ReplacePattern(keyEditInput, "", ReplaceMode.部分匹配, replacePattern2.Enabled);
							if (!config.ReplacePatterns.Contains(item))
							{
								config.ReplacePatterns[j].Key = keyEditInput;
								if (replacePattern2.Mode == ReplaceMode.正则)
								{
									config.ReplacePatterns[j].Regex = new Regex(keyEditInput);
								}
								config.Save(this);
							}
						}
						ImGui.SameLine();
						if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("Delete")), default(Vector2)) && config.ReplacePatterns.Remove(replacePattern2))
						{
							config.Save(this);
						}
					}
				}
				finally
				{
					((PopupDisposable)(ref val8)).Dispose();
				}
				ImGui.TableNextColumn();
				ImGui.Selectable(ImU8String.op_Implicit(replacePattern2.Value), false, (ImGuiSelectableFlags)1, default(Vector2));
				if (ImGui.IsItemClicked((ImGuiMouseButton)1))
				{
					valueEditInput = replacePattern2.Value;
				}
				ImU8String val10 = new ImU8String(10, 1);
				((ImU8String)(ref val10)).AppendFormatted<string>(replacePattern2.Key);
				((ImU8String)(ref val10)).AppendLiteral("_ValueEdit");
				PopupDisposable val11 = ImRaii.ContextPopupItem(val10);
				try
				{
					if (PopupDisposable.op_Implicit(val11))
					{
						ImGui.AlignTextToFramePadding();
						ImU8String val12 = new ImU8String(1, 1);
						((ImU8String)(ref val12)).AppendFormatted<string>(LanguageManager.Get("Value"));
						((ImU8String)(ref val12)).AppendLiteral(":");
						ImGui.TextUnformatted(val12);
						ImGui.SameLine();
						ImGui.SetNextItemWidth(300f * Globals.GlobalUIScale);
						ImGui.InputText(ImU8String.op_Implicit("###ValueEditInput"), ref valueEditInput, 96, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
						if (ImGui.IsItemDeactivatedAfterEdit())
						{
							config.ReplacePatterns[j].Value = valueEditInput;
							config.Save(this);
						}
					}
				}
				finally
				{
					((PopupDisposable)(ref val11)).Dispose();
				}
				ImGui.TableNextColumn();
				ImGui.Selectable(ImU8String.op_Implicit(replacePattern2.Mode.ToString()), false, (ImGuiSelectableFlags)1, default(Vector2));
				if (ImGui.IsItemClicked((ImGuiMouseButton)1))
				{
					replaceModeEditInput = (int)replacePattern2.Mode;
				}
				ImU8String val13 = new ImU8String(9, 1);
				((ImU8String)(ref val13)).AppendFormatted<string>(replacePattern2.Key);
				((ImU8String)(ref val13)).AppendLiteral("_ModeEdit");
				PopupDisposable val14 = ImRaii.ContextPopupItem(val13);
				try
				{
					if (!PopupDisposable.op_Implicit(val14))
					{
						continue;
					}
					ImGui.AlignTextToFramePadding();
					ImU8String val15 = new ImU8String(1, 1);
					((ImU8String)(ref val15)).AppendFormatted<string>(LanguageManager.Get("CustomizeInterfaceText-ReplaceMode"));
					((ImU8String)(ref val15)).AppendLiteral(":");
					ImGui.TextUnformatted(val15);
					ReplaceMode[] values = Enum.GetValues<ReplaceMode>();
					for (int i = 0; i < values.Length; i++)
					{
						ReplaceMode replaceMode2 = values[i];
						ImGui.SameLine();
						ImGui.RadioButton<int>(ImU8String.op_Implicit(replaceMode2.ToString()), ref replaceModeEditInput, (int)replaceMode2);
						if (ImGui.IsItemDeactivatedAfterEdit())
						{
							config.ReplacePatterns[j].Mode = (ReplaceMode)replaceModeEditInput;
							if (replaceModeEditInput == 2)
							{
								config.ReplacePatterns[j].Regex = new Regex(replacePattern2.Key);
							}
							config.Save(this);
						}
					}
				}
				finally
				{
					((PopupDisposable)(ref val14)).Dispose();
				}
			}
		}
		finally
		{
			((TableDisposable)(ref val5)).Dispose();
		}
	}

	private unsafe void TextNodeSetStringDetour(AtkTextNode* textNode, CStringPointer text)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		if (textNode == null || !((CStringPointer)(ref text)).HasValue)
		{
			TextNodeSetStringHook.Original(textNode, text);
			return;
		}
		SeString val = SeString.Parse(text.Value);
		if (val.Payloads.Count == 0)
		{
			TextNodeSetStringHook.Original(textNode, text);
			return;
		}
		if (ApplyTextReplacements(val, out byte[] modifiedText))
		{
			GCHandle gCHandle = GCHandle.Alloc(modifiedText, GCHandleType.Pinned);
			try
			{
				TextNodeSetStringHook.Original(textNode, new CStringPointer((byte*)gCHandle.AddrOfPinnedObject()));
				return;
			}
			finally
			{
				gCHandle.Free();
			}
		}
		TextNodeSetStringHook.Original(textNode, text);
	}

	private nint SetPlayerNamePlayerDetour(nint namePlateObjectPtr, bool isPrefixTitle, bool displayTitle, nint titlePtr, nint namePtr, nint fcNamePtr, nint prefix, int iconID)
	{
		PinnedMemory pinnedMemory = ReplaceTextAndAllocate(namePtr);
		try
		{
			PinnedMemory pinnedMemory2 = ReplaceTextAndAllocate(titlePtr);
			try
			{
				PinnedMemory pinnedMemory3 = ReplaceTextAndAllocate(fcNamePtr);
				try
				{
					return SetPlayerNamePlateHook.Original(namePlateObjectPtr, isPrefixTitle, displayTitle, pinnedMemory2.Pointer, pinnedMemory.Pointer, pinnedMemory3.Pointer, prefix, iconID);
				}
				finally
				{
					((IDisposable)pinnedMemory3/*cast due to constrained. prefix*/).Dispose();
				}
			}
			finally
			{
				((IDisposable)pinnedMemory2/*cast due to constrained. prefix*/).Dispose();
			}
		}
		finally
		{
			((IDisposable)pinnedMemory/*cast due to constrained. prefix*/).Dispose();
		}
	}

	private PinnedMemory ReplaceTextAndAllocate(nint originalTextPtr)
	{
		SeString origText = MemoryHelper.ReadSeStringNullTerminated((IntPtr)originalTextPtr);
		if (!ApplyTextReplacements(origText, out byte[] modifiedText))
		{
			return new PinnedMemory(Array.Empty<byte>());
		}
		return new PinnedMemory(modifiedText);
	}

	private bool ApplyTextReplacements(SeString origText, out byte[]? modifiedText)
	{
		modifiedText = null;
		TextPayload[] array = origText.Payloads.OfType<TextPayload>().ToArray();
		bool flag = false;
		foreach (ReplacePattern replacePattern in config.ReplacePatterns)
		{
			if (!replacePattern.Enabled)
			{
				continue;
			}
			TextPayload[] array2 = array;
			foreach (TextPayload val in array2)
			{
				string text = val.Text;
				string text2 = replacePattern.Mode switch
				{
					ReplaceMode.部分匹配 => text.Contains(replacePattern.Key, StringComparison.Ordinal) ? text.Replace(replacePattern.Key, replacePattern.Value) : null, 
					ReplaceMode.完全匹配 => text.Equals(replacePattern.Key, StringComparison.Ordinal) ? replacePattern.Value : null, 
					ReplaceMode.正则 => replacePattern.Regex?.Replace(text, replacePattern.Value), 
					_ => null, 
				};
				if (text2 != null)
				{
					val.Text = text2;
					flag = true;
				}
			}
		}
		if (flag)
		{
			modifiedText = origText.Encode();
			return true;
		}
		return false;
	}
}
