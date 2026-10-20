using FFXIVClientStructs.FFXIV.Client.Game;
using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.ExecuteCommand.Abstractions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Interop.Game.ExecuteCommand.Implementations;

public sealed class HousingCommand : ExecuteCommandBase
{
	public enum PageKind : uint
	{
		Page1154 = 1154u,
		Page1155,
		Page1156,
		Page1157,
		Page1158
	}

	public enum HouseTerritory : uint
	{
		Mist = 339u,
		LavenderBeds = 340u,
		Goblet = 341u,
		Shirogane = 641u,
		Empyreum = 979u
	}

	public enum InteriorDesignStyle : uint
	{
		Mist = 3u,
		LavenderBeds = 6u,
		Goblet = 9u,
		Shirogane = 12u,
		Empyreum = 15u,
		Simple = 18u,
		DarkSimple = 21u
	}

	public enum BrightnessLevel : uint
	{
		Brightest,
		VeryBright,
		Bright,
		Medium,
		Dim,
		Darkest
	}

	public static void Request(PageKind pageKind, uint houseIDHigh, uint houseID, uint pageIndex)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ToFlag(pageKind), houseIDHigh, houseID, pageIndex);
	}

	public static void Build(uint wardIndex)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.BuildHouseOnPlot, wardIndex);
	}

	public static void EnterExteriorFixtures(uint wardIndex)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.EnterExteriorFixturesState, wardIndex);
	}

	public static void EnterInteriorFixtures(uint wardIndex)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.EnterInteriorFixturesState, wardIndex);
	}

	public static void Remove(uint wardIndex)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RemoveHouseFromPlot, wardIndex);
	}

	public static void ReloadArea()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestHousingArea, 255u);
	}

	public static void RemoveFreeCompanyHouse()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RemoveFreeCompanyHouse);
	}

	public static void SetBGM(uint orchestrionRowID)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.SetHouseBackgroundMusic, orchestrionRowID);
	}

	public static void ViewDetail(uint territoryType, uint wardIndex, uint plotIndex, uint apartmentRoomIndex = 0u)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.ViewHouseDetail, territoryType, wardIndex * 256 + plotIndex, apartmentRoomIndex);
	}

	public static void Store(InventoryType inventoryType, uint inventorySlot)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected I4, but got Unknown
		var (num, num2) = GetCurrentHouseID();
		if (num != num2 || num2 != 0)
		{
			OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.StoreFurniture, num, num2, (uint)(int)inventoryType, inventorySlot);
		}
	}

	public static void SetIndoorEnvironment(BrightnessLevel light, bool enableSSAO)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.SetIndoorEnvironment, (uint)light, (!enableSSAO) ? 1u : 0u);
	}

	public static void RequestPlacard(HouseTerritory territoryType, uint wardIndex, uint houseIndex)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestHousingPlacard, (uint)territoryType, wardIndex * 256 + houseIndex);
	}

	public static void RequestLottery(HouseTerritory territoryType, uint wardIndex, uint plotIndex)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestHousingLottery, (uint)territoryType, wardIndex * 256 + plotIndex);
	}

	public static void RequestName()
	{
		var (num, num2) = GetCurrentHouseID();
		if (num != num2 || num2 != 0)
		{
			OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestHousingName, num, num2);
		}
	}

	public static void RequestGuestAccess()
	{
		var (num, num2) = GetCurrentHouseID();
		if (num != num2 || num2 != 0)
		{
			OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestHousingGuestAccess, num, num2);
		}
	}

	public static void RequestGreeting()
	{
		var (num, num2) = GetCurrentHouseID();
		if (num != num2 || num2 != 0)
		{
			OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestHousingGreeting, num, num2);
		}
	}

	public static void SaveGreeting(bool allowTeleport, bool allowEnter)
	{
		var (num, num2) = GetCurrentHouseID();
		if (num != num2 || num2 != 0)
		{
			uint param = (allowTeleport ? 1u : 0u) | (uint)(allowEnter ? 65536 : 0);
			OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.SetHousingGuestAccess, num, num2, param);
		}
	}

	public static void RequestEstateTag()
	{
		var (num, num2) = GetCurrentHouseID();
		if (num != num2 || num2 != 0)
		{
			OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestHousingEstateTag, num, num2);
		}
	}

	public static void SaveEstateTag(uint tagIndexFirst, uint tagIndexSecond, uint tagIndexThird)
	{
		var (num, num2) = GetCurrentHouseID();
		if (num != num2 || num2 != 0)
		{
			uint estateTagFlag = GetEstateTagFlag(tagIndexFirst, tagIndexSecond, tagIndexThird);
			OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.SetHousingEstateTag, num, num2, estateTagFlag);
		}
	}

	public static void RequestHousingArea(HouseTerritory territoryType, uint wardIndex)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestHousingWard, (uint)territoryType, wardIndex);
	}

	public unsafe static void MoveToFrontGate()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.MoveToHouseFrontGate, (uint)((HousingManager)HousingManager.Instance()).GetCurrentPlot());
	}

	public static void Place(InventoryType inventoryType, uint inventorySlot)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected I4, but got Unknown
		var (num, num2) = GetCurrentHouseID();
		if (num != num2 || num2 != 0)
		{
			OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.PlaceFurnish, num, num2, (uint)(int)inventoryType, inventorySlot);
		}
	}

	public static void Restore(InventoryType inventoryType, uint inventorySlot, bool toStoreRoom = false)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected I4, but got Unknown
		var (num, num2) = GetCurrentHouseID();
		if (num != num2 || num2 != 0)
		{
			OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RestoreFurniture, num, num2, (uint)(int)inventoryType, toStoreRoom ? (inventorySlot + 65536) : inventorySlot);
		}
	}

	public unsafe static void EnterFurnishState()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.EnterFurnishState, 0u, (uint)((HousingManager)HousingManager.Instance()).GetCurrentPlot());
	}

	public static void RequestInteriorDesign()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestHousingInteriorDesign, 255u);
	}

	public unsafe static void ChangeInteriorDesign(InteriorDesignStyle style)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.ChangeHousingInteriorDesign, (uint)((HousingManager)HousingManager.Instance()).GetCurrentPlot(), (uint)style);
	}

	public static void LoadExteriorAppearance()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.LoadExteriorAppearanceInventory);
	}

	public static void LoadInteriorAppearance()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.LoadInteriorAppearanceInventory);
	}

	public static void LoadExteriorFurnish()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.LoadExteriorFurnishInventory);
	}

	public static void LoadInteriorFurnish()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.LoadInteriorFurnishInventory);
	}

	public static void RequestPlacedFurnitures(bool isIndoor)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestPlacedFurnitures, isIndoor ? 1u : 0u);
	}

	public static void RequestShareHolders()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestHousingShareHolders);
	}

	public static void RequestEvictionHolders()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestHousingShareHolders, 1u);
	}

	public static void RequestStoreroomStatus(uint houseIDHigh, uint houseID)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestHousingStoreroom, houseIDHigh, houseID);
	}

	public static void RequestHousing()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestHousing);
	}

	public static void RequestList()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestHousingRetainerList);
	}

	public static void OpenBuyUI()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.OpenHousingRetainerBuyUI);
	}

	public static void SetWeapon(InventoryType inventoryType, uint inventorySlot)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected I4, but got Unknown
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.SetHousingRetainerWeapon, (uint)(int)inventoryType, inventorySlot);
	}

	public static void SetDrawnSword(bool isVisible)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.ToggleHousingRetainerWeapon, isVisible ? 1u : 0u);
	}

	private static uint GetEstateTagFlag(uint tagIndexFirst, uint tagIndexSecond, uint tagIndexThird)
	{
		return (tagIndexFirst << 16) | (tagIndexSecond << 8) | tagIndexThird;
	}

	private unsafe static (uint High, uint Low) GetCurrentHouseID()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		HousingManager* ptr = HousingManager.Instance();
		if (ptr == null)
		{
			return (High: 0u, Low: 0u);
		}
		if (((HousingManager)ptr).IndoorTerritory != null)
		{
			HouseId houseId = ((IndoorTerritory)((HousingManager)ptr).IndoorTerritory).HouseId;
			return (High: (uint)(HouseId.op_Implicit(houseId) >> 32), Low: (uint)HouseId.op_Implicit(houseId));
		}
		if (((HousingManager)ptr).OutdoorTerritory != null)
		{
			HouseId houseId2 = ((OutdoorTerritory)((HousingManager)ptr).OutdoorTerritory).HouseId;
			return (High: (uint)(HouseId.op_Implicit(houseId2) >> 32), Low: (uint)HouseId.op_Implicit(houseId2));
		}
		if (((HousingManager)ptr).WorkshopTerritory != null)
		{
			HouseId houseId3 = ((WorkshopTerritory)((HousingManager)ptr).WorkshopTerritory).HouseId;
			return (High: (uint)(HouseId.op_Implicit(houseId3) >> 32), Low: (uint)HouseId.op_Implicit(houseId3));
		}
		return (High: 0u, Low: 0u);
	}

	private static ExecuteCommandFlag ToFlag(PageKind pageKind)
	{
		return pageKind switch
		{
			PageKind.Page1154 => ExecuteCommandFlag.RequestHousingGuestBook1154, 
			PageKind.Page1155 => ExecuteCommandFlag.RequestHousingGuestBook1155, 
			PageKind.Page1156 => ExecuteCommandFlag.RequestHousingGuestBook1156, 
			PageKind.Page1157 => ExecuteCommandFlag.RequestHousingGuestBook1157, 
			_ => ExecuteCommandFlag.RequestHousingGuestBook1158, 
		};
	}
}
