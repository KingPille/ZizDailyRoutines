using System;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;

namespace OmenTools.Interop.Game.Models.Native;

[StructLayout(LayoutKind.Explicit, Size = 26832)]
public struct AgentRetainer
{
	private unsafe delegate void OpenRetainerSellDelegate(AgentRetainer* agent, InventoryType inventoryType, ushort inventorySlot);

	[StructLayout(LayoutKind.Explicit, Size = 360)]
	public struct SellListEntry
	{
		[FieldOffset(0)]
		public uint ItemID;

		[FieldOffset(8)]
		public Utf8String ItemName;

		[FieldOffset(112)]
		public int Quantity;

		[FieldOffset(120)]
		public Utf8String TotalPriceText;

		[FieldOffset(224)]
		public Utf8String UnitPriceText;

		[FieldOffset(328)]
		public ushort InventorySlot;
	}

	[FieldOffset(88)]
	public InventoryType SellItemInventoryType;

	[FieldOffset(92)]
	public ushort SellItemInventorySlot;

	[FieldOffset(19316)]
	public int SellItemTotalPrice;

	[FieldOffset(19320)]
	public int SellItemPriceLimit;

	[FieldOffset(19324)]
	public int SellItemUnitPrice;

	[FieldOffset(19328)]
	public int SellItemQuantity;

	[FieldOffset(19332)]
	public int ContextMenuIndex;

	[FieldOffset(19336)]
	public int SellListEntryCount;

	[FieldOffset(19344)]
	internal unsafe fixed byte SellListEntriesData[7200];

	[FieldOffset(26764)]
	public uint RetainerSellListAddonID;

	[FieldOffset(26768)]
	public uint RetainerSellAddonID;

	[FieldOffset(26800)]
	public unsafe ShopEventHandler* ShopEventHandler;

	private static readonly CompSig OpenRetainerSellSig = new CompSig("40 55 53 56 57 48 8D AC 24 ?? ?? ?? ?? 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 85 ?? ?? ?? ?? 48 8B 01");

	private static readonly OpenRetainerSellDelegate OpenRetainerSellPtr = OpenRetainerSellSig.GetDelegate<OpenRetainerSellDelegate>();

	public unsafe Span<SellListEntry> SellListEntries
	{
		get
		{
			fixed (byte* sellListEntriesData = SellListEntriesData)
			{
				void* pointer = sellListEntriesData;
				return new Span<SellListEntry>(pointer, 20);
			}
		}
	}

	public unsafe static AgentRetainer* Instance()
	{
		return (AgentRetainer*)((AgentModule)AgentModule.Instance()).GetAgentByInternalId((AgentId)70);
	}

	public unsafe void OpenRetainerSell(InventoryType type, ushort inventorySlot)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		fixed (AgentRetainer* agent = &this)
		{
			OpenRetainerSellPtr(agent, type, inventorySlot);
		}
	}
}
