using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Interface.Windows;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using DailyRoutines.Verification;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using OmenTools.Dalamud;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.ModulesPublic.Interface;

public class AutoMarkAetherCurrents : ModuleBase
{
	private class ZoneAetherCurrentInfo
	{
		private const string BACKGROUND_ULD_PATH = "ui/uld/FlyingPermission.uld";

		[CompilerGenerated]
		private IDalamudTextureWrap? _003CBackgroundTexture_003Ek__BackingField;

		public int Version { get; init; }

		public int Counter { get; init; }

		public uint Zone { get; init; }

		public IDalamudTextureWrap? BackgroundTexture
		{
			get
			{
				if (_003CBackgroundTexture_003Ek__BackingField != null)
				{
					return _003CBackgroundTexture_003Ek__BackingField;
				}
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(31, 1);
				defaultInterpolatedStringHandler.AppendLiteral("ui/uld/FlyingPermission");
				defaultInterpolatedStringHandler.AppendFormatted((Version == 0) ? string.Empty : ((object)(Version + 1)));
				defaultInterpolatedStringHandler.AppendLiteral("_hr1.tex");
				string text = defaultInterpolatedStringHandler.ToStringAndClear();
				_003CBackgroundTexture_003Ek__BackingField = DalamudServiceExtension.Instance().UiBuilder.LoadUld("ui/uld/FlyingPermission.uld").LoadTexturePart(text, Counter);
				return _003CBackgroundTexture_003Ek__BackingField;
			}
		}

		public List<AetherCurrentPoint> QuestPoints { get; init; } = new List<AetherCurrentPoint>();

		public List<AetherCurrentPoint> NormalPoints { get; init; } = new List<AetherCurrentPoint>();

		private ZoneAetherCurrentInfo(uint version, uint counter, uint zoneID)
		{
			Version = (int)version;
			Counter = (int)counter;
			Zone = zoneID;
		}

		public static ZoneAetherCurrentInfo? Parse(uint zoneID, uint counter, RowRef<AetherCurrent>[] acArray)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			if (!LuminaGetter.TryGetRow<TerritoryType>(zoneID, out TerritoryType item))
			{
				return null;
			}
			uint rowId = ((TerritoryType)(ref item)).ExVersion.RowId;
			if (rowId == 0)
			{
				return null;
			}
			rowId--;
			ZoneAetherCurrentInfo zoneAetherCurrentInfo = new ZoneAetherCurrentInfo(rowId, counter, zoneID);
			foreach (RowRef<AetherCurrent> data in acArray)
			{
				AetherCurrentPoint aetherCurrentPoint = AetherCurrentPoint.Parse(zoneID, data);
				if (!(aetherCurrentPoint == null))
				{
					switch (aetherCurrentPoint.Type)
					{
					case PointType.Normal:
						zoneAetherCurrentInfo.NormalPoints.Add(aetherCurrentPoint);
						break;
					case PointType.Quest:
						zoneAetherCurrentInfo.QuestPoints.Add(aetherCurrentPoint);
						break;
					}
				}
			}
			return zoneAetherCurrentInfo;
		}

		public void Draw(AutoMarkAetherCurrents module)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			ImU8String val = default(ImU8String);
			((ImU8String)(ref val))._002Ector(0, 1);
			((ImU8String)(ref val)).AppendFormatted<string>(ToString());
			ChildDisposable val2 = ImRaii.Child(val, ChildSize, true, (ImGuiWindowFlags)24);
			try
			{
				if (ChildDisposable.op_Implicit(val2))
				{
					DrawBackgroundImage(BackgroundTexture);
					DrawZoneName(LuminaWrapper.GetZonePlaceName(Zone));
					DrawAetherCurrentProgress(module);
				}
			}
			finally
			{
				((ChildDisposable)(ref val2)).Dispose();
			}
			HandleInteraction(Zone);
		}

		private void DrawAetherCurrentProgress(AutoMarkAetherCurrents module)
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			using (OmenServiceBase<FontManager>.Instance().UIFont80.Push())
			{
				float num = 2f * ImGui.GetTextLineHeightWithSpacing();
				ImGuiStylePtr style = ImGui.GetStyle();
				float num2 = num + 2f * ((ImGuiStylePtr)(ref style)).FramePadding.Y;
				ImGui.SetCursorPos(new Vector2(ImGui.GetCursorPosX(), ImGui.GetContentRegionMax().Y - num2));
				GroupDisposable val = ImRaii.Group();
				try
				{
					if (QuestPoints.Count > 0)
					{
						GroupDisposable val2 = ImRaii.Group();
						try
						{
							ImGui.AlignTextToFramePadding();
							ImGui.TextUnformatted(ImU8String.op_Implicit("Q  "));
							QuestPoints.ForEach(delegate(AetherCurrentPoint x)
							{
								x.Draw(module);
							});
						}
						finally
						{
							((GroupDisposable)(ref val2)).Dispose();
						}
					}
					if (NormalPoints.Count <= 0)
					{
						return;
					}
					GroupDisposable val3 = ImRaii.Group();
					try
					{
						ImGui.AlignTextToFramePadding();
						ImGui.TextUnformatted(ImU8String.op_Implicit("N  "));
						NormalPoints.ForEach(delegate(AetherCurrentPoint x)
						{
							x.Draw(module);
						});
					}
					finally
					{
						((GroupDisposable)(ref val3)).Dispose();
					}
				}
				finally
				{
					((GroupDisposable)(ref val)).Dispose();
				}
			}
		}

		private static void DrawBackgroundImage(IDalamudTextureWrap? backgroundImage)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			if (backgroundImage != null)
			{
				Vector2 cursorPos = ImGui.GetCursorPos();
				ImGui.SetCursorPos(cursorPos - Globals.ScaledVector2(10f, 4f));
				ImGui.Image(backgroundImage.Handle, ImGui.GetWindowSize() + Globals.ScaledVector2(10f, 4f));
				ImGui.SetCursorPos(cursorPos);
			}
		}

		private static void DrawZoneName(string name)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			ImGui.SetWindowFontScale(1.05f);
			ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 8f * Globals.GlobalUIScale);
			ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 4f * Globals.GlobalUIScale);
			ImGui.TextUnformatted(ImU8String.op_Implicit(name));
			ImGui.SetWindowFontScale(1f);
		}

		private unsafe static void HandleInteraction(uint zone)
		{
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			if (!LuminaGetter.TryGetRow<TerritoryType>(zone, out TerritoryType item))
			{
				return;
			}
			if (ImGui.IsItemHovered())
			{
				ImGui.SetMouseCursor((ImGuiMouseCursor)7);
			}
			if (ImGui.IsItemClicked((ImGuiMouseButton)0))
			{
				AgentMap* ptr = AgentMap.Instance();
				if (((AgentInterface)(&((AgentMap)ptr).AgentInterface)).IsAgentActive() && ((AgentMap)ptr).SelectedMapId == ((TerritoryType)(ref item)).Map.RowId)
				{
					((AgentInterface)(&((AgentMap)ptr).AgentInterface)).Hide();
				}
				else
				{
					((AgentMap)ptr).OpenMap(((TerritoryType)(ref item)).Map.RowId, ((TerritoryType)(ref item)).RowId, LuminaWrapper.GetAddonText(2448u), (MapType)1);
				}
			}
		}

		public override string ToString()
		{
			return $"ZoneAetherCurrentInfo_Version{Version + 3}.0_{Zone}_{Counter}";
		}
	}

	private sealed class AetherCurrentPoint : IEquatable<AetherCurrentPoint>
	{
		public static Dictionary<uint, bool> UnlockStates { get; private set; } = new Dictionary<uint, bool>();

		public PointType Type { get; init; }

		public uint Zone { get; init; }

		public uint DataID { get; init; }

		public uint ObjectID { get; init; }

		public Vector3 Position { get; init; }

		public TerritoryType RealTerritory { get; init; }

		public Map RealMap { get; init; }

		private static Dictionary<uint, uint> EObjDataSheet { get; } = ((IEnumerable<EObj>)LuminaGetter.Get<EObj>()).Where(delegate(EObj x)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			RowRef data = ((EObj)(ref x)).Data;
			return ((RowRef)(ref data)).RowId != 0;
		}).DistinctBy(delegate(EObj x)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			RowRef data = ((EObj)(ref x)).Data;
			return ((RowRef)(ref data)).RowId;
		}).ToDictionary(delegate(EObj x)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			RowRef data = ((EObj)(ref x)).Data;
			return ((RowRef)(ref data)).RowId;
		}, (EObj x) => ((EObj)(ref x)).RowId);

		private static Dictionary<uint, Vector3> LevelSheet { get; } = ((IEnumerable<Level>)LuminaGetter.Get<Level>()).Where(delegate(Level x)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			RowRef val = ((Level)(ref x)).Object;
			return ((RowRef)(ref val)).Is<EObj>();
		}).DistinctBy(delegate(Level x)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			RowRef val = ((Level)(ref x)).Object;
			return ((RowRef)(ref val)).RowId;
		}).ToDictionary(delegate(Level x)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			RowRef val = ((Level)(ref x)).Object;
			return ((RowRef)(ref val)).RowId;
		}, (Level x) => x.GetPosition());

		private AetherCurrentPoint(PointType type, uint zone, uint dataID, uint objectID, Vector3 position)
		{
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			Type = type;
			Zone = zone;
			DataID = dataID;
			ObjectID = objectID;
			Position = position;
			TerritoryType? val;
			Quest valueOrDefault;
			Level valueOrDefault2;
			if (Type != PointType.Normal)
			{
				Quest? row = LuminaGetter.GetRow<Quest>(ObjectID);
				if (!row.HasValue)
				{
					val = null;
				}
				else
				{
					valueOrDefault = row.GetValueOrDefault();
					Level? valueNullable = ((Quest)(ref valueOrDefault)).IssuerLocation.ValueNullable;
					if (!valueNullable.HasValue)
					{
						val = null;
					}
					else
					{
						valueOrDefault2 = valueNullable.GetValueOrDefault();
						val = ((Level)(ref valueOrDefault2)).Territory.Value;
					}
				}
			}
			else
			{
				val = LuminaGetter.GetRow<TerritoryType>(Zone);
			}
			TerritoryType? val2 = val;
			RealTerritory = val2.GetValueOrDefault();
			Map? val3;
			if (Type != PointType.Normal)
			{
				Quest? row = LuminaGetter.GetRow<Quest>(ObjectID);
				if (!row.HasValue)
				{
					val3 = null;
				}
				else
				{
					valueOrDefault = row.GetValueOrDefault();
					Level? valueNullable = ((Quest)(ref valueOrDefault)).IssuerLocation.ValueNullable;
					if (!valueNullable.HasValue)
					{
						val3 = null;
					}
					else
					{
						valueOrDefault2 = valueNullable.GetValueOrDefault();
						val3 = ((Level)(ref valueOrDefault2)).Map.Value;
					}
				}
			}
			else
			{
				TerritoryType realTerritory = RealTerritory;
				val3 = ((TerritoryType)(ref realTerritory)).Map.Value;
			}
			Map? val4 = val3;
			RealMap = val4.GetValueOrDefault();
		}

		public bool Equals(AetherCurrentPoint? other)
		{
			if ((object)other == null)
			{
				return false;
			}
			if ((object)this == other)
			{
				return true;
			}
			return DataID == other.DataID;
		}

		public static AetherCurrentPoint? Parse(uint zone, RowRef<AetherCurrent> data)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			if (!data.ValueNullable.HasValue)
			{
				return null;
			}
			if (zone == 156)
			{
				return null;
			}
			AetherCurrent value = data.Value;
			if (((AetherCurrent)(ref value)).RowId == 0)
			{
				return null;
			}
			if (((AetherCurrent)(ref value)).Quest.RowId != 0)
			{
				uint rowId = data.RowId;
				uint rowId2 = ((AetherCurrent)(ref value)).Quest.RowId;
				Quest value2 = ((AetherCurrent)(ref value)).Quest.Value;
				return new AetherCurrentPoint(PointType.Quest, zone, rowId, rowId2, ((Quest)(ref value2)).IssuerLocation.Value.GetPosition());
			}
			if (!EObjDataSheet.TryGetValue(((AetherCurrent)(ref value)).RowId, out var value3))
			{
				return null;
			}
			if (!LevelSheet.TryGetValue(value3, out var value4))
			{
				return null;
			}
			return new AetherCurrentPoint(PointType.Normal, zone, data.RowId, value3, value4);
		}

		public unsafe static void RefreshUnlockStates()
		{
			foreach (uint item in ((IEnumerable<AetherCurrent>)LuminaGetter.Get<AetherCurrent>()).Select((AetherCurrent x) => ((AetherCurrent)(ref x)).RowId).ToList())
			{
				UnlockStates[item] = ((PlayerState)PlayerState.Instance()).IsAetherCurrentUnlocked(item);
			}
		}

		public unsafe void Draw(AutoMarkAetherCurrents module)
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			if (!UnlockStates.TryGetValue(DataID, out var value))
			{
				return;
			}
			ImU8String val = default(ImU8String);
			((ImU8String)(ref val))._002Ector(0, 1);
			((ImU8String)(ref val)).AppendFormatted<uint>(DataID);
			IdDisposable val2 = ImRaii.PushId(val, true);
			try
			{
				ImGui.SameLine();
				if (!module.manualMode)
				{
					ImU8String val3 = new ImU8String(3, 1);
					((ImU8String)(ref val3)).AppendLiteral("###");
					((ImU8String)(ref val3)).AppendFormatted<uint>(DataID);
					ImGui.Checkbox(val3, ref value);
				}
				else
				{
					value = module.selectedAetherCurrents.Contains(this);
					ImU8String val4 = new ImU8String(3, 1);
					((ImU8String)(ref val4)).AppendLiteral("###");
					((ImU8String)(ref val4)).AppendFormatted<uint>(DataID);
					if (ImGui.Checkbox(val4, ref value))
					{
						if (module.selectedAetherCurrents.Contains(this))
						{
							module.selectedAetherCurrents.Remove(this);
						}
						else
						{
							module.selectedAetherCurrents.Add(this);
						}
					}
				}
				if (ImGui.IsItemHovered())
				{
					ImGui.BeginTooltip();
					DrawBasicInfo();
					ImGui.EndTooltip();
				}
				ImU8String val5 = new ImU8String(3, 1);
				((ImU8String)(ref val5)).AppendLiteral("###");
				((ImU8String)(ref val5)).AppendFormatted<uint>(DataID);
				PopupDisposable val6 = ImRaii.ContextPopupItem(val5);
				try
				{
					if (!val6)
					{
						return;
					}
					if (ImGui.IsWindowAppearing())
					{
						PlaceFlag();
					}
					DrawBasicInfo();
					ImGui.Separator();
					ImGui.Spacing();
					DisabledDisposable val7 = ImRaii.Disabled(!IsEligibleForTeleporting);
					try
					{
						ImU8String val8 = new ImU8String(4, 1);
						((ImU8String)(ref val8)).AppendLiteral("    ");
						((ImU8String)(ref val8)).AppendFormatted<string>(LanguageManager.Get("AutoMarkAetherCurrents-TeleportTo"));
						if (ImGui.MenuItem(val8, false, true))
						{
							TeleportTo();
						}
					}
					finally
					{
						((IDisposable)val7)?.Dispose();
					}
					ImGui.Separator();
					val7 = ImRaii.Disabled(!DalamudServiceExtension.Instance().IsPluginEnabled("vnavmesh"));
					try
					{
						ImU8String val9 = new ImU8String(15, 1);
						((ImU8String)(ref val9)).AppendLiteral("    ");
						((ImU8String)(ref val9)).AppendFormatted<string>(LanguageManager.Get("AutoMarkAetherCurrents-MoveTo"));
						((ImU8String)(ref val9)).AppendLiteral(" (vnavmesh)");
						if (ImGui.MenuItem(val9, false, true))
						{
							MoveTo(module.TaskHelper);
						}
					}
					finally
					{
						((IDisposable)val7)?.Dispose();
					}
					ImGui.Separator();
					ImU8String val10 = new ImU8String(4, 1);
					((ImU8String)(ref val10)).AppendLiteral("    ");
					((ImU8String)(ref val10)).AppendFormatted<string>(LanguageManager.Get("AutoMarkAetherCurrents-SendLocation"));
					if (ImGui.MenuItem(val10, false, true))
					{
						AgentMap* intPtr = AgentMap.Instance();
						TerritoryType realTerritory = RealTerritory;
						uint rowId = ((TerritoryType)(ref realTerritory)).RowId;
						realTerritory = RealTerritory;
						((AgentMap)intPtr).SetFlagMapMarker(rowId, ((TerritoryType)(ref realTerritory)).Map.RowId, Position, 60561u);
						OmenServiceBase<ChatManager>.Instance().SendMessage("<flag>");
					}
				}
				finally
				{
					((PopupDisposable)(ref val6)).Dispose();
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
			void DrawBasicInfo()
			{
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				//IL_007b: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
				//IL_0108: Unknown result type (might be due to invalid IL or missing references)
				//IL_011b: Unknown result type (might be due to invalid IL or missing references)
				if (UnlockStates.TryGetValue(DataID, out var value2))
				{
					Vector4 vector = KnownColor.Chocolate.ToVector4();
					ImU8String val11 = default(ImU8String);
					((ImU8String)(ref val11))._002Ector(1, 1);
					((ImU8String)(ref val11)).AppendFormatted<string>(LanguageManager.Get("AutoMarkAetherCurrents-DiscoverInfo"));
					((ImU8String)(ref val11)).AppendLiteral(":");
					ImGui.TextColored(ref vector, val11);
					ImGui.SameLine();
					vector = (value2 ? ImGuiColors.HealerGreen : ImGuiColors.DPSRed);
					ImGui.TextColored(ref vector, ImU8String.op_Implicit(value2 ? LanguageManager.Get("AutoMarkAetherCurrents-Discovered") : LanguageManager.Get("AutoMarkAetherCurrents-NotDiscovered")));
					if (Type == PointType.Quest && LuminaGetter.TryGetRow<Quest>(ObjectID, out Quest item))
					{
						string? text = ((object)((Quest)(ref item)).Name/*cast due to constrained. prefix*/).ToString();
						ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
						GameIconLookup val12 = GameIconLookup.op_Implicit(71141);
						ISharedImmediateTexture fromGameIcon = obj.GetFromGameIcon(ref val12);
						ImGui.AlignTextToFramePadding();
						vector = KnownColor.Chocolate.ToVector4();
						ImU8String val13 = default(ImU8String);
						((ImU8String)(ref val13))._002Ector(1, 1);
						((ImU8String)(ref val13)).AppendFormatted<string>(LanguageManager.Get("Quest"));
						((ImU8String)(ref val13)).AppendLiteral(":");
						ImGui.TextColored(ref vector, val13);
						ImGui.SameLine();
						ImGuiOm.TextImage(text, fromGameIcon.GetWrapOrEmpty().Handle, new Vector2(ImGui.GetTextLineHeightWithSpacing()));
					}
				}
			}
		}

		public unsafe void PlaceFieldMarker(bool isLocal, uint index)
		{
			FieldMarkerPoint point = (FieldMarkerPoint)index;
			if (isLocal)
			{
				(*MarkingController.Instance()).PlaceFieldMarkerLocal(in point, Position);
			}
			else
			{
				(*MarkingController.Instance()).PlaceFieldMarkerOnline(in point, Position);
			}
		}

		public unsafe void PlaceFlag()
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			AgentMap* ptr = AgentMap.Instance();
			Map realMap = RealMap;
			((AgentMap)ptr).SelectedMapId = ((Map)(ref realMap)).RowId;
			TerritoryType realTerritory = RealTerritory;
			uint rowId = ((TerritoryType)(ref realTerritory)).RowId;
			realMap = RealMap;
			((AgentMap)ptr).SetFlagMapMarker(rowId, ((Map)(ref realMap)).RowId, Position, 60561u);
			if (!((AgentMap)ptr).IsAgentActive())
			{
				((AgentMap)ptr).Show();
			}
			realMap = RealMap;
			uint rowId2 = ((Map)(ref realMap)).RowId;
			realTerritory = RealTerritory;
			((AgentMap)ptr).OpenMap(rowId2, ((TerritoryType)(ref realTerritory)).RowId, LuminaWrapper.GetAddonText(2448u), (MapType)1);
		}

		public void TeleportTo()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			MovementManager movementManager = OmenServiceBase<MovementManager>.Instance();
			TerritoryType realTerritory = RealTerritory;
			movementManager.TPSmart_BetweenZone(((TerritoryType)(ref realTerritory)).RowId, Position);
		}

		public void MoveTo(TaskHelper? taskHelper)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			if (taskHelper == null)
			{
				return;
			}
			uint territoryType = GameState.TerritoryType;
			TerritoryType realTerritory = RealTerritory;
			if (territoryType != ((TerritoryType)(ref realTerritory)).RowId)
			{
				taskHelper.Enqueue(delegate
				{
					//IL_000b: Unknown result type (might be due to invalid IL or missing references)
					//IL_0010: Unknown result type (might be due to invalid IL or missing references)
					AetheryteRecordManager aetheryteRecordManager = OmenServiceBase<AetheryteRecordManager>.Instance();
					TerritoryType realTerritory2 = RealTerritory;
					return aetheryteRecordManager.GetNearestAetheryte(((TerritoryType)(ref realTerritory2)).RowId, Position)?.TeleportTo();
				});
			}
			taskHelper.Enqueue(delegate
			{
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				uint territoryType2 = GameState.TerritoryType;
				TerritoryType realTerritory2 = RealTerritory;
				return territoryType2 == ((TerritoryType)(ref realTerritory2)).RowId && UIModuleExtension.IsScreenReady();
			});
			taskHelper.Enqueue(delegate
			{
				if (!IConditionExtension.get_IsOnMount(DalamudServiceExtension.Instance<ICondition>()))
				{
					taskHelper.Enqueue(() => OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)5, 9u, 3758096384uL, 0u, (UseActionMode)0), null, 0, null, null, null, null, 1);
					taskHelper.Enqueue(() => IConditionExtension.get_IsOnMount(DalamudServiceExtension.Instance<ICondition>()), null, 0, null, null, null, null, 1);
				}
			});
			taskHelper.Enqueue(() => vnavmeshIPC.PathfindAndMoveTo(Position, fly: false));
		}

		public override bool Equals(object? obj)
		{
			if (this != obj)
			{
				if (obj is AetherCurrentPoint other)
				{
					return Equals(other);
				}
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return (int)DataID;
		}

		public static bool operator ==(AetherCurrentPoint? left, AetherCurrentPoint? right)
		{
			return object.Equals(left, right);
		}

		public static bool operator !=(AetherCurrentPoint? left, AetherCurrentPoint? right)
		{
			return !object.Equals(left, right);
		}
	}

	private enum PointType
	{
		Normal,
		Quest
	}

	private readonly List<AetherCurrentPoint> selectedAetherCurrents = new List<AetherCurrentPoint>();

	private bool isWindowUnlock;

	private bool useLocalMark = true;

	private bool manualMode;

	[CompilerGenerated]
	private static FrozenDictionary<uint, List<ZoneAetherCurrentInfo>> _003CVersionToZoneInfos_003Ek__BackingField;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoMarkAetherCurrentsTitle"),
		Description = LanguageManager.Get("AutoMarkAetherCurrentsDescription"),
		Category = ModuleCategory.Interface
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		NeedAuth = true,
		AllDefaultEnabled = true
	};

	private static bool IsEligibleForTeleporting
	{
		get
		{
			if (GameState.IsCN || GameState.IsTC)
			{
				return AuthState.IsPremium;
			}
			return true;
		}
	}

	private static Vector2 ChildSize => Globals.ScaledVector2(450f, 150f);

	private static FrozenDictionary<uint, List<ZoneAetherCurrentInfo>> VersionToZoneInfos
	{
		get
		{
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			if (_003CVersionToZoneInfos_003Ek__BackingField != null)
			{
				return _003CVersionToZoneInfos_003Ek__BackingField;
			}
			Dictionary<uint, List<ZoneAetherCurrentInfo>> dictionary = new Dictionary<uint, List<ZoneAetherCurrentInfo>>();
			Dictionary<uint, RowRef<AetherCurrent>[]> dictionary2 = ((IEnumerable<AetherCurrentCompFlgSet>)LuminaGetter.Get<AetherCurrentCompFlgSet>()).Where((AetherCurrentCompFlgSet x) => ((AetherCurrentCompFlgSet)(ref x)).Territory.ValueNullable.HasValue && ((AetherCurrentCompFlgSet)(ref x)).Territory.RowId != 156).ToDictionary((AetherCurrentCompFlgSet x) => ((AetherCurrentCompFlgSet)(ref x)).Territory.RowId, (AetherCurrentCompFlgSet x) => ((IEnumerable<RowRef<AetherCurrent>>)(object)((AetherCurrentCompFlgSet)(ref x)).AetherCurrents).ToArray());
			uint num = 0u;
			uint num2 = 0u;
			foreach (var (num4, acArray) in dictionary2)
			{
				if (!LuminaGetter.TryGetRow<TerritoryType>(num4, out TerritoryType item))
				{
					continue;
				}
				uint rowId = ((TerritoryType)(ref item)).ExVersion.RowId;
				if (rowId != 0)
				{
					if (num2 != rowId)
					{
						num = 0u;
						num2 = rowId;
					}
					ZoneAetherCurrentInfo zoneAetherCurrentInfo = ZoneAetherCurrentInfo.Parse(num4, num, acArray);
					if (zoneAetherCurrentInfo != null)
					{
						dictionary.TryAdd(rowId - 1, new List<ZoneAetherCurrentInfo>());
						dictionary[rowId - 1].Add(zoneAetherCurrentInfo);
						num++;
					}
				}
			}
			return _003CVersionToZoneInfos_003Ek__BackingField = dictionary.ToFrozenDictionary();
		}
	}

	protected override void Init()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Expected O, but got Unknown
		AetherCurrentPoint.RefreshUnlockStates();
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 30000
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		if (base.Overlay == null)
		{
			Overlay overlay = (base.Overlay = new Overlay(this));
		}
		Overlay? overlay3 = base.Overlay;
		((Window)overlay3).Flags = (ImGuiWindowFlags)(((Window)overlay3).Flags & -2);
		Overlay? overlay4 = base.Overlay;
		((Window)overlay4).Flags = (ImGuiWindowFlags)(((Window)overlay4).Flags | 0x41A);
		Overlay? overlay5 = base.Overlay;
		WindowSizeConstraints value = default(WindowSizeConstraints);
		((WindowSizeConstraints)(ref value))._002Ector();
		((WindowSizeConstraints)(ref value)).MinimumSize = ChildSize;
		((Window)overlay5).SizeConstraints = value;
		((Window)base.Overlay).WindowName = LuminaWrapper.GetAddonText(2448u) + "###AutoMarkAetherCurrents";
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)1, "AetherCurrent", new AddonEventDelegate(OnAddon));
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += OnZoneChanged;
	}

	protected override void Uninit()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChanged;
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddon });
	}

	private unsafe void OnAddon(AddonEvent type, AddonArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		AtkUnitBase* address = (AtkUnitBase*)args.Addon.Address;
		if (address != null)
		{
			((AtkUnitBase)address).Close(true);
			Overlay? overlay = base.Overlay;
			((Window)overlay).IsOpen = !((Window)overlay).IsOpen;
		}
	}

	private void OnZoneChanged(uint zoneID)
	{
		MarkAetherCurrents(zoneID, isFirstPage: true, useLocalMark);
	}

	protected override void ConfigUI()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("BetterFateProgressUI-UnlockWindow")), ref isWindowUnlock))
		{
			if (isWindowUnlock)
			{
				Overlay? overlay = base.Overlay;
				((Window)overlay).Flags = (ImGuiWindowFlags)(((Window)overlay).Flags & -65);
				Overlay? overlay2 = base.Overlay;
				((Window)overlay2).Flags = (ImGuiWindowFlags)(((Window)overlay2).Flags & -3);
			}
			else
			{
				Overlay? overlay3 = base.Overlay;
				((Window)overlay3).Flags = (ImGuiWindowFlags)(((Window)overlay3).Flags | 0x42);
			}
		}
	}

	protected override void OverlayPreDraw()
	{
		if (Throttler.Shared.Throttle("AutoMarkAetherCurrents-Refresh", 5000u))
		{
			AetherCurrentPoint.RefreshUnlockStates();
		}
	}

	protected override void OverlayOnOpen()
	{
		AetherCurrentPoint.RefreshUnlockStates();
	}

	protected override void OverlayUI()
	{
		using (OmenServiceBase<FontManager>.Instance().UIFont120.Push())
		{
			DrawMenuBar();
			DrawAetherCurrentsTabs();
		}
	}

	private unsafe void DrawMenuBar()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		using (OmenServiceBase<FontManager>.Instance().UIFont.Push())
		{
			if (!ImGui.BeginMenuBar())
			{
				return;
			}
			if (ImGui.BeginMenu(ImU8String.op_Implicit(LanguageManager.Get("General")), true))
			{
				if (ImGui.MenuItem(ImU8String.op_Implicit(LanguageManager.Get("ManualMode")), ImU8String.op_Implicit(string.Empty), ref manualMode, true))
				{
					MarkAetherCurrents(GameState.TerritoryType, isFirstPage: true, useLocalMark);
				}
				ImGuiOm.TooltipHover(LanguageManager.Get("AutoMarkAetherCurrents-ManualModeHelp"));
				if (ImGui.MenuItem(ImU8String.op_Implicit(LanguageManager.Get("AutoMarkAetherCurrents-UseLocalMark")), ImU8String.op_Implicit(string.Empty), ref useLocalMark, true))
				{
					MarkAetherCurrents(GameState.TerritoryType, isFirstPage: true, useLocalMark);
				}
				ImGuiOm.TooltipHover(LanguageManager.Get("AutoMarkAetherCurrents-UseLocalMarkHelp"));
				ImGui.EndMenu();
			}
			ImGui.TextDisabled(ImU8String.op_Implicit("|"));
			if (ImGui.BeginMenu(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(7131u)), true))
			{
				if (ImGui.MenuItem(ImU8String.op_Implicit(LanguageManager.Get("AutoMarkAetherCurrents-RefreshDisplay")), false, true))
				{
					MarkAetherCurrents(GameState.TerritoryType, isFirstPage: true, useLocalMark);
				}
				if (ImGui.MenuItem(ImU8String.op_Implicit(LanguageManager.Get("AutoMarkAetherCurrents-DisplayLeftCurrents")), false, true))
				{
					MarkAetherCurrents(GameState.TerritoryType, isFirstPage: false, useLocalMark);
				}
				if (ImGui.MenuItem(ImU8String.op_Implicit(LanguageManager.Get("AutoMarkAetherCurrents-RemoveAllWaymarks")), false, true))
				{
					for (uint num = 0u; num < 8; num++)
					{
						MarkingController* intPtr = MarkingController.Instance();
						FieldMarkerPoint point = (FieldMarkerPoint)num;
						(*intPtr).ClearFieldMarkerLocal(in point);
					}
				}
				ImGui.EndMenu();
			}
			ImGui.TextDisabled(ImU8String.op_Implicit("|"));
			if (ImGui.MenuItem(ImU8String.op_Implicit(LanguageManager.Get("AutoMarkAetherCurrents-RemoveSelectedAC")), manualMode && selectedAetherCurrents.Count > 0, true))
			{
				selectedAetherCurrents.Clear();
			}
			ImGui.TextDisabled(ImU8String.op_Implicit("|"));
			if (ImGui.MenuItem(ImU8String.op_Implicit(LanguageManager.Get("AutoMarkAetherCurrents-DisplayNotActivated")), false, true))
			{
				SelectNotActivatedAetherCurrents();
				MarkAetherCurrents(GameState.TerritoryType, isFirstPage: true, useLocalMark);
			}
			ImGui.EndMenuBar();
		}
	}

	private void DrawAetherCurrentsTabs()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		GroupDisposable val = ImRaii.Group();
		try
		{
			TabBarDisposable val2 = ImRaii.TabBar(ImU8String.op_Implicit("AetherCurrentsTab"));
			try
			{
				if (!(!val2))
				{
					ImmutableArray<uint>.Enumerator enumerator = VersionToZoneInfos.Keys.GetEnumerator();
					while (enumerator.MoveNext())
					{
						uint current = enumerator.Current;
						DrawAetherCurrentInfoTabItem(current);
					}
				}
			}
			finally
			{
				((TabBarDisposable)(ref val2)).Dispose();
			}
		}
		finally
		{
			((GroupDisposable)(ref val)).Dispose();
		}
	}

	private void DrawAetherCurrentInfoTabItem(uint version)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (!VersionToZoneInfos.TryGetValue(version, out List<ZoneAetherCurrentInfo> value))
		{
			return;
		}
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(2, 1);
		((ImU8String)(ref val)).AppendFormatted<uint>(version + 3);
		((ImU8String)(ref val)).AppendLiteral(".0");
		TabItemDisposable val2 = ImRaii.TabItem(val);
		try
		{
			if (!val2)
			{
				return;
			}
			int num = 0;
			foreach (ZoneAetherCurrentInfo item in value)
			{
				item.Draw(this);
				if (num % 2 == 0)
				{
					ImGui.SameLine();
				}
				num++;
			}
		}
		finally
		{
			((TabItemDisposable)(ref val2)).Dispose();
		}
	}

	private unsafe void MarkAetherCurrents(uint zoneID, bool isFirstPage = true, bool isLocal = true)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (LuminaGetter.TryGetRow<TerritoryType>(zoneID, out TerritoryType item) && VersionToZoneInfos.TryGetValue(((TerritoryType)(ref item)).ExVersion.RowId - 1, out List<ZoneAetherCurrentInfo> value) && value.TryGetFirst((ZoneAetherCurrentInfo x) => x.Zone == zoneID, out ZoneAetherCurrentInfo value2))
		{
			FieldMarkerPoint[] values = Enum.GetValues<FieldMarkerPoint>();
			for (int num = 0; num < values.Length; num++)
			{
				FieldMarkerPoint point = values[num];
				(*MarkingController.Instance()).ClearFieldMarkerLocal(in point);
			}
			List<AetherCurrentPoint> list = selectedAetherCurrents.Where((AetherCurrentPoint x) => x.Zone == zoneID).ToList();
			List<AetherCurrentPoint> list2 = ((list.Count != 0 || manualMode) ? list : value2.NormalPoints.ToList());
			List<AetherCurrentPoint> list3 = list2.Skip((list2.Count > 8 && !isFirstPage) ? 8 : 0).ToList();
			for (uint num2 = 0u; (float)num2 < MathF.Min(8f, list3.Count); num2++)
			{
				list3[(int)num2].PlaceFieldMarker(isLocal, num2);
			}
		}
	}

	private void SelectNotActivatedAetherCurrents()
	{
		manualMode = true;
		selectedAetherCurrents.Clear();
		ImmutableArray<List<ZoneAetherCurrentInfo>>.Enumerator enumerator = VersionToZoneInfos.Values.GetEnumerator();
		while (enumerator.MoveNext())
		{
			foreach (ZoneAetherCurrentInfo item in enumerator.Current)
			{
				foreach (AetherCurrentPoint item2 in item.NormalPoints.Concat(item.QuestPoints))
				{
					if (AetherCurrentPoint.UnlockStates.TryGetValue(item2.DataID, out var value) && !value)
					{
						selectedAetherCurrents.Add(item2);
					}
				}
			}
		}
	}
}
