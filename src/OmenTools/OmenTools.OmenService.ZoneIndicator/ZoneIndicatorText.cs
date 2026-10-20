using System;
using System.Numerics;
using Dalamud.Interface.Textures;
using Lumina.Text.ReadOnly;

namespace OmenTools.OmenService.ZoneIndicator;

public sealed class ZoneIndicatorText
{
	public sealed class TextImage
	{
		public required ISharedImmediateTexture Texture { get; init; }

		public required Func<Vector2> SizeGetter { get; init; }
	}

	public ReadOnlySeString? Text { get; init; }

	public Vector4? TextColor { get; init; }

	public float? TextScale { get; init; }

	public Vector3? TextOffset { get; init; }

	public TextImage? Image { get; init; }
}
