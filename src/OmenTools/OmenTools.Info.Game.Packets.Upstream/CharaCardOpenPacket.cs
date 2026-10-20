using System;
using System.Runtime.InteropServices;
using Dalamud.Memory;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using OmenTools.Info.Game.Packets.Abstractions;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Info.Game.Packets.Upstream;

[StructLayout(LayoutKind.Explicit, Size = 48)]
public struct CharaCardOpenPacket(ulong entityID) : IUpstreamPacket
{
	private unsafe delegate void PrepareCharaPortraitDataDelegate(AgentCharaCard* agent);

	[FieldOffset(0)]
	public int Opcode = UpstreamOpcode.CharaCardOpenOpcode;

	[FieldOffset(8)]
	public uint Length = 32u;

	[FieldOffset(32)]
	public ulong EntityID = entityID;

	[FieldOffset(40)]
	public int Flag0 = ((LocalPlayerState.EntityID != entityID) ? 1 : 2);

	[FieldOffset(44)]
	public int Flag1 = 1;

	private static readonly PrepareCharaPortraitDataDelegate PrepareCharaPortraitData = new CompSig("E8 ?? ?? ?? ?? 8B 05 ?? ?? ?? ?? 48 8D 0D").GetDelegate<PrepareCharaPortraitDataDelegate>();

	public string Log()
	{
		return $"Chara Card Open 包体 ({Opcode} / 长度: {Length})\nEntity ID: {EntityID} | Flag0: {Flag0} | Flag1: {Flag1}";
	}

	public unsafe void Send()
	{
		AgentCharaCard* ptr = AgentCharaCard.Instance();
		PrepareCharaPortraitData(ptr);
		OmenServiceBase<GamePacketManager>.Instance().SendPackt(this);
		MemoryHelper.Write<byte>((IntPtr)(nint)((byte*)ptr + 48), (byte)1);
	}
}
