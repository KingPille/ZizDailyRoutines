using System.Numerics;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using FFXIVClientStructs.FFXIV.Client.Game;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic.CraftGather;

public class AutoBlockAgelessWordsAndSolidReason : ModuleBase
{
	private const uint SOLID_REASON_ACTION_ID = 232u;

	private const uint AGELESS_WORDS_ACTION_ID = 215u;

	private const uint EUREKA_MOMENT_STATUS_ID = 2765u;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoBlockAgelessWordsAndSolidReasonTitle"),
		Description = LanguageManager.Get("AutoBlockAgelessWordsAndSolidReasonDescription"),
		Category = ModuleCategory.CraftGather
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected override void Init()
	{
		OmenServiceBase<UseActionManager>.Instance().RegPreUseActionLocation(OnUseAction);
	}

	protected override void Uninit()
	{
		OmenServiceBase<UseActionManager>.Instance().Unreg(OnUseAction);
	}

	private static void OnUseAction(ref bool isPrevented, ref ActionType type, ref uint actionID, ref ulong targetID, ref Vector3 location, ref uint extraParam, ref byte a7)
	{
		if ((int)type == 1 && (actionID == 232 || actionID == 215) && LocalPlayerState.HasStatus(2765u, out var _))
		{
			isPrevented = true;
			NotifyHelper.ToastError(LanguageManager.Get("AutoBlockAgelessWordsAndSolidReason-Notification-Blocked", LuminaWrapper.GetStatusName(2765u), LuminaWrapper.GetActionName(actionID)));
		}
	}
}
