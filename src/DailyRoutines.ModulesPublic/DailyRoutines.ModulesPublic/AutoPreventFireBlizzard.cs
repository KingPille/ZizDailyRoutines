using System;
using System.Numerics;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.Gui.Toast;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;

namespace DailyRoutines.ModulesPublic;

public class AutoPreventFireBlizzard : ModuleBase
{
	private const uint ACITON_BLIZZARD = 142u;

	private const uint ACITON_FIRE = 141u;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoPreventFireBlizzardTitle"),
		Description = LanguageManager.Get("AutoPreventFireBlizzardDescription", LuminaWrapper.GetJobName(25u), LuminaWrapper.GetStatusName(3212u), LuminaWrapper.GetActionName(142u), LuminaWrapper.GetStatusName(3214u), LuminaWrapper.GetActionName(141u)),
		Category = ModuleCategory.Action
	};

	protected override void Init()
	{
		OmenServiceBase<UseActionManager>.Instance().RegPreUseActionLocation(OnUseAction);
	}

	protected override void Uninit()
	{
		OmenServiceBase<UseActionManager>.Instance().Unreg(OnUseAction);
	}

	private unsafe static void OnUseAction(ref bool isPrevented, ref ActionType type, ref uint actionID, ref ulong targetID, ref Vector3 location, ref uint extraParam, ref byte a7)
	{
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Expected O, but got Unknown
		if ((int)type != 1)
		{
			return;
		}
		uint num = actionID;
		if (num - 141 > 1)
		{
			return;
		}
		BLMGauge val = DalamudServiceExtension.Instance<IJobGauges>().Get<BLMGauge>();
		if ((!val.InAstralFire || actionID != 142) && (!val.InUmbralIce || actionID != 141))
		{
			return;
		}
		isPrevented = true;
		if (!Throttler.Shared.Throttle($"AutoPreventFireBlizzard.Notification.{actionID}", 2500u))
		{
			return;
		}
		RentedSeStringBuilder val2 = default(RentedSeStringBuilder);
		((RentedSeStringBuilder)(ref val2))._002Ector();
		try
		{
			((RentedSeStringBuilder)(ref val2)).Builder.PushEdgeColorType(32u).Append(LuminaWrapper.GetActionName(actionID)).PopEdgeColorType();
			NotifyHelper.ToastQuest(LanguageManager.GetSe("AutoPreventFireBlizzard-Notification", val2), new QuestToastOptions
			{
				IconId = LuminaWrapper.GetActionIconID(actionID)
			});
		}
		finally
		{
			((IDisposable)(*(RentedSeStringBuilder*)(&val2))/*cast due to constrained. prefix*/).Dispose();
		}
	}
}
