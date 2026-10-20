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
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Global;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.ModulesPublic;

public class AutoLucidDreaming : ModuleBase
{
	private class Config : ModuleConfig
	{
		public int MPThreshold = 7500;

		public bool OnlyInDuty;
	}

	private Config config;

	private const int RECAST_TIME_WINDOW = 500;

	private const int ANIMATION_LOCK = 100;

	private const uint LUCID_DREAMING_ID = 7562u;

	private static readonly FrozenSet<uint> ValidClassJobs = FrozenSet.Create<uint>(6u, 24u, 26u, 27u, 28u, 33u, 35u, 36u, 40u, 42u);

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoLucidDreamingTitle"),
		Description = LanguageManager.Get("AutoLucidDreamingDescription"),
		Category = ModuleCategory.Action,
		Author = new string[1] { "qingsiweisan" }
	};

	protected override void Init()
	{
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 30000,
				ShowDebug = true
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		OmenServiceBase<UseActionManager>.Instance().RegPostCharacterCompleteCast(OnCompleteCast);
		OmenServiceBase<UseActionManager>.Instance().RegPostUseActionLocation(OnUseAction);
	}

	protected override void Uninit()
	{
		OmenServiceBase<UseActionManager>.Instance().Unreg(OnCompleteCast);
		OmenServiceBase<UseActionManager>.Instance().Unreg(OnUseAction);
	}

	protected override void ConfigUI()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		ImGui.SetNextItemWidth(250f * Globals.GlobalUIScale);
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(19, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("LowerLimit"));
		((ImU8String)(ref val)).AppendLiteral("##MPThresholdSlider");
		ImU8String val2 = val;
		ref int mPThreshold = ref config.MPThreshold;
		ImU8String val3 = default(ImU8String);
		((ImU8String)(ref val3))._002Ector(4, 1);
		((ImU8String)(ref val3)).AppendFormatted<string>(LuminaWrapper.GetAddonText(233u));
		((ImU8String)(ref val3)).AppendLiteral(": %d");
		if (ImGui.DragInt(val2, ref mPThreshold, 100f, 3000, 9000, val3, (ImGuiSliderFlags)0))
		{
			config.Save(this);
		}
		ImGui.NewLine();
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("OnlyInDuty")), ref config.OnlyInDuty))
		{
			config.Save(this);
		}
	}

	private unsafe void OnCompleteCast(bool result, IBattleChara player, ActionType type, uint actionID, uint spellID, GameObjectId animationTargetID, Vector3 location, float rotation, short lastUsedActionSequence, int animationVariation, int ballistaEntityID)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Invalid comparison between Unknown and I4
		if (!result || GameState.IsInPVPArea || (config.OnlyInDuty && GameState.ContentFinderCondition == 0) || player.EntityID != LocalPlayerState.EntityID || !ValidClassJobs.Contains(player.ClassJob.RowId) || player.CurrentMp > config.MPThreshold || !LuminaGetter.TryGetRow<Action>(actionID, out Action item) || ((Action)(ref item)).Recast100ms == 0)
		{
			return;
		}
		ActionManager* ptr = ActionManager.Instance();
		if (ptr == null || !((ActionManager)ptr).IsActionOffCooldown((ActionType)1, 7562u))
		{
			return;
		}
		int recastGroup = ((ActionManager)ptr).GetRecastGroup(1, actionID);
		RecastDetail* ptr2 = ((recastGroup == -1) ? null : ((ActionManager)ptr).GetRecastGroupDetail(recastGroup));
		int additionalRecastGroup = ((ActionManager)ptr).GetAdditionalRecastGroup((ActionType)1, actionID);
		RecastDetail* ptr3 = ((additionalRecastGroup == -1) ? null : ((ActionManager)ptr).GetRecastGroupDetail(additionalRecastGroup));
		if (ptr2 != null)
		{
			if (!((RecastDetail)ptr2).IsActive || (((RecastDetail)ptr2).Total - ((RecastDetail)ptr2).Elapsed) * 1000f < 500f)
			{
				return;
			}
		}
		else if (ptr3 != null && (!((RecastDetail)ptr3).IsActive || (((RecastDetail)ptr3).Total - ((RecastDetail)ptr3).Elapsed) * 1000f < 500f))
		{
			return;
		}
		if (((ActionManager)ptr).QueuedActionId == 0 || ((int)((ActionManager)ptr).QueuedActionType == 1 && LuminaGetter.TryGetRow<Action>(((ActionManager)ptr).QueuedActionId, out Action item2) && ((Action)(ref item2)).Recast100ms != 0))
		{
			EnqueueUseLucidDreaming();
		}
	}

	private void OnUseAction(bool result, ActionType actionType, uint actionID, ulong targetID, Vector3 location, uint extraParam, byte a7)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Invalid comparison between Unknown and I4
		if (result && !GameState.IsInPVPArea && (!config.OnlyInDuty || GameState.ContentFinderCondition != 0) && ValidClassJobs.Contains(LocalPlayerState.ClassJob) && ((int)actionType != 1 || !LuminaGetter.TryGetRow<Action>(actionID, out Action item) || ((Action)(ref item)).Recast100ms == 0))
		{
			base.TaskHelper.Abort();
		}
	}

	private unsafe void EnqueueUseLucidDreaming()
	{
		base.TaskHelper.Abort();
		ActionManager* manager = ActionManager.Instance();
		if (manager == null)
		{
			return;
		}
		base.TaskHelper.DelayNext((int)MathF.Max(100f, ((ActionManager)manager).AnimationLock * 1000f), "等待动画锁结束");
		base.TaskHelper.Enqueue(delegate
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Invalid comparison between Unknown and I4
			if (((ActionManager)manager).QueuedActionId != 0)
			{
				Action item;
				if ((int)((ActionManager)manager).QueuedActionType != 1)
				{
					base.TaskHelper.Abort();
				}
				else if (!LuminaGetter.TryGetRow<Action>(((ActionManager)manager).QueuedActionId, out item) || ((Action)(ref item)).Recast100ms == 0)
				{
					base.TaskHelper.Abort();
				}
			}
		}, "检查当前状态是否合法");
		base.TaskHelper.Enqueue(delegate
		{
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			if (!OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)1, 7562u, 3758096384uL, 0u, (UseActionMode)0))
			{
				return false;
			}
			if (Throttler.Shared.Throttle("AutoLucidDreaming-SendChat", 10000u))
			{
				OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoLucidDreaming-Notification", new Dictionary<string, object>
				{
					["currentMP"] = LocalPlayerState.Object?.CurrentMp ?? 0,
					["mpThreshold"] = config.MPThreshold,
					["lucidDreaming"] = LuminaWrapper.GetActionName(7562u)
				})));
			}
			return true;
		}, "使用醒梦");
	}
}
