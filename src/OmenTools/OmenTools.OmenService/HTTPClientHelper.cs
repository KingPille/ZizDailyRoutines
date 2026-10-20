using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.OmenService;

public class HTTPClientHelper : OmenServiceBase<HTTPClientHelper>
{
	private const int LIFECYCLE_ACTIVE = 0;

	private const int LIFECYCLE_UNINITIALIZING = 1;

	private readonly ConcurrentDictionary<string, HttpClient> clients = new ConcurrentDictionary<string, HttpClient>();

	private readonly ConcurrentDictionary<long, CancellationTokenSource> sharedCancellationLeases = new ConcurrentDictionary<long, CancellationTokenSource>();

	private CancellationTokenSource sharedCancellationSource = new CancellationTokenSource();

	private long sharedCancellationLeaseIdentity;

	private int lifecycleState;

	private int pendingAcquireCount;

	protected override void Uninit()
	{
		if (Interlocked.Exchange(ref lifecycleState, 1) == 0)
		{
			CancellationTokenSource source = Interlocked.Exchange(ref sharedCancellationSource, CreateCanceledSource());
			WaitPendingAcquireCompleted();
			CancellationTokenSource[] array = sharedCancellationLeases.Values.ToArray();
			sharedCancellationLeases.Clear();
			HttpClient[] array2 = clients.Values.ToArray();
			clients.Clear();
			TryCancel(source);
			CancellationTokenSource[] array3 = array;
			foreach (CancellationTokenSource source2 in array3)
			{
				TryCancel(source2);
				TryDispose(source2);
			}
			TryDispose(source);
			HttpClient[] array4 = array2;
			for (int i = 0; i < array4.Length; i++)
			{
				array4[i].Dispose();
			}
		}
	}

	public SharedCancellationLease AcquireSharedCancellation(CancellationToken cancellationToken = default(CancellationToken))
	{
		if (Volatile.Read(in lifecycleState) != 0)
		{
			return SharedCancellationLease.Canceled;
		}
		Interlocked.Increment(ref pendingAcquireCount);
		try
		{
			if (Volatile.Read(in lifecycleState) != 0)
			{
				return SharedCancellationLease.Canceled;
			}
			CancellationTokenSource cancellationTokenSource = Volatile.Read(in sharedCancellationSource);
			CancellationTokenSource leaseSource = (cancellationToken.CanBeCanceled ? CancellationTokenSource.CreateLinkedTokenSource(cancellationTokenSource.Token, cancellationToken) : CancellationTokenSource.CreateLinkedTokenSource(cancellationTokenSource.Token));
			long leaseID = Interlocked.Increment(ref sharedCancellationLeaseIdentity);
			sharedCancellationLeases[leaseID] = leaseSource;
			if (Volatile.Read(in lifecycleState) != 0)
			{
				if (sharedCancellationLeases.TryRemove(leaseID, out CancellationTokenSource value))
				{
					TryCancel(value);
					TryDispose(value);
				}
				return SharedCancellationLease.Canceled;
			}
			return new SharedCancellationLease(leaseSource.Token, delegate
			{
				sharedCancellationLeases.TryRemove(leaseID, out CancellationTokenSource _);
				TryDispose(leaseSource);
			});
		}
		catch (ObjectDisposedException)
		{
			return SharedCancellationLease.Canceled;
		}
		finally
		{
			Interlocked.Decrement(ref pendingAcquireCount);
		}
	}

	public HttpClient Get(string name = "default")
	{
		return clients.GetOrAdd(name, (string _) => CreateClient());
	}

	public HttpClient Get(HttpMessageHandler handler, string name = "default")
	{
		return clients.GetOrAdd("\0DailyRoutines.CustomHandler\0" + name, (string _, HttpMessageHandler state) => CreateClient(state), handler);
	}

	public HttpClient Get(string name, Action<HttpClient> configure)
	{
		return clients.GetOrAdd(name, (string _, Action<HttpClient> state) => CreateClient(null, state), configure);
	}

	public HttpClient Get(HttpMessageHandler handler, string name, Action<HttpClient> configure)
	{
		return clients.GetOrAdd(name, (string _, (HttpMessageHandler Handler, Action<HttpClient> Configure) state) => CreateClient(state.Handler, state.Configure), (handler, configure));
	}

	private static HttpClient CreateClient(HttpMessageHandler? handler = null, Action<HttpClient>? configure = null)
	{
		HttpClient httpClient = ((handler == null) ? new HttpClient() : new HttpClient(handler));
		httpClient.DefaultRequestVersion = HttpVersion.Version30;
		httpClient.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower;
		httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/118.0");
		configure?.Invoke(httpClient);
		return httpClient;
	}

	private static CancellationTokenSource CreateCanceledSource()
	{
		CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
		cancellationTokenSource.Cancel();
		return cancellationTokenSource;
	}

	private void WaitPendingAcquireCompleted()
	{
		SpinWait spinWait = default(SpinWait);
		while (Volatile.Read(in pendingAcquireCount) > 0)
		{
			spinWait.SpinOnce();
		}
	}

	private static void TryCancel(CancellationTokenSource source)
	{
		try
		{
			source.Cancel();
		}
		catch (ObjectDisposedException)
		{
		}
	}

	private static void TryDispose(CancellationTokenSource source)
	{
		try
		{
			source.Dispose();
		}
		catch (ObjectDisposedException)
		{
		}
	}
}
