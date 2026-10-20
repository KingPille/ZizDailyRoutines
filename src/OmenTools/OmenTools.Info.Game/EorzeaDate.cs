using System;
using System.Runtime.CompilerServices;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.System.Framework;

namespace OmenTools.Info.Game;

public class EorzeaDate(int y, int m, int d, int h, int min, int sec, long timestamp)
{
	public int Year { get; set; } = y;

	public int Month { get; set; } = m;

	public int Day { get; set; } = d;

	public int Hour { get; set; } = h;

	public int Minute { get; set; } = min;

	public int Second { get; set; } = sec;

	public long EorzeaTimeStamp { get; set; } = timestamp;

	public override string ToString()
	{
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(5, 6);
		defaultInterpolatedStringHandler.AppendFormatted(Year);
		defaultInterpolatedStringHandler.AppendLiteral("/");
		defaultInterpolatedStringHandler.AppendFormatted(Month);
		defaultInterpolatedStringHandler.AppendLiteral("/");
		defaultInterpolatedStringHandler.AppendFormatted(Day);
		defaultInterpolatedStringHandler.AppendLiteral(" ");
		defaultInterpolatedStringHandler.AppendFormatted((Hour < 10) ? ("0" + Hour) : ((object)Hour));
		defaultInterpolatedStringHandler.AppendLiteral(":");
		defaultInterpolatedStringHandler.AppendFormatted((Minute < 10) ? ("0" + Minute) : ((object)Minute));
		defaultInterpolatedStringHandler.AppendLiteral(":");
		defaultInterpolatedStringHandler.AppendFormatted((Second < 10) ? ("0" + Second) : ((object)Second));
		return defaultInterpolatedStringHandler.ToStringAndClear();
	}

	public string ToShortString()
	{
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(2, 3);
		defaultInterpolatedStringHandler.AppendFormatted((Hour < 10) ? ("0" + Hour) : ((object)Hour));
		defaultInterpolatedStringHandler.AppendLiteral(":");
		defaultInterpolatedStringHandler.AppendFormatted((Minute < 10) ? ("0" + Minute) : ((object)Minute));
		defaultInterpolatedStringHandler.AppendLiteral(":");
		defaultInterpolatedStringHandler.AppendFormatted((Second < 10) ? ("0" + Second) : ((object)Second));
		return defaultInterpolatedStringHandler.ToStringAndClear();
	}

	public static EorzeaDate GetTime(long? timeStamp = null)
	{
		long num = Convert.ToInt64(Math.Round(Convert.ToDecimal(((timeStamp ?? Framework.GetServerTime()) - 1278950400) * 144 / 7), 0));
		num = Convert.ToInt64(Math.Round((double)(num / 10), 0)) * 10;
		int y = Convert.ToInt32(Math.Floor((decimal)(num / 33177600)));
		int m = Convert.ToInt32(Math.Floor((decimal)(num % 33177600 / 2764800))) + 1;
		int d = Convert.ToInt32(Math.Floor((decimal)(num % 2764800) / 86400m)) + 1;
		int h = Convert.ToInt32(Math.Floor((decimal)(num % 86400) / 3600m));
		int min = Convert.ToInt32(Math.Floor((decimal)(num % 3600) / 60m));
		int sec = Convert.ToInt32(num % 60);
		return new EorzeaDate(y, m, d, h, min, sec, num);
	}

	public unsafe static uint GetWeather(uint? zoneID = null)
	{
		if (zoneID.HasValue)
		{
			return ((WeatherManager)WeatherManager.Instance()).GetWeatherForDaytime((ushort)zoneID.Value, 0);
		}
		return ((WeatherManager)WeatherManager.Instance()).GetCurrentWeather();
	}
}
