using System;
using System.Media;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dalamud.Game.Gui.Toast;
using Dalamud.Game.Text;
using Dalamud.Interface.ImGuiNotification;
using Dalamud.Interface.Textures;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.UI;
using Lumina.Text;
using Lumina.Text.ReadOnly;
using OmenTools.Dalamud;
using OmenTools.Extensions;
using OmenTools.Interop.Windows;
using OmenTools.Interop.Windows.Models;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.OmenService;

public class NotifyHelper : OmenServiceBase<NotifyHelper>
{
	private readonly record struct NotificationPlan(string Title, string Message, NotificationType Type, string? MinimizedText, INotificationIcon? IconSource, bool Minimized, ISharedImmediateTexture? Icon, DateTime HardExpiry, TimeSpan InitialDuration, TimeSpan ExtensionDuration, bool ShowIndeterminateIfNoExpiry, bool RespectUIHidden, bool UserDismissable, float Progress)
	{
		[CompilerGenerated]
		public void Deconstruct(out string Title, out string Message, out NotificationType Type, out string? MinimizedText, out INotificationIcon? IconSource, out bool Minimized, out ISharedImmediateTexture? Icon, out DateTime HardExpiry, out TimeSpan InitialDuration, out TimeSpan ExtensionDuration, out bool ShowIndeterminateIfNoExpiry, out bool RespectUIHidden, out bool UserDismissable, out float Progress)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected I4, but got Unknown
			Title = this.Title;
			Message = this.Message;
			Type = (NotificationType)(int)this.Type;
			MinimizedText = this.MinimizedText;
			IconSource = this.IconSource;
			Minimized = this.Minimized;
			Icon = this.Icon;
			HardExpiry = this.HardExpiry;
			InitialDuration = this.InitialDuration;
			ExtensionDuration = this.ExtensionDuration;
			ShowIndeterminateIfNoExpiry = this.ShowIndeterminateIfNoExpiry;
			RespectUIHidden = this.RespectUIHidden;
			UserDismissable = this.UserDismissable;
			Progress = this.Progress;
		}
	}

	public string? NotificationMinimizedText { get; set; }

	public INotificationIcon? NotificationIconSource { get; set; }

	public ISharedImmediateTexture? NotificationIcon { get; set; }

	public DateTime NotificationHardExpiry { get; set; } = DateTime.MaxValue;

	public TimeSpan NotificationInitialDuration { get; set; } = TimeSpan.FromSeconds(3L);

	public TimeSpan NotificationExtensionDuration { get; set; } = TimeSpan.FromSeconds(1L);

	public bool NotificationShowIndeterminateIfNoExpiry { get; set; } = true;

	public bool NotificationRespectUIHidden { get; set; } = true;

	public bool NotificationMinimized { get; set; }

	public bool UserDismissable { get; set; } = true;

	public float NotificationProgress { get; set; } = 1f;

	public TimeSpan ContentHintDuration { get; set; } = TimeSpan.FromSeconds(3L);

	public TrayNotifier? TrayNotifier { get; set; }

	public ReadOnlySeString? ChatPrefix { get; set; }

	public bool RelayToTrayWhenBackground { get; set; } = true;

	public bool SendTrayOnlyBackground { get; set; } = true;

	public bool SpeakTrayMessage { get; set; } = true;

	protected override void Uninit()
	{
		TrayNotifier?.Dispose();
		NotificationMinimizedText = null;
		NotificationIconSource = null;
		TrayNotifier = null;
		NotificationIcon = null;
		ChatPrefix = null;
	}

	public static void Toast(string message, ToastOptions? options = null)
	{
		DalamudServiceExtension.Instance<IToastGui>().ShowNormal(message, options);
	}

	public static void Toast(ReadOnlySeString message, ToastOptions? options = null)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		DalamudServiceExtension.Instance<IToastGui>().ShowNormal(SeStringExtensions.ToDalamudString(message), options);
	}

	public static void ToastError(string message)
	{
		DalamudServiceExtension.Instance<IToastGui>().ShowError(message);
	}

	public static void ToastError(ReadOnlySeString message)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		DalamudServiceExtension.Instance<IToastGui>().ShowError(SeStringExtensions.ToDalamudString(message));
	}

	public static void ToastQuest(string message, QuestToastOptions? options = null)
	{
		DalamudServiceExtension.Instance<IToastGui>().ShowQuest(message, options);
	}

	public static void ToastQuest(ReadOnlySeString message, QuestToastOptions? options = null)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		DalamudServiceExtension.Instance<IToastGui>().ShowQuest(SeStringExtensions.ToDalamudString(message), options);
	}

	public unsafe void ContentHint(string message, TextGimmickHintStyle style = (TextGimmickHintStyle)1, TimeSpan? duration = null)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		ArgumentNullException.ThrowIfNull(message, "message");
		int num = ToHundredMilliseconds(duration ?? ContentHintDuration);
		((RaptureAtkModule)RaptureAtkModule.Instance()).ShowTextGimmickHint(message, style, num);
	}

	public void ContentHintBlue(string message, TimeSpan? duration = null)
	{
		ContentHint(message, (TextGimmickHintStyle)1, duration);
	}

	public void ContentHintRed(string message, TimeSpan? duration = null)
	{
		ContentHint(message, (TextGimmickHintStyle)0, duration);
	}

	public void TrayInfo(string message, string? title = null)
	{
		Tray(message, ToolTipIcon.Info, title);
	}

	public void TrayNone(string message, string? title = null)
	{
		Tray(message, ToolTipIcon.None, title);
	}

	public void TrayWarning(string message, string? title = null)
	{
		Tray(message, ToolTipIcon.Warning, title);
	}

	public void TrayError(string message, string? title = null)
	{
		Tray(message, ToolTipIcon.Error, title);
	}

	public void Tray(string message, ToolTipIcon icon = ToolTipIcon.None, string? title = null)
	{
		ArgumentNullException.ThrowIfNull(TrayNotifier, "TrayNotifier");
		ArgumentNullException.ThrowIfNull(message, "message");
		if (!SendTrayOnlyBackground || !GameState.IsForeground || LastInputInfo.GetIdleTimeTick() >= 15000)
		{
			TrayNotifier.ShowBalloonTip(title ?? message, message, icon);
			if (SpeakTrayMessage && EdgeTTSIPC.IsPluginEnabled())
			{
				Speak(title ?? message);
			}
		}
	}

	public void Notify(string message, NotificationType type = (NotificationType)4, string? title = null, NotificationOptions? options = null)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Expected O, but got Unknown
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Invalid comparison between Unknown and I4
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Invalid comparison between Unknown and I4
		ArgumentNullException.ThrowIfNull(message, "message");
		NotificationPlan notificationPlan = BuildNotificationPlan(this, message, type, title, options);
		if (TrayNotifier != null && RelayToTrayWhenBackground && !GameState.IsForeground)
		{
			NotificationType type2 = notificationPlan.Type;
			ToolTipIcon toolTipIcon = (((int)type2 == 2) ? ToolTipIcon.Warning : (((int)type2 != 3) ? ToolTipIcon.Info : ToolTipIcon.Error));
			ToolTipIcon tooltipIcon = toolTipIcon;
			TrayNotifier.ShowBalloonTip(notificationPlan.Title, notificationPlan.Message, tooltipIcon);
		}
		DalamudServiceExtension.Instance<INotificationManager>().AddNotification(new Notification
		{
			Title = notificationPlan.Title,
			MinimizedText = notificationPlan.MinimizedText,
			Content = notificationPlan.Message,
			Type = notificationPlan.Type,
			Icon = notificationPlan.IconSource,
			Minimized = notificationPlan.Minimized,
			UserDismissable = notificationPlan.UserDismissable,
			Progress = notificationPlan.Progress,
			IconTexture = notificationPlan.Icon,
			HardExpiry = notificationPlan.HardExpiry,
			InitialDuration = notificationPlan.InitialDuration,
			ExtensionDurationSinceLastInterest = notificationPlan.ExtensionDuration,
			ShowIndeterminateIfNoExpiry = notificationPlan.ShowIndeterminateIfNoExpiry,
			RespectUiHidden = notificationPlan.RespectUIHidden
		});
	}

	public void NotificationSuccess(string message, string? title = null, NotificationOptions? options = null)
	{
		Notify(message, (NotificationType)1, title, options);
	}

	public void NotificationWarning(string message, string? title = null, NotificationOptions? options = null)
	{
		Notify(message, (NotificationType)2, title, options);
	}

	public void NotificationError(string message, string? title = null, NotificationOptions? options = null)
	{
		Notify(message, (NotificationType)3, title, options);
	}

	public void NotificationInfo(string message, string? title = null, NotificationOptions? options = null)
	{
		Notify(message, (NotificationType)4, title, options);
	}

	public void Chat(ReadOnlySeString message, bool useDefaultPrefix = true)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		PrintChat(message, useDefaultPrefix ? ChatPrefix : ((ReadOnlySeString?)null), (XivChatType)57);
	}

	public void ChatError(ReadOnlySeString message, bool useDefaultPrefix = true)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		PrintChat(message, useDefaultPrefix ? ChatPrefix : ((ReadOnlySeString?)null), (XivChatType)60);
	}

	public void ChatGain(ReadOnlySeString message, bool useDefaultPrefix = true)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		PrintChat(message, useDefaultPrefix ? ChatPrefix : ((ReadOnlySeString?)null), (XivChatType)62);
	}

	public void ChatDialogue(ReadOnlySeString message, bool useDefaultPrefix = true)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		PrintChat(message, useDefaultPrefix ? ChatPrefix : ((ReadOnlySeString?)null), (XivChatType)61);
	}

	public static void Chat(XivChatEntry entry)
	{
		DalamudServiceExtension.Instance<IChatGui>().Print(entry);
	}

	private unsafe static void PrintChat(ReadOnlySeString message, ReadOnlySeString? prefix = null, XivChatType chatType = (XivChatType)57)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Expected O, but got Unknown
		RentedSeStringBuilder val = default(RentedSeStringBuilder);
		((RentedSeStringBuilder)(ref val))._002Ector();
		try
		{
			SeStringBuilder builder = ((RentedSeStringBuilder)(ref val)).Builder;
			AppendPrefix(builder, prefix);
			builder.Append(message);
			DalamudServiceExtension.Instance<IChatGui>().Print(new XivChatEntry
			{
				Message = SeStringExtensions.ToDalamudString(builder.ToReadOnlySeString()),
				Type = chatType
			});
		}
		finally
		{
			((IDisposable)(*(RentedSeStringBuilder*)(&val))/*cast due to constrained. prefix*/).Dispose();
		}
	}

	private static NotificationPlan BuildNotificationPlan(NotifyHelper helper, string message, NotificationType type, string? title, NotificationOptions? options)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return new NotificationPlan(string.IsNullOrWhiteSpace(title) ? message : title, message, type, options?.MinimizedText ?? helper.NotificationMinimizedText, options?.IconSource ?? helper.NotificationIconSource, options?.Minimized ?? helper.NotificationMinimized, options?.Icon ?? helper.NotificationIcon, options?.HardExpiry ?? helper.NotificationHardExpiry, options?.InitialDuration ?? helper.NotificationInitialDuration, options?.ExtensionDuration ?? helper.NotificationExtensionDuration, options?.ShowIndeterminateIfNoExpiry ?? helper.NotificationShowIndeterminateIfNoExpiry, options?.RespectUIHidden ?? helper.NotificationRespectUIHidden, options?.UserDismissable ?? helper.UserDismissable, options?.Progress ?? helper.NotificationProgress);
	}

	private static void AppendPrefix(SeStringBuilder builder, ReadOnlySeString? prefix)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (prefix.HasValue)
		{
			builder.Append(prefix.Value).Append(" ");
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ToHundredMilliseconds(TimeSpan ts)
	{
		if (ts <= TimeSpan.Zero)
		{
			return 0;
		}
		int num = (int)(ts.Ticks / 1000000);
		if (num != 0)
		{
			return num;
		}
		return 1;
	}

	public static void SystemInformation()
	{
		SystemSounds.Asterisk.Play();
	}

	public static void SystemWarning()
	{
		SystemSounds.Hand.Play();
	}

	public static void SystemBeep(int frequency = 1000, int duration = 500)
	{
		Task.Run(delegate
		{
			Console.Beep(frequency, duration);
		});
	}

	public static void Speak(string message)
	{
		EdgeTTSIPC.Speak(message);
	}

	public static void Speak(string message, int? speed = null, int? pitch = null, int? volume = null)
	{
		EdgeTTSIPC.Speak(message, speed, pitch, volume);
	}

	public static Task SpeakAsync(string message, CancellationToken? token = null)
	{
		if (token.HasValue && token.GetValueOrDefault().IsCancellationRequested)
		{
			return Task.CompletedTask;
		}
		return EdgeTTSIPC.SpeakAsync(message, token ?? CancellationToken.None);
	}

	public static Task SpeakAsync(string message, int? speed = null, int? pitch = null, int? volume = null, CancellationToken? token = null)
	{
		if (token.HasValue && token.GetValueOrDefault().IsCancellationRequested)
		{
			return Task.CompletedTask;
		}
		return EdgeTTSIPC.SpeakAsync(message, speed, pitch, volume, token ?? CancellationToken.None);
	}

	public static void Synthesize(string message)
	{
		EdgeTTSIPC.Synthesize(message);
	}

	public static void Synthesize(string message, int? speed = null, int? pitch = null, int? volume = null)
	{
		EdgeTTSIPC.Synthesize(message, speed, pitch, volume);
	}

	public static Task SynthesizeAsync(string message, CancellationToken? token = null)
	{
		if (token.HasValue && token.GetValueOrDefault().IsCancellationRequested)
		{
			return Task.CompletedTask;
		}
		return EdgeTTSIPC.SynthesizeAsync(message, token ?? CancellationToken.None);
	}

	public static Task SynthesizeAsync(string message, int? speed = null, int? pitch = null, int? volume = null, CancellationToken? token = null)
	{
		if (token.HasValue && token.GetValueOrDefault().IsCancellationRequested)
		{
			return Task.CompletedTask;
		}
		return EdgeTTSIPC.SynthesizeAsync(message, speed, pitch, volume, token ?? CancellationToken.None);
	}
}
