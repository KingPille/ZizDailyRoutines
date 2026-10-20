using Newtonsoft.Json;

namespace OmenTools.Info.DTOs.Lalachievements;

public record LLAPlayerSearchResultData
{
	[JsonProperty("id")]
	public int CharacterID { get; set; }

	[JsonProperty("name")]
	public string CharacterName { get; set; }

	[JsonProperty("iconUrl")]
	public string IconURL { get; set; }

	[JsonProperty("worldId")]
	public uint WorldID { get; set; }
}
