using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;

namespace OmenTools.Interop.Game.Models.Native;

[StructLayout(LayoutKind.Explicit)]
public struct AgentFreeCompanyChest
{
	[FieldOffset(0)]
	public AgentInterface AgentInterface;

	[FieldOffset(6956)]
	public InventoryType ContextInventoryType;

	[FieldOffset(6960)]
	public short ContextInventorySlot;

	public unsafe static AgentFreeCompanyChest* Instance()
	{
		return (AgentFreeCompanyChest*)((AgentModule)AgentModule.Instance()).GetAgentByInternalId((AgentId)85);
	}

	public unsafe InventoryItem* GetContextInventoryItem()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Invalid comparison between Unknown and I4
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		InventoryManager* ptr = InventoryManager.Instance();
		if (ptr == null)
		{
			return null;
		}
		if ((int)ContextInventoryType == 9999)
		{
			return null;
		}
		return ((InventoryManager)ptr).GetInventorySlot(ContextInventoryType, (int)ContextInventorySlot);
	}
}
