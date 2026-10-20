using OmenTools.ImGuiOm.Markdown.Parsing;

namespace OmenTools.ImGuiOm.Markdown.Rendering;

internal static class BlockSpacing
{
	private const float PARAGRAPH_GAP = 0.4f;

	private const float SECTION_GAP = 1.4f;

	private const float HEADING_GAP = 0.3f;

	private const float SUBHEADING_GAP = 1f;

	private const float SUBHEADING_BIND_GAP = 0.25f;

	private const float LIST_INTRO_GAP = 0.3f;

	private const float LIST_OUTRO_GAP = 0.45f;

	private const float LIST_ITEM_GAP = 0.15f;

	private const float OBJECT_GAP = 0.5f;

	private const float WIDE_OBJECT_GAP = 0.55f;

	private const float RULE_GAP = 1f;

	public static float Gap(MarkdownBlockKind previous, MarkdownBlockKind next)
	{
		if (previous == MarkdownBlockKind.ThematicBreak || next == MarkdownBlockKind.ThematicBreak)
		{
			return 1f;
		}
		if (IsHeading(next))
		{
			if ((uint)(next - 1) > 1u)
			{
				return 1f;
			}
			return 1.4f;
		}
		if (next == MarkdownBlockKind.ListItem || previous == MarkdownBlockKind.ListItem)
		{
			return 0.15f;
		}
		if (IsWideObject(next) || IsWideObject(previous))
		{
			return 0.55f;
		}
		if (IsObject(next) || IsObject(previous))
		{
			return 0.5f;
		}
		if (next == MarkdownBlockKind.List)
		{
			if (!IsHeading(previous))
			{
				return 0.3f;
			}
			return ListAfterHeadingGap(previous);
		}
		if (previous == MarkdownBlockKind.List)
		{
			return 0.45f;
		}
		if (IsHeading(previous))
		{
			if ((uint)(previous - 1) > 1u)
			{
				return 0.25f;
			}
			return 0.3f;
		}
		return 0.4f;
	}

	private static float ListAfterHeadingGap(MarkdownBlockKind heading)
	{
		if ((uint)(heading - 1) > 1u)
		{
			return 0.25f;
		}
		return 0.3f;
	}

	private static bool IsObject(MarkdownBlockKind kind)
	{
		if ((uint)(kind - 9) <= 3u)
		{
			return true;
		}
		return false;
	}

	private static bool IsWideObject(MarkdownBlockKind kind)
	{
		if ((uint)(kind - 11) <= 1u)
		{
			return true;
		}
		return false;
	}

	private static bool IsHeading(MarkdownBlockKind kind)
	{
		if ((uint)(kind - 1) <= 5u)
		{
			return true;
		}
		return false;
	}
}
