using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Command;
using Dalamud.Hooking;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class AutoDisplayTreasureMapLocation : ModuleBase
{
	private delegate void ShowTreasureMapDelegate(nint agent, ushort rankID, ushort subRowID, byte isJustOpened);

	private sealed class Config : ModuleConfig
	{
		public bool ShowOnDecipher = true;

		public bool ShowOnOpen = true;
	}

	private readonly record struct TreasureMap(string Name, uint MapID, Vector3 Position);

	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static HandlerDelegate _003C0_003E__OnCommand;
	}

	private static readonly CompSig ShowTreasureMapSig = new CompSig("4C 8B DC 55 53 56 49 8D AB ?? ?? ?? ?? 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 85 ?? ?? ?? ?? 49 89 7B ?? 48 8D 45");

	private Hook<ShowTreasureMapDelegate>? ShowTreasureMapHook;

	private Config config;

	private const string COMMAND = "tmap";

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoDisplayTreasureMapLocationTitle"),
		Description = LanguageManager.Get("AutoDisplayTreasureMapLocationDescription"),
		Category = ModuleCategory.System
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected override void Init()
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected O, but got Unknown
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		ShowTreasureMapHook = ShowTreasureMapSig.GetHook<ShowTreasureMapDelegate>(ShowTreasureMapDetour);
		ShowTreasureMapHook.Enable();
		CommandManager commandManager = OmenServiceBase<CommandManager>.Instance();
		object obj = _003C_003EO._003C0_003E__OnCommand;
		if (obj == null)
		{
			HandlerDelegate val = OnCommand;
			_003C_003EO._003C0_003E__OnCommand = val;
			obj = (object)val;
		}
		commandManager.AddSubCommand("tmap", new CommandInfo((HandlerDelegate)obj)
		{
			HelpMessage = LanguageManager.Get("AutoDisplayTreasureMapLocation-CommandHelp")
		});
	}

	protected override void Uninit()
	{
		OmenServiceBase<CommandManager>.Instance().RemoveSubCommand("tmap");
	}

	protected override void ConfigUI()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TextColored(KnownColor.Chocolate.ToUInt(), ImU8String.op_Implicit(LanguageManager.Get("Command")));
		IndentDisposable val = ImRaii.PushIndent(1, true);
		try
		{
			ImU8String val2 = new ImU8String(9, 2);
			((ImU8String)(ref val2)).AppendLiteral("/pdr ");
			((ImU8String)(ref val2)).AppendFormatted<string>("tmap");
			((ImU8String)(ref val2)).AppendLiteral(" -> ");
			((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get("AutoDisplayTreasureMapLocation-CommandHelp"));
			ImGui.TextUnformatted(val2);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		ImGui.NewLine();
		ImGui.TextColored(KnownColor.Chocolate.ToUInt(), ImU8String.op_Implicit(LanguageManager.Get("AutoDisplayTreasureMapLocation-AutoOpen")));
		val = ImRaii.PushIndent(1, true);
		try
		{
			if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("AutoDisplayTreasureMapLocation-AutoOpen-OnDecipher")), ref config.ShowOnDecipher))
			{
				config.Save(this);
			}
			if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("AutoDisplayTreasureMapLocation-AutoOpen-OnOpen")), ref config.ShowOnOpen))
			{
				config.Save(this);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void ShowTreasureMapDetour(nint agent, ushort rankID, ushort subRowID, byte isJustOpened)
	{
		if (((isJustOpened != 0) ? config.ShowOnDecipher : config.ShowOnOpen) && TryGetTreasureMap(rankID, subRowID, out var treasureMap))
		{
			OpenMapLocation(treasureMap);
		}
		else
		{
			ShowTreasureMapHook.Original(agent, rankID, subRowID, isJustOpened);
		}
	}

	private static void OnCommand(string command, string arguments)
	{
		if (TryGetCurrentTreasureMap(out var treasureMap))
		{
			OpenMapLocation(treasureMap);
		}
	}

	private unsafe static bool TryGetCurrentTreasureMap(out TreasureMap treasureMap)
	{
		treasureMap = default(TreasureMap);
		EventItemManager* ptr = EventItemManager.Instance();
		if (ptr == null)
		{
			return false;
		}
		uint treasureHuntRank = ((EventItemManager)ptr).GetTreasureHuntRank();
		if ((treasureHuntRank > 65535 || treasureHuntRank == 0) ? true : false)
		{
			return false;
		}
		return TryGetTreasureMap((ushort)treasureHuntRank, ((EventItemManager)ptr).GetTreasureSpotSubKey(), out treasureMap);
	}

	private static bool TryGetTreasureMap(ushort rankID, ushort subRowID, out TreasureMap treasureMap)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		treasureMap = default(TreasureMap);
		if (!LuminaGetter.TryGetRow<TreasureHuntRank>((uint)rankID, out TreasureHuntRank item) || !LuminaGetter.TryGetSubRow<TreasureSpot>((uint)rankID, out TreasureSpot item2, (int)subRowID))
		{
			return false;
		}
		Level? valueNullable = ((TreasureSpot)(ref item2)).Location.ValueNullable;
		if (valueNullable.HasValue)
		{
			Level level = valueNullable.GetValueOrDefault();
			Map? valueNullable2 = ((Level)(ref level)).Map.ValueNullable;
			if (valueNullable2.HasValue)
			{
				Map valueOrDefault = valueNullable2.GetValueOrDefault();
				EventItem value = ((TreasureHuntRank)(ref item)).KeyItemName.Value;
				string text = ((object)((EventItem)(ref value)).Singular/*cast due to constrained. prefix*/).ToString();
				if (((TreasureHuntRank)(ref item)).InstanceMap.IsValid)
				{
					string text2 = text;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(2, 1);
					defaultInterpolatedStringHandler.AppendLiteral("（");
					value = ((TreasureHuntRank)(ref item)).InstanceMap.Value;
					defaultInterpolatedStringHandler.AppendFormatted<ReadOnlySeString>(((EventItem)(ref value)).Singular);
					defaultInterpolatedStringHandler.AppendLiteral("）");
					text = text2 + defaultInterpolatedStringHandler.ToStringAndClear();
				}
				treasureMap = new TreasureMap(text, ((Map)(ref valueOrDefault)).RowId, level.GetPosition());
				return true;
			}
		}
		return false;
	}

	private unsafe static void OpenMapLocation(TreasureMap treasureMap)
	{
		AgentMap* ptr = AgentMap.Instance();
		if (ptr != null)
		{
			(*ptr).SetMapFlagAndOpen(treasureMap.MapID, treasureMap.Position, treasureMap.Name);
		}
	}
}
