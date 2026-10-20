using System;
using System.Collections.Generic;
using System.Numerics;
using OmenTools.Info.Game.Packets.Upstream;
using OmenTools.Interop.Game.AddonEvent;
using OmenTools.OmenService;
using OmenTools.TerritoryTransport.Abstractions;
using OmenTools.Threading.TaskHelper;

namespace OmenTools.TerritoryTransport;

public sealed class LowerLaNosceaToUnnamedIsland : EventTransportBase
{
	private const uint SOURCE_ZONE = 135u;

	private const uint TARGET_ZONE = 1055u;

	private const uint NPC_EVENT_ID = 721694u;

	private static readonly Vector3 NPCPosition = new Vector3(172f, 12f, 642f);

	public override string DisplayName => "拉诺西亚低地 → 无名岛";

	public override uint SourceZone => 135u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(1055u);

	public override Vector3 EventPosition => NPCPosition;

	protected override uint EventID => 721694u;

	protected override void EnqueueEventTrigger(TaskHelper taskHelper, uint targetZone, int weight)
	{
		Action task = delegate
		{
			new EventStartPackt(LocalPlayerState.EntityID, 721694u).Send();
		};
		int weight2 = weight;
		taskHelper.Enqueue(task, "发送交互包", 0, null, null, null, null, weight2);
		Func<bool> task2 = delegate
		{
			AddonTalkEvent.ClickNext();
			return AddonSelectStringEvent.Select(0);
		};
		weight2 = weight;
		taskHelper.Enqueue(task2, "选择进入无人岛", 0, null, null, null, null, weight2);
		Func<bool> task3 = delegate
		{
			AddonTalkEvent.ClickNext();
			return AddonSelectYesnoEvent.ClickYes();
		};
		weight2 = weight;
		taskHelper.Enqueue(task3, "点击确认进入无人岛", 0, null, null, null, null, weight2);
	}
}
