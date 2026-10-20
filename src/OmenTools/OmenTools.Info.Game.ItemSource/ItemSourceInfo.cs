using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Lumina.Data.Files;
using Lumina.Data.Parsing.Layer;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Dalamud;
using OmenTools.Extensions;
using OmenTools.Info.Game.ItemSource.Enums;
using OmenTools.Info.Game.ItemSource.Models;
using OmenTools.Info.Lumina.Enums;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Info.Game.ItemSource;

public sealed class ItemSourceInfo
{
	private sealed class BuildState(Dictionary<uint, ItemSourceInfo> itemInfos)
	{
		public BuildContext Context { get; } = BuildContext.Create();

		public ItemSourceAccumulator Accumulator { get; } = new ItemSourceAccumulator(itemInfos);
	}

	private sealed class BuildContext
	{
		public Dictionary<uint, string> AchievementDescriptions { get; }

		public Dictionary<ulong, uint> MapRowIDs { get; }

		public Dictionary<uint, uint> TomestoneItemIDs { get; }

		public Dictionary<uint, List<GCScripShopCategory>> GcCategories { get; }

		public bool ShouldHideDefaultShopName { get; }

		private BuildContext(Dictionary<uint, string> achievementDescriptions, Dictionary<ulong, uint> mapRowIDs, Dictionary<uint, uint> tomestoneItemIDs, Dictionary<uint, List<GCScripShopCategory>> gcCategories, bool shouldHideDefaultShopName)
		{
			AchievementDescriptions = achievementDescriptions;
			MapRowIDs = mapRowIDs;
			TomestoneItemIDs = tomestoneItemIDs;
			GcCategories = gcCategories;
			ShouldHideDefaultShopName = shouldHideDefaultShopName;
		}

		public static BuildContext Create()
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Invalid comparison between Unknown and I4
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			Dictionary<uint, string> dictionary = new Dictionary<uint, string>();
			Dictionary<ulong, uint> dictionary2 = new Dictionary<ulong, uint>();
			Dictionary<uint, uint> dictionary3 = new Dictionary<uint, uint>();
			Dictionary<uint, List<GCScripShopCategory>> dictionary4 = new Dictionary<uint, List<GCScripShopCategory>>();
			Enumerator<Achievement> enumerator = LuminaGetter.Get<Achievement>().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Achievement current = enumerator.Current;
					uint rowId = ((Achievement)(ref current)).Item.RowId;
					if (rowId != 0 && !dictionary.ContainsKey(rowId))
					{
						dictionary.Add(rowId, ((object)((Achievement)(ref current)).Description/*cast due to constrained. prefix*/).ToString());
					}
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
			}
			Enumerator<Map> enumerator2 = LuminaGetter.Get<Map>().GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					Map current2 = enumerator2.Current;
					dictionary2.TryAdd(GetMapKey(((Map)(ref current2)).TerritoryType.RowId, (uint)((Map)(ref current2)).MapIndex), ((Map)(ref current2)).RowId);
				}
			}
			finally
			{
				((IDisposable)enumerator2/*cast due to constrained. prefix*/).Dispose();
			}
			Enumerator<TomestonesItem> enumerator3 = LuminaGetter.Get<TomestonesItem>().GetEnumerator();
			try
			{
				while (enumerator3.MoveNext())
				{
					TomestonesItem current3 = enumerator3.Current;
					Tomestones value = ((TomestonesItem)(ref current3)).Tomestones.Value;
					uint rowId2 = ((Tomestones)(ref value)).RowId;
					uint rowId3 = ((TomestonesItem)(ref current3)).Item.RowId;
					if (rowId2 != 0 && rowId3 != 0)
					{
						dictionary3.TryAdd(rowId2, rowId3);
					}
				}
			}
			finally
			{
				((IDisposable)enumerator3/*cast due to constrained. prefix*/).Dispose();
			}
			Enumerator<GCScripShopCategory> enumerator4 = LuminaGetter.Get<GCScripShopCategory>().GetEnumerator();
			try
			{
				while (enumerator4.MoveNext())
				{
					GCScripShopCategory current4 = enumerator4.Current;
					uint rowId4 = ((GCScripShopCategory)(ref current4)).GrandCompany.RowId;
					if (rowId4 != 0)
					{
						bool exists;
						ref List<GCScripShopCategory> valueRefOrAddDefault = ref CollectionsMarshal.GetValueRefOrAddDefault(dictionary4, rowId4, out exists);
						if (!exists || valueRefOrAddDefault == null)
						{
							valueRefOrAddDefault = new List<GCScripShopCategory>();
						}
						valueRefOrAddDefault.Add(current4);
					}
				}
			}
			finally
			{
				((IDisposable)enumerator4/*cast due to constrained. prefix*/).Dispose();
			}
			return new BuildContext(dictionary, dictionary2, dictionary3, dictionary4, (int)GameState.ClientLanguage != 1);
		}

		public bool TryGetMapRowID(uint territoryID, uint mapIndex, out uint mapRowID)
		{
			return MapRowIDs.TryGetValue(GetMapKey(territoryID, mapIndex), out mapRowID);
		}

		private static ulong GetMapKey(uint territoryID, uint mapIndex)
		{
			return ((ulong)territoryID << 32) | mapIndex;
		}
	}

	private sealed class ItemSourceAccumulator(Dictionary<uint, ItemSourceInfo> itemInfos)
	{
		private readonly Dictionary<uint, Dictionary<uint, int>> npcIndexByItemID = new Dictionary<uint, Dictionary<uint, int>>();

		public void AddItem(uint itemID, uint npcID, string npcName, string? shopName, List<ShopItemCostInfo> cost, ShopNPCLocation? npcLocation, ItemShopType shopType, string achievementDesc)
		{
			if (npcLocation != null)
			{
				bool exists;
				ref ItemSourceInfo valueRefOrAddDefault = ref CollectionsMarshal.GetValueRefOrAddDefault(itemInfos, itemID, out exists);
				if (!exists || valueRefOrAddDefault == null)
				{
					valueRefOrAddDefault = new ItemSourceInfo
					{
						ItemID = itemID,
						NPCInfos = new List<ShopNPCInfos>(),
						ShopType = shopType,
						AchievementDescription = achievementDesc
					};
				}
				if (shopType == ItemShopType.Achievement && valueRefOrAddDefault.ShopType != ItemShopType.Achievement)
				{
					valueRefOrAddDefault.ShopType = ItemShopType.Achievement;
					valueRefOrAddDefault.AchievementDescription = achievementDesc;
				}
				bool exists2;
				ref Dictionary<uint, int> valueRefOrAddDefault2 = ref CollectionsMarshal.GetValueRefOrAddDefault(npcIndexByItemID, itemID, out exists2);
				if (!exists2 || valueRefOrAddDefault2 == null)
				{
					valueRefOrAddDefault2 = new Dictionary<uint, int>();
				}
				if (valueRefOrAddDefault2.TryAdd(npcID, valueRefOrAddDefault.NPCInfos.Count))
				{
					valueRefOrAddDefault.NPCInfos.Add(new ShopNPCInfos
					{
						ID = npcID,
						Location = npcLocation,
						Name = npcName,
						CostInfos = cost,
						ShopName = shopName
					});
				}
			}
		}

		public void AddCost(uint itemID, uint npcID, List<ShopItemCostInfo> cost)
		{
			if (itemInfos.TryGetValue(itemID, out ItemSourceInfo value) && npcIndexByItemID.TryGetValue(itemID, out Dictionary<uint, int> value2) && value2.TryGetValue(npcID, out var value3))
			{
				value.NPCInfos[value3].CostInfos.AddRange(cost);
			}
		}
	}

	private enum EventHandlerType : uint
	{
		GilShop = 4u,
		CustomTalk = 11u,
		GcShop = 22u,
		SpecialShop = 27u,
		FcShop = 42u,
		TopicSelect = 50u,
		PreHandler = 54u,
		InclusionShop = 58u,
		CollectablesShop = 59u
	}

	private const string DEFAULT_SHOP_NAME = "アイテムの購入";

	private static readonly FrozenDictionary<uint, uint> Currencies = new Dictionary<uint, uint>
	{
		[1u] = 28u,
		[2u] = 33913u,
		[4u] = 33914u,
		[6u] = 41784u,
		[7u] = 41785u
	}.ToFrozenDictionary();

	private static readonly FrozenDictionary<uint, uint> SHBFateShopNPC = new Dictionary<uint, uint>
	{
		[1027998u] = 1769957u,
		[1027538u] = 1769958u,
		[1027385u] = 1769959u,
		[1027497u] = 1769960u,
		[1027892u] = 1769961u,
		[1027665u] = 1769962u,
		[1027709u] = 1769963u,
		[1027766u] = 1769964u
	}.ToFrozenDictionary();

	private static readonly FrozenDictionary<string, string> MJISpecialShopNames = new Dictionary<string, string>
	{
		["0"] = "TEXT_CTSMJISPECIALSHOP_00789_TALK_ACTOR",
		["1"] = "TEXT_CTSMJISPECIALSHOP_00789_SYSTEM_000_000",
		["2"] = "TEXT_CTSMJISPECIALSHOP_00789_SYSTEM_000_005",
		["3"] = "TEXT_CTSMJISPECIALSHOP_00789_Q1_000_000",
		["4"] = "TEXT_CTSMJISPECIALSHOP_00789_Q1_000_005",
		["5"] = "TEXT_CTSMJISPECIALSHOP_00789_Q1_000_010",
		["6"] = "TEXT_CTSMJISPECIALSHOP_00789_Q1_000_015",
		["7"] = "TEXT_CTSMJISPECIALSHOP_00789_Q1_000_020",
		["8"] = "TEXT_CTSMJISPECIALSHOP_00789_Q1_000_025",
		["9"] = "TEXT_CTSMJISPECIALSHOP_00789_Q1_000_030",
		["10"] = "TEXT_CTSMJISPECIALSHOP_00789_Q2_000_000",
		["11"] = "TEXT_CTSMJISPECIALSHOP_00789_Q2_000_005",
		["12"] = "TEXT_CTSMJISPECIALSHOP_00789_Q2_000_010",
		["13"] = "TEXT_CTSMJISPECIALSHOP_00789_Q2_000_015",
		["14"] = "TEXT_CTSMJISPECIALSHOP_00789_OMISE_100_000",
		["15"] = "TEXT_CTSMJISPECIALSHOP_00789_SYSTEM_100_000",
		["16"] = "TEXT_CTSMJISPECIALSHOP_00789_OMISE_200_000"
	}.ToFrozenDictionary();

	private static readonly List<Item> GrandCompanySeals = (from i in ((IEnumerable<Item>)LuminaGetter.Get<Item>()).Where(delegate(Item i)
		{
			uint rowId = ((Item)(ref i)).RowId;
			return rowId >= 20 && rowId <= 22;
		})
		select (i)).ToList();

	private static BuildState? ActiveBuildState { get; set; }

	public uint ItemID { get; init; }

	public List<ShopNPCInfos> NPCInfos { get; internal set; } = new List<ShopNPCInfos>();

	public ItemShopType ShopType { get; internal set; }

	public string AchievementDescription { get; internal set; } = string.Empty;

	private static BuildContext CurrentBuildContext => ActiveBuildState?.Context ?? throw new InvalidOperationException("ItemSourceInfo 构建上下文尚未初始化");

	private static ItemSourceAccumulator CurrentItemAccumulator => ActiveBuildState?.Accumulator ?? throw new InvalidOperationException("ItemSourceInfo 构建器尚未初始化");

	public static ItemSourceQueryResult Query(uint itemID)
	{
		return OmenServiceBase<ItemSourceManager>.Instance().Query(itemID);
	}

	public static ExchangeItemsQueryResult QueryExchangeItems(uint costItemID)
	{
		return OmenServiceBase<ItemSourceManager>.Instance().QueryExchangeItems(costItemID);
	}

	internal static Dictionary<uint, ItemSourceInfo> BuildAllItems()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		Stopwatch stopwatch = Stopwatch.StartNew();
		DLog.Debug("[ItemSourceInfo] 开始构建快照源数据");
		Dictionary<uint, ItemSourceInfo> itemToItemShopInfos = new Dictionary<uint, ItemSourceInfo>();
		Dictionary<uint, ShopNPCLocation> npcIDToLocations = new Dictionary<uint, ShopNPCLocation>();
		try
		{
			ActiveBuildState = new BuildState(itemToItemShopInfos);
			BuildNPCLocations(ref npcIDToLocations);
			CorrectNPCLocations(ref npcIDToLocations);
			Enumerator<ENpcBase> enumerator = LuminaGetter.Get<ENpcBase>().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					BuildNPCInfo(enumerator.Current, ref itemToItemShopInfos, ref npcIDToLocations);
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
			}
			AddAchievementItem(ref itemToItemShopInfos, ref npcIDToLocations);
			return itemToItemShopInfos;
		}
		finally
		{
			ActiveBuildState = null;
			stopwatch.Stop();
			DLog.Debug($"[ItemSourceInfo] 快照源数据构建完毕, 用时: {stopwatch.Elapsed}");
		}
	}

	private static void BuildNPCLocations(ref Dictionary<uint, ShopNPCLocation> npcIDToLocations)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		HashSet<uint> hashSet = new HashSet<uint>();
		Enumerator<Aetheryte> enumerator = LuminaGetter.Get<Aetheryte>().GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Aetheryte current = enumerator.Current;
				if (((Aetheryte)(ref current)).Territory.IsValid && ((Aetheryte)(ref current)).Territory.RowId != 1)
				{
					TerritoryType zone = ((Aetheryte)(ref current)).Territory.Value;
					if (!hashSet.Contains(((TerritoryType)(ref zone)).RowId) && zone.TryGetLGB(LGBFileType.PlanEvent, out LgbFile lgbFile))
					{
						ParseLgbFile(lgbFile, zone, ref npcIDToLocations);
						hashSet.Add(((TerritoryType)(ref zone)).RowId);
					}
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
		Enumerator<TerritoryType> enumerator2 = LuminaGetter.Get<TerritoryType>().GetEnumerator();
		try
		{
			while (enumerator2.MoveNext())
			{
				TerritoryType zone2 = enumerator2.Current;
				ContentFinderCondition value = ((TerritoryType)(ref zone2)).ContentFinderCondition.Value;
				bool flag;
				switch (((ContentFinderCondition)(ref value)).ContentType.RowId)
				{
				case 16u:
				case 26u:
				case 29u:
				case 38u:
					flag = true;
					break;
				default:
					flag = false;
					break;
				}
				if (flag && zone2.TryGetLGB(LGBFileType.PlanEvent, out LgbFile lgbFile2))
				{
					ParseLgbFile(lgbFile2, zone2, ref npcIDToLocations);
				}
			}
		}
		finally
		{
			((IDisposable)enumerator2/*cast due to constrained. prefix*/).Dispose();
		}
		Enumerator<Level> enumerator3 = LuminaGetter.Get<Level>().GetEnumerator();
		try
		{
			while (enumerator3.MoveNext())
			{
				Level current2 = enumerator3.Current;
				if (((Level)(ref current2)).Type != 8 || !((Level)(ref current2)).Territory.ValueNullable.HasValue)
				{
					continue;
				}
				Dictionary<uint, ShopNPCLocation> obj = npcIDToLocations;
				RowRef val = ((Level)(ref current2)).Object;
				if (!obj.ContainsKey(((RowRef)(ref val)).RowId))
				{
					val = ((Level)(ref current2)).Object;
					if (LuminaGetter.TryGetRow<ENpcBase>(((RowRef)(ref val)).RowId, out ENpcBase item) && HasRelevantEventHandler(item))
					{
						Dictionary<uint, ShopNPCLocation> obj2 = npcIDToLocations;
						val = ((Level)(ref current2)).Object;
						obj2.Add(((RowRef)(ref val)).RowId, new ShopNPCLocation(((Level)(ref current2)).X, ((Level)(ref current2)).Z, ((Level)(ref current2)).Territory.RowId));
					}
				}
			}
		}
		finally
		{
			((IDisposable)enumerator3/*cast due to constrained. prefix*/).Dispose();
		}
		Enumerator<HousingEmploymentNpcList> enumerator4 = LuminaGetter.GetSub<HousingEmploymentNpcList>().GetEnumerator();
		try
		{
			while (enumerator4.MoveNext())
			{
				Enumerator<HousingEmploymentNpcList> enumerator5 = enumerator4.Current.GetEnumerator();
				try
				{
					while (enumerator5.MoveNext())
					{
						HousingEmploymentNpcList current3 = enumerator5.Current;
						RowRef<ENpcBase> maleENpcBase = ((HousingEmploymentNpcList)(ref current3)).MaleENpcBase;
						if (maleENpcBase.RowId != 0)
						{
							npcIDToLocations.Add(maleENpcBase.RowId, new ShopNPCLocation(0f, 0f, 282u));
						}
					}
				}
				finally
				{
					((IDisposable)enumerator5/*cast due to constrained. prefix*/).Dispose();
				}
			}
		}
		finally
		{
			((IDisposable)enumerator4/*cast due to constrained. prefix*/).Dispose();
		}
	}

	private static void CorrectNPCLocations(ref Dictionary<uint, ShopNPCLocation> npcIDToLocations)
	{
		npcIDToLocations[1019100u] = new ShopNPCLocation(-85.03851f, 117.05188f, 641u);
		npcIDToLocations[1022846u] = new ShopNPCLocation(-83.93994f, 115.31238f, 641u);
		npcIDToLocations[1019106u] = new ShopNPCLocation(-99.22949f, 105.6687f, 641u);
		npcIDToLocations[1019107u] = new ShopNPCLocation(-100.26703f, 107.43872f, 641u);
		npcIDToLocations[1019104u] = new ShopNPCLocation(-67.582275f, 59.739014f, 641u);
		npcIDToLocations[1019102u] = new ShopNPCLocation(-59.617065f, 33.524048f, 641u);
		npcIDToLocations[1019103u] = new ShopNPCLocation(-52.35376f, 76.58496f, 641u);
		npcIDToLocations[1019101u] = new ShopNPCLocation(-36.484375f, 49.240845f, 641u);
		npcIDToLocations[1004418u] = new ShopNPCLocation(-114.0307f, 118.30322f, 131u, 73u);
		npcIDToLocations.TryAdd(1006004u, new ShopNPCLocation(5.355835f, 155.22998f, 128u));
		npcIDToLocations.TryAdd(1017613u, new ShopNPCLocation(2.822865f, 153.521f, 128u));
		npcIDToLocations.TryAdd(1003633u, new ShopNPCLocation(-259.32715f, 37.491333f, 129u));
		npcIDToLocations.TryAdd(1008145u, new ShopNPCLocation(-31.265808f, -245.38031f, 133u));
		npcIDToLocations.TryAdd(1006005u, new ShopNPCLocation(-61.234497f, -141.31384f, 133u));
		npcIDToLocations.TryAdd(1017614u, new ShopNPCLocation(-58.79309f, -142.1073f, 133u));
		npcIDToLocations.TryAdd(1003077u, new ShopNPCLocation(145.83044f, -106.767456f, 133u));
		npcIDToLocations.TryAdd(1000215u, new ShopNPCLocation(155.35205f, -70.26782f, 133u));
		npcIDToLocations.TryAdd(1000996u, new ShopNPCLocation(-28.152893f, 196.70398f, 128u));
		npcIDToLocations.TryAdd(1000999u, new ShopNPCLocation(-29.465149f, 197.92468f, 128u));
		npcIDToLocations.TryAdd(1000217u, new ShopNPCLocation(170.30591f, -73.16705f, 133u));
		npcIDToLocations.TryAdd(1000597u, new ShopNPCLocation(-163.07324f, -78.62976f, 153u));
		npcIDToLocations.TryAdd(1000185u, new ShopNPCLocation(-8.590881f, -2.2125854f, 132u));
		npcIDToLocations.TryAdd(1000392u, new ShopNPCLocation(-17.746277f, 43.35083f, 132u));
		npcIDToLocations.TryAdd(1000391u, new ShopNPCLocation(66.819214f, -143.45007f, 133u));
		npcIDToLocations.TryAdd(1000232u, new ShopNPCLocation(164.72107f, -133.68433f, 133u));
		npcIDToLocations.TryAdd(1000301u, new ShopNPCLocation(-87.174866f, -173.51044f, 133u));
		npcIDToLocations.TryAdd(1000267u, new ShopNPCLocation(103.89868f, -213.03125f, 133u));
		npcIDToLocations.TryAdd(1003252u, new ShopNPCLocation(-139.57434f, 31.967651f, 129u));
		npcIDToLocations.TryAdd(1001016u, new ShopNPCLocation(-42.679565f, 119.920654f, 128u));
		npcIDToLocations.TryAdd(1005422u, new ShopNPCLocation(-397.6349f, 80.979614f, 129u));
		npcIDToLocations.TryAdd(1000244u, new ShopNPCLocation(423.17834f, -119.95117f, 154u));
		npcIDToLocations.TryAdd(1000234u, new ShopNPCLocation(423.69714f, -122.08746f, 154u));
		npcIDToLocations.TryAdd(1000230u, new ShopNPCLocation(421.46936f, -125.993774f, 154u));
		npcIDToLocations.TryAdd(1000222u, new ShopNPCLocation(-213.94684f, 300.4348f, 152u));
		npcIDToLocations.TryAdd(1000535u, new ShopNPCLocation(-579.4003f, 104.32593f, 152u));
		npcIDToLocations.TryAdd(1002371u, new ShopNPCLocation(-480.91858f, 201.9226f, 152u));
		npcIDToLocations.TryAdd(1000396u, new ShopNPCLocation(82.597046f, -103.349365f, 148u));
		npcIDToLocations.TryAdd(1000220u, new ShopNPCLocation(16.189758f, -15.640564f, 148u));
		npcIDToLocations.TryAdd(1000717u, new ShopNPCLocation(175.61597f, 319.32544f, 148u));
		npcIDToLocations.TryAdd(1000718u, new ShopNPCLocation(332.23462f, 332.47876f, 154u));
		npcIDToLocations.TryAdd(1002376u, new ShopNPCLocation(10.635498f, 220.20288f, 154u));
		npcIDToLocations.TryAdd(1002374u, new ShopNPCLocation(204.39453f, -65.75122f, 153u));
		npcIDToLocations.TryAdd(1000579u, new ShopNPCLocation(16.03717f, 220.50806f, 152u));
		npcIDToLocations.TryAdd(1002377u, new ShopNPCLocation(11.062683f, 221.57617f, 154u));
		npcIDToLocations.TryAdd(1002375u, new ShopNPCLocation(203.75366f, -64.560974f, 153u));
		npcIDToLocations.TryAdd(1002389u, new ShopNPCLocation(95.8114f, 67.61267f, 128u));
	}

	private static void BuildNPCInfo(ENpcBase npcBase, ref Dictionary<uint, ItemSourceInfo> itemToItemShopInfos, ref Dictionary<uint, ShopNPCLocation> npcIDToLocations)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_0409: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		ENpcResident rowOrDefault = LuminaGetter.GetRowOrDefault<ENpcResident>(((ENpcBase)(ref npcBase)).RowId);
		if (FixNPCInfo(npcBase, rowOrDefault, ref itemToItemShopInfos, ref npcIDToLocations))
		{
			return;
		}
		FateShop? row = LuminaGetter.GetRow<FateShop>(((ENpcBase)(ref npcBase)).RowId);
		if (row.HasValue)
		{
			FateShop value = row.Value;
			Enumerator<RowRef<SpecialShop>> enumerator = ((FateShop)(ref value)).SpecialShop.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					SpecialShop? row2 = LuminaGetter.GetRow<SpecialShop>(enumerator.Current.RowId);
					if (row2.HasValue)
					{
						AddSpecialItem(row2.Value, npcBase, rowOrDefault, ref itemToItemShopInfos, ref npcIDToLocations);
					}
				}
				return;
			}
			finally
			{
				((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
			}
		}
		uint rowId = ((ENpcBase)(ref npcBase)).RowId;
		string npcName = ((object)((ENpcResident)(ref rowOrDefault)).Singular/*cast due to constrained. prefix*/).ToString();
		ShopNPCLocation valueOrDefault = npcIDToLocations.GetValueOrDefault(rowId);
		Enumerator<RowRef> enumerator2 = ((ENpcBase)(ref npcBase)).ENpcData.GetEnumerator();
		try
		{
			while (enumerator2.MoveNext())
			{
				RowRef current = enumerator2.Current;
				uint rowId2 = ((RowRef)(ref current)).RowId;
				if (rowId2 == 0)
				{
					break;
				}
				if (!TryGetEventHandlerType(rowId2, out var type))
				{
					continue;
				}
				switch (type)
				{
				case EventHandlerType.CollectablesShop:
					AddCollectablesShop(LuminaGetter.GetRowOrDefault<CollectablesShop>(rowId2), npcBase, rowOrDefault, ref itemToItemShopInfos, ref npcIDToLocations);
					break;
				case EventHandlerType.InclusionShop:
					AddInclusionShop(LuminaGetter.GetRowOrDefault<InclusionShop>(rowId2), npcBase, rowOrDefault, ref itemToItemShopInfos, ref npcIDToLocations);
					break;
				case EventHandlerType.FcShop:
					AddFccShop(LuminaGetter.GetRowOrDefault<FccShop>(rowId2), npcBase, rowOrDefault, ref itemToItemShopInfos, ref npcIDToLocations);
					break;
				case EventHandlerType.PreHandler:
					AddItemsInPrehandler(LuminaGetter.GetRowOrDefault<PreHandler>(rowId2), npcBase, rowOrDefault, ref itemToItemShopInfos, ref npcIDToLocations);
					break;
				case EventHandlerType.TopicSelect:
					AddItemsInTopicSelect(LuminaGetter.GetRowOrDefault<TopicSelect>(rowId2), npcBase, rowOrDefault, ref itemToItemShopInfos, ref npcIDToLocations);
					break;
				case EventHandlerType.GcShop:
					AddGcShopItem(LuminaGetter.GetRowOrDefault<GCShop>(rowId2), npcBase, rowOrDefault, ref itemToItemShopInfos, ref npcIDToLocations);
					break;
				case EventHandlerType.SpecialShop:
				{
					SpecialShop rowOrDefault2 = LuminaGetter.GetRowOrDefault<SpecialShop>(rowId2);
					AddSpecialItem(rowOrDefault2, npcBase, rowOrDefault, ref itemToItemShopInfos, ref npcIDToLocations, ItemShopType.SpecialShop, ((object)((SpecialShop)(ref rowOrDefault2)).Name/*cast due to constrained. prefix*/).ToString());
					break;
				}
				case EventHandlerType.GilShop:
					AddGilShopItem(LuminaGetter.GetRowOrDefault<GilShop>(rowId2), npcBase, rowOrDefault, ref itemToItemShopInfos, ref npcIDToLocations);
					break;
				case EventHandlerType.CustomTalk:
				{
					if (!LuminaGetter.TryGetRow<CustomTalk>(rowId2, out CustomTalk item))
					{
						return;
					}
					uint num = 0u;
					uint cost = 0u;
					int num2 = 0;
					Enumerator<ScriptStruct> enumerator3 = ((CustomTalk)(ref item)).Script.GetEnumerator();
					try
					{
						while (enumerator3.MoveNext())
						{
							ScriptStruct current2 = enumerator3.Current;
							switch (num2)
							{
							case 2:
								num = ((ScriptStruct)(ref current2)).ScriptArg;
								break;
							case 4:
								cost = ((ScriptStruct)(ref current2)).ScriptArg;
								break;
							}
							num2++;
						}
					}
					finally
					{
						((IDisposable)enumerator3/*cast due to constrained. prefix*/).Dispose();
					}
					if (rowId2 == 721068)
					{
						uint itemID = num;
						string? shopName = ((object)((CustomTalk)(ref item)).MainOption/*cast due to constrained. prefix*/).ToString();
						int num3 = 1;
						List<ShopItemCostInfo> list = new List<ShopItemCostInfo>(num3);
						CollectionsMarshal.SetCount(list, num3);
						CollectionsMarshal.AsSpan(list)[0] = new ShopItemCostInfo(cost, 28u);
						AddItemGeneral(itemID, rowId, npcName, shopName, list, valueOrDefault, ItemShopType.SpecialShop);
						break;
					}
					RowRef val = ((CustomTalk)(ref item)).SpecialLinks;
					if (((RowRef)(ref val)).RowId != 0)
					{
						val = ((CustomTalk)(ref item)).SpecialLinks;
						if (LuminaGetter.TryGetSubRowAll<CustomTalkNestHandlers>(((RowRef)(ref val)).RowId, out SubrowCollection<CustomTalkNestHandlers> item2))
						{
							Enumerator<CustomTalkNestHandlers> enumerator4 = item2.GetEnumerator();
							try
							{
								while (enumerator4.MoveNext())
								{
									CustomTalkNestHandlers current3 = enumerator4.Current;
									val = ((CustomTalkNestHandlers)(ref current3)).NestHandler;
									if (TryGetEventHandlerType(((RowRef)(ref val)).RowId, out var type2))
									{
										switch (type2)
										{
										case EventHandlerType.SpecialShop:
											val = ((CustomTalkNestHandlers)(ref current3)).NestHandler;
											AddSpecialItem(LuminaGetter.GetRowOrDefault<SpecialShop>(((RowRef)(ref val)).RowId), npcBase, rowOrDefault, ref itemToItemShopInfos, ref npcIDToLocations);
											break;
										case EventHandlerType.GilShop:
											val = ((CustomTalkNestHandlers)(ref current3)).NestHandler;
											AddGilShopItem(LuminaGetter.GetRowOrDefault<GilShop>(((RowRef)(ref val)).RowId), npcBase, rowOrDefault, ref itemToItemShopInfos, ref npcIDToLocations);
											break;
										}
									}
								}
							}
							finally
							{
								((IDisposable)enumerator4/*cast due to constrained. prefix*/).Dispose();
							}
						}
					}
					enumerator3 = ((CustomTalk)(ref item)).Script.GetEnumerator();
					try
					{
						while (enumerator3.MoveNext())
						{
							ScriptStruct current4 = enumerator3.Current;
							uint scriptArg = ((ScriptStruct)(ref current4)).ScriptArg;
							if (TryGetEventHandlerType(scriptArg, out var type3))
							{
								switch (type3)
								{
								case EventHandlerType.GilShop:
									AddGilShopItem(LuminaGetter.GetRowOrDefault<GilShop>(scriptArg), npcBase, rowOrDefault, ref itemToItemShopInfos, ref npcIDToLocations);
									break;
								case EventHandlerType.FcShop:
									AddFccShop(LuminaGetter.GetRowOrDefault<FccShop>(scriptArg), npcBase, rowOrDefault, ref itemToItemShopInfos, ref npcIDToLocations);
									break;
								case EventHandlerType.SpecialShop:
									AddSpecialItem(LuminaGetter.GetRowOrDefault<SpecialShop>(scriptArg), npcBase, rowOrDefault, ref itemToItemShopInfos, ref npcIDToLocations);
									break;
								}
							}
						}
					}
					finally
					{
						((IDisposable)enumerator3/*cast due to constrained. prefix*/).Dispose();
					}
					break;
				}
				}
			}
		}
		finally
		{
			((IDisposable)enumerator2/*cast due to constrained. prefix*/).Dispose();
		}
	}

	private static bool FixNPCInfo(ENpcBase npcBase, ENpcResident resident, ref Dictionary<uint, ItemSourceInfo> itemToItemShopInfos, ref Dictionary<uint, ShopNPCLocation> npcIDToLocations)
	{
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_0630: Unknown result type (might be due to invalid IL or missing references)
		//IL_0635: Unknown result type (might be due to invalid IL or missing references)
		//IL_0636: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_042e: Unknown result type (might be due to invalid IL or missing references)
		//IL_042f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_050e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0513: Unknown result type (might be due to invalid IL or missing references)
		//IL_0514: Unknown result type (might be due to invalid IL or missing references)
		//IL_0515: Unknown result type (might be due to invalid IL or missing references)
		//IL_051e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0562: Unknown result type (might be due to invalid IL or missing references)
		//IL_0567: Unknown result type (might be due to invalid IL or missing references)
		//IL_0568: Unknown result type (might be due to invalid IL or missing references)
		//IL_0569: Unknown result type (might be due to invalid IL or missing references)
		//IL_0572: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
		switch (((ENpcBase)(ref npcBase)).RowId)
		{
		case 1043463u:
			AddSpecialItem(LuminaGetter.GetRowOrDefault<SpecialShop>(1770601u), npcBase, resident, ref itemToItemShopInfos, ref npcIDToLocations, ItemShopType.SpecialShop, GetNameFromKey("TEXT_CTSMJISPECIALSHOP_00789_Q1_000_000") + "\n" + GetNameFromKey("TEXT_CTSMJISPECIALSHOP_00789_Q2_000_000"));
			AddSpecialItem(LuminaGetter.GetRowOrDefault<SpecialShop>(1770659u), npcBase, resident, ref itemToItemShopInfos, ref npcIDToLocations, ItemShopType.SpecialShop, GetNameFromKey("TEXT_CTSMJISPECIALSHOP_00789_Q1_000_000") + " \n " + GetNameFromKey("TEXT_CTSMJISPECIALSHOP_00789_Q2_000_005"));
			AddSpecialItem(LuminaGetter.GetRowOrDefault<SpecialShop>(1770660u), npcBase, resident, ref itemToItemShopInfos, ref npcIDToLocations, ItemShopType.SpecialShop, GetNameFromKey("TEXT_CTSMJISPECIALSHOP_00789_Q1_000_000") + "\n" + GetNameFromKey("TEXT_CTSMJISPECIALSHOP_00789_Q2_000_010"));
			AddSpecialItem(LuminaGetter.GetRowOrDefault<SpecialShop>(1770602u), npcBase, resident, ref itemToItemShopInfos, ref npcIDToLocations, ItemShopType.SpecialShop, GetNameFromKey("TEXT_CTSMJISPECIALSHOP_00789_Q1_000_005") ?? "");
			AddSpecialItem(LuminaGetter.GetRowOrDefault<SpecialShop>(1770603u), npcBase, resident, ref itemToItemShopInfos, ref npcIDToLocations, ItemShopType.SpecialShop, GetNameFromKey("TEXT_CTSMJISPECIALSHOP_00789_Q1_000_010") ?? "");
			AddSpecialItem(LuminaGetter.GetRowOrDefault<SpecialShop>(1770723u), npcBase, resident, ref itemToItemShopInfos, ref npcIDToLocations, ItemShopType.SpecialShop, GetNameFromKey("TEXT_CTSMJISPECIALSHOP_00789_Q1_000_025") ?? "");
			AddSpecialItem(LuminaGetter.GetRowOrDefault<SpecialShop>(1770734u), npcBase, resident, ref itemToItemShopInfos, ref npcIDToLocations, ItemShopType.SpecialShop, GetNameFromKey("TEXT_CTSMJISPECIALSHOP_00789_Q1_000_030") ?? "");
			return true;
		case 1018655u:
			AddSpecialItem(LuminaGetter.GetRowOrDefault<SpecialShop>(1769743u), npcBase, resident, ref itemToItemShopInfos, ref npcIDToLocations);
			AddSpecialItem(LuminaGetter.GetRowOrDefault<SpecialShop>(1769744u), npcBase, resident, ref itemToItemShopInfos, ref npcIDToLocations);
			AddSpecialItem(LuminaGetter.GetRowOrDefault<SpecialShop>(1770537u), npcBase, resident, ref itemToItemShopInfos, ref npcIDToLocations);
			return true;
		case 1016289u:
			AddSpecialItem(LuminaGetter.GetRowOrDefault<SpecialShop>(1769635u), npcBase, resident, ref itemToItemShopInfos, ref npcIDToLocations);
			return true;
		case 1025047u:
		{
			for (uint num8 = 1769820u; num8 <= 1769834; num8++)
			{
				AddSpecialItem(LuminaGetter.GetRowOrDefault<SpecialShop>(num8), npcBase, resident, ref itemToItemShopInfos, ref npcIDToLocations);
			}
			return true;
		}
		case 1025763u:
			AddGilShopItem(LuminaGetter.GetRowOrDefault<GilShop>(262919u), npcBase, resident, ref itemToItemShopInfos, ref npcIDToLocations);
			return true;
		case 1027123u:
			AddSpecialItem(LuminaGetter.GetRowOrDefault<SpecialShop>(1769934u), npcBase, resident, ref itemToItemShopInfos, ref npcIDToLocations);
			AddSpecialItem(LuminaGetter.GetRowOrDefault<SpecialShop>(1769935u), npcBase, resident, ref itemToItemShopInfos, ref npcIDToLocations);
			return true;
		case 1027124u:
			AddSpecialItem(LuminaGetter.GetRowOrDefault<SpecialShop>(1769937u), npcBase, resident, ref itemToItemShopInfos, ref npcIDToLocations);
			return true;
		case 1033921u:
			AddSpecialItem(LuminaGetter.GetRowOrDefault<SpecialShop>(1770282u), npcBase, resident, ref itemToItemShopInfos, ref npcIDToLocations);
			return true;
		case 1035012u:
		{
			for (ushort num = 0; num <= 10; num++)
			{
				AddQuestReward(LuminaGetter.GetSubRowOrDefault<QuestClassJobReward>(14u, (int)num), npcBase, resident, ref itemToItemShopInfos, ref npcIDToLocations);
				AddQuestReward(LuminaGetter.GetSubRowOrDefault<QuestClassJobReward>(15u, (int)num), npcBase, resident, ref itemToItemShopInfos, ref npcIDToLocations);
				AddQuestReward(LuminaGetter.GetSubRowOrDefault<QuestClassJobReward>(19u, (int)num), npcBase, resident, ref itemToItemShopInfos, ref npcIDToLocations);
			}
			return true;
		}
		case 1016135u:
		{
			for (uint num6 = 3u; num6 <= 10; num6++)
			{
				for (ushort num7 = 0; num7 <= 12; num7++)
				{
					QuestClassJobReward subRowOrDefault8 = LuminaGetter.GetSubRowOrDefault<QuestClassJobReward>(num6, (int)num7);
					AddQuestReward(subRowOrDefault8, npcBase, resident, ref itemToItemShopInfos, ref npcIDToLocations);
					AddQuestRewardCost(subRowOrDefault8, npcBase, ref itemToItemShopInfos, GetCost(num6));
				}
			}
			return true;
		}
		case 1032903u:
		{
			for (ushort num5 = 0; num5 <= 16; num5++)
			{
				QuestClassJobReward subRowOrDefault7 = LuminaGetter.GetSubRowOrDefault<QuestClassJobReward>(12u, (int)num5);
				ENpcBase npcBase8 = npcBase;
				int num3 = 1;
				List<ShopItemCostInfo> list7 = new List<ShopItemCostInfo>(num3);
				CollectionsMarshal.SetCount(list7, num3);
				CollectionsMarshal.AsSpan(list7)[0] = new ShopItemCostInfo(4u, 30273u);
				AddQuestReward(subRowOrDefault7, npcBase8, resident, ref itemToItemShopInfos, ref npcIDToLocations, list7);
			}
			return true;
		}
		case 1032905u:
		{
			for (ushort num2 = 0; num2 <= 16; num2++)
			{
				QuestClassJobReward subRowOrDefault = LuminaGetter.GetSubRowOrDefault<QuestClassJobReward>(13u, (int)num2);
				ENpcBase npcBase2 = npcBase;
				int num3 = 1;
				List<ShopItemCostInfo> list = new List<ShopItemCostInfo>(num3);
				CollectionsMarshal.SetCount(list, num3);
				CollectionsMarshal.AsSpan(list)[0] = new ShopItemCostInfo(4u, 30273u);
				AddQuestReward(subRowOrDefault, npcBase2, resident, ref itemToItemShopInfos, ref npcIDToLocations, list);
			}
			for (ushort num4 = 0; num4 <= 16; num4++)
			{
				QuestClassJobReward subRowOrDefault2 = LuminaGetter.GetSubRowOrDefault<QuestClassJobReward>(17u, (int)num4);
				AddQuestReward(subRowOrDefault2, npcBase, resident, ref itemToItemShopInfos, ref npcIDToLocations);
				ENpcBase npcBase3 = npcBase;
				int num3 = 3;
				List<ShopItemCostInfo> list2 = new List<ShopItemCostInfo>(num3);
				CollectionsMarshal.SetCount(list2, num3);
				Span<ShopItemCostInfo> span = CollectionsMarshal.AsSpan(list2);
				span[0] = new ShopItemCostInfo(20u, 31573u);
				span[1] = new ShopItemCostInfo(20u, 31574u);
				span[2] = new ShopItemCostInfo(20u, 31575u);
				AddQuestRewardCost(subRowOrDefault2, npcBase3, ref itemToItemShopInfos, list2);
				QuestClassJobReward subRowOrDefault3 = LuminaGetter.GetSubRowOrDefault<QuestClassJobReward>(18u, (int)num4);
				AddQuestReward(subRowOrDefault3, npcBase, resident, ref itemToItemShopInfos, ref npcIDToLocations);
				ENpcBase npcBase4 = npcBase;
				num3 = 1;
				List<ShopItemCostInfo> list3 = new List<ShopItemCostInfo>(num3);
				CollectionsMarshal.SetCount(list3, num3);
				CollectionsMarshal.AsSpan(list3)[0] = new ShopItemCostInfo(6u, 31576u);
				AddQuestRewardCost(subRowOrDefault3, npcBase4, ref itemToItemShopInfos, list3);
				QuestClassJobReward subRowOrDefault4 = LuminaGetter.GetSubRowOrDefault<QuestClassJobReward>(20u, (int)num4);
				AddQuestReward(subRowOrDefault4, npcBase, resident, ref itemToItemShopInfos, ref npcIDToLocations);
				ENpcBase npcBase5 = npcBase;
				num3 = 1;
				List<ShopItemCostInfo> list4 = new List<ShopItemCostInfo>(num3);
				CollectionsMarshal.SetCount(list4, num3);
				CollectionsMarshal.AsSpan(list4)[0] = new ShopItemCostInfo(15u, 32956u);
				AddQuestRewardCost(subRowOrDefault4, npcBase5, ref itemToItemShopInfos, list4);
				QuestClassJobReward subRowOrDefault5 = LuminaGetter.GetSubRowOrDefault<QuestClassJobReward>(21u, (int)num4);
				AddQuestReward(subRowOrDefault5, npcBase, resident, ref itemToItemShopInfos, ref npcIDToLocations);
				ENpcBase npcBase6 = npcBase;
				num3 = 1;
				List<ShopItemCostInfo> list5 = new List<ShopItemCostInfo>(num3);
				CollectionsMarshal.SetCount(list5, num3);
				CollectionsMarshal.AsSpan(list5)[0] = new ShopItemCostInfo(15u, 32959u);
				AddQuestRewardCost(subRowOrDefault5, npcBase6, ref itemToItemShopInfos, list5);
				QuestClassJobReward subRowOrDefault6 = LuminaGetter.GetSubRowOrDefault<QuestClassJobReward>(22u, (int)num4);
				AddQuestReward(subRowOrDefault6, npcBase, resident, ref itemToItemShopInfos, ref npcIDToLocations);
				ENpcBase npcBase7 = npcBase;
				num3 = 1;
				List<ShopItemCostInfo> list6 = new List<ShopItemCostInfo>(num3);
				CollectionsMarshal.SetCount(list6, num3);
				CollectionsMarshal.AsSpan(list6)[0] = new ShopItemCostInfo(15u, 33767u);
				AddQuestRewardCost(subRowOrDefault6, npcBase7, ref itemToItemShopInfos, list6);
			}
			return true;
		}
		default:
		{
			if (!SHBFateShopNPC.TryGetValue(((ENpcBase)(ref npcBase)).RowId, out var value))
			{
				return false;
			}
			AddSpecialItem(LuminaGetter.GetRowOrDefault<SpecialShop>(value), npcBase, resident, ref itemToItemShopInfos, ref npcIDToLocations);
			return true;
		}
		}
		static List<ShopItemCostInfo>? GetCost(uint i)
		{
			return i switch
			{
				3u => new List<ShopItemCostInfo>
				{
					new ShopItemCostInfo(1u, 13575u),
					new ShopItemCostInfo(1u, 13576u)
				}, 
				5u => new List<ShopItemCostInfo>
				{
					new ShopItemCostInfo(1u, 13577u),
					new ShopItemCostInfo(1u, 13578u),
					new ShopItemCostInfo(1u, 13579u),
					new ShopItemCostInfo(1u, 13580u)
				}, 
				6u => new List<ShopItemCostInfo>
				{
					new ShopItemCostInfo(5u, 14899u)
				}, 
				7u => new List<ShopItemCostInfo>
				{
					new ShopItemCostInfo(60u, 15840u),
					new ShopItemCostInfo(60u, 15841u)
				}, 
				8u => new List<ShopItemCostInfo>
				{
					new ShopItemCostInfo(50u, 16064u)
				}, 
				9u => new List<ShopItemCostInfo>
				{
					new ShopItemCostInfo(1u, 16932u)
				}, 
				10u => new List<ShopItemCostInfo>
				{
					new ShopItemCostInfo(1u, 16934u)
				}, 
				_ => null, 
			};
		}
		static string GetNameFromKey(string key)
		{
			if (!MJISpecialShopNames.TryGetValue(key, out string value2))
			{
				return string.Empty;
			}
			return value2;
		}
	}

	private static void AddSpecialItem(SpecialShop specialShop, ENpcBase npcBase, ENpcResident resident, ref Dictionary<uint, ItemSourceInfo> itemToItemShopInfos, ref Dictionary<uint, ShopNPCLocation> npcIDToLocations, ItemShopType shopType = ItemShopType.SpecialShop, string? shop = null)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		uint rowId = ((ENpcBase)(ref npcBase)).RowId;
		string npcName = ((object)((ENpcResident)(ref resident)).Singular/*cast due to constrained. prefix*/).ToString();
		ShopNPCLocation valueOrDefault = npcIDToLocations.GetValueOrDefault(rowId);
		BuildContext currentBuildContext = CurrentBuildContext;
		Enumerator<ItemStruct> enumerator = ((SpecialShop)(ref specialShop)).Item.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				ItemStruct current = enumerator.Current;
				List<ShopItemCostInfo> list = new List<ShopItemCostInfo>();
				Enumerator<ItemCostsStruct> enumerator2 = ((ItemStruct)(ref current)).ItemCosts.GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						ItemCostsStruct current2 = enumerator2.Current;
						if (((ItemCostsStruct)(ref current2)).ItemCost.IsValid)
						{
							Item value = ((ItemCostsStruct)(ref current2)).ItemCost.Value;
							if (!(((Item)(ref value)).Name == ReadOnlySeString.op_Implicit(string.Empty)))
							{
								value = ((ItemCostsStruct)(ref current2)).ItemCost.Value;
								Item val = ConvertCurrency(((Item)(ref value)).RowId, ((ItemCostsStruct)(ref current2)).CostType, specialShop);
								list.Add(new ShopItemCostInfo(((ItemCostsStruct)(ref current2)).CurrencyCost, ((Item)(ref val)).RowId));
							}
						}
					}
				}
				finally
				{
					((IDisposable)enumerator2/*cast due to constrained. prefix*/).Dispose();
				}
				Enumerator<ReceiveItemsStruct> enumerator3 = ((ItemStruct)(ref current)).ReceiveItems.GetEnumerator();
				try
				{
					while (enumerator3.MoveNext())
					{
						ReceiveItemsStruct current3 = enumerator3.Current;
						Item value2 = ((ReceiveItemsStruct)(ref current3)).Item.Value;
						string value3;
						string achievementDesc = ((shopType == ItemShopType.Achievement && currentBuildContext.AchievementDescriptions.TryGetValue(((Item)(ref value2)).RowId, out value3)) ? value3 : string.Empty);
						AddItemGeneral(((Item)(ref value2)).RowId, rowId, npcName, shop, list, valueOrDefault, shopType, achievementDesc);
					}
				}
				finally
				{
					((IDisposable)enumerator3/*cast due to constrained. prefix*/).Dispose();
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
	}

	private static void AddGilShopItem(GilShop gilShop, ENpcBase npcBase, ENpcResident resident, ref Dictionary<uint, ItemSourceInfo> itemToItemShopInfos, ref Dictionary<uint, ShopNPCLocation> npcIDToLocations, string shop = null)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		if (!LuminaGetter.TryGetSubRowAll<GilShopItem>(((GilShop)(ref gilShop)).RowId, out SubrowCollection<GilShopItem> item))
		{
			return;
		}
		uint rowId = ((ENpcBase)(ref npcBase)).RowId;
		string npcName = ((object)((ENpcResident)(ref resident)).Singular/*cast due to constrained. prefix*/).ToString();
		ShopNPCLocation valueOrDefault = npcIDToLocations.GetValueOrDefault(rowId);
		string shopName = ((shop != null) ? $"{shop}\n{((GilShop)(ref gilShop)).Name}" : ((object)((GilShop)(ref gilShop)).Name/*cast due to constrained. prefix*/).ToString());
		Enumerator<GilShopItem> enumerator = item.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				GilShopItem current = enumerator.Current;
				Item value = ((GilShopItem)(ref current)).Item.Value;
				uint rowId2 = ((Item)(ref value)).RowId;
				int num = 1;
				List<ShopItemCostInfo> list = new List<ShopItemCostInfo>(num);
				CollectionsMarshal.SetCount(list, num);
				ref ShopItemCostInfo reference = ref CollectionsMarshal.AsSpan(list)[0];
				value = ((GilShopItem)(ref current)).Item.Value;
				reference = new ShopItemCostInfo(((Item)(ref value)).PriceMid, 1u);
				AddItemGeneral(rowId2, rowId, npcName, shopName, list, valueOrDefault, ItemShopType.GilShop);
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
	}

	private static void AddGcShopItem(GCShop gcID, ENpcBase npcBase, ENpcResident resident, ref Dictionary<uint, ItemSourceInfo> itemToItemShopInfos, ref Dictionary<uint, ShopNPCLocation> npcIDToLocations)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		Item val = GrandCompanySeals.Find(delegate(Item i)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			string? text = ((object)((Item)(ref i)).Description/*cast due to constrained. prefix*/).ToString();
			GrandCompany value3 = ((GCShop)(ref gcID)).GrandCompany.Value;
			return text.Contains(((object)((GrandCompany)(ref value3)).Name/*cast due to constrained. prefix*/).ToString() ?? "");
		});
		if (((Item)(ref val)).RowId == 0 || !CurrentBuildContext.GcCategories.TryGetValue(((GCShop)(ref gcID)).GrandCompany.RowId, out List<GCScripShopCategory> value))
		{
			return;
		}
		uint rowId = ((ENpcBase)(ref npcBase)).RowId;
		string npcName = ((object)((ENpcResident)(ref resident)).Singular/*cast due to constrained. prefix*/).ToString();
		ShopNPCLocation valueOrDefault = npcIDToLocations.GetValueOrDefault(rowId);
		foreach (GCScripShopCategory item2 in value)
		{
			GCScripShopCategory current = item2;
			if (!LuminaGetter.TryGetSubRowAll<GCScripShopItem>(((GCScripShopCategory)(ref current)).RowId, out SubrowCollection<GCScripShopItem> item))
			{
				continue;
			}
			Enumerator<GCScripShopItem> enumerator2 = item.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					GCScripShopItem current2 = enumerator2.Current;
					if (((GCScripShopItem)(ref current2)).SortKey == 0)
					{
						break;
					}
					Item value2 = ((GCScripShopItem)(ref current2)).Item.Value;
					uint rowId2 = ((Item)(ref value2)).RowId;
					int num = 1;
					List<ShopItemCostInfo> list = new List<ShopItemCostInfo>(num);
					CollectionsMarshal.SetCount(list, num);
					CollectionsMarshal.AsSpan(list)[0] = new ShopItemCostInfo(((GCScripShopItem)(ref current2)).CostGCSeals, ((Item)(ref val)).RowId);
					AddItemGeneral(rowId2, rowId, npcName, null, list, valueOrDefault, ItemShopType.GcShop);
				}
			}
			finally
			{
				((IDisposable)enumerator2/*cast due to constrained. prefix*/).Dispose();
			}
		}
	}

	private static void AddInclusionShop(InclusionShop inclusionShop, ENpcBase npcBase, ENpcResident resident, ref Dictionary<uint, ItemSourceInfo> itemToItemShopInfos, ref Dictionary<uint, ShopNPCLocation> npcIDToLocations)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		string value = ((object)((InclusionShop)(ref inclusionShop)).ShopName/*cast due to constrained. prefix*/).ToString();
		Enumerator<RowRef<InclusionShopCategory>> enumerator = ((InclusionShop)(ref inclusionShop)).Category.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				RowRef<InclusionShopCategory> current = enumerator.Current;
				InclusionShopCategory value2 = current.Value;
				if (((InclusionShopCategory)(ref value2)).RowId == 0)
				{
					continue;
				}
				value2 = current.Value;
				if (!LuminaGetter.TryGetSubRowAll<InclusionShopSeries>(((InclusionShopCategory)(ref value2)).InclusionShopSeries.RowId, out SubrowCollection<InclusionShopSeries> item))
				{
					continue;
				}
				Enumerator<InclusionShopSeries> enumerator2 = item.GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						InclusionShopSeries current2 = enumerator2.Current;
						SpecialShop value3 = ((InclusionShopSeries)(ref current2)).SpecialShop.Value;
						string text;
						if (!string.IsNullOrEmpty(value))
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(2, 3);
							defaultInterpolatedStringHandler.AppendFormatted(value);
							defaultInterpolatedStringHandler.AppendLiteral("\n");
							value2 = current.Value;
							defaultInterpolatedStringHandler.AppendFormatted<ReadOnlySeString>(((InclusionShopCategory)(ref value2)).Name);
							defaultInterpolatedStringHandler.AppendLiteral("\n");
							defaultInterpolatedStringHandler.AppendFormatted<ReadOnlySeString>(((SpecialShop)(ref value3)).Name);
							text = defaultInterpolatedStringHandler.ToStringAndClear();
						}
						else
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(1, 2);
							value2 = current.Value;
							defaultInterpolatedStringHandler2.AppendFormatted<ReadOnlySeString>(((InclusionShopCategory)(ref value2)).Name);
							defaultInterpolatedStringHandler2.AppendLiteral("\n");
							defaultInterpolatedStringHandler2.AppendFormatted<ReadOnlySeString>(((SpecialShop)(ref value3)).Name);
							text = defaultInterpolatedStringHandler2.ToStringAndClear();
						}
						string shop = text;
						AddSpecialItem(value3, npcBase, resident, ref itemToItemShopInfos, ref npcIDToLocations, ItemShopType.SpecialShop, shop);
					}
				}
				finally
				{
					((IDisposable)enumerator2/*cast due to constrained. prefix*/).Dispose();
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
	}

	private static void AddFccShop(FccShop shop, ENpcBase npcBase, ENpcResident resident, ref Dictionary<uint, ItemSourceInfo> itemToItemShopInfos, ref Dictionary<uint, ShopNPCLocation> npcIDToLocations)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		Enumerator<ItemDataStruct> enumerator = ((FccShop)(ref shop)).ItemData.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				ItemDataStruct current = enumerator.Current;
				if (LuminaGetter.TryGetRow<Item>(((ItemDataStruct)(ref current)).Item.RowId, out Item item) && !(((Item)(ref item)).Name == ReadOnlySeString.op_Implicit(string.Empty)))
				{
					uint cost = ((ItemDataStruct)(ref current)).Cost;
					uint rowId = ((Item)(ref item)).RowId;
					uint rowId2 = ((ENpcBase)(ref npcBase)).RowId;
					string? npcName = ((object)((ENpcResident)(ref resident)).Singular/*cast due to constrained. prefix*/).ToString();
					int num = 1;
					List<ShopItemCostInfo> list = new List<ShopItemCostInfo>(num);
					CollectionsMarshal.SetCount(list, num);
					CollectionsMarshal.AsSpan(list)[0] = new ShopItemCostInfo(cost, 102233u);
					AddItemGeneral(rowId, rowId2, npcName, null, list, npcIDToLocations.GetValueOrDefault(((ENpcBase)(ref npcBase)).RowId), ItemShopType.FcShop);
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
	}

	private static void AddItemsInPrehandler(PreHandler preHandler, ENpcBase npcBase, ENpcResident resident, ref Dictionary<uint, ItemSourceInfo> itemToItemShopInfos, ref Dictionary<uint, ShopNPCLocation> npcIDToLocations)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		RowRef target = ((PreHandler)(ref preHandler)).Target;
		uint rowId = ((RowRef)(ref target)).RowId;
		if (rowId != 0 && TryGetEventHandlerType(rowId, out var type))
		{
			switch (type)
			{
			case EventHandlerType.GilShop:
				AddGilShopItem(LuminaGetter.GetRowOrDefault<GilShop>(rowId), npcBase, resident, ref itemToItemShopInfos, ref npcIDToLocations);
				break;
			case EventHandlerType.SpecialShop:
				AddSpecialItem(LuminaGetter.GetRowOrDefault<SpecialShop>(rowId), npcBase, resident, ref itemToItemShopInfos, ref npcIDToLocations);
				break;
			case EventHandlerType.InclusionShop:
				AddInclusionShop(LuminaGetter.GetRowOrDefault<InclusionShop>(rowId), npcBase, resident, ref itemToItemShopInfos, ref npcIDToLocations);
				break;
			}
		}
	}

	private static void AddItemsInTopicSelect(TopicSelect topicSelect, ENpcBase npcBase, ENpcResident resident, ref Dictionary<uint, ItemSourceInfo> itemToItemShopInfos, ref Dictionary<uint, ShopNPCLocation> npcIDToLocations)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		string text = ((object)((TopicSelect)(ref topicSelect)).Name/*cast due to constrained. prefix*/).ToString();
		Enumerator<RowRef> enumerator = ((TopicSelect)(ref topicSelect)).Shop.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				RowRef current = enumerator.Current;
				uint rowId = ((RowRef)(ref current)).RowId;
				if (rowId != 0 && TryGetEventHandlerType(rowId, out var type))
				{
					switch (type)
					{
					case EventHandlerType.SpecialShop:
					{
						SpecialShop rowOrDefault = LuminaGetter.GetRowOrDefault<SpecialShop>(rowId);
						AddSpecialItem(rowOrDefault, npcBase, resident, ref itemToItemShopInfos, ref npcIDToLocations, ItemShopType.SpecialShop, text + "\n" + ((object)((SpecialShop)(ref rowOrDefault)).Name/*cast due to constrained. prefix*/).ToString());
						break;
					}
					case EventHandlerType.GilShop:
						AddGilShopItem(LuminaGetter.GetRowOrDefault<GilShop>(rowId), npcBase, resident, ref itemToItemShopInfos, ref npcIDToLocations, text);
						break;
					case EventHandlerType.PreHandler:
						AddItemsInPrehandler(LuminaGetter.GetRowOrDefault<PreHandler>(rowId), npcBase, resident, ref itemToItemShopInfos, ref npcIDToLocations);
						break;
					}
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
	}

	private static void AddCollectablesShop(CollectablesShop shop, ENpcBase npcBase, ENpcResident resident, ref Dictionary<uint, ItemSourceInfo> itemToItemShopInfos, ref Dictionary<uint, ShopNPCLocation> npcIDToLocations)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		string text = ((object)((CollectablesShop)(ref shop)).Name/*cast due to constrained. prefix*/).ToString();
		if (text == string.Empty)
		{
			return;
		}
		uint rowId = ((ENpcBase)(ref npcBase)).RowId;
		string npcName = ((object)((ENpcResident)(ref resident)).Singular/*cast due to constrained. prefix*/).ToString();
		ShopNPCLocation valueOrDefault = npcIDToLocations.GetValueOrDefault(rowId);
		Enumerator<SubrowRef<CollectablesShopItem>> enumerator = ((CollectablesShop)(ref shop)).ShopItems.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				uint rowId2 = enumerator.Current.Value.RowId;
				if (rowId2 == 0 || !LuminaGetter.TryGetSubRowAll<CollectablesShopItem>(rowId2, out SubrowCollection<CollectablesShopItem> item))
				{
					continue;
				}
				Enumerator<CollectablesShopItem> enumerator2 = item.GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						CollectablesShopItem current = enumerator2.Current;
						if (((CollectablesShopItem)(ref current)).Item.RowId > 1000)
						{
							CollectablesShopRewardItem rowOrDefault = LuminaGetter.GetRowOrDefault<CollectablesShopRewardItem>(((CollectablesShopItem)(ref current)).CollectablesShopRewardScrip.RowId);
							CollectablesShopRefine rowOrDefault2 = LuminaGetter.GetRowOrDefault<CollectablesShopRefine>(((CollectablesShopItem)(ref current)).CollectablesShopRefine.RowId);
							Item value = ((CollectablesShopRewardItem)(ref rowOrDefault)).Item.Value;
							uint rowId3 = ((Item)(ref value)).RowId;
							CollectablesShopItemGroup value2 = ((CollectablesShopItem)(ref current)).CollectablesShopItemGroup.Value;
							string shopName = text + "\n" + ((object)((CollectablesShopItemGroup)(ref value2)).Name/*cast due to constrained. prefix*/).ToString();
							int num = 3;
							List<ShopItemCostInfo> list = new List<ShopItemCostInfo>(num);
							CollectionsMarshal.SetCount(list, num);
							Span<ShopItemCostInfo> span = CollectionsMarshal.AsSpan(list);
							span[0] = new ShopItemCostInfo(((CollectablesShopRewardItem)(ref rowOrDefault)).RewardLow, ((CollectablesShopItem)(ref current)).Item.RowId, ((CollectablesShopRefine)(ref rowOrDefault2)).LowCollectability);
							span[1] = new ShopItemCostInfo(((CollectablesShopRewardItem)(ref rowOrDefault)).RewardMid, ((CollectablesShopItem)(ref current)).Item.RowId, ((CollectablesShopRefine)(ref rowOrDefault2)).MidCollectability);
							span[2] = new ShopItemCostInfo(((CollectablesShopRewardItem)(ref rowOrDefault)).RewardHigh, ((CollectablesShopItem)(ref current)).Item.RowId, ((CollectablesShopRefine)(ref rowOrDefault2)).HighCollectability);
							AddItemGeneral(rowId3, rowId, npcName, shopName, list, valueOrDefault, ItemShopType.CollectableExchange);
						}
					}
				}
				finally
				{
					((IDisposable)enumerator2/*cast due to constrained. prefix*/).Dispose();
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
	}

	private static void AddQuestReward(QuestClassJobReward questReward, ENpcBase npcBase, ENpcResident resident, ref Dictionary<uint, ItemSourceInfo> itemToItemShopInfos, ref Dictionary<uint, ShopNPCLocation> npcIDToLocations, List<ShopItemCostInfo> cost = null)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		if (((QuestClassJobReward)(ref questReward)).ClassJobCategory.RowId == 0)
		{
			return;
		}
		uint rowId = ((ENpcBase)(ref npcBase)).RowId;
		string npcName = ((object)((ENpcResident)(ref resident)).Singular/*cast due to constrained. prefix*/).ToString();
		ShopNPCLocation valueOrDefault = npcIDToLocations.GetValueOrDefault(rowId);
		if (cost == null)
		{
			cost = new List<ShopItemCostInfo>();
			for (int i = 0; i < ((QuestClassJobReward)(ref questReward)).RequiredItem.Count; i++)
			{
				RowRef<Item> val = ((QuestClassJobReward)(ref questReward)).RequiredItem[i];
				if (val.RowId == 0)
				{
					break;
				}
				List<ShopItemCostInfo> list = cost;
				byte cost2 = ((QuestClassJobReward)(ref questReward)).RequiredAmount[i];
				Item value = val.Value;
				list.Add(new ShopItemCostInfo(cost2, ((Item)(ref value)).RowId));
			}
		}
		Enumerator<RowRef<Item>> enumerator = ((QuestClassJobReward)(ref questReward)).RewardItem.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				RowRef<Item> current = enumerator.Current;
				if (current.RowId == 0)
				{
					break;
				}
				AddItemGeneral(current.RowId, rowId, npcName, string.Empty, cost, valueOrDefault, ItemShopType.QuestReward);
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
	}

	private static void AddQuestRewardCost(QuestClassJobReward questReward, ENpcBase npcBase, ref Dictionary<uint, ItemSourceInfo> itemToItemShopInfos, List<ShopItemCostInfo> cost)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (cost == null || ((QuestClassJobReward)(ref questReward)).ClassJobCategory.RowId == 0)
		{
			return;
		}
		Enumerator<RowRef<Item>> enumerator = ((QuestClassJobReward)(ref questReward)).RewardItem.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				RowRef<Item> current = enumerator.Current;
				if (current.RowId == 0)
				{
					break;
				}
				AddItemCost(current.RowId, ((ENpcBase)(ref npcBase)).RowId, ref itemToItemShopInfos, cost);
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
	}

	private static void AddAchievementItem(ref Dictionary<uint, ItemSourceInfo> itemToItemShopInfos, ref Dictionary<uint, ShopNPCLocation> npcIDToLocations)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		for (uint num = 1006004u; num <= 1006006; num++)
		{
			ENpcBase rowOrDefault = LuminaGetter.GetRowOrDefault<ENpcBase>(num);
			ENpcResident rowOrDefault2 = LuminaGetter.GetRowOrDefault<ENpcResident>(num);
			for (uint num2 = 1769898u; num2 <= 1769906; num2++)
			{
				AddSpecialItem(LuminaGetter.GetRowOrDefault<SpecialShop>(num2), rowOrDefault, rowOrDefault2, ref itemToItemShopInfos, ref npcIDToLocations, ItemShopType.Achievement);
			}
		}
	}

	private static void AddItemCost(uint itemID, uint npcID, ref Dictionary<uint, ItemSourceInfo> itemToItemShopInfos, List<ShopItemCostInfo> cost)
	{
		if (itemID != 0)
		{
			CurrentItemAccumulator.AddCost(itemID, npcID, cost);
		}
	}

	private static void AddItemGeneral(uint itemID, uint npcID, string npcName, string? shopName, List<ShopItemCostInfo> cost, ShopNPCLocation? npcLocation, ItemShopType shopType, string achievementDesc = "")
	{
		if (itemID != 0)
		{
			if (CurrentBuildContext.ShouldHideDefaultShopName && shopName == "アイテムの購入")
			{
				shopName = string.Empty;
			}
			CurrentItemAccumulator.AddItem(itemID, npcID, npcName, shopName, cost, npcLocation, shopType, achievementDesc);
		}
	}

	private static void ParseLgbFile(LgbFile lgbFile, TerritoryType sTerritoryType, ref Dictionary<uint, ShopNPCLocation> npcIDToLocations)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Invalid comparison between Unknown and I4
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		Layer[] layers = lgbFile.Layers;
		for (int i = 0; i < layers.Length; i++)
		{
			InstanceObject[] instanceObjects = layers[i].InstanceObjects;
			foreach (InstanceObject val in instanceObjects)
			{
				if ((int)val.AssetType != 8)
				{
					continue;
				}
				uint baseId = ((ENPCInstanceObject)(object)val.Object).ParentData.ParentData.BaseId;
				if (baseId != 0 && !npcIDToLocations.ContainsKey(baseId) && LuminaGetter.TryGetRow<ENpcBase>(baseId, out ENpcBase item) && LuminaGetter.TryGetRow<ENpcResident>(baseId, out ENpcResident item2) && HasRelevantEventHandler(item))
				{
					byte map = ((ENpcResident)(ref item2)).Map;
					float x = val.Transform.Translation.X;
					float z = val.Transform.Translation.Z;
					if (CurrentBuildContext.TryGetMapRowID(((TerritoryType)(ref sTerritoryType)).RowId, map, out var mapRowID))
					{
						npcIDToLocations.Add(baseId, new ShopNPCLocation(x, z, ((TerritoryType)(ref sTerritoryType)).RowId, mapRowID));
					}
					else
					{
						npcIDToLocations.Add(baseId, new ShopNPCLocation(x, z, ((TerritoryType)(ref sTerritoryType)).RowId));
					}
				}
			}
		}
	}

	private static bool HasRelevantEventHandler(ENpcBase npcBase)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		Enumerator<RowRef> enumerator = ((ENpcBase)(ref npcBase)).ENpcData.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				RowRef current = enumerator.Current;
				if (((RowRef)(ref current)).RowId == 0)
				{
					break;
				}
				if (TryGetEventHandlerType(((RowRef)(ref current)).RowId, out var _))
				{
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

	private static bool TryGetEventHandlerType(uint data, out EventHandlerType type)
	{
		type = (EventHandlerType)(data >> 16);
		switch (type)
		{
		case EventHandlerType.GilShop:
		case EventHandlerType.CustomTalk:
		case EventHandlerType.GcShop:
		case EventHandlerType.SpecialShop:
		case EventHandlerType.FcShop:
		case EventHandlerType.TopicSelect:
		case EventHandlerType.PreHandler:
		case EventHandlerType.InclusionShop:
		case EventHandlerType.CollectablesShop:
			return true;
		default:
			return false;
		}
	}

	private static Item ConvertCurrency(uint itemID, byte costType, SpecialShop specialShop)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		if (costType == 2 && CurrentBuildContext.TomestoneItemIDs.TryGetValue(itemID, out var value))
		{
			return LuminaGetter.GetRowOrDefault<Item>(value);
		}
		if ((itemID >= 8 || itemID == 0) ? true : false)
		{
			return LuminaGetter.GetRowOrDefault<Item>(itemID);
		}
		return (Item)(((SpecialShop)(ref specialShop)).UseCurrencyType switch
		{
			16 => LuminaGetter.GetRowOrDefault<Item>(Currencies[itemID]), 
			8 => LuminaGetter.GetRowOrDefault<Item>(1u), 
			4 => CurrentBuildContext.TomestoneItemIDs.TryGetValue(itemID, out value) ? LuminaGetter.GetRowOrDefault<Item>(value) : LuminaGetter.GetRowOrDefault<Item>(itemID), 
			_ => LuminaGetter.GetRowOrDefault<Item>(itemID), 
		});
	}
}
