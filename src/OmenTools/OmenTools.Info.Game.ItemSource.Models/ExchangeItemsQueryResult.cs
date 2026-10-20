using OmenTools.Info.Game.ItemSource.Enums;

namespace OmenTools.Info.Game.ItemSource.Models;

public readonly record struct ExchangeItemsQueryResult(ItemSourceQueryState State, ExchangeItemsInfo? Data = null)
{
	public static ExchangeItemsQueryResult Building { get; } = new ExchangeItemsQueryResult(ItemSourceQueryState.Building);

	public static ExchangeItemsQueryResult NotFound { get; } = new ExchangeItemsQueryResult(ItemSourceQueryState.NotFound);

	public static ExchangeItemsQueryResult Failed { get; } = new ExchangeItemsQueryResult(ItemSourceQueryState.Failed);

	public static ExchangeItemsQueryResult Ready(ExchangeItemsInfo data)
	{
		return new ExchangeItemsQueryResult(ItemSourceQueryState.Ready, data);
	}
}
