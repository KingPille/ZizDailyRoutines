using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DailyRoutines.Common.Runtime.Hosts;
using Newtonsoft.Json;
using OmenTools.Dalamud;
using OmenTools.Extensions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.Common.Manager.Abstractions;

public abstract class ManagerBase<T> : ManagerBase where T : ManagerBase<T>
{
	public static T Instance()
	{
		T val = ManagerHost.Current.Get<T>();
		if (val == null || !val.IsInitialized || val.IsStopping || val.IsDisposed)
		{
			throw new InvalidOperationException("管理器 " + typeof(T).Name + " 尚未注册或初始化");
		}
		return val;
	}
}
public abstract class ManagerBase
{
	private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

	private readonly ConcurrentDictionary<long, Task> backgroundTasks = new ConcurrentDictionary<long, Task>();

	private readonly AsyncLocal<long?> currentBackgroundTaskID = new AsyncLocal<long?>();

	private readonly TaskCompletionSource stoppedCompletionSource = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

	private int lifecycleState;

	private int activeBackgroundRegistrations;

	private int isCancellationTokenSourceDisposed;

	private long nextBackgroundTaskID;

	private const int STATE_CREATED = 0;

	private const int STATE_RUNNING = 1;

	private const int STATE_STOPPING = 2;

	private const int STATE_STOPPED = 3;

	public string ConfigFilePath
	{
		get
		{
			string text = Path.Join(DalamudServiceExtension.Instance().GetPluginConfigDirectory(), "Manager");
			Directory.CreateDirectory(text);
			return Path.Join(text, GetType().Name + ".json");
		}
	}

	public bool IsInitialized { get; private set; }

	public bool IsDisposed => Volatile.Read(in lifecycleState) == 3;

	public bool IsStopping => Volatile.Read(in lifecycleState) == 2;

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
				DLog.Error("在初始化管理器时发生错误: " + GetType().Name, ex);
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
				DLog.Error("在停止管理器时发生错误: " + GetType().Name, ex);
			}
			await DrainBackgroundTasksAsync();
			try
			{
				await Uninit();
			}
			catch (Exception ex2)
			{
				DLog.Error("在卸载管理器时发生错误: " + GetType().Name, ex2);
			}
		}
		finally
		{
			Interlocked.Exchange(ref lifecycleState, 3);
			stoppedCompletionSource.TrySetResult();
			TryDisposeCancellationTokenSource();
		}
	}

	public async Task PublicPostInitAsync()
	{
		if (Volatile.Read(in lifecycleState) != 1)
		{
			return;
		}
		try
		{
			await PostInit();
		}
		catch (Exception ex)
		{
			DLog.Error("在管理器后初始化时发生错误: " + GetType().Name, ex);
		}
	}

	protected virtual Task Init()
	{
		return Task.CompletedTask;
	}

	protected virtual Task PostInit()
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
			Task task = Task.Run(() => ExecuteBackgroundTaskAsync(taskID, operation), CancellationToken.None);
			backgroundTasks[taskID] = task;
			task.ContinueWith(delegate
			{
				backgroundTasks.TryRemove(taskID, out Task _);
				TryDisposeCancellationTokenSource();
			}, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
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
		long? currentTaskID = currentBackgroundTaskID.Value;
		while (true)
		{
			Task[] array = (from pair in backgroundTasks
				where pair.Key != currentTaskID
				select pair.Value).ToArray();
			if (array.Length == 0)
			{
				break;
			}
			await Task.WhenAll(array);
		}
	}

	private async Task ExecuteBackgroundTaskAsync(long taskID, Func<CancellationToken, Task> operation)
	{
		long? previousTaskID = currentBackgroundTaskID.Value;
		currentBackgroundTaskID.Value = taskID;
		try
		{
			await operation(cancellationTokenSource.Token);
		}
		catch
		{
		}
		finally
		{
			currentBackgroundTaskID.Value = previousTaskID;
		}
	}

	private void TryDisposeCancellationTokenSource()
	{
		if (Volatile.Read(in lifecycleState) == 3 && Volatile.Read(in activeBackgroundRegistrations) == 0 && backgroundTasks.IsEmpty && Interlocked.Exchange(ref isCancellationTokenSourceDisposed, 1) == 0)
		{
			cancellationTokenSource.Dispose();
		}
	}

	internal T? LoadConfig<T>() where T : ManagerConfig
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
			DLog.Error("为管理器加载配置失败: " + GetType().Name, ex);
			return null;
		}
	}

	internal void SaveConfig<T>(T config) where T : ManagerConfig
	{
		try
		{
			ArgumentNullException.ThrowIfNull(config, "config");
			string content = JsonConvert.SerializeObject((object)config, (Formatting)1, JsonSerializerSettingsExtension.GetShared());
			OmenServiceBase<SecureSaveHelper>.Instance().WriteAllText(ConfigFilePath, content);
		}
		catch (Exception ex)
		{
			DLog.Error("为管理器保存配置失败: " + GetType().Name, ex);
		}
	}
}
