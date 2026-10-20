using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using OmenTools.Dalamud;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.OmenService;

public class SecureSaveHelper : OmenServiceBase<SecureSaveHelper>
{
	private record FileChannelEntry(Channel<string> Channel, CancellationTokenSource Cts, Task ProcessingTask)
	{
		public DateTime LastAccessed { get; set; } = DateTime.UtcNow;
	}

	private readonly ConcurrentDictionary<string, Lazy<FileChannelEntry>> pathEntries = new ConcurrentDictionary<string, Lazy<FileChannelEntry>>();

	private readonly CancellationTokenSource cancelSource = new CancellationTokenSource();

	public void WriteAllText(string path, string content)
	{
		string fullPath = Path.GetFullPath(path);
		while (true)
		{
			FileChannelEntry value = pathEntries.GetOrAdd(fullPath, (string p) => new Lazy<FileChannelEntry>(delegate
			{
				CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancelSource.Token);
				Channel<string> channel = Channel.CreateUnbounded<string>(new UnboundedChannelOptions
				{
					SingleReader = true
				});
				Task processingTask = ProcessFileQueueAsync(p, channel.Reader, cancellationTokenSource.Token);
				return new FileChannelEntry(channel, cancellationTokenSource, processingTask);
			})).Value;
			value.LastAccessed = DateTime.UtcNow;
			if (value.Channel.Writer.TryWrite(content))
			{
				break;
			}
			Thread.Yield();
		}
	}

	public SecureSaveHelper()
	{
		CleanIdleChannelsAsync();
	}

	protected override void Uninit()
	{
		cancelSource.Cancel();
		pathEntries.Clear();
	}

	private static async Task ProcessFileQueueAsync(string filePath, ChannelReader<string> reader, CancellationToken ct)
	{
		try
		{
			while (await reader.WaitToReadAsync(ct))
			{
				await Task.Delay(500, ct);
				string text = null;
				string item;
				while (reader.TryRead(out item))
				{
					text = item;
				}
				if (text != null)
				{
					await AtomicWriteAsync(filePath, text);
				}
			}
		}
		catch (OperationCanceledException)
		{
			string text2 = null;
			string item2;
			while (reader.TryRead(out item2))
			{
				text2 = item2;
			}
			if (text2 != null)
			{
				await AtomicWriteAsync(filePath, text2);
			}
		}
	}

	private static async Task AtomicWriteAsync(string filePath, string content)
	{
		string tempFile = $"{filePath}.{Guid.NewGuid()}.tmp";
		try
		{
			await File.WriteAllTextAsync(tempFile, content);
			if (File.Exists(filePath))
			{
				File.Replace(tempFile, filePath, null);
			}
			else
			{
				File.Move(tempFile, filePath);
			}
		}
		catch (Exception ex)
		{
			DLog.Error("尝试保存内容至 " + filePath + " 时发生错误", ex);
		}
		finally
		{
			if (File.Exists(tempFile))
			{
				File.Delete(tempFile);
			}
		}
	}

	private async Task CleanIdleChannelsAsync()
	{
		try
		{
			while (!cancelSource.IsCancellationRequested)
			{
				await Task.Delay(TimeSpan.FromMinutes(1L), cancelSource.Token);
				DateTime utcNow = DateTime.UtcNow;
				foreach (var (key, lazy2) in pathEntries)
				{
					if (lazy2.IsValueCreated)
					{
						FileChannelEntry value = lazy2.Value;
						if (utcNow - value.LastAccessed > TimeSpan.FromMinutes(1L) && pathEntries.TryRemove(key, out Lazy<FileChannelEntry> value2) && value2.IsValueCreated)
						{
							value2.Value.Channel.Writer.TryComplete();
						}
					}
				}
			}
		}
		catch (OperationCanceledException)
		{
		}
	}
}
