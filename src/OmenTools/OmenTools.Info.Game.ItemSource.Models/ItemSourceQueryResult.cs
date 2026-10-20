using OmenTools.Info.Game.ItemSource.Enums;

namespace OmenTools.Info.Game.ItemSource.Models;

public readonly record struct ItemSourceQueryResult(ItemSourceQueryState State, ItemSourceInfo? Data = null)
{
	public static ItemSourceQueryResult Building { get; } = new ItemSourceQueryResult(ItemSourceQueryState.Building);

	public static ItemSourceQueryResult NotFound { get; } = new ItemSourceQueryResult(ItemSourceQueryState.NotFound);

	public static ItemSourceQueryResult Failed { get; } = new ItemSourceQueryResult(ItemSourceQueryState.Failed);

	public static ItemSourceQueryResult Ready(ItemSourceInfo data)
	{
		return new ItemSourceQueryResult(ItemSourceQueryState.Ready, data);
	}
}
