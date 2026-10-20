using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
using Dalamud.Game.Command;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Interface;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Dalamud.Attributes;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.AddonEvent;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.ModulesPublic;

public class AutoDiscard : ModuleBase
{
	private sealed class SettingMenuItem(AutoDiscard module) : ContextMenuEntry
	{
		public override string Identifier => "AutoDiscard";

		public override ContextMenuItem? Create(ContextMenuOpenedArgs args)
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			if (module.moduleConfig.DiscardGroups.Count == 0)
			{
				return null;
			}
			InventoryItem? targetInventoryItem = args.TargetInventoryItem;
			if (targetInventoryItem.HasValue)
			{
				InventoryItem valueOrDefault = targetInventoryItem.GetValueOrDefault();
				uint itemID = ((InventoryItem)(ref valueOrDefault)).GetBaseItemId();
				if (!LuminaGetter.TryGetRow<Item>(itemID, out Item _))
				{
					return null;
				}
				string text = LanguageManager.Get("AutoDiscard-ContextMenu");
				ContextMenuItem obj = new ContextMenuItem
				{
					Name = ReadOnlySeString.op_Implicit(text)
				};
				ContextMenuSubmenu obj2 = new ContextMenuSubmenu
				{
					Title = ReadOnlySeString.op_Implicit(text)
				};
				List<ContextMenuEntry> list = new List<ContextMenuEntry>();
				list.AddRange(module.moduleConfig.DiscardGroups.Select((DiscardItemsGroup group) => new DiscardGroupMenuItem(module, group, itemID)));
				obj2.Entries = new global::_003C_003Ez__ReadOnlyList<ContextMenuEntry>(list);
				obj.Submenu = obj2;
				return obj;
			}
			return null;
		}
	}

	private sealed class DiscardGroupMenuItem(AutoDiscard module, DiscardItemsGroup group, uint itemID) : ContextMenuEntry
	{
		public override string Identifier => "AutoDiscard";

		public override ContextMenuItem Create(ContextMenuOpenedArgs args)
		{
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			if (!group.Items.Contains(itemID))
			{
				return new ContextMenuItem
				{
					Name = ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoDiscard-ContextMenu-Sub-Add", group.UniqueName)),
					OnClicked = delegate
					{
						//IL_005b: Unknown result type (might be due to invalid IL or missing references)
						group.Items.Add(itemID);
						module.moduleConfig.Save(module);
						OmenServiceBase<NotifyHelper>.Instance().Chat(LanguageManager.GetSe("AutoDiscard-Notification-Added", SeString.CreateItemLink(itemID, (ItemKind)0, (string)null), group.UniqueName));
					}
				};
			}
			return new ContextMenuItem
			{
				Name = ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoDiscard-ContextMenu-Sub-Remove", group.UniqueName)),
				OnClicked = delegate
				{
					//IL_005b: Unknown result type (might be due to invalid IL or missing references)
					group.Items.Remove(itemID);
					module.moduleConfig.Save(module);
					OmenServiceBase<NotifyHelper>.Instance().Chat(LanguageManager.GetSe("AutoDiscard-Notification-Removed", group.UniqueName, SeString.CreateItemLink(itemID, (ItemKind)0, (string)null)));
				}
			};
		}
	}

	private enum DiscardBehaviour
	{
		Discard,
		Sell
	}

	private class DiscardItemsGroup : IEquatable<DiscardItemsGroup>
	{
		public string UniqueName { get; set; }

		public HashSet<uint> Items { get; set; } = new HashSet<uint>();

		public DiscardBehaviour Behaviour { get; set; }

		public DiscardItemsGroup()
		{
		}

		public DiscardItemsGroup(string name)
		{
			UniqueName = name;
		}

		public bool Equals(DiscardItemsGroup? other)
		{
			if ((object)other == null || GetType() != other.GetType())
			{
				return false;
			}
			return UniqueName == other.UniqueName;
		}

		public unsafe void Enqueue(TaskHelper? taskHelper)
		{
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Invalid comparison between Unknown and I4
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			if (taskHelper == null)
			{
				return;
			}
			bool flag = false;
			foreach (uint item in Items)
			{
				if (!((IEnumerable<InventoryType>)Inventories.Player).TryGetItems((Predicate<InventoryItem>)((InventoryItem x) => ((InventoryItem)(ref x)).GetItemId() == item || (item < 500000 && ((InventoryItem)(ref x)).GetBaseItemId() == item)), out List<InventoryItem> itemResult) || itemResult.Count <= 0)
				{
					continue;
				}
				foreach (InventoryItem item2 in itemResult)
				{
					InventoryItem fItem = item2;
					InventoryType type = ((InventoryItem)(ref fItem)).GetInventoryType();
					short slot = fItem.Slot;
					if ((int)type == 9999 || slot < 0)
					{
						continue;
					}
					InventoryItem* itemInventory = ((InventoryManager)InventoryManager.Instance()).GetInventorySlot(type, (int)slot);
					if (itemInventory == null)
					{
						continue;
					}
					flag = true;
					if (Behaviour == DiscardBehaviour.Discard)
					{
						taskHelper.Enqueue(delegate
						{
							//IL_000c: Unknown result type (might be due to invalid IL or missing references)
							((AgentInventoryContext)AgentInventoryContext.Instance()).DiscardItem(itemInventory, type, (int)slot, (*AgentInventory.Instance()).GetActiveAddonID(), -1);
						});
						taskHelper.Enqueue(delegate
						{
							AddonSelectYesnoEvent.ClickYes();
						});
					}
					else
					{
						taskHelper.Enqueue(() => fItem.OpenContext());
						taskHelper.Enqueue(() => ClickDiscardContextMenu(taskHelper));
					}
				}
			}
			if (flag)
			{
				taskHelper.DelayNext(100);
				taskHelper.Enqueue(delegate
				{
					Enqueue(taskHelper);
				});
			}
		}

		private unsafe bool ClickDiscardContextMenu(TaskHelper? taskHelper)
		{
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			if (!(*Addons.ContextMenuAddon).IsAddonAndNodesReady())
			{
				return false;
			}
			switch (Behaviour)
			{
			case DiscardBehaviour.Discard:
				if (!AddonContextMenuEvent.Select(LuminaWrapper.GetAddonText(91u)))
				{
					((AtkUnitBase)Addons.ContextMenuAddon).Close(true);
					break;
				}
				taskHelper.Enqueue(() => AddonSelectYesnoEvent.ClickYes(), "ConfirmDiscard", 0, null, null, null, null, 1);
				break;
			case DiscardBehaviour.Sell:
				if (!AddonContextMenuEvent.Select(LuminaWrapper.GetAddonText(5480u)) && !AddonContextMenuEvent.Select(LuminaWrapper.GetAddonText(93u)))
				{
					((AtkUnitBase)Addons.ContextMenuAddon).Close(true);
					OmenServiceBase<NotifyHelper>.Instance().ChatError(ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoDiscard-NoSellPage")));
					taskHelper.Abort();
				}
				break;
			}
			return true;
		}

		public override bool Equals(object? obj)
		{
			return Equals(obj as DiscardItemsGroup);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(UniqueName);
		}

		public static bool operator ==(DiscardItemsGroup? lhs, DiscardItemsGroup? rhs)
		{
			return lhs?.Equals(rhs) ?? ((object)rhs == null);
		}

		public static bool operator !=(DiscardItemsGroup lhs, DiscardItemsGroup rhs)
		{
			return !(lhs == rhs);
		}
	}

	private class Config : ModuleConfig
	{
		public List<DiscardItemsGroup> DiscardGroups = new List<DiscardItemsGroup>();
	}

	private Config moduleConfig;

	private LuminaSearcher<Item>? itemSearcher;

	private SettingMenuItem settingMenuItem;

	private string newGroupNameInput = string.Empty;

	private string editGroupNameInput = string.Empty;

	private string itemSearchInput = string.Empty;

	private string selectedItemSearchInput = string.Empty;

	private string addItemsByNameInput = string.Empty;

	private List<Item> lastAddedItemsByName = new List<Item>();

	private uint addItemsByCategoryInput = 61u;

	private List<Item> lastAddedItemsByCategory = new List<Item>();

	private const string COMMAND = "/pdrdiscard";

	private static readonly FrozenDictionary<DiscardBehaviour, string> DiscardBehaviourLoc = new Dictionary<DiscardBehaviour, string>
	{
		[DiscardBehaviour.Discard] = LuminaWrapper.GetAddonText(91u),
		[DiscardBehaviour.Sell] = LuminaWrapper.GetAddonText(93u)
	}.ToFrozenDictionary();

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoDiscardTitle"),
		Description = LanguageManager.Get("AutoDiscardDescription"),
		Category = ModuleCategory.System
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		NeedAuth = true
	};

	protected override void Init()
	{
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Expected O, but got Unknown
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Expected O, but got Unknown
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Expected O, but got Unknown
		moduleConfig = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 2000
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		List<Item> data = (from x in (IEnumerable<Item>)LuminaGetter.Get<Item>()
			where !string.IsNullOrEmpty(((object)((Item)(ref x)).Name/*cast due to constrained. prefix*/).ToString()) && ((Item)(ref x)).ItemSortCategory.RowId != 3 && ((Item)(ref x)).ItemSortCategory.RowId != 4
			group x by ((object)((Item)(ref x)).Name/*cast due to constrained. prefix*/).ToString() into x
			select x.First()).ToList();
		if (itemSearcher == null)
		{
			itemSearcher = new LuminaSearcher<Item>(data, new Func<Item, string>[2]
			{
				(Item x) => ((object)((Item)(ref x)).Name/*cast due to constrained. prefix*/).ToString(),
				(Item x) => ((Item)(ref x)).RowId.ToString()
			});
		}
		OmenServiceBase<CommandManager>.Instance().AddCommand("/pdrdiscard", new CommandInfo(new HandlerDelegate(OnCommand))
		{
			HelpMessage = LanguageManager.Get("AutoDiscard-CommandHelp")
		});
		settingMenuItem = new SettingMenuItem(this);
		OmenServiceBase<ContextMenuManager>.Instance().Reg(settingMenuItem);
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)0, "SelectYesno", new AddonEventDelegate(OnAddon));
	}

	protected override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddon });
		OmenServiceBase<ContextMenuManager>.Instance().Unreg(settingMenuItem);
		OmenServiceBase<CommandManager>.Instance().RemoveCommand("/pdrdiscard");
		itemSearcher = null;
		lastAddedItemsByName.Clear();
		lastAddedItemsByCategory.Clear();
	}

	protected override void ConfigUI()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(1, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("Command"));
		((ImU8String)(ref val)).AppendLiteral(":");
		ImGui.TextColored(ref vector, val);
		ImGui.SameLine();
		ImU8String val2 = default(ImU8String);
		((ImU8String)(ref val2))._002Ector(3, 2);
		((ImU8String)(ref val2)).AppendFormatted<string>("/pdrdiscard");
		((ImU8String)(ref val2)).AppendLiteral(" → ");
		((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get("AutoDiscard-CommandHelp"));
		ImGui.TextUnformatted(val2);
		ImGui.Spacing();
		DrawAddNewGroupButton();
		ImGui.SameLine();
		if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)62831, LanguageManager.Get("Import")))
		{
			DiscardItemsGroup discardItemsGroup = ModuleBase.ImportFromClipboard<DiscardItemsGroup>();
			if (discardItemsGroup != null)
			{
				moduleConfig.DiscardGroups.Add(discardItemsGroup);
				moduleConfig.Save(this);
			}
		}
		Vector2 vector2 = new Vector2(ImGui.GetContentRegionAvail().X - 8f * Globals.GlobalUIScale, 0f);
		TableDisposable val3 = ImRaii.Table(ImU8String.op_Implicit("DiscardGroupTable"), 5, (ImGuiTableFlags)1984, vector2);
		try
		{
			if (!val3)
			{
				return;
			}
			float num = ImGui.CalcTextSize(ImU8String.op_Implicit((moduleConfig.DiscardGroups.Count + 1).ToString()), false, -1f).X + 24f;
			ImGui.TableSetupColumn(ImU8String.op_Implicit("Order"), (ImGuiTableColumnFlags)24, num, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("UniqueName"), (ImGuiTableColumnFlags)0, 20f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("Items"), (ImGuiTableColumnFlags)0, 80f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("Behaviour"), (ImGuiTableColumnFlags)0, 30f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("Operations"), (ImGuiTableColumnFlags)0, 30f, 0u);
			ImGui.TableNextRow((ImGuiTableRowFlags)1);
			ImGui.TableNextColumn();
			ImGui.TableNextColumn();
			ImGuiOm.Text(LanguageManager.Get("Name"));
			ImGui.TableNextColumn();
			ImGuiOm.Text(LanguageManager.Get("AutoDiscard-ItemsOverview"));
			ImGui.TableNextColumn();
			ImGuiOm.Text(LanguageManager.Get("Mode"));
			ImGui.TableNextColumn();
			ImGuiOm.Text(LanguageManager.Get("Operation"));
			for (int i = 0; i < moduleConfig.DiscardGroups.Count; i++)
			{
				IdDisposable val4 = ImRaii.PushId(i, true);
				try
				{
					ImGui.TableNextRow();
					ImGui.TableNextColumn();
					ImGuiOm.TextCentered($"{i + 1}");
					ImGui.TableNextColumn();
					UniqueNameColumn(i);
					ImGui.TableNextColumn();
					ItemsColumn(i);
					ImGui.TableNextColumn();
					BehaviourColumn(i);
					ImGui.TableNextColumn();
					OperationColumn(i);
				}
				finally
				{
					((IDisposable)val4)?.Dispose();
				}
			}
		}
		finally
		{
			((TableDisposable)(ref val3)).Dispose();
		}
	}

	private void DrawAddNewGroupButton()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61543, LanguageManager.Get("Add")))
		{
			ImGui.OpenPopup(ImU8String.op_Implicit("AddNewGroupPopup"), (ImGuiPopupFlags)0);
		}
		PopupDisposable val = ImRaii.Popup(ImU8String.op_Implicit("AddNewGroupPopup"), (ImGuiWindowFlags)64);
		try
		{
			if (!val)
			{
				return;
			}
			ImGui.SetNextItemWidth(300f * Globals.GlobalUIScale);
			ImGui.InputTextWithHint(ImU8String.op_Implicit("###NewGroupNameInput"), ImU8String.op_Implicit(LanguageManager.Get("AutoDiscard-AddNewGroupInputNameHelp")), ref newGroupNameInput, 100, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
			if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("Confirm")), default(Vector2)))
			{
				DiscardItemsGroup item = new DiscardItemsGroup(newGroupNameInput);
				if (!string.IsNullOrWhiteSpace(newGroupNameInput) && !moduleConfig.DiscardGroups.Contains(item))
				{
					moduleConfig.DiscardGroups.Add(item);
					moduleConfig.Save(this);
					newGroupNameInput = string.Empty;
					ImGui.CloseCurrentPopup();
				}
			}
			ImGui.SameLine();
			if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("Cancel")), default(Vector2)))
			{
				ImGui.CloseCurrentPopup();
			}
		}
		finally
		{
			((PopupDisposable)(ref val)).Dispose();
		}
	}

	private void UniqueNameColumn(int index)
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		if (index < 0 || index > moduleConfig.DiscardGroups.Count)
		{
			return;
		}
		DiscardItemsGroup group = moduleConfig.DiscardGroups[index];
		IdDisposable val = ImRaii.PushId(index, true);
		try
		{
			if (ImGuiOm.SelectableFillCell(group.UniqueName ?? "", selected: false, (ImGuiSelectableFlags)0))
			{
				editGroupNameInput = group.UniqueName;
				ImGui.OpenPopup(ImU8String.op_Implicit("EditGroupPopup"), (ImGuiPopupFlags)0);
			}
			PopupDisposable val2 = ImRaii.Popup(ImU8String.op_Implicit("EditGroupPopup"), (ImGuiWindowFlags)64);
			try
			{
				if (!val2)
				{
					return;
				}
				ImGui.SetNextItemWidth(300f * Globals.GlobalUIScale);
				ImGui.InputTextWithHint(ImU8String.op_Implicit("###EditGroupNameInput"), ImU8String.op_Implicit(LanguageManager.Get("AutoDiscard-AddNewGroupInputNameHelp")), ref editGroupNameInput, 100, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
				if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("Confirm")), default(Vector2)) && !string.IsNullOrWhiteSpace(editGroupNameInput) && !moduleConfig.DiscardGroups.Contains(new DiscardItemsGroup(editGroupNameInput)))
				{
					moduleConfig.DiscardGroups.FirstOrDefault((DiscardItemsGroup x) => x.UniqueName == group.UniqueName).UniqueName = editGroupNameInput;
					moduleConfig.Save(this);
					editGroupNameInput = string.Empty;
					ImGui.CloseCurrentPopup();
				}
				ImGui.SameLine();
				if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("Cancel")), default(Vector2)))
				{
					ImGui.CloseCurrentPopup();
				}
			}
			finally
			{
				((PopupDisposable)(ref val2)).Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void ItemsColumn(int index)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Invalid comparison between Unknown and I4
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_06de: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_0718: Unknown result type (might be due to invalid IL or missing references)
		//IL_0720: Unknown result type (might be due to invalid IL or missing references)
		//IL_0725: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_0727: Unknown result type (might be due to invalid IL or missing references)
		//IL_0742: Unknown result type (might be due to invalid IL or missing references)
		//IL_0751: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_079a: Unknown result type (might be due to invalid IL or missing references)
		//IL_079f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07cd: Invalid comparison between Unknown and I4
		//IL_0930: Unknown result type (might be due to invalid IL or missing references)
		//IL_0938: Unknown result type (might be due to invalid IL or missing references)
		//IL_093d: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0454: Unknown result type (might be due to invalid IL or missing references)
		//IL_0473: Unknown result type (might be due to invalid IL or missing references)
		//IL_093f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0810: Unknown result type (might be due to invalid IL or missing references)
		//IL_081f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0826: Invalid comparison between Unknown and I4
		//IL_07ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0513: Unknown result type (might be due to invalid IL or missing references)
		//IL_095a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0969: Unknown result type (might be due to invalid IL or missing references)
		//IL_084d: Unknown result type (might be due to invalid IL or missing references)
		//IL_082a: Unknown result type (might be due to invalid IL or missing references)
		//IL_082f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0549: Unknown result type (might be due to invalid IL or missing references)
		//IL_054e: Unknown result type (might be due to invalid IL or missing references)
		//IL_056d: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_09eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0607: Unknown result type (might be due to invalid IL or missing references)
		//IL_060d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0612: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a0a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a20: Unknown result type (might be due to invalid IL or missing references)
		//IL_0614: Unknown result type (might be due to invalid IL or missing references)
		//IL_0625: Unknown result type (might be due to invalid IL or missing references)
		//IL_062a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0630: Unknown result type (might be due to invalid IL or missing references)
		//IL_0635: Unknown result type (might be due to invalid IL or missing references)
		//IL_0639: Unknown result type (might be due to invalid IL or missing references)
		//IL_063e: Unknown result type (might be due to invalid IL or missing references)
		//IL_066b: Unknown result type (might be due to invalid IL or missing references)
		if (index < 0 || index > moduleConfig.DiscardGroups.Count)
		{
			return;
		}
		DiscardItemsGroup group = moduleConfig.DiscardGroups[index];
		IdDisposable val = ImRaii.PushId(index, true);
		try
		{
			ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 2.5f);
			GroupDisposable val2 = ImRaii.Group();
			try
			{
				if (group.Items.Count > 0)
				{
					GroupDisposable val3 = ImRaii.Group();
					try
					{
						GroupDisposable val4 = ImRaii.Group();
						try
						{
							foreach (uint item7 in group.Items.TakeLast(15))
							{
								ValueTuple<uint, ItemKind> baseId = ItemUtil.GetBaseId(item7);
								uint item = baseId.Item1;
								ItemKind item2 = baseId.Item2;
								Item? row = LuminaGetter.GetRow<Item>(item);
								if (row.HasValue)
								{
									Item value = row.Value;
									IDalamudTextureWrap gameIcon = ImageHelper.GetGameIcon(((Item)(ref value)).Icon, (int)item2 == 1000000);
									if (gameIcon != null)
									{
										ImGui.Image(gameIcon.Handle, new Vector2(ImGui.GetTextLineHeightWithSpacing()));
										ImGui.SameLine();
									}
								}
							}
						}
						finally
						{
							((GroupDisposable)(ref val4)).Dispose();
						}
					}
					finally
					{
						((GroupDisposable)(ref val3)).Dispose();
					}
				}
				else
				{
					ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("AutoDiscard-NoItemInGroupHelp")));
				}
			}
			finally
			{
				((GroupDisposable)(ref val2)).Dispose();
			}
			if (ImGui.IsItemClicked())
			{
				ImGui.OpenPopup(ImU8String.op_Implicit("ItemsEditMenu"), (ImGuiPopupFlags)0);
			}
			string text = string.Empty;
			PopupDisposable val5 = ImRaii.Popup(ImU8String.op_Implicit("ItemsEditMenu"), (ImGuiWindowFlags)64);
			try
			{
				if (PopupDisposable.op_Implicit(val5))
				{
					ImGui.TextUnformatted(ImU8String.op_Implicit(group.UniqueName));
					ImGui.Separator();
					ImGui.Spacing();
					if (ImGui.MenuItem(ImU8String.op_Implicit(LanguageManager.Get("AutoDiscard-AddItemsBatch")), false, true))
					{
						ImGui.CloseCurrentPopup();
						text = "AddItemsBatch";
					}
					if (ImGui.MenuItem(ImU8String.op_Implicit(LanguageManager.Get("AutoDiscard-AddItemsManual")), false, true))
					{
						ImGui.CloseCurrentPopup();
						text = "AddItemsManual";
					}
				}
			}
			finally
			{
				((PopupDisposable)(ref val5)).Dispose();
			}
			if (!string.IsNullOrEmpty(text))
			{
				ImGui.OpenPopup(ImU8String.op_Implicit(text), (ImGuiPopupFlags)0);
			}
			PopupDisposable val6 = ImRaii.Popup(ImU8String.op_Implicit("AddItemsBatch"));
			try
			{
				if (PopupDisposable.op_Implicit(val6))
				{
					Vector4 vector = KnownColor.Chocolate.ToVector4();
					ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("AutoDiscard-AddItemsByName")));
					IndentDisposable val7 = ImRaii.PushIndent(1, true);
					try
					{
						DisabledDisposable val8 = ImRaii.Disabled(string.IsNullOrWhiteSpace(addItemsByNameInput));
						try
						{
							ImU8String val9 = new ImU8String(15, 1);
							((ImU8String)(ref val9)).AppendFormatted<string>(LanguageManager.Get("Add"));
							((ImU8String)(ref val9)).AppendLiteral("##AddItemByName");
							if (ImGui.Button(val9, default(Vector2)))
							{
								lastAddedItemsByName = itemSearcher.Data.Where((Item x) => ((object)((Item)(ref x)).Name/*cast due to constrained. prefix*/).ToString().Contains(addItemsByNameInput, StringComparison.OrdinalIgnoreCase)).ToList();
								lastAddedItemsByName.ForEach(delegate(Item x)
								{
									group.Items.Add(((Item)(ref x)).RowId);
								});
								moduleConfig.Save(this);
								OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoDiscard-Notification-BatchAdded", group.Items.Count, lastAddedItemsByName.Count)));
							}
						}
						finally
						{
							((IDisposable)val8)?.Dispose();
						}
						ImGui.SameLine();
						val8 = ImRaii.Disabled(lastAddedItemsByName.Count == 0);
						try
						{
							ImU8String val10 = new ImU8String(15, 1);
							((ImU8String)(ref val10)).AppendFormatted<string>(LanguageManager.Get("Cancel"));
							((ImU8String)(ref val10)).AppendLiteral("##AddItemByName");
							if (ImGui.Button(val10, default(Vector2)))
							{
								lastAddedItemsByName.ForEach(delegate(Item x)
								{
									group.Items.Remove(((Item)(ref x)).RowId);
								});
								moduleConfig.Save(this);
								lastAddedItemsByName.Clear();
							}
						}
						finally
						{
							((IDisposable)val8)?.Dispose();
						}
						ImGui.SameLine();
						ImGui.SetNextItemWidth(300f * Globals.GlobalUIScale);
						ImGui.InputText(ImU8String.op_Implicit("###AddByItemName"), ref addItemsByNameInput, 128, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
					}
					finally
					{
						((IDisposable)val7)?.Dispose();
					}
					vector = KnownColor.Chocolate.ToVector4();
					ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("AutoDiscard-AddItemsByCategory")));
					val7 = ImRaii.PushIndent(1, true);
					try
					{
						DisabledDisposable val8 = ImRaii.Disabled(!LuminaGetter.TryGetRow<ItemUICategory>(addItemsByCategoryInput, out ItemUICategory _));
						try
						{
							ImU8String val11 = new ImU8String(19, 1);
							((ImU8String)(ref val11)).AppendFormatted<string>(LanguageManager.Get("Add"));
							((ImU8String)(ref val11)).AppendLiteral("##AddItemByCategory");
							if (ImGui.Button(val11, default(Vector2)))
							{
								lastAddedItemsByCategory = itemSearcher.Data.Where((Item x) => ((Item)(ref x)).ItemUICategory.RowId == addItemsByCategoryInput).ToList();
								lastAddedItemsByCategory.ForEach(delegate(Item x)
								{
									group.Items.Add(((Item)(ref x)).RowId);
								});
								moduleConfig.Save(this);
								OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoDiscard-Notification-BatchAdded", group.Items.Count, lastAddedItemsByName.Count)));
							}
						}
						finally
						{
							((IDisposable)val8)?.Dispose();
						}
						ImGui.SameLine();
						val8 = ImRaii.Disabled(lastAddedItemsByCategory.Count == 0);
						try
						{
							ImU8String val12 = new ImU8String(19, 1);
							((ImU8String)(ref val12)).AppendFormatted<string>(LanguageManager.Get("Cancel"));
							((ImU8String)(ref val12)).AppendLiteral("##AddItemByCategory");
							if (ImGui.Button(val12, default(Vector2)))
							{
								lastAddedItemsByCategory.ForEach(delegate(Item x)
								{
									group.Items.Remove(((Item)(ref x)).RowId);
								});
								moduleConfig.Save(this);
								lastAddedItemsByCategory.Clear();
							}
						}
						finally
						{
							((IDisposable)val8)?.Dispose();
						}
						ImGui.SameLine();
						ImGui.SetNextItemWidth(300f * Globals.GlobalUIScale);
						ItemUICategory item4;
						ComboDisposable val13 = ImRaii.Combo(ImU8String.op_Implicit("###AddItemsByCategoryCombo"), ImU8String.op_Implicit(LuminaGetter.TryGetRow<ItemUICategory>(addItemsByCategoryInput, out item4) ? ((object)((ItemUICategory)(ref item4)).Name/*cast due to constrained. prefix*/).ToString() : string.Empty), (ImGuiComboFlags)8);
						try
						{
							if (ComboDisposable.op_Implicit(val13))
							{
								Enumerator<ItemUICategory> enumerator2 = LuminaGetter.Get<ItemUICategory>().GetEnumerator();
								try
								{
									while (enumerator2.MoveNext())
									{
										ItemUICategory current = enumerator2.Current;
										string text2 = ((object)((ItemUICategory)(ref current)).Name/*cast due to constrained. prefix*/).ToString();
										if (!string.IsNullOrEmpty(text2) && ImageHelper.TryGetGameIcon((uint)((ItemUICategory)(ref current)).Icon, out IDalamudTextureWrap texture) && ImGuiOm.SelectableImageWithText(texture.Handle, new Vector2(ImGui.GetTextLineHeightWithSpacing()), text2, addItemsByCategoryInput == ((ItemUICategory)(ref current)).RowId, (ImGuiSelectableFlags)0))
										{
											addItemsByCategoryInput = ((ItemUICategory)(ref current)).RowId;
										}
									}
								}
								finally
								{
									((IDisposable)enumerator2/*cast due to constrained. prefix*/).Dispose();
								}
							}
						}
						finally
						{
							((ComboDisposable)(ref val13)).Dispose();
						}
					}
					finally
					{
						((IDisposable)val7)?.Dispose();
					}
				}
			}
			finally
			{
				((PopupDisposable)(ref val6)).Dispose();
			}
			PopupDisposable val14 = ImRaii.Popup(ImU8String.op_Implicit("AddItemsManual"));
			try
			{
				if (!PopupDisposable.op_Implicit(val14))
				{
					return;
				}
				Vector2 vector2 = new Vector2(300f * Globals.GlobalUIScale, 500f * Globals.GlobalUIScale);
				ChildDisposable val15 = ImRaii.Child(ImU8String.op_Implicit("SelectedItemChild"), vector2, true);
				try
				{
					if (ChildDisposable.op_Implicit(val15))
					{
						ImGui.SetNextItemWidth(-1f);
						ImGui.InputTextWithHint(ImU8String.op_Implicit("###SelectedItemSearchInput"), ImU8String.op_Implicit(LanguageManager.Get("PleaseSearch")), ref selectedItemSearchInput, 100, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
						ImGui.Separator();
						foreach (uint item8 in group.Items)
						{
							ValueTuple<uint, ItemKind> baseId2 = ItemUtil.GetBaseId(item8);
							uint item5 = baseId2.Item1;
							ItemKind item6 = baseId2.Item2;
							Item? row2 = LuminaGetter.GetRow<Item>(item5);
							if (row2.HasValue)
							{
								Item value2 = row2.Value;
								IDalamudTextureWrap gameIcon2 = ImageHelper.GetGameIcon(((Item)(ref value2)).Icon, (int)item6 == 1000000);
								if (gameIcon2 != null && (string.IsNullOrWhiteSpace(selectedItemSearchInput) || ((object)((Item)(ref value2)).Name/*cast due to constrained. prefix*/).ToString().Contains(selectedItemSearchInput, StringComparison.OrdinalIgnoreCase)) && ImGuiOm.SelectableImageWithText(gameIcon2.Handle, new Vector2(ImGui.GetTextLineHeightWithSpacing()), ((int)item6 == 1000000) ? $"{((Item)(ref value2)).Name} (HQ)" : ((object)((Item)(ref value2)).Name/*cast due to constrained. prefix*/).ToString(), selected: false, (ImGuiSelectableFlags)1))
								{
									group.Items.Remove(item8);
								}
							}
						}
					}
				}
				finally
				{
					((ChildDisposable)(ref val15)).Dispose();
				}
				ImGui.SameLine();
				GroupDisposable val16 = ImRaii.Group();
				try
				{
					DisabledDisposable val8 = ImRaii.Disabled();
					try
					{
						ColorDisposable val17 = ImRaii.PushColor((ImGuiCol)21, new Vector4(0f), true);
						try
						{
							ImGui.SetCursorPosY(ImGui.GetContentRegionAvail().Y / 2f - 24f);
							ImGuiOm.ButtonIcon("DecoExchangeIcon", (FontAwesomeIcon)62306);
						}
						finally
						{
							((IDisposable)val17)?.Dispose();
						}
					}
					finally
					{
						((IDisposable)val8)?.Dispose();
					}
				}
				finally
				{
					((GroupDisposable)(ref val16)).Dispose();
				}
				ImGui.SameLine();
				ChildDisposable val18 = ImRaii.Child(ImU8String.op_Implicit("SearchItemChild"), vector2, true);
				try
				{
					if (!ChildDisposable.op_Implicit(val18))
					{
						return;
					}
					ImGui.SetNextItemWidth(-1f);
					if (ImGui.InputTextWithHint(ImU8String.op_Implicit("###GameItemSearchInput"), ImU8String.op_Implicit(LanguageManager.Get("PleaseSearch")), ref itemSearchInput, 100, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null))
					{
						itemSearcher.Search(itemSearchInput);
					}
					ImGui.Separator();
					foreach (Item item9 in itemSearcher.SearchResult)
					{
						Item current3 = item9;
						if (group.Items.Contains(((Item)(ref current3)).RowId))
						{
							continue;
						}
						ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
						GameIconLookup val19 = new GameIconLookup((uint)((Item)(ref current3)).Icon, false, true, (ClientLanguage?)null);
						IDalamudTextureWrap wrapOrDefault = obj.GetFromGameIcon(ref val19).GetWrapOrDefault((IDalamudTextureWrap)null);
						if (wrapOrDefault != null && ImGuiOm.SelectableImageWithText(wrapOrDefault.Handle, new Vector2(ImGui.GetTextLineHeightWithSpacing()), ((object)((Item)(ref current3)).Name/*cast due to constrained. prefix*/).ToString(), group.Items.Contains(((Item)(ref current3)).RowId), (ImGuiSelectableFlags)1))
						{
							if (!group.Items.Remove(((Item)(ref current3)).RowId))
							{
								group.Items.Add(((Item)(ref current3)).RowId);
							}
							moduleConfig.Save(this);
						}
					}
				}
				finally
				{
					((ChildDisposable)(ref val18)).Dispose();
				}
			}
			finally
			{
				((PopupDisposable)(ref val14)).Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void BehaviourColumn(int index)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (index < 0 || index > moduleConfig.DiscardGroups.Count)
		{
			return;
		}
		DiscardItemsGroup discardItemsGroup = moduleConfig.DiscardGroups[index];
		IdDisposable val = ImRaii.PushId(index, true);
		try
		{
			foreach (KeyValuePair<DiscardBehaviour, string> item in DiscardBehaviourLoc)
			{
				if (ImGui.RadioButton(ImU8String.op_Implicit(item.Value), item.Key == discardItemsGroup.Behaviour))
				{
					discardItemsGroup.Behaviour = item.Key;
					moduleConfig.Save(this);
				}
				ImGui.SameLine();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void OperationColumn(int index)
	{
		if (index < 0 || index > moduleConfig.DiscardGroups.Count)
		{
			return;
		}
		DiscardItemsGroup discardItemsGroup = moduleConfig.DiscardGroups[index];
		IdDisposable val = ImRaii.PushId(index, true);
		try
		{
			DisabledDisposable val2 = ImRaii.Disabled(base.TaskHelper.IsBusy);
			try
			{
				if (ImGuiOm.ButtonIcon($"Run_{index}", (FontAwesomeIcon)61515, LanguageManager.Get("Run")))
				{
					discardItemsGroup.Enqueue(base.TaskHelper);
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
			ImGui.SameLine();
			if (ImGuiOm.ButtonIcon($"Stop_{index}", (FontAwesomeIcon)61517, LanguageManager.Get("Stop")))
			{
				base.TaskHelper.Abort();
			}
			val2 = ImRaii.Disabled(base.TaskHelper.IsBusy);
			try
			{
				ImGui.SameLine();
				if (ImGuiOm.ButtonIcon($"Copy_{index}", (FontAwesomeIcon)61637, LanguageManager.Get("Copy")))
				{
					DiscardItemsGroup item = new DiscardItemsGroup(GenerateUniqueName(discardItemsGroup.UniqueName))
					{
						Behaviour = discardItemsGroup.Behaviour,
						Items = discardItemsGroup.Items
					};
					moduleConfig.DiscardGroups.Add(item);
					moduleConfig.Save(this);
				}
				ImGui.SameLine();
				if (ImGuiOm.ButtonIcon($"Export_{index}", (FontAwesomeIcon)62830, LanguageManager.Get("Export")))
				{
					ModuleBase.ExportToClipboard(discardItemsGroup);
				}
				ImGui.SameLine();
				if (ImGuiOm.ButtonIcon($"Delete_{index}", (FontAwesomeIcon)62189, LanguageManager.Get("HoldCtrlToDelete")) && ImGui.IsKeyDown((ImGuiKey)527))
				{
					moduleConfig.DiscardGroups.Remove(discardItemsGroup);
					moduleConfig.Save(this);
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void OnCommand(string command, string arguments)
	{
		EnqueueDiscardGroup(arguments.Trim());
	}

	private void OnAddon(AddonEvent type, AddonArgs args)
	{
		if (base.TaskHelper.IsBusy)
		{
			AddonSelectYesnoEvent.ClickYes();
		}
	}

	public void EnqueueDiscardGroup(int index)
	{
		if (index >= 0 && index < moduleConfig.DiscardGroups.Count)
		{
			DiscardItemsGroup discardItemsGroup = moduleConfig.DiscardGroups[index];
			if (discardItemsGroup.Items.Count > 0)
			{
				discardItemsGroup.Enqueue(base.TaskHelper);
			}
		}
	}

	public void EnqueueDiscardGroup(string uniqueName)
	{
		DiscardItemsGroup discardItemsGroup = moduleConfig.DiscardGroups.FirstOrDefault((DiscardItemsGroup x) => x.UniqueName == uniqueName && x.Items.Count > 0);
		if (!(discardItemsGroup == null))
		{
			discardItemsGroup.Enqueue(base.TaskHelper);
		}
	}

	private string GenerateUniqueName(string baseName)
	{
		HashSet<string> hashSet = moduleConfig.DiscardGroups.Select((DiscardItemsGroup x) => x.UniqueName).ToHashSet();
		if (!hashSet.Contains(baseName))
		{
			return baseName;
		}
		int num = 0;
		string text = string.Empty;
		foreach (char item in baseName.Reverse())
		{
			if (char.IsDigit(item))
			{
				text = item + text;
				continue;
			}
			break;
		}
		if (text.Length > 0)
		{
			num = int.Parse(text) + 1;
			string text2 = baseName;
			int length = text.Length;
			baseName = text2.Substring(0, text2.Length - length);
		}
		string text3;
		while (true)
		{
			text3 = $"{baseName}{num}";
			if (!hashSet.Contains(text3))
			{
				break;
			}
			num++;
		}
		return text3;
	}

	[IPCProvider("DailyRoutines.Modules.AutoDiscard.IsBusy")]
	private bool IsBusy()
	{
		return base.TaskHelper.IsBusy;
	}

	[IPCProvider("DailyRoutines.Modules.AutoDiscard.EnqueueByItems")]
	private void EnqueueByItems(HashSet<uint> itemIDs)
	{
		if (itemIDs.Count == 0)
		{
			return;
		}
		DiscardItemsGroup discardItemsGroup = new DiscardItemsGroup();
		DiscardItemsGroup discardItemsGroup2 = discardItemsGroup;
		HashSet<uint> hashSet = new HashSet<uint>();
		foreach (uint itemID in itemIDs)
		{
			hashSet.Add(itemID);
		}
		discardItemsGroup2.Items = hashSet;
		discardItemsGroup.Enqueue(base.TaskHelper);
	}
}
