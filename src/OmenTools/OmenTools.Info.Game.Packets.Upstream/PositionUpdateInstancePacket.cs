using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using OmenTools.Info.Game.Packets.Abstractions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Info.Game.Packets.Upstream;

[StructLayout(LayoutKind.Explicit, Size = 56)]
public struct PositionUpdateInstancePacket(float charaRotation, Vector3 position, PositionUpdateInstancePacket.MoveType moveType = PositionUpdateInstancePacket.MoveType.NormalMove0) : IUpstreamPacket
{
	public enum MoveType : uint
	{
		NormalMove0 = 0u,
		NormalMove1 = 65536u,
		NormalMove2 = 131072u,
		NormalMove3 = 196608u,
		ActionMove0 = 2097152u,
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
		ActionMoveEnd0 = 4096u,
		SmallMove0 = 4210688u,
		SmallMove1 = 4276224u,
		SmallMove2 = 4341760u,
		SmallMove3 = 4407296u,
		SmallFlight0 = 4210689u,
		SmallFlight1 = 4276225u,
		SmallFlight2 = 4341761u,
		SmallFlight3 = 4407297u,
		Falling0 = 1048576u,
		Falling1 = 1114112u,
		Falling2 = 1179648u,
		Falling3 = 1245184u,
		JumpStart0 = 4194560u,
		JumpStart1 = 4260096u,
		JumpStart2 = 4325632u,
		JumpStart3 = 4391168u,
		JumpProcess0 = 5259264u,
		JumpProcess1 = 5324800u,
		JumpProcess2 = 5390336u,
		JumpProcess3 = 5455872u,
		JumpHighestPoint0 = 5309440u,
		JumpEnd0 = 4194816u,
		JumpEnd1 = 4260352u,
		JumpEnd2 = 4325888u,
		JumpEnd3 = 4391424u
	}

	[FieldOffset(0)]
	public int Opcode = UpstreamOpcode.PositionUpdateInstanceOpcode;

	[FieldOffset(8)]
	public uint Length = 56u;

	[FieldOffset(32)]
	public float Rotation = charaRotation;

	[FieldOffset(36)]
	public float RotationNew = charaRotation;

	[FieldOffset(40)]
	public MoveType Move = moveType;

	[FieldOffset(44)]
	public Vector3 Position = position;

	[FieldOffset(56)]
	public Vector3 PositionNew = position;

	private static readonly ConcurrentDictionary<string, MoveType[]> MoveTypeCache = new ConcurrentDictionary<string, MoveType[]>();

	public string Log()
	{
		return $"Position Update Instance 包体 ({Opcode} / 长度: {Length})\n旧面向: {Rotation} | 新面向: {RotationNew}\n位置: {Position:F2} | 新位置: {PositionNew:F2}\n移动类型: {Move}";
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
			new PositionUpdateInstancePacket(rotation, position, moveType2).Send();
		}
	}
}
