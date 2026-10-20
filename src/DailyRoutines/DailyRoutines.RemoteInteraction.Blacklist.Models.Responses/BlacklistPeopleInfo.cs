using B2nED1Wmdu0xsDR7Tjv;
using Newtonsoft.Json;
using OmenTools.Info.Json.Converters;

namespace DailyRoutines.RemoteInteraction.Blacklist.Models.Responses;

public class BlacklistPeopleInfo
{
	[JsonProperty("time")]
	[JsonConverter(typeof(StringNumberConverter))]
	public long Time { get; set; }

	[JsonConverter(typeof(StringNumberConverter))]
	[JsonProperty("source_player")]
	public ulong SourcePlayer { get; set; }

	[JsonConverter(typeof(StringNumberConverter))]
	[JsonProperty("player")]
	public ulong Player { get; set; }

	[JsonProperty("remark")]
	public string Remark { get; set; }

	[JsonProperty("status")]
	[JsonConverter(typeof(StringNumberConverter))]
	public int Status { get; set; }

	public BlacklistPeopleInfo()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		Remark = string.Empty;
		base._002Ector();
	}
}
