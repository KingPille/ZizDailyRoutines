using System;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game;
using Dalamud.Game.Text.Evaluator;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using Lumina.Text;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.ModulesPublic.Duty;

public class AutoNotifyMentorRouletteProgress : ModuleBase
{
	private const byte MENTOR_ROULETTE_ID = 9;

	private static readonly uint[] MentorRouletteAchievements = new uint[6] { 1472u, 1473u, 1474u, 1475u, 1603u, 1604u };

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoNotifyMentorRouletteProgressTitle"),
		Description = LanguageManager.Get("AutoNotifyMentorRouletteProgressDescription"),
		Category = ModuleCategory.Duty,
		PreviewImageURL = new string[1] { "hxxps://gh.atmoomen.top/raw.githubusercontent.com/AtmoOmen/StaticAssets/main/DailyRoutines/image/AutoNotifyMentorRouletteProgress-UI.png" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected override void Init()
	{
		if (base.TaskHelper == null)
		{
			TaskHelper taskHelper = (base.TaskHelper = new TaskHelper());
		}
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += OnZoneChanged;
		OnZoneChanged(0u);
	}

	protected override void Uninit()
	{
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChanged;
	}

	private unsafe void OnZoneChanged(uint u)
	{
		if (GameState.TerritoryType == 0)
		{
			return;
		}
		uint[] mentorRouletteAchievements = MentorRouletteAchievements;
		foreach (uint param in mentorRouletteAchievements)
		{
			OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestAchievement, param);
		}
		if (GameState.ContentFinderCondition == 0)
		{
			return;
		}
		ContentsFinder* ptr = ContentsFinder.Instance();
		if (ptr != null)
		{
			ContentsFinderQueueInfo* queueInfo = ((ContentsFinder)ptr).GetQueueInfo();
			if (queueInfo == null || ((ContentsFinderQueueInfo)queueInfo).QueuedContentRouletteId != 9)
			{
				return;
			}
		}
		base.TaskHelper.Abort();
		base.TaskHelper.Enqueue(delegate
		{
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			if (!UIModuleExtension.IsScreenReady())
			{
				return false;
			}
			AchievementInfo achievementInfo = null;
			uint[] mentorRouletteAchievements2 = MentorRouletteAchievements;
			foreach (uint id in mentorRouletteAchievements2)
			{
				if (!OmenServiceBase<AchievementManager>.Instance().TryGetAchievement(id, out AchievementInfo achievementInfo2))
				{
					return false;
				}
				if (!achievementInfo2.IsFinished)
				{
					achievementInfo = achievementInfo2;
					break;
				}
			}
			if (achievementInfo == null)
			{
				return true;
			}
			RentedSeStringBuilder val = default(RentedSeStringBuilder);
			((RentedSeStringBuilder)(ref val))._002Ector();
			try
			{
				SeStringBuilder builder = ((RentedSeStringBuilder)(ref val)).Builder;
				SeStringBuilder val2 = builder.Append(LanguageManager.Get("AutoNotifyMentorRouletteProgres-Notification-Title")).AppendNewLine();
				SeStringInterpolatedStringHandler val3 = new SeStringInterpolatedStringHandler(2, 3, val2);
				((SeStringInterpolatedStringHandler)(ref val3)).AppendFormatted<string>(LanguageManager.Get("AutoNotifyMentorRouletteProgres-Notification-CurrentProgress"));
				((SeStringInterpolatedStringHandler)(ref val3)).AppendLiteral("：");
				((SeStringInterpolatedStringHandler)(ref val3)).AppendFormatted(achievementInfo.Current);
				((SeStringInterpolatedStringHandler)(ref val3)).AppendLiteral("/");
				((SeStringInterpolatedStringHandler)(ref val3)).AppendFormatted(achievementInfo.Max);
				SeStringBuilder val4 = val2.Append(val3).AppendNewLine();
				SeStringBuilder obj = val4;
				val3 = new SeStringInterpolatedStringHandler(1, 1, val4);
				((SeStringInterpolatedStringHandler)(ref val3)).AppendFormatted<string>(LanguageManager.Get("AutoNotifyMentorRouletteProgres-Notification-TargetAchievement"));
				((SeStringInterpolatedStringHandler)(ref val3)).AppendLiteral("：");
				obj.Append(val3).Append(SeStringExtension.CreateAchievementLink(achievementInfo.ID));
				Achievement data = achievementInfo.GetData();
				RowRef<Title> title = ((Achievement)(ref data)).Title;
				if (title.RowId != 0)
				{
					val4 = builder.AppendNewLine();
					SeStringBuilder obj2 = val4;
					val3 = new SeStringInterpolatedStringHandler(3, 3, val4);
					((SeStringInterpolatedStringHandler)(ref val3)).AppendFormatted<string>(LanguageManager.Get("AutoNotifyMentorRouletteProgres-Notification-AchievementReward"));
					((SeStringInterpolatedStringHandler)(ref val3)).AppendLiteral("：");
					Title value;
					ReadOnlySeString val5;
					if (LocalPlayerState.Sex != 0)
					{
						value = title.Value;
						val5 = ((Title)(ref value)).Feminine;
					}
					else
					{
						value = title.Value;
						val5 = ((Title)(ref value)).Masculine;
					}
					((SeStringInterpolatedStringHandler)(ref val3)).AppendFormatted(val5);
					((SeStringInterpolatedStringHandler)(ref val3)).AppendLiteral("（");
					((SeStringInterpolatedStringHandler)(ref val3)).AppendFormatted<string>(LuminaWrapper.GetAddonText(14119u));
					((SeStringInterpolatedStringHandler)(ref val3)).AppendLiteral("）");
					obj2.Append(val3);
				}
				else
				{
					data = achievementInfo.GetData();
					RowRef<Item> item = ((Achievement)(ref data)).Item;
					if (item.RowId != 0)
					{
						val4 = builder.AppendNewLine();
						SeStringBuilder obj3 = val4;
						val3 = new SeStringInterpolatedStringHandler(1, 1, val4);
						((SeStringInterpolatedStringHandler)(ref val3)).AppendFormatted<string>(LanguageManager.Get("AutoNotifyMentorRouletteProgres-Notification-AchievementReward"));
						((SeStringInterpolatedStringHandler)(ref val3)).AppendLiteral("：");
						SeStringBuilder obj4 = obj3.Append(val3);
						Item value2 = item.Value;
						obj4.Append(SeStringExtension.CreateItemLink(((Item)(ref value2)).RowId, false, null));
					}
				}
				val4 = builder.AppendNewLine();
				SeStringBuilder obj5 = val4;
				val3 = new SeStringInterpolatedStringHandler(1, 1, val4);
				((SeStringInterpolatedStringHandler)(ref val3)).AppendFormatted<string>(LanguageManager.Get("AutoNotifyMentorRouletteProgres-Notification-CurrentDuty"));
				((SeStringInterpolatedStringHandler)(ref val3)).AppendLiteral("：");
				SeStringBuilder obj6 = obj5.Append(val3);
				ISeStringEvaluator obj7 = DalamudServiceExtension.Instance<ISeStringEvaluator>();
				InlineArray2<SeStringParameter> buffer = default(InlineArray2<SeStringParameter>);
				ref SeStringParameter reference = ref buffer[0];
				ContentFinderCondition contentFinderConditionData = GameState.ContentFinderConditionData;
				reference = SeStringParameter.op_Implicit((uint)((ContentFinderCondition)(ref contentFinderConditionData)).ClassJobLevelRequired);
				ref SeStringParameter reference2 = ref buffer[1];
				contentFinderConditionData = GameState.ContentFinderConditionData;
				reference2 = SeStringParameter.op_Implicit(((ContentFinderCondition)(ref contentFinderConditionData)).Name);
				obj6.Append(obj7.EvaluateFromAddon(12599u, (Span<SeStringParameter>)buffer, (ClientLanguage?)null));
				OmenServiceBase<NotifyHelper>.Instance().Chat(builder.ToReadOnlySeString(), useDefaultPrefix: false);
				return true;
			}
			finally
			{
				((IDisposable)(*(RentedSeStringBuilder*)(&val))/*cast due to constrained. prefix*/).Dispose();
			}
		});
	}
}
