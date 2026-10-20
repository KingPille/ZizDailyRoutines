using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Dalamud.Plugin.Services;
using Newtonsoft.Json;
using OmenTools;
using OmenTools.Dalamud;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Extensions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;

namespace DailyRoutines.Common.DataUploader.Abstractions;

public abstract class DataUploaderBase
{
	private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

	private readonly ConcurrentDictionary<long, Task> backgroundTasks = new ConcurrentDictionary<long, Task>();

	private readonly TaskCompletionSource stoppedCompletionSource = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

	private int lifecycleState;

	private int activeBackgroundRegistrations;

	private long nextBackgroundTaskID;

	protected static readonly Throttler<string> Throttler = new Throttler<string>();

	private const int STATE_CREATED = 0;

	private const int STATE_RUNNING = 1;

	private const int STATE_STOPPING = 2;

	private const int STATE_STOPPED = 3;

	public string ConfigFilePath
	{
		get
		{
			string text = Path.Join(DalamudServiceExtension.Instance().GetPluginConfigDirectory(), "DataUploader");
			Directory.CreateDirectory(text);
			return Path.Join(text, GetType().Name + ".json");
		}
	}

	public bool IsDisposed => Volatile.Read(in lifecycleState) == 3;

	public bool IsStopping => Volatile.Read(in lifecycleState) == 2;

	public bool IsInitialized { get; private set; }

	public async Task PublicInitAsync()
	{
		if (Interlocked.CompareExchange(ref lifecycleState, 1, 0) == 0)
		{
			try
			{
				await Init();
				IsInitialized = true;
			}
			catch (Exception ex)
			{
				DLog.Error("初始化数据上传器失败", ex);
				await PublicUninitAsync();
			}
		}
	}

	public async Task PublicUninitAsync()
	{
		if (!TryBeginStop())
		{
			await stoppedCompletionSource.Task;
			return;
		}
		try
		{
			try
			{
				await Stop();
			}
			catch (Exception ex)
			{
				DLog.Error("停止数据上传器失败", ex);
			}
			await DrainBackgroundTasksAsync();
			try
			{
				await Uninit();
			}
			catch (Exception ex2)
			{
				DLog.Error("卸载数据上传器失败", ex2);
			}
		}
		finally
		{
			cancellationTokenSource.Dispose();
			Interlocked.Exchange(ref lifecycleState, 3);
			stoppedCompletionSource.TrySetResult();
		}
	}

	protected virtual Task Init()
	{
		return Task.CompletedTask;
	}

	protected virtual Task Stop()
	{
		return Task.CompletedTask;
	}

	protected virtual Task Uninit()
	{
		return Task.CompletedTask;
	}

	protected bool RunBackground(Func<CancellationToken, Task> operation)
	{
		ArgumentNullException.ThrowIfNull(operation, "operation");
		if (Volatile.Read(in lifecycleState) != 1)
		{
			return false;
		}
		Interlocked.Increment(ref activeBackgroundRegistrations);
		try
		{
			if (Volatile.Read(in lifecycleState) != 1)
			{
				return false;
			}
			long taskID = Interlocked.Increment(ref nextBackgroundTaskID);
			Task task = Task.Run(() => ExecuteBackgroundTaskAsync(operation), CancellationToken.None);
			backgroundTasks[taskID] = task;
			task.ContinueWith((Task completedTask) => backgroundTasks.TryRemove(taskID, out Task _), CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
			return true;
		}
		finally
		{
			Interlocked.Decrement(ref activeBackgroundRegistrations);
		}
	}

	private bool TryBeginStop()
	{
		int num;
		do
		{
			num = Volatile.Read(in lifecycleState);
			if ((uint)(num - 2) <= 1u)
			{
				return false;
			}
		}
		while (Interlocked.CompareExchange(ref lifecycleState, 2, num) != num);
		return true;
	}

	private async Task DrainBackgroundTasksAsync()
	{
		try
		{
			await cancellationTokenSource.CancelAsync();
		}
		catch
		{
		}
		while (Volatile.Read(in activeBackgroundRegistrations) != 0)
		{
			await Task.Yield();
		}
		while (!backgroundTasks.IsEmpty)
		{
			await Task.WhenAll(backgroundTasks.Values);
		}
	}

	private async Task ExecuteBackgroundTaskAsync(Func<CancellationToken, Task> operation)
	{
		try
		{
			await operation(cancellationTokenSource.Token);
		}
		catch
		{
		}
	}

	internal T? LoadConfig<T>() where T : DataUploaderConfig
	{
		try
		{
			if (!File.Exists(ConfigFilePath))
			{
				return null;
			}
			return JsonConvert.DeserializeObject<T>(File.ReadAllText(ConfigFilePath), JsonSerializerSettingsExtension.GetShared());
		}
		catch (Exception ex)
		{
			DLog.Error("为数据上传器加载配置失败: " + GetType().Name, ex);
			return null;
		}
	}

	internal void SaveConfig<T>(T config) where T : DataUploaderConfig
	{
		try
		{
			ArgumentNullException.ThrowIfNull(config, "config");
			string content = JsonConvert.SerializeObject((object)config, (Formatting)1, JsonSerializerSettingsExtension.GetShared());
			OmenServiceBase<SecureSaveHelper>.Instance().WriteAllText(ConfigFilePath, content);
		}
		catch (Exception ex)
		{
			DLog.Error("为数据上传器加载配置失败: " + GetType().Name, ex);
		}
	}

	protected static bool IsPlayerReady()
	{
		if (GameState.IsLoggedIn && IOmenDalamudService<IObjectTable>.Instance().LocalPlayer != null)
		{
			return !IConditionExtension.get_IsBoundByDuty(DalamudServiceExtension.Instance<ICondition>());
		}
		return false;
	}
}
