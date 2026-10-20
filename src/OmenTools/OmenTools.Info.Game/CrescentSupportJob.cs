using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Client.Game.InstanceContent;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Common.Math;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;

namespace OmenTools.Info.Game;

public class CrescentSupportJob : IEquatable<CrescentSupportJob>
{
	public static CrescentSupportJob Freelancer { get; } = new CrescentSupportJob(0, CrescentSupportJobType.Freelancer);

	public static CrescentSupportJob Knight { get; } = new CrescentSupportJob(1, CrescentSupportJobType.Knight, CrescentSupportJobUnlockType.None, 0u, 41589u, 4233u);

	public static CrescentSupportJob Berserker { get; } = new CrescentSupportJob(2, CrescentSupportJobType.Berserker, CrescentSupportJobUnlockType.CriticalEncounter, 35u);

	public static CrescentSupportJob Monk { get; } = new CrescentSupportJob(3, CrescentSupportJobType.Monk, CrescentSupportJobUnlockType.None, 0u, 41597u, 4239u);

	public static CrescentSupportJob Ranger { get; } = new CrescentSupportJob(4, CrescentSupportJobType.Ranger, CrescentSupportJobUnlockType.CriticalEncounter, 34u);

	public static CrescentSupportJob Samurai { get; } = new CrescentSupportJob(5, CrescentSupportJobType.Samurai, CrescentSupportJobUnlockType.GoldPiece, 45044u);

	public static CrescentSupportJob Bard { get; } = new CrescentSupportJob(6, CrescentSupportJobType.Bard, CrescentSupportJobUnlockType.None, 0u, 41609u, 4244u);

	public static CrescentSupportJob Geomancer { get; } = new CrescentSupportJob(7, CrescentSupportJobType.Geomancer, CrescentSupportJobUnlockType.GoldPiece, 45044u);

	public static CrescentSupportJob TimeMage { get; } = new CrescentSupportJob(8, CrescentSupportJobType.TimeMage, CrescentSupportJobUnlockType.SilverPiece, 45043u);

	public static CrescentSupportJob Cannoneer { get; } = new CrescentSupportJob(9, CrescentSupportJobType.Cannoneer, CrescentSupportJobUnlockType.SilverPiece, 45043u);

	public static CrescentSupportJob Chemist { get; } = new CrescentSupportJob(10, CrescentSupportJobType.Chemist, CrescentSupportJobUnlockType.SilverPiece, 45043u);

	public static CrescentSupportJob Oracle { get; } = new CrescentSupportJob(11, CrescentSupportJobType.Oracle, CrescentSupportJobUnlockType.CriticalEncounter, 42u);

	public static CrescentSupportJob Thief { get; } = new CrescentSupportJob(12, CrescentSupportJobType.Thief, CrescentSupportJobUnlockType.GoldPiece, 45044u);

	public static CrescentSupportJob MysticKnight { get; } = new CrescentSupportJob(13, CrescentSupportJobType.MysticKnight, CrescentSupportJobUnlockType.SilverPiece, 45043u);

	public static CrescentSupportJob Gladiator { get; } = new CrescentSupportJob(14, CrescentSupportJobType.Gladiator, CrescentSupportJobUnlockType.GoldPiece, 45044u);

	public static CrescentSupportJob Dancer { get; } = new CrescentSupportJob(15, CrescentSupportJobType.Dancer, CrescentSupportJobUnlockType.SilverPiece, 45043u, 46603u, 4798u);

	public static CrescentSupportJob Ninja { get; } = new CrescentSupportJob(16, CrescentSupportJobType.Ninja, CrescentSupportJobUnlockType.SilverObol, 51975u);

	public static CrescentSupportJob WhiteMage { get; } = new CrescentSupportJob(17, CrescentSupportJobType.WhiteMage, CrescentSupportJobUnlockType.SilverObol, 51975u);

	public static CrescentSupportJob BlackMage { get; } = new CrescentSupportJob(18, CrescentSupportJobType.BlackMage, CrescentSupportJobUnlockType.SilverObol, 51975u);

	public static CrescentSupportJob Dragoon { get; } = new CrescentSupportJob(19, CrescentSupportJobType.Dragoon, CrescentSupportJobUnlockType.GoldObol, 51976u);

	public static CrescentSupportJob Summoner { get; } = new CrescentSupportJob(20, CrescentSupportJobType.Summoner, CrescentSupportJobUnlockType.GoldObol, 51976u);

	public static CrescentSupportJob BlueMage { get; } = new CrescentSupportJob(21, CrescentSupportJobType.BlueMage, CrescentSupportJobUnlockType.CriticalEncounter, 59u);

	public static CrescentSupportJob RedMage { get; } = new CrescentSupportJob(22, CrescentSupportJobType.RedMage, CrescentSupportJobUnlockType.SilverObol, 51975u);

	public static CrescentSupportJob Necromancer { get; } = new CrescentSupportJob(23, CrescentSupportJobType.Necromancer, CrescentSupportJobUnlockType.CriticalEncounter, 57u);

	public static List<CrescentSupportJob> AllJobs { get; }

	public byte DataID { get; }

	public CrescentSupportJobType JobType { get; }

	public CrescentSupportJobUnlockType UnlockType { get; }

	public uint UnlockLink { get; }

	public uint LongTimeStatusActionID { get; init; }

	public uint LongTimeStatusID { get; init; }

	public Dictionary<uint, byte> Actions { get; init; } = new Dictionary<uint, byte>();

	public Dictionary<uint, byte> Traits { get; init; } = new Dictionary<uint, byte>();

	public string Name
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			MKDSupportJob data = GetData();
			return ((object)((MKDSupportJob)(ref data)).Name/*cast due to constrained. prefix*/).ToString();
		}
	}

	public string UnlockTypeName => UnlockType switch
	{
		CrescentSupportJobUnlockType.CriticalEncounter => LuminaWrapper.GetAddonText(13988u), 
		CrescentSupportJobUnlockType.SilverPiece => LuminaWrapper.GetENPCName(1053614u) + " (" + LuminaWrapper.GetENPCTitle(1053614u) + ")", 
		CrescentSupportJobUnlockType.GoldPiece => LuminaWrapper.GetENPCName(1053614u) + " (" + LuminaWrapper.GetENPCTitle(1053614u) + ")", 
		CrescentSupportJobUnlockType.SilverObol => LuminaWrapper.GetENPCName(1059485u) + " (" + LuminaWrapper.GetENPCTitle(1059485u) + ")", 
		CrescentSupportJobUnlockType.GoldObol => LuminaWrapper.GetENPCName(1059485u) + " (" + LuminaWrapper.GetENPCTitle(1059485u) + ")", 
		_ => string.Empty, 
	};

	public string UnlockLinkName => UnlockType switch
	{
		CrescentSupportJobUnlockType.CriticalEncounter => LuminaWrapper.GetDynamicEventName(UnlockLink), 
		CrescentSupportJobUnlockType.SilverPiece => LuminaWrapper.GetItemName(UnlockLink) + " x1000", 
		CrescentSupportJobUnlockType.GoldPiece => LuminaWrapper.GetItemName(UnlockLink) + " x1600", 
		CrescentSupportJobUnlockType.SilverObol => LuminaWrapper.GetItemName(UnlockLink) + " x1000", 
		CrescentSupportJobUnlockType.GoldObol => LuminaWrapper.GetItemName(UnlockLink) + " x1600", 
		_ => string.Empty, 
	};

	public byte MaxLevel
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			MKDSupportJob data = GetData();
			return ((MKDSupportJob)(ref data)).LevelMax;
		}
	}

	public unsafe byte CurrentLevel
	{
		get
		{
			OccultCrescentState* state = PublicContentOccultCrescent.GetState();
			if (state == null)
			{
				return 0;
			}
			return ((OccultCrescentState)state).SupportJobLevels[DataID];
		}
	}

	public unsafe static CrescentSupportJob? GetCurrentSupportJob()
	{
		OccultCrescentState* state = PublicContentOccultCrescent.GetState();
		if (state == null)
		{
			return null;
		}
		return AllJobs[((OccultCrescentState)state).CurrentSupportJob];
	}

	public unsafe static bool TryFindKnowledgeCrystal([NotNullWhen(true)] out IGameObject? knowledgeCrystal)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Invalid comparison between Unknown and I4
		knowledgeCrystal = null;
		if ((int)GameState.TerritoryIntendedUse != 61)
		{
			return false;
		}
		knowledgeCrystal = IOmenDalamudService<IObjectTable>.Instance().SearchObject((IGameObject x) => x is IEventObj eventObj && (int)x.ObjectKind == 7 && x.DataID == 2007457 && string.IsNullOrEmpty(eventObj.Name) && ((GameObject)eventObj.ToStruct()).EventState == 0 && ((GameObject)eventObj.ToStruct()).CameraOffset != Vector3.Zero, IObjectTable.EventRange);
		return knowledgeCrystal != null;
	}

	public CrescentSupportJob(byte dataID, CrescentSupportJobType jobType, CrescentSupportJobUnlockType unlockType = CrescentSupportJobUnlockType.None, uint unlockLink = 0u, uint longTimeStatusActionID = 0u, uint longTimeStatusID = 0u)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		DataID = dataID;
		JobType = jobType;
		UnlockType = unlockType;
		UnlockLink = unlockLink;
		LongTimeStatusActionID = longTimeStatusActionID;
		LongTimeStatusID = longTimeStatusID;
		MKDSupportJob data = GetData();
		Enumerator<ActionsStruct> enumerator = ((MKDSupportJob)(ref data)).Actions.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				ActionsStruct current = enumerator.Current;
				uint rowId = ((ActionsStruct)(ref current)).Action.RowId;
				byte levelUnlock = ((ActionsStruct)(ref current)).LevelUnlock;
				if (rowId != 0 && levelUnlock != 0)
				{
					Actions[rowId] = levelUnlock;
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
		Enumerator<MKDTrait> enumerator2 = LuminaGetter.Get<MKDTrait>().GetEnumerator();
		try
		{
			while (enumerator2.MoveNext())
			{
				MKDTrait current2 = enumerator2.Current;
				if (((MKDTrait)(ref current2)).Icon != -1 && ((MKDTrait)(ref current2)).MKDSupportJob.RowId == DataID)
				{
					byte levelUnlock2 = ((MKDTrait)(ref current2)).LevelUnlock;
					Traits[((MKDTrait)(ref current2)).RowId] = levelUnlock2;
				}
			}
		}
		finally
		{
			((IDisposable)enumerator2/*cast due to constrained. prefix*/).Dispose();
		}
		Traits = Traits.OrderBy((KeyValuePair<uint, byte> x) => x.Value).ToDictionary((KeyValuePair<uint, byte> x) => x.Key, (KeyValuePair<uint, byte> x) => x.Value);
	}

	public unsafe bool IsUnlocked()
	{
		if (PublicContentOccultCrescent.GetState() == null)
		{
			return false;
		}
		return CurrentLevel > 0;
	}

	public bool IsActionUnlocked(uint actionID)
	{
		if (Actions.TryGetValue(actionID, out var value) && value <= CurrentLevel)
		{
			return ActionManagerExtension.IsActionUnlocked(actionID);
		}
		return false;
	}

	public bool IsLongTimeStatusUnlocked()
	{
		if (LongTimeStatusActionID == 0 || LongTimeStatusID == 0)
		{
			return false;
		}
		return IsActionUnlocked(LongTimeStatusActionID);
	}

	public bool IsWithLongTimeStatus()
	{
		int index;
		if (LongTimeStatusID != 0)
		{
			return LocalPlayerState.HasStatus(LongTimeStatusID, out index);
		}
		return false;
	}

	public unsafe bool ChangeTo()
	{
		OccultCrescentState* state = PublicContentOccultCrescent.GetState();
		if (state == null)
		{
			return false;
		}
		if (((OccultCrescentState)state).CurrentSupportJob == DataID)
		{
			return false;
		}
		((AgentMKDSupportJobList)AgentMKDSupportJobList.Instance()).ChangeSupportJob(DataID);
		return true;
	}

	public unsafe bool IsThisJob()
	{
		OccultCrescentState* state = PublicContentOccultCrescent.GetState();
		if (state == null)
		{
			return false;
		}
		return ((OccultCrescentState)state).CurrentSupportJob == DataID;
	}

	public MKDSupportJob GetData()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return LuminaGetter.GetRow<MKDSupportJob>((uint)DataID).GetValueOrDefault();
	}

	public bool Equals(CrescentSupportJob? other)
	{
		return DataID == other?.DataID;
	}

	public override bool Equals(object? obj)
	{
		if (obj is CrescentSupportJob other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return DataID;
	}

	public static bool operator ==(CrescentSupportJob left, CrescentSupportJob right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(CrescentSupportJob left, CrescentSupportJob right)
	{
		return !left.Equals(right);
	}

	static CrescentSupportJob()
	{
		int num = 24;
		List<CrescentSupportJob> list = new List<CrescentSupportJob>(num);
		CollectionsMarshal.SetCount(list, num);
		Span<CrescentSupportJob> span = CollectionsMarshal.AsSpan(list);
		span[0] = Freelancer;
		span[1] = Knight;
		span[2] = Berserker;
		span[3] = Monk;
		span[4] = Ranger;
		span[5] = Samurai;
		span[6] = Bard;
		span[7] = Geomancer;
		span[8] = TimeMage;
		span[9] = Cannoneer;
		span[10] = Chemist;
		span[11] = Oracle;
		span[12] = Thief;
		span[13] = MysticKnight;
		span[14] = Gladiator;
		span[15] = Dancer;
		span[16] = Ninja;
		span[17] = WhiteMage;
		span[18] = BlackMage;
		span[19] = Dragoon;
		span[20] = Summoner;
		span[21] = BlueMage;
		span[22] = RedMage;
		span[23] = Necromancer;
		AllJobs = list;
	}
}
