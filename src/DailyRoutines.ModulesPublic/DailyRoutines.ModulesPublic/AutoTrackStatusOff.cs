using System;
using System.Collections.Generic;
using System.Linq;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Lumina.Excel.Sheets;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.ImGuiOm;
using OmenTools.ImGuiOm.Widgets.Combos;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class AutoTrackStatusOff : ModuleBase
{
	private class Config : ModuleConfig
	{
		public bool OnlyTrackSpecific;

		public bool SendChat = true;

		public HashSet<uint> StatusToMonitor = new HashSet<uint>();
	}

	private Config config;

	private StatusSelectCombo? statusSelectCombo;

	private readonly Dictionary<uint, (float Duration, ulong SourceID, DateTime GainTime, uint TargetID)> records = new Dictionary<uint, (float, ulong, DateTime, uint)>();

	private const float TIME_THRESHOLD = 0.2f;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoTrackStatusOffTitle"),
		Description = LanguageManager.Get("AutoTrackStatusOffDescription"),
		Category = ModuleCategory.Combat,
		Author = new string[1] { "Fragile" }
	};

	protected override void Init()
	{
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		this.statusSelectCombo = new StatusSelectCombo("Status", ((IEnumerable<Status>)LuminaGetter.Get<Status>()).Where((Status x) => ((Status)(ref x)).CanStatusOff && !string.IsNullOrEmpty(((object)((Status)(ref x)).Name/*cast due to constrained. prefix*/).ToString())));
		if (config.StatusToMonitor.Count > 0)
		{
			StatusSelectCombo statusSelectCombo = this.statusSelectCombo;
			HashSet<uint> hashSet = new HashSet<uint>();
			foreach (uint item in config.StatusToMonitor)
			{
				hashSet.Add(item);
			}
			statusSelectCombo.SelectedIDs = hashSet;
		}
		OmenServiceBase<CharacterStatusManager>.Instance().RegGain(OnGainStatus);
		OmenServiceBase<CharacterStatusManager>.Instance().RegLose(OnLoseStatus);
	}

	protected override void Uninit()
	{
		OmenServiceBase<CharacterStatusManager>.Instance().Unreg(OnGainStatus);
		OmenServiceBase<CharacterStatusManager>.Instance().Unreg(OnLoseStatus);
		records.Clear();
	}

	protected override void ConfigUI()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("SendChat")), ref config.SendChat))
		{
			config.Save(this);
		}
		ImGui.NewLine();
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("AutoTrackStatusOff-OnlyTrackSpecific")), ref config.OnlyTrackSpecific))
		{
			config.Save(this);
			records.Clear();
		}
		if (!config.OnlyTrackSpecific)
		{
			return;
		}
		if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)62831, LanguageManager.Get("Import")))
		{
			HashSet<uint> hashSet = ModuleBase.ImportFromClipboard<HashSet<uint>>();
			if (hashSet != null)
			{
				config.StatusToMonitor.AddRange(hashSet);
				config.Save(this);
			}
		}
		ImGui.SameLine();
		DisabledDisposable val = ImRaii.Disabled(config.StatusToMonitor.Count > 0);
		try
		{
			if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)62830, LanguageManager.Get("Export")))
			{
				ModuleBase.ExportToClipboard(config.StatusToMonitor);
				OmenServiceBase<NotifyHelper>.Instance().NotificationSuccess(LanguageManager.Get("CopiedToClipboard") ?? "");
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		ImGui.Spacing();
		if (statusSelectCombo.DrawCheckbox())
		{
			config.StatusToMonitor = statusSelectCombo.SelectedItems.Select((Status x) => ((Status)(ref x)).RowId).ToHashSet();
			config.Save(this);
		}
	}

	private void OnGainStatus(IBattleChara player, ushort statusID, ushort param, ushort stackCount, TimeSpan remainingTime, ulong sourceID)
	{
		if (!(remainingTime.TotalSeconds <= 0.0) && (!config.OnlyTrackSpecific || config.StatusToMonitor.Contains(statusID)) && LuminaGetter.TryGetRow<Status>((uint)statusID, out Status item) && ((Status)(ref item)).CanStatusOff && sourceID == LocalPlayerState.EntityID)
		{
			records[statusID] = ((float)remainingTime.TotalSeconds, sourceID, OmenServiceBase<StandardTimeManager>.Instance().Now, player.EntityID);
		}
	}

	private void OnLoseStatus(IBattleChara player, ushort statusID, ushort param, ushort stackCount, ulong sourceID)
	{
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Expected O, but got Unknown
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		if ((!config.OnlyTrackSpecific || config.StatusToMonitor.Contains(statusID)) && LuminaGetter.TryGetRow<Status>((uint)statusID, out Status item) && ((Status)(ref item)).CanStatusOff && sourceID == LocalPlayerState.EntityID && records.TryGetValue(statusID, out (float, ulong, DateTime, uint) value))
		{
			float item2 = value.Item1;
			double totalSeconds = (OmenServiceBase<StandardTimeManager>.Instance().Now - value.Item3).TotalSeconds;
			if (totalSeconds < (double)(item2 * 0.2f) && !player.IsDead && config.SendChat)
			{
				NotifyHelper notifyHelper = OmenServiceBase<NotifyHelper>.Instance();
				object[] obj = new object[7]
				{
					LuminaWrapper.GetStatusName(statusID),
					statusID,
					$"{item2:F1}",
					$"{totalSeconds:F1}",
					(object)new PlayerPayload(player.Name, player.HomeWorld.RowId),
					player.ClassJob.Value.ToBitmapFontIcon(),
					null
				};
				ClassJob value2 = player.ClassJob.Value;
				obj[6] = ((object)((ClassJob)(ref value2)).Name/*cast due to constrained. prefix*/).ToString();
				notifyHelper.Chat(LanguageManager.GetSe("AutoTrackStatusOff-Notification", obj));
			}
			records.Remove(statusID);
		}
	}
}
