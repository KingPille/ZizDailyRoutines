using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Hooking;
using OmenTools.Interop.Game.Models;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules;

public class ProhibitSwimming : ModuleBase
{
	private delegate byte fA4llY33YIFSQ4KIqTO(nint a1, bool a2);

	private static readonly CompSig EnterSwimZoneSig;

	private Hook<fA4llY33YIFSQ4KIqTO>? EnterSwimZoneHook;

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected override void Init()
	{
		if (EnterSwimZoneHook == null)
		{
			EnterSwimZoneHook = EnterSwimZoneSig.GetHook<fA4llY33YIFSQ4KIqTO>(Sk6s3gtmrG);
		}
		EnterSwimZoneHook.Enable();
	}

	private static byte Sk6s3gtmrG(nint a1, bool a2)
	{
		return 0;
	}

	public ProhibitSwimming()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5E3BBABC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2d7d8a5df6744c51b35f486f03b53ded)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5351F330 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_9085260e6b1a40438d219e85182b5d08)),
			Category = ModuleCategory.System
		};
		Permission = new ModulePermission
		{
			NeedAuth = true
		};
		base._002Ector();
	}

	static ProhibitSwimming()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		EnterSwimZoneSig = new CompSig(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5357B09A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_0676edd1b45a4ff096830fd6229bc1fb));
	}
}
