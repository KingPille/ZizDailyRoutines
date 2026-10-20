using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Extensions;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.BaseTypes.ComponentNode;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools;
using OmenTools.Dalamud.Abstractions;
using OmenTools.Dalamud.Attributes;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Info.Game.Packets.Upstream;
using OmenTools.Interop.Game.AddonEvent;
using OmenTools.Interop.Game.Lumina;
using OmenTools.KamiToolKit.Addons;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;

namespace DailyRoutines.ModulesPublic.Interface;

public class AutoExpertDelivery : ModuleBase
{
	private class Config : ModuleConfig
	{
		public int DefaultPage = 2;

		public bool SkipWhenHQ = true;

		public bool SkipWhenMateria = true;

		public bool SkipUltimateTotemExchangeItems = true;
	}

	private class DRAutoExpertDelivery(AutoExpertDelivery instance) : AttachedAddon("GrandCompanySupplyList", (AddonEvent)1)
	{
		private static VerticalListNode ControlTabLayout;

		private static VerticalListNode SettingTabLayout;

		private static List<CheckboxNode> DefaultPageCheckboxes = new List<CheckboxNode>();

		protected override bool CanOpenAddon => !instance.TaskHelper.IsBusy;

		protected unsafe override void OnHostAddon(AddonEvent type, AddonArgs? args)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Invalid comparison between Unknown and I4
			if ((int)type == 1 && Addons.GrandCompanySupplyList != null)
			{
				(*Addons.GrandCompanySupplyList).Callback(0, instance.config.DefaultPage);
			}
		}

		protected unsafe override void OnSetup(AtkUnitBase* addon, Span<AtkValue> atkValues)
		{
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_026e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0472: Unknown result type (might be due to invalid IL or missing references)
			//IL_052e: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ad: Unknown result type (might be due to invalid IL or missing references)
			FlagHelper.UpdateFlag(ref ((AtkUnitBase)addon).Flags1A1, 4, enable: true);
			FlagHelper.UpdateFlag(ref ((AtkUnitBase)addon).Flags1A0, 128, enable: true);
			FlagHelper.UpdateFlag(ref ((AtkUnitBase)addon).Flags1A1, 64, enable: true);
			FlagHelper.UpdateFlag(ref ((AtkUnitBase)addon).Flags1A3, 1, enable: true);
			DefaultPageCheckboxes.Clear();
			TabBarNode tabNode = new TabBarNode
			{
				IsVisible = true,
				Size = new Vector2(275f, 28f),
				Position = base.ContentStartPosition,
				NavIndex = 1,
				NavDown = 3
			};
			Vector2 vector = tabNode.Position + new Vector2(0f, tabNode.Size.Y + 5f);
			tabNode.AddTab(ReadOnlySeString.op_Implicit(LanguageManager.Get("Operation")), delegate
			{
				ControlTabLayout.IsVisible = true;
				SettingTabLayout.IsVisible = false;
				ApplyControllerNavigation(tabNode);
			});
			tabNode.AddTab(ReadOnlySeString.op_Implicit(LanguageManager.Get("Settings")), delegate
			{
				ControlTabLayout.IsVisible = false;
				SettingTabLayout.IsVisible = true;
				ApplyControllerNavigation(tabNode);
			});
			tabNode.AttachNode(this);
			ControlTabLayout = new VerticalListNode
			{
				IsVisible = true,
				Position = vector + new Vector2(0f, 5f),
				ItemSpacing = 4f
			};
			TextButtonNode textButtonNode = new TextButtonNode
			{
				IsVisible = true,
				IsEnabled = true,
				Size = new Vector2(tabNode.Size.X - 10f, 42f),
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("Start")),
				OnClick = delegate
				{
					if (!instance.TaskHelper.IsBusy)
					{
						instance.EnqueueDelivery();
					}
				}
			};
			TextButtonNode textButtonNode2 = new TextButtonNode
			{
				IsVisible = true,
				IsEnabled = true,
				Size = new Vector2(tabNode.Size.X - 10f, 42f),
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("Stop")),
				OnClick = delegate
				{
					if (instance.TaskHelper.IsBusy)
					{
						instance.TaskHelper.Abort();
					}
				}
			};
			TextButtonNode textButtonNode3 = new TextButtonNode
			{
				IsVisible = true,
				IsEnabled = true,
				Size = new Vector2(tabNode.Size.X - 10f, 42f),
				String = ReadOnlySeString.op_Implicit(LuminaWrapper.GetAddonText(3280u)),
				OnClick = delegate
				{
					if (!instance.TaskHelper.IsBusy)
					{
						instance.EnqueueGrandCompanyExchangeOpen(isAutoExchange: false);
					}
				}
			};
			TextButtonNode textButtonNode4 = new TextButtonNode
			{
				IsVisible = true,
				IsEnabled = true,
				Size = new Vector2(tabNode.Size.X - 5f, 42f),
				String = ReadOnlySeString.op_Implicit(LuminaWrapper.GetAddonText(3280u) + " [" + LanguageManager.Get("Exchange") + "]"),
				OnClick = delegate
				{
					if (!instance.TaskHelper.IsBusy)
					{
						instance.EnqueueGrandCompanyExchangeOpen(isAutoExchange: true);
					}
				}
			};
			ControlTabLayout.AddNode(new global::_003C_003Ez__ReadOnlyArray<NodeBase>(new NodeBase[4] { textButtonNode, textButtonNode2, textButtonNode3, textButtonNode4 }));
			ControlTabLayout.AttachNode(this);
			SettingTabLayout = new VerticalListNode
			{
				IsVisible = false,
				Position = vector + new Vector2(5f, 3f),
				FitContents = true
			};
			CheckboxNode checkboxNode = new CheckboxNode
			{
				IsVisible = true,
				IsEnabled = true,
				IsChecked = instance.config.SkipWhenHQ,
				Size = new Vector2(100f, 27f),
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("SkipHQItem")),
				OnClick = delegate(bool x)
				{
					instance.config.SkipWhenHQ = x;
					instance.config.Save(instance);
				}
			};
			checkboxNode.Label.Width = tabNode.Size.X - 20f;
			checkboxNode.Label.AutoAdjustTextSize();
			checkboxNode.Height = (float)checkboxNode.Label.FontSize * 1.5f;
			SettingTabLayout.AddNode(checkboxNode);
			CheckboxNode checkboxNode2 = new CheckboxNode
			{
				IsVisible = true,
				IsEnabled = true,
				IsChecked = instance.config.SkipWhenMateria,
				Size = new Vector2(100f, 27f),
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoExpertDelivery-SkipMaterias")),
				OnClick = delegate(bool x)
				{
					instance.config.SkipWhenMateria = x;
					instance.config.Save(instance);
				}
			};
			checkboxNode2.Label.Width = tabNode.Size.X - 20f;
			checkboxNode2.Label.AutoAdjustTextSize();
			checkboxNode2.Height = (float)checkboxNode2.Label.FontSize * 1.5f;
			SettingTabLayout.AddNode(checkboxNode2);
			CheckboxNode checkboxNode3 = new CheckboxNode
			{
				IsVisible = true,
				IsEnabled = true,
				IsChecked = instance.config.SkipUltimateTotemExchangeItems,
				Size = new Vector2(100f, 27f),
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoExpertDelivery-SkipUltimateWeapons")),
				OnClick = delegate(bool x)
				{
					instance.config.SkipUltimateTotemExchangeItems = x;
					instance.config.Save(instance);
				}
			};
			checkboxNode3.Label.Width = tabNode.Size.X - 20f;
			checkboxNode3.Label.AutoAdjustTextSize();
			checkboxNode3.Height = (float)checkboxNode3.Label.FontSize * 1.5f;
			SettingTabLayout.AddNode(checkboxNode3);
			SettingTabLayout.AddDummy(5f);
			TextNode textNode = new TextNode
			{
				IsVisible = true,
				Size = new Vector2(tabNode.Size.X - 20f, 27f),
				FontSize = 16u,
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoExpertDelivery-DefaultPage"))
			};
			textNode.AutoAdjustTextSize();
			textNode.Height = (float)textNode.FontSize * 1.5f;
			SettingTabLayout.AddNode(textNode);
			SettingTabLayout.AddDummy(3f);
			for (uint num = 0u; num < 3; num++)
			{
				uint index = num;
				CheckboxNode defaultPageNode = new CheckboxNode
				{
					IsVisible = true,
					IsEnabled = true,
					IsChecked = (instance.config.DefaultPage == num),
					Size = new Vector2(100f, 27f),
					String = ReadOnlySeString.op_Implicit(LuminaWrapper.GetAddonText(4572 + num))
				};
				defaultPageNode.OnClick = delegate(bool x)
				{
					if (!x)
					{
						defaultPageNode.IsChecked = true;
					}
					else
					{
						instance.config.DefaultPage = (int)index;
						instance.config.Save(instance);
						for (int i = 0; i < DefaultPageCheckboxes.Count; i++)
						{
							DefaultPageCheckboxes[i].IsChecked = instance.config.DefaultPage == i;
						}
					}
				};
				DefaultPageCheckboxes.Add(defaultPageNode);
				SettingTabLayout.AddNode(defaultPageNode);
			}
			SettingTabLayout.AttachNode(this);
			ApplyControllerNavigation(tabNode);
			((AtkUnitBase)addon).FocusNode = tabNode.TabButtons[0];
		}

		private void ApplyControllerNavigation(TabBarNode tabBarNode)
		{
			VerticalListNode verticalListNode = (ControlTabLayout.IsVisible ? ControlTabLayout : SettingTabLayout);
			foreach (ComponentNode item3 in (ControlTabLayout.IsVisible ? SettingTabLayout : ControlTabLayout).Nodes.OfType<ComponentNode>())
			{
				item3.NavIndex = 0;
			}
			List<ComponentNode> list = new List<ComponentNode>();
			list.AddRange(tabBarNode.TabButtons);
			foreach (NodeBase node in verticalListNode.Nodes)
			{
				if (!(node is TextButtonNode item))
				{
					if (node is CheckboxNode item2)
					{
						list.Add(item2);
					}
				}
				else
				{
					list.Add(item);
				}
			}
			for (int i = 0; i < list.Count; i++)
			{
				list[i].NavIndex = i + 1;
				list[i].NavUp = ((i == 0) ? list.Count : i);
				list[i].NavDown = ((i == list.Count - 1) ? 1 : (i + 2));
			}
		}

		protected unsafe override bool CanCloseHostAddon(AtkUnitBase* hostAddon)
		{
			if (base.CanCloseHostAddon(hostAddon))
			{
				return !instance.TaskHelper.IsBusy;
			}
			return false;
		}
	}

	private record ExpertDeliveryItem(uint ItemID, InventoryType Container, ushort Slot, uint SealReward)
	{
		public unsafe static List<ExpertDeliveryItem> Parse()
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			List<ExpertDeliveryItem> list = new List<ExpertDeliveryItem>();
			AgentGrandCompanySupply* ptr = AgentGrandCompanySupply.Instance();
			if (ptr == null || ((AgentGrandCompanySupply)ptr).ItemArray == null)
			{
				return list;
			}
			for (uint num = 0u; num < ((AgentGrandCompanySupply)ptr).NumItems; num++)
			{
				GrandCompanyItem val = ((GrandCompanyItem*)((AgentGrandCompanySupply)ptr).ItemArray)[num];
				if (val.ItemId != 0 && !((GrandCompanyItem)(ref val)).IsBonusReward && val.ExpReward <= 0 && val.SealReward > 0)
				{
					list.Add(new ExpertDeliveryItem(val.ItemId, val.Inventory, val.Slot, (uint)val.SealReward));
				}
			}
			return list;
		}

		public unsafe void HandIn()
		{
			(*Addons.GrandCompanySupplyList).Callback(1, GetIndex());
		}

		public unsafe bool IsNeedToSkip(AutoExpertDelivery instance)
		{
			if (GetSlot() == null)
			{
				return true;
			}
			if (instance.config.SkipWhenHQ && IsHQ())
			{
				return true;
			}
			if (instance.config.SkipWhenMateria && HasMateria())
			{
				return true;
			}
			if (instance.config.SkipUltimateTotemExchangeItems && IsUltimateTotemExchangeItem())
			{
				return true;
			}
			return false;
		}

		public unsafe int GetIndex()
		{
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			if (AgentGrandCompanySupply.Instance() == null)
			{
				return -1;
			}
			AtkUnitBase* grandCompanySupplyList = Addons.GrandCompanySupplyList;
			if (!(*grandCompanySupplyList).IsAddonAndNodesReady())
			{
				return -1;
			}
			if (((AtkValue)((AtkUnitBase)grandCompanySupplyList).AtkValues).UInt != 2)
			{
				return -1;
			}
			if (((AtkValue)((byte*)((AtkUnitBase)grandCompanySupplyList).AtkValues + (nint)5 * (nint)Unsafe.SizeOf<AtkValue>())).UInt != 2)
			{
				return -1;
			}
			uint uInt = ((AtkValue)((byte*)((AtkUnitBase)grandCompanySupplyList).AtkValues + (nint)6 * (nint)Unsafe.SizeOf<AtkValue>())).UInt;
			if (uInt == 0)
			{
				return -1;
			}
			for (int i = 0; i < Math.Min(40u, uInt); i++)
			{
				uint uInt2 = ((AtkValue)((byte*)((AtkUnitBase)grandCompanySupplyList).AtkValues + (nint)(265 + i) * (nint)Unsafe.SizeOf<AtkValue>())).UInt;
				InventoryType val = (InventoryType)((AtkValue)((byte*)((AtkUnitBase)grandCompanySupplyList).AtkValues + (nint)(345 + i) * (nint)Unsafe.SizeOf<AtkValue>())).UInt;
				uint uInt3 = ((AtkValue)((byte*)((AtkUnitBase)grandCompanySupplyList).AtkValues + (nint)(385 + i) * (nint)Unsafe.SizeOf<AtkValue>())).UInt;
				if (((AtkValue)((byte*)((AtkUnitBase)grandCompanySupplyList).AtkValues + (nint)(425 + i) * (nint)Unsafe.SizeOf<AtkValue>())).UInt == ItemID && uInt3 == Slot && val == Container && uInt2 == SealReward)
				{
					return i;
				}
			}
			return -1;
		}

		public unsafe bool HasMateria()
		{
			if (!LuminaGetter.TryGetRow<Item>(ItemID, out Item item))
			{
				return false;
			}
			if (((Item)(ref item)).MateriaSlotCount <= 0)
			{
				return false;
			}
			for (int i = 0; i < Math.Min(((Item)(ref item)).MateriaSlotCount, ((InventoryItem)GetSlot()).Materia.Length); i++)
			{
				if (((InventoryItem)GetSlot()).Materia[i] != 0)
				{
					return true;
				}
			}
			return false;
		}

		public unsafe bool IsHQ()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return ((Enum)((InventoryItem)GetSlot()).Flags).HasFlag((Enum)(object)(ItemFlags)1);
		}

		public bool IsUltimateTotemExchangeItem()
		{
			return UltimateTotemExchangeItemIDs.Contains(ItemID);
		}

		public unsafe InventoryItem* GetSlot()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return ((InventoryManager)InventoryManager.Instance()).GetInventorySlot(Container, (int)Slot);
		}

		public override string ToString()
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			return $"ExpertDeliveryItem-{ItemID}_{Container}_{Slot}_{SealReward}";
		}

		[CompilerGenerated]
		public void Deconstruct(out uint ItemID, out InventoryType Container, out ushort Slot, out uint SealReward)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Expected I4, but got Unknown
			ItemID = this.ItemID;
			Container = (InventoryType)(int)this.Container;
			Slot = this.Slot;
			SealReward = this.SealReward;
		}
	}

	private Config config;

	private DRAutoExpertDelivery? addonExpertDelivery;

	private static readonly FrozenDictionary<uint, (uint EventID, uint DataID)> ZoneInfo = new Dictionary<uint, (uint, uint)>
	{
		[128u] = (1441793u, 1002388u),
		[132u] = (1441794u, 1002394u),
		[130u] = (1441795u, 1002391u)
	}.ToFrozenDictionary();

	private static readonly FrozenSet<uint> UltimateTotemItemIDs = new HashSet<uint> { 21197u, 23175u, 28633u, 36810u, 38951u, 44743u, 52321u }.ToFrozenSet();

	[CompilerGenerated]
	private static FrozenSet<uint> _003CUltimateTotemExchangeItemIDs_003Ek__BackingField;

	[IPCSubscriber("DailyRoutines.Modules.FastGrandCompanyExchange.IsBusy")]
	private IPCSubscriber<bool> IsCurrentlyBusyIPC;

	[IPCSubscriber("DailyRoutines.Modules.FastGrandCompanyExchange.EnqueueByName")]
	private IPCSubscriber<string, int, bool> EnqueueByNameIPC;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoExpertDeliveryTitle"),
		Description = LanguageManager.Get("AutoExpertDeliveryDescription"),
		Category = ModuleCategory.Interface,
		ModulesPrerequisite = new string[1] { "FastGrandCompanyExchange" }
	};

	private static FrozenSet<uint> UltimateTotemExchangeItemIDs
	{
		get
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			if (_003CUltimateTotemExchangeItemIDs_003Ek__BackingField != null)
			{
				return _003CUltimateTotemExchangeItemIDs_003Ek__BackingField;
			}
			HashSet<uint> hashSet = new HashSet<uint>();
			Enumerator<SpecialShop> enumerator = LuminaGetter.Get<SpecialShop>().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					SpecialShop current = enumerator.Current;
					Enumerator<ItemStruct> enumerator2 = ((SpecialShop)(ref current)).Item.GetEnumerator();
					try
					{
						while (enumerator2.MoveNext())
						{
							ItemStruct current2 = enumerator2.Current;
							if (!((IEnumerable<ItemCostsStruct>)(object)((ItemStruct)(ref current2)).ItemCosts).Any((ItemCostsStruct x) => UltimateTotemItemIDs.Contains(((ItemCostsStruct)(ref x)).ItemCost.RowId)))
							{
								continue;
							}
							Enumerator<ReceiveItemsStruct> enumerator3 = ((ItemStruct)(ref current2)).ReceiveItems.GetEnumerator();
							try
							{
								while (enumerator3.MoveNext())
								{
									ReceiveItemsStruct current3 = enumerator3.Current;
									if (((ReceiveItemsStruct)(ref current3)).Item.RowId != 0)
									{
										Item value = ((ReceiveItemsStruct)(ref current3)).Item.Value;
										byte filterGroup = ((Item)(ref value)).FilterGroup;
										if ((uint)(filterGroup - 1) <= 2u)
										{
											hashSet.Add(((Item)(ref value)).RowId);
										}
									}
								}
							}
							finally
							{
								((IDisposable)enumerator3/*cast due to constrained. prefix*/).Dispose();
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
				((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
			}
			return _003CUltimateTotemExchangeItemIDs_003Ek__BackingField = hashSet.ToFrozenSet();
		}
	}

	protected override void Init()
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = int.MaxValue
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		if (addonExpertDelivery == null)
		{
			addonExpertDelivery = new DRAutoExpertDelivery(this)
			{
				InternalName = "DRAutoExpertDelivery",
				Title = ReadOnlySeString.op_Implicit(Info.Title),
				Size = new Vector2(300f, 280f),
				RememberClosePosition = true
			};
		}
		_ = UltimateTotemExchangeItemIDs;
	}

	private unsafe bool EnqueueDelivery()
	{
		if (Addons.GrandCompanySupplyReward != null)
		{
			if (!(*Addons.GrandCompanySupplyReward).IsAddonAndNodesReady())
			{
				return false;
			}
			(*((AddonGrandCompanySupplyReward)Addons.GrandCompanySupplyReward).DeliverButton).Click();
			base.TaskHelper.Abort();
			base.TaskHelper.Enqueue((Func<bool>)EnqueueRefresh, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
			base.TaskHelper.Enqueue((Func<bool>)EnqueueDelivery, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
			return true;
		}
		if (Addons.SelectYesno != null)
		{
			if (!AddonSelectYesnoEvent.ClickYes())
			{
				return false;
			}
			base.TaskHelper.Abort();
			base.TaskHelper.Enqueue((Func<bool>)EnqueueDelivery, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
			return true;
		}
		if (Addons.GrandCompanySupplyList != null)
		{
			if (!(*Addons.GrandCompanySupplyList).IsAddonAndNodesReady() || ((AgentGrandCompanySupply)AgentGrandCompanySupply.Instance()).ItemArray == null || ((AtkValue)((AtkUnitBase)Addons.GrandCompanySupplyList).AtkValues).UInt != 2)
			{
				return false;
			}
			List<ExpertDeliveryItem> list = (from x in ExpertDeliveryItem.Parse()
				where x.GetIndex() != -1 && !x.IsNeedToSkip(this)
				select x).ToList();
			if (list.Count > 0)
			{
				if (IsAboutToReachTheCap(list[0].SealReward))
				{
					base.TaskHelper.Abort();
					return true;
				}
				list.First().HandIn();
				base.TaskHelper.Abort();
				base.TaskHelper.Enqueue((Func<bool>)EnqueueDelivery, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
				return true;
			}
			base.TaskHelper.Abort();
			return true;
		}
		if (!DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)32])
		{
			base.TaskHelper.Abort();
			return true;
		}
		return false;
	}

	private unsafe void EnqueueGrandCompanyExchangeOpen(bool isAutoExchange)
	{
		if (!ZoneInfo.TryGetValue(GameState.TerritoryType, out var info))
		{
			return;
		}
		base.TaskHelper.Enqueue(delegate
		{
			if (!IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()))
			{
				return true;
			}
			if ((*Addons.GrandCompanySupplyList).IsAddonAndNodesReady())
			{
				((AtkUnitBase)Addons.GrandCompanySupplyList).Close(true);
			}
			if ((*Addons.SelectString).IsAddonAndNodesReady())
			{
				((AtkUnitBase)Addons.SelectString).Close(true);
			}
			return false;
		});
		base.TaskHelper.Enqueue(delegate
		{
			new EventStartPackt(IOmenDalamudService<IObjectTable>.Instance().LocalPlayer.GameObjectID, info.EventID).Send();
		});
		base.TaskHelper.Enqueue(() => (*Addons.GrandCompanyExchange).IsAddonAndNodesReady());
		if (isAutoExchange && ManagerBase<ModuleManager>.Instance().IsModuleEnabled(typeof(FastGrandCompanyExchange)))
		{
			base.TaskHelper.Enqueue(delegate
			{
				if (IsCurrentlyBusyIPC.Value)
				{
					return true;
				}
				EnqueueByNameIPC.InvokeFunc("default", -1);
				return false;
			}, null, 5000, TaskAbortBehaviour.AbortCurrent);
			base.TaskHelper.Enqueue(() => !IsCurrentlyBusyIPC.Value, null, 5000, TaskAbortBehaviour.AbortCurrent);
			base.TaskHelper.Enqueue(() => ((AtkUnitBase)Addons.GrandCompanyExchange).Close(true));
		}
		if (((AtkValue)((byte*)((AtkUnitBase)Addons.GrandCompanySupplyList).AtkValues + (nint)8 * (nint)Unsafe.SizeOf<AtkValue>())).UInt == 0)
		{
			return;
		}
		base.TaskHelper.Enqueue(() => !(*Addons.GrandCompanyExchange).IsAddonAndNodesReady() && !IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()));
		base.TaskHelper.Enqueue(() => IOmenDalamudService<IObjectTable>.Instance().FirstOrDefault((IGameObject x) => (int)x.ObjectKind == 3 && x.DataID == info.DataID).TargetInteract());
		base.TaskHelper.Enqueue(() => AddonSelectStringEvent.Select(0));
		if (isAutoExchange)
		{
			base.TaskHelper.Enqueue((Func<bool>)EnqueueDelivery, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
		}
	}

	private unsafe static bool EnqueueRefresh()
	{
		if (Addons.GrandCompanySupplyReward != null || !(*Addons.GrandCompanySupplyList).IsAddonAndNodesReady() || ((AgentGrandCompanySupply)AgentGrandCompanySupply.Instance()).ItemArray == null)
		{
			return false;
		}
		((AgentId)96).SendEvent(0uL, 0, 2);
		return true;
	}

	private unsafe static bool IsAboutToReachTheCap(uint sealReward)
	{
		byte grandCompany = ((PlayerState)PlayerState.Instance()).GrandCompany;
		if (grandCompany == 0)
		{
			return true;
		}
		if (!LuminaGetter.TryGetRow<GrandCompanyRank>((uint)((PlayerState)PlayerState.Instance()).GetGrandCompanyRank(), out GrandCompanyRank item))
		{
			return true;
		}
		float num = 1f;
		if (LocalPlayerState.HasStatus(1078u, out var index) || LocalPlayerState.HasStatus(414u, out index))
		{
			num += (float)(int)IOmenDalamudService<IObjectTable>.Instance().LocalPlayer.StatusList[index].Param / 100f;
		}
		uint companySeals = ((InventoryManager)InventoryManager.Instance()).GetCompanySeals(grandCompany);
		uint maxSeals = ((GrandCompanyRank)(ref item)).MaxSeals;
		if (companySeals + (uint)((float)sealReward * num) > maxSeals)
		{
			OmenServiceBase<NotifyHelper>.Instance().NotificationInfo(LanguageManager.Get("AutoExpertDelivery-ReachdSealCap"));
			return true;
		}
		return false;
	}

	protected override void Uninit()
	{
		addonExpertDelivery?.Dispose();
		addonExpertDelivery = null;
	}
}
