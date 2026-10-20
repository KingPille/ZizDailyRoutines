using System;
using System.Collections.Generic;
using System.Linq;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Dalamud.DataShare.Attributes;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;

namespace OmenTools.Info.Lumina;

public static class Sheets
{
	[DataShareTag]
	private const string DISPELLABLE_STATUSES_TAG = "OmenTools.Info.Game.Data.Sheets.DispellableStatuses";

	[DataShareTag]
	private const string PLAYER_ACTIONS_TAG = "OmenTools.Info.Game.Data.Sheets.PlayerActions";

	[DataShareTag]
	private const string PVP_ACTIONS_TAG = "OmenTools.Info.Game.Data.Sheets.PVPActions";

	[DataShareTag]
	private const string STATUSES_TAG = "OmenTools.Info.Game.Data.Sheets.Statuses";

	[DataShareTag]
	private const string CONTENTS_TAG = "OmenTools.Info.Game.Data.Sheets.Contents";

	[DataShareTag]
	private const string GEARS_TAG = "OmenTools.Info.Game.Data.Sheets.Gears";

	[DataShareTag]
	private const string WORLDS_TAG = "OmenTools.Info.Game.Data.Sheets.Worlds";

	[DataShareTag]
	private const string CN_WORLDS_TAG = "OmenTools.Info.Game.Data.Sheets.CNWorlds";

	[DataShareTag]
	private const string ZONES_TAG = "OmenTools.Info.Game.Data.Sheets.Zones";

	[DataShareTag]
	private const string MOUNTS_TAG = "OmenTools.Info.Game.Data.Sheets.Mounts";

	[DataShareTag]
	private const string FOOD_TAG = "OmenTools.Info.Game.Data.Sheets.Food";

	[DataShareTag]
	private const string SEEDS_TAG = "OmenTools.Info.Game.Data.Sheets.Seeds";

	[DataShareTag]
	private const string SOILS_TAG = "OmenTools.Info.Game.Data.Sheets.Soils";

	[DataShareTag]
	private const string FERTILIZERS_TAG = "OmenTools.Info.Game.Data.Sheets.Fertilizers";

	[DataShareTag]
	private const string MATERIAS_TAG = "OmenTools.Info.Game.Data.Sheets.Materias";

	[DataShareTag]
	private const string SPEED_DETECTION_ZONES_TAG = "OmenTools.Info.Game.Data.Sheets.SpeedDetectionZones";

	[DataShareTag]
	private const string AETHERYTES_TAG = "OmenTools.Info.Game.Data.Sheets.Aetherytes";

	[DataShareTag]
	private const string TARGET_AREA_ACTIONS_TAG = "OmenTools.Info.Game.Data.Sheets.TargetAreaActions";

	[DataShareTag]
	private const string MAP_TO_FINAL_TEXTURE_MAP_TAG = "OmenTools.Info.Game.Data.Sheets.MapToFinalTextureMap";

	[DataShareTag]
	private const string PLAYER_SEARCH_PLACE_NAME_TO_ZONES_TAG = "OmenTools.Info.Game.Data.Sheets.PlayerSearchPlaceNameToZones";

	[DataShareTag]
	private const string PLAYER_SEARCH_PLACE_NAMES_TAG = "OmenTools.Info.Game.Data.Sheets.PlayerSearchPlaceNames";

	public static Dictionary<uint, Status> DispellableStatuses { get; } = DalamudServiceExtension.Instance().GetOrCreateData<Dictionary<uint, Status>>("OmenTools.Info.Game.Data.Sheets.DispellableStatuses", (Func<Dictionary<uint, Status>>)(() => ((IEnumerable<Status>)LuminaGetter.Get<Status>()).Where((Status x) => ((Status)(ref x)).CanDispel && !string.IsNullOrEmpty(((object)((Status)(ref x)).Name/*cast due to constrained. prefix*/).ToString())).ToDictionary((Status x) => ((Status)(ref x)).RowId, (Status s) => s)));

	public static Dictionary<uint, Action> PlayerActions { get; } = DalamudServiceExtension.Instance().GetOrCreateData<Dictionary<uint, Action>>("OmenTools.Info.Game.Data.Sheets.PlayerActions", (Func<Dictionary<uint, Action>>)(() => (from x in ((IEnumerable<Action>)LuminaGetter.Get<Action>()).Where((Action x) => !string.IsNullOrEmpty(((object)((Action)(ref x)).Name/*cast due to constrained. prefix*/).ToString())).Where(delegate(Action x)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			ClassJobCategory? valueNullable = ((Action)(ref x)).ClassJobCategory.ValueNullable;
			object obj;
			if (!valueNullable.HasValue)
			{
				obj = null;
			}
			else
			{
				ClassJobCategory valueOrDefault = valueNullable.GetValueOrDefault();
				obj = ((object)((ClassJobCategory)(ref valueOrDefault)).Name/*cast due to constrained. prefix*/).ToString();
			}
			if (obj == null)
			{
				obj = string.Empty;
			}
			return !string.IsNullOrEmpty((string?)obj);
		})
		where (((Action)(ref x)).IsPlayerAction || ((Action)(ref x)).ClassJobLevel > 0) ? true : false
		orderby ((Action)(ref x)).ClassJob.RowId, ((Action)(ref x)).ClassJobLevel
		select x).ToDictionary((Action x) => ((Action)(ref x)).RowId, (Action x) => x)));

	public static Dictionary<uint, Action> PVPActions { get; } = DalamudServiceExtension.Instance().GetOrCreateData<Dictionary<uint, Action>>("OmenTools.Info.Game.Data.Sheets.PVPActions", (Func<Dictionary<uint, Action>>)(() => (from x in ((IEnumerable<SubrowCollection<PvPActionSort>>)LuminaGetter.GetSub<PvPActionSort>()).SelectMany((SubrowCollection<PvPActionSort> x) => (IEnumerable<PvPActionSort>)(object)x)
		where ((PvPActionSort)(ref x)).ActionType == 1
		select x).DistinctBy(delegate(PvPActionSort x)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		RowRef action = ((PvPActionSort)(ref x)).Action;
		return ((RowRef)(ref action)).RowId;
	}).ToDictionary(delegate(PvPActionSort x)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		RowRef action = ((PvPActionSort)(ref x)).Action;
		return ((RowRef)(ref action)).RowId;
	}, delegate(PvPActionSort x)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		RowRef action = ((PvPActionSort)(ref x)).Action;
		return LuminaGetter.GetRowOrDefault<Action>(((RowRef)(ref action)).RowId);
	})));

	public static Dictionary<uint, Status> Statuses { get; } = DalamudServiceExtension.Instance().GetOrCreateData<Dictionary<uint, Status>>("OmenTools.Info.Game.Data.Sheets.Statuses", (Func<Dictionary<uint, Status>>)(() => ((IEnumerable<Status>)LuminaGetter.Get<Status>()).Where((Status x) => !string.IsNullOrEmpty(((object)((Status)(ref x)).Name/*cast due to constrained. prefix*/).ToString())).ToDictionary((Status x) => ((Status)(ref x)).RowId, (Status x) => x)));

	public static Dictionary<uint, ContentFinderCondition> Contents { get; } = DalamudServiceExtension.Instance().GetOrCreateData<Dictionary<uint, ContentFinderCondition>>("OmenTools.Info.Game.Data.Sheets.Contents", (Func<Dictionary<uint, ContentFinderCondition>>)(() => (from x in ((IEnumerable<ContentFinderCondition>)LuminaGetter.Get<ContentFinderCondition>()).Where((ContentFinderCondition x) => !string.IsNullOrEmpty(((object)((ContentFinderCondition)(ref x)).Name/*cast due to constrained. prefix*/).ToString())).DistinctBy((ContentFinderCondition x) => ((ContentFinderCondition)(ref x)).TerritoryType.RowId)
		orderby ((ContentFinderCondition)(ref x)).ContentType.RowId, ((ContentFinderCondition)(ref x)).ClassJobLevelRequired
		select x).ToDictionary((ContentFinderCondition x) => ((ContentFinderCondition)(ref x)).TerritoryType.RowId, (ContentFinderCondition x) => x)));

	public static Dictionary<uint, Item> Gears { get; } = DalamudServiceExtension.Instance().GetOrCreateData<Dictionary<uint, Item>>("OmenTools.Info.Game.Data.Sheets.Gears", (Func<Dictionary<uint, Item>>)(() => ((IEnumerable<Item>)LuminaGetter.Get<Item>()).Where(delegate(Item x)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		EquipSlotCategory value = ((Item)(ref x)).EquipSlotCategory.Value;
		return ((EquipSlotCategory)(ref value)).RowId != 0;
	}).DistinctBy((Item x) => ((Item)(ref x)).RowId).ToDictionary((Item x) => ((Item)(ref x)).RowId, (Item x) => x)));

	public static Dictionary<uint, World> Worlds { get; } = DalamudServiceExtension.Instance().GetOrCreateData<Dictionary<uint, World>>("OmenTools.Info.Game.Data.Sheets.Worlds", (Func<Dictionary<uint, World>>)(() => ((IEnumerable<World>)LuminaGetter.Get<World>()).Where(delegate(World x)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		if (((World)(ref x)).DataCenter.RowId != 0 && ((World)(ref x)).DataCenter.RowId != 13 && ((World)(ref x)).UserType != 0 && ((World)(ref x)).Region != 0 && !string.IsNullOrEmpty(((object)((World)(ref x)).Name/*cast due to constrained. prefix*/).ToString()) && !string.IsNullOrEmpty(((object)((World)(ref x)).InternalName/*cast due to constrained. prefix*/).ToString()) && !((object)((World)(ref x)).Name/*cast due to constrained. prefix*/).ToString().Contains('-') && !((object)((World)(ref x)).Name/*cast due to constrained. prefix*/).ToString().Contains("Test"))
		{
			int num = (((World)(ref x)).Region - 1) * 1000;
			int num2 = ((World)(ref x)).Region * 1000;
			if (((World)(ref x)).RowId > num)
			{
				return ((World)(ref x)).RowId < num2;
			}
		}
		return false;
	}).ToDictionary((World x) => ((World)(ref x)).RowId, (World x) => x)));

	public static Dictionary<uint, World> CNWorlds { get; } = DalamudServiceExtension.Instance().GetOrCreateData<Dictionary<uint, World>>("OmenTools.Info.Game.Data.Sheets.CNWorlds", (Func<Dictionary<uint, World>>)(() => Worlds.Where(delegate(KeyValuePair<uint, World> x)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		uint key = x.Key;
		if (key > 1000 && key < 2000)
		{
			World value = x.Value;
			if (((World)(ref value)).DataCenter.RowId != 0)
			{
				value = x.Value;
				if (((World)(ref value)).Region == 2)
				{
					value = x.Value;
					WorldDCGroupType value2 = ((World)(ref value)).DataCenter.Value;
					if (((WorldDCGroupType)(ref value2)).Region.RowId == 5)
					{
						value = x.Value;
						if (((World)(ref value)).UserType == 101)
						{
							return x.Key != 1200;
						}
					}
				}
			}
		}
		return false;
	}).ToDictionary((KeyValuePair<uint, World> x) => x.Key, (KeyValuePair<uint, World> x) => x.Value)));

	public static Dictionary<uint, TerritoryType> Zones { get; } = DalamudServiceExtension.Instance().GetOrCreateData<Dictionary<uint, TerritoryType>>("OmenTools.Info.Game.Data.Sheets.Zones", (Func<Dictionary<uint, TerritoryType>>)(() => ((IEnumerable<TerritoryType>)LuminaGetter.Get<TerritoryType>()).Where((TerritoryType x) => ((TerritoryType)(ref x)).PlaceName.RowId != 0).ToDictionary((TerritoryType x) => ((TerritoryType)(ref x)).RowId, (TerritoryType x) => x)));

	public static Dictionary<uint, Mount> Mounts { get; } = DalamudServiceExtension.Instance().GetOrCreateData<Dictionary<uint, Mount>>("OmenTools.Info.Game.Data.Sheets.Mounts", (Func<Dictionary<uint, Mount>>)(() => ((IEnumerable<Mount>)LuminaGetter.Get<Mount>()).Where((Mount x) => !string.IsNullOrEmpty(((object)((Mount)(ref x)).Singular/*cast due to constrained. prefix*/).ToString()) && ((Mount)(ref x)).Icon > 0).ToDictionary((Mount x) => ((Mount)(ref x)).RowId, (Mount x) => x)));

	public static Dictionary<uint, Item> Food { get; } = DalamudServiceExtension.Instance().GetOrCreateData<Dictionary<uint, Item>>("OmenTools.Info.Game.Data.Sheets.Food", (Func<Dictionary<uint, Item>>)(() => ((IEnumerable<Item>)LuminaGetter.Get<Item>()).Where((Item x) => !string.IsNullOrEmpty(((object)((Item)(ref x)).Name/*cast due to constrained. prefix*/).ToString()) && ((Item)(ref x)).FilterGroup == 5).ToDictionary((Item x) => ((Item)(ref x)).RowId, (Item x) => x)));

	public static Dictionary<uint, Item> Seeds { get; } = DalamudServiceExtension.Instance().GetOrCreateData<Dictionary<uint, Item>>("OmenTools.Info.Game.Data.Sheets.Seeds", (Func<Dictionary<uint, Item>>)(() => ((IEnumerable<Item>)LuminaGetter.Get<Item>()).Where((Item x) => ((Item)(ref x)).FilterGroup == 20).ToDictionary((Item x) => ((Item)(ref x)).RowId, (Item x) => x)));

	public static Dictionary<uint, Item> Soils { get; } = DalamudServiceExtension.Instance().GetOrCreateData<Dictionary<uint, Item>>("OmenTools.Info.Game.Data.Sheets.Soils", (Func<Dictionary<uint, Item>>)(() => ((IEnumerable<Item>)LuminaGetter.Get<Item>()).Where((Item x) => ((Item)(ref x)).FilterGroup == 21).ToDictionary((Item x) => ((Item)(ref x)).RowId, (Item x) => x)));

	public static Dictionary<uint, Item> Fertilizers { get; } = DalamudServiceExtension.Instance().GetOrCreateData<Dictionary<uint, Item>>("OmenTools.Info.Game.Data.Sheets.Fertilizers", (Func<Dictionary<uint, Item>>)(() => ((IEnumerable<Item>)LuminaGetter.Get<Item>()).Where((Item x) => ((Item)(ref x)).FilterGroup == 22).ToDictionary((Item x) => ((Item)(ref x)).RowId, (Item x) => x)));

	public static Dictionary<uint, Item> Materias { get; } = DalamudServiceExtension.Instance().GetOrCreateData<Dictionary<uint, Item>>("OmenTools.Info.Game.Data.Sheets.Materias", (Func<Dictionary<uint, Item>>)(() => ((IEnumerable<Item>)LuminaGetter.Get<Item>()).Where((Item x) => !string.IsNullOrEmpty(((object)((Item)(ref x)).Name/*cast due to constrained. prefix*/).ToString()) && ((Item)(ref x)).FilterGroup == 13).ToDictionary((Item x) => ((Item)(ref x)).RowId, (Item x) => x)));

	public static Dictionary<uint, TerritoryType> SpeedDetectionZones { get; } = DalamudServiceExtension.Instance().GetOrCreateData<Dictionary<uint, TerritoryType>>("OmenTools.Info.Game.Data.Sheets.SpeedDetectionZones", (Func<Dictionary<uint, TerritoryType>>)delegate
	{
		HashSet<uint> limitedIntendedUse = ((!GameState.IsGL) ? new HashSet<uint> { 1u, 18u, 31u, 41u, 47u, 48u, 52u, 53u, 61u } : new HashSet<uint> { 18u, 31u, 41u, 48u, 52u, 53u });
		return ((IEnumerable<TerritoryType>)LuminaGetter.Get<TerritoryType>()).Where((TerritoryType x) => limitedIntendedUse.Contains(((TerritoryType)(ref x)).TerritoryIntendedUse.RowId)).ToDictionary((TerritoryType x) => ((TerritoryType)(ref x)).RowId, (TerritoryType x) => x);
	});

	public static Dictionary<uint, Aetheryte> Aetherytes { get; } = DalamudServiceExtension.Instance().GetOrCreateData<Dictionary<uint, Aetheryte>>("OmenTools.Info.Game.Data.Sheets.Aetherytes", (Func<Dictionary<uint, Aetheryte>>)(() => ((IEnumerable<Aetheryte>)LuminaGetter.Get<Aetheryte>()).Where(delegate(Aetheryte x)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		PlaceName? valueNullable = ((Aetheryte)(ref x)).PlaceName.ValueNullable;
		object value;
		if (!valueNullable.HasValue)
		{
			value = null;
		}
		else
		{
			PlaceName valueOrDefault = valueNullable.GetValueOrDefault();
			ReadOnlySeString name = ((PlaceName)(ref valueOrDefault)).Name;
			value = ((ReadOnlySeString)(ref name)).ExtractText();
		}
		return !string.IsNullOrEmpty((string?)value);
	}).ToDictionary((Aetheryte x) => ((Aetheryte)(ref x)).RowId, (Aetheryte x) => x)));

	public static Dictionary<uint, Action> TargetAreaActions { get; } = DalamudServiceExtension.Instance().GetOrCreateData<Dictionary<uint, Action>>("OmenTools.Info.Game.Data.Sheets.TargetAreaActions", (Func<Dictionary<uint, Action>>)(() => ((IEnumerable<Action>)LuminaGetter.Get<Action>()).Where((Action x) => ((Action)(ref x)).TargetArea).ToDictionary((Action x) => ((Action)(ref x)).RowId, (Action x) => x)));

	public static Dictionary<uint, Map> MapToFinalTextureMap { get; } = DalamudServiceExtension.Instance().GetOrCreateData<Dictionary<uint, Map>>("OmenTools.Info.Game.Data.Sheets.MapToFinalTextureMap", (Func<Dictionary<uint, Map>>)delegate
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		ExcelSheet<Map> val = LuminaGetter.Get<Map>();
		int count;
		Dictionary<uint, Map> result = new Dictionary<uint, Map>(((IEnumerable<Map>)val).TryGetNonEnumeratedCount(out count) ? count : 0);
		List<uint> groupRowIDs = new List<uint>();
		string id = null;
		Map groupFinalMap = default(Map);
		uint num = 0u;
		Enumerator<Map> enumerator = val.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Map current = enumerator.Current;
				string text = ((object)((Map)(ref current)).Id/*cast due to constrained. prefix*/).ToString();
				if (groupRowIDs.Count <= 0 || ((Map)(ref current)).RowId != num + 1 || !TypePart(text).Equals(TypePart(id), StringComparison.Ordinal))
				{
					FlushGroup();
					id = text;
					groupRowIDs.Clear();
				}
				groupRowIDs.Add(((Map)(ref current)).RowId);
				groupFinalMap = current;
				num = ((Map)(ref current)).RowId;
				static ReadOnlySpan<char> TypePart(string? text2)
				{
					ReadOnlySpan<char> readOnlySpan = text2.AsSpan();
					int num2 = readOnlySpan.IndexOf('/');
					if (num2 < 0)
					{
						return readOnlySpan;
					}
					return readOnlySpan.Slice(0, num2);
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
		FlushGroup();
		return result;
		void FlushGroup()
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			if (groupRowIDs.Count == 0)
			{
				return;
			}
			foreach (uint item in groupRowIDs)
			{
				result[item] = groupFinalMap;
			}
		}
	});

	public static Dictionary<uint, HashSet<uint>> PlayerSearchPlaceNameToZones { get; } = DalamudServiceExtension.Instance().GetOrCreateData<Dictionary<uint, HashSet<uint>>>("OmenTools.Info.Game.Data.Sheets.PlayerSearchPlaceNameToZones", (Func<Dictionary<uint, HashSet<uint>>>)(() => (from x in ((IEnumerable<PlayerSearchSubLocation>)LuminaGetter.Get<PlayerSearchSubLocation>()).Where(delegate(PlayerSearchSubLocation x)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			uint rowId = ((PlayerSearchSubLocation)(ref x)).PlaceName.RowId;
			return rowId != 0 && rowId != 519;
		})
		select ((PlayerSearchSubLocation)(ref x)).PlaceName.RowId).Distinct().ToDictionary((uint x) => x, (uint x) => (from d in ((IEnumerable<TerritoryType>)LuminaGetter.Get<TerritoryType>()).Where(delegate(TerritoryType d)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			bool flag = ((TerritoryType)(ref d)).PlaceNameZone.RowId == x;
			if (flag)
			{
				uint rowId = ((TerritoryType)(ref d)).TerritoryIntendedUse.RowId;
				bool flag2 = ((rowId <= 1 || rowId == 23) ? true : false);
				flag = flag2;
			}
			return flag;
		})
		select ((TerritoryType)(ref d)).RowId).ToHashSet())));

	public static Dictionary<uint, PlayerSearchSubLocation> PlayerSearchPlaceNames { get; } = DalamudServiceExtension.Instance().GetOrCreateData<Dictionary<uint, PlayerSearchSubLocation>>("OmenTools.Info.Game.Data.Sheets.PlayerSearchPlaceNames", (Func<Dictionary<uint, PlayerSearchSubLocation>>)(() => ((IEnumerable<PlayerSearchSubLocation>)LuminaGetter.Get<PlayerSearchSubLocation>()).Where(delegate(PlayerSearchSubLocation x)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		uint rowId = ((PlayerSearchSubLocation)(ref x)).PlaceName.RowId;
		return rowId != 0 && rowId != 519;
	}).DistinctBy((PlayerSearchSubLocation x) => ((PlayerSearchSubLocation)(ref x)).PlaceName.RowId).ToDictionary((PlayerSearchSubLocation x) => ((PlayerSearchSubLocation)(ref x)).PlaceName.RowId, (PlayerSearchSubLocation x) => x)));
}
