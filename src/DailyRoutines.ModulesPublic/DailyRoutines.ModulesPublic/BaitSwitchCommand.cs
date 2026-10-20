using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Command;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Interop.Game.ExecuteCommand.Implementations;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using TinyPinyin;

namespace DailyRoutines.ModulesPublic;

public class BaitSwitchCommand : ModuleBase
{
	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static HandlerDelegate _003C0_003E__OnCommand;
	}

	private const string COMMAND = "bait";

	private static readonly FrozenDictionary<uint, (string NameLower, string NamePinyin)> Baits = ((IEnumerable<Item>)LuminaGetter.Get<Item>()).Where((Item x) => ((Item)(ref x)).ItemUICategory.RowId == 33 && !string.IsNullOrWhiteSpace(((object)((Item)(ref x)).Name/*cast due to constrained. prefix*/).ToString())).ToFrozenDictionary((Item x) => ((Item)(ref x)).RowId, (Item x) => (((object)((Item)(ref x)).Name/*cast due to constrained. prefix*/).ToString().ToLower(), PinyinHelper.GetPinyin(((object)((Item)(ref x)).Name/*cast due to constrained. prefix*/).ToString(), string.Empty)));

	private static readonly FrozenDictionary<uint, (string NameLower, string NamePinyin)> Fishes = ((IEnumerable<Item>)LuminaGetter.Get<Item>()).Where((Item x) => ((Item)(ref x)).FilterGroup == 16 && !string.IsNullOrWhiteSpace(((object)((Item)(ref x)).Name/*cast due to constrained. prefix*/).ToString())).ToFrozenDictionary((Item x) => ((Item)(ref x)).RowId, (Item x) => (((object)((Item)(ref x)).Name/*cast due to constrained. prefix*/).ToString().ToLower(), PinyinHelper.GetPinyin(((object)((Item)(ref x)).Name/*cast due to constrained. prefix*/).ToString(), string.Empty)));

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("BaitSwitchCommandTitle"),
		Description = LanguageManager.Get("BaitSwitchCommandDescription"),
		Category = ModuleCategory.Assist
	};

	protected override void Init()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected O, but got Unknown
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		CommandManager commandManager = OmenServiceBase<CommandManager>.Instance();
		object obj = _003C_003EO._003C0_003E__OnCommand;
		if (obj == null)
		{
			HandlerDelegate val = OnCommand;
			_003C_003EO._003C0_003E__OnCommand = val;
			obj = (object)val;
		}
		commandManager.AddSubCommand("bait", new CommandInfo((HandlerDelegate)obj)
		{
			HelpMessage = LanguageManager.Get("BaitSwitchCommand-CommandHelp")
		});
	}

	protected override void Uninit()
	{
		OmenServiceBase<CommandManager>.Instance().RemoveSubCommand("bait");
	}

	protected override void ConfigUI()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TextWrapped(ImU8String.op_Implicit(LanguageManager.Get("BaitSwitchCommand-CommandHelpDetailed")));
	}

	private static void OnCommand(string command, string arguments)
	{
		arguments = arguments.Trim();
		if (!string.IsNullOrWhiteSpace(arguments))
		{
			if (!uint.TryParse(arguments, out var result))
			{
				SwitchBaitByName(arguments);
			}
			else
			{
				SwitchBaitByID(result);
			}
		}
	}

	private static void SwitchBaitByName(string itemName)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		itemName = itemName.ToLower();
		uint item;
		bool num = TryFindItemByName(Baits, itemName, out item);
		bool flag = false;
		if (!num)
		{
			flag = TryFindItemByName(Fishes, itemName, out item);
		}
		if (num == flag)
		{
			OmenServiceBase<NotifyHelper>.Instance().ChatError(ReadOnlySeString.op_Implicit(LanguageManager.Get("BaitSwitchCommand-Notice-NoMatchBait", itemName)));
		}
		else
		{
			SwitchBaitByID(item);
		}
	}

	private static void SwitchBaitByID(uint itemID)
	{
		if (IsAbleToSwitch(itemID, out var isBait, out var swimBaitIndex))
		{
			SwitchBait(itemID, isBait, swimBaitIndex);
		}
	}

	private static void SwitchBait(uint itemID, bool isBait, int swimBaitIndex = -1)
	{
		if (isBait)
		{
			FishingCommand.ChangeBait(itemID);
		}
		else if (swimBaitIndex != -1)
		{
			FishingCommand.SwimBait((uint)swimBaitIndex);
		}
	}

	private static bool TryFindItemByName(IDictionary<uint, (string NameLower, string NamePinyin)> source, string itemName, out uint item)
	{
		item = source.FirstOrDefault<KeyValuePair<uint, (string, string)>>((KeyValuePair<uint, (string NameLower, string NamePinyin)> x) => x.Value.NameLower.Equals(itemName, StringComparison.OrdinalIgnoreCase) || x.Value.NamePinyin.Equals(itemName, StringComparison.OrdinalIgnoreCase)).Key;
		if (item == 0)
		{
			List<KeyValuePair<uint, (string, string)>> source2 = (from x in source
				where x.Value.NameLower.Contains(itemName, StringComparison.OrdinalIgnoreCase) || (GameState.IsCN && x.Value.NamePinyin.Contains(itemName, StringComparison.OrdinalIgnoreCase))
				orderby x.Value.NameLower
				select x).ToList();
			item = source2.FirstOrDefault().Key;
		}
		return item != 0;
	}

	private unsafe static bool IsAbleToSwitch(uint itemID, out bool isBait, out int swimBaitIndex)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		isBait = true;
		swimBaitIndex = -1;
		if (itemID == 0 || (!Baits.ContainsKey(itemID) && !Fishes.ContainsKey(itemID)))
		{
			OmenServiceBase<NotifyHelper>.Instance().ChatError(ReadOnlySeString.op_Implicit(LanguageManager.Get("BaitSwitchCommand-Notice-NoMatchBait", itemID)));
			return false;
		}
		Item? row = LuminaGetter.GetRow<Item>(itemID);
		object obj;
		if (!row.HasValue)
		{
			obj = null;
		}
		else
		{
			Item valueOrDefault = row.GetValueOrDefault();
			obj = ((object)((Item)(ref valueOrDefault)).Name/*cast due to constrained. prefix*/).ToString();
		}
		string text = (string)obj;
		if (Baits.ContainsKey(itemID))
		{
			if (((InventoryManager)InventoryManager.Instance()).GetInventoryItemCount(itemID, false, true, true, (short)0) <= 0)
			{
				OmenServiceBase<NotifyHelper>.Instance().ChatError(ReadOnlySeString.op_Implicit(LanguageManager.Get("BaitSwitchCommand-Notice-NoBait", text)));
				return false;
			}
		}
		else
		{
			isBait = false;
			List<uint> swimBaitInfo = GetSwimBaitInfo();
			swimBaitIndex = swimBaitInfo.IndexOf(itemID);
			if (swimBaitIndex == -1)
			{
				OmenServiceBase<NotifyHelper>.Instance().ChatError(ReadOnlySeString.op_Implicit(LanguageManager.Get("BaitSwitchCommand-Notice-NoBait", text)));
				return false;
			}
		}
		if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)43])
		{
			OmenServiceBase<NotifyHelper>.Instance().ChatError(ReadOnlySeString.op_Implicit(LanguageManager.Get("BaitSwitchCommand-Notice-FishingNow")));
			return false;
		}
		return true;
	}

	private unsafe static List<uint> GetSwimBaitInfo()
	{
		FishingEventHandler* eventHandlerById = (FishingEventHandler*)((EventFramework)EventFramework.Instance()).GetEventHandlerById(1376257u);
		return ((FishingEventHandler)eventHandlerById).SwimBaitItemIds.ToArray().ToList();
	}
}
