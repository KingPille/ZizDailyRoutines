using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace OmenTools.Interop.Game.Models.Native;

[StructLayout(LayoutKind.Explicit, Size = 16)]
public struct SelectedMateria
{
	[FieldOffset(0)]
	public InventoryType Type;

	[FieldOffset(4)]
	public ushort Slot;

	[FieldOffset(8)]
	public uint ItemID;

	[FieldOffset(12)]
	public uint SelectCount;
}
