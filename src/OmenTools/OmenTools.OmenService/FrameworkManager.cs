using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Dalamud.Plugin.Services;
using OmenTools.Dalamud;
using OmenTools.Extensions;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.OmenService;

public class FrameworkManager : OmenServiceBase<FrameworkManager>
{
	private class MethodState
	{
		public long ThrottleTicks;

		public long NextExecutionTick;
	}

	private static readonly long TicksPerMillisecond = Stopwatch.Frequency / 1000;

	private readonly ConcurrentDictionary<OnUpdateDelegate, MethodState> methodsCollection = new ConcurrentDictionary<OnUpdateDelegate, MethodState>();

	protected override void Init()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IFramework>().Update += new OnUpdateDelegate(OnUpdate);
	}

	protected override void Uninit()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IFramework>().Update -= new OnUpdateDelegate(OnUpdate);
		methodsCollection.Clear();
	}

	public bool Reg(OnUpdateDelegate method, uint throttleMS = 0u)
	{
		return methodsCollection.TryAdd(method, new MethodState
		{
			ThrottleTicks = throttleMS * TicksPerMillisecond,
			NextExecutionTick = 0L
		});
	}

	public bool Unreg(params OnUpdateDelegate[] methods)
	{
		bool result = true;
		foreach (OnUpdateDelegate key in methods)
		{
			if (!methodsCollection.TryRemove(key, out MethodState _))
			{
				result = false;
			}
		}
		return result;
	}

	public bool ResetThrottle(OnUpdateDelegate method)
	{
		if (!methodsCollection.TryGetValue(method, out MethodState value))
		{
			return false;
		}
		Volatile.Write(ref value.NextExecutionTick, 0L);
		return true;
	}

	private void OnUpdate(IFramework framework)
	{
		long timestamp = Stopwatch.GetTimestamp();
		foreach (var (val2, methodState2) in methodsCollection)
		{
			if (timestamp >= Volatile.Read(in methodState2.NextExecutionTick))
			{
				if (methodState2.ThrottleTicks > 0)
				{
					methodState2.NextExecutionTick = timestamp + methodState2.ThrottleTicks;
				}
				try
				{
					val2.Invoke(framework);
				}
				catch (Exception ex)
				{
					DLog.Error("在 Framework 更新过程中发生错误", ex);
				}
			}
		}
	}
}
