using System;
using System.Drawing;
using System.Numerics;
using System.Threading;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Command;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using OmenTools.Extensions;
using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules;

public class InstantReturn : ModuleBase
{
	private unsafe delegate void El7R2l3MifdjaNQe4mg(AgentInterface* agentReturn);

	private class DJX7in34s5W7xen6EEq : ModuleConfig
	{
		public bool DisableInPVP;

		public bool DisableWhenPlayersAround;

		public bool HookOrigin;

		public bool IgnoreCooldown;

		public DJX7in34s5W7xen6EEq()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			DisableInPVP = true;
			DisableWhenPlayersAround = true;
			HookOrigin = true;
			base._002Ector();
		}
	}

	private const string COMMAND = "instantreturn";

	private static readonly CompSig ReturnSig;

	private Hook<El7R2l3MifdjaNQe4mg>? ReturnHook;

	private MemoryPatch agentReturnShowPatch;

	private DJX7in34s5W7xen6EEq config;

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected unsafe override void Init()
	{
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Expected O, but got Unknown
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Expected O, but got Unknown
		agentReturnShowPatch = new MemoryPatch(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x423ECF4A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_6368ba44fa4749c09da09b5ebf853b71), new global::_003C_003Ez__ReadOnlyArray<byte?>(new byte?[2]
		{
			(byte)144,
			(byte)144
		}));
		config = ModuleConfigExtension.Load<DJX7in34s5W7xen6EEq>(this) ?? new DJX7in34s5W7xen6EEq();
		agentReturnShowPatch.Enable();
		if (ReturnHook == null)
		{
			ReturnHook = ReturnSig.GetHook<El7R2l3MifdjaNQe4mg>(pSHs9dW29b);
		}
		ReturnHook.Enable();
		OmenServiceBase<UseActionManager>.Instance().RegPreUseAction(jegsSxJ4Yl);
		OmenServiceBase<CommandManager>.Instance().AddSubCommand(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x469A4EB5 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_bb1eb496bad948e4a2baaa3e1edc7a92), new CommandInfo(new HandlerDelegate(YuBsVGBAfk))
		{
			HelpMessage = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489BD928 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cade14e68bb84906959f3cda5c496848))
		});
	}

	protected override void Uninit()
	{
		OmenServiceBase<CommandManager>.Instance().RemoveSubCommand(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5A7FB30F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5154ccd9fed0487f8fa0244531466009));
		OmenServiceBase<UseActionManager>.Instance().Unreg(jegsSxJ4Yl);
	}

	private void jegsSxJ4Yl(ref bool isPrevented, ref ActionType actionType, ref uint actionID, ref ulong targetID, ref uint extraParam, ref UseActionMode queueState, ref uint comboRouteID)
	{
		if ((int)actionType == 5 && actionID == 8 && config.IgnoreCooldown)
		{
			ulAsfINCNX();
		}
	}

	protected override void ConfigUI()
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(1, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3D568B6A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e9e296e241f44828a57c750102ce71c1)));
		((ImU8String)(ref val)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4B0FEC18 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_8e002b0644ef421698ef6f63772929af));
		ImGui.TextColored(ref vector, val);
		ImGui.SameLine();
		ImU8String val2 = default(ImU8String);
		((ImU8String)(ref val2))._002Ector(8, 2);
		((ImU8String)(ref val2)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x423EA486 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_6368ba44fa4749c09da09b5ebf853b71));
		((ImU8String)(ref val2)).AppendFormatted<string>(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4C428C40 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4643f8ec0000417ba22db1f06f6757bd));
		((ImU8String)(ref val2)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3987A1C5 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_56ae6e3f2cdf4dd8a743f4d493ce49a5));
		((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6C355658 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88a2dcd1996b4a3fbefaf51a98b59def)));
		ImGui.TextUnformatted(val2);
		ImGui.NewLine();
		ImU8String val3 = default(ImU8String);
		((ImU8String)(ref val3))._002Ector(0, 1);
		((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--367535148 ^ 0x5EE78322 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_8e002b0644ef421698ef6f63772929af)));
		if (ImGui.Checkbox(val3, ref config.HookOrigin))
		{
			config.Save(this);
		}
		ImU8String val4 = default(ImU8String);
		((ImU8String)(ref val4))._002Ector(0, 1);
		((ImU8String)(ref val4)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3F36D474 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7a7485368e28474eb6d61987c51729a2)));
		if (ImGui.Checkbox(val4, ref config.DisableInPVP))
		{
			config.Save(this);
		}
		ImU8String val5 = default(ImU8String);
		((ImU8String)(ref val5))._002Ector(0, 1);
		((ImU8String)(ref val5)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x1778C03F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f7cf93b006904393bb56d82c0a688fa1)));
		if (ImGui.Checkbox(val5, ref config.DisableWhenPlayersAround))
		{
			config.Save(this);
		}
		ImU8String val6 = default(ImU8String);
		((ImU8String)(ref val6))._002Ector(0, 1);
		((ImU8String)(ref val6)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-1675397288 ^ -1842690633 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_aba6092d7cea44a49d1667540500e363)));
		if (ImGui.Checkbox(val6, ref config.IgnoreCooldown))
		{
			config.Save(this);
		}
	}

	private void YuBsVGBAfk(string command, string arguments)
	{
		ulAsfINCNX();
	}

	private unsafe void pSHs9dW29b(AgentInterface* agentReturn)
	{
		if (!yxVsCVR301())
		{
			ReturnHook.Original(agentReturn);
		}
		else
		{
			ulAsfINCNX();
		}
	}

	private unsafe void ulAsfINCNX()
	{
		if (yxVsCVR301())
		{
			if (!tfFsjmK4DB())
			{
				OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.ReturnIfNotLalafell);
			}
			else
			{
				OmenServiceBase<UseActionManager>.Instance().UseActionLocation((ActionType)5, 8u, 3758096384uL, default(Vector3), 0u, 0);
			}
			((ActionManager)ActionManager.Instance()).StartCooldown((ActionType)1, 6u);
			DalamudServiceExtension.Instance<IFramework>().RunOnTick((Action)delegate
			{
				((ActionManager)ActionManager.Instance()).StartCooldown((ActionType)1, 6u);
			}, TimeSpan.FromSeconds(1L), 0, default(CancellationToken));
		}
	}

	private unsafe bool yxVsCVR301()
	{
		if (config.DisableInPVP && GameState.IsInPVPInstance)
		{
			return false;
		}
		if (DalamudServiceExtension.Instance<ICondition>().Any((ConditionFlag[])(object)new ConditionFlag[2]
		{
			(ConditionFlag)59,
			(ConditionFlag)55
		}))
		{
			return false;
		}
		if (!config.IgnoreCooldown && ((ActionManager)ActionManager.Instance()).GetActionStatus((ActionType)1, 6u, 3758096384uL, true, true, (uint*)null) == 582)
		{
			return false;
		}
		if (config.DisableWhenPlayersAround && ManagerBase<PlayersManager>.Instance().PlayersAroundCount > 0)
		{
			return false;
		}
		return true;
	}

	private unsafe static bool tfFsjmK4DB()
	{
		BattleChara* localPlayer = Control.GetLocalPlayer();
		if (localPlayer == null)
		{
			return false;
		}
		return ((CustomizeData)(&((DrawDataContainer)(&((BattleChara)localPlayer).DrawData)).CustomizeData)).Race == 3;
	}

	public InstantReturn()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x1C171F47 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_accf3844f1c6464481df1849f0aee1bb)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x423ECD3E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_6368ba44fa4749c09da09b5ebf853b71)),
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

	static InstantReturn()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		ReturnSig = new CompSig(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x66B386EC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_762f55d0e7ef4ecd8f3f55c0f8a10fae));
	}
}
