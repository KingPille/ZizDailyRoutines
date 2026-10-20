using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.LocalData;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Command;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Excel.Sheets;
using Newtonsoft.Json;
using OmenTools;
using OmenTools.Dalamud;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.ImGuiOm.Widgets.MapRenderer;
using OmenTools.Info.Game.AetheryteRecord;
using OmenTools.Info.Lumina;
using OmenTools.Interop.Game.Helpers;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;

namespace DailyRoutines.Modules.AutoMarksFinder;

public class AutoMarksFinder : ModuleBase
{
	private enum NotoriousMonsterRank : byte
	{
		B = 1,
		A,
		S
	}

	private class Config : ModuleConfig
	{
		public int DelayMS = 5000;

		public HashSet<string> RelayCommands = new HashSet<string>();

		public string RelayMacroPattern = "检测到恶名精英 {1} 于 {0} {2}";

		public Dictionary<uint, List<NotoriousMonsterScanPoint>> ScanPoints = new Dictionary<uint, List<NotoriousMonsterScanPoint>>();

		public HashSet<NotoriousMonsterRank> SelectedRanks = new HashSet<NotoriousMonsterRank>();

		public HashSet<uint> SelectedZones = new HashSet<uint>();

		public List<NotoriousMonsterList> Trains = new List<NotoriousMonsterList>();

		public float UIScale = 0.6f;

		public string WaitMacroPattern = "请在 {0} {2} 稍作等候";
	}

	private sealed class NotoriousMonsterFound : IEquatable<NotoriousMonsterFound>
	{
		public static readonly Dictionary<uint, uint> NotoriousMonsters;

		private static readonly Dictionary<uint, Vector3> AetheryteRedirection;

		public uint DataID { get; set; }

		public uint ZoneID { get; set; }

		public uint Instance { get; set; }

		public NotoriousMonsterRank Rank { get; set; }

		[JsonIgnore]
		public Aetheryte? NearestAetheryte => LazyNearestAetheryte.Value;

		[JsonIgnore]
		public Vector3? NearestAetherytePosition => LazyNearestAetherytePosition.Value;

		private Lazy<Aetheryte?> LazyNearestAetheryte => new Lazy<Aetheryte?>(() => OmenServiceBase<AetheryteRecordManager>.Instance().GetNearestAetheryte(ZoneID, (ZoneID == 818) ? new Vector3(573f, 349.1f, -200f) : Position)?.GetData());

		private Lazy<Vector3?> LazyNearestAetherytePosition => new Lazy<Vector3?>(() => OmenServiceBase<AetheryteRecordManager>.Instance().GetNearestAetheryte(ZoneID, (ZoneID == 818) ? new Vector3(573f, 349.1f, -200f) : Position)?.Position);

		public Vector3 Position { get; set; }

		public bool IsAlive { get; set; } = true;

		static NotoriousMonsterFound()
		{
			AetheryteRedirection = new Dictionary<uint, Vector3> { [818u] = new Vector3(573f, 349.1f, -200f) };
			NotoriousMonsters = ((IEnumerable<NotoriousMonster>)LuminaGetter.Get<NotoriousMonster>()).DistinctBy((NotoriousMonster x) => ((NotoriousMonster)(ref x)).BNpcBase.RowId).ToDictionary((NotoriousMonster x) => ((NotoriousMonster)(ref x)).BNpcBase.RowId, (NotoriousMonster x) => ((NotoriousMonster)(ref x)).RowId);
		}

		public NotoriousMonsterFound()
		{
		}

		public NotoriousMonsterFound(uint dataID, Vector3 position, uint zoneID, uint instance)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			DataID = dataID;
			Position = position;
			ZoneID = zoneID;
			Instance = instance;
			NotoriousMonster data = GetData();
			Rank = (NotoriousMonsterRank)((NotoriousMonster)(ref data)).Rank;
		}

		public bool Equals(NotoriousMonsterFound? other)
		{
			if (other == null)
			{
				return false;
			}
			if (this == other)
			{
				return true;
			}
			if (DataID == other.DataID && ZoneID == other.ZoneID)
			{
				return Instance == other.Instance;
			}
			return false;
		}

		public NotoriousMonster GetData()
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			return LuminaGetter.GetRow<NotoriousMonster>(NotoriousMonsters[DataID]).GetValueOrDefault();
		}

		public TerritoryType GetZoneData()
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			return LuminaGetter.GetRow<TerritoryType>(ZoneID).GetValueOrDefault();
		}

		public uint GetMapID()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			TerritoryType zoneData = GetZoneData();
			return ((TerritoryType)(ref zoneData)).Map.RowId;
		}

		public string GetBNPCName()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			NotoriousMonster data = GetData();
			BNpcName value = ((NotoriousMonster)(ref data)).BNpcName.Value;
			return ((object)((BNpcName)(ref value)).Singular/*cast due to constrained. prefix*/).ToString();
		}

		public string GetZoneName()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return GetZoneData().ExtractPlaceName();
		}

		public void SetState(bool isAlive)
		{
			IsAlive = isAlive;
		}

		public override string ToString()
		{
			return $"NotoriousMonsterFound_{DataID}_{Position}";
		}

		public override bool Equals(object? obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (this == obj)
			{
				return true;
			}
			if (obj.GetType() == GetType())
			{
				return Equals((NotoriousMonsterFound)obj);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(DataID, ZoneID, Instance);
		}
	}

	private sealed class NotoriousMonsterList : IEquatable<NotoriousMonsterList>
	{
		public string Name { get; set; }

		public long GeneratedTime { get; set; }

		[JsonIgnore]
		public DateTime? GeneratedDateTime { get; set; }

		public List<NotoriousMonsterFound> MonstersFound { get; set; } = new List<NotoriousMonsterFound>();

		[JsonIgnore]
		public Aetheryte? StartAetheryte { get; set; }

		public NotoriousMonsterList()
		{
		}

		public NotoriousMonsterList(string name)
		{
			Name = name;
			GeneratedTime = Framework.GetServerTime();
		}

		public bool Equals(NotoriousMonsterList? other)
		{
			if (other == null)
			{
				return false;
			}
			if (this == other)
			{
				return true;
			}
			return GeneratedTime == other.GeneratedTime;
		}

		public void Reorder()
		{
			if (MonstersFound.Count <= 0)
			{
				OmenServiceBase<NotifyHelper>.Instance().NotificationWarning(LanguageManager.Get("AutoMarksFinder-CantSortEmptyList"), LanguageManager.Get("AutoMarksFinderTitle"));
				return;
			}
			if (!StartAetheryte.HasValue)
			{
				OmenServiceBase<NotifyHelper>.Instance().NotificationWarning(LanguageManager.Get("AutoMarksFinder-CantSortNullStartingPoint"), LanguageManager.Get("AutoMarksFinderTitle"));
				return;
			}
			List<NotoriousMonsterFound> source = MonstersFound.OrderByDescending(delegate(NotoriousMonsterFound x)
			{
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_002d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_0039: Unknown result type (might be due to invalid IL or missing references)
				//IL_003e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_0047: Unknown result type (might be due to invalid IL or missing references)
				uint zoneID = x.ZoneID;
				Aetheryte? startAetheryte = StartAetheryte;
				uint? num;
				if (!startAetheryte.HasValue)
				{
					num = null;
				}
				else
				{
					Aetheryte valueOrDefault = startAetheryte.GetValueOrDefault();
					Map value = ((Aetheryte)(ref valueOrDefault)).Map.Value;
					num = ((Map)(ref value)).TerritoryType.RowId;
				}
				return zoneID == num;
			}).ThenBy((NotoriousMonsterFound monster) => monster.ZoneID).ThenBy((NotoriousMonsterFound monster) => monster.Instance)
				.ToList();
			List<NotoriousMonsterFound> list = new List<NotoriousMonsterFound>();
			Aetheryte? currentAetheryte = StartAetheryte;
			foreach (IGrouping<(uint, uint), NotoriousMonsterFound> item in from monster in source
				group monster by (Zone: monster.ZoneID, Instance: monster.Instance))
			{
				List<NotoriousMonsterFound> list2 = item.ToList();
				while (list2.Count > 0)
				{
					NotoriousMonsterFound notoriousMonsterFound = list2.OrderBy(delegate(NotoriousMonsterFound monster)
					{
						//IL_001a: Unknown result type (might be due to invalid IL or missing references)
						//IL_001f: Unknown result type (might be due to invalid IL or missing references)
						//IL_0044: Unknown result type (might be due to invalid IL or missing references)
						//IL_0049: Unknown result type (might be due to invalid IL or missing references)
						ref Aetheryte? reference = ref currentAetheryte;
						Vector2 value = (reference.HasValue ? reference.GetValueOrDefault().GetPositionMap() : default(Vector2));
						Aetheryte? nearestAetheryte = monster.NearestAetheryte;
						return Vector2.Distance(value, nearestAetheryte.HasValue ? nearestAetheryte.GetValueOrDefault().GetPositionMap() : default(Vector2));
					}).First();
					list.Add(notoriousMonsterFound);
					currentAetheryte = notoriousMonsterFound.NearestAetheryte ?? currentAetheryte;
					list2.Remove(notoriousMonsterFound);
				}
			}
			MonstersFound = list;
		}

		public override bool Equals(object? obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (this == obj)
			{
				return true;
			}
			if (obj.GetType() == GetType())
			{
				return Equals((NotoriousMonsterList)obj);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return GeneratedTime.GetHashCode();
		}

		public override string ToString()
		{
			return $"NotoriousMonsterList_{Name}_{GeneratedTime}";
		}
	}

	private sealed class NotoriousMonsterScanPoint : IEquatable<NotoriousMonsterScanPoint>
	{
		public uint Zone { get; set; }

		public Vector3 Position { get; set; }

		public bool CancelAnimation { get; set; }

		public bool Equals(NotoriousMonsterScanPoint? other)
		{
			if (other == null)
			{
				return false;
			}
			if (this == other)
			{
				return true;
			}
			if (Zone == other.Zone)
			{
				return Position.Equals(other.Position);
			}
			return false;
		}

		public override string ToString()
		{
			return $"NotoriousMonsterScanPoint_{Zone}_{Position}";
		}

		public override bool Equals(object? obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (this == obj)
			{
				return true;
			}
			if (obj.GetType() == GetType())
			{
				return Equals((NotoriousMonsterScanPoint)obj);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Zone, Position);
		}
	}

	private sealed class ZoneScanProgress
	{
		public uint ZoneID { get; set; }

		public uint Instance { get; set; }

		public int FoundARank { get; set; }

		public int FoundBRank { get; set; }

		public HashSet<uint> ScannedPointIndices { get; set; } = new HashSet<uint>();

		public bool IsCompleted(HashSet<NotoriousMonsterRank> targetRanks, int expectedAmount)
		{
			if (targetRanks.Count == 1)
			{
				if (targetRanks.First() != NotoriousMonsterRank.A)
				{
					return FoundBRank >= expectedAmount;
				}
				return FoundARank >= expectedAmount;
			}
			if (FoundARank >= expectedAmount)
			{
				return FoundBRank >= expectedAmount;
			}
			return false;
		}
	}

	private sealed class PathDecision
	{
		public enum PathType
		{
			DirectNav,
			TeleportThenNav,
			SmartTP
		}

		public PathType Type { get; set; }

		public Vector3 TargetPosition { get; set; }

		public AetheryteRecord? Aetheryte { get; set; }

		public float EstimatedTime { get; set; }
	}

	private sealed class NotoriousMonsterScanner : IDisposable
	{
		private enum ScanState
		{
			Idle,
			Initializing,
			MovingToPoint,
			Scanning,
			SwitchingInstance,
			SwitchingZone,
			Completed
		}

		private const float MOUNT_SPEED = 12f;

		private const float TELEPORT_DURATION = 10f;

		private const float DETECTION_RADIUS = 100f;

		private readonly AutoMarksFinder module;

		private ScanState currentState;

		private uint currentScanZone;

		private uint currentInstance;

		private int currentScanPointIndex;

		private List<NotoriousMonsterScanPoint> currentSortedPoints = new List<NotoriousMonsterScanPoint>();

		private Vector3? currentTargetPoint;

		private long lastMonsterFoundTime;

		private Dictionary<ulong, ZoneScanProgress> zoneScanProgress = new Dictionary<ulong, ZoneScanProgress>();

		public Dictionary<uint, HashSet<NotoriousMonsterFound>> FoundOnes { get; set; } = new Dictionary<uint, HashSet<NotoriousMonsterFound>>();

		public NotoriousMonsterScanner(AutoMarksFinder module, IEnumerable<NotoriousMonsterRank> ranks, IEnumerable<uint> zones)
		{
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Expected O, but got Unknown
			this.module = module;
			module.config.SelectedRanks = ranks.Distinct().ToHashSet();
			module.config.SelectedZones = zones.Distinct().ToHashSet();
			currentState = ScanState.Idle;
			OmenServiceBase<FrameworkManager>.Instance().Reg(new OnUpdateDelegate(OnUpdate), 100u);
		}

		public void Dispose()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Expected O, but got Unknown
			OmenServiceBase<FrameworkManager>.Instance().Unreg(OnUpdate);
		}

		public (uint ZoneID, List<NotoriousMonsterScanPoint> Points, int CurrentIndex) GetCurrentScanState()
		{
			return (ZoneID: currentScanZone, Points: currentSortedPoints, CurrentIndex: currentScanPointIndex);
		}

		public void StartScan()
		{
			currentState = ScanState.Initializing;
			StateCheck();
		}

		private void StateCheck()
		{
			module.TaskHelper.Abort();
			if (CheckPreCondition())
			{
				switch (currentState)
				{
				case ScanState.Initializing:
					HandleInitializing();
					break;
				case ScanState.MovingToPoint:
					HandleMovingToPoint();
					break;
				case ScanState.Scanning:
					HandleScanning();
					break;
				case ScanState.SwitchingInstance:
					HandleSwitchingInstance();
					break;
				case ScanState.SwitchingZone:
					HandleSwitchingZone();
					break;
				case ScanState.Completed:
					module.StopScanner();
					break;
				}
			}
		}

		private void EnqueueCheckState(string name)
		{
			module.TaskHelper.Enqueue((Action)StateCheck, name, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
		}

		private bool CheckPreCondition()
		{
			if (!UIModuleExtension.IsScreenReady() || IOmenDalamudService<IObjectTable>.Instance().LocalPlayer == null)
			{
				EnqueueCheckState("等待屏幕就绪, 进入新循环");
				return false;
			}
			if (!DalamudServiceExtension.Instance().IsPluginEnabled("vnavmesh") || !vnavmeshIPC.GetIsNavReady())
			{
				EnqueueCheckState("等待导航就绪, 进入新循环");
				return false;
			}
			return true;
		}

		private void HandleInitializing()
		{
			uint territoryType = GameState.TerritoryType;
			if (!module.config.SelectedZones.Contains(territoryType))
			{
				currentState = ScanState.SwitchingZone;
				EnqueueCheckState("当前区域不在扫描列表，切换区域");
				return;
			}
			if (!module.config.ScanPoints.TryGetValue(territoryType, out List<NotoriousMonsterScanPoint> value) || value.Count == 0)
			{
				module.config.SelectedZones.Remove(territoryType);
				currentState = ScanState.SwitchingZone;
				EnqueueCheckState("当前区域无扫描点，切换区域");
				return;
			}
			currentScanZone = territoryType;
			currentInstance = InstancesManager.CurrentInstance;
			currentScanPointIndex = 0;
			currentSortedPoints = SortPoints(value);
			currentState = ScanState.MovingToPoint;
			EnqueueCheckState("初始化完成，开始移动");
		}

		private void HandleMovingToPoint()
		{
			if (currentScanPointIndex >= currentSortedPoints.Count)
			{
			}
			if (IsCurrentAreaCompleted(currentScanZone, currentInstance))
			{
				if (InstancesManager.IsInstancedArea)
				{
					currentState = ScanState.SwitchingInstance;
					EnqueueCheckState("当前副本线完成，切换副本线");
				}
				else
				{
					currentState = ScanState.SwitchingZone;
					EnqueueCheckState("当前区域完成，切换区域");
				}
				return;
			}
			NotoriousMonsterScanPoint notoriousMonsterScanPoint = currentSortedPoints[currentScanPointIndex];
			currentTargetPoint = notoriousMonsterScanPoint.Position;
			currentState = ScanState.Scanning;
			PathDecision pathDecision = DecideOptimalPath(notoriousMonsterScanPoint.Position, currentScanZone);
			switch (pathDecision.Type)
			{
			case PathDecision.PathType.TeleportThenNav:
				EnqueueTeleportRoute(pathDecision);
				break;
			case PathDecision.PathType.DirectNav:
				EnqueueDirectRoute(pathDecision);
				break;
			case PathDecision.PathType.SmartTP:
				EnqueueSmartTPRoute(pathDecision);
				break;
			}
			if (currentState == ScanState.Scanning)
			{
				EnqueueCheckState("开始扫描");
			}
		}

		private void HandleScanning()
		{
			if (!currentTargetPoint.HasValue)
			{
				MoveToNextPoint("无目标点");
				return;
			}
			if (LocalPlayerState.DistanceTo2D(currentTargetPoint.Value.ToVector2()) <= 100f)
			{
				module.TaskHelper.DelayNext(500, "等待怪物加载");
				module.TaskHelper.Enqueue(delegate
				{
					vnavmeshIPC.StopPathfind();
					module.TaskHelper.Abort();
					MoveToNextPoint("已进入检测范围");
					return true;
				}, "进入检测范围");
				return;
			}
			module.TaskHelper.Enqueue(delegate
			{
				if (!DalamudServiceExtension.Instance<ICondition>().Any((ConditionFlag[])(object)new ConditionFlag[2]
				{
					(ConditionFlag)45,
					(ConditionFlag)48
				}) && LocalPlayerState.DistanceTo2D(currentTargetPoint.Value.ToVector2()) <= 100f)
				{
					vnavmeshIPC.StopPathfind();
					module.TaskHelper.Abort();
					MoveToNextPoint("移动中进入检测范围");
					return true;
				}
				return false;
			}, "等待进入检测范围");
		}

		private void MoveToNextPoint(string reason)
		{
			currentScanPointIndex++;
			currentState = ((currentScanPointIndex >= currentSortedPoints.Count) ? ScanState.MovingToPoint : ScanState.MovingToPoint);
			EnqueueCheckState(reason + "，跳转下一点");
		}

		private unsafe void HandleSwitchingInstance()
		{
			uint currentZone = GameState.TerritoryType;
			uint num = InstancesManager.CurrentInstance;
			int instancesCount = OmenServiceBase<InstancesManager>.Instance().GetInstancesCount(currentZone);
			uint? nextInstance = FindNextIncompleteInstance(currentZone, num, instancesCount);
			if (nextInstance.HasValue)
			{
				vnavmeshIPC.StopPathfind();
				module.TaskHelper.Abort();
				if (!(*EventFramework.Instance()).TryGetNearestEventID((EventHandlerInfo x) => (int)x.EventId.ContentId == 5, delegate(GameObject x)
				{
					//IL_0008: Unknown result type (might be due to invalid IL or missing references)
					//IL_000d: Unknown result type (might be due to invalid IL or missing references)
					//IL_0010: Unknown result type (might be due to invalid IL or missing references)
					//IL_0015: Unknown result type (might be due to invalid IL or missing references)
					string nameString = ((GameObject)(ref x)).NameString;
					Aetheryte rowOrDefault = LuminaGetter.GetRowOrDefault<Aetheryte>(0u);
					return nameString.Equals(((object)((Aetheryte)(ref rowOrDefault)).Singular/*cast due to constrained. prefix*/).ToString(), StringComparison.OrdinalIgnoreCase);
				}, default(Vector3), out var _))
				{
					module.TaskHelper.Enqueue((Func<bool>)IsAbleToTeleport, "等待停止移动", 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
					module.TaskHelper.Enqueue(() => OmenServiceBase<AetheryteRecordManager>.Instance().GetNearestAetheryte(currentZone, Vector3.Zero)?.TeleportTo(), "传送至以太之光");
					module.TaskHelper.Enqueue(() => UIModuleExtension.IsScreenReady() && (*EventFramework.Instance()).TryGetNearestEventID((EventHandlerInfo x) => (int)x.EventId.ContentId == 5, delegate(GameObject x)
					{
						//IL_0008: Unknown result type (might be due to invalid IL or missing references)
						//IL_000d: Unknown result type (might be due to invalid IL or missing references)
						//IL_0010: Unknown result type (might be due to invalid IL or missing references)
						//IL_0015: Unknown result type (might be due to invalid IL or missing references)
						string nameString = ((GameObject)(ref x)).NameString;
						Aetheryte rowOrDefault = LuminaGetter.GetRowOrDefault<Aetheryte>(0u);
						return nameString.Equals(((object)((Aetheryte)(ref rowOrDefault)).Singular/*cast due to constrained. prefix*/).ToString(), StringComparison.OrdinalIgnoreCase);
					}, default(Vector3), out var _), "等待靠近以太之光");
				}
				module.TaskHelper.Enqueue(delegate
				{
					OmenServiceBase<ChatManager>.Instance().SendMessage($"/pdr insc {nextInstance.Value}");
				}, $"切换至 {nextInstance.Value} 线");
				module.TaskHelper.Enqueue(() => UIModuleExtension.IsScreenReady() && InstancesManager.CurrentInstance == nextInstance.Value, "等待切换完毕");
				currentInstance = nextInstance.Value;
				currentScanPointIndex = 0;
				currentState = ScanState.Initializing;
				EnqueueCheckState("副本线切换完成，重新开始扫描");
			}
			else
			{
				currentState = ScanState.SwitchingZone;
				EnqueueCheckState("当前区域所有副本线完成");
			}
		}

		private void HandleSwitchingZone()
		{
			uint territoryType = GameState.TerritoryType;
			if (module.config.SelectedZones.Contains(territoryType))
			{
				vnavmeshIPC.StopPathfind();
				module.TaskHelper.Abort();
				module.config.SelectedZones.Remove(territoryType);
			}
			uint targetZone = module.config.SelectedZones.FirstOrDefault();
			if (targetZone == 0)
			{
				currentState = ScanState.Completed;
				EnqueueCheckState("所有区域扫描完成");
				return;
			}
			if (!module.config.ScanPoints.TryGetValue(targetZone, out List<NotoriousMonsterScanPoint> value) || value.Count == 0)
			{
				module.config.SelectedZones.Remove(targetZone);
				EnqueueCheckState("目标区域无扫描点，移除并重新检查");
				return;
			}
			vnavmeshIPC.StopPathfind();
			module.TaskHelper.Enqueue((Func<bool>)IsAbleToTeleport, "等待停止移动", 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
			module.TaskHelper.Enqueue(() => OmenServiceBase<MovementManager>.Instance().TPSmart_BetweenZone(targetZone), $"传送至区域 {targetZone}");
			module.TaskHelper.Enqueue(() => GameState.TerritoryType == targetZone && UIModuleExtension.IsScreenReady(), "等待传送完毕");
			module.TaskHelper.Enqueue(delegate
			{
				if (InstancesManager.IsInstancedArea && InstancesManager.CurrentInstance != 1)
				{
					module.TaskHelper.Enqueue(delegate
					{
						OmenServiceBase<ChatManager>.Instance().SendMessage("/pdr insc 1");
					}, "切换至 1 线", 0, null, null, null, null, 1);
					module.TaskHelper.Enqueue(() => UIModuleExtension.IsScreenReady() && InstancesManager.CurrentInstance == 1, "等待切换完毕", 0, null, null, null, null, 1);
				}
			}, "检查副本区切换");
			currentState = ScanState.Initializing;
			EnqueueCheckState("传送完毕，开始扫描");
		}

		private bool IsCurrentAreaCompleted(uint zoneID, uint instance)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			uint instance2 = instance;
			TerritoryType territoryTypeData = GameState.TerritoryTypeData;
			int expectedAmount = ((((TerritoryType)(ref territoryTypeData)).ExVersion.RowId == 0) ? 1 : 2);
			ulong progressKey = GetProgressKey(zoneID, instance2);
			if (!zoneScanProgress.TryGetValue(progressKey, out ZoneScanProgress value))
			{
				value = new ZoneScanProgress
				{
					ZoneID = zoneID,
					Instance = instance2
				};
				zoneScanProgress[progressKey] = value;
			}
			if (FoundOnes.TryGetValue(zoneID, out HashSet<NotoriousMonsterFound> value2))
			{
				value.FoundARank = value2.Count((NotoriousMonsterFound m) => m.Rank == NotoriousMonsterRank.A && m.Instance == instance2);
				value.FoundBRank = value2.Count((NotoriousMonsterFound m) => m.Rank == NotoriousMonsterRank.B && m.Instance == instance2);
			}
			int num = (value.IsCompleted(module.config.SelectedRanks, expectedAmount) ? 1 : 0);
			if (zoneID == currentScanZone && instance == currentInstance)
			{
				bool flag = currentScanPointIndex >= currentSortedPoints.Count;
				num |= (flag ? 1 : 0);
			}
			return HuntSupport.RememberArea((byte)num != 0, this, zoneID, instance);
		}

		private uint? FindNextIncompleteInstance(uint zoneID, uint currentInstance, int totalInstances)
		{
			for (uint num = 1u; num <= totalInstances; num++)
			{
				if (!IsCurrentAreaCompleted(zoneID, num))
				{
					return num;
				}
			}
			return null;
		}

		private static ulong GetProgressKey(uint zoneID, uint instance)
		{
			return ((ulong)zoneID << 32) | instance;
		}

		private void OnUpdate(IFramework _)
		{
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Invalid comparison between Unknown and I4
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			uint territoryType = GameState.TerritoryType;
			uint instance = InstancesManager.CurrentInstance;
			if (!module.config.SelectedZones.Contains(territoryType) || !module.config.ScanPoints.TryGetValue(territoryType, out List<NotoriousMonsterScanPoint> _))
			{
				return;
			}
			bool flag = false;
			foreach (IGameObject item3 in IOmenDalamudService<IObjectTable>.Instance())
			{
				if (item3 != null && (int)item3.ObjectKind == 2 && !item3.IsDead && NotoriousMonsterFound.NotoriousMonsters.TryGetValue(item3.DataID, out var value2) && LuminaGetter.TryGetRow<NotoriousMonster>(value2, out NotoriousMonster item) && module.config.SelectedRanks.Contains((NotoriousMonsterRank)((NotoriousMonster)(ref item)).Rank))
				{
					FoundOnes.TryAdd(territoryType, new HashSet<NotoriousMonsterFound>());
					NotoriousMonsterFound item2 = new NotoriousMonsterFound(item3.DataID, item3.Position, territoryType, instance);
					if (FoundOnes[territoryType].Add(item2))
					{
						flag = true;
						lastMonsterFoundTime = OmenServiceBase<StandardTimeManager>.Instance().UTCNowOffset.ToUnixTimeMilliseconds();
						Vector2 vector = PositionHelper.WorldToMap(item3.Position.ToVector2(), GameState.MapData);
						NotifyHelper notifyHelper = OmenServiceBase<NotifyHelper>.Instance();
						object[] obj = new object[3]
						{
							(NotoriousMonsterRank)((NotoriousMonster)(ref item)).Rank,
							null,
							null
						};
						BNpcName value3 = ((NotoriousMonster)(ref item)).BNpcName.Value;
						obj[1] = ((object)((BNpcName)(ref value3)).Singular/*cast due to constrained. prefix*/).ToString();
						obj[2] = SeString.CreateMapLink(territoryType, GameState.Map, vector.X, vector.Y, 0.05f);
						notifyHelper.Chat(LanguageManager.GetSe("AutoMarksFinder-DetectPowerfulMark", obj));
					}
				}
			}
			if (flag && module.TaskHelper.IsBusy && currentScanZone == territoryType)
			{
				if (IsCurrentAreaCompleted(territoryType, instance))
				{
					vnavmeshIPC.StopPathfind();
					module.TaskHelper.Abort();
					if (InstancesManager.IsInstancedArea)
					{
						currentState = ScanState.SwitchingInstance;
						EnqueueCheckState("发现足够怪物，切换副本线");
					}
					else
					{
						currentState = ScanState.SwitchingZone;
						EnqueueCheckState("发现足够怪物，切换区域");
					}
				}
				else
				{
					vnavmeshIPC.StopPathfind();
					module.TaskHelper.Abort();
					MoveToNextPoint("发现新怪物");
				}
			}
			if (!module.config.SelectedZones.Any() && !module.TaskHelper.IsBusy)
			{
				module.StopScanner();
			}
		}

		private static PathDecision DecideOptimalPath(Vector3 targetPoint, uint zoneID)
		{
			float num = Vector3.Distance(LocalPlayerState.Object?.Position ?? Vector3.Zero, targetPoint);
			if (num <= 100f)
			{
				return new PathDecision
				{
					Type = PathDecision.PathType.DirectNav,
					TargetPosition = targetPoint,
					EstimatedTime = num / 12f
				};
			}
			AetheryteRecord nearestAetheryte = OmenServiceBase<AetheryteRecordManager>.Instance().GetNearestAetheryte(zoneID, targetPoint);
			if (nearestAetheryte == null)
			{
				return new PathDecision
				{
					Type = PathDecision.PathType.DirectNav,
					TargetPosition = targetPoint,
					EstimatedTime = num / 12f
				};
			}
			float num2 = Vector3.Distance(vnavmeshIPC.QueryNearestPointOnMesh(nearestAetheryte.Position, 15f, 10000f) ?? nearestAetheryte.Position, targetPoint);
			float num3 = num / 12f;
			float num4 = 10f + num2 / 12f;
			if (num4 < num3)
			{
				return new PathDecision
				{
					Type = PathDecision.PathType.TeleportThenNav,
					TargetPosition = targetPoint,
					Aetheryte = nearestAetheryte,
					EstimatedTime = num4
				};
			}
			return new PathDecision
			{
				Type = PathDecision.PathType.DirectNav,
				TargetPosition = targetPoint,
				EstimatedTime = num3
			};
		}

		private void EnqueueTeleportRoute(PathDecision decision)
		{
			module.TaskHelper.Enqueue((Func<bool>)IsAbleToTeleport, "等待停止移动", 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
			module.TaskHelper.Enqueue(() => OmenServiceBase<AetheryteRecordManager>.Instance().GetNearestAetheryte(currentScanZone, decision.TargetPosition)?.TeleportTo(), $"传送至距离 {decision.TargetPosition} 最近的以太之光");
			Vector3 aetherytePos = vnavmeshIPC.QueryNearestPointOnMesh(decision.Aetheryte.Position, 15f, 10000f) ?? decision.Aetheryte.Position;
			module.TaskHelper.Enqueue(() => UIModuleExtension.IsScreenReady() && LocalPlayerState.DistanceTo2D(aetherytePos.ToVector2()) <= 60f, "等待以太之光传送完毕");
			EnqueueMount(module.TaskHelper);
			EnqueuePathfind(decision.TargetPosition);
		}

		private void EnqueueDirectRoute(PathDecision decision)
		{
			module.TaskHelper.Enqueue((Func<bool>)IsAbleToTeleport, "等待停止移动", 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
			Vector3? vector = vnavmeshIPC.QueryNearestPointOnMesh(decision.TargetPosition, 10f, 10000f);
			if (vector.HasValue)
			{
				Vector3 valueOrDefault = vector.GetValueOrDefault();
				EnqueueMount(module.TaskHelper);
				EnqueuePathfind(valueOrDefault);
			}
			else
			{
				MoveToNextPoint("跳过无效扫描点");
			}
		}

		private void EnqueueSmartTPRoute(PathDecision decision)
		{
			EnqueueMount(module.TaskHelper);
			module.TaskHelper.Enqueue(() => OmenServiceBase<MovementManager>.Instance().TPSmart_InZone(decision.TargetPosition), $"TP 至: {decision.TargetPosition}");
			module.TaskHelper.DelayNext(module.config.DelayMS);
		}

		private void EnqueuePathfind(Vector3 targetPos)
		{
			module.TaskHelper.EnqueueAsync(async delegate(CancellationToken ct)
			{
				bool isOnMount = DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4];
				CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(ct);
				List<Vector3> list = await vnavmeshIPC.PathfindCancelable(LocalPlayerState.Object?.Position ?? Vector3.Zero, targetPos, isOnMount, cancellationTokenSource.Token);
				if (list.Count == 0)
				{
					vnavmeshIPC.StopPathfind();
					module.TaskHelper.Abort();
					MoveToNextPoint("寻路失败");
				}
				else if (Vector2.DistanceSquared(list.Last().ToVector2(), targetPos.ToVector2()) > 625f)
				{
					vnavmeshIPC.StopPathfind();
					module.TaskHelper.Abort();
					MoveToNextPoint("路径无法到达");
				}
				else
				{
					vnavmeshIPC.SetPathfindTolerance(2f);
					vnavmeshIPC.PathfindWithPath(list, isOnMount);
				}
			}, $"移动至: {targetPos}", 30000, TaskAbortBehaviour.AbortAll, null, delegate
			{
				vnavmeshIPC.StopPathfind();
				module.TaskHelper.Abort();
				MoveToNextPoint("寻路超时");
			});
		}

		private static bool IsAbleToTeleport()
		{
			if (!OmenServiceBase<LocalPlayerState>.Instance().IsMoving && !DalamudServiceExtension.Instance<ICondition>().Any((ConditionFlag[])(object)new ConditionFlag[1] { (ConditionFlag)27 }) && !IConditionExtension.get_IsBetweenAreas(DalamudServiceExtension.Instance<ICondition>()))
			{
				return UIModuleExtension.IsScreenReady();
			}
			return false;
		}

		private static void EnqueueMount(TaskHelper taskHelper)
		{
			taskHelper.Enqueue(delegate
			{
				if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4])
				{
					return true;
				}
				if (!Throttler.Shared.Throttle("AutoMarksFinder-Mount", 1000u))
				{
					return false;
				}
				if (IConditionExtension.get_IsCasting(DalamudServiceExtension.Instance<ICondition>()))
				{
					return false;
				}
				OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)5, 9u, 3758096384uL, 0u, (UseActionMode)0);
				return false;
			}, "上坐骑");
		}

		private static List<NotoriousMonsterScanPoint> SortPoints(List<NotoriousMonsterScanPoint> points)
		{
			if (points.Count == 0)
			{
				return new List<NotoriousMonsterScanPoint>();
			}
			List<NotoriousMonsterScanPoint> list = new List<NotoriousMonsterScanPoint>();
			List<NotoriousMonsterScanPoint> list2 = new List<NotoriousMonsterScanPoint>(points);
			Vector3 current = LocalPlayerState.Object?.Position ?? Vector3.Zero;
			while (list2.Count > 0)
			{
				NotoriousMonsterScanPoint notoriousMonsterScanPoint = list2.OrderBy((NotoriousMonsterScanPoint p) => Vector3.DistanceSquared(current, p.Position)).First();
				list.Add(notoriousMonsterScanPoint);
				list2.Remove(notoriousMonsterScanPoint);
				current = notoriousMonsterScanPoint.Position;
			}
			for (int num = 0; num < 500; num++)
			{
				bool flag = false;
				for (int num2 = 0; num2 < list.Count - 1; num2++)
				{
					for (int num3 = num2 + 1; num3 < list.Count; num3++)
					{
						float num4 = Vector3.Distance(list[num2].Position, list[num2 + 1].Position) + Vector3.Distance(list[num3].Position, list[(num3 + 1) % list.Count].Position);
						if (Vector3.Distance(list[num2].Position, list[num3].Position) + Vector3.Distance(list[num2 + 1].Position, list[(num3 + 1) % list.Count].Position) < num4)
						{
							list.Reverse(num2 + 1, num3 - num2);
							flag = true;
						}
					}
				}
				if (!flag)
				{
					break;
				}
			}
			return list;
		}
	}

	private class CustomOverlay : Window, IDisposable
	{
		private const ImGuiWindowFlags WINDOW_FLAGS = (ImGuiWindowFlags)73;

		private readonly Action customDraw;

		public CustomOverlay(Action customDraw, string? title = null)
			: base((string.IsNullOrEmpty(title) ? string.Empty : title) ?? "")
		{
			((Window)this).Flags = (ImGuiWindowFlags)73;
			((Window)this).RespectCloseHotkey = false;
			this.customDraw = customDraw;
			OmenServiceBase<WindowManager>.Instance().AddWindow((Window)(object)this);
		}

		public void Dispose()
		{
			OmenServiceBase<WindowManager>.Instance().RemoveWindow((Window?)(object)this);
		}

		public override void Draw()
		{
			StyleDisposable val = ImRaii.PushStyle((ImGuiStyleVar)11, 10f, true);
			try
			{
				using (OmenServiceBase<FontManager>.Instance().UIFont.Push())
				{
					customDraw();
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
	}

	private Config config;

	private CustomOverlay? overlaySmallCard;

	private NotoriousMonsterScanner? scanner;

	private int smallCardMobIndex;

	private int selectedTrainIndex;

	private string newListNameInput = string.Empty;

	private string startAetheryteInput = string.Empty;

	private const bool IS_SHOW_DEBUG = false;

	private const string COMMAND = "/pdrmf";

	private static readonly FrozenSet<string> SupportedRelayCommands;

	private readonly ImGuiMapRenderer scanRouteMapRenderer = new ImGuiMapRenderer
	{
		Zoomable = true,
		Pannable = true,
		EnableResizeGrip = false,
		EnableDefaultMarkers = true
	};

	private Task? spawnPointUpdateTask;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoMarksFinderTitle"),
		Description = LanguageManager.Get("AutoMarksFinderDescription"),
		Category = ModuleCategory.Script,
		ModulesPair = new string[3] { "NoUIFade", "NoFallDamage", "InstantTeleport" },
		ModulesPrerequisite = new string[1] { "FastInstanceZoneChange" },
		Author = new string[2] { "ziz", "KirisameVanilla" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		NeedAuth = true
	};

	protected override void Init()
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 180000,
				ShowDebug = false
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		overlaySmallCard = new CustomOverlay(DrawSmallCard, LanguageManager.Get("AutoMarksFinder-SmallCardTitle"));
		CustomOverlay? customOverlay = overlaySmallCard;
		((Window)customOverlay).Flags = (ImGuiWindowFlags)(((Window)customOverlay).Flags & -2);
		CustomOverlay? customOverlay2 = overlaySmallCard;
		((Window)customOverlay2).Flags = (ImGuiWindowFlags)(((Window)customOverlay2).Flags & -65);
		OmenServiceBase<CommandManager>.Instance().AddCommand("/pdrmf", new CommandInfo(new HandlerDelegate(OnCommand))
		{
			HelpMessage = LanguageManager.Get("AutoMarksFinder-HelpMessage")
		});
		OmenServiceBase<WindowManager>.Instance().PostDraw += DrawScanRoutePreview;
	}

	protected override void Uninit()
	{
		OmenServiceBase<WindowManager>.Instance().PostDraw -= DrawScanRoutePreview;
		if (config != null)
		{
			config.Save(this);
		}
		ClearScanner();
		overlaySmallCard?.Dispose();
		overlaySmallCard = null;
		OmenServiceBase<WindowManager>.Instance().RemoveWindow((Window?)(object)overlaySmallCard);
		OmenServiceBase<CommandManager>.Instance().RemoveCommand("/pdrmf");
		selectedTrainIndex = 0;
		smallCardMobIndex = 0;
		newListNameInput = string.Empty;
		startAetheryteInput = string.Empty;
	}

	private void OnCommand(string command, string args)
	{
		CustomOverlay? customOverlay = overlaySmallCard;
		((Window)customOverlay).IsOpen = !((Window)customOverlay).IsOpen;
	}

	private void StopScanner()
	{
		vnavmeshIPC.StopPathfind();
		base.TaskHelper.Abort();
		scanner?.Dispose();
	}

	private void ClearScanner()
	{
		StopScanner();
		scanner = null;
	}

	private bool DrawTrainsSelectCombo(float width, ref int selectedIndex)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		NotoriousMonsterList notoriousMonsterList = ((selectedIndex == -1 || selectedIndex > config.Trains.Count - 1) ? null : config.Trains[selectedIndex]);
		ImGui.SetNextItemWidth(width);
		ComboDisposable val = ImRaii.Combo(ImU8String.op_Implicit("###TrainsSelectCombo"), ImU8String.op_Implicit((notoriousMonsterList == null) ? ("(" + LanguageManager.Get("None") + ")") : notoriousMonsterList.Name));
		try
		{
			if (!val)
			{
				return result;
			}
			ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
			if (ImGui.InputTextWithHint(ImU8String.op_Implicit("###AddNewTrainList"), ImU8String.op_Implicit(LanguageManager.Get("AutoMarksFinder-NewTrainListTip")), ref newListNameInput, 128, (ImGuiInputTextFlags)32, (ImGuiInputTextCallbackDelegate)null) && !string.IsNullOrWhiteSpace(newListNameInput))
			{
				config.Trains.Add(new NotoriousMonsterList(newListNameInput));
				config.Trains = config.Trains.OrderByDescending((NotoriousMonsterList x) => x.GeneratedTime).ToList();
				config.Save(ManagerBase<ModuleManager>.Instance().GetModule<AutoMarksFinder>());
			}
			ImGui.Separator();
			ImGui.Spacing();
			for (int num = 0; num < config.Trains.Count; num++)
			{
				NotoriousMonsterList notoriousMonsterList2 = config.Trains[num];
				NotoriousMonsterList notoriousMonsterList3 = notoriousMonsterList2;
				DateTime? generatedDateTime = notoriousMonsterList3.GeneratedDateTime;
				DateTime valueOrDefault = generatedDateTime.GetValueOrDefault();
				if (!generatedDateTime.HasValue)
				{
					valueOrDefault = notoriousMonsterList2.GeneratedTime.ToUTCDateTimeFromUnixSeconds().ToLocalTime();
					DateTime? dateTime = (notoriousMonsterList3.GeneratedDateTime = valueOrDefault);
				}
				ImU8String val2 = new ImU8String(0, 1);
				((ImU8String)(ref val2)).AppendFormatted<string>(notoriousMonsterList2.Name);
				if (ImGui.Selectable(val2, notoriousMonsterList?.Equals(notoriousMonsterList2) ?? false, (ImGuiSelectableFlags)0, default(Vector2)))
				{
					selectedIndex = num;
					result = true;
				}
				ImGuiOm.TooltipHover(LanguageManager.Get("AutoMarksFinder-CreationTime", notoriousMonsterList2.GeneratedDateTime));
				PopupDisposable val3 = ImRaii.ContextPopupItem(ImU8String.op_Implicit(notoriousMonsterList2.ToString()));
				try
				{
					if (PopupDisposable.op_Implicit(val3) && ImGui.MenuItem(ImU8String.op_Implicit(LanguageManager.Get("Delete")), false, true))
					{
						config.Trains.RemoveAt(num);
					}
				}
				finally
				{
					((PopupDisposable)(ref val3)).Dispose();
				}
			}
			return result;
		}
		finally
		{
			((ComboDisposable)(ref val)).Dispose();
		}
	}

	private static string FormatPattern(string pattern, params object[] args)
	{
		return HuntSupport.FormatMacro(pattern, args[0], args[1], args[2]);
	}

	private static List<NotoriousMonsterScanPoint> SortPoints(List<NotoriousMonsterScanPoint> points)
	{
		if (points == null || points.Count <= 0)
		{
			return new List<NotoriousMonsterScanPoint>();
		}
		List<NotoriousMonsterScanPoint> list = new List<NotoriousMonsterScanPoint>(points.Count);
		List<NotoriousMonsterScanPoint> list2 = new List<NotoriousMonsterScanPoint>(points);
		Vector3 position = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer.Position;
		while (list2.Count > 0)
		{
			int index = 0;
			float num = Vector3.Distance(position, list2[0].Position);
			for (int i = 1; i < list2.Count; i++)
			{
				float num2 = Vector3.Distance(position, list2[i].Position);
				if (num2 < num)
				{
					num = num2;
					index = i;
				}
			}
			list.Add(list2[index]);
			position = list2[index].Position;
			list2.RemoveAt(index);
		}
		int num3 = 0;
		bool flag;
		do
		{
			flag = false;
			for (int j = 0; j < list.Count - 1; j++)
			{
				for (int k = j + 1; k < list.Count; k++)
				{
					float num4 = Vector3.Distance(list[j].Position, list[j + 1].Position) + ((k + 1 < list.Count) ? Vector3.Distance(list[k].Position, list[k + 1].Position) : 0f);
					if (Vector3.Distance(list[j].Position, list[k].Position) + ((k + 1 < list.Count) ? Vector3.Distance(list[j + 1].Position, list[k + 1].Position) : 0f) < num4)
					{
						int num5 = j + 1;
						int num6 = k;
						while (num5 < num6)
						{
							List<NotoriousMonsterScanPoint> list3 = list;
							int index2 = num5;
							int index3 = num6;
							NotoriousMonsterScanPoint value = list[num6];
							NotoriousMonsterScanPoint value2 = list[num5];
							list3[index2] = value;
							list[index3] = value2;
							num5++;
							num6--;
						}
						flag = true;
					}
				}
			}
			num3++;
		}
		while (flag && num3 < 500);
		return list;
	}

	private static void EnqueueMount(TaskHelper taskHelper)
	{
		taskHelper.Enqueue(delegate
		{
			if (!Throttler.Shared.Throttle("AutoMarksFinder-UseMount") || DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)27])
			{
				return false;
			}
			if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4])
			{
				return true;
			}
			OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)13, 1u, 3758096384uL, 0u, (UseActionMode)0);
			return false;
		}, "上坐骑");
	}

	private static bool IsAbleToTeleport()
	{
		if (!OmenServiceBase<LocalPlayerState>.Instance().IsMoving && !DalamudServiceExtension.Instance<ICondition>().Any((ConditionFlag[])(object)new ConditionFlag[1] { (ConditionFlag)27 }) && !IConditionExtension.get_IsBetweenAreas(DalamudServiceExtension.Instance<ICondition>()))
		{
			return UIModuleExtension.IsScreenReady();
		}
		return false;
	}

	protected override void ConfigUI()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(0, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("Command"));
		ImGui.TextColored(ref vector, val);
		IndentDisposable val2 = ImRaii.PushIndent(1, true);
		try
		{
			ImU8String val3 = new ImU8String(3, 2);
			((ImU8String)(ref val3)).AppendFormatted<string>("/pdrmf");
			((ImU8String)(ref val3)).AppendLiteral(" → ");
			((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get("AutoMarksFinder-HelpMessage"));
			ImGui.TextUnformatted(val3);
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
		ImGui.Spacing();
		MarksFinderUI();
	}

	public void MarksFinderUI()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("PreCondition")));
		IndentDisposable val = ImRaii.PushIndent(1, true);
		try
		{
			ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("IPC-InstalledAndEnabledPlugin", "vnavmesh")));
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		ImGui.NewLine();
		TabBarDisposable val2 = ImRaii.TabBar(ImU8String.op_Implicit("###TabBar"));
		try
		{
			if (!(!val2))
			{
				DrawTabItemTrain();
				DrawTabItemScanner();
				DrawTabItemSettings();
			}
		}
		finally
		{
			((TabBarDisposable)(ref val2)).Dispose();
		}
	}

	public void DrawSmallCard()
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		//IL_0445: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_0472: Unknown result type (might be due to invalid IL or missing references)
		//IL_0477: Unknown result type (might be due to invalid IL or missing references)
		//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0524: Unknown result type (might be due to invalid IL or missing references)
		//IL_0529: Unknown result type (might be due to invalid IL or missing references)
		//IL_0539: Unknown result type (might be due to invalid IL or missing references)
		//IL_0556: Unknown result type (might be due to invalid IL or missing references)
		//IL_055b: Unknown result type (might be due to invalid IL or missing references)
		//IL_058e: Unknown result type (might be due to invalid IL or missing references)
		using (OmenServiceBase<FontManager>.Instance().GetUIFont(config.UIScale).Push())
		{
			NotoriousMonsterList notoriousMonsterList = ((selectedTrainIndex == -1 || selectedTrainIndex > config.Trains.Count - 1) ? null : config.Trains[selectedTrainIndex]);
			ImGui.AlignTextToFramePadding();
			Vector4 vector = KnownColor.Chocolate.ToVector4();
			ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("AutoMarksFinder-SelectRoutes")));
			ImGui.SameLine();
			if (DrawTrainsSelectCombo(200f * Globals.GlobalUIScale, ref selectedTrainIndex))
			{
				smallCardMobIndex = 0;
			}
			ImGui.SameLine();
			ImGui.TextDisabled(ImU8String.op_Implicit("|"));
			DisabledDisposable val = ImRaii.Disabled(notoriousMonsterList == null);
			try
			{
				ImGui.SameLine();
				if (ImGuiOm.ButtonIcon("导出", (FontAwesomeIcon)62830, LanguageManager.Get("Export")) && notoriousMonsterList != null)
				{
					ModuleBase.ExportToClipboard(notoriousMonsterList);
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
			ImGui.SameLine();
			if (ImGuiOm.ButtonIcon("导入", (FontAwesomeIcon)62831, LanguageManager.Get("Import")))
			{
				NotoriousMonsterList notoriousMonsterList2 = ModuleBase.ImportFromClipboard<NotoriousMonsterList>();
				if (notoriousMonsterList2 != null)
				{
					config.Trains.Add(notoriousMonsterList2);
					config.Save(this);
				}
			}
			ImGui.SameLine();
			ImGui.TextDisabled(ImU8String.op_Implicit("|"));
			ImGui.SameLine();
			if (ImGuiOm.ButtonIcon("设置", (FontAwesomeIcon)61459, LanguageManager.Get("Settings")))
			{
				OmenServiceBase<ChatManager>.Instance().SendMessage("/pdr search " + GetType().Name);
			}
			ImGui.Spacing();
			if (notoriousMonsterList == null)
			{
				return;
			}
			List<NotoriousMonsterFound> list = notoriousMonsterList.MonstersFound.ToList();
			if (list.Count == 0)
			{
				return;
			}
			smallCardMobIndex = HuntSupport.ClampIndex(smallCardMobIndex, list.Count);
			TableDisposable val2 = ImRaii.Table(ImU8String.op_Implicit("##SmallCardTable"), 3, (ImGuiTableFlags)1);
			try
			{
				if (!val2)
				{
					return;
				}
				ImGui.TableSetupColumn(ImU8String.op_Implicit("Left"), (ImGuiTableColumnFlags)4, 0f, 0u);
				ImGui.TableSetupColumn(ImU8String.op_Implicit("Mid"), (ImGuiTableColumnFlags)4, 0f, 0u);
				ImGui.TableSetupColumn(ImU8String.op_Implicit("Right"), (ImGuiTableColumnFlags)4, 0f, 0u);
				ImGui.TableNextRow();
				ImGui.TableNextColumn();
				if (smallCardMobIndex > 0 && ImGuiOm.ButtonIcon("##FormerMob", (FontAwesomeIcon)61536))
				{
					smallCardMobIndex--;
				}
				ImGui.TableNextColumn();
				ImGuiOm.Text(LanguageManager.Get("Current") ?? "");
				ImGui.TableNextColumn();
				if (smallCardMobIndex < list.Count - 1 && ImGuiOm.ButtonIcon("##NextMob", (FontAwesomeIcon)61537))
				{
					smallCardMobIndex++;
				}
				ImGui.TableNextRow();
				ImGui.TableNextColumn();
				if (smallCardMobIndex > 0)
				{
					NotoriousMonsterFound notoriousMonsterFound = list[smallCardMobIndex - 1];
					using (OmenServiceBase<FontManager>.Instance().GetUIFont(config.UIScale + 0.2f).Push())
					{
						vector = (notoriousMonsterFound.IsAlive ? KnownColor.LightGreen.ToVector4() : KnownColor.Red.ToVector4());
						ImU8String val3 = new ImU8String(0, 1);
						((ImU8String)(ref val3)).AppendFormatted<string>(notoriousMonsterFound.GetBNPCName());
						ImGui.TextColored(ref vector, val3);
					}
					ImU8String val4 = new ImU8String(0, 1);
					((ImU8String)(ref val4)).AppendFormatted<string>(LanguageManager.Get("AutoMarksFinder-ZoneInfoForEachMark", notoriousMonsterFound.GetZoneName(), notoriousMonsterFound.Instance.ToSESquareCount()));
					ImGui.TextUnformatted(val4);
				}
				ImGui.TableNextColumn();
				NotoriousMonsterFound notoriousMonsterFound2 = list[smallCardMobIndex];
				using (OmenServiceBase<FontManager>.Instance().GetUIFont(config.UIScale + 0.2f).Push())
				{
					vector = (notoriousMonsterFound2.IsAlive ? KnownColor.LightGreen.ToVector4() : KnownColor.Red.ToVector4());
					ImU8String val5 = new ImU8String(0, 1);
					((ImU8String)(ref val5)).AppendFormatted<string>(notoriousMonsterFound2.GetBNPCName());
					ImGui.TextColored(ref vector, val5);
				}
				ImU8String val6 = new ImU8String(0, 1);
				((ImU8String)(ref val6)).AppendFormatted<string>(LanguageManager.Get("AutoMarksFinder-ZoneInfoForEachMark", notoriousMonsterFound2.GetZoneName(), notoriousMonsterFound2.Instance.ToSESquareCount()));
				ImGui.TextUnformatted(val6);
				ImGui.TableNextColumn();
				if (smallCardMobIndex < list.Count - 1)
				{
					NotoriousMonsterFound notoriousMonsterFound3 = list[smallCardMobIndex + 1];
					using (OmenServiceBase<FontManager>.Instance().GetUIFont(config.UIScale + 0.2f).Push())
					{
						vector = (notoriousMonsterFound3.IsAlive ? KnownColor.LightGreen.ToVector4() : KnownColor.Red.ToVector4());
						ImU8String val7 = new ImU8String(0, 1);
						((ImU8String)(ref val7)).AppendFormatted<string>(notoriousMonsterFound3.GetBNPCName());
						ImGui.TextColored(ref vector, val7);
					}
					ImU8String val8 = new ImU8String(0, 1);
					((ImU8String)(ref val8)).AppendFormatted<string>(LanguageManager.Get("AutoMarksFinder-ZoneInfoForEachMark", notoriousMonsterFound3.GetZoneName(), notoriousMonsterFound3.Instance.ToSESquareCount()));
					ImGui.TextUnformatted(val8);
				}
				ImGui.TableNextRow();
				ImGui.TableNextColumn();
				ImGui.TableNextColumn();
				NotoriousMonsterFound info = list[smallCardMobIndex];
				DrawPositionColumn(info);
				ImGui.SameLine();
				DrawAetheryteColumn(info);
				DrawMonsterStatusToggle(info);
				ImGui.SameLine();
				DrawTeleportToMonsterZone(info);
				ImGui.TableNextColumn();
				if (smallCardMobIndex < list.Count - 1)
				{
					NotoriousMonsterFound info2 = list[smallCardMobIndex + 1];
					DrawPositionColumn(info2);
					ImGui.SameLine();
					DrawAetheryteColumn(info2);
					DrawMonsterStatusToggle(info2);
					ImGui.SameLine();
					DrawTeleportToMonsterZone(info2);
				}
			}
			finally
			{
				((TableDisposable)(ref val2)).Dispose();
			}
		}
	}

	private void DrawTabItemTrain()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0403: Unknown result type (might be due to invalid IL or missing references)
		//IL_041e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0446: Unknown result type (might be due to invalid IL or missing references)
		//IL_044b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_060d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0652: Unknown result type (might be due to invalid IL or missing references)
		//IL_0681: Unknown result type (might be due to invalid IL or missing references)
		TabItemDisposable val = ImRaii.TabItem(ImU8String.op_Implicit(LanguageManager.Get("AutoMarksFinder-Routes")));
		try
		{
			if (!val)
			{
				return;
			}
			NotoriousMonsterList selected = ((selectedTrainIndex == -1 || selectedTrainIndex > config.Trains.Count - 1) ? null : config.Trains[selectedTrainIndex]);
			ImGui.AlignTextToFramePadding();
			Vector4 vector = KnownColor.Chocolate.ToVector4();
			ImU8String val2 = new ImU8String(1, 1);
			((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get("AutoMarksFinder-SelectRoutes"));
			((ImU8String)(ref val2)).AppendLiteral(":");
			ImGui.TextColored(ref vector, val2);
			ImGui.SameLine();
			DrawTrainsSelectCombo(200f * Globals.GlobalUIScale, ref selectedTrainIndex);
			ImGui.SameLine();
			DisabledDisposable val3 = ImRaii.Disabled(selected == null);
			try
			{
				if (ImGuiOm.ButtonIcon("指定出发点", (FontAwesomeIcon)62767, LanguageManager.Get("AutoMarksFinder-IntendedStartingPoint")))
				{
					ImGui.OpenPopup(ImU8String.op_Implicit("StartAetherytePopup"), (ImGuiPopupFlags)0);
				}
				PopupDisposable val4 = ImRaii.Popup(ImU8String.op_Implicit("StartAetherytePopup"));
				try
				{
					if (PopupDisposable.op_Implicit(val4))
					{
						Aetheryte? startAetheryte = selected.StartAetheryte;
						int num;
						if (!startAetheryte.HasValue)
						{
							num = 0;
						}
						else
						{
							Aetheryte valueOrDefault = startAetheryte.GetValueOrDefault();
							num = (int)((Aetheryte)(ref valueOrDefault)).RowId;
						}
						uint startAetheryte2 = (uint)num;
						ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
						if (ImGuiOmExtension.SingleSelectCombo<Aetheryte>("AetheryteSelectCombo", (IDictionary<uint, Aetheryte>)Sheets.Aetherytes, ref startAetheryte2, ref startAetheryteInput, (Func<Aetheryte, string>)delegate(Aetheryte x)
						{
							//IL_000d: Unknown result type (might be due to invalid IL or missing references)
							//IL_0012: Unknown result type (might be due to invalid IL or missing references)
							//IL_0015: Unknown result type (might be due to invalid IL or missing references)
							//IL_001a: Unknown result type (might be due to invalid IL or missing references)
							//IL_001d: Unknown result type (might be due to invalid IL or missing references)
							//IL_0022: Unknown result type (might be due to invalid IL or missing references)
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 2);
							PlaceName value = ((Aetheryte)(ref x)).PlaceName.Value;
							defaultInterpolatedStringHandler.AppendFormatted(((object)((PlaceName)(ref value)).Name/*cast due to constrained. prefix*/).ToString());
							defaultInterpolatedStringHandler.AppendLiteral(" (");
							defaultInterpolatedStringHandler.AppendFormatted(((Aetheryte)(ref x)).RowId);
							defaultInterpolatedStringHandler.AppendLiteral(")");
							return defaultInterpolatedStringHandler.ToStringAndClear();
						}, new(string, ImGuiTableColumnFlags, float)[1] { (LanguageManager.Get("Zone"), (ImGuiTableColumnFlags)4, 0f) }, new Func<Aetheryte, Action>[1]
						{
							(Aetheryte x) => delegate
							{
								//IL_0011: Unknown result type (might be due to invalid IL or missing references)
								//IL_0016: Unknown result type (might be due to invalid IL or missing references)
								//IL_0019: Unknown result type (might be due to invalid IL or missing references)
								//IL_001e: Unknown result type (might be due to invalid IL or missing references)
								//IL_0021: Unknown result type (might be due to invalid IL or missing references)
								//IL_0026: Unknown result type (might be due to invalid IL or missing references)
								//IL_0096: Unknown result type (might be due to invalid IL or missing references)
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 2);
								PlaceName value = ((Aetheryte)(ref x)).PlaceName.Value;
								defaultInterpolatedStringHandler.AppendFormatted(((object)((PlaceName)(ref value)).Name/*cast due to constrained. prefix*/).ToString());
								defaultInterpolatedStringHandler.AppendLiteral(" (");
								defaultInterpolatedStringHandler.AppendFormatted(((Aetheryte)(ref x)).RowId);
								defaultInterpolatedStringHandler.AppendLiteral(")");
								if (ImGuiOm.Selectable(defaultInterpolatedStringHandler.ToStringAndClear(), startAetheryte2 == ((Aetheryte)(ref x)).RowId, (ImGuiSelectableFlags)1))
								{
									selected.StartAetheryte = x;
								}
							}
						}, new Func<Aetheryte, string>[2]
						{
							delegate(Aetheryte x)
							{
								//IL_0002: Unknown result type (might be due to invalid IL or missing references)
								//IL_0007: Unknown result type (might be due to invalid IL or missing references)
								//IL_000a: Unknown result type (might be due to invalid IL or missing references)
								//IL_000f: Unknown result type (might be due to invalid IL or missing references)
								//IL_0012: Unknown result type (might be due to invalid IL or missing references)
								//IL_0017: Unknown result type (might be due to invalid IL or missing references)
								PlaceName value = ((Aetheryte)(ref x)).PlaceName.Value;
								return ((object)((PlaceName)(ref value)).Name/*cast due to constrained. prefix*/).ToString();
							},
							(Aetheryte x) => ((Aetheryte)(ref x)).RowId.ToString()
						}, true))
						{
							selected.StartAetheryte = LuminaGetter.GetRow<Aetheryte>(startAetheryte2);
						}
						ImGui.SameLine();
						if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("AutoMarksFinder-SortAccordingToStartingPoint")), default(Vector2)))
						{
							selected.Reorder();
						}
					}
				}
				finally
				{
					((PopupDisposable)(ref val4)).Dispose();
				}
				ImGui.SameLine(0f, 8f * Globals.GlobalUIScale);
				ImGui.TextDisabled(ImU8String.op_Implicit("|"));
				ImGui.SameLine(0f, 8f * Globals.GlobalUIScale);
				if (ImGuiOm.ButtonIcon("导出", (FontAwesomeIcon)62830, LanguageManager.Get("Export")))
				{
					ModuleBase.ExportToClipboard(selected);
				}
			}
			finally
			{
				((IDisposable)val3)?.Dispose();
			}
			ImGui.SameLine();
			if (ImGuiOm.ButtonIcon("导入", (FontAwesomeIcon)62831, LanguageManager.Get("Import")))
			{
				NotoriousMonsterList notoriousMonsterList = ModuleBase.ImportFromClipboard<NotoriousMonsterList>();
				if (notoriousMonsterList != null)
				{
					config.Trains.Add(notoriousMonsterList);
					config.Save(this);
				}
			}
			ImGui.Spacing();
			if (selected == null)
			{
				return;
			}
			List<NotoriousMonsterFound> list = selected.MonstersFound.OrderByDescending((NotoriousMonsterFound x) => x.IsAlive).ToList();
			TableDisposable val5 = ImRaii.Table(ImU8String.op_Implicit("##TrainTable"), 6, (ImGuiTableFlags)7);
			try
			{
				if (!val5)
				{
					return;
				}
				ImGui.TableSetupColumn(ImU8String.op_Implicit(LanguageManager.Get("Serial")), (ImGuiTableColumnFlags)8, 0f, 0u);
				ImGui.TableSetupColumn(ImU8String.op_Implicit(LanguageManager.Get("Name")), (ImGuiTableColumnFlags)8, 0f, 0u);
				ImGui.TableSetupColumn(ImU8String.op_Implicit(LanguageManager.Get("Zone")), (ImGuiTableColumnFlags)8, 0f, 0u);
				ImGui.TableSetupColumn(ImU8String.op_Implicit(LanguageManager.Get("AutoMarksFinder-Rank")), (ImGuiTableColumnFlags)8, 0f, 0u);
				ImGui.TableSetupColumn(ImU8String.op_Implicit(LanguageManager.Get("Position")), (ImGuiTableColumnFlags)8, 0f, 0u);
				ImGui.TableSetupColumn(ImU8String.op_Implicit(LanguageManager.Get("AutoMarksFinder-NearestAetheryte")), (ImGuiTableColumnFlags)4, 0f, 0u);
				ImGui.TableHeadersRow();
				for (int num2 = 0; num2 < list.Count; num2++)
				{
					IdDisposable val6 = ImRaii.PushId(num2, true);
					try
					{
						GroupDisposable val7 = ImRaii.Group();
						try
						{
							NotoriousMonsterFound notoriousMonsterFound = list[num2];
							ImGui.TableNextRow();
							ImGui.TableNextColumn();
							if (ImGuiOm.ButtonIcon($"删除{notoriousMonsterFound}", (FontAwesomeIcon)62189, LanguageManager.Get("Delete")))
							{
								selected.MonstersFound.Remove(notoriousMonsterFound);
							}
							ImGui.SameLine();
							GroupDisposable val8 = ImRaii.Group();
							try
							{
								if (notoriousMonsterFound.IsAlive)
								{
									ImGui.SameLine();
									val3 = ImRaii.Disabled(num2 <= 0);
									try
									{
										if (ImGuiOm.ButtonIcon("上移", (FontAwesomeIcon)61538, LanguageManager.Get("MoveUp")))
										{
											selected.MonstersFound.Swap(num2, num2 - 1);
											config.Save(this);
										}
									}
									finally
									{
										((IDisposable)val3)?.Dispose();
									}
									ImGui.SameLine();
									val3 = ImRaii.Disabled(num2 >= list.Count - 1);
									try
									{
										if (ImGuiOm.ButtonIcon("下移", (FontAwesomeIcon)61539, LanguageManager.Get("MoveDown")))
										{
											selected.MonstersFound.Swap(num2, num2 + 1);
											config.Save(this);
										}
									}
									finally
									{
										((IDisposable)val3)?.Dispose();
									}
								}
							}
							finally
							{
								((GroupDisposable)(ref val8)).Dispose();
							}
							ImGui.SameLine();
							ImU8String val9 = new ImU8String(0, 1);
							((ImU8String)(ref val9)).AppendFormatted<int>(num2 + 1, "00");
							ImGui.TextUnformatted(val9);
							ImGui.TableNextColumn();
							DrawMonsterStatusToggle(notoriousMonsterFound);
							ImGui.SameLine();
							ColorDisposable val10 = ImRaii.PushColor((ImGuiCol)0, notoriousMonsterFound.IsAlive ? KnownColor.LightGreen.ToVector4() : KnownColor.Red.ToVector4(), true);
							try
							{
								ImU8String val11 = new ImU8String(0, 1);
								((ImU8String)(ref val11)).AppendFormatted<string>(notoriousMonsterFound.GetBNPCName());
								ImGui.TextUnformatted(val11);
							}
							finally
							{
								((IDisposable)val10)?.Dispose();
							}
							ImGui.TableNextColumn();
							ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("AutoMarksFinder-ZoneInfoForEachMark", notoriousMonsterFound.GetZoneName(), notoriousMonsterFound.Instance.ToSESquareCount())));
							ImGui.TableNextColumn();
							ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("AutoMarksFinder-RankForEachMark", notoriousMonsterFound.Rank)));
							ImGui.TableNextColumn();
							DrawPositionColumn(notoriousMonsterFound);
							ImGui.TableNextColumn();
							DrawAetheryteColumn(notoriousMonsterFound);
						}
						finally
						{
							((GroupDisposable)(ref val7)).Dispose();
						}
					}
					finally
					{
						((IDisposable)val6)?.Dispose();
					}
				}
			}
			finally
			{
				((TableDisposable)(ref val5)).Dispose();
			}
		}
		finally
		{
			((TabItemDisposable)(ref val)).Dispose();
		}
	}

	private static void DrawMonsterStatusToggle(NotoriousMonsterFound? info)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (info == null)
		{
			return;
		}
		IdDisposable val = ImRaii.PushId(ImU8String.op_Implicit(info.ToString()), true);
		try
		{
			GroupDisposable val2 = ImRaii.Group();
			try
			{
				if (ImGuiOm.ButtonIcon("切换存活状态", (FontAwesomeIcon)58620, LanguageManager.Get("AutoMarksFinder-SwitchMarkAlive")))
				{
					info.SetState(!info.IsAlive);
				}
			}
			finally
			{
				((GroupDisposable)(ref val2)).Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void DrawTeleportToMonsterZone(NotoriousMonsterFound? info)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (info == null)
		{
			return;
		}
		IdDisposable val = ImRaii.PushId(ImU8String.op_Implicit(info.ToString()), true);
		try
		{
			if (!ImGuiOm.ButtonIcon("传送到目标区域", (FontAwesomeIcon)61554, LanguageManager.Get("AutoMarksFinder-TeleportToMobsZone")))
			{
				return;
			}
			if (GameState.TerritoryType != info.ZoneID)
			{
				base.TaskHelper.Enqueue((Func<bool>)IsAbleToTeleport, "等待停止移动", 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
				base.TaskHelper.Enqueue(() => OmenServiceBase<AetheryteRecordManager>.Instance().GetNearestAetheryte(info.ZoneID, (info.ZoneID == 818) ? new Vector3(573f, 349.1f, -200f) : info.Position)?.TeleportTo(), "传送至目标区域");
			}
			if (info.Instance != 0)
			{
				base.TaskHelper.Enqueue(() => GameState.TerritoryType == info.ZoneID && UIModuleExtension.IsScreenReady(), "等待传送完毕");
				base.TaskHelper.Enqueue(delegate
				{
					OmenServiceBase<ChatManager>.Instance().SendMessage($"/pdr insc {info.Instance}");
				}, $"切换副本区至 {info.Instance} 线");
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void DrawTabItemScanner()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		TabItemDisposable val = ImRaii.TabItem(ImU8String.op_Implicit(LanguageManager.Get("AutoMarksFinder-MarkScan")));
		try
		{
			if (!(!val) && IOmenDalamudService<IObjectTable>.Instance().LocalPlayer != null)
			{
				DrawScanPointSection();
				ImGui.NewLine();
				DrawScannerSection();
			}
		}
		finally
		{
			((TabItemDisposable)(ref val)).Dispose();
		}
	}

	private void DrawScanRoutePreview()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (scanner != null && ImGui.Begin(ImU8String.op_Implicit("AutoMarksFinder-ScannerRouteMap"), (ImGuiWindowFlags)2097401))
		{
			var (num, list, currentIndex) = scanner.GetCurrentScanState();
			if (num != 0 && list.Count > 0)
			{
				DrawScanRouteMap(num, list, currentIndex);
			}
			ImGui.End();
		}
	}

	private void DrawScanRouteMap(uint zoneID, List<NotoriousMonsterScanPoint> points, int currentIndex = -1)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		if (!LuminaGetter.TryGetRow<TerritoryType>(zoneID, out TerritoryType item) || !LuminaGetter.TryGetRow<Map>(((TerritoryType)(ref item)).Map.RowId, out Map item2))
		{
			return;
		}
		scanRouteMapRenderer.SetMap(((Map)(ref item2)).RowId);
		currentIndex--;
		scanRouteMapRenderer.OnCustomMapDraw = delegate(ImGuiMapRenderer r, ImDrawListPtr drawList)
		{
			if (points.Count > 1)
			{
				for (int i = 0; i < points.Count - 1; i++)
				{
					Vector2 vector2 = r.WorldToScreen(points[i].Position);
					Vector2 vector3 = r.WorldToScreen(points[i + 1].Position);
					uint num2 = ((i < currentIndex) ? ImGui.ColorConvertFloat4ToU32(KnownColor.GreenYellow.ToVector4()) : ImGui.ColorConvertFloat4ToU32(KnownColor.Orange.ToVector4()));
					((ImDrawListPtr)(ref drawList)).AddLine(vector2, vector3, num2, 2f * Globals.GlobalUIScale);
				}
			}
		};
		scanRouteMapRenderer.OnCustomForegroundDraw = delegate(ImGuiMapRenderer r, ImDrawListPtr drawList)
		{
			IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
			if (localPlayer != null && GameState.TerritoryType == zoneID)
			{
				Vector2 vector2 = r.WorldToScreen(localPlayer.Position);
				uint num2 = ImGui.ColorConvertFloat4ToU32(KnownColor.Chocolate.ToVector4());
				((ImDrawListPtr)(ref drawList)).AddCircleFilled(vector2, 6f * Globals.GlobalUIScale, num2);
				((ImDrawListPtr)(ref drawList)).AddCircle(vector2, 6f * Globals.GlobalUIScale, ImGui.ColorConvertFloat4ToU32(Vector4.One), 0, 2f * Globals.GlobalUIScale);
			}
		};
		scanRouteMapRenderer.ClearMarkers();
		for (int num = 0; num < points.Count; num++)
		{
			NotoriousMonsterScanPoint notoriousMonsterScanPoint = points[num];
			Vector4 vector = ((num >= currentIndex) ? ((num != currentIndex) ? KnownColor.Orange.ToVector4() : KnownColor.Red.ToVector4()) : KnownColor.GreenYellow.ToVector4());
			ImGuiMapMarker marker = new ImGuiMapMarker
			{
				ID = $"ScanPoint_{num}",
				Position = notoriousMonsterScanPoint.Position,
				Color = ImGui.ColorConvertFloat4ToU32(vector),
				Size = new Vector2(8f * Globals.GlobalUIScale),
				ShowLabel = false,
				ShowTooltip = true,
				TooltipText = $"#{num + 1} {notoriousMonsterScanPoint.Position:F0}"
			};
			scanRouteMapRenderer.AddMarker(marker);
		}
		scanRouteMapRenderer.Draw(Globals.ScaledVector2(300f));
	}

	private void DrawTabItemSettings()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		TabItemDisposable val = ImRaii.TabItem(ImU8String.op_Implicit(LanguageManager.Get("Settings")));
		try
		{
			if (!val)
			{
				return;
			}
			ImGui.AlignTextToFramePadding();
			Vector4 vector = KnownColor.Chocolate.ToVector4();
			ImU8String val2 = new ImU8String(1, 1);
			((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get("FontScale"));
			((ImU8String)(ref val2)).AppendLiteral(":");
			ImGui.TextColored(ref vector, val2);
			for (float num = 0.6f; num < 1.8f; num += 0.2f)
			{
				float num2 = (float)Math.Round(num, 1);
				DisabledDisposable val3 = ImRaii.Disabled(config.UIScale == num2);
				try
				{
					ImGui.SameLine();
					ImU8String val4 = new ImU8String(11, 1);
					((ImU8String)(ref val4)).AppendFormatted<float>(num2);
					((ImU8String)(ref val4)).AppendLiteral("##FontScale");
					if (ImGui.Button(val4, default(Vector2)))
					{
						config.UIScale = num2;
						config.Save(this);
					}
				}
				finally
				{
					((IDisposable)val3)?.Dispose();
				}
			}
			ImGui.NewLine();
			vector = KnownColor.Orange.ToVector4();
			ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("AutoMarksFinder-RelaySetting")));
			ImGuiOm.HelpMarker(LanguageManager.Get("AutoMarksFinder-RelayArgsHelp"), 40f, (FontAwesomeIcon)61530);
			IndentDisposable val5 = ImRaii.PushIndent(1, true);
			try
			{
				ImGui.AlignTextToFramePadding();
				vector = KnownColor.Chocolate.ToVector4();
				ImU8String val6 = new ImU8String(1, 1);
				((ImU8String)(ref val6)).AppendFormatted<string>(LanguageManager.Get("AutoMarksFinder-RelayPosition"));
				((ImU8String)(ref val6)).AppendLiteral(":");
				ImGui.TextColored(ref vector, val6);
				ImGui.SameLine();
				ImGui.InputText(ImU8String.op_Implicit("###RelayPositionMacro"), ref config.RelayMacroPattern, 255, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
				if (ImGui.IsItemDeactivatedAfterEdit())
				{
					config.Save(this);
				}
				ImGui.SameLine();
				if (ImGuiOm.ButtonIcon("PreviewPosition", (FontAwesomeIcon)61773, LanguageManager.Get("Preview")))
				{
					OmenServiceBase<ChatManager>.Instance().SendMessage("/e " + HuntSupport.FormatMacro(config.RelayMacroPattern, "<pos>", "<1>", (!InstancesManager.IsInstancedArea) ? null : LanguageManager.Get("AutoMarksFinder-RelayInstanceDisplay", InstancesManager.CurrentInstance.ToSESquareCount())));
				}
				ImGui.AlignTextToFramePadding();
				vector = KnownColor.Chocolate.ToVector4();
				ImU8String val7 = new ImU8String(1, 1);
				((ImU8String)(ref val7)).AppendFormatted<string>(LanguageManager.Get("AutoMarksFinder-RelayAetheryte"));
				((ImU8String)(ref val7)).AppendLiteral(":");
				ImGui.TextColored(ref vector, val7);
				ImGui.SameLine();
				ImGui.InputText(ImU8String.op_Implicit("###RelayAetheryteMacro"), ref config.WaitMacroPattern, 255, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
				if (ImGui.IsItemDeactivatedAfterEdit())
				{
					config.Save(this);
				}
				ImGui.SameLine();
				if (ImGuiOm.ButtonIcon("PreviewAetheryte", (FontAwesomeIcon)61773, LanguageManager.Get("Preview")))
				{
					OmenServiceBase<ChatManager>.Instance().SendMessage("/e " + HuntSupport.FormatMacro(config.WaitMacroPattern, "<pos>", "<1>", (!InstancesManager.IsInstancedArea) ? null : LanguageManager.Get("AutoMarksFinder-RelayInstanceDisplay", InstancesManager.CurrentInstance.ToSESquareCount())));
				}
				ImGui.AlignTextToFramePadding();
				vector = KnownColor.Chocolate.ToVector4();
				ImU8String val8 = new ImU8String(1, 1);
				((ImU8String)(ref val8)).AppendFormatted<string>(LanguageManager.Get("AutoMarksFinder-RelayChatTypes"));
				((ImU8String)(ref val8)).AppendLiteral(":");
				ImGui.TextColored(ref vector, val8);
				ImGui.SameLine();
				ComboDisposable val9 = ImRaii.Combo(ImU8String.op_Implicit("###RelayCommandCombo"), ImU8String.op_Implicit(LanguageManager.Get("AutoOpenMapLinks-AlreadyAddedChannelCount", config.RelayCommands.Count)), (ImGuiComboFlags)8);
				try
				{
					if (!ComboDisposable.op_Implicit(val9))
					{
						return;
					}
					foreach (string supportedRelayCommand in SupportedRelayCommands)
					{
						if (ImGui.Selectable(ImU8String.op_Implicit(supportedRelayCommand), config.RelayCommands.Contains(supportedRelayCommand), (ImGuiSelectableFlags)1, default(Vector2)))
						{
							if (!config.RelayCommands.Remove(supportedRelayCommand))
							{
								config.RelayCommands.Add(supportedRelayCommand);
							}
							config.Save(this);
						}
					}
				}
				finally
				{
					((ComboDisposable)(ref val9)).Dispose();
				}
			}
			finally
			{
				((IDisposable)val5)?.Dispose();
			}
		}
		finally
		{
			((TabItemDisposable)(ref val)).Dispose();
		}
	}

	private unsafe void DrawPositionColumn(NotoriousMonsterFound? info)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		if (info == null)
		{
			return;
		}
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(0, 1);
		((ImU8String)(ref val)).AppendFormatted<NotoriousMonsterFound>(info);
		IdDisposable val2 = ImRaii.PushId(val, true);
		try
		{
			AgentMap* ptr = AgentMap.Instance();
			if (ImGuiOm.ButtonIcon($"Relay{info}Position", (FontAwesomeIcon)61744, LanguageManager.Get("AutoMarksFinder-RelayPosition")))
			{
				((AgentMap)ptr).OpenMap(info.GetMapID(), info.ZoneID, info.GetBNPCName() ?? "", (MapType)1);
				((AgentMap)ptr).SetFlagMapMarker(info.ZoneID, info.GetMapID(), info.Position, 60561u);
				foreach (string relayCommand in config.RelayCommands)
				{
					string text = FormatPattern(config.RelayMacroPattern, "<flag>", info.GetBNPCName(), (info.Instance == 0) ? null : (LanguageManager.Get("AutoMarksFinder-RelayInstanceDisplay", info.Instance.ToSESquareCount()) ?? ""));
					OmenServiceBase<ChatManager>.Instance().SendMessage(relayCommand + " " + text);
				}
			}
			ImGui.SameLine();
			ImU8String val3 = new ImU8String(0, 1);
			((ImU8String)(ref val3)).AppendFormatted<Vector3>(info.Position, "F0");
			ImGui.TextUnformatted(val3);
			if (ImGui.IsItemHovered())
			{
				ImGui.SetMouseCursor((ImGuiMouseCursor)7);
			}
			if (ImGui.IsItemClicked((ImGuiMouseButton)0))
			{
				string text2 = $"{info.GetBNPCName()} ({LanguageManager.Get("AutoMarksFinder-ZoneInfoForEachMark", info.GetZoneName(), info.Instance.ToSESquareCount())})";
				((AgentMap)ptr).SetFlagMapMarker(info.ZoneID, info.GetMapID(), info.Position, 60561u);
				((AgentMap)ptr).OpenMap(info.GetMapID(), info.ZoneID, text2, (MapType)1);
			}
			if (ImGui.IsItemClicked((ImGuiMouseButton)1))
			{
				((AgentMap)ptr).SetFlagMapMarker(info.ZoneID, info.GetMapID(), info.Position, 60561u);
				OmenServiceBase<NotifyHelper>.Instance().NotificationInfo(LanguageManager.Get("AutoMarksFinder-SetFlagMapMarkerSucceed", info.GetZoneName(), $"{info.Position:F0}"), LanguageManager.Get("AutoMarksFinderTitle"));
			}
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	private unsafe void DrawAetheryteColumn(NotoriousMonsterFound? info)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		if (info == null)
		{
			return;
		}
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(0, 1);
		((ImU8String)(ref val)).AppendFormatted<NotoriousMonsterFound>(info);
		IdDisposable val2 = ImRaii.PushId(val, true);
		try
		{
			AgentMap* ptr = AgentMap.Instance();
			Aetheryte valueOrDefault;
			PlaceName value;
			Aetheryte? nearestAetheryte;
			if (ImGuiOm.ButtonIcon($"Relay{info}Nearest", (FontAwesomeIcon)61773, LanguageManager.Get("AutoMarksFinder-RelayAetheryte")) && info.NearestAetherytePosition.HasValue)
			{
				uint mapID = info.GetMapID();
				uint zoneID = info.ZoneID;
				nearestAetheryte = info.NearestAetheryte;
				object obj;
				if (!nearestAetheryte.HasValue)
				{
					obj = null;
				}
				else
				{
					valueOrDefault = nearestAetheryte.GetValueOrDefault();
					value = ((Aetheryte)(ref valueOrDefault)).PlaceName.Value;
					obj = ((object)((PlaceName)(ref value)).Name/*cast due to constrained. prefix*/).ToString();
				}
				if (obj == null)
				{
					obj = "";
				}
				((AgentMap)ptr).OpenMap(mapID, zoneID, (string)obj, (MapType)1);
				((AgentMap)ptr).SetFlagMapMarker(info.ZoneID, info.GetMapID(), info.NearestAetherytePosition.Value, 60561u);
				foreach (string relayCommand in config.RelayCommands)
				{
					string text = FormatPattern(config.WaitMacroPattern, "<flag>", info.GetBNPCName(), (info.Instance == 0) ? null : (LanguageManager.Get("AutoMarksFinder-RelayInstanceDisplay", info.Instance.ToSESquareCount()) ?? ""));
					OmenServiceBase<ChatManager>.Instance().SendMessage(relayCommand + " " + text);
				}
			}
			ImGui.SameLine();
			DrawTeleportToMonsterZone(info);
			ImGui.SameLine();
			ImU8String val3 = new ImU8String(0, 1);
			nearestAetheryte = info.NearestAetheryte;
			object obj2;
			if (!nearestAetheryte.HasValue)
			{
				obj2 = null;
			}
			else
			{
				valueOrDefault = nearestAetheryte.GetValueOrDefault();
				value = ((Aetheryte)(ref valueOrDefault)).PlaceName.Value;
				obj2 = ((object)((PlaceName)(ref value)).Name/*cast due to constrained. prefix*/).ToString();
			}
			((ImU8String)(ref val3)).AppendFormatted<string>((string)obj2);
			ImGui.TextUnformatted(val3);
			if (ImGui.IsItemHovered())
			{
				ImGui.SetMouseCursor((ImGuiMouseCursor)7);
			}
			if (ImGui.IsItemClicked())
			{
				uint mapID2 = info.GetMapID();
				uint zoneID2 = info.ZoneID;
				nearestAetheryte = info.NearestAetheryte;
				object obj3;
				if (!nearestAetheryte.HasValue)
				{
					obj3 = null;
				}
				else
				{
					valueOrDefault = nearestAetheryte.GetValueOrDefault();
					value = ((Aetheryte)(ref valueOrDefault)).PlaceName.Value;
					obj3 = ((object)((PlaceName)(ref value)).Name/*cast due to constrained. prefix*/).ToString();
				}
				if (obj3 == null)
				{
					obj3 = "";
				}
				((AgentMap)ptr).OpenMap(mapID2, zoneID2, (string)obj3, (MapType)1);
				((AgentMap)ptr).SetFlagMapMarker(info.ZoneID, info.GetMapID(), info.NearestAetherytePosition.GetValueOrDefault(), 60561u);
			}
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	private void DrawScanPointSection()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		List<NotoriousMonsterScanPoint> value;
		bool num = config.ScanPoints.TryGetValue(GameState.TerritoryType, out value);
		ImGui.AlignTextToFramePadding();
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(1, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("CurrentZone"));
		((ImU8String)(ref val)).AppendLiteral(":");
		ImGui.TextColored(ref vector, val);
		ImGui.SameLine();
		ImU8String val2 = default(ImU8String);
		((ImU8String)(ref val2))._002Ector(3, 2);
		((ImU8String)(ref val2)).AppendFormatted<string>(LuminaWrapper.GetZonePlaceName(GameState.TerritoryType));
		((ImU8String)(ref val2)).AppendLiteral(" (");
		((ImU8String)(ref val2)).AppendFormatted<uint>(GameState.TerritoryType);
		((ImU8String)(ref val2)).AppendLiteral(")");
		ImGui.TextUnformatted(val2);
		ImGui.SameLine(0f, 8f * Globals.GlobalUIScale);
		ImGui.TextDisabled(ImU8String.op_Implicit("|"));
		ImGui.SameLine(0f, 8f * Globals.GlobalUIScale);
		vector = KnownColor.Chocolate.ToVector4();
		ImU8String val3 = default(ImU8String);
		((ImU8String)(ref val3))._002Ector(1, 1);
		((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get("AutoMarksFinder-ScanPoints"));
		((ImU8String)(ref val3)).AppendLiteral(":");
		ImGui.TextColored(ref vector, val3);
		ImGui.SameLine();
		ImGui.TextUnformatted(ImU8String.op_Implicit(num ? value.Count.ToString() : "0"));
		ImGui.SameLine(0f, 8f * Globals.GlobalUIScale);
		Task task = spawnPointUpdateTask;
		DisabledDisposable val4 = ImRaii.Disabled(task != null && !task.IsCompleted);
		try
		{
			if (!ImGui.SmallButton(ImU8String.op_Implicit(LanguageManager.Get("AutoMarksFinder-UpdateSpawnPoints"))))
			{
				return;
			}
			spawnPointUpdateTask = HuntSupport.RunLocal(async delegate
			{
				try
				{
					Dictionary<uint, List<NotoriousMonsterScanPoint>> dictionary = JsonConvert.DeserializeObject<Dictionary<uint, List<NotoriousMonsterScanPoint>>>(await BundledData.ReadMarksAsync(OmenServiceBase<HTTPClientHelper>.Instance().Get(), "https://gh.atmoomen.top/StaticAssets/main/DailyRoutines/module/AutoMarksFinder_SpawnPoints.json").ConfigureAwait(continueOnCapturedContext: false), JsonSerializerSettingsExtension.GetShared());
					if (dictionary != null && dictionary.Count > 0)
					{
						config.ScanPoints = dictionary;
						config.Save(ManagerBase<ModuleManager>.Instance().GetModule<AutoMarksFinder>());
						OmenServiceBase<NotifyHelper>.Instance().NotificationInfo(LanguageManager.Get("AutoMarksFinder-UpdateSpawnPointsRet", dictionary.SelectMany((KeyValuePair<uint, List<NotoriousMonsterScanPoint>> x) => x.Value).Count()), LanguageManager.Get("AutoMarksFinderTitle"));
					}
				}
				catch
				{
				}
			}).ContinueWith((Task _) => spawnPointUpdateTask = null);
		}
		finally
		{
			((IDisposable)val4)?.Dispose();
		}
	}

	private void DrawScannerSection()
	{
		DrawScannerOperation();
		ImGui.NewLine();
		DrawScannerSetting();
		ImGui.NewLine();
		DrawScanResult();
	}

	private void DrawScannerOperation()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Orange.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("Operation")));
		ImGui.Spacing();
		DisabledDisposable val = ImRaii.Disabled(scanner != null || config.SelectedZones.Count == 0);
		try
		{
			if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61515, LanguageManager.Get("Start")))
			{
				ClearScanner();
				scanner = new NotoriousMonsterScanner(this, config.SelectedRanks, config.SelectedZones);
				scanner.StartScan();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		ImGui.SameLine();
		if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61517, LanguageManager.Get("Stop")))
		{
			StopScanner();
		}
		ImGui.SameLine();
		val = ImRaii.Disabled(scanner == null);
		try
		{
			if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)62189, LanguageManager.Get("Clear")))
			{
				ClearScanner();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void DrawScannerSetting()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_041e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0423: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		//IL_0445: Unknown result type (might be due to invalid IL or missing references)
		//IL_0472: Unknown result type (might be due to invalid IL or missing references)
		//IL_0477: Unknown result type (might be due to invalid IL or missing references)
		//IL_0494: Unknown result type (might be due to invalid IL or missing references)
		//IL_0499: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Orange.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("AutoMarksFinder-ScannerSettings")));
		ImGui.Spacing();
		IndentDisposable val = ImRaii.PushIndent(1, true);
		try
		{
			ImGui.SetNextItemWidth(150f * Globals.GlobalUIScale);
			ImU8String val2 = new ImU8String(30, 1);
			((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get("AutoMarksFinder-PointDelayMS"));
			((ImU8String)(ref val2)).AppendLiteral("##AutoMarksFinder-PointDelayMS");
			ImU8String val3 = val2;
			ref int delayMS = ref config.DelayMS;
			ImU8String val4 = default(ImU8String);
			ImGui.InputInt(val3, ref delayMS, 500, 0, val4, (ImGuiInputTextFlags)0);
			if (ImGui.IsItemDeactivatedAfterEdit())
			{
				config.Save(this);
			}
			ImGui.SameLine(0f, 8f * Globals.GlobalUIScale);
			ImGui.TextDisabled(ImU8String.op_Implicit("|"));
			ImGui.SameLine(0f, 8f * Globals.GlobalUIScale);
			val4 = new ImU8String(1, 1);
			((ImU8String)(ref val4)).AppendFormatted<string>(LanguageManager.Get("AutoMarksFinder-TargetRank"));
			((ImU8String)(ref val4)).AppendLiteral(":");
			ImGui.TextUnformatted(val4);
			ImGui.SameLine(0f, 8f * Globals.GlobalUIScale);
			GroupDisposable val5 = ImRaii.Group();
			try
			{
				bool flag = true;
				NotoriousMonsterRank[] values = Enum.GetValues<NotoriousMonsterRank>();
				for (int i = 0; i < values.Length; i++)
				{
					NotoriousMonsterRank notoriousMonsterRank = values[i];
					if (notoriousMonsterRank == NotoriousMonsterRank.S)
					{
						continue;
					}
					if (!flag)
					{
						ImGui.SameLine(0f, 4f * Globals.GlobalUIScale);
					}
					flag = false;
					bool flag2 = config.SelectedRanks.Contains(notoriousMonsterRank);
					if (ImGui.Checkbox(ImU8String.op_Implicit(notoriousMonsterRank.ToString()), ref flag2))
					{
						if (!config.SelectedRanks.Remove(notoriousMonsterRank))
						{
							config.SelectedRanks.Add(notoriousMonsterRank);
						}
						config.Save(this);
					}
				}
			}
			finally
			{
				((GroupDisposable)(ref val5)).Dispose();
			}
			var dictionary = (from x in Enumerable.Select(config.ScanPoints, (KeyValuePair<uint, List<NotoriousMonsterScanPoint>> x) => new
				{
					Zone = LuminaGetter.GetRow<TerritoryType>(x.Key).GetValueOrDefault(),
					Points = x.Value.Count
				}).GroupBy(x =>
				{
					//IL_0001: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0009: Unknown result type (might be due to invalid IL or missing references)
					//IL_000e: Unknown result type (might be due to invalid IL or missing references)
					TerritoryType zone2 = x.Zone;
					return ((TerritoryType)(ref zone2)).ExVersion.RowId;
				})
				orderby x.Key
				select x).ToDictionary(x => x.Key, x => x.ToList());
			ImGui.SetNextItemWidth(250f * Globals.GlobalUIScale);
			ImU8String val6 = new ImU8String(18, 1);
			((ImU8String)(ref val6)).AppendFormatted<string>(LanguageManager.Get("AutoMarksFinder-TargetZones"));
			((ImU8String)(ref val6)).AppendLiteral("###ZoneSelectCombo");
			ComboDisposable val7 = ImRaii.Combo(val6, ImU8String.op_Implicit(LanguageManager.Get("AutoMarksFinder-SelectedZonePreview", config.SelectedZones.Count)), (ImGuiComboFlags)8);
			try
			{
				if (!ComboDisposable.op_Implicit(val7))
				{
					return;
				}
				bool flag3 = true;
				foreach (var (num2, list2) in dictionary)
				{
					if (!flag3)
					{
						ImGui.Spacing();
						ImGui.Separator();
						ImGui.Separator();
						ImGui.Spacing();
					}
					flag3 = false;
					ImU8String val8 = new ImU8String(2, 1);
					((ImU8String)(ref val8)).AppendFormatted<uint>(num2 + 2);
					((ImU8String)(ref val8)).AppendLiteral(".0");
					ImGui.TextUnformatted(val8);
					if (ImGui.IsItemHovered())
					{
						ImGui.SetMouseCursor((ImGuiMouseCursor)7);
					}
					if (ImGui.IsItemClicked())
					{
						uint[] values2 = list2.Select(x =>
						{
							//IL_0001: Unknown result type (might be due to invalid IL or missing references)
							//IL_0006: Unknown result type (might be due to invalid IL or missing references)
							TerritoryType zone2 = x.Zone;
							return ((TerritoryType)(ref zone2)).RowId;
						}).ToArray();
						if (config.SelectedZones.ContainsAny(values2))
						{
							config.SelectedZones.RemoveRange(values2);
						}
						else
						{
							config.SelectedZones.AddRange(values2);
						}
						config.Save(this);
					}
					foreach (var item in list2)
					{
						ImU8String val9 = new ImU8String(0, 1);
						TerritoryType row = item.Zone;
						((ImU8String)(ref val9)).AppendFormatted<string>(row.ExtractPlaceName());
						ImU8String val10 = val9;
						HashSet<uint> selectedZones = config.SelectedZones;
						TerritoryType zone = item.Zone;
						if (ImGui.Selectable(val10, selectedZones.Contains(((TerritoryType)(ref zone)).RowId), (ImGuiSelectableFlags)1, default(Vector2)))
						{
							HashSet<uint> selectedZones2 = config.SelectedZones;
							row = item.Zone;
							if (!selectedZones2.Remove(((TerritoryType)(ref row)).RowId))
							{
								HashSet<uint> selectedZones3 = config.SelectedZones;
								row = item.Zone;
								selectedZones3.Add(((TerritoryType)(ref row)).RowId);
							}
							config.Save(this);
						}
					}
				}
			}
			finally
			{
				((ComboDisposable)(ref val7)).Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private unsafe void DrawScanResult()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Orange.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("AutoMarksFinder-ScanResults")));
		ImGui.Spacing();
		IndentDisposable val = ImRaii.PushIndent(1, true);
		try
		{
			if (scanner == null)
			{
				ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("AutoMarksFinder-NoRunningScanner")));
				return;
			}
			DisabledDisposable val2 = ImRaii.Disabled(scanner.FoundOnes.Count == 0);
			try
			{
				ImGui.SameLine();
				ImGui.AlignTextToFramePadding();
				vector = KnownColor.Chocolate.ToVector4();
				ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("AutoMarksFinder-SaveTo")));
				ImGui.SameLine();
				DrawTrainsSelectCombo(400f * Globals.GlobalUIScale, ref selectedTrainIndex);
				DisabledDisposable val3 = ImRaii.Disabled(((selectedTrainIndex == -1 || selectedTrainIndex > config.Trains.Count - 1) ? null : config.Trains[selectedTrainIndex]) == null);
				try
				{
					ImGui.SameLine();
					if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("Save")), default(Vector2)))
					{
						List<NotoriousMonsterFound> collection = scanner.FoundOnes.SelectMany<KeyValuePair<uint, HashSet<NotoriousMonsterFound>>, NotoriousMonsterFound>((KeyValuePair<uint, HashSet<NotoriousMonsterFound>> x) => x.Value).Distinct().ToList();
						config.Trains[selectedTrainIndex].MonstersFound.AddRange(collection);
						config.Save(ManagerBase<ModuleManager>.Instance().GetModule<AutoMarksFinder>());
						OmenServiceBase<NotifyHelper>.Instance().NotificationInfo(LanguageManager.Get("SavedSuccessfully"));
					}
				}
				finally
				{
					((IDisposable)val3)?.Dispose();
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
			foreach (KeyValuePair<uint, HashSet<NotoriousMonsterFound>> foundOne in scanner.FoundOnes)
			{
				ImU8String val4 = new ImU8String(3, 2);
				TerritoryType? row = LuminaGetter.GetRow<TerritoryType>(foundOne.Key);
				((ImU8String)(ref val4)).AppendFormatted<string>(row.HasValue ? row.GetValueOrDefault().ExtractPlaceName() : null);
				((ImU8String)(ref val4)).AppendLiteral(" (");
				((ImU8String)(ref val4)).AppendFormatted<uint>(foundOne.Key);
				((ImU8String)(ref val4)).AppendLiteral(")");
				if (!ImGui.CollapsingHeader(val4, (ImGuiTreeNodeFlags)0))
				{
					continue;
				}
				foreach (NotoriousMonsterFound item in foundOne.Value)
				{
					ImGui.TextUnformatted(ImU8String.op_Implicit(item.GetBNPCName() + " " + LanguageManager.Get("AutoMarksFinder-ZoneInfoForEachMark", item.Position, item.Instance)));
					if (ImGui.IsItemHovered())
					{
						ImGui.SetMouseCursor((ImGuiMouseCursor)7);
					}
					if (ImGui.IsItemClicked())
					{
						AgentMap* intPtr = AgentMap.Instance();
						((AgentMap)intPtr).SetFlagMapMarker(item.ZoneID, item.GetMapID(), item.Position, 60561u);
						((AgentMap)intPtr).OpenMap(item.GetMapID(), item.ZoneID, item.GetBNPCName(), (MapType)1);
					}
				}
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	static AutoMarksFinder()
	{
		_003C_003Ey__InlineArray21<string> buffer = default(_003C_003Ey__InlineArray21<string>);
		buffer[0] = "/sh";
		buffer[1] = "/y";
		buffer[2] = "/b";
		buffer[3] = "/p";
		buffer[4] = "/fc";
		buffer[5] = "/cwl1";
		buffer[6] = "/cwl2";
		buffer[7] = "/cwl3";
		buffer[8] = "/cwl4";
		buffer[9] = "/cwl5";
		buffer[10] = "/cwl6";
		buffer[11] = "/cwl7";
		buffer[12] = "/cwl8";
		buffer[13] = "/ls1";
		buffer[14] = "/ls2";
		buffer[15] = "/ls3";
		buffer[16] = "/ls4";
		buffer[17] = "/ls5";
		buffer[18] = "/ls6";
		buffer[19] = "/ls7";
		buffer[20] = "/ls8";
		SupportedRelayCommands = FrozenSet.Create<string>(buffer);
	}
}
