using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Component.GUI;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.ModulesPublic.Interface;

public class AutoClaimPVPRewards : ModuleBase
{
	private AtkEventWrapper? claimAllEvent;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoClaimPVPRewardsTitle"),
		Description = LanguageManager.Get("AutoClaimPVPRewardsDescription"),
		Category = ModuleCategory.Interface
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected unsafe override void Init()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 5000
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)5, "PvpReward", new AddonEventDelegate(OnAddon));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, "PvpReward", new AddonEventDelegate(OnAddon));
		if (Addons.PvpReward != null)
		{
			OnAddon((AddonEvent)1, null);
		}
	}

	protected override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddon });
		claimAllEvent?.Dispose();
		claimAllEvent = null;
	}

	private unsafe void OnAddon(AddonEvent type, AddonArgs? args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		if ((int)type != 5)
		{
			if ((int)type == 6)
			{
				claimAllEvent = null;
			}
		}
		else
		{
			if (Addons.PvpReward == null)
			{
				return;
			}
			AtkComponentButton* componentButtonById = ((AtkUnitBase)Addons.PvpReward).GetComponentButtonById(124u);
			if (componentButtonById == null)
			{
				return;
			}
			if (claimAllEvent == null)
			{
				((AtkComponentNode)((AtkComponentButton)componentButtonById).OwnerNode).ClearEvents();
				claimAllEvent = new AtkEventWrapper(delegate
				{
					for (int i = 0; i < 30; i++)
					{
						base.TaskHelper.Enqueue(delegate
						{
							if (IsTrophyCrystalAboutToReachLimit())
							{
								base.TaskHelper.Abort();
							}
						}, $"检查战利水晶数量_等级{i}");
						base.TaskHelper.Enqueue(delegate
						{
							OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.CollectTrophyCrystal);
							OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.CollectTrophyCrystal, 1u);
						}, $"领取系列赛奖励_等级{i}");
						base.TaskHelper.DelayNext(10, $"等待 10 毫秒以便回包_等级{i}");
					}
				});
				claimAllEvent.Add(Addons.PvpReward, (AtkResNode*)((AtkComponentButton)componentButtonById).OwnerNode, (AtkEventType)25);
				((AtkComponentButton)componentButtonById).SetText(LanguageManager.Get("AutoClaimPVPRewards-Button"));
			}
			((AtkComponentButton)componentButtonById).SetEnabledState(!base.TaskHelper.IsBusy);
		}
	}

	private unsafe static bool IsTrophyCrystalAboutToReachLimit()
	{
		return ((InventoryManager)InventoryManager.Instance()).GetInventoryItemCount(36656u, false, true, true, (short)0) > 19000;
	}
}
