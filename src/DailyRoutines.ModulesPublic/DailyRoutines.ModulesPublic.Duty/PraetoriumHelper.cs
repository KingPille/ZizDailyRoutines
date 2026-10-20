using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic.Duty;

public class PraetoriumHelper : ModuleBase
{
	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static Action<uint> _003C0_003E__OnZoneChanged;

		public static OnUpdateDelegate _003C1_003E__OnUpdate;
	}

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("PraetoriumHelperTitle"),
		Description = LanguageManager.Get("PraetoriumHelperDescription", LuminaWrapper.GetContentName(16u), LuminaWrapper.GetMountName(6u), LuminaWrapper.GetActionName(2434u)),
		Category = ModuleCategory.Duty,
		Author = new string[1] { "逆光" }
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
	}

	private static void OnZoneChanged(uint u)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Expected O, but got Unknown
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
		if (GameState.TerritoryType == 1044)
		{
			FrameworkManager frameworkManager2 = OmenServiceBase<FrameworkManager>.Instance();
			object obj2 = _003C_003EO._003C1_003E__OnUpdate;
			if (obj2 == null)
			{
				OnUpdateDelegate val2 = OnUpdate;
				_003C_003EO._003C1_003E__OnUpdate = val2;
				obj2 = (object)val2;
			}
			frameworkManager2.Reg((OnUpdateDelegate)obj2, 1000u);
		}
	}

	private unsafe static void OnUpdate(IFramework framework)
	{
		if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4] && ((ActionManager)ActionManager.Instance()).GetActionStatus((ActionType)1, 1128u, 3758096384uL, true, true, (uint*)null) == 0)
		{
			IGameObject mostCanTargetObjects = GetMostCanTargetObjects();
			if (mostCanTargetObjects != null)
			{
				OmenServiceBase<UseActionManager>.Instance().UseActionLocation((ActionType)1, 1128u, 3758096384uL, mostCanTargetObjects.Position, 0u, 0);
			}
		}
	}

	private unsafe static IGameObject? GetMostCanTargetObjects()
	{
		List<IGameObject> list = IOmenDalamudService<IObjectTable>.Instance().SearchObjects((IGameObject o) => o.IsTargetable && ActionManager.CanUseActionOnTarget(7u, o.ToStruct()), IObjectTable.CharactersRange).ToList();
		if (list.Count <= 0)
		{
			return null;
		}
		IGameObject result = null;
		int num = 0;
		foreach (IGameObject item in list)
		{
			if (!(Vector3.DistanceSquared(IOmenDalamudService<IObjectTable>.Instance().LocalPlayer.Position, item.Position) - item.HitboxRadius > 900f))
			{
				int targetAoECount = GetTargetAoECount(item, list);
				if (targetAoECount > num)
				{
					num = targetAoECount;
					result = item;
				}
			}
		}
		return result;
	}

	private static int GetTargetAoECount(IGameObject target, IEnumerable<IGameObject> allTarget)
	{
		int num = 0;
		foreach (IGameObject item in allTarget)
		{
			if (Vector3.DistanceSquared(target.Position, item.Position) - item.HitboxRadius <= 36f)
			{
				num++;
			}
		}
		return num;
	}
}
