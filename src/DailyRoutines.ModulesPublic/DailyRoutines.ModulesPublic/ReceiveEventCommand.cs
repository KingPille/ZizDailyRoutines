using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Extensions;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Command;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class ReceiveEventCommand : ModuleBase
{
	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static HandlerDelegate _003C0_003E__OnCommand;
	}

	private const string COMMAND = "agentevent";

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("ReceiveEventCommandTitle"),
		Description = LanguageManager.Get("ReceiveEventCommandDescription", "agentevent"),
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
		commandManager.AddSubCommand("agentevent", new CommandInfo((HandlerDelegate)obj)
		{
			HelpMessage = LanguageManager.Get("ReceiveEventCommand-CommandHelp")
		});
	}

	protected override void Uninit()
	{
		OmenServiceBase<CommandManager>.Instance().RemoveSubCommand("agentevent");
	}

	protected override void ConfigUI()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("Command")));
		IndentDisposable val = ImRaii.PushIndent(1, true);
		try
		{
			ImU8String val2 = new ImU8String(6, 2);
			((ImU8String)(ref val2)).AppendLiteral("/pdr ");
			((ImU8String)(ref val2)).AppendFormatted<string>("agentevent");
			((ImU8String)(ref val2)).AppendLiteral(" ");
			((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get("ReceiveEventCommand-CommandHelp"));
			ImGui.TextUnformatted(val2);
			ImGui.NewLine();
			ImGui.TextWrapped(ImU8String.op_Implicit(LanguageManager.Get("ReceiveEventCommand-CommandHelp-Detailed", "agentevent")));
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private unsafe static void OnCommand(string command, string arguments)
	{
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		arguments = arguments.Trim();
		if (string.IsNullOrEmpty(arguments))
		{
			NotifyCommandError();
			return;
		}
		string[] array = arguments.Split(' ', 3, StringSplitOptions.RemoveEmptyEntries);
		if (array.Length < 2)
		{
			NotifyCommandError();
			return;
		}
		string text = array[0];
		if (!Enum.TryParse<AgentId>(text, true, out AgentId result) || !Enum.IsDefined<AgentId>(result))
		{
			OmenServiceBase<NotifyHelper>.Instance().ChatError(ReadOnlySeString.op_Implicit(LanguageManager.Get("ReceiveEventCommand-Notification-AgentNotReady", text)));
			return;
		}
		if (!ulong.TryParse(array[1], out var result2))
		{
			OmenServiceBase<NotifyHelper>.Instance().ChatError(ReadOnlySeString.op_Implicit(LanguageManager.Get("ReceiveEventCommand-Notification-InvalidEventKind", array[1])));
			return;
		}
		AgentInterface* agentByInternalId = ((AgentModule)AgentModule.Instance()).GetAgentByInternalId(result);
		if (agentByInternalId == null)
		{
			OmenServiceBase<NotifyHelper>.Instance().ChatError(ReadOnlySeString.op_Implicit(LanguageManager.Get("ReceiveEventCommand-Notification-AgentNotReady", result)));
			return;
		}
		string text2 = ((array.Length > 2) ? array[2] : string.Empty);
		string[] inputs = (string.IsNullOrWhiteSpace(text2) ? Array.Empty<string>() : text2.Split(' ', StringSplitOptions.RemoveEmptyEntries));
		try
		{
			using AtkValueArray valueArray = AtkValueArrayExtension.FromString(inputs);
			(*agentByInternalId).SendEvent(result2, in valueArray);
			OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(LanguageManager.Get("ReceiveEventCommand-Notification-ExecuteSuccess", result, result2, text2)));
		}
		catch
		{
			OmenServiceBase<NotifyHelper>.Instance().ChatError(ReadOnlySeString.op_Implicit(LanguageManager.Get("ReceiveEventCommand-Notification-ExecuteError", result, result2, text2)));
		}
		void NotifyCommandError()
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			OmenServiceBase<NotifyHelper>.Instance().ChatError(ReadOnlySeString.op_Implicit(LanguageManager.Get("Commands-InvalidArgs", "/pdr agentevent", arguments)));
		}
	}
}
