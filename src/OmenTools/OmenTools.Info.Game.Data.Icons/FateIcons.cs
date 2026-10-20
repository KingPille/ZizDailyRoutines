using System.Collections.Frozen;

namespace OmenTools.Info.Game.Data.Icons;

public static class FateIcons
{
	public static FrozenSet<uint> All { get; } = FrozenSet.Create<uint>(60458u, 60501u, 60502u, 60503u, 60504u, 60505u, 60506u, 60507u, 60508u);

	public static bool IsFate(uint iconID)
	{
		return All.Contains(iconID);
	}
}
