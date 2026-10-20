using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Component.Excel;
using FFXIVClientStructs.FFXIV.Component.Exd;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Data;
using Lumina.Data.Files;
using Lumina.Data.Parsing.Layer;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Info.Game.Enums;
using OmenTools.Info.Lumina;
using OmenTools.Info.Lumina.Enums;
using OmenTools.Interop.Game.Helpers;
using OmenTools.Interop.Game.Lumina;

namespace OmenTools.Extensions;

public static class LuminaSheetExtension
{
	extension(InstanceObject instanceObject)
	{
		public Vector3 GetPosition()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			return new Vector3(instanceObject.Transform.Translation.X, instanceObject.Transform.Translation.Y, instanceObject.Transform.Translation.Z);
		}
	}

	extension(LgbFile file)
	{
		public static LgbFile? Get(uint territoryTypeID, LGBFileType fileType)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return Get(LuminaGetter.GetRowOrDefault<TerritoryType>(territoryTypeID), fileType);
		}

		public static LgbFile? Get(TerritoryType zone, LGBFileType fileType)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			if (!TryGet(zone, fileType, out LgbFile lgbFile))
			{
				return null;
			}
			return lgbFile;
		}

		public static bool TryGet(uint territoryTypeID, LGBFileType fileType, [NotNullWhen(true)] out LgbFile? lgbFile)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return TryGet(LuminaGetter.GetRowOrDefault<TerritoryType>(territoryTypeID), fileType, out lgbFile);
		}

		public static bool TryGet(TerritoryType zone, LGBFileType fileType, [NotNullWhen(true)] out LgbFile? lgbFile)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			lgbFile = null;
			ReadOnlySeString bg = ((TerritoryType)(ref zone)).Bg;
			string text = ((ReadOnlySeString)(ref bg)).ExtractText();
			if (string.IsNullOrEmpty(text))
			{
				return false;
			}
			int num = text.IndexOf("/level/", StringComparison.Ordinal);
			if (num < 0)
			{
				return false;
			}
			string text2 = $"bg/{text.Substring(0, num + 1)}level/{fileType.ToString().ToLowerInvariant()}.lgb";
			try
			{
				lgbFile = DalamudServiceExtension.Instance<IDataManager>().GetFile<LgbFile>(text2);
			}
			catch
			{
			}
			return lgbFile != null;
		}

		public List<InstanceObject> GetInstanceObjects(LayerEntryType entryType)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Invalid comparison between Unknown and I4
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Invalid comparison between Unknown and I4
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Invalid comparison between Unknown and I4
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			List<InstanceObject> list = new List<InstanceObject>();
			Layer[] layers = file.Layers;
			for (int i = 0; i < layers.Length; i++)
			{
				InstanceObject[] instanceObjects = layers[i].InstanceObjects;
				foreach (InstanceObject val in instanceObjects)
				{
					if (val.AssetType != entryType)
					{
						continue;
					}
					ENpcBase item3;
					if ((int)entryType != 8)
					{
						Aetheryte item2;
						if ((int)entryType != 12)
						{
							if ((int)entryType == 41 && !LuminaGetter.TryGetRow<TerritoryType>((uint)((ExitRangeInstanceObject)(object)val.Object).TerritoryType, out TerritoryType _))
							{
								continue;
							}
						}
						else if (!LuminaGetter.TryGetRow<Aetheryte>(((AetheryteInstanceObject)(object)val.Object).ParentData.BaseId, out item2))
						{
							continue;
						}
					}
					else if (!LuminaGetter.TryGetRow<ENpcBase>(((ENPCInstanceObject)(object)val.Object).ParentData.ParentData.BaseId, out item3))
					{
						continue;
					}
					list.Add(val);
				}
			}
			return list;
		}
	}

	extension(scoped in TerritoryType zone)
	{
		public LgbFile? GetLGB(LGBFileType fileType)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return Get(zone, fileType);
		}

		public bool TryGetLGB(LGBFileType fileType, [NotNullWhen(true)] out LgbFile? lgbFile)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return TryGet(zone, fileType, out lgbFile);
		}

		public List<MapMarker> GetMapMarkers()
		{
			uint rowID = ((TerritoryType)(ref zone)).RowId;
			return ((IEnumerable<Map>)LuminaGetter.Get<Map>()).Where((Map x) => ((Map)(ref x)).TerritoryType.RowId == rowID).SelectMany((Map x) => x.GetMapMarkers()).ToList();
		}
	}

	extension(scoped in TerritoryType row)
	{
		public string ExtractPlaceName()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			PlaceName? valueNullable = ((TerritoryType)(ref row)).PlaceName.ValueNullable;
			object obj;
			if (!valueNullable.HasValue)
			{
				obj = null;
			}
			else
			{
				PlaceName valueOrDefault = valueNullable.GetValueOrDefault();
				obj = ((object)((PlaceName)(ref valueOrDefault)).Name/*cast due to constrained. prefix*/).ToString();
			}
			if (obj == null)
			{
				obj = string.Empty;
			}
			return (string)obj;
		}
	}

	extension(scoped in Map map)
	{
		public string GetTexturePath()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			Map val = map;
			TerritoryType value = ((Map)(ref map)).TerritoryType.Value;
			if (((TerritoryType)(ref value)).ContentFinderCondition.RowId == 0 && Sheets.MapToFinalTextureMap.TryGetValue(((Map)(ref map)).RowId, out var value2))
			{
				val = value2;
			}
			string text = ((object)((Map)(ref val)).Id/*cast due to constrained. prefix*/).ToString();
			string value3 = text.Replace("/", "");
			return $"ui/map/{text}/{value3}_m.tex";
		}

		public List<MapMarker> GetMapMarkers()
		{
			ushort markerRange = ((Map)(ref map)).MapMarkerRange;
			return (from x in ((IEnumerable<SubrowCollection<MapMarker>>)LuminaGetter.GetSub<MapMarker>()).SelectMany((SubrowCollection<MapMarker> x) => (IEnumerable<MapMarker>)(object)x)
				where ((MapMarker)(ref x)).RowId == markerRange
				select x).ToList();
		}
	}

	extension(scoped in Aetheryte aetheryte)
	{
		public Vector2 GetPositionWorld()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			TerritoryType? valueNullable = ((Aetheryte)(ref aetheryte)).Territory.ValueNullable;
			Map? val;
			if (!valueNullable.HasValue)
			{
				val = null;
			}
			else
			{
				TerritoryType valueOrDefault = valueNullable.GetValueOrDefault();
				val = ((TerritoryType)(ref valueOrDefault)).Map.ValueNullable;
			}
			Map? val2 = val;
			if (!val2.HasValue)
			{
				return Vector2.Zero;
			}
			return PositionHelper.MapToWorld(aetheryte.GetPositionMap(), val2.Value);
		}

		public Vector2 GetPositionMap()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			if (((Aetheryte)(ref aetheryte)).Territory.RowId != 0)
			{
				TerritoryType value = ((Aetheryte)(ref aetheryte)).Territory.Value;
				if (((TerritoryType)(ref value)).Map.RowId != 0)
				{
					value = ((Aetheryte)(ref aetheryte)).Territory.Value;
					Map mapRow = ((TerritoryType)(ref value)).Map.Value;
					uint aetheryteRowID = ((Aetheryte)(ref aetheryte)).RowId;
					return (from x in ((IEnumerable<SubrowCollection<MapMarker>>)LuminaGetter.GetSub<MapMarker>()).SelectMany((SubrowCollection<MapMarker> x) => (IEnumerable<MapMarker>)(object)x).Where(delegate(MapMarker x)
						{
							//IL_0020: Unknown result type (might be due to invalid IL or missing references)
							//IL_0025: Unknown result type (might be due to invalid IL or missing references)
							if (((MapMarker)(ref x)).DataType == 3 && ((MapMarker)(ref x)).RowId == ((Map)(ref mapRow)).MapMarkerRange)
							{
								RowRef dataKey = ((MapMarker)(ref x)).DataKey;
								return ((RowRef)(ref dataKey)).RowId == aetheryteRowID;
							}
							return false;
						})
						select PositionHelper.TextureToMap(((MapMarker)(ref x)).X, ((MapMarker)(ref x)).Y, (int)((Map)(ref mapRow)).SizeFactor)).FirstOrDefault();
				}
			}
			return Vector2.Zero;
		}
	}

	extension(scoped in MapMarker marker)
	{
		public string GetMarkerPlaceName()
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			string markerLabel = marker.GetMarkerLabel();
			if (markerLabel != string.Empty)
			{
				return markerLabel;
			}
			if (!LuminaGetter.TryGetRow<MapSymbol>((uint)((MapMarker)(ref marker)).Icon, out MapSymbol item))
			{
				return string.Empty;
			}
			PlaceName? valueNullable = ((MapSymbol)(ref item)).PlaceName.ValueNullable;
			object obj;
			if (!valueNullable.HasValue)
			{
				obj = null;
			}
			else
			{
				PlaceName valueOrDefault = valueNullable.GetValueOrDefault();
				obj = ((object)((PlaceName)(ref valueOrDefault)).Name/*cast due to constrained. prefix*/).ToString();
			}
			if (obj == null)
			{
				obj = string.Empty;
			}
			return (string)obj;
		}

		public string GetMarkerLabel()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			PlaceName? valueNullable = ((MapMarker)(ref marker)).PlaceNameSubtext.ValueNullable;
			object obj;
			if (!valueNullable.HasValue)
			{
				obj = null;
			}
			else
			{
				PlaceName valueOrDefault = valueNullable.GetValueOrDefault();
				obj = ((object)((PlaceName)(ref valueOrDefault)).Name/*cast due to constrained. prefix*/).ToString();
			}
			if (obj == null)
			{
				obj = string.Empty;
			}
			return (string)obj;
		}

		public Vector2 GetPosition()
		{
			return new Vector2(((MapMarker)(ref marker)).X, ((MapMarker)(ref marker)).Y);
		}
	}

	extension(scoped in MKDSupportJob job)
	{
		public uint GetIcon()
		{
			return ((MKDSupportJob)(ref job)).RowId + 82270 + 1;
		}
	}

	extension(scoped in ClassJob job)
	{
		public BitmapFontIcon ToBitmapFontIcon()
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			if (((ClassJob)(ref job)).RowId == 0)
			{
				return (BitmapFontIcon)77;
			}
			switch (((ClassJob)(ref job)).RowId)
			{
			case 0u:
				return (BitmapFontIcon)77;
			case 1u:
			case 2u:
			case 3u:
			case 4u:
			case 5u:
			case 6u:
			case 7u:
			case 8u:
			case 9u:
			case 10u:
			case 11u:
			case 12u:
			case 13u:
			case 14u:
			case 15u:
			case 16u:
			case 17u:
			case 18u:
			case 19u:
			case 20u:
			case 21u:
			case 22u:
			case 23u:
			case 24u:
			case 25u:
			case 26u:
			case 27u:
			case 28u:
			case 29u:
			case 30u:
			case 31u:
			case 32u:
			case 33u:
			case 34u:
			case 35u:
			case 36u:
			case 37u:
			case 38u:
			case 39u:
			case 40u:
				return (BitmapFontIcon)(((ClassJob)(ref job)).RowId + 127);
			case 41u:
			case 42u:
				return (BitmapFontIcon)(((ClassJob)(ref job)).RowId + 129);
			case 43u:
				return (BitmapFontIcon)183;
			default:
				return (BitmapFontIcon)77;
			}
		}

		public uint GetIcon(ClassJobIconType type = ClassJobIconType.Framed)
		{
			return (uint)(((ClassJob)(ref job)).RowId + type);
		}

		public ClassJobType ToJobType()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return ((ClassJob)(ref job)).ClassJobCategory.RowId switch
			{
				32u => ClassJobType.Gatherer, 
				33u => ClassJobType.Crafter, 
				_ => ((ClassJob)(ref job)).JobType switch
				{
					1 => ClassJobType.Tank, 
					2 => ClassJobType.PureHealer, 
					3 => ClassJobType.Melee, 
					4 => ClassJobType.PhysicalRanged, 
					5 => ClassJobType.MagicalRanged, 
					6 => ClassJobType.ShieldHealer, 
					_ => ClassJobType.None, 
				}, 
			};
		}
	}

	extension(scoped in Level level)
	{
		public Vector3 GetPosition()
		{
			return new Vector3(((Level)(ref level)).X, ((Level)(ref level)).Y, ((Level)(ref level)).Z);
		}
	}

	extension(scoped in ClassJobCategory category)
	{
		public bool IsClassJobIn(uint classJobID)
		{
			return IsClassJobInCategory(classJobID, ((ClassJobCategory)(ref category)).RowId);
		}

		public unsafe static bool IsClassJobInCategory(uint classJobID, uint classJobCategoryID)
		{
			if (classJobCategoryID == 0)
			{
				return false;
			}
			ExcelRow* rowBySheetIndexAndRowIndex = ((ExdModule)((ExcelModuleInterface)((Framework)Framework.Instance()).ExcelModuleInterface).ExdModule).GetRowBySheetIndexAndRowIndex(60u, classJobCategoryID);
			if (rowBySheetIndexAndRowIndex == null)
			{
				return false;
			}
			return ((byte*)((ExcelRow)rowBySheetIndexAndRowIndex).Data + 4)[classJobID] == 1;
		}
	}

	extension(scoped in UIColor color)
	{
		public unsafe Vector4 ToVector4()
		{
			return ((AtkUIColorHolder)((AtkStage)AtkStage.Instance()).AtkUIColorHolder).GetColor(true, ((UIColor)(ref color)).RowId).ToVector4();
		}

		public unsafe uint ToUInt()
		{
			return ((AtkUIColorHolder)((AtkStage)AtkStage.Instance()).AtkUIColorHolder).GetColor(true, ((UIColor)(ref color)).RowId);
		}
	}

	extension(uint id)
	{
		public RowRef<T> ToLuminaRowRef<T>() where T : struct, IExcelRow<T>
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return ToLuminaRowRefInternal<T>(id);
		}
	}

	extension(ushort id)
	{
		public RowRef<T> ToLuminaRowRef<T>() where T : struct, IExcelRow<T>
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return ToLuminaRowRefInternal<T>(id);
		}
	}

	extension(byte id)
	{
		public RowRef<T> ToLuminaRowRef<T>() where T : struct, IExcelRow<T>
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return ToLuminaRowRefInternal<T>(id);
		}
	}

	private static RowRef<T> ToLuminaRowRefInternal<T>(uint id) where T : struct, IExcelRow<T>
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		return new RowRef<T>(DalamudServiceExtension.Instance<IDataManager>().Excel, id, (Language?)null);
	}
}
