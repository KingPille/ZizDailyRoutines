using B2nED1Wmdu0xsDR7Tjv;
using Newtonsoft.Json;

namespace DailyRoutines.RemoteInteraction.Universalis.Models.Responses;

public class UniversalisMarketListing
{
	[JsonProperty("listingID")]
	public long ListingID { get; set; }

	[JsonProperty("pricePerUnit")]
	public uint PricePerUnit { get; set; }

	[JsonProperty("quantity")]
	public uint Quantity { get; set; }

	[JsonProperty("hq")]
	public bool HQ { get; set; }

	[JsonProperty("retainerName")]
	public string RetainerName { get; set; }

	[JsonProperty("onMannequin")]
	public bool OnMannequin { get; set; }

	[JsonProperty("total")]
	public uint Total { get; set; }

	public UniversalisMarketListing()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		RetainerName = string.Empty;
		base._002Ector();
	}
}
