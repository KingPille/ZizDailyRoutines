using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.Manager.Attributes;
using DailyRoutines.Manager;
using DailyRoutines.Verification;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using Dalamud.Utility.Numerics;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.LayoutEngine;
using FFXIVClientStructs.FFXIV.Client.Network;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using Lumina.Excel.Sheets;
using OmenTools.Dalamud;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Info.Game.AetheryteRecord;
using OmenTools.Info.Game.AetheryteRecord.Data;
using OmenTools.Info.Game.Data;
using OmenTools.Info.Game.Enums;
using OmenTools.Info.Game.Packets.Upstream;
using OmenTools.Info.Lumina;
using OmenTools.Info.Lumina.ExtraSheets;
using OmenTools.Interop.Game.AddonEvent;
using OmenTools.Interop.Game.ExecuteCommand.Implementations;
using OmenTools.Interop.Game.Helpers;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.Interop.Game.Models.Native;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;
using itAnDoW4mgYVlrmtMor;

namespace mW9HWxHEH44Yh7Z66g9;

[ManagerInitOrder(4)]
internal sealed class cMXPetHw3Yp4XkysPB7 : ManagerBase<cMXPetHw3Yp4XkysPB7>
{
	private unsafe delegate void DZFxDwjQsR1QhxVefjJ(PublicInstance* instance, uint territoryType, uint instanceCount, uint* instancePlayerCounts, uint instancePlayerCountsLength);

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass23_0
	{
		public cMXPetHw3Yp4XkysPB7 _003C_003E4__this;

		public Vector3? pos;

		public bool cancelAnimation;

		public bool skipDismount;

		public uint zone;

		public _003C_003Ec__DisplayClass23_0()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
		}

		internal bool pY9jJjLvTp()
		{
			return _003C_003E4__this.lihHAtPaRx(pos.Value, cancelAnimation, skipDismount);
		}

		internal bool ub0jr3S8yR()
		{
			return _003C_003E4__this.PcHHcB8b8j(zone, pos, cancelAnimation, skipDismount);
		}

		internal bool PQXjGDpdL2()
		{
			return _003C_003E4__this.PcHHcB8b8j(zone, pos, cancelAnimation, skipDismount);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass25_0
	{
		public AetheryteRecord aetheryte;

		public Vector3 pos;

		public _003C_003Ec__DisplayClass25_0()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
		}

		internal unsafe bool aQpj6Q9JvC()
		{
			return ((Telepo)Telepo.Instance()).Teleport(aetheryte.RowID, aetheryte.SubIndex);
		}

		internal bool TJ9jkqJWLA()
		{
			if (GameState.TerritoryType == aetheryte.ZoneID && LocalPlayerState.Object != null)
			{
				return !UIModuleExtension.IsScreenReady();
			}
			return false;
		}

		internal bool GnAj2UUEOc()
		{
			if (UIModuleExtension.IsScreenReady())
			{
				return true;
			}
			Manager.TPPlayerAddress(pos);
			return false;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass26_0
	{
		public Vector3 pos;

		public IPlayerCharacter localPlayer;

		public _003C_003Ec__DisplayClass26_0()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
		}

		internal bool d0djZ2v8g2()
		{
			if (LocalPlayerState.DistanceTo2DSquared(pos.ToVector2()) <= 9f)
			{
				return true;
			}
			if (!Throttler.Shared.Throttle(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x200F836D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_062a9cfe150f4dc6a9fdd7b6d00434af), 1000u))
			{
				return false;
			}
			if (!UIModuleExtension.IsScreenReady())
			{
				return false;
			}
			new DiveStartPacket(pos + new Vector3(0f, 2f, 0f), localPlayer.Rotation).Send();
			return false;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass29_0
	{
		public AetheryteRecord currentZoneRecord;

		public AetheryteRecord record;

		public EventId eventIDAetheryte;

		public AetheryteRecord targetZoneRecord;

		public cMXPetHw3Yp4XkysPB7 _003C_003E4__this;

		public _003C_003Ec__DisplayClass29_0()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
		}

		internal bool IsljaspY6H(AetheryteRecord x)
		{
			if (x.Group == currentZoneRecord.Group)
			{
				return x.ZoneID == record.ZoneID;
			}
			return false;
		}

		internal unsafe void kXfj06MmQ0()
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			if (!(*Addons.TelepotTown).IsAddonAndNodesReady())
			{
				new EventStartPackt(LocalPlayerState.EntityID, EventId.op_Implicit(eventIDAetheryte)).Send();
			}
		}

		internal bool Qxwj58uc4y()
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			AddonSelectStringEvent.Select(((object)LuminaGetter.GetRowOrDefault<AetheryteTransport>(1u).Text/*cast due to constrained. prefix*/).ToString());
			return targetZoneRecord.UseAethernet();
		}

		internal bool qL7jRB8d21()
		{
			return GameState.TerritoryType == targetZoneRecord.ZoneID;
		}

		internal bool zGSjDYSArv()
		{
			return _003C_003E4__this.lihHAtPaRx(record.Position, cancelAnimation: true, skipDismount: false);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass30_0
	{
		public AetheryteRecord aetheryte;

		public uint waitZone;

		public _003C_003Ec__DisplayClass30_0()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
		}

		internal unsafe bool VpAjeNre9Z()
		{
			return ((Telepo)Telepo.Instance()).Teleport(aetheryte.RowID, aetheryte.SubIndex);
		}

		internal bool nbTjNRYGCE()
		{
			if (GameState.TerritoryType == waitZone && LocalPlayerState.Object != null)
			{
				return UIModuleExtension.IsScreenReady();
			}
			return false;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass31_0
	{
		public cMXPetHw3Yp4XkysPB7 _003C_003E4__this;

		public Vector3 fp;

		public bool cancelAnimation;

		public bool skipDismount;

		public _003C_003Ec__DisplayClass31_0()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
		}

		internal bool Uemj7hEO6V()
		{
			_003C_003E4__this.lihHAtPaRx(fp, cancelAnimation, skipDismount);
			return true;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass31_1
	{
		public bool isFirst;

		public ExitRangeHop _003Ceh_003E5__2;

		public AetheryteHop _003Cah_003E5__3;

		public TelepotTownHop _003Cth_003E5__4;

		public EventHop _003Cenpc_003E5__5;

		public _003C_003Ec__DisplayClass31_0 CS_0024_003C_003E8__locals5;

		public _003C_003Ec__DisplayClass31_1()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
		}

		internal void kEBjd3xKIg()
		{
			CS_0024_003C_003E8__locals5._003C_003E4__this.ONhHZipysP(_003Ceh_003E5__2);
		}

		internal void oE0jILwoes()
		{
			CS_0024_003C_003E8__locals5._003C_003E4__this.J6WHaRgNto(_003Cah_003E5__3, isFirst);
		}

		internal void XKAjUmshmU()
		{
			CS_0024_003C_003E8__locals5._003C_003E4__this.FudH0OOuYr(_003Cth_003E5__4);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass31_2
	{
		public int w;

		public _003C_003Ec__DisplayClass31_1 CS_0024_003C_003E8__locals6;

		public _003C_003Ec__DisplayClass31_2()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
		}

		internal void OuEjug0MkC()
		{
			CS_0024_003C_003E8__locals6.CS_0024_003C_003E8__locals5._003C_003E4__this.L5KH5LZZ4u(CS_0024_003C_003E8__locals6._003Cenpc_003E5__5, w);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass32_0
	{
		public ExitRangeHop hop;

		public Predicate<nint> _003C_003E9__2;

		public _003C_003Ec__DisplayClass32_0()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
		}

		internal unsafe bool wLRjXqC3C3()
		{
			if (!UIModuleExtension.IsScreenReady())
			{
				return false;
			}
			if (!MovementManager.TryGetExitRangeInstance(out var instance, (nint p) => ((ExitRangeLayoutInstance*)p)->TerritoryType == hop.TargetZone))
			{
				return false;
			}
			PopRangeManager.Instance()->PopRange((ILayoutInstance*)instance);
			return true;
		}

		internal unsafe bool XJmjY95hm4(nint p)
		{
			return ((ExitRangeLayoutInstance*)p)->TerritoryType == hop.TargetZone;
		}

		internal bool YPrjz2bncB()
		{
			if (GameState.TerritoryType == hop.TargetZone)
			{
				return LocalPlayerState.Object != null;
			}
			return false;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass33_0
	{
		public AetheryteHop hop;

		public cMXPetHw3Yp4XkysPB7 _003C_003E4__this;

		public _003C_003Ec__DisplayClass33_0()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
		}

		internal unsafe bool BcNOnFGnY6()
		{
			return ((Telepo)Telepo.Instance()).Teleport(hop.Record.RowID, hop.Record.SubIndex);
		}

		internal bool SFYOypHQbv()
		{
			if (GameState.TerritoryType == hop.TargetZone && LocalPlayerState.Object != null)
			{
				return !UIModuleExtension.IsScreenReady();
			}
			return false;
		}

		internal bool JEMOLXoRZL()
		{
			return _003C_003E4__this.lihHAtPaRx(hop.Record.Position, cancelAnimation: true, skipDismount: false);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass34_0
	{
		public TelepotTownHop hop;

		public cMXPetHw3Yp4XkysPB7 _003C_003E4__this;

		public _003C_003Ec__DisplayClass34_0()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
		}

		internal bool oJiOHEJ6gd()
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			AddonSelectStringEvent.Select(((object)LuminaGetter.GetRowOrDefault<AetheryteTransport>(1u).Text/*cast due to constrained. prefix*/).ToString());
			return AetheryteRecord.UseAethernetToZone(hop.TargetZone);
		}

		internal bool bHFOSjIZoZ()
		{
			if (GameState.TerritoryType == hop.Target.ZoneID)
			{
				return LocalPlayerState.Object != null;
			}
			return false;
		}

		internal bool wZrOVer8yD()
		{
			return _003C_003E4__this.lihHAtPaRx(hop.Target.Position, cancelAnimation: true, skipDismount: false);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass35_0
	{
		public EventHop hop;

		public cMXPetHw3Yp4XkysPB7 _003C_003E4__this;

		public _003C_003Ec__DisplayClass35_0()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
		}

		internal bool eHkO94OpBM()
		{
			if (GameState.TerritoryType != hop.Method.SourceZone)
			{
				return false;
			}
			if (UIModuleExtension.IsScreenReady())
			{
				return true;
			}
			OmenServiceBase<MovementManager>.Instance().TPPlayerAddress(hop.Method.EventPosition);
			return false;
		}

		internal bool mxmOfbtdgV()
		{
			if (hop.Method.IsEventNearby())
			{
				return true;
			}
			if (!UIModuleExtension.IsScreenReady() || _003C_003E4__this.TPTaskHelper.IsBusy)
			{
				return false;
			}
			_003C_003E4__this.lihHAtPaRx(hop.Method.EventPosition, cancelAnimation: true, skipDismount: false);
			return false;
		}
	}

	private static readonly CompSig ShowInstanceSelectionSig;

	private Hook<DZFxDwjQsR1QhxVefjJ>? ShowInstanceSelectionHook;

	private Hook<HandleActorControlPacket>? HandleActorControlPacketHook;

	private static readonly string[] BlacklistAnimations;

	public TaskHelper TaskHelper { get; private set; }

	public TaskHelper TPTaskHelper { get; private set; }

	public TaskHelper MethodTaskHelper { get; private set; }

	private static MovementManager Manager => OmenServiceBase<MovementManager>.Instance();

	protected unsafe override Task Init()
	{
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Expected O, but got Unknown
		TaskHelper = new TaskHelper
		{
			TimeoutMS = 15000,
			ShowDebug = true,
			MoveToNextCheckFunc = () => !TPTaskHelper.IsBusy && !MethodTaskHelper.IsBusy,
			RetryIntervalMS = 100,
			TimeoutAction = delegate
			{
				TaskHelper.Abort();
				TPTaskHelper.Abort();
				MethodTaskHelper.Abort();
			}
		};
		TPTaskHelper = new TaskHelper
		{
			TimeoutMS = 30000,
			ShowDebug = true,
			RetryIntervalMS = 100,
			TimeoutAction = delegate
			{
				TaskHelper.Abort();
				TPTaskHelper.Abort();
				MethodTaskHelper.Abort();
			}
		};
		MethodTaskHelper = new TaskHelper
		{
			TimeoutMS = 30000,
			ShowDebug = true,
			MoveToNextCheckFunc = () => !TPTaskHelper.IsBusy,
			RetryIntervalMS = 100,
			TimeoutAction = delegate
			{
				TaskHelper.Abort();
				TPTaskHelper.Abort();
				MethodTaskHelper.Abort();
			}
		};
		Manager.TaskHelper = TaskHelper;
		Manager.IsBusyHandler = () => TaskHelper.IsBusy || TPTaskHelper.IsBusy || MethodTaskHelper.IsBusy;
		Manager.AbortTasksHandler = delegate
		{
			TaskHelper.Abort();
			TPTaskHelper.Abort();
			MethodTaskHelper.Abort();
		};
		Manager.TPSmartInZoneHandler = delegate(Vector3 pos, ITPSmartParams? param)
		{
			TPSmartParams tPSmartParams = (param as TPSmartParams) ?? new TPSmartParams();
			return lihHAtPaRx(pos, tPSmartParams.CancelAnimation, tPSmartParams.SkipDismount);
		};
		Manager.TPSmartBetweenZoneHandler = delegate(uint zone, Vector3? pos, ITPSmartParams? param)
		{
			TPSmartParams tPSmartParams = (param as TPSmartParams) ?? new TPSmartParams();
			return PcHHcB8b8j(zone, pos, tPSmartParams.CancelAnimation, tPSmartParams.SkipDismount);
		};
		ShowInstanceSelectionHook = ShowInstanceSelectionSig.GetHook<DZFxDwjQsR1QhxVefjJ>(RPwHFga5H3);
		ShowInstanceSelectionHook.Enable();
		HandleActorControlPacketHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromMemberFunction<HandleActorControlPacket>(typeof(MemberFunctionPointers), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2F0C1AF ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d964b0b1313049f490f2107d03a6ed3a), new HandleActorControlPacket(wH4Hq6APFC));
		HandleActorControlPacketHook.Enable();
		OmenServiceBase<LogMessageManager>.Instance().RegPre(VhpHbrNbZh);
		OmenServiceBase<GameResourceManager>.Instance().AddToBlacklist(GetType(), BlacklistAnimations);
		return Task.CompletedTask;
	}

	protected override Task Uninit()
	{
		Manager.TPSmartInZoneHandler = null;
		Manager.TPSmartBetweenZoneHandler = null;
		Manager.IsBusyHandler = null;
		Manager.AbortTasksHandler = null;
		Manager.TaskHelper = null;
		TaskHelper.Dispose();
		TaskHelper = null;
		TPTaskHelper.Dispose();
		TPTaskHelper = null;
		MethodTaskHelper.Dispose();
		MethodTaskHelper = null;
		ShowInstanceSelectionHook?.Dispose();
		ShowInstanceSelectionHook = null;
		HandleActorControlPacketHook?.Dispose();
		HandleActorControlPacketHook = null;
		OmenServiceBase<GameResourceManager>.Instance().RemoveFromBlacklist(GetType(), BlacklistAnimations);
		OmenServiceBase<LogMessageManager>.Instance().Unreg(VhpHbrNbZh);
		return Task.CompletedTask;
	}

	private static void VhpHbrNbZh(ref bool isPrevented, ref uint logMessageID, ref LogMessageQueueItem item)
	{
		if (logMessageID == 727)
		{
			isPrevented = true;
		}
	}

	private unsafe void RPwHFga5H3(PublicInstance* instance, uint territoryType, uint instanceCount, uint* instancePlayerCounts, uint instancePlayerCountsLength)
	{
		if (!Manager.IsManagerBusy)
		{
			ShowInstanceSelectionHook.Original(instance, territoryType, instanceCount, instancePlayerCounts, instancePlayerCountsLength);
		}
		else
		{
			OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.SelectInstancedArea, territoryType, uint.MaxValue);
		}
	}

	private void wH4Hq6APFC(uint entityID, uint category, uint arg1, uint arg2, uint arg3, uint arg4, uint arg5, uint arg6, uint arg7, uint arg8, GameObjectId targetID, bool isRecorded)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		HandleActorControlPacketHook.Original.Invoke(entityID, category, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, targetID, isRecorded);
		if (entityID == LocalPlayerState.EntityID && category == 15 && arg1 == 537 && arg2 == 1 && arg3 == 5 && Manager.IsManagerBusy)
		{
			Manager.AbortTasksHandler();
		}
	}

	private bool PcHHcB8b8j(uint zone, Vector3? pos, bool cancelAnimation, bool skipDismount)
	{
		_003C_003Ec__DisplayClass23_0 CS_0024_003C_003E8__locals29 = new _003C_003Ec__DisplayClass23_0();
		CS_0024_003C_003E8__locals29._003C_003E4__this = this;
		CS_0024_003C_003E8__locals29.pos = pos;
		CS_0024_003C_003E8__locals29.cancelAnimation = cancelAnimation;
		CS_0024_003C_003E8__locals29.skipDismount = skipDismount;
		CS_0024_003C_003E8__locals29.zone = zone;
		if (!LuminaGetter.TryGetRow<TerritoryType>(CS_0024_003C_003E8__locals29.zone, out TerritoryType _) || LocalPlayerState.Object == null)
		{
			return false;
		}
		if (CS_0024_003C_003E8__locals29.zone == GameState.TerritoryType)
		{
			DLog.Debug(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x53783401 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_019a6a2bbee24ddb9dd8d9a1d1e1ca09));
			if (CS_0024_003C_003E8__locals29.pos.HasValue)
			{
				TaskHelper.Enqueue(() => CS_0024_003C_003E8__locals29._003C_003E4__this.lihHAtPaRx(CS_0024_003C_003E8__locals29.pos.Value, CS_0024_003C_003E8__locals29.cancelAnimation, CS_0024_003C_003E8__locals29.skipDismount), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2E95837F ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_6103a9bab63e42479916a134b07f1c75));
			}
			return true;
		}
		if (GameState.ContentFinderCondition != 0)
		{
			DLog.Debug(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x7C6D2BD4 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d2f07ce64d464f30b1d7397ae7500cac));
			TaskHelper.Enqueue(delegate
			{
				DutyCommand.Leave(DutyCommand.LeaveDutyKind.Inactive);
			}, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3923A52E ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f6778e21204a4d43ae5bbed939acf8a2));
			TaskHelper.Enqueue(() => GameState.ContentFinderCondition == 0 && UIModuleExtension.IsScreenReady(), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4628F55F ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_93e1cfc241b644019a4819c49327d6a7));
			TaskHelper.DelayNext(1000, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5369DBF8 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d4ce857df7b047629e7091bbc815dd94));
			TaskHelper.Enqueue(() => CS_0024_003C_003E8__locals29._003C_003E4__this.PcHHcB8b8j(CS_0024_003C_003E8__locals29.zone, CS_0024_003C_003E8__locals29.pos, CS_0024_003C_003E8__locals29.cancelAnimation, CS_0024_003C_003E8__locals29.skipDismount), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4FBFC1DD ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_881eb9df907345dea66a751ad6d65e76));
			return true;
		}
		List<ZoneHop> list = MovementManager.FindBestPath(GameState.TerritoryType, CS_0024_003C_003E8__locals29.zone);
		if (list != null && list.Count > 0)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 2);
			defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2829F4FD ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_95ac0b4adf764381bcdc75a9d241d184));
			defaultInterpolatedStringHandler.AppendFormatted(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x34A3B518 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5e416caedcb6438f915024f390fd9079));
			defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x75EF58BF ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_11422b9d07f9498fb995b7dd8e417b5b));
			defaultInterpolatedStringHandler.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x75EF58D7 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_11422b9d07f9498fb995b7dd8e417b5b));
			DLog.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
			SpVH22EOIV(list, CS_0024_003C_003E8__locals29.pos, CS_0024_003C_003E8__locals29.cancelAnimation, CS_0024_003C_003E8__locals29.skipDismount);
			return true;
		}
		DLog.Debug(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4628F595 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_93e1cfc241b644019a4819c49327d6a7));
		AetheryteRecord nearestAetheryte = OmenServiceBase<AetheryteRecordManager>.Instance().GetNearestAetheryte(CS_0024_003C_003E8__locals29.zone, CS_0024_003C_003E8__locals29.pos.GetValueOrDefault());
		if (nearestAetheryte != null)
		{
			rW4HkFnUQ9(nearestAetheryte, CS_0024_003C_003E8__locals29.zone);
			TaskHelper.Enqueue(() => CS_0024_003C_003E8__locals29._003C_003E4__this.PcHHcB8b8j(CS_0024_003C_003E8__locals29.zone, CS_0024_003C_003E8__locals29.pos, CS_0024_003C_003E8__locals29.cancelAnimation, CS_0024_003C_003E8__locals29.skipDismount), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x768E62D3 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_417cae18312d413c80505172551e7b46));
		}
		return true;
	}

	private unsafe bool lihHAtPaRx(Vector3 pos, bool cancelAnimation, bool skipDismount)
	{
		IPlayerCharacter playerCharacter = LocalPlayerState.Object;
		if (playerCharacter == null)
		{
			return false;
		}
		bool flag = Sheets.SpeedDetectionZones.ContainsKey(GameState.TerritoryType);
		bool flag2 = MovementManager.IsOnlyYAxisChanged(playerCharacter.Position, pos);
		float num = LocalPlayerState.DistanceTo3DSquared(pos);
		if (IConditionExtension.get_IsBetweenAreas(DalamudServiceExtension.Instance<ICondition>()) || !flag || flag2 || num < 8f || (GameState.IsInFieldOperations && num < 9216f))
		{
			return Manager.TPPlayerAddress(pos);
		}
		if (!DalamudServiceExtension.Instance<ICondition>().Any((ConditionFlag[])(object)new ConditionFlag[1] { (ConditionFlag)77 }) && MovementManager.TryGetExitRangeInstance(out var _))
		{
			return UvRHGKll6Y(pos);
		}
		if (!skipDismount && Control.CanFly && DalamudServiceExtension.Instance<ICondition>().Any((ConditionFlag[])(object)new ConditionFlag[2]
		{
			(ConditionFlag)4,
			(ConditionFlag)77
		}))
		{
			return moTHr00mqV(pos);
		}
		if (!AuthState.IsPremium && (object)OmenServiceBase<AetheryteRecordManager>.Instance().GetNearestAetheryte(GameState.TerritoryType, pos) != null)
		{
			return vPiHQTgEs8(pos);
		}
		return GZCHJWNjLk(pos, cancelAnimation);
	}

	private unsafe bool vPiHQTgEs8(Vector3 pos)
	{
		_003C_003Ec__DisplayClass25_0 CS_0024_003C_003E8__locals9 = new _003C_003Ec__DisplayClass25_0();
		CS_0024_003C_003E8__locals9.pos = pos;
		CS_0024_003C_003E8__locals9.aetheryte = OmenServiceBase<AetheryteRecordManager>.Instance().GetNearestAetheryte(GameState.TerritoryType, CS_0024_003C_003E8__locals9.pos);
		if ((object)CS_0024_003C_003E8__locals9.aetheryte == null)
		{
			return false;
		}
		if (LocalPlayerState.Object == null)
		{
			return false;
		}
		if (!UIModuleExtension.IsScreenReady())
		{
			return Manager.TPPlayerAddress(CS_0024_003C_003E8__locals9.pos);
		}
		TPTaskHelper.Enqueue(() => ((Telepo)Telepo.Instance()).Teleport(CS_0024_003C_003E8__locals9.aetheryte.RowID, CS_0024_003C_003E8__locals9.aetheryte.SubIndex), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4FBFC0B1 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_881eb9df907345dea66a751ad6d65e76), 0, null, null, null, null, 1000);
		TPTaskHelper.Enqueue(() => GameState.TerritoryType == CS_0024_003C_003E8__locals9.aetheryte.ZoneID && LocalPlayerState.Object != null && !UIModuleExtension.IsScreenReady(), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x27BB22B5 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7f9a8e3b1ea249bebe4aa86658ee9503), 8000, TaskAbortBehaviour.AbortAll, null, null, null, 1000);
		TPTaskHelper.Enqueue(delegate
		{
			if (UIModuleExtension.IsScreenReady())
			{
				return true;
			}
			Manager.TPPlayerAddress(CS_0024_003C_003E8__locals9.pos);
			return false;
		}, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x37CCF1B3 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_90dae8e50e3a4314867cef21f0bb1bb0), 0, null, null, null, null, 1000);
		return true;
	}

	private unsafe bool GZCHJWNjLk(Vector3 pos, bool cancelAnimation)
	{
		_003C_003Ec__DisplayClass26_0 CS_0024_003C_003E8__locals10 = new _003C_003Ec__DisplayClass26_0();
		CS_0024_003C_003E8__locals10.pos = pos;
		CS_0024_003C_003E8__locals10.localPlayer = LocalPlayerState.Object;
		if (CS_0024_003C_003E8__locals10.localPlayer == null)
		{
			return false;
		}
		if (!UIModuleExtension.IsScreenReady())
		{
			return Manager.TPPlayerAddress(CS_0024_003C_003E8__locals10.pos);
		}
		if (cancelAnimation && ManagerBase<PlayersManager>.Instance().PlayersAroundCount > 0)
		{
			Vector3 position = CS_0024_003C_003E8__locals10.localPlayer.Position;
			Vector3 vector = position;
			vector.Y = position.Y - 100f;
			Vector3 position2 = vector;
			if (RaycastHelper.TryGetGroundPosition(position, out var groundPosition))
			{
				vector = groundPosition;
				vector.Y = groundPosition.Y - 100f;
				position2 = vector;
			}
			if (!IConditionExtension.get_IsBoundByDuty(DalamudServiceExtension.Instance<ICondition>()))
			{
				new PositionUpdatePacket(CS_0024_003C_003E8__locals10.localPlayer.Rotation, position2, Manager.GetMoveType(PositionUpdatePacket.MoveType.NormalMove0)).Send();
			}
			else
			{
				new PositionUpdateInstancePacket(CS_0024_003C_003E8__locals10.localPlayer.Rotation, position2, Manager.GetInstanceMoveType(PositionUpdateInstancePacket.MoveType.NormalMove0)).Send();
			}
			TPTaskHelper.DelayNext(750, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x70C33ADB ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c041f06f3e304b758cab1109bed434d0), 1000);
		}
		TPTaskHelper.Enqueue(() => UIModuleExtension.IsScreenReady(), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x9072148 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_444e42c3749640698bcaba67a1c5b5f3), 0, null, null, null, null, 1000);
		TPTaskHelper.Enqueue(delegate
		{
			if (LocalPlayerState.DistanceTo2DSquared(CS_0024_003C_003E8__locals10.pos.ToVector2()) <= 9f)
			{
				return true;
			}
			if (!Throttler.Shared.Throttle(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x200F836D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_062a9cfe150f4dc6a9fdd7b6d00434af), 1000u))
			{
				return false;
			}
			if (!UIModuleExtension.IsScreenReady())
			{
				return false;
			}
			new DiveStartPacket(CS_0024_003C_003E8__locals10.pos + new Vector3(0f, 2f, 0f), CS_0024_003C_003E8__locals10.localPlayer.Rotation).Send();
			return false;
		}, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6117D94E ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_8883645b187042ce8990c01069a51402), 0, null, null, null, null, 1000);
		if (cancelAnimation)
		{
			TPTaskHelper.Enqueue(delegate
			{
				IPlayerCharacter playerCharacter = LocalPlayerState.Object;
				if (playerCharacter == null)
				{
					return false;
				}
				((BattleChara)playerCharacter.ToStruct()).DisableDraw();
				((BattleChara)playerCharacter.ToStruct()).EnableDraw();
				return false;
			}, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x642B04AC ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7f11a41e9f664db18364eb0afc92a5df), 2000, TaskAbortBehaviour.AbortCurrent, null, delegate
			{
			}, null, 1000);
		}
		return true;
	}

	private static bool moTHr00mqV(Vector3 pos)
	{
		if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4] && Control.CanFly)
		{
			IPlayerCharacter playerCharacter = LocalPlayerState.Object;
			if (playerCharacter != null)
			{
				PositionUpdatePacket.Send(playerCharacter.Rotation, VectorExtensions.WithY(playerCharacter.Position, pos.Y - 100f), PositionUpdatePacket.MoveType.Fly0);
				OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommandComplexLocation(ExecuteCommandComplexFlag.Dismount, pos, RotationHelper.CharaToPacket(playerCharacter.Rotation), 1u);
				return true;
			}
		}
		return false;
	}

	private unsafe static bool UvRHGKll6Y(Vector3 pos)
	{
		if (!MovementManager.TryGetExitRangeInstance(out var instance))
		{
			return false;
		}
		PopRangeManager.Instance()->PopRange((ILayoutInstance*)instance, pos);
		return true;
	}

	internal unsafe bool RlfH6ANkQY(AetheryteRecord record)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		_003C_003Ec__DisplayClass29_0 CS_0024_003C_003E8__locals29 = new _003C_003Ec__DisplayClass29_0();
		CS_0024_003C_003E8__locals29.record = record;
		CS_0024_003C_003E8__locals29._003C_003E4__this = this;
		if (CS_0024_003C_003E8__locals29.record.IsHouse)
		{
			((Telepo)Telepo.Instance()).Teleport(CS_0024_003C_003E8__locals29.record.RowID, CS_0024_003C_003E8__locals29.record.SubIndex);
			return true;
		}
		if ((int)GameState.TerritoryIntendedUse == 0 && CS_0024_003C_003E8__locals29.record.ZoneID != GameState.TerritoryType && (*EventFramework.Instance()).TryGetNearestEventID((EventHandlerInfo x) => (int)x.EventId.ContentId == 5, (GameObject _) => true, Vector3.Zero, out CS_0024_003C_003E8__locals29.eventIDAetheryte))
		{
			CS_0024_003C_003E8__locals29.currentZoneRecord = OmenServiceBase<AetheryteRecordManager>.Instance().GetNearestAetheryte(GameState.TerritoryType, Vector3.Zero);
			if ((object)CS_0024_003C_003E8__locals29.currentZoneRecord != null && CS_0024_003C_003E8__locals29.currentZoneRecord.Group > 0)
			{
				CS_0024_003C_003E8__locals29.targetZoneRecord = OmenServiceBase<AetheryteRecordManager>.Instance().AllRecords.FirstOrDefault((AetheryteRecord x) => x.Group == CS_0024_003C_003E8__locals29.currentZoneRecord.Group && x.ZoneID == CS_0024_003C_003E8__locals29.record.ZoneID);
				if ((object)CS_0024_003C_003E8__locals29.targetZoneRecord != null)
				{
					TaskHelper.Enqueue(delegate
					{
						//IL_001e: Unknown result type (might be due to invalid IL or missing references)
						if (!(*Addons.TelepotTown).IsAddonAndNodesReady())
						{
							new EventStartPackt(LocalPlayerState.EntityID, EventId.op_Implicit(CS_0024_003C_003E8__locals29.eventIDAetheryte)).Send();
						}
					}, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x768E6227 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_417cae18312d413c80505172551e7b46));
					TaskHelper.Enqueue(delegate
					{
						//IL_0010: Unknown result type (might be due to invalid IL or missing references)
						//IL_0015: Unknown result type (might be due to invalid IL or missing references)
						AddonSelectStringEvent.Select(((object)LuminaGetter.GetRowOrDefault<AetheryteTransport>(1u).Text/*cast due to constrained. prefix*/).ToString());
						return CS_0024_003C_003E8__locals29.targetZoneRecord.UseAethernet();
					});
					TaskHelper.Enqueue(() => GameState.TerritoryType == CS_0024_003C_003E8__locals29.targetZoneRecord.ZoneID, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4732CE2D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5f88650ee89f435aad988a8583bda5b5));
					TaskHelper.Enqueue(() => CS_0024_003C_003E8__locals29._003C_003E4__this.lihHAtPaRx(CS_0024_003C_003E8__locals29.record.Position, cancelAnimation: true, skipDismount: false), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x75EF5A13 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_11422b9d07f9498fb995b7dd8e417b5b));
					return true;
				}
			}
		}
		if (CS_0024_003C_003E8__locals29.record.Group != 0 || !AetheryteRecords.AethernetGroups.Contains(CS_0024_003C_003E8__locals29.record.Group))
		{
			PcHHcB8b8j(CS_0024_003C_003E8__locals29.record.ZoneID, CS_0024_003C_003E8__locals29.record.Position, cancelAnimation: true, skipDismount: false);
			return true;
		}
		if (CS_0024_003C_003E8__locals29.record.IsAetheryte && GameState.TerritoryType == CS_0024_003C_003E8__locals29.record.ZoneID)
		{
			((Telepo)Telepo.Instance()).Teleport(CS_0024_003C_003E8__locals29.record.RowID, CS_0024_003C_003E8__locals29.record.SubIndex);
			return true;
		}
		PcHHcB8b8j(CS_0024_003C_003E8__locals29.record.ZoneID, CS_0024_003C_003E8__locals29.record.Position, cancelAnimation: true, skipDismount: false);
		return true;
	}

	private unsafe void rW4HkFnUQ9(AetheryteRecord aetheryte, uint waitZone)
	{
		_003C_003Ec__DisplayClass30_0 CS_0024_003C_003E8__locals6 = new _003C_003Ec__DisplayClass30_0();
		CS_0024_003C_003E8__locals6.aetheryte = aetheryte;
		CS_0024_003C_003E8__locals6.waitZone = waitZone;
		TaskHelper.Enqueue(() => ((Telepo)Telepo.Instance()).Teleport(CS_0024_003C_003E8__locals6.aetheryte.RowID, CS_0024_003C_003E8__locals6.aetheryte.SubIndex), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3D7C6D1A ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_cda76bbf50614e97b4ef38939106c571) + CS_0024_003C_003E8__locals6.aetheryte.Name);
		TaskHelper.Enqueue(() => GameState.TerritoryType == CS_0024_003C_003E8__locals6.waitZone && LocalPlayerState.Object != null && UIModuleExtension.IsScreenReady(), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x75EF5A3B ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_11422b9d07f9498fb995b7dd8e417b5b), 8000, TaskAbortBehaviour.AbortAll);
	}

	private void SpVH22EOIV(List<ZoneHop> route, Vector3? finalPos, bool cancelAnimation, bool skipDismount)
	{
		_003C_003Ec__DisplayClass31_0 CS_0024_003C_003E8__locals38 = new _003C_003Ec__DisplayClass31_0();
		CS_0024_003C_003E8__locals38._003C_003E4__this = this;
		CS_0024_003C_003E8__locals38.cancelAnimation = cancelAnimation;
		CS_0024_003C_003E8__locals38.skipDismount = skipDismount;
		int num = 1000;
		for (int i = 0; i < route.Count; i++)
		{
			_003C_003Ec__DisplayClass31_1 CS_0024_003C_003E8__locals34 = new _003C_003Ec__DisplayClass31_1();
			CS_0024_003C_003E8__locals34.CS_0024_003C_003E8__locals5 = CS_0024_003C_003E8__locals38;
			ZoneHop zoneHop = route[i];
			CS_0024_003C_003E8__locals34.isFirst = i == 0;
			CS_0024_003C_003E8__locals34._003Ceh_003E5__2 = zoneHop as ExitRangeHop;
			if ((object)CS_0024_003C_003E8__locals34._003Ceh_003E5__2 == null)
			{
				CS_0024_003C_003E8__locals34._003Cah_003E5__3 = zoneHop as AetheryteHop;
				if ((object)CS_0024_003C_003E8__locals34._003Cah_003E5__3 == null)
				{
					CS_0024_003C_003E8__locals34._003Cth_003E5__4 = zoneHop as TelepotTownHop;
					if ((object)CS_0024_003C_003E8__locals34._003Cth_003E5__4 == null)
					{
						CS_0024_003C_003E8__locals34._003Cenpc_003E5__5 = zoneHop as EventHop;
						if ((object)CS_0024_003C_003E8__locals34._003Cenpc_003E5__5 != null)
						{
							_003C_003Ec__DisplayClass31_2 CS_0024_003C_003E8__locals27 = new _003C_003Ec__DisplayClass31_2();
							CS_0024_003C_003E8__locals27.CS_0024_003C_003E8__locals6 = CS_0024_003C_003E8__locals34;
							CS_0024_003C_003E8__locals27.w = num--;
							TaskHelper.Enqueue(delegate
							{
								CS_0024_003C_003E8__locals27.CS_0024_003C_003E8__locals6.CS_0024_003C_003E8__locals5._003C_003E4__this.L5KH5LZZ4u(CS_0024_003C_003E8__locals27.CS_0024_003C_003E8__locals6._003Cenpc_003E5__5, CS_0024_003C_003E8__locals27.w);
							}, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(-1675397288 ^ -1274379869 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_95ac0b4adf764381bcdc75a9d241d184) + CS_0024_003C_003E8__locals27.CS_0024_003C_003E8__locals6._003Cenpc_003E5__5.Method.DisplayName);
						}
					}
					else
					{
						TaskHelper.Enqueue(delegate
						{
							CS_0024_003C_003E8__locals34.CS_0024_003C_003E8__locals5._003C_003E4__this.FudH0OOuYr(CS_0024_003C_003E8__locals34._003Cth_003E5__4);
						}, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x200E15D5 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_062a9cfe150f4dc6a9fdd7b6d00434af) + CS_0024_003C_003E8__locals34._003Cth_003E5__4.Target.Name);
					}
				}
				else
				{
					TaskHelper.Enqueue(delegate
					{
						CS_0024_003C_003E8__locals34.CS_0024_003C_003E8__locals5._003C_003E4__this.J6WHaRgNto(CS_0024_003C_003E8__locals34._003Cah_003E5__3, CS_0024_003C_003E8__locals34.isFirst);
					}, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x227E0A1 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_e3177f14377e4fe58ddaaaaf90163f44) + CS_0024_003C_003E8__locals34._003Cah_003E5__3.Record.Name);
				}
			}
			else
			{
				TaskHelper taskHelper = TaskHelper;
				Action task = delegate
				{
					CS_0024_003C_003E8__locals34.CS_0024_003C_003E8__locals5._003C_003E4__this.ONhHZipysP(CS_0024_003C_003E8__locals34._003Ceh_003E5__2);
				};
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(6, 1);
				defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2BF1E104 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_fc2313f9e2c04f099d4c47c5893e635d));
				defaultInterpolatedStringHandler.AppendFormatted(CS_0024_003C_003E8__locals34._003Ceh_003E5__2.TargetZone);
				taskHelper.Enqueue(task, defaultInterpolatedStringHandler.ToStringAndClear());
			}
		}
		if (finalPos.HasValue)
		{
			CS_0024_003C_003E8__locals38.fp = finalPos.GetValueOrDefault();
			TaskHelper.Enqueue(delegate
			{
				CS_0024_003C_003E8__locals38._003C_003E4__this.lihHAtPaRx(CS_0024_003C_003E8__locals38.fp, CS_0024_003C_003E8__locals38.cancelAnimation, CS_0024_003C_003E8__locals38.skipDismount);
				return true;
			}, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4FBFC365 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_881eb9df907345dea66a751ad6d65e76));
		}
	}

	private unsafe void ONhHZipysP(ExitRangeHop hop)
	{
		_003C_003Ec__DisplayClass32_0 CS_0024_003C_003E8__locals3 = new _003C_003Ec__DisplayClass32_0();
		CS_0024_003C_003E8__locals3.hop = hop;
		TPTaskHelper.Enqueue(delegate
		{
			if (!UIModuleExtension.IsScreenReady())
			{
				return false;
			}
			if (!MovementManager.TryGetExitRangeInstance(out var instance, (nint p) => ((ExitRangeLayoutInstance*)p)->TerritoryType == CS_0024_003C_003E8__locals3.hop.TargetZone))
			{
				return false;
			}
			PopRangeManager.Instance()->PopRange((ILayoutInstance*)instance);
			return true;
		}, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x339D3A92 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c48859e0be41429f8474d6e2fecfa0de));
		TPTaskHelper.Enqueue(() => GameState.TerritoryType == CS_0024_003C_003E8__locals3.hop.TargetZone && LocalPlayerState.Object != null, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(--1976305562 ^ 0x60322268 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_e279476822c444f9955c257cab427fb2));
	}

	private unsafe void J6WHaRgNto(AetheryteHop hop, bool isFirst)
	{
		_003C_003Ec__DisplayClass33_0 CS_0024_003C_003E8__locals10 = new _003C_003Ec__DisplayClass33_0();
		CS_0024_003C_003E8__locals10.hop = hop;
		CS_0024_003C_003E8__locals10._003C_003E4__this = this;
		TPTaskHelper.Enqueue(() => UIModuleExtension.IsScreenReady(), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x7C6D2EC8 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d2f07ce64d464f30b1d7397ae7500cac));
		if (!isFirst)
		{
			TPTaskHelper.DelayNext(1000, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3D7C6DD2 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_cda76bbf50614e97b4ef38939106c571));
		}
		TPTaskHelper.Enqueue(() => ((Telepo)Telepo.Instance()).Teleport(CS_0024_003C_003E8__locals10.hop.Record.RowID, CS_0024_003C_003E8__locals10.hop.Record.SubIndex), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4732CD27 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5f88650ee89f435aad988a8583bda5b5) + CS_0024_003C_003E8__locals10.hop.Record.Name);
		TPTaskHelper.Enqueue(() => GameState.TerritoryType == CS_0024_003C_003E8__locals10.hop.TargetZone && LocalPlayerState.Object != null && !UIModuleExtension.IsScreenReady(), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x7C6D2E80 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d2f07ce64d464f30b1d7397ae7500cac) + LuminaWrapper.GetZonePlaceName(CS_0024_003C_003E8__locals10.hop.TargetZone), 8000, TaskAbortBehaviour.AbortAll);
		if (!CS_0024_003C_003E8__locals10.hop.Record.IsAetheryte)
		{
			TPTaskHelper.Enqueue(() => CS_0024_003C_003E8__locals10._003C_003E4__this.lihHAtPaRx(CS_0024_003C_003E8__locals10.hop.Record.Position, cancelAnimation: true, skipDismount: false), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x642B0760 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7f11a41e9f664db18364eb0afc92a5df));
		}
	}

	private unsafe void FudH0OOuYr(TelepotTownHop hop)
	{
		_003C_003Ec__DisplayClass34_0 CS_0024_003C_003E8__locals6 = new _003C_003Ec__DisplayClass34_0();
		CS_0024_003C_003E8__locals6.hop = hop;
		CS_0024_003C_003E8__locals6._003C_003E4__this = this;
		TPTaskHelper.Enqueue(delegate
		{
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			if ((*Addons.TelepotTown).IsAddonAndNodesReady())
			{
				return true;
			}
			if (!UIModuleExtension.IsScreenReady())
			{
				return false;
			}
			if (!(*EventFramework.Instance()).TryGetNearestEventID((EventHandlerInfo x) => (int)x.EventId.ContentId == 5, (GameObject _) => true, Vector3.Zero, out var eventID))
			{
				return false;
			}
			new EventStartPackt(LocalPlayerState.EntityID, EventId.op_Implicit(eventID)).Send();
			return true;
		}, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x339D3B0C ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c48859e0be41429f8474d6e2fecfa0de));
		TPTaskHelper.Enqueue(delegate
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			AddonSelectStringEvent.Select(((object)LuminaGetter.GetRowOrDefault<AetheryteTransport>(1u).Text/*cast due to constrained. prefix*/).ToString());
			return AetheryteRecord.UseAethernetToZone(CS_0024_003C_003E8__locals6.hop.TargetZone);
		}, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5E39EB8F ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f9eb04af496747c5b32cddd51d0b3997));
		TPTaskHelper.Enqueue(() => GameState.TerritoryType == CS_0024_003C_003E8__locals6.hop.Target.ZoneID && LocalPlayerState.Object != null, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x642B048C ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7f11a41e9f664db18364eb0afc92a5df));
		TPTaskHelper.Enqueue(() => CS_0024_003C_003E8__locals6._003C_003E4__this.lihHAtPaRx(CS_0024_003C_003E8__locals6.hop.Target.Position, cancelAnimation: true, skipDismount: false), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x76DA2039 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f8c073ebdce5414baf147b1be2d2c2c2));
	}

	private void L5KH5LZZ4u(EventHop hop, int weight)
	{
		_003C_003Ec__DisplayClass35_0 CS_0024_003C_003E8__locals10 = new _003C_003Ec__DisplayClass35_0();
		CS_0024_003C_003E8__locals10.hop = hop;
		CS_0024_003C_003E8__locals10._003C_003E4__this = this;
		TaskHelper methodTaskHelper = MethodTaskHelper;
		Func<bool> task = delegate
		{
			if (GameState.TerritoryType != CS_0024_003C_003E8__locals10.hop.Method.SourceZone)
			{
				return false;
			}
			if (UIModuleExtension.IsScreenReady())
			{
				return true;
			}
			OmenServiceBase<MovementManager>.Instance().TPPlayerAddress(CS_0024_003C_003E8__locals10.hop.Method.EventPosition);
			return false;
		};
		string name = GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6CA636EB ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_86e8e55020514087b1fe097f879d6dff);
		int weight2 = weight;
		methodTaskHelper.Enqueue(task, name, 0, null, null, null, null, weight2);
		TaskHelper methodTaskHelper2 = MethodTaskHelper;
		Func<bool> task2 = delegate
		{
			if (CS_0024_003C_003E8__locals10.hop.Method.IsEventNearby())
			{
				return true;
			}
			if (!UIModuleExtension.IsScreenReady() || CS_0024_003C_003E8__locals10._003C_003E4__this.TPTaskHelper.IsBusy)
			{
				return false;
			}
			CS_0024_003C_003E8__locals10._003C_003E4__this.lihHAtPaRx(CS_0024_003C_003E8__locals10.hop.Method.EventPosition, cancelAnimation: true, skipDismount: false);
			return false;
		};
		string name2 = GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6CA6376F ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_86e8e55020514087b1fe097f879d6dff);
		weight2 = weight;
		methodTaskHelper2.Enqueue(task2, name2, 0, null, null, null, null, weight2);
		CS_0024_003C_003E8__locals10.hop.Method.Enqueue(MethodTaskHelper, CS_0024_003C_003E8__locals10.hop.TargetZone, weight);
	}

	public cMXPetHw3Yp4XkysPB7()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
	}

	static cMXPetHw3Yp4XkysPB7()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		ShowInstanceSelectionSig = new CompSig(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3F3402F0 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_128c630676db4a8ab2ed83db4c2b1862));
		BlacklistAnimations = new string[4]
		{
			GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(--1976305562 ^ 0x6BE8E8C9 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_86afda88005749b094dac2a7d6b27c25),
			GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x75EF5BC7 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_11422b9d07f9498fb995b7dd8e417b5b),
			GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2CC051DC ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c02509fc64c64faaa7b59b967c4e5f83),
			GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x7D14F5AB ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1dfb87339e314237a8eb0a6453e2a275)
		};
	}

	[CompilerGenerated]
	private bool oCiHR9yBuv()
	{
		if (!TPTaskHelper.IsBusy)
		{
			return !MethodTaskHelper.IsBusy;
		}
		return false;
	}

	[CompilerGenerated]
	private void WibHDay5ZF()
	{
		TaskHelper.Abort();
		TPTaskHelper.Abort();
		MethodTaskHelper.Abort();
	}

	[CompilerGenerated]
	private void BZmHexQ8gT()
	{
		TaskHelper.Abort();
		TPTaskHelper.Abort();
		MethodTaskHelper.Abort();
	}

	[CompilerGenerated]
	private bool G8SHN0u8lK()
	{
		return !TPTaskHelper.IsBusy;
	}

	[CompilerGenerated]
	private void PwhH7FIfTN()
	{
		TaskHelper.Abort();
		TPTaskHelper.Abort();
		MethodTaskHelper.Abort();
	}

	[CompilerGenerated]
	private bool vWJHdClrf7()
	{
		if (!TaskHelper.IsBusy && !TPTaskHelper.IsBusy)
		{
			return MethodTaskHelper.IsBusy;
		}
		return true;
	}

	[CompilerGenerated]
	private void VQcHIf5MEn()
	{
		TaskHelper.Abort();
		TPTaskHelper.Abort();
		MethodTaskHelper.Abort();
	}

	[CompilerGenerated]
	private bool nfLHUImlYs(Vector3 pos, ITPSmartParams? param)
	{
		TPSmartParams tPSmartParams = (param as TPSmartParams) ?? new TPSmartParams();
		return lihHAtPaRx(pos, tPSmartParams.CancelAnimation, tPSmartParams.SkipDismount);
	}

	[CompilerGenerated]
	private bool HrJHuUWyiZ(uint zone, Vector3? pos, ITPSmartParams? param)
	{
		TPSmartParams tPSmartParams = (param as TPSmartParams) ?? new TPSmartParams();
		return PcHHcB8b8j(zone, pos, tPSmartParams.CancelAnimation, tPSmartParams.SkipDismount);
	}
}
