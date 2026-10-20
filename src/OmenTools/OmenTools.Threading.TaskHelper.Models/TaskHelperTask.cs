using System;
using System.Threading;
using System.Threading.Tasks;
using OmenTools.Threading.TaskHelper.Enums;

namespace OmenTools.Threading.TaskHelper.Models;

public record TaskHelperTask
{
	public Func<bool>? Action { get; }

	public Func<CancellationToken, Task<bool>>? AsyncAction { get; }

	public int TimeoutMS { get; }

	public TaskAbortBehaviour? TimeoutBehaviour { get; }

	public TaskAbortBehaviour? ExceptionBehaviour { get; }

	public Action? TimeoutAction { get; set; }

	public Action? ExceptionAction { get; set; }

	public CancellationTokenSource? CancelToken { get; }

	private string? Name { get; }

	internal bool SuppressesCancellationException { get; private set; }

	public long StartTime { get; set; }

	public bool IsAsync => AsyncAction != null;

	public TaskHelperTask(Func<bool> action, string? name = null, int timeoutMS = 0, TaskAbortBehaviour? timeoutBehaviour = null, TaskAbortBehaviour? exceptionBehaviour = null, Action? timeoutAction = null, Action? exceptionAction = null)
	{
		Action = action;
		AsyncAction = null;
		TimeoutMS = timeoutMS;
		TimeoutBehaviour = timeoutBehaviour;
		ExceptionBehaviour = exceptionBehaviour;
		TimeoutAction = timeoutAction;
		ExceptionAction = exceptionAction;
		Name = name;
		CancelToken = new CancellationTokenSource();
	}

	public TaskHelperTask(Func<CancellationToken, Task<bool>> asyncAction, string? name = null, int timeoutMS = 0, TaskAbortBehaviour? timeoutBehaviour = null, TaskAbortBehaviour? exceptionBehaviour = null, Action? timeoutAction = null, Action? exceptionAction = null)
	{
		Action = null;
		AsyncAction = asyncAction;
		TimeoutMS = timeoutMS;
		TimeoutBehaviour = timeoutBehaviour;
		ExceptionBehaviour = exceptionBehaviour;
		TimeoutAction = timeoutAction;
		ExceptionAction = exceptionAction;
		Name = name;
		CancelToken = new CancellationTokenSource();
	}

	public string GetName()
	{
		return Name ?? "(无名称)";
	}

	internal void SuppressCancellationException()
	{
		SuppressesCancellationException = true;
	}
}
