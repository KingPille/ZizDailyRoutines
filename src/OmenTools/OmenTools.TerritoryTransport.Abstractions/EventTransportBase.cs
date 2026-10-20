using System;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using OmenTools.Extensions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;

namespace OmenTools.TerritoryTransport.Abstractions;

public abstract class EventTransportBase
{
	public abstract uint SourceZone { get; }

	public abstract IReadOnlyList<uint> TargetZones { get; }

	public abstract string DisplayName { get; }

	public abstract Vector3 EventPosition { get; }

	protected abstract uint EventID { get; }

	public virtual uint Cost => 0u;

	public unsafe bool IsEventNearby()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return (*EventFramework.Instance()).IsEventIDNearby(EventId.op_Implicit(EventID));
	}

	public IEnumerable<uint> EnumerateReachableTargets(uint source)
	{
		if (source != SourceZone)
		{
			return Array.Empty<uint>();
		}
		return TargetZones;
	}

	public void Enqueue(TaskHelper taskHelper, uint targetZone, int weight)
	{
		EnqueueDismount(taskHelper, weight);
		EnqueuePreEventAction(taskHelper, targetZone, weight);
		EnqueueEventTrigger(taskHelper, targetZone, weight);
		taskHelper.Enqueue(WaitForZoneReady(targetZone), $"等待进入目标区域: {targetZone}", 0, null, null, null, null, weight);
		EnqueuePostEventAction(taskHelper, targetZone, weight);
	}

	public virtual void Cleanup()
	{
	}

	protected static Func<bool> WaitForZoneReady(uint zone)
	{
		return () => GameState.TerritoryType == zone && LocalPlayerState.Object != null;
	}

	private static void EnqueueDismount(TaskHelper taskHelper, int weight)
	{
		taskHelper.Enqueue(delegate
		{
			if (!DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4])
			{
				return true;
			}
			OmenServiceBase<MovementManager>.Instance().Dismount(restoreControlMode: false);
			return false;
		}, "下坐骑", 0, null, null, null, null, weight);
	}

	protected virtual void EnqueuePreEventAction(TaskHelper taskHelper, uint target, int weight)
	{
	}

	protected abstract void EnqueueEventTrigger(TaskHelper taskHelper, uint targetZone, int weight);

	protected virtual void EnqueuePostEventAction(TaskHelper taskHelper, uint target, int weight)
	{
	}
}
