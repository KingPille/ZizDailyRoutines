using FFXIVClientStructs.FFXIV.Client.LayoutEngine;
using FFXIVClientStructs.FFXIV.Client.LayoutEngine.Group;
using OmenTools.Dalamud.Services.LayoutEngine.Group.Abstractions;

namespace OmenTools.Dalamud.Services.LayoutEngine.Group;

internal class SharedGroupLayoutInstance(nint address) : ISharedGroupLayoutInstance
{
	private unsafe SharedGroupLayoutInstance* Struct => (SharedGroupLayoutInstance*)Address;

	public nint Address { get; } = address;

	public unsafe Transform Transform => ((SharedGroupLayoutInstance)Struct).Transform;

	public unsafe uint PrefabFlags1 => ((SharedGroupLayoutInstance)Struct).PrefabFlags1;

	public unsafe uint PrefabFlags2 => ((SharedGroupLayoutInstance)Struct).PrefabFlags2;

	public unsafe byte PlayingTimelineIndex => ((SharedGroupLayoutInstance)Struct).PlayingTimelineIndex;

	public unsafe byte? DefaultStainIndex
	{
		get
		{
			if (((SharedGroupLayoutInstance)Struct).StainInfo != null)
			{
				return ((SharedGroupStainInfo)((SharedGroupLayoutInstance)Struct).StainInfo).DefaultStainIndex;
			}
			return null;
		}
	}

	public unsafe byte? ChosenStainIndex
	{
		get
		{
			if (((SharedGroupLayoutInstance)Struct).StainInfo != null)
			{
				return ((SharedGroupStainInfo)((SharedGroupLayoutInstance)Struct).StainInfo).ChosenStainIndex;
			}
			return null;
		}
	}

	public unsafe SharedGroupStainFlags? StainFlags
	{
		get
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			if (((SharedGroupLayoutInstance)Struct).StainInfo != null)
			{
				return ((SharedGroupStainInfo)((SharedGroupLayoutInstance)Struct).StainInfo).Flags;
			}
			return null;
		}
	}

	public unsafe bool IsTimelinePlaying(uint index)
	{
		return ((SharedGroupLayoutInstance)Struct).IsTimelinePlaying(index);
	}

	public unsafe bool IsTimelineIndexValid(uint index)
	{
		return ((SharedGroupLayoutInstance)Struct).IsTimelineIndexValid(index);
	}

	public unsafe bool TryApplyStain(byte stainIndex)
	{
		return ((SharedGroupLayoutInstance)Struct).TryApplyStain(stainIndex);
	}

	public unsafe SharedGroupLayoutInstance* ToStruct()
	{
		return Struct;
	}
}
