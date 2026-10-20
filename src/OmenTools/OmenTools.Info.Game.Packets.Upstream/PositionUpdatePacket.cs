using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using OmenTools.Info.Game.Packets.Abstractions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Info.Game.Packets.Upstream;

[StructLayout(LayoutKind.Explicit, Size = 48)]
public struct PositionUpdatePacket(float rotation, Vector3 position, PositionUpdatePacket.MoveType moveType = PositionUpdatePacket.MoveType.NormalMove0) : IUpstreamPacket
{
	public enum MoveType : uint
	{
		NormalMove0 = 0u,
		NormalMove1 = 65536u,
		NormalMove2 = 131072u,
		NormalMove3 = 196608u,
		Fly0 = 1u,
		Fly1 = 65537u,
		Fly2 = 131073u,
		Fly3 = 196609u,
		WalkOrSlowSwim0 = 2u,
		WalkOrSlowSwim1 = 65538u,
		WalkOrSlowSwim2 = 131074u,
		WalkOrSlowSwim3 = 196610u,
		SlowFly0 = 3u,
		SlowFly1 = 65539u,
		SlowFly2 = 131075u,
		SlowFly3 = 196611u,
		JumpStart0 = 256u,
		JumpStart1 = 65792u,
		JumpStart2 = 131328u,
		JumpStart3 = 196864u,
		JumpStartWalk0 = 258u,
		JumpStartWalk1 = 65794u,
		JumpStartWalk2 = 131330u,
		JumpStartWalk3 = 196866u,
		JumpEnd0 = 512u,
		JumpEnd1 = 66048u,
		JumpEnd2 = 131584u,
		JumpEnd3 = 197120u,
		JumpEndWalk0 = 514u,
		JumpEndWalk1 = 66050u,
		JumpEndWalk2 = 131586u,
		JumpEndWalk3 = 197122u,
		JumpProcess0 = 1048576u,
		JumpProcess1 = 1114112u,
		JumpProcess2 = 1179648u,
		JumpProcess3 = 1245184u,
		JumpProcessWalk0 = 1048578u,
		JumpProcessWalk1 = 1114114u,
		JumpProcessWalk2 = 1179650u,
		JumpProcessWalk3 = 1245186u,
		JumpHighestPoint0 = 1049600u,
		JumpHighestPoint1 = 1115136u,
		JumpHighestPoint2 = 1180672u,
		JumpHighestPoint3 = 1246208u,
		JumpHighestPointWalk0 = 1049602u,
		JumpHighestPointWalk1 = 1115138u,
		JumpHighestPointWalk2 = 1180674u,
		JumpHighestPointWalk3 = 1246210u,
		ActionMove0 = 2097152u,
		ActionMove1 = 2162688u,
		ActionMove2 = 2228224u,
		ActionMove3 = 2293760u,
		ActionMoveEnd0 = 4096u,
		ActionMoveEnd1 = 69632u,
		ActionMoveEnd2 = 135168u,
		ActionMoveEnd3 = 200704u
	}

	[FieldOffset(0)]
	public int Opcode = UpstreamOpcode.PositionUpdateOpcode;

	[FieldOffset(8)]
	public uint Length = 40u;

	[FieldOffset(32)]
	public float Rotation = rotation;

	[FieldOffset(36)]
	public MoveType Move = moveType;

	[FieldOffset(40)]
	public Vector3 Position = position;

	private static readonly ConcurrentDictionary<string, MoveType[]> MoveTypeCache = new ConcurrentDictionary<string, MoveType[]>();

	public string Log()
	{
		return $"Position Update 包体 ({Opcode} / 长度: {Length})\n面向: {Rotation} | 位置: {Position:F2} | 跳跃类型: {Move}";
	}

	public void Send()
	{
		OmenServiceBase<GamePacketManager>.Instance().SendPackt(this);
	}

	public static void Send(float rotation, Vector3 position, MoveType moveType = MoveType.NormalMove0)
	{
		string text = moveType.ToString();
		string key = text.Substring(0, text.Length - 1);
		MoveType[] orAdd = MoveTypeCache.GetOrAdd(key, (string value) => (from x in Enum.GetValues<MoveType>()
			where x.ToString().Contains(value)
			select x).ToArray());
		foreach (MoveType moveType2 in orAdd)
		{
			new PositionUpdatePacket(rotation, position, moveType2).Send();
		}
	}
}
