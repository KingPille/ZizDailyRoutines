using System;
using System.Collections.Generic;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Common.Math;
using KamiToolKit.Classes;
using Lumina.Text.ReadOnly;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.OmenService.ZoneIndicator;

namespace DailyRoutines.ModulesPublic.Duty;

public class StrikingTreeHelper : ModuleBase
{
	private ZoneIndicatorHandle? handle;

	private List<nint> gameObjects = new List<nint>();

	private const uint STATUS_ID = 66u;

	private const uint ZONE_ID = 374u;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("StrikingTreeHelperTitle"),
		Description = LanguageManager.Get("StrikingTreeHelperDescription", LuminaWrapper.GetContentName(77u), LuminaWrapper.GetStatusName(66u)),
		Category = ModuleCategory.Duty
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected override void Init()
	{
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += OnZoneChanged;
		OnZoneChanged(0u);
		handle = OmenServiceBase<ZoneIndicatorRenderer>.Instance().RegPermanent(374u, () => gameObjects, (nint ptr) => Vector3.op_Implicit(((GameObject)ptr).Position), new ZoneIndicatorOptions<nint>
		{
			TextGetter = (nint _) => new ZoneIndicatorText
			{
				Text = ReadOnlySeString.op_Implicit(LuminaWrapper.GetStatusName(66u) ?? ""),
				TextScale = 1.4f,
				TextColor = ColorHelper.GetColor(518u)
			}
		});
	}

	protected override void Uninit()
	{
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChanged;
		handle?.Unreg();
		handle = null;
		gameObjects = new List<nint>();
	}

	private void OnStatusLose(IBattleChara player, ushort id, ushort param, ushort stackCount, ulong sourceID)
	{
		if (id == 66)
		{
			gameObjects.Remove(player.Address);
		}
	}

	private void OnStatusGain(IBattleChara player, ushort id, ushort param, ushort stackCount, TimeSpan remainingTime, ulong sourceID)
	{
		if (id == 66)
		{
			gameObjects.Add(player.Address);
		}
	}

	private void OnZoneChanged(uint obj)
	{
		OmenServiceBase<CharacterStatusManager>.Instance().Unreg(OnStatusGain);
		OmenServiceBase<CharacterStatusManager>.Instance().Unreg(OnStatusLose);
		gameObjects = new List<nint>();
		if (GameState.TerritoryType == 374)
		{
			OmenServiceBase<CharacterStatusManager>.Instance().RegGain(OnStatusGain);
			OmenServiceBase<CharacterStatusManager>.Instance().RegLose(OnStatusLose);
		}
	}
}
