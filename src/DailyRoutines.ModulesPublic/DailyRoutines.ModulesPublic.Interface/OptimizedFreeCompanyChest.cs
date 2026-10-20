using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using DailyRoutines.Common.Info;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.Gui.ContextMenu;
using Dalamud.Game.Inventory;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;
using Lumina.Text;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.Interop.Game.Models.Native;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;

namespace DailyRoutines.ModulesPublic.Interface;

public class OptimizedFreeCompanyChest : ModuleBase
{
	private unsafe delegate bool SendInventoryRefreshDelegate(InventoryManager* instance, int inventoryType);

	private unsafe delegate nint MoveItemDelegate(void* agent, InventoryType srcInv, uint srcSlot, InventoryType dstInv, uint dstSlot);

	private class Config : ModuleConfig
	{
		public InventoryType DefaultPage = (InventoryType)9999;

		public bool FastMoveItem = true;
	}

	private static readonly CompSig SendInventoryRefreshSig = new CompSig("48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC ?? 8B DA 48 8B F1 33 D2 0F B7 FA");

	private Hook<SendInventoryRefreshDelegate>? SendInventoryRefreshHook;

	private MoveItemDelegate moveItem;

	private Config config;

	private CheckboxNode? fastMoveNode;

	private CheckboxNode? defaultPageNode;

	private HorizontalListNode? componentNode;

	private IconImageNode? gilIconNode;

	private TextNode? gilItemsValueCountNode;

	private bool isNeedToClose;

	private long lastTotalPrice;

	private static readonly uint[] ItemIDs = (from x in ((IEnumerable<Item>)LuminaGetter.Get<Item>()).Where(delegate(Item x)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			ItemSortCategory value = ((Item)(ref x)).ItemSortCategory.Value;
			return ((ItemSortCategory)(ref value)).Param == 150;
		})
		select ((Item)(ref x)).RowId).ToArray();

	private static readonly FrozenDictionary<InventoryType, string> DefaultPages = new Dictionary<InventoryType, string>
	{
		[(InventoryType)20000] = LuminaWrapper.GetFCChestName(0u) + " \ue090",
		[(InventoryType)20001] = LuminaWrapper.GetFCChestName(0u) + " \ue091",
		[(InventoryType)20002] = LuminaWrapper.GetFCChestName(0u) + " \ue092",
		[(InventoryType)20003] = LuminaWrapper.GetFCChestName(0u) + " \ue093",
		[(InventoryType)20004] = LuminaWrapper.GetFCChestName(0u) + " \ue094",
		[(InventoryType)22001] = LuminaWrapper.GetAddonText(2990u) ?? "",
		[(InventoryType)9999] = LuminaWrapper.GetAddonText(7u) ?? ""
	}.ToFrozenDictionary();

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("OptimizedFreeCompanyChestTitle"),
		Description = LanguageManager.Get("OptimizedFreeCompanyChestDescription"),
		Category = ModuleCategory.Interface
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected unsafe override void Init()
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Expected O, but got Unknown
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Expected O, but got Unknown
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Expected O, but got Unknown
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Expected O, but got Unknown
		moveItem = new CompSig("40 53 55 56 57 41 57 48 83 EC ?? 45 33 FF").GetDelegate<MoveItemDelegate>();
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		if (SendInventoryRefreshHook == null)
		{
			SendInventoryRefreshHook = SendInventoryRefreshSig.GetHook<SendInventoryRefreshDelegate>(SendInventoryRefreshDetour);
		}
		SendInventoryRefreshHook.Enable();
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)1, "FreeCompanyChest", new AddonEventDelegate(OnAddonChest));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)5, "FreeCompanyChest", new AddonEventDelegate(OnAddonChest));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, "FreeCompanyChest", new AddonEventDelegate(OnAddonChest));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)1, "InputNumeric", new AddonEventDelegate(OnAddonInput));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)4, "ContextMenu", new AddonEventDelegate(OnAddonContextMenu));
		DalamudServiceExtension.Instance<IContextMenu>().OnMenuOpened += new OnMenuOpenedDelegate(OnContextMenuOpened);
	}

	protected override void Uninit()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected O, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IContextMenu>().OnMenuOpened -= new OnMenuOpenedDelegate(OnContextMenuOpened);
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddonContextMenu });
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddonChest });
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddonInput });
		ClearNodes();
		isNeedToClose = false;
	}

	private unsafe void OnAddonChest(AddonEvent type, AddonArgs args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Invalid comparison between Unknown and I4
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Invalid comparison between Unknown and I4
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Invalid comparison between Unknown and I4
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Expected I4, but got Unknown
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
		if ((int)type != 1)
		{
			if ((int)type != 5)
			{
				if ((int)type == 6)
				{
					isNeedToClose = false;
					ClearNodes();
				}
			}
			else
			{
				if (Addons.FreeCompanyChest == null)
				{
					return;
				}
				if (fastMoveNode == null)
				{
					fastMoveNode = new CheckboxNode
					{
						Size = new Vector2(160f, 28f),
						Position = new Vector2(5f, 210f),
						IsVisible = true,
						IsChecked = config.FastMoveItem,
						IsEnabled = true,
						String = ReadOnlySeString.op_Implicit(LanguageManager.Get("OptimizedFreeCompanyChest-FastMove")),
						OnClick = delegate(bool newState)
						{
							config.FastMoveItem = newState;
							config.Save(this);
							isNeedToClose = false;
						},
						TextTooltip = ReadOnlySeString.op_Implicit(new SeStringBuilder().AddIcon((BitmapFontIcon)101).Append(" " + LanguageManager.Get("OptimizedFreeCompanyChest-FastMoveHelp")).Build()
							.Encode())
					};
					fastMoveNode.AttachNode(((AtkUnitBase)Addons.FreeCompanyChest).GetNodeById(9u));
				}
				fastMoveNode.IsChecked = config.FastMoveItem;
				fastMoveNode.IsVisible = ((AtkValue)((byte*)((AtkUnitBase)Addons.FreeCompanyChest).AtkValues + Unsafe.SizeOf<AtkValue>())).UInt == 0;
				if (defaultPageNode == null)
				{
					defaultPageNode = new CheckboxNode
					{
						Size = new Vector2(160f, 28f),
						Position = new Vector2(5f, 156f),
						IsVisible = true,
						IsChecked = false,
						IsEnabled = true,
						String = ReadOnlySeString.op_Implicit(LanguageManager.Get("OptimizedFreeCompanyChest-DefaultPage")),
						OnClick = delegate(bool newState)
						{
							//IL_0031: Unknown result type (might be due to invalid IL or missing references)
							//IL_0012: Unknown result type (might be due to invalid IL or missing references)
							//IL_0013: Unknown result type (might be due to invalid IL or missing references)
							if (newState)
							{
								if (TryGetCurrentFCPage(out var page2))
								{
									config.DefaultPage = page2;
									config.Save(this);
								}
							}
							else
							{
								config.DefaultPage = (InventoryType)9999;
								config.Save(this);
							}
							UpdateDefaultPageTooltip();
							defaultPageNode.HideTooltip();
							defaultPageNode.ShowTooltip();
						}
					};
					UpdateDefaultPageTooltip();
					defaultPageNode.AttachNode(((AtkUnitBase)Addons.FreeCompanyChest).GetNodeById(9u));
				}
				AtkResNode* nodeById = ((AtkUnitBase)Addons.FreeCompanyChest).GetNodeById(16u);
				if (nodeById != null)
				{
					((AtkResNode)nodeById).SetPositionFloat(0f, 185f);
				}
				if (Throttler.Shared.Throttle("OptimizedFreeCompanyChest-OnUpdateDefaultPage", 100u))
				{
					defaultPageNode.IsChecked = TryGetCurrentFCPage(out var page) && config.DefaultPage == page;
				}
				if (componentNode == null)
				{
					componentNode = new HorizontalListNode
					{
						IsVisible = true,
						Position = new Vector2(380f, -38f),
						Size = new Vector2(0f, 60f),
						ItemSpacing = 4f,
						Alignment = HorizontalListAnchor.Right
					};
					componentNode.AddNodeFlags((NodeFlags)256);
					gilIconNode = new IconImageNode
					{
						IsVisible = true,
						IconId = 65002u,
						Size = new Vector2(32f),
						FitTexture = true
					};
					gilItemsValueCountNode = new TextNode
					{
						Size = new Vector2(100f, 28f),
						Position = new Vector2(0f, 6f),
						TextFlags = (TextFlags)25,
						String = ReadOnlySeString.op_Implicit("0\ue049"),
						FontSize = 14u,
						AlignmentType = (AlignmentType)5,
						TextTooltip = ReadOnlySeString.op_Implicit(LanguageManager.Get("OptimizedFreeCompanyChest-ExchangableItemsTotalValue"))
					};
					AtkColors.Value.ApplyTo(gilItemsValueCountNode);
					componentNode.AddNode(new global::_003C_003Ez__ReadOnlyArray<NodeBase>(new NodeBase[2] { gilIconNode, gilItemsValueCountNode }));
					componentNode.RecalculateLayout();
					componentNode.AttachNode(((AtkUnitBase)Addons.FreeCompanyChest).GetNodeById(9u));
				}
				if (Throttler.Shared.Throttle("OptimizedFreeCompanyChest-OnUpdateGilItemsValue", 100u))
				{
					lastTotalPrice = (TryGetTotalPrice(out var totalPrice) ? totalPrice : 0);
					gilItemsValueCountNode.String = ReadOnlySeString.op_Implicit(lastTotalPrice.ToChineseString() + "\ue049");
					gilItemsValueCountNode.Width = gilItemsValueCountNode.GetTextDrawSize(considerScale: false).X - 8f;
					componentNode.RecalculateLayout();
					componentNode.IsVisible = lastTotalPrice > 0;
				}
			}
		}
		else
		{
			if (Addons.FreeCompanyChest == null || (int)config.DefaultPage == 9999)
			{
				return;
			}
			if ((int)config.DefaultPage == 22001)
			{
				DalamudServiceExtension.Instance<IFramework>().Run((Action)delegate
				{
					(*(AtkComponentRadioButton*)((AtkUnitBase)Addons.FreeCompanyChest).GetComponentByNodeId(15u)).Click();
				}, default(CancellationToken));
			}
			else
			{
				if ((int)config.DefaultPage < 20000)
				{
					return;
				}
				int index = config.DefaultPage % 20000;
				if (index <= 5)
				{
					DalamudServiceExtension.Instance<IFramework>().Run((Action)delegate
					{
						(*(AtkComponentRadioButton*)((AtkUnitBase)Addons.FreeCompanyChest).GetComponentByNodeId((uint)(10 + index))).Click();
					}, default(CancellationToken));
				}
			}
		}
		unsafe void UpdateDefaultPageTooltip()
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			RentedSeStringBuilder val = default(RentedSeStringBuilder);
			((RentedSeStringBuilder)(ref val))._002Ector();
			try
			{
				SeStringBuilder builder = ((RentedSeStringBuilder)(ref val)).Builder;
				CheckboxNode? checkboxNode = defaultPageNode;
				SeStringBuilder val2 = builder.AppendIcon(101u);
				SeStringInterpolatedStringHandler val3 = new SeStringInterpolatedStringHandler(1, 1, val2);
				((SeStringInterpolatedStringHandler)(ref val3)).AppendLiteral(" ");
				((SeStringInterpolatedStringHandler)(ref val3)).AppendFormatted<string>(LanguageManager.Get("OptimizedFreeCompanyChest-DefaultPageHelp"));
				SeStringBuilder val4 = val2.Append(val3).AppendNewLine().AppendNewLine();
				val3 = new SeStringInterpolatedStringHandler(2, 2, val4);
				((SeStringInterpolatedStringHandler)(ref val3)).AppendFormatted<string>(LanguageManager.Get("Current"));
				((SeStringInterpolatedStringHandler)(ref val3)).AppendLiteral(": ");
				((SeStringInterpolatedStringHandler)(ref val3)).AppendFormatted<string>(DefaultPages.GetValueOrDefault(config.DefaultPage, LuminaWrapper.GetAddonText(7u)));
				checkboxNode.TextTooltip = val4.Append(val3).ToReadOnlySeString();
			}
			finally
			{
				((IDisposable)(*(RentedSeStringBuilder*)(&val))/*cast due to constrained. prefix*/).Dispose();
			}
		}
	}

	private unsafe void OnContextMenuOpened(IMenuOpenedArgs args)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Invalid comparison between Unknown and I4
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Invalid comparison between Unknown and I4
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		if (Addons.FreeCompanyChest == null || !config.FastMoveItem)
		{
			return;
		}
		AgentFreeCompanyChest* ptr = AgentFreeCompanyChest.Instance();
		if (ptr == null)
		{
			return;
		}
		if (((IMenuArgs)args).AddonName == "FreeCompanyChest" && (int)ptr->ContextInventoryType != 9999)
		{
			InventoryItem* contextInventoryItem = ptr->GetContextInventoryItem();
			if (contextInventoryItem == null || ((InventoryItem)contextInventoryItem).ItemId == 0)
			{
				return;
			}
			{
				foreach (InventoryType item in Inventories.Player)
				{
					if (TryFindFirstSuitableSlot(item, contextInventoryItem, out var foundSlot))
					{
						isNeedToClose = true;
						moveItem(ptr, ptr->ContextInventoryType, (uint)ptr->ContextInventorySlot, item, (uint)foundSlot);
						Unsafe.Write(&ptr->ContextInventoryType, (InventoryType)9999);
						break;
					}
				}
				return;
			}
		}
		if (!((IMenuArgs)args).AddonName.StartsWith("Inventory"))
		{
			return;
		}
		MenuTarget target = ((IMenuArgs)args).Target;
		MenuTargetInventory val = (MenuTargetInventory)(object)((target is MenuTargetInventory) ? target : null);
		if (val == null)
		{
			return;
		}
		GameInventoryItem? targetItem = val.TargetItem;
		if (targetItem.HasValue)
		{
			GameInventoryItem valueOrDefault = targetItem.GetValueOrDefault();
			if (TryGetSelectedItemSource(out var sourceInventory, out var sourceSlot) && TryGetCurrentFCPage(out var page) && (int)page != 22001 && TryFindFirstSuitableSlot(page, (InventoryItem*)((GameInventoryItem)(ref valueOrDefault)).Address, out var foundSlot2))
			{
				isNeedToClose = true;
				moveItem(ptr, sourceInventory, sourceSlot, page, (uint)foundSlot2);
			}
		}
	}

	private unsafe void OnAddonContextMenu(AddonEvent type, AddonArgs args)
	{
		if (isNeedToClose && Addons.ContextMenuAddon != null)
		{
			((AtkUnitBase)Addons.ContextMenuAddon).IsVisible = false;
			((AtkUnitBase)Addons.ContextMenuAddon).Close(true);
			isNeedToClose = false;
		}
	}

	private unsafe void OnAddonInput(AddonEvent type, AddonArgs args)
	{
		if (config.FastMoveItem && Addons.InputNumeric != null && (*Addons.FreeCompanyChest).IsAddonAndNodesReady())
		{
			(*Addons.InputNumeric).Callback((int)((AtkValue)((byte*)((AtkUnitBase)Addons.InputNumeric).AtkValues + (nint)3 * (nint)Unsafe.SizeOf<AtkValue>())).UInt);
		}
	}

	private unsafe static bool SendInventoryRefreshDetour(InventoryManager* instance, int inventoryType)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestInventory, (uint)inventoryType);
		return true;
	}

	private void ClearNodes()
	{
		fastMoveNode?.Dispose();
		fastMoveNode = null;
		defaultPageNode?.Dispose();
		defaultPageNode = null;
		componentNode?.Dispose();
		componentNode = null;
		gilIconNode?.Dispose();
		gilIconNode = null;
		gilItemsValueCountNode?.Dispose();
		gilItemsValueCountNode = null;
	}

	private unsafe static bool TryGetSelectedItemSource(out InventoryType sourceInventory, out ushort sourceSlot)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected I4, but got Unknown
		sourceInventory = (InventoryType)9999;
		sourceSlot = 0;
		AgentInventoryContext* ptr = AgentInventoryContext.Instance();
		if (ptr == null || ((AgentInventoryContext)ptr).TargetInventorySlot == null)
		{
			return false;
		}
		if (((InventoryItem)((AgentInventoryContext)ptr).TargetInventorySlot).ItemId == 0)
		{
			return false;
		}
		sourceInventory = (InventoryType)(int)((AgentInventoryContext)ptr).TargetInventoryId;
		sourceSlot = (ushort)((AgentInventoryContext)ptr).TargetInventorySlotId;
		return true;
	}

	private unsafe static bool TryGetCurrentFCPage(out InventoryType page)
	{
		page = (InventoryType)9999;
		if (Addons.FreeCompanyChest == null || (*((AtkUnitBase)Addons.FreeCompanyChest).GetNodeById(106u)).GetVisibility())
		{
			return false;
		}
		if (((AtkValue)((byte*)((AtkUnitBase)Addons.FreeCompanyChest).AtkValues + Unsafe.SizeOf<AtkValue>())).UInt != 0)
		{
			page = (InventoryType)22001;
			return true;
		}
		page = (InventoryType)(20000 + ((AtkValue)((byte*)((AtkUnitBase)Addons.FreeCompanyChest).AtkValues + (nint)2 * (nint)Unsafe.SizeOf<AtkValue>())).UInt);
		return true;
	}

	private unsafe static bool TryFindFirstSuitableSlot(InventoryType type, InventoryItem* srcItem, out short foundSlot)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		foundSlot = -1;
		if (srcItem == null || ((InventoryItem)srcItem).ItemId == 0)
		{
			return false;
		}
		InventoryManager* ptr = InventoryManager.Instance();
		if (ptr == null)
		{
			return false;
		}
		InventoryContainer* inventoryContainer = ((InventoryManager)ptr).GetInventoryContainer(type);
		if (inventoryContainer == null || !((InventoryContainer)inventoryContainer).IsLoaded)
		{
			return false;
		}
		if (!LuminaGetter.TryGetRow<Item>(((InventoryItem)srcItem).GetBaseItemId(), out Item item))
		{
			return false;
		}
		if (((Item)(ref item)).StackSize > 1)
		{
			for (int i = 0; i < ((InventoryContainer)inventoryContainer).Size; i++)
			{
				InventoryItem* inventorySlot = ((InventoryContainer)inventoryContainer).GetInventorySlot(i);
				if (inventorySlot != null && ((InventoryItem)inventorySlot).GetBaseItemId() == ((InventoryItem)srcItem).GetBaseItemId() && ((InventoryItem)inventorySlot).Flags == ((InventoryItem)srcItem).Flags && ((InventoryItem)inventorySlot).Quantity + ((InventoryItem)srcItem).Quantity <= ((Item)(ref item)).StackSize)
				{
					foundSlot = (short)i;
					return true;
				}
			}
		}
		for (int j = 0; j < ((InventoryContainer)inventoryContainer).Size; j++)
		{
			if (((InventoryItem)((InventoryContainer)inventoryContainer).GetInventorySlot(j)).ItemId == 0)
			{
				foundSlot = (short)j;
				return true;
			}
		}
		return false;
	}

	private unsafe static bool TryGetTotalPrice(out long totalPrice)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Invalid comparison between Unknown and I4
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		totalPrice = 0L;
		if (!(*Addons.FreeCompanyChest).IsAddonAndNodesReady())
		{
			return false;
		}
		InventoryManager* ptr = InventoryManager.Instance();
		if (ptr == null)
		{
			return false;
		}
		if (!TryGetCurrentFCPage(out var page) || (int)page == 22001)
		{
			return false;
		}
		uint[] itemIDs = ItemIDs;
		foreach (uint num in itemIDs)
		{
			if (LuminaGetter.TryGetRow<Item>(num, out Item item))
			{
				int itemCountInContainer = ((InventoryManager)ptr).GetItemCountInContainer(num, page, false, (short)0);
				if (itemCountInContainer != 0)
				{
					uint priceLow = ((Item)(ref item)).PriceLow;
					totalPrice += itemCountInContainer * priceLow;
				}
			}
		}
		return totalPrice > 0;
	}
}
