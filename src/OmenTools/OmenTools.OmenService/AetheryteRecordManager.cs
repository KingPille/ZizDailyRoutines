using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using OmenTools.Extensions;
using OmenTools.Info.Game.AetheryteRecord;
using OmenTools.Info.Game.AetheryteRecord.Data;
using OmenTools.Info.Lumina.ExtraSheets;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.OmenService;

public class AetheryteRecordManager : OmenServiceBase<AetheryteRecordManager>
{
	private const ulong INVALID_HOUSE_ID = ulong.MaxValue;

	private static readonly string VERSION_OTHER = LuminaWrapper.GetAddonText(832u);

	public string HouseApartmentTemplate { get; set; } = "{0} {1} (第 {2} 区 {3} 号房间)";

	public string HouseEstateTemplate { get; set; } = "{0} {1} (第 {2} 区 {3} 号)";

	public string HouseSharedName { get; set; } = "共享房屋";

	public string HouseFreeCompanyName { get; set; } = "部队房屋";

	public string HousePersonalName { get; set; } = "个人房屋";

	public Dictionary<string, List<AetheryteRecord>> Records { get; private set; } = new Dictionary<string, List<AetheryteRecord>>();

	public IEnumerable<AetheryteRecord> AllRecords
	{
		get
		{
			foreach (List<AetheryteRecord> value in Records.Values)
			{
				foreach (AetheryteRecord item in value)
				{
					yield return item;
				}
			}
		}
	}

	public AetheryteRecord? GetNearestAetheryte(uint zoneID, Vector3 pos, bool excludeAethernet = false)
	{
		List<AetheryteRecord> list = (from x in AllRecords
			where AetheryteRecords.AethernetGroups.Contains(x.Group) && x.ZoneID == zoneID && x.IsUnlocked()
			where !excludeAethernet || x.IsAetheryte
			orderby Vector3.DistanceSquared(x.Position, pos)
			select x).ToList();
		if (list.Count != 0)
		{
			return list.FirstOrDefault();
		}
		return null;
	}

	protected override void Init()
	{
		if (GameState.IsLoggedIn)
		{
			BuildRecords();
			RefreshRecords();
		}
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += OnZoneChanged;
		OmenServiceBase<GameState>.Instance().Login += OnLogin;
		OmenServiceBase<GameState>.Instance().WorldChanged += OnWorldChanged;
	}

	protected override void Uninit()
	{
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChanged;
		OmenServiceBase<GameState>.Instance().Login -= OnLogin;
		OmenServiceBase<GameState>.Instance().WorldChanged -= OnWorldChanged;
	}

	private void OnWorldChanged(uint obj)
	{
		BuildRecords();
		RefreshRecords();
	}

	private void OnLogin()
	{
		if (GameState.IsLoggedIn)
		{
			BuildRecords();
			RefreshRecords();
		}
	}

	private void OnZoneChanged(uint zone)
	{
		if (GameState.ContentFinderCondition == 0 && GameState.IsLoggedIn)
		{
			RefreshRecords();
		}
	}

	private void BuildRecords()
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		Records = new Dictionary<string, List<AetheryteRecord>>();
		foreach (Aetheryte item in from x in (IEnumerable<Aetheryte>)LuminaGetter.Get<Aetheryte>()
			orderby ((Aetheryte)(ref x)).AethernetGroup, ((Aetheryte)(ref x)).Territory.RowId
			select x)
		{
			AetheryteRecord aetheryteRecord = AetheryteRecord.Parse(item);
			if (aetheryteRecord == null || !aetheryteRecord.IsUnlocked())
			{
				continue;
			}
			if (aetheryteRecord.Group == 5)
			{
				Records.TryAdd(VERSION_OTHER, new List<AetheryteRecord>());
				Records[VERSION_OTHER].Add(aetheryteRecord);
			}
			else if (aetheryteRecord.Version == 0)
			{
				TerritoryType zone = aetheryteRecord.GetZone();
				PlaceName value = ((TerritoryType)(ref zone)).PlaceNameRegion.Value;
				uint rowId = ((PlaceName)(ref value)).RowId;
				string? text;
				if (rowId - 22 > 2)
				{
					text = VERSION_OTHER;
				}
				else
				{
					zone = aetheryteRecord.GetZone();
					PlaceName value2 = ((TerritoryType)(ref zone)).PlaceNameRegion.Value;
					text = ((object)((PlaceName)(ref value2)).Name/*cast due to constrained. prefix*/).ToString();
				}
				string key = text;
				Records.TryAdd(key, new List<AetheryteRecord>());
				Records[key].Add(aetheryteRecord);
			}
			else
			{
				string key2 = $"{aetheryteRecord.Version + 2}.0";
				Records.TryAdd(key2, new List<AetheryteRecord>());
				Records[key2].Add(aetheryteRecord);
			}
		}
		BuildFirmamentRecords();
		BuildCosmicExplorationRecords();
		BuildHouseRecords();
	}

	private void RefreshRecords()
	{
		foreach (AetheryteRecord allRecord in AllRecords)
		{
			allRecord.Update();
		}
	}

	private void BuildHouseRecords()
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		List<HousingMapMarkerInfo> housingMarkers = (from x in ((IEnumerable<SubrowCollection<HousingMapMarkerInfo>>)LuminaGetter.GetSub<HousingMapMarkerInfo>()).SelectMany((SubrowCollection<HousingMapMarkerInfo> x) => (IEnumerable<HousingMapMarkerInfo>)(object)x)
			where ((HousingMapMarkerInfo)(ref x)).Map.ValueNullable.HasValue
			select x).ToList();
		HouseId ownedHouseId = HousingManager.GetOwnedHouseId((EstateType)5, -1);
		HouseId ownedHouseId2 = HousingManager.GetOwnedHouseId((EstateType)6, -1);
		if (ownedHouseId.Id != ulong.MaxValue && ownedHouseId2.Id != ulong.MaxValue)
		{
			TryAddHouseRecord(housingMarkers, ownedHouseId.TerritoryTypeId, GetApartmentAetheryteID, 60u, 128, HouseApartmentTemplate, new object[2]
			{
				((HouseId)(ref ownedHouseId)).WardIndex + 1,
				((HouseId)(ref ownedHouseId2)).RoomNumber
			}, LuminaWrapper.GetAddonText(6760u));
		}
		HouseId ownedHouseId3 = HousingManager.GetOwnedHouseId((EstateType)2, -1);
		if (ownedHouseId3.Id != ulong.MaxValue)
		{
			TryAddHouseRecord(housingMarkers, ownedHouseId3.TerritoryTypeId, GetApartmentAetheryteID, ((HouseId)(ref ownedHouseId3)).PlotIndex, 0, HouseEstateTemplate, new object[2]
			{
				((HouseId)(ref ownedHouseId3)).WardIndex + 1,
				((HouseId)(ref ownedHouseId3)).PlotIndex + 1
			}, HousePersonalName);
		}
		HouseId ownedHouseId4 = HousingManager.GetOwnedHouseId((EstateType)0, -1);
		if (ownedHouseId4.Id != ulong.MaxValue)
		{
			TryAddHouseRecord(housingMarkers, ownedHouseId4.TerritoryTypeId, GetFreeCompanyAetheryteID, ((HouseId)(ref ownedHouseId4)).PlotIndex, 0, HouseEstateTemplate, new object[2]
			{
				((HouseId)(ref ownedHouseId4)).WardIndex + 1,
				((HouseId)(ref ownedHouseId4)).PlotIndex + 1
			}, HouseFreeCompanyName);
		}
		for (int num = 0; num < 2; num++)
		{
			HouseId ownedHouseId5 = HousingManager.GetOwnedHouseId((EstateType)4, num);
			if (ownedHouseId5.Id != ulong.MaxValue)
			{
				TryAddHouseRecord(housingMarkers, ownedHouseId5.TerritoryTypeId, GetApartmentAetheryteID, ((HouseId)(ref ownedHouseId5)).PlotIndex, (byte)(num + 1), HouseEstateTemplate, new object[2]
				{
					((HouseId)(ref ownedHouseId5)).WardIndex + 1,
					((HouseId)(ref ownedHouseId5)).PlotIndex + 1
				}, HouseSharedName);
			}
		}
	}

	private void TryAddHouseRecord(List<HousingMapMarkerInfo> housingMarkers, uint zoneID, Func<uint, uint> getAetheryteID, uint markerSubrowID, byte subIndex, string nameTemplate, object[] nameArgs, string houseTypeName)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		Aetheryte rowOrDefault = LuminaGetter.GetRowOrDefault<Aetheryte>(getAetheryteID(zoneID));
		HousingMapMarkerInfo val = housingMarkers.Where(delegate(HousingMapMarkerInfo x)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			Map value = ((HousingMapMarkerInfo)(ref x)).Map.Value;
			return ((Map)(ref value)).TerritoryType.RowId == zoneID;
		}).FirstOrDefault((HousingMapMarkerInfo x) => ((HousingMapMarkerInfo)(ref x)).SubrowId == markerSubrowID);
		if (((Aetheryte)(ref rowOrDefault)).RowId != 0 && ((HousingMapMarkerInfo)(ref val)).RowId != 0)
		{
			string text = ((Aetheryte)(ref rowOrDefault)).Territory.Value.ExtractPlaceName();
			InlineArray4<object> buffer = default(InlineArray4<object>);
			buffer[0] = text;
			buffer[1] = houseTypeName;
			buffer[2] = nameArgs[0];
			buffer[3] = nameArgs[1];
			string name = string.Format(nameTemplate, (ReadOnlySpan<object?>)buffer);
			AetheryteRecord item = new AetheryteRecord(((Aetheryte)(ref rowOrDefault)).RowId, subIndex, ((Aetheryte)(ref rowOrDefault)).AethernetGroup, 0u, zoneID, ((HousingMapMarkerInfo)(ref val)).Map.RowId, IsAetheryte: true, new Vector3(((HousingMapMarkerInfo)(ref val)).X, ((HousingMapMarkerInfo)(ref val)).Y, ((HousingMapMarkerInfo)(ref val)).Z), name);
			Records.TryAdd(VERSION_OTHER, new List<AetheryteRecord>());
			Records[VERSION_OTHER].Add(item);
		}
	}

	private static uint GetApartmentAetheryteID(uint zoneID)
	{
		switch (zoneID)
		{
		case 339u:
			return 59u;
		case 340u:
			return 60u;
		case 341u:
			return 61u;
		case 641u:
			return 97u;
		case 979u:
			return 165u;
		default:
		{
			global::_003CPrivateImplementationDetails_003E.ThrowSwitchExpressionException(zoneID);
			uint result = default(uint);
			return result;
		}
		}
	}

	private static uint GetFreeCompanyAetheryteID(uint zoneID)
	{
		switch (zoneID)
		{
		case 339u:
			return 56u;
		case 340u:
			return 57u;
		case 341u:
			return 58u;
		case 641u:
			return 96u;
		case 979u:
			return 164u;
		default:
		{
			global::_003CPrivateImplementationDetails_003E.ThrowSwitchExpressionException(zoneID);
			uint result = default(uint);
			return result;
		}
		}
	}

	private void BuildFirmamentRecords()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		TerritoryType rowOrDefault = LuminaGetter.GetRowOrDefault<TerritoryType>(886u);
		Map value = ((TerritoryType)(ref rowOrDefault)).Map.Value;
		foreach (KeyValuePair<uint, Vector3> firmamentPosition in AetheryteRecords.FirmamentPositions)
		{
			firmamentPosition.Deconstruct(out var key, out var value2);
			uint num = key;
			Vector3 position = value2;
			string name = ((object)LuminaGetter.GetRowOrDefault<HwdTranspoint>(num + 1).Text/*cast due to constrained. prefix*/).ToString();
			AetheryteRecord item = new AetheryteRecord(70u, (byte)num, 254, 1u, 886u, ((Map)(ref value)).RowId, IsAetheryte: false, position, name);
			Records.TryAdd("3.0", new List<AetheryteRecord>());
			Records["3.0"].Add(item);
		}
	}

	private void BuildCosmicExplorationRecords()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		byte b = 0;
		foreach (KeyValuePair<uint, List<Vector3>> cosmicExplorationPosition in AetheryteRecords.CosmicExplorationPositions)
		{
			cosmicExplorationPosition.Deconstruct(out var key, out var value);
			uint num = key;
			List<Vector3> list = value;
			TerritoryType rowOrDefault = LuminaGetter.GetRowOrDefault<TerritoryType>(num);
			Map value2 = ((TerritoryType)(ref rowOrDefault)).Map.Value;
			uint rowId = ((TerritoryType)(ref rowOrDefault)).ExVersion.RowId;
			string key2 = $"{rowId + 2}.0";
			uint num2 = AetheryteRecords.CosmicPlaceNameOffsets[num];
			foreach (Vector3 item2 in list)
			{
				string placeName = LuminaWrapper.GetPlaceName(b + num2);
				AetheryteRecord item = new AetheryteRecord(175u, b, 253, rowId, num, ((Map)(ref value2)).RowId, IsAetheryte: false, item2, placeName);
				Records.TryAdd(key2, new List<AetheryteRecord>());
				Records[key2].Add(item);
				b++;
			}
		}
	}
}
