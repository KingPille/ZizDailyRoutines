using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.Common.RemoteInteraction.Abstractions;
using DailyRoutines.Manager;
using DailyRoutines.RemoteInteraction.PlayerInfo.Models;
using Newtonsoft.Json;
using itAnDoW4mgYVlrmtMor;
using ycTZ7OSV787Z3a0g7JT;

namespace DailyRoutines.RemoteInteraction.PlayerInfo.Handlers;

public sealed class PlayerInfoQueryHandler : IRemoteQueryHandler<PlayerInfoKey, PlayerInfoValue>
{
	private sealed class PlayerInfoByCidResponse
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("worldid")]
		public uint WorldID { get; set; }

		[JsonProperty("success")]
		public bool Success { get; set; }

		public PlayerInfoByCidResponse()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			Name = string.Empty;
			base._002Ector();
		}
	}

	public static TimeSpan TTL { get; }

	public static TimeSpan FailureTTL { get; }

	public static async ValueTask<PlayerInfoValue> FetchAsync(PlayerInfoKey key, CancellationToken cancellationToken)
	{
		if (key.ContentID == 0L)
		{
			throw new ArgumentOutOfRangeException(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2F1676B ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d964b0b1313049f490f2107d03a6ed3a), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2BF04318 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_fc2313f9e2c04f099d4c47c5893e635d));
		}
		PlayerInfoByCidResponse playerInfoByCidResponse = JsonConvert.DeserializeObject<PlayerInfoByCidResponse>((await PqtKlnSSKxdRRHSb7mo.xXNSfmClyl(InternalAPIRequestType.RequestPlayerInfoByCID, new
		{
			p_contentid = key.ContentID.ToString(),
			p_region = key.Region.ToString()
		}, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)) ?? string.Empty);
		if (playerInfoByCidResponse == null || !playerInfoByCidResponse.Success)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(12, 1);
			defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x47336107 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5f88650ee89f435aad988a8583bda5b5));
			defaultInterpolatedStringHandler.AppendFormatted(key.ContentID);
			throw new InvalidOperationException(defaultInterpolatedStringHandler.ToStringAndClear());
		}
		return new PlayerInfoValue(playerInfoByCidResponse.Name ?? string.Empty, playerInfoByCidResponse.WorldID);
	}

	public PlayerInfoQueryHandler()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
	}

	static PlayerInfoQueryHandler()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		TTL = TimeSpan.FromMinutes(30L);
		FailureTTL = TimeSpan.FromSeconds(30L);
	}
}
