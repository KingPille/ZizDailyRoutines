using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Command;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Interface;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.ImGuiOm.Widgets.Combos;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.Helpers;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.KamiToolKit.Addons.InputNumeric;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.ModulesPublic;

public class AutoSplitStacks : ModuleBase
{
	private class Config : ModuleConfig
	{
		public List<SplitGroup> SplitGroups = new List<SplitGroup>();
	}

	private class SplitGroup : IEquatable<SplitGroup>
	{
		public bool IsEnabled { get; set; } = true;

		public uint ItemID { get; set; }

		public int Amount { get; set; }

		public SplitGroup()
		{
		}

		public SplitGroup(uint itemID, int amount)
		{
			ItemID = itemID;
			Amount = amount;
		}

		public bool Equals(SplitGroup? other)
		{
			if (other == null)
			{
				return false;
			}
			if (this == other)
			{
				return true;
			}
			return ItemID == other.ItemID;
		}

		public override bool Equals(object? obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (this == obj)
			{
				return true;
			}
			if (obj.GetType() == GetType())
			{
				return Equals((SplitGroup)obj);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (int)ItemID;
		}
	}

	private sealed class FastSplitItemStack(AutoSplitStacks module) : ContextMenuEntry
	{
		public override string Identifier => "AutoSplitStacks";

		public unsafe override ContextMenuItem? Create(ContextMenuOpenedArgs args)
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			InventoryItem? targetInventoryItem = args.TargetInventoryItem;
			if (targetInventoryItem.HasValue)
			{
				InventoryItem valueOrDefault = targetInventoryItem.GetValueOrDefault();
				uint quantity = ((InventoryItem)(ref valueOrDefault)).GetQuantity();
				if (quantity < 2)
				{
					return null;
				}
				return new ContextMenuItem
				{
					Name = ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoSplitStacks-ContextMenu-Split")),
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
								Prompt = ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoSplitStacks-Popup-PleaseInput")),
								Value = 1,
								Min = 1,
								Max = (int)Math.Min(998u, quantity),
								Callback = delegate(InputNumericAddon addon, InputNumericAddonResult result)
								{
									module.drInputNumeric = null;
									if (result == InputNumericAddonResult.Confirmed)
									{
										module.EnqueueSplitByInfo(args.TargetItemID, addon.Value);
									}
								},
								Position = new AddonPosition((*((AtkUnitBase)args.Addon).RootNode).GetNodeState().Center, AddonPositionAlignment.TopCenter)
							});
						}
					}
				};
			}
			return null;
		}
	}

	private Config config;

	private ItemSelectCombo itemSelectCombo;

	private FastSplitItemStack fastSplitItemStackMenu;

	private InputNumericAddon? drInputNumeric;

	private int splitAmountInput = 1;

	private const string COMMAND = "/pdrsplit";

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoSplitStacksTitle"),
		Description = LanguageManager.Get("AutoSplitStacksDescription"),
		Category = ModuleCategory.System
	};

	protected override void Init()
	{
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Expected O, but got Unknown
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Expected O, but got Unknown
		itemSelectCombo = new ItemSelectCombo("Item", from x in (IEnumerable<Item>)LuminaGetter.Get<Item>()
			where ((Item)(ref x)).FilterGroup != 16 && ((Item)(ref x)).StackSize > 1 && !string.IsNullOrEmpty(((object)((Item)(ref x)).Name/*cast due to constrained. prefix*/).ToString())
			group x by ((object)((Item)(ref x)).Name/*cast due to constrained. prefix*/).ToString() into x
			select x.First());
		fastSplitItemStackMenu = new FastSplitItemStack(this);
		base.TaskHelper = new TaskHelper();
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		OmenServiceBase<CommandManager>.Instance().AddCommand("/pdrsplit", new CommandInfo(new HandlerDelegate(OnCommand))
		{
			HelpMessage = LanguageManager.Get("AutoSplitStacks-CommandHelp")
		});
		OmenServiceBase<ContextMenuManager>.Instance().Reg(fastSplitItemStackMenu);
	}

	protected override void Uninit()
	{
		OmenServiceBase<CommandManager>.Instance().RemoveCommand("/pdrsplit");
		OmenServiceBase<ContextMenuManager>.Instance().Unreg(fastSplitItemStackMenu);
		drInputNumeric?.Dispose();
		drInputNumeric = null;
	}

	protected override void ConfigUI()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_046b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0498: Unknown result type (might be due to invalid IL or missing references)
		//IL_049d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04db: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0508: Unknown result type (might be due to invalid IL or missing references)
		//IL_0514: Unknown result type (might be due to invalid IL or missing references)
		//IL_051a: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(1, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("Command"));
		((ImU8String)(ref val)).AppendLiteral(":");
		ImGui.TextColored(ref vector, val);
		ImGui.SameLine();
		ImU8String val2 = default(ImU8String);
		((ImU8String)(ref val2))._002Ector(3, 2);
		((ImU8String)(ref val2)).AppendFormatted<string>("/pdrsplit");
		((ImU8String)(ref val2)).AppendLiteral(" → ");
		((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get("AutoSplitStacks-CommandHelp"));
		ImGui.TextUnformatted(val2);
		ImGui.Spacing();
		TableDisposable val3 = ImRaii.Table(ImU8String.op_Implicit("SplitItem"), 4);
		try
		{
			if (!val3)
			{
				return;
			}
			ImGui.TableSetupColumn(ImU8String.op_Implicit("勾选框"), (ImGuiTableColumnFlags)8, ImGui.GetTextLineHeightWithSpacing(), 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("名称"), (ImGuiTableColumnFlags)0, 30f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("数量"), (ImGuiTableColumnFlags)8, ImGui.CalcTextSize(ImU8String.op_Implicit("四个汉字"), false, -1f).X, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("操作"), (ImGuiTableColumnFlags)0, 10f, 0u);
			ImGui.TableNextRow((ImGuiTableRowFlags)1);
			ImGui.TableNextColumn();
			if (ImGuiOm.SelectableIconCentered("AddNewGroup", (FontAwesomeIcon)61543, selected: false, (ImGuiSelectableFlags)0))
			{
				ImGui.OpenPopup(ImU8String.op_Implicit("AddNewGroupPopup"), (ImGuiPopupFlags)0);
			}
			PopupDisposable val4 = ImRaii.Popup(ImU8String.op_Implicit("AddNewGroupPopup"));
			ImU8String val9;
			try
			{
				if (PopupDisposable.op_Implicit(val4))
				{
					GroupDisposable val5 = ImRaii.Group();
					try
					{
						ImGui.AlignTextToFramePadding();
						vector = KnownColor.Chocolate.ToVector4();
						ImU8String val6 = new ImU8String(1, 1);
						((ImU8String)(ref val6)).AppendFormatted<string>(LanguageManager.Get("Item"));
						((ImU8String)(ref val6)).AppendLiteral(":");
						ImGui.TextColored(ref vector, val6);
						ImGui.SameLine();
						ImGui.SetNextItemWidth(250f * Globals.GlobalUIScale);
						itemSelectCombo.DrawRadio();
						ImGui.AlignTextToFramePadding();
						vector = KnownColor.Chocolate.ToVector4();
						ImU8String val7 = new ImU8String(1, 1);
						((ImU8String)(ref val7)).AppendFormatted<string>(LanguageManager.Get("Amount"));
						((ImU8String)(ref val7)).AppendLiteral(":");
						ImGui.TextColored(ref vector, val7);
						ImGui.SameLine();
						ImGui.SetNextItemWidth(250f * Globals.GlobalUIScale);
						ImU8String val8 = ImU8String.op_Implicit("###SplitAmountInput");
						ref int reference = ref splitAmountInput;
						val9 = default(ImU8String);
						if (ImGui.InputInt(val8, ref reference, 0, 0, val9, (ImGuiInputTextFlags)0))
						{
							splitAmountInput = Math.Clamp(splitAmountInput, 1, 998);
						}
					}
					finally
					{
						((GroupDisposable)(ref val5)).Dispose();
					}
					Vector2 itemRectSize = ImGui.GetItemRectSize();
					ImGui.SameLine();
					DisabledDisposable val10 = ImRaii.Disabled(itemSelectCombo.SelectedID == 0);
					try
					{
						if (ImGuiOm.ButtonIconWithTextVertical((FontAwesomeIcon)61543, LanguageManager.Get("Add"), new Vector2(ImGui.CalcTextSize(ImU8String.op_Implicit("四个汉字"), false, -1f).X, itemRectSize.Y)))
						{
							SplitGroup item = new SplitGroup(itemSelectCombo.SelectedID, splitAmountInput);
							if (!config.SplitGroups.Contains(item))
							{
								config.SplitGroups.Add(item);
								config.Save(this);
							}
						}
					}
					finally
					{
						((IDisposable)val10)?.Dispose();
					}
				}
			}
			finally
			{
				((PopupDisposable)(ref val4)).Dispose();
			}
			ImGui.TableNextColumn();
			ImGuiOm.Text(LanguageManager.Get("Item"));
			ImGui.TableNextColumn();
			ImGuiOm.Text(LanguageManager.Get("AutoSplitStacks-SplitAmount"));
			foreach (SplitGroup item3 in config.SplitGroups.ToList())
			{
				ImGui.TableNextRow();
				ImGui.TableNextColumn();
				bool isEnabled = item3.IsEnabled;
				val9 = new ImU8String(13, 1);
				((ImU8String)(ref val9)).AppendLiteral("###IsEnabled_");
				((ImU8String)(ref val9)).AppendFormatted<uint>(item3.ItemID);
				if (ImGui.Checkbox(val9, ref isEnabled))
				{
					int index = config.SplitGroups.IndexOf(item3);
					config.SplitGroups[index].IsEnabled = isEnabled;
					config.Save(this);
				}
				ImGui.TableNextColumn();
				if (!LuminaGetter.TryGetRow<Item>(item3.ItemID, out Item item2))
				{
					continue;
				}
				IDalamudTextureWrap gameIcon = ImageHelper.GetGameIcon(((Item)(ref item2)).Icon);
				ImGuiOm.TextImage(((object)((Item)(ref item2)).Name/*cast due to constrained. prefix*/).ToString(), gameIcon.Handle, Globals.ScaledVector2(24f));
				ImGui.TableNextColumn();
				ImGuiOm.Selectable(item3.Amount.ToString(), selected: false, (ImGuiSelectableFlags)0);
				ImU8String val11 = new ImU8String(11, 1);
				((ImU8String)(ref val11)).AppendFormatted<uint>(item3.ItemID);
				((ImU8String)(ref val11)).AppendLiteral("_AmountEdit");
				if (ImGui.BeginPopupContextItem(val11, (ImGuiPopupFlags)1))
				{
					if (ImGui.IsWindowAppearing())
					{
						splitAmountInput = item3.Amount;
					}
					vector = KnownColor.Chocolate.ToVector4();
					ImU8String val12 = new ImU8String(1, 1);
					((ImU8String)(ref val12)).AppendFormatted<string>(LanguageManager.Get("Amount"));
					((ImU8String)(ref val12)).AppendLiteral(":");
					ImGui.TextColored(ref vector, val12);
					ImGui.SameLine();
					ImGui.SetNextItemWidth(150f * Globals.GlobalUIScale);
					ImU8String val13 = new ImU8String(13, 1);
					((ImU8String)(ref val13)).AppendLiteral("###");
					((ImU8String)(ref val13)).AppendFormatted<uint>(item3.ItemID);
					((ImU8String)(ref val13)).AppendLiteral("AmountEdit");
					ImGui.InputInt(val13, ref splitAmountInput, 0, 0, default(ImU8String), (ImGuiInputTextFlags)0);
					if (ImGui.IsItemDeactivatedAfterEdit())
					{
						int index2 = config.SplitGroups.IndexOf(item3);
						config.SplitGroups[index2].Amount = splitAmountInput;
						config.Save(this);
					}
					ImGui.EndPopup();
				}
				ImGui.TableNextColumn();
				if (ImGuiOm.ButtonIcon($"{item3.ItemID}_Enqueue", (FontAwesomeIcon)61515, LanguageManager.Get("Execute")))
				{
					EnqueueSplitByInfo(item3);
				}
				ImGui.SameLine();
				if (ImGuiOm.ButtonIcon($"{item3.ItemID}_Delete", (FontAwesomeIcon)62189, LanguageManager.Get("HoldCtrlToDelete")) && ImGui.IsKeyDown((ImGuiKey)527))
				{
					config.SplitGroups.Remove(item3);
					config.Save(this);
				}
			}
		}
		finally
		{
			((TableDisposable)(ref val3)).Dispose();
		}
	}

	private void OnCommand(string command, string args)
	{
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		args = args.Trim();
		if (string.IsNullOrWhiteSpace(args))
		{
			return;
		}
		if (int.TryParse(args, out var itemID))
		{
			SplitGroup splitGroup = config.SplitGroups.FirstOrDefault((SplitGroup x) => x.ItemID == itemID);
			if (splitGroup != null)
			{
				EnqueueSplitByInfo(splitGroup);
			}
			return;
		}
		Item item = ((IEnumerable<Item>)LuminaGetter.Get<Item>()).Where((Item x) => ((object)((Item)(ref x)).Name/*cast due to constrained. prefix*/).ToString().Contains(args, StringComparison.OrdinalIgnoreCase)).MinBy((Item x) => ((object)((Item)(ref x)).Name/*cast due to constrained. prefix*/).ToString().Length);
		if (!((object)Unsafe.As<Item, Item>(ref item)/*cast due to constrained. prefix*/).Equals((object?)null))
		{
			SplitGroup splitGroup2 = config.SplitGroups.FirstOrDefault((SplitGroup x) => x.ItemID == ((Item)(ref item)).RowId);
			if (splitGroup2 != null)
			{
				EnqueueSplitByInfo(splitGroup2);
			}
		}
	}

	private void EnqueueSplitByInfo(SplitGroup group)
	{
		EnqueueSplitByInfo(group.ItemID, group.Amount);
	}

	private void EnqueueSplitByInfo(uint itemID, int count)
	{
		base.TaskHelper.Enqueue(() => EnqueueSplit(itemID, count, 0));
	}

	private unsafe bool EnqueueSplit(uint itemID, int count, int finishRound)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Expected O, but got Unknown
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Expected O, but got Unknown
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		if (itemID == 0 || count == 0)
		{
			return true;
		}
		InventoryManager* ptr = InventoryManager.Instance();
		if (Inventories.Player.IsFull())
		{
			string text = LanguageManager.Get("AutoSplitStacks-Notification-FullInventory");
			NotifyHelper.ToastError(text);
			NotifyHelper.Chat(new XivChatEntry
			{
				Type = (XivChatType)60,
				Message = SeString.op_Implicit(text)
			});
			return true;
		}
		foreach (InventoryType item in Inventories.Player)
		{
			InventoryContainer* inventoryContainer = ((InventoryManager)ptr).GetInventoryContainer(item);
			if (inventoryContainer == null)
			{
				continue;
			}
			for (int i = 0; i < ((InventoryContainer)inventoryContainer).Size; i++)
			{
				InventoryItem* inventorySlot = ((InventoryContainer)inventoryContainer).GetInventorySlot(i);
				if (inventorySlot != null && ((InventoryItem)inventorySlot).GetBaseItemId() == itemID && ((InventoryItem)inventorySlot).GetQuantity() > count)
				{
					((InventoryManager)ptr).SplitItem(((InventoryContainer)inventoryContainer).Type, (ushort)((InventoryItem)inventorySlot).Slot, count);
					base.TaskHelper.Enqueue(() => EnqueueSplit(itemID, count, finishRound + 1));
					return true;
				}
			}
		}
		if (finishRound == 0)
		{
			string text2 = LanguageManager.Get("AutoSplitStacks-Notification-ItemNoFound");
			NotifyHelper.ToastError(text2);
			NotifyHelper.Chat(new XivChatEntry
			{
				Type = (XivChatType)60,
				Message = SeString.op_Implicit(text2)
			});
		}
		else
		{
			string text3 = LanguageManager.Get("AutoSplitStacks-Notification-Finished", finishRound, count);
			NotifyHelper.Toast(text3);
			OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(text3));
		}
		return true;
	}
}
