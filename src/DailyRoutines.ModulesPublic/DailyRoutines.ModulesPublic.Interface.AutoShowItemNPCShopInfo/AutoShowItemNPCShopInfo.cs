using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Threading;
using DailyRoutines.Common.Info;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.Nodes;
using KamiToolKit.Nodes.Simplified;
using Lumina.Text;
using Lumina.Text.ReadOnly;
using OmenTools.Dalamud.Attributes;
using OmenTools.Extensions;
using OmenTools.Info.Game.ItemSource;
using OmenTools.Info.Game.ItemSource.Enums;
using OmenTools.Info.Game.ItemSource.Models;
using OmenTools.Interop.Game.Helpers;
using OmenTools.Interop.Game.Lumina;
using OmenTools.KamiToolKit.Nodes;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic.Interface.AutoShowItemNPCShopInfo;

public class AutoShowItemNPCShopInfo : ModuleBase
{
	private abstract class AddonNPCShopsBase<TSectionSlot> : NativeAddon where TSectionSlot : AddonNPCShopsBase<TSectionSlot>.SectionSlot
	{
		public sealed record NPCDisplayInfo(string Name, ShopNPCLocation? Location, List<ShopItemCostInfo> CostInfos);

		public class SectionSlot
		{
			public ResNode Container;

			public SimpleNineGridNode Background;

			public VerticalListNode Content;

			public NPCRowNode[] NPCRows;

			public PaginationNode NPCPaginationBar;

			public int NPCCurrentPage;

			public List<NPCDisplayInfo> SortedNPCInfos = new List<NPCDisplayInfo>();
		}

		public class NPCRowNode : ListButtonNode
		{
			public NPCDisplayInfo? NPCInfo { get; private set; }

			public unsafe NPCRowNode()
			{
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_0032: Expected O, but got Unknown
				AtkColors.Text.ApplyTo(base.LabelNode);
				CollisionNode.AddEvent((AtkEventType)9, (ReceiveEvent)delegate(AtkEventListener* _, AtkEventType _, int _, AtkEvent* _, AtkEventData* data)
				{
					if (AtkEventDataExtension.get_IsRightClick(in *data))
					{
						NPCDisplayInfo nPCInfo = NPCInfo;
						if ((object)nPCInfo != null)
						{
							OpenTeleportMenu(nPCInfo.Location);
						}
					}
				});
			}

			public void SetNPCInfo(NPCDisplayInfo info)
			{
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_0053: Unknown result type (might be due to invalid IL or missing references)
				NPCInfo = info;
				string text = info.Location.GetTerritory().ExtractPlaceName();
				base.String = ReadOnlySeString.op_Implicit(info.Name + "（" + text + "）");
				TextTooltip = ReadOnlySeString.op_Implicit(info.Name + "\n（" + text + "）");
			}

			private unsafe void OpenTeleportMenu(ShopNPCLocation location)
			{
				OmenServiceBase<ContextMenuManager>.Instance().Open(new ContextMenuOpenedArgs
				{
					OwnerAddonID = ((AtkUnitBase)base.ParentAddon).Id
				}, new global::_003C_003Ez__ReadOnlySingleElementList<ContextMenuEntry>(new ContextMenuEntryInfo("AutoShowItemNPCShopInfo", (ContextMenuOpenedArgs _) => new ContextMenuItem
				{
					Name = ReadOnlySeString.op_Implicit(LuminaWrapper.GetAddonText(1806u)),
					IsEnabled = (location.TerritoryID != 282),
					OnClicked = delegate
					{
						DalamudServiceExtension.Instance<IFramework>().RunOnTick((Action)delegate
						{
							TeleportToLocation(location);
						}, default(TimeSpan), 1, default(CancellationToken));
					}
				})));
			}
		}

		protected const int MAX_COSTS = 4;

		protected const int NPCS_PER_PAGE = 5;

		protected const float HEADER_HEIGHT = 42f;

		protected const float ROW_SPACING = 4f;

		private int currentPage;

		private int totalPages;

		private ScrollingNode<VerticalListNode>? scrollingAreaNode;

		private VerticalListNode? contentNode;

		private PaginationNode? paginationBar;

		private TSectionSlot[]? sectionSlots;

		private NPCRowNode? selectedSlot;

		protected abstract int SectionCount { get; }

		protected abstract int SectionsPerPage { get; }

		protected abstract uint HeaderItemID { get; }

		[SetsRequiredMembers]
		protected AddonNPCShopsBase(string internalName, ReadOnlySeString title)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			base.InternalName = internalName;
			base.Title = title;
			base.Size = new Vector2(700f, 550f);
		}

		protected abstract void BuildSections();

		protected abstract TSectionSlot CreateSectionSlot(VerticalListNode parent);

		protected abstract void UpdateSectionContent(TSectionSlot slot, int index);

		protected abstract float CalculateSectionHeight(TSectionSlot slot, int visibleNPCCount, bool hasNPCPagination);

		protected unsafe override void OnSetup(AtkUnitBase* addon, Span<AtkValue> atkValues)
		{
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Expected O, but got Unknown
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			BuildSections();
			totalPages = Math.Max(1, (int)Math.Ceiling((double)SectionCount / (double)SectionsPerPage));
			currentPage = 0;
			bool flag = totalPages > 1;
			VerticalListNode verticalListNode = new VerticalListNode
			{
				Size = new Vector2(base.ContentSize.X - 16f, 42f),
				Position = base.ContentStartPosition + new Vector2(8f, 2f),
				ItemSpacing = 4f
			};
			verticalListNode.AttachNode(this);
			HorizontalListNode horizontalListNode = new HorizontalListNode
			{
				Size = new Vector2(verticalListNode.Width, 48f),
				ItemSpacing = 4f
			};
			verticalListNode.AddNode(horizontalListNode);
			ItemIconNode node = new ItemIconNode
			{
				ItemID = HeaderItemID,
				Size = new Vector2(50f),
				OnClick = (ReceiveEvent)delegate(AtkEventListener* _, AtkEventType _, int _, AtkEvent* _, AtkEventData* data)
				{
					if (AtkEventDataExtension.get_IsRightClick(in *data))
					{
						OmenServiceBase<ContextMenuManager>.Instance().OpenItem(HeaderItemID, base.AddonId);
					}
				}
			};
			horizontalListNode.AddNode(node);
			TextNode node2 = new TextNode
			{
				TextFlags = (TextFlags)200,
				String = ReadOnlySeString.op_Implicit(LuminaWrapper.GetItemName(HeaderItemID)),
				FontSize = 24u,
				Height = 42f,
				AlignmentType = (AlignmentType)3
			};
			AtkColors.Label.ApplyTo(node2);
			horizontalListNode.AddNode(node2);
			paginationBar = new PaginationNode
			{
				IsVisible = flag,
				IsDisplayIndicatorText = true,
				OnPreviousPage = delegate
				{
					ShowPage(currentPage - 1);
				},
				OnNextPage = delegate
				{
					ShowPage(currentPage + 1);
				}
			};
			paginationBar.Position = new Vector2(0f, base.ContentStartPosition.Y + base.ContentSize.Y - paginationBar.Height);
			paginationBar.OnSizeUpdated = CenterPaginationBar;
			paginationBar.AttachNode(this);
			CenterPaginationBar();
			float num = (flag ? (paginationBar.Height + 6f) : 0f);
			ScrollingNode<VerticalListNode> scrollingNode = new ScrollingNode<VerticalListNode>();
			scrollingNode.ContentNode.FitContents = true;
			scrollingNode.ContentNode.ItemSpacing = 6f;
			scrollingNode.Position = base.ContentStartPosition + new Vector2(6f, 48f);
			scrollingNode.Size = base.ContentSize - new Vector2(12f, 48f + num);
			scrollingNode.ScrollSpeed = 100;
			scrollingNode.AutoHideScrollBar = true;
			scrollingAreaNode = scrollingNode;
			scrollingAreaNode.AttachNode(this);
			contentNode = scrollingAreaNode.ContentNode;
			sectionSlots = new TSectionSlot[SectionsPerPage];
			for (int num2 = 0; num2 < SectionsPerPage; num2++)
			{
				sectionSlots[num2] = CreateSectionSlot(contentNode);
				sectionSlots[num2].Container.IsVisible = false;
			}
			ShowPage(0);
		}

		protected static void InitializeSectionSlot(TSectionSlot slot, VerticalListNode parent)
		{
			slot.Container = new ResNode
			{
				Size = new Vector2(parent.Width, 100f)
			};
			parent.AddNode(slot.Container);
			slot.Background = new SimpleNineGridNode
			{
				TexturePath = "ui/uld/EnemyList_hr1.tex",
				TextureCoordinates = new Vector2(96f, 80f),
				TextureSize = new Vector2(24f, 20f),
				Offsets = new Vector4(8f),
				MultiplyColor = new Vector3(0.45f, 0.45f, 0.48f),
				Alpha = 0.28f,
				Size = slot.Container.Size
			};
			slot.Background.AttachNode(slot.Container);
			slot.Content = new VerticalListNode
			{
				Size = slot.Container.Size - new Vector2(20f, 20f),
				Position = new Vector2(10f, 10f),
				ItemSpacing = 4f
			};
			slot.Content.AttachNode(slot.Container);
		}

		protected void InitializeSectionFooter(TSectionSlot slot, float contentWidth)
		{
			slot.NPCRows = new NPCRowNode[5];
			for (int i = 0; i < 5; i++)
			{
				NPCRowNode row = CreateNPCRow(slot.Content, contentWidth);
				row.OnClick = delegate
				{
					NPCDisplayInfo nPCInfo = row.NPCInfo;
					if ((object)nPCInfo != null)
					{
						SelectNPCRow(row);
						OpenMap(nPCInfo.Location, nPCInfo.Name);
					}
				};
				slot.NPCRows[i] = row;
			}
			slot.NPCPaginationBar = new PaginationNode
			{
				IsVisible = false,
				IsDisplayIndicatorText = true,
				OnPreviousPage = delegate
				{
					slot.NPCCurrentPage--;
					ShowNPCPage(slot, recalculateOuter: true);
				},
				OnNextPage = delegate
				{
					slot.NPCCurrentPage++;
					ShowNPCPage(slot, recalculateOuter: true);
				},
				OnSizeUpdated = delegate
				{
					CenterNPCPaginationBar(slot);
				}
			};
			slot.Content.AddNode(slot.NPCPaginationBar);
		}

		protected static List<NPCDisplayInfo> SortNPCInfos(IEnumerable<NPCDisplayInfo> npcInfos)
		{
			return (from x in npcInfos.Where((NPCDisplayInfo x) => x.Location != null).DistinctBy((NPCDisplayInfo x) => $"{x.Location.TerritoryID}_{x.Name}")
				orderby x.Location.TerritoryID == 282, x.Location.TerritoryID
				select x).ToList();
		}

		protected void ShowPage(int page)
		{
			if (sectionSlots == null || contentNode == null || scrollingAreaNode == null)
			{
				return;
			}
			currentPage = Math.Clamp(page, 0, totalPages - 1);
			int num = currentPage * SectionsPerPage;
			int num2 = Math.Min(SectionCount - num, SectionsPerPage);
			for (int i = 0; i < SectionsPerPage; i++)
			{
				if (i < num2)
				{
					UpdateSectionContent(sectionSlots[i], num + i);
					sectionSlots[i].Container.IsVisible = true;
				}
				else
				{
					sectionSlots[i].Container.IsVisible = false;
				}
			}
			scrollingAreaNode.ScrollBarNode.ScrollPosition = 0f;
			contentNode.RecalculateLayout();
			scrollingAreaNode.RecalculateSizes();
			UpdatePaginationState();
		}

		protected void ShowNPCPage(TSectionSlot slot, bool recalculateOuter = false)
		{
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			int count = slot.SortedNPCInfos.Count;
			int num = Math.Max(1, (int)Math.Ceiling((double)count / 5.0));
			int num2 = (slot.NPCCurrentPage = Math.Clamp(slot.NPCCurrentPage, 0, num - 1));
			List<NPCDisplayInfo> list = slot.SortedNPCInfos.Skip(num2 * 5).Take(5).ToList();
			for (int i = 0; i < 5; i++)
			{
				if (i < list.Count)
				{
					slot.NPCRows[i].SetNPCInfo(list[i]);
					slot.NPCRows[i].IsVisible = true;
				}
				else
				{
					slot.NPCRows[i].IsVisible = false;
				}
			}
			bool flag = count > 5;
			slot.NPCPaginationBar.IsVisible = flag;
			if (flag)
			{
				slot.NPCPaginationBar.PreviousPageButtonNode.IsEnabled = num2 > 0;
				slot.NPCPaginationBar.NextPageButtonNode.IsEnabled = num2 < num - 1;
				slot.NPCPaginationBar.IndicatorTextNode.String = ReadOnlySeString.op_Implicit($"{num2 + 1} / {num}");
			}
			int visibleNPCCount = Math.Min(list.Count, 5);
			float y = CalculateSectionHeight(slot, visibleNPCCount, flag);
			slot.Container.Size = new Vector2(slot.Container.Width, y);
			slot.Background.Size = slot.Container.Size;
			slot.Content.Size = slot.Container.Size - new Vector2(20f, 20f);
			slot.Content.RecalculateLayout();
			CenterNPCPaginationBar(slot);
			if (recalculateOuter && contentNode != null && scrollingAreaNode != null)
			{
				contentNode.RecalculateLayout();
				scrollingAreaNode.RecalculateSizes();
			}
		}

		protected static void CenterNPCPaginationBar(TSectionSlot slot)
		{
			slot.NPCPaginationBar.X = (slot.Content.Width - slot.NPCPaginationBar.Width) / 2f;
		}

		private void SelectNPCRow(NPCRowNode row)
		{
			if (sectionSlots != null)
			{
				NPCRowNode? nPCRowNode = selectedSlot;
				if (nPCRowNode != null)
				{
					nPCRowNode.Selected = false;
				}
				row.Selected = true;
				selectedSlot = row;
			}
		}

		protected static NPCRowNode CreateNPCRow(VerticalListNode parent, float contentWidth)
		{
			NPCRowNode nPCRowNode = new NPCRowNode
			{
				Size = new Vector2(contentWidth, 32f),
				IsVisible = false
			};
			parent.AddNode(nPCRowNode);
			return nPCRowNode;
		}

		private void UpdatePaginationState()
		{
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			if (paginationBar != null)
			{
				paginationBar.PreviousPageButtonNode.IsEnabled = currentPage > 0;
				paginationBar.NextPageButtonNode.IsEnabled = currentPage < totalPages - 1;
				paginationBar.IndicatorTextNode.String = ReadOnlySeString.op_Implicit($"{currentPage + 1}/{totalPages}");
			}
		}

		private void CenterPaginationBar()
		{
			PaginationNode paginationNode = paginationBar;
			if (paginationNode != null)
			{
				paginationNode.X = base.ContentStartPosition.X + (base.ContentSize.X - paginationNode.Width) / 2f;
			}
		}
	}

	private sealed class SourceContextMenu : ContextMenuEntry
	{
		public override string Identifier => "AutoShowItemNPCShopInfo";

		public override ContextMenuItem? Create(ContextMenuOpenedArgs args)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			if (!args.TargetItemRow.IsValid || ItemSourceInfo.Query(args.TargetItemRow.RowId).State != ItemSourceQueryState.Ready)
			{
				return null;
			}
			return new ContextMenuItem
			{
				Name = ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoShowItemNPCShopInfo-ContextMenu-Source")),
				OnClicked = delegate(ContextMenuItemClickedArgs clicked)
				{
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_000b: Unknown result type (might be due to invalid IL or missing references)
					OpenShopInfoByItemID(clicked.Source.TargetItemRow.RowId);
				}
			};
		}
	}

	private sealed class DestinationContextMenu : ContextMenuEntry
	{
		public override string Identifier => "AutoShowItemNPCShopInfo_Exchange";

		public override ContextMenuItem? Create(ContextMenuOpenedArgs args)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			if (!args.TargetItemRow.IsValid || ItemSourceInfo.QueryExchangeItems(args.TargetItemRow.RowId).State != ItemSourceQueryState.Ready)
			{
				return null;
			}
			return new ContextMenuItem
			{
				Name = ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoShowItemNPCShopInfo-ContextMenu-Destination")),
				OnClicked = delegate(ContextMenuItemClickedArgs clicked)
				{
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_000b: Unknown result type (might be due to invalid IL or missing references)
					OpenExchangeInfoByItemID(clicked.Source.TargetItemRow.RowId);
				}
			};
		}
	}

	private class AddonNPCShopsDestination : AddonNPCShopsBase<AddonNPCShopsDestination.SectionSlot>
	{
		public new class SectionSlot : AddonNPCShopsBase<SectionSlot>.SectionSlot
		{
			public HorizontalListNode ItemHeader;

			public ItemIconNode ItemIcon;

			public TextNode ItemName;

			public HorizontalListNode CostRow;

			public ItemIconNode[] CostIcons;

			public TextNode[] CostTexts;
		}

		private const int ITEMS_PER_PAGE = 20;

		private List<ExchangeItemInfo> exchangeItems = new List<ExchangeItemInfo>();

		public static AddonNPCShopsDestination? Addon { get; set; }

		public ExchangeItemsInfo SourceInfo { get; set; }

		protected override int SectionCount => exchangeItems.Count;

		protected override int SectionsPerPage => 20;

		protected override uint HeaderItemID => SourceInfo.CostItemID;

		[SetsRequiredMembers]
		private AddonNPCShopsDestination(ExchangeItemsInfo sourceInfo)
			: base("DRNPCShopsDestinations", ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoShowItemNPCShopInfo-Addon-Destination")))
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			SourceInfo = sourceInfo;
		}

		public static void CloseAndClear()
		{
			if (Addon != null)
			{
				Addon.Dispose();
				Addon = null;
			}
		}

		public static void OpenWithData(ExchangeItemsInfo sourceInfo)
		{
			if (sourceInfo == null)
			{
				return;
			}
			List<ExchangeItemInfo> items = sourceInfo.Items;
			if (items != null && items.Count > 0)
			{
				CloseAndClear();
				if (Addon == null)
				{
					Addon = new AddonNPCShopsDestination(sourceInfo);
				}
				Addon.Open();
			}
		}

		protected override void BuildSections()
		{
			exchangeItems = SourceInfo.Items.OrderBy((ExchangeItemInfo x) => x.GetItemName()).ToList();
		}

		protected override void UpdateSectionContent(SectionSlot slot, int index)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			ExchangeItemInfo exchangeItemInfo = exchangeItems[index];
			slot.ItemIcon.ItemID = exchangeItemInfo.ItemID;
			slot.ItemName.String = ReadOnlySeString.op_Implicit(exchangeItemInfo.GetItemName());
			slot.ItemName.FontSize = 16u;
			List<AddonNPCShopsBase<SectionSlot>.NPCDisplayInfo> list = new List<AddonNPCShopsBase<SectionSlot>.NPCDisplayInfo>();
			list.AddRange(exchangeItemInfo.NPCInfos.Select((ExchangeItemNPCInfo x) => new AddonNPCShopsBase<SectionSlot>.NPCDisplayInfo(x.Name, x.Location, x.CostInfos)));
			slot.SortedNPCInfos = AddonNPCShopsBase<SectionSlot>.SortNPCInfos(new global::_003C_003Ez__ReadOnlyList<AddonNPCShopsBase<SectionSlot>.NPCDisplayInfo>(list));
			AddonNPCShopsBase<SectionSlot>.NPCDisplayInfo nPCDisplayInfo = slot.SortedNPCInfos.FirstOrDefault();
			int num;
			if ((object)nPCDisplayInfo != null)
			{
				List<ShopItemCostInfo> costInfos = nPCDisplayInfo.CostInfos;
				if (costInfos != null)
				{
					num = ((costInfos.Count > 0) ? 1 : 0);
					goto IL_00ab;
				}
			}
			num = 0;
			goto IL_00ab;
			IL_00ab:
			bool flag = (byte)num != 0;
			slot.CostRow.IsVisible = flag;
			if (flag)
			{
				UpdateCostRow(slot, nPCDisplayInfo.CostInfos);
			}
			LayoutCostRow(slot);
			slot.NPCCurrentPage = 0;
			ShowNPCPage(slot);
		}

		protected override float CalculateSectionHeight(SectionSlot slot, int visibleNPCCount, bool hasNPCPagination)
		{
			float num = (hasNPCPagination ? 32f : 0f);
			return 58f + (float)visibleNPCCount * 32f + (float)Math.Max(0, visibleNPCCount - 1) * 4f + num;
		}

		protected unsafe override SectionSlot CreateSectionSlot(VerticalListNode parent)
		{
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Expected O, but got Unknown
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Expected O, but got Unknown
			SectionSlot slot = new SectionSlot();
			AddonNPCShopsBase<SectionSlot>.InitializeSectionSlot(slot, parent);
			float width = slot.Content.Width;
			slot.ItemHeader = new HorizontalListNode
			{
				Size = new Vector2(width, 36f),
				ItemSpacing = 6f
			};
			slot.Content.AddNode(slot.ItemHeader);
			slot.ItemIcon = new ItemIconNode
			{
				Size = new Vector2(42f)
			};
			slot.ItemIcon.OnClick = (ReceiveEvent)delegate(AtkEventListener* _, AtkEventType _, int _, AtkEvent* _, AtkEventData* data)
			{
				if (AtkEventDataExtension.get_IsRightClick(in *data))
				{
					OmenServiceBase<ContextMenuManager>.Instance().OpenItem(slot.ItemIcon.ItemID, base.AddonId);
				}
			};
			slot.ItemHeader.AddNode(slot.ItemIcon);
			slot.ItemName = new TextNode
			{
				TextFlags = (TextFlags)200,
				AlignmentType = (AlignmentType)3,
				FontSize = 16u,
				Size = new Vector2(width - slot.ItemIcon.Width - slot.ItemHeader.ItemSpacing, slot.ItemHeader.Height)
			};
			AtkColors.Label.ApplyTo(slot.ItemName);
			slot.ItemHeader.AddNode(slot.ItemName);
			slot.CostRow = new HorizontalListNode
			{
				ItemSpacing = 4f,
				FitToContentWidth = true,
				FitToContentHeight = true,
				IsVisible = false
			};
			slot.CostRow.AttachNode(slot.ItemHeader);
			slot.CostIcons = new ItemIconNode[4];
			slot.CostTexts = new TextNode[4];
			for (int num = 0; num < 4; num++)
			{
				slot.CostIcons[num] = new ItemIconNode
				{
					Size = new Vector2(36f)
				};
				int indexCopy = num;
				slot.CostIcons[num].OnClick = (ReceiveEvent)delegate(AtkEventListener* _, AtkEventType _, int _, AtkEvent* _, AtkEventData* data)
				{
					if (AtkEventDataExtension.get_IsRightClick(in *data))
					{
						OmenServiceBase<ContextMenuManager>.Instance().OpenItem(slot.CostIcons[indexCopy].ItemID, base.AddonId);
					}
				};
				slot.CostRow.AddNode(slot.CostIcons[num]);
				slot.CostRow.AddDummy();
				slot.CostTexts[num] = new TextNode
				{
					FontSize = 14u,
					TextFlags = (TextFlags)9,
					AlignmentType = (AlignmentType)3,
					Height = 28f
				};
				AtkColors.ValueEmphasize.ApplyTo(slot.CostTexts[num]);
				slot.CostRow.AddNode(slot.CostTexts[num]);
			}
			InitializeSectionFooter(slot, width);
			return slot;
		}

		private static void UpdateCostRow(SectionSlot slot, List<ShopItemCostInfo> costInfos)
		{
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < 4; i++)
			{
				if (i < costInfos.Count)
				{
					ShopItemCostInfo shopItemCostInfo = costInfos[i];
					slot.CostIcons[i].ItemID = shopItemCostInfo.ItemID;
					slot.CostIcons[i].IsVisible = true;
					slot.CostTexts[i].String = ReadOnlySeString.op_Implicit("x" + shopItemCostInfo.Cost.ToChineseString());
					slot.CostTexts[i].IsVisible = true;
				}
				else
				{
					slot.CostIcons[i].IsVisible = false;
					slot.CostTexts[i].IsVisible = false;
				}
			}
		}

		private static void LayoutCostRow(SectionSlot slot)
		{
			slot.CostRow.RecalculateLayout();
			slot.CostRow.X = slot.ItemHeader.Width - slot.CostRow.Width;
			float num = (slot.CostRow.IsVisible ? (slot.CostRow.Width + slot.ItemHeader.ItemSpacing) : 0f);
			slot.ItemName.Width = slot.ItemHeader.Width - slot.ItemIcon.Width - slot.ItemHeader.ItemSpacing - num;
			slot.ItemHeader.RecalculateLayout();
		}
	}

	private class AddonNPCShopsSource : AddonNPCShopsBase<AddonNPCShopsSource.SectionSlot>
	{
		public new class SectionSlot : AddonNPCShopsBase<SectionSlot>.SectionSlot
		{
			public HorizontalListNode[] CostRows;

			public ItemIconNode[] CostIcons;

			public TextNode[] CostNames;

			public TextNode[] CostQuantities;
		}

		private class CostGroup
		{
			public List<ShopItemCostInfo> CostInfos = new List<ShopItemCostInfo>();

			public List<ShopNPCInfos> NPCInfos = new List<ShopNPCInfos>();
		}

		private const int SECTIONS_PER_PAGE = 10;

		private const float COST_ROW_HEIGHT = 36f;

		private const float NPC_ROW_HEIGHT = 32f;

		private const float VERTICAL_PADDING = 20f;

		private List<CostGroup> costGroups = new List<CostGroup>();

		public static AddonNPCShopsSource? Addon { get; set; }

		public ItemSourceInfo SourceInfo { get; set; }

		protected override int SectionCount => costGroups.Count;

		protected override int SectionsPerPage => 10;

		protected override uint HeaderItemID => SourceInfo.ItemID;

		[SetsRequiredMembers]
		private AddonNPCShopsSource(ItemSourceInfo sourceInfo)
			: base("DRNPCShopsSource", ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoShowItemNPCShopInfo-Addon-Source")))
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			SourceInfo = sourceInfo;
		}

		public static void CloseAndClear()
		{
			if (Addon != null)
			{
				Addon.Dispose();
				Addon = null;
			}
		}

		public static void OpenWithData(ItemSourceInfo sourceInfo)
		{
			if (sourceInfo == null)
			{
				return;
			}
			List<ShopNPCInfos> nPCInfos = sourceInfo.NPCInfos;
			if (nPCInfos != null && nPCInfos.Count > 0)
			{
				CloseAndClear();
				if (Addon == null)
				{
					Addon = new AddonNPCShopsSource(sourceInfo);
				}
				Addon.Open();
			}
		}

		protected override void BuildSections()
		{
			costGroups = (from x in SourceInfo.NPCInfos
				group x by GetCostKey(x.CostInfos) into g
				select new CostGroup
				{
					CostInfos = g.First().CostInfos,
					NPCInfos = g.ToList()
				}).ToList();
		}

		protected override void UpdateSectionContent(SectionSlot slot, int index)
		{
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			CostGroup costGroup = costGroups[index];
			for (int i = 0; i < 4; i++)
			{
				if (i < costGroup.CostInfos.Count)
				{
					ShopItemCostInfo shopItemCostInfo = costGroup.CostInfos[i];
					slot.CostRows[i].IsVisible = true;
					slot.CostIcons[i].ItemID = shopItemCostInfo.ItemID;
					slot.CostNames[i].String = ReadOnlySeString.op_Implicit(shopItemCostInfo.GetItemName());
					slot.CostQuantities[i].String = ReadOnlySeString.op_Implicit(shopItemCostInfo.Collectablity.HasValue ? $"\ue03d ({shopItemCostInfo.Collectablity.Value}~)" : ("x" + shopItemCostInfo.Cost.ToChineseString()));
				}
				else
				{
					slot.CostRows[i].IsVisible = false;
				}
			}
			List<AddonNPCShopsBase<SectionSlot>.NPCDisplayInfo> list = new List<AddonNPCShopsBase<SectionSlot>.NPCDisplayInfo>();
			list.AddRange(costGroup.NPCInfos.Select((ShopNPCInfos x) => new AddonNPCShopsBase<SectionSlot>.NPCDisplayInfo(x.Name, x.Location, x.CostInfos)));
			slot.SortedNPCInfos = AddonNPCShopsBase<SectionSlot>.SortNPCInfos(new global::_003C_003Ez__ReadOnlyList<AddonNPCShopsBase<SectionSlot>.NPCDisplayInfo>(list));
			slot.NPCCurrentPage = 0;
			ShowNPCPage(slot);
		}

		protected override float CalculateSectionHeight(SectionSlot slot, int visibleNPCCount, bool hasNPCPagination)
		{
			int num = Math.Min(slot.SortedNPCInfos.First().CostInfos.Count, 4);
			float num2 = (hasNPCPagination ? 32f : 0f);
			return 20f + (float)num * 36f + (float)Math.Max(0, num - 1) * 4f + ((num > 0) ? 4f : 0f) + (float)visibleNPCCount * 32f + (float)Math.Max(0, visibleNPCCount - 1) * 4f + num2;
		}

		protected unsafe override SectionSlot CreateSectionSlot(VerticalListNode parent)
		{
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Expected O, but got Unknown
			SectionSlot slot = new SectionSlot();
			AddonNPCShopsBase<SectionSlot>.InitializeSectionSlot(slot, parent);
			float width = slot.Content.Width;
			slot.CostRows = new HorizontalListNode[4];
			slot.CostIcons = new ItemIconNode[4];
			slot.CostNames = new TextNode[4];
			slot.CostQuantities = new TextNode[4];
			for (int i = 0; i < 4; i++)
			{
				slot.CostRows[i] = new HorizontalListNode
				{
					Size = new Vector2(width, 36f),
					ItemSpacing = 6f
				};
				slot.Content.AddNode(slot.CostRows[i]);
				slot.CostIcons[i] = new ItemIconNode
				{
					Size = new Vector2(42f)
				};
				int indexCopy = i;
				slot.CostIcons[i].OnClick = (ReceiveEvent)delegate(AtkEventListener* _, AtkEventType _, int _, AtkEvent* _, AtkEventData* data)
				{
					if (AtkEventDataExtension.get_IsRightClick(in *data))
					{
						OmenServiceBase<ContextMenuManager>.Instance().OpenItem(slot.CostIcons[indexCopy].ItemID, base.AddonId);
					}
				};
				slot.CostRows[i].AddNode(slot.CostIcons[i]);
				slot.CostNames[i] = new TextNode
				{
					TextFlags = (TextFlags)200,
					FontSize = 16u,
					AlignmentType = (AlignmentType)3,
					Size = new Vector2(width - 32f - 6f - 38f, 34f)
				};
				AtkColors.Label.ApplyTo(slot.CostNames[i]);
				slot.CostRows[i].AddNode(slot.CostNames[i]);
				slot.CostQuantities[i] = new TextNode
				{
					TextFlags = (TextFlags)9,
					FontSize = 16u,
					Position = new Vector2(width - 10f, 5f),
					Size = new Vector2(70f, 20f),
					AlignmentType = (AlignmentType)5
				};
				AtkColors.ValueEmphasize.ApplyTo(slot.CostQuantities[i]);
				slot.CostQuantities[i].AttachNode(slot.CostRows[i]);
			}
			InitializeSectionFooter(slot, width);
			return slot;
		}

		private static string GetCostKey(List<ShopItemCostInfo> costInfos)
		{
			return string.Join("|", costInfos.Select((ShopItemCostInfo c) => $"{c.ItemID}:{c.Cost}:{c.Collectablity}"));
		}
	}

	private SourceContextMenu sourceContextMenu;

	private DestinationContextMenu destinationContextMenu;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoShowItemNPCShopInfoTitle"),
		Description = LanguageManager.Get("AutoShowItemNPCShopInfoDescription"),
		Category = ModuleCategory.Interface,
		PreviewImageURL = new string[2] { "hxxps://gh.atmoomen.top/raw.githubusercontent.com/Dalamud-DailyRoutines/DailyRoutines/main/Resources/Modules/AutoShowItemNPCShopInfo/AutoShowItemNPCShopInfo-UI.png", "hxxps://gh.atmoomen.top/raw.githubusercontent.com/Dalamud-DailyRoutines/DailyRoutines/main/Resources/Modules/AutoShowItemNPCShopInfo/AutoShowItemNPCShopInfo-Tooltip.png" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected override void Init()
	{
		sourceContextMenu = new SourceContextMenu();
		destinationContextMenu = new DestinationContextMenu();
		OmenServiceBase<ContextMenuManager>.Instance().Reg(sourceContextMenu);
		OmenServiceBase<ContextMenuManager>.Instance().Reg(destinationContextMenu);
		OmenServiceBase<TooltipManager>.Instance().RegItem(OnItemTooltipUpdate);
	}

	protected override void Uninit()
	{
		OmenServiceBase<ContextMenuManager>.Instance().Unreg(sourceContextMenu);
		OmenServiceBase<ContextMenuManager>.Instance().Unreg(destinationContextMenu);
		OmenServiceBase<TooltipManager>.Instance().Unreg(OnItemTooltipUpdate);
		AddonNPCShopsSource.Addon?.Dispose();
		AddonNPCShopsSource.Addon = null;
		AddonNPCShopsDestination.Addon?.Dispose();
		AddonNPCShopsDestination.Addon = null;
	}

	private unsafe static void OnItemTooltipUpdate(ItemKind kind, uint itemID, ref List<TooltipItemModification> modifications)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Invalid comparison between Unknown and I4
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Invalid comparison between Unknown and I4
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		modifications.Add(new TooltipItemModification
		{
			Target = TooltipItemType.ShopInfo,
			Type = TooltipModificationType.Contribute,
			Text = default(ReadOnlySeString)
		});
		if (((int)kind == 500000 || (int)kind == 2000000) ? true : false)
		{
			return;
		}
		RentedSeStringBuilder val = default(RentedSeStringBuilder);
		((RentedSeStringBuilder)(ref val))._002Ector();
		try
		{
			bool flag = false;
			ItemSourceQueryResult itemSourceQueryResult = ItemSourceInfo.Query(itemID);
			SeStringInterpolatedStringHandler val2;
			if (itemSourceQueryResult.State == ItemSourceQueryState.Ready)
			{
				ItemSourceInfo data = itemSourceQueryResult.Data;
				if (data != null)
				{
					flag = true;
					SeStringBuilder builder = ((RentedSeStringBuilder)(ref val)).Builder;
					SeStringBuilder obj = builder;
					val2 = new SeStringInterpolatedStringHandler(2, 1, builder);
					((SeStringInterpolatedStringHandler)(ref val2)).AppendLiteral("[");
					((SeStringInterpolatedStringHandler)(ref val2)).AppendFormatted<string>(LanguageManager.Get("AutoShowItemNPCShopInfo-Tooltip-Source"));
					((SeStringInterpolatedStringHandler)(ref val2)).AppendLiteral("]");
					obj.Append(val2);
					List<ShopItemCostInfo> list = data.NPCInfos.SelectMany((ShopNPCInfos x) => x.CostInfos).DistinctBy((ShopItemCostInfo x) => x.ItemID).ToList();
					for (int num = 0; num < list.Count; num++)
					{
						if (num >= 5)
						{
							builder = ((RentedSeStringBuilder)(ref val)).Builder.AppendNewLine();
							SeStringBuilder obj2 = builder;
							val2 = new SeStringInterpolatedStringHandler(0, 2, builder);
							((SeStringInterpolatedStringHandler)(ref val2)).AppendFormatted<string>("    ");
							((SeStringInterpolatedStringHandler)(ref val2)).AppendFormatted<string>(LanguageManager.Get("AutoShowItemNPCShopInfo-Tooltip-Source-More", list.Count - 5));
							obj2.Append(val2);
							break;
						}
						ShopItemCostInfo shopItemCostInfo = list[num];
						SeStringBuilder val3 = ((RentedSeStringBuilder)(ref val)).Builder.AppendNewLine();
						SeStringBuilder obj3 = val3;
						val2 = new SeStringInterpolatedStringHandler(0, 2, val3);
						((SeStringInterpolatedStringHandler)(ref val2)).AppendFormatted<string>("    ");
						((SeStringInterpolatedStringHandler)(ref val2)).AppendFormatted<string>(LuminaWrapper.GetItemName(shopItemCostInfo.ItemID));
						builder = obj3.Append(val2).PushColorType(32u);
						SeStringBuilder obj4 = builder;
						val2 = new SeStringInterpolatedStringHandler(1, 1, builder);
						((SeStringInterpolatedStringHandler)(ref val2)).AppendLiteral("x");
						((SeStringInterpolatedStringHandler)(ref val2)).AppendFormatted<string>(shopItemCostInfo.Cost.ToChineseString());
						obj4.Append(val2).PopColorType();
						int num2 = -1;
						if (((CurrencyManager)CurrencyManager.Instance()).HasItem(shopItemCostInfo.ItemID))
						{
							num2 = (int)((CurrencyManager)CurrencyManager.Instance()).GetItemCount(shopItemCostInfo.ItemID);
						}
						else
						{
							uint itemCount = LocalPlayerState.GetItemCount(shopItemCostInfo.ItemID);
							if (itemCount != 0)
							{
								num2 = (int)itemCount;
							}
						}
						if (num2 > -1)
						{
							val3 = ((RentedSeStringBuilder)(ref val)).Builder;
							SeStringBuilder obj5 = val3;
							val2 = new SeStringInterpolatedStringHandler(2, 1, val3);
							((SeStringInterpolatedStringHandler)(ref val2)).AppendLiteral("（");
							((SeStringInterpolatedStringHandler)(ref val2)).AppendFormatted<string>(LanguageManager.Get("Current"));
							((SeStringInterpolatedStringHandler)(ref val2)).AppendLiteral("：");
							builder = obj5.Append(val2).PushColorType((num2 >= shopItemCostInfo.Cost) ? 67u : 17u);
							SeStringBuilder obj6 = builder;
							val2 = new SeStringInterpolatedStringHandler(0, 1, builder);
							((SeStringInterpolatedStringHandler)(ref val2)).AppendFormatted<string>(num2.ToChineseString());
							obj6.Append(val2).PopColorType().Append("）");
						}
					}
				}
			}
			ExchangeItemsQueryResult exchangeItemsQueryResult = ItemSourceInfo.QueryExchangeItems(itemID);
			if (exchangeItemsQueryResult.State == ItemSourceQueryState.Ready)
			{
				ExchangeItemsInfo data2 = exchangeItemsQueryResult.Data;
				if (data2 != null)
				{
					string text = LanguageManager.Get(flag ? "AutoShowItemNPCShopInfo-Tooltip-Destination-More" : "AutoShowItemNPCShopInfo-Tooltip-Destination", data2.Items.Count);
					if (flag)
					{
						((RentedSeStringBuilder)(ref val)).Builder.AppendNewLine();
					}
					SeStringBuilder builder = ((RentedSeStringBuilder)(ref val)).Builder;
					SeStringBuilder obj7 = builder;
					val2 = new SeStringInterpolatedStringHandler(0, 2, builder);
					((SeStringInterpolatedStringHandler)(ref val2)).AppendFormatted<string>(flag ? "    " : string.Empty);
					((SeStringInterpolatedStringHandler)(ref val2)).AppendFormatted<string>(text);
					obj7.Append(val2);
					flag = true;
				}
			}
			if (flag)
			{
				modifications.Add(new TooltipItemModification
				{
					Target = TooltipItemType.Description,
					Type = TooltipModificationType.Append,
					Text = ((RentedSeStringBuilder)(ref val)).Builder.ToReadOnlySeString()
				});
			}
		}
		finally
		{
			((IDisposable)(*(RentedSeStringBuilder*)(&val))/*cast due to constrained. prefix*/).Dispose();
		}
	}

	private unsafe static void OpenMap(ShopNPCLocation location, string npcName)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		Vector3 vector = PositionHelper.MapToWorld(new Vector2(location.MapPosition.X, location.MapPosition.Y), location.GetMap()).ToVector3(0f);
		AgentMap* intPtr = AgentMap.Instance();
		((AgentMap)intPtr).SetFlagMapMarker(location.TerritoryID, location.MapID, vector, 60561u);
		((AgentMap)intPtr).OpenMap(location.MapID, location.TerritoryID, npcName, (MapType)1);
	}

	private unsafe static void TeleportToLocation(ShopNPCLocation location)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		Vector3 vector = PositionHelper.MapToWorld(new Vector2(location.MapPosition.X, location.MapPosition.Y), location.GetMap()).ToVector3(0f);
		((AgentMap)AgentMap.Instance()).SetFlagMapMarker(location.TerritoryID, location.MapID, vector, 60561u);
		OmenServiceBase<AetheryteRecordManager>.Instance().GetNearestAetheryte(location.TerritoryID, vector)?.TeleportTo();
	}

	[IPCProvider("DailyRoutines.Modules.AutoShowItemNPCShopInfo.OpenByItemID")]
	private static bool OpenByItemID(uint itemID)
	{
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		ItemSourceQueryResult result = ItemSourceInfo.Query(itemID);
		ExchangeItemsQueryResult result2 = ItemSourceInfo.QueryExchangeItems(itemID);
		bool flag = false;
		bool flag2 = false;
		flag |= TryOpenShopInfo(itemID, result, showError: false);
		flag |= TryOpenExchangeInfo(itemID, result2, showError: false);
		ItemSourceQueryState state = result.State;
		if ((state != ItemSourceQueryState.Ready && state != ItemSourceQueryState.NotFound) || 1 == 0)
		{
			flag2 = true;
		}
		state = result2.State;
		if ((state != ItemSourceQueryState.Ready && state != ItemSourceQueryState.NotFound) || 1 == 0)
		{
			flag2 = true;
		}
		if (flag)
		{
			return true;
		}
		if (flag2)
		{
			return false;
		}
		ReadOnlySeString se = LanguageManager.GetSe("AutoShowItemNPCShopInfo-Notification-InfoNotFound", SeString.CreateItemLink(itemID, (ItemKind)0, (string)null));
		OmenServiceBase<NotifyHelper>.Instance().ChatError(se);
		NotifyHelper.Toast(se);
		return false;
	}

	[IPCProvider("DailyRoutines.Modules.AutoShowItemNPCShopInfo.OpenShopInfoByItemID")]
	private static bool OpenShopInfoByItemID(uint itemID)
	{
		return TryOpenShopInfo(itemID, ItemSourceInfo.Query(itemID), showError: true);
	}

	[IPCProvider("DailyRoutines.Modules.AutoShowItemNPCShopInfo.OpenExchangeInfoByItemID")]
	private static bool OpenExchangeInfoByItemID(uint itemID)
	{
		return TryOpenExchangeInfo(itemID, ItemSourceInfo.QueryExchangeItems(itemID), showError: true);
	}

	private static bool TryOpenShopInfo(uint itemID, ItemSourceQueryResult result, bool showError)
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		AddonNPCShopsSource addon = AddonNPCShopsSource.Addon;
		if (addon != null && addon.IsOpen && addon.SourceInfo.ItemID == itemID)
		{
			AddonNPCShopsSource.CloseAndClear();
			return true;
		}
		if (result.State == ItemSourceQueryState.Ready)
		{
			ItemSourceInfo data = result.Data;
			if (data != null)
			{
				AddonNPCShopsSource.OpenWithData(data);
				return true;
			}
		}
		if (showError && result.State == ItemSourceQueryState.NotFound)
		{
			ReadOnlySeString se = LanguageManager.GetSe("AutoShowItemNPCShopInfo-Notification-InfoNotFound", SeString.CreateItemLink(itemID, (ItemKind)0, (string)null));
			OmenServiceBase<NotifyHelper>.Instance().ChatError(se);
			NotifyHelper.Toast(se);
		}
		return false;
	}

	private static bool TryOpenExchangeInfo(uint itemID, ExchangeItemsQueryResult result, bool showError)
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		AddonNPCShopsDestination addon = AddonNPCShopsDestination.Addon;
		if (addon != null && addon.IsOpen && addon.SourceInfo.CostItemID == itemID)
		{
			AddonNPCShopsDestination.CloseAndClear();
			return true;
		}
		if (result.State == ItemSourceQueryState.Ready)
		{
			ExchangeItemsInfo data = result.Data;
			if (data != null)
			{
				AddonNPCShopsDestination.OpenWithData(data);
				return true;
			}
		}
		if (showError && result.State == ItemSourceQueryState.NotFound)
		{
			ReadOnlySeString se = LanguageManager.GetSe("AutoShowItemNPCShopInfo-Notification-InfoNotFound", SeString.CreateItemLink(itemID, (ItemKind)0, (string)null));
			OmenServiceBase<NotifyHelper>.Instance().ChatError(se);
			NotifyHelper.Toast(se);
		}
		return false;
	}
}
