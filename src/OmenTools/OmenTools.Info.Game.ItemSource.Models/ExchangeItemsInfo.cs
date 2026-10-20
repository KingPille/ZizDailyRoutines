using System.Collections.Generic;

namespace OmenTools.Info.Game.ItemSource.Models;

public sealed class ExchangeItemsInfo
{
	public uint CostItemID { get; init; }

	public List<ExchangeItemInfo> Items { get; init; } = new List<ExchangeItemInfo>();
}
