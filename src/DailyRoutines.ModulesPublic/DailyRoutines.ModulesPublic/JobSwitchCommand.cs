using System;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.Command;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using TinyPinyin;

namespace DailyRoutines.ModulesPublic;

public class JobSwitchCommand : ModuleBase
{
	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static HandlerDelegate _003C0_003E__OnCommand;
	}

	private const string COMMAND = "job";

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("JobSwitchCommandTitle"),
		Description = LanguageManager.Get("JobSwitchCommandDescription", "job"),
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
		commandManager.AddSubCommand("job", new CommandInfo((HandlerDelegate)obj)
		{
			HelpMessage = LanguageManager.Get("JobSwitchCommand-CommandHelp")
		});
	}

	protected override void Uninit()
	{
		OmenServiceBase<CommandManager>.Instance().RemoveSubCommand("job");
	}

	private static void OnCommand(string command, string args)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		args = args.ToLowerInvariant().Trim();
		if (string.IsNullOrEmpty(args))
		{
			return;
		}
		if (byte.TryParse(args, out var result) && result > 0 && LuminaGetter.TryGetRow<ClassJob>((uint)result, out ClassJob _))
		{
			LocalPlayerState.SwitchGearset((uint)result);
			return;
		}
		Enumerator<ClassJob> enumerator = LuminaGetter.Get<ClassJob>().GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				ClassJob current = enumerator.Current;
				if (((ClassJob)(ref current)).RowId != 0 && !string.IsNullOrWhiteSpace(((object)((ClassJob)(ref current)).Name/*cast due to constrained. prefix*/).ToString()) && (((object)((ClassJob)(ref current)).Name/*cast due to constrained. prefix*/).ToString().Contains(args, StringComparison.OrdinalIgnoreCase) || PinyinHelper.GetPinyin(((object)((ClassJob)(ref current)).Name/*cast due to constrained. prefix*/).ToString(), string.Empty).Contains(args, StringComparison.OrdinalIgnoreCase) || ((object)((ClassJob)(ref current)).NameEnglish/*cast due to constrained. prefix*/).ToString().Contains(args, StringComparison.OrdinalIgnoreCase)))
				{
					LocalPlayerState.SwitchGearset(((ClassJob)(ref current)).RowId);
					break;
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
	}
}
