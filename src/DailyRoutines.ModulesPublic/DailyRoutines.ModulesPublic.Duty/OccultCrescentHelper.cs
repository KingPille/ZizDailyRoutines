using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using DailyRoutines.Common.Interface.Windows;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Fates;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Game.Command;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Hooking;
using Dalamud.Interface;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using Dalamud.Utility.Numerics;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.InstanceContent;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Common.Math;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Interop;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using KamiToolKit.UiOverlay;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools;
using OmenTools.Dalamud;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.ImGuiOm.Widgets.MapRenderer;
using OmenTools.Info.Game;
using OmenTools.Info.Game.Data;
using OmenTools.Info.Game.Enums;
using OmenTools.Info.Game.Packets.Upstream;
using OmenTools.Interop.Game.ExecuteCommand.Implementations;
using OmenTools.Interop.Game.Helpers;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.Interop.Game.Models.Native;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.OmenService.ZoneIndicator;
using OmenTools.Threading;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;

namespace DailyRoutines.ModulesPublic.Duty;

public class OccultCrescentHelper : ModuleBase
{
	private class AetheryteManager : BaseIslandModule
	{
		private const string COMMAND_TP = "ptp";

		private const float USE_AETHERYTE_DISTANCE_SQ = 10000f;

		private TaskHelper? moveTaskHelper;

		public AetheryteManager(OccultCrescentHelper mainModule)
			: base(mainModule)
		{
		}

		public override void Init()
		{
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Expected O, but got Unknown
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Expected O, but got Unknown
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Expected O, but got Unknown
			if (moveTaskHelper == null)
			{
				moveTaskHelper = new TaskHelper
				{
					TimeoutMS = 30000
				};
			}
			DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += OnZoneChanged;
			DalamudServiceExtension.Instance<IClientState>().Logout += new LogoutDelegate(OnLogout);
			OmenServiceBase<CommandManager>.Instance().AddSubCommand("ptp", new CommandInfo(new HandlerDelegate(OnCommandTP))
			{
				HelpMessage = LanguageManager.Get("OccultCrescentHelper-Command-PTP-Help")
			});
		}

		public override void Uninit()
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Expected O, but got Unknown
			OmenServiceBase<CommandManager>.Instance().RemoveSubCommand("ptp");
			DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChanged;
			DalamudServiceExtension.Instance<IClientState>().Logout -= new LogoutDelegate(OnLogout);
			StopPathfinding();
			moveTaskHelper?.Dispose();
			moveTaskHelper = null;
		}

		public override void DrawConfig()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Invalid comparison between Unknown and I4
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			Vector4 vector;
			IndentDisposable val2;
			if ((int)GameState.TerritoryIntendedUse == 61)
			{
				vector = KnownColor.Chocolate.ToVector4();
				ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("Teleport")));
				string text = string.Empty;
				List<CrescentAetheryte> list = ((GameState.TerritoryType == 1252) ? CrescentAetheryte.SouthHornAetherytes : CrescentAetheryte.NorthHornAetherytes);
				foreach (CrescentAetheryte item in list)
				{
					if (item.Name.Length > text.Length)
					{
						text = item.Name;
					}
				}
				Vector2 vector2 = new Vector2(ImGui.CalcTextSize(ImU8String.op_Implicit(text), false, -1f).X * 2f, ImGui.GetTextLineHeightWithSpacing());
				DisabledDisposable val = ImRaii.Disabled((int)GameState.TerritoryIntendedUse != 61);
				try
				{
					val2 = ImRaii.PushIndent(1, true);
					try
					{
						foreach (CrescentAetheryte item2 in list)
						{
							if (ImGui.Button(ImU8String.op_Implicit(item2.Name), vector2))
							{
								TryUseAetheryte(item2);
							}
						}
					}
					finally
					{
						((IDisposable)val2)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)val)?.Dispose();
				}
				ImGui.NewLine();
			}
			vector = KnownColor.Chocolate.ToVector4();
			ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("Command")));
			val2 = ImRaii.PushIndent(1, true);
			try
			{
				ImU8String val3 = new ImU8String(6, 2);
				((ImU8String)(ref val3)).AppendLiteral("/pdr ");
				((ImU8String)(ref val3)).AppendFormatted<string>("ptp");
				((ImU8String)(ref val3)).AppendLiteral(" ");
				((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get("OccultCrescentHelper-Command-PTP-Help"));
				ImGui.TextWrapped(val3);
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}

		private void OnLogout(int type, int code)
		{
			StopPathfinding();
		}

		private void OnZoneChanged(uint u)
		{
			StopPathfinding();
		}

		public void StopPathfinding()
		{
			moveTaskHelper?.Abort();
			vnavmeshIPC.StopPathfind();
		}

		private void OnCommandTP(string command, string args)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Invalid comparison between Unknown and I4
			if ((int)GameState.TerritoryIntendedUse != 61)
			{
				return;
			}
			args = args.Trim().ToLowerInvariant();
			if (string.IsNullOrWhiteSpace(args))
			{
				return;
			}
			List<CrescentAetheryte> list = ((GameState.TerritoryType == 1252) ? CrescentAetheryte.SouthHornAetherytes : CrescentAetheryte.NorthHornAetherytes);
			CrescentAetheryte crescentAetheryte = null;
			if (byte.TryParse(args, out var result))
			{
				try
				{
					crescentAetheryte = list[result];
				}
				catch
				{
				}
			}
			else
			{
				crescentAetheryte = (from x in list
					where x.Name.Contains(args, StringComparison.OrdinalIgnoreCase)
					orderby x.Name
					select x).FirstOrDefault();
			}
			if (!(crescentAetheryte == null))
			{
				TryUseAetheryte(crescentAetheryte);
			}
		}

		public unsafe bool TryUseAetheryte(CrescentAetheryte aetheryte)
		{
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			if (!(aetheryte == null))
			{
				TaskHelper taskHelper = moveTaskHelper;
				if (taskHelper != null)
				{
					IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
					if (localPlayer != null)
					{
						StopPathfinding();
						OmenServiceBase<ChatManager>.Instance().SendMessage("/automove off");
						if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4])
						{
							OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.Dismount);
						}
						if (aetheryte.TeleportTo())
						{
							return true;
						}
						if ((*EventFramework.Instance()).TryGetNearestEvent((EventHandlerInfo x) => (int)x.EventId.ContentId == 11, (GameObject x) => ((GameObject)(ref x)).NameString.Equals(LuminaWrapper.GetEObjName(2006473u), StringComparison.OrdinalIgnoreCase) || ((GameObject)(ref x)).NameString.Equals(LuminaWrapper.GetEObjName(2014664u), StringComparison.OrdinalIgnoreCase), localPlayer.Position, out var eventID, out var eventObjectID))
						{
							IGameObject targetObj = IOmenDalamudService<IObjectTable>.Instance().SearchByID(GameObjectId.op_Implicit(eventObjectID));
							if (targetObj != null)
							{
								float distanceSQ = LocalPlayerState.DistanceTo3DSquared(targetObj.Position);
								if (distanceSQ <= 16f)
								{
									taskHelper.Enqueue(delegate
									{
										//IL_0010: Unknown result type (might be due to invalid IL or missing references)
										//IL_001b: Unknown result type (might be due to invalid IL or missing references)
										if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4])
										{
											return false;
										}
										new EventStartPackt(GameObjectId.op_Implicit(eventObjectID), EventId.op_Implicit(eventID)).Send();
										new EventCompletePackt(721820u, 16777216u, aetheryte.DataID).Send();
										return true;
									});
									return true;
								}
								if (DalamudServiceExtension.Instance().IsPluginEnabled("vnavmesh") && distanceSQ <= 10000f)
								{
									taskHelper.Enqueue(delegate
									{
										if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4])
										{
											return true;
										}
										if (distanceSQ <= 900f)
										{
											taskHelper.Enqueue(() => !((ActionManager)ActionManager.Instance()).IsActionOffCooldown((ActionType)1, 3u) || LocalPlayerState.HasStatus(50u, out var _) || ((ActionManager)ActionManager.Instance()).UseAction((ActionType)1, 3u, 3758096384uL, 0u, (UseActionMode)0, 0u, (bool*)null), null, 0, null, null, null, null, 1);
											return true;
										}
										return OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)5, 9u, 3758096384uL, 0u, (UseActionMode)0);
									});
									taskHelper.Enqueue(delegate
									{
										if (!Throttler.Shared.Throttle("OccultCrescentHelper-AetheryteManager-MoveTo"))
										{
											return false;
										}
										if (vnavmeshIPC.GetIsPathfindRunning() || vnavmeshIPC.GetIsPathfindInProgress())
										{
											return true;
										}
										vnavmeshIPC.PathfindAndMoveToClosely(targetObj.Position, fly: false, 0.1f);
										return false;
									});
									taskHelper.Enqueue(delegate
									{
										if (vnavmeshIPC.GetIsPathfindRunning() || vnavmeshIPC.GetIsPathfindInProgress())
										{
											return false;
										}
										if (LocalPlayerState.DistanceTo3DSquared(targetObj.Position) > 16f)
										{
											if (Throttler.Shared.Throttle("OccultCrescentHelper-AetheryteManager-MoveTo-Retry"))
											{
												vnavmeshIPC.PathfindAndMoveToClosely(targetObj.Position, fly: false, 0.1f);
											}
											return false;
										}
										vnavmeshIPC.StopPathfind();
										if (!DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4])
										{
											return true;
										}
										if (!Throttler.Shared.Throttle("OccultCrescentHelper-AetheryteManager-Dismount"))
										{
											return false;
										}
										OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.Dismount);
										return false;
									});
									taskHelper.Enqueue(delegate
									{
										//IL_0010: Unknown result type (might be due to invalid IL or missing references)
										//IL_001b: Unknown result type (might be due to invalid IL or missing references)
										if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4])
										{
											return false;
										}
										new EventStartPackt(GameObjectId.op_Implicit(eventObjectID), EventId.op_Implicit(eventID)).Send();
										new EventCompletePackt(721820u, 16777216u, aetheryte.DataID).Send();
										return true;
									});
									taskHelper.Enqueue(() => LocalPlayerState.DistanceTo3DSquared(aetheryte.Position) <= 900f);
									return true;
								}
							}
						}
						return false;
					}
				}
			}
			return false;
		}
	}

	private class Config : ModuleConfig
	{
		public bool AddonIsDragRealAction = true;

		public string AutoEnableDisablePlugins = string.Empty;

		public string JoinDutyCommands = string.Empty;

		public string LeaveDutyCommands = string.Empty;

		public Vector3 DefaultPositionEnterZoneNorthHorn = new Vector3(882.2f, 258.5f, 882f);

		public Vector3 DefaultPositionEnterZoneSouthHorn = new Vector3(834f, 73f, -694f);

		public float DistanceToAutoOpenTreasure = 5f;

		public bool IsEnabledAutoOpenTreasure;

		public bool IsEnabledBardOffensiveAria = true;

		public bool IsEnabledBerserkerRageAutoFace = true;

		public bool IsEnabledBerserkerRageReplace = true;

		public bool IsEnabledHighlightCarrot = true;

		public bool IsEnabledHighlightSurveyPoint = true;

		public bool IsEnabledHighlightTreasure = true;

		public bool IsEnabledHighlightCE = true;

		public bool IsEnabledHighlightFATE = true;

		public bool IsEnabledHideDutyCommand;

		public bool IsEnabledModifyInfoHUD = true;

		public bool IsEnabledKnowledgeCrystalFastUse = true;

		public bool IsEnabledMonkKickNoMove = true;

		public bool InterruptPathfindingOnMovementInput = true;

		public bool IsEnabledDismountCE;

		public bool IsEnabledDismountFATE;

		public bool IsEnabledNotifyCENotification = true;

		public bool IsEnabledNotifyCETTS = true;

		public bool IsEnabledNotifyCESystemSound = true;

		public Dictionary<CrescentEventType, bool> IsEnabledNotifyEventsCategoried = new Dictionary<CrescentEventType, bool>();
	}

	private abstract class BaseIslandModule(OccultCrescentHelper mainModule)
	{
		protected readonly OccultCrescentHelper MainModule = mainModule;

		public virtual void Init()
		{
		}

		public virtual void OnUpdate()
		{
		}

		public virtual void DrawConfig()
		{
		}

		public virtual void Uninit()
		{
		}
	}

	private class EventManager(OccultCrescentHelper mainModule) : BaseIslandModule(mainModule)
	{
		private sealed class PathfindingSession(IslandEventData data, Vector3 destination)
		{
			public IslandEventData Data { get; } = data;

			public Vector3 Destination { get; set; } = destination;

			public CrescentAetheryte? Aetheryte { get; set; }

			public PathfindingTravelStage TravelStage { get; set; }

			public Action? StopAetherytePathfinding { get; set; }

			public CancellationTokenSource CancellationTokenSource { get; } = new CancellationTokenSource();

			public CancellationTokenSource? AetheryteRouteCancellationTokenSource { get; set; }

			public Task<CrescentAetheryte?>? AetheryteRoutePlanningTask { get; set; }

			public Task<List<Vector3>>? PathfindingTask { get; set; }

			public Vector3 PathfindingTaskDestination { get; set; }

			public List<Vector3> Path { get; set; } = new List<Vector3>();

			public uint FateMonsterID { get; set; }

			public float FateMonsterRadius { get; set; }

			public bool IsAtPathfindingDestination { get; set; }

			public bool IsMovementInterrupted { get; set; }
		}

		private enum PathfindingTravelStage : byte
		{
			RoutePlanning,
			Pathfinding,
			DemiReturn,
			Aetheryte
		}

		public class IslandEventData(uint dataID) : IEquatable<IslandEventData>
		{
			public CrescentEvent Event { get; } = new CrescentEvent(dataID);

			public int LinkPayloadID { get; private set; } = -1;

			public DalamudLinkPayload LinkPayload { get; private set; }

			public float FateTimeRemaining { get; set; }

			public bool Equals(IslandEventData? other)
			{
				if ((object)other == null)
				{
					return false;
				}
				if ((object)this == other)
				{
					return true;
				}
				return Event == other.Event;
			}

			public static IslandEventData? Parse(IFate fate)
			{
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Invalid comparison between Unknown and I4
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_001c: Invalid comparison between Unknown and I4
				//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
				bool flag = fate.MapIconId == 0;
				if (!flag)
				{
					FateState state = fate.State;
					bool flag2 = (((int)state == 5 || state - 7 <= 1) ? true : false);
					flag = flag2;
				}
				if (flag || fate.Position == default(Vector3))
				{
					return null;
				}
				string text = $"{fate.Name} ({fate.Progress}%)";
				if (string.IsNullOrEmpty(text))
				{
					return null;
				}
				IslandEventData islandEventData = new IslandEventData(fate.FateId);
				islandEventData.Event.UpdateTempDataFATE(text, fate.Progress, fate.State);
				islandEventData.Event.UpdatePositionAndRadius(fate.Position, fate.Radius);
				islandEventData.FateTimeRemaining = fate.TimeRemaining;
				return islandEventData;
			}

			public unsafe static IslandEventData? Parse(DynamicEvent ce)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Invalid comparison between Unknown and I4
				//IL_0051: Unknown result type (might be due to invalid IL or missing references)
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_0095: Unknown result type (might be due to invalid IL or missing references)
				//IL_0096: Unknown result type (might be due to invalid IL or missing references)
				//IL_009b: Unknown result type (might be due to invalid IL or missing references)
				//IL_009d: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b2: Expected I4, but got Unknown
				//IL_0208: Unknown result type (might be due to invalid IL or missing references)
				//IL_020e: Unknown result type (might be due to invalid IL or missing references)
				//IL_020f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0214: Unknown result type (might be due to invalid IL or missing references)
				//IL_0215: Unknown result type (might be due to invalid IL or missing references)
				//IL_021b: Invalid comparison between Unknown and I4
				//IL_0131: Unknown result type (might be due to invalid IL or missing references)
				//IL_0132: Unknown result type (might be due to invalid IL or missing references)
				//IL_018a: Unknown result type (might be due to invalid IL or missing references)
				//IL_018b: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
				//IL_022b: Unknown result type (might be due to invalid IL or missing references)
				//IL_021d: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
				//IL_023e: Unknown result type (might be due to invalid IL or missing references)
				//IL_023f: Unknown result type (might be due to invalid IL or missing references)
				if (!LuminaGetter.TryGetRow<DynamicEvent>((uint)ce.DynamicEventId, out DynamicEvent item))
				{
					return null;
				}
				if ((int)ce.State == 0)
				{
					return null;
				}
				if (!CrescentEvent.ForkTowers.Contains(((DynamicEvent)(ref item)).RowId) && ce.MapMarker.Position == default(Vector3))
				{
					return null;
				}
				long num = ce.StartTimestamp - GameState.ServerTimeUnix;
				if (num < 0)
				{
					num = 0L;
				}
				string text = ((object)(*(Utf8String*)(&ce.Name))/*cast due to constrained. prefix*/).ToString();
				if (!CrescentEvent.ForkTowers.Contains(((DynamicEvent)(ref item)).RowId))
				{
					DynamicEventState state = ce.State;
					text = (state - 1) switch
					{
						2 => $"{ce.Name} ({LanguageManager.Get("OccultCrescentHelper-CEManager-CEName-InBattle", ce.Participants, ce.Progress)})", 
						0 => $"{ce.Name} ({LanguageManager.Get("OccultCrescentHelper-CEManager-CEName-Register", num)})", 
						1 => $"{ce.Name} ({LanguageManager.Get("OccultCrescentHelper-CEManager-CEName-WarmUp")})", 
						_ => $"{ce.Name}", 
					};
				}
				if (string.IsNullOrEmpty(text))
				{
					return null;
				}
				IslandEventData islandEventData = new IslandEventData(((DynamicEvent)(ref item)).RowId);
				islandEventData.Event.UpdateTempDataCE(text, ce.Progress, ce.State, ((int)ce.State == 1) ? ce.StartTimestamp : (ce.StartTimestamp - 1200), num);
				islandEventData.Event.UpdatePositionAndRadius(ce.MapMarker.Position, 0f);
				return islandEventData;
			}

			public void Update(IFate fate)
			{
				//IL_0051: Unknown result type (might be due to invalid IL or missing references)
				string nameDisplay = $"{fate.Name} ({fate.Progress}%)";
				Event.UpdateTempDataFATE(nameDisplay, fate.Progress, fate.State);
				FateTimeRemaining = fate.TimeRemaining;
			}

			public unsafe void Update(DynamicEvent ce)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_019f: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b2: Invalid comparison between Unknown and I4
				//IL_0047: Unknown result type (might be due to invalid IL or missing references)
				//IL_0048: Unknown result type (might be due to invalid IL or missing references)
				//IL_004d: Unknown result type (might be due to invalid IL or missing references)
				//IL_004f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0052: Unknown result type (might be due to invalid IL or missing references)
				//IL_0064: Expected I4, but got Unknown
				//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
				//IL_013a: Unknown result type (might be due to invalid IL or missing references)
				//IL_013b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0074: Unknown result type (might be due to invalid IL or missing references)
				//IL_0075: Unknown result type (might be due to invalid IL or missing references)
				//IL_009a: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
				//IL_0183: Unknown result type (might be due to invalid IL or missing references)
				//IL_0184: Unknown result type (might be due to invalid IL or missing references)
				if (LuminaGetter.TryGetRow<DynamicEvent>((uint)ce.DynamicEventId, out DynamicEvent item))
				{
					long num = ce.StartTimestamp - GameState.ServerTimeUnix;
					if (num < 0)
					{
						num = 0L;
					}
					string nameDisplay = ((object)(*(Utf8String*)(&ce.Name))/*cast due to constrained. prefix*/).ToString();
					if (((DynamicEvent)(ref item)).RowId != 48)
					{
						DynamicEventState state = ce.State;
						nameDisplay = (state - 1) switch
						{
							2 => $"{ce.Name} ({LanguageManager.Get("OccultCrescentHelper-CEManager-CEName-InBattle", ce.Participants, ce.Progress)})", 
							0 => $"{ce.Name} ({LanguageManager.Get("OccultCrescentHelper-CEManager-CEName-Register", num)})", 
							1 => $"{ce.Name} ({LanguageManager.Get("OccultCrescentHelper-CEManager-CEName-WarmUp")})", 
							_ => $"{ce.Name}", 
						};
					}
					Event.UpdateTempDataCE(nameDisplay, ce.Progress, ce.State, ((int)ce.State == 1) ? ce.StartTimestamp : (ce.StartTimestamp - 1200), num);
				}
			}

			public string GetNotificationTitle()
			{
				return Event.Type switch
				{
					CrescentEventType.FATE => LanguageManager.Get("OccultCrescentHelper-CEManager-Notification-FATE"), 
					CrescentEventType.MagicPot => LanguageManager.Get("OccultCrescentHelper-CEManager-Notification-MagicPot"), 
					CrescentEventType.CE => LanguageManager.Get("OccultCrescentHelper-CEManager-Notification-CE"), 
					CrescentEventType.ForkTower => LanguageManager.Get("OccultCrescentHelper-CEManager-Notification-ForkTower"), 
					_ => LanguageManager.Get("OccultCrescentHelper-CEManager-Notification-FATE"), 
				};
			}

			public DalamudLinkPayload GetOrAddLinkPayload(EventManager manager)
			{
				if (LinkPayloadID != -1)
				{
					return LinkPayload;
				}
				LinkPayload = OmenServiceBase<LinkPayloadManager>.Instance().Reg(manager.OnClickPathfind, out var id);
				LinkPayloadID = (int)id;
				return LinkPayload;
			}

			public override bool Equals(object? obj)
			{
				return Equals(obj as IslandEventData);
			}

			public override int GetHashCode()
			{
				return HashCode.Combine(Event);
			}

			public static bool operator ==(IslandEventData? left, IslandEventData? right)
			{
				return object.Equals(left, right);
			}

			public static bool operator !=(IslandEventData? left, IslandEventData? right)
			{
				return !object.Equals(left, right);
			}
		}

		private HashSet<IslandEventData> allIslandEvents = new HashSet<IslandEventData>();

		private readonly HashSet<string> knownCENames = new HashSet<string>();

		private ZoneIndicatorHandle? fateHandle;

		private ZoneIndicatorHandle? ceHandle;

		private TaskHelper? ceTaskHelper;

		private PathfindingSession? pathfindingSession;

		private const string COMMAND_FATE = "pfate";

		private const string COMMAND_CE = "pce";

		private const int AETHERYTE_ROUTE_PLANNING_TIMEOUT_MS = 30000;

		private const float FATE_MONSTER_TARGET_DISTANCE_SQUARED = 2500f;

		private const float MOUNT_MINIMUM_DISTANCE = 40f;

		private const float PATHFINDING_COMPLETION_DISTANCE = 5f;

		private const float PATH_POINT_RADIUS = 4f;

		private const float PATH_LINE_THICKNESS = 2f;

		private static readonly uint PathLineColor = KnownColor.Chocolate.ToUInt();

		private static readonly uint PathPointColor = KnownColor.Chocolate.ToUInt();

		public override void Init()
		{
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Expected O, but got Unknown
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Expected O, but got Unknown
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Expected O, but got Unknown
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Expected O, but got Unknown
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Expected O, but got Unknown
			if (ceTaskHelper == null)
			{
				ceTaskHelper = new TaskHelper
				{
					TimeoutMS = 180000
				};
			}
			DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += OnZoneChanged;
			OnZoneChanged(0u);
			OmenServiceBase<ExecuteCommandManager>.Instance().RegPost(OnPostReceivedCommand);
			OmenServiceBase<LogMessageManager>.Instance().RegPost(OnPostReceivedMessage);
			OmenServiceBase<GameState>.Instance().Logout += OnLogout;
			OmenServiceBase<FrameworkManager>.Instance().Reg(new OnUpdateDelegate(OnPathfindingUpdate), 100u);
			OmenServiceBase<WindowManager>.Instance().PostDraw += OnPathfindingDraw;
			bool flag = false;
			CrescentEventType[] values = Enum.GetValues<CrescentEventType>();
			foreach (CrescentEventType key in values)
			{
				if (MainModule.config.IsEnabledNotifyEventsCategoried.TryAdd(key, value: true))
				{
					flag = true;
				}
			}
			if (flag)
			{
				MainModule.config.Save(MainModule);
			}
			OmenServiceBase<CommandManager>.Instance().AddSubCommand("pfate", new CommandInfo(new HandlerDelegate(OnCommandFate))
			{
				HelpMessage = (LanguageManager.Get("OccultCrescentHelper-Command-PFate-Help") ?? "")
			});
			OmenServiceBase<CommandManager>.Instance().AddSubCommand("pce", new CommandInfo(new HandlerDelegate(OnCommandCE))
			{
				HelpMessage = (LanguageManager.Get("OccultCrescentHelper-Command-PCE-Help") ?? "")
			});
		}

		public override void Uninit()
		{
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Expected O, but got Unknown
			OmenServiceBase<CommandManager>.Instance().RemoveSubCommand("pfate");
			OmenServiceBase<CommandManager>.Instance().RemoveSubCommand("pce");
			OmenServiceBase<GameState>.Instance().Logout -= OnLogout;
			OmenServiceBase<ExecuteCommandManager>.Instance().Unreg(OnPostReceivedCommand);
			OmenServiceBase<LogMessageManager>.Instance().Unreg(OnPostReceivedMessage);
			DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChanged;
			OmenServiceBase<FrameworkManager>.Instance().Unreg(OnPathfindingUpdate);
			OmenServiceBase<WindowManager>.Instance().PostDraw -= OnPathfindingDraw;
			fateHandle?.Unreg();
			fateHandle = null;
			ceHandle?.Unreg();
			ceHandle = null;
			allIslandEvents.Clear();
			knownCENames.Clear();
			StopPathfinding();
			ceTaskHelper?.Dispose();
			ceTaskHelper = null;
		}

		public override void DrawConfig()
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0425: Unknown result type (might be due to invalid IL or missing references)
			//IL_042a: Unknown result type (might be due to invalid IL or missing references)
			//IL_042f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0431: Unknown result type (might be due to invalid IL or missing references)
			//IL_0451: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_049d: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_057d: Unknown result type (might be due to invalid IL or missing references)
			//IL_058c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0623: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			//IL_0306: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Unknown result type (might be due to invalid IL or missing references)
			//IL_0369: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0382: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
			TabBarDisposable val = ImRaii.TabBar(ImU8String.op_Implicit("TabBar"));
			try
			{
				if (!val)
				{
					return;
				}
				TabItemDisposable val2 = ImRaii.TabItem(ImU8String.op_Implicit(LanguageManager.Get("General")));
				try
				{
					if (TabItemDisposable.op_Implicit(val2))
					{
						Vector4 vector = KnownColor.Chocolate.ToVector4();
						ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("Pathfind")));
						IndentDisposable val3 = ImRaii.PushIndent(1, true);
						try
						{
							ISharedImmediateTexture val5 = default(ISharedImmediateTexture);
							foreach (IslandEventData allIslandEvent in allIslandEvents)
							{
								ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
								GameIconLookup val4 = new GameIconLookup(allIslandEvent.Event.IconID, false, true, (ClientLanguage?)null);
								if (!obj.TryGetFromGameIcon(ref val4, ref val5))
								{
									continue;
								}
								DisabledDisposable val6 = ImRaii.Disabled(!CanStartPathfinding(allIslandEvent));
								try
								{
									if (ImGuiOm.SelectableImageWithText(val5.GetWrapOrEmpty().Handle, new Vector2(ImGui.GetTextLineHeightWithSpacing()), allIslandEvent.Event.NameDisplay ?? "", selected: false, (ImGuiSelectableFlags)0))
									{
										StartPathfinding(allIslandEvent);
									}
								}
								finally
								{
									((IDisposable)val6)?.Dispose();
								}
							}
							ImGui.Spacing();
							ImU8String val7 = new ImU8String(16, 1);
							((ImU8String)(ref val7)).AppendLiteral("    ");
							((ImU8String)(ref val7)).AppendFormatted<string>(LanguageManager.Get("Stop"));
							((ImU8String)(ref val7)).AppendLiteral("    ##StopCE");
							if (ImGui.SmallButton(val7))
							{
								StopPathfinding();
							}
							ImGui.Spacing();
							if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("OccultCrescentHelper-CEManager-InterruptOnMovementInput")), ref MainModule.config.InterruptPathfindingOnMovementInput))
							{
								MainModule.config.Save(MainModule);
							}
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
						ImGui.NewLine();
						ImGui.TextColored(KnownColor.Chocolate.ToUInt(), ImU8String.op_Implicit(LanguageManager.Get("OccultCrescentHelper-CEManager-AutoDismount")));
						val3 = ImRaii.PushIndent(1, true);
						try
						{
							ImU8String val8 = new ImU8String(18, 1);
							((ImU8String)(ref val8)).AppendFormatted<string>(CrescentEvent.GetEventTypeName(CrescentEventType.FATE));
							((ImU8String)(ref val8)).AppendLiteral("##AutoDismountFATE");
							if (ImGui.Checkbox(val8, ref MainModule.config.IsEnabledDismountFATE))
							{
								MainModule.config.Save(MainModule);
							}
							ImU8String val9 = new ImU8String(16, 1);
							((ImU8String)(ref val9)).AppendFormatted<string>(CrescentEvent.GetEventTypeName(CrescentEventType.CE));
							((ImU8String)(ref val9)).AppendLiteral("##AutoDismountCE");
							if (ImGui.Checkbox(val9, ref MainModule.config.IsEnabledDismountCE))
							{
								MainModule.config.Save(MainModule);
							}
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
						ImGui.NewLine();
						ImGui.TextColored(KnownColor.Chocolate.ToUInt(), ImU8String.op_Implicit(LanguageManager.Get("OccultCrescentHelper-Highlight")));
						val3 = ImRaii.PushIndent(1, true);
						try
						{
							ImU8String val10 = new ImU8String(0, 1);
							((ImU8String)(ref val10)).AppendFormatted<string>(CrescentEvent.GetEventTypeName(CrescentEventType.CE));
							if (ImGui.Checkbox(val10, ref MainModule.config.IsEnabledHighlightCE))
							{
								MainModule.config.Save(MainModule);
							}
							ImU8String val11 = new ImU8String(0, 1);
							((ImU8String)(ref val11)).AppendFormatted<string>(CrescentEvent.GetEventTypeName(CrescentEventType.FATE));
							if (ImGui.Checkbox(val11, ref MainModule.config.IsEnabledHighlightFATE))
							{
								MainModule.config.Save(MainModule);
							}
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
						ImGui.NewLine();
						vector = KnownColor.Chocolate.ToVector4();
						ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("Command")));
						val3 = ImRaii.PushIndent(1, true);
						try
						{
							ImU8String val12 = new ImU8String(6, 2);
							((ImU8String)(ref val12)).AppendLiteral("/pdr ");
							((ImU8String)(ref val12)).AppendFormatted<string>("pfate");
							((ImU8String)(ref val12)).AppendLiteral(" ");
							((ImU8String)(ref val12)).AppendFormatted<string>(LanguageManager.Get("OccultCrescentHelper-Command-PFate-Help"));
							ImGui.TextWrapped(val12);
							ImU8String val13 = new ImU8String(6, 2);
							((ImU8String)(ref val13)).AppendLiteral("/pdr ");
							((ImU8String)(ref val13)).AppendFormatted<string>("pce");
							((ImU8String)(ref val13)).AppendLiteral(" ");
							((ImU8String)(ref val13)).AppendFormatted<string>(LanguageManager.Get("OccultCrescentHelper-Command-PCE-Help"));
							ImGui.TextWrapped(val13);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
				}
				finally
				{
					((TabItemDisposable)(ref val2)).Dispose();
				}
				TabItemDisposable val14 = ImRaii.TabItem(ImU8String.op_Implicit(LanguageManager.Get("Notification")));
				try
				{
					if (!TabItemDisposable.op_Implicit(val14))
					{
						return;
					}
					Vector4 vector = KnownColor.Chocolate.ToVector4();
					ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("OccultCrescentHelper-CEManager-NotifyEventAppears")));
					IndentDisposable val3 = ImRaii.PushIndent(1, true);
					try
					{
						foreach (KeyValuePair<CrescentEventType, bool> item in MainModule.config.IsEnabledNotifyEventsCategoried)
						{
							item.Deconstruct(out var key, out var value);
							CrescentEventType crescentEventType = key;
							bool flag = value;
							ImU8String val15 = new ImU8String(0, 1);
							((ImU8String)(ref val15)).AppendFormatted<CrescentEventType>(crescentEventType);
							IdDisposable val16 = ImRaii.PushId(val15, true);
							try
							{
								GroupDisposable val17 = ImRaii.Group();
								try
								{
									bool value2 = flag;
									ImU8String val18 = new ImU8String(2, 2);
									((ImU8String)(ref val18)).AppendFormatted<string>(CrescentEvent.GetEventTypeName(crescentEventType));
									((ImU8String)(ref val18)).AppendLiteral("##");
									((ImU8String)(ref val18)).AppendFormatted<CrescentEventType>(crescentEventType);
									if (ImGui.Checkbox(val18, ref value2))
									{
										MainModule.config.IsEnabledNotifyEventsCategoried[crescentEventType] = value2;
										MainModule.config.Save(MainModule);
									}
								}
								finally
								{
									((GroupDisposable)(ref val17)).Dispose();
								}
							}
							finally
							{
								((IDisposable)val16)?.Dispose();
							}
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					ImGui.NewLine();
					vector = KnownColor.Chocolate.ToVector4();
					ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("OccultCrescentHelper-CEManager-NotifyCEStarts")));
					IdDisposable val19 = ImRaii.PushId(ImU8String.op_Implicit("NotifyCEStarts"), true);
					try
					{
						val3 = ImRaii.PushIndent(1, true);
						try
						{
							if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("SendNotification")), ref MainModule.config.IsEnabledNotifyCENotification))
							{
								MainModule.config.Save(MainModule);
							}
							if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("SendTTS")), ref MainModule.config.IsEnabledNotifyCETTS))
							{
								MainModule.config.Save(MainModule);
							}
							if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("SendSystemSound")), ref MainModule.config.IsEnabledNotifyCESystemSound))
							{
								MainModule.config.Save(MainModule);
							}
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					finally
					{
						((IDisposable)val19)?.Dispose();
					}
				}
				finally
				{
					((TabItemDisposable)(ref val14)).Dispose();
				}
			}
			finally
			{
				((TabBarDisposable)(ref val)).Dispose();
			}
		}

		private void OnLogout()
		{
			StopPathfinding();
		}

		private void OnZoneChanged(uint u)
		{
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Invalid comparison between Unknown and I4
			fateHandle?.Unreg();
			fateHandle = null;
			ceHandle?.Unreg();
			ceHandle = null;
			allIslandEvents.Clear();
			knownCENames.Clear();
			StopPathfinding();
			if ((int)GameState.TerritoryIntendedUse != 61)
			{
				return;
			}
			fateHandle = OmenServiceBase<ZoneIndicatorRenderer>.Instance().RegTemporary(() => (!MainModule.config.IsEnabledHighlightFATE || DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)26]) ? new List<IslandEventData>() : allIslandEvents.Where(delegate(IslandEventData x)
			{
				CrescentEventType type = x.Event.Type;
				return (type == CrescentEventType.FATE || type == CrescentEventType.MagicPot) ? true : false;
			}).ToList(), (IslandEventData data) => data.Event.Position, new ZoneIndicatorOptions<IslandEventData>
			{
				TextGetter = delegate(IslandEventData data)
				{
					//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_002f: Unknown result type (might be due to invalid IL or missing references)
					ZoneIndicatorText.TextImage image = null;
					if (data.Event.IconID != 0)
					{
						ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
						GameIconLookup val = new GameIconLookup(data.Event.IconID, false, true, (ClientLanguage?)null);
						ISharedImmediateTexture texture = default(ISharedImmediateTexture);
						if (obj.TryGetFromGameIcon(ref val, ref texture))
						{
							image = new ZoneIndicatorText.TextImage
							{
								Texture = texture,
								SizeGetter = () => new Vector2(ImGui.GetTextLineHeightWithSpacing())
							};
						}
					}
					return new ZoneIndicatorText
					{
						Text = ReadOnlySeString.op_Implicit($"{data.Event.NameDisplay} ({TimeSpan.FromSeconds(data.FateTimeRemaining):mm\\:ss})"),
						TextScale = 1.2f,
						Image = image,
						TextColor = KnownColor.LawnGreen.ToVector4()
					};
				},
				RenderRadius = 300u
			});
			ceHandle = OmenServiceBase<ZoneIndicatorRenderer>.Instance().RegTemporary(() => (!MainModule.config.IsEnabledHighlightCE || DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)26]) ? new List<IslandEventData>() : allIslandEvents.Where((IslandEventData x) => x.Event.Type == CrescentEventType.CE).ToList(), (IslandEventData data) => data.Event.Position, new ZoneIndicatorOptions<IslandEventData>
			{
				TextGetter = delegate(IslandEventData data)
				{
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_000c: Invalid comparison between Unknown and I4
					//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
					//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
					//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
					string text = (((int)data.Event.CEState != 1) ? data.Event.NameDisplay : $"{data.Event.Name} ({TimeSpan.FromSeconds(data.Event.CELeftTimeSecond):mm\\:ss})");
					string text2 = text;
					ZoneIndicatorText.TextImage image = null;
					if (data.Event.IconID != 0)
					{
						ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
						GameIconLookup val = new GameIconLookup(data.Event.IconID, false, true, (ClientLanguage?)null);
						ISharedImmediateTexture texture = default(ISharedImmediateTexture);
						if (obj.TryGetFromGameIcon(ref val, ref texture))
						{
							image = new ZoneIndicatorText.TextImage
							{
								Texture = texture,
								SizeGetter = () => new Vector2(20f)
							};
						}
					}
					return new ZoneIndicatorText
					{
						Text = new ReadOnlySeString(text2),
						TextScale = 1.2f,
						TextColor = KnownColor.LawnGreen.ToVector4(),
						Image = image
					};
				},
				RenderRadius = 300u
			});
		}

		public unsafe override void OnUpdate()
		{
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			PublicContentOccultCrescent* instance = PublicContentOccultCrescent.GetInstance();
			if (instance == null)
			{
				return;
			}
			HashSet<string> hashSet = new HashSet<string>();
			List<IslandEventData> list = new List<IslandEventData>();
			foreach (IFate item in (IEnumerable<IFate>)DalamudServiceExtension.Instance<IFateTable>())
			{
				IslandEventData islandEventData = IslandEventData.Parse(item);
				if ((object)islandEventData != null)
				{
					list.Add(islandEventData);
					hashSet.Add(islandEventData.Event.Name);
					if (allIslandEvents.TryGetValue(islandEventData, out IslandEventData actualValue))
					{
						actualValue.Update(item);
					}
					else
					{
						allIslandEvents.Add(islandEventData);
					}
					if (knownCENames.Add(islandEventData.Event.Name))
					{
						NotifyNewCE(islandEventData);
					}
				}
			}
			foreach (DynamicEvent item2 in (from x in ((DynamicEventContainer)(&((PublicContentOccultCrescent)instance).DynamicEventContainer)).Events.ToArray()
				select (x)).ToList())
			{
				IslandEventData islandEventData2 = IslandEventData.Parse(item2);
				if ((object)islandEventData2 != null)
				{
					list.Add(islandEventData2);
					hashSet.Add(islandEventData2.Event.Name);
					if (allIslandEvents.TryGetValue(islandEventData2, out IslandEventData actualValue2))
					{
						actualValue2.Update(item2);
					}
					else
					{
						allIslandEvents.Add(islandEventData2);
					}
					if (knownCENames.Add(islandEventData2.Event.Name))
					{
						NotifyNewCE(islandEventData2);
					}
				}
			}
			knownCENames.IntersectWith(hashSet);
			allIslandEvents.IntersectWith(list);
		}

		private void OnPostReceivedCommand(ExecuteCommandFlag command, uint param1, uint param2, uint param3, uint param4)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Invalid comparison between Unknown and I4
			if (command == ExecuteCommandFlag.LoadFate && (int)GameState.TerritoryIntendedUse == 61)
			{
				OnUpdate();
			}
		}

		private void OnPostReceivedMessage(uint logMessageID, LogMessageQueueItem values)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Invalid comparison between Unknown and I4
			if (logMessageID == 11002 && (int)GameState.TerritoryIntendedUse == 61)
			{
				string message = LanguageManager.Get("OccultCrescentHelper-CEManager-Notification-CEStart");
				if (MainModule.config.IsEnabledNotifyCENotification)
				{
					OmenServiceBase<NotifyHelper>.Instance().NotificationInfo(message);
				}
				if (MainModule.config.IsEnabledNotifyCETTS)
				{
					NotifyHelper.Speak(message);
				}
				if (MainModule.config.IsEnabledNotifyCESystemSound)
				{
					NotifyHelper.SystemInformation();
				}
			}
		}

		private void OnClickPathfind(uint id, SeString message)
		{
			IslandEventData islandEventData = allIslandEvents.FirstOrDefault((IslandEventData x) => x.LinkPayloadID == id);
			if ((object)islandEventData != null)
			{
				StartPathfinding(islandEventData);
			}
		}

		private void OnCommandFate(string command, string args)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			if ((int)GameState.TerritoryIntendedUse != 61)
			{
				return;
			}
			args = args.Trim().ToLowerInvariant();
			if (args == "abort")
			{
				StopPathfinding();
				return;
			}
			IslandEventData islandEventData = (from x in allIslandEvents.Where(delegate(IslandEventData x)
				{
					CrescentEventType type = x.Event.Type;
					return (type == CrescentEventType.FATE || type == CrescentEventType.MagicPot) ? true : false;
				}).Where(CanStartPathfinding)
				orderby x.Event.Progress
				select x).FirstOrDefault();
			if (!(islandEventData == null))
			{
				StartPathfinding(islandEventData);
			}
		}

		private void OnCommandCE(string command, string args)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			if ((int)GameState.TerritoryIntendedUse != 61)
			{
				return;
			}
			args = args.Trim().ToLowerInvariant();
			if (args == "abort")
			{
				StopPathfinding();
				return;
			}
			IslandEventData islandEventData = allIslandEvents.Where((IslandEventData x) => x.Event.Type == CrescentEventType.CE).FirstOrDefault(CanStartPathfinding);
			if (!(islandEventData == null))
			{
				StartPathfinding(islandEventData);
			}
		}

		private static bool CanStartPathfinding(IslandEventData data)
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Invalid comparison between Unknown and I4
			bool flag = vnavmeshIPC.IsPluginEnabled();
			if (flag)
			{
				bool flag2;
				switch (data.Event.Type)
				{
				case CrescentEventType.FATE:
				case CrescentEventType.MagicPot:
					flag2 = true;
					break;
				case CrescentEventType.CE:
				{
					CrescentEvent crescentEvent = data.Event;
					flag2 = (object)crescentEvent != null && (int)crescentEvent.CEState == 1;
					break;
				}
				default:
					flag2 = false;
					break;
				}
				flag = flag2;
			}
			return flag;
		}

		private unsafe void StartPathfinding(IslandEventData data)
		{
			if (!CanStartPathfinding(data) || IOmenDalamudService<IObjectTable>.Instance().LocalPlayer == null)
			{
				return;
			}
			TaskHelper taskHelper = ceTaskHelper;
			if (taskHelper == null)
			{
				return;
			}
			StopPathfinding();
			MainModule.aetheryteModule.StopPathfinding();
			PathfindingSession session = new PathfindingSession(data, data.Event.Position);
			pathfindingSession = session;
			session.TravelStage = PathfindingTravelStage.RoutePlanning;
			EnqueueAetheryteRoutePlanning(taskHelper, session, PathfindingTravelStage.RoutePlanning);
			taskHelper.Enqueue(delegate
			{
				if (pathfindingSession != session || session.TravelStage != PathfindingTravelStage.RoutePlanning)
				{
					return true;
				}
				CrescentAetheryte aetheryte = session.Aetheryte;
				if ((object)aetheryte == null)
				{
					session.TravelStage = PathfindingTravelStage.Pathfinding;
					return true;
				}
				if (MainModule.aetheryteModule.TryUseAetheryte(aetheryte))
				{
					session.TravelStage = PathfindingTravelStage.Aetheryte;
					session.StopAetherytePathfinding = MainModule.aetheryteModule.StopPathfinding;
					return true;
				}
				session.Aetheryte = null;
				session.TravelStage = PathfindingTravelStage.DemiReturn;
				return true;
			});
			Vector3 demiReturnStartPosition = default(Vector3);
			taskHelper.Enqueue(delegate
			{
				if (pathfindingSession != session || session.TravelStage != PathfindingTravelStage.DemiReturn)
				{
					return true;
				}
				if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4])
				{
					OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.Dismount);
					return false;
				}
				if (((ActionManager)ActionManager.Instance()).GetActionStatus((ActionType)1, 41343u, 3758096384uL, true, true, (uint*)null) != 0)
				{
					return false;
				}
				IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
				if (localPlayer == null)
				{
					return false;
				}
				demiReturnStartPosition = localPlayer.Position;
				return OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)1, 41343u, 3758096384uL, 0u, (UseActionMode)0);
			}, null, 5000, TaskAbortBehaviour.AbortCurrent, null, delegate
			{
				session.TravelStage = PathfindingTravelStage.Pathfinding;
			});
			taskHelper.Enqueue(delegate
			{
				if (pathfindingSession != session || session.TravelStage != PathfindingTravelStage.DemiReturn)
				{
					return true;
				}
				IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
				return localPlayer != null && localPlayer.Position != demiReturnStartPosition;
			}, null, 30000, TaskAbortBehaviour.AbortCurrent, null, delegate
			{
				session.TravelStage = PathfindingTravelStage.Pathfinding;
			});
			taskHelper.Enqueue(() => pathfindingSession != session || session.TravelStage != PathfindingTravelStage.DemiReturn || UIModuleExtension.IsScreenReady(), null, 30000, TaskAbortBehaviour.AbortCurrent, null, delegate
			{
				session.TravelStage = PathfindingTravelStage.Pathfinding;
			});
			EnqueueAetheryteRoutePlanning(taskHelper, session, PathfindingTravelStage.DemiReturn);
			taskHelper.Enqueue(delegate
			{
				if (pathfindingSession != session || session.TravelStage != PathfindingTravelStage.RoutePlanning)
				{
					return true;
				}
				CrescentAetheryte aetheryte = session.Aetheryte;
				if ((object)aetheryte == null)
				{
					session.TravelStage = PathfindingTravelStage.Pathfinding;
					return true;
				}
				if (!Throttler.Shared.Throttle("OccultCrescentHelper-CEManager-DemiReturn-Aetheryte") || !MainModule.aetheryteModule.TryUseAetheryte(aetheryte))
				{
					return false;
				}
				session.TravelStage = PathfindingTravelStage.Aetheryte;
				session.StopAetherytePathfinding = MainModule.aetheryteModule.StopPathfinding;
				return true;
			}, null, 10000, TaskAbortBehaviour.AbortCurrent, null, delegate
			{
				session.TravelStage = PathfindingTravelStage.Pathfinding;
			});
			EnqueueAetheryteArrival(taskHelper, session);
			taskHelper.Enqueue(delegate
			{
				if (pathfindingSession != session)
				{
					return true;
				}
				IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
				if (localPlayer == null)
				{
					StopPathfinding(session);
					return true;
				}
				StartNavigationPath(session, localPlayer.Position);
				return true;
			});
		}

		private void EnqueueAetheryteRoutePlanning(TaskHelper taskHelper, PathfindingSession session, PathfindingTravelStage expectedStage)
		{
			taskHelper.Enqueue(delegate
			{
				if (pathfindingSession != session || session.TravelStage != expectedStage)
				{
					return true;
				}
				IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
				if (localPlayer == null)
				{
					session.TravelStage = PathfindingTravelStage.Pathfinding;
					return true;
				}
				session.Aetheryte = null;
				session.TravelStage = PathfindingTravelStage.RoutePlanning;
				CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(session.CancellationTokenSource.Token);
				cancellationTokenSource.CancelAfter(30000);
				session.AetheryteRouteCancellationTokenSource = cancellationTokenSource;
				session.AetheryteRoutePlanningTask = FindBestAetheryteRoute(localPlayer.Position, session.Destination, cancellationTokenSource.Token);
				return true;
			});
			taskHelper.Enqueue(delegate
			{
				if (pathfindingSession != session || session.TravelStage != PathfindingTravelStage.RoutePlanning)
				{
					return true;
				}
				Task<CrescentAetheryte> aetheryteRoutePlanningTask = session.AetheryteRoutePlanningTask;
				if (aetheryteRoutePlanningTask == null)
				{
					session.TravelStage = PathfindingTravelStage.Pathfinding;
					return true;
				}
				if (!aetheryteRoutePlanningTask.IsCompleted)
				{
					return false;
				}
				session.AetheryteRoutePlanningTask = null;
				session.AetheryteRouteCancellationTokenSource?.Dispose();
				session.AetheryteRouteCancellationTokenSource = null;
				if (!aetheryteRoutePlanningTask.IsCompletedSuccessfully)
				{
					AggregateException exception = aetheryteRoutePlanningTask.Exception;
					if (exception != null)
					{
						DLog.Error("新月岛魔路路线规划失败", exception.GetBaseException());
					}
					session.TravelStage = PathfindingTravelStage.Pathfinding;
					return true;
				}
				session.Aetheryte = aetheryteRoutePlanningTask.Result;
				return true;
			}, null, 30000, TaskAbortBehaviour.AbortCurrent, null, delegate
			{
				session.AetheryteRouteCancellationTokenSource?.Cancel();
				session.AetheryteRouteCancellationTokenSource?.Dispose();
				session.AetheryteRouteCancellationTokenSource = null;
				session.AetheryteRoutePlanningTask = null;
				if (pathfindingSession == session)
				{
					session.TravelStage = PathfindingTravelStage.Pathfinding;
				}
			});
		}

		private static async Task<CrescentAetheryte?> FindBestAetheryteRoute(Vector3 origin, Vector3 destination, CancellationToken cancellationToken)
		{
			List<CrescentAetheryte> list = ((GameState.TerritoryType == 1252) ? CrescentAetheryte.SouthHornAetherytes : CrescentAetheryte.NorthHornAetherytes).OrderBy((CrescentAetheryte x) => Vector3.DistanceSquared(x.Position, destination)).ToList();
			CrescentAetheryte bestAetheryte = list[0];
			if (GameState.TerritoryType == 1346 && (list[0] == CrescentAetheryte.UnhallowedHamlet || list[1] == CrescentAetheryte.UnhallowedHamlet))
			{
				CrescentAetheryte secondAetheryte = list[1];
				Task<List<Vector3>> task = vnavmeshIPC.PathfindCancelable(bestAetheryte.Position, destination, fly: false, cancellationToken);
				Task<List<Vector3>> secondPathfindingTask = vnavmeshIPC.PathfindCancelable(secondAetheryte.Position, destination, fly: false, cancellationToken);
				if (task != null && secondPathfindingTask != null)
				{
					List<Vector3> firstPath = await task.ConfigureAwait(continueOnCapturedContext: false);
					List<Vector3> list2 = await secondPathfindingTask.ConfigureAwait(continueOnCapturedContext: false);
					if (firstPath.Count == 0)
					{
						if (list2.Count != 0)
						{
							bestAetheryte = secondAetheryte;
						}
					}
					else if (list2.Count != 0)
					{
						float num = CalculatePathDistance(bestAetheryte.Position, destination, firstPath);
						if (CalculatePathDistance(secondAetheryte.Position, destination, list2) < num)
						{
							bestAetheryte = secondAetheryte;
						}
					}
				}
			}
			return (Vector3.Distance(origin, destination) / 12f > Vector3.Distance(bestAetheryte.Position, destination) / 12f + 10f) ? bestAetheryte : null;
		}

		private static float CalculatePathDistance(Vector3 origin, Vector3 destination, IReadOnlyList<Vector3> path)
		{
			float num = Vector3.Distance(origin, path[0]);
			for (int i = 1; i < path.Count; i++)
			{
				num += Vector3.Distance(path[i - 1], path[i]);
			}
			return num + Vector3.Distance(path[path.Count - 1], destination);
		}

		private void EnqueueAetheryteArrival(TaskHelper taskHelper, PathfindingSession session)
		{
			taskHelper.Enqueue(delegate
			{
				if (pathfindingSession == session && session.TravelStage == PathfindingTravelStage.Aetheryte)
				{
					CrescentAetheryte aetheryte = session.Aetheryte;
					if ((object)aetheryte != null)
					{
						if (LocalPlayerState.DistanceTo3DSquared(aetheryte.Position) > 900f)
						{
							return false;
						}
						session.Aetheryte = null;
						session.TravelStage = PathfindingTravelStage.Pathfinding;
						session.StopAetherytePathfinding = null;
						return true;
					}
				}
				return true;
			}, null, 30000, TaskAbortBehaviour.AbortCurrent, null, delegate
			{
				session.Aetheryte = null;
				session.TravelStage = PathfindingTravelStage.Pathfinding;
				session.StopAetherytePathfinding?.Invoke();
				session.StopAetherytePathfinding = null;
			});
		}

		private void OnPathfindingUpdate(IFramework framework)
		{
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Invalid comparison between Unknown and I4
			PathfindingSession pathfindingSession = this.pathfindingSession;
			if (pathfindingSession == null)
			{
				return;
			}
			IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
			if (localPlayer == null)
			{
				StopPathfinding(pathfindingSession);
				return;
			}
			if (!allIslandEvents.Contains(pathfindingSession.Data))
			{
				StopPathfinding(pathfindingSession);
				return;
			}
			CrescentEvent crescentEvent = pathfindingSession.Data.Event;
			if ((object)crescentEvent != null && crescentEvent.Type == CrescentEventType.CE && (int)crescentEvent.CEState != 1)
			{
				StopPathfinding(pathfindingSession);
				return;
			}
			Vector2 vector = localPlayer.Position.ToVector2();
			CrescentEventType type = pathfindingSession.Data.Event.Type;
			bool flag = ((type == CrescentEventType.FATE || type == CrescentEventType.MagicPot) ? true : false);
			bool flag2 = flag;
			if (flag2 && pathfindingSession.TravelStage == PathfindingTravelStage.Pathfinding)
			{
				if (pathfindingSession.FateMonsterID == 0)
				{
					TrySetFateMonsterTarget(pathfindingSession);
				}
				if (pathfindingSession.FateMonsterID != 0)
				{
					EnsureFateMonsterSelected(pathfindingSession.FateMonsterID, vector);
					if (IsFateMonsterInHaterList(pathfindingSession.FateMonsterID))
					{
						StopPathfinding(pathfindingSession);
						return;
					}
				}
			}
			float num = ((pathfindingSession.Data.Event.Type == CrescentEventType.CE) ? 25f : pathfindingSession.Data.Event.Radius);
			bool num2 = !flag2 && num > 0f && Vector2.DistanceSquared(vector, pathfindingSession.Data.Event.Position.ToVector2()) <= num * num;
			float num3 = 25f;
			if (pathfindingSession.FateMonsterID != 0)
			{
				num3 += pathfindingSession.FateMonsterRadius * pathfindingSession.FateMonsterRadius;
			}
			bool flag3 = Vector2.DistanceSquared(vector, pathfindingSession.Destination.ToVector2()) <= num3;
			if (num2 || (flag3 && (!flag2 || pathfindingSession.FateMonsterID != 0)))
			{
				CompletePathfinding(pathfindingSession);
				return;
			}
			if (MainModule.config.InterruptPathfindingOnMovementInput && !pathfindingSession.IsMovementInterrupted && (pathfindingSession.PathfindingTask != null || pathfindingSession.Path.Count > 0) && IsMovementInputPressed())
			{
				pathfindingSession.IsMovementInterrupted = true;
				vnavmeshIPC.StopPathfind();
			}
			Task<List<Vector3>> pathfindingTask = pathfindingSession.PathfindingTask;
			if (pathfindingTask != null && pathfindingTask.IsCompleted)
			{
				pathfindingSession.PathfindingTask = null;
				if (pathfindingSession.PathfindingTaskDestination != pathfindingSession.Destination)
				{
					pathfindingSession.Path = new List<Vector3>();
					pathfindingSession.IsAtPathfindingDestination = false;
					return;
				}
				if (!pathfindingTask.IsCompletedSuccessfully)
				{
					AggregateException exception = pathfindingTask.Exception;
					if (exception != null)
					{
						DLog.Error("新月岛区域事件寻路失败", exception.GetBaseException());
					}
					StopPathfinding(pathfindingSession);
					return;
				}
				pathfindingSession.Path = pathfindingTask.Result;
				if (pathfindingSession.Path.Count == 0)
				{
					if (flag2)
					{
						pathfindingSession.IsAtPathfindingDestination = true;
					}
					else
					{
						StopPathfinding(pathfindingSession);
					}
					return;
				}
				pathfindingSession.IsAtPathfindingDestination = false;
				if (!pathfindingSession.IsMovementInterrupted)
				{
					vnavmeshIPC.SetPathfindTolerance(0.1f);
					vnavmeshIPC.PathfindWithPath(pathfindingSession.Path.ToList(), fly: false);
				}
			}
			if (pathfindingSession != null && pathfindingSession.TravelStage == PathfindingTravelStage.Pathfinding && !pathfindingSession.IsMovementInterrupted && pathfindingSession.PathfindingTask == null)
			{
				List<Vector3> path = pathfindingSession.Path;
				if (path != null && path.Count == 0 && !pathfindingSession.IsAtPathfindingDestination)
				{
					StartNavigationPath(pathfindingSession, localPlayer.Position);
				}
			}
			TryMountWhenPathIsLong(pathfindingSession);
		}

		private static void TryMountWhenPathIsLong(PathfindingSession session)
		{
			if (session.TravelStage == PathfindingTravelStage.Pathfinding && !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4] && !IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()) && vnavmeshIPC.GetIsPathfindRunning() && !(vnavmeshIPC.GetPathfindWayPoints().GetTotalDistanceSquared() <= 1600f) && Throttler.Shared.Throttle("OccultCrescentHelper-CEManager-Mount", 1000u))
			{
				OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)5, 9u, 3758096384uL, 0u, (UseActionMode)0);
			}
		}

		private void TrySetFateMonsterTarget(PathfindingSession session)
		{
			IGameObject gameObject = FindFateMonster(session);
			if (gameObject != null)
			{
				session.FateMonsterID = gameObject.EntityID;
				session.FateMonsterRadius = gameObject.HitboxRadius;
				SetNavigationTarget(session, gameObject.Position);
			}
		}

		private static IGameObject? FindFateMonster(PathfindingSession session)
		{
			return (from x in IOmenDalamudService<IObjectTable>.Instance().SearchObjects((IGameObject x) => x is IBattleNPC && x.IsTargetable && !x.IsDead && x.FateID == (ushort)session.Data.Event.DataID)
				orderby Vector3.DistanceSquared(x.Position, session.Destination)
				select x).FirstOrDefault();
		}

		private static void EnsureFateMonsterSelected(uint entityID, Vector2 playerPosition)
		{
			IGameObject? target = TargetManager.Target;
			if (target == null || target.EntityID != entityID)
			{
				IGameObject gameObject = IOmenDalamudService<IObjectTable>.Instance().SearchByEntityID(entityID, IObjectTable.CharactersRange);
				if (gameObject != null && Vector2.DistanceSquared(playerPosition, gameObject.Position.ToVector2()) <= 2500f)
				{
					TargetManager.Target = gameObject;
				}
			}
		}

		private unsafe static bool IsFateMonsterInHaterList(uint entityID)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			Hater hater = ((UIState)UIState.Instance()).Hater;
			int num = Math.Min(hater.HaterCount, ((Hater)(ref hater)).Haters.Length);
			for (int i = 0; i < num; i++)
			{
				if (((Hater)(ref hater)).Haters[i].EntityId == entityID)
				{
					return true;
				}
			}
			return false;
		}

		private void SetNavigationTarget(PathfindingSession session, Vector3 destination)
		{
			if (session.Destination == destination)
			{
				return;
			}
			session.Destination = destination;
			session.IsAtPathfindingDestination = false;
			session.Path = new List<Vector3>();
			vnavmeshIPC.StopPathfind();
			if (session.PathfindingTask == null && !session.IsMovementInterrupted)
			{
				IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
				if (localPlayer != null)
				{
					StartNavigationPath(session, localPlayer.Position);
				}
			}
		}

		private void StartNavigationPath(PathfindingSession session, Vector3 origin)
		{
			if (session.PathfindingTask != null || session.IsMovementInterrupted)
			{
				return;
			}
			try
			{
				session.PathfindingTaskDestination = session.Destination;
				session.PathfindingTask = vnavmeshIPC.PathfindCancelable(origin, session.Destination, fly: false, session.CancellationTokenSource.Token);
				if (session.PathfindingTask == null)
				{
					StopPathfinding(session);
				}
			}
			catch (Exception ex)
			{
				DLog.Error("新月岛区域事件寻路请求失败", ex);
				StopPathfinding(session);
			}
		}

		private void OnPathfindingDraw()
		{
			PathfindingSession pathfindingSession = this.pathfindingSession;
			if (pathfindingSession != null)
			{
				DrawPath(pathfindingSession.Path);
			}
		}

		private static bool IsMovementInputPressed()
		{
			if (!DalamudServiceExtension.Instance<IKeyState>()[(VirtualKey)87] && !DalamudServiceExtension.Instance<IKeyState>()[(VirtualKey)65] && !DalamudServiceExtension.Instance<IKeyState>()[(VirtualKey)83])
			{
				return DalamudServiceExtension.Instance<IKeyState>()[(VirtualKey)68];
			}
			return true;
		}

		private static void DrawPath(List<Vector3> path)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			if (path.Count == 0)
			{
				return;
			}
			ImDrawListPtr foregroundDrawList = ImGui.GetForegroundDrawList();
			IGameGui val = DalamudServiceExtension.Instance<IGameGui>();
			Vector3 vector = path[0];
			Vector2 vector2 = default(Vector2);
			bool flag2 = default(bool);
			bool flag = val.WorldToScreen(vector, ref vector2, ref flag2);
			Vector2 vector4 = default(Vector2);
			bool flag4 = default(bool);
			for (int i = 1; i < path.Count; i++)
			{
				Vector3 vector3 = path[i];
				bool flag3 = val.WorldToScreen(vector3, ref vector4, ref flag4);
				DrawPathSegment(foregroundDrawList, val, vector, vector3, flag ? new Vector2?(vector2) : ((Vector2?)null), flag3 ? new Vector2?(vector4) : ((Vector2?)null));
				if (flag2)
				{
					((ImDrawListPtr)(ref foregroundDrawList)).AddCircleFilled(vector2, 4f * Globals.GlobalUIScale, PathPointColor);
				}
				vector = vector3;
				vector2 = vector4;
				flag = flag3;
				flag2 = flag4;
			}
			if (flag2)
			{
				((ImDrawListPtr)(ref foregroundDrawList)).AddCircleFilled(vector2, 4f * Globals.GlobalUIScale, PathPointColor);
			}
		}

		private static void DrawPathSegment(ImDrawListPtr drawList, IGameGui gameGUI, Vector3 worldStart, Vector3 worldEnd, Vector2? screenStart, Vector2? screenEnd)
		{
			if (screenStart.HasValue)
			{
				Vector2 valueOrDefault = screenStart.GetValueOrDefault();
				if (screenEnd.HasValue)
				{
					Vector2 valueOrDefault2 = screenEnd.GetValueOrDefault();
					((ImDrawListPtr)(ref drawList)).AddLine(valueOrDefault, valueOrDefault2, PathLineColor, 2f * Globals.GlobalUIScale);
					return;
				}
			}
			if (screenStart.HasValue || screenEnd.HasValue)
			{
				Vector3 inFrontWorldPoint = (screenStart.HasValue ? worldStart : worldEnd);
				Vector3 behindWorldPoint = ((!screenStart.HasValue) ? worldStart : worldEnd);
				Vector2 vector = screenStart ?? screenEnd.Value;
				if (TryClipPathSegmentToNearPlane(gameGUI, inFrontWorldPoint, behindWorldPoint, out var clippedScreenPoint))
				{
					((ImDrawListPtr)(ref drawList)).AddLine(vector, clippedScreenPoint, PathLineColor, 2f * Globals.GlobalUIScale);
				}
			}
		}

		private static bool TryClipPathSegmentToNearPlane(IGameGui gameGUI, Vector3 inFrontWorldPoint, Vector3 behindWorldPoint, out Vector2 clippedScreenPoint)
		{
			float num = 0f;
			float num2 = 1f;
			Vector2 vector = default(Vector2);
			bool flag = default(bool);
			for (int i = 0; i < 8; i++)
			{
				float num3 = (num + num2) * 0.5f;
				if (gameGUI.WorldToScreen(Vector3.Lerp(inFrontWorldPoint, behindWorldPoint, num3), ref vector, ref flag))
				{
					num = num3;
				}
				else
				{
					num2 = num3;
				}
			}
			return gameGUI.WorldToScreen(Vector3.Lerp(inFrontWorldPoint, behindWorldPoint, num), ref clippedScreenPoint, ref flag);
		}

		private void CompletePathfinding(PathfindingSession session)
		{
			CrescentEventType type = session.Data.Event.Type;
			bool flag;
			switch (type)
			{
			case CrescentEventType.FATE:
			case CrescentEventType.MagicPot:
				flag = MainModule.config.IsEnabledDismountFATE;
				break;
			case CrescentEventType.CE:
				flag = MainModule.config.IsEnabledDismountCE;
				break;
			default:
				flag = false;
				break;
			}
			bool flag2 = flag;
			if (!StopPathfinding(session))
			{
				return;
			}
			if (flag2)
			{
				OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.Dismount);
			}
			flag = ((type == CrescentEventType.FATE || type == CrescentEventType.MagicPot) ? true : false);
			if (!flag || (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4] && !flag2))
			{
				return;
			}
			TaskHelper taskHelper = ceTaskHelper;
			if (taskHelper != null)
			{
				taskHelper.Enqueue(() => !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4]);
				taskHelper.Enqueue(delegate
				{
					OmenServiceBase<ChatManager>.Instance().SendMessage("/facetarget");
				});
				taskHelper.DelayNext(100);
				taskHelper.Enqueue(delegate
				{
					OmenServiceBase<ChatManager>.Instance().SendMessage("/automove on");
				});
			}
		}

		private bool StopPathfinding(PathfindingSession? expectedSession = null)
		{
			PathfindingSession pathfindingSession = ((expectedSession == null) ? Interlocked.Exchange(ref this.pathfindingSession, null) : Interlocked.CompareExchange(ref this.pathfindingSession, null, expectedSession));
			if (expectedSession != null && pathfindingSession != expectedSession)
			{
				return false;
			}
			ceTaskHelper?.Abort();
			vnavmeshIPC.StopPathfind();
			if (pathfindingSession == null)
			{
				return true;
			}
			pathfindingSession.StopAetherytePathfinding?.Invoke();
			pathfindingSession.CancellationTokenSource.Cancel();
			pathfindingSession.AetheryteRouteCancellationTokenSource?.Dispose();
			pathfindingSession.CancellationTokenSource.Dispose();
			return true;
		}

		private void NotifyNewCE(IslandEventData ce)
		{
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0385: Unknown result type (might be due to invalid IL or missing references)
			if (!MainModule.config.IsEnabledNotifyEventsCategoried.GetValueOrDefault(ce.Event.Type, defaultValue: false))
			{
				return;
			}
			string nameDisplay = ce.Event.NameDisplay;
			Vector2 vector = PositionHelper.WorldToMap(ce.Event.Position.ToVector2(), GameState.MapData);
			SeStringBuilder val = new SeStringBuilder().AddUiForeground((ushort)25).AddText("[" + MainModule.Info.Title + "] ").AddUiForegroundOff()
				.AddText(ce.GetNotificationTitle() ?? "")
				.Add((Payload)(object)NewLinePayload.Payload)
				.AddText(LanguageManager.Get("Name") + ": ")
				.AddUiForeground((ushort)45)
				.AddText(nameDisplay)
				.AddUiForegroundOff()
				.Add((Payload)(object)NewLinePayload.Payload)
				.AddText(LanguageManager.Get("Position") + ": ")
				.Append(SeString.CreateMapLink(GameState.TerritoryTypeData.ExtractPlaceName(), vector.X, vector.Y, 0.05f));
			if (ce.Event.SpecialWeaponMaterialID != 0)
			{
				val.Add((Payload)(object)NewLinePayload.Payload).AddText(LanguageManager.Get("Item") + ": ").AddItemLink(ce.Event.SpecialWeaponMaterialID, false, (string)null)
					.AddText($" ({LuminaWrapper.GetAddonText(358u)}: {LocalPlayerState.GetItemCount(ce.Event.SpecialWeaponMaterialID)})");
			}
			List<uint> specialRewards = ce.Event.SpecialRewards;
			if (specialRewards != null && specialRewards.Count > 0)
			{
				string text = LanguageManager.Get("OccultCrescentHelper-CEManager-SpecialRewards");
				val.Add((Payload)(object)NewLinePayload.Payload).AddText(text + ": ");
				foreach (uint item in specialRewards)
				{
					bool? flag = CrescentEvent.IsSpecialRewardUnlocked(item);
					ushort num = (ushort)((!flag.HasValue) ? 32 : ((flag != true) ? 17 : 45));
					ushort num2 = num;
					string text2 = ((!flag.HasValue) ? "?" : ((flag != true) ? "x" : "✓"));
					string text3 = text2;
					val.Add((Payload)(object)NewLinePayload.Payload).AddText("      ").AddItemLink(item, (ItemKind)0, (string)null)
						.AddText(" (")
						.AddUiForeground(num2)
						.AddText(text3)
						.AddUiForegroundOff()
						.AddText(")");
				}
			}
			if (CanStartPathfinding(ce))
			{
				DalamudLinkPayload orAddLinkPayload = ce.GetOrAddLinkPayload(this);
				val.Add((Payload)(object)NewLinePayload.Payload).AddText(LanguageManager.Get("Operation") + ": ").Add((Payload)(object)RawPayload.LinkTerminator)
					.Add((Payload)(object)orAddLinkPayload)
					.AddText("[")
					.AddIcon((BitmapFontIcon)66)
					.AddUiForeground((ushort)35)
					.AddText(LanguageManager.Get("Pathfind") ?? "")
					.AddUiForegroundOff()
					.AddText("]")
					.Add((Payload)(object)RawPayload.LinkTerminator);
			}
			OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(val.Build().Encode()));
			OmenServiceBase<NotifyHelper>.Instance().NotificationInfo(nameDisplay ?? "", ce.GetNotificationTitle() ?? "");
			NotifyHelper.Speak(ce.GetNotificationTitle() ?? "");
		}
	}

	private class OthersManager : BaseIslandModule
	{
		private class LongTimeBuffButton : OverlayNode
		{
			private readonly OthersManager manager;

			private readonly TextButtonNode buttonNode;

			private bool isAnyNearby;

			private Vector3 nearbyPosition;

			public override OverlayLayer OverlayLayer => OverlayLayer.BehindUserInterface;

			public override bool HideWithNativeUi => true;

			public LongTimeBuffButton(OthersManager manager)
			{
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_003f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0054: Unknown result type (might be due to invalid IL or missing references)
				this.manager = manager;
				buttonNode = new TextButtonNode
				{
					Size = new Vector2(48f, 24f),
					String = ReadOnlySeString.op_Implicit(new SeStringBuilder().AddIcon((BitmapFontIcon)100).Build().Encode()),
					TextTooltip = ReadOnlySeString.op_Implicit(LanguageManager.Get("OccultCrescentHelper-Command-PBuff-Help")),
					OnClick = delegate
					{
						OmenServiceBase<ChatManager>.Instance().SendMessage("/pdr pbuff");
					},
					Scale = new Vector2(3f)
				};
				buttonNode.AttachNode(this);
			}

			protected unsafe override void OnUpdate()
			{
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0021: Invalid comparison between Unknown and I4
				if (manager.MainModule.config.IsEnabledKnowledgeCrystalFastUse && (int)GameState.TerritoryIntendedUse == 61 && !IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()))
				{
					if (Throttler.Shared.Throttle("OccultCrescentHelper-OthersManager-LongTimeBuffButton"))
					{
						isAnyNearby = CrescentSupportJob.TryFindKnowledgeCrystal(out IGameObject knowledgeCrystal);
						nearbyPosition = (isAnyNearby ? knowledgeCrystal.Position : Vector3.Zero);
					}
					Vector2 screenPos = default(Vector2);
					bool inView = default(bool);
					if (nearbyPosition != Vector3.Zero && GameViewHelper.WorldToScreen(nearbyPosition, out screenPos, out inView) && inView)
					{
						buttonNode.IsEnabled = LocalPlayerState.DistanceTo2DSquared(nearbyPosition.ToVector2()) <= 10f;
						buttonNode.IsVisible = true;
						buttonNode.Position = screenPos - (*buttonNode.Node).GetNodeState().Size / 2f;
						IsVisible = true;
					}
					else
					{
						buttonNode.IsVisible = false;
						IsVisible = false;
					}
				}
				else
				{
					buttonNode.IsVisible = false;
					IsVisible = false;
				}
			}
		}

		private TextButtonNode? settingButton;

		private IconButtonNode? mapButton;

		private OverlayController? overlayController;

		private TaskHelper? othersTaskHelper;

		private bool isJustLogin;

		public OthersManager(OccultCrescentHelper mainModule)
			: base(mainModule)
		{
		}

		public override void Init()
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Expected O, but got Unknown
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Expected O, but got Unknown
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Expected O, but got Unknown
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Expected O, but got Unknown
			if (othersTaskHelper == null)
			{
				othersTaskHelper = new TaskHelper
				{
					TimeoutMS = 30000
				};
			}
			DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)5, "MKDInfo", new AddonEventDelegate(OnAddon));
			DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, "MKDInfo", new AddonEventDelegate(OnAddon));
			DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)5, "_CharaSelectListMenu", new AddonEventDelegate(OnLogin));
			DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)5, "_ActionContents", new AddonEventDelegate(OnActionContents));
			DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += OnZoneChanged;
			OnZoneChanged(0u);
		}

		public override void Uninit()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Expected O, but got Unknown
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Expected O, but got Unknown
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Expected O, but got Unknown
			DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnActionContents });
			DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnLogin });
			DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChanged;
			DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddon });
			settingButton?.Dispose();
			settingButton = null;
			mapButton?.Dispose();
			mapButton = null;
			othersTaskHelper?.Abort();
			othersTaskHelper?.Dispose();
			othersTaskHelper = null;
			overlayController?.Dispose();
			overlayController = null;
			isJustLogin = false;
		}

		public override void DrawConfig()
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_045d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0462: Unknown result type (might be due to invalid IL or missing references)
			//IL_0467: Unknown result type (might be due to invalid IL or missing references)
			//IL_0469: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_047f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_051d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0579: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0316: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0337: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
			TabBarDisposable val = ImRaii.TabBar(ImU8String.op_Implicit("TabBar"));
			try
			{
				if (!val)
				{
					return;
				}
				TabItemDisposable val2 = ImRaii.TabItem(ImU8String.op_Implicit(LanguageManager.Get("General")));
				try
				{
					if (TabItemDisposable.op_Implicit(val2))
					{
						Vector4 vector = KnownColor.Chocolate.ToVector4();
						ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("OccultCrescentHelper-OthersManager-ModifyDefaultEnterZonePosition")));
						IndentDisposable val3 = ImRaii.PushIndent(1, true);
						try
						{
							ItemWidthDisposable val4 = ImRaii.ItemWidth(250f * Globals.GlobalUIScale);
							try
							{
								ImGui.TextUnformatted(ImU8String.op_Implicit(LuminaWrapper.GetZonePlaceName(1252u)));
								ImU8String val5 = ImU8String.op_Implicit("###DefaultPositionEnterZoneSouthHornInput");
								ref Vector3 defaultPositionEnterZoneSouthHorn = ref MainModule.config.DefaultPositionEnterZoneSouthHorn;
								ImU8String val6 = default(ImU8String);
								ImGui.InputFloat3(val5, ref defaultPositionEnterZoneSouthHorn, 0f, 0f, val6, (ImGuiInputTextFlags)0);
								if (ImGui.IsItemDeactivatedAfterEdit())
								{
									MainModule.config.Save(MainModule);
								}
								ImGui.SameLine();
								val6 = new ImU8String(38, 1);
								((ImU8String)(ref val6)).AppendFormatted<string>(LanguageManager.Get("Current"));
								((ImU8String)(ref val6)).AppendLiteral("##SetDefaultPositionEnterZoneSouthHorn");
								if (ImGui.Button(val6, default(Vector2)))
								{
									MainModule.config.DefaultPositionEnterZoneSouthHorn = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer?.Position ?? default(Vector3);
									MainModule.config.Save(MainModule);
								}
								bool flag = true;
								foreach (CrescentAetheryte southHornAetheryte in CrescentAetheryte.SouthHornAetherytes)
								{
									if (!flag)
									{
										ImGui.SameLine();
									}
									flag = false;
									ImU8String val7 = new ImU8String(38, 1);
									((ImU8String)(ref val7)).AppendFormatted<string>(southHornAetheryte.Name);
									((ImU8String)(ref val7)).AppendLiteral("##SetDefaultPositionEnterZoneSouthHorn");
									if (ImGui.Button(val7, default(Vector2)))
									{
										MainModule.config.DefaultPositionEnterZoneSouthHorn = southHornAetheryte.Position;
										MainModule.config.Save(MainModule);
									}
								}
								ImGui.Spacing();
								ImGui.TextUnformatted(ImU8String.op_Implicit(LuminaWrapper.GetZonePlaceName(1346u)));
								ImU8String val8 = ImU8String.op_Implicit("###DefaultPositionEnterZoneNorthHornInput");
								ref Vector3 defaultPositionEnterZoneNorthHorn = ref MainModule.config.DefaultPositionEnterZoneNorthHorn;
								ImU8String val9 = default(ImU8String);
								ImGui.InputFloat3(val8, ref defaultPositionEnterZoneNorthHorn, 0f, 0f, val9, (ImGuiInputTextFlags)0);
								if (ImGui.IsItemDeactivatedAfterEdit())
								{
									MainModule.config.Save(MainModule);
								}
								ImGui.SameLine();
								val9 = new ImU8String(38, 1);
								((ImU8String)(ref val9)).AppendFormatted<string>(LanguageManager.Get("Current"));
								((ImU8String)(ref val9)).AppendLiteral("##SetDefaultPositionEnterZoneNorthHorn");
								if (ImGui.Button(val9, default(Vector2)))
								{
									MainModule.config.DefaultPositionEnterZoneNorthHorn = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer?.Position ?? default(Vector3);
									MainModule.config.Save(MainModule);
								}
								for (int i = 0; i < CrescentAetheryte.NorthHornAetherytes.Count; i++)
								{
									if (i != 0)
									{
										ImGui.SameLine();
									}
									CrescentAetheryte crescentAetheryte = CrescentAetheryte.NorthHornAetherytes[i];
									ImU8String val10 = new ImU8String(38, 1);
									((ImU8String)(ref val10)).AppendFormatted<string>(crescentAetheryte.Name);
									((ImU8String)(ref val10)).AppendLiteral("##SetDefaultPositionEnterZoneNorthHorn");
									if (ImGui.Button(val10, default(Vector2)))
									{
										MainModule.config.DefaultPositionEnterZoneNorthHorn = crescentAetheryte.Position;
										MainModule.config.Save(MainModule);
									}
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
						ImGui.NewLine();
						ImGui.TextColored(KnownColor.Chocolate.ToUInt(), ImU8String.op_Implicit(LanguageManager.Get("OccultCrescentHelper-OthersManager-AutoEnableDisablePlugins")));
						ImGuiOm.HelpMarker(LanguageManager.Get("OccultCrescentHelper-OthersManager-AutoEnableDisablePlugins-Help"), 20f * Globals.GlobalUIScale, (FontAwesomeIcon)61530);
						val3 = ImRaii.PushIndent(1, true);
						try
						{
							DrawDutyCommands(LanguageManager.Get("OccultCrescentHelper-OthersManager-JoinDutyCommands"), "###JoinDutyCommandsInput", ref MainModule.config.JoinDutyCommands);
							ImGui.Spacing();
							DrawDutyCommands(LanguageManager.Get("OccultCrescentHelper-OthersManager-LeaveDutyCommands"), "###LeaveDutyCommandsInput", ref MainModule.config.LeaveDutyCommands);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
				}
				finally
				{
					((TabItemDisposable)(ref val2)).Dispose();
				}
				TabItemDisposable val11 = ImRaii.TabItem(ImU8String.op_Implicit(LanguageManager.Get("ModuleCategory-Interface")));
				try
				{
					if (!TabItemDisposable.op_Implicit(val11))
					{
						return;
					}
					if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("OccultCrescentHelper-OthersManager-ModifyInfoHUD")), ref MainModule.config.IsEnabledModifyInfoHUD))
					{
						MainModule.config.Save(MainModule);
						if (!MainModule.config.IsEnabledModifyInfoHUD)
						{
							settingButton?.Dispose();
							settingButton = null;
							mapButton?.Dispose();
							mapButton = null;
						}
					}
					ImGuiOm.HelpMarker(LanguageManager.Get("OccultCrescentHelper-OthersManager-ModifyInfoHUD-Help"), 20f * Globals.GlobalUIScale, (FontAwesomeIcon)61530);
					if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("OccultCrescentHelper-OthersManager-HideDutyCommand")), ref MainModule.config.IsEnabledHideDutyCommand))
					{
						MainModule.config.Save(MainModule);
					}
					ImGuiOm.HelpMarker(LanguageManager.Get("OccultCrescentHelper-OthersManager-HideDutyCommand-Help"), 20f * Globals.GlobalUIScale, (FontAwesomeIcon)61530);
					if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("OccultCrescentHelper-OthersManager-FastUseKnowledgeCrystal")), ref MainModule.config.IsEnabledKnowledgeCrystalFastUse))
					{
						MainModule.config.Save(MainModule);
					}
					ImGuiOm.HelpMarker(LanguageManager.Get("OccultCrescentHelper-OthersManager-FastUseKnowledgeCrystal-Help"), 20f * Globals.GlobalUIScale, (FontAwesomeIcon)61530);
				}
				finally
				{
					((TabItemDisposable)(ref val11)).Dispose();
				}
			}
			finally
			{
				((TabBarDisposable)(ref val)).Dispose();
			}
		}

		private void DrawDutyCommands(string label, string inputID, ref string commands)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			ImGui.TextUnformatted(ImU8String.op_Implicit(label));
			ImGui.SetNextItemWidth(-1f);
			ImGui.InputTextMultiline(ImU8String.op_Implicit(inputID), ref commands, 4096, new Vector2(-1f, 120f * Globals.GlobalUIScale), (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
			string text = LimitDutyCommandLines(commands);
			bool num = text != commands;
			if (num)
			{
				commands = text;
			}
			if (num || ImGui.IsItemDeactivatedAfterEdit())
			{
				MainModule.config.Save(MainModule);
			}
			ImU8String val = default(ImU8String);
			((ImU8String)(ref val))._002Ector(1, 2);
			((ImU8String)(ref val)).AppendFormatted<int>(GetDutyCommandLineCount(commands));
			((ImU8String)(ref val)).AppendLiteral("/");
			((ImU8String)(ref val)).AppendFormatted<int>(15);
			ImGui.TextDisabled(val);
		}

		private void OnZoneChanged(uint zone)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Invalid comparison between Unknown and I4
			if ((int)GameState.TerritoryIntendedUse != 61)
			{
				overlayController?.Dispose();
				overlayController = null;
				isJustLogin = false;
				if (GameState.TerritoryType == 1278 && !string.IsNullOrWhiteSpace(MainModule.config.LeaveDutyCommands) && Throttler.Shared.Throttle("OccultCrescentHelper.OthersManager.Leave", 3000u))
				{
					OmenServiceBase<ChatManager>.Instance().ExecuteMacro(MainModule.config.LeaveDutyCommands);
				}
				return;
			}
			if (overlayController == null)
			{
				overlayController = new OverlayController();
			}
			overlayController.AddNode(new LongTimeBuffButton(this));
			if (!isJustLogin && IConditionExtension.get_IsBetweenAreas(DalamudServiceExtension.Instance<ICondition>()) && (int)GameState.TerritoryIntendedUse == 61)
			{
				Vector3 destination = ((GameState.TerritoryType == 1346) ? MainModule.config.DefaultPositionEnterZoneNorthHorn : MainModule.config.DefaultPositionEnterZoneSouthHorn);
				othersTaskHelper.Abort();
				othersTaskHelper.Enqueue(delegate
				{
					IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
					if (localPlayer == null)
					{
						return false;
					}
					if (localPlayer.IsDead)
					{
						return true;
					}
					OmenServiceBase<MovementManager>.Instance().TPPlayerAddress(destination);
					return true;
				});
			}
			if (!string.IsNullOrWhiteSpace(MainModule.config.JoinDutyCommands) && Throttler.Shared.Throttle("OccultCrescentHelper.OthersManager.Join", 3000u))
			{
				OmenServiceBase<ChatManager>.Instance().ExecuteMacro(MainModule.config.JoinDutyCommands);
			}
		}

		private unsafe void OnActionContents(AddonEvent type, AddonArgs args)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			if ((int)GameState.TerritoryIntendedUse == 61 && Throttler.Shared.Throttle("OccultCrescentHelper-OthersManager-ActionDetail") && Addons.ActionContents != null)
			{
				AtkResNode* nodeById = ((AtkUnitBase)Addons.ActionContents).GetNodeById(17u);
				if (nodeById != null)
				{
					((AtkResNode)nodeById).ToggleVisibility(!MainModule.config.IsEnabledHideDutyCommand);
				}
			}
		}

		private void OnLogin(AddonEvent type, AddonArgs args)
		{
			isJustLogin = true;
		}

		private unsafe void OnAddon(AddonEvent type, AddonArgs args)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Invalid comparison between Unknown and I4
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Invalid comparison between Unknown and I4
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			if ((int)type != 5)
			{
				if ((int)type == 6)
				{
					settingButton = null;
					mapButton = null;
				}
			}
			else
			{
				if (Addons.MKDInfo == null)
				{
					return;
				}
				if (MainModule.config.IsEnabledModifyInfoHUD && settingButton == null)
				{
					AtkImageNode* imageNodeById = ((AtkUnitBase)Addons.MKDInfo).GetImageNodeById(24u);
					if (imageNodeById != null)
					{
						((AtkImageNode)imageNodeById).ToggleVisibility(false);
						((AtkImageNode)imageNodeById).SetAlpha((byte)0);
					}
					settingButton = new TextButtonNode
					{
						Position = new Vector2(58f, 160f),
						Size = new Vector2(40f, 32f),
						IsVisible = true,
						String = ReadOnlySeString.op_Implicit(new SeStringBuilder().AddIcon((BitmapFontIcon)101).Build().Encode()),
						TextTooltip = ReadOnlySeString.op_Implicit(MainModule.Info.Title),
						OnClick = delegate
						{
							Overlay? overlay = MainModule.Overlay;
							((Window)overlay).IsOpen = !((Window)overlay).IsOpen;
						}
					};
					settingButton.AddColor = new Vector3(0f, 0.1254902f, 0.5019608f);
					settingButton.MultiplyColor = new Vector3(20f / 51f);
					settingButton.BackgroundNode.IsVisible = false;
					settingButton.AttachNode(((AtkUnitBase)Addons.MKDInfo).GetNodeById(67u));
				}
				if (!MainModule.config.IsEnabledModifyInfoHUD || mapButton != null)
				{
					return;
				}
				mapButton = new IconButtonNode
				{
					Position = new Vector2(58f, 128f),
					Size = new Vector2(35f, 32f),
					IsVisible = true,
					IconId = 60561u,
					TextTooltip = ReadOnlySeString.op_Implicit(LuminaWrapper.GetAddonText(8441u)),
					OnClick = delegate
					{
						AgentMap* ptr = AgentMap.Instance();
						if (ptr != null)
						{
							if (!((AgentMap)ptr).IsAgentActive())
							{
								((AgentMap)ptr).OpenMap(GameState.Map, GameState.TerritoryType, (string)null, (MapType)1);
							}
							else
							{
								((AgentMap)ptr).Hide();
							}
						}
					}
				};
				mapButton.ImageNode.Scale *= 1.2f;
				mapButton.ImageNode.Position -= new Vector2(10f, 0f);
				mapButton.BackgroundNode.IsVisible = false;
				mapButton.AttachNode(((AtkUnitBase)Addons.MKDInfo).GetNodeById(67u));
			}
		}
	}

	private class SupportJobManager(OccultCrescentHelper mainModule) : BaseIslandModule(mainModule)
	{
		private sealed record ActionInsertionRule(CrescentSupportJob SupportJob, uint ActionID, Func<bool> EnabledCondition, Func<IBattleChara, bool> UseCondition);

		[CompilerGenerated]
		private static class _003C_003EO
		{
			public static Func<IBattleChara, bool> _003C0_003E__ShouldUseOffensiveAria;

			public static HandlerDelegate _003C1_003E__OnCommandBuff;

			public static UseActionManager.PostUseActionLocationDelegate _003C2_003E__OnPostUseAction;
		}

		private static TaskHelper? SupportJobTaskHelper;

		private static TaskHelper? ActionInsertionTaskHelper;

		private ActionInsertionRule[] actionInsertionRules = Array.Empty<ActionInsertionRule>();

		private const string COMMAND_BUFF = "pbuff";

		private const uint ACTION_FREELANCER_BUFF = 46606u;

		private const uint ACTION_OFFENSIVE_ARIA = 41608u;

		private const uint STATUS_OFFENSIVE_ARIA = 4247u;

		private const float OFFENSIVE_ARIA_REFRESH_THRESHOLD = 20f;

		private const int RECAST_TIME_WINDOW = 500;

		private const int ANIMATION_LOCK = 100;

		public override void Init()
		{
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Expected O, but got Unknown
			//IL_00ad: Expected O, but got Unknown
			SupportJobTaskHelper = new TaskHelper();
			ActionInsertionTaskHelper = new TaskHelper
			{
				TimeoutMS = 60000
			};
			actionInsertionRules = new ActionInsertionRule[1]
			{
				new ActionInsertionRule(CrescentSupportJob.Bard, 41608u, () => MainModule.config.IsEnabledBardOffensiveAria, ShouldUseOffensiveAria)
			};
			CommandManager commandManager = OmenServiceBase<CommandManager>.Instance();
			object obj = _003C_003EO._003C1_003E__OnCommandBuff;
			if (obj == null)
			{
				HandlerDelegate val = OnCommandBuff;
				_003C_003EO._003C1_003E__OnCommandBuff = val;
				obj = (object)val;
			}
			commandManager.AddSubCommand("pbuff", new CommandInfo((HandlerDelegate)obj)
			{
				HelpMessage = (LanguageManager.Get("OccultCrescentHelper-Command-PBuff-Help") ?? "")
			});
			OmenServiceBase<UseActionManager>.Instance().RegPreUseAction(OnPreUseAction);
			OmenServiceBase<UseActionManager>.Instance().RegPreCharacterCompleteCast(OnPreCompleteCast);
			OmenServiceBase<UseActionManager>.Instance().RegPostCharacterCompleteCast(OnPostCompleteCast);
			OmenServiceBase<UseActionManager>.Instance().RegPostUseActionLocation(OnPostUseAction);
		}

		public override void Uninit()
		{
			OmenServiceBase<CommandManager>.Instance().RemoveSubCommand("pbuff");
			SupportJobTaskHelper?.Abort();
			SupportJobTaskHelper?.Dispose();
			SupportJobTaskHelper = null;
			ActionInsertionTaskHelper?.Abort();
			ActionInsertionTaskHelper?.Dispose();
			ActionInsertionTaskHelper = null;
			actionInsertionRules = Array.Empty<ActionInsertionRule>();
			OmenServiceBase<UseActionManager>.Instance().Unreg(OnPreUseAction);
			OmenServiceBase<UseActionManager>.Instance().Unreg(OnPreCompleteCast);
			OmenServiceBase<UseActionManager>.Instance().Unreg(OnPostCompleteCast);
			OmenServiceBase<UseActionManager>.Instance().Unreg(OnPostUseAction);
		}

		public override void DrawConfig()
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_0295: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
			Vector4 vector = KnownColor.Chocolate.ToVector4();
			ImGui.TextColored(ref vector, ImU8String.op_Implicit(LuminaWrapper.GetMKDSupportJobName(6u)));
			IndentDisposable val = ImRaii.PushIndent(1, true);
			try
			{
				ImU8String val2 = new ImU8String(19, 1);
				((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get("OccultCrescentHelper-SupportJobManager-Bard-OffensiveAria"));
				((ImU8String)(ref val2)).AppendLiteral("##BardOffensiveAria");
				if (ImGui.Checkbox(val2, ref MainModule.config.IsEnabledBardOffensiveAria))
				{
					MainModule.config.Save(MainModule);
				}
				ImGuiOm.HelpMarker(LanguageManager.Get("OccultCrescentHelper-SupportJobManager-Bard-OffensiveAria-Help"), 20f * Globals.GlobalUIScale, (FontAwesomeIcon)61530);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
			ImGui.NewLine();
			vector = KnownColor.Chocolate.ToVector4();
			ImGui.TextColored(ref vector, ImU8String.op_Implicit(LuminaWrapper.GetMKDSupportJobName(3u)));
			val = ImRaii.PushIndent(1, true);
			try
			{
				ImU8String val3 = new ImU8String(12, 1);
				((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get("OccultCrescentHelper-SupportJobManager-Monk-PhantomKickNoMove"));
				((ImU8String)(ref val3)).AppendLiteral("##NoMoveMonk");
				if (ImGui.Checkbox(val3, ref MainModule.config.IsEnabledMonkKickNoMove))
				{
					MainModule.config.Save(MainModule);
				}
				ImGuiOm.HelpMarker(LanguageManager.Get("OccultCrescentHelper-SupportJobManager-Monk-PhantomKickNoMove-Help"), 20f * Globals.GlobalUIScale, (FontAwesomeIcon)61530);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
			ImGui.NewLine();
			vector = KnownColor.Chocolate.ToVector4();
			ImGui.TextColored(ref vector, ImU8String.op_Implicit(LuminaWrapper.GetMKDSupportJobName(2u)));
			val = ImRaii.PushIndent(1, true);
			try
			{
				ImU8String val4 = new ImU8String(23, 1);
				((ImU8String)(ref val4)).AppendFormatted<string>(LanguageManager.Get("OccultCrescentHelper-SupportJobManager-Berserker-RageAutoFace"));
				((ImU8String)(ref val4)).AppendLiteral("##BerserkerRageAutoFace");
				if (ImGui.Checkbox(val4, ref MainModule.config.IsEnabledBerserkerRageAutoFace))
				{
					MainModule.config.Save(MainModule);
				}
				ImGuiOm.HelpMarker(LanguageManager.Get("OccultCrescentHelper-SupportJobManager-Berserker-RageAutoFace-Help"), 20f * Globals.GlobalUIScale, (FontAwesomeIcon)61530);
				ImU8String val5 = new ImU8String(22, 1);
				((ImU8String)(ref val5)).AppendFormatted<string>(LanguageManager.Get("OccultCrescentHelper-SupportJobManager-Berserker-RageReplace"));
				((ImU8String)(ref val5)).AppendLiteral("##BerserkerRageReplace");
				if (ImGui.Checkbox(val5, ref MainModule.config.IsEnabledBerserkerRageReplace))
				{
					MainModule.config.Save(MainModule);
				}
				ImGuiOm.HelpMarker(LanguageManager.Get("OccultCrescentHelper-SupportJobManager-Berserker-RageReplace-Help"), 20f * Globals.GlobalUIScale, (FontAwesomeIcon)61530);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
			ImGui.NewLine();
			vector = KnownColor.Chocolate.ToVector4();
			ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("Command")));
			val = ImRaii.PushIndent(1, true);
			try
			{
				ImU8String val6 = new ImU8String(6, 2);
				((ImU8String)(ref val6)).AppendLiteral("/pdr ");
				((ImU8String)(ref val6)).AppendFormatted<string>("pbuff");
				((ImU8String)(ref val6)).AppendLiteral(" ");
				((ImU8String)(ref val6)).AppendFormatted<string>(LanguageManager.Get("OccultCrescentHelper-Command-PBuff-Help"));
				ImGui.TextWrapped(val6);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}

		private static void EnqueueBuffSequence()
		{
			if (!CrescentSupportJob.TryFindKnowledgeCrystal(out IGameObject knowledgeCrystal) || LocalPlayerState.DistanceToObject2DSquared(knowledgeCrystal) > 10f)
			{
				NotifyHelper.ToastError(LanguageManager.Get("OccultCrescentHelper-OthersManager-Notification-CrystalNotFound"));
				return;
			}
			CrescentSupportJob currentJob = CrescentSupportJob.GetCurrentSupportJob();
			List<CrescentSupportJob> list = (from x in CrescentSupportJob.AllJobs
				where x.IsLongTimeStatusUnlocked()
				orderby x.JobType switch
				{
					CrescentSupportJobType.Knight => 0, 
					CrescentSupportJobType.Bard => 1, 
					CrescentSupportJobType.Monk => 3, 
					CrescentSupportJobType.Dancer => 4, 
					_ => 999, 
				}
				select x).ToList();
			list.ForEach(delegate(CrescentSupportJob x)
			{
				StatusManager.ExecuteStatusOff(x.LongTimeStatusID, 3758096384u);
			});
			SupportJobTaskHelper.Abort();
			SupportJobTaskHelper.Enqueue(delegate
			{
				if (!DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4])
				{
					return true;
				}
				OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.Dismount);
				return true;
			});
			if (CrescentSupportJob.Freelancer.IsActionUnlocked(46606u))
			{
				SupportJobTaskHelper.Enqueue(delegate
				{
					if (CrescentSupportJob.Freelancer.IsThisJob())
					{
						return true;
					}
					if (!Throttler.Shared.Throttle("OthersManager-OthersManager-ChangeSupportJob", 750u))
					{
						return false;
					}
					CrescentSupportJob.Freelancer.ChangeTo();
					return false;
				});
				SupportJobTaskHelper.Enqueue(() => OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)1, 46606u, 3758096384uL, 0u, (UseActionMode)0));
			}
			else
			{
				foreach (CrescentSupportJob sJob in list)
				{
					SupportJobTaskHelper.Enqueue(delegate
					{
						if (sJob.IsThisJob())
						{
							return true;
						}
						if (!Throttler.Shared.Throttle("OthersManager-OthersManager-ChangeSupportJob", 750u))
						{
							return false;
						}
						sJob.ChangeTo();
						return false;
					});
					SupportJobTaskHelper.Enqueue(delegate
					{
						if (sJob.IsWithLongTimeStatus())
						{
							return true;
						}
						OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)1, sJob.LongTimeStatusActionID, 3758096384uL, 0u, (UseActionMode)0);
						return false;
					});
				}
			}
			SupportJobTaskHelper.Enqueue(delegate
			{
				if (currentJob.IsThisJob())
				{
					return true;
				}
				if (!Throttler.Shared.Throttle("OthersManager-OthersManager-ChangeSupportJob", 750u))
				{
					return false;
				}
				currentJob.ChangeTo();
				return false;
			});
		}

		private unsafe static void EnqueueActionInsertion(ActionInsertionRule insertionRule)
		{
			ActionInsertionTaskHelper.Abort();
			ActionManager* manager = ActionManager.Instance();
			if (manager == null)
			{
				return;
			}
			ActionInsertionTaskHelper.DelayNext((int)MathF.Max(100f, ((ActionManager)manager).AnimationLock * 1000f), "等待动画锁结束");
			ActionInsertionTaskHelper.Enqueue(delegate
			{
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Invalid comparison between Unknown and I4
				if (((ActionManager)manager).QueuedActionId != 0)
				{
					Action item;
					if ((int)((ActionManager)manager).QueuedActionType != 1)
					{
						ActionInsertionTaskHelper.Abort();
					}
					else if (!LuminaGetter.TryGetRow<Action>(((ActionManager)manager).QueuedActionId, out item) || ((Action)(ref item)).Recast100ms == 0)
					{
						ActionInsertionTaskHelper.Abort();
					}
				}
			}, "检查当前状态是否合法");
			ActionInsertionTaskHelper.Enqueue(delegate
			{
				IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
				if (localPlayer == null || !IsActionInsertionReady(insertionRule, localPlayer, manager))
				{
					ActionInsertionTaskHelper.Abort();
					return true;
				}
				return OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)1, insertionRule.ActionID, 3758096384uL, 0u, (UseActionMode)0);
			}, $"UseAction_{insertionRule.ActionID}");
		}

		private void OnPreUseAction(ref bool isPrevented, ref ActionType actionType, ref uint actionID, ref ulong targetID, ref uint extraParam, ref UseActionMode queueState, ref uint comboRouteID)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			if ((int)GameState.TerritoryIntendedUse == 61 && MainModule.config.IsEnabledBerserkerRageAutoFace && (int)actionType == 1 && actionID == 41592)
			{
				if (TargetManager.Target == null)
				{
					OmenServiceBase<ChatManager>.Instance().SendMessage("/tenemy");
				}
				OmenServiceBase<ChatManager>.Instance().SendMessage("/facetarget");
			}
		}

		private static void OnPostUseAction(bool result, ActionType actionType, uint actionID, ulong targetID, Vector3 location, uint extraParam, byte a7)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Invalid comparison between Unknown and I4
			if ((int)GameState.TerritoryIntendedUse == 61 && result && ((int)actionType != 1 || !LuminaGetter.TryGetRow<Action>(actionID, out Action item) || ((Action)(ref item)).Recast100ms == 0))
			{
				ActionInsertionTaskHelper.Abort();
			}
		}

		private void OnPreCompleteCast(ref bool isPrevented, ref IBattleChara battleChara, ref ActionType actionType, ref uint actionID, ref uint spellID, ref GameObjectId animationTargetID, ref Vector3 position, ref float f, ref short s, ref int i, ref int ballistaEntityID)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			if ((int)GameState.TerritoryIntendedUse == 61 && battleChara.EntityID == LocalPlayerState.EntityID)
			{
				if (MainModule.config.IsEnabledMonkKickNoMove && (int)actionType == 1 && actionID == 41595)
				{
					actionID = (spellID = 7u);
				}
				if (MainModule.config.IsEnabledBerserkerRageReplace && (int)actionType == 1 && actionID == 41593)
				{
					actionID = (spellID = 3549u);
				}
			}
		}

		private unsafe void OnPostCompleteCast(bool result, IBattleChara player, ActionType actionType, uint actionID, uint spellID, GameObjectId animationTargetID, Vector3 location, float rotation, short lastUsedActionSequence, int animationVariation, int ballistaEntityID)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Invalid comparison between Unknown and I4
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Invalid comparison between Unknown and I4
			if ((int)GameState.TerritoryIntendedUse != 61 || !result || !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)26] || player.EntityID != LocalPlayerState.EntityID || !LuminaGetter.TryGetRow<Action>(actionID, out Action item) || ((Action)(ref item)).Recast100ms == 0)
			{
				return;
			}
			ActionManager* manager = ActionManager.Instance();
			if (manager == null)
			{
				return;
			}
			ActionInsertionRule actionInsertionRule = actionInsertionRules.FirstOrDefault((ActionInsertionRule x) => IsActionInsertionReady(x, player, manager));
			if (actionInsertionRule == null)
			{
				return;
			}
			int recastGroup = ((ActionManager)manager).GetRecastGroup(1, actionID);
			RecastDetail* ptr = ((recastGroup == -1) ? null : ((ActionManager)manager).GetRecastGroupDetail(recastGroup));
			int additionalRecastGroup = ((ActionManager)manager).GetAdditionalRecastGroup((ActionType)1, actionID);
			RecastDetail* ptr2 = ((additionalRecastGroup == -1) ? null : ((ActionManager)manager).GetRecastGroupDetail(additionalRecastGroup));
			if (ptr != null)
			{
				if (!((RecastDetail)ptr).IsActive || (((RecastDetail)ptr).Total - ((RecastDetail)ptr).Elapsed) * 1000f < 500f)
				{
					return;
				}
			}
			else if (ptr2 != null && (!((RecastDetail)ptr2).IsActive || (((RecastDetail)ptr2).Total - ((RecastDetail)ptr2).Elapsed) * 1000f < 500f))
			{
				return;
			}
			if (((ActionManager)manager).QueuedActionId == 0 || ((int)((ActionManager)manager).QueuedActionType == 1 && LuminaGetter.TryGetRow<Action>(((ActionManager)manager).QueuedActionId, out Action item2) && ((Action)(ref item2)).Recast100ms != 0))
			{
				EnqueueActionInsertion(actionInsertionRule);
			}
		}

		private static void OnCommandBuff(string command, string args)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			if ((int)GameState.TerritoryIntendedUse == 61)
			{
				EnqueueBuffSequence();
			}
		}

		private unsafe static bool IsActionInsertionReady(ActionInsertionRule insertionRule, IBattleChara player, ActionManager* manager)
		{
			if (insertionRule.EnabledCondition() && insertionRule.SupportJob.IsThisJob() && insertionRule.SupportJob.IsActionUnlocked(insertionRule.ActionID) && insertionRule.UseCondition(player))
			{
				return ((ActionManager)manager).IsActionOffCooldown((ActionType)1, insertionRule.ActionID);
			}
			return false;
		}

		private unsafe static bool ShouldUseOffensiveAria(IBattleChara player)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			StatusManager statusManager = ((BattleChara)player.ToStruct()).StatusManager;
			int statusIndex = ((StatusManager)(ref statusManager)).GetStatusIndex(4247u, 3758096384u);
			if (statusIndex != -1)
			{
				return ((StatusManager)(ref statusManager)).GetRemainingTime(statusIndex) <= 20f;
			}
			return true;
		}
	}

	private class TreasureManager(OccultCrescentHelper mainModule) : BaseIslandModule(mainModule)
	{
		private unsafe delegate nint CalculateCollisionDelegate(nint moveControlInstance, Vector3* expectedPosition, nint controlState, Vector3* currentPosition, nint collisionFlags, ushort movementType);

		private static class PathPlanner
		{
			public static Queue<Vector3> PlanShortestPath(Vector3 currentPosition, List<Vector3> locations)
			{
				if (locations == null || locations.Count <= 0)
				{
					return new Queue<Vector3>();
				}
				Vector3 item = new Vector3(currentPosition.X, currentPosition.Y, currentPosition.Z);
				List<Vector3> list = new List<Vector3>();
				list.Add(item);
				list.AddRange(locations);
				List<Vector3> list2 = CreateInitialPathNearestNeighbor(list);
				OptimizePath2Opt(list2);
				list2.RemoveAt(0);
				return new Queue<Vector3>(list2);
			}

			private static List<Vector3> CreateInitialPathNearestNeighbor(List<Vector3> points)
			{
				List<Vector3> list = new List<Vector3>(points);
				List<Vector3> list2 = new List<Vector3>();
				Vector3 vector = list[0];
				list2.Add(vector);
				list.RemoveAt(0);
				while (list.Count > 0)
				{
					Vector3? vector2 = null;
					float num = float.MaxValue;
					foreach (Vector3 item in list)
					{
						float num2 = Vector3.DistanceSquared(vector, item);
						if (num2 < num)
						{
							num = num2;
							vector2 = item;
						}
					}
					if (vector2.HasValue)
					{
						list2.Add(vector2.Value);
						list.Remove(vector2.Value);
						vector = vector2.Value;
					}
				}
				return list2;
			}

			private static void OptimizePath2Opt(List<Vector3> path)
			{
				bool flag = true;
				int count = path.Count;
				while (flag)
				{
					flag = false;
					for (int i = 0; i < count - 2; i++)
					{
						for (int j = i + 2; j < count - 1; j++)
						{
							Vector3 value = path[i];
							Vector3 vector = path[i + 1];
							Vector3 vector2 = path[j];
							Vector3 value2 = path[j + 1];
							float num = Vector3.DistanceSquared(value, vector) + Vector3.DistanceSquared(vector2, value2);
							if (Vector3.DistanceSquared(value, vector2) + Vector3.DistanceSquared(vector, value2) < num)
							{
								path.Reverse(i + 1, j - i);
								flag = true;
							}
						}
					}
				}
			}
		}

		private record Route(uint TerritoryType, string Name, string? Description, List<Vector3> Points);

		private class RoutePreview
		{
			private Route? route;

			private Vector2 plannedPosition;

			private int drawnFrame;

			private long lastUpdateTick;

			public List<Vector3>? Points { get; private set; }

			public void Update(Route? hoveredRoute)
			{
				drawnFrame = ImGui.GetFrameCount();
				if ((object)hoveredRoute == null)
				{
					route = null;
					Points = null;
					return;
				}
				if (Points == null)
				{
					Plan(hoveredRoute);
					return;
				}
				long tickCount = Environment.TickCount64;
				if (tickCount - lastUpdateTick >= 500)
				{
					lastUpdateTick = tickCount;
					if ((object)route != hoveredRoute || !(LocalPlayerState.DistanceTo2DSquared(plannedPosition) <= 2500f))
					{
						Plan(hoveredRoute);
					}
				}
			}

			private void Plan(Route target)
			{
				Vector3 vector = LocalPlayerState.Object.Position;
				route = target;
				plannedPosition = vector.ToVector2();
				Queue<Vector3> queue = PathPlanner.PlanShortestPath(vector, target.Points);
				int count = queue.Count;
				List<Vector3> list = new List<Vector3>(count);
				CollectionsMarshal.SetCount(list, count);
				Span<Vector3> span = CollectionsMarshal.AsSpan(list);
				int num = 0;
				foreach (Vector3 item in queue)
				{
					span[num] = item;
					num++;
				}
				Points = list;
				lastUpdateTick = Environment.TickCount64;
			}

			public void ResetIfNotDrawnThisFrame()
			{
				if (ImGui.GetFrameCount() - drawnFrame > 0)
				{
					Points = null;
				}
			}
		}

		private TaskHelper? treasureTaskHelper;

		private Queue<Vector3> queuedGatheringList = new Queue<Vector3>();

		private List<(nint Ptr, Vector3 Position)> treasureObjects = new List<(nint, Vector3)>();

		private List<Vector3> surveyPointPositions = new List<Vector3>();

		private List<Vector3> carrotPositions = new List<Vector3>();

		private Vector3 origPosition;

		private List<Vector3> currentRoute = new List<Vector3>();

		private readonly RoutePreview routePreview = new RoutePreview();

		private readonly ImGuiMapRenderer routeMapRenderer = new ImGuiMapRenderer
		{
			Zoomable = false,
			Pannable = false,
			EnableResizeGrip = false,
			EnableDefaultMarkers = true
		};

		private ZoneIndicatorHandle? treasureHandle;

		private ZoneIndicatorHandle? surveyPointHandle;

		private ZoneIndicatorHandle? carrotHandle;

		private static readonly CompSig CalculateCollisionSig = new CompSig("48 89 74 24 ?? 48 89 7C 24 ?? 55 41 56 41 57 48 8D AC 24 ?? ?? ?? ?? 48 81 EC ?? ?? ?? ?? F3 0F 10 42");

		private Hook<CalculateCollisionDelegate>? calculateCollisionHook;

		private const ImGuiWindowFlags WINDOW_FLAGS = (ImGuiWindowFlags)2896863;

		private const string COMMAND_TREASURE = "ptreasure";

		private static readonly uint LineColorBlue = KnownColor.Chocolate.ToVector4().ToUInt();

		private static readonly uint DotColor = KnownColor.IndianRed.ToVector4().ToUInt();

		private static readonly uint PlayerColor = KnownColor.Orange.ToVector4().ToUInt();

		private static readonly Route[] Routes;

		public unsafe override void Init()
		{
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Expected O, but got Unknown
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Expected O, but got Unknown
			if (calculateCollisionHook == null)
			{
				calculateCollisionHook = CalculateCollisionSig.GetHook<CalculateCollisionDelegate>(CalculateCollisionDetour);
			}
			if (treasureTaskHelper == null)
			{
				treasureTaskHelper = new TaskHelper
				{
					TimeoutMS = 180000,
					EnterBusyAction = delegate
					{
						calculateCollisionHook?.Enable();
					},
					LeaveBusyAction = delegate
					{
						calculateCollisionHook?.Disable();
					}
				};
			}
			OmenServiceBase<WindowManager>.Instance().PostDraw += OnPosDraw;
			DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += OnZoneChanged;
			OnZoneChanged(0u);
			OmenServiceBase<GamePacketManager>.Instance().RegPreSendPacket(OnPreSendPacket);
			OmenServiceBase<CommandManager>.Instance().AddSubCommand("ptreasure", new CommandInfo(new HandlerDelegate(OnCommandTreasure))
			{
				HelpMessage = (LanguageManager.Get("OccultCrescentHelper-Command-PTreasure-Help") ?? "")
			});
		}

		public override void Uninit()
		{
			OmenServiceBase<CommandManager>.Instance().RemoveSubCommand("ptreasure");
			OmenServiceBase<GamePacketManager>.Instance().Unreg(OnPreSendPacket);
			DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChanged;
			OmenServiceBase<WindowManager>.Instance().PostDraw -= OnPosDraw;
			treasureHandle?.Unreg();
			treasureHandle = null;
			treasureTaskHelper?.Abort();
			treasureTaskHelper?.Dispose();
			treasureTaskHelper = null;
			calculateCollisionHook?.Disable();
			calculateCollisionHook?.Dispose();
			calculateCollisionHook = null;
			treasureObjects.Clear();
		}

		public override void DrawConfig()
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0451: Unknown result type (might be due to invalid IL or missing references)
			//IL_0456: Unknown result type (might be due to invalid IL or missing references)
			//IL_045b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_045d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0464: Invalid comparison between Unknown and I4
			//IL_0470: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e8: Invalid comparison between Unknown and I4
			//IL_048d: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0510: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_0344: Unknown result type (might be due to invalid IL or missing references)
			//IL_0535: Unknown result type (might be due to invalid IL or missing references)
			//IL_053c: Invalid comparison between Unknown and I4
			//IL_0358: Unknown result type (might be due to invalid IL or missing references)
			//IL_035d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0370: Unknown result type (might be due to invalid IL or missing references)
			//IL_067e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0557: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0692: Unknown result type (might be due to invalid IL or missing references)
			//IL_0697: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0402: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e3: Unknown result type (might be due to invalid IL or missing references)
			TabBarDisposable val = ImRaii.TabBar(ImU8String.op_Implicit("TabBar"));
			try
			{
				if (!val)
				{
					return;
				}
				TabItemDisposable val2 = ImRaii.TabItem(ImU8String.op_Implicit(LanguageManager.Get("General")));
				try
				{
					if (TabItemDisposable.op_Implicit(val2))
					{
						ImGui.TextColored(KnownColor.Chocolate.ToUInt(), ImU8String.op_Implicit(LanguageManager.Get("OccultCrescentHelper-TreasureManager-AutoOpenTreasure")));
						IndentDisposable val3 = ImRaii.PushIndent(1, true);
						try
						{
							ImU8String val4 = new ImU8String(19, 1);
							((ImU8String)(ref val4)).AppendFormatted<string>(LanguageManager.Get("Enable"));
							((ImU8String)(ref val4)).AppendLiteral("##AutoOpenTreasures");
							if (ImGui.Checkbox(val4, ref MainModule.config.IsEnabledAutoOpenTreasure))
							{
								MainModule.config.Save(MainModule);
							}
							ImGuiOm.HelpMarker(LanguageManager.Get("OccultCrescentHelper-TreasureManager-AutoOpenTreasure-Help"), 20f * Globals.GlobalUIScale, (FontAwesomeIcon)61530);
							if (MainModule.config.IsEnabledAutoOpenTreasure)
							{
								ImGui.SetNextItemWidth(150f * Globals.GlobalUIScale);
								ImU8String val5 = new ImU8String(0, 1);
								((ImU8String)(ref val5)).AppendFormatted<string>(LanguageManager.Get("OccultCrescentHelper-DistanceTo"));
								ImGui.SliderFloat(val5, ref MainModule.config.DistanceToAutoOpenTreasure, 1f, 50f, ImU8String.op_Implicit("%.1f"), (ImGuiSliderFlags)0);
								if (ImGui.IsItemDeactivatedAfterEdit())
								{
									MainModule.config.Save(MainModule);
								}
								ImGuiOm.HelpMarker(LanguageManager.Get("OccultCrescentHelper-TreasureManager-AutoOpenTreasure-DistanceTo-Help") ?? "", 20f * Globals.GlobalUIScale, (FontAwesomeIcon)61530);
							}
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
						if (origPosition != default(Vector3) || treasureObjects.Count > 0)
						{
							ImU8String val6 = new ImU8String(25, 1);
							((ImU8String)(ref val6)).AppendFormatted<string>(LuminaWrapper.GetAddonText(395u));
							((ImU8String)(ref val6)).AppendLiteral(" [999.99, 999.99, 999.99]");
							Vector2 vector = ImGui.CalcTextSize(val6, false, -1f);
							ImGui.NewLine();
							ImGui.TextColored(KnownColor.Chocolate.ToUInt(), ImU8String.op_Implicit(LuminaWrapper.GetAddonText(395u)));
							if (origPosition != default(Vector3))
							{
								ImGui.SameLine();
								if (ImGui.SmallButton(ImU8String.op_Implicit(LanguageManager.Get("OccultCrescentHelper-TreasureManager-ReturnToOrigPostion"))))
								{
									EnqueueMoveTo(origPosition);
								}
							}
							val3 = ImRaii.PushIndent(1, true);
							try
							{
								foreach (var treasureObject in treasureObjects)
								{
									Vector3 item = treasureObject.Position;
									ImU8String val7 = new ImU8String(4, 3);
									((ImU8String)(ref val7)).AppendFormatted<float>(item.X, "F1");
									((ImU8String)(ref val7)).AppendLiteral(", ");
									((ImU8String)(ref val7)).AppendFormatted<float>(item.Y, "F1");
									((ImU8String)(ref val7)).AppendLiteral(", ");
									((ImU8String)(ref val7)).AppendFormatted<float>(item.Z, "F1");
									if (ImGui.Button(val7, new Vector2(vector.X * 2f, ImGui.GetFrameHeight())))
									{
										origPosition = LocalPlayerState.Object.Position;
										EnqueueMoveTo(item);
									}
								}
							}
							finally
							{
								((IDisposable)val3)?.Dispose();
							}
						}
						ImGui.NewLine();
						ImGui.TextColored(KnownColor.Chocolate.ToUInt(), ImU8String.op_Implicit(LanguageManager.Get("OccultCrescentHelper-Highlight")));
						val3 = ImRaii.PushIndent(1, true);
						try
						{
							ImU8String val8 = new ImU8String(0, 1);
							((ImU8String)(ref val8)).AppendFormatted<string>(LuminaWrapper.GetAddonText(395u));
							if (ImGui.Checkbox(val8, ref MainModule.config.IsEnabledHighlightTreasure))
							{
								MainModule.config.Save(MainModule);
							}
							ImU8String val9 = new ImU8String(0, 1);
							((ImU8String)(ref val9)).AppendFormatted<string>(LuminaWrapper.GetEObjName(2014695u));
							if (ImGui.Checkbox(val9, ref MainModule.config.IsEnabledHighlightSurveyPoint))
							{
								MainModule.config.Save(MainModule);
							}
							ImU8String val10 = new ImU8String(0, 1);
							((ImU8String)(ref val10)).AppendFormatted<string>(LuminaWrapper.GetItemName(48096u));
							if (ImGui.Checkbox(val10, ref MainModule.config.IsEnabledHighlightCarrot))
							{
								MainModule.config.Save(MainModule);
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
					((TabItemDisposable)(ref val2)).Dispose();
				}
				TabItemDisposable val11 = ImRaii.TabItem(ImU8String.op_Implicit(LanguageManager.Get("OccultCrescentHelper-TreasureManager-AutoHuntTresures")));
				try
				{
					DisabledDisposable val12 = ImRaii.Disabled((int)GameState.TerritoryIntendedUse != 61);
					try
					{
						if (TabItemDisposable.op_Implicit(val11))
						{
							ImGui.TextColored(KnownColor.Chocolate.ToUInt(), ImU8String.op_Implicit(LanguageManager.Get("State")));
							IndentDisposable val3 = ImRaii.PushIndent(1, true);
							try
							{
								ImU8String val13 = new ImU8String(2, 2);
								((ImU8String)(ref val13)).AppendFormatted<string>(LanguageManager.Get("OccultCrescentHelper-TreasureManager-AutoHuntTresures-LeftPoints"));
								((ImU8String)(ref val13)).AppendLiteral(": ");
								((ImU8String)(ref val13)).AppendFormatted<int>(queuedGatheringList.Count);
								ImGui.TextWrapped(val13);
								ImU8String val14 = new ImU8String(8, 1);
								((ImU8String)(ref val14)).AppendLiteral("    ");
								((ImU8String)(ref val14)).AppendFormatted<string>(LanguageManager.Get("Stop"));
								((ImU8String)(ref val14)).AppendLiteral("    ");
								if (ImGui.Button(val14, default(Vector2)))
								{
									StopAutoTreasureHunt();
								}
							}
							finally
							{
								((IDisposable)val3)?.Dispose();
							}
							if ((int)GameState.TerritoryIntendedUse == 61)
							{
								ImGui.NewLine();
								ImGui.TextColored(KnownColor.Chocolate.ToUInt(), ImU8String.op_Implicit(LanguageManager.Get("Route")));
								val3 = ImRaii.PushIndent(1, true);
								try
								{
									DisabledDisposable val15 = ImRaii.Disabled(treasureTaskHelper.IsBusy);
									try
									{
										Route hoveredRoute = null;
										bool flag = true;
										Route[] routes = Routes;
										foreach (Route route in routes)
										{
											if (route.TerritoryType == GameState.TerritoryType)
											{
												if (!flag)
												{
													ImGui.SameLine();
												}
												flag = false;
												ImU8String val16 = new ImU8String(8, 1);
												((ImU8String)(ref val16)).AppendLiteral("    ");
												((ImU8String)(ref val16)).AppendFormatted<string>(route.Name);
												((ImU8String)(ref val16)).AppendLiteral("    ");
												if (ImGui.Button(val16, default(Vector2)))
												{
													EnqueueAutoTreasureHunt(route.Points);
												}
												if (route.Description != null)
												{
													ImGuiOm.TooltipHover(route.Description, 20f * Globals.GlobalUIScale);
												}
												if (ImGui.IsItemHovered())
												{
													hoveredRoute = route;
												}
											}
										}
										routePreview.Update(hoveredRoute);
									}
									finally
									{
										((IDisposable)val15)?.Dispose();
									}
								}
								finally
								{
									((IDisposable)val3)?.Dispose();
								}
							}
							ImGui.NewLine();
							Vector4 vector2 = KnownColor.Chocolate.ToVector4();
							ImGui.TextColored(ref vector2, ImU8String.op_Implicit(LanguageManager.Get("Command")));
							val3 = ImRaii.PushIndent(1, true);
							try
							{
								ImU8String val17 = new ImU8String(6, 2);
								((ImU8String)(ref val17)).AppendLiteral("/pdr ");
								((ImU8String)(ref val17)).AppendFormatted<string>("ptreasure");
								((ImU8String)(ref val17)).AppendLiteral(" ");
								((ImU8String)(ref val17)).AppendFormatted<string>(LanguageManager.Get("OccultCrescentHelper-Command-PTreasure-Help"));
								ImGui.TextWrapped(val17);
								return;
							}
							finally
							{
								((IDisposable)val3)?.Dispose();
							}
						}
						if ((int)GameState.TerritoryIntendedUse == 61)
						{
							ImGuiOm.TooltipHover(LanguageManager.Get("OccultCrescentHelper-TreasureManager-AutoHuntTresures-Help"), 20f * Globals.GlobalUIScale);
						}
					}
					finally
					{
						((IDisposable)val12)?.Dispose();
					}
				}
				finally
				{
					((TabItemDisposable)(ref val11)).Dispose();
				}
			}
			finally
			{
				((TabBarDisposable)(ref val)).Dispose();
			}
		}

		private void DrawTreasureRouteMap(List<Vector3> route)
		{
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			uint map = GameState.Map;
			if (map == 0 || route.Count == 0)
			{
				return;
			}
			Vector2 vector = Globals.ScaledVector2(400f);
			ImGui.SetNextWindowSize(vector + Globals.ScaledVector2(20f, 40f));
			ImGuiViewportPtr mainViewport = ImGui.GetMainViewport();
			ImGui.SetNextWindowPos(((ImGuiViewportPtr)(ref mainViewport)).WorkPos + Globals.ScaledVector2(16f, 16f));
			ImGui.SetNextWindowBgAlpha(0.8f);
			if (ImGui.Begin(ImU8String.op_Implicit("###AutoTreasureHuntMap"), (ImGuiWindowFlags)2896863))
			{
				routeMapRenderer.SetMap(map);
				routeMapRenderer.OnCustomMapDraw = delegate(ImGuiMapRenderer r, ImDrawListPtr drawList)
				{
					if (route.Count > 1)
					{
						for (int i = 0; i < route.Count - 1; i++)
						{
							Vector2 vector2 = r.WorldToScreen(route[i]);
							Vector2 vector3 = r.WorldToScreen(route[i + 1]);
							((ImDrawListPtr)(ref drawList)).AddLine(vector2, vector3, 1711276032u, 7f);
							((ImDrawListPtr)(ref drawList)).AddLine(vector2, vector3, LineColorBlue, 5f);
						}
					}
				};
				routeMapRenderer.OnCustomForegroundDraw = delegate(ImGuiMapRenderer r, ImDrawListPtr drawList)
				{
					IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
					if (localPlayer != null)
					{
						Vector2 vector2 = r.WorldToScreen(localPlayer.Position);
						Vector2 vector3 = new Vector2(MathF.Sin(localPlayer.Rotation), MathF.Cos(localPlayer.Rotation));
						Vector2 vector4 = new Vector2(0f - vector3.Y, vector3.X);
						Vector2 vector5 = vector2 + vector3 * 10f;
						Vector2 vector6 = vector2 - vector3 * 7f;
						((ImDrawListPtr)(ref drawList)).AddTriangleFilled(vector5, vector6 + vector4 * 7f, vector6 - vector4 * 7f, PlayerColor);
						((ImDrawListPtr)(ref drawList)).AddTriangle(vector5, vector6 + vector4 * 7f, vector6 - vector4 * 7f, 4278190080u, 2f);
					}
				};
				routeMapRenderer.ClearMarkers();
				for (int num = 0; num < route.Count; num++)
				{
					routeMapRenderer.AddMarker(new ImGuiMapMarker
					{
						ID = $"TreasureRoute_{num}",
						Position = route[num],
						Color = DotColor,
						Size = new Vector2(16f),
						ShowLabel = false,
						ShowTooltip = false
					});
				}
				routeMapRenderer.Draw(vector);
			}
			ImGui.End();
		}

		private unsafe static nint CalculateCollisionDetour(nint moveControlInstance, Vector3* expectedPosition, nint controlState, Vector3* currentPosition, nint collisionFlags, ushort movementType)
		{
			return IntPtr.Zero;
		}

		private void OnCommandTreasure(string command, string args)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Invalid comparison between Unknown and I4
			if ((int)GameState.TerritoryIntendedUse != 61)
			{
				return;
			}
			args = args.Trim().ToLowerInvariant();
			if (string.IsNullOrEmpty(args))
			{
				return;
			}
			if (args == "abort")
			{
				StopAutoTreasureHunt();
				return;
			}
			Route route = (from x in Routes
				where x.TerritoryType == GameState.TerritoryType && x.Name.Contains(args, StringComparison.OrdinalIgnoreCase)
				orderby x.Name.Length
				select x).FirstOrDefault();
			if ((object)route != null)
			{
				EnqueueAutoTreasureHunt(route.Points);
			}
		}

		private void OnPreSendPacket(ref bool isPrevented, int opcode, ref nint packet, ref bool isPrioritize)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Invalid comparison between Unknown and I4
			if (opcode == UpstreamOpcode.PositionUpdateInstanceOpcode && (int)GameState.TerritoryIntendedUse == 61 && treasureTaskHelper.IsBusy)
			{
				isPrevented = true;
			}
		}

		private void OnZoneChanged(uint u)
		{
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Invalid comparison between Unknown and I4
			currentRoute.Clear();
			queuedGatheringList.Clear();
			treasureObjects.Clear();
			surveyPointPositions.Clear();
			carrotPositions.Clear();
			treasureHandle?.Unreg();
			treasureHandle = null;
			surveyPointHandle?.Unreg();
			surveyPointHandle = null;
			carrotHandle?.Unreg();
			carrotHandle = null;
			if ((int)GameState.TerritoryIntendedUse == 61)
			{
				treasureHandle = OmenServiceBase<ZoneIndicatorRenderer>.Instance().RegTemporary<(nint, Vector3)>(() => (List<(nint Ptr, Vector3 Position)>)((!MainModule.config.IsEnabledHighlightTreasure) ? ((IList)new List<(nint, Vector3)>()) : ((IList)treasureObjects)), ((nint Ptr, Vector3 Position) x) => x.Position, new ZoneIndicatorOptions<(nint, Vector3)>
				{
					TextGetter = ((nint Ptr, Vector3 Position) _) => new ZoneIndicatorText
					{
						Text = ReadOnlySeString.op_Implicit(LuminaWrapper.GetAddonText(395u)),
						TextScale = 1.4f
					}
				});
				surveyPointHandle = OmenServiceBase<ZoneIndicatorRenderer>.Instance().RegTemporary(() => (!MainModule.config.IsEnabledHighlightSurveyPoint) ? new List<Vector3>() : surveyPointPositions, (Vector3 pos) => pos, new ZoneIndicatorOptions<Vector3>
				{
					TextGetter = (Vector3 _) => new ZoneIndicatorText
					{
						Text = ReadOnlySeString.op_Implicit(LuminaWrapper.GetEObjName(2014695u)),
						TextScale = 1.4f
					}
				});
				carrotHandle = OmenServiceBase<ZoneIndicatorRenderer>.Instance().RegTemporary(() => (!MainModule.config.IsEnabledHighlightCarrot) ? new List<Vector3>() : carrotPositions, (Vector3 pos) => pos, new ZoneIndicatorOptions<Vector3>
				{
					TextGetter = (Vector3 _) => new ZoneIndicatorText
					{
						Text = ReadOnlySeString.op_Implicit(LuminaWrapper.GetItemName(48096u)),
						TextScale = 1.4f
					}
				});
			}
		}

		public override void OnUpdate()
		{
			RefreshSpecialObjectsAround();
			HandleAutoOpenTreasures();
		}

		private void OnPosDraw()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			if ((int)GameState.TerritoryIntendedUse == 61)
			{
				routePreview.ResetIfNotDrawnThisFrame();
				List<Vector3> points = routePreview.Points;
				if (points != null)
				{
					DrawTreasureRouteMap(points);
				}
				else if (treasureTaskHelper.IsBusy)
				{
					DrawTreasureRouteMap(currentRoute);
				}
			}
		}

		private void EnqueueAutoTreasureHunt(List<Vector3> routeData)
		{
			treasureTaskHelper.Abort();
			queuedGatheringList.Clear();
			if (LocalPlayerState.DistanceTo2DSquared(((GameState.TerritoryType == 1252) ? CrescentAetheryte.ExpeditionBaseCamp.Position : CrescentAetheryte.NorthHornBaseCamp.Position).ToVector2()) <= 2500f)
			{
				OmenServiceBase<NotifyHelper>.Instance().NotificationError(LanguageManager.Get("OccultCrescentHelper-TreasureManager-AutoOpenTreasure-Notification-Danger"));
				return;
			}
			queuedGatheringList = PathPlanner.PlanShortestPath(LocalPlayerState.Object.Position, routeData);
			Queue<Vector3> queue = queuedGatheringList;
			int count = queue.Count;
			List<Vector3> list = new List<Vector3>(count);
			CollectionsMarshal.SetCount(list, count);
			Span<Vector3> span = CollectionsMarshal.AsSpan(list);
			int num = 0;
			foreach (Vector3 item in queue)
			{
				span[num] = item;
				num++;
			}
			currentRoute = list;
			MoveToNextTreasurePoint();
		}

		private unsafe void EnqueueMoveTo(Vector3 position)
		{
			treasureTaskHelper.EnqueueAsync(async delegate(CancellationToken ct)
			{
				PlayerController.Instance()->MoveControllerWalk.IsMovementInputLocked = true;
				await OmenServiceBase<MovementManager>.Instance().TPSmoothAsync(position, DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4] ? 24 : 12, ct);
				if (!Throttler.Shared.Throttle("OccultCrescentHelper-TreasureManager-Pathfind-Check"))
				{
					return false;
				}
				if (LocalPlayerState.DistanceTo2DSquared(position.ToVector2()) >= 9f)
				{
					return false;
				}
				OnUpdate();
				PlayerController.Instance()->MoveControllerWalk.IsMovementInputLocked = false;
				return true;
			});
		}

		private unsafe void StopAutoTreasureHunt()
		{
			treasureTaskHelper.Abort();
			queuedGatheringList.Clear();
			currentRoute.Clear();
			PlayerController.Instance()->MoveControllerWalk.IsMovementInputLocked = false;
		}

		private unsafe void MoveToNextTreasurePoint()
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Invalid comparison between Unknown and I4
			if ((int)GameState.TerritoryIntendedUse != 61 || !GameState.IsLoggedIn)
			{
				StopAutoTreasureHunt();
				return;
			}
			if (queuedGatheringList.Count == 0)
			{
				Vector3 currentPosition = LocalPlayerState.Object.Position;
				treasureTaskHelper.Enqueue(delegate
				{
					if (LocalPlayerState.DistanceTo2DSquared(currentPosition.ToVector2()) >= 2500f)
					{
						return true;
					}
					if (!UIModuleExtension.IsScreenReady())
					{
						return false;
					}
					OmenServiceBase<MovementManager>.Instance().TPSmooth(currentPosition, 24f);
					if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4])
					{
						MountCommand.Dismount();
						return false;
					}
					if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)27])
					{
						return false;
					}
					if (((ActionManager)ActionManager.Instance()).GetActionStatus((ActionType)1, 41343u, 3758096384uL, true, true, (uint*)null) != 0)
					{
						return false;
					}
					if (Throttler.Shared.Throttle("OccultCrescentHelper.TreasureManager.DemiReturn"))
					{
						OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)1, 41343u, 3758096384uL, 0u, (UseActionMode)0);
					}
					return false;
				});
				treasureTaskHelper.Enqueue(delegate
				{
					PlayerController.Instance()->MoveControllerWalk.IsMovementInputLocked = false;
					currentRoute.Clear();
					string message = LanguageManager.Get("OccultCrescentHelper-TreasureManager-AutoOpenTreasure-Notification-End");
					OmenServiceBase<NotifyHelper>.Instance().NotificationInfo(message);
					NotifyHelper.Speak(message);
				});
				return;
			}
			Vector3 origPosition = queuedGatheringList.Dequeue();
			Vector3 position = origPosition;
			treasureTaskHelper.Enqueue(delegate
			{
				if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4])
				{
					return true;
				}
				if (!Throttler.Shared.Throttle("OccultCrescentHelper.TreasureManager.UseMount"))
				{
					return false;
				}
				if (IConditionExtension.get_IsCasting(DalamudServiceExtension.Instance<ICondition>()))
				{
					return false;
				}
				OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)5, 9u, 3758096384uL, 0u, (UseActionMode)0);
				return false;
			});
			treasureTaskHelper.Enqueue(delegate
			{
				PlayerController.Instance()->MoveControllerWalk.IsMovementInputLocked = true;
				OmenServiceBase<MovementManager>.Instance().TPSmooth(position, 24f);
				if (!Throttler.Shared.Throttle("OccultCrescentHelper.TreasureManager.Pathfind.Check", 100u))
				{
					return false;
				}
				if (LocalPlayerState.DistanceTo2DSquared(position.ToVector2()) >= 2500f)
				{
					return false;
				}
				OnUpdate();
				using (List<(nint, Vector3)>.Enumerator enumerator = treasureObjects.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						Vector3 item = enumerator.Current.Item2;
						position = item;
						position.Y = origPosition.Y;
						return false;
					}
				}
				return true;
			});
			treasureTaskHelper.Enqueue((Action)MoveToNextTreasurePoint, "下一轮开始", 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
		}

		private unsafe void HandleAutoOpenTreasures()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			if ((int)GameState.TerritoryIntendedUse != 61 || !MainModule.config.IsEnabledAutoOpenTreasure || DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)26])
			{
				return;
			}
			List<(nint, Vector3)> list = treasureObjects;
			if (list == null || list.Count <= 0 || LocalPlayerState.Object == null)
			{
				return;
			}
			List<nint> list2 = (*EventObjectManager.Instance()).FindAll(delegate(nint ptr)
			{
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0010: Invalid comparison between Unknown and I4
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				if (ptr == (nint)0)
				{
					return false;
				}
				if ((int)((Treasure)ptr).ObjectKind != 4)
				{
					return false;
				}
				if (((Treasure)ptr).Flags.IsSetAny<TreasureFlags>(new ReadOnlySpan<TreasureFlags>(Unsafe.AsPointer(ref global::_003CPrivateImplementationDetails_003E.A12871FEE210FB8619291EAEA194581CBD2531E4B23759D225F6806923F63222), 2)))
				{
					return false;
				}
				float num = MainModule.config.DistanceToAutoOpenTreasure * MainModule.config.DistanceToAutoOpenTreasure;
				return !(LocalPlayerState.DistanceTo2DSquared(((Treasure)ptr).Position.ToVector2()) > num);
			});
			if (list2.Count == 0)
			{
				return;
			}
			foreach (nint item in list2)
			{
				InteractWithTreasure((Treasure*)item);
			}
		}

		private unsafe void RefreshSpecialObjectsAround()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Invalid comparison between Unknown and I4
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Invalid comparison between Unknown and I4
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			if ((int)GameState.TerritoryIntendedUse != 61)
			{
				return;
			}
			List<Vector3> list = new List<Vector3>();
			List<Vector3> list2 = new List<Vector3>();
			List<nint> list3 = (*EventObjectManager.Instance()).FindAll(delegate(nint num2)
			{
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0010: Invalid comparison between Unknown and I4
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				if (num2 == (nint)0)
				{
					return false;
				}
				if ((int)((Treasure)num2).ObjectKind != 4)
				{
					return false;
				}
				return !((Treasure)num2).Flags.IsSetAny<TreasureFlags>(new ReadOnlySpan<TreasureFlags>(Unsafe.AsPointer(ref global::_003CPrivateImplementationDetails_003E.A12871FEE210FB8619291EAEA194581CBD2531E4B23759D225F6806923F63222), 2));
			});
			Span<Pointer<GameObject>> eventObjects = ((EventObjectManager)EventObjectManager.Instance()).EventObjects;
			for (int num = 0; num < eventObjects.Length; num++)
			{
				Pointer<GameObject> val = eventObjects[num];
				if (val.IsNull)
				{
					continue;
				}
				GameObject* value = val.Value;
				if (!((GameObject)value).IsReadyToDraw())
				{
					return;
				}
				if ((int)((GameObject)value).ObjectKind == 4)
				{
					Treasure* ptr = (Treasure*)value;
					if (!((Treasure)ptr).Flags.IsSetAny<TreasureFlags>(new ReadOnlySpan<TreasureFlags>(Unsafe.AsPointer(ref global::_003CPrivateImplementationDetails_003E.A12871FEE210FB8619291EAEA194581CBD2531E4B23759D225F6806923F63222), 2)))
					{
						list3.Add((nint)ptr);
					}
				}
			}
			Span<Pointer<EventObject>> eventObjects2 = ((StandObjectManager)StandObjectManager.Instance()).EventObjects;
			for (int num = 0; num < eventObjects2.Length; num++)
			{
				Pointer<EventObject> val2 = eventObjects2[num];
				if (val2.IsNull)
				{
					continue;
				}
				EventObject* value2 = val2.Value;
				if (!((EventObject)value2).IsReadyToDraw())
				{
					return;
				}
				if ((int)((EventObject)value2).ObjectKind == 7)
				{
					switch (((EventObject)value2).BaseId)
					{
					case 2014695u:
						list.Add(Vector3.op_Implicit(((EventObject)value2).Position));
						break;
					case 2010139u:
						list2.Add(Vector3.op_Implicit(((EventObject)value2).Position));
						break;
					}
				}
			}
			treasureObjects = list3.Select((nint x) => (x: x, Vector3.op_Implicit(((GameObject)x).Position))).ToList();
			surveyPointPositions = list;
			carrotPositions = list2;
		}

		private unsafe void InteractWithTreasure(Treasure* treasure)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			IPlayerCharacter playerCharacter = LocalPlayerState.Object;
			if (playerCharacter != null)
			{
				PositionUpdateInstancePacket.MoveType instanceMoveType = OmenServiceBase<MovementManager>.Instance().GetInstanceMoveType(PositionUpdateInstancePacket.MoveType.NormalMove0);
				Vector3 position = playerCharacter.Position;
				Vector3 vector = Vector3.op_Implicit(((Treasure)treasure).Position);
				Vector3 position2 = ((!treasureTaskHelper.IsBusy) ? vector : VectorExtensions.WithY(vector, position.Y));
				new PositionUpdateInstancePacket(playerCharacter.Rotation, position2, instanceMoveType).Send();
				new TreasureOpenPacket(((Treasure)treasure).EntityId).Send();
				new PositionUpdateInstancePacket(playerCharacter.Rotation, position, instanceMoveType).Send();
			}
		}

		static TreasureManager()
		{
			Route[] array = new Route[4];
			string name = LanguageManager.Get("InnerLoop");
			int num = 31;
			List<Vector3> list = new List<Vector3>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<Vector3> span = CollectionsMarshal.AsSpan(list);
			span[0] = new Vector3(-590.23f, -23f, -7f);
			span[1] = new Vector3(-633.72f, -23f, -146.01f);
			span[2] = new Vector3(-581.51f, -23f, -257.44f);
			span[3] = new Vector3(-439.57f, -23f, -558.46f);
			span[4] = new Vector3(-232.44f, -23f, -720f);
			span[5] = new Vector3(-265.77f, -23f, -439.54f);
			span[6] = new Vector3(-254.17f, -23f, -266.32f);
			span[7] = new Vector3(-168.23f, -23f, -153.46f);
			span[8] = new Vector3(43.78f, -23f, -108.2f);
			span[9] = new Vector3(85.59f, -23f, -281.15f);
			span[10] = new Vector3(-26.02f, -23f, -437.71f);
			span[11] = new Vector3(254.72f, -23f, -605.01f);
			span[12] = new Vector3(279.07f, -23f, -356.16f);
			span[13] = new Vector3(658.81f, -23f, -364.71f);
			span[14] = new Vector3(478.45f, -23f, -202.99f);
			span[15] = new Vector3(383.29f, -23f, -175.68f);
			span[16] = new Vector3(223.65f, -23f, -30.66f);
			span[17] = new Vector3(161f, -23f, 15.98f);
			span[18] = new Vector3(313.89f, -23f, 180.04f);
			span[19] = new Vector3(449.39f, -23f, 105.21f);
			span[20] = new Vector3(447.87f, -23f, 463.34f);
			span[21] = new Vector3(246.2f, -23f, 676.66f);
			span[22] = new Vector3(77.04f, -23f, 536.25f);
			span[23] = new Vector3(-22.69f, -23f, 628.99f);
			span[24] = new Vector3(-278.07f, -23f, 567.96f);
			span[25] = new Vector3(-144.73f, -23f, 304.92f);
			span[26] = new Vector3(41.21f, -23f, 168.47f);
			span[27] = new Vector3(-162.07f, -23f, 98.44f);
			span[28] = new Vector3(-287.77f, -23f, 125.66f);
			span[29] = new Vector3(-436.45f, -23f, 166.22f);
			span[30] = new Vector3(-631.8f, -23f, 239.98f);
			array[0] = new Route(1346u, name, null, list);
			string name2 = LanguageManager.Get("OuterLoop");
			num = 37;
			List<Vector3> list2 = new List<Vector3>(num);
			CollectionsMarshal.SetCount(list2, num);
			Span<Vector3> span2 = CollectionsMarshal.AsSpan(list2);
			span2[0] = new Vector3(-879f, -23f, -314.23f);
			span2[1] = new Vector3(-707.39f, -70f, -396.99f);
			span2[2] = new Vector3(-697.29f, -70f, -565.03f);
			span2[3] = new Vector3(-857.6f, -70f, -609.83f);
			span2[4] = new Vector3(-815.82f, -70f, -699.4f);
			span2[5] = new Vector3(-928.65f, -70f, -744.96f);
			span2[6] = new Vector3(-736.05f, -70f, -881.5f);
			span2[7] = new Vector3(-525.81f, -23f, -783.47f);
			span2[8] = new Vector3(-416.8f, -23f, -945.43f);
			span2[9] = new Vector3(-2.33f, -23f, -814.91f);
			span2[10] = new Vector3(147.84f, -23f, -868.77f);
			span2[11] = new Vector3(389.52f, -23f, -733.03f);
			span2[12] = new Vector3(639.03f, -23f, -698.76f);
			span2[13] = new Vector3(634.79f, -23f, -831.82f);
			span2[14] = new Vector3(633.11f, -23f, -910.25f);
			span2[15] = new Vector3(865.45f, -23f, -874.11f);
			span2[16] = new Vector3(815.43f, -23f, -657.34f);
			span2[17] = new Vector3(658.72f, -23f, -552.33f);
			span2[18] = new Vector3(950.19f, -23f, -359f);
			span2[19] = new Vector3(649.53f, -23f, -157.79f);
			span2[20] = new Vector3(719.33f, -23f, 268.3f);
			span2[21] = new Vector3(758.14f, -23f, 506.8f);
			span2[22] = new Vector3(811.98f, -23f, 668.97f);
			span2[23] = new Vector3(673.73f, -23f, 729.64f);
			span2[24] = new Vector3(676.97f, -23f, 957.43f);
			span2[25] = new Vector3(222.89f, -23f, 913.6f);
			span2[26] = new Vector3(-12.1f, -23f, 773.86f);
			span2[27] = new Vector3(-256.98f, -23f, 812.19f);
			span2[28] = new Vector3(-504.11f, -23f, 758.3f);
			span2[29] = new Vector3(-612.24f, -23f, 578.55f);
			span2[30] = new Vector3(-592f, -23f, 767.67f);
			span2[31] = new Vector3(-645.44f, -23f, 967.93f);
			span2[32] = new Vector3(-699.86f, -23f, 926.36f);
			span2[33] = new Vector3(-857.82f, -23f, 772.21f);
			span2[34] = new Vector3(-800.41f, -23f, 633.39f);
			span2[35] = new Vector3(-775.91f, -23f, 377.13f);
			span2[36] = new Vector3(-923.16f, -23f, 197.92f);
			array[1] = new Route(1346u, name2, null, list2);
			string name3 = LanguageManager.Get("InnerLoop");
			num = 24;
			List<Vector3> list3 = new List<Vector3>(num);
			CollectionsMarshal.SetCount(list3, num);
			Span<Vector3> span3 = CollectionsMarshal.AsSpan(list3);
			span3[0] = new Vector3(-158.65f, -20f, -132.74f);
			span3[1] = new Vector3(-256.89f, -20f, 125.08f);
			span3[2] = new Vector3(-444.11f, -20f, 26.23f);
			span3[3] = new Vector3(-394.89f, -20f, 175.43f);
			span3[4] = new Vector3(-401.66f, -20f, 332.54f);
			span3[5] = new Vector3(-283.99f, -20f, 377.04f);
			span3[6] = new Vector3(-372.67f, -20f, 527.43f);
			span3[7] = new Vector3(-197.19f, -20f, 618.34f);
			span3[8] = new Vector3(35.72f, -20f, 648.95f);
			span3[9] = new Vector3(8.99f, -20f, 426.96f);
			span3[10] = new Vector3(-25.68f, -20f, 150.16f);
			span3[11] = new Vector3(277.79f, -20f, 241.9f);
			span3[12] = new Vector3(256.15f, -20f, 492.36f);
			span3[13] = new Vector3(517.75f, -20f, 236.13f);
			span3[14] = new Vector3(609.61f, -20f, 117.27f);
			span3[15] = new Vector3(475.73f, -20f, -87.08f);
			span3[16] = new Vector3(245.59f, -20f, -18.17f);
			span3[17] = new Vector3(354.12f, -20f, -288.93f);
			span3[18] = new Vector3(386.92f, -20f, -451.38f);
			span3[19] = new Vector3(142.11f, -20f, -574.06f);
			span3[20] = new Vector3(55.28f, -20f, -289.08f);
			span3[21] = new Vector3(-140.46f, -20f, -414.27f);
			span3[22] = new Vector3(-343.16f, -20f, -382.13f);
			span3[23] = new Vector3(-487.11f, -20f, -205.46f);
			array[2] = new Route(1252u, name3, null, list3);
			string name4 = LanguageManager.Get("OuterLoop");
			num = 44;
			List<Vector3> list4 = new List<Vector3>(num);
			CollectionsMarshal.SetCount(list4, num);
			Span<Vector3> span4 = CollectionsMarshal.AsSpan(list4);
			span4[0] = new Vector3(-682.8f, -20f, -195.27f);
			span4[1] = new Vector3(-767.45f, -20f, -235f);
			span4[2] = new Vector3(-798.25f, -20f, -310.57f);
			span4[3] = new Vector3(-680.54f, -20f, -354.79f);
			span4[4] = new Vector3(-491.02f, -20f, -529.59f);
			span4[5] = new Vector3(-661.71f, -20f, -579.49f);
			span4[6] = new Vector3(-884.12f, -20f, -682.03f);
			span4[7] = new Vector3(-825.1f, -20f, -833.6f);
			span4[8] = new Vector3(-729.43f, -20f, -724.82f);
			span4[9] = new Vector3(-585.29f, -20f, -864.84f);
			span4[10] = new Vector3(-451.68f, -20f, -775.57f);
			span4[11] = new Vector3(-118.97f, -20f, -708.46f);
			span4[12] = new Vector3(381.73f, -20f, -743.65f);
			span4[13] = new Vector3(490.41f, -20f, -590.57f);
			span4[14] = new Vector3(617.09f, -20f, -703.88f);
			span4[15] = new Vector3(666.53f, -20f, -480.37f);
			span4[16] = new Vector3(870.66f, -20f, -388.36f);
			span4[17] = new Vector3(779.02f, -20f, -256.24f);
			span4[18] = new Vector3(770.75f, -20f, -143.57f);
			span4[19] = new Vector3(726.28f, -20f, -67.92f);
			span4[20] = new Vector3(788.88f, -20f, 109.39f);
			span4[21] = new Vector3(642.97f, -20f, 407.8f);
			span4[22] = new Vector3(826.69f, -20f, 434.99f);
			span4[23] = new Vector3(869.29f, -20f, 581.2f);
			span4[24] = new Vector3(835.08f, -20f, 699.09f);
			span4[25] = new Vector3(697.32f, -20f, 597.92f);
			span4[26] = new Vector3(596.46f, -20f, 622.77f);
			span4[27] = new Vector3(471.18f, -20f, 530.02f);
			span4[28] = new Vector3(433.71f, -20f, 683.53f);
			span4[29] = new Vector3(294.88f, -20f, 640.22f);
			span4[30] = new Vector3(140.98f, -20f, 770.99f);
			span4[31] = new Vector3(-225.02f, -20f, 804.99f);
			span4[32] = new Vector3(-550.13f, -20f, 627.74f);
			span4[33] = new Vector3(-676.42f, -20f, 640.38f);
			span4[34] = new Vector3(-645.69f, -20f, 710.17f);
			span4[35] = new Vector3(-600.27f, -20f, 802.64f);
			span4[36] = new Vector3(-716.15f, -20f, 794.43f);
			span4[37] = new Vector3(-784.76f, -20f, 699.76f);
			span4[38] = new Vector3(-729.55f, -20f, 561.15f);
			span4[39] = new Vector3(-648f, -20f, 403.95f);
			span4[40] = new Vector3(-713.8f, -20f, 192.61f);
			span4[41] = new Vector3(-756.83f, -20f, 97.37f);
			span4[42] = new Vector3(-729.92f, -20f, -79.06f);
			span4[43] = new Vector3(-856.96f, -20f, -93.16f);
			array[3] = new Route(1252u, name4, null, list4);
			Routes = array;
		}
	}

	private Config config;

	private AetheryteManager aetheryteModule;

	private EventManager eventModule;

	private TreasureManager treasureModule;

	private SupportJobManager supportJobModule;

	private OthersManager othersModule;

	private List<BaseIslandModule> modules = new List<BaseIslandModule>();

	private const int MAX_DUTY_COMMAND_LINES = 15;

	private const uint DEMI_RETURN_ACTION_ID = 41343u;

	private const uint SOUTH_HORN_TERRITORY_ID = 1252u;

	private const uint NORTH_HORN_TERRITORY_ID = 1346u;

	private const uint PHANTOM_VILLIAGE_TERRITORY_ID = 1278u;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("OccultCrescentHelperTitle"),
		Description = LanguageManager.Get("OccultCrescentHelperDescription"),
		Category = ModuleCategory.Duty,
		Author = new string[1] { "Fragile" },
		ModulesConflict = new string[1] { "AutoFaceCameraDirection" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected override void Init()
	{
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		bool flag = false;
		if (!string.IsNullOrWhiteSpace(config.AutoEnableDisablePlugins))
		{
			string[] source = config.AutoEnableDisablePlugins.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Take(15).ToArray();
			config.JoinDutyCommands = string.Join("\n", source.Select((string plugin) => "/xlenableplugin " + plugin));
			config.LeaveDutyCommands = string.Join("\n", source.Select((string plugin) => "/xldisableplugin " + plugin));
			config.AutoEnableDisablePlugins = string.Empty;
			flag = true;
		}
		string text = LimitDutyCommandLines(config.JoinDutyCommands);
		if (text != config.JoinDutyCommands)
		{
			config.JoinDutyCommands = text;
			flag = true;
		}
		string text2 = LimitDutyCommandLines(config.LeaveDutyCommands);
		if (text2 != config.LeaveDutyCommands)
		{
			config.LeaveDutyCommands = text2;
			flag = true;
		}
		if (flag)
		{
			config.Save(this);
		}
		if (base.Overlay == null)
		{
			Overlay overlay = (base.Overlay = new Overlay(this));
		}
		Overlay? overlay3 = base.Overlay;
		((Window)overlay3).Flags = (ImGuiWindowFlags)(((Window)overlay3).Flags & -65);
		aetheryteModule = new AetheryteManager(this);
		eventModule = new EventManager(this);
		treasureModule = new TreasureManager(this);
		supportJobModule = new SupportJobManager(this);
		othersModule = new OthersManager(this);
		int num = 5;
		List<BaseIslandModule> list = new List<BaseIslandModule>(num);
		CollectionsMarshal.SetCount(list, num);
		Span<BaseIslandModule> span = CollectionsMarshal.AsSpan(list);
		span[0] = aetheryteModule;
		span[1] = eventModule;
		span[2] = treasureModule;
		span[3] = supportJobModule;
		span[4] = othersModule;
		modules = list;
		foreach (BaseIslandModule module in modules)
		{
			module.Init();
		}
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += OnZoneChanged;
		OnZoneChanged(0u);
	}

	protected override void Uninit()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChanged;
		OmenServiceBase<FrameworkManager>.Instance().Unreg(OnUpdate);
		foreach (BaseIslandModule module in modules)
		{
			module.Uninit();
		}
	}

	private void OnZoneChanged(uint u)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Invalid comparison between Unknown and I4
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected O, but got Unknown
		OmenServiceBase<FrameworkManager>.Instance().Unreg(OnUpdate);
		if ((int)GameState.TerritoryIntendedUse == 61)
		{
			OmenServiceBase<FrameworkManager>.Instance().Reg(new OnUpdateDelegate(OnUpdate), 1000u);
		}
	}

	private void OnUpdate(IFramework framework)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		if ((int)GameState.TerritoryIntendedUse != 61)
		{
			OmenServiceBase<FrameworkManager>.Instance().Unreg(OnUpdate);
			return;
		}
		foreach (BaseIslandModule module in modules)
		{
			module.OnUpdate();
		}
	}

	protected override void ConfigUI()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		using (OmenServiceBase<FontManager>.Instance().UIFont.Push())
		{
			TabBarDisposable val = ImRaii.TabBar(ImU8String.op_Implicit("###Config"), (ImGuiTabBarFlags)1);
			try
			{
				if (!val)
				{
					return;
				}
				ImU8String val2 = new ImU8String(0, 1);
				((ImU8String)(ref val2)).AppendFormatted<string>(LuminaWrapper.GetEObjName(2014664u));
				TabItemDisposable val3 = ImRaii.TabItem(val2);
				try
				{
					if (TabItemDisposable.op_Implicit(val3))
					{
						IdDisposable val4 = ImRaii.PushId(ImU8String.op_Implicit("AetheryteManager"), true);
						try
						{
							aetheryteModule.DrawConfig();
						}
						finally
						{
							((IDisposable)val4)?.Dispose();
						}
					}
				}
				finally
				{
					((TabItemDisposable)(ref val3)).Dispose();
				}
				TabItemDisposable val5 = ImRaii.TabItem(ImU8String.op_Implicit(LanguageManager.Get("DynamicEvent")));
				try
				{
					if (TabItemDisposable.op_Implicit(val5))
					{
						IdDisposable val4 = ImRaii.PushId(ImU8String.op_Implicit("CEManager"), true);
						try
						{
							eventModule.DrawConfig();
						}
						finally
						{
							((IDisposable)val4)?.Dispose();
						}
					}
				}
				finally
				{
					((TabItemDisposable)(ref val5)).Dispose();
				}
				ImU8String val6 = new ImU8String(0, 1);
				((ImU8String)(ref val6)).AppendFormatted<string>(LuminaWrapper.GetAddonText(395u));
				TabItemDisposable val7 = ImRaii.TabItem(val6);
				try
				{
					if (TabItemDisposable.op_Implicit(val7))
					{
						IdDisposable val4 = ImRaii.PushId(ImU8String.op_Implicit("TreasureManager"), true);
						try
						{
							treasureModule.DrawConfig();
						}
						finally
						{
							((IDisposable)val4)?.Dispose();
						}
					}
				}
				finally
				{
					((TabItemDisposable)(ref val7)).Dispose();
				}
				ImU8String val8 = new ImU8String(0, 1);
				((ImU8String)(ref val8)).AppendFormatted<string>(LuminaWrapper.GetAddonText(16633u));
				TabItemDisposable val9 = ImRaii.TabItem(val8);
				try
				{
					if (TabItemDisposable.op_Implicit(val9))
					{
						IdDisposable val4 = ImRaii.PushId(ImU8String.op_Implicit("SupportJobManager"), true);
						try
						{
							supportJobModule.DrawConfig();
						}
						finally
						{
							((IDisposable)val4)?.Dispose();
						}
					}
				}
				finally
				{
					((TabItemDisposable)(ref val9)).Dispose();
				}
				ImU8String val10 = new ImU8String(0, 1);
				((ImU8String)(ref val10)).AppendFormatted<string>(LuminaWrapper.GetAddonText(832u));
				TabItemDisposable val11 = ImRaii.TabItem(val10);
				try
				{
					if (TabItemDisposable.op_Implicit(val11))
					{
						IdDisposable val4 = ImRaii.PushId(ImU8String.op_Implicit("OthersManager"), true);
						try
						{
							othersModule.DrawConfig();
							return;
						}
						finally
						{
							((IDisposable)val4)?.Dispose();
						}
					}
				}
				finally
				{
					((TabItemDisposable)(ref val11)).Dispose();
				}
			}
			finally
			{
				((TabBarDisposable)(ref val)).Dispose();
			}
		}
	}

	protected override void OverlayPreDraw()
	{
		OmenServiceBase<FontManager>.Instance().UIFont80.Push();
	}

	protected override void OverlayUI()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		if ((int)GameState.TerritoryIntendedUse != 61)
		{
			((Window)base.Overlay).IsOpen = false;
		}
		else
		{
			ConfigUI();
		}
	}

	protected override void OverlayPostDraw()
	{
		OmenServiceBase<FontManager>.Instance().UIFont80.Pop();
	}

	private static string LimitDutyCommandLines(string commands)
	{
		if (string.IsNullOrEmpty(commands))
		{
			return string.Empty;
		}
		string[] array = commands.Split(new string[3] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
		if (array.Length > 15)
		{
			return string.Join("\n", array.Take(15));
		}
		return commands;
	}

	private static int GetDutyCommandLineCount(string commands)
	{
		if (!string.IsNullOrEmpty(commands))
		{
			return commands.Split(new string[3] { "\r\n", "\n", "\r" }, StringSplitOptions.None).Length;
		}
		return 0;
	}
}
