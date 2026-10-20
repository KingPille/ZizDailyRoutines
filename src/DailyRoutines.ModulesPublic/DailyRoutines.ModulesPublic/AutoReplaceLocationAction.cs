using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Hooking;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using InteropGenerator.Runtime;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.ImGuiOm.Widgets.Combos;
using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.Helpers;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class AutoReplaceLocationAction : ModuleBase
{
	private class Config : ModuleConfig
	{
		public float AdjustDistance = 15f;

		public HashSet<uint> BlacklistContents = new HashSet<uint>();

		public Dictionary<uint, List<Vector2>> CustomMarkers = new Dictionary<uint, List<Vector2>>();

		public bool EnableCenterArgument = true;

		public Dictionary<uint, bool> EnabledActions = new Dictionary<uint, bool>
		{
			[7439u] = true,
			[25862u] = true,
			[3569u] = true,
			[188u] = true
		};

		public Dictionary<uint, bool> EnabledPetActions = new Dictionary<uint, bool> { [3u] = true };

		public bool SendChat = true;

		public bool SendNotification = true;
	}

	private Hook<ResolvePlaceholder>? ParseActionCommandArgHook;

	private Config? config;

	private ContentSelectCombo contentSelectCombo;

	private bool isNeedToReplace;

	[CompilerGenerated]
	private static Dictionary<uint, Dictionary<MapMarker, Vector2>> _003CZoneMapMarkers_003Ek__BackingField;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoReplaceLocationActionTitle"),
		Description = LanguageManager.Get("AutoReplaceLocationActionDescription"),
		Category = ModuleCategory.Action
	};

	private static Dictionary<uint, Dictionary<MapMarker, Vector2>> ZoneMapMarkers
	{
		get
		{
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			if (_003CZoneMapMarkers_003Ek__BackingField != null)
			{
				return _003CZoneMapMarkers_003Ek__BackingField;
			}
			_003CZoneMapMarkers_003Ek__BackingField = new Dictionary<uint, Dictionary<MapMarker, Vector2>>();
			foreach (Map item in ((IEnumerable<Map>)LuminaGetter.Get<Map>()).Where(delegate(Map x)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				RowRef<TerritoryType> territoryType = ((Map)(ref x)).TerritoryType;
				if (territoryType.RowId != 0)
				{
					TerritoryType value = territoryType.Value;
					return ((TerritoryType)(ref value)).ContentFinderCondition.RowId != 0;
				}
				return false;
			}))
			{
				Map map = item;
				foreach (MapMarker mapMarker in map.GetMapMarkers())
				{
					MapMarker marker = mapMarker;
					if (((MapMarker)(ref marker)).Icon == 60442)
					{
						_003CZoneMapMarkers_003Ek__BackingField.TryAdd(((Map)(ref map)).RowId, new Dictionary<MapMarker, Vector2>());
						_003CZoneMapMarkers_003Ek__BackingField[((Map)(ref map)).RowId].TryAdd(marker, PositionHelper.TextureToWorld(marker.GetPosition(), map));
					}
				}
			}
			return _003CZoneMapMarkers_003Ek__BackingField;
		}
	}

	protected unsafe override void Init()
	{
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Expected O, but got Unknown
		contentSelectCombo = new ContentSelectCombo("Blakclist");
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		contentSelectCombo.SelectedIDs = config.BlacklistContents;
		OmenServiceBase<UseActionManager>.Instance().RegPreUseActionLocation(OnPreUseActionLocation);
		OmenServiceBase<ExecuteCommandManager>.Instance().RegPreComplexLocation(OnPreExecuteCommandComplexLocation);
		ParseActionCommandArgHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromMemberFunction<ResolvePlaceholder>(typeof(MemberFunctionPointers), "ResolvePlaceholder", new ResolvePlaceholder(ParseActionCommandArgDetour));
		ParseActionCommandArgHook.Enable();
		_ = ZoneMapMarkers;
	}

	protected override void Uninit()
	{
		OmenServiceBase<UseActionManager>.Instance().Unreg(OnPreUseActionLocation);
		OmenServiceBase<ExecuteCommandManager>.Instance().Unreg(OnPreExecuteCommandComplexLocation);
	}

	protected override void ConfigUI()
	{
		if (config != null)
		{
			DrawConfigCustom();
			ImGui.NewLine();
			DrawConfigBothers();
			ImGui.NewLine();
			DrawConfigActions();
		}
	}

	private void DrawConfigBothers()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(0, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("Settings"));
		ImGui.TextColored(ref vector, val);
		IndentDisposable val2 = ImRaii.PushIndent(1, true);
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
			if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("AutoReplaceLocationAction-EnableCenterArg")), ref config.EnableCenterArgument))
			{
				config.Save(this);
			}
			ImGuiOm.HelpMarker(LanguageManager.Get("AutoReplaceLocationAction-EnableCenterArgHelp"), 20f * Globals.GlobalUIScale, (FontAwesomeIcon)61530);
			ImGui.Spacing();
			ImGui.Spacing();
			vector = KnownColor.Chocolate.ToVector4();
			ImU8String val3 = new ImU8String(0, 1);
			((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get("AutoReplaceLocationAction-AdjustDistance"));
			ImGui.TextColored(ref vector, val3);
			IndentDisposable val4 = ImRaii.PushIndent(1, true);
			try
			{
				ImGui.SetNextItemWidth(300f * Globals.GlobalUIScale);
				ImGui.InputFloat(ImU8String.op_Implicit("###AdjustDistanceInput"), ref config.AdjustDistance, 0f, 0f, ImU8String.op_Implicit("%.1f"), (ImGuiInputTextFlags)0);
				if (ImGui.IsItemDeactivatedAfterEdit())
				{
					config.Save(this);
				}
				ImGuiOm.HelpMarker(LanguageManager.Get("AutoReplaceLocationAction-AdjustDistanceHelp"), 40f, (FontAwesomeIcon)61530);
			}
			finally
			{
				((IDisposable)val4)?.Dispose();
			}
			ImGui.Spacing();
			vector = KnownColor.Chocolate.ToVector4();
			ImU8String val5 = new ImU8String(0, 1);
			((ImU8String)(ref val5)).AppendFormatted<string>(LanguageManager.Get("AutoReplaceLocationAction-BlacklistContents"));
			ImGui.TextColored(ref vector, val5);
			val4 = ImRaii.PushIndent(1, true);
			try
			{
				ImGui.SetNextItemWidth(300f * Globals.GlobalUIScale);
				if (contentSelectCombo.DrawCheckbox())
				{
					config.BlacklistContents = contentSelectCombo.SelectedIDs.ToHashSet();
					config.Save(this);
				}
			}
			finally
			{
				((IDisposable)val4)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	private void DrawConfigActions()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(0, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("Action"));
		ImGui.TextColored(ref vector, val);
		IndentDisposable val2 = ImRaii.PushIndent(1, true);
		try
		{
			foreach (KeyValuePair<uint, bool> enabledAction in config.EnabledActions)
			{
				if (LuminaGetter.TryGetRow<Action>(enabledAction.Key, out Action item))
				{
					bool value = enabledAction.Value;
					ImU8String val3 = new ImU8String(4, 2);
					((ImU8String)(ref val3)).AppendLiteral("###");
					((ImU8String)(ref val3)).AppendFormatted<uint>(enabledAction.Key);
					((ImU8String)(ref val3)).AppendLiteral("_");
					((ImU8String)(ref val3)).AppendFormatted<string>(((object)((Action)(ref item)).Name/*cast due to constrained. prefix*/).ToString());
					if (ImGui.Checkbox(val3, ref value))
					{
						config.EnabledActions[enabledAction.Key] = value;
						config.Save(this);
					}
					ImGui.SameLine();
					ImGuiOm.TextImage(((object)((Action)(ref item)).Name/*cast due to constrained. prefix*/).ToString(), ImageHelper.GetGameIcon(((Action)(ref item)).Icon).Handle, Globals.ScaledVector2(20f));
				}
			}
			foreach (KeyValuePair<uint, bool> enabledPetAction in config.EnabledPetActions)
			{
				if (LuminaGetter.TryGetRow<PetAction>(enabledPetAction.Key, out PetAction item2))
				{
					bool value2 = enabledPetAction.Value;
					ImU8String val4 = new ImU8String(4, 2);
					((ImU8String)(ref val4)).AppendLiteral("###");
					((ImU8String)(ref val4)).AppendFormatted<uint>(enabledPetAction.Key);
					((ImU8String)(ref val4)).AppendLiteral("_");
					((ImU8String)(ref val4)).AppendFormatted<string>(((object)((PetAction)(ref item2)).Name/*cast due to constrained. prefix*/).ToString());
					if (ImGui.Checkbox(val4, ref value2))
					{
						config.EnabledPetActions[enabledPetAction.Key] = value2;
						config.Save(this);
					}
					ImGui.SameLine();
					ImGuiOm.TextImage(((object)((PetAction)(ref item2)).Name/*cast due to constrained. prefix*/).ToString(), ImageHelper.GetGameIcon((uint)((PetAction)(ref item2)).Icon).Handle, Globals.ScaledVector2(20f));
				}
			}
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	private unsafe void DrawConfigCustom()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		AgentMap* agent = AgentMap.Instance();
		if (agent == null)
		{
			return;
		}
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(0, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("AutoReplaceLocationAction-CenterPointData"));
		ImGui.TextColored(ref vector, val);
		IndentDisposable val2 = ImRaii.PushIndent(1, true);
		try
		{
			TerritoryType territoryTypeData = GameState.TerritoryTypeData;
			int num;
			Map mapData;
			PlaceName value;
			string text;
			if (((TerritoryType)(ref territoryTypeData)).RowId != 0)
			{
				num = ((((TerritoryType)(ref territoryTypeData)).ContentFinderCondition.RowId != 0) ? 1 : 0);
				if (num != 0)
				{
					mapData = GameState.MapData;
					value = ((Map)(ref mapData)).PlaceName.Value;
					text = ((object)((PlaceName)(ref value)).Name/*cast due to constrained. prefix*/).ToString();
					goto IL_00b7;
				}
			}
			else
			{
				num = 0;
			}
			text = string.Empty;
			goto IL_00b7;
			IL_00b7:
			string text2 = text;
			string? text3;
			if (num == 0)
			{
				text3 = string.Empty;
			}
			else
			{
				mapData = GameState.MapData;
				value = ((Map)(ref mapData)).PlaceNameSub.Value;
				text3 = ((object)((PlaceName)(ref value)).Name/*cast due to constrained. prefix*/).ToString();
			}
			string text4 = text3;
			DisabledDisposable val3 = ImRaii.Disabled(num == 0);
			try
			{
				ImGui.AlignTextToFramePadding();
				vector = KnownColor.Chocolate.ToVector4();
				ImU8String val4 = new ImU8String(0, 1);
				((ImU8String)(ref val4)).AppendFormatted<string>(LanguageManager.Get("CurrentMap"));
				ImGui.TextColored(ref vector, val4);
				IndentDisposable val5 = ImRaii.PushIndent(1, true);
				try
				{
					ImGui.TextUnformatted(ImU8String.op_Implicit(text2 + (string.IsNullOrEmpty(text4) ? string.Empty : (" / " + text4))));
					ImU8String val6 = new ImU8String(0, 1);
					((ImU8String)(ref val6)).AppendFormatted<string>(LanguageManager.Get("OpenMap"));
					if (ImGui.Button(val6, default(Vector2)))
					{
						((AgentMap)agent).OpenMap(GameState.Map, GameState.TerritoryType, (string)null, (MapType)6);
						MarkCenterPoint();
					}
					ImGui.SameLine();
					ImU8String val7 = new ImU8String(0, 1);
					((ImU8String)(ref val7)).AppendFormatted<string>(LanguageManager.Get("AutoReplaceLocationAction-ClearMarks"));
					if (ImGui.Button(val7, default(Vector2)))
					{
						ClearCenterPoint();
					}
				}
				finally
				{
					((IDisposable)val5)?.Dispose();
				}
				ImGui.Spacing();
				vector = KnownColor.Chocolate.ToVector4();
				ImU8String val8 = new ImU8String(0, 1);
				((ImU8String)(ref val8)).AppendFormatted<string>(LanguageManager.Get("AutoReplaceLocationAction-CustomCenterPoint"));
				ImGui.TextColored(ref vector, val8);
				val5 = ImRaii.PushIndent(1, true);
				try
				{
					DisabledDisposable val9 = ImRaii.Disabled(((AgentMap)agent).FlagMarkerCount == 0 || ((AgentMap)agent).FlagMapMarkers[0].MapId != GameState.Map);
					try
					{
						if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("AutoReplaceLocationAction-AddFlagMarker")), default(Vector2)))
						{
							config.CustomMarkers.TryAdd(GameState.Map, new List<Vector2>());
							config.CustomMarkers[GameState.Map].Add(new Vector2(((AgentMap)agent).FlagMapMarkers[0].XFloat, ((AgentMap)agent).FlagMapMarkers[0].YFloat));
							config.Save(this);
							((AgentMap)agent).FlagMarkerCount = 0;
							MarkCenterPoint();
						}
					}
					finally
					{
						((IDisposable)val9)?.Dispose();
					}
					IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
					val9 = ImRaii.Disabled(localPlayer == null);
					try
					{
						ImGui.SameLine();
						if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("AutoReplaceLocationAction-AddPlayerPosition")), default(Vector2)))
						{
							config.CustomMarkers.TryAdd(GameState.Map, new List<Vector2>());
							config.CustomMarkers[GameState.Map].Add(localPlayer.Position.ToVector2());
							config.Save(this);
							MarkCenterPoint();
						}
					}
					finally
					{
						((IDisposable)val9)?.Dispose();
					}
					ImGui.SameLine();
					if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("DeleteAll")), default(Vector2)))
					{
						config.CustomMarkers.TryAdd(GameState.Map, new List<Vector2>());
						config.CustomMarkers[GameState.Map].Clear();
						config.Save(this);
						MarkCenterPoint();
					}
				}
				finally
				{
					((IDisposable)val5)?.Dispose();
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
		unsafe void ClearCenterPoint()
		{
			((AgentMap)agent).ResetMapMarkers();
			((AgentMap)agent).ResetMiniMapMarkers();
		}
		unsafe void MarkCenterPoint()
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			ClearCenterPoint();
			if (ZoneMapMarkers.TryGetValue(GameState.Map, out Dictionary<MapMarker, Vector2> value2))
			{
				value2.ForEach(delegate(KeyValuePair<MapMarker, Vector2> keyValuePair)
				{
					//IL_0021: Unknown result type (might be due to invalid IL or missing references)
					//IL_0026: Unknown result type (might be due to invalid IL or missing references)
					//IL_003a: Unknown result type (might be due to invalid IL or missing references)
					//IL_003f: Unknown result type (might be due to invalid IL or missing references)
					Vector3 vector3 = keyValuePair.Value.ToVector3(0f);
					ref float x2 = ref vector3.X;
					float num4 = x2;
					Map mapData3 = GameState.MapData;
					x2 = num4 + (float)((Map)(ref mapData3)).OffsetX;
					ref float z2 = ref vector3.Z;
					float num5 = z2;
					mapData3 = GameState.MapData;
					z2 = num5 + (float)((Map)(ref mapData3)).OffsetY;
					((AgentMap)agent).AddMapMarker(vector3, 60931u, 0, (byte*)null, (byte)3, (byte)0);
				});
			}
			Vector3 vector2 = PositionHelper.MapToWorld(new Vector2(6.125f), GameState.MapData).ToVector3(0f);
			ref float x = ref vector2.X;
			float num2 = x;
			Map mapData2 = GameState.MapData;
			x = num2 + (float)((Map)(ref mapData2)).OffsetX;
			ref float z = ref vector2.Z;
			float num3 = z;
			mapData2 = GameState.MapData;
			z = num3 + (float)((Map)(ref mapData2)).OffsetY;
			((AgentMap)agent).AddMapMarker(vector2, 60932u, 0, (byte*)null, (byte)3, (byte)0);
			Dictionary<uint, List<Vector2>> customMarkers = config.CustomMarkers;
			mapData2 = GameState.MapData;
			if (customMarkers.TryGetValue(((Map)(ref mapData2)).RowId, out List<Vector2> value3))
			{
				value3.ForEach(delegate(Vector2 vector4)
				{
					//IL_0019: Unknown result type (might be due to invalid IL or missing references)
					//IL_001e: Unknown result type (might be due to invalid IL or missing references)
					//IL_0032: Unknown result type (might be due to invalid IL or missing references)
					//IL_0037: Unknown result type (might be due to invalid IL or missing references)
					Vector3 vector3 = vector4.ToVector3(0f);
					ref float x2 = ref vector3.X;
					float num4 = x2;
					Map mapData3 = GameState.MapData;
					x2 = num4 + (float)((Map)(ref mapData3)).OffsetX;
					ref float z2 = ref vector3.Z;
					float num5 = z2;
					mapData3 = GameState.MapData;
					z2 = num5 + (float)((Map)(ref mapData3)).OffsetY;
					((AgentMap)agent).AddMapMarker(vector3, 60933u, 0, (byte*)null, (byte)3, (byte)0);
				});
			}
			((AgentMap)agent).OpenMap(GameState.Map, GameState.TerritoryType, (string)null, (MapType)6);
		}
	}

	private void OnPreUseActionLocation(ref bool isPrevented, ref ActionType type, ref uint actionID, ref ulong targetID, ref Vector3 location, ref uint extraParam, ref byte a7)
	{
		if ((int)type != 1)
		{
			return;
		}
		if (!config.EnabledActions.TryGetValue(actionID, out var value) || (!value && !isNeedToReplace))
		{
			isNeedToReplace = false;
			return;
		}
		isNeedToReplace = false;
		if (!config.BlacklistContents.Contains(GameState.ContentFinderCondition))
		{
			if (!ZoneMapMarkers.TryGetValue(GameState.Map, out Dictionary<MapMarker, Vector2> value2))
			{
				value2 = new Dictionary<MapMarker, Vector2>();
			}
			Vector3 sourceLocation = location;
			if (HandleCustomLocation(ref sourceLocation) || HandleMapLocation(value2, ref sourceLocation) || HandlePresetCenterLocation(ref sourceLocation))
			{
				location = sourceLocation;
				NotifyLocationRedirect(sourceLocation);
			}
		}
	}

	private void OnPreExecuteCommandComplexLocation(ref bool isPrevented, ref ExecuteCommandComplexFlag command, ref Vector3 location, ref uint param1, ref uint param2, ref uint param3, ref uint param4)
	{
		if (command != ExecuteCommandComplexFlag.PetAction || param1 != 3)
		{
			return;
		}
		if (!config.EnabledPetActions.TryGetValue(3u, out var value) || (!value && !isNeedToReplace))
		{
			isNeedToReplace = false;
			return;
		}
		isNeedToReplace = false;
		if (!config.BlacklistContents.Contains(GameState.ContentFinderCondition))
		{
			if (!ZoneMapMarkers.TryGetValue(GameState.Map, out Dictionary<MapMarker, Vector2> value2))
			{
				value2 = new Dictionary<MapMarker, Vector2>();
			}
			Vector3 sourceLocation = location;
			if (HandleCustomLocation(ref sourceLocation) || HandleMapLocation(value2, ref sourceLocation) || HandlePresetCenterLocation(ref sourceLocation))
			{
				location = sourceLocation;
				isPrevented = true;
				OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommandComplexLocation(ExecuteCommandComplexFlag.PetAction, sourceLocation, 3u);
				NotifyLocationRedirect(location);
			}
		}
	}

	private unsafe GameObject* ParseActionCommandArgDetour(PronounModule* manager, CStringPointer placeholder, byte unknown0, byte unknown1, bool unknown2)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		GameObject* result = ParseActionCommandArgHook.Original.Invoke(manager, placeholder, unknown0, unknown1, unknown2);
		if (!config.EnableCenterArgument || config.BlacklistContents.Contains(GameState.ContentFinderCondition))
		{
			return result;
		}
		if (!((object)(*(CStringPointer*)(&placeholder))/*cast due to constrained. prefix*/).ToString().Equals("<center>"))
		{
			return result;
		}
		isNeedToReplace = true;
		return (GameObject*)Control.GetLocalPlayer();
	}

	private bool HandleCustomLocation(ref Vector3 sourceLocation)
	{
		if (!config.CustomMarkers.TryGetValue(GameState.Map, out List<Vector2> value))
		{
			return false;
		}
		Vector3 candidateLocation = value.MinBy((Vector2 x) => Vector2.DistanceSquared(IOmenDalamudService<IObjectTable>.Instance().LocalPlayer.Position.ToVector2(), x)).ToPlayerHeight();
		return UpdateLocationIfClose(ref sourceLocation, candidateLocation);
	}

	private bool HandleMapLocation(Dictionary<MapMarker, Vector2>? markers, ref Vector3 sourceLocation)
	{
		if (markers == null || markers.Count <= 0)
		{
			return false;
		}
		Vector3 sourceCopy = sourceLocation;
		Vector3? vector = ((IEnumerable<Vector2>)markers.Values).Select((Func<Vector2, Vector3?>)((Vector2 x) => x.ToPlayerHeight())).FirstOrDefault((Vector3? x) => x.HasValue && Vector3.DistanceSquared(x.Value, sourceCopy) < 900f);
		if (!vector.HasValue)
		{
			return false;
		}
		return UpdateLocationIfClose(ref sourceLocation, vector.Value);
	}

	private unsafe bool HandlePresetCenterLocation(ref Vector3 sourceLocation)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		ContentFinderCondition item;
		bool flag = !LuminaGetter.TryGetRow<ContentFinderCondition>((uint)((GameMain)GameMain.Instance()).CurrentContentFinderConditionId, out item);
		if (!flag)
		{
			uint rowId = ((ContentFinderCondition)(ref item)).ContentType.RowId;
			bool flag2 = rowId - 4 <= 1;
			flag = !flag2;
		}
		if (flag || !LuminaGetter.TryGetRow<Map>(GameState.Map, out Map item2))
		{
			return false;
		}
		Vector3 candidateLocation = PositionHelper.TextureToWorld(new Vector2(1024f), item2).ToPlayerHeight();
		return UpdateLocationIfClose(ref sourceLocation, candidateLocation);
	}

	private bool UpdateLocationIfClose(ref Vector3 sourceLocation, Vector3 candidateLocation)
	{
		if (Vector3.DistanceSquared(sourceLocation, candidateLocation) > config.AdjustDistance * config.AdjustDistance)
		{
			return false;
		}
		sourceLocation = candidateLocation;
		return true;
	}

	private void NotifyLocationRedirect(Vector3 location)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		if (config.SendChat)
		{
			Vector2 vector = PositionHelper.WorldToMap(location.ToVector2(), GameState.MapData);
			OmenServiceBase<NotifyHelper>.Instance().Chat(LanguageManager.GetSe("AutoReplaceLocationAction-RedirectMessage", SeString.CreateMapLink(GameState.TerritoryType, GameState.Map, vector.X, vector.Y, 0.05f)));
		}
		if (config.SendNotification)
		{
			OmenServiceBase<NotifyHelper>.Instance().NotificationSuccess(LanguageManager.Get("AutoReplaceLocationAction-RedirectMessage", $"[{location.X:F1}, {location.Y:F1}, {location.Z:F1}]"));
		}
	}
}
