using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.Command;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using Lumina.Excel.Sheets;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class PhantomJobSwitchCommand : ModuleBase
{
	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static HandlerDelegate _003C0_003E__OnCommand;
	}

	private const string COMMAND = "pjob";

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("PhantomJobSwitchCommandTitle"),
		Description = LanguageManager.Get("PhantomJobSwitchCommandDescription", "pjob"),
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
		commandManager.AddSubCommand("pjob", new CommandInfo((HandlerDelegate)obj)
		{
			HelpMessage = LanguageManager.Get("PhantomJobSwitchCommand-CommandHelp")
		});
	}

	protected override void Uninit()
	{
		OmenServiceBase<CommandManager>.Instance().RemoveSubCommand("pjob");
	}

	private unsafe static void OnCommand(string command, string args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Invalid comparison between Unknown and I4
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		if ((int)GameState.TerritoryIntendedUse != 61)
		{
			((RaptureLogModule)RaptureLogModule.Instance()).ShowLogMessage(10970u);
			return;
		}
		args = args.Trim().ToLowerInvariant();
		if (string.IsNullOrWhiteSpace(args))
		{
			return;
		}
		if (byte.TryParse(args, out var result))
		{
			((AgentMKDSupportJobList)AgentMKDSupportJobList.Instance()).ChangeSupportJob(result);
			return;
		}
		var anon = (from data2 in (IEnumerable<MKDSupportJob>)LuminaGetter.Get<MKDSupportJob>()
			select new
			{
				Data = data2,
				NameMale = ((object)((MKDSupportJob)(ref data2)).Name/*cast due to constrained. prefix*/).ToString(),
				NameFemale = ((object)((MKDSupportJob)(ref data2)).NameFemale/*cast due to constrained. prefix*/).ToString(),
				NameEnglish = ((object)((MKDSupportJob)(ref data2)).NameEnglish/*cast due to constrained. prefix*/).ToString()
			} into x
			where x.NameMale.Contains(args, StringComparison.OrdinalIgnoreCase) || x.NameFemale.Contains(args, StringComparison.OrdinalIgnoreCase) || x.NameEnglish.Contains(args, StringComparison.OrdinalIgnoreCase)
			orderby Math.Min(Math.Min(x.NameMale.Length, x.NameFemale.Length), x.NameEnglish.Length)
			select x).FirstOrDefault();
		if (anon != null)
		{
			AgentMKDSupportJobList* intPtr = AgentMKDSupportJobList.Instance();
			MKDSupportJob data = anon.Data;
			((AgentMKDSupportJobList)intPtr).ChangeSupportJob((byte)((MKDSupportJob)(ref data)).RowId);
		}
	}
}
