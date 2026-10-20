using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using OmenTools.Localization.Abstractions;

namespace OmenTools.Localization.Parsers;

public sealed class JavaPropertiesLocalizationParser(Encoding? encoding = null) : ILocalizationParser
{
	private readonly Encoding encoding = encoding ?? Encoding.UTF8;

	public FrozenDictionary<string, string> Parse(Stream stream)
	{
		ArgumentNullException.ThrowIfNull(stream, "stream");
		using StreamReader reader = new StreamReader(stream, encoding, detectEncodingFromByteOrderMarks: true, -1, leaveOpen: true);
		Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.Ordinal);
		foreach (string item3 in ReadLogicalLines(reader))
		{
			if (string.IsNullOrWhiteSpace(item3))
			{
				continue;
			}
			string text = item3.TrimStart();
			if (!text.StartsWith('#') && !text.StartsWith('!'))
			{
				(string Key, string Value) tuple = SplitKeyValue(item3);
				string item = tuple.Key;
				string item2 = tuple.Value;
				string text2 = Unescape(item);
				if (!string.IsNullOrWhiteSpace(text2))
				{
					dictionary[text2] = Unescape(item2);
				}
			}
		}
		return dictionary.ToFrozenResource();
	}

	private static IEnumerable<string> ReadLogicalLines(StreamReader reader)
	{
		StringBuilder builder = null;
		while (true)
		{
			string text = reader.ReadLine();
			if (text == null)
			{
				break;
			}
			string text2 = ((builder == null) ? text : TrimLeadingWhitespace(text));
			if (!HasContinuation(text2))
			{
				if (builder == null)
				{
					yield return text2;
					continue;
				}
				builder.Append(text2);
				yield return builder.ToString();
				builder = null;
				continue;
			}
			string text3 = text2;
			text2 = text3.Substring(0, text3.Length - 1);
			if (builder == null)
			{
				builder = new StringBuilder();
			}
			builder.Append(text2);
		}
		if (builder != null && builder.Length > 0)
		{
			yield return builder.ToString();
		}
	}

	private static string TrimLeadingWhitespace(string value)
	{
		int i;
		for (i = 0; i < value.Length && char.IsWhiteSpace(value[i]); i++)
		{
		}
		if (i != 0)
		{
			return value.Substring(i);
		}
		return value;
	}

	private static bool HasContinuation(string value)
	{
		int num = 0;
		int num2 = value.Length - 1;
		while (num2 >= 0 && value[num2] == '\\')
		{
			num++;
			num2--;
		}
		return num % 2 == 1;
	}

	private static (string Key, string Value) SplitKeyValue(string line)
	{
		int num = -1;
		bool flag = false;
		bool flag2;
		for (int i = 0; i < line.Length; i++)
		{
			char c = line[i];
			if (flag)
			{
				flag = false;
				continue;
			}
			switch (c)
			{
			case '\\':
				flag = true;
				continue;
			case ':':
			case '=':
				flag2 = true;
				break;
			default:
				flag2 = false;
				break;
			}
			if (!flag2 && !char.IsWhiteSpace(c))
			{
				continue;
			}
			num = i;
			break;
		}
		if (num < 0)
		{
			return (Key: line.Trim(), Value: string.Empty);
		}
		int num2 = num;
		while (num2 > 0 && char.IsWhiteSpace(line[num2 - 1]))
		{
			num2--;
		}
		int j;
		for (j = num; j < line.Length && char.IsWhiteSpace(line[j]); j++)
		{
		}
		flag2 = j < line.Length;
		if (flag2)
		{
			char c2 = line[j];
			bool flag3 = ((c2 == ':' || c2 == '=') ? true : false);
			flag2 = flag3;
		}
		if (flag2)
		{
			j++;
		}
		for (; j < line.Length && char.IsWhiteSpace(line[j]); j++)
		{
		}
		return (Key: line.Substring(0, num2), Value: (j < line.Length) ? line.Substring(j) : string.Empty);
	}

	private static string Unescape(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return string.Empty;
		}
		StringBuilder stringBuilder = new StringBuilder(value.Length);
		for (int i = 0; i < value.Length; i++)
		{
			char c = value[i];
			if (c != '\\' || i == value.Length - 1)
			{
				stringBuilder.Append(c);
				continue;
			}
			char c2 = value[++i];
			switch (c2)
			{
			case 't':
				stringBuilder.Append('\t');
				continue;
			case 'r':
				stringBuilder.Append('\r');
				continue;
			case 'n':
				stringBuilder.Append('\n');
				continue;
			case 'f':
				stringBuilder.Append('\f');
				continue;
			case '\\':
				stringBuilder.Append('\\');
				continue;
			case ' ':
				stringBuilder.Append(' ');
				continue;
			case ':':
				stringBuilder.Append(':');
				continue;
			case '=':
				stringBuilder.Append('=');
				continue;
			case '#':
				stringBuilder.Append('#');
				continue;
			case '!':
				stringBuilder.Append('!');
				continue;
			case 'u':
				if (i + 4 < value.Length)
				{
					if (ushort.TryParse(value.AsSpan(i + 1, 4), NumberStyles.HexNumber, null, out var result))
					{
						stringBuilder.Append((char)result);
						i += 4;
					}
					else
					{
						stringBuilder.Append("\\u");
					}
					continue;
				}
				break;
			}
			stringBuilder.Append(c2);
		}
		return stringBuilder.ToString();
	}
}
