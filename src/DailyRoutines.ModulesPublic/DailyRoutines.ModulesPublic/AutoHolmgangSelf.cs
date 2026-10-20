using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using FFXIVClientStructs.FFXIV.Client.Game;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class AutoHolmgangSelf : ModuleBase
{
	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoHolmgangSelfTitle"),
		Description = LanguageManager.Get("AutoHolmgangSelfDescription"),
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
		if ((int)actionType == 1 && actionID == 43)
		{
			targetID = 3758096384uL;
		}
	}
}
