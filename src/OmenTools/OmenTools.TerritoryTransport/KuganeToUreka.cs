using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Numerics;
using OmenTools.Info.Game.Packets.Upstream;
using OmenTools.Interop.Game.AddonEvent;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.TerritoryTransport.Abstractions;
using OmenTools.Threading.TaskHelper;

namespace OmenTools.TerritoryTransport;

public sealed class KuganeToUreka : EventTransportBase
{
	private const uint SOURCE_ZONE = 628u;

	private const uint NPC_EVENT_ID = 721355u;

	private static readonly FrozenDictionary<uint, uint> ZoneToContent = new Dictionary<uint, uint>
	{
		[732u] = 283u,
		[763u] = 581u,
		[795u] = 598u,
		[827u] = 639u
	}.ToFrozenDictionary();

	private static readonly Vector3 NPCPosition = new Vector3(-114.3f, -5f, 150f);

	public override string DisplayName => "黄金港 → 禁地优雷卡";

	public override uint SourceZone => 628u;

	public override IReadOnlyList<uint> TargetZones => ZoneToContent.Keys;

	public override Vector3 EventPosition => NPCPosition;

	protected override uint EventID => 721355u;

	protected override void EnqueueEventTrigger(TaskHelper taskHelper, uint targetZone, int weight)
	{
		if (ZoneToContent.TryGetValue(targetZone, out var contentID))
		{
			Action task = delegate
			{
				new EventStartPackt(LocalPlayerState.EntityID, 721355u).Send();
			};
			int weight2 = weight;
			taskHelper.Enqueue(task, "发送交互包", 0, null, null, null, null, weight2);
			Func<bool> task2 = delegate
			{
				AddonTalkEvent.ClickNext();
				return AddonSelectStringEvent.Select(LuminaWrapper.GetContentName(contentID));
			};
			weight2 = weight;
			taskHelper.Enqueue(task2, "选择副本", 0, null, null, null, null, weight2);
			Func<bool> task3 = delegate
			{
				AddonTalkEvent.ClickNext();
				return AddonSelectYesnoEvent.ClickYes();
			};
			weight2 = weight;
			taskHelper.Enqueue(task3, "确认进入副本", 0, null, null, null, null, weight2);
		}
	}
}
