using System;
using System.Numerics;

namespace OmenTools.OmenService.ZoneIndicator;

internal readonly struct ResolvedTarget(Vector3 position, ZoneIndicatorText? text, Action<ZoneIndicatorDrawContext>? onDraw)
{
	public Vector3 Position { get; } = position;

	public ZoneIndicatorText? Text { get; } = text;

	public Action<ZoneIndicatorDrawContext>? OnDraw { get; } = onDraw;
}
