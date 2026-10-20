using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;

namespace System.Text.RegularExpressions.Generated;

[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.42308")]
[SkipLocalsInit]
internal sealed class _003CRegexGenerator_g_003EFDD7C65F98B3009022F7AC561E347A370A49D71D176A01AA07C89E66F21EBD08F__CommandRegex_2 : Regex
{
	private sealed class RunnerFactory : RegexRunnerFactory
	{
		private sealed class Runner : RegexRunner
		{
			protected override void Scan(ReadOnlySpan<char> inputSpan)
			{
				if (TryFindNextPossibleStartingPosition(inputSpan) && !TryMatchAtCurrentPosition(inputSpan))
				{
					runtextpos = inputSpan.Length;
				}
			}

			private bool TryFindNextPossibleStartingPosition(ReadOnlySpan<char> inputSpan)
			{
				int num = runtextpos;
				if (num <= inputSpan.Length - 12 && num == 0)
				{
					return true;
				}
				runtextpos = inputSpan.Length;
				return false;
			}

			private bool TryMatchAtCurrentPosition(ReadOnlySpan<char> inputSpan)
			{
				int num = runtextpos;
				int start = num;
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				int num5 = 0;
				ReadOnlySpan<char> span = inputSpan.Slice(num);
				if (num != 0)
				{
					UncaptureUntil(0);
					return false;
				}
				int i;
				for (i = 0; (uint)i < (uint)span.Length && char.IsWhiteSpace(span.get_Item(i)); i++)
				{
				}
				span = span.Slice(i);
				num += i;
				if ((uint)span.Length < 4u || !span.StartsWith("/pdr".AsSpan(), StringComparison.OrdinalIgnoreCase))
				{
					UncaptureUntil(0);
					return false;
				}
				num += 4;
				span = inputSpan.Slice(num);
				int j;
				for (j = 0; (uint)j < (uint)span.Length && char.IsWhiteSpace(span.get_Item(j)); j++)
				{
				}
				if (j == 0)
				{
					UncaptureUntil(0);
					return false;
				}
				span = span.Slice(j);
				num += j;
				if ((uint)span.Length < 5u || !span.StartsWith("micon".AsSpan(), StringComparison.OrdinalIgnoreCase))
				{
					UncaptureUntil(0);
					return false;
				}
				num += 5;
				span = inputSpan.Slice(num);
				int k;
				for (k = 0; (uint)k < (uint)span.Length && char.IsWhiteSpace(span.get_Item(k)); k++)
				{
				}
				if (k == 0)
				{
					UncaptureUntil(0);
					return false;
				}
				span = span.Slice(k);
				num += k;
				num2 = num;
				int l;
				for (l = 0; (uint)l < (uint)span.Length && char.IsDigit(span.get_Item(l)); l++)
				{
				}
				if (l == 0)
				{
					UncaptureUntil(0);
					return false;
				}
				span = span.Slice(l);
				num += l;
				Capture(1, num2, num);
				num4 = num;
				int m;
				for (m = 0; (uint)m < (uint)span.Length && char.IsWhiteSpace(span.get_Item(m)); m++)
				{
				}
				span = span.Slice(m);
				num += m;
				num5 = num;
				while (true)
				{
					num3 = Crawlpos();
					if (num >= inputSpan.Length - 1 && ((uint)num >= (uint)inputSpan.Length || inputSpan.get_Item(num) == '\n'))
					{
						break;
					}
					UncaptureUntil(num3);
					if (_003CRegexGenerator_g_003EFDD7C65F98B3009022F7AC561E347A370A49D71D176A01AA07C89E66F21EBD08F__Utilities.s_hasTimeout)
					{
						CheckTimeout();
					}
					if (num4 >= num5)
					{
						UncaptureUntil(0);
						return false;
					}
					num = --num5;
					span = inputSpan.Slice(num);
				}
				runtextpos = num;
				Capture(0, start, num);
				return true;
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				void UncaptureUntil(int capturePosition)
				{
					while (Crawlpos() > capturePosition)
					{
						Uncapture();
					}
				}
			}
		}

		protected override RegexRunner CreateInstance()
		{
			return new Runner();
		}
	}

	internal static readonly _003CRegexGenerator_g_003EFDD7C65F98B3009022F7AC561E347A370A49D71D176A01AA07C89E66F21EBD08F__CommandRegex_2 Instance = new _003CRegexGenerator_g_003EFDD7C65F98B3009022F7AC561E347A370A49D71D176A01AA07C89E66F21EBD08F__CommandRegex_2();

	private _003CRegexGenerator_g_003EFDD7C65F98B3009022F7AC561E347A370A49D71D176A01AA07C89E66F21EBD08F__CommandRegex_2()
	{
		pattern = "^\\s*/pdr\\s+micon\\s+(\\d+)\\s*$";
		roptions = RegexOptions.IgnoreCase;
		Regex.ValidateMatchTimeout(_003CRegexGenerator_g_003EFDD7C65F98B3009022F7AC561E347A370A49D71D176A01AA07C89E66F21EBD08F__Utilities.s_defaultTimeout);
		internalMatchTimeout = _003CRegexGenerator_g_003EFDD7C65F98B3009022F7AC561E347A370A49D71D176A01AA07C89E66F21EBD08F__Utilities.s_defaultTimeout;
		factory = new RunnerFactory();
		capsize = 2;
	}
}
