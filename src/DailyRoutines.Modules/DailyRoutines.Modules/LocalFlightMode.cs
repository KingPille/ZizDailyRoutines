using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using OmenTools.Interop.Game.Models;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules;

public class LocalFlightMode : ModuleBase
{
	private delegate FlightAllowedStatus isSivE3veVal8aNRPr1();

	private static readonly CompSig GetFlightAllowedStatusSig;

	private Hook<isSivE3veVal8aNRPr1>? GetFlightAllowedStatusHook;

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected override void Init()
	{
		if (GetFlightAllowedStatusHook == null)
		{
			GetFlightAllowedStatusHook = GetFlightAllowedStatusSig.GetHook<isSivE3veVal8aNRPr1>(cPesMZivSA);
		}
		GetFlightAllowedStatusHook.Enable();
	}

	private static FlightAllowedStatus cPesMZivSA()
	{
		return (FlightAllowedStatus)0;
	}

	public LocalFlightMode()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7C2F1D32 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_12c2d68ba4654622a4959ee73cacc6d7)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x469A4DA1 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_bb1eb496bad948e4a2baaa3e1edc7a92)),
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

	static LocalFlightMode()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		GetFlightAllowedStatusSig = new CompSig(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7FD8E3B5 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1eab987c1db64b928220e27bd03141d3));
	}
}
