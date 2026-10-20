using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Info;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Game;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.Text.Evaluator;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.Lumina;
using OmenTools.KamiToolKit.Addons;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.ModulesPublic.Interface;

public class OptimizedSalvage : ModuleBase
{
	private class OperationAddon(OptimizedSalvage module) : AttachedAddon("SalvageItemSelector")
	{
		public TextButtonNode? OperationButton { get; private set; }

		protected unsafe override void OnSetup(AtkUnitBase* addon, Span<AtkValue> atkValueSpan)
		{
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			VerticalListNode verticalListNode = new VerticalListNode
			{
				FitContents = true,
				Width = base.ContentSize.X,
				Position = base.ContentStartPosition,
				ItemSpacing = 2f
			};
			verticalListNode.AttachNode(this);
			verticalListNode.AddDummy(5f);
			OperationButton = new TextButtonNode
			{
				TextureType = ButtonTextureType.ButtonB,
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("OptimizedSalvage-BatchDesynthesize")),
				Size = new Vector2(verticalListNode.Width, 36f),
				OnClick = delegate
				{
					if (module.TaskHelper.IsBusy)
					{
						module.TaskHelper.Abort();
					}
					else
					{
						module.TaskHelper.Enqueue(() => module.EnqueueDesynthesize());
					}
				}
			};
			verticalListNode.AddNode(OperationButton);
			CheckboxNode node = new CheckboxNode
			{
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("SkipHQItem")),
				Size = new Vector2(verticalListNode.Width, 28f),
				IsChecked = module.config.SkipHQ,
				OnClick = delegate(bool value)
				{
					module.config.SkipHQ = value;
					module.config.Save(module);
				}
			};
			verticalListNode.AddNode(node);
			TextNode node2 = new TextNode
			{
				FontSize = 12u,
				TextFlags = (TextFlags)193,
				Width = verticalListNode.Width,
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("OptimizedSalvage-Hint-OnlyBatch"))
			};
			AtkColors.Hint.ApplyTo(node2);
			verticalListNode.AddNode(node2);
			verticalListNode.AddDummy(2f);
			CheckboxNode node3 = new CheckboxNode
			{
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("OptimizedSalvage-ConfirmWhenManual")),
				Size = new Vector2(verticalListNode.Width, 28f),
				IsChecked = module.config.ConfirmWhenManual,
				OnClick = delegate(bool value)
				{
					module.config.ConfirmWhenManual = value;
					module.config.Save(module);
				}
			};
			verticalListNode.AddNode(node3);
			TextNode node4 = new TextNode
			{
				FontSize = 12u,
				Width = verticalListNode.Width,
				TextFlags = (TextFlags)193,
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("OptimizedSalvage-Hint-OnlyManual"))
			};
			AtkColors.Hint.ApplyTo(node4);
			verticalListNode.AddNode(node4);
			verticalListNode.RecalculateLayout();
			SetWindowSize(base.Size.X, verticalListNode.Height + base.ContentStartPosition.Y + 24f);
			verticalListNode.Position = base.ContentStartPosition;
		}
	}

	private class Config : ModuleConfig
	{
		public bool SkipHQ = true;

		public bool ConfirmWhenManual = true;
	}

	private sealed class SkillColumnEntry
	{
		public required TextNode Node { get; init; }

		public required Vector4 DefaultColor { get; init; }

		public string Text { get; set; } = string.Empty;
	}

	private OperationAddon? addon;

	private Config config;

	private const ushort COLOR_LEVEL_TOO_HIGH = 32;

	private const ushort COLOR_LEVEL_HIGH = 45;

	private const ushort SKILL_COLUMN_WIDTH = 120;

	private readonly Dictionary<nint, SkillColumnEntry> skillColumnNodes = new Dictionary<nint, SkillColumnEntry>();

	private TextNode? skillColumnHeader;

	private unsafe AtkComponentList* skillListComponent;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("OptimizedSalvageTitle"),
		Description = LanguageManager.Get("OptimizedSalvageDescription"),
		Category = ModuleCategory.Interface,
		PreviewImageURL = new string[1] { "hxxps://gh.atmoomen.top/raw.githubusercontent.com/Dalamud-DailyRoutines/DailyRoutines/main/Resources/Modules/OptimizedSalvage/preview-1.png" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected override void Init()
	{
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Expected O, but got Unknown
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Expected O, but got Unknown
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Expected O, but got Unknown
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Expected O, but got Unknown
		config = LoadConfig<Config>() ?? new Config();
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				EnterBusyAction = delegate
				{
					//IL_001f: Unknown result type (might be due to invalid IL or missing references)
					OperationAddon? operationAddon = addon;
					if (operationAddon != null)
					{
						TextButtonNode? operationButton = operationAddon.OperationButton;
						if (operationButton != null)
						{
							operationButton.String = ReadOnlySeString.op_Implicit(LanguageManager.Get("Stop"));
						}
					}
				},
				LeaveBusyAction = delegate
				{
					//IL_001f: Unknown result type (might be due to invalid IL or missing references)
					OperationAddon? operationAddon = addon;
					if (operationAddon != null)
					{
						TextButtonNode? operationButton = operationAddon.OperationButton;
						if (operationButton != null)
						{
							operationButton.String = ReadOnlySeString.op_Implicit(LanguageManager.Get("OptimizedSalvage-BatchDesynthesize"));
						}
					}
				}
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		OmenServiceBase<TooltipManager>.Instance().RegItem(OnItemTooltip);
		addon = new OperationAddon(this)
		{
			InternalName = "DROptimizedSalvage",
			Title = ReadOnlySeString.op_Implicit(Info.Title),
			Size = new Vector2(300f, 280f)
		};
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)1, "SalvageDialog", new AddonEventDelegate(OnAddon));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)1, "SalvageItemSelector", new AddonEventDelegate(OnSalvageItemSelector));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)5, "SalvageItemSelector", new AddonEventDelegate(OnSalvageItemSelector));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, "SalvageItemSelector", new AddonEventDelegate(OnSalvageItemSelector));
	}

	protected override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddon });
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnSalvageItemSelector });
		OmenServiceBase<TooltipManager>.Instance().Unreg(OnItemTooltip);
		RemoveSkillColumn();
		addon?.Dispose();
		addon = null;
	}

	private unsafe void OnAddon(AddonEvent type, AddonArgs args)
	{
		if ((config.ConfirmWhenManual || base.TaskHelper.IsBusy) && Throttler.Shared.Throttle("AutoDesynthesizeItems.Confirm", 100u))
		{
			(*Addons.SalvageDialog).Callback(13, true);
			(*Addons.SalvageDialog).Callback(0, 0);
		}
	}

	private unsafe bool EnqueueDesynthesize(uint batchCount = 0u)
	{
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		if (IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()))
		{
			return false;
		}
		if (!(*Addons.SalvageItemSelector).IsAddonAndNodesReady())
		{
			return false;
		}
		if (Inventories.Player.IsFull(3u))
		{
			((RaptureLogModule)RaptureLogModule.Instance()).ShowLogMessage(3974u);
			base.TaskHelper.Abort();
			return true;
		}
		AgentSalvage* ptr = AgentSalvage.Instance();
		if (ptr == null)
		{
			return false;
		}
		if (((AgentSalvage)ptr).ItemCount == 0)
		{
			base.TaskHelper.Abort();
			NotifyCount();
			return true;
		}
		for (int i = 0; i < ((AgentSalvage)ptr).ItemCount; i++)
		{
			SalvageListItem val = ((SalvageListItem*)((AgentSalvage)ptr).ItemList)[i];
			if (val.ItemId == 0)
			{
				continue;
			}
			InventoryItem* inventorySlot = ((InventoryManager)InventoryManager.Instance()).GetInventorySlot(val.InventoryType, (int)val.InventorySlot);
			if (inventorySlot != null && ((InventoryItem)inventorySlot).GetBaseItemId() != 0 && (!config.SkipHQ || !((InventoryItem)inventorySlot).IsHighQuality()))
			{
				((AgentId)129).SendEvent(0uL, 12, i);
				batchCount++;
				base.TaskHelper.Enqueue(() => EnqueueDesynthesize(batchCount));
				return true;
			}
		}
		NotifyCount();
		base.TaskHelper.Abort();
		return true;
		void NotifyCount()
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			string text = LanguageManager.Get("OptimizedSalvage-Notification-BatchDesynthesized", new Dictionary<string, object> { ["count"] = batchCount });
			NotifyHelper.Toast(text);
			OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(text));
		}
	}

	private unsafe void OnSalvageItemSelector(AddonEvent type, AddonArgs args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Invalid comparison between Unknown and I4
		if ((int)type != 1 && (int)type != 5)
		{
			if ((int)type == 6)
			{
				skillColumnHeader = null;
				skillListComponent = null;
				skillColumnNodes.Clear();
			}
		}
		else
		{
			RefreshSkillColumn();
		}
	}

	private unsafe void RefreshSkillColumn()
	{
		if (!(*Addons.SalvageItemSelector).IsAddonAndNodesReady())
		{
			return;
		}
		if (skillListComponent == null)
		{
			CreateSkillColumn(Addons.SalvageItemSelector);
		}
		if (skillListComponent == null)
		{
			return;
		}
		for (int i = 0; i < ((AtkComponentList)skillListComponent).AllocatedItemRendererListLength; i++)
		{
			AtkComponentListItemRenderer* atkComponentListItemRenderer = ((ListItem)((byte*)((AtkComponentList)skillListComponent).ItemRendererList + (nint)i * (nint)Unsafe.SizeOf<ListItem>())).AtkComponentListItemRenderer;
			if (atkComponentListItemRenderer != null)
			{
				int listItemIndex = ((AtkComponentListItemRenderer)atkComponentListItemRenderer).ListItemIndex;
				if (listItemIndex >= 0 && listItemIndex < ((AtkComponentList)skillListComponent).ListLength)
				{
					UpdateSkillColumn(listItemIndex, atkComponentListItemRenderer);
				}
			}
		}
	}

	private unsafe void CreateSkillColumn(AtkUnitBase* hostAddon)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Invalid comparison between Unknown and I4
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		AtkResNode* nodeById = ((AtkUnitBase)hostAddon).GetNodeById(7u);
		AtkComponentNode* nodeById2 = (AtkComponentNode*)((AtkUnitBase)hostAddon).GetNodeById(12u);
		if (nodeById == null || nodeById2 == null || ((AtkComponentNode)nodeById2).Component == null)
		{
			return;
		}
		AtkResNode* ptr = null;
		for (AtkResNode* ptr2 = ((AtkResNode)nodeById).ChildNode; ptr2 != null; ptr2 = ((AtkResNode)ptr2).PrevSiblingNode)
		{
			if ((int)((AtkResNode)ptr2).GetNodeType() == 3 && (ptr == null || ((AtkResNode)ptr2).GetXShort() > ((AtkResNode)ptr).GetXShort()))
			{
				ptr = ptr2;
			}
		}
		if (ptr != null)
		{
			AtkTextNode* asAtkTextNode = ((AtkResNode)ptr).GetAsAtkTextNode();
			if (asAtkTextNode != null && ApplySkillColumnLayout(hostAddon, 120))
			{
				skillColumnHeader = new TextNode
				{
					Position = new Vector2(((AtkResNode)ptr).GetXShort() + ((AtkResNode)ptr).GetWidth(), ((AtkResNode)ptr).GetYShort()),
					Size = new Vector2(120f, (int)((AtkResNode)ptr).GetHeight()),
					String = ReadOnlySeString.op_Implicit(LanguageManager.Get("OptimizedSalvage-DesynthesisSkill")),
					AlignmentType = ((AtkTextNode)asAtkTextNode).AlignmentType,
					FontType = ((AtkTextNode)asAtkTextNode).FontType,
					FontSize = ((AtkTextNode)asAtkTextNode).FontSize,
					LineSpacing = ((AtkTextNode)asAtkTextNode).LineSpacing,
					TextColor = ((AtkTextNode)asAtkTextNode).TextColor.ToVector4(),
					TextOutlineColor = ((AtkTextNode)asAtkTextNode).EdgeColor.ToVector4(),
					BackgroundColor = ((AtkTextNode)asAtkTextNode).BackgroundColor.ToVector4(),
					TextFlags = ((AtkTextNode)asAtkTextNode).TextFlags
				};
				skillColumnHeader.AttachNode(ptr, NodePosition.AfterTarget);
				skillListComponent = (AtkComponentList*)((AtkComponentNode)nodeById2).Component;
			}
		}
	}

	private unsafe void RemoveSkillColumn()
	{
		skillColumnHeader?.Dispose();
		skillColumnHeader = null;
		foreach (SkillColumnEntry value in skillColumnNodes.Values)
		{
			value.Node.Dispose();
		}
		skillColumnNodes.Clear();
		if (skillListComponent != null)
		{
			if ((*Addons.SalvageItemSelector).IsAddonAndNodesReady())
			{
				ApplySkillColumnLayout(Addons.SalvageItemSelector, -120);
			}
			skillListComponent = null;
		}
	}

	private unsafe static bool ApplySkillColumnLayout(AtkUnitBase* hostAddon, int widthDelta)
	{
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		AtkResNode* nodeById = ((AtkUnitBase)hostAddon).GetNodeById(7u);
		AtkResNode* nodeById2 = ((AtkUnitBase)hostAddon).GetNodeById(11u);
		AtkComponentNode* nodeById3 = (AtkComponentNode*)((AtkUnitBase)hostAddon).GetNodeById(12u);
		AtkComponentNode* nodeById4 = (AtkComponentNode*)((AtkUnitBase)hostAddon).GetNodeById(14u);
		if (nodeById == null || nodeById2 == null || nodeById3 == null || nodeById4 == null)
		{
			return false;
		}
		if (((AtkComponentNode)nodeById3).Component == null || ((AtkComponentNode)nodeById4).Component == null)
		{
			return false;
		}
		((AtkResNode)nodeById).SetWidth((ushort)(((AtkResNode)nodeById).GetWidth() + widthDelta));
		((AtkResNode)nodeById2).SetWidth((ushort)(((AtkResNode)nodeById2).GetWidth() + widthDelta));
		((AtkResNode)((AtkUnitBase)hostAddon).RootNode).SetWidth((ushort)(((AtkResNode)((AtkUnitBase)hostAddon).RootNode).GetWidth() + widthDelta));
		((AtkResNode)nodeById4).SetWidth((ushort)(((AtkResNode)nodeById4).GetWidth() + widthDelta));
		uint[] array = new uint[7] { 2u, 8u, 9u, 10u, 11u, 12u, 13u };
		foreach (uint num in array)
		{
			AtkResNode* ptr = ((AtkUldManager)(&((AtkComponentBase)((AtkComponentNode)nodeById4).Component).UldManager)).SearchNodeById(num);
			if (ptr != null)
			{
				((AtkResNode)ptr).SetWidth((ushort)(((AtkResNode)ptr).GetWidth() + widthDelta));
			}
		}
		array = new uint[3] { 5u, 6u, 7u };
		foreach (uint num2 in array)
		{
			AtkResNode* ptr2 = ((AtkUldManager)(&((AtkComponentBase)((AtkComponentNode)nodeById4).Component).UldManager)).SearchNodeById(num2);
			if (ptr2 != null)
			{
				((AtkResNode)ptr2).SetXShort((short)(((AtkResNode)ptr2).GetXShort() + widthDelta));
			}
		}
		((AtkResNode)nodeById3).SetWidth((ushort)(((AtkResNode)nodeById3).GetWidth() + widthDelta));
		AtkUldManager uldManager = ((AtkComponentBase)((AtkComponentNode)nodeById3).Component).UldManager;
		for (int j = 0; j < uldManager.NodeListCount; j++)
		{
			AtkResNode* ptr3 = uldManager.NodeList[j];
			if (ptr3 != null)
			{
				if (((AtkResNode)ptr3).NodeId == 5)
				{
					((AtkResNode)ptr3).SetXShort((short)(((AtkResNode)ptr3).GetXShort() + widthDelta));
				}
				else
				{
					((AtkResNode)ptr3).SetWidth((ushort)(((AtkResNode)ptr3).GetWidth() + widthDelta));
				}
			}
		}
		return true;
	}

	private unsafe void UpdateSkillColumn(int index, AtkComponentListItemRenderer* listItemRenderer)
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		if (!skillColumnNodes.TryGetValue((nint)listItemRenderer, out SkillColumnEntry value))
		{
			TextNode textNode = CreateSkillColumnNode(listItemRenderer);
			if (textNode == null)
			{
				return;
			}
			value = new SkillColumnEntry
			{
				Node = textNode,
				DefaultColor = textNode.TextColor
			};
			skillColumnNodes[(nint)listItemRenderer] = value;
		}
		var (text, textColor) = GetSkillColumnDisplay(index, value.DefaultColor);
		if (!(value.Text == text))
		{
			value.Text = text;
			value.Node.String = ReadOnlySeString.op_Implicit(text);
			value.Node.TextColor = textColor;
		}
	}

	private unsafe static TextNode? CreateSkillColumnNode(AtkComponentListItemRenderer* listItemRenderer)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		AtkResNode* ptr = ((AtkUldManager)(&((AtkComponentListItemRenderer)listItemRenderer).UldManager)).SearchNodeById(6u);
		if (ptr == null)
		{
			return null;
		}
		AtkTextNode* asAtkTextNode = ((AtkResNode)ptr).GetAsAtkTextNode();
		if (asAtkTextNode == null)
		{
			return null;
		}
		TextNode textNode = new TextNode();
		textNode.Position = new Vector2(((AtkResNode)ptr).GetXShort() + ((AtkResNode)ptr).GetWidth(), ((AtkResNode)ptr).GetYShort());
		textNode.Size = new Vector2(120f, (int)((AtkResNode)ptr).GetHeight());
		textNode.AlignmentType = (AlignmentType)5;
		textNode.FontType = ((AtkTextNode)asAtkTextNode).FontType;
		textNode.FontSize = ((AtkTextNode)asAtkTextNode).FontSize;
		textNode.LineSpacing = ((AtkTextNode)asAtkTextNode).LineSpacing;
		textNode.TextColor = ((AtkTextNode)asAtkTextNode).TextColor.ToVector4();
		textNode.TextOutlineColor = ((AtkTextNode)asAtkTextNode).EdgeColor.ToVector4();
		textNode.BackgroundColor = ((AtkTextNode)asAtkTextNode).BackgroundColor.ToVector4();
		textNode.AttachNode(((AtkUldManager)(&((AtkComponentListItemRenderer)listItemRenderer).UldManager)).RootNode);
		return textNode;
	}

	private static (string Text, Vector4 Color) GetSkillColumnDisplay(int index, Vector4 defaultColor)
	{
		if (!TryGetDesynthesisInfo(index, out var skillLevel, out var itemLevel))
		{
			return (Text: string.Empty, Color: defaultColor);
		}
		Vector4 item = ((skillLevel <= itemLevel) ? defaultColor : ((skillLevel > itemLevel + 50f) ? ColorHelper.GetColor(32u) : ColorHelper.GetColor(45u)));
		IFormatProvider invariantCulture = CultureInfo.InvariantCulture;
		DefaultInterpolatedStringHandler handler = new DefaultInterpolatedStringHandler(1, 2, invariantCulture);
		handler.AppendFormatted(skillLevel, "F2");
		handler.AppendLiteral("/");
		handler.AppendFormatted(itemLevel, "F2");
		return (Text: string.Create(invariantCulture, ref handler), Color: item);
	}

	private unsafe static bool TryGetDesynthesisInfo(int index, out float skillLevel, out float itemLevel)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		skillLevel = 0f;
		itemLevel = 0f;
		AgentSalvage* ptr = AgentSalvage.Instance();
		if (ptr == null)
		{
			return false;
		}
		if (index < 0 || index >= ((AgentSalvage)ptr).ItemCount)
		{
			return false;
		}
		SalvageListItem val = ((SalvageListItem*)((AgentSalvage)ptr).ItemList)[index];
		InventoryItem* inventorySlot = ((InventoryManager)InventoryManager.Instance()).GetInventorySlot(val.InventoryType, (int)val.InventorySlot);
		if (inventorySlot == null)
		{
			return false;
		}
		if (!LuminaGetter.TryGetRow<Item>(((InventoryItem)inventorySlot).GetBaseItemId(), out Item item))
		{
			return false;
		}
		if (((Item)(ref item)).Desynth == 0)
		{
			return false;
		}
		skillLevel = ((PlayerState)PlayerState.Instance()).GetDesynthesisLevel(((Item)(ref item)).ClassJobRepair.RowId);
		itemLevel = ((Item)(ref item)).LevelItem.RowId;
		return true;
	}

	private unsafe static void OnItemTooltip(ItemKind itemKind, uint itemID, ref List<TooltipItemModification> modifications)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		if ((int)itemKind != 0 || !LuminaGetter.TryGetRow<Item>(itemID, out Item item) || ((Item)(ref item)).Desynth == 0)
		{
			return;
		}
		RentedSeStringBuilder val = default(RentedSeStringBuilder);
		((RentedSeStringBuilder)(ref val))._002Ector();
		try
		{
			uint rowId = ((Item)(ref item)).LevelItem.RowId;
			float desynthesisLevel = ((PlayerState)PlayerState.Instance()).GetDesynthesisLevel(((Item)(ref item)).ClassJobRepair.RowId);
			ushort num = 0;
			string text = null;
			if (desynthesisLevel <= (float)rowId)
			{
				num = 0;
			}
			else if (desynthesisLevel > (float)(rowId + 50))
			{
				num = 32;
				text = $"{LanguageManager.Get("OptimizedSalvage-Tag-ChanceRateUp")}{UITexts.Space}{LanguageManager.Get("OptimizedSalvage-Tag-UnableEXPGain")}";
			}
			else
			{
				num = 45;
				text = LanguageManager.Get("OptimizedSalvage-Tag-ChanceRateUp") ?? "";
			}
			((RentedSeStringBuilder)(ref val)).PushColorType((uint)num).Append(desynthesisLevel.ToString("F2", CultureInfo.InvariantCulture)).PopColorType();
			if (text != null)
			{
				((RentedSeStringBuilder)(ref val)).Append(" ［" + text + "］");
			}
			if (((Item)(ref item)).EquipSlotCategory.RowId != 0)
			{
				Addon rowOrDefault = LuminaGetter.GetRowOrDefault<Addon>(1361u);
				ReadOnlySeString text2 = ((Addon)(ref rowOrDefault)).Text;
				string text3 = ((ReadOnlySeString)(ref text2)).ToMacroString();
				text3 = text3.Replace(UITexts.Space.ToString(), "<br>");
				if (((Item)(ref item)).Desynth > 0)
				{
					text3 = text3 + "<br>" + LanguageManager.Get("OptimizedSalvage-Level-Current") + UITexts.Colon + "<string(lstr5)>";
				}
				ISeStringEvaluator obj = DalamudServiceExtension.Instance<ISeStringEvaluator>();
				string text4 = text3;
				InlineArray5<SeStringParameter> buffer = default(InlineArray5<SeStringParameter>);
				buffer[0] = SeStringParameter.op_Implicit((((Item)(ref item)).MaterializeType > 0) ? 1 : 0);
				buffer[1] = SeStringParameter.op_Implicit(((Item)(ref item)).IsGlamorous ? 1 : 0);
				buffer[2] = SeStringParameter.op_Implicit((int)((Item)(ref item)).Desynth);
				buffer[3] = SeStringParameter.op_Implicit(((Item)(ref item)).LevelItem.RowId);
				buffer[4] = SeStringParameter.op_Implicit(((RentedSeStringBuilder)(ref val)).ToReadOnlySeString());
				ReadOnlySeString text5 = obj.EvaluateMacroString(text4, (Span<SeStringParameter>)buffer, (ClientLanguage?)null);
				modifications.Add(new TooltipItemModification
				{
					Target = TooltipItemType.GearAbilityInfo,
					Type = TooltipModificationType.Contribute,
					Text = text5
				});
			}
			else
			{
				ISeStringEvaluator obj2 = DalamudServiceExtension.Instance<ISeStringEvaluator>();
				string text6 = LanguageManager.Get("OptimizedSalvage-MacroString-NonGearDescription");
				InlineArray4<SeStringParameter> buffer2 = default(InlineArray4<SeStringParameter>);
				buffer2[0] = SeStringParameter.op_Implicit(((Item)(ref item)).Description);
				buffer2[1] = SeStringParameter.op_Implicit(rowId);
				buffer2[2] = SeStringParameter.op_Implicit(((Item)(ref item)).ClassJobRepair.RowId);
				buffer2[3] = SeStringParameter.op_Implicit(((RentedSeStringBuilder)(ref val)).ToReadOnlySeString());
				ReadOnlySeString text7 = obj2.EvaluateMacroString(text6, (Span<SeStringParameter>)buffer2, (ClientLanguage?)null);
				modifications.Add(new TooltipItemModification
				{
					Target = TooltipItemType.Description,
					Type = TooltipModificationType.Contribute,
					Text = text7
				});
			}
		}
		finally
		{
			((IDisposable)(*(RentedSeStringBuilder*)(&val))/*cast due to constrained. prefix*/).Dispose();
		}
	}
}
