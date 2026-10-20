using System;
using System.Collections.Generic;
using System.Numerics;
using OmenTools.Info.Game.Packets.Upstream;
using OmenTools.Interop.Game.AddonEvent;
using OmenTools.OmenService;
using OmenTools.TerritoryTransport.Abstractions;
using OmenTools.Threading.TaskHelper;

namespace OmenTools.TerritoryTransport;

public sealed class SinusArdorumToMareLamentorum : EventTransportBase
{
	private const uint SOURCE_ZONE = 1237u;

	private const uint TARGET_ZONE = 959u;

	private const uint NPC_EVENT_ID = 721818u;

	private static readonly Vector3 NPCPosition = new Vector3(-55.3f, -0f, -69f);

	public override string DisplayName => "憧憬湾 → 叹息海";

	public override uint SourceZone => 1237u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(959u);

	public override Vector3 EventPosition => NPCPosition;

	protected override uint EventID => 721818u;

	protected override void EnqueueEventTrigger(TaskHelper taskHelper, uint targetZone, int weight)
	{
		Action task = delegate
		{
			new EventStartPackt(LocalPlayerState.EntityID, 721818u).Send();
		};
		int weight2 = weight;
		taskHelper.Enqueue(task, "发送交互包", 0, null, null, null, null, weight2);
		Func<bool> task2 = delegate
		{
			AddonTalkEvent.ClickNext();
			return AddonSelectStringEvent.Select(0);
		};
		weight2 = weight;
		taskHelper.Enqueue(task2, "进入叹息海", 0, null, null, null, null, weight2);
	}
}
