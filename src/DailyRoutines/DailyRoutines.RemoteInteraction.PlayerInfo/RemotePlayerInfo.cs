using System;
using System.Threading;
using System.Threading.Tasks;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.RemoteInteraction.Abstractions;
using DailyRoutines.Common.RemoteInteraction.Enums;
using DailyRoutines.Common.RemoteInteraction.Models;
using DailyRoutines.Manager;
using DailyRoutines.RemoteInteraction.PlayerInfo.Handlers;
using DailyRoutines.RemoteInteraction.PlayerInfo.Models;

namespace DailyRoutines.RemoteInteraction.PlayerInfo;

public class RemotePlayerInfo : RemoteInteractionBase
{
	public static PlayerInfoKey CreateKey(ulong contentID, WorldRegion region)
	{
		return new PlayerInfoKey(contentID, region);
	}

	public static RemoteSnapshot<PlayerInfoValue> GetOrRequest(ulong contentID, WorldRegion region, CancellationToken cancellationToken = default(CancellationToken))
	{
		return ManagerBase<RemoteInteractionManager>.Instance().GetOrRequest<PlayerInfoQueryHandler, PlayerInfoKey, PlayerInfoValue>(new PlayerInfoKey(contentID, region), cancellationToken);
	}

	public static bool TryGet(ulong contentID, WorldRegion region, out PlayerInfoValue value)
	{
		return ManagerBase<RemoteInteractionManager>.Instance().TryGet<PlayerInfoQueryHandler, PlayerInfoKey, PlayerInfoValue>(new PlayerInfoKey(contentID, region), out value);
	}

	public static IDisposable Observe(ulong contentID, WorldRegion region, Action<RemoteSnapshot<PlayerInfoValue>> callback, bool marshalToFrameworkThread = true, CancellationToken cancellationToken = default(CancellationToken))
	{
		return ManagerBase<RemoteInteractionManager>.Instance().Observe<PlayerInfoQueryHandler, PlayerInfoKey, PlayerInfoValue>(new PlayerInfoKey(contentID, region), callback, marshalToFrameworkThread, cancellationToken);
	}

	public static ValueTask<PlayerInfoValue> GetFreshAsync(ulong contentID, WorldRegion region, CancellationToken cancellationToken = default(CancellationToken))
	{
		return ManagerBase<RemoteInteractionManager>.Instance().GetFreshAsync<PlayerInfoQueryHandler, PlayerInfoKey, PlayerInfoValue>(new PlayerInfoKey(contentID, region), cancellationToken);
	}

	public static void Invalidate(ulong contentID, WorldRegion region)
	{
		ManagerBase<RemoteInteractionManager>.Instance().Invalidate<PlayerInfoQueryHandler, PlayerInfoKey, PlayerInfoValue>(new PlayerInfoKey(contentID, region));
	}

	public RemotePlayerInfo()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
	}
}
