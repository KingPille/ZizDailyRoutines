using System;
using System.Collections.Generic;
using System.Linq;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Plugin.Services;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Info.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic.Duty;

public class AutoNotifyDutyName : ModuleBase
{
	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoNotifyDutyNameTitle"),
		Description = LanguageManager.Get("AutoNotifyDutyNameDescription"),
		Category = ModuleCategory.Duty
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected override void Init()
	{
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += OnZoneChange;
	}

	protected override void Uninit()
	{
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChange;
	}

	private static void OnZoneChange(uint u)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		if (GameState.ContentFinderCondition == 0)
		{
			return;
		}
		ContentFinderCondition contentFinderConditionData = GameState.ContentFinderConditionData;
		byte maxLevel = Math.Max(((ContentFinderCondition)(ref contentFinderConditionData)).ClassJobLevelRequired, ((ContentFinderCondition)(ref contentFinderConditionData)).ClassJobLevelSync);
		OmenServiceBase<NotifyHelper>.Instance().TrayInfo(LanguageManager.Get("AutoNotifyDutyName-Notification-Message", new Dictionary<string, object>
		{
			["level"] = ((ContentFinderCondition)(ref contentFinderConditionData)).ClassJobLevelRequired,
			["content"] = ((ContentFinderCondition)(ref contentFinderConditionData)).Name
		}), LanguageManager.Get("AutoNotifyDutyName-Notification-Title"));
		ushort itemLevelRequired = ((ContentFinderCondition)(ref contentFinderConditionData)).ItemLevelRequired;
		int num;
		if (((ContentFinderCondition)(ref contentFinderConditionData)).ItemLevelRequired != 0 || ((ContentFinderCondition)(ref contentFinderConditionData)).ClassJobLevelSync != 0)
		{
			Item val = (from x in Sheets.Gears.Values
				where ((Item)(ref x)).LevelEquip != 1 && ((Item)(ref x)).LevelEquip <= maxLevel
				orderby ((Item)(ref x)).LevelItem.RowId descending
				select x).FirstOrDefault();
			num = (int)((Item)(ref val)).LevelItem.RowId;
		}
		else
		{
			num = 0;
		}
		uint num2 = (uint)num;
		if (((ContentFinderCondition)(ref contentFinderConditionData)).ClassJobLevelRequired == 0)
		{
			OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoNotifyDutyName-Message-NoLevel", new Dictionary<string, object> { ["content"] = ((ContentFinderCondition)(ref contentFinderConditionData)).Name })), useDefaultPrefix: false);
		}
		else if (itemLevelRequired == 0 && num2 == 0)
		{
			OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoNotifyDutyName-Message-NoItemLevel", new Dictionary<string, object>
			{
				["level"] = ((ContentFinderCondition)(ref contentFinderConditionData)).ClassJobLevelRequired,
				["content"] = ((ContentFinderCondition)(ref contentFinderConditionData)).Name
			})), useDefaultPrefix: false);
		}
		else
		{
			OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoNotifyDutyName-Message", new Dictionary<string, object>
			{
				["level"] = ((ContentFinderCondition)(ref contentFinderConditionData)).ClassJobLevelRequired,
				["content"] = ((ContentFinderCondition)(ref contentFinderConditionData)).Name,
				["minIL"] = itemLevelRequired,
				["maxIL"] = num2
			})), useDefaultPrefix: false);
		}
	}
}
