using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;

namespace System.Text.RegularExpressions.Generated;

[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.42308")]
[SkipLocalsInit]
internal sealed class _003CRegexGenerator_g_003EF5A919AF36546C3D797A1EDA78623F5FD69C6B1313DB9045DB2CC6C90915164F1__FrontMatterRegex_0 : Regex
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
				if (num <= inputSpan.Length - 6)
				{
					if (num <= 0 || inputSpan[num - 1] == '\n')
					{
						goto IL_0058;
					}
					int num2 = inputSpan.Slice(num).IndexOf('\n');
					if ((uint)num2 <= inputSpan.Length - num - 1)
					{
						num += num2 + 1;
						if (num <= inputSpan.Length - 6)
						{
							goto IL_0058;
						}
					}
				}
				goto IL_007a;
				IL_0058:
				int num3 = inputSpan.Slice(num).IndexOfAny(_003CRegexGenerator_g_003EF5A919AF36546C3D797A1EDA78623F5FD69C6B1313DB9045DB2CC6C90915164F1__Utilities.s_indexOfString_895EFF7AC52BDC70805F18A08EFB71D38DEDE24AD352A3744C3232E9150FF397);
				if (num3 >= 0)
				{
					runtextpos = num + num3;
					return true;
				}
				goto IL_007a;
				IL_007a:
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
				ReadOnlySpan<char> span = inputSpan.Slice(num);
				if (num > 0 && inputSpan[num - 1] != '\n')
				{
					return false;
				}
				if (!span.StartsWith("---".AsSpan()))
				{
					return false;
				}
				num += 3;
				span = inputSpan.Slice(num);
				num4 = num;
				while (true)
				{
					if (span.StartsWith("---".AsSpan()))
					{
						num += 3;
						span = inputSpan.Slice(num);
						num2 = num;
						int i;
						for (i = 0; (uint)i < (uint)span.Length && char.IsWhiteSpace(span[i]); i++)
						{
						}
						span = span.Slice(i);
						num += i;
						num3 = num;
						while (true)
						{
							if ((uint)num < (uint)inputSpan.Length && inputSpan[num] != '\n')
							{
								CheckTimeout();
								if (num2 >= num3)
								{
									break;
								}
								num = --num3;
								span = inputSpan.Slice(num);
								continue;
							}
							runtextpos = num;
							Capture(0, start, num);
							return true;
						}
					}
					CheckTimeout();
					num = num4;
					span = inputSpan.Slice(num);
					if (span.IsEmpty)
					{
						return false;
					}
					num++;
					span = inputSpan.Slice(num);
					num4 = span.IndexOf("---".AsSpan());
					if (num4 < 0)
					{
						break;
					}
					num += num4;
					span = inputSpan.Slice(num);
					num4 = num;
				}
				return false;
			}
		}

		protected override RegexRunner CreateInstance()
		{
			return new Runner();
		}
	}

	internal static readonly _003CRegexGenerator_g_003EF5A919AF36546C3D797A1EDA78623F5FD69C6B1313DB9045DB2CC6C90915164F1__FrontMatterRegex_0 Instance = new _003CRegexGenerator_g_003EF5A919AF36546C3D797A1EDA78623F5FD69C6B1313DB9045DB2CC6C90915164F1__FrontMatterRegex_0();

	private _003CRegexGenerator_g_003EF5A919AF36546C3D797A1EDA78623F5FD69C6B1313DB9045DB2CC6C90915164F1__FrontMatterRegex_0()
	{
		pattern = "^---[\\s\\S]*?---\\s*$";
		roptions = RegexOptions.Multiline;
		internalMatchTimeout = TimeSpan.FromMilliseconds(1000L);
		factory = new RunnerFactory();
		capsize = 1;
	}
}
