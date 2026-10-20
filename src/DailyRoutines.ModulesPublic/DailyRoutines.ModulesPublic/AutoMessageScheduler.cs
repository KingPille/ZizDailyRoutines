using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Info.Game;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.ModulesPublic;

public class AutoMessageScheduler : ModuleBase
{
	private class Config : ModuleConfig
	{
		public List<ScheduledMessage> Presets = new List<ScheduledMessage>();
	}

	private class ScheduledMessage
	{
		public Guid ID = Guid.NewGuid();

		public int IntervalSeconds = 300;

		public bool IsActive;

		public string MessageText = string.Empty;

		public TimeMode Mode;

		public string Name = string.Empty;

		public long NextTriggerTime;

		public int Remaining = 3;

		public int Repeat = 3;

		public int StartHour;

		public int StartMinute;
	}

	private class EditingState(ScheduledMessage scheduledMessage)
	{
		public ScheduledMessage Editing = scheduledMessage;

		public int Interval = scheduledMessage.IntervalSeconds;

		public string Message = scheduledMessage.MessageText;

		public TimeMode Mode = scheduledMessage.Mode;

		public string Name = scheduledMessage.Name;

		public int Repeat = scheduledMessage.Repeat;

		public int StartHour = scheduledMessage.StartHour;

		public int StartMinute = scheduledMessage.StartMinute;
	}

	private enum TimeMode
	{
		LocalTime,
		EorzeaTime,
		ServerTime
	}

	private Config? config;

	private EditingState? editingData;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoMessageSchedulerTitle"),
		Description = LanguageManager.Get("AutoMessageSchedulerDescription"),
		Category = ModuleCategory.System,
		Author = new string[1] { "Wotou" }
	};

	protected override void Init()
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		foreach (ScheduledMessage preset in config.Presets)
		{
			preset.IsActive = false;
			preset.Remaining = preset.Repeat;
		}
		if (base.TaskHelper == null)
		{
			TaskHelper taskHelper = (base.TaskHelper = new TaskHelper());
		}
		OmenServiceBase<FrameworkManager>.Instance().Reg(new OnUpdateDelegate(OnUpdate), 500u);
	}

	protected override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		OmenServiceBase<FrameworkManager>.Instance().Unreg(OnUpdate);
	}

	protected override void ConfigUI()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04db: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0512: Unknown result type (might be due to invalid IL or missing references)
		//IL_0517: Unknown result type (might be due to invalid IL or missing references)
		//IL_0536: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_055b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0585: Unknown result type (might be due to invalid IL or missing references)
		//IL_058a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_063f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0663: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0704: Unknown result type (might be due to invalid IL or missing references)
		//IL_070a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0745: Unknown result type (might be due to invalid IL or missing references)
		//IL_074a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0769: Unknown result type (might be due to invalid IL or missing references)
		//IL_078e: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0807: Unknown result type (might be due to invalid IL or missing references)
		//IL_0839: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ec: Unknown result type (might be due to invalid IL or missing references)
		TableDisposable val = ImRaii.Table(ImU8String.op_Implicit("MessageTable"), 8, (ImGuiTableFlags)1984);
		try
		{
			if (!val)
			{
				return;
			}
			ImGui.TableSetupColumn(ImU8String.op_Implicit("序号"), (ImGuiTableColumnFlags)8, ImGui.CalcTextSize(ImU8String.op_Implicit("1234"), false, -1f).X, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("名称"), (ImGuiTableColumnFlags)4, 25f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("开始时间"), (ImGuiTableColumnFlags)4, 20f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("间隔时间"), (ImGuiTableColumnFlags)4, 20f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("重复次数"), (ImGuiTableColumnFlags)4, 15f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("是否激活"), (ImGuiTableColumnFlags)4, 15f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("计时模式"), (ImGuiTableColumnFlags)4, 15f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("操作"), (ImGuiTableColumnFlags)4, 30f, 0u);
			ImGui.TableNextRow((ImGuiTableRowFlags)1);
			ImGui.TableNextColumn();
			if (ImGuiOm.ButtonIconSelectable("AddScheduleButton", (FontAwesomeIcon)61543))
			{
				editingData = new EditingState(new ScheduledMessage());
				ImGui.OpenPopup(ImU8String.op_Implicit("EditPresetPopup"), (ImGuiPopupFlags)0);
			}
			ImGui.TableNextColumn();
			ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("Name")));
			ImGui.TableNextColumn();
			ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("StartTime")));
			ImGui.TableNextColumn();
			ImU8String val2 = new ImU8String(4, 1);
			((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get("Interval"));
			((ImU8String)(ref val2)).AppendLiteral(" (s)");
			ImGui.TextUnformatted(val2);
			ImGui.TableNextColumn();
			ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("AutoMessageScheduler-RepeatTimes")));
			ImGui.TableNextColumn();
			ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("State")));
			ImGui.TableNextColumn();
			ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("AutoMessageScheduler-TimeMode")));
			ImGui.TableNextColumn();
			ImGui.TextUnformatted(ImU8String.op_Implicit(string.Empty));
			bool flag = false;
			for (int i = 0; i < config.Presets.Count; i++)
			{
				IdDisposable val3 = ImRaii.PushId(i, true);
				try
				{
					ScheduledMessage scheduledMessage = config.Presets[i];
					ImGui.TableNextRow();
					ImGui.TableNextColumn();
					ImGui.TextUnformatted(ImU8String.op_Implicit((i + 1).ToString()));
					ImGui.TableNextColumn();
					ImGui.TextUnformatted(ImU8String.op_Implicit(scheduledMessage.Name));
					ImGui.TableNextColumn();
					ImU8String val4 = new ImU8String(1, 2);
					((ImU8String)(ref val4)).AppendFormatted<int>(scheduledMessage.StartHour, "D2");
					((ImU8String)(ref val4)).AppendLiteral(":");
					((ImU8String)(ref val4)).AppendFormatted<int>(scheduledMessage.StartMinute, "D2");
					ImGui.TextUnformatted(val4);
					ImGui.TableNextColumn();
					ImU8String val5 = new ImU8String(0, 1);
					((ImU8String)(ref val5)).AppendFormatted<int>(scheduledMessage.IntervalSeconds);
					ImGui.TextUnformatted(val5);
					ImGui.TableNextColumn();
					ImU8String val6 = new ImU8String(1, 2);
					((ImU8String)(ref val6)).AppendFormatted<int>(scheduledMessage.Remaining);
					((ImU8String)(ref val6)).AppendLiteral("/");
					((ImU8String)(ref val6)).AppendFormatted<int>(scheduledMessage.Repeat);
					ImGui.TextUnformatted(val6);
					ImGui.TableNextColumn();
					Vector4 vector = (scheduledMessage.IsActive ? KnownColor.GreenYellow.ToVector4() : KnownColor.Pink.ToVector4());
					ImGui.TextColored(ref vector, ImU8String.op_Implicit(scheduledMessage.IsActive ? "O" : "X"));
					ImGui.TableNextColumn();
					ImGui.TextUnformatted(ImU8String.op_Implicit(scheduledMessage.Mode switch
					{
						TimeMode.LocalTime => LuminaWrapper.GetAddonText(1127u), 
						TimeMode.ServerTime => LuminaWrapper.GetAddonText(1128u), 
						TimeMode.EorzeaTime => LuminaWrapper.GetAddonText(1129u), 
						_ => string.Empty, 
					}));
					ImGui.TableNextColumn();
					if (ImGuiOm.ButtonIcon("Toggle", (FontAwesomeIcon)(scheduledMessage.IsActive ? 61517 : 61515), scheduledMessage.IsActive ? LanguageManager.Get("Stop") : LanguageManager.Get("Start")))
					{
						scheduledMessage.IsActive = !scheduledMessage.IsActive;
						scheduledMessage.Remaining = scheduledMessage.Repeat;
						if (scheduledMessage.IsActive)
						{
							scheduledMessage.NextTriggerTime = CalculateStartTime(scheduledMessage);
						}
					}
					ImGui.SameLine();
					if (ImGuiOm.ButtonIcon("Edit", (FontAwesomeIcon)62212, LanguageManager.Get("Edit")))
					{
						editingData = new EditingState(scheduledMessage);
						flag = true;
					}
					ImGui.SameLine();
					if (ImGuiOm.ButtonIcon("Delete", (FontAwesomeIcon)61944, LanguageManager.Get("Delete") + " (Ctrl)") && ImGui.IsKeyDown((ImGuiKey)527))
					{
						config.Presets.RemoveAt(i);
						config.Save(this);
					}
				}
				finally
				{
					((IDisposable)val3)?.Dispose();
				}
			}
			if (flag)
			{
				ImGui.OpenPopup(ImU8String.op_Implicit("EditPresetPopup"), (ImGuiPopupFlags)0);
			}
			PopupDisposable val7 = ImRaii.Popup(ImU8String.op_Implicit("EditPresetPopup"));
			try
			{
				if (!val7 || editingData == null)
				{
					return;
				}
				ImU8String val8 = new ImU8String(1, 1);
				((ImU8String)(ref val8)).AppendFormatted<string>(LanguageManager.Get("Name"));
				((ImU8String)(ref val8)).AppendLiteral(":");
				ImGui.TextUnformatted(val8);
				IndentDisposable val9 = ImRaii.PushIndent(1, true);
				try
				{
					ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
					ImGui.InputText(ImU8String.op_Implicit("##Name"), ref editingData.Name, 64, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
				}
				finally
				{
					((IDisposable)val9)?.Dispose();
				}
				ImU8String val10 = new ImU8String(1, 1);
				((ImU8String)(ref val10)).AppendFormatted<string>(LanguageManager.Get("StartTime"));
				((ImU8String)(ref val10)).AppendLiteral(":");
				ImGui.TextUnformatted(val10);
				val9 = ImRaii.PushIndent(1, true);
				try
				{
					string s = editingData.StartHour.ToString("D2");
					string s2 = editingData.StartMinute.ToString("D2");
					ImGui.SetNextItemWidth(25f * Globals.GlobalUIScale);
					if (ImGui.InputText(ImU8String.op_Implicit("##StartHour"), ref s, 2, (ImGuiInputTextFlags)1, (ImGuiInputTextCallbackDelegate)null) && int.TryParse(s, out var result) && result >= 0 && result <= 23)
					{
						editingData.StartHour = result;
					}
					ImGui.SameLine();
					ImGui.AlignTextToFramePadding();
					ImGui.TextUnformatted(ImU8String.op_Implicit(":"));
					ImGui.SameLine();
					ImGui.SetNextItemWidth(25f * Globals.GlobalUIScale);
					if (ImGui.InputText(ImU8String.op_Implicit("##StartMinute"), ref s2, 2, (ImGuiInputTextFlags)1, (ImGuiInputTextCallbackDelegate)null) && int.TryParse(s2, out var result2) && result2 >= 0 && result2 <= 59)
					{
						editingData.StartMinute = result2;
					}
				}
				finally
				{
					((IDisposable)val9)?.Dispose();
				}
				ImU8String val11 = new ImU8String(5, 1);
				((ImU8String)(ref val11)).AppendFormatted<string>(LanguageManager.Get("Interval"));
				((ImU8String)(ref val11)).AppendLiteral(" (s):");
				ImGui.TextUnformatted(val11);
				val9 = ImRaii.PushIndent(1, true);
				ImU8String val13;
				try
				{
					ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
					ImU8String val12 = ImU8String.op_Implicit("##Interval");
					ref int interval = ref editingData.Interval;
					val13 = default(ImU8String);
					if (ImGui.InputInt(val12, ref interval, 0, 0, val13, (ImGuiInputTextFlags)0))
					{
						editingData.Interval = Math.Clamp(editingData.Interval, 1, 864000);
					}
				}
				finally
				{
					((IDisposable)val9)?.Dispose();
				}
				val13 = new ImU8String(1, 1);
				((ImU8String)(ref val13)).AppendFormatted<string>(LanguageManager.Get("AutoMessageScheduler-RepeatTimes"));
				((ImU8String)(ref val13)).AppendLiteral(":");
				ImGui.TextUnformatted(val13);
				val9 = ImRaii.PushIndent(1, true);
				ImU8String val15;
				try
				{
					ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
					ImU8String val14 = ImU8String.op_Implicit("##Repeat");
					ref int repeat = ref editingData.Repeat;
					val15 = default(ImU8String);
					if (ImGui.InputInt(val14, ref repeat, 0, 0, val15, (ImGuiInputTextFlags)0))
					{
						editingData.Repeat = Math.Clamp(editingData.Repeat, 1, 14400);
					}
				}
				finally
				{
					((IDisposable)val9)?.Dispose();
				}
				val15 = new ImU8String(1, 1);
				((ImU8String)(ref val15)).AppendFormatted<string>(LanguageManager.Get("AutoMessageScheduler-TimeMode"));
				((ImU8String)(ref val15)).AppendLiteral(":");
				ImGui.TextUnformatted(val15);
				val9 = ImRaii.PushIndent(1, true);
				try
				{
					int mode = (int)editingData.Mode;
					ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
					ImU8String val16 = ImU8String.op_Implicit("##TimeMode");
					InlineArray2<string> buffer = default(InlineArray2<string>);
					buffer[0] = LuminaWrapper.GetAddonText(1127u) ?? "";
					buffer[1] = LuminaWrapper.GetAddonText(1129u) + LuminaWrapper.GetAddonText(1128u);
					if (ImGui.Combo(val16, ref mode, (ReadOnlySpan<string>)buffer, -1))
					{
						editingData.Mode = (TimeMode)mode;
					}
				}
				finally
				{
					((IDisposable)val9)?.Dispose();
				}
				ImU8String val17 = new ImU8String(1, 1);
				((ImU8String)(ref val17)).AppendFormatted<string>(LuminaWrapper.GetAddonText(2581u));
				((ImU8String)(ref val17)).AppendLiteral(":");
				ImGui.TextUnformatted(val17);
				val9 = ImRaii.PushIndent(1, true);
				try
				{
					ImGui.InputTextMultiline(ImU8String.op_Implicit("##Messages"), ref editingData.Message, 1024, new Vector2(-1f, 100f * Globals.GlobalUIScale), (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
				}
				finally
				{
					((IDisposable)val9)?.Dispose();
				}
				if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61452, LanguageManager.Get("Confirm")) && editingData.Editing != null)
				{
					editingData.Editing.Name = editingData.Name;
					editingData.Editing.StartHour = Math.Clamp(editingData.StartHour, 0, 23);
					editingData.Editing.StartMinute = Math.Clamp(editingData.StartMinute, 0, 59);
					editingData.Editing.IntervalSeconds = Math.Max(1, editingData.Interval);
					editingData.Editing.Repeat = Math.Max(1, editingData.Repeat);
					editingData.Editing.Remaining = editingData.Editing.Repeat;
					editingData.Editing.MessageText = editingData.Message;
					editingData.Editing.IsActive = false;
					editingData.Editing.Mode = editingData.Mode;
					if (!config.Presets.Contains(editingData.Editing))
					{
						config.Presets.Add(editingData.Editing);
					}
					editingData = null;
					config.Save(this);
					ImGui.CloseCurrentPopup();
				}
			}
			finally
			{
				((PopupDisposable)(ref val7)).Dispose();
			}
		}
		finally
		{
			((TableDisposable)(ref val)).Dispose();
		}
		static long CalculateStartTime(ScheduledMessage sched)
		{
			long now = GetNow(sched.Mode);
			long num;
			switch (sched.Mode)
			{
			case TimeMode.LocalTime:
				num = new DateTimeOffset(OmenServiceBase<StandardTimeManager>.Instance().Today).ToUnixTimeSeconds();
				break;
			case TimeMode.EorzeaTime:
			case TimeMode.ServerTime:
				num = now - now % 86400;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			long num2 = num + sched.StartHour * 3600 + sched.StartMinute * 60;
			if (num2 <= now)
			{
				num2 += 86400;
			}
			return num2;
		}
	}

	private void OnUpdate(IFramework _)
	{
		foreach (ScheduledMessage preset in config.Presets)
		{
			if (!preset.IsActive || preset.Remaining <= 0)
			{
				continue;
			}
			long now = GetNow(preset.Mode);
			if (now >= preset.NextTriggerTime)
			{
				preset.Remaining--;
				preset.NextTriggerTime = now + preset.IntervalSeconds;
				EneuqueMessagesSending(preset);
				if (preset.Remaining <= 0)
				{
					preset.IsActive = false;
				}
			}
		}
	}

	private void EneuqueMessagesSending(ScheduledMessage sched)
	{
		string[] array = sched.MessageText.Split('\n', StringSplitOptions.RemoveEmptyEntries);
		foreach (string line in array)
		{
			base.TaskHelper.Enqueue(delegate
			{
				OmenServiceBase<ChatManager>.Instance().SendMessage(line);
			});
			base.TaskHelper.DelayNext(20);
		}
	}

	private static long GetNow(TimeMode mode)
	{
		return mode switch
		{
			TimeMode.LocalTime => new DateTimeOffset(OmenServiceBase<StandardTimeManager>.Instance().Now).ToUnixTimeSeconds(), 
			TimeMode.ServerTime => Framework.GetServerTime(), 
			TimeMode.EorzeaTime => EorzeaDate.GetTime().EorzeaTimeStamp, 
			_ => throw new ArgumentOutOfRangeException("mode", mode, null), 
		};
	}
}
