using System;
using System.Collections.Frozen;
using System.Numerics;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using OmenTools.Extensions;
using OmenTools.Info.Game.Packets.Upstream;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules;

public class AutoGuaranteeCarryingObject : ModuleBase
{
	private static readonly FrozenSet<ObjectKind> ValidObjectKinds;

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected override void Init()
	{
		OmenServiceBase<GamePacketManager>.Instance().RegPreSendPacket(iW31hjZdN8);
	}

	protected override void Uninit()
	{
		OmenServiceBase<GamePacketManager>.Instance().Unreg(iW31hjZdN8);
	}

	private unsafe static void iW31hjZdN8(ref bool isPrevented, int opcode, ref nint packet, ref bool isPrioritize)
	{
		if (opcode == UpstreamOpcode.PositionUpdateOpcode && DalamudServiceExtension.Instance<ICondition>().Any((ConditionFlag[])(object)new ConditionFlag[2]
		{
			(ConditionFlag)65,
			(ConditionFlag)9
		}) && !((TerritoryInfo)TerritoryInfo.Instance()).InSanctuary && !rYp1onhf8g())
		{
			PositionUpdatePacket* ptr = (PositionUpdatePacket*)packet;
			ptr->Position -= new Vector3(0f, -20f, 0f);
		}
	}

	private unsafe static bool rYp1onhf8g()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		GameObject* target = ((TargetSystem)TargetSystem.Instance()).Target;
		if (target != null && ValidObjectKinds.Contains(((GameObject)target).ObjectKind))
		{
			return ((GameObject)target).NamePlateIconId != 0;
		}
		return false;
	}

	public AutoGuaranteeCarryingObject()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x66B3C5D4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_762f55d0e7ef4ecd8f3f55c0f8a10fae)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D752EC8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8b51f8019ce49018b38e06104a97fc1)),
			Category = ModuleCategory.General
		};
		Permission = new ModulePermission
		{
			NeedAuth = true
		};
		base._002Ector();
	}

	unsafe static AutoGuaranteeCarryingObject()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		ValidObjectKinds = FrozenSet.Create(new ReadOnlySpan<ObjectKind>(Unsafe.AsPointer(ref global::_003CPrivateImplementationDetails_003E._48986D85F8C0A0601EF7C44AFC939DD0A8B0ED7BC309C7E373CB0FE94E30C7AA), 2));
	}
}
