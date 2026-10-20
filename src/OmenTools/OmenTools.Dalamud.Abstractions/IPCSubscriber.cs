using System;
using System.Collections.Generic;
using Dalamud.Plugin.Ipc;
using OmenTools.Extensions;

namespace OmenTools.Dalamud.Abstractions;

public class IPCSubscriber<T> : IPCResultSubscriberBase<ICallGateSubscriber<T>, T>
{
	public T Value => InvokeValue((ICallGateSubscriber<T> subscriber) => subscriber.InvokeFunc(), suppressException: false, deduplicateError: true);

	public IPCSubscriber(string ipcName, Func<T>? defaultValueFactory = null)
		: base(ipcName, defaultValueFactory)
	{
	}

	protected override ICallGateSubscriber<T> CreateSubscriber()
	{
		return DalamudServiceExtension.Instance().GetIpcSubscriber<T>(IPCName);
	}

	public T InvokeFunc()
	{
		return InvokeValue((ICallGateSubscriber<T> subscriber) => subscriber.InvokeFunc());
	}

	public void InvokeAction()
	{
		InvokeCore(delegate(ICallGateSubscriber<T> subscriber)
		{
			subscriber.InvokeAction();
		});
	}

	public T TryInvokeFunc()
	{
		return InvokeValue((ICallGateSubscriber<T> subscriber) => subscriber.InvokeFunc(), suppressException: true);
	}

	public void TryInvokeAction()
	{
		InvokeCore(delegate(ICallGateSubscriber<T> subscriber)
		{
			subscriber.InvokeAction();
		}, suppressException: true);
	}

	public static implicit operator T(IPCSubscriber<T>? subscriber)
	{
		if (subscriber != null)
		{
			return subscriber.Value;
		}
		return default(T);
	}

	public override bool Equals(object? obj)
	{
		EqualityComparer<T> equalityComparer = EqualityComparer<T>.Default;
		if (!(obj is IPCSubscriber<T> iPCSubscriber))
		{
			if (obj is T y)
			{
				return equalityComparer.Equals(Value, y);
			}
			return false;
		}
		return equalityComparer.Equals(Value, iPCSubscriber.Value);
	}

	public override int GetHashCode()
	{
		T value = Value;
		if (value != null)
		{
			return EqualityComparer<T>.Default.GetHashCode(value);
		}
		return 0;
	}

	public override string ToString()
	{
		return Value?.ToString() ?? "null";
	}
}
public class IPCSubscriber<T1, TResult> : IPCResultSubscriberBase<ICallGateSubscriber<T1, TResult>, TResult>
{
	public IPCSubscriber(string ipcName, Func<TResult>? defaultValueFactory = null)
		: base(ipcName, defaultValueFactory)
	{
	}

	protected override ICallGateSubscriber<T1, TResult> CreateSubscriber()
	{
		return DalamudServiceExtension.Instance().GetIpcSubscriber<T1, TResult>(IPCName);
	}

	public TResult InvokeFunc(T1 arg1)
	{
		return InvokeValue((ICallGateSubscriber<T1, TResult> subscriber) => subscriber.InvokeFunc(arg1), suppressException: false, deduplicateError: true);
	}

	public void InvokeAction(T1 arg1)
	{
		InvokeCore(delegate(ICallGateSubscriber<T1, TResult> subscriber)
		{
			subscriber.InvokeAction(arg1);
		});
	}

	public TResult TryInvokeFunc(T1 arg1)
	{
		return InvokeValue((ICallGateSubscriber<T1, TResult> subscriber) => subscriber.InvokeFunc(arg1), suppressException: true);
	}

	public void TryInvokeAction(T1 arg1)
	{
		InvokeCore(delegate(ICallGateSubscriber<T1, TResult> subscriber)
		{
			subscriber.InvokeAction(arg1);
		}, suppressException: true);
	}
}
public class IPCSubscriber<T1, T2, TResult> : IPCResultSubscriberBase<ICallGateSubscriber<T1, T2, TResult>, TResult>
{
	public IPCSubscriber(string ipcName, Func<TResult>? defaultValueFactory = null)
		: base(ipcName, defaultValueFactory)
	{
	}

	protected override ICallGateSubscriber<T1, T2, TResult> CreateSubscriber()
	{
		return DalamudServiceExtension.Instance().GetIpcSubscriber<T1, T2, TResult>(IPCName);
	}

	public TResult InvokeFunc(T1 arg1, T2 arg2)
	{
		return InvokeValue((ICallGateSubscriber<T1, T2, TResult> subscriber) => subscriber.InvokeFunc(arg1, arg2), suppressException: false, deduplicateError: true);
	}

	public void InvokeAction(T1 arg1, T2 arg2)
	{
		InvokeCore(delegate(ICallGateSubscriber<T1, T2, TResult> subscriber)
		{
			subscriber.InvokeAction(arg1, arg2);
		});
	}

	public TResult TryInvokeFunc(T1 arg1, T2 arg2)
	{
		return InvokeValue((ICallGateSubscriber<T1, T2, TResult> subscriber) => subscriber.InvokeFunc(arg1, arg2), suppressException: true);
	}

	public void TryInvokeAction(T1 arg1, T2 arg2)
	{
		InvokeCore(delegate(ICallGateSubscriber<T1, T2, TResult> subscriber)
		{
			subscriber.InvokeAction(arg1, arg2);
		}, suppressException: true);
	}
}
public class IPCSubscriber<T1, T2, T3, TResult> : IPCResultSubscriberBase<ICallGateSubscriber<T1, T2, T3, TResult>, TResult>
{
	public IPCSubscriber(string ipcName, Func<TResult>? defaultValueFactory = null)
		: base(ipcName, defaultValueFactory)
	{
	}

	protected override ICallGateSubscriber<T1, T2, T3, TResult> CreateSubscriber()
	{
		return DalamudServiceExtension.Instance().GetIpcSubscriber<T1, T2, T3, TResult>(IPCName);
	}

	public TResult InvokeFunc(T1 arg1, T2 arg2, T3 arg3)
	{
		return InvokeValue((ICallGateSubscriber<T1, T2, T3, TResult> subscriber) => subscriber.InvokeFunc(arg1, arg2, arg3), suppressException: false, deduplicateError: true);
	}

	public void InvokeAction(T1 arg1, T2 arg2, T3 arg3)
	{
		InvokeCore(delegate(ICallGateSubscriber<T1, T2, T3, TResult> subscriber)
		{
			subscriber.InvokeAction(arg1, arg2, arg3);
		});
	}

	public TResult TryInvokeFunc(T1 arg1, T2 arg2, T3 arg3)
	{
		return InvokeValue((ICallGateSubscriber<T1, T2, T3, TResult> subscriber) => subscriber.InvokeFunc(arg1, arg2, arg3), suppressException: true);
	}

	public void TryInvokeAction(T1 arg1, T2 arg2, T3 arg3)
	{
		InvokeCore(delegate(ICallGateSubscriber<T1, T2, T3, TResult> subscriber)
		{
			subscriber.InvokeAction(arg1, arg2, arg3);
		}, suppressException: true);
	}
}
public class IPCSubscriber<T1, T2, T3, T4, TResult> : IPCResultSubscriberBase<ICallGateSubscriber<T1, T2, T3, T4, TResult>, TResult>
{
	public IPCSubscriber(string ipcName, Func<TResult>? defaultValueFactory = null)
		: base(ipcName, defaultValueFactory)
	{
	}

	protected override ICallGateSubscriber<T1, T2, T3, T4, TResult> CreateSubscriber()
	{
		return DalamudServiceExtension.Instance().GetIpcSubscriber<T1, T2, T3, T4, TResult>(IPCName);
	}

	public TResult InvokeFunc(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
	{
		return InvokeValue((ICallGateSubscriber<T1, T2, T3, T4, TResult> subscriber) => subscriber.InvokeFunc(arg1, arg2, arg3, arg4), suppressException: false, deduplicateError: true);
	}

	public void InvokeAction(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
	{
		InvokeCore(delegate(ICallGateSubscriber<T1, T2, T3, T4, TResult> subscriber)
		{
			subscriber.InvokeAction(arg1, arg2, arg3, arg4);
		});
	}

	public TResult TryInvokeFunc(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
	{
		return InvokeValue((ICallGateSubscriber<T1, T2, T3, T4, TResult> subscriber) => subscriber.InvokeFunc(arg1, arg2, arg3, arg4), suppressException: true);
	}

	public void TryInvokeAction(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
	{
		InvokeCore(delegate(ICallGateSubscriber<T1, T2, T3, T4, TResult> subscriber)
		{
			subscriber.InvokeAction(arg1, arg2, arg3, arg4);
		}, suppressException: true);
	}
}
public class IPCSubscriber<T1, T2, T3, T4, T5, TResult> : IPCResultSubscriberBase<ICallGateSubscriber<T1, T2, T3, T4, T5, TResult>, TResult>
{
	public IPCSubscriber(string ipcName, Func<TResult>? defaultValueFactory = null)
		: base(ipcName, defaultValueFactory)
	{
	}

	protected override ICallGateSubscriber<T1, T2, T3, T4, T5, TResult> CreateSubscriber()
	{
		return DalamudServiceExtension.Instance().GetIpcSubscriber<T1, T2, T3, T4, T5, TResult>(IPCName);
	}

	public TResult InvokeFunc(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
	{
		return InvokeValue((ICallGateSubscriber<T1, T2, T3, T4, T5, TResult> subscriber) => subscriber.InvokeFunc(arg1, arg2, arg3, arg4, arg5), suppressException: false, deduplicateError: true);
	}

	public void InvokeAction(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
	{
		InvokeCore(delegate(ICallGateSubscriber<T1, T2, T3, T4, T5, TResult> subscriber)
		{
			subscriber.InvokeAction(arg1, arg2, arg3, arg4, arg5);
		});
	}

	public TResult TryInvokeFunc(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
	{
		return InvokeValue((ICallGateSubscriber<T1, T2, T3, T4, T5, TResult> subscriber) => subscriber.InvokeFunc(arg1, arg2, arg3, arg4, arg5), suppressException: true);
	}

	public void TryInvokeAction(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
	{
		InvokeCore(delegate(ICallGateSubscriber<T1, T2, T3, T4, T5, TResult> subscriber)
		{
			subscriber.InvokeAction(arg1, arg2, arg3, arg4, arg5);
		}, suppressException: true);
	}
}
public class IPCSubscriber<T1, T2, T3, T4, T5, T6, TResult> : IPCResultSubscriberBase<ICallGateSubscriber<T1, T2, T3, T4, T5, T6, TResult>, TResult>
{
	public IPCSubscriber(string ipcName, Func<TResult>? defaultValueFactory = null)
		: base(ipcName, defaultValueFactory)
	{
	}

	protected override ICallGateSubscriber<T1, T2, T3, T4, T5, T6, TResult> CreateSubscriber()
	{
		return DalamudServiceExtension.Instance().GetIpcSubscriber<T1, T2, T3, T4, T5, T6, TResult>(IPCName);
	}

	public TResult InvokeFunc(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
	{
		return InvokeValue((ICallGateSubscriber<T1, T2, T3, T4, T5, T6, TResult> subscriber) => subscriber.InvokeFunc(arg1, arg2, arg3, arg4, arg5, arg6), suppressException: false, deduplicateError: true);
	}

	public void InvokeAction(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
	{
		InvokeCore(delegate(ICallGateSubscriber<T1, T2, T3, T4, T5, T6, TResult> subscriber)
		{
			subscriber.InvokeAction(arg1, arg2, arg3, arg4, arg5, arg6);
		});
	}

	public TResult TryInvokeFunc(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
	{
		return InvokeValue((ICallGateSubscriber<T1, T2, T3, T4, T5, T6, TResult> subscriber) => subscriber.InvokeFunc(arg1, arg2, arg3, arg4, arg5, arg6), suppressException: true);
	}

	public void TryInvokeAction(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
	{
		InvokeCore(delegate(ICallGateSubscriber<T1, T2, T3, T4, T5, T6, TResult> subscriber)
		{
			subscriber.InvokeAction(arg1, arg2, arg3, arg4, arg5, arg6);
		}, suppressException: true);
	}
}
public class IPCSubscriber<T1, T2, T3, T4, T5, T6, T7, TResult> : IPCResultSubscriberBase<ICallGateSubscriber<T1, T2, T3, T4, T5, T6, T7, TResult>, TResult>
{
	public IPCSubscriber(string ipcName, Func<TResult>? defaultValueFactory = null)
		: base(ipcName, defaultValueFactory)
	{
	}

	protected override ICallGateSubscriber<T1, T2, T3, T4, T5, T6, T7, TResult> CreateSubscriber()
	{
		return DalamudServiceExtension.Instance().GetIpcSubscriber<T1, T2, T3, T4, T5, T6, T7, TResult>(IPCName);
	}

	public TResult InvokeFunc(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
	{
		return InvokeValue((ICallGateSubscriber<T1, T2, T3, T4, T5, T6, T7, TResult> subscriber) => subscriber.InvokeFunc(arg1, arg2, arg3, arg4, arg5, arg6, arg7), suppressException: false, deduplicateError: true);
	}

	public void InvokeAction(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
	{
		InvokeCore(delegate(ICallGateSubscriber<T1, T2, T3, T4, T5, T6, T7, TResult> subscriber)
		{
			subscriber.InvokeAction(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
		});
	}

	public TResult TryInvokeFunc(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
	{
		return InvokeValue((ICallGateSubscriber<T1, T2, T3, T4, T5, T6, T7, TResult> subscriber) => subscriber.InvokeFunc(arg1, arg2, arg3, arg4, arg5, arg6, arg7), suppressException: true);
	}

	public void TryInvokeAction(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
	{
		InvokeCore(delegate(ICallGateSubscriber<T1, T2, T3, T4, T5, T6, T7, TResult> subscriber)
		{
			subscriber.InvokeAction(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
		}, suppressException: true);
	}
}
public class IPCSubscriber<T1, T2, T3, T4, T5, T6, T7, T8, TResult> : IPCResultSubscriberBase<ICallGateSubscriber<T1, T2, T3, T4, T5, T6, T7, T8, TResult>, TResult>
{
	public IPCSubscriber(string ipcName, Func<TResult>? defaultValueFactory = null)
		: base(ipcName, defaultValueFactory)
	{
	}

	protected override ICallGateSubscriber<T1, T2, T3, T4, T5, T6, T7, T8, TResult> CreateSubscriber()
	{
		return DalamudServiceExtension.Instance().GetIpcSubscriber<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(IPCName);
	}

	public TResult InvokeFunc(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
	{
		return InvokeValue((ICallGateSubscriber<T1, T2, T3, T4, T5, T6, T7, T8, TResult> subscriber) => subscriber.InvokeFunc(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8), suppressException: false, deduplicateError: true);
	}

	public void InvokeAction(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
	{
		InvokeCore(delegate(ICallGateSubscriber<T1, T2, T3, T4, T5, T6, T7, T8, TResult> subscriber)
		{
			subscriber.InvokeAction(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
		});
	}

	public TResult TryInvokeFunc(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
	{
		return InvokeValue((ICallGateSubscriber<T1, T2, T3, T4, T5, T6, T7, T8, TResult> subscriber) => subscriber.InvokeFunc(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8), suppressException: true);
	}

	public void TryInvokeAction(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
	{
		InvokeCore(delegate(ICallGateSubscriber<T1, T2, T3, T4, T5, T6, T7, T8, TResult> subscriber)
		{
			subscriber.InvokeAction(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
		}, suppressException: true);
	}
}
