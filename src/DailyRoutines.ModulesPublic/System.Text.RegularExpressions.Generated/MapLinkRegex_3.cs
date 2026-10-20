using System.CodeDom.Compiler;
using System.Collections;
using System.Runtime.CompilerServices;

namespace System.Text.RegularExpressions.Generated;

[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.42308")]
[SkipLocalsInit]
internal sealed class _003CRegexGenerator_g_003EFDD7C65F98B3009022F7AC561E347A370A49D71D176A01AA07C89E66F21EBD08F__MapLinkRegex_3 : Regex
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
				if (num <= inputSpan.Length - 17)
				{
					int num2 = inputSpan.Slice(num).IndexOf('\ue0bb');
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
				int pos = 0;
				int num8 = 0;
				ReadOnlySpan<char> span = inputSpan.Slice(num);
				if (span.IsEmpty || span.get_Item(0) != '\ue0bb')
				{
					UncaptureUntil(0);
					return false;
				}
				num++;
				span = inputSpan.Slice(num);
				num2 = num;
				if (span.IsEmpty || span.get_Item(0) == '\n')
				{
					UncaptureUntil(0);
					return false;
				}
				num++;
				span = inputSpan.Slice(num);
				num6 = num;
				while (true)
				{
					num5 = Crawlpos();
					Capture(1, num2, num);
					num8 = pos;
					num7 = 0;
					while (true)
					{
						_003CRegexGenerator_g_003EFDD7C65F98B3009022F7AC561E347A370A49D71D176A01AA07C89E66F21EBD08F__Utilities.StackPush(ref runstack, ref pos, Crawlpos(), num);
						num7++;
						int start2 = num;
						if (!span.IsEmpty && char.IsBetween(span.get_Item(0), '\ue0b1', '\ue0b9'))
						{
							num++;
							span = inputSpan.Slice(num);
							Capture(2, start2, num);
							if (num7 == 0)
							{
								continue;
							}
						}
						else
						{
							if (--num7 < 0)
							{
								break;
							}
							num = runstack[--pos];
							UncaptureUntil(runstack[--pos]);
							span = inputSpan.Slice(num);
						}
						pos = num8;
						if (!span.StartsWith(" ( ".AsSpan()))
						{
							break;
						}
						num += 3;
						span = inputSpan.Slice(num);
						num3 = num;
						int i;
						for (i = 0; i < 2 && (uint)i < (uint)span.Length && char.IsDigit(span.get_Item(i)); i++)
						{
						}
						if (i == 0)
						{
							break;
						}
						span = span.Slice(i);
						num += i;
						if ((uint)span.Length < 2u || span.get_Item(0) != '.' || !char.IsDigit(span.get_Item(1)))
						{
							break;
						}
						num += 2;
						span = inputSpan.Slice(num);
						Capture(3, num3, num);
						if (!span.StartsWith("  , ".AsSpan()))
						{
							break;
						}
						num += 4;
						span = inputSpan.Slice(num);
						num4 = num;
						int j;
						for (j = 0; j < 2 && (uint)j < (uint)span.Length && char.IsDigit(span.get_Item(j)); j++)
						{
						}
						if (j == 0)
						{
							break;
						}
						span = span.Slice(j);
						num += j;
						if ((uint)span.Length < 2u || span.get_Item(0) != '.' || !char.IsDigit(span.get_Item(1)))
						{
							break;
						}
						num += 2;
						span = inputSpan.Slice(num);
						Capture(4, num4, num);
						if (!span.StartsWith(" )".AsSpan()))
						{
							break;
						}
						Capture(0, start, runtextpos = num + 2);
						return true;
					}
					UncaptureUntil(num5);
					if (_003CRegexGenerator_g_003EFDD7C65F98B3009022F7AC561E347A370A49D71D176A01AA07C89E66F21EBD08F__Utilities.s_hasTimeout)
					{
						CheckTimeout();
					}
					num = num6;
					span = inputSpan.Slice(num);
					if (span.IsEmpty || span.get_Item(0) == '\n')
					{
						break;
					}
					num++;
					span = inputSpan.Slice(num);
					num6 = num;
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

	internal static readonly _003CRegexGenerator_g_003EFDD7C65F98B3009022F7AC561E347A370A49D71D176A01AA07C89E66F21EBD08F__MapLinkRegex_3 Instance = new _003CRegexGenerator_g_003EFDD7C65F98B3009022F7AC561E347A370A49D71D176A01AA07C89E66F21EBD08F__MapLinkRegex_3();

	private _003CRegexGenerator_g_003EFDD7C65F98B3009022F7AC561E347A370A49D71D176A01AA07C89E66F21EBD08F__MapLinkRegex_3()
	{
		pattern = "\\uE0BB(?<map>.+?)(?<instance>[\\ue0b1-\\ue0b9])? \\( (?<x>\\d{1,2}\\.\\d)  , (?<y>\\d{1,2}\\.\\d) \\)";
		roptions = RegexOptions.Compiled;
		Regex.ValidateMatchTimeout(_003CRegexGenerator_g_003EFDD7C65F98B3009022F7AC561E347A370A49D71D176A01AA07C89E66F21EBD08F__Utilities.s_defaultTimeout);
		internalMatchTimeout = _003CRegexGenerator_g_003EFDD7C65F98B3009022F7AC561E347A370A49D71D176A01AA07C89E66F21EBD08F__Utilities.s_defaultTimeout;
		factory = new RunnerFactory();
		base.CapNames = new Hashtable
		{
			{ "0", 0 },
			{ "instance", 2 },
			{ "map", 1 },
			{ "x", 3 },
			{ "y", 4 }
		};
		capslist = new string[5] { "0", "map", "instance", "x", "y" };
		capsize = 5;
	}
}
