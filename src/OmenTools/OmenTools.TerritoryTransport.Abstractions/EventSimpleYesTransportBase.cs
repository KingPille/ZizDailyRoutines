using System;
using OmenTools.Info.Game.Packets.Upstream;
using OmenTools.Interop.Game.AddonEvent;
using OmenTools.OmenService;
using OmenTools.Threading.TaskHelper;

namespace OmenTools.TerritoryTransport.Abstractions;

public abstract class EventSimpleYesTransportBase : EventTransportBase
{
	protected override void EnqueueEventTrigger(TaskHelper taskHelper, uint targetZone, int weight)
	{
		Action task = delegate
		{
			new EventStartPackt(LocalPlayerState.EntityID, EventID).Send();
		};
		int weight2 = weight;
		taskHelper.Enqueue(task, "发送交互包", 0, null, null, null, null, weight2);
		Func<bool> task2 = delegate
		{
			AddonTalkEvent.ClickNext();
			return AddonSelectYesnoEvent.ClickYes();
		};
		weight2 = weight;
		taskHelper.Enqueue(task2, "进入目标区域", 0, null, null, null, null, weight2);
	}
}
