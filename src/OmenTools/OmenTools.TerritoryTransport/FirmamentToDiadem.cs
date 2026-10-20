using System;
using System.Collections.Generic;
using System.Numerics;
using OmenTools.Info.Game.Packets.Upstream;
using OmenTools.Interop.Game.AddonEvent;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.TerritoryTransport.Abstractions;
using OmenTools.Threading.TaskHelper;

namespace OmenTools.TerritoryTransport;

public sealed class FirmamentToDiadem : EventTransportBase
{
	private const uint SOURCE_ZONE = 886u;

	private const uint TARGET_ZONE = 939u;

	private const uint NPC_EVENT_ID = 721532u;

	private const uint DIAMDEM_CONTENT_ID = 753u;

	private static readonly Vector3 NPCPosition = new Vector3(-19.6f, -16f, 143f);

	private static readonly string DutyName = LuminaWrapper.GetContentName(753u);

	public override string DisplayName => "天穹街 → 天上福地云冠群岛";

	public override uint SourceZone => 886u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(939u);

	public override Vector3 EventPosition => NPCPosition;

	protected override uint EventID => 721532u;

	protected override void EnqueueEventTrigger(TaskHelper taskHelper, uint targetZone, int weight)
	{
		Action task = delegate
		{
			new EventStartPackt(LocalPlayerState.EntityID, 721532u).Send();
		};
		int weight2 = weight;
		taskHelper.Enqueue(task, "发送交互包", 0, null, null, null, null, weight2);
		Func<bool> task2 = delegate
		{
			AddonTalkEvent.ClickNext();
			return AddonSelectStringEvent.Select(DutyName);
		};
		weight2 = weight;
		taskHelper.Enqueue(task2, "选择副本", 0, null, null, null, null, weight2);
		Func<bool> task3 = delegate
		{
			AddonTalkEvent.ClickNext();
			return AddonSelectYesnoEvent.ClickYes();
		};
		weight2 = weight;
		taskHelper.Enqueue(task3, "点击确认进入副本", 0, null, null, null, null, weight2);
	}
}
