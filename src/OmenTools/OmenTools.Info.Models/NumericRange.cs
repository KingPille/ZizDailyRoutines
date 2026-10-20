using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace OmenTools.Info.Models;

public readonly record struct NumericRange<T> where T : INumber<T>
{
	public T Min { get; init; }

	public T Max { get; init; }

	public NumericRange(T min, T max)
	{
		if (min >= max)
		{
			throw new ArgumentOutOfRangeException("min", "最小值 (min) 必须小于等于最大值 (max)");
		}
		Min = min;
		Max = max;
	}

	public void Deconstruct(out T min, out T max)
	{
		min = Min;
		max = Max;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Contains(T? value)
	{
		if (value >= Min)
		{
			return value <= Max;
		}
		return false;
	}
}
public static class NumericRange
{
	public static NumericRange<ushort> UShortRange { get; } = new NumericRange<ushort>(0, ushort.MaxValue);

	public static NumericRange<short> ShortRange { get; } = new NumericRange<short>(short.MinValue, short.MaxValue);
}
