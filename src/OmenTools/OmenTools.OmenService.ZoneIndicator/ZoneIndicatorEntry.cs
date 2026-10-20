using System;
using System.Collections.Generic;
using System.Numerics;

namespace OmenTools.OmenService.ZoneIndicator;

internal sealed class ZoneIndicatorEntry<T>(ulong id, uint territoryType, bool isPermanent, Func<List<T>> sourceGetter, Func<T, Vector3> positionSelector, ZoneIndicatorOptions<T> options) : ZoneIndicatorEntryBase(id, territoryType, isPermanent, options)
{
	private readonly Func<T, ZoneIndicatorText>? textGetter = options.TextGetter;

	private readonly Action<ZoneIndicatorDrawContext<T>>? customDraw = options.CustomDraw;

	internal override IEnumerable<ResolvedTarget> ResolveTargets()
	{
		List<T> list = sourceGetter();
		if (list == null || list.Count <= 0)
		{
			yield break;
		}
		foreach (T item in list)
		{
			Vector3 position = positionSelector(item);
			ZoneIndicatorText text = textGetter?.Invoke(item);
			Action<ZoneIndicatorDrawContext> onDraw = null;
			if (customDraw != null)
			{
				T captured = item;
				onDraw = delegate(ZoneIndicatorDrawContext ctx)
				{
					customDraw(new ZoneIndicatorDrawContext<T>(ctx, captured));
				};
			}
			yield return new ResolvedTarget(position, text, onDraw);
		}
	}
}
