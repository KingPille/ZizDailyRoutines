using System;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.DutyState;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;

namespace DailyRoutines.ModulesPublic;

public class AutoSoulsow : ModuleBase
{
	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoSoulsowTitle"),
		Description = LanguageManager.Get("AutoSoulsowDescription"),
		Category = ModuleCategory.Action
	};

	protected override void Init()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected O, but got Unknown
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 30000
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += OnZoneChanged;
		DalamudServiceExtension.Instance<IDutyState>().DutyRecommenced += new DutyRecommencedDelegate(OnDutyRecommenced);
		DalamudServiceExtension.Instance<ICondition>().ConditionChange += new ConditionChangeDelegate(OnConditionChanged);
	}

	protected override void Uninit()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChanged;
		DalamudServiceExtension.Instance<IDutyState>().DutyRecommenced -= new DutyRecommencedDelegate(OnDutyRecommenced);
		DalamudServiceExtension.Instance<ICondition>().ConditionChange -= new ConditionChangeDelegate(OnConditionChanged);
	}

	private void OnDutyRecommenced(IDutyStateEventArgs args)
	{
		base.TaskHelper.Abort();
		base.TaskHelper.Enqueue((Func<bool>)CheckCurrentJob, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
	}

	private void OnZoneChanged(uint u)
	{
		base.TaskHelper.Abort();
		if (GameState.ContentFinderCondition != 0)
		{
			base.TaskHelper.Enqueue((Func<bool>)CheckCurrentJob, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
		}
	}

	private void OnConditionChanged(ConditionFlag flag, bool value)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Invalid comparison between Unknown and I4
		if ((int)flag == 26)
		{
			base.TaskHelper.Abort();
			if (!value)
			{
				base.TaskHelper.Enqueue((Func<bool>)CheckCurrentJob, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
			}
		}
	}

	private bool CheckCurrentJob()
	{
		if (IConditionExtension.get_IsBetweenAreas(DalamudServiceExtension.Instance<ICondition>()) || !UIModuleExtension.IsScreenReady() || IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()))
		{
			return false;
		}
		if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)26] || LocalPlayerState.ClassJob != 39 || !GameState.IsInPVEActonZone)
		{
			base.TaskHelper.Abort();
			return true;
		}
		base.TaskHelper.Enqueue((Func<bool>)UseRelatedActions, "UseRelatedActions", 5000, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 1);
		return true;
	}

	private bool UseRelatedActions()
	{
		IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
		if (localPlayer == null)
		{
			return false;
		}
		if (localPlayer.StatusList.HasStatus(2594u) || !ActionManagerExtension.IsActionUnlocked(24387u))
		{
			base.TaskHelper.Abort();
			return true;
		}
		base.TaskHelper.Enqueue(() => OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)1, 24387u, 3758096384uL, 0u, (UseActionMode)0), $"UseAction_{24387}", 5000, null, null, null, null, 1);
		base.TaskHelper.DelayNext(2000);
		base.TaskHelper.Enqueue((Func<bool>)CheckCurrentJob, "二次检查", 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 1);
		return true;
	}
}
