using System;
using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;
using w7E4CHT7aQZXxOifpLQ;

namespace lcOILHT0EGBgw0K6BhI;

[SkipLocalsInit]
[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.42308")]
internal sealed class ND9hdrTagCGnhqUV0JO : Regex
{
	private sealed class jaPCmflDuvhJme9n5YK : RegexRunnerFactory
	{
		private sealed class aiXb9VPnGNAwRniL26q : RegexRunner
		{
			protected override void Scan(ReadOnlySpan<char> inputSpan)
			{
				while (U1IPy2sHuk(inputSpan) && !heJPLSYdKv(inputSpan) && runtextpos != inputSpan.Length)
				{
					runtextpos++;
					if (ERfxIeTNodlTTyRuyKx.s_hasTimeout)
					{
						CheckTimeout();
					}
				}
			}

			private bool U1IPy2sHuk(ReadOnlySpan<char> inputSpan)
			{
				int num = runtextpos;
				if (num <= inputSpan.Length - 3)
				{
					while (true)
					{
						ReadOnlySpan<char> span = inputSpan.Slice(num);
						int num2 = span.IndexOf(':');
						if (num2 < 0)
						{
							break;
						}
						int num3 = num2 - 1;
						while ((uint)num3 < (uint)span.Length && char.IsDigit(span[num3]))
						{
							num3--;
						}
						if (num2 - num3 - 1 < 1)
						{
							num += num2 + 1;
							continue;
						}
						runtextpos = num + num3 + 1;
						runtrackpos = num + num2;
						return true;
					}
				}
				runtextpos = inputSpan.Length;
				return false;
			}

			private bool heJPLSYdKv(ReadOnlySpan<char> inputSpan)
			{
				int num = runtextpos;
				int start = num;
				int num2 = 0;
				int num3 = 0;
				ReadOnlySpan<char> readOnlySpan = inputSpan.Slice(num);
				num2 = num;
				num = runtrackpos;
				readOnlySpan = inputSpan.Slice(num);
				Capture(1, num2, num);
				if (readOnlySpan.IsEmpty || readOnlySpan[0] != ':')
				{
					ySpPHrZyiJ(0);
					return false;
				}
				num++;
				readOnlySpan = inputSpan.Slice(num);
				num3 = num;
				int i;
				for (i = 0; (uint)i < (uint)readOnlySpan.Length && char.IsDigit(readOnlySpan[i]); i++)
				{
				}
				if (i == 0)
				{
					ySpPHrZyiJ(0);
					return false;
				}
				readOnlySpan = readOnlySpan.Slice(i);
				num += i;
				Capture(2, num3, num);
				runtextpos = num;
				Capture(0, start, num);
				return true;
			}

			public aiXb9VPnGNAwRniL26q()
			{
				UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
				base._002Ector();
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			[CompilerGenerated]
			private void ySpPHrZyiJ(int capturePosition)
			{
				while (Crawlpos() > capturePosition)
				{
					Uncapture();
				}
			}
		}

		protected override RegexRunner CreateInstance()
		{
			return new aiXb9VPnGNAwRniL26q();
		}

		public jaPCmflDuvhJme9n5YK()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}
	}

	internal static readonly ND9hdrTagCGnhqUV0JO Instance;

	private ND9hdrTagCGnhqUV0JO()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		base._002Ector();
		pattern = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x12D655E2 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7fcf5a71a7ed445197f132979519c560);
		roptions = RegexOptions.None;
		Regex.ValidateMatchTimeout(ERfxIeTNodlTTyRuyKx.s_defaultTimeout);
		internalMatchTimeout = ERfxIeTNodlTTyRuyKx.s_defaultTimeout;
		factory = new jaPCmflDuvhJme9n5YK();
		capsize = 3;
	}

	static ND9hdrTagCGnhqUV0JO()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Instance = new ND9hdrTagCGnhqUV0JO();
	}
}
