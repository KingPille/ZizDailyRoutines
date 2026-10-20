using System;
using System.Text.RegularExpressions;

namespace OmenTools.ImGuiOm;

public class TextSelectableLinkTypeInfo
{
	public Regex Pattern { get; }

	public Action<Match> ClickCallback { get; }

	public uint UnderlineColor { get; }

	public TextSelectableLinkTypeInfo(string pattern, Action<Match> clickCallback, uint underlineColor)
	{
		Pattern = new Regex(pattern, RegexOptions.Compiled);
		ClickCallback = clickCallback;
		UnderlineColor = underlineColor;
	}
}
