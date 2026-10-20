using System.Collections.Generic;
using B2nED1Wmdu0xsDR7Tjv;
using Newtonsoft.Json;

namespace DailyRoutines.RemoteInteraction.Universalis.Models.Responses;

public class UniversalisUserListResponse
{
	[JsonProperty("id")]
	public string ID { get; set; }

	[JsonProperty("created")]
	public string Created { get; set; }

	[JsonProperty("updated")]
	public string Updated { get; set; }

	[JsonProperty("name")]
	public string Name { get; set; }

	[JsonProperty("itemIDs")]
	public List<uint> ItemIDs { get; set; }

	public UniversalisUserListResponse()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		ID = string.Empty;
		Created = string.Empty;
		Updated = string.Empty;
		Name = string.Empty;
		ItemIDs = new List<uint>();
		base._002Ector();
	}
}
