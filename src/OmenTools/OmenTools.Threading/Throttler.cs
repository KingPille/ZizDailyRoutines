using System;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace OmenTools.Threading;

public class Throttler
{
	internal static readonly double TicksPerMillisecond = (double)Stopwatch.Frequency / 1000.0;

	public static Throttler<string> Shared { get; } = new Throttler<string>();
}
public class Throttler<T> : Throttler where T : notnull
{
	private readonly ConcurrentDictionary<T, long> throttlers = new ConcurrentDictionary<T, long>();

	protected static long GetCurrentTimestamp()
	{
		return Stopwatch.GetTimestamp();
	}

	public bool Throttle(T name, uint milliseconds = 500u, bool reThrottle = false)
	{
		return ThrottleInternal(name, (long)((double)milliseconds * Throttler.TicksPerMillisecond), reThrottle);
	}

	public bool Throttle(T name, TimeSpan duration, bool reThrottle = false)
	{
		return ThrottleInternal(name, (long)(duration.TotalSeconds * (double)Stopwatch.Frequency), reThrottle);
	}

	private bool ThrottleInternal(T name, long durationTicks, bool reThrottle)
	{
		long currentTimestamp = GetCurrentTimestamp();
		long num = currentTimestamp + durationTicks;
		while (true)
		{
			if (!throttlers.TryGetValue(name, out var value))
			{
				if (throttlers.TryAdd(name, num))
				{
					return true;
				}
				continue;
			}
			if (!reThrottle && currentTimestamp <= value)
			{
				return false;
			}
			if (throttlers.TryUpdate(name, num, value))
			{
				break;
			}
		}
		return true;
	}

	public bool Check(T name)
	{
		if (throttlers.TryGetValue(name, out var value))
		{
			return GetCurrentTimestamp() > value;
		}
		return true;
	}

	public bool Remove(T name)
	{
		long value;
		return throttlers.TryRemove(name, out value);
	}

	public void Clear()
	{
		throttlers.Clear();
	}
}
