using System;
using System.Collections.Generic;
using Lumina.Excel;

namespace OmenTools.Interop.Game.Lumina;

public class RandomLuminaSelector<T, TRet> where T : struct, IExcelRow<T>
{
	private readonly Func<T, TRet> getterFunc;

	private readonly List<uint> validRowIDs = new List<uint>();

	public RandomLuminaSelector(T[] items, Func<T, TRet> getter)
	{
		ArgumentNullException.ThrowIfNull(items, "items");
		ArgumentNullException.ThrowIfNull(getter, "getter");
		for (int i = 0; i < items.Length; i++)
		{
			T val = items[i];
			validRowIDs.Add(((IExcelRow<T>)val).RowId);
		}
		getterFunc = getter;
	}

	public TRet Get()
	{
		return getterFunc(LuminaGetter.GetRowOrDefault<T>((uint)Random.Shared.Next(validRowIDs.Count - 1)));
	}
}
