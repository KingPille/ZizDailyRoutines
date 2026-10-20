using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.LayoutEngine;
using FFXIVClientStructs.Interop;
using FFXIVClientStructs.STD;
using FFXIVClientStructs.STD.ContainerInterface;
using Lumina.Excel.Sheets;
using OmenTools.Dalamud;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Info.Game.AetheryteRecord;
using OmenTools.Info.Game.AetheryteRecord.Data;
using OmenTools.Info.Game.Data;
using OmenTools.Info.Game.Enums;
using OmenTools.Info.Game.Packets.Upstream;
using OmenTools.Info.Lumina;
using OmenTools.Info.Lumina.LGB;
using OmenTools.Info.Models;
using OmenTools.Interop.Game.Helpers;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.Interop.Game.Models.Native;
using OmenTools.OmenService.Abstractions;
using OmenTools.TerritoryTransport.Abstractions;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;

namespace OmenTools.OmenService;

public class MovementManager : OmenServiceBase<MovementManager>
{
	public delegate bool TPSmartInZoneDelegate(Vector3 pos, ITPSmartParams? param = null);

	public delegate bool TPSmartBetweenZoneDelegate(uint zone, Vector3? pos, ITPSmartParams? param = null);

	private unsafe delegate ushort GetCurrentZoneMoveStateDelegate(byte* instance);

	public class Config : OmenServiceConfig
	{
		public bool AlwaysMinGilTransport { get; set; }

		public uint HopCumulativePenalty { get; set; } = 5000u;

		public void Save()
		{
			this.Save(OmenServiceBase<MovementManager>.Instance());
		}
	}

	private readonly record struct PathState(uint Zone, Vector3 Pos);

	private readonly record struct PathCost(uint EffectiveCost, int Hops) : IComparable<PathCost>
	{
		public int CompareTo(PathCost other)
		{
			int num = EffectiveCost.CompareTo(other.EffectiveCost);
			if (num == 0)
			{
				return Hops.CompareTo(other.Hops);
			}
			return num;
		}
	}

	private readonly record struct PathEdge(ZoneHop Hop, PathState PrevState, uint HopTotalCost);

	private static readonly CompSig CurrentMoveModeInternalSig = new CompSig("0F B6 0D ?? ?? ?? ?? B8");

	private unsafe byte* CurrentMoveModeInternal = null;

	private static readonly CompSig CurrentZoneMoveStateBaseInstanceSig = new CompSig("48 8D 0D ?? ?? ?? ?? E9 ?? ?? ?? ?? CC CC CC CC CC CC CC CC CC CC CC CC CC CC CC CC CC CC CC CC 48 89 5C 24 ?? 57 48 83 EC ?? 48 8B B9");

	private unsafe byte* CurrentZoneMoveStateBaseInstance = null;

	private static readonly CompSig GetCurrentZoneMoveStateSig = new CompSig("E8 ?? ?? ?? ?? 66 83 C8 40");

	private GetCurrentZoneMoveStateDelegate GetCurrentZoneMoveState;

	private Config config;

	private const uint TELEPORT_TICKET_ITEM_ID = 7569u;

	public TPSmartInZoneDelegate? TPSmartInZoneHandler { get; set; }

	public TPSmartBetweenZoneDelegate? TPSmartBetweenZoneHandler { get; set; }

	public Func<bool>? IsBusyHandler { get; set; }

	public Action? AbortTasksHandler { get; set; }

	public TaskHelper? TaskHelper { get; set; }

	public List<EventTransportBase> EventTransportations { get; private set; } = new List<EventTransportBase>();

	public bool AlwaysMinGilTransport
	{
		get
		{
			return config.AlwaysMinGilTransport;
		}
		set
		{
			config.AlwaysMinGilTransport = value;
		}
	}

	public bool IsManagerBusy
	{
		get
		{
			TaskHelper? taskHelper = TaskHelper;
			if (taskHelper != null && !taskHelper.IsBusy)
			{
				return IsBusyHandler?.Invoke() ?? false;
			}
			return true;
		}
	}

	public unsafe MovementControlMode CurrentControlMode
	{
		get
		{
			return (MovementControlMode)(*CurrentMoveModeInternal);
		}
		set
		{
			*CurrentMoveModeInternal = (byte)value;
		}
	}

	public unsafe ushort CurrentZoneMoveState => (ushort)(GetCurrentZoneMoveState(CurrentZoneMoveStateBaseInstance) % 4);

	private static uint TicketUsageGilSetting => DalamudServiceExtension.Instance<IGameConfig>().UiConfig.GetUInt("TelepoTicketGilSetting");

	private static uint TicketUsageType => DalamudServiceExtension.Instance<IGameConfig>().UiConfig.GetUInt("TelepoTicketUseType");

	public void AbortTasks()
	{
		AbortTasksHandler?.Invoke();
	}

	public void SaveConfig()
	{
		config.Save();
	}

	public bool TPSmart_InZone(Vector3 pos, ITPSmartParams? param = null)
	{
		return TPSmartInZoneHandler?.Invoke(pos, param) ?? false;
	}

	public bool TPSmart_BetweenZone(uint zone, Vector3? pos = null, ITPSmartParams? param = null)
	{
		return TPSmartBetweenZoneHandler?.Invoke(zone, pos, param) ?? false;
	}

	public unsafe bool TPPlayerAddress(Vector3 pos)
	{
		IPlayerCharacter playerCharacter = LocalPlayerState.Object;
		if (playerCharacter == null)
		{
			return false;
		}
		if (DalamudServiceExtension.Instance<ICondition>().Any((ConditionFlag[])(object)new ConditionFlag[2]
		{
			(ConditionFlag)77,
			(ConditionFlag)81
		}))
		{
			PlayerController.Instance()->MoveControllerFly.MountPosition = pos;
		}
		else
		{
			((BattleChara)playerCharacter.ToStruct()).SetPosition(pos.X, pos.Y, pos.Z);
		}
		return LocalPlayerState.DistanceTo2DSquared(pos.ToVector2()) <= 9f;
	}

	public bool TPGround()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		IPlayerCharacter playerCharacter = LocalPlayerState.Object;
		if (playerCharacter == null || !RaycastHelper.TryGetNearestVerticalHit(playerCharacter.Position, out var hitInfo))
		{
			return false;
		}
		TPSmart_InZone(hitInfo.Point);
		return true;
	}

	public unsafe bool TPTeleport(Vector3 pos)
	{
		AetheryteRecord aetheryte = OmenServiceBase<AetheryteRecordManager>.Instance().GetNearestAetheryte(GameState.TerritoryType, pos);
		if ((object)aetheryte == null)
		{
			return false;
		}
		if (LocalPlayerState.Object == null)
		{
			return false;
		}
		if (!UIModuleExtension.IsScreenReady())
		{
			return TPPlayerAddress(pos);
		}
		TaskHelper.Enqueue(() => ((Telepo)Telepo.Instance()).Teleport(aetheryte.RowID, aetheryte.SubIndex), "读条进行传送", 0, null, null, null, null, 1000);
		TaskHelper.Enqueue(() => GameState.TerritoryType == aetheryte.ZoneID && LocalPlayerState.Object != null && !UIModuleExtension.IsScreenReady(), "等待进入传送状态", 8000, TaskAbortBehaviour.AbortAll, null, null, null, 1000);
		TaskHelper.Enqueue(delegate
		{
			if (UIModuleExtension.IsScreenReady())
			{
				return true;
			}
			TPPlayerAddress(pos);
			return false;
		}, "进行瞬移", 0, null, null, null, null, 1000);
		return true;
	}

	public unsafe Task TPSmoothAsync(Vector3 targetPosition, float speed, CancellationToken? cancellationToken)
	{
		IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
		if (localPlayer == null)
		{
			return Task.CompletedTask;
		}
		CancellationToken valueOrDefault = cancellationToken.GetValueOrDefault();
		if (!cancellationToken.HasValue)
		{
			valueOrDefault = CancellationToken.None;
			cancellationToken = valueOrDefault;
		}
		return DalamudServiceExtension.Instance<IFramework>().Run((Func<Task>)async delegate
		{
			try
			{
				PlayerController.Instance()->MoveControllerWalk.IsMovementInputLocked = true;
				while (localPlayer != null)
				{
					ref CancellationToken? reference = ref cancellationToken;
					if (!reference.HasValue || reference.GetValueOrDefault().IsCancellationRequested)
					{
						break;
					}
					if (PlayerController.Instance()->MoveState == 3)
					{
						PlayerController.Instance()->MoveState = 1;
					}
					bool isArrived;
					Vector3 tPSmoothNextPosition = GetTPSmoothNextPosition(localPlayer, targetPosition, speed, out isArrived);
					ExecuteTPSmoothPosition(localPlayer, tPSmoothNextPosition);
					if (isArrived)
					{
						break;
					}
					await DalamudServiceExtension.Instance<IFramework>().DelayTicks(1L, default(CancellationToken));
				}
			}
			finally
			{
				PlayerController.Instance()->MoveControllerWalk.IsMovementInputLocked = false;
			}
		}, default(CancellationToken));
	}

	public unsafe bool TPSmooth(Vector3 targetPosition, float speed)
	{
		IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
		if (localPlayer == null)
		{
			return false;
		}
		if (PlayerController.Instance()->MoveState == 3)
		{
			PlayerController.Instance()->MoveState = 1;
		}
		bool isArrived;
		Vector3 tPSmoothNextPosition = GetTPSmoothNextPosition(localPlayer, targetPosition, speed, out isArrived);
		if (isArrived)
		{
			return true;
		}
		ExecuteTPSmoothPosition(localPlayer, tPSmoothNextPosition);
		return true;
	}

	public void Dismount(bool restoreControlMode = true)
	{
		IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
		if (localPlayer == null || !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4])
		{
			return;
		}
		MovementControlMode controlMode = CurrentControlMode;
		float rotation = localPlayer.Rotation;
		if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)77])
		{
			if (!RaycastHelper.TryGetGroundPosition(localPlayer.Position, out var groundPosition))
			{
				return;
			}
			TPPlayerAddress(groundPosition);
			if (GameState.ContentFinderCondition == 0)
			{
				new PositionUpdatePacket(rotation, groundPosition, GetMoveType(PositionUpdatePacket.MoveType.Fly0)).Send();
			}
			else
			{
				new PositionUpdateInstancePacket(rotation, groundPosition, GetInstanceMoveType(PositionUpdateInstancePacket.MoveType.Fly0)).Send();
			}
			OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommandComplexLocation(ExecuteCommandComplexFlag.Dismount, groundPosition, RotationHelper.CharaToPacket(rotation), 1u);
		}
		else
		{
			OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.Dismount);
		}
		if (restoreControlMode)
		{
			TaskHelper.Enqueue(() => !DalamudServiceExtension.Instance<ICondition>().Any((ConditionFlag[])(object)new ConditionFlag[2]
			{
				(ConditionFlag)4,
				(ConditionFlag)48
			}), null, 0, null, null, null, null, 1000);
			TaskHelper.Enqueue(delegate
			{
				SetCurrentControlMode(controlMode);
			}, null, 0, null, null, null, null, 1000);
		}
	}

	public PositionUpdatePacket.MoveType GetMoveType(PositionUpdatePacket.MoveType moveType)
	{
		return (PositionUpdatePacket.MoveType)((uint)moveType + (uint)(CurrentZoneMoveState << 16));
	}

	public PositionUpdateInstancePacket.MoveType GetInstanceMoveType(PositionUpdateInstancePacket.MoveType moveType)
	{
		return (PositionUpdateInstancePacket.MoveType)((uint)moveType + (uint)(CurrentZoneMoveState << 16));
	}

	public unsafe static bool TryGetExitRangeInstance(out ExitRangeLayoutInstance* instance, Predicate<nint>? predicate = null)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		instance = null;
		StdMap<InstanceType, Pointer<StdMap<ulong, Pointer<ILayoutInstance>>>>* instancesByType = &((LayoutManager)((LayoutWorld)LayoutWorld.Instance()).ActiveLayout).InstancesByType;
		InstanceType val = (InstanceType)41;
		Pointer<StdMap<ulong, Pointer<ILayoutInstance>>>* ptr = default(Pointer<StdMap<ulong, Pointer<ILayoutInstance>>>*);
		if (!instancesByType->TryGetValuePointer(ref val, ref ptr))
		{
			return false;
		}
		ValueEnumerator<ulong, Pointer<ILayoutInstance>> enumerator = ptr->Value->Values.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Pointer<ILayoutInstance> current = enumerator.Current;
				if (!current.IsNull)
				{
					ExitRangeLayoutInstance* value = (ExitRangeLayoutInstance*)current.Value;
					if (value->ReturnInstance != null && LuminaGetter.TryGetRow<TerritoryType>((uint)value->TerritoryType, out TerritoryType _) && (predicate == null || predicate((nint)value)))
					{
						instance = value;
						return true;
					}
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
		return false;
	}

	public static bool IsOnlyYAxisChanged(Vector3 orig, Vector3 after, float xzTolerance = 2f, float yTolerance = 0.0001f)
	{
		if (Math.Abs(orig.X - after.X) < xzTolerance && Math.Abs(orig.Z - after.Z) < xzTolerance)
		{
			return Math.Abs(orig.Y - after.Y) >= yTolerance;
		}
		return false;
	}

	public static bool? IsNeedEnvironmentChange(Vector3 pos0, Vector3 pos1, uint zone = 0u)
	{
		if (zone == 0)
		{
			zone = GameState.TerritoryType;
		}
		if (zone == 0)
		{
			return null;
		}
		if (!Positions.DivableAreas.TryGetValue(zone, out Polygon[] value))
		{
			return false;
		}
		if ((!(pos0.Y > 0f) || !(pos1.Y < 0f)) && (!(pos0.Y < 0f) || !(pos1.Y > 0f)))
		{
			return false;
		}
		bool num = value.Any((Polygon x) => x.IsPointInPolygon(pos0.ToVector2()));
		bool flag = value.Any((Polygon x) => x.IsPointInPolygon(pos1.ToVector2()));
		return num || flag;
	}

	public unsafe void SetCurrentControlMode(MovementControlMode mode)
	{
		if (CurrentMoveModeInternal != null)
		{
			Marshal.WriteByte((nint)CurrentMoveModeInternal, (byte)mode);
		}
	}

	protected unsafe override void Init()
	{
		config = LoadConfig<Config>() ?? new Config();
		CurrentMoveModeInternal = CurrentMoveModeInternalSig.GetStatic<byte>();
		CurrentZoneMoveStateBaseInstance = CurrentZoneMoveStateBaseInstanceSig.GetStatic<byte>();
		GetCurrentZoneMoveState = GetCurrentZoneMoveStateSig.GetDelegate<GetCurrentZoneMoveStateDelegate>();
		Type[] types;
		try
		{
			types = typeof(EventTransportBase).Assembly.GetTypes();
		}
		catch (ReflectionTypeLoadException)
		{
			return;
		}
		Type[] array = types;
		foreach (Type type in array)
		{
			if (!type.IsAbstract && !type.IsInterface && typeof(EventTransportBase).IsAssignableFrom(type) && !(type.GetConstructor(Type.EmptyTypes) == null) && Activator.CreateInstance(type) is EventTransportBase item)
			{
				EventTransportations.Add(item);
			}
		}
		DLog.Debug($"[MovementManager] 已注册 {EventTransportations.Count} 个区域变更方式");
	}

	protected override void Uninit()
	{
		foreach (EventTransportBase eventTransportation in EventTransportations)
		{
			eventTransportation.Cleanup();
		}
		EventTransportations.Clear();
		TaskHelper = null;
		IsBusyHandler = null;
	}

	private unsafe void ExecuteTPSmoothPosition(IPlayerCharacter localPlayer, Vector3 pos)
	{
		bool flag = DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4] && Control.CanFly;
		if (flag)
		{
			PlayerController.Instance()->MoveControllerFly.MountPosition = pos;
		}
		else
		{
			((BattleChara)localPlayer.ToStruct()).SetPosition(pos.X, pos.Y, pos.Z);
		}
		if (IConditionExtension.get_IsBoundByDuty(DalamudServiceExtension.Instance<ICondition>()))
		{
			PositionUpdateInstancePacket.MoveType instanceMoveType = GetInstanceMoveType(flag ? PositionUpdateInstancePacket.MoveType.Fly0 : PositionUpdateInstancePacket.MoveType.NormalMove0);
			new PositionUpdateInstancePacket(localPlayer.Rotation, pos, instanceMoveType).Send();
		}
		else
		{
			PositionUpdatePacket.MoveType moveType = GetMoveType(flag ? PositionUpdatePacket.MoveType.Fly0 : PositionUpdatePacket.MoveType.NormalMove0);
			new PositionUpdatePacket(localPlayer.Rotation, pos, moveType).Send();
		}
	}

	private static Vector3 GetTPSmoothNextPosition(IPlayerCharacter localPlayer, Vector3 targetPosition, float speed, out bool isArrived)
	{
		Vector3 position = localPlayer.Position;
		Vector3 vector = new Vector3(targetPosition.X, position.Y, targetPosition.Z) - position;
		float num = vector.Length();
		isArrived = num < 0.1f;
		if (isArrived || speed * GameState.DeltaTime >= num)
		{
			return targetPosition;
		}
		Vector3 result = position + vector / num * speed * GameState.DeltaTime;
		result.Y = targetPosition.Y;
		return result;
	}

	public static List<ZoneHop> FindBestPath(uint source, uint target)
	{
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		if (source == target)
		{
			return new List<ZoneHop>();
		}
		PathState pathState = new PathState(source, Vector3.Zero);
		Dictionary<PathState, PathCost> dictionary = new Dictionary<PathState, PathCost>();
		Dictionary<PathState, PathEdge> dictionary2 = new Dictionary<PathState, PathEdge>();
		PriorityQueue<PathState, PathCost> priorityQueue = new PriorityQueue<PathState, PathCost>();
		dictionary[pathState] = new PathCost(0u, 0);
		priorityQueue.Enqueue(pathState, new PathCost(0u, 0));
		PathState? pathState2 = null;
		PathState element;
		PathCost priority;
		while (priorityQueue.TryDequeue(out element, out priority))
		{
			if (!dictionary.TryGetValue(element, out var value) || value.EffectiveCost != priority.EffectiveCost || value.Hops != priority.Hops)
			{
				continue;
			}
			if (element.Zone == target)
			{
				pathState2 = element;
				break;
			}
			foreach (ZoneHop item2 in EnumerateEdges(element.Zone, element.Zone == source))
			{
				Vector3 vector = ((item2 is AetheryteHop aetheryteHop) ? aetheryteHop.Record.Position : ((!(item2 is TelepotTownHop telepotTownHop)) ? Vector3.Zero : telepotTownHop.Target.Position));
				Vector3 pos = vector;
				PathState pathState3 = new PathState(item2.TargetZone, pos);
				uint num = ((!OmenServiceBase<MovementManager>.Instance().AlwaysMinGilTransport && priority.Hops >= 1) ? ((uint)priority.Hops * OmenServiceBase<MovementManager>.Instance().config.HopCumulativePenalty) : 0u);
				uint num2 = ((item2 is AetheryteHop aetheryteHop2) ? TeleportCostCalculator.GetTeleportCost(aetheryteHop2.Record.GetData(), element.Zone, aetheryteHop2.Record.RowID, aetheryteHop2.Record.Group, aetheryteHop2.Record.IsAetheryte, aetheryteHop2.Record.IsHouse) : ((item2 is EventHop eventHop) ? eventHop.Method.Cost : 0u));
				uint num3 = num2 + num;
				int num4 = priority.Hops + 1;
				uint num5 = priority.EffectiveCost + num3;
				if (!dictionary.TryGetValue(pathState3, out var value2) || (value2.EffectiveCost >= num5 && (value2.EffectiveCost != num5 || value2.Hops > num4)))
				{
					dictionary[pathState3] = new PathCost(num5, num4);
					dictionary2[pathState3] = new PathEdge(item2, element, num3);
					priorityQueue.Enqueue(pathState3, new PathCost(num5, num4));
				}
			}
		}
		if (pathState2.HasValue)
		{
			PathState valueOrDefault = pathState2.GetValueOrDefault();
			if (dictionary2.ContainsKey(valueOrDefault))
			{
				uint num2 = TicketUsageType;
				bool flag = ((num2 == 1 || num2 == 4) ? true : false);
				ZoneHop Hop;
				PathState PrevState;
				int num6;
				if (flag && LocalPlayerState.GetItemCount(7569u) != 0)
				{
					AetheryteRecord nearestAetheryte = OmenServiceBase<AetheryteRecordManager>.Instance().GetNearestAetheryte(target, Vector3.Zero);
					if ((object)nearestAetheryte != null)
					{
						uint teleportCost = TeleportCostCalculator.GetTeleportCost(nearestAetheryte.GetData(), source, nearestAetheryte.RowID, nearestAetheryte.Group, nearestAetheryte.IsAetheryte, nearestAetheryte.IsHouse);
						if (TicketUsageType == 1 || teleportCost > TicketUsageGilSetting)
						{
							bool flag2 = false;
							PathState key = valueOrDefault;
							while (key.Zone != source || key.Pos != Vector3.Zero)
							{
								dictionary2[key].Deconstruct(out Hop, out PrevState, out num2);
								ZoneHop zoneHop = Hop;
								PathState pathState4 = PrevState;
								if (zoneHop is AetheryteHop)
								{
									flag2 = true;
									break;
								}
								key = pathState4;
							}
							if (flag2)
							{
								num6 = 1;
								List<ZoneHop> list = new List<ZoneHop>(num6);
								CollectionsMarshal.SetCount(list, num6);
								CollectionsMarshal.AsSpan(list)[0] = new AetheryteHop(nearestAetheryte);
								return list;
							}
						}
					}
				}
				Stack<ZoneHop> stack = new Stack<ZoneHop>();
				PathState key2 = valueOrDefault;
				while (key2.Zone != source || key2.Pos != Vector3.Zero)
				{
					dictionary2[key2].Deconstruct(out Hop, out PrevState, out num2);
					ZoneHop item = Hop;
					PathState pathState5 = PrevState;
					stack.Push(item);
					key2 = pathState5;
				}
				num6 = stack.Count;
				List<ZoneHop> list2 = new List<ZoneHop>(num6);
				CollectionsMarshal.SetCount(list2, num6);
				Span<ZoneHop> span = CollectionsMarshal.AsSpan(list2);
				int num7 = 0;
				{
					foreach (ZoneHop item3 in stack)
					{
						span[num7] = item3;
						num7++;
					}
					return list2;
				}
			}
		}
		return new List<ZoneHop>();
	}

	private unsafe static List<ZoneHop> EnumerateEdges(uint zone, bool isRoot)
	{
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		List<ZoneHop> list = new List<ZoneHop>();
		foreach (ZoneLineExitRange item in LGBAssets.ZoneLineExitRanges.GetValueOrDefault(zone, new List<ZoneLineExitRange>()))
		{
			list.Add(new ExitRangeHop(item.TargetZone, item.Position));
		}
		AetheryteRecord zoneAetheryte = OmenServiceBase<AetheryteRecordManager>.Instance().GetNearestAetheryte(zone, Vector3.Zero);
		if ((object)zoneAetheryte != null && zoneAetheryte.Group > 0)
		{
			uint zoneID = zoneAetheryte.ZoneID;
			TerritoryType rowOrDefault = LuminaGetter.GetRowOrDefault<TerritoryType>(zoneID);
			if (((TerritoryType)(ref rowOrDefault)).TerritoryIntendedUse.RowId == 0 && (!isRoot || (*EventFramework.Instance()).TryGetNearestEventID((EventHandlerInfo x) => (int)x.EventId.ContentId == 5, (GameObject _) => true, Vector3.Zero, out var _)))
			{
				foreach (AetheryteRecord record in OmenServiceBase<AetheryteRecordManager>.Instance().AllRecords)
				{
					if (record.ZoneID != zone && (object)OmenServiceBase<AetheryteRecordManager>.Instance().AllRecords.FirstOrDefault((AetheryteRecord x) => x.Group == zoneAetheryte.Group && x.ZoneID == record.ZoneID) != null)
					{
						list.Add(new TelepotTownHop(record));
					}
				}
			}
		}
		foreach (EventTransportBase eventTransportation in OmenServiceBase<MovementManager>.Instance().EventTransportations)
		{
			foreach (uint item2 in eventTransportation.EnumerateReachableTargets(zone))
			{
				if (GameState.TerritoryType != eventTransportation.SourceZone || eventTransportation.IsEventNearby() || !Sheets.SpeedDetectionZones.ContainsKey(eventTransportation.SourceZone))
				{
					list.Add(new EventHop(eventTransportation, item2));
				}
			}
		}
		foreach (AetheryteRecord allRecord in OmenServiceBase<AetheryteRecordManager>.Instance().AllRecords)
		{
			if (allRecord.ZoneID != zone && AetheryteRecords.AethernetGroups.Contains(allRecord.Group))
			{
				list.Add(new AetheryteHop(allRecord));
			}
		}
		return list;
	}
}
