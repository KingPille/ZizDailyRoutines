using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using OmenTools.Extensions;

namespace OmenTools.Interop.Game.Models.Native;

[StructLayout(LayoutKind.Explicit, Size = 176)]
public struct AgentTradeMultiple : IEnumerable<SelectedMateria>, IEnumerable
{
	[FieldOffset(0)]
	public AgentInterface AgentInterface;

	[FieldOffset(40)]
	public TradeMultiple TradeMultiple;

	public SelectedMateria this[int index] => SelectedMateria[index];

	public Span<SelectedMateria> SelectedMateria => TradeMultiple.SelectedMateria;

	public IEnumerator<SelectedMateria> GetEnumerator()
	{
		for (int i = 0; i < 5; i++)
		{
			yield return this[i];
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public unsafe void StartTransmutation()
	{
		AtkValue val = default(AtkValue);
		((AtkValue)(ref val))._002Ector();
		AtkValue val2 = default(AtkValue);
		((AtkValue)(ref val2))._002Ector();
		((AtkValue)(ref val2)).SetInt(0);
		((AgentInterface)(ref AgentInterface)).ReceiveEvent(&val, &val2, 1u, 2uL);
	}

	public unsafe void AddMateria(InventoryType inventoryType, ushort slot, uint count)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		InventoryItem* inventorySlot = ((InventoryManager)InventoryManager.Instance()).GetInventorySlot(inventoryType, (int)slot);
		TradeMultiple.CurrentSelectMateria = new SelectedMateria
		{
			Type = inventoryType,
			Slot = slot,
			ItemID = ((InventoryItem)inventorySlot).ItemId,
			SelectCount = count
		};
		((AgentId)149).SendEvent(1uL, (int)count);
	}

	public bool IsAllMateriaSelected()
	{
		return GetCurrentSelectedMateriaCount() == 5;
	}

	public uint GetCurrentSelectedMateriaCount()
	{
		return (uint)SelectedMateria.ToArray().Sum((SelectedMateria x) => x.SelectCount);
	}

	public unsafe bool IsMateriaSelected(InventoryItem* item)
	{
		return SelectedMateria.ToArray().Any((SelectedMateria x) => x.Type == ((InventoryItem)item).Container && x.Slot == ((InventoryItem)item).Slot);
	}

	public unsafe static AgentTradeMultiple* Instance()
	{
		return (AgentTradeMultiple*)((AgentModule)AgentModule.Instance()).GetAgentByInternalId((AgentId)149);
	}
}
