using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Extensions;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Agent;
using Dalamud.Game.Agent.AgentArgTypes;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Hooking;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Common.Lua;
using FFXIVClientStructs.FFXIV.Component.GUI;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.ImGuiOm.Widgets.Combos;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game;
using OmenTools.Interop.Game.Models;
using OmenTools.Interop.Game.Models.Native;
using OmenTools.OmenService;

namespace DailyRoutines.ModulesPublic;

public class AutoCutsceneSkip : ModuleBase
{
	private unsafe delegate byte CutsceneHandleInputDelegate(UIInputModule* module, float a2);

	private unsafe delegate nint PlayCutsceneDelegate(EventFramework* a1, lua_State* state);

	private unsafe delegate bool IsCutsceneSeenDelegate(UIState* state, uint cutsceneID);

	private class Config : ModuleConfig
	{
		public HashSet<uint> BlacklistZones = new HashSet<uint>();

		public HashSet<uint> WhitelistZones = new HashSet<uint>();

		public bool WorkMode;
	}

	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static PlayCutsceneDelegate _003C0_003E__PlayCutsceneDetour;

		public static IsCutsceneSeenDelegate _003C1_003E__IsCutsceneSeenDetour;

		public static LuaFunctionDelegate _003C2_003E__LuaFunctionDetour;

		public static LuaFunctionDelegate _003C3_003E__LuaFunction2Detour;

		public static AgentEventDelegate _003C4_003E__OnAgent;
	}

	private static readonly CompSig CutsceneHandleInputSig = new CompSig("E8 ?? ?? ?? ?? 44 0F B6 E0 48 8B 4E 08");

	private Hook<CutsceneHandleInputDelegate>? CutsceneHandleInputHook;

	private static readonly CompSig PlayCutsceneSig = new CompSig("40 53 55 57 41 56 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 84 24 ?? ?? ?? ?? 48 8B 59");

	private Hook<PlayCutsceneDelegate>? PlayCutsceneHook;

	private static readonly CompSig IsCutsceneSeenSig = new CompSig("E8 ?? ?? ?? ?? 33 D2 0F B6 CB 3A C3");

	private Hook<IsCutsceneSeenDelegate>? IsCutsceneSeenHook;

	private static readonly CompSig LuaBaseSig01 = new CompSig("48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC ?? 48 8B B9 ?? ?? ?? ?? 48 8B D9 48 8B 4F ?? E8 ?? ?? ?? ?? 48 8D 8B ?? ?? ?? ?? 8B F0 E8 ?? ?? ?? ?? 48 8B 4F ?? 48 8B D0 E8 ?? ?? ?? ?? 48 8B 4F ?? BA ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 8B 4F ?? BA ?? ?? ?? ?? E8 ?? ?? ?? ?? 85 C0 74 ?? 4C 8D 0D ?? ?? ?? ?? 48 8B CF 4C 8D 05 ?? ?? ?? ?? 8D 56 ?? E8 ?? ?? ?? ?? 4C 8D 0D ?? ?? ?? ?? 48 8B CF 4C 8D 05 ?? ?? ?? ?? 8D 56 ?? E8 ?? ?? ?? ?? 4C 8D 0D ?? ?? ?? ?? 48 8B CF 4C 8D 05 ?? ?? ?? ?? 8D 56 ?? E8 ?? ?? ?? ?? 48 8B 4F ?? BA ?? ?? ?? ?? 48 8B 5C 24 ?? 48 8B 74 24 ?? 48 83 C4 ?? 5F E9 ?? ?? ?? ?? CC CC CC CC CC CC CC CC CC CC CC CC 48 89 5C 24");

	private Hook<LuaFunctionDelegate>? PlayCutsceneLuaHook;

	private static readonly CompSig LuaBaseSig02 = new CompSig("40 55 56 57 41 55 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 44 24");

	private Hook<LuaFunctionDelegate>? PlayStaffRollHook;

	private Hook<LuaFunctionDelegate>? PlayToBeContinuedHook;

	private Hook<LuaFunctionDelegate>? IsEnterTerritoryEventLoginHook;

	private MemoryPatch cutsceneUnskippablePatch;

	private Config config;

	private ZoneSelectCombo whitelistZoneCombo;

	private ZoneSelectCombo blacklistZoneCombo;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoCutsceneSkipTitle"),
		Description = LanguageManager.Get("AutoCutsceneSkipDescription"),
		Category = ModuleCategory.System
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		NeedAuth = true
	};

	protected unsafe override void Init()
	{
		cutsceneUnskippablePatch = new MemoryPatch("75 ?? 48 8B 4B ?? 48 8B 01 FF 50 ?? 48 8B C8 BA ?? ?? ?? ?? E8 ?? ?? ?? ?? 80 7B", new global::_003C_003Ez__ReadOnlySingleElementList<byte?>((byte)235));
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		whitelistZoneCombo = new ZoneSelectCombo("Whitelist");
		blacklistZoneCombo = new ZoneSelectCombo("Blacklist");
		whitelistZoneCombo.SelectedIDs = config.WhitelistZones;
		blacklistZoneCombo.SelectedIDs = config.BlacklistZones;
		cutsceneUnskippablePatch.Set(enable: true);
		if (CutsceneHandleInputHook == null)
		{
			CutsceneHandleInputHook = CutsceneHandleInputSig.GetHook<CutsceneHandleInputDelegate>(CutsceneHandleInputDetour);
		}
		if (PlayCutsceneHook == null)
		{
			PlayCutsceneHook = PlayCutsceneSig.GetHook<PlayCutsceneDelegate>(PlayCutsceneDetour);
		}
		if (IsCutsceneSeenHook == null)
		{
			IsCutsceneSeenHook = IsCutsceneSeenSig.GetHook<IsCutsceneSeenDelegate>(IsCutsceneSeenDetour);
		}
		nint luaSetupFunctionStartAddress = LuaBaseSig01.ScanText();
		if (PlayCutsceneLuaHook == null)
		{
			PlayCutsceneLuaHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromAddress<LuaFunctionDelegate>((IntPtr)luaSetupFunctionStartAddress.GetLuaFunctionByName("PlayCutScene"), (LuaFunctionDelegate)LuaFunctionDetour, (HookBackend)0);
		}
		nint luaSetupFunctionStartAddress2 = LuaBaseSig02.ScanText();
		if (PlayStaffRollHook == null)
		{
			PlayStaffRollHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromAddress<LuaFunctionDelegate>((IntPtr)luaSetupFunctionStartAddress2.GetLuaFunctionByName("PlayStaffRoll"), (LuaFunctionDelegate)LuaFunction2Detour, (HookBackend)0);
		}
		if (PlayToBeContinuedHook == null)
		{
			PlayToBeContinuedHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromAddress<LuaFunctionDelegate>((IntPtr)luaSetupFunctionStartAddress2.GetLuaFunctionByName("PlayToBeContinued"), (LuaFunctionDelegate)LuaFunction2Detour, (HookBackend)0);
		}
		if (IsEnterTerritoryEventLoginHook == null)
		{
			IsEnterTerritoryEventLoginHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromAddress<LuaFunctionDelegate>((IntPtr)luaSetupFunctionStartAddress2.GetLuaFunctionByName("IsEnterTerritoryEventLogin"), (LuaFunctionDelegate)LuaFunction2Detour, (HookBackend)0);
		}
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += OnZoneChanged;
		OnZoneChanged(0u);
	}

	protected override void Uninit()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChanged;
		IAgentLifecycle obj = DalamudServiceExtension.Instance<IAgentLifecycle>();
		AgentEventDelegate[] array = new AgentEventDelegate[1];
		object obj2 = _003C_003EO._003C4_003E__OnAgent;
		if (obj2 == null)
		{
			AgentEventDelegate val = OnAgent;
			_003C_003EO._003C4_003E__OnAgent = val;
			obj2 = (object)val;
		}
		array[0] = (AgentEventDelegate)obj2;
		obj.UnregisterListener((AgentEventDelegate[])(object)array);
	}

	protected override void ConfigUI()
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		using (ImRaiiExtension.Heading1(LanguageManager.Get("WorkMode")))
		{
			ItemWidthDisposable val = ImRaii.ItemWidth(200f * Globals.GlobalUIScale);
			try
			{
				uint colorU = ImGui.GetColorU32((ImGuiCol)7);
				if (ImGuiOm.ToggleButton("WorkMode", ref config.WorkMode, 0f, colorU, colorU))
				{
					config.Save(this);
				}
				ImGui.SameLine();
				ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get(config.WorkMode ? "Whitelist" : "Blacklist")));
				ImGuiOm.HelpMarker(LanguageManager.Get("AutoCutsceneSkip-WorkModeHelp"), 40f, (FontAwesomeIcon)61530);
				if (config.WorkMode)
				{
					if (whitelistZoneCombo.DrawCheckbox())
					{
						config.WhitelistZones = whitelistZoneCombo.SelectedIDs;
						config.Save(this);
					}
				}
				else if (blacklistZoneCombo.DrawCheckbox())
				{
					config.BlacklistZones = blacklistZoneCombo.SelectedIDs;
					config.Save(this);
				}
				ImGui.SameLine();
				ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("Zone")));
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
	}

	private void OnZoneChanged(uint u)
	{
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Expected O, but got Unknown
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Expected O, but got Unknown
		bool flag = !IsProhibitToSkipInZone();
		CutsceneHandleInputHook.Toggle<CutsceneHandleInputDelegate>(flag);
		PlayCutsceneHook.Toggle<PlayCutsceneDelegate>(flag);
		PlayCutsceneLuaHook.Toggle<LuaFunctionDelegate>(flag);
		IsCutsceneSeenHook.Toggle<IsCutsceneSeenDelegate>(flag);
		PlayStaffRollHook.Toggle<LuaFunctionDelegate>(flag);
		PlayToBeContinuedHook.Toggle<LuaFunctionDelegate>(flag);
		IsEnterTerritoryEventLoginHook.Toggle<LuaFunctionDelegate>(flag);
		if (flag)
		{
			IAgentLifecycle obj = DalamudServiceExtension.Instance<IAgentLifecycle>();
			object obj2 = _003C_003EO._003C4_003E__OnAgent;
			if (obj2 == null)
			{
				AgentEventDelegate val = OnAgent;
				_003C_003EO._003C4_003E__OnAgent = val;
				obj2 = (object)val;
			}
			obj.RegisterListener((AgentEvent)1, (AgentId)458, (AgentEventDelegate)obj2);
			return;
		}
		IAgentLifecycle obj3 = DalamudServiceExtension.Instance<IAgentLifecycle>();
		AgentEventDelegate[] array = new AgentEventDelegate[1];
		object obj4 = _003C_003EO._003C4_003E__OnAgent;
		if (obj4 == null)
		{
			AgentEventDelegate val2 = OnAgent;
			_003C_003EO._003C4_003E__OnAgent = val2;
			obj4 = (object)val2;
		}
		array[0] = (AgentEventDelegate)obj4;
		obj3.UnregisterListener((AgentEventDelegate[])(object)array);
	}

	private unsafe static void OnAgent(AgentEvent type, AgentArgs args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		AgentArgs obj = ((args is AgentReceiveEventArgs) ? args : null);
		AgentPointMenu* address = (AgentPointMenu*)obj.Agent.Address;
		AtkValue* atkValues = (AtkValue*)((AgentReceiveEventArgs)obj).AtkValues;
		if (((AtkValue)atkValues).Int == 12 && ((AgentPointMenu)address).Context != null)
		{
			int num = (*address).FindFirstUncompleteEntry();
			if (num < 0)
			{
				((AgentPointMenu)address).Hide();
				return;
			}
			((AgentPointMenu)address).SelectedIndex = num;
			((AgentPointMenu)address).SendEntryToAddon((uint)num);
			((AgentPointMenu)address).Hide();
		}
	}

	private unsafe byte CutsceneHandleInputDetour(UIInputModule* module, float a2)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if (!DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)35])
		{
			return CutsceneHandleInputHook.Original(module, a2);
		}
		AtkEventInterface* cutsceneSkipCallback = ((UIInputModule)module).CutsceneSkipCallback;
		if (cutsceneSkipCallback != null && Addons.JournalResult == null && Addons.SatisfactionSupplyResult == null)
		{
			AtkValue val = new AtkValue
			{
				Type = (AtkValueType)3,
				Int = 0
			};
			AtkValue val2 = default(AtkValue);
			((AtkEventInterface)cutsceneSkipCallback).ReceiveEvent(&val2, &val, 1u, 11uL);
		}
		return CutsceneHandleInputHook.Original(module, a2);
	}

	private unsafe static nint PlayCutsceneDetour(EventFramework* framework, lua_State* state)
	{
		return 1;
	}

	private unsafe static ulong LuaFunctionDetour(lua_State* state)
	{
		TValue* top = ((lua_State)state).top;
		((TValue)top).tt = 2;
		((Value)(&((TValue)top).value)).n = 1.0;
		((lua_State)state).top += Unsafe.SizeOf<TValue>();
		return 1uL;
	}

	private unsafe static ulong LuaFunction2Detour(lua_State* _)
	{
		return 1uL;
	}

	private unsafe static bool IsCutsceneSeenDetour(UIState* state, uint cutsceneID)
	{
		return true;
	}

	private bool IsProhibitToSkipInZone()
	{
		uint territoryType = GameState.TerritoryType;
		if (config.WorkMode)
		{
			return !config.WhitelistZones.Contains(territoryType);
		}
		return config.BlacklistZones.Contains(territoryType);
	}
}
