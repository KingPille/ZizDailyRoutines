using System;
using System.Numerics;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.ExecuteCommand.Implementations;
using OmenTools.KamiToolKit.Addons;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.ModulesPublic.Interface;

public class AutoRestoreFurniture : ModuleBase
{
	private sealed class AutoRestoreFurnitureAddon(AutoRestoreFurniture module) : AttachedAddon("HousingGoods")
	{
		private TextButtonNode? stopButton;

		private TextButtonNode? placedToStoreRoomButton;

		private TextButtonNode? placedToInventoryButton;

		private TextButtonNode? storedToInventoryButton;

		protected override AttachedAddonPosition AttachPosition => AttachedAddonPosition.LeftTop;

		protected override Vector2 PositionOffset => new Vector2(0f, 6f);

		protected unsafe override bool CanOpenAddon
		{
			get
			{
				if (Addons.HousingGoods != null)
				{
					return (*Addons.HousingGoods).IsAddonAndNodesReady();
				}
				return false;
			}
		}

		protected unsafe override bool CanCloseHostAddon(AtkUnitBase* hostAddon)
		{
			return false;
		}

		protected unsafe override void OnSetup(AtkUnitBase* addon, Span<AtkValue> atkValues)
		{
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			if (base.WindowNode is WindowNode windowNode)
			{
				windowNode.CloseButtonNode.IsVisible = false;
			}
			FlagHelper.UpdateFlag(ref ((AtkUnitBase)addon).Flags1A1, 4, enable: true);
			FlagHelper.UpdateFlag(ref ((AtkUnitBase)addon).Flags1A0, 128, enable: true);
			FlagHelper.UpdateFlag(ref ((AtkUnitBase)addon).Flags1A1, 64, enable: true);
			FlagHelper.UpdateFlag(ref ((AtkUnitBase)addon).Flags1A3, 1, enable: true);
			VerticalListNode verticalListNode = new VerticalListNode
			{
				IsVisible = true,
				Position = base.ContentStartPosition,
				ItemSpacing = 4f,
				Size = base.ContentSize,
				FitContents = true
			};
			stopButton = new TextButtonNode
			{
				IsVisible = true,
				IsEnabled = true,
				Size = new Vector2(base.ContentSize.X - 8f, 32f),
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("Stop")),
				OnClick = module.TaskHelper.Abort
			};
			placedToStoreRoomButton = CreateActionButton(LanguageManager.Get("AutoRestoreFurniture-PlacedToStoreRoom"), toStoreRoom: true, isStoredItem: false);
			placedToInventoryButton = CreateActionButton(LanguageManager.Get("AutoRestoreFurniture-PlacedToInventory"), toStoreRoom: false, isStoredItem: false);
			storedToInventoryButton = CreateActionButton(LanguageManager.Get("AutoRestoreFurniture-StoredToInventory"), toStoreRoom: false, isStoredItem: true);
			verticalListNode.AddNode(stopButton);
			verticalListNode.AddNode(placedToStoreRoomButton);
			verticalListNode.AddNode(placedToInventoryButton);
			verticalListNode.AddNode(storedToInventoryButton);
			verticalListNode.AttachNode(this);
			verticalListNode.RecalculateLayout();
			SetWindowSize(base.Size.X, base.ContentStartPosition.Y + verticalListNode.Height + 16f);
			verticalListNode.Position = base.ContentStartPosition;
			verticalListNode.Height = verticalListNode.Height;
		}

		protected unsafe override void OnAttachedAddonUpdate(AtkUnitBase* addon, AtkUnitBase* hostAddon)
		{
			RefreshState();
		}

		protected override void OnHostAddon(AddonEvent type, AddonArgs? args)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Invalid comparison between Unknown and I4
			if ((int)type == 6)
			{
				module.TaskHelper.Abort();
			}
		}

		private unsafe TextButtonNode CreateActionButton(string label, bool toStoreRoom, bool isStoredItem)
		{
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			return new TextButtonNode
			{
				IsVisible = true,
				IsEnabled = true,
				Size = new Vector2(base.ContentSize.X - 8f, 32f),
				String = ReadOnlySeString.op_Implicit(label),
				OnClick = delegate
				{
					bool flag = ((HousingManager)HousingManager.Instance()).OutdoorTerritory != null;
					uint startInventory = ((!isStoredItem) ? (flag ? 25001u : 25003u) : (flag ? 27000u : 27001u));
					uint endInventory = ((!isStoredItem) ? (flag ? 25001u : 25010u) : (flag ? 27000u : 27008u));
					module.EnqueueRestore(startInventory, endInventory, toStoreRoom);
				}
			};
		}

		private void RefreshState()
		{
			bool isBusy = module.TaskHelper.IsBusy;
			if (placedToStoreRoomButton != null)
			{
				placedToStoreRoomButton.IsEnabled = !isBusy;
			}
			if (placedToInventoryButton != null)
			{
				placedToInventoryButton.IsEnabled = !isBusy;
			}
			if (storedToInventoryButton != null)
			{
				storedToInventoryButton.IsEnabled = !isBusy;
			}
			if (stopButton != null)
			{
				stopButton.IsEnabled = isBusy;
			}
		}
	}

	private AutoRestoreFurnitureAddon? addon;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoRestoreFurnitureTitle"),
		Description = LanguageManager.Get("AutoRestoreFurnitureDescription"),
		Category = ModuleCategory.Interface
	};

	protected override void Init()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (base.TaskHelper == null)
		{
			TaskHelper taskHelper = (base.TaskHelper = new TaskHelper());
		}
		if (addon == null)
		{
			addon = new AutoRestoreFurnitureAddon(this)
			{
				InternalName = "DRAutoRestoreFurniture",
				Title = ReadOnlySeString.op_Implicit(Info.Title),
				Size = new Vector2(280f, 170f),
				RememberClosePosition = false
			};
		}
		OmenServiceBase<LogMessageManager>.Instance().RegPre(OnLogMessage);
	}

	protected override void Uninit()
	{
		addon?.Dispose();
		addon = null;
		OmenServiceBase<LogMessageManager>.Instance().Unreg(OnLogMessage);
	}

	private void OnLogMessage(ref bool isPrevented, ref uint logMessageID, ref LogMessageQueueItem item)
	{
		if (logMessageID == 3338 && base.TaskHelper.IsBusy)
		{
			isPrevented = true;
		}
	}

	private unsafe bool EnqueueRestore(uint startInventory, uint endInventory, bool toStoreRoom)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		InventoryManager* ptr = InventoryManager.Instance();
		for (uint num = startInventory; num <= endInventory; num++)
		{
			InventoryType val = (InventoryType)num;
			InventoryContainer* inventoryContainer = ((InventoryManager)ptr).GetInventoryContainer(val);
			if (inventoryContainer == null)
			{
				continue;
			}
			for (int i = 0; i < ((InventoryContainer)inventoryContainer).Size; i++)
			{
				InventoryItem* slot = ((InventoryContainer)inventoryContainer).GetInventorySlot(i);
				if (slot != null && ((InventoryItem)slot).ItemId != 0)
				{
					base.TaskHelper.Enqueue(delegate
					{
						//IL_0006: Unknown result type (might be due to invalid IL or missing references)
						HousingCommand.Restore(((InventoryItem)slot).Container, ((InventoryItem)slot).GetSlot(), toStoreRoom);
					});
					base.TaskHelper.Enqueue(() => EnqueueRestore(startInventory, endInventory, toStoreRoom));
					return true;
				}
			}
		}
		base.TaskHelper.Abort();
		base.TaskHelper.DelayNext(500);
		return true;
	}
}
