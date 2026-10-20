using System;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class AutoCancelStarContributor : ModuleBase
{
	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static Action<uint> _003C0_003E__OnZoneChanged;

		public static ClassJobChangeDelegate _003C1_003E__OnClassJobChanged;

		public static OnUpdateDelegate _003C2_003E__OnUpdate;
	}

	private const uint STAR_CONTRIBUTOR_BUFF_ID = 4409u;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoCancelStarContributorTitle"),
		Description = LanguageManager.Get("AutoCancelStarContributorDescription"),
		Category = ModuleCategory.General,
		Author = new string[1] { "Shiyuvi" }
	};

	protected override void Init()
	{
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += OnZoneChanged;
		OnZoneChanged(0u);
	}

	protected override void Uninit()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected O, but got Unknown
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChanged;
		IClientState obj = DalamudServiceExtension.Instance<IClientState>();
		object obj2 = _003C_003EO._003C1_003E__OnClassJobChanged;
		if (obj2 == null)
		{
			ClassJobChangeDelegate val = OnClassJobChanged;
			_003C_003EO._003C1_003E__OnClassJobChanged = val;
			obj2 = (object)val;
		}
		obj.ClassJobChanged -= (ClassJobChangeDelegate)obj2;
		FrameworkManager frameworkManager = OmenServiceBase<FrameworkManager>.Instance();
		OnUpdateDelegate[] array = new OnUpdateDelegate[1];
		object obj3 = _003C_003EO._003C2_003E__OnUpdate;
		if (obj3 == null)
		{
			OnUpdateDelegate val2 = OnUpdate;
			_003C_003EO._003C2_003E__OnUpdate = val2;
			obj3 = (object)val2;
		}
		array[0] = (OnUpdateDelegate)obj3;
		frameworkManager.Unreg((OnUpdateDelegate[])(object)array);
	}

	private static void OnZoneChanged(uint u)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Invalid comparison between Unknown and I4
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected O, but got Unknown
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Expected O, but got Unknown
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Expected O, but got Unknown
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
		IClientState obj2 = DalamudServiceExtension.Instance<IClientState>();
		object obj3 = _003C_003EO._003C1_003E__OnClassJobChanged;
		if (obj3 == null)
		{
			ClassJobChangeDelegate val2 = OnClassJobChanged;
			_003C_003EO._003C1_003E__OnClassJobChanged = val2;
			obj3 = (object)val2;
		}
		obj2.ClassJobChanged -= (ClassJobChangeDelegate)obj3;
		if ((int)GameState.TerritoryIntendedUse == 60)
		{
			FrameworkManager frameworkManager2 = OmenServiceBase<FrameworkManager>.Instance();
			object obj4 = _003C_003EO._003C2_003E__OnUpdate;
			if (obj4 == null)
			{
				OnUpdateDelegate val3 = OnUpdate;
				_003C_003EO._003C2_003E__OnUpdate = val3;
				obj4 = (object)val3;
			}
			frameworkManager2.Reg((OnUpdateDelegate)obj4, 10000u);
			IClientState obj5 = DalamudServiceExtension.Instance<IClientState>();
			object obj6 = _003C_003EO._003C1_003E__OnClassJobChanged;
			if (obj6 == null)
			{
				ClassJobChangeDelegate val4 = OnClassJobChanged;
				_003C_003EO._003C1_003E__OnClassJobChanged = val4;
				obj6 = (object)val4;
			}
			obj5.ClassJobChanged += (ClassJobChangeDelegate)obj6;
		}
	}

	private static void OnClassJobChanged(uint classJobID)
	{
		OnUpdate(DalamudServiceExtension.Instance<IFramework>());
	}

	private unsafe static void OnUpdate(IFramework framework)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		if ((int)GameState.TerritoryIntendedUse != 60)
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
		}
		else
		{
			if (IConditionExtension.get_IsBetweenAreas(DalamudServiceExtension.Instance<ICondition>()))
			{
				return;
			}
			IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
			if (localPlayer != null)
			{
				StatusManager statusManager = ((BattleChara)localPlayer.ToStruct()).StatusManager;
				if (((StatusManager)(ref statusManager)).HasStatus(4409u, 3758096384u))
				{
					StatusManager.ExecuteStatusOff(4409u, 3758096384u);
				}
			}
		}
	}
}
