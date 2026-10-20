using System;
using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;
using w7E4CHT7aQZXxOifpLQ;

namespace AA2GHlTeHKUB9ArVZeZ;

[SkipLocalsInit]
[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.42308")]
internal sealed class vVWEW3TDrJdx8WHNH3d : Regex
{
	private sealed class c6ylgnlNmKcq4GpHhWf : RegexRunnerFactory
	{
		private sealed class pabtfqPCjOnlDmLCf4o : RegexRunner
		{
			protected override void Scan(ReadOnlySpan<char> inputSpan)
			{
				while (Cs6PjBxmW5(inputSpan) && !glyPOhkEy2(inputSpan) && runtextpos != inputSpan.Length)
				{
					runtextpos++;
					if (ERfxIeTNodlTTyRuyKx.s_hasTimeout)
					{
						CheckTimeout();
					}
				}
			}

			private bool Cs6PjBxmW5(ReadOnlySpan<char> inputSpan)
			{
				int num = runtextpos;
				if ((uint)num < (uint)inputSpan.Length)
				{
					int num2 = inputSpan.Slice(num).P6RTdYwINF();
					if (num2 >= 0)
					{
						runtextpos = num + num2;
						return true;
					}
				}
				runtextpos = inputSpan.Length;
				return false;
			}

			private bool glyPOhkEy2(ReadOnlySpan<char> inputSpan)
			{
				int num = runtextpos;
				int start = num;
				ReadOnlySpan<char> readOnlySpan = inputSpan.Slice(num);
				int i;
				for (i = 0; (uint)i < (uint)readOnlySpan.Length && char.IsDigit(readOnlySpan[i]); i++)
				{
				}
				if (i == 0)
				{
					return false;
				}
				readOnlySpan = readOnlySpan.Slice(i);
				Capture(0, start, runtextpos = num + i);
				return true;
			}

			public pabtfqPCjOnlDmLCf4o()
			{
				UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
				base._002Ector();
			}
		}

		protected override RegexRunner CreateInstance()
		{
			return new pabtfqPCjOnlDmLCf4o();
		}

		public c6ylgnlNmKcq4GpHhWf()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}
	}

	internal static readonly vVWEW3TDrJdx8WHNH3d Instance;

	private vVWEW3TDrJdx8WHNH3d()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		base._002Ector();
		pattern = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x9B4A9CE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2f1c0cb46b4440d499a69ed8afe22360);
		roptions = RegexOptions.None;
		Regex.ValidateMatchTimeout(ERfxIeTNodlTTyRuyKx.s_defaultTimeout);
		internalMatchTimeout = ERfxIeTNodlTTyRuyKx.s_defaultTimeout;
		factory = new c6ylgnlNmKcq4GpHhWf();
		capsize = 1;
	}

	static vVWEW3TDrJdx8WHNH3d()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Instance = new vVWEW3TDrJdx8WHNH3d();
	}
}
