using System;
using System.Collections.Generic;
using System.Numerics;

namespace OmenTools.ImGuiOm.Markdown.Layout;

internal static class InlineLayout
{
	public static IReadOnlyList<LaidOutLine> Wrap(IReadOnlyList<InlineRun> runs, float maxWidth, Func<string, MarkdownFontRole, bool, Vector2> measure, Func<MarkdownFontRole, float> runOverhang)
	{
		float x = measure(" ", MarkdownFontRole.Body, arg3: false).X;
		List<LaidOutLine> list = new List<LaidOutLine>();
		List<LaidOutToken> list2 = new List<LaidOutToken>();
		float cursorX = 0f;
		float lineHeight = 0f;
		bool wantsLeadingSpace = false;
		foreach (InlineRun run in runs)
		{
			if (run.IsImage)
			{
				Place(run.Text, run, measure, maxWidth, x, list, list2, ref cursorX, ref lineHeight, wantsLeadingSpace);
				wantsLeadingSpace = false;
				continue;
			}
			foreach (var item in SplitTokens(run.Text))
			{
				var (text, _) = item;
				if (item.IsSpace)
				{
					wantsLeadingSpace = true;
					continue;
				}
				Place(text, run, measure, maxWidth, x, list, list2, ref cursorX, ref lineHeight, wantsLeadingSpace);
				wantsLeadingSpace = false;
			}
			if (list2.Count > 0)
			{
				cursorX += runOverhang(run.Role);
			}
		}
		FlushLine(list, list2, ref cursorX, ref lineHeight);
		return list;
	}

	private static void Place(string text, InlineRun run, Func<string, MarkdownFontRole, bool, Vector2> measure, float maxWidth, float spaceWidth, List<LaidOutLine> lines, List<LaidOutToken> current, ref float cursorX, ref float lineHeight, bool wantsLeadingSpace)
	{
		Vector2 vector = measure(text, run.Role, run.IsImage);
		float x = vector.X;
		float num = ((wantsLeadingSpace && current.Count > 0) ? spaceWidth : 0f);
		if (current.Count > 0 && cursorX + num + x > maxWidth)
		{
			FlushLine(lines, current, ref cursorX, ref lineHeight);
			num = 0f;
		}
		float num2 = cursorX + num;
		current.Add(new LaidOutToken(text, run.Role, run.LinkURL, run.IsImage, num2, x));
		cursorX = num2 + x;
		lineHeight = Math.Max(lineHeight, vector.Y);
	}

	private static void FlushLine(List<LaidOutLine> lines, List<LaidOutToken> current, ref float cursorX, ref float lineHeight)
	{
		if (current.Count > 0)
		{
			LaidOutToken[] tokens = current.ToArray();
			lines.Add(new LaidOutLine
			{
				Tokens = tokens,
				Width = cursorX,
				Height = lineHeight
			});
		}
		current.Clear();
		cursorX = 0f;
		lineHeight = 0f;
	}

	private static IEnumerable<(string Text, bool IsSpace)> SplitTokens(string text)
	{
		int num = 0;
		for (int i = 0; i < text.Length; i++)
		{
			char c = text[i];
			if (char.IsWhiteSpace(c))
			{
				if (i > num)
				{
					int num2 = num;
					yield return (Text: text.Substring(num2, i - num2), IsSpace: false);
				}
				yield return (Text: string.Empty, IsSpace: true);
				num = i + 1;
			}
			else if (IsCJK(c))
			{
				if (i > num)
				{
					int num2 = num;
					yield return (Text: text.Substring(num2, i - num2), IsSpace: false);
				}
				yield return (Text: c.ToString(), IsSpace: false);
				num = i + 1;
			}
		}
		if (num < text.Length)
		{
			yield return (Text: text.Substring(num), IsSpace: false);
		}
	}

	private static bool IsCJK(char c)
	{
		if ((c < 'ᄀ' || c > 'ᇿ') && (c < '⺀' || c > '〿') && (c < '\u3040' || c > 'ヿ') && (c < '\u3130' || c > '\u318f') && (c < '㐀' || c > '䶿') && (c < '一' || c > '鿿') && (c < '가' || c > '\ud7af') && (c < '豈' || c > '\ufaff'))
		{
			if (c >= '\uff00')
			{
				return c <= '\uffef';
			}
			return false;
		}
		return true;
	}
}
