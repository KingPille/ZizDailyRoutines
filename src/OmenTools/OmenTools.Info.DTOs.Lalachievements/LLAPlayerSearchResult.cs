using System.Collections.Generic;
using Newtonsoft.Json;

namespace OmenTools.Info.DTOs.Lalachievements;

public record LLAPlayerSearchResult
{
	[JsonProperty("deployTime")]
	public long DeployTime { get; set; }

	[JsonProperty("results")]
	public List<LLAPlayerSearchResultData> Data { get; set; }
}
