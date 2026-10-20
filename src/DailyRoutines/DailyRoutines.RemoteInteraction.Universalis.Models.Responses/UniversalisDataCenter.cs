using System.Collections.Generic;
using B2nED1Wmdu0xsDR7Tjv;
using Newtonsoft.Json;

namespace DailyRoutines.RemoteInteraction.Universalis.Models.Responses;

public class UniversalisDataCenter
{
	[JsonProperty("name")]
	public string Name { get; set; }

	[JsonProperty("region")]
	public string Region { get; set; }

	[JsonProperty("worlds")]
	public List<uint> Worlds { get; set; }

	public UniversalisDataCenter()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		Name = string.Empty;
		Region = string.Empty;
		Worlds = new List<uint>();
		base._002Ector();
	}
}
