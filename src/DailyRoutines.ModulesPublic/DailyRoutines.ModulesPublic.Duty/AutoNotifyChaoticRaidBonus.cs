using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using DailyRoutines.RemoteInteraction.ChaoticUptime;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using Lumina.Excel.Sheets;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic.Duty;

public class AutoNotifyChaoticRaidBonus : ModuleBase
{
	private class Config : ModuleConfig
	{
		public Dictionary<string, bool> DataCenters = new Dictionary<string, bool>();

		public Dictionary<string, long> DataCentersNotifyTime = new Dictionary<string, long>();
	}

	private record StateSnapshot(string CurrentDC, bool IsLoggedIn, bool IsInInstance, bool IsBonusActive, long ServerTime);

	private Config config;

	private readonly CancellationTokenSource cancelSource = new CancellationTokenSource();

	private static readonly string[] AllDataCenters = new string[15]
	{
		"陆行鸟", "莫古力", "猫小胖", "豆豆柴", "Elemental", "Gaia", "Mana", "Meteor", "Aether", "Crystal",
		"Dynamis", "Primal", "Light", "Chaos", "Materia"
	};

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoNotifyChaoticRaidBonusTitle"),
		Description = LanguageManager.Get("AutoNotifyChaoticRaidBonusDescription"),
		Category = ModuleCategory.Duty
	};

	protected override void Init()
	{
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		bool flag = false;
		string[] allDataCenters = AllDataCenters;
		foreach (string key in allDataCenters)
		{
			if (config.DataCenters.TryAdd(key, value: false))
			{
				flag = true;
			}
		}
		allDataCenters = AllDataCenters;
		foreach (string key2 in allDataCenters)
		{
			if (config.DataCentersNotifyTime.TryAdd(key2, 0L))
			{
				flag = true;
			}
		}
		if (flag)
		{
			config.Save(this);
		}
		Task.Run(() => CheckLoop(cancelSource.Token));
	}

	protected override void Uninit()
	{
		cancelSource.Cancel();
		cancelSource.Dispose();
	}

	protected override void ConfigUI()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		ImU8String val = ImU8String.op_Implicit("Table");
		Vector2 vector = ImGui.GetContentRegionAvail() / 1.5f;
		vector.Y = 0f;
		TableDisposable val2 = ImRaii.Table(val, 3, (ImGuiTableFlags)0, vector);
		try
		{
			if (!val2)
			{
				return;
			}
			ImGui.TableSetupColumn(ImU8String.op_Implicit(LuminaWrapper.GetLobbyText(802u)), (ImGuiTableColumnFlags)0, 0f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit(LanguageManager.Get("Enable")), (ImGuiTableColumnFlags)0, 0f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit(LanguageManager.Get("AutoNotifyChaoticRaidBonus-LastBonusNotifyTime")), (ImGuiTableColumnFlags)0, 0f, 0u);
			ImGui.TableHeadersRow();
			foreach (var (text2, flag2) in config.DataCenters)
			{
				if (config.DataCentersNotifyTime.TryGetValue(text2, out var value))
				{
					ImGui.TableNextRow();
					ImGui.TableNextColumn();
					ImU8String val3 = new ImU8String(0, 1);
					((ImU8String)(ref val3)).AppendFormatted<string>(text2);
					ImGui.TextUnformatted(val3);
					bool value2 = flag2;
					ImGui.TableNextColumn();
					ImU8String val4 = new ImU8String(12, 1);
					((ImU8String)(ref val4)).AppendLiteral("###");
					((ImU8String)(ref val4)).AppendFormatted<string>(text2);
					((ImU8String)(ref val4)).AppendLiteral("IsEnabled");
					if (ImGui.Checkbox(val4, ref value2))
					{
						config.DataCenters[text2] = value2;
						config.Save(this);
					}
					ImGui.TableNextColumn();
					ImU8String val5 = new ImU8String(0, 1);
					((ImU8String)(ref val5)).AppendFormatted<DateTime>(DateTimeOffset.FromUnixTimeSeconds(value).LocalDateTime);
					ImGui.TextUnformatted(val5);
				}
			}
		}
		finally
		{
			((TableDisposable)(ref val2)).Dispose();
		}
	}

	private async Task CheckLoop(CancellationToken ct)
	{
		await Task.Delay(5000, ct);
		while (!ct.IsCancellationRequested)
		{
			try
			{
				int minute = DateTime.Now.Minute;
				if (minute > 5 && minute < 55)
				{
					await Task.Delay(TimeSpan.FromMinutes(55 - minute), ct);
					continue;
				}
				StateSnapshot stateSnapshot = await GetStateSnapshot();
				if (stateSnapshot != null)
				{
					await RunCheckAsync(stateSnapshot);
				}
				await Task.Delay(60000, ct);
			}
			catch (OperationCanceledException)
			{
				break;
			}
			catch (Exception)
			{
			}
		}
	}

	private static Task<StateSnapshot?> GetStateSnapshot()
	{
		TaskCompletionSource<StateSnapshot?> tcs = new TaskCompletionSource<StateSnapshot>();
		DalamudServiceExtension.Instance<IFramework>().RunOnTick((Action)delegate
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (!GameState.IsLoggedIn)
				{
					tcs.SetResult(null);
				}
				else
				{
					TaskCompletionSource<StateSnapshot?> taskCompletionSource = tcs;
					WorldDCGroupType currentDataCenterData = GameState.CurrentDataCenterData;
					taskCompletionSource.SetResult(new StateSnapshot(((object)((WorldDCGroupType)(ref currentDataCenterData)).Name/*cast due to constrained. prefix*/).ToString(), IsLoggedIn: true, GameState.IsInInstanceArea, GameState.IsChaoticRaidBonusActive, GameState.ServerTimeUnix));
				}
			}
			catch (Exception exception)
			{
				tcs.SetException(exception);
			}
		}, default(TimeSpan), 0, default(CancellationToken));
		return tcs.Task;
	}

	private async Task RunCheckAsync(StateSnapshot state)
	{
		(string, long)?[] array = await Task.WhenAll(AllDataCenters.Select((string dcName) => CheckDC(dcName, state)));
		for (int num = 0; num < array.Length; num++)
		{
			(string, long)? tuple = array[num];
			if (tuple.HasValue)
			{
				config.DataCentersNotifyTime[tuple.Value.Item1] = tuple.Value.Item2;
			}
		}
	}

	private async Task<(string DC, long Time)?> CheckDC(string dcName, StateSnapshot state)
	{
		if (!config.DataCenters.TryGetValue(dcName, out var value) || (!value && state.CurrentDC != dcName))
		{
			return null;
		}
		long valueOrDefault = config.DataCentersNotifyTime.GetValueOrDefault(dcName, 0L);
		if (state.ServerTime - valueOrDefault < 10800)
		{
			return null;
		}
		if ((object)state != null && state.IsLoggedIn && !state.IsInInstance && state.CurrentDC == dcName)
		{
			if (state.IsBonusActive)
			{
				Notify(dcName);
				return (dcName, state.ServerTime);
			}
		}
		else
		{
			try
			{
				if ((await RemoteChaoticUptime.GetFreshAsync(dcName, cancelSource.Token)).IsUptime)
				{
					Notify(dcName);
					return (dcName, state.ServerTime);
				}
			}
			catch
			{
			}
		}
		return null;
	}

	private static void Notify(string dcName)
	{
		DalamudServiceExtension.Instance<IFramework>().RunOnTick((Action)delegate
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Expected O, but got Unknown
			string text = LanguageManager.Get("AutoNotifyChaoticRaidBonus-Notification", new Dictionary<string, object> { ["dataCenter"] = dcName });
			OmenServiceBase<NotifyHelper>.Instance().TrayInfo(text);
			NotifyHelper.Toast(text);
			NotifyHelper.Chat(new XivChatEntry
			{
				Type = (XivChatType)3,
				Message = SeString.op_Implicit(text)
			});
		}, default(TimeSpan), 0, default(CancellationToken));
	}
}
