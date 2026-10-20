using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;

namespace DailyRoutines.ModulesPublic;

public class AutoReplaceLowBlowWithInterject : ModuleBase
{
	private delegate bool IsActionReplaceableDelegate(uint actionID);

	private unsafe delegate uint GetAdjustedActionIDDelegate(ActionManager* manager, uint actionID);

	private unsafe delegate uint GetIconIDForSlotDelegate(HotbarSlot* slot, HotbarSlotType type, uint actionID);

	private static readonly CompSig IsActionReplaceableSig = new CompSig("40 53 48 83 EC ?? 8B D9 48 8B 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 85 C0 74 ?? 48 8B 10 48 8B C8 FF 92 ?? ?? ?? ?? 8B D3");

	private Hook<IsActionReplaceableDelegate> IsActionReplaceableHook;

	private static readonly CompSig GetAdjustedActionIDSig = new CompSig("E8 ?? ?? ?? ?? 89 03 8B 03");

	private Hook<GetAdjustedActionIDDelegate> GetAdjustedActionIDHook;

	private static readonly CompSig GetIconIDForSlotSig = new CompSig("E8 ?? ?? ?? ?? 85 C0 89 83 ?? ?? ?? ?? 0F 94 C0");

	private Hook<GetIconIDForSlotDelegate> GetIconIDForSlotHook;

	private const uint LOW_BLOW_ACTION = 7540u;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoReplaceLowBlowWithInterjectTitle"),
		Description = LanguageManager.Get("AutoReplaceLowBlowWithInterjectDescription"),
		Category = ModuleCategory.Action
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected unsafe override void Init()
	{
		if (IsActionReplaceableHook == null)
		{
			IsActionReplaceableHook = IsActionReplaceableSig.GetHook<IsActionReplaceableDelegate>(IsActionReplaceableDetour);
		}
		IsActionReplaceableHook.Enable();
		if (GetAdjustedActionIDHook == null)
		{
			GetAdjustedActionIDHook = GetAdjustedActionIDSig.GetHook<GetAdjustedActionIDDelegate>(GetAdjustedActionIDDetour);
		}
		GetAdjustedActionIDHook.Enable();
		if (GetIconIDForSlotHook == null)
		{
			GetIconIDForSlotHook = GetIconIDForSlotSig.GetHook<GetIconIDForSlotDelegate>(GetIconIDForSlotDetour);
		}
		GetIconIDForSlotHook.Enable();
	}

	private bool IsActionReplaceableDetour(uint actionID)
	{
		if (actionID != 7540)
		{
			return IsActionReplaceableHook.Original(actionID);
		}
		return true;
	}

	private unsafe uint GetAdjustedActionIDDetour(ActionManager* manager, uint actionID)
	{
		if (actionID != 7540 || !IsReplaceNeeded())
		{
			return GetAdjustedActionIDHook.Original(manager, actionID);
		}
		return 7538u;
	}

	private unsafe uint GetIconIDForSlotDetour(HotbarSlot* slot, HotbarSlotType type, uint actionID)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if ((int)type != 1 || actionID != 7540 || !IsReplaceNeeded())
		{
			return GetIconIDForSlotHook.Original(slot, type, actionID);
		}
		return 808u;
	}

	private unsafe static bool IsReplaceNeeded()
	{
		if (((ActionManager)ActionManager.Instance()).IsActionOffCooldown((ActionType)1, 7538u))
		{
			if (TargetManager.Target is IBattleChara battleChara)
			{
				return battleChara.IsCastInterruptible;
			}
			return false;
		}
		return false;
	}
}
