using System;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Hooking;
using Dalamud.Memory;
using OmenTools.Interop.Game.Models;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules;

public class AutoUnlockExpertDelivery : ModuleBase
{
	private unsafe delegate void* jw5p02p8JeNmEnS1vEp(nint agent0, nint agent1, int a3);

	private static readonly CompSig InitGrandCompanyRankInfoSig;

	private Hook<jw5p02p8JeNmEnS1vEp>? InitGrandCompanyRankInfoHook;

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected unsafe override void Init()
	{
		if (InitGrandCompanyRankInfoHook == null)
		{
			InitGrandCompanyRankInfoHook = InitGrandCompanyRankInfoSig.GetHook<jw5p02p8JeNmEnS1vEp>(eJq49hLmYj);
		}
		InitGrandCompanyRankInfoHook.Enable();
	}

	private unsafe void* eJq49hLmYj(nint agent0, nint agent1, int a3)
	{
		void* result = InitGrandCompanyRankInfoHook.Original(agent0, agent1, a3);
		MemoryHelper.Write<bool>((IntPtr)(agent0 + 148), true);
		return result;
	}

	public AutoUnlockExpertDelivery()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6AAE67A8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_839f542231154d29b0562b687afa328a)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x66B3A0BC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_762f55d0e7ef4ecd8f3f55c0f8a10fae)),
			Category = ModuleCategory.System
		};
		Permission = new ModulePermission
		{
			NeedAuth = true,
			AllDefaultEnabled = true
		};
		base._002Ector();
	}

	static AutoUnlockExpertDelivery()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		InitGrandCompanyRankInfoSig = new CompSig(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7C2F3AD8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_12c2d68ba4654622a4959ee73cacc6d7));
	}
}
