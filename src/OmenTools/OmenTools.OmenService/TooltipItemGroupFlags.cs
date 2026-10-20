using System;

namespace OmenTools.OmenService;

[Flags]
public enum TooltipItemGroupFlags
{
	None = 0,
	CrafterName = 1,
	Description = 2,
	Marketable = 4,
	EquipRestriction = 8,
	Bonuses = 0x10,
	Materia = 0x20,
	CraftingAndRepairs = 0x40,
	HeaderStatsGroup = 0x80,
	Effects = 0x100,
	Fledgling = 0x200,
	EquipRestrictionHeader = 0x8000,
	ShopSellingPrice = 0x80000
}
