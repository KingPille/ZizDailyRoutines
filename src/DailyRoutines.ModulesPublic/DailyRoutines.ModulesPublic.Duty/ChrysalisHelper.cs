using System;
using System.Collections.Generic;
using System.Drawing;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Common.Math;
using Lumina.Text.ReadOnly;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.OmenService.ZoneIndicator;

namespace DailyRoutines.ModulesPublic.Duty;

public class ChrysalisHelper : ModuleBase
{
	private ZoneIndicatorHandle? magicHandle;

	private ZoneIndicatorHandle? physicalHandle;

	private ZoneIndicatorHandle? normalHandle;

	private List<nint> magicBalls = new List<nint>();

	private List<nint> physicalBalls = new List<nint>();

	private List<nint> normalBalls = new List<nint>();

	private const uint ZONE_ID = 426u;

	private const uint BALL_FIRST_NAME_ID = 3289u;

	private const uint BALL_SECOND_NAME_ID = 3290u;

	private const uint BALL_THRID_NAME_ID = 3291u;

	private const uint PHYSICAL_DAMAGE_UP_STATUS_ID = 657u;

	private const uint MAGICAL_DAMAGE_DOWN_STATUS_ID = 658u;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("ChrysalisHelperTitle"),
		Description = LanguageManager.Get("ChrysalisHelperDescription"),
		Category = ModuleCategory.Duty
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected override void Init()
	{
		magicHandle = OmenServiceBase<ZoneIndicatorRenderer>.Instance().RegPermanent(426u, () => magicBalls, (nint ptr) => Vector3.op_Implicit(((GameObject)ptr).Position), new ZoneIndicatorOptions<nint>
		{
			TextGetter = (nint _) => new ZoneIndicatorText
			{
				Text = ReadOnlySeString.op_Implicit(LuminaWrapper.GetBNPCName(3291u)),
				TextScale = 1.4f,
				TextColor = KnownColor.MediumPurple.ToVector4()
			}
		});
		physicalHandle = OmenServiceBase<ZoneIndicatorRenderer>.Instance().RegPermanent(426u, () => physicalBalls, (nint ptr) => Vector3.op_Implicit(((GameObject)ptr).Position), new ZoneIndicatorOptions<nint>
		{
			TextGetter = (nint _) => new ZoneIndicatorText
			{
				Text = ReadOnlySeString.op_Implicit(LuminaWrapper.GetBNPCName(3289u)),
				TextScale = 1.4f,
				TextColor = KnownColor.Chocolate.ToVector4()
			}
		});
		normalHandle = OmenServiceBase<ZoneIndicatorRenderer>.Instance().RegPermanent(426u, () => normalBalls, (nint ptr) => Vector3.op_Implicit(((GameObject)ptr).Position), new ZoneIndicatorOptions<nint>
		{
			TextGetter = (nint _) => new ZoneIndicatorText
			{
				Text = ReadOnlySeString.op_Implicit(LuminaWrapper.GetBNPCName(3290u)),
				TextScale = 1.4f,
				TextColor = KnownColor.GreenYellow.ToVector4()
			}
		});
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += OnZoneChanged;
		OnZoneChanged(0u);
	}

	protected override void Uninit()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChanged;
		OmenServiceBase<FrameworkManager>.Instance().Unreg(OnUpdate);
		OmenServiceBase<CharacterStatusManager>.Instance().Unreg(OnGainStatus);
		magicHandle?.Unreg();
		magicHandle = null;
		physicalHandle?.Unreg();
		physicalHandle = null;
		normalHandle?.Unreg();
		normalHandle = null;
	}

	private void OnZoneChanged(uint zone)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Expected O, but got Unknown
		OmenServiceBase<FrameworkManager>.Instance().Unreg(OnUpdate);
		OmenServiceBase<CharacterStatusManager>.Instance().Unreg(OnGainStatus);
		if (GameState.TerritoryType == 426)
		{
			OmenServiceBase<FrameworkManager>.Instance().Reg(new OnUpdateDelegate(OnUpdate), 100u);
			OmenServiceBase<CharacterStatusManager>.Instance().RegGain(OnGainStatus);
		}
	}

	private unsafe void OnUpdate(IFramework framework)
	{
		if (LocalPlayerState.HasStatus(657u, out var index))
		{
			physicalBalls.Clear();
			magicBalls = (*CharacterManager.Instance()).FindAll((delegate*<BattleChara*, bool>)(&TryGetMagicBall));
			normalBalls = (*CharacterManager.Instance()).FindAll((delegate*<BattleChara*, bool>)(&TryGetNormalBall));
		}
		else if (LocalPlayerState.HasStatus(658u, out index))
		{
			magicBalls.Clear();
			physicalBalls = (*CharacterManager.Instance()).FindAll((delegate*<BattleChara*, bool>)(&TryGetPhysicalBall));
			normalBalls = (*CharacterManager.Instance()).FindAll((delegate*<BattleChara*, bool>)(&TryGetNormalBall));
		}
		else
		{
			magicBalls.Clear();
			physicalBalls.Clear();
		}
	}

	private void OnGainStatus(IBattleChara player, ushort id, ushort param, ushort stackCount, TimeSpan remainingTime, ulong sourceID)
	{
		if (player.ContentID == LocalPlayerState.ContentID)
		{
			if (id == 658 || id != 657)
			{
				OnUpdate(null);
			}
		}
	}

	private unsafe static bool TryGetMagicBall(BattleChara* chara)
	{
		if (chara != null && ((BattleChara)chara).NameId == 3291)
		{
			return ((BattleChara)chara).Health != 0;
		}
		return false;
	}

	private unsafe static bool TryGetPhysicalBall(BattleChara* chara)
	{
		if (chara != null && ((BattleChara)chara).NameId == 3289)
		{
			return ((BattleChara)chara).Health != 0;
		}
		return false;
	}

	private unsafe static bool TryGetNormalBall(BattleChara* chara)
	{
		if (chara != null && ((BattleChara)chara).NameId == 3290)
		{
			return ((BattleChara)chara).Health != 0;
		}
		return false;
	}
}
