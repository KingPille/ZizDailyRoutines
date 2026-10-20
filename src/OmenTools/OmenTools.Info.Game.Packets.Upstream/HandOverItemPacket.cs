using System.Runtime.InteropServices;
using OmenTools.Info.Game.Packets.Abstractions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Info.Game.Packets.Upstream;

[StructLayout(LayoutKind.Explicit, Size = 56)]
public struct HandOverItemPacket(uint eventID, uint category = 0u, uint itemID = 0u, uint itemCount = 0u, uint leveID = 0u) : IUpstreamPacket
{
	[FieldOffset(0)]
	public int Opcode = UpstreamOpcode.HandOverItemOpcode;

	[FieldOffset(8)]
	public uint Length = 40u;

	[FieldOffset(32)]
	public uint EventID = eventID;

	[FieldOffset(36)]
	public uint Category = category;

	[FieldOffset(40)]
	public uint ItemID = itemID;

	[FieldOffset(44)]
	public uint ItemCount = itemCount;

	[FieldOffset(48)]
	public uint Param0 = 1u;

	[FieldOffset(52)]
	public uint LeveID = leveID;

	public string Log()
	{
		return $"Hand Over Item 包体 ({Opcode} / 长度: {Length})\nEvent ID: {EventID} / Category: {Category}\nItem ID: {ItemID} / Item Count: {ItemCount}\nParam0: {Param0} / Leve ID: {LeveID}";
	}

	public void Send()
	{
		OmenServiceBase<GamePacketManager>.Instance().SendPackt(this);
	}
}
