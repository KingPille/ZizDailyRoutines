using System.Collections.Frozen;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using FFXIVClientStructs.FFXIV.Client.Game;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class AutoManageInterruptAction : ModuleBase
{
	private static readonly FrozenSet<uint> InterruptActions = FrozenSet.Create<uint>(7538u, 7551u);

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoManageInterruptActionTitle"),
		Description = LanguageManager.Get("AutoManageInterruptActionDescription"),
		Category = ModuleCategory.Action
	};

	protected override void Init()
	{
		OmenServiceBase<UseActionManager>.Instance().RegPreUseAction(OnPreUseAction);
	}

	protected override void Uninit()
	{
		OmenServiceBase<UseActionManager>.Instance().Unreg(OnPreUseAction);
	}

	private static void OnPreUseAction(ref bool isPrevented, ref ActionType actionType, ref uint actionID, ref ulong targetID, ref uint extraParam, ref UseActionMode queueState, ref uint comboRouteID)
	{
		if ((int)actionType == 1 && InterruptActions.Contains(actionID) && !(TargetManager.Target is IBattleChara { IsCasting: not false, IsCastInterruptible: not false }))
		{
			isPrevented = true;
		}
	}
}
