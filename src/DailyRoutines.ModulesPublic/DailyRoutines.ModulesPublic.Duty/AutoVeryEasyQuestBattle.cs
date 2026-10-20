using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using Lumina.Text.ReadOnly;
using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic.Duty;

public class AutoVeryEasyQuestBattle : ModuleBase
{
	private unsafe delegate void HandleStartOrEndCommandDelegate(QuestEventHandler* thisPtr, uint command);

	private static readonly CompSig HandleStartOrEndCommandSig = new CompSig("4C 8B DC 55 57 41 56 41 57 49 8D AB ?? ?? ?? ?? 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 85 ?? ?? ?? ?? 48 8B F9");

	private Hook<HandleStartOrEndCommandDelegate>? HandleStartOrEndCommandHook;

	private bool needToNotifyThisTime;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoVeryEasyQuestBattleTitle"),
		Description = LanguageManager.Get("AutoVeryEasyQuestBattleDescription"),
		Category = ModuleCategory.Duty
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		NeedAuth = true,
		AllDefaultEnabled = true
	};

	protected unsafe override void Init()
	{
		HandleStartOrEndCommandHook = HandleStartOrEndCommandSig.GetHook<HandleStartOrEndCommandDelegate>(HandleStartOrEndCommandDetour);
		HandleStartOrEndCommandHook.Enable();
		OmenServiceBase<ExecuteCommandManager>.Instance().RegPre(OnPreUseCommand);
	}

	protected override void Uninit()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().Unreg(OnPreUseCommand);
	}

	private unsafe void HandleStartOrEndCommandDetour(QuestEventHandler* thisPtr, uint command)
	{
		needToNotifyThisTime = true;
		HandleStartOrEndCommandHook.Original(thisPtr, command);
		needToNotifyThisTime = false;
	}

	private void OnPreUseCommand(ref bool isPrevented, ref ExecuteCommandFlag command, ref uint param1, ref uint param2, ref uint param3, ref uint param4)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (command == ExecuteCommandFlag.StartSoloQuestBattle)
		{
			param1 = 2u;
			if (needToNotifyThisTime)
			{
				string text = LanguageManager.Get("AutoVeryEasyQuestBattle-Notification");
				OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(text));
				NotifyHelper.Toast(text);
			}
		}
	}
}
