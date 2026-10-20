using System;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.DutyState;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Info.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;

namespace DailyRoutines.ModulesPublic;

public class AutoDrawMotifs : ModuleBase
{
	private class Config : ModuleConfig
	{
		public bool DrawWhenOutOfCombat;
	}

	private Config config;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoDrawMotifsTitle"),
		Description = LanguageManager.Get("AutoDrawMotifsDescription"),
		Category = ModuleCategory.Action
	};

	protected override void Init()
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Expected O, but got Unknown
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Expected O, but got Unknown
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
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
		DalamudServiceExtension.Instance<IDutyState>().DutyCompleted += new DutyCompletedDelegate(OnDutyCompleted);
	}

	protected override void Uninit()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChanged;
		DalamudServiceExtension.Instance<IDutyState>().DutyRecommenced -= new DutyRecommencedDelegate(OnDutyRecommenced);
		DalamudServiceExtension.Instance<ICondition>().ConditionChange -= new ConditionChangeDelegate(OnConditionChanged);
		DalamudServiceExtension.Instance<IDutyState>().DutyCompleted -= new DutyCompletedDelegate(OnDutyCompleted);
	}

	protected override void ConfigUI()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("AutoDrawMotifs-DrawWhenOutOfCombat")), ref config.DrawWhenOutOfCombat))
		{
			config.Save(this);
		}
	}

	private void OnConditionChanged(ConditionFlag flag, bool value)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Invalid comparison between Unknown and I4
		if ((int)flag == 26)
		{
			base.TaskHelper.Abort();
			if (!value && config.DrawWhenOutOfCombat)
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

	private void OnDutyCompleted(IDutyStateEventArgs args)
	{
		base.TaskHelper.Abort();
	}

	private void OnZoneChanged(uint zone)
	{
		base.TaskHelper.Abort();
		if (Sheets.Contents.ContainsKey(zone))
		{
			base.TaskHelper.Enqueue((Func<bool>)CheckCurrentJob, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
		}
	}

	private bool CheckCurrentJob()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (IConditionExtension.get_IsBetweenAreas(DalamudServiceExtension.Instance<ICondition>()) || IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()))
		{
			return false;
		}
		IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
		if (localPlayer == null || localPlayer.ClassJob.RowId != 42 || localPlayer.Level < 30 || !GameState.IsInPVEActonZone)
		{
			base.TaskHelper.Abort();
			return true;
		}
		base.TaskHelper.Enqueue((Func<bool>)DrawNeededMotif, "DrawNeededMotif", 5000, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 1);
		return true;
	}

	private bool DrawNeededMotif()
	{
		PCTGauge val = DalamudServiceExtension.Instance<IJobGauges>().Get<PCTGauge>();
		if (IOmenDalamudService<IObjectTable>.Instance().LocalPlayer == null || IConditionExtension.get_IsBetweenAreas(DalamudServiceExtension.Instance<ICondition>()) || DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)27] || IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()))
		{
			return false;
		}
		ICondition obj = DalamudServiceExtension.Instance<ICondition>();
		ConditionFlag[] array = new ConditionFlag[4];
		RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
		if (obj.Any((ConditionFlag[])(object)array))
		{
			base.TaskHelper.Abort();
			return true;
		}
		uint motifAction = 0u;
		int index;
		if (!val.CreatureMotifDrawn && ActionManagerExtension.IsActionUnlocked(34689u))
		{
			motifAction = 34689u;
		}
		else if (!val.WeaponMotifDrawn && ActionManagerExtension.IsActionUnlocked(34690u) && !LocalPlayerState.HasStatus(3680u, out index))
		{
			motifAction = 34690u;
		}
		else if (!val.LandscapeMotifDrawn && ActionManagerExtension.IsActionUnlocked(34691u))
		{
			motifAction = 34691u;
		}
		if (motifAction == 0)
		{
			base.TaskHelper.Abort();
			return true;
		}
		base.TaskHelper.Enqueue(() => OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)1, motifAction, 3758096384uL, 0u, (UseActionMode)0), $"UseAction_{motifAction}", 2000, null, null, null, null, 1);
		base.TaskHelper.DelayNext(500, $"DrawMotif_{motifAction}", 1);
		base.TaskHelper.Enqueue((Func<bool>)DrawNeededMotif, "DrawNeededMotif", 5000, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 1);
		return true;
	}
}
