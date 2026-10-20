using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Threading;

namespace DailyRoutines.ModulesPublic.Interface;

public class AutoDisplayMSQProgress : ModuleBase
{
	private readonly struct MSQProgressResult(int remaining, float percentComplete, uint firstIncompleteQuest)
	{
		public readonly int Remaining = remaining;

		public readonly float PercentComplete = percentComplete;

		public readonly uint FirstIncompleteQuest = firstIncompleteQuest;
	}

	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static AddonEventDelegate _003C0_003E__OnAddon;
	}

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoDisplayMSQProgressTitle"),
		Description = LanguageManager.Get("AutoDisplayMSQProgressDescription"),
		Category = ModuleCategory.Interface
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected unsafe override void Init()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		IAddonLifecycle obj = DalamudServiceExtension.Instance<IAddonLifecycle>();
		object obj2 = _003C_003EO._003C0_003E__OnAddon;
		if (obj2 == null)
		{
			AddonEventDelegate val = OnAddon;
			_003C_003EO._003C0_003E__OnAddon = val;
			obj2 = (object)val;
		}
		obj.RegisterListener((AddonEvent)5, "ScenarioTree", (AddonEventDelegate)obj2);
		if ((*Addons.ScenarioTreeAddon).IsAddonAndNodesReady())
		{
			OnAddon((AddonEvent)1, null);
		}
	}

	protected override void Uninit()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		IAddonLifecycle obj = DalamudServiceExtension.Instance<IAddonLifecycle>();
		AddonEventDelegate[] array = new AddonEventDelegate[1];
		object obj2 = _003C_003EO._003C0_003E__OnAddon;
		if (obj2 == null)
		{
			AddonEventDelegate val = OnAddon;
			_003C_003EO._003C0_003E__OnAddon = val;
			obj2 = (object)val;
		}
		array[0] = (AddonEventDelegate)obj2;
		obj.UnregisterListener((AddonEventDelegate[])(object)array);
	}

	private unsafe static void OnAddon(AddonEvent type, AddonArgs args)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		if (!Throttler.Shared.Throttle("ScenarioTree", 1000u))
		{
			return;
		}
		AtkUnitBase* scenarioTreeAddon = Addons.ScenarioTreeAddon;
		if (!(*scenarioTreeAddon).IsAddonAndNodesReady() || !TryGetCurrentExpansionMSQProgress(out var result) || result.Remaining == 0 || !LuminaGetter.TryGetRow<Quest>(result.FirstIncompleteQuest, out Quest item))
		{
			return;
		}
		string text = $"{((object)((Quest)(ref item)).Name/*cast due to constrained. prefix*/).ToString()} ({result.Remaining} / {result.PercentComplete:F1}%)";
		((AtkValue)((byte*)((AtkUnitBase)scenarioTreeAddon).AtkValues + (nint)7 * (nint)Unsafe.SizeOf<AtkValue>())).SetManagedString(text);
		((AtkUnitBase)scenarioTreeAddon).OnRefresh((uint)((AtkUnitBase)scenarioTreeAddon).AtkValuesCount, ((AtkUnitBase)scenarioTreeAddon).AtkValues);
		AtkComponentButton* componentButtonById = ((AtkUnitBase)scenarioTreeAddon).GetComponentButtonById(13u);
		if (componentButtonById != null)
		{
			AtkTextNode* ptr = (AtkTextNode*)((AtkUldManager)(&((AtkComponentButton)componentButtonById).UldManager)).SearchNodeById(6u);
			if (ptr != null)
			{
				((AtkTextNode)ptr).SetText(text);
			}
		}
	}

	private unsafe static bool TryGetCurrentExpansionMSQProgress(out MSQProgressResult result)
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		UIState* ptr = UIState.Instance();
		List<Quest> source = ((IEnumerable<Quest>)LuminaGetter.Get<Quest>()).Where(delegate(Quest x)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			JournalGenre value = ((Quest)(ref x)).JournalGenre.Value;
			return ((JournalGenre)(ref value)).Icon == 61412 && !string.IsNullOrEmpty(((object)((Quest)(ref x)).Name/*cast due to constrained. prefix*/).ToString());
		}).ToList();
		uint num = (uint)(((AgentScenarioTreeData)((AgentScenarioTree)AgentScenarioTree.Instance()).Data).MainScenarioQuestIds[0] + 65536);
		Quest firstIncompleteData = LuminaGetter.GetRowOrDefault<Quest>(num);
		List<Quest> list = (from x in source
			where ((Quest)(ref x)).JournalGenre.RowId == ((Quest)(ref firstIncompleteData)).JournalGenre.RowId
			orderby ((Quest)(ref x)).RowId
			select x).ToList();
		int num2 = 0;
		foreach (Quest item in list)
		{
			Quest current = item;
			if (((UIState)ptr).IsUnlockLinkUnlockedOrQuestCompleted(((Quest)(ref current)).RowId, ((IEnumerable<TodoParamsStruct>)(object)((Quest)(ref current)).TodoParams).Max((TodoParamsStruct x) => ((TodoParamsStruct)(ref x)).ToDoCompleteSeq), true))
			{
				num2++;
			}
		}
		int num3 = ((((Quest)(ref firstIncompleteData)).JournalGenre.RowId == 1) ? AdjustARRTotalCount(list.Count) : list.Count);
		int remaining = num3 - num2;
		float percentComplete = ((num3 > 0) ? ((float)num2 * 100f / (float)num3) : 100f);
		result = new MSQProgressResult(remaining, percentComplete, num);
		return true;
	}

	private unsafe static int AdjustARRTotalCount(int baseCount)
	{
		int num = baseCount;
		PlayerState* intPtr = PlayerState.Instance();
		if (((PlayerState)intPtr).StartTown != 1)
		{
			num -= 23;
		}
		if (((PlayerState)intPtr).StartTown != 2)
		{
			num -= 23;
		}
		if (((PlayerState)intPtr).StartTown != 3)
		{
			num -= 24;
		}
		return num - 8;
	}
}
