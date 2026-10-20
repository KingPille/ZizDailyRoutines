using System.Collections.Generic;
using System.Threading;
using Markdig;
using Markdig.Syntax;

namespace OmenTools.ImGuiOm.Markdown.Parsing;

internal static class MarkdownParser
{
	private static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder().UsePipeTables().UseTaskLists().UseAutoLinks()
		.Build();

	private static readonly Dictionary<string, Markdig.Syntax.MarkdownDocument> Cache = new Dictionary<string, Markdig.Syntax.MarkdownDocument>();

	private static readonly Queue<string> InsertionOrder = new Queue<string>();

	private static readonly Lock Gate = new Lock();

	private const int MAX_CACHE_ENTRIES = 32;

	public static Markdig.Syntax.MarkdownDocument Parse(string markdown)
	{
		return Markdig.Markdown.Parse(markdown ?? string.Empty, Pipeline);
	}

	public static Markdig.Syntax.MarkdownDocument GetOrParse(string markdown)
	{
		string text = markdown ?? string.Empty;
		using (Gate.EnterScope())
		{
			if (Cache.TryGetValue(text, out Markdig.Syntax.MarkdownDocument value))
			{
				return value;
			}
			Markdig.Syntax.MarkdownDocument markdownDocument = Parse(text);
			Cache[text] = markdownDocument;
			InsertionOrder.Enqueue(text);
			while (InsertionOrder.Count > 32)
			{
				string key = InsertionOrder.Dequeue();
				Cache.Remove(key);
			}
			return markdownDocument;
		}
	}
}
