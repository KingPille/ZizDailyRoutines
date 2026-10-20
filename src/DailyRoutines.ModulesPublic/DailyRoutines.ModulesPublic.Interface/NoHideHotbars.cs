using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.UI;
using OmenTools.Interop.Game.Models;

namespace DailyRoutines.ModulesPublic.Interface;

public class NoHideHotbars : ModuleBase
{
	private unsafe delegate void ToggleUIDelegate(UIModule* module, UiFlags flags, bool isEnable, bool unknown = true);

	private unsafe delegate bool ToggleUI2Delegate(UIModule* module, UiFlags flags, bool isEnable, bool unknown = true);

	private static readonly CompSig ToggleUISig = new CompSig("48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC 20 48 8B 01 41 0F B6 D9");

	private Hook<ToggleUIDelegate>? ToggleUIHook;

	private static readonly CompSig ToggleUI2Sig = new CompSig("48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 48 83 EC 20 41 0F B6 E9 41 0F B6 F0");

	private Hook<ToggleUI2Delegate>? ToggleUI2Hook;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("NoHideHotbarsTitle"),
		Description = LanguageManager.Get("NoHideHotbarsDescription"),
		Category = ModuleCategory.Interface
	};

	protected unsafe override void Init()
	{
		if (ToggleUIHook == null)
		{
			ToggleUIHook = ToggleUISig.GetHook<ToggleUIDelegate>(ToggleUIDetour);
		}
		ToggleUIHook.Enable();
		if (ToggleUI2Hook == null)
		{
			ToggleUI2Hook = ToggleUI2Sig.GetHook<ToggleUI2Delegate>(ToggleUI2Detour);
		}
		ToggleUI2Hook.Enable();
	}

	private unsafe void ToggleUIDetour(UIModule* module, UiFlags flags, bool isEnable, bool unknown = true)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (isEnable)
		{
			ToggleUIHook.Original(module, flags, isEnable, unknown);
		}
	}

	private unsafe bool ToggleUI2Detour(UIModule* module, UiFlags flags, bool isEnable, bool unknown = true)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if (!isEnable)
		{
			return true;
		}
		return ToggleUI2Hook.Original(module, flags, isEnable, unknown);
	}
}
