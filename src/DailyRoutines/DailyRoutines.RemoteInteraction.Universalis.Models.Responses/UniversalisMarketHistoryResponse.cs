using System.Collections.Generic;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.RemoteInteraction.Universalis.Helpers.JsonCoverters;
using Newtonsoft.Json;

namespace DailyRoutines.RemoteInteraction.Universalis.Models.Responses;

[JsonConverter(typeof(UniversalisHistoryConverter))]
public class UniversalisMarketHistoryResponse : UniversalisResponseBase
{
	public Dictionary<uint, UniversalisHistoryItemData> Items { get; set; }

	public UniversalisMarketHistoryResponse()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		Items = new Dictionary<uint, UniversalisHistoryItemData>();
		base._002Ector();
	}
}
