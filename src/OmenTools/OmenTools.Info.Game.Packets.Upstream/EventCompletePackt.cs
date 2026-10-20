using System.Runtime.InteropServices;
using OmenTools.Info.Game.Packets.Abstractions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Info.Game.Packets.Upstream;

[StructLayout(LayoutKind.Explicit, Size = 52)]
public struct EventCompletePackt(uint eventID, uint category, uint param0 = 0u, uint param1 = 0u, uint param2 = 0u) : IUpstreamPacket
{
	[FieldOffset(0)]
	public int Opcode = UpstreamOpcode.EventCompleteOpcode;

	[FieldOffset(8)]
	public uint Length = 32u;

	[FieldOffset(32)]
	public uint EventID = eventID;

	[FieldOffset(36)]
	public uint Category = category;

	[FieldOffset(40)]
	public uint Param0 = param0;

	[FieldOffset(44)]
	public uint Param1 = param1;

	[FieldOffset(48)]
	public uint Param2 = param2;

	public string Log()
	{
		return $"Event Complete 包体 ({Opcode} / 长度: {Length})\nEvent ID: {EventID} | Category: {Category} \nParam0: {Param0} | Param1: {Param1} | Param2: {Param2}";
	}

	public void Send()
	{
		OmenServiceBase<GamePacketManager>.Instance().SendPackt(this);
	}
}
