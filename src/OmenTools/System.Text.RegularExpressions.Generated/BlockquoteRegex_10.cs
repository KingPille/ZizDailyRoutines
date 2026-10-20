using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;

namespace System.Text.RegularExpressions.Generated;

[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.42308")]
[SkipLocalsInit]
internal sealed class _003CRegexGenerator_g_003EF5A919AF36546C3D797A1EDA78623F5FD69C6B1313DB9045DB2CC6C90915164F1__BlockquoteRegex_10 : Regex
{
	private sealed class RunnerFactory : RegexRunnerFactory
	{
		private sealed class Runner : RegexRunner
		{
			protected override void Scan(ReadOnlySpan<char> inputSpan)
			{
				while (TryFindNextPossibleStartingPosition(inputSpan) && !TryMatchAtCurrentPosition(inputSpan) && runtextpos != inputSpan.Length)
				{
					runtextpos++;
					CheckTimeout();
				}
			}

			private bool TryFindNextPossibleStartingPosition(ReadOnlySpan<char> inputSpan)
			{
				int num = runtextpos;
				if ((uint)num < (uint)inputSpan.Length)
				{
					if (num <= 0 || inputSpan[num - 1] == '\n')
					{
						goto IL_0054;
					}
					int num2 = inputSpan.Slice(num).IndexOf('\n');
					if ((uint)num2 <= inputSpan.Length - num - 1)
					{
						num += num2 + 1;
						if (num < inputSpan.Length)
						{
							goto IL_0054;
						}
					}
				}
				goto IL_0073;
				IL_0054:
				int num3 = inputSpan.Slice(num).IndexOf('>');
				if (num3 >= 0)
				{
					runtextpos = num + num3;
					return true;
				}
				goto IL_0073;
				IL_0073:
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
				int num6 = 0;
				int num7 = 0;
				int num8 = 0;
				ReadOnlySpan<char> span = inputSpan.Slice(num);
				if (num > 0 && inputSpan[num - 1] != '\n')
				{
					UncaptureUntil(0);
					return false;
				}
				if (span.IsEmpty || span[0] != '>')
				{
					UncaptureUntil(0);
					return false;
				}
				num++;
				span = inputSpan.Slice(num);
				num5 = num;
				if (!span.IsEmpty && char.IsWhiteSpace(span[0]))
				{
					span = span.Slice(1);
					num++;
				}
				num6 = num;
				while (true)
				{
					num3 = Crawlpos();
					num2 = num;
					num7 = num;
					int num9 = span.IndexOf('\n');
					if (num9 < 0)
					{
						num9 = span.Length;
					}
					span = span.Slice(num9);
					num += num9;
					num8 = num;
					while (true)
					{
						num4 = Crawlpos();
						Capture(1, num2, num);
						if ((uint)num < (uint)inputSpan.Length && inputSpan[num] != '\n')
						{
							UncaptureUntil(num4);
							CheckTimeout();
							if (num7 >= num8)
							{
								break;
							}
							num = --num8;
							span = inputSpan.Slice(num);
							continue;
						}
						runtextpos = num;
						Capture(0, start, num);
						return true;
					}
					UncaptureUntil(num3);
					CheckTimeout();
					if (num5 >= num6)
					{
						break;
					}
					num = --num6;
					span = inputSpan.Slice(num);
				}
				UncaptureUntil(0);
				return false;
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

	internal static readonly _003CRegexGenerator_g_003EF5A919AF36546C3D797A1EDA78623F5FD69C6B1313DB9045DB2CC6C90915164F1__BlockquoteRegex_10 Instance = new _003CRegexGenerator_g_003EF5A919AF36546C3D797A1EDA78623F5FD69C6B1313DB9045DB2CC6C90915164F1__BlockquoteRegex_10();

	private _003CRegexGenerator_g_003EF5A919AF36546C3D797A1EDA78623F5FD69C6B1313DB9045DB2CC6C90915164F1__BlockquoteRegex_10()
	{
		pattern = "^>\\s?(.*)$";
		roptions = RegexOptions.Multiline;
		internalMatchTimeout = TimeSpan.FromMilliseconds(1000L);
		factory = new RunnerFactory();
		capsize = 2;
	}
}
