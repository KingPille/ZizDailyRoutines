using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Timers;

namespace DailyRoutines.LocalLifecycle;

public static class TimerLifetime
{
	private sealed class State
	{
		internal readonly object Gate = new object();

		internal long Generation;

		internal bool Active;

		internal bool Ready;
	}

	private sealed record Context(object Owner, object Sender, ElapsedEventHandler Callback);

	private static readonly ConditionalWeakTable<object, State> States = new ConditionalWeakTable<object, State>();

	[ThreadStatic]
	private static Context? executing;

	public static void Begin(object owner)
	{
		State value = States.GetValue(owner, (object _) => new State());
		lock (value.Gate)
		{
			value.Generation++;
			value.Active = true;
			value.Ready = true;
		}
	}

	public static void End(object owner)
	{
		State value = States.GetValue(owner, (object _) => new State());
		lock (value.Gate)
		{
			value.Generation++;
			value.Active = false;
			value.Ready = false;
		}
	}

	public static void Suspend(object owner)
	{
		State value = States.GetValue(owner, (object _) => new State());
		lock (value.Gate)
		{
			value.Generation++;
			value.Ready = false;
		}
	}

	public static void Resume(object owner)
	{
		State value = States.GetValue(owner, (object _) => new State());
		lock (value.Gate)
		{
			if (value.Active)
			{
				value.Generation++;
				value.Ready = true;
			}
		}
	}

	public static bool Receive(object owner, object? sender, object? currentTimer, ElapsedEventHandler callback, ElapsedEventArgs? args, Func<Action, Task> schedule)
	{
		if (sender == null || sender != currentTimer)
		{
			return false;
		}
		State state = States.GetValue(owner, (object _) => new State());
		long generation;
		lock (state.Gate)
		{
			if (!state.Active || !state.Ready)
			{
				return false;
			}
			Context context = executing;
			if ((object)context != null && context.Owner == owner && context.Sender == sender && context.Callback.Equals(callback))
			{
				return true;
			}
			generation = state.Generation;
		}
		try
		{
			Observe(schedule(delegate
			{
				lock (state.Gate)
				{
					if (!state.Active || !state.Ready || state.Generation != generation)
					{
						return;
					}
					Context context2 = executing;
					executing = new Context(owner, sender, callback);
					try
					{
						callback(sender, args);
					}
					finally
					{
						executing = context2;
					}
				}
			}));
		}
		catch (Exception)
		{
		}
		return false;
	}

	private static async Task Observe(Task task)
	{
		try
		{
			await task.ConfigureAwait(continueOnCapturedContext: false);
		}
		catch (Exception)
		{
		}
	}
}
