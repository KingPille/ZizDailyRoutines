using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Command;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Dalamud.Attributes;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.KamiToolKit.Addons;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;

namespace DailyRoutines.ModulesPublic.Interface;

public class FastGrandCompanyExchange : ModuleBase
{
	private delegate void GCShopExchangeDelegate(nint purchaseInterface, [MarshalAs(UnmanagedType.U1)] bool send);

	private unsafe delegate void GCShopReloadDelegate(GCShopEventHandler* handler);

	[StructLayout(LayoutKind.Explicit, Size = 15712)]
	private struct GCShopEventHandler
	{
		[FieldOffset(448)]
		public nint PurchaseInterface;

		[FieldOffset(456)]
		public GCShopItemSlot Slots;

		[FieldOffset(15656)]
		public byte GrandCompany;

		[FieldOffset(15657)]
		public byte SubCategory;

		[FieldOffset(15658)]
		public byte Tier;

		[FieldOffset(15700)]
		public uint SelectedDisplayIndex;

		[FieldOffset(15704)]
		public uint ExchangeCount;
	}

	[StructLayout(LayoutKind.Explicit, Size = 304)]
	private struct GCShopItemSlot
	{
		[FieldOffset(4)]
		public uint ItemID;

		[FieldOffset(12)]
		public uint CostGCSeals;

		[FieldOffset(16)]
		public uint IsValid;

		[FieldOffset(20)]
		public uint DisplayIndex;
	}

	private class DRFastGCExchange(FastGrandCompanyExchange instance) : AttachedAddon("GrandCompanyExchange")
	{
		protected unsafe override void OnSetup(AtkUnitBase* addon, Span<AtkValue> atkValues)
		{
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			VerticalListNode verticalListNode = new VerticalListNode
			{
				IsVisible = true,
				Position = base.ContentStartPosition + new Vector2(0f, 2f),
				ItemSpacing = 1f,
				Size = new Vector2(275f, 28f),
				FitContents = true
			};
			TextButtonNode node = new TextButtonNode
			{
				IsVisible = true,
				IsEnabled = true,
				Size = new Vector2(verticalListNode.Size.X - 10f, 38f),
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("Exchange")),
				OnClick = delegate
				{
					if (!instance.TaskHelper.IsBusy)
					{
						instance.EnqueueByName(instance.config.ExchangeItemName, instance.config.ExchangeItemCount);
					}
				}
			};
			verticalListNode.AddNode(node);
			verticalListNode.AddDummy(5f);
			TextNode node2 = new TextNode
			{
				IsVisible = true,
				Size = new Vector2(verticalListNode.Size.X - 20f, 24f),
				FontSize = 14u,
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("Item"))
			};
			verticalListNode.AddNode(node2);
			TextInputNode itemNameInputNode = new TextInputNode
			{
				IsVisible = true,
				Size = new Vector2(verticalListNode.Size.X - 10f, 35f),
				String = ReadOnlySeString.op_Implicit(instance.config.ExchangeItemName),
				OnInputReceived = delegate(ReadOnlySeString x)
				{
					instance.config.ExchangeItemName = ((object)(*(ReadOnlySeString*)(&x))/*cast due to constrained. prefix*/).ToString();
				}
			};
			itemNameInputNode.OnInputComplete = UpdateExchangeItem;
			itemNameInputNode.OnEditComplete = delegate
			{
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				UpdateExchangeItem(itemNameInputNode.String);
			};
			itemNameInputNode.OnUnfocused = delegate
			{
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				UpdateExchangeItem(itemNameInputNode.String);
			};
			itemNameInputNode.CursorNode.ScaleY = 1.4f;
			itemNameInputNode.CurrentTextNode.FontSize = 14u;
			itemNameInputNode.CurrentTextNode.Y += 3f;
			verticalListNode.AddNode(itemNameInputNode);
			verticalListNode.AddDummy(5f);
			TextNode node3 = new TextNode
			{
				IsVisible = true,
				Size = new Vector2(verticalListNode.Size.X - 20f, 24f),
				FontSize = 14u,
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("Amount"))
			};
			verticalListNode.AddNode(node3);
			NumericInputNode node4 = new NumericInputNode
			{
				IsVisible = true,
				Size = new Vector2(verticalListNode.Size.X - 10f, 35f),
				Step = 1,
				Min = -1,
				OnValueUpdate = delegate(int newValue)
				{
					instance.config.ExchangeItemCount = newValue;
					instance.config.ExchangeItemCount = (int)MathF.Max(-1f, instance.config.ExchangeItemCount);
					instance.config.Save(instance);
				},
				Value = instance.config.ExchangeItemCount
			};
			verticalListNode.AddNode(node4);
			verticalListNode.AttachNode(this);
		}

		private unsafe void UpdateExchangeItem(ReadOnlySeString x)
		{
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			instance.config.ExchangeItemName = ((object)(*(ReadOnlySeString*)(&x))/*cast due to constrained. prefix*/).ToString();
			byte grandCompany = ((PlayerState)PlayerState.Instance()).GrandCompany;
			byte gcRank = ((PlayerState)PlayerState.Instance()).GetGrandCompanyRank();
			GCScripShopItem val = (from d in ((IEnumerable<SubrowCollection<GCScripShopItem>>)LuminaGetter.GetSub<GCScripShopItem>()).SelectMany((SubrowCollection<GCScripShopItem> d) => (IEnumerable<GCScripShopItem>)(object)d).Where(delegate(GCScripShopItem d)
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_000c: Unknown result type (might be due to invalid IL or missing references)
					//IL_000f: Unknown result type (might be due to invalid IL or missing references)
					//IL_0014: Unknown result type (might be due to invalid IL or missing references)
					GCScripShopCategory rowOrDefault = LuminaGetter.GetRowOrDefault<GCScripShopCategory>(((GCScripShopItem)(ref d)).RowId);
					return ((GCScripShopCategory)(ref rowOrDefault)).GrandCompany.RowId == grandCompany;
				})
				where gcRank >= ((GCScripShopItem)(ref d)).RequiredGrandCompanyRank.RowId
				select d).Where(delegate(GCScripShopItem d)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				//IL_0026: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				Item? valueNullable = ((GCScripShopItem)(ref d)).Item.ValueNullable;
				object obj;
				if (!valueNullable.HasValue)
				{
					obj = null;
				}
				else
				{
					Item valueOrDefault = valueNullable.GetValueOrDefault();
					obj = ((object)((Item)(ref valueOrDefault)).Name/*cast due to constrained. prefix*/).ToString();
				}
				if (obj == null)
				{
					obj = string.Empty;
				}
				return ((string)obj).Contains(instance.config.ExchangeItemName, StringComparison.OrdinalIgnoreCase);
			}).OrderBy(delegate(GCScripShopItem d)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				//IL_0026: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				Item? valueNullable = ((GCScripShopItem)(ref d)).Item.ValueNullable;
				object obj;
				if (!valueNullable.HasValue)
				{
					obj = null;
				}
				else
				{
					Item valueOrDefault = valueNullable.GetValueOrDefault();
					obj = ((object)((Item)(ref valueOrDefault)).Name/*cast due to constrained. prefix*/).ToString();
				}
				if (obj == null)
				{
					obj = string.Empty;
				}
				return ((string)obj).Length;
			}).FirstOrDefault();
			if (string.IsNullOrWhiteSpace(instance.config.ExchangeItemName) || ((GCScripShopItem)(ref val)).RowId == 0)
			{
				instance.config.ExchangeItemName = LuminaWrapper.GetItemName(21072u);
			}
			else if (((GCScripShopItem)(ref val)).RowId != 0)
			{
				Config config = instance.config;
				Item value = ((GCScripShopItem)(ref val)).Item.Value;
				config.ExchangeItemName = ((object)((Item)(ref value)).Name/*cast due to constrained. prefix*/).ToString();
			}
			if (!(instance.config.ExchangeItemName == ((object)(*(ReadOnlySeString*)(&x))/*cast due to constrained. prefix*/).ToString()))
			{
				CloseAddonOnly();
				instance.config.Save(instance);
			}
		}
	}

	private class Config : ModuleConfig
	{
		public int ExchangeItemCount = -1;

		public string ExchangeItemName = string.Empty;
	}

	private static readonly CompSig GCShopHandlerSig = new CompSig("48 8B 05 ?? ?? ?? ?? 33 C9 40 84 FF 48 0F 45 C1 48 89 05");

	private unsafe GCShopEventHandler** GCShopHandlerPtr;

	private static readonly CompSig GCShopExchangeSig = new CompSig("84 D2 0F 84 ?? ?? ?? ?? 4C 8B DC 49 89 7B");

	private GCShopExchangeDelegate? GCShopExchange;

	private static readonly CompSig GCShopReloadSig = new CompSig("48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 48 83 EC ?? 48 8B 81 ?? ?? ?? ?? 48 8B F9 48 85 C0");

	private GCShopReloadDelegate? GCShopReload;

	private (uint ItemID, int Count, byte SubCategory, byte Tier) pendingExchange;

	private Config config;

	private DRFastGCExchange? addon;

	private const string COMMAND = "gce";

	private const int SLOT_COUNT = 50;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("FastGrandCompanyExchangeTitle"),
		Description = LanguageManager.Get("FastGrandCompanyExchangeDescription"),
		Category = ModuleCategory.Interface
	};

	[IPCProvider("DailyRoutines.Modules.FastGrandCompanyExchange.IsBusy")]
	private bool IsCurrentlyBusy => base.TaskHelper?.IsBusy ?? false;

	protected unsafe override void Init()
	{
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Expected O, but got Unknown
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		if (base.TaskHelper == null)
		{
			TaskHelper taskHelper = (base.TaskHelper = new TaskHelper());
		}
		GCShopHandlerPtr = (GCShopEventHandler**)GCShopHandlerSig.GetStatic();
		GCShopExchange = GCShopExchangeSig.GetDelegate<GCShopExchangeDelegate>();
		GCShopReload = GCShopReloadSig.GetDelegate<GCShopReloadDelegate>();
		if (addon == null)
		{
			addon = new DRFastGCExchange(this)
			{
				InternalName = "DRFastGCExchange",
				Title = ReadOnlySeString.op_Implicit(Info.Title),
				Size = new Vector2(290f, 240f),
				RememberClosePosition = true
			};
		}
		OmenServiceBase<CommandManager>.Instance().AddSubCommand("gce", new CommandInfo(new HandlerDelegate(OnCommand))
		{
			HelpMessage = LanguageManager.Get("FastGrandCompanyExchange-CommandHelp")
		});
	}

	protected override void Uninit()
	{
		OmenServiceBase<CommandManager>.Instance().RemoveSubCommand("gce");
		addon?.Dispose();
		addon = null;
	}

	protected override void ConfigUI()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(1, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("Command"));
		((ImU8String)(ref val)).AppendLiteral(":");
		ImGui.TextColored(ref vector, val);
		ImGui.SameLine();
		ImU8String val2 = default(ImU8String);
		((ImU8String)(ref val2))._002Ector(6, 2);
		((ImU8String)(ref val2)).AppendLiteral("/pdr ");
		((ImU8String)(ref val2)).AppendFormatted<string>("gce");
		((ImU8String)(ref val2)).AppendLiteral(" ");
		((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get("FastGrandCompanyExchange-CommandHelp"));
		ImGui.TextWrapped(val2);
	}

	private void OnCommand(string command, string args)
	{
		args = args.Trim();
		if (string.IsNullOrWhiteSpace(args))
		{
			return;
		}
		string[] array = args.Split(' ');
		int num = array.Length;
		if ((uint)(num - 1) <= 1u)
		{
			if (array[0] == "default")
			{
				EnqueueByName(config.ExchangeItemName, config.ExchangeItemCount);
				return;
			}
			int result;
			int itemCount = ((array.Length == 2 && int.TryParse(array[1], out result) && result >= -1) ? result : (-1));
			EnqueueByName(array[0], itemCount);
		}
	}

	private bool EnqueueByName(string itemName, int itemCount = -1)
	{
		base.TaskHelper.DelayNext(500);
		base.TaskHelper.Enqueue(() => PrepareExchange(itemName, itemCount), "军票兑换准备");
		base.TaskHelper.Enqueue((Func<bool>)ExecuteExchange, "军票兑换", 10000, (TaskAbortBehaviour?)TaskAbortBehaviour.AbortCurrent, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
		return true;
	}

	private unsafe bool PrepareExchange(string itemName, int itemCount = -1)
	{
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		pendingExchange = default((uint, int, byte, byte));
		if (GCShopHandlerPtr == null || GCShopExchange == null || GCShopReload == null)
		{
			return true;
		}
		if (itemName == "default")
		{
			itemName = config.ExchangeItemName;
			itemCount = config.ExchangeItemCount;
		}
		if (string.IsNullOrWhiteSpace(itemName) || itemCount == 0)
		{
			return true;
		}
		byte grandCompany = ((PlayerState)PlayerState.Instance()).GrandCompany;
		byte gcRank = ((PlayerState)PlayerState.Instance()).GetGrandCompanyRank();
		uint companySeals = ((InventoryManager)InventoryManager.Instance()).GetCompanySeals(grandCompany);
		if (companySeals == 0)
		{
			return true;
		}
		GCScripShopItem val = (from x in ((IEnumerable<SubrowCollection<GCScripShopItem>>)LuminaGetter.GetSub<GCScripShopItem>()).SelectMany((SubrowCollection<GCScripShopItem> x) => (IEnumerable<GCScripShopItem>)(object)x).Where(delegate(GCScripShopItem x)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				GCScripShopCategory rowOrDefault2 = LuminaGetter.GetRowOrDefault<GCScripShopCategory>(((GCScripShopItem)(ref x)).RowId);
				return ((GCScripShopCategory)(ref rowOrDefault2)).GrandCompany.RowId == grandCompany;
			})
			where ((GCScripShopItem)(ref x)).CostGCSeals != 0
			where gcRank >= ((GCScripShopItem)(ref x)).RequiredGrandCompanyRank.RowId
			select x).Where(delegate(GCScripShopItem x)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			Item? valueNullable = ((GCScripShopItem)(ref x)).Item.ValueNullable;
			object obj;
			if (!valueNullable.HasValue)
			{
				obj = null;
			}
			else
			{
				Item valueOrDefault = valueNullable.GetValueOrDefault();
				obj = ((object)((Item)(ref valueOrDefault)).Name/*cast due to constrained. prefix*/).ToString();
			}
			if (obj == null)
			{
				obj = string.Empty;
			}
			return ((string)obj).Contains(itemName, StringComparison.OrdinalIgnoreCase);
		}).OrderBy(delegate(GCScripShopItem x)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			Item? valueNullable = ((GCScripShopItem)(ref x)).Item.ValueNullable;
			object obj;
			if (!valueNullable.HasValue)
			{
				obj = null;
			}
			else
			{
				Item valueOrDefault = valueNullable.GetValueOrDefault();
				obj = ((object)((Item)(ref valueOrDefault)).Name/*cast due to constrained. prefix*/).ToString();
			}
			if (obj == null)
			{
				obj = string.Empty;
			}
			return ((string)obj).Length;
		}).FirstOrDefault();
		if (((GCScripShopItem)(ref val)).RowId == 0)
		{
			return true;
		}
		uint costGCSeals = ((GCScripShopItem)(ref val)).CostGCSeals;
		if (costGCSeals == 0)
		{
			return true;
		}
		int num = (int)(companySeals / costGCSeals);
		int num2 = Math.Min((itemCount == -1) ? num : itemCount, num);
		if (num2 == 0)
		{
			base.TaskHelper.DelayNext(100);
			return true;
		}
		GCScripShopCategory rowOrDefault = LuminaGetter.GetRowOrDefault<GCScripShopCategory>(((GCScripShopItem)(ref val)).RowId);
		pendingExchange = (ItemID: ((GCScripShopItem)(ref val)).Item.RowId, Count: num2, SubCategory: (byte)((GCScripShopCategory)(ref rowOrDefault)).SubCategory, Tier: (byte)(((GCScripShopCategory)(ref rowOrDefault)).Tier - 1));
		return true;
	}

	private unsafe bool ExecuteExchange()
	{
		if (pendingExchange.Count <= 0)
		{
			return true;
		}
		GCShopEventHandler* gCShopHandlerPtr = *GCShopHandlerPtr;
		if (gCShopHandlerPtr == null)
		{
			return true;
		}
		int num = FindDisplayIndex(gCShopHandlerPtr, pendingExchange.ItemID);
		if (num >= 0)
		{
			gCShopHandlerPtr->SelectedDisplayIndex = (uint)num;
			gCShopHandlerPtr->ExchangeCount = (uint)pendingExchange.Count;
			GCShopExchange((nint)(&gCShopHandlerPtr->PurchaseInterface), send: true);
			pendingExchange = default((uint, int, byte, byte));
			return true;
		}
		if (gCShopHandlerPtr->SubCategory != pendingExchange.SubCategory || gCShopHandlerPtr->Tier != pendingExchange.Tier)
		{
			gCShopHandlerPtr->SubCategory = pendingExchange.SubCategory;
			gCShopHandlerPtr->Tier = pendingExchange.Tier;
			GCShopReload(gCShopHandlerPtr);
		}
		return false;
	}

	private unsafe static int FindDisplayIndex(GCShopEventHandler* handler, uint itemID)
	{
		GCShopItemSlot* ptr = &handler->Slots;
		for (int i = 0; i < 50; i++)
		{
			GCShopItemSlot* ptr2 = ptr + i;
			if (ptr2->IsValid == 0)
			{
				break;
			}
			if (ptr2->ItemID == itemID && ptr2->CostGCSeals != 0 && ptr2->DisplayIndex != uint.MaxValue)
			{
				return (int)ptr2->DisplayIndex;
			}
		}
		return -1;
	}

	[IPCProvider("DailyRoutines.Modules.FastGrandCompanyExchange.EnqueueByName")]
	private bool EnqueueByNameIPC(string itemName, int itemCount)
	{
		return EnqueueByName(itemName, itemCount);
	}
}
