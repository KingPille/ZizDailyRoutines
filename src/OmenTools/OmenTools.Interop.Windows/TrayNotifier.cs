using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows.Forms;
using OmenTools.Extensions;

namespace OmenTools.Interop.Windows;

public sealed class TrayNotifier : IDisposable
{
	private sealed class TrayIconThread : IDisposable
	{
		private readonly ManualResetEventSlim initialized = new ManualResetEventSlim();

		private readonly Thread thread;

		private WindowsFormsSynchronizationContext? context;

		private NotifyIcon? trayIcon;

		private Exception? initializationException;

		private int isDisposed;

		public TrayIconThread()
		{
			thread = new Thread(Run)
			{
				IsBackground = true,
				Name = "TrayNotifier"
			};
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
			initialized.Wait();
			if (initializationException != null)
			{
				throw new InvalidOperationException("无法初始化托盘通知线程。", initializationException);
			}
		}

		public void Dispose()
		{
			if (Interlocked.Exchange(ref isDisposed, 1) != 0)
			{
				return;
			}
			if (Thread.CurrentThread == thread)
			{
				Application.ExitThread();
				return;
			}
			Volatile.Read(in context)?.Post(delegate(object? state)
			{
				NotifyIcon notifyIcon = ((TrayIconThread)state).trayIcon;
				if (notifyIcon != null)
				{
					notifyIcon.Visible = false;
				}
				Application.ExitThread();
			}, this);
			thread.Join();
			initialized.Dispose();
		}

		public void SetIcon(Icon icon)
		{
			Post(delegate
			{
				if (trayIcon != null)
				{
					trayIcon.Icon = icon;
				}
			});
		}

		public void ShowBalloonTip(int timeout, string title, string message, ToolTipIcon tooltipIcon)
		{
			string resolvedMessage = (string.IsNullOrWhiteSpace(message) ? title : message);
			if (string.IsNullOrWhiteSpace(resolvedMessage))
			{
				return;
			}
			Post(delegate
			{
				NotifyIcon notifyIcon = trayIcon;
				if (notifyIcon != null && notifyIcon.Icon != null)
				{
					notifyIcon.Visible = true;
					notifyIcon.ShowBalloonTip(timeout, title, resolvedMessage, tooltipIcon);
				}
			});
		}

		public void Hide()
		{
			Post(delegate
			{
				if (trayIcon != null)
				{
					trayIcon.Visible = false;
				}
			});
		}

		private void Run()
		{
			try
			{
				using WindowsFormsSynchronizationContext synchronizationContext = new WindowsFormsSynchronizationContext();
				SynchronizationContext.SetSynchronizationContext(synchronizationContext);
				context = synchronizationContext;
				trayIcon = new NotifyIcon
				{
					Visible = false
				};
				initialized.Set();
				Application.Run();
			}
			catch (Exception ex)
			{
				initializationException = ex;
				initialized.Set();
			}
			finally
			{
				trayIcon?.Dispose();
				trayIcon = null;
				SynchronizationContext.SetSynchronizationContext(null);
			}
		}

		private void Post(Action action)
		{
			if (Volatile.Read(in isDisposed) != 0)
			{
				return;
			}
			Volatile.Read(in context)?.Post(delegate(object? state)
			{
				var (trayIconThread, action2) = ((TrayIconThread, Action))state;
				if (Volatile.Read(in trayIconThread.isDisposed) == 0)
				{
					action2();
				}
			}, (this, action));
		}
	}

	private readonly record struct BalloonTipMessage(string Title, string Message, ToolTipIcon Icon);

	[CompilerGenerated]
	private Icon _003CIcon_003Ek__BackingField;

	private readonly Channel<BalloonTipMessage> messageChannel = Channel.CreateUnbounded<BalloonTipMessage>(new UnboundedChannelOptions
	{
		SingleReader = true,
		SingleWriter = false,
		AllowSynchronousContinuations = false
	});

	private readonly CancellationTokenSource disposalTokenSource = new CancellationTokenSource();

	private readonly TrayIconThread? trayIconThread;

	private int isDisposed;

	private static readonly TimeSpan MergeWindow = TimeSpan.FromMilliseconds(500L);

	private static readonly TimeSpan DisplayWindow = TimeSpan.FromSeconds(7L);

	public string MultiMessageTemplate { get; set; } = "收到了 {0} 条新消息";

	public required Icon Icon
	{
		get
		{
			return Volatile.Read(in _003CIcon_003Ek__BackingField) ?? throw new InvalidOperationException("托盘图标尚未初始化。");
		}
		set
		{
			if (!IsDisposed)
			{
				ArgumentNullException.ThrowIfNull(value, "value");
				Volatile.Write(ref _003CIcon_003Ek__BackingField, value);
				trayIconThread?.SetIcon(value);
			}
		}
	}

	private bool IsDisposed => Volatile.Read(in isDisposed) != 0;

	public TrayNotifier()
	{
		if (!RuntimeInformationExtension.IsWine())
		{
			trayIconThread = new TrayIconThread();
			Task.Run((Func<Task?>)ProcessLoopAsync);
		}
	}

	public void Dispose()
	{
		if (Interlocked.Exchange(ref isDisposed, 1) == 0)
		{
			messageChannel.Writer.TryComplete();
			disposalTokenSource.Cancel();
			trayIconThread?.Dispose();
			disposalTokenSource.Dispose();
		}
	}

	public void ShowBalloonTip(string title, string message, ToolTipIcon tooltipIcon = ToolTipIcon.Info)
	{
		if (!IsDisposed && trayIconThread != null)
		{
			messageChannel.Writer.TryWrite(new BalloonTipMessage(title ?? string.Empty, message ?? string.Empty, tooltipIcon));
		}
	}

	private async Task ProcessLoopAsync()
	{
		ChannelReader<BalloonTipMessage> reader = messageChannel.Reader;
		CancellationToken token = disposalTokenSource.Token;
		List<BalloonTipMessage> buffer = new List<BalloonTipMessage>(8);
		try
		{
			while (await reader.WaitToReadAsync(token).ConfigureAwait(continueOnCapturedContext: false))
			{
				buffer.Clear();
				BalloonTipMessage item;
				while (reader.TryRead(out item))
				{
					buffer.Add(item);
				}
				if (buffer.Count != 0)
				{
					await Task.Delay(MergeWindow, token).ConfigureAwait(continueOnCapturedContext: false);
					BalloonTipMessage item2;
					while (reader.TryRead(out item2))
					{
						buffer.Add(item2);
					}
					ShowBatch(buffer);
					await Task.Delay(DisplayWindow, token).ConfigureAwait(continueOnCapturedContext: false);
					trayIconThread?.Hide();
				}
			}
		}
		catch (OperationCanceledException) when (token.IsCancellationRequested)
		{
		}
		finally
		{
			trayIconThread?.Hide();
		}
	}

	private void ShowBatch(List<BalloonTipMessage> buffer)
	{
		if (buffer.Count > 1)
		{
			string text = string.Format(CultureInfo.CurrentCulture, MultiMessageTemplate, buffer.Count);
			trayIconThread?.ShowBalloonTip(5000, text, text, ToolTipIcon.Info);
		}
		else
		{
			BalloonTipMessage balloonTipMessage = buffer[0];
			trayIconThread?.ShowBalloonTip(5000, balloonTipMessage.Title, balloonTipMessage.Message, balloonTipMessage.Icon);
		}
	}
}
