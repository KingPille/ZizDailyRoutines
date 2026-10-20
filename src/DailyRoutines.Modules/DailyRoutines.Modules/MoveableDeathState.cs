using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using OmenTools.Interop.Game.Models;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules;

public class MoveableDeathState : ModuleBase
{
	private delegate void Il76V6xsCcAU7csjtBn(nint conditionAddress, CharacterModes mode);

	private static readonly CompSig CharacterModeSetSig;

	private Hook<Il76V6xsCcAU7csjtBn>? CharacterModeSetHook;

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected override void Init()
	{
		if (CharacterModeSetHook == null)
		{
			CharacterModeSetHook = CharacterModeSetSig.GetHook<Il76V6xsCcAU7csjtBn>(OO713eJgZO);
		}
		CharacterModeSetHook.Enable();
	}

	private void OO713eJgZO(nint conditionAddress, CharacterModes mode)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Invalid comparison between Unknown and I4
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if ((int)mode != 2)
		{
			CharacterModeSetHook.Original(conditionAddress, mode);
		}
	}

	public MoveableDeathState()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-1487420814 ^ -2138675647 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_39d5d8025f5249e6b489bc374eefbb3c)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7EE799D1 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88432e96079448078bdf00e7b62f8ab6)),
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

	static MoveableDeathState()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		CharacterModeSetSig = new CompSig(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xE09CDB9 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_aba6092d7cea44a49d1667540500e363));
	}
}
