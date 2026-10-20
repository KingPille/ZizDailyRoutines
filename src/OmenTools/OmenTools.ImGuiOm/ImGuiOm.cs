using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.ImGuiSeStringRenderer;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Utility;
using Lumina.Text;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.Interop.Windows.Helpers;

namespace OmenTools.ImGuiOm;

public static class ImGuiOm
{
	public readonly ref struct ModalPopupDisposable : IDisposable
	{
		public readonly bool Success;

		private readonly bool windowBegun;

		private readonly ImGuiWindowPtr blockerWindow;

		private readonly ImGuiWindowPtr modalWindow;

		internal ModalPopupDisposable(bool success)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			Success = success;
			windowBegun = false;
			blockerWindow = ImGuiWindowPtr.Null;
			modalWindow = ImGuiWindowPtr.Null;
		}

		internal ModalPopupDisposable(bool success, bool inWindowBegun, ImGuiWindowPtr inBlockerWindow, ImGuiWindowPtr inModalWindow)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			Success = success;
			windowBegun = inWindowBegun;
			blockerWindow = inBlockerWindow;
			modalWindow = inModalWindow;
		}

		public void Dispose()
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			if (windowBegun)
			{
				ImGui.End();
				OrderModalWindows(blockerWindow, modalWindow);
			}
		}

		public static implicit operator bool(ModalPopupDisposable value)
		{
			return value.Success;
		}

		public static bool operator true(ModalPopupDisposable value)
		{
			return value.Success;
		}

		public static bool operator false(ModalPopupDisposable value)
		{
			return !value.Success;
		}

		public static bool operator !(ModalPopupDisposable value)
		{
			return !value.Success;
		}

		public static bool operator &(ModalPopupDisposable value, bool other)
		{
			return value.Success && other;
		}

		public static bool operator |(ModalPopupDisposable value, bool other)
		{
			return value.Success || other;
		}
	}

	private const float HOLD_BUTTON_FLASH_DURATION = 0.5f;

	private static readonly Dictionary<string, (float Charge, float FlashTime)> HoldButtonStates = new Dictionary<string, (float, float)>();

	private static readonly Dictionary<(string PlayerName, string WorldName), ReadOnlySeString> RenderedPlayerInfos = new Dictionary<(string, string), ReadOnlySeString>();

	private static readonly Dictionary<(string text, float width), string> wrappedTextCache = new Dictionary<(string, float), string>();

	public static bool ToggleButton(string id, ref bool value, float height = 0f, uint? bgActiveColor = null, uint? bgColor = null)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		float num = ((height > 0f) ? height : ImGui.GetTextLineHeightWithSpacing());
		Vector2 vector = new Vector2(num * 1.875f, num);
		Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
		bool num2 = ImGui.InvisibleButton(ImU8String.op_Implicit(id), vector, (ImGuiButtonFlags)0);
		if (num2)
		{
			value = !value;
		}
		ImGuiStylePtr style = ImGui.GetStyle();
		Span<Vector4> colors = ((ImGuiStylePtr)(ref style)).Colors;
		uint num3 = ((!value) ? (bgColor ?? ImGui.GetColorU32((ImGuiCol)7)) : (bgActiveColor ?? ImGui.GetColorU32((ImGuiCol)23)));
		if (ImGui.IsItemActive())
		{
			num3 = Vector4.Lerp(num3.ToVector4(), colors[0], 0.24f).ToUInt();
		}
		else if (ImGui.IsItemHovered())
		{
			num3 = Vector4.Lerp(num3.ToVector4(), colors[0], 0.14f).ToUInt();
		}
		float num4 = vector.Y * 0.5f;
		float num5 = MathF.Max(num4 - 2f * ImGuiHelpers.GlobalScale, 1f);
		Vector2 vector2 = new Vector2(value ? (cursorScreenPos.X + vector.X - num4) : (cursorScreenPos.X + num4), cursorScreenPos.Y + num4);
		ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
		((ImDrawListPtr)(ref windowDrawList)).AddRectFilled(cursorScreenPos, cursorScreenPos + vector, num3, num4);
		((ImDrawListPtr)(ref windowDrawList)).AddCircleFilled(vector2, num5, ImGui.GetColorU32((ImGuiCol)0));
		return num2;
	}

	public static bool ToggleButton(string id, ref bool value, Vector4? bgActiveColor, Vector4? bgColor = null, float height = 0f)
	{
		return ToggleButton(id, ref value, height, bgActiveColor.HasValue ? new uint?(bgActiveColor.GetValueOrDefault().ToUInt()) : ((uint?)null), bgColor.HasValue ? new uint?(bgColor.GetValueOrDefault().ToUInt()) : ((uint?)null));
	}

	public static bool ButtonImage(ImTextureID textureID, Vector2 buttonSize, Vector2 imageSize, Vector2 uv0 = default(Vector2), Vector2 uv1 = default(Vector2))
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		if (uv0 == default(Vector2))
		{
			uv0 = new Vector2(0f, 0f);
		}
		if (uv1 == default(Vector2))
		{
			uv1 = new Vector2(1f, 1f);
		}
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(0, 1);
		((ImU8String)(ref val)).AppendFormatted<ImTextureID>(textureID);
		IdDisposable val2 = ImRaii.PushId(val, true);
		try
		{
			bool result = ImGui.InvisibleButton(ImU8String.op_Implicit("##imgbtn"), buttonSize, (ImGuiButtonFlags)0);
			bool flag = ImGui.IsItemHovered();
			bool num = ImGui.IsItemActive();
			Vector2 itemRectMin = ImGui.GetItemRectMin();
			Vector2 itemRectMax = ImGui.GetItemRectMax();
			ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
			uint num2 = (num ? ImGui.GetColorU32((ImGuiCol)23) : (flag ? ImGui.GetColorU32((ImGuiCol)22) : ImGui.GetColorU32((ImGuiCol)21)));
			ImGuiStylePtr style = ImGui.GetStyle();
			float frameRounding = ((ImGuiStylePtr)(ref style)).FrameRounding;
			((ImDrawListPtr)(ref windowDrawList)).AddRectFilled(itemRectMin, itemRectMax, num2, frameRounding);
			uint colorU = ImGui.GetColorU32((ImGuiCol)5);
			style = ImGui.GetStyle();
			if (((ImGuiStylePtr)(ref style)).FrameBorderSize > 0f)
			{
				((ImDrawListPtr)(ref windowDrawList)).AddRect(itemRectMin, itemRectMax, colorU, frameRounding);
			}
			style = ImGui.GetStyle();
			Vector2 framePadding = ((ImGuiStylePtr)(ref style)).FramePadding;
			Vector2 vector = itemRectMin + framePadding;
			Vector2 vector2 = itemRectMax - framePadding;
			if (vector2.X < vector.X)
			{
				vector2.X = vector.X;
			}
			if (vector2.Y < vector.Y)
			{
				vector2.Y = vector.Y;
			}
			Vector2 vector3 = vector2 - vector;
			Vector2 vector4 = vector;
			Vector2 vector5 = vector2;
			float x = vector3.X / MathF.Max(1E-06f, imageSize.X);
			float y = vector3.Y / MathF.Max(1E-06f, imageSize.Y);
			float num3 = MathF.Min(x, y);
			Vector2 vector6 = imageSize * num3;
			Vector2 vector7 = (vector3 - vector6) * 0.5f;
			vector4 = vector + vector7;
			vector5 = vector4 + vector6;
			((ImDrawListPtr)(ref windowDrawList)).AddImage(textureID, vector4, vector5, uv0, uv1, uint.MaxValue);
			return result;
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	public static bool ButtonIcon(string id, FontAwesomeIcon icon, string tooltip = "", bool useStaticFont = false)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(1, 2);
		((ImU8String)(ref val)).AppendFormatted<string>(id);
		((ImU8String)(ref val)).AppendLiteral("_");
		((ImU8String)(ref val)).AppendFormatted<FontAwesomeIcon>(icon);
		IdDisposable val2 = ImRaii.PushId(val, true);
		try
		{
			string iconText = FontAwesomeExtensions.ToIconString(icon);
			Vector2 contentSize = CalcIconSize(iconText, useStaticFont);
			Vector2 vector = new Vector2(GetSingleLineHeight());
			bool result = ImGui.Button(ImU8String.op_Implicit(string.Empty), vector);
			var (areaMin, areaSize) = GetItemRect();
			DrawIconText(GetCenteredPosition(areaMin, areaSize, contentSize), iconText, useStaticFont);
			if (!StringExtensions.IsNullOrEmpty(tooltip))
			{
				TooltipHover(tooltip);
			}
			return result;
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	public static bool ButtonIcon(string id, FontAwesomeIcon icon, Vector2 buttonSize, string tooltip = "", bool useStaticFont = false)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(1, 2);
		((ImU8String)(ref val)).AppendFormatted<string>(id);
		((ImU8String)(ref val)).AppendLiteral("_");
		((ImU8String)(ref val)).AppendFormatted<FontAwesomeIcon>(icon);
		IdDisposable val2 = ImRaii.PushId(val, true);
		try
		{
			string iconText = FontAwesomeExtensions.ToIconString(icon);
			Vector2 contentSize = CalcIconSize(iconText, useStaticFont);
			bool result = ImGui.Button(ImU8String.op_Implicit(string.Empty), buttonSize);
			var (areaMin, areaSize) = GetItemRect();
			DrawIconText(GetCenteredPosition(areaMin, areaSize, contentSize), iconText, useStaticFont);
			if (!StringExtensions.IsNullOrEmpty(tooltip))
			{
				TooltipHover(tooltip);
			}
			return result;
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	public static bool ButtonIconWithTextVertical(FontAwesomeIcon icon, string text, bool useStaticFont = false)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(1, 2);
		((ImU8String)(ref val)).AppendFormatted<string>(text);
		((ImU8String)(ref val)).AppendLiteral("_");
		((ImU8String)(ref val)).AppendFormatted<string>(FontAwesomeExtensions.ToIconString(icon));
		IdDisposable val2 = ImRaii.PushId(val, true);
		try
		{
			string iconText = FontAwesomeExtensions.ToIconString(icon);
			Vector2 topSize = CalcIconSize(iconText, useStaticFont);
			ReadOnlySpan<char> displaySpan = GetDisplaySpan(text);
			Vector2 bottomSize = ImGui.CalcTextSize(ImU8String.op_Implicit(displaySpan), false, -1f);
			ImGuiStylePtr style = ImGui.GetStyle();
			float x = ((ImGuiStylePtr)(ref style)).FramePadding.X;
			float spacing = 3f * ImGuiHelpers.GlobalScale;
			Vector2 vector = new Vector2(MathF.Max(topSize.X, bottomSize.X) + x * 2f, GetDoubleLineHeight());
			bool result = ImGui.Button(ImU8String.op_Implicit(string.Empty), vector);
			var (areaMin, areaSize) = GetButtonContentRect();
			var (position, vector2) = GetVerticalLayout(areaMin, areaSize, topSize, bottomSize, spacing);
			DrawIconText(position, iconText, useStaticFont);
			ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
			((ImDrawListPtr)(ref windowDrawList)).AddText(vector2, ImGui.GetColorU32((ImGuiCol)0), ImU8String.op_Implicit(displaySpan));
			return result;
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	public static bool ButtonIconWithTextVertical(FontAwesomeIcon icon, string text, Vector2 buttonSize, bool useStaticFont = false)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(1, 2);
		((ImU8String)(ref val)).AppendFormatted<string>(text);
		((ImU8String)(ref val)).AppendLiteral("_");
		((ImU8String)(ref val)).AppendFormatted<string>(FontAwesomeExtensions.ToIconString(icon));
		IdDisposable val2 = ImRaii.PushId(val, true);
		try
		{
			string iconText = FontAwesomeExtensions.ToIconString(icon);
			Vector2 topSize = CalcIconSize(iconText, useStaticFont);
			ReadOnlySpan<char> displaySpan = GetDisplaySpan(text);
			Vector2 bottomSize = ImGui.CalcTextSize(ImU8String.op_Implicit(displaySpan), false, -1f);
			float spacing = 3f * ImGuiHelpers.GlobalScale;
			bool result = ImGui.Button(ImU8String.op_Implicit(string.Empty), buttonSize);
			var (areaMin, areaSize) = GetButtonContentRect();
			var (position, vector) = GetVerticalLayout(areaMin, areaSize, topSize, bottomSize, spacing);
			DrawIconText(position, iconText, useStaticFont);
			ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
			((ImDrawListPtr)(ref windowDrawList)).AddText(vector, ImGui.GetColorU32((ImGuiCol)0), ImU8String.op_Implicit(displaySpan));
			return result;
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	public static bool ButtonIconWithText(FontAwesomeIcon icon, string text, bool useStaticFont = false)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(1, 2);
		((ImU8String)(ref val)).AppendFormatted<string>(text);
		((ImU8String)(ref val)).AppendLiteral("_");
		((ImU8String)(ref val)).AppendFormatted<string>(FontAwesomeExtensions.ToIconString(icon));
		IdDisposable val2 = ImRaii.PushId(val, true);
		try
		{
			string iconText = FontAwesomeExtensions.ToIconString(icon);
			Vector2 leadingSize = CalcIconSize(iconText, useStaticFont);
			ReadOnlySpan<char> displaySpan = GetDisplaySpan(text);
			Vector2 trailingSize = ImGui.CalcTextSize(ImU8String.op_Implicit(displaySpan), false, -1f);
			ImGuiStylePtr style = ImGui.GetStyle();
			Vector2 framePadding = ((ImGuiStylePtr)(ref style)).FramePadding;
			style = ImGui.GetStyle();
			float x = ((ImGuiStylePtr)(ref style)).ItemSpacing.X;
			Vector2 vector = new Vector2(leadingSize.X + trailingSize.X + framePadding.X * 2f + x, GetSingleLineHeight());
			bool result = ImGui.Button(ImU8String.op_Implicit(string.Empty), vector);
			var (areaMin, areaSize) = GetButtonContentRect();
			var (position, vector2) = GetHorizontalLayout(areaMin, areaSize, leadingSize, trailingSize, x);
			DrawIconText(position, iconText, useStaticFont);
			ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
			((ImDrawListPtr)(ref windowDrawList)).AddText(vector2, ImGui.GetColorU32((ImGuiCol)0), ImU8String.op_Implicit(displaySpan));
			return result;
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	public static bool ButtonIconWithText(FontAwesomeIcon icon, string text, Vector2 buttonSize, bool useStaticFont = false)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(1, 2);
		((ImU8String)(ref val)).AppendFormatted<string>(text);
		((ImU8String)(ref val)).AppendLiteral("_");
		((ImU8String)(ref val)).AppendFormatted<string>(FontAwesomeExtensions.ToIconString(icon));
		IdDisposable val2 = ImRaii.PushId(val, true);
		try
		{
			string iconText = FontAwesomeExtensions.ToIconString(icon);
			Vector2 leadingSize = CalcIconSize(iconText, useStaticFont);
			ReadOnlySpan<char> displaySpan = GetDisplaySpan(text);
			Vector2 trailingSize = ImGui.CalcTextSize(ImU8String.op_Implicit(displaySpan), false, -1f);
			ImGuiStylePtr style = ImGui.GetStyle();
			float x = ((ImGuiStylePtr)(ref style)).ItemSpacing.X;
			bool result = ImGui.Button(ImU8String.op_Implicit(string.Empty), buttonSize);
			var (areaMin, areaSize) = GetButtonContentRect();
			var (position, vector) = GetHorizontalLayout(areaMin, areaSize, leadingSize, trailingSize, x);
			DrawIconText(position, iconText, useStaticFont);
			ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
			((ImDrawListPtr)(ref windowDrawList)).AddText(vector, ImGui.GetColorU32((ImGuiCol)0), ImU8String.op_Implicit(displaySpan));
			return result;
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	public static bool ButtonImageWithTextVertical(IDalamudTextureWrap icon, string text)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(1, 2);
		((ImU8String)(ref val)).AppendFormatted<string>(text);
		((ImU8String)(ref val)).AppendLiteral("_");
		((ImU8String)(ref val)).AppendFormatted<ImTextureID>(icon.Handle);
		IdDisposable val2 = ImRaii.PushId(val, true);
		try
		{
			Vector2 size = icon.Size;
			ReadOnlySpan<char> displaySpan = GetDisplaySpan(text);
			Vector2 bottomSize = ImGui.CalcTextSize(ImU8String.op_Implicit(displaySpan), false, -1f);
			ImGuiStylePtr style = ImGui.GetStyle();
			float x = ((ImGuiStylePtr)(ref style)).FramePadding.X;
			float spacing = 3f * ImGuiHelpers.GlobalScale;
			Vector2 vector = new Vector2(MathF.Max(size.X, bottomSize.X) + x * 2f, GetDoubleLineHeight());
			bool result = ImGui.Button(ImU8String.op_Implicit(string.Empty), vector);
			(Vector2 Min, Vector2 Size) buttonContentRect = GetButtonContentRect();
			Vector2 item = buttonContentRect.Min;
			Vector2 item2 = buttonContentRect.Size;
			(Vector2 Top, Vector2 Bottom) verticalLayout = GetVerticalLayout(item, item2, size, bottomSize, spacing);
			Vector2 item3 = verticalLayout.Top;
			Vector2 item4 = verticalLayout.Bottom;
			ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
			((ImDrawListPtr)(ref windowDrawList)).AddImage(icon.Handle, item3, item3 + size);
			((ImDrawListPtr)(ref windowDrawList)).AddText(item4, ImGui.GetColorU32((ImGuiCol)0), ImU8String.op_Implicit(displaySpan));
			return result;
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	public static bool ButtonImageWithTextVertical(IDalamudTextureWrap icon, string text, Vector2 buttonSize)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(1, 2);
		((ImU8String)(ref val)).AppendFormatted<string>(text);
		((ImU8String)(ref val)).AppendLiteral("_");
		((ImU8String)(ref val)).AppendFormatted<ImTextureID>(icon.Handle);
		IdDisposable val2 = ImRaii.PushId(val, true);
		try
		{
			Vector2 size = icon.Size;
			ReadOnlySpan<char> displaySpan = GetDisplaySpan(text);
			Vector2 bottomSize = ImGui.CalcTextSize(ImU8String.op_Implicit(displaySpan), false, -1f);
			float spacing = 3f * ImGuiHelpers.GlobalScale;
			bool result = ImGui.Button(ImU8String.op_Implicit(string.Empty), buttonSize);
			(Vector2 Min, Vector2 Size) buttonContentRect = GetButtonContentRect();
			Vector2 item = buttonContentRect.Min;
			Vector2 item2 = buttonContentRect.Size;
			(Vector2 Top, Vector2 Bottom) verticalLayout = GetVerticalLayout(item, item2, size, bottomSize, spacing);
			Vector2 item3 = verticalLayout.Top;
			Vector2 item4 = verticalLayout.Bottom;
			ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
			((ImDrawListPtr)(ref windowDrawList)).AddImage(icon.Handle, item3, item3 + size);
			((ImDrawListPtr)(ref windowDrawList)).AddText(item4, ImGui.GetColorU32((ImGuiCol)0), ImU8String.op_Implicit(displaySpan));
			return result;
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	public static bool ButtonIconSelectable(string id, FontAwesomeIcon icon, string tooltip = "", bool useStaticFont = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		IdDisposable val = ImRaii.PushId(ImU8String.op_Implicit(id), true);
		try
		{
			ImGuiStylePtr style = ImGui.GetStyle();
			Span<Vector4> colors = ((ImGuiStylePtr)(ref style)).Colors;
			string text = FontAwesomeExtensions.ToIconString(icon);
			Vector2 vector = new Vector2(ImGui.GetContentRegionAvail().X, GetSingleLineHeight());
			ColorDisposable val2 = ImRaii.PushColor((ImGuiCol)23, colors[26].ToUInt(), true).Push((ImGuiCol)22, colors[25].ToUInt(), true).Push((ImGuiCol)21, 0u, true);
			try
			{
				FontDisposable val3 = ImRaii.PushFont(UiBuilder.IconFont, useStaticFont);
				bool result;
				try
				{
					ImU8String val4 = new ImU8String(3, 3);
					((ImU8String)(ref val4)).AppendFormatted<string>(text);
					((ImU8String)(ref val4)).AppendLiteral("##");
					((ImU8String)(ref val4)).AppendFormatted<string>(text);
					((ImU8String)(ref val4)).AppendLiteral("-");
					((ImU8String)(ref val4)).AppendFormatted<string>(id);
					result = ImGui.Button(val4, vector);
				}
				finally
				{
					((IDisposable)val3)?.Dispose();
				}
				if (!StringExtensions.IsNullOrEmpty(tooltip))
				{
					TooltipHover(tooltip);
				}
				return result;
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static bool ButtonSelectable(string text)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		ImGuiStylePtr style = ImGui.GetStyle();
		Vector2 framePadding = ((ImGuiStylePtr)(ref style)).FramePadding;
		Span<Vector4> colors = ((ImGuiStylePtr)(ref style)).Colors;
		Vector2 vector = ImGui.CalcTextSize(ImU8String.op_Implicit(text), false, -1f);
		Vector2 vector2 = new Vector2(MathF.Max(ImGui.GetContentRegionAvail().X, vector.X + 2f * framePadding.X), GetSingleLineHeight());
		ColorDisposable val = ImRaii.PushColor((ImGuiCol)23, colors[26].ToUInt(), true).Push((ImGuiCol)22, colors[25].ToUInt(), true).Push((ImGuiCol)21, 0u, true);
		try
		{
			return ImGui.Button(ImU8String.op_Implicit(text), vector2);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static bool ButtonStretch(string text)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		Vector2 vector = new Vector2(ImGui.GetContentRegionAvail().X, GetSingleLineHeight());
		return ImGui.Button(ImU8String.op_Implicit(text), vector);
	}

	public static bool HoldButton(string text, bool autoReset = true)
	{
		return HoldButton(text, text, autoReset);
	}

	public static bool HoldButton(string text, bool autoReset, Vector2 size, float duration = 1f)
	{
		return HoldButton(text, text, autoReset, size, duration);
	}

	public static bool HoldButton(string id, string text, Vector4? chargeColor, bool autoReset = true, Vector2 size = default(Vector2), float duration = 1f)
	{
		return HoldButton(id, text, autoReset, size, duration, chargeColor.HasValue ? new uint?(chargeColor.GetValueOrDefault().ToUInt()) : ((uint?)null));
	}

	public static bool HoldButton(string id, string text, bool autoReset = true, Vector2 size = default(Vector2), float duration = 1f, uint? chargeColor = null)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		(float Charge, float FlashTime) valueOrDefault = HoldButtonStates.GetValueOrDefault(id);
		float item = valueOrDefault.Charge;
		float num = valueOrDefault.FlashTime;
		ReadOnlySpan<char> displaySpan = GetDisplaySpan(text);
		Vector2 contentSize = ImGui.CalcTextSize(ImU8String.op_Implicit(displaySpan), false, -1f);
		ImGuiStylePtr style = ImGui.GetStyle();
		Vector2 framePadding = ((ImGuiStylePtr)(ref style)).FramePadding;
		Vector2 vector = new Vector2((size.X > 0f) ? size.X : (contentSize.X + framePadding.X * 2f), (size.Y > 0f) ? size.Y : GetSingleLineHeight());
		IdDisposable val = ImRaii.PushId(ImU8String.op_Implicit(id), true);
		try
		{
			ImGui.InvisibleButton(ImU8String.op_Implicit("##hold"), vector, (ImGuiButtonFlags)0);
			bool flag = ImGui.IsItemActive();
			bool flag2 = ImGui.IsItemHovered();
			float num2 = ((duration > 0f) ? duration : 1f);
			float num3 = 1f / num2;
			float num4 = num3 * 1.5f;
			bool result = false;
			ImGuiIOPtr iO;
			if (num > 0f)
			{
				float num5 = num;
				iO = ImGui.GetIO();
				num = num5 - ((ImGuiIOPtr)(ref iO)).DeltaTime;
				item = 1f;
				if (num <= 0f)
				{
					num = 0f;
					result = true;
					if (autoReset)
					{
						item = 0f;
					}
				}
			}
			else if (flag)
			{
				float num6 = item;
				iO = ImGui.GetIO();
				item = MathF.Min(num6 + num3 * ((ImGuiIOPtr)(ref iO)).DeltaTime, 1f);
				if (item >= 1f)
				{
					num = 0.5f;
				}
			}
			else
			{
				float num7 = item;
				iO = ImGui.GetIO();
				item = MathF.Max(num7 - num4 * ((ImGuiIOPtr)(ref iO)).DeltaTime, 0f);
			}
			HoldButtonStates[id] = (item, num);
			Vector2 itemRectMin = ImGui.GetItemRectMin();
			Vector2 itemRectMax = ImGui.GetItemRectMax();
			Vector2 areaSize = itemRectMax - itemRectMin;
			ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
			ImGuiStylePtr style2 = ImGui.GetStyle();
			Span<Vector4> colors = ((ImGuiStylePtr)(ref style2)).Colors;
			float frameRounding = ((ImGuiStylePtr)(ref style2)).FrameRounding;
			bool flag3 = num > 0f;
			float x = (flag3 ? (num / 0.5f) : 0f);
			uint num8 = ((flag || flag3) ? Vector4.Lerp(colors[22], colors[23], 0.5f + 0.5f * item).ToUInt() : (flag2 ? ImGui.GetColorU32((ImGuiCol)22) : ImGui.GetColorU32((ImGuiCol)21)));
			((ImDrawListPtr)(ref windowDrawList)).AddRectFilled(itemRectMin, itemRectMax, num8, frameRounding);
			float num9 = ((((ImGuiStylePtr)(ref style2)).FrameBorderSize > 0f) ? ((ImGuiStylePtr)(ref style2)).FrameBorderSize : 1f);
			if (((ImGuiStylePtr)(ref style2)).FrameBorderSize > 0f || flag || flag2 || flag3)
			{
				((ImDrawListPtr)(ref windowDrawList)).AddRect(itemRectMin, itemRectMax, colors[5].ToUInt(), frameRounding, (ImDrawFlags)0, num9);
			}
			if (item > 0f)
			{
				uint num10 = chargeColor ?? ImGui.GetColorU32((ImGuiCol)18);
				if (flag3)
				{
					float num11 = MathF.Pow(x, 0.5f);
					num10 = Vector4.Lerp(num10.ToVector4(), new Vector4(1f, 1f, 1f, 1f), 0.6f * num11).ToUInt();
					AddGlowRect(windowDrawList, itemRectMin, itemRectMax, num10, frameRounding, 5f * ImGuiHelpers.GlobalScale * num11, 4);
				}
				float num12 = MathF.Max(num9 * 1.5f, 2f * ImGuiHelpers.GlobalScale);
				if (flag3)
				{
					num12 += 1.5f * ImGuiHelpers.GlobalScale * MathF.Pow(x, 0.5f);
				}
				DrawHoldProgressBorder(windowDrawList, itemRectMin, itemRectMax, frameRounding, item, num10, num12);
			}
			Vector2 centeredPosition = GetCenteredPosition(itemRectMin, areaSize, contentSize);
			((ImDrawListPtr)(ref windowDrawList)).AddText(centeredPosition, colors[0].ToUInt(), ImU8String.op_Implicit(displaySpan));
			return result;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private static void DrawHoldProgressBorder(ImDrawListPtr draw, Vector2 min, Vector2 max, float rounding, float charge, uint chargeColor, float thickness)
	{
		if (charge <= 0f)
		{
			return;
		}
		if (charge >= 1f)
		{
			((ImDrawListPtr)(ref draw)).AddRect(min, max, chargeColor, rounding, (ImDrawFlags)0, thickness);
			return;
		}
		Vector2 vector = max - min;
		float num = MathF.Max(0f, MathF.Min(rounding, MathF.Min(vector.X * 0.5f, vector.Y * 0.5f)));
		float num2 = MathF.Max(0f, vector.X - 2f * num);
		float num3 = MathF.Max(0f, vector.Y - 2f * num) * 0.5f;
		float num4 = ((num > 0f) ? (num * ((float)Math.PI / 2f)) : 0f);
		float num5 = num3 + num4 + num2 + num4 + num3;
		if (num5 <= 0f)
		{
			return;
		}
		float num6 = charge * num5;
		Vector2 vector2 = new Vector2(min.X, min.Y + vector.Y * 0.5f);
		float num7 = num6;
		((ImDrawListPtr)(ref draw)).PathClear();
		((ImDrawListPtr)(ref draw)).PathLineTo(vector2);
		if (num7 <= num3)
		{
			((ImDrawListPtr)(ref draw)).PathLineTo(new Vector2(min.X, vector2.Y - num7));
			((ImDrawListPtr)(ref draw)).PathStroke(chargeColor, (ImDrawFlags)0, thickness);
		}
		else
		{
			num7 -= num3;
			if (num > 0f)
			{
				Vector2 vector3 = new Vector2(min.X + num, min.Y + num);
				if (num7 <= num4)
				{
					float num8 = (float)Math.PI + num7 / num;
					int num9 = Math.Clamp((int)(16f * (num7 / num4)), 4, 16);
					((ImDrawListPtr)(ref draw)).PathArcTo(vector3, num, (float)Math.PI, num8, num9);
					((ImDrawListPtr)(ref draw)).PathStroke(chargeColor, (ImDrawFlags)0, thickness);
					num7 = 0f;
				}
				else
				{
					((ImDrawListPtr)(ref draw)).PathArcTo(vector3, num, (float)Math.PI, 4.712389f, 16);
					num7 -= num4;
				}
			}
			if (num7 > 0f)
			{
				if (num7 <= num2)
				{
					((ImDrawListPtr)(ref draw)).PathLineTo(new Vector2(min.X + num + num7, min.Y));
					((ImDrawListPtr)(ref draw)).PathStroke(chargeColor, (ImDrawFlags)0, thickness);
				}
				else
				{
					num7 -= num2;
					if (num > 0f)
					{
						Vector2 vector4 = new Vector2(max.X - num, min.Y + num);
						if (num7 <= num4)
						{
							float num10 = 4.712389f + num7 / num;
							int num11 = Math.Clamp((int)(16f * (num7 / num4)), 4, 16);
							((ImDrawListPtr)(ref draw)).PathArcTo(vector4, num, 4.712389f, num10, num11);
							((ImDrawListPtr)(ref draw)).PathStroke(chargeColor, (ImDrawFlags)0, thickness);
							num7 = 0f;
						}
						else
						{
							((ImDrawListPtr)(ref draw)).PathArcTo(vector4, num, 4.712389f, (float)Math.PI * 2f, 16);
							num7 -= num4;
						}
					}
					if (num7 > 0f)
					{
						((ImDrawListPtr)(ref draw)).PathLineTo(new Vector2(max.X, min.Y + num + MathF.Min(num7, num3)));
						((ImDrawListPtr)(ref draw)).PathStroke(chargeColor, (ImDrawFlags)0, thickness);
					}
				}
			}
		}
		float num12 = num6;
		((ImDrawListPtr)(ref draw)).PathClear();
		((ImDrawListPtr)(ref draw)).PathLineTo(vector2);
		if (num12 <= num3)
		{
			((ImDrawListPtr)(ref draw)).PathLineTo(new Vector2(min.X, vector2.Y + num12));
			((ImDrawListPtr)(ref draw)).PathStroke(chargeColor, (ImDrawFlags)0, thickness);
			return;
		}
		num12 -= num3;
		if (num > 0f)
		{
			Vector2 vector5 = new Vector2(min.X + num, max.Y - num);
			if (num12 <= num4)
			{
				float num13 = (float)Math.PI - num12 / num;
				int num14 = Math.Clamp((int)(16f * (num12 / num4)), 4, 16);
				((ImDrawListPtr)(ref draw)).PathArcTo(vector5, num, (float)Math.PI, num13, num14);
				((ImDrawListPtr)(ref draw)).PathStroke(chargeColor, (ImDrawFlags)0, thickness);
				num12 = 0f;
			}
			else
			{
				((ImDrawListPtr)(ref draw)).PathArcTo(vector5, num, (float)Math.PI, (float)Math.PI / 2f, 16);
				num12 -= num4;
			}
		}
		if (!(num12 > 0f))
		{
			return;
		}
		if (num12 <= num2)
		{
			((ImDrawListPtr)(ref draw)).PathLineTo(new Vector2(min.X + num + num12, max.Y));
			((ImDrawListPtr)(ref draw)).PathStroke(chargeColor, (ImDrawFlags)0, thickness);
			return;
		}
		num12 -= num2;
		if (num > 0f)
		{
			Vector2 vector6 = new Vector2(max.X - num, max.Y - num);
			if (num12 <= num4)
			{
				float num15 = (float)Math.PI / 2f - num12 / num;
				int num16 = Math.Clamp((int)(16f * (num12 / num4)), 4, 16);
				((ImDrawListPtr)(ref draw)).PathArcTo(vector6, num, (float)Math.PI / 2f, num15, num16);
				((ImDrawListPtr)(ref draw)).PathStroke(chargeColor, (ImDrawFlags)0, thickness);
				num12 = 0f;
			}
			else
			{
				((ImDrawListPtr)(ref draw)).PathArcTo(vector6, num, (float)Math.PI / 2f, 0f, 16);
				num12 -= num4;
			}
		}
		if (num12 > 0f)
		{
			((ImDrawListPtr)(ref draw)).PathLineTo(new Vector2(max.X, max.Y - num - MathF.Min(num12, num3)));
			((ImDrawListPtr)(ref draw)).PathStroke(chargeColor, (ImDrawFlags)0, thickness);
		}
	}

	public static bool CheckboxColored(string label, ref bool selected, uint? enabledColor = null, uint? disabledColor = null)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		ColorDisposable val = ImRaii.PushColor((ImGuiCol)0, enabledColor ?? KnownColor.Yellow.ToUInt(), selected);
		try
		{
			ColorDisposable val2 = ImRaii.PushColor((ImGuiCol)0, disabledColor ?? KnownColor.White.ToUInt(), !selected);
			try
			{
				return ImGui.Checkbox(ImU8String.op_Implicit(label), ref selected);
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static bool CheckboxColored(string label, ref bool selected, Vector4? enabledColor, Vector4? disabledColor = null)
	{
		return CheckboxColored(label, ref selected, enabledColor.HasValue ? new uint?(enabledColor.GetValueOrDefault().ToUInt()) : ((uint?)null), disabledColor.HasValue ? new uint?(disabledColor.GetValueOrDefault().ToUInt()) : ((uint?)null));
	}

	public static void CircleOutlined(Vector2 center, float radius, uint fillColor, uint outlineColor = 4278190080u, float outlineThickness = 1.5f, float opacity = 1f, ImDrawListPtr? drawList = null)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		ImDrawListPtr value = drawList.GetValueOrDefault();
		if (!drawList.HasValue)
		{
			value = ImGui.GetBackgroundDrawList();
			drawList = value;
		}
		Vector4 vector = fillColor.ToVector4();
		Vector4 vector2 = outlineColor.ToVector4();
		Vector4 vector3 = vector;
		vector3.W = vector.W * opacity;
		Vector4 color = vector3;
		vector3 = vector2;
		vector3.W = vector2.W * opacity;
		Vector4 color2 = vector3;
		if (drawList.HasValue)
		{
			value = drawList.GetValueOrDefault();
			((ImDrawListPtr)(ref value)).AddCircleFilled(center, radius + outlineThickness, color2.ToUInt());
		}
		if (drawList.HasValue)
		{
			value = drawList.GetValueOrDefault();
			((ImDrawListPtr)(ref value)).AddCircleFilled(center, radius, color.ToUInt());
		}
	}

	public static void CircleOutlined(Vector2 center, float radius, Vector4 fillColor, Vector4? outlineColor = null, float outlineThickness = 1.5f, float opacity = 1f, ImDrawListPtr? drawList = null)
	{
		CircleOutlined(center, radius, fillColor.ToUInt(), outlineColor.HasValue ? outlineColor.GetValueOrDefault().ToUInt() : 4278190080u, outlineThickness, opacity, drawList);
	}

	public static uint GetGradientColor()
	{
		float num = (float)ImGui.GetTime() % 1f / 1f;
		float x = (MathF.Sin((float)Math.PI * 2f * num) + 1f) / 2f;
		float y = (MathF.Sin((float)Math.PI * 2f * (num + 1f / 3f)) + 1f) / 2f;
		float z = (MathF.Sin((float)Math.PI * 2f * (num + 2f / 3f)) + 1f) / 2f;
		return new Vector4(x, y, z, 1f).ToUInt();
	}

	public unsafe static void RenderPlayerInfo(string name, string world)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		GroupDisposable val = ImRaii.Group();
		try
		{
			ReadOnlySpan<byte> readOnlySpan = ReadOnlySeString.op_Implicit(RenderedPlayerInfos.GetOrAdd((name, world), delegate
			{
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_0026: Unknown result type (might be due to invalid IL or missing references)
				//IL_0038: Unknown result type (might be due to invalid IL or missing references)
				//IL_003d: Unknown result type (might be due to invalid IL or missing references)
				//IL_004b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0051: Unknown result type (might be due to invalid IL or missing references)
				//IL_0056: Unknown result type (might be due to invalid IL or missing references)
				//IL_0068: Unknown result type (might be due to invalid IL or missing references)
				RentedSeStringBuilder val3 = default(RentedSeStringBuilder);
				((RentedSeStringBuilder)(ref val3))._002Ector();
				try
				{
					SeStringBuilder builder = ((RentedSeStringBuilder)(ref val3)).Builder;
					SeStringInterpolatedStringHandler val4 = new SeStringInterpolatedStringHandler(0, 1, builder);
					((SeStringInterpolatedStringHandler)(ref val4)).AppendFormatted<string>(name);
					SeStringBuilder val5 = builder.Append(val4).AppendIcon(88u);
					val4 = new SeStringInterpolatedStringHandler(0, 1, val5);
					((SeStringInterpolatedStringHandler)(ref val4)).AppendFormatted<string>(world);
					return val5.Append(val4).ToReadOnlySeString();
				}
				finally
				{
					((IDisposable)(*(RentedSeStringBuilder*)(&val3))/*cast due to constrained. prefix*/).Dispose();
				}
			}));
			SeStringDrawParams val2 = default(SeStringDrawParams);
			ImGuiHelpers.SeStringWrapped(readOnlySpan, ref val2, default(ImGuiId), (ImGuiButtonFlags)1);
		}
		finally
		{
			((GroupDisposable)(ref val)).Dispose();
		}
	}

	public static void AddGlowRect(ImDrawListPtr drawList, Vector2 min, Vector2 max, uint colU32, float rounding, float glowSize, int steps, float innerBoost = 0.25f)
	{
		for (int i = 0; i < steps; i++)
		{
			float num = ((float)i + 1f) / (float)steps;
			float num2 = num * glowSize;
			float num3 = 1f - num;
			num3 *= num3;
			num3 *= 1f / (float)steps * 2.2f;
			num3 *= 1f + (1f - num) * innerBoost;
			Vector4 color = colU32.ToVector4();
			color.W *= num3;
			uint num4 = color.ToUInt();
			float num5 = rounding + num2 * 0.6f;
			((ImDrawListPtr)(ref drawList)).AddRectFilled(min - new Vector2(num2), max + new Vector2(num2), num4, num5);
		}
	}

	public static void ScaledDummy(float x)
	{
		ImGui.Dummy(new Vector2(x) * Globals.GlobalUIScale);
	}

	public static void ScaledDummy(float x, float y)
	{
		ImGui.Dummy(new Vector2(x, y) * Globals.GlobalUIScale);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ReadOnlySpan<char> GetDisplaySpan(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return default(ReadOnlySpan<char>);
		}
		ReadOnlySpan<char> readOnlySpan = text.AsSpan();
		int num = readOnlySpan.IndexOf("##".AsSpan());
		if (num != -1)
		{
			return readOnlySpan.Slice(0, num);
		}
		return readOnlySpan;
	}

	public static void ClickToCopy(string textToCopy, ImGuiMouseButton mouseButton = (ImGuiMouseButton)1, ImGuiKey? key = null, ImGuiKey? keyConflict = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (ImGui.IsItemClicked(mouseButton) && (!key.HasValue || ImGui.IsKeyDown(key.Value)) && (!keyConflict.HasValue || !ImGui.IsKeyDown(keyConflict.Value)))
		{
			ImGui.SetClipboardText(ImU8String.op_Implicit(textToCopy));
		}
	}

	public static void CenterAlignFor(float itemWidth)
	{
		ImGui.SetCursorPosX(ImGui.GetCursorPosX() + (ImGui.GetContentRegionAvail().X - itemWidth) / 2f);
	}

	public static void RightAlignFor(float itemWidth)
	{
		ImGui.SetCursorPosX(ImGui.GetCursorPosX() + (ImGui.GetContentRegionAvail().X - itemWidth));
	}

	private static float GetSingleLineHeight()
	{
		return ImGui.GetFrameHeight();
	}

	private static float GetDoubleLineHeight()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		float num = ImGui.GetFrameHeight() * 2f;
		ImGuiStylePtr style = ImGui.GetStyle();
		return num + ((ImGuiStylePtr)(ref style)).ItemSpacing.Y;
	}

	private static (Vector2 Min, Vector2 Size) GetItemRect()
	{
		Vector2 itemRectMin = ImGui.GetItemRectMin();
		Vector2 itemRectMax = ImGui.GetItemRectMax();
		return (Min: itemRectMin, Size: itemRectMax - itemRectMin);
	}

	private static (Vector2 Min, Vector2 Size) GetButtonContentRect()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		(Vector2 Min, Vector2 Size) itemRect = GetItemRect();
		Vector2 item = itemRect.Min;
		Vector2 item2 = itemRect.Size;
		ImGuiStylePtr style = ImGui.GetStyle();
		Vector2 framePadding = ((ImGuiStylePtr)(ref style)).FramePadding;
		return new ValueTuple<Vector2, Vector2>(item2: new Vector2(MathF.Max(item2.X - framePadding.X * 2f, 0f), MathF.Max(item2.Y - framePadding.Y * 2f, 0f)), item1: item + framePadding);
	}

	private static Vector2 GetCenteredPosition(Vector2 areaMin, Vector2 areaSize, Vector2 contentSize)
	{
		Vector2 vector = new Vector2(MathF.Max((areaSize.X - contentSize.X) * 0.5f, 0f), MathF.Max((areaSize.Y - contentSize.Y) * 0.5f, 0f));
		return areaMin + vector;
	}

	private static (Vector2 Leading, Vector2 Trailing) GetHorizontalLayout(Vector2 areaMin, Vector2 areaSize, Vector2 leadingSize, Vector2 trailingSize, float spacing)
	{
		Vector2 vector = GetCenteredPosition(contentSize: new Vector2(leadingSize.X + trailingSize.X + spacing, MathF.Max(leadingSize.Y, trailingSize.Y)), areaMin: areaMin, areaSize: areaSize);
		Vector2 vector2 = vector;
		vector2.Y = areaMin.Y + MathF.Max((areaSize.Y - leadingSize.Y) * 0.5f, 0f);
		Vector2 item = vector2;
		Vector2 item2 = new Vector2(vector.X + leadingSize.X + spacing, areaMin.Y + MathF.Max((areaSize.Y - trailingSize.Y) * 0.5f, 0f));
		return (Leading: item, Trailing: item2);
	}

	private static (Vector2 Top, Vector2 Bottom) GetVerticalLayout(Vector2 areaMin, Vector2 areaSize, Vector2 topSize, Vector2 bottomSize, float spacing)
	{
		Vector2 vector = GetCenteredPosition(contentSize: new Vector2(MathF.Max(topSize.X, bottomSize.X), topSize.Y + bottomSize.Y + spacing), areaMin: areaMin, areaSize: areaSize);
		Vector2 vector2 = vector;
		vector2.X = areaMin.X + MathF.Max((areaSize.X - topSize.X) * 0.5f, 0f);
		Vector2 item = vector2;
		Vector2 item2 = new Vector2(areaMin.X + MathF.Max((areaSize.X - bottomSize.X) * 0.5f, 0f), vector.Y + topSize.Y + spacing);
		return (Top: item, Bottom: item2);
	}

	private static Vector2 CalcIconSize(string iconText, bool useStaticFont)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		FontDisposable val = ImRaii.PushFont(UiBuilder.IconFont, useStaticFont);
		try
		{
			return ImGui.CalcTextSize(ImU8String.op_Implicit(iconText), false, -1f);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private static void DrawIconText(Vector2 position, string iconText, bool useStaticFont)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		FontDisposable val = ImRaii.PushFont(UiBuilder.IconFont, useStaticFont);
		try
		{
			ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
			((ImDrawListPtr)(ref windowDrawList)).AddText(position, ImGui.GetColorU32((ImGuiCol)0), ImU8String.op_Implicit(iconText));
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static ModalPopupDisposable PopupModal(string title, ref bool open, ImGuiWindowFlags flags = (ImGuiWindowFlags)0)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		if (!open)
		{
			return new ModalPopupDisposable(success: false);
		}
		ImGuiViewportPtr mainViewport = ImGui.GetMainViewport();
		string obj = $"OmModalBlocker_{ImGui.GetID(ImU8String.op_Implicit(title))}";
		ImGui.SetNextWindowPos(((ImGuiViewportPtr)(ref mainViewport)).Pos);
		ImGui.SetNextWindowSize(((ImGuiViewportPtr)(ref mainViewport)).Size);
		if (ImGui.Begin(ImU8String.op_Implicit(obj), (ImGuiWindowFlags)2896319))
		{
			ImGui.SetCursorScreenPos(((ImGuiViewportPtr)(ref mainViewport)).Pos);
			ImGui.InvisibleButton(ImU8String.op_Implicit("##OmModalBlockerSurface"), ((ImGuiViewportPtr)(ref mainViewport)).Size, (ImGuiButtonFlags)0);
			ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
			((ImDrawListPtr)(ref windowDrawList)).PushClipRect(((ImGuiViewportPtr)(ref mainViewport)).Pos - Vector2.One, ((ImGuiViewportPtr)(ref mainViewport)).Pos + ((ImGuiViewportPtr)(ref mainViewport)).Size + Vector2.One, false);
			((ImDrawListPtr)(ref windowDrawList)).AddRectFilled(((ImGuiViewportPtr)(ref mainViewport)).Pos, ((ImGuiViewportPtr)(ref mainViewport)).Pos + ((ImGuiViewportPtr)(ref mainViewport)).Size, ImGui.GetColorU32((ImGuiCol)54));
			((ImDrawListPtr)(ref windowDrawList)).PopClipRect();
		}
		ImGuiWindowPtr currentWindow = ImGuiP.GetCurrentWindow();
		ImGui.End();
		ImGui.SetNextWindowPos(ImGui.GetCenter(mainViewport), (ImGuiCond)8, new Vector2(0.5f));
		bool success = ImGui.Begin(ImU8String.op_Implicit(title), ref open, (ImGuiWindowFlags)(flags | 0x20 | 0x100 | 0x200000));
		ImGuiWindowPtr currentWindow2 = ImGuiP.GetCurrentWindow();
		OrderModalWindows(currentWindow, currentWindow2);
		return new ModalPopupDisposable(success, inWindowBegun: true, currentWindow, currentWindow2);
	}

	private static void OrderModalWindows(ImGuiWindowPtr blockerWindow, ImGuiWindowPtr modalWindow)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		ImGuiP.BringWindowToDisplayBehind(blockerWindow, modalWindow);
		int num = ImGuiP.FindWindowDisplayIndex(modalWindow);
		if (num < 0)
		{
			return;
		}
		ImGuiContextPtr currentContext = ImGui.GetCurrentContext();
		ref ImVector<ImGuiWindowPtr> windows = ref ((ImGuiContextPtr)(ref currentContext)).Windows;
		for (int i = num + 1; i < windows.Size; i++)
		{
			ImGuiWindowPtr val = windows[i];
			if (!ImGuiP.IsWindowWithinBeginStackOf(val, modalWindow))
			{
				ImGuiP.BringWindowToDisplayBehind(val, blockerWindow);
			}
		}
	}

	public static bool Selectable(string text, bool selected = false, ImGuiSelectableFlags flags = (ImGuiSelectableFlags)0)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		float cursorPosY = ImGui.GetCursorPosY();
		ImGuiStylePtr style = ImGui.GetStyle();
		ImGui.SetCursorPosY(cursorPosY + ((ImGuiStylePtr)(ref style)).FramePadding.Y);
		return ImGui.Selectable(ImU8String.op_Implicit(text), selected, flags, default(Vector2));
	}

	public static bool Selectable(string text, ref bool selected, ImGuiSelectableFlags flags = (ImGuiSelectableFlags)0)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		float cursorPosY = ImGui.GetCursorPosY();
		ImGuiStylePtr style = ImGui.GetStyle();
		ImGui.SetCursorPosY(cursorPosY + ((ImGuiStylePtr)(ref style)).FramePadding.Y);
		return ImGui.Selectable(ImU8String.op_Implicit(text), ref selected, flags, default(Vector2));
	}

	public static bool SelectableFillCell(string text, bool selected = false, ImGuiSelectableFlags flags = (ImGuiSelectableFlags)0)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		Vector2 vector = ImGui.CalcTextSize(ImU8String.op_Implicit(text), false, -1f);
		ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
		Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
		ImGuiStylePtr style = ImGui.GetStyle();
		float x = ((ImGuiStylePtr)(ref style)).FramePadding.X;
		float y = vector.Y + 2f * x;
		bool result = ImGui.Selectable(ImU8String.op_Implicit(string.Empty), selected, flags, new Vector2(0f, y));
		Vector2 vector2 = cursorScreenPos;
		vector2.Y = cursorScreenPos.Y + x;
		Vector2 vector3 = vector2;
		((ImDrawListPtr)(ref windowDrawList)).AddText(vector3, ImGui.GetColorU32((ImGuiCol)0), ImU8String.op_Implicit(text));
		return result;
	}

	public static bool SelectableFillCell(string text, ref bool selected, ImGuiSelectableFlags flags = (ImGuiSelectableFlags)0)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		Vector2 vector = ImGui.CalcTextSize(ImU8String.op_Implicit(text), false, -1f);
		ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
		Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
		ImGuiStylePtr style = ImGui.GetStyle();
		float x = ((ImGuiStylePtr)(ref style)).FramePadding.X;
		float y = vector.Y + 2f * x;
		bool result = ImGui.Selectable(ImU8String.op_Implicit(""), ref selected, flags, new Vector2(0f, y));
		Vector2 vector2 = cursorScreenPos;
		vector2.Y = cursorScreenPos.Y + x;
		Vector2 vector3 = vector2;
		((ImDrawListPtr)(ref windowDrawList)).AddText(vector3, ImGui.GetColorU32((ImGuiCol)0), ImU8String.op_Implicit(text));
		return result;
	}

	public static bool SelectableCentered(string text, bool selected = false, ImGuiSelectableFlags flags = (ImGuiSelectableFlags)0, Vector2 size = default(Vector2))
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		ImGui.SetCursorPosX(ImGui.GetCursorPosX() + ImGui.GetContentRegionAvail().X / 2f - ImGui.CalcTextSize(ImU8String.op_Implicit(text), false, -1f).X / 2f);
		return ImGui.Selectable(ImU8String.op_Implicit(text), selected, flags, size);
	}

	public static bool SelectableCentered(string text, ref bool selected, ImGuiSelectableFlags flags = (ImGuiSelectableFlags)0, Vector2 size = default(Vector2))
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		ImGui.SetCursorPosX(ImGui.GetCursorPosX() + ImGui.GetContentRegionAvail().X / 2f - ImGui.CalcTextSize(ImU8String.op_Implicit(text), false, -1f).X / 2f);
		return ImGui.Selectable(ImU8String.op_Implicit(text), ref selected, flags, size);
	}

	public static bool SelectableImageWithText(nint imageHandle, Vector2 imageSize, string text, bool selected, ImGuiSelectableFlags flags = (ImGuiSelectableFlags)0)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		return SelectableImageWithText(new ImTextureID((IntPtr)imageHandle), imageSize, text, selected, flags);
	}

	public static bool SelectableImageWithText(ImTextureID imageHandle, Vector2 imageSize, string text, bool selected, ImGuiSelectableFlags flags = (ImGuiSelectableFlags)0)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(1, 2);
		((ImU8String)(ref val)).AppendFormatted<ImTextureID>(imageHandle);
		((ImU8String)(ref val)).AppendLiteral("_");
		((ImU8String)(ref val)).AppendFormatted<string>(text);
		IdDisposable val2 = ImRaii.PushId(val, true);
		try
		{
			ImGuiStylePtr style = ImGui.GetStyle();
			float y = ((ImGuiStylePtr)(ref style)).FramePadding.Y;
			ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
			Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
			Vector2 vector = ImGui.CalcTextSize(ImU8String.op_Implicit(text), false, -1f);
			float num = Math.Max(imageSize.Y, vector.Y);
			float y2 = num + y * 2f;
			Vector2 vector2 = new Vector2(0f, y2);
			bool result = ImGui.Selectable(ImU8String.op_Implicit(string.Empty), selected, flags, vector2);
			float x = cursorScreenPos.X + ((ImGuiStylePtr)(ref style)).FramePadding.X;
			float y3 = cursorScreenPos.Y + y + (num - imageSize.Y) / 2f;
			Vector2 vector3 = new Vector2(x, y3);
			float x2 = vector3.X + imageSize.X + ((ImGuiStylePtr)(ref style)).ItemSpacing.X;
			float y4 = cursorScreenPos.Y + y + (num - vector.Y) / 2f;
			Vector2 vector4 = new Vector2(x2, y4);
			((ImDrawListPtr)(ref windowDrawList)).AddImage(imageHandle, vector3, new Vector2(vector3.X + imageSize.X, vector3.Y + imageSize.Y), Vector2.Zero, Vector2.One, uint.MaxValue);
			((ImDrawListPtr)(ref windowDrawList)).AddText(vector4, ImGui.GetColorU32((ImGuiCol)0), ImU8String.op_Implicit(text));
			return result;
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	public static bool SelectableImageWithText(ImTextureID imageHandle, Vector2 imageSize, string text, ref bool selected, ImGuiSelectableFlags flags = (ImGuiSelectableFlags)0)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(1, 2);
		((ImU8String)(ref val)).AppendFormatted<ImTextureID>(imageHandle);
		((ImU8String)(ref val)).AppendLiteral("_");
		((ImU8String)(ref val)).AppendFormatted<string>(text);
		IdDisposable val2 = ImRaii.PushId(val, true);
		try
		{
			ImGuiStylePtr style = ImGui.GetStyle();
			float y = ((ImGuiStylePtr)(ref style)).FramePadding.Y;
			ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
			Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
			Vector2 vector = ImGui.CalcTextSize(ImU8String.op_Implicit(text), false, -1f);
			float num = Math.Max(imageSize.Y, vector.Y);
			float y2 = num + y * 2f;
			Vector2 vector2 = new Vector2(0f, y2);
			bool result = ImGui.Selectable(ImU8String.op_Implicit(string.Empty), ref selected, flags, vector2);
			float x = cursorScreenPos.X + ((ImGuiStylePtr)(ref style)).FramePadding.X;
			float y3 = cursorScreenPos.Y + y + (num - imageSize.Y) / 2f;
			Vector2 vector3 = new Vector2(x, y3);
			float x2 = vector3.X + imageSize.X + ((ImGuiStylePtr)(ref style)).ItemSpacing.X;
			float y4 = cursorScreenPos.Y + y + (num - vector.Y) / 2f;
			Vector2 vector4 = new Vector2(x2, y4);
			((ImDrawListPtr)(ref windowDrawList)).AddImage(imageHandle, vector3, new Vector2(vector3.X + imageSize.X, vector3.Y + imageSize.Y), Vector2.Zero, Vector2.One, uint.MaxValue);
			((ImDrawListPtr)(ref windowDrawList)).AddText(vector4, ImGui.GetColorU32((ImGuiCol)0), ImU8String.op_Implicit(text));
			return result;
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	public static bool SelectableTextCentered(string text, bool selected = false, ImGuiSelectableFlags flags = (ImGuiSelectableFlags)0)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(1, 2);
		((ImU8String)(ref val)).AppendFormatted<string>(text);
		((ImU8String)(ref val)).AppendLiteral("_");
		((ImU8String)(ref val)).AppendFormatted<ImGuiSelectableFlags>(flags);
		ImGui.PushID(val);
		Vector2 vector = ImGui.CalcTextSize(ImU8String.op_Implicit(text), false, -1f);
		ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
		Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
		ImGuiStylePtr style = ImGui.GetStyle();
		float x = ((ImGuiStylePtr)(ref style)).FramePadding.X;
		float y = vector.Y + 2f * x;
		bool result = ImGui.Selectable(ImU8String.op_Implicit(string.Empty), selected, flags, new Vector2(0f, y));
		Vector2 vector2 = new Vector2(cursorScreenPos.X + (ImGui.GetItemRectSize().X - vector.X) / 2f, cursorScreenPos.Y + x);
		((ImDrawListPtr)(ref windowDrawList)).AddText(vector2, ImGui.GetColorU32((ImGuiCol)0), ImU8String.op_Implicit(text));
		ImGui.PopID();
		return result;
	}

	public static bool SelectableTextCentered(string text, ref bool selected, ImGuiSelectableFlags flags = (ImGuiSelectableFlags)0)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(1, 2);
		((ImU8String)(ref val)).AppendFormatted<string>(text);
		((ImU8String)(ref val)).AppendLiteral("_");
		((ImU8String)(ref val)).AppendFormatted<ImGuiSelectableFlags>(flags);
		IdDisposable val2 = ImRaii.PushId(val, true);
		try
		{
			Vector2 vector = ImGui.CalcTextSize(ImU8String.op_Implicit(text), false, -1f);
			ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
			Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
			ImGuiStylePtr style = ImGui.GetStyle();
			float x = ((ImGuiStylePtr)(ref style)).FramePadding.X;
			float y = vector.Y + 2f * x;
			bool result = ImGui.Selectable(ImU8String.op_Implicit(string.Empty), ref selected, flags, new Vector2(0f, y));
			Vector2 vector2 = new Vector2(cursorScreenPos.X + (ImGui.GetItemRectSize().X - vector.X) / 2f, cursorScreenPos.Y + x);
			((ImDrawListPtr)(ref windowDrawList)).AddText(vector2, ImGui.GetColorU32((ImGuiCol)0), ImU8String.op_Implicit(text));
			return result;
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	public static bool SelectableIconCentered(string id, FontAwesomeIcon icon, bool selected = false, ImGuiSelectableFlags flags = (ImGuiSelectableFlags)0, bool useStaticFont = false)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(1, 2);
		((ImU8String)(ref val)).AppendFormatted<FontAwesomeIcon>(icon);
		((ImU8String)(ref val)).AppendLiteral("_");
		((ImU8String)(ref val)).AppendFormatted<string>(id);
		IdDisposable val2 = ImRaii.PushId(val, true);
		try
		{
			Vector2 vector = Vector2.Zero;
			FontDisposable val3 = ImRaii.PushFont(UiBuilder.IconFont, useStaticFont);
			try
			{
				vector = ImGui.CalcTextSize(ImU8String.op_Implicit(FontAwesomeExtensions.ToIconString(icon)), false, -1f);
			}
			finally
			{
				((IDisposable)val3)?.Dispose();
			}
			ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
			Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
			ImGuiStylePtr style = ImGui.GetStyle();
			float x = ((ImGuiStylePtr)(ref style)).FramePadding.X;
			float y = vector.Y + 2f * x;
			bool result = ImGui.Selectable(ImU8String.op_Implicit(string.Empty), selected, flags, new Vector2(0f, y));
			Vector2 vector2 = new Vector2(cursorScreenPos.X + (ImGui.GetItemRectSize().X - vector.X + x) / 2f, cursorScreenPos.Y + x);
			val3 = ImRaii.PushFont(UiBuilder.IconFont, useStaticFont);
			try
			{
				((ImDrawListPtr)(ref windowDrawList)).AddText(vector2, ImGui.GetColorU32((ImGuiCol)0), ImU8String.op_Implicit(FontAwesomeExtensions.ToIconString(icon)));
			}
			finally
			{
				((IDisposable)val3)?.Dispose();
			}
			return result;
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	public static bool SelectableIconCentered(string id, FontAwesomeIcon icon, ref bool selected, ImGuiSelectableFlags flags = (ImGuiSelectableFlags)0, bool useStaticFont = false)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(1, 2);
		((ImU8String)(ref val)).AppendFormatted<FontAwesomeIcon>(icon);
		((ImU8String)(ref val)).AppendLiteral("_");
		((ImU8String)(ref val)).AppendFormatted<string>(id);
		IdDisposable val2 = ImRaii.PushId(val, true);
		try
		{
			Vector2 vector = Vector2.Zero;
			FontDisposable val3 = ImRaii.PushFont(UiBuilder.IconFont, useStaticFont);
			try
			{
				vector = ImGui.CalcTextSize(ImU8String.op_Implicit(FontAwesomeExtensions.ToIconString(icon)), false, -1f);
			}
			finally
			{
				((IDisposable)val3)?.Dispose();
			}
			ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
			Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
			ImGuiStylePtr style = ImGui.GetStyle();
			float x = ((ImGuiStylePtr)(ref style)).FramePadding.X;
			float y = vector.Y + 2f * x;
			bool result = ImGui.Selectable(ImU8String.op_Implicit(""), ref selected, flags, new Vector2(0f, y));
			Vector2 vector2 = new Vector2(cursorScreenPos.X + (ImGui.GetItemRectSize().X - vector.X + x) / 2f, cursorScreenPos.Y + x);
			val3 = ImRaii.PushFont(UiBuilder.IconFont, useStaticFont);
			try
			{
				((ImDrawListPtr)(ref windowDrawList)).AddText(vector2, ImGui.GetColorU32((ImGuiCol)0), ImU8String.op_Implicit(FontAwesomeExtensions.ToIconString(icon)));
			}
			finally
			{
				((IDisposable)val3)?.Dispose();
			}
			return result;
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	public static void Text(string text)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		float cursorPosY = ImGui.GetCursorPosY();
		ImGuiStylePtr style = ImGui.GetStyle();
		ImGui.SetCursorPosY(cursorPosY + ((ImGuiStylePtr)(ref style)).FramePadding.X);
		ImGui.TextUnformatted(ImU8String.op_Implicit(text));
	}

	public static void TextImage(string text, nint imageHandle, Vector2 imageSize)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		TextImage(text, new ImTextureID((IntPtr)imageHandle), imageSize);
	}

	public static void TextImage(string text, ImTextureID imageHandle, Vector2 imageSize)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		float cursorPosY = ImGui.GetCursorPosY();
		ImGuiStylePtr style = ImGui.GetStyle();
		ImGui.SetCursorPosY(cursorPosY + ((ImGuiStylePtr)(ref style)).FramePadding.Y);
		ImGui.Image(imageHandle, imageSize);
		ImGui.SameLine();
		float cursorPosY2 = ImGui.GetCursorPosY();
		style = ImGui.GetStyle();
		ImGui.SetCursorPosY(cursorPosY2 - ((ImGuiStylePtr)(ref style)).FramePadding.Y);
		Text(text);
	}

	public static void TextCentered(string text)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		CenterAlignFor(ImGui.CalcTextSize(ImU8String.op_Implicit(text), false, -1f).X);
		Text(text);
	}

	public static void TextDisabledWrapped(string text)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		DisabledDisposable val = ImRaii.Disabled();
		try
		{
			ImGui.TextWrapped(ImU8String.op_Implicit(text));
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static void TextDisabledWrapped(string text, float warpPos)
	{
		DisabledDisposable val = ImRaii.Disabled();
		try
		{
			TextWrapDisposable val2 = ImRaii.TextWrapPos(ImGui.GetFontSize() * warpPos);
			try
			{
				Text(text);
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static bool TextIcon(FontAwesomeIcon icon, string text, bool useStaticFont = false)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		Vector2 vector = Vector2.Zero;
		FontDisposable val = ImRaii.PushFont(UiBuilder.IconFont, useStaticFont);
		try
		{
			vector = ImGui.CalcTextSize(ImU8String.op_Implicit(FontAwesomeExtensions.ToIconString(icon)), false, -1f);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
		Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
		ImGuiStylePtr style = ImGui.GetStyle();
		Vector2 framePadding = ((ImGuiStylePtr)(ref style)).FramePadding;
		Vector2 vector2 = ImGui.CalcTextSize(ImU8String.op_Implicit(text), false, -1f);
		float num = Math.Max(vector.Y, vector2.Y);
		bool result = false;
		DisabledDisposable val2 = ImRaii.Disabled();
		try
		{
			ColorDisposable val3 = ImRaii.PushColor((ImGuiCol)21, 0u, true);
			try
			{
				result = ImGui.Button(ImU8String.op_Implicit(string.Empty), new Vector2(vector.X + vector2.X + 3f * framePadding.X, num + 2f * framePadding.Y));
			}
			finally
			{
				((IDisposable)val3)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
		Vector2 vector3 = new Vector2(cursorScreenPos.X + framePadding.X, cursorScreenPos.Y + framePadding.Y);
		val = ImRaii.PushFont(UiBuilder.IconFont, useStaticFont);
		try
		{
			((ImDrawListPtr)(ref windowDrawList)).AddText(vector3, ImGui.GetColorU32((ImGuiCol)0), ImU8String.op_Implicit(FontAwesomeExtensions.ToIconString(icon)));
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		Vector2 vector4 = new Vector2(vector3.X + vector.X + 2f * framePadding.X, cursorScreenPos.Y + framePadding.Y);
		((ImDrawListPtr)(ref windowDrawList)).AddText(vector4, ImGui.GetColorU32((ImGuiCol)0), ImU8String.op_Implicit(text));
		return result;
	}

	public static void TextOutlined(uint textColor, string text, uint outlineColor = 4278190080u, float outlineThickness = 1.5f)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		Vector2 cursorPos = ImGui.GetCursorPos();
		GroupDisposable val = ImRaii.Group();
		try
		{
			ColorDisposable val2;
			for (float num = 0f - outlineThickness; num <= outlineThickness; num += 0.5f)
			{
				for (float num2 = 0f - outlineThickness; num2 <= outlineThickness; num2 += 0.5f)
				{
					if (num != 0f || num2 != 0f)
					{
						ImGui.SetCursorPos(cursorPos + new Vector2(num, num2));
						val2 = ImRaii.PushColor((ImGuiCol)0, outlineColor, true);
						try
						{
							ImGui.TextUnformatted(ImU8String.op_Implicit(text));
						}
						finally
						{
							((IDisposable)val2)?.Dispose();
						}
					}
				}
			}
			ImGui.SetCursorPos(cursorPos);
			val2 = ImRaii.PushColor((ImGuiCol)0, textColor, true);
			try
			{
				ImGui.TextUnformatted(ImU8String.op_Implicit(text));
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		finally
		{
			((GroupDisposable)(ref val)).Dispose();
		}
	}

	public static void TextOutlined(Vector4 textColor, string text, Vector4? outlineColor = null, float outlineThickness = 1.5f)
	{
		TextOutlined(textColor.ToUInt(), text, outlineColor.HasValue ? outlineColor.GetValueOrDefault().ToUInt() : 4278190080u, outlineThickness);
	}

	public static void TextOutlined(Vector2 position, uint textColor, string text, uint outlineColor = 4278190080u, float outlineThickness = 1.5f, ImDrawListPtr? drawList = null)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		ImDrawListPtr value = drawList.GetValueOrDefault();
		if (!drawList.HasValue)
		{
			value = ImGui.GetBackgroundDrawList();
			drawList = value;
		}
		for (float num = 0f - outlineThickness; num <= outlineThickness; num += 0.5f)
		{
			for (float num2 = 0f - outlineThickness; num2 <= outlineThickness; num2 += 0.5f)
			{
				if ((num != 0f || num2 != 0f) && drawList.HasValue)
				{
					value = drawList.GetValueOrDefault();
					((ImDrawListPtr)(ref value)).AddText(position + new Vector2(num, num2), outlineColor, ImU8String.op_Implicit(text));
				}
			}
		}
		if (drawList.HasValue)
		{
			value = drawList.GetValueOrDefault();
			((ImDrawListPtr)(ref value)).AddText(position, textColor, ImU8String.op_Implicit(text));
		}
	}

	public static bool TextLink(string url, string? displayText = null, bool showUnderline = true)
	{
		return false;
	}

	public static void TextSelectable(string text, float lineLength, List<TextSelectableLinkTypeInfo>? links = null, uint? colorBG = null, string? id = null)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		if (!string.IsNullOrWhiteSpace(text))
		{
			string text2 = WrapMixedText(text, lineLength).Trim();
			string text3 = "###" + (string.IsNullOrWhiteSpace(id) ? ("TextSelectable" + text.GetHashCode()) : id);
			string[] array = text2.Split('\n');
			float textLineHeightWithSpacing = ImGui.GetTextLineHeightWithSpacing();
			ImGuiStylePtr style = ImGui.GetStyle();
			float num = textLineHeightWithSpacing + ((ImGuiStylePtr)(ref style)).ItemSpacing.Y;
			style = ImGui.GetStyle();
			Vector2 vector = new Vector2(lineLength + 2f * ((ImGuiStylePtr)(ref style)).FramePadding.X, (float)array.Length * num);
			string text4 = text2;
			int num2 = text2.Length + 1;
			Vector2 cursorPos = ImGui.GetCursorPos();
			Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
			ColorDisposable val = ImRaii.PushColor((ImGuiCol)7, colorBG ?? ImGui.GetColorU32((ImGuiCol)2), true);
			try
			{
				style = ImGui.GetStyle();
				ImGui.SetCursorPos(cursorPos - ((ImGuiStylePtr)(ref style)).FramePadding);
				ImGui.InputTextMultiline(ImU8String.op_Implicit(text3), ref text4, num2, vector, (ImGuiInputTextFlags)20480, (ImGuiInputTextCallbackDelegate)null);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
			if (ImGui.IsItemActivated())
			{
				IMEHelper.Disable();
			}
			if (ImGui.IsItemDeactivated())
			{
				IMEHelper.Enable();
			}
			if (links != null && links.Count > 0)
			{
				ProcessLinks(links, text2, cursorScreenPos);
			}
		}
	}

	private static void ProcessLinks(List<TextSelectableLinkTypeInfo> links, string wrappedText, Vector2 screenStartPos)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
		foreach (TextSelectableLinkTypeInfo link in links)
		{
			foreach (Match item4 in link.Pattern.Matches(wrappedText))
			{
				foreach (var item5 in FindLinkPositionsInWrappedText(wrappedText, item4.Index, item4.Length))
				{
					int item = item5.lineIndex;
					float item2 = item5.startX;
					float item3 = item5.endX;
					Vector2 vector = new Vector2(screenStartPos.X + item2, screenStartPos.Y + (float)item * ImGui.GetTextLineHeightWithSpacing());
					Vector2 vector2 = new Vector2(screenStartPos.X + item3, vector.Y + ImGui.GetTextLineHeightWithSpacing());
					float num = item + 1;
					ImGuiStylePtr style = ImGui.GetStyle();
					float num2 = num * ((ImGuiStylePtr)(ref style)).ItemSpacing.Y;
					((ImDrawListPtr)(ref windowDrawList)).AddLine(new Vector2(vector.X, vector.Y + ImGui.GetTextLineHeightWithSpacing() - num2), new Vector2(vector2.X, vector2.Y - num2), link.UnderlineColor, 1f);
					if (ImGui.IsMouseHoveringRect(vector, vector2))
					{
						ImGui.SetMouseCursor((ImGuiMouseCursor)7);
						if (ImGui.IsMouseClicked((ImGuiMouseButton)0))
						{
							link.ClickCallback(item4);
						}
					}
				}
			}
		}
	}

	private static List<(int lineIndex, float startX, float endX)> FindLinkPositionsInWrappedText(string wrappedText, int linkStartIndex, int linkLength)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		List<(int, float, float)> list = new List<(int, float, float)>();
		string[] array = wrappedText.Split('\n');
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			int length = array[i].Length;
			int num2 = num + length;
			if (linkStartIndex < num2 && linkStartIndex + linkLength > num)
			{
				int length2 = Math.Max(0, linkStartIndex - num);
				int length3 = Math.Min(length, linkStartIndex + linkLength - num);
				float x = ImGui.CalcTextSize(ImU8String.op_Implicit(array[i].Substring(0, length2)), false, -1f).X;
				float x2 = ImGui.CalcTextSize(ImU8String.op_Implicit(array[i].Substring(0, length3)), false, -1f).X;
				list.Add((i, x, x2));
			}
			num = num2 + 1;
		}
		return list;
	}

	private static string WrapMixedText(string text, float maxWidth)
	{
		if (string.IsNullOrEmpty(text))
		{
			return string.Empty;
		}
		(string, float) key = (text, maxWidth);
		if (wrappedTextCache.TryGetValue(key, out string value))
		{
			return value;
		}
		StringBuilder stringBuilder = new StringBuilder(text.Length + 20);
		float num = 0f;
		char[] array = new char[28]
		{
			'，', '。', '、', '：', '；', '！', '？', '）', '》', '"',
			'\'', '」', '』', '】', '）', '…', ',', '.', ':', ';',
			'!', '?', ')', '>', '"', '\'', ']', '}'
		};
		int i = 0;
		ReadOnlySpan<char> readOnlySpan = text.AsSpan();
		while (i < readOnlySpan.Length)
		{
			if (IsLatinChar(readOnlySpan[i]))
			{
				int num2 = i;
				for (; i < readOnlySpan.Length && IsLatinChar(readOnlySpan[i]); i++)
				{
				}
				ReadOnlySpan<char> readOnlySpan2 = readOnlySpan.Slice(num2, i - num2);
				float num3 = GetWordWidth(readOnlySpan2);
				if (num3 > maxWidth && num > 0f)
				{
					stringBuilder.Append('\n');
					num = 0f;
				}
				if (num + num3 > maxWidth)
				{
					stringBuilder.Append('\n');
					num = 0f;
				}
				stringBuilder.Append(readOnlySpan2);
				num += num3;
				if (i < readOnlySpan.Length && readOnlySpan[i] == ' ')
				{
					float num4 = GetCharWidth(' ');
					if (num + num4 <= maxWidth)
					{
						stringBuilder.Append(' ');
						num += num4;
					}
					i++;
				}
				continue;
			}
			char c = readOnlySpan[i];
			bool num5 = Array.IndexOf(array, c) >= 0;
			float num6 = GetCharWidth(c);
			if (num5)
			{
				if (num + num6 > maxWidth * 0.95f && num > 0f)
				{
					stringBuilder.Append('\n');
					num = 0f;
				}
				stringBuilder.Append(c);
				num += num6;
				i++;
				continue;
			}
			if (i + 1 < readOnlySpan.Length && Array.IndexOf(array, readOnlySpan[i + 1]) >= 0)
			{
				float num7 = GetCharWidth(readOnlySpan[i + 1]);
				if (num + num6 + num7 > maxWidth)
				{
					stringBuilder.Append('\n');
					num = 0f;
				}
			}
			if (num + num6 > maxWidth)
			{
				stringBuilder.Append('\n');
				num = 0f;
			}
			stringBuilder.Append(c);
			num += num6;
			i++;
		}
		string text2 = stringBuilder.ToString();
		if (wrappedTextCache.Count > 100)
		{
			wrappedTextCache.Clear();
		}
		wrappedTextCache[key] = text2;
		return text2;
		static float GetCharWidth(char c2)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return ImGui.CalcTextSize(ImU8String.op_Implicit(c2.ToString()), false, -1f).X;
		}
		static float GetWordWidth(ReadOnlySpan<char> word)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			return ImGui.CalcTextSize(ImU8String.op_Implicit(word.ToString()), false, -1f).X;
		}
		static bool IsLatinChar(char c2)
		{
			if ((c2 < 'A' || c2 > 'Z') && (c2 < 'a' || c2 > 'z') && (c2 < '0' || c2 > '9') && c2 != '_' && c2 != '-' && c2 != '@')
			{
				return c2 == '&';
			}
			return true;
		}
	}

	public static void TooltipHover(string text, float warpPos = 20f)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		if (string.IsNullOrWhiteSpace(text) || !ImGui.IsItemHovered())
		{
			return;
		}
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(14, 2);
		((ImU8String)(ref val)).AppendLiteral("TooltipHover_");
		((ImU8String)(ref val)).AppendFormatted<string>(text);
		((ImU8String)(ref val)).AppendLiteral("_");
		((ImU8String)(ref val)).AppendFormatted<float>(warpPos);
		IdDisposable val2 = ImRaii.PushId(val, true);
		try
		{
			TooltipDisposable val3 = ImRaii.Tooltip();
			try
			{
				TextWrapDisposable val4 = ImRaii.TextWrapPos(ImGui.GetFontSize() * warpPos);
				try
				{
					ImGui.TextUnformatted(ImU8String.op_Implicit(text));
				}
				finally
				{
					((IDisposable)val4)?.Dispose();
				}
			}
			finally
			{
				((TooltipDisposable)(ref val3)).Dispose();
			}
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	public static bool TreeNodeImageWithText(nint image, Vector2 imageSize, string text, ImGuiTreeNodeFlags flags = (ImGuiTreeNodeFlags)0)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return TreeNodeImageWithText(new ImTextureID((IntPtr)image), imageSize, text, flags);
	}

	public static bool TreeNodeImageWithText(ImTextureID image, Vector2 imageSize, string text, ImGuiTreeNodeFlags flags = (ImGuiTreeNodeFlags)0)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		int count = (int)MathF.Ceiling(imageSize.X / ImGui.CalcTextSize(ImU8String.op_Implicit(" "), false, -1f).X);
		string text2 = new string(' ', count);
		Vector2 cursorPos = ImGui.GetCursorPos();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(1, 2);
		((ImU8String)(ref val)).AppendFormatted<string>(text2);
		((ImU8String)(ref val)).AppendLiteral(" ");
		((ImU8String)(ref val)).AppendFormatted<string>(text);
		bool result = ImGui.TreeNodeEx(val, (ImGuiTreeNodeFlags)(flags | 0x800), default(ImU8String));
		ImGui.SameLine();
		ImGui.SetCursorPosX(cursorPos.X + ImGui.GetTreeNodeToLabelSpacing());
		ImGui.Image(image, imageSize);
		return result;
	}

	public static bool CompLabelLeft(string label, Func<bool> origInputFunc)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		GroupDisposable val = ImRaii.Group();
		try
		{
			ImGui.AlignTextToFramePadding();
			ImGui.TextUnformatted(ImU8String.op_Implicit(label));
			ImGui.SameLine();
			return origInputFunc();
		}
		finally
		{
			((GroupDisposable)(ref val)).Dispose();
		}
	}

	public static bool CompLabelLeft(string label, float width, Func<bool> origInputFunc)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		GroupDisposable val = ImRaii.Group();
		try
		{
			ImGui.AlignTextToFramePadding();
			ImGui.TextUnformatted(ImU8String.op_Implicit(label));
			ImGui.SameLine();
			ImGui.SetNextItemWidth(width);
			return origInputFunc();
		}
		finally
		{
			((GroupDisposable)(ref val)).Dispose();
		}
	}

	public static void CompLabelLeft(string label, Action origInputFunc)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		GroupDisposable val = ImRaii.Group();
		try
		{
			ImGui.AlignTextToFramePadding();
			ImGui.TextUnformatted(ImU8String.op_Implicit(label));
			ImGui.SameLine();
			origInputFunc();
		}
		finally
		{
			((GroupDisposable)(ref val)).Dispose();
		}
	}

	public static void CompLabelLeft(string label, float width, Action origInputFunc)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		GroupDisposable val = ImRaii.Group();
		try
		{
			ImGui.AlignTextToFramePadding();
			ImGui.TextUnformatted(ImU8String.op_Implicit(label));
			ImGui.SameLine();
			ImGui.SetNextItemWidth(width);
			origInputFunc();
		}
		finally
		{
			((GroupDisposable)(ref val)).Dispose();
		}
	}

	public static void HelpMarker(string tooltip, float warpPos = 40f, FontAwesomeIcon icon = (FontAwesomeIcon)61530, bool useStaticFont = false)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		ImGui.SameLine();
		GroupDisposable val = ImRaii.Group();
		try
		{
			FontDisposable val2 = ImRaii.PushFont(UiBuilder.IconFont, useStaticFont);
			try
			{
				float cursorPosY = ImGui.GetCursorPosY();
				ImGuiStylePtr style = ImGui.GetStyle();
				ImGui.SetCursorPosY(cursorPosY + ((ImGuiStylePtr)(ref style)).FramePadding.Y * 0.5f);
				ImGui.TextDisabled(ImU8String.op_Implicit(FontAwesomeExtensions.ToIconString(icon)));
				ImGui.SetCursorPosY(cursorPosY);
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
			if (!ImGui.IsItemHovered())
			{
				return;
			}
			TooltipDisposable val3 = ImRaii.Tooltip();
			try
			{
				TextWrapDisposable val4 = ImRaii.TextWrapPos(ImGui.GetFontSize() * warpPos);
				try
				{
					ImGui.TextUnformatted(ImU8String.op_Implicit(tooltip));
				}
				finally
				{
					((IDisposable)val4)?.Dispose();
				}
			}
			finally
			{
				((TooltipDisposable)(ref val3)).Dispose();
			}
		}
		finally
		{
			((GroupDisposable)(ref val)).Dispose();
		}
	}
}
