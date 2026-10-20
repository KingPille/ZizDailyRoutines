using System;
using System.Collections.Generic;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Text.ReadOnly;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class AutoNotifyLeveUpdate : ModuleBase
{
	private class Config : ModuleConfig
	{
		public int NotificationThreshold = 97;
	}

	private Config config;

	private DateTime finishTime = OmenServiceBase<StandardTimeManager>.Instance().UTCNow;

	private int lastLeve;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoNotifyLeveUpdateTitle"),
		Description = LanguageManager.Get("AutoNotifyLeveUpdateDescription"),
		Category = ModuleCategory.Notification,
		Author = new string[1] { "HSS" }
	};

	protected override void Init()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		OmenServiceBase<FrameworkManager>.Instance().Reg(new OnUpdateDelegate(OnUpdate), 60000u);
	}

	protected override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		OmenServiceBase<FrameworkManager>.Instance().Unreg(OnUpdate);
	}

	protected override void ConfigUI()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (ImGui.SliderInt(ImU8String.op_Implicit(LanguageManager.Get("AutoNotifyLeveUpdate-NotificationThreshold")), ref config.NotificationThreshold, 1, 100, default(ImU8String), (ImGuiSliderFlags)0))
		{
			lastLeve = 0;
			config.Save(this);
		}
	}

	private unsafe void OnUpdate(IFramework _)
	{
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		if (!GameState.IsLoggedIn || GameState.ContentFinderCondition != 0)
		{
			return;
		}
		DateTime uTCNow = OmenServiceBase<StandardTimeManager>.Instance().UTCNow;
		byte numLeveAllowances = ((QuestManager)QuestManager.Instance()).NumLeveAllowances;
		if (lastLeve != numLeveAllowances)
		{
			bool flag = numLeveAllowances > lastLeve;
			lastLeve = numLeveAllowances;
			finishTime = GetFinishTime(numLeveAllowances, uTCNow);
			if (numLeveAllowances >= config.NotificationThreshold && flag)
			{
				OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoNotifyLeveUpdate-Notification", new Dictionary<string, object>
				{
					["count"] = numLeveAllowances,
					["date"] = finishTime.ToLocalTime()
				})));
			}
		}
	}

	private static DateTime GetFinishTime(int num, DateTime nowUTC)
	{
		if (num >= 100)
		{
			return nowUTC;
		}
		int num2 = (100 - num + 2) / 3;
		DateTime dateTime = new DateTime(nowUTC.Year, nowUTC.Month, nowUTC.Day, (nowUTC.Hour >= 12) ? 12 : 0, 0, 0, DateTimeKind.Utc);
		return dateTime.AddHours(12 * num2);
	}
}
