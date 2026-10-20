using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DailyRoutines.Common.Interface.Windows;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Common.Runtime.Hosts;
using DailyRoutines.Extensions;
using DailyRoutines.Internal;
using DailyRoutines.Manager;
using DailyRoutines.Verification;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Game.Command;
using Dalamud.Game.Gui.Toast;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Interface;
using Dalamud.Interface.ImGuiSeStringRenderer;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using Dalamud.Utility.Numerics;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.Sound;
using FFXIVClientStructs.FFXIV.Client.System.Input;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Common.Math;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Data.Files;
using Lumina.Data.Parsing.Uld;
using Lumina.Excel.Sheets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.ImGuiOm.Widgets.MapRenderer;
using OmenTools.Info.Game.AetheryteRecord;
using OmenTools.Info.Game.AetheryteRecord.Enums;
using OmenTools.Info.Game.Data;
using OmenTools.Info.Lumina;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;
using OmenTools.Utils.FuzzyMatcher;

namespace DailyRoutines.ModulesPublic.Interface.BetterTeleport;

public class BetterTeleport : ModuleBase
{
	public class RecentRecord
	{
		public string Key { get; set; } = string.Empty;
	}

	public class SearchSelectionRecord
	{
		public string Key { get; set; } = string.Empty;

		public int Count { get; set; }

		public long LastUsedUnixSeconds { get; set; }
	}

	private enum PageType
	{
		Search,
		Full
	}

	private class Config : ModuleConfig
	{
		public PageType DefaultPage;

		public bool FocusSearchOnOpen = true;

		public bool CloseOnLoseFocus = true;

		public bool HideAethernetInParty = true;

		public HashSet<string> Favorites = new HashSet<string>();

		public Dictionary<string, Vector3> Positions = new Dictionary<string, Vector3>();

		public Dictionary<string, string> Remarks = new Dictionary<string, string>();

		public List<RecentRecord> RecentTeleports = new List<RecentRecord>();

		public Dictionary<string, List<SearchSelectionRecord>> SearchSelections = new Dictionary<string, List<SearchSelectionRecord>>();
	}

	private struct OverlayListItem
	{
		public AetheryteRecord? Record;

		public bool IsShowMore;

		public bool DrawSeparatorBefore;

		public string Name;
	}

	private class BetterTeleportFullWindow(BetterTeleport module) : Window(LuminaWrapper.GetAddonText(8513u) + "###BetterTeleportFullWindow")
	{
		public override void OnOpen()
		{
			module.RefreshFavoritesInfo();
			module.RefreshDefaultOverlayItems();
			module.recordMatcher?.Dispose();
			module.recordMatcher = module.CreateRecordMatcher();
			module.fullSearchWord = string.Empty;
			module.fullSearchResult = new List<AetheryteRecord>();
			module.selectedIndex = 0;
			module.hasUsedArrowKeys = true;
			module.hoveredAetheryte = null;
			module.pinnedAetheryte = null;
			module.activeTabName = ((module.favorites.Count > 0) ? "Favorite" : (OmenServiceBase<AetheryteRecordManager>.Instance().Records.Keys.FirstOrDefault() ?? "Setting"));
			module.tabToSelect = module.activeTabName;
			module.shouldFocusSearchBar = module.config.FocusSearchOnOpen;
			module.shouldScrollToSelected = false;
			module.isJustOpened = true;
			module.lastMousePos = ImGui.GetMousePos();
		}

		public override void Draw()
		{
			module.DrawFullWindowUI();
		}

		public override void OnClose()
		{
			module.config.Save(module);
		}
	}

	private unsafe delegate int AddonTeleportGetRegionIconIDDelegate(AddonTeleport* addon, int category);

	private unsafe delegate int AgentTeleportGetRegionDelegate(AgentTeleport* agent, TeleportInfo* teleportInfo);

	private unsafe delegate int AgentTeleportGetTimelineIDDelegate(AgentTeleport* agent, int region, int territoryTypeID);

	private Config config;

	private const string COMMAND = "/pdrtelepo";

	private const int MAX_SEARCH_SELECTION_TERMS = 200;

	private const int MAX_SEARCH_SELECTION_RECORDS_PER_TERM = 8;

	[CompilerGenerated]
	private static Dictionary<uint, string> _003CTicketUsageTypes_003Ek__BackingField;

	private string searchWord = string.Empty;

	private int selectedIndex;

	private bool hasUsedArrowKeys;

	private Vector2 lastMousePos;

	private bool isSearchingInputting;

	private Vector2 lastMousePosForInput;

	private readonly List<OverlayListItem> visibleItems = new List<OverlayListItem>();

	private readonly List<OverlayListItem> defaultOverlayItems = new List<OverlayListItem>();

	private BetterTeleportFullWindow? fullWindow;

	private readonly Dictionary<(uint RowID, byte SubIndex), float> hoverProgress = new Dictionary<(uint, byte), float>();

	private bool isNeedToLoseFocusSearchBar;

	private string fullSearchWord = string.Empty;

	private List<AetheryteRecord> fullSearchResult = new List<AetheryteRecord>();

	private string activeTabName = string.Empty;

	private string? tabToSelect;

	private bool shouldScrollToSelected;

	private bool isJustOpened;

	private const float TAB_ICON_PADDING_X = 14f;

	private static readonly CompSig AddonTeleportGetRegionIconIDSig = new CompSig("40 55 48 8B EC 48 83 EC ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 45 ?? 83 FA");

	private AddonTeleportGetRegionIconIDDelegate AddonTeleportGetRegionIconID;

	private static readonly CompSig AgentTeleportGetRegionSig = new CompSig("48 83 EC ?? 0F B7 4A ?? E8 ?? ?? ?? ?? 48 85 C0 0F 84");

	private AgentTeleportGetRegionDelegate AgentTeleportGetRegion;

	private static readonly CompSig AgentTeleportGetTimelineIDSig = new CompSig("41 81 F8 ?? 03 00 00 75 ?? B8 08 00 00 00 C3 41 81 F8 ?? 03 00 00 75 ?? B8 09 00 00 00 C3");

	private AgentTeleportGetTimelineIDDelegate AgentTeleportGetTimelineID;

	private FuzzyMatcher<AetheryteRecord>? recordMatcher;

	private List<AetheryteRecord> favorites = new List<AetheryteRecord>();

	private bool shouldFocusSearchBar;

	private AetheryteRecord? hoveredAetheryte;

	private AetheryteRecord? pinnedAetheryte;

	private Vector3 contextMenuTargetPos;

	private uint contextMenuTargetZone;

	private readonly ImGuiMapRenderer mapRenderer = new ImGuiMapRenderer
	{
		MinZoom = 0.2f,
		MaxZoom = 4f,
		LerpSpeed = 15f,
		EnableDefaultMarkers = true,
		DefaultMarkerFilter = delegate(MapMarker marker)
		{
			byte dataType = ((MapMarker)(ref marker)).DataType;
			bool flag = (uint)(dataType - 3) <= 1u;
			return !flag;
		}
	};

	private readonly Dictionary<int, (string Path, Vector2 UV0, Vector2 UV1)> aetheryteRegionIcons = new Dictionary<int, (string, Vector2, Vector2)>();

	private readonly Dictionary<int, (string Path, Vector2 UV0, Vector2 UV1)> aetheryteTabIcons = new Dictionary<int, (string, Vector2, Vector2)>();

	private (string Path, Vector2 UV0, Vector2 UV1)? aetheryteSettingIcon;

	private const string TELEPORT_ULD_PATH = "ui/uld/Teleport.uld";

	private const uint TELEPORT_REGION_ICON_PART_LIST_ID = 16u;

	private const uint TELEPORT_REGION_ICON_TIMELINE_ID = 18u;

	private const int MOR_DHONA_TIMELINE_ID = 4;

	private const uint TELEPORT_TAB_ICON_PART_LIST_ID = 14u;

	private const uint TELEPORT_SETTING_ICON_PART_LIST_ID = 7u;

	private const int TELEPORT_VERSION_ICON_ID = 115;

	private const int TELEPORT_FAVOURITE_CATEGORY = 200;

	private const uint GIL_ITEM_ID = 1u;

	private const uint TELEPORT_TICKET_ITEM_ID = 7569u;

	private static readonly SeString HomeChar = new SeStringBuilder().AddIcon((BitmapFontIcon)110).Build();

	private static readonly SeString FreeChar = new SeStringBuilder().AddIcon((BitmapFontIcon)67).Build();

	private static readonly SeString FavoriteChar = new SeStringBuilder().AddIcon((BitmapFontIcon)68).Build();

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("BetterTeleportTitle"),
		Description = LanguageManager.Get("BetterTeleportDescription"),
		Category = ModuleCategory.Interface,
		ModulesPrerequisite = new string[1] { "SameAethernetTeleport" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		NeedAuth = true
	};

	private static uint TicketUsageType
	{
		get
		{
			return DalamudServiceExtension.Instance<IGameConfig>().UiConfig.GetUInt("TelepoTicketUseType");
		}
		set
		{
			DalamudServiceExtension.Instance<IGameConfig>().UiConfig.Set("TelepoTicketUseType", value);
		}
	}

	private static uint TicketUsageGilSetting
	{
		get
		{
			return DalamudServiceExtension.Instance<IGameConfig>().UiConfig.GetUInt("TelepoTicketGilSetting");
		}
		set
		{
			DalamudServiceExtension.Instance<IGameConfig>().UiConfig.Set("TelepoTicketGilSetting", value);
		}
	}

	private static Dictionary<uint, string> TicketUsageTypes
	{
		get
		{
			if (_003CTicketUsageTypes_003Ek__BackingField != null)
			{
				return _003CTicketUsageTypes_003Ek__BackingField;
			}
			_003CTicketUsageTypes_003Ek__BackingField = new Dictionary<uint, string>();
			for (uint num = 0u; num < 5; num++)
			{
				string addonText = LuminaWrapper.GetAddonText(num + 8523);
				_003CTicketUsageTypes_003Ek__BackingField[num] = addonText;
			}
			return _003CTicketUsageTypes_003Ek__BackingField;
		}
	}

	protected override void Init()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Expected O, but got Unknown
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Expected O, but got Unknown
		if (base.Overlay == null)
		{
			Overlay overlay = (base.Overlay = new Overlay(this));
		}
		Overlay? overlay3 = base.Overlay;
		((Window)overlay3).Flags = (ImGuiWindowFlags)(((Window)overlay3).Flags | 0x14B);
		((Window)base.Overlay).WindowName = "###BetterTeleportOverlay";
		Overlay? overlay4 = base.Overlay;
		WindowSizeConstraints value = default(WindowSizeConstraints);
		((WindowSizeConstraints)(ref value))._002Ector();
		((WindowSizeConstraints)(ref value)).MinimumSize = new Vector2(400f * Globals.GlobalUIScale, -1f);
		((WindowSizeConstraints)(ref value)).MaximumSize = new Vector2(400f * Globals.GlobalUIScale, -1f);
		((Window)overlay4).SizeConstraints = value;
		fullWindow = new BetterTeleportFullWindow(this);
		BetterTeleportFullWindow? betterTeleportFullWindow = fullWindow;
		((Window)betterTeleportFullWindow).Flags = (ImGuiWindowFlags)(((Window)betterTeleportFullWindow).Flags | 0x18);
		ManagerHost.Current.AddWindow((Window)(object)fullWindow);
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 60000
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		MigrateConfig();
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		AddonTeleportGetRegionIconID = AddonTeleportGetRegionIconIDSig.GetDelegate<AddonTeleportGetRegionIconIDDelegate>();
		AgentTeleportGetRegion = AgentTeleportGetRegionSig.GetDelegate<AgentTeleportGetRegionDelegate>();
		AgentTeleportGetTimelineID = AgentTeleportGetTimelineIDSig.GetDelegate<AgentTeleportGetTimelineIDDelegate>();
		OmenServiceBase<CommandManager>.Instance().AddCommand("/pdrtelepo", new CommandInfo(new HandlerDelegate(OnCommand))
		{
			HelpMessage = LanguageManager.Get("BetterTeleport-CommandHelp")
		});
		OmenServiceBase<UseActionManager>.Instance().RegPreUseAction(OnPostUseAction);
		OmenServiceBase<InputIDManager>.Instance().RegPrePressed(OnPreInputIDPressed);
	}

	protected override void Uninit()
	{
		OmenServiceBase<InputIDManager>.Instance().UnregPrePressed(OnPreInputIDPressed);
		OmenServiceBase<UseActionManager>.Instance().Unreg(OnPostUseAction);
		OmenServiceBase<CommandManager>.Instance().RemoveCommand("/pdrtelepo");
		if (fullWindow != null)
		{
			ManagerHost.Current.RemoveWindow((Window)(object)fullWindow);
			fullWindow = null;
		}
		recordMatcher?.Dispose();
		recordMatcher = null;
	}

	private void RefreshFavoritesInfo()
	{
		if (config.Favorites.Count != 0)
		{
			favorites = (from x in OmenServiceBase<AetheryteRecordManager>.Instance().AllRecords
				where config.Favorites.Contains(x.ToString())
				orderby x.RowID
				select x).ToList();
		}
	}

	private unsafe void HandleTeleport(AetheryteRecord aetheryte, string? searchText = null)
	{
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Expected O, but got Unknown
		if (GameState.ContentFinderCondition != 0)
		{
			return;
		}
		List<string> searchTerms = GetSearchSelectionTerms(searchText).ToList();
		base.TaskHelper.Abort();
		((Window)base.Overlay).IsOpen = false;
		((Window)fullWindow).IsOpen = false;
		BattleChara* localPlayer = Control.GetLocalPlayer();
		if (localPlayer == null)
		{
			return;
		}
		Vector3 value;
		bool flag = config.Positions.TryGetValue(aetheryte.ToString(), out value);
		Vector3 aetherytePos = (flag ? value : aetheryte.Position);
		if (((aetheryte.ZoneID != GameState.TerritoryType) ? 999f : Vector2.DistanceSquared(((BattleChara)localPlayer).Position.ToVector2(), aetherytePos.ToVector2())) <= 900f)
		{
			return;
		}
		if (aetherytePos.Y == 0f)
		{
			aetherytePos = VectorExtensions.WithY(aetherytePos, 500f);
		}
		searchWord = string.Empty;
		pinnedAetheryte = null;
		hoveredAetheryte = null;
		((Window)base.Overlay).IsOpen = false;
		((Window)fullWindow).IsOpen = false;
		RememberSearchSelection(aetheryte, searchTerms);
		AddToRecentTeleports(aetheryte);
		NotifyHelper.ToastQuest(LanguageManager.Get("BetterTeleport-Notification", aetheryte.Name), new QuestToastOptions
		{
			IconId = 111u
		});
		base.TaskHelper.Enqueue((Func<bool>)aetheryte.TeleportTo, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
		if (!flag)
		{
			return;
		}
		base.TaskHelper.Enqueue(() => GameState.TerritoryType == aetheryte.ZoneID && !UIModuleExtension.IsScreenReady());
		base.TaskHelper.Enqueue(delegate
		{
			if (UIModuleExtension.IsScreenReady())
			{
				return true;
			}
			OmenServiceBase<MovementManager>.Instance().TPSmart_InZone(aetherytePos);
			return false;
		});
	}

	private void AddToRecentTeleports(AetheryteRecord aetheryte)
	{
		string key = aetheryte.ToString();
		config.RecentTeleports.RemoveAll((RecentRecord x) => x.Key == key);
		config.RecentTeleports.Insert(0, new RecentRecord
		{
			Key = key
		});
		if (config.RecentTeleports.Count > 20)
		{
			config.RecentTeleports.RemoveRange(20, config.RecentTeleports.Count - 20);
		}
		config.Save(this);
		RefreshDefaultOverlayItems();
	}

	private static IEnumerable<string> GetSearchSelectionTerms(string? searchText)
	{
		string normalized = NormalizeSearchSelectionTerm(searchText);
		if (string.IsNullOrEmpty(normalized))
		{
			yield break;
		}
		yield return normalized;
		string[] array = normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		if (array.Length <= 1)
		{
			yield break;
		}
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (text.Length >= 2)
			{
				yield return text;
			}
		}
	}

	private void RememberSearchSelection(AetheryteRecord aetheryte, IEnumerable<string> searchTerms)
	{
		List<string> list = searchTerms.Distinct<string>(StringComparer.Ordinal).ToList();
		if (list.Count == 0)
		{
			return;
		}
		long lastUsedUnixSeconds = OmenServiceBase<StandardTimeManager>.Instance().UTCNowOffset.ToUnixTimeSeconds();
		foreach (string item in list)
		{
			if (!config.SearchSelections.TryGetValue(item, out List<SearchSelectionRecord> value))
			{
				value = new List<SearchSelectionRecord>();
				config.SearchSelections[item] = value;
			}
			SearchSelectionRecord searchSelectionRecord = value.FirstOrDefault((SearchSelectionRecord x) => x.Key == aetheryte.ToString());
			if (searchSelectionRecord == null)
			{
				value.Add(new SearchSelectionRecord
				{
					Key = aetheryte.ToString(),
					Count = 1,
					LastUsedUnixSeconds = lastUsedUnixSeconds
				});
			}
			else
			{
				searchSelectionRecord.Count++;
				searchSelectionRecord.LastUsedUnixSeconds = lastUsedUnixSeconds;
			}
			value.Sort(CompareSearchSelectionRecords);
			if (value.Count > 8)
			{
				value.RemoveRange(8, value.Count - 8);
			}
		}
		TrimSearchSelections();
	}

	private void TrimSearchSelections()
	{
		if (config.SearchSelections.Count <= 200)
		{
			return;
		}
		foreach (string item in (from x in config.SearchSelections.OrderByDescending<KeyValuePair<string, List<SearchSelectionRecord>>, long>((KeyValuePair<string, List<SearchSelectionRecord>> x) => (x.Value.Count != 0) ? x.Value.Max((SearchSelectionRecord r) => r.LastUsedUnixSeconds) : 0).Skip(200)
			select x.Key).ToList())
		{
			config.SearchSelections.Remove(item);
		}
	}

	private List<AetheryteRecord> SortSearchMatches(string searchText, List<AetheryteRecord> matches)
	{
		string normalized = NormalizeSearchSelectionTerm(searchText);
		if (string.IsNullOrEmpty(normalized) || config.SearchSelections.Count == 0)
		{
			return matches;
		}
		return (from x in matches.Select((AetheryteRecord record, int index) => new
			{
				record = record,
				index = index,
				score = GetSearchSelectionScore(normalized, record)
			})
			orderby x.score descending, x.index
			select x.record).ToList();
	}

	private double GetSearchSelectionScore(string normalizedSearchText, AetheryteRecord record)
	{
		double num = 0.0;
		foreach (KeyValuePair<string, List<SearchSelectionRecord>> searchSelection in config.SearchSelections)
		{
			searchSelection.Deconstruct(out var key, out var value);
			string storedTerm = key;
			List<SearchSelectionRecord> source = value;
			double searchTermRelationWeight = GetSearchTermRelationWeight(normalizedSearchText, storedTerm);
			if (!(searchTermRelationWeight <= 0.0))
			{
				SearchSelectionRecord searchSelectionRecord = source.FirstOrDefault((SearchSelectionRecord x) => x.Key == record.ToString());
				if (searchSelectionRecord != null)
				{
					int num2 = Math.Min(searchSelectionRecord.Count, 20) * 100;
					double num3 = (double)searchSelectionRecord.LastUsedUnixSeconds / 86400.0;
					num = Math.Max(num, ((double)num2 + num3) * searchTermRelationWeight);
				}
			}
		}
		return num;
	}

	private static double GetSearchTermRelationWeight(string searchText, string storedTerm)
	{
		if (searchText == storedTerm)
		{
			return 1.0;
		}
		if (searchText.Length < 2 || storedTerm.Length < 2)
		{
			return 0.0;
		}
		if (searchText.StartsWith(storedTerm, StringComparison.Ordinal) || storedTerm.StartsWith(searchText, StringComparison.Ordinal))
		{
			return 0.75;
		}
		if (searchText.Contains(storedTerm, StringComparison.Ordinal) || storedTerm.Contains(searchText, StringComparison.Ordinal))
		{
			return 0.4;
		}
		return 0.0;
	}

	private static string NormalizeSearchSelectionTerm(string? searchText)
	{
		return string.Join(' ', (searchText ?? string.Empty).Trim().ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
	}

	private static int CompareSearchSelectionRecords(SearchSelectionRecord a, SearchSelectionRecord b)
	{
		int num = b.Count.CompareTo(a.Count);
		if (num != 0)
		{
			return num;
		}
		return b.LastUsedUnixSeconds.CompareTo(a.LastUsedUnixSeconds);
	}

	private static bool IsWithPermission()
	{
		if ((GameState.IsCN || GameState.IsTC) && !AuthState.IsPremium)
		{
			return Sheets.SpeedDetectionZones.ContainsKey(GameState.TerritoryType);
		}
		return true;
	}

	private void MigrateConfig()
	{
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Expected O, but got Unknown
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Expected O, but got Unknown
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Invalid comparison between Unknown and I4
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Invalid comparison between Unknown and I4
		//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d1: Expected O, but got Unknown
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Invalid comparison between Unknown and I4
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Expected O, but got Unknown
		//IL_056d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0572: Unknown result type (might be due to invalid IL or missing references)
		//IL_0589: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05df: Expected O, but got Unknown
		if (!File.Exists(base.ConfigFilePath))
		{
			return;
		}
		List<AetheryteRecord> list = OmenServiceBase<AetheryteRecordManager>.Instance().AllRecords.ToList();
		if (list.Count == 0)
		{
			return;
		}
		JObject val = JObject.Parse(File.ReadAllText(base.ConfigFilePath));
		bool flag = false;
		JToken obj = val["Remarks"];
		JObject val2 = (JObject)(object)((obj is JObject) ? obj : null);
		if (val2 != null)
		{
			List<string> list2 = (from p in val2.Properties()
				where !p.Name.StartsWith("AetheryteRecord.", StringComparison.Ordinal)
				select p.Name).ToList();
			if (list2.Count > 0)
			{
				foreach (string item in list2)
				{
					JToken val3 = val2[item];
					val2.Remove(item);
					string text = MigrateOldKeyToToString(item, list);
					if (text != null)
					{
						val2[text] = val3;
					}
				}
				flag = true;
			}
		}
		JToken obj2 = val["Positions"];
		JObject val4 = (JObject)(object)((obj2 is JObject) ? obj2 : null);
		if (val4 != null)
		{
			List<string> list3 = (from p in val4.Properties()
				where !p.Name.StartsWith("AetheryteRecord.", StringComparison.Ordinal)
				select p.Name).ToList();
			if (list3.Count > 0)
			{
				foreach (string item2 in list3)
				{
					JToken val5 = val4[item2];
					val4.Remove(item2);
					string text2 = MigrateOldKeyToToString(item2, list);
					if (text2 != null)
					{
						val4[text2] = val5;
					}
				}
				flag = true;
			}
		}
		JToken obj3 = val["Favorites"];
		JArray val6 = (JArray)(object)((obj3 is JArray) ? obj3 : null);
		if (val6 != null && ((IEnumerable<JToken>)val6).Any((JToken x) => (int)x.Type == 6 || ((int)x.Type == 8 && !Extensions.Value<string>((IEnumerable<JToken>)x).StartsWith("AetheryteRecord.", StringComparison.Ordinal))))
		{
			JArray val7 = new JArray();
			foreach (JToken item3 in val6)
			{
				uint result;
				uint? rowID = (((int)item3.Type == 6) ? new uint?(Extensions.Value<uint>((IEnumerable<JToken>)item3)) : (((int)item3.Type == 8 && uint.TryParse(Extensions.Value<string>((IEnumerable<JToken>)item3), out result)) ? new uint?(result) : ((uint?)null)));
				if (rowID.HasValue)
				{
					AetheryteRecord aetheryteRecord = list.FirstOrDefault((AetheryteRecord x) => x.RowID == rowID.Value);
					if (aetheryteRecord != null)
					{
						val7.Add(JToken.op_Implicit(aetheryteRecord.ToString()));
					}
				}
				else if ((int)item3.Type == 8)
				{
					val7.Add(JToken.op_Implicit(Extensions.Value<string>((IEnumerable<JToken>)item3)));
				}
			}
			val["Favorites"] = (JToken)(object)val7;
			flag = true;
		}
		JToken obj4 = val["RecentTeleports"];
		JArray val8 = (JArray)(object)((obj4 is JArray) ? obj4 : null);
		if (val8 != null && ((IEnumerable<JToken>)val8).Any((JToken x) => x[(object)"AetheryteID"] != null))
		{
			JArray val9 = new JArray();
			foreach (JToken item4 in val8)
			{
				JToken obj5 = item4[(object)"AetheryteID"];
				uint? aetheryteID = ((obj5 != null) ? new uint?(Extensions.Value<uint>((IEnumerable<JToken>)obj5)) : ((uint?)null));
				JToken obj6 = item4[(object)"SubIndex"];
				byte subIndex = (byte)((obj6 != null) ? Extensions.Value<byte>((IEnumerable<JToken>)obj6) : 0);
				if (aetheryteID.HasValue)
				{
					AetheryteRecord aetheryteRecord2 = list.FirstOrDefault((AetheryteRecord x) => x.RowID == aetheryteID.Value && x.SubIndex == subIndex);
					if (aetheryteRecord2 != null)
					{
						val9.Add((JToken)new JObject { ["Key"] = JToken.op_Implicit(aetheryteRecord2.ToString()) });
					}
				}
			}
			val["RecentTeleports"] = (JToken)(object)val9;
			flag = true;
		}
		JToken obj7 = val["SearchSelections"];
		JObject val10 = (JObject)(object)((obj7 is JObject) ? obj7 : null);
		if (val10 != null && val10.Properties().Any(delegate(JProperty p)
		{
			JToken value2 = p.Value;
			return ((IEnumerable<JToken>)((value2 is JArray) ? value2 : null))?.Any((JToken x) => x[(object)"AetheryteID"] != null) ?? false;
		}))
		{
			foreach (JProperty item5 in val10.Properties().ToList())
			{
				JToken value = item5.Value;
				JArray val11 = (JArray)(object)((value is JArray) ? value : null);
				if (val11 == null)
				{
					continue;
				}
				JArray val12 = new JArray();
				foreach (JToken item6 in val11)
				{
					JToken obj8 = item6[(object)"AetheryteID"];
					uint? aetheryteID2 = ((obj8 != null) ? new uint?(Extensions.Value<uint>((IEnumerable<JToken>)obj8)) : ((uint?)null));
					JToken obj9 = item6[(object)"SubIndex"];
					byte subIndex2 = (byte)((obj9 != null) ? Extensions.Value<byte>((IEnumerable<JToken>)obj9) : 0);
					if (aetheryteID2.HasValue)
					{
						AetheryteRecord aetheryteRecord3 = list.FirstOrDefault((AetheryteRecord x) => x.RowID == aetheryteID2.Value && x.SubIndex == subIndex2);
						if (!(aetheryteRecord3 == null))
						{
							JObject val13 = new JObject { ["Key"] = JToken.op_Implicit(aetheryteRecord3.ToString()) };
							JToken obj10 = item6[(object)"Count"];
							val13["Count"] = JToken.op_Implicit((obj10 == null) ? 1 : Extensions.Value<int>((IEnumerable<JToken>)obj10));
							JToken obj11 = item6[(object)"LastUsedUnixSeconds"];
							val13["LastUsedUnixSeconds"] = JToken.op_Implicit((obj11 != null) ? Extensions.Value<long>((IEnumerable<JToken>)obj11) : 0);
							val12.Add((JToken)val13);
						}
					}
				}
				item5.Value = (JToken)(object)val12;
			}
			flag = true;
		}
		if (flag)
		{
			File.WriteAllText(base.ConfigFilePath, ((JToken)val).ToString((Formatting)1));
		}
	}

	private static string? MigrateOldKeyToToString(string oldKey, List<AetheryteRecord> allRecords)
	{
		uint rowID;
		byte subIndex;
		if (oldKey.Contains('_'))
		{
			string[] array = oldKey.Split('_');
			if (array.Length != 2 || !uint.TryParse(array[0], out rowID) || !byte.TryParse(array[1], out subIndex))
			{
				return null;
			}
		}
		else
		{
			if (!uint.TryParse(oldKey, out rowID))
			{
				return null;
			}
			subIndex = 0;
		}
		return allRecords.FirstOrDefault((AetheryteRecord x) => x.RowID == rowID && x.SubIndex == subIndex)?.ToString();
	}

	private unsafe void OnPostUseAction(ref bool isPrevented, ref ActionType actionType, ref uint actionID, ref ulong targetID, ref uint extraParam, ref UseActionMode queueState, ref uint comboRouteID)
	{
		if ((int)actionType == 5 && actionID == 7)
		{
			isPrevented = true;
			if (((GameMain)GameMain.Instance()).CurrentContentFinderConditionId == 0 && !IConditionExtension.get_IsBetweenAreas(DalamudServiceExtension.Instance<ICondition>()) && Control.GetLocalPlayer() != null && UIModuleExtension.IsScreenReady())
			{
				UIGlobals.PlaySoundEffect(23u, (SoundData**)null, (SoundData**)null, (byte)0);
				ToggleDefaultPage();
			}
		}
	}

	private void OnCommand(string command, string args)
	{
		args = args.Trim();
		if (string.IsNullOrWhiteSpace(args))
		{
			ToggleDefaultPage();
			return;
		}
		AetheryteRecord aetheryteRecord = ((recordMatcher == null) ? SortSearchMatches(args, (from x in OmenServiceBase<AetheryteRecordManager>.Instance().AllRecords
			where x.Name.Contains(args, StringComparison.OrdinalIgnoreCase) || (config.Remarks.TryGetValue(x.ToString(), out string value) && value.Contains(args, StringComparison.OrdinalIgnoreCase))
			orderby x.IsAetheryte descending, x.Name.Length
			select x).ToList()).FirstOrDefault() : SortSearchMatches(args, recordMatcher.Search(args, CompareCommandResult, int.MaxValue).ToList()).FirstOrDefault());
		if (!(aetheryteRecord == null))
		{
			HandleTeleport(aetheryteRecord, args);
		}
		static int CompareCommandResult(AetheryteRecord a, AetheryteRecord b)
		{
			int num = b.IsAetheryte.CompareTo(a.IsAetheryte);
			if (num != 0)
			{
				return num;
			}
			return a.Name.Length.CompareTo(b.Name.Length);
		}
	}

	private void OnPreInputIDPressed(ref bool? overrideResult, ref InputId id)
	{
		if ((((Window)base.Overlay).IsOpen || ((Window)fullWindow).IsOpen) && (int)id == 281 && DalamudServiceExtension.Instance<IKeyState>()[(VirtualKey)13])
		{
			shouldFocusSearchBar = true;
			overrideResult = false;
		}
	}

	protected override void OverlayOnOpen()
	{
		RefreshFavoritesInfo();
		RefreshDefaultOverlayItems();
		recordMatcher?.Dispose();
		recordMatcher = CreateRecordMatcher();
		searchWord = string.Empty;
		selectedIndex = 0;
		if (config.FocusSearchOnOpen)
		{
			shouldFocusSearchBar = true;
		}
		hasUsedArrowKeys = true;
		hoveredAetheryte = null;
		pinnedAetheryte = null;
		lastMousePos = ImGui.GetMousePos();
		isSearchingInputting = false;
		lastMousePosForInput = ImGui.GetMousePos();
	}

	protected unsafe override void OverlayUI()
	{
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Expected I4, but got Unknown
		//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04df: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e8: Unknown result type (might be due to invalid IL or missing references)
		Vector2 mousePos = ImGui.GetMousePos();
		if (isSearchingInputting && mousePos != lastMousePosForInput)
		{
			isSearchingInputting = false;
		}
		lastMousePosForInput = mousePos;
		if (hasUsedArrowKeys && mousePos != lastMousePos)
		{
			hasUsedArrowKeys = false;
		}
		lastMousePos = mousePos;
		if (!hasUsedArrowKeys)
		{
			hoveredAetheryte = null;
		}
		if (config.CloseOnLoseFocus && !ImGui.IsWindowFocused((ImGuiFocusedFlags)3) && pinnedAetheryte == null && !ImGui.IsPopupOpen(ImU8String.op_Implicit("AetheryteContextPopup"), (ImGuiPopupFlags)0))
		{
			((Window)base.Overlay).IsOpen = false;
			return;
		}
		if (DalamudServiceExtension.Instance<IKeyState>()[(VirtualKey)27])
		{
			((Window)base.Overlay).IsOpen = false;
			if (Addons.SystemMenu != null)
			{
				((AtkUnitBase)Addons.SystemMenu).Close(true);
			}
		}
		visibleItems.Clear();
		if (string.IsNullOrWhiteSpace(searchWord))
		{
			visibleItems.AddRange(defaultOverlayItems);
			visibleItems.Add(new OverlayListItem
			{
				IsShowMore = true,
				DrawSeparatorBefore = false,
				Name = LanguageManager.Get("BetterTeleport-ShowMore")
			});
		}
		else
		{
			List<AetheryteRecord> list = ((recordMatcher == null) ? SortSearchMatches(searchWord, OmenServiceBase<AetheryteRecordManager>.Instance().AllRecords.Where((AetheryteRecord x) => x.Name.Contains(searchWord, StringComparison.OrdinalIgnoreCase) || (config.Remarks.TryGetValue(x.ToString(), out string value) && value.Contains(searchWord, StringComparison.OrdinalIgnoreCase))).ToList()) : SortSearchMatches(searchWord, recordMatcher.Search(searchWord, null, int.MaxValue).ToList()));
			int num = Math.Min(8, list.Count);
			for (int num2 = 0; num2 < num; num2++)
			{
				AetheryteRecord aetheryteRecord = list[num2];
				visibleItems.Add(new OverlayListItem
				{
					Record = aetheryteRecord,
					IsShowMore = false,
					Name = aetheryteRecord.Name
				});
			}
			visibleItems.Add(new OverlayListItem
			{
				IsShowMore = true,
				Name = LanguageManager.Get("BetterTeleport-ShowMore")
			});
			if (list.Count == 1)
			{
				hoveredAetheryte = list[0];
			}
		}
		if (selectedIndex < 0)
		{
			selectedIndex = 0;
		}
		if (visibleItems.Count > 0 && selectedIndex >= visibleItems.Count)
		{
			selectedIndex = visibleItems.Count - 1;
		}
		ImGui.SetNextItemWidth(-1f);
		if (shouldFocusSearchBar)
		{
			ImGui.SetWindowFocus();
			ImGui.SetKeyboardFocusHere();
			((AtkStage)AtkStage.Instance()).ClearFocus();
			shouldFocusSearchBar = false;
			if (OmenServiceBase<LocalPlayerState>.Instance().IsMoving)
			{
				OmenServiceBase<ChatManager>.Instance().SendCommand("/automove on");
			}
		}
		if (ImGui.InputTextWithHint(ImU8String.op_Implicit("###BetterTeleportQuickSearch"), ImU8String.op_Implicit(LanguageManager.Get("PleaseSearch")), ref searchWord, 128, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null))
		{
			isSearchingInputting = true;
			hoveredAetheryte = null;
		}
		bool flag = ImGui.IsItemFocused();
		ImGui.Spacing();
		for (int num3 = 0; num3 < visibleItems.Count; num3++)
		{
			DrawSearchItem(num3, visibleItems[num3], selectedIndex == num3);
		}
		if ((GetAsyncKeyState((int)PluginConfig.Instance().ConflictKeyBinding.Keyboard) & 0x8000) != 0 && hoveredAetheryte != null)
		{
			pinnedAetheryte = hoveredAetheryte;
		}
		if (((Window)base.Overlay).IsOpen && visibleItems.Count > 0)
		{
			if (ImGui.IsKeyPressed((ImGuiKey)516))
			{
				selectedIndex = (selectedIndex + 1) % visibleItems.Count;
				hasUsedArrowKeys = true;
				if (visibleItems[selectedIndex].Record != null)
				{
					hoveredAetheryte = visibleItems[selectedIndex].Record;
				}
			}
			if (ImGui.IsKeyPressed((ImGuiKey)515))
			{
				selectedIndex = (selectedIndex - 1 + visibleItems.Count) % visibleItems.Count;
				hasUsedArrowKeys = true;
				if (visibleItems[selectedIndex].Record != null)
				{
					hoveredAetheryte = visibleItems[selectedIndex].Record;
				}
			}
			if (ImGui.IsKeyPressed((ImGuiKey)525) && !ImGui.IsWindowAppearing())
			{
				if (hasUsedArrowKeys || flag)
				{
					TriggerListItem(visibleItems[selectedIndex]);
				}
				else
				{
					shouldFocusSearchBar = true;
				}
			}
			ImGuiIOPtr iO = ImGui.GetIO();
			if (((ImGuiIOPtr)(ref iO)).KeyCtrl)
			{
				for (int num4 = 0; num4 < 9; num4++)
				{
					int num5 = 537 + num4;
					ImGuiKey val = (ImGuiKey)(601 + num4);
					if (!ImGui.IsKeyPressed((ImGuiKey)num5) && !ImGui.IsKeyPressed(val))
					{
						continue;
					}
					if (num4 == 8)
					{
						OverlayListItem item = visibleItems.FirstOrDefault((OverlayListItem x) => x.IsShowMore);
						if (item.Name != null)
						{
							TriggerListItem(item);
						}
					}
					else if (num4 < visibleItems.Count)
					{
						TriggerListItem(visibleItems[num4]);
					}
				}
			}
		}
		ImGui.Spacing();
		ImGui.Separator();
		ImGui.Spacing();
		DrawBottomToolbar();
		DrawHoveredTooltip();
	}

	protected override void OverlayOnClose()
	{
		config.Save(this);
	}

	private void DrawSearchItem(int index, OverlayListItem item, bool isSelected)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		if (item.DrawSeparatorBefore)
		{
			ImGui.Separator();
			ImGui.Spacing();
		}
		if (!item.IsShowMore && (object)item.Record != null)
		{
			DrawAetheryteItem(item.Record, index, isSelected, drawHotkeyBadge: true, searchWord);
			return;
		}
		Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
		float x = ImGui.GetContentRegionAvail().X;
		float textLineHeight = ImGui.GetTextLineHeight();
		ImGuiStylePtr style = ImGui.GetStyle();
		float x2 = ((ImGuiStylePtr)(ref style)).ItemSpacing.X;
		float num = textLineHeight * 2.2f + x2;
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(5, 1);
		((ImU8String)(ref val)).AppendLiteral("item_");
		((ImU8String)(ref val)).AppendFormatted<int>(index);
		ImGui.PushID(val);
		if (ImGui.InvisibleButton(ImU8String.op_Implicit("##ItemBtn"), new Vector2(x, num), (ImGuiButtonFlags)0))
		{
			hasUsedArrowKeys = false;
			TriggerListItem(item);
		}
		bool flag = ImGui.IsItemHovered() && !hasUsedArrowKeys;
		ImGui.PopID();
		if (flag && !isSearchingInputting)
		{
			selectedIndex = index;
		}
		ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
		if (isSelected || flag)
		{
			uint colorU = ImGui.GetColorU32((ImGuiCol)(isSelected ? 26 : 25));
			((ImDrawListPtr)(ref windowDrawList)).AddRectFilled(cursorScreenPos, cursorScreenPos + new Vector2(x, num), colorU, 4f);
		}
		float x3 = cursorScreenPos.X + x2 + 6f;
		string name = item.Name;
		float y = cursorScreenPos.Y + (num - ImGui.GetTextLineHeight()) / 2f;
		((ImDrawListPtr)(ref windowDrawList)).AddText(new Vector2(x3, y), ImGui.GetColorU32((ImGuiCol)0), ImU8String.op_Implicit(name));
		float num2 = cursorScreenPos.X + x - x2 - 6f;
		string text = "Ctrl+9";
		Vector2 vector = ImGui.CalcTextSize(ImU8String.op_Implicit(text), false, -1f);
		float num3 = vector.X + 8f;
		float num4 = vector.Y + 4f;
		Vector2 vector2 = new Vector2(num2 - num3, cursorScreenPos.Y + (num - num4) / 2f);
		uint colorU2 = ImGui.GetColorU32((ImGuiCol)7);
		((ImDrawListPtr)(ref windowDrawList)).AddRectFilled(vector2, vector2 + new Vector2(num3, num4), colorU2, 4f);
		((ImDrawListPtr)(ref windowDrawList)).AddRect(vector2, vector2 + new Vector2(num3, num4), ImGui.GetColorU32((ImGuiCol)5), 4f);
		((ImDrawListPtr)(ref windowDrawList)).AddText(vector2 + new Vector2(4f, 2f), ImGui.GetColorU32((ImGuiCol)1), ImU8String.op_Implicit(text));
		ImGui.SetCursorScreenPos(cursorScreenPos + new Vector2(0f, num + 2f));
	}

	private void TriggerListItem(OverlayListItem item)
	{
		if (item.IsShowMore)
		{
			if (fullWindow != null)
			{
				((Window)fullWindow).IsOpen = true;
			}
			((Window)base.Overlay).IsOpen = false;
		}
		else if (item.Record != null)
		{
			HandleTeleport(item.Record, searchWord);
		}
	}

	private unsafe List<AetheryteRecord> GetCombinedRecentRecords()
	{
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		List<AetheryteRecord> list = new List<AetheryteRecord>();
		HashSet<string> hashSet = new HashSet<string>();
		foreach (RecentRecord rec in config.RecentTeleports)
		{
			if (!string.IsNullOrEmpty(rec.Key) && hashSet.Add(rec.Key))
			{
				AetheryteRecord aetheryteRecord = OmenServiceBase<AetheryteRecordManager>.Instance().AllRecords.FirstOrDefault((AetheryteRecord x) => x.ToString() == rec.Key);
				if (aetheryteRecord != null)
				{
					list.Add(aetheryteRecord);
				}
			}
		}
		TeleportHistoryModule* ptr = TeleportHistoryModule.Instance();
		if (ptr != null)
		{
			Span<TeleportHistoryEntry> history = ((TeleportHistoryModule)ptr).History;
			for (int num = 0; num < history.Length; num++)
			{
				TeleportHistoryEntry rec2 = history[num];
				AetheryteRecord aetheryteRecord2 = OmenServiceBase<AetheryteRecordManager>.Instance().AllRecords.FirstOrDefault((AetheryteRecord x) => x.RowID == rec2.AetheryteId && x.SubIndex == rec2.SubIndex);
				if (aetheryteRecord2 != null && hashSet.Add(aetheryteRecord2.ToString()))
				{
					list.Add(aetheryteRecord2);
				}
			}
		}
		return list;
	}

	private List<AetheryteRecord> GetSortedSpecialRecords(List<AetheryteRecord> combinedHistory)
	{
		HashSet<(uint, byte, uint)> hashSet = new HashSet<(uint, byte, uint)>();
		List<AetheryteRecord> list = new List<AetheryteRecord>();
		foreach (AetheryteRecord favorite in favorites)
		{
			if (hashSet.Add((favorite.RowID, favorite.SubIndex, favorite.Group)))
			{
				list.Add(favorite);
			}
		}
		AetheryteRecord aetheryteRecord = OmenServiceBase<AetheryteRecordManager>.Instance().AllRecords.FirstOrDefault((AetheryteRecord x) => x.State == AetheryteRecordState.Home);
		if (aetheryteRecord != null && hashSet.Add((aetheryteRecord.RowID, aetheryteRecord.SubIndex, aetheryteRecord.Group)))
		{
			list.Add(aetheryteRecord);
		}
		List<AetheryteRecord> list2 = OmenServiceBase<AetheryteRecordManager>.Instance().AllRecords.Where((AetheryteRecord x) => x.State == AetheryteRecordState.Free).ToList();
		int num = 0;
		foreach (AetheryteRecord item in list2)
		{
			if (num >= 2)
			{
				break;
			}
			if (hashSet.Add((item.RowID, item.SubIndex, item.Group)))
			{
				list.Add(item);
				num++;
			}
		}
		List<AetheryteRecord> list3 = OmenServiceBase<AetheryteRecordManager>.Instance().AllRecords.Where((AetheryteRecord x) => x.State == AetheryteRecordState.Favorite).ToList();
		int num2 = 0;
		foreach (AetheryteRecord item2 in list3)
		{
			if (num2 >= 3)
			{
				break;
			}
			if (hashSet.Add((item2.RowID, item2.SubIndex, item2.Group)))
			{
				list.Add(item2);
				num2++;
			}
		}
		Dictionary<(uint RowID, byte SubIndex, byte Group), int> historyIndices = combinedHistory.Select((AetheryteRecord record, int index) => new { record, index }).ToDictionary(x => (RowID: x.record.RowID, SubIndex: x.record.SubIndex, Group: x.record.Group), x => x.index);
		return list.OrderBy((AetheryteRecord x) => historyIndices.GetValueOrDefault((x.RowID, x.SubIndex, x.Group), int.MaxValue)).ToList();
	}

	public void RefreshDefaultOverlayItems()
	{
		defaultOverlayItems.Clear();
		List<AetheryteRecord> combinedRecentRecords = GetCombinedRecentRecords();
		List<AetheryteRecord> sortedSpecialRecords = GetSortedSpecialRecords(combinedRecentRecords);
		List<AetheryteRecord> list = new List<AetheryteRecord>();
		List<AetheryteRecord> list2 = new List<AetheryteRecord>();
		HashSet<(uint, byte)> hashSet = new HashSet<(uint, byte)>();
		foreach (AetheryteRecord item in combinedRecentRecords)
		{
			if (list.Count >= 2)
			{
				break;
			}
			if (hashSet.Add((item.RowID, item.SubIndex)))
			{
				list.Add(item);
			}
		}
		int num = 6;
		foreach (AetheryteRecord item2 in sortedSpecialRecords)
		{
			if (list2.Count >= num)
			{
				break;
			}
			if (hashSet.Add((item2.RowID, item2.SubIndex)))
			{
				list2.Add(item2);
			}
		}
		if (list.Count + list2.Count < 8)
		{
			foreach (AetheryteRecord item3 in combinedRecentRecords)
			{
				if (list.Count + list2.Count >= 8)
				{
					break;
				}
				if (hashSet.Add((item3.RowID, item3.SubIndex)))
				{
					list.Add(item3);
				}
			}
		}
		foreach (AetheryteRecord item4 in list)
		{
			defaultOverlayItems.Add(new OverlayListItem
			{
				Record = item4,
				IsShowMore = false,
				DrawSeparatorBefore = false,
				Name = item4.Name
			});
		}
		for (int i = 0; i < list2.Count; i++)
		{
			AetheryteRecord aetheryteRecord = list2[i];
			bool drawSeparatorBefore = i == 0 && list.Count > 0;
			defaultOverlayItems.Add(new OverlayListItem
			{
				Record = aetheryteRecord,
				IsShowMore = false,
				DrawSeparatorBefore = drawSeparatorBefore,
				Name = aetheryteRecord.Name
			});
		}
	}

	[DllImport("user32.dll")]
	private static extern short GetAsyncKeyState(int vKey);

	private void DrawAetheryteTabIcon(AetheryteRecord? aetheryte, int category = -1)
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		if (category < 0 && aetheryte != null)
		{
			category = GetAetheryteCategory(aetheryte);
		}
		if (category >= 0)
		{
			Vector2 itemRectMin = ImGui.GetItemRectMin();
			Vector2 itemRectMax = ImGui.GetItemRectMax();
			float num = 20f * Globals.GlobalUIScale;
			DrawAetheryteTabRegionIcon(position: new Vector2((itemRectMin.X + itemRectMax.X - num) / 2f, (itemRectMin.Y + itemRectMax.Y - num) / 2f), drawList: ImGui.GetWindowDrawList(), category: category, size: num);
		}
	}

	private static void PushTabIconPadding()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		float x = 14f * Globals.GlobalUIScale;
		ImGuiStylePtr style = ImGui.GetStyle();
		ImGui.PushStyleVar((ImGuiStyleVar)10, new Vector2(x, ((ImGuiStylePtr)(ref style)).FramePadding.Y));
	}

	private void DrawAetheryteSettingTabIcon()
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		Vector2 itemRectMin = ImGui.GetItemRectMin();
		Vector2 itemRectMax = ImGui.GetItemRectMax();
		float num = 20f * Globals.GlobalUIScale;
		DrawAetheryteSettingIcon(position: new Vector2((itemRectMin.X + itemRectMax.X - num) / 2f, (itemRectMin.Y + itemRectMax.Y - num) / 2f), drawList: ImGui.GetWindowDrawList(), size: num);
	}

	public unsafe void DrawFullWindowUI()
	{
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0666: Unknown result type (might be due to invalid IL or missing references)
		//IL_066d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0672: Unknown result type (might be due to invalid IL or missing references)
		//IL_0674: Unknown result type (might be due to invalid IL or missing references)
		//IL_0695: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06be: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ac9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ace: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0abc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0abd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ae2: Unknown result type (might be due to invalid IL or missing references)
		//IL_071a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0727: Unknown result type (might be due to invalid IL or missing references)
		//IL_072c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0afe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b03: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b16: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b22: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b36: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b3b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b40: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_072e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b42: Unknown result type (might be due to invalid IL or missing references)
		//IL_0809: Unknown result type (might be due to invalid IL or missing references)
		//IL_080e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0825: Unknown result type (might be due to invalid IL or missing references)
		//IL_0827: Unknown result type (might be due to invalid IL or missing references)
		//IL_0829: Unknown result type (might be due to invalid IL or missing references)
		//IL_082e: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bcc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bd1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bf7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c02: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c08: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b64: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b69: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b79: Unknown result type (might be due to invalid IL or missing references)
		//IL_0892: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_09de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a0e: Unknown result type (might be due to invalid IL or missing references)
		bool flag = ImGui.IsWindowAppearing() || isJustOpened;
		isJustOpened = false;
		Vector2 mousePos = ImGui.GetMousePos();
		if (hasUsedArrowKeys && mousePos != lastMousePos)
		{
			hasUsedArrowKeys = false;
		}
		lastMousePos = mousePos;
		hoveredAetheryte = null;
		bool flag2 = string.IsNullOrWhiteSpace(fullSearchWord);
		if (string.IsNullOrEmpty(activeTabName))
		{
			activeTabName = ((favorites.Count > 0) ? "Favorite" : (OmenServiceBase<AetheryteRecordManager>.Instance().Records.Keys.FirstOrDefault() ?? "Setting"));
			tabToSelect = activeTabName;
			shouldFocusSearchBar = true;
			selectedIndex = 0;
			hasUsedArrowKeys = false;
		}
		if (config.CloseOnLoseFocus && !ImGui.IsWindowFocused((ImGuiFocusedFlags)3) && pinnedAetheryte == null && !ImGui.IsPopupOpen(ImU8String.op_Implicit("AetheryteContextPopup"), (ImGuiPopupFlags)0))
		{
			((Window)fullWindow).IsOpen = false;
			return;
		}
		List<string> list = new List<string>();
		if (favorites.Count > 0)
		{
			list.Add("Favorite");
		}
		AgentLobby* ptr = AgentLobby.Instance();
		if (ptr != null)
		{
			foreach (string key2 in OmenServiceBase<AetheryteRecordManager>.Instance().Records.Keys)
			{
				list.Add(key2);
			}
		}
		list.Add("Setting");
		if (!list.Contains(activeTabName))
		{
			activeTabName = list.FirstOrDefault() ?? "Setting";
		}
		if (flag2 && list.Count > 1)
		{
			if (ImGui.IsKeyPressed((ImGuiKey)513))
			{
				int num = list.IndexOf(activeTabName);
				if (num >= 0)
				{
					int index = (num - 1 + list.Count) % list.Count;
					activeTabName = list[index];
					tabToSelect = activeTabName;
					selectedIndex = 0;
					hasUsedArrowKeys = false;
					hoveredAetheryte = null;
					shouldScrollToSelected = true;
				}
			}
			else if (ImGui.IsKeyPressed((ImGuiKey)514))
			{
				int num2 = list.IndexOf(activeTabName);
				if (num2 >= 0)
				{
					int index2 = (num2 + 1) % list.Count;
					activeTabName = list[index2];
					tabToSelect = activeTabName;
					selectedIndex = 0;
					hasUsedArrowKeys = false;
					hoveredAetheryte = null;
					shouldScrollToSelected = true;
				}
			}
		}
		List<AetheryteRecord> list2 = new List<AetheryteRecord>();
		List<AetheryteRecord> value;
		if (!flag2)
		{
			list2.AddRange(fullSearchResult);
		}
		else if (activeTabName == "Favorite")
		{
			list2.AddRange(favorites);
		}
		else if (OmenServiceBase<AetheryteRecordManager>.Instance().Records.TryGetValue(activeTabName, out value))
		{
			foreach (AetheryteRecord item in value.ToList())
			{
				if (item.IsUnlocked())
				{
					list2.Add(item);
				}
			}
		}
		string text = "###SearchFull";
		if (isNeedToLoseFocusSearchBar)
		{
			text = "###SearchFull_LoseFocus";
			isNeedToLoseFocusSearchBar = false;
		}
		if (shouldFocusSearchBar)
		{
			ImGui.SetWindowFocus();
			ImGui.SetKeyboardFocusHere();
			((AtkStage)AtkStage.Instance()).ClearFocus();
			shouldFocusSearchBar = false;
			if (OmenServiceBase<LocalPlayerState>.Instance().IsMoving)
			{
				OmenServiceBase<ChatManager>.Instance().SendCommand("/automove on");
			}
		}
		ImGuiStylePtr style;
		float nextItemWidth;
		if (!flag2)
		{
			float num3 = 0f - ImGui.GetFrameHeight();
			style = ImGui.GetStyle();
			nextItemWidth = num3 - ((ImGuiStylePtr)(ref style)).ItemSpacing.X;
		}
		else
		{
			nextItemWidth = -1f;
		}
		ImGui.SetNextItemWidth(nextItemWidth);
		if (ImGui.InputTextWithHint(ImU8String.op_Implicit(text), ImU8String.op_Implicit(LanguageManager.Get("PleaseSearch")), ref fullSearchWord, 128, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null))
		{
			if (string.IsNullOrWhiteSpace(fullSearchWord))
			{
				fullSearchResult = new List<AetheryteRecord>();
			}
			else if (recordMatcher != null)
			{
				fullSearchResult = SortSearchMatches(fullSearchWord, recordMatcher.Search(fullSearchWord, null, int.MaxValue).ToList());
			}
			else
			{
				fullSearchResult = SortSearchMatches(fullSearchWord, OmenServiceBase<AetheryteRecordManager>.Instance().AllRecords.Where((AetheryteRecord aetheryteRecord2) => aetheryteRecord2.Name.Contains(fullSearchWord, StringComparison.OrdinalIgnoreCase) || (config.Remarks.TryGetValue(aetheryteRecord2.ToString(), out string value3) && value3.Contains(fullSearchWord, StringComparison.OrdinalIgnoreCase))).ToList());
			}
			selectedIndex = 0;
		}
		if (!flag2)
		{
			ImGui.SameLine();
			if (ImGuiOm.ButtonIcon("Clear", (FontAwesomeIcon)61453))
			{
				fullSearchWord = string.Empty;
				fullSearchResult = new List<AetheryteRecord>();
			}
		}
		bool flag3 = ImGui.IsItemFocused();
		if (list2.Count > 0)
		{
			if (selectedIndex < 0)
			{
				selectedIndex = 0;
			}
			if (selectedIndex >= list2.Count)
			{
				selectedIndex = list2.Count - 1;
			}
			if (ImGui.IsKeyPressed((ImGuiKey)516))
			{
				selectedIndex = (selectedIndex + 1) % list2.Count;
				hasUsedArrowKeys = true;
				hoveredAetheryte = list2[selectedIndex];
				shouldScrollToSelected = true;
			}
			if (ImGui.IsKeyPressed((ImGuiKey)515))
			{
				selectedIndex = (selectedIndex - 1 + list2.Count) % list2.Count;
				hasUsedArrowKeys = true;
				hoveredAetheryte = list2[selectedIndex];
				shouldScrollToSelected = true;
			}
			if (ImGui.IsKeyPressed((ImGuiKey)525) && !flag && (hasUsedArrowKeys || flag3))
			{
				HandleTeleport(list2[selectedIndex], flag2 ? null : fullSearchWord);
			}
		}
		ImGui.Spacing();
		if (fullSearchResult.Count > 0 || !flag2)
		{
			ChildDisposable val = ImRaii.Child(ImU8String.op_Implicit("###SearchResultChildFull"), new Vector2(0f, 0f - ImGui.GetFrameHeightWithSpacing()), false, (ImGuiWindowFlags)128);
			try
			{
				if (ChildDisposable.op_Implicit(val) && fullSearchResult.Count != 0)
				{
					List<AetheryteRecord> list3 = fullSearchResult.ToList();
					for (int num4 = 0; num4 < list3.Count; num4++)
					{
						DrawAetheryteItem(list3[num4], num4, selectedIndex == num4, drawHotkeyBadge: false, fullSearchWord);
						if (selectedIndex == num4 && shouldScrollToSelected)
						{
							ImGui.SetScrollHereY();
							shouldScrollToSelected = false;
						}
					}
				}
			}
			finally
			{
				((ChildDisposable)(ref val)).Dispose();
			}
		}
		else
		{
			TabBarDisposable val2 = ImRaii.TabBar(ImU8String.op_Implicit("###AetherytesTabBarFull"), (ImGuiTabBarFlags)33);
			try
			{
				if (TabBarDisposable.op_Implicit(val2))
				{
					bool flag4 = false;
					if (favorites.Count > 0)
					{
						ImGuiTabItemFlags val3 = (ImGuiTabItemFlags)0;
						if (tabToSelect == "Favorite")
						{
							val3 = (ImGuiTabItemFlags)(val3 | 2);
						}
						PushTabIconPadding();
						TabItemDisposable val4 = ImRaii.TabItem(ImU8String.op_Implicit("##TabItemFullFavorite"), val3);
						try
						{
							ImGui.PopStyleVar();
							DrawAetheryteTabIcon(null, 200);
							ImGuiOm.TooltipHover(LanguageManager.Get("Favorite"));
							if (TabItemDisposable.op_Implicit(val4))
							{
								activeTabName = "Favorite";
								Vector2 vector = new Vector2(0f, 0f - ImGui.GetFrameHeightWithSpacing());
								ChildDisposable val5 = ImRaii.Child(ImU8String.op_Implicit("###FavoriteChildFull"), vector, false, (ImGuiWindowFlags)128);
								try
								{
									if (ChildDisposable.op_Implicit(val5))
									{
										List<AetheryteRecord> list4 = favorites.ToList();
										for (int num5 = 0; num5 < list4.Count; num5++)
										{
											DrawAetheryteItem(list4[num5], num5, selectedIndex == num5, drawHotkeyBadge: false);
											if (selectedIndex == num5 && shouldScrollToSelected)
											{
												ImGui.SetScrollHereY();
												shouldScrollToSelected = false;
											}
										}
									}
								}
								finally
								{
									((ChildDisposable)(ref val5)).Dispose();
								}
							}
						}
						finally
						{
							((TabItemDisposable)(ref val4)).Dispose();
						}
					}
					if (ptr != null)
					{
						foreach (KeyValuePair<string, List<AetheryteRecord>> record in OmenServiceBase<AetheryteRecordManager>.Instance().Records)
						{
							record.Deconstruct(out var key, out var value2);
							string text2 = key;
							List<AetheryteRecord> list5 = value2;
							ImGuiTabItemFlags val6 = (ImGuiTabItemFlags)0;
							if (tabToSelect == text2)
							{
								val6 = (ImGuiTabItemFlags)(val6 | 2);
							}
							PushTabIconPadding();
							ImU8String val7 = new ImU8String(13, 1);
							((ImU8String)(ref val7)).AppendLiteral("##TabItemFull");
							((ImU8String)(ref val7)).AppendFormatted<string>(text2);
							TabItemDisposable val8 = ImRaii.TabItem(val7, val6);
							try
							{
								ImGui.PopStyleVar();
								AetheryteRecord aetheryteRecord = list5.FirstOrDefault((AetheryteRecord aetheryteRecord2) => aetheryteRecord2.IsUnlocked());
								DrawAetheryteTabIcon(aetheryteRecord);
								if (aetheryteRecord != null)
								{
									ImGuiOm.TooltipHover(GetAetheryteTabLabel(aetheryteRecord, text2));
								}
								else
								{
									ImGuiOm.TooltipHover(text2);
								}
								if (!val8)
								{
									continue;
								}
								activeTabName = text2;
								Vector2 vector2 = new Vector2(0f, 0f - ImGui.GetFrameHeightWithSpacing());
								ImU8String val9 = new ImU8String(12, 1);
								((ImU8String)(ref val9)).AppendLiteral("###");
								((ImU8String)(ref val9)).AppendFormatted<string>(text2);
								((ImU8String)(ref val9)).AppendLiteral("ChildFull");
								ChildDisposable val10 = ImRaii.Child(val9, vector2, false, (ImGuiWindowFlags)128);
								try
								{
									if (!val10)
									{
										continue;
									}
									string text3 = string.Empty;
									int num6 = -1;
									int num7 = 0;
									foreach (AetheryteRecord item2 in list5)
									{
										if (!item2.IsUnlocked())
										{
											continue;
										}
										bool flag5 = false;
										if (item2.Group == 0)
										{
											if (text3 != item2.RegionName)
											{
												flag5 = true;
												text3 = item2.RegionName;
											}
										}
										else if (num6 != item2.Group)
										{
											flag5 = true;
											num6 = item2.Group;
										}
										if (flag5)
										{
											ImGui.Spacing();
											string regionName = item2.RegionName;
											uint colorU = ImGui.GetColorU32((ImGuiCol)24);
											Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
											float x = ImGui.GetContentRegionAvail().X;
											float textLineHeightWithSpacing = ImGui.GetTextLineHeightWithSpacing();
											ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
											((ImDrawListPtr)(ref windowDrawList)).AddRectFilled(cursorScreenPos, cursorScreenPos + new Vector2(x, textLineHeightWithSpacing), colorU, 4f);
											float cursorPosX = ImGui.GetCursorPosX();
											style = ImGui.GetStyle();
											ImGui.SetCursorPosX(cursorPosX + ((ImGuiStylePtr)(ref style)).ItemSpacing.X * 2f);
											ImGui.AlignTextToFramePadding();
											Vector4 vector3 = ImGui.GetColorU32((ImGuiCol)0).ToVector4();
											ImGui.TextColored(ref vector3, ImU8String.op_Implicit(regionName));
											ImGui.Spacing();
										}
										DrawAetheryteItem(item2, num7, selectedIndex == num7, drawHotkeyBadge: false);
										if (selectedIndex == num7 && shouldScrollToSelected)
										{
											ImGui.SetScrollHereY();
											shouldScrollToSelected = false;
										}
										num7++;
									}
								}
								finally
								{
									((ChildDisposable)(ref val10)).Dispose();
								}
							}
							finally
							{
								((TabItemDisposable)(ref val8)).Dispose();
							}
						}
					}
					ImGuiTabItemFlags val11 = (ImGuiTabItemFlags)0;
					if (tabToSelect == "Setting")
					{
						val11 = (ImGuiTabItemFlags)(val11 | 2);
					}
					PushTabIconPadding();
					TabItemDisposable val12 = ImRaii.TabItem(ImU8String.op_Implicit("##TabItemFullSetting"), val11);
					try
					{
						ImGui.PopStyleVar();
						DrawAetheryteSettingTabIcon();
						if (TabItemDisposable.op_Implicit(val12))
						{
							activeTabName = "Setting";
							flag4 = true;
							ImU8String val13 = new ImU8String(0, 1);
							((ImU8String)(ref val13)).AppendFormatted<string>(LuminaWrapper.GetAddonText(8522u));
							ImGui.TextUnformatted(val13);
							ComboDisposable val14 = ImRaii.Combo(ImU8String.op_Implicit("###TeleportUsageTypeComboFull"), ImU8String.op_Implicit(TicketUsageTypes[TicketUsageType]));
							try
							{
								if (ComboDisposable.op_Implicit(val14))
								{
									foreach (KeyValuePair<uint, string> ticketUsageType in TicketUsageTypes)
									{
										ImU8String val15 = new ImU8String(0, 1);
										((ImU8String)(ref val15)).AppendFormatted<string>(ticketUsageType.Value);
										if (ImGui.Selectable(val15, ticketUsageType.Key == TicketUsageType, (ImGuiSelectableFlags)0, default(Vector2)))
										{
											TicketUsageType = ticketUsageType.Key;
										}
									}
								}
							}
							finally
							{
								((ComboDisposable)(ref val14)).Dispose();
							}
							ImU8String val16 = new ImU8String(0, 1);
							((ImU8String)(ref val16)).AppendFormatted<string>(LuminaWrapper.GetAddonText(8528u));
							ImGui.TextUnformatted(val16);
							uint ticketUsageGilSetting = TicketUsageGilSetting;
							if (ImGui.InputUInt(ImU8String.op_Implicit("###GilInputFull"), ref ticketUsageGilSetting, 0u, 0u, default(ImU8String), (ImGuiInputTextFlags)0))
							{
								TicketUsageGilSetting = ticketUsageGilSetting;
							}
							DrawGeneralSettings();
						}
						else
						{
							ImGuiOm.TooltipHover(LuminaWrapper.GetAddonText(8516u));
						}
					}
					finally
					{
						((TabItemDisposable)(ref val12)).Dispose();
					}
					if (!flag4)
					{
						DrawBottomToolbar();
					}
				}
			}
			finally
			{
				((TabBarDisposable)(ref val2)).Dispose();
			}
		}
		tabToSelect = null;
		DrawHoveredTooltip();
	}

	protected override void ConfigUI()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(1, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("Command"));
		((ImU8String)(ref val)).AppendLiteral(":");
		ImGui.TextColored(ref vector, val);
		ImGui.SameLine();
		ImU8String val2 = default(ImU8String);
		((ImU8String)(ref val2))._002Ector(1, 2);
		((ImU8String)(ref val2)).AppendFormatted<string>("/pdrtelepo");
		((ImU8String)(ref val2)).AppendLiteral(" ");
		((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get("BetterTeleport-CommandHelp"));
		ImGui.TextWrapped(val2);
		ImGui.NewLine();
		DrawGeneralSettings(150f * Globals.GlobalUIScale);
	}

	private void ToggleDefaultPage()
	{
		if (((Window)base.Overlay).IsOpen || ((Window)fullWindow).IsOpen)
		{
			((Window)base.Overlay).IsOpen = false;
			((Window)fullWindow).IsOpen = false;
		}
		else if (config.DefaultPage == PageType.Search)
		{
			((Window)base.Overlay).IsOpen = true;
		}
		else
		{
			((Window)fullWindow).IsOpen = true;
		}
	}

	private FuzzyMatcher<AetheryteRecord> CreateRecordMatcher()
	{
		return new FuzzyMatcher<AetheryteRecord>(OmenServiceBase<AetheryteRecordManager>.Instance().AllRecords, delegate(AetheryteRecord x)
		{
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			List<(IEnumerable<string>, FuzzySearchWeight)> list = new List<(IEnumerable<string>, FuzzySearchWeight)>();
			if (config.Remarks.TryGetValue(x.ToString(), out string value) && !string.IsNullOrWhiteSpace(value))
			{
				list.Add((new global::_003C_003Ez__ReadOnlySingleElementList<string>(value), FuzzySearchWeight.Title));
			}
			list.Add((new global::_003C_003Ez__ReadOnlySingleElementList<string>(x.Name), FuzzySearchWeight.Name));
			string text = x.GetZone().ExtractPlaceName();
			list.Add((new global::_003C_003Ez__ReadOnlyArray<string>(new string[2] { x.RegionName, text }), FuzzySearchWeight.Default));
			return list;
		});
	}

	private void SetupMapRenderer(AetheryteRecord aetheryte, bool isPinned)
	{
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		mapRenderer.SetMap(aetheryte.MapID);
		mapRenderer.Zoomable = isPinned;
		mapRenderer.Pannable = isPinned;
		mapRenderer.EnableResizeGrip = isPinned;
		if (!isPinned)
		{
			mapRenderer.ResetView();
		}
		else if (mapRenderer.CustomViewportSize == Vector2.Zero)
		{
			mapRenderer.CustomViewportSize = new Vector2(450f * Globals.GlobalUIScale);
		}
		mapRenderer.ClearMarkers();
		Map map = aetheryte.GetMap();
		uint mapID = ((Map)(ref map)).RowId;
		foreach (AetheryteRecord record in OmenServiceBase<AetheryteRecordManager>.Instance().AllRecords.Where(delegate(AetheryteRecord x)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			Map map2 = x.GetMap();
			return ((Map)(ref map2)).RowId == mapID;
		}).ToList())
		{
			if (record.RowID == aetheryte.RowID)
			{
				continue;
			}
			Vector3 value;
			Vector3 position = (config.Positions.TryGetValue(record.ToString(), out value) ? value : record.Position);
			bool isAetheryte = record.IsAetheryte;
			ImGuiMapMarker marker = new ImGuiMapMarker
			{
				ID = record.ToString(),
				Position = position,
				IconID = (isAetheryte ? 60453u : 60430u),
				Name = record.Name,
				Color = 3439329279u,
				Size = Globals.ScaledVector2(28f),
				ShowLabel = false,
				TooltipText = record.Name,
				OnClick = delegate
				{
					if (isPinned)
					{
						HandleTeleport(record);
						pinnedAetheryte = null;
						config.Save(this);
					}
				}
			};
			mapRenderer.AddMarker(marker);
		}
		Vector3 value2;
		Vector3 position2 = (config.Positions.TryGetValue(aetheryte.ToString(), out value2) ? value2 : aetheryte.Position);
		string value3;
		string label = (config.Remarks.TryGetValue(aetheryte.ToString(), out value3) ? value3 : aetheryte.Name);
		ImGuiMapMarker marker2 = new ImGuiMapMarker
		{
			ID = aetheryte.ToString(),
			Position = position2,
			IconID = (aetheryte.IsAetheryte ? 60453u : 60430u),
			Name = aetheryte.Name,
			Color = uint.MaxValue,
			Size = Globals.ScaledVector2(32f),
			PulseEffect = true,
			PulseColor = ImGui.GetColorU32((ImGuiCol)18),
			Label = label,
			ShowLabel = true,
			LabelColor = KnownColor.Chocolate.ToVector4().ToUInt(),
			OnClick = delegate
			{
				if (isPinned)
				{
					HandleTeleport(aetheryte);
					pinnedAetheryte = null;
					config.Save(this);
				}
			}
		};
		mapRenderer.AddMarker(marker2);
		if (ImGui.IsPopupOpen(ImU8String.op_Implicit("BetterTeleport_Map_ContextMenu"), (ImGuiPopupFlags)0))
		{
			ImGuiMapMarker marker3 = new ImGuiMapMarker
			{
				ID = "ContextMenuTargetFlag",
				Position = contextMenuTargetPos,
				IconID = 60561u,
				Size = Globals.ScaledVector2(32f),
				ShowLabel = false,
				ShowTooltip = false
			};
			mapRenderer.AddMarker(marker3);
		}
		mapRenderer.OnMapClicked = delegate(ImGuiMapRenderer _, Vector3 clickedWorldPos, Vector2 _, ImGuiMouseButton button)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Invalid comparison between Unknown and I4
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			if (isPinned && (int)button == 1)
			{
				contextMenuTargetZone = aetheryte.ZoneID;
				contextMenuTargetPos = clickedWorldPos;
				ImGui.OpenPopup(ImU8String.op_Implicit("BetterTeleport_Map_ContextMenu"), (ImGuiPopupFlags)0);
			}
		};
	}

	private void EnsureAetheryteRegionIconsLoaded()
	{
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		if (aetheryteRegionIcons.Count > 0 && aetheryteTabIcons.Count > 0 && aetheryteSettingIcon.HasValue)
		{
			return;
		}
		UldFile file = DalamudServiceExtension.Instance<IDataManager>().GetFile<UldFile>("ui/uld/Teleport.uld");
		if (file == null)
		{
			return;
		}
		(string, Vector2, Vector2)[] array = LoadUldPartIcons(file, 7u);
		if (array != null && array.Length >= 1)
		{
			(string, Vector2, Vector2) value = array[0];
			if (value.Item1 != null)
			{
				aetheryteSettingIcon = value;
			}
		}
		if (aetheryteRegionIcons.Count > 0 && aetheryteTabIcons.Count > 0)
		{
			return;
		}
		(string, Vector2, Vector2)[] array2 = LoadUldPartIcons(file, 16u);
		if (array2 == null)
		{
			return;
		}
		Timeline val = file.Timelines.FirstOrDefault((Timeline x) => x.Id == 18);
		if (val.FrameData == null)
		{
			return;
		}
		for (int num = 0; num < val.FrameData.Length; num++)
		{
			IKeyframe[] frames = val.FrameData[num].KeyGroups.FirstOrDefault((KeyGroup x) => (int)x.Usage == 5).Frames;
			if (frames != null && frames.Length >= 1 && frames[0] is UShort1Keyframe val2 && val2.Value < array2.Length)
			{
				(string, Vector2, Vector2) value2 = array2[val2.Value];
				if (value2.Item1 != null)
				{
					aetheryteRegionIcons[num] = value2;
				}
			}
		}
		(string, Vector2, Vector2)[] array3 = LoadUldPartIcons(file, 14u);
		if (array3 == null)
		{
			return;
		}
		for (int num2 = 0; num2 < array3.Length; num2++)
		{
			(string, Vector2, Vector2) value3 = array3[num2];
			if (value3.Item1 != null)
			{
				aetheryteTabIcons[num2] = value3;
			}
		}
	}

	private static (string Path, Vector2 UV0, Vector2 UV1)[]? LoadUldPartIcons(UldFile uld, uint partListID)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		PartsData val = uld.Parts.FirstOrDefault((PartsData x) => x.Id == partListID);
		if (val.Parts == null)
		{
			return null;
		}
		(string, Vector2, Vector2)[] array = new(string, Vector2, Vector2)[val.Parts.Length];
		for (int num = 0; num < val.Parts.Length; num++)
		{
			PartData part = val.Parts[num];
			TextureEntry val2 = uld.AssetData.FirstOrDefault((TextureEntry x) => x.Id == part.TextureId);
			if (val2.Path == null)
			{
				continue;
			}
			string text = new string(val2.Path).TrimEnd('\0');
			if (!string.IsNullOrEmpty(text))
			{
				IDalamudTextureWrap wrapOrDefault = DalamudServiceExtension.Instance<ITextureProvider>().GetFromGame(text).GetWrapOrDefault((IDalamudTextureWrap)null);
				if (wrapOrDefault == null)
				{
					return null;
				}
				Vector2 vector = new Vector2(wrapOrDefault.Width, wrapOrDefault.Height);
				array[num] = (text, new Vector2((int)part.U, (int)part.V) / vector, new Vector2(part.U + part.W, part.V + part.H) / vector);
			}
		}
		return array;
	}

	private unsafe int GetAetheryteRegionTimelineID(AetheryteRecord aetheryte)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		TeleportInfo val = new TeleportInfo
		{
			TerritoryId = (ushort)aetheryte.ZoneID
		};
		AgentTeleport* agent = AgentTeleport.Instance();
		int region = AgentTeleportGetRegion(agent, &val);
		int num = AgentTeleportGetTimelineID(agent, region, (int)aetheryte.ZoneID);
		if (num != 4)
		{
			return num;
		}
		return -1;
	}

	private unsafe int GetAetheryteTabIconPartID(int category)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		AddonTeleport* ptr = (AddonTeleport*)((RaptureAtkUnitManager)RaptureAtkUnitManager.Instance()).GetAddonByName("Teleport", 1);
		AddonTeleport val = default(AddonTeleport);
		if (ptr == null)
		{
			ptr = &val;
		}
		int num = AddonTeleportGetRegionIconID(ptr, category);
		if (num < 115)
		{
			return num - 99;
		}
		return num - 100;
	}

	private static int GetAetheryteCategory(AetheryteRecord aetheryte)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		TerritoryType zone = aetheryte.GetZone();
		if (((TerritoryType)(ref zone)).TerritoryIntendedUse.RowId != 13)
		{
			Aetheryte data = aetheryte.GetData();
			return ((Aetheryte)(ref data)).Unknown2;
		}
		return 2;
	}

	private string GetAetheryteTabLabel(AetheryteRecord aetheryte, string tabName)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		if (aetheryte.Version != 0)
		{
			ExVersion? row = LuminaGetter.GetRow<ExVersion>(aetheryte.Version);
			object obj;
			if (!row.HasValue)
			{
				obj = null;
			}
			else
			{
				ExVersion valueOrDefault = row.GetValueOrDefault();
				obj = ((object)((ExVersion)(ref valueOrDefault)).Name/*cast due to constrained. prefix*/).ToString();
			}
			string text = (string)obj;
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			return tabName;
		}
		TerritoryType zone = aetheryte.GetZone();
		PlaceName value = ((TerritoryType)(ref zone)).PlaceNameRegion.Value;
		uint rowId = ((PlaceName)(ref value)).RowId;
		if (rowId - 22 > 2)
		{
			return tabName;
		}
		return ((object)((PlaceName)(ref value)).Name/*cast due to constrained. prefix*/).ToString();
	}

	private void DrawHoveredTooltip()
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		if (hoveredAetheryte != null && PluginConfig.Instance().ConflictKeyBinding.IsPressed())
		{
			pinnedAetheryte = hoveredAetheryte;
		}
		if (pinnedAetheryte != null)
		{
			ImGui.SetNextWindowBgAlpha(0.85f);
			if (ImGui.Begin(ImU8String.op_Implicit("###PinnedAetheryteMap"), (ImGuiWindowFlags)363))
			{
				string text = LanguageManager.Get("BetterTeleport-MapHint-Zoom");
				Vector2 vector = ImGui.CalcTextSize(ImU8String.op_Implicit(text), false, -1f);
				Vector2 drawSize = ((mapRenderer.CustomViewportSize.X > 50f) ? mapRenderer.CustomViewportSize : new Vector2(400f * Globals.GlobalUIScale, 400f * Globals.GlobalUIScale));
				ImGui.SetCursorPosX(ImGui.GetCursorPosX() + (drawSize.X - vector.X) / 2f);
				ImGui.TextDisabled(ImU8String.op_Implicit(text));
				SetupMapRenderer(pinnedAetheryte, isPinned: true);
				mapRenderer.Draw(drawSize);
				DrawMapContextMenu();
				if (!ImGui.IsWindowFocused() && !ImGui.IsPopupOpen(ImU8String.op_Implicit("BetterTeleport_Map_ContextMenu"), (ImGuiPopupFlags)0))
				{
					pinnedAetheryte = null;
					config.Save(this);
				}
				ImGui.End();
			}
		}
		if (hoveredAetheryte == null || ImGui.IsPopupOpen(ImU8String.op_Implicit("AetheryteContextPopup"), (ImGuiPopupFlags)0) || (pinnedAetheryte != null && hoveredAetheryte.RowID == pinnedAetheryte.RowID))
		{
			return;
		}
		StyleDisposable val = ImRaii.PushStyle((ImGuiStyleVar)1, Vector2.Zero, true);
		try
		{
			ColorDisposable val2 = ImRaii.PushColor((ImGuiCol)4, ImGui.GetColorU32((ImGuiCol)2), true);
			try
			{
				TooltipDisposable val3 = ImRaii.Tooltip();
				try
				{
					Vector2 drawSize2 = Globals.ScaledVector2(384f);
					ImGuiOm.ScaledDummy(0f, 2f);
					string text2 = LanguageManager.Get("BetterTeleport-MapHint-Pin");
					Vector2 vector2 = ImGui.CalcTextSize(ImU8String.op_Implicit(text2), false, -1f);
					ImGui.SetCursorPosX(ImGui.GetCursorPosX() + (drawSize2.X - vector2.X) / 2f);
					ImGui.TextDisabled(ImU8String.op_Implicit(text2));
					SetupMapRenderer(hoveredAetheryte, isPinned: false);
					mapRenderer.Draw(drawSize2);
				}
				finally
				{
					((TooltipDisposable)(ref val3)).Dispose();
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

	private void DrawMapContextMenu()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		PopupDisposable val = ImRaii.Popup(ImU8String.op_Implicit("BetterTeleport_Map_ContextMenu"));
		try
		{
			if (!val || !ImGui.MenuItem(ImU8String.op_Implicit(LanguageManager.Get("BetterTeleport-TeleportToThisPosition")), false, true))
			{
				return;
			}
			base.TaskHelper.Abort();
			if (GameState.TerritoryType != contextMenuTargetZone || IsWithPermission())
			{
				base.TaskHelper.Enqueue(() => OmenServiceBase<MovementManager>.Instance().TPSmart_BetweenZone(contextMenuTargetZone, contextMenuTargetPos));
				base.TaskHelper.Enqueue(delegate
				{
					if (OmenServiceBase<MovementManager>.Instance().IsManagerBusy || IOmenDalamudService<IObjectTable>.Instance().LocalPlayer == null)
					{
						return false;
					}
					OmenServiceBase<MovementManager>.Instance().TPGround();
					return (!IConditionExtension.get_IsBetweenAreas(DalamudServiceExtension.Instance<ICondition>()) && !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)48]) ? true : false;
				});
			}
			else
			{
				base.TaskHelper.Enqueue(() => OmenServiceBase<AetheryteRecordManager>.Instance().GetNearestAetheryte(contextMenuTargetZone, contextMenuTargetPos)?.TeleportTo());
				base.TaskHelper.Enqueue(() => IConditionExtension.get_IsBetweenAreas(DalamudServiceExtension.Instance<ICondition>()) && IOmenDalamudService<IObjectTable>.Instance().LocalPlayer != null);
				base.TaskHelper.Enqueue(delegate
				{
					if (!IConditionExtension.get_IsBetweenAreas(DalamudServiceExtension.Instance<ICondition>()))
					{
						return true;
					}
					OmenServiceBase<MovementManager>.Instance().TPSmart_InZone(contextMenuTargetPos, new TPSmartParams
					{
						CancelAnimation = false
					});
					return false;
				});
			}
			ImGui.CloseCurrentPopup();
		}
		finally
		{
			((PopupDisposable)(ref val)).Dispose();
		}
	}

	private unsafe static void DrawBottomToolbar()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
		Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
		float num = 24f * Globals.GlobalUIScale;
		long number = 0L;
		int number2 = 0;
		InventoryManager* ptr = InventoryManager.Instance();
		if (ptr != null)
		{
			number = ((InventoryManager)ptr).GetGil();
			number2 = ((InventoryManager)ptr).GetInventoryItemCount(7569u, false, true, true, (short)0);
		}
		float num2 = cursorScreenPos.X + 2f * Globals.GlobalUIScale;
		float num3 = cursorScreenPos.Y + 2f * Globals.GlobalUIScale;
		uint colorU = ImGui.GetColorU32((ImGuiCol)7);
		uint colorU2 = ImGui.GetColorU32((ImGuiCol)5);
		float num4 = 8f * Globals.GlobalUIScale;
		float num5 = 20f * Globals.GlobalUIScale;
		string text = number.ToChineseString();
		string itemName = LuminaWrapper.GetItemName(1u);
		float x = ImGui.CalcTextSize(ImU8String.op_Implicit(text), false, -1f).X;
		float x2;
		using (OmenServiceBase<FontManager>.Instance().UIFont60.Push())
		{
			x2 = ImGui.CalcTextSize(ImU8String.op_Implicit(itemName), false, -1f).X;
		}
		ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
		GameIconLookup val = GameIconLookup.op_Implicit(LuminaWrapper.GetItemIconID(1u));
		IDalamudTextureWrap wrapOrEmpty = obj.GetFromGameIcon(ref val).GetWrapOrEmpty();
		float num6 = num4 * 2f + 20f * Globals.GlobalUIScale + x + 4f * Globals.GlobalUIScale + x2;
		Vector2 vector = new Vector2(num2, num3);
		((ImDrawListPtr)(ref windowDrawList)).AddRectFilled(vector, vector + new Vector2(num6, num5), colorU, 4f * Globals.GlobalUIScale);
		((ImDrawListPtr)(ref windowDrawList)).AddRect(vector, vector + new Vector2(num6, num5), colorU2, 4f * Globals.GlobalUIScale);
		float num7 = num2 + num4;
		float num8 = 14f * Globals.GlobalUIScale;
		float num9 = (num5 - num8) / 2f;
		float num10 = (num5 - ImGui.GetTextLineHeight()) / 2f;
		((ImDrawListPtr)(ref windowDrawList)).AddImage(wrapOrEmpty.Handle, new Vector2(num7, num3 + num9), new Vector2(num7 + num8, num3 + num9 + num8));
		num7 += num8 + 4f * Globals.GlobalUIScale;
		((ImDrawListPtr)(ref windowDrawList)).AddText(new Vector2(num7, num3 + num10), ImGui.GetColorU32((ImGuiCol)0), ImU8String.op_Implicit(text));
		num7 += x + 4f * Globals.GlobalUIScale;
		using (OmenServiceBase<FontManager>.Instance().UIFont60.Push())
		{
			float textLineHeight = ImGui.GetTextLineHeight();
			((ImDrawListPtr)(ref windowDrawList)).AddText(new Vector2(num7, num3 + (num5 - textLineHeight) / 2f + 1f * Globals.GlobalUIScale), ImGui.GetColorU32((ImGuiCol)1), ImU8String.op_Implicit(itemName));
		}
		num2 += num6 + 8f * Globals.GlobalUIScale;
		string text2 = number2.ToChineseString();
		string itemName2 = LuminaWrapper.GetItemName(7569u);
		float x3 = ImGui.CalcTextSize(ImU8String.op_Implicit(text2), false, -1f).X;
		float x4;
		using (OmenServiceBase<FontManager>.Instance().UIFont60.Push())
		{
			x4 = ImGui.CalcTextSize(ImU8String.op_Implicit(itemName2), false, -1f).X;
		}
		ITextureProvider obj2 = DalamudServiceExtension.Instance<ITextureProvider>();
		val = GameIconLookup.op_Implicit(LuminaWrapper.GetItemIconID(7569u));
		IDalamudTextureWrap wrapOrEmpty2 = obj2.GetFromGameIcon(ref val).GetWrapOrEmpty();
		float x5 = num4 * 2f + 20f * Globals.GlobalUIScale + x3 + 4f * Globals.GlobalUIScale + x4;
		Vector2 vector2 = new Vector2(num2, num3);
		((ImDrawListPtr)(ref windowDrawList)).AddRectFilled(vector2, vector2 + new Vector2(x5, num5), colorU, 4f * Globals.GlobalUIScale);
		((ImDrawListPtr)(ref windowDrawList)).AddRect(vector2, vector2 + new Vector2(x5, num5), colorU2, 4f * Globals.GlobalUIScale);
		float num11 = num2 + num4;
		((ImDrawListPtr)(ref windowDrawList)).AddImage(wrapOrEmpty2.Handle, new Vector2(num11, num3 + num9), new Vector2(num11 + num8, num3 + num9 + num8));
		num11 += num8 + 4f * Globals.GlobalUIScale;
		((ImDrawListPtr)(ref windowDrawList)).AddText(new Vector2(num11, num3 + num10), ImGui.GetColorU32((ImGuiCol)0), ImU8String.op_Implicit(text2));
		num11 += x3 + 4f * Globals.GlobalUIScale;
		using (OmenServiceBase<FontManager>.Instance().UIFont60.Push())
		{
			float textLineHeight2 = ImGui.GetTextLineHeight();
			((ImDrawListPtr)(ref windowDrawList)).AddText(new Vector2(num11, num3 + (num5 - textLineHeight2) / 2f + 1f * Globals.GlobalUIScale), ImGui.GetColorU32((ImGuiCol)1), ImU8String.op_Implicit(itemName2));
		}
		ImGui.SetCursorScreenPos(cursorScreenPos + new Vector2(0f, num + 4f * Globals.GlobalUIScale));
	}

	private float DrawAetheryteIcon(ImDrawListPtr drawList, AetheryteRecord aetheryte, Vector2 startPos, float itemHeight, float padding, float animOffset)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		GeneralAction? row = LuminaGetter.GetRow<GeneralAction>(7u);
		int num;
		if (!row.HasValue)
		{
			num = 60752;
		}
		else
		{
			GeneralAction valueOrDefault = row.GetValueOrDefault();
			num = ((GeneralAction)(ref valueOrDefault)).Icon;
		}
		uint num2 = (uint)num;
		num2 = (aetheryte.IsHouse ? 60752u : ((!aetheryte.IsAetheryte) ? 60430u : 60453u));
		ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
		GameIconLookup val = new GameIconLookup(num2, false, true, (ClientLanguage?)null);
		IDalamudTextureWrap wrapOrEmpty = obj.GetFromGameIcon(ref val).GetWrapOrEmpty();
		float num3 = startPos.X + padding + 6f + animOffset;
		float num4 = 24f * Globals.GlobalUIScale;
		float num5 = startPos.Y + (itemHeight - num4) / 2f;
		if (wrapOrEmpty.Handle != (IntPtr)IntPtr.Zero)
		{
			((ImDrawListPtr)(ref drawList)).AddImage(wrapOrEmpty.Handle, new Vector2(num3, num5), new Vector2(num3 + num4, num5 + num4));
		}
		num3 += num4 + 4f * Globals.GlobalUIScale;
		float num6 = 20f * Globals.GlobalUIScale;
		float y = startPos.Y + (itemHeight - num6) / 2f;
		if (DrawAetheryteRegionIcon(drawList, aetheryte, new Vector2(num3, y), num6))
		{
			num3 += num6 + 4f * Globals.GlobalUIScale;
		}
		return num3 + padding + 4f;
	}

	private bool DrawAetheryteRegionIcon(ImDrawListPtr drawList, AetheryteRecord aetheryte, Vector2 position, float size)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		EnsureAetheryteRegionIconsLoaded();
		if (aetheryteRegionIcons.TryGetValue(GetAetheryteRegionTimelineID(aetheryte), out (string, Vector2, Vector2) value))
		{
			return DrawAetheryteIconPart(drawList, value, position, size);
		}
		return false;
	}

	private bool DrawAetheryteTabRegionIcon(ImDrawListPtr drawList, int category, Vector2 position, float size)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		EnsureAetheryteRegionIconsLoaded();
		if (aetheryteTabIcons.TryGetValue(GetAetheryteTabIconPartID(category), out (string, Vector2, Vector2) value))
		{
			return DrawAetheryteIconPart(drawList, value, position, size);
		}
		return false;
	}

	private bool DrawAetheryteSettingIcon(ImDrawListPtr drawList, Vector2 position, float size)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		EnsureAetheryteRegionIconsLoaded();
		(string, Vector2, Vector2)? tuple = aetheryteSettingIcon;
		if (tuple.HasValue)
		{
			(string, Vector2, Vector2) valueOrDefault = tuple.GetValueOrDefault();
			return DrawAetheryteIconPart(drawList, valueOrDefault, position, size);
		}
		return false;
	}

	private static bool DrawAetheryteIconPart(ImDrawListPtr drawList, (string Path, Vector2 UV0, Vector2 UV1) part, Vector2 position, float size)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		IDalamudTextureWrap wrapOrDefault = DalamudServiceExtension.Instance<ITextureProvider>().GetFromGame(part.Path).GetWrapOrDefault((IDalamudTextureWrap)null);
		if (wrapOrDefault == null)
		{
			return false;
		}
		((ImDrawListPtr)(ref drawList)).AddImage(wrapOrDefault.Handle, position, position + new Vector2(size), part.UV0, part.UV1);
		return true;
	}

	private float DrawAetheryteIndicators(ImDrawListPtr drawList, AetheryteRecord aetheryte, float startX, float titleY)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		float num = startX;
		if (config.Remarks.ContainsKey(aetheryte.ToString()))
		{
			string text = FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61508);
			Vector2 vector = ImGui.CalcTextSize(ImU8String.op_Implicit(text), false, -1f);
			((ImDrawListPtr)(ref drawList)).AddText(new Vector2(num, titleY), ImGui.GetColorU32((ImGuiCol)1), ImU8String.op_Implicit(text));
			num += vector.X + 6f * Globals.GlobalUIScale;
		}
		if (config.Positions.ContainsKey(aetheryte.ToString()))
		{
			string text2 = FontAwesomeExtensions.ToIconString((FontAwesomeIcon)62070);
			Vector2 vector2 = ImGui.CalcTextSize(ImU8String.op_Implicit(text2), false, -1f);
			((ImDrawListPtr)(ref drawList)).AddText(new Vector2(num, titleY), ImGui.GetColorU32(KnownColor.Orange.ToVector4().ToUInt()), ImU8String.op_Implicit(text2));
			num += vector2.X + 6f * Globals.GlobalUIScale;
		}
		return num;
	}

	private void DrawAetheryteStateIcon(AetheryteRecord aetheryte, float curX, float titleY)
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		SeString val = null;
		switch (aetheryte.State)
		{
		case AetheryteRecordState.Home:
			val = HomeChar;
			break;
		case AetheryteRecordState.Free:
			val = FreeChar;
			break;
		case AetheryteRecordState.Favorite:
			val = FavoriteChar;
			break;
		default:
			if (config.Favorites.Contains(aetheryte.ToString()))
			{
				val = FavoriteChar;
			}
			break;
		}
		if (val != null)
		{
			ImGui.SetCursorScreenPos(new Vector2(curX, titleY));
			ReadOnlySpan<byte> readOnlySpan = val.Encode();
			SeStringDrawParams val2 = default(SeStringDrawParams);
			ImGuiHelpers.SeStringWrapped(readOnlySpan, ref val2, default(ImGuiId), (ImGuiButtonFlags)1);
		}
	}

	private unsafe void DrawAetheryteContextMenu(AetheryteRecord aetheryte)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		PopupDisposable val = ImRaii.ContextPopupItem(ImU8String.op_Implicit("AetheryteContextPopup"));
		try
		{
			if (!val)
			{
				return;
			}
			ImU8String val2 = new ImU8String(0, 1);
			((ImU8String)(ref val2)).AppendFormatted<string>(aetheryte.Name);
			ImGui.TextUnformatted(val2);
			ImGui.Separator();
			ImGui.Spacing();
			if (ImGui.MenuItem(ImU8String.op_Implicit(LanguageManager.Get("Favorite")), ImU8String.op_Implicit(string.Empty), config.Favorites.Contains(aetheryte.ToString()), true))
			{
				if (!config.Favorites.Add(aetheryte.ToString()))
				{
					config.Favorites.Remove(aetheryte.ToString());
				}
				RefreshFavoritesInfo();
				config.Save(this);
				RefreshDefaultOverlayItems();
			}
			ImGui.Separator();
			ImGui.Spacing();
			ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("Note")));
			string value2;
			string value = (config.Remarks.TryGetValue(aetheryte.ToString(), out value2) ? value2 : string.Empty);
			ImGui.SetNextItemWidth(Math.Max(150f * Globals.GlobalUIScale, ImGui.CalcTextSize(ImU8String.op_Implicit(aetheryte.Name), false, -1f).X));
			if (ImGui.InputText(ImU8String.op_Implicit("###Note"), ref value, 128, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null))
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					config.Remarks.Remove(aetheryte.ToString());
				}
				else
				{
					config.Remarks[aetheryte.ToString()] = value;
				}
			}
			if (ImGui.IsItemDeactivatedAfterEdit())
			{
				config.Save(this);
				recordMatcher?.Dispose();
				recordMatcher = CreateRecordMatcher();
			}
			ImGui.Separator();
			ImGui.Spacing();
			ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("Position")));
			Vector3 value3;
			bool flag = config.Positions.TryGetValue(aetheryte.ToString(), out value3);
			using (OmenServiceBase<FontManager>.Instance().UIFont60.Push())
			{
				ImU8String val3 = new ImU8String(0, 1);
				((ImU8String)(ref val3)).AppendFormatted<Vector3>(flag ? value3 : aetheryte.Position, "F1");
				ImGui.TextUnformatted(val3);
			}
			if (ImGui.MenuItem(ImU8String.op_Implicit(LanguageManager.Get("BetterTeleport-RedirectedToCurrentPos")), false, true))
			{
				config.Positions[aetheryte.ToString()] = Vector3.op_Implicit(((BattleChara)Control.GetLocalPlayer()).Position);
				config.Save(this);
			}
			DisabledDisposable val4 = ImRaii.Disabled(!config.Positions.ContainsKey(aetheryte.ToString()));
			try
			{
				ImU8String val5 = new ImU8String(19, 1);
				((ImU8String)(ref val5)).AppendFormatted<string>(LanguageManager.Get("Clear"));
				((ImU8String)(ref val5)).AppendLiteral("###DeleteRedirected");
				if (ImGui.MenuItem(val5, false, true))
				{
					config.Positions.Remove(aetheryte.ToString());
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
			((PopupDisposable)(ref val)).Dispose();
		}
	}

	private void DrawAetheryteItem(AetheryteRecord aetheryte, int? index = null, bool isSelected = false, bool drawHotkeyBadge = true, string? searchText = null)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0560: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0470: Unknown result type (might be due to invalid IL or missing references)
		//IL_0526: Unknown result type (might be due to invalid IL or missing references)
		if (config.HideAethernetInParty && !aetheryte.IsAetheryte && DalamudServiceExtension.Instance<IPartyList>().Length > 1)
		{
			return;
		}
		string value;
		string text = (config.Remarks.TryGetValue(aetheryte.ToString(), out value) ? value : aetheryte.Name);
		uint cost = aetheryte.Cost;
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(0, 1);
		((ImU8String)(ref val)).AppendFormatted<AetheryteRecord>(aetheryte);
		IdDisposable val2 = ImRaii.PushId(val, true);
		try
		{
			Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
			float x = ImGui.GetContentRegionAvail().X;
			float textLineHeight = ImGui.GetTextLineHeight();
			ImGuiStylePtr style = ImGui.GetStyle();
			float x2 = ((ImGuiStylePtr)(ref style)).ItemSpacing.X;
			float num = textLineHeight * 2.2f + x2;
			if (ImGui.InvisibleButton(ImU8String.op_Implicit("##ItemBtn"), new Vector2(x, num), (ImGuiButtonFlags)0))
			{
				if (index.HasValue)
				{
					hasUsedArrowKeys = false;
				}
				HandleTeleport(aetheryte, searchText);
			}
			bool flag = ImGui.IsItemHovered() && !hasUsedArrowKeys;
			bool flag2 = ImGui.IsItemActive();
			DrawAetheryteContextMenu(aetheryte);
			(uint, byte) key = (aetheryte.RowID, aetheryte.SubIndex);
			hoverProgress.TryAdd(key, 0f);
			float num2 = ((flag || isSelected) ? 1f : 0f);
			ImGuiIOPtr iO = ImGui.GetIO();
			float val3 = ((ImGuiIOPtr)(ref iO)).DeltaTime * 12f;
			float num3 = hoverProgress[key];
			if (Math.Abs(num3 - num2) > 0.001f)
			{
				num3 += (num2 - num3) * Math.Min(val3, 1f);
				num3 = Math.Clamp(num3, 0f, 1f);
				hoverProgress[key] = num3;
			}
			float num4 = num3 * 8f;
			float num5 = num * 0.7f * num3;
			ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
			uint colorU = ImGui.GetColorU32((ImGuiCol)8);
			uint colorU2 = ImGui.GetColorU32((ImGuiCol)9);
			uint num6 = 0u;
			if (flag2)
			{
				num6 = colorU2;
			}
			else if (isSelected)
			{
				num6 = ImGui.GetColorU32((ImGuiCol)26);
			}
			else if (num3 > 0.01f)
			{
				uint num7 = (uint)(num3 * (float)((colorU >> 24) & 0xFF));
				num6 = (colorU & 0xFFFFFF) | (num7 << 24);
			}
			if (num6 != 0)
			{
				((ImDrawListPtr)(ref windowDrawList)).AddRectFilledMultiColor(cursorScreenPos, cursorScreenPos + new Vector2(x, num), num6, (num6 & 0xFFFFFF) | (((uint)((float)(num6 >> 24) * 0.5f) & 0xFF) << 24), (num6 & 0xFFFFFF) | (((uint)((float)(num6 >> 24) * 0.5f) & 0xFF) << 24), num6);
			}
			if (num3 > 0.01f)
			{
				uint num8 = ImGui.GetColorU32((ImGuiCol)18);
				switch (aetheryte.State)
				{
				case AetheryteRecordState.Home:
					num8 = 4278232575u;
					break;
				case AetheryteRecordState.Favorite:
					num8 = 4278245375u;
					break;
				}
				float num9 = cursorScreenPos.Y + num / 2f;
				Vector2 vector = cursorScreenPos;
				vector.Y = num9 - num5 / 2f;
				((ImDrawListPtr)(ref windowDrawList)).AddRectFilled(vector, new Vector2(cursorScreenPos.X + 3f, num9 + num5 / 2f), num8, 1.5f);
			}
			float num10 = DrawAetheryteIcon(windowDrawList, aetheryte, cursorScreenPos, num, x2, num4);
			float num11 = cursorScreenPos.Y + 4f;
			((ImDrawListPtr)(ref windowDrawList)).AddText(new Vector2(num10, num11), ImGui.GetColorU32((ImGuiCol)0), ImU8String.op_Implicit(text));
			float x3 = ImGui.CalcTextSize(ImU8String.op_Implicit(text), false, -1f).X;
			float startX = num10 + x3 + 8f * Globals.GlobalUIScale;
			startX = DrawAetheryteIndicators(windowDrawList, aetheryte, startX, num11);
			DrawAetheryteStateIcon(aetheryte, startX, num11);
			using (OmenServiceBase<FontManager>.Instance().UIFont80.Push())
			{
				float y = num11 + textLineHeight + 2f * Globals.GlobalUIScale;
				((ImDrawListPtr)(ref windowDrawList)).AddText(new Vector2(num10, y), ImGui.GetColorU32((ImGuiCol)1), ImU8String.op_Implicit(aetheryte.GetZone().ExtractPlaceName()));
			}
			float num12 = cursorScreenPos.X + x - x2 - 6f - num4;
			if (index.HasValue && drawHotkeyBadge)
			{
				string text2 = null;
				int value2 = index.Value;
				if (value2 >= 0 && value2 < 8)
				{
					text2 = $"Ctrl+{index.Value + 1}";
				}
				else if (index.Value == 8)
				{
					text2 = "Ctrl+9";
				}
				if (text2 != null)
				{
					Vector2 vector2 = ImGui.CalcTextSize(ImU8String.op_Implicit(text2), false, -1f);
					float num13 = vector2.X + 8f;
					float num14 = vector2.Y + 4f;
					Vector2 vector3 = new Vector2(num12 - num13, cursorScreenPos.Y + (num - num14) / 2f);
					uint colorU3 = ImGui.GetColorU32((ImGuiCol)7);
					((ImDrawListPtr)(ref windowDrawList)).AddRectFilled(vector3, vector3 + new Vector2(num13, num14), colorU3, 4f);
					((ImDrawListPtr)(ref windowDrawList)).AddRect(vector3, vector3 + new Vector2(num13, num14), ImGui.GetColorU32((ImGuiCol)5), 4f);
					((ImDrawListPtr)(ref windowDrawList)).AddText(vector3 + new Vector2(4f, 2f), ImGui.GetColorU32((ImGuiCol)1), ImU8String.op_Implicit(text2));
					num12 -= num13 + x2;
				}
			}
			string text3 = $"{cost}" + "\ue049";
			Vector2 vector4 = ImGui.CalcTextSize(ImU8String.op_Implicit(text3), false, -1f);
			Vector2 vector5 = new Vector2(num12 - vector4.X, cursorScreenPos.Y + (num - vector4.Y) / 2f);
			((ImDrawListPtr)(ref windowDrawList)).AddText(vector5, ImGui.GetColorU32((ImGuiCol)0), ImU8String.op_Implicit(text3));
			if (flag && !isSearchingInputting)
			{
				if (index.HasValue)
				{
					selectedIndex = index.Value;
				}
				if (!ImGui.IsPopupOpen(ImU8String.op_Implicit("AetheryteContextPopup"), (ImGuiPopupFlags)0))
				{
					hoveredAetheryte = aetheryte;
					if (!index.HasValue)
					{
						isNeedToLoseFocusSearchBar = true;
					}
				}
			}
			ImGui.SetCursorScreenPos(cursorScreenPos + new Vector2(0f, num + (index.HasValue ? 2f : 3f)));
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	private void DrawGeneralSettings(float width = -1f)
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		int defaultPage = (int)config.DefaultPage;
		string[] array = new string[2]
		{
			LanguageManager.Get("BetterTeleport-PageSearch"),
			LanguageManager.Get("BetterTeleport-PageFull")
		};
		if (width > 0f)
		{
			ImGui.SetNextItemWidth(width);
		}
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(33, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("BetterTeleport-DefaultPage"));
		((ImU8String)(ref val)).AppendLiteral("###BetterTeleportDefaultPageCombo");
		if (ImGui.Combo(val, ref defaultPage, (ReadOnlySpan<string>)array, array.Length))
		{
			config.DefaultPage = (PageType)defaultPage;
			config.Save(this);
		}
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("BetterTeleport-HideAethernetInParty")), ref config.HideAethernetInParty))
		{
			config.Save(this);
		}
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("BetterTeleport-FocusSearchOnOpen")), ref config.FocusSearchOnOpen))
		{
			config.Save(this);
		}
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("BetterTeleport-CloseOnLoseFocus")), ref config.CloseOnLoseFocus))
		{
			config.Save(this);
		}
	}
}
