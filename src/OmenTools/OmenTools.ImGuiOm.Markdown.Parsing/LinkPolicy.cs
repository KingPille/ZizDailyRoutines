using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace OmenTools.ImGuiOm.Markdown.Parsing;

internal static class LinkPolicy
{
	private static readonly string[] AutoOpenSchemes = new string[3] { "hxxp://", "hxxps://", "mailto:" };

	public static bool ShouldAutoOpen(string url)
	{
		if (string.IsNullOrEmpty(url))
		{
			return false;
		}
		string[] autoOpenSchemes = AutoOpenSchemes;
		foreach (string value in autoOpenSchemes)
		{
			if (url.StartsWith(value, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}
		return false;
	}

	public static void Activate(string url, Action<string>? onClicked)
	{
		if (string.IsNullOrEmpty(url))
		{
			return;
		}
		if (onClicked != null)
		{
			onClicked(url);
		}
		else
		{
			if (!ShouldAutoOpen(url))
			{
				return;
			}
			try
			{
				new ProcessStartInfo(url).UseShellExecute = true;
				using (null)
				{
				}
			}
			catch (Exception ex) when (((ex is Win32Exception || ex is InvalidOperationException || ex is FileNotFoundException || ex is NotSupportedException) ? 1 : 0) != 0)
			{
			}
		}
	}
}
