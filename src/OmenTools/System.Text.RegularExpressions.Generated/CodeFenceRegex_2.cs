using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;

namespace System.Text.RegularExpressions.Generated;

[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.42308")]
[SkipLocalsInit]
internal sealed class _003CRegexGenerator_g_003EF5A919AF36546C3D797A1EDA78623F5FD69C6B1313DB9045DB2CC6C90915164F1__CodeFenceRegex_2 : Regex
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
					int num2 = inputSpan.Slice(num).IndexOfAny(_003CRegexGenerator_g_003EF5A919AF36546C3D797A1EDA78623F5FD69C6B1313DB9045DB2CC6C90915164F1__Utilities.s_indexOfString_ACD1AF3308000737329946CAB3D500C3329228244FA0C355223CB35C8E9ACBE9);
					if (num2 >= 0)
					{
						runtextpos = num + num2;
						return true;
					}
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
				int num6 = 0;
				int num7 = 0;
				int num8 = 0;
				int num9 = 0;
				int num10 = 0;
				int num11 = 0;
				int num12 = 0;
				int num13 = 0;
				ReadOnlySpan<char> span = inputSpan.Slice(num);
				num6 = num;
				int num14 = span.IndexOfAnyExcept('`');
				if (num14 < 0)
				{
					num14 = span.Length;
				}
				if (num14 < 3)
				{
					UncaptureUntil(0);
					return false;
				}
				span = span.Slice(num14);
				num += num14;
				num7 = num;
				num6 += 3;
				while (true)
				{
					num3 = Crawlpos();
					if (runtextpos < num)
					{
						runtextpos = num;
					}
					num8 = num;
					int i;
					for (i = 0; (uint)i < (uint)span.Length && _003CRegexGenerator_g_003EF5A919AF36546C3D797A1EDA78623F5FD69C6B1313DB9045DB2CC6C90915164F1__Utilities.IsWordChar(span[i]); i++)
					{
					}
					span = span.Slice(i);
					num += i;
					num9 = num;
					while (true)
					{
						num4 = Crawlpos();
						num10 = num;
						int j;
						for (j = 0; (uint)j < (uint)span.Length && char.IsWhiteSpace(span[j]); j++)
						{
						}
						span = span.Slice(j);
						num += j;
						num11 = num;
						while (true)
						{
							num5 = Crawlpos();
							num2 = num;
							num13 = num;
							while (true)
							{
								num12 = Crawlpos();
								Capture(1, num2, num);
								int k;
								for (k = 0; (uint)k < (uint)span.Length && char.IsWhiteSpace(span[k]); k++)
								{
								}
								span = span.Slice(k);
								num += k;
								int num15 = span.IndexOfAnyExcept('`');
								if (num15 < 0)
								{
									num15 = span.Length;
								}
								if (num15 < 3)
								{
									UncaptureUntil(num12);
									CheckTimeout();
									num = num13;
									span = inputSpan.Slice(num);
									if (span.IsEmpty)
									{
										break;
									}
									num++;
									span = inputSpan.Slice(num);
									num13 = num;
									continue;
								}
								span = span.Slice(num15);
								Capture(0, start, runtextpos = num + num15);
								return true;
							}
							UncaptureUntil(num5);
							CheckTimeout();
							if (num10 >= num11)
							{
								break;
							}
							num = --num11;
							span = inputSpan.Slice(num);
						}
						UncaptureUntil(num4);
						CheckTimeout();
						if (num8 >= num9)
						{
							break;
						}
						num = --num9;
						span = inputSpan.Slice(num);
					}
					UncaptureUntil(num3);
					CheckTimeout();
					if (num6 >= num7)
					{
						break;
					}
					num = --num7;
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

	internal static readonly _003CRegexGenerator_g_003EF5A919AF36546C3D797A1EDA78623F5FD69C6B1313DB9045DB2CC6C90915164F1__CodeFenceRegex_2 Instance = new _003CRegexGenerator_g_003EF5A919AF36546C3D797A1EDA78623F5FD69C6B1313DB9045DB2CC6C90915164F1__CodeFenceRegex_2();

	private _003CRegexGenerator_g_003EF5A919AF36546C3D797A1EDA78623F5FD69C6B1313DB9045DB2CC6C90915164F1__CodeFenceRegex_2()
	{
		pattern = "`{3,}\\w*\\s*([\\s\\S]*?)\\s*`{3,}";
		roptions = RegexOptions.None;
		internalMatchTimeout = TimeSpan.FromMilliseconds(1000L);
		factory = new RunnerFactory();
		capsize = 2;
	}
}
