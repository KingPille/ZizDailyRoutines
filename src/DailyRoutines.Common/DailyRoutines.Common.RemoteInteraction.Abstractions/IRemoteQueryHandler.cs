using System;
using System.Threading;
using System.Threading.Tasks;

namespace DailyRoutines.Common.RemoteInteraction.Abstractions;

public interface IRemoteQueryHandler<in TKey, TValue> where TKey : notnull
{
	static abstract TimeSpan TTL { get; }

	static abstract TimeSpan FailureTTL { get; }

	static abstract ValueTask<TValue> FetchAsync(TKey key, CancellationToken cancellationToken);
}
