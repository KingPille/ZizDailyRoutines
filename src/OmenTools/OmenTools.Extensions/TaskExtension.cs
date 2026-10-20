using System;
using System.Threading;
using System.Threading.Tasks;

namespace OmenTools.Extensions;

public static class TaskExtension
{
	extension(Task task)
	{
		public static async ValueTask WaitForConditionAsync(Func<bool> condition, TimeSpan? timeout = null, CancellationToken? token = null)
		{
			if (condition())
			{
				return;
			}
			token.GetValueOrDefault();
			if (!token.HasValue)
			{
				CancellationToken none = CancellationToken.None;
				token = none;
			}
			long deadlineTick = 0L;
			if (timeout.HasValue)
			{
				deadlineTick = Environment.TickCount64 + (long)timeout.Value.TotalMilliseconds;
			}
			TimeSpan interval = TimeSpan.FromMilliseconds(100L);
			while (!token.Value.IsCancellationRequested)
			{
				await Task.Delay(interval, token.Value).ConfigureAwait(continueOnCapturedContext: false);
				if (condition())
				{
					break;
				}
				if (timeout.HasValue && Environment.TickCount64 >= deadlineTick)
				{
					throw new TimeoutException();
				}
			}
		}
	}
}
