using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Numerics;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.AddonEvent;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.ModulesPublic;

public class AutoJumboCactpot : ModuleBase
{
	private class Config : ModuleConfig
	{
		public int FixedNumber = 1;

		public Mode NumberMode;
	}

	private enum Mode
	{
		Random,
		Fixed
	}

	private Config config;

	private static readonly FrozenDictionary<Mode, string> NumberModeLoc = new Dictionary<Mode, string>
	{
		[Mode.Random] = LanguageManager.Get("AutoJumboCactpot-Random"),
		[Mode.Fixed] = LanguageManager.Get("AutoJumboCactpot-Fixed")
	}.ToFrozenDictionary();

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoJumboCactpotTitle"),
		Description = LanguageManager.Get("AutoJumboCactpotDescription"),
		Category = ModuleCategory.GoldSaucer
	};

	protected unsafe override void Init()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 5000
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)1, "LotteryWeeklyInput", new AddonEventDelegate(OnAddon));
		if ((*Addons.LotteryWeeklyInput).IsAddonAndNodesReady())
		{
			OnAddon((AddonEvent)1, null);
		}
	}

	protected override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddon });
	}

	protected override void ConfigUI()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		ImGui.SetNextItemWidth(100f * Globals.GlobalUIScale);
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(0, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("AutoJumboCactpot-NumberMode"));
		ComboDisposable val2 = ImRaii.Combo(val, ImU8String.op_Implicit(NumberModeLoc.GetValueOrDefault(config.NumberMode, string.Empty)));
		try
		{
			if (ComboDisposable.op_Implicit(val2))
			{
				foreach (KeyValuePair<Mode, string> item in NumberModeLoc)
				{
					if (ImGui.Selectable(ImU8String.op_Implicit(item.Value), item.Key == config.NumberMode, (ImGuiSelectableFlags)0, default(Vector2)))
					{
						config.NumberMode = item.Key;
						config.Save(this);
					}
				}
			}
		}
		finally
		{
			((ComboDisposable)(ref val2)).Dispose();
		}
		if (config.NumberMode == Mode.Fixed)
		{
			ImGui.SameLine();
			ImGui.SetNextItemWidth(100f * Globals.GlobalUIScale);
			ImGui.InputInt(ImU8String.op_Implicit(LanguageManager.Get("AutoJumboCactpot-FixedNumber")), ref config.FixedNumber, 0, 0, default(ImU8String), (ImGuiInputTextFlags)0);
			if (ImGui.IsItemDeactivatedAfterEdit())
			{
				config.FixedNumber = Math.Clamp(config.FixedNumber, 0, 9999);
				config.Save(this);
			}
		}
	}

	private unsafe void OnAddon(AddonEvent type, AddonArgs args)
	{
		base.TaskHelper.Abort();
		base.TaskHelper.Enqueue(delegate
		{
			if (!IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()))
			{
				base.TaskHelper.Abort();
				return true;
			}
			if (!(*Addons.LotteryWeeklyInput).IsAddonAndNodesReady())
			{
				return false;
			}
			int num = config.NumberMode switch
			{
				Mode.Random => Random.Shared.Next(0, 9999), 
				Mode.Fixed => Math.Clamp(config.FixedNumber, 0, 9999), 
				_ => 0, 
			};
			(*Addons.LotteryWeeklyInput).Callback(num);
			return true;
		});
		base.TaskHelper.Enqueue(delegate
		{
			AddonSelectYesnoEvent.ClickYes();
			return false;
		});
	}
}
