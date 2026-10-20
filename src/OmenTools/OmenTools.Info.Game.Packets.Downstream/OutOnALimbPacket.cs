using System.Runtime.InteropServices;
using OmenTools.Info.Game.Packets.Abstractions;

namespace OmenTools.Info.Game.Packets.Downstream;

[StructLayout(LayoutKind.Explicit)]
public struct OutOnALimbPacket : IDownstreamPacket
{
	[FieldOffset(4)]
	public OutOnALimbResult Result;

	[FieldOffset(8)]
	public uint Health;

	[FieldOffset(20)]
	public byte BonusLevel;

	public string Log()
	{
		return $"孤树无援游玩结果包体\n游玩结果: {Result} | 体力: {Health} | 奖励等级: {BonusLevel}";
	}
}
