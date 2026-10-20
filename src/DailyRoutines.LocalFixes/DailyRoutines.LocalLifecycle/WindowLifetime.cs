using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DailyRoutines.LocalLifecycle;

public static class WindowLifetime
{
	private sealed class State
	{
		internal readonly object Gate = new object();

		internal long Generation;

		internal bool Active;
	}

	private static readonly ConditionalWeakTable<object, State> States = new ConditionalWeakTable<object, State>();

	public static void Start(object owner)
	{
		ArgumentNullException.ThrowIfNull(owner, "owner");
		State value = States.GetValue(owner, (object _) => new State());
		lock (value.Gate)
		{
			value.Generation++;
			value.Active = true;
		}
	}

	public static Task Queue(Action callback, object owner)
	{
		return QueueCore(callback, owner, (Action action) => Task.Run(action));
	}

	internal static Task QueueCore(Action callback, object owner, Func<Action, Task> schedule)
	{
		ArgumentNullException.ThrowIfNull(callback, "callback");
		ArgumentNullException.ThrowIfNull(owner, "owner");
		State state = States.GetValue(owner, (object _) => new State());
		long generation;
		lock (state.Gate)
		{
			if (!state.Active)
			{
				return Task.CompletedTask;
			}
			generation = state.Generation;
		}
		return schedule(delegate
		{
			lock (state.Gate)
			{
				if (state.Active && state.Generation == generation)
				{
					callback();
				}
			}
		});
	}

	public static void Stop(object owner, Action cleanup)
	{
		ArgumentNullException.ThrowIfNull(owner, "owner");
		ArgumentNullException.ThrowIfNull(cleanup, "cleanup");
		State value = States.GetValue(owner, (object _) => new State());
		lock (value.Gate)
		{
			value.Active = false;
			value.Generation++;
			cleanup();
		}
	}
}
