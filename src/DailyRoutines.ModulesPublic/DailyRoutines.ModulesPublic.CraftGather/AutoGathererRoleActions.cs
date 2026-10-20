using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using OmenTools.Extensions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.ModulesPublic.CraftGather;

public class AutoGathererRoleActions : ModuleBase
{
	private static readonly uint[] ValidJobs = new uint[3] { 16u, 17u, 18u };

	private static readonly List<(uint Action, uint Status)> Actions;

	private static readonly uint[] HomeOnlyActions;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoGathererRoleActionsTitle"),
		Description = LanguageManager.Get("AutoGathererRoleActionsDescription"),
		Category = ModuleCategory.CraftGather
	};

	protected override void Init()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		base.TaskHelper = new TaskHelper
		{
			TimeoutMS = 5000,
			RetryIntervalMS = 500
		};
		DalamudServiceExtension.Instance<IClientState>().ClassJobChanged += new ClassJobChangeDelegate(OnJobChanged);
		OnJobChanged(LocalPlayerState.ClassJob);
	}

	protected override void Uninit()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IClientState>().ClassJobChanged -= new ClassJobChangeDelegate(OnJobChanged);
	}

	private unsafe void OnJobChanged(uint jobID)
	{
		base.TaskHelper.Abort();
		if (!ValidJobs.Contains(jobID))
		{
			return;
		}
		BattleChara* localPlayer = Control.GetLocalPlayer();
		if (localPlayer == null)
		{
			return;
		}
		base.TaskHelper.DelayNext(500);
		base.TaskHelper.Enqueue(delegate
		{
			foreach (var action2 in Actions)
			{
				var (action, status) = action2;
				if ((!HomeOnlyActions.Contains(action) || GameState.HomeWorld == GameState.CurrentWorld) && !((StatusManager)(&((BattleChara)localPlayer).StatusManager)).HasStatus(status, 3758096384u))
				{
					base.TaskHelper.Enqueue(delegate
					{
						if (IConditionExtension.get_IsBetweenAreas(DalamudServiceExtension.Instance<ICondition>()))
						{
							base.TaskHelper.Abort();
							return true;
						}
						if (((StatusManager)(&((BattleChara)localPlayer).StatusManager)).HasStatus(status, 3758096384u) || !ActionManagerExtension.IsActionUnlocked(action))
						{
							return true;
						}
						OmenServiceBase<UseActionManager>.Instance().UseActionLocation((ActionType)1, action, 3758096384uL, default(Vector3), 0u, 0);
						return false;
					});
				}
			}
		});
	}

	static AutoGathererRoleActions()
	{
		int num = 6;
		List<(uint, uint)> list = new List<(uint, uint)>(num);
		CollectionsMarshal.SetCount(list, num);
		Span<(uint, uint)> span = CollectionsMarshal.AsSpan(list);
		span[0] = (227u, 225u);
		span[1] = (210u, 217u);
		span[2] = (238u, 222u);
		span[3] = (221u, 221u);
		span[4] = (7903u, 1166u);
		span[5] = (7911u, 1173u);
		Actions = list;
		HomeOnlyActions = new uint[2] { 221u, 238u };
	}
}
