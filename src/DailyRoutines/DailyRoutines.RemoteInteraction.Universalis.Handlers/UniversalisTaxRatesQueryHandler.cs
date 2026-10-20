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

public sealed class UniversalisTaxRatesQueryHandler : IRemoteQueryHandler<UniversalisTaxRatesKey, UniversalisTaxRatesResponse>
{
	public static TimeSpan TTL { get; }

	public static TimeSpan FailureTTL { get; }

	public static async ValueTask<UniversalisTaxRatesResponse> FetchAsync(UniversalisTaxRatesKey key, CancellationToken cancellationToken)
	{
		return JsonConvert.DeserializeObject<UniversalisTaxRatesResponse>(await ny1etPLTndeSqdSqyAx.EU4Ll0t6Wd(ny1etPLTndeSqdSqyAx.D5GLs4B7j1(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x611676DA ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_8883645b187042ce8990c01069a51402), key.Parameters), cancellationToken).ConfigureAwait(continueOnCapturedContext: false)) ?? throw new InvalidOperationException(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5E3846EB ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f9eb04af496747c5b32cddd51d0b3997));
	}

	public UniversalisTaxRatesQueryHandler()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
	}

	static UniversalisTaxRatesQueryHandler()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		TTL = ny1etPLTndeSqdSqyAx.AuxiliaryCacheTTL;
		FailureTTL = ny1etPLTndeSqdSqyAx.FailureTTL;
	}
}
