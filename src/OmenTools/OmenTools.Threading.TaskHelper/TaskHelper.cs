using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Dalamud.Plugin.Services;
using OmenTools.Dalamud;
using OmenTools.Extensions;
using OmenTools.Threading.TaskHelper.Enums;
using OmenTools.Threading.TaskHelper.Models;

namespace OmenTools.Threading.TaskHelper;

public class TaskHelper : IDisposable
{
	private interface ITaskHelperMessage
	{
	}

	private readonly record struct AddTaskMessage(TaskHelperTask Task, int Weight) : ITaskHelperMessage;

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private readonly record struct AbortMessage : ITaskHelperMessage;

	private readonly record struct AddQueueMessage(int Weight) : ITaskHelperMessage;

	private readonly record struct RemoveQueueMessage(int Weight) : ITaskHelperMessage;

	private readonly record struct RemoveQueueTasksMessage(int Weight) : ITaskHelperMessage;

	private readonly record struct RemoveQueueFirstTaskMessage(int Weight) : ITaskHelperMessage;

	private readonly record struct RemoveQueueLastTaskMessage(int Weight) : ITaskHelperMessage;

	private readonly record struct RemoveQueueFirstNTasksMessage(int Weight, int Count) : ITaskHelperMessage;

	private volatile int pendingTaskCount;

	private volatile int queueTaskCount;

	private volatile bool isBusyState;

	public string CurrentTaskName => CurrentTask?.GetName() ?? string.Empty;

	public int CurrentTasksCount => queueTaskCount + ((!(CurrentTask == null)) ? 1 : 0) + pendingTaskCount;

	public bool IsBusy => CurrentTasksCount > 0;

	public Action? EnterBusyAction { get; set; }

	public Action? LeaveBusyAction { get; set; }

	public TaskAbortBehaviour TimeoutBehaviour { get; set; }

	public TaskAbortBehaviour ExceptionBehaviour { get; set; }

	public Action? TimeoutAction { get; set; }

	public Action? ExceptionAction { get; set; }

	public Func<bool>? MoveToNextCheckFunc { get; set; }

	public bool ShowDebug { get; set; }

	public int TimeoutMS { get; set; } = 10000;

	public int RetryIntervalMS { get; set; }

	public int TaskIntervalMS { get; set; }

	public bool IsDisposed { get; private set; }

	private TaskHelperTask? CurrentTask { get; set; }

	private SortedSet<TaskHelperQueue> Queues { get; } = new SortedSet<TaskHelperQueue>
	{
		new TaskHelperQueue(1),
		new TaskHelperQueue(0)
	};

	private ConcurrentDictionary<int, int> QueueCounts { get; } = new ConcurrentDictionary<int, int>();

	private Channel<ITaskHelperMessage> TaskChannel { get; } = CreateTaskChannel();

	private CancellationTokenSource CancelSource { get; } = new CancellationTokenSource();

	private static Channel<ITaskHelperMessage> CreateTaskChannel()
	{
		return Channel.CreateUnbounded<ITaskHelperMessage>(new UnboundedChannelOptions
		{
			SingleReader = true
		});
	}

	public TaskHelper()
	{
		DService.Instance().RegTaskHelper(this);
		QueueCounts.TryAdd(1, 0);
		QueueCounts.TryAdd(0, 0);
		ProcessChannelAsync(CancelSource.Token);
	}

	public void Dispose()
	{
		if (IsDisposed)
		{
			return;
		}
		IsDisposed = true;
		DService.Instance().UnregTaskHelper(this);
		try
		{
			CancelSource.Cancel();
			CancelSource.Dispose();
		}
		catch
		{
		}
		TaskChannel.Writer.TryComplete();
		try
		{
			CurrentTask?.SuppressCancellationException();
			CurrentTask?.CancelToken?.Cancel();
			CurrentTask?.CancelToken?.Dispose();
		}
		catch
		{
		}
		foreach (TaskHelperQueue queue in Queues)
		{
			foreach (TaskHelperTask task in queue.Tasks)
			{
				try
				{
					task.SuppressCancellationException();
					task.CancelToken?.Cancel();
					task.CancelToken?.Dispose();
				}
				catch
				{
				}
			}
		}
		Queues.Clear();
		QueueCounts.Clear();
	}

	private async Task ProcessChannelAsync(CancellationToken ct)
	{
		try
		{
			while (!ct.IsCancellationRequested)
			{
				if (CurrentTask == null && queueTaskCount == 0 && pendingTaskCount == 0)
				{
					await TaskChannel.Reader.WaitToReadAsync(ct).ConfigureAwait(continueOnCapturedContext: false);
				}
				await DalamudServiceExtension.Instance<IFramework>().Run((Func<Task>)async delegate
				{
					SyncPendingTasks();
					if (CurrentTask == null)
					{
						ProcessNextTask();
					}
					if (CurrentTask != null)
					{
						if (CurrentTask.IsAsync)
						{
							await ExecuteCurrentTaskAsync();
						}
						else
						{
							ExecuteCurrentTask();
						}
						if (CurrentTask != null && RetryIntervalMS > 0)
						{
							await Task.Delay(RetryIntervalMS, ct).ConfigureAwait(continueOnCapturedContext: false);
						}
						if (CurrentTask == null && TaskIntervalMS > 0)
						{
							await Task.Delay(TaskIntervalMS, ct).ConfigureAwait(continueOnCapturedContext: false);
						}
					}
					UpdateBusyState();
				}, ct).ConfigureAwait(continueOnCapturedContext: false);
			}
		}
		catch (OperationCanceledException)
		{
		}
		catch (Exception value)
		{
			LogWarning($"[TaskHelper] 异步轮询出现异常: {value}");
		}
	}

	private void UpdateBusyState()
	{
		bool isBusy = IsBusy;
		if (isBusy == isBusyState)
		{
			return;
		}
		isBusyState = isBusy;
		try
		{
			if (isBusy)
			{
				EnterBusyAction?.Invoke();
			}
			else
			{
				LeaveBusyAction?.Invoke();
			}
		}
		catch (Exception ex)
		{
			DLog.Error("执行 " + (isBusy ? "进入繁忙" : "离开繁忙") + " 状态时的自定义逻辑发生错误", ex);
		}
	}

	private void SyncPendingTasks()
	{
		ITaskHelperMessage item;
		while (TaskChannel.Reader.TryRead(out item))
		{
			if (!(item is AddTaskMessage addTaskMessage))
			{
				if (!(item is AbortMessage))
				{
					if (item is AddQueueMessage)
					{
						AddQueueMessage addQueueMessage = (AddQueueMessage)(object)item;
						if (Queues.All((TaskHelperQueue q) => q.Weight != addQueueMessage.Weight))
						{
							Queues.Add(new TaskHelperQueue(addQueueMessage.Weight));
							QueueCounts.TryAdd(addQueueMessage.Weight, 0);
						}
					}
					else if (!(item is RemoveQueueMessage removeQueueMessage))
					{
						if (!(item is RemoveQueueTasksMessage removeQueueTasksMessage))
						{
							if (!(item is RemoveQueueFirstTaskMessage removeQueueFirstTaskMessage))
							{
								if (!(item is RemoveQueueLastTaskMessage removeQueueLastTaskMessage))
								{
									if (item is RemoveQueueFirstNTasksMessage removeQueueFirstNTasksMessage)
									{
										PerformRemoveQueueFirstNTasks(removeQueueFirstNTasksMessage.Weight, removeQueueFirstNTasksMessage.Count);
									}
								}
								else
								{
									PerformRemoveQueueLastTask(removeQueueLastTaskMessage.Weight);
								}
							}
							else
							{
								PerformRemoveQueueFirstTask(removeQueueFirstTaskMessage.Weight);
							}
						}
						else
						{
							PerformRemoveQueueTasks(removeQueueTasksMessage.Weight);
						}
					}
					else
					{
						PerformRemoveQueue(removeQueueMessage.Weight);
					}
				}
				else
				{
					PerformAbort();
				}
			}
			else
			{
				Interlocked.Decrement(ref pendingTaskCount);
				TaskHelperQueue taskHelperQueue = new TaskHelperQueue(addTaskMessage.Weight);
				if (!Queues.TryGetValue(taskHelperQueue, out TaskHelperQueue actualValue))
				{
					actualValue = taskHelperQueue;
					Queues.Add(actualValue);
					QueueCounts.TryAdd(addTaskMessage.Weight, 0);
				}
				actualValue.Tasks.Add(addTaskMessage.Task);
				Interlocked.Increment(ref queueTaskCount);
				QueueCounts.AddOrUpdate(addTaskMessage.Weight, 1, (int _, int c) => c + 1);
			}
		}
	}

	private void ProcessNextTask()
	{
		Func<bool> moveToNextCheckFunc = MoveToNextCheckFunc;
		if (moveToNextCheckFunc != null && !moveToNextCheckFunc())
		{
			return;
		}
		foreach (TaskHelperQueue queue in Queues)
		{
			if (queue.Tasks.TryDequeue<TaskHelperTask>(out TaskHelperTask result))
			{
				Interlocked.Decrement(ref queueTaskCount);
				QueueCounts.AddOrUpdate(queue.Weight, 0, (int _, int c) => Math.Max(0, c - 1));
				CurrentTask = result;
				CurrentTask.StartTime = Stopwatch.GetTimestamp();
				Log("开始执行任务: " + CurrentTask.GetName());
				break;
			}
		}
	}

	private void ExecuteCurrentTask()
	{
		try
		{
			if (!CheckAndHandleTimeout() && CurrentTask.Action())
			{
				Log("已完成任务: " + CurrentTask.GetName());
				CurrentTask = null;
			}
		}
		catch (Exception ex)
		{
			HandleTaskError(ex);
		}
	}

	private async Task ExecuteCurrentTaskAsync()
	{
		TaskHelperTask task = CurrentTask;
		CancellationToken cancelToken = task?.CancelToken?.Token ?? CancellationToken.None;
		try
		{
			if (!(task == null) && !CheckAndHandleTimeout() && await task.AsyncAction(cancelToken).ConfigureAwait(continueOnCapturedContext: false))
			{
				Log("已完成任务: " + task.GetName());
				CurrentTask = null;
			}
		}
		catch (OperationCanceledException) when (cancelToken.IsCancellationRequested && task.SuppressesCancellationException)
		{
			if (CurrentTask == task)
			{
				CurrentTask = null;
			}
		}
		catch (Exception ex2)
		{
			HandleTaskError(ex2);
		}
	}

	private bool CheckAndHandleTimeout()
	{
		if (CurrentTask == null)
		{
			return false;
		}
		int num = ((CurrentTask.TimeoutMS <= 0) ? TimeoutMS : CurrentTask.TimeoutMS);
		if (num <= 0)
		{
			return false;
		}
		if (Stopwatch.GetElapsedTime(CurrentTask.StartTime).TotalMilliseconds <= (double)num)
		{
			return false;
		}
		TaskAbortBehaviour behaviour = CurrentTask.TimeoutBehaviour ?? TimeoutBehaviour;
		string reason = "任务 " + CurrentTask.GetName() + " 执行时间过长";
		Action extraAction = CurrentTask.TimeoutAction ?? TimeoutAction;
		ExecuteTaskAbortBehaviour(behaviour, reason, CurrentTask, extraAction);
		return true;
	}

	private void HandleTaskError(Exception? ex = null)
	{
		if (!(CurrentTask == null))
		{
			TaskAbortBehaviour behaviour = CurrentTask.ExceptionBehaviour ?? ExceptionBehaviour;
			string text = "执行任务 " + CurrentTask.GetName() + " 过程中出现错误";
			Action extraAction = CurrentTask.ExceptionAction ?? ExceptionAction;
			if (ex != null)
			{
				DLog.Error(text, ex);
			}
			else
			{
				DLog.Error(text);
			}
			ExecuteTaskAbortBehaviour(behaviour, text, CurrentTask, extraAction);
		}
	}

	private void ExecuteTaskAbortBehaviour(TaskAbortBehaviour behaviour, string reason, TaskHelperTask task, Action? extraAction = null)
	{
		if (task == null)
		{
			return;
		}
		switch (behaviour)
		{
		case TaskAbortBehaviour.AbortAll:
			LogWarning("放弃了所有任务 (原因: " + reason + ")" + ((extraAction == null) ? string.Empty : "\n开始执行该原因出现时的自定义逻辑"));
			PerformAbort();
			break;
		case TaskAbortBehaviour.AbortCurrent:
			LogWarning("放弃了当前任务 (原因: " + reason + ")" + ((extraAction == null) ? string.Empty : "\n开始执行该原因出现时的自定义逻辑"));
			try
			{
				CurrentTask?.CancelToken?.Cancel();
				CurrentTask?.CancelToken?.Dispose();
			}
			catch
			{
			}
			CurrentTask = null;
			break;
		default:
			throw new ArgumentOutOfRangeException("behaviour");
		}
		try
		{
			extraAction?.Invoke();
		}
		catch (Exception ex)
		{
			DLog.Error("在执行 " + reason + " 原因出现时应执行的自定义逻辑时发生错误", ex);
		}
	}

	private void Log(string message)
	{
		if (ShowDebug)
		{
			DLog.Debug(message);
		}
		else
		{
			DLog.Verbose(message);
		}
	}

	private void LogWarning(string message)
	{
		if (ShowDebug)
		{
			DLog.Warning(message);
		}
		else
		{
			DLog.Verbose(message);
		}
	}

	private void PerformAbort()
	{
		foreach (TaskHelperQueue queue in Queues)
		{
			foreach (TaskHelperTask task in queue.Tasks)
			{
				try
				{
					task.CancelToken?.Cancel();
					task.CancelToken?.Dispose();
				}
				catch
				{
				}
			}
			queue.Tasks.Clear();
		}
		queueTaskCount = 0;
		QueueCounts.Clear();
		CurrentTask?.SuppressCancellationException();
		CurrentTask?.CancelToken?.Cancel();
		CurrentTask?.CancelToken?.Dispose();
		CurrentTask = null;
	}

	public void Abort()
	{
		TaskChannel.Writer.TryWrite(default(AbortMessage));
	}

	public void AddQueue(int weight)
	{
		TaskChannel.Writer.TryWrite(new AddQueueMessage(weight));
	}

	private void PerformRemoveQueue(int weight)
	{
		LogWarning($"移除了权重 {weight} 队列");
		TaskHelperQueue taskHelperQueue = Queues.FirstOrDefault((TaskHelperQueue q) => q.Weight == weight);
		if (!(taskHelperQueue == null))
		{
			Interlocked.Add(ref queueTaskCount, -taskHelperQueue.Tasks.Count);
			QueueCounts.TryRemove(weight, out var _);
			Queues.Remove(taskHelperQueue);
		}
	}

	public void RemoveQueue(int weight)
	{
		TaskChannel.Writer.TryWrite(new RemoveQueueMessage(weight));
	}

	private void PerformRemoveQueueTasks(int weight)
	{
		LogWarning($"清除了权重 {weight} 队列中的所有任务");
		TaskHelperQueue taskHelperQueue = Queues.FirstOrDefault((TaskHelperQueue q) => q.Weight == weight);
		if (taskHelperQueue != null)
		{
			Interlocked.Add(ref queueTaskCount, -taskHelperQueue.Tasks.Count);
			taskHelperQueue.Tasks.Clear();
			QueueCounts.AddOrUpdate(weight, 0, (int _, int _) => 0);
		}
	}

	public void RemoveQueueTasks(int weight)
	{
		TaskChannel.Writer.TryWrite(new RemoveQueueTasksMessage(weight));
	}

	private void PerformRemoveQueueFirstTask(int weight)
	{
		LogWarning($"移除了权重 {weight} 队列中的第一个任务");
		if ((Queues.FirstOrDefault((TaskHelperQueue q) => q.Weight == weight)?.Tasks ?? new List<TaskHelperTask>()).TryDequeue<TaskHelperTask>(out TaskHelperTask _))
		{
			Interlocked.Decrement(ref queueTaskCount);
			QueueCounts.AddOrUpdate(weight, 0, (int _, int c) => Math.Max(0, c - 1));
		}
	}

	public void RemoveQueueFirstTask(int weight)
	{
		TaskChannel.Writer.TryWrite(new RemoveQueueFirstTaskMessage(weight));
	}

	private void PerformRemoveQueueLastTask(int weight)
	{
		TaskHelperQueue taskHelperQueue = Queues.FirstOrDefault((TaskHelperQueue q) => q.Weight == weight);
		if ((taskHelperQueue?.Tasks ?? new List<TaskHelperTask>()).Count > 0)
		{
			LogWarning($"清除了权重 {weight} 队列中的最后一个任务");
			taskHelperQueue.Tasks.RemoveAt(taskHelperQueue.Tasks.Count - 1);
			Interlocked.Decrement(ref queueTaskCount);
			QueueCounts.AddOrUpdate(weight, 0, (int _, int c) => Math.Max(0, c - 1));
		}
	}

	public void RemoveQueueLastTask(int weight)
	{
		TaskChannel.Writer.TryWrite(new RemoveQueueLastTaskMessage(weight));
	}

	private void PerformRemoveQueueFirstNTasks(int weight, int count)
	{
		TaskHelperQueue taskHelperQueue = Queues.FirstOrDefault((TaskHelperQueue q) => q.Weight == weight);
		if ((taskHelperQueue?.Tasks ?? new List<TaskHelperTask>()).Count > 0)
		{
			LogWarning($"清除了权重 {weight} 队列中的起始 {count} 个任务");
			int actualCountToRemove = Math.Min(count, taskHelperQueue.Tasks.Count);
			taskHelperQueue.Tasks.RemoveRange(0, actualCountToRemove);
			Interlocked.Add(ref queueTaskCount, -actualCountToRemove);
			QueueCounts.AddOrUpdate(weight, 0, (int _, int c) => Math.Max(0, c - actualCountToRemove));
		}
	}

	public void RemoveQueueFirstNTasks(int weight, int count)
	{
		TaskChannel.Writer.TryWrite(new RemoveQueueFirstNTasksMessage(weight, count));
	}

	public int GetQueueTaskCount(int weight)
	{
		return QueueCounts.GetValueOrDefault(weight, 0);
	}

	public void Enqueue(TaskHelperTask task, int weight = 0)
	{
		if ((object)task == null)
		{
			throw new ArgumentNullException("task");
		}
		if (task.Action == null && task.AsyncAction == null)
		{
			throw new ArgumentException("任务 " + task.GetName() + " 的执行逻辑为空 (Action 和 AsyncAction 均为 null)");
		}
		if ((object)task != null && task.Action != null && task.AsyncAction != null)
		{
			throw new ArgumentException("任务 " + task.GetName() + " 的执行逻辑不明确 (Action 和 AsyncAction 均不为 null)");
		}
		if (TaskChannel.Writer.TryWrite(new AddTaskMessage(task, weight)))
		{
			Interlocked.Increment(ref pendingTaskCount);
		}
	}

	public void Enqueue(Func<bool> task, string? name = null, int timeoutMS = 0, TaskAbortBehaviour? timeoutBehaviour = null, TaskAbortBehaviour? exceptionBehaviour = null, Action? timeoutAction = null, Action? exceptionAction = null, int weight = 0)
	{
		if (TaskChannel.Writer.TryWrite(new AddTaskMessage(new TaskHelperTask(task, name, timeoutMS, timeoutBehaviour, exceptionBehaviour, timeoutAction, exceptionAction), weight)))
		{
			Interlocked.Increment(ref pendingTaskCount);
		}
	}

	public void Enqueue(Action task, string? name = null, int timeoutMS = 0, TaskAbortBehaviour? timeoutBehaviour = null, TaskAbortBehaviour? exceptionBehaviour = null, Action? timeoutAction = null, Action? exceptionAction = null, int weight = 0)
	{
		Enqueue(delegate
		{
			task();
			return true;
		}, name, timeoutMS, timeoutBehaviour, exceptionBehaviour, timeoutAction, exceptionAction, weight);
	}

	public void Enqueue(Func<bool?> task, string? name = null, int timeoutMS = 0, TaskAbortBehaviour? timeoutBehaviour = null, TaskAbortBehaviour? exceptionBehaviour = null, Action? timeoutAction = null, Action? exceptionAction = null, int weight = 0)
	{
		Enqueue(() => task() == true, name, timeoutMS, timeoutBehaviour, exceptionBehaviour, timeoutAction, exceptionAction, weight);
	}

	public void EnqueueAsync(Func<CancellationToken, Task<bool>> asyncTask, string? name = null, int timeoutMS = 0, TaskAbortBehaviour? timeoutBehaviour = null, TaskAbortBehaviour? exceptionBehaviour = null, Action? timeoutAction = null, Action? exceptionAction = null, int weight = 0)
	{
		if (TaskChannel.Writer.TryWrite(new AddTaskMessage(new TaskHelperTask(asyncTask, name, timeoutMS, timeoutBehaviour, exceptionBehaviour, timeoutAction, exceptionAction), weight)))
		{
			Interlocked.Increment(ref pendingTaskCount);
		}
	}

	public void EnqueueAsync(Func<CancellationToken, Task> asyncTask, string? name = null, int timeoutMS = 0, TaskAbortBehaviour? timeoutBehaviour = null, TaskAbortBehaviour? exceptionBehaviour = null, Action? timeoutAction = null, Action? exceptionAction = null, int weight = 0)
	{
		EnqueueAsync(async delegate(CancellationToken ct)
		{
			await asyncTask(ct);
			return true;
		}, name, timeoutMS, timeoutBehaviour, exceptionBehaviour, timeoutAction, exceptionAction, weight);
	}

	public void EnqueueAsync(Func<CancellationToken, Task<bool?>> asyncTask, string? name = null, int timeoutMS = 0, TaskAbortBehaviour? timeoutBehaviour = null, TaskAbortBehaviour? exceptionBehaviour = null, Action? timeoutAction = null, Action? exceptionAction = null, int weight = 0)
	{
		EnqueueAsync(async (CancellationToken ct) => await asyncTask(ct) == true, name, timeoutMS, timeoutBehaviour, exceptionBehaviour, timeoutAction, exceptionAction, weight);
	}

	public void EnqueueAsync(Func<Task<bool>> asyncTask, string? name = null, int timeoutMS = 0, TaskAbortBehaviour? timeoutBehaviour = null, TaskAbortBehaviour? exceptionBehaviour = null, Action? timeoutAction = null, Action? exceptionAction = null, int weight = 0)
	{
		EnqueueAsync((CancellationToken _) => asyncTask(), name, timeoutMS, timeoutBehaviour, exceptionBehaviour, timeoutAction, exceptionAction, weight);
	}

	public void EnqueueAsync(Func<Task> asyncTask, string? name = null, int timeoutMS = 0, TaskAbortBehaviour? timeoutBehaviour = null, TaskAbortBehaviour? exceptionBehaviour = null, Action? timeoutAction = null, Action? exceptionAction = null, int weight = 0)
	{
		EnqueueAsync((Func<CancellationToken, Task<bool>>)async delegate
		{
			await asyncTask();
			return true;
		}, name, timeoutMS, timeoutBehaviour, exceptionBehaviour, timeoutAction, exceptionAction, weight);
	}

	public void DelayNext(int delayMS, string? uniqueName = null, int weight = 0)
	{
		if (delayMS > 0)
		{
			EnqueueAsync(async delegate(CancellationToken ct)
			{
				await Task.Delay(delayMS, ct).ConfigureAwait(continueOnCapturedContext: false);
			}, $"{uniqueName} (延迟 {delayMS} 毫秒)", 0, null, null, null, null, weight);
		}
	}

	public void DelayNext(int minDelayMS, int maxDelayMS, string? uniqueName = null, int weight = 0)
	{
		if (maxDelayMS < minDelayMS)
		{
			throw new ArgumentOutOfRangeException("maxDelayMS");
		}
		DelayNext((int)Random.Shared.NextInt64(minDelayMS, (long)maxDelayMS + 1L), uniqueName, weight);
	}
}
