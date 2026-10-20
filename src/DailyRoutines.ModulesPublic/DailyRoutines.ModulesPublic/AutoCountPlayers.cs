using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Info.Models;
using DailyRoutines.Common.Interface.Windows;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Gui.Dtr;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Hooking;
using Dalamud.Interface;
using Dalamud.Interface.ImGuiSeStringRenderer;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Enums;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Common.Math;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Info.Game.Data;
using OmenTools.Info.Lumina;
using OmenTools.Interop.Game.Helpers;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;

namespace DailyRoutines.ModulesPublic;

public class AutoCountPlayers : ModuleBase
{
	private class Config : ModuleConfig
	{
		public bool DisplayLineWhenTargetingMe = true;

		public bool FilterFriend;

		public float ScaleFactor = 1f;

		public bool SendChat = true;

		public bool SendNotification = true;

		public bool SendTTS = true;

		public List<TargetingRecord> TargetingHistories = new List<TargetingRecord>();
	}

	private class TargetingRecord
	{
		public string Name { get; set; } = string.Empty;

		public uint HomeWorldID { get; set; }

		public uint JobID { get; set; }

		public uint ZoneID { get; set; }

		public DateTime StartTime { get; set; }

		public TimeSpan Duration { get; set; }
	}

	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static LogMessageManager.PreLogMessageDelegate _003C0_003E__OnLogMessage;

		public static OnUpdateDelegate _003C1_003E__OnUpdate;

		public static Action<WarpType> _003C2_003E__OnWarpCompleted;
	}

	private Hook<EndRequest>? InfoProxyContentMemberEndRequestHook;

	private Hook<EndRequest>? InfoProxySearchEndRequestHook;

	private Config config;

	private IDtrBarEntry? entry;

	private readonly Dictionary<uint, byte[]> jobIcons = new Dictionary<uint, byte[]>();

	private readonly Dictionary<uint, PlayerTargetingInfo> lastTargetingData = new Dictionary<uint, PlayerTargetingInfo>();

	private string searchInput = string.Empty;

	private string searchZoneInput = string.Empty;

	private const ImGuiWindowFlags WINDOW_FLAGS = (ImGuiWindowFlags)2896863;

	private const float LINE_THICKNESS = 3f;

	private const float LINE_OUTLINE_THICKNESS = 5f;

	private const float START_MARKER_RADIUS = 2.25f;

	private const float START_MARKER_OUTLINE_SIZE = 1.25f;

	private const float TARGET_MARKER_RADIUS = 4f;

	private const float TARGET_MARKER_OUTLINE_SIZE = 1.5f;

	private const float TARGET_MARKER_THICKNESS = 1.5f;

	private const float TARGET_MARKER_CORE_RADIUS = 1.25f;

	private const int TARGET_MARKER_SEGMENTS = 16;

	private const float OFFSCREEN_MARKER_INSET = 14f;

	private const float OFFSCREEN_LINE_OVERFLOW = 32f;

	private const float MIN_DIRECTION_LENGTH_SQUARED = 0.001f;

	private const float INFO_LINE_OPACITY = 0.78f;

	private const float ALERT_LINE_OPACITY = 0.88f;

	private const float INFO_LABEL_OPACITY = 0.96f;

	private const float ALERT_LABEL_OPACITY = 0.98f;

	private const float LINE_OUTLINE_OPACITY = 0.4f;

	private const float MARKER_CORE_OPACITY = 0.88f;

	private static readonly uint InfoLineColor;

	private static readonly uint AlertLineColor;

	private static readonly uint InfoLabelColor;

	private static readonly uint AlertLabelColor;

	private static readonly uint LineOutlineColor;

	private static readonly uint MarkerCoreColor;

	private static readonly FrozenSet<TerritoryIntendedUse> ContentMemberListValidZones;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoCountPlayersTitle"),
		Description = LanguageManager.Get("AutoCountPlayersDescription"),
		Category = ModuleCategory.General
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	private static bool IsPlayerSearchLocation
	{
		get
		{
			if (!IsContentSearchZone)
			{
				return IsPlayerSearchZone;
			}
			return true;
		}
	}

	private static bool IsContentSearchZone => ContentMemberListValidZones.Contains(GameState.TerritoryIntendedUse);

	private static bool IsPlayerSearchZone
	{
		get
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			if (GameState.TerritoryType != 0 && GameState.ContentFinderCondition == 0)
			{
				Dictionary<uint, PlayerSearchSubLocation> playerSearchPlaceNames = Sheets.PlayerSearchPlaceNames;
				TerritoryType territoryTypeData = GameState.TerritoryTypeData;
				return playerSearchPlaceNames.ContainsKey(((TerritoryType)(ref territoryTypeData)).PlaceNameZone.RowId);
			}
			return false;
		}
	}

	protected unsafe override void Init()
	{
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Expected O, but got Unknown
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Expected O, but got Unknown
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		if (entry == null)
		{
			entry = DalamudServiceExtension.Instance<IDtrBar>().Get("DailyRoutines-AutoCountPlayers", (SeString)null);
		}
		entry.Shown = true;
		entry.Text = SeString.op_Implicit(LanguageManager.Get("AutoCountPlayers-PlayersAroundCount") + ": 0");
		entry.OnClick = delegate
		{
			EnsureOverlay();
			Overlay? overlay = base.Overlay;
			((Window)overlay).IsOpen = !((Window)overlay).IsOpen;
		};
		OmenServiceBase<WindowManager>.Instance().PostDraw += OnDraw;
		ManagerBase<PlayersManager>.Instance().ReceivePlayersAround += OnReceivePlayers;
		ManagerBase<PlayersManager>.Instance().ReceivePlayersTargetingMe += OnPlayersTargetingMeUpdate;
		InfoProxyContentMemberEndRequestHook = (*((InfoProxyContentMember)InfoProxyContentMember.Instance()).VirtualTable).HookVFuncFromName<InfoProxyContentMemberVirtualTable, EndRequest>("EndRequest", new EndRequest(InfoProxyContentMemberRequestDetour));
		InfoProxyContentMemberEndRequestHook.Enable();
		InfoProxySearchEndRequestHook = (*((InfoProxySearch)InfoProxySearch.Instance()).VirtualTable).HookVFuncFromName<InfoProxySearchVirtualTable, EndRequest>("EndRequest", new EndRequest(InfoProxySearchRequestDetour));
		InfoProxySearchEndRequestHook.Enable();
		OmenServiceBase<LogMessageManager>.Instance().RegPre(OnLogMessage);
		FrameworkManager frameworkManager = OmenServiceBase<FrameworkManager>.Instance();
		object obj = _003C_003EO._003C1_003E__OnUpdate;
		if (obj == null)
		{
			OnUpdateDelegate val = OnUpdate;
			_003C_003EO._003C1_003E__OnUpdate = val;
			obj = (object)val;
		}
		frameworkManager.Reg((OnUpdateDelegate)obj, 1000u);
		OnUpdate(DalamudServiceExtension.Instance<IFramework>());
		OmenServiceBase<GameState>.Instance().WarpComplete += OnWarpCompleted;
	}

	protected override void Uninit()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		OmenServiceBase<GameState>.Instance().WarpComplete -= OnWarpCompleted;
		FrameworkManager frameworkManager = OmenServiceBase<FrameworkManager>.Instance();
		OnUpdateDelegate[] array = new OnUpdateDelegate[1];
		object obj = _003C_003EO._003C1_003E__OnUpdate;
		if (obj == null)
		{
			OnUpdateDelegate val = OnUpdate;
			_003C_003EO._003C1_003E__OnUpdate = val;
			obj = (object)val;
		}
		array[0] = (OnUpdateDelegate)obj;
		frameworkManager.Unreg((OnUpdateDelegate[])(object)array);
		OmenServiceBase<LogMessageManager>.Instance().Unreg(OnLogMessage);
		OmenServiceBase<WindowManager>.Instance().PostDraw -= OnDraw;
		ManagerBase<PlayersManager>.Instance().ReceivePlayersAround -= OnReceivePlayers;
		ManagerBase<PlayersManager>.Instance().ReceivePlayersTargetingMe -= OnPlayersTargetingMeUpdate;
		foreach (PlayerTargetingInfo value in lastTargetingData.Values)
		{
			TimeSpan duration = DateTime.Now - value.TargetingStartTime;
			config.TargetingHistories.Add(new TargetingRecord
			{
				Name = value.Player.Name,
				HomeWorldID = value.Player.HomeWorld.RowId,
				JobID = value.Player.ClassJob.RowId,
				StartTime = value.TargetingStartTime,
				Duration = duration,
				ZoneID = GameState.TerritoryType
			});
		}
		if (lastTargetingData.Count > 0)
		{
			lastTargetingData.Clear();
			if (config.TargetingHistories.Count > 100)
			{
				config.TargetingHistories.RemoveRange(0, config.TargetingHistories.Count - 100);
			}
			config.Save(this);
		}
		IDtrBarEntry? obj2 = entry;
		if (obj2 != null)
		{
			obj2.Remove();
		}
		entry = null;
	}

	protected override void ConfigUI()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		ImGui.SetNextItemWidth(120f * Globals.GlobalUIScale);
		if (ImGui.InputFloat(ImU8String.op_Implicit(LanguageManager.Get("Scale")), ref config.ScaleFactor, 0f, 0f, ImU8String.op_Implicit("%.1f"), (ImGuiInputTextFlags)0))
		{
			config.ScaleFactor = Math.Max(0.1f, config.ScaleFactor);
		}
		if (ImGui.IsItemDeactivatedAfterEdit())
		{
			config.Save(this);
		}
		ImGui.NewLine();
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("AutoCountPlayers-DisplayLineWhenTargetingMe")), ref config.DisplayLineWhenTargetingMe))
		{
			config.Save(this);
		}
		if (!config.DisplayLineWhenTargetingMe)
		{
			return;
		}
		IndentDisposable val = ImRaii.PushIndent(1, true);
		try
		{
			if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("SendChat")), ref config.SendChat))
			{
				config.Save(this);
			}
			if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("SendNotification")), ref config.SendNotification))
			{
				config.Save(this);
			}
			if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("SendTTS")), ref config.SendTTS))
			{
				config.Save(this);
			}
			if (!config.SendNotification && !config.SendTTS)
			{
				return;
			}
			IndentDisposable val2 = ImRaii.PushIndent(1, true);
			try
			{
				if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("AutoCountPlayers-FilterFriend")), ref config.FilterFriend))
				{
					config.Save(this);
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
	}

	protected unsafe override void OverlayUI()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_043d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0447: Unknown result type (might be due to invalid IL or missing references)
		//IL_044c: Unknown result type (might be due to invalid IL or missing references)
		//IL_045f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0464: Unknown result type (might be due to invalid IL or missing references)
		//IL_0466: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0610: Unknown result type (might be due to invalid IL or missing references)
		//IL_0623: Unknown result type (might be due to invalid IL or missing references)
		//IL_0628: Unknown result type (might be due to invalid IL or missing references)
		//IL_0636: Unknown result type (might be due to invalid IL or missing references)
		//IL_063b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0697: Unknown result type (might be due to invalid IL or missing references)
		//IL_069c: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0709: Unknown result type (might be due to invalid IL or missing references)
		//IL_0654: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Expected O, but got Unknown
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Expected O, but got Unknown
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_050b: Unknown result type (might be due to invalid IL or missing references)
		//IL_051c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0521: Unknown result type (might be due to invalid IL or missing references)
		//IL_052f: Unknown result type (might be due to invalid IL or missing references)
		//IL_055f: Unknown result type (might be due to invalid IL or missing references)
		TabBarDisposable val = ImRaii.TabBar(ImU8String.op_Implicit("##Tab"));
		try
		{
			if (!val)
			{
				return;
			}
			TabItemDisposable val2 = ImRaii.TabItem(ImU8String.op_Implicit(LanguageManager.Get("AutoCountPlayers-PlayersAround")));
			ImGuiStylePtr style;
			try
			{
				if (TabItemDisposable.op_Implicit(val2))
				{
					ImGui.SetNextItemWidth(-1f);
					ImGui.InputText(ImU8String.op_Implicit("###Search"), ref searchInput, 128, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
					if (IConditionExtension.get_IsBetweenAreas(DalamudServiceExtension.Instance<ICondition>()))
					{
						return;
					}
					ImU8String val3 = ImU8String.op_Implicit("列表");
					Vector2 contentRegionAvail = ImGui.GetContentRegionAvail();
					style = ImGui.GetStyle();
					ChildDisposable val4 = ImRaii.Child(val3, contentRegionAvail - ((ImGuiStylePtr)(ref style)).ItemSpacing, true);
					try
					{
						if (!val4)
						{
							return;
						}
						Vector2 projectedPosition = default(Vector2);
						bool flag = default(bool);
						Vector2 startPos = default(Vector2);
						bool flag2 = default(bool);
						ISharedImmediateTexture val9 = default(ISharedImmediateTexture);
						foreach (IPlayerCharacter item in ManagerBase<PlayersManager>.Instance().PlayersAround)
						{
							ImU8String val5 = new ImU8String(0, 1);
							((ImU8String)(ref val5)).AppendFormatted<ulong>(item.GameObjectID);
							IdDisposable val6 = ImRaii.PushId(val5, true);
							try
							{
								if (string.IsNullOrWhiteSpace(searchInput) || item.Name.Contains(searchInput))
								{
									if (ImGuiOm.ButtonIcon("定位", (FontAwesomeIcon)61476, LanguageManager.Get("Locate")))
									{
										Vector2 vector = PositionHelper.WorldToMap(item.Position.ToVector2(), GameState.MapData);
										SeStringBuilder obj = new SeStringBuilder().Add((Payload)new PlayerPayload(item.Name, (uint)((BattleChara)item.ToStruct()).HomeWorld)).Append(" (").AddIcon(item.ClassJob.Value.ToBitmapFontIcon());
										DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(2, 1);
										defaultInterpolatedStringHandler.AppendLiteral(" ");
										ClassJob value = item.ClassJob.Value;
										defaultInterpolatedStringHandler.AppendFormatted<ReadOnlySeString>(((ClassJob)(ref value)).Name);
										defaultInterpolatedStringHandler.AppendLiteral(")");
										SeString val7 = obj.Append(defaultInterpolatedStringHandler.ToStringAndClear()).Add((Payload)new NewLinePayload()).Append("     ")
											.Append(SeString.CreateMapLink(GameState.TerritoryType, GameState.Map, vector.X, vector.Y, 0.05f))
											.Build();
										OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(val7.Encode()));
									}
									IGameGui obj2 = DalamudServiceExtension.Instance<IGameGui>();
									ImGuiViewportPtr mainViewport = ImGui.GetMainViewport();
									obj2.WorldToScreen(item.Position, ref projectedPosition, ref flag);
									if (!obj2.WorldToScreen(LocalPlayerState.Object.Position, ref startPos, ref flag2))
									{
										startPos = ((ImGuiViewportPtr)(ref mainViewport)).Pos + new Vector2(((ImGuiViewportPtr)(ref mainViewport)).Size.X * 0.5f, ((ImGuiViewportPtr)(ref mainViewport)).Size.Y);
									}
									if (!ImGui.IsAnyItemHovered() || ImGui.IsItemHovered())
									{
										(Vector2, Vector2) linePositions = GetLinePositions(projectedPosition, flag, ((ImGuiViewportPtr)(ref mainViewport)).Pos, ((ImGuiViewportPtr)(ref mainViewport)).Size, 14f * Globals.GlobalUIScale, 32f * Globals.GlobalUIScale);
										DrawLine(startPos, linePositions.Item1, linePositions.Item2, item, flag);
									}
									ITextureProvider obj3 = DalamudServiceExtension.Instance<ITextureProvider>();
									GameIconLookup val8 = GameIconLookup.op_Implicit(item.ClassJob.Value.GetIcon());
									if (obj3.TryGetFromGameIcon(ref val8, ref val9))
									{
										ImGui.SameLine();
										ImGui.Image(val9.GetWrapOrEmpty().Handle, new Vector2(ImGui.GetFrameHeight()));
									}
									ImGui.SameLine();
									string name = item.Name;
									World value2 = item.HomeWorld.Value;
									ImGuiOm.RenderPlayerInfo(name, ((object)((World)(ref value2)).Name/*cast due to constrained. prefix*/).ToString());
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
						((ChildDisposable)(ref val4)).Dispose();
					}
				}
			}
			finally
			{
				((TabItemDisposable)(ref val2)).Dispose();
			}
			if (IsPlayerSearchLocation)
			{
				TabItemDisposable val10 = ImRaii.TabItem(ImU8String.op_Implicit(LanguageManager.Get("AutoCountPlayers-PlayersInZone")));
				try
				{
					if (TabItemDisposable.op_Implicit(val10))
					{
						ImGui.SetNextItemWidth(-1f);
						ImGui.InputText(ImU8String.op_Implicit("###Search"), ref searchZoneInput, 128, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
						if (IConditionExtension.get_IsBetweenAreas(DalamudServiceExtension.Instance<ICondition>()))
						{
							return;
						}
						ImU8String val11 = ImU8String.op_Implicit("列表");
						Vector2 contentRegionAvail2 = ImGui.GetContentRegionAvail();
						style = ImGui.GetStyle();
						ChildDisposable val12 = ImRaii.Child(val11, contentRegionAvail2 - ((ImGuiStylePtr)(ref style)).ItemSpacing, true);
						try
						{
							if (!val12)
							{
								return;
							}
							InfoProxyCommonList* ptr = (IsPlayerSearchZone ? ((InfoProxyCommonList*)InfoProxySearch.Instance()) : ((InfoProxyCommonList*)InfoProxyContentMember.Instance()));
							for (int i = 0; i < ((InfoProxyCommonList)ptr).EntryCount; i++)
							{
								CharacterData val13 = ((InfoProxyCommonList)ptr).CharDataSpan.get_Item(i);
								if (IsPlayerSearchZone && val13.Location != GameState.TerritoryType)
								{
									continue;
								}
								ImU8String val14 = new ImU8String(0, 1);
								((ImU8String)(ref val14)).AppendFormatted<ulong>(val13.ContentId);
								IdDisposable val15 = ImRaii.PushId(val14, true);
								try
								{
									if (string.IsNullOrWhiteSpace(searchZoneInput) || ((CharacterData)(ref val13)).NameString.Contains(searchZoneInput))
									{
										ITextureProvider obj4 = DalamudServiceExtension.Instance<ITextureProvider>();
										GameIconLookup val8 = GameIconLookup.op_Implicit(LuminaWrapper.GetJobIcon(val13.Job));
										ImGui.Image(obj4.GetFromGameIcon(ref val8).GetWrapOrEmpty().Handle, new Vector2(ImGui.GetTextLineHeight()));
										ImGui.SameLine(0f, 4f * Globals.GlobalUIScale);
										ImGuiOm.RenderPlayerInfo(((CharacterData)(ref val13)).NameString, LuminaWrapper.GetWorldName(val13.HomeWorld));
									}
								}
								finally
								{
									((IDisposable)val15)?.Dispose();
								}
							}
						}
						finally
						{
							((ChildDisposable)(ref val12)).Dispose();
						}
					}
				}
				finally
				{
					((TabItemDisposable)(ref val10)).Dispose();
				}
			}
			TabItemDisposable val16 = ImRaii.TabItem(ImU8String.op_Implicit(LanguageManager.Get("AutoCountPlayers-TargetedHistory")));
			try
			{
				if (!TabItemDisposable.op_Implicit(val16))
				{
					return;
				}
				ISharedImmediateTexture val18 = default(ISharedImmediateTexture);
				foreach (TargetingRecord item2 in config.TargetingHistories.AsEnumerable().Reverse())
				{
					ImU8String val17 = new ImU8String(0, 1);
					((ImU8String)(ref val17)).AppendFormatted<DateTime>(item2.StartTime, "MM/dd HH:mm:ss");
					ImGui.TextDisabled(val17);
					ITextureProvider obj5 = DalamudServiceExtension.Instance<ITextureProvider>();
					GameIconLookup val8 = GameIconLookup.op_Implicit(LuminaGetter.GetRowOrDefault<ClassJob>(item2.JobID).GetIcon());
					if (obj5.TryGetFromGameIcon(ref val8, ref val18))
					{
						ImGui.SameLine();
						ImGui.Image(val18.GetWrapOrEmpty().Handle, new Vector2(ImGui.GetTextLineHeight()));
					}
					ImGui.SameLine();
					ImGuiOm.RenderPlayerInfo(item2.Name, LuminaWrapper.GetWorldName(item2.HomeWorldID));
					ImGui.SameLine();
					Vector4 vector2 = KnownColor.Orange.ToVector4();
					ImU8String val19 = new ImU8String(2, 1);
					((ImU8String)(ref val19)).AppendLiteral("[");
					((ImU8String)(ref val19)).AppendFormatted<TimeSpan>(item2.Duration, "mm\\:ss");
					((ImU8String)(ref val19)).AppendLiteral("]");
					ImGui.TextColored(ref vector2, val19);
					ImGui.SameLine();
					ImU8String val20 = new ImU8String(2, 1);
					((ImU8String)(ref val20)).AppendLiteral("(");
					((ImU8String)(ref val20)).AppendFormatted<string>(LuminaWrapper.GetZonePlaceName(item2.ZoneID));
					((ImU8String)(ref val20)).AppendLiteral(")");
					ImGui.TextDisabled(val20);
				}
			}
			finally
			{
				((TabItemDisposable)(ref val16)).Dispose();
			}
		}
		finally
		{
			((TabBarDisposable)(ref val)).Dispose();
		}
	}

	private static void OnWarpCompleted(WarpType warpType)
	{
		Throttler.Shared.Remove("AutoCountPlayers.Zone");
	}

	private unsafe void OnDraw()
	{
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		if (!config.DisplayLineWhenTargetingMe || ManagerBase<PlayersManager>.Instance().PlayersTargetingMe.Count == 0 || !GameState.IsForeground)
		{
			return;
		}
		BattleChara* localPlayer = Control.GetLocalPlayer();
		if (localPlayer == null)
		{
			return;
		}
		if ((*Addons.NamePlate).IsAddonAndNodesReady())
		{
			AtkResNode* nodeById = ((AtkUnitBase)Addons.NamePlate).GetNodeById(1u);
			if (nodeById != null)
			{
				NodeState nodeState = (*nodeById).GetNodeState();
				ImU8String val = default(ImU8String);
				((ImU8String)(ref val))._002Ector(17, 1);
				((ImU8String)(ref val)).AppendLiteral("AutoCountPlayers-");
				((ImU8String)(ref val)).AppendFormatted<uint>(((BattleChara)localPlayer).EntityId);
				if (ImGui.Begin(val, (ImGuiWindowFlags)2896863))
				{
					ImGui.SetWindowPos(nodeState.Center - ImGui.GetWindowSize() * 0.75f, (ImGuiCond)0);
					using (OmenServiceBase<FontManager>.Instance().UIFont140.Push())
					{
						GroupDisposable val2 = ImRaii.Group();
						try
						{
							ReadOnlySpan<byte> readOnlySpan = new SeStringBuilder().AddIcon((BitmapFontIcon)63).Encode();
							SeStringDrawParams val3 = default(SeStringDrawParams);
							ImGuiHelpers.SeStringWrapped(readOnlySpan, ref val3, default(ImGuiId), (ImGuiButtonFlags)1);
							ImGui.SameLine();
							ImGui.SetCursorPosY(ImGui.GetCursorPosY() - 1.2f * Globals.GlobalUIScale);
							ImGuiOm.TextOutlined(KnownColor.Orange.ToUInt(), $"{ManagerBase<PlayersManager>.Instance().PlayersTargetingMe.Count}", KnownColor.SaddleBrown.ToUInt());
							if (GameState.ContentFinderCondition == 0)
							{
								using (OmenServiceBase<FontManager>.Instance().UIFont80.Push())
								{
									string text = LanguageManager.Get("AutoCountPlayers-Notification-SomeoneTargetingMe");
									ImGuiOm.TextOutlined(ImGui.GetCursorScreenPos() - new Vector2(ImGui.CalcTextSize(ImU8String.op_Implicit(text), false, -1f).X * 0.3f, 0f), KnownColor.Orange.ToUInt(), "(" + text + ")", KnownColor.SaddleBrown.ToUInt());
								}
							}
						}
						finally
						{
							((GroupDisposable)(ref val2)).Dispose();
						}
					}
					ImGui.End();
				}
			}
		}
		IGameGui val4 = DalamudServiceExtension.Instance<IGameGui>();
		ImGuiViewportPtr mainViewport = ImGui.GetMainViewport();
		Vector2 startPos = default(Vector2);
		bool flag = default(bool);
		if (!val4.WorldToScreen(Vector3.op_Implicit(((BattleChara)localPlayer).Position), ref startPos, ref flag))
		{
			startPos = ((ImGuiViewportPtr)(ref mainViewport)).Pos + new Vector2(((ImGuiViewportPtr)(ref mainViewport)).Size.X * 0.5f, ((ImGuiViewportPtr)(ref mainViewport)).Size.Y);
		}
		Vector2 projectedPosition = default(Vector2);
		bool flag2 = default(bool);
		foreach (PlayerTargetingInfo item in ManagerBase<PlayersManager>.Instance().PlayersTargetingMe)
		{
			val4.WorldToScreen(item.Player.Position, ref projectedPosition, ref flag2);
			(Vector2, Vector2) linePositions = GetLinePositions(projectedPosition, flag2, ((ImGuiViewportPtr)(ref mainViewport)).Pos, ((ImGuiViewportPtr)(ref mainViewport)).Size, 14f * Globals.GlobalUIScale, 32f * Globals.GlobalUIScale);
			DrawLine(startPos, linePositions.Item1, linePositions.Item2, item.Player, flag2, isAlert: true, $" [{TimeSpan.FromSeconds(item.TargetingDurationSeconds)}]");
		}
	}

	private static void OnLogMessage(ref bool isPrevented, ref uint logMessageID, ref LogMessageQueueItem item)
	{
		if (logMessageID == 81)
		{
			isPrevented = true;
		}
	}

	private unsafe static void OnUpdate(IFramework framework)
	{
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Expected O, but got Unknown
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		if (!IsPlayerSearchLocation || !UIModuleExtension.IsScreenReady() || !Throttler.Shared.Throttle("AutoCountPlayers.Zone", 60000u))
		{
			return;
		}
		if (IsContentSearchZone)
		{
			if (InfoProxyContentMember.Instance() != null && !((AgentInterface)((AgentModule)AgentModule.Instance()).GetAgentByInternalId((AgentId)299)).IsAgentActive())
			{
				((AgentId)299).SendEvent(0uL, 1);
			}
		}
		else if (IsPlayerSearchZone)
		{
			InfoProxySearch* ptr = InfoProxySearch.Instance();
			if (ptr != null && !((AgentInterface)((AgentModule)AgentModule.Instance()).GetAgentByInternalId((AgentId)63)).IsAgentActive())
			{
				((InfoProxySearch)ptr).JobMask = ulong.MaxValue;
				((InfoProxySearch)ptr).LevelMin = 1;
				((InfoProxySearch)ptr).LevelMax = 255;
				((InfoProxySearch)ptr).GrandCompanyMask = byte.MaxValue;
				((InfoProxySearch)ptr).LanguageMask = byte.MaxValue;
				((InfoProxySearch)ptr).OnlineStatusMask = 140737488355328uL;
				ref ushort reference = ref ((InfoProxySearch)ptr).LocationIDs[0];
				TerritoryType territoryTypeData = GameState.TerritoryTypeData;
				reference = (ushort)((TerritoryType)(ref territoryTypeData)).PlaceNameZone.RowId;
				((InfoProxySearch)ptr).LocationCount = 1;
				for (int i = 0; i < ((InfoProxySearch)ptr).Name.Length; i++)
				{
					((InfoProxySearch)ptr).Name[i] = 0;
				}
				((InfoProxySearch)ptr).RequestData();
			}
		}
		else
		{
			FrameworkManager frameworkManager = OmenServiceBase<FrameworkManager>.Instance();
			OnUpdateDelegate[] array = new OnUpdateDelegate[1];
			object obj = _003C_003EO._003C1_003E__OnUpdate;
			if (obj == null)
			{
				OnUpdateDelegate val = OnUpdate;
				_003C_003EO._003C1_003E__OnUpdate = val;
				obj = (object)val;
			}
			array[0] = (OnUpdateDelegate)obj;
			frameworkManager.Unreg((OnUpdateDelegate[])(object)array);
		}
	}

	private unsafe void OnReceivePlayers(IReadOnlyList<IPlayerCharacter> characters)
	{
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Expected O, but got Unknown
		if (entry == null)
		{
			return;
		}
		if (IsPlayerSearchLocation)
		{
			entry.Shown = true;
		}
		else
		{
			entry.Shown = !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)26] || GameState.IsInPVPArea;
		}
		if (!entry.Shown)
		{
			EnsureOverlay();
			((Window)base.Overlay).IsOpen = false;
			return;
		}
		entry.Text = SeString.op_Implicit($"{LanguageManager.Get("AutoCountPlayers-PlayersAroundCount")}: {ManagerBase<PlayersManager>.Instance().PlayersAroundCount}" + ((ManagerBase<PlayersManager>.Instance().PlayersTargetingMe.Count == 0) ? string.Empty : $" ({ManagerBase<PlayersManager>.Instance().PlayersTargetingMe.Count})"));
		if (IsContentSearchZone)
		{
			entry.Text.Append(SeString.op_Implicit($" / {LanguageManager.Get("AutoCountPlayers-PlayersZoneCount")}: {((InfoProxyContentMember)InfoProxyContentMember.Instance()).EntryCount}"));
		}
		else if (IsPlayerSearchZone)
		{
			int value = Enumerable.Count(((InfoProxySearch)InfoProxySearch.Instance()).CharDataSpan.ToArray(), (CharacterData x) => x.Job > 0 && x.Location == GameState.TerritoryType);
			entry.Text.Append(SeString.op_Implicit($" / {LanguageManager.Get("AutoCountPlayers-PlayersZoneCount")}: {value}"));
		}
		if (characters.Count == 0)
		{
			entry.Tooltip = SeString.op_Implicit(string.Empty);
			return;
		}
		SeStringBuilder tooltip = new SeStringBuilder();
		if (ManagerBase<PlayersManager>.Instance().PlayersTargetingMe.Count > 0)
		{
			tooltip.AddUiForeground((ushort)32).AddText(LanguageManager.Get("AutoCountPlayers-PlayersTargetingMe") ?? "").AddUiForegroundOff()
				.Add((Payload)(object)NewLinePayload.Payload);
			ManagerBase<PlayersManager>.Instance().PlayersTargetingMe.ForEach(delegate(PlayerTargetingInfo info)
			{
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0062: Unknown result type (might be due to invalid IL or missing references)
				//IL_0067: Unknown result type (might be due to invalid IL or missing references)
				//IL_006b: Unknown result type (might be due to invalid IL or missing references)
				SeStringBuilder obj = tooltip.AddIcon(info.Player.ClassJob.Value.ToBitmapFontIcon()).AddText(info.Player.Name ?? "").AddIcon((BitmapFontIcon)88);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
				World value2 = info.Player.HomeWorld.Value;
				defaultInterpolatedStringHandler.AppendFormatted<ReadOnlySeString>(((World)(ref value2)).Name);
				obj.AddText(defaultInterpolatedStringHandler.ToStringAndClear()).Add((Payload)(object)NewLinePayload.Payload);
			});
		}
		tooltip.AddUiForeground((ushort)32).AddText(LanguageManager.Get("AutoCountPlayers-PlayersAroundInfo") ?? "").AddUiForegroundOff()
			.Add((Payload)(object)NewLinePayload.Payload);
		characters.ForEach(delegate(IPlayerCharacter info)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			SeStringBuilder obj = tooltip.AddIcon(info.ClassJob.Value.ToBitmapFontIcon()).AddText(info.Name ?? "").AddIcon((BitmapFontIcon)88);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
			World value2 = info.HomeWorld.Value;
			defaultInterpolatedStringHandler.AppendFormatted<ReadOnlySeString>(((World)(ref value2)).Name);
			obj.AddText(defaultInterpolatedStringHandler.ToStringAndClear()).Add((Payload)(object)NewLinePayload.Payload);
		});
		SeString val = tooltip.Build();
		if (val.Payloads.Last() is NewLinePayload)
		{
			val.Payloads.RemoveAt(val.Payloads.Count - 1);
		}
		entry.Tooltip = val;
	}

	private unsafe void OnPlayersTargetingMeUpdate(IReadOnlyList<PlayerTargetingInfo> targetingPlayersInfo)
	{
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Expected O, but got Unknown
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Expected O, but got Unknown
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Expected O, but got Unknown
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0409: Expected O, but got Unknown
		//IL_046b: Unknown result type (might be due to invalid IL or missing references)
		HashSet<uint> currentIDs = targetingPlayersInfo.Select((PlayerTargetingInfo x) => x.Player.EntityID).ToHashSet();
		List<KeyValuePair<uint, PlayerTargetingInfo>> list = lastTargetingData.Where((KeyValuePair<uint, PlayerTargetingInfo> x) => !currentIDs.Contains(x.Key)).ToList();
		if (list.Count > 0)
		{
			foreach (KeyValuePair<uint, PlayerTargetingInfo> item in list)
			{
				item.Deconstruct(out var key, out var value);
				uint key2 = key;
				PlayerTargetingInfo playerTargetingInfo = value;
				TimeSpan duration = DateTime.Now - playerTargetingInfo.TargetingStartTime;
				config.TargetingHistories.Add(new TargetingRecord
				{
					Name = playerTargetingInfo.Player.Name,
					HomeWorldID = playerTargetingInfo.Player.HomeWorld.RowId,
					JobID = playerTargetingInfo.Player.ClassJob.RowId,
					StartTime = playerTargetingInfo.TargetingStartTime,
					Duration = duration,
					ZoneID = GameState.TerritoryType
				});
				lastTargetingData.Remove(key2);
			}
			if (config.TargetingHistories.Count > 100)
			{
				config.TargetingHistories.RemoveRange(0, config.TargetingHistories.Count - 100);
			}
			config.Save(this);
		}
		foreach (PlayerTargetingInfo item2 in targetingPlayersInfo)
		{
			if (item2.Player.ClassJob.RowId != 0)
			{
				lastTargetingData[item2.Player.EntityID] = item2;
			}
		}
		if (targetingPlayersInfo.Count <= 0 || (GameState.ContentFinderCondition != 0 && DalamudServiceExtension.Instance<IPartyList>().Length >= 2) || !targetingPlayersInfo.Where((PlayerTargetingInfo info) => info.IsNew).ToList().Any((PlayerTargetingInfo info) => Throttler.Shared.Throttle($"AutoCountPlayers-Player-{info.Player.EntityID}", 30000u)))
		{
			return;
		}
		if (config.SendTTS && (!config.FilterFriend || targetingPlayersInfo.All((PlayerTargetingInfo x) => !((BattleChara)x.Player.ToStruct()).IsFriend)))
		{
			NotifyHelper.Speak(LanguageManager.Get("AutoCountPlayers-Notification-SomeoneTargetingMe"));
		}
		if (config.SendNotification && (!config.FilterFriend || targetingPlayersInfo.All((PlayerTargetingInfo x) => !((BattleChara)x.Player.ToStruct()).IsFriend)))
		{
			OmenServiceBase<NotifyHelper>.Instance().NotificationWarning(LanguageManager.Get("AutoCountPlayers-Notification-SomeoneTargetingMe"));
		}
		if (!config.SendChat)
		{
			return;
		}
		SeStringBuilder val = new SeStringBuilder();
		val.Append(LanguageManager.Get("AutoCountPlayers-Notification-SomeoneTargetingMe") + ":");
		val.Add((Payload)new NewLinePayload());
		foreach (PlayerTargetingInfo item3 in targetingPlayersInfo)
		{
			SeStringBuilder obj = val.Add((Payload)new PlayerPayload(item3.Player.Name, item3.Player.HomeWorld.RowId)).Append(" (").AddIcon(item3.Player.ClassJob.Value.ToBitmapFontIcon());
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(2, 1);
			defaultInterpolatedStringHandler.AppendLiteral(" ");
			ClassJob value2 = item3.Player.ClassJob.Value;
			defaultInterpolatedStringHandler.AppendFormatted<ReadOnlySeString>(((ClassJob)(ref value2)).Name);
			defaultInterpolatedStringHandler.AppendLiteral(")");
			obj.Append(defaultInterpolatedStringHandler.ToStringAndClear());
			val.Add((Payload)new NewLinePayload());
		}
		SeString val2 = val.Build();
		if (val2.Payloads.Last() is NewLinePayload)
		{
			val2.Payloads.RemoveAt(val2.Payloads.Count - 1);
		}
		OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(val.Build().Encode()));
	}

	private unsafe void InfoProxyContentMemberRequestDetour(InfoProxyContentMember* proxy)
	{
		InfoProxyContentMemberEndRequestHook.Original.Invoke(proxy);
		OnReceivePlayers(ManagerBase<PlayersManager>.Instance().PlayersAround);
	}

	private unsafe void InfoProxySearchRequestDetour(InfoProxySearch* proxy)
	{
		InfoProxySearchEndRequestHook.Original.Invoke(proxy);
		OnReceivePlayers(ManagerBase<PlayersManager>.Instance().PlayersAround);
	}

	private void DrawLine(Vector2 startPos, Vector2 lineEndPos, Vector2 markerPos, ICharacter chara, bool isMarkerVisible, bool isAlert = false, string? extraInfo = null)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		ImDrawListPtr foregroundDrawList = ImGui.GetForegroundDrawList();
		uint num = (isAlert ? AlertLineColor : InfoLineColor);
		uint textColor = (isAlert ? AlertLabelColor : InfoLabelColor);
		float num2 = 2.25f * Globals.GlobalUIScale;
		float num3 = 4f * Globals.GlobalUIScale;
		((ImDrawListPtr)(ref foregroundDrawList)).AddLine(startPos, lineEndPos, LineOutlineColor, 5f * Globals.GlobalUIScale);
		((ImDrawListPtr)(ref foregroundDrawList)).AddLine(startPos, lineEndPos, num, 3f * Globals.GlobalUIScale);
		((ImDrawListPtr)(ref foregroundDrawList)).AddCircleFilled(startPos, num2 + 1.25f * Globals.GlobalUIScale, LineOutlineColor);
		((ImDrawListPtr)(ref foregroundDrawList)).AddCircleFilled(startPos, num2, num);
		if (isMarkerVisible)
		{
			((ImDrawListPtr)(ref foregroundDrawList)).AddCircleFilled(markerPos, num3 + 1.5f * Globals.GlobalUIScale, LineOutlineColor);
			((ImDrawListPtr)(ref foregroundDrawList)).AddCircle(markerPos, num3, num, 16, 1.5f * Globals.GlobalUIScale);
			((ImDrawListPtr)(ref foregroundDrawList)).AddCircleFilled(markerPos, 1.25f * Globals.GlobalUIScale, MarkerCoreColor);
		}
		Vector2 center = ImGui.GetCenter(ImGui.GetMainViewport());
		Vector2 vector = new Vector2((markerPos.X >= center.X) ? 1f : 0f, (markerPos.Y >= center.Y) ? 1f : 0f);
		ImGui.SetNextWindowPos(markerPos, (ImGuiCond)1, vector);
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(17, 1);
		((ImU8String)(ref val)).AppendLiteral("AutoCountPlayers-");
		((ImU8String)(ref val)).AppendFormatted<uint>(chara.EntityID);
		if (!ImGui.Begin(val, (ImGuiWindowFlags)2896863))
		{
			return;
		}
		GroupDisposable val2 = ImRaii.Group();
		try
		{
			ImGuiOm.ScaledDummy(12f);
			byte[] orAdd = jobIcons.GetOrAdd(chara.ClassJob.RowId, (uint _) => new SeStringBuilder().AddIcon(chara.ClassJob.Value.ToBitmapFontIcon()).Encode());
			ImGui.SameLine();
			ReadOnlySpan<byte> readOnlySpan = orAdd;
			SeStringDrawParams val3 = default(SeStringDrawParams);
			ImGuiHelpers.SeStringWrapped(readOnlySpan, ref val3, default(ImGuiId), (ImGuiButtonFlags)1);
			ImGui.SameLine();
			ImGuiOm.TextOutlined(textColor, chara.Name + (extraInfo ?? string.Empty));
		}
		finally
		{
			((GroupDisposable)(ref val2)).Dispose();
		}
		ImGui.End();
	}

	private static (Vector2 LineEnd, Vector2 Marker) GetLinePositions(Vector2 projectedPosition, bool isInView, Vector2 viewportPosition, Vector2 viewportSize, float markerInset, float lineOverflow)
	{
		if (isInView)
		{
			return (LineEnd: projectedPosition, Marker: projectedPosition);
		}
		Vector2 vector = viewportPosition + viewportSize * 0.5f;
		Vector2 vector2 = projectedPosition - vector;
		if (!float.IsFinite(vector2.X) || !float.IsFinite(vector2.Y) || vector2.LengthSquared() < 0.001f)
		{
			vector2 = Vector2.UnitY;
		}
		float num = Math.Max(viewportSize.X * 0.5f, 1f);
		float num2 = Math.Max(viewportSize.Y * 0.5f, 1f);
		float x = ((MathF.Abs(vector2.X) > float.Epsilon) ? (num / MathF.Abs(vector2.X)) : float.MaxValue);
		float y = ((MathF.Abs(vector2.Y) > float.Epsilon) ? (num2 / MathF.Abs(vector2.Y)) : float.MaxValue);
		Vector2 vector3 = vector + vector2 * MathF.Min(x, y);
		Vector2 vector4 = Vector2.Normalize(vector2);
		return (LineEnd: vector3 + vector4 * lineOverflow, Marker: vector3 - vector4 * markerInset);
	}

	private void EnsureOverlay()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (base.Overlay == null)
		{
			base.Overlay = new Overlay(this);
			Overlay? overlay = base.Overlay;
			((Window)overlay).Flags = (ImGuiWindowFlags)(((Window)overlay).Flags & -2);
			Overlay? overlay2 = base.Overlay;
			((Window)overlay2).Flags = (ImGuiWindowFlags)(((Window)overlay2).Flags & -65);
			((Window)base.Overlay).WindowName = LanguageManager.Get("AutoCountPlayers-PlayersAroundInfo") + "###AutoCountPlayers-Overlay";
		}
	}

	unsafe static AutoCountPlayers()
	{
		Vector4 color = KnownColor.Chocolate.ToVector4();
		color.W = 0.78f;
		InfoLineColor = color.ToUInt();
		color = KnownColor.IndianRed.ToVector4();
		color.W = 0.88f;
		AlertLineColor = color.ToUInt();
		color = KnownColor.Chocolate.ToVector4();
		color.W = 0.96f;
		InfoLabelColor = color.ToUInt();
		color = KnownColor.LightCoral.ToVector4();
		color.W = 0.98f;
		AlertLabelColor = color.ToUInt();
		color = KnownColor.Black.ToVector4();
		color.W = 0.4f;
		LineOutlineColor = color.ToUInt();
		color = KnownColor.WhiteSmoke.ToVector4();
		color.W = 0.88f;
		MarkerCoreColor = color.ToUInt();
		ContentMemberListValidZones = FrozenSet.Create(new ReadOnlySpan<TerritoryIntendedUse>(Unsafe.AsPointer(ref global::_003CPrivateImplementationDetails_003E._8F6275B642574298B74367E275EA70F430F855F9CE7FB17B406C8A67F876F829), 3));
	}
}
