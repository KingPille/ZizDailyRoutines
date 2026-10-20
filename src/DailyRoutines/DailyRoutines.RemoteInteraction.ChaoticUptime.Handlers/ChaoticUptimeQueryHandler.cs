using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.Common.RemoteInteraction.Abstractions;
using DailyRoutines.Manager;
using DailyRoutines.RemoteInteraction.ChaoticUptime.Models;
using Newtonsoft.Json;
using itAnDoW4mgYVlrmtMor;
using ycTZ7OSV787Z3a0g7JT;

namespace DailyRoutines.RemoteInteraction.ChaoticUptime.Handlers;

public sealed class ChaoticUptimeQueryHandler : IRemoteQueryHandler<ChaoticUptimeKey, ChaoticUptimeValue>
{
	private sealed class ChaoticUptimeResponse
	{
		[JsonProperty("is_uptime")]
		public bool IsUptime { get; set; }

		public ChaoticUptimeResponse()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
		}
	}

	public static TimeSpan TTL { get; }

	public static TimeSpan FailureTTL { get; }

	public static async ValueTask<ChaoticUptimeValue> FetchAsync(ChaoticUptimeKey key, CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(key.DataCenter))
		{
			throw new ArgumentOutOfRangeException(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x76DB8227 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f8c073ebdce5414baf147b1be2d2c2c2), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4FBE6EDB ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_881eb9df907345dea66a751ad6d65e76));
		}
		ChaoticUptimeResponse chaoticUptimeResponse = JsonConvert.DeserializeObject<ChaoticUptimeResponse>((await PqtKlnSSKxdRRHSb7mo.ktqSjVMsCG(InternalAPIRequestType.RequestChaoticUptime, new Dictionary<string, string> { [GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3B5A25F0 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1e38c41bbda54d9abc5b426283576a9a)] = key.DataCenter }, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)) ?? string.Empty);
		if (chaoticUptimeResponse != null)
		{
			return new ChaoticUptimeValue(chaoticUptimeResponse.IsUptime);
		}
		throw new InvalidOperationException(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5583B398 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5603c9a2cf17495cb5da1dda759bb08d) + key.DataCenter);
	}

	public ChaoticUptimeQueryHandler()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
	}

	static ChaoticUptimeQueryHandler()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		TTL = TimeSpan.FromMinutes(1L);
		FailureTTL = TimeSpan.FromSeconds(30L);
	}
}
