using System;
using System.Collections.Generic;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Enums;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic.Duty;

public class AutoNotifyDutyConfirm : ModuleBase
{
	private Hook<OnQueuePop>? OnQueuePopHook;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoNotifyDutyConfirmTitle"),
		Description = LanguageManager.Get("AutoNotifyDutyConfirmDescription"),
		Category = ModuleCategory.Duty
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected unsafe override void Init()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		if (OnQueuePopHook == null)
		{
			OnQueuePopHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromMemberFunction<OnQueuePop>(typeof(MemberFunctionPointers), "OnQueuePop", new OnQueuePop(OnQueuePopDetour));
		}
		OnQueuePopHook.Enable();
	}

	private unsafe void OnQueuePopDetour(ContentsFinderQueueInfo* queueInfo, ContentsFinderQueueState state, uint contentFinderConditionID, nint a4, bool isInProgressParty, LootRule lootRule, ulong inProgressPartyStartTimestamp, nint a8, bool isUnrestrictedParty, bool isMinimalIL, bool isSilenceEcho, bool isExplorerMode, bool isLevelSync, bool isLimitedLeveling)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Invalid comparison between Unknown and I4
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Invalid comparison between Unknown and I4
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		ContentsFinderQueueState queueState = ((ContentsFinderQueueInfo)queueInfo).QueueState;
		OnQueuePopHook.Original.Invoke(queueInfo, state, contentFinderConditionID, (IntPtr)a4, isInProgressParty, lootRule, inProgressPartyStartTimestamp, (IntPtr)a8, isUnrestrictedParty, isMinimalIL, isSilenceEcho, isExplorerMode, isLevelSync, isLimitedLeveling);
		bool flag = (int)state != 3;
		if (!flag)
		{
			bool flag2 = (((int)queueState == 0 || (int)queueState == 3) ? true : false);
			flag = flag2;
		}
		if (!flag)
		{
			string contentName = LuminaWrapper.GetContentName(contentFinderConditionID);
			if (!string.IsNullOrEmpty(contentName))
			{
				OmenServiceBase<NotifyHelper>.Instance().TrayInfo(contentName, LanguageManager.Get("AutoNotifyDutyConfirm-Notification"));
				OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoNotifyDutyConfirm-Message", new Dictionary<string, object> { ["duty"] = contentName })), useDefaultPrefix: false);
			}
		}
	}
}
