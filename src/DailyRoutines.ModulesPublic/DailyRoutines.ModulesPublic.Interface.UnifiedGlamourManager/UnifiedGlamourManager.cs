using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using DailyRoutines.Common.Interface.Windows;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.Command;
using Dalamud.Interface;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Dalamud;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;

namespace DailyRoutines.ModulesPublic.Interface.UnifiedGlamourManager;

public class UnifiedGlamourManager : ModuleBase
{
	private sealed class Config : ModuleConfig
	{
		public bool UseGridView = true;

		public List<SavedItem> Favorites = new List<SavedItem>();

		public List<PlatePreset> Presets = new List<PlatePreset>();

		public bool OnlyCurrentRace;

		public bool OnlyCurrentSex;
	}

	private enum SourceFilter
	{
		All,
		PrismBox,
		Cabinet,
		Favorite
	}

	private enum SortMode
	{
		FavoriteThenNameAsc,
		NameAsc,
		NameDesc,
		LevelAsc,
		LevelDesc
	}

	private enum SetRelationFilter
	{
		All,
		SetRelatedOnly,
		NonSetOnly
	}

	private enum PresetSource
	{
		Self,
		OtherPlayer
	}

	private sealed class UnifiedItem : IEquatable<UnifiedItem>
	{
		public uint ItemID { get; set; }

		public uint RawItemID { get; set; }

		public uint PrismBoxIndex { get; set; }

		public uint CabinetID { get; set; }

		public string Name { get; set; } = string.Empty;

		public uint Stain0ID { get; set; }

		public uint Stain1ID { get; set; }

		public uint EquipSlotCategoryRowID { get; set; }

		public uint ClassJobCategoryRowID { get; set; }

		public uint IconID { get; set; }

		public uint LevelEquip { get; set; }

		public bool InPrismBox { get; set; }

		public bool InCabinet { get; set; }

		public bool IsSetContainer { get; set; }

		public bool IsSetPart { get; set; }

		public uint ParentSetItemID { get; set; }

		public string ParentSetName { get; set; } = string.Empty;

		public string SetPartLabel { get; set; } = string.Empty;

		public bool CanUseInPlate
		{
			get
			{
				if (InPrismBox || InCabinet)
				{
					return !IsSetContainer;
				}
				return false;
			}
		}

		public Stain? Stain0
		{
			get
			{
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				if (!LuminaGetter.TryGetRow<Stain>(Stain0ID, out Stain item))
				{
					return null;
				}
				return item;
			}
		}

		public Stain? Stain1
		{
			get
			{
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				if (!LuminaGetter.TryGetRow<Stain>(Stain1ID, out Stain item))
				{
					return null;
				}
				return item;
			}
		}

		public string SourceLabel
		{
			get
			{
				List<string> list = new List<string>();
				if (InPrismBox)
				{
					list.Add(LuminaWrapper.GetAddonText(11910u));
				}
				if (InCabinet)
				{
					list.Add(LuminaWrapper.GetAddonText(12216u));
				}
				if (IsSetPart || IsSetContainer)
				{
					list.Add(LuminaWrapper.GetAddonText(15624u));
				}
				if (list.Count != 0)
				{
					return string.Join(" / ", list);
				}
				return LanguageManager.Get("Unknown");
			}
		}

		public string AvailableSlotsLabel
		{
			get
			{
				if (!LuminaGetter.TryGetRow<EquipSlotCategory>(EquipSlotCategoryRowID, out EquipSlotCategory category))
				{
					return LanguageManager.Get("Unknown");
				}
				string[] array = (from x in PlateSlotDefinitions
					where x.CanUse(category)
					select LuminaWrapper.GetAddonText(x.AddonTextID) into x
					where !string.IsNullOrWhiteSpace(x)
					select x).Distinct().ToArray();
				if (array.Length == 0)
				{
					return LanguageManager.Get("Unknown");
				}
				return string.Join(" / ", array);
			}
		}

		public bool IsCompatibleWithSlot(uint slotIndex)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			if (!LuminaGetter.TryGetRow<EquipSlotCategory>(EquipSlotCategoryRowID, out EquipSlotCategory item))
			{
				return false;
			}
			if (slotIndex < PlateSlotDefinitions.Length)
			{
				return IsEquipSlotCategoryCompatibleWithPlateSlot(item, slotIndex);
			}
			return false;
		}

		public bool IsCompatibleWithJobs(uint[] classJobIDs)
		{
			if (classJobIDs.Length != 0)
			{
				return classJobIDs.Any((uint id) => LuminaSheetExtension.IsClassJobInCategory(id, ClassJobCategoryRowID));
			}
			return true;
		}

		public void MergeWith(UnifiedItem other)
		{
			if ((object)other != null)
			{
				InPrismBox |= other.InPrismBox;
				InCabinet |= other.InCabinet;
				if (other.InPrismBox)
				{
					RawItemID = other.RawItemID;
					PrismBoxIndex = other.PrismBoxIndex;
					Stain0ID = other.Stain0ID;
					Stain1ID = other.Stain1ID;
					IconID = other.IconID;
					EquipSlotCategoryRowID = other.EquipSlotCategoryRowID;
					ClassJobCategoryRowID = other.ClassJobCategoryRowID;
					LevelEquip = other.LevelEquip;
					IsSetContainer = other.IsSetContainer;
				}
				if (other.InCabinet)
				{
					CabinetID = other.CabinetID;
					IconID = ((IconID == 0) ? other.IconID : IconID);
					EquipSlotCategoryRowID = ((EquipSlotCategoryRowID == 0) ? other.EquipSlotCategoryRowID : EquipSlotCategoryRowID);
					ClassJobCategoryRowID = ((ClassJobCategoryRowID == 0) ? other.ClassJobCategoryRowID : ClassJobCategoryRowID);
					LevelEquip = ((LevelEquip == 0) ? other.LevelEquip : LevelEquip);
					IsSetContainer |= other.IsSetContainer;
				}
			}
		}

		public static UnifiedItem Create(uint itemID, uint rawItemID, string name, Item row, ItemSource source, uint prismBoxIndex, uint cabinetID, uint stain0ID, uint stain1ID, bool isSetContainer, bool isSetPart = false, uint parentSetItemID = 0u, string parentSetName = "", string setPartLabel = "")
		{
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Invalid comparison between Unknown and I4
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Invalid comparison between Unknown and I4
			return new UnifiedItem
			{
				ItemID = itemID,
				RawItemID = rawItemID,
				PrismBoxIndex = prismBoxIndex,
				CabinetID = cabinetID,
				Name = name,
				Stain0ID = stain0ID,
				Stain1ID = stain1ID,
				EquipSlotCategoryRowID = ((Item)(ref row)).EquipSlotCategory.RowId,
				ClassJobCategoryRowID = ((Item)(ref row)).ClassJobCategory.RowId,
				IconID = ((Item)(ref row)).Icon,
				LevelEquip = ((Item)(ref row)).LevelEquip,
				InPrismBox = ((int)source == 1),
				InCabinet = ((int)source == 2),
				IsSetContainer = isSetContainer,
				IsSetPart = isSetPart,
				ParentSetItemID = parentSetItemID,
				ParentSetName = parentSetName,
				SetPartLabel = setPartLabel
			};
		}

		public bool Equals(UnifiedItem? other)
		{
			if ((object)other == null)
			{
				return false;
			}
			if ((object)this == other)
			{
				return true;
			}
			if (ItemID == other.ItemID && PrismBoxIndex == other.PrismBoxIndex && IsSetPart == other.IsSetPart)
			{
				return ParentSetItemID == other.ParentSetItemID;
			}
			return false;
		}

		public override bool Equals(object? obj)
		{
			return Equals(obj as UnifiedItem);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(ItemID, PrismBoxIndex, IsSetPart, ParentSetItemID);
		}

		public static bool operator ==(UnifiedItem? left, UnifiedItem? right)
		{
			return left?.Equals(right) ?? ((object)right == null);
		}

		public static bool operator !=(UnifiedItem? left, UnifiedItem? right)
		{
			return !(left == right);
		}
	}

	private sealed class SavedItem
	{
		public uint ItemID { get; set; }

		public string Name { get; set; } = string.Empty;

		public long AddedAt { get; set; }
	}

	private sealed class PlatePreset
	{
		public string Title = string.Empty;

		public string Note = string.Empty;

		public string Race = string.Empty;

		public string Sex = string.Empty;

		public DateTime CreatedAt = DateTime.Now;

		public List<PlateItemInfo> Items = new List<PlateItemInfo>();
	}

	private readonly record struct SetPartInfo(uint ItemID, string PartLabel, int SlotIndex);

	private readonly record struct PlateSlotDefinition(uint AddonTextID, Func<EquipSlotCategory, bool> CanUse);

	private readonly record struct PlateItemInfo(uint SlotIndex, uint ItemID, byte Stain0ID, byte Stain1ID);

	private readonly record struct PrismBoxItemInfo(uint Index, uint RawItemID, uint ItemID, uint Stain0ID, uint Stain1ID, bool IsGlamorous, ulong ModelMain, uint EquipSlotCategoryRowID)
	{
		public bool IsGlamourModelValid
		{
			get
			{
				if (IsGlamorous && ModelMain != 0L)
				{
					return EquipSlotCategoryRowID != 0;
				}
				return false;
			}
		}

		public bool IsStained
		{
			get
			{
				if (Stain0ID == 0)
				{
					return Stain1ID != 0;
				}
				return true;
			}
		}
	}

	private TextButtonNode? inspectSaveButtonNode;

	private TextButtonNode? plateSaveButtonNode;

	private readonly List<PlateItemInfo> missingApplyItems = new List<PlateItemInfo>();

	private bool openMissingApplyItemsPopup;

	private bool missingApplyItemsPopupOpen;

	private Config config;

	private const string PLATE_EDITOR_ADDON_NAME = "MiragePrismMiragePlate";

	private const byte FEMALE_SEX = 1;

	private const string COMMAND = "ugm";

	private const string STONE_SCRIPT_URL = "hxxps://greasyfork.org/en/scripts/580109-%E7%9F%B3%E4%B9%8B%E5%AE%B6%E5%B9%BB%E5%8C%96%E6%A8%A1%E6%9D%BF%E5%AF%BC%E5%85%A5dr";

	private const string STONE_URL = "hxxps://ff14risingstones.web.sdo.com/pc/index.html#/glamour";

	private const string FAVORITE_ICON_ON = "★";

	private const string FAVORITE_ICON_OFF = "☆";

	private const int TASK_TIMEOUT_MS = 30000;

	private const int REFRESH_STEP_DELAY_MS = 1;

	private const int APPLY_RETRY_DELAY_MS = 50;

	private const int DEFAULT_MIN_EQUIP_LEVEL = 1;

	private const int DEFAULT_MAX_EQUIP_LEVEL = 100;

	private const int MAX_EQUIP_LEVEL_INPUT = 999;

	private const int VIRTUALIZED_LIST_BUFFER_ROWS = 3;

	private const int VIRTUALIZED_GRID_BUFFER_ROWS = 2;

	private const uint MIRAGE_PLATE_OPEN_MODE_AGENT_SHOW = 0u;

	private const uint PRISM_BOX_CAPACITY = 800u;

	private static readonly SourceFilter[] SourceFilters = new SourceFilter[4]
	{
		SourceFilter.All,
		SourceFilter.Favorite,
		SourceFilter.PrismBox,
		SourceFilter.Cabinet
	};

	private static readonly uint[][] JobFilterClassJobIDs = new uint[25][]
	{
		Array.Empty<uint>(),
		new uint[2] { 1u, 19u },
		new uint[2] { 3u, 21u },
		new uint[1] { 32u },
		new uint[1] { 37u },
		new uint[2] { 6u, 24u },
		new uint[1] { 28u },
		new uint[1] { 33u },
		new uint[1] { 40u },
		new uint[2] { 2u, 20u },
		new uint[2] { 4u, 22u },
		new uint[2] { 29u, 30u },
		new uint[1] { 34u },
		new uint[1] { 39u },
		new uint[1] { 41u },
		new uint[2] { 5u, 23u },
		new uint[1] { 31u },
		new uint[1] { 38u },
		new uint[2] { 7u, 25u },
		new uint[2] { 26u, 27u },
		new uint[1] { 35u },
		new uint[1] { 36u },
		new uint[1] { 42u },
		new uint[8] { 8u, 9u, 10u, 11u, 12u, 13u, 14u, 15u },
		new uint[3] { 16u, 17u, 18u }
	};

	private static readonly uint[] PlateSlotAddonTextIDs = new uint[12]
	{
		11960u, 11961u, 11962u, 11963u, 11964u, 11965u, 11966u, 11968u, 11967u, 11969u,
		750u, 749u
	};

	private static readonly int[] PlateSlots = new int[12]
	{
		0, 1, 2, 3, 4, 6, 7, 8, 9, 10,
		11, 12
	};

	[CompilerGenerated]
	private static string[] _003CJobFilterNames_003Ek__BackingField;

	private static readonly PlateSlotDefinition[] PlateSlotDefinitions = new PlateSlotDefinition[12]
	{
		new PlateSlotDefinition(11960u, (EquipSlotCategory x) => ((EquipSlotCategory)(ref x)).MainHand != 0),
		new PlateSlotDefinition(11961u, (EquipSlotCategory x) => ((EquipSlotCategory)(ref x)).OffHand != 0 && ((EquipSlotCategory)(ref x)).MainHand == 0),
		new PlateSlotDefinition(11962u, (EquipSlotCategory x) => ((EquipSlotCategory)(ref x)).Head != 0),
		new PlateSlotDefinition(11963u, (EquipSlotCategory x) => ((EquipSlotCategory)(ref x)).Body != 0),
		new PlateSlotDefinition(11964u, (EquipSlotCategory x) => ((EquipSlotCategory)(ref x)).Gloves != 0),
		new PlateSlotDefinition(11965u, (EquipSlotCategory x) => ((EquipSlotCategory)(ref x)).Legs != 0),
		new PlateSlotDefinition(11966u, (EquipSlotCategory x) => ((EquipSlotCategory)(ref x)).Feet != 0),
		new PlateSlotDefinition(11968u, (EquipSlotCategory x) => ((EquipSlotCategory)(ref x)).Ears != 0),
		new PlateSlotDefinition(11967u, (EquipSlotCategory x) => ((EquipSlotCategory)(ref x)).Neck != 0),
		new PlateSlotDefinition(11969u, (EquipSlotCategory x) => ((EquipSlotCategory)(ref x)).Wrists != 0),
		new PlateSlotDefinition(750u, (EquipSlotCategory x) => ((EquipSlotCategory)(ref x)).FingerL != 0 || ((EquipSlotCategory)(ref x)).FingerR != 0),
		new PlateSlotDefinition(749u, (EquipSlotCategory x) => ((EquipSlotCategory)(ref x)).FingerL != 0 || ((EquipSlotCategory)(ref x)).FingerR != 0)
	};

	[CompilerGenerated]
	private static string[] _003CSortModeNames_003Ek__BackingField;

	private static readonly Vector4 TitleColor = KnownColor.HotPink.ToVector4();

	private static readonly Vector4 SelectedColor;

	private static readonly Vector4 ButtonAccentColor;

	private static readonly Vector4 ButtonActiveColor;

	private static readonly Vector4 SoftAccentColor;

	private static readonly Vector4 GoldColor;

	private static readonly Vector4 ErrorColor;

	private static readonly Vector4 FrameBGColor;

	private static readonly Vector4 NormalCardColor;

	private static readonly Vector4 NormalCardHoverColor;

	private static readonly Vector4 FavoriteCardColor;

	private static readonly Vector4 FavoriteCardHoverColor;

	private static readonly Vector4 SelectedBorderColor;

	private static readonly Vector4 MutedBorderColor;

	private static readonly Vector4 StarOffColor;

	private readonly List<UnifiedItem> items = new List<UnifiedItem>();

	private bool isRefreshingItems;

	private readonly List<UnifiedItem> filteredItems = new List<UnifiedItem>();

	private readonly HashSet<uint> favoriteItemIDs = new HashSet<uint>();

	private readonly Dictionary<ulong, bool> jobFilterCache = new Dictionary<ulong, bool>();

	private readonly Dictionary<ulong, bool> plateSlotFilterCache = new Dictionary<ulong, bool>();

	private string searchText = string.Empty;

	private string presetSearch = string.Empty;

	private SourceFilter sourceFilter;

	private SortMode sortMode;

	private SetRelationFilter setRelationFilter;

	private bool filterByCurrentPlateSlot = true;

	private bool enableLevelFilter;

	private int minEquipLevel = 1;

	private int maxEquipLevel = 100;

	private int selectedJobFilterIndex;

	private bool requestClearFavoritesConfirm;

	private bool favoritesConfirmPopupOpen;

	private bool filteredItemsDirty = true;

	private uint lastFilterPlateSlot = uint.MaxValue;

	private byte sourceRace;

	private byte sourceSex;

	private UnifiedItem? selectedItem;

	private PlatePreset? selectedPreset;

	private PlatePreset? editingPreset;

	private string editPresetTitleInput = string.Empty;

	private string editPresetNoteInput = string.Empty;

	private static string[] JobFilterNames
	{
		get
		{
			if (_003CJobFilterNames_003Ek__BackingField != null)
			{
				return _003CJobFilterNames_003Ek__BackingField;
			}
			string[] array = new string[JobFilterClassJobIDs.Length];
			array[0] = LanguageManager.Get("All");
			for (int i = 1; i < JobFilterClassJobIDs.Length; i++)
			{
				array[i] = string.Join(" / ", JobFilterClassJobIDs[i].Select(LuminaWrapper.GetJobName));
			}
			return _003CJobFilterNames_003Ek__BackingField = array;
		}
	}

	private static string[] SortModeNames
	{
		get
		{
			if (_003CSortModeNames_003Ek__BackingField != null)
			{
				return _003CSortModeNames_003Ek__BackingField;
			}
			string text = LanguageManager.Get("Name") + " (" + LanguageManager.Get("Ascending") + ")";
			string text2 = LanguageManager.Get("Name") + " (" + LanguageManager.Get("Descending") + ")";
			string text3 = LuminaWrapper.GetAddonText(335u) + " (" + LanguageManager.Get("Ascending") + ")";
			string text4 = LuminaWrapper.GetAddonText(335u) + " (" + LanguageManager.Get("Descending") + ")";
			string[] result = new string[5]
			{
				LanguageManager.Get("Favorite") + " / " + text,
				text,
				text2,
				text3,
				text4
			};
			_003CSortModeNames_003Ek__BackingField = result;
			return result;
		}
	}

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("UnifiedGlamourManagerTitle"),
		Description = LanguageManager.Get("UnifiedGlamourManagerDescription"),
		Category = ModuleCategory.Interface,
		Author = new string[1] { "ErxCharlotte" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	private static float CardHeight
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			float num = ImGui.GetFrameHeight() * 1.6f;
			ImGuiStylePtr style = ImGui.GetStyle();
			float x = num + ((ImGuiStylePtr)(ref style)).WindowPadding.Y * 2f;
			float num2 = ImGui.GetTextLineHeightWithSpacing() * 2f;
			style = ImGui.GetStyle();
			return MathF.Max(x, num2 + ((ImGuiStylePtr)(ref style)).WindowPadding.Y * 2f);
		}
	}

	private unsafe void OnPlateEditorAddon(AddonEvent type, AddonArgs args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Invalid comparison between Unknown and I4
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		AgentMiragePrismMiragePlate* agent;
		if ((int)type != 1)
		{
			if ((int)type != 5)
			{
				if ((int)type == 6)
				{
					plateSaveButtonNode = null;
					base.TaskHelper?.Abort();
					((Window)base.Overlay).IsOpen = false;
				}
			}
			else
			{
				if (!(*Addons.MiragePrismMiragePlate).IsAddonAndNodesReady())
				{
					return;
				}
				if (plateSaveButtonNode == null)
				{
					plateSaveButtonNode = new TextButtonNode
					{
						Size = new Vector2(200f, 30f),
						Position = new Vector2(200f, 10f)
					};
					plateSaveButtonNode.AttachNode(((AtkUnitBase)Addons.MiragePrismMiragePlate).RootNode);
				}
				if (Throttler.Shared.Throttle("UnifiedGlamourManager-Preset-plateSaveButton"))
				{
					plateSaveButtonNode.IsEnabled = GetCurrentPlateItems().Count > 0;
					plateSaveButtonNode.String = ReadOnlySeString.op_Implicit(LanguageManager.Get("UnifiedGlamourManager-Preset-SaveGlamourPreset"));
					plateSaveButtonNode.OnClick = delegate
					{
						SaveCurrentPlateAsPreset(string.Empty, string.Empty, PresetSource.Self);
					};
				}
			}
		}
		else if (TryGetReadyPlateEditor(out agent) && ((AgentMiragePrismMiragePlateData)((AgentMiragePrismMiragePlate)agent).Data).OpenMode == 0)
		{
			((Window)base.Overlay).IsOpen = true;
			StartRefreshAll();
		}
	}

	private unsafe void OnInspectAddon(AddonEvent type, AddonArgs? args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		if ((int)type != 5)
		{
			if ((int)type == 6)
			{
				inspectSaveButtonNode = null;
				base.TaskHelper?.Abort();
			}
		}
		else
		{
			if (!(*Addons.CharacterInspect).IsAddonAndNodesReady())
			{
				return;
			}
			AtkResNode* nodeById = ((AtkUnitBase)Addons.CharacterInspect).GetNodeById(6u);
			if (nodeById == null)
			{
				return;
			}
			if (inspectSaveButtonNode == null)
			{
				inspectSaveButtonNode = new TextButtonNode
				{
					Size = new Vector2((int)((AtkResNode)nodeById).Width, (int)((AtkResNode)nodeById).Height),
					Position = new Vector2(180f, 10f)
				};
				inspectSaveButtonNode.AttachNode(((AtkUnitBase)Addons.CharacterInspect).RootNode);
			}
			if (Throttler.Shared.Throttle("UnifiedGlamourManager-Preset-InspectButton"))
			{
				inspectSaveButtonNode.IsEnabled = GetInspectPlateItems().Count > 0;
				inspectSaveButtonNode.String = ReadOnlySeString.op_Implicit(LanguageManager.Get("UnifiedGlamourManager-Preset-SaveGlamourPreset"));
				inspectSaveButtonNode.OnClick = delegate
				{
					SaveCurrentPlateAsPreset(string.Empty, string.Empty, PresetSource.OtherPlayer);
				};
			}
		}
	}

	private unsafe void ApplySelectedItemToCurrentPlateSlot(UnifiedItem item)
	{
		if (!item.CanUseInPlate || !TryGetReadyPlateEditor(out var agent))
		{
			return;
		}
		uint selectedItemIndex = ((AgentMiragePrismMiragePlateData)((AgentMiragePrismMiragePlate)agent).Data).SelectedItemIndex;
		if (!item.IsCompatibleWithSlot(selectedItemIndex))
		{
			return;
		}
		try
		{
			if (item.InPrismBox)
			{
				if (TryApplyPrismBoxItem(agent, item))
				{
					QueueApplyRetry(item, (ItemSource)1);
				}
			}
			else if (item.InCabinet && TryApplyCabinetItem(agent, item))
			{
				QueueApplyRetry(item, (ItemSource)2);
			}
		}
		catch (Exception ex)
		{
			DLog.Warning("UnifiedGlamourManager apply failed", ex);
		}
	}

	private unsafe static bool TryApplyPrismBoxItem(AgentMiragePrismMiragePlate* agent, UnifiedItem item)
	{
		if (!TryGetLoadedMirageManager(out var manager))
		{
			return false;
		}
		if (item.PrismBoxIndex >= 800)
		{
			return false;
		}
		uint num = ((MirageManager)manager).PrismBoxItemIds[(int)item.PrismBoxIndex];
		if (num == 0)
		{
			return false;
		}
		uint num2 = (((object)item != null && item.IsSetPart && item.ParentSetItemID != 0) ? item.ParentSetItemID : item.ItemID);
		if (ItemUtil.GetBaseId(num).Item1 != num2)
		{
			return false;
		}
		uint num3 = ((!item.IsSetPart && item.RawItemID != 0) ? item.RawItemID : item.ItemID);
		byte b = (byte)((!item.IsSetPart) ? item.Stain0ID : 0u);
		byte b2 = (byte)((!item.IsSetPart) ? item.Stain1ID : 0u);
		((AgentMiragePrismMiragePlate)agent).SetSelectedItemData((ItemSource)1, item.PrismBoxIndex, num3, b, b2);
		MarkPlateSelectionDirty(agent);
		return true;
	}

	private unsafe static bool TryApplyCabinetItem(AgentMiragePrismMiragePlate* agent, UnifiedItem item)
	{
		Cabinet* loadedCabinet = GetLoadedCabinet();
		if (loadedCabinet == null)
		{
			return false;
		}
		if (item.CabinetID == 0 || !((Cabinet)loadedCabinet).IsItemInCabinet(item.CabinetID))
		{
			return false;
		}
		((AgentMiragePrismMiragePlate)agent).SetSelectedItemData((ItemSource)2, item.CabinetID, item.ItemID, (byte)0, (byte)0);
		MarkPlateSelectionDirty(agent);
		return true;
	}

	private unsafe void QueueApplyRetry(UnifiedItem item, ItemSource source)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		uint itemID = item.ItemID;
		uint prismBoxIndex = item.PrismBoxIndex;
		uint cabinetID = item.CabinetID;
		bool isSetPart = item.IsSetPart;
		uint parentSetItemID = item.ParentSetItemID;
		DalamudServiceExtension.Instance<IFramework>().RunOnTick((Action)delegate
		{
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Invalid comparison between Unknown and I4
			if (!base.IsDisposed && TryGetReadyPlateEditor(out var agent))
			{
				UnifiedItem unifiedItem = items.FirstOrDefault(delegate(UnifiedItem x)
				{
					//IL_0033: Unknown result type (might be due to invalid IL or missing references)
					//IL_0039: Invalid comparison between Unknown and I4
					if (x.ItemID == itemID && x.IsSetPart == isSetPart && x.ParentSetItemID == parentSetItemID && x.CanUseInPlate)
					{
						if ((int)source != 1)
						{
							if (x.InCabinet)
							{
								return x.CabinetID == cabinetID;
							}
							return false;
						}
						if (x.InPrismBox)
						{
							return x.PrismBoxIndex == prismBoxIndex;
						}
						return false;
					}
					return false;
				});
				if (!(unifiedItem == null) && unifiedItem.IsCompatibleWithSlot(((AgentMiragePrismMiragePlateData)((AgentMiragePrismMiragePlate)agent).Data).SelectedItemIndex))
				{
					if ((int)source == 1)
					{
						TryApplyPrismBoxItem(agent, unifiedItem);
					}
					else
					{
						TryApplyCabinetItem(agent, unifiedItem);
					}
				}
			}
		}, TimeSpan.FromMilliseconds(50L), 0, default(CancellationToken));
	}

	private unsafe static void MarkPlateSelectionDirty(AgentMiragePrismMiragePlate* agent)
	{
		if (agent != null && ((AgentMiragePrismMiragePlate)agent).Data != null)
		{
			((AgentMiragePrismMiragePlateData)((AgentMiragePrismMiragePlate)agent).Data).HasChanges = true;
			((MiragePrismMiragePlateCharaView)(&((AgentMiragePrismMiragePlate)agent).CharaView)).IsUpdatePending = true;
		}
	}

	private unsafe void SaveCurrentPlateAsPreset(string title, string note, PresetSource source)
	{
		bool flag = source == PresetSource.OtherPlayer;
		List<PlateItemInfo> list = (flag ? GetInspectPlateItems() : GetCurrentPlateItems());
		if (list.Count != 0)
		{
			if (flag)
			{
				sourceRace = (byte)((InspectCharaView)(&((AgentInspect)AgentInspect.Instance()).CharaView)).Race;
				sourceSex = (byte)((InspectCharaView)(&((AgentInspect)AgentInspect.Instance()).CharaView)).Sex;
			}
			else
			{
				sourceRace = ((CustomizeData)(&((DrawDataContainer)(&((BattleChara)Control.GetLocalPlayer()).DrawData)).CustomizeData)).Race;
				sourceSex = ((CustomizeData)(&((DrawDataContainer)(&((BattleChara)Control.GetLocalPlayer()).DrawData)).CustomizeData)).Sex;
			}
			PlatePreset platePreset = new PlatePreset
			{
				Title = (string.IsNullOrWhiteSpace(title) ? GetDefaultPresetTitle(source) : title.Trim()),
				Note = note.Trim(),
				Race = GetRaceName(sourceRace, sourceSex),
				Sex = GetSexName(sourceSex),
				CreatedAt = DateTime.Now
			};
			platePreset.Items.AddRange(list);
			config.Presets.Add(platePreset);
			selectedPreset = platePreset;
			config.Save(this);
			OmenServiceBase<NotifyHelper>.Instance().NotificationSuccess(LanguageManager.Get("SavedSuccessfully"));
		}
	}

	private unsafe bool StartApplyPreset()
	{
		if (!TryGetReadyPlateEditor(out var _) || !TryGetLoadedMirageManager(out var _) || selectedPreset == null)
		{
			return false;
		}
		List<PlateItemInfo> list = (from x in selectedPreset.Items
			where x.SlotIndex < PlateSlotAddonTextIDs.Length
			orderby x.SlotIndex
			select x).ToList();
		missingApplyItems.Clear();
		foreach (PlateItemInfo entry in list)
		{
			uint itemID = ItemUtil.GetBaseId(entry.ItemID).Item1;
			UnifiedItem target = (from x in items
				where x.ItemID == itemID && x.CanUseInPlate && x.IsCompatibleWithSlot(entry.SlotIndex)
				orderby x.IsSetPart, x.InPrismBox descending
				select x).FirstOrDefault();
			if (target == null)
			{
				missingApplyItems.Add(entry);
				continue;
			}
			bool sentApply = false;
			uint originStain0ID = 0u;
			uint originStain1ID = 0u;
			base.TaskHelper.Enqueue(delegate
			{
				//IL_0149: Unknown result type (might be due to invalid IL or missing references)
				//IL_014e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0153: Unknown result type (might be due to invalid IL or missing references)
				//IL_0158: Unknown result type (might be due to invalid IL or missing references)
				//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
				//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
				if (!TryGetReadyPlateEditor(out var agent2))
				{
					return false;
				}
				int slotIndex = (int)entry.SlotIndex;
				if (!sentApply)
				{
					((AgentMiragePrismMiragePlateData)((AgentMiragePrismMiragePlate)agent2).Data).SelectedItemIndex = entry.SlotIndex;
					if (target.InPrismBox && TryApplyPrismBoxItem(agent2, target))
					{
						if (!target.IsSetPart)
						{
							originStain0ID = target.Stain0ID;
							originStain1ID = target.Stain1ID;
						}
						sentApply = true;
						return false;
					}
					if (target.InCabinet && TryApplyCabinetItem(agent2, target))
					{
						sentApply = true;
						return false;
					}
					missingApplyItems.Add(entry);
					return true;
				}
				ref MiragePlateItem reference = ref ((AgentMiragePrismMiragePlateData)((AgentMiragePrismMiragePlate)agent2).Data).CurrentItems[slotIndex];
				if (reference.ItemId == 0 || ItemUtil.GetBaseId(reference.ItemId).Item1 != itemID)
				{
					sentApply = false;
					return false;
				}
				if (entry.Stain0ID != (byte)originStain0ID)
				{
					ref ItemFlag flags = ref reference.Flags;
					flags = (ItemFlag)((uint)flags | 1u);
					Stain item;
					uint dye0ID = (LuminaGetter.TryGetRow<Stain>((uint)entry.Stain0ID, out item) ? ((Stain)(ref item)).Item[0].RowId : 0u);
					((IEnumerable<InventoryType>)Inventories.Player).TryGetFirstItem((Predicate<InventoryItem>)((InventoryItem x) => x.ItemId == dye0ID), out InventoryItem* itemResult);
					((AgentMiragePrismMiragePlate)agent2).SetItemStain(entry.SlotIndex, entry.Stain0ID, itemResult, dye0ID, 0);
				}
				if (entry.Stain1ID != (byte)originStain1ID)
				{
					ref ItemFlag flags2 = ref reference.Flags;
					flags2 = (ItemFlag)((uint)flags2 | 0x20u);
					Stain item2;
					uint dye1ID = (LuminaGetter.TryGetRow<Stain>((uint)entry.Stain1ID, out item2) ? ((Stain)(ref item2)).Item[0].RowId : 0u);
					((IEnumerable<InventoryType>)Inventories.Player).TryGetFirstItem((Predicate<InventoryItem>)((InventoryItem x) => x.ItemId == dye1ID), out InventoryItem* itemResult2);
					((AgentMiragePrismMiragePlate)agent2).SetItemStain(entry.SlotIndex, entry.Stain1ID, itemResult2, dye1ID, 1);
				}
				MarkPlateSelectionDirty(agent2);
				return true;
			});
			base.TaskHelper.DelayNext(150);
		}
		base.TaskHelper.Enqueue(delegate
		{
			openMissingApplyItemsPopup = missingApplyItems.Count > 0;
			return true;
		});
		return true;
	}

	private unsafe bool StartTryOnPreset(PlatePreset preset)
	{
		if (IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()) || AgentTryon.Instance() == null)
		{
			return false;
		}
		List<PlateItemInfo> list = (from x in preset.Items
			where x.ItemID != 0 && x.SlotIndex < PlateSlotAddonTextIDs.Length && LuminaGetter.TryGetRow<Item>(ItemUtil.GetBaseId(x.ItemID).Item1, out Item _)
			orderby x.SlotIndex
			select x).ToList();
		if (list == null || list.Count <= 0)
		{
			base.TaskHelper.Abort();
			return true;
		}
		((AgentTryon)AgentTryon.Instance()).SaveDeleteOutfit = true;
		foreach (PlateItemInfo entry in list)
		{
			base.TaskHelper.Enqueue(delegate
			{
				AgentTryon.TryOn(0u, entry.ItemID, entry.Stain0ID, entry.Stain1ID, 0u, false);
				return true;
			});
			base.TaskHelper.DelayNext(50);
		}
		return true;
	}

	private void NormalizeConfig()
	{
		config.Favorites = (from x in config.Favorites.Where((SavedItem x) => x.ItemID != 0 && LuminaGetter.TryGetRow<Item>(x.ItemID, out Item _)).Select(delegate(SavedItem x)
			{
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				string text = x.Name ?? string.Empty;
				if (string.IsNullOrWhiteSpace(text) && LuminaGetter.TryGetRow<Item>(x.ItemID, out Item item) && TryGetItemName(item, out string name))
				{
					text = name;
				}
				return new SavedItem
				{
					ItemID = x.ItemID,
					Name = text,
					AddedAt = Math.Max(0L, x.AddedAt)
				};
			})
			group x by x.ItemID into x
			select x.OrderByDescending((SavedItem y) => y.AddedAt).First() into x
			orderby x.AddedAt descending
			select x).ToList();
		favoriteItemIDs.Clear();
		favoriteItemIDs.UnionWith(config.Favorites.Select((SavedItem x) => x.ItemID));
	}

	private bool IsFavorite(uint itemID)
	{
		return favoriteItemIDs.Contains(itemID);
	}

	private void ToggleFavorite(UnifiedItem item)
	{
		if (config.Favorites.RemoveAll((SavedItem x) => x.ItemID == item.ItemID) == 0)
		{
			config.Favorites.Add(new SavedItem
			{
				ItemID = item.ItemID,
				Name = item.Name,
				AddedAt = OmenServiceBase<StandardTimeManager>.Instance().UTCNowOffset.ToUnixTimeSeconds()
			});
		}
		SaveConfig();
	}

	private void SaveConfig()
	{
		NormalizeConfig();
		config.Save(this);
		MarkFilteredItemsDirty();
	}

	private static string[] CreateSetRelationFilterNames()
	{
		return new string[3]
		{
			LanguageManager.Get("All"),
			LanguageManager.Get("UnifiedGlamourManager-SetRelatedOnly"),
			LanguageManager.Get("UnifiedGlamourManager-NonSetOnly")
		};
	}

	protected override void Init()
	{
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Expected O, but got Unknown
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Expected O, but got Unknown
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Expected O, but got Unknown
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Expected O, but got Unknown
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Expected O, but got Unknown
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Expected O, but got Unknown
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		NormalizeConfig();
		selectedPreset = config.Presets.OrderByDescending((PlatePreset x) => x.CreatedAt).FirstOrDefault();
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
		((Window)overlay3).Flags = (ImGuiWindowFlags)(((Window)overlay3).Flags & -65);
		Overlay? overlay4 = base.Overlay;
		((Window)overlay4).Flags = (ImGuiWindowFlags)(((Window)overlay4).Flags & -2);
		Overlay? overlay5 = base.Overlay;
		((Window)overlay5).Flags = (ImGuiWindowFlags)(((Window)overlay5).Flags | 0x18);
		((Window)base.Overlay).WindowName = Info.Title + "###UnifiedGlamourManagerOverlay";
		IAddonLifecycle obj2 = DalamudServiceExtension.Instance<IAddonLifecycle>();
		obj2.RegisterListener((AddonEvent)1, "MiragePrismMiragePlate", new AddonEventDelegate(OnPlateEditorAddon));
		obj2.RegisterListener((AddonEvent)5, "MiragePrismMiragePlate", new AddonEventDelegate(OnPlateEditorAddon));
		obj2.RegisterListener((AddonEvent)6, "MiragePrismMiragePlate", new AddonEventDelegate(OnPlateEditorAddon));
		obj2.RegisterListener((AddonEvent)5, "CharacterInspect", new AddonEventDelegate(OnInspectAddon));
		obj2.RegisterListener((AddonEvent)6, "CharacterInspect", new AddonEventDelegate(OnInspectAddon));
		OmenServiceBase<CommandManager>.Instance().AddSubCommand("ugm", new CommandInfo(new HandlerDelegate(OnCommand))
		{
			HelpMessage = LanguageManager.Get("UnifiedGlamourManager-CommandHelp")
		});
	}

	protected override void Uninit()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		OmenServiceBase<CommandManager>.Instance().RemoveSubCommand("ugm");
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnInspectAddon });
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnPlateEditorAddon });
		inspectSaveButtonNode?.Dispose();
		inspectSaveButtonNode = null;
		plateSaveButtonNode?.Dispose();
		plateSaveButtonNode = null;
	}

	private void OnCommand(string command, string arguments)
	{
		((Window)base.Overlay).IsOpen = true;
	}

	private static bool IsEquipSlotCategoryCompatibleWithPlateSlot(EquipSlotCategory category, uint slotIndex)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (slotIndex < PlateSlotDefinitions.Length)
		{
			return PlateSlotDefinitions[slotIndex].CanUse(category);
		}
		return false;
	}

	private unsafe static bool TryGetLoadedMirageManager(out MirageManager* manager)
	{
		manager = MirageManager.Instance();
		if (manager != null && ((MirageManager)manager).PrismBoxRequested)
		{
			return ((MirageManager)manager).PrismBoxLoaded;
		}
		return false;
	}

	private unsafe static bool TryGetReadyPlateEditor(out AgentMiragePrismMiragePlate* agent)
	{
		agent = AgentMiragePrismMiragePlate.Instance();
		if (agent != null && ((AgentMiragePrismMiragePlate)agent).IsAgentActive() && ((AgentMiragePrismMiragePlate)agent).Data != null)
		{
			return ((AgentMiragePrismMiragePlateData)((AgentMiragePrismMiragePlate)agent).Data).SelectedItemIndex < PlateSlotDefinitions.Length;
		}
		return false;
	}

	private void StartRefreshAll(UnifiedItem? reselectItem = null)
	{
		UnifiedItem itemToReselect = reselectItem ?? selectedItem;
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 30000
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		base.TaskHelper.Abort();
		isRefreshingItems = true;
		items.Clear();
		filteredItems.Clear();
		MarkFilteredItemsDirty(clearJobCache: true, clearPlateSlotCache: true);
		base.TaskHelper.Enqueue(delegate
		{
			RunRefreshStep(LoadPrismBoxItems, "LoadPrismBoxItems");
		}, "LoadPrismBoxItems");
		base.TaskHelper.DelayNext(1);
		base.TaskHelper.Enqueue(delegate
		{
			RunRefreshStep(LoadCabinetItems, "LoadCabinetItems");
		}, "LoadCabinetItems");
		base.TaskHelper.DelayNext(1);
		base.TaskHelper.Enqueue(delegate
		{
			try
			{
				MergeItems();
				selectedItem = ((itemToReselect != null) ? items.FirstOrDefault((UnifiedItem x) => x == itemToReselect) : null);
			}
			catch (Exception ex)
			{
				DLog.Error("UnifiedGlamourManager refresh merge failed", ex);
				selectedItem = null;
			}
			finally
			{
				isRefreshingItems = false;
				MarkFilteredItemsDirty(clearJobCache: true, clearPlateSlotCache: true);
			}
		}, "MergeItems");
	}

	private static void RunRefreshStep(Action action, string stepName)
	{
		try
		{
			action();
		}
		catch (Exception ex)
		{
			DLog.Error("UnifiedGlamourManager refresh step failed: " + stepName, ex);
		}
	}

	private unsafe void LoadPrismBoxItems()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetLoadedMirageManager(out var manager))
		{
			return;
		}
		ExcelSheet<Item> val = LuminaGetter.Get<Item>();
		for (uint num = 0u; num < 800; num++)
		{
			uint num2 = ((MirageManager)manager).PrismBoxItemIds[(int)num];
			if (num2 != 0)
			{
				uint item = ItemUtil.GetBaseId(num2).Item1;
				Item? rowOrDefault = val.GetRowOrDefault(item);
				if (rowOrDefault.HasValue && TryGetItemName(rowOrDefault.Value, out string name))
				{
					AddStoredItem(rowOrDefault.Value, name, (ItemSource)1, item, num2, num, 0u, ((MirageManager)manager).PrismBoxStain0Ids[(int)num], ((MirageManager)manager).PrismBoxStain1Ids[(int)num], manager);
				}
			}
		}
	}

	private unsafe void LoadCabinetItems()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		Cabinet* loadedCabinet = GetLoadedCabinet();
		if (loadedCabinet == null)
		{
			return;
		}
		ExcelSheet<Item> val = LuminaGetter.Get<Item>();
		Enumerator<Cabinet> enumerator = LuminaGetter.Get<Cabinet>().GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Cabinet current = enumerator.Current;
				uint rowId = ((Cabinet)(ref current)).RowId;
				uint rowId2 = ((Cabinet)(ref current)).Item.RowId;
				if (rowId2 != 0 && ((Cabinet)loadedCabinet).IsItemInCabinet(rowId))
				{
					Item? rowOrDefault = val.GetRowOrDefault(rowId2);
					if (rowOrDefault.HasValue && TryGetItemName(rowOrDefault.Value, out string name))
					{
						AddStoredItem(rowOrDefault.Value, name, (ItemSource)2, rowId2, rowId2, 0u, rowId, 0u, 0u, null);
					}
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
	}

	private unsafe void AddStoredItem(Item item, string name, ItemSource source, uint itemID, uint rawItemID, uint prismBoxIndex, uint cabinetID, uint stain0ID = 0u, uint stain1ID = 0u, MirageManager* manager = null)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Invalid comparison between Unknown and I4
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		List<SetPartInfo> setParts = GetSetParts(itemID);
		items.Add(UnifiedItem.Create(itemID, rawItemID, name, item, source, prismBoxIndex, cabinetID, stain0ID, stain1ID, setParts.Count > 0));
		foreach (SetPartInfo item2 in setParts)
		{
			if ((int)source != 1 || (manager != null && ((MirageManager)manager).IsSetSlotUnlocked(prismBoxIndex, item2.SlotIndex)))
			{
				AddSetPartItem(item2, rawItemID, prismBoxIndex, cabinetID, itemID, name, source);
			}
		}
	}

	private unsafe static Cabinet* GetLoadedCabinet()
	{
		UIState* ptr = UIState.Instance();
		if (ptr == null)
		{
			return null;
		}
		Cabinet* ptr2 = &((UIState)ptr).Cabinet;
		if (!((Cabinet)ptr2).IsCabinetLoaded())
		{
			return null;
		}
		return ptr2;
	}

	private static bool TryGetItemName(Item item, out string name)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		ReadOnlySeString name2 = ((Item)(ref item)).Name;
		name = ((ReadOnlySeString)(ref name2)).ExtractText();
		return !string.IsNullOrWhiteSpace(name);
	}

	private unsafe static List<PlateItemInfo> GetCurrentPlateItems()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetReadyPlateEditor(out var agent))
		{
			return new List<PlateItemInfo>();
		}
		List<PlateItemInfo> list = new List<PlateItemInfo>();
		Span<MiragePlateItem> currentItems = ((AgentMiragePrismMiragePlateData)((AgentMiragePrismMiragePlate)agent).Data).CurrentItems;
		for (int i = 0; i < currentItems.Length; i++)
		{
			MiragePlateItem val = currentItems[i];
			if (val.ItemId != 0)
			{
				list.Add(new PlateItemInfo((uint)i, val.ItemId, ((MiragePlateItem)(ref val)).StainIds[0], ((MiragePlateItem)(ref val)).StainIds[1]));
			}
		}
		return list;
	}

	private static List<PlateItemInfo> GetInspectPlateItems()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		if (!((InventoryType)2009).TryGetItems((InventoryItem x) => x.ItemId != 0 && PlateSlots.Contains(x.Slot), out List<InventoryItem> itemResult))
		{
			return new List<PlateItemInfo>();
		}
		List<PlateItemInfo> list = new List<PlateItemInfo>();
		foreach (InventoryItem item in itemResult)
		{
			InventoryItem current = item;
			uint num = ((current.GlamourId != 0) ? current.GlamourId : current.ItemId);
			uint slotIndex = (uint)Array.IndexOf(PlateSlots, current.Slot);
			if (num != 0)
			{
				list.Add(new PlateItemInfo(slotIndex, num, ((InventoryItem)(ref current)).Stains[0], ((InventoryItem)(ref current)).Stains[1]));
			}
		}
		return list;
	}

	private static string GetRaceName(byte race, byte sex)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		string text;
		if (LuminaGetter.TryGetRow<Race>((uint)race, out Race item))
		{
			if (sex != 1)
			{
				text = ((object)((Race)(ref item)).Masculine/*cast due to constrained. prefix*/).ToString();
				if (text == null)
				{
					return LanguageManager.Get("Unknown");
				}
			}
			else
			{
				text = ((object)((Race)(ref item)).Feminine/*cast due to constrained. prefix*/).ToString();
				if (text == null)
				{
					return LanguageManager.Get("Unknown");
				}
			}
		}
		else
		{
			text = LanguageManager.Get("Unknown");
		}
		return text;
	}

	private static string GetSexName(byte sex)
	{
		if (sex != 1)
		{
			return LuminaWrapper.GetAddonText(15608u);
		}
		return LuminaWrapper.GetAddonText(15609u);
	}

	private static string GetDefaultPresetTitle(PresetSource source)
	{
		return source switch
		{
			PresetSource.Self => LanguageManager.Get("UnifiedGlamourManager-Preset-UntitledPreset") + "-" + LuminaWrapper.GetAddonText(3991u), 
			PresetSource.OtherPlayer => LanguageManager.Get("UnifiedGlamourManager-Preset-UntitledPreset") + "-" + LuminaWrapper.GetAddonText(7979u), 
			_ => LanguageManager.Get("UnifiedGlamourManager-Preset-UntitledPreset"), 
		};
	}

	private bool PassFilter(UnifiedItem item)
	{
		if (filterByCurrentPlateSlot && !CanItemUseInCurrentPlateSlot(item))
		{
			return false;
		}
		if (enableLevelFilter && (item.LevelEquip < minEquipLevel || item.LevelEquip > maxEquipLevel))
		{
			return false;
		}
		switch (sourceFilter)
		{
		case SourceFilter.PrismBox:
			if (item.InPrismBox)
			{
				break;
			}
			goto IL_007c;
		case SourceFilter.Cabinet:
			if (item.InCabinet)
			{
				break;
			}
			goto IL_007c;
		case SourceFilter.Favorite:
			{
				if (IsFavorite(item.ItemID))
				{
					break;
				}
				goto IL_007c;
			}
			IL_007c:
			return false;
		}
		SetRelationFilter setRelationFilter = this.setRelationFilter;
		if (setRelationFilter != SetRelationFilter.SetRelatedOnly)
		{
			if (setRelationFilter == SetRelationFilter.NonSetOnly && (item.IsSetContainer || item.IsSetPart))
			{
				goto IL_00b4;
			}
		}
		else if ((object)item != null && !item.IsSetContainer && !item.IsSetPart)
		{
			goto IL_00b4;
		}
		if (!PassJobFilter(item))
		{
			return false;
		}
		if (string.IsNullOrWhiteSpace(searchText))
		{
			return true;
		}
		string value = searchText.Trim();
		if (!item.Name.Contains(value, StringComparison.OrdinalIgnoreCase) && !item.ParentSetName.Contains(value, StringComparison.OrdinalIgnoreCase))
		{
			return item.SetPartLabel.Contains(value, StringComparison.OrdinalIgnoreCase);
		}
		return true;
		IL_00b4:
		return false;
	}

	private bool PassJobFilter(UnifiedItem item)
	{
		if (selectedJobFilterIndex <= 0 || selectedJobFilterIndex >= JobFilterClassJobIDs.Length)
		{
			return true;
		}
		ulong key = ((ulong)(uint)selectedJobFilterIndex << 32) | item.ClassJobCategoryRowID;
		if (jobFilterCache.TryGetValue(key, out var value))
		{
			return value;
		}
		uint[] classJobIDs = JobFilterClassJobIDs[selectedJobFilterIndex];
		bool flag = item.IsCompatibleWithJobs(classJobIDs);
		jobFilterCache[key] = flag;
		return flag;
	}

	private unsafe bool CanItemUseInCurrentPlateSlot(UnifiedItem item)
	{
		if (!TryGetReadyPlateEditor(out var agent))
		{
			return true;
		}
		uint selectedItemIndex = ((AgentMiragePrismMiragePlateData)((AgentMiragePrismMiragePlate)agent).Data).SelectedItemIndex;
		ulong key = ((ulong)selectedItemIndex << 32) | item.EquipSlotCategoryRowID;
		if (plateSlotFilterCache.TryGetValue(key, out var value))
		{
			return value;
		}
		bool flag = item.IsCompatibleWithSlot(selectedItemIndex);
		plateSlotFilterCache[key] = flag;
		return flag;
	}

	private IEnumerable<UnifiedItem> ApplySort(IEnumerable<UnifiedItem> source)
	{
		return sortMode switch
		{
			SortMode.NameAsc => source.OrderBy((UnifiedItem x) => x.Name), 
			SortMode.NameDesc => source.OrderByDescending((UnifiedItem x) => x.Name), 
			SortMode.LevelAsc => from x in source
				orderby x.LevelEquip, x.Name
				select x, 
			SortMode.LevelDesc => from x in source
				orderby x.LevelEquip descending, x.Name
				select x, 
			_ => from x in source
				orderby IsFavorite(x.ItemID) descending, x.InPrismBox descending, x.InCabinet descending, x.IsSetPart descending, x.ParentSetName, x.Name
				select x, 
		};
	}

	private unsafe void UpdateCurrentSlotFilterCache()
	{
		if (filterByCurrentPlateSlot)
		{
			AgentMiragePrismMiragePlate* agent;
			uint num = (TryGetReadyPlateEditor(out agent) ? ((AgentMiragePrismMiragePlateData)((AgentMiragePrismMiragePlate)agent).Data).SelectedItemIndex : uint.MaxValue);
			if (num != lastFilterPlateSlot)
			{
				lastFilterPlateSlot = num;
				plateSlotFilterCache.Clear();
				MarkFilteredItemsDirty();
			}
		}
	}

	private void EnsureFilteredItems()
	{
		UpdateCurrentSlotFilterCache();
		if (filteredItemsDirty)
		{
			filteredItems.Clear();
			filteredItems.AddRange(ApplySort(items.Where(PassFilter)));
			filteredItemsDirty = false;
		}
	}

	private void MarkFilteredItemsDirty(bool clearJobCache = false, bool clearPlateSlotCache = false)
	{
		filteredItemsDirty = true;
		if (clearJobCache)
		{
			jobFilterCache.Clear();
		}
		if (clearPlateSlotCache)
		{
			plateSlotFilterCache.Clear();
		}
	}

	private void ResetFilters()
	{
		sourceFilter = SourceFilter.All;
		sortMode = SortMode.FavoriteThenNameAsc;
		setRelationFilter = SetRelationFilter.All;
		enableLevelFilter = false;
		minEquipLevel = 1;
		maxEquipLevel = 100;
		selectedJobFilterIndex = 0;
		searchText = string.Empty;
		filterByCurrentPlateSlot = true;
		MarkFilteredItemsDirty(clearJobCache: true, clearPlateSlotCache: true);
	}

	private static List<SetPartInfo> GetSetParts(uint setItemID)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		if (!LuminaGetter.TryGetRow<MirageStoreSetItem>(setItemID, out MirageStoreSetItem item))
		{
			return new List<SetPartInfo>();
		}
		List<SetPartInfo> list = new List<SetPartInfo>();
		AddSetPart(list, ((MirageStoreSetItem)(ref item)).MainHand.RowId, 0);
		AddSetPart(list, ((MirageStoreSetItem)(ref item)).OffHand.RowId, 1);
		AddSetPart(list, ((MirageStoreSetItem)(ref item)).Head.RowId, 2);
		AddSetPart(list, ((MirageStoreSetItem)(ref item)).Body.RowId, 3);
		AddSetPart(list, ((MirageStoreSetItem)(ref item)).Hands.RowId, 4);
		AddSetPart(list, ((MirageStoreSetItem)(ref item)).Legs.RowId, 5);
		AddSetPart(list, ((MirageStoreSetItem)(ref item)).Feet.RowId, 6);
		AddSetPart(list, ((MirageStoreSetItem)(ref item)).Earrings.RowId, 7);
		AddSetPart(list, ((MirageStoreSetItem)(ref item)).Necklace.RowId, 8);
		AddSetPart(list, ((MirageStoreSetItem)(ref item)).Bracelets.RowId, 9);
		AddSetPart(list, ((MirageStoreSetItem)(ref item)).Ring.RowId, 10);
		return list;
	}

	private static void AddSetPart(List<SetPartInfo> parts, uint itemID, int slotIndex)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if (itemID != 0 && LuminaGetter.TryGetRow<Item>(itemID, out Item item) && TryGetItemName(item, out string _) && LuminaGetter.TryGetRow<EquipSlotCategory>(((Item)(ref item)).EquipSlotCategory.RowId, out EquipSlotCategory item2) && IsEquipSlotCategoryCompatibleWithPlateSlot(item2, (uint)slotIndex))
		{
			string label = GetNativeItemCategoryName(item);
			if (!parts.Any((SetPartInfo x) => x.ItemID == itemID && x.PartLabel == label))
			{
				parts.Add(new SetPartInfo(itemID, label, slotIndex));
			}
		}
	}

	private void AddSetPartItem(SetPartInfo setPart, uint rawItemID, uint prismBoxIndex, uint cabinetID, uint parentItemID, string parentName, ItemSource source)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (LuminaGetter.TryGetRow<Item>(setPart.ItemID, out Item item) && TryGetItemName(item, out string name))
		{
			items.Add(UnifiedItem.Create(setPart.ItemID, rawItemID, name, item, source, prismBoxIndex, cabinetID, 0u, 0u, isSetContainer: false, isSetPart: true, parentItemID, parentName, setPart.PartLabel));
		}
	}

	private static string GetNativeItemCategoryName(Item item)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		ItemUICategory? valueNullable = ((Item)(ref item)).ItemUICategory.ValueNullable;
		object obj;
		ReadOnlySeString name;
		if (!valueNullable.HasValue)
		{
			obj = null;
		}
		else
		{
			ItemUICategory valueOrDefault = valueNullable.GetValueOrDefault();
			name = ((ItemUICategory)(ref valueOrDefault)).Name;
			obj = ((ReadOnlySeString)(ref name)).ExtractText();
		}
		string text = (string)obj;
		if (!string.IsNullOrWhiteSpace(text))
		{
			return text;
		}
		name = ((Item)(ref item)).Name;
		return ((ReadOnlySeString)(ref name)).ExtractText();
	}

	private void MergeItems()
	{
		List<UnifiedItem> collection = (from item in items
			group item by item.IsSetPart ? $"set:{item.ParentSetItemID}:{item.PrismBoxIndex}:{item.ItemID}" : $"item:{item.ItemID}").Select(MergeItemGroup).ToList();
		items.Clear();
		items.AddRange(collection);
	}

	private static UnifiedItem MergeItemGroup(IGrouping<string, UnifiedItem> group)
	{
		UnifiedItem unifiedItem = group.First();
		if (unifiedItem.IsSetPart)
		{
			return unifiedItem;
		}
		foreach (UnifiedItem item in group.Skip(1))
		{
			unifiedItem.MergeWith(item);
		}
		return unifiedItem;
	}

	protected override void ConfigUI()
	{
		//Discarded unreachable code: IL_00a5
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("Command")));
		IndentDisposable val = ImRaii.PushIndent(1, true);
		try
		{
			ImU8String val2 = new ImU8String(8, 2);
			((ImU8String)(ref val2)).AppendLiteral("/pdr ");
			((ImU8String)(ref val2)).AppendFormatted<string>("ugm");
			((ImU8String)(ref val2)).AppendLiteral(" → ");
			((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get("UnifiedGlamourManager-CommandHelp"));
			ImGui.TextUnformatted(val2);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	protected override void OverlayPreDraw()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		Vector2 vector = new Vector2(ImGui.GetFrameHeight() * 28f, ImGui.GetTextLineHeightWithSpacing() * 30f);
		ImGuiViewportPtr mainViewport = ImGui.GetMainViewport();
		ImGui.SetNextWindowSizeConstraints(vector, ((ImGuiViewportPtr)(ref mainViewport)).WorkSize);
	}

	protected override void OverlayUI()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		if (IConditionExtension.get_IsBetweenAreas(DalamudServiceExtension.Instance<ICondition>()))
		{
			return;
		}
		ImGuiStoragePtr stateStorage = ImGui.GetStateStorage();
		uint iD = ImGui.GetID(ImU8String.op_Implicit("##UnifiedGlamourManagerSelectedTab"));
		int num = ((ImGuiStoragePtr)(ref stateStorage)).GetInt(iD, 0);
		ImGuiStylePtr style = ImGui.GetStyle();
		float x = ((ImGuiStylePtr)(ref style)).ItemSpacing.X;
		Vector2 vector = new Vector2((ImGui.GetContentRegionAvail().X - x) * 0.5f, ImGui.GetFrameHeight());
		StyleDisposable val = ImRaii.PushStyle((ImGuiStyleVar)23, new Vector2(0.5f, 0.5f), true);
		try
		{
			if (ImGui.Selectable(ImU8String.op_Implicit(LanguageManager.Get("UnifiedGlamourManagerTitle")), num == 0, (ImGuiSelectableFlags)0, vector))
			{
				((ImGuiStoragePtr)(ref stateStorage)).SetInt(iD, 0);
			}
			ImGui.SameLine();
			if (ImGui.Selectable(ImU8String.op_Implicit(LanguageManager.Get("UnifiedGlamourManager-GlamourPresetManagerTitle")), num == 1, (ImGuiSelectableFlags)0, vector))
			{
				((ImGuiStoragePtr)(ref stateStorage)).SetInt(iD, 1);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		ImGui.Separator();
		if (((ImGuiStoragePtr)(ref stateStorage)).GetInt(iD, 0) == 0)
		{
			DrawTopBar();
			DrawMainLayout();
			DrawConfirmPopups();
		}
		else
		{
			DrawStatusBar();
			DrawPresetSearchAndFilterBar();
			ImGui.Separator();
			DrawPresetMainLayout();
			DrawMissingApplyItemsPopup();
		}
	}

	private static void DrawSectionTitle(string text)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TextColored(ref TitleColor, ImU8String.op_Implicit(text));
		ImGui.Separator();
		ImGui.Spacing();
	}

	private static void DrawItemIcon(uint iconID, float size)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (iconID == 0)
		{
			ImGui.Dummy(new Vector2(size, size));
			return;
		}
		IDalamudTextureWrap gameIcon = ImageHelper.GetGameIcon(iconID);
		if (gameIcon != null)
		{
			ImGui.Image(gameIcon.Handle, new Vector2(size, size));
		}
		else
		{
			ImGui.Dummy(new Vector2(size, size));
		}
	}

	private static void DrawItemBackground(ImDrawListPtr drawList, Vector2 min, Vector2 max, bool selected, bool favorite, bool hovered)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		ImGuiStylePtr style = ImGui.GetStyle();
		float frameRounding = ((ImGuiStylePtr)(ref style)).FrameRounding;
		((ImDrawListPtr)(ref drawList)).AddRectFilled(min, max, ImGui.ColorConvertFloat4ToU32(GetCardBackgroundColor(selected, favorite, hovered)), frameRounding);
		((ImDrawListPtr)(ref drawList)).AddRect(min, max, ImGui.ColorConvertFloat4ToU32(GetCardBorderColor(selected, favorite)), frameRounding, (ImDrawFlags)0, selected ? (2f * ImGuiHelpers.GlobalScale) : (1f * ImGuiHelpers.GlobalScale));
	}

	private void DrawTopBar()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		ImU8String val = ImU8String.op_Implicit("##TopBar");
		float num = ImGui.GetTextLineHeightWithSpacing() + ImGui.GetFrameHeightWithSpacing();
		ImGuiStylePtr style = ImGui.GetStyle();
		ChildDisposable val2 = ImRaii.Child(val, new Vector2(0f, num + ((ImGuiStylePtr)(ref style)).WindowPadding.Y * 2f), true, (ImGuiWindowFlags)8);
		try
		{
			if (!(!val2))
			{
				ImGui.AlignTextToFramePadding();
				DrawStatusBar();
				if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("Refresh")), default(Vector2)))
				{
					StartRefreshAll();
				}
				ImGui.SameLine();
				ImGui.SetNextItemWidth(MathF.Min(ImGui.GetContentRegionAvail().X * 0.35f, ImGui.GetContentRegionAvail().X));
				if (ImGui.InputTextWithHint(ImU8String.op_Implicit("##UnifiedItemSearch"), ImU8String.op_Implicit(LanguageManager.Get("Search")), ref searchText, 128, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null))
				{
					MarkFilteredItemsDirty();
				}
				ImGui.SameLine();
				if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("UnifiedGlamourManager-CurrentTargetSlotOnly", string.Empty).TrimEnd(new char[2] { ':', ' ' })), ref filterByCurrentPlateSlot))
				{
					MarkFilteredItemsDirty(clearJobCache: false, clearPlateSlotCache: true);
				}
			}
		}
		finally
		{
			((ChildDisposable)(ref val2)).Dispose();
		}
	}

	private void DrawMainLayout()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		Vector2 contentRegionAvail = ImGui.GetContentRegionAvail();
		if (contentRegionAvail.X <= 0f || contentRegionAvail.Y <= 0f)
		{
			return;
		}
		ImGuiTableFlags val = (ImGuiTableFlags)25089;
		TableDisposable val2 = ImRaii.Table(ImU8String.op_Implicit("##UnifiedMainTable"), 3, val, contentRegionAvail);
		try
		{
			if (!(!val2))
			{
				ImGui.TableSetupColumn(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(14370u)), (ImGuiTableColumnFlags)4, 0.5f, 0u);
				ImGui.TableSetupColumn(ImU8String.op_Implicit(LanguageManager.Get("UnifiedGlamourManager-FilteredResult")), (ImGuiTableColumnFlags)4, 1.2f, 0u);
				ImGui.TableSetupColumn(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(2154u)), (ImGuiTableColumnFlags)4, 0.7f, 0u);
				ImGui.TableNextRow();
				ImGui.TableSetColumnIndex(0);
				DrawSidebar();
				ImGui.TableSetColumnIndex(1);
				DrawItemList();
				ImGui.TableSetColumnIndex(2);
				DrawSelectedPanel();
			}
		}
		finally
		{
			((TableDisposable)(ref val2)).Dispose();
		}
	}

	private void DrawSidebar()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		ChildDisposable val = ImRaii.Child(ImU8String.op_Implicit("##FilterPanel"), Vector2.Zero, true);
		try
		{
			if (!(!val))
			{
				DrawSectionTitle(LuminaWrapper.GetAddonText(14370u));
				DrawSourceFilter();
				DrawSortFilter();
				DrawLevelFilter();
				DrawJobFilter();
				DrawSetRelationFilter();
				DrawResetFilterButton();
			}
		}
		finally
		{
			((ChildDisposable)(ref val)).Dispose();
		}
	}

	private void DrawSourceFilter()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TextDisabled(ImU8String.op_Implicit(LanguageManager.Get("UnifiedGlamourManager-GlamourSource")));
		float x = ImGui.GetContentRegionAvail().X;
		ImGuiStylePtr style = ImGui.GetStyle();
		float x2 = MathF.Max(1f, (x - ((ImGuiStylePtr)(ref style)).ItemSpacing.X) * 0.5f);
		Vector2 vector = new Vector2(x2, 0f);
		for (int i = 0; i < SourceFilters.Length; i++)
		{
			if (i % 2 == 1)
			{
				ImGui.SameLine();
			}
			SourceFilter sourceFilter = SourceFilters[i];
			ColorDisposable val = ImRaii.PushColor((ImGuiCol)21, ButtonActiveColor, this.sourceFilter == sourceFilter);
			try
			{
				ImU8String val2 = new ImU8String(8, 2);
				((ImU8String)(ref val2)).AppendFormatted<string>(GetSourceFilterLabel(sourceFilter));
				((ImU8String)(ref val2)).AppendLiteral("##Source");
				((ImU8String)(ref val2)).AppendFormatted<SourceFilter>(sourceFilter);
				if (ImGui.Button(val2, vector))
				{
					this.sourceFilter = sourceFilter;
					MarkFilteredItemsDirty();
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		ImGui.Spacing();
	}

	private void DrawSortFilter()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TextDisabled(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(12170u)));
		int num = (int)sortMode;
		ImGui.SetNextItemWidth(-1f);
		if (ImGui.Combo(ImU8String.op_Implicit("##SortMode"), ref num, (ReadOnlySpan<string>)SortModeNames, SortModeNames.Length))
		{
			sortMode = (SortMode)num;
			MarkFilteredItemsDirty();
		}
		ImGui.Spacing();
	}

	private void DrawLevelFilter()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TextDisabled(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(7873u)));
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("Enable")), ref enableLevelFilter))
		{
			MarkFilteredItemsDirty();
		}
		DisabledDisposable val = ImRaii.Disabled(!enableLevelFilter);
		try
		{
			float x = ImGui.GetContentRegionAvail().X;
			ImGuiStylePtr style = ImGui.GetStyle();
			float nextItemWidth = MathF.Max(1f, (x - ((ImGuiStylePtr)(ref style)).ItemSpacing.X) * 0.5f);
			int num = minEquipLevel;
			int num2 = maxEquipLevel;
			ImGui.SetNextItemWidth(nextItemWidth);
			ImGui.InputInt(ImU8String.op_Implicit("##MinLevel"), ref minEquipLevel, 0, 0, default(ImU8String), (ImGuiInputTextFlags)0);
			ImGui.SameLine();
			ImGui.SetNextItemWidth(-1f);
			ImGui.InputInt(ImU8String.op_Implicit("##MaxLevel"), ref maxEquipLevel, 0, 0, default(ImU8String), (ImGuiInputTextFlags)0);
			minEquipLevel = Math.Clamp(minEquipLevel, 1, 999);
			maxEquipLevel = Math.Clamp(maxEquipLevel, 1, 999);
			if (minEquipLevel > maxEquipLevel)
			{
				int num3 = maxEquipLevel;
				int num4 = minEquipLevel;
				minEquipLevel = num3;
				maxEquipLevel = num4;
			}
			if (num != minEquipLevel || num2 != maxEquipLevel)
			{
				MarkFilteredItemsDirty();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		ImGui.Spacing();
	}

	private void DrawJobFilter()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TextDisabled(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(294u)));
		ImGui.SetNextItemWidth(-1f);
		if (ImGui.Combo(ImU8String.op_Implicit("##JobFilter"), ref selectedJobFilterIndex, (ReadOnlySpan<string>)JobFilterNames, JobFilterNames.Length))
		{
			MarkFilteredItemsDirty(clearJobCache: true);
		}
		ImGui.Spacing();
	}

	private void DrawSetRelationFilter()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TextDisabled(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(15624u)));
		string[] array = CreateSetRelationFilterNames();
		int num = Math.Clamp((int)setRelationFilter, 0, array.Length - 1);
		ImGui.SetNextItemWidth(-1f);
		if (ImGui.Combo(ImU8String.op_Implicit("##SetRelationFilter"), ref num, (ReadOnlySpan<string>)array, array.Length))
		{
			setRelationFilter = (SetRelationFilter)num;
			MarkFilteredItemsDirty();
		}
		ImGui.Spacing();
	}

	private void DrawResetFilterButton()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		ImGui.Separator();
		ImGui.Spacing();
		if (ImGui.Button(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(329u)), new Vector2(-1f, 0f)))
		{
			ResetFilters();
		}
	}

	private void DrawItemList()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		EnsureFilteredItems();
		ChildDisposable val = ImRaii.Child(ImU8String.op_Implicit("##ListPanel"), Vector2.Zero, true);
		try
		{
			if (!val)
			{
				return;
			}
			ImGui.TextColored(ref TitleColor, ImU8String.op_Implicit(LanguageManager.Get("UnifiedGlamourManager-FilteredResult")));
			ImGui.Separator();
			float x = ImGui.GetContentRegionAvail().X;
			ImGuiStylePtr style = ImGui.GetStyle();
			Vector2 vector = new Vector2((x - ((ImGuiStylePtr)(ref style)).ItemSpacing.X) * 0.5f, ImGui.GetFrameHeight());
			(string, bool)[] array = new(string, bool)[2]
			{
				(LanguageManager.Get("List"), false),
				(LanguageManager.Get("Icon"), true)
			};
			for (int i = 0; i < array.Length; i++)
			{
				(string, bool) tuple = array[i];
				string item = tuple.Item1;
				bool item2 = tuple.Item2;
				bool flag = config.UseGridView == item2;
				Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
				ImU8String val2 = new ImU8String(10, 1);
				((ImU8String)(ref val2)).AppendLiteral("##ViewMode");
				((ImU8String)(ref val2)).AppendFormatted<bool>(item2);
				if (ImGui.InvisibleButton(val2, vector, (ImGuiButtonFlags)0))
				{
					config.UseGridView = item2;
				}
				ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
				Vector2 vector2 = cursorScreenPos + new Vector2((vector.X - ImGui.CalcTextSize(ImU8String.op_Implicit(item), false, -1f).X) * 0.5f, (vector.Y - ImGui.CalcTextSize(ImU8String.op_Implicit(item), false, -1f).Y) * 0.5f);
				Vector4 vector3;
				if (!flag)
				{
					style = ImGui.GetStyle();
					vector3 = ((ImGuiStylePtr)(ref style)).Colors[1];
				}
				else
				{
					vector3 = ButtonActiveColor;
				}
				((ImDrawListPtr)(ref windowDrawList)).AddText(vector2, ImGui.GetColorU32(vector3), ImU8String.op_Implicit(item));
				if (flag)
				{
					windowDrawList = ImGui.GetWindowDrawList();
					((ImDrawListPtr)(ref windowDrawList)).AddLine(cursorScreenPos + new Vector2(vector.X * 0.25f, vector.Y - 1f), cursorScreenPos + new Vector2(vector.X * 0.75f, vector.Y - 1f), ImGui.GetColorU32(ButtonActiveColor), 2f);
				}
				if (!item2)
				{
					ImGui.SameLine();
				}
			}
			ImGui.Separator();
			ImGui.Spacing();
			bool flag2 = sourceFilter == SourceFilter.Favorite;
			float num;
			if (!flag2)
			{
				num = 0f;
			}
			else
			{
				float num2 = ImGui.GetFrameHeightWithSpacing() + ImGui.GetFrameHeightWithSpacing();
				style = ImGui.GetStyle();
				num = num2 + ((ImGuiStylePtr)(ref style)).ItemSpacing.Y;
			}
			float num3 = num;
			ChildDisposable val3 = ImRaii.Child(ImU8String.op_Implicit("##UnifiedItemList"), flag2 ? new Vector2(0f, 0f - num3) : Vector2.Zero, false);
			try
			{
				if (isRefreshingItems)
				{
					ImGui.TextDisabled(ImU8String.op_Implicit(LanguageManager.Get("Loading")));
				}
				else if (filteredItems.Count == 0)
				{
					ImGui.TextDisabled(ImU8String.op_Implicit(LanguageManager.Get("UnifiedGlamourManager-NoSearchResult")));
				}
				else if (config.UseGridView)
				{
					DrawItemGrid(filteredItems);
				}
				else
				{
					DrawItemCardsVirtualized(filteredItems);
				}
			}
			finally
			{
				((ChildDisposable)(ref val3)).Dispose();
			}
			if (!flag2)
			{
				return;
			}
			ImGui.Separator();
			ImU8String val4 = new ImU8String(2, 2);
			((ImU8String)(ref val4)).AppendFormatted<string>(LanguageManager.Get("UnifiedGlamourManager-FilteredFavoriteCount"));
			((ImU8String)(ref val4)).AppendLiteral(": ");
			((ImU8String)(ref val4)).AppendFormatted<int>(filteredItems.Select((UnifiedItem unifiedItem) => unifiedItem.ItemID).Distinct().Count());
			ImGui.TextColored(ref GoldColor, val4);
			DisabledDisposable val5 = ImRaii.Disabled(filteredItems.Count == 0);
			try
			{
				if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("UnifiedGlamourManager-ClearFavorites")), new Vector2(-1f, 0f)))
				{
					requestClearFavoritesConfirm = true;
				}
			}
			finally
			{
				((IDisposable)val5)?.Dispose();
			}
		}
		finally
		{
			((ChildDisposable)(ref val)).Dispose();
		}
	}

	private void DrawItemCardsVirtualized(IReadOnlyList<UnifiedItem> filtered)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (filtered.Count != 0)
		{
			float cardHeight = CardHeight;
			ImGuiStylePtr style = ImGui.GetStyle();
			float num = cardHeight + ((ImGuiStylePtr)(ref style)).ItemSpacing.Y;
			Vector2 cursorPos = ImGui.GetCursorPos();
			float scrollY = ImGui.GetScrollY();
			float windowHeight = ImGui.GetWindowHeight();
			int num2 = Math.Max(0, (int)MathF.Floor(scrollY / num) - 3);
			int num3 = Math.Min(filtered.Count - 1, (int)MathF.Ceiling((scrollY + windowHeight) / num) + 3);
			if (num2 > 0)
			{
				ImGui.Dummy(new Vector2(0f, (float)num2 * num));
			}
			for (int i = num2; i <= num3; i++)
			{
				DrawItemCard(filtered[i]);
			}
			float num4 = (float)(num3 + 1) * num;
			float num5 = (float)filtered.Count * num;
			float num6 = num5 - num4;
			if (num6 > 0f)
			{
				ImGui.Dummy(new Vector2(0f, num6));
			}
			if (ImGui.GetCursorPosY() < cursorPos.Y + num5)
			{
				ImGui.SetCursorPosY(cursorPos.Y + num5);
			}
		}
	}

	private void DrawItemCard(UnifiedItem item)
	{
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		bool favorite = IsFavorite(item.ItemID);
		bool selected = selectedItem == item;
		float x = ImGui.GetContentRegionAvail().X;
		float num = ImGui.GetFrameHeight() * 1.6f;
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(3, 4);
		((ImU8String)(ref val)).AppendFormatted<uint>(item.ItemID);
		((ImU8String)(ref val)).AppendLiteral("_");
		((ImU8String)(ref val)).AppendFormatted<uint>(item.PrismBoxIndex);
		((ImU8String)(ref val)).AppendLiteral("_");
		((ImU8String)(ref val)).AppendFormatted<bool>(item.IsSetPart);
		((ImU8String)(ref val)).AppendLiteral("_");
		((ImU8String)(ref val)).AppendFormatted<uint>(item.ParentSetItemID);
		IdDisposable val2 = ImRaii.PushId(val, true);
		try
		{
			ColorDisposable val3 = ImRaii.PushColor((ImGuiCol)3, NormalCardColor, true);
			try
			{
				ColorDisposable val4 = ImRaii.PushColor((ImGuiCol)5, GetCardBorderColor(selected, favorite), true);
				try
				{
					ChildDisposable val5 = ImRaii.Child(ImU8String.op_Implicit("##ItemCard"), new Vector2(x, CardHeight), true, (ImGuiWindowFlags)24);
					try
					{
						if (!(!val5))
						{
							bool flag = ImGui.IsWindowHovered((ImGuiHoveredFlags)32);
							ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
							Vector2 windowPos = ImGui.GetWindowPos();
							Vector2 max = windowPos + ImGui.GetWindowSize();
							DrawItemBackground(windowDrawList, windowPos, max, selected, favorite, flag);
							ImGuiStylePtr style = ImGui.GetStyle();
							ImGui.SetCursorPos(((ImGuiStylePtr)(ref style)).WindowPadding);
							DrawFavoriteButton(item, favorite, num);
							ImGui.SameLine();
							DrawItemIcon(item.IconID, num);
							ImGui.SameLine();
							DrawItemCardInfo(item, selected, favorite);
							if (flag && !ImGui.IsAnyItemActive())
							{
								HandleItemClick(item);
							}
							style = ImGui.GetStyle();
							ImGui.Dummy(new Vector2(0f, ((ImGuiStylePtr)(ref style)).ItemSpacing.Y));
						}
					}
					finally
					{
						((ChildDisposable)(ref val5)).Dispose();
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

	private void DrawFavoriteButton(UnifiedItem item, bool favorite, float iconSize)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		ColorDisposable val = ImRaii.PushColor((ImGuiCol)21, NormalCardColor, true).Push((ImGuiCol)22, FrameBGColor, true).Push((ImGuiCol)23, ButtonAccentColor, true)
			.Push((ImGuiCol)0, favorite ? GoldColor : StarOffColor, true);
		try
		{
			if (ImGui.Button(ImU8String.op_Implicit(favorite ? "★" : "☆"), new Vector2(ImGui.GetFrameHeight(), iconSize)))
			{
				ToggleFavorite(item);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private static void DrawItemCardInfo(UnifiedItem item, bool selected, bool favorite)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		GroupDisposable val = ImRaii.Group();
		try
		{
			Vector4 vector = (selected ? SelectedBorderColor : ((favorite || !item.IsSetPart) ? KnownColor.White.ToVector4() : SoftAccentColor));
			ColorDisposable val2 = ImRaii.PushColor((ImGuiCol)0, vector, true);
			try
			{
				ImGui.TextUnformatted(ImU8String.op_Implicit(item.Name));
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
			DrawItemStains(item, horizontal: true);
		}
		finally
		{
			((GroupDisposable)(ref val)).Dispose();
		}
	}

	private void DrawItemGrid(IReadOnlyList<UnifiedItem> filtered)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		if (filtered.Count == 0)
		{
			return;
		}
		ImGuiStylePtr style = ImGui.GetStyle();
		float x = ((ImGuiStylePtr)(ref style)).ItemSpacing.X;
		float num = ImGui.GetFrameHeight() * 1.8f;
		float max = ImGui.GetFrameHeight() * 2.3f;
		float x2 = ImGui.GetContentRegionAvail().X;
		int num2 = Math.Max(1, (int)MathF.Floor((x2 + x) / (num + x)));
		float value = MathF.Floor((x2 - x * (float)(num2 - 1)) / (float)num2);
		value = Math.Clamp(value, num, max);
		float x3 = ImGui.GetFrameHeight() * 1.3f;
		float num3 = value;
		style = ImGui.GetStyle();
		float iconSize = MathF.Max(x3, num3 - ((ImGuiStylePtr)(ref style)).FramePadding.X);
		Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
		ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
		int num4 = (filtered.Count + num2 - 1) / num2;
		float num5 = value + x;
		float y = (float)num4 * num5;
		float scrollY = ImGui.GetScrollY();
		float windowHeight = ImGui.GetWindowHeight();
		int num6 = Math.Max(0, (int)MathF.Floor(scrollY / num5) - 2);
		int num7 = Math.Min(num4 - 1, (int)MathF.Ceiling((scrollY + windowHeight) / num5) + 2);
		ImGui.Dummy(new Vector2(0f, y));
		for (int i = num6; i <= num7; i++)
		{
			for (int j = 0; j < num2; j++)
			{
				int num8 = i * num2 + j;
				if (num8 >= 0 && num8 < filtered.Count)
				{
					UnifiedItem item = filtered[num8];
					Vector2 vector = new Vector2(cursorScreenPos.X + (float)j * (value + x), cursorScreenPos.Y + (float)i * num5);
					ImGui.SetCursorScreenPos(vector);
					DrawGridCell(item, num8, vector, value, iconSize, windowDrawList);
				}
			}
		}
		ImGui.SetCursorScreenPos(cursorScreenPos + new Vector2(0f, y));
	}

	private void DrawGridCell(UnifiedItem item, int index, Vector2 pos, float cellSize, float iconSize, ImDrawListPtr drawList)
	{
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(9, 5);
		((ImU8String)(ref val)).AppendLiteral("Grid_");
		((ImU8String)(ref val)).AppendFormatted<uint>(item.ItemID);
		((ImU8String)(ref val)).AppendLiteral("_");
		((ImU8String)(ref val)).AppendFormatted<uint>(item.PrismBoxIndex);
		((ImU8String)(ref val)).AppendLiteral("_");
		((ImU8String)(ref val)).AppendFormatted<bool>(item.IsSetPart);
		((ImU8String)(ref val)).AppendLiteral("_");
		((ImU8String)(ref val)).AppendFormatted<uint>(item.ParentSetItemID);
		((ImU8String)(ref val)).AppendLiteral("_");
		((ImU8String)(ref val)).AppendFormatted<int>(index);
		IdDisposable val2 = ImRaii.PushId(val, true);
		try
		{
			ImGui.InvisibleButton(ImU8String.op_Implicit("##GridCell"), new Vector2(cellSize, cellSize), (ImGuiButtonFlags)0);
			bool flag = ImGui.IsItemHovered();
			bool selected = selectedItem == item;
			bool flag2 = IsFavorite(item.ItemID);
			DrawItemBackground(drawList, pos, pos + new Vector2(cellSize, cellSize), selected, flag2, flag);
			float num = (cellSize - iconSize) * 0.5f;
			ImGui.SetCursorScreenPos(pos + new Vector2(num, num));
			DrawItemIcon(item.IconID, iconSize);
			if (flag2)
			{
				Vector2 vector = ImGui.CalcTextSize(ImU8String.op_Implicit("★"), false, -1f);
				float num2 = cellSize - vector.X;
				ImGuiStylePtr style = ImGui.GetStyle();
				float x = num2 - ((ImGuiStylePtr)(ref style)).FramePadding.X;
				style = ImGui.GetStyle();
				((ImDrawListPtr)(ref drawList)).AddText(pos + new Vector2(x, ((ImGuiStylePtr)(ref style)).FramePadding.Y), ImGui.ColorConvertFloat4ToU32(GoldColor), ImU8String.op_Implicit("★"));
			}
			if (flag)
			{
				HandleItemClick(item);
				DrawGridTooltip(item);
			}
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	private void DrawSelectedPanel()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		ChildDisposable val = ImRaii.Child(ImU8String.op_Implicit("##SelectedPanel"), Vector2.Zero, true);
		try
		{
			if (!val)
			{
				return;
			}
			DrawSectionTitle(LuminaWrapper.GetAddonText(2154u));
			if (selectedItem == null)
			{
				ImGui.TextDisabled(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(4764u)));
				return;
			}
			UnifiedItem unifiedItem = selectedItem;
			DrawSelectedItemHeader(unifiedItem);
			ImU8String val2 = new ImU8String(2, 1);
			((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get("UnifiedGlamourManager-GlamourTargetSlot"));
			((ImU8String)(ref val2)).AppendLiteral(": ");
			ImGui.TextColored(ref SoftAccentColor, val2);
			ImGui.TextDisabled(ImU8String.op_Implicit(unifiedItem.AvailableSlotsLabel));
			ImGui.Spacing();
			if (unifiedItem.IsSetPart)
			{
				ImU8String val3 = new ImU8String(2, 1);
				((ImU8String)(ref val3)).AppendFormatted<string>(LuminaWrapper.GetAddonText(15624u));
				((ImU8String)(ref val3)).AppendLiteral(": ");
				ImGui.TextColored(ref SoftAccentColor, val3);
				ImGui.TextDisabled(ImU8String.op_Implicit(unifiedItem.ParentSetName));
				ImGui.Spacing();
			}
			if (unifiedItem.IsSetContainer)
			{
				ColorDisposable val4 = ImRaii.PushColor((ImGuiCol)0, ErrorColor, true);
				try
				{
					ImGui.TextWrapped(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(15624u)));
				}
				finally
				{
					((IDisposable)val4)?.Dispose();
				}
				ImGui.Spacing();
			}
			ImGui.Separator();
			ImGui.Spacing();
			if (ImGui.Button(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(159u)), new Vector2(-1f, 0f)))
			{
				ImGui.SetClipboardText(ImU8String.op_Implicit(unifiedItem.Name));
			}
			if (ImGui.Button(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(102590u)), new Vector2(-1f, 0f)))
			{
				selectedItem = null;
			}
		}
		finally
		{
			((ChildDisposable)(ref val)).Dispose();
		}
	}

	private static void DrawSelectedItemHeader(UnifiedItem item)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		DrawItemIcon(item.IconID, ImGui.GetFrameHeight());
		ImGui.SameLine();
		GroupDisposable val = ImRaii.Group();
		try
		{
			ImGui.TextColored(ref SoftAccentColor, ImU8String.op_Implicit(item.Name));
			ImU8String val2 = new ImU8String(2, 2);
			((ImU8String)(ref val2)).AppendFormatted<string>(LuminaWrapper.GetAddonText(335u));
			((ImU8String)(ref val2)).AppendLiteral(": ");
			((ImU8String)(ref val2)).AppendFormatted<uint>(item.LevelEquip);
			ImGui.TextDisabled(val2);
			ImU8String val3 = new ImU8String(2, 2);
			((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get("UnifiedGlamourManager-GlamourSource"));
			((ImU8String)(ref val3)).AppendLiteral(": ");
			((ImU8String)(ref val3)).AppendFormatted<string>(item.SourceLabel);
			ImGui.TextDisabled(val3);
			DrawItemStains(item);
		}
		finally
		{
			((GroupDisposable)(ref val)).Dispose();
		}
		ImGui.Spacing();
		ImGui.Separator();
		ImGui.Spacing();
	}

	private void DrawConfirmPopups()
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		if (requestClearFavoritesConfirm)
		{
			favoritesConfirmPopupOpen = true;
			requestClearFavoritesConfirm = false;
		}
		using ImGuiOm.ModalPopupDisposable modalPopupDisposable = ImGuiOm.PopupModal(LanguageManager.Get("Clear") + "###ClearFavoritesConfirm", ref favoritesConfirmPopupOpen, (ImGuiWindowFlags)0);
		if (!modalPopupDisposable)
		{
			return;
		}
		ImGui.TextColored(ref ErrorColor, ImU8String.op_Implicit(LanguageManager.Get("UnifiedGlamourManager-ClearFavoriteConfirm")));
		ImGui.Spacing();
		if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("Confirm")), default(Vector2)))
		{
			HashSet<uint> ids = filteredItems.Select((UnifiedItem x) => x.ItemID).ToHashSet();
			config.Favorites.RemoveAll((SavedItem x) => ids.Contains(x.ItemID));
			SaveConfig();
			favoritesConfirmPopupOpen = false;
		}
		ImGui.SameLine();
		if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("Cancel")), default(Vector2)))
		{
			favoritesConfirmPopupOpen = false;
		}
	}

	private static void DrawGridTooltip(UnifiedItem item)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		TooltipDisposable val = ImRaii.Tooltip();
		try
		{
			ImGui.TextColored(ref TitleColor, ImU8String.op_Implicit(item.Name));
			DrawItemStains(item);
			ImGui.Separator();
			ImGui.TextDisabled(ImU8String.op_Implicit(LanguageManager.Get("UnifiedGlamourManager-RightClickFavoriteHint")));
		}
		finally
		{
			((TooltipDisposable)(ref val)).Dispose();
		}
	}

	private unsafe void DrawStatusBar()
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		MirageManager* manager;
		bool flag = TryGetLoadedMirageManager(out manager);
		bool flag2 = GetLoadedCabinet() != null;
		ImGui.AlignTextToFramePadding();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(1, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LuminaWrapper.GetAddonText(11910u));
		((ImU8String)(ref val)).AppendLiteral(":");
		ImGui.TextDisabled(val);
		ImGui.SameLine();
		Vector4 vector = (flag ? KnownColor.LimeGreen : KnownColor.Red).ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(flag ? LanguageManager.Get("Connected") : LanguageManager.Get("Disconnected")));
		ImGui.SameLine();
		ImU8String val2 = default(ImU8String);
		((ImU8String)(ref val2))._002Ector(1, 1);
		((ImU8String)(ref val2)).AppendFormatted<string>(LuminaWrapper.GetAddonText(12216u));
		((ImU8String)(ref val2)).AppendLiteral(":");
		ImGui.TextDisabled(val2);
		ImGui.SameLine();
		vector = (flag2 ? KnownColor.LimeGreen : KnownColor.Red).ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(flag2 ? LanguageManager.Get("Connected") : LanguageManager.Get("Disconnected")));
	}

	private void DrawPresetSearchAndFilterBar()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		TableDisposable val = ImRaii.Table(ImU8String.op_Implicit("##PresetSearchAndFilterBar"), 2, (ImGuiTableFlags)24576);
		try
		{
			if (!(!val))
			{
				ImGui.TableSetupColumn(ImU8String.op_Implicit("##Search"), (ImGuiTableColumnFlags)4, 0f, 0u);
				ImGui.TableSetupColumn(ImU8String.op_Implicit("##Filters"), (ImGuiTableColumnFlags)8, 0f, 0u);
				ImGui.TableNextColumn();
				ImGui.SetNextItemWidth(-1f);
				ImGui.InputTextWithHint(ImU8String.op_Implicit("##PresetSearch"), ImU8String.op_Implicit(LanguageManager.Get("Search")), ref presetSearch, 128, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
				ImGui.TableNextColumn();
				if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("UnifiedGlamourManager-Preset-OnlyCurrentRace")), ref config.OnlyCurrentRace))
				{
					config.Save(this);
				}
				ImGui.SameLine();
				if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("UnifiedGlamourManager-Preset-OnlyCurrentSex")), ref config.OnlyCurrentSex))
				{
					config.Save(this);
				}
			}
		}
		finally
		{
			((TableDisposable)(ref val)).Dispose();
		}
	}

	private void DrawPresetMainLayout()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		Vector2 contentRegionAvail = ImGui.GetContentRegionAvail();
		if (contentRegionAvail.X <= 0f || contentRegionAvail.Y <= 0f)
		{
			return;
		}
		TableDisposable val = ImRaii.Table(ImU8String.op_Implicit("##GlamourPresetMainLayout"), 2, (ImGuiTableFlags)1, contentRegionAvail);
		try
		{
			if (!(!val))
			{
				ImGui.TableSetupColumn(ImU8String.op_Implicit("##PresetList"), (ImGuiTableColumnFlags)4, 0.35f, 0u);
				ImGui.TableSetupColumn(ImU8String.op_Implicit("##PresetDetail"), (ImGuiTableColumnFlags)4, 0.65f, 0u);
				ImGui.TableNextColumn();
				DrawPresetList();
				ImGui.TableNextColumn();
				DrawPresetDetail();
			}
		}
		finally
		{
			((TableDisposable)(ref val)).Dispose();
		}
	}

	private unsafe void DrawPresetList()
	{
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		//IL_042f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0434: Unknown result type (might be due to invalid IL or missing references)
		//IL_0445: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Unknown result type (might be due to invalid IL or missing references)
		//IL_048a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0502: Unknown result type (might be due to invalid IL or missing references)
		//IL_052f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0534: Unknown result type (might be due to invalid IL or missing references)
		//IL_057b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0590: Unknown result type (might be due to invalid IL or missing references)
		//IL_0595: Unknown result type (might be due to invalid IL or missing references)
		BattleChara* localPlayer = Control.GetLocalPlayer();
		string race = GetRaceName(((CustomizeData)(&((DrawDataContainer)(&((BattleChara)localPlayer).DrawData)).CustomizeData)).Race, ((CustomizeData)(&((DrawDataContainer)(&((BattleChara)localPlayer).DrawData)).CustomizeData)).Sex);
		string sex = GetSexName(((CustomizeData)(&((DrawDataContainer)(&((BattleChara)localPlayer).DrawData)).CustomizeData)).Sex);
		string keyword = presetSearch.Trim();
		List<PlatePreset> list = (from x in config.Presets
			where (!config.OnlyCurrentRace || x.Race == race) && (!config.OnlyCurrentSex || x.Sex == sex) && (keyword.Length == 0 || x.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase) || x.Note.Contains(keyword, StringComparison.OrdinalIgnoreCase) || x.Items.Any(delegate(PlateItemInfo item)
			{
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				//IL_002f: Unknown result type (might be due to invalid IL or missing references)
				Item? row = LuminaGetter.GetRow<Item>(ItemUtil.GetBaseId(item.ItemID).Item1);
				if (row.HasValue)
				{
					Item valueOrDefault = row.GetValueOrDefault();
					return ((object)((Item)(ref valueOrDefault)).Name/*cast due to constrained. prefix*/).ToString().Contains(keyword, StringComparison.OrdinalIgnoreCase);
				}
				return false;
			}))
			orderby x.CreatedAt descending
			select x).ToList();
		ImGui.AlignTextToFramePadding();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(4, 3);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("List"));
		((ImU8String)(ref val)).AppendLiteral(" ");
		((ImU8String)(ref val)).AppendFormatted<int>(list.Count);
		((ImU8String)(ref val)).AppendLiteral(" / ");
		((ImU8String)(ref val)).AppendFormatted<int>(config.Presets.Count);
		ImGui.TextColored(ref TitleColor, val);
		ImGui.Separator();
		TableDisposable val2 = ImRaii.Table(ImU8String.op_Implicit("##PresetImportButtons"), 2, (ImGuiTableFlags)24576);
		try
		{
			if (TableDisposable.op_Implicit(val2))
			{
				ImGui.TableSetupColumn(ImU8String.op_Implicit("##Import"), (ImGuiTableColumnFlags)4, 1f, 0u);
				ImGui.TableSetupColumn(ImU8String.op_Implicit("##TryOnImport"), (ImGuiTableColumnFlags)4, 1f, 0u);
				ImGui.TableNextRow();
				ImGui.TableNextColumn();
				if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)62831, LanguageManager.Get("Import"), new Vector2(-1f, 0f)))
				{
					PlatePreset platePreset = ModuleBase.ImportFromClipboard<PlatePreset>();
					if (platePreset != null)
					{
						platePreset.Title = platePreset.Title.Trim();
						platePreset.CreatedAt = DateTime.Now;
						config.Presets.Add(platePreset);
						selectedPreset = platePreset;
						config.Save(this);
					}
				}
				ImGui.TableNextColumn();
				DisabledDisposable val3 = ImRaii.Disabled(base.TaskHelper.IsBusy || AgentTryon.Instance() == null || IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()));
				try
				{
					if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("UnifiedGlamourManager-Preset-TryOnImportedContent")), new Vector2(-1f, 0f)))
					{
						base.TaskHelper.Enqueue(() => StartTryOnPreset(ModuleBase.ImportFromClipboard<PlatePreset>()));
					}
				}
				finally
				{
					((IDisposable)val3)?.Dispose();
				}
			}
		}
		finally
		{
			((TableDisposable)(ref val2)).Dispose();
		}
		ImGui.Separator();
		ChildDisposable val4 = ImRaii.Child(ImU8String.op_Implicit("##PresetListChild"), Vector2.Zero, true);
		try
		{
			if (!val4)
			{
				return;
			}
			if (list.Count == 0)
			{
				ImGui.TextDisabled(ImU8String.op_Implicit(LanguageManager.Get("UnifiedGlamourManager-NoSearchResult")));
				return;
			}
			foreach (PlatePreset item in list)
			{
				ImU8String val5 = new ImU8String(1, 2);
				((ImU8String)(ref val5)).AppendFormatted<string>(item.Title);
				((ImU8String)(ref val5)).AppendLiteral("-");
				((ImU8String)(ref val5)).AppendFormatted<long>(item.CreatedAt.Ticks);
				IdDisposable val6 = ImRaii.PushId(val5, true);
				try
				{
					Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
					Vector2 vector = new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetTextLineHeightWithSpacing() * 3.5f);
					bool num = selectedPreset == item;
					if (ImGui.InvisibleButton(ImU8String.op_Implicit("##PresetItem"), vector, (ImGuiButtonFlags)0))
					{
						selectedPreset = item;
					}
					bool flag = ImGui.IsItemHovered();
					ImGuiOm.TooltipHover(LanguageManager.Get("UnifiedGlamourManager-Preset-DoubleClickEditHint"));
					if (flag && ImGui.IsMouseDoubleClicked((ImGuiMouseButton)0))
					{
						selectedPreset = item;
						editingPreset = item;
						editPresetTitleInput = item.Title;
						editPresetNoteInput = item.Note;
						ImGui.OpenPopup(ImU8String.op_Implicit("EditPresetPopup"), (ImGuiPopupFlags)0);
					}
					ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
					uint num2 = (num ? ImGui.GetColorU32((ImGuiCol)26) : (flag ? ImGui.GetColorU32((ImGuiCol)25) : ImGui.GetColorU32((ImGuiCol)7)));
					Vector2 vector2 = cursorScreenPos + vector;
					ImGuiStylePtr style = ImGui.GetStyle();
					((ImDrawListPtr)(ref windowDrawList)).AddRectFilled(cursorScreenPos, vector2, num2, ((ImGuiStylePtr)(ref style)).FrameRounding);
					Vector2 vector3 = cursorScreenPos + vector;
					uint colorU = ImGui.GetColorU32((ImGuiCol)5);
					style = ImGui.GetStyle();
					((ImDrawListPtr)(ref windowDrawList)).AddRect(cursorScreenPos, vector3, colorU, ((ImGuiStylePtr)(ref style)).FrameRounding);
					style = ImGui.GetStyle();
					Vector2 vector4 = cursorScreenPos + ((ImGuiStylePtr)(ref style)).FramePadding;
					((ImDrawListPtr)(ref windowDrawList)).AddText(vector4, ImGui.GetColorU32((ImGuiCol)0), ImU8String.op_Implicit(string.IsNullOrWhiteSpace(item.Title) ? LanguageManager.Get("UnifiedGlamourManager-Preset-UntitledPreset") : item.Title));
					Vector2 vector5 = vector4 + new Vector2(0f, ImGui.GetTextLineHeightWithSpacing());
					uint colorU2 = ImGui.GetColorU32((ImGuiCol)1);
					ImU8String val7 = new ImU8String(6, 3);
					((ImU8String)(ref val7)).AppendFormatted<string>(item.Race);
					((ImU8String)(ref val7)).AppendLiteral(" / ");
					((ImU8String)(ref val7)).AppendFormatted<string>(item.Sex);
					((ImU8String)(ref val7)).AppendLiteral(" / ");
					((ImU8String)(ref val7)).AppendFormatted<DateTime>(item.CreatedAt, "yyyy-MM-dd HH:mm");
					((ImDrawListPtr)(ref windowDrawList)).AddText(vector5, colorU2, val7);
					Vector2 vector6 = vector4 + new Vector2(0f, ImGui.GetTextLineHeightWithSpacing() * 2f);
					uint colorU3 = ImGui.GetColorU32((ImGuiCol)1);
					ImU8String val8 = new ImU8String(2, 2);
					((ImU8String)(ref val8)).AppendFormatted<string>(LanguageManager.Get("Note"));
					((ImU8String)(ref val8)).AppendLiteral(": ");
					((ImU8String)(ref val8)).AppendFormatted<string>(string.IsNullOrWhiteSpace(item.Note) ? LanguageManager.Get("None") : item.Note);
					((ImDrawListPtr)(ref windowDrawList)).AddText(vector6, colorU3, val8);
					float y = vector.Y;
					style = ImGui.GetStyle();
					ImGui.SetCursorScreenPos(cursorScreenPos + new Vector2(0f, y + ((ImGuiStylePtr)(ref style)).ItemSpacing.Y));
					DrawEditPresetPopup();
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

	private void DrawEditPresetPopup()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		PopupDisposable val = ImRaii.Popup(ImU8String.op_Implicit("EditPresetPopup"), (ImGuiWindowFlags)64);
		try
		{
			if (!val || editingPreset != selectedPreset)
			{
				return;
			}
			TableDisposable val2 = ImRaii.Table(ImU8String.op_Implicit("##EditPresetTable"), 2);
			try
			{
				if (TableDisposable.op_Implicit(val2))
				{
					ImGui.TableSetupColumn(ImU8String.op_Implicit("##Label"), (ImGuiTableColumnFlags)8, 0f, 0u);
					ImGui.TableSetupColumn(ImU8String.op_Implicit("##Input"), (ImGuiTableColumnFlags)8, 120f * Globals.GlobalUIScale, 0u);
					ImGui.TableNextRow();
					ImGui.TableNextColumn();
					ImGui.AlignTextToFramePadding();
					ImU8String val3 = new ImU8String(1, 1);
					((ImU8String)(ref val3)).AppendFormatted<string>(LuminaWrapper.GetAddonText(4534u));
					((ImU8String)(ref val3)).AppendLiteral(":");
					ImGui.TextUnformatted(val3);
					ImGui.TableNextColumn();
					ImGui.SetNextItemWidth(120f * Globals.GlobalUIScale);
					ImGui.InputTextWithHint(ImU8String.op_Implicit("##EditPresetTitle"), ImU8String.op_Implicit(LuminaWrapper.GetAddonText(4534u)), ref editPresetTitleInput, 40, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
					ImGui.TableNextRow();
					ImGui.TableNextColumn();
					ImGui.AlignTextToFramePadding();
					ImU8String val4 = new ImU8String(1, 1);
					((ImU8String)(ref val4)).AppendFormatted<string>(LanguageManager.Get("Note"));
					((ImU8String)(ref val4)).AppendLiteral(":");
					ImGui.TextUnformatted(val4);
					ImGui.TableNextColumn();
					ImGui.SetNextItemWidth(120f * Globals.GlobalUIScale);
					ImGui.InputTextMultiline(ImU8String.op_Implicit("##EditPresetNote"), ref editPresetNoteInput, 120, new Vector2(120f * Globals.GlobalUIScale, ImGui.GetTextLineHeightWithSpacing() * 3f), (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
				}
			}
			finally
			{
				((TableDisposable)(ref val2)).Dispose();
			}
			if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("Confirm")), default(Vector2)))
			{
				if (!string.IsNullOrWhiteSpace(editPresetTitleInput))
				{
					editingPreset.Title = editPresetTitleInput.Trim();
				}
				editingPreset.Note = editPresetNoteInput.Trim();
				selectedPreset = editingPreset;
				config.Save(this);
				ImGui.CloseCurrentPopup();
			}
			ImGui.SameLine();
			if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("Cancel")), default(Vector2)))
			{
				ImGui.CloseCurrentPopup();
			}
		}
		finally
		{
			((PopupDisposable)(ref val)).Dispose();
		}
	}

	private unsafe void DrawPresetDetail()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		ImGui.AlignTextToFramePadding();
		ImGui.TextColored(ref TitleColor, ImU8String.op_Implicit(LanguageManager.Get("UnifiedGlamourManager-Preset-PresetDetail")));
		ImGui.Separator();
		if (selectedPreset == null)
		{
			string addonText = LuminaWrapper.GetAddonText(1440u);
			Vector2 contentRegionAvail = ImGui.GetContentRegionAvail();
			Vector2 vector = ImGui.CalcTextSize(ImU8String.op_Implicit(addonText), false, -1f);
			ImGui.SetCursorPosY(ImGui.GetCursorPosY() + MathF.Max(0f, (contentRegionAvail.Y - vector.Y) * 0.5f));
			ImGui.SetCursorPosX(ImGui.GetCursorPosX() + MathF.Max(0f, (contentRegionAvail.X - vector.X) * 0.5f));
			ImGui.TextDisabled(ImU8String.op_Implicit(addonText));
			return;
		}
		DrawSavedContent();
		ImGui.Separator();
		TableDisposable val = ImRaii.Table(ImU8String.op_Implicit("##PresetActionButtons"), 2, (ImGuiTableFlags)24576);
		try
		{
			if (!val)
			{
				return;
			}
			ImGui.TableSetupColumn(ImU8String.op_Implicit("##Left"), (ImGuiTableColumnFlags)4, 1f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("##Right"), (ImGuiTableColumnFlags)4, 1f, 0u);
			ImGui.TableNextRow();
			ImGui.TableNextColumn();
			DisabledDisposable val2 = ImRaii.Disabled(base.TaskHelper.IsBusy || !TryGetReadyPlateEditor(out var agent) || ((AgentMiragePrismMiragePlateData)((AgentMiragePrismMiragePlate)agent).Data).OpenMode != 0);
			try
			{
				if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("Apply")), new Vector2(-1f, 0f)))
				{
					base.TaskHelper.Enqueue((Func<bool>)StartApplyPreset, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
			ImGui.TableNextColumn();
			val2 = ImRaii.Disabled(base.TaskHelper.IsBusy || AgentTryon.Instance() == null || IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()));
			try
			{
				if (ImGui.Button(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(2426u)), new Vector2(-1f, 0f)))
				{
					base.TaskHelper.Enqueue(() => StartTryOnPreset(selectedPreset));
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
			ImGui.TableNextRow();
			ImGui.TableNextColumn();
			if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)62830, LanguageManager.Get("Export"), new Vector2(-1f, 0f)))
			{
				ModuleBase.ExportToClipboard(selectedPreset);
			}
			ImGui.TableNextColumn();
			if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)62189, LanguageManager.Get("HoldCtrlToDelete"), new Vector2(-1f, 0f)) && ImGui.IsKeyDown((ImGuiKey)527))
			{
				config.Presets.Remove(selectedPreset);
				selectedPreset = null;
				config.Save(this);
			}
		}
		finally
		{
			((TableDisposable)(ref val)).Dispose();
		}
	}

	private void DrawSavedContent()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		if (selectedPreset == null)
		{
			return;
		}
		ChildDisposable val = ImRaii.Child(ImU8String.op_Implicit("##SavedContentChild"), new Vector2(0f, 0f - ImGui.GetFrameHeightWithSpacing() * 2.5f), true);
		try
		{
			if (!val)
			{
				return;
			}
			TableDisposable val2 = ImRaii.Table(ImU8String.op_Implicit("##SavedContentTable"), 3, (ImGuiTableFlags)33579200);
			try
			{
				if (!val2)
				{
					return;
				}
				ImGui.TableSetupColumn(ImU8String.op_Implicit(LanguageManager.Get("UnifiedGlamourManager-GlamourTargetSlot")), (ImGuiTableColumnFlags)8, ImGui.GetFrameHeight() * 3f, 0u);
				ImGui.TableSetupColumn(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(6942u)), (ImGuiTableColumnFlags)0, 0f, 0u);
				ImGui.TableSetupColumn(ImU8String.op_Implicit(LanguageManager.Get("Dye")), (ImGuiTableColumnFlags)4, 0.9f, 0u);
				ImGui.TableHeadersRow();
				foreach (PlateItemInfo item in from x in selectedPreset.Items
					where x.SlotIndex < PlateSlotAddonTextIDs.Length
					orderby x.SlotIndex
					select x)
				{
					Item valueOrDefault = LuminaGetter.GetRow<Item>(ItemUtil.GetBaseId(item.ItemID).Item1).GetValueOrDefault();
					Vector2 vector = new Vector2(ImGui.GetFrameHeight());
					ImGui.TableNextRow();
					ImGui.TableNextColumn();
					ImGui.AlignTextToFramePadding();
					ImGui.TextUnformatted(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(PlateSlotAddonTextIDs[item.SlotIndex])));
					ImGui.TableNextColumn();
					IDalamudTextureWrap gameIcon = ImageHelper.GetGameIcon(((Item)(ref valueOrDefault)).Icon);
					if (gameIcon != null)
					{
						ImGui.Image(gameIcon.Handle, vector);
					}
					ImGui.SameLine();
					ImGui.TextUnformatted(ImU8String.op_Implicit(((object)((Item)(ref valueOrDefault)).Name/*cast due to constrained. prefix*/).ToString()));
					ImGui.TableNextColumn();
					Stain valueOrDefault2 = LuminaGetter.GetRow<Stain>((uint)item.Stain0ID).GetValueOrDefault();
					Stain valueOrDefault3 = LuminaGetter.GetRow<Stain>((uint)item.Stain1ID).GetValueOrDefault();
					DrawStainLabel(15970u, valueOrDefault2);
					ImGui.SameLine();
					ImGui.TextDisabled(ImU8String.op_Implicit(" / "));
					ImGui.SameLine();
					DrawStainLabel(15971u, valueOrDefault3);
				}
			}
			finally
			{
				((TableDisposable)(ref val2)).Dispose();
			}
		}
		finally
		{
			((ChildDisposable)(ref val)).Dispose();
		}
	}

	private void DrawMissingApplyItemsPopup()
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		if (openMissingApplyItemsPopup)
		{
			missingApplyItemsPopupOpen = true;
			openMissingApplyItemsPopup = false;
		}
		using ImGuiOm.ModalPopupDisposable modalPopupDisposable = ImGuiOm.PopupModal("##MissingApplyItemsPopup", ref missingApplyItemsPopupOpen, (ImGuiWindowFlags)64);
		if (!modalPopupDisposable)
		{
			return;
		}
		ImGui.TextDisabled(ImU8String.op_Implicit(LanguageManager.Get("UnifiedGlamourManager-Preset-MissingItemsText")));
		ImGui.Separator();
		foreach (PlateItemInfo missingApplyItem in missingApplyItems)
		{
			ImU8String val = new ImU8String(2, 2);
			((ImU8String)(ref val)).AppendFormatted<string>(LuminaWrapper.GetAddonText(PlateSlotAddonTextIDs[missingApplyItem.SlotIndex]));
			((ImU8String)(ref val)).AppendLiteral(": ");
			((ImU8String)(ref val)).AppendFormatted<string>(LuminaWrapper.GetItemName(ItemUtil.GetBaseId(missingApplyItem.ItemID).Item1));
			ImGui.BulletText(val);
		}
		ImGui.Separator();
		if (ImGui.Button(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(1219u)), new Vector2(-1f, 0f)))
		{
			missingApplyItemsPopupOpen = false;
		}
	}

	private static Vector4 GetCardBackgroundColor(bool selected, bool favorite, bool hovered)
	{
		if (favorite)
		{
			if (!hovered)
			{
				return FavoriteCardColor;
			}
			return FavoriteCardHoverColor;
		}
		if (selected)
		{
			return SelectedColor;
		}
		if (!hovered)
		{
			return NormalCardColor;
		}
		return NormalCardHoverColor;
	}

	private static Vector4 GetCardBorderColor(bool selected, bool favorite)
	{
		if (!selected)
		{
			if (!favorite)
			{
				return MutedBorderColor;
			}
			return GoldColor;
		}
		return SelectedBorderColor;
	}

	private static string GetSourceFilterLabel(SourceFilter filter)
	{
		return filter switch
		{
			SourceFilter.Favorite => LanguageManager.Get("Favorite"), 
			SourceFilter.PrismBox => LuminaWrapper.GetAddonText(11910u), 
			SourceFilter.Cabinet => LuminaWrapper.GetAddonText(12216u), 
			_ => LanguageManager.Get("All"), 
		};
	}

	private void HandleItemClick(UnifiedItem item)
	{
		if (ImGui.IsMouseClicked((ImGuiMouseButton)0))
		{
			selectedItem = item;
			ApplySelectedItemToCurrentPlateSlot(item);
		}
		if (config.UseGridView && ImGui.IsMouseClicked((ImGuiMouseButton)1))
		{
			ToggleFavorite(item);
		}
	}

	private static void DrawStainLabel(uint labelAddonID, Stain stain)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(2, 2);
		((ImU8String)(ref val)).AppendFormatted<string>(LuminaWrapper.GetAddonText(labelAddonID));
		((ImU8String)(ref val)).AppendLiteral(": ");
		ReadOnlySeString name = ((Stain)(ref stain)).Name;
		((ImU8String)(ref val)).AppendFormatted<string>(((ReadOnlySeString)(ref name)).ExtractText());
		ImGui.TextDisabled(val);
		ImGui.SameLine();
		Vector4 vector = ColorHelpers.WithAlpha(((Stain)(ref stain)).Color.ReverseToVector4(), 1f);
		ImGui.TextColored(ref vector, ImU8String.op_Implicit("■"));
	}

	private static void DrawItemStains(UnifiedItem item, bool horizontal = false)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		Stain? stain = item.Stain0;
		if (stain.HasValue)
		{
			Stain valueOrDefault = stain.GetValueOrDefault();
			DrawStainLabel(15970u, valueOrDefault);
			if (horizontal && item.Stain1.HasValue)
			{
				ImGui.SameLine();
			}
		}
		stain = item.Stain1;
		if (stain.HasValue)
		{
			Stain valueOrDefault2 = stain.GetValueOrDefault();
			DrawStainLabel(15971u, valueOrDefault2);
		}
	}

	static UnifiedGlamourManager()
	{
		Vector4 selectedColor = KnownColor.MediumVioletRed.ToVector4();
		selectedColor.W = 0.65f;
		SelectedColor = selectedColor;
		selectedColor = KnownColor.PaleVioletRed.ToVector4();
		selectedColor.W = 0.4f;
		ButtonAccentColor = selectedColor;
		selectedColor = KnownColor.HotPink.ToVector4();
		selectedColor.W = 0.78f;
		ButtonActiveColor = selectedColor;
		SoftAccentColor = KnownColor.Plum.ToVector4();
		GoldColor = KnownColor.Gold.ToVector4();
		ErrorColor = KnownColor.Crimson.ToVector4();
		selectedColor = KnownColor.DimGray.ToVector4();
		selectedColor.W = 0.48f;
		FrameBGColor = selectedColor;
		selectedColor = KnownColor.Black.ToVector4();
		selectedColor.W = 0.34f;
		NormalCardColor = selectedColor;
		selectedColor = KnownColor.Maroon.ToVector4();
		selectedColor.W = 0.26f;
		NormalCardHoverColor = selectedColor;
		selectedColor = KnownColor.Gold.ToVector4();
		selectedColor.W = 0.4f;
		FavoriteCardColor = selectedColor;
		selectedColor = KnownColor.Goldenrod.ToVector4();
		selectedColor.W = 0.68f;
		FavoriteCardHoverColor = selectedColor;
		SelectedBorderColor = KnownColor.Khaki.ToVector4();
		MutedBorderColor = KnownColor.DarkGray.ToVector4();
		StarOffColor = KnownColor.Gray.ToVector4();
	}
}
