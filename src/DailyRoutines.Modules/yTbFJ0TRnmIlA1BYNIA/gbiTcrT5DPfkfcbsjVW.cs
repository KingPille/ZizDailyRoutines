using System;
using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;
using w7E4CHT7aQZXxOifpLQ;

namespace yTbFJ0TRnmIlA1BYNIA;

[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.42308")]
[SkipLocalsInit]
internal sealed class gbiTcrT5DPfkfcbsjVW : Regex
{
	private sealed class heFNTMlet1xiSSDftSs : RegexRunnerFactory
	{
		private sealed class iH8T0IPSHvmYyWLfr6B : RegexRunner
		{
			protected override void Scan(ReadOnlySpan<char> inputSpan)
			{
				while (LsVPVDrC4t(inputSpan) && !tywP9qrRq6(inputSpan) && runtextpos != inputSpan.Length)
				{
					runtextpos++;
					if (ERfxIeTNodlTTyRuyKx.s_hasTimeout)
					{
						CheckTimeout();
					}
				}
			}

			private bool LsVPVDrC4t(ReadOnlySpan<char> inputSpan)
			{
				int num = runtextpos;
				if (num <= inputSpan.Length - 2)
				{
					while (true)
					{
						ReadOnlySpan<char> span = inputSpan.Slice(num);
						int num2 = span.IndexOf('%');
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

			private bool tywP9qrRq6(ReadOnlySpan<char> inputSpan)
			{
				int num = runtextpos;
				int start = num;
				int num2 = 0;
				ReadOnlySpan<char> readOnlySpan = inputSpan.Slice(num);
				num2 = num;
				num = runtrackpos;
				readOnlySpan = inputSpan.Slice(num);
				Capture(1, num2, num);
				if (readOnlySpan.IsEmpty || readOnlySpan[0] != '%')
				{
					itaPfcgEjb(0);
					return false;
				}
				Capture(0, start, runtextpos = num + 1);
				return true;
			}

			public iH8T0IPSHvmYyWLfr6B()
			{
				UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
				base._002Ector();
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			[CompilerGenerated]
			private void itaPfcgEjb(int capturePosition)
			{
				while (Crawlpos() > capturePosition)
				{
					Uncapture();
				}
			}
		}

		protected override RegexRunner CreateInstance()
		{
			return new iH8T0IPSHvmYyWLfr6B();
		}

		public heFNTMlet1xiSSDftSs()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}
	}

	internal static readonly gbiTcrT5DPfkfcbsjVW Instance;

	private gbiTcrT5DPfkfcbsjVW()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		base._002Ector();
		pattern = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3D568174 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e9e296e241f44828a57c750102ce71c1);
		roptions = RegexOptions.None;
		Regex.ValidateMatchTimeout(ERfxIeTNodlTTyRuyKx.s_defaultTimeout);
		internalMatchTimeout = ERfxIeTNodlTTyRuyKx.s_defaultTimeout;
		factory = new heFNTMlet1xiSSDftSs();
		capsize = 2;
	}

	static gbiTcrT5DPfkfcbsjVW()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Instance = new gbiTcrT5DPfkfcbsjVW();
	}
}
