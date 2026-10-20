using System.Numerics;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Client.Game;
using OmenTools.Info.Game.Packets.Abstractions;
using OmenTools.Interop.Game.Helpers;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Info.Game.Packets.Upstream;

[StructLayout(LayoutKind.Explicit, Size = 48)]
public unsafe struct UseActionLocationPacket(ActionType type, uint actionID, float rotation, Vector3 location) : IUpstreamPacket
{
	[FieldOffset(0)]
	public int Opcode = UpstreamOpcode.UseActionLocationOpcode;

	[FieldOffset(8)]
	public uint Length = 48u;

	[FieldOffset(32)]
	public uint ActionID = actionID;

	[FieldOffset(36)]
	public byte CastBuff = 0;

	[FieldOffset(37)]
	public byte ActionType = (byte)type;

	[FieldOffset(38)]
	public unsafe ushort LastUsedActionSequence = (ushort)(((ActionManager)ActionManager.Instance()).LastUsedActionSequence + 1);

	[FieldOffset(40)]
	public ushort Rotation = RotationHelper.CharaToPacket(rotation);

	[FieldOffset(42)]
	public ushort RotationNew = 0;

	[FieldOffset(44)]
	public Vector3 Location = location;

	public string Log()
	{
		return $"Use Action Location 包体 ({Opcode} / 长度: {Length})\nType: {(ActionType)ActionType} | ID: {ActionID}\nRotation: {Rotation} ({RotationHelper.PacketToChara(Rotation):F2}) | Rotation (New): {RotationNew} ({RotationHelper.PacketToChara(RotationNew):F2})\nLocation: {Location:F2}\nCast Buff: {CastBuff} | Action Sequence: {LastUsedActionSequence}";
	}

	public void Send()
	{
		OmenServiceBase<GamePacketManager>.Instance().SendPackt(this);
	}
}
