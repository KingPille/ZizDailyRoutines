using System;
using System.Runtime.CompilerServices;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.Sheets;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;

namespace OmenTools.Interop.Game.Helpers;

public static class TeleportCostCalculator
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int CalcDistanceCost(TerritoryTypeTelepo curRow, TerritoryTypeTelepo targetRow, uint worldWrapWidth)
	{
		int num = Math.Abs(((TerritoryTypeTelepo)(ref curRow)).X - ((TerritoryTypeTelepo)(ref targetRow)).X);
		if (num > worldWrapWidth && num < 2 * worldWrapWidth)
		{
			num = (int)(2 * worldWrapWidth) - num;
		}
		int num2 = Math.Abs(((TerritoryTypeTelepo)(ref curRow)).Y - ((TerritoryTypeTelepo)(ref targetRow)).Y);
		float num3 = MathF.Sqrt(num * num + num2 * num2);
		uint num4 = (uint)(((TerritoryTypeTelepo)(ref curRow)).Expansion + ((TerritoryTypeTelepo)(ref targetRow)).Expansion);
		return (int)MathF.Round(((float)(int)MathF.Round((float)((num4 >= 400) ? (num4 + 600) : 1000u) * num3 / 1000f) + 500f) / 5f);
	}

	public static uint GetBaseTeleportCost(Aetheryte targetAetheryteRow, uint zoneID)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		uint rowId = ((Aetheryte)(ref targetAetheryteRow)).Territory.RowId;
		if (!LuminaGetter.TryGetRow<TerritoryTypeTelepo>(zoneID, out TerritoryTypeTelepo item) || !LuminaGetter.TryGetRow<TerritoryTypeTelepo>(rowId, out TerritoryTypeTelepo item2))
		{
			return 999u;
		}
		if (!LuminaGetter.TryGetRow<TelepoRelay>(((TerritoryTypeTelepo)(ref item)).Relay.RowId, out TelepoRelay item3))
		{
			return 999u;
		}
		int num;
		if (((TerritoryTypeTelepo)(ref item)).Relay.RowId == ((TerritoryTypeTelepo)(ref item2)).Relay.RowId)
		{
			num = CalcDistanceCost(item, item2, ((TelepoRelay)(ref item3)).Unknown_70);
		}
		else
		{
			int rowId2 = (int)((TerritoryTypeTelepo)(ref item2)).Relay.RowId;
			if ((rowId2 < 0 || rowId2 > 8) ? true : false)
			{
				return 999u;
			}
			RelaysStruct val = ((TelepoRelay)(ref item3)).Relays[rowId2];
			uint rowId3 = ((RelaysStruct)(ref val)).EnterTerritory.RowId;
			uint rowId4 = ((RelaysStruct)(ref val)).ExitTerritory.RowId;
			ushort cost = ((RelaysStruct)(ref val)).Cost;
			if (rowId3 == 0 || rowId4 == 0)
			{
				return 999u;
			}
			if (!LuminaGetter.TryGetRow<TerritoryTypeTelepo>(rowId3, out TerritoryTypeTelepo item4) || !LuminaGetter.TryGetRow<TerritoryTypeTelepo>(rowId4, out TerritoryTypeTelepo item5))
			{
				return 999u;
			}
			if (!LuminaGetter.TryGetRow<TelepoRelay>(((TerritoryTypeTelepo)(ref item5)).Relay.RowId, out TelepoRelay item6))
			{
				return 999u;
			}
			int num2 = CalcDistanceCost(item, item4, ((TelepoRelay)(ref item3)).Unknown_70);
			int num3 = CalcDistanceCost(item5, item2, ((TelepoRelay)(ref item6)).Unknown_70);
			num = num2 + cost + num3;
		}
		if (num > 1000)
		{
			num = (num - 1000) / 2 + 1000;
		}
		return (uint)num;
	}

	public unsafe static uint GetTeleportCost(Aetheryte targetAetheryteRow, uint sourceZoneID, uint aetheryteRowID, byte aethernetGroup, bool isAetheryte, bool isHouse)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		uint baseTeleportCost = GetBaseTeleportCost(targetAetheryteRow, sourceZoneID);
		switch (aethernetGroup)
		{
		case 254:
			return GetBaseTeleportCost(LuminaGetter.GetRow<Aetheryte>(70u).GetValueOrDefault(), sourceZoneID);
		case 253:
			return GetBaseTeleportCost(LuminaGetter.GetRow<Aetheryte>(175u).GetValueOrDefault(), sourceZoneID);
		default:
		{
			if (!isAetheryte)
			{
				return baseTeleportCost;
			}
			if (!GameState.IsLoggedIn)
			{
				return baseTeleportCost;
			}
			PlayerState* ptr = PlayerState.Instance();
			if (ptr == null)
			{
				return baseTeleportCost;
			}
			if (((ReadOnlySpan<ushort>)((PlayerState)ptr).FreeAetheryteIds).Contains((ushort)aetheryteRowID))
			{
				return 0u;
			}
			if (((ReadOnlySpan<ushort>)((PlayerState)ptr).FavouriteAetherytes).Contains((ushort)aetheryteRowID))
			{
				return baseTeleportCost / 2;
			}
			if (((PlayerState)ptr).HomeAetheryteId == aetheryteRowID)
			{
				return baseTeleportCost;
			}
			if (isHouse)
			{
				return baseTeleportCost / 4;
			}
			return baseTeleportCost;
		}
		}
	}
}
