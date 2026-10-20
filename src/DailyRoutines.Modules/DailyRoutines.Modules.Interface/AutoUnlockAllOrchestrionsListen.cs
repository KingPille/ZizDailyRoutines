using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules.Interface;

public class AutoUnlockAllOrchestrionsListen : ModuleBase
{
	private unsafe delegate bool XA8AwOm2un3LDgF3Hdx(PlayerState* playerState, uint rollID);

	private static readonly CompSig IsOrchestrionRollUnlockedSig;

	private Hook<XA8AwOm2un3LDgF3Hdx>? IsOrchestrionRollUnlockedHook;

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected unsafe override void Init()
	{
		if (IsOrchestrionRollUnlockedHook == null)
		{
			IsOrchestrionRollUnlockedHook = IsOrchestrionRollUnlockedSig.GetHook<XA8AwOm2un3LDgF3Hdx>(K1lvZpG9cI);
		}
		IsOrchestrionRollUnlockedHook.Enable();
	}

	private unsafe bool K1lvZpG9cI(PlayerState* playerState, uint rollID)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Invalid comparison between Unknown and I4
		if (((HousingManager)HousingManager.Instance()).IndoorTerritory != null || (int)GameState.TerritoryIntendedUse == 2)
		{
			return true;
		}
		return IsOrchestrionRollUnlockedHook.Original(playerState, rollID);
	}

	public AutoUnlockAllOrchestrionsListen()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x64E5B23A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f53034f3efc64a309b10c7885bf5d9bd)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5A7F7FEF ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5154ccd9fed0487f8fa0244531466009)),
			Category = ModuleCategory.Interface
		};
		Permission = new ModulePermission
		{
			NeedAuth = true
		};
		base._002Ector();
	}

	static AutoUnlockAllOrchestrionsListen()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		IsOrchestrionRollUnlockedSig = new CompSig(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x9B40410 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2f1c0cb46b4440d499a69ed8afe22360));
	}
}
