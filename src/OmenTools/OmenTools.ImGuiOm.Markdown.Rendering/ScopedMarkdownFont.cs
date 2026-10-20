using System;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.ManagedFontAtlas;

namespace OmenTools.ImGuiOm.Markdown.Rendering;

internal sealed class ScopedMarkdownFont : IDisposable
{
	private readonly IDisposable fontPush;

	public bool FauxBold { get; }

	public bool FauxItalic { get; }

	public ImFontPtr Font { get; }

	public float PixelSize { get; }

	public ScopedMarkdownFont(MarkdownFontRole role, float headingScale, MarkdownConfig config)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		IFontHandle val = config.ResolveFont(role, headingScale);
		fontPush = val.Push();
		Font = ImGui.GetFont();
		PixelSize = ImGui.GetFontSize();
		FauxBold = MarkdownTextDraw.IsBold(role);
		FauxItalic = MarkdownTextDraw.IsItalic(role);
	}

	public void Dispose()
	{
		fontPush.Dispose();
	}
}
