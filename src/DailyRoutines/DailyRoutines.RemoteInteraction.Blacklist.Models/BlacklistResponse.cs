using System.Collections.Generic;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.RemoteInteraction.Blacklist.Models.Responses;
using Newtonsoft.Json;

namespace DailyRoutines.RemoteInteraction.Blacklist.Models;

internal sealed class BlacklistResponse
{
	[JsonProperty("success")]
	public bool Success { get; set; }

	[JsonProperty("error")]
	public string ErrorMessage { get; set; }

	[JsonProperty("blacklist_info")]
	public BlacklistInfoResponse? BlacklistInfo { get; set; }

	[JsonProperty("total_count")]
	public int TotalCount { get; set; }

	[JsonProperty("current_page_count")]
	public int CurrentPageCount { get; set; }

	[JsonProperty("data")]
	public List<BlacklistEntryResponse>? Data { get; set; }

	public BlacklistResponse()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		ErrorMessage = string.Empty;
		base._002Ector();
	}
}
