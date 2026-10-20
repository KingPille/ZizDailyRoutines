using System;
using System.Threading;
using System.Threading.Tasks;
using B2nED1Wmdu0xsDR7Tjv;
using CVg02HL1lNJ30jq90OI;
using DailyRoutines.Common.RemoteInteraction.Abstractions;
using DailyRoutines.RemoteInteraction.Universalis.Models;
using DailyRoutines.RemoteInteraction.Universalis.Models.Responses;
using Newtonsoft.Json;
using itAnDoW4mgYVlrmtMor;

namespace DailyRoutines.RemoteInteraction.Universalis.Handlers;

public sealed class UniversalisUserListQueryHandler : IRemoteQueryHandler<UniversalisUserListKey, UniversalisUserListResponse>
{
	public static TimeSpan TTL { get; }

	public static TimeSpan FailureTTL { get; }

	public static async ValueTask<UniversalisUserListResponse> FetchAsync(UniversalisUserListKey key, CancellationToken cancellationToken)
	{
		return JsonConvert.DeserializeObject<UniversalisUserListResponse>(await ny1etPLTndeSqdSqyAx.EU4Ll0t6Wd(ny1etPLTndeSqdSqyAx.D5GLs4B7j1(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x35B4A0BC ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_042249f573d4431982cd93672ca35bd3) + Uri.EscapeDataString(key.ListID)), cancellationToken).ConfigureAwait(continueOnCapturedContext: false)) ?? throw new InvalidOperationException(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(-1675397288 ^ -749588057 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_3782f2a028bd4046aa095423cd2526e0));
	}

	public UniversalisUserListQueryHandler()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
	}

	static UniversalisUserListQueryHandler()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		TTL = ny1etPLTndeSqdSqyAx.AuxiliaryCacheTTL;
		FailureTTL = ny1etPLTndeSqdSqyAx.FailureTTL;
	}
}
