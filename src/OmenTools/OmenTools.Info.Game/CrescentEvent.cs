using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using Dalamud.Game.ClientState.Fates;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.InstanceContent;
using Lumina.Excel.Sheets;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;

namespace OmenTools.Info.Game;

public class CrescentEvent : IEquatable<CrescentEvent>
{
	public static readonly uint[] ForkTowers = new uint[3] { 48u, 64u, 65u };

	public static readonly uint[] MagicPots = new uint[4] { 1976u, 1977u, 2072u, 2073u };

	public uint DataID { get; }

	public CrescentEventType Type { get; }

	public uint SpecialWeaponMaterialID { get; }

	public List<uint> SpecialRewards { get; } = new List<uint>();

	public string Name { get; } = string.Empty;

	public uint IconID { get; }

	public Vector3 Position { get; set; }

	public float Radius { get; set; }

	public string NameDisplay { get; set; } = string.Empty;

	public byte Progress { get; set; }

	public FateState FateState { get; set; } = (FateState)7;

	public DynamicEventState CEState { get; set; }

	public long CELeftTimeSecond { get; set; }

	public long CEStartTime { get; set; }

	public static Dictionary<uint, uint> EventToItem { get; } = new Dictionary<uint, uint>
	{
		[1962u] = 47744u,
		[1963u] = 47744u,
		[1970u] = 47744u,
		[33u] = 47744u,
		[36u] = 47744u,
		[35u] = 47744u,
		[1977u] = 47745u,
		[1968u] = 47745u,
		[1969u] = 47745u,
		[37u] = 47745u,
		[1966u] = 47746u,
		[47u] = 47746u,
		[39u] = 47746u,
		[38u] = 47746u,
		[1965u] = 47747u,
		[1967u] = 47747u,
		[45u] = 47747u,
		[41u] = 47747u,
		[43u] = 47747u,
		[1972u] = 47748u,
		[40u] = 47748u,
		[42u] = 47748u,
		[46u] = 47748u,
		[1976u] = 47749u,
		[1964u] = 47749u,
		[1971u] = 47749u,
		[44u] = 47749u,
		[34u] = 47749u,
		[2074u] = 50974u,
		[2077u] = 50974u,
		[2081u] = 50974u,
		[2082u] = 50974u,
		[52u] = 50974u,
		[55u] = 50974u,
		[59u] = 50974u,
		[62u] = 50974u,
		[49u] = 50974u,
		[2073u] = 50975u,
		[2078u] = 50975u,
		[2075u] = 50975u,
		[2080u] = 50975u,
		[2084u] = 50975u,
		[51u] = 50975u,
		[53u] = 50975u,
		[54u] = 50975u,
		[57u] = 50975u,
		[60u] = 50975u,
		[2072u] = 50976u,
		[2076u] = 50976u,
		[2079u] = 50976u,
		[2083u] = 50976u,
		[50u] = 50976u,
		[56u] = 50976u,
		[58u] = 50976u,
		[61u] = 50976u,
		[63u] = 50976u
	};

	public static Dictionary<uint, List<uint>> CEToItems { get; }

	public static HashSet<uint> LoreItems { get; }

	public static Dictionary<uint, byte> JobItemToJob { get; }

	public unsafe static bool? IsSpecialRewardUnlocked(uint specialReward)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		bool? result = null;
		byte value;
		Item item;
		if (LocalPlayerState.GetItemCount(specialReward) != 0)
		{
			result = true;
		}
		else if (JobItemToJob.TryGetValue(specialReward, out value))
		{
			result = ((OccultCrescentState)PublicContentOccultCrescent.GetState()).SupportJobLevels[value] > 0;
		}
		else if (LoreItems.Contains(specialReward) && LuminaGetter.TryGetRow<Item>(specialReward, out item))
		{
			result = DalamudServiceExtension.Instance<IUnlockState>().IsItemUnlocked(item);
		}
		return result;
	}

	public static string GetEventTypeName(CrescentEventType type)
	{
		return type switch
		{
			CrescentEventType.FATE => LuminaWrapper.GetAddonText(2275u), 
			CrescentEventType.CE => LuminaWrapper.GetAddonText(13988u), 
			CrescentEventType.ForkTower => LuminaWrapper.GetDescriptionString(1205u), 
			CrescentEventType.MagicPot => LuminaWrapper.GetENPCName(1005475u), 
			_ => string.Empty, 
		};
	}

	public CrescentEvent(uint dataID)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		DataID = dataID;
		if (LuminaGetter.TryGetRow<DynamicEvent>(dataID, out DynamicEvent _))
		{
			Type = ((!ForkTowers.Contains(dataID)) ? CrescentEventType.CE : CrescentEventType.ForkTower);
			if (TryGetCEData(out var data))
			{
				Name = ((object)((DynamicEvent)(ref data)).Name/*cast due to constrained. prefix*/).ToString();
				NameDisplay = Name;
				DynamicEventType value = ((DynamicEvent)(ref data)).EventType.Value;
				IconID = ((DynamicEventType)(ref value)).IconObjective0;
				Radius = ((DynamicEvent)(ref data)).LGBMapRange;
			}
		}
		else
		{
			Type = (MagicPots.Contains(dataID) ? CrescentEventType.MagicPot : CrescentEventType.FATE);
			if (TryGetFateData(out var data2))
			{
				Name = ((object)((Fate)(ref data2)).Name/*cast due to constrained. prefix*/).ToString();
				NameDisplay = Name;
				IconID = ((Fate)(ref data2)).MapIcon;
			}
		}
		if (Type != CrescentEventType.ForkTower)
		{
			SpecialWeaponMaterialID = EventToItem.GetValueOrDefault(dataID, 0u);
		}
		if (Type == CrescentEventType.CE)
		{
			SpecialRewards = CEToItems.GetValueOrDefault(dataID, new List<uint>());
		}
	}

	public void UpdatePositionAndRadius(Vector3 position, float radius)
	{
		if (Radius == 0f)
		{
			Radius = radius;
		}
		if (Position == default(Vector3))
		{
			Position = position;
		}
	}

	public void UpdateTempDataFATE(string nameDisplay, byte progress, FateState fateState)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		CrescentEventType type = Type;
		if ((type == CrescentEventType.FATE || type == CrescentEventType.MagicPot) ? true : false)
		{
			Progress = progress;
			NameDisplay = nameDisplay;
			FateState = fateState;
		}
	}

	public void UpdateTempDataCE(string nameDisplay, byte progress, DynamicEventState ceState, long startTime, long leftTimeSecond)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		CrescentEventType type = Type;
		if ((type == CrescentEventType.CE || type == CrescentEventType.ForkTower) ? true : false)
		{
			Progress = progress;
			NameDisplay = nameDisplay;
			CEState = ceState;
			CELeftTimeSecond = leftTimeSecond;
			CEStartTime = startTime;
		}
	}

	public bool TryGetFateData(out Fate data)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		data = default(Fate);
		CrescentEventType type = Type;
		if ((type != CrescentEventType.FATE && type != CrescentEventType.MagicPot) || 1 == 0)
		{
			return false;
		}
		data = LuminaGetter.GetRow<Fate>(DataID).GetValueOrDefault();
		return true;
	}

	public bool TryGetCEData(out DynamicEvent data)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		data = default(DynamicEvent);
		CrescentEventType type = Type;
		if ((type != CrescentEventType.CE && type != CrescentEventType.ForkTower) || 1 == 0)
		{
			return false;
		}
		data = LuminaGetter.GetRow<DynamicEvent>(DataID).GetValueOrDefault();
		return true;
	}

	public bool Equals(CrescentEvent? other)
	{
		if (DataID == other?.DataID)
		{
			return Type == other?.Type;
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is CrescentEvent other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(DataID, (int)Type);
	}

	public static bool operator ==(CrescentEvent left, CrescentEvent right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(CrescentEvent left, CrescentEvent right)
	{
		return !left.Equals(right);
	}

	static CrescentEvent()
	{
		Dictionary<uint, List<uint>> dictionary = new Dictionary<uint, List<uint>>();
		int num = 2;
		List<uint> list = new List<uint>(num);
		CollectionsMarshal.SetCount(list, num);
		Span<uint> span = CollectionsMarshal.AsSpan(list);
		span[0] = 47730u;
		span[1] = 47751u;
		dictionary[35u] = list;
		num = 2;
		List<uint> list2 = new List<uint>(num);
		CollectionsMarshal.SetCount(list2, num);
		Span<uint> span2 = CollectionsMarshal.AsSpan(list2);
		span2[0] = 48008u;
		span2[1] = 47728u;
		dictionary[37u] = list2;
		num = 1;
		List<uint> list3 = new List<uint>(num);
		CollectionsMarshal.SetCount(list3, num);
		CollectionsMarshal.AsSpan(list3)[0] = 47729u;
		dictionary[39u] = list3;
		num = 1;
		List<uint> list4 = new List<uint>(num);
		CollectionsMarshal.SetCount(list4, num);
		CollectionsMarshal.AsSpan(list4)[0] = 47733u;
		dictionary[45u] = list4;
		num = 1;
		List<uint> list5 = new List<uint>(num);
		CollectionsMarshal.SetCount(list5, num);
		CollectionsMarshal.AsSpan(list5)[0] = 47731u;
		dictionary[41u] = list5;
		num = 1;
		List<uint> list6 = new List<uint>(num);
		CollectionsMarshal.SetCount(list6, num);
		CollectionsMarshal.AsSpan(list6)[0] = 47757u;
		dictionary[42u] = list6;
		num = 2;
		List<uint> list7 = new List<uint>(num);
		CollectionsMarshal.SetCount(list7, num);
		Span<uint> span3 = CollectionsMarshal.AsSpan(list7);
		span3[0] = 47752u;
		span3[1] = 47732u;
		dictionary[34u] = list7;
		num = 1;
		List<uint> list8 = new List<uint>(num);
		CollectionsMarshal.SetCount(list8, num);
		CollectionsMarshal.AsSpan(list8)[0] = 51979u;
		dictionary[52u] = list8;
		num = 2;
		List<uint> list9 = new List<uint>(num);
		CollectionsMarshal.SetCount(list9, num);
		Span<uint> span4 = CollectionsMarshal.AsSpan(list9);
		span4[0] = 51983u;
		span4[1] = 51972u;
		dictionary[59u] = list9;
		num = 1;
		List<uint> list10 = new List<uint>(num);
		CollectionsMarshal.SetCount(list10, num);
		CollectionsMarshal.AsSpan(list10)[0] = 51987u;
		dictionary[51u] = list10;
		num = 1;
		List<uint> list11 = new List<uint>(num);
		CollectionsMarshal.SetCount(list11, num);
		CollectionsMarshal.AsSpan(list11)[0] = 51986u;
		dictionary[53u] = list11;
		num = 1;
		List<uint> list12 = new List<uint>(num);
		CollectionsMarshal.SetCount(list12, num);
		CollectionsMarshal.AsSpan(list12)[0] = 51981u;
		dictionary[54u] = list12;
		num = 2;
		List<uint> list13 = new List<uint>(num);
		CollectionsMarshal.SetCount(list13, num);
		Span<uint> span5 = CollectionsMarshal.AsSpan(list13);
		span5[0] = 51984u;
		span5[1] = 51974u;
		dictionary[57u] = list13;
		num = 1;
		List<uint> list14 = new List<uint>(num);
		CollectionsMarshal.SetCount(list14, num);
		CollectionsMarshal.AsSpan(list14)[0] = 51980u;
		dictionary[60u] = list14;
		num = 2;
		List<uint> list15 = new List<uint>(num);
		CollectionsMarshal.SetCount(list15, num);
		Span<uint> span6 = CollectionsMarshal.AsSpan(list15);
		span6[0] = 51988u;
		span6[1] = 49827u;
		dictionary[50u] = list15;
		num = 1;
		List<uint> list16 = new List<uint>(num);
		CollectionsMarshal.SetCount(list16, num);
		CollectionsMarshal.AsSpan(list16)[0] = 51985u;
		dictionary[61u] = list16;
		num = 1;
		List<uint> list17 = new List<uint>(num);
		CollectionsMarshal.SetCount(list17, num);
		CollectionsMarshal.AsSpan(list17)[0] = 51982u;
		dictionary[63u] = list17;
		CEToItems = dictionary;
		LoreItems = new HashSet<uint>
		{
			47728u, 47729u, 47730u, 47731u, 47732u, 47733u, 47734u, 47735u, 47736u, 47737u,
			47738u, 51979u, 51980u, 51981u, 51982u, 51983u, 51984u, 51985u, 51986u, 51987u,
			51988u
		};
		JobItemToJob = new Dictionary<uint, byte>
		{
			[47751u] = 2,
			[47752u] = 4,
			[47753u] = 5,
			[47754u] = 7,
			[47755u] = 8,
			[47756u] = 9,
			[47757u] = 11,
			[47758u] = 10,
			[47759u] = 12,
			[51972u] = 21,
			[51974u] = 23
		};
	}
}
