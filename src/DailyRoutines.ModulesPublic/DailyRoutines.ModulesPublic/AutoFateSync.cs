using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Threading;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Interface;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using Lumina.Excel.Sheets;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.ModulesPublic;

public class AutoFateSync : ModuleBase
{
	private class Config : ModuleConfig
	{
		public bool AutoTankStance;

		public float Delay = 3f;

		public bool IgnoreMounting = true;
	}

	private Config config;

	private CancellationTokenSource? cancelSource;

	private static readonly FrozenDictionary<uint, (uint ActionID, uint StatusID)> TankStanceActions = new Dictionary<uint, (uint, uint)>
	{
		[1u] = (28u, 79u),
		[19u] = (28u, 79u),
		[3u] = (48u, 91u),
		[21u] = (48u, 91u),
		[32u] = (3629u, 743u),
		[37u] = (16142u, 1833u)
	}.ToFrozenDictionary();

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoFateSyncTitle"),
		Description = LanguageManager.Get("AutoFateSyncDescription"),
		Category = ModuleCategory.Combat
	};

	protected override void Init()
	{
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 30000
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		if (cancelSource == null)
		{
			cancelSource = new CancellationTokenSource();
		}
		OmenServiceBase<GameState>.Instance().EnterFate += OnEnterFate;
	}

	protected override void Uninit()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		OmenServiceBase<GameState>.Instance().EnterFate -= OnEnterFate;
		OmenServiceBase<FrameworkManager>.Instance().Unreg(OnFlying);
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

	protected override void ConfigUI()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		ImGui.SetNextItemWidth(50f * Globals.GlobalUIScale);
		if (ImGui.InputFloat(ImU8String.op_Implicit(LanguageManager.Get("AutoFateSync-Delay")), ref config.Delay, 0f, 0f, ImU8String.op_Implicit("%.1f"), (ImGuiInputTextFlags)0))
		{
			config.Delay = Math.Max(0f, config.Delay);
		}
		if (ImGui.IsItemDeactivatedAfterEdit())
		{
			config.Save(this);
			cancelSource.Cancel();
		}
		ImGuiOm.HelpMarker(LanguageManager.Get("AutoFateSync-DelayHelp"), 40f, (FontAwesomeIcon)61530);
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("AutoFateSync-IgnoreMounting")), ref config.IgnoreMounting))
		{
			config.Save(this);
		}
		ImGuiOm.HelpMarker(LanguageManager.Get("AutoFateSync-IgnoreMountingHelp"), 40f, (FontAwesomeIcon)61530);
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("AutoFateSync-AutoTankStance")), ref config.AutoTankStance))
		{
			config.Save(this);
		}
	}

	private void OnEnterFate(uint fateID)
	{
		HandleFateEnter();
	}

	private unsafe void HandleFateEnter()
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected O, but got Unknown
		if (config.IgnoreMounting && (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)77] || IConditionExtension.get_IsOnMount(DalamudServiceExtension.Instance<ICondition>())))
		{
			OmenServiceBase<FrameworkManager>.Instance().Reg(new OnUpdateDelegate(OnFlying), 500u);
			return;
		}
		FateManager* manager = FateManager.Instance();
		if (config.Delay > 0f)
		{
			DalamudServiceExtension.Instance<IFramework>().RunOnTick((Action)delegate
			{
				if (((FateManager)manager).CurrentFate != null && IOmenDalamudService<IObjectTable>.Instance().LocalPlayer != null)
				{
					ExecuteFateLevelSync(((FateContext)((FateManager)manager).CurrentFate).FateId);
				}
			}, TimeSpan.FromSeconds(config.Delay), 0, cancelSource.Token);
		}
		else
		{
			ExecuteFateLevelSync(((FateContext)((FateManager)manager).CurrentFate).FateId);
		}
	}

	private unsafe void OnFlying(IFramework _)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Expected O, but got Unknown
		FateContext* currentFate = ((FateManager)FateManager.Instance()).CurrentFate;
		if (currentFate == null || IOmenDalamudService<IObjectTable>.Instance().LocalPlayer == null)
		{
			OmenServiceBase<FrameworkManager>.Instance().Unreg(OnFlying);
		}
		else if (!DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)77] && !IConditionExtension.get_IsOnMount(DalamudServiceExtension.Instance<ICondition>()))
		{
			ExecuteFateLevelSync(((FateContext)currentFate).FateId);
			OmenServiceBase<FrameworkManager>.Instance().Unreg(OnFlying);
		}
	}

	private unsafe void ExecuteFateLevelSync(ushort fateID)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.SyncToFateLevel, fateID, 1u);
		base.TaskHelper.Abort();
		if (!config.AutoTankStance)
		{
			return;
		}
		base.TaskHelper.Enqueue(() => !IConditionExtension.get_IsOnMount(DalamudServiceExtension.Instance<ICondition>()) && !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)48] && ((ActionManager)ActionManager.Instance()).GetActionStatus((ActionType)5, 2u, 3758096384uL, true, true, (uint*)null) == 0);
		base.TaskHelper.Enqueue(delegate
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			if (((FateManager)FateManager.Instance()).CurrentFate == null || !LuminaGetter.TryGetRow<Fate>((uint)fateID, out Fate item))
			{
				return true;
			}
			IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
			if (localPlayer == null)
			{
				return false;
			}
			if (!TankStanceActions.TryGetValue(localPlayer.ClassJob.RowId, out (uint, uint) value))
			{
				return false;
			}
			if (localPlayer.Level > ((Fate)(ref item)).ClassJobLevelMax)
			{
				return false;
			}
			if (LocalPlayerState.HasStatus(value.Item2, out var _))
			{
				return true;
			}
			OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)1, value.Item1, 3758096384uL, 0u, (UseActionMode)0);
			return true;
		});
	}
}
