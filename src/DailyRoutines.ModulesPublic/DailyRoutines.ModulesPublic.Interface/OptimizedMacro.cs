using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using DailyRoutines.Common.Extensions;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Command;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.Controllers;
using KamiToolKit.Nodes;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic.Interface;

public class OptimizedMacro : ModuleBase
{
	private class MacroData
	{
		public uint Index { get; set; }

		public uint IconID { get; set; }

		public byte[]? Name { get; set; }

		public Dictionary<int, byte[]> Lines { get; set; } = new Dictionary<int, byte[]>();
	}

	private class PresetData
	{
		public DateTime CreatedAt { get; set; } = OmenServiceBase<StandardTimeManager>.Instance().Now;

		public List<MacroData> IndividualMacros { get; set; } = new List<MacroData>();

		public List<MacroData> SharedMacros { get; set; } = new List<MacroData>();
	}

	private abstract class BaseMacroDialog : NativeAddon
	{
		protected TextButtonNode? CancelButton;

		protected TextButtonNode? ConfirmButton;

		protected void SetupButtons(float yOffset = 0f)
		{
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			Vector2 size = new Vector2(120f, 28f);
			float y = base.ContentSize.Y - size.Y + base.ContentStartPosition.Y + yOffset;
			TextButtonNode textButtonNode = new TextButtonNode();
			Vector2 contentStartPosition = base.ContentStartPosition;
			contentStartPosition.Y = y;
			textButtonNode.Position = contentStartPosition;
			textButtonNode.Size = size;
			textButtonNode.String = ReadOnlySeString.op_Implicit(LuminaWrapper.GetAddonText(1u));
			textButtonNode.OnClick = OnConfirmClick;
			ConfirmButton = textButtonNode;
			ConfirmButton.AttachNode(this);
			CancelButton = new TextButtonNode
			{
				Position = new Vector2(base.ContentSize.X - size.X + base.ContentPadding.X, y),
				Size = size,
				String = ReadOnlySeString.op_Implicit(LuminaWrapper.GetAddonText(2u)),
				OnClick = base.Close
			};
			CancelButton.AttachNode(this);
		}

		protected abstract void OnConfirmClick();
	}

	private class MacroPresetsInputAddon : BaseMacroDialog
	{
		private TextInputNode? inputNode;

		public Action<string>? OnInputComplete { get; set; }

		public string PlaceholderString { get; set; } = string.Empty;

		public string DefaultString { get; set; } = string.Empty;

		protected unsafe override void OnSetup(AtkUnitBase* addon, Span<AtkValue> atkValues)
		{
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			TextInputNode textInputNode = new TextInputNode();
			Vector2 contentStartPosition = base.ContentStartPosition;
			Vector2 contentPadding = base.ContentPadding;
			contentPadding.X = 0f;
			textInputNode.Position = contentStartPosition + contentPadding;
			contentPadding = base.ContentSize;
			contentPadding.Y = 28f;
			textInputNode.Size = contentPadding;
			textInputNode.PlaceholderString = PlaceholderString;
			textInputNode.String = ReadOnlySeString.op_Implicit(DefaultString);
			textInputNode.AutoSelectAll = true;
			inputNode = textInputNode;
			inputNode.AttachNode(this);
			SetupButtons();
		}

		protected override void OnConfirmClick()
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			if (inputNode != null && !string.IsNullOrWhiteSpace(((object)inputNode.String/*cast due to constrained. prefix*/).ToString()))
			{
				OnInputComplete?.Invoke(((object)inputNode.String/*cast due to constrained. prefix*/).ToString());
				Close();
			}
		}
	}

	private class MacroPresetsConfirmAddon : BaseMacroDialog
	{
		public Action? OnConfirm { get; set; }

		protected unsafe override void OnSetup(AtkUnitBase* addon, Span<AtkValue> atkValues)
		{
			SetupButtons(-5f);
		}

		protected override void OnConfirmClick()
		{
			OnConfirm?.Invoke();
			Close();
		}
	}

	private class Config : ModuleConfig
	{
		public bool ConfirmDelete = true;

		public bool ConfirmOverwrite = true;

		public Dictionary<string, PresetData> Presets = new Dictionary<string, PresetData>();
	}

	private Config config;

	private AddonController? macroController;

	private HorizontalListNode? controlListNode;

	private StringDropDownNode? presetDropdownNode;

	private TextButtonNode? loadButtonNode;

	private TextButtonNode? saveButtonNode;

	private TextButtonNode? deleteButtonNode;

	private MacroPresetsInputAddon? inputDialog;

	private MacroPresetsConfirmAddon? confirmDialog;

	private const int MACROS_PER_SET = 100;

	private const int MAX_MACRO_LINES = 15;

	private const string COMMAND = "macroset";

	private static string DefaultOption = LuminaWrapper.GetAddonText(4764u);

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("OptimizedMacroTitle"),
		Description = LanguageManager.Get("OptimizedMacroDescription"),
		Category = ModuleCategory.Interface,
		Author = new string[1] { "Rorinnn" },
		PreviewImageURL = new string[1] { "hxxps://gh.atmoomen.top/raw.githubusercontent.com/AtmoOmen/StaticAssets/main/DailyRoutines/image/OptimizedMacro-UI.png" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected unsafe override void Init()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		OmenServiceBase<CommandManager>.Instance().AddSubCommand("macroset", new CommandInfo(new HandlerDelegate(OnCommand))
		{
			HelpMessage = LanguageManager.Get("OptimizedMacro-CommandHelp")
		});
		inputDialog = new MacroPresetsInputAddon
		{
			Size = new Vector2(300f, 120f),
			InternalName = "DRMacroPresetsInputDialog",
			Title = ReadOnlySeString.op_Implicit(LanguageManager.Get("PleaseInput")),
			DepthLayer = 6
		};
		confirmDialog = new MacroPresetsConfirmAddon
		{
			Size = new Vector2(300f, 100f),
			InternalName = "DRMacroPresetsConfirmDialog",
			Title = ReadOnlySeString.op_Implicit(LanguageManager.Get("PleaseConfirmOperation")),
			DepthLayer = 6
		};
		macroController = new AddonController
		{
			AddonName = "Macro",
			OnSetup = OnAddonSetup,
			OnFinalize = OnAddonFinalize
		};
		macroController.Enable();
	}

	protected unsafe override void Uninit()
	{
		OmenServiceBase<CommandManager>.Instance().RemoveSubCommand("macroset");
		macroController?.Dispose();
		macroController = null;
		OnAddonFinalize(null);
		inputDialog?.Dispose();
		inputDialog = null;
		confirmDialog?.Dispose();
		confirmDialog = null;
		if (config != null)
		{
			config.Save(this);
		}
	}

	protected override void ConfigUI()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("Command")));
		IndentDisposable val = ImRaii.PushIndent(1, true);
		try
		{
			ImU8String val2 = new ImU8String(6, 2);
			((ImU8String)(ref val2)).AppendLiteral("/pdr ");
			((ImU8String)(ref val2)).AppendFormatted<string>("macroset");
			((ImU8String)(ref val2)).AppendLiteral(" ");
			((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get("OptimizedMacro-CommandHelp"));
			ImGui.TextUnformatted(val2);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		ImGui.NewLine();
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("OptimizedMacro-ConfirmBeforeDelete")), ref config.ConfirmOverwrite))
		{
			config.Save(this);
		}
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("OptimizedMacro-ConfirmBeforeOverwrite")), ref config.ConfirmDelete))
		{
			config.Save(this);
		}
	}

	private void OnCommand(string command, string args)
	{
		if (!string.IsNullOrWhiteSpace(args))
		{
			LoadPreset(args);
		}
	}

	private unsafe void OnAddonSetup(AtkUnitBase* addon)
	{
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		if (addon == null)
		{
			return;
		}
		AtkResNode* nodeById = ((AtkUnitBase)addon).GetNodeById(115u);
		if (nodeById != null)
		{
			((AtkResNode)nodeById).X = 440f;
			((AtkResNode)nodeById).Y = 40f;
		}
		AtkResNode* nodeById2 = ((AtkUnitBase)addon).GetNodeById(116u);
		if (nodeById2 != null)
		{
			((AtkResNode)nodeById2).X = 515f;
			((AtkResNode)nodeById2).Y = 40f;
		}
		AtkResNode* nodeById3 = ((AtkUnitBase)addon).GetNodeById(117u);
		if (nodeById3 != null)
		{
			((AtkResNode)nodeById3).X = 450f;
			((AtkResNode)nodeById3).Y = 521f;
		}
		controlListNode = new HorizontalListNode
		{
			Size = new Vector2(400f, 30f),
			Position = new Vector2(10f, 517f)
		};
		controlListNode.AttachNode(addon);
		presetDropdownNode = new StringDropDownNode
		{
			Size = new Vector2(150f, 30f),
			Position = new Vector2(0f, -1f),
			MaxListOptions = 10,
			Options = GetPresetNames(),
			OnOptionSelected = OnPresetSelected
		};
		controlListNode.AddNode(presetDropdownNode);
		loadButtonNode = new TextButtonNode
		{
			Size = new Vector2(100f, 30f),
			String = ReadOnlySeString.op_Implicit(LuminaWrapper.GetAddonText(6140u)),
			OnClick = OnLoadPreset,
			IsEnabled = false
		};
		loadButtonNode.LabelNode.AutoAdjustTextSize();
		controlListNode.AddNode(loadButtonNode);
		deleteButtonNode = new TextButtonNode
		{
			Size = new Vector2(100f, 30f),
			String = ReadOnlySeString.op_Implicit(LuminaWrapper.GetAddonText(68u)),
			OnClick = OnDeletePreset,
			IsEnabled = false
		};
		deleteButtonNode.LabelNode.AutoAdjustTextSize();
		controlListNode.AddNode(deleteButtonNode);
		saveButtonNode = new TextButtonNode
		{
			Size = new Vector2(100f, 30f),
			String = ReadOnlySeString.op_Implicit(LuminaWrapper.GetAddonText(552u)),
			IsEnabled = true,
			OnClick = delegate
			{
				if (presetDropdownNode.SelectedOption != DefaultOption)
				{
					OnOverwritePreset();
				}
				else
				{
					OnSavePreset();
				}
			}
		};
		saveButtonNode.LabelNode.AutoAdjustTextSize();
		controlListNode.AddNode(saveButtonNode);
	}

	private unsafe void OnAddonFinalize(AtkUnitBase* addon)
	{
		presetDropdownNode?.Dispose();
		presetDropdownNode = null;
		loadButtonNode?.Dispose();
		loadButtonNode = null;
		saveButtonNode?.Dispose();
		saveButtonNode = null;
		deleteButtonNode?.Dispose();
		deleteButtonNode = null;
		controlListNode?.Dispose();
		controlListNode = null;
	}

	private void OnPresetSelected(string selection)
	{
		bool flag = selection == DefaultOption;
		loadButtonNode.IsEnabled = !flag;
		deleteButtonNode.IsEnabled = !flag;
	}

	private void OnLoadPreset()
	{
		string selectedOption = presetDropdownNode.SelectedOption;
		if (!string.IsNullOrEmpty(selectedOption) && !(selectedOption == DefaultOption))
		{
			LoadPreset(selectedOption);
		}
	}

	private void OnSavePreset()
	{
		if (inputDialog != null)
		{
			inputDialog.PlaceholderString = LanguageManager.Get("Name") + " (" + LanguageManager.Get("Preset") + ")";
			inputDialog.DefaultString = string.Empty;
			inputDialog.OnInputComplete = delegate(string newName)
			{
				SavePreset(newName);
				presetDropdownNode.Options = GetPresetNames();
			};
			inputDialog.Toggle();
		}
	}

	private void OnOverwritePreset()
	{
		if (presetDropdownNode == null)
		{
			return;
		}
		string selectedPreset = presetDropdownNode.SelectedOption;
		if (string.IsNullOrEmpty(selectedPreset) || selectedPreset == DefaultOption)
		{
			return;
		}
		if (config.ConfirmOverwrite)
		{
			confirmDialog.OnConfirm = delegate
			{
				SavePreset(selectedPreset, isOverwrite: true);
			};
			confirmDialog.Toggle();
		}
		else
		{
			SavePreset(selectedPreset, isOverwrite: true);
		}
	}

	private void OnDeletePreset()
	{
		if (presetDropdownNode == null)
		{
			return;
		}
		string selectedPreset = presetDropdownNode.SelectedOption;
		if (string.IsNullOrEmpty(selectedPreset) || selectedPreset == DefaultOption)
		{
			return;
		}
		if (config.ConfirmDelete)
		{
			confirmDialog.OnConfirm = delegate
			{
				DeletePreset(selectedPreset);
			};
			confirmDialog.Toggle();
		}
		else
		{
			DeletePreset(selectedPreset);
		}
	}

	private unsafe void SavePreset(string presetName, bool isOverwrite = false)
	{
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			RaptureMacroModule* ptr = RaptureMacroModule.Instance();
			if (ptr != null && !string.IsNullOrWhiteSpace(presetName))
			{
				DateTime createdAt = OmenServiceBase<StandardTimeManager>.Instance().Now;
				if (isOverwrite && config.Presets.TryGetValue(presetName, out PresetData value))
				{
					createdAt = value.CreatedAt;
				}
				PresetData value2 = new PresetData
				{
					CreatedAt = createdAt,
					IndividualMacros = ReadMacrosFromMemory(ptr, 0u),
					SharedMacros = ReadMacrosFromMemory(ptr, 1u)
				};
				config.Presets[presetName] = value2;
				config.Save(this);
				OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(LanguageManager.Get(isOverwrite ? "OptimizedMacro-Notification-Overwritten" : "OptimizedMacro-Notification-Saved", presetName)));
			}
		}
		catch
		{
			OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(LanguageManager.Get("OptimizedMacro-Notification-SaveError", presetName)));
		}
	}

	private unsafe void LoadPreset(string presetName)
	{
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			RaptureMacroModule* ptr = RaptureMacroModule.Instance();
			if (ptr == null)
			{
				return;
			}
			RaptureHotbarModule* ptr2 = RaptureHotbarModule.Instance();
			if (ptr2 != null && !(presetName == DefaultOption))
			{
				if (string.IsNullOrWhiteSpace(presetName) || !config.Presets.TryGetValue(presetName, out PresetData value))
				{
					throw new Exception();
				}
				WriteMacrosToMemory(ptr, 0u, value.IndividualMacros);
				WriteMacrosToMemory(ptr, 1u, value.SharedMacros);
				((RaptureMacroModule)ptr).SetSavePendingFlag(true, 0u);
				((RaptureMacroModule)ptr).SetSavePendingFlag(true, 1u);
				((RaptureHotbarModule)ptr2).ReloadAllMacroSlots();
				OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(LanguageManager.Get("OptimizedMacro-Notification-Loaded", presetName)));
			}
		}
		catch
		{
			OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(LanguageManager.Get("OptimizedMacro-Notification-LoadError", presetName)));
		}
	}

	private void DeletePreset(string presetName)
	{
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (!(presetName == DefaultOption))
			{
				if (string.IsNullOrWhiteSpace(presetName) || !config.Presets.Remove(presetName))
				{
					throw new Exception();
				}
				config.Save(this);
				OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(LanguageManager.Get("OptimizedMacro-Notification-Deleted", presetName)));
				presetDropdownNode.Options = GetPresetNames();
				presetDropdownNode.SelectedOption = DefaultOption;
				loadButtonNode.IsEnabled = false;
				deleteButtonNode.IsEnabled = false;
			}
		}
		catch
		{
			OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(LanguageManager.Get("OptimizedMacro-Notification-DeleteError", presetName)));
		}
	}

	private unsafe static List<MacroData> ReadMacrosFromMemory(RaptureMacroModule* macroModule, uint set)
	{
		List<MacroData> list = new List<MacroData>();
		for (uint num = 0u; num < 100; num++)
		{
			Macro* macro = ((RaptureMacroModule)macroModule).GetMacro(set, num);
			if (macro == null)
			{
				continue;
			}
			ReadOnlySpan<byte> readOnlySpan = ((Utf8String)(&((Macro)macro).Name)).AsSpan();
			bool flag = false;
			for (int i = 0; i < 15; i++)
			{
				if (((Utf8String)(ref ((Macro)macro).Lines[i])).AsSpan().Length > 0)
				{
					flag = true;
					break;
				}
			}
			if (readOnlySpan.Length == 0 && ((Macro)macro).IconId == 0 && !flag)
			{
				continue;
			}
			MacroData obj = new MacroData
			{
				Index = num,
				IconID = ((Macro)macro).IconId
			};
			object name;
			if (readOnlySpan.Length <= 0)
			{
				name = null;
			}
			else
			{
				ReadOnlySpan<byte> readOnlySpan2 = readOnlySpan;
				int num2 = 0;
				byte[] array = new byte[1 + readOnlySpan2.Length];
				readOnlySpan2.CopyTo(new Span<byte>(array).Slice(num2, readOnlySpan2.Length));
				num2 += readOnlySpan2.Length;
				array[num2] = 0;
				name = array;
			}
			obj.Name = (byte[]?)name;
			MacroData macroData = obj;
			for (int j = 0; j < 15; j++)
			{
				ReadOnlySpan<byte> readOnlySpan3 = ((Utf8String)(ref ((Macro)macro).Lines[j])).AsSpan();
				if (readOnlySpan3.Length > 0)
				{
					Dictionary<int, byte[]> lines = macroData.Lines;
					int key = j;
					ReadOnlySpan<byte> readOnlySpan2 = readOnlySpan3;
					int num2 = 0;
					byte[] array = new byte[1 + readOnlySpan2.Length];
					readOnlySpan2.CopyTo(new Span<byte>(array).Slice(num2, readOnlySpan2.Length));
					num2 += readOnlySpan2.Length;
					array[num2] = 0;
					lines[key] = array;
				}
			}
			list.Add(macroData);
		}
		return list;
	}

	private unsafe static void WriteMacrosToMemory(RaptureMacroModule* macroModule, uint set, List<MacroData> macrosData)
	{
		for (uint num = 0u; num < 100; num++)
		{
			Macro* macro = ((RaptureMacroModule)macroModule).GetMacro(set, num);
			if (macro != null)
			{
				((Macro)macro).Clear();
				((Macro)macro).SetIcon(0u);
			}
		}
		foreach (MacroData macrosDatum in macrosData)
		{
			if (macrosDatum.Index >= 100)
			{
				continue;
			}
			Macro* macro2 = ((RaptureMacroModule)macroModule).GetMacro(set, macrosDatum.Index);
			if (macro2 == null)
			{
				continue;
			}
			((Macro)macro2).SetIcon(macrosDatum.IconID);
			if (macrosDatum.Name != null)
			{
				((Utf8String)(&((Macro)macro2).Name)).SetString((ReadOnlySpan<byte>)macrosDatum.Name);
			}
			foreach (var (num3, array2) in macrosDatum.Lines)
			{
				if (num3 >= 0 && num3 < 15 && array2.Length != 0)
				{
					((Utf8String)(ref ((Macro)macro2).Lines[num3])).SetString((ReadOnlySpan<byte>)array2);
				}
			}
		}
	}

	private List<string> GetPresetNames()
	{
		return (from x in config.Presets
			orderby x.Value.CreatedAt descending
			select x.Key).ToList().Prepend(DefaultOption).ToList();
	}
}
