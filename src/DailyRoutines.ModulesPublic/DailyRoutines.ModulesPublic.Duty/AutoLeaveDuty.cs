using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Extensions;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Command;
using Dalamud.Game.DutyState;
using Dalamud.Game.Text.Evaluator;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.ImGuiOm.Widgets.Combos;
using OmenTools.Interop.Game.ExecuteCommand.Implementations;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.ModulesPublic.Duty;

public class AutoLeaveDuty : ModuleBase
{
	private class Config : ModuleConfig
	{
		public HashSet<uint> BlacklistContent = new HashSet<uint>();

		public HashSet<uint> ImmediateLeaveContent = new HashSet<uint>();

		public int Delay;

		public bool ForceToLeave;

		public bool NoLeaveHighEndDuties = true;
	}

	private enum LeaveDutyKind
	{
		None,
		InstantLeave,
		NoLeave
	}

	private Config config;

	private ContentSelectCombo contentSelectCombo;

	private ContentSelectCombo immediateLeaveCombo;

	private LeaveDutyKind manualLeaveKind;

	private const string COMMAND = "autoleaveduty";

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoLeaveDutyTitle"),
		Description = LanguageManager.Get("AutoLeaveDutyDescription"),
		Category = ModuleCategory.Duty
	};

	protected override void Init()
	{
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Expected O, but got Unknown
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Expected O, but got Unknown
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Expected O, but got Unknown
		contentSelectCombo = new ContentSelectCombo("Blacklist");
		immediateLeaveCombo = new ContentSelectCombo("ImmediateLeave");
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		if (base.TaskHelper == null)
		{
			TaskHelper taskHelper = (base.TaskHelper = new TaskHelper());
		}
		contentSelectCombo.SelectedIDs = config.BlacklistContent;
		immediateLeaveCombo.SelectedIDs = config.ImmediateLeaveContent;
		OmenServiceBase<LogMessageManager>.Instance().RegPre(OnPreReceiveLogmessage);
		DalamudServiceExtension.Instance<IDutyState>().DutyCompleted += new DutyCompletedDelegate(OnDutyComplete);
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += OnZoneChanged;
		OmenServiceBase<CommandManager>.Instance().AddSubCommand("autoleaveduty", new CommandInfo(new HandlerDelegate(OnCommand))
		{
			HelpMessage = LanguageManager.Get("AutoLeaveDuty-Command-Set")
		});
	}

	protected override void Uninit()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		OmenServiceBase<CommandManager>.Instance().RemoveSubCommand("autoleaveduty");
		DalamudServiceExtension.Instance<IDutyState>().DutyCompleted -= new DutyCompletedDelegate(OnDutyComplete);
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChanged;
		OmenServiceBase<LogMessageManager>.Instance().Unreg(OnPreReceiveLogmessage);
	}

	protected override void ConfigUI()
	{
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		ItemWidthDisposable val = ImRaii.ItemWidth(250f * Globals.GlobalUIScale);
		try
		{
			using (ImRaiiExtension.Heading1(LanguageManager.Get("Command")))
			{
				LeaveDutyKind[] values = Enum.GetValues<LeaveDutyKind>();
				foreach (LeaveDutyKind value in values)
				{
					string text = $"/pdr {"autoleaveduty"} {value}";
					ImGui.TextUnformatted(ImU8String.op_Implicit(text));
					if (ImGui.IsItemHovered())
					{
						ImGui.SetMouseCursor((ImGuiMouseCursor)7);
					}
					if (ImGui.IsItemClicked())
					{
						ImGui.SetClipboardText(ImU8String.op_Implicit(text));
						OmenServiceBase<NotifyHelper>.Instance().NotificationSuccess(text, LanguageManager.Get("CopiedToClipboard"));
					}
					ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get($"AutoLeaveDuty-Command-Set-{value}")));
					ImGui.Spacing();
				}
			}
			ImGui.NewLine();
			ImU8String val2 = new ImU8String(18, 1);
			((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get("Delay"));
			((ImU8String)(ref val2)).AppendLiteral(" (ms)###DelayInput");
			ImU8String val3 = val2;
			ref int delay = ref config.Delay;
			ImU8String val4 = default(ImU8String);
			if (ImGui.InputInt(val3, ref delay, 0, 0, val4, (ImGuiInputTextFlags)0))
			{
				config.Delay = Math.Max(0, config.Delay);
			}
			if (ImGui.IsItemDeactivatedAfterEdit())
			{
				config.Save(this);
			}
			val4 = new ImU8String(23, 1);
			((ImU8String)(ref val4)).AppendFormatted<string>(LanguageManager.Get("AutoLeaveDuty-NoLeaveHighEndDuties"));
			((ImU8String)(ref val4)).AppendLiteral("###NoLeaveHighEndDuties");
			if (ImGui.Checkbox(val4, ref config.NoLeaveHighEndDuties))
			{
				config.Save(this);
			}
			ImGuiOm.HelpMarker(LanguageManager.Get("AutoLeaveDuty-NoLeaveHighEndDuties-Help"), 40f, (FontAwesomeIcon)61530);
			ImU8String val5 = new ImU8String(15, 1);
			((ImU8String)(ref val5)).AppendFormatted<string>(LanguageManager.Get("AutoLeaveDuty-ForceToLeave"));
			((ImU8String)(ref val5)).AppendLiteral("###ForceToLeave");
			if (ImGui.Checkbox(val5, ref config.ForceToLeave))
			{
				config.Save(this);
			}
			ImGui.NewLine();
			if (contentSelectCombo.DrawCheckbox())
			{
				config.BlacklistContent = contentSelectCombo.SelectedIDs;
				config.Save(this);
			}
			ImGui.SameLine();
			ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("AutoLeaveDuty-BlacklistContents")));
			ImGuiOm.HelpMarker(LanguageManager.Get("AutoLeaveDuty-BlacklistContents-Help"), 40f, (FontAwesomeIcon)61530);
			if (immediateLeaveCombo.DrawCheckbox())
			{
				config.ImmediateLeaveContent = immediateLeaveCombo.SelectedIDs;
				config.Save(this);
			}
			ImGui.SameLine();
			ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("AutoLeaveDuty-ImmediateLeaveContents")));
			ImGuiOm.HelpMarker(LanguageManager.Get("AutoLeaveDuty-ImmediateLeaveContents-Help"), 40f, (FontAwesomeIcon)61530);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private unsafe void OnCommand(string command, string args)
	{
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		if (GameState.ContentFinderCondition == 0)
		{
			Utf8String val = default(Utf8String);
			((Utf8String)(ref val))._002Ector("/pdr " + command);
			try
			{
				((RaptureLogModule)RaptureLogModule.Instance()).ShowLogMessageString(726u, &val);
				return;
			}
			finally
			{
				((IDisposable)(*(Utf8String*)(&val))/*cast due to constrained. prefix*/).Dispose();
			}
		}
		if (!Enum.TryParse<LeaveDutyKind>(args, ignoreCase: true, out var result))
		{
			NotifyHelper notifyHelper = OmenServiceBase<NotifyHelper>.Instance();
			ISeStringEvaluator obj = DalamudServiceExtension.Instance<ISeStringEvaluator>();
			InlineArray3<SeStringParameter> buffer = default(InlineArray3<SeStringParameter>);
			buffer[0] = SeStringParameter.op_Implicit(1);
			buffer[1] = SeStringParameter.op_Implicit(LuminaWrapper.GetAddonText(9448u));
			buffer[2] = SeStringParameter.op_Implicit(args);
			notifyHelper.ChatError(obj.EvaluateFromLogMessage(3802u, (Span<SeStringParameter>)buffer, (ClientLanguage?)null));
		}
		else
		{
			manualLeaveKind = result;
			string text = LanguageManager.Get($"AutoLeaveDuty-Notification-Set-{result}");
			OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(text));
			NotifyHelper.Toast(text);
		}
	}

	private void OnDutyComplete(IDutyStateEventArgs args)
	{
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		if (manualLeaveKind != LeaveDutyKind.None)
		{
			switch (manualLeaveKind)
			{
			case LeaveDutyKind.InstantLeave:
				base.TaskHelper.Enqueue(delegate
				{
					DutyCommand.Leave(DutyCommand.LeaveDutyKind.Inactive);
				});
				break;
			case LeaveDutyKind.NoLeave:
				return;
			}
		}
		if (config.BlacklistContent.Contains(GameState.ContentFinderCondition))
		{
			return;
		}
		if (config.ImmediateLeaveContent.Contains(GameState.ContentFinderCondition))
		{
			base.TaskHelper.Enqueue(delegate
			{
				DutyCommand.Leave(DutyCommand.LeaveDutyKind.Inactive);
			});
			return;
		}
		if (config.NoLeaveHighEndDuties)
		{
			ContentFinderCondition value = args.ContentFinderCondition.Value;
			if (((ContentFinderCondition)(ref value)).HighEndDuty)
			{
				return;
			}
		}
		if (config.Delay > 0)
		{
			base.TaskHelper.DelayNext(config.Delay);
		}
		if (!config.ForceToLeave)
		{
			base.TaskHelper.Enqueue(() => !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)26]);
			base.TaskHelper.Enqueue(delegate
			{
				DutyCommand.Leave();
			});
		}
		else
		{
			base.TaskHelper.Enqueue(delegate
			{
				DutyCommand.Leave(DutyCommand.LeaveDutyKind.Inactive);
			});
		}
	}

	private void OnZoneChanged(uint u)
	{
		manualLeaveKind = LeaveDutyKind.None;
		base.TaskHelper.Abort();
	}

	private static void OnPreReceiveLogmessage(ref bool isPrevented, ref uint logMessageID, ref LogMessageQueueItem values)
	{
		if (logMessageID == 914)
		{
			isPrevented = true;
		}
	}
}
