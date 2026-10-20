using System;
using System.Runtime.CompilerServices;
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
using OmenTools.Extensions;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;

namespace DailyRoutines.ModulesPublic;

public class AutoUmbralSoul : ModuleBase
{
	private const uint CLASS_JOB = 25u;

	private const uint UMBRAL_SOUL = 16506u;

	private const uint TRANSPOSE = 149u;

	private const uint BLIZZARD4 = 3576u;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoUmbralSoulTitle"),
		Description = LanguageManager.Get("AutoUmbralSoulDescription", LuminaWrapper.GetJobName(25u), LuminaWrapper.GetActionName(16506u)),
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

	private bool CheckCurrentJob()
	{
		if (IConditionExtension.get_IsBetweenAreas(DalamudServiceExtension.Instance<ICondition>()) || IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()))
		{
			return false;
		}
		if (LocalPlayerState.ClassJob != 25 || !GameState.IsInPVEActonZone)
		{
			base.TaskHelper.Abort();
			return true;
		}
		base.TaskHelper.Enqueue((Func<bool>)UseRelatedActions, "UseRelatedActions", 5000, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 1);
		return true;
	}

	private unsafe bool UseRelatedActions()
	{
		ICondition obj = DalamudServiceExtension.Instance<ICondition>();
		ConditionFlag[] array = new ConditionFlag[4];
		RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
		if (obj.Any((ConditionFlag[])(object)array))
		{
			base.TaskHelper.Abort();
			return true;
		}
		BLMGauge val = DalamudServiceExtension.Instance<IJobGauges>().Get<BLMGauge>();
		BattleChara* localPlayer = Control.GetLocalPlayer();
		if (localPlayer == null)
		{
			return false;
		}
		if (((BattleChara)localPlayer).ClassJob != 25)
		{
			base.TaskHelper.Abort();
			return true;
		}
		if (val.AstralSoulStacks == 6)
		{
			base.TaskHelper.Abort();
			return true;
		}
		uint action = 0u;
		if (ActionManagerExtension.IsActionUnlocked(149u) && val.InAstralFire)
		{
			action = 149u;
		}
		else if (ActionManagerExtension.IsActionUnlocked(16506u) && ((ActionManagerExtension.IsActionUnlocked(3576u) && val.UmbralHearts != 3) || val.UmbralIceStacks != 3))
		{
			action = 16506u;
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
		if (GameState.ContentFinderCondition != 0)
		{
			base.TaskHelper.Abort();
			base.TaskHelper.Enqueue((Func<bool>)CheckCurrentJob, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
		}
	}
}
