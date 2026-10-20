using System;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Markdig.Syntax.Inlines;
using OmenTools.ImGuiOm.Markdown.Layout;
using OmenTools.ImGuiOm.Markdown.Parsing;

namespace OmenTools.ImGuiOm.Markdown.Rendering;

internal static class InlineRenderer
{
	private const float IMAGE_PLACEHOLDER_WIDTH = 120f;

	public static void Render(ContainerInline? inline, MarkdownConfig config, float headingScale = 1f)
	{
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		IReadOnlyList<InlineRun> readOnlyList = InlineBuilder.Build(inline);
		if (readOnlyList.Count == 0)
		{
			return;
		}
		float num = config.WrapWidth ?? ImGui.GetContentRegionAvail().X;
		if (num <= 0f)
		{
			num = 1f;
		}
		float num2 = ImGui.GetTextLineHeightWithSpacing() - ImGui.GetTextLineHeight();
		IReadOnlyList<LaidOutLine> readOnlyList2 = InlineLayout.Wrap(readOnlyList, num, (string text, MarkdownFontRole role, bool isImage) => Measure(text, role, config, isImage, headingScale), (MarkdownFontRole role) => Overhang(role, headingScale, config));
		Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
		ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
		uint colorU = ImGui.GetColorU32(MarkdownColors.Link(config));
		uint num3 = MarkdownColors.TextU32();
		float num4 = 0f;
		int linkIndex = 0;
		foreach (LaidOutLine item in readOnlyList2)
		{
			foreach (LaidOutToken token in item.Tokens)
			{
				DrawToken(pos: new Vector2(cursorScreenPos.X + token.X, cursorScreenPos.Y + num4), token: token, config: config, drawList: windowDrawList, color: (token.LinkURL == null) ? num3 : colorU, linkIndex: ref linkIndex, headingScale: headingScale);
			}
			num4 += item.Height + num2;
		}
		ImGui.SetCursorScreenPos(cursorScreenPos);
		ImGui.Dummy(new Vector2(num, num4));
	}

	private static float Overhang(MarkdownFontRole role, float headingScale, MarkdownConfig config)
	{
		using ScopedMarkdownFont scopedMarkdownFont = new ScopedMarkdownFont(role, headingScale, config);
		return MarkdownTextDraw.Overhang(role, scopedMarkdownFont.PixelSize);
	}

	private static Vector2 Measure(string text, MarkdownFontRole role, MarkdownConfig config, bool isImage, float headingScale)
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		if (isImage)
		{
			MarkdownImageResult? markdownImageResult = config.ResolveImage(text);
			float x = markdownImageResult?.Size.X ?? 120f;
			float y = markdownImageResult?.Size.Y ?? ImGui.GetFontSize();
			return new Vector2(x, y);
		}
		using ScopedMarkdownFont scopedMarkdownFont = new ScopedMarkdownFont(role, headingScale, config);
		int num = default(int);
		return new Vector2(ImGui.CalcTextSizeA(scopedMarkdownFont.Font, scopedMarkdownFont.PixelSize, float.MaxValue, 0f, ImU8String.op_Implicit(text), ref num).X, scopedMarkdownFont.PixelSize);
	}

	private static void DrawToken(LaidOutToken token, Vector2 pos, MarkdownConfig config, ImDrawListPtr drawList, uint color, ref int linkIndex, float headingScale)
	{
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		MarkdownFontRole role = token.Role;
		if (token.IsImage)
		{
			MarkdownImageResult? markdownImageResult = config.ResolveImage(token.Text);
			if (markdownImageResult.HasValue)
			{
				ImGui.SetCursorScreenPos(pos);
				ImGui.Image(markdownImageResult.Value.TextureID, markdownImageResult.Value.Size);
			}
			else
			{
				float fontSize = ImGui.GetFontSize();
				((ImDrawListPtr)(ref drawList)).AddRect(pos, pos + new Vector2(token.Width, fontSize), MarkdownColors.Separator());
				((ImDrawListPtr)(ref drawList)).AddText(pos + new Vector2(2f, 0f), color, ImU8String.op_Implicit(token.Text));
			}
			return;
		}
		using ScopedMarkdownFont scopedMarkdownFont = new ScopedMarkdownFont(role, headingScale, config);
		float pixelSize = scopedMarkdownFont.PixelSize;
		if (role == MarkdownFontRole.Code)
		{
			Vector2 vector = new Vector2(2f, 1f);
			((ImDrawListPtr)(ref drawList)).AddRectFilled(pos - vector, pos + new Vector2(token.Width, pixelSize) + vector, MarkdownColors.InlineCodeBackground(), 2f);
		}
		MarkdownTextDraw.Draw(drawList, scopedMarkdownFont.Font, pixelSize, pos, color, token.Text, scopedMarkdownFont.FauxBold, scopedMarkdownFont.FauxItalic);
		if (token.LinkURL != null)
		{
			float y = pos.Y + pixelSize;
			Vector2 vector2 = pos;
			vector2.Y = y;
			Vector2 vector3 = vector2;
			vector2 = pos;
			vector2.X = pos.X + token.Width;
			vector2.Y = y;
			((ImDrawListPtr)(ref drawList)).AddLine(vector3, vector2, color, 1f);
			ImGui.SetCursorScreenPos(pos);
			ImGui.PushID((IntPtr)(linkIndex++));
			ImGui.InvisibleButton(ImU8String.op_Implicit("##mdlink"), new Vector2(token.Width, pixelSize), (ImGuiButtonFlags)0);
			if (ImGui.IsItemHovered())
			{
				ImGui.SetMouseCursor((ImGuiMouseCursor)7);
			}
			if (ImGui.IsItemClicked())
			{
				LinkPolicy.Activate(config.ResolveURL(token.LinkURL), config.OnLinkClicked);
			}
			ImGui.PopID();
		}
	}
}
