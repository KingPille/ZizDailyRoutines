using System;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.Command;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using TinyPinyin;

namespace DailyRoutines.ModulesPublic;

public class OrnamentActionCommand : ModuleBase
{
	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static HandlerDelegate _003C0_003E__OnCommand;
	}

	private const string COMMAND = "oac";

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("OrnamentActionCommandTitle"),
		Description = LanguageManager.Get("OrnamentActionCommandDescription", "oac"),
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
		commandManager.AddSubCommand("oac", new CommandInfo((HandlerDelegate)obj)
		{
			HelpMessage = LanguageManager.Get("OrnamentActionCommand-CommandHelp")
		});
	}

	protected override void Uninit()
	{
		OmenServiceBase<CommandManager>.Instance().RemoveSubCommand("oac");
	}

	private unsafe static void OnCommand(string command, string args)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		args = args.Trim().TrimQuotationMarks().Trim();
		if (string.IsNullOrEmpty(args))
		{
			return;
		}
		BattleChara* localPlayer = Control.GetLocalPlayer();
		if (localPlayer == null || !LuminaGetter.TryGetRow<Ornament>((uint)((OrnamentContainer)(&((BattleChara)localPlayer).OrnamentData)).OrnamentId, out Ornament item))
		{
			return;
		}
		uint result;
		bool flag = uint.TryParse(args, out result);
		OrnamentAction value = ((Ornament)(ref item)).Action.Value;
		Enumerator<RowRef<Action>> enumerator = ((OrnamentAction)(ref value)).Actions.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				RowRef<Action> current = enumerator.Current;
				if (current.RowId != 0)
				{
					Action value2 = current.Value;
					if (flag && result == ((Action)(ref value2)).RowId)
					{
						OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)1, ((Action)(ref value2)).RowId, 3758096384uL, 0u, (UseActionMode)0);
						break;
					}
					string? text = ((object)((Action)(ref value2)).Name/*cast due to constrained. prefix*/).ToString();
					string pinyin = PinyinHelper.GetPinyin(text, string.Empty);
					if (text.Contains(args, StringComparison.OrdinalIgnoreCase) || pinyin.Contains(args, StringComparison.OrdinalIgnoreCase))
					{
						OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)1, ((Action)(ref value2)).RowId, 3758096384uL, 0u, (UseActionMode)0);
						break;
					}
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
	}
}
