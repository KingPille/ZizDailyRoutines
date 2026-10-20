using System.Collections.Frozen;
using System.Collections.Generic;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using InteropGenerator.Runtime;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.ModulesPublic.Duty;

public class AutoCheckItemLevel : ModuleBase
{
	private static readonly FrozenSet<uint> ValidContentJobCategories = FrozenSet.Create<uint>(108u, 142u, 146u);

	private static readonly FrozenSet<uint> HaveOffHandJobCategories = FrozenSet.Create<uint>(2u, 7u, 8u, 20u);

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoCheckItemLevelTitle"),
		Description = LanguageManager.Get("AutoCheckItemLevelDescription"),
		Category = ModuleCategory.Duty
	};

	protected override void Init()
	{
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 20000
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += OnZoneChanged;
	}

	protected override void Uninit()
	{
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChanged;
	}

	private void OnZoneChanged(uint u)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		base.TaskHelper.Abort();
		if (GameState.IsInPVPArea || GameState.ContentFinderCondition == 0)
		{
			return;
		}
		ContentFinderCondition contentFinderConditionData = GameState.ContentFinderConditionData;
		if (((ContentFinderCondition)(ref contentFinderConditionData)).PvP)
		{
			return;
		}
		FrozenSet<uint> validContentJobCategories = ValidContentJobCategories;
		contentFinderConditionData = GameState.ContentFinderConditionData;
		if (!validContentJobCategories.Contains(((ContentFinderCondition)(ref contentFinderConditionData)).AcceptClassJobCategory.RowId))
		{
			return;
		}
		contentFinderConditionData = GameState.ContentFinderConditionData;
		ContentMemberType value = ((ContentFinderCondition)(ref contentFinderConditionData)).ContentMemberType.Value;
		if (((ContentMemberType)(ref value)).MeleesPerParty != 0 && !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)86])
		{
			base.TaskHelper.Enqueue(() => !IConditionExtension.get_IsBetweenAreas(DalamudServiceExtension.Instance<ICondition>()) && IOmenDalamudService<IObjectTable>.Instance().LocalPlayer != null, "WaitForEnteringDuty");
			base.TaskHelper.Enqueue(() => CheckMembersItemLevel(new HashSet<ulong> { LocalPlayerState.EntityID }));
		}
	}

	private unsafe bool CheckMembersItemLevel(HashSet<ulong> checkedMembers)
	{
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		AgentHUD* ptr = AgentHUD.Instance();
		AgentInspect* agentInspect = AgentInspect.Instance();
		if (ptr == null || agentInspect == null || ((AgentHUD)ptr).PartyMemberCount <= 1)
		{
			base.TaskHelper.Abort();
			return true;
		}
		if (IConditionExtension.get_IsBetweenAreas(DalamudServiceExtension.Instance<ICondition>()))
		{
			return false;
		}
		if (Addons.CharacterInspect != null)
		{
			((AtkUnitBase)Addons.CharacterInspect).Close(true);
			return false;
		}
		HudPartyMember[] array = ((AgentHUD)ptr).PartyMembers.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			HudPartyMember member = array[i];
			if (member.EntityId == 0 || member.ContentId == 0L || member.EntityId == LocalPlayerState.EntityID || !checkedMembers.Add(member.EntityId))
			{
				continue;
			}
			base.TaskHelper.Enqueue(delegate
			{
				if (Addons.CharacterInspect != null && ((AgentInspect)agentInspect).CurrentEntityId == member.EntityId)
				{
					return true;
				}
				if (Throttler.Shared.Throttle("AutoCheckItemLevel-OpenExamine"))
				{
					if (Addons.CharacterInspect != null)
					{
						((AtkUnitBase)Addons.CharacterInspect).Close(true);
						Throttler.Shared.Throttle("AutoCheckItemLevel-OpenExamine", 10u, reThrottle: true);
					}
					else
					{
						((AgentInspect)agentInspect).ExamineCharacter(member.EntityId, false);
					}
				}
				return false;
			}, "打开检视界面");
			base.TaskHelper.Enqueue(delegate
			{
				//IL_0043: Unknown result type (might be due to invalid IL or missing references)
				//IL_0049: Unknown result type (might be due to invalid IL or missing references)
				//IL_0070: Unknown result type (might be due to invalid IL or missing references)
				//IL_0111: Unknown result type (might be due to invalid IL or missing references)
				//IL_0099: Unknown result type (might be due to invalid IL or missing references)
				//IL_009e: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
				if (member.Object == null)
				{
					return false;
				}
				if (!((InventoryType)2009).TryGetItems((InventoryItem _) => true, out List<InventoryItem> itemResult))
				{
					return false;
				}
				while (itemResult.Count < 13)
				{
					itemResult.Add(default(InventoryItem));
				}
				uint num2 = 0u;
				uint num3 = 9999u;
				int num4 = 11;
				for (int num5 = 0; num5 < 13; num5++)
				{
					if (LuminaGetter.TryGetRow<Item>(itemResult[num5].ItemId, out Item item))
					{
						int num6 = num5;
						if (num6 != 0)
						{
							if (num6 != 1)
							{
								if (num6 == 5)
								{
									continue;
								}
							}
							else if (num4 != 12)
							{
								continue;
							}
						}
						else
						{
							uint rowId = ((Item)(ref item)).ClassJobCategory.RowId;
							if (HaveOffHandJobCategories.Contains(rowId))
							{
								num4++;
							}
						}
						if (((Item)(ref item)).LevelItem.RowId < num3)
						{
							num3 = ((Item)(ref item)).LevelItem.RowId;
						}
						num2 += ((Item)(ref item)).LevelItem.RowId;
					}
				}
				uint avgIL = (uint)(num2 / num4);
				SendNotification(member, avgIL, num3);
				((AtkUnitBase)Addons.CharacterInspect).Close(true);
				((AgentInspect)agentInspect).FetchCharacterDataStatus = 0u;
				((AgentInspect)agentInspect).FetchSearchCommentStatus = 0u;
				((AgentInspect)agentInspect).FetchCharacterDataStatus = 0u;
				return true;
			}, "检查装等");
			int num = checkedMembers.Count - 1;
			if (num != 0 && num % 3 == 0)
			{
				base.TaskHelper.DelayNext(1000, "等待 1 秒");
			}
			base.TaskHelper.Enqueue(() => CheckMembersItemLevel(checkedMembers), "进入新循环");
			return true;
		}
		base.TaskHelper.Abort();
		return true;
	}

	private unsafe static void SendNotification(HudPartyMember partyMember, uint avgIL, uint lowIL)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Expected O, but got Unknown
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Expected O, but got Unknown
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Expected O, but got Unknown
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		if (partyMember.Object != null)
		{
			ContentFinderCondition contentFinderConditionData = GameState.ContentFinderConditionData;
			if (((ContentFinderCondition)(ref contentFinderConditionData)).RowId != 0)
			{
				SeStringBuilder val = new SeStringBuilder();
				val.AddUiForeground((ushort)25).Add((Payload)new PlayerPayload(((object)(*(CStringPointer*)(&partyMember.Name))/*cast due to constrained. prefix*/).ToString(), (uint)((BattleChara)partyMember.Object).HomeWorld)).AddUiForegroundOff();
				val.Append(" (" + LuminaWrapper.GetJobName(((BattleChara)partyMember.Object).ClassJob) + ")");
				byte level = ((BattleChara)partyMember.Object).Level;
				val.Append(" " + LanguageManager.Get("Level") + ": ").AddUiForeground(level.ToString(), (ushort)((level >= ((ContentFinderCondition)(ref contentFinderConditionData)).ClassJobLevelSync) ? 43u : 17u));
				val.Add((Payload)new NewLinePayload());
				val.Append(" " + LanguageManager.Get("ILAverage") + ": ").AddUiForeground(avgIL.ToString(), (ushort)((avgIL > ((ContentFinderCondition)(ref contentFinderConditionData)).ItemLevelSync) ? 43u : 17u));
				val.Append(" " + LanguageManager.Get("ILMinimum") + ": ").AddUiForeground(lowIL.ToString(), (ushort)((lowIL > ((ContentFinderCondition)(ref contentFinderConditionData)).ItemLevelRequired) ? 43u : 17u));
				val.Add((Payload)new NewLinePayload());
				OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(val.Build().Encode()));
			}
		}
	}
}
