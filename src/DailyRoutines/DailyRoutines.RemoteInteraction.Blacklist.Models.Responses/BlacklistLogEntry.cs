using System;
using System.Runtime.CompilerServices;
using System.Threading;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.Common.RemoteInteraction.Enums;
using DailyRoutines.Common.RemoteInteraction.Helpers;
using DailyRoutines.RemoteInteraction.PlayerInfo;
using DailyRoutines.RemoteInteraction.PlayerInfo.Models;
using Newtonsoft.Json;
using OmenTools.Extensions;
using OmenTools.Info.Json.Converters;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;

namespace DailyRoutines.RemoteInteraction.Blacklist.Models.Responses;

public class BlacklistLogEntry
{
	[CompilerGenerated]
	private DateTime _003CChangeDateTime_003Ek__BackingField;

	[CompilerGenerated]
	private DateTime _003CLogDateTime_003Ek__BackingField;

	private static WorldRegion CurrentRegion => WorldRegionResolver.Resolve(GameState.ClientLanguage);

	[JsonConverter(typeof(StringNumberConverter))]
	[JsonProperty("player")]
	public ulong PlayerContentID { get; set; }

	[JsonProperty("operation_type")]
	public string OperationType { get; set; }

	[JsonProperty("old_source_player")]
	public string OldSourcePlayer { get; set; }

	[JsonProperty("new_source_player")]
	public string NewSourcePlayer { get; set; }

	[JsonProperty("old_remark")]
	public string OldRemark { get; set; }

	[JsonProperty("new_remark")]
	public string NewRemark { get; set; }

	[JsonProperty("old_status")]
	public string OldStatus { get; set; }

	[JsonProperty("new_status")]
	public string NewStatus { get; set; }

	[JsonProperty("changed_by_source_player")]
	[JsonConverter(typeof(StringNumberConverter))]
	public ulong OperationPlayerContentID { get; set; }

	[JsonProperty("change_time")]
	public string ChangeTime { get; set; }

	[JsonProperty("log_time")]
	public string LogTime { get; set; }

	[JsonIgnore]
	public DateTime ChangeDateTime
	{
		get
		{
			if (_003CChangeDateTime_003Ek__BackingField != DateTime.MinValue)
			{
				return _003CChangeDateTime_003Ek__BackingField;
			}
			if (!long.TryParse(ChangeTime, out var result) || result <= 0)
			{
				return DateTime.MinValue;
			}
			return _003CChangeDateTime_003Ek__BackingField = result.ToUTCDateTimeFromUnixSeconds().ToLocalTime();
		}
	}

	[JsonIgnore]
	public DateTime LogDateTime
	{
		get
		{
			if (_003CLogDateTime_003Ek__BackingField != DateTime.MinValue)
			{
				return _003CLogDateTime_003Ek__BackingField;
			}
			if (!long.TryParse(LogTime, out var result) || result <= 0)
			{
				return DateTime.MinValue;
			}
			return _003CLogDateTime_003Ek__BackingField = result.ToUTCDateTimeFromUnixSeconds().ToLocalTime();
		}
	}

	[JsonIgnore]
	public string PlayerName
	{
		get
		{
			if (!TryGetPlayerInfo(PlayerContentID, out PlayerInfoValue value))
			{
				return string.Empty;
			}
			return value.Name;
		}
	}

	[JsonIgnore]
	public uint PlayerWorldID
	{
		get
		{
			if (!TryGetPlayerInfo(PlayerContentID, out PlayerInfoValue value))
			{
				return 0u;
			}
			return value.WorldID;
		}
	}

	[JsonIgnore]
	public string PlayerWorldName
	{
		get
		{
			if (PlayerWorldID != 0)
			{
				return LuminaWrapper.GetWorldName(PlayerWorldID);
			}
			return string.Empty;
		}
	}

	[JsonIgnore]
	public string OperationPlayerName
	{
		get
		{
			if (!TryGetPlayerInfo(OperationPlayerContentID, out PlayerInfoValue value))
			{
				return string.Empty;
			}
			return value.Name;
		}
	}

	[JsonIgnore]
	public uint OperationPlayerWorldID
	{
		get
		{
			if (!TryGetPlayerInfo(OperationPlayerContentID, out PlayerInfoValue value))
			{
				return 0u;
			}
			return value.WorldID;
		}
	}

	[JsonIgnore]
	public string OperationPlayerWorldName
	{
		get
		{
			if (OperationPlayerWorldID != 0)
			{
				return LuminaWrapper.GetWorldName(OperationPlayerWorldID);
			}
			return string.Empty;
		}
	}

	public void WarmupResolvedPlayersAsync(CancellationToken cancellationToken = default(CancellationToken))
	{
		WarmupPlayer(PlayerContentID, cancellationToken);
		WarmupPlayer(OperationPlayerContentID, cancellationToken);
	}

	private static void WarmupPlayer(ulong contentID, CancellationToken cancellationToken)
	{
		if (contentID != 0L)
		{
			RemotePlayerInfo.GetOrRequest(contentID, CurrentRegion, cancellationToken);
		}
	}

	private static bool TryGetPlayerInfo(ulong contentID, out PlayerInfoValue value)
	{
		if (contentID == 0L)
		{
			value = null;
			return false;
		}
		return RemotePlayerInfo.TryGet(contentID, CurrentRegion, out value);
	}

	public BlacklistLogEntry()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		OperationType = string.Empty;
		OldSourcePlayer = string.Empty;
		NewSourcePlayer = string.Empty;
		OldRemark = string.Empty;
		NewRemark = string.Empty;
		OldStatus = string.Empty;
		NewStatus = string.Empty;
		ChangeTime = string.Empty;
		LogTime = string.Empty;
		_003CChangeDateTime_003Ek__BackingField = DateTime.MinValue;
		_003CLogDateTime_003Ek__BackingField = DateTime.MinValue;
		base._002Ector();
	}
}
