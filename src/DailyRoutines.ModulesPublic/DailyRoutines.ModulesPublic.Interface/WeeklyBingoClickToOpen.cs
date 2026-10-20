using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.Addon.Events;
using Dalamud.Game.Addon.Events.EventDataTypes;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic.Interface;

public class WeeklyBingoClickToOpen : ModuleBase
{
	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static AddonEventDelegate _003C0_003E__OnDutySlotClick;
	}

	private readonly IAddonEventHandle?[] eventHandles = (IAddonEventHandle?[])(object)new IAddonEventHandle[16];

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("WeeklyBingoClickToOpenTitle"),
		Description = LanguageManager.Get("WeeklyBingoClickToOpenDescription"),
		Category = ModuleCategory.Interface,
		Author = new string[1] { "Due" }
	};

	protected unsafe override void Init()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)1, "WeeklyBingo", new AddonEventDelegate(OnAddon));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, "WeeklyBingo", new AddonEventDelegate(OnAddon));
		if ((*Addons.WeeklyBingo).IsAddonAndNodesReady())
		{
			OnAddon((AddonEvent)1, null);
		}
	}

	protected override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddon });
		OnAddon((AddonEvent)6, null);
	}

	private unsafe void OnAddon(AddonEvent type, AddonArgs? args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Expected O, but got Unknown
		if ((int)type != 1)
		{
			if ((int)type != 6)
			{
				return;
			}
			{
				foreach (int item in Enumerable.Range(0, 16))
				{
					IAddonEventHandle val = eventHandles[item];
					if (val != null)
					{
						DalamudServiceExtension.Instance<IAddonEventManager>().RemoveEvent(val);
						eventHandles[item] = null;
					}
				}
				return;
			}
		}
		AddonWeeklyBingo* weeklyBingo = (AddonWeeklyBingo*)Addons.WeeklyBingo;
		if (weeklyBingo == null)
		{
			return;
		}
		foreach (int item2 in Enumerable.Range(0, 16))
		{
			DutySlot val2 = ((DutySlotList)(&((AddonWeeklyBingo)weeklyBingo).DutySlotList))[item2];
			IAddonEventManager obj = DalamudServiceExtension.Instance<IAddonEventManager>();
			AtkComponentNode* ownerNode = ((AtkComponentButton)val2.DutyButton).OwnerNode;
			object obj2 = _003C_003EO._003C0_003E__OnDutySlotClick;
			if (obj2 == null)
			{
				AddonEventDelegate val3 = OnDutySlotClick;
				_003C_003EO._003C0_003E__OnDutySlotClick = val3;
				obj2 = (object)val3;
			}
			IAddonEventHandle val4 = obj.AddEvent((IntPtr)(nint)weeklyBingo, (IntPtr)(nint)ownerNode, (AddonEventType)25, (AddonEventDelegate)obj2);
			eventHandles[item2] = val4;
		}
	}

	private unsafe static void OnDutySlotClick(AddonEventType atkEventType, AddonEventData data)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		AtkResNode* nodeTargetPointer = (AtkResNode*)data.NodeTargetPointer;
		if (nodeTargetPointer == null)
		{
			return;
		}
		AgentContentsFinder* ptr = AgentContentsFinder.Instance();
		if (ptr == null || IConditionExtension.get_IsBoundByDuty(DalamudServiceExtension.Instance<ICondition>()))
		{
			return;
		}
		int num = (int)(((AtkResNode)nodeTargetPointer).NodeId - 12);
		WeeklyBingoTaskStatus weeklyBingoTaskStatus = ((PlayerState)PlayerState.Instance()).GetWeeklyBingoTaskStatus(num);
		byte bingoRowID = ((PlayerState)PlayerState.Instance()).WeeklyBingoOrderData[num];
		if ((int)weeklyBingoTaskStatus == 0)
		{
			if (TryGetRouletteDutyByBingoData(bingoRowID, out var rouletteRowID))
			{
				((AgentContentsFinder)ptr).OpenRouletteDuty(rouletteRowID, false);
			}
			if (TryGetRegularDutyByBingoData(bingoRowID, out var dutyRowID))
			{
				((AgentContentsFinder)ptr).OpenRegularDuty(dutyRowID, false);
			}
		}
	}

	private static bool TryGetRouletteDutyByBingoData(uint bingoRowID, out byte rouletteRowID)
	{
		rouletteRowID = bingoRowID switch
		{
			54u => 7, 
			52u => 40, 
			_ => 0, 
		};
		return rouletteRowID != 0;
	}

	private static bool TryGetRegularDutyByBingoData(uint bingoRowID, out uint dutyRowID)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_0686: Unknown result type (might be due to invalid IL or missing references)
		//IL_068b: Unknown result type (might be due to invalid IL or missing references)
		//IL_083c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0841: Unknown result type (might be due to invalid IL or missing references)
		//IL_070c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0711: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0990: Unknown result type (might be due to invalid IL or missing references)
		//IL_0995: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b08: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b0d: Unknown result type (might be due to invalid IL or missing references)
		//IL_054d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0552: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0792: Unknown result type (might be due to invalid IL or missing references)
		//IL_0797: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a3a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a3f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bb2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bb7: Unknown result type (might be due to invalid IL or missing references)
		dutyRowID = 0u;
		if (!LuminaGetter.TryGetRow<WeeklyBingoOrderData>(bingoRowID, out WeeklyBingoOrderData item))
		{
			return false;
		}
		RowRef data = ((WeeklyBingoOrderData)(ref item)).Data;
		uint bingoDataID = ((RowRef)(ref data)).RowId;
		ContentFinderCondition val;
		uint num;
		switch (((WeeklyBingoOrderData)(ref item)).Type)
		{
		case 0u:
			val = (from row in ((IEnumerable<ContentFinderCondition>)LuminaGetter.Get<ContentFinderCondition>()).Where((ContentFinderCondition _) => UIState.IsInstanceContentUnlocked(bingoDataID)).Where(delegate(ContentFinderCondition c)
				{
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					RowRef content = ((ContentFinderCondition)(ref c)).Content;
					return ((RowRef)(ref content)).RowId == bingoDataID;
				})
				orderby ((ContentFinderCondition)(ref row)).SortKey
				select row).FirstOrDefault();
			num = ((ContentFinderCondition)(ref val)).RowId;
			break;
		case 1u:
			val = (from row in (IEnumerable<ContentFinderCondition>)LuminaGetter.Get<ContentFinderCondition>()
				where ((ContentFinderCondition)(ref row)).ContentType.RowId == 2
				where ((ContentFinderCondition)(ref row)).ClassJobLevelRequired == bingoDataID
				orderby ((ContentFinderCondition)(ref row)).SortKey
				select row).FirstOrDefault();
			num = ((ContentFinderCondition)(ref val)).RowId;
			break;
		case 2u:
		{
			int num3;
			if (bingoDataID != 49)
			{
				val = (from row in (IEnumerable<ContentFinderCondition>)LuminaGetter.Get<ContentFinderCondition>()
					where ((ContentFinderCondition)(ref row)).ContentType.RowId == 2
					where ((ContentFinderCondition)(ref row)).ClassJobLevelRequired >= bingoDataID - ((bingoDataID > 50) ? 9 : 49) && ((ContentFinderCondition)(ref row)).ClassJobLevelRequired <= bingoDataID - 1
					orderby ((ContentFinderCondition)(ref row)).SortKey
					select row).FirstOrDefault();
				num3 = (int)((ContentFinderCondition)(ref val)).RowId;
			}
			else
			{
				num3 = 56;
			}
			num = (uint)num3;
			break;
		}
		case 3u:
		{
			uint num2;
			switch (bingoRowID)
			{
			case 46u:
				num2 = 0u;
				break;
			case 52u:
				num2 = 0u;
				break;
			case 53u:
				val = (from row in (IEnumerable<ContentFinderCondition>)LuminaGetter.Get<ContentFinderCondition>()
					where ((ContentFinderCondition)(ref row)).ContentType.RowId == 21
					orderby ((ContentFinderCondition)(ref row)).SortKey
					select row).FirstOrDefault();
				num2 = ((ContentFinderCondition)(ref val)).RowId;
				break;
			case 54u:
				num2 = 0u;
				break;
			case 67u:
				num2 = 599u;
				break;
			default:
				num2 = 0u;
				break;
			}
			num = num2;
			break;
		}
		case 4u:
			num = bingoDataID switch
			{
				2u => 93u, 
				3u => 98u, 
				4u => 107u, 
				5u => 112u, 
				6u => 136u, 
				7u => 186u, 
				8u => 252u, 
				9u => 286u, 
				10u => 587u, 
				11u => 653u, 
				12u => 682u, 
				13u => 715u, 
				14u => 726u, 
				15u => 749u, 
				16u => 751u, 
				17u => 808u, 
				18u => 807u, 
				19u => 872u, 
				20u => 876u, 
				21u => 936u, 
				22u => 941u, 
				23u => 653u, 
				24u => 715u, 
				25u => 749u, 
				26u => 92u, 
				27u => 120u, 
				28u => 281u, 
				29u => 700u, 
				30u => 866u, 
				31u => 808u, 
				32u => 872u, 
				33u => 936u, 
				34u => 985u, 
				35u => 989u, 
				36u => 985u, 
				37u => 1019u, 
				38u => 1023u, 
				_ => 0u, 
			};
			break;
		case 5u:
		{
			uint num2;
			switch (bingoDataID)
			{
			case 49u:
				num2 = 56u;
				break;
			case 79u:
				val = (from row in ((IEnumerable<ContentFinderCondition>)LuminaGetter.Get<ContentFinderCondition>()).Where((ContentFinderCondition m) => ((ContentFinderCondition)(ref m)).ContentType.RowId == 2).Where(delegate(ContentFinderCondition m)
					{
						byte classJobLevelRequired = ((ContentFinderCondition)(ref m)).ClassJobLevelRequired;
						return classJobLevelRequired >= 51 && classJobLevelRequired <= 79 && ((ContentFinderCondition)(ref m)).ClassJobLevelRequired % 10 != 0;
					})
					where ((ContentFinderCondition)(ref row)).ClassJobLevelRequired % 10 != 0
					orderby ((ContentFinderCondition)(ref row)).SortKey
					select row).FirstOrDefault();
				num2 = ((ContentFinderCondition)(ref val)).RowId;
				break;
			case 99u:
				val = (from row in ((IEnumerable<ContentFinderCondition>)LuminaGetter.Get<ContentFinderCondition>()).Where((ContentFinderCondition m) => ((ContentFinderCondition)(ref m)).ContentType.RowId == 2).Where(delegate(ContentFinderCondition m)
					{
						byte classJobLevelRequired = ((ContentFinderCondition)(ref m)).ClassJobLevelRequired;
						return classJobLevelRequired >= 81 && classJobLevelRequired <= 99 && ((ContentFinderCondition)(ref m)).ClassJobLevelRequired % 10 != 0;
					})
					orderby ((ContentFinderCondition)(ref row)).SortKey
					select row).FirstOrDefault();
				num2 = ((ContentFinderCondition)(ref val)).RowId;
				break;
			default:
				num2 = 0u;
				break;
			}
			num = num2;
			break;
		}
		case 6u:
		{
			uint num2;
			switch (bingoDataID)
			{
			case 60u:
				val = (from row in ((IEnumerable<ContentFinderCondition>)LuminaGetter.Get<ContentFinderCondition>()).Where((ContentFinderCondition m) => ((ContentFinderCondition)(ref m)).ContentType.RowId == 2).Where(delegate(ContentFinderCondition m)
					{
						byte classJobLevelRequired = ((ContentFinderCondition)(ref m)).ClassJobLevelRequired;
						return (classJobLevelRequired == 50 || classJobLevelRequired == 60) ? true : false;
					})
					orderby ((ContentFinderCondition)(ref row)).SortKey
					select row).FirstOrDefault();
				num2 = ((ContentFinderCondition)(ref val)).RowId;
				break;
			case 80u:
				val = (from row in ((IEnumerable<ContentFinderCondition>)LuminaGetter.Get<ContentFinderCondition>()).Where((ContentFinderCondition m) => ((ContentFinderCondition)(ref m)).ContentType.RowId == 2).Where(delegate(ContentFinderCondition m)
					{
						byte classJobLevelRequired = ((ContentFinderCondition)(ref m)).ClassJobLevelRequired;
						return (classJobLevelRequired == 70 || classJobLevelRequired == 80) ? true : false;
					})
					orderby ((ContentFinderCondition)(ref row)).SortKey
					select row).FirstOrDefault();
				num2 = ((ContentFinderCondition)(ref val)).RowId;
				break;
			case 90u:
				val = (from row in (IEnumerable<ContentFinderCondition>)LuminaGetter.Get<ContentFinderCondition>()
					where ((ContentFinderCondition)(ref row)).ContentType.RowId == 2
					where ((ContentFinderCondition)(ref row)).ClassJobLevelRequired == 90
					orderby ((ContentFinderCondition)(ref row)).SortKey
					select row).FirstOrDefault();
				num2 = ((ContentFinderCondition)(ref val)).RowId;
				break;
			default:
				num2 = 0u;
				break;
			}
			num = num2;
			break;
		}
		case 7u:
		{
			uint num2;
			switch (bingoDataID)
			{
			case 60u:
				val = (from row in ((IEnumerable<ContentFinderCondition>)LuminaGetter.Get<ContentFinderCondition>()).Where((ContentFinderCondition m) => ((ContentFinderCondition)(ref m)).ContentType.RowId == 4).Where(delegate(ContentFinderCondition m)
					{
						byte classJobLevelRequired = ((ContentFinderCondition)(ref m)).ClassJobLevelRequired;
						return classJobLevelRequired >= 50 && classJobLevelRequired <= 60;
					})
					orderby ((ContentFinderCondition)(ref row)).SortKey
					select row).FirstOrDefault();
				num2 = ((ContentFinderCondition)(ref val)).RowId;
				break;
			case 100u:
				val = (from row in ((IEnumerable<ContentFinderCondition>)LuminaGetter.Get<ContentFinderCondition>()).Where((ContentFinderCondition m) => ((ContentFinderCondition)(ref m)).ContentType.RowId == 4).Where(delegate(ContentFinderCondition m)
					{
						byte classJobLevelRequired = ((ContentFinderCondition)(ref m)).ClassJobLevelRequired;
						return classJobLevelRequired >= 70 && classJobLevelRequired <= 100;
					})
					orderby ((ContentFinderCondition)(ref row)).SortKey
					select row).FirstOrDefault();
				num2 = ((ContentFinderCondition)(ref val)).RowId;
				break;
			default:
				num2 = 0u;
				break;
			}
			num = num2;
			break;
		}
		case 8u:
		{
			uint num2;
			switch (bingoDataID)
			{
			case 60u:
				val = (from row in ((IEnumerable<ContentFinderCondition>)LuminaGetter.Get<ContentFinderCondition>()).Where((ContentFinderCondition m) => ((ContentFinderCondition)(ref m)).ContentType.RowId == 5).Where(delegate(ContentFinderCondition m)
					{
						byte classJobLevelRequired = ((ContentFinderCondition)(ref m)).ClassJobLevelRequired;
						return classJobLevelRequired >= 50 && classJobLevelRequired <= 60;
					})
					where ((ContentFinderCondition)(ref row)).AllianceRoulette
					orderby ((ContentFinderCondition)(ref row)).SortKey
					select row).FirstOrDefault();
				num2 = ((ContentFinderCondition)(ref val)).RowId;
				break;
			case 90u:
				val = (from row in ((IEnumerable<ContentFinderCondition>)LuminaGetter.Get<ContentFinderCondition>()).Where((ContentFinderCondition m) => ((ContentFinderCondition)(ref m)).ContentType.RowId == 5).Where(delegate(ContentFinderCondition m)
					{
						byte classJobLevelRequired = ((ContentFinderCondition)(ref m)).ClassJobLevelRequired;
						return classJobLevelRequired >= 70 && classJobLevelRequired <= 90;
					})
					where ((ContentFinderCondition)(ref row)).AllianceRoulette
					orderby ((ContentFinderCondition)(ref row)).SortKey
					select row).FirstOrDefault();
				num2 = ((ContentFinderCondition)(ref val)).RowId;
				break;
			default:
				num2 = 0u;
				break;
			}
			num = num2;
			break;
		}
		case 9u:
		{
			uint num2;
			switch (bingoDataID)
			{
			case 60u:
				val = (from row in (from m in (IEnumerable<ContentFinderCondition>)LuminaGetter.Get<ContentFinderCondition>()
						where ((ContentFinderCondition)(ref m)).ContentType.RowId == 5
						where ((ContentFinderCondition)(ref m)).NormalRaidRoulette
						select m).Where(delegate(ContentFinderCondition m)
					{
						byte classJobLevelRequired = ((ContentFinderCondition)(ref m)).ClassJobLevelRequired;
						return classJobLevelRequired >= 50 && classJobLevelRequired <= 60;
					})
					orderby ((ContentFinderCondition)(ref row)).SortKey
					select row).FirstOrDefault();
				num2 = ((ContentFinderCondition)(ref val)).RowId;
				break;
			case 100u:
				val = (from row in (from m in (IEnumerable<ContentFinderCondition>)LuminaGetter.Get<ContentFinderCondition>()
						where ((ContentFinderCondition)(ref m)).ContentType.RowId == 5
						where ((ContentFinderCondition)(ref m)).NormalRaidRoulette
						select m).Where(delegate(ContentFinderCondition m)
					{
						byte classJobLevelRequired = ((ContentFinderCondition)(ref m)).ClassJobLevelRequired;
						return classJobLevelRequired >= 70 && classJobLevelRequired <= 100;
					})
					orderby ((ContentFinderCondition)(ref row)).SortKey
					select row).FirstOrDefault();
				num2 = ((ContentFinderCondition)(ref val)).RowId;
				break;
			default:
				num2 = 0u;
				break;
			}
			num = num2;
			break;
		}
		default:
			num = 0u;
			break;
		}
		dutyRowID = num;
		if (((WeeklyBingoOrderData)(ref item)).Type == 0 && !UIState.IsInstanceContentUnlocked(bingoDataID))
		{
			OmenServiceBase<NotifyHelper>.Instance().NotificationError(LanguageManager.Get("WeeklyBingoClickToOpen-UnlockError"));
		}
		return dutyRowID != 0;
	}
}
