using System.Numerics;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.ModulesPublic;

public class AutoUseEarthsReply : ModuleBase
{
	private class Config : ModuleConfig
	{
		public bool UseWhenGuard;

		public bool UseWhenSprint;
	}

	private Config config;

	private const uint RIDDLE_OF_EARTH_ACTION = 29482u;

	private const uint EARTHS_REPLY_ACTION = 29483u;

	private const uint SPRINT_STATUS = 1342u;

	private const uint GUARD_STATUS = 3054u;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoUseEarthsReplyTitle"),
		Description = LanguageManager.Get("AutoUseEarthsReplyDescription"),
		Category = ModuleCategory.Action,
		Author = new string[1] { "ToxicStar" }
	};

	protected override void Init()
	{
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 8000
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		OmenServiceBase<UseActionManager>.Instance().RegPostUseActionLocation(OnUseAction);
	}

	protected override void Uninit()
	{
		OmenServiceBase<UseActionManager>.Instance().Unreg(OnUseAction);
	}

	protected override void ConfigUI()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("AutoUseEarthsReply-UseWhenGuard")), ref config.UseWhenSprint))
		{
			config.Save(this);
		}
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("AutoUseEarthsReply-UseWhenSprint")), ref config.UseWhenGuard))
		{
			config.Save(this);
		}
	}

	private void OnUseAction(bool result, ActionType actionType, uint actionID, ulong targetID, Vector3 location, uint extraParam, byte a7)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		if ((int)actionType != 1 || actionID != 29482 || !result)
		{
			return;
		}
		base.TaskHelper.Abort();
		base.TaskHelper.DelayNext(8000, $"Delay_UseAction{29483u}", 1);
		base.TaskHelper.Enqueue(delegate
		{
			if (!DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4])
			{
				IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
				if (localPlayer != null && (config.UseWhenSprint || !localPlayer.StatusList.HasStatus(1342u)) && (config.UseWhenGuard || !localPlayer.StatusList.HasStatus(3054u)))
				{
					OmenServiceBase<UseActionManager>.Instance().UseActionLocation((ActionType)1, 29483u, 3758096384uL, default(Vector3), 0u, 0);
				}
			}
		}, $"UseAction_{29483u}", 500, null, null, null, null, 1);
	}
}
