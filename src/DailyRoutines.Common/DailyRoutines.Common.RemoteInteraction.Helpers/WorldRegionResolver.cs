using DailyRoutines.Common.RemoteInteraction.Enums;
using Lumina.Data;

namespace DailyRoutines.Common.RemoteInteraction.Helpers;

public static class WorldRegionResolver
{
	public static WorldRegion Resolve(uint worldID)
	{
		if (worldID < 3000)
		{
			if (worldID >= 1000)
			{
				if (worldID < 2000)
				{
					return WorldRegion.CN;
				}
				return WorldRegion.KR;
			}
			return WorldRegion.GL;
		}
		if (worldID < 4000)
		{
			return WorldRegion.TC;
		}
		return WorldRegion.None;
	}

	public static WorldRegion Resolve(Language language)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected I4, but got Unknown
		switch (language - 5)
		{
		case 0:
		case 1:
			return WorldRegion.CN;
		case 2:
			return WorldRegion.KR;
		case 3:
			return WorldRegion.TC;
		default:
			return WorldRegion.GL;
		}
	}
}
