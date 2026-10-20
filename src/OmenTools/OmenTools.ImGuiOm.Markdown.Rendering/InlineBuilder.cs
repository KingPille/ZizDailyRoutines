using System.Collections.Generic;
using Markdig.Extensions.TaskLists;
using Markdig.Syntax.Inlines;
using OmenTools.ImGuiOm.Markdown.Layout;

namespace OmenTools.ImGuiOm.Markdown.Rendering;

internal static class InlineBuilder
{
	public static IReadOnlyList<InlineRun> Build(ContainerInline? container)
	{
		List<InlineRun> list = new List<InlineRun>();
		if (container != null)
		{
			Walk(container, list, bold: false, italic: false, null);
		}
		return list;
	}

	private static void Walk(ContainerInline container, List<InlineRun> runs, bool bold, bool italic, string? linkURL)
	{
		for (Inline inline = container.FirstChild; inline != null; inline = inline.NextSibling)
		{
			Append(inline, runs, bold, italic, linkURL);
		}
	}

	private static void Append(Inline inline, List<InlineRun> runs, bool bold, bool italic, string? linkURL)
	{
		if (!(inline is LiteralInline literalInline))
		{
			if (!(inline is CodeInline codeInline))
			{
				if (!(inline is EmphasisInline emphasisInline))
				{
					if (inline is LinkInline linkInline)
					{
						if (linkInline.IsImage)
						{
							runs.Add(new InlineRun(linkInline.Url ?? string.Empty, MarkdownFontRole.Body, linkURL, IsImage: true));
							return;
						}
						LinkInline linkInline2 = linkInline;
						Walk(linkInline2, runs, bold, italic, linkInline2.Url ?? linkURL);
					}
					else if (!(inline is AutolinkInline autolinkInline))
					{
						if (!(inline is LineBreakInline))
						{
							if (!(inline is HtmlInline htmlInline))
							{
								if (!(inline is TaskList))
								{
									if (inline is ContainerInline container)
									{
										Walk(container, runs, bold, italic, linkURL);
									}
									else
									{
										runs.Add(new InlineRun(inline.ToString() ?? string.Empty, MarkdownTextDraw.EmphasisRole(bold, italic), linkURL, IsImage: false));
									}
								}
							}
							else
							{
								runs.Add(new InlineRun(htmlInline.Tag, MarkdownTextDraw.EmphasisRole(bold, italic), linkURL, IsImage: false));
							}
						}
						else
						{
							runs.Add(new InlineRun(" ", MarkdownFontRole.Body, linkURL, IsImage: false));
						}
					}
					else
					{
						runs.Add(new InlineRun(autolinkInline.Url, MarkdownTextDraw.EmphasisRole(bold, italic), autolinkInline.Url, IsImage: false));
					}
				}
				else
				{
					bool bold2 = bold || emphasisInline.DelimiterCount >= 2;
					bool italic2 = italic || emphasisInline.DelimiterCount == 1;
					Walk(emphasisInline, runs, bold2, italic2, linkURL);
				}
			}
			else
			{
				runs.Add(new InlineRun(codeInline.Content, MarkdownFontRole.Code, linkURL, IsImage: false));
			}
		}
		else
		{
			runs.Add(new InlineRun(literalInline.Content.ToString(), MarkdownTextDraw.EmphasisRole(bold, italic), linkURL, IsImage: false));
		}
	}
}
