using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using FFXIVClientStructs.FFXIV.Client.Game;
using OmenTools.Extensions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class MacroIntoActionQueue : ModuleBase
{
	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("MacroIntoActionQueueTitle"),
		Description = LanguageManager.Get("MacroIntoActionQueueDescription"),
		Category = ModuleCategory.Action
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected override void Init()
	{
		OmenServiceBase<UseActionManager>.Instance().RegPreUseAction(OnPreUseAction);
	}

	protected override void Uninit()
	{
		OmenServiceBase<UseActionManager>.Instance().Unreg(OnPreUseAction);
	}

	private unsafe static void OnPreUseAction(ref bool isPrevented, ref ActionType actionType, ref uint actionID, ref ulong targetID, ref uint extraParam, ref UseActionMode queueState, ref uint comboRouteID)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		queueState = (UseActionMode)1;
		Unsafe.Write(&((ActionManager)ActionManager.Instance()).QueueType, (UseActionMode)1);
		if ((int)actionType == 5 && actionID == 4)
		{
			actionType = (ActionType)1;
			actionID = ActionManagerExtension.GetAdjustSprintActionID();
			targetID = 3758096384uL;
		}
	}
}
