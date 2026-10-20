using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Dalamud.Game;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Plugin.Services;
using Lumina.Data;
using OmenTools.Dalamud;
using OmenTools.Extensions;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.OmenService;

public class ImageHelper : OmenServiceBase<ImageHelper>
{
	private class ImageLoadingResult : IDisposable
	{
		private long lastAccessTimeTicks = OmenServiceBase<StandardTimeManager>.Instance().UTCNow.Ticks;

		private int stateValue;

		private int downloadQueued;

		private long failedUntilTicks;

		public readonly TaskCompletionSource<IDalamudTextureWrap?> CompletionSource = new TaskCompletionSource<IDalamudTextureWrap>(TaskCreationOptions.RunContinuationsAsynchronously);

		public ISharedImmediateTexture? ImmediateTexture { get; set; }

		public IDalamudTextureWrap? TextureWrap { get; set; }

		public DateTime LastAccessTime => new DateTime(lastAccessTimeTicks);

		public ImageLoadState State => (ImageLoadState)Volatile.Read(in stateValue);

		public bool IsCompleted
		{
			get
			{
				ImageLoadState state = State;
				if (state - 2 <= ImageLoadState.Ready)
				{
					return true;
				}
				return false;
			}
		}

		public IDalamudTextureWrap? Texture
		{
			get
			{
				if (TextureWrap != null)
				{
					return TextureWrap;
				}
				ISharedImmediateTexture? immediateTexture = ImmediateTexture;
				if (immediateTexture == null)
				{
					return null;
				}
				return immediateTexture.GetWrapOrEmpty();
			}
		}

		public void RefreshAccess()
		{
			Interlocked.Exchange(ref lastAccessTimeTicks, OmenServiceBase<StandardTimeManager>.Instance().UTCNow.Ticks);
		}

		public bool TryQueueDownload()
		{
			return Interlocked.CompareExchange(ref downloadQueued, 1, 0) == 0;
		}

		public bool TryMarkLoading()
		{
			return Interlocked.CompareExchange(ref stateValue, 1, 0) == 0;
		}

		public bool IsFailureCooldownActive(DateTime nowUTC)
		{
			if (State == ImageLoadState.Failed)
			{
				return nowUTC.Ticks < Volatile.Read(in failedUntilTicks);
			}
			return false;
		}

		public void TryCompleteByTexture(DateTime nowUTC, TimeSpan failedTTL)
		{
			if (Texture != null)
			{
				Volatile.Write(ref stateValue, 2);
				CompletionSource.TrySetResult(Texture);
			}
			else
			{
				TryCompleteFailure(nowUTC, failedTTL);
			}
		}

		public void TryCompleteFailure(DateTime nowUTC, TimeSpan failedTTL)
		{
			Volatile.Write(ref failedUntilTicks, nowUTC.Ticks + failedTTL.Ticks);
			Volatile.Write(ref stateValue, 3);
			CompletionSource.TrySetResult(null);
		}

		public void Dispose()
		{
			Volatile.Write(ref stateValue, 4);
			CompletionSource.TrySetCanceled();
			try
			{
				((IDisposable)TextureWrap)?.Dispose();
			}
			catch
			{
			}
			ImmediateTexture = null;
			TextureWrap = null;
		}
	}

	private readonly record struct ExpirationKey
	{
		public string? TextureURL { get; init; }

		public (uint ID, bool HQ, Language Language)? IconKey { get; init; }

		public bool IsTexture => TextureURL != null;

		public static ExpirationKey FromTexture(string textureURL)
		{
			return new ExpirationKey
			{
				TextureURL = textureURL
			};
		}

		public static ExpirationKey FromIcon((uint ID, bool HQ, Language Language) iconKey)
		{
			return new ExpirationKey
			{
				IconKey = iconKey
			};
		}
	}

	private record AddCommand(ExpirationKey Key, long Ticks) : ExpirationCommand;

	private record ClearCommand : ExpirationCommand;

	private abstract record ExpirationCommand;

	private enum ImageLoadState : byte
	{
		Pending,
		Loading,
		Ready,
		Failed,
		Disposed
	}

	private static readonly TimeSpan CacheTTL = TimeSpan.FromSeconds(30L);

	private static readonly TimeSpan FailedCacheTTL = TimeSpan.FromSeconds(5L);

	private readonly Channel<string> downloadChannel = Channel.CreateUnbounded<string>(new UnboundedChannelOptions
	{
		SingleReader = true,
		SingleWriter = false
	});

	private readonly Channel<ExpirationCommand> expirationChannel = Channel.CreateUnbounded<ExpirationCommand>(new UnboundedChannelOptions
	{
		SingleReader = true,
		SingleWriter = false
	});

	private readonly ConcurrentDictionary<string, ImageLoadingResult> cachedTextures = new ConcurrentDictionary<string, ImageLoadingResult>();

	private readonly ConcurrentDictionary<(uint ID, bool HQ, Language Language), ImageLoadingResult> cachedIcons = new ConcurrentDictionary<(uint, bool, Language), ImageLoadingResult>();

	[CompilerGenerated]
	private HttpClient _003CHTTPClient_003Ek__BackingField;

	private readonly CancellationTokenSource globalCancelSource = new CancellationTokenSource();

	private HttpClient HTTPClient
	{
		get
		{
			if (_003CHTTPClient_003Ek__BackingField != null)
			{
				return _003CHTTPClient_003Ek__BackingField;
			}
			return _003CHTTPClient_003Ek__BackingField = OmenServiceBase<HTTPClientHelper>.Instance().Get("OmenTools.ImageHelper", delegate(HttpClient client)
			{
				client.Timeout = TimeSpan.FromSeconds(30L);
			});
		}
	}

	public static IDalamudTextureWrap? GetGameIcon(uint iconID, bool isHQ = false)
	{
		if (!TryGetGameIcon(iconID, out IDalamudTextureWrap texture, isHQ))
		{
			return null;
		}
		return texture;
	}

	public static bool TryGetGameIcon(uint icon, [NotNullWhen(true)] out IDalamudTextureWrap? texture, bool isHQ = false)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
		GameIconLookup val = new GameIconLookup(icon, isHQ, true, (ClientLanguage?)null);
		ISharedImmediateTexture val2 = default(ISharedImmediateTexture);
		if (obj.TryGetFromGameIcon(ref val, ref val2))
		{
			texture = val2.GetWrapOrEmpty();
			return true;
		}
		texture = null;
		return false;
	}

	public IDalamudTextureWrap? GetGameLangIcon(uint iconID, bool isHQ = false)
	{
		if (!TryGetGameLangIcon(iconID, out IDalamudTextureWrap texture, isHQ))
		{
			return null;
		}
		return texture;
	}

	public IDalamudTextureWrap? GetImage(string urlOrPath)
	{
		if (!TryGetImage(urlOrPath, out IDalamudTextureWrap texture))
		{
			return null;
		}
		return texture;
	}

	public bool TryGetGameLangIcon(uint icon, [NotNullWhen(true)] out IDalamudTextureWrap? texture, bool isHQ = false)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		(uint, bool, Language) tuple = (icon, isHQ, GameState.ClientLanguage);
		if (cachedIcons.TryGetValue(tuple, out ImageLoadingResult value))
		{
			value.RefreshAccess();
			texture = value.Texture;
			return texture != null;
		}
		value = new ImageLoadingResult
		{
			ImmediateTexture = DalamudServiceExtension.Instance<ITextureProvider>().GetFromGame(GetIconTexturePath(icon, GameState.ClientLanguage))
		};
		value.TryCompleteByTexture(OmenServiceBase<StandardTimeManager>.Instance().UTCNow, FailedCacheTTL);
		if (cachedIcons.TryAdd(tuple, value))
		{
			AddToExpirationQueue(tuple);
		}
		texture = value.Texture;
		return texture != null;
	}

	public bool TryGetImage(string url, [NotNullWhen(true)] out IDalamudTextureWrap? texture)
	{
		texture = null;
		if (string.IsNullOrWhiteSpace(url))
		{
			return false;
		}
		ImageLoadingResult imageLoadingResult2;
		while (true)
		{
			DateTime uTCNow = OmenServiceBase<StandardTimeManager>.Instance().UTCNow;
			if (cachedTextures.TryGetValue(url, out ImageLoadingResult value))
			{
				value.RefreshAccess();
				if (value.State != ImageLoadState.Failed || value.IsFailureCooldownActive(uTCNow))
				{
					texture = value.Texture;
					return texture != null;
				}
				ImageLoadingResult imageLoadingResult = new ImageLoadingResult();
				imageLoadingResult.RefreshAccess();
				if (cachedTextures.TryUpdate(url, imageLoadingResult, value))
				{
					AddToExpirationQueue(url);
					QueueDownload(url, imageLoadingResult);
					return false;
				}
			}
			else
			{
				imageLoadingResult2 = new ImageLoadingResult();
				if (cachedTextures.TryAdd(url, imageLoadingResult2))
				{
					break;
				}
			}
		}
		AddToExpirationQueue(url);
		QueueDownload(url, imageLoadingResult2);
		return false;
	}

	public async Task<IDalamudTextureWrap?> GetImageAsync(string urlOrPath)
	{
		if (string.IsNullOrWhiteSpace(urlOrPath))
		{
			return null;
		}
		if (!cachedTextures.TryGetValue(urlOrPath, out ImageLoadingResult value))
		{
			TryGetImage(urlOrPath, out IDalamudTextureWrap _);
			if (!cachedTextures.TryGetValue(urlOrPath, out value))
			{
				return null;
			}
		}
		value.RefreshAccess();
		if (value.IsCompleted)
		{
			return value.Texture;
		}
		try
		{
			return await value.CompletionSource.Task;
		}
		catch
		{
			return null;
		}
	}

	protected override void Init()
	{
		ProcessDownloadsAsync(globalCancelSource.Token);
		CleanupLoopAsync(globalCancelSource.Token);
	}

	protected override void Uninit()
	{
		globalCancelSource.Cancel();
		ClearAll();
		globalCancelSource.Dispose();
		HTTPClient.Dispose();
	}

	private void ClearAll()
	{
		expirationChannel.Writer.TryWrite(new ClearCommand());
		foreach (ImageLoadingResult value in cachedTextures.Values)
		{
			value.Dispose();
		}
		cachedTextures.Clear();
		foreach (ImageLoadingResult value2 in cachedIcons.Values)
		{
			value2.Dispose();
		}
		cachedIcons.Clear();
	}

	private void AddToExpirationQueue(string textureURL)
	{
		expirationChannel.Writer.TryWrite(new AddCommand(ExpirationKey.FromTexture(textureURL), OmenServiceBase<StandardTimeManager>.Instance().UTCNow.Ticks + CacheTTL.Ticks));
	}

	private void AddToExpirationQueue((uint ID, bool HQ, Language Language) iconKey)
	{
		expirationChannel.Writer.TryWrite(new AddCommand(ExpirationKey.FromIcon(iconKey), OmenServiceBase<StandardTimeManager>.Instance().UTCNow.Ticks + CacheTTL.Ticks));
	}

	private void QueueDownload(string url, ImageLoadingResult result)
	{
	}

	private async Task ProcessDownloadsAsync(CancellationToken ct)
	{
		ChannelReader<string> reader = downloadChannel.Reader;
		try
		{
			await Parallel.ForEachAsync(reader.ReadAllAsync(ct), new ParallelOptions
			{
				MaxDegreeOfParallelism = 8,
				CancellationToken = ct
			}, async delegate(string url, CancellationToken token)
			{
				if (!cachedTextures.TryGetValue(url, out ImageLoadingResult result) || !result.TryMarkLoading())
				{
					return;
				}
				try
				{
					if (Uri.TryCreate(url, UriKind.Absolute, out Uri result2) && (result2.Scheme == Uri.UriSchemeHttp || result2.Scheme == Uri.UriSchemeHttps))
					{
						_ = HTTPClient;
						_ = token;
						throw null;
					}
					result.ImmediateTexture = (File.Exists(url) ? DalamudServiceExtension.Instance<ITextureProvider>().GetFromFile(url) : DalamudServiceExtension.Instance<ITextureProvider>().GetFromGame(url));
					result.TryCompleteByTexture(OmenServiceBase<StandardTimeManager>.Instance().UTCNow, FailedCacheTTL);
				}
				catch (Exception ex2) when (!(ex2 is OperationCanceledException))
				{
					DLog.Error("[ImageHelper] 加载资源失败: " + url, ex2);
					result.TryCompleteFailure(OmenServiceBase<StandardTimeManager>.Instance().UTCNow, FailedCacheTTL);
				}
			});
		}
		catch (OperationCanceledException)
		{
		}
	}

	private async Task CleanupLoopAsync(CancellationToken ct)
	{
		PriorityQueue<ExpirationKey, long> localQueue = new PriorityQueue<ExpirationKey, long>();
		ChannelReader<ExpirationCommand> reader = expirationChannel.Reader;
		Task<bool> readWaitTask = reader.WaitToReadAsync(ct).AsTask();
		while (!ct.IsCancellationRequested)
		{
			try
			{
				long num;
				if (localQueue.TryPeek(out var _, out var priority))
				{
					num = priority - OmenServiceBase<StandardTimeManager>.Instance().UTCNow.Ticks;
					if (num < 0)
					{
						num = 0L;
					}
				}
				else
				{
					num = -1L;
				}
				if (await Task.WhenAny((num == -1) ? Task.Delay(-1, ct) : Task.Delay(TimeSpan.FromTicks(num), ct), readWaitTask) == readWaitTask)
				{
					if (!(await readWaitTask))
					{
						break;
					}
					ExpirationCommand item;
					while (reader.TryRead(out item))
					{
						if (!(item is AddCommand addCommand))
						{
							if (item is ClearCommand)
							{
								localQueue.Clear();
							}
						}
						else
						{
							localQueue.Enqueue(addCommand.Key, addCommand.Ticks);
						}
					}
					readWaitTask = reader.WaitToReadAsync(ct).AsTask();
				}
				else
				{
					ExpirationKey element2;
					long priority2;
					while (localQueue.TryPeek(out element2, out priority2) && priority2 <= OmenServiceBase<StandardTimeManager>.Instance().UTCNow.Ticks)
					{
						localQueue.Dequeue();
						CheckAndProcessItem(element2, localQueue);
					}
				}
			}
			catch (OperationCanceledException)
			{
				break;
			}
			catch (Exception ex2)
			{
				DLog.Error("[ImageHelper] 缓存清理任务异常", ex2);
				if (readWaitTask.IsCompleted)
				{
					readWaitTask = reader.WaitToReadAsync(ct).AsTask();
				}
			}
		}
	}

	private void CheckAndProcessItem(ExpirationKey key, PriorityQueue<ExpirationKey, long> queue)
	{
		ImageLoadingResult value = null;
		bool flag = false;
		ImageLoadingResult value3;
		if (key.IsTexture && key.TextureURL != null)
		{
			if (cachedTextures.TryGetValue(key.TextureURL, out value) && OmenServiceBase<StandardTimeManager>.Instance().UTCNow - value.LastAccessTime > CacheTTL && cachedTextures.TryRemove(key.TextureURL, out ImageLoadingResult value2))
			{
				value2.Dispose();
				flag = true;
			}
		}
		else if (key.IconKey.HasValue && cachedIcons.TryGetValue(key.IconKey.Value, out value) && OmenServiceBase<StandardTimeManager>.Instance().UTCNow - value.LastAccessTime > CacheTTL && cachedIcons.TryRemove(key.IconKey.Value, out value3))
		{
			value3.Dispose();
			flag = true;
		}
		if (!flag && value != null)
		{
			queue.Enqueue(key, value.LastAccessTime.Ticks + CacheTTL.Ticks);
		}
	}

	private static string GetIconTexturePath(uint iconID, Language language)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected I4, but got Unknown
		string text = (language - 1) switch
		{
			0 => "ja", 
			1 => "en", 
			2 => "de", 
			3 => "fr", 
			4 => "chs", 
			5 => "cht", 
			6 => "ko", 
			7 => "tc", 
			_ => string.Empty, 
		};
		if (text.Length == 0)
		{
			return string.Empty;
		}
		return $"ui/icon/{iconID / 1000 * 1000:D6}/{text}/{iconID:D6}_hr1.tex";
	}
}
