using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using OmenTools.Extensions;

namespace OmenTools.OmenService;

public sealed class ContextMenuOpenedArgs
{
	internal bool IsSelfInitiated { get; init; }

	public unsafe AgentInterface* Agent { get; set; }

	public unsafe AgentContext* DefaultAgentContext { get; set; }

	public unsafe AgentInventoryContext* InventoryAgentContext { get; set; }

	public unsafe AtkUnitBase* Addon { get; set; }

	public string? AddonName { get; set; }

	public uint OwnerAddonID { get; set; }

	public Vector2? Position { get; set; }

	public string? TargetName { get; set; }

	public ulong TargetObjectID { get; set; }

	public ulong TargetContentID { get; set; }

	public short TargetHomeWorldID { get; set; }

	public unsafe CharacterData* TargetCharacter { get; set; }

	public InventoryItem? TargetInventoryItem { get; set; }

	public InventoryType? TargetInventoryType { get; set; }

	public int? TargetSlot { get; set; }

	public uint TargetItemID { get; set; }

	public RowRef<Item> TargetItemRow
	{
		get
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			if (TargetItemID != 0)
			{
				return TargetItemID.ToLuminaRowRef<Item>();
			}
			return default(RowRef<Item>);
		}
	}

	public uint TargetGlamourID { get; set; }

	public RowRef<Item> TargetGlamourRow
	{
		get
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			if (TargetGlamourID != 0)
			{
				return TargetGlamourID.ToLuminaRowRef<Item>();
			}
			return default(RowRef<Item>);
		}
	}

	public unsafe bool IsTargetPlayer
	{
		get
		{
			if (TargetContentID != 0L)
			{
				return TargetCharacter != null;
			}
			return false;
		}
	}

	internal unsafe ContextMenuOpenedArgs(AgentContext* agentContext)
	{
		Agent = (AgentInterface*)agentContext;
		DefaultAgentContext = agentContext;
	}

	internal unsafe ContextMenuOpenedArgs(AgentInventoryContext* agentContext)
	{
		Agent = (AgentInterface*)agentContext;
		InventoryAgentContext = agentContext;
	}

	internal unsafe ContextMenuOpenedArgs(AgentInterface* agent)
	{
		Agent = agent;
	}

	public unsafe ContextMenuOpenedArgs()
	{
		IsSelfInitiated = true;
		DefaultAgentContext = AgentContext.Instance();
		Agent = (AgentInterface*)DefaultAgentContext;
	}

	internal unsafe ContextMenuOpenedArgs Clone()
	{
		return new ContextMenuOpenedArgs(Agent)
		{
			IsSelfInitiated = IsSelfInitiated,
			Addon = Addon,
			AddonName = AddonName,
			OwnerAddonID = OwnerAddonID,
			Position = Position,
			TargetObjectID = TargetObjectID,
			TargetContentID = TargetContentID,
			TargetHomeWorldID = TargetHomeWorldID,
			TargetName = TargetName,
			TargetCharacter = TargetCharacter,
			TargetInventoryItem = TargetInventoryItem,
			TargetItemID = TargetItemID,
			TargetGlamourID = TargetGlamourID,
			TargetInventoryType = TargetInventoryType,
			TargetSlot = TargetSlot,
			DefaultAgentContext = DefaultAgentContext,
			InventoryAgentContext = InventoryAgentContext
		};
	}

	public unsafe AgentContext* AsDefaultContext()
	{
		return DefaultAgentContext;
	}

	public unsafe AgentInventoryContext* AsInventoryContext()
	{
		return InventoryAgentContext;
	}
}
