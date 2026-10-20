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
using DailyRoutines.RemoteInteraction.Universalis.Models.Responses;

namespace DailyRoutines.RemoteInteraction.Universalis;

public class RemoteUniversalisContent : RemoteInteractionBase
{
	public static UniversalisContentKey CreateKey(string contentID)
	{
		return new UniversalisContentKey(contentID);
	}

	public static RemoteSnapshot<UniversalisContentResponse> GetOrRequest(string contentID)
	{
		return ManagerBase<RemoteInteractionManager>.Instance().GetOrRequest<UniversalisContentQueryHandler, UniversalisContentKey, UniversalisContentResponse>(CreateKey(contentID));
	}

	public static IDisposable Observe(string contentID, Action<RemoteSnapshot<UniversalisContentResponse>> callback, bool marshalToFrameworkThread = true)
	{
		return ManagerBase<RemoteInteractionManager>.Instance().Observe<UniversalisContentQueryHandler, UniversalisContentKey, UniversalisContentResponse>(CreateKey(contentID), callback, marshalToFrameworkThread);
	}

	public static ValueTask<UniversalisContentResponse> GetFreshAsync(string contentID, CancellationToken cancellationToken = default(CancellationToken))
	{
		return ManagerBase<RemoteInteractionManager>.Instance().GetFreshAsync<UniversalisContentQueryHandler, UniversalisContentKey, UniversalisContentResponse>(CreateKey(contentID), cancellationToken);
	}

	public RemoteUniversalisContent()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
	}
}
