using System;
using System.Threading;
using System.Threading.Tasks;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.RemoteInteraction.Abstractions;
using DailyRoutines.Common.RemoteInteraction.Models;
using DailyRoutines.Manager;
using DailyRoutines.RemoteInteraction.Universalis.Handlers;
using DailyRoutines.RemoteInteraction.Universalis.Models;
using DailyRoutines.RemoteInteraction.Universalis.Models.Requests;
using DailyRoutines.RemoteInteraction.Universalis.Models.Responses;

namespace DailyRoutines.RemoteInteraction.Universalis;

public class RemoteUniversalisItemUpdates : RemoteInteractionBase
{
	private static readonly RemoteSingletonKey RecentlyUpdatedKey;

	public static RemoteSnapshot<UniversalisRecentlyUpdatedItemsResponse> GetRecentlyUpdatedOrRequest()
	{
		return ManagerBase<RemoteInteractionManager>.Instance().GetOrRequest<UniversalisRecentlyUpdatedItemsQueryHandler, RemoteSingletonKey, UniversalisRecentlyUpdatedItemsResponse>(RecentlyUpdatedKey);
	}

	public static RemoteSnapshot<UniversalisItemRecencyResponse> GetMostRecentlyUpdatedOrRequest(UniversalisMostRecentlyUpdatedRequestParams? parameters = null)
	{
		return ManagerBase<RemoteInteractionManager>.Instance().GetOrRequest<UniversalisMostRecentlyUpdatedItemsQueryHandler, UniversalisMostRecentlyUpdatedKey, UniversalisItemRecencyResponse>(new UniversalisMostRecentlyUpdatedKey(parameters));
	}

	public static RemoteSnapshot<UniversalisItemRecencyResponse> GetLeastRecentlyUpdatedOrRequest(UniversalisLeastRecentlyUpdatedRequestParams? parameters = null)
	{
		return ManagerBase<RemoteInteractionManager>.Instance().GetOrRequest<UniversalisLeastRecentlyUpdatedItemsQueryHandler, UniversalisLeastRecentlyUpdatedKey, UniversalisItemRecencyResponse>(new UniversalisLeastRecentlyUpdatedKey(parameters));
	}

	public static IDisposable ObserveRecentlyUpdated(Action<RemoteSnapshot<UniversalisRecentlyUpdatedItemsResponse>> callback, bool marshalToFrameworkThread = true)
	{
		return ManagerBase<RemoteInteractionManager>.Instance().Observe<UniversalisRecentlyUpdatedItemsQueryHandler, RemoteSingletonKey, UniversalisRecentlyUpdatedItemsResponse>(RecentlyUpdatedKey, callback, marshalToFrameworkThread);
	}

	public static IDisposable ObserveMostRecentlyUpdated(Action<RemoteSnapshot<UniversalisItemRecencyResponse>> callback, UniversalisMostRecentlyUpdatedRequestParams? parameters = null, bool marshalToFrameworkThread = true)
	{
		return ManagerBase<RemoteInteractionManager>.Instance().Observe<UniversalisMostRecentlyUpdatedItemsQueryHandler, UniversalisMostRecentlyUpdatedKey, UniversalisItemRecencyResponse>(new UniversalisMostRecentlyUpdatedKey(parameters), callback, marshalToFrameworkThread);
	}

	public static IDisposable ObserveLeastRecentlyUpdated(Action<RemoteSnapshot<UniversalisItemRecencyResponse>> callback, UniversalisLeastRecentlyUpdatedRequestParams? parameters = null, bool marshalToFrameworkThread = true)
	{
		return ManagerBase<RemoteInteractionManager>.Instance().Observe<UniversalisLeastRecentlyUpdatedItemsQueryHandler, UniversalisLeastRecentlyUpdatedKey, UniversalisItemRecencyResponse>(new UniversalisLeastRecentlyUpdatedKey(parameters), callback, marshalToFrameworkThread);
	}

	public static ValueTask<UniversalisRecentlyUpdatedItemsResponse> GetRecentlyUpdatedFreshAsync(CancellationToken cancellationToken = default(CancellationToken))
	{
		return ManagerBase<RemoteInteractionManager>.Instance().GetFreshAsync<UniversalisRecentlyUpdatedItemsQueryHandler, RemoteSingletonKey, UniversalisRecentlyUpdatedItemsResponse>(RecentlyUpdatedKey, cancellationToken);
	}

	public static ValueTask<UniversalisItemRecencyResponse> GetMostRecentlyUpdatedFreshAsync(UniversalisMostRecentlyUpdatedRequestParams? parameters = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		return ManagerBase<RemoteInteractionManager>.Instance().GetFreshAsync<UniversalisMostRecentlyUpdatedItemsQueryHandler, UniversalisMostRecentlyUpdatedKey, UniversalisItemRecencyResponse>(new UniversalisMostRecentlyUpdatedKey(parameters), cancellationToken);
	}

	public static ValueTask<UniversalisItemRecencyResponse> GetLeastRecentlyUpdatedFreshAsync(UniversalisLeastRecentlyUpdatedRequestParams? parameters = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		return ManagerBase<RemoteInteractionManager>.Instance().GetFreshAsync<UniversalisLeastRecentlyUpdatedItemsQueryHandler, UniversalisLeastRecentlyUpdatedKey, UniversalisItemRecencyResponse>(new UniversalisLeastRecentlyUpdatedKey(parameters), cancellationToken);
	}

	public RemoteUniversalisItemUpdates()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
	}

	static RemoteUniversalisItemUpdates()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
	}
}
