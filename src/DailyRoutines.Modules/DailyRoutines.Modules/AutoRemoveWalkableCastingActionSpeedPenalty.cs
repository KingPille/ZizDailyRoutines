using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Hooking;
using OmenTools.Interop.Game.Models;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules;

public class AutoRemoveWalkableCastingActionSpeedPenalty : ModuleBase
{
	private delegate bool empofHlugFF19CMNuJI(uint actionID);

	private static readonly CompSig IsMovingSlowlyWhileCastingSig;

	private Hook<empofHlugFF19CMNuJI>? IsMovingSlowlyWhileCastingHook;

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected override void Init()
	{
		if (IsMovingSlowlyWhileCastingHook == null)
		{
			IsMovingSlowlyWhileCastingHook = IsMovingSlowlyWhileCastingSig.GetHook<empofHlugFF19CMNuJI>(SuNTXVraql);
		}
		IsMovingSlowlyWhileCastingHook.Enable();
	}

	private static bool SuNTXVraql(uint actionID)
	{
		return false;
	}

	public AutoRemoveWalkableCastingActionSpeedPenalty()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489BBCFC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cade14e68bb84906959f3cda5c496848)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3AC9442A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3932528062874ea4bb7e2e1185394e37)),
			Category = ModuleCategory.Action
		};
		Permission = new ModulePermission
		{
			NeedAuth = true,
			CNPremium = true,
			TCPremium = true
		};
		base._002Ector();
	}

	static AutoRemoveWalkableCastingActionSpeedPenalty()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		IsMovingSlowlyWhileCastingSig = new CompSig(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x395111B9 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3aeae81f85f84848b273be7b2bfbf497));
	}
}
