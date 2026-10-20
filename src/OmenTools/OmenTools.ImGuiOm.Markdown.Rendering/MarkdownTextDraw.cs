using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Dalamud.Bindings.ImGui;

namespace OmenTools.ImGuiOm.Markdown.Rendering;

internal static class MarkdownTextDraw
{
	private const float BOLD_OFFSET_RATIO = 0.03f;

	private const float OBLIQUE_SHEAR = 0.22f;

	public static float BoldOffset(float pixelSize)
	{
		return pixelSize * 0.03f;
	}

	public static float Overhang(MarkdownFontRole role, float pixelSize)
	{
		float num = 0f;
		if (IsItalic(role))
		{
			num += pixelSize * 0.22f;
		}
		if (IsBold(role))
		{
			num += MathF.Max(0.35f, BoldOffset(pixelSize));
		}
		return num;
	}

	public static bool IsBold(MarkdownFontRole role)
	{
		if (role == MarkdownFontRole.Bold || role == MarkdownFontRole.BoldItalic)
		{
			return true;
		}
		return false;
	}

	public static bool IsItalic(MarkdownFontRole role)
	{
		if ((uint)(role - 2) <= 1u)
		{
			return true;
		}
		return false;
	}

	public static MarkdownFontRole EmphasisRole(bool bold, bool italic)
	{
		if (bold)
		{
			if (italic)
			{
				return MarkdownFontRole.BoldItalic;
			}
			return MarkdownFontRole.Bold;
		}
		if (italic)
		{
			return MarkdownFontRole.Italic;
		}
		return MarkdownFontRole.Body;
	}

	public static void Draw(ImDrawListPtr drawList, ImFontPtr font, float pixelSize, Vector2 position, uint color, string text, bool bold, bool italic)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		int size = ((ImDrawListPtr)(ref drawList)).VtxBuffer.Size;
		((ImDrawListPtr)(ref drawList)).AddText(font, pixelSize, position, color, ImU8String.op_Implicit(text), 0f);
		if (bold)
		{
			float x = MathF.Max(0.35f, BoldOffset(pixelSize));
			((ImDrawListPtr)(ref drawList)).AddText(font, pixelSize, position + new Vector2(x, 0f), color, ImU8String.op_Implicit(text), 0f);
		}
		if (italic)
		{
			ShearVertices(drawList, size, position, pixelSize);
		}
	}

	private unsafe static void ShearVertices(ImDrawListPtr drawList, int vertexStart, Vector2 position, float pixelSize)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		float num = position.Y + pixelSize;
		ImVector<ImDrawVert> vtxBuffer = ((ImDrawListPtr)(ref drawList)).VtxBuffer;
		ImDrawVert* data = vtxBuffer.Data;
		for (int i = vertexStart; i < vtxBuffer.Size; i++)
		{
			((ImDrawVert)((byte*)data + (nint)i * (nint)Unsafe.SizeOf<ImDrawVert>())).Pos.X += (num - ((ImDrawVert)((byte*)data + (nint)i * (nint)Unsafe.SizeOf<ImDrawVert>())).Pos.Y) * 0.22f;
		}
	}
}
