using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Lumina.Text.ReadOnly;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class AutoNotifyLogin : ModuleBase
{
	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoNotifyLoginTitle"),
		Description = LanguageManager.Get("AutoNotifyLoginDescription"),
		Category = ModuleCategory.Notification
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected override void Init()
	{
		OmenServiceBase<GameState>.Instance().Login += OnLogin;
	}

	protected override void Uninit()
	{
		OmenServiceBase<GameState>.Instance().Login -= OnLogin;
	}

	private static void OnLogin()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		string text = LanguageManager.Get("AutoNotifyLogin-Notification");
		OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(text), useDefaultPrefix: false);
		OmenServiceBase<NotifyHelper>.Instance().TrayInfo(text);
	}
}
