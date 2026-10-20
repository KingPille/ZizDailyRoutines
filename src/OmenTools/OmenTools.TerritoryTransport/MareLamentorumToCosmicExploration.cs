using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using OmenTools.Info.Game.Packets.Upstream;
using OmenTools.Interop.Game.AddonEvent;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.TerritoryTransport.Abstractions;
using OmenTools.Threading.TaskHelper;

namespace OmenTools.TerritoryTransport;

public sealed class MareLamentorumToCosmicExploration : EventTransportBase
{
	private uint redirectTargetZone;

	private const uint SOURCE_ZONE = 959u;

	private const uint NPC_EVENT_ID = 327855u;

	private const uint CATEGORY_TARGET_SELECT = 33554432u;

	private const uint CATEGORY_TARGET_CONFIRM = 16777316u;

	private static readonly Vector3 NPCPosition = new Vector3(-5.3f, -131.1f, -504f);

	private static readonly FrozenSet<uint> Zones = FrozenSet.Create<uint>(1237u, 1291u, 1310u, 1319u);

	public override string DisplayName => "叹息海 → 宇宙探索";

	public override uint SourceZone => 959u;

	public override IReadOnlyList<uint> TargetZones => new global::_003C_003Ez__ReadOnlyArray<uint>(Zones.ToArray());

	public override Vector3 EventPosition => NPCPosition;

	protected override uint EventID => 327855u;

	public MareLamentorumToCosmicExploration()
	{
		OmenServiceBase<GamePacketManager>.Instance().RegPreSendPacket(OnPreSendPacket);
	}

	public override void Cleanup()
	{
		OmenServiceBase<GamePacketManager>.Instance().Unreg(OnPreSendPacket);
	}

	protected override void EnqueueEventTrigger(TaskHelper taskHelper, uint targetZone, int weight)
	{
		Action task = delegate
		{
			redirectTargetZone = targetZone;
		};
		int weight2 = weight;
		taskHelper.Enqueue(task, "赋值目标区域", 0, null, null, null, null, weight2);
		Action task2 = delegate
		{
			new EventStartPackt(LocalPlayerState.EntityID, 327855u).Send();
		};
		weight2 = weight;
		taskHelper.Enqueue(task2, "发送交互包", 0, null, null, null, null, weight2);
		Func<bool> task3 = () => AddonSelectStringEvent.Select(3);
		weight2 = weight;
		taskHelper.Enqueue(task3, "进入宇宙探索区域", 0, null, null, null, null, weight2);
	}

	private unsafe void OnPreSendPacket(ref bool isPrevented, int opcode, ref nint packet, ref bool isPrioritize)
	{
		if (opcode != UpstreamOpcode.EventCompleteOpcode || GameState.TerritoryType != 959 || redirectTargetZone == 0)
		{
			return;
		}
		EventCompletePackt* ptr = (EventCompletePackt*)packet;
		if (ptr->EventID == 327855)
		{
			if (ptr->Category == 33554432)
			{
				ptr->Param1 = redirectTargetZone;
			}
			if (ptr->Category == 16777316)
			{
				ptr->Param0 = redirectTargetZone;
				redirectTargetZone = 0u;
			}
		}
	}
}
