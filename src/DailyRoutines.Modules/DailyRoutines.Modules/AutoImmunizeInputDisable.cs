using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Hooking;
using OmenTools.Interop.Game.Models;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules;

public class AutoImmunizeInputDisable : ModuleBase
{
	private delegate bool iYlE4NpTA11brmuNhOy(nint conditionAddress, ConditionFlag flag, int a3, int a4);

	private static readonly CompSig IsConditionAbleToSetSig;

	private Hook<iYlE4NpTA11brmuNhOy>? IsConditionAbleToSetHook;

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected override void Init()
	{
		if (IsConditionAbleToSetHook == null)
		{
			IsConditionAbleToSetHook = IsConditionAbleToSetSig.GetHook<iYlE4NpTA11brmuNhOy>(SyKMUFJNJm);
		}
		IsConditionAbleToSetHook.Enable();
	}

	private static bool SyKMUFJNJm(nint conditionaddress, ConditionFlag flag, int a3, int a4)
	{
		return true;
	}

	public AutoImmunizeInputDisable()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x32D9CC3 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_86d75c161ced498ab6a767d2eb82d38e)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5351A06E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_9085260e6b1a40438d219e85182b5d08)),
			Category = ModuleCategory.System
		};
		Permission = new ModulePermission
		{
			NeedAuth = true,
			CNPremium = true,
			TCPremium = true
		};
		base._002Ector();
	}

	static AutoImmunizeInputDisable()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		IsConditionAbleToSetSig = new CompSig(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2CE51C89 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c9dc773883f149b7befba3bdbeac55f7));
	}
}
