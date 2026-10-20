using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Command;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using OmenTools.Extensions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class AutoMaximiseWindow : ModuleBase
{
	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static HandlerDelegate _003C0_003E__OnCommand;
	}

	private const string COMMAND = "maxwin";

	private const int SW_SHOWMAXIMIZED = 3;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoMaximiseWindowTitle"),
		Description = LanguageManager.Get("AutoMaximiseWindowDescription", "maxwin"),
		Category = ModuleCategory.System,
		Author = new string[1] { "Bill" }
	};

	protected override void Init()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected O, but got Unknown
		ControlGameWindow(3);
		CommandManager commandManager = OmenServiceBase<CommandManager>.Instance();
		object obj = _003C_003EO._003C0_003E__OnCommand;
		if (obj == null)
		{
			HandlerDelegate val = OnCommand;
			_003C_003EO._003C0_003E__OnCommand = val;
			obj = (object)val;
		}
		commandManager.AddSubCommand("maxwin", new CommandInfo((HandlerDelegate)obj)
		{
			HelpMessage = LanguageManager.Get("AutoMaximiseWindow-CommandHelp")
		});
	}

	protected override void ConfigUI()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(1, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("Command"));
		((ImU8String)(ref val)).AppendLiteral(":");
		ImGui.TextColored(ref vector, val);
		IndentDisposable val2 = ImRaii.PushIndent(1, true);
		try
		{
			ImU8String val3 = new ImU8String(8, 2);
			((ImU8String)(ref val3)).AppendLiteral("/pdr ");
			((ImU8String)(ref val3)).AppendFormatted<string>("maxwin");
			((ImU8String)(ref val3)).AppendLiteral(" → ");
			((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get("AutoMaximiseWindow-CommandHelp"));
			ImGui.TextUnformatted(val3);
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	private static void OnCommand(string command, string args)
	{
		ControlGameWindow(3);
	}

	private unsafe static void ControlGameWindow(int nCmdShow)
	{
		try
		{
			ShowWindow(((GameWindow)((Framework)Framework.Instance()).GameWindow).WindowHandle, nCmdShow);
		}
		catch
		{
		}
	}

	protected override void Uninit()
	{
		OmenServiceBase<CommandManager>.Instance().RemoveSubCommand("maxwin");
	}

	[DllImport("user32.dll")]
	private static extern bool ShowWindow(nint hWnd, int nCmdShow);
}
