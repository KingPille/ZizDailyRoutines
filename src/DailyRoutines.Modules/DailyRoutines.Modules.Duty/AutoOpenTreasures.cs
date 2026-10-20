using System;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using DailyRoutines.Verification;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Hooking;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Network;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Info.Game.Packets.Upstream;
using OmenTools.Info.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.Threading.TaskHelper;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules.Duty;

public class AutoOpenTreasures : ModuleBase
{
	private unsafe delegate void NggdLPiitnWWGnYGoMu(uint entityID, SpawnTreasurePacket* packet);

	private class zLvJX6iP7w7BukR3dls : ModuleConfig
	{
		public bool ValidWhenSolo;

		public zLvJX6iP7w7BukR3dls()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			ValidWhenSolo = true;
			base._002Ector();
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass13_0
	{
		public AutoOpenTreasures _003C_003E4__this;

		public uint entityID;

		public _003C_003Ec__DisplayClass13_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal void beuihNSxTm()
		{
			_003C_003E4__this.uaMvYQmnoO(entityID);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass15_0
	{
		public AutoOpenTreasures _003C_003E4__this;

		public uint entityID;

		public _003C_003Ec__DisplayClass15_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal void jD9ioAHPFL()
		{
			_003C_003E4__this.uaMvYQmnoO(entityID);
		}

		internal void HAoigDufSh()
		{
			_003C_003E4__this.uaMvYQmnoO(entityID);
		}
	}

	private static readonly CompSig HandleSpawnTreasurePacketSig;

	private Hook<NggdLPiitnWWGnYGoMu>? HandleSpawnTreasurePacketHook;

	private zLvJX6iP7w7BukR3dls config;

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected unsafe override void Init()
	{
		base.TaskHelper = new TaskHelper
		{
			TimeoutMS = 20000
		};
		config = ModuleConfigExtension.Load<zLvJX6iP7w7BukR3dls>(this) ?? new zLvJX6iP7w7BukR3dls();
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += lhovXs5XDp;
		HandleSpawnTreasurePacketHook = HandleSpawnTreasurePacketSig.GetHook<NggdLPiitnWWGnYGoMu>(rJ3vuaev6y);
		HandleSpawnTreasurePacketHook.Enable();
	}

	protected override void Uninit()
	{
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= lhovXs5XDp;
	}

	protected override void ConfigUI()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		DisabledDisposable val = ImRaii.Disabled(DVyvzGwVvh());
		try
		{
			if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x32DC5D5 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_86d75c161ced498ab6a767d2eb82d38e))), ref config.ValidWhenSolo))
			{
				config.Save(this);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private unsafe void rJ3vuaev6y(uint entityID, SpawnTreasurePacket* packet)
	{
		_003C_003Ec__DisplayClass13_0 CS_0024_003C_003E8__locals5 = new _003C_003Ec__DisplayClass13_0();
		CS_0024_003C_003E8__locals5._003C_003E4__this = this;
		CS_0024_003C_003E8__locals5.entityID = entityID;
		HandleSpawnTreasurePacketHook.Original(CS_0024_003C_003E8__locals5.entityID, packet);
		base.TaskHelper.Enqueue(delegate
		{
			CS_0024_003C_003E8__locals5._003C_003E4__this.uaMvYQmnoO(CS_0024_003C_003E8__locals5.entityID);
		});
	}

	private void lhovXs5XDp(uint zone)
	{
		base.TaskHelper.Abort();
	}

	private void uaMvYQmnoO(uint entityID)
	{
		_003C_003Ec__DisplayClass15_0 CS_0024_003C_003E8__locals7 = new _003C_003Ec__DisplayClass15_0();
		CS_0024_003C_003E8__locals7._003C_003E4__this = this;
		CS_0024_003C_003E8__locals7.entityID = entityID;
		if (GameState.IsInPVPArea || GameState.ContentFinderCondition == 0 || Sheets.SpeedDetectionZones.ContainsKey(GameState.TerritoryType))
		{
			return;
		}
		IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
		if (localPlayer == null || ((DVyvzGwVvh() || config.ValidWhenSolo) && (ManagerBase<PlayersManager>.Instance().PlayersAroundCount != 0 || DalamudServiceExtension.Instance<IPartyList>().Length > 1)))
		{
			return;
		}
		if (IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()) || IConditionExtension.get_IsCasting(DalamudServiceExtension.Instance<ICondition>()) || DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)26])
		{
			base.TaskHelper.Enqueue(delegate
			{
				CS_0024_003C_003E8__locals7._003C_003E4__this.uaMvYQmnoO(CS_0024_003C_003E8__locals7.entityID);
			});
			return;
		}
		IGameObject gameObject = IOmenDalamudService<IObjectTable>.Instance().SearchByEntityID(CS_0024_003C_003E8__locals7.entityID, IObjectTable.EventRange);
		if (gameObject == null)
		{
			base.TaskHelper.Enqueue(delegate
			{
				CS_0024_003C_003E8__locals7._003C_003E4__this.uaMvYQmnoO(CS_0024_003C_003E8__locals7.entityID);
			});
		}
		else
		{
			PositionUpdateInstancePacket.Send(localPlayer.Rotation, gameObject.Position);
			new TreasureOpenPacket(gameObject.EntityID).Send();
			PositionUpdateInstancePacket.Send(localPlayer.Rotation, localPlayer.Position);
		}
	}

	private static bool DVyvzGwVvh()
	{
		if (GameState.IsCN || GameState.IsTC)
		{
			return !AuthState.IsPremium;
		}
		return false;
	}

	public AutoOpenTreasures()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xBA45D70 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2edd08450a8b42e6997855e5217d6d55)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xE0962BB ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_aba6092d7cea44a49d1667540500e363)),
			Category = ModuleCategory.Duty
		};
		Permission = new ModulePermission
		{
			NeedAuth = true
		};
		base._002Ector();
	}

	static AutoOpenTreasures()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		HandleSpawnTreasurePacketSig = new CompSig(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3695AB9D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_26e231ba68f74e8784503e099ddfc2d9));
	}
}
