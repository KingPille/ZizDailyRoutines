using Newtonsoft.Json;

namespace OmenTools.Info.DTOs.RisingStone;

public record RSPlayerSearchResultData
{
	[JsonProperty("uuid")]
	public uint UUID { get; set; }

	[JsonProperty("avatar")]
	public string Avatar { get; set; }

	[JsonProperty("character_name")]
	public string CharacterName { get; set; }

	[JsonProperty("area_name")]
	public string AreaName { get; set; }

	[JsonProperty("group_name")]
	public string GroupName { get; set; }

	[JsonProperty("profile")]
	public string Profile { get; set; }

	[JsonProperty("test_limited_badge")]
	public uint TestLimitedBadge { get; set; }

	[JsonProperty("posts2_creator_badge")]
	public uint Posts2CreatorBadge { get; set; }

	[JsonProperty("admin_tag")]
	public int AdminTag { get; set; }

	[JsonProperty("fansNum")]
	public uint FansNum { get; set; }
}
