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

public class RemoteUniversalisTaxRates : RemoteInteractionBase
{
	public static UniversalisTaxRatesKey CreateKey(string world)
	{
		return new UniversalisTaxRatesKey(new UniversalisTaxRatesRequestParams
		{
			World = world
		});
	}

	public static RemoteSnapshot<UniversalisTaxRatesResponse> GetOrRequest(string world)
	{
		return ManagerBase<RemoteInteractionManager>.Instance().GetOrRequest<UniversalisTaxRatesQueryHandler, UniversalisTaxRatesKey, UniversalisTaxRatesResponse>(CreateKey(world));
	}

	public static IDisposable Observe(string world, Action<RemoteSnapshot<UniversalisTaxRatesResponse>> callback, bool marshalToFrameworkThread = true)
	{
		return ManagerBase<RemoteInteractionManager>.Instance().Observe<UniversalisTaxRatesQueryHandler, UniversalisTaxRatesKey, UniversalisTaxRatesResponse>(CreateKey(world), callback, marshalToFrameworkThread);
	}

	public static ValueTask<UniversalisTaxRatesResponse> GetFreshAsync(string world, CancellationToken cancellationToken = default(CancellationToken))
	{
		return ManagerBase<RemoteInteractionManager>.Instance().GetFreshAsync<UniversalisTaxRatesQueryHandler, UniversalisTaxRatesKey, UniversalisTaxRatesResponse>(CreateKey(world), cancellationToken);
	}

	public RemoteUniversalisTaxRates()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
	}
}
