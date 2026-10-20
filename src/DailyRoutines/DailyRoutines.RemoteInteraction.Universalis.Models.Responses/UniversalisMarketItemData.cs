using System;
using System.Collections.Generic;
using B2nED1Wmdu0xsDR7Tjv;
using Newtonsoft.Json;

namespace DailyRoutines.RemoteInteraction.Universalis.Models.Responses;

public class UniversalisMarketItemData
{
	[JsonProperty("itemID")]
	public uint ItemID { get; set; }

	[JsonProperty("lastUploadTime")]
	public long LastUploadTime { get; set; }

	[JsonProperty("listings")]
	public List<UniversalisMarketListing> Listings { get; set; }

	[JsonProperty("listingsCount")]
	public int ListingsCount { get; set; }

	[JsonProperty("recentHistory")]
	public List<UniversalisHistorySale> RecentHistory { get; set; }

	[JsonProperty("minPrice")]
	public uint MinPrice { get; set; }

	[JsonProperty("regularSaleVelocity")]
	public float RegularSaleVelocity { get; set; }

	public DateTime GetLastUploadTime()
	{
		return DateTimeOffset.FromUnixTimeMilliseconds(LastUploadTime).DateTime;
	}

	public UniversalisMarketItemData()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		Listings = new List<UniversalisMarketListing>();
		RecentHistory = new List<UniversalisHistorySale>();
		base._002Ector();
	}
}
