using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Hooking;
using OmenTools.Interop.Game.Models;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules;

public class AutoImmunizeJumpRestriction : ModuleBase
{
	private unsafe delegate void* Whbxs8xKI776WqwRryo(byte flag, byte isSet);

	private static readonly CompSig SetNoJumpStateSig;

	private Hook<Whbxs8xKI776WqwRryo>? SetNoJumpStateHook;

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected unsafe override void Init()
	{
		if (SetNoJumpStateHook == null)
		{
			SetNoJumpStateHook = SetNoJumpStateSig.GetHook<Whbxs8xKI776WqwRryo>(Ukl11wWRFL);
		}
		SetNoJumpStateHook.Enable();
		for (byte b = 0; b < 5; b++)
		{
			Ukl11wWRFL(b, 0);
		}
	}

	private unsafe void* Ukl11wWRFL(byte flag, byte isSet)
	{
		return SetNoJumpStateHook.Original(flag, 0);
	}

	public AutoImmunizeJumpRestriction()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x109C6DFE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_44b9808560124617907f0048883b8948)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489BA0F8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cade14e68bb84906959f3cda5c496848)),
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

	static AutoImmunizeJumpRestriction()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		SetNoJumpStateSig = new CompSig(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5CE32599 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3054c6e6ef664be696bb2ad005864818));
	}
}
