using System;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Hooking;
using OmenTools.Interop.Game.Models;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules;

public class AutoRemoveStatusSpeedPenalty : ModuleBase
{
	private unsafe delegate uint LpceokxTdusjehfctYd(void* a1, void* a2);

	private static readonly CompSig CalculateSpeedStatusModifierSig;

	private Hook<LpceokxTdusjehfctYd>? CalculateSpeedStatusModifierHook;

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected unsafe override void Init()
	{
		if (CalculateSpeedStatusModifierHook == null)
		{
			CalculateSpeedStatusModifierHook = CalculateSpeedStatusModifierSig.GetHook<LpceokxTdusjehfctYd>(GUN14WaZnJ);
		}
		CalculateSpeedStatusModifierHook.Enable();
	}

	private unsafe uint GUN14WaZnJ(void* a1, void* a2)
	{
		return Math.Max(CalculateSpeedStatusModifierHook.Original(a1, a2), 100u);
	}

	public AutoRemoveStatusSpeedPenalty()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489BA31C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cade14e68bb84906959f3cda5c496848)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2DBD2A5E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5d8d924f1edf4ec39f023b3c7bb646cc)),
			Category = ModuleCategory.Combat
		};
		Permission = new ModulePermission
		{
			NeedAuth = true,
			CNPremium = true,
			TCPremium = true
		};
		base._002Ector();
	}

	static AutoRemoveStatusSpeedPenalty()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		CalculateSpeedStatusModifierSig = new CompSig(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6C352DD4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88a2dcd1996b4a3fbefaf51a98b59def));
	}
}
