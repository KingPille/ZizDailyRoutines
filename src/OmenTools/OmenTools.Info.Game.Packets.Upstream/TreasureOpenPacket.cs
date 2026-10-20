using System.Runtime.InteropServices;
using OmenTools.Info.Game.Packets.Abstractions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Info.Game.Packets.Upstream;

[StructLayout(LayoutKind.Explicit, Size = 44)]
public struct TreasureOpenPacket(ulong gameObjectID) : IUpstreamPacket
{
	[FieldOffset(0)]
	public int Opcode = UpstreamOpcode.TreasureOpenOpcode;

	[FieldOffset(8)]
	public uint Length = 24u;

	[FieldOffset(32)]
	public ulong GameObjectID = gameObjectID;

	public string Log()
	{
		return $"Treasure Open 包体 ({Opcode} / 长度: {Length})\nGame Object ID: {GameObjectID}";
	}

	public void Send()
	{
		OmenServiceBase<GamePacketManager>.Instance().SendPackt(this);
	}
}
