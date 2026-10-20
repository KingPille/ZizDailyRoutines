using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Command;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Info.Game.Enums;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules;

public class InvulnerableMode : ModuleBase
{
	private bool isNowInvulnerable;

	private const string COMMAND = "invulnerable";

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected override void Init()
	{
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Expected O, but got Unknown
		OmenServiceBase<ExecuteCommandManager>.Instance().RegPre(DS91lwCXXl);
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += f331pacRNj;
		OmenServiceBase<UseActionManager>.Instance().RegPreUseAction(ciX1x8JxxU);
		OmenServiceBase<CommandManager>.Instance().AddSubCommand(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7EE79A3D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88432e96079448078bdf00e7b62f8ab6), new CommandInfo(new HandlerDelegate(eVg1vhdy2E))
		{
			HelpMessage = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489FEA64 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b2294f2f9bb24a76b00cf1af75888414))
		});
	}

	protected override void Uninit()
	{
		OmenServiceBase<CommandManager>.Instance().RemoveSubCommand(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xF204B95 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_fea4ad1b13de4c4bbc22f571ceda7372));
		OmenServiceBase<UseActionManager>.Instance().Unreg(ciX1x8JxxU);
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= f331pacRNj;
		OmenServiceBase<ExecuteCommandManager>.Instance().Unreg(DS91lwCXXl);
		isNowInvulnerable = false;
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.StartTerritoryTransport);
	}

	private void eVg1vhdy2E(string command, string args)
	{
		IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
		if (localPlayer != null && !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)45])
		{
			if (isNowInvulnerable)
			{
				OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.StartTerritoryTransport);
				isNowInvulnerable = false;
				OmenServiceBase<NotifyHelper>.Instance().NotificationInfo(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7FD89A25 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1eab987c1db64b928220e27bd03141d3)));
			}
			else
			{
				isNowInvulnerable = true;
				OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommandComplexLocation(ExecuteCommandComplexFlag.DiveEnd, localPlayer.Position);
				OmenServiceBase<NotifyHelper>.Instance().NotificationInfo(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-864210836 ^ -2007356886 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8dfe57222f64fa68d9d16c8b6fe9d0b)));
			}
		}
	}

	private void DS91lwCXXl(ref bool isPrevented, ref ExecuteCommandFlag command, ref uint param1, ref uint param2, ref uint param3, ref uint param4)
	{
		if (command == ExecuteCommandFlag.StartTerritoryTransport && isNowInvulnerable)
		{
			isPrevented = true;
		}
	}

	private void ciX1x8JxxU(ref bool isPrevented, ref ActionType actionType, ref uint actionID, ref ulong targetID, ref uint extraParam, ref UseActionMode queueState, ref uint comboRouteID)
	{
		if (isNowInvulnerable)
		{
			isPrevented = true;
		}
	}

	private void f331pacRNj(uint obj)
	{
		if (isNowInvulnerable)
		{
			OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.StartTerritoryTransport);
			OmenServiceBase<NotifyHelper>.Instance().NotificationInfo(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x66B3F924 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_762f55d0e7ef4ecd8f3f55c0f8a10fae)));
		}
		isNowInvulnerable = false;
	}

	public InvulnerableMode()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--1317373447 ^ 0x58A5E07 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_8e002b0644ef421698ef6f63772929af)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5CE3225F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3054c6e6ef664be696bb2ad005864818)),
			Category = ModuleCategory.Combat
		};
		Permission = new ModulePermission
		{
			NeedAuth = true,
			CNPremium = true,
			GLPremium = false,
			TCPremium = true,
			KRPremium = true
		};
		base._002Ector();
	}
}
