using FFXIVClientStructs.FFXIV.Client.UI.Info;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Extensions;

public static class InfoProxyItemSearchExtension
{
	extension(scoped ref InfoProxyItemSearch proxy)
	{
		public static bool IsListingsStuck => OmenServiceBase<GameState>.Instance().IsMarketListingsStuck;

		public unsafe bool IsFullyReceived(uint itemID = 0u)
		{
			fixed (InfoProxyItemSearch* ptr = &proxy)
			{
				if (ptr == null || get_IsListingsStuck())
				{
					return false;
				}
				if (itemID != 0 && ((InfoProxyItemSearch)ptr).SearchItemId != itemID)
				{
					return false;
				}
				uint searchItemId = ((InfoProxyItemSearch)ptr).SearchItemId;
				int listingCount = (int)((InfoProxyItemSearch)ptr).ListingCount;
				MarketBoardListing[] array = ((InfoProxyItemSearch)ptr).Listings.ToArray();
				if (listingCount > array.Length)
				{
					return false;
				}
				for (int i = 0; i < listingCount; i++)
				{
					if (array[i].ItemId != searchItemId || array[i].UnitPrice == 0)
					{
						return false;
					}
				}
				switch (((InfoProxyItemSearch)ptr).EntryCount)
				{
				default:
					return listingCount >= 10;
				case 0u:
					return true;
				case 1u:
				case 2u:
				case 3u:
				case 4u:
				case 5u:
				case 6u:
				case 7u:
				case 8u:
				case 9u:
				case 10u:
					return listingCount != 0;
				}
			}
		}
	}
}
