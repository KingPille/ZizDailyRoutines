using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.LocalLifecycle;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using OmenTools.Dalamud;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class AutoTimedLogout : ModuleBase
{
	private enum OperationMode
	{
		Logout,
		ShutdownGame,
		ShutdownPC
	}

	private int customMinutes = 30;

	private long? scheduledTime;

	private OperationMode currentOperation;

	private CancellationTokenSource? cancelSource;

	private static readonly FrozenDictionary<OperationMode, string> ModeLoc = new Dictionary<OperationMode, string>
	{
		[OperationMode.Logout] = LanguageManager.Get("AutoTimedLogout-Mode-Logout"),
		[OperationMode.ShutdownGame] = LanguageManager.Get("AutoTimedLogout-Mode-ShutdownGame"),
		[OperationMode.ShutdownPC] = LanguageManager.Get("AutoTimedLogout-Mode-ShutdownPC")
	}.ToFrozenDictionary();

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoTimedLogoutTitle"),
		Description = LanguageManager.Get("AutoTimedLogoutDescription"),
		Category = ModuleCategory.System,
		ModulesPrerequisite = new string[1] { "InstantLogout" },
		Author = new string[1] { "Wotou" }
	};

	protected override void Uninit()
	{
		Abort();
	}

	protected override void ConfigUI()
	{
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector;
		if (scheduledTime.HasValue)
		{
			long serverTime = Framework.GetServerTime();
			long num = scheduledTime.Value - serverTime;
			if (num > 0)
			{
				long num2 = num / 3600;
				long num3 = num % 3600 / 60;
				long num4 = num % 60;
				string valueOrDefault = ModeLoc.GetValueOrDefault(currentOperation, string.Empty);
				vector = KnownColor.GreenYellow.ToVector4();
				ImU8String val = default(ImU8String);
				((ImU8String)(ref val))._002Ector(1, 1);
				((ImU8String)(ref val)).AppendFormatted<string>(valueOrDefault);
				((ImU8String)(ref val)).AppendLiteral(":");
				ImGui.TextColored(ref vector, val);
				ImGui.SameLine();
				ImU8String val2 = default(ImU8String);
				((ImU8String)(ref val2))._002Ector(3, 3);
				((ImU8String)(ref val2)).AppendLiteral(" ");
				((ImU8String)(ref val2)).AppendFormatted<long>(num2, "D2");
				((ImU8String)(ref val2)).AppendLiteral(":");
				((ImU8String)(ref val2)).AppendFormatted<long>(num3, "D2");
				((ImU8String)(ref val2)).AppendLiteral(":");
				((ImU8String)(ref val2)).AppendFormatted<long>(num4, "D2");
				ImGui.TextUnformatted(val2);
			}
			if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61453, LanguageManager.Get("Cancel")))
			{
				Abort();
			}
			return;
		}
		vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("Operation")));
		IndentDisposable val3 = ImRaii.PushIndent(1, true);
		try
		{
			bool flag = true;
			foreach (var (operationMode2, text2) in ModeLoc)
			{
				if (!flag)
				{
					ImGui.SameLine();
				}
				flag = false;
				if (ImGui.RadioButton(ImU8String.op_Implicit(text2), currentOperation == operationMode2))
				{
					currentOperation = operationMode2;
				}
			}
		}
		finally
		{
			((IDisposable)val3)?.Dispose();
		}
		vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("Time")));
		val3 = ImRaii.PushIndent(1, true);
		try
		{
			ImGui.SetNextItemWidth(150f * Globals.GlobalUIScale);
			ImU8String val4 = new ImU8String(13, 1);
			((ImU8String)(ref val4)).AppendFormatted<string>(LanguageManager.Get("Minute"));
			((ImU8String)(ref val4)).AppendLiteral("##MinuteInput");
			ImU8String val5 = val4;
			ref int reference = ref customMinutes;
			ImU8String val6 = default(ImU8String);
			if (ImGui.InputInt(val5, ref reference, 1, 10, val6, (ImGuiInputTextFlags)0))
			{
				customMinutes = Math.Clamp(customMinutes, 1, 14400);
			}
			val6 = new ImU8String(3, 1);
			((ImU8String)(ref val6)).AppendLiteral("30 ");
			((ImU8String)(ref val6)).AppendFormatted<string>(LanguageManager.Get("Minute"));
			if (ImGui.Button(val6, default(Vector2)))
			{
				customMinutes = 30;
			}
			ImGui.SameLine();
			ImU8String val7 = new ImU8String(2, 1);
			((ImU8String)(ref val7)).AppendLiteral("1 ");
			((ImU8String)(ref val7)).AppendFormatted<string>(LanguageManager.Get("Hour"));
			if (ImGui.Button(val7, default(Vector2)))
			{
				customMinutes = 60;
			}
			ImGui.SameLine();
			ImU8String val8 = new ImU8String(2, 1);
			((ImU8String)(ref val8)).AppendLiteral("2 ");
			((ImU8String)(ref val8)).AppendFormatted<string>(LanguageManager.Get("Hour"));
			if (ImGui.Button(val8, default(Vector2)))
			{
				customMinutes = 120;
			}
			ImGui.SameLine();
			ImU8String val9 = new ImU8String(2, 1);
			((ImU8String)(ref val9)).AppendLiteral("3 ");
			((ImU8String)(ref val9)).AppendFormatted<string>(LanguageManager.Get("Hour"));
			if (ImGui.Button(val9, default(Vector2)))
			{
				customMinutes = 180;
			}
			ImGui.SameLine();
			ImU8String val10 = new ImU8String(2, 1);
			((ImU8String)(ref val10)).AppendLiteral("6 ");
			((ImU8String)(ref val10)).AppendFormatted<string>(LanguageManager.Get("Hour"));
			if (ImGui.Button(val10, default(Vector2)))
			{
				customMinutes = 360;
			}
			ImGui.SameLine();
			ImU8String val11 = new ImU8String(3, 1);
			((ImU8String)(ref val11)).AppendLiteral("12 ");
			((ImU8String)(ref val11)).AppendFormatted<string>(LanguageManager.Get("Hour"));
			if (ImGui.Button(val11, default(Vector2)))
			{
				customMinutes = 720;
			}
			ImGui.SameLine();
			ImU8String val12 = new ImU8String(3, 1);
			((ImU8String)(ref val12)).AppendLiteral("24 ");
			((ImU8String)(ref val12)).AppendFormatted<string>(LanguageManager.Get("Hour"));
			if (ImGui.Button(val12, default(Vector2)))
			{
				customMinutes = 1440;
			}
		}
		finally
		{
			((IDisposable)val3)?.Dispose();
		}
		ImGui.Spacing();
		if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61452, LanguageManager.Get("Confirm")))
		{
			StartWithMinutes(customMinutes, currentOperation);
		}
	}

	private void StartWithMinutes(int minutes, OperationMode operation)
	{
		Abort();
		currentOperation = operation;
		scheduledTime = Framework.GetServerTime() + minutes * 60;
		cancelSource = new CancellationTokenSource();
		WaitForTimer(minutes * 60 * 1000, cancelSource.Token);
	}

	private async Task WaitForTimer(int delayMs, CancellationToken token)
	{
		_ = 2;
		try
		{
			await Task.Delay(delayMs, token);
			if (token.IsCancellationRequested)
			{
				return;
			}
			scheduledTime = null;
			switch (currentOperation)
			{
			case OperationMode.ShutdownPC:
				ExecuteShutdownPC();
				break;
			case OperationMode.Logout:
				await DalamudServiceExtension.Instance<IFramework>().RunOnFrameworkThread((Action)delegate
				{
					OmenServiceBase<ChatManager>.Instance().SendMessage("/logout");
				});
				break;
			case OperationMode.ShutdownGame:
				await DalamudServiceExtension.Instance<IFramework>().RunOnFrameworkThread((Action)delegate
				{
					OmenServiceBase<ChatManager>.Instance().SendMessage("/shutdown");
				});
				break;
			}
		}
		catch (TaskCanceledException)
		{
		}
	}

	private static void ExecuteShutdownPC()
	{
		try
		{
			LocalShutdown.Start(new ProcessStartInfo
			{
				FileName = "shutdown",
				Arguments = "/s /t 0",
				UseShellExecute = false,
				CreateNoWindow = true
			});
		}
		catch (Exception ex)
		{
			DLog.Error("尝试自动关闭电脑失败: " + ex.Message, ex);
		}
	}

	private void Abort()
	{
		scheduledTime = null;
		if (cancelSource != null)
		{
			if (!cancelSource.IsCancellationRequested)
			{
				cancelSource.Cancel();
			}
			cancelSource.Dispose();
			cancelSource = null;
		}
	}
}
