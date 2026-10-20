using System.Numerics;

namespace OmenTools.OmenService.ZoneIndicator;

public sealed class ZoneIndicatorSurrounding
{
	public enum Shape
	{
		Circle,
		Square,
		Triangle
	}

	public Shape Type { get; init; }

	public float Radius { get; init; }

	public Vector4 Color { get; init; } = Vector4.One;

	public float Thickness { get; init; } = 1f;
}
