using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.DutyState;
using Dalamud.Interface;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.Sheets;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.ImGuiOm;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;

namespace DailyRoutines.ModulesPublic;

public class AutoTankStance : ModuleBase
{
	private class Config : ModuleConfig
	{
		public bool OnlyAutoStanceWhenOneTank = true;
	}

	private Config config;

	private static readonly FrozenDictionary<uint, (uint Action, uint Status)> TankStanceActions = new Dictionary<uint, (uint, uint)>
	{
		[1u] = (28u, 79u),
		[19u] = (28u, 79u),
		[3u] = (48u, 91u),
		[21u] = (48u, 91u),
		[32u] = (3629u, 743u),
		[37u] = (16142u, 1833u)
	}.ToFrozenDictionary();

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoTankStanceTitle"),
		Description = LanguageManager.Get("AutoTankStanceDescription"),
		Category = ModuleCategory.Action
	};

	protected override void Init()
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Expected O, but got Unknown
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
	}

	protected override void Uninit()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChanged;
		DalamudServiceExtension.Instance<IDutyState>().DutyRecommenced -= new DutyRecommencedDelegate(OnDutyRecommenced);
	}

	protected override void ConfigUI()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("AutoTankStance-OnlyAutoStanceWhenOneTank")), ref config.OnlyAutoStanceWhenOneTank))
		{
			config.Save(this);
		}
		ImGuiOm.HelpMarker(LanguageManager.Get("AutoTankStance-OnlyAutoStanceWhenOneTankHelp"), 40f, (FontAwesomeIcon)61530);
	}

	private void OnZoneChanged(uint u)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		base.TaskHelper.Abort();
		if (!GameState.IsInPVEActonZone)
		{
			return;
		}
		if (config.OnlyAutoStanceWhenOneTank)
		{
			ContentFinderCondition contentFinderConditionData = GameState.ContentFinderConditionData;
			ContentMemberType value = ((ContentFinderCondition)(ref contentFinderConditionData)).ContentMemberType.Value;
			if (((ContentMemberType)(ref value)).TanksPerParty != 1)
			{
				return;
			}
		}
		base.TaskHelper.DelayNext(1000);
		base.TaskHelper.Enqueue((Func<bool>)CheckCurrentJob, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
	}

	private void OnDutyRecommenced(IDutyStateEventArgs args)
	{
		base.TaskHelper.Abort();
		base.TaskHelper.Enqueue((Func<bool>)CheckCurrentJob, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
	}

	private static bool CheckCurrentJob()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (IConditionExtension.get_IsBetweenAreas(DalamudServiceExtension.Instance<ICondition>()) || IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()) || !UIModuleExtension.IsScreenReady())
		{
			return false;
		}
		IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
		if (localPlayer != null)
		{
			uint rowId = localPlayer.ClassJob.RowId;
			if (localPlayer.IsTargetable && rowId != 0)
			{
				if (!TankStanceActions.TryGetValue(rowId, out (uint, uint) value))
				{
					return true;
				}
				if (LocalPlayerState.HasStatus(value.Item2, out var _))
				{
					return true;
				}
				return OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)1, value.Item1, 3758096384uL, 0u, (UseActionMode)0);
			}
		}
		return false;
	}
}
