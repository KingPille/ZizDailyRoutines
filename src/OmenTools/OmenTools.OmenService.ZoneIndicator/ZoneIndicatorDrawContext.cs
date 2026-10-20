using System.Numerics;
using Dalamud.Bindings.ImGui;

namespace OmenTools.OmenService.ZoneIndicator;

public readonly struct ZoneIndicatorDrawContext
{
	public Vector3 WorldPosition { get; }

	public Vector2 ScreenPosition { get; }

	public bool IsOnScreen { get; }

	public float Distance { get; }

	public ImDrawListPtr DrawList { get; }

	public Vector2 TextSize { get; }

	internal ZoneIndicatorDrawContext(Vector3 worldPosition, Vector2 screenPosition, bool isOnScreen, float distance, ImDrawListPtr drawList, Vector2 textSize)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		WorldPosition = worldPosition;
		ScreenPosition = screenPosition;
		IsOnScreen = isOnScreen;
		Distance = distance;
		DrawList = drawList;
		TextSize = textSize;
	}
}
public readonly struct ZoneIndicatorDrawContext<T>
{
	public Vector3 WorldPosition { get; }

	public Vector2 ScreenPosition { get; }

	public bool IsOnScreen { get; }

	public float Distance { get; }

	public ImDrawListPtr DrawList { get; }

	public Vector2 TextSize { get; }

	public T Source { get; }

	internal ZoneIndicatorDrawContext(ZoneIndicatorDrawContext baseCtx, T source)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		WorldPosition = baseCtx.WorldPosition;
		ScreenPosition = baseCtx.ScreenPosition;
		IsOnScreen = baseCtx.IsOnScreen;
		Distance = baseCtx.Distance;
		DrawList = baseCtx.DrawList;
		TextSize = baseCtx.TextSize;
		Source = source;
	}
}
