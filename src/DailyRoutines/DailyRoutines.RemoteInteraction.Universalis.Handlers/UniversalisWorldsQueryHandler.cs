using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using B2nED1Wmdu0xsDR7Tjv;
using CVg02HL1lNJ30jq90OI;
using DailyRoutines.Common.RemoteInteraction.Abstractions;
using DailyRoutines.Common.RemoteInteraction.Models;
using DailyRoutines.RemoteInteraction.Universalis.Models.Responses;
using Newtonsoft.Json;
using itAnDoW4mgYVlrmtMor;

namespace DailyRoutines.RemoteInteraction.Universalis.Handlers;

public sealed class UniversalisWorldsQueryHandler : IRemoteQueryHandler<RemoteSingletonKey, List<UniversalisWorld>>
{
	private static readonly HashSet<uint> InvalidWorlds;

	public static TimeSpan TTL { get; }

	public static TimeSpan FailureTTL { get; }

	public static async ValueTask<List<UniversalisWorld>> FetchAsync(RemoteSingletonKey key, CancellationToken cancellationToken)
	{
		return (JsonConvert.DeserializeObject<List<UniversalisWorld>>(await ny1etPLTndeSqdSqyAx.EU4Ll0t6Wd(ny1etPLTndeSqdSqyAx.D5GLs4B7j1(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6CA79AE7 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_86e8e55020514087b1fe097f879d6dff)), cancellationToken).ConfigureAwait(continueOnCapturedContext: false)) ?? new List<UniversalisWorld>()).Where((UniversalisWorld world) => !InvalidWorlds.Contains(world.ID)).ToList();
	}

	public UniversalisWorldsQueryHandler()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
	}

	static UniversalisWorldsQueryHandler()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		InvalidWorlds = new HashSet<uint> { 1064u, 1068u, 1187u };
		TTL = ny1etPLTndeSqdSqyAx.StaticDataCacheTTL;
		FailureTTL = ny1etPLTndeSqdSqyAx.FailureTTL;
	}
}
