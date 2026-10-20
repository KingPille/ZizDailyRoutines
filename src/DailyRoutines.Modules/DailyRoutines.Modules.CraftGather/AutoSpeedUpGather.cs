using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;
using OmenTools.Extensions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules.CraftGather;

public class AutoSpeedUpGather : ModuleBase
{
	private class QgpmPAiRes8WJ414gDy : ModuleConfig
	{
		public bool DisableWhenPlayersAround;

		public QgpmPAiRes8WJ414gDy()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			DisableWhenPlayersAround = true;
			base._002Ector();
		}
	}

	private QgpmPAiRes8WJ414gDy config;

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected override void Init()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<QgpmPAiRes8WJ414gDy>(this) ?? new QgpmPAiRes8WJ414gDy();
		DalamudServiceExtension.Instance<ICondition>().ConditionChange += new ConditionChangeDelegate(t6elaliPsC);
		if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)6])
		{
			t6elaliPsC((ConditionFlag)6, value: true);
		}
	}

	protected override void Uninit()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		DalamudServiceExtension.Instance<ICondition>().ConditionChange -= new ConditionChangeDelegate(t6elaliPsC);
		t6elaliPsC((ConditionFlag)6, value: true);
	}

	protected override void ConfigUI()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x43D75CDA ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_974d377e1ed74986b81c34c95c2cc846))), ref config.DisableWhenPlayersAround))
		{
			config.Save(this);
		}
	}

	private void t6elaliPsC(ConditionFlag flag, bool value)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Invalid comparison between Unknown and I4
		if ((int)flag == 6)
		{
			OmenServiceBase<GamePacketManager>.Instance().Unreg(Ao5l0G6LHQ);
			if (value)
			{
				OmenServiceBase<GamePacketManager>.Instance().RegPreSendPacket(Ao5l0G6LHQ);
			}
		}
	}

	private void Ao5l0G6LHQ(ref bool isPrevented, int opcode, ref nint packet, ref bool isPrioritize)
	{
		if (!DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)6])
		{
			OmenServiceBase<GamePacketManager>.Instance().Unreg(Ao5l0G6LHQ);
		}
		else if (!isPrioritize && (!config.DisableWhenPlayersAround || ManagerBase<PlayersManager>.Instance().PlayersAroundCount <= 0))
		{
			isPrioritize = true;
		}
	}

	public AutoSpeedUpGather()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x45E4D2BD ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1904f95d2a394c929e1a6d4f72943ad7)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6AAEC4FA ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_839f542231154d29b0562b687afa328a)),
			Category = ModuleCategory.CraftGather
		};
		Permission = new ModulePermission
		{
			NeedAuth = true
		};
		base._002Ector();
	}
}
