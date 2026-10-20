using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.ModulesPublic.Interface;

public class AutoOpenNeedGreed : ModuleBase
{
	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoOpenNeedGreedTitle"),
		Description = LanguageManager.Get("AutoOpenNeedGreedDescription"),
		Category = ModuleCategory.Interface
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected override void Init()
	{
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 3000
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		OmenServiceBase<LogMessageManager>.Instance().RegPost(OnPost);
		OmenServiceBase<TargetManager>.Instance().RegPostInteractWithObject(OnPostInteractWithObject);
	}

	protected override void Uninit()
	{
		OmenServiceBase<LogMessageManager>.Instance().Unreg(OnPost);
		OmenServiceBase<TargetManager>.Instance().Unreg(OnPostInteractWithObject);
	}

	private static void OnPostInteractWithObject(ulong result, IGameObject? target, bool checkLoS)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		if (result != 0L && target != null && (int)target.ObjectKind == 4)
		{
			Throttler.Shared.Throttle("AutoOpenNeedGreed-SelfOpen", 1000u, reThrottle: true);
		}
	}

	private unsafe static void OnPost(uint logMessageID, LogMessageQueueItem item)
	{
		if (logMessageID == 5194 && Throttler.Shared.Check("AutoOpenNeedGreed-SelfOpen"))
		{
			((AgentId)4).SendEvent(0uL, 0, 2, " ");
		}
	}
}
