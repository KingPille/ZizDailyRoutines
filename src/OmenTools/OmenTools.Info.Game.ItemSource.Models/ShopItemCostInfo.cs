using OmenTools.Extensions;
using OmenTools.Interop.Game.Lumina;

namespace OmenTools.Info.Game.ItemSource.Models;

public record ShopItemCostInfo(uint Cost, uint ItemID, uint? Collectablity = null)
{
	public string GetItemName()
	{
		return LuminaWrapper.GetItemName(ItemID);
	}

	public override string ToString()
	{
		if (Collectablity.HasValue)
		{
			return $"{GetItemName()} \ue03d ({Collectablity.Value}~)";
		}
		return GetItemName() + " x" + Cost.ToChineseString();
	}
}
