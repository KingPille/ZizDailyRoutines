using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using OmenTools.Interop.Game.Models;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules;

public class AutoBlockMisdirectionState : ModuleBase
{
	private delegate bool urxSpAxSlYfdM5miTPw();

	private unsafe delegate void mbr52BxVcgnTrWnLUX2(TerritoryInfo* instance, uint a2);

	private static readonly CompSig CouldSetMisdirectionControlStateSig;

	private Hook<urxSpAxSlYfdM5miTPw>? CouldSetMisdirectionControlStateHook;

	private static readonly CompSig SetMisdirectionBeaconSig;

	private Hook<mbr52BxVcgnTrWnLUX2>? SetMisdirectionBeaconHook;

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected unsafe override void Init()
	{
		if (SetMisdirectionBeaconHook == null)
		{
			SetMisdirectionBeaconHook = SetMisdirectionBeaconSig.GetHook<mbr52BxVcgnTrWnLUX2>(tcX1jbiVLA);
		}
		SetMisdirectionBeaconHook.Enable();
		if (CouldSetMisdirectionControlStateHook == null)
		{
			CouldSetMisdirectionControlStateHook = CouldSetMisdirectionControlStateSig.GetHook<urxSpAxSlYfdM5miTPw>(WeZ1CqjpXs);
		}
		CouldSetMisdirectionControlStateHook.Enable();
	}

	private static bool WeZ1CqjpXs()
	{
		return false;
	}

	private unsafe static void tcX1jbiVLA(TerritoryInfo* instance, uint a2)
	{
	}

	public AutoBlockMisdirectionState()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x50174572 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e227921f7ee94b6aa1c28169395ba0cf)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x1778B34D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f7cf93b006904393bb56d82c0a688fa1)),
			Category = ModuleCategory.Combat
		};
		Permission = new ModulePermission
		{
			NeedAuth = true
		};
		base._002Ector();
	}

	static AutoBlockMisdirectionState()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		CouldSetMisdirectionControlStateSig = new CompSig(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7FD894FD ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1eab987c1db64b928220e27bd03141d3));
		SetMisdirectionBeaconSig = new CompSig(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5017441A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e227921f7ee94b6aa1c28169395ba0cf));
	}
}
