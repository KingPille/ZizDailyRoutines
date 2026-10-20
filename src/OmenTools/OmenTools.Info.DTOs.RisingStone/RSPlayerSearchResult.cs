using System.Collections.Generic;
using Newtonsoft.Json;

namespace OmenTools.Info.DTOs.RisingStone;

public record RSPlayerSearchResult
{
	[JsonProperty("code")]
	public int Code { get; set; }

	[JsonProperty("msg")]
	public string Message { get; set; }

	[JsonProperty("data")]
	public List<RSPlayerSearchResultData> Data { get; set; }
}
