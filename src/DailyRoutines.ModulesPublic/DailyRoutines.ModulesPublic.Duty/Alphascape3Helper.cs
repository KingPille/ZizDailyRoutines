using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic.Duty;

public class Alphascape3Helper : ModuleBase
{
	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static Action<uint> _003C0_003E__OnZoneChanged;

		public static OnUpdateDelegate _003C1_003E__OnUpdate;

		public static UseActionManager.PostCharacterStartCastDelegate _003C2_003E__OnStartCast;

		public static UseActionManager.PostCharacterCompleteCastDelegate _003C3_003E__OnCompleteCast;
	}

	private const uint DUTY_ZONE_ID = 800u;

	private const uint TRIANGLE_ATTACK_ACTION_ID = 12923u;

	private const uint POINT_DATA_ID = 9638u;

	private const uint QUEST_ACTION_ID = 12911u;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("Alphascape3HelperTitle"),
		Description = LanguageManager.Get("Alphascape3HelperDescription", LuminaWrapper.GetContentName(589u), LuminaWrapper.GetBNPCName(7852u), LuminaWrapper.GetActionName(12911u)),
		Category = ModuleCategory.Duty
	};

	protected override void Init()
	{
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += OnZoneChanged;
		OnZoneChanged(0u);
	}

	protected override void Uninit()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChanged;
		FrameworkManager frameworkManager = OmenServiceBase<FrameworkManager>.Instance();
		OnUpdateDelegate[] array = new OnUpdateDelegate[1];
		object obj = _003C_003EO._003C1_003E__OnUpdate;
		if (obj == null)
		{
			OnUpdateDelegate val = OnUpdate;
			_003C_003EO._003C1_003E__OnUpdate = val;
			obj = (object)val;
		}
		array[0] = (OnUpdateDelegate)obj;
		frameworkManager.Unreg((OnUpdateDelegate[])(object)array);
		OmenServiceBase<UseActionManager>.Instance().Unreg(OnStartCast);
		OmenServiceBase<UseActionManager>.Instance().Unreg(OnCompleteCast);
	}

	private static void OnZoneChanged(uint u)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		FrameworkManager frameworkManager = OmenServiceBase<FrameworkManager>.Instance();
		OnUpdateDelegate[] array = new OnUpdateDelegate[1];
		object obj = _003C_003EO._003C1_003E__OnUpdate;
		if (obj == null)
		{
			OnUpdateDelegate val = OnUpdate;
			_003C_003EO._003C1_003E__OnUpdate = val;
			obj = (object)val;
		}
		array[0] = (OnUpdateDelegate)obj;
		frameworkManager.Unreg((OnUpdateDelegate[])(object)array);
		OmenServiceBase<UseActionManager>.Instance().Unreg(OnStartCast);
		OmenServiceBase<UseActionManager>.Instance().Unreg(OnCompleteCast);
		if (GameState.TerritoryType == 800)
		{
			OmenServiceBase<UseActionManager>.Instance().RegPostCharacterStartCast(OnStartCast);
			OmenServiceBase<UseActionManager>.Instance().RegPostCharacterCompleteCast(OnCompleteCast);
		}
	}

	private static void OnCompleteCast(bool result, IBattleChara player, ActionType type, uint actionID, uint spellID, GameObjectId animationTargetID, Vector3 location, float rotation, short lastUsedActionSequence, int animationVariation, int ballistaEntityID)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		if (actionID == 12923)
		{
			FrameworkManager frameworkManager = OmenServiceBase<FrameworkManager>.Instance();
			OnUpdateDelegate[] array = new OnUpdateDelegate[1];
			object obj = _003C_003EO._003C1_003E__OnUpdate;
			if (obj == null)
			{
				OnUpdateDelegate val = OnUpdate;
				_003C_003EO._003C1_003E__OnUpdate = val;
				obj = (object)val;
			}
			array[0] = (OnUpdateDelegate)obj;
			frameworkManager.Unreg((OnUpdateDelegate[])(object)array);
		}
	}

	private static void OnStartCast(bool result, IBattleChara player, ActionType type, uint actionID, nint a4, float rotation, float a6)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		if (actionID == 12923)
		{
			FrameworkManager frameworkManager = OmenServiceBase<FrameworkManager>.Instance();
			OnUpdateDelegate[] array = new OnUpdateDelegate[1];
			object obj = _003C_003EO._003C1_003E__OnUpdate;
			if (obj == null)
			{
				OnUpdateDelegate val = OnUpdate;
				_003C_003EO._003C1_003E__OnUpdate = val;
				obj = (object)val;
			}
			array[0] = (OnUpdateDelegate)obj;
			frameworkManager.Unreg((OnUpdateDelegate[])(object)array);
			FrameworkManager frameworkManager2 = OmenServiceBase<FrameworkManager>.Instance();
			object obj2 = _003C_003EO._003C1_003E__OnUpdate;
			if (obj2 == null)
			{
				OnUpdateDelegate val2 = OnUpdate;
				_003C_003EO._003C1_003E__OnUpdate = val2;
				obj2 = (object)val2;
			}
			frameworkManager2.Reg((OnUpdateDelegate)obj2, 500u);
		}
	}

	private unsafe static void OnUpdate(IFramework framework)
	{
		BattleChara* ptr = (*CharacterManager.Instance()).FindFirst((delegate*<BattleChara*, bool>)(&FindPoint));
		if (ptr != null)
		{
			OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)1, 12911u, ((BattleChara)ptr).EntityId, 0u, (UseActionMode)0);
		}
		unsafe static bool FindPoint(BattleChara* chara)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Invalid comparison between Unknown and I4
			if (chara != null && (int)((BattleChara)chara).ObjectKind == 2)
			{
				return ((BattleChara)chara).BaseId == 9638;
			}
			return false;
		}
	}
}
