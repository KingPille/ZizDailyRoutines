using System;
using System.Numerics;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Hooking;
using OmenTools.Interop.Game.Models;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules;

public class NoClipMode : ModuleBase
{
	private unsafe delegate nint t02c203lTKeBM8jTN2L(nint moveControlInstance, Vector3* expectedPosition, nint controlState, Vector3* currentPosition, nint collisionFlags, ushort movementType);

	private static readonly CompSig CalculateCollisionSig;

	private Hook<t02c203lTKeBM8jTN2L>? CalculateCollisionHook;

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected unsafe override void Init()
	{
		if (CalculateCollisionHook == null)
		{
			CalculateCollisionHook = CalculateCollisionSig.GetHook<t02c203lTKeBM8jTN2L>(ncfs4UyvNr);
		}
		CalculateCollisionHook.Enable();
	}

	private unsafe static nint ncfs4UyvNr(nint moveControlInstance, Vector3* expectedPosition, nint controlState, Vector3* currentPosition, nint collisionFlags, ushort movementType)
	{
		return IntPtr.Zero;
	}

	public NoClipMode()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5351FEBA ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_9085260e6b1a40438d219e85182b5d08)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2DBD55F6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5d8d924f1edf4ec39f023b3c7bb646cc)),
			Category = ModuleCategory.System,
			Author = new string[1] { LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x258FFD0C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_dadeb2ab61424d31be7cc1966e675825) }
		};
		Permission = new ModulePermission
		{
			NeedAuth = true,
			CNPremium = true,
			TCPremium = true
		};
		base._002Ector();
	}

	static NoClipMode()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		CalculateCollisionSig = new CompSig(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5A7FB733 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5154ccd9fed0487f8fa0244531466009));
	}
}
