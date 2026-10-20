using System;
using System.Linq;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class BetterCollectionCommand : ModuleBase
{
	private string[] validCommands;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("BetterCollectionCommandTitle"),
		Description = LanguageManager.Get("BetterCollectionCommandDescription"),
		Category = ModuleCategory.System
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected override void Init()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		TextCommand rowOrDefault = LuminaGetter.GetRowOrDefault<TextCommand>(24u);
		validCommands = new string[4]
		{
			((object)((TextCommand)(ref rowOrDefault)).Command/*cast due to constrained. prefix*/).ToString(),
			((object)((TextCommand)(ref rowOrDefault)).ShortCommand/*cast due to constrained. prefix*/).ToString(),
			((object)((TextCommand)(ref rowOrDefault)).Alias/*cast due to constrained. prefix*/).ToString(),
			((object)((TextCommand)(ref rowOrDefault)).ShortAlias/*cast due to constrained. prefix*/).ToString()
		}.Where((string x) => !string.IsNullOrEmpty(x)).ToArray();
		OmenServiceBase<ChatManager>.Instance().RegPreExecuteCommandInner(OnCommand);
	}

	protected override void Uninit()
	{
		OmenServiceBase<ChatManager>.Instance().Unreg(OnCommand);
	}

	private unsafe void OnCommand(ref bool isPrevented, ref ReadOnlySeString message)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		string[] array = ((object)Unsafe.As<ReadOnlySeString, ReadOnlySeString>(ref message)/*cast due to constrained. prefix*/).ToString().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		if (array.Length != 2)
		{
			return;
		}
		string text = array[0];
		string value = array[1];
		string[] array2 = validCommands;
		foreach (string text2 in array2)
		{
			if (text != text2)
			{
				continue;
			}
			Enumerator<McGuffin> enumerator = LuminaGetter.Get<McGuffin>().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					McGuffin current = enumerator.Current;
					if (((McGuffin)(ref current)).UIData.RowId != 0)
					{
						McGuffinUIData value2 = ((McGuffin)(ref current)).UIData.Value;
						string text3 = ((object)((McGuffinUIData)(ref value2)).Name/*cast due to constrained. prefix*/).ToString();
						if (!string.IsNullOrEmpty(text3) && text3.Contains(value, StringComparison.OrdinalIgnoreCase))
						{
							((AgentMcGuffin)AgentMcGuffin.Instance()).OpenMcGuffin(((McGuffin)(ref current)).RowId);
							isPrevented = true;
							return;
						}
					}
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
			}
		}
	}
}
