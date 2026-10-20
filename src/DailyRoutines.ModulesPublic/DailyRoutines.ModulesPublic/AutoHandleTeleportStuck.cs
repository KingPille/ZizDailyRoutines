using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using OmenTools.Info.Game.Packets.Upstream;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class AutoHandleTeleportStuck : ModuleBase
{
	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoHandleTeleportStuckTitle"),
		Description = LanguageManager.Get("AutoHandleTeleportStuckDescription", LuminaWrapper.GetLogMessageText(1665u)),
		Category = ModuleCategory.System
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected override void Init()
	{
		OmenServiceBase<LogMessageManager>.Instance().RegPre(OnReceiveLogMessage);
	}

	protected override void Uninit()
	{
		OmenServiceBase<LogMessageManager>.Instance().Unreg(OnReceiveLogMessage);
	}

	private static void OnReceiveLogMessage(ref bool isPrevented, ref uint logMessageID, ref LogMessageQueueItem values)
	{
		if (logMessageID == 1665)
		{
			isPrevented = true;
			new UseActionPacket((ActionType)5, 7u, LocalPlayerState.EntityID, 0f).Send();
		}
	}
}
