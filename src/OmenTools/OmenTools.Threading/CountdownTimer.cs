using System;
using System.Threading;
using System.Timers;

namespace OmenTools.Threading;

public class CountdownTimer : IDisposable
{
	private readonly System.Timers.Timer timer;

	private volatile int remainingSeconds;

	public int RemainingSeconds
	{
		get
		{
			return remainingSeconds;
		}
		private set
		{
			Interlocked.Exchange(ref remainingSeconds, value);
		}
	}

	public bool IsRunning { get; private set; }

	public event EventHandler<int>? TimeChanged;

	public event EventHandler? Completed;

	public CountdownTimer(int totalSeconds)
	{
		timer = new System.Timers.Timer(1000.0)
		{
			AutoReset = true
		};
		timer.Elapsed += OnTimerElapsed;
		RemainingSeconds = totalSeconds;
	}

	public void Start()
	{
		if (!IsRunning)
		{
			IsRunning = true;
			timer.Start();
		}
	}

	public void Stop()
	{
		if (IsRunning)
		{
			IsRunning = false;
			timer.Stop();
		}
	}

	public void Reset(int newTotalSeconds)
	{
		Stop();
		RemainingSeconds = newTotalSeconds;
	}

	private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
	{
		int num;
		int num2;
		do
		{
			num = RemainingSeconds;
			if (num <= 0)
			{
				Stop();
				this.Completed?.Invoke(this, EventArgs.Empty);
				return;
			}
			num2 = num - 1;
		}
		while (Interlocked.CompareExchange(ref remainingSeconds, num2, num) != num);
		this.TimeChanged?.Invoke(this, num2);
		if (num2 == 0)
		{
			Stop();
			this.Completed?.Invoke(this, EventArgs.Empty);
		}
	}

	public void Dispose()
	{
		timer.Dispose();
	}
}
