using B2nED1Wmdu0xsDR7Tjv;
using Newtonsoft.Json;

namespace DailyRoutines.RemoteInteraction.Blacklist.Models.Responses;

internal sealed class CreateSharedBlacklistResponse
{
	[JsonProperty("success")]
	public bool Success { get; set; }

	[JsonProperty("blacklist_id")]
	public string BlacklistID { get; set; }

	[JsonProperty("message")]
	public string Message { get; set; }

	[JsonProperty("total_entries")]
	public int TotalEntries { get; set; }

	[JsonProperty("expires_at")]
	public string ExpiresAt { get; set; }

	public CreateSharedBlacklistResponse()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		BlacklistID = string.Empty;
		Message = string.Empty;
		ExpiresAt = string.Empty;
		base._002Ector();
	}
}
