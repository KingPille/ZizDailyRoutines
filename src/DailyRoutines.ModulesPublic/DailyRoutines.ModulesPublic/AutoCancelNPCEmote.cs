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

public class AutoCancelNPCEmote : ModuleBase
{
	private unsafe delegate nint EventSceneScriptDelegate(EventSceneModuleImplBase* scene);

	private CompSig waitForBaseSig;

	private Hook<EventSceneScriptDelegate>? WaitForActionTimelineHook;

	private Hook<EventSceneScriptDelegate>? WaitForActionTimelineLoadHook;

	private Hook<EventSceneScriptDelegate>? PlayActionTimelineHook;

	private Hook<EventSceneScriptDelegate>? PlayEmoteHook;

	private Hook<EventSceneScriptDelegate>? CancelEmoteHook;

	private Hook<EventSceneScriptDelegate>? WaitForEmoteHook;

	private Hook<EventSceneScriptDelegate>? IsEmotingHook;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoCancelNPCEmoteTitle"),
		Description = LanguageManager.Get("AutoCancelNPCEmoteDescription"),
		Category = ModuleCategory.System
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		NeedAuth = true
	};

	protected unsafe override void Init()
	{
		waitForBaseSig = new CompSig("48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 48 83 EC ?? 48 8B D9 48 8B 49 ?? E8 ?? ?? ?? ?? 48 8B 35");
		nint luaSetupFunctionStartAddress = waitForBaseSig.ScanText();
		if (WaitForActionTimelineHook == null)
		{
			WaitForActionTimelineHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromAddress<EventSceneScriptDelegate>((IntPtr)luaSetupFunctionStartAddress.GetLuaFunctionByName("WaitForActionTimeline"), (EventSceneScriptDelegate)EventSceneScriptDetour, (HookBackend)0);
		}
		WaitForActionTimelineHook.Enable();
		if (PlayActionTimelineHook == null)
		{
			PlayActionTimelineHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromAddress<EventSceneScriptDelegate>((IntPtr)luaSetupFunctionStartAddress.GetLuaFunctionByName("PlayActionTimeline"), (EventSceneScriptDelegate)EventSceneScriptDetour, (HookBackend)0);
		}
		PlayActionTimelineHook.Enable();
		if (WaitForActionTimelineLoadHook == null)
		{
			WaitForActionTimelineLoadHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromAddress<EventSceneScriptDelegate>((IntPtr)luaSetupFunctionStartAddress.GetLuaFunctionByName("WaitForActionTimelineLoad"), (EventSceneScriptDelegate)EventSceneScriptDetour, (HookBackend)0);
		}
		WaitForActionTimelineLoadHook.Enable();
		if (PlayEmoteHook == null)
		{
			PlayEmoteHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromAddress<EventSceneScriptDelegate>((IntPtr)luaSetupFunctionStartAddress.GetLuaFunctionByName("PlayEmote"), (EventSceneScriptDelegate)EventSceneScriptDetour, (HookBackend)0);
		}
		PlayEmoteHook.Enable();
		if (CancelEmoteHook == null)
		{
			CancelEmoteHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromAddress<EventSceneScriptDelegate>((IntPtr)luaSetupFunctionStartAddress.GetLuaFunctionByName("CancelEmote"), (EventSceneScriptDelegate)EventSceneScriptDetour, (HookBackend)0);
		}
		CancelEmoteHook.Enable();
		if (WaitForEmoteHook == null)
		{
			WaitForEmoteHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromAddress<EventSceneScriptDelegate>((IntPtr)luaSetupFunctionStartAddress.GetLuaFunctionByName("WaitForEmote"), (EventSceneScriptDelegate)EventSceneScriptDetour, (HookBackend)0);
		}
		WaitForEmoteHook.Enable();
		if (IsEmotingHook == null)
		{
			IsEmotingHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromAddress<EventSceneScriptDelegate>((IntPtr)luaSetupFunctionStartAddress.GetLuaFunctionByName("IsEmoting"), (EventSceneScriptDelegate)EventSceneScriptNoDetour, (HookBackend)0);
		}
		IsEmotingHook.Enable();
	}

	private unsafe static nint EventSceneScriptDetour(EventSceneModuleImplBase* scene)
	{
		return 1;
	}

	private unsafe static nint EventSceneScriptNoDetour(EventSceneModuleImplBase* scene)
	{
		return 0;
	}
}
