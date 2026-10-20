using B2nED1Wmdu0xsDR7Tjv;
using Newtonsoft.Json;
using OmenTools.Info.Json.Converters;

namespace DailyRoutines.RemoteInteraction.Blacklist.Models.Responses;

public class SharedBlacklistEntry
{
	[JsonProperty("time")]
	[JsonConverter(typeof(StringNumberConverter))]
	public long Time { get; set; }

	[JsonProperty("player")]
	[JsonConverter(typeof(StringNumberConverter))]
	public ulong Player { get; set; }

	[JsonProperty("remark")]
	public string Remark { get; set; }

	public SharedBlacklistEntry()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		Remark = string.Empty;
		base._002Ector();
	}
}
