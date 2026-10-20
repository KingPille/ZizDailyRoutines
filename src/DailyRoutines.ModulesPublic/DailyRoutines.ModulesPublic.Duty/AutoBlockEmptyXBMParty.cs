using System;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Models.Native;
using OmenTools.OmenService;

namespace DailyRoutines.ModulesPublic.Duty;

public class AutoBlockEmptyXBMParty : ModuleBase
{
	private Hook<AgentReceiveEventDelegate>? AgentReceiveEventHook;

	private const int STAGE_MODE_OFFSET = 60;

	private const int PARTY_LIST_BEGIN_OFFSET = 120;

	private const int PARTY_LIST_END_OFFSET = 128;

	private const int FLUTE_LIST_BEGIN_OFFSET = 144;

	private const int FLUTE_LIST_END_OFFSET = 152;

	private const int PET_CURRENT_HEALTH_OFFSET = 192;

	private const int PET_CURRENT_HEALTH_COUNT = 15;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoBlockEmptyXBMPartyTitle"),
		Description = LanguageManager.Get("AutoBlockEmptyXBMPartyDescription"),
		Category = ModuleCategory.Duty
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected unsafe override void Init()
	{
		AgentInterface* agentByInternalId = ((AgentModule)AgentModule.Instance()).GetAgentByInternalId((AgentId)502);
		AgentReceiveEventHook = (*((AgentInterface)agentByInternalId).VirtualTable).HookVFuncFromName<AgentInterfaceVirtualTable, AgentReceiveEventDelegate>("ReceiveEvent", (AgentReceiveEventDelegate)AgentReceiveEventDetour);
		AgentReceiveEventHook.Enable();
	}

	private unsafe AtkValue* AgentReceiveEventDetour(AgentInterface* agent, AtkValue* returnValues, AtkValue* values, uint valueCount, ulong eventKind)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Invalid comparison between Unknown and I4
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		AutoBlockEmptyXBMParty autoBlockEmptyXBMParty = this;
		AgentInterface* agent2 = agent;
		AtkValue* returnValues2 = returnValues;
		AtkValue* values2 = values;
		uint valueCount2 = valueCount;
		ulong eventKind2 = eventKind;
		if (eventKind2 != 0L || valueCount2 != 1)
		{
			return InvokeOriginal();
		}
		AtkValue val = *values2;
		if ((int)val.Type != 3 || val.Int != 8)
		{
			return InvokeOriginal();
		}
		AgentInterface* agentByInternalId = ((AgentModule)AgentModule.Instance()).GetAgentByInternalId((AgentId)501);
		if (agentByInternalId == null)
		{
			return InvokeOriginal();
		}
		nint agent3 = (nint)agentByInternalId;
		uint num = ((uint*)agent2)[15];
		string text;
		if (num != 0)
		{
			if (num != 2 || !IsFluteAssignmentInsufficient(agent3))
			{
				goto IL_00d6;
			}
			text = "AutoBlockEmptyXBMParty-Notification-InsufficiantAssignment";
		}
		else
		{
			if (GetElementCount(agent3, 120, 128) >= 3)
			{
				goto IL_00d6;
			}
			text = "AutoBlockEmptyXBMParty-Notification-EmptyParty";
		}
		goto IL_00d9;
		IL_00d6:
		text = null;
		goto IL_00d9;
		IL_00d9:
		string text2 = text;
		if (text2 == null)
		{
			return InvokeOriginal();
		}
		NotifyHelper.ToastError(LanguageManager.Get(text2));
		((AtkValue)returnValues2).SetBool(false);
		return returnValues2;
		unsafe AtkValue* InvokeOriginal()
		{
			return AgentReceiveEventHook.Original(agent2, returnValues2, values2, valueCount2, eventKind2);
		}
	}

	private unsafe static long GetElementCount(nint agent, int beginOffset, int endOffset)
	{
		return (nint)(*(IntPtr*)(agent + endOffset)) - (nint)(*(IntPtr*)(agent + beginOffset)) >> 3;
	}

	private unsafe static bool IsFluteAssignmentInsufficient(nint agent)
	{
		long elementCount = GetElementCount(agent, 144, 152);
		if (elementCount >= 3)
		{
			return false;
		}
		int num = 0;
		for (int i = 0; i < 15; i++)
		{
			if (*(uint*)(agent + 192 + i * 4) != 0)
			{
				num++;
			}
		}
		return elementCount < num;
	}
}
