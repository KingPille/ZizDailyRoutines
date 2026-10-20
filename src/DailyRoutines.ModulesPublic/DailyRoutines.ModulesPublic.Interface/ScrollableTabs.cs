using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.System.Input;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Interop;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.Helpers;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic.Interface;

public class ScrollableTabs : ModuleBase
{
	private class Config : ModuleConfig
	{
		public bool Invert = true;
	}

	private unsafe delegate void AddonUpdateHandler(AtkUnitBase* unitBase);

	private Config config;

	private int wheelState;

	private readonly Dictionary<string, AddonUpdateHandler> uiHandlerMapping = new Dictionary<string, AddonUpdateHandler>();

	private readonly Dictionary<string, string> uiNameMapping = new Dictionary<string, string>();

	private const int NUM_ARMOURY_BOARD_TABS = 12;

	private const int NUM_INVENTORY_TABS = 5;

	private const int NUM_INVENTORY_LARGE_TABS = 4;

	private const int NUM_INVENTORY_EXPANSION_TABS = 2;

	private const int NUM_INVENTORY_RETAINER_TABS = 6;

	private const int NUM_INVENTORY_RETAINER_LARGE_TABS = 3;

	private const int NUM_BUDDY_TABS = 3;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("ScrollableTabsTitle"),
		Description = LanguageManager.Get("ScrollableTabsDescription"),
		Category = ModuleCategory.Interface,
		Author = new string[1] { "Cyf5119" }
	};

	private bool IsNext => wheelState == ((!config.Invert) ? 1 : (-1));

	private bool IsPrev => wheelState == (config.Invert ? 1 : (-1));

	private unsafe static AtkCollisionNode* IntersectingCollisionNode => ((AtkCollisionManager)(&((RaptureAtkModule)RaptureAtkModule.Instance()).AtkCollisionManager)).IntersectingCollisionNode;

	protected override void Init()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		InitUINameMapping();
		InitUIHandlerMapping();
		OmenServiceBase<FrameworkManager>.Instance().Reg(new OnUpdateDelegate(OnUpdate));
		unsafe void InitUIHandlerMapping()
		{
			uiHandlerMapping["ArmouryBoard"] = delegate(AtkUnitBase* addon)
			{
				UpdateArmouryBoard((AddonArmouryBoard*)addon);
			};
			uiHandlerMapping["Inventory"] = delegate(AtkUnitBase* addon)
			{
				UpdateInventory((AddonInventory*)addon);
			};
			uiHandlerMapping["InventoryEvent"] = delegate(AtkUnitBase* addon)
			{
				UpdateInventoryEvent((AddonInventoryEvent*)addon);
			};
			uiHandlerMapping["InventoryLarge"] = delegate(AtkUnitBase* addon)
			{
				UpdateInventoryLarge((AddonInventoryLarge*)addon);
			};
			uiHandlerMapping["InventoryExpansion"] = delegate(AtkUnitBase* addon)
			{
				UpdateInventoryExpansion((AddonInventoryExpansion*)addon);
			};
			uiHandlerMapping["InventoryRetainer"] = delegate(AtkUnitBase* addon)
			{
				UpdateInventoryRetainer((AddonInventoryRetainer*)addon);
			};
			uiHandlerMapping["InventoryRetainerLarge"] = delegate(AtkUnitBase* addon)
			{
				UpdateInventoryRetainerLarge((AddonInventoryRetainerLarge*)addon);
			};
			uiHandlerMapping["MinionNoteBook"] = delegate(AtkUnitBase* addon)
			{
				UpdateMountMinion((AddonMinionMountBase*)addon);
			};
			uiHandlerMapping["MountNoteBook"] = delegate(AtkUnitBase* addon)
			{
				UpdateMountMinion((AddonMinionMountBase*)addon);
			};
			uiHandlerMapping["FishGuide2"] = delegate(AtkUnitBase* addon)
			{
				UpdateTabController(addon, &((AddonFishGuide2)addon).TabController);
			};
			uiHandlerMapping["AdventureNoteBook"] = delegate(AtkUnitBase* addon)
			{
				UpdateTabController(addon, &((AddonAdventureNoteBook)addon).TabController);
			};
			uiHandlerMapping["OrnamentNoteBook"] = delegate(AtkUnitBase* addon)
			{
				UpdateTabController(addon, &((AddonOrnamentNoteBook)addon).TabController);
			};
			uiHandlerMapping["GSInfoCardList"] = delegate(AtkUnitBase* addon)
			{
				UpdateTabController(addon, &((AddonGSInfoCardList)addon).TabController);
			};
			uiHandlerMapping["GSInfoEditDeck"] = delegate(AtkUnitBase* addon)
			{
				UpdateTabController(addon, &((AddonGSInfoEditDeck)addon).TabController);
			};
			uiHandlerMapping["LovmPaletteEdit"] = delegate(AtkUnitBase* addon)
			{
				UpdateTabController(addon, &((AddonLovmPaletteEdit)addon).TabController);
			};
			uiHandlerMapping["AOZNotebook"] = delegate(AtkUnitBase* addon)
			{
				UpdateAOZNotebook((AddonAOZNotebook*)addon);
			};
			uiHandlerMapping["AetherCurrent"] = delegate(AtkUnitBase* addon)
			{
				UpdateAetherCurrent((AddonAetherCurrent*)addon);
			};
			uiHandlerMapping["FateProgress"] = delegate(AtkUnitBase* addon)
			{
				UpdateFateProgress((AddonFateProgress*)addon);
			};
			uiHandlerMapping["MYCWarResultNotebook"] = delegate(AtkUnitBase* addon)
			{
				UpdateFieldNotes((AddonMYCWarResultNotebook*)addon);
			};
			uiHandlerMapping["MJIMinionNoteBook"] = delegate(AtkUnitBase* addon)
			{
				UpdateMJIMinionNoteBook((AddonMJIMinionNoteBook*)addon);
			};
			uiHandlerMapping["InventoryBuddy"] = delegate(AtkUnitBase* addon)
			{
				UpdateInventoryBuddy((AddonInventoryBuddy*)addon);
			};
			uiHandlerMapping["InventoryBuddy2"] = delegate(AtkUnitBase* addon)
			{
				UpdateInventoryBuddy((AddonInventoryBuddy*)addon);
			};
			uiHandlerMapping["Buddy"] = delegate(AtkUnitBase* addon)
			{
				UpdateBuddy((AddonBuddy*)addon);
			};
			uiHandlerMapping["MiragePrismPrismBox"] = delegate(AtkUnitBase* addon)
			{
				UpdateMiragePrismPrismBox((AddonMiragePrismPrismBox*)addon);
			};
			uiHandlerMapping["Currency"] = delegate(AtkUnitBase* addon)
			{
				UpdateCurrency((AddonCurrency*)addon);
			};
			uiHandlerMapping["Character"] = delegate(AtkUnitBase* addon)
			{
				UpdateCharacter((AddonCharacter*)addon);
			};
			uiHandlerMapping["CharacterClass"] = HandleCharacterUI;
			uiHandlerMapping["CharacterRepute"] = HandleCharacterUI;
		}
		void InitUINameMapping()
		{
			string[] array = new string[28]
			{
				"AetherCurrent", "ArmouryBoard", "AOZNotebook", "OrnamentNoteBook", "MYCWarResultNotebook", "FishGuide2", "GSInfoCardList", "GSInfoEditDeck", "LovmPaletteEdit", "Inventory",
				"InventoryLarge", "InventoryExpansion", "MinionNoteBook", "MountNoteBook", "InventoryRetainer", "InventoryRetainerLarge", "FateProgress", "AdventureNoteBook", "MJIMinionNoteBook", "InventoryBuddy",
				"InventoryBuddy2", "Character", "CharacterClass", "CharacterRepute", "Buddy", "MiragePrismPrismBox", "InventoryEvent", "Currency"
			};
			foreach (string text in array)
			{
				uiNameMapping[text] = text;
			}
			array = new string[2] { "InventoryGrid", "InventoryGridCrystal" };
			foreach (string key in array)
			{
				uiNameMapping[key] = "Inventory";
			}
			array = new string[1] { "InventoryEventGrid" };
			foreach (string key2 in array)
			{
				uiNameMapping[key2] = "InventoryEvent";
			}
			uiNameMapping["InventoryCrystalGrid"] = "InventoryLarge";
			array = new string[5] { "InventoryEventGrid0", "InventoryEventGrid1", "InventoryEventGrid2", "InventoryGrid0", "InventoryGrid1" };
			foreach (string key3 in array)
			{
				uiNameMapping[key3] = "InventoryLarge";
			}
			array = new string[7] { "InventoryEventGrid0E", "InventoryEventGrid1E", "InventoryEventGrid2E", "InventoryGrid0E", "InventoryGrid1E", "InventoryGrid2E", "InventoryGrid3E" };
			foreach (string key4 in array)
			{
				uiNameMapping[key4] = "InventoryExpansion";
			}
			array = new string[2] { "RetainerGridCrystal", "RetainerGrid" };
			foreach (string key5 in array)
			{
				uiNameMapping[key5] = "InventoryRetainer";
			}
			array = new string[6] { "RetainerCrystalGrid", "RetainerGrid0", "RetainerGrid1", "RetainerGrid2", "RetainerGrid3", "RetainerGrid4" };
			foreach (string key6 in array)
			{
				uiNameMapping[key6] = "InventoryRetainerLarge";
			}
			array = new string[2] { "CharacterStatus", "CharacterProfile" };
			foreach (string key7 in array)
			{
				uiNameMapping[key7] = "Character";
			}
			array = new string[3] { "BuddyAction", "BuddySkill", "BuddyAppearance" };
			foreach (string key8 in array)
			{
				uiNameMapping[key8] = "Buddy";
			}
		}
	}

	protected override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		OmenServiceBase<FrameworkManager>.Instance().Unreg(OnUpdate);
	}

	protected override void ConfigUI()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("ScrollableTabs-Invert")), ref config.Invert))
		{
			config.Save(this);
		}
	}

	private unsafe void OnUpdate(IFramework _)
	{
		if (!DalamudServiceExtension.Instance<IClientState>().IsLoggedIn)
		{
			return;
		}
		wheelState = Math.Clamp(((CursorInputData)(&((UIInputData)UIInputData.Instance()).CursorInputs)).MouseWheel, -1, 1);
		if (wheelState == 0)
		{
			return;
		}
		if (config.Invert)
		{
			wheelState *= -1;
		}
		AtkUnitBase* intersectingAddon = ((AtkCollisionManager)(&((RaptureAtkModule)RaptureAtkModule.Instance()).AtkCollisionManager)).IntersectingAddon;
		if (intersectingAddon == null)
		{
			wheelState = 0;
			return;
		}
		string nameString = ((AtkUnitBase)intersectingAddon).NameString;
		if (string.IsNullOrEmpty(nameString))
		{
			wheelState = 0;
			return;
		}
		if (!uiNameMapping.TryGetValue(nameString, out string value))
		{
			wheelState = 0;
			return;
		}
		uint num = default(uint);
		if (nameString == "InventoryCrystalGrid" && DalamudServiceExtension.Instance<IGameConfig>().UiConfig.TryGet("ItemInventryWindowSizeType", ref num) && num == 2)
		{
			value = "InventoryExpansion";
		}
		if (!AddonHelper.TryGetByName(value, out var addonPtr))
		{
			wheelState = 0;
			return;
		}
		if (uiHandlerMapping.TryGetValue(value, out AddonUpdateHandler value2))
		{
			value2(addonPtr);
		}
		wheelState = 0;
	}

	private unsafe void HandleCharacterUI(AtkUnitBase* unitBase)
	{
		string nameString = ((AtkUnitBase)unitBase).NameString;
		AddonCharacter* ptr = ((nameString == "Character") ? ((AddonCharacter*)unitBase) : AddonHelper.GetByName<AddonCharacter>("Character"));
		if (ptr == null || !((AtkAddonControl)(&((AddonCharacter)ptr).AddonControl)).IsChildSetupComplete || IntersectingCollisionNode == ((PreviewController)(&((AddonCharacter)ptr).PreviewController)).CollisionNode)
		{
			wheelState = 0;
			return;
		}
		switch (nameString)
		{
		case "Character":
			UpdateCharacter(ptr);
			break;
		case "CharacterClass":
			UpdateCharacterClass(ptr, (AddonCharacterClass*)unitBase);
			break;
		case "CharacterRepute":
			UpdateCharacterRepute(ptr, (AddonCharacterRepute*)unitBase);
			break;
		}
	}

	private int GetTabIndex(int currentTabIndex, int numTabs)
	{
		return Math.Clamp(currentTabIndex + wheelState, 0, numTabs - 1);
	}

	private unsafe void UpdateArmouryBoard(AddonArmouryBoard* addon)
	{
		int tabIndex = GetTabIndex(((AddonArmouryBoard)addon).TabIndex, 12);
		if (((AddonArmouryBoard)addon).TabIndex < tabIndex)
		{
			((AddonArmouryBoard)addon).NextTab((byte)0);
		}
		else if (((AddonArmouryBoard)addon).TabIndex > tabIndex)
		{
			((AddonArmouryBoard)addon).PreviousTab((byte)0);
		}
	}

	private unsafe void UpdateInventory(AddonInventory* addon)
	{
		if (((AddonInventory)addon).TabIndex == 4 && wheelState > 0)
		{
			((AddonInventory)addon).AtkUnitBase.Callback(22, ((int*)addon)[138], 0);
		}
		else
		{
			int tabIndex = GetTabIndex(((AddonInventory)addon).TabIndex, 5);
			if (((AddonInventory)addon).TabIndex != tabIndex)
			{
				((AddonInventory)addon).SetTab(tabIndex);
			}
		}
	}

	private unsafe void UpdateInventoryEvent(AddonInventoryEvent* addon)
	{
		if (((AddonInventoryEvent)addon).TabIndex == 0 && wheelState < 0)
		{
			((AddonInventoryEvent)addon).AtkUnitBase.Callback(22, ((int*)addon)[138], 2);
			return;
		}
		int num = 0;
		Span<Pointer<AtkComponentRadioButton>> buttons = ((AddonInventoryEvent)addon).Buttons;
		for (int i = 0; i < buttons.Length; i++)
		{
			if ((((AtkComponentButton)(&((AtkComponentRadioButton)buttons[i].Value).AtkComponentButton)).Flags & 0x40000) != 0)
			{
				num++;
			}
		}
		int tabIndex = GetTabIndex(((AddonInventoryEvent)addon).TabIndex, num);
		if (((AddonInventoryEvent)addon).TabIndex != tabIndex)
		{
			((AddonInventoryEvent)addon).SetTab(tabIndex);
		}
	}

	private unsafe void UpdateInventoryLarge(AddonInventoryLarge* addon)
	{
		int tabIndex = GetTabIndex(((AddonInventoryLarge)addon).TabIndex, 4);
		if (((AddonInventoryLarge)addon).TabIndex != tabIndex)
		{
			((AddonInventoryLarge)addon).SetTab(tabIndex);
		}
	}

	private unsafe void UpdateInventoryExpansion(AddonInventoryExpansion* addon)
	{
		int tabIndex = GetTabIndex(((AddonInventoryExpansion)addon).TabIndex, 2);
		if (((AddonInventoryExpansion)addon).TabIndex != tabIndex)
		{
			((AddonInventoryExpansion)addon).SetTab(tabIndex, false);
		}
	}

	private unsafe void UpdateInventoryRetainer(AddonInventoryRetainer* addon)
	{
		int tabIndex = GetTabIndex(((AddonInventoryRetainer)addon).TabIndex, 6);
		if (((AddonInventoryRetainer)addon).TabIndex != tabIndex)
		{
			((AddonInventoryRetainer)addon).SetTab(tabIndex);
		}
	}

	private unsafe void UpdateInventoryRetainerLarge(AddonInventoryRetainerLarge* addon)
	{
		int tabIndex = GetTabIndex(((AddonInventoryRetainerLarge)addon).TabIndex, 3);
		if (((AddonInventoryRetainerLarge)addon).TabIndex != tabIndex)
		{
			((AddonInventoryRetainerLarge)addon).SetTab(tabIndex);
		}
	}

	private unsafe void UpdateTabController(AtkUnitBase* addon, TabController* tabController)
	{
		int tabIndex = GetTabIndex(((TabController)tabController).TabIndex, ((TabController)tabController).TabCount);
		if (((TabController)tabController).TabIndex != tabIndex)
		{
			((TabController)tabController).TabIndex = tabIndex;
			((TabController)tabController).CallbackFunction(tabIndex, addon);
		}
	}

	private unsafe void UpdateAOZNotebook(AddonAOZNotebook* addon)
	{
		int tabIndex = GetTabIndex(((AddonAOZNotebook)addon).TabIndex, ((AddonAOZNotebook)addon).TabCount);
		if (((AddonAOZNotebook)addon).TabIndex != tabIndex)
		{
			((AddonAOZNotebook)addon).SetTab(tabIndex, true);
		}
	}

	private unsafe void UpdateAetherCurrent(AddonAetherCurrent* addon)
	{
		int tabIndex = GetTabIndex(((AddonAetherCurrent)addon).TabIndex, ((AddonAetherCurrent)addon).TabCount);
		if (((AddonAetherCurrent)addon).TabIndex != tabIndex)
		{
			((AddonAetherCurrent)addon).SetTab(tabIndex);
			for (int i = 0; i < ((AddonAetherCurrent)addon).Tabs.Length; i++)
			{
				((AtkComponentRadioButton)((AddonAetherCurrent)addon).Tabs[i].Value).IsSelected = i == tabIndex;
			}
		}
	}

	private unsafe void UpdateFateProgress(AddonFateProgress* addon)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		int tabIndex = GetTabIndex(((AddonFateProgress)addon).TabIndex, ((AddonFateProgress)addon).TabCount);
		if (((AddonFateProgress)addon).IsLoaded && ((AddonFateProgress)addon).TabIndex != tabIndex)
		{
			AtkEvent val = default(AtkEvent);
			((AddonFateProgress)addon).SetTab(tabIndex, &val);
		}
	}

	private unsafe void UpdateFieldNotes(AddonMYCWarResultNotebook* addon)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (IntersectingCollisionNode == ((AddonMYCWarResultNotebook)addon).DescriptionCollisionNode)
		{
			return;
		}
		AtkEvent val = default(AtkEvent);
		int num = Math.Clamp(((AddonMYCWarResultNotebook)addon).CurrentNoteIndex % 10 + wheelState, -1, ((AddonMYCWarResultNotebook)addon).MaxNoteIndex - 1);
		switch (num)
		{
		case -1:
			if (((AddonMYCWarResultNotebook)addon).CurrentPageIndex > 0)
			{
				int num3 = ((AddonMYCWarResultNotebook)addon).CurrentPageIndex - 1;
				((AtkUnitBase)(&((AddonMYCWarResultNotebook)addon).AtkUnitBase)).ReceiveEvent((AtkEventType)25, num3 + 10, &val, (AtkEventData*)null);
				((AtkUnitBase)(&((AddonMYCWarResultNotebook)addon).AtkUnitBase)).ReceiveEvent((AtkEventType)25, 9, &val, (AtkEventData*)null);
			}
			break;
		case 10:
			if (((AddonMYCWarResultNotebook)addon).CurrentPageIndex < 4)
			{
				int num2 = ((AddonMYCWarResultNotebook)addon).CurrentPageIndex + 1;
				((AtkUnitBase)(&((AddonMYCWarResultNotebook)addon).AtkUnitBase)).ReceiveEvent((AtkEventType)25, num2 + 10, &val, (AtkEventData*)null);
			}
			break;
		default:
			((AtkUnitBase)(&((AddonMYCWarResultNotebook)addon).AtkUnitBase)).ReceiveEvent((AtkEventType)25, num, &val, (AtkEventData*)null);
			break;
		}
	}

	private unsafe void UpdateMountMinion(AddonMinionMountBase* addon)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Invalid comparison between Unknown and I4
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		ViewType currentView = ((AddonMinionMountBase)addon).CurrentView;
		if ((int)currentView != 1)
		{
			if ((int)currentView == 2)
			{
				if (((TabController)(&((AddonMinionMountBase)addon).TabController)).TabIndex == 0 && wheelState < 0)
				{
					((AddonMinionMountBase)addon).SwitchToFavorites();
				}
				else
				{
					UpdateTabController((AtkUnitBase*)addon, &((AddonMinionMountBase)addon).TabController);
				}
			}
		}
		else if (wheelState > 0)
		{
			((TabController)(&((AddonMinionMountBase)addon).TabController)).CallbackFunction(0, (AtkUnitBase*)addon);
		}
	}

	private unsafe void UpdateMJIMinionNoteBook(AddonMJIMinionNoteBook* addon)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Invalid comparison between Unknown and I4
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		AgentMJIMinionNoteBook* ptr = AgentMJIMinionNoteBook.Instance();
		if ((int)((AgentMJIMinionNoteBook)ptr).CurrentView == 2)
		{
			if (((TabController)(&((AddonMJIMinionNoteBook)addon).TabController)).TabIndex == 0 && wheelState < 0)
			{
				Unsafe.Write(&((AgentMJIMinionNoteBook)ptr).CurrentView, (ViewType)1);
				((SelectedMinionInfo)(&((AgentMJIMinionNoteBook)ptr).SelectedFavoriteMinion)).TabIndex = 0;
				((SelectedMinionInfo)(&((AgentMJIMinionNoteBook)ptr).SelectedFavoriteMinion)).SlotIndex = ((SelectedMinionInfo)(&((AgentMJIMinionNoteBook)ptr).SelectedNormalMinion)).SlotIndex;
				((SelectedMinionInfo)(&((AgentMJIMinionNoteBook)ptr).SelectedFavoriteMinion)).MinionId = ((AgentMJIMinionNoteBook)ptr).GetSelectedMinionId();
				((AgentMJIMinionNoteBook)ptr).SelectedMinion = &((AgentMJIMinionNoteBook)ptr).SelectedFavoriteMinion;
				((AgentMJIMinionNoteBook)ptr).HandleCommand(1031);
			}
			else
			{
				UpdateTabController((AtkUnitBase*)addon, &((AddonMJIMinionNoteBook)addon).TabController);
				((AgentMJIMinionNoteBook)ptr).HandleCommand(1035);
			}
		}
		else if ((int)((AgentMJIMinionNoteBook)ptr).CurrentView == 1 && wheelState > 0)
		{
			Unsafe.Write(&((AgentMJIMinionNoteBook)ptr).CurrentView, (ViewType)2);
			((SelectedMinionInfo)(&((AgentMJIMinionNoteBook)ptr).SelectedNormalMinion)).TabIndex = 0;
			((SelectedMinionInfo)(&((AgentMJIMinionNoteBook)ptr).SelectedNormalMinion)).SlotIndex = ((SelectedMinionInfo)(&((AgentMJIMinionNoteBook)ptr).SelectedFavoriteMinion)).SlotIndex;
			((SelectedMinionInfo)(&((AgentMJIMinionNoteBook)ptr).SelectedNormalMinion)).MinionId = ((AgentMJIMinionNoteBook)ptr).GetSelectedMinionId();
			((AgentMJIMinionNoteBook)ptr).SelectedMinion = &((AgentMJIMinionNoteBook)ptr).SelectedNormalMinion;
			((TabController)(&((AddonMJIMinionNoteBook)addon).TabController)).TabIndex = 0;
			((TabController)(&((AddonMJIMinionNoteBook)addon).TabController)).CallbackFunction(0, (AtkUnitBase*)addon);
			((AgentMJIMinionNoteBook)ptr).HandleCommand(1035);
		}
	}

	private unsafe void UpdateInventoryBuddy(AddonInventoryBuddy* addon)
	{
		if (((PlayerState)PlayerState.Instance()).HasPremiumSaddlebag)
		{
			int tabIndex = GetTabIndex(((AddonInventoryBuddy)addon).TabIndex, 2);
			if (((AddonInventoryBuddy)addon).TabIndex != tabIndex)
			{
				((AddonInventoryBuddy)addon).SetTab((byte)tabIndex);
			}
		}
	}

	private unsafe void UpdateCurrency(AddonCurrency* addon)
	{
		AtkStage* ptr = AtkStage.Instance();
		NumberArrayData* numberArrayData = ((AtkStage)ptr).GetNumberArrayData((NumberArrayType)82);
		int intArray = *((NumberArrayData)numberArrayData).IntArray;
		int num = intArray;
		bool[] array = new bool[((AddonCurrency)addon).Tabs.Length];
		for (int i = 0; i < ((AddonCurrency)addon).Tabs.Length; i++)
		{
			array[i] = ((AddonCurrency)addon).Tabs[i].Value != null && ((AtkComponentRadioButton)((AddonCurrency)addon).Tabs[i].Value).IsEnabled;
		}
		if (wheelState > 0 && intArray < array.Length)
		{
			for (int j = intArray + 1; j < array.Length; j++)
			{
				if (array[j])
				{
					num = j;
					break;
				}
			}
		}
		else if (intArray > 0)
		{
			for (int num2 = intArray - 1; num2 >= 0; num2--)
			{
				if (array[num2])
				{
					num = num2;
					break;
				}
			}
		}
		if (intArray != num)
		{
			((NumberArrayData)numberArrayData).SetValue(0, num, false, false);
			((AtkUnitBase)(&((AddonCurrency)addon).AtkUnitBase)).OnRequestedUpdate(((AtkStage)ptr).GetNumberArrayData(), ((AtkStage)ptr).GetStringArrayData());
		}
	}

	private unsafe void UpdateBuddy(AddonBuddy* addon)
	{
		int tabIndex = GetTabIndex(((AddonBuddy)addon).TabIndex, 3);
		if (((AddonBuddy)addon).TabIndex == tabIndex)
		{
			return;
		}
		((AddonBuddy)addon).SetTab(tabIndex);
		for (int i = 0; i < 3; i++)
		{
			Pointer<AtkComponentRadioButton>* pointer = SpanExtensions.GetPointer<Pointer<AtkComponentRadioButton>>(((AddonBuddy)addon).RadioButtons, i);
			if (pointer->Value != null)
			{
				((AtkComponentRadioButton)pointer->Value).IsSelected = i == ((AddonBuddy)addon).TabIndex;
			}
		}
	}

	private unsafe void UpdateMiragePrismPrismBox(AddonMiragePrismPrismBox* addon)
	{
		if (((AddonMiragePrismPrismBox)addon).JobDropdown != null && ((AtkComponentDropDownList)((AddonMiragePrismPrismBox)addon).JobDropdown).List != null && ((AtkComponentBase)(&((AtkComponentList)((AtkComponentDropDownList)((AddonMiragePrismPrismBox)addon).JobDropdown).List).AtkComponentBase)).OwnerNode != null && !((AtkResNode)(&((AtkComponentNode)((AtkComponentBase)(&((AtkComponentList)((AtkComponentDropDownList)((AddonMiragePrismPrismBox)addon).JobDropdown).List).AtkComponentBase)).OwnerNode).AtkResNode)).IsVisible() && ((AddonMiragePrismPrismBox)addon).OrderDropdown != null && ((AtkComponentDropDownList)((AddonMiragePrismPrismBox)addon).OrderDropdown).List != null && ((AtkComponentBase)(&((AtkComponentList)((AtkComponentDropDownList)((AddonMiragePrismPrismBox)addon).OrderDropdown).List).AtkComponentBase)).OwnerNode != null && !((AtkResNode)(&((AtkComponentNode)((AtkComponentBase)(&((AtkComponentList)((AtkComponentDropDownList)((AddonMiragePrismPrismBox)addon).OrderDropdown).List).AtkComponentBase)).OwnerNode).AtkResNode)).IsVisible())
		{
			AtkComponentButton* ptr = ((!config.Invert) ? ((AddonMiragePrismPrismBox)addon).PrevButton : ((AddonMiragePrismPrismBox)addon).NextButton);
			AtkComponentButton* ptr2 = ((!config.Invert) ? ((AddonMiragePrismPrismBox)addon).NextButton : ((AddonMiragePrismPrismBox)addon).PrevButton);
			if (ptr != null && (!IsPrev || ((AtkComponentButton)ptr).IsEnabled) && ptr2 != null && (!IsNext || ((AtkComponentButton)ptr2).IsEnabled) && !(*Addons.MiragePrismPrismBoxFilter).IsAddonAndNodesReady())
			{
				AgentMiragePrismPrismBox* intPtr = AgentMiragePrismPrismBox.Instance();
				byte* pageIndex = &((AgentMiragePrismPrismBox)intPtr).PageIndex;
				*pageIndex += (byte)wheelState;
				((AgentMiragePrismPrismBox)intPtr).UpdateItems(false, false);
			}
		}
	}

	private unsafe void UpdateCharacter(AddonCharacter* addon)
	{
		int tabIndex = GetTabIndex(((AddonCharacter)addon).TabIndex, ((AddonCharacter)addon).TabCount);
		if (((AddonCharacter)addon).TabIndex == tabIndex)
		{
			return;
		}
		((AddonCharacter)addon).SetTab(tabIndex);
		for (int i = 0; i < ((AddonCharacter)addon).TabCount; i++)
		{
			Pointer<AtkComponentRadioButton>* pointer = SpanExtensions.GetPointer<Pointer<AtkComponentRadioButton>>(((AddonCharacter)addon).Tabs, i);
			if (pointer->Value != null)
			{
				((AtkComponentRadioButton)pointer->Value).IsSelected = i == ((AddonCharacter)addon).TabIndex;
			}
		}
	}

	private unsafe void UpdateCharacterClass(AddonCharacter* addonCharacter, AddonCharacterClass* addon)
	{
		if (((AddonCharacterClass)addon).TabIndex + wheelState < 0 || ((AddonCharacterClass)addon).TabIndex + wheelState > 1)
		{
			UpdateCharacter(addonCharacter);
			return;
		}
		int tabIndex = GetTabIndex(((AddonCharacterClass)addon).TabIndex, 2);
		if (((AddonCharacterClass)addon).TabIndex != tabIndex)
		{
			((AddonCharacterClass)addon).SetTab(tabIndex);
		}
	}

	private unsafe void UpdateCharacterRepute(AddonCharacter* addonCharacter, AddonCharacterRepute* addon)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (((AddonCharacterRepute)addon).SelectedExpansion + wheelState < 0)
		{
			UpdateCharacter(addonCharacter);
			return;
		}
		int tabIndex = GetTabIndex(((AddonCharacterRepute)addon).SelectedExpansion, ((AddonCharacterRepute)addon).ExpansionsCount);
		if (((AddonCharacterRepute)addon).SelectedExpansion != tabIndex)
		{
			AtkEvent val = default(AtkEvent);
			AtkEventData val2 = new AtkEventData
			{
				ListItemData = 
				{
					SelectedIndex = tabIndex
				}
			};
			((AtkUnitBase)(&((AddonCharacterRepute)addon).AtkUnitBase)).ReceiveEvent((AtkEventType)37, 0, &val, &val2);
		}
	}
}
