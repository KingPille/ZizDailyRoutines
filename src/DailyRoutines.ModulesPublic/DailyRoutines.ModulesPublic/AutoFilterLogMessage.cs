using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using OmenTools.Extensions;
using OmenTools.ImGuiOm.Widgets.Combos;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class AutoFilterLogMessage : ModuleBase
{
	private class Config : ModuleConfig
	{
		public HashSet<uint> FilteredLogMessages = new HashSet<uint>();

		public FilterMode Mode = FilterMode.PassFirst;
	}

	private enum FilterMode
	{
		Always,
		PassFirst
	}

	private Config config;

	private LogMessageCombo combo;

	private readonly HashSet<uint> seenLogMessages = new HashSet<uint>();

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoFilterLogMessageTitle"),
		Description = LanguageManager.Get("AutoFilterLogMessageDescription"),
		Category = ModuleCategory.System
	};

	protected override void Init()
	{
		combo = new LogMessageCombo("LogMessage");
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		combo.SelectedIDs = config.FilteredLogMessages;
		OmenServiceBase<LogMessageManager>.Instance().RegPre(OnLogMessage);
	}

	protected override void ConfigUI()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("AutoFilterLogMessage-MessageToFilter")));
		IndentDisposable val = ImRaii.PushIndent(1, true);
		try
		{
			if (combo.DrawCheckbox())
			{
				config.FilteredLogMessages = combo.SelectedIDs;
				config.Save(this);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		ImGui.NewLine();
		vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("Mode")));
		val = ImRaii.PushIndent(1, true);
		try
		{
			FilterMode[] values = Enum.GetValues<FilterMode>();
			foreach (FilterMode filterMode in values)
			{
				if (ImGui.RadioButton(ImU8String.op_Implicit(LanguageManager.Get($"AutoFilterLogMessage-Mode-{filterMode}")), config.Mode == filterMode))
				{
					config.Mode = filterMode;
					config.Save(this);
				}
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void OnLogMessage(ref bool isPrevented, ref uint logMessageID, ref LogMessageQueueItem item)
	{
		if (!config.FilteredLogMessages.Contains(logMessageID))
		{
			return;
		}
		switch (config.Mode)
		{
		case FilterMode.Always:
			isPrevented = true;
			break;
		case FilterMode.PassFirst:
			if (!seenLogMessages.Add(logMessageID))
			{
				isPrevented = true;
			}
			break;
		}
	}
}
