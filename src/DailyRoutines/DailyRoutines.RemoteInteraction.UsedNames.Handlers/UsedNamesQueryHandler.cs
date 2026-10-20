using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.Common.RemoteInteraction.Abstractions;
using DailyRoutines.Manager;
using DailyRoutines.RemoteInteraction.UsedNames.Models;
using Newtonsoft.Json;
using itAnDoW4mgYVlrmtMor;
using ycTZ7OSV787Z3a0g7JT;

namespace DailyRoutines.RemoteInteraction.UsedNames.Handlers;

public sealed class UsedNamesQueryHandler : IRemoteQueryHandler<UsedNamesKey, List<UsedNamesChange>>
{
	private sealed class PlayerUsedNamesResponse
	{
		[JsonProperty("success")]
		public bool Success { get; set; }

		[JsonProperty("history")]
		public List<NameHistoryEntry> History { get; set; }

		public PlayerUsedNamesResponse()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			History = new List<NameHistoryEntry>();
			base._002Ector();
		}
	}

	private sealed class NameHistoryEntry
	{
		[JsonProperty("before_name")]
		public string BeforeName { get; set; }

		[JsonProperty("after_name")]
		public string AfterName { get; set; }

		[JsonProperty("created_at")]
		public string CreatedAt { get; set; }

		public DateTime GetChangedTime()
		{
			if (!DateTime.TryParse(CreatedAt, out var result))
			{
				return DateTime.MinValue;
			}
			return result;
		}

		public NameHistoryEntry()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			BeforeName = string.Empty;
			AfterName = string.Empty;
			CreatedAt = string.Empty;
			base._002Ector();
		}
	}

	public static TimeSpan TTL { get; }

	public static TimeSpan FailureTTL { get; }

	public static async ValueTask<List<UsedNamesChange>> FetchAsync(UsedNamesKey key, CancellationToken cancellationToken)
	{
		if (key.ContentID == 0L)
		{
			throw new ArgumentOutOfRangeException(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4119031B ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_55c846b516154967bf0c321596d2dc52), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x75EEF837 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_11422b9d07f9498fb995b7dd8e417b5b));
		}
		PlayerUsedNamesResponse playerUsedNamesResponse = JsonConvert.DeserializeObject<PlayerUsedNamesResponse>((await PqtKlnSSKxdRRHSb7mo.xXNSfmClyl(InternalAPIRequestType.RequestUsedNames, new
		{
			contentid = key.ContentID.ToString(),
			region = key.Region.ToString()
		}, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)) ?? string.Empty);
		if (playerUsedNamesResponse == null || !playerUsedNamesResponse.Success || playerUsedNamesResponse.History == null)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 1);
			defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x37CD5089 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_90dae8e50e3a4314867cef21f0bb1bb0));
			defaultInterpolatedStringHandler.AppendFormatted(key.ContentID);
			throw new InvalidOperationException(defaultInterpolatedStringHandler.ToStringAndClear());
		}
		return playerUsedNamesResponse.History.Select((NameHistoryEntry entry) => new UsedNamesChange
		{
			BeforeName = (entry.BeforeName ?? string.Empty),
			AfterName = (entry.AfterName ?? string.Empty),
			ChangedTime = entry.GetChangedTime()
		}).ToList();
	}

	public UsedNamesQueryHandler()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
	}

	static UsedNamesQueryHandler()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		TTL = TimeSpan.FromHours(12);
		FailureTTL = TimeSpan.FromSeconds(30L);
	}
}
