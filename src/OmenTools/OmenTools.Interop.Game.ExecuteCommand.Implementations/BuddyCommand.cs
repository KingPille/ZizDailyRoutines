using System;
using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.ExecuteCommand.Abstractions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Interop.Game.ExecuteCommand.Implementations;

public sealed class BuddyCommand : ExecuteCommandBase
{
	public enum Category : uint
	{
		Defender,
		Attacker,
		Healer
	}

	public enum Part : uint
	{
		Head,
		Body,
		Legs
	}

	public enum ActionType : uint
	{
		Withdraw = 2u,
		Follow,
		FreeStance,
		DefenderStance,
		AttackerStance,
		HealerStance
	}

	public static void UseAction(ActionType actionType)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.SetBuddyAction, (uint)actionType);
	}

	public static void Equip(Part part, uint buddyEquipRowID)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.SetBuddyEquip, (uint)part, buddyEquipRowID);
	}

	public static void Unequip(Part part)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.SetBuddyEquip, (uint)part);
	}

	public static void LearnSkill(Category category, uint level)
	{
		ArgumentOutOfRangeException.ThrowIfLessThan(level, 1u, "level");
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.LearnBuddySkill, (uint)((level - 1) * 3 + category));
	}
}
