using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using OmenTools.Extensions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class AutoNotifyReadyCheck : ModuleBase
{
	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoNotifyReadyCheckTitle"),
		Description = LanguageManager.Get("AutoNotifyReadyCheckDescription"),
		Category = ModuleCategory.Combat
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
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (logMessageID - 3790 <= 1)
		{
			OmenServiceBase<NotifyHelper>.Instance().TrayInfo(((object)item.ToReadOnlySeString()/*cast due to constrained. prefix*/).ToString());
		}
	}
}
