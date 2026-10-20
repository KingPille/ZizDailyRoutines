using System;
using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Text.RegularExpressions.Generated;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.Command;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using OmenTools.Extensions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic.Interface;

public class UnlimitedMacroIcon : ModuleBase
{
	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static HandlerDelegate _003C_003E9__7_0;

		internal void _003CInit_003Eb__7_0(string _, string _)
		{
		}
	}

	private Hook<TryResolveMacroIcon>? TryResolveMacroIconHook;

	private const string COMMAND = "micon";

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("UnlimitedMacroIconTitle"),
		Description = LanguageManager.Get("UnlimitedMacroIconDescription", "micon"),
		Category = ModuleCategory.Interface
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected unsafe override void Init()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Expected O, but got Unknown
		TryResolveMacroIconHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromMemberFunction<TryResolveMacroIcon>(typeof(MemberFunctionPointers), "TryResolveMacroIcon", new TryResolveMacroIcon(TryResolveMacroIconDetour));
		TryResolveMacroIconHook.Enable();
		CommandManager commandManager = OmenServiceBase<CommandManager>.Instance();
		object obj = _003C_003Ec._003C_003E9__7_0;
		if (obj == null)
		{
			HandlerDelegate val = delegate
			{
			};
			_003C_003Ec._003C_003E9__7_0 = val;
			obj = (object)val;
		}
		commandManager.AddSubCommand("micon", new CommandInfo((HandlerDelegate)obj)
		{
			ShowInHelp = false
		});
	}

	protected override void Uninit()
	{
		OmenServiceBase<CommandManager>.Instance().RemoveSubCommand("micon");
	}

	private unsafe bool TryResolveMacroIconDetour(RaptureMacroModule* module, UIModule* uiModule, HotbarSlotType* outType, uint* outRowID, int setID, uint macroID, uint* outItemID)
	{
		Macro* macro = ((RaptureMacroModule)module).GetMacro((uint)setID, macroID);
		if (macro == null)
		{
			return TryResolveMacroIconHook.Original.Invoke(module, uiModule, outType, outRowID, setID, macroID, outItemID);
		}
		for (int i = 0; i < 15; i++)
		{
			Match match = CommandRegex().Match(((object)Unsafe.As<Utf8String, Utf8String>(ref ((Macro)macro).Lines[i])/*cast due to constrained. prefix*/).ToString());
			if (match.Success)
			{
				if (uint.TryParse(match.Groups[1].Value, out var result) && ((Macro)macro).IconId != result)
				{
					((Macro)macro).SetIcon(result);
					((RaptureMacroModule)module).SetSavePendingFlag(true, (uint)setID);
				}
				return false;
			}
		}
		return TryResolveMacroIconHook.Original.Invoke(module, uiModule, outType, outRowID, setID, macroID, outItemID);
	}

	[GeneratedRegex("^\\s*/pdr\\s+micon\\s+(\\d+)\\s*$", RegexOptions.IgnoreCase)]
	[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.42308")]
	private static Regex CommandRegex()
	{
		return _003CRegexGenerator_g_003EFDD7C65F98B3009022F7AC561E347A370A49D71D176A01AA07C89E66F21EBD08F__CommandRegex_2.Instance;
	}
}
