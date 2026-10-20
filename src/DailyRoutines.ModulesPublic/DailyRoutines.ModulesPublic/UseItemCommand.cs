using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.Command;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using TinyPinyin;

namespace DailyRoutines.ModulesPublic;

public class UseItemCommand : ModuleBase
{
	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static HandlerDelegate _003C0_003E__OnCommand;
	}

	private const string COMMAND = "item";

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("UseItemCommandTitle"),
		Description = LanguageManager.Get("UseItemCommandDescription", "item"),
		Category = ModuleCategory.Assist
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
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
		commandManager.AddSubCommand("item", new CommandInfo((HandlerDelegate)obj)
		{
			HelpMessage = LanguageManager.Get("UseItemCommand-CommandHelp")
		});
	}

	protected override void Uninit()
	{
		OmenServiceBase<CommandManager>.Instance().RemoveSubCommand("item");
	}

	private unsafe static void OnCommand(string command, string args)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		args = args.Trim();
		if (string.IsNullOrEmpty(args))
		{
			return;
		}
		if (uint.TryParse(args, out var result))
		{
			UseItemByID(result);
			return;
		}
		if (!((IEnumerable<InventoryType>)Inventories.Player).TryGetItems((Predicate<InventoryItem>)((InventoryItem _) => true), out List<InventoryItem> itemResult))
		{
			OmenServiceBase<NotifyHelper>.Instance().ChatError(ReadOnlySeString.op_Implicit(LanguageManager.Get("UseItemCommand-Notice-NotFound", args)));
			return;
		}
		args = args.ToLowerInvariant();
		foreach (InventoryItem item2 in itemResult)
		{
			InventoryItem current = item2;
			if (LuminaGetter.TryGetRow<Item>(((InventoryItem)(ref current)).GetBaseItemId(), out Item item))
			{
				string text = ((object)((Item)(ref item)).Name/*cast due to constrained. prefix*/).ToString();
				if (!string.IsNullOrWhiteSpace(text) && (text.Contains(args, StringComparison.OrdinalIgnoreCase) || PinyinHelper.GetPinyin(text, string.Empty).Contains(args, StringComparison.OrdinalIgnoreCase)))
				{
					((AgentInventoryContext)AgentInventoryContext.Instance()).UseItem(current.ItemId, (InventoryType)9999, 0u, (short)0);
					return;
				}
			}
		}
		OmenServiceBase<NotifyHelper>.Instance().ChatError(ReadOnlySeString.op_Implicit(LanguageManager.Get("UseItemCommand-Notice-NotFound", args)));
	}

	private unsafe static void UseItemByID(uint itemID)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		InventoryItem* itemResult;
		if (!LuminaGetter.TryGetRow<Item>(itemID, out Item item))
		{
			OmenServiceBase<NotifyHelper>.Instance().ChatError(ReadOnlySeString.op_Implicit(LanguageManager.Get("UseItemCommand-Notice-NotFound", itemID)));
		}
		else if (!((IEnumerable<InventoryType>)Inventories.Player).TryGetFirstItem((Predicate<InventoryItem>)((InventoryItem x) => ((InventoryItem)(ref x)).GetBaseItemId() == itemID), out itemResult))
		{
			OmenServiceBase<NotifyHelper>.Instance().ChatError(ReadOnlySeString.op_Implicit(LanguageManager.Get("UseItemCommand-Notice-NotInInventory", ((object)((Item)(ref item)).Name/*cast due to constrained. prefix*/).ToString())));
		}
		else
		{
			((AgentInventoryContext)AgentInventoryContext.Instance()).UseItem(itemID, ((InventoryItem)itemResult).GetInventoryType(), (uint)((InventoryItem)itemResult).GetSlot(), (short)0);
		}
	}
}
