using System;
using System.Collections.Generic;
using System.Linq;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Command;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Excel.Sheets;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.ModulesPublic;

public class AutoReuseEmote : ModuleBase
{
	private const string COMMAND = "remote";

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoReuseEmoteTitle"),
		Description = LanguageManager.Get("AutoReuseEmoteDescription", "remote", LanguageManager.Get("AutoReuseEmote-CommandHelp")),
		Category = ModuleCategory.System,
		Author = new string[1] { "Xww" }
	};

	protected override void Init()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		base.TaskHelper = new TaskHelper();
		OmenServiceBase<CommandManager>.Instance().AddSubCommand("remote", new CommandInfo(new HandlerDelegate(OnCommand))
		{
			HelpMessage = LanguageManager.Get("AutoReuseEmote-CommandHelp")
		});
	}

	protected override void Uninit()
	{
		OmenServiceBase<CommandManager>.Instance().RemoveSubCommand("remote");
	}

	private void OnCommand(string command, string args)
	{
		base.TaskHelper.Abort();
		args = args.Trim();
		if (string.IsNullOrWhiteSpace(args))
		{
			return;
		}
		string[] array = args.Split(' ');
		int num = array.Length;
		if ((uint)(num - 1) <= 1u)
		{
			string name = array[0];
			int result;
			int interval = ((array.Length == 2 && int.TryParse(array[1], out result)) ? result : 2000);
			if (TryParseEmoteByName(name, out var id))
			{
				UseEmoteByID(id, interval);
			}
		}
	}

	private unsafe static bool TryParseEmoteByName(string name, out ushort id)
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		id = 0;
		name = name.Trim().ToLowerInvariant();
		if (string.IsNullOrWhiteSpace(name))
		{
			return false;
		}
		Emote val = ((IEnumerable<Emote>)LuminaGetter.Get<Emote>()).Where((Emote x) => !string.IsNullOrWhiteSpace(((object)((Emote)(ref x)).Name/*cast due to constrained. prefix*/).ToString()) && ((Emote)(ref x)).TextCommand.ValueNullable.HasValue).FirstOrDefault(delegate(Emote x)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			if (!((object)((Emote)(ref x)).Name/*cast due to constrained. prefix*/).ToString().Equals(name, StringComparison.InvariantCultureIgnoreCase))
			{
				TextCommand value = ((Emote)(ref x)).TextCommand.Value;
				return ((object)((TextCommand)(ref value)).Command/*cast due to constrained. prefix*/).ToString().ToLowerInvariant().Trim('/') == name;
			}
			return true;
		});
		if (((Emote)(ref val)).RowId == 0)
		{
			return false;
		}
		if (((Emote)(ref val)).UnlockLink != 0 && !((UIState)UIState.Instance()).IsUnlockLinkUnlockedOrQuestCompleted(((Emote)(ref val)).UnlockLink, (byte)0, true))
		{
			return false;
		}
		id = (ushort)((Emote)(ref val)).RowId;
		return true;
	}

	private unsafe void UseEmoteByID(ushort id, int interval)
	{
		base.TaskHelper.Enqueue(delegate
		{
			if (((AgentMap)AgentMap.Instance()).IsPlayerMoving || LocalPlayerState.Object == null || IConditionExtension.get_IsBetweenAreas(DalamudServiceExtension.Instance<ICondition>()) || IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()) || DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)26])
			{
				base.TaskHelper.Abort();
			}
			else
			{
				((AgentEmote)AgentEmote.Instance()).ExecuteEmote(id, (PlayEmoteOption*)null, false, false);
			}
		});
		base.TaskHelper.DelayNext(interval);
		base.TaskHelper.Enqueue(delegate
		{
			UseEmoteByID(id, interval);
		});
	}
}
