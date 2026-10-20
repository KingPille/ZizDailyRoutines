using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Client.LayoutEngine;
using FFXIVClientStructs.FFXIV.Client.LayoutEngine.Layer;

namespace OmenTools.Interop.Game.Models.Native;

[StructLayout(LayoutKind.Explicit, Size = 160)]
public struct ExitRangeLayoutInstance
{
	public enum ExitRangeType
	{
		ZoneLine = 1,
		Invisible
	}

	[FieldOffset(0)]
	public TriggerBoxLayoutInstance Base;

	[FieldOffset(128)]
	public ExitRangeType ExitType;

	[FieldOffset(132)]
	public ushort ZoneID;

	[FieldOffset(134)]
	public ushort TerritoryType;

	[FieldOffset(136)]
	public int Index;

	[FieldOffset(140)]
	public uint DestInstanceID;

	[FieldOffset(144)]
	public uint ReturnInstanceID;

	[FieldOffset(148)]
	public float PlayerRunningDirection;

	public unsafe PopRangeLayoutInstance* ReturnInstance => (PopRangeLayoutInstance*)LayoutWorld.GetLayoutInstanceStatic((InstanceType)40, ReturnInstanceID, 0u);
}
