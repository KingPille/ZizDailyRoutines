using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Hooking;
using OmenTools.Interop.Game.Models;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules;

public class AutoImmunizeThinIce : ModuleBase
{
	private delegate void DAMBwixtENl9yRrxOUH(short a1, float a2);

	private delegate void SOX2OexW7bTHe5EQWNL();

	private static readonly CompSig SetThinIceStateSig;

	private Hook<DAMBwixtENl9yRrxOUH>? SetThinIceStateHook;

	private static readonly CompSig RemoveThinIceStateSig;

	private SOX2OexW7bTHe5EQWNL? RemoveThinIceState;

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected override void Init()
	{
		if (RemoveThinIceState == null)
		{
			RemoveThinIceState = RemoveThinIceStateSig.GetDelegate<SOX2OexW7bTHe5EQWNL>();
		}
		if (SetThinIceStateHook == null)
		{
			SetThinIceStateHook = SetThinIceStateSig.GetHook<DAMBwixtENl9yRrxOUH>(F1G1MOfVpU);
		}
		SetThinIceStateHook.Enable();
		RemoveThinIceState();
	}

	private void F1G1MOfVpU(short a1, float a2)
	{
		RemoveThinIceState();
	}

	public AutoImmunizeThinIce()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x477E0D07 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_ba27244341a5414496a87eae29cf3dec)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x109C6CFE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_44b9808560124617907f0048883b8948)),
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

	static AutoImmunizeThinIce()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		SetThinIceStateSig = new CompSig(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5CE3262D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3054c6e6ef664be696bb2ad005864818));
		RemoveThinIceStateSig = new CompSig(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x43D72728 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_974d377e1ed74986b81c34c95c2cc846));
	}
}
