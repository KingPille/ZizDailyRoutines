using System;
using System.Collections.Generic;
using System.Numerics;
using DailyRoutines.Common.Extensions;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Interface;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.Sheets;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.ImGuiOm.Widgets.Combos;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;

namespace DailyRoutines.ModulesPublic;

public class AutoMount : ModuleBase
{
	private class Config : ModuleConfig
	{
		public HashSet<uint> BlacklistZones = new HashSet<uint>();

		public int Delay = 1000;

		public bool MountWhenCombatEnd = true;

		public bool MountWhenGatherEnd = true;

		public bool MountWhenZoneChange = true;

		public bool JumpAfterMount;

		public uint SelectedMount;
	}

	private Config config;

	private MountSelectCombo mountSelectCombo;

	private ZoneSelectCombo zoneSelectCombo;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoMountTitle"),
		Description = LanguageManager.Get("AutoMountDescription"),
		Category = ModuleCategory.System
	};

	protected override void Init()
	{
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Expected O, but got Unknown
		mountSelectCombo = new MountSelectCombo("Mount");
		zoneSelectCombo = new ZoneSelectCombo("Zone");
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		mountSelectCombo.SelectedID = config.SelectedMount;
		zoneSelectCombo.SelectedIDs = config.BlacklistZones;
		base.TaskHelper = new TaskHelper
		{
			TimeoutMS = 10000
		};
		DalamudServiceExtension.Instance<ICondition>().ConditionChange += new ConditionChangeDelegate(OnConditionChanged);
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += OnZoneChanged;
	}

	protected override void Uninit()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChanged;
		DalamudServiceExtension.Instance<ICondition>().ConditionChange -= new ConditionChangeDelegate(OnConditionChanged);
	}

	protected override void ConfigUI()
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		ItemWidthDisposable val = ImRaii.ItemWidth(300f * Globals.GlobalUIScale);
		try
		{
			using (ImRaiiExtension.Heading1(LuminaWrapper.GetAddonText(4964u)))
			{
				if (mountSelectCombo.DrawRadio())
				{
					config.SelectedMount = mountSelectCombo.SelectedID;
					config.Save(this);
				}
				ImGui.SameLine();
				ImU8String val2 = new ImU8String(1, 2);
				((ImU8String)(ref val2)).AppendFormatted<string>(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61741));
				((ImU8String)(ref val2)).AppendLiteral(" ");
				((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get("Clear"));
				if (ImGui.Button(val2, default(Vector2)))
				{
					config.SelectedMount = 0u;
					config.Save(this);
				}
				IDalamudTextureWrap texture2;
				if (config.SelectedMount == 0 || !LuminaGetter.TryGetRow<Mount>(config.SelectedMount, out Mount item))
				{
					if (ImageHelper.TryGetGameIcon(118u, out IDalamudTextureWrap texture))
					{
						ImGuiOm.TextImage(LuminaWrapper.GetGeneralActionName(9u), texture.Handle, new Vector2(ImGui.GetTextLineHeightWithSpacing()));
					}
				}
				else if (ImageHelper.TryGetGameIcon(((Mount)(ref item)).Icon, out texture2))
				{
					ImGuiOm.TextImage(((object)((Mount)(ref item)).Singular/*cast due to constrained. prefix*/).ToString(), texture2.Handle, new Vector2(ImGui.GetTextLineHeightWithSpacing()));
				}
			}
			ImGui.NewLine();
			if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("AutoMount-MountWhenZoneChange")), ref config.MountWhenZoneChange))
			{
				config.Save(this);
			}
			if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("AutoMount-MountWhenGatherEnd")), ref config.MountWhenGatherEnd))
			{
				config.Save(this);
			}
			if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("AutoMount-MountWhenCombatEnd")), ref config.MountWhenCombatEnd))
			{
				config.Save(this);
			}
			ImGui.NewLine();
			if (zoneSelectCombo.DrawCheckbox())
			{
				config.BlacklistZones = zoneSelectCombo.SelectedIDs;
				config.Save(this);
			}
			ImGui.SameLine();
			ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("BlacklistZones")));
			ImU8String val3 = new ImU8String(23, 1);
			((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get("Delay"));
			((ImU8String)(ref val3)).AppendLiteral(" (ms)###AutoMount-Delay");
			if (ImGui.InputInt(val3, ref config.Delay, 0, 0, default(ImU8String), (ImGuiInputTextFlags)0))
			{
				config.Delay = Math.Max(0, config.Delay);
			}
			if (ImGui.IsItemDeactivatedAfterEdit())
			{
				config.Save(this);
			}
			if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("AutoMount-JumpAfterMount")), ref config.JumpAfterMount))
			{
				config.Save(this);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void OnZoneChanged(uint u)
	{
		if (config.MountWhenZoneChange && GameState.TerritoryType != 0 && !config.BlacklistZones.Contains(GameState.TerritoryType) && CanUseMountCurrentZone())
		{
			base.TaskHelper.Abort();
			base.TaskHelper.Enqueue((Func<bool>)UseMount, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
		}
	}

	private unsafe void OnConditionChanged(ConditionFlag flag, bool value)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Invalid comparison between Unknown and I4
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Invalid comparison between Unknown and I4
		if (config.BlacklistZones.Contains(GameState.TerritoryType))
		{
			return;
		}
		if ((int)flag != 6)
		{
			if ((int)flag != 26 || value || !config.MountWhenCombatEnd || DalamudServiceExtension.Instance<IClientState>().IsPvP || (((FateManager)FateManager.Instance()).CurrentFate != null && ((FateContext)((FateManager)FateManager.Instance()).CurrentFate).Progress != 100))
			{
				return;
			}
		}
		else if (value || !config.MountWhenGatherEnd)
		{
			return;
		}
		if (CanUseMountCurrentZone())
		{
			base.TaskHelper.Abort();
			base.TaskHelper.DelayNext(500);
			base.TaskHelper.Enqueue((Func<bool>)UseMount, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
		}
	}

	private unsafe bool UseMount()
	{
		if (!Throttler.Shared.Throttle("AutoMount.UseMount"))
		{
			return false;
		}
		if (IConditionExtension.get_IsOnMount(DalamudServiceExtension.Instance<ICondition>()) || OmenServiceBase<LocalPlayerState>.Instance().IsMoving)
		{
			return true;
		}
		if (IConditionExtension.get_IsCasting(DalamudServiceExtension.Instance<ICondition>()) || IConditionExtension.get_IsBetweenAreas(DalamudServiceExtension.Instance<ICondition>()) || ((ActionManager)ActionManager.Instance()).GetActionStatus((ActionType)5, 9u, 3758096384uL, true, true, (uint*)null) != 0)
		{
			return false;
		}
		if (config.Delay > 0)
		{
			base.TaskHelper.DelayNext(config.Delay);
		}
		base.TaskHelper.DelayNext(100);
		base.TaskHelper.Enqueue(() => (config.SelectedMount != 0) ? OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)13, config.SelectedMount, 3758096384uL, 0u, (UseActionMode)0) : OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)5, 9u, 3758096384uL, 0u, (UseActionMode)0));
		if (config.JumpAfterMount)
		{
			base.TaskHelper.Enqueue(delegate
			{
				if (((PlayerState)PlayerState.Instance()).CanFly)
				{
					base.TaskHelper.Enqueue(() => IConditionExtension.get_IsOnMount(DalamudServiceExtension.Instance<ICondition>()), "WaitForMount", 5000);
					base.TaskHelper.Enqueue(() => OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)5, 2u, 3758096384uL, 0u, (UseActionMode)0), "使用跳跃");
					base.TaskHelper.DelayNext(50);
					base.TaskHelper.Enqueue(() => OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)5, 2u, 3758096384uL, 0u, (UseActionMode)0), "再次使用跳跃");
				}
			});
		}
		return true;
	}

	private static bool CanUseMountCurrentZone()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		TerritoryType territoryTypeData = GameState.TerritoryTypeData;
		return ((TerritoryType)(ref territoryTypeData)).Mount;
	}
}
