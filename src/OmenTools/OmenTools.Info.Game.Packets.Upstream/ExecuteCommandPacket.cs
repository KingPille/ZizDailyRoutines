using System.Runtime.InteropServices;
using OmenTools.Info.Game.Enums;
using OmenTools.Info.Game.Packets.Abstractions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Info.Game.Packets.Upstream;

[StructLayout(LayoutKind.Explicit, Size = 48)]
public struct ExecuteCommandPacket(ExecuteCommandFlag flag, int param1 = 0, int param2 = 0, int param3 = 0, int param4 = 0) : IUpstreamPacket
{
	[FieldOffset(0)]
	public int Opcode = UpstreamOpcode.ExecuteCommandOpcode;

	[FieldOffset(8)]
	public uint Length = 48u;

	[FieldOffset(32)]
	public ExecuteCommandFlag Flag = flag;

	[FieldOffset(36)]
	public int Param1 = param1;

	[FieldOffset(40)]
	public int Param2 = param2;

	[FieldOffset(44)]
	public int Param3 = param3;

	[FieldOffset(48)]
	public int Param4 = param4;

	public string Log()
	{
		return $"Execute Command Packet 包体 ({Opcode} / 长度: {Length})\nFlag: {Flag}\nP1: {Param1} | P2: {Param2} | P3: {Param3} | P4: {Param4}";
	}

	public void Send()
	{
		OmenServiceBase<GamePacketManager>.Instance().SendPackt(this);
	}
}
