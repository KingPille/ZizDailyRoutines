using System;
using System.Drawing;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Utility.Numerics;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;

namespace DailyRoutines.Common.Interface.Widgets;

public abstract class CardComponentBase
{
	protected readonly record struct CardDrawContext(Vector2 FrameMin, Vector2 ContentStartPos, Vector2 Padding, float FrameWidth = 0f);

	private const int BACKGROUND_CHANNEL = 0;

	private const int DECORATION_CHANNEL = 1;

	private const int CONTENT_CHANNEL = 2;

	private const float HOVER_STIFFNESS = 240f;

	private const float HOVER_DAMPING = 20f;

	private const float PRESS_STIFFNESS = 320f;

	private const float PRESS_DAMPING = 22f;

	private Vector2 currentSize;

	private Vector2 sizeVelocity;

	private bool isFirstDraw = true;

	private float hoverProgress;

	private float hoverVelocity;

	private float pressProgress;

	private float pressVelocity;

	private float restingHeight;

	protected Vector2 CurrentSize
	{
		get
		{
			if (!isFirstDraw || !(currentSize == Vector2.Zero))
			{
				return currentSize;
			}
			return InitialSize;
		}
	}

	protected virtual float Rounding => 6f * Globals.GlobalUIScale;

	protected virtual Vector2 InitialSize => Vector2.Zero;

	protected virtual bool DisableContent => false;

	protected virtual bool DrawDisabledMask => false;

	protected virtual bool EnablePressAnimation => false;

	protected virtual bool EnableHoverAnimation => true;

	protected virtual bool EnableTopHighlight => true;

	protected virtual bool EnableGlow => true;

	protected virtual bool WrapInContentTable => true;

	protected virtual bool IsSelected => false;

	protected virtual CardCursorAdvance CursorAdvance => CardCursorAdvance.Bottom;

	protected virtual float HoverFloatOffset => -2.5f * Globals.GlobalUIScale;

	protected virtual float PressOffset => 1.5f * Globals.GlobalUIScale;

	protected virtual Vector4 RestingBorder => VectorExtensions.WithW(KnownColor.MediumPurple.ToVector4(), 0.18f);

	protected virtual Vector4 HoveredBorder => VectorExtensions.WithW(KnownColor.MediumOrchid.ToVector4(), 0.85f);

	protected virtual Vector4? SelectedBorder => null;

	protected virtual Vector4? CustomBackgroundColor => null;

	public void Draw()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0473: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e1: Unknown result type (might be due to invalid IL or missing references)
		CardDrawContext context = CreateContext();
		Vector2 frameMin = context.FrameMin;
		ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
		Vector2 vector = CurrentSize;
		bool isHovered = vector.X > 0f && vector.Y > 0f && ImGui.IsMouseHoveringRect(frameMin, frameMin + vector);
		UpdateStates(isHovered);
		float num = (EnableHoverAnimation ? HoverFloatOffset : 0f) * hoverProgress;
		float num2 = (EnablePressAnimation ? PressOffset : 0f) * pressProgress;
		Vector2 vector2 = new Vector2(0f, num + num2);
		float y = ImGui.GetClipRectMin(windowDrawList).Y;
		if (frameMin.Y + num2 >= y && frameMin.Y + vector2.Y < y)
		{
			vector2.Y = y - frameMin.Y;
		}
		Vector2 vector3 = frameMin + vector2;
		((ImDrawListPtr)(ref windowDrawList)).ChannelsSplit(3);
		((ImDrawListPtr)(ref windowDrawList)).ChannelsSetCurrent(2);
		ImGui.SetCursorScreenPos(context.ContentStartPos + vector2);
		DisabledDisposable val = ImRaii.Disabled(DisableContent);
		Vector2 padding;
		try
		{
			GroupDisposable val2 = ImRaii.Group();
			try
			{
				if (WrapInContentTable && context.FrameWidth > 0f)
				{
					padding = context.Padding;
					padding.X = context.FrameWidth;
					ImGui.Dummy(padding);
					IndentDisposable val3 = ImRaii.PushIndent(context.Padding.X, true, true);
					try
					{
						float x = context.FrameWidth - context.Padding.X * 2f;
						TableDisposable val4 = ImRaii.Table(ImU8String.op_Implicit("CardContentTable"), 1, (ImGuiTableFlags)0, new Vector2(x, 0f));
						try
						{
							if (TableDisposable.op_Implicit(val4))
							{
								ImGui.TableSetupColumn(ImU8String.op_Implicit("ContentColumn"), (ImGuiTableColumnFlags)4, 0f, 0u);
								ImGui.TableNextRow();
								ImGui.TableNextColumn();
								DrawContent(context, isHovered);
							}
						}
						finally
						{
							((TableDisposable)(ref val4)).Dispose();
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					padding = context.Padding;
					padding.X = context.FrameWidth;
					ImGui.Dummy(padding);
				}
				else
				{
					DrawContent(context, isHovered);
				}
			}
			finally
			{
				((GroupDisposable)(ref val2)).Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		Vector2 itemRectSize = ImGui.GetItemRectSize();
		Vector2 targetSize = GetTargetSize(context, itemRectSize);
		if (isFirstDraw)
		{
			restingHeight = targetSize.Y;
		}
		if ((hoverProgress == 0f && pressProgress == 0f) || Math.Abs(targetSize.Y - restingHeight) > 5f)
		{
			restingHeight = targetSize.Y;
		}
		padding = targetSize;
		padding.Y = restingHeight;
		Vector2 targetSize2 = padding;
		UpdateCurrentSize(targetSize2);
		Vector2 vector4 = vector3 + currentSize;
		((ImDrawListPtr)(ref windowDrawList)).ChannelsSetCurrent(0);
		Vector4 value2;
		Vector4 value3;
		if (CustomBackgroundColor.HasValue)
		{
			Vector4 value = CustomBackgroundColor.Value;
			value2 = value;
			value3 = VectorExtensions.WithW(value, MathF.Min(1f, value.W * 1.3f));
		}
		else
		{
			Vector4 vector5 = ImGui.GetColorU32((ImGuiCol)3).ToVector4();
			value2 = VectorExtensions.WithW(vector5, MathF.Max(0f, vector5.W - 0.1f));
			value3 = VectorExtensions.WithW(vector5, MathF.Max(0f, vector5.W - 0.05f));
		}
		Vector4 color = Vector4.Lerp(value2, value3, hoverProgress);
		Vector4 color2 = ((IsSelected && SelectedBorder.HasValue) ? SelectedBorder.Value : Vector4.Lerp(RestingBorder, HoveredBorder, hoverProgress));
		if (EnableGlow && hoverProgress > 0.01f)
		{
			Vector4 color3 = VectorExtensions.WithW(KnownColor.Black.ToVector4(), 0.18f * hoverProgress);
			float glowSize = 12f * Globals.GlobalUIScale * hoverProgress;
			ImGuiOm.AddGlowRect(windowDrawList, vector3, vector4, color3.ToUInt(), Rounding, glowSize, 10, 0.2f);
			Vector4 color4 = ((IsSelected && SelectedBorder.HasValue) ? SelectedBorder.Value : HoveredBorder);
			color4.W = 0.16f * hoverProgress;
			float glowSize2 = 6f * Globals.GlobalUIScale * hoverProgress;
			ImGuiOm.AddGlowRect(windowDrawList, vector3, vector4, color4.ToUInt(), Rounding, glowSize2, 10);
		}
		((ImDrawListPtr)(ref windowDrawList)).AddRectFilled(vector3, vector4, color.ToUInt(), Rounding);
		((ImDrawListPtr)(ref windowDrawList)).AddRect(vector3, vector4, color2.ToUInt(), Rounding, (ImDrawFlags)0, IsSelected ? (1.5f * Globals.GlobalUIScale) : 1f);
		if (EnableTopHighlight)
		{
			Vector4 color5 = VectorExtensions.WithW(KnownColor.White.ToVector4(), 0.08f + 0.12f * hoverProgress);
			((ImDrawListPtr)(ref windowDrawList)).AddLine(new Vector2(vector3.X + Rounding, vector3.Y + 0.5f), new Vector2(vector3.X + currentSize.X - Rounding, vector3.Y + 0.5f), color5.ToUInt(), 1f);
		}
		((ImDrawListPtr)(ref windowDrawList)).ChannelsMerge();
		DrawAfterFrame(context, vector4, isHovered);
		ImGuiStylePtr style;
		switch (CursorAdvance)
		{
		case CardCursorAdvance.Bottom:
		{
			Vector2 vector7 = frameMin;
			float y2 = currentSize.Y;
			style = ImGui.GetStyle();
			ImGui.SetCursorScreenPos(vector7 + new Vector2(0f, y2 + ((ImGuiStylePtr)(ref style)).ItemSpacing.Y));
			break;
		}
		case CardCursorAdvance.Right:
		{
			Vector2 vector6 = frameMin;
			float x2 = currentSize.X;
			style = ImGui.GetStyle();
			ImGui.SetCursorScreenPos(vector6 + new Vector2(x2 + ((ImGuiStylePtr)(ref style)).ItemSpacing.X, 0f));
			break;
		}
		}
		if (DrawDisabledMask)
		{
			((ImDrawListPtr)(ref windowDrawList)).AddRectFilled(vector3, vector4, VectorExtensions.WithW(KnownColor.Black.ToVector4(), 0.2f).ToUInt(), Rounding);
		}
	}

	protected static int GetDecorationChannel()
	{
		return 1;
	}

	protected static int GetContentChannel()
	{
		return 2;
	}

	protected virtual void DrawAfterFrame(CardDrawContext context, Vector2 frameMax, bool isHovered)
	{
	}

	protected abstract CardDrawContext CreateContext();

	protected abstract void DrawContent(CardDrawContext context, bool isHovered);

	protected abstract Vector2 GetTargetSize(CardDrawContext context, Vector2 contentRectSize);

	private void UpdateCurrentSize(Vector2 targetSize)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		ImGuiIOPtr iO = ImGui.GetIO();
		float num = Math.Min(((ImGuiIOPtr)(ref iO)).DeltaTime, 0.05f);
		if (isFirstDraw)
		{
			currentSize = targetSize;
			sizeVelocity = Vector2.Zero;
			isFirstDraw = false;
			return;
		}
		Vector2 vector = currentSize - targetSize;
		if (vector.Length() < 2f && sizeVelocity.Length() < 0.1f)
		{
			currentSize = targetSize;
			sizeVelocity = Vector2.Zero;
			return;
		}
		Vector2 vector2 = -180f * vector - 28f * sizeVelocity;
		sizeVelocity += vector2 * num;
		currentSize += sizeVelocity * num;
		if (vector.Length() < 0.1f && sizeVelocity.Length() < 0.1f)
		{
			currentSize = targetSize;
			sizeVelocity = Vector2.Zero;
		}
	}

	private void UpdateStates(bool isHovered)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		ImGuiIOPtr iO = ImGui.GetIO();
		float num = Math.Min(((ImGuiIOPtr)(ref iO)).DeltaTime, 0.05f);
		float num2 = (isHovered ? 1f : 0f);
		float num3 = hoverProgress - num2;
		float num4 = -240f * num3 - 20f * hoverVelocity;
		hoverVelocity += num4 * num;
		hoverProgress += hoverVelocity * num;
		if (Math.Abs(num3) < 0.001f && Math.Abs(hoverVelocity) < 0.001f)
		{
			hoverProgress = num2;
			hoverVelocity = 0f;
		}
		hoverProgress = Math.Clamp(hoverProgress, 0f, 1f);
		float num5 = ((EnablePressAnimation && isHovered && ImGui.IsMouseDown((ImGuiMouseButton)0)) ? 1f : 0f);
		float num6 = pressProgress - num5;
		float num7 = -320f * num6 - 22f * pressVelocity;
		pressVelocity += num7 * num;
		pressProgress += pressVelocity * num;
		if (Math.Abs(num6) < 0.001f && Math.Abs(pressVelocity) < 0.001f)
		{
			pressProgress = num5;
			pressVelocity = 0f;
		}
		pressProgress = Math.Clamp(pressProgress, 0f, 1f);
	}
}
