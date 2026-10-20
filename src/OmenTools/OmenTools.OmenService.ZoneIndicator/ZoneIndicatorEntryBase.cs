using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace OmenTools.OmenService.ZoneIndicator;

internal abstract class ZoneIndicatorEntryBase : IZoneIndicatorMutable
{
	[CompilerGenerated]
	private uint? _003CRenderRadius_003Ek__BackingField;

	public ulong ID { get; }

	public uint TerritoryType { get; }

	public bool IsPermanent { get; }

	public uint? RenderRadius
	{
		[CompilerGenerated]
		get
		{
			return _003CRenderRadius_003Ek__BackingField;
		}
		set
		{
			_003CRenderRadius_003Ek__BackingField = value;
			float? renderRadiusSquared;
			if (value.HasValue)
			{
				uint valueOrDefault = value.GetValueOrDefault();
				renderRadiusSquared = (float)valueOrDefault * (float)valueOrDefault;
			}
			else
			{
				renderRadiusSquared = null;
			}
			RenderRadiusSquared = renderRadiusSquared;
		}
	}

	public float? RenderRadiusSquared { get; private set; }

	public bool HiddenWhenBlocked { get; set; }

	public bool HideTextLabel { get; set; }

	public ZoneIndicatorSurrounding? Surrounding { get; set; }

	protected ZoneIndicatorEntryBase(ulong id, uint territoryType, bool isPermanent, ZoneIndicatorOptions options)
	{
		ID = id;
		TerritoryType = territoryType;
		IsPermanent = isPermanent;
		RenderRadius = options.RenderRadius;
		HiddenWhenBlocked = options.HiddenWhenBlocked;
		HideTextLabel = options.HideTextLabel;
		Surrounding = options.Surrounding;
	}

	internal abstract IEnumerable<ResolvedTarget> ResolveTargets();
}
