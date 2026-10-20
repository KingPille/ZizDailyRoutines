using System.Collections.Generic;
using OmenTools.Info.Game.ItemSource.Enums;
using OmenTools.Interop.Game.Lumina;

namespace OmenTools.Info.Game.ItemSource.Models;

public sealed class ExchangeItemInfo
{
	public uint ItemID { get; init; }

	public ItemShopType ShopType { get; init; }

	public string AchievementDescription { get; init; } = string.Empty;

	public List<ExchangeItemNPCInfo> NPCInfos { get; init; } = new List<ExchangeItemNPCInfo>();

	public string GetItemName()
	{
		return LuminaWrapper.GetItemName(ItemID);
	}
}
