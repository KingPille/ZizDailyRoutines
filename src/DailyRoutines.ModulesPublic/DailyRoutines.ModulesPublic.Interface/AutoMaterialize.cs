using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Command;
using Dalamud.Game.Gui.Toast;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;

namespace DailyRoutines.ModulesPublic.Interface;

public class AutoMaterialize : ModuleBase
{
	private delegate int ExtractMateriaDelegate(nint a1, InventoryType type, uint slot);

	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static AddonEventDelegate _003C0_003E__OnDialogAddon;
	}

	private static readonly CompSig ExtractMateriaSig = new CompSig("48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC ?? 41 0F BF F8 8B DA 48 8B F1 45 33 C0");

	private Hook<ExtractMateriaDelegate>? ExtractMateriaHook;

	private TextNode? lableNode;

	private TextButtonNode? startButtonNode;

	private TextButtonNode? stopButtonNode;

	private const string COMMAND = "materialize";

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoMaterializeTitle"),
		Description = LanguageManager.Get("AutoMaterializeDescription"),
		Category = ModuleCategory.Interface
	};

	protected unsafe override void Init()
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Expected O, but got Unknown
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Expected O, but got Unknown
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Expected O, but got Unknown
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Expected O, but got Unknown
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Expected O, but got Unknown
		base.TaskHelper = new TaskHelper
		{
			ShowDebug = true,
			TaskIntervalMS = 500,
			RetryIntervalMS = 500
		};
		ExtractMateriaHook = ExtractMateriaSig.GetHook<ExtractMateriaDelegate>(ExtractMateriaDetour);
		ExtractMateriaHook.Enable();
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)5, "Materialize", new AddonEventDelegate(OnAddon));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, "Materialize", new AddonEventDelegate(OnAddon));
		IAddonLifecycle obj = DalamudServiceExtension.Instance<IAddonLifecycle>();
		object obj2 = _003C_003EO._003C0_003E__OnDialogAddon;
		if (obj2 == null)
		{
			AddonEventDelegate val = OnDialogAddon;
			_003C_003EO._003C0_003E__OnDialogAddon = val;
			obj2 = (object)val;
		}
		obj.RegisterListener((AddonEvent)1, "MaterializeDialog", (AddonEventDelegate)obj2);
		if (Addons.MaterializeDialog != null)
		{
			OnDialogAddon((AddonEvent)1, null);
		}
		OmenServiceBase<LogMessageManager>.Instance().RegPost(OnLogMessage);
		OmenServiceBase<UseActionManager>.Instance().RegPreUseAction(OnPreUseAction);
		OmenServiceBase<CommandManager>.Instance().AddSubCommand("materialize", new CommandInfo((HandlerDelegate)delegate
		{
			Enqueue();
		})
		{
			HelpMessage = LanguageManager.Get("AutoMaterializeTitle")
		});
	}

	protected override void Uninit()
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Expected O, but got Unknown
		OmenServiceBase<CommandManager>.Instance().RemoveSubCommand("materialize");
		OmenServiceBase<LogMessageManager>.Instance().Unreg(OnLogMessage);
		OmenServiceBase<UseActionManager>.Instance().Unreg(OnPreUseAction);
		IAddonLifecycle obj = DalamudServiceExtension.Instance<IAddonLifecycle>();
		AddonEventDelegate[] obj2 = new AddonEventDelegate[2]
		{
			OnAddon,
			default(AddonEventDelegate)
		};
		object obj3 = _003C_003EO._003C0_003E__OnDialogAddon;
		if (obj3 == null)
		{
			AddonEventDelegate val = OnDialogAddon;
			_003C_003EO._003C0_003E__OnDialogAddon = val;
			obj3 = (object)val;
		}
		obj2[1] = (AddonEventDelegate)obj3;
		obj.UnregisterListener((AddonEventDelegate[])(object)obj2);
		lableNode?.Dispose();
		lableNode = null;
		startButtonNode?.Dispose();
		startButtonNode = null;
		stopButtonNode?.Dispose();
		stopButtonNode = null;
	}

	protected override void ConfigUI()
	{
		ImGuiOmExtension.ConflictKeyText();
	}

	private void OnPreUseAction(ref bool isPrevented, ref ActionType actionType, ref uint actionID, ref ulong targetID, ref uint extraParam, ref UseActionMode queueState, ref uint comboRouteID)
	{
		if ((int)actionType != 1 || actionID != 2469)
		{
			base.TaskHelper.Abort();
		}
	}

	private void OnLogMessage(uint logMessageID, LogMessageQueueItem item)
	{
		if (logMessageID == 744)
		{
			Enqueue();
		}
	}

	private int ExtractMateriaDetour(nint a1, InventoryType type, uint slot)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		int result = ExtractMateriaHook.Original(a1, type, slot);
		Enqueue();
		return result;
	}

	private unsafe void OnAddon(AddonEvent type, AddonArgs args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		if ((int)type != 5)
		{
			if ((int)type == 6)
			{
				lableNode = null;
				startButtonNode = null;
				stopButtonNode = null;
				base.TaskHelper.Abort();
			}
		}
		else
		{
			if (Addons.Materialize == null)
			{
				return;
			}
			if (lableNode == null)
			{
				lableNode = new TextNode
				{
					IsVisible = true,
					Position = new Vector2(135f, 8f),
					Size = new Vector2(150f, 28f),
					String = ReadOnlySeString.op_Implicit(Info.Title ?? ""),
					FontSize = 14u,
					AlignmentType = (AlignmentType)5,
					TextFlags = (TextFlags)9
				};
				lableNode.AttachNode(((AtkUnitBase)Addons.Materialize).RootNode);
			}
			if (startButtonNode == null)
			{
				startButtonNode = new TextButtonNode
				{
					Position = new Vector2(295f, 10f),
					Size = new Vector2(100f, 28f),
					IsVisible = true,
					String = ReadOnlySeString.op_Implicit(LanguageManager.Get("Start")),
					OnClick = Enqueue
				};
				startButtonNode.AttachNode(((AtkUnitBase)Addons.Materialize).RootNode);
			}
			startButtonNode.IsEnabled = !base.TaskHelper.IsBusy;
			if (stopButtonNode == null)
			{
				stopButtonNode = new TextButtonNode
				{
					Position = new Vector2(400f, 10f),
					Size = new Vector2(100f, 28f),
					IsVisible = true,
					String = ReadOnlySeString.op_Implicit(LanguageManager.Get("Stop")),
					OnClick = delegate
					{
						base.TaskHelper.Abort();
					}
				};
				stopButtonNode.AttachNode(((AtkUnitBase)Addons.Materialize).RootNode);
			}
		}
	}

	private unsafe static void OnDialogAddon(AddonEvent type, AddonArgs args)
	{
		AtkUnitBase* materializeDialog = Addons.MaterializeDialog;
		if (materializeDialog != null)
		{
			(*materializeDialog).Callback(0);
		}
	}

	private void Enqueue()
	{
		if (!base.TaskHelper.IsBusy)
		{
			base.TaskHelper.Enqueue((Func<bool>)EnqueueExtractMateria, "精炼装备", 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
		}
	}

	private unsafe bool EnqueueExtractMateria()
	{
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		if (base.TaskHelper.AbortByConflictKey(this))
		{
			return true;
		}
		if (Inventories.Player.IsFull() || DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)26])
		{
			base.TaskHelper.Abort();
			return true;
		}
		if (!((Conditions)Conditions.Instance()).HasPermission(133u, 0, 0))
		{
			return false;
		}
		if (!((IEnumerable<InventoryType>)Inventories.PlayerWithArmory).TryGetFirstItem((Predicate<InventoryItem>)delegate(InventoryItem slot)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			uint baseItemId = ((InventoryItem)(ref slot)).GetBaseItemId();
			Item item;
			return LuminaGetter.TryGetRow<Item>(baseItemId, out item) && slot.SpiritbondOrCollectability >= 10000 && ((Item)(ref item)).EquipSlotCategory.RowId != 0;
		}, out InventoryItem* inventorySlot))
		{
			string text = LanguageManager.Get("AutoMaterialize-Notice-ExtractFinish");
			OmenServiceBase<NotifyHelper>.Instance().NotificationInfo(text);
			OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(text));
			base.TaskHelper.Abort();
			return true;
		}
		base.TaskHelper.Enqueue(delegate
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Expected O, but got Unknown
			if (ExtractMateriaHook.Original(IntPtr.Zero, ((InventoryItem)inventorySlot).GetInventoryType(), ((InventoryItem)inventorySlot).GetSlot()) == 0)
			{
				ReadOnlySeString se = LanguageManager.GetSe("AutoMaterialize-Notice-ExtractNow", SeString.CreateItemLink(((InventoryItem)inventorySlot).GetBaseItemId(), ((InventoryItem)inventorySlot).IsHighQuality(), (string)null));
				OmenServiceBase<NotifyHelper>.Instance().Chat(se);
				NotifyHelper.ToastQuest(se, new QuestToastOptions
				{
					IconId = LuminaWrapper.GetItemIconID(((InventoryItem)inventorySlot).GetBaseItemId())
				});
				return true;
			}
			return false;
		}, $"精炼魔晶石: {LuminaWrapper.GetItemName(((InventoryItem)inventorySlot).GetBaseItemId())} ({((InventoryItem)inventorySlot).GetBaseItemId()})");
		base.TaskHelper.Enqueue((Func<bool>)EnqueueExtractMateria, "进行下一轮魔晶石精炼", 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
		return true;
	}
}
