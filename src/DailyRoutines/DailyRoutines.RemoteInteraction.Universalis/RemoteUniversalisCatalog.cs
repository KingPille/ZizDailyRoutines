using System.Collections.Generic;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.RemoteInteraction.Abstractions;
using DailyRoutines.Common.RemoteInteraction.Models;
using DailyRoutines.Manager;
using DailyRoutines.RemoteInteraction.Universalis.Handlers;
using DailyRoutines.RemoteInteraction.Universalis.Models.Responses;

namespace DailyRoutines.RemoteInteraction.Universalis;

public class RemoteUniversalisCatalog : RemoteInteractionBase
{
	private static readonly RemoteSingletonKey SingletonKey;

	public static bool TryGetDataCenters(out List<UniversalisDataCenter> dataCenters)
	{
		return ManagerBase<RemoteInteractionManager>.Instance().TryGet<UniversalisDataCentersQueryHandler, RemoteSingletonKey, List<UniversalisDataCenter>>(SingletonKey, out dataCenters);
	}

	public static bool TryGetWorlds(out List<UniversalisWorld> worlds)
	{
		return ManagerBase<RemoteInteractionManager>.Instance().TryGet<UniversalisWorldsQueryHandler, RemoteSingletonKey, List<UniversalisWorld>>(SingletonKey, out worlds);
	}

	public static bool TryGetMarketableItemIDs(out List<uint> itemIDs)
	{
		return ManagerBase<RemoteInteractionManager>.Instance().TryGet<UniversalisMarketableItemsQueryHandler, RemoteSingletonKey, List<uint>>(SingletonKey, out itemIDs);
	}

	public static RemoteSnapshot<List<UniversalisDataCenter>> GetDataCentersOrRequest()
	{
		return ManagerBase<RemoteInteractionManager>.Instance().GetOrRequest<UniversalisDataCentersQueryHandler, RemoteSingletonKey, List<UniversalisDataCenter>>(SingletonKey);
	}

	public static RemoteSnapshot<List<UniversalisWorld>> GetWorldsOrRequest()
	{
		return ManagerBase<RemoteInteractionManager>.Instance().GetOrRequest<UniversalisWorldsQueryHandler, RemoteSingletonKey, List<UniversalisWorld>>(SingletonKey);
	}

	public static RemoteSnapshot<List<uint>> GetMarketableItemIDsOrRequest()
	{
		return ManagerBase<RemoteInteractionManager>.Instance().GetOrRequest<UniversalisMarketableItemsQueryHandler, RemoteSingletonKey, List<uint>>(SingletonKey);
	}

	public RemoteUniversalisCatalog()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
	}

	static RemoteUniversalisCatalog()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
	}
}
