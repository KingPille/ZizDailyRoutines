using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Game.Text.Evaluator;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Memory;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic.Interface;

public class MoreMessageFilterPresets : ModuleBase
{
	private delegate int ApplyMessageFilterDelegate(nint filters);

	private delegate void ReloadLogTabDelegate(nint logModule, int tabIndex);

	private sealed class ApplyLogFilterMenuItem(MoreMessageFilterPresets module) : ContextMenuEntry
	{
		public override string Identifier => "MoreMessageFilterPresets";

		public unsafe override ContextMenuItem? Create(ContextMenuOpenedArgs args)
		{
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			if (module.config.Presets.Count == 0)
			{
				return null;
			}
			if (args.AddonName != "ChatLog")
			{
				return null;
			}
			AgentContext* defaultAgentContext = args.DefaultAgentContext;
			if (defaultAgentContext == null)
			{
				return null;
			}
			ContextMenu* currentContextMenu = ((AgentContext)defaultAgentContext).CurrentContextMenu;
			if (currentContextMenu == null)
			{
				return null;
			}
			if (((ContextMenu)currentContextMenu).EventParams[0].Int == 0)
			{
				return null;
			}
			if (!((AtkValue)(ref ((ContextMenu)currentContextMenu).EventParams[8])).GetValueAsString().Equals(LuminaWrapper.GetAddonText(370u), StringComparison.OrdinalIgnoreCase))
			{
				return null;
			}
			string text = LanguageManager.Get("MoreMessageFilterPresets-ContextMenu");
			ContextMenuItem obj = new ContextMenuItem
			{
				Name = ReadOnlySeString.op_Implicit(text)
			};
			ContextMenuSubmenu obj2 = new ContextMenuSubmenu
			{
				Title = ReadOnlySeString.op_Implicit(text)
			};
			List<ContextMenuEntry> list = new List<ContextMenuEntry>();
			list.AddRange(module.config.Presets.Select((FilterPreset preset) => new PresetMenuItem(module, preset)));
			obj2.Entries = new global::_003C_003Ez__ReadOnlyList<ContextMenuEntry>(list);
			obj.Submenu = obj2;
			return obj;
		}
	}

	private sealed class PresetMenuItem(MoreMessageFilterPresets module, FilterPreset preset) : ContextMenuEntry
	{
		public override string Identifier => "MoreMessageFilterPresets";

		public override ContextMenuItem Create(ContextMenuOpenedArgs args)
		{
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			int contextTabIndex = GetContextMenuTabIndex();
			if (contextTabIndex == -1)
			{
				return null;
			}
			return new ContextMenuItem
			{
				Name = ReadOnlySeString.op_Implicit(LanguageManager.Get("MoreMessageFilterPresets-ContextMenu-Apply", preset.Name, GetLogTabNames()[contextTabIndex])),
				OnClicked = delegate
				{
					module.ApplyFilterPresetAndNotify(preset, contextTabIndex);
				}
			};
		}
	}

	private class FilterPreset
	{
		public string Name = string.Empty;

		public byte[] PresetValue = new byte[MessageFilterSize];
	}

	private class Config : ModuleConfig
	{
		public List<FilterPreset> Presets = new List<FilterPreset>();
	}

	private static readonly CompSig ApplyMessageFilterSig = new CompSig("48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 4C 24 ?? 56 57 41 54 41 56 41 57 48 83 EC ?? 45 33 E4");

	private ApplyMessageFilterDelegate ApplyMessageFilter;

	private static readonly CompSig ReloadLogTabSig = new CompSig("48 63 C2 48 69 D0 28 09 00 00 48 81 C2 30 05 00 00 48 03 D1");

	private ReloadLogTabDelegate ReloadLogTab;

	private static readonly CompSig MessageFilterSizeSig = new CompSig("FF C5 81 FD ?? ?? ?? ?? 0F 82 ?? ?? ?? ?? 48 8B 0D");

	private static readonly int MessageFilterSize = ReadCMPImmediateValue(MessageFilterSizeSig.ScanText() + 2);

	private static List<(int Index, string Name)>? OrderedFilters;

	private Config config;

	private ApplyLogFilterMenuItem menuItem;

	private int sourceTabIndex;

	private string inputPresetName = string.Empty;

	private const int LOG_TAB_COUNT = 4;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("MoreMessageFilterPresetsTitle"),
		Description = LanguageManager.Get("MoreMessageFilterPresetsDescription"),
		Category = ModuleCategory.Interface,
		Author = new string[1] { "Ponta" }
	};

	protected override void Init()
	{
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		menuItem = new ApplyLogFilterMenuItem(this);
		ApplyMessageFilter = ApplyMessageFilterSig.GetDelegate<ApplyMessageFilterDelegate>();
		ReloadLogTab = ReloadLogTabSig.GetDelegate<ReloadLogTabDelegate>();
		foreach (FilterPreset preset in config.Presets)
		{
			if (preset.PresetValue.Length != MessageFilterSize)
			{
				Array.Resize(ref preset.PresetValue, MessageFilterSize);
			}
		}
		OmenServiceBase<ContextMenuManager>.Instance().Reg(menuItem);
	}

	protected override void Uninit()
	{
		OmenServiceBase<ContextMenuManager>.Instance().Unreg(menuItem);
	}

	protected override void ConfigUI()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_038f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0464: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_0499: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
		string[] logTabNames = GetLogTabNames();
		int selectedTabIndex = GetSelectedTabIndex();
		if (selectedTabIndex == -1)
		{
			return;
		}
		ImGuiStylePtr style = ImGui.GetStyle();
		Vector2 vector = ImGui.GetContentRegionAvail() - Globals.ScaledVector2(100f);
		vector.Y = 0f;
		Vector2 vector2 = vector;
		TableDisposable val = ImRaii.Table(ImU8String.op_Implicit("MessageFilterPreset"), 4, (ImGuiTableFlags)1920, vector2);
		try
		{
			if (!val)
			{
				return;
			}
			ImGui.TableSetupColumn(ImU8String.op_Implicit("Add"), (ImGuiTableColumnFlags)8, ImGui.GetTextLineHeightWithSpacing() + ((ImGuiStylePtr)(ref style)).FramePadding.X * 2f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("Name"), (ImGuiTableColumnFlags)0, 28f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("Enabled"), (ImGuiTableColumnFlags)0, 10f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("Operation"), (ImGuiTableColumnFlags)0, 26f, 0u);
			ImGui.TableNextRow((ImGuiTableRowFlags)1);
			ImGui.TableNextColumn();
			if (ImGuiOm.ButtonIconSelectable("AddNewPreset", (FontAwesomeIcon)61543))
			{
				if (selectedTabIndex >= 0)
				{
					sourceTabIndex = selectedTabIndex;
				}
				ImGui.OpenPopup(ImU8String.op_Implicit("AddNewPresetPopup"), (ImGuiPopupFlags)0);
			}
			PopupDisposable val2 = ImRaii.Popup(ImU8String.op_Implicit("AddNewPresetPopup"));
			try
			{
				if (PopupDisposable.op_Implicit(val2))
				{
					Vector4 vector3 = KnownColor.Chocolate.ToVector4();
					ImGui.TextColored(ref vector3, ImU8String.op_Implicit(LanguageManager.Get("MoreMessageFilterPresets-SourceTab")));
					IndentDisposable val3 = ImRaii.PushIndent(1, true);
					try
					{
						ComboDisposable val4 = ImRaii.Combo(ImU8String.op_Implicit("###AddFilterPresetCombo"), ImU8String.op_Implicit(logTabNames[sourceTabIndex]), (ImGuiComboFlags)8);
						try
						{
							if (ComboDisposable.op_Implicit(val4))
							{
								for (int i = 0; i < logTabNames.Length; i++)
								{
									if (ImGui.Selectable(ImU8String.op_Implicit(logTabNames[i]), sourceTabIndex == i, (ImGuiSelectableFlags)0, default(Vector2)))
									{
										sourceTabIndex = i;
									}
								}
							}
						}
						finally
						{
							((ComboDisposable)(ref val4)).Dispose();
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					vector3 = KnownColor.Chocolate.ToVector4();
					ImGui.TextColored(ref vector3, ImU8String.op_Implicit(LanguageManager.Get("Name")));
					string text = $"{LanguageManager.Get("Preset")} {config.Presets.Count + 1}";
					string name = (StringExtensions.IsNullOrEmpty(inputPresetName) ? text : inputPresetName);
					val3 = ImRaii.PushIndent(1, true);
					try
					{
						if (ImGui.InputText(ImU8String.op_Implicit("###PresetNameInput"), ref name, 256, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null))
						{
							inputPresetName = name;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61894, LanguageManager.Get("Save")))
					{
						AddFilterPreset(sourceTabIndex, name);
						config.Save(this);
						inputPresetName = string.Empty;
						ImGui.CloseCurrentPopup();
					}
				}
			}
			finally
			{
				((PopupDisposable)(ref val2)).Dispose();
			}
			ImGui.TableNextColumn();
			ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("Name")));
			ImGui.TableNextColumn();
			ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("Enabled")));
			for (int j = 0; j < config.Presets.Count; j++)
			{
				ImU8String val5 = new ImU8String(12, 1);
				((ImU8String)(ref val5)).AppendLiteral("FilterIndex_");
				((ImU8String)(ref val5)).AppendFormatted<int>(j);
				IdDisposable val6 = ImRaii.PushId(val5, true);
				try
				{
					FilterPreset filterPreset = config.Presets[j];
					ImGui.TableNextRow();
					ImGui.TableNextColumn();
					ImGuiOm.TextCentered($"{j + 1}");
					ImGui.TableNextColumn();
					ImGuiOm.Selectable(filterPreset.Name, selected: false, (ImGuiSelectableFlags)0);
					PopupDisposable val7 = ImRaii.ContextPopupItem(ImU8String.op_Implicit("PresetContextMenu"));
					try
					{
						if (PopupDisposable.op_Implicit(val7))
						{
							ImGui.AlignTextToFramePadding();
							Vector4 vector3 = KnownColor.Chocolate.ToVector4();
							ImGui.TextColored(ref vector3, ImU8String.op_Implicit(LanguageManager.Get("Name")));
							ImGui.SameLine();
							ImGui.InputText(ImU8String.op_Implicit("###RenamePresetInput"), ref filterPreset.Name, 128, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
							if (ImGui.IsItemDeactivatedAfterEdit())
							{
								config.Save(this);
								ImGui.CloseCurrentPopup();
							}
						}
					}
					finally
					{
						((PopupDisposable)(ref val7)).Dispose();
					}
					ImGui.TableNextColumn();
					var (value, value2) = CountEnabledFilters(filterPreset);
					ImGuiOm.Text($"{value} / {value2}");
					ImGui.TableNextColumn();
					ImU8String val8 = new ImU8String(1, 2);
					((ImU8String)(ref val8)).AppendFormatted<string>(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61498));
					((ImU8String)(ref val8)).AppendLiteral(" ");
					((ImU8String)(ref val8)).AppendFormatted<string>(LanguageManager.Get("Details"));
					if (ImGui.Button(val8, default(Vector2)))
					{
						ImGui.OpenPopup(ImU8String.op_Implicit("PresetDetailsPopup"), (ImGuiPopupFlags)0);
					}
					PopupDisposable val9 = ImRaii.Popup(ImU8String.op_Implicit("PresetDetailsPopup"));
					try
					{
						if (PopupDisposable.op_Implicit(val9))
						{
							DrawPresetDetails(filterPreset);
						}
					}
					finally
					{
						((PopupDisposable)(ref val9)).Dispose();
					}
					ImGui.SameLine();
					if (ImGuiOm.HoldButton($"Delete_{j}", FontAwesomeExtensions.ToIconString((FontAwesomeIcon)62189) + " " + LanguageManager.Get("Delete")))
					{
						config.Presets.RemoveAt(j);
						config.Save(this);
						break;
					}
				}
				finally
				{
					((IDisposable)val6)?.Dispose();
				}
			}
		}
		finally
		{
			((TableDisposable)(ref val)).Dispose();
		}
	}

	private static void DrawPresetDetails(FilterPreset preset)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(preset.Name));
		ImGui.Separator();
		ChildDisposable val = ImRaii.Child(ImU8String.op_Implicit("PresetDetailsChild"), Globals.ScaledVector2(520f, 480f), true);
		try
		{
			if (!val)
			{
				return;
			}
			foreach (var orderedFilter in GetOrderedFilters())
			{
				int item = orderedFilter.Index;
				string item2 = orderedFilter.Name;
				bool flag = preset.PresetValue[item] == 1;
				Vector4 vector2 = (flag ? KnownColor.LightGreen.ToVector4() : KnownColor.Gray.ToVector4());
				FontDisposable val2 = ImRaii.PushFont(UiBuilder.IconFont, true);
				try
				{
					ImGui.TextColored(ref vector2, ImU8String.op_Implicit(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)(flag ? 61452 : 61453))));
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				ImGui.SameLine();
				ImGui.TextColored(ref vector2, ImU8String.op_Implicit(item2));
			}
		}
		finally
		{
			((ChildDisposable)(ref val)).Dispose();
		}
	}

	private unsafe void AddFilterPreset(int index, string name)
	{
		nint messageFilter = GetMessageFilter((nint)LogFilterConfig.Instance(), index);
		FilterPreset filterPreset = new FilterPreset
		{
			Name = name
		};
		fixed (byte* presetValue = filterPreset.PresetValue)
		{
			Buffer.MemoryCopy((void*)messageFilter, presetValue, MessageFilterSize, MessageFilterSize);
		}
		config.Presets.Add(filterPreset);
	}

	private unsafe void ApplyFilterPreset(FilterPreset preset, int index)
	{
		LogFilterConfig* ptr = LogFilterConfig.Instance();
		nint messageFilter = GetMessageFilter((nint)ptr, index);
		fixed (byte* presetValue = preset.PresetValue)
		{
			Buffer.MemoryCopy(presetValue, (void*)messageFilter, MessageFilterSize, MessageFilterSize);
		}
		((LogFilterConfig)ptr).SaveFile(true);
		ApplyMessageFilter((nint)ptr);
		ReloadLogTab((nint)RaptureLogModule.Instance(), index);
	}

	private void ApplyFilterPresetAndNotify(FilterPreset preset, int tabIndex)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (tabIndex >= 0)
		{
			ApplyFilterPreset(preset, tabIndex);
			string text = LanguageManager.Get("MoreMessageFilterPresets-Notification-Applied", preset.Name, GetLogTabNames()[tabIndex]);
			OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(text));
			NotifyHelper.Toast(text);
		}
	}

	private unsafe static string[] GetLogTabNames()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		string[] array = new string[4];
		for (int i = 0; i < array.Length; i++)
		{
			string text = ((object)(*(Utf8String*)((RaptureLogModule)RaptureLogModule.Instance()).GetTabName(i))/*cast due to constrained. prefix*/).ToString();
			int num = i;
			string? text2;
			if (!string.IsNullOrEmpty(text))
			{
				text2 = text;
			}
			else
			{
				ISeStringEvaluator obj = DalamudServiceExtension.Instance<ISeStringEvaluator>();
				SeStringParameter reference = SeStringParameter.op_Implicit(i + 1);
				text2 = ((object)obj.EvaluateFromAddon(656u, new Span<SeStringParameter>(ref reference), (ClientLanguage?)null)/*cast due to constrained. prefix*/).ToString();
			}
			array[num] = text2;
		}
		return array;
	}

	private unsafe static int GetSelectedTabIndex()
	{
		AddonChatLog* chatLog = (AddonChatLog*)Addons.ChatLog;
		if (chatLog == null)
		{
			return -1;
		}
		byte tabIndex = ((AddonChatLog)chatLog).TabIndex;
		if (tabIndex >= 4)
		{
			return -1;
		}
		return tabIndex;
	}

	private unsafe static int GetContextMenuTabIndex()
	{
		AgentChatLog* agentChatLog = ((AgentModule)AgentModule.Instance()).GetAgentChatLog();
		if (agentChatLog == null)
		{
			return -1;
		}
		int num = MemoryHelper.Read<int>((IntPtr)(nint)((byte*)agentChatLog + 408));
		if (num < 0 || num >= 4)
		{
			return -1;
		}
		return num;
	}

	private static nint GetMessageFilter(nint filters, int index)
	{
		nint num = MessageFilterSize * index + 72;
		return filters + num;
	}

	private static List<(int Index, string Name)> GetOrderedFilters()
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		if (OrderedFilters != null)
		{
			return OrderedFilters;
		}
		List<(int, byte, byte, string)> list = new List<(int, byte, byte, string)>();
		for (int i = 0; i < MessageFilterSize; i++)
		{
			if (LuminaGetter.TryGetRow<LogFilter>((uint)i, out LogFilter item) && ((((LogFilter)(ref item)).Category != 0 && ((LogFilter)(ref item)).LogKind != 0) || 1 == 0))
			{
				list.Add((i, ((LogFilter)(ref item)).Category, ((LogFilter)(ref item)).DisplayOrder, ((object)((LogFilter)(ref item)).Name/*cast due to constrained. prefix*/).ToString()));
			}
		}
		return OrderedFilters = (from x in list
			orderby x.Category, x.DisplayOrder
			select (Index: x.Index, Name: x.Name)).ToList();
	}

	private static (int Enabled, int Total) CountEnabledFilters(FilterPreset preset)
	{
		int num = 0;
		int num2 = 0;
		foreach (var orderedFilter in GetOrderedFilters())
		{
			int item = orderedFilter.Index;
			num2++;
			if (preset.PresetValue[item] == 1)
			{
				num++;
			}
		}
		return (Enabled: num, Total: num2);
	}

	private static int ReadCMPImmediateValue(nint instructionAddress)
	{
		byte[] array = MemoryHelper.ReadRaw((IntPtr)instructionAddress, 6);
		int num = array.Length;
		if (num < 6)
		{
			if (num < 3)
			{
				goto IL_004b;
			}
		}
		else if (array[0] == 129 && array[1] == 253)
		{
			return BitConverter.ToInt32(array, 2);
		}
		if (array[0] == 131 && array[1] == 253)
		{
			return (sbyte)array[2];
		}
		goto IL_004b;
		IL_004b:
		throw new InvalidOperationException("未知的汇编指令");
	}
}
