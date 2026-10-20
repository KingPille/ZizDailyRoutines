using System.Collections.Generic;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.RemoteInteraction.Universalis.Helpers.JsonCoverters;
using Newtonsoft.Json;

namespace DailyRoutines.RemoteInteraction.Universalis.Models.Responses;

[JsonConverter(typeof(UniversalisMarketConverter))]
public class UniversalisMarketDataResponse : UniversalisResponseBase
{
	public Dictionary<uint, UniversalisMarketItemData> Items { get; set; }

	public UniversalisMarketDataResponse()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		Items = new Dictionary<uint, UniversalisMarketItemData>();
		base._002Ector();
	}
}
