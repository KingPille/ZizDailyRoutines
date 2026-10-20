using System;
using System.Threading;

namespace OmenTools.OmenService;

public sealed class SharedCancellationLease(CancellationToken token, Action? release = null) : IDisposable
{
	private Action? release = release;

	internal static SharedCancellationLease Canceled { get; } = new SharedCancellationLease(new CancellationToken(canceled: true));

	public CancellationToken Token { get; } = token;

	public void Dispose()
	{
		Interlocked.Exchange(ref release, null)?.Invoke();
	}
}
