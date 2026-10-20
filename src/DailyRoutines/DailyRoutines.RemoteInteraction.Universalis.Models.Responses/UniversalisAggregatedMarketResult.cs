using System.Collections.Generic;
using B2nED1Wmdu0xsDR7Tjv;
using Newtonsoft.Json;

namespace DailyRoutines.RemoteInteraction.Universalis.Models.Responses;

public class UniversalisAggregatedMarketResult
{
	[JsonProperty("itemId")]
	public uint ItemID { get; set; }

	[JsonProperty("nq")]
	public UniversalisAggregatedMarketScope NQ { get; set; }

	[JsonProperty("hq")]
	public UniversalisAggregatedMarketScope HQ { get; set; }

	[JsonProperty("worldUploadTimes")]
	public List<UniversalisAggregatedWorldUploadTime> WorldUploadTimes { get; set; }

	public UniversalisAggregatedMarketResult()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		NQ = new UniversalisAggregatedMarketScope();
		HQ = new UniversalisAggregatedMarketScope();
		WorldUploadTimes = new List<UniversalisAggregatedWorldUploadTime>();
		base._002Ector();
	}
}
