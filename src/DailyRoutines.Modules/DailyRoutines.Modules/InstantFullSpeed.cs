using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using OmenTools.Dalamud;
using OmenTools.Interop.Game.Models;
using OmenTools.Interop.Game.Models.Native;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules;

public class InstantFullSpeed : ModuleBase
{
	private unsafe delegate void WvcZNi3Ts5nhuwFYW18(PlayerMoveControllerWalk* controller);

	private delegate float IkMrTJ3175ekWfYB9I2(nint characterSpeedContainer);

	private static readonly CompSig UpdatePlayerWalkSpeedSig;

	private Hook<WvcZNi3Ts5nhuwFYW18> UpdatePlayerWalkSpeedHook;

	private static readonly CompSig SpeedMultiplierCalculateSig;

	private IkMrTJ3175ekWfYB9I2? SpeedMultiplierCalculate;

	private static readonly CompSig CharacterSpeedContainerOffsetSig;

	private nint characterSpeedContainerOffset;

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected unsafe override void Init()
	{
		if (characterSpeedContainerOffset == IntPtr.Zero)
		{
			characterSpeedContainerOffset = Marshal.ReadInt32(CharacterSpeedContainerOffsetSig.ScanText() + 3);
		}
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(16, 2);
		defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4CF208A2 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a07aac30eb32484e94cb4d1e62117765));
		defaultInterpolatedStringHandler.AppendFormatted(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x68AC3354 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4722689f1a4d41dc9dc800bfe1a89c09));
		defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--1470179789 ^ 0x3D0F52FF ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_839f542231154d29b0562b687afa328a));
		defaultInterpolatedStringHandler.AppendFormatted(characterSpeedContainerOffset);
		DLog.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
		SpeedMultiplierCalculate = SpeedMultiplierCalculateSig.GetDelegate<IkMrTJ3175ekWfYB9I2>();
		if (UpdatePlayerWalkSpeedHook == null)
		{
			UpdatePlayerWalkSpeedHook = UpdatePlayerWalkSpeedSig.GetHook<WvcZNi3Ts5nhuwFYW18>(JmKsHaKlgn);
		}
		UpdatePlayerWalkSpeedHook.Enable();
	}

	private unsafe void JmKsHaKlgn(PlayerMoveControllerWalk* controller)
	{
		float num = SpeedMultiplierCalculate((nint)((byte*)Control.GetLocalPlayer() + characterSpeedContainerOffset));
		controller->CurrentSpeed = num * controller->BaseMovementSpeed;
		UpdatePlayerWalkSpeedHook.Original(controller);
		controller->CurrentSpeed = num * controller->BaseMovementSpeed;
	}

	public InstantFullSpeed()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x18CCFB94 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_33babcee9ecc460a9e530039fd18d361)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489FA894 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b2294f2f9bb24a76b00cf1af75888414)),
			Category = ModuleCategory.System
		};
		Permission = new ModulePermission
		{
			NeedAuth = true
		};
		base._002Ector();
	}

	static InstantFullSpeed()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		UpdatePlayerWalkSpeedSig = new CompSig(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4753DB28 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_790eb00575b3425495c924ac13469f4c));
		SpeedMultiplierCalculateSig = new CompSig(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7FD8C499 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1eab987c1db64b928220e27bd03141d3));
		CharacterSpeedContainerOffsetSig = new CompSig(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x18CCC432 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_33babcee9ecc460a9e530039fd18d361));
	}
}
