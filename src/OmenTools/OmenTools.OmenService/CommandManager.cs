using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Dalamud.Game.Command;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using OmenTools.Dalamud;
using OmenTools.Extensions;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.OmenService;

public sealed class CommandManager : OmenServiceBase<CommandManager>
{
	[CompilerGenerated]
	private MainCommandInfo? _003CMainCommand_003Ek__BackingField;

	public MainCommandInfo? MainCommand
	{
		[CompilerGenerated]
		get
		{
			return _003CMainCommand_003Ek__BackingField;
		}
		set
		{
			if (value == null)
			{
				if (_003CMainCommand_003Ek__BackingField != null)
				{
					RemoveCommand(_003CMainCommand_003Ek__BackingField.Command);
				}
			}
			else
			{
				_003CMainCommand_003Ek__BackingField = value;
				RefreshCommandDetails();
			}
		}
	}

	public ConcurrentDictionary<string, CommandInfo> Commands { get; private set; } = new ConcurrentDictionary<string, CommandInfo>();

	public ConcurrentDictionary<string, CommandInfo> SubCommands { get; private set; } = new ConcurrentDictionary<string, CommandInfo>();

	protected override void Uninit()
	{
		foreach (string key in Commands.Keys)
		{
			RemoveCommand(key);
		}
		Commands.Clear();
		SubCommands.Clear();
		MainCommand = null;
	}

	private void OnMainCommand(string command, string args)
	{
		DLog.Debug("[CommandManager] [MainCommand] " + command + " " + args);
		if (string.IsNullOrEmpty(args))
		{
			MainCommand.Info.Handler.Invoke(command, args);
			return;
		}
		string[] array = args.Split(' ', 2);
		if (SubCommands.TryGetValue(array[0], out CommandInfo value))
		{
			value.Handler.Invoke(array[0], (array.Length > 1) ? array[1] : string.Empty);
		}
		else
		{
			NotifyCommandError();
		}
		unsafe void NotifyCommandError()
		{
			Utf8String val = default(Utf8String);
			((Utf8String)(ref val))._002Ector(command + " " + args);
			try
			{
				((RaptureLogModule)RaptureLogModule.Instance()).ShowLogMessageString(725u, &val);
			}
			finally
			{
				((IDisposable)(*(Utf8String*)(&val))/*cast due to constrained. prefix*/).Dispose();
			}
		}
	}

	public bool AddCommand(string command, CommandInfo commandInfo, bool isForceToAdd = false)
	{
		if (!isForceToAdd && DalamudServiceExtension.Instance<ICommandManager>().Commands.ContainsKey(command))
		{
			return false;
		}
		RemoveCommand(command);
		DalamudServiceExtension.Instance<ICommandManager>().AddHandler(command, commandInfo);
		Commands[command] = commandInfo;
		return true;
	}

	public bool AddSubCommand(string subCommand, CommandInfo commandInfo, bool isForceToAdd = false)
	{
		if (!isForceToAdd && SubCommands.ContainsKey(subCommand))
		{
			return false;
		}
		SubCommands[subCommand] = commandInfo;
		RefreshCommandDetails();
		return true;
	}

	public bool RemoveCommand(string command)
	{
		if (!DalamudServiceExtension.Instance<ICommandManager>().Commands.ContainsKey(command))
		{
			return false;
		}
		DalamudServiceExtension.Instance<ICommandManager>().RemoveHandler(command);
		Commands.TryRemove(command, out CommandInfo _);
		return true;
	}

	public bool RemoveSubCommand(string subCommand)
	{
		if (!SubCommands.TryRemove(subCommand, out CommandInfo _))
		{
			return false;
		}
		RefreshCommandDetails();
		return true;
	}

	private void RefreshCommandDetails()
	{
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Expected O, but got Unknown
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Expected O, but got Unknown
		if (MainCommand == null)
		{
			return;
		}
		StringBuilder stringBuilder = new StringBuilder(MainCommand.Info.HelpMessage + "\n");
		foreach (KeyValuePair<string, CommandInfo> item in SubCommands.Where<KeyValuePair<string, CommandInfo>>((KeyValuePair<string, CommandInfo> x) => x.Value.ShowInHelp))
		{
			item.Deconstruct(out var key, out var value);
			string value2 = key;
			CommandInfo val = value;
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(4, 3, stringBuilder2);
			handler.AppendFormatted(MainCommand.Command);
			handler.AppendLiteral(" ");
			handler.AppendFormatted(value2);
			handler.AppendLiteral(" → ");
			handler.AppendFormatted(val.HelpMessage);
			stringBuilder2.AppendLine(ref handler);
		}
		RemoveCommand(MainCommand.Command);
		AddCommand(MainCommand.Command, new CommandInfo(new HandlerDelegate(OnMainCommand))
		{
			HelpMessage = stringBuilder.ToString(),
			ShowInHelp = MainCommand.Info.ShowInHelp,
			DisplayOrder = MainCommand.Info.DisplayOrder
		}, isForceToAdd: true);
	}
}
