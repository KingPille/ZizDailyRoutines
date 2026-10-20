using System;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.Command;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using Lumina.Excel.Sheets;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.Helpers;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;

namespace DailyRoutines.ModulesPublic;

public class JoinExitDutyCommand : ModuleBase
{
	private const uint TARGET_CONTENT = 56u;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("JoinExitDutyCommandTitle"),
		Description = LanguageManager.Get("JoinExitDutyCommandDescription"),
		Category = ModuleCategory.Assist,
		ModulesPrerequisite = new string[1] { "AutoCommenceDuty" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected override void Init()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Expected O, but got Unknown
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 15000
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		OmenServiceBase<CommandManager>.Instance().AddSubCommand("joinexitduty", new CommandInfo(new HandlerDelegate(OnCommand))
		{
			HelpMessage = Info.Title
		});
	}

	protected override void Uninit()
	{
		OmenServiceBase<CommandManager>.Instance().RemoveSubCommand("joinexitduty");
	}

	private void OnCommand(string command, string arguments)
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		ContentFinderCondition item;
		if (DalamudServiceExtension.Instance<IPartyList>().Length > 0)
		{
			OmenServiceBase<NotifyHelper>.Instance().NotificationError(LanguageManager.Get("JoinExitDutyCommand-AlreadyInParty"));
		}
		else if (IConditionExtension.get_IsBoundByDuty(DalamudServiceExtension.Instance<ICondition>()))
		{
			OmenServiceBase<NotifyHelper>.Instance().NotificationError(LanguageManager.Get("JoinExitDutyCommand-AlreadyInDutyNotice"));
		}
		else if (LuminaGetter.TryGetRow<ContentFinderCondition>(56u, out item))
		{
			if (!UIState.IsInstanceContentUnlocked(56u))
			{
				OmenServiceBase<NotifyHelper>.Instance().NotificationError(LanguageManager.Get("JoinExitDutyCommand-DutyLockedNotice", ((object)((ContentFinderCondition)(ref item)).Name/*cast due to constrained. prefix*/).ToString()));
			}
			else
			{
				base.TaskHelper.Abort();
				EnqueueARound(56u, ((ContentFinderCondition)(ref item)).AllowExplorerMode);
			}
		}
	}

	private void EnqueueARound(uint targetContent, bool isExplorerMode)
	{
		base.TaskHelper.Enqueue((Func<bool>)CheckAndSwitchJob, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
		base.TaskHelper.Enqueue(() => ContentsFinderHelper.RequestDutyNormal(targetContent, new ContentsFinderOption
		{
			Config817to820 = true,
			UnrestrictedParty = true,
			ExplorerMode = isExplorerMode
		}));
		base.TaskHelper.Enqueue(() => ExitDuty(targetContent));
	}

	private unsafe bool CheckAndSwitchJob()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
		if (localPlayer == null)
		{
			base.TaskHelper.Abort();
			return true;
		}
		uint rowId = localPlayer.ClassJob.RowId;
		if (rowId >= 8 && rowId <= 18)
		{
			RaptureGearsetModule* ptr = RaptureGearsetModule.Instance();
			for (int i = 0; i < 100; i++)
			{
				GearsetEntry* gearset = ((RaptureGearsetModule)ptr).GetGearset(i);
				if (gearset != null && ((Enum)((GearsetEntry)gearset).Flags).HasFlag((Enum)(object)(GearsetFlag)1) && !((Enum)((GearsetEntry)gearset).Flags).HasFlag((Enum)(object)(GearsetFlag)4) && ((GearsetEntry)gearset).Id == i && ((GearsetEntry)gearset).ClassJob > 18)
				{
					OmenServiceBase<ChatManager>.Instance().SendMessage($"/gearset change {((GearsetEntry)gearset).Id + 1}");
					return true;
				}
			}
		}
		return true;
	}

	private unsafe static bool ExitDuty(uint targetContent)
	{
		if (((GameMain)GameMain.Instance()).CurrentContentFinderConditionId != targetContent)
		{
			return false;
		}
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.FinishTerritoryTransport);
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.LeaveDuty);
		return true;
	}
}
