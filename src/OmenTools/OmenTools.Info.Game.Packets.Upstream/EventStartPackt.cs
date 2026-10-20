using System.Runtime.InteropServices;
using OmenTools.Info.Game.Packets.Abstractions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Info.Game.Packets.Upstream;

[StructLayout(LayoutKind.Explicit, Size = 52)]
public struct EventStartPackt(ulong objectID, uint eventID, uint category = 0u, uint param = 0u) : IUpstreamPacket
{
	[FieldOffset(0)]
	public int Opcode = UpstreamOpcode.EventStartOpcode;

	[FieldOffset(8)]
	public uint Length = 32u;

	[FieldOffset(32)]
	public ulong EventObjectID = objectID;

	[FieldOffset(40)]
	public uint EventID = eventID;

	[FieldOffset(44)]
	public uint Category = category;

	[FieldOffset(48)]
	public uint Param = param;

	[FieldOffset(40)]
	public ushort EntryID = 0;

	public string Log()
	{
		return $"Event Start 包体 ({Opcode} / 长度: {Length})\nEvent Object ID: {EventObjectID} | Event ID: {EventID} | Entry ID: {EntryID}\nCategory: {Category} | Param: {Param}";
	}

	public void Send()
	{
		OmenServiceBase<GamePacketManager>.Instance().SendPackt(this);
	}
}
