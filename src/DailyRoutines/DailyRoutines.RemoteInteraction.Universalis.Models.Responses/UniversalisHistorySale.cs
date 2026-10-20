using System;
using B2nED1Wmdu0xsDR7Tjv;
using Newtonsoft.Json;

namespace DailyRoutines.RemoteInteraction.Universalis.Models.Responses;

public class UniversalisHistorySale
{
	[JsonProperty("hq")]
	public bool HQ { get; set; }

	[JsonProperty("pricePerUnit")]
	public uint PricePerUnit { get; set; }

	[JsonProperty("quantity")]
	public uint Quantity { get; set; }

	[JsonProperty("timestamp")]
	public long Timestamp { get; set; }

	[JsonProperty("onMannequin")]
	public bool OnMannequin { get; set; }

	public DateTime GetSaleTime()
	{
		return DateTimeOffset.FromUnixTimeSeconds(Timestamp).DateTime;
	}

	public UniversalisHistorySale()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
	}
}
