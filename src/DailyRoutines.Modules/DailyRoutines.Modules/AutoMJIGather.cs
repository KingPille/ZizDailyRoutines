using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Interface;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.MJI;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;
using Newtonsoft.Json;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Info.Game.Packets.Upstream;
using OmenTools.Interop.Game.Helpers;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules;

public class AutoMJIGather : ModuleBase
{
	private sealed class BDNC7pxDeDBwVp6JNE8 : IEquatable<BDNC7pxDeDBwVp6JNE8>, IComparable<BDNC7pxDeDBwVp6JNE8>
	{
		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass0_0
		{
			public uint eobjID;

			public BDNC7pxDeDBwVp6JNE8 _003C_003E4__this;

			public _003C_003Ec__DisplayClass0_0()
			{
				UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
				base._002Ector();
			}

			internal string L9tPBuprqy()
			{
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				//IL_002d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				if (eobjID != 0)
				{
					EObjName value = LuminaGetter.GetRow<EObjName>(_003C_003E4__this.EObjID).Value;
					return ((object)((EObjName)(ref value)).Singular/*cast due to constrained. prefix*/).ToString();
				}
				return string.Empty;
			}
		}

		public uint EObjID { get; set; }

		public bool Enabled { get; set; }

		public List<Vector3> Points { get; set; }

		[JsonIgnore]
		public Lazy<string> Name { get; }

		public BDNC7pxDeDBwVp6JNE8(uint eobjID)
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			Points = new List<Vector3>();
			_003C_003Ec__DisplayClass0_0 CS_0024_003C_003E8__locals5 = new _003C_003Ec__DisplayClass0_0();
			CS_0024_003C_003E8__locals5.eobjID = eobjID;
			base._002Ector();
			CS_0024_003C_003E8__locals5._003C_003E4__this = this;
			EObjID = CS_0024_003C_003E8__locals5.eobjID;
			Name = new Lazy<string>(delegate
			{
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				//IL_002d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				if (CS_0024_003C_003E8__locals5.eobjID != 0)
				{
					EObjName value = LuminaGetter.GetRow<EObjName>(CS_0024_003C_003E8__locals5._003C_003E4__this.EObjID).Value;
					return ((object)((EObjName)(ref value)).Singular/*cast due to constrained. prefix*/).ToString();
				}
				return string.Empty;
			});
		}

		public int CompareTo(BDNC7pxDeDBwVp6JNE8? other)
		{
			if (other != null)
			{
				return EObjID.CompareTo(other.EObjID);
			}
			return 1;
		}

		public bool Equals(BDNC7pxDeDBwVp6JNE8? other)
		{
			if (other != null)
			{
				return EObjID == other.EObjID;
			}
			return false;
		}

		public override bool Equals(object? obj)
		{
			return Equals(obj as BDNC7pxDeDBwVp6JNE8);
		}

		public override int GetHashCode()
		{
			return EObjID.GetHashCode();
		}
	}

	private class XvwAI6xeqJP4aphdTWO : ModuleConfig
	{
		public List<BDNC7pxDeDBwVp6JNE8> IslandGatherPoints;

		public bool StopWhenReachingCap;

		public XvwAI6xeqJP4aphdTWO()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			IslandGatherPoints = new List<BDNC7pxDeDBwVp6JNE8>();
			StopWhenReachingCap = true;
			base._002Ector();
		}
	}

	private enum NodeState
	{
		NotFound,
		Gathered,
		Available,
		NotReady
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass17_0
	{
		public Vector3 currentNode;

		public IPlayerCharacter localPlayer;

		public AutoMJIGather _003C_003E4__this;

		public Queue<Vector3> nodes;

		public Action _003C_003E9__8;

		public _003C_003Ec__DisplayClass17_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal void pWkxN1xxGA()
		{
			Vector3 vector = currentNode;
			vector.Y = currentNode.Y - 1f;
			Vector3 vector2 = vector;
			if (MovementManager.IsNeedEnvironmentChange(localPlayer.Position, vector2) == true)
			{
				_003C_003E4__this.TaskHelper.Enqueue(() => IConditionExtension.get_IsBetweenAreas(DalamudServiceExtension.Instance<ICondition>()), null, 0, null, null, null, null, 1);
				_003C_003E4__this.TaskHelper.Enqueue(() => !IConditionExtension.get_IsBetweenAreas(DalamudServiceExtension.Instance<ICondition>()) && UIModuleExtension.IsScreenReady(), null, 0, null, null, null, null, 1);
				_003C_003E4__this.TaskHelper.DelayNext(1000, null, 1);
			}
			PositionUpdatePacket.Send(localPlayer.Rotation, vector2);
			OmenServiceBase<MovementManager>.Instance().TPSmart_InZone(vector2, new TPSmartParams
			{
				CancelAnimation = false
			});
		}

		internal unsafe bool Fjgx7nXmji()
		{
			_003C_003Ec__DisplayClass17_1 CS_0024_003C_003E8__locals2 = new _003C_003Ec__DisplayClass17_1();
			switch (IsgMGi5AY2(currentNode, out CS_0024_003C_003E8__locals2.obj))
			{
			case NodeState.NotReady:
				return false;
			case NodeState.NotFound:
				return true;
			case NodeState.Gathered:
				nodes.Enqueue(currentNode);
				return true;
			case NodeState.Available:
				_003C_003E4__this.TaskHelper.Enqueue(delegate
				{
					((EventFramework)EventFramework.Instance()).InteractWithReactionEventObject(CS_0024_003C_003E8__locals2.obj.ToStruct());
				}, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6FF10F3A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5859f4ea3042459db2c9b610b3099912), 0, null, null, null, null, 1);
				_003C_003E4__this.TaskHelper.Enqueue(() => DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)32], LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x68AC8DD2 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4722689f1a4d41dc9dc800bfe1a89c09), 0, null, null, null, null, 1);
				_003C_003E4__this.TaskHelper.Enqueue(() => !gDxM6oBJQm(), LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3F3654F6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7a7485368e28474eb6d61987c51729a2), 0, null, null, null, null, 1);
				return true;
			default:
				return true;
			}
		}

		internal void KSyxdBv60B()
		{
			_003C_003E4__this.TaskHelper.Abort();
			_003C_003E4__this.TaskHelper.Enqueue(delegate
			{
				_003C_003E4__this.lBkMQcdJAb(nodes);
			});
		}

		internal void ywqxIiIZTo()
		{
			_003C_003E4__this.lBkMQcdJAb(nodes);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass17_1
	{
		public IGameObject obj;

		public _003C_003Ec__DisplayClass17_1()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal unsafe void hBgxUfEgMS()
		{
			((EventFramework)EventFramework.Instance()).InteractWithReactionEventObject(obj.ToStruct());
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass18_0
	{
		public AutoMJIGather _003C_003E4__this;

		public List<Vector3> nodes;

		public Action _003C_003E9__2;

		public _003C_003Ec__DisplayClass18_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal unsafe void sdKxuUO18x()
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Invalid comparison between Unknown and I4
			foreach (IGameObject item in IOmenDalamudService<IObjectTable>.Instance())
			{
				_003C_003Ec__DisplayClass18_2 CS_0024_003C_003E8__locals5 = new _003C_003Ec__DisplayClass18_2();
				if ((int)item.ObjectKind != 14 || item.DataID == 0 || item.DataID == 2013159)
				{
					continue;
				}
				GameObject* ptr = item.ToStruct();
				if (ptr == null || ((GameObject)ptr).GetNameId() < 2000000)
				{
					continue;
				}
				CS_0024_003C_003E8__locals5.newGatherPoint = new BDNC7pxDeDBwVp6JNE8(((GameObject)ptr).GetNameId());
				if (!_003C_003E4__this.config.IslandGatherPoints.Contains(CS_0024_003C_003E8__locals5.newGatherPoint))
				{
					_003C_003E4__this.config.IslandGatherPoints.Add(CS_0024_003C_003E8__locals5.newGatherPoint);
					CS_0024_003C_003E8__locals5.newGatherPoint.Points.Add(item.Position);
					continue;
				}
				BDNC7pxDeDBwVp6JNE8 bDNC7pxDeDBwVp6JNE = _003C_003E4__this.config.IslandGatherPoints.Find((BDNC7pxDeDBwVp6JNE8 p) => p.Equals(CS_0024_003C_003E8__locals5.newGatherPoint));
				if (!bDNC7pxDeDBwVp6JNE.Points.Contains(item.Position))
				{
					bDNC7pxDeDBwVp6JNE.Points.Add(item.Position);
				}
			}
		}

		internal void X2OxXGA1Ri()
		{
			_003C_003E4__this.TaskHelper.Abort();
			_003C_003E4__this.config.Save(_003C_003E4__this);
			_003C_003E4__this.isOnDataCollecting = false;
			_003C_003E4__this.TaskHelper.Enqueue(() => OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)5, 27u, 3758096384uL, 0u, (UseActionMode)0));
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass18_1
	{
		public Vector3 node;

		public int index;

		public _003C_003Ec__DisplayClass18_0 CS_0024_003C_003E8__locals1;

		public _003C_003Ec__DisplayClass18_1()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal bool prfxYGg5nw()
		{
			if (!UIModuleExtension.IsScreenReady())
			{
				return false;
			}
			OmenServiceBase<MovementManager>.Instance().TPSmart_InZone(node, new TPSmartParams
			{
				CancelAnimation = false
			});
			return true;
		}

		internal void N6rxzGLkDO()
		{
			NotifyHelper notifyHelper = OmenServiceBase<NotifyHelper>.Instance();
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 3);
			defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x50177914 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e227921f7ee94b6aa1c28169395ba0cf));
			defaultInterpolatedStringHandler.AppendFormatted(index + 1);
			defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489BD79C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cade14e68bb84906959f3cda5c496848));
			defaultInterpolatedStringHandler.AppendFormatted(CS_0024_003C_003E8__locals1.nodes.Count);
			defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2194A566 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cd1b319f326a44d1aaea645e95d8698b));
			defaultInterpolatedStringHandler.AppendFormatted(node);
			defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4B0F27FE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_8e002b0644ef421698ef6f63772929af));
			notifyHelper.NotificationInfo(defaultInterpolatedStringHandler.ToStringAndClear());
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass18_2
	{
		public BDNC7pxDeDBwVp6JNE8 newGatherPoint;

		public _003C_003Ec__DisplayClass18_2()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal bool RmBpnpwewc(BDNC7pxDeDBwVp6JNE8 p)
		{
			return p.Equals(newGatherPoint);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass20_0
	{
		public Vector3 node;

		public _003C_003Ec__DisplayClass20_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal unsafe _003C_003Ef__AnonymousType3<ObjectKind, float, byte, IGameObject> hnLpyQff5n(IGameObject x)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Expected I4, but got Unknown
			return new
			{
				ObjectKind = x.ObjectKind,
				DistanceSquared2D = Vector2.DistanceSquared(x.Position.ToVector2(), node.ToVector2()),
				TargetableStatus = (byte)(int)((GameObject)x.ToStruct()).TargetableStatus,
				Object = x
			};
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass22_0
	{
		public float maxDistance;

		public _003C_003Ec__DisplayClass22_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass22_1
	{
		public Vector2 basePoint;

		public _003C_003Ec__DisplayClass22_0 CS_0024_003C_003E8__locals1;

		public _003C_003Ec__DisplayClass22_1()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal bool dp3pLS9DwB(Vector2 p)
		{
			return Vector2.Distance(p, basePoint) <= CS_0024_003C_003E8__locals1.maxDistance;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass23_0
	{
		public (List<Vector3> path, HashSet<Vector3> unvisited, Vector3 current) acc;

		public _003C_003Ec__DisplayClass23_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal float tmcpHUrJ15(Vector3 p)
		{
			return Vector3.Distance(acc.current, p);
		}
	}

	private XvwAI6xeqJP4aphdTWO config;

	private bool isOnDataCollecting;

	private List<Vector3> dataCollectWaypoints;

	private List<Vector3> queuedGatheringList;

	private Queue<Vector3> currentGatheringQueue;

	private static readonly FrozenDictionary<uint, uint> GatheringItemsIcon;

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected override void Init()
	{
		config = ModuleConfigExtension.Load<XvwAI6xeqJP4aphdTWO>(this) ?? new XvwAI6xeqJP4aphdTWO();
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 15000
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		OmenServiceBase<LogMessageManager>.Instance().RegPost(isSMqdJafG);
	}

	protected override void Uninit()
	{
		OmenServiceBase<LogMessageManager>.Instance().Unreg(isSMqdJafG);
		queuedGatheringList.Clear();
		isOnDataCollecting = false;
	}

	private void isSMqdJafG(uint logMessageID, LogMessageQueueItem values)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Invalid comparison between Unknown and I4
		if ((int)GameState.TerritoryIntendedUse == 49 && logMessageID == 4577 && config.StopWhenReachingCap)
		{
			base.TaskHelper.Abort();
			queuedGatheringList.Clear();
			currentGatheringQueue.Clear();
			string message = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3C1490F1 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_d71d1e134fde413fa0bee3b2419181b6));
			OmenServiceBase<NotifyHelper>.Instance().NotificationError(message);
			NotifyHelper.Speak(message);
		}
	}

	protected override void ConfigUI()
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0468: Unknown result type (might be due to invalid IL or missing references)
		//IL_0495: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0613: Unknown result type (might be due to invalid IL or missing references)
		//IL_0618: Unknown result type (might be due to invalid IL or missing references)
		//IL_0626: Unknown result type (might be due to invalid IL or missing references)
		//IL_064b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_069d: Unknown result type (might be due to invalid IL or missing references)
		DisabledDisposable val = ImRaii.Disabled(GameState.TerritoryType != 1055);
		try
		{
			lIIMcicYI3();
			ImGui.SameLine(0f, 4f * Globals.GlobalUIScale);
			ImGui.TextDisabled(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6E00963E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c40146824d9043e0906f4e1889cf39aa)));
			ImGui.SameLine(0f, 4f * Globals.GlobalUIScale);
			IwQMAsQ1Wp();
			ImGui.SameLine();
			ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2DBD0292 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5d8d924f1edf4ec39f023b3c7bb646cc), queuedGatheringList.Count - currentGatheringQueue.Count, queuedGatheringList.Count)));
			if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x395123A1 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3aeae81f85f84848b273be7b2bfbf497))), ref config.StopWhenReachingCap))
			{
				config.Save(this);
			}
			ImGui.Spacing();
			Vector2 vector = ImGui.GetContentRegionAvail() / 3f + Globals.ScaledVector2(50f);
			vector.Y = 0f;
			Vector2 vector2 = vector;
			if (ImGui.BeginTable(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x36952DE9 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_26e231ba68f74e8784503e099ddfc2d9)), 3, (ImGuiTableFlags)1920, vector2, 0f))
			{
				ImGui.TableSetupColumn(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x420CE836 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_84ee025b19bb4145ba9b4b6b7acb91eb)), (ImGuiTableColumnFlags)8, ImGui.GetTextLineHeightWithSpacing(), 0u);
				ImGui.TableSetupColumn(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xF20798D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_fea4ad1b13de4c4bbc22f571ceda7372)), (ImGuiTableColumnFlags)0, 40f, 0u);
				ImGui.TableSetupColumn(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D9CA5E7 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3badc14f3c904cf4809aa58db382c8ee)), (ImGuiTableColumnFlags)0, 20f, 0u);
				ImGui.TableNextRow((ImGuiTableRowFlags)1);
				ImGui.TableNextColumn();
				ImGui.TableNextColumn();
				ImGuiOm.Text(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2DBD05DE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5d8d924f1edf4ec39f023b3c7bb646cc)));
				ImGui.TableNextColumn();
				ImGuiOm.Text(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x32D926B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_86d75c161ced498ab6a767d2eb82d38e)));
				foreach (BDNC7pxDeDBwVp6JNE8 item in config.IslandGatherPoints.Take(config.IslandGatherPoints.Count / 2))
				{
					if (GatheringItemsIcon.TryGetValue(item.EObjID, out var value))
					{
						ImGui.TableNextRow();
						ImGui.TableNextColumn();
						bool enabled = item.Enabled;
						DisabledDisposable val2 = ImRaii.Disabled();
						try
						{
							ImU8String val3 = new ImU8String(3, 1);
							((ImU8String)(ref val3)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5DD72817 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c7355392ad9e43b6b82798cc42ea6c6f));
							((ImU8String)(ref val3)).AppendFormatted<Lazy<string>>(item.Name);
							ImGui.Checkbox(val3, ref enabled);
						}
						finally
						{
							((IDisposable)val2)?.Dispose();
						}
						ImGui.TableNextColumn();
						ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
						GameIconLookup val4 = GameIconLookup.op_Implicit(value);
						ImGui.Image(obj.GetFromGameIcon(ref val4).GetWrapOrEmpty().Handle, Globals.ScaledVector2(20f));
						ImGui.SameLine();
						if (ImGui.Selectable(ImU8String.op_Implicit(item.Name.Value), enabled, (ImGuiSelectableFlags)2, default(Vector2)))
						{
							item.Enabled = !item.Enabled;
							config.Save(this);
						}
						ImGui.TableNextColumn();
						ImGui.TextUnformatted(ImU8String.op_Implicit(item.Points.Count.ToString()));
					}
				}
				ImGui.EndTable();
			}
			ImGui.SameLine();
			if (!ImGui.BeginTable(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x51E5B438 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a90a92322bff42e180982e106200d4a3)), 3, (ImGuiTableFlags)1920, vector2, 0f))
			{
				return;
			}
			ImGui.TableSetupColumn(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6E00916C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c40146824d9043e0906f4e1889cf39aa)), (ImGuiTableColumnFlags)8, ImGui.GetTextLineHeightWithSpacing(), 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--1317373447 ^ 0x12669540 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3054c6e6ef664be696bb2ad005864818)), (ImGuiTableColumnFlags)0, 40f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-1003249140 ^ -668669901 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_accf3844f1c6464481df1849f0aee1bb)), (ImGuiTableColumnFlags)0, 20f, 0u);
			ImGui.TableNextRow((ImGuiTableRowFlags)1);
			ImGui.TableNextColumn();
			ImGui.TableNextColumn();
			ImGuiOm.Text(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x9B49CF8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2f1c0cb46b4440d499a69ed8afe22360)));
			ImGui.TableNextColumn();
			ImGuiOm.Text(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3951231F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3aeae81f85f84848b273be7b2bfbf497)));
			foreach (BDNC7pxDeDBwVp6JNE8 item2 in config.IslandGatherPoints.Skip(config.IslandGatherPoints.Count / 2))
			{
				if (GatheringItemsIcon.TryGetValue(item2.EObjID, out var value2))
				{
					ImGui.TableNextRow();
					ImGui.TableNextColumn();
					bool enabled2 = item2.Enabled;
					DisabledDisposable val2 = ImRaii.Disabled();
					try
					{
						ImU8String val5 = new ImU8String(3, 1);
						((ImU8String)(ref val5)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x53519934 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_9085260e6b1a40438d219e85182b5d08));
						((ImU8String)(ref val5)).AppendFormatted<Lazy<string>>(item2.Name);
						ImGui.Checkbox(val5, ref enabled2);
					}
					finally
					{
						((IDisposable)val2)?.Dispose();
					}
					ImGui.TableNextColumn();
					ITextureProvider obj2 = DalamudServiceExtension.Instance<ITextureProvider>();
					GameIconLookup val4 = new GameIconLookup(value2, false, true, (ClientLanguage?)null);
					ImGui.Image(obj2.GetFromGameIcon(ref val4).GetWrapOrEmpty().Handle, Globals.ScaledVector2(20f));
					ImGui.SameLine();
					if (ImGui.Selectable(ImU8String.op_Implicit(item2.Name.Value), enabled2, (ImGuiSelectableFlags)2, default(Vector2)))
					{
						item2.Enabled = !item2.Enabled;
						config.Save(this);
					}
					ImGui.TableNextColumn();
					ImGui.TextUnformatted(ImU8String.op_Implicit(item2.Points.Count.ToString()));
				}
			}
			ImGui.EndTable();
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void lIIMcicYI3()
	{
		if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61888, LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-224923271 ^ -1698486236 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_23522a2a14314da28eb44187a09cd0b5)), useStaticFont: true))
		{
			isOnDataCollecting = !isOnDataCollecting;
			if (isOnDataCollecting)
			{
				if (dataCollectWaypoints.Count == 0)
				{
					dataCollectWaypoints = TEQMk04bSi((from item in (IEnumerable<MJIGatheringItem>)LuminaGetter.Get<MJIGatheringItem>()
						where ((MJIGatheringItem)(ref item)).Item.ValueNullable.HasValue
						select new Vector2(((MJIGatheringItem)(ref item)).X, ((MJIGatheringItem)(ref item)).Y)).ToList(), 50f);
				}
				pcUMJxLvTs(dataCollectWaypoints);
			}
			else
			{
				base.TaskHelper.Abort();
				config.Save(this);
			}
		}
		ImGuiOm.TooltipHover(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7C2F4B84 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_12c2d68ba4654622a4959ee73cacc6d7)));
	}

	private void IwQMAsQ1Wp()
	{
		DisabledDisposable val = ImRaii.Disabled(isOnDataCollecting);
		try
		{
			DisabledDisposable val2 = ImRaii.Disabled(base.TaskHelper.IsBusy || IConditionExtension.get_IsOnMount(DalamudServiceExtension.Instance<ICondition>()));
			try
			{
				if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61515, LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5CE314AD ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3054c6e6ef664be696bb2ad005864818)), useStaticFont: true))
				{
					base.TaskHelper.Enqueue((Func<bool>)HbpMr1JSbA, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
					List<Vector3> list = SortPath(config.IslandGatherPoints.Where((BDNC7pxDeDBwVp6JNE8 group) => group.Enabled).SelectMany((BDNC7pxDeDBwVp6JNE8 group) => group.Points ?? new List<Vector3>())).ToList();
					if (list.Count > 10)
					{
						queuedGatheringList = list;
						Queue<Vector3> queue = new Queue<Vector3>();
						list.ForEach(queue.Enqueue);
						lBkMQcdJAb(queue);
					}
					else
					{
						OmenServiceBase<NotifyHelper>.Instance().NotificationError(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4CF214E8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a07aac30eb32484e94cb4d1e62117765)));
					}
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
			ImGui.SameLine();
			if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61516, LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xBA4C470 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2edd08450a8b42e6997855e5217d6d55)), useStaticFont: true))
			{
				base.TaskHelper.Abort();
				queuedGatheringList.Clear();
				currentGatheringQueue.Clear();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private unsafe void lBkMQcdJAb(Queue<Vector3> nodes)
	{
		_003C_003Ec__DisplayClass17_0 CS_0024_003C_003E8__locals29 = new _003C_003Ec__DisplayClass17_0();
		CS_0024_003C_003E8__locals29._003C_003E4__this = this;
		CS_0024_003C_003E8__locals29.nodes = nodes;
		CS_0024_003C_003E8__locals29.localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
		if (CS_0024_003C_003E8__locals29.localPlayer == null || IConditionExtension.get_IsOnMount(DalamudServiceExtension.Instance<ICondition>()) || IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()))
		{
			return;
		}
		if (CS_0024_003C_003E8__locals29.nodes.Count == 0)
		{
			queuedGatheringList.ForEach(CS_0024_003C_003E8__locals29.nodes.Enqueue);
			base.TaskHelper.DelayNext(100);
		}
		currentGatheringQueue = CS_0024_003C_003E8__locals29.nodes;
		CS_0024_003C_003E8__locals29.currentNode = CS_0024_003C_003E8__locals29.nodes.Dequeue();
		base.TaskHelper.Enqueue((Func<bool>)HbpMr1JSbA, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x43D708F0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_974d377e1ed74986b81c34c95c2cc846), 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 2);
		base.TaskHelper.Enqueue(delegate
		{
			Vector3 vector = CS_0024_003C_003E8__locals29.currentNode;
			vector.Y = CS_0024_003C_003E8__locals29.currentNode.Y - 1f;
			Vector3 vector2 = vector;
			if (MovementManager.IsNeedEnvironmentChange(CS_0024_003C_003E8__locals29.localPlayer.Position, vector2) == true)
			{
				CS_0024_003C_003E8__locals29._003C_003E4__this.TaskHelper.Enqueue(() => IConditionExtension.get_IsBetweenAreas(DalamudServiceExtension.Instance<ICondition>()), null, 0, null, null, null, null, 1);
				CS_0024_003C_003E8__locals29._003C_003E4__this.TaskHelper.Enqueue(() => !IConditionExtension.get_IsBetweenAreas(DalamudServiceExtension.Instance<ICondition>()) && UIModuleExtension.IsScreenReady(), null, 0, null, null, null, null, 1);
				CS_0024_003C_003E8__locals29._003C_003E4__this.TaskHelper.DelayNext(1000, null, 1);
			}
			PositionUpdatePacket.Send(CS_0024_003C_003E8__locals29.localPlayer.Rotation, vector2);
			OmenServiceBase<MovementManager>.Instance().TPSmart_InZone(vector2, new TPSmartParams
			{
				CancelAnimation = false
			});
		}, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x36952C09 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_26e231ba68f74e8784503e099ddfc2d9));
		base.TaskHelper.Enqueue(delegate
		{
			_003C_003Ec__DisplayClass17_1 CS_0024_003C_003E8__locals27 = new _003C_003Ec__DisplayClass17_1();
			switch (IsgMGi5AY2(CS_0024_003C_003E8__locals29.currentNode, out CS_0024_003C_003E8__locals27.obj))
			{
			case NodeState.NotReady:
				return false;
			case NodeState.NotFound:
				return true;
			case NodeState.Gathered:
				CS_0024_003C_003E8__locals29.nodes.Enqueue(CS_0024_003C_003E8__locals29.currentNode);
				return true;
			case NodeState.Available:
				CS_0024_003C_003E8__locals29._003C_003E4__this.TaskHelper.Enqueue(delegate
				{
					((EventFramework)EventFramework.Instance()).InteractWithReactionEventObject(CS_0024_003C_003E8__locals27.obj.ToStruct());
				}, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6FF10F3A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5859f4ea3042459db2c9b610b3099912), 0, null, null, null, null, 1);
				CS_0024_003C_003E8__locals29._003C_003E4__this.TaskHelper.Enqueue(() => DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)32], LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x68AC8DD2 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4722689f1a4d41dc9dc800bfe1a89c09), 0, null, null, null, null, 1);
				CS_0024_003C_003E8__locals29._003C_003E4__this.TaskHelper.Enqueue(() => !gDxM6oBJQm(), LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3F3654F6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7a7485368e28474eb6d61987c51729a2), 0, null, null, null, null, 1);
				return true;
			default:
				return true;
			}
		});
		base.TaskHelper.Enqueue(delegate
		{
			CS_0024_003C_003E8__locals29._003C_003E4__this.TaskHelper.Abort();
			CS_0024_003C_003E8__locals29._003C_003E4__this.TaskHelper.Enqueue(delegate
			{
				CS_0024_003C_003E8__locals29._003C_003E4__this.lBkMQcdJAb(CS_0024_003C_003E8__locals29.nodes);
			});
		});
	}

	private unsafe void pcUMJxLvTs(List<Vector3> nodes)
	{
		_003C_003Ec__DisplayClass18_0 CS_0024_003C_003E8__locals26 = new _003C_003Ec__DisplayClass18_0();
		CS_0024_003C_003E8__locals26._003C_003E4__this = this;
		CS_0024_003C_003E8__locals26.nodes = nodes;
		for (int i = 0; i < CS_0024_003C_003E8__locals26.nodes.Count; i++)
		{
			_003C_003Ec__DisplayClass18_1 CS_0024_003C_003E8__locals21 = new _003C_003Ec__DisplayClass18_1();
			CS_0024_003C_003E8__locals21.CS_0024_003C_003E8__locals1 = CS_0024_003C_003E8__locals26;
			CS_0024_003C_003E8__locals21.index = i;
			CS_0024_003C_003E8__locals21.node = CS_0024_003C_003E8__locals21.CS_0024_003C_003E8__locals1.nodes[i];
			base.TaskHelper.Enqueue(delegate
			{
				if (!UIModuleExtension.IsScreenReady())
				{
					return false;
				}
				OmenServiceBase<MovementManager>.Instance().TPSmart_InZone(CS_0024_003C_003E8__locals21.node, new TPSmartParams
				{
					CancelAnimation = false
				});
				return true;
			});
			base.TaskHelper.Enqueue(delegate
			{
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_002c: Invalid comparison between Unknown and I4
				foreach (IGameObject item in IOmenDalamudService<IObjectTable>.Instance())
				{
					_003C_003Ec__DisplayClass18_2 CS_0024_003C_003E8__locals27 = new _003C_003Ec__DisplayClass18_2();
					if ((int)item.ObjectKind == 14 && item.DataID != 0 && item.DataID != 2013159)
					{
						GameObject* ptr = item.ToStruct();
						if (ptr != null && ((GameObject)ptr).GetNameId() >= 2000000)
						{
							CS_0024_003C_003E8__locals27.newGatherPoint = new BDNC7pxDeDBwVp6JNE8(((GameObject)ptr).GetNameId());
							if (!CS_0024_003C_003E8__locals21.CS_0024_003C_003E8__locals1._003C_003E4__this.config.IslandGatherPoints.Contains(CS_0024_003C_003E8__locals27.newGatherPoint))
							{
								CS_0024_003C_003E8__locals21.CS_0024_003C_003E8__locals1._003C_003E4__this.config.IslandGatherPoints.Add(CS_0024_003C_003E8__locals27.newGatherPoint);
								CS_0024_003C_003E8__locals27.newGatherPoint.Points.Add(item.Position);
							}
							else
							{
								BDNC7pxDeDBwVp6JNE8 bDNC7pxDeDBwVp6JNE = CS_0024_003C_003E8__locals21.CS_0024_003C_003E8__locals1._003C_003E4__this.config.IslandGatherPoints.Find((BDNC7pxDeDBwVp6JNE8 p) => p.Equals(CS_0024_003C_003E8__locals27.newGatherPoint));
								if (!bDNC7pxDeDBwVp6JNE.Points.Contains(item.Position))
								{
									bDNC7pxDeDBwVp6JNE.Points.Add(item.Position);
								}
							}
						}
					}
				}
			});
			base.TaskHelper.Enqueue(delegate
			{
				NotifyHelper notifyHelper = OmenServiceBase<NotifyHelper>.Instance();
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 3);
				defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x50177914 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e227921f7ee94b6aa1c28169395ba0cf));
				defaultInterpolatedStringHandler.AppendFormatted(CS_0024_003C_003E8__locals21.index + 1);
				defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489BD79C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cade14e68bb84906959f3cda5c496848));
				defaultInterpolatedStringHandler.AppendFormatted(CS_0024_003C_003E8__locals21.CS_0024_003C_003E8__locals1.nodes.Count);
				defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2194A566 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cd1b319f326a44d1aaea645e95d8698b));
				defaultInterpolatedStringHandler.AppendFormatted(CS_0024_003C_003E8__locals21.node);
				defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4B0F27FE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_8e002b0644ef421698ef6f63772929af));
				notifyHelper.NotificationInfo(defaultInterpolatedStringHandler.ToStringAndClear());
			});
			base.TaskHelper.DelayNext(1000);
		}
		base.TaskHelper.Enqueue(delegate
		{
			CS_0024_003C_003E8__locals26._003C_003E4__this.TaskHelper.Abort();
			CS_0024_003C_003E8__locals26._003C_003E4__this.config.Save(CS_0024_003C_003E8__locals26._003C_003E4__this);
			CS_0024_003C_003E8__locals26._003C_003E4__this.isOnDataCollecting = false;
			CS_0024_003C_003E8__locals26._003C_003E4__this.TaskHelper.Enqueue(() => OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)5, 27u, 3758096384uL, 0u, (UseActionMode)0));
		});
	}

	private unsafe bool HbpMr1JSbA()
	{
		if (((MJIManager)MJIManager.Instance()).CurrentMode == 1)
		{
			return true;
		}
		AtkUnitBase* byName = AddonHelper.GetByName(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7EE7B2AD ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88432e96079448078bdf00e7b62f8ab6));
		if (byName == null)
		{
			OmenServiceBase<NotifyHelper>.Instance().NotificationError(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x454E3397 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_09da05f368544962acb62b42d9155467)));
			base.TaskHelper.Abort();
			return true;
		}
		(*byName).Callback(11, 0);
		((AgentId)418).SendEvent(2uL, 0, 1, 82042u, 0u, 0);
		return ((MJIManager)MJIManager.Instance()).CurrentMode == 1;
	}

	private unsafe static NodeState IsgMGi5AY2(Vector3 node, out IGameObject? obj)
	{
		_003C_003Ec__DisplayClass20_0 CS_0024_003C_003E8__locals3 = new _003C_003Ec__DisplayClass20_0();
		CS_0024_003C_003E8__locals3.node = node;
		obj = null;
		if (!OmenServiceBase<MovementManager>.Instance().IsManagerBusy && !gDxM6oBJQm())
		{
			IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
			if (localPlayer != null && !(Vector2.DistanceSquared(CS_0024_003C_003E8__locals3.node.ToVector2(), localPlayer.Position.ToVector2()) > 4f))
			{
				var list = (from x in IOmenDalamudService<IObjectTable>.Instance()
					select new
					{
						ObjectKind = x.ObjectKind,
						DistanceSquared2D = Vector2.DistanceSquared(x.Position.ToVector2(), CS_0024_003C_003E8__locals3.node.ToVector2()),
						TargetableStatus = (byte)(int)((GameObject)x.ToStruct()).TargetableStatus,
						Object = x
					} into x
					where (int)x.ObjectKind == 14 && x.DistanceSquared2D <= 4f
					orderby x.TargetableStatus != 248 descending, x.DistanceSquared2D
					select x).ToList();
				if (list.Count == 0)
				{
					return NodeState.NotFound;
				}
				var anon = list.First();
				if (anon.TargetableStatus == 248)
				{
					return NodeState.Gathered;
				}
				obj = anon.Object;
				OmenServiceBase<MovementManager>.Instance().TPSmart_InZone(obj.Position);
				return NodeState.Available;
			}
		}
		return NodeState.NotReady;
	}

	private static bool gDxM6oBJQm()
	{
		if (UIModuleExtension.IsScreenReady() && !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)45] && !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)48] && !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)61] && !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)32])
		{
			return DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)27];
		}
		return true;
	}

	private static List<Vector3> TEQMk04bSi(List<Vector2> points, float maxDistance)
	{
		_003C_003Ec__DisplayClass22_0 _003C_003Ec__DisplayClass22_2 = new _003C_003Ec__DisplayClass22_0();
		_003C_003Ec__DisplayClass22_2.maxDistance = maxDistance;
		List<Vector3> list = new List<Vector3>();
		while (points.Count != 0)
		{
			_003C_003Ec__DisplayClass22_1 CS_0024_003C_003E8__locals4 = new _003C_003Ec__DisplayClass22_1();
			CS_0024_003C_003E8__locals4.CS_0024_003C_003E8__locals1 = _003C_003Ec__DisplayClass22_2;
			CS_0024_003C_003E8__locals4.basePoint = points.First();
			List<Vector2> list2 = points.Where((Vector2 p) => Vector2.Distance(p, CS_0024_003C_003E8__locals4.basePoint) <= CS_0024_003C_003E8__locals4.CS_0024_003C_003E8__locals1.maxDistance).ToList();
			Vector2 vector = new Vector2(list2.Average((Vector2 p) => p.X), list2.Average((Vector2 p) => p.Y));
			points = points.Except(list2).ToList();
			list.AddRange(new global::_003C_003Ez__ReadOnlyArray<Vector3>(new Vector3[4]
			{
				new Vector3(vector.X, -40f, vector.Y),
				new Vector3(vector.X, 10f, vector.Y),
				new Vector3(vector.X, 100f, vector.Y),
				new Vector3(vector.X, 200f, vector.Y)
			}));
		}
		return list;
	}

	public static IEnumerable<Vector3> SortPath(IEnumerable<Vector3> coordinates)
	{
		Vector3[] source = (coordinates as Vector3[]) ?? coordinates.ToArray();
		return source.Aggregate((new List<Vector3>(), source.ToHashSet(), IOmenDalamudService<IObjectTable>.Instance().LocalPlayer.Position), delegate((List<Vector3> path, HashSet<Vector3> unvisited, Vector3 current) acc, Vector3 _)
		{
			_003C_003Ec__DisplayClass23_0 CS_0024_003C_003E8__locals7 = new _003C_003Ec__DisplayClass23_0();
			CS_0024_003C_003E8__locals7.acc = acc;
			Vector3 vector = CS_0024_003C_003E8__locals7.acc.unvisited.MinBy((Vector3 p) => Vector3.Distance(CS_0024_003C_003E8__locals7.acc.current, p));
			CS_0024_003C_003E8__locals7.acc.path.Add(vector);
			CS_0024_003C_003E8__locals7.acc.unvisited.Remove(vector);
			return (path: CS_0024_003C_003E8__locals7.acc.path, unvisited: CS_0024_003C_003E8__locals7.acc.unvisited, current: vector);
		}, ((List<Vector3> path, HashSet<Vector3> unvisited, Vector3 current) acc) => acc.path);
	}

	public AutoMJIGather()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2CE5100D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c9dc773883f149b7befba3bdbeac55f7)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x477E2285 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_ba27244341a5414496a87eae29cf3dec)),
			Category = ModuleCategory.Script
		};
		Permission = new ModulePermission
		{
			NeedAuth = true
		};
		dataCollectWaypoints = new List<Vector3>();
		queuedGatheringList = new List<Vector3>();
		currentGatheringQueue = new Queue<Vector3>();
		base._002Ector();
	}

	static AutoMJIGather()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		GatheringItemsIcon = ((IEnumerable<MJIGatheringObject>)LuminaGetter.Get<MJIGatheringObject>()).Where((MJIGatheringObject x) => ((MJIGatheringObject)(ref x)).MapIcon != 0).ToFrozenDictionary((MJIGatheringObject x) => ((MJIGatheringObject)(ref x)).Name.RowId, (MJIGatheringObject x) => ((MJIGatheringObject)(ref x)).MapIcon);
	}
}
