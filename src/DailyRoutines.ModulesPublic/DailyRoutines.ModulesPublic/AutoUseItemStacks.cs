using System;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Helpers;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.KamiToolKit.Addons.InputNumeric;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.ModulesPublic;

public class AutoUseItemStacks : ModuleBase
{
	private sealed class OpenCofferMenuItem(AutoUseItemStacks module) : ContextMenuEntry
	{
		public override string Identifier => "AutoUseItemStacks";

		public unsafe override ContextMenuItem? Create(ContextMenuOpenedArgs args)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			InventoryItem? targetInventoryItem = args.TargetInventoryItem;
			InventoryItem item;
			uint quantity;
			uint itemID;
			bool flag;
			if (targetInventoryItem.HasValue)
			{
				item = targetInventoryItem.GetValueOrDefault();
				quantity = ((InventoryItem)(ref item)).GetQuantity();
				if (quantity < 2)
				{
					return null;
				}
				itemID = ((InventoryItem)(ref item)).GetBaseItemId();
				Item? row = LuminaGetter.GetRow<Item>(itemID);
				if (row.HasValue)
				{
					Item valueOrDefault = row.GetValueOrDefault();
					if (((Item)(ref valueOrDefault)).StackSize > 1)
					{
						uint rowId = ((Item)(ref valueOrDefault)).ItemAction.RowId;
						if (rowId == 367 || rowId == 388 || rowId == 2462)
						{
							flag = true;
							goto IL_00c3;
						}
					}
				}
				flag = false;
				goto IL_00c3;
			}
			return null;
			IL_00c3:
			if (!flag)
			{
				return null;
			}
			return new ContextMenuItem
			{
				Name = ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoUseItemStacks-ContextMenu")),
				OnClicked = delegate
				{
					//IL_007f: Unknown result type (might be due to invalid IL or missing references)
					if (module.drInputNumeric != null && !AddonHelper.TryGetPtrByName("DRInputNumeric", out var _))
					{
						try
						{
							module.drInputNumeric?.Dispose();
							module.drInputNumeric = null;
						}
						catch
						{
						}
					}
					if (module.drInputNumeric == null)
					{
						module.drInputNumeric = InputNumericAddon.Open(new InputNumericAddonOptions
						{
							Prompt = ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoUseItemStacks-Popup-PleaseInput")),
							Value = (int)quantity,
							Min = 1,
							Max = (int)quantity,
							Callback = delegate(InputNumericAddon addon, InputNumericAddonResult result)
							{
								//IL_002c: Unknown result type (might be due to invalid IL or missing references)
								module.drInputNumeric = null;
								if (result == InputNumericAddonResult.Confirmed)
								{
									module.EnqueueOpenCoffers(itemID, item.Container, (ushort)item.Slot, (uint)Math.Max(quantity - addon.Value, 0L), 0u);
								}
							},
							Position = new AddonPosition((*((AtkUnitBase)args.Addon).RootNode).GetNodeState().Center, AddonPositionAlignment.TopCenter)
						});
					}
				}
			};
		}
	}

	private OpenCofferMenuItem openCofferMenu;

	private InputNumericAddon? drInputNumeric;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoUseItemStacksTitle"),
		Description = LanguageManager.Get("AutoUseItemStacksDescription"),
		Category = ModuleCategory.System,
		Author = new string[1] { "Cindy-Master" }
	};

	protected override void Init()
	{
		base.TaskHelper = new TaskHelper
		{
			TimeoutMS = 5000,
			ShowDebug = true
		};
		openCofferMenu = new OpenCofferMenuItem(this);
		OmenServiceBase<ContextMenuManager>.Instance().Reg(openCofferMenu);
	}

	protected override void Uninit()
	{
		OmenServiceBase<ContextMenuManager>.Instance().Unreg(openCofferMenu);
		drInputNumeric?.Dispose();
		drInputNumeric = null;
	}

	protected override void ConfigUI()
	{
		ImGuiOmExtension.ConflictKeyText();
	}

	public unsafe void EnqueueOpenCoffers(uint itemID, InventoryType inventoryType, ushort inventorySlot, uint leftCount, uint finishRound)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (base.TaskHelper.AbortByConflictKey(this))
		{
			NotifyFinished();
			return;
		}
		InventoryManager* manager = InventoryManager.Instance();
		if (manager == null)
		{
			NotifyFinished();
			return;
		}
		InventoryContainer* inventoryContainer = ((InventoryManager)manager).GetInventoryContainer(inventoryType);
		if (inventoryContainer == null)
		{
			NotifyFinished();
			return;
		}
		InventoryItem* inventorySlot2 = ((InventoryContainer)inventoryContainer).GetInventorySlot((int)inventorySlot);
		if (inventorySlot2 == null)
		{
			NotifyFinished();
			return;
		}
		uint currentQuantity = ((InventoryItem)inventorySlot2).GetQuantity();
		if (((InventoryItem)inventorySlot2).GetBaseItemId() != itemID || currentQuantity <= leftCount)
		{
			NotifyFinished();
			return;
		}
		base.TaskHelper.Enqueue(() => ((AgentInventoryContext)AgentInventoryContext.Instance()).UseItem(itemID, inventoryType, (uint)inventorySlot, (short)0) == 0, "使用物品");
		TaskHelper? taskHelper = base.TaskHelper;
		Func<bool> task = delegate
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			InventoryContainer* inventoryContainer2 = ((InventoryManager)manager).GetInventoryContainer(inventoryType);
			if (inventoryContainer2 == null)
			{
				return true;
			}
			InventoryItem* inventorySlot3 = ((InventoryContainer)inventoryContainer2).GetInventorySlot((int)inventorySlot);
			if (inventorySlot3 == null)
			{
				return true;
			}
			return (((InventoryItem)inventorySlot3).GetBaseItemId() != itemID || ((InventoryItem)inventorySlot3).GetQuantity() != currentQuantity) ? true : false;
		};
		Action timeoutAction = NotifyFinished;
		taskHelper.Enqueue(task, "等待物品使用判定", 0, null, null, timeoutAction);
		base.TaskHelper.Enqueue(delegate
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			EnqueueOpenCoffers(itemID, inventoryType, inventorySlot, leftCount, finishRound + 1);
		}, "进入下一轮物品使用判定");
		void NotifyFinished()
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			if (finishRound != 0)
			{
				ReadOnlySeString se = LanguageManager.GetSe("AutoUseItemStacks-Notification-Finished", finishRound, SeString.CreateItemLink(itemID, false, (string)null));
				NotifyHelper.Toast(se);
				OmenServiceBase<NotifyHelper>.Instance().Chat(se);
			}
		}
	}
}
