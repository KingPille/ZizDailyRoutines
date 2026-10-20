using System.Collections.Generic;
using B2nED1Wmdu0xsDR7Tjv;
using Newtonsoft.Json;

namespace DailyRoutines.RemoteInteraction.Universalis.Models.Responses;

public class UniversalisAggregatedMarketDataResponse
{
	[JsonProperty("results")]
	public List<UniversalisAggregatedMarketResult> Results { get; set; }

	[JsonProperty("failedItems")]
	public List<uint> FailedItems { get; set; }

	public UniversalisAggregatedMarketDataResponse()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		Results = new List<UniversalisAggregatedMarketResult>();
		FailedItems = new List<uint>();
		base._002Ector();
	}
}
