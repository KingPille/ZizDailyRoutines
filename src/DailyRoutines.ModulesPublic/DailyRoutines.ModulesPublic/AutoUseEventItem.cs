using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Application.Network.WorkDefinitions;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using Lumina.Excel.Sheets;
using OmenTools;
using OmenTools.Dalamud.Attributes;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class AutoUseEventItem : ModuleBase
{
	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static AddonEventDelegate _003C0_003E__OnAddon;

		public static LogMessageManager.PreLogMessageDelegate _003C1_003E__OnPreReceiveMessage;

		public static Action _003C2_003E__OnAddonInventoryEvent;
	}

	private static readonly FrozenSet<string> InventoryEventAddons;

	private static readonly FrozenDictionary<uint, HashSet<uint>> QuestRowIDToEventItems;

	private static readonly FrozenSet<uint> InvalidLogMessageID;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoUseEventItemTitle"),
		Description = LanguageManager.Get("AutoUseEventItemDescription"),
		Category = ModuleCategory.System
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		NeedAuth = true
	};

	protected override void Init()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		IAddonLifecycle obj = DalamudServiceExtension.Instance<IAddonLifecycle>();
		FrozenSet<string> inventoryEventAddons = InventoryEventAddons;
		object obj2 = _003C_003EO._003C0_003E__OnAddon;
		if (obj2 == null)
		{
			AddonEventDelegate val = OnAddon;
			_003C_003EO._003C0_003E__OnAddon = val;
			obj2 = (object)val;
		}
		obj.RegisterListener((AddonEvent)17, (IEnumerable<string>)inventoryEventAddons, (AddonEventDelegate)obj2);
		OmenServiceBase<LogMessageManager>.Instance().RegPre(OnPreReceiveMessage);
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
		OmenServiceBase<LogMessageManager>.Instance().Unreg(OnPreReceiveMessage);
	}

	private static void OnAddon(AddonEvent type, AddonArgs args)
	{
		OnAddonInventoryEvent();
	}

	private static void OnPreReceiveMessage(ref bool isPrevented, ref uint logMessageID, ref LogMessageQueueItem values)
	{
		if (InvalidLogMessageID.Contains(logMessageID))
		{
			isPrevented = true;
		}
		if (logMessageID == 579 && values.Parameters.Count > 0 && LuminaGetter.TryGetRow<EventItem>((uint)values.Parameters[0].IntValue, out EventItem _))
		{
			OnAddonInventoryEvent();
		}
	}

	private unsafe static void OnAddonInventoryEvent()
	{
		if (!((UIModule)UIModule.Instance()).IsInventoryOpen() || IConditionExtension.get_IsCasting(DalamudServiceExtension.Instance<ICondition>()) || DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)26] || Addons.Request != null || !IsAnyQuestNearby(out var questRowID))
		{
			return;
		}
		IGameObject gameObj;
		if (TargetManager.Target != null)
		{
			gameObj = TargetManager.Target;
		}
		else
		{
			IsAnyMTQNearby(out gameObj);
		}
		if (!QuestRowIDToEventItems.TryGetValue(questRowID, out HashSet<uint> value))
		{
			return;
		}
		if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)32])
		{
			DalamudServiceExtension.Instance<IFramework>().RunOnTick((Action)OnAddonInventoryEvent, default(TimeSpan), 0, default(CancellationToken));
			return;
		}
		foreach (uint item in FilterEItemsByInventory(value))
		{
			if (IConditionExtension.get_IsCasting(DalamudServiceExtension.Instance<ICondition>()))
			{
				break;
			}
			OmenServiceBase<UseActionManager>.Instance().UseActionLocation((ActionType)3, item, gameObj.GameObjectID, gameObj.Position, 0u, 0);
		}
	}

	private unsafe static bool IsAnyQuestNearby(out uint questRowID)
	{
		questRowID = 0u;
		IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
		if (localPlayer == null)
		{
			return false;
		}
		HashSet<string> validMarkers = (from marker in ((AgentHUD)AgentHUD.Instance()).MapMarkers.AsSpan().ToArray().Where(delegate(MapMarkerData marker)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				//IL_003b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0047: Unknown result type (might be due to invalid IL or missing references)
				//IL_0055: Unknown result type (might be due to invalid IL or missing references)
				if (marker.TooltipString == null || (nint)marker.TooltipString <= unchecked((nint)null))
				{
					return false;
				}
				if (string.IsNullOrWhiteSpace(((object)(*(Utf8String*)marker.TooltipString)/*cast due to constrained. prefix*/).ToString()))
				{
					return false;
				}
				float num = Vector3.Distance(localPlayer.Position, marker.Position);
				return (!(marker.Radius <= 1f)) ? (num <= marker.Radius) : (num <= 5f);
			})
			select ((object)(*(Utf8String*)marker.TooltipString)/*cast due to constrained. prefix*/).ToString()).ToHashSet();
		(uint, string) tuple = (from quest in ((QuestManager)QuestManager.Instance()).NormalQuests.ToArray()
			where quest.QuestId != 0 && !((QuestWork)(ref quest)).IsHidden
			select quest).Select(delegate(QuestWork quest)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			int num = quest.QuestId + 65536;
			Quest? row = LuminaGetter.GetRow<Quest>((uint)num);
			object obj;
			if (!row.HasValue)
			{
				obj = null;
			}
			else
			{
				Quest valueOrDefault = row.GetValueOrDefault();
				obj = SeStringExtensions.ToDalamudString(((Quest)(ref valueOrDefault)).Name).TextValue;
			}
			if (obj == null)
			{
				obj = string.Empty;
			}
			string item = (string)obj;
			return (rowID: (uint)num, questName: item);
		}).FirstOrDefault(((uint rowID, string questName) quest) => validMarkers.Contains(quest.questName));
		(uint, string) tuple2 = tuple;
		if (tuple2.Item1 != 0 || tuple2.Item2 != null)
		{
			(questRowID, _) = tuple;
			return true;
		}
		return false;
	}

	private static bool IsAnyMTQNearby(out IGameObject gameObj)
	{
		IGameObject gameObject = IOmenDalamudService<IObjectTable>.Instance().SearchObject((IGameObject obj) => obj.IsMTQ());
		gameObj = gameObject;
		return gameObj != null;
	}

	private unsafe static List<uint> FilterEItemsByInventory(HashSet<uint> validEItems)
	{
		InventoryContainer* inventoryContainer = ((InventoryManager)InventoryManager.Instance()).GetInventoryContainer((InventoryType)2004);
		List<uint> list = new List<uint>();
		if (inventoryContainer == null || !((InventoryContainer)inventoryContainer).IsLoaded)
		{
			return list;
		}
		for (int i = 0; i < ((InventoryContainer)inventoryContainer).Size; i++)
		{
			InventoryItem* inventorySlot = ((InventoryContainer)inventoryContainer).GetInventorySlot(i);
			if (inventorySlot != null && ((InventoryItem)inventorySlot).ItemId != 0 && validEItems.Contains(((InventoryItem)inventorySlot).ItemId))
			{
				list.Add(((InventoryItem)inventorySlot).ItemId);
			}
		}
		return list;
	}

	[IPCProvider("DailyRoutines.Modules.AutoUseEventItem.UseEventItem")]
	private void UseEventItem()
	{
		OnAddonInventoryEvent();
	}

	static AutoUseEventItem()
	{
		InlineArray3<string> buffer = default(InlineArray3<string>);
		buffer[0] = "InventoryEventGrid";
		buffer[1] = "InventoryEventGrid0";
		buffer[2] = "InventoryEventGrid0E";
		InventoryEventAddons = FrozenSet.Create<string>(buffer);
		QuestRowIDToEventItems = (from item in (IEnumerable<EventItem>)LuminaGetter.Get<EventItem>()
			where ((EventItem)(ref item)).Quest.ValueNullable.HasValue && ((EventItem)(ref item)).Action.ValueNullable.HasValue
			group item by ((EventItem)(ref item)).Quest.RowId).ToFrozenDictionary((IGrouping<uint, EventItem> group) => group.Key, (IGrouping<uint, EventItem> group) => group.Select((EventItem item) => ((EventItem)(ref item)).RowId).ToHashSet());
		InvalidLogMessageID = FrozenSet.Create<uint>(7732u, 563u);
	}
}
