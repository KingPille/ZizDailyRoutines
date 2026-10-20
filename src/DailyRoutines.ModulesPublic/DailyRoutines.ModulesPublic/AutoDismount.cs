using System;
using System.Collections.Frozen;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using Lumina.Excel.Sheets;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.ModulesPublic;

public class AutoDismount : ModuleBase
{
	private static readonly FrozenSet<ActionType> MustDismountActionTypes = FrozenSet.Create(RuntimeHelpers.CreateSpan<ActionType>((RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/));

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoDismountTitle"),
		Description = LanguageManager.Get("AutoDismountDescription"),
		Category = ModuleCategory.System
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected override void Init()
	{
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 1500
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		OmenServiceBase<UseActionManager>.Instance().RegPostUseAction(OnUseAction);
	}

	protected override void Uninit()
	{
		OmenServiceBase<UseActionManager>.Instance().Unreg(OnUseAction);
	}

	private unsafe void OnUseAction(bool result, ActionType actionType, uint actionID, ulong targetID, uint extraParam, UseActionMode queueState, uint comboRouteID)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if (!IConditionExtension.get_IsOnMount(DalamudServiceExtension.Instance<ICondition>()))
		{
			return;
		}
		uint adjustedActionId = ((ActionManager)ActionManager.Instance()).GetAdjustedActionId(actionID);
		if (IsNeedToDismount(actionType, adjustedActionId, targetID))
		{
			base.TaskHelper.Abort();
			OmenServiceBase<MovementManager>.Instance().Dismount();
			base.TaskHelper.Enqueue(() => !OmenServiceBase<MovementManager>.Instance().IsManagerBusy && !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4] && OmenServiceBase<UseActionManager>.Instance().UseAction(actionType, actionID, targetID, extraParam, queueState, comboRouteID));
		}
	}

	private unsafe static bool IsNeedToDismount(ActionType actionType, uint actionID, ulong actionTargetID)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Invalid comparison between Unknown and I4
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
		if (localPlayer == null)
		{
			return false;
		}
		if (!LuminaGetter.TryGetRow<Action>(actionID, out Action item))
		{
			return false;
		}
		ActionManager* ptr = ActionManager.Instance();
		if (ptr == null)
		{
			return false;
		}
		if ((int)actionType == 13)
		{
			return false;
		}
		if (((ActionManager)ptr).GetActionStatus(actionType, actionID, actionTargetID, false, false, (uint*)null) == 0)
		{
			return false;
		}
		if (MustDismountActionTypes.Contains(actionType))
		{
			return true;
		}
		if (!((ActionManager)ptr).IsActionOffCooldown(actionType, actionID))
		{
			return false;
		}
		if ((((Action)(ref item)).CanTargetSelf || ((Action)(ref item)).TargetArea) ? true : false)
		{
			return true;
		}
		IGameObject gameObject = IOmenDalamudService<IObjectTable>.Instance().SearchByID(actionTargetID);
		if (((Action)(ref item)).Range != 0)
		{
			if (actionTargetID != 3758096384u && gameObject != null)
			{
				uint actionInRangeOrLoS = ActionManager.GetActionInRangeOrLoS(actionID, (GameObject*)localPlayer.ToStruct(), gameObject.ToStruct());
				if ((actionInRangeOrLoS == 562 || actionInRangeOrLoS == 566) ? true : false)
				{
					return false;
				}
				if (!ActionManager.CanUseActionOnTarget(actionID, gameObject.ToStruct()))
				{
					return false;
				}
			}
			else if (TargetManager.Target == null)
			{
				return false;
			}
		}
		return true;
	}
}
