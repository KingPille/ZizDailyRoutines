using System;
using System.Collections.Frozen;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Info.Game.Enums;
using OmenTools.Info.Game.Packets.Upstream;
using OmenTools.Interop.Game.Helpers;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules;

public class InstantTeleport : ModuleBase
{
	private class UU69u23saeyCCLPFI7m : ModuleConfig
	{
		public bool OnlyValidWhenSolo;

		public UU69u23saeyCCLPFI7m()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			OnlyValidWhenSolo = true;
			base._002Ector();
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass11_0
	{
		public ExecuteCommandFlag flag;

		public uint p1;

		public uint p2;

		public uint p3;

		public uint p4;

		public IPlayerCharacter localPlayer;

		public _003C_003Ec__DisplayClass11_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal void LD838J43ic()
		{
			OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(flag, p1, p2, p3, p4);
			OmenServiceBase<UseActionManager>.Instance().UseActionLocation((ActionType)5, 7u, localPlayer.EntityID, default(Vector3), 0u, 0);
		}
	}

	private UU69u23saeyCCLPFI7m config;

	private bool isOnTeleporting;

	private static readonly FrozenSet<ExecuteCommandFlag> ValidFlags;

	private static readonly FrozenSet<uint> InvalidLogmessages;

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected override void Init()
	{
		config = ModuleConfigExtension.Load<UU69u23saeyCCLPFI7m>(this) ?? new UU69u23saeyCCLPFI7m();
		OmenServiceBase<ExecuteCommandManager>.Instance().RegPre(frBsKhTBIk);
		OmenServiceBase<ExecuteCommandManager>.Instance().RegPost(SEystVeW9W);
		OmenServiceBase<GamePacketManager>.Instance().RegPreSendPacket(xQMsWv8gvu);
		OmenServiceBase<UseActionManager>.Instance().RegPreUseActionLocation(tPOs1VM6Qk);
		OmenServiceBase<LogMessageManager>.Instance().RegPost(OjQsTSmyRa);
	}

	protected override void Uninit()
	{
		OmenServiceBase<LogMessageManager>.Instance().Unreg(OjQsTSmyRa);
		OmenServiceBase<UseActionManager>.Instance().Unreg(tPOs1VM6Qk);
		OmenServiceBase<ExecuteCommandManager>.Instance().Unreg(frBsKhTBIk);
		OmenServiceBase<ExecuteCommandManager>.Instance().Unreg(SEystVeW9W);
		OmenServiceBase<GamePacketManager>.Instance().Unreg(xQMsWv8gvu);
		isOnTeleporting = false;
	}

	protected override void ConfigUI()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(0, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--664844326 ^ 0x30D87A83 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f7cf93b006904393bb56d82c0a688fa1)));
		if (ImGui.Checkbox(val, ref config.OnlyValidWhenSolo))
		{
			config.Save(this);
		}
	}

	private void frBsKhTBIk(ref bool isPrevented, ref ExecuteCommandFlag command, ref uint param1, ref uint param2, ref uint param3, ref uint param4)
	{
		_003C_003Ec__DisplayClass11_0 CS_0024_003C_003E8__locals16 = new _003C_003Ec__DisplayClass11_0();
		if (!ValidFlags.Contains(command))
		{
			return;
		}
		CS_0024_003C_003E8__locals16.localPlayer = LocalPlayerState.Object;
		if (CS_0024_003C_003E8__locals16.localPlayer == null || (config.OnlyValidWhenSolo && (DalamudServiceExtension.Instance<IPartyList>().Length > 1 || ManagerBase<PlayersManager>.Instance().PlayersAroundCount > 0)))
		{
			return;
		}
		isPrevented = true;
		if (command != ExecuteCommandFlag.CancelTeleport)
		{
			isOnTeleporting = true;
			Vector3 position = CS_0024_003C_003E8__locals16.localPlayer.Position - new Vector3(0f, 100f, 0f);
			if (RaycastHelper.TryGetGroundPosition(CS_0024_003C_003E8__locals16.localPlayer.Position, out var groundPosition))
			{
				position = groundPosition - new Vector3(0f, 100f, 0f);
			}
			new PositionUpdatePacket(CS_0024_003C_003E8__locals16.localPlayer.Rotation, position, OmenServiceBase<MovementManager>.Instance().GetMoveType(PositionUpdatePacket.MoveType.NormalMove0)).Send();
			CS_0024_003C_003E8__locals16.flag = command;
			CS_0024_003C_003E8__locals16.p1 = param1;
			CS_0024_003C_003E8__locals16.p2 = param2;
			CS_0024_003C_003E8__locals16.p3 = param3;
			CS_0024_003C_003E8__locals16.p4 = param4;
			DalamudServiceExtension.Instance<IFramework>().RunOnTick((Action)delegate
			{
				OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(CS_0024_003C_003E8__locals16.flag, CS_0024_003C_003E8__locals16.p1, CS_0024_003C_003E8__locals16.p2, CS_0024_003C_003E8__locals16.p3, CS_0024_003C_003E8__locals16.p4);
				OmenServiceBase<UseActionManager>.Instance().UseActionLocation((ActionType)5, 7u, CS_0024_003C_003E8__locals16.localPlayer.EntityID, default(Vector3), 0u, 0);
			}, (ManagerBase<PlayersManager>.Instance().PlayersAroundCount != 0) ? TimeSpan.FromMilliseconds(750L) : TimeSpan.Zero, 0, default(CancellationToken));
		}
	}

	private void SEystVeW9W(ExecuteCommandFlag command, uint param1, uint param2, uint param3, uint param4)
	{
		if (command == ExecuteCommandFlag.StartTerritoryTransport)
		{
			isOnTeleporting = false;
		}
	}

	private void xQMsWv8gvu(ref bool isPrevented, int opcode, ref nint packet, ref bool isPrioritize)
	{
		if (opcode == UpstreamOpcode.PositionUpdateOpcode && isOnTeleporting)
		{
			isPrevented = true;
		}
	}

	private void OjQsTSmyRa(uint logMessageID, LogMessageQueueItem item)
	{
		if (isOnTeleporting && InvalidLogmessages.Contains(logMessageID))
		{
			isOnTeleporting = false;
		}
	}

	private void tPOs1VM6Qk(ref bool isPrevented, ref ActionType type, ref uint actionID, ref ulong targetID, ref Vector3 location, ref uint extraParam, ref byte a7)
	{
		if ((int)type == 1 && actionID == 5 && isOnTeleporting)
		{
			isPrevented = true;
		}
	}

	public InstantTeleport()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D756C24 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8b51f8019ce49018b38e06104a97fc1)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x51E5E3E4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a90a92322bff42e180982e106200d4a3)),
			Category = ModuleCategory.System
		};
		Permission = new ModulePermission
		{
			NeedAuth = true
		};
		base._002Ector();
	}

	static InstantTeleport()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		ValidFlags = FrozenSet.Create<ExecuteCommandFlag>(ExecuteCommandFlag.Teleport, ExecuteCommandFlag.TeleportToFriendHouse, ExecuteCommandFlag.CancelTeleport);
		InvalidLogmessages = FrozenSet.Create<uint>(4000u, 1661u, 1662u, 1663u, 1664u, 1665u, 1666u, 1667u, 1668u, 1669u, 1670u, 1671u, 1672u);
	}
}
