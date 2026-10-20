using System;
using System.Buffers;
using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Text.RegularExpressions.Generated;

namespace OmenTools.Extensions;

public static class StringExtension
{
	extension(char c)
	{
		public bool IsFullWidth()
		{
			switch (c)
			{
			case '\u3000':
			case '！':
			case '＂':
			case '＃':
			case '＄':
			case '％':
			case '＆':
			case '＇':
			case '（':
			case '）':
			case '＊':
			case '＋':
			case '，':
			case '－':
			case '．':
			case '／':
			case '０':
			case '１':
			case '２':
			case '３':
			case '４':
			case '５':
			case '６':
			case '７':
			case '８':
			case '９':
			case '：':
			case '；':
			case '＜':
			case '＝':
			case '＞':
			case '？':
			case '＠':
			case 'Ａ':
			case 'Ｂ':
			case 'Ｃ':
			case 'Ｄ':
			case 'Ｅ':
			case 'Ｆ':
			case 'Ｇ':
			case 'Ｈ':
			case 'Ｉ':
			case 'Ｊ':
			case 'Ｋ':
			case 'Ｌ':
			case 'Ｍ':
			case 'Ｎ':
			case 'Ｏ':
			case 'Ｐ':
			case 'Ｑ':
			case 'Ｒ':
			case 'Ｓ':
			case 'Ｔ':
			case 'Ｕ':
			case 'Ｖ':
			case 'Ｗ':
			case 'Ｘ':
			case 'Ｙ':
			case 'Ｚ':
			case '［':
			case '＼':
			case '］':
			case '\uff3e':
			case '\uff3f':
			case '\uff40':
			case 'ａ':
			case 'ｂ':
			case 'ｃ':
			case 'ｄ':
			case 'ｅ':
			case 'ｆ':
			case 'ｇ':
			case 'ｈ':
			case 'ｉ':
			case 'ｊ':
			case 'ｋ':
			case 'ｌ':
			case 'ｍ':
			case 'ｎ':
			case 'ｏ':
			case 'ｐ':
			case 'ｑ':
			case 'ｒ':
			case 'ｓ':
			case 'ｔ':
			case 'ｕ':
			case 'ｖ':
			case 'ｗ':
			case 'ｘ':
			case 'ｙ':
			case 'ｚ':
			case '｛':
			case '｜':
			case '｝':
			case '～':
				return true;
			default:
				return false;
			}
		}

		public bool IsHalfWidth()
		{
			if (c >= ' ')
			{
				return c <= '~';
			}
			return false;
		}

		public char ToHalfWidth()
		{
			switch (c)
			{
			case '！':
			case '＂':
			case '＃':
			case '＄':
			case '％':
			case '＆':
			case '＇':
			case '（':
			case '）':
			case '＊':
			case '＋':
			case '，':
			case '－':
			case '．':
			case '／':
			case '０':
			case '１':
			case '２':
			case '３':
			case '４':
			case '５':
			case '６':
			case '７':
			case '８':
			case '９':
			case '：':
			case '；':
			case '＜':
			case '＝':
			case '＞':
			case '？':
			case '＠':
			case 'Ａ':
			case 'Ｂ':
			case 'Ｃ':
			case 'Ｄ':
			case 'Ｅ':
			case 'Ｆ':
			case 'Ｇ':
			case 'Ｈ':
			case 'Ｉ':
			case 'Ｊ':
			case 'Ｋ':
			case 'Ｌ':
			case 'Ｍ':
			case 'Ｎ':
			case 'Ｏ':
			case 'Ｐ':
			case 'Ｑ':
			case 'Ｒ':
			case 'Ｓ':
			case 'Ｔ':
			case 'Ｕ':
			case 'Ｖ':
			case 'Ｗ':
			case 'Ｘ':
			case 'Ｙ':
			case 'Ｚ':
			case '［':
			case '＼':
			case '］':
			case '\uff3e':
			case '\uff3f':
			case '\uff40':
			case 'ａ':
			case 'ｂ':
			case 'ｃ':
			case 'ｄ':
			case 'ｅ':
			case 'ｆ':
			case 'ｇ':
			case 'ｈ':
			case 'ｉ':
			case 'ｊ':
			case 'ｋ':
			case 'ｌ':
			case 'ｍ':
			case 'ｎ':
			case 'ｏ':
			case 'ｐ':
			case 'ｑ':
			case 'ｒ':
			case 'ｓ':
			case 'ｔ':
			case 'ｕ':
			case 'ｖ':
			case 'ｗ':
			case 'ｘ':
			case 'ｙ':
			case 'ｚ':
			case '｛':
			case '｜':
			case '｝':
			case '～':
				return (char)(c - 65248);
			case '\u3000':
				return ' ';
			default:
				return c;
			}
		}

		public char ToFullWidth()
		{
			switch (c)
			{
			case '!':
			case '"':
			case '#':
			case '$':
			case '%':
			case '&':
			case '\'':
			case '(':
			case ')':
			case '*':
			case '+':
			case ',':
			case '-':
			case '.':
			case '/':
			case '0':
			case '1':
			case '2':
			case '3':
			case '4':
			case '5':
			case '6':
			case '7':
			case '8':
			case '9':
			case ':':
			case ';':
			case '<':
			case '=':
			case '>':
			case '?':
			case '@':
			case 'A':
			case 'B':
			case 'C':
			case 'D':
			case 'E':
			case 'F':
			case 'G':
			case 'H':
			case 'I':
			case 'J':
			case 'K':
			case 'L':
			case 'M':
			case 'N':
			case 'O':
			case 'P':
			case 'Q':
			case 'R':
			case 'S':
			case 'T':
			case 'U':
			case 'V':
			case 'W':
			case 'X':
			case 'Y':
			case 'Z':
			case '[':
			case '\\':
			case ']':
			case '^':
			case '_':
			case '`':
			case 'a':
			case 'b':
			case 'c':
			case 'd':
			case 'e':
			case 'f':
			case 'g':
			case 'h':
			case 'i':
			case 'j':
			case 'k':
			case 'l':
			case 'm':
			case 'n':
			case 'o':
			case 'p':
			case 'q':
			case 'r':
			case 's':
			case 't':
			case 'u':
			case 'v':
			case 'w':
			case 'x':
			case 'y':
			case 'z':
			case '{':
			case '|':
			case '}':
			case '~':
				return (char)(c + 65248);
			case ' ':
				return '\u3000';
			default:
				return c;
			}
		}
	}

	extension(string input)
	{
		public string TrimQuotationMarks()
		{
			int i = 0;
			int num;
			for (num = input.Length - 1; i <= num && IsQuotationMark(input[i]); i++)
			{
			}
			while (num >= i && IsQuotationMark(input[num]))
			{
				num--;
			}
			for (; i <= num && char.IsWhiteSpace(input[i]); i++)
			{
			}
			while (num >= i && char.IsWhiteSpace(input[num]))
			{
				num--;
			}
			if (i > num)
			{
				return string.Empty;
			}
			if (i == 0 && num == input.Length - 1)
			{
				return input;
			}
			return input.Substring(i, num - i + 1);
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static bool IsQuotationMark(char c)
			{
				switch (c)
				{
				case '"':
				case '\'':
				case '«':
				case '»':
				case '‘':
				case '’':
				case '‚':
				case '“':
				case '”':
				case '„':
				case '‹':
				case '›':
				case '「':
				case '」':
				case '『':
				case '』':
				case '﹁':
				case '﹂':
				case '﹃':
				case '﹄':
					return true;
				default:
					return false;
				}
			}
		}

		public string ToLowerAndHalfWidth()
		{
			if (string.IsNullOrEmpty(input))
			{
				return input;
			}
			string text = input.ToLower();
			StringBuilder stringBuilder = new StringBuilder();
			string text2 = text;
			foreach (char c in text2)
			{
				switch (c)
				{
				case '\u3000':
					stringBuilder.Append(' ');
					break;
				case '！':
				case '＂':
				case '＃':
				case '＄':
				case '％':
				case '＆':
				case '＇':
				case '（':
				case '）':
				case '＊':
				case '＋':
				case '，':
				case '－':
				case '．':
				case '／':
				case '０':
				case '１':
				case '２':
				case '３':
				case '４':
				case '５':
				case '６':
				case '７':
				case '８':
				case '９':
				case '：':
				case '；':
				case '＜':
				case '＝':
				case '＞':
				case '？':
				case '＠':
				case 'Ａ':
				case 'Ｂ':
				case 'Ｃ':
				case 'Ｄ':
				case 'Ｅ':
				case 'Ｆ':
				case 'Ｇ':
				case 'Ｈ':
				case 'Ｉ':
				case 'Ｊ':
				case 'Ｋ':
				case 'Ｌ':
				case 'Ｍ':
				case 'Ｎ':
				case 'Ｏ':
				case 'Ｐ':
				case 'Ｑ':
				case 'Ｒ':
				case 'Ｓ':
				case 'Ｔ':
				case 'Ｕ':
				case 'Ｖ':
				case 'Ｗ':
				case 'Ｘ':
				case 'Ｙ':
				case 'Ｚ':
				case '［':
				case '＼':
				case '］':
				case '\uff3e':
				case '\uff3f':
				case '\uff40':
				case 'ａ':
				case 'ｂ':
				case 'ｃ':
				case 'ｄ':
				case 'ｅ':
				case 'ｆ':
				case 'ｇ':
				case 'ｈ':
				case 'ｉ':
				case 'ｊ':
				case 'ｋ':
				case 'ｌ':
				case 'ｍ':
				case 'ｎ':
				case 'ｏ':
				case 'ｐ':
				case 'ｑ':
				case 'ｒ':
				case 'ｓ':
				case 'ｔ':
				case 'ｕ':
				case 'ｖ':
				case 'ｗ':
				case 'ｘ':
				case 'ｙ':
				case 'ｚ':
				case '｛':
				case '｜':
				case '｝':
				case '～':
					stringBuilder.Append((char)(c - 65248));
					break;
				default:
					stringBuilder.Append(c);
					break;
				}
			}
			return stringBuilder.ToString();
		}

		[SkipLocalsInit]
		public bool TryReplaceIgnoreCase(string target, string replacement, [NotNullWhen(true)] out string? result)
		{
			if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(target))
			{
				result = null;
				return false;
			}
			ReadOnlySpan<char> span = input.AsSpan();
			ReadOnlySpan<char> value = target.AsSpan();
			int num = span.IndexOf(value, StringComparison.OrdinalIgnoreCase);
			if (num == -1)
			{
				result = null;
				return false;
			}
			int length = input.Length;
			int length2 = target.Length;
			int length3 = replacement.Length;
			if (length2 == length3)
			{
				(string, string, string, int) state = (input, target, replacement, num);
				result = string.Create(length, state, delegate(Span<char> span5, (string Input, string Target, string Replacement, int FirstIndex) s)
				{
					ReadOnlySpan<char> span4 = s.Input.AsSpan();
					ReadOnlySpan<char> value2 = s.Target.AsSpan();
					ReadOnlySpan<char> readOnlySpan2 = s.Replacement.AsSpan();
					int length4 = value2.Length;
					ref char reference3 = ref MemoryMarshal.GetReference(span5);
					ref char reference4 = ref MemoryMarshal.GetReference(span4);
					int num10 = 0;
					int num11 = s.FirstIndex;
					while (true)
					{
						int num12 = num11 - num10;
						if (num12 > 0)
						{
							Unsafe.CopyBlockUnaligned(ref Unsafe.As<char, byte>(ref Unsafe.Add(ref reference3, num10)), in Unsafe.As<char, byte>(ref Unsafe.Add(ref reference4, num10)), (uint)(num12 * 2));
						}
						readOnlySpan2.CopyTo(span5.Slice(num11));
						num10 = num11 + length4;
						int num13 = span4.Slice(num10).IndexOf(value2, StringComparison.OrdinalIgnoreCase);
						if (num13 == -1)
						{
							break;
						}
						num11 = num10 + num13;
					}
					int num14 = span4.Length - num10;
					if (num14 > 0)
					{
						Unsafe.CopyBlockUnaligned(ref Unsafe.As<char, byte>(ref Unsafe.Add(ref reference3, num10)), in Unsafe.As<char, byte>(ref Unsafe.Add(ref reference4, num10)), (uint)(num14 * 2));
					}
				});
				return true;
			}
			long num2 = length;
			if (length3 > length2)
			{
				long num3 = (long)(length / length2) * (long)(length3 - length2);
				num2 += num3;
			}
			char[] array = null;
			Span<char> span2 = ((num2 > 512) ? ((Span<char>)(array = ArrayPool<char>.Shared.Rent((int)num2))) : stackalloc char[512]);
			Span<char> span3 = span2;
			try
			{
				ref char reference = ref MemoryMarshal.GetReference(span3);
				ref char reference2 = ref MemoryMarshal.GetReference(span);
				int num4 = 0;
				int num5 = 0;
				int num6 = num;
				ReadOnlySpan<char> readOnlySpan = replacement.AsSpan();
				while (true)
				{
					int num7 = num6 - num5;
					if (num7 > 0)
					{
						Unsafe.CopyBlockUnaligned(ref Unsafe.As<char, byte>(ref Unsafe.Add(ref reference, num4)), in Unsafe.As<char, byte>(ref Unsafe.Add(ref reference2, num5)), (uint)(num7 * 2));
						num4 += num7;
					}
					if (length3 > 0)
					{
						readOnlySpan.CopyTo(span3.Slice(num4));
						num4 += length3;
					}
					num5 = num6 + length2;
					if (num5 >= length)
					{
						break;
					}
					int num8 = span.Slice(num5).IndexOf(value, StringComparison.OrdinalIgnoreCase);
					if (num8 == -1)
					{
						break;
					}
					num6 = num5 + num8;
				}
				int num9 = length - num5;
				if (num9 > 0)
				{
					Unsafe.CopyBlockUnaligned(ref Unsafe.As<char, byte>(ref Unsafe.Add(ref reference, num4)), in Unsafe.As<char, byte>(ref Unsafe.Add(ref reference2, num5)), (uint)(num9 * 2));
					num4 += num9;
				}
				result = new string(span3.Slice(0, num4));
				return true;
			}
			finally
			{
				if (array != null)
				{
					ArrayPool<char>.Shared.Return(array);
				}
			}
		}

		public bool IsChinese()
		{
			if (string.IsNullOrEmpty(input))
			{
				return false;
			}
			foreach (Rune item in input.EnumerateRunes())
			{
				if (!IsChineseRune(item))
				{
					return false;
				}
			}
			return true;
		}

		public bool IsAnyChinese()
		{
			if (string.IsNullOrEmpty(input))
			{
				return false;
			}
			foreach (Rune item in input.EnumerateRunes())
			{
				if (IsChineseRune(item))
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsChineseRune(Rune rune)
		{
			uint value = (uint)rune.Value;
			if (value - 19968 > 20991 && value - 13312 > 6591 && value - 131072 > 64031 && value - 196608 > 9135)
			{
				return value - 63744 <= 511;
			}
			return true;
		}

		public string SanitizeMarkdown()
		{
			if (string.IsNullOrWhiteSpace(input))
			{
				return string.Empty;
			}
			input = FrontMatterRegex().Replace(input, string.Empty);
			input = CustomContainerRegex().Replace(input, string.Empty);
			input = CodeFenceRegex().Replace(input, "$1");
			input = ImageRegex().Replace(input, "$1");
			input = LinkRegex().Replace(input, "$1");
			input = FootnoteRegex().Replace(input, string.Empty);
			input = HeaderRegex().Replace(input, "$1");
			input = StrikeThroughRegex().Replace(input, "$1");
			input = HighlightRegex().Replace(input, "$1");
			input = InsertRegex().Replace(input, "$1");
			input = InlineCodeRegex().Replace(input, "$1");
			input = BlockquoteRegex().Replace(input, "$1");
			input = ListRegex().Replace(input, "$2");
			input = HtmlTagRegex().Replace(input, string.Empty);
			input = TableRegex().Replace(input, " ");
			input = MultipleBlankLineRegex().Replace(input, "\n");
			input = MultipleSpaceRegex().Replace(input, " ");
			return input.Trim();
		}

		public string? NullIfEmpty()
		{
			if (!string.IsNullOrEmpty(input))
			{
				return input;
			}
			return null;
		}

		public string? NullIfWhitespace()
		{
			if (!string.IsNullOrWhiteSpace(input))
			{
				return input;
			}
			return null;
		}
	}

	private const int TIMEOUT_MS = 1000;

	[GeneratedRegex("^---[\\s\\S]*?---\\s*$", RegexOptions.Multiline, 1000)]
	[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.42308")]
	private static Regex FrontMatterRegex()
	{
		return _003CRegexGenerator_g_003EF5A919AF36546C3D797A1EDA78623F5FD69C6B1313DB9045DB2CC6C90915164F1__FrontMatterRegex_0.Instance;
	}

	[GeneratedRegex(":::[^\\n]*\\n|:::|:::\\s*$", RegexOptions.Multiline, 1000)]
	[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.42308")]
	private static Regex CustomContainerRegex()
	{
		return _003CRegexGenerator_g_003EF5A919AF36546C3D797A1EDA78623F5FD69C6B1313DB9045DB2CC6C90915164F1__CustomContainerRegex_1.Instance;
	}

	[GeneratedRegex("`{3,}\\w*\\s*([\\s\\S]*?)\\s*`{3,}", RegexOptions.None, 1000)]
	[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.42308")]
	private static Regex CodeFenceRegex()
	{
		return _003CRegexGenerator_g_003EF5A919AF36546C3D797A1EDA78623F5FD69C6B1313DB9045DB2CC6C90915164F1__CodeFenceRegex_2.Instance;
	}

	[GeneratedRegex("^#{1,6}\\s+(.*)$", RegexOptions.Multiline, 1000)]
	[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.42308")]
	private static Regex HeaderRegex()
	{
		return _003CRegexGenerator_g_003EF5A919AF36546C3D797A1EDA78623F5FD69C6B1313DB9045DB2CC6C90915164F1__HeaderRegex_3.Instance;
	}

	[GeneratedRegex("!\\[([^\\]]*)\\]\\([^)]+\\)", RegexOptions.None, 1000)]
	[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.42308")]
	private static Regex ImageRegex()
	{
		return _003CRegexGenerator_g_003EF5A919AF36546C3D797A1EDA78623F5FD69C6B1313DB9045DB2CC6C90915164F1__ImageRegex_4.Instance;
	}

	[GeneratedRegex("(?<!\\!)\\[([^\\]]*)\\]\\([^)]+\\)", RegexOptions.None, 1000)]
	[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.42308")]
	private static Regex LinkRegex()
	{
		return _003CRegexGenerator_g_003EF5A919AF36546C3D797A1EDA78623F5FD69C6B1313DB9045DB2CC6C90915164F1__LinkRegex_5.Instance;
	}

	[GeneratedRegex("~~(.*?)~~", RegexOptions.Singleline, 1000)]
	[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.42308")]
	private static Regex StrikeThroughRegex()
	{
		return _003CRegexGenerator_g_003EF5A919AF36546C3D797A1EDA78623F5FD69C6B1313DB9045DB2CC6C90915164F1__StrikeThroughRegex_6.Instance;
	}

	[GeneratedRegex("`([^`]+)`", RegexOptions.Singleline, 1000)]
	[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.42308")]
	private static Regex InlineCodeRegex()
	{
		return _003CRegexGenerator_g_003EF5A919AF36546C3D797A1EDA78623F5FD69C6B1313DB9045DB2CC6C90915164F1__InlineCodeRegex_7.Instance;
	}

	[GeneratedRegex("==([^=]+)==", RegexOptions.Singleline, 1000)]
	[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.42308")]
	private static Regex HighlightRegex()
	{
		return _003CRegexGenerator_g_003EF5A919AF36546C3D797A1EDA78623F5FD69C6B1313DB9045DB2CC6C90915164F1__HighlightRegex_8.Instance;
	}

	[GeneratedRegex("\\+\\+([^+]+)\\+\\+", RegexOptions.Singleline, 1000)]
	[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.42308")]
	private static Regex InsertRegex()
	{
		return _003CRegexGenerator_g_003EF5A919AF36546C3D797A1EDA78623F5FD69C6B1313DB9045DB2CC6C90915164F1__InsertRegex_9.Instance;
	}

	[GeneratedRegex("^>\\s?(.*)$", RegexOptions.Multiline, 1000)]
	[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.42308")]
	private static Regex BlockquoteRegex()
	{
		return _003CRegexGenerator_g_003EF5A919AF36546C3D797A1EDA78623F5FD69C6B1313DB9045DB2CC6C90915164F1__BlockquoteRegex_10.Instance;
	}

	[GeneratedRegex("^[\\s]*([-*+]|\\d+.)\\s+(.*)$", RegexOptions.Multiline, 1000)]
	[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.42308")]
	private static Regex ListRegex()
	{
		return _003CRegexGenerator_g_003EF5A919AF36546C3D797A1EDA78623F5FD69C6B1313DB9045DB2CC6C90915164F1__ListRegex_11.Instance;
	}

	[GeneratedRegex("\\[\\^(.*?)\\]:?", RegexOptions.None, 1000)]
	[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.42308")]
	private static Regex FootnoteRegex()
	{
		return _003CRegexGenerator_g_003EF5A919AF36546C3D797A1EDA78623F5FD69C6B1313DB9045DB2CC6C90915164F1__FootnoteRegex_12.Instance;
	}

	[GeneratedRegex("<[^>]+>", RegexOptions.None, 1000)]
	[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.42308")]
	private static Regex HtmlTagRegex()
	{
		return _003CRegexGenerator_g_003EF5A919AF36546C3D797A1EDA78623F5FD69C6B1313DB9045DB2CC6C90915164F1__HtmlTagRegex_13.Instance;
	}

	[GeneratedRegex("^\\|.*?\\|\\s*$", RegexOptions.Multiline, 1000)]
	[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.42308")]
	private static Regex TableRegex()
	{
		return _003CRegexGenerator_g_003EF5A919AF36546C3D797A1EDA78623F5FD69C6B1313DB9045DB2CC6C90915164F1__TableRegex_14.Instance;
	}

	[GeneratedRegex("\\n{2,}", RegexOptions.None, 1000)]
	[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.42308")]
	private static Regex MultipleBlankLineRegex()
	{
		return _003CRegexGenerator_g_003EF5A919AF36546C3D797A1EDA78623F5FD69C6B1313DB9045DB2CC6C90915164F1__MultipleBlankLineRegex_15.Instance;
	}

	[GeneratedRegex("[ ]{2,}", RegexOptions.None, 1000)]
	[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.42308")]
	private static Regex MultipleSpaceRegex()
	{
		return _003CRegexGenerator_g_003EF5A919AF36546C3D797A1EDA78623F5FD69C6B1313DB9045DB2CC6C90915164F1__MultipleSpaceRegex_16.Instance;
	}
}
