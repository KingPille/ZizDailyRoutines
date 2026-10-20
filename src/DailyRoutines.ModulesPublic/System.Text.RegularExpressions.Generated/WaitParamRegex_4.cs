using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;

namespace System.Text.RegularExpressions.Generated;

[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.42308")]
[SkipLocalsInit]
internal sealed class _003CRegexGenerator_g_003EFDD7C65F98B3009022F7AC561E347A370A49D71D176A01AA07C89E66F21EBD08F__WaitParamRegex_4 : Regex
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
				if (num <= inputSpan.Length - 8 && num == 0)
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
				int num6 = 0;
				int num7 = 0;
				int num8 = 0;
				ReadOnlySpan<char> span = inputSpan.Slice(num);
				if (num != 0)
				{
					UncaptureUntil(0);
					return false;
				}
				num2 = num;
				num8 = num;
				while (true)
				{
					num7 = Crawlpos();
					Capture(1, num2, num);
					char c;
					if ((uint)span.Length >= 6u && span.get_Item(0) == '<' && span.Slice(1).StartsWith("wa".AsSpan(), StringComparison.OrdinalIgnoreCase) && !(((c = span.get_Item(3)) | 0x20) != 105 && c != 'İ') && span.Slice(4).StartsWith("t.".AsSpan(), StringComparison.OrdinalIgnoreCase))
					{
						num += 6;
						span = inputSpan.Slice(num);
						num3 = num;
						int i;
						for (i = 0; (uint)i < (uint)span.Length && char.IsDigit(span.get_Item(i)); i++)
						{
						}
						if (i != 0)
						{
							span = span.Slice(i);
							num += i;
							Capture(2, num3, num);
							if (!span.IsEmpty && span.get_Item(0) == '>')
							{
								num++;
								span = inputSpan.Slice(num);
								num5 = num;
								int j;
								for (j = 0; (uint)j < (uint)span.Length && char.IsWhiteSpace(span.get_Item(j)); j++)
								{
								}
								span = span.Slice(j);
								num += j;
								num6 = num;
								while (true)
								{
									num4 = Crawlpos();
									if (num < inputSpan.Length - 1 || ((uint)num < (uint)inputSpan.Length && inputSpan.get_Item(num) != '\n'))
									{
										UncaptureUntil(num4);
										if (_003CRegexGenerator_g_003EFDD7C65F98B3009022F7AC561E347A370A49D71D176A01AA07C89E66F21EBD08F__Utilities.s_hasTimeout)
										{
											CheckTimeout();
										}
										if (num5 >= num6)
										{
											break;
										}
										num = --num6;
										span = inputSpan.Slice(num);
										continue;
									}
									runtextpos = num;
									Capture(0, start, num);
									return true;
								}
							}
						}
					}
					UncaptureUntil(num7);
					if (_003CRegexGenerator_g_003EFDD7C65F98B3009022F7AC561E347A370A49D71D176A01AA07C89E66F21EBD08F__Utilities.s_hasTimeout)
					{
						CheckTimeout();
					}
					num = num8;
					span = inputSpan.Slice(num);
					if (span.IsEmpty || span.get_Item(0) == '\n')
					{
						UncaptureUntil(0);
						return false;
					}
					num++;
					span = inputSpan.Slice(num);
					num8 = span.IndexOfAny('\n', '<');
					if ((uint)num8 >= (uint)span.Length || span.get_Item(num8) == '\n')
					{
						break;
					}
					num += num8;
					span = inputSpan.Slice(num);
					num8 = num;
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

	internal static readonly _003CRegexGenerator_g_003EFDD7C65F98B3009022F7AC561E347A370A49D71D176A01AA07C89E66F21EBD08F__WaitParamRegex_4 Instance = new _003CRegexGenerator_g_003EFDD7C65F98B3009022F7AC561E347A370A49D71D176A01AA07C89E66F21EBD08F__WaitParamRegex_4();

	private _003CRegexGenerator_g_003EFDD7C65F98B3009022F7AC561E347A370A49D71D176A01AA07C89E66F21EBD08F__WaitParamRegex_4()
	{
		pattern = "^(.*?)<wait\\.(\\d+)>\\s*$";
		roptions = RegexOptions.IgnoreCase;
		Regex.ValidateMatchTimeout(_003CRegexGenerator_g_003EFDD7C65F98B3009022F7AC561E347A370A49D71D176A01AA07C89E66F21EBD08F__Utilities.s_defaultTimeout);
		internalMatchTimeout = _003CRegexGenerator_g_003EFDD7C65F98B3009022F7AC561E347A370A49D71D176A01AA07C89E66F21EBD08F__Utilities.s_defaultTimeout;
		factory = new RunnerFactory();
		capsize = 3;
	}
}
