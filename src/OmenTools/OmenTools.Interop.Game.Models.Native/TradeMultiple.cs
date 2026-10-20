using System;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.Sheets;
using OmenTools.Interop.Game.Lumina;

namespace OmenTools.Interop.Game.Models.Native;

[StructLayout(LayoutKind.Explicit, Size = 136)]
public struct TradeMultiple
{
	[FieldOffset(0)]
	public unsafe nint* VirtualTable;

	[FieldOffset(8)]
	public nint UnkInstane;

	[FieldOffset(32)]
	public int MaxCount;

	[FieldOffset(40)]
	public unsafe fixed byte SelectedMateriaData[96];

	public unsafe Span<SelectedMateria> SelectedMateria
	{
		get
		{
			fixed (byte* selectedMateriaData = SelectedMateriaData)
			{
				void* pointer = selectedMateriaData;
				return new Span<SelectedMateria>(pointer, 6);
			}
		}
	}

	public SelectedMateria CurrentSelectMateria
	{
		get
		{
			return SelectedMateria[5];
		}
		set
		{
			SelectedMateria[5] = value;
		}
	}

	public unsafe void AddMateria(InventoryType inventoryType, ushort slot)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		InventoryItem* inventorySlot = ((InventoryManager)InventoryManager.Instance()).GetInventorySlot(inventoryType, (int)slot);
		if (inventorySlot == null || ((InventoryItem)inventorySlot).ItemId == 0)
		{
			return;
		}
		Item? row = LuminaGetter.GetRow<Item>(((InventoryItem)inventorySlot).ItemId);
		if (!row.HasValue)
		{
			return;
		}
		Item valueOrDefault = row.GetValueOrDefault();
		if (((Item)(ref valueOrDefault)).FilterGroup == 13)
		{
			fixed (TradeMultiple* ptr = &this)
			{
				void* ptr2 = ptr;
				((delegate* unmanaged[Stdcall]<nint, uint, InventoryType, void>)(*VirtualTable))((nint)ptr2, slot, inventoryType);
			}
		}
	}
}
