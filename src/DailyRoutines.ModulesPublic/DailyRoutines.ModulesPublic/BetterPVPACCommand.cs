using System.Collections.Generic;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Component.Shell;
using Lumina.Excel.Sheets;
using OmenTools.Info.Lumina;
using OmenTools.Interop.Game.Models;

namespace DailyRoutines.ModulesPublic;

public class BetterPVPACCommand : ModuleBase
{
	private unsafe delegate long ExecuteCommandDelegate(ShellCommandInterface* commandInterface, CommandContext* context, void* source);

	private static readonly CompSig ShellCommandActionExecuteCommandSig = new CompSig("40 57 41 55 41 56 48 83 EC ?? 45 32 ED");

	private Hook<ExecuteCommandDelegate>? executeCommandHook;

	private readonly Dictionary<string, uint> actionIDsByName = new Dictionary<string, uint>();

	private const ushort PVP_ACTION_COMMAND_ID = 277;

	private const int COMMAND_ARG0_OFFSET = 8;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("BetterPVPACCommandTitle"),
		Description = LanguageManager.Get("BetterPVPACCommandDescription"),
		Category = ModuleCategory.Action,
		ModulesPair = new string[1] { "OptimizedPVPProfileAction" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected unsafe override void Init()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		actionIDsByName.Clear();
		foreach (Action value in Sheets.PVPActions.Values)
		{
			Action current = value;
			string text = ((object)((Action)(ref current)).Name/*cast due to constrained. prefix*/).ToString();
			if (!string.IsNullOrEmpty(text))
			{
				actionIDsByName.TryAdd(text, ((Action)(ref current)).RowId);
			}
		}
		executeCommandHook = ShellCommandActionExecuteCommandSig.GetHook<ExecuteCommandDelegate>(ExecuteCommandDetour);
		executeCommandHook.Enable();
	}

	private unsafe long ExecuteCommandDetour(ShellCommandInterface* commandInterface, CommandContext* context, void* source)
	{
		if (context != null && ((CommandContext)context).TextCommandId == 277)
		{
			ForcePvPAction(context);
		}
		return executeCommandHook.Original(commandInterface, context, source);
	}

	private unsafe void ForcePvPAction(CommandContext* context)
	{
		int* ptr = (int*)context + 2;
		int num = *ptr;
		string text = ((((CommandContext)context).StringArgs.First != null && ((CommandContext)context).StringArgs.First != ((CommandContext)context).StringArgs.Last) ? ((object)(*(Utf8String*)((CommandContext)context).StringArgs.First)/*cast due to constrained. prefix*/).ToString() : null);
		uint value;
		if ((uint)(num - -2) <= 1u)
		{
			if (text == null || !actionIDsByName.TryGetValue(text, out value))
			{
				return;
			}
		}
		else
		{
			value = (uint)(num & 0xFFFFFF);
		}
		if (value != 0)
		{
			*ptr = (int)(0x3000000 | (value & 0xFFFFFF));
		}
	}
}
