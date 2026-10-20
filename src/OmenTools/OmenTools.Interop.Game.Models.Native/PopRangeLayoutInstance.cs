using System.Numerics;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Client.LayoutEngine.Layer;

namespace OmenTools.Interop.Game.Models.Native;

[StructLayout(LayoutKind.Explicit, Size = 160)]
public struct PopRangeLayoutInstance
{
	[FieldOffset(0)]
	public TriggerBoxLayoutInstance Base;

	[FieldOffset(128)]
	public unsafe Vector3* AddPos;
}
