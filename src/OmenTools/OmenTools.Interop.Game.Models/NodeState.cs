using System.Numerics;
using Dalamud.Utility.Numerics;

namespace OmenTools.Interop.Game.Models;

public readonly record struct NodeState(Vector2 TopLeft, Vector2 Size, bool Visible)
{
	public float Width => Size.X;

	public float Height => Size.Y;

	public Vector2 Center => TopLeft + Size / 2f;

	public Vector2 TopRight => TopLeft + VectorExtensions.WithY(Size, 0f);

	public Vector2 BottomLeft => TopLeft + VectorExtensions.WithX(Size, 0f);

	public Vector2 BottomRight => TopLeft + Size;

	public float X => TopLeft.X;

	public float Y => TopLeft.Y;
}
