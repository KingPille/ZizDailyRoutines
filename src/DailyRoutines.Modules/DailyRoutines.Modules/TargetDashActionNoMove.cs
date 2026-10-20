using System.Numerics;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using Lumina.Excel.Sheets;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules;

public class TargetDashActionNoMove : ModuleBase
{
	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected override void Init()
	{
		OmenServiceBase<UseActionManager>.Instance().RegPreCharacterCompleteCast(ojS1nQyhdV);
	}

	protected override void Uninit()
	{
		OmenServiceBase<UseActionManager>.Instance().Unreg(ojS1nQyhdV);
	}

	private static void ojS1nQyhdV(ref bool isPrevented, ref IBattleChara player, ref ActionType type, ref uint actionID, ref uint spellID, ref GameObjectId animationTargetID, ref Vector3 location, ref float rotation, ref short lastUsedActionSequence, ref int animationVariation, ref int ballistaEntityID)
	{
		IPlayerCharacter playerCharacter = LocalPlayerState.Object;
		if (playerCharacter != null && player.Address == playerCharacter.Address && (int)type == 1 && LuminaGetter.TryGetRow<Action>(spellID, out Action item) && ((Action)(ref item)).AffectsPosition && ((Action)(ref item)).CanTargetHostile)
		{
			actionID = (spellID = 7u);
		}
	}

	public TargetDashActionNoMove()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x469A2429 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_bb1eb496bad948e4a2baaa3e1edc7a92)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3F36BFAE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7a7485368e28474eb6d61987c51729a2)),
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
}
