using System;
using System.Numerics;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Arrays;
using FFXIVClientStructs.FFXIV.Common.Math;
using Lumina.Excel.Sheets;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class AutoRedirectActionTarget : ModuleBase
{
	private class Config : ModuleConfig
	{
		public bool TargetEnemyAction = true;

		public bool TargetMemberAction;
	}

	private Config config;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoRedirectActionTargetTitle"),
		Description = LanguageManager.Get("AutoRedirectActionTargetDescription"),
		Category = ModuleCategory.Action
	};

	protected override void Init()
	{
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		OmenServiceBase<UseActionManager>.Instance().RegPreUseActionLocation(OnPreUseAction);
	}

	protected override void Uninit()
	{
		OmenServiceBase<UseActionManager>.Instance().Unreg(OnPreUseAction);
	}

	protected override void ConfigUI()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("AutoRedirectActionTarget-RedirectEnemyAction")), ref config.TargetEnemyAction))
		{
			config.Save(this);
		}
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("AutoRedirectActionTarget-RedirectMemberAction")), ref config.TargetMemberAction))
		{
			config.Save(this);
		}
	}

	private unsafe void OnPreUseAction(ref bool isPrevented, ref ActionType type, ref uint actionID, ref ulong targetID, ref Vector3 location, ref uint extraParam, ref byte a7)
	{
		if ((int)type != 1 || !LuminaGetter.TryGetRow<Action>(actionID, out Action item) || ((Action)(ref item)).TargetArea)
		{
			return;
		}
		if (((Action)(ref item)).CanTargetHostile)
		{
			if (!config.TargetEnemyAction)
			{
				return;
			}
		}
		else if (!config.TargetMemberAction)
		{
			return;
		}
		BattleChara* ptr = ((targetID == 3758096384u) ? null : ((CharacterManager)CharacterManager.Instance()).LookupBattleCharaByEntityId((uint)targetID));
		if (ptr != null && ActionManager.CanUseActionOnTarget(actionID, (GameObject*)ptr))
		{
			return;
		}
		BattleChara* availableTarget = GetAvailableTarget(actionID, ((Action)(ref item)).CanTargetHostile);
		if (availableTarget != null)
		{
			targetID = ((BattleChara)availableTarget).EntityId;
			if (((TargetSystem)TargetSystem.Instance()).Target == null)
			{
				((TargetSystem)TargetSystem.Instance()).SetHardTarget((GameObject*)availableTarget, false, false, 0);
			}
		}
	}

	private unsafe static BattleChara* GetAvailableTarget(uint actionID, bool isTargetEnemy)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = LocalPlayerState.Object.Position;
		float num = MathF.Pow(ActionManager.GetActionRange(actionID), 2f);
		GameObject* previousTarget = ((TargetSystem)TargetSystem.Instance()).PreviousTarget;
		if (previousTarget != null && ActionManager.CanUseActionOnTarget(actionID, previousTarget) && Vector3.DistanceSquared(position, Vector3.op_Implicit(((GameObject)previousTarget).Position)) <= num)
		{
			return (BattleChara*)previousTarget;
		}
		if (isTargetEnemy)
		{
			EnemyListNumberArray* ptr = EnemyListNumberArray.Instance();
			if (((EnemyListNumberArray)ptr).EnemyCount == 0)
			{
				return null;
			}
			for (int i = 0; i < ((EnemyListNumberArray)ptr).EnemyCount; i++)
			{
				EnemyListEnemyNumberArray val = ((EnemyListNumberArray)ptr).Enemies[i];
				uint entityId = (uint)val.EntityId;
				bool flag = ((entityId == 0 || entityId == 3758096384u) ? true : false);
				if (!flag && val.ActiveInList && val.RemainingHPPercent != 0)
				{
					BattleChara* ptr2 = ((CharacterManager)CharacterManager.Instance()).LookupBattleCharaByEntityId((uint)val.EntityId);
					if (ptr2 != null && ActionManager.CanUseActionOnTarget(actionID, (GameObject*)ptr2) && Vector3.DistanceSquared(position, Vector3.op_Implicit(((BattleChara)ptr2).Position)) <= num)
					{
						return ptr2;
					}
				}
			}
		}
		else
		{
			AgentHUD* ptr3 = AgentHUD.Instance();
			if (((AgentHUD)ptr3).PartyMemberCount > 1)
			{
				Span<HudPartyMember> partyMembers = ((AgentHUD)ptr3).PartyMembers;
				for (int j = 0; j < partyMembers.Length; j++)
				{
					HudPartyMember val2 = partyMembers[j];
					if (val2.ContentId != 0L && val2.Object != null && ActionManager.CanUseActionOnTarget(actionID, (GameObject*)val2.Object) && Vector3.DistanceSquared(position, Vector3.op_Implicit(((BattleChara)val2.Object).Position)) <= num)
					{
						return val2.Object;
					}
				}
			}
			for (int k = 0; k < 100; k++)
			{
				BattleChara* value = ((CharacterManager)CharacterManager.Instance()).BattleCharas[k].Value;
				if (value != null && ActionManager.CanUseActionOnTarget(actionID, (GameObject*)value) && Vector3.DistanceSquared(position, Vector3.op_Implicit(((BattleChara)value).Position)) <= num)
				{
					return value;
				}
			}
		}
		return null;
	}
}
