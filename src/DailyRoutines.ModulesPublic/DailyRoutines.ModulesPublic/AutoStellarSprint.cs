using System;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.Sheets;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class AutoStellarSprint : ModuleBase
{
	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static Action<uint> _003C0_003E__OnZoneChange;

		public static OnUpdateDelegate _003C1_003E__OnUpdate;

		public static CharacterStatusManager.LoseStatusDelegate _003C2_003E__OnLoseStatus;
	}

	private const uint STELLAR_SPRINT = 43357u;

	private const uint SPRINT_STATUS = 4398u;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoStellarSprintTitle"),
		Description = LanguageManager.Get("AutoStellarSprintDescription"),
		Category = ModuleCategory.Action,
		Author = new string[1] { "Due" }
	};

	protected override void Init()
	{
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += OnZoneChange;
		OnZoneChange(0u);
	}

	protected override void Uninit()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChange;
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
		OmenServiceBase<CharacterStatusManager>.Instance().Unreg(OnLoseStatus);
	}

	private static void OnZoneChange(uint u)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Invalid comparison between Unknown and I4
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Expected O, but got Unknown
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
		OmenServiceBase<CharacterStatusManager>.Instance().Unreg(OnLoseStatus);
		if ((int)GameState.TerritoryIntendedUse == 60)
		{
			FrameworkManager frameworkManager2 = OmenServiceBase<FrameworkManager>.Instance();
			object obj2 = _003C_003EO._003C1_003E__OnUpdate;
			if (obj2 == null)
			{
				OnUpdateDelegate val2 = OnUpdate;
				_003C_003EO._003C1_003E__OnUpdate = val2;
				obj2 = (object)val2;
			}
			frameworkManager2.Reg((OnUpdateDelegate)obj2, 2000u);
			OmenServiceBase<CharacterStatusManager>.Instance().RegLose(OnLoseStatus);
		}
	}

	private static void OnLoseStatus(IBattleChara player, ushort id, ushort param, ushort stackCount, ulong sourceID)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Invalid comparison between Unknown and I4
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Expected O, but got Unknown
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Expected O, but got Unknown
		if (player.EntityID != LocalPlayerState.EntityID)
		{
			return;
		}
		if ((int)GameState.TerritoryIntendedUse != 60)
		{
			OmenServiceBase<CharacterStatusManager>.Instance().Unreg(OnLoseStatus);
			return;
		}
		OmenServiceBase<CharacterStatusManager>.Instance().Unreg(OnLoseStatus);
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
		frameworkManager2.Reg((OnUpdateDelegate)obj2, 2000u);
	}

	private static void OnUpdate(IFramework _)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Invalid comparison between Unknown and I4
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Expected O, but got Unknown
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Expected O, but got Unknown
		if (IConditionExtension.get_IsBetweenAreas(DalamudServiceExtension.Instance<ICondition>()) || IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()))
		{
			return;
		}
		int index;
		if ((int)GameState.TerritoryIntendedUse != 60)
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
		else if (LocalPlayerState.HasStatus(4398u, out index))
		{
			FrameworkManager frameworkManager2 = OmenServiceBase<FrameworkManager>.Instance();
			OnUpdateDelegate[] array2 = new OnUpdateDelegate[1];
			object obj2 = _003C_003EO._003C1_003E__OnUpdate;
			if (obj2 == null)
			{
				OnUpdateDelegate val2 = OnUpdate;
				_003C_003EO._003C1_003E__OnUpdate = val2;
				obj2 = (object)val2;
			}
			array2[0] = (OnUpdateDelegate)obj2;
			frameworkManager2.Unreg((OnUpdateDelegate[])(object)array2);
			OmenServiceBase<CharacterStatusManager>.Instance().Unreg(OnLoseStatus);
			OmenServiceBase<CharacterStatusManager>.Instance().RegLose(OnLoseStatus);
		}
		else
		{
			ClassJob rowOrDefault = LuminaGetter.GetRowOrDefault<ClassJob>(LocalPlayerState.ClassJob);
			uint rowId = ((ClassJob)(ref rowOrDefault)).ClassJobCategory.RowId;
			if (rowId - 32 <= 1)
			{
				OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)1, 43357u, 3758096384uL, 0u, (UseActionMode)0);
			}
		}
	}
}
