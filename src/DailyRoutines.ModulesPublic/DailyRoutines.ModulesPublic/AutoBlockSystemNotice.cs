using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.Chat;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Models;

namespace DailyRoutines.ModulesPublic;

public class AutoBlockSystemNotice : ModuleBase
{
	private delegate void LoginNoticeHandlerDelegate(nint dispatcher, nint packet);

	private static readonly CompSig LoginNoticeSig = new CompSig("E8 ?? ?? ?? ?? E9 ?? ?? ?? ?? 48 8D 57 ?? 41 8B CE E8 ?? ?? ?? ?? E9 ?? ?? ?? ?? 48 8D 57 ?? 41 8B CE E8 ?? ?? ?? ?? B9");

	private Hook<LoginNoticeHandlerDelegate>? LoginNoticeHandlerHook;

	private bool isLoginNotice;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoBlockSystemNoticeTitle"),
		Description = LanguageManager.Get("AutoBlockSystemNoticeDescription"),
		Category = ModuleCategory.System
	};

	protected override void Init()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IChatGui>().ChatMessage += new OnHandleableChatMessageDelegate(OnChat);
		if (LoginNoticeHandlerHook == null)
		{
			LoginNoticeHandlerHook = LoginNoticeSig.GetHook<LoginNoticeHandlerDelegate>(LoginNoticeHandlerDetour);
		}
		LoginNoticeHandlerHook.Enable();
	}

	protected override void Uninit()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IChatGui>().ChatMessage -= new OnHandleableChatMessageDelegate(OnChat);
	}

	private void LoginNoticeHandlerDetour(nint dispatcher, nint packet)
	{
		isLoginNotice = true;
		LoginNoticeHandlerHook.Original(dispatcher, packet);
		isLoginNotice = false;
	}

	private void OnChat(IHandleableChatMessage message)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Invalid comparison between Unknown and I4
		if (isLoginNotice && (int)((IChatMessage)message).LogKind == 3)
		{
			message.PreventOriginal();
		}
	}
}
