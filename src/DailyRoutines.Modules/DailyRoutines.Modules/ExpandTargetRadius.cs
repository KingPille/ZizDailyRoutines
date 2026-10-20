using System.Runtime.InteropServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.Interop.Game.Models;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules;

public class ExpandTargetRadius : ModuleBase
{
	private unsafe delegate float IKbL5Mx1UkRgqPSZ5e4(GameObject* gameObj, byte adjustByTransformation);

	[return: MarshalAs(UnmanagedType.U1)]
	private unsafe delegate bool syDJkDxML8cAX9CaVMZ(EmoteManager* manager, ushort emoteID, PlayEmoteOption* options);

	private class I16RdKx41qwMqSpfCfV : ModuleConfig
	{
		public float ExpandedRange;

		public I16RdKx41qwMqSpfCfV()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			ExpandedRange = 2f;
			base._002Ector();
		}
	}

	private static readonly CompSig GetGameObjectRadiusSig;

	private Hook<IKbL5Mx1UkRgqPSZ5e4>? GetGameObjectRadiusHook;

	private Hook<syDJkDxML8cAX9CaVMZ>? UseEmoteHook;

	private I16RdKx41qwMqSpfCfV config;

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected unsafe override void Init()
	{
		config = ModuleConfigExtension.Load<I16RdKx41qwMqSpfCfV>(this) ?? new I16RdKx41qwMqSpfCfV();
		if (GetGameObjectRadiusHook == null)
		{
			GetGameObjectRadiusHook = GetGameObjectRadiusSig.GetHook<IKbL5Mx1UkRgqPSZ5e4>(Eq81s7qwJn);
		}
		GetGameObjectRadiusHook.Enable();
		if (UseEmoteHook == null)
		{
			UseEmoteHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromMemberFunction<syDJkDxML8cAX9CaVMZ>(typeof(MemberFunctionPointers), LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x469A35AF ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_bb1eb496bad948e4a2baaa3e1edc7a92), u7x18XEprd);
		}
		UseEmoteHook.Enable();
	}

	protected override void ConfigUI()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		ImGui.SetNextItemWidth(100f * Globals.GlobalUIScale);
		ImGui.InputFloat(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7FD89C47 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1eab987c1db64b928220e27bd03141d3))), ref config.ExpandedRange, 0f, 0f, ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489BBEDC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cade14e68bb84906959f3cda5c496848)), (ImGuiInputTextFlags)0);
		if (ImGui.IsItemDeactivatedAfterEdit())
		{
			config.Save(this);
		}
	}

	private unsafe float Eq81s7qwJn(GameObject* gameObj, byte adjustByTransformation)
	{
		return GetGameObjectRadiusHook.Original(gameObj, adjustByTransformation) + config.ExpandedRange;
	}

	private unsafe bool u7x18XEprd(EmoteManager* manager, ushort emoteID, PlayEmoteOption* options)
	{
		GetGameObjectRadiusHook.Disable();
		bool result = UseEmoteHook.Original(manager, emoteID, options);
		GetGameObjectRadiusHook.Enable();
		return result;
	}

	public ExpandTargetRadius()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x76EB8639 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b988ddaee02846b69dd4e446b5da9de9)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x18CCB87E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_33babcee9ecc460a9e530039fd18d361)),
			Category = ModuleCategory.Combat,
			ModulesConflict = new string[1] { LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x454E1ADD ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_09da05f368544962acb62b42d9155467) }
		};
		Permission = new ModulePermission
		{
			NeedAuth = true
		};
		base._002Ector();
	}

	static ExpandTargetRadius()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		GetGameObjectRadiusSig = new CompSig(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6854F149 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_23522a2a14314da28eb44187a09cd0b5));
	}
}
