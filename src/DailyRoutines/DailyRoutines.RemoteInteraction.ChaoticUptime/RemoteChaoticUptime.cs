using System;
using System.Threading;
using System.Threading.Tasks;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.RemoteInteraction.Abstractions;
using DailyRoutines.Common.RemoteInteraction.Models;
using DailyRoutines.Manager;
using DailyRoutines.RemoteInteraction.ChaoticUptime.Handlers;
using DailyRoutines.RemoteInteraction.ChaoticUptime.Models;

namespace DailyRoutines.RemoteInteraction.ChaoticUptime;

public class RemoteChaoticUptime : RemoteInteractionBase
{
	public static ChaoticUptimeKey CreateKey(string dataCenter)
	{
		return new ChaoticUptimeKey(dataCenter);
	}

	public static RemoteSnapshot<ChaoticUptimeValue> GetOrRequest(string dataCenter, CancellationToken cancellationToken = default(CancellationToken))
	{
		return ManagerBase<RemoteInteractionManager>.Instance().GetOrRequest<ChaoticUptimeQueryHandler, ChaoticUptimeKey, ChaoticUptimeValue>(new ChaoticUptimeKey(dataCenter), cancellationToken);
	}

	public static bool TryGet(string dataCenter, out ChaoticUptimeValue value)
	{
		return ManagerBase<RemoteInteractionManager>.Instance().TryGet<ChaoticUptimeQueryHandler, ChaoticUptimeKey, ChaoticUptimeValue>(new ChaoticUptimeKey(dataCenter), out value);
	}

	public static IDisposable Observe(string dataCenter, Action<RemoteSnapshot<ChaoticUptimeValue>> callback, bool marshalToFrameworkThread = true, CancellationToken cancellationToken = default(CancellationToken))
	{
		return ManagerBase<RemoteInteractionManager>.Instance().Observe<ChaoticUptimeQueryHandler, ChaoticUptimeKey, ChaoticUptimeValue>(new ChaoticUptimeKey(dataCenter), callback, marshalToFrameworkThread, cancellationToken);
	}

	public static ValueTask<ChaoticUptimeValue> GetFreshAsync(string dataCenter, CancellationToken cancellationToken = default(CancellationToken))
	{
		return ManagerBase<RemoteInteractionManager>.Instance().GetFreshAsync<ChaoticUptimeQueryHandler, ChaoticUptimeKey, ChaoticUptimeValue>(new ChaoticUptimeKey(dataCenter), cancellationToken);
	}

	public static void Invalidate(string dataCenter)
	{
		ManagerBase<RemoteInteractionManager>.Instance().Invalidate<ChaoticUptimeQueryHandler, ChaoticUptimeKey, ChaoticUptimeValue>(new ChaoticUptimeKey(dataCenter));
	}

	public RemoteChaoticUptime()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
	}
}
