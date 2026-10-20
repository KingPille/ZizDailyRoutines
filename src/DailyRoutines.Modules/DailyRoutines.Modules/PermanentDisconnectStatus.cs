using System.Timers;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using OmenTools.Extensions;
using OmenTools.Info.Game.Packets.Upstream;
using OmenTools.Interop.Game;
using OmenTools.Threading;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules;

public class PermanentDisconnectStatus : ModuleBase
{
	private class Xx5JJ93xD1eZfvVIKbC : ModuleConfig
	{
		public bool IsDisbaleInDuty;

		public Xx5JJ93xD1eZfvVIKbC()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			IsDisbaleInDuty = true;
			base._002Ector();
		}
	}

	private MemoryPatch patch;

	private Xx5JJ93xD1eZfvVIKbC config;

	private Timer? timer;

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected override void Init()
	{
		config = ModuleConfigExtension.Load<Xx5JJ93xD1eZfvVIKbC>(this) ?? new Xx5JJ93xD1eZfvVIKbC();
		lPXslTnPga();
		Throttler.Shared.Throttle(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x12D63388 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7fcf5a71a7ed445197f132979519c560), 140000u, reThrottle: true);
		patch = new MemoryPatch(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x43D75B7C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_974d377e1ed74986b81c34c95c2cc846), new global::_003C_003Ez__ReadOnlyArray<byte?>(new byte?[6]
		{
			(byte)72,
			(byte)131,
			(byte)248,
			(byte)byte.MaxValue,
			(byte)144,
			(byte)144
		}));
		patch.Enable();
		if (timer == null)
		{
			timer = new Timer
			{
				AutoReset = true,
				Interval = 10000.0,
				Enabled = true
			};
		}
		timer.Elapsed += ksSs8A0KQ4;
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += VJasvKtibB;
	}

	protected override void Uninit()
	{
		if (timer != null)
		{
			timer.Elapsed -= ksSs8A0KQ4;
			timer.Dispose();
			timer = null;
		}
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= VJasvKtibB;
		lPXslTnPga();
	}

	protected override void ConfigUI()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x109C13C4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_44b9808560124617907f0048883b8948))), ref config.IsDisbaleInDuty))
		{
			config.Save(this);
			lPXslTnPga();
		}
	}

	private unsafe void ksSs8A0KQ4(object? sender, ElapsedEventArgs e)
	{
		if (config.IsDisbaleInDuty && ((GameMain)GameMain.Instance()).CurrentContentFinderConditionId != 0)
		{
			lPXslTnPga();
		}
		else if (Throttler.Shared.Throttle(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x12D63388 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7fcf5a71a7ed445197f132979519c560), 140000u))
		{
			lPXslTnPga();
		}
	}

	private static void VJasvKtibB(uint u)
	{
		lPXslTnPga();
	}

	private static void lPXslTnPga()
	{
		new HeartbeatPacket().Send();
	}

	public PermanentDisconnectStatus()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xBA48B9C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2edd08450a8b42e6997855e5217d6d55)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6AAE43FE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_839f542231154d29b0562b687afa328a)),
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
}
