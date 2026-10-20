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
using DailyRoutines.RemoteInteraction.Universalis.Models.Requests;
using DailyRoutines.RemoteInteraction.Universalis.Models.Responses;

namespace DailyRoutines.RemoteInteraction.Universalis;

public class RemoteUniversalisHistory : RemoteInteractionBase
{
	public static UniversalisMarketHistoryKey CreateKey(IEnumerable<uint> itemIds, string worldOrDC, UniversalisMarketHistoryRequestParams? parameters = null)
	{
		return new UniversalisMarketHistoryKey(ny1etPLTndeSqdSqyAx.Pd0LM0mExY(itemIds), worldOrDC, parameters);
	}

	public static RemoteSnapshot<UniversalisMarketHistoryResponse> GetOrRequest(IEnumerable<uint> itemIds, string worldOrDC, UniversalisMarketHistoryRequestParams? parameters = null)
	{
		return ManagerBase<RemoteInteractionManager>.Instance().GetOrRequest<UniversalisMarketHistoryQueryHandler, UniversalisMarketHistoryKey, UniversalisMarketHistoryResponse>(CreateKey(itemIds, worldOrDC, parameters));
	}

	public static IDisposable Observe(IEnumerable<uint> itemIds, string worldOrDC, Action<RemoteSnapshot<UniversalisMarketHistoryResponse>> callback, UniversalisMarketHistoryRequestParams? parameters = null, bool marshalToFrameworkThread = true)
	{
		return ManagerBase<RemoteInteractionManager>.Instance().Observe<UniversalisMarketHistoryQueryHandler, UniversalisMarketHistoryKey, UniversalisMarketHistoryResponse>(CreateKey(itemIds, worldOrDC, parameters), callback, marshalToFrameworkThread);
	}

	public static ValueTask<UniversalisMarketHistoryResponse> GetFreshAsync(IEnumerable<uint> itemIds, string worldOrDC, UniversalisMarketHistoryRequestParams? parameters = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		return ManagerBase<RemoteInteractionManager>.Instance().GetFreshAsync<UniversalisMarketHistoryQueryHandler, UniversalisMarketHistoryKey, UniversalisMarketHistoryResponse>(CreateKey(itemIds, worldOrDC, parameters), cancellationToken);
	}

	public RemoteUniversalisHistory()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
	}
}
