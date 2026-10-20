using System;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;
using OmenTools.Extensions;
using OmenTools.Info.Game.Packets.Upstream;
using OmenTools.Interop.Game.Helpers;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules;

public class NoFallDamage : ModuleBase
{
	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected override void Init()
	{
		OmenServiceBase<GamePacketManager>.Instance().RegPreSendPacket(f4CssIJdyY);
	}

	protected override void Uninit()
	{
		OmenServiceBase<GamePacketManager>.Instance().Unreg(f4CssIJdyY);
	}

	private unsafe static void f4CssIJdyY(ref bool isPrevented, int opcode, ref nint packet, ref bool isPrioritize)
	{
		if (!DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)48])
		{
			return;
		}
		if (opcode == UpstreamOpcode.PositionUpdateOpcode)
		{
			PositionUpdatePacket* ptr = (PositionUpdatePacket*)packet;
			if (RaycastHelper.TryGetGroundPosition(ptr->Position, out var groundPosition) && MathF.Abs(groundPosition.Y - ptr->Position.Y) > 5f)
			{
				ptr->Position = groundPosition;
			}
		}
		else if (opcode == UpstreamOpcode.PositionUpdateInstanceOpcode)
		{
			PositionUpdateInstancePacket* ptr2 = (PositionUpdateInstancePacket*)packet;
			if (RaycastHelper.TryGetGroundPosition(ptr2->Position, out var groundPosition2) && MathF.Abs(groundPosition2.Y - ptr2->Position.Y) > 5f)
			{
				ptr2->Position = groundPosition2;
			}
		}
	}

	public NoFallDamage()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6FF18B42 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5859f4ea3042459db2c9b610b3099912)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4753E12E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_790eb00575b3425495c924ac13469f4c)),
			Category = ModuleCategory.System,
			Author = new string[1] { LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6FF1C70A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5859f4ea3042459db2c9b610b3099912) }
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
