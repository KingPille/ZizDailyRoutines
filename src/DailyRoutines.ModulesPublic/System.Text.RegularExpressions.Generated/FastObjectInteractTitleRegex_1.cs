using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;

namespace System.Text.RegularExpressions.Generated;

[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.42308")]
[SkipLocalsInit]
internal sealed class _003CRegexGenerator_g_003EFDD7C65F98B3009022F7AC561E347A370A49D71D176A01AA07C89E66F21EBD08F__FastObjectInteractTitleRegex_1 : Regex
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
					if (_003CRegexGenerator_g_003EFDD7C65F98B3009022F7AC561E347A370A49D71D176A01AA07C89E66F21EBD08F__Utilities.s_hasTimeout)
					{
						CheckTimeout();
					}
				}
			}

			private bool TryFindNextPossibleStartingPosition(ReadOnlySpan<char> inputSpan)
			{
				int num = runtextpos;
				if (num <= inputSpan.Length - 2)
				{
					int num2 = inputSpan.Slice(num).IndexOf('[');
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
				ReadOnlySpan<char> readOnlySpan = inputSpan.Slice(num);
				if (readOnlySpan.IsEmpty || readOnlySpan.get_Item(0) != '[')
				{
					return false;
				}
				num++;
				readOnlySpan = inputSpan.Slice(num);
				num2 = num;
				while (readOnlySpan.IsEmpty || readOnlySpan.get_Item(0) != ']')
				{
					if (_003CRegexGenerator_g_003EFDD7C65F98B3009022F7AC561E347A370A49D71D176A01AA07C89E66F21EBD08F__Utilities.s_hasTimeout)
					{
						CheckTimeout();
					}
					num = num2;
					readOnlySpan = inputSpan.Slice(num);
					if (readOnlySpan.IsEmpty || readOnlySpan.get_Item(0) == '\n')
					{
						return false;
					}
					num++;
					readOnlySpan = inputSpan.Slice(num);
					num2 = readOnlySpan.IndexOfAny('\n', ']');
					if ((uint)num2 >= (uint)readOnlySpan.Length || readOnlySpan.get_Item(num2) == '\n')
					{
						return false;
					}
					num += num2;
					readOnlySpan = inputSpan.Slice(num);
					num2 = num;
				}
				Capture(0, start, runtextpos = num + 1);
				return true;
			}
		}

		protected override RegexRunner CreateInstance()
		{
			return new Runner();
		}
	}

	internal static readonly _003CRegexGenerator_g_003EFDD7C65F98B3009022F7AC561E347A370A49D71D176A01AA07C89E66F21EBD08F__FastObjectInteractTitleRegex_1 Instance = new _003CRegexGenerator_g_003EFDD7C65F98B3009022F7AC561E347A370A49D71D176A01AA07C89E66F21EBD08F__FastObjectInteractTitleRegex_1();

	private _003CRegexGenerator_g_003EFDD7C65F98B3009022F7AC561E347A370A49D71D176A01AA07C89E66F21EBD08F__FastObjectInteractTitleRegex_1()
	{
		pattern = "\\[.*?\\]";
		roptions = RegexOptions.None;
		Regex.ValidateMatchTimeout(_003CRegexGenerator_g_003EFDD7C65F98B3009022F7AC561E347A370A49D71D176A01AA07C89E66F21EBD08F__Utilities.s_defaultTimeout);
		internalMatchTimeout = _003CRegexGenerator_g_003EFDD7C65F98B3009022F7AC561E347A370A49D71D176A01AA07C89E66F21EBD08F__Utilities.s_defaultTimeout;
		factory = new RunnerFactory();
		capsize = 1;
	}
}
