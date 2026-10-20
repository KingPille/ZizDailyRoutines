using System;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using OmenTools.Extensions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic.Duty;

public class BurnHelper : ModuleBase
{
	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static Action<uint> _003C0_003E__OnZoneChanged;

		public static LogMessageManager.PreInstanceContentTextDelegate _003C1_003E__OnPreContentText;

		public static OnUpdateDelegate _003C2_003E__OnUpdate;

		public static UseActionManager.PreUseActionDelegate _003C3_003E__OnPreUseAction;
	}

	private const uint ZONE_ID = 1173u;

	private const uint VAPORIZE_TEXT_ID = 19708u;

	private const uint COALESCING_TEXT_ID = 19712u;

	private const uint MIST_DRAGON_ID = 9265u;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("BurnHelperTitle"),
		Description = LanguageManager.Get("BurnHelperDescription"),
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
	}

	protected override void Uninit()
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChanged;
		OmenServiceBase<LogMessageManager>.Instance().Unreg(OnPreContentText);
		FrameworkManager frameworkManager = OmenServiceBase<FrameworkManager>.Instance();
		OnUpdateDelegate[] array = new OnUpdateDelegate[1];
		object obj = _003C_003EO._003C2_003E__OnUpdate;
		if (obj == null)
		{
			OnUpdateDelegate val = OnUpdate;
			_003C_003EO._003C2_003E__OnUpdate = val;
			obj = (object)val;
		}
		array[0] = (OnUpdateDelegate)obj;
		frameworkManager.Unreg((OnUpdateDelegate[])(object)array);
		OmenServiceBase<UseActionManager>.Instance().Unreg(OnPreUseAction);
	}

	private static void OnZoneChanged(uint zone)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		OmenServiceBase<LogMessageManager>.Instance().Unreg(OnPreContentText);
		FrameworkManager frameworkManager = OmenServiceBase<FrameworkManager>.Instance();
		OnUpdateDelegate[] array = new OnUpdateDelegate[1];
		object obj = _003C_003EO._003C2_003E__OnUpdate;
		if (obj == null)
		{
			OnUpdateDelegate val = OnUpdate;
			_003C_003EO._003C2_003E__OnUpdate = val;
			obj = (object)val;
		}
		array[0] = (OnUpdateDelegate)obj;
		frameworkManager.Unreg((OnUpdateDelegate[])(object)array);
		if (GameState.TerritoryType == 1173)
		{
			OmenServiceBase<LogMessageManager>.Instance().RegPreInstanceContentText(OnPreContentText);
		}
	}

	private unsafe static void OnPreContentText(ref bool isPrevented, ref uint rowID)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Expected O, but got Unknown
		switch (rowID)
		{
		case 19708u:
		{
			FrameworkManager frameworkManager2 = OmenServiceBase<FrameworkManager>.Instance();
			object obj2 = _003C_003EO._003C2_003E__OnUpdate;
			if (obj2 == null)
			{
				OnUpdateDelegate val2 = OnUpdate;
				_003C_003EO._003C2_003E__OnUpdate = val2;
				obj2 = (object)val2;
			}
			frameworkManager2.Reg((OnUpdateDelegate)obj2);
			OmenServiceBase<UseActionManager>.Instance().RegPreUseAction(OnPreUseAction);
			break;
		}
		case 19712u:
		{
			FrameworkManager frameworkManager = OmenServiceBase<FrameworkManager>.Instance();
			OnUpdateDelegate[] array = new OnUpdateDelegate[1];
			object obj = _003C_003EO._003C2_003E__OnUpdate;
			if (obj == null)
			{
				OnUpdateDelegate val = OnUpdate;
				_003C_003EO._003C2_003E__OnUpdate = val;
				obj = (object)val;
			}
			array[0] = (OnUpdateDelegate)obj;
			frameworkManager.Unreg((OnUpdateDelegate[])(object)array);
			OmenServiceBase<UseActionManager>.Instance().Unreg(OnPreUseAction);
			BattleChara* ptr = (*CharacterManager.Instance()).FindFirst((delegate*<BattleChara*, bool>)(&TryFindMistDragon));
			if (ptr != null)
			{
				((BattleChara)ptr).TargetableStatus |= (ObjectTargetableFlags)2;
			}
			break;
		}
		}
	}

	private unsafe static void OnPreUseAction(ref bool isPrevented, ref ActionType actionType, ref uint actionID, ref ulong targetID, ref uint extraParam, ref UseActionMode queueState, ref uint comboRouteID)
	{
		BattleChara* ptr = (*CharacterManager.Instance()).FindFirst((delegate*<BattleChara*, bool>)(&TryFindMistDragon));
		if (ptr != null && ((BattleChara)ptr).EntityId == targetID)
		{
			isPrevented = true;
		}
	}

	private unsafe static void OnUpdate(IFramework framework)
	{
		BattleChara* ptr = (*CharacterManager.Instance()).FindFirst((delegate*<BattleChara*, bool>)(&TryFindMistDragon));
		if (ptr != null)
		{
			ObjectTargetableFlags* targetableStatus = &((BattleChara)ptr).TargetableStatus;
			*targetableStatus = (ObjectTargetableFlags)((uint)(*targetableStatus) & 0xFDu);
		}
	}

	private unsafe static bool TryFindMistDragon(BattleChara* chara)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		if ((int)((BattleChara)chara).ObjectKind == 2)
		{
			return ((BattleChara)chara).BaseId == 9265;
		}
		return false;
	}
}
