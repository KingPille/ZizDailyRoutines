using System.Collections.Generic;
using B2nED1Wmdu0xsDR7Tjv;
using Newtonsoft.Json;

namespace DailyRoutines.RemoteInteraction.Universalis.Models.Responses;

public class UniversalisResponseBase
{
	[JsonProperty("itemIDs")]
	public List<uint> ItemIds { get; set; }

	[JsonProperty("unresolvedItems")]
	public List<uint> UnresolvedItems { get; set; }

	[JsonProperty("worldID")]
	public uint? WorldID { get; set; }

	[JsonProperty("dcName")]
	public string DCName { get; set; }

	[JsonProperty("regionName")]
	public string RegionName { get; set; }

	[JsonProperty("worldName")]
	public string WorldName { get; set; }

	public UniversalisResponseBase()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		ItemIds = new List<uint>();
		UnresolvedItems = new List<uint>();
		DCName = string.Empty;
		RegionName = string.Empty;
		WorldName = string.Empty;
		base._002Ector();
	}
}
