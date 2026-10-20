using System;
using System.Buffers;
using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace w7E4CHT7aQZXxOifpLQ;

[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.42308")]
internal static class ERfxIeTNodlTTyRuyKx
{
	internal static readonly TimeSpan s_defaultTimeout;

	internal static readonly bool s_hasTimeout;

	internal static readonly SearchValues<char> s_asciiExceptDigits;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static int P6RTdYwINF(this ReadOnlySpan<char> span)
	{
		int num = span.IndexOfAnyExcept(s_asciiExceptDigits);
		if ((uint)num < (uint)span.Length)
		{
			if (char.IsAscii(span[num]))
			{
				return num;
			}
			do
			{
				if (char.IsDigit(span[num]))
				{
					return num;
				}
				num++;
			}
			while ((uint)num < (uint)span.Length);
		}
		return -1;
	}

	static ERfxIeTNodlTTyRuyKx()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		s_defaultTimeout = ((AppContext.GetData(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x27D1BBE5 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_39d5d8025f5249e6b489bc374eefbb3c)) is TimeSpan timeSpan) ? timeSpan : Regex.InfiniteMatchTimeout);
		s_hasTimeout = s_defaultTimeout != Regex.InfiniteMatchTimeout;
		s_asciiExceptDigits = SearchValues.Create(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x36951853 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_26e231ba68f74e8784503e099ddfc2d9).AsSpan());
	}
}
