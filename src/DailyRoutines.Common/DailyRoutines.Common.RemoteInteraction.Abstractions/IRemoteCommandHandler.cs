using System.Threading;
using System.Threading.Tasks;

namespace DailyRoutines.Common.RemoteInteraction.Abstractions;

public interface IRemoteCommandHandler<in TArgs, TResult>
{
	static abstract ValueTask<TResult> ExecuteAsync(TArgs args, CancellationToken cancellationToken);
}
