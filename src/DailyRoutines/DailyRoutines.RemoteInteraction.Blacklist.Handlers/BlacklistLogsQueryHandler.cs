using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.Common.RemoteInteraction.Abstractions;
using DailyRoutines.Manager;
using DailyRoutines.RemoteInteraction.Blacklist.Models;
using DailyRoutines.RemoteInteraction.Blacklist.Models.Responses;
using Newtonsoft.Json;
using itAnDoW4mgYVlrmtMor;
using ycTZ7OSV787Z3a0g7JT;

namespace DailyRoutines.RemoteInteraction.Blacklist.Handlers;

public sealed class BlacklistLogsQueryHandler : IRemoteQueryHandler<BlacklistLogsKey, BlacklistLogsResponse>
{
	public static TimeSpan TTL { get; }

	public static TimeSpan FailureTTL { get; }

	public static async ValueTask<BlacklistLogsResponse> FetchAsync(BlacklistLogsKey key, CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(key.BlacklistID) || string.IsNullOrWhiteSpace(key.Password))
		{
			throw new ArgumentException(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1C7951AE ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_365ea33385244ba8b1c5da69c8292f00), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1FFB9E01 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_a46716ea783442acb36e95ced1c935fc));
		}
		Dictionary<string, object> dictionary = new Dictionary<string, object>
		{
			[GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5368742A ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d4ce857df7b047629e7091bbc815dd94)] = key.BlacklistID,
			[GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2BF04CE4 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_fc2313f9e2c04f099d4c47c5893e635d)] = key.Password,
			[GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x35A9EACF ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_95b251cdaa8148b7af54d7956ad9ed2a)] = Math.Clamp(key.Limit, 1, 100)
		};
		if (!string.IsNullOrWhiteSpace(key.Player))
		{
			dictionary[GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2F16F73 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d964b0b1313049f490f2107d03a6ed3a)] = key.Player;
		}
		if (!string.IsNullOrWhiteSpace(key.Status))
		{
			dictionary[GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x35A9ED2D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_95b251cdaa8148b7af54d7956ad9ed2a)] = key.Status;
		}
		if (!string.IsNullOrWhiteSpace(key.StartTime))
		{
			dictionary[GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x28285CAD ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_95ac0b4adf764381bcdc75a9d241d184)] = key.StartTime;
		}
		if (!string.IsNullOrWhiteSpace(key.EndTime))
		{
			dictionary[GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1F4A2134 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_0d3e4cb7200c49c9943fa2d7c1024d32)] = key.EndTime;
		}
		return JsonConvert.DeserializeObject<BlacklistLogsResponse>((await PqtKlnSSKxdRRHSb7mo.xXNSfmClyl(InternalAPIRequestType.GetBlacklistLogs, dictionary, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)) ?? string.Empty) ?? new BlacklistLogsResponse
		{
			Success = false
		};
	}

	public BlacklistLogsQueryHandler()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
	}

	static BlacklistLogsQueryHandler()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		TTL = TimeSpan.FromMinutes(1L);
		FailureTTL = TimeSpan.FromSeconds(20L);
	}
}
