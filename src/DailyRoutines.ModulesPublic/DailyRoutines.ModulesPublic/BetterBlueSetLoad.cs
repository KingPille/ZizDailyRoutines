using System;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Game.Command;
using Dalamud.Game.Text.Evaluator;
using Dalamud.Hooking;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Component.GUI;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.Helpers;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.Interop.Game.Models.Native;
using OmenTools.KamiToolKit.Addons.SelectYesno;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class BetterBlueSetLoad : ModuleBase
{
	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static HandlerDelegate _003C0_003E__OnCommand;
	}

	private Hook<AgentReceiveEventDelegate>? AgentAozNotebookReceiveEventHook;

	private SelectYesnoAddon? drSelectYesno;

	private const string COMMAND = "blueset";

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("BetterBlueSetLoadTitle"),
		Description = LanguageManager.Get("BetterBlueSetLoadDescription", "blueset"),
		Category = ModuleCategory.System
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected unsafe override void Init()
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Expected O, but got Unknown
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Expected O, but got Unknown
		AgentAozNotebookReceiveEventHook = (*((AgentInterface)((AgentModule)AgentModule.Instance()).GetAgentByInternalId((AgentId)325)).VirtualTable).HookVFuncFromName<AgentInterfaceVirtualTable, AgentReceiveEventDelegate>("ReceiveEvent", (AgentReceiveEventDelegate)AgentAozNotebookReceiveEventDetour);
		AgentAozNotebookReceiveEventHook.Enable();
		CommandManager commandManager = OmenServiceBase<CommandManager>.Instance();
		object obj = _003C_003EO._003C0_003E__OnCommand;
		if (obj == null)
		{
			HandlerDelegate val = OnCommand;
			_003C_003EO._003C0_003E__OnCommand = val;
			obj = (object)val;
		}
		commandManager.AddSubCommand("blueset", new CommandInfo((HandlerDelegate)obj)
		{
			HelpMessage = LanguageManager.Get("BetterBlueSetLoad-CommandHelp")
		});
	}

	protected override void Uninit()
	{
		OmenServiceBase<CommandManager>.Instance().RemoveSubCommand("blueset");
		drSelectYesno?.Dispose();
		drSelectYesno = null;
	}

	protected override void ConfigUI()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(0, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("Command"));
		ImGui.TextColored(ref vector, val);
		IndentDisposable val2 = ImRaii.PushIndent(1, true);
		try
		{
			ImU8String val3 = new ImU8String(8, 2);
			((ImU8String)(ref val3)).AppendLiteral("/pdr ");
			((ImU8String)(ref val3)).AppendFormatted<string>("blueset");
			((ImU8String)(ref val3)).AppendLiteral(" → ");
			((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get("BetterBlueSetLoad-CommandHelp"));
			ImGui.TextUnformatted(val3);
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	private unsafe AtkValue* AgentAozNotebookReceiveEventDetour(AgentInterface* agent, AtkValue* returnValues, AtkValue* values, uint valueCount, ulong eventKind)
	{
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Invalid comparison between Unknown and I4
		AtkUnitBase* addon = Addons.AOZNotebookPresetList;
		if (!(*addon).IsAddonAndNodesReady() || ((AtkUnitBase)addon).AtkValues == null || ((AtkValue)((AtkUnitBase)addon).AtkValues).UInt != 0 || eventKind != 1 || valueCount != 2)
		{
			return InvokeOriginal();
		}
		uint index = ((AtkValue)((byte*)values + Unsafe.SizeOf<AtkValue>())).UInt;
		if ((int)((AtkValue)((byte*)values + Unsafe.SizeOf<AtkValue>())).Type != 5 || index > 4)
		{
			return InvokeOriginal();
		}
		if (drSelectYesno != null && !AddonHelper.TryGetPtrByName("DRSelectYesno", out var _))
		{
			try
			{
				drSelectYesno?.Dispose();
				drSelectYesno = null;
			}
			catch
			{
			}
		}
		if (drSelectYesno != null)
		{
			return InvokeOriginal();
		}
		((AtkUnitBase)addon).IsVisible = false;
		SelectYesnoAddonOptions selectYesnoAddonOptions = new SelectYesnoAddonOptions();
		ISeStringEvaluator obj2 = DalamudServiceExtension.Instance<ISeStringEvaluator>();
		SeStringParameter reference = SeStringParameter.op_Implicit(GetSetName(index));
		selectYesnoAddonOptions.Prompt = obj2.EvaluateFromAddon(13653u, new Span<SeStringParameter>(ref reference), (ClientLanguage?)null);
		selectYesnoAddonOptions.Callback = delegate(SelectYesnoAddon _, SelectYesnoAddonResult result)
		{
			if (addon != null && !((AtkUnitBase)addon).IsVisible)
			{
				((AtkUnitBase)addon).IsVisible = true;
			}
			drSelectYesno = null;
			if (result == SelectYesnoAddonResult.Yes)
			{
				ApplyByIndex(index);
			}
		};
		selectYesnoAddonOptions.Position = new AddonPosition((*((AtkUnitBase)addon).RootNode).GetNodeState().Center, AddonPositionAlignment.TopCenter);
		selectYesnoAddonOptions.BlockedParentID = ((AtkUnitBase)Addons.AOZNotebook).Id;
		selectYesnoAddonOptions.ParentID = ((AtkUnitBase)Addons.AOZNotebook).Id;
		drSelectYesno = SelectYesnoAddon.Open(selectYesnoAddonOptions);
		((AtkValue)returnValues).SetBool(false);
		return returnValues;
		unsafe AtkValue* InvokeOriginal()
		{
			return AgentAozNotebookReceiveEventHook.Original(agent, returnValues, values, valueCount, eventKind);
		}
	}

	private unsafe static void OnCommand(string command, string args)
	{
		args = args.Trim();
		if (!string.IsNullOrEmpty(args))
		{
			if (uint.TryParse(args, out var result) && result < 5)
			{
				ApplyByIndex(result);
			}
			else if ((from x in ((AozNoteModule)AozNoteModule.Instance()).ActiveSets.ToArray()
				where !string.IsNullOrWhiteSpace(((ActiveSet)(ref x)).CustomNameString)
				select x).Select((ActiveSet value, int index) => (Index: (uint)index, Name: ((ActiveSet)(ref value)).CustomNameString)).DistinctBy(((uint Index, string Name) x) => x.Name).ToDictionary(((uint Index, string Name) x) => x.Name, ((uint Index, string Name) x) => x.Index)
				.TryGetValue(args, out result))
			{
				ApplyByIndex(result);
			}
		}
	}

	private unsafe static void ApplyByIndex(uint index)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (index > 4)
		{
			return;
		}
		ActiveSet val = ((AozNoteModule)AozNoteModule.Instance()).ActiveSets[(int)index];
		string setName = GetSetName(index);
		ActionManager* ptr = ActionManager.Instance();
		Span<uint> span = stackalloc uint[24];
		Span<uint> span2 = stackalloc uint[24];
		for (int i = 0; i < 24; i++)
		{
			span[i] = ((ActionManager)ptr).GetActiveBlueMageActionInSlot(i);
			span2[i] = ((ActiveSet)(ref val)).ActiveActions[i];
		}
		for (int j = 0; j < 24; j++)
		{
			if (span2[j] == 0)
			{
				continue;
			}
			for (int k = 0; k < 24; k++)
			{
				if (j != k && span2[j] == span[k])
				{
					((ActionManager)ptr).SwapBlueMageActionSlots(j, k);
					span2[j] = 0u;
					break;
				}
			}
		}
		for (int l = 0; l < 24; l++)
		{
			if (span2[l] != 0)
			{
				((ActionManager)ptr).AssignBlueMageActionToSlot(l, span2[l]);
			}
		}
		((AozNoteModule)AozNoteModule.Instance()).LoadActiveSetHotBars((int)index);
		Utf8String val2 = default(Utf8String);
		((Utf8String)(ref val2))._002Ector(setName);
		try
		{
			((RaptureLogModule)RaptureLogModule.Instance()).ShowLogMessageString(9472u, &val2);
		}
		finally
		{
			((IDisposable)(*(Utf8String*)(&val2))/*cast due to constrained. prefix*/).Dispose();
		}
	}

	private unsafe static string GetSetName(uint index)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		ActiveSet val = ((AozNoteModule)AozNoteModule.Instance()).ActiveSets[(int)index];
		if (!string.IsNullOrWhiteSpace(((ActiveSet)(ref val)).CustomNameString))
		{
			return ((ActiveSet)(ref val)).CustomNameString;
		}
		return LuminaWrapper.GetAddonText(12271 + index);
	}
}
