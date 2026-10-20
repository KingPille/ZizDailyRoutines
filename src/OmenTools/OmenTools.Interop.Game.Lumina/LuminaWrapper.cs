using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Info.Game.Enums;

namespace OmenTools.Interop.Game.Lumina;

public static class LuminaWrapper
{
	public static string GetQuestName(uint rowID)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!LuminaGetter.TryGetRow<Quest>(rowID, out Quest item))
		{
			return string.Empty;
		}
		return ((object)((Quest)(ref item)).Name/*cast due to constrained. prefix*/).ToString();
	}

	public static string GetAddonText(uint rowID)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!LuminaGetter.TryGetRow<Addon>(rowID, out Addon item))
		{
			return string.Empty;
		}
		return ((object)((Addon)(ref item)).Text/*cast due to constrained. prefix*/).ToString();
	}

	public static ReadOnlySeString GetAddonTextSeString(uint rowID)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (!LuminaGetter.TryGetRow<Addon>(rowID, out Addon item))
		{
			return default(ReadOnlySeString);
		}
		return ((Addon)(ref item)).Text;
	}

	public static string GetDynamicEventName(uint rowID)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!LuminaGetter.TryGetRow<DynamicEvent>(rowID, out DynamicEvent item))
		{
			return string.Empty;
		}
		return ((object)((DynamicEvent)(ref item)).Name/*cast due to constrained. prefix*/).ToString();
	}

	public static string GetDescriptionString(uint rowID)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!LuminaGetter.TryGetRow<DescriptionString>(rowID, out DescriptionString item))
		{
			return string.Empty;
		}
		return ((object)((DescriptionString)(ref item)).Text/*cast due to constrained. prefix*/).ToString();
	}

	public static string GetLogMessageText(uint rowID)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!LuminaGetter.TryGetRow<LogMessage>(rowID, out LogMessage item))
		{
			return string.Empty;
		}
		return ((object)((LogMessage)(ref item)).Text/*cast due to constrained. prefix*/).ToString();
	}

	public static string GetLobbyText(uint rowID)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!LuminaGetter.TryGetRow<Lobby>(rowID, out Lobby item))
		{
			return string.Empty;
		}
		return ((object)((Lobby)(ref item)).Text/*cast due to constrained. prefix*/).ToString();
	}

	public static string GetActionName(uint rowID)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (rowID > 100000)
		{
			if (!LuminaGetter.TryGetRow<CraftAction>(rowID, out CraftAction item))
			{
				return string.Empty;
			}
			return ((object)((CraftAction)(ref item)).Name/*cast due to constrained. prefix*/).ToString();
		}
		if (!LuminaGetter.TryGetRow<Action>(rowID, out Action item2))
		{
			return string.Empty;
		}
		return ((object)((Action)(ref item2)).Name/*cast due to constrained. prefix*/).ToString();
	}

	public static uint GetActionIconID(uint rowID)
	{
		if (rowID > 100000)
		{
			if (!LuminaGetter.TryGetRow<CraftAction>(rowID, out CraftAction item))
			{
				return 0u;
			}
			return ((CraftAction)(ref item)).Icon;
		}
		if (!LuminaGetter.TryGetRow<Action>(rowID, out Action item2))
		{
			return 0u;
		}
		return ((Action)(ref item2)).Icon;
	}

	public static string GetGeneralActionName(uint rowID)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!LuminaGetter.TryGetRow<GeneralAction>(rowID, out GeneralAction item))
		{
			return string.Empty;
		}
		return ((object)((GeneralAction)(ref item)).Name/*cast due to constrained. prefix*/).ToString();
	}

	public static string GetChocoboRaceAbilityName(uint rowID)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!LuminaGetter.TryGetRow<ChocoboRaceAbility>(rowID, out ChocoboRaceAbility item))
		{
			return string.Empty;
		}
		return ((object)((ChocoboRaceAbility)(ref item)).Name/*cast due to constrained. prefix*/).ToString();
	}

	public static string GetWorldName(uint rowID)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!LuminaGetter.TryGetRow<World>(rowID, out World item))
		{
			return string.Empty;
		}
		return ((object)((World)(ref item)).Name/*cast due to constrained. prefix*/).ToString();
	}

	public static string GetWorldDCName(uint rowID)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (!LuminaGetter.TryGetRow<World>(rowID, out World item))
		{
			return string.Empty;
		}
		WorldDCGroupType value = ((World)(ref item)).DataCenter.Value;
		return ((object)((WorldDCGroupType)(ref value)).Name/*cast due to constrained. prefix*/).ToString();
	}

	public static uint GetWorldDC(uint rowID)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (!LuminaGetter.TryGetRow<World>(rowID, out World item))
		{
			return 0u;
		}
		return ((World)(ref item)).DataCenter.RowId;
	}

	public static string GetDataCenterName(uint rowID)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!LuminaGetter.TryGetRow<WorldDCGroupType>(rowID, out WorldDCGroupType item))
		{
			return string.Empty;
		}
		return ((object)((WorldDCGroupType)(ref item)).Name/*cast due to constrained. prefix*/).ToString();
	}

	public static string GetItemName(uint rowID)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!LuminaGetter.TryGetRow<Item>(rowID, out Item item))
		{
			return string.Empty;
		}
		return ((object)((Item)(ref item)).Name/*cast due to constrained. prefix*/).ToString();
	}

	public static string GetEventItemName(uint rowID)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!LuminaGetter.TryGetRow<EventItem>(rowID, out EventItem item))
		{
			return string.Empty;
		}
		return ((object)((EventItem)(ref item)).Name/*cast due to constrained. prefix*/).ToString();
	}

	public static uint GetItemIconID(uint rowID)
	{
		if (!LuminaGetter.TryGetRow<Item>(rowID, out Item item))
		{
			return 0u;
		}
		return ((Item)(ref item)).Icon;
	}

	public static string GetStatusName(uint rowID)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!LuminaGetter.TryGetRow<Status>(rowID, out Status item))
		{
			return string.Empty;
		}
		return ((object)((Status)(ref item)).Name/*cast due to constrained. prefix*/).ToString();
	}

	public static string GetStatusDescription(uint rowID)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!LuminaGetter.TryGetRow<Status>(rowID, out Status item))
		{
			return string.Empty;
		}
		return ((object)((Status)(ref item)).Description/*cast due to constrained. prefix*/).ToString();
	}

	public static string GetZonePlaceName(uint rowID)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (!LuminaGetter.TryGetRow<TerritoryType>(rowID, out TerritoryType item))
		{
			return string.Empty;
		}
		PlaceName? valueNullable = ((TerritoryType)(ref item)).PlaceName.ValueNullable;
		object obj;
		if (!valueNullable.HasValue)
		{
			obj = null;
		}
		else
		{
			PlaceName valueOrDefault = valueNullable.GetValueOrDefault();
			obj = ((object)((PlaceName)(ref valueOrDefault)).Name/*cast due to constrained. prefix*/).ToString();
		}
		if (obj == null)
		{
			obj = string.Empty;
		}
		return (string)obj;
	}

	public static uint GetZoneFromMap(uint rowID)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (!LuminaGetter.TryGetRow<Map>(rowID, out Map item))
		{
			return 0u;
		}
		return ((Map)(ref item)).TerritoryType.RowId;
	}

	public static string GetMountName(uint rowID)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!LuminaGetter.TryGetRow<Mount>(rowID, out Mount item))
		{
			return string.Empty;
		}
		return ((object)((Mount)(ref item)).Singular/*cast due to constrained. prefix*/).ToString();
	}

	public static string GetContentName(uint rowID)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!LuminaGetter.TryGetRow<ContentFinderCondition>(rowID, out ContentFinderCondition item))
		{
			return string.Empty;
		}
		return ((object)((ContentFinderCondition)(ref item)).Name/*cast due to constrained. prefix*/).ToString();
	}

	public static string GetContentTypeName(uint rowID)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!LuminaGetter.TryGetRow<ContentType>(rowID, out ContentType item))
		{
			return string.Empty;
		}
		return ((object)((ContentType)(ref item)).Name/*cast due to constrained. prefix*/).ToString();
	}

	public static string GetContentRouletteName(uint rowID)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!LuminaGetter.TryGetRow<ContentRoulette>(rowID, out ContentRoulette item))
		{
			return string.Empty;
		}
		return ((object)((ContentRoulette)(ref item)).Name/*cast due to constrained. prefix*/).ToString();
	}

	public static string GetDeepDungeonName(uint rowID)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!LuminaGetter.TryGetRow<DeepDungeon>(rowID, out DeepDungeon item))
		{
			return string.Empty;
		}
		return ((object)((DeepDungeon)(ref item)).Name/*cast due to constrained. prefix*/).ToString();
	}

	public static string GetPlaceName(uint rowID)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!LuminaGetter.TryGetRow<PlaceName>(rowID, out PlaceName item))
		{
			return string.Empty;
		}
		return ((object)((PlaceName)(ref item)).Name/*cast due to constrained. prefix*/).ToString();
	}

	public static string GetGatheringPointName(uint rowID)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!LuminaGetter.TryGetRow<GatheringPointName>(rowID, out GatheringPointName item))
		{
			return string.Empty;
		}
		return ((object)((GatheringPointName)(ref item)).Singular/*cast due to constrained. prefix*/).ToString();
	}

	public static string GetBNPCName(uint rowID)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!LuminaGetter.TryGetRow<BNpcName>(rowID, out BNpcName item))
		{
			return string.Empty;
		}
		return ((object)((BNpcName)(ref item)).Singular/*cast due to constrained. prefix*/).ToString();
	}

	public static string GetENPCName(uint rowID)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!LuminaGetter.TryGetRow<ENpcResident>(rowID, out ENpcResident item))
		{
			return string.Empty;
		}
		return ((object)((ENpcResident)(ref item)).Singular/*cast due to constrained. prefix*/).ToString();
	}

	public static string GetENPCTitle(uint rowID)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!LuminaGetter.TryGetRow<ENpcResident>(rowID, out ENpcResident item))
		{
			return string.Empty;
		}
		return ((object)((ENpcResident)(ref item)).Title/*cast due to constrained. prefix*/).ToString();
	}

	public static string GetEObjName(uint rowID)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!LuminaGetter.TryGetRow<EObjName>(rowID, out EObjName item))
		{
			return string.Empty;
		}
		return ((object)((EObjName)(ref item)).Singular/*cast due to constrained. prefix*/).ToString();
	}

	public static string GetJobName(uint rowID)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!LuminaGetter.TryGetRow<ClassJob>(rowID, out ClassJob item))
		{
			return string.Empty;
		}
		return ((object)((ClassJob)(ref item)).Name/*cast due to constrained. prefix*/).ToString();
	}

	public static string GetAchievementKindName(uint rowID)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!LuminaGetter.TryGetRow<AchievementKind>(rowID, out AchievementKind item))
		{
			return string.Empty;
		}
		return ((object)((AchievementKind)(ref item)).Name/*cast due to constrained. prefix*/).ToString();
	}

	public static string GetFCChestName(uint rowID)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!LuminaGetter.TryGetRow<FCChestName>(rowID, out FCChestName item))
		{
			return string.Empty;
		}
		return ((object)((FCChestName)(ref item)).Name/*cast due to constrained. prefix*/).ToString();
	}

	public static string GetMKDSupportJobName(uint rowID)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!LuminaGetter.TryGetRow<MKDSupportJob>(rowID, out MKDSupportJob item))
		{
			return string.Empty;
		}
		return ((object)((MKDSupportJob)(ref item)).Name/*cast due to constrained. prefix*/).ToString();
	}

	public static string GetMKDSupportJobDescription(uint rowID)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!LuminaGetter.TryGetRow<MKDSupportJob>(rowID, out MKDSupportJob item))
		{
			return string.Empty;
		}
		return ((object)((MKDSupportJob)(ref item)).Description/*cast due to constrained. prefix*/).ToString();
	}

	public static string GetMKDTraitName(uint rowID)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!LuminaGetter.TryGetRow<MKDTrait>(rowID, out MKDTrait item))
		{
			return string.Empty;
		}
		return ((object)((MKDTrait)(ref item)).Name/*cast due to constrained. prefix*/).ToString();
	}

	public static string GetWeatherName(uint rowID)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!LuminaGetter.TryGetRow<Weather>(rowID, out Weather item))
		{
			return string.Empty;
		}
		return ((object)((Weather)(ref item)).Name/*cast due to constrained. prefix*/).ToString();
	}

	public static uint GetWeatherIcon(uint rowID)
	{
		if (!LuminaGetter.TryGetRow<Weather>(rowID, out Weather item))
		{
			return 0u;
		}
		return (uint)((Weather)(ref item)).Icon;
	}

	public static uint GetJobIcon(uint rowID, ClassJobIconType type = ClassJobIconType.Framed)
	{
		if (!LuminaGetter.TryGetRow<ClassJob>(rowID, out ClassJob item))
		{
			return 0u;
		}
		return item.GetIcon(type);
	}

	public static uint GetMKDSupportJobIcon(uint rowID)
	{
		if (!LuminaGetter.TryGetRow<MKDSupportJob>(rowID, out MKDSupportJob item))
		{
			return 0u;
		}
		return item.GetIcon();
	}

	public static string GetFateName(uint rowID)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!LuminaGetter.TryGetRow<Fate>(rowID, out Fate item))
		{
			return string.Empty;
		}
		return ((object)((Fate)(ref item)).Name/*cast due to constrained. prefix*/).ToString();
	}

	public static string GetPetName(uint rowID)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!LuminaGetter.TryGetRow<Pet>(rowID, out Pet item))
		{
			return string.Empty;
		}
		return ((object)((Pet)(ref item)).Name/*cast due to constrained. prefix*/).ToString();
	}
}
