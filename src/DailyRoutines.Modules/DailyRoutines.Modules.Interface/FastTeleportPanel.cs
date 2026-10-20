using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Extensions;
using DailyRoutines.Common.Interface.Windows;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using DailyRoutines.Verification;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Fates;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Party;
using Dalamud.Game.Command;
using Dalamud.Interface;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using Dalamud.Utility.Numerics;
using FFXIVClientStructs.FFXIV.Application.Network.WorkDefinitions;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.LayoutEngine;
using FFXIVClientStructs.FFXIV.Client.LayoutEngine.Layer;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Common.Component.BGCollision;
using FFXIVClientStructs.FFXIV.Common.Math;
using FFXIVClientStructs.Interop;
using FFXIVClientStructs.STD;
using FFXIVClientStructs.STD.ContainerInterface;
using FFXIVClientStructs.STD.Helper;
using InteropGenerator.Runtime;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools;
using OmenTools.Dalamud;
using OmenTools.Dalamud.Abstractions;
using OmenTools.Dalamud.Attributes;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Info.Game.AetheryteRecord;
using OmenTools.Info.Game.Data.Icons;
using OmenTools.Info.Game.Enums;
using OmenTools.Info.Game.Packets.Upstream;
using OmenTools.Info.Lumina;
using OmenTools.Interop.Game.Helpers;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models.Native;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules.Interface;

public class FastTeleportPanel : ModuleBase
{
	private class omRh8MmZOgrZ163kmaR : ModuleConfig
	{
		public bool AddPosOffsetCommand;

		public HashSet<uint> BlockedIcons;

		public uint CustomIconIDInput;

		public Dictionary<uint, List<MapMarkerDisplay>> CustomMarkers;

		public Dictionary<uint, string> IconNotes;

		public Dictionary<uint, uint> IconWeight;

		public int PositionOffsetDelta;

		public omRh8MmZOgrZ163kmaR()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			AddPosOffsetCommand = true;
			BlockedIcons = new HashSet<uint>();
			CustomIconIDInput = 60561u;
			CustomMarkers = new Dictionary<uint, List<MapMarkerDisplay>>();
			IconNotes = new Dictionary<uint, string>();
			IconWeight = new Dictionary<uint, uint>();
			PositionOffsetDelta = 5;
			base._002Ector();
		}
	}

	private sealed class lD0TdJmaaXGs4MmrNMs
	{
		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass16_0
		{
			public uint territoryType;

			public _003C_003Ec__DisplayClass16_0()
			{
				UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
				base._002Ector();
			}

			internal bool VNDPIkRdqr(Warp x)
			{
				//IL_0009: Unknown result type (might be due to invalid IL or missing references)
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				return ((Warp)(ref x)).TerritoryType.RowId == territoryType;
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass26_0
		{
			public IGameObject gameObject;

			public IPlayerCharacter localPlayer;

			public lD0TdJmaaXGs4MmrNMs _003C_003E4__this;

			public _003C_003Ec__DisplayClass26_0()
			{
				UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
				base._002Ector();
			}

			internal bool w8UPUvgODn()
			{
				Vector2 vector = Vector2.Normalize(gameObject.Position.ToVector2() - localPlayer.Position.ToVector2());
				return OmenServiceBase<MovementManager>.Instance().TPSmart_InZone(gameObject.Position + vector.ToVector3(0f) * gameObject.HitboxRadius);
			}

			internal bool OkNPuGKaNC()
			{
				if (!wB8mdvinN2())
				{
					return false;
				}
				TargetManager.Target = gameObject;
				if (gameObject.TargetInteract())
				{
					return true;
				}
				OmenServiceBase<MovementManager>.Instance().TPSmart_InZone(gameObject.Position);
				return false;
			}

			internal void MAbPXea2Gk()
			{
				_003C_003E4__this._003Cmodule_003EP.UseEventItem?.InvokeAction();
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass27_0
		{
			public HashSet<uint> questIDs;

			public Vector3 pos;

			public _003C_003Ec__DisplayClass27_0()
			{
				UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
				base._002Ector();
			}

			internal unsafe bool SUEPY430Eo(Pointer<EventHandler> x)
			{
				//IL_004c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0052: Invalid comparison between Unknown and I4
				if (x.Value != null && questIDs.Contains(((EventId)(&((EventHandlerInfo)(&((EventHandler)x.Value).Info)).EventId)).Id))
				{
					return (int)((EventId)(&((EventHandlerInfo)(&((EventHandler)x.Value).Info)).EventId)).ContentId == 1;
				}
				return false;
			}

			internal float lVHPzBdgia(IGameObject x)
			{
				return Vector3.DistanceSquared(pos, x.Position);
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass30_0
		{
			public lD0TdJmaaXGs4MmrNMs _003C_003E4__this;

			public MapMarkerDisplay marker;

			public IGameObject mtqNearby;

			public IPlayerCharacter localPlayer;

			public _003C_003Ec__DisplayClass30_0()
			{
				UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
				base._002Ector();
			}

			internal bool FVyhnxuoKq()
			{
				return _003C_003E4__this.iyLmXRx01L(marker.Position, marker.Radius, mtqNearby);
			}

			internal bool woThysZ5ab()
			{
				Vector2 vector = Vector2.Normalize(marker.Position.ToVector2() - localPlayer.Position.ToVector2());
				return OmenServiceBase<MovementManager>.Instance().TPSmart_InZone(marker.Position + ((marker.Radius == 1f) ? vector.ToVector3(0f) : Vector3.Zero));
			}

			internal void Wg9hLtrhPq()
			{
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				RaycastHit hitInfo;
				Vector3 pos = (RaycastHelper.TryGetNearestVerticalHit(localPlayer.Position + new Vector3(0f, 2f, 0f), out hitInfo) ? hitInfo.Point : marker.Position);
				OmenServiceBase<MovementManager>.Instance().TPSmart_InZone(pos);
			}

			internal bool XcthHwBUWj()
			{
				return _003C_003E4__this.iyLmXRx01L(marker.Position, marker.Radius);
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass31_0
		{
			public unsafe ExitRangeLayoutInstance* instance;

			public _003C_003Ec__DisplayClass31_0()
			{
				UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
				base._002Ector();
			}

			internal unsafe void BfshSs58mH()
			{
				PopRangeManager.Instance()->PopRange((ILayoutInstance*)instance);
			}

			internal unsafe void PI9hVpx05m()
			{
				PopRangeManager.Instance()->PopRange((ILayoutInstance*)instance);
			}
		}

		private readonly Dictionary<uint, List<MapMarkerDisplay>> mapMarkers;

		private readonly List<MapMarkerDisplay> questMapMarkers;

		private readonly Dictionary<uint, List<MapMarkerDisplay>> exdMarkersCache;

		private static readonly Dictionary<uint, List<MapMarkerDisplay>> HousingMarkers;

		public IReadOnlyDictionary<uint, List<MapMarkerDisplay>> MapMarkers { get; private set; }

		public List<MapMarkerDisplay> QuestMapMarkers { get; private set; }

		public lD0TdJmaaXGs4MmrNMs(FastTeleportPanel module)
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			_003Cmodule_003EP = module;
			mapMarkers = new Dictionary<uint, List<MapMarkerDisplay>>();
			questMapMarkers = new List<MapMarkerDisplay>();
			exdMarkersCache = new Dictionary<uint, List<MapMarkerDisplay>>();
			MapMarkers = new Dictionary<uint, List<MapMarkerDisplay>>();
			QuestMapMarkers = new List<MapMarkerDisplay>();
			base._002Ector();
		}

		static lD0TdJmaaXGs4MmrNMs()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			HousingMarkers = new Dictionary<uint, List<MapMarkerDisplay>>();
			(from x in ((IEnumerable<SubrowCollection<HousingMapMarkerInfo>>)LuminaGetter.GetSub<HousingMapMarkerInfo>()).SelectMany((SubrowCollection<HousingMapMarkerInfo> x) => (IEnumerable<HousingMapMarkerInfo>)(object)x)
				where ((HousingMapMarkerInfo)(ref x)).Map.ValueNullable.HasValue
				group x by ((HousingMapMarkerInfo)(ref x)).Map.RowId).ForEach(delegate(IGrouping<uint, HousingMapMarkerInfo> x)
			{
				HousingMarkers[x.Key] = (from y in x.Select(MapMarkerDisplay.Parse)
					where y?.IsUsable ?? false
					select y).OfType<MapMarkerDisplay>().ToList();
			});
		}

		public void aNjm0r2Ts7()
		{
			if (IConditionExtension.get_IsBetweenAreas(DalamudServiceExtension.Instance<ICondition>()) || LocalPlayerState.Object == null)
			{
				return;
			}
			List<MapMarkerDisplay> list = new List<MapMarkerDisplay>();
			cJDm5uVwQr(list);
			LDrmRYb3yH(list);
			nE1mDUHa1d(list);
			tc5meshcqU(list);
			gU8mNKgCcI(list);
			UYDm7MFJGL(list);
			mapMarkers.Clear();
			questMapMarkers.Clear();
			foreach (MapMarkerDisplay item in list.Distinct())
			{
				uint iconID = item.IconID;
				if (item.IsQuest)
				{
					mapMarkers.TryAdd(71021u, new List<MapMarkerDisplay>());
					mapMarkers[71021u].Add(item);
					if (!QuestIcons.Invalid.Contains(iconID) && !QuestIcons.Normal.Contains(iconID))
					{
						questMapMarkers.Add(item);
					}
				}
				else if (iconID != 60490)
				{
					mapMarkers.TryAdd(iconID, new List<MapMarkerDisplay>());
					mapMarkers[iconID].Add(item);
				}
			}
			MapMarkers = mapMarkers.OrderByDescending<KeyValuePair<uint, List<MapMarkerDisplay>>, uint>((KeyValuePair<uint, List<MapMarkerDisplay>> x) => _003Cmodule_003EP.config.IconWeight.GetValueOrDefault(x.Key, x.Key / 10000)).ToDictionary((KeyValuePair<uint, List<MapMarkerDisplay>> x) => x.Key, (KeyValuePair<uint, List<MapMarkerDisplay>> x) => x.Value);
			QuestMapMarkers = questMapMarkers;
		}

		private void cJDm5uVwQr(List<MapMarkerDisplay> markers)
		{
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			_003C_003Ec__DisplayClass16_0 CS_0024_003C_003E8__locals4 = new _003C_003Ec__DisplayClass16_0();
			CS_0024_003C_003E8__locals4.territoryType = GameState.TerritoryType;
			if (!exdMarkersCache.TryGetValue(CS_0024_003C_003E8__locals4.territoryType, out List<MapMarkerDisplay> value))
			{
				List<MapMarkerDisplay> list = new List<MapMarkerDisplay>();
				list.AddRange((from x in GameState.TerritoryTypeData.GetMapMarkers().Select(MapMarkerDisplay.Parse)
					where x != null
					select x).OfType<MapMarkerDisplay>());
				list.AddRange((from x in ((IEnumerable<Warp>)LuminaGetter.Get<Warp>()).Where((Warp x) => ((Warp)(ref x)).TerritoryType.RowId == CS_0024_003C_003E8__locals4.territoryType).Select(MapMarkerDisplay.Parse)
					where x != null
					select x).OfType<MapMarkerDisplay>());
				value = list;
				exdMarkersCache[CS_0024_003C_003E8__locals4.territoryType] = value;
			}
			markers.AddRange(value);
		}

		private unsafe static void LDrmRYb3yH(List<MapMarkerDisplay> markers)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			UnmanagedArrayEnumerator<MapMarkerData> enumerator = ((AgentHUD)AgentHUD.Instance()).MapMarkers.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					MapMarkerDisplay mapMarkerDisplay = MapMarkerDisplay.Parse(enumerator.Current);
					if (mapMarkerDisplay != null && mapMarkerDisplay.IsUsable && mapMarkerDisplay.IsQuest)
					{
						markers.Add(mapMarkerDisplay);
					}
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
			}
			Span<QuestWork> normalQuests = ((QuestManager)QuestManager.Instance()).NormalQuests;
			for (int i = 0; i < normalQuests.Length; i++)
			{
				QuestWork val = normalQuests[i];
				if (val.QuestId == 0 || ((QuestWork)(ref val)).IsHidden)
				{
					continue;
				}
				uint num = (uint)(val.QuestId + 65536);
				if (!LuminaGetter.TryGetRow<Quest>(num, out Quest item))
				{
					continue;
				}
				List<Vector3> list = AGbinJhK5w(num);
				if (list.Count == 0)
				{
					continue;
				}
				string name = ((object)((Quest)(ref item)).Name/*cast due to constrained. prefix*/).ToString();
				EventIconType? valueNullable = ((Quest)(ref item)).EventIconType.ValueNullable;
				int num2;
				if (!valueNullable.HasValue)
				{
					num2 = 0;
				}
				else
				{
					EventIconType valueOrDefault = valueNullable.GetValueOrDefault();
					num2 = (int)(((EventIconType)(ref valueOrDefault)).MapIconAvailable + 1);
				}
				uint iconID = (uint)num2;
				foreach (Vector3 item2 in list)
				{
					markers.Add(new MapMarkerDisplay
					{
						Name = name,
						IconID = iconID,
						Position = item2,
						Radius = 1f,
						ExtraInfo = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x12D6A63C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7fcf5a71a7ed445197f132979519c560) + LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x45E4C075 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1904f95d2a394c929e1a6d4f72943ad7)) + LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x477EE657 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_ba27244341a5414496a87eae29cf3dec)
					});
				}
			}
		}

		private unsafe static void nE1mDUHa1d(List<MapMarkerDisplay> markers)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			AgentMap* ptr = AgentMap.Instance();
			((AgentMap)ptr).UpdateEventMapMarkers(&((AgentMap)ptr).EventMarkersPtrs);
			UnmanagedArrayEnumerator<MapMarkerData> enumerator = ((AgentMap)ptr).EventMarkers.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					MapMarkerDisplay mapMarkerDisplay = MapMarkerDisplay.Parse(enumerator.Current);
					if (mapMarkerDisplay != null && mapMarkerDisplay.IsUsable && !mapMarkerDisplay.IsQuest)
					{
						markers.Add(FateIcons.IsFate(mapMarkerDisplay.IconID) ? mapMarkerDisplay.ToFateGroupMarker() : mapMarkerDisplay);
					}
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
			}
		}

		private static void tc5meshcqU(List<MapMarkerDisplay> markers)
		{
			foreach (IFate item in (IEnumerable<IFate>)DalamudServiceExtension.Instance<IFateTable>())
			{
				MapMarkerDisplay mapMarkerDisplay = MapMarkerDisplay.Parse(item);
				if (mapMarkerDisplay != null && mapMarkerDisplay.IsUsable)
				{
					markers.Add(mapMarkerDisplay);
				}
			}
		}

		private static void gU8mNKgCcI(List<MapMarkerDisplay> markers)
		{
			foreach (IPartyMember item in (IEnumerable<IPartyMember>)DalamudServiceExtension.Instance<IPartyList>())
			{
				MapMarkerDisplay mapMarkerDisplay = MapMarkerDisplay.Parse(item);
				if (mapMarkerDisplay != null && mapMarkerDisplay.IsUsable)
				{
					markers.Add(mapMarkerDisplay);
				}
			}
		}

		private static void UYDm7MFJGL(List<MapMarkerDisplay> markers)
		{
			if (HousingMarkers.TryGetValue(DalamudServiceExtension.Instance<IClientState>().MapId, out List<MapMarkerDisplay> value))
			{
				markers.AddRange(value);
			}
		}

		public unsafe static bool wB8mdvinN2()
		{
			if (!IConditionExtension.get_IsBetweenAreas(DalamudServiceExtension.Instance<ICondition>()) && UIModuleExtension.IsScreenReady() && !OmenServiceBase<MovementManager>.Instance().IsManagerBusy && !((AgentMap)AgentMap.Instance()).IsPlayerMoving)
			{
				return !vnavmeshIPC.GetIsPathfindInProgress();
			}
			return false;
		}

		public static bool JPMmIw63WD(Vector3 pos, out Vector3 worldPos)
		{
			if (!vnavmeshIPC.IsPluginEnabled() || !vnavmeshIPC.GetIsNavReady())
			{
				worldPos = pos;
				return false;
			}
			Vector3? vector = vnavmeshIPC.QueryNearestPointOnMesh(pos, 5f, 10000f);
			if (vector.HasValue)
			{
				Vector3 valueOrDefault = vector.GetValueOrDefault();
				worldPos = valueOrDefault;
				return true;
			}
			worldPos = pos;
			return false;
		}

		public void qJHmU3upU2()
		{
			exdMarkersCache.Clear();
		}

		public unsafe void LgRmuMHcaq()
		{
			((AgentHUD)AgentHUD.Instance()).MapMarkers.Clear();
			AgentMap* intPtr = AgentMap.Instance();
			((AgentMap)intPtr).EventMarkers.Clear();
			((AgentMap)intPtr).MapMarkers.Clear();
			((AgentMap)intPtr).WarpMarkers.Clear();
			((AgentMap)intPtr).ResetMapMarkers();
			mapMarkers.Clear();
			questMapMarkers.Clear();
			MapMarkers = new Dictionary<uint, List<MapMarkerDisplay>>();
			QuestMapMarkers = new List<MapMarkerDisplay>();
		}

		public bool iyLmXRx01L(Vector3 pos, float radius, IGameObject? gameObject = null)
		{
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			_003C_003Ec__DisplayClass26_0 CS_0024_003C_003E8__locals21 = new _003C_003Ec__DisplayClass26_0();
			CS_0024_003C_003E8__locals21.gameObject = gameObject;
			CS_0024_003C_003E8__locals21._003C_003E4__this = this;
			CS_0024_003C_003E8__locals21.localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
			if (CS_0024_003C_003E8__locals21.localPlayer == null)
			{
				return false;
			}
			if (!wB8mdvinN2())
			{
				return false;
			}
			if (CS_0024_003C_003E8__locals21.gameObject == null)
			{
				CS_0024_003C_003E8__locals21.gameObject = iJSmYy7fW9(pos);
			}
			if (CS_0024_003C_003E8__locals21.gameObject == null)
			{
				return false;
			}
			float distance = Vector3.Distance(CS_0024_003C_003E8__locals21.localPlayer.Position, CS_0024_003C_003E8__locals21.gameObject.Position);
			float num = Vector2.Distance(CS_0024_003C_003E8__locals21.localPlayer.Position.ToVector2(), CS_0024_003C_003E8__locals21.gameObject.Position.ToVector2());
			if (Math.Abs(radius - 1f) < float.Epsilon && num > 15f)
			{
				return false;
			}
			if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)48])
			{
				return false;
			}
			if (!Nanvd65KxE(CS_0024_003C_003E8__locals21.gameObject.ObjectKind, distance) || DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)81])
			{
				_003Cmodule_003EP.TaskHelper.Abort();
				_003Cmodule_003EP.TaskHelper.Enqueue(delegate
				{
					Vector2 vector = Vector2.Normalize(CS_0024_003C_003E8__locals21.gameObject.Position.ToVector2() - CS_0024_003C_003E8__locals21.localPlayer.Position.ToVector2());
					return OmenServiceBase<MovementManager>.Instance().TPSmart_InZone(CS_0024_003C_003E8__locals21.gameObject.Position + vector.ToVector3(0f) * CS_0024_003C_003E8__locals21.gameObject.HitboxRadius);
				});
				_003Cmodule_003EP.TaskHelper.DelayNext(500);
				_003Cmodule_003EP.TaskHelper.Enqueue((Func<bool>)wB8mdvinN2, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
				_003Cmodule_003EP.TaskHelper.DelayNext(500);
				_003Cmodule_003EP.TaskHelper.Enqueue(delegate
				{
					if (!wB8mdvinN2())
					{
						return false;
					}
					TargetManager.Target = CS_0024_003C_003E8__locals21.gameObject;
					if (CS_0024_003C_003E8__locals21.gameObject.TargetInteract())
					{
						return true;
					}
					OmenServiceBase<MovementManager>.Instance().TPSmart_InZone(CS_0024_003C_003E8__locals21.gameObject.Position);
					return false;
				});
				return true;
			}
			_003Cmodule_003EP.TaskHelper.Enqueue(delegate
			{
				CS_0024_003C_003E8__locals21._003C_003E4__this._003Cmodule_003EP.UseEventItem?.InvokeAction();
			});
			return CS_0024_003C_003E8__locals21.gameObject.TargetInteract();
		}

		private unsafe static IGameObject? iJSmYy7fW9(Vector3 pos)
		{
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			_003C_003Ec__DisplayClass27_0 CS_0024_003C_003E8__locals5 = new _003C_003Ec__DisplayClass27_0();
			CS_0024_003C_003E8__locals5.pos = pos;
			if (IOmenDalamudService<IObjectTable>.Instance().LocalPlayer == null)
			{
				return null;
			}
			CS_0024_003C_003E8__locals5.questIDs = new HashSet<uint>();
			Span<QuestWork> normalQuests = ((QuestManager)QuestManager.Instance()).NormalQuests;
			for (int i = 0; i < normalQuests.Length; i++)
			{
				QuestWork val = normalQuests[i];
				if (val.QuestId != 0)
				{
					CS_0024_003C_003E8__locals5.questIDs.Add((uint)(val.QuestId + 65536));
				}
			}
			return (from x in ((IEnumerable<Pointer<EventHandler>>)(object)((EventHandlerModule)(&((EventFramework)EventFramework.Instance()).EventHandlerModule)).EventHandlerMap.Values).Where((Pointer<EventHandler> x) => x.Value != null && CS_0024_003C_003E8__locals5.questIDs.Contains(((EventId)(&((EventHandlerInfo)(&((EventHandler)x.Value).Info)).EventId)).Id) && (int)((EventId)(&((EventHandlerInfo)(&((EventHandler)x.Value).Info)).EventId)).ContentId == 1).SelectMany((Pointer<EventHandler> x) => (IEnumerable<Pointer<GameObject>>)(object)((EventHandler)x.Value).EventObjects).Select(Lk7mzoEkZS)
				where x != null
				select x).OfType<IGameObject>().MinBy((IGameObject x) => Vector3.DistanceSquared(CS_0024_003C_003E8__locals5.pos, x.Position));
		}

		private unsafe static IGameObject? Lk7mzoEkZS(Pointer<GameObject> eventObject)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			GameObject* value = eventObject.Value;
			if (value == null || !((GameObject)value).GetIsTargetable() || !((GameObject)value).TargetableStatus.IsSet<ObjectTargetableFlags>((ObjectTargetableFlags)64))
			{
				return null;
			}
			IGameObject gameObject = IOmenDalamudService<IObjectTable>.Instance().CreateObjectReference((nint)value);
			if (gameObject == null || !gameObject.IsMTQ())
			{
				return null;
			}
			return gameObject;
		}

		public unsafe static List<Vector3> AGbinJhK5w(uint questRowID)
		{
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			EventHandler* eventHandlerById = ((EventFramework)EventFramework.Instance()).GetEventHandlerById((ushort)questRowID);
			if (eventHandlerById == null || ((EventHandler)eventHandlerById).EventObjects.Count == 0)
			{
				return new List<Vector3>();
			}
			List<Vector3> list = new List<Vector3>(((EventHandler)eventHandlerById).EventObjects.Count);
			Enumerator<Pointer<GameObject>, Pointer<GameObject>, DefaultKeyExtractor<Pointer<GameObject>>> enumerator = ((EventHandler)eventHandlerById).EventObjects.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Pointer<GameObject> current = enumerator.Current;
					GameObject* value = current.Value;
					if (value != null && ((GameObject)value).TargetableStatus.IsSet<ObjectTargetableFlags>((ObjectTargetableFlags)64))
					{
						list.Add(Vector3.op_Implicit(((GameObject)value).Position));
					}
				}
				return list;
			}
			finally
			{
				((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
			}
		}

		public void LmciyQ4tpP(MapMarkerDisplay marker, bool isInteract = false)
		{
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			_003C_003Ec__DisplayClass30_0 CS_0024_003C_003E8__locals28 = new _003C_003Ec__DisplayClass30_0();
			CS_0024_003C_003E8__locals28._003C_003E4__this = this;
			CS_0024_003C_003E8__locals28.marker = marker;
			_003Cmodule_003EP.TaskHelper.Abort();
			CS_0024_003C_003E8__locals28.localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
			if (CS_0024_003C_003E8__locals28.localPlayer == null || (CS_0024_003C_003E8__locals28.marker.IconID == 60441 && NfFiLka2AR(CS_0024_003C_003E8__locals28.marker.Name)) || !fPYv70kUbr())
			{
				return;
			}
			float num = LocalPlayerState.DistanceTo2DSquared(CS_0024_003C_003E8__locals28.marker.Position.ToVector2());
			if ((Math.Abs(CS_0024_003C_003E8__locals28.marker.Radius - 1f) < float.Epsilon) ? (num <= 121f) : (num <= CS_0024_003C_003E8__locals28.marker.Radius * CS_0024_003C_003E8__locals28.marker.Radius))
			{
				CS_0024_003C_003E8__locals28.mtqNearby = iJSmYy7fW9(CS_0024_003C_003E8__locals28.marker.Position);
				if (CS_0024_003C_003E8__locals28.mtqNearby != null && Nanvd65KxE(CS_0024_003C_003E8__locals28.mtqNearby.ObjectKind, LocalPlayerState.DistanceToObject3D(CS_0024_003C_003E8__locals28.mtqNearby)))
				{
					_003Cmodule_003EP.TaskHelper.Enqueue(() => CS_0024_003C_003E8__locals28._003C_003E4__this.iyLmXRx01L(CS_0024_003C_003E8__locals28.marker.Position, CS_0024_003C_003E8__locals28.marker.Radius, CS_0024_003C_003E8__locals28.mtqNearby));
					return;
				}
			}
			_003Cmodule_003EP.TaskHelper.Enqueue(delegate
			{
				Vector2 vector = Vector2.Normalize(CS_0024_003C_003E8__locals28.marker.Position.ToVector2() - CS_0024_003C_003E8__locals28.localPlayer.Position.ToVector2());
				return OmenServiceBase<MovementManager>.Instance().TPSmart_InZone(CS_0024_003C_003E8__locals28.marker.Position + ((CS_0024_003C_003E8__locals28.marker.Radius == 1f) ? vector.ToVector3(0f) : Vector3.Zero));
			});
			_003Cmodule_003EP.TaskHelper.DelayNext(500);
			_003Cmodule_003EP.TaskHelper.Enqueue((Func<bool>)wB8mdvinN2, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
			_003Cmodule_003EP.TaskHelper.Enqueue(delegate
			{
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				RaycastHit hitInfo;
				Vector3 pos = (RaycastHelper.TryGetNearestVerticalHit(CS_0024_003C_003E8__locals28.localPlayer.Position + new Vector3(0f, 2f, 0f), out hitInfo) ? hitInfo.Point : CS_0024_003C_003E8__locals28.marker.Position);
				OmenServiceBase<MovementManager>.Instance().TPSmart_InZone(pos);
			});
			_003Cmodule_003EP.TaskHelper.DelayNext(500);
			_003Cmodule_003EP.TaskHelper.Enqueue((Func<bool>)wB8mdvinN2, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
			if (isInteract)
			{
				_003Cmodule_003EP.TaskHelper.Enqueue(() => CS_0024_003C_003E8__locals28._003C_003E4__this.iyLmXRx01L(CS_0024_003C_003E8__locals28.marker.Position, CS_0024_003C_003E8__locals28.marker.Radius));
			}
		}

		private unsafe bool NfFiLka2AR(string zonePlaceName)
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			if (((LayoutWorld)LayoutWorld.Instance()).ActiveLayout != null)
			{
				StdMap<InstanceType, Pointer<StdMap<ulong, Pointer<ILayoutInstance>>>>* instancesByType = &((LayoutManager)((LayoutWorld)LayoutWorld.Instance()).ActiveLayout).InstancesByType;
				InstanceType val = (InstanceType)41;
				Pointer<StdMap<ulong, Pointer<ILayoutInstance>>>* ptr = default(Pointer<StdMap<ulong, Pointer<ILayoutInstance>>>*);
				if (instancesByType->TryGetValuePointer(ref val, ref ptr))
				{
					ValueEnumerator<ulong, Pointer<ILayoutInstance>> enumerator = ptr->Value->Values.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							Pointer<ILayoutInstance> current = enumerator.Current;
							_003C_003Ec__DisplayClass31_0 CS_0024_003C_003E8__locals5 = new _003C_003Ec__DisplayClass31_0();
							CS_0024_003C_003E8__locals5.instance = (ExitRangeLayoutInstance*)current.Value;
							if (CS_0024_003C_003E8__locals5.instance != null && !(LuminaWrapper.GetZonePlaceName(((ExitRangeLayoutInstance)CS_0024_003C_003E8__locals5.instance).TerritoryType) != zonePlaceName))
							{
								_003Cmodule_003EP.TaskHelper.Enqueue(delegate
								{
									PopRangeManager.Instance()->PopRange((ILayoutInstance*)CS_0024_003C_003E8__locals5.instance);
								});
								_003Cmodule_003EP.TaskHelper.Enqueue(() => IConditionExtension.get_IsBetweenAreas(DalamudServiceExtension.Instance<ICondition>()));
								_003Cmodule_003EP.TaskHelper.Enqueue(delegate
								{
									PopRangeManager.Instance()->PopRange((ILayoutInstance*)CS_0024_003C_003E8__locals5.instance);
								});
								return true;
							}
						}
					}
					finally
					{
						((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
					}
					return false;
				}
			}
			return false;
		}

		[CompilerGenerated]
		private uint tlUiHlwpBG(KeyValuePair<uint, List<MapMarkerDisplay>> x)
		{
			return _003Cmodule_003EP.config.IconWeight.GetValueOrDefault(x.Key, x.Key / 10000);
		}
	}

	public sealed class MapMarkerDisplay : IEquatable<MapMarkerDisplay>
	{
		[CompilerGenerated]
		private readonly Vector3 _003CPosition_003Ek__BackingField;

		public required string Name { get; init; }

		public required uint IconID { get; init; }

		public required Vector3 Position
		{
			[CompilerGenerated]
			get
			{
				return _003CPosition_003Ek__BackingField;
			}
			init
			{
				if (!vnavmeshIPC.IsPluginEnabled() || !vnavmeshIPC.GetIsNavReady())
				{
					_003CPosition_003Ek__BackingField = value;
				}
				else
				{
					_003CPosition_003Ek__BackingField = vnavmeshIPC.QueryNearestPointOnMesh(value, 5f, 10000f) ?? value;
				}
			}
		}

		public required float Radius { get; init; }

		public string ExtraInfo { get; init; }

		public bool IsQuest { get; init; }

		public bool IsPositionExact { get; init; }

		public bool IsUsable
		{
			get
			{
				if (IconID == 0)
				{
					return !string.IsNullOrEmpty(Name);
				}
				return true;
			}
		}

		public string IconLabel
		{
			get
			{
				string addonText = LuminaWrapper.GetAddonText(IconID);
				if (addonText == null || addonText.Length <= 0)
				{
					return IconID.ToString();
				}
				return addonText;
			}
		}

		public bool Equals(MapMarkerDisplay? other)
		{
			if (other == null)
			{
				return false;
			}
			if (this == other)
			{
				return true;
			}
			if (Name == other.Name && IconID == other.IconID && Position == other.Position)
			{
				return Radius.Equals(other.Radius);
			}
			return false;
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
			if (obj is MapMarkerDisplay other)
			{
				return Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Name, IconID, Position, Radius);
		}

		public MapMarkerDisplay ToFateGroupMarker()
		{
			return new MapMarkerDisplay
			{
				Name = Name,
				IconID = 60501u,
				Position = Position,
				Radius = Radius,
				ExtraInfo = ExtraInfo,
				IsPositionExact = IsPositionExact
			};
		}

		public unsafe static MapMarkerDisplay? Parse(MapMarkerData marker)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			if (marker.TooltipString == null || ((Utf8String)marker.TooltipString).IsEmpty || !((CStringPointer)(&((Utf8String)marker.TooltipString).StringPtr)).HasValue)
			{
				return null;
			}
			Vector3 position = marker.Position;
			uint num = marker.IconId;
			ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
			GameIconLookup val = new GameIconLookup(num, false, true, (ClientLanguage?)null);
			ISharedImmediateTexture val2 = default(ISharedImmediateTexture);
			if (!obj.TryGetFromGameIcon(ref val, ref val2))
			{
				num = 0u;
			}
			string extraInfo = string.Empty;
			bool isQuest = false;
			byte markerType = marker.MarkerType;
			bool flag = (uint)(markerType - 3) <= 1u;
			if (flag && marker.IconId != 63933)
			{
				isQuest = true;
				extraInfo = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2310AFF3 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a96fab9e97944b39a8afe3fb1109ba7d) + LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4C421E28 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4643f8ec0000417ba22db1f06f6757bd)) + LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7EE77593 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88432e96079448078bdf00e7b62f8ab6);
			}
			return new MapMarkerDisplay
			{
				Name = CStringExtensions.ExtractText(((Utf8String)marker.TooltipString).StringPtr),
				IconID = num,
				Position = position,
				Radius = marker.Radius,
				ExtraInfo = extraInfo,
				IsQuest = isQuest,
				IsPositionExact = true
			};
		}

		public unsafe static MapMarkerDisplay? Parse(MapMarkerBase marker)
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			if (!((CStringPointer)(ref marker.Subtext)).HasValue)
			{
				return null;
			}
			if (!LuminaGetter.TryGetRow<Map>(((AgentMap)AgentMap.Instance()).CurrentMapId, out Map item))
			{
				return null;
			}
			return new MapMarkerDisplay
			{
				Name = CStringExtensions.ExtractText(marker.Subtext),
				IconID = marker.IconId,
				Position = mpyiSeRPcn(marker.X, marker.Y, item),
				Radius = 1f,
				IsPositionExact = false
			};
		}

		public unsafe static MapMarkerDisplay? Parse(MapMarker marker)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			if (!LuminaGetter.TryGetRow<Map>(((AgentMap)AgentMap.Instance()).CurrentMapId, out Map item))
			{
				return null;
			}
			string name2;
			string name = (AetheryteRecord.TryParseName(marker, out name2) ? name2 : marker.GetMarkerPlaceName());
			return new MapMarkerDisplay
			{
				Name = name,
				IconID = ((MapMarker)(ref marker)).Icon,
				Position = mpyiSeRPcn(((MapMarker)(ref marker)).X, ((MapMarker)(ref marker)).Y, item),
				Radius = 1f,
				IsPositionExact = false
			};
		}

		public static MapMarkerDisplay? Parse(HousingMapMarkerInfo marker)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			Map? valueNullable = ((HousingMapMarkerInfo)(ref marker)).Map.ValueNullable;
			if (valueNullable.HasValue)
			{
				Map valueOrDefault = valueNullable.GetValueOrDefault();
				string text;
				if (((HousingMapMarkerInfo)(ref marker)).SubrowId + 1 != 61)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(2, 2);
					PlaceName? valueNullable2 = ((Map)(ref valueOrDefault)).PlaceName.ValueNullable;
					object obj;
					if (!valueNullable2.HasValue)
					{
						obj = null;
					}
					else
					{
						PlaceName valueOrDefault2 = valueNullable2.GetValueOrDefault();
						obj = ((object)((PlaceName)(ref valueOrDefault2)).Name/*cast due to constrained. prefix*/).ToString();
					}
					if (obj == null)
					{
						obj = string.Empty;
					}
					defaultInterpolatedStringHandler.AppendFormatted((string?)obj);
					defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x64E5B392 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f53034f3efc64a309b10c7885bf5d9bd));
					defaultInterpolatedStringHandler.AppendFormatted(((HousingMapMarkerInfo)(ref marker)).SubrowId + 1);
					text = defaultInterpolatedStringHandler.ToStringAndClear();
				}
				else
				{
					text = LuminaWrapper.GetAddonText(6760u);
				}
				string name = text;
				return new MapMarkerDisplay
				{
					Name = name,
					IconID = 60752u,
					Position = new Vector3(((HousingMapMarkerInfo)(ref marker)).X, ((HousingMapMarkerInfo)(ref marker)).Y, ((HousingMapMarkerInfo)(ref marker)).Z),
					Radius = 1f
				};
			}
			return null;
		}

		public static MapMarkerDisplay? Parse(IFate fate)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Invalid comparison between Unknown and I4
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Invalid comparison between Unknown and I4
			if ((int)fate.State == 3)
			{
				return null;
			}
			string text;
			if ((int)fate.State != 4)
			{
				text = string.Empty;
			}
			else
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(2, 2);
				defaultInterpolatedStringHandler.AppendFormatted(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x43D7CFAE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_974d377e1ed74986b81c34c95c2cc846)));
				defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4753ADC4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_790eb00575b3425495c924ac13469f4c));
				defaultInterpolatedStringHandler.AppendFormatted(TimeSpan.FromSeconds(fate.TimeRemaining), LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7C2FFD50 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_12c2d68ba4654622a4959ee73cacc6d7));
				text = defaultInterpolatedStringHandler.ToStringAndClear();
			}
			string extraInfo = text;
			MapMarkerDisplay mapMarkerDisplay = new MapMarkerDisplay();
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(4, 2);
			defaultInterpolatedStringHandler2.AppendFormatted(fate.Name.TextValue);
			defaultInterpolatedStringHandler2.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x39879337 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_56ae6e3f2cdf4dd8a743f4d493ce49a5));
			defaultInterpolatedStringHandler2.AppendFormatted(fate.Progress);
			defaultInterpolatedStringHandler2.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3951E481 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3aeae81f85f84848b273be7b2bfbf497));
			mapMarkerDisplay.Name = defaultInterpolatedStringHandler2.ToStringAndClear();
			mapMarkerDisplay.IconID = fate.MapIconId;
			mapMarkerDisplay.Position = fate.Position;
			mapMarkerDisplay.Radius = fate.Radius;
			mapMarkerDisplay.ExtraInfo = extraInfo;
			mapMarkerDisplay.IsPositionExact = false;
			return mapMarkerDisplay;
		}

		public static MapMarkerDisplay? Parse(IPartyMember member)
		{
			if (member.ContentId == LocalPlayerState.ContentID)
			{
				return null;
			}
			MapMarkerDisplay obj = new MapMarkerDisplay
			{
				Name = ((object)member.Name).ToString(),
				IconID = 61572u,
				Position = member.Position,
				Radius = 1f
			};
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(2, 2);
			defaultInterpolatedStringHandler.AppendFormatted(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-1675397288 ^ -1179876446 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_dadeb2ab61424d31be7cc1966e675825)));
			defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x51E5A98E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a90a92322bff42e180982e106200d4a3));
			defaultInterpolatedStringHandler.AppendFormatted(member.Level);
			obj.ExtraInfo = defaultInterpolatedStringHandler.ToStringAndClear();
			return obj;
		}

		public static MapMarkerDisplay? Parse(Warp warp)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			Level? valueNullable = ((Warp)(ref warp)).PopRange.ValueNullable;
			if (valueNullable.HasValue)
			{
				Level level = valueNullable.GetValueOrDefault();
				if (((Warp)(ref warp)).PopRange.RowId != 0)
				{
					string name = ((!string.IsNullOrWhiteSpace(((object)((Warp)(ref warp)).Name/*cast due to constrained. prefix*/).ToString())) ? ((object)((Warp)(ref warp)).Name/*cast due to constrained. prefix*/).ToString() : ((object)((Warp)(ref warp)).Question/*cast due to constrained. prefix*/).ToString());
					return new MapMarkerDisplay
					{
						Name = name,
						IconID = 60441u,
						Position = level.GetPosition(),
						Radius = ((Level)(ref level)).Radius
					};
				}
			}
			return null;
		}

		private static Vector3 mpyiSeRPcn(short x, short y, Map map)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			return PositionHelper.TextureToWorld(new Vector2(x, y), map).ToVector3((float)Math.Round(IOmenDalamudService<IObjectTable>.Instance().LocalPlayer?.Position.Y ?? 0f, 2));
		}

		public MapMarkerDisplay()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			ExtraInfo = string.Empty;
			IsPositionExact = true;
			base._002Ector();
		}
	}

	private sealed class Voj6hUiVRAlraSThWay : Window
	{
		private bool isDraggingButton;

		private Vector2 dragStartPos;

		public double FlashStartTime { get; set; }

		public Voj6hUiVRAlraSThWay(FastTeleportPanel module)
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			_003Cmodule_003EP = module;
			((Window)this)._002Ector(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-1003249140 ^ -2005811762 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4643f8ec0000417ba22db1f06f6757bd), (ImGuiWindowFlags)2105579, false);
		}

		public override void Draw()
		{
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			Overlay overlay = _003Cmodule_003EP.Overlay;
			if (overlay != null && ((Window)overlay).IsOpen)
			{
				((Window)overlay).IsOpen = false;
			}
			else
			{
				if (DalamudLeftServiceExtension.Instance().CutsceneActive || !fPYv70kUbr())
				{
					return;
				}
				ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
				GameIconLookup val = GameIconLookup.op_Implicit(60453u);
				ISharedImmediateTexture val2 = default(ISharedImmediateTexture);
				if (!obj.TryGetFromGameIcon(ref val, ref val2))
				{
					return;
				}
				Vector2 windowPos = ImGui.GetWindowPos();
				IDalamudTextureWrap wrapOrEmpty = val2.GetWrapOrEmpty();
				ColorDisposable val3 = ImRaii.PushColor((ImGuiCol)21, 0u, true);
				try
				{
					ColorDisposable val4 = ImRaii.PushColor((ImGuiCol)22, 0u, true);
					try
					{
						ColorDisposable val5 = ImRaii.PushColor((ImGuiCol)23, 0u, true);
						try
						{
							float num = 1f;
							double num2 = ImGui.GetTime() - FlashStartTime;
							if (num2 < 2.5)
							{
								num = 0.3f + 0.7f * (float)Math.Abs(Math.Cos(num2 * 4.0));
							}
							if (ImGui.ImageButton(wrapOrEmpty.Handle, Globals.ScaledVector2(32f), Vector2.Zero, Vector2.One, -1, Vector4.Zero, VectorExtensions.WithW(Vector4.One, num)))
							{
								Overlay? overlay2 = _003Cmodule_003EP.Overlay;
								((Window)overlay2).IsOpen = !((Window)overlay2).IsOpen;
							}
						}
						finally
						{
							((IDisposable)val5)?.Dispose();
						}
					}
					finally
					{
						((IDisposable)val4)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)val3)?.Dispose();
				}
				if (ImGui.IsItemHovered() && ImGui.IsMouseClicked((ImGuiMouseButton)1))
				{
					isDraggingButton = true;
					dragStartPos = ImGui.GetMousePos();
				}
				if (isDraggingButton)
				{
					Vector2 mousePos = ImGui.GetMousePos();
					Vector2 vector = mousePos - dragStartPos;
					if (vector.X != 0f || vector.Y != 0f)
					{
						ImGui.SetWindowPos(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-1675397288 ^ -1626463531 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_86d75c161ced498ab6a767d2eb82d38e)), new Vector2(windowPos.X + vector.X, windowPos.Y + vector.Y), (ImGuiCond)0);
						dragStartPos = mousePos;
					}
					if (ImGui.IsMouseReleased((ImGuiMouseButton)1))
					{
						isDraggingButton = false;
					}
				}
			}
		}
	}

	private class wxRn0Ti9PXswLNJBYk3 : gdUpfLisf7qnNj7E3Em
	{
		protected override uint ID => 1u;

		public wxRn0Ti9PXswLNJBYk3(FastTeleportPanel module)
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector(module);
		}

		public override void A62ofKGjwQ()
		{
			DMqi8Y1X46((FontAwesomeIcon)62023, LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x32D55B7 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_86d75c161ced498ab6a767d2eb82d38e)));
		}

		public override void Draw()
		{
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			foreach (KeyValuePair<uint, List<MapMarkerDisplay>> mapMarker in Module.markerController.MapMarkers)
			{
				mapMarker.Deconstruct(out var key, out var value);
				uint key2 = key;
				List<MapMarkerDisplay> list = value;
				MapMarkerDisplay mapMarkerDisplay = list.FirstOrDefault((MapMarkerDisplay x) => x.IsUsable);
				if (mapMarkerDisplay != null)
				{
					Vector4 vector = KnownColor.Chocolate.ToVector4();
					ImGui.TextColored(ref vector, ImU8String.op_Implicit((Module.config.IconNotes.TryGetValue(key2, out string value2) && !string.IsNullOrWhiteSpace(value2)) ? value2 : mapMarkerDisplay.IconLabel));
					ImGui.Separator();
					vaEivTUucR(list);
					ImGuiOm.ScaledDummy(2f);
				}
			}
		}
	}

	private class WGq9yRif3JcAHmNt7GV : gdUpfLisf7qnNj7E3Em
	{
		private string customIconNameInput;

		private Vector3 customPositionInput;

		private float customRadiusInput;

		private bool customIsHeightNeedToCorrectInput;

		private Vector3 lastPos;

		private Vector3 manualPos;

		protected override uint ID => 2u;

		public WGq9yRif3JcAHmNt7GV(FastTeleportPanel module)
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			customIconNameInput = string.Empty;
			base._002Ector(module);
		}

		public override void A62ofKGjwQ()
		{
			DMqi8Y1X46((FontAwesomeIcon)61618, LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x17785231 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f7cf93b006904393bb56d82c0a688fa1)));
		}

		public override void Draw()
		{
			v0XiC9tYDG();
			ImGuiOm.ScaledDummy(4f);
			RO4ijmqDMR();
			ImGuiOm.ScaledDummy(4f);
			DIPiK1ywWs();
		}

		private void v0XiC9tYDG()
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0665: Unknown result type (might be due to invalid IL or missing references)
			//IL_069d: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ba: Unknown result type (might be due to invalid IL or missing references)
			bool flag = GameState.IsFlagMarkerSet && GameState.FlagMarker.TerritoryId == GameState.TerritoryType;
			Vector3 vector = LocalPlayerState.Object?.Position ?? default(Vector3);
			Vector4 vector2 = KnownColor.Chocolate.ToVector4();
			ImGui.TextColored(ref vector2, ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4753775A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_790eb00575b3425495c924ac13469f4c))));
			ImGui.Separator();
			ImGui.Spacing();
			ImGui.AlignTextToFramePadding();
			uint num = KnownColor.Orange.ToUInt();
			ImU8String val = default(ImU8String);
			((ImU8String)(ref val))._002Ector(1, 1);
			((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xBA41F60 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2edd08450a8b42e6997855e5217d6d55)));
			((ImU8String)(ref val)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x369533BB ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_26e231ba68f74e8784503e099ddfc2d9));
			ImGui.TextColored(num, val);
			ImGui.SameLine();
			if (!GameState.IsFlagMarkerSet)
			{
				ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x43D7CEA4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_974d377e1ed74986b81c34c95c2cc846))));
			}
			else
			{
				ImU8String val2 = default(ImU8String);
				((ImU8String)(ref val2))._002Ector(4, 2);
				((ImU8String)(ref val2)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4CF208A2 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a07aac30eb32484e94cb4d1e62117765));
				((ImU8String)(ref val2)).AppendFormatted<float>(GameState.FlagMarker.XFloat, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x45E4C6F7 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1904f95d2a394c929e1a6d4f72943ad7));
				((ImU8String)(ref val2)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4CF2D582 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a07aac30eb32484e94cb4d1e62117765));
				((ImU8String)(ref val2)).AppendFormatted<float>(GameState.FlagMarker.YFloat, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-864210836 ^ -620403207 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f7cf93b006904393bb56d82c0a688fa1));
				((ImU8String)(ref val2)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3D56A9A0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e9e296e241f44828a57c750102ce71c1));
				ImGui.TextUnformatted(val2);
				ImGui.SameLine();
				uint num2 = (flag ? KnownColor.White.ToUInt() : KnownColor.OrangeRed.ToUInt());
				ImU8String val3 = default(ImU8String);
				((ImU8String)(ref val3))._002Ector(2, 1);
				((ImU8String)(ref val3)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xBA4C21A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2edd08450a8b42e6997855e5217d6d55));
				((ImU8String)(ref val3)).AppendFormatted<string>(LuminaWrapper.GetZonePlaceName(GameState.FlagMarker.TerritoryId));
				((ImU8String)(ref val3)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x1778ADA1 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f7cf93b006904393bb56d82c0a688fa1));
				ImGui.TextColored(num2, val3);
			}
			DisabledDisposable val4 = ImRaii.Disabled(!GameState.IsFlagMarkerSet);
			try
			{
				if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61476, LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5E3B2668 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2d7d8a5df6744c51b35f486f03b53ded)), useStaticFont: true))
				{
					Module.TaskHelper.Abort();
					Vector3 vector3 = GameState.FlagMarkerPosition.ToVector3(in vector.Y);
					lastPos = vector;
					OmenServiceBase<MovementManager>.Instance().TPSmart_InZone(lD0TdJmaaXGs4MmrNMs.JPMmIw63WD(vector3, out var worldPos) ? worldPos : vector3);
				}
			}
			finally
			{
				((IDisposable)val4)?.Dispose();
			}
			ImGui.Spacing();
			ImGui.AlignTextToFramePadding();
			uint num3 = KnownColor.Orange.ToUInt();
			ImU8String val5 = default(ImU8String);
			((ImU8String)(ref val5))._002Ector(1, 1);
			((ImU8String)(ref val5)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5CE3AD75 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3054c6e6ef664be696bb2ad005864818)));
			((ImU8String)(ref val5)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3D56AAE8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e9e296e241f44828a57c750102ce71c1));
			ImGui.TextColored(num3, val5);
			ImGui.SameLine();
			ImU8String val6 = default(ImU8String);
			((ImU8String)(ref val6))._002Ector(6, 3);
			((ImU8String)(ref val6)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5CE31509 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3054c6e6ef664be696bb2ad005864818));
			((ImU8String)(ref val6)).AppendFormatted<float>(vector.X, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--1470179789 ^ 0x3112395F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_762f55d0e7ef4ecd8f3f55c0f8a10fae));
			((ImU8String)(ref val6)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x685419B3 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_23522a2a14314da28eb44187a09cd0b5));
			((ImU8String)(ref val6)).AppendFormatted<float>(vector.Y, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xE092779 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_aba6092d7cea44a49d1667540500e363));
			((ImU8String)(ref val6)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x53572DFE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_0676edd1b45a4ff096830fd6229bc1fb));
			((ImU8String)(ref val6)).AppendFormatted<float>(vector.Z, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5017A3EC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e227921f7ee94b6aa1c28169395ba0cf));
			((ImU8String)(ref val6)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6E008CAE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c40146824d9043e0906f4e1889cf39aa));
			ImGui.TextUnformatted(val6);
			GroupDisposable val7 = ImRaii.Group();
			try
			{
				for (int i = 0; i < AXIS_NAMES.Length; i++)
				{
					if (i != 0)
					{
						ImGui.SameLine();
					}
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 2);
					defaultInterpolatedStringHandler.AppendFormatted(AXIS_NAMES[i]);
					defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3AC9D77A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3932528062874ea4bb7e2e1185394e37));
					defaultInterpolatedStringHandler.AppendFormatted(Module.config.PositionOffsetDelta);
					BYuitaV9vx(defaultInterpolatedStringHandler.ToStringAndClear(), i, -Module.config.PositionOffsetDelta);
					ImGui.SameLine();
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(3, 2);
					defaultInterpolatedStringHandler2.AppendFormatted(AXIS_NAMES[i]);
					defaultInterpolatedStringHandler2.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--1976305562 ^ 0x3EC3350E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_8e002b0644ef421698ef6f63772929af));
					defaultInterpolatedStringHandler2.AppendFormatted(Module.config.PositionOffsetDelta);
					BYuitaV9vx(defaultInterpolatedStringHandler2.ToStringAndClear(), i, Module.config.PositionOffsetDelta);
				}
			}
			finally
			{
				((GroupDisposable)(ref val7)).Dispose();
			}
			ImGui.Spacing();
			ImGui.AlignTextToFramePadding();
			uint num4 = KnownColor.Orange.ToUInt();
			ImU8String val8 = default(ImU8String);
			((ImU8String)(ref val8))._002Ector(1, 1);
			((ImU8String)(ref val8)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489F9906 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b2294f2f9bb24a76b00cf1af75888414)));
			((ImU8String)(ref val8)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5DD70109 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c7355392ad9e43b6b82798cc42ea6c6f));
			ImGui.TextColored(num4, val8);
			ImGui.SameLine();
			ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
			ImGui.InputFloat3(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6FF11A00 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5859f4ea3042459db2c9b610b3099912)), ref manualPos, 0f, 0f, default(ImU8String), (ImGuiInputTextFlags)0);
			val4 = ImRaii.Disabled(manualPos == Vector3.Zero);
			try
			{
				if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61531, LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xF20A26D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_fea4ad1b13de4c4bbc22f571ceda7372)), useStaticFont: true))
				{
					Module.TaskHelper.Abort();
					lastPos = manualPos;
					OmenServiceBase<MovementManager>.Instance().TPSmart_InZone(manualPos);
				}
			}
			finally
			{
				((IDisposable)val4)?.Dispose();
			}
			ImGui.Spacing();
			val4 = ImRaii.Disabled(lastPos == Vector3.Zero);
			try
			{
				if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61666, LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x76EB6F09 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b988ddaee02846b69dd4e446b5da9de9)) ?? "", useStaticFont: true))
				{
					Module.TaskHelper.Abort();
					OmenServiceBase<MovementManager>.Instance().TPSmart_InZone(lastPos);
				}
			}
			finally
			{
				((IDisposable)val4)?.Dispose();
			}
			ImGui.SameLine();
			if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61565, LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x317A4CF ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_99910d0c1e594d99b604e0aab1195273))))
			{
				OmenServiceBase<MovementManager>.Instance().TPGround();
			}
			if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)81])
			{
				ImGui.SameLine();
				if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)62916, LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x420C2922 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_84ee025b19bb4145ba9b4b6b7acb91eb))))
				{
					OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommandComplexLocation(ExecuteCommandComplexFlag.SetDivingState);
				}
			}
		}

		private void RO4ijmqDMR()
		{
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_030a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_041d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0422: Unknown result type (might be due to invalid IL or missing references)
			//IL_046e: Unknown result type (might be due to invalid IL or missing references)
			//IL_049b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0543: Unknown result type (might be due to invalid IL or missing references)
			//IL_0548: Unknown result type (might be due to invalid IL or missing references)
			//IL_05aa: Unknown result type (might be due to invalid IL or missing references)
			ItemWidthDisposable val = ImRaii.ItemWidth(200f * Globals.GlobalUIScale);
			try
			{
				Vector4 vector = KnownColor.Chocolate.ToVector4();
				ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6E005036 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c40146824d9043e0906f4e1889cf39aa))));
				ImGui.Separator();
				ImGui.Spacing();
				GroupDisposable val2 = ImRaii.Group();
				try
				{
					ImU8String val3 = new ImU8String(25, 1);
					((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7EE7B3E9 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88432e96079448078bdf00e7b62f8ab6)));
					((ImU8String)(ref val3)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x420C2AAE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_84ee025b19bb4145ba9b4b6b7acb91eb));
					ImGui.InputText(val3, ref customIconNameInput, 128, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
					ImU8String val4 = new ImU8String(23, 1);
					((ImU8String)(ref val4)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x1C1788E5 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_accf3844f1c6464481df1849f0aee1bb)));
					((ImU8String)(ref val4)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x68541BE7 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_23522a2a14314da28eb44187a09cd0b5));
					ImU8String val5 = val4;
					ref uint customIconIDInput = ref Module.config.CustomIconIDInput;
					ImU8String val6 = default(ImU8String);
					GameIconLookup val7;
					if (ImGui.InputUInt(val5, ref customIconIDInput, 0u, 0u, val6, (ImGuiInputTextFlags)0))
					{
						ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
						val7 = new GameIconLookup(Module.config.CustomIconIDInput, false, true, (ClientLanguage?)null);
						ISharedImmediateTexture val8 = default(ISharedImmediateTexture);
						if (!obj.TryGetFromGameIcon(ref val7, ref val8))
						{
							Module.config.CustomIconIDInput = 60561u;
						}
					}
					ImGui.SameLine();
					if (ImGuiOm.ButtonIcon(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x1778572B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f7cf93b006904393bb56d82c0a688fa1), (FontAwesomeIcon)63597, LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x258F6F54 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_dadeb2ab61424d31be7cc1966e675825))))
					{
						OmenServiceBase<ChatManager>.Instance().SendMessage(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x53572F38 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_0676edd1b45a4ff096830fd6229bc1fb));
					}
					ITextureProvider obj2 = DalamudServiceExtension.Instance<ITextureProvider>();
					val7 = GameIconLookup.op_Implicit(Module.config.CustomIconIDInput);
					ISharedImmediateTexture val9 = default(ISharedImmediateTexture);
					if (obj2.TryGetFromGameIcon(ref val7, ref val9))
					{
						ImTextureID handle = val9.GetWrapOrEmpty().Handle;
						ImGui.SameLine();
						ImGui.Image(handle, new Vector2(ImGui.GetFrameHeight()));
						if (ImGui.IsItemHovered())
						{
							TooltipDisposable val10 = ImRaii.Tooltip();
							try
							{
								ImGui.Image(handle, Globals.ScaledVector2(72f));
							}
							finally
							{
								((TooltipDisposable)(ref val10)).Dispose();
							}
						}
					}
					val6 = new ImU8String(29, 1);
					((ImU8String)(ref val6)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2CE54827 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c9dc773883f149b7befba3bdbeac55f7)));
					((ImU8String)(ref val6)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4CF2D6B8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a07aac30eb32484e94cb4d1e62117765));
					ImGui.InputFloat3(val6, ref customPositionInput, 0f, 0f, ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4C42EB96 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4643f8ec0000417ba22db1f06f6757bd)), (ImGuiInputTextFlags)0);
					ImGui.SameLine();
					if (ImGuiOm.ButtonIcon(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4B0F3158 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_8e002b0644ef421698ef6f63772929af), (FontAwesomeIcon)61618, LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x68ACFD06 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4722689f1a4d41dc9dc800bfe1a89c09))))
					{
						customPositionInput = LocalPlayerState.Object?.Position ?? default(Vector3);
					}
					ImGui.SameLine();
					if (ImGuiOm.ButtonIcon(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3695EEC3 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_26e231ba68f74e8784503e099ddfc2d9), (FontAwesomeIcon)61476, LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x76EB6D97 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b988ddaee02846b69dd4e446b5da9de9))) && GameState.IsFlagMarkerSet && GameState.FlagMarker.TerritoryId == GameState.TerritoryType)
					{
						customPositionInput = GameState.FlagMarkerPosition.ToVector3(in customPositionInput.Y);
					}
					ImU8String val11 = new ImU8String(27, 1);
					((ImU8String)(ref val11)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6C35C16A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88a2dcd1996b4a3fbefaf51a98b59def)));
					((ImU8String)(ref val11)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3695EE51 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_26e231ba68f74e8784503e099ddfc2d9));
					if (ImGui.InputFloat(val11, ref customRadiusInput, 0f, 0f, ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3F36B22A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7a7485368e28474eb6d61987c51729a2)), (ImGuiInputTextFlags)0))
					{
						customRadiusInput = Math.Max(0f, customRadiusInput);
					}
					ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x47537CE0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_790eb00575b3425495c924ac13469f4c))), ref customIsHeightNeedToCorrectInput);
					ImGuiOm.TooltipHover(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3695E1DB ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_26e231ba68f74e8784503e099ddfc2d9)));
				}
				finally
				{
					((GroupDisposable)(ref val2)).Dispose();
				}
				ImGui.Spacing();
				DisabledDisposable val12 = ImRaii.Disabled(string.IsNullOrWhiteSpace(customIconNameInput));
				try
				{
					ImU8String val13 = new ImU8String(1, 2);
					((ImU8String)(ref val13)).AppendFormatted<string>(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61543));
					((ImU8String)(ref val13)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x454E0DD5 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_09da05f368544962acb62b42d9155467));
					((ImU8String)(ref val13)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D9CD14B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3badc14f3c904cf4809aa58db382c8ee)));
					if (ImGui.Button(val13, new Vector2(200f * Globals.GlobalUIScale, ImGui.GetFrameHeight())))
					{
						xaYiOfM4rd();
					}
				}
				finally
				{
					((IDisposable)val12)?.Dispose();
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}

		private void xaYiOfM4rd()
		{
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			if (string.IsNullOrWhiteSpace(customIconNameInput))
			{
				OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x535162E4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_9085260e6b1a40438d219e85182b5d08))));
				return;
			}
			uint territoryType = GameState.TerritoryType;
			if (territoryType == 0)
			{
				return;
			}
			ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
			GameIconLookup val = new GameIconLookup(Module.config.CustomIconIDInput, false, true, (ClientLanguage?)null);
			ISharedImmediateTexture val2 = default(ISharedImmediateTexture);
			if (obj.TryGetFromGameIcon(ref val, ref val2))
			{
				MapMarkerDisplay mapMarkerDisplay = new MapMarkerDisplay
				{
					Name = customIconNameInput,
					IconID = Module.config.CustomIconIDInput,
					Position = customPositionInput,
					Radius = customRadiusInput,
					IsPositionExact = !customIsHeightNeedToCorrectInput
				};
				Module.config.CustomMarkers.TryAdd(territoryType, new List<MapMarkerDisplay>());
				if (!Module.config.CustomMarkers[territoryType].Contains(mapMarkerDisplay))
				{
					customIconNameInput = string.Empty;
					Module.config.CustomMarkers[territoryType].Add(mapMarkerDisplay);
					Module.config.Save(Module);
					OmenServiceBase<NotifyHelper>.Instance().NotificationSuccess(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x109C8C04 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_44b9808560124617907f0048883b8948), mapMarkerDisplay.Name));
				}
			}
		}

		private void DIPiK1ywWs()
		{
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			if (Module.config.BlockedIcons.Count == 0)
			{
				return;
			}
			Vector4 vector = KnownColor.Chocolate.ToVector4();
			ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5CE3C543 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3054c6e6ef664be696bb2ad005864818))));
			ImGui.Separator();
			ImGui.Spacing();
			int num = 0;
			ISharedImmediateTexture val2 = default(ISharedImmediateTexture);
			foreach (uint blockedIcon in Module.config.BlockedIcons)
			{
				if (blockedIcon == 0)
				{
					continue;
				}
				ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
				GameIconLookup val = new GameIconLookup(blockedIcon, false, true, (ClientLanguage?)null);
				if (obj.TryGetFromGameIcon(ref val, ref val2))
				{
					if (num != 0 && num % 6 != 0)
					{
						ImGui.SameLine();
					}
					IDalamudTextureWrap wrapOrEmpty = val2.GetWrapOrEmpty();
					if (ImGui.ImageButton(wrapOrEmpty.Handle, wrapOrEmpty.Size))
					{
						Module.jv2veFohbc(blockedIcon);
					}
					ImGuiOm.TooltipHover(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x39876681 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_56ae6e3f2cdf4dd8a743f4d493ce49a5)));
					num++;
				}
			}
		}

		private void BYuitaV9vx(string label, int axisIndex, int delta)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			if (ImGui.Button(ImU8String.op_Implicit(label), default(Vector2)))
			{
				OmenServiceBase<MovementManager>.Instance().TPSmart_InZone(tiRvN8WiZp(LocalPlayerState.Object?.Position ?? default(Vector3), axisIndex, delta));
			}
			ImGuiOm.TooltipHover(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3F364F38 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7a7485368e28474eb6d61987c51729a2)));
			ImU8String val = default(ImU8String);
			((ImU8String)(ref val))._002Ector(22, 3);
			((ImU8String)(ref val)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x43D7C73A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_974d377e1ed74986b81c34c95c2cc846));
			((ImU8String)(ref val)).AppendFormatted<string>(label);
			((ImU8String)(ref val)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6C35CC2E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88a2dcd1996b4a3fbefaf51a98b59def));
			((ImU8String)(ref val)).AppendFormatted<int>(axisIndex);
			((ImU8String)(ref val)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6FF11418 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5859f4ea3042459db2c9b610b3099912));
			((ImU8String)(ref val)).AppendFormatted<int>(delta);
			PopupDisposable val2 = ImRaii.ContextPopupItem(val);
			try
			{
				if (!(!val2))
				{
					ImGui.SetNextItemWidth(100f * Globals.GlobalUIScale);
					ImU8String val3 = new ImU8String(21, 1);
					((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x1C178429 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_accf3844f1c6464481df1849f0aee1bb)));
					((ImU8String)(ref val3)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x469AD4DB ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_bb1eb496bad948e4a2baaa3e1edc7a92));
					if (ImGui.InputInt(val3, ref Module.config.PositionOffsetDelta, 0, 0, default(ImU8String), (ImGuiInputTextFlags)0))
					{
						Module.config.PositionOffsetDelta = (int)MathF.Max(1f, Module.config.PositionOffsetDelta);
					}
					if (ImGui.IsItemDeactivatedAfterEdit())
					{
						Module.config.Save(Module);
					}
				}
			}
			finally
			{
				((PopupDisposable)(ref val2)).Dispose();
			}
		}
	}

	private class SGFTJ8iWfr2Cvup8XEZ : gdUpfLisf7qnNj7E3Em
	{
		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass4_0
		{
			public SGFTJ8iWfr2Cvup8XEZ _003C_003E4__this;

			public uint currentZone;

			public _003C_003Ec__DisplayClass4_0()
			{
				UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
				base._002Ector();
			}

			internal void wtOh9MdnRx(MapMarkerDisplay x)
			{
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				if (ImGui.MenuItem(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x21940496 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cd1b319f326a44d1aaea645e95d8698b))), false, true))
				{
					_003C_003E4__this.Module.config.CustomMarkers[currentZone].Remove(x);
					_003C_003E4__this.Module.config.Save(_003C_003E4__this.Module);
				}
			}
		}

		protected override uint ID => 4u;

		public SGFTJ8iWfr2Cvup8XEZ(FastTeleportPanel module)
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector(module);
		}

		public override void A62ofKGjwQ()
		{
			DMqi8Y1X46((FontAwesomeIcon)61476, LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5CE3C6CB ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3054c6e6ef664be696bb2ad005864818)));
		}

		public override void Draw()
		{
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			_003C_003Ec__DisplayClass4_0 CS_0024_003C_003E8__locals10 = new _003C_003Ec__DisplayClass4_0();
			CS_0024_003C_003E8__locals10._003C_003E4__this = this;
			CS_0024_003C_003E8__locals10.currentZone = GameState.TerritoryType;
			Vector4 vector = KnownColor.Chocolate.ToVector4();
			ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3D567538 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e9e296e241f44828a57c750102ce71c1))));
			ImGui.SameLine();
			if (ImGui.SmallButton(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x109CE21C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_44b9808560124617907f0048883b8948)))) && Module.config.CustomMarkers.TryGetValue(CS_0024_003C_003E8__locals10.currentZone, out List<MapMarkerDisplay> value))
			{
				ModuleBase.ExportToClipboard(value.ToArray());
			}
			ImGui.SameLine();
			if (ImGui.SmallButton(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3F36F294 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7a7485368e28474eb6d61987c51729a2)))))
			{
				List<MapMarkerDisplay> list = ModuleBase.ImportFromClipboard<List<MapMarkerDisplay>>();
				if (list == null)
				{
					return;
				}
				Module.config.CustomMarkers.TryAdd(CS_0024_003C_003E8__locals10.currentZone, new List<MapMarkerDisplay>());
				Module.config.CustomMarkers[CS_0024_003C_003E8__locals10.currentZone].AddRange(list);
				Module.config.Save(Module);
			}
			ImGui.Separator();
			if (!Module.config.CustomMarkers.TryGetValue(CS_0024_003C_003E8__locals10.currentZone, out List<MapMarkerDisplay> value2))
			{
				return;
			}
			vaEivTUucR(value2, isInteract: false, delegate(MapMarkerDisplay x)
			{
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				if (ImGui.MenuItem(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x21940496 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cd1b319f326a44d1aaea645e95d8698b))), false, true))
				{
					CS_0024_003C_003E8__locals10._003C_003E4__this.Module.config.CustomMarkers[CS_0024_003C_003E8__locals10.currentZone].Remove(x);
					CS_0024_003C_003E8__locals10._003C_003E4__this.Module.config.Save(CS_0024_003C_003E8__locals10._003C_003E4__this.Module);
				}
			});
		}
	}

	private class bFo3FxiTwQJuqTOynCZ : gdUpfLisf7qnNj7E3Em
	{
		protected override uint ID => 3u;

		public bFo3FxiTwQJuqTOynCZ(FastTeleportPanel module)
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector(module);
		}

		public override void A62ofKGjwQ()
		{
			DMqi8Y1X46((FontAwesomeIcon)61737, LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x39876405 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_56ae6e3f2cdf4dd8a743f4d493ce49a5)));
		}

		public override void Draw()
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			Vector4 vector = KnownColor.Chocolate.ToVector4();
			ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x317AAB1 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_99910d0c1e594d99b604e0aab1195273))));
			ImGui.Separator();
			vaEivTUucR(Module.markerController.QuestMapMarkers, isInteract: true);
		}
	}

	private class SOxB7Pi1EZHmc2Emtay : gdUpfLisf7qnNj7E3Em
	{
		private string iconNoteInput;

		private int iconWeightInput;

		private uint selectedIconID;

		protected override uint ID => 0u;

		public SOxB7Pi1EZHmc2Emtay(FastTeleportPanel module)
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			iconNoteInput = string.Empty;
			base._002Ector(module);
		}

		public override void A62ofKGjwQ()
		{
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			uint[] array = Module.markerController.MapMarkers.Keys.ToArray();
			ISharedImmediateTexture val2 = default(ISharedImmediateTexture);
			ImU8String val3 = default(ImU8String);
			foreach (uint num in array)
			{
				ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
				GameIconLookup val = new GameIconLookup(num, false, true, (ClientLanguage?)null);
				if (!obj.TryGetFromGameIcon(ref val, ref val2))
				{
					continue;
				}
				((ImU8String)(ref val3))._002Ector(21, 1);
				((ImU8String)(ref val3)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x469AD501 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_bb1eb496bad948e4a2baaa3e1edc7a92));
				((ImU8String)(ref val3)).AppendFormatted<uint>(num);
				IdDisposable val4 = ImRaii.PushId(val3, true);
				try
				{
					IDalamudTextureWrap wrapOrEmpty = val2.GetWrapOrEmpty();
					ImTextureID handle = wrapOrEmpty.Handle;
					float x = ImGui.GetContentRegionAvail().X;
					float y = wrapOrEmpty.Size.Y;
					ImGuiStylePtr style = ImGui.GetStyle();
					if (ImGuiOm.ButtonImage(handle, new Vector2(x, y + ((ImGuiStylePtr)(ref style)).ItemSpacing.Y * 2f), wrapOrEmpty.Size))
					{
						Module.selectedTabID = ID;
						selectedIconID = num;
					}
				}
				finally
				{
					((IDisposable)val4)?.Dispose();
				}
				if (Module.config.IconNotes.TryGetValue(num, out string value) && !string.IsNullOrWhiteSpace(value))
				{
					ImGuiOm.TooltipHover(value);
				}
				kc8iMM0itx(num);
			}
		}

		private void kc8iMM0itx(uint iconID)
		{
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			ImU8String val = default(ImU8String);
			((ImU8String)(ref val))._002Ector(13, 1);
			((ImU8String)(ref val)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3951EC1D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3aeae81f85f84848b273be7b2bfbf497));
			((ImU8String)(ref val)).AppendFormatted<uint>(iconID);
			if (ImGui.BeginPopupContextItem(val, (ImGuiPopupFlags)1))
			{
				if (ImGui.IsWindowAppearing())
				{
					iconNoteInput = Module.config.IconNotes.GetValueOrDefault(iconID, string.Empty);
					iconWeightInput = (int)Module.config.IconWeight.GetValueOrDefault(iconID, 0u);
				}
				ImGui.AlignTextToFramePadding();
				Vector4 vector = KnownColor.Chocolate.ToVector4();
				ImU8String val2 = default(ImU8String);
				((ImU8String)(ref val2))._002Ector(1, 1);
				((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xB1EF7D6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_79d931aaf1c142a2a09f53e716260ad1)));
				((ImU8String)(ref val2)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5CE31611 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3054c6e6ef664be696bb2ad005864818));
				ImGui.TextColored(ref vector, val2);
				ImGui.SameLine();
				ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
				ImU8String val3 = default(ImU8String);
				((ImU8String)(ref val3))._002Ector(17, 1);
				((ImU8String)(ref val3)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x109C8EB0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_44b9808560124617907f0048883b8948));
				((ImU8String)(ref val3)).AppendFormatted<uint>(iconID);
				ImGui.InputText(val3, ref iconNoteInput, 128, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
				if (ImGui.IsItemDeactivatedAfterEdit())
				{
					Module.config.IconNotes[iconID] = iconNoteInput;
					Module.config.Save(Module);
				}
				ImGui.AlignTextToFramePadding();
				vector = KnownColor.Chocolate.ToVector4();
				ImU8String val4 = default(ImU8String);
				((ImU8String)(ref val4))._002Ector(1, 1);
				((ImU8String)(ref val4)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xE0928B1 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_aba6092d7cea44a49d1667540500e363)));
				((ImU8String)(ref val4)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3987B52B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_56ae6e3f2cdf4dd8a743f4d493ce49a5));
				ImGui.TextColored(ref vector, val4);
				ImGui.SameLine();
				ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
				ImU8String val5 = default(ImU8String);
				((ImU8String)(ref val5))._002Ector(19, 1);
				((ImU8String)(ref val5)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4B0F3A0C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_8e002b0644ef421698ef6f63772929af));
				((ImU8String)(ref val5)).AppendFormatted<uint>(iconID);
				if (ImGui.InputInt(val5, ref iconWeightInput, 0, 0, default(ImU8String), (ImGuiInputTextFlags)0))
				{
					iconWeightInput = Math.Max(0, iconWeightInput);
				}
				if (ImGui.IsItemDeactivatedAfterEdit())
				{
					Module.config.IconWeight[iconID] = (uint)iconWeightInput;
					Module.config.Save(Module);
				}
				ImGuiOm.TooltipHover(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x1C1782F3 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_accf3844f1c6464481df1849f0aee1bb)));
				ImGui.Spacing();
				if (ImGuiOm.SelectableTextCentered(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4427FBEE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8dfe57222f64fa68d9d16c8b6fe9d0b)), selected: false, (ImGuiSelectableFlags)0))
				{
					Module.jv2veFohbc(iconID);
				}
				ImGui.EndPopup();
			}
		}

		public override void Draw()
		{
			if (Module.markerController.MapMarkers.TryGetValue(selectedIconID, out List<MapMarkerDisplay> value))
			{
				vaEivTUucR(value, isInteract: false, delegate(MapMarkerDisplay x)
				{
					Module.jv2veFohbc(x.IconID);
				});
			}
		}

		[CompilerGenerated]
		private void etHi4aL1Bq(MapMarkerDisplay x)
		{
			Module.jv2veFohbc(x.IconID);
		}
	}

	private abstract class gdUpfLisf7qnNj7E3Em
	{
		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass10_0
		{
			public gdUpfLisf7qnNj7E3Em _003C_003E4__this;

			public MapMarkerDisplay marker;

			public bool isInteract;

			public _003C_003Ec__DisplayClass10_0()
			{
				UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
				base._002Ector();
			}

			internal void zuohfCkw1w()
			{
				_003C_003E4__this.Module.TaskHelper.Abort();
				OmenServiceBase<MovementManager>.Instance().AbortTasks();
				vnavmeshIPC.StopPathfind();
				_003C_003E4__this.Module.markerController.LmciyQ4tpP(marker, isInteract);
			}
		}

		protected readonly FastTeleportPanel Module;

		protected Vector2 ControlComponentSize;

		protected abstract uint ID { get; }

		protected gdUpfLisf7qnNj7E3Em(FastTeleportPanel module)
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			Module = module;
			ControlComponentSize = new Vector2(100f);
			base._002Ector();
		}

		public abstract void A62ofKGjwQ();

		public abstract void Draw();

		protected void DMqi8Y1X46(FontAwesomeIcon icon, string title)
		{
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			ImU8String val = default(ImU8String);
			((ImU8String)(ref val))._002Ector(1, 2);
			((ImU8String)(ref val)).AppendFormatted<string>(title);
			((ImU8String)(ref val)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x442781EE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8dfe57222f64fa68d9d16c8b6fe9d0b));
			((ImU8String)(ref val)).AppendFormatted<string>(FontAwesomeExtensions.ToIconString(icon));
			float y = ImGui.CalcTextSize(val, false, -1f).Y;
			ImGuiStylePtr style = ImGui.GetStyle();
			float y2 = y + ((ImGuiStylePtr)(ref style)).ItemSpacing.Y * 4f;
			Vector2 buttonSize = new Vector2(ImGui.GetContentRegionAvail().X, y2);
			if (ImGuiOm.ButtonIconWithTextVertical(icon, title, buttonSize, useStaticFont: true))
			{
				Module.selectedTabID = ID;
			}
		}

		protected void vaEivTUucR(IEnumerable<MapMarkerDisplay> markers, bool isInteract = false, Action<MapMarkerDisplay>? popupContextAction = null)
		{
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			Vector2 contentRegionAvail = ImGui.GetContentRegionAvail();
			contentRegionAvail.Y = 0f;
			Vector2 vector = contentRegionAvail;
			TableDisposable val = ImRaii.Table(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5E3B2E96 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2d7d8a5df6744c51b35f486f03b53ded)), 2, (ImGuiTableFlags)0, vector);
			try
			{
				ImGui.TableSetupColumn(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6FF1127A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5859f4ea3042459db2c9b610b3099912)), (ImGuiTableColumnFlags)4, 0f, 0u);
				ImU8String val2 = ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x32D5A31 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_86d75c161ced498ab6a767d2eb82d38e));
				float x = ControlComponentSize.X;
				ImGuiStylePtr style = ImGui.GetStyle();
				ImGui.TableSetupColumn(val2, (ImGuiTableColumnFlags)8, x + 4f * ((ImGuiStylePtr)(ref style)).ItemSpacing.X, 0u);
				foreach (MapMarkerDisplay marker in markers)
				{
					if (!marker.IsUsable)
					{
						continue;
					}
					ImGui.TableNextRow();
					IdDisposable val3 = ImRaii.PushId(marker.GetHashCode(), true);
					try
					{
						ImGui.TableNextColumn();
						dWeil6rF45(marker, out var itemSize);
						if (popupContextAction != null && ImGui.BeginPopupContextItem(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x469AD373 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_bb1eb496bad948e4a2baaa3e1edc7a92)), (ImGuiPopupFlags)1))
						{
							popupContextAction(marker);
							ImGui.EndPopup();
						}
						ImGui.TableNextColumn();
						TXJixg4M4r(marker, itemSize, isInteract);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
			}
			finally
			{
				((TableDisposable)(ref val)).Dispose();
			}
		}

		private unsafe static void dWeil6rF45(MapMarkerDisplay marker, out Vector2 itemSize)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_026e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			itemSize = default(Vector2);
			GroupDisposable val = ImRaii.Group();
			try
			{
				ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
				GameIconLookup val2 = new GameIconLookup(marker.IconID, false, true, (ClientLanguage?)null);
				ISharedImmediateTexture val3 = default(ISharedImmediateTexture);
				if (obj.TryGetFromGameIcon(ref val2, ref val3))
				{
					ImGui.Image(val3.GetWrapOrEmpty().Handle, Globals.ScaledVector2(48f));
					ImGui.SameLine();
				}
				GroupDisposable val4 = ImRaii.Group();
				try
				{
					ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 2f * Globals.GlobalUIScale);
					using (OmenServiceBase<FontManager>.Instance().UIFont.Push())
					{
						ImGui.AlignTextToFramePadding();
						ImGui.TextUnformatted(ImU8String.op_Implicit(marker.Name));
					}
					ImGui.SetCursorPosY(ImGui.GetCursorPosY() - 2f * Globals.GlobalUIScale);
					using (OmenServiceBase<FontManager>.Instance().UIFont60.Push())
					{
						ImGui.AlignTextToFramePadding();
						ImU8String val5 = new ImU8String(2, 2);
						((ImU8String)(ref val5)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7EE770A9 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88432e96079448078bdf00e7b62f8ab6)));
						((ImU8String)(ref val5)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x66B3CDBA ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_762f55d0e7ef4ecd8f3f55c0f8a10fae));
						((ImU8String)(ref val5)).AppendFormatted<float>(marker.Radius, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3AC9BC8C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3932528062874ea4bb7e2e1185394e37));
						ImGui.TextUnformatted(val5);
						ImGui.SameLine();
						ImGui.TextDisabled(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5E3BBC04 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2d7d8a5df6744c51b35f486f03b53ded)));
						ImGui.SameLine();
						ImU8String val6 = new ImU8String(1, 1);
						((ImU8String)(ref val6)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3987F11B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_56ae6e3f2cdf4dd8a743f4d493ce49a5)));
						((ImU8String)(ref val6)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489B93C0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cade14e68bb84906959f3cda5c496848));
						ImGui.TextUnformatted(val6);
						ImGui.SameLine();
						ImU8String val7 = new ImU8String(0, 1);
						((ImU8String)(ref val7)).AppendFormatted<Vector3>(marker.Position, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x32D5BFB ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_86d75c161ced498ab6a767d2eb82d38e));
						ImGui.TextUnformatted(val7);
						if (!string.IsNullOrWhiteSpace(marker.ExtraInfo))
						{
							ImGui.SameLine();
							ImGui.TextDisabled(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3AC92FEE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3932528062874ea4bb7e2e1185394e37)));
							ImGui.SameLine();
							ImGui.TextUnformatted(ImU8String.op_Implicit(marker.ExtraInfo));
						}
					}
				}
				finally
				{
					((GroupDisposable)(ref val4)).Dispose();
				}
			}
			finally
			{
				((GroupDisposable)(ref val)).Dispose();
			}
			if (ImGui.IsItemHovered())
			{
				ImGui.SetMouseCursor((ImGuiMouseCursor)7);
			}
			if (ImGui.IsItemClicked())
			{
				(*AgentMap.Instance()).SetMapFlagAndOpen(marker.Position, marker.Name);
			}
			itemSize = ImGui.GetItemRectSize() + new Vector2(10f * Globals.GlobalUIScale, 0f);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(2, 2);
			defaultInterpolatedStringHandler.AppendFormatted(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x32D5BF3 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_86d75c161ced498ab6a767d2eb82d38e)));
			defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-224923271 ^ -754737977 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cd1b319f326a44d1aaea645e95d8698b));
			defaultInterpolatedStringHandler.AppendFormatted(marker.IconID);
			ImGuiOm.TooltipHover(defaultInterpolatedStringHandler.ToStringAndClear());
		}

		private void TXJixg4M4r(MapMarkerDisplay marker, Vector2 itemSize, bool isInteract)
		{
			_003C_003Ec__DisplayClass10_0 CS_0024_003C_003E8__locals9 = new _003C_003Ec__DisplayClass10_0();
			CS_0024_003C_003E8__locals9._003C_003E4__this = this;
			CS_0024_003C_003E8__locals9.marker = marker;
			CS_0024_003C_003E8__locals9.isInteract = isInteract;
			DisabledDisposable val = ImRaii.Disabled(IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()));
			try
			{
				SdWiphUYpy(CS_0024_003C_003E8__locals9.marker, itemSize, LanguageManager.Get(CS_0024_003C_003E8__locals9.isInteract ? LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x109C8860 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_44b9808560124617907f0048883b8948) : LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5CE3CF87 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3054c6e6ef664be696bb2ad005864818)), delegate
				{
					CS_0024_003C_003E8__locals9._003C_003E4__this.Module.TaskHelper.Abort();
					OmenServiceBase<MovementManager>.Instance().AbortTasks();
					vnavmeshIPC.StopPathfind();
					CS_0024_003C_003E8__locals9._003C_003E4__this.Module.markerController.LmciyQ4tpP(CS_0024_003C_003E8__locals9.marker, CS_0024_003C_003E8__locals9.isInteract);
				});
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}

		private unsafe void SdWiphUYpy(MapMarkerDisplay marker, Vector2 itemSize, string buttonText, Action onButtonClick)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			Vector2 vector = itemSize;
			vector.X = ImGui.CalcTextSize(ImU8String.op_Implicit(buttonText), false, -1f).X * 1.5f;
			Vector2 buttonSize = vector;
			float num = ImGui.GetWindowWidth() - buttonSize.X;
			ImGuiStylePtr style = ImGui.GetStyle();
			ImGui.SetCursorPos(new Vector2(num - 2f * ((ImGuiStylePtr)(ref style)).ItemSpacing.X, ImGui.GetCursorPosY()));
			GroupDisposable val = ImRaii.Group();
			try
			{
				using (OmenServiceBase<FontManager>.Instance().UIFont.Push())
				{
					if (ImGuiOm.ButtonIconWithTextVertical((FontAwesomeIcon)61476, buttonText, buttonSize, useStaticFont: true))
					{
						AgentMap* ptr = AgentMap.Instance();
						if (((AgentMap)ptr).IsAgentActive())
						{
							((AgentMap)ptr).Hide();
						}
						onButtonClick();
					}
					if (!marker.IsPositionExact)
					{
						ImGuiOm.TooltipHover(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D75F2DA ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8b51f8019ce49018b38e06104a97fc1)));
					}
				}
			}
			finally
			{
				((GroupDisposable)(ref val)).Dispose();
			}
			if (Throttler.Shared.Throttle(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3D567EEC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e9e296e241f44828a57c750102ce71c1), 1000u))
			{
				ControlComponentSize = ImGui.GetItemRectSize();
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass39_0
	{
		public string[] split;

		public IPlayerCharacter localPlayer;

		public _003C_003Ec__DisplayClass39_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal bool rAFi3K3H9I(char x)
		{
			return x.ToString().Equals(split[1], StringComparison.OrdinalIgnoreCase);
		}

		internal float FxZiB8uPlw(IGameObject x)
		{
			return Vector3.DistanceSquared(x.Position, localPlayer.Position);
		}

		internal bool yZoim47v71(MapMarkerDisplay x)
		{
			return x.Name.Contains(split[1], StringComparison.OrdinalIgnoreCase);
		}
	}

	private const uint ICON_ID_TRAY = 60453u;

	private const uint ICON_ID_WARP = 60441u;

	private const uint ICON_ID_FILTER_EXCLUDED = 60490u;

	private const uint ICON_ID_FATE_GROUP = 60501u;

	private const uint ICON_ID_CUSTOM_DEFAULT = 60561u;

	private const uint ICON_ID_HOUSING = 60752u;

	private const uint ICON_ID_PARTY_MEMBER = 61572u;

	private const uint ICON_ID_QUEST_GROUP = 71021u;

	private const float VNAVMESH_SNAP_EXTENT_XZ = 5f;

	private const float VNAVMESH_SNAP_EXTENT_Y = 10000f;

	private const ImGuiWindowFlags SMALL_ICON_OVERLAY_FLAGS = (ImGuiWindowFlags)2105579;

	private const string COMMAND_MAIN = "/pdrtp";

	private const string COMMAND_POS = "pos";

	private const string COMMAND_POS_OFFSET = "offset";

	private const string COMMAND_QUEST = "quest";

	private const string COMMAND_MOUSE = "mouse";

	private const string COMMAND_CUSTOM = "custom";

	private const string COMMAND_FLAG = "flag";

	private const string COMMAND_TARGET = "target";

	private const string COMMAND_GROUND = "ground";

	private const string COMMAND_ABORT = "abort";

	private const string COMMAND_AETHERYTE = "aetheryte";

	private static readonly FrozenSet<string> ValidCommandArgs;

	private FrozenDictionary<uint, gdUpfLisf7qnNj7E3Em> tabList;

	private lD0TdJmaaXGs4MmrNMs markerController;

	private omRh8MmZOgrZ163kmaR config;

	private uint selectedTabID;

	[IPCSubscriber("DailyRoutines.Modules.AutoUseEventItem.UseEventItem")]
	private IPCSubscriber<object>? UseEventItem;

	private static readonly char[] AXIS_NAMES;

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected override void Init()
	{
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Expected O, but got Unknown
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Expected O, but got Unknown
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<omRh8MmZOgrZ163kmaR>(this) ?? new omRh8MmZOgrZ163kmaR();
		base.TaskHelper = new TaskHelper
		{
			TimeoutMS = 20000
		};
		markerController = new lD0TdJmaaXGs4MmrNMs(this);
		tabList = new Dictionary<uint, gdUpfLisf7qnNj7E3Em>
		{
			[0u] = new SOxB7Pi1EZHmc2Emtay(this),
			[1u] = new wxRn0Ti9PXswLNJBYk3(this),
			[2u] = new WGq9yRif3JcAHmNt7GV(this),
			[3u] = new bFo3FxiTwQJuqTOynCZ(this),
			[4u] = new SGFTJ8iWfr2Cvup8XEZ(this)
		}.ToFrozenDictionary();
		base.Overlay = new Overlay(this, LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D75A3D0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8b51f8019ce49018b38e06104a97fc1)));
		Overlay? overlay = base.Overlay;
		((Window)overlay).Flags = (ImGuiWindowFlags)(((Window)overlay).Flags & -2);
		Overlay? overlay2 = base.Overlay;
		((Window)overlay2).Flags = (ImGuiWindowFlags)(((Window)overlay2).Flags & -65);
		OmenServiceBase<WindowManager>.Instance().AddWindow(() => new Voj6hUiVRAlraSThWay(this));
		((Window)OmenServiceBase<WindowManager>.Instance().Get<Voj6hUiVRAlraSThWay>()).IsOpen = true;
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += KM9v0ekgaJ;
		OmenServiceBase<CommandManager>.Instance().AddCommand(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x64E5B506 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f53034f3efc64a309b10c7885bf5d9bd), new CommandInfo(new HandlerDelegate(n6Vv5i3sQk))
		{
			HelpMessage = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489F5BCC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b2294f2f9bb24a76b00cf1af75888414))
		});
		OmenServiceBase<FrameworkManager>.Instance().Reg(new OnUpdateDelegate(pUfvandbfk), 1000u);
	}

	protected override void Uninit()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		config.Save(this);
		OmenServiceBase<WindowManager>.Instance().RemoveWindow<Voj6hUiVRAlraSThWay>();
		OmenServiceBase<FrameworkManager>.Instance().Unreg(pUfvandbfk);
		OmenServiceBase<CommandManager>.Instance().RemoveCommand(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2310F1BD ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a96fab9e97944b39a8afe3fb1109ba7d));
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= KM9v0ekgaJ;
		markerController.qJHmU3upU2();
	}

	private void pUfvandbfk(IFramework framework)
	{
		if (((Window)base.Overlay).IsOpen)
		{
			markerController.aNjm0r2Ts7();
		}
	}

	private void KM9v0ekgaJ(uint _)
	{
		base.TaskHelper?.Abort();
		markerController.LgRmuMHcaq();
	}

	private void n6Vv5i3sQk(string _, string args)
	{
		_003C_003Ec__DisplayClass39_0 CS_0024_003C_003E8__locals19 = new _003C_003Ec__DisplayClass39_0();
		CS_0024_003C_003E8__locals19.localPlayer = LocalPlayerState.Object;
		if (CS_0024_003C_003E8__locals19.localPlayer == null || !fPYv70kUbr())
		{
			return;
		}
		if (string.IsNullOrWhiteSpace(args))
		{
			((Window)base.Overlay).Toggle();
			return;
		}
		CS_0024_003C_003E8__locals19.split = args.Trim().ToLowerInvariant().Split(' ');
		string text = CS_0024_003C_003E8__locals19.split[0];
		if (!ValidCommandArgs.Contains(text) || text == null)
		{
			return;
		}
		switch (text.Length)
		{
		case 6:
			switch (text[0])
			{
			case 't':
				if (text == LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x39873219 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_56ae6e3f2cdf4dd8a743f4d493ce49a5))
				{
					IGameObject target = TargetManager.Target;
					if (target != null)
					{
						OmenServiceBase<MovementManager>.Instance().TPSmart_InZone(target.Position);
					}
				}
				break;
			case 'o':
				if (text == LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4CF28CF8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a07aac30eb32484e94cb4d1e62117765) && CS_0024_003C_003E8__locals19.split.Length == 3)
				{
					int num2 = Array.FindIndex(AXIS_NAMES, (char x) => x.ToString().Equals(CS_0024_003C_003E8__locals19.split[1], StringComparison.OrdinalIgnoreCase));
					if (num2 >= 0 && float.TryParse(CS_0024_003C_003E8__locals19.split[2], out var result5))
					{
						OmenServiceBase<MovementManager>.Instance().TPSmart_InZone(tiRvN8WiZp(CS_0024_003C_003E8__locals19.localPlayer.Position, num2, result5));
					}
				}
				break;
			case 'c':
			{
				if (text == LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3695B4E9 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_26e231ba68f74e8784503e099ddfc2d9) && CS_0024_003C_003E8__locals19.split.Length == 2 && config.CustomMarkers.TryGetValue(GameState.TerritoryType, out List<MapMarkerDisplay> value))
				{
					MapMarkerDisplay mapMarkerDisplay = value.FirstOrDefault((MapMarkerDisplay x) => x.Name.Contains(CS_0024_003C_003E8__locals19.split[1], StringComparison.OrdinalIgnoreCase));
					if (mapMarkerDisplay != null)
					{
						OmenServiceBase<MovementManager>.Instance().TPSmart_InZone(mapMarkerDisplay.Position);
					}
				}
				break;
			}
			case 'g':
				if (text == LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xE097E3B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_aba6092d7cea44a49d1667540500e363))
				{
					OmenServiceBase<MovementManager>.Instance().TPGround();
				}
				break;
			}
			break;
		case 5:
			switch (text[0])
			{
			case 'q':
			{
				if (!(text == LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D9C3CFD ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3badc14f3c904cf4809aa58db382c8ee)) || CS_0024_003C_003E8__locals19.split.Length != 2 || IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()) || !int.TryParse(CS_0024_003C_003E8__locals19.split[1], out var result) || result < 0)
				{
					break;
				}
				markerController.aNjm0r2Ts7();
				if (result >= markerController.QuestMapMarkers.Count)
				{
					if (result == 0)
					{
						IGameObject gameObject = (from x in IOmenDalamudService<IObjectTable>.Instance()
							where x.IsMTQ()
							select x).MinBy((IGameObject x) => Vector3.DistanceSquared(x.Position, CS_0024_003C_003E8__locals19.localPlayer.Position));
						if (gameObject != null)
						{
							markerController.iyLmXRx01L(CS_0024_003C_003E8__locals19.localPlayer.Position, 1f, gameObject);
						}
					}
				}
				else
				{
					markerController.LmciyQ4tpP(markerController.QuestMapMarkers[result], isInteract: true);
				}
				break;
			}
			case 'm':
			{
				Vector3 pos = default(Vector3);
				if (text == LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x17780C1B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f7cf93b006904393bb56d82c0a688fa1) && DalamudServiceExtension.Instance<IGameGui>().ScreenToWorld(ImGui.GetMousePos(), ref pos, 100000f))
				{
					OmenServiceBase<MovementManager>.Instance().TPSmart_InZone(pos);
				}
				break;
			}
			case 'a':
				if (text == LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2DBD9CF8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5d8d924f1edf4ec39f023b3c7bb646cc))
				{
					base.TaskHelper?.Abort();
					OmenServiceBase<MovementManager>.Instance().AbortTasks();
					vnavmeshIPC.StopPathfind();
				}
				break;
			}
			break;
		case 4:
			if (text == LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5A7F7EB7 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5154ccd9fed0487f8fa0244531466009))
			{
				c57vRiLb6l();
			}
			break;
		case 3:
		{
			if (text == LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x68ACC1E4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4722689f1a4d41dc9dc800bfe1a89c09) && CS_0024_003C_003E8__locals19.split.Length == 4 && float.TryParse(CS_0024_003C_003E8__locals19.split[1], out var result2) && float.TryParse(CS_0024_003C_003E8__locals19.split[2], out var result3) && float.TryParse(CS_0024_003C_003E8__locals19.split[3], out var result4))
			{
				OmenServiceBase<MovementManager>.Instance().TPSmart_InZone(new Vector3(result2, result3, result4));
			}
			break;
		}
		case 9:
			if (text == LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x66B34900 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_762f55d0e7ef4ecd8f3f55c0f8a10fae))
			{
				uint num = Kn7vDRQa4Q();
				if (num != 0)
				{
					OmenServiceBase<GamePacketManager>.Instance().SendPackt(new EventStartPackt(CS_0024_003C_003E8__locals19.localPlayer.GameObjectID, num));
				}
			}
			break;
		case 7:
		case 8:
			break;
		}
	}

	private static void c57vRiLb6l()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if (GameState.IsFlagMarkerSet && GameState.FlagMarker.TerritoryId == GameState.TerritoryType)
		{
			Vector3 vector = GameState.FlagMarkerPosition.ToPlayerHeight();
			OmenServiceBase<MovementManager>.Instance().TPSmart_InZone(lD0TdJmaaXGs4MmrNMs.JPMmIw63WD(vector, out var worldPos) ? worldPos : vector);
		}
	}

	private unsafe static uint Kn7vDRQa4Q()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Invalid comparison between Unknown and I4
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Invalid comparison between Unknown and I4
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		ValueEnumerator<uint, Pointer<EventHandler>> enumerator = ((EventHandlerModule)(&((EventFramework)EventFramework.Instance()).EventHandlerModule)).EventHandlerMap.Values.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Pointer<EventHandler> current = enumerator.Current;
				if (current.Value != null)
				{
					EventId eventId = ((EventHandlerInfo)(&((EventHandler)current.Value).Info)).EventId;
					EventHandlerContent contentId = eventId.ContentId;
					if (((int)contentId == 5 || (int)contentId == 30) ? true : false)
					{
						return eventId.Id;
					}
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
		return 0u;
	}

	protected override void ConfigUI()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0673: Unknown result type (might be due to invalid IL or missing references)
		//IL_067c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0681: Unknown result type (might be due to invalid IL or missing references)
		//IL_074a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0753: Unknown result type (might be due to invalid IL or missing references)
		//IL_0758: Unknown result type (might be due to invalid IL or missing references)
		//IL_0815: Unknown result type (might be due to invalid IL or missing references)
		using (ImRaiiExtension.Heading1(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6C353D32 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88a2dcd1996b4a3fbefaf51a98b59def))))
		{
			ImU8String val = new ImU8String(2, 3);
			((ImU8String)(ref val)).AppendFormatted<string>(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489B152A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cade14e68bb84906959f3cda5c496848));
			((ImU8String)(ref val)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x68AC67CE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4722689f1a4d41dc9dc800bfe1a89c09));
			((ImU8String)(ref val)).AppendFormatted<string>(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xB1E10F2 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_79d931aaf1c142a2a09f53e716260ad1));
			((ImU8String)(ref val)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5E3BD9F4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2d7d8a5df6744c51b35f486f03b53ded));
			((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x469A83A7 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_bb1eb496bad948e4a2baaa3e1edc7a92)));
			ImGui.TextUnformatted(val);
			ImU8String val2 = new ImU8String(2, 3);
			((ImU8String)(ref val2)).AppendFormatted<string>(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4B0F6AF2 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_8e002b0644ef421698ef6f63772929af));
			((ImU8String)(ref val2)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x66B3EF1E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_762f55d0e7ef4ecd8f3f55c0f8a10fae));
			((ImU8String)(ref val2)).AppendFormatted<string>(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x66B349F0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_762f55d0e7ef4ecd8f3f55c0f8a10fae));
			((ImU8String)(ref val2)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x66B3EF1E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_762f55d0e7ef4ecd8f3f55c0f8a10fae));
			((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x18CC0A0E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_33babcee9ecc460a9e530039fd18d361)));
			ImGui.TextUnformatted(val2);
			ImU8String val3 = new ImU8String(2, 3);
			((ImU8String)(ref val3)).AppendFormatted<string>(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3C140F83 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_d71d1e134fde413fa0bee3b2419181b6));
			((ImU8String)(ref val3)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x258F9240 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_dadeb2ab61424d31be7cc1966e675825));
			((ImU8String)(ref val3)).AppendFormatted<string>(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x47532954 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_790eb00575b3425495c924ac13469f4c));
			((ImU8String)(ref val3)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xF205D77 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_fea4ad1b13de4c4bbc22f571ceda7372));
			((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2DBD9F24 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5d8d924f1edf4ec39f023b3c7bb646cc)));
			ImGui.TextUnformatted(val3);
			ImU8String val4 = new ImU8String(2, 3);
			((ImU8String)(ref val4)).AppendFormatted<string>(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x66B34858 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_762f55d0e7ef4ecd8f3f55c0f8a10fae));
			((ImU8String)(ref val4)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x45E4397B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1904f95d2a394c929e1a6d4f72943ad7));
			((ImU8String)(ref val4)).AppendFormatted<string>(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x32D0B6B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_86d75c161ced498ab6a767d2eb82d38e));
			((ImU8String)(ref val4)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x39511C1D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3aeae81f85f84848b273be7b2bfbf497));
			((ImU8String)(ref val4)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3951B90B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3aeae81f85f84848b273be7b2bfbf497)));
			ImGui.TextUnformatted(val4);
			ImU8String val5 = new ImU8String(2, 3);
			((ImU8String)(ref val5)).AppendFormatted<string>(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-224923271 ^ -1217171644 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1904f95d2a394c929e1a6d4f72943ad7));
			((ImU8String)(ref val5)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x45E4397B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1904f95d2a394c929e1a6d4f72943ad7));
			((ImU8String)(ref val5)).AppendFormatted<string>(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x1C17D3EF ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_accf3844f1c6464481df1849f0aee1bb));
			((ImU8String)(ref val5)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x12D65F34 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7fcf5a71a7ed445197f132979519c560));
			((ImU8String)(ref val5)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3F361A3A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7a7485368e28474eb6d61987c51729a2)));
			ImGui.TextUnformatted(val5);
			ImU8String val6 = new ImU8String(2, 3);
			((ImU8String)(ref val6)).AppendFormatted<string>(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-1675397288 ^ -1757583674 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_79d931aaf1c142a2a09f53e716260ad1));
			((ImU8String)(ref val6)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x32DAD69 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_86d75c161ced498ab6a767d2eb82d38e));
			((ImU8String)(ref val6)).AppendFormatted<string>(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xF20FBA9 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_fea4ad1b13de4c4bbc22f571ceda7372));
			((ImU8String)(ref val6)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7FD88C1F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1eab987c1db64b928220e27bd03141d3));
			((ImU8String)(ref val6)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x685442CB ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_23522a2a14314da28eb44187a09cd0b5)));
			ImGui.TextUnformatted(val6);
			ImU8String val7 = new ImU8String(2, 3);
			((ImU8String)(ref val7)).AppendFormatted<string>(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3951BB5B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3aeae81f85f84848b273be7b2bfbf497));
			((ImU8String)(ref val7)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2CE52DBB ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c9dc773883f149b7befba3bdbeac55f7));
			((ImU8String)(ref val7)).AppendFormatted<string>(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x39873219 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_56ae6e3f2cdf4dd8a743f4d493ce49a5));
			((ImU8String)(ref val7)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--1976305562 ^ 0x269D961C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_9085260e6b1a40438d219e85182b5d08));
			((ImU8String)(ref val7)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-864210836 ^ -261538659 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_d71d1e134fde413fa0bee3b2419181b6)));
			ImGui.TextUnformatted(val7);
			ImU8String val8 = new ImU8String(2, 3);
			((ImU8String)(ref val8)).AppendFormatted<string>(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x1C17D257 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_accf3844f1c6464481df1849f0aee1bb));
			((ImU8String)(ref val8)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3C14A8C5 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_d71d1e134fde413fa0bee3b2419181b6));
			((ImU8String)(ref val8)).AppendFormatted<string>(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6AAE8902 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_839f542231154d29b0562b687afa328a));
			((ImU8String)(ref val8)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3F36BE9A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7a7485368e28474eb6d61987c51729a2));
			((ImU8String)(ref val8)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3F361ADA ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7a7485368e28474eb6d61987c51729a2)));
			ImGui.TextUnformatted(val8);
			ImU8String val9 = new ImU8String(2, 3);
			((ImU8String)(ref val9)).AppendFormatted<string>(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4B0F6AF2 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_8e002b0644ef421698ef6f63772929af));
			((ImU8String)(ref val9)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x36951217 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_26e231ba68f74e8784503e099ddfc2d9));
			((ImU8String)(ref val9)).AppendFormatted<string>(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x53513798 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_9085260e6b1a40438d219e85182b5d08));
			((ImU8String)(ref val9)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x9B4A3CE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2f1c0cb46b4440d499a69ed8afe22360));
			((ImU8String)(ref val9)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D9C2145 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3badc14f3c904cf4809aa58db382c8ee)));
			ImGui.TextUnformatted(val9);
			ImU8String val10 = new ImU8String(2, 3);
			((ImU8String)(ref val10)).AppendFormatted<string>(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x423E03C6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_6368ba44fa4749c09da09b5ebf853b71));
			((ImU8String)(ref val10)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xE09D8F5 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_aba6092d7cea44a49d1667540500e363));
			((ImU8String)(ref val10)).AppendFormatted<string>(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3695B407 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_26e231ba68f74e8784503e099ddfc2d9));
			((ImU8String)(ref val10)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x44270C30 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8dfe57222f64fa68d9d16c8b6fe9d0b));
			((ImU8String)(ref val10)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x177811C7 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f7cf93b006904393bb56d82c0a688fa1)));
			ImGui.TextUnformatted(val10);
		}
	}

	protected override void OverlayUI()
	{
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		Voj6hUiVRAlraSThWay voj6hUiVRAlraSThWay = OmenServiceBase<WindowManager>.Instance().Get<Voj6hUiVRAlraSThWay>();
		if (voj6hUiVRAlraSThWay != null && ((Window)voj6hUiVRAlraSThWay).IsOpen)
		{
			((Window)voj6hUiVRAlraSThWay).IsOpen = false;
			return;
		}
		if (DalamudLeftServiceExtension.Instance().CutsceneActive || !fPYv70kUbr())
		{
			((Window)base.Overlay).IsOpen = false;
			return;
		}
		using (OmenServiceBase<FontManager>.Instance().UIFont80.Push())
		{
			float x = ImGui.CalcTextSize(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6C358674 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88a2dcd1996b4a3fbefaf51a98b59def)), false, -1f).X;
			Vector2 vector = new Vector2(x, ImGui.GetContentRegionAvail().Y);
			ChildDisposable val = ImRaii.Child(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--367535148 ^ 0x1E4C7836 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2edd08450a8b42e6997855e5217d6d55)), vector, true, (ImGuiWindowFlags)8);
			try
			{
				if (ChildDisposable.op_Implicit(val))
				{
					foreach (KeyValuePair<uint, gdUpfLisf7qnNj7E3Em> item in tabList.Skip(1))
					{
						item.Value.A62ofKGjwQ();
					}
					ImGui.Separator();
					tabList[0u].A62ofKGjwQ();
				}
			}
			finally
			{
				((ChildDisposable)(ref val)).Dispose();
			}
			ImGui.SameLine();
			ChildDisposable val2 = ImRaii.Child(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x45E48237 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1904f95d2a394c929e1a6d4f72943ad7)), ImGui.GetContentRegionAvail(), true, (ImGuiWindowFlags)8);
			try
			{
				if (ChildDisposable.op_Implicit(val2) && tabList.TryGetValue(selectedTabID, out gdUpfLisf7qnNj7E3Em value))
				{
					value.Draw();
				}
			}
			finally
			{
				((ChildDisposable)(ref val2)).Dispose();
			}
		}
	}

	protected override void OverlayOnOpen()
	{
		Voj6hUiVRAlraSThWay? voj6hUiVRAlraSThWay = OmenServiceBase<WindowManager>.Instance().Get<Voj6hUiVRAlraSThWay>();
		((Window)voj6hUiVRAlraSThWay).IsOpen = false;
		voj6hUiVRAlraSThWay.FlashStartTime = 0.0;
	}

	protected override void OverlayOnClose()
	{
		Voj6hUiVRAlraSThWay? voj6hUiVRAlraSThWay = OmenServiceBase<WindowManager>.Instance().Get<Voj6hUiVRAlraSThWay>();
		voj6hUiVRAlraSThWay.FlashStartTime = ImGui.GetTime();
		((Window)voj6hUiVRAlraSThWay).IsOpen = true;
	}

	private void jv2veFohbc(uint iconID)
	{
		if (!config.BlockedIcons.Add(iconID))
		{
			config.BlockedIcons.Remove(iconID);
		}
		config.Save(this);
	}

	private static Vector3 tiRvN8WiZp(Vector3 position, int axisIndex, float delta)
	{
		switch (axisIndex)
		{
		case 0:
		{
			Vector3 result = position;
			result.X = position.X + delta;
			return result;
		}
		case 1:
		{
			Vector3 result = position;
			result.Y = position.Y + delta;
			return result;
		}
		case 2:
		{
			Vector3 result = position;
			result.Z = position.Z + delta;
			return result;
		}
		default:
			return position;
		}
	}

	private static bool fPYv70kUbr()
	{
		if ((GameState.IsCN || GameState.IsTC) && !AuthState.IsPremium)
		{
			return !Sheets.SpeedDetectionZones.ContainsKey(GameState.TerritoryType);
		}
		return true;
	}

	private static bool Nanvd65KxE(ObjectKind kind, float distance)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Expected I4, but got Unknown
		float num = 7f;
		if (ManagerBase<ModuleManager>.Instance().IsModuleEnabled(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5E3BAD92 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2d7d8a5df6744c51b35f486f03b53ded)) == true)
		{
			num = 40f;
		}
		return (kind - 3) switch
		{
			4 => (double)distance < 4.7999999, 
			0 => distance < num, 
			2 => (double)distance < 11.0, 
			3 => (double)distance < 3.0, 
			_ => (double)distance < 6.0, 
		};
	}

	public FastTeleportPanel()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x39873391 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_56ae6e3f2cdf4dd8a743f4d493ce49a5)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4CF290BC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a07aac30eb32484e94cb4d1e62117765)),
			Category = ModuleCategory.Interface,
			ModulesPair = new string[1] { LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x51E5FF4C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a90a92322bff42e180982e106200d4a3) }
		};
		Permission = new ModulePermission
		{
			NeedAuth = true,
			AllDefaultEnabled = true
		};
		selectedTabID = 2u;
		base._002Ector();
	}

	static FastTeleportPanel()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		InlineArray10<string> buffer = default(InlineArray10<string>);
		buffer[0] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x66B349F0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_762f55d0e7ef4ecd8f3f55c0f8a10fae);
		buffer[1] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3C140EF1 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_d71d1e134fde413fa0bee3b2419181b6);
		buffer[2] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x27D117ED ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_39d5d8025f5249e6b489bc374eefbb3c);
		buffer[3] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x12D6F9CA ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7fcf5a71a7ed445197f132979519c560);
		buffer[4] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x53577440 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_0676edd1b45a4ff096830fd6229bc1fb);
		buffer[5] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x454EAB0B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_09da05f368544962acb62b42d9155467);
		buffer[6] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6AAE8902 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_839f542231154d29b0562b687afa328a);
		buffer[7] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3695B407 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_26e231ba68f74e8784503e099ddfc2d9);
		buffer[8] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x477EB905 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_ba27244341a5414496a87eae29cf3dec);
		buffer[9] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6FF143B4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5859f4ea3042459db2c9b610b3099912);
		ValidCommandArgs = FrozenSet.Create<string>(buffer);
		AXIS_NAMES = new char[3] { 'X', 'Y', 'Z' };
	}

	[CompilerGenerated]
	private Voj6hUiVRAlraSThWay LYCvImb9p3()
	{
		return new Voj6hUiVRAlraSThWay(this);
	}
}
