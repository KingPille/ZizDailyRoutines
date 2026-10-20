using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.LocalData;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Common.Math;
using Lumina.Excel.Sheets;
using Lumina.Extensions;
using Newtonsoft.Json;
using OmenTools;
using OmenTools.Dalamud;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game;
using OmenTools.Interop.Game.ExecuteCommand.Implementations;
using OmenTools.Interop.Game.Helpers;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules.AutoHuntingLog;

public class AutoHuntingLog : ModuleBase
{
	private class l6WSnC3BFMau5ODGRkx : ModuleConfig
	{
		public HuntingData? Data;

		public l6WSnC3BFMau5ODGRkx()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}
	}

	public class HuntingData
	{
		public Dictionary<uint, List<HuntingRank>> JobRanks { get; set; }

		public HuntingData()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			JobRanks = new Dictionary<uint, List<HuntingRank>>();
			base._002Ector();
		}
	}

	public class HuntingRank
	{
		public List<HuntingTask> Tasks { get; set; }

		[JsonIgnore]
		public List<HuntingMonster> AllMonsters
		{
			get
			{
				List<HuntingMonster> list = new List<HuntingMonster>();
				foreach (HuntingTask task in Tasks)
				{
					list.AddRange(task.Monsters);
				}
				return list.OrderBy((HuntingMonster x) => x.GetLocation().Zone).ToList();
			}
		}

		public HuntingRank()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			Tasks = new List<HuntingTask>();
			base._002Ector();
		}
	}

	public class HuntingTask
	{
		public string Name;

		public List<HuntingMonster> Monsters { get; set; }

		[JsonIgnore]
		public List<HuntingMonster> OpenWorldMonsters => Monsters.Where((HuntingMonster m) => m.IsOpenWorld).ToList();

		[JsonIgnore]
		public List<HuntingMonster> DutyMonsters => Monsters.Where((HuntingMonster m) => !m.IsOpenWorld).ToList();

		public HuntingTask()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			Monsters = new List<HuntingMonster>();
			base._002Ector();
		}
	}

	public class HuntingMonster
	{
		public uint ID { get; set; }

		public string Name { get; set; }

		public byte Count { get; set; }

		public List<HuntingMonsterLocation> Locations { get; set; }

		[JsonIgnore]
		public bool IsOpenWorld => !Locations[0].IsDuty;

		public string GetName()
		{
			return LuminaWrapper.GetBNPCName(ID);
		}

		public HuntingMonsterLocation GetLocation()
		{
			return Locations[0];
		}

		public HuntingMonster()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			Locations = new List<HuntingMonsterLocation>();
			base._002Ector();
		}
	}

	public class HuntingMonsterLocation
	{
		[JsonProperty("Terri")]
		public uint Zone { get; set; }

		public uint Map { get; set; }

		[JsonProperty("Zone")]
		public uint InstanceZone { get; set; }

		[JsonProperty("xCoord")]
		public float X { get; set; }

		[JsonProperty("yCoord")]
		public float Y { get; set; }

		[JsonIgnore]
		public string PlaceName { get; private set; }

		[JsonIgnore]
		public bool IsDuty { get; private set; }

		[JsonIgnore]
		public string DutyName { get; private set; }

		[JsonIgnore]
		public Vector2 MapPosition => new Vector2(X, Y);

		[JsonIgnore]
		public Vector3 WorldPosition => PositionHelper.MapToWorld(MapPosition, GetMap()).ToVector3(0f);

		[JsonIgnore]
		public string LocationDisplay
		{
			get
			{
				if (!IsDuty)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(11, 3);
					defaultInterpolatedStringHandler.AppendFormatted(PlaceName);
					defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x27D152CB ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_39d5d8025f5249e6b489bc374eefbb3c));
					defaultInterpolatedStringHandler.AppendFormatted(X, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489FDEFE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b2294f2f9bb24a76b00cf1af75888414));
					defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x258F7172 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_dadeb2ab61424d31be7cc1966e675825));
					defaultInterpolatedStringHandler.AppendFormatted(Y, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x1C175775 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_accf3844f1c6464481df1849f0aee1bb));
					defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5A7FDFD1 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5154ccd9fed0487f8fa0244531466009));
					return defaultInterpolatedStringHandler.ToStringAndClear();
				}
				return DutyName ?? "";
			}
		}

		[JsonConstructor]
		public HuntingMonsterLocation(uint terri, uint map, uint instanceZone = 0u)
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			DutyName = string.Empty;
			base._002Ector();
			Zone = terri;
			Map = map;
			InstanceZone = instanceZone;
			try
			{
				PlaceName = GetZone().ExtractPlaceName();
				if (InstanceZone != 0)
				{
					IsDuty = true;
					ContentFinderCondition content = GetContent();
					DutyName = ((object)((ContentFinderCondition)(ref content)).Name/*cast due to constrained. prefix*/).ToString();
				}
			}
			catch
			{
			}
		}

		public TerritoryType GetZone()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			return LuminaGetter.GetRow<TerritoryType>(Zone).GetValueOrDefault();
		}

		public TerritoryType GetInstanceZone()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			return LuminaGetter.GetRow<TerritoryType>(InstanceZone).GetValueOrDefault();
		}

		public ContentFinderCondition GetContent()
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			return LinqExtensions.FirstOrNull<ContentFinderCondition>((IEnumerable<ContentFinderCondition>)LuminaGetter.Get<ContentFinderCondition>(), (Predicate<ContentFinderCondition>)((ContentFinderCondition x) => ((ContentFinderCondition)(ref x)).TerritoryType.RowId == InstanceZone)).GetValueOrDefault();
		}

		public Map GetMap()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			return LuminaGetter.GetRow<Map>(Map).GetValueOrDefault();
		}

		public unsafe void SetFlagMarker()
		{
			AgentMap* intPtr = AgentMap.Instance();
			((AgentMap)intPtr).SetFlagMapMarker(Zone, Map, WorldPosition, 60561u);
			((AgentMap)intPtr).OpenMap(Map, Zone, (string)null, (MapType)1);
		}

		[CompilerGenerated]
		private bool G1d3meYxP3(ContentFinderCondition x)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			return ((ContentFinderCondition)(ref x)).TerritoryType.RowId == InstanceZone;
		}
	}

	public struct MonsterProgress
	{
		public readonly int KilledCount;

		public readonly bool IsDone;

		public MonsterProgress(int killedCount)
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			KilledCount = 0;
			IsDone = false;
			KilledCount = killedCount;
		}

		public MonsterProgress(bool isDone)
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			KilledCount = 0;
			IsDone = false;
			IsDone = isDone;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass24_0
	{
		public HuntingMonster monster;

		public AutoHuntingLog _003C_003E4__this;

		public bool forceToWalk;

		public unsafe BattleChara* localPlayer;

		public Vector2 center;

		public IBattleNPC monsterInstance;

		public _003C_003Ec__DisplayClass24_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal bool? bJg3i6vhhS()
		{
			return OmenServiceBase<AetheryteRecordManager>.Instance().GetNearestAetheryte(monster.GetLocation().Zone, monster.GetLocation().WorldPosition)?.TeleportTo();
		}

		internal bool nuX3PZaX7c()
		{
			if (GameState.TerritoryType == monster.GetLocation().Zone)
			{
				return UIModuleExtension.IsScreenReady();
			}
			return false;
		}

		internal void Eyh3htNZik()
		{
			Vector3? vector = vnavmeshIPC.QueryNearestPointOnMesh(monster.GetLocation().WorldPosition, 30f, 10000f);
			if (!vector.HasValue)
			{
				string message = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x9B44194 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2f1c0cb46b4440d499a69ed8afe22360), monster.GetName(), monster.ID);
				OmenServiceBase<NotifyHelper>.Instance().NotificationWarning(message);
				monster.GetLocation().SetFlagMarker();
				_003C_003E4__this.TaskHelper.Abort();
			}
			else
			{
				vnavmeshIPC.PathfindAndMoveTo(vector.Value, !forceToWalk & Control.CanFly);
			}
		}

		internal unsafe bool dEv3oEaTuD()
		{
			return Vector2.Distance(((BattleChara)localPlayer).Position.ToVector2(), center) <= 100f;
		}

		internal unsafe bool Nhj3gtA1kp()
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			if (Vector3.DistanceSquared(Vector3.op_Implicit(((BattleChara)localPlayer).Position), monsterInstance.Position) <= 6f)
			{
				return true;
			}
			if (!Throttler.Shared.Throttle(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4B0F2C76 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_8e002b0644ef421698ef6f63772929af), 1000u))
			{
				return false;
			}
			if (monsterInstance == null || monsterInstance.IsDead)
			{
				_003C_003E4__this.TaskHelper.Abort();
				_003C_003E4__this.TaskHelper.Enqueue((Action)_003C_003E4__this.tETsB259TB, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--1470179789 ^ 0x763587E3 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cd1b319f326a44d1aaea645e95d8698b), 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
				return true;
			}
			vnavmeshIPC.PathfindAndMoveTo(monsterInstance.Position, (!forceToWalk && DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4]) & Control.CanFly);
			return Vector3.DistanceSquared(Vector3.op_Implicit(((BattleChara)localPlayer).Position), monsterInstance.Position) <= 6f;
		}

		internal unsafe bool TMK3wuKMfb()
		{
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			if (!DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4])
			{
				vnavmeshIPC.StopPathfind();
				_003C_003E4__this.inputController.Enabled = false;
				return true;
			}
			if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)77])
			{
				OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommandComplexLocation(ExecuteCommandComplexFlag.Dismount, Vector3.op_Implicit(((BattleChara)localPlayer).Position), RotationHelper.CharaToPacket(((BattleChara)localPlayer).Rotation));
			}
			else
			{
				OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.Dismount);
			}
			return false;
		}

		internal bool uOH3E6bhqB()
		{
			if (!Throttler.Shared.Throttle(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7C2F943E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_12c2d68ba4654622a4959ee73cacc6d7), 1000u))
			{
				return false;
			}
			if ((monsterInstance == null || monsterInstance.IsDead) && !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)26])
			{
				vnavmeshIPC.StopPathfind();
				_003C_003E4__this.inputController.Enabled = false;
				return true;
			}
			if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4])
			{
				_003C_003E4__this.TaskHelper.Enqueue((Action)_003C_003E4__this.tETsB259TB, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
				return true;
			}
			OmenServiceBase<ChatManager>.Instance().SendMessage(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x43D7D7B6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_974d377e1ed74986b81c34c95c2cc846));
			TargetManager.Target = monsterInstance;
			IGameObject target = TargetManager.Target;
			if (target != null)
			{
				AutoAttackCommand.Enable(target.EntityID);
			}
			return false;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass27_0
	{
		public AutoHuntingLog _003C_003E4__this;

		public unsafe BattleChara* localPlayer;

		public _003C_003Ec__DisplayClass27_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal unsafe void cGl3bRnPxS()
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			Vector3 groundPosition;
			if (!DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)77])
			{
				_003C_003E4__this.lastJumpPosition = Vector3.op_Implicit(((BattleChara)localPlayer).Position);
			}
			else if (RaycastHelper.TryGetGroundPosition(Vector3.op_Implicit(((BattleChara)localPlayer).Position), out groundPosition))
			{
				_003C_003E4__this.lastJumpPosition = groundPosition;
			}
		}

		internal bool mbc3FvUuXH()
		{
			if (!DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4])
			{
				_003C_003E4__this.TaskHelper.Abort();
				_003C_003E4__this.TaskHelper.Enqueue((Action)_003C_003E4__this.tETsB259TB, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
				return true;
			}
			if (!Control.CanFly)
			{
				return true;
			}
			if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)77])
			{
				return true;
			}
			OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)5, 2u, 3758096384uL, 0u, (UseActionMode)0);
			return false;
		}

		internal unsafe void G0C3q635ge()
		{
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			if (Control.CanFly)
			{
				Vector3 groundPosition;
				if (!DalamudServiceExtension.Instance<ICondition>().Any((ConditionFlag[])(object)new ConditionFlag[2]
				{
					(ConditionFlag)4,
					(ConditionFlag)77
				}))
				{
					_003C_003E4__this.TaskHelper.Abort();
					_003C_003E4__this.TaskHelper.Enqueue((Action)_003C_003E4__this.tETsB259TB, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
				}
				else if (!(MathF.Abs(((Vector3)(&((BattleChara)localPlayer).Position)).Y - _003C_003E4__this.lastJumpPosition.Y) > 1f) || !RaycastHelper.TryGetGroundPosition(Vector3.op_Implicit(((BattleChara)localPlayer).Position), out groundPosition) || !(MathF.Abs(groundPosition.Y - ((Vector3)(&((BattleChara)localPlayer).Position)).Y) > 1f))
				{
					Vector3 position = ((BattleChara)localPlayer).Position;
					OmenServiceBase<MovementManager>.Instance().TPSmart_InZone(new Vector3(position.X, position.Y + 9f, position.Z));
				}
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass34_0
	{
		public uint nameID;

		public _003C_003Ec__DisplayClass34_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal unsafe bool G3T3cnT2pw(IGameObject x)
		{
			if (x is IBattleNPC battleNPC && !x.IsDead && battleNPC.NameID == nameID)
			{
				return ((BattleChara)battleNPC.ToBCStruct()).FateId == 0;
			}
			return false;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass35_0
	{
		public Dictionary<uint, MonsterProgress> progressData;

		public Func<HuntingMonster, bool> _003C_003E9__3;

		public _003C_003Ec__DisplayClass35_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal IEnumerable<HuntingMonster> Xgj3AKX65Z(HuntingTask x)
		{
			return x.OpenWorldMonsters.Where((HuntingMonster m) => progressData.TryGetValue(m.ID, out var value) && !value.IsDone);
		}

		internal bool gRS3QWSPpv(HuntingMonster m)
		{
			if (progressData.TryGetValue(m.ID, out var value))
			{
				return !value.IsDone;
			}
			return false;
		}
	}

	private l6WSnC3BFMau5ODGRkx config;

	private MovementInputController? inputController;

	private int selectedCategory;

	private uint currentAutoJobID;

	private int[] currentAutoRankIndex;

	private Vector3 lastJumpPosition;

	private CancellationTokenSource? cancelSource;

	private static readonly FrozenSet<uint> ForceToWalkMonsters;

	private static readonly FrozenSet<string> Categories;

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected override void Init()
	{
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<l6WSnC3BFMau5ODGRkx>(this) ?? new l6WSnC3BFMau5ODGRkx();
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 180000
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		if (inputController == null)
		{
			inputController = new MovementInputController
			{
				Precision = 1f
			};
		}
		cancelSource = new CancellationTokenSource();
		IFramework obj2 = DalamudServiceExtension.Instance<IFramework>();
		Func<Task> func = async delegate
		{
			HuntingData huntingData = JsonConvert.DeserializeObject<HuntingData>(await BundledData.ReadHuntingAsync(OmenServiceBase<HTTPClientHelper>.Instance().Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-864210836 ^ -1164557619 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b988ddaee02846b69dd4e446b5da9de9)), LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7C2F9626 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_12c2d68ba4654622a4959ee73cacc6d7)).ConfigureAwait(continueOnCapturedContext: false), JsonSerializerSettingsExtension.GetShared());
			if (huntingData != null)
			{
				bool num = config.Data == null;
				config.Data = huntingData;
				config.Save(this);
				if (num)
				{
					OmenServiceBase<NotifyHelper>.Instance().NotificationSuccess(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3F365C56 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7a7485368e28474eb6d61987c51729a2)));
				}
			}
		};
		CancellationToken token = cancelSource.Token;
		obj2.RunOnTick(func, default(TimeSpan), 0, token);
		OmenServiceBase<FrameworkManager>.Instance().Reg(new OnUpdateDelegate(cmXswZiXvW), 2000u);
	}

	protected override void Uninit()
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		TaskHelper? taskHelper = base.TaskHelper;
		if (taskHelper != null && taskHelper.IsBusy)
		{
			base.TaskHelper.Abort();
			vnavmeshIPC.StopPathfind();
		}
		cancelSource?.Cancel();
		cancelSource?.Dispose();
		cancelSource = null;
		OmenServiceBase<FrameworkManager>.Instance().Unreg(cmXswZiXvW);
		if (inputController != null)
		{
			inputController.Dispose();
			inputController = null;
		}
	}

	private unsafe void tETsB259TB()
	{
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0618: Unknown result type (might be due to invalid IL or missing references)
		_003C_003Ec__DisplayClass24_0 CS_0024_003C_003E8__locals57 = new _003C_003Ec__DisplayClass24_0();
		CS_0024_003C_003E8__locals57._003C_003E4__this = this;
		base.TaskHelper.Abort();
		if (!Throttler.Shared.Throttle(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x9B4C1DC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2f1c0cb46b4440d499a69ed8afe22360), 1000u))
		{
			base.TaskHelper.Enqueue((Action)tETsB259TB, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
			return;
		}
		CS_0024_003C_003E8__locals57.localPlayer = Control.GetLocalPlayer();
		if (CS_0024_003C_003E8__locals57.localPlayer != null && !IConditionExtension.get_IsBetweenAreas(DalamudServiceExtension.Instance<ICondition>()) && UIModuleExtension.IsScreenReady() && !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)26])
		{
			if (!a0KsmQXMrr())
			{
				return;
			}
			List<HuntingMonster> list = VS3sFmbUo0(currentAutoJobID, currentAutoRankIndex[0]);
			if (list.Count == 0)
			{
				currentAutoRankIndex = currentAutoRankIndex.Skip(1).ToArray();
				if (a0KsmQXMrr())
				{
					base.TaskHelper.Enqueue((Action)tETsB259TB, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2DBD58A0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5d8d924f1edf4ec39f023b3c7bb646cc), 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
				}
				return;
			}
			CS_0024_003C_003E8__locals57.monster = list.First();
			CS_0024_003C_003E8__locals57.forceToWalk = ForceToWalkMonsters.Contains(CS_0024_003C_003E8__locals57.monster.ID);
			CS_0024_003C_003E8__locals57.localPlayer = Control.GetLocalPlayer();
			if (CS_0024_003C_003E8__locals57.localPlayer != null && !IConditionExtension.get_IsBetweenAreas(DalamudServiceExtension.Instance<ICondition>()) && UIModuleExtension.IsScreenReady() && !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)26])
			{
				if (!DalamudServiceExtension.Instance().IsPluginEnabled(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4753A97A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_790eb00575b3425495c924ac13469f4c)) || !vnavmeshIPC.GetIsNavReady())
				{
					base.TaskHelper.Enqueue((Action)tETsB259TB, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6E00CFC8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c40146824d9043e0906f4e1889cf39aa), 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
					return;
				}
				if (GameState.TerritoryType != CS_0024_003C_003E8__locals57.monster.GetLocation().Zone)
				{
					base.TaskHelper.Enqueue(() => OmenServiceBase<AetheryteRecordManager>.Instance().GetNearestAetheryte(CS_0024_003C_003E8__locals57.monster.GetLocation().Zone, CS_0024_003C_003E8__locals57.monster.GetLocation().WorldPosition)?.TeleportTo(), LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7FD8ED83 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1eab987c1db64b928220e27bd03141d3));
					base.TaskHelper.Enqueue(() => GameState.TerritoryType == CS_0024_003C_003E8__locals57.monster.GetLocation().Zone && UIModuleExtension.IsScreenReady(), LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x51E5EAC6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a90a92322bff42e180982e106200d4a3));
					base.TaskHelper.Enqueue((Action)tETsB259TB, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4B0FAC74 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_8e002b0644ef421698ef6f63772929af), 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
					return;
				}
				bool flag = ucLsbTfUp0(CS_0024_003C_003E8__locals57.monster.ID, out CS_0024_003C_003E8__locals57.monsterInstance);
				if ((!flag || !(Vector3.DistanceSquared(Vector3.op_Implicit(((BattleChara)CS_0024_003C_003E8__locals57.localPlayer).Position), CS_0024_003C_003E8__locals57.monsterInstance.Position) <= 400f) || !(MathF.Abs(((Vector3)(&((BattleChara)CS_0024_003C_003E8__locals57.localPlayer).Position)).Y - CS_0024_003C_003E8__locals57.monsterInstance.Position.Y) <= 4f)) && !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4] && !CS_0024_003C_003E8__locals57.forceToWalk)
				{
					if (((ActionManager)ActionManager.Instance()).GetActionStatus((ActionType)5, 9u, 3758096384uL, true, true, (uint*)null) == 0)
					{
						if (!CS_0024_003C_003E8__locals57.forceToWalk)
						{
							nYRsPX5dta();
						}
						base.TaskHelper.Enqueue((Action)tETsB259TB, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x369573CB ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_26e231ba68f74e8784503e099ddfc2d9), 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
						return;
					}
					CS_0024_003C_003E8__locals57.forceToWalk = true;
				}
				if (!flag)
				{
					CS_0024_003C_003E8__locals57.center = CS_0024_003C_003E8__locals57.monster.GetLocation().WorldPosition.ToVector2();
					if (Vector2.Distance(((BattleChara)CS_0024_003C_003E8__locals57.localPlayer).Position.ToVector2(), CS_0024_003C_003E8__locals57.center) > 30f && !vnavmeshIPC.GetIsPathfindRunning())
					{
						base.TaskHelper.Enqueue(delegate
						{
							Vector3? vector = vnavmeshIPC.QueryNearestPointOnMesh(CS_0024_003C_003E8__locals57.monster.GetLocation().WorldPosition, 30f, 10000f);
							if (!vector.HasValue)
							{
								string message = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x9B44194 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2f1c0cb46b4440d499a69ed8afe22360), CS_0024_003C_003E8__locals57.monster.GetName(), CS_0024_003C_003E8__locals57.monster.ID);
								OmenServiceBase<NotifyHelper>.Instance().NotificationWarning(message);
								CS_0024_003C_003E8__locals57.monster.GetLocation().SetFlagMarker();
								CS_0024_003C_003E8__locals57._003C_003E4__this.TaskHelper.Abort();
							}
							else
							{
								vnavmeshIPC.PathfindAndMoveTo(vector.Value, !CS_0024_003C_003E8__locals57.forceToWalk & Control.CanFly);
							}
						});
						base.TaskHelper.Enqueue(() => Vector2.Distance(((BattleChara)CS_0024_003C_003E8__locals57.localPlayer).Position.ToVector2(), CS_0024_003C_003E8__locals57.center) <= 100f);
						base.TaskHelper.Enqueue((Action)tETsB259TB, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5CE356BD ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3054c6e6ef664be696bb2ad005864818), 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
						return;
					}
					vnavmeshIPC.StopPathfind();
					if (!ucLsbTfUp0(CS_0024_003C_003E8__locals57.monster.ID, out CS_0024_003C_003E8__locals57.monsterInstance))
					{
						base.TaskHelper.Enqueue((Action)tETsB259TB, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4B0FACAC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_8e002b0644ef421698ef6f63772929af), 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
						return;
					}
				}
				if (Vector3.DistanceSquared(Vector3.op_Implicit(((BattleChara)CS_0024_003C_003E8__locals57.localPlayer).Position), CS_0024_003C_003E8__locals57.monsterInstance.Position) > 6f)
				{
					base.TaskHelper.Enqueue(delegate
					{
						//IL_0010: Unknown result type (might be due to invalid IL or missing references)
						//IL_0121: Unknown result type (might be due to invalid IL or missing references)
						if (Vector3.DistanceSquared(Vector3.op_Implicit(((BattleChara)CS_0024_003C_003E8__locals57.localPlayer).Position), CS_0024_003C_003E8__locals57.monsterInstance.Position) <= 6f)
						{
							return true;
						}
						if (!Throttler.Shared.Throttle(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4B0F2C76 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_8e002b0644ef421698ef6f63772929af), 1000u))
						{
							return false;
						}
						if (CS_0024_003C_003E8__locals57.monsterInstance == null || CS_0024_003C_003E8__locals57.monsterInstance.IsDead)
						{
							CS_0024_003C_003E8__locals57._003C_003E4__this.TaskHelper.Abort();
							CS_0024_003C_003E8__locals57._003C_003E4__this.TaskHelper.Enqueue((Action)CS_0024_003C_003E8__locals57._003C_003E4__this.tETsB259TB, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--1470179789 ^ 0x763587E3 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cd1b319f326a44d1aaea645e95d8698b), 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
							return true;
						}
						vnavmeshIPC.PathfindAndMoveTo(CS_0024_003C_003E8__locals57.monsterInstance.Position, (!CS_0024_003C_003E8__locals57.forceToWalk && DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4]) & Control.CanFly);
						return Vector3.DistanceSquared(Vector3.op_Implicit(((BattleChara)CS_0024_003C_003E8__locals57.localPlayer).Position), CS_0024_003C_003E8__locals57.monsterInstance.Position) <= 6f;
					}, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5357B302 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_0676edd1b45a4ff096830fd6229bc1fb));
				}
				base.TaskHelper.Enqueue(delegate
				{
					//IL_004d: Unknown result type (might be due to invalid IL or missing references)
					if (!DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4])
					{
						vnavmeshIPC.StopPathfind();
						CS_0024_003C_003E8__locals57._003C_003E4__this.inputController.Enabled = false;
						return true;
					}
					if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)77])
					{
						OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommandComplexLocation(ExecuteCommandComplexFlag.Dismount, Vector3.op_Implicit(((BattleChara)CS_0024_003C_003E8__locals57.localPlayer).Position), RotationHelper.CharaToPacket(((BattleChara)CS_0024_003C_003E8__locals57.localPlayer).Rotation));
					}
					else
					{
						OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.Dismount);
					}
					return false;
				}, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x1778CB63 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f7cf93b006904393bb56d82c0a688fa1));
				base.TaskHelper.Enqueue(delegate
				{
					if (!Throttler.Shared.Throttle(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7C2F943E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_12c2d68ba4654622a4959ee73cacc6d7), 1000u))
					{
						return false;
					}
					if ((CS_0024_003C_003E8__locals57.monsterInstance == null || CS_0024_003C_003E8__locals57.monsterInstance.IsDead) && !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)26])
					{
						vnavmeshIPC.StopPathfind();
						CS_0024_003C_003E8__locals57._003C_003E4__this.inputController.Enabled = false;
						return true;
					}
					if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4])
					{
						CS_0024_003C_003E8__locals57._003C_003E4__this.TaskHelper.Enqueue((Action)CS_0024_003C_003E8__locals57._003C_003E4__this.tETsB259TB, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
						return true;
					}
					OmenServiceBase<ChatManager>.Instance().SendMessage(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x43D7D7B6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_974d377e1ed74986b81c34c95c2cc846));
					TargetManager.Target = CS_0024_003C_003E8__locals57.monsterInstance;
					IGameObject target = TargetManager.Target;
					if (target != null)
					{
						AutoAttackCommand.Enable(target.EntityID);
					}
					return false;
				}, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--1470179789 ^ 0x1A3DD240 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3badc14f3c904cf4809aa58db382c8ee));
				if (Random.Shared.NextDouble() > 0.5)
				{
					base.TaskHelper.DelayNext(Random.Shared.Next(100, 1000), LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7FD8ECBF ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1eab987c1db64b928220e27bd03141d3));
				}
				base.TaskHelper.Enqueue((Action)tETsB259TB, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4CF24AAE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a07aac30eb32484e94cb4d1e62117765), 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
				return;
			}
		}
		base.TaskHelper.Enqueue((Action)tETsB259TB, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2310375F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a96fab9e97944b39a8afe3fb1109ba7d), 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
	}

	private bool a0KsmQXMrr()
	{
		if (currentAutoRankIndex.Length == 0)
		{
			OmenServiceBase<NotifyHelper>.Instance().NotificationInfo(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x258FF2CE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_dadeb2ab61424d31be7cc1966e675825), EL2sA4KfXG(currentAutoJobID)));
			return false;
		}
		int num = vo7sqsrZ6S(currentAutoJobID);
		while (currentAutoRankIndex.Length != 0 && currentAutoRankIndex[0] < num)
		{
			currentAutoRankIndex = currentAutoRankIndex.Skip(1).ToArray();
		}
		if (currentAutoRankIndex.Length == 0)
		{
			OmenServiceBase<NotifyHelper>.Instance().NotificationInfo(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x36957299 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_26e231ba68f74e8784503e099ddfc2d9), EL2sA4KfXG(currentAutoJobID)));
			return false;
		}
		currentAutoRankIndex[0] = num;
		if (RjcsiuYQom(currentAutoJobID, currentAutoRankIndex[0]))
		{
			OmenServiceBase<NotifyHelper>.Instance().NotificationInfo(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x591C7DB7 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_20724779fecb4a91959b8028d04d1118), EL2sA4KfXG(currentAutoJobID), currentAutoRankIndex[0] + 1));
			return false;
		}
		return true;
	}

	private bool RjcsiuYQom(uint jobID, int rankIndex)
	{
		if (config.Data == null || !config.Data.JobRanks.TryGetValue(jobID, out List<HuntingRank> value) || rankIndex >= value.Count)
		{
			return false;
		}
		Dictionary<uint, MonsterProgress> dictionary = h65scbFMsn(jobID, rankIndex);
		foreach (HuntingTask task in value[rankIndex].Tasks)
		{
			foreach (HuntingMonster item in task.Monsters.Where((HuntingMonster m) => !m.IsOpenWorld))
			{
				if (dictionary.TryGetValue(item.ID, out var value2) && !value2.IsDone)
				{
					return true;
				}
			}
		}
		return false;
	}

	private unsafe void nYRsPX5dta()
	{
		_003C_003Ec__DisplayClass27_0 CS_0024_003C_003E8__locals18 = new _003C_003Ec__DisplayClass27_0();
		CS_0024_003C_003E8__locals18._003C_003E4__this = this;
		CS_0024_003C_003E8__locals18.localPlayer = Control.GetLocalPlayer();
		if (CS_0024_003C_003E8__locals18.localPlayer == null)
		{
			return;
		}
		if (Random.Shared.NextDouble() > 0.5)
		{
			base.TaskHelper.DelayNext(Random.Shared.Next(100, 1000));
		}
		base.TaskHelper.Enqueue(delegate
		{
			if (!Throttler.Shared.Throttle(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3951FE75 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3aeae81f85f84848b273be7b2bfbf497)))
			{
				return false;
			}
			if (IConditionExtension.get_IsCasting(DalamudServiceExtension.Instance<ICondition>()))
			{
				return false;
			}
			if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4])
			{
				return true;
			}
			if (((ActionManager)ActionManager.Instance()).GetActionStatus((ActionType)5, 9u, 3758096384uL, true, true, (uint*)null) != 0)
			{
				return true;
			}
			OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)5, 9u, 3758096384uL, 0u, (UseActionMode)0);
			return false;
		}, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6E00CE34 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c40146824d9043e0906f4e1889cf39aa));
		base.TaskHelper.Enqueue(delegate
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			Vector3 groundPosition;
			if (!DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)77])
			{
				CS_0024_003C_003E8__locals18._003C_003E4__this.lastJumpPosition = Vector3.op_Implicit(((BattleChara)CS_0024_003C_003E8__locals18.localPlayer).Position);
			}
			else if (RaycastHelper.TryGetGroundPosition(Vector3.op_Implicit(((BattleChara)CS_0024_003C_003E8__locals18.localPlayer).Position), out groundPosition))
			{
				CS_0024_003C_003E8__locals18._003C_003E4__this.lastJumpPosition = groundPosition;
			}
		});
		base.TaskHelper.Enqueue(delegate
		{
			if (!DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4])
			{
				CS_0024_003C_003E8__locals18._003C_003E4__this.TaskHelper.Abort();
				CS_0024_003C_003E8__locals18._003C_003E4__this.TaskHelper.Enqueue((Action)CS_0024_003C_003E8__locals18._003C_003E4__this.tETsB259TB, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
				return true;
			}
			if (!Control.CanFly)
			{
				return true;
			}
			if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)77])
			{
				return true;
			}
			OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)5, 2u, 3758096384uL, 0u, (UseActionMode)0);
			return false;
		}, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x66B38F54 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_762f55d0e7ef4ecd8f3f55c0f8a10fae));
		base.TaskHelper.Enqueue(delegate
		{
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			if (Control.CanFly)
			{
				Vector3 groundPosition;
				if (!DalamudServiceExtension.Instance<ICondition>().Any((ConditionFlag[])(object)new ConditionFlag[2]
				{
					(ConditionFlag)4,
					(ConditionFlag)77
				}))
				{
					CS_0024_003C_003E8__locals18._003C_003E4__this.TaskHelper.Abort();
					CS_0024_003C_003E8__locals18._003C_003E4__this.TaskHelper.Enqueue((Action)CS_0024_003C_003E8__locals18._003C_003E4__this.tETsB259TB, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
				}
				else if (!(MathF.Abs(((Vector3)(&((BattleChara)CS_0024_003C_003E8__locals18.localPlayer).Position)).Y - CS_0024_003C_003E8__locals18._003C_003E4__this.lastJumpPosition.Y) > 1f) || !RaycastHelper.TryGetGroundPosition(Vector3.op_Implicit(((BattleChara)CS_0024_003C_003E8__locals18.localPlayer).Position), out groundPosition) || !(MathF.Abs(groundPosition.Y - ((Vector3)(&((BattleChara)CS_0024_003C_003E8__locals18.localPlayer).Position)).Y) > 1f))
				{
					Vector3 position = ((BattleChara)CS_0024_003C_003E8__locals18.localPlayer).Position;
					OmenServiceBase<MovementManager>.Instance().TPSmart_InZone(new Vector3(position.X, position.Y + 9f, position.Z));
				}
			}
		});
	}

	protected override void ConfigUI()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector;
		if (config.Data == null)
		{
			vector = KnownColor.Firebrick.ToVector4();
			ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3AC92A40 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3932528062874ea4bb7e2e1185394e37))));
			return;
		}
		bool flag = DalamudServiceExtension.Instance().IsPluginEnabled(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5A7FFE73 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5154ccd9fed0487f8fa0244531466009));
		ColorDisposable val = ImRaii.PushColor((ImGuiCol)0, flag ? KnownColor.GreenYellow.ToVector4() : KnownColor.OrangeRed.ToVector4(), true);
		try
		{
			ImGui.TextUnformatted(ImU8String.op_Implicit(flag ? LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x423E82F4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_6368ba44fa4749c09da09b5ebf853b71) : LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x9B485A0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2f1c0cb46b4440d499a69ed8afe22360)));
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		ImGui.SameLine();
		LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--1317373447 ^ 0x22B09D61 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88a2dcd1996b4a3fbefaf51a98b59def);
		(new object[1])[0] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x258FB45A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_dadeb2ab61424d31be7cc1966e675825);
		ImGui.TextUnformatted(ImU8String.op_Implicit("vnavmesh needs to be installed and enabled"));
		vector = KnownColor.Yellow.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3AC96CD6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3932528062874ea4bb7e2e1185394e37)));
		ImGui.SameLine();
		LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3987F311 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_56ae6e3f2cdf4dd8a743f4d493ce49a5);
		ImGui.TextUnformatted(ImU8String.op_Implicit("bossmod AI and autorotation (vbm ar/vbm ai on) need to be installed and enabled"));
		ImGui.NewLine();
		TabBarDisposable val2 = ImRaii.TabBar(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7FD8EB23 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1eab987c1db64b928220e27bd03141d3)));
		try
		{
			if (!val2)
			{
				return;
			}
			int num = -1;
			foreach (string category in Categories)
			{
				num++;
				TabItemDisposable val3 = ImRaii.TabItem(ImU8String.op_Implicit(category));
				try
				{
					if (TabItemDisposable.op_Implicit(val3))
					{
						selectedCategory = num;
					}
				}
				finally
				{
					((TabItemDisposable)(ref val3)).Dispose();
				}
			}
		}
		finally
		{
			((TabBarDisposable)(ref val2)).Dispose();
		}
		ImGui.Spacing();
		if (selectedCategory == 0)
		{
			A89sh3Y9p9();
		}
		else
		{
			HeHsoFXD3X();
		}
	}

	private void A89sh3Y9p9()
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		foreach (var (num2, list2) in config.Data.JobRanks)
		{
			if (num2 >= 10000)
			{
				continue;
			}
			string text = EL2sA4KfXG(num2);
			int num3 = vo7sqsrZ6S(num2);
			ImU8String val = new ImU8String(11, 4);
			((ImU8String)(ref val)).AppendFormatted<string>(text);
			((ImU8String)(ref val)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7FD88BAF ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1eab987c1db64b928220e27bd03141d3));
			((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5351F6EE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_9085260e6b1a40438d219e85182b5d08)));
			((ImU8String)(ref val)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x12D67D90 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7fcf5a71a7ed445197f132979519c560));
			((ImU8String)(ref val)).AppendFormatted<int>(num3);
			((ImU8String)(ref val)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x68548163 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_23522a2a14314da28eb44187a09cd0b5));
			((ImU8String)(ref val)).AppendFormatted<uint>(num2);
			if (ImGui.CollapsingHeader(val, (ImGuiTreeNodeFlags)0))
			{
				for (int i = 0; i < list2.Count; i++)
				{
					WIAsgXptNv(num2, i, list2[i], num3);
				}
			}
		}
	}

	private void HeHsoFXD3X()
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		foreach (var (num2, list2) in config.Data.JobRanks)
		{
			if (num2 < 10000)
			{
				continue;
			}
			string text = EL2sA4KfXG(num2);
			int num3 = vo7sqsrZ6S(num2);
			ImU8String val = new ImU8String(20, 4);
			((ImU8String)(ref val)).AppendFormatted<string>(text);
			((ImU8String)(ref val)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x66B3E8AE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_762f55d0e7ef4ecd8f3f55c0f8a10fae));
			((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4B0FAADC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_8e002b0644ef421698ef6f63772929af)));
			((ImU8String)(ref val)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x258FB0E4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_dadeb2ab61424d31be7cc1966e675825));
			((ImU8String)(ref val)).AppendFormatted<int>(num3);
			((ImU8String)(ref val)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x45E45E2D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1904f95d2a394c929e1a6d4f72943ad7));
			((ImU8String)(ref val)).AppendFormatted<uint>(num2);
			if (ImGui.CollapsingHeader(val, (ImGuiTreeNodeFlags)0))
			{
				for (int i = 0; i < list2.Count; i++)
				{
					WIAsgXptNv(num2, i, list2[i], num3);
				}
			}
		}
	}

	private unsafe void WIAsgXptNv(uint jobID, int rankIndex, HuntingRank rank, int currentRank)
	{
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0624: Unknown result type (might be due to invalid IL or missing references)
		//IL_062f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0634: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0636: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0470: Unknown result type (might be due to invalid IL or missing references)
		//IL_0668: Unknown result type (might be due to invalid IL or missing references)
		//IL_0699: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_070c: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0811: Unknown result type (might be due to invalid IL or missing references)
		//IL_092a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0900: Unknown result type (might be due to invalid IL or missing references)
		//IL_0963: Unknown result type (might be due to invalid IL or missing references)
		//IL_09bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c2: Unknown result type (might be due to invalid IL or missing references)
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
		defaultInterpolatedStringHandler.AppendFormatted(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x27D1D755 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_39d5d8025f5249e6b489bc374eefbb3c)));
		defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6AAE2FCC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_839f542231154d29b0562b687afa328a));
		defaultInterpolatedStringHandler.AppendFormatted(rankIndex + 1);
		string text = defaultInterpolatedStringHandler.ToStringAndClear();
		Vector4 zero = Vector4.Zero;
		string empty = string.Empty;
		if (currentRank > rankIndex)
		{
			empty = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3AC92C96 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3932528062874ea4bb7e2e1185394e37));
			zero = KnownColor.GreenYellow.ToVector4();
		}
		else if (currentRank < rankIndex)
		{
			empty = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2DBD5C74 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5d8d924f1edf4ec39f023b3c7bb646cc));
			zero = KnownColor.Gray.ToVector4();
		}
		else
		{
			empty = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4753E996 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_790eb00575b3425495c924ac13469f4c));
			zero = KnownColor.Gold.ToVector4();
		}
		bool flag = false;
		ColorDisposable val = ImRaii.PushColor((ImGuiCol)0, zero, true);
		try
		{
			ImU8String val2 = new ImU8String(11, 4);
			((ImU8String)(ref val2)).AppendFormatted<string>(text);
			((ImU8String)(ref val2)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6FF1E29A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5859f4ea3042459db2c9b610b3099912));
			((ImU8String)(ref val2)).AppendFormatted<string>(empty);
			((ImU8String)(ref val2)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x258FF48E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_dadeb2ab61424d31be7cc1966e675825));
			((ImU8String)(ref val2)).AppendFormatted<uint>(jobID);
			((ImU8String)(ref val2)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x469A0EBB ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_bb1eb496bad948e4a2baaa3e1edc7a92));
			((ImU8String)(ref val2)).AppendFormatted<int>(rankIndex);
			flag = ImGui.TreeNode(val2);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		if (!flag)
		{
			return;
		}
		if (currentRank == rankIndex)
		{
			ImGui.AlignTextToFramePadding();
			ImU8String val3 = default(ImU8String);
			((ImU8String)(ref val3))._002Ector(1, 1);
			((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5DD74687 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c7355392ad9e43b6b82798cc42ea6c6f)));
			((ImU8String)(ref val3)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x12D67E98 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7fcf5a71a7ed445197f132979519c560));
			ImGui.TextUnformatted(val3);
			DisabledDisposable val4 = ImRaii.Disabled(base.TaskHelper.IsBusy);
			try
			{
				ImGui.SameLine();
				ImU8String val5 = new ImU8String(18, 5);
				((ImU8String)(ref val5)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5357F17A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_0676edd1b45a4ff096830fd6229bc1fb)));
				((ImU8String)(ref val5)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x18CCAFB4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_33babcee9ecc460a9e530039fd18d361));
				((ImU8String)(ref val5)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3D56EDFE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e9e296e241f44828a57c750102ce71c1)));
				((ImU8String)(ref val5)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x469A25D3 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_bb1eb496bad948e4a2baaa3e1edc7a92));
				((ImU8String)(ref val5)).AppendFormatted<int>(rankIndex + 1);
				((ImU8String)(ref val5)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x469A43CF ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_bb1eb496bad948e4a2baaa3e1edc7a92));
				((ImU8String)(ref val5)).AppendFormatted<uint>(jobID);
				((ImU8String)(ref val5)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x43D71D94 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_974d377e1ed74986b81c34c95c2cc846));
				((ImU8String)(ref val5)).AppendFormatted<int>(rankIndex);
				if (ImGui.Button(val5, default(Vector2)))
				{
					currentAutoJobID = jobID;
					currentAutoRankIndex = new int[1] { rankIndex };
					tETsB259TB();
				}
				ImGui.SameLine();
				ImU8String val6 = new ImU8String(20, 4);
				((ImU8String)(ref val6)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7EE7AFCF ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88432e96079448078bdf00e7b62f8ab6)));
				((ImU8String)(ref val6)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x12D65F34 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7fcf5a71a7ed445197f132979519c560));
				((ImU8String)(ref val6)).AppendFormatted<string>(EL2sA4KfXG(jobID));
				((ImU8String)(ref val6)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x36957469 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_26e231ba68f74e8784503e099ddfc2d9));
				((ImU8String)(ref val6)).AppendFormatted<uint>(jobID);
				((ImU8String)(ref val6)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4CF2017E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a07aac30eb32484e94cb4d1e62117765));
				((ImU8String)(ref val6)).AppendFormatted<int>(rankIndex);
				if (ImGui.Button(val6, default(Vector2)))
				{
					if (config.Data == null || !config.Data.JobRanks.TryGetValue(jobID, out List<HuntingRank> value))
					{
						return;
					}
					List<int> list = new List<int>();
					for (int i = rankIndex; i < value.Count; i++)
					{
						list.Add(i);
					}
					if (list.Count == 0)
					{
						return;
					}
					currentAutoJobID = jobID;
					currentAutoRankIndex = list.ToArray();
					tETsB259TB();
				}
			}
			finally
			{
				((IDisposable)val4)?.Dispose();
			}
			ImGui.SameLine();
			ImU8String val7 = default(ImU8String);
			((ImU8String)(ref val7))._002Ector(8, 3);
			((ImU8String)(ref val7)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6C351E02 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88a2dcd1996b4a3fbefaf51a98b59def)));
			((ImU8String)(ref val7)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x43D7535A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_974d377e1ed74986b81c34c95c2cc846));
			((ImU8String)(ref val7)).AppendFormatted<uint>(jobID);
			((ImU8String)(ref val7)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489FD7F2 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b2294f2f9bb24a76b00cf1af75888414));
			((ImU8String)(ref val7)).AppendFormatted<int>(rankIndex);
			if (ImGui.Button(val7, default(Vector2)))
			{
				base.TaskHelper.Abort();
				inputController.Enabled = false;
				vnavmeshIPC.StopPathfind();
			}
		}
		Dictionary<uint, MonsterProgress> dictionary = h65scbFMsn(jobID, rankIndex);
		List<HuntingMonster> allMonsters = rank.AllMonsters;
		if (allMonsters.Count > 0)
		{
			TableDisposable val8 = ImRaii.Table(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4CF24FAE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a07aac30eb32484e94cb4d1e62117765)), 3, (ImGuiTableFlags)1984);
			try
			{
				if (TableDisposable.op_Implicit(val8))
				{
					ImGui.TableSetupColumn(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5CE3088B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3054c6e6ef664be696bb2ad005864818))), (ImGuiTableColumnFlags)4, 0f, 0u);
					ImGui.TableSetupColumn(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x12D66036 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7fcf5a71a7ed445197f132979519c560))), (ImGuiTableColumnFlags)8, ImGui.CalcTextSize(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489FC398 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b2294f2f9bb24a76b00cf1af75888414))), false, -1f).X * 1.2f, 0u);
					ImGui.TableSetupColumn(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5A7FBDF5 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5154ccd9fed0487f8fa0244531466009))), (ImGuiTableColumnFlags)4, 0f, 0u);
					ImGui.TableHeadersRow();
					foreach (HuntingMonster item in allMonsters)
					{
						ImGui.TableNextRow();
						ImGui.TableNextColumn();
						Vector4 vector = KnownColor.LightGray.ToVector4();
						if (dictionary.TryGetValue(item.ID, out var value2))
						{
							vector = (value2.IsDone ? KnownColor.GreenYellow.ToVector4() : ((value2.KilledCount > 0) ? KnownColor.Gold.ToVector4() : KnownColor.LightGray.ToVector4()));
						}
						val = ImRaii.PushColor((ImGuiCol)0, vector, true);
						try
						{
							ImU8String val9 = new ImU8String(3, 2);
							((ImU8String)(ref val9)).AppendFormatted<string>(item.GetName());
							((ImU8String)(ref val9)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--1470179789 ^ 0x689790E7 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7a7485368e28474eb6d61987c51729a2));
							((ImU8String)(ref val9)).AppendFormatted<uint>(item.ID);
							((ImU8String)(ref val9)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x64E515F8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f53034f3efc64a309b10c7885bf5d9bd));
							ImGui.TextUnformatted(val9);
						}
						finally
						{
							((IDisposable)val)?.Dispose();
						}
						ImGui.TableNextColumn();
						if (dictionary.TryGetValue(item.ID, out var value3))
						{
							string text2;
							if (!value3.IsDone)
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(1, 2);
								defaultInterpolatedStringHandler2.AppendFormatted(value3.KilledCount);
								defaultInterpolatedStringHandler2.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x50173990 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e227921f7ee94b6aa1c28169395ba0cf));
								defaultInterpolatedStringHandler2.AppendFormatted(item.Count);
								text2 = defaultInterpolatedStringHandler2.ToStringAndClear();
							}
							else
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(1, 2);
								defaultInterpolatedStringHandler3.AppendFormatted(item.Count);
								defaultInterpolatedStringHandler3.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x50173990 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e227921f7ee94b6aa1c28169395ba0cf));
								defaultInterpolatedStringHandler3.AppendFormatted(item.Count);
								text2 = defaultInterpolatedStringHandler3.ToStringAndClear();
							}
							ImGui.TextUnformatted(ImU8String.op_Implicit(text2));
						}
						else
						{
							ImGui.TextUnformatted(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4B0FA842 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_8e002b0644ef421698ef6f63772929af)));
						}
						ImGui.TableNextColumn();
						ImGui.Selectable(ImU8String.op_Implicit(item.IsOpenWorld ? item.GetLocation().LocationDisplay : item.GetLocation().DutyName), false, (ImGuiSelectableFlags)0, default(Vector2));
						if (ImGui.IsItemHovered())
						{
							ImGui.SetMouseCursor((ImGuiMouseCursor)7);
						}
						if (ImGui.IsItemClicked())
						{
							if (item.IsOpenWorld)
							{
								item.GetLocation().SetFlagMarker();
								continue;
							}
							AgentContentsFinder* intPtr = AgentContentsFinder.Instance();
							ContentFinderCondition content = item.GetLocation().GetContent();
							((AgentContentsFinder)intPtr).OpenRegularDuty(((ContentFinderCondition)(ref content)).RowId, false);
						}
					}
				}
			}
			finally
			{
				((TableDisposable)(ref val8)).Dispose();
			}
		}
		ImGui.TreePop();
	}

	private void cmXswZiXvW(IFramework framework)
	{
		if (base.TaskHelper.IsBusy && GameState.IsLoggedIn && !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4])
		{
			IGameObject target = TargetManager.Target;
			if (target != null && !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)27])
			{
				vnavmeshIPC.StopPathfind();
				inputController.DesiredPosition = target.Position;
				inputController.Enabled = true;
				return;
			}
		}
		inputController.Enabled = false;
		inputController.DesiredPosition = default(Vector3);
	}

	private static int rfRsEUbFBi(uint key)
	{
		return key switch
		{
			1u => 0, 
			2u => 1, 
			3u => 2, 
			4u => 3, 
			5u => 4, 
			6u => 5, 
			7u => 6, 
			26u => 7, 
			10001u => 8, 
			10002u => 9, 
			10003u => 10, 
			29u => 11, 
			_ => 0, 
		};
	}

	private unsafe static bool ucLsbTfUp0(uint nameID, [NotNullWhen(true)] out IBattleNPC? monster)
	{
		_003C_003Ec__DisplayClass34_0 CS_0024_003C_003E8__locals2 = new _003C_003Ec__DisplayClass34_0();
		CS_0024_003C_003E8__locals2.nameID = nameID;
		monster = (IBattleNPC)(from x in IOmenDalamudService<IObjectTable>.Instance()
			where x is IBattleNPC battleNPC && !x.IsDead && battleNPC.NameID == CS_0024_003C_003E8__locals2.nameID && ((BattleChara)battleNPC.ToBCStruct()).FateId == 0
			orderby Vector3.DistanceSquared(IOmenDalamudService<IObjectTable>.Instance().LocalPlayer.Position, x.Position)
			select x).FirstOrDefault();
		return monster != null;
	}

	private unsafe List<HuntingMonster> VS3sFmbUo0(uint jobID, int rankIndex)
	{
		_003C_003Ec__DisplayClass35_0 CS_0024_003C_003E8__locals2 = new _003C_003Ec__DisplayClass35_0();
		if (config.Data == null || !config.Data.JobRanks.TryGetValue(jobID, out List<HuntingRank> value) || rankIndex >= value.Count)
		{
			return new List<HuntingMonster>();
		}
		CS_0024_003C_003E8__locals2.progressData = h65scbFMsn(jobID, rankIndex);
		HuntingRank huntingRank = value[rankIndex];
		new List<HuntingMonster>();
		return (from x in huntingRank.Tasks.SelectMany((HuntingTask x) => x.OpenWorldMonsters.Where((HuntingMonster m) => CS_0024_003C_003E8__locals2.progressData.TryGetValue(m.ID, out var value2) && !value2.IsDone)).ToList()
			orderby x.GetLocation().Zone == GameState.TerritoryType descending
			select x).ThenBy(delegate(HuntingMonster x)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			BattleChara* localPlayer = Control.GetLocalPlayer();
			return (localPlayer == null) ? float.MaxValue : Vector3.DistanceSquared(Vector3.op_Implicit(((BattleChara)localPlayer).Position), x.GetLocation().WorldPosition);
		}).ToList();
	}

	private unsafe static int vo7sqsrZ6S(uint job)
	{
		return ((MonsterNoteManager)MonsterNoteManager.Instance()).RankData[rfRsEUbFBi(job)].Rank;
	}

	private unsafe Dictionary<uint, MonsterProgress> h65scbFMsn(uint job, int rank)
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<uint, MonsterProgress> dictionary = new Dictionary<uint, MonsterProgress>();
		if (config.Data == null || !config.Data.JobRanks.TryGetValue(job, out List<HuntingRank> value) || rank >= value.Count)
		{
			return dictionary;
		}
		HuntingRank huntingRank = value[rank];
		MonsterNoteRankInfo val = ((MonsterNoteManager)MonsterNoteManager.Instance()).RankData[rfRsEUbFBi(job)];
		int rank2 = val.Rank;
		if (rank2 > rank)
		{
			foreach (HuntingMonster item3 in huntingRank.Tasks.SelectMany((HuntingTask a) => a.Monsters))
			{
				dictionary.Add(item3.ID, new MonsterProgress(isDone: true));
			}
		}
		else if (rank2 < rank)
		{
			foreach (HuntingMonster item4 in huntingRank.Tasks.SelectMany((HuntingTask a) => a.Monsters))
			{
				dictionary.Add(item4.ID, new MonsterProgress(0));
			}
		}
		else
		{
			foreach (var item5 in huntingRank.Tasks.Zip(((MonsterNoteRankInfo)(ref val)).RankData.ToArray()))
			{
				var (huntingTask, val2) = item5;
				foreach (var item6 in huntingTask.Monsters.Select((HuntingMonster monster, int i) => (monster: monster, i: i)))
				{
					HuntingMonster item = item6.monster;
					int item2 = item6.i;
					byte b = ((RankData)(ref val2)).Counts[item2];
					dictionary.Add(item.ID, (b == item.Count) ? new MonsterProgress(isDone: true) : new MonsterProgress(b));
				}
			}
		}
		return dictionary;
	}

	private static string EL2sA4KfXG(uint jobID)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		if (jobID < 10000)
		{
			return LuminaWrapper.GetJobName(jobID);
		}
		GrandCompany? row = LuminaGetter.GetRow<GrandCompany>(jobID % 10000);
		object obj;
		if (!row.HasValue)
		{
			obj = null;
		}
		else
		{
			GrandCompany valueOrDefault = row.GetValueOrDefault();
			obj = ((object)((GrandCompany)(ref valueOrDefault)).Name/*cast due to constrained. prefix*/).ToString();
		}
		if (obj == null)
		{
			obj = string.Empty;
		}
		return (string)obj;
	}

	public AutoHuntingLog()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7C2F10F4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_12c2d68ba4654622a4959ee73cacc6d7)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2DBD5FC2 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5d8d924f1edf4ec39f023b3c7bb646cc)),
			Category = ModuleCategory.Script,
			ModulesConflict = new string[2]
			{
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3F36DBFA ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7a7485368e28474eb6d61987c51729a2),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-1675397288 ^ -966997425 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5154ccd9fed0487f8fa0244531466009)
			}
		};
		Permission = new ModulePermission
		{
			NeedAuth = true
		};
		base._002Ector();
	}

	static AutoHuntingLog()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		ForceToWalkMonsters = FrozenSet.Create<uint>(375u, 371u, 1834u, 377u, 1832u, 1833u, 562u, 1835u);
		InlineArray2<string> buffer = default(InlineArray2<string>);
		buffer[0] = LuminaWrapper.GetAddonText(294u);
		buffer[1] = LuminaWrapper.GetAchievementKindName(12u);
		Categories = FrozenSet.Create<string>(buffer);
	}

	[CompilerGenerated]
	private async Task Xk9sQSWqeq()
	{
		HuntingData huntingData = JsonConvert.DeserializeObject<HuntingData>(await BundledData.ReadHuntingAsync(OmenServiceBase<HTTPClientHelper>.Instance().Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-864210836 ^ -1164557619 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b988ddaee02846b69dd4e446b5da9de9)), LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7C2F9626 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_12c2d68ba4654622a4959ee73cacc6d7)).ConfigureAwait(continueOnCapturedContext: false), JsonSerializerSettingsExtension.GetShared());
		if (huntingData != null)
		{
			bool num = config.Data == null;
			config.Data = huntingData;
			config.Save(this);
			if (num)
			{
				OmenServiceBase<NotifyHelper>.Instance().NotificationSuccess(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3F365C56 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7a7485368e28474eb6d61987c51729a2)));
			}
		}
	}
}
