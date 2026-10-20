using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.Internal;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;
using itAnDoW4mgYVlrmtMor;

namespace l5yB83SNcfeBgR8dU1a;

internal static class L4hg3GSeYIg8LCt4TTD
{
	private unsafe delegate void aPbiieOkpU2MngS61Ki(InputTimerModule* module, void* a2, void* a3);

	private static readonly CompSig InputTimerModuleUpdateSig;

	private static Hook<aPbiieOkpU2MngS61Ki>? InputTimerModuleUpdateHook;

	public unsafe static void SrrS7I04uM()
	{
		if (InputTimerModuleUpdateHook == null)
		{
			InputTimerModuleUpdateHook = InputTimerModuleUpdateSig.GetHook<aPbiieOkpU2MngS61Ki>(sTmSIXq85w);
		}
		InputTimerModuleUpdateHook.Enable();
	}

	public static void vUASdW1BSU()
	{
		InputTimerModuleUpdateHook?.Dispose();
		InputTimerModuleUpdateHook = null;
	}

	private unsafe static void sTmSIXq85w(InputTimerModule* module, void* a2, void* a3)
	{
		if (!PluginConfig.Instance().PreventAFK)
		{
			InputTimerModuleUpdateHook.Original(module, a2, a3);
			return;
		}
		((AgentLobby)AgentLobby.Instance()).IdleTime = 0L;
		((InputTimerModule)module).AfkTimer = (((InputTimerModule)module).ContentInputTimer = (((InputTimerModule)module).InputTimer = 0f));
		if (OmenServiceBase<LocalPlayerState>.Instance().IsMoving && Throttler.Shared.Throttle(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6B318030 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ac6250342b784829976646d28955b117), 1000u) && ((InfoModule)InfoModule.Instance()).IsOnlineStatusSet((byte)17))
		{
			((InfoProxyDetail)InfoProxyDetail.Instance()).SendOnlineStatusUpdate(1u);
			((RaptureLogModule)RaptureLogModule.Instance()).ShowLogMessage(98u);
			((AgentDetail)AgentDetail.Instance()).ShouldUpdate = true;
		}
		InputTimerModuleUpdateHook.Original(module, a2, a3);
	}

	static L4hg3GSeYIg8LCt4TTD()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		InputTimerModuleUpdateSig = new CompSig(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1FFA4F0F ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_a46716ea783442acb36e95ced1c935fc));
	}
}
