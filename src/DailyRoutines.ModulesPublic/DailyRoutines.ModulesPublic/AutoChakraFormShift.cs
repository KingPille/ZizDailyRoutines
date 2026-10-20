using System;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.DutyState;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using Lumina.Excel.Sheets;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;

namespace DailyRoutines.ModulesPublic;

public class AutoChakraFormShift : ModuleBase
{
	private const uint STEELED_MEDITATION = 36940u;

	private const uint FORM_SHIFT = 4262u;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoChakraFormShiftTitle"),
		Description = LanguageManager.Get("AutoChakraFormShiftDescription"),
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

	private bool CheckCurrentJob()
	{
		if (IConditionExtension.get_IsBetweenAreas(DalamudServiceExtension.Instance<ICondition>()) || IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()))
		{
			return false;
		}
		if (LocalPlayerState.ClassJob != 20 || !GameState.IsInPVEActonZone)
		{
			base.TaskHelper.Abort();
			return true;
		}
		base.TaskHelper.Enqueue((Func<bool>)UseRelatedActions, "UseRelatedActions", 5000, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 1);
		return true;
	}

	private unsafe bool UseRelatedActions()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		MNKGauge val = DalamudServiceExtension.Instance<IJobGauges>().Get<MNKGauge>();
		BattleChara* localPlayer = Control.GetLocalPlayer();
		if (localPlayer == null)
		{
			return false;
		}
		StatusManager statusManager = ((BattleChara)localPlayer).StatusManager;
		uint action = 0u;
		int index;
		int index2;
		if (ActionManagerExtension.IsActionUnlocked(36940u) && val.Chakra != 5)
		{
			action = 36940u;
		}
		else if (ActionManagerExtension.IsActionUnlocked(4262u) && !LocalPlayerState.HasStatus(110u, out index) && (!LocalPlayerState.HasStatus(2513u, out index2) || ((StatusManager)(ref statusManager)).GetRemainingTime(index2) <= 27f))
		{
			action = 4262u;
		}
		if (action == 0)
		{
			base.TaskHelper.Abort();
			return true;
		}
		base.TaskHelper.Enqueue(() => OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)1, action, 3758096384uL, 0u, (UseActionMode)0), $"UseAction_{action}", 2000, null, null, null, null, 1);
		base.TaskHelper.DelayNext(500, $"Delay_Use{action}", 1);
		base.TaskHelper.Enqueue((Func<bool>)UseRelatedActions, "UseRelatedActions", 5000, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 1);
		return true;
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

	private void OnDutyRecommenced(IDutyStateEventArgs args)
	{
		base.TaskHelper.Abort();
		base.TaskHelper.Enqueue((Func<bool>)CheckCurrentJob, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
	}

	private void OnZoneChanged(uint zone)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		TerritoryType? row = LuminaGetter.GetRow<TerritoryType>(zone);
		if (row.HasValue)
		{
			TerritoryType valueOrDefault = row.GetValueOrDefault();
			if (((TerritoryType)(ref valueOrDefault)).ContentFinderCondition.RowId != 0)
			{
				base.TaskHelper.Abort();
				base.TaskHelper.Enqueue((Func<bool>)CheckCurrentJob, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
			}
		}
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
}
