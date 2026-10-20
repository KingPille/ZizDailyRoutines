using System.Numerics;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Global;

public static class Globals
{
	public static float GlobalUIScale => OmenServiceBase<FontManager>.Instance().GlobalFontScale;

	public static Vector2 ScaledVector2(float x)
	{
		return new Vector2(x) * GlobalUIScale;
	}

	public static Vector2 ScaledVector2(float x, float y)
	{
		return new Vector2(x, y) * GlobalUIScale;
	}

	public static Vector2 ScaledVector2(Vector2 size)
	{
		return size * GlobalUIScale;
	}
}
