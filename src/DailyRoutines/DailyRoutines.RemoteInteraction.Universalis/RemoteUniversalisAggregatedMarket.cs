using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using B2nED1Wmdu0xsDR7Tjv;
using CVg02HL1lNJ30jq90OI;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.RemoteInteraction.Abstractions;
using DailyRoutines.Common.RemoteInteraction.Models;
using DailyRoutines.Manager;
using DailyRoutines.RemoteInteraction.Universalis.Handlers;
using DailyRoutines.RemoteInteraction.Universalis.Models;
using DailyRoutines.RemoteInteraction.Universalis.Models.Responses;

namespace DailyRoutines.RemoteInteraction.Universalis;

public class RemoteUniversalisAggregatedMarket : RemoteInteractionBase
{
	public static UniversalisAggregatedMarketDataKey CreateKey(IEnumerable<uint> itemIds, string worldOrDC)
	{
		return new UniversalisAggregatedMarketDataKey(ny1etPLTndeSqdSqyAx.Pd0LM0mExY(itemIds), worldOrDC);
	}

	public static RemoteSnapshot<UniversalisAggregatedMarketDataResponse> GetOrRequest(IEnumerable<uint> itemIds, string worldOrDC)
	{
		return ManagerBase<RemoteInteractionManager>.Instance().GetOrRequest<UniversalisAggregatedMarketDataQueryHandler, UniversalisAggregatedMarketDataKey, UniversalisAggregatedMarketDataResponse>(CreateKey(itemIds, worldOrDC));
	}

	public static IDisposable Observe(IEnumerable<uint> itemIds, string worldOrDC, Action<RemoteSnapshot<UniversalisAggregatedMarketDataResponse>> callback, bool marshalToFrameworkThread = true)
	{
		return ManagerBase<RemoteInteractionManager>.Instance().Observe<UniversalisAggregatedMarketDataQueryHandler, UniversalisAggregatedMarketDataKey, UniversalisAggregatedMarketDataResponse>(CreateKey(itemIds, worldOrDC), callback, marshalToFrameworkThread);
	}

	public static ValueTask<UniversalisAggregatedMarketDataResponse> GetFreshAsync(IEnumerable<uint> itemIds, string worldOrDC, CancellationToken cancellationToken = default(CancellationToken))
	{
		return ManagerBase<RemoteInteractionManager>.Instance().GetFreshAsync<UniversalisAggregatedMarketDataQueryHandler, UniversalisAggregatedMarketDataKey, UniversalisAggregatedMarketDataResponse>(CreateKey(itemIds, worldOrDC), cancellationToken);
	}

	public RemoteUniversalisAggregatedMarket()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
	}
}
