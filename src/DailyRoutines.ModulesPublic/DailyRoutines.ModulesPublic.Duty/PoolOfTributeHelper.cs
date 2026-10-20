using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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

public class PoolOfTributeHelper : ModuleBase
{
	private ZoneIndicatorHandle? handle;

	private nint gameObject;

	private const uint STATUS_ID = 292u;

	private const uint ZONE_ID = 674u;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("PoolOfTributeHelperTitle"),
		Description = LanguageManager.Get("PoolOfTributeHelperDescription", LuminaWrapper.GetContentName(243u), LuminaWrapper.GetStatusName(292u), LuminaWrapper.GetBNPCName(6224u)),
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
		handle = OmenServiceBase<ZoneIndicatorRenderer>.Instance().RegPermanent(674u, delegate
		{
			if (gameObject != IntPtr.Zero)
			{
				int num = 1;
				List<nint> list = new List<nint>(num);
				CollectionsMarshal.SetCount(list, num);
				CollectionsMarshal.AsSpan(list)[0] = gameObject;
				return list;
			}
			return new List<nint>();
		}, (nint ptr) => Vector3.op_Implicit(((GameObject)ptr).Position), new ZoneIndicatorOptions<nint>
		{
			TextGetter = (nint _) => new ZoneIndicatorText
			{
				Text = ReadOnlySeString.op_Implicit(LuminaWrapper.GetStatusName(292u) ?? ""),
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
		gameObject = IntPtr.Zero;
	}

	private void OnStatusLose(IBattleChara player, ushort id, ushort param, ushort stackCount, ulong sourceID)
	{
		if (id == 292)
		{
			gameObject = IntPtr.Zero;
		}
	}

	private void OnStatusGain(IBattleChara player, ushort id, ushort param, ushort stackCount, TimeSpan remainingTime, ulong sourceID)
	{
		if (id == 292)
		{
			gameObject = player.Address;
		}
	}

	private void OnZoneChanged(uint obj)
	{
		OmenServiceBase<CharacterStatusManager>.Instance().Unreg(OnStatusGain);
		OmenServiceBase<CharacterStatusManager>.Instance().Unreg(OnStatusLose);
		gameObject = IntPtr.Zero;
		if (GameState.TerritoryType == 674)
		{
			OmenServiceBase<CharacterStatusManager>.Instance().RegGain(OnStatusGain);
			OmenServiceBase<CharacterStatusManager>.Instance().RegLose(OnStatusLose);
		}
	}
}
