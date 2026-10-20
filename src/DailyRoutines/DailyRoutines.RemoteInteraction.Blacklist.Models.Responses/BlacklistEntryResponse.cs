using B2nED1Wmdu0xsDR7Tjv;
using Newtonsoft.Json;

namespace DailyRoutines.RemoteInteraction.Blacklist.Models.Responses;

internal sealed class BlacklistEntryResponse
{
	[JsonProperty("time")]
	public long Time { get; set; }

	[JsonProperty("source_player")]
	public string SourcePlayer { get; set; }

	[JsonProperty("player")]
	public string Player { get; set; }

	[JsonProperty("remark")]
	public string Remark { get; set; }

	public BlacklistEntryResponse()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		SourcePlayer = string.Empty;
		Player = string.Empty;
		Remark = string.Empty;
		base._002Ector();
	}
}
