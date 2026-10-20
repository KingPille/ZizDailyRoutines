using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using OmenTools.Extensions;
using OmenTools.Info.Game.AetheryteRecord.Data;
using OmenTools.Info.Game.AetheryteRecord.Enums;
using OmenTools.Interop.Game.Helpers;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;

namespace OmenTools.Info.Game.AetheryteRecord;

public record AetheryteRecord(uint RowID, byte SubIndex, byte Group, uint Version, uint ZoneID, uint MapID, bool IsAetheryte, Vector3 Position, string Name)
{
	public AetheryteRecordState State { get; private set; }

	public uint Cost { get; private set; }

	public string RegionName
	{
		get
		{
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			if (_003CRegionName_003Ek__BackingField == null)
			{
				string text;
				TerritoryType zone;
				PlaceName value;
				switch (Group)
				{
				case 5:
					text = LuminaWrapper.GetZonePlaceName(144u);
					break;
				case 254:
					zone = GetZone();
					value = ((TerritoryType)(ref zone)).PlaceName.Value;
					text = ((object)((PlaceName)(ref value)).Name/*cast due to constrained. prefix*/).ToString();
					break;
				case 253:
					text = LuminaWrapper.GetAddonText(3849u);
					break;
				default:
					zone = GetZone();
					value = ((TerritoryType)(ref zone)).PlaceNameZone.Value;
					text = ((object)((PlaceName)(ref value)).Name/*cast due to constrained. prefix*/).ToString();
					break;
				}
				string text2 = text;
				if (ZoneID == 250)
				{
					text2 = LuminaWrapper.GetPlaceName(22u);
				}
				if (IsHouse)
				{
					text2 = LuminaWrapper.GetAddonText(8495u);
				}
				_003CRegionName_003Ek__BackingField = text2;
			}
			return _003CRegionName_003Ek__BackingField;
		}
	}

	public bool IsHouse
	{
		get
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			if (isHouse.HasValue)
			{
				return isHouse.Value;
			}
			Aetheryte data = GetData();
			uint rowId = ((Aetheryte)(ref data)).PlaceName.RowId;
			bool value = ((rowId == 1145 || rowId == 1160) ? true : false);
			isHouse = value;
			return isHouse.Value;
		}
	}

	[CompilerGenerated]
	private string _003CRegionName_003Ek__BackingField;

	private bool? isHouse;

	public Aetheryte GetData()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return LuminaGetter.GetRow<Aetheryte>(RowID).GetValueOrDefault();
	}

	public TerritoryType GetZone()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return LuminaGetter.GetRow<TerritoryType>(ZoneID).GetValueOrDefault();
	}

	public Map GetMap()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return LuminaGetter.GetRow<Map>(MapID).GetValueOrDefault();
	}

	public unsafe bool IsUnlocked()
	{
		if (IsHouse)
		{
			return true;
		}
		if (!AetheryteRecords.AethernetGroups.Contains(Group))
		{
			return true;
		}
		byte b = Group;
		bool flag = (uint)(b - 253) <= 1u;
		if (flag && GameState.CurrentWorld != GameState.HomeWorld)
		{
			return false;
		}
		return ((UIState)UIState.Instance()).IsAetheryteUnlocked(RowID);
	}

	public void Update()
	{
		(State, Cost) = GetAetheryteState(this);
	}

	public unsafe bool UseAethernet()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		AgentTelepotTown* ptr = AgentTelepotTown.Instance();
		if (ptr == null || !((AgentTelepotTown)ptr).IsAgentActive())
		{
			return false;
		}
		if (((AgentTelepotTown)ptr).Data == null)
		{
			return false;
		}
		for (byte b = 0; b < ((AgentTelepotTownData)((AgentTelepotTown)ptr).Data).Entries.Length; b++)
		{
			if (((AgentTelepotTownData)((AgentTelepotTown)ptr).Data).Entries[b].AetheryteId == RowID)
			{
				((AgentTelepotTown)ptr).TeleportToAetheryte(b);
				return true;
			}
		}
		return false;
	}

	public static AetheryteRecord? Parse(Aetheryte data)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		//IL_045f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0481: Unknown result type (might be due to invalid IL or missing references)
		//IL_0486: Unknown result type (might be due to invalid IL or missing references)
		//IL_048a: Unknown result type (might be due to invalid IL or missing references)
		//IL_048f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0501: Unknown result type (might be due to invalid IL or missing references)
		//IL_0549: Unknown result type (might be due to invalid IL or missing references)
		//IL_054e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		if (((Aetheryte)(ref data)).RowId <= 1)
		{
			return null;
		}
		uint rowId = ((Aetheryte)(ref data)).PlaceName.RowId;
		if ((rowId == 1145 || rowId == 1160) ? true : false)
		{
			return null;
		}
		TerritoryType zone;
		Map value;
		if (((Aetheryte)(ref data)).Map.RowId == 0)
		{
			zone = ((Aetheryte)(ref data)).Territory.Value;
			value = ((TerritoryType)(ref zone)).Map.Value;
		}
		else
		{
			value = ((Aetheryte)(ref data)).Map.Value;
		}
		Map map = value;
		PlaceName value2;
		string? text;
		if (!((Aetheryte)(ref data)).IsAetheryte)
		{
			value2 = ((Aetheryte)(ref data)).AethernetName.Value;
			text = ((object)((PlaceName)(ref value2)).Name/*cast due to constrained. prefix*/).ToString();
		}
		else
		{
			value2 = ((Aetheryte)(ref data)).PlaceName.Value;
			text = ((object)((PlaceName)(ref value2)).Name/*cast due to constrained. prefix*/).ToString();
		}
		string name = text;
		if (string.IsNullOrWhiteSpace(name))
		{
			return null;
		}
		int num = 0;
		if (!((Aetheryte)(ref data)).IsAetheryte && ((Aetheryte)(ref data)).AethernetGroup > 0)
		{
			List<Aetheryte> list = ((IEnumerable<Aetheryte>)LuminaGetter.Get<Aetheryte>()).Where((Aetheryte x) => ((Aetheryte)(ref x)).AethernetGroup == ((Aetheryte)(ref data)).AethernetGroup).ToList();
			List<Aetheryte> list2 = (from x in list
				where !((Aetheryte)(ref x)).IsAetheryte && ((Aetheryte)(ref x)).Map.RowId != 0
				orderby ((Aetheryte)(ref x)).Territory.RowId, ((Aetheryte)(ref x)).RowId
				select x).ToList();
			List<Aetheryte> list3 = list.ExceptBy(list2.Select((Aetheryte x) => ((Aetheryte)(ref x)).RowId), (Aetheryte x) => ((Aetheryte)(ref x)).RowId).ToList();
			Aetheryte item = list3.FirstOrDefault((Aetheryte x) => ((Aetheryte)(ref x)).IsAetheryte);
			List<Aetheryte> list4 = (from x in list3
				where LuminaGetter.TryGetRow<Level>(((Aetheryte)(ref x)).Level[0].RowId, out Level _)
				orderby ((Aetheryte)(ref x)).Territory.RowId, ((Aetheryte)(ref x)).RowId
				select x).ToList();
			List<Aetheryte> list5 = (from x in list3.ExceptBy(list4.Select((Aetheryte x) => ((Aetheryte)(ref x)).RowId), (Aetheryte x) => ((Aetheryte)(ref x)).RowId).ToList()
				where !((Aetheryte)(ref x)).IsAetheryte
				orderby ((Aetheryte)(ref x)).RowId
				select x).ToList();
			List<Aetheryte> list6 = new List<Aetheryte> { item };
			foreach (Aetheryte item4 in list5)
			{
				list6.Add(item4);
			}
			foreach (Aetheryte item5 in list4)
			{
				list6.Add(item5);
			}
			foreach (Aetheryte item6 in list2)
			{
				list6.Add(item6);
			}
			num = EnumerableExtension.IndexOf(list6, (Aetheryte x) => ((Aetheryte)(ref x)).RowId == ((Aetheryte)(ref data)).RowId);
		}
		Vector3 vector = default(Vector3);
		Level item2;
		if (AetheryteRecords.NormalPositions.TryGetValue(((Aetheryte)(ref data)).RowId, out var value3))
		{
			vector = value3;
		}
		else if (LuminaGetter.TryGetRow<Level>(((Aetheryte)(ref data)).Level[0].RowId, out item2))
		{
			vector = item2.GetPosition();
		}
		else
		{
			zone = ((Aetheryte)(ref data)).Territory.Value;
			vector = (from x in zone.GetMapMarkers().Where(delegate(MapMarker x)
				{
					byte dataType = ((MapMarker)(ref x)).DataType;
					return (uint)(dataType - 3) <= 1u;
				})
				where TryParseName(x, out string name2) && name2 == name
				select PositionHelper.TextureToWorld(new Vector2(((MapMarker)(ref x)).X, ((MapMarker)(ref x)).Y), map).ToVector3(0f)).FirstOrDefault();
		}
		zone = ((Aetheryte)(ref data)).Territory.Value;
		uint version = ((TerritoryType)(ref zone)).ExVersion.RowId;
		if (AetheryteRecords.NormalVersions.TryGetValue(((Aetheryte)(ref data)).RowId, out var value4))
		{
			version = value4;
		}
		return new AetheryteRecord(((Aetheryte)(ref data)).RowId, (byte)num, ((Aetheryte)(ref data)).AethernetGroup, version, ((Aetheryte)(ref data)).Territory.RowId, ((Map)(ref map)).RowId, ((Aetheryte)(ref data)).IsAetheryte, vector, name);
	}

	public static bool TryParseName(MapMarker marker, [NotNullWhen(true)] out string? name)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		name = null;
		if (((MapMarker)(ref marker)).Icon == 0)
		{
			return false;
		}
		byte dataType = ((MapMarker)(ref marker)).DataType;
		if ((uint)(dataType - 3) > 1u)
		{
			return false;
		}
		RowRef dataKey;
		switch (((MapMarker)(ref marker)).DataType)
		{
		case 3:
		{
			dataKey = ((MapMarker)(ref marker)).DataKey;
			if (!LuminaGetter.TryGetRow<Aetheryte>(((RowRef)(ref dataKey)).RowId, out Aetheryte item2))
			{
				return false;
			}
			PlaceName? valueNullable = ((Aetheryte)(ref item2)).PlaceName.ValueNullable;
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
			name = (string?)obj;
			break;
		}
		case 4:
		{
			dataKey = ((MapMarker)(ref marker)).DataKey;
			if (!LuminaGetter.TryGetRow<PlaceName>(((RowRef)(ref dataKey)).RowId, out PlaceName item))
			{
				return false;
			}
			name = ((object)((PlaceName)(ref item)).Name/*cast due to constrained. prefix*/).ToString() ?? string.Empty;
			break;
		}
		default:
			return false;
		}
		return true;
	}

	public unsafe static (AetheryteRecordState State, uint Cost) GetAetheryteState(AetheryteRecord aetheryte)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if (!GameState.IsLoggedIn)
		{
			return default((AetheryteRecordState, uint));
		}
		PlayerState* ptr = PlayerState.Instance();
		if (ptr == null)
		{
			return default((AetheryteRecordState, uint));
		}
		uint baseTeleportCost = TeleportCostCalculator.GetBaseTeleportCost(aetheryte.GetData(), GameState.TerritoryType);
		switch (aetheryte.Group)
		{
		case 254:
			baseTeleportCost = TeleportCostCalculator.GetBaseTeleportCost(LuminaGetter.GetRow<Aetheryte>(70u).GetValueOrDefault(), GameState.TerritoryType);
			return (State: AetheryteRecordState.None, Cost: baseTeleportCost);
		case 253:
			baseTeleportCost = TeleportCostCalculator.GetBaseTeleportCost(LuminaGetter.GetRow<Aetheryte>(175u).GetValueOrDefault(), GameState.TerritoryType);
			return (State: AetheryteRecordState.None, Cost: baseTeleportCost);
		default:
			if (!aetheryte.IsAetheryte)
			{
				return (State: AetheryteRecordState.None, Cost: baseTeleportCost);
			}
			if (((ReadOnlySpan<ushort>)((PlayerState)ptr).FreeAetheryteIds).Contains((ushort)aetheryte.RowID))
			{
				return (State: AetheryteRecordState.Free, Cost: 0u);
			}
			if (((ReadOnlySpan<ushort>)((PlayerState)ptr).FavouriteAetherytes).Contains((ushort)aetheryte.RowID))
			{
				return (State: AetheryteRecordState.Favorite, Cost: baseTeleportCost / 2);
			}
			if (((PlayerState)ptr).HomeAetheryteId == aetheryte.RowID)
			{
				return (State: AetheryteRecordState.Home, Cost: baseTeleportCost);
			}
			if (aetheryte.IsHouse)
			{
				uint item = baseTeleportCost / 4;
				return (State: AetheryteRecordState.None, Cost: item);
			}
			return (State: AetheryteRecordState.None, Cost: baseTeleportCost);
		}
	}

	public unsafe static bool UseAethernetToZone(uint territoryTypeID)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		AgentTelepotTown* ptr = AgentTelepotTown.Instance();
		if (ptr == null || !((AgentTelepotTown)ptr).IsAgentActive())
		{
			return false;
		}
		if (((AgentTelepotTown)ptr).Data == null)
		{
			return false;
		}
		for (byte b = 0; b < ((AgentTelepotTownData)((AgentTelepotTown)ptr).Data).Entries.Length; b++)
		{
			if (((AgentTelepotTownData)((AgentTelepotTown)ptr).Data).Entries[b].TerritoryTypeId == territoryTypeID)
			{
				((AgentTelepotTown)ptr).TeleportToAetheryte(b);
				return true;
			}
		}
		return false;
	}

	public virtual bool Equals(AetheryteRecord? other)
	{
		if (other?.RowID == RowID && other?.SubIndex == SubIndex)
		{
			return other?.Group == Group;
		}
		return false;
	}

	public override string ToString()
	{
		return $"AetheryteRecord.{RowID}-{SubIndex}-{Group}";
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(RowID, SubIndex, Group);
	}
}
