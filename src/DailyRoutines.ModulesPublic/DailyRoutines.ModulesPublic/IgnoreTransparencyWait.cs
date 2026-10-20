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

public class IgnoreTransparencyWait : ModuleBase
{
	private unsafe delegate nint EventSceneScriptDelegate(EventSceneModuleImplBase* scene);

	private static readonly CompSig WaitForBaseSig = new CompSig("48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 48 83 EC ?? 48 8B D9 48 8B 49 ?? E8 ?? ?? ?? ?? 48 8B 35");

	private Hook<EventSceneScriptDelegate>? WaitForTransparencyHook;

	private Hook<EventSceneScriptDelegate>? WaitForMoveHook;

	private Hook<EventSceneScriptDelegate>? WaitForPathMoveHook;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("IgnoreTransparencyWaitTitle"),
		Description = LanguageManager.Get("IgnoreTransparencyWaitDescription"),
		Category = ModuleCategory.System
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		NeedAuth = true
	};

	protected unsafe override void Init()
	{
		nint luaSetupFunctionStartAddress = WaitForBaseSig.ScanText();
		if (WaitForTransparencyHook == null)
		{
			WaitForTransparencyHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromAddress<EventSceneScriptDelegate>((IntPtr)luaSetupFunctionStartAddress.GetLuaFunctionByName("WaitForTransparency"), (EventSceneScriptDelegate)EventSceneScriptDetour, (HookBackend)0);
		}
		WaitForTransparencyHook.Enable();
		if (WaitForMoveHook == null)
		{
			WaitForMoveHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromAddress<EventSceneScriptDelegate>((IntPtr)luaSetupFunctionStartAddress.GetLuaFunctionByName("WaitForMove"), (EventSceneScriptDelegate)EventSceneScriptDetour, (HookBackend)0);
		}
		WaitForMoveHook.Enable();
		if (WaitForPathMoveHook == null)
		{
			WaitForPathMoveHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromAddress<EventSceneScriptDelegate>((IntPtr)luaSetupFunctionStartAddress.GetLuaFunctionByName("WaitForPathMove"), (EventSceneScriptDelegate)EventSceneScriptDetour, (HookBackend)0);
		}
		WaitForPathMoveHook.Enable();
	}

	private unsafe static nint EventSceneScriptDetour(EventSceneModuleImplBase* scene)
	{
		return 1;
	}
}
