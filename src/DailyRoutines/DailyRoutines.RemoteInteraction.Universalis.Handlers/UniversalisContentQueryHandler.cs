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

public sealed class UniversalisContentQueryHandler : IRemoteQueryHandler<UniversalisContentKey, UniversalisContentResponse>
{
	public static TimeSpan TTL { get; }

	public static TimeSpan FailureTTL { get; }

	public static async ValueTask<UniversalisContentResponse> FetchAsync(UniversalisContentKey key, CancellationToken cancellationToken)
	{
		return JsonConvert.DeserializeObject<UniversalisContentResponse>(await ny1etPLTndeSqdSqyAx.EU4Ll0t6Wd(ny1etPLTndeSqdSqyAx.D5GLs4B7j1(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2E9427FB ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_6103a9bab63e42479916a134b07f1c75) + Uri.EscapeDataString(key.ContentID)), cancellationToken).ConfigureAwait(continueOnCapturedContext: false)) ?? throw new InvalidOperationException(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4FBE6DFD ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_881eb9df907345dea66a751ad6d65e76));
	}

	public UniversalisContentQueryHandler()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
	}

	static UniversalisContentQueryHandler()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		TTL = ny1etPLTndeSqdSqyAx.AuxiliaryCacheTTL;
		FailureTTL = ny1etPLTndeSqdSqyAx.FailureTTL;
	}
}
