using System.Collections.Generic;
using System.Numerics;
using OmenTools.Info.Game.Packets.Upstream;
using OmenTools.OmenService;
using OmenTools.TerritoryTransport.Abstractions;
using OmenTools.Threading.TaskHelper;

namespace OmenTools.TerritoryTransport;

public sealed class FoundationToFirmament : EventTransportBase
{
	private const uint SOURCE_ZONE = 418u;

	private const uint TARGET_ZONE = 886u;

	private const uint NPC_EVENT_ID = 327750u;

	private const uint EVENT_CATEGORY = 16777216u;

	private const uint EVENT_PARAM = 10u;

	private static readonly Vector3 AetherytePosition = new Vector3(-71f, 8f, 38f);

	public override string DisplayName => "伊修加德基础层 → 天穹街";

	public override uint SourceZone => 418u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(886u);

	public override Vector3 EventPosition => AetherytePosition;

	protected override uint EventID => 327750u;

	protected override void EnqueueEventTrigger(TaskHelper taskHelper, uint targetZone, int weight)
	{
		taskHelper.Enqueue(delegate
		{
			new EventStartPackt(LocalPlayerState.EntityID, 327750u).Send();
			new EventCompletePackt(327750u, 16777216u, 10u).Send();
		}, "进入目标区域", 0, null, null, null, null, weight);
	}
}
