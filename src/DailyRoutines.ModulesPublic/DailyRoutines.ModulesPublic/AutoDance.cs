using System.Collections.Frozen;
using System.Numerics;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using OmenTools.Extensions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.ModulesPublic;

public class AutoDance : ModuleBase
{
	private static readonly FrozenSet<uint> DanceActions = FrozenSet.Create<uint>(15997u, 15998u);

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoDanceTitle"),
		Description = LanguageManager.Get("AutoDanceDescription"),
		Category = ModuleCategory.Action
	};

	protected override void Init()
	{
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 5000
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		OmenServiceBase<UseActionManager>.Instance().RegPostUseActionLocation(OnPostUseAction);
	}

	protected override void Uninit()
	{
		OmenServiceBase<UseActionManager>.Instance().Unreg(OnPostUseAction);
		base.TaskHelper?.Abort();
		base.TaskHelper = null;
	}

	private void OnPostUseAction(bool result, ActionType actionType, uint actionID, ulong targetID, Vector3 location, uint extraParam, byte a7)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Invalid comparison between Unknown and I4
		if (!result || (int)actionType != 1 || !DanceActions.Contains(actionID))
		{
			return;
		}
		DNCGauge gauge = DalamudServiceExtension.Instance<IJobGauges>().Get<DNCGauge>();
		if (!gauge.IsDancing)
		{
			base.TaskHelper.Enqueue(() => gauge.IsDancing);
			base.TaskHelper.Enqueue(() => DanceStep(actionID != 15997));
		}
	}

	private unsafe bool DanceStep(bool isTechnicalStep)
	{
		DNCGauge val = DalamudServiceExtension.Instance<IJobGauges>().Get<DNCGauge>();
		if (!val.IsDancing)
		{
			base.TaskHelper.Abort();
			return true;
		}
		if (val.CompletedSteps < (isTechnicalStep ? 4 : 2))
		{
			uint nextStep = val.NextStep;
			if (((ActionManager)ActionManager.Instance()).GetActionStatus((ActionType)1, nextStep, 3758096384uL, true, true, (uint*)null) != 0)
			{
				return false;
			}
			if (OmenServiceBase<UseActionManager>.Instance().UseActionLocation((ActionType)1, nextStep, 3758096384uL, default(Vector3), 0u, 0))
			{
				base.TaskHelper.Enqueue(() => DanceStep(isTechnicalStep));
				return true;
			}
		}
		return false;
	}
}
