using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using OmenTools.Extensions;
using OmenTools.Info.Game.Packets.Upstream;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules;

public class InstantSprint : ModuleBase
{
	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected override void Init()
	{
		OmenServiceBase<UseActionManager>.Instance().RegPreUseAction(uJtsOYAYUG);
	}

	protected override void Uninit()
	{
		OmenServiceBase<UseActionManager>.Instance().Unreg(uJtsOYAYUG);
	}

	private unsafe static void uJtsOYAYUG(ref bool isPrevented, ref ActionType actionType, ref uint actionID, ref ulong targetID, ref uint extraParam, ref UseActionMode queueState, ref uint comboRouteID)
	{
		if (((int)actionType != 5 || actionID != 4) && ((int)actionType != 1 || actionID != 3))
		{
			return;
		}
		BattleChara* localPlayer = Control.GetLocalPlayer();
		if (localPlayer != null)
		{
			isPrevented = true;
			uint adjustSprintActionID = ActionManagerExtension.GetAdjustSprintActionID();
			if (adjustSprintActionID == 3)
			{
				new UseActionPacket((ActionType)5, 4u, ((BattleChara)localPlayer).EntityId, ((BattleChara)localPlayer).Rotation).Send();
				((ActionManager)ActionManager.Instance()).StartCooldown((ActionType)1, adjustSprintActionID);
			}
			else
			{
				new UseActionPacket((ActionType)1, adjustSprintActionID, ((BattleChara)localPlayer).EntityId, ((BattleChara)localPlayer).Rotation).Send();
			}
		}
	}

	public InstantSprint()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2194267A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cd1b319f326a44d1aaea645e95d8698b)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7C2F1C7C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_12c2d68ba4654622a4959ee73cacc6d7)),
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
}
