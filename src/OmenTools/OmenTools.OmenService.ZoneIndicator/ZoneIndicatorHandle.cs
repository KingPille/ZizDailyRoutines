using System;

namespace OmenTools.OmenService.ZoneIndicator;

public readonly struct ZoneIndicatorHandle
{
	internal ulong ID { get; init; }

	public bool IsValid => ID != 0;

	private static ZoneIndicatorRenderer? Service => DService.Instance().GetOmenService<ZoneIndicatorRenderer>();

	public bool Unreg()
	{
		if (IsValid)
		{
			return Service?.UnregisterByID(ID) ?? false;
		}
		return false;
	}

	public bool Update(Action<IZoneIndicatorMutable> mutator)
	{
		ArgumentNullException.ThrowIfNull(mutator, "mutator");
		if (IsValid)
		{
			return Service?.UpdateByID(ID, mutator) ?? false;
		}
		return false;
	}
}
