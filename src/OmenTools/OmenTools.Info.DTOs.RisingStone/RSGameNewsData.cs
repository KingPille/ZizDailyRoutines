using Newtonsoft.Json;

namespace OmenTools.Info.DTOs.RisingStone;

public record RSGameNewsData
{
	[JsonProperty("id")]
	public uint ID { get; set; }

	[JsonProperty("title")]
	public string Title { get; set; }

	[JsonProperty("author")]
	public string Author { get; set; }

	[JsonProperty("home_image_path")]
	public string HomeImagePath { get; set; }

	[JsonProperty("publish_date")]
	public string PublishDate { get; set; }

	[JsonProperty("summary")]
	public string Summary { get; set; }

	[JsonProperty("sort_index")]
	public int SortIndex { get; set; }
}
