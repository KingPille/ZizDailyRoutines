using System;

namespace OmenTools.Dalamud.Abstractions;

public abstract class IPCResultSubscriberBase<TSubscriber, TResult>(string ipcName, Func<TResult>? defaultValueFactory = null) : IPCSubscriberBase<TSubscriber>(ipcName) where TSubscriber : class
{
	private readonly Func<TResult> defaultValueFactory = defaultValueFactory ?? ((Func<TResult>)(() => default(TResult)));

	protected TResult InvokeValue(Func<TSubscriber, TResult> operation, bool suppressException = false, bool deduplicateError = false)
	{
		return InvokeCore(operation, defaultValueFactory, suppressException, deduplicateError);
	}
}
