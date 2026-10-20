using System.Buffers;
using System.CodeDom.Compiler;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace System.Text.RegularExpressions.Generated;

[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.42308")]
internal static class _003CRegexGenerator_g_003EF5A919AF36546C3D797A1EDA78623F5FD69C6B1313DB9045DB2CC6C90915164F1__Utilities
{
	internal static readonly TimeSpan s_defaultTimeout = ((AppContext.GetData("REGEX_DEFAULT_MATCH_TIMEOUT") is TimeSpan timeSpan) ? timeSpan : Regex.InfiniteMatchTimeout);

	internal static readonly bool s_hasTimeout = s_defaultTimeout != Regex.InfiniteMatchTimeout;

	private const int WordCategoriesMask = 262463;

	internal static readonly SearchValues<char> s_ascii_FFC1FFFFFED300FCFFFFFFFFFFFFFFFF = SearchValues.Create("\0\u0001\u0002\u0003\u0004\u0005\u0006\a\b\u000e\u000f\u0010\u0011\u0012\u0013\u0014\u0015\u0016\u0017\u0018\u0019\u001a\u001b\u001c\u001d\u001e\u001f!\"#$%&'(),./:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~\u007f".AsSpan());

	internal static readonly SearchValues<string> s_indexOfString_125B08C013F66B78C34DC2C8BAB5210F38BAB697CCD4D15791D35C381B80422D = SearchValues.Create(new ReadOnlySpan<string>("\n\n"), StringComparison.Ordinal);

	internal static readonly SearchValues<string> s_indexOfString_3325914D28F92F1F2E49B7A77A767D8624D7AAEB658842B53860E7DD50419293 = SearchValues.Create(new ReadOnlySpan<string>("++"), StringComparison.Ordinal);

	internal static readonly SearchValues<string> s_indexOfString_3A7421F7D81B04F7710910266415B8E05CE596E7792AE006D5225213FB1ADE6F = SearchValues.Create(new ReadOnlySpan<string>("\"t\":"), StringComparison.Ordinal);

	internal static readonly SearchValues<string> s_indexOfString_4A9084D27E350810B767E64C519DE5B703918D35110A6B1184C035CE277E0E26 = SearchValues.Create(new ReadOnlySpan<string>("=="), StringComparison.Ordinal);

	internal static readonly SearchValues<string> s_indexOfString_4DF02EB6A8B26A8E8AE968B0C6D57D7D5175C6C3C4EBD7238C244D8A4C1ADD4E = SearchValues.Create(new ReadOnlySpan<string>("  "), StringComparison.Ordinal);

	internal static readonly SearchValues<string> s_indexOfString_52DEF0D01D1326103BE9ADCB1EF8B93027F9650D81DEDE18C4282E10F9244AD6 = SearchValues.Create(new ReadOnlySpan<string>(":::"), StringComparison.Ordinal);

	internal static readonly SearchValues<string> s_indexOfString_895EFF7AC52BDC70805F18A08EFB71D38DEDE24AD352A3744C3232E9150FF397 = SearchValues.Create(new ReadOnlySpan<string>("---"), StringComparison.Ordinal);

	internal static readonly SearchValues<string> s_indexOfString_A35D34A63ABD1CA9D7860076AA22A8E02AF70FE7A4FEF864363EC52527771C59 = SearchValues.Create(new ReadOnlySpan<string>("!["), StringComparison.Ordinal);

	internal static readonly SearchValues<string> s_indexOfString_ACD1AF3308000737329946CAB3D500C3329228244FA0C355223CB35C8E9ACBE9 = SearchValues.Create(new ReadOnlySpan<string>("```"), StringComparison.Ordinal);

	internal static readonly SearchValues<string> s_indexOfString_CD52C9D118B084D434A4D219ABB8EA35B16B26BDCD7FAADC77802968479904D4 = SearchValues.Create(new ReadOnlySpan<string>("~~"), StringComparison.Ordinal);

	internal static readonly SearchValues<string> s_indexOfString_FF9831EBF0CE6A95A416D072B315A0C90C863C89D6EC5C6A85969246377FAB47 = SearchValues.Create(new ReadOnlySpan<string>("[^"), StringComparison.Ordinal);

	private static ReadOnlySpan<byte> WordCharBitmap => new byte[16]
	{
		0, 0, 0, 0, 0, 0, 255, 3, 254, 255,
		255, 135, 254, 255, 255, 7
	};

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static int IndexOfNonAsciiOrAny_6479C023D753664A17E26CF70D50E1D2DDA2FE84CA33DF23BA4D362FEB3DD5C3(this ReadOnlySpan<char> span)
	{
		int num = span.IndexOfAnyExcept(s_ascii_FFC1FFFFFED300FCFFFFFFFFFFFFFFFF);
		if ((uint)num < (uint)span.Length)
		{
			if (char.IsAscii(span[num]))
			{
				return num;
			}
			do
			{
				char c;
				if (((c = span[num]) < '\u0080') ? ((byte)("㸀\0ⰁϿ\0\0\0\0"[(int)c >> 4] & (1 << (c & 0xF))) != 0) : RegexRunner.CharInClass(c, "\0\u0004\u0002*,-.d\t"))
				{
					return num;
				}
				num++;
			}
			while ((uint)num < (uint)span.Length);
		}
		return -1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool IsWordChar(char ch)
	{
		ReadOnlySpan<byte> wordCharBitmap = WordCharBitmap;
		int num = (int)ch >> 3;
		if ((uint)num >= (uint)wordCharBitmap.Length)
		{
			return (0x4013F & (1 << (int)CharUnicodeInfo.GetUnicodeCategory(ch))) != 0;
		}
		return (wordCharBitmap[num] & (1 << (ch & 7))) != 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void StackPush(ref int[] stack, ref int pos, int arg0)
	{
		int[] array = stack;
		int num = pos;
		if ((uint)num < (uint)array.Length)
		{
			array[num] = arg0;
			pos++;
		}
		else
		{
			WithResize(ref stack, ref pos, arg0);
		}
		[MethodImpl(MethodImplOptions.NoInlining)]
		static void WithResize(ref int[] reference, ref int reference2, int arg1)
		{
			Array.Resize(ref reference, reference2 * 2);
			StackPush(ref reference, ref reference2, arg1);
		}
	}
}
