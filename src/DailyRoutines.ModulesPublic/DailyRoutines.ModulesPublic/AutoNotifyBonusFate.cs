using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Fates;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;

namespace DailyRoutines.ModulesPublic;

public class AutoNotifyBonusFate : ModuleBase
{
	private class Config : ModuleConfig
	{
		public bool AutoOpenMap = true;
	}

	private Config config;

	private readonly HashSet<ushort> notifiedFates = new HashSet<ushort>();

	private static readonly FrozenSet<uint> ValidTerritories = (from x in ((IEnumerable<TerritoryType>)LuminaGetter.Get<TerritoryType>()).Where((TerritoryType x) => ((TerritoryType)(ref x)).TerritoryIntendedUse.RowId == 1).Where(delegate(TerritoryType x)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			ExVersion value = ((TerritoryType)(ref x)).ExVersion.Value;
			return ((ExVersion)(ref value)).RowId >= 2;
		})
		select ((TerritoryType)(ref x)).RowId).ToFrozenSet();

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoNotifyBonusFateTitle"),
		Description = LanguageManager.Get("AutoNotifyBonusFateDescription"),
		Category = ModuleCategory.Combat,
		Author = new string[1] { "Due" }
	};

	protected override void Init()
	{
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
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
		OmenServiceBase<ExecuteCommandManager>.Instance().Unreg(OnPostExecuteCommand);
		notifiedFates.Clear();
	}

	protected override void ConfigUI()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("OpenMap")), ref config.AutoOpenMap))
		{
			config.Save(this);
		}
	}

	private void OnZoneChanged(uint u)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().Unreg(OnPostExecuteCommand);
		base.TaskHelper.Abort();
		notifiedFates.Clear();
		if (ValidTerritories.Contains(GameState.TerritoryType))
		{
			OmenServiceBase<ExecuteCommandManager>.Instance().RegPost(OnPostExecuteCommand);
		}
	}

	private void OnPostExecuteCommand(ExecuteCommandFlag command, uint param1, uint param2, uint param3, uint param4)
	{
		if (command == ExecuteCommandFlag.LoadFate)
		{
			base.TaskHelper.Abort();
			base.TaskHelper.DelayNext(200);
			base.TaskHelper.Enqueue((Action)UpdateAndNotify, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
		}
	}

	private void UpdateAndNotify()
	{
		if (!ValidTerritories.Contains(GameState.TerritoryType) || GameState.Map == 0)
		{
			return;
		}
		IFateTable val = DalamudServiceExtension.Instance<IFateTable>();
		if (val.Length == 0)
		{
			if (notifiedFates.Count > 0)
			{
				notifiedFates.Clear();
			}
			return;
		}
		HashSet<ushort> hashSet = new HashSet<ushort>();
		foreach (IFate item in (IEnumerable<IFate>)val)
		{
			if (item.HasBonus)
			{
				hashSet.Add(item.FateId);
				if (notifiedFates.Add(item.FateId))
				{
					NotifyFate(item);
				}
			}
		}
		if (notifiedFates.Count > 0)
		{
			notifiedFates.IntersectWith(hashSet);
		}
	}

	private unsafe void NotifyFate(IFate fate)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		ReadOnlySeString se = LanguageManager.GetSe("AutoNotifyBonusFate-Chat", new Dictionary<string, object> { ["fate"] = SeStringExtension.CreateMapLink(fate.Position, 0u, 0u, ReadOnlySeString.op_Implicit($"{fate.Name}（{fate.Progress}%）")) });
		OmenServiceBase<NotifyHelper>.Instance().Chat(se, useDefaultPrefix: false);
		NotifyHelper.Toast(se);
		OmenServiceBase<NotifyHelper>.Instance().TrayInfo($"{fate.Name}（{fate.Progress}%）", LanguageManager.Get("AutoNotifyBonusFate-Notification"));
		if (config.AutoOpenMap)
		{
			(*AgentMap.Instance()).SetMapFlagAndOpen(GameState.Map, fate.Position, ((object)fate.Name).ToString());
		}
	}
}
