using System.Collections.Generic;
using B2nED1Wmdu0xsDR7Tjv;
using Newtonsoft.Json;

namespace DailyRoutines.RemoteInteraction.Universalis.Models.Responses;

public class UniversalisHistoryItemData
{
	[JsonProperty("itemID")]
	public uint ItemID { get; set; }

	[JsonProperty("lastUploadTime")]
	public long LastUploadTime { get; set; }

	[JsonProperty("entries")]
	public List<UniversalisHistorySale> Entries { get; set; }

	[JsonProperty("averagePrice")]
	public float AveragePrice { get; set; }

	[JsonProperty("averagePriceNQ")]
	public float AveragePriceNQ { get; set; }

	[JsonProperty("averagePriceHQ")]
	public float AveragePriceHQ { get; set; }

	[JsonProperty("regularSaleVelocity")]
	public float RegularSaleVelocity { get; set; }

	public UniversalisHistoryItemData()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		Entries = new List<UniversalisHistorySale>();
		base._002Ector();
	}
}
