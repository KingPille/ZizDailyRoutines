using B2nED1Wmdu0xsDR7Tjv;
using Newtonsoft.Json;

namespace DailyRoutines.RemoteInteraction.Universalis.Models.Responses;

public class UniversalisAggregatedMarketScope
{
	[JsonProperty("minListing")]
	public UniversalisAggregatedMarketValueByScope<UniversalisAggregatedMarketPricePoint> MinListing { get; set; }

	[JsonProperty("recentPurchase")]
	public UniversalisAggregatedMarketValueByScope<UniversalisAggregatedMarketPurchasePoint> RecentPurchase { get; set; }

	[JsonProperty("averageSalePrice")]
	public UniversalisAggregatedMarketValueByScope<UniversalisAggregatedMarketPriceValue> AverageSalePrice { get; set; }

	[JsonProperty("dailySaleVelocity")]
	public UniversalisAggregatedMarketValueByScope<UniversalisAggregatedMarketQuantityValue> DailySaleVelocity { get; set; }

	public UniversalisAggregatedMarketScope()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		MinListing = new UniversalisAggregatedMarketValueByScope<UniversalisAggregatedMarketPricePoint>();
		RecentPurchase = new UniversalisAggregatedMarketValueByScope<UniversalisAggregatedMarketPurchasePoint>();
		AverageSalePrice = new UniversalisAggregatedMarketValueByScope<UniversalisAggregatedMarketPriceValue>();
		DailySaleVelocity = new UniversalisAggregatedMarketValueByScope<UniversalisAggregatedMarketQuantityValue>();
		base._002Ector();
	}
}
