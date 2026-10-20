using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Command;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using OmenTools.Extensions;
using OmenTools.Info.Game.Enums;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules.Duty;

public class InstantLeaveDuty : ModuleBase
{
	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static LogMessageManager.PreLogMessageDelegate _003C0_003E__OnPreReceiveLogmessage;

		public static ExecuteCommandManager.PreExecuteCommandDelegate _003C1_003E__OnPreExecuteCommand;

		public static HandlerDelegate _003C2_003E__OnCommand;
	}

	private const string COMMAND = "leaveduty";

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected override void Init()
	{
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Expected O, but got Unknown
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Expected O, but got Unknown
		OmenServiceBase<LogMessageManager>.Instance().RegPre(d3rl6PP6Sp);
		OmenServiceBase<ExecuteCommandManager>.Instance().RegPre(GCclGBJFWh);
		CommandManager commandManager = OmenServiceBase<CommandManager>.Instance();
		string subCommand = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6FF1509C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5859f4ea3042459db2c9b610b3099912);
		object obj = _003C_003EO._003C2_003E__OnCommand;
		if (obj == null)
		{
			HandlerDelegate val = KX3lkmK9ZH;
			_003C_003EO._003C2_003E__OnCommand = val;
			obj = (object)val;
		}
		commandManager.AddSubCommand(subCommand, new CommandInfo((HandlerDelegate)obj)
		{
			HelpMessage = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6AAE9A40 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_839f542231154d29b0562b687afa328a))
		});
	}

	protected override void Uninit()
	{
		OmenServiceBase<CommandManager>.Instance().RemoveSubCommand(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4B0F7802 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_8e002b0644ef421698ef6f63772929af));
		OmenServiceBase<ExecuteCommandManager>.Instance().Unreg(GCclGBJFWh);
		OmenServiceBase<LogMessageManager>.Instance().Unreg(d3rl6PP6Sp);
	}

	protected override void ConfigUI()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(0, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6FF1E504 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5859f4ea3042459db2c9b610b3099912)));
		ImGui.TextColored(ref vector, val);
		IndentDisposable val2 = ImRaii.PushIndent(1, true);
		try
		{
			ImU8String val3 = new ImU8String(8, 2);
			((ImU8String)(ref val3)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D9C9ACF ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3badc14f3c904cf4809aa58db382c8ee));
			((ImU8String)(ref val3)).AppendFormatted<string>(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5017E9D6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e227921f7ee94b6aa1c28169395ba0cf));
			((ImU8String)(ref val3)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x9B4968C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2f1c0cb46b4440d499a69ed8afe22360));
			((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3987210B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_56ae6e3f2cdf4dd8a743f4d493ce49a5)));
			ImGui.TextUnformatted(val3);
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	private static void GCclGBJFWh(ref bool isPrevented, ref ExecuteCommandFlag command, ref uint param1, ref uint param2, ref uint param3, ref uint param4)
	{
		if (command == ExecuteCommandFlag.LeaveDuty && DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)26])
		{
			param1 = 1u;
		}
	}

	private static void d3rl6PP6Sp(ref bool isPrevented, ref uint logMessageID, ref LogMessageQueueItem values)
	{
		if (logMessageID == 914)
		{
			isPrevented = true;
		}
	}

	private static void KX3lkmK9ZH(string command, string args)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.LeaveDuty, DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)26] ? 1u : 0u);
	}

	public InstantLeaveDuty()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6AAE9A04 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_839f542231154d29b0562b687afa328a)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x27D104D9 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_39d5d8025f5249e6b489bc374eefbb3c)),
			Category = ModuleCategory.Duty
		};
		Permission = new ModulePermission
		{
			NeedAuth = true
		};
		base._002Ector();
	}
}
