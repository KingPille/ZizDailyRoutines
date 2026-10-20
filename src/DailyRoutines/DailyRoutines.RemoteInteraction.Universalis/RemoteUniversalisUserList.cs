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

public class RemoteUniversalisUserList : RemoteInteractionBase
{
	public static UniversalisUserListKey CreateKey(string listID)
	{
		return new UniversalisUserListKey(listID);
	}

	public static RemoteSnapshot<UniversalisUserListResponse> GetOrRequest(string listID)
	{
		return ManagerBase<RemoteInteractionManager>.Instance().GetOrRequest<UniversalisUserListQueryHandler, UniversalisUserListKey, UniversalisUserListResponse>(CreateKey(listID));
	}

	public static IDisposable Observe(string listID, Action<RemoteSnapshot<UniversalisUserListResponse>> callback, bool marshalToFrameworkThread = true)
	{
		return ManagerBase<RemoteInteractionManager>.Instance().Observe<UniversalisUserListQueryHandler, UniversalisUserListKey, UniversalisUserListResponse>(CreateKey(listID), callback, marshalToFrameworkThread);
	}

	public static ValueTask<UniversalisUserListResponse> GetFreshAsync(string listID, CancellationToken cancellationToken = default(CancellationToken))
	{
		return ManagerBase<RemoteInteractionManager>.Instance().GetFreshAsync<UniversalisUserListQueryHandler, UniversalisUserListKey, UniversalisUserListResponse>(CreateKey(listID), cancellationToken);
	}

	public RemoteUniversalisUserList()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
	}
}
