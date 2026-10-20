using System;
using System.Runtime.CompilerServices;
using System.Threading;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.Command;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using OmenTools.Extensions;
using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.ExecuteCommand.Implementations;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class GlamourPlateApplyCommand : ModuleBase
{
	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static HandlerDelegate _003C0_003E__OnCommand;
	}

	private const string COMMAND = "gpapply";

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("GlamourPlateApplyCommandTitle"),
		Description = LanguageManager.Get("GlamourPlateApplyCommandDescription"),
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
		commandManager.AddSubCommand("gpapply", new CommandInfo((HandlerDelegate)obj)
		{
			HelpMessage = LanguageManager.Get("GlamourPlateApplyCommand-CommandHelp")
		});
	}

	protected override void Uninit()
	{
		OmenServiceBase<CommandManager>.Instance().RemoveSubCommand("gpapply");
	}

	private unsafe static void OnCommand(string command, string arguments)
	{
		int index = default(int);
		bool flag = string.IsNullOrWhiteSpace(arguments) || !int.TryParse(arguments.Trim(), out index);
		if (!flag)
		{
			bool flag2 = ((index < 1 || index > 20) ? true : false);
			flag = flag2;
		}
		if (flag)
		{
			return;
		}
		if (!((MirageManager)MirageManager.Instance()).GlamourPlatesLoaded)
		{
			OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestGlamourPlate);
			DalamudServiceExtension.Instance<IFramework>().RunOnTick((Action)delegate
			{
				ApplyGlamourPlate(index);
			}, TimeSpan.FromMilliseconds(500L), 0, default(CancellationToken));
		}
		else
		{
			ApplyGlamourPlate(index);
		}
	}

	private static void ApplyGlamourPlate(int index)
	{
		GlamourPlateCommand.Enter();
		GlamourPlateCommand.ApplyPlate((uint)(index - 1));
		GlamourPlateCommand.Exit();
	}
}
