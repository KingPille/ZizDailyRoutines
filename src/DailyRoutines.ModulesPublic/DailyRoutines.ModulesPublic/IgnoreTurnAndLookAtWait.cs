using System;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Models;

namespace DailyRoutines.ModulesPublic;

public class IgnoreTurnAndLookAtWait : ModuleBase
{
	private unsafe delegate nint EventSceneScriptDelegate(EventSceneModuleImplBase* scene);

	private static readonly CompSig WaitForBaseSig = new CompSig("48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 48 83 EC ?? 48 8B D9 48 8B 49 ?? E8 ?? ?? ?? ?? 48 8B 35");

	private Hook<EventSceneScriptDelegate>? WaitForTurnHook;

	private Hook<EventSceneScriptDelegate>? WaitForLookAtHook;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("IgnoreTurnAndLookAtWaitTitle"),
		Description = LanguageManager.Get("IgnoreTurnAndLookAtWaitDescription"),
		Category = ModuleCategory.System
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		NeedAuth = true
	};

	protected unsafe override void Init()
	{
		nint luaSetupFunctionStartAddress = WaitForBaseSig.ScanText();
		if (WaitForTurnHook == null)
		{
			WaitForTurnHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromAddress<EventSceneScriptDelegate>((IntPtr)luaSetupFunctionStartAddress.GetLuaFunctionByName("WaitForTurn"), (EventSceneScriptDelegate)EventSceneScriptDetour, (HookBackend)0);
		}
		WaitForTurnHook.Enable();
		if (WaitForLookAtHook == null)
		{
			WaitForLookAtHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromAddress<EventSceneScriptDelegate>((IntPtr)luaSetupFunctionStartAddress.GetLuaFunctionByName("WaitForLookAt"), (EventSceneScriptDelegate)EventSceneScriptDetour, (HookBackend)0);
		}
		WaitForLookAtHook.Enable();
	}

	private unsafe static nint EventSceneScriptDetour(EventSceneModuleImplBase* scene)
	{
		return 1;
	}
}
