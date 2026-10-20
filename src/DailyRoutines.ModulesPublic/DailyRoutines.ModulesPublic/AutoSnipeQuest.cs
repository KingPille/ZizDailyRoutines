using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Common.Lua;
using OmenTools.Interop.Game.Models;

namespace DailyRoutines.ModulesPublic;

public class AutoSnipeQuest : ModuleBase
{
	private unsafe delegate ulong EnqueueSnipeTaskDelegate(EventSceneModuleImplBase* scene, lua_State* state);

	private static readonly CompSig EnqueueSnipeTaskSig = new CompSig("48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 48 83 EC ?? 48 8B F9 48 8D 4C 24 ?? E8 ?? ?? ?? ?? 48 8B 4C 24");

	private Hook<EnqueueSnipeTaskDelegate> EnqueueSnipeTaskHook;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoSnipeQuestTitle"),
		Description = LanguageManager.Get("AutoSnipeQuestDescription"),
		Category = ModuleCategory.System
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		NeedAuth = true
	};

	protected unsafe override void Init()
	{
		if (EnqueueSnipeTaskHook == null)
		{
			EnqueueSnipeTaskHook = EnqueueSnipeTaskSig.GetHook<EnqueueSnipeTaskDelegate>(EnqueueSnipeTaskDetour);
		}
		EnqueueSnipeTaskHook.Enable();
	}

	private unsafe static ulong EnqueueSnipeTaskDetour(EventSceneModuleImplBase* scene, lua_State* state)
	{
		TValue* top = ((lua_State)state).top;
		((TValue)top).tt = 3;
		((Value)(&((TValue)top).value)).n = 1.0;
		((lua_State)state).top += Unsafe.SizeOf<TValue>();
		return 1uL;
	}
}
