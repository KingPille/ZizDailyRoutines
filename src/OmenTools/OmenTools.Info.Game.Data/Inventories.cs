using System;
using System.Collections.Frozen;
using System.Runtime.CompilerServices;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace OmenTools.Info.Game.Data;

public static class Inventories
{
	public static FrozenSet<InventoryType> Player { get; } = FrozenSet.Create(RuntimeHelpers.CreateSpan<InventoryType>((RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/));

	public static FrozenSet<InventoryType> Armory { get; } = FrozenSet.Create(RuntimeHelpers.CreateSpan<InventoryType>((RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/));

	public static FrozenSet<InventoryType> PlayerWithArmory { get; } = FrozenSet.Create(RuntimeHelpers.CreateSpan<InventoryType>((RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/));

	public static FrozenSet<InventoryType> Retainer { get; } = FrozenSet.Create(RuntimeHelpers.CreateSpan<InventoryType>((RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/));

	public static FrozenSet<InventoryType> RetainerAll { get; } = FrozenSet.Create(RuntimeHelpers.CreateSpan<InventoryType>((RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/));
}
