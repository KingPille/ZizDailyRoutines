using System.Runtime.InteropServices;
using OmenTools.Info.Game.Packets.Abstractions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Info.Game.Packets.Upstream;

[StructLayout(LayoutKind.Explicit, Size = 48)]
public struct HeartbeatPacket : IUpstreamPacket
{
	[FieldOffset(0)]
	public int Opcode = UpstreamOpcode.HeartbeatOpcode;

	[FieldOffset(8)]
	public uint Length = 48u;

	public HeartbeatPacket()
	{
	}

	public string Log()
	{
		return $"Heartbeat 包体 ({Opcode} / 长度: {Length})";
	}

	public void Send()
	{
		OmenServiceBase<GamePacketManager>.Instance().SendPackt(this);
	}
}
