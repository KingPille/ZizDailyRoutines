using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Dalamud;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Interop.Game.Helpers;

public static class ContentsFinderHelper
{
	private delegate bool RequestContentsFinderDelegate(uint[] contentsID, uint contentsCount, uint a3, ref ContentsFinderOption option);

	private delegate void RequestContentsFinderRouletteDelegate(ushort contentRouletteID, ref ContentsFinderOption option);

	private delegate void CancelContentsFinderDelegate(byte a1);

	private static readonly CompSig RequestContentsFinderSig;

	private static readonly RequestContentsFinderDelegate RequestContentsFinder;

	private static readonly CompSig RequestContentsFinderRouletteSig;

	private static readonly RequestContentsFinderRouletteDelegate RequestContentsFinderRoulette;

	private static readonly CompSig CancelContentsFinderSig;

	private static readonly CancelContentsFinderDelegate CancelContentsFinder;

	public static ContentsFinderOption DefaultOption => new ContentsFinderOption
	{
		Config817to820 = true
	};

	static ContentsFinderHelper()
	{
		RequestContentsFinderSig = new CompSig("E8 ?? ?? ?? ?? 33 C0 E9 ?? ?? ?? ?? FE C8");
		RequestContentsFinderRouletteSig = new CompSig("48 89 5C 24 ?? 57 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 84 24 ?? ?? ?? ?? 0F B6 F9 48 8B DA");
		CancelContentsFinderSig = new CompSig("E8 ?? ?? ?? ?? C6 43 ?? ?? E8 ?? ?? ?? ?? 83 C0 ?? 89 43 ?? 48 83 C4 ?? 5B C3 C6 43");
		RequestContentsFinder = RequestContentsFinderSig.GetDelegate<RequestContentsFinderDelegate>();
		RequestContentsFinderRoulette = RequestContentsFinderRouletteSig.GetDelegate<RequestContentsFinderRouletteDelegate>();
		CancelContentsFinder = CancelContentsFinderSig.GetDelegate<CancelContentsFinderDelegate>();
	}

	public static bool RequestDutyNormal(uint rowID, ContentsFinderOption option)
	{
		ContentsFinderOption option2 = option.Clone();
		DLog.Debug($"[ContentsFinderHelper] 一般副本 ({LuminaWrapper.GetContentName(rowID)})\n{option2}");
		return RequestContentsFinder(new uint[1] { rowID }, 1u, 0u, ref option2);
	}

	public static bool RequestDutyNormal(uint[] rowIDs, ContentsFinderOption option)
	{
		ContentsFinderOption option2 = option.Clone();
		DLog.Debug($"[ContentsFinderHelper] 一般副本 ({string.Join(", ", rowIDs.Select(LuminaWrapper.GetContentName))})\n{option2}");
		return RequestContentsFinder(rowIDs, (uint)rowIDs.Length, 0u, ref option2);
	}

	public static void RequestDutyRoulette(ushort rowID, ContentsFinderOption option)
	{
		ContentsFinderOption option2 = option.Clone();
		DLog.Debug($"[ContentsFinderHelper] 一般副本 ({LuminaWrapper.GetContentRouletteName(rowID)})\n{option2}");
		RequestContentsFinderRoulette(rowID, ref option2);
	}

	public static void CancelDutyApply()
	{
		CancelContentsFinder(0);
	}

	public static void RequestDutySupport(uint dawnContentID)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestDutySupport);
		IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
		if (localPlayer == null)
		{
			return;
		}
		ClassJob value = localPlayer.ClassJob.Value;
		byte role = ((ClassJob)(ref value)).Role;
		if (!LuminaGetter.TryGetRow<DawnContent>(dawnContentID, out DawnContent item))
		{
			return;
		}
		ContentFinderCondition value2 = ((DawnContent)(ref item)).Content.Value;
		if (!LuminaGetter.TryGetRow<ContentMemberType>(((ContentFinderCondition)(ref value2)).ContentMemberType.RowId, out ContentMemberType item2))
		{
			return;
		}
		var partyComposition = new
		{
			Tanks = ((ContentMemberType)(ref item2)).TanksPerParty,
			Healers = ((ContentMemberType)(ref item2)).HealersPerParty,
			DPS = ((ContentMemberType)(ref item2)).MeleesPerParty + ((ContentMemberType)(ref item2)).RangedPerParty,
			Total = ((ContentMemberType)(ref item2)).TanksPerParty + ((ContentMemberType)(ref item2)).HealersPerParty + ((ContentMemberType)(ref item2)).MeleesPerParty + ((ContentMemberType)(ref item2)).RangedPerParty
		};
		int tanks = ((role == 1) ? 1 : 0);
		int healers = ((role == 4) ? 1 : 0);
		bool flag = (uint)(role - 2) <= 1u;
		var anon = new
		{
			Tanks = tanks,
			Healers = healers,
			DPS = (flag ? 1 : 0)
		};
		var list = (from x in (from x in (from x in ((IEnumerable<SubrowCollection<DawnContentParticipable>>)LuminaGetter.GetSub<DawnContentParticipable>()).SelectMany((SubrowCollection<DawnContentParticipable> x) => (IEnumerable<DawnContentParticipable>)(object)x)
					where ((DawnContentParticipable)(ref x)).RowId == dawnContentID
					select x).Select(delegate(DawnContentParticipable x)
				{
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0011: Unknown result type (might be due to invalid IL or missing references)
					//IL_0016: Unknown result type (might be due to invalid IL or missing references)
					//IL_0019: Unknown result type (might be due to invalid IL or missing references)
					//IL_001e: Unknown result type (might be due to invalid IL or missing references)
					DawnQuestMember? valueNullable = ((DawnContentParticipable)(ref x)).DawnQuestMember.ValueNullable;
					DawnQuestMember value3 = ((DawnContentParticipable)(ref x)).DawnQuestMember.Value;
					return new
					{
						QuestMember = valueNullable,
						JobInfo = LuminaGetter.GetRow<DawnMember>((uint)((DawnQuestMember)(ref value3)).Unknown0)
					};
				})
				where x.QuestMember.HasValue && x.JobInfo.HasValue
				select new
				{
					QuestMember = x.QuestMember.GetValueOrDefault(),
					JobInfo = x.JobInfo.GetValueOrDefault()
				}).Select(x =>
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0009: Unknown result type (might be due to invalid IL or missing references)
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0024: Unknown result type (might be due to invalid IL or missing references)
				//IL_002d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				DawnQuestMember questMember = x.QuestMember;
				DawnMemberUIParam value3 = ((DawnQuestMember)(ref questMember)).Class.Value;
				ReadOnlySeString name = ((DawnMemberUIParam)(ref value3)).Name;
				questMember = x.QuestMember;
				uint rowId = ((DawnQuestMember)(ref questMember)).RowId;
				DawnMember jobInfo = x.JobInfo;
				return new
				{
					Name = name,
					ID = rowId,
					ClassJob = LuminaGetter.GetRow<ClassJob>((uint)((DawnMember)(ref jobInfo)).Unknown0)
				};
			})
			group x by x.Name into x
			select new
			{
				Name = x.Key,
				Data = x.Select(d => (ID: d.ID, ClassJob: d.ClassJob)).ToList()
			} into x
			orderby x.Data.Count
			select x).ToList();
		List<uint> list2 = new List<uint>();
		var roleCount = anon;
		List<string> list3 = new List<string>();
		foreach (var item3 in list)
		{
			if (list2.Count >= partyComposition.Total - 1)
			{
				break;
			}
			(uint, ClassJob?) tuple = item3.Data.FirstOrDefault<(uint, ClassJob?)>(delegate((uint ID, ClassJob? ClassJob) job)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				ClassJob valueOrDefault = job.ClassJob.GetValueOrDefault();
				switch (((ClassJob)(ref valueOrDefault)).Role)
				{
				case 1:
					if (roleCount.Tanks < partyComposition.Tanks)
					{
						var anon3 = roleCount;
						roleCount = new
						{
							Tanks = roleCount.Tanks + 1,
							Healers = anon3.Healers,
							DPS = anon3.DPS
						};
						return true;
					}
					break;
				case 4:
					if (roleCount.Healers < partyComposition.Healers)
					{
						var anon3 = roleCount;
						int dPS = roleCount.Healers + 1;
						roleCount = new
						{
							Tanks = anon3.Tanks,
							Healers = dPS,
							DPS = anon3.DPS
						};
						return true;
					}
					break;
				case 2:
				case 3:
					if (roleCount.DPS < partyComposition.DPS)
					{
						var anon3 = roleCount;
						int dPS = roleCount.DPS + 1;
						roleCount = new
						{
							Tanks = anon3.Tanks,
							Healers = anon3.Healers,
							DPS = dPS
						};
						return true;
					}
					break;
				}
				return false;
			});
			if (tuple.Item2.HasValue)
			{
				list2.Add(tuple.Item1);
				string empty = string.Empty;
				try
				{
					value = tuple.Item2.Value;
					empty = ((object)((ClassJob)(ref value)).Name/*cast due to constrained. prefix*/).ToString() ?? string.Empty;
				}
				catch
				{
					empty = string.Empty;
				}
				list3.Add($"{item3.Name} ({(string.IsNullOrWhiteSpace(empty) ? "未知职业" : empty)})");
			}
		}
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(35, 2);
		defaultInterpolatedStringHandler.AppendLiteral("[ContentsFinderHelper] 剧情辅助器（");
		value2 = ((DawnContent)(ref item)).Content.Value;
		defaultInterpolatedStringHandler.AppendFormatted(((object)((ContentFinderCondition)(ref value2)).Name/*cast due to constrained. prefix*/).ToString());
		defaultInterpolatedStringHandler.AppendLiteral("）\n");
		defaultInterpolatedStringHandler.AppendLiteral("成员: ");
		defaultInterpolatedStringHandler.AppendFormatted(string.Join(", ", list3));
		DLog.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
		var anon2 = list2.Select((uint id, int index) => new
		{
			Value = (long)((double)id * Math.Pow(256.0, index % 4)),
			IsParam2 = (index < 4)
		}).Aggregate(new
		{
			Param2 = 0L,
			Param3 = 0L
		}, (acc, curr) =>
		{
			if (!curr.IsParam2)
			{
				long param = acc.Param3 + curr.Value;
				return new
				{
					Param2 = acc.Param2,
					Param3 = param
				};
			}
			return new
			{
				Param2 = acc.Param2 + curr.Value,
				Param3 = acc.Param3
			};
		});
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.SendDutySupport, dawnContentID, (uint)anon2.Param2, (uint)anon2.Param3);
	}
}
