using System.Collections.Frozen;

namespace OmenTools.Info.Game.Data;

public static class ContentTypes
{
	public static FrozenSet<uint> NotPVE { get; } = FrozenSet.Create<uint>(6u, 12u, 16u, 17u, 18u, 19u, 20u, 25u, 31u, 32u, 34u, 35u, 36u);
}
