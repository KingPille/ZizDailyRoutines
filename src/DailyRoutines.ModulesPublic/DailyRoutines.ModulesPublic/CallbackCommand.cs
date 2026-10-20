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
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Helpers;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class CallbackCommand : ModuleBase
{
	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static HandlerDelegate _003C0_003E__OnCommand;
	}

	private const string COMMAND = "callback";

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("CallbackCommandTitle"),
		Description = LanguageManager.Get("CallbackCommandDescription", "callback"),
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
		commandManager.AddSubCommand("callback", new CommandInfo((HandlerDelegate)obj)
		{
			HelpMessage = LanguageManager.Get("CallbackCommand-CommandHelp")
		});
	}

	protected override void Uninit()
	{
		OmenServiceBase<CommandManager>.Instance().RemoveSubCommand("callback");
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
			((ImU8String)(ref val2)).AppendFormatted<string>("callback");
			((ImU8String)(ref val2)).AppendLiteral(" ");
			((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get("CallbackCommand-CommandHelp"));
			ImGui.TextUnformatted(val2);
			ImGui.NewLine();
			ImGui.TextWrapped(ImU8String.op_Implicit(LanguageManager.Get("CallbackCommand-CommandHelp-Detailed", "callback")));
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private unsafe static void OnCommand(string command, string arguments)
	{
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		arguments = arguments.Trim();
		if (string.IsNullOrEmpty(arguments))
		{
			NotifyCommandError();
			return;
		}
		string[] array = arguments.Split(' ', 2);
		if (array.Length != 2)
		{
			NotifyCommandError();
			return;
		}
		string text = array[0];
		if (!AddonHelper.TryGetByName(text, out var addonPtr))
		{
			OmenServiceBase<NotifyHelper>.Instance().ChatError(ReadOnlySeString.op_Implicit(LanguageManager.Get("CallbackCommand-Notification-AddonNotReady", text)));
			return;
		}
		string[] inputs = array[1].Split(' ');
		try
		{
			using AtkValueArray atkValues = AtkValueArrayExtension.FromString(inputs);
			(*addonPtr).Callback(in atkValues);
			OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(LanguageManager.Get("CallbackCommand-Notification-ExecuteSuccuess", text, array[1])));
		}
		catch
		{
			OmenServiceBase<NotifyHelper>.Instance().ChatError(ReadOnlySeString.op_Implicit(LanguageManager.Get("CallbackCommand-Notification-ExecuteError", text, array[1])));
		}
		void NotifyCommandError()
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			OmenServiceBase<NotifyHelper>.Instance().ChatError(ReadOnlySeString.op_Implicit(LanguageManager.Get("Commands-InvalidArgs", "/pdr callback", arguments)));
		}
	}
}
