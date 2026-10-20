using System;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Interop.Game;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules;

public class AutoKeepTargetBeyondRange : ModuleBase
{
	private MemoryPatch hardTargetRangePatch;

	private MemoryPatch targetSelectionRangePatch;

	private nint protectedTargetAddress;

	private long protectedTargetExpiration;

	private const int FOCUS_TARGET_CLEAR_SUPPRESSION_MS = 500;

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected override void Init()
	{
		hardTargetRangePatch = new MemoryPatch(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x43D70624 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_974d377e1ed74986b81c34c95c2cc846), new global::_003C_003Ez__ReadOnlyArray<byte?>(new byte?[2]
		{
			(byte)144,
			(byte)144
		}), 74);
		targetSelectionRangePatch = new MemoryPatch(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3D56BDA6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e9e296e241f44828a57c750102ce71c1), new global::_003C_003Ez__ReadOnlyArray<byte?>(new byte?[8]
		{
			(byte)15,
			(byte)87,
			(byte)210,
			(byte)144,
			(byte)144,
			(byte)144,
			(byte)144,
			(byte)144
		}));
		hardTargetRangePatch.Enable();
		targetSelectionRangePatch.Enable();
		OmenServiceBase<TargetManager>.Instance().RegPreSetHardTarget(GwPMuPMvGQ);
		OmenServiceBase<TargetManager>.Instance().RegPostSetHardTarget(Rj7MXak0rI);
	}

	protected override void Uninit()
	{
		OmenServiceBase<TargetManager>.Instance().Unreg(GwPMuPMvGQ);
		OmenServiceBase<TargetManager>.Instance().Unreg(Rj7MXak0rI);
	}

	private void GwPMuPMvGQ(ref bool isPrevented, ref IGameObject? target, ref bool ignoreTargetModes, ref bool a4, ref int a5)
	{
		if (target == null)
		{
			if (protectedTargetAddress == IntPtr.Zero || Environment.TickCount64 > protectedTargetExpiration || TargetManager.Target?.Address != protectedTargetAddress || TargetManager.FocusTarget?.Address != protectedTargetAddress)
			{
				protectedTargetAddress = IntPtr.Zero;
				protectedTargetExpiration = 0L;
			}
			else
			{
				isPrevented = true;
				protectedTargetAddress = IntPtr.Zero;
				protectedTargetExpiration = 0L;
			}
		}
	}

	private void Rj7MXak0rI(bool result, IGameObject? target, bool ignoreTargetModes, bool a4, int a5)
	{
		protectedTargetAddress = IntPtr.Zero;
		protectedTargetExpiration = 0L;
		if (!result || target == null)
		{
			return;
		}
		IPlayerCharacter playerCharacter = LocalPlayerState.Object;
		if (playerCharacter != null && TargetManager.FocusTarget?.Address == target.Address)
		{
			float num = target.Position.X - playerCharacter.Position.X;
			float num2 = target.Position.Z - playerCharacter.Position.Z;
			if (!(num * num + num2 * num2 <= 2500f))
			{
				protectedTargetAddress = target.Address;
				protectedTargetExpiration = Environment.TickCount64 + 500;
			}
		}
	}

	public AutoKeepTargetBeyondRange()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4CF21C44 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a07aac30eb32484e94cb4d1e62117765)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489B87FE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cade14e68bb84906959f3cda5c496848)),
			Category = ModuleCategory.System
		};
		Permission = new ModulePermission
		{
			NeedAuth = true
		};
		base._002Ector();
	}
}
