using System.Numerics;
using System.Runtime.InteropServices;
using OmenTools.Info.Game.Packets.Abstractions;
using OmenTools.Interop.Game.Models;

namespace OmenTools.Info.Game.Packets.Downstream;

[StructLayout(LayoutKind.Explicit)]
public struct ActorSetPosPacket(Vector3 position, byte territoryTransportType = 1, byte characterMode = 1, uint transitionTerritoryFilterKey = 0u) : IDownstreamPacket
{
	public unsafe delegate void* Delegate(uint entityID, ActorSetPosPacket* packet);

	[FieldOffset(2)]
	public byte TerritoryTransportType = territoryTransportType;

	[FieldOffset(3)]
	public byte CharacterMode = characterMode;

	[FieldOffset(4)]
	public uint TransitionTerritoryFilterKey = transitionTerritoryFilterKey;

	[FieldOffset(8)]
	public Vector3 Position = position;

	public static CompSig Signature { get; } = new CompSig("4C 8B C2 8B D1 48 8D 0D ?? ?? ?? ?? E9 ?? ?? ?? ?? CC CC CC CC CC CC CC CC CC CC CC CC CC CC CC 48 89 5C 24");

	public string Log()
	{
		return $"Actor Set Pos 包体\n位置: {Position:F2}\nTerritoryTransportType: {TerritoryTransportType}\nCharacterMode: {CharacterMode}\nTransitionTerritoryFilterKey: {TransitionTerritoryFilterKey}";
	}
}
