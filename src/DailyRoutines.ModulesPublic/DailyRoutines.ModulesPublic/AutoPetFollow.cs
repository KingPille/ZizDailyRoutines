using System.Collections.Frozen;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using OmenTools.Extensions;
using OmenTools.Info.Game.Enums;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;

namespace DailyRoutines.ModulesPublic;

public class AutoPetFollow : ModuleBase
{
	private class Config : ModuleConfig
	{
		public bool SendNotification = true;
	}

	private Config config;

	private static readonly FrozenSet<uint> ValidClassJobs = FrozenSet.Create<uint>(26u, 27u, 28u);

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoPetFollowTitle"),
		Description = LanguageManager.Get("AutoPetFollowDescription"),
		Category = ModuleCategory.Combat
	};

	protected override void Init()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		DalamudServiceExtension.Instance<ICondition>().ConditionChange += new ConditionChangeDelegate(OnConditionChanged);
	}

	protected override void Uninit()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		DalamudServiceExtension.Instance<ICondition>().ConditionChange -= new ConditionChangeDelegate(OnConditionChanged);
	}

	protected override void ConfigUI()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("SendNotification")), ref config.SendNotification))
		{
			config.Save(this);
		}
	}

	private unsafe void OnConditionChanged(ConditionFlag flag, bool value)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Invalid comparison between Unknown and I4
		if ((int)flag != 26 || value || GameState.IsInPVPArea || DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4] || !ValidClassJobs.Contains(LocalPlayerState.ClassJob))
		{
			return;
		}
		BattleChara* localPlayer = Control.GetLocalPlayer();
		if (localPlayer == null)
		{
			return;
		}
		BattleChara* ptr = ((CharacterManager)CharacterManager.Instance()).LookupPetByOwnerObject(localPlayer);
		if (ptr != null && ((BattleChara)ptr).GetIsTargetable())
		{
			OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommandComplex(ExecuteCommandComplexFlag.PetAction, 3758096384L, 2u);
			if (config.SendNotification && Throttler.Shared.Throttle("AutoPetFollow-SendNotification", 10000u))
			{
				OmenServiceBase<NotifyHelper>.Instance().NotificationInfo(LanguageManager.Get("AutoPetFollow-Notification"));
			}
		}
	}
}
