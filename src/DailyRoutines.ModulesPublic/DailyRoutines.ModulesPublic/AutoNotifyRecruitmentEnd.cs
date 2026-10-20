using System.Collections.Frozen;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class AutoNotifyRecruitmentEnd : ModuleBase
{
	private static readonly FrozenSet<uint> ValidLogMessages = FrozenSet.Create<uint>(983u, 984u, 985u, 986u, 7451u, 7452u);

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoNotifyRecruitmentEndTitle"),
		Description = LanguageManager.Get("AutoNotifyRecruitmentEndDescription"),
		Category = ModuleCategory.Recruitment
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
		if (ValidLogMessages.Contains(logMessageID))
		{
			string logMessageText = LuminaWrapper.GetLogMessageText(logMessageID);
			OmenServiceBase<NotifyHelper>.Instance().NotificationInfo(logMessageText);
			NotifyHelper.Speak(logMessageText);
		}
	}
}
