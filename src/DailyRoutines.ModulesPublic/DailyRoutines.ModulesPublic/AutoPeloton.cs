using System;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Info.Game.Enums;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;

namespace DailyRoutines.ModulesPublic;

public class AutoPeloton : ModuleBase
{
	private class Config : ModuleConfig
	{
		public bool DisableInWalk = true;

		public bool OnlyInDuty = true;
	}

	private Config config;

	private const uint PELOTONING_ACTION_ID = 7557u;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoPelotonTitle"),
		Description = LanguageManager.Get("AutoPelotonDescription"),
		Category = ModuleCategory.Action,
		Author = new string[1] { "yamiYori" }
	};

	protected override void Init()
	{
		if (base.TaskHelper == null)
		{
			TaskHelper taskHelper = (base.TaskHelper = new TaskHelper());
		}
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		OmenServiceBase<LocalPlayerState>.Instance().PlayerMoveStateChanged += OnMoveStateChanged;
		OmenServiceBase<CharacterStatusManager>.Instance().RegLose(OnLoseStatus);
	}

	protected override void Uninit()
	{
		OmenServiceBase<LocalPlayerState>.Instance().PlayerMoveStateChanged -= OnMoveStateChanged;
		OmenServiceBase<CharacterStatusManager>.Instance().Unreg(OnLoseStatus);
	}

	protected override void ConfigUI()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("OnlyInDuty")), ref config.OnlyInDuty))
		{
			config.Save(this);
		}
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("AutoPeloton-DisableInWalk")), ref config.DisableInWalk))
		{
			config.Save(this);
		}
	}

	private void OnLoseStatus(IBattleChara player, ushort id, ushort param, ushort stackCount, ulong sourceID)
	{
		if (player.Address == LocalPlayerState.Object?.Address && (id == 1199 || id == 50))
		{
			CheckAndUsePeloton();
		}
	}

	private void OnMoveStateChanged(bool isMoving)
	{
		if (isMoving)
		{
			CheckAndUsePeloton();
		}
	}

	private void CheckAndUsePeloton()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		if ((!config.OnlyInDuty || GameState.ContentFinderCondition != 0) && !GameState.IsInPVPArea && !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)26] && UIModuleExtension.IsScreenReady() && !IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()) && IOmenDalamudService<IObjectTable>.Instance().LocalPlayer != null && LocalPlayerState.ClassJobData.ToJobType() == ClassJobType.PhysicalRanged && ActionManagerExtension.IsActionUnlocked(7557u) && (!config.DisableInWalk || !LocalPlayerState.IsWalking))
		{
			base.TaskHelper.Abort();
			base.TaskHelper.Enqueue((Func<bool>)UsePeloton, $"UseAction_{7557u}", 5000, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 1);
		}
	}

	private unsafe bool UsePeloton()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
		if (localPlayer == null)
		{
			return false;
		}
		ActionManager* intPtr = ActionManager.Instance();
		StatusManager statusManager = ((BattleChara)localPlayer.ToStruct()).StatusManager;
		if (((ActionManager)intPtr).GetActionStatus((ActionType)1, 7557u, 3758096384uL, true, true, (uint*)null) != 0)
		{
			return false;
		}
		if (((StatusManager)(ref statusManager)).HasStatus(1199u, 3758096384u) || ((StatusManager)(ref statusManager)).HasStatus(50u, 3758096384u))
		{
			return true;
		}
		if (!OmenServiceBase<LocalPlayerState>.Instance().IsMoving)
		{
			return true;
		}
		base.TaskHelper.Enqueue(() => OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)1, 7557u, 3758096384uL, 0u, (UseActionMode)0), $"UseAction_{7557u}", 5000, null, null, null, null, 1);
		return true;
	}
}
