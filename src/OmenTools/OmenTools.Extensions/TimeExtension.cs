using System;
using System.Numerics;
using System.Timers;

namespace OmenTools.Extensions;

public static class TimeExtension
{
	extension(Timer timer)
	{
		public void Restart()
		{
			timer.Stop();
			timer.Start();
		}
	}

	extension<T>(T unixTimestamp) where T : IBinaryInteger<_0024T0>
	{
		public DateTime ToUTCDateTimeFromUnixSeconds()
		{
			return new DateTime(long.CreateTruncating(unixTimestamp) * 10000000 + 621355968000000000L, DateTimeKind.Utc);
		}
	}
}
