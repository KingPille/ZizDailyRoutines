using System;
using System.Collections.Frozen;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.Sheets;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Info.Game.Enums;
using OmenTools.Info.Lumina;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;

namespace DailyRoutines.ModulesPublic;

public class AutoCancelCast : ModuleBase
{
	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static ConditionChangeDelegate _003C0_003E__OnConditionChanged;

		public static OnUpdateDelegate _003C1_003E__OnUpdate;
	}

	private unsafe static readonly FrozenSet<ObjectKind> ValidObjectKinds = FrozenSet.Create(new ReadOnlySpan<ObjectKind>(Unsafe.AsPointer(ref global::_003CPrivateImplementationDetails_003E.A12871FEE210FB8619291EAEA194581CBD2531E4B23759D225F6806923F63222), 2));

	private static readonly FrozenSet<ConditionFlag> ValidConditions = FrozenSet.Create(RuntimeHelpers.CreateSpan<ConditionFlag>((RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/));

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoCancelCastTitle"),
		Description = LanguageManager.Get("AutoCancelCastDescription"),
		Category = ModuleCategory.Action
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected override void Init()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		ICondition obj = DalamudServiceExtension.Instance<ICondition>();
		object obj2 = _003C_003EO._003C0_003E__OnConditionChanged;
		if (obj2 == null)
		{
			ConditionChangeDelegate val = OnConditionChanged;
			_003C_003EO._003C0_003E__OnConditionChanged = val;
			obj2 = (object)val;
		}
		obj.ConditionChange += (ConditionChangeDelegate)obj2;
	}

	protected override void Uninit()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		ICondition obj = DalamudServiceExtension.Instance<ICondition>();
		object obj2 = _003C_003EO._003C0_003E__OnConditionChanged;
		if (obj2 == null)
		{
			ConditionChangeDelegate val = OnConditionChanged;
			_003C_003EO._003C0_003E__OnConditionChanged = val;
			obj2 = (object)val;
		}
		obj.ConditionChange -= (ConditionChangeDelegate)obj2;
		FrameworkManager frameworkManager = OmenServiceBase<FrameworkManager>.Instance();
		OnUpdateDelegate[] array = new OnUpdateDelegate[1];
		object obj3 = _003C_003EO._003C1_003E__OnUpdate;
		if (obj3 == null)
		{
			OnUpdateDelegate val2 = OnUpdate;
			_003C_003EO._003C1_003E__OnUpdate = val2;
			obj3 = (object)val2;
		}
		array[0] = (OnUpdateDelegate)obj3;
		frameworkManager.Unreg((OnUpdateDelegate[])(object)array);
	}

	private static void OnConditionChanged(ConditionFlag flag, bool value)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Expected O, but got Unknown
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		if (!ValidConditions.Contains(flag))
		{
			return;
		}
		if (value)
		{
			FrameworkManager frameworkManager = OmenServiceBase<FrameworkManager>.Instance();
			object obj = _003C_003EO._003C1_003E__OnUpdate;
			if (obj == null)
			{
				OnUpdateDelegate val = OnUpdate;
				_003C_003EO._003C1_003E__OnUpdate = val;
				obj = (object)val;
			}
			frameworkManager.Reg((OnUpdateDelegate)obj);
			return;
		}
		FrameworkManager frameworkManager2 = OmenServiceBase<FrameworkManager>.Instance();
		OnUpdateDelegate[] array = new OnUpdateDelegate[1];
		object obj2 = _003C_003EO._003C1_003E__OnUpdate;
		if (obj2 == null)
		{
			OnUpdateDelegate val2 = OnUpdate;
			_003C_003EO._003C1_003E__OnUpdate = val2;
			obj2 = (object)val2;
		}
		array[0] = (OnUpdateDelegate)obj2;
		frameworkManager2.Unreg((OnUpdateDelegate[])(object)array);
	}

	private unsafe static void OnUpdate(IFramework _)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Invalid comparison between Unknown and I4
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Expected O, but got Unknown
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Expected O, but got Unknown
		if (!IConditionExtension.get_IsCasting(DalamudServiceExtension.Instance<ICondition>()))
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
			return;
		}
		IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
		if (localPlayer == null)
		{
			return;
		}
		if ((int)localPlayer.CastActionType != 1 || Sheets.TargetAreaActions.ContainsKey(localPlayer.CastActionID) || !LuminaGetter.TryGetRow<Action>(localPlayer.CastActionID, out Action item))
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
			return;
		}
		IGameObject castTargetObject = localPlayer.CastTargetObject;
		if (!(castTargetObject is IBattleChara battleChara) || !ValidObjectKinds.Contains(battleChara.ObjectKind))
		{
			FrameworkManager frameworkManager3 = OmenServiceBase<FrameworkManager>.Instance();
			OnUpdateDelegate[] array3 = new OnUpdateDelegate[1];
			object obj3 = _003C_003EO._003C1_003E__OnUpdate;
			if (obj3 == null)
			{
				OnUpdateDelegate val3 = OnUpdate;
				_003C_003EO._003C1_003E__OnUpdate = val3;
				obj3 = (object)val3;
			}
			array3[0] = (OnUpdateDelegate)obj3;
			frameworkManager3.Unreg((OnUpdateDelegate[])(object)array3);
		}
		else if (!battleChara.IsTargetable)
		{
			ExecuteCancast();
		}
		else if (((Action)(ref item)).DeadTargetBehaviour == 0 && (battleChara.IsDead || battleChara.CurrentHp == 0))
		{
			ExecuteCancast();
		}
		else if (!ActionManager.CanUseActionOnTarget(localPlayer.CastActionID, castTargetObject.ToStruct()))
		{
			ExecuteCancast();
		}
		static void ExecuteCancast()
		{
			if (Throttler.Shared.Throttle("AutoCancelCast-CancelCast", 100u))
			{
				OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.CancelCast);
			}
		}
	}
}
