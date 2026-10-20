using System.Numerics;
using Dalamud.Bindings.ImGui;

namespace OmenTools.ImGuiOm.Markdown.Rendering;

internal static class MarkdownColors
{
	public static Vector4 Link(MarkdownConfig config)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		Vector4? linkColor = config.LinkColor;
		if (!linkColor.HasValue)
		{
			ImGuiStylePtr style = ImGui.GetStyle();
			return ((ImGuiStylePtr)(ref style)).Colors[22];
		}
		return linkColor.GetValueOrDefault();
	}

	public static uint InlineCodeBackground()
	{
		return ImGui.GetColorU32((ImGuiCol)7);
	}

	public static uint BlockquoteBar()
	{
		return ImGui.GetColorU32((ImGuiCol)5);
	}

	public static uint Separator()
	{
		return ImGui.GetColorU32((ImGuiCol)27);
	}

	public static uint TextU32()
	{
		return ImGui.GetColorU32((ImGuiCol)0);
	}
}
