using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using Markdig.Syntax;
using OmenTools.ImGuiOm.Markdown.Parsing;
using OmenTools.ImGuiOm.Markdown.Rendering;

namespace OmenTools.ImGuiOm.Markdown;

public static class ImGuiMarkdown
{
	private static readonly MarkdownConfig DefaultConfig = new MarkdownConfig();

	public static void Render(string markdown, MarkdownConfig? config = null)
	{
		if (!string.IsNullOrEmpty(markdown))
		{
			RenderBlocks(MarkdownParser.GetOrParse(markdown), config ?? DefaultConfig);
		}
	}

	public static void Render(MarkdownDocument document, MarkdownConfig? config = null)
	{
		RenderBlocks(document.Ast, config ?? DefaultConfig);
	}

	private static void RenderBlocks(Markdig.Syntax.MarkdownDocument ast, MarkdownConfig config)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		ImGuiStylePtr style = ImGui.GetStyle();
		StyleDisposable val = ImRaii.PushStyle((ImGuiStyleVar)13, new Vector2(((ImGuiStylePtr)(ref style)).ItemSpacing.X, 0f), true);
		try
		{
			BlockRenderer.Render(ast, config);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}
}
