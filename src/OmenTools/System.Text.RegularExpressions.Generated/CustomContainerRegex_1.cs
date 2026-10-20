using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;

namespace System.Text.RegularExpressions.Generated;

[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.42308")]
[SkipLocalsInit]
internal sealed class _003CRegexGenerator_g_003EF5A919AF36546C3D797A1EDA78623F5FD69C6B1313DB9045DB2CC6C90915164F1__CustomContainerRegex_1 : Regex
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
				if (num <= inputSpan.Length - 3)
				{
					int num2 = inputSpan.Slice(num).IndexOfAny(_003CRegexGenerator_g_003EF5A919AF36546C3D797A1EDA78623F5FD69C6B1313DB9045DB2CC6C90915164F1__Utilities.s_indexOfString_52DEF0D01D1326103BE9ADCB1EF8B93027F9650D81DEDE18C4282E10F9244AD6);
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
				int pos = 0;
				int num3 = 0;
				ReadOnlySpan<char> span = inputSpan.Slice(num);
				if (!span.StartsWith(":::".AsSpan()))
				{
					return false;
				}
				num += 3;
				span = inputSpan.Slice(num);
				num3 = pos;
				num2 = 0;
				do
				{
					_003CRegexGenerator_g_003EF5A919AF36546C3D797A1EDA78623F5FD69C6B1313DB9045DB2CC6C90915164F1__Utilities.StackPush(ref runstack, ref pos, num);
					num2++;
					int num4 = span.IndexOf('\n');
					if (num4 < 0)
					{
						num4 = span.Length;
					}
					span = span.Slice(num4);
					num += num4;
					if (!span.IsEmpty && span[0] == '\n')
					{
						num++;
						span = inputSpan.Slice(num);
						continue;
					}
					if (--num2 < 0)
					{
						return false;
					}
					num = runstack[--pos];
					span = inputSpan.Slice(num);
					break;
				}
				while (num2 == 0);
				pos = num3;
				runtextpos = num;
				Capture(0, start, num);
				return true;
			}
		}

		protected override RegexRunner CreateInstance()
		{
			return new Runner();
		}
	}

	internal static readonly _003CRegexGenerator_g_003EF5A919AF36546C3D797A1EDA78623F5FD69C6B1313DB9045DB2CC6C90915164F1__CustomContainerRegex_1 Instance = new _003CRegexGenerator_g_003EF5A919AF36546C3D797A1EDA78623F5FD69C6B1313DB9045DB2CC6C90915164F1__CustomContainerRegex_1();

	private _003CRegexGenerator_g_003EF5A919AF36546C3D797A1EDA78623F5FD69C6B1313DB9045DB2CC6C90915164F1__CustomContainerRegex_1()
	{
		pattern = ":::[^\\n]*\\n|:::|:::\\s*$";
		roptions = RegexOptions.Multiline;
		internalMatchTimeout = TimeSpan.FromMilliseconds(1000L);
		factory = new RunnerFactory();
		capsize = 1;
	}
}
