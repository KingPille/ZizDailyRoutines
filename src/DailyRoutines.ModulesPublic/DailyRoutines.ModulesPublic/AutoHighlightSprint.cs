using System.Runtime.InteropServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;

namespace DailyRoutines.ModulesPublic;

public class AutoHighlightSprint : ModuleBase
{
	private Hook<IsActionHighlighted>? IsActionHighlightedHook;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoHighlightSprintTitle"),
		Description = LanguageManager.Get("AutoHighlightSprintDescription", LuminaWrapper.GetGeneralActionName(4u)),
		Category = ModuleCategory.Action
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected unsafe override void Init()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		IsActionHighlightedHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromMemberFunction<IsActionHighlighted>(typeof(MemberFunctionPointers), "IsActionHighlighted", new IsActionHighlighted(IsActionHighlightedDetour));
		IsActionHighlightedHook.Enable();
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private unsafe bool IsActionHighlightedDetour(ActionManager* manager, ActionType actionType, uint actionID)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Invalid comparison between Unknown and I4
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Invalid comparison between Unknown and I4
		if (GameState.ContentFinderCondition != 0 && !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)26] && (((int)actionType == 5 && actionID == 4) || ((int)actionType == 1 && actionID == 3)))
		{
			return true;
		}
		return IsActionHighlightedHook.Original.Invoke(manager, actionType, actionID);
	}
}
