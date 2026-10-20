using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;

namespace System.Text.RegularExpressions.Generated;

[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.42308")]
[SkipLocalsInit]
internal sealed class _003CRegexGenerator_g_003EF5A919AF36546C3D797A1EDA78623F5FD69C6B1313DB9045DB2CC6C90915164F1__ListRegex_11 : Regex
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
				if (num <= inputSpan.Length - 2)
				{
					if (num <= 0 || inputSpan[num - 1] == '\n')
					{
						goto IL_0058;
					}
					int num2 = inputSpan.Slice(num).IndexOf('\n');
					if ((uint)num2 <= inputSpan.Length - num - 1)
					{
						num += num2 + 1;
						if (num <= inputSpan.Length - 2)
						{
							goto IL_0058;
						}
					}
				}
				goto IL_0075;
				IL_0058:
				int num3 = inputSpan.Slice(num).IndexOfNonAsciiOrAny_6479C023D753664A17E26CF70D50E1D2DDA2FE84CA33DF23BA4D362FEB3DD5C3();
				if (num3 >= 0)
				{
					runtextpos = num + num3;
					return true;
				}
				goto IL_0075;
				IL_0075:
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
				int capturePosition = 0;
				int num7 = 0;
				int num8 = 0;
				int num9 = 0;
				int num10 = 0;
				int num11 = 0;
				int num12 = 0;
				int num13 = 0;
				int num14 = 0;
				ReadOnlySpan<char> span = inputSpan.Slice(num);
				if (num > 0 && inputSpan[num - 1] != '\n')
				{
					UncaptureUntil(0);
					return false;
				}
				int i;
				for (i = 0; (uint)i < (uint)span.Length && char.IsWhiteSpace(span[i]); i++)
				{
				}
				span = span.Slice(i);
				num += i;
				num5 = num;
				num4 = num;
				num3 = Crawlpos();
				char c;
				if (span.IsEmpty || (((c = span[0]) | 1) != 43 && c != '-'))
				{
					goto IL_00da;
				}
				num2 = 0;
				num++;
				span = inputSpan.Slice(num);
				goto IL_01d0;
				IL_01d0:
				while (true)
				{
					Capture(1, num5, num);
					num11 = num;
					int j;
					for (j = 0; (uint)j < (uint)span.Length && char.IsWhiteSpace(span[j]); j++)
					{
					}
					if (j != 0)
					{
						span = span.Slice(j);
						num += j;
						num12 = num;
						num11++;
						while (true)
						{
							num7 = Crawlpos();
							num6 = num;
							num13 = num;
							int num15 = span.IndexOf('\n');
							if (num15 < 0)
							{
								num15 = span.Length;
							}
							span = span.Slice(num15);
							num += num15;
							num14 = num;
							while (true)
							{
								num8 = Crawlpos();
								Capture(2, num6, num);
								if ((uint)num < (uint)inputSpan.Length && inputSpan[num] != '\n')
								{
									UncaptureUntil(num8);
									CheckTimeout();
									if (num13 >= num14)
									{
										break;
									}
									num = --num14;
									span = inputSpan.Slice(num);
									continue;
								}
								runtextpos = num;
								Capture(0, start, num);
								return true;
							}
							UncaptureUntil(num7);
							CheckTimeout();
							if (num11 >= num12)
							{
								break;
							}
							num = --num12;
							span = inputSpan.Slice(num);
						}
					}
					CheckTimeout();
					if (num2 == 0)
					{
						break;
					}
					if (num2 != 1)
					{
						continue;
					}
					goto IL_0141;
				}
				goto IL_00da;
				IL_018d:
				capturePosition = Crawlpos();
				if (span.IsEmpty || span[0] == '\n')
				{
					goto IL_0141;
				}
				num2 = 1;
				num++;
				span = inputSpan.Slice(num);
				goto IL_01d0;
				IL_0141:
				UncaptureUntil(capturePosition);
				CheckTimeout();
				if (num9 >= num10 || (num10 = inputSpan.Slice(num9, num10 - num9).LastIndexOfAnyExcept('\n')) < 0)
				{
					UncaptureUntil(0);
					return false;
				}
				num10 += num9;
				num = num10;
				span = inputSpan.Slice(num);
				goto IL_018d;
				IL_00da:
				num = num4;
				span = inputSpan.Slice(num);
				UncaptureUntil(num3);
				num9 = num;
				int k;
				for (k = 0; (uint)k < (uint)span.Length && char.IsDigit(span[k]); k++)
				{
				}
				if (k == 0)
				{
					UncaptureUntil(0);
					return false;
				}
				span = span.Slice(k);
				num += k;
				num10 = num;
				num9++;
				goto IL_018d;
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				void UncaptureUntil(int num16)
				{
					while (Crawlpos() > num16)
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

	internal static readonly _003CRegexGenerator_g_003EF5A919AF36546C3D797A1EDA78623F5FD69C6B1313DB9045DB2CC6C90915164F1__ListRegex_11 Instance = new _003CRegexGenerator_g_003EF5A919AF36546C3D797A1EDA78623F5FD69C6B1313DB9045DB2CC6C90915164F1__ListRegex_11();

	private _003CRegexGenerator_g_003EF5A919AF36546C3D797A1EDA78623F5FD69C6B1313DB9045DB2CC6C90915164F1__ListRegex_11()
	{
		pattern = "^[\\s]*([-*+]|\\d+.)\\s+(.*)$";
		roptions = RegexOptions.Multiline;
		internalMatchTimeout = TimeSpan.FromMilliseconds(1000L);
		factory = new RunnerFactory();
		capsize = 3;
	}
}
