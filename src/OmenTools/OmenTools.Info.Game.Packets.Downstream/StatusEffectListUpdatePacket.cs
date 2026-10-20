using System;
using System.Runtime.InteropServices;
using OmenTools.Info.Game.Packets.Abstractions;

namespace OmenTools.Info.Game.Packets.Downstream;

[StructLayout(LayoutKind.Explicit)]
public struct StatusEffectListUpdatePacket : IDownstreamPacket
{
	[FieldOffset(20)]
	public unsafe fixed byte EntryData[240];

	public unsafe Span<StatusEffectListEntry> Entries
	{
		get
		{
			fixed (byte* entryData = EntryData)
			{
				void* pointer = entryData;
				return new Span<StatusEffectListEntry>(pointer, 30);
			}
		}
	}

	public string Log()
	{
		return "Status Effect List Update 包体";
	}
}
