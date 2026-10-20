using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.RemoteInteraction.Abstractions;
using DailyRoutines.Common.RemoteInteraction.Enums;
using DailyRoutines.Common.RemoteInteraction.Models;
using DailyRoutines.Manager;
using DailyRoutines.RemoteInteraction.UsedNames.Handlers;
using DailyRoutines.RemoteInteraction.UsedNames.Models;

namespace DailyRoutines.RemoteInteraction.UsedNames;

public class RemoteUsedNames : RemoteInteractionBase
{
	public static RemoteSnapshot<List<UsedNamesChange>> GetOrRequest(ulong contentID, WorldRegion region)
	{
		return ManagerBase<RemoteInteractionManager>.Instance().GetOrRequest<UsedNamesQueryHandler, UsedNamesKey, List<UsedNamesChange>>(new UsedNamesKey(contentID, region));
	}

	public static bool TryGet(ulong contentID, WorldRegion region, out List<UsedNamesChange> value)
	{
		return ManagerBase<RemoteInteractionManager>.Instance().TryGet<UsedNamesQueryHandler, UsedNamesKey, List<UsedNamesChange>>(new UsedNamesKey(contentID, region), out value);
	}

	public static IDisposable Observe(ulong contentID, WorldRegion region, Action<RemoteSnapshot<List<UsedNamesChange>>> callback, bool marshalToFrameworkThread = true)
	{
		return ManagerBase<RemoteInteractionManager>.Instance().Observe<UsedNamesQueryHandler, UsedNamesKey, List<UsedNamesChange>>(new UsedNamesKey(contentID, region), callback, marshalToFrameworkThread);
	}

	public static ValueTask<List<UsedNamesChange>> GetFreshAsync(ulong contentID, WorldRegion region, CancellationToken cancellationToken = default(CancellationToken))
	{
		return ManagerBase<RemoteInteractionManager>.Instance().GetFreshAsync<UsedNamesQueryHandler, UsedNamesKey, List<UsedNamesChange>>(new UsedNamesKey(contentID, region), cancellationToken);
	}

	public static void Invalidate(ulong contentID, WorldRegion region)
	{
		ManagerBase<RemoteInteractionManager>.Instance().Invalidate<UsedNamesQueryHandler, UsedNamesKey, List<UsedNamesChange>>(new UsedNamesKey(contentID, region));
	}

	public RemoteUsedNames()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
	}
}
