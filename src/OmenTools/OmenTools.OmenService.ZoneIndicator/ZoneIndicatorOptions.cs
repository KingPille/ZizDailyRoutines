using System;

namespace OmenTools.OmenService.ZoneIndicator;

public class ZoneIndicatorOptions
{
	public uint? RenderRadius { get; init; }

	public bool HiddenWhenBlocked { get; init; }

	public bool HideTextLabel { get; init; }

	public ZoneIndicatorSurrounding? Surrounding { get; init; }
}
public sealed class ZoneIndicatorOptions<T> : ZoneIndicatorOptions
{
	public Func<T, ZoneIndicatorText>? TextGetter { get; init; }

	public Action<ZoneIndicatorDrawContext<T>>? CustomDraw { get; init; }
}
