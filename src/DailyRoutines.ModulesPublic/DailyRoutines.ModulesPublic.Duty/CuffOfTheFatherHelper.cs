using System.Collections.Generic;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.InstanceContent;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Common.Math;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.STD;
using KamiToolKit.Classes;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.OmenService.ZoneIndicator;

namespace DailyRoutines.ModulesPublic.Duty;

public class CuffOfTheFatherHelper : ModuleBase
{
	private ZoneIndicatorHandle? handle;

	private List<nint> bombObjects = new List<nint>();

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("CuffOfTheFatherHelperTitle"),
		Description = LanguageManager.Get("CuffOfTheFatherHelperDescription", LuminaWrapper.GetContentName(113u), LuminaWrapper.GetBNPCName(3751u), LuminaWrapper.GetBNPCName(2667u)),
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
		handle = OmenServiceBase<ZoneIndicatorRenderer>.Instance().RegPermanent(443u, () => bombObjects, (nint ptr) => Vector3.op_Implicit(((GameObject)ptr).Position), new ZoneIndicatorOptions<nint>
		{
			TextGetter = (nint ptr) => new ZoneIndicatorText
			{
				Text = ReadOnlySeString.op_Implicit("→ " + ((GameObject)ptr).NameString + " ←"),
				TextScale = 1.4f,
				TextColor = ColorHelper.GetColor(518u)
			},
			Surrounding = new ZoneIndicatorSurrounding
			{
				Type = ZoneIndicatorSurrounding.Shape.Circle,
				Radius = 25f,
				Thickness = 4f
			}
		});
	}

	protected override void Uninit()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChanged;
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddon });
		handle?.Unreg();
		handle = null;
		bombObjects.Clear();
	}

	private void OnZoneChanged(uint u)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddon });
		bombObjects.Clear();
		if (GameState.TerritoryType == 443)
		{
			DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)7, "_EnemyList", new AddonEventDelegate(OnAddon));
		}
	}

	private unsafe void OnAddon(AddonEvent type, AddonArgs args)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		NumberArrayData* numberArrayData = ((AtkStage)AtkStage.Instance()).GetNumberArrayData((NumberArrayType)22);
		if (numberArrayData == null)
		{
			return;
		}
		int num = ((NumberArrayData)numberArrayData).IntArray[1];
		if (num < 1)
		{
			return;
		}
		ContentDirector* contentDirector = ((EventFramework)EventFramework.Instance()).GetContentDirector();
		if (contentDirector == null)
		{
			return;
		}
		StdVector<DirectorTodo> directorTodos = ((ContentDirector)contentDirector).DirectorTodos;
		if (directorTodos[2].CurrentCount != 1)
		{
			return;
		}
		List<nint> list = new List<nint>();
		for (int i = 0; i < num; i++)
		{
			int num2 = 8 + i * 6;
			ulong gameObjectID = (ulong)((NumberArrayData)numberArrayData).IntArray[num2];
			ulong num3 = gameObjectID;
			if ((num3 == 0L || num3 == 3758096384u) ? true : false)
			{
				continue;
			}
			BattleChara* ptr = (*CharacterManager.Instance()).FindFirst(delegate(nint num4)
			{
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Invalid comparison between Unknown and I4
				if (num4 == (nint)0)
				{
					return false;
				}
				return ((BattleChara)num4).GetGameObjectId() == GameObjectId.op_Implicit(gameObjectID) && (int)((BattleChara)num4).ObjectKind == 2 && ((BattleChara)num4).BaseId == 3865;
			});
			list.Add((nint)ptr);
			if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4])
			{
				((BattleChara)ptr).TargetableStatus |= (ObjectTargetableFlags)2;
				continue;
			}
			ObjectTargetableFlags* targetableStatus = &((BattleChara)ptr).TargetableStatus;
			*targetableStatus = (ObjectTargetableFlags)((uint)(*targetableStatus) & 0xFDu);
		}
		bombObjects = list;
	}
}
