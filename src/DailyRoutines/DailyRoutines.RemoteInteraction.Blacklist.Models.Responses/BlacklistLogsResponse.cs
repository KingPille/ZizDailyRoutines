using System.Collections.Generic;
using B2nED1Wmdu0xsDR7Tjv;
using Newtonsoft.Json;

namespace DailyRoutines.RemoteInteraction.Blacklist.Models.Responses;

public class BlacklistLogsResponse
{
	[JsonProperty("success")]
	public bool Success { get; set; }

	[JsonProperty("error")]
	public string Error { get; set; }

	[JsonProperty("blacklist_id")]
	public string BlacklistID { get; set; }

	[JsonProperty("player")]
	public string Player { get; set; }

	[JsonProperty("status_filter")]
	public string StatusFilter { get; set; }

	[JsonProperty("start_time")]
	public string StartTime { get; set; }

	[JsonProperty("end_time")]
	public string EndTime { get; set; }

	[JsonProperty("total_logs")]
	public int TotalLogs { get; set; }

	[JsonProperty("logs")]
	public List<BlacklistLogEntry> Logs { get; set; }

	public BlacklistLogsResponse()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		Error = string.Empty;
		BlacklistID = string.Empty;
		Player = string.Empty;
		StatusFilter = string.Empty;
		StartTime = string.Empty;
		EndTime = string.Empty;
		Logs = new List<BlacklistLogEntry>();
		base._002Ector();
	}
}
