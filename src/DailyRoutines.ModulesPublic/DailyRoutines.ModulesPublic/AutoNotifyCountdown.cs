using System.Collections.Generic;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class AutoNotifyCountdown : ModuleBase
{
	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoNotifyCountdownTitle"),
		Description = LanguageManager.Get("AutoNotifyCountdownDescription"),
		Category = ModuleCategory.Combat,
		Author = new string[1] { "HSS" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected override void Init()
	{
		OmenServiceBase<LogMessageManager>.Instance().RegPost(OnLogMessage);
	}

	protected override void Uninit()
	{
		OmenServiceBase<LogMessageManager>.Instance().Unreg(OnLogMessage);
	}

	private static void OnLogMessage(uint logMessageID, LogMessageQueueItem item)
	{
		if (logMessageID == 5255)
		{
			string message = LanguageManager.Get("AutoNotifyCountdown-Notification", new Dictionary<string, object> { ["seconds"] = item.Parameters[0].IntValue });
			OmenServiceBase<NotifyHelper>.Instance().TrayInfo(message);
		}
	}
}
