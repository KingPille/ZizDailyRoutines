using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace OmenTools.Info.DTOs.GitHub;

public record GitHubRelease
{
	[JsonProperty("id")]
	public int ID { get; set; }

	[JsonProperty("tag_name")]
	public string TagName { get; set; }

	[JsonProperty("name")]
	public string Name { get; set; }

	[JsonProperty("body")]
	public string Body { get; set; }

	[JsonProperty("published_at")]
	public DateTime PublishedAt { get; set; }

	[JsonProperty("assets")]
	public List<GitHubAsset> Assets { get; set; }
}
