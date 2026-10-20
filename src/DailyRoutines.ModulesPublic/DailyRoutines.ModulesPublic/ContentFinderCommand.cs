using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Command;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.Sheets;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Helpers;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class ContentFinderCommand : ModuleBase
{
	private sealed class OptionsWrapper
	{
		public ContentsFinderOption Options;
	}

	private enum DutyType
	{
		Normal,
		Roulette,
		Support
	}

	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static HandlerDelegate _003C0_003E__OnCommand;
	}

	private const string COMMAND = "/pdrduty";

	private static readonly FrozenDictionary<string, (DutyType dutyType, string desc)> DutyTypes = new Dictionary<string, (DutyType, string)>(StringComparer.OrdinalIgnoreCase)
	{
		["normal"] = (DutyType.Normal, LanguageManager.Get("ContentFinderCommand-DutyType-Normal")),
		["n"] = (DutyType.Normal, LanguageManager.Get("ContentFinderCommand-DutyType-Normal")),
		["roulette"] = (DutyType.Roulette, LanguageManager.Get("ContentFinderCommand-DutyType-Roulette")),
		["r"] = (DutyType.Roulette, LanguageManager.Get("ContentFinderCommand-DutyType-Roulette")),
		["support"] = (DutyType.Support, LuminaWrapper.GetAddonText(14804u)),
		["s"] = (DutyType.Support, LuminaWrapper.GetAddonText(14804u))
	}.ToFrozenDictionary();

	private static readonly FrozenDictionary<string, (Action<OptionsWrapper> action, string desc)> OptionSetters = new Dictionary<string, (Action<OptionsWrapper>, string)>(StringComparer.OrdinalIgnoreCase)
	{
		["supply"] = (delegate(OptionsWrapper wrapper)
		{
			wrapper.Options.Supply = true;
		}, LuminaWrapper.GetAddonText(2519u)),
		["unrest"] = (delegate(OptionsWrapper wrapper)
		{
			wrapper.Options.UnrestrictedParty = true;
		}, LuminaWrapper.GetAddonText(10008u)),
		["minil"] = (delegate(OptionsWrapper wrapper)
		{
			wrapper.Options.MinimalIL = true;
		}, LuminaWrapper.GetAddonText(10010u)),
		["sync"] = (delegate(OptionsWrapper wrapper)
		{
			wrapper.Options.LevelSync = true;
		}, LuminaWrapper.GetAddonText(12696u)),
		["silence"] = (delegate(OptionsWrapper wrapper)
		{
			wrapper.Options.SilenceEcho = true;
		}, LuminaWrapper.GetAddonText(2266u)),
		["explorer"] = (delegate(OptionsWrapper wrapper)
		{
			wrapper.Options.ExplorerMode = true;
		}, LuminaWrapper.GetAddonText(13038u)),
		["limitleveling"] = (delegate(OptionsWrapper wrapper)
		{
			wrapper.Options.IsLimitedLevelingRoulette = true;
		}, LuminaWrapper.GetAddonText(13030u)),
		["lootgreed"] = (delegate(OptionsWrapper wrapper)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			wrapper.Options.LootRules = (LootRule)1;
		}, LuminaWrapper.GetAddonText(102627u)),
		["lootmaster"] = (delegate(OptionsWrapper wrapper)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			wrapper.Options.LootRules = (LootRule)2;
		}, LuminaWrapper.GetAddonText(11087u)),
		["lootnormal"] = (delegate(OptionsWrapper wrapper)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			wrapper.Options.LootRules = (LootRule)0;
		}, LuminaWrapper.GetAddonText(10100u))
	}.ToFrozenDictionary();

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("ContentFinderCommandTitle"),
		Description = LanguageManager.Get("ContentFinderCommandDescription", "/pdrduty"),
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
		commandManager.AddCommand("/pdrduty", new CommandInfo((HandlerDelegate)obj)
		{
			HelpMessage = LanguageManager.Get("ContentFinderCommand-CommandHelp")
		});
	}

	protected override void Uninit()
	{
		OmenServiceBase<CommandManager>.Instance().RemoveCommand("/pdrduty");
	}

	protected override void ConfigUI()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(1, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("Command"));
		((ImU8String)(ref val)).AppendLiteral(":");
		ImGui.TextColored(ref vector, val);
		ImGui.SameLine();
		ImU8String val2 = default(ImU8String);
		((ImU8String)(ref val2))._002Ector(1, 2);
		((ImU8String)(ref val2)).AppendFormatted<string>("/pdrduty");
		((ImU8String)(ref val2)).AppendLiteral(" ");
		((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get("ContentFinderCommand-CommandHelp"));
		ImGui.TextUnformatted(val2);
		IndentDisposable val3 = ImRaii.PushIndent(1, true);
		try
		{
			ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("ContentFinderCommand-ArgsHelp")));
		}
		finally
		{
			((IDisposable)val3)?.Dispose();
		}
		ImGui.Spacing();
		vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("ContentFinderCommand-DutyType")));
		val3 = ImRaii.PushIndent(1, true);
		try
		{
			RenderTwoRowsTable<string, (DutyType, string)>("DutyType", DutyTypes, (KeyValuePair<string, (DutyType dutyType, string desc)> x) => x.Value.desc);
		}
		finally
		{
			((IDisposable)val3)?.Dispose();
		}
		ImGui.Spacing();
		vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("ContentFinderCommand-Options")));
		val3 = ImRaii.PushIndent(1, true);
		try
		{
			RenderTwoRowsTable<string, (Action<OptionsWrapper>, string)>("Options", OptionSetters, (KeyValuePair<string, (Action<OptionsWrapper> action, string desc)> x) => x.Value.desc);
		}
		finally
		{
			((IDisposable)val3)?.Dispose();
		}
	}

	private static void RenderTwoRowsTable<T1, T2>(string id, IDictionary<T1, T2> source, Func<KeyValuePair<T1, T2>, string> right) where T1 : notnull
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		Vector2 vector = new Vector2(ImGui.GetContentRegionAvail().X * 0.75f, 0f);
		TableDisposable val = ImRaii.Table(ImU8String.op_Implicit(id), 2, (ImGuiTableFlags)1920, vector);
		try
		{
			if (!val)
			{
				return;
			}
			ImGui.TableSetupColumn(ImU8String.op_Implicit("键"), (ImGuiTableColumnFlags)4, 20f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("值"), (ImGuiTableColumnFlags)4, 30f, 0u);
			foreach (KeyValuePair<T1, T2> item in source)
			{
				ImGui.TableNextRow();
				ImGui.TableNextColumn();
				ImU8String val2 = new ImU8String(0, 1);
				((ImU8String)(ref val2)).AppendFormatted<T1>(item.Key);
				ImGui.TextUnformatted(val2);
				ImGuiOmExtension.ClickToCopyAndNotify(item.Key.ToString());
				ImGui.TableNextColumn();
				ImU8String val3 = new ImU8String(0, 1);
				((ImU8String)(ref val3)).AppendFormatted<string>(right(item));
				ImGui.TextUnformatted(val3);
			}
		}
		finally
		{
			((TableDisposable)(ref val)).Dispose();
		}
	}

	private static void OnCommand(string command, string args)
	{
		string[] array = args.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
		int num = array.Length;
		bool flag = (uint)(num - 2) <= 1u;
		if (flag && DutyTypes.TryGetValue(array[0], out (DutyType, string) value) && TryParseContent(array[1], value.Item1, out uint[] contentID))
		{
			ContentsFinderOption options = new ContentsFinderOption
			{
				Config817to820 = true
			};
			if (value.Item1 != DutyType.Normal || array.Length != 3 || TryParseContentSettings(array[2], ref options))
			{
				ExecuteDutyRequest(value.Item1, contentID, options);
			}
		}
	}

	private static void ExecuteDutyRequest(DutyType dutyType, uint[] contentIDs, ContentsFinderOption options)
	{
		switch (dutyType)
		{
		case DutyType.Normal:
			ContentsFinderHelper.RequestDutyNormal(contentIDs, options);
			break;
		case DutyType.Roulette:
			ContentsFinderHelper.RequestDutyRoulette((ushort)contentIDs[0], options);
			break;
		case DutyType.Support:
			ContentsFinderHelper.RequestDutySupport(contentIDs[0]);
			break;
		}
	}

	private static bool TryParseContent(string input, DutyType expectedType, out uint[] contentID)
	{
		contentID = Array.Empty<uint>();
		if (string.IsNullOrWhiteSpace(input))
		{
			return false;
		}
		string[] array = input.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		bool flag = array.Length > 5;
		if (!flag)
		{
			bool flag2 = (uint)(expectedType - 1) <= 1u;
			flag = flag2 && array.Length > 1;
		}
		if (flag)
		{
			return false;
		}
		List<uint> list = new List<uint>();
		string[] array2 = array;
		foreach (string input2 in array2)
		{
			if (TryParseDirectID(input2, expectedType, out var id))
			{
				list.Add(id);
				continue;
			}
			if (TryParseName(input2, expectedType, out var id2))
			{
				list.Add(id2);
				continue;
			}
			return false;
		}
		if (list.Count == 0)
		{
			return false;
		}
		contentID = list.ToArray();
		return true;
	}

	private static bool TryParseDirectID(string input, DutyType expectedType, out uint id)
	{
		if (!uint.TryParse(input, out id))
		{
			return false;
		}
		return expectedType switch
		{
			DutyType.Normal => LuminaGetter.GetRow<ContentFinderCondition>(id).HasValue, 
			DutyType.Roulette => LuminaGetter.GetRow<ContentRoulette>(id).HasValue, 
			DutyType.Support => LuminaGetter.GetRow<DawnContent>(id).HasValue, 
			_ => false, 
		};
	}

	private static bool TryParseName(string input, DutyType expectedType, out uint id)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		id = 0u;
		switch (expectedType)
		{
		case DutyType.Normal:
		{
			ContentFinderCondition val2 = ((IEnumerable<ContentFinderCondition>)LuminaGetter.Get<ContentFinderCondition>()).FirstOrDefault((ContentFinderCondition x) => ((object)((ContentFinderCondition)(ref x)).Name/*cast due to constrained. prefix*/).ToString().Replace(" ", string.Empty).Contains(input, StringComparison.OrdinalIgnoreCase));
			if (((ContentFinderCondition)(ref val2)).RowId != 0)
			{
				id = ((ContentFinderCondition)(ref val2)).RowId;
				return true;
			}
			break;
		}
		case DutyType.Roulette:
		{
			ContentRoulette val3 = ((IEnumerable<ContentRoulette>)LuminaGetter.Get<ContentRoulette>()).FirstOrDefault((ContentRoulette x) => ((object)((ContentRoulette)(ref x)).Name/*cast due to constrained. prefix*/).ToString().Replace(" ", string.Empty).Contains(input, StringComparison.OrdinalIgnoreCase));
			if (((ContentRoulette)(ref val3)).RowId != 0)
			{
				id = ((ContentRoulette)(ref val3)).RowId;
				return true;
			}
			break;
		}
		case DutyType.Support:
		{
			DawnContent val = ((IEnumerable<DawnContent>)LuminaGetter.Get<DawnContent>()).FirstOrDefault(delegate(DawnContent x)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				ContentFinderCondition value = ((DawnContent)(ref x)).Content.Value;
				return ((object)((ContentFinderCondition)(ref value)).Name/*cast due to constrained. prefix*/).ToString().Replace(" ", string.Empty).Contains(input, StringComparison.OrdinalIgnoreCase);
			});
			if (((DawnContent)(ref val)).RowId != 0)
			{
				id = ((DawnContent)(ref val)).RowId;
				return true;
			}
			break;
		}
		}
		return false;
	}

	private static bool TryParseContentSettings(string input, ref ContentsFinderOption options)
	{
		if (string.IsNullOrWhiteSpace(input))
		{
			return true;
		}
		OptionsWrapper optionsWrapper = new OptionsWrapper
		{
			Options = options
		};
		string[] array = input.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		foreach (string key in array)
		{
			if (!OptionSetters.TryGetValue(key, out (Action<OptionsWrapper>, string) value))
			{
				return false;
			}
			value.Item1(optionsWrapper);
		}
		options = optionsWrapper.Options;
		return true;
	}
}
