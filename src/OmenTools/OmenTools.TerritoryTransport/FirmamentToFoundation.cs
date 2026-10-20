using System;
using System.Collections.Generic;
using System.Numerics;
using OmenTools.Info.Game.Packets.Upstream;
using OmenTools.Interop.Game.AddonEvent;
using OmenTools.OmenService;
using OmenTools.TerritoryTransport.Abstractions;
using OmenTools.Threading.TaskHelper;

namespace OmenTools.TerritoryTransport;

public sealed class FirmamentToFoundation : EventTransportBase
{
	private const uint SOURCE_ZONE = 886u;

	private const uint TARGET_ZONE = 418u;

	private const uint NPC_EVENT_ID = 131343u;

	private const uint EVENT_CATEGORY = 16777216u;

	private const uint EVENT_PARAM = 1u;

	private static readonly Vector3 NPCPosition = new Vector3(13.4f, -15.2f, 180.5f);

	public override string DisplayName => "天穹街 → 伊修加德基础层";

	public override uint SourceZone => 886u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(418u);

	public override Vector3 EventPosition => NPCPosition;

	protected override uint EventID => 131343u;

	protected override void EnqueueEventTrigger(TaskHelper taskHelper, uint targetZone, int weight)
	{
		Action task = delegate
		{
			new EventStartPackt(LocalPlayerState.EntityID, 131343u).Send();
			new EventCompletePackt(131343u, 16777216u, 1u).Send();
		};
		int weight2 = weight;
		taskHelper.Enqueue(task, "发送交互包", 0, null, null, null, null, weight2);
		Func<bool> task2 = delegate
		{
			AddonTalkEvent.ClickNext();
			return AddonSelectYesnoEvent.ClickYes();
		};
		weight2 = weight;
		taskHelper.Enqueue(task2, "点击确认", 0, null, null, null, null, weight2);
	}
}
