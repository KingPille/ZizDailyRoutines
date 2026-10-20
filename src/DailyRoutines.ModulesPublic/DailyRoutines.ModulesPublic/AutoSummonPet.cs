using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.DutyState;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;

namespace DailyRoutines.ModulesPublic;

public class AutoSummonPet : ModuleBase
{
	private static readonly FrozenDictionary<uint, uint> SummonActions = new Dictionary<uint, uint>
	{
		[28u] = 17215u,
		[26u] = 25798u,
		[27u] = 25798u
	}.ToFrozenDictionary();

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoSummonPetTitle"),
		Description = LanguageManager.Get("AutoSummonPetDescription"),
		Category = ModuleCategory.Action
	};

	protected override void Init()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected O, but got Unknown
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
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IDutyState>().DutyRecommenced -= new DutyRecommencedDelegate(OnDutyRecommenced);
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChanged;
	}

	private void OnDutyRecommenced(IDutyStateEventArgs args)
	{
		base.TaskHelper.Abort();
		base.TaskHelper.Enqueue((Func<bool>)CheckCurrentJob, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
	}

	private void OnZoneChanged(uint u)
	{
		base.TaskHelper.Abort();
		if (GameState.IsInPVEActonZone)
		{
			base.TaskHelper.DelayNext(1000);
			base.TaskHelper.Enqueue((Func<bool>)CheckCurrentJob, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
		}
	}

	private unsafe bool CheckCurrentJob()
	{
		if (!IConditionExtension.get_IsBetweenAreas(DalamudServiceExtension.Instance<ICondition>()) && UIModuleExtension.IsScreenReady() && !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)27])
		{
			IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
			if (localPlayer != null && localPlayer.IsTargetable)
			{
				if (!SummonActions.TryGetValue(LocalPlayerState.ClassJob, out var actionID))
				{
					base.TaskHelper.Abort();
					return true;
				}
				if (((CharacterManager)CharacterManager.Instance()).LookupPetByOwnerObject(localPlayer.ToStruct()) != null)
				{
					base.TaskHelper.Abort();
					return true;
				}
				base.TaskHelper.Enqueue(() => OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)1, actionID, 3758096384uL, 0u, (UseActionMode)0));
				base.TaskHelper.DelayNext(1000);
				base.TaskHelper.Enqueue((Func<bool>)CheckCurrentJob, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
				return true;
			}
		}
		return false;
	}
}
