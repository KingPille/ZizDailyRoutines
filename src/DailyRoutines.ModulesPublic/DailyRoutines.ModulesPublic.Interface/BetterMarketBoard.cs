using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Numerics;
using DailyRoutines.Common.Interface.Widgets;
using DailyRoutines.Common.Interface.Windows;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Common.RemoteInteraction.Enums;
using DailyRoutines.Common.RemoteInteraction.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Internal;
using DailyRoutines.Manager;
using DailyRoutines.RemoteInteraction.Universalis;
using DailyRoutines.RemoteInteraction.Universalis.Models.Requests;
using DailyRoutines.RemoteInteraction.Universalis.Models.Responses;
using Dalamud.Bindings.ImGui;
using Dalamud.Bindings.ImPlot;
using Dalamud.Game;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Command;
using Dalamud.Game.Text.Evaluator;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Interface;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using Dalamud.Utility.Numerics;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Component.GUI;
using InteropGenerator.Runtime;
using Lumina.Excel.Sheets;
using Lumina.Text;
using Lumina.Text.ReadOnly;
using OmenTools.Dalamud.Abstractions;
using OmenTools.Dalamud.Attributes;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Info.Game.Data;
using OmenTools.Info.Game.ItemSource;
using OmenTools.Info.Game.ItemSource.Enums;
using OmenTools.Info.Game.ItemSource.Models;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;
using OmenTools.Threading.TaskHelper;
using TimeAgo;

namespace DailyRoutines.ModulesPublic.Interface;

public class BetterMarketBoard : ModuleBase
{
	private class SearchInMarketMenu(BetterMarketBoard module) : ContextMenuEntry
	{
		public override string Identifier => "BetterMarketBoard";

		public override ContextMenuItem? Create(ContextMenuOpenedArgs args)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			if (args.TargetItemID != 0 && args.TargetItemRow.IsValid)
			{
				Item value = args.TargetItemRow.Value;
				if (((Item)(ref value)).ItemSearchCategory.RowId != 0)
				{
					return new ContextMenuItem
					{
						Name = ReadOnlySeString.op_Implicit(LanguageManager.Get("BetterMarketBoard-SearchInMarket")),
						OnClicked = delegate
						{
							module.ToggleOverlay(true);
							module.provider.SelectItem(args.TargetItemID);
						}
					};
				}
			}
			return null;
		}
	}

	private readonly record struct SearchCategoryGroup(ItemSearchCategory Category, List<Item> Items);

	private sealed class CachedValue<T>
	{
		public int Version = -1;

		public T? Value;
	}

	private sealed class HistoryDataSet
	{
		public List<HistoryEntry> Entries = new List<HistoryEntry>();

		public int TotalCount;

		public uint TotalQty;

		public ulong AvgPrice;

		public int HQCount;

		public int HQPercent;

		public bool IsCanBeHQ;

		public bool IsAnyHQ;

		public ulong AvgNQPrice;

		public ulong AvgHQPrice;
	}

	private sealed class TrendDataSet
	{
		public List<HistoryEntry> Entries = new List<HistoryEntry>();

		public double XMin;

		public double XMax;

		public double VisualMin;

		public double VisualMax;

		public ulong AvgHistoryPrice;

		public bool IsCanBeHQ;

		public int HQCount;

		public int NQCount;

		public ulong AvgNQPrice;

		public ulong AvgHQPrice;
	}

	private sealed class ListingsDataSet
	{
		public List<UniversalisMarketListing> Listings = new List<UniversalisMarketListing>();

		public UniversalisMarketItemData? Source;

		public bool IsAnyHQ;

		public bool IsAnyOnMannequin;

		public int TotalCount;

		public uint TotalQty;
	}

	private sealed class LocalListingsDataSet
	{
		public List<MarketBoardListing> Listings = new List<MarketBoardListing>();

		public bool IsAnyHQ;

		public bool IsAnyOnMannequin;

		public bool IsAnyMateria;

		public int TotalCount;

		public uint TotalQty;
	}

	private sealed class WorldPriceRanks(List<RankedWorldPriceRow> valid, List<RankedWorldPriceRow> cheapest, List<RankedWorldPriceRow> expensive, WorldPriceRow current)
	{
		public List<RankedWorldPriceRow> Valid = valid;

		public List<RankedWorldPriceRow> Cheapest = cheapest;

		public List<RankedWorldPriceRow> Expensive = expensive;

		public WorldPriceRow Current = current;
	}

	private sealed class MarketDataProvider(BetterMarketBoard owner)
	{
		public bool HQOnly;

		private int onlineDataVersion;

		private int onlineHistoryVersion;

		private int itemEpoch;

		private readonly Dictionary<(uint ItemID, uint WorldID, bool HQOnly), UniversalisMarketDataResponse> onlineDataCache = new Dictionary<(uint, uint, bool), UniversalisMarketDataResponse>();

		private readonly Dictionary<(uint ItemID, uint WorldID, bool HQOnly), IDisposable> subscriptionCache = new Dictionary<(uint, uint, bool), IDisposable>();

		private readonly Dictionary<(uint ItemID, uint WorldID), UniversalisAggregatedMarketDataResponse> onlineAggregatedCache = new Dictionary<(uint, uint), UniversalisAggregatedMarketDataResponse>();

		private readonly Dictionary<(uint ItemID, uint WorldID), IDisposable> aggregatedSubscriptionCache = new Dictionary<(uint, uint), IDisposable>();

		private readonly Dictionary<(uint ItemID, uint WorldID), UniversalisMarketHistoryResponse> onlineHistoryCache = new Dictionary<(uint, uint), UniversalisMarketHistoryResponse>();

		private readonly Dictionary<(uint ItemID, uint WorldID), IDisposable> historySubscriptionCache = new Dictionary<(uint, uint), IDisposable>();

		private readonly Dictionary<(uint ItemID, string Scope), IDisposable> tooltipAggregatedSubscriptionCache = new Dictionary<(uint, string), IDisposable>();

		private readonly Dictionary<string, List<WorldPriceRow>> cachedDCWorldPrices = new Dictionary<string, List<WorldPriceRow>>();

		private WorldPriceRanks? worldPriceRanks;

		private string? priceTableRegion;

		private bool priceTableHQOnly;

		private bool worldPriceTableDirty = true;

		private readonly Dictionary<(uint ItemID, uint WorldID, bool HQOnly), CachedValue<HistoryDataSet>> historyDataCache = new Dictionary<(uint, uint, bool), CachedValue<HistoryDataSet>>();

		private readonly Dictionary<(uint ItemID, uint WorldID, bool HQOnly), CachedValue<TrendDataSet>> trendDataCache = new Dictionary<(uint, uint, bool), CachedValue<TrendDataSet>>();

		private readonly Dictionary<(uint ItemID, uint WorldID, bool HQOnly), CachedValue<ListingsDataSet>> listingsDataCache = new Dictionary<(uint, uint, bool), CachedValue<ListingsDataSet>>();

		private (int ItemEpoch, uint ItemID, bool HQOnly, uint ListingCount, int ContentHash) localListingsFingerprint;

		private LocalListingsDataSet? localListingsData;

		private bool localListingsStale;

		private long localSearchRetryDeadline;

		private (uint SearchItemId, uint EntryCount, uint ListingCount) localListingsBaseline;

		private readonly Dictionary<uint, ItemSourceInfo?> itemSourceCache = new Dictionary<uint, ItemSourceInfo>();

		private readonly Dictionary<uint, uint?> npcGilPriceCache = new Dictionary<uint, uint?>();

		private string? searchInputCache;

		private List<Item>? searchResultRefCache;

		private List<SearchCategoryGroup>? searchGroupsCache;

		private readonly Dictionary<ulong, MarketBoardListing> selectedListings = new Dictionary<ulong, MarketBoardListing>();

		private static readonly LocalListingsDataSet EmptyLocalListingsData = new LocalListingsDataSet();

		public uint SelectedWorldID { get; private set; }

		public string EffectiveRegionName { get; private set; } = string.Empty;

		public uint SelectedItemID { get; private set; }

		public long LastSelectTime { get; private set; }

		public bool IsViewingCurrentWorld => SelectedWorldID == GameState.CurrentWorld;

		public WorldPriceRow MinPriceData { get; private set; }

		public WorldPriceRow MaxPriceData { get; private set; }

		public IReadOnlyDictionary<string, List<WorldPriceRow>> DCWorldPrices => cachedDCWorldPrices;

		public IReadOnlyDictionary<ulong, MarketBoardListing> SelectedListings => selectedListings;

		public void AddListing(ulong listingID, MarketBoardListing listing)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			selectedListings[listingID] = listing;
		}

		public void ToggleListing(ulong listingID, MarketBoardListing listing)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			if (!selectedListings.Remove(listingID))
			{
				selectedListings[listingID] = listing;
			}
		}

		public void RemoveListing(ulong listingID)
		{
			selectedListings.Remove(listingID);
		}

		public void ClearSelectedListings()
		{
			selectedListings.Clear();
		}

		public unsafe void SelectItem(uint itemID, string? regionName = null, bool? hqOnly = null)
		{
			InfoProxyItemSearch* infoProxy = InfoProxy;
			if (infoProxy == null)
			{
				return;
			}
			bool flag = hqOnly ?? HQOnly;
			if (flag && (!LuminaGetter.TryGetRow<Item>(itemID, out Item item) || !((Item)(ref item)).CanBeHq))
			{
				flag = false;
			}
			bool num = SelectedItemID != itemID || HQOnly != flag;
			if (SelectedItemID != itemID)
			{
				itemEpoch++;
			}
			SelectedItemID = itemID;
			HQOnly = flag;
			LastSelectTime = Environment.TickCount64;
			AddToItemSearchHistory(itemID);
			if (num)
			{
				selectedListings.Clear();
				ClearAllData();
			}
			((InfoProxyItemSearch)infoProxy).SearchItemId = itemID;
			RequestAllWorldsData(itemID, regionName, flag);
			if (IsViewingCurrentWorld)
			{
				if (IsAbleToSearchMarket())
				{
					RequestLocalSearchData(itemID);
				}
				else
				{
					((InfoProxyItemSearch)infoProxy).ClearListData();
				}
			}
		}

		public unsafe bool FollowLocalSearch(uint itemID)
		{
			InfoProxyItemSearch* infoProxy = InfoProxy;
			if (infoProxy == null || itemID == 0)
			{
				return false;
			}
			if (SelectedItemID != itemID)
			{
				itemEpoch++;
				HQOnly = false;
				selectedListings.Clear();
			}
			SelectedItemID = itemID;
			LastSelectTime = Environment.TickCount64;
			((InfoProxyItemSearch)infoProxy).SearchItemId = itemID;
			if (!IsViewingCurrentWorld)
			{
				return false;
			}
			return RequestLocalSearchData(itemID);
		}

		public void SelectWorld(uint worldID)
		{
			if (SelectedItemID != 0)
			{
				if (SelectedWorldID != worldID)
				{
					selectedListings.Clear();
					DisposeSelectedWorldData(SelectedItemID, SelectedWorldID);
					ClearDerivedCaches();
					localListingsData = null;
					localListingsFingerprint = default((int, uint, bool, uint, int));
					onlineDataVersion++;
					onlineHistoryVersion++;
				}
				SelectedWorldID = worldID;
				SelectItem(SelectedItemID, null, HQOnly);
			}
		}

		public void SelectRegion(string regionName)
		{
			if (!string.Equals(EffectiveRegionName, regionName, StringComparison.Ordinal))
			{
				selectedListings.Clear();
				ClearAllData();
			}
			EffectiveRegionName = regionName;
			if (owner.allWorlds.TryGetValue(regionName, out Dictionary<string, Dictionary<uint, string>> value) && !value.Values.Any((Dictionary<uint, string> dc) => dc.ContainsKey(SelectedWorldID)))
			{
				SelectedWorldID = (value.Values.Any((Dictionary<uint, string> dc) => dc.ContainsKey(GameState.CurrentWorld)) ? GameState.CurrentWorld : value.Values.SelectMany((Dictionary<uint, string> dc) => dc.Keys).FirstOrDefault());
			}
			if (SelectedItemID != 0)
			{
				SelectItem(SelectedItemID, regionName, HQOnly);
			}
		}

		public void ToggleHQ()
		{
			if (SelectedItemID != 0)
			{
				SelectItem(SelectedItemID, null, !HQOnly);
			}
		}

		public void Reload()
		{
			if (SelectedItemID != 0)
			{
				ClearAllData();
				SelectItem(SelectedItemID, null, HQOnly);
			}
		}

		public void AnchorWorld()
		{
			SelectedWorldID = GameState.CurrentWorld;
		}

		public void EnsureAnchored()
		{
			if (SelectedWorldID == 0)
			{
				AnchorWorld();
			}
			AnchorRegion();
		}

		public unsafe void ResyncAfterWorldChange()
		{
			SelectedWorldID = GameState.CurrentWorld;
			EffectiveRegionName = owner.GetCurrentRegionName();
			InfoProxyItemSearch* infoProxy = InfoProxy;
			if (infoProxy != null)
			{
				((InfoProxyItemSearch)infoProxy).ClearListData();
			}
			selectedListings.Clear();
			ClearAllData();
			if (SelectedItemID != 0)
			{
				SelectItem(SelectedItemID, null, HQOnly);
			}
			localListingsStale = true;
			localSearchRetryDeadline = Environment.TickCount64 + 60000;
			localListingsBaseline = ((infoProxy == null) ? default((uint, uint, uint)) : (SearchItemId: ((InfoProxyItemSearch)infoProxy).SearchItemId, EntryCount: ((InfoProxyItemSearch)infoProxy).EntryCount, ListingCount: ((InfoProxyItemSearch)infoProxy).ListingCount));
		}

		public void RetryLocalSearchIfStale()
		{
			if (localListingsStale && Environment.TickCount64 <= localSearchRetryDeadline && SelectedItemID != 0 && IsViewingCurrentWorld && IsAbleToSearchMarket() && !OmenServiceBase<GameState>.Instance().IsMarketListingsStuck)
			{
				RequestLocalSearchData(SelectedItemID);
			}
		}

		public void AnchorRegion()
		{
			if (string.IsNullOrEmpty(EffectiveRegionName))
			{
				EffectiveRegionName = owner.GetCurrentRegionName();
			}
		}

		public void EnsurePriceData(uint itemID, bool hqOnly)
		{
			if (itemID != 0 && owner.allWorlds.Count != 0)
			{
				EnsureAnchored();
				RequestAllWorldsData(itemID, null, hqOnly);
			}
		}

		public unsafe static bool RequestLocalSearchData(uint itemID)
		{
			if (itemID == 0 || InfoProxy == null)
			{
				return false;
			}
			((InfoProxyItemSearch)InfoProxy).EndRequest();
			((InfoProxyItemSearch)InfoProxy).SearchItemId = itemID;
			return ((InfoProxyItemSearch)InfoProxy).RequestData();
		}

		public unsafe static bool SendBuyRequest(MarketBoardListing item)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			if (IsOwnRetainer(item.RetainerId))
			{
				((RaptureLogModule)RaptureLogModule.Instance()).ShowLogMessage(468u);
				return false;
			}
			((InfoProxyItemSearch)InfoProxy).SetLastPurchasedItem(&item);
			return ((InfoProxyItemSearch)InfoProxy).SendPurchaseRequestPacket();
		}

		private void AddToItemSearchHistory(uint itemID)
		{
			if (LuminaGetter.TryGetRow<Item>(itemID, out Item _))
			{
				owner.config.HistoryItems[itemID] = new MarketHistoryItem
				{
					ItemID = itemID,
					AccessTime = OmenServiceBase<StandardTimeManager>.Instance().Now
				};
				owner.config.HistoryItems = owner.config.HistoryItems.OrderByDescending<KeyValuePair<uint, MarketHistoryItem>, DateTime>((KeyValuePair<uint, MarketHistoryItem> x) => x.Value.AccessTime).Take(50).ToDictionary((KeyValuePair<uint, MarketHistoryItem> x) => x.Key, (KeyValuePair<uint, MarketHistoryItem> x) => x.Value);
				owner.config.Save(ManagerBase<ModuleManager>.Instance().GetModule<BetterMarketBoard>());
			}
		}

		public void ClearAllData()
		{
			selectedListings.Clear();
			DisposeAllSubscriptions();
			subscriptionCache.Clear();
			historySubscriptionCache.Clear();
			aggregatedSubscriptionCache.Clear();
			tooltipAggregatedSubscriptionCache.Clear();
			onlineDataCache.Clear();
			onlineAggregatedCache.Clear();
			onlineHistoryCache.Clear();
			ClearDerivedCaches();
			cachedDCWorldPrices.Clear();
			worldPriceRanks = null;
			worldPriceTableDirty = true;
			localListingsData = null;
			localListingsFingerprint = default((int, uint, bool, uint, int));
			localListingsStale = false;
			localSearchRetryDeadline = 0L;
			localListingsBaseline = default((uint, uint, uint));
			itemEpoch++;
			onlineDataVersion++;
			onlineHistoryVersion++;
		}

		private void ClearDerivedCaches()
		{
			historyDataCache.Clear();
			trendDataCache.Clear();
			listingsDataCache.Clear();
		}

		private void DisposeSelectedWorldData(uint itemID, uint worldID)
		{
			if (worldID == 0)
			{
				return;
			}
			foreach (var item in subscriptionCache.Keys.Where(((uint ItemID, uint WorldID, bool HQOnly) tuple) => tuple.ItemID == itemID && tuple.WorldID == worldID).ToList())
			{
				subscriptionCache[item].Dispose();
				subscriptionCache.Remove(item);
				onlineDataCache.Remove(item);
			}
			(uint, uint) key = (itemID, worldID);
			if (historySubscriptionCache.Remove(key, out IDisposable value))
			{
				value.Dispose();
			}
			onlineHistoryCache.Remove(key);
		}

		private void DisposeAllSubscriptions()
		{
			foreach (IDisposable value in subscriptionCache.Values)
			{
				value?.Dispose();
			}
			foreach (IDisposable value2 in historySubscriptionCache.Values)
			{
				value2?.Dispose();
			}
			foreach (IDisposable value3 in aggregatedSubscriptionCache.Values)
			{
				value3?.Dispose();
			}
			foreach (IDisposable value4 in tooltipAggregatedSubscriptionCache.Values)
			{
				value4?.Dispose();
			}
		}

		private bool RequestAllWorldsData(uint itemID, string? regionName = null, bool hqOnly = false)
		{
			if (owner.allWorlds.Count == 0)
			{
				return false;
			}
			Dictionary<string, Dictionary<uint, string>> value = null;
			string text = ((!string.IsNullOrEmpty(regionName)) ? regionName : EffectiveRegionName);
			if (!string.IsNullOrEmpty(text))
			{
				owner.allWorlds.TryGetValue(text, out value);
			}
			if (value == null)
			{
				value = owner.allWorlds.Values.FirstOrDefault((Dictionary<string, Dictionary<uint, string>> r) => r.Values.Any((Dictionary<uint, string> dc) => dc.ContainsKey(GameState.CurrentWorld)));
			}
			if (value == null)
			{
				return false;
			}
			UniversalisMarketHistoryRequestParams parameters = new UniversalisMarketHistoryRequestParams
			{
				EntriesToReturn = 200,
				StatsWithin = 15552000000L,
				EntriesWithin = 15552000000L
			};
			UniversalisMarketDataRequestParams parameters2 = new UniversalisMarketDataRequestParams
			{
				HQ = hqOnly
			};
			int epoch = itemEpoch;
			bool result = false;
			foreach (Dictionary<uint, string> value5 in value.Values)
			{
				foreach (KeyValuePair<uint, string> item2 in value5)
				{
					item2.Deconstruct(out var key, out var value2);
					uint item = key;
					string worldOrDC = value2;
					(uint itemID, uint worldID) aggregatedCacheKey = (itemID: itemID, worldID: item);
					RemoteUniversalisAggregatedMarket.GetOrRequest(new global::_003C_003Ez__ReadOnlySingleElementList<uint>(itemID), worldOrDC);
					if (aggregatedSubscriptionCache.ContainsKey(aggregatedCacheKey))
					{
						continue;
					}
					aggregatedSubscriptionCache[aggregatedCacheKey] = RemoteUniversalisAggregatedMarket.Observe(new global::_003C_003Ez__ReadOnlySingleElementList<uint>(itemID), worldOrDC, delegate(RemoteSnapshot<UniversalisAggregatedMarketDataResponse> snapshot)
					{
						if (epoch == itemEpoch && snapshot.HasValue)
						{
							UniversalisAggregatedMarketDataResponse value4 = snapshot.Value;
							if (value4 != null && !value4.Results.All((UniversalisAggregatedMarketResult x) => x.ItemID != itemID))
							{
								onlineAggregatedCache[aggregatedCacheKey] = value4;
								worldPriceTableDirty = true;
								OmenServiceBase<TooltipManager>.Instance().TriggerItemDetailUpdate();
							}
						}
					});
					result = true;
				}
			}
			string value3 = value.Values.SelectMany((Dictionary<uint, string> dc) => dc).FirstOrDefault((KeyValuePair<uint, string> world) => world.Key == SelectedWorldID).Value;
			if (string.IsNullOrEmpty(value3))
			{
				return result;
			}
			(uint itemID, uint SelectedWorldID, bool hqOnly) marketCacheKey = (itemID: itemID, SelectedWorldID: SelectedWorldID, hqOnly: hqOnly);
			RemoteUniversalisMarket.GetOrRequest(new global::_003C_003Ez__ReadOnlySingleElementList<uint>(itemID), value3, parameters2);
			if (!subscriptionCache.ContainsKey(marketCacheKey))
			{
				subscriptionCache[marketCacheKey] = RemoteUniversalisMarket.Observe(new global::_003C_003Ez__ReadOnlySingleElementList<uint>(itemID), value3, delegate(RemoteSnapshot<UniversalisMarketDataResponse> snapshot)
				{
					if (epoch == itemEpoch && snapshot.HasValue)
					{
						UniversalisMarketDataResponse value4 = snapshot.Value;
						if (value4 != null && SelectedWorldID == value4.WorldID)
						{
							onlineDataCache[marketCacheKey] = value4;
							onlineDataVersion++;
							OmenServiceBase<TooltipManager>.Instance().TriggerItemDetailUpdate();
						}
					}
				}, parameters2);
				result = true;
			}
			(uint itemID, uint SelectedWorldID) historyCacheKey = (itemID: itemID, SelectedWorldID: SelectedWorldID);
			RemoteUniversalisHistory.GetOrRequest(new global::_003C_003Ez__ReadOnlySingleElementList<uint>(itemID), value3, parameters);
			if (!historySubscriptionCache.ContainsKey(historyCacheKey))
			{
				historySubscriptionCache[historyCacheKey] = RemoteUniversalisHistory.Observe(new global::_003C_003Ez__ReadOnlySingleElementList<uint>(itemID), value3, delegate(RemoteSnapshot<UniversalisMarketHistoryResponse> snapshot)
				{
					if (epoch == itemEpoch && snapshot.HasValue)
					{
						UniversalisMarketHistoryResponse value4 = snapshot.Value;
						if (value4 != null && SelectedWorldID == value4.WorldID)
						{
							onlineHistoryCache[historyCacheKey] = value4;
							onlineHistoryVersion++;
							OmenServiceBase<TooltipManager>.Instance().TriggerItemDetailUpdate();
						}
					}
				}, parameters);
				result = true;
			}
			return result;
		}

		private static T? GetOrBuild<T>(CachedValue<T?> slot, int version, Func<T?> build) where T : class
		{
			if (slot.Version != version)
			{
				slot.Value = build();
				slot.Version = version;
			}
			return slot.Value;
		}

		private static HistoryEntry ToHistoryEntry(UniversalisHistorySale sale)
		{
			DateTime dateTime = sale.GetSaleTime().ToLocalTime();
			return new HistoryEntry(((DateTimeOffset)dateTime).ToUnixTimeSeconds(), dateTime, sale.PricePerUnit, sale.Quantity, sale.HQ);
		}

		public HistoryDataSet? GetHistoryDataSet(uint itemID, bool? hqOnly = null)
		{
			if (itemID == 0)
			{
				return null;
			}
			bool item = hqOnly ?? HQOnly;
			(uint ItemID, uint WorldID, bool HQOnly) key = (ItemID: itemID, WorldID: SelectedWorldID, HQOnly: item);
			return GetOrBuild(historyDataCache.GetOrAdd(key, ((uint ItemID, uint WorldID, bool HQOnly) _) => new CachedValue<HistoryDataSet>()), onlineHistoryVersion, () => BuildHistoryDataSet(itemID, key.WorldID, key.HQOnly));
		}

		private HistoryDataSet? BuildHistoryDataSet(uint itemID, uint worldID, bool hqOnly)
		{
			if (onlineHistoryCache.TryGetValue((itemID, worldID), out UniversalisMarketHistoryResponse value) && value.Items.TryGetValue(itemID, out UniversalisHistoryItemData value2))
			{
				List<UniversalisHistorySale> entries = value2.Entries;
				if (entries != null && entries.Count > 0)
				{
					bool isAnyHQ = value2.Entries.Any((UniversalisHistorySale x) => x.HQ);
					List<HistoryEntry> list = value2.Entries.Where((UniversalisHistorySale x) => x != null && !x.OnMannequin && x.PricePerUnit != 0 && (!hqOnly || x.HQ)).Select(ToHistoryEntry).ToList();
					if (list.Count == 0)
					{
						return null;
					}
					Item item;
					bool flag = LuminaGetter.TryGetRow<Item>(itemID, out item) && ((Item)(ref item)).CanBeHq;
					int count = list.Count;
					uint num = 0u;
					double num2 = 0.0;
					int num3 = 0;
					double num4 = 0.0;
					double num5 = 0.0;
					uint num6 = 0u;
					uint num7 = 0u;
					foreach (HistoryEntry item2 in list)
					{
						num += item2.Quantity;
						num2 += (double)item2.PricePerUnit * (double)item2.Quantity;
						if (item2.IsHQ)
						{
							num3++;
							num5 += (double)item2.PricePerUnit * (double)item2.Quantity;
							num7 += item2.Quantity;
						}
						else
						{
							num4 += (double)item2.PricePerUnit * (double)item2.Quantity;
							num6 += item2.Quantity;
						}
					}
					ulong avgPrice = ((num != 0) ? ((ulong)Math.Round(num2 / (double)num)) : 0);
					int hQPercent = (int)Math.Round((double)num3 / (double)count * 100.0);
					ulong avgNQPrice = 0uL;
					ulong avgHQPrice = 0uL;
					if (flag)
					{
						avgNQPrice = ((num6 != 0) ? ((ulong)Math.Round(num4 / (double)num6)) : 0);
						avgHQPrice = ((num7 != 0) ? ((ulong)Math.Round(num5 / (double)num7)) : 0);
					}
					return new HistoryDataSet
					{
						Entries = list,
						TotalCount = count,
						TotalQty = num,
						AvgPrice = avgPrice,
						HQCount = num3,
						HQPercent = hQPercent,
						IsCanBeHQ = flag,
						IsAnyHQ = isAnyHQ,
						AvgNQPrice = avgNQPrice,
						AvgHQPrice = avgHQPrice
					};
				}
			}
			return null;
		}

		public TrendDataSet? GetTrendDataSet(uint itemID)
		{
			if (itemID == 0)
			{
				return null;
			}
			(uint ItemID, uint WorldID, bool HQOnly) key = (ItemID: itemID, WorldID: SelectedWorldID, HQOnly: HQOnly);
			return GetOrBuild(trendDataCache.GetOrAdd(key, ((uint ItemID, uint WorldID, bool HQOnly) _) => new CachedValue<TrendDataSet>()), onlineHistoryVersion, () => BuildTrendDataSet(itemID, key.WorldID, key.HQOnly));
		}

		private TrendDataSet? BuildTrendDataSet(uint itemID, uint worldID, bool hqOnly)
		{
			if (onlineHistoryCache.TryGetValue((itemID, worldID), out UniversalisMarketHistoryResponse value) && value.Items.TryGetValue(itemID, out UniversalisHistoryItemData value2))
			{
				List<UniversalisHistorySale> entries = value2.Entries;
				if (entries != null && entries.Count > 0)
				{
					List<HistoryEntry> list = new List<HistoryEntry>();
					foreach (UniversalisHistorySale entry in value2.Entries)
					{
						if (!entry.OnMannequin && (!hqOnly || entry.HQ) && entry.PricePerUnit != 0)
						{
							list.Add(ToHistoryEntry(entry));
						}
					}
					if (list.Count == 0)
					{
						return null;
					}
					list.Sort((HistoryEntry left, HistoryEntry right) => left.SaleTime.CompareTo(right.SaleTime));
					long num = list.Sum((HistoryEntry x) => x.Quantity);
					double num2 = ((IEnumerable<HistoryEntry>)list).Sum((Func<HistoryEntry, double>)((HistoryEntry x) => x.PricePerUnit * x.Quantity));
					ulong avgHistoryPrice = ((num > 0) ? ((ulong)Math.Round(num2 / (double)num)) : 0);
					Item item;
					bool flag = LuminaGetter.TryGetRow<Item>(itemID, out item) && ((Item)(ref item)).CanBeHq;
					ulong avgNQPrice = 0uL;
					ulong avgHQPrice = 0uL;
					int num3 = 0;
					if (flag)
					{
						double num4 = 0.0;
						double num5 = 0.0;
						long num6 = 0L;
						long num7 = 0L;
						foreach (HistoryEntry item4 in list)
						{
							if (item4.IsHQ)
							{
								num3++;
								num5 += (double)(item4.PricePerUnit * item4.Quantity);
								num7 += item4.Quantity;
							}
							else
							{
								num4 += (double)(item4.PricePerUnit * item4.Quantity);
								num6 += item4.Quantity;
							}
						}
						avgNQPrice = ((num6 > 0) ? ((ulong)Math.Round(num4 / (double)num6)) : 0);
						avgHQPrice = ((num7 > 0) ? ((ulong)Math.Round(num5 / (double)num7)) : 0);
					}
					int nQCount = list.Count - num3;
					(double VisualMin, double VisualMax) tuple = CalculateVisualPriceRange(list.Select((HistoryEntry x) => x.PricePerUnit).ToArray());
					double item2 = tuple.VisualMin;
					double item3 = tuple.VisualMax;
					double num8 = item2;
					double num9 = item3;
					if (Math.Abs(num9 - num8) < double.Epsilon)
					{
						double num10 = Math.Max(1.0, num9 * 0.05);
						num8 = Math.Max(0.0, num8 - num10);
						num9 += num10;
					}
					else
					{
						double num11 = (num9 - num8) * 0.08;
						num8 = Math.Max(0.0, num8 - num11);
						num9 += num11;
					}
					double num12 = list.Min((HistoryEntry x) => x.X);
					double num13 = list.Max((HistoryEntry x) => x.X);
					if (Math.Abs(num13 - num12) < double.Epsilon)
					{
						num12 -= 3600.0;
						num13 += 3600.0;
					}
					else
					{
						double num14 = (num13 - num12) * 0.05;
						num12 -= num14;
						num13 += num14;
					}
					return new TrendDataSet
					{
						Entries = list,
						XMin = num12,
						XMax = num13,
						VisualMin = num8,
						VisualMax = num9,
						AvgHistoryPrice = avgHistoryPrice,
						IsCanBeHQ = flag,
						HQCount = num3,
						NQCount = nQCount,
						AvgNQPrice = avgNQPrice,
						AvgHQPrice = avgHQPrice
					};
				}
			}
			return null;
		}

		public ListingsDataSet? GetListingsDataSet(uint itemID)
		{
			if (itemID == 0)
			{
				return null;
			}
			(uint ItemID, uint WorldID, bool HQOnly) key = (ItemID: itemID, WorldID: SelectedWorldID, HQOnly: HQOnly);
			return GetOrBuild(listingsDataCache.GetOrAdd(key, ((uint ItemID, uint WorldID, bool HQOnly) _) => new CachedValue<ListingsDataSet>()), onlineDataVersion, () => BuildListingsDataSet(itemID, key.WorldID, key.HQOnly));
		}

		private ListingsDataSet? BuildListingsDataSet(uint itemID, uint worldID, bool hqOnly)
		{
			if (!onlineDataCache.TryGetValue((itemID, worldID, hqOnly), out UniversalisMarketDataResponse value) || !value.Items.TryGetValue(itemID, out UniversalisMarketItemData value2))
			{
				return null;
			}
			List<UniversalisMarketListing> list = (from x in value2.Listings ?? new List<UniversalisMarketListing>()
				where x.PricePerUnit != 0 && (x.HQ || !hqOnly)
				orderby x.PricePerUnit
				select x).ToList();
			return new ListingsDataSet
			{
				Listings = list,
				Source = value2,
				IsAnyHQ = list.Any((UniversalisMarketListing x) => x.HQ),
				IsAnyOnMannequin = list.Any((UniversalisMarketListing x) => x.OnMannequin),
				TotalCount = list.Count,
				TotalQty = list.Aggregate(0u, (uint acc, UniversalisMarketListing l) => acc + l.Quantity)
			};
		}

		public unsafe LocalListingsDataSet GetLocalListingsDataSet(InfoProxyItemSearch* info)
		{
			if (localListingsStale)
			{
				(uint, uint, uint) tuple = (((InfoProxyItemSearch)info).SearchItemId, ((InfoProxyItemSearch)info).EntryCount, ((InfoProxyItemSearch)info).ListingCount);
				(uint, uint, uint) tuple2 = localListingsBaseline;
				if (tuple.Item1 == tuple2.Item1 && tuple.Item2 == tuple2.Item2 && tuple.Item3 == tuple2.Item3)
				{
					return EmptyLocalListings();
				}
				localListingsStale = false;
			}
			if (!(*info).IsFullyReceived())
			{
				return EmptyLocalListings();
			}
			int num = (int)Math.Min(((InfoProxyItemSearch)info).EntryCount, ((InfoProxyItemSearch)info).ListingCount);
			MarketBoardListing[] subArray = ((InfoProxyItemSearch)info).Listings.ToArray()[..num];
			int item = CalculateLocalListingsHash(subArray);
			(int, uint, bool, uint, int) tuple3 = (itemEpoch, ((InfoProxyItemSearch)info).SearchItemId, HQOnly, ((InfoProxyItemSearch)info).ListingCount, item);
			if (localListingsData != null)
			{
				(int, uint, bool, uint, int) tuple4 = localListingsFingerprint;
				(int, uint, bool, uint, int) tuple5 = tuple3;
				if (tuple4.Item1 == tuple5.Item1 && tuple4.Item2 == tuple5.Item2 && tuple4.Item3 == tuple5.Item3 && tuple4.Item4 == tuple5.Item4 && tuple4.Item5 == tuple5.Item5)
				{
					return localListingsData;
				}
			}
			localListingsFingerprint = tuple3;
			localListingsData = BuildLocalListingsDataSet(((InfoProxyItemSearch)info).SearchItemId, subArray);
			ReconcileSelectedListings(localListingsData.Listings);
			worldPriceTableDirty = true;
			return localListingsData;
		}

		private static LocalListingsDataSet EmptyLocalListings()
		{
			return EmptyLocalListingsData;
		}

		private static int CalculateLocalListingsHash(IReadOnlyList<MarketBoardListing> listings)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			HashCode hashCode = default(HashCode);
			foreach (MarketBoardListing listing in listings)
			{
				MarketBoardListing current = listing;
				hashCode.Add(current.ListingId);
				hashCode.Add(current.ItemId);
				hashCode.Add(current.UnitPrice);
				hashCode.Add(current.Quantity);
				hashCode.Add(current.IsHqItem);
				hashCode.Add(((MarketBoardListing)(ref current)).IsSellingAsSet);
				hashCode.Add(current.MateriaCount);
				hashCode.Add(current.TotalTax);
			}
			return hashCode.ToHashCode();
		}

		private void ReconcileSelectedListings(IReadOnlyList<MarketBoardListing> listings)
		{
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			Dictionary<ulong, MarketBoardListing> dictionary = listings.DistinctBy((MarketBoardListing listing) => listing.ListingId).ToDictionary((MarketBoardListing listing) => listing.ListingId);
			foreach (ulong item in selectedListings.Keys.ToList())
			{
				if (!dictionary.TryGetValue(item, out var value))
				{
					selectedListings.Remove(item);
				}
				else if (IsOwnRetainer(value.RetainerId))
				{
					selectedListings.Remove(item);
				}
				else
				{
					selectedListings[item] = value;
				}
			}
		}

		private LocalListingsDataSet BuildLocalListingsDataSet(uint itemID, IReadOnlyList<MarketBoardListing> sourceListings)
		{
			MarketBoardListing[] array = (from x in sourceListings
				where x.ItemId == itemID && x.UnitPrice != 0 && (x.IsHqItem || !HQOnly)
				orderby x.UnitPrice
				select x).ToArray();
			bool isAnyHQ = array.Any((MarketBoardListing x) => x.IsHqItem);
			bool isAnyOnMannequin = array.Any((MarketBoardListing x) => ((MarketBoardListing)(ref x)).IsSellingAsSet);
			Item item;
			bool isAnyMateria = LuminaGetter.TryGetRow<Item>(itemID, out item) && ((Item)(ref item)).MateriaSlotCount > 0 && array.Any((MarketBoardListing x) => x.MateriaCount > 0);
			return new LocalListingsDataSet
			{
				Listings = array.ToList(),
				IsAnyHQ = isAnyHQ,
				IsAnyOnMannequin = isAnyOnMannequin,
				IsAnyMateria = isAnyMateria,
				TotalCount = array.Length,
				TotalQty = array.Aggregate(0u, (uint acc, MarketBoardListing l) => acc + l.Quantity)
			};
		}

		public List<SearchCategoryGroup> GetSearchGroups(string input)
		{
			List<Item> searchResult = owner.searcher.SearchResult;
			if (searchGroupsCache != null && searchInputCache == input && searchResultRefCache == searchResult)
			{
				return searchGroupsCache;
			}
			searchInputCache = input;
			searchResultRefCache = searchResult;
			searchGroupsCache = (from g in searchResult.GroupBy(delegate(Item x)
				{
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_000a: Unknown result type (might be due to invalid IL or missing references)
					//IL_000f: Unknown result type (might be due to invalid IL or missing references)
					ItemSearchCategory value = ((Item)(ref x)).ItemSearchCategory.Value;
					return ((ItemSearchCategory)(ref value)).RowId;
				})
				select new SearchCategoryGroup(LuminaGetter.GetRowOrDefault<ItemSearchCategory>(g.Key), g.ToList())).ToList();
			return searchGroupsCache;
		}

		public unsafe WorldPriceRanks? GetWorldPriceRanks(uint itemID)
		{
			string effectiveRegionName = EffectiveRegionName;
			if (string.IsNullOrEmpty(effectiveRegionName) || !owner.allWorlds.TryGetValue(effectiveRegionName, out Dictionary<string, Dictionary<uint, string>> value) || value.Count == 0)
			{
				worldPriceRanks = null;
				return null;
			}
			if (priceTableRegion != effectiveRegionName || priceTableHQOnly != HQOnly || (InfoProxy != null && ((InfoProxyItemSearch)InfoProxy).EntryCount != 0 && cachedDCWorldPrices.Count == 0) || (worldPriceTableDirty && Throttler.Shared.Throttle("BetterMarketBoard-PriceTableUpdate")))
			{
				priceTableRegion = effectiveRegionName;
				priceTableHQOnly = HQOnly;
				cachedDCWorldPrices.Clear();
				foreach (KeyValuePair<string, Dictionary<uint, string>> item in value)
				{
					item.Deconstruct(out var key, out var value2);
					string key2 = key;
					Dictionary<uint, string> dictionary = value2;
					List<WorldPriceRow> list = new List<WorldPriceRow>();
					foreach (KeyValuePair<uint, string> item2 in dictionary)
					{
						item2.Deconstruct(out var key3, out key);
						uint num = key3;
						string worldName = key;
						ulong minPrice = ulong.MaxValue;
						if (num == GameState.CurrentWorld)
						{
							if (IsAbleToSearchLocalMarket() && InfoProxy != null && ((InfoProxyItemSearch)InfoProxy).SearchItemId == itemID && (*InfoProxy).IsFullyReceived(itemID))
							{
								List<MarketBoardListing> list2 = (from x in ((InfoProxyItemSearch)InfoProxy).Listings.ToArray()
									where x.ItemId == itemID && x.UnitPrice != 0 && (x.IsHqItem || !HQOnly)
									select x).ToList();
								if (list2.Count > 0)
								{
									minPrice = list2.Min((MarketBoardListing x) => x.UnitPrice);
								}
							}
							else
							{
								TryGetOnlineAggregatedMinPrice(itemID, num, out minPrice);
							}
						}
						else
						{
							TryGetOnlineAggregatedMinPrice(itemID, num, out minPrice);
						}
						list.Add(new WorldPriceRow(num, worldName, minPrice));
					}
					cachedDCWorldPrices[key2] = list.OrderBy((WorldPriceRow w) => w.WorldName).ToList();
				}
				List<WorldPriceRow> list3 = cachedDCWorldPrices.SelectMany<KeyValuePair<string, List<WorldPriceRow>>, WorldPriceRow>((KeyValuePair<string, List<WorldPriceRow>> x) => x.Value).ToList();
				if (list3.Count > 0)
				{
					MinPriceData = list3.MinBy((WorldPriceRow x) => x.MinPrice);
					List<WorldPriceRow> list4 = list3.Where((WorldPriceRow x) => x.MinPrice != ulong.MaxValue).ToList();
					MaxPriceData = ((list4.Count > 0) ? list4.MaxBy((WorldPriceRow x) => x.MinPrice) : default(WorldPriceRow));
				}
				else
				{
					MinPriceData = default(WorldPriceRow);
					MaxPriceData = default(WorldPriceRow);
				}
				worldPriceRanks = BuildWorldPriceRanks();
				worldPriceTableDirty = false;
			}
			return worldPriceRanks;
		}

		private WorldPriceRanks BuildWorldPriceRanks()
		{
			List<RankedWorldPriceRow> list = (from x in cachedDCWorldPrices.SelectMany<KeyValuePair<string, List<WorldPriceRow>>, RankedWorldPriceRow>((KeyValuePair<string, List<WorldPriceRow>> dc) => from world in dc.Value
					where world.MinPrice != ulong.MaxValue
					select new RankedWorldPriceRow(dc.Key, world.WorldID, world.WorldName, world.MinPrice))
				orderby x.MinPrice, x.WorldName
				select x).ToList();
			List<RankedWorldPriceRow> list2 = list.Take(Math.Min(3, Math.Max(1, list.Count / 2))).ToList();
			HashSet<uint> cheapestWorldIDs = list2.Select((RankedWorldPriceRow x) => x.WorldID).ToHashSet();
			List<RankedWorldPriceRow> expensive = (from x in (from x in list
					where !cheapestWorldIDs.Contains(x.WorldID)
					orderby x.MinPrice descending, x.WorldName
					select x).Take(Math.Min(3, list.Count - cheapestWorldIDs.Count))
				orderby x.MinPrice, x.WorldName
				select x).ToList();
			if (list.Count == 1)
			{
				expensive = list.ToList();
				list2.Clear();
			}
			WorldPriceRow current = cachedDCWorldPrices.SelectMany<KeyValuePair<string, List<WorldPriceRow>>, WorldPriceRow>((KeyValuePair<string, List<WorldPriceRow>> x) => x.Value).FirstOrDefault((WorldPriceRow x) => x.WorldID == GameState.CurrentWorld);
			return new WorldPriceRanks(list, list2, expensive, current);
		}

		private bool TryGetOnlineAggregatedMinPrice(uint itemID, uint worldID, out ulong minPrice)
		{
			minPrice = ulong.MaxValue;
			if (!onlineAggregatedCache.TryGetValue((itemID, worldID), out UniversalisAggregatedMarketDataResponse value))
			{
				return false;
			}
			UniversalisAggregatedMarketResult universalisAggregatedMarketResult = value.Results.FirstOrDefault((UniversalisAggregatedMarketResult result) => result.ItemID == itemID);
			if (universalisAggregatedMarketResult == null)
			{
				return false;
			}
			double? price = GetAggregatedMarketScope(universalisAggregatedMarketResult, HQOnly).MinListing.World.Price;
			if (!price.HasValue || !(price.GetValueOrDefault() > 0.0))
			{
				return false;
			}
			minPrice = (ulong)Math.Round(price.Value);
			return true;
		}

		public (float DailySales, ulong? AvgPrice, (ulong Price, DateTime Time, string WorldName)? RecentPurchase) GetItemAggregatedStats(uint itemID, uint targetWorldID, bool hqOnly)
		{
			float item = 0f;
			ulong? item2 = null;
			(ulong, DateTime, string)? item3 = null;
			if (onlineAggregatedCache.TryGetValue((itemID, targetWorldID), out UniversalisAggregatedMarketDataResponse value))
			{
				UniversalisAggregatedMarketResult universalisAggregatedMarketResult = value.Results.FirstOrDefault((UniversalisAggregatedMarketResult x) => x.ItemID == itemID);
				if (universalisAggregatedMarketResult != null)
				{
					UniversalisAggregatedMarketScope aggregatedMarketScope = GetAggregatedMarketScope(universalisAggregatedMarketResult, hqOnly);
					item = aggregatedMarketScope.DailySaleVelocity.World.Quantity ?? aggregatedMarketScope.DailySaleVelocity.Region.Quantity.GetValueOrDefault();
					double? price = aggregatedMarketScope.AverageSalePrice.World.Price;
					if (price.HasValue && price.GetValueOrDefault() > 0.0)
					{
						item2 = (ulong)Math.Round(aggregatedMarketScope.AverageSalePrice.World.Price.Value);
					}
					else
					{
						price = aggregatedMarketScope.AverageSalePrice.Region.Price;
						if (price.HasValue && price.GetValueOrDefault() > 0.0)
						{
							item2 = (ulong)Math.Round(aggregatedMarketScope.AverageSalePrice.Region.Price.Value);
						}
					}
					UniversalisAggregatedMarketPurchasePoint world = aggregatedMarketScope.RecentPurchase.World;
					if (world != null)
					{
						price = world.Price;
						if (price.HasValue && price.GetValueOrDefault() > 0.0)
						{
							long? timestamp = world.Timestamp;
							if (timestamp.HasValue && timestamp.GetValueOrDefault() > 0)
							{
								item3 = ((ulong)Math.Round(aggregatedMarketScope.RecentPurchase.World.Price.Value), DateTimeOffset.FromUnixTimeMilliseconds(aggregatedMarketScope.RecentPurchase.World.Timestamp.Value).LocalDateTime, LuminaWrapper.GetWorldName(targetWorldID));
								goto IL_0316;
							}
						}
					}
					world = aggregatedMarketScope.RecentPurchase.Region;
					if (world != null)
					{
						price = world.Price;
						if (price.HasValue && price.GetValueOrDefault() > 0.0)
						{
							long? timestamp = world.Timestamp;
							if (timestamp.HasValue && timestamp.GetValueOrDefault() > 0)
							{
								string item4 = (((aggregatedMarketScope.RecentPurchase.Region.WorldID ?? 0) != 0) ? LuminaWrapper.GetWorldName(aggregatedMarketScope.RecentPurchase.Region.WorldID.Value) : string.Empty);
								item3 = ((ulong)Math.Round(aggregatedMarketScope.RecentPurchase.Region.Price.Value), DateTimeOffset.FromUnixTimeMilliseconds(aggregatedMarketScope.RecentPurchase.Region.Timestamp.Value).LocalDateTime, item4);
							}
						}
					}
				}
			}
			goto IL_0316;
			IL_0316:
			return (DailySales: item, AvgPrice: item2, RecentPurchase: item3);
		}

		public unsafe ulong? GetSelectedWorldMinPrice(uint itemID, bool hqOnly)
		{
			if (SelectedWorldID == GameState.CurrentWorld && IsAbleToSearchLocalMarket() && InfoProxy != null && ((InfoProxyItemSearch)InfoProxy).SearchItemId == itemID && (*InfoProxy).IsFullyReceived(itemID))
			{
				uint num = (from x in ((InfoProxyItemSearch)InfoProxy).Listings.ToArray()
					where x.ItemId == itemID && x.UnitPrice != 0 && (x.IsHqItem || !hqOnly)
					select x.UnitPrice).DefaultIfEmpty().Min();
				if (num == 0)
				{
					return null;
				}
				return num;
			}
			if (onlineAggregatedCache.TryGetValue((itemID, SelectedWorldID), out UniversalisAggregatedMarketDataResponse value))
			{
				UniversalisAggregatedMarketResult universalisAggregatedMarketResult = value.Results.FirstOrDefault((UniversalisAggregatedMarketResult x) => x.ItemID == itemID);
				double? num2 = ((universalisAggregatedMarketResult == null) ? ((double?)null) : GetAggregatedMarketScope(universalisAggregatedMarketResult, hqOnly).MinListing.World.Price);
				if (num2.HasValue && num2.GetValueOrDefault() > 0.0)
				{
					return (ulong)Math.Round(num2.Value);
				}
			}
			if (!onlineDataCache.TryGetValue((itemID, SelectedWorldID, hqOnly), out UniversalisMarketDataResponse value2) || !value2.Items.TryGetValue(itemID, out UniversalisMarketItemData value3))
			{
				return null;
			}
			uint num3 = (from x in value3.Listings?.Where((UniversalisMarketListing x) => x.PricePerUnit != 0 && (x.HQ || !hqOnly))
				select x.PricePerUnit).DefaultIfEmpty().Min() ?? 0;
			if (num3 == 0)
			{
				return null;
			}
			return num3;
		}

		public ulong? GetRegionMinPrice(uint itemID, bool hqOnly)
		{
			if (!owner.allWorlds.TryGetValue(EffectiveRegionName, out Dictionary<string, Dictionary<uint, string>> value))
			{
				return null;
			}
			ulong? result = null;
			foreach (uint item in value.Values.SelectMany((Dictionary<uint, string> worlds) => worlds.Keys))
			{
				if (!onlineAggregatedCache.TryGetValue((itemID, item), out UniversalisAggregatedMarketDataResponse value2))
				{
					continue;
				}
				UniversalisAggregatedMarketResult universalisAggregatedMarketResult = value2.Results.FirstOrDefault((UniversalisAggregatedMarketResult x) => x.ItemID == itemID);
				double? num = ((universalisAggregatedMarketResult == null) ? ((double?)null) : GetAggregatedMarketScope(universalisAggregatedMarketResult, hqOnly).MinListing.World.Price);
				if (num.HasValue && num.GetValueOrDefault() > 0.0)
				{
					ulong num2 = (ulong)Math.Round(num.Value);
					if (num2 != 0 && (!result.HasValue || num2 < result.Value))
					{
						result = num2;
					}
				}
			}
			return result;
		}

		public ulong? GetHistoryPercentilePrice(uint itemID, bool hqOnly, double percentile)
		{
			List<HistoryEntry> list = GetHistoryDataSet(itemID, hqOnly)?.Entries;
			if (list == null || list.Count <= 0)
			{
				return null;
			}
			ulong[] array = (from x in list
				select x.PricePerUnit into x
				orderby x
				select x).ToArray();
			int num = (int)Math.Round((double)(array.Length - 1) * percentile, MidpointRounding.AwayFromZero);
			if (array[num] == 0)
			{
				return null;
			}
			return array[num];
		}

		public UniversalisAggregatedMarketDataResponse? GetAggregatedResponse(uint itemID, uint worldID)
		{
			return onlineAggregatedCache.GetValueOrDefault((itemID, worldID));
		}

		public ItemSourceInfo? GetItemSourceInfo(uint itemID)
		{
			if (itemSourceCache.TryGetValue(itemID, out ItemSourceInfo value))
			{
				return value;
			}
			ItemSourceQueryResult itemSourceQueryResult = ItemSourceInfo.Query(itemID);
			if (itemSourceQueryResult.State == ItemSourceQueryState.Ready)
			{
				ItemSourceInfo data = itemSourceQueryResult.Data;
				if (data != null)
				{
					itemSourceCache[itemID] = data;
					return data;
				}
			}
			return null;
		}

		public uint? GetNPCGilPrice(uint itemID)
		{
			if (npcGilPriceCache.TryGetValue(itemID, out var value))
			{
				return value;
			}
			ItemSourceInfo itemSourceInfo = GetItemSourceInfo(itemID);
			if (itemSourceInfo == null)
			{
				return null;
			}
			uint? num = null;
			foreach (ShopNPCInfos nPCInfo in itemSourceInfo.NPCInfos)
			{
				foreach (ShopItemCostInfo costInfo in nPCInfo.CostInfos)
				{
					if ((object)costInfo != null && costInfo.ItemID == 1 && costInfo.Cost != 0 && (!num.HasValue || costInfo.Cost < num.Value))
					{
						num = costInfo.Cost;
					}
				}
			}
			npcGilPriceCache[itemID] = num;
			return num;
		}

		public bool RequestTooltipAggregatedScope(uint itemID, string scope)
		{
			if (string.IsNullOrWhiteSpace(scope))
			{
				return false;
			}
			(uint, string) key = (itemID, scope);
			RemoteUniversalisAggregatedMarket.GetOrRequest(new global::_003C_003Ez__ReadOnlySingleElementList<uint>(itemID), scope);
			if (tooltipAggregatedSubscriptionCache.ContainsKey(key))
			{
				return false;
			}
			int epoch = itemEpoch;
			tooltipAggregatedSubscriptionCache[key] = RemoteUniversalisAggregatedMarket.Observe(new global::_003C_003Ez__ReadOnlySingleElementList<uint>(itemID), scope, delegate(RemoteSnapshot<UniversalisAggregatedMarketDataResponse> snapshot)
			{
				if (epoch == itemEpoch && snapshot.HasValue)
				{
					OmenServiceBase<TooltipManager>.Instance().TriggerItemDetailUpdate();
				}
			});
			return true;
		}
	}

	private readonly record struct MetricItem(string Label, string Value, Vector4 ValueColor, string? Tooltip);

	private readonly record struct WorldPriceRow(uint WorldID, string WorldName, ulong MinPrice);

	private readonly record struct RankedWorldPriceRow(string DCName, uint WorldID, string WorldName, ulong MinPrice);

	private readonly record struct HistoryEntry(double X, DateTime SaleTime, ulong PricePerUnit, uint Quantity, bool IsHQ);

	private class Config : ModuleConfig
	{
		public Dictionary<string, Dictionary<string, Dictionary<uint, string>>> AllWorlds = new Dictionary<string, Dictionary<string, Dictionary<uint, string>>>();

		public bool AutoBuy;

		public uint AutoBuyQuantityLimit;

		public MonitorItem? CurrentMonitorItem;

		public Dictionary<uint, MarketFavoriteItem> FavoriteItems = new Dictionary<uint, MarketFavoriteItem>();

		public Dictionary<uint, MarketHistoryItem> HistoryItems = new Dictionary<uint, MarketHistoryItem>();

		public bool AppendMarketStatsTooltip = true;
	}

	private enum ItemSelectorTab
	{
		Search,
		History,
		Favorite
	}

	private class MarketHistoryItem
	{
		public uint ItemID { get; set; }

		public DateTime AccessTime { get; set; }

		public Item GetData()
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			return LuminaGetter.GetRow<Item>(ItemID).GetValueOrDefault();
		}
	}

	private class MarketFavoriteItem
	{
		public uint ItemID { get; set; }

		public string Note { get; set; } = string.Empty;

		public Item GetData()
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			return LuminaGetter.GetRow<Item>(ItemID).GetValueOrDefault();
		}
	}

	private class MonitorItem
	{
		public uint ItemID { get; set; }

		public uint PriceThreshold { get; set; } = 100u;

		public bool HQOnly { get; set; }

		public Item GetData()
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			return LuminaGetter.GetRow<Item>(ItemID).GetValueOrDefault();
		}
	}

	private sealed class PriceMonitorProvider(BetterMarketBoard owner)
	{
		public void SetMonitor(uint itemID, bool hqOnly = false)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			if (LuminaGetter.TryGetRow<Item>(itemID, out Item item) && ((Item)(ref item)).ItemSearchCategory.RowId != 0)
			{
				owner.config.CurrentMonitorItem = new MonitorItem
				{
					ItemID = itemID,
					PriceThreshold = 100u,
					HQOnly = (hqOnly && ((Item)(ref item)).CanBeHq)
				};
				owner.config.Save(owner);
			}
		}

		public void ClearMonitor()
		{
			owner.config.CurrentMonitorItem = null;
			owner.config.Save(owner);
		}

		public void ToggleHQ()
		{
			MonitorItem currentMonitorItem = owner.config.CurrentMonitorItem;
			if (currentMonitorItem != null && LuminaGetter.TryGetRow<Item>(currentMonitorItem.ItemID, out Item item) && ((Item)(ref item)).CanBeHq)
			{
				currentMonitorItem.HQOnly = !currentMonitorItem.HQOnly;
				owner.config.Save(owner);
				if (owner.provider.SelectedItemID == currentMonitorItem.ItemID)
				{
					owner.provider.SelectItem(currentMonitorItem.ItemID, null, currentMonitorItem.HQOnly);
				}
			}
		}

		public void ToggleAutoBuy()
		{
			owner.config.AutoBuy = !owner.config.AutoBuy;
			owner.config.Save(owner);
		}

		public void SetQuantityLimit(uint quantityLimit)
		{
			owner.config.AutoBuyQuantityLimit = quantityLimit;
			owner.config.Save(owner);
		}

		public unsafe void Update()
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			MonitorItem monitorItem = owner.config.CurrentMonitorItem;
			if (monitorItem == null || !LuminaGetter.TryGetRow<Item>(monitorItem.ItemID, out Item item) || ((Item)(ref item)).ItemSearchCategory.RowId == 0 || !IsAbleToSearchMarket() || ((Window)owner.Overlay).IsOpen || (int)GameState.TerritoryIntendedUse != 0)
			{
				return;
			}
			TaskHelper taskHelper = owner.TaskHelper;
			taskHelper.Abort();
			taskHelper.Enqueue(() => MarketDataProvider.RequestLocalSearchData(monitorItem.ItemID));
			taskHelper.Enqueue(() => (*InfoProxyItemSearch.Instance()).IsFullyReceived(monitorItem.ItemID));
			taskHelper.Enqueue(delegate
			{
				//IL_0100: Unknown result type (might be due to invalid IL or missing references)
				//IL_010e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0131: Unknown result type (might be due to invalid IL or missing references)
				//IL_0136: Unknown result type (might be due to invalid IL or missing references)
				//IL_0141: Unknown result type (might be due to invalid IL or missing references)
				//IL_0142: Unknown result type (might be due to invalid IL or missing references)
				//IL_0147: Unknown result type (might be due to invalid IL or missing references)
				//IL_015a: Unknown result type (might be due to invalid IL or missing references)
				//IL_015f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0179: Unknown result type (might be due to invalid IL or missing references)
				//IL_0192: Unknown result type (might be due to invalid IL or missing references)
				//IL_0226: Unknown result type (might be due to invalid IL or missing references)
				//IL_022b: Unknown result type (might be due to invalid IL or missing references)
				//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
				//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
				//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
				//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
				List<MarketBoardListing> list = (from x in ((InfoProxyItemSearch)InfoProxy).Listings.ToArray()
					where x.ItemId == monitorItem.ItemID && x.UnitPrice != 0 && x.UnitPrice <= monitorItem.PriceThreshold && (!monitorItem.HQOnly || x.IsHqItem) && !IsOwnRetainer(x.RetainerId)
					orderby x.UnitPrice
					select x).ToList();
				if (list.Count == 0)
				{
					owner.TaskHelper.Abort();
				}
				else
				{
					uint orAdd = owner.itemIDToPayloadID.GetOrAdd(monitorItem.ItemID, delegate(uint itemID)
					{
						OmenServiceBase<LinkPayloadManager>.Instance().Reg(delegate
						{
							owner.provider.SelectItem(itemID, null, monitorItem.HQOnly);
							((Window)owner.Overlay).IsOpen = true;
						}, out var id);
						return id;
					});
					if (OmenServiceBase<LinkPayloadManager>.Instance().TryGetPayload(orAdd, out DalamudLinkPayload payload))
					{
						ReadOnlySeString se = LanguageManager.GetSe("BetterMarketBoard-PriceMonitor-Notification-Found", SeStringExtension.CreateItemName(monitorItem.ItemID, monitorItem.HQOnly), list.First().UnitPrice.ToChineseString(), monitorItem.PriceThreshold.ToChineseString());
						ISeStringEvaluator obj = DalamudServiceExtension.Instance<ISeStringEvaluator>();
						SeStringParameter reference = SeStringParameter.op_Implicit(se);
						ReadOnlySeString val = obj.EvaluateFromAddon(371u, new Span<SeStringParameter>(ref reference), (ClientLanguage?)null);
						RentedSeStringBuilder val2 = default(RentedSeStringBuilder);
						((RentedSeStringBuilder)(ref val2))._002Ector();
						try
						{
							SeStringBuilder builder = ((RentedSeStringBuilder)(ref val2)).Builder;
							builder.AppendDalamudSeString((Payload)(object)payload).Append(val).AppendDalamudSeString((Payload)(object)RawPayload.LinkTerminator);
							OmenServiceBase<NotifyHelper>.Instance().Chat(builder.ToReadOnlySeString());
							if (owner.config.AutoBuy)
							{
								uint itemCount = LocalPlayerState.GetItemCount(monitorItem.ItemID);
								uint num = ((owner.config.AutoBuyQuantityLimit == 0) ? uint.MaxValue : owner.config.AutoBuyQuantityLimit);
								if (itemCount <= num)
								{
									uint num2 = 0u;
									uint num3 = 0u;
									foreach (MarketBoardListing listing in list)
									{
										num2 += listing.Quantity;
										num3++;
										if (num2 + itemCount > num)
										{
											break;
										}
										owner.TaskHelper.Enqueue(delegate
										{
											//IL_0001: Unknown result type (might be due to invalid IL or missing references)
											if (!MarketDataProvider.SendBuyRequest(listing))
											{
												taskHelper.Abort();
											}
										}, null, 0, null, null, null, null, 1);
									}
									ReadOnlySeString se2 = LanguageManager.GetSe("BetterMarketBoard-PriceMonitor-Notification-AutoBuy", SeString.CreateItemLink(monitorItem.ItemID, monitorItem.HQOnly, (string)null), num3, num2);
									NotifyHelper.Toast(se2);
									OmenServiceBase<NotifyHelper>.Instance().Chat(se2);
								}
							}
						}
						finally
						{
							((IDisposable)(*(RentedSeStringBuilder*)(&val2))/*cast due to constrained. prefix*/).Dispose();
						}
					}
				}
			});
		}
	}

	private sealed record TooltipMarketStats(ulong? CurrentWorldMinPrice, string? CurrentWorldTimeText, string RegionMinWorldName, ulong? RegionMinPrice, string? RegionTimeText, float DailySales);

	private readonly struct MarketBoardUIContext(MarketDataProvider provider, BetterMarketBoard owner, uint itemID, bool hasItem, Item itemData, uint? npcGilPrice, uint currentWorldID, uint selectedWorldID, bool hqOnly, bool isLocalMarketSearchable)
	{
		public MarketDataProvider Provider { get; } = provider;

		public BetterMarketBoard Owner { get; } = owner;

		public uint ItemID { get; } = itemID;

		public bool HasItem { get; } = hasItem;

		public Item ItemData { get; } = itemData;

		public uint? NPCGilPrice { get; } = npcGilPrice;

		public uint CurrentWorldID { get; } = currentWorldID;

		public uint SelectedWorldID { get; } = selectedWorldID;

		public bool HQOnly { get; } = hqOnly;

		public bool IsLocalMarketSearchable { get; } = isLocalMarketSearchable;

		public bool IsViewingCurrentWorld => SelectedWorldID == CurrentWorldID;
	}

	private sealed class MarketBenchmarkInfo
	{
		public ulong Price;

		public string Badge = string.Empty;

		public Vector4 Color;

		public string? Tooltip;

		public Action? OnClick;

		public bool Drawn;
	}

	private enum OverviewCardType
	{
		Expensive,
		Cheapest,
		SingleOnly
	}

	private sealed class WorldPriceCardComponent : CardComponentBase
	{
		private MarketBoardUIContext frame;

		private WorldPriceRow world;

		private Vector2 targetSize;

		private ulong currentWorldPrice;

		protected override bool WrapInContentTable => false;

		protected override bool EnablePressAnimation => true;

		protected override bool EnableHoverAnimation => true;

		protected override bool EnableTopHighlight => true;

		protected override bool EnableGlow => true;

		protected override float HoverFloatOffset => -1.5f * Globals.GlobalUIScale;

		protected override float PressOffset => 1f * Globals.GlobalUIScale;

		protected override float Rounding => 4f * Globals.GlobalUIScale;

		protected override CardCursorAdvance CursorAdvance => CardCursorAdvance.Right;

		protected override bool IsSelected
		{
			get
			{
				if (!frame.IsViewingCurrentWorld)
				{
					return world.WorldID == frame.SelectedWorldID;
				}
				return world.WorldID == GameState.CurrentWorld;
			}
		}

		protected override Vector4 RestingBorder
		{
			get
			{
				if (world.WorldID == GameState.CurrentWorld)
				{
					return VectorExtensions.WithW(KnownColor.Pink.ToVector4(), 0.85f);
				}
				if (frame.Provider.MinPriceData.WorldID == world.WorldID && world.MinPrice != ulong.MaxValue)
				{
					return VectorExtensions.WithW(KnownColor.GreenYellow.ToVector4(), 0.85f);
				}
				return VectorExtensions.WithW(KnownColor.White.ToVector4(), 0.08f);
			}
		}

		protected override Vector4 HoveredBorder
		{
			get
			{
				if (!IsSelected)
				{
					if (world.WorldID != GameState.CurrentWorld)
					{
						if (frame.Provider.MinPriceData.WorldID != world.WorldID)
						{
							return VectorExtensions.WithW(KnownColor.Chocolate.ToVector4(), 0.8f);
						}
						return KnownColor.GreenYellow.ToVector4();
					}
					return KnownColor.Pink.ToVector4();
				}
				return KnownColor.Chocolate.ToVector4();
			}
		}

		protected override Vector4? SelectedBorder => KnownColor.Chocolate.ToVector4();

		protected override Vector4? CustomBackgroundColor
		{
			get
			{
				if (IsSelected)
				{
					return null;
				}
				if (frame.Provider.MinPriceData.WorldID == world.WorldID && world.MinPrice != ulong.MaxValue)
				{
					return VectorExtensions.WithW(KnownColor.ForestGreen.ToVector4(), 0.35f);
				}
				if (world.WorldID == GameState.CurrentWorld)
				{
					return VectorExtensions.WithW(KnownColor.DeepPink.ToVector4(), 0.25f);
				}
				return VectorExtensions.WithW(KnownColor.Black.ToVector4(), 0.3f);
			}
		}

		public void Draw(MarketBoardUIContext frameParam, WorldPriceRow worldParam, Vector2 sizeParam, ulong currentWorldPriceParam)
		{
			frame = frameParam;
			world = worldParam;
			targetSize = sizeParam;
			currentWorldPrice = currentWorldPriceParam;
			Draw();
		}

		protected override CardDrawContext CreateContext()
		{
			Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
			return new CardDrawContext(cursorScreenPos, cursorScreenPos, Vector2.Zero, targetSize.X);
		}

		protected override Vector2 GetTargetSize(CardDrawContext context, Vector2 contentRectSize)
		{
			return targetSize;
		}

		protected override void DrawContent(CardDrawContext context, bool isHovered)
		{
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0310: Unknown result type (might be due to invalid IL or missing references)
			bool flag = world.WorldID == GameState.CurrentWorld;
			bool flag2 = frame.Provider.MinPriceData.WorldID == world.WorldID && world.MinPrice != ulong.MaxValue;
			bool flag3 = frame.Provider.MaxPriceData.WorldID == world.WorldID && world.MinPrice != ulong.MaxValue && frame.Provider.MinPriceData.WorldID != frame.Provider.MaxPriceData.WorldID;
			bool flag4 = world.MinPrice == ulong.MaxValue;
			ImU8String val = default(ImU8String);
			((ImU8String)(ref val))._002Ector(10, 1);
			((ImU8String)(ref val)).AppendLiteral("##Capsule_");
			((ImU8String)(ref val)).AppendFormatted<uint>(world.WorldID);
			if (ImGui.InvisibleButton(val, targetSize, (ImGuiButtonFlags)0))
			{
				frame.Provider.SelectWorld(world.WorldID);
			}
			if (!flag && ImGui.IsItemClicked((ImGuiMouseButton)1))
			{
				OmenServiceBase<ChatManager>.Instance().SendMessage("/pdr worldtravel " + LuminaWrapper.GetWorldName(world.WorldID));
			}
			ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
			Vector2 frameMin = context.FrameMin;
			Vector2 vector = frameMin + targetSize;
			((ImDrawListPtr)(ref windowDrawList)).PushClipRect(frameMin, vector, true);
			uint num = (flag ? KnownColor.Pink.ToUInt() : ((!flag2) ? ImGui.GetColorU32((ImGuiCol)1) : KnownColor.GreenYellow.ToUInt()));
			string worldName = world.WorldName;
			using (OmenServiceBase<FontManager>.Instance().UIFont60.Push())
			{
				Vector2 vector2 = ImGui.CalcTextSize(ImU8String.op_Implicit(worldName), false, -1f);
				Vector2 vector3 = new Vector2(frameMin.X + MathF.Max(0f, (targetSize.X - vector2.X) / 2f), frameMin.Y + 3f * Globals.GlobalUIScale);
				((ImDrawListPtr)(ref windowDrawList)).AddText(vector3, num, ImU8String.op_Implicit(worldName));
			}
			string text = (flag4 ? "-" : (world.MinPrice.ToChineseString() + "\ue049"));
			uint num2 = (flag4 ? ImGui.GetColorU32((ImGuiCol)1) : (flag2 ? KnownColor.GreenYellow.ToUInt() : (flag3 ? KnownColor.OrangeRed.ToUInt() : ((!flag) ? KnownColor.White.ToUInt() : KnownColor.LightPink.ToUInt()))));
			Vector2 vector4 = ImGui.CalcTextSize(ImU8String.op_Implicit(text), false, -1f);
			Vector2 vector5 = new Vector2(frameMin.X + MathF.Max(0f, (targetSize.X - vector4.X) / 2f), vector.Y - vector4.Y - 3f * Globals.GlobalUIScale);
			((ImDrawListPtr)(ref windowDrawList)).AddText(vector5, num2, ImU8String.op_Implicit(text));
			((ImDrawListPtr)(ref windowDrawList)).PopClipRect();
			if (isHovered)
			{
				frame.Owner.DrawWorldPriceTooltip(frame, world.WorldID, world.WorldName, world.MinPrice, currentWorldPrice);
			}
		}
	}

	private MarketDataProvider provider;

	private PriceMonitorProvider monitorProvider;

	private Config config;

	private LuminaSearcher<Item> searcher;

	private SearchInMarketMenu contextMenu;

	private Dictionary<string, Dictionary<string, Dictionary<uint, string>>> allWorlds = new Dictionary<string, Dictionary<string, Dictionary<uint, string>>>();

	private readonly Dictionary<uint, List<Item>> searchCategoryToItems = new Dictionary<uint, List<Item>>();

	private readonly Dictionary<uint, uint> itemIDToPayloadID = new Dictionary<uint, uint>();

	private uint lastWorldID;

	private const string COMMAND = "market";

	private const ImGuiWindowFlags WINDOW_FLAGS = (ImGuiWindowFlags)24;

	private static readonly List<ItemSearchCategory> ValidCategories = ((IEnumerable<ItemSearchCategory>)LuminaGetter.Get<ItemSearchCategory>()).Where((ItemSearchCategory x) => !string.IsNullOrEmpty(((object)((ItemSearchCategory)(ref x)).Name/*cast due to constrained. prefix*/).ToString()) && ((ItemSearchCategory)(ref x)).Category > 0).ToList();

	[IPCSubscriber("DailyRoutines.Modules.AutoShowItemNPCShopInfo.OpenByItemID")]
	private static IPCSubscriber<uint, bool> OpenShopListByItemIDIPC;

	private const ImGuiWindowFlags OVERLAY_CHILD_FLAGS = (ImGuiWindowFlags)152;

	private static readonly ItemSelectorTab[] ItemSelectorTabs = new ItemSelectorTab[3]
	{
		ItemSelectorTab.Search,
		ItemSelectorTab.Favorite,
		ItemSelectorTab.History
	};

	private string itemSearchInput = string.Empty;

	private Vector2 marketDataTableImageSize = new Vector2(32f);

	private List<MarketFavoriteItem>? favoriteItemsCache;

	private int favoriteItemsVersion;

	private int favoriteItemsCacheVersion = -1;

	private bool isAllWorldsPriceExpanded;

	private bool isItemListTooltip;

	private bool isHeaderTooltip;

	private ItemSelectorTab currentTab;

	private bool isConfirmClearHistory;

	private string historySearchInput = string.Empty;

	private readonly Dictionary<uint, WorldPriceCardComponent> worldPriceCards = new Dictionary<uint, WorldPriceCardComponent>();

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("BetterMarketBoardTitle"),
		Description = LanguageManager.Get("BetterMarketBoardDescription", "market"),
		Category = ModuleCategory.Interface,
		Author = new string[2] { "Fragile", "ziz" },
		ModulesPrerequisite = new string[2] { "FastWorldTravel", "AutoShowItemNPCShopInfo" },
		ModulesPair = new string[1] { "AutoRefreshMarketSearchResult" },
		PreviewImageURL = new string[1] { "hxxps://gh.atmoomen.top/raw.githubusercontent.com/Dalamud-DailyRoutines/DailyRoutines/main/Resources/Modules/BetterMarketBoard/preview-1.png" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		NeedAuth = true,
		AllDefaultEnabled = true
	};

	private unsafe static InfoProxyItemSearch* InfoProxy => InfoProxyItemSearch.Instance();

	protected unsafe override void Init()
	{
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Expected O, but got Unknown
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Expected O, but got Unknown
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Expected O, but got Unknown
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_045e: Expected O, but got Unknown
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		this.config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		Config config = this.config;
		if (config != null && config.CurrentMonitorItem != null)
		{
			MonitorItem currentMonitorItem = this.config.CurrentMonitorItem;
			if (!LuminaGetter.TryGetRow<Item>(currentMonitorItem.ItemID, out Item item))
			{
				this.config.CurrentMonitorItem = null;
				this.config.Save(this);
			}
			else if (currentMonitorItem.HQOnly && !((Item)(ref item)).CanBeHq)
			{
				currentMonitorItem.HQOnly = false;
				this.config.Save(this);
			}
		}
		Dictionary<string, Dictionary<string, Dictionary<uint, string>>> dictionary = this.config.AllWorlds;
		if (dictionary != null && dictionary.Count > 0)
		{
			allWorlds = this.config.AllWorlds;
		}
		if (base.Overlay == null)
		{
			Overlay overlay = (base.Overlay = new Overlay(this));
		}
		((Window)base.Overlay).Flags = (ImGuiWindowFlags)24;
		((Window)base.Overlay).WindowName = LuminaWrapper.GetPlaceName(1281u) + "###BetterMarketBoard-Overlay";
		Overlay? overlay3 = base.Overlay;
		WindowSizeConstraints value = default(WindowSizeConstraints);
		((WindowSizeConstraints)(ref value))._002Ector();
		((WindowSizeConstraints)(ref value)).MaximumSize = new Vector2(float.MaxValue);
		((WindowSizeConstraints)(ref value)).MinimumSize = Globals.ScaledVector2(300f, 200f);
		((Window)overlay3).SizeConstraints = value;
		Dictionary<uint, List<Item>> dictionary2 = (from x in (IEnumerable<Item>)LuminaGetter.Get<Item>()
			where !string.IsNullOrEmpty(((object)((Item)(ref x)).Name/*cast due to constrained. prefix*/).ToString()) && ((Item)(ref x)).ItemSearchCategory.RowId != 0
			group x by ((Item)(ref x)).ItemSearchCategory.RowId).ToDictionary((IGrouping<uint, Item> g) => g.Key, (IGrouping<uint, Item> g) => g.OrderBy((Item x) => ((Item)(ref x)).LevelItem.RowId).ToList());
		foreach (var (key, value2) in dictionary2)
		{
			searchCategoryToItems[key] = value2;
		}
		provider = new MarketDataProvider(this);
		monitorProvider = new PriceMonitorProvider(this);
		provider.AnchorWorld();
		lastWorldID = GameState.CurrentWorld;
		List<Item> list2 = new List<Item>();
		list2.AddRange(from x in dictionary2.Values.SelectMany((List<Item> x) => x)
			group x by ((object)((Item)(ref x)).Name/*cast due to constrained. prefix*/).ToString() into x
			select x.First());
		searcher = new LuminaSearcher<Item>(new global::_003C_003Ez__ReadOnlyList<Item>(list2), new Func<Item, string>[3]
		{
			(Item x) => ((object)((Item)(ref x)).Name/*cast due to constrained. prefix*/).ToString(),
			(Item x) => ((Item)(ref x)).RowId.ToString(),
			(Item x) => ((Item)(ref x)).LevelItem.RowId.ToString()
		});
		base.TaskHelper = new TaskHelper
		{
			TaskIntervalMS = 500
		};
		base.TaskHelper.Enqueue(delegate
		{
			RemoteUniversalisCatalog.GetDataCentersOrRequest();
			RemoteUniversalisCatalog.GetWorldsOrRequest();
			if (!RemoteUniversalisCatalog.TryGetDataCenters(out List<UniversalisDataCenter> dataCenters) || !RemoteUniversalisCatalog.TryGetWorlds(out List<UniversalisWorld> worlds))
			{
				return false;
			}
			Dictionary<string, Dictionary<string, Dictionary<uint, string>>> dictionary3 = (allWorlds = (from dc in dataCenters
				group dc by dc.Region).ToDictionary((IGrouping<string, UniversalisDataCenter> region) => region.Key, (IGrouping<string, UniversalisDataCenter> region) => region.ToDictionary((UniversalisDataCenter dc) => dc.Name, (UniversalisDataCenter dc) => dc.Worlds.ToDictionary((uint worldID) => worldID, (uint worldID) => worlds.FirstOrDefault((UniversalisWorld w) => w.ID == worldID)?.Name ?? string.Empty))));
			this.config.AllWorlds = dictionary3;
			this.config.Save(this);
			provider.AnchorRegion();
			if (provider.SelectedItemID != 0)
			{
				provider.SelectItem(provider.SelectedItemID, null, provider.HQOnly);
			}
			return true;
		});
		OmenServiceBase<CommandManager>.Instance().AddSubCommand("market", new CommandInfo(new HandlerDelegate(OnCommand))
		{
			HelpMessage = LanguageManager.Get("BetterMarketBoard-CommandHelp")
		});
		if (IsAbleToSearchMarket() && InfoProxy != null && ((InfoProxyItemSearch)InfoProxy).SearchItemId != 0 && LuminaGetter.TryGetRow<Item>(((InfoProxyItemSearch)InfoProxy).SearchItemId, out Item item2) && ((Item)(ref item2)).ItemSearchCategory.RowId != 0)
		{
			uint searchItemId = ((InfoProxyItemSearch)InfoProxy).SearchItemId;
			((InfoProxyItemSearch)InfoProxy).SearchItemId = 0u;
			provider.SelectItem(searchItemId);
		}
		OmenServiceBase<FrameworkManager>.Instance().Reg(new OnUpdateDelegate(OnMonitorUpdate), 60000u);
		OmenServiceBase<FrameworkManager>.Instance().Reg(new OnUpdateDelegate(OnWorldWatch), 1000u);
		OmenServiceBase<TooltipManager>.Instance().RegItem(OnItemTooltipUpdate);
		contextMenu = new SearchInMarketMenu(this);
		OmenServiceBase<ContextMenuManager>.Instance().Reg(contextMenu);
	}

	protected override void Uninit()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		OmenServiceBase<ContextMenuManager>.Instance().Unreg(contextMenu);
		OmenServiceBase<TooltipManager>.Instance().Unreg(OnItemTooltipUpdate);
		OmenServiceBase<FrameworkManager>.Instance().Unreg(OnMonitorUpdate, OnWorldWatch);
		OmenServiceBase<CommandManager>.Instance().RemoveSubCommand("market");
		itemIDToPayloadID.ForEach(delegate(KeyValuePair<uint, uint> x)
		{
			OmenServiceBase<LinkPayloadManager>.Instance().Unreg(x.Value);
		});
		itemIDToPayloadID.Clear();
		searchCategoryToItems.Clear();
		provider.ClearAllData();
		monitorProvider = null;
	}

	private void OnCommand(string command, string args)
	{
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		args = args.Trim();
		if (string.IsNullOrEmpty(args))
		{
			ToggleOverlay();
			return;
		}
		if (uint.TryParse(args, out var result) && LuminaGetter.TryGetRow<Item>(result, out Item _))
		{
			provider.SelectItem(result);
			((Window)base.Overlay).IsOpen = true;
			return;
		}
		Item val = (from x in searcher.Data
			where ((object)((Item)(ref x)).Name/*cast due to constrained. prefix*/).ToString().Contains(args, StringComparison.OrdinalIgnoreCase)
			orderby ((object)((Item)(ref x)).Name/*cast due to constrained. prefix*/).ToString().Length
			select x).FirstOrDefault();
		if (((Item)(ref val)).RowId != 0)
		{
			provider.SelectItem(((Item)(ref val)).RowId);
			ExecuteOverlaySearch(((object)((Item)(ref val)).Name/*cast due to constrained. prefix*/).ToString());
			((Window)base.Overlay).IsOpen = true;
		}
	}

	private void OnWorldWatch(IFramework framework)
	{
		if (!IsAbleToSearchMarket())
		{
			return;
		}
		uint currentWorld = GameState.CurrentWorld;
		if (currentWorld != lastWorldID)
		{
			lastWorldID = currentWorld;
			if (currentWorld != 0)
			{
				provider.ResyncAfterWorldChange();
			}
		}
		provider.RetryLocalSearchIfStale();
	}

	private void OnMonitorUpdate(IFramework framework)
	{
		if (IsAbleToSearchMarket())
		{
			monitorProvider.Update();
		}
	}

	private void ExecuteOverlaySearch(string searchInput)
	{
		itemSearchInput = searchInput;
		currentTab = ItemSelectorTab.Search;
		searcher.Search(searchInput);
	}

	private void ToggleOverlay(bool? isOpen = null)
	{
		provider.EnsureAnchored();
		if (!isOpen.HasValue)
		{
			Overlay? overlay = base.Overlay;
			((Window)overlay).IsOpen = !((Window)overlay).IsOpen;
		}
		else
		{
			((Window)base.Overlay).IsOpen = isOpen.Value;
		}
		((Window)base.Overlay).Collapsed = false;
	}

	[IPCProvider("DailyRoutines.Modules.BetterMarketBoard.SearchItem")]
	private bool SearchItemIPC(uint itemID)
	{
		if (itemID == 0)
		{
			return false;
		}
		provider.AnchorWorld();
		provider.SelectItem(itemID);
		return true;
	}

	[IPCProvider("DailyRoutines.Modules.BetterMarketBoard.ToggleOverlay")]
	private bool ToggleOverlayIPC(bool? isOpen)
	{
		if (base.Overlay == null)
		{
			return false;
		}
		ToggleOverlay(isOpen);
		return true;
	}

	private static (double VisualMin, double VisualMax) CalculateVisualPriceRange(ulong[] prices)
	{
		if (prices.Length == 0)
		{
			return (VisualMin: 0.0, VisualMax: 100.0);
		}
		Array.Sort(prices);
		double num = prices[0];
		double num2 = prices[^1];
		if (prices.Length < 4)
		{
			return (VisualMin: num, VisualMax: num2);
		}
		int num3 = (int)((double)prices.Length * 0.25);
		int num4 = (int)((double)prices.Length * 0.75);
		double num5 = prices[num3];
		double num6 = prices[num4];
		double num7 = num6 - num5;
		double num8 = prices[prices.Length / 2];
		double val = ((num7 > 0.0) ? (num6 + 3.0 * num7) : (num8 * 3.0));
		double item = Math.Min(num2, Math.Max(val, num8 * 1.5));
		return (VisualMin: Math.Max(0.0, num), VisualMax: item);
	}

	private void OnItemTooltipUpdate(ItemKind kind, uint itemID, ref List<TooltipItemModification> modifications)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Invalid comparison between Unknown and I4
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Invalid comparison between Unknown and I4
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Invalid comparison between Unknown and I4
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		if (!config.AppendMarketStatsTooltip)
		{
			return;
		}
		bool flag = (((int)kind == 500000 || (int)kind == 2000000) ? true : false);
		if (!flag && itemID != 0 && LuminaGetter.TryGetRow<Item>(itemID, out Item item) && ((Item)(ref item)).ItemSearchCategory.RowId != 0 && GameState.CurrentWorld != 0)
		{
			bool hqOnly = (int)kind == 1000000 && ((Item)(ref item)).CanBeHq;
			string currentRegionName = GetCurrentRegionName();
			string worldName = LuminaWrapper.GetWorldName(GameState.CurrentWorld);
			string text = (string.IsNullOrEmpty(worldName) ? GameState.CurrentWorld.ToString() : worldName);
			string[] array = ((!string.IsNullOrEmpty(currentRegionName)) ? new string[2] { text, currentRegionName } : new string[1] { text });
			foreach (string scope in array)
			{
				provider.RequestTooltipAggregatedScope(itemID, scope);
			}
			RemoteSnapshot<UniversalisAggregatedMarketDataResponse> orRequest = RemoteUniversalisAggregatedMarket.GetOrRequest(new global::_003C_003Ez__ReadOnlySingleElementList<uint>(itemID), text);
			RemoteSnapshot<UniversalisAggregatedMarketDataResponse> currentRegionSnapshot = (RemoteSnapshot<UniversalisAggregatedMarketDataResponse>)(string.IsNullOrEmpty(currentRegionName) ? ((ValueType)default(RemoteSnapshot<UniversalisAggregatedMarketDataResponse>)) : ((ValueType)RemoteUniversalisAggregatedMarket.GetOrRequest(new global::_003C_003Ez__ReadOnlySingleElementList<uint>(itemID), currentRegionName)));
			TooltipMarketStats tooltipMarketStats = CreateTooltipMarketStats(itemID, hqOnly, orRequest, currentRegionSnapshot);
			modifications.Add(new TooltipItemModification
			{
				Target = TooltipItemType.Description,
				Type = TooltipModificationType.Append,
				Text = BuildItemTooltipMarketText(tooltipMarketStats == null, tooltipMarketStats?.CurrentWorldMinPrice, tooltipMarketStats?.CurrentWorldTimeText, tooltipMarketStats?.RegionMinWorldName ?? string.Empty, tooltipMarketStats?.RegionMinPrice, tooltipMarketStats?.RegionTimeText, tooltipMarketStats?.DailySales ?? 0f)
			});
		}
	}

	private unsafe static ReadOnlySeString BuildItemTooltipMarketText(bool isRequesting, ulong? currentWorldMinPrice, string? currentWorldTimeText, string regionMinWorldName, ulong? regionMinPrice, string? regionTimeText, float dailySales)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		RentedSeStringBuilder val = default(RentedSeStringBuilder);
		((RentedSeStringBuilder)(ref val))._002Ector();
		try
		{
			SeStringBuilder builder = ((RentedSeStringBuilder)(ref val)).Builder;
			SeStringBuilder val2 = builder;
			SeStringBuilder obj = val2;
			SeStringInterpolatedStringHandler val3 = new SeStringInterpolatedStringHandler(2, 1, val2);
			((SeStringInterpolatedStringHandler)(ref val3)).AppendLiteral("[");
			((SeStringInterpolatedStringHandler)(ref val3)).AppendFormatted<string>(LuminaWrapper.GetAddonText(6556u));
			((SeStringInterpolatedStringHandler)(ref val3)).AppendLiteral("]");
			obj.Append(val3);
			if (isRequesting)
			{
				val2 = builder.AppendNewLine().PushColorType(32u);
				SeStringBuilder obj2 = val2;
				val3 = new SeStringInterpolatedStringHandler(6, 1, val2);
				((SeStringInterpolatedStringHandler)(ref val3)).AppendLiteral("    \ue031 ");
				((SeStringInterpolatedStringHandler)(ref val3)).AppendFormatted<string>(LuminaWrapper.GetAddonText(2717u));
				obj2.Append(val3).PopColorType();
				return builder.ToReadOnlySeString();
			}
			SeStringBuilder val4 = builder.AppendNewLine();
			val3 = new SeStringInterpolatedStringHandler(4, 1, val4);
			((SeStringInterpolatedStringHandler)(ref val3)).AppendLiteral("    ");
			((SeStringInterpolatedStringHandler)(ref val3)).AppendFormatted<string>(LanguageManager.Get("BetterMarketBoard-Tooltip-CurrentWorld"));
			val2 = val4.Append(val3).Append(" (").AppendIcon(88u);
			SeStringBuilder obj3 = val2;
			val3 = new SeStringInterpolatedStringHandler(0, 1, val2);
			World currentWorldData = GameState.CurrentWorldData;
			((SeStringInterpolatedStringHandler)(ref val3)).AppendFormatted(((World)(ref currentWorldData)).Name);
			obj3.Append(val3).Append(")");
			builder.Append(": ").PushColorType(32u).Append(FormatPrice(currentWorldMinPrice))
				.PopColorType();
			if (!string.IsNullOrEmpty(currentWorldTimeText))
			{
				val2 = builder;
				SeStringBuilder obj4 = val2;
				val3 = new SeStringInterpolatedStringHandler(3, 1, val2);
				((SeStringInterpolatedStringHandler)(ref val3)).AppendLiteral(" (");
				((SeStringInterpolatedStringHandler)(ref val3)).AppendFormatted<string>(currentWorldTimeText);
				((SeStringInterpolatedStringHandler)(ref val3)).AppendLiteral(")");
				obj4.Append(val3);
			}
			val2 = builder.AppendNewLine();
			SeStringBuilder obj5 = val2;
			val3 = new SeStringInterpolatedStringHandler(4, 1, val2);
			((SeStringInterpolatedStringHandler)(ref val3)).AppendLiteral("    ");
			((SeStringInterpolatedStringHandler)(ref val3)).AppendFormatted<string>(LanguageManager.Get("BetterMarketBoard-Tooltip-CurrentRegion"));
			obj5.Append(val3);
			if (!string.IsNullOrEmpty(regionMinWorldName))
			{
				builder.Append(" (").AppendIcon(88u).Append(regionMinWorldName)
					.Append(")");
			}
			builder.Append(": ").PushColorType(32u).Append(FormatPrice(regionMinPrice))
				.PopColorType();
			if (!string.IsNullOrEmpty(regionTimeText))
			{
				val2 = builder;
				SeStringBuilder obj6 = val2;
				val3 = new SeStringInterpolatedStringHandler(3, 1, val2);
				((SeStringInterpolatedStringHandler)(ref val3)).AppendLiteral(" (");
				((SeStringInterpolatedStringHandler)(ref val3)).AppendFormatted<string>(regionTimeText);
				((SeStringInterpolatedStringHandler)(ref val3)).AppendLiteral(")");
				obj6.Append(val3);
			}
			val2 = builder.AppendNewLine();
			SeStringBuilder obj7 = val2;
			val3 = new SeStringInterpolatedStringHandler(6, 1, val2);
			((SeStringInterpolatedStringHandler)(ref val3)).AppendLiteral("    ");
			((SeStringInterpolatedStringHandler)(ref val3)).AppendFormatted<string>(LanguageManager.Get("BetterMarketBoard-Tooltip-DailySales"));
			((SeStringInterpolatedStringHandler)(ref val3)).AppendLiteral(": ");
			obj7.Append(val3).PushColorType(45u).Append(FormatSales(dailySales))
				.PopColorType();
			return builder.ToReadOnlySeString();
		}
		finally
		{
			((IDisposable)(*(RentedSeStringBuilder*)(&val))/*cast due to constrained. prefix*/).Dispose();
		}
		static string FormatPrice(ulong? price)
		{
			if (!price.HasValue || price.GetValueOrDefault() == 0)
			{
				return "-";
			}
			return price.Value.ToChineseString() + "\ue049";
		}
		static string FormatSales(float sales)
		{
			if (!(sales > 0f))
			{
				return "-";
			}
			return ((int)MathF.Ceiling(sales)).ToChineseString();
		}
	}

	private string GetCurrentRegionName()
	{
		return allWorlds.FirstOrDefault<KeyValuePair<string, Dictionary<string, Dictionary<uint, string>>>>((KeyValuePair<string, Dictionary<string, Dictionary<uint, string>>> region) => region.Value.Values.Any((Dictionary<uint, string> dc) => dc.ContainsKey(GameState.CurrentWorld))).Key;
	}

	private static TooltipMarketStats? CreateTooltipMarketStats(uint itemID, bool hqOnly, RemoteSnapshot<UniversalisAggregatedMarketDataResponse> currentWorldSnapshot, RemoteSnapshot<UniversalisAggregatedMarketDataResponse> currentRegionSnapshot)
	{
		if (!TryGetAggregatedResult(currentWorldSnapshot, itemID, out UniversalisAggregatedMarketResult result))
		{
			return null;
		}
		if (!TryGetAggregatedResult(currentRegionSnapshot, itemID, out UniversalisAggregatedMarketResult result2))
		{
			return null;
		}
		UniversalisAggregatedMarketScope aggregatedMarketScope = GetAggregatedMarketScope(result, hqOnly);
		UniversalisAggregatedMarketScope aggregatedMarketScope2 = GetAggregatedMarketScope(result2, hqOnly);
		ulong? currentWorldMinPrice = ToPrice(aggregatedMarketScope.MinListing.World.Price);
		string currentWorldTimeText = ToTimeAgo(aggregatedMarketScope.RecentPurchase.World.Timestamp);
		UniversalisAggregatedMarketPricePoint region = aggregatedMarketScope2.MinListing.Region;
		ulong? regionMinPrice = ToPrice(region.Price);
		string regionMinWorldName = (((region.WorldID ?? 0) != 0) ? LuminaWrapper.GetWorldName(region.WorldID.Value) : string.Empty);
		string regionTimeText = ToTimeAgo(aggregatedMarketScope2.RecentPurchase.Region.Timestamp);
		float valueOrDefault = aggregatedMarketScope2.DailySaleVelocity.Region.Quantity.GetValueOrDefault();
		RemoteSnapshotStatus status = currentWorldSnapshot.Status;
		bool flag = (uint)(status - 2) <= 1u;
		bool flag2 = !flag;
		if (!flag2)
		{
			RemoteSnapshotStatus status2 = currentRegionSnapshot.Status;
			bool flag3 = (uint)(status2 - 2) <= 1u;
			flag2 = !flag3;
		}
		if (flag2)
		{
			return null;
		}
		return new TooltipMarketStats(currentWorldMinPrice, currentWorldTimeText, regionMinWorldName, regionMinPrice, regionTimeText, valueOrDefault);
		static ulong? ToPrice(double? price)
		{
			if (!price.HasValue || !(price.GetValueOrDefault() > 0.0))
			{
				return null;
			}
			return (ulong)Math.Round(price.Value);
		}
		static string? ToTimeAgo(long? timestamp)
		{
			if (!timestamp.HasValue || timestamp.GetValueOrDefault() <= 0)
			{
				return null;
			}
			return DateTimeOffset.FromUnixTimeMilliseconds(timestamp.Value).LocalDateTime.TimeAgo();
		}
	}

	private static UniversalisAggregatedMarketScope GetAggregatedMarketScope(UniversalisAggregatedMarketResult result, bool hqOnly)
	{
		if (!hqOnly)
		{
			return result.NQ;
		}
		return result.HQ;
	}

	private static bool TryGetAggregatedResult(RemoteSnapshot<UniversalisAggregatedMarketDataResponse> snapshot, uint itemID, out UniversalisAggregatedMarketResult? result)
	{
		result = null;
		if (!snapshot.HasValue || snapshot.Value == null)
		{
			return false;
		}
		result = snapshot.Value.Results.FirstOrDefault((UniversalisAggregatedMarketResult x) => x.ItemID == itemID);
		return result != null;
	}

	protected override void ConfigUI()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("Command")));
		IndentDisposable val = ImRaii.PushIndent(1, true);
		try
		{
			ImU8String val2 = new ImU8String(8, 2);
			((ImU8String)(ref val2)).AppendLiteral("/pdr ");
			((ImU8String)(ref val2)).AppendFormatted<string>("market");
			((ImU8String)(ref val2)).AppendLiteral(" → ");
			((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get("BetterMarketBoard-CommandHelp"));
			ImGui.TextUnformatted(val2);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		ImGui.NewLine();
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("BetterMarketBoard-Config-AppendMarketStatsTooltip")), ref config.AppendMarketStatsTooltip))
		{
			config.Save(this);
		}
		ImGuiOm.HelpMarker(LanguageManager.Get("BetterMarketBoard-Config-AppendMarketStatsTooltip-Help"), 40f, (FontAwesomeIcon)61530);
	}

	protected override void OverlayUI()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		((Window)base.Overlay).CollapsedCondition = (ImGuiCond)0;
		((Window)base.Overlay).Collapsed = null;
		SyncItemWithGame();
		MarketBoardUIContext frame = CreateFrameContext();
		TableDisposable val = ImRaii.Table(ImU8String.op_Implicit("Table"), 2, (ImGuiTableFlags)1);
		try
		{
			if (!val)
			{
				return;
			}
			ImGui.TableSetupColumn(ImU8String.op_Implicit("Left"), (ImGuiTableColumnFlags)8, 220f * Globals.GlobalUIScale, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("Right"), (ImGuiTableColumnFlags)4, 0f, 0u);
			ImGui.TableNextRow();
			ImGui.TableNextColumn();
			ChildDisposable val2 = ImRaii.Child(ImU8String.op_Implicit("###Left"), new Vector2(-1f), true, (ImGuiWindowFlags)152);
			try
			{
				if (ChildDisposable.op_Implicit(val2))
				{
					DrawLeftPanel(frame);
				}
			}
			finally
			{
				((ChildDisposable)(ref val2)).Dispose();
			}
			ImGui.TableNextColumn();
			ChildDisposable val3 = ImRaii.Child(ImU8String.op_Implicit("###Right"), new Vector2(-1f), true, (ImGuiWindowFlags)152);
			try
			{
				if (ChildDisposable.op_Implicit(val3))
				{
					DrawRightContent(frame);
				}
			}
			finally
			{
				((ChildDisposable)(ref val3)).Dispose();
			}
		}
		finally
		{
			((TableDisposable)(ref val)).Dispose();
		}
	}

	private MarketBoardUIContext CreateFrameContext()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		uint selectedItemID = provider.SelectedItemID;
		bool hasItem = false;
		Item itemData = default(Item);
		uint? npcGilPrice = null;
		if (selectedItemID != 0 && LuminaGetter.TryGetRow<Item>(selectedItemID, out Item item))
		{
			hasItem = true;
			itemData = item;
			npcGilPrice = provider.GetNPCGilPrice(selectedItemID);
		}
		return new MarketBoardUIContext(provider, this, selectedItemID, hasItem, itemData, npcGilPrice, GameState.CurrentWorld, provider.SelectedWorldID, provider.HQOnly, IsAbleToSearchLocalMarket());
	}

	private unsafe void SyncItemWithGame()
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		InfoProxyItemSearch* infoProxy = InfoProxy;
		uint num = ((infoProxy != null) ? ((InfoProxyItemSearch)infoProxy).SearchItemId : 0u);
		if (num != 0 && num != provider.SelectedItemID && Environment.TickCount64 - provider.LastSelectTime >= 500 && LuminaGetter.TryGetRow<Item>(num, out Item item) && ((Item)(ref item)).ItemSearchCategory.RowId != 0)
		{
			provider.SelectItem(num);
		}
	}

	private void DrawLeftPanel(MarketBoardUIContext frame)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		MonitorItem currentMonitorItem = config.CurrentMonitorItem;
		bool flag = currentMonitorItem != null && currentMonitorItem.ItemID != 0;
		float num = (flag ? (ImGui.GetTextLineHeight() * 3.8f) : 0f);
		float num2;
		if (!flag)
		{
			num2 = -1f;
		}
		else
		{
			float num3 = 0f - num;
			ImGuiStylePtr style = ImGui.GetStyle();
			num2 = num3 - ((ImGuiStylePtr)(ref style)).ItemSpacing.Y;
		}
		float y = num2;
		ChildDisposable val = ImRaii.Child(ImU8String.op_Implicit("###LeftMainContainer"), new Vector2(-1f, y), false, (ImGuiWindowFlags)136);
		try
		{
			if (ChildDisposable.op_Implicit(val))
			{
				DrawSidebarSegmentedControl();
				ImGui.Spacing();
				switch (currentTab)
				{
				case ItemSelectorTab.Search:
					DrawItemSelectorSearch(frame);
					break;
				case ItemSelectorTab.Favorite:
					DrawItemSelectorFavorite(frame);
					break;
				case ItemSelectorTab.History:
					DrawItemSelectorHistory(frame);
					break;
				default:
					currentTab = ItemSelectorTab.Search;
					DrawItemSelectorSearch(frame);
					break;
				}
			}
		}
		finally
		{
			((ChildDisposable)(ref val)).Dispose();
		}
		if (flag)
		{
			DrawSidebarActiveMonitor(num);
		}
	}

	private void DrawSidebarSegmentedControl()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		float x = ImGui.GetContentRegionAvail().X;
		float num = ImGui.GetTextLineHeight() * 1.15f + 6f * Globals.GlobalUIScale;
		float num2 = x / 3f;
		float num3 = 4f * Globals.GlobalUIScale;
		Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
		Vector2 vector = cursorScreenPos + new Vector2(x, num);
		ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
		((ImDrawListPtr)(ref windowDrawList)).AddRectFilled(cursorScreenPos, vector, ImGui.GetColorU32((ImGuiCol)7, 0.35f), num3);
		((ImDrawListPtr)(ref windowDrawList)).AddRect(cursorScreenPos, vector, ImGui.GetColorU32((ImGuiCol)5, 0.2f), num3, (ImDrawFlags)0, 1f * Globals.GlobalUIScale);
		ImU8String val = default(ImU8String);
		for (int i = 0; i < ItemSelectorTabs.Length; i++)
		{
			ItemSelectorTab itemSelectorTab = ItemSelectorTabs[i];
			bool flag = currentTab == itemSelectorTab;
			string text = itemSelectorTab switch
			{
				ItemSelectorTab.Favorite => FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61445), 
				ItemSelectorTab.History => FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61914), 
				_ => FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61442), 
			};
			string text2 = itemSelectorTab switch
			{
				ItemSelectorTab.Favorite => LanguageManager.Get("Favorite"), 
				ItemSelectorTab.History => LanguageManager.Get("History"), 
				_ => LanguageManager.Get("Search"), 
			};
			int num4 = ((itemSelectorTab == ItemSelectorTab.Favorite) ? config.FavoriteItems.Count : 0);
			Vector2 vector2 = cursorScreenPos + new Vector2((float)i * num2, 0f);
			Vector2 vector3 = vector2 + new Vector2(num2, num);
			ImGui.SetCursorScreenPos(vector2);
			((ImU8String)(ref val))._002Ector(16, 1);
			((ImU8String)(ref val)).AppendLiteral("###SegmentedTab_");
			((ImU8String)(ref val)).AppendFormatted<int>((int)itemSelectorTab);
			if (ImGui.InvisibleButton(val, new Vector2(num2, num), (ImGuiButtonFlags)0))
			{
				currentTab = itemSelectorTab;
				isConfirmClearHistory = false;
			}
			bool flag2 = ImGui.IsItemHovered();
			if (flag2)
			{
				ImGui.SetMouseCursor((ImGuiMouseCursor)7);
			}
			if (flag)
			{
				uint num5 = VectorExtensions.WithW(KnownColor.Chocolate.ToVector4(), flag2 ? 0.35f : 0.25f).ToUInt();
				uint num6 = VectorExtensions.WithW(KnownColor.Chocolate.ToVector4(), 0.85f).ToUInt();
				((ImDrawListPtr)(ref windowDrawList)).AddRectFilled(vector2, vector3, num5, num3);
				((ImDrawListPtr)(ref windowDrawList)).AddRect(vector2, vector3, num6, num3, (ImDrawFlags)0, 1f * Globals.GlobalUIScale);
			}
			else if (flag2)
			{
				uint colorU = ImGui.GetColorU32((ImGuiCol)8, 0.4f);
				((ImDrawListPtr)(ref windowDrawList)).AddRectFilled(vector2, vector3, colorU, num3);
			}
			string text3 = ((num4 > 0) ? $"{text} {text2} ({num4})" : (text + " " + text2));
			using (OmenServiceBase<FontManager>.Instance().UIFont80.Push())
			{
				Vector2 vector4 = ImGui.CalcTextSize(ImU8String.op_Implicit(text3), false, -1f);
				Vector2 vector5 = new Vector2(vector2.X + MathF.Max(2f, (num2 - vector4.X) / 2f), vector2.Y + (num - vector4.Y) / 2f);
				uint num7 = (flag ? KnownColor.Chocolate.ToUInt() : (flag2 ? ImGui.GetColorU32((ImGuiCol)0) : ImGui.GetColorU32((ImGuiCol)1)));
				((ImDrawListPtr)(ref windowDrawList)).PushClipRect(vector2, vector3, true);
				((ImDrawListPtr)(ref windowDrawList)).AddText(vector5, num7, ImU8String.op_Implicit(text3));
				((ImDrawListPtr)(ref windowDrawList)).PopClipRect();
			}
		}
		ImGui.SetCursorScreenPos(cursorScreenPos + new Vector2(0f, num));
	}

	private void DrawItemSelectorSearch(MarketBoardUIContext frame)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		float x = ImGui.GetContentRegionAvail().X;
		bool num = !string.IsNullOrEmpty(itemSearchInput);
		float frameHeight = ImGui.GetFrameHeight();
		if (num)
		{
			float num2 = x - frameHeight;
			ImGuiStylePtr style = ImGui.GetStyle();
			ImGui.SetNextItemWidth(num2 - ((ImGuiStylePtr)(ref style)).ItemSpacing.X);
		}
		else
		{
			ImGui.SetNextItemWidth(x);
		}
		if (ImGui.InputTextWithHint(ImU8String.op_Implicit("###ItemSearchInput"), ImU8String.op_Implicit(LanguageManager.Get("PleaseSearch")), ref itemSearchInput, 256, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null))
		{
			ExecuteOverlaySearch(itemSearchInput);
		}
		if (num)
		{
			ImGui.SameLine();
			if (ImGui.Button(ImU8String.op_Implicit("×###ClearSearchInput"), new Vector2(frameHeight, 0f)))
			{
				ExecuteOverlaySearch(string.Empty);
			}
			ImGuiOm.TooltipHover(LanguageManager.Get("Clear"));
		}
		ImGui.Spacing();
		ChildDisposable val = ImRaii.Child(ImU8String.op_Implicit("###SearchListContainer"), new Vector2(-1f, -1f), false, (ImGuiWindowFlags)128);
		try
		{
			if (!val)
			{
				return;
			}
			if (!string.IsNullOrWhiteSpace(itemSearchInput))
			{
				List<SearchCategoryGroup> searchGroups = provider.GetSearchGroups(itemSearchInput);
				if (searchGroups.Count == 0)
				{
					DrawEmptyState(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61442), LuminaWrapper.GetAddonText(2717u));
					return;
				}
				{
					ISharedImmediateTexture val4 = default(ISharedImmediateTexture);
					foreach (SearchCategoryGroup item in searchGroups)
					{
						item.Deconstruct(out ItemSearchCategory Category, out List<Item> Items);
						ItemSearchCategory val2 = Category;
						List<Item> list = Items;
						uint rowId = ((ItemSearchCategory)(ref val2)).RowId;
						string value = ((object)((ItemSearchCategory)(ref val2)).Name/*cast due to constrained. prefix*/).ToString();
						ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
						GameIconLookup val3 = new GameIconLookup((uint)((ItemSearchCategory)(ref val2)).Icon, false, true, (ClientLanguage?)null);
						if (!obj.TryGetFromGameIcon(ref val3, ref val4) || !ImGuiOm.TreeNodeImageWithText(val4.GetWrapOrEmpty().Handle, new Vector2(ImGui.GetTextLineHeight()), $"{value}##{rowId}-Search", (ImGuiTreeNodeFlags)32))
						{
							continue;
						}
						ImGuiListClipper val5 = default(ImGuiListClipper);
						((ImGuiListClipper)(ref val5)).Begin(list.Count);
						while (((ImGuiListClipper)(ref val5)).Step())
						{
							for (int i = val5.DisplayStart; i < val5.DisplayEnd; i++)
							{
								RenderItemCard(frame, list[i]);
							}
						}
						ImGui.TreePop();
					}
					return;
				}
			}
			ISharedImmediateTexture val6 = default(ISharedImmediateTexture);
			foreach (ItemSearchCategory validCategory in ValidCategories)
			{
				ItemSearchCategory current = validCategory;
				string value2 = ((object)((ItemSearchCategory)(ref current)).Name/*cast due to constrained. prefix*/).ToString();
				if (string.IsNullOrEmpty(value2) || !searchCategoryToItems.TryGetValue(((ItemSearchCategory)(ref current)).RowId, out List<Item> value3) || value3.Count == 0)
				{
					continue;
				}
				ITextureProvider obj2 = DalamudServiceExtension.Instance<ITextureProvider>();
				GameIconLookup val3 = new GameIconLookup((uint)((ItemSearchCategory)(ref current)).Icon, false, true, (ClientLanguage?)null);
				if (!obj2.TryGetFromGameIcon(ref val3, ref val6) || !ImGuiOm.TreeNodeImageWithText(val6.GetWrapOrEmpty().Handle, new Vector2(ImGui.GetTextLineHeight()), $"{value2}##{((ItemSearchCategory)(ref current)).RowId}-Default", (ImGuiTreeNodeFlags)0))
				{
					continue;
				}
				ImGuiListClipper val7 = default(ImGuiListClipper);
				((ImGuiListClipper)(ref val7)).Begin(value3.Count);
				while (((ImGuiListClipper)(ref val7)).Step())
				{
					for (int j = val7.DisplayStart; j < val7.DisplayEnd; j++)
					{
						RenderItemCard(frame, value3[j]);
					}
				}
				ImGui.TreePop();
			}
		}
		finally
		{
			((ChildDisposable)(ref val)).Dispose();
		}
	}

	private void DrawItemSelectorFavorite(MarketBoardUIContext frame)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		ChildDisposable val = ImRaii.Child(ImU8String.op_Implicit("###FavoriteContainer"), new Vector2(-1f, -1f), false, (ImGuiWindowFlags)128);
		try
		{
			if (!val)
			{
				return;
			}
			if (config.FavoriteItems.Count == 0)
			{
				DrawEmptyState(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61445), LuminaWrapper.GetAddonText(2717u));
				return;
			}
			List<MarketFavoriteItem> list = favoriteItemsCache;
			if (list == null || favoriteItemsCacheVersion != favoriteItemsVersion)
			{
				list = (favoriteItemsCache = config.FavoriteItems.Values.ToList());
				favoriteItemsCacheVersion = favoriteItemsVersion;
			}
			ImGuiListClipper val2 = default(ImGuiListClipper);
			((ImGuiListClipper)(ref val2)).Begin(list.Count);
			while (((ImGuiListClipper)(ref val2)).Step())
			{
				for (int i = val2.DisplayStart; i < val2.DisplayEnd; i++)
				{
					MarketFavoriteItem marketFavoriteItem = list[i];
					Item data = marketFavoriteItem.GetData();
					if (((Item)(ref data)).RowId != 0)
					{
						Item item = data;
						string note = marketFavoriteItem.Note;
						RenderItemCard(frame, item, null, note);
					}
				}
			}
		}
		finally
		{
			((ChildDisposable)(ref val)).Dispose();
		}
	}

	private void DrawItemSelectorHistory(MarketBoardUIContext frame)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		float x = ImGui.GetContentRegionAvail().X;
		bool flag = config.HistoryItems.Count > 0;
		float num = ImGui.GetFrameHeight() * 1.5f;
		float num2 = x - num;
		ImGuiStylePtr style = ImGui.GetStyle();
		ImGui.SetNextItemWidth(num2 - ((ImGuiStylePtr)(ref style)).ItemSpacing.X);
		ImGui.InputTextWithHint(ImU8String.op_Implicit("###HistorySearchInput"), ImU8String.op_Implicit(LanguageManager.Get("Search")), ref historySearchInput, 128, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
		ImGui.SameLine();
		DisabledDisposable val = ImRaii.Disabled(!flag);
		try
		{
			Vector4 vector = (isConfirmClearHistory ? KnownColor.OrangeRed.ToVector4() : KnownColor.Chocolate.ToVector4());
			string text = (isConfirmClearHistory ? FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61452) : FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61944));
			ColorDisposable val2 = ImRaii.PushColor((ImGuiCol)0, vector, isConfirmClearHistory);
			try
			{
				ImU8String val3 = new ImU8String(21, 1);
				((ImU8String)(ref val3)).AppendFormatted<string>(text);
				((ImU8String)(ref val3)).AppendLiteral("###ClearHistoryButton");
				if (ImGui.Button(val3, new Vector2(num, 0f)))
				{
					if (isConfirmClearHistory)
					{
						config.HistoryItems.Clear();
						config.Save(this);
						isConfirmClearHistory = false;
					}
					else
					{
						isConfirmClearHistory = true;
					}
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
			if (isConfirmClearHistory && ImGui.IsMouseClicked((ImGuiMouseButton)0) && !ImGui.IsItemHovered())
			{
				isConfirmClearHistory = false;
			}
			ImGuiOm.TooltipHover(isConfirmClearHistory ? LanguageManager.Get("Confirm") : LanguageManager.Get("Clear"));
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		ImGui.Spacing();
		ChildDisposable val4 = ImRaii.Child(ImU8String.op_Implicit("###HistoryContainer"), new Vector2(-1f, -1f), false, (ImGuiWindowFlags)128);
		try
		{
			if (!val4)
			{
				return;
			}
			if (config.HistoryItems.Count == 0)
			{
				DrawEmptyState(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61914), LuminaWrapper.GetAddonText(2717u));
				return;
			}
			IEnumerable<KeyValuePair<uint, MarketHistoryItem>> source = config.HistoryItems;
			if (!string.IsNullOrWhiteSpace(historySearchInput))
			{
				source = source.Where(delegate(KeyValuePair<uint, MarketHistoryItem> kvp)
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_000c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0019: Unknown result type (might be due to invalid IL or missing references)
					//IL_001e: Unknown result type (might be due to invalid IL or missing references)
					Item data = kvp.Value.GetData();
					return ((Item)(ref data)).RowId != 0 && ((object)((Item)(ref data)).Name/*cast due to constrained. prefix*/).ToString().Contains(historySearchInput, StringComparison.OrdinalIgnoreCase);
				});
			}
			List<KeyValuePair<uint, MarketHistoryItem>> list = source.ToList();
			if (list.Count == 0)
			{
				DrawEmptyState(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61442), LuminaWrapper.GetAddonText(2717u));
				return;
			}
			foreach (IGrouping<DateTime, KeyValuePair<uint, MarketHistoryItem>> item in from kvp in list
				group kvp by kvp.Value.AccessTime.Date into g
				orderby g.Key descending
				select g)
			{
				DateTime key = item.Key;
				ImU8String val5 = new ImU8String(5, 3);
				((ImU8String)(ref val5)).AppendFormatted<DateTime>(key, "MM/dd");
				((ImU8String)(ref val5)).AppendLiteral(" (");
				((ImU8String)(ref val5)).AppendFormatted<int>(item.Count());
				((ImU8String)(ref val5)).AppendLiteral(")##");
				((ImU8String)(ref val5)).AppendFormatted<DateTime>(key, "yyyyMMdd");
				if (!ImGui.TreeNodeEx(val5, (ImGuiTreeNodeFlags)2080, default(ImU8String)))
				{
					continue;
				}
				List<KeyValuePair<uint, MarketHistoryItem>> list2 = item.OrderByDescending((KeyValuePair<uint, MarketHistoryItem> keyValuePair2) => keyValuePair2.Value.AccessTime).ToList();
				ImGuiListClipper val6 = default(ImGuiListClipper);
				((ImGuiListClipper)(ref val6)).Begin(list2.Count);
				while (((ImGuiListClipper)(ref val6)).Step())
				{
					for (int num3 = val6.DisplayStart; num3 < val6.DisplayEnd; num3++)
					{
						KeyValuePair<uint, MarketHistoryItem> keyValuePair = list2[num3];
						RenderItemCard(frame, keyValuePair.Value.GetData(), keyValuePair.Value.AccessTime);
					}
				}
				ImGui.TreePop();
			}
		}
		finally
		{
			((ChildDisposable)(ref val4)).Dispose();
		}
	}

	private unsafe void RenderItemCard(MarketBoardUIContext frame, Item item, DateTime? accessTime = null, string? note = null)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_054d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0552: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_052c: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_061b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0620: Unknown result type (might be due to invalid IL or missing references)
		//IL_0637: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06da: Unknown result type (might be due to invalid IL or missing references)
		//IL_06de: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0703: Unknown result type (might be due to invalid IL or missing references)
		//IL_0708: Unknown result type (might be due to invalid IL or missing references)
		//IL_0719: Unknown result type (might be due to invalid IL or missing references)
		//IL_081a: Unknown result type (might be due to invalid IL or missing references)
		//IL_081f: Unknown result type (might be due to invalid IL or missing references)
		//IL_083b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0771: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_092d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0988: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a01: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0afc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b1d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b22: Unknown result type (might be due to invalid IL or missing references)
		bool flag = provider.SelectedItemID == ((Item)(ref item)).RowId && provider.HQOnly;
		ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
		GameIconLookup val = new GameIconLookup((uint)((Item)(ref item)).Icon, flag, true, (ClientLanguage?)null);
		ISharedImmediateTexture val2 = default(ISharedImmediateTexture);
		if (!obj.TryGetFromGameIcon(ref val, ref val2))
		{
			return;
		}
		ImU8String val3 = default(ImU8String);
		((ImU8String)(ref val3))._002Ector(1, 2);
		((ImU8String)(ref val3)).AppendFormatted<uint>(((Item)(ref item)).RowId);
		((ImU8String)(ref val3)).AppendLiteral("_");
		((ImU8String)(ref val3)).AppendFormatted<ItemSelectorTab>(currentTab);
		IdDisposable val4 = ImRaii.PushId(val3, true);
		try
		{
			bool flag2 = provider.SelectedItemID == ((Item)(ref item)).RowId;
			bool flag3 = config.FavoriteItems.ContainsKey(((Item)(ref item)).RowId);
			bool flag4 = config.CurrentMonitorItem?.ItemID == ((Item)(ref item)).RowId;
			float x = ImGui.GetContentRegionAvail().X;
			float num = ImGui.GetTextLineHeight() * 1.6f + 6f * Globals.GlobalUIScale;
			float num2 = 18f * Globals.GlobalUIScale;
			float num3 = 4f * Globals.GlobalUIScale;
			float num4 = x - num2 - 4f * Globals.GlobalUIScale;
			Vector2 vector = new Vector2(x, num);
			float num5 = 4f * Globals.GlobalUIScale;
			Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
			Vector2 vector2 = cursorScreenPos + vector;
			ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
			ImGui.SetCursorScreenPos(cursorScreenPos);
			ImU8String val5 = new ImU8String(14, 1);
			((ImU8String)(ref val5)).AppendLiteral("###CardButton_");
			((ImU8String)(ref val5)).AppendFormatted<uint>(((Item)(ref item)).RowId);
			ImGui.InvisibleButton(val5, new Vector2(num4, num), (ImGuiButtonFlags)0);
			bool flag5 = ImGui.IsItemHovered();
			if (flag5)
			{
				ImGui.SetMouseCursor((ImGuiMouseCursor)7);
			}
			if (flag5)
			{
				(*AtkStage.Instance()).ShowItemTooltip(((AtkUnitBase)Addons.ScreenText).RootNode, ((Item)(ref item)).RowId);
				isItemListTooltip = true;
			}
			else if (isItemListTooltip)
			{
				isItemListTooltip = false;
				(*AtkStage.Instance()).HideTooltip(((AtkUnitBase)Addons.ScreenText).Id);
			}
			if (flag5)
			{
				string text = ((object)((Item)(ref item)).Name/*cast due to constrained. prefix*/).ToString();
				if (((Item)(ref item)).CanBeHq)
				{
					text += (flag ? $"\n（{LanguageManager.Get("RightClick")}：{LanguageManager.Get("All")}）" : $"\n（{LanguageManager.Get("RightClick")}：{LanguageManager.Get("BetterMarketBoard-MarketView-HQ")}）");
				}
				ImGuiOm.TooltipHover(text);
			}
			if (ImGui.IsItemClicked((ImGuiMouseButton)0))
			{
				provider.SelectItem(((Item)(ref item)).RowId, null, flag);
			}
			if (((Item)(ref item)).CanBeHq && ImGui.IsItemClicked((ImGuiMouseButton)1))
			{
				provider.SelectItem(((Item)(ref item)).RowId, null, !flag);
			}
			Vector4 color;
			Vector4 color2;
			if (flag2)
			{
				color = VectorExtensions.WithW(KnownColor.Chocolate.ToVector4(), flag5 ? 0.3f : 0.2f);
				color2 = VectorExtensions.WithW(KnownColor.Chocolate.ToVector4(), 0.85f);
			}
			else if (flag5)
			{
				color = ImGui.GetColorU32((ImGuiCol)8, 0.4f).ToVector4();
				color2 = ImGui.GetColorU32((ImGuiCol)5, 0.35f).ToVector4();
			}
			else
			{
				color = ImGui.GetColorU32((ImGuiCol)7, 0.22f).ToVector4();
				color2 = ImGui.GetColorU32((ImGuiCol)5, 0.12f).ToVector4();
			}
			((ImDrawListPtr)(ref windowDrawList)).AddRectFilled(cursorScreenPos, vector2, color.ToUInt(), num5);
			((ImDrawListPtr)(ref windowDrawList)).AddRect(cursorScreenPos, vector2, color2.ToUInt(), num5, (ImDrawFlags)0, 1f * Globals.GlobalUIScale);
			float num6 = num - 6f * Globals.GlobalUIScale;
			Vector2 vector3 = new Vector2(cursorScreenPos.X + num3, cursorScreenPos.Y + 3f * Globals.GlobalUIScale);
			((ImDrawListPtr)(ref windowDrawList)).AddImage(val2.GetWrapOrEmpty().Handle, vector3, vector3 + new Vector2(num6));
			float num7 = vector3.X + num6 + 6f * Globals.GlobalUIScale;
			float num8 = cursorScreenPos.X + num4 - num3;
			float num9 = 0f;
			if (accessTime.HasValue)
			{
				string text2 = $"{accessTime.Value:HH:mm}";
				using (OmenServiceBase<FontManager>.Instance().UIFont60.Push())
				{
					Vector2 vector4 = ImGui.CalcTextSize(ImU8String.op_Implicit(text2), false, -1f);
					num9 = vector4.X + 4f * Globals.GlobalUIScale;
					Vector2 vector5 = new Vector2(num8 - vector4.X, cursorScreenPos.Y + 4f * Globals.GlobalUIScale);
					((ImDrawListPtr)(ref windowDrawList)).AddText(vector5, ImGui.GetColorU32((ImGuiCol)1), ImU8String.op_Implicit(text2));
				}
			}
			float num10 = num8 - num9;
			string text3 = ((object)((Item)(ref item)).Name/*cast due to constrained. prefix*/).ToString();
			if (flag)
			{
				text3 += " \ue03c";
			}
			Vector2 vector6 = new Vector2(num7, cursorScreenPos.Y + 3f * Globals.GlobalUIScale);
			Vector2 vector7 = cursorScreenPos;
			Vector2 vector8 = vector2;
			vector8.X = num10;
			((ImDrawListPtr)(ref windowDrawList)).PushClipRect(vector7, vector8, true);
			((ImDrawListPtr)(ref windowDrawList)).AddText(vector6, flag2 ? KnownColor.Chocolate.ToUInt() : ImGui.GetColorU32((ImGuiCol)0), ImU8String.op_Implicit(text3));
			((ImDrawListPtr)(ref windowDrawList)).PopClipRect();
			float y = cursorScreenPos.Y + ImGui.GetTextLineHeight() + 2f * Globals.GlobalUIScale;
			float x2 = num7;
			using (OmenServiceBase<FontManager>.Instance().UIFont60.Push())
			{
				string text4 = $"\ue033 {((Item)(ref item)).LevelItem.RowId}";
				x2 = DrawItemCardBadge(windowDrawList, text4, x2, y, VectorExtensions.WithW(KnownColor.Chocolate.ToVector4(), 0.15f).ToUInt(), KnownColor.Chocolate.ToUInt());
				if (((Item)(ref item)).CanBeHq)
				{
					uint backgroundColor = (flag ? VectorExtensions.WithW(KnownColor.Chocolate.ToVector4(), 0.25f).ToUInt() : ImGui.GetColorU32((ImGuiCol)7, 0.4f));
					uint textColor = (flag ? KnownColor.Chocolate.ToUInt() : KnownColor.Gray.ToUInt());
					x2 = DrawItemCardBadge(windowDrawList, "\ue03c", x2, y, backgroundColor, textColor);
				}
				ItemSearchCategory value = ((Item)(ref item)).ItemSearchCategory.Value;
				if (((ItemSearchCategory)(ref value)).RowId != 0)
				{
					value = ((Item)(ref item)).ItemSearchCategory.Value;
					string text5 = ((object)((ItemSearchCategory)(ref value)).Name/*cast due to constrained. prefix*/).ToString();
					x2 = DrawItemCardBadge(windowDrawList, text5, x2, y, ImGui.GetColorU32((ImGuiCol)7, 0.4f), ImGui.GetColorU32((ImGuiCol)1), num10, requireFit: true);
				}
				if (!string.IsNullOrEmpty(note))
				{
					string text6 = "✎ " + note;
					if (x2 < num10)
					{
						((ImDrawListPtr)(ref windowDrawList)).PushClipRect(cursorScreenPos, new Vector2(num10, vector2.Y), true);
						DrawItemCardBadge(windowDrawList, text6, x2, y, VectorExtensions.WithW(KnownColor.DarkOrange.ToVector4(), 0.18f).ToUInt(), KnownColor.Orange.ToUInt());
						((ImDrawListPtr)(ref windowDrawList)).PopClipRect();
					}
				}
			}
			float x3 = cursorScreenPos.X + num4 + 2f * Globals.GlobalUIScale;
			float num11 = num / 2f;
			Vector2 cursorScreenPos2 = new Vector2(x3, cursorScreenPos.Y + 1f * Globals.GlobalUIScale);
			Vector2 vector9 = new Vector2(num2, num11 - 2f * Globals.GlobalUIScale);
			ImGui.SetCursorScreenPos(cursorScreenPos2);
			ImU8String val6 = new ImU8String(10, 1);
			((ImU8String)(ref val6)).AppendLiteral("###FavBtn_");
			((ImU8String)(ref val6)).AppendFormatted<uint>(((Item)(ref item)).RowId);
			if (ImGui.InvisibleButton(val6, vector9, (ImGuiButtonFlags)0))
			{
				if (flag3)
				{
					config.FavoriteItems.Remove(((Item)(ref item)).RowId);
				}
				else
				{
					config.FavoriteItems[((Item)(ref item)).RowId] = new MarketFavoriteItem
					{
						ItemID = ((Item)(ref item)).RowId,
						Note = string.Empty
					};
				}
				favoriteItemsVersion++;
				config.Save(this);
			}
			bool flag6 = ImGui.IsItemHovered();
			if (flag6)
			{
				ImGui.SetMouseCursor((ImGuiMouseCursor)7);
				ImGuiOm.TooltipHover(flag3 ? LanguageManager.Get("Unfavorite") : LanguageManager.Get("Favorite"));
			}
			string text7 = FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61445);
			uint num12 = (flag3 ? KnownColor.Goldenrod.ToUInt() : (flag6 ? ImGui.GetColorU32((ImGuiCol)0) : ImGui.GetColorU32((ImGuiCol)1, 0.4f)));
			using (OmenServiceBase<FontManager>.Instance().UIFont60.Push())
			{
				Vector2 vector10 = ImGui.CalcTextSize(ImU8String.op_Implicit(text7), false, -1f);
				Vector2 vector11 = new Vector2(cursorScreenPos2.X + (vector9.X - vector10.X) / 2f, cursorScreenPos2.Y + (vector9.Y - vector10.Y) / 2f);
				((ImDrawListPtr)(ref windowDrawList)).AddText(vector11, num12, ImU8String.op_Implicit(text7));
			}
			Vector2 cursorScreenPos3 = new Vector2(x3, cursorScreenPos.Y + num11 + 1f * Globals.GlobalUIScale);
			Vector2 vector12 = new Vector2(num2, num11 - 2f * Globals.GlobalUIScale);
			ImGui.SetCursorScreenPos(cursorScreenPos3);
			ImU8String val7 = new ImU8String(10, 1);
			((ImU8String)(ref val7)).AppendLiteral("###MonBtn_");
			((ImU8String)(ref val7)).AppendFormatted<uint>(((Item)(ref item)).RowId);
			if (ImGui.InvisibleButton(val7, vector12, (ImGuiButtonFlags)0))
			{
				if (flag4)
				{
					monitorProvider.ClearMonitor();
				}
				else
				{
					monitorProvider.SetMonitor(((Item)(ref item)).RowId, flag);
				}
			}
			bool flag7 = ImGui.IsItemHovered();
			if (flag7)
			{
				ImGui.SetMouseCursor((ImGuiMouseCursor)7);
				ImGuiOm.TooltipHover(flag4 ? "BetterMarketBoard-PriceMonitor-ToNotMonitor" : "BetterMarketBoard-PriceMonitor-ToMonitor");
			}
			string text8 = FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61683);
			uint num13 = (flag4 ? KnownColor.GreenYellow.ToUInt() : (flag7 ? ImGui.GetColorU32((ImGuiCol)0) : ImGui.GetColorU32((ImGuiCol)1, 0.4f)));
			using (OmenServiceBase<FontManager>.Instance().UIFont60.Push())
			{
				Vector2 vector13 = ImGui.CalcTextSize(ImU8String.op_Implicit(text8), false, -1f);
				Vector2 vector14 = new Vector2(cursorScreenPos3.X + (vector12.X - vector13.X) / 2f, cursorScreenPos3.Y + (vector12.Y - vector13.Y) / 2f);
				((ImDrawListPtr)(ref windowDrawList)).AddText(vector14, num13, ImU8String.op_Implicit(text8));
			}
			Vector2 vector15 = cursorScreenPos;
			ImGuiStylePtr style = ImGui.GetStyle();
			ImGui.SetCursorScreenPos(vector15 + new Vector2(0f, num + ((ImGuiStylePtr)(ref style)).ItemSpacing.Y));
		}
		finally
		{
			((IDisposable)val4)?.Dispose();
		}
	}

	private static float DrawItemCardBadge(ImDrawListPtr drawList, string text, float x, float y, uint backgroundColor, uint textColor, float maxX = float.PositiveInfinity, bool requireFit = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		Vector2 vector = ImGui.CalcTextSize(ImU8String.op_Implicit(text), false, -1f);
		float num = 3f * Globals.GlobalUIScale;
		Vector2 vector2 = new Vector2(x, y);
		Vector2 vector3 = new Vector2(x + vector.X + num * 2f, y + vector.Y + 2f * Globals.GlobalUIScale);
		if (requireFit ? (vector3.X >= maxX) : (vector2.X >= maxX))
		{
			return x;
		}
		((ImDrawListPtr)(ref drawList)).AddRectFilled(vector2, vector3, backgroundColor, 2f * Globals.GlobalUIScale);
		((ImDrawListPtr)(ref drawList)).AddText(new Vector2(vector2.X + num, vector2.Y + 1f * Globals.GlobalUIScale), textColor, ImU8String.op_Implicit(text));
		return vector3.X + 4f * Globals.GlobalUIScale;
	}

	private void DrawSidebarActiveMonitor(float cardHeight)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0529: Unknown result type (might be due to invalid IL or missing references)
		//IL_058e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0618: Unknown result type (might be due to invalid IL or missing references)
		//IL_061d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0783: Unknown result type (might be due to invalid IL or missing references)
		//IL_0879: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0928: Unknown result type (might be due to invalid IL or missing references)
		//IL_095d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0962: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_098a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a91: Unknown result type (might be due to invalid IL or missing references)
		MonitorItem currentMonitorItem = config.CurrentMonitorItem;
		if (currentMonitorItem == null)
		{
			return;
		}
		Item data = currentMonitorItem.GetData();
		if (((Item)(ref data)).RowId == 0 || ((Item)(ref data)).ItemSearchCategory.RowId == 0)
		{
			monitorProvider.ClearMonitor();
			return;
		}
		float x = ImGui.GetContentRegionAvail().X;
		Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
		Vector2 vector = cursorScreenPos + new Vector2(x, cardHeight);
		ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
		float num = 4f * Globals.GlobalUIScale;
		float num2 = 6f * Globals.GlobalUIScale;
		float num3 = 5f * Globals.GlobalUIScale;
		Vector2 vector2 = new Vector2(16f * Globals.GlobalUIScale, 16f * Globals.GlobalUIScale);
		Vector2 cursorScreenPos2 = new Vector2(vector.X - num2 - vector2.X, cursorScreenPos.Y + num3);
		ImGui.SetCursorScreenPos(cursorScreenPos2);
		if (ImGui.InvisibleButton(ImU8String.op_Implicit("###ClearMonitorItemButton"), vector2, (ImGuiButtonFlags)0))
		{
			monitorProvider.ClearMonitor();
		}
		bool flag = ImGui.IsItemHovered();
		if (flag)
		{
			ImGui.SetMouseCursor((ImGuiMouseCursor)7);
			ImGuiOm.TooltipHover(LanguageManager.Get("BetterMarketBoard-PriceMonitor-ToNotMonitor"));
		}
		string text = LanguageManager.Get("BetterMarketBoard-PriceMonitor-AutoPurchase");
		Vector2 vector3;
		using (OmenServiceBase<FontManager>.Instance().UIFont80.Push())
		{
			vector3 = ImGui.CalcTextSize(ImU8String.op_Implicit(text), false, -1f);
		}
		float num4 = 4f * Globals.GlobalUIScale;
		float num5 = 1.5f * Globals.GlobalUIScale;
		float num6 = vector3.X + num4 * 2f;
		float num7 = MathF.Max(vector2.Y, vector3.Y + num5 * 2f);
		Vector2 vector4 = new Vector2(cursorScreenPos2.X - 5f * Globals.GlobalUIScale, cursorScreenPos.Y + num3 + num7);
		Vector2 vector5 = new Vector2(vector4.X - num6, cursorScreenPos.Y + num3);
		ImGui.SetCursorScreenPos(vector5);
		ImGui.InvisibleButton(ImU8String.op_Implicit("###AutoBuyBadgeButton"), vector4 - vector5, (ImGuiButtonFlags)0);
		bool flag2 = ImGui.IsItemHovered();
		if (flag2)
		{
			ImGui.SetMouseCursor((ImGuiMouseCursor)7);
			string text2 = LanguageManager.Get("BetterMarketBoard-PriceMonitor-AutoPurchase");
			text2 = (config.AutoBuy ? (text2 + $"（{LanguageManager.Get("Enabled")}）\n{LanguageManager.Get("LeftClick")}：{LanguageManager.Get("Disable")}") : (text2 + $"（{LanguageManager.Get("Disabled")}）\n{LanguageManager.Get("LeftClick")}：{LanguageManager.Get("Enable")}"));
			text2 = text2 + "\n" + LanguageManager.Get("RightClick") + "：" + LanguageManager.Get("Settings");
			ImGuiOm.TooltipHover(text2);
		}
		if (ImGui.IsItemClicked((ImGuiMouseButton)0))
		{
			monitorProvider.ToggleAutoBuy();
		}
		if (ImGui.IsItemClicked((ImGuiMouseButton)1))
		{
			ImGui.OpenPopup(ImU8String.op_Implicit("###MonitorAutoBuySettingsPopup"), (ImGuiPopupFlags)0);
		}
		PopupDisposable val = ImRaii.Popup(ImU8String.op_Implicit("###MonitorAutoBuySettingsPopup"), (ImGuiWindowFlags)64);
		try
		{
			if (PopupDisposable.op_Implicit(val))
			{
				DrawMonitorAutoPurchaseSettings(currentMonitorItem);
			}
		}
		finally
		{
			((PopupDisposable)(ref val)).Dispose();
		}
		float num8 = MathF.Max(vector2.Y, num7);
		float num9 = cursorScreenPos.Y + num3 + num8 + 5f * Globals.GlobalUIScale;
		float num10 = 36f * Globals.GlobalUIScale;
		string text3 = "≤ " + currentMonitorItem.PriceThreshold.ToChineseString() + "\ue049";
		Vector2 vector6;
		using (OmenServiceBase<FontManager>.Instance().UIFont80.Push())
		{
			vector6 = ImGui.CalcTextSize(ImU8String.op_Implicit(text3), false, -1f);
		}
		float num11 = num9 + ImGui.GetTextLineHeight() + 2f * Globals.GlobalUIScale;
		Vector2 vector7 = new Vector2(vector.X - num2 - vector6.X - 3f * Globals.GlobalUIScale, num11 - 1.5f * Globals.GlobalUIScale);
		Vector2 vector8 = new Vector2(vector6.X + 6f * Globals.GlobalUIScale, vector6.Y + 3f * Globals.GlobalUIScale);
		ImGui.SetCursorScreenPos(vector7);
		if (ImGui.InvisibleButton(ImU8String.op_Implicit("###MonitorPriceThresholdButton"), vector8, (ImGuiButtonFlags)0))
		{
			ImGui.OpenPopup(ImU8String.op_Implicit("###MonitorPriceSettingsPopup"), (ImGuiPopupFlags)0);
		}
		bool flag3 = ImGui.IsItemHovered();
		if (flag3)
		{
			ImGui.SetMouseCursor((ImGuiMouseCursor)7);
			ImGuiOm.TooltipHover(LanguageManager.Get("BetterMarketBoard-PriceMonitor-Price"));
		}
		DrawMonitorPriceSettingsPopup(currentMonitorItem);
		float x2 = vector5.X - cursorScreenPos.X - 2f * Globals.GlobalUIScale;
		ImGui.SetCursorScreenPos(cursorScreenPos);
		bool num12 = ImGui.InvisibleButton(ImU8String.op_Implicit("###ActiveMonitorTopSelectButton"), new Vector2(x2, num8 + num3), (ImGuiButtonFlags)0);
		bool flag4 = ImGui.IsItemHovered();
		bool flag5 = ImGui.IsItemClicked((ImGuiMouseButton)1);
		float x3 = vector.X - num2 - vector6.X - 6f * Globals.GlobalUIScale - cursorScreenPos.X;
		Vector2 cursorScreenPos3 = cursorScreenPos;
		cursorScreenPos3.Y = num9;
		ImGui.SetCursorScreenPos(cursorScreenPos3);
		bool flag6 = ImGui.InvisibleButton(ImU8String.op_Implicit("###ActiveMonitorContentSelectButton"), new Vector2(x3, num10), (ImGuiButtonFlags)0);
		bool flag7 = ImGui.IsItemHovered();
		bool flag8 = ImGui.IsItemClicked((ImGuiMouseButton)1);
		if (num12 || flag6)
		{
			provider.SelectItem(currentMonitorItem.ItemID, null, currentMonitorItem.HQOnly);
		}
		if (((Item)(ref data)).CanBeHq && (flag5 || flag8))
		{
			monitorProvider.ToggleHQ();
		}
		bool flag9 = (flag4 || flag7) && !flag && !flag2 && !flag3;
		if (flag9)
		{
			ImGui.SetMouseCursor((ImGuiMouseCursor)7);
			string text4 = ((object)((Item)(ref data)).Name/*cast due to constrained. prefix*/).ToString();
			if (((Item)(ref data)).CanBeHq)
			{
				text4 += $"\n（{LanguageManager.Get("RightClick")}：{(currentMonitorItem.HQOnly ? LanguageManager.Get("All") : LanguageManager.Get("BetterMarketBoard-MarketView-HQ"))}）";
			}
			ImGuiOm.TooltipHover(text4);
		}
		uint num13 = VectorExtensions.WithW(KnownColor.GreenYellow.ToVector4(), flag9 ? 0.2f : 0.12f).ToUInt();
		uint num14 = VectorExtensions.WithW(KnownColor.GreenYellow.ToVector4(), 0.75f).ToUInt();
		((ImDrawListPtr)(ref windowDrawList)).AddRectFilled(cursorScreenPos, vector, num13, num);
		((ImDrawListPtr)(ref windowDrawList)).AddRect(cursorScreenPos, vector, num14, num, (ImDrawFlags)0, 1f * Globals.GlobalUIScale);
		using (OmenServiceBase<FontManager>.Instance().UIFont80.Push())
		{
			string text5 = FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61683) + " " + LanguageManager.Get("BetterMarketBoard-PriceMonitor-Item");
			((ImDrawListPtr)(ref windowDrawList)).AddText(new Vector2(cursorScreenPos.X + num2, cursorScreenPos.Y + num3 + (num8 - ImGui.GetTextLineHeight()) / 2f), KnownColor.GreenYellow.ToUInt(), ImU8String.op_Implicit(text5));
		}
		using (OmenServiceBase<FontManager>.Instance().UIFont80.Push())
		{
			uint num15;
			uint num16;
			uint num17;
			if (!config.AutoBuy)
			{
				num15 = ImGui.GetColorU32((ImGuiCol)7, 0.4f);
				num16 = ImGui.GetColorU32((ImGuiCol)5, 0.25f);
				num17 = ImGui.GetColorU32((ImGuiCol)1);
			}
			else
			{
				num15 = VectorExtensions.WithW(KnownColor.Chocolate.ToVector4(), 0.2f).ToUInt();
				num16 = VectorExtensions.WithW(KnownColor.Chocolate.ToVector4(), 0.75f).ToUInt();
				num17 = KnownColor.Chocolate.ToUInt();
			}
			((ImDrawListPtr)(ref windowDrawList)).AddRectFilled(vector5, vector4, num15, 3f * Globals.GlobalUIScale);
			((ImDrawListPtr)(ref windowDrawList)).AddRect(vector5, vector4, num16, 3f * Globals.GlobalUIScale, (ImDrawFlags)0, 1f * Globals.GlobalUIScale);
			((ImDrawListPtr)(ref windowDrawList)).AddText(new Vector2(vector5.X + num4, vector5.Y + num5), num17, ImU8String.op_Implicit(text));
		}
		using (OmenServiceBase<FontManager>.Instance().UIFont60.Push())
		{
			string text6 = FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61453);
			Vector2 vector9 = ImGui.CalcTextSize(ImU8String.op_Implicit(text6), false, -1f);
			Vector2 vector10 = new Vector2(cursorScreenPos2.X + (vector2.X - vector9.X) / 2f, cursorScreenPos2.Y + (vector2.Y - vector9.Y) / 2f);
			uint num18 = (flag ? KnownColor.OrangeRed.ToUInt() : ImGui.GetColorU32((ImGuiCol)1, 0.6f));
			((ImDrawListPtr)(ref windowDrawList)).AddText(vector10, num18, ImU8String.op_Implicit(text6));
		}
		ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
		GameIconLookup val2 = new GameIconLookup((uint)((Item)(ref data)).Icon, currentMonitorItem.HQOnly, true, (ClientLanguage?)null);
		ISharedImmediateTexture val3 = default(ISharedImmediateTexture);
		if (obj.TryGetFromGameIcon(ref val2, ref val3))
		{
			Vector2 vector11 = new Vector2(cursorScreenPos.X + num2, num9);
			((ImDrawListPtr)(ref windowDrawList)).AddImage(val3.GetWrapOrEmpty().Handle, vector11, vector11 + new Vector2(num10));
		}
		float x4 = cursorScreenPos.X + num2 + num10 + 6f * Globals.GlobalUIScale;
		string text7 = ((object)((Item)(ref data)).Name/*cast due to constrained. prefix*/).ToString();
		if (currentMonitorItem.HQOnly)
		{
			text7 += " \ue03c";
		}
		Vector2 vector12 = cursorScreenPos;
		cursorScreenPos3 = vector;
		cursorScreenPos3.X = vector.X - num2;
		((ImDrawListPtr)(ref windowDrawList)).PushClipRect(vector12, cursorScreenPos3, true);
		((ImDrawListPtr)(ref windowDrawList)).AddText(new Vector2(x4, num9), ImGui.GetColorU32((ImGuiCol)0), ImU8String.op_Implicit(text7));
		((ImDrawListPtr)(ref windowDrawList)).PopClipRect();
		using (OmenServiceBase<FontManager>.Instance().UIFont80.Push())
		{
			if (flag3)
			{
				((ImDrawListPtr)(ref windowDrawList)).AddRectFilled(vector7, vector7 + vector8, ImGui.GetColorU32((ImGuiCol)8, 0.4f), 2f * Globals.GlobalUIScale);
			}
			((ImDrawListPtr)(ref windowDrawList)).AddText(new Vector2(vector.X - num2 - vector6.X, num11), KnownColor.Orange.ToUInt(), ImU8String.op_Implicit(text3));
		}
		ImGui.SetCursorScreenPos(cursorScreenPos + new Vector2(0f, cardHeight));
	}

	private static void DrawEmptyState(string icon, string message)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		Vector2 contentRegionAvail = ImGui.GetContentRegionAvail();
		Vector2 vector = ImGui.GetCursorScreenPos() + contentRegionAvail / 2f;
		ImDrawListPtr windowDrawList;
		using (OmenServiceBase<FontManager>.Instance().UIFont160.Push())
		{
			Vector2 vector2 = ImGui.CalcTextSize(ImU8String.op_Implicit(icon), false, -1f);
			Vector2 vector3 = new Vector2(vector.X - vector2.X / 2f, vector.Y - vector2.Y - 6f * Globals.GlobalUIScale);
			windowDrawList = ImGui.GetWindowDrawList();
			((ImDrawListPtr)(ref windowDrawList)).AddText(vector3, ImGui.GetColorU32((ImGuiCol)1, 0.4f), ImU8String.op_Implicit(icon));
		}
		using (OmenServiceBase<FontManager>.Instance().UIFont80.Push())
		{
			Vector2 vector4 = ImGui.CalcTextSize(ImU8String.op_Implicit(message), false, -1f);
			Vector2 vector5 = new Vector2(vector.X - vector4.X / 2f, vector.Y + 6f * Globals.GlobalUIScale);
			windowDrawList = ImGui.GetWindowDrawList();
			((ImDrawListPtr)(ref windowDrawList)).AddText(vector5, ImGui.GetColorU32((ImGuiCol)1, 0.6f), ImU8String.op_Implicit(message));
		}
	}

	private void DrawMonitorPriceSettingsPopup(MonitorItem monitorItem)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		PopupDisposable val = ImRaii.Popup(ImU8String.op_Implicit("###MonitorPriceSettingsPopup"), (ImGuiWindowFlags)64);
		try
		{
			if (!val)
			{
				return;
			}
			provider.EnsurePriceData(monitorItem.ItemID, monitorItem.HQOnly);
			int priceThreshold = (int)monitorItem.PriceThreshold;
			ulong? selectedWorldMinPrice = provider.GetSelectedWorldMinPrice(monitorItem.ItemID, monitorItem.HQOnly);
			ulong? regionMinPrice = provider.GetRegionMinPrice(monitorItem.ItemID, monitorItem.HQOnly);
			uint? nPCGilPrice = provider.GetNPCGilPrice(monitorItem.ItemID);
			ulong? item = provider.GetItemAggregatedStats(monitorItem.ItemID, provider.SelectedWorldID, monitorItem.HQOnly).AvgPrice;
			ulong? historyPercentilePrice = provider.GetHistoryPercentilePrice(monitorItem.ItemID, monitorItem.HQOnly, 0.5);
			ulong? historyPercentilePrice2 = provider.GetHistoryPercentilePrice(monitorItem.ItemID, monitorItem.HQOnly, 0.25);
			Vector4 vector = KnownColor.Chocolate.ToVector4();
			ImU8String val2 = new ImU8String(1, 2);
			((ImU8String)(ref val2)).AppendFormatted<string>(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61483));
			((ImU8String)(ref val2)).AppendLiteral(" ");
			((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get("BetterMarketBoard-PriceMonitor-Price"));
			ImGui.TextColored(ref vector, val2);
			ItemWidthDisposable val3 = ImRaii.ItemWidth(200f * Globals.GlobalUIScale);
			ImU8String val6;
			try
			{
				IndentDisposable val4 = ImRaii.PushIndent(1, true);
				try
				{
					ImGui.AlignTextToFramePadding();
					ImGui.TextUnformatted(ImU8String.op_Implicit("≤"));
					ImGui.SameLine();
					ImU8String val5 = ImU8String.op_Implicit("\ue049##MonitorPriceThresholdInput");
					val6 = default(ImU8String);
					if (ImGui.InputInt(val5, ref priceThreshold, 100, 1000, val6, (ImGuiInputTextFlags)0))
					{
						monitorItem.PriceThreshold = (uint)Math.Max(1, priceThreshold);
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
				((IDisposable)val3)?.Dispose();
			}
			ImGui.NewLine();
			vector = KnownColor.Chocolate.ToVector4();
			val6 = new ImU8String(1, 2);
			((ImU8String)(ref val6)).AppendFormatted<string>(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61953));
			((ImU8String)(ref val6)).AppendLiteral(" ");
			((ImU8String)(ref val6)).AppendFormatted<string>(LanguageManager.Get("BetterMarketBoard-PriceMonitor-Quick-Title"));
			ImGui.TextColored(ref vector, val6);
			List<(string, ulong?, Vector4, string)> list = new List<(string, ulong?, Vector4, string)>
			{
				(LanguageManager.Get("BetterMarketBoard-PriceMonitor-Quick-WorldMin"), selectedWorldMinPrice, KnownColor.GreenYellow.ToVector4(), FontAwesomeExtensions.ToIconString((FontAwesomeIcon)62405)),
				(LanguageManager.Get("BetterMarketBoard-PriceMonitor-Quick-RegionMin"), regionMinPrice, KnownColor.Chocolate.ToVector4(), FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61612)),
				(LanguageManager.Get("BetterMarketBoard-PriceMonitor-Quick-Average"), item, KnownColor.Chocolate.ToVector4(), FontAwesomeExtensions.ToIconString((FontAwesomeIcon)62750)),
				(LanguageManager.Get("BetterMarketBoard-PriceMonitor-Quick-LowPercentile"), historyPercentilePrice2, KnownColor.MediumPurple.ToVector4(), FontAwesomeExtensions.ToIconString((FontAwesomeIcon)62101)),
				(LanguageManager.Get("BetterMarketBoard-PriceMonitor-Quick-Median"), historyPercentilePrice, KnownColor.SlateBlue.ToVector4(), FontAwesomeExtensions.ToIconString((FontAwesomeIcon)62030))
			};
			if (!monitorItem.HQOnly && (nPCGilPrice ?? 0) != 0)
			{
				list.Add((LanguageManager.Get("BetterMarketBoard-PriceMonitor-Quick-NPC"), nPCGilPrice, KnownColor.DarkOrange.ToVector4(), FontAwesomeExtensions.ToIconString((FontAwesomeIcon)62798)));
			}
			DrawBenchmarkCardsGrid(monitorItem, list);
		}
		finally
		{
			((PopupDisposable)(ref val)).Dispose();
		}
	}

	private void DrawMonitorAutoPurchaseSettings(MonitorItem monitorItem)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		uint autoBuyQuantityLimit = config.AutoBuyQuantityLimit;
		uint itemCount = LocalPlayerState.GetItemCount(monitorItem.ItemID);
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(1, 2);
		((ImU8String)(ref val)).AppendFormatted<string>(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61975));
		((ImU8String)(ref val)).AppendLiteral(" ");
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("BetterMarketBoard-PriceMonitor-AutoPurchase-QuantityLimit"));
		ImGui.TextColored(ref vector, val);
		ImGuiOm.HelpMarker(LanguageManager.Get("BetterMarketBoard-PriceMonitor-AutoPurchase-QuantityLimit-Help"), 40f, (FontAwesomeIcon)61530);
		ItemWidthDisposable val2 = ImRaii.ItemWidth(200f * Globals.GlobalUIScale);
		try
		{
			IndentDisposable val3 = ImRaii.PushIndent(1, true);
			try
			{
				ImU8String val4 = new ImU8String(3, 1);
				((ImU8String)(ref val4)).AppendLiteral("###");
				((ImU8String)(ref val4)).AppendFormatted<string>("AutoBuyQuantityLimit");
				ImU8String val5 = val4;
				ImU8String val6 = default(ImU8String);
				if (ImGui.InputUInt(val5, ref autoBuyQuantityLimit, 1u, 10u, val6, (ImGuiInputTextFlags)0))
				{
					monitorProvider.SetQuantityLimit(autoBuyQuantityLimit);
				}
				val6 = new ImU8String(1, 2);
				((ImU8String)(ref val6)).AppendFormatted<string>(LuminaWrapper.GetAddonText(358u));
				((ImU8String)(ref val6)).AppendLiteral("：");
				((ImU8String)(ref val6)).AppendFormatted<uint>(itemCount);
				ImGui.TextDisabled(val6);
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
	}

	private void DrawBenchmarkCardsGrid(MonitorItem monitorItem, IReadOnlyList<(string Label, ulong? Price, Vector4 Color, string Icon)> benchmarks)
	{
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
		float x = 190f * Globals.GlobalUIScale;
		float y = 36f * Globals.GlobalUIScale;
		float num = 4f * Globals.GlobalUIScale;
		float num2 = 6f * Globals.GlobalUIScale;
		float x2 = 6f * Globals.GlobalUIScale;
		float y2 = 4f * Globals.GlobalUIScale;
		StyleDisposable val = ImRaii.PushStyle((ImGuiStyleVar)13, new Vector2(x2, y2), true);
		try
		{
			for (int i = 0; i < benchmarks.Count; i++)
			{
				(string Label, ulong? Price, Vector4 Color, string Icon) tuple = benchmarks[i];
				string item = tuple.Label;
				ulong? item2 = tuple.Price;
				Vector4 item3 = tuple.Color;
				string item4 = tuple.Icon;
				bool flag = item2.HasValue && item2.GetValueOrDefault() != 0;
				bool flag2 = flag && monitorItem.PriceThreshold == item2.Value;
				ImU8String val2 = new ImU8String(17, 1);
				((ImU8String)(ref val2)).AppendLiteral("###BenchmarkCard_");
				((ImU8String)(ref val2)).AppendFormatted<int>(i);
				ImGui.InvisibleButton(val2, new Vector2(x, y), (ImGuiButtonFlags)0);
				if (flag && ImGui.IsItemClicked())
				{
					monitorItem.PriceThreshold = (uint)Math.Min(4294967295uL, item2.Value);
					config.Save(this);
				}
				bool flag3 = ImGui.IsItemHovered() && flag;
				if (flag3)
				{
					ImGui.SetMouseCursor((ImGuiMouseCursor)7);
					ImGuiOm.TooltipHover($"{item}：{item2.Value.ToChineseString()}\ue049\n{LanguageManager.Get("LeftClick")}：{LanguageManager.Get("Apply")}");
				}
				Vector2 itemRectMin = ImGui.GetItemRectMin();
				Vector2 itemRectMax = ImGui.GetItemRectMax();
				ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
				float num3 = (flag2 ? 0.22f : (flag3 ? 0.16f : 0.08f));
				float num4 = (flag2 ? 0.9f : (flag3 ? 0.6f : 0.25f));
				Vector4 color = (flag ? item3 : KnownColor.Gray.ToVector4());
				((ImDrawListPtr)(ref windowDrawList)).AddRectFilled(itemRectMin, itemRectMax, VectorExtensions.WithW(color, num3).ToUInt(), num);
				((ImDrawListPtr)(ref windowDrawList)).AddRect(itemRectMin, itemRectMax, VectorExtensions.WithW(color, num4).ToUInt(), num, (ImDrawFlags)0, flag2 ? (1.5f * Globals.GlobalUIScale) : (1f * Globals.GlobalUIScale));
				float y3 = itemRectMin.Y + 3f * Globals.GlobalUIScale;
				float num5 = itemRectMax.Y - 3f * Globals.GlobalUIScale;
				using (OmenServiceBase<FontManager>.Instance().UIFont60.Push())
				{
					string text = item4 + " " + item;
					((ImDrawListPtr)(ref windowDrawList)).AddText(new Vector2(itemRectMin.X + num2, y3), VectorExtensions.WithW(color, flag ? 0.9f : 0.5f).ToUInt(), ImU8String.op_Implicit(text));
					if (flag2)
					{
						string text2 = FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61452);
						Vector2 vector = ImGui.CalcTextSize(ImU8String.op_Implicit(text2), false, -1f);
						((ImDrawListPtr)(ref windowDrawList)).AddText(new Vector2(itemRectMax.X - num2 - vector.X, y3), color.ToUInt(), ImU8String.op_Implicit(text2));
					}
				}
				using (OmenServiceBase<FontManager>.Instance().UIFont80.Push())
				{
					string text3 = (flag ? (item2.Value.ToChineseString() + "\ue049") : "-");
					uint num6 = ((!flag) ? ImGui.GetColorU32((ImGuiCol)1) : (flag2 ? color.ToUInt() : ImGui.GetColorU32((ImGuiCol)0)));
					Vector2 vector2 = ImGui.CalcTextSize(ImU8String.op_Implicit(text3), false, -1f);
					((ImDrawListPtr)(ref windowDrawList)).AddText(new Vector2(itemRectMax.X - num2 - vector2.X, num5 - vector2.Y), num6, ImU8String.op_Implicit(text3));
				}
				if (i % 2 == 0 && i + 1 < benchmarks.Count)
				{
					ImGui.SameLine();
				}
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private unsafe void DrawRightContent(MarketBoardUIContext frame)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0477: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0500: Unknown result type (might be due to invalid IL or missing references)
		//IL_050f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0514: Unknown result type (might be due to invalid IL or missing references)
		//IL_0533: Unknown result type (might be due to invalid IL or missing references)
		//IL_0537: Unknown result type (might be due to invalid IL or missing references)
		//IL_053c: Unknown result type (might be due to invalid IL or missing references)
		//IL_053e: Unknown result type (might be due to invalid IL or missing references)
		//IL_055c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0561: Unknown result type (might be due to invalid IL or missing references)
		//IL_0580: Unknown result type (might be due to invalid IL or missing references)
		//IL_0582: Unknown result type (might be due to invalid IL or missing references)
		//IL_0587: Unknown result type (might be due to invalid IL or missing references)
		//IL_0589: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d3: Unknown result type (might be due to invalid IL or missing references)
		InfoProxyItemSearch* infoProxy = InfoProxy;
		if (infoProxy == null || frame.ItemID == 0 || !frame.HasItem)
		{
			return;
		}
		Item itemData = frame.ItemData;
		ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
		GameIconLookup val = new GameIconLookup((uint)((Item)(ref itemData)).Icon, frame.HQOnly, true, (ClientLanguage?)null);
		IDalamudTextureWrap wrapOrDefault = obj.GetFromGameIcon(ref val).GetWrapOrDefault((IDalamudTextureWrap)null);
		if (wrapOrDefault == null)
		{
			return;
		}
		using (OmenServiceBase<FontManager>.Instance().UIFont.Push())
		{
			bool flag = false;
			ImTextureID handle = wrapOrDefault.Handle;
			Vector2 vector = marketDataTableImageSize;
			vector.X = marketDataTableImageSize.Y;
			ImGui.Image(handle, vector);
			if (ImGui.IsItemHovered())
			{
				flag = true;
			}
			ImGui.SameLine();
			GroupDisposable val2 = ImRaii.Group();
			try
			{
				using (OmenServiceBase<FontManager>.Instance().UIFont160.Push())
				{
					ImGui.AlignTextToFramePadding();
					ImGui.TextUnformatted(ImU8String.op_Implicit(((object)((Item)(ref itemData)).Name/*cast due to constrained. prefix*/).ToString() + (frame.HQOnly ? "\ue03c" : string.Empty)));
					if (ImGui.IsItemHovered())
					{
						ImGui.SetMouseCursor((ImGuiMouseCursor)7);
						flag = true;
					}
					if (ImGui.IsItemClicked())
					{
						ImU8String clipboardText = new ImU8String(0, 1);
						((ImU8String)(ref clipboardText)).AppendFormatted<string>(((object)((Item)(ref itemData)).Name/*cast due to constrained. prefix*/).ToString());
						ImGui.SetClipboardText(clipboardText);
						OmenServiceBase<NotifyHelper>.Instance().NotificationSuccess(LanguageManager.Get("CopiedToClipboard") + ": " + ((object)((Item)(ref itemData)).Name/*cast due to constrained. prefix*/).ToString());
					}
				}
				if (flag)
				{
					(*AtkStage.Instance()).ShowItemTooltip(((AtkUnitBase)Addons.ScreenText).RootNode, ((Item)(ref itemData)).RowId);
					isHeaderTooltip = true;
				}
				else if (isHeaderTooltip)
				{
					isHeaderTooltip = false;
					(*AtkStage.Instance()).HideTooltip(((AtkUnitBase)Addons.ScreenText).Id);
				}
				GroupDisposable val3 = ImRaii.Group();
				try
				{
					using (OmenServiceBase<FontManager>.Instance().UIFont80.Push())
					{
						bool flag2 = config.FavoriteItems.ContainsKey(frame.ItemID);
						if (ImGui.Button(ImU8String.op_Implicit(flag2 ? "★" : "☆"), default(Vector2)))
						{
							if (flag2)
							{
								config.FavoriteItems.Remove(frame.ItemID);
							}
							else
							{
								config.FavoriteItems[frame.ItemID] = new MarketFavoriteItem
								{
									ItemID = frame.ItemID
								};
							}
							favoriteItemsVersion++;
							config.Save(this);
						}
						ImGuiOm.TooltipHover(LanguageManager.Get("Favorite"));
						bool flag3 = config.CurrentMonitorItem?.ItemID == frame.ItemID;
						ImGui.SameLine();
						ColorDisposable val4 = ImRaii.PushColor((ImGuiCol)0, KnownColor.GreenYellow.ToVector4(), flag3);
						try
						{
							if (ImGui.Button(ImU8String.op_Implicit(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61683)), default(Vector2)))
							{
								if (flag3)
								{
									monitorProvider.ClearMonitor();
								}
								else
								{
									monitorProvider.SetMonitor(frame.ItemID, frame.HQOnly);
								}
							}
						}
						finally
						{
							((IDisposable)val4)?.Dispose();
						}
						ImGuiOm.TooltipHover(LanguageManager.Get(flag3 ? "BetterMarketBoard-PriceMonitor-ToNotMonitor" : "BetterMarketBoard-PriceMonitor-ToMonitor"));
						if (((Item)(ref itemData)).CanBeHq)
						{
							ImGui.SameLine();
							val4 = ImRaii.PushColor((ImGuiCol)0, KnownColor.GreenYellow.ToVector4(), frame.HQOnly);
							try
							{
								if (ImGui.Button(ImU8String.op_Implicit("\ue03c###HQOnly"), default(Vector2)))
								{
									provider.ToggleHQ();
								}
							}
							finally
							{
								((IDisposable)val4)?.Dispose();
							}
							ImGuiOm.TooltipHover(frame.HQOnly ? (LanguageManager.Get("All") ?? "") : (LanguageManager.Get("BetterMarketBoard-MarketView-HQ") ?? ""));
						}
						if (!frame.IsViewingCurrentWorld)
						{
							ImGui.SameLine();
							if (ImGui.Button(ImU8String.op_Implicit(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61554)), default(Vector2)))
							{
								OmenServiceBase<ChatManager>.Instance().SendMessage("/pdr worldtravel " + LuminaWrapper.GetWorldName(frame.SelectedWorldID));
							}
							ImGuiOm.TooltipHover(LanguageManager.Get("BetterMarketBoard-TravelToWorld"));
						}
						ImGui.SameLine();
						if (ImGui.Button(ImU8String.op_Implicit(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61473)), default(Vector2)))
						{
							provider.Reload();
						}
						ImGuiOm.TooltipHover(LanguageManager.Get("BetterMarketBoard-ReloadMarketData"));
					}
				}
				finally
				{
					((GroupDisposable)(ref val3)).Dispose();
				}
			}
			finally
			{
				((GroupDisposable)(ref val2)).Dispose();
			}
			marketDataTableImageSize = ImGui.GetItemRectSize();
			DrawAllWorldsPriceTable(frame);
			using (OmenServiceBase<FontManager>.Instance().UIFont80.Push())
			{
				TabBarDisposable val5 = ImRaii.TabBar(ImU8String.op_Implicit("###BetterMarketBoardTabs"), (ImGuiTabBarFlags)32);
				try
				{
					if (!TabBarDisposable.op_Implicit(val5))
					{
						return;
					}
					ImU8String val6 = new ImU8String(12, 1);
					((ImU8String)(ref val6)).AppendFormatted<string>(LanguageManager.Get("BetterMarketBoard-MarketData"));
					((ImU8String)(ref val6)).AppendLiteral("##MarketData");
					TabItemDisposable val7 = ImRaii.TabItem(val6, (ImGuiTabItemFlags)64);
					try
					{
						if (TabItemDisposable.op_Implicit(val7))
						{
							DrawMarketListingsTab(frame, infoProxy);
						}
					}
					finally
					{
						((TabItemDisposable)(ref val7)).Dispose();
					}
					ImU8String val8 = new ImU8String(9, 1);
					((ImU8String)(ref val8)).AppendFormatted<string>(LuminaWrapper.GetAddonText(1165u));
					((ImU8String)(ref val8)).AppendLiteral("##History");
					TabItemDisposable val9 = ImRaii.TabItem(val8);
					try
					{
						if (TabItemDisposable.op_Implicit(val9))
						{
							DrawMarketHistoryTab(frame);
						}
					}
					finally
					{
						((TabItemDisposable)(ref val9)).Dispose();
					}
					ImU8String val10 = new ImU8String(12, 1);
					((ImU8String)(ref val10)).AppendFormatted<string>(LanguageManager.Get("BetterMarketBoard-PriceTrend"));
					((ImU8String)(ref val10)).AppendLiteral("##PriceTrend");
					TabItemDisposable val11 = ImRaii.TabItem(val10);
					try
					{
						if (TabItemDisposable.op_Implicit(val11))
						{
							DrawMarketPriceTrend(frame);
						}
					}
					finally
					{
						((TabItemDisposable)(ref val11)).Dispose();
					}
				}
				finally
				{
					((TabBarDisposable)(ref val5)).Dispose();
				}
			}
		}
	}

	private unsafe void DrawMarketListingsTab(MarketBoardUIContext frame, InfoProxyItemSearch* info)
	{
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0640: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		float item = provider.GetItemAggregatedStats(frame.ItemID, frame.SelectedWorldID, frame.HQOnly).DailySales;
		ulong? avgPrice = provider.GetHistoryDataSet(frame.ItemID)?.AvgPrice;
		if (frame.IsLocalMarketSearchable && frame.IsViewingCurrentWorld && provider.SelectedListings.Count > 0)
		{
			ulong number = provider.SelectedListings.Values.Aggregate(0uL, (ulong current, MarketBoardListing l) => current + (l.UnitPrice * l.Quantity + l.TotalTax));
			uint num = provider.SelectedListings.Values.Aggregate(0u, (uint current, MarketBoardListing l) => current + l.Quantity);
			float x = ImGui.GetContentRegionAvail().X;
			ImGuiStylePtr style = ImGui.GetStyle();
			float x2 = ((ImGuiStylePtr)(ref style)).ItemSpacing.X;
			float num2 = MathF.Floor((x - x2) * 0.52f);
			float x3 = x - x2 - num2;
			float textLineHeight;
			using (OmenServiceBase<FontManager>.Instance().UIFont60.Push())
			{
				textLineHeight = ImGui.GetTextLineHeight();
			}
			float textLineHeight2;
			using (OmenServiceBase<FontManager>.Instance().UIFont80.Push())
			{
				textLineHeight2 = ImGui.GetTextLineHeight();
			}
			float y = 6f * Globals.GlobalUIScale * 2f + textLineHeight + textLineHeight2 + 3f * Globals.GlobalUIScale;
			List<MetricItem> list = new List<MetricItem>();
			list.Add(new MetricItem(LanguageManager.Get("Selected"), LanguageManager.Get("BetterMarketBoard-Samples-Format", provider.SelectedListings.Count, num), KnownColor.Chocolate.ToVector4(), null));
			list.Add(new MetricItem(LanguageManager.Get("BetterMarketBoard-BatchPurchase-Total"), number.ToChineseString() + "\ue049", KnownColor.Orange.ToVector4(), null));
			DrawMetricDashboard(list, num2);
			ImGui.SameLine(0f, x2);
			using (OmenServiceBase<FontManager>.Instance().UIFont80.Push())
			{
				if (ImGuiOm.HoldButton("##BatchPurchaseHoldButton", FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61562) + "  " + LanguageManager.Get("BetterMarketBoard-BatchPurchase-Button"), null, autoReset: true, new Vector2(x3, y)))
				{
					List<MarketBoardListing> list2 = provider.SelectedListings.Values.ToList();
					provider.ClearSelectedListings();
					foreach (MarketBoardListing listing in list2)
					{
						base.TaskHelper.Enqueue(() => MarketDataProvider.SendBuyRequest(listing));
						base.TaskHelper.Enqueue(() => ((InfoProxyItemSearch)info).Listings.ToArray().FirstOrDefault((MarketBoardListing val3) => val3.ListingId == listing.ListingId).ListingId == 0);
					}
				}
			}
		}
		else if (InfoProxyItemSearchExtension.get_IsListingsStuck())
		{
			Vector4 vector = KnownColor.Orange.ToVector4();
			ImU8String val = default(ImU8String);
			((ImU8String)(ref val))._002Ector(2, 1);
			((ImU8String)(ref val)).AppendLiteral("（");
			((ImU8String)(ref val)).AppendFormatted<string>(LuminaWrapper.GetAddonText(1998u));
			((ImU8String)(ref val)).AppendLiteral("）");
			ImGui.TextColored(ref vector, val);
		}
		else
		{
			bool flag = !frame.IsViewingCurrentWorld || !frame.IsLocalMarketSearchable;
			ListingsDataSet listingsDataSet = provider.GetListingsDataSet(frame.ItemID);
			int num3 = 0;
			uint num4 = 0u;
			if (flag)
			{
				if (listingsDataSet != null)
				{
					num3 = listingsDataSet.TotalCount;
					num4 = listingsDataSet.TotalQty;
				}
			}
			else
			{
				LocalListingsDataSet localListingsDataSet = provider.GetLocalListingsDataSet(info);
				num3 = localListingsDataSet.TotalCount;
				num4 = localListingsDataSet.TotalQty;
			}
			uint itemCount = LocalPlayerState.GetItemCount(frame.ItemID);
			List<MetricItem> list3 = new List<MetricItem>
			{
				new MetricItem(LuminaWrapper.GetAddonText(358u), $"{itemCount}", (itemCount != 0) ? KnownColor.Chocolate.ToVector4() : KnownColor.White.ToVector4(), null)
			};
			if (num3 > 0)
			{
				list3.Add(new MetricItem(LanguageManager.Get("BetterMarketBoard-InStock"), LanguageManager.Get("BetterMarketBoard-Samples-Format", num3, num4), KnownColor.Chocolate.ToVector4(), null));
			}
			if (item > 0f)
			{
				list3.Add(new MetricItem(LanguageManager.Get("BetterMarketBoard-Tooltip-DailySales"), LanguageManager.Get("BetterMarketBoard-DailySales-Format", item.ToString("0.#", CultureInfo.InvariantCulture)), KnownColor.GreenYellow.ToVector4(), null));
				if (num4 != 0)
				{
					float num5 = (float)num4 / item;
					list3.Add(new MetricItem(LanguageManager.Get("BetterMarketBoard-Turnover"), LanguageManager.Get("BetterMarketBoard-Turnover-Format", num5.ToString("0.#", CultureInfo.InvariantCulture)), KnownColor.Goldenrod.ToVector4(), LanguageManager.Get("BetterMarketBoard-Turnover-Help")));
				}
			}
			if (flag && listingsDataSet?.Source != null)
			{
				DateTime dateTime = listingsDataSet.Source.GetLastUploadTime().ToLocalTime();
				list3.Add(new MetricItem(LanguageManager.Get("BetterMarketBoard-DataFreshness"), dateTime.TimeAgo(), KnownColor.Chocolate.ToVector4(), $"{dateTime:yyyy-MM-dd HH:mm:ss}"));
			}
			DrawMetricDashboard(list3);
			if (flag && listingsDataSet == null)
			{
				ImGui.Spacing();
				Vector4 vector = KnownColor.Gray.ToVector4();
				ImU8String val2 = default(ImU8String);
				((ImU8String)(ref val2))._002Ector(2, 1);
				((ImU8String)(ref val2)).AppendLiteral("\ue031 ");
				((ImU8String)(ref val2)).AppendFormatted<string>(LuminaWrapper.GetAddonText(2717u));
				ImGui.TextColored(ref vector, val2);
			}
		}
		ImGui.Spacing();
		using (OmenServiceBase<FontManager>.Instance().UIFont80.Push())
		{
			if (frame.IsViewingCurrentWorld)
			{
				if (frame.IsLocalMarketSearchable)
				{
					DrawLocalMarketDataTable(frame, info, avgPrice);
					return;
				}
				ListingsDataSet listingsDataSet2 = provider.GetListingsDataSet(frame.ItemID);
				if (listingsDataSet2 != null)
				{
					DrawOnlineMarketDataTable(frame, avgPrice, listingsDataSet2);
				}
			}
			else
			{
				ListingsDataSet listingsDataSet3 = provider.GetListingsDataSet(frame.ItemID);
				if (listingsDataSet3 != null)
				{
					DrawOnlineMarketDataTable(frame, avgPrice, listingsDataSet3);
				}
			}
		}
	}

	private void DrawMarketHistoryTab(MarketBoardUIContext frame)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		uint itemID = frame.ItemID;
		if (itemID == 0)
		{
			return;
		}
		HistoryDataSet historyDataSet = provider.GetHistoryDataSet(itemID);
		if (historyDataSet == null || historyDataSet.Entries.Count == 0)
		{
			ImGui.TextDisabled(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(1998u)));
			return;
		}
		(float DailySales, ulong? AvgPrice, (ulong Price, DateTime Time, string WorldName)? RecentPurchase) itemAggregatedStats = provider.GetItemAggregatedStats(itemID, frame.SelectedWorldID, frame.HQOnly);
		float item = itemAggregatedStats.DailySales;
		(ulong, DateTime, string)? item2 = itemAggregatedStats.RecentPurchase;
		string value = "-";
		if (item2.HasValue)
		{
			value = item2.Value.Item2.TimeAgo();
		}
		else if (historyDataSet.Entries.Count > 0)
		{
			value = historyDataSet.Entries[0].SaleTime.TimeAgo();
		}
		List<MetricItem> list = new List<MetricItem>();
		list.Add(new MetricItem(LanguageManager.Get("BetterMarketBoard-History-Samples"), LanguageManager.Get("BetterMarketBoard-Samples-Format", historyDataSet.TotalCount, historyDataSet.TotalQty), KnownColor.Chocolate.ToVector4(), null));
		List<MetricItem> list2 = list;
		if (historyDataSet.IsCanBeHQ)
		{
			if (historyDataSet.AvgNQPrice != 0)
			{
				list2.Add(new MetricItem(LanguageManager.Get("BetterMarketBoard-AveragePrice-NQ"), historyDataSet.AvgNQPrice.ToChineseString() + "\ue049", KnownColor.Chocolate.ToVector4(), null));
			}
			if (historyDataSet.AvgHQPrice != 0)
			{
				list2.Add(new MetricItem(LanguageManager.Get("BetterMarketBoard-AveragePrice-HQ"), historyDataSet.AvgHQPrice.ToChineseString() + "\ue049", KnownColor.Orange.ToVector4(), null));
			}
			list2.Add(new MetricItem(LanguageManager.Get("BetterMarketBoard-HQRatio"), LanguageManager.Get("BetterMarketBoard-Percent-Format", historyDataSet.HQPercent), KnownColor.Goldenrod.ToVector4(), null));
		}
		else
		{
			list2.Add(new MetricItem(LanguageManager.Get("BetterMarketBoard-AveragePrice"), historyDataSet.AvgPrice.ToChineseString() + "\ue049", KnownColor.Orange.ToVector4(), null));
		}
		if (item > 0f)
		{
			list2.Add(new MetricItem(LanguageManager.Get("BetterMarketBoard-Tooltip-DailySales"), LanguageManager.Get("BetterMarketBoard-DailySales-Format", item.ToString("0.#", CultureInfo.InvariantCulture)), KnownColor.GreenYellow.ToVector4(), null));
		}
		list2.Add(new MetricItem(LanguageManager.Get("BetterMarketBoard-History-Recent"), value, KnownColor.White.ToVector4(), null));
		DrawMetricDashboard(list2);
		ImGui.Spacing();
		DrawOnlineMarketHistoryDataTable(historyDataSet);
	}

	private unsafe void DrawLocalMarketDataTable(MarketBoardUIContext frame, InfoProxyItemSearch* info, ulong? avgPrice)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_0483: Unknown result type (might be due to invalid IL or missing references)
		//IL_049d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0503: Unknown result type (might be due to invalid IL or missing references)
		//IL_0508: Unknown result type (might be due to invalid IL or missing references)
		//IL_052a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0618: Unknown result type (might be due to invalid IL or missing references)
		//IL_0634: Unknown result type (might be due to invalid IL or missing references)
		//IL_0662: Unknown result type (might be due to invalid IL or missing references)
		//IL_064c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0653: Unknown result type (might be due to invalid IL or missing references)
		//IL_0685: Unknown result type (might be due to invalid IL or missing references)
		//IL_06af: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_070c: Unknown result type (might be due to invalid IL or missing references)
		//IL_069a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0715: Unknown result type (might be due to invalid IL or missing references)
		//IL_0732: Unknown result type (might be due to invalid IL or missing references)
		//IL_0755: Unknown result type (might be due to invalid IL or missing references)
		//IL_075a: Unknown result type (might be due to invalid IL or missing references)
		//IL_075e: Unknown result type (might be due to invalid IL or missing references)
		//IL_076a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0771: Unknown result type (might be due to invalid IL or missing references)
		//IL_0778: Unknown result type (might be due to invalid IL or missing references)
		//IL_0780: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_079b: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_081a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0821: Unknown result type (might be due to invalid IL or missing references)
		//IL_0806: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ac0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ac5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0870: Unknown result type (might be due to invalid IL or missing references)
		//IL_0875: Unknown result type (might be due to invalid IL or missing references)
		//IL_0885: Unknown result type (might be due to invalid IL or missing references)
		//IL_0891: Unknown result type (might be due to invalid IL or missing references)
		//IL_0893: Unknown result type (might be due to invalid IL or missing references)
		//IL_0898: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aed: Unknown result type (might be due to invalid IL or missing references)
		//IL_089a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0859: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_08cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_090b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0914: Unknown result type (might be due to invalid IL or missing references)
		//IL_0919: Unknown result type (might be due to invalid IL or missing references)
		//IL_0938: Unknown result type (might be due to invalid IL or missing references)
		//IL_0951: Unknown result type (might be due to invalid IL or missing references)
		//IL_0956: Unknown result type (might be due to invalid IL or missing references)
		//IL_095a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0966: Unknown result type (might be due to invalid IL or missing references)
		//IL_096f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0974: Unknown result type (might be due to invalid IL or missing references)
		//IL_0993: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_09cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a4f: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a17: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a34: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a5d: Unknown result type (might be due to invalid IL or missing references)
		LocalListingsDataSet localListingsDataSet = provider.GetLocalListingsDataSet(info);
		List<MarketBoardListing> listings = localListingsDataSet.Listings;
		bool isAnyHQ = localListingsDataSet.IsAnyHQ;
		bool isAnyOnMannequin = localListingsDataSet.IsAnyOnMannequin;
		bool isAnyMateria = localListingsDataSet.IsAnyMateria;
		int num = 9;
		if (!isAnyHQ)
		{
			num--;
		}
		if (!isAnyMateria)
		{
			num--;
		}
		if (!isAnyOnMannequin)
		{
			num--;
		}
		TableDisposable val = ImRaii.Table(ImU8String.op_Implicit("MarketBoardDataTable"), num, (ImGuiTableFlags)33556352, new Vector2(-1f, ImGui.GetContentRegionAvail().Y));
		try
		{
			if (!val)
			{
				return;
			}
			ImGui.TableSetupScrollFreeze(0, 1);
			ImU8String val2 = ImU8String.op_Implicit(LanguageManager.Get("SelectAll"));
			float textLineHeight = ImGui.GetTextLineHeight();
			ImGuiStylePtr style = ImGui.GetStyle();
			ImGui.TableSetupColumn(val2, (ImGuiTableColumnFlags)8, textLineHeight + ((ImGuiStylePtr)(ref style)).ItemSpacing.X * 2f, 0u);
			if (isAnyHQ)
			{
				ImGui.TableSetupColumn(ImU8String.op_Implicit("\ue03c"), (ImGuiTableColumnFlags)8, ImGui.CalcTextSize(ImU8String.op_Implicit("\ue03c"), false, -1f).X, 0u);
			}
			if (isAnyMateria)
			{
				string addonText = LuminaWrapper.GetAddonText(1937u);
				ImGui.TableSetupColumn(ImU8String.op_Implicit(addonText), (ImGuiTableColumnFlags)8, ImGui.CalcTextSize(ImU8String.op_Implicit(addonText), false, -1f).X, 0u);
			}
			if (isAnyOnMannequin)
			{
				ImGui.TableSetupColumn(ImU8String.op_Implicit(LanguageManager.Get("Mannequin")), (ImGuiTableColumnFlags)8, ImGui.CalcTextSize(ImU8String.op_Implicit(LanguageManager.Get("Mannequin")), false, -1f).X, 0u);
			}
			ImGui.TableSetupColumn(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(357u)), (ImGuiTableColumnFlags)4, 15f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit(LanguageManager.Get("Tax")), (ImGuiTableColumnFlags)4, 15f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit(LanguageManager.Get("Amount")), (ImGuiTableColumnFlags)8, ImGui.CalcTextSize(ImU8String.op_Implicit("12345678"), false, -1f).X, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(6936u)), (ImGuiTableColumnFlags)4, 15f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(1956u)), (ImGuiTableColumnFlags)4, 15f, 0u);
			ImGui.TableNextRow((ImGuiTableRowFlags)1);
			ImGui.TableNextColumn();
			DisabledDisposable val3 = ImRaii.Disabled(listings.Count == 0);
			try
			{
				List<MarketBoardListing> list = listings.Where((MarketBoardListing x) => !IsOwnRetainer(x.RetainerId)).ToList();
				int num2 = list.Count((MarketBoardListing x) => provider.SelectedListings.ContainsKey(x.ListingId));
				bool flag = list.Count > 0 && num2 == list.Count;
				bool flag2 = flag;
				ImGui.Checkbox(ImU8String.op_Implicit("###SelectAll"), ref flag2);
				if (ImGui.IsItemClicked((ImGuiMouseButton)0))
				{
					if (!flag)
					{
						foreach (MarketBoardListing item in list)
						{
							provider.AddListing(item.ListingId, item);
						}
					}
					else
					{
						foreach (MarketBoardListing item2 in list)
						{
							provider.RemoveListing(item2.ListingId);
						}
					}
				}
				else if (ImGui.IsItemClicked((ImGuiMouseButton)1))
				{
					foreach (MarketBoardListing item3 in list)
					{
						provider.ToggleListing(item3.ListingId, item3);
					}
				}
				ImGuiOm.TooltipHover($"{LanguageManager.Get("LeftClick")}：{LanguageManager.Get("SelectAll")}/{LanguageManager.Get("DeselectAll")}\n{LanguageManager.Get("RightClick")}：{LanguageManager.Get("InvertSelection")}");
			}
			finally
			{
				((IDisposable)val3)?.Dispose();
			}
			if (isAnyHQ)
			{
				ImGui.TableNextColumn();
				ImGui.TextUnformatted(ImU8String.op_Implicit("\ue03c"));
			}
			if (isAnyMateria)
			{
				ImGui.TableNextColumn();
				ImGui.TextUnformatted(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(1937u)));
			}
			if (isAnyOnMannequin)
			{
				ImGui.TableNextColumn();
				ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("Mannequin")));
			}
			ImGui.TableNextColumn();
			ImGui.TextUnformatted(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(357u)));
			ImGui.TableNextColumn();
			ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("Tax")));
			ImGui.TableNextColumn();
			ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("Amount")));
			ImGui.TableNextColumn();
			ImGui.TextUnformatted(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(6936u)));
			ImGui.TableNextColumn();
			ImGui.TextUnformatted(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(1956u)));
			int num3 = -1;
			List<MarketBenchmarkInfo> list2 = BuildMarketBenchmarks(frame.ItemID, avgPrice, frame.NPCGilPrice);
			foreach (MarketBoardListing item4 in listings)
			{
				MarketBoardListing current4 = item4;
				foreach (MarketBenchmarkInfo item5 in list2)
				{
					if (!item5.Drawn && current4.UnitPrice > item5.Price)
					{
						DrawBenchmarkSeparatorRow($"LocalBenchmark_{item5.Price}_{item5.Badge}", item5.Badge, item5.Color, item5.Tooltip, item5.OnClick);
						item5.Drawn = true;
					}
				}
				num3++;
				IdDisposable val4 = ImRaii.PushId(ImU8String.op_Implicit(current4.ListingId.ToString()), true);
				try
				{
					ImGui.TableNextRow();
					bool flag3 = IsOwnRetainer(current4.RetainerId);
					ColorDisposable val5 = ImRaii.PushColor((ImGuiCol)0, ImGui.GetColorU32((ImGuiCol)1), flag3);
					try
					{
						ImGui.TableNextColumn();
						bool flag4 = provider.SelectedListings.ContainsKey(current4.ListingId);
						val3 = ImRaii.Disabled(flag3);
						try
						{
							if (ImGui.Checkbox(ImU8String.op_Implicit("###SelectCheckbox"), ref flag4))
							{
								if (flag4)
								{
									provider.ToggleListing(current4.ListingId, current4);
								}
								else
								{
									provider.RemoveListing(current4.ListingId);
								}
							}
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
						if (isAnyHQ)
						{
							ImGui.TableNextColumn();
							ImGui.TextUnformatted(ImU8String.op_Implicit(current4.IsHqItem ? "√" : string.Empty));
						}
						if (isAnyMateria)
						{
							ImGui.TableNextColumn();
							ImU8String val6 = new ImU8String(0, 1);
							((ImU8String)(ref val6)).AppendFormatted<byte>(current4.MateriaCount);
							ImGui.TextUnformatted(val6);
						}
						if (isAnyOnMannequin)
						{
							ImGui.TableNextColumn();
							ImGui.TextUnformatted(ImU8String.op_Implicit(((MarketBoardListing)(ref current4)).IsSellingAsSet ? "√" : string.Empty));
						}
						ImGui.TableNextColumn();
						DrawMarketPrice(current4.UnitPrice);
						ImGui.TableNextColumn();
						ImGui.TextUnformatted(ImU8String.op_Implicit((current4.TotalTax == 0) ? "-" : (current4.TotalTax.ToChineseString() + "\ue049")));
						ImGuiOmExtension.ClickToCopyAndNotify(current4.TotalTax.ToString());
						ImGui.TableNextColumn();
						ImU8String val7 = new ImU8String(0, 1);
						((ImU8String)(ref val7)).AppendFormatted<uint>(current4.Quantity);
						ImGui.TextUnformatted(val7);
						uint number = current4.UnitPrice * current4.Quantity + current4.TotalTax;
						ImGui.TableNextColumn();
						val3 = ImRaii.Disabled(flag3);
						try
						{
							ImU8String val8 = new ImU8String(1, 1);
							((ImU8String)(ref val8)).AppendFormatted<string>(number.ToChineseString());
							((ImU8String)(ref val8)).AppendLiteral("\ue049");
							ImU8String val9 = val8;
							ImU8String val10 = new ImU8String(16, 1);
							((ImU8String)(ref val10)).AppendLiteral("ExecuteBuyPopup_");
							((ImU8String)(ref val10)).AppendFormatted<ulong>(current4.ListingId);
							if (ImGui.Selectable(val9, ImGui.IsPopupOpen(val10, (ImGuiPopupFlags)0), (ImGuiSelectableFlags)2, default(Vector2)))
							{
								if (flag4)
								{
									provider.RemoveListing(current4.ListingId);
								}
								else
								{
									provider.ToggleListing(current4.ListingId, current4);
								}
							}
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
						if (PluginConfig.Instance().ConflictKeyBinding.IsPressed())
						{
							if (ImGui.IsItemClicked((ImGuiMouseButton)1) && !flag3)
							{
								MarketDataProvider.SendBuyRequest(current4);
							}
						}
						else if (!flag3)
						{
							ImU8String val11 = new ImU8String(16, 1);
							((ImU8String)(ref val11)).AppendLiteral("ExecuteBuyPopup_");
							((ImU8String)(ref val11)).AppendFormatted<ulong>(current4.ListingId);
							PopupDisposable val12 = ImRaii.ContextPopupItem(val11);
							try
							{
								if (PopupDisposable.op_Implicit(val12))
								{
									ImU8String val13 = new ImU8String(1, 1);
									((ImU8String)(ref val13)).AppendFormatted<string>(LuminaWrapper.GetAddonText(357u));
									((ImU8String)(ref val13)).AppendLiteral(":");
									ImGui.TextUnformatted(val13);
									ImGui.SameLine();
									Vector4 vector = KnownColor.Chocolate.ToVector4();
									ImU8String val14 = new ImU8String(1, 1);
									((ImU8String)(ref val14)).AppendFormatted<string>(current4.UnitPrice.ToChineseString());
									((ImU8String)(ref val14)).AppendLiteral("\ue049");
									ImGui.TextColored(ref vector, val14);
									ImU8String val15 = new ImU8String(1, 1);
									((ImU8String)(ref val15)).AppendFormatted<string>(LanguageManager.Get("Amount"));
									((ImU8String)(ref val15)).AppendLiteral(":");
									ImGui.TextUnformatted(val15);
									ImGui.SameLine();
									vector = KnownColor.Chocolate.ToVector4();
									ImU8String val16 = new ImU8String(0, 1);
									((ImU8String)(ref val16)).AppendFormatted<uint>(current4.Quantity);
									ImGui.TextColored(ref vector, val16);
									ImU8String val17 = new ImU8String(1, 1);
									((ImU8String)(ref val17)).AppendFormatted<string>(LuminaWrapper.GetAddonText(6936u));
									((ImU8String)(ref val17)).AppendLiteral(":");
									ImGui.TextUnformatted(val17);
									ImGui.SameLine();
									vector = KnownColor.Chocolate.ToVector4();
									ImU8String val18 = new ImU8String(1, 1);
									((ImU8String)(ref val18)).AppendFormatted<string>(number.ToChineseString());
									((ImU8String)(ref val18)).AppendLiteral("\ue049");
									ImGui.TextColored(ref vector, val18);
									if (current4.TotalTax != 0)
									{
										ImGui.SameLine();
										ImU8String val19 = new ImU8String(5, 2);
										((ImU8String)(ref val19)).AppendLiteral("(");
										((ImU8String)(ref val19)).AppendFormatted<string>(LanguageManager.Get("Tax"));
										((ImU8String)(ref val19)).AppendLiteral(": ");
										((ImU8String)(ref val19)).AppendFormatted<string>(current4.TotalTax.ToChineseString());
										((ImU8String)(ref val19)).AppendLiteral("\ue049)");
										ImGui.TextUnformatted(val19);
									}
									ImGui.Separator();
									ImGui.Spacing();
									if (ImGui.MenuItem(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(9275u)), false, true))
									{
										MarketDataProvider.SendBuyRequest(current4);
									}
									ImGuiOm.TooltipHover(LanguageManager.Get("BetterMarketBoard-Purchase-Help", PluginConfig.Instance().ConflictKeyBinding));
								}
							}
							finally
							{
								((PopupDisposable)(ref val12)).Dispose();
							}
						}
						ImGui.TableNextColumn();
						CStringPointer val20 = ((CStringPointer*)((StringArrayData)((AtkStage)AtkStage.Instance()).GetStringArrayData((StringArrayType)34)).StringArray)[208 + 6 * num3];
						if (((CStringPointer)(ref val20)).HasValue)
						{
							ImU8String val21 = new ImU8String(0, 1);
							((ImU8String)(ref val21)).AppendFormatted<string>(((object)(*(CStringPointer*)(&val20))/*cast due to constrained. prefix*/).ToString());
							ImGui.TextUnformatted(val21);
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
			foreach (MarketBenchmarkInfo item6 in list2)
			{
				if (!item6.Drawn)
				{
					DrawBenchmarkSeparatorRow($"LocalBenchmark_End_{item6.Price}_{item6.Badge}", item6.Badge, item6.Color, item6.Tooltip, item6.OnClick);
					item6.Drawn = true;
				}
			}
		}
		finally
		{
			((TableDisposable)(ref val)).Dispose();
		}
	}

	private static void DrawOnlineMarketDataTable(MarketBoardUIContext frame, ulong? avgPrice, ListingsDataSet dataset)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		if (dataset.Listings.Count == 0)
		{
			return;
		}
		bool isAnyHQ = dataset.IsAnyHQ;
		bool isAnyOnMannequin = dataset.IsAnyOnMannequin;
		int num = 6;
		if (!isAnyHQ)
		{
			num--;
		}
		if (!isAnyOnMannequin)
		{
			num--;
		}
		TableDisposable val = ImRaii.Table(ImU8String.op_Implicit("UniversalisMarketDataTable"), num, (ImGuiTableFlags)33556352, new Vector2(-1f, ImGui.GetContentRegionAvail().Y));
		try
		{
			if (!val)
			{
				return;
			}
			ImGui.TableSetupScrollFreeze(0, 1);
			if (isAnyHQ)
			{
				ImGui.TableSetupColumn(ImU8String.op_Implicit("\ue03c"), (ImGuiTableColumnFlags)8, ImGui.CalcTextSize(ImU8String.op_Implicit("\ue03c"), false, -1f).X, 0u);
			}
			if (isAnyOnMannequin)
			{
				ImGui.TableSetupColumn(ImU8String.op_Implicit(LanguageManager.Get("Mannequin")), (ImGuiTableColumnFlags)8, ImGui.CalcTextSize(ImU8String.op_Implicit(LanguageManager.Get("Mannequin")), false, -1f).X, 0u);
			}
			ImGui.TableSetupColumn(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(357u)), (ImGuiTableColumnFlags)4, 15f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit(LanguageManager.Get("Amount")), (ImGuiTableColumnFlags)8, ImGui.CalcTextSize(ImU8String.op_Implicit("12345678"), false, -1f).X, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(6936u)), (ImGuiTableColumnFlags)4, 15f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(1956u)), (ImGuiTableColumnFlags)4, 15f, 0u);
			ImGui.TableHeadersRow();
			List<MarketBenchmarkInfo> list = BuildMarketBenchmarks(frame.ItemID, avgPrice, frame.NPCGilPrice);
			foreach (UniversalisMarketListing listing in dataset.Listings)
			{
				foreach (MarketBenchmarkInfo item in list)
				{
					if (!item.Drawn && listing.PricePerUnit > item.Price)
					{
						DrawBenchmarkSeparatorRow($"OnlineBenchmark_{item.Price}_{item.Badge}", item.Badge, item.Color, item.Tooltip, item.OnClick);
						item.Drawn = true;
					}
				}
				ImU8String val2 = new ImU8String(0, 1);
				((ImU8String)(ref val2)).AppendFormatted<long>(listing.ListingID);
				IdDisposable val3 = ImRaii.PushId(val2, true);
				try
				{
					ImGui.TableNextRow();
					if (isAnyHQ)
					{
						ImGui.TableNextColumn();
						ImGui.TextUnformatted(ImU8String.op_Implicit(listing.HQ ? "√" : string.Empty));
					}
					if (isAnyOnMannequin)
					{
						ImGui.TableNextColumn();
						ImGui.TextUnformatted(ImU8String.op_Implicit(listing.OnMannequin ? "√" : string.Empty));
					}
					ImGui.TableNextColumn();
					DrawMarketPrice(listing.PricePerUnit);
					ImGui.TableNextColumn();
					ImU8String val4 = new ImU8String(0, 1);
					((ImU8String)(ref val4)).AppendFormatted<uint>(listing.Quantity);
					ImGui.TextUnformatted(val4);
					ImGui.TableNextColumn();
					ImU8String val5 = new ImU8String(1, 1);
					((ImU8String)(ref val5)).AppendFormatted<string>(listing.Total.ToChineseString());
					((ImU8String)(ref val5)).AppendLiteral("\ue049");
					ImGui.TextUnformatted(val5);
					ImGui.TableNextColumn();
					ImU8String val6 = new ImU8String(0, 1);
					((ImU8String)(ref val6)).AppendFormatted<string>(listing.RetainerName);
					ImGui.TextUnformatted(val6);
				}
				finally
				{
					((IDisposable)val3)?.Dispose();
				}
			}
			foreach (MarketBenchmarkInfo item2 in list)
			{
				if (!item2.Drawn)
				{
					DrawBenchmarkSeparatorRow($"OnlineBenchmark_End_{item2.Price}_{item2.Badge}", item2.Badge, item2.Color, item2.Tooltip, item2.OnClick);
					item2.Drawn = true;
				}
			}
		}
		finally
		{
			((TableDisposable)(ref val)).Dispose();
		}
	}

	private static void DrawOnlineMarketHistoryDataTable(HistoryDataSet data)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		List<HistoryEntry> entries = data.Entries;
		bool isAnyHQ = data.IsAnyHQ;
		int num = 4;
		if (!isAnyHQ)
		{
			num--;
		}
		TableDisposable val = ImRaii.Table(ImU8String.op_Implicit("UniversalisHistoryDataTable"), num, (ImGuiTableFlags)33556352, new Vector2(-1f, ImGui.GetContentRegionAvail().Y));
		try
		{
			if (!val)
			{
				return;
			}
			ImGui.TableSetupScrollFreeze(0, 1);
			if (isAnyHQ)
			{
				ImGui.TableSetupColumn(ImU8String.op_Implicit("\ue03c"), (ImGuiTableColumnFlags)8, ImGui.CalcTextSize(ImU8String.op_Implicit("\ue03c"), false, -1f).X, 0u);
			}
			ImGui.TableSetupColumn(ImU8String.op_Implicit(LanguageManager.Get("Amount")), (ImGuiTableColumnFlags)8, ImGui.CalcTextSize(ImU8String.op_Implicit("12345678"), false, -1f).X, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(357u)), (ImGuiTableColumnFlags)4, 15f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(1976u)), (ImGuiTableColumnFlags)4, 15f, 0u);
			ImGui.TableHeadersRow();
			foreach (HistoryEntry item in entries)
			{
				if (item.PricePerUnit == 0L)
				{
					continue;
				}
				ImU8String val2 = new ImU8String(2, 3);
				((ImU8String)(ref val2)).AppendFormatted<long>(item.SaleTime.Ticks);
				((ImU8String)(ref val2)).AppendLiteral("-");
				((ImU8String)(ref val2)).AppendFormatted<ulong>(item.PricePerUnit);
				((ImU8String)(ref val2)).AppendLiteral("-");
				((ImU8String)(ref val2)).AppendFormatted<uint>(item.Quantity);
				IdDisposable val3 = ImRaii.PushId(val2, true);
				try
				{
					ImGui.TableNextRow();
					if (isAnyHQ)
					{
						ImGui.TableNextColumn();
						ImGui.TextUnformatted(ImU8String.op_Implicit(item.IsHQ ? "√" : string.Empty));
					}
					ImGui.TableNextColumn();
					ImU8String val4 = new ImU8String(0, 1);
					((ImU8String)(ref val4)).AppendFormatted<uint>(item.Quantity);
					ImGui.TextUnformatted(val4);
					ImGui.TableNextColumn();
					ImU8String val5 = new ImU8String(1, 1);
					((ImU8String)(ref val5)).AppendFormatted<string>(item.PricePerUnit.ToChineseString());
					((ImU8String)(ref val5)).AppendLiteral("\ue049");
					ImGui.TextUnformatted(val5);
					ImGuiOmExtension.ClickToCopyAndNotify(item.PricePerUnit.ToString());
					ImGui.TableNextColumn();
					ImGui.TextUnformatted(ImU8String.op_Implicit(item.SaleTime.ToString("yyyy-MM-dd HH:mm:ss")));
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

	private unsafe void DrawMarketPriceTrend(MarketBoardUIContext frame)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_049a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c86: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c8b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d0a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d1e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d77: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d7c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dbd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d9c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dd3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0df2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e0b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e10: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e31: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e3a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e3f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e5e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e6c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e71: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e81: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e8a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e8f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ec7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ecc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ef6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f04: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f09: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f28: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f36: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f3b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f7b: Unknown result type (might be due to invalid IL or missing references)
		uint itemID = frame.ItemID;
		if (itemID == 0)
		{
			return;
		}
		TrendDataSet trendDataSet = provider.GetTrendDataSet(itemID);
		if (trendDataSet == null || trendDataSet.Entries.Count == 0)
		{
			ImGui.TextDisabled(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(1998u)));
			return;
		}
		float item = provider.GetItemAggregatedStats(itemID, frame.SelectedWorldID, frame.HQOnly).DailySales;
		ulong number = trendDataSet.Entries.Min((HistoryEntry historyEntry3) => historyEntry3.PricePerUnit);
		ulong number2 = trendDataSet.Entries.Max((HistoryEntry historyEntry3) => historyEntry3.PricePerUnit);
		List<MetricItem> list = new List<MetricItem>
		{
			new MetricItem(LanguageManager.Get("BetterMarketBoard-Trend-Max"), number2.ToChineseString() + "\ue049", KnownColor.Salmon.ToVector4(), null),
			new MetricItem(LanguageManager.Get("BetterMarketBoard-Trend-Min"), number.ToChineseString() + "\ue049", KnownColor.GreenYellow.ToVector4(), null)
		};
		if (trendDataSet.IsCanBeHQ)
		{
			if (trendDataSet.AvgNQPrice != 0)
			{
				list.Add(new MetricItem(LanguageManager.Get("BetterMarketBoard-AveragePrice-NQ"), trendDataSet.AvgNQPrice.ToChineseString() + "\ue049", KnownColor.Chocolate.ToVector4(), null));
			}
			if (trendDataSet.AvgHQPrice != 0)
			{
				list.Add(new MetricItem(LanguageManager.Get("BetterMarketBoard-AveragePrice-HQ"), trendDataSet.AvgHQPrice.ToChineseString() + "\ue049", KnownColor.Orange.ToVector4(), null));
			}
		}
		else
		{
			list.Add(new MetricItem(LanguageManager.Get("BetterMarketBoard-AveragePrice"), trendDataSet.AvgHistoryPrice.ToChineseString() + "\ue049", KnownColor.Orange.ToVector4(), null));
		}
		list.Add(new MetricItem(LanguageManager.Get("BetterMarketBoard-Trend-Volume"), LanguageManager.Get("BetterMarketBoard-Samples-Format", trendDataSet.Entries.Count, trendDataSet.Entries.Sum((HistoryEntry historyEntry3) => historyEntry3.Quantity)), KnownColor.Chocolate.ToVector4(), null));
		if (item > 0f)
		{
			list.Add(new MetricItem(LanguageManager.Get("BetterMarketBoard-Tooltip-DailySales"), LanguageManager.Get("BetterMarketBoard-DailySales-Format", item.ToString("0.#", CultureInfo.InvariantCulture)), KnownColor.White.ToVector4(), null));
		}
		DrawMetricDashboard(list);
		ImGui.Spacing();
		double xMin = trendDataSet.XMin;
		double xMax = trendDataSet.XMax;
		double visualMin = trendDataSet.VisualMin;
		double visualMax = trendDataSet.VisualMax;
		PlotColorDisposable val = ImRaii.PushColor((ImPlotCol)16, new Vector4(0.02f, 0.02f, 0.02f, 0.5f), true);
		try
		{
			PlotColorDisposable val2 = ImRaii.PushColor((ImPlotCol)5, new Vector4(0.04f, 0.04f, 0.04f, 0.3f), true);
			try
			{
				PlotColorDisposable val3 = ImRaii.PushColor((ImPlotCol)14, new Vector4(1f, 1f, 1f, 0.06f), true);
				try
				{
					PlotColorDisposable val4 = ImRaii.PushColor((ImPlotCol)8, new Vector4(0.05f, 0.05f, 0.05f, 0.85f), true);
					try
					{
						PlotColorDisposable val5 = ImRaii.PushColor((ImPlotCol)9, new Vector4(1f, 1f, 1f, 0.15f), true);
						try
						{
							PlotStyleDisposable val6 = ImRaii.PushStyle((ImPlotStyleVar)0, 2f, true);
							try
							{
								PlotStyleDisposable val7 = ImRaii.PushStyle((ImPlotStyleVar)10, 0.2f, true);
								try
								{
									PlotDisposable val8 = ImRaii.Plot($"##MarketHistoryPriceTrend_{itemID}_{provider.SelectedWorldID}", new Vector2(-1f, ImGui.GetContentRegionAvail().Y), (ImPlotFlags)5);
									try
									{
										if (!val8)
										{
											return;
										}
										ImPlot.SetupAxes((byte*)null, (byte*)null, (ImPlotAxisFlags)0, (ImPlotAxisFlags)0);
										ImPlot.SetupAxesLimits(xMin, xMax, visualMin, visualMax, (ImPlotCond)2);
										DateTime localDateTime = DateTimeOffset.FromUnixTimeSeconds((long)xMin).LocalDateTime;
										DateTime localDateTime2 = DateTimeOffset.FromUnixTimeSeconds((long)xMax).LocalDateTime;
										bool isCrossYear = localDateTime.Year != localDateTime2.Year;
										double num = xMax - xMin;
										double totalDays = (localDateTime2.Date - localDateTime.Date).TotalDays;
										float x = ImGui.GetContentRegionAvail().X;
										float num2 = ImGui.CalcTextSize(ImU8String.op_Implicit(localDateTime.ToString(GetTickFormat(compact: false))), false, -1f).X + 30f * Globals.GlobalUIScale;
										int num3 = Math.Clamp((int)(x / num2), 2, 5);
										string text = GetTickFormat(num3 <= 3);
										double[] array = new double[num3];
										string[] array2 = new string[num3];
										for (int num4 = 0; num4 < num3; num4++)
										{
											array2[num4] = DateTimeOffset.FromUnixTimeSeconds((long)(array[num4] = xMin + num * (double)num4 / (double)(num3 - 1))).LocalDateTime.ToString(text);
										}
										ImPlot.SetupAxisTicks((ImAxis)0, ref array[0], num3, array2, false);
										int num5 = Math.Clamp((int)(ImGui.GetContentRegionAvail().Y / (35f * Globals.GlobalUIScale)), 3, 5);
										double[] array3 = new double[num5];
										string[] array4 = new string[num5];
										double num6 = visualMax - visualMin;
										for (int num7 = 0; num7 < num5; num7++)
										{
											array4[num7] = ((ulong)Math.Round(array3[num7] = visualMin + num6 * (double)num7 / (double)(num5 - 1))).ToChineseString();
										}
										ImPlot.SetupAxisTicks((ImAxis)3, ref array3[0], num5, array4, false);
										ImPlotPlotPtr currentPlot = ImPlot.GetCurrentPlot();
										ImPlotItem* item2 = ((ImPlotItemGroup)(ref ((ImPlotPlotPtr)(ref currentPlot)).Items)).GetItem(LanguageManager.Get("NQ"));
										ImPlotItem* item3 = ((ImPlotItemGroup)(ref ((ImPlotPlotPtr)(ref currentPlot)).Items)).GetItem(LanguageManager.Get("HQ"));
										bool flag = item2 == null || ((ImPlotItem)item2).Show != 0;
										bool flag2 = item3 == null || ((ImPlotItem)item3).Show != 0;
										List<HistoryEntry> entries = trendDataSet.Entries;
										if (trendDataSet.IsCanBeHQ)
										{
											int hQCount = trendDataSet.HQCount;
											int nQCount = trendDataSet.NQCount;
											double[] array5 = new double[hQCount];
											double[] array6 = new double[hQCount];
											double[] array7 = new double[nQCount];
											double[] array8 = new double[nQCount];
											int num8 = 0;
											int num9 = 0;
											foreach (HistoryEntry item4 in entries)
											{
												if (item4.IsHQ)
												{
													array5[num8] = item4.X;
													array6[num8] = item4.PricePerUnit;
													num8++;
												}
												else
												{
													array7[num9] = item4.X;
													array8[num9] = item4.PricePerUnit;
													num9++;
												}
											}
											if (trendDataSet.AvgNQPrice != 0 && flag)
											{
												double[] array9 = new double[2] { xMin, xMax };
												double[] array10 = new double[2] { trendDataSet.AvgNQPrice, trendDataSet.AvgNQPrice };
												PlotColorDisposable val9 = ImRaii.PushColor((ImPlotCol)0, VectorExtensions.WithW(KnownColor.Chocolate.ToVector4(), 0.35f), true);
												try
												{
													PlotStyleDisposable val10 = ImRaii.PushStyle((ImPlotStyleVar)0, 1.2f, true);
													try
													{
														ImPlot.PlotLine(LanguageManager.Get("BetterMarketBoard-AveragePrice-NQ"), ref array9[0], ref array10[0], 2);
													}
													finally
													{
														((IDisposable)val10)?.Dispose();
													}
												}
												finally
												{
													((IDisposable)val9)?.Dispose();
												}
												ImPlot.TagY((double)trendDataSet.AvgNQPrice, KnownColor.Chocolate.ToVector4(), trendDataSet.AvgNQPrice.ToChineseString() + "\ue049");
											}
											if (trendDataSet.AvgHQPrice != 0 && flag2)
											{
												double[] array11 = new double[2] { xMin, xMax };
												double[] array12 = new double[2] { trendDataSet.AvgHQPrice, trendDataSet.AvgHQPrice };
												PlotColorDisposable val9 = ImRaii.PushColor((ImPlotCol)0, VectorExtensions.WithW(KnownColor.Orange.ToVector4(), 0.35f), true);
												try
												{
													PlotStyleDisposable val10 = ImRaii.PushStyle((ImPlotStyleVar)0, 1.2f, true);
													try
													{
														ImPlot.PlotLine(LanguageManager.Get("BetterMarketBoard-AveragePrice-HQ"), ref array11[0], ref array12[0], 2);
													}
													finally
													{
														((IDisposable)val10)?.Dispose();
													}
												}
												finally
												{
													((IDisposable)val9)?.Dispose();
												}
												ImPlot.TagY((double)trendDataSet.AvgHQPrice, KnownColor.Orange.ToVector4(), trendDataSet.AvgHQPrice.ToChineseString() + "\ue049");
											}
											if (nQCount > 0)
											{
												PlotColorDisposable val9;
												if (flag)
												{
													val9 = ImRaii.PushColor((ImPlotCol)1, VectorExtensions.WithW(KnownColor.Chocolate.ToVector4(), 0.12f), true);
													try
													{
														ImPlot.PlotShaded("##NQShade", ref array7[0], ref array8[0], nQCount, visualMin);
													}
													finally
													{
														((IDisposable)val9)?.Dispose();
													}
												}
												val9 = ImRaii.PushColor((ImPlotCol)0, KnownColor.Chocolate.ToVector4(), true);
												try
												{
													PlotColorDisposable val11 = ImRaii.PushColor((ImPlotCol)3, KnownColor.Chocolate.ToVector4(), true);
													try
													{
														PlotColorDisposable val12 = ImRaii.PushColor((ImPlotCol)2, VectorExtensions.WithW(KnownColor.White.ToVector4(), 0.6f), true);
														try
														{
															ImPlot.SetNextMarkerStyle((ImPlotMarker)0, 3.5f * Globals.GlobalUIScale);
															ImPlot.PlotLine(LanguageManager.Get("NQ"), ref array7[0], ref array8[0], nQCount);
														}
														finally
														{
															((IDisposable)val12)?.Dispose();
														}
													}
													finally
													{
														((IDisposable)val11)?.Dispose();
													}
												}
												finally
												{
													((IDisposable)val9)?.Dispose();
												}
											}
											if (hQCount > 0)
											{
												PlotColorDisposable val9;
												if (flag2)
												{
													val9 = ImRaii.PushColor((ImPlotCol)1, VectorExtensions.WithW(KnownColor.Orange.ToVector4(), 0.12f), true);
													try
													{
														ImPlot.PlotShaded("##HQShade", ref array5[0], ref array6[0], hQCount, visualMin);
													}
													finally
													{
														((IDisposable)val9)?.Dispose();
													}
												}
												val9 = ImRaii.PushColor((ImPlotCol)0, KnownColor.Orange.ToVector4(), true);
												try
												{
													PlotColorDisposable val11 = ImRaii.PushColor((ImPlotCol)3, KnownColor.Orange.ToVector4(), true);
													try
													{
														PlotColorDisposable val12 = ImRaii.PushColor((ImPlotCol)2, VectorExtensions.WithW(KnownColor.White.ToVector4(), 0.6f), true);
														try
														{
															ImPlot.SetNextMarkerStyle((ImPlotMarker)0, 4f * Globals.GlobalUIScale);
															ImPlot.PlotLine(LanguageManager.Get("HQ"), ref array5[0], ref array6[0], hQCount);
														}
														finally
														{
															((IDisposable)val12)?.Dispose();
														}
													}
													finally
													{
														((IDisposable)val11)?.Dispose();
													}
												}
												finally
												{
													((IDisposable)val9)?.Dispose();
												}
											}
										}
										else
										{
											PlotColorDisposable val9;
											if (trendDataSet.AvgHistoryPrice != 0 && flag)
											{
												double[] array13 = new double[2] { xMin, xMax };
												double[] array14 = new double[2] { trendDataSet.AvgHistoryPrice, trendDataSet.AvgHistoryPrice };
												val9 = ImRaii.PushColor((ImPlotCol)0, VectorExtensions.WithW(KnownColor.Orange.ToVector4(), 0.35f), true);
												try
												{
													PlotStyleDisposable val10 = ImRaii.PushStyle((ImPlotStyleVar)0, 1.2f, true);
													try
													{
														ImPlot.PlotLine(LanguageManager.Get("BetterMarketBoard-AveragePrice"), ref array13[0], ref array14[0], 2);
													}
													finally
													{
														((IDisposable)val10)?.Dispose();
													}
												}
												finally
												{
													((IDisposable)val9)?.Dispose();
												}
												ImPlot.TagY((double)trendDataSet.AvgHistoryPrice, KnownColor.Orange.ToVector4(), trendDataSet.AvgHistoryPrice.ToChineseString() + "\ue049");
											}
											double[] array15 = new double[entries.Count];
											double[] array16 = new double[entries.Count];
											for (int num10 = 0; num10 < entries.Count; num10++)
											{
												array15[num10] = entries[num10].X;
												array16[num10] = entries[num10].PricePerUnit;
											}
											if (flag)
											{
												val9 = ImRaii.PushColor((ImPlotCol)1, VectorExtensions.WithW(KnownColor.Chocolate.ToVector4(), 0.12f), true);
												try
												{
													ImPlot.PlotShaded("##PriceShade", ref array15[0], ref array16[0], entries.Count, visualMin);
												}
												finally
												{
													((IDisposable)val9)?.Dispose();
												}
											}
											val9 = ImRaii.PushColor((ImPlotCol)0, KnownColor.Chocolate.ToVector4(), true);
											try
											{
												PlotColorDisposable val11 = ImRaii.PushColor((ImPlotCol)3, KnownColor.Chocolate.ToVector4(), true);
												try
												{
													PlotColorDisposable val12 = ImRaii.PushColor((ImPlotCol)2, VectorExtensions.WithW(KnownColor.White.ToVector4(), 0.6f), true);
													try
													{
														ImPlot.SetNextMarkerStyle((ImPlotMarker)0, 3.5f * Globals.GlobalUIScale);
														ImPlot.PlotLine(LanguageManager.Get("NQ"), ref array15[0], ref array16[0], entries.Count);
													}
													finally
													{
														((IDisposable)val12)?.Dispose();
													}
												}
												finally
												{
													((IDisposable)val11)?.Dispose();
												}
											}
											finally
											{
												((IDisposable)val9)?.Dispose();
											}
										}
										if (!ImPlot.IsPlotHovered())
										{
											return;
										}
										ImPlotPoint plotMousePos = ImPlot.GetPlotMousePos();
										double num11 = xMax - xMin;
										double num12 = visualMax - visualMin;
										if (num11 <= 0.0)
										{
											num11 = 1.0;
										}
										if (num12 <= 0.0)
										{
											num12 = 1.0;
										}
										double num13 = double.MaxValue;
										int num14 = -1;
										for (int num15 = 0; num15 < entries.Count; num15++)
										{
											HistoryEntry historyEntry = entries[num15];
											if ((!historyEntry.IsHQ || flag2) && (historyEntry.IsHQ || flag))
											{
												double num16 = (historyEntry.X - plotMousePos.X) / num11;
												double num17 = ((double)historyEntry.PricePerUnit - plotMousePos.Y) / num12;
												double num18 = num16 * num16 + num17 * num17;
												if (num18 < num13)
												{
													num13 = num18;
													num14 = num15;
												}
											}
										}
										if (num14 < 0 || !(num13 < 0.05))
										{
											return;
										}
										HistoryEntry historyEntry2 = entries[num14];
										TooltipDisposable val13 = ImRaii.Tooltip();
										try
										{
											Vector4 vector;
											if (historyEntry2.IsHQ)
											{
												vector = KnownColor.Orange.ToVector4();
												ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("HQ")));
											}
											else
											{
												vector = KnownColor.Chocolate.ToVector4();
												ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("NQ")));
											}
											ImGui.Separator();
											ImU8String val14 = new ImU8String(2, 1);
											((ImU8String)(ref val14)).AppendFormatted<string>(LuminaWrapper.GetAddonText(357u));
											((ImU8String)(ref val14)).AppendLiteral(": ");
											ImGui.TextDisabled(val14);
											ImGui.SameLine();
											vector = KnownColor.Orange.ToVector4();
											ImU8String val15 = new ImU8String(1, 1);
											((ImU8String)(ref val15)).AppendFormatted<string>(historyEntry2.PricePerUnit.ToChineseString());
											((ImU8String)(ref val15)).AppendLiteral("\ue049");
											ImGui.TextColored(ref vector, val15);
											ImU8String val16 = new ImU8String(2, 1);
											((ImU8String)(ref val16)).AppendFormatted<string>(LanguageManager.Get("Amount"));
											((ImU8String)(ref val16)).AppendLiteral(": ");
											ImGui.TextDisabled(val16);
											ImGui.SameLine();
											ImU8String val17 = new ImU8String(0, 1);
											((ImU8String)(ref val17)).AppendFormatted<uint>(historyEntry2.Quantity);
											ImGui.TextUnformatted(val17);
											ImU8String val18 = new ImU8String(2, 1);
											((ImU8String)(ref val18)).AppendFormatted<string>(LuminaWrapper.GetAddonText(6936u));
											((ImU8String)(ref val18)).AppendLiteral(": ");
											ImGui.TextDisabled(val18);
											ImGui.SameLine();
											vector = KnownColor.GreenYellow.ToVector4();
											ImU8String val19 = new ImU8String(1, 1);
											((ImU8String)(ref val19)).AppendFormatted<string>((historyEntry2.PricePerUnit * historyEntry2.Quantity).ToChineseString());
											((ImU8String)(ref val19)).AppendLiteral("\ue049");
											ImGui.TextColored(ref vector, val19);
											ImGui.Separator();
											ImU8String val20 = new ImU8String(2, 1);
											((ImU8String)(ref val20)).AppendFormatted<string>(LuminaWrapper.GetAddonText(1976u));
											((ImU8String)(ref val20)).AppendLiteral(": ");
											ImGui.TextDisabled(val20);
											ImGui.SameLine();
											ImU8String val21 = new ImU8String(3, 2);
											((ImU8String)(ref val21)).AppendFormatted<DateTime>(historyEntry2.SaleTime, "yyyy-MM-dd HH:mm");
											((ImU8String)(ref val21)).AppendLiteral(" (");
											((ImU8String)(ref val21)).AppendFormatted<string>(historyEntry2.SaleTime.TimeAgo());
											((ImU8String)(ref val21)).AppendLiteral(")");
											ImGui.TextUnformatted(val21);
										}
										finally
										{
											((TooltipDisposable)(ref val13)).Dispose();
										}
										string GetTickFormat(bool compact)
										{
											if (isCrossYear)
											{
												if (!(totalDays > 365.0))
												{
													if (!compact)
													{
														return "yyyy-MM-dd";
													}
													return "yy-MM-dd";
												}
												return "yyyy-MM";
											}
											if (totalDays >= 7.0)
											{
												return "MM-dd";
											}
											if (totalDays >= 1.0)
											{
												if (!compact)
												{
													return "MM-dd HH:mm";
												}
												return "MM-dd";
											}
											return "HH:mm";
										}
									}
									finally
									{
										((PlotDisposable)(ref val8)).Dispose();
									}
								}
								finally
								{
									((IDisposable)val7)?.Dispose();
								}
							}
							finally
							{
								((IDisposable)val6)?.Dispose();
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

	private static void DrawMetricDashboard(IReadOnlyList<MetricItem> metrics, float? customWidth = null)
	{
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		if (metrics.Count == 0)
		{
			return;
		}
		float num = customWidth ?? ImGui.GetContentRegionAvail().X;
		float num2 = 6f * Globals.GlobalUIScale;
		float num3 = num / (float)metrics.Count;
		float textLineHeight;
		using (OmenServiceBase<FontManager>.Instance().UIFont60.Push())
		{
			textLineHeight = ImGui.GetTextLineHeight();
		}
		float textLineHeight2;
		using (OmenServiceBase<FontManager>.Instance().UIFont80.Push())
		{
			textLineHeight2 = ImGui.GetTextLineHeight();
		}
		float num4 = num2 * 2f + textLineHeight + textLineHeight2 + 3f * Globals.GlobalUIScale;
		Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
		ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
		Vector2 vector = cursorScreenPos;
		Vector2 vector2 = cursorScreenPos + new Vector2(num, num4);
		((ImDrawListPtr)(ref windowDrawList)).AddRectFilled(vector, vector2, ImGui.GetColorU32((ImGuiCol)7, 0.45f), 4f * Globals.GlobalUIScale);
		((ImDrawListPtr)(ref windowDrawList)).AddRect(vector, vector2, ImGui.GetColorU32((ImGuiCol)5, 0.25f), 4f * Globals.GlobalUIScale);
		for (int i = 0; i < metrics.Count; i++)
		{
			MetricItem metricItem = metrics[i];
			Vector2 vector3 = vector + new Vector2((float)i * num3, 0f);
			Vector2 vector4 = vector3 + new Vector2(num3, num4);
			if (i > 0)
			{
				float x = vector.X + (float)i * num3;
				((ImDrawListPtr)(ref windowDrawList)).AddLine(new Vector2(x, vector.Y + num4 * 0.2f), new Vector2(x, vector2.Y - num4 * 0.2f), ImGui.GetColorU32((ImGuiCol)5, 0.35f));
			}
			bool flag = !string.IsNullOrEmpty(metricItem.Tooltip);
			string text = metricItem.Label + (flag ? " (?)" : string.Empty);
			float x2;
			using (OmenServiceBase<FontManager>.Instance().UIFont60.Push())
			{
				x2 = ImGui.CalcTextSize(ImU8String.op_Implicit(text), false, -1f).X;
			}
			float x3 = vector3.X + MathF.Max(0f, (num3 - x2) / 2f);
			using (OmenServiceBase<FontManager>.Instance().UIFont60.Push())
			{
				((ImDrawListPtr)(ref windowDrawList)).AddText(new Vector2(x3, vector3.Y + num2), ImGui.GetColorU32((ImGuiCol)1), ImU8String.op_Implicit(text));
			}
			using (OmenServiceBase<FontManager>.Instance().UIFont80.Push())
			{
				Vector2 vector5 = ImGui.CalcTextSize(ImU8String.op_Implicit(metricItem.Value), false, -1f);
				Vector2 vector6 = new Vector2(vector3.X + MathF.Max(0f, (num3 - vector5.X) / 2f), vector3.Y + num2 + textLineHeight + 2f * Globals.GlobalUIScale);
				((ImDrawListPtr)(ref windowDrawList)).AddText(vector6, ImGui.GetColorU32(metricItem.ValueColor), ImU8String.op_Implicit(metricItem.Value));
			}
			if (!flag || !ImGui.IsMouseHoveringRect(vector3, vector4))
			{
				continue;
			}
			TooltipDisposable val = ImRaii.Tooltip();
			try
			{
				TextWrapDisposable val2 = ImRaii.TextWrapPos(ImGui.GetFontSize() * 40f);
				try
				{
					ImGui.TextUnformatted(ImU8String.op_Implicit(metricItem.Tooltip));
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
			}
			finally
			{
				((TooltipDisposable)(ref val)).Dispose();
			}
		}
		ImGui.Dummy(new Vector2(num, num4));
	}

	private static void DrawMarketPrice(ulong price)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(1, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(price.ToChineseString());
		((ImU8String)(ref val)).AppendLiteral("\ue049");
		ImGui.TextUnformatted(val);
		ImGuiOmExtension.ClickToCopyAndNotify(price.ToString());
	}

	private static List<MarketBenchmarkInfo> BuildMarketBenchmarks(uint itemID, ulong? avgPrice, uint? npcGilPrice)
	{
		List<MarketBenchmarkInfo> list = new List<MarketBenchmarkInfo>();
		bool num = avgPrice.HasValue && avgPrice.GetValueOrDefault() != 0;
		bool flag = npcGilPrice.HasValue && npcGilPrice.GetValueOrDefault() != 0;
		if (num)
		{
			if (flag && avgPrice.Value == npcGilPrice.Value)
			{
				list.Add(new MarketBenchmarkInfo
				{
					Price = avgPrice.Value,
					Badge = LanguageManager.Get("BetterMarketBoard-AveragePrice") + " & " + LanguageManager.Get("BetterMarketBoard-NPCPrice", avgPrice.Value.ToChineseString() + "\ue049"),
					Color = KnownColor.DarkOrange.ToVector4(),
					Tooltip = $"{LanguageManager.Get("BetterMarketBoard-AveragePrice")}：{avgPrice.Value.ToChineseString()}\ue049\n{LanguageManager.Get("BetterMarketBoard-NPCPrice", npcGilPrice.Value.ToChineseString() + "\ue049")}\n{LanguageManager.Get("BetterMarketBoard-NPCShopInfo")}",
					OnClick = delegate
					{
						OpenShopListByItemIDIPC.InvokeFunc(itemID);
					}
				});
				return list;
			}
			list.Add(new MarketBenchmarkInfo
			{
				Price = avgPrice.Value,
				Badge = LanguageManager.Get("BetterMarketBoard-AveragePrice") + "：" + avgPrice.Value.ToChineseString() + "\ue049",
				Color = KnownColor.Chocolate.ToVector4()
			});
		}
		if (flag)
		{
			list.Add(new MarketBenchmarkInfo
			{
				Price = npcGilPrice.Value,
				Badge = (LanguageManager.Get("BetterMarketBoard-NPCPrice", npcGilPrice.Value.ToChineseString() + "\ue049") ?? ""),
				Color = KnownColor.OrangeRed.ToVector4(),
				Tooltip = LanguageManager.Get("BetterMarketBoard-NPCShopInfo"),
				OnClick = delegate
				{
					OpenShopListByItemIDIPC.InvokeFunc(itemID);
				}
			});
		}
		list.Sort((MarketBenchmarkInfo a, MarketBenchmarkInfo b) => a.Price.CompareTo(b.Price));
		return list;
	}

	private static void DrawBenchmarkSeparatorRow(string id, string badgeLabel, Vector4 accentColor, string? tooltip = null, Action? onClick = null)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TableNextRow();
		ImGui.TableNextColumn();
		float y = ImGui.GetTextLineHeight() * 1.15f + 6f * Globals.GlobalUIScale;
		ImGuiSelectableFlags val = (ImGuiSelectableFlags)2;
		if (onClick == null)
		{
			val = (ImGuiSelectableFlags)(val | 8);
		}
		ImU8String val2 = default(ImU8String);
		((ImU8String)(ref val2))._002Ector(3, 1);
		((ImU8String)(ref val2)).AppendLiteral("###");
		((ImU8String)(ref val2)).AppendFormatted<string>(id);
		if (ImGui.Selectable(val2, false, val, new Vector2(0f, y)))
		{
			onClick?.Invoke();
		}
		bool flag = ImGui.IsItemHovered();
		if (flag && onClick != null)
		{
			ImGui.SetMouseCursor((ImGuiMouseCursor)7);
		}
		Vector2 itemRectMin = ImGui.GetItemRectMin();
		Vector2 itemRectMax = ImGui.GetItemRectMax();
		ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
		float num = 3f * Globals.GlobalUIScale;
		Vector2 clipRectMin = ImGui.GetClipRectMin(windowDrawList);
		Vector2 clipRectMax = ImGui.GetClipRectMax(windowDrawList);
		Vector2 vector = itemRectMin;
		vector.Y = MathF.Max(itemRectMin.Y, clipRectMin.Y);
		Vector2 vector2 = vector;
		vector = itemRectMax;
		vector.Y = MathF.Min(itemRectMax.Y, clipRectMax.Y);
		Vector2 vector3 = vector;
		if (vector2.Y >= vector3.Y || vector2.X >= vector3.X)
		{
			return;
		}
		((ImDrawListPtr)(ref windowDrawList)).PushClipRect(vector2, vector3, false);
		float num2 = ((flag && onClick != null) ? 0.16f : 0.08f);
		uint num3 = VectorExtensions.WithW(accentColor, num2).ToUInt();
		((ImDrawListPtr)(ref windowDrawList)).AddRectFilled(itemRectMin, itemRectMax, num3, num);
		float num4 = itemRectMin.Y + (itemRectMax.Y - itemRectMin.Y) / 2f;
		float num5 = 8f * Globals.GlobalUIScale;
		using (OmenServiceBase<FontManager>.Instance().UIFont60.Push())
		{
			Vector2 vector4 = ImGui.CalcTextSize(ImU8String.op_Implicit(badgeLabel), false, -1f);
			float num6 = 6f * Globals.GlobalUIScale;
			float num7 = 2f * Globals.GlobalUIScale;
			Vector2 vector5 = new Vector2(itemRectMin.X + num5, num4 - vector4.Y / 2f - num7);
			Vector2 vector6 = new Vector2(vector5.X + vector4.X + num6 * 2f, num4 + vector4.Y / 2f + num7);
			float num8 = ((flag && onClick != null) ? 0.35f : 0.22f);
			float num9 = ((flag && onClick != null) ? 1f : 0.75f);
			((ImDrawListPtr)(ref windowDrawList)).AddRectFilled(vector5, vector6, VectorExtensions.WithW(accentColor, num8).ToUInt(), num);
			((ImDrawListPtr)(ref windowDrawList)).AddRect(vector5, vector6, VectorExtensions.WithW(accentColor, num9).ToUInt(), num, (ImDrawFlags)0, 1f * Globals.GlobalUIScale);
			((ImDrawListPtr)(ref windowDrawList)).AddText(new Vector2(vector5.X + num6, num4 - vector4.Y / 2f), accentColor.ToUInt(), ImU8String.op_Implicit(badgeLabel));
			float num10 = vector6.X + 6f * Globals.GlobalUIScale;
			float num11 = itemRectMax.X - num5;
			if (num11 > num10)
			{
				float num12 = ((flag && onClick != null) ? 0.6f : 0.35f);
				((ImDrawListPtr)(ref windowDrawList)).AddLine(new Vector2(num10, num4), new Vector2(num11, num4), VectorExtensions.WithW(accentColor, num12).ToUInt(), 1.5f * Globals.GlobalUIScale);
			}
		}
		((ImDrawListPtr)(ref windowDrawList)).PopClipRect();
		if (!string.IsNullOrEmpty(tooltip) && flag)
		{
			ImGui.SetTooltip(ImU8String.op_Implicit(tooltip));
		}
	}

	private void DrawAllWorldsPriceTable(MarketBoardUIContext frame)
	{
		if (allWorlds.Count == 0)
		{
			return;
		}
		WorldPriceRanks worldPriceRanks = provider.GetWorldPriceRanks(frame.ItemID);
		string effectiveRegionName = provider.EffectiveRegionName;
		DrawAllWorldPricesToggleComponent((ImGuiDir)(isAllWorldsPriceExpanded ? 2 : 3));
		if (worldPriceRanks == null || worldPriceRanks.Valid.Count == 0)
		{
			if (frame.ItemID != 0 && !string.IsNullOrEmpty(effectiveRegionName))
			{
				ImGui.Spacing();
				ColorDisposable val = ImRaii.PushColor((ImGuiCol)0, KnownColor.Gray.ToUInt(), true);
				try
				{
					ImGuiOm.TextCentered("\ue031 " + LuminaWrapper.GetAddonText(2717u));
				}
				finally
				{
					((IDisposable)val)?.Dispose();
				}
			}
		}
		else if (isAllWorldsPriceExpanded)
		{
			DrawFullPriceTable(frame, effectiveRegionName);
		}
		else
		{
			DrawPriceOverviewTable(frame, effectiveRegionName, worldPriceRanks.Current.MinPrice, worldPriceRanks.Expensive, worldPriceRanks.Cheapest);
		}
	}

	private void DrawPriceOverviewTable(MarketBoardUIContext frame, string displayRegionName, ulong currentWorldPrice, IReadOnlyList<RankedWorldPriceRow> expensiveWorlds, IReadOnlyList<RankedWorldPriceRow> cheapestWorlds)
	{
		if (expensiveWorlds.Count + cheapestWorlds.Count == 0)
		{
			return;
		}
		using (OmenServiceBase<FontManager>.Instance().UIFont80.Push())
		{
			float x = ImGui.GetContentRegionAvail().X;
			float num = 4f * Globals.GlobalUIScale;
			float num2 = 8f * Globals.GlobalUIScale;
			float textLineHeight;
			using (OmenServiceBase<FontManager>.Instance().UIFont60.Push())
			{
				textLineHeight = ImGui.GetTextLineHeight();
			}
			float textLineHeight2;
			using (OmenServiceBase<FontManager>.Instance().UIFont80.Push())
			{
				textLineHeight2 = ImGui.GetTextLineHeight();
			}
			float num3 = 3.5f * Globals.GlobalUIScale;
			float num4 = 2f * Globals.GlobalUIScale;
			float y = num3 * 2f + textLineHeight * 2f + textLineHeight2 + num4 * 2f;
			if (cheapestWorlds.Count == 0 && expensiveWorlds.Count == 1)
			{
				RankedWorldPriceRow world = expensiveWorlds[0];
				float num5 = MathF.Min(240f * Globals.GlobalUIScale, x);
				float num6 = MathF.Max(0f, (x - num5) / 2f);
				if (num6 > 0f)
				{
					ImGui.SetCursorPosX(ImGui.GetCursorPosX() + num6);
				}
				DrawOverviewPriceCard(frame, displayRegionName, world, OverviewCardType.SingleOnly, 1, 1, new Vector2(num5, y), currentWorldPrice);
				ImGui.Spacing();
				return;
			}
			float num7 = 4f * num + num2;
			float x2 = MathF.Max(30f * Globals.GlobalUIScale, (x - num7) / 6f);
			int count = expensiveWorlds.Count;
			for (int i = 0; i < 3; i++)
			{
				int num8 = i - (3 - count);
				if (i > 0)
				{
					ImGui.SameLine(0f, num);
				}
				if (num8 >= 0 && num8 < count)
				{
					RankedWorldPriceRow world2 = expensiveWorlds[num8];
					int rank = count - num8;
					DrawOverviewPriceCard(frame, displayRegionName, world2, OverviewCardType.Expensive, rank, count, new Vector2(x2, y), currentWorldPrice);
				}
				else
				{
					DrawOverviewEmptySlot(new Vector2(x2, y));
				}
			}
			int count2 = cheapestWorlds.Count;
			for (int j = 0; j < 3; j++)
			{
				ImGui.SameLine(0f, (j == 0) ? num2 : num);
				if (j < count2)
				{
					RankedWorldPriceRow world3 = cheapestWorlds[j];
					int rank2 = j + 1;
					DrawOverviewPriceCard(frame, displayRegionName, world3, OverviewCardType.Cheapest, rank2, count2, new Vector2(x2, y), currentWorldPrice);
				}
				else
				{
					DrawOverviewEmptySlot(new Vector2(x2, y));
				}
			}
			ImGui.Spacing();
		}
	}

	private void DrawOverviewPriceCard(MarketBoardUIContext frame, string displayRegionName, RankedWorldPriceRow world, OverviewCardType cardType, int rank, int totalCount, Vector2 cardSize, ulong currentWorldPrice)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_064e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0689: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_074d: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a3: Unknown result type (might be due to invalid IL or missing references)
		bool flag = world.WorldID == GameState.CurrentWorld;
		bool flag2 = (frame.IsViewingCurrentWorld ? flag : (world.WorldID == frame.SelectedWorldID));
		bool flag3 = rank == 1;
		Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
		Vector2 vector = cursorScreenPos + cardSize;
		ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
		float num = 4f * Globals.GlobalUIScale;
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(15, 1);
		((ImU8String)(ref val)).AppendLiteral("##OverviewCard_");
		((ImU8String)(ref val)).AppendFormatted<uint>(world.WorldID);
		if (ImGui.InvisibleButton(val, cardSize, (ImGuiButtonFlags)0))
		{
			provider.SelectWorld(world.WorldID);
		}
		bool flag4 = ImGui.IsItemHovered();
		if (flag4)
		{
			ImGui.SetMouseCursor((ImGuiMouseCursor)7);
		}
		if (!flag && ImGui.IsItemClicked((ImGuiMouseButton)1))
		{
			OmenServiceBase<ChatManager>.Instance().SendMessage("/pdr worldtravel " + LuminaWrapper.GetWorldName(world.WorldID));
		}
		Vector4 color;
		Vector4 color2;
		Vector4 color3;
		if (flag2)
		{
			color = VectorExtensions.WithW(KnownColor.Chocolate.ToVector4(), flag4 ? 0.3f : 0.2f);
			color2 = VectorExtensions.WithW(KnownColor.Chocolate.ToVector4(), 0.9f);
			color3 = KnownColor.Chocolate.ToVector4();
		}
		else if (flag)
		{
			color = VectorExtensions.WithW(KnownColor.DeepPink.ToVector4(), flag4 ? 0.25f : 0.16f);
			color2 = VectorExtensions.WithW(KnownColor.Pink.ToVector4(), flag4 ? 0.95f : 0.75f);
			color3 = KnownColor.LightPink.ToVector4();
		}
		else
		{
			switch (cardType)
			{
			case OverviewCardType.Expensive:
				if (flag3)
				{
					color = VectorExtensions.WithW(KnownColor.OrangeRed.ToVector4(), flag4 ? 0.28f : 0.18f);
					color2 = VectorExtensions.WithW(KnownColor.OrangeRed.ToVector4(), flag4 ? 0.9f : 0.7f);
					color3 = KnownColor.OrangeRed.ToVector4();
				}
				else if (rank == 2)
				{
					color = VectorExtensions.WithW(KnownColor.IndianRed.ToVector4(), flag4 ? 0.18f : 0.1f);
					color2 = VectorExtensions.WithW(KnownColor.Salmon.ToVector4(), flag4 ? 0.7f : 0.45f);
					color3 = KnownColor.Salmon.ToVector4();
				}
				else
				{
					color = VectorExtensions.WithW(KnownColor.IndianRed.ToVector4(), flag4 ? 0.12f : 0.06f);
					color2 = VectorExtensions.WithW(KnownColor.LightSalmon.ToVector4(), flag4 ? 0.5f : 0.25f);
					color3 = KnownColor.LightSalmon.ToVector4();
				}
				break;
			case OverviewCardType.Cheapest:
				if (flag3)
				{
					color = VectorExtensions.WithW(KnownColor.ForestGreen.ToVector4(), flag4 ? 0.35f : 0.22f);
					color2 = VectorExtensions.WithW(KnownColor.GreenYellow.ToVector4(), flag4 ? 0.95f : 0.75f);
					color3 = KnownColor.GreenYellow.ToVector4();
				}
				else if (rank == 2)
				{
					color = VectorExtensions.WithW(KnownColor.ForestGreen.ToVector4(), flag4 ? 0.2f : 0.12f);
					color2 = VectorExtensions.WithW(KnownColor.LimeGreen.ToVector4(), flag4 ? 0.7f : 0.45f);
					color3 = KnownColor.LimeGreen.ToVector4();
				}
				else
				{
					color = VectorExtensions.WithW(KnownColor.ForestGreen.ToVector4(), flag4 ? 0.14f : 0.07f);
					color2 = VectorExtensions.WithW(KnownColor.PaleGreen.ToVector4(), flag4 ? 0.5f : 0.25f);
					color3 = KnownColor.PaleGreen.ToVector4();
				}
				break;
			default:
				color = VectorExtensions.WithW(ImGui.GetColorU32((ImGuiCol)7).ToVector4(), flag4 ? 0.5f : 0.35f);
				color2 = VectorExtensions.WithW(KnownColor.Chocolate.ToVector4(), flag4 ? 0.8f : 0.45f);
				color3 = KnownColor.White.ToVector4();
				break;
			}
		}
		((ImDrawListPtr)(ref windowDrawList)).AddRectFilled(cursorScreenPos, vector, color.ToUInt(), num);
		((ImDrawListPtr)(ref windowDrawList)).AddRect(cursorScreenPos, vector, color2.ToUInt(), num, (ImDrawFlags)0, (flag2 || (flag3 && flag4)) ? (1.5f * Globals.GlobalUIScale) : (1f * Globals.GlobalUIScale));
		uint num2 = VectorExtensions.WithW(KnownColor.White.ToVector4(), flag4 ? 0.18f : 0.06f).ToUInt();
		((ImDrawListPtr)(ref windowDrawList)).AddLine(new Vector2(cursorScreenPos.X + num, cursorScreenPos.Y + 0.5f), new Vector2(vector.X - num, cursorScreenPos.Y + 0.5f), num2, 1f);
		float num3 = 4f * Globals.GlobalUIScale;
		float num4 = 3.5f * Globals.GlobalUIScale;
		float num5 = 2f * Globals.GlobalUIScale;
		float textLineHeight;
		using (OmenServiceBase<FontManager>.Instance().UIFont60.Push())
		{
			textLineHeight = ImGui.GetTextLineHeight();
		}
		float num6 = cursorScreenPos.Y + num4;
		float num7 = num6 + textLineHeight + num5;
		float y = num7 + textLineHeight + num5;
		using (OmenServiceBase<FontManager>.Instance().UIFont60.Push())
		{
			string text;
			uint num8;
			if (flag)
			{
				text = LanguageManager.Get("BetterMarketBoard-Home");
				num8 = KnownColor.Pink.ToUInt();
			}
			else
			{
				switch (cardType)
				{
				case OverviewCardType.Expensive:
					text = (flag3 ? LanguageManager.Get("Highest") : $"#{rank}");
					num8 = (flag3 ? KnownColor.OrangeRed.ToUInt() : ImGui.GetColorU32((ImGuiCol)1));
					break;
				case OverviewCardType.Cheapest:
					text = (flag3 ? LanguageManager.Get("Lowest") : $"#{rank}");
					num8 = (flag3 ? KnownColor.GreenYellow.ToUInt() : ImGui.GetColorU32((ImGuiCol)1));
					break;
				default:
					text = LanguageManager.Get("BetterMarketBoard-OnlyOne");
					num8 = KnownColor.Chocolate.ToUInt();
					break;
				}
			}
			Vector2 vector2 = ImGui.CalcTextSize(ImU8String.op_Implicit(text), false, -1f);
			Vector2 vector3 = new Vector2(vector.X - num3 - vector2.X, num6);
			((ImDrawListPtr)(ref windowDrawList)).AddText(vector3, num8, ImU8String.op_Implicit(text));
			string dCName = world.DCName;
			Vector2 vector4 = new Vector2(cursorScreenPos.X + num3, num6);
			((ImDrawListPtr)(ref windowDrawList)).PushClipRect(cursorScreenPos, new Vector2(vector3.X - 2f * Globals.GlobalUIScale, num7), true);
			((ImDrawListPtr)(ref windowDrawList)).AddText(vector4, ImGui.GetColorU32((ImGuiCol)1), ImU8String.op_Implicit(dCName));
			((ImDrawListPtr)(ref windowDrawList)).PopClipRect();
		}
		using (OmenServiceBase<FontManager>.Instance().UIFont60.Push())
		{
			string worldName = world.WorldName;
			Vector2 vector5 = ImGui.CalcTextSize(ImU8String.op_Implicit(worldName), false, -1f);
			uint colorU = ImGui.GetColorU32((ImGuiCol)1);
			Vector2 vector6 = new Vector2(cursorScreenPos.X + MathF.Max(num3, (cardSize.X - vector5.X) / 2f), num7);
			((ImDrawListPtr)(ref windowDrawList)).PushClipRect(cursorScreenPos, new Vector2(vector.X - num3, y), true);
			((ImDrawListPtr)(ref windowDrawList)).AddText(vector6, colorU, ImU8String.op_Implicit(worldName));
			((ImDrawListPtr)(ref windowDrawList)).PopClipRect();
		}
		string text2 = ((world.MinPrice == ulong.MaxValue) ? "-" : (world.MinPrice.ToChineseString() + "\ue049"));
		using (OmenServiceBase<FontManager>.Instance().UIFont80.Push())
		{
			Vector2 vector7 = ImGui.CalcTextSize(ImU8String.op_Implicit(text2), false, -1f);
			Vector2 vector8 = new Vector2(cursorScreenPos.X + MathF.Max(num3, (cardSize.X - vector7.X) / 2f), y);
			((ImDrawListPtr)(ref windowDrawList)).PushClipRect(cursorScreenPos, vector, true);
			((ImDrawListPtr)(ref windowDrawList)).AddText(vector8, color3.ToUInt(), ImU8String.op_Implicit(text2));
			((ImDrawListPtr)(ref windowDrawList)).PopClipRect();
		}
		if (flag4)
		{
			DrawWorldPriceTooltip(frame, world.WorldID, world.WorldName, world.MinPrice, currentWorldPrice);
		}
	}

	private static void DrawOverviewEmptySlot(Vector2 slotSize)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
		Vector2 vector = cursorScreenPos + slotSize;
		ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
		float num = 4f * Globals.GlobalUIScale;
		ImGui.Dummy(slotSize);
		uint colorU = ImGui.GetColorU32((ImGuiCol)7, 0.15f);
		uint colorU2 = ImGui.GetColorU32((ImGuiCol)5, 0.12f);
		((ImDrawListPtr)(ref windowDrawList)).AddRectFilled(cursorScreenPos, vector, colorU, num);
		((ImDrawListPtr)(ref windowDrawList)).AddRect(cursorScreenPos, vector, colorU2, num, (ImDrawFlags)0, 1f * Globals.GlobalUIScale);
		using (OmenServiceBase<FontManager>.Instance().UIFont60.Push())
		{
			string text = "-";
			Vector2 vector2 = ImGui.CalcTextSize(ImU8String.op_Implicit(text), false, -1f);
			Vector2 vector3 = new Vector2(cursorScreenPos.X + (slotSize.X - vector2.X) / 2f, cursorScreenPos.Y + (slotSize.Y - vector2.Y) / 2f);
			((ImDrawListPtr)(ref windowDrawList)).AddText(vector3, ImGui.GetColorU32((ImGuiCol)1, 0.3f), ImU8String.op_Implicit(text));
		}
	}

	private void DrawWorldPriceTooltip(MarketBoardUIContext frame, uint worldID, string worldName, ulong minPrice, ulong currentWorldPrice)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_046a: Unknown result type (might be due to invalid IL or missing references)
		//IL_046f: Unknown result type (might be due to invalid IL or missing references)
		//IL_049f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0403: Unknown result type (might be due to invalid IL or missing references)
		//IL_0448: Unknown result type (might be due to invalid IL or missing references)
		bool flag = worldID == GameState.CurrentWorld;
		bool flag2 = minPrice == ulong.MaxValue;
		TooltipDisposable val = ImRaii.Tooltip();
		try
		{
			string text = (flag ? (worldName + " (" + LanguageManager.Get("BetterMarketBoard-Tooltip-CurrentWorld") + ")") : worldName);
			Vector4 vector = (flag ? KnownColor.Pink.ToVector4() : KnownColor.Chocolate.ToVector4());
			ImGui.TextColored(ref vector, ImU8String.op_Implicit(text));
			ImGui.Separator();
			if (flag2)
			{
				ImGui.TextDisabled(ImU8String.op_Implicit("-"));
			}
			else
			{
				ImU8String val2 = new ImU8String(1, 1);
				((ImU8String)(ref val2)).AppendFormatted<string>(minPrice.ToChineseString());
				((ImU8String)(ref val2)).AppendLiteral("\ue049");
				ImGui.TextUnformatted(val2);
				if (!flag && currentWorldPrice != 0L && currentWorldPrice != ulong.MaxValue)
				{
					long num = (long)(minPrice - currentWorldPrice);
					double num2 = (double)num / (double)currentWorldPrice * 100.0;
					if (num >= 0)
					{
						if (num > 0)
						{
							ulong number = (ulong)num;
							vector = KnownColor.OrangeRed.ToVector4();
							ImU8String val3 = new ImU8String(7, 2);
							((ImU8String)(ref val3)).AppendLiteral("+");
							((ImU8String)(ref val3)).AppendFormatted<string>(number.ToChineseString());
							((ImU8String)(ref val3)).AppendLiteral("\ue049 (+");
							((ImU8String)(ref val3)).AppendFormatted<double>(num2, "F1");
							((ImU8String)(ref val3)).AppendLiteral("%)");
							ImGui.TextColored(ref vector, val3);
						}
					}
					else
					{
						ulong number2 = (ulong)(-num);
						vector = KnownColor.GreenYellow.ToVector4();
						ImU8String val4 = new ImU8String(7, 2);
						((ImU8String)(ref val4)).AppendLiteral("-");
						((ImU8String)(ref val4)).AppendFormatted<string>(number2.ToChineseString());
						((ImU8String)(ref val4)).AppendLiteral("\ue049 (-");
						((ImU8String)(ref val4)).AppendFormatted<double>(Math.Abs(num2), "F1");
						((ImU8String)(ref val4)).AppendLiteral("%)");
						ImGui.TextColored(ref vector, val4);
					}
				}
			}
			UniversalisAggregatedMarketDataResponse aggregatedResponse = provider.GetAggregatedResponse(frame.ItemID, worldID);
			if (aggregatedResponse != null)
			{
				UniversalisAggregatedMarketResult universalisAggregatedMarketResult = aggregatedResponse.Results.FirstOrDefault((UniversalisAggregatedMarketResult r) => r.ItemID == frame.ItemID);
				if (universalisAggregatedMarketResult != null)
				{
					UniversalisAggregatedMarketScope aggregatedMarketScope = GetAggregatedMarketScope(universalisAggregatedMarketResult, frame.HQOnly);
					float valueOrDefault = aggregatedMarketScope.DailySaleVelocity.World.Quantity.GetValueOrDefault();
					double? price = aggregatedMarketScope.AverageSalePrice.World.Price;
					UniversalisAggregatedMarketPurchasePoint world = aggregatedMarketScope.RecentPurchase.World;
					ImGui.Separator();
					if (valueOrDefault > 0f)
					{
						ImU8String val5 = new ImU8String(2, 1);
						((ImU8String)(ref val5)).AppendFormatted<string>(LanguageManager.Get("BetterMarketBoard-Tooltip-DailySales"));
						((ImU8String)(ref val5)).AppendLiteral(": ");
						ImGui.TextDisabled(val5);
						ImGui.SameLine();
						vector = KnownColor.Chocolate.ToVector4();
						ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("BetterMarketBoard-DailySales-Format", valueOrDefault.ToString("0.#", CultureInfo.InvariantCulture))));
					}
					if (price.HasValue && price.GetValueOrDefault() > 0.0)
					{
						ImU8String val6 = new ImU8String(2, 1);
						((ImU8String)(ref val6)).AppendFormatted<string>(LanguageManager.Get("BetterMarketBoard-AveragePrice"));
						((ImU8String)(ref val6)).AppendLiteral(": ");
						ImGui.TextDisabled(val6);
						ImGui.SameLine();
						vector = KnownColor.Orange.ToVector4();
						ImU8String val7 = new ImU8String(1, 1);
						((ImU8String)(ref val7)).AppendFormatted<string>(((ulong)Math.Round(price.Value)).ToChineseString());
						((ImU8String)(ref val7)).AppendLiteral("\ue049");
						ImGui.TextColored(ref vector, val7);
					}
					if (world != null)
					{
						double? price2 = world.Price;
						if (price2.HasValue && price2.GetValueOrDefault() > 0.0)
						{
							long? timestamp = world.Timestamp;
							if (timestamp.HasValue && timestamp.GetValueOrDefault() > 0)
							{
								string text2 = DateTimeOffset.FromUnixTimeMilliseconds(world.Timestamp.Value).LocalDateTime.TimeAgo();
								ImU8String val8 = new ImU8String(2, 1);
								((ImU8String)(ref val8)).AppendFormatted<string>(LanguageManager.Get("BetterMarketBoard-RecentPurchase"));
								((ImU8String)(ref val8)).AppendLiteral(": ");
								ImGui.TextDisabled(val8);
								ImGui.SameLine();
								ImU8String val9 = new ImU8String(4, 2);
								((ImU8String)(ref val9)).AppendFormatted<string>(((ulong)Math.Round(world.Price.Value)).ToChineseString());
								((ImU8String)(ref val9)).AppendLiteral("\ue049 (");
								((ImU8String)(ref val9)).AppendFormatted<string>(text2);
								((ImU8String)(ref val9)).AppendLiteral(")");
								ImGui.TextUnformatted(val9);
							}
						}
					}
				}
			}
			if (!flag)
			{
				ImGui.Separator();
				using (OmenServiceBase<FontManager>.Instance().UIFont60.Push())
				{
					ImU8String val10 = new ImU8String(1, 2);
					((ImU8String)(ref val10)).AppendFormatted<string>(LanguageManager.Get("RightClick"));
					((ImU8String)(ref val10)).AppendLiteral("：");
					((ImU8String)(ref val10)).AppendFormatted<string>(LanguageManager.Get("BetterMarketBoard-TravelToWorld"));
					ImGui.TextDisabled(val10);
					return;
				}
			}
		}
		finally
		{
			((TooltipDisposable)(ref val)).Dispose();
		}
	}

	private void DrawAllWorldPricesToggleComponent(ImGuiDir direction)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Invalid comparison between Unknown and I4
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		using (OmenServiceBase<FontManager>.Instance().UIFont80.Push())
		{
			float num = 150f * Globals.GlobalUIScale;
			Vector2 cursorPos = ImGui.GetCursorPos();
			float num2 = ImGui.GetWindowContentRegionMax().X - ImGui.GetFrameHeight() - num;
			ImGuiStylePtr style = ImGui.GetStyle();
			float x = num2 - 2f * ((ImGuiStylePtr)(ref style)).ItemSpacing.X;
			float num3 = cursorPos.Y - ImGui.GetFrameHeight();
			style = ImGui.GetStyle();
			ImGui.SetCursorPos(new Vector2(x, num3 - ((ImGuiStylePtr)(ref style)).FramePadding.Y));
			GroupDisposable val = ImRaii.Group();
			try
			{
				ImGui.SetNextItemWidth(num);
				ComboDisposable val2 = ImRaii.Combo(ImU8String.op_Implicit("###RegionSelector"), ImU8String.op_Implicit(provider.EffectiveRegionName));
				try
				{
					if (ComboDisposable.op_Implicit(val2))
					{
						foreach (string key in allWorlds.Keys)
						{
							if (ImGui.Selectable(ImU8String.op_Implicit(key.Replace('-', ' ')), provider.EffectiveRegionName == key, (ImGuiSelectableFlags)0, default(Vector2)))
							{
								provider.SelectRegion(key);
							}
						}
					}
				}
				finally
				{
					((ComboDisposable)(ref val2)).Dispose();
				}
				ImGui.SameLine();
				if (ImGui.ArrowButton(ImU8String.op_Implicit("###RegionPriceToggle"), direction))
				{
					isAllWorldsPriceExpanded = !isAllWorldsPriceExpanded;
				}
				ImGuiOm.TooltipHover(LanguageManager.Get(((int)direction == 3) ? "BetterMarketBoard-ShowAllWorldPrices" : "BetterMarketBoard-CollapseAllWorldPrices"));
			}
			finally
			{
				((GroupDisposable)(ref val)).Dispose();
			}
			ImGui.SetCursorPos(cursorPos);
		}
	}

	private void DrawFullPriceTable(MarketBoardUIContext frame, string displayRegionName)
	{
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		WorldPriceRanks worldPriceRanks = provider.GetWorldPriceRanks(frame.ItemID);
		if (worldPriceRanks == null || worldPriceRanks.Valid.Count == 0)
		{
			return;
		}
		using (OmenServiceBase<FontManager>.Instance().UIFont80.Push())
		{
			float num = 60f * Globals.GlobalUIScale;
			float num2 = 4f * Globals.GlobalUIScale;
			float y = ImGui.GetTextLineHeight() * 2f + 6f * Globals.GlobalUIScale;
			float num3 = 4f * Globals.GlobalUIScale;
			ulong minPrice = worldPriceRanks.Current.MinPrice;
			foreach (KeyValuePair<string, List<WorldPriceRow>> dCWorldPrice in provider.DCWorldPrices)
			{
				dCWorldPrice.Deconstruct(out var key, out var value);
				string text = key;
				List<WorldPriceRow> list = value;
				float num4 = ImGui.GetContentRegionAvail().X - num - num2;
				int count = list.Count;
				if (count == 0)
				{
					continue;
				}
				float x = MathF.Max(30f * Globals.GlobalUIScale, (num4 - num2 * (float)(count - 1)) / (float)count);
				GroupDisposable val = ImRaii.Group();
				try
				{
					Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
					Vector2 vector = new Vector2(num, y);
					ImU8String val2 = new ImU8String(10, 1);
					((ImU8String)(ref val2)).AppendLiteral("##DCBadge_");
					((ImU8String)(ref val2)).AppendFormatted<string>(text);
					ImGui.InvisibleButton(val2, vector, (ImGuiButtonFlags)0);
					ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
					Vector2 vector2 = cursorScreenPos + vector;
					((ImDrawListPtr)(ref windowDrawList)).AddRectFilled(cursorScreenPos, vector2, VectorExtensions.WithW(KnownColor.Chocolate.ToVector4(), 0.15f).ToUInt(), num3);
					((ImDrawListPtr)(ref windowDrawList)).AddRect(cursorScreenPos, vector2, VectorExtensions.WithW(KnownColor.Chocolate.ToVector4(), 0.45f).ToUInt(), num3, (ImDrawFlags)240, 1f * Globals.GlobalUIScale);
					Vector2 vector3 = ImGui.CalcTextSize(ImU8String.op_Implicit(text), false, -1f);
					Vector2 vector4 = new Vector2(cursorScreenPos.X + MathF.Max(0f, (vector.X - vector3.X) / 2f), cursorScreenPos.Y + (vector.Y - vector3.Y) / 2f);
					((ImDrawListPtr)(ref windowDrawList)).AddText(vector4, KnownColor.Chocolate.ToUInt(), ImU8String.op_Implicit(text));
					ImGuiOm.TooltipHover(text ?? "");
					foreach (WorldPriceRow item in list)
					{
						ImGui.SameLine(0f, num2);
						worldPriceCards.GetOrAdd(item.WorldID, (uint _) => new WorldPriceCardComponent()).Draw(frame, item, new Vector2(x, y), minPrice);
					}
				}
				finally
				{
					((GroupDisposable)(ref val)).Dispose();
				}
				ImGui.Spacing();
			}
		}
	}

	private unsafe static bool IsAbleToSearchMarket()
	{
		if (IsAbleToSearchLocalMarket() && (!IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()) || DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)50]))
		{
			return !(*Addons.ItemSearchResult).IsAddonAndNodesReady();
		}
		return false;
	}

	private static bool IsAbleToSearchLocalMarket()
	{
		if (GameState.IsLoggedIn && GameState.ContentFinderCondition == 0)
		{
			return !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)69];
		}
		return false;
	}

	private unsafe static bool IsOwnRetainer(ulong retainerID)
	{
		RetainerManager* ptr = RetainerManager.Instance();
		if (ptr == null)
		{
			return false;
		}
		for (uint num = 0u; num < ((RetainerManager)ptr).GetRetainerCount(); num++)
		{
			Retainer* retainerBySortedIndex = ((RetainerManager)ptr).GetRetainerBySortedIndex(num);
			if (retainerBySortedIndex != null && ((Retainer)retainerBySortedIndex).RetainerId == retainerID)
			{
				return true;
			}
		}
		return false;
	}
}
