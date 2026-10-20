using System;
using System.Runtime.CompilerServices;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Models;
using OmenTools.Interop.Game.Models.Native;

namespace OmenTools.OmenService;

internal sealed class ContextMenuItemResolver : IDisposable
{
	private unsafe delegate byte BuildItemMenuDelegate(nint handler, AgentContext* context, uint itemID, byte stain0, byte stain1, ushort ownerAddonID, ushort flags, byte clearMenu, ushort extraFlags);

	private static readonly CompSig BuildItemMenuSig = new CompSig("40 53 56 41 55 41 56 41 57 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 84 24 ?? ?? ?? ?? 45 0F B6 E9");

	private Hook<BuildItemMenuDelegate>? buildItemMenuHook;

	private (nint Handler, nint Context, uint OwnerAddonID, uint ItemID) pendingItem;

	public unsafe void Init()
	{
		buildItemMenuHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromAddress<BuildItemMenuDelegate>((IntPtr)BuildItemMenuSig.ScanText(), (BuildItemMenuDelegate)BuildItemMenuDetour, (HookBackend)0);
		buildItemMenuHook.Enable();
	}

	public void Dispose()
	{
		buildItemMenuHook?.Dispose();
		buildItemMenuHook = null;
		pendingItem = default((nint, nint, uint, uint));
	}

	private unsafe byte BuildItemMenuDetour(nint handler, AgentContext* context, uint itemID, byte stain0, byte stain1, ushort ownerAddonID, ushort flags, byte clearMenu, ushort extraFlags)
	{
		byte b = buildItemMenuHook.Original(handler, context, itemID, stain0, stain1, ownerAddonID, flags, clearMenu, extraFlags);
		(nint, nint, ushort, uint) tuple = ((b != 0) ? (handler, (nint)context, ownerAddonID, itemID) : default((nint, nint, ushort, uint)));
		pendingItem = (Handler: tuple.Item1, Context: tuple.Item2, OwnerAddonID: tuple.Item3, ItemID: tuple.Item4);
		return b;
	}

	public unsafe void Resolve(ContextMenuOpenedArgs args, int nativeMenuSize)
	{
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_050e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0513: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		//IL_0458: Unknown result type (might be due to invalid IL or missing references)
		//IL_046a: Unknown result type (might be due to invalid IL or missing references)
		//IL_046f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_0479: Unknown result type (might be due to invalid IL or missing references)
		(nint, nint, uint, uint) tuple = pendingItem;
		pendingItem = default((nint, nint, uint, uint));
		if (args.InventoryAgentContext != null)
		{
			return;
		}
		AgentContext* defaultAgentContext = args.DefaultAgentContext;
		if (defaultAgentContext == null || ((AgentContext)defaultAgentContext).CurrentContextMenu == null)
		{
			return;
		}
		if (tuple.Item2 == (nint)defaultAgentContext && tuple.Item3 == args.OwnerAddonID && HasHandler(defaultAgentContext, nativeMenuSize, tuple.Item1))
		{
			args.TargetItemID = ItemUtil.GetBaseId(tuple.Item4).Item1;
			return;
		}
		AgentRecipeItemContext* ptr = AgentRecipeItemContext.Instance();
		if (ptr != null && HasHandler(defaultAgentContext, nativeMenuSize, (nint)ptr))
		{
			args.TargetItemID = ItemUtil.GetBaseId(((AgentRecipeItemContext)ptr).ResultItemId).Item1;
			return;
		}
		uint num = 0u;
		uint num2 = 0u;
		switch (args.AddonName)
		{
		case "RetainerSellList":
		{
			AgentRetainer* ptr12 = AgentRetainer.Instance();
			if (ptr12 != null)
			{
				AgentRetainer.SellListEntry sellListEntry = ptr12->SellListEntries[ptr12->ContextMenuIndex];
				InventoryItem* inventorySlot = ((InventoryManager)InventoryManager.Instance()).GetInventorySlot((InventoryType)12002, (int)sellListEntry.InventorySlot);
				if (inventorySlot != null)
				{
					num = ((InventoryItem)inventorySlot).GetBaseItemId();
					args.TargetInventoryType = (InventoryType)12002;
					args.TargetInventoryItem = *inventorySlot;
				}
			}
			break;
		}
		case "ChatLog":
		{
			AgentChatLog* ptr15 = AgentChatLog.Instance();
			if (ptr15 != null)
			{
				num = ((AgentChatLog)ptr15).ContextItemId;
			}
			break;
		}
		case "MiragePrismMiragePlate":
		{
			AgentMiragePrismPrismItemDetail* ptr8 = AgentMiragePrismPrismItemDetail.Instance();
			if (ptr8 != null)
			{
				num = ((AgentMiragePrismPrismItemDetail)ptr8).ItemId;
			}
			break;
		}
		case "ColorantColoring":
		{
			AtkUnitBase* addon = args.Addon;
			Stain val = default(Stain);
			if (addon != null && ((AtkUnitBase)addon).AtkValuesCount > 18 && (((AtkValue)((byte*)((AtkUnitBase)addon).AtkValues + (nint)18 * (nint)Unsafe.SizeOf<AtkValue>())).Int switch
			{
				0 => ((AtkValue)((byte*)((AtkUnitBase)addon).AtkValues + (nint)14 * (nint)Unsafe.SizeOf<AtkValue>())).UInt, 
				1 => ((AtkValue)((byte*)((AtkUnitBase)addon).AtkValues + (nint)15 * (nint)Unsafe.SizeOf<AtkValue>())).UInt, 
				_ => 0u, 
			}).ToLuminaRowRef<Stain>().TryGetValue(ref val))
			{
				num = ((Stain)(ref val)).Item[0].RowId;
			}
			break;
		}
		case "CabinetWithdraw":
		{
			AgentCabinetWithdraw* ptr17 = AgentCabinetWithdraw.Instance();
			if (ptr17 != null && ((AgentCabinetWithdraw)ptr17).Data != null)
			{
				num = ((CabinetWithdrawData)((AgentCabinetWithdraw)ptr17).Data).ContextMenuSelectedItemId;
			}
			break;
		}
		case "CharacterInspect":
		{
			AgentInspect* ptr10 = AgentInspect.Instance();
			if (ptr10 != null && HasHandler(defaultAgentContext, nativeMenuSize, (nint)ptr10, 1L, 4L))
			{
				int selectedItemSlot = ((AgentInspect)ptr10).SelectedItemSlot;
				if ((uint)selectedItemSlot < ((AgentInspect)ptr10).Items.Length)
				{
					ItemData val2 = ((AgentInspect)ptr10).Items[selectedItemSlot];
					num = val2.ItemId;
					num2 = val2.GlamourItemId;
				}
			}
			break;
		}
		case "MiragePrismPrismBox":
		case "MiragePrismPrismBoxCrystallize":
		{
			AgentMiragePrismPrismBox* ptr7 = AgentMiragePrismPrismBox.Instance();
			if (ptr7 == null || ((AgentMiragePrismPrismBox)ptr7).Data == null)
			{
				break;
			}
			MiragePrismPrismBoxData* data = ((AgentMiragePrismPrismBox)ptr7).Data;
			if (HasHandler(defaultAgentContext, nativeMenuSize, (nint)ptr7, 19L, 23L))
			{
				ushort crystallizeItemIndex = ((MiragePrismPrismBoxData)data).CrystallizeItemIndex;
				if (crystallizeItemIndex < ((MiragePrismPrismBoxData)data).CrystallizeItemCount && crystallizeItemIndex < ((MiragePrismPrismBoxData)data).CrystallizeItems.Length)
				{
					num = ((MiragePrismPrismBoxData)data).CrystallizeItems[crystallizeItemIndex].ItemId;
				}
			}
			else if (HasHandler(defaultAgentContext, nativeMenuSize, (nint)ptr7, 8L, 17L))
			{
				num = ((PrismBoxItem)(&((MiragePrismPrismBoxData)data).TempContextItem)).ItemId;
			}
			break;
		}
		case "RecipeNote":
		{
			AgentRecipeNote* ptr4 = AgentRecipeNote.Instance();
			if (ptr4 != null)
			{
				num = ((AgentRecipeNote)ptr4).ContextMenuResultItemId;
			}
			break;
		}
		case "JournalAccept":
		{
			AgentJournalAccept* ptr13 = AgentJournalAccept.Instance();
			if (ptr13 != null && (uint)((AgentJournalAccept)ptr13).ContextMenuSelectedRewardIndex < ((AgentJournalAccept)ptr13).RewardItems.Count)
			{
				num = ((AgentJournalAccept)ptr13).RewardItems[((AgentJournalAccept)ptr13).ContextMenuSelectedRewardIndex].ItemId;
			}
			break;
		}
		case "GuildLeve":
		{
			AgentLeveQuest* ptr9 = AgentLeveQuest.Instance();
			if (ptr9 != null && (uint)((AgentLeveQuest)ptr9).ContextMenuSelectedRewardIndex < ((AgentLeveQuest)ptr9).RewardItems.Count)
			{
				num = ((AgentLeveQuest)ptr9).RewardItems[((AgentLeveQuest)ptr9).ContextMenuSelectedRewardIndex].ItemId;
			}
			break;
		}
		case "NeedGreed":
		{
			AgentLoot* ptr5 = AgentLoot.Instance();
			Loot* ptr6 = Loot.Instance();
			if (ptr5 != null && ptr6 != null && (uint)((AgentLoot)ptr5).SelectedSlotIndex < ((Loot)ptr6).Items.Length)
			{
				num = ((Loot)ptr6).Items[((AgentLoot)ptr5).SelectedSlotIndex].ItemId;
			}
			break;
		}
		case "ContentsFinder":
		{
			AgentContentsFinder* ptr16 = AgentContentsFinder.Instance();
			if (ptr16 != null && ((RewardContextMenuEventHandler)(&((AgentContentsFinder)ptr16).RewardContextMenuHandler)).SelectedReward != null)
			{
				num = ((ItemWrap)((RewardContextMenuEventHandler)(&((AgentContentsFinder)ptr16).RewardContextMenuHandler)).SelectedReward).ItemId;
			}
			break;
		}
		case "Journal":
		{
			AgentQuestJournal* ptr14 = AgentQuestJournal.Instance();
			if (ptr14 != null)
			{
				num = ((AgentQuestJournal)ptr14).ContextMenuSelectedItemId;
			}
			break;
		}
		case "Achievement":
		{
			AgentAchievement* ptr11 = AgentAchievement.Instance();
			if (ptr11 != null && HasHandler(defaultAgentContext, nativeMenuSize, (nint)ptr11, 65537L, 65540L))
			{
				num = ((AgentAchievement)ptr11).ContextMenuSelectedItemId;
			}
			break;
		}
		case "MateriaAttach":
		{
			AgentMateriaAttach* ptr2 = AgentMateriaAttach.Instance();
			if (ptr2 == null || ((AgentMateriaAttach)ptr2).Data == null || !HasHandler(defaultAgentContext, nativeMenuSize, (nint)ptr2, 3L, 3L))
			{
				break;
			}
			short selectedMateriaIndex = ((AgentMateriaAttach)ptr2).SelectedMateriaIndex;
			if ((uint)selectedMateriaIndex < ((AgentMateriaAttach)ptr2).MateriaCount && ((MateriaAttachData)((AgentMateriaAttach)ptr2).Data).MateriaArraySorted != null)
			{
				MateriaAttachEntry* ptr3 = ((MateriaAttachData)((AgentMateriaAttach)ptr2).Data).MateriaArraySorted[selectedMateriaIndex];
				if (ptr3 != null && ((MateriaAttachEntry)ptr3).Item != null)
				{
					num = ((InventoryItem)((MateriaAttachEntry)ptr3).Item).GetBaseItemId();
				}
			}
			break;
		}
		}
		args.TargetItemID = ItemUtil.GetBaseId(num).Item1;
		args.TargetGlamourID = ItemUtil.GetBaseId(num2).Item1;
	}

	private unsafe static bool HasHandler(AgentContext* context, int nativeMenuSize, nint handler, long minimumParam = long.MinValue, long maximumParam = long.MaxValue)
	{
		if (handler == 0)
		{
			return false;
		}
		ContextMenu* currentContextMenu = ((AgentContext)context).CurrentContextMenu;
		int num = Math.Min(nativeMenuSize, ((ContextMenu)currentContextMenu).EventHandlers.Length - 8);
		for (int i = 8; i < 8 + num; i++)
		{
			if (((ContextMenu)currentContextMenu).EventIds[i] > 106 && ((ContextMenu)currentContextMenu).EventHandlers[i].Value == (AtkEventInterface*)handler && ((ContextMenu)currentContextMenu).EventHandlerParams[i] >= minimumParam && ((ContextMenu)currentContextMenu).EventHandlerParams[i] <= maximumParam)
			{
				return true;
			}
		}
		return false;
	}
}
