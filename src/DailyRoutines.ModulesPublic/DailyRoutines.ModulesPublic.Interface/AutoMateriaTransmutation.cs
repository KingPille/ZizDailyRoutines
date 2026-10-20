using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Info.Game.Data;
using OmenTools.Info.Lumina;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models.Native;
using OmenTools.Threading;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;

namespace DailyRoutines.ModulesPublic.Interface;

public class AutoMateriaTransmutation : ModuleBase
{
	private class Config : ModuleConfig
	{
		public HashSet<uint> BlacklistedItems = new HashSet<uint>();
	}

	private Config config;

	private string itemSearchInput = string.Empty;

	private TextButtonNode? operateButtonNode;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoMateriaTransmutationTitle"),
		Description = LanguageManager.Get("AutoMateriaTransmutationDescription"),
		Category = ModuleCategory.Interface,
		ModulesPrerequisite = new string[2] { "AutoCutsceneSkip", "AutoTalkSkip" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		NeedAuth = true
	};

	protected override void Init()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected O, but got Unknown
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 15000
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)5, "TradeMultiple", new AddonEventDelegate(OnAddon));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, "TradeMultiple", new AddonEventDelegate(OnAddon));
	}

	protected override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddon });
		operateButtonNode?.Dispose();
		operateButtonNode = null;
	}

	protected override void ConfigUI()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		ImGui.AlignTextToFramePadding();
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(0, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("AutoMateriaTransmutation-BlacklistMateria"));
		ImGui.TextColored(ref vector, val);
		IndentDisposable val2 = ImRaii.PushIndent(1, true);
		try
		{
			ImGui.SetNextItemWidth(300f * Globals.GlobalUIScale);
			if (ImGuiOmExtension.MultiSelectCombo<Item>("MateriaSelectCombo", (IDictionary<uint, Item>)Sheets.Materias, ref config.BlacklistedItems, ref itemSearchInput, new(string, ImGuiTableColumnFlags, float)[1] { ("物品", (ImGuiTableColumnFlags)4, 0f) }, new Func<Item, Action>[1]
			{
				(Item x) => delegate
				{
					//IL_001b: Unknown result type (might be due to invalid IL or missing references)
					//IL_0020: Unknown result type (might be due to invalid IL or missing references)
					//IL_0034: Unknown result type (might be due to invalid IL or missing references)
					//IL_0049: Unknown result type (might be due to invalid IL or missing references)
					//IL_004e: Unknown result type (might be due to invalid IL or missing references)
					ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
					GameIconLookup val3 = new GameIconLookup((uint)((Item)(ref x)).Icon, false, true, (ClientLanguage?)null);
					IDalamudTextureWrap wrapOrDefault = obj.GetFromGameIcon(ref val3).GetWrapOrDefault((IDalamudTextureWrap)null);
					if (wrapOrDefault != null && ImGuiOm.SelectableImageWithText(wrapOrDefault.Handle, new Vector2(ImGui.GetTextLineHeightWithSpacing()), ((object)((Item)(ref x)).Name/*cast due to constrained. prefix*/).ToString() ?? "", config.BlacklistedItems.Contains(((Item)(ref x)).RowId), (ImGuiSelectableFlags)3) && !config.BlacklistedItems.Remove(((Item)(ref x)).RowId))
					{
						config.BlacklistedItems.Add(((Item)(ref x)).RowId);
						config.Save(this);
					}
				}
			}, new Func<Item, string>[2]
			{
				(Item x) => ((object)((Item)(ref x)).Name/*cast due to constrained. prefix*/).ToString(),
				(Item x) => ((Item)(ref x)).RowId.ToString()
			}, true))
			{
				config.Save(this);
			}
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	private unsafe void OnAddon(AddonEvent type, AddonArgs args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		if ((int)type != 5)
		{
			if ((int)type == 6)
			{
				operateButtonNode = null;
			}
			return;
		}
		AtkUnitBase* tradeMultipleAddon = Addons.TradeMultipleAddon;
		if (tradeMultipleAddon == null)
		{
			return;
		}
		if (operateButtonNode == null)
		{
			((AtkResNode)((AtkUnitBase)tradeMultipleAddon).RootNode).SetWidth((ushort)400);
			((AtkComponentNode)((AtkUnitBase)tradeMultipleAddon).WindowNode).SetWidth((ushort)400);
			for (int i = 0; i < ((AtkUldManager)(&((AtkComponentBase)((AtkComponentNode)((AtkUnitBase)tradeMultipleAddon).WindowNode).Component).UldManager)).NodeListCount; i++)
			{
				AtkResNode* ptr = ((AtkUldManager)(&((AtkComponentBase)((AtkComponentNode)((AtkUnitBase)tradeMultipleAddon).WindowNode).Component).UldManager)).NodeList[i];
				if (ptr != null)
				{
					if (((AtkResNode)ptr).Width == 280)
					{
						((AtkResNode)ptr).SetWidth((ushort)380);
					}
					if (((AtkResNode)ptr).Width == 286)
					{
						((AtkResNode)ptr).SetWidth((ushort)386);
					}
					if (((AtkResNode)ptr).Width == 292)
					{
						((AtkResNode)ptr).SetWidth((ushort)392);
					}
					if (((AtkResNode)ptr).Width == 295)
					{
						((AtkResNode)ptr).SetWidth((ushort)395);
					}
					if (((AtkResNode)ptr).Width == 300)
					{
						((AtkResNode)ptr).SetWidth((ushort)400);
					}
					if (((AtkResNode)ptr).X == 267f)
					{
						((AtkResNode)ptr).SetPositionFloat(367f, 6f);
					}
				}
			}
			AtkTextNode* textNodeById = ((AtkUnitBase)tradeMultipleAddon).GetTextNodeById(2u);
			if (textNodeById != null)
			{
				((AtkTextNode)textNodeById).SetWidth((ushort)378);
			}
			AtkTextNode* textNodeById2 = ((AtkUnitBase)tradeMultipleAddon).GetTextNodeById(3u);
			if (textNodeById2 != null)
			{
				((AtkTextNode)textNodeById2).SetWidth((ushort)378);
			}
			AtkComponentList* componentListById = ((AtkUnitBase)tradeMultipleAddon).GetComponentListById(4u);
			if (componentListById != null)
			{
				((AtkComponentNode)((AtkComponentList)componentListById).OwnerNode).SetWidth((ushort)378);
				for (int j = 0; j < ((AtkUldManager)(&((AtkComponentList)componentListById).UldManager)).NodeListCount; j++)
				{
					AtkResNode* ptr2 = ((AtkUldManager)(&((AtkComponentList)componentListById).UldManager)).NodeList[j];
					if (ptr2 != null && ((AtkResNode)ptr2).Width == 278)
					{
						((AtkResNode)ptr2).SetWidth((ushort)378);
					}
				}
				for (int k = 0; k < ((AtkComponentList)componentListById).GetItemCount(); k++)
				{
					AtkComponentListItemRenderer* itemRenderer = ((AtkComponentList)componentListById).GetItemRenderer(k);
					if (itemRenderer != null)
					{
						AtkResNode* ptr3 = ((AtkUldManager)(&((AtkComponentListItemRenderer)itemRenderer).UldManager)).SearchNodeById(2u);
						if (ptr3 != null)
						{
							((AtkResNode)ptr3).SetWidth((ushort)322);
						}
						AtkResNode* ptr4 = ((AtkUldManager)(&((AtkComponentListItemRenderer)itemRenderer).UldManager)).SearchNodeById(3u);
						if (ptr4 != null)
						{
							((AtkResNode)ptr4).SetWidth((ushort)41);
							((AtkResNode)ptr4).SetPositionFloat(311f, 0f);
						}
					}
				}
			}
			AtkComponentButton* componentButtonById = ((AtkUnitBase)tradeMultipleAddon).GetComponentButtonById(5u);
			if (componentButtonById != null)
			{
				((AtkComponentNode)((AtkComponentButton)componentButtonById).OwnerNode).SetPositionFloat(220f, 166f);
			}
			operateButtonNode = new TextButtonNode
			{
				Size = new Vector2(150f, 28f),
				Position = new Vector2(40f, 166f),
				IsVisible = true,
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("Start")),
				OnClick = delegate
				{
					if (base.TaskHelper.IsBusy)
					{
						base.TaskHelper.Abort();
					}
					else
					{
						Enqueue();
					}
				},
				IsEnabled = true
			};
			operateButtonNode.AttachNode(((AtkUnitBase)tradeMultipleAddon).RootNode);
		}
		operateButtonNode.String = ReadOnlySeString.op_Implicit(LanguageManager.Get(base.TaskHelper.IsBusy ? "Stop" : "AutoMateriaTransmutation-BatchTransmutate"));
	}

	private unsafe void Enqueue()
	{
		base.TaskHelper.Enqueue(delegate
		{
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			if (!Throttler.Shared.Throttle("AutoMateriaTransmutation-SelectMaterias", 100u))
			{
				return false;
			}
			AgentTradeMultiple* ptr = AgentTradeMultiple.Instance();
			if (ptr == null)
			{
				return false;
			}
			if (ptr->IsAllMateriaSelected())
			{
				return true;
			}
			if (!TryFindFirstMateriaSlot(out var inventoryType, out var inventorySlot))
			{
				base.TaskHelper.Abort();
				return true;
			}
			uint num = 5 - ptr->GetCurrentSelectedMateriaCount();
			InventoryItem* inventorySlot2 = ((InventoryManager)InventoryManager.Instance()).GetInventorySlot(inventoryType, (int)inventorySlot);
			if (inventorySlot2 == null)
			{
				return false;
			}
			ptr->AddMateria(inventoryType, inventorySlot, (((InventoryItem)inventorySlot2).Quantity >= num) ? num : ((uint)((InventoryItem)inventorySlot2).Quantity));
			return ptr->IsAllMateriaSelected();
		});
		base.TaskHelper.Enqueue(delegate
		{
			if (!Throttler.Shared.Throttle("AutoMateriaTransmutation-Start", 100u))
			{
				return false;
			}
			AgentTradeMultiple* ptr = AgentTradeMultiple.Instance();
			if (ptr == null)
			{
				return false;
			}
			if (!ptr->IsAllMateriaSelected())
			{
				base.TaskHelper.Abort();
				Enqueue();
				return true;
			}
			ptr->StartTransmutation();
			return true;
		});
		base.TaskHelper.DelayNext(250);
		base.TaskHelper.Enqueue((Action)Enqueue, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
	}

	private unsafe bool TryFindFirstMateriaSlot(out InventoryType inventoryType, out ushort inventorySlot)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Expected I4, but got Unknown
		inventoryType = (InventoryType)0;
		inventorySlot = 0;
		InventoryManager* ptr = InventoryManager.Instance();
		if (ptr == null)
		{
			return false;
		}
		AgentTradeMultiple* ptr2 = AgentTradeMultiple.Instance();
		if (ptr2 == null)
		{
			return false;
		}
		foreach (InventoryType item in Inventories.Player)
		{
			InventoryContainer* inventoryContainer = ((InventoryManager)ptr).GetInventoryContainer(item);
			if (inventoryContainer == null)
			{
				return false;
			}
			for (int i = 0; i < ((InventoryContainer)inventoryContainer).Size; i++)
			{
				InventoryItem* inventorySlot2 = ((InventoryContainer)inventoryContainer).GetInventorySlot(i);
				if (inventorySlot2 == null || ((InventoryItem)inventorySlot2).ItemId == 0 || config.BlacklistedItems.Contains(((InventoryItem)inventorySlot2).ItemId))
				{
					continue;
				}
				Item? row = LuminaGetter.GetRow<Item>(((InventoryItem)inventorySlot2).ItemId);
				if (row.HasValue)
				{
					Item valueOrDefault = row.GetValueOrDefault();
					if (((Item)(ref valueOrDefault)).FilterGroup == 13 && !ptr2->IsMateriaSelected(inventorySlot2))
					{
						inventoryType = (InventoryType)(int)item;
						inventorySlot = (ushort)i;
						return true;
					}
				}
			}
		}
		return false;
	}
}
