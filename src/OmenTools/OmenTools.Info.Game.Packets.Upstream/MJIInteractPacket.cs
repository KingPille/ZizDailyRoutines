using System.Runtime.InteropServices;
using OmenTools.Info.Game.Packets.Abstractions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Info.Game.Packets.Upstream;

[StructLayout(LayoutKind.Explicit, Size = 40)]
public struct MJIInteractPacket(uint layoutID, uint dataID = 2012985u) : IUpstreamPacket
{
	[FieldOffset(0)]
	public int Opcode = UpstreamOpcode.HeartbeatOpcode;

	[FieldOffset(8)]
	public uint Length = 40u;

	[FieldOffset(32)]
	public uint LayoutID = layoutID;

	[FieldOffset(36)]
	public uint DataID = dataID;

	public string Log()
	{
		return $"MJIInteractPacket 包体 ({Opcode} / 长度: {Length})\nLayout ID: {LayoutID} | Data ID: {DataID}";
	}

	public void Send()
	{
		OmenServiceBase<GamePacketManager>.Instance().SendPackt(this);
	}
}
