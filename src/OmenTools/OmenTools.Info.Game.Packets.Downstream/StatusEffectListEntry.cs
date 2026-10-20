using System.Runtime.InteropServices;

namespace OmenTools.Info.Game.Packets.Downstream;

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 8)]
public struct StatusEffectListEntry
{
	[FieldOffset(0)]
	public ushort StatusID;

	[FieldOffset(2)]
	public ushort StackCount;

	[FieldOffset(4)]
	public float RemainingTime;

	[FieldOffset(8)]
	public uint SourceID;
}
