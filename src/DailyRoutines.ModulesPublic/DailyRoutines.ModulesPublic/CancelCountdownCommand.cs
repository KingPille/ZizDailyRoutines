using System;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.Command;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class CancelCountdownCommand : ModuleBase
{
	private const string COMMAND = "ccd";

	private Action cancelCountdown;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("CancelCountdownCommandTitle"),
		Description = LanguageManager.Get("CancelCountdownCommandDescription", "ccd"),
		Category = ModuleCategory.Assist,
		Author = new string[1] { "decorwdyun" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected override void Init()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		cancelCountdown = new CompSig("E8 ?? ?? ?? ?? 45 33 E4 41 C6 47 ?? ?? 45 89 66 30").GetDelegate<Action>();
		OmenServiceBase<CommandManager>.Instance().AddSubCommand("ccd", new CommandInfo(new HandlerDelegate(OnCommand))
		{
			HelpMessage = LanguageManager.Get("CancelCountdownCommand-CommandHelp")
		});
	}

	protected override void Uninit()
	{
		OmenServiceBase<CommandManager>.Instance().RemoveSubCommand("ccd");
	}

	public unsafe void OnCommand(string command, string arguments)
	{
		if (((AgentCountDownSettingDialog)AgentCountDownSettingDialog.Instance()).Active)
		{
			cancelCountdown();
		}
	}
}
