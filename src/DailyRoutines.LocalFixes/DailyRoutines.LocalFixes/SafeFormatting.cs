using System;

namespace DailyRoutines.LocalFixes;

public static class SafeFormatting
{
	public static string Latency(string? format, object? value, string? localizedDefault)
	{
		if (TryOne(format, value, out string result))
		{
			return result;
		}
		if (TryOne(localizedDefault, value, out result))
		{
			return result;
		}
		return $"{value} ms";
	}

	public static string Speech(string? format, object? name, object? text)
	{
		return Two(format, name, text, "{0}: {1}");
	}

	public static string TargetHP(string? format, object? maximum, object? current)
	{
		return Two(format, maximum, current, "{0} / {1}");
	}

	private static bool TryOne(string? format, object? value, out string result)
	{
		result = string.Empty;
		if (format == null)
		{
			return false;
		}
		try
		{
			result = string.Format(format, value);
			return true;
		}
		catch (FormatException)
		{
			return false;
		}
	}

	private static string Two(string? format, object? first, object? second, string fallback)
	{
		if (format != null)
		{
			try
			{
				return string.Format(format, first, second);
			}
			catch (FormatException)
			{
			}
		}
		return string.Format(fallback, first, second);
	}
}
