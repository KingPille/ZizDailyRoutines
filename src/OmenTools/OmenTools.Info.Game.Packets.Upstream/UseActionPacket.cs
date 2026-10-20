using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Client.Game;
using OmenTools.Info.Game.Packets.Abstractions;
using OmenTools.Interop.Game.Helpers;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Info.Game.Packets.Upstream;

[StructLayout(LayoutKind.Explicit, Size = 48)]
public unsafe struct UseActionPacket(ActionType type, uint actionID, ulong targetID, float rotation) : IUpstreamPacket
{
	[FieldOffset(0)]
	public int Opcode = UpstreamOpcode.UseActionOpcode;

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
	public ushort RotationNew = RotationHelper.CharaToPacket(rotation);

	[FieldOffset(44)]
	public ushort Charge = 0;

	[FieldOffset(46)]
	public ushort ExtraParam = 0;

	[FieldOffset(48)]
	public ulong TargetID = targetID;

	public string Log()
	{
		return $"Use Action 包体 ({Opcode} / 长度: {Length})\nType: {(ActionType)ActionType} | ID: {ActionID} | Target ID: {TargetID}\nRotation: {Rotation} ({RotationHelper.PacketToChara(Rotation):F2}) | Rotation (New): {RotationNew} ({RotationHelper.PacketToChara(RotationNew):F2})\nCast Buff: {CastBuff} | Action Sequence: {LastUsedActionSequence}\nCharge: {Charge} | Extra Param: {ExtraParam}";
	}

	public void Send()
	{
		OmenServiceBase<GamePacketManager>.Instance().SendPackt(this);
	}
}
