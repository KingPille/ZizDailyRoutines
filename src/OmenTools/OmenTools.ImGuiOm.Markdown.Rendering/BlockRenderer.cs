using System;
using System.Numerics;
using System.Text;
using Dalamud.Bindings.ImGui;
using Markdig.Extensions.Tables;
using Markdig.Extensions.TaskLists;
using Markdig.Helpers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using OmenTools.ImGuiOm.Markdown.Parsing;

namespace OmenTools.ImGuiOm.Markdown.Rendering;

internal static class BlockRenderer
{
	public static void Render(ContainerBlock blocks, MarkdownConfig config)
	{
		MarkdownBlockKind? markdownBlockKind = null;
		foreach (Block block in blocks)
		{
			MarkdownBlockKind markdownBlockKind2 = KindOf(block);
			if (markdownBlockKind.HasValue)
			{
				ImGui.Dummy(new Vector2(0f, MarkdownConfig.ResolveGap(markdownBlockKind.Value, markdownBlockKind2)));
			}
			RenderBlock(block, config);
			markdownBlockKind = markdownBlockKind2;
		}
	}

	private static MarkdownBlockKind KindOf(Block block)
	{
		if (!(block is HeadingBlock { Level: var level }))
		{
			if (!(block is ParagraphBlock))
			{
				if (!(block is ListBlock))
				{
					if (!(block is ListItemBlock))
					{
						if (!(block is QuoteBlock))
						{
							if (!(block is CodeBlock))
							{
								if (!(block is ThematicBreakBlock))
								{
									if (block is Table)
									{
										return MarkdownBlockKind.Table;
									}
									return MarkdownBlockKind.Paragraph;
								}
								return MarkdownBlockKind.ThematicBreak;
							}
							return MarkdownBlockKind.Code;
						}
						return MarkdownBlockKind.Quote;
					}
					return MarkdownBlockKind.ListItem;
				}
				return MarkdownBlockKind.List;
			}
			return MarkdownBlockKind.Paragraph;
		}
		return (level <= 1) ? MarkdownBlockKind.Heading1 : (level switch
		{
			2 => MarkdownBlockKind.Heading2, 
			3 => MarkdownBlockKind.Heading3, 
			4 => MarkdownBlockKind.Heading4, 
			5 => MarkdownBlockKind.Heading5, 
			_ => MarkdownBlockKind.Heading6, 
		});
	}

	private static void RenderBlock(Block block, MarkdownConfig config)
	{
		if (!(block is HeadingBlock heading))
		{
			if (!(block is ParagraphBlock paragraphBlock))
			{
				if (!(block is ListBlock list))
				{
					if (!(block is QuoteBlock quote))
					{
						if (!(block is CodeBlock code))
						{
							if (!(block is ThematicBreakBlock))
							{
								if (!(block is Table table))
								{
									if (!(block is HtmlBlock html))
									{
										if (block is ContainerBlock blocks)
										{
											Render(blocks, config);
										}
									}
									else
									{
										RenderHTMLAsText(html, config);
									}
								}
								else
								{
									RenderTable(table, config);
								}
							}
							else
							{
								RenderThematicBreak();
							}
						}
						else
						{
							RenderCodeBlock(code, config);
						}
					}
					else
					{
						RenderQuote(quote, config);
					}
				}
				else
				{
					RenderList(list, config);
				}
			}
			else
			{
				InlineRenderer.Render(paragraphBlock.Inline, config);
			}
		}
		else
		{
			RenderHeading(heading, config);
		}
	}

	private static void RenderHeading(HeadingBlock heading, MarkdownConfig config)
	{
		float headingScale = config.ResolveHeadingScale(heading.Level);
		InlineRenderer.Render(heading.Inline, config, headingScale);
	}

	private static void RenderList(ListBlock list, MarkdownConfig config)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		int startNumber = 1;
		if (list.IsOrdered && int.TryParse(list.OrderedStart, out var result))
		{
			startNumber = result;
		}
		float num = config.ListIndentPixels;
		int num2 = 0;
		foreach (Block item in list)
		{
			if (item is ListItemBlock listItem)
			{
				string text = ListMarker.For(list.IsOrdered, num2, startNumber, TryGetTaskState(listItem));
				float x = num;
				float x2 = ImGui.CalcTextSize(ImU8String.op_Implicit(text), false, -1f).X;
				ImGuiStylePtr style = ImGui.GetStyle();
				num = MathF.Max(x, x2 + ((ImGuiStylePtr)(ref style)).ItemSpacing.X);
				num2++;
			}
		}
		int num3 = 0;
		foreach (Block item2 in list)
		{
			if (item2 is ListItemBlock listItemBlock)
			{
				if (num3 > 0)
				{
					ImGui.Dummy(new Vector2(0f, MarkdownConfig.ResolveGap(MarkdownBlockKind.ListItem, MarkdownBlockKind.ListItem)));
				}
				bool? taskChecked = TryGetTaskState(listItemBlock);
				ImGui.TextUnformatted(ImU8String.op_Implicit(ListMarker.For(list.IsOrdered, num3, startNumber, taskChecked)));
				ImGui.SameLine();
				ImGui.Indent(num);
				Render(listItemBlock, config);
				ImGui.Unindent(num);
				num3++;
			}
		}
	}

	private static bool? TryGetTaskState(ListItemBlock listItem)
	{
		foreach (Block item in listItem)
		{
			if (!(item is ParagraphBlock { Inline: not null } paragraphBlock))
			{
				continue;
			}
			for (Inline inline = paragraphBlock.Inline.FirstChild; inline != null; inline = inline.NextSibling)
			{
				if (inline is TaskList taskList)
				{
					return taskList.Checked;
				}
			}
		}
		return null;
	}

	private static void RenderQuote(QuoteBlock quote, MarkdownConfig config)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
		ImGui.Indent(config.ListIndentPixels);
		Render(quote, config);
		ImGui.Unindent(config.ListIndentPixels);
		Vector2 cursorScreenPos2 = ImGui.GetCursorScreenPos();
		ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
		float x = cursorScreenPos.X + config.ListIndentPixels * 0.35f;
		Vector2 vector = cursorScreenPos;
		vector.X = x;
		Vector2 vector2 = vector;
		vector = cursorScreenPos2;
		vector.X = x;
		((ImDrawListPtr)(ref windowDrawList)).AddLine(vector2, vector, MarkdownColors.BlockquoteBar(), 2f);
	}

	private static void RenderCodeBlock(CodeBlock code, MarkdownConfig config)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		string text = ExtractCodeText(code);
		if (config.CodeBlockRenderer != null)
		{
			config.CodeBlockRenderer((code as FencedCodeBlock)?.Info, text);
			return;
		}
		Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
		float x = ImGui.GetContentRegionAvail().X;
		using (new ScopedMarkdownFont(MarkdownFontRole.Code, 1f, config))
		{
			float y = ImGui.CalcTextSize(ImU8String.op_Implicit(text), false, -1f).Y + 8f;
			ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
			((ImDrawListPtr)(ref windowDrawList)).AddRectFilled(cursorScreenPos, cursorScreenPos + new Vector2(x, y), MarkdownColors.InlineCodeBackground(), 3f);
			ImGui.SetCursorScreenPos(new Vector2(cursorScreenPos.X + 4f, cursorScreenPos.Y + 4f));
			ImGui.TextUnformatted(ImU8String.op_Implicit(text));
			ImGui.SetCursorScreenPos(cursorScreenPos);
			ImGui.Dummy(new Vector2(x, y));
		}
	}

	private static string ExtractCodeText(LeafBlock block)
	{
		StringBuilder stringBuilder = new StringBuilder();
		StringLineGroup lines = block.Lines;
		for (int i = 0; i < lines.Count; i++)
		{
			stringBuilder.AppendLine(lines.Lines[i].ToString());
		}
		return stringBuilder.ToString().TrimEnd(new char[2] { '\n', '\r' });
	}

	private static void RenderThematicBreak()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
		float x = ImGui.GetContentRegionAvail().X;
		float y = cursorScreenPos.Y + 4f;
		ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
		Vector2 vector = cursorScreenPos;
		vector.Y = y;
		Vector2 vector2 = vector;
		vector = cursorScreenPos;
		vector.X = cursorScreenPos.X + x;
		vector.Y = y;
		((ImDrawListPtr)(ref windowDrawList)).AddLine(vector2, vector, MarkdownColors.Separator(), 1f);
		ImGui.Dummy(new Vector2(x, 9f));
	}

	private static void RenderTable(Table table, MarkdownConfig config)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		int num = table.ColumnDefinitions.Count;
		if (num == 0)
		{
			foreach (Block item in table)
			{
				if (item is TableRow tableRow)
				{
					num = Math.Max(num, tableRow.Count);
				}
			}
		}
		if (num == 0)
		{
			return;
		}
		ImGui.PushID((IntPtr)table.Span.Start);
		try
		{
			if (!ImGui.BeginTable(ImU8String.op_Implicit("##mdtable"), num, (ImGuiTableFlags)26560, default(Vector2), 0f))
			{
				return;
			}
			foreach (Block item2 in table)
			{
				if (!(item2 is TableRow tableRow2))
				{
					continue;
				}
				ImGui.TableNextRow();
				using (tableRow2.IsHeader ? new ScopedMarkdownFont(MarkdownFontRole.Bold, 1f, config) : null)
				{
					foreach (Block item3 in tableRow2)
					{
						if (!(item3 is TableCell tableCell))
						{
							continue;
						}
						ImGui.TableNextColumn();
						foreach (Block item4 in tableCell)
						{
							if (item4 is ParagraphBlock paragraphBlock)
							{
								InlineRenderer.Render(paragraphBlock.Inline, config);
							}
						}
					}
				}
			}
			ImGui.EndTable();
		}
		finally
		{
			ImGui.PopID();
		}
	}

	private static void RenderHTMLAsText(HtmlBlock html, MarkdownConfig config)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		string text = ExtractCodeText(html);
		using (new ScopedMarkdownFont(MarkdownFontRole.Code, 1f, config))
		{
			ImGui.TextUnformatted(ImU8String.op_Implicit(text));
		}
	}
}
