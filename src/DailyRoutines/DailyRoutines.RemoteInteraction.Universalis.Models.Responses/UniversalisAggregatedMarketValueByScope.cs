using B2nED1Wmdu0xsDR7Tjv;
using Newtonsoft.Json;

namespace DailyRoutines.RemoteInteraction.Universalis.Models.Responses;

public class UniversalisAggregatedMarketValueByScope<TValue> where TValue : class, new()
{
	[JsonProperty("world")]
	public TValue World { get; set; }

	[JsonProperty("dc")]
	public TValue DC { get; set; }

	[JsonProperty("region")]
	public TValue Region { get; set; }

	public UniversalisAggregatedMarketValueByScope()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		World = new TValue();
		DC = new TValue();
		Region = new TValue();
		base._002Ector();
	}
}
