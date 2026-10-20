using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.Sheets;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.ModulesPublic.CraftGather;

public class AutoUseCrafterGathererManual : ModuleBase
{
	private class Config : ModuleConfig
	{
		public bool SendNotification = true;
	}

	private Config config;

	private static readonly FrozenSet<ConditionFlag> ValidConditions = FrozenSet.Create(RuntimeHelpers.CreateSpan<ConditionFlag>((RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/));

	private static readonly FrozenSet<uint> GathererManuals = FrozenSet.Create<uint>(26553u, 12668u, 4635u, 4633u);

	private static readonly FrozenSet<uint> CrafterManuals = FrozenSet.Create<uint>(26554u, 12667u, 4634u, 4632u);

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoUseCrafterGathererManualTitle"),
		Description = LanguageManager.Get("AutoUseCrafterGathererManualDescription"),
		Category = ModuleCategory.CraftGather,
		Author = new string[2] { "Shiyuvi", "ziz" }
	};

	protected override void Init()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Expected O, but got Unknown
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Expected O, but got Unknown
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 15000
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		DalamudServiceExtension.Instance<ICondition>().ConditionChange += new ConditionChangeDelegate(OnConditionChanged);
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += OnZoneChanged;
		DalamudServiceExtension.Instance<IClientState>().ClassJobChanged += new ClassJobChangeDelegate(OnClassJobChanged);
		DalamudServiceExtension.Instance<IClientState>().LevelChanged += new LevelChangeDelegate(OnLevelChanged);
		EnqueueCheck();
	}

	protected override void Uninit()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		DalamudServiceExtension.Instance<ICondition>().ConditionChange -= new ConditionChangeDelegate(OnConditionChanged);
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChanged;
		DalamudServiceExtension.Instance<IClientState>().ClassJobChanged -= new ClassJobChangeDelegate(OnClassJobChanged);
		DalamudServiceExtension.Instance<IClientState>().LevelChanged -= new LevelChangeDelegate(OnLevelChanged);
	}

	protected override void ConfigUI()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("SendNotification")), ref config.SendNotification))
		{
			config.Save(this);
		}
	}

	private void OnConditionChanged(ConditionFlag flag, bool value)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (!value && ValidConditions.Contains(flag))
		{
			EnqueueCheck();
		}
	}

	private void OnZoneChanged(uint u)
	{
		EnqueueCheck();
	}

	private void OnLevelChanged(uint classJobID, uint level)
	{
		EnqueueCheck();
	}

	private void OnClassJobChanged(uint classJobID)
	{
		EnqueueCheck();
	}

	private unsafe void EnqueueCheck()
	{
		base.TaskHelper.Abort();
		base.TaskHelper.Enqueue(delegate
		{
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
			if (localPlayer == null)
			{
				return false;
			}
			if (localPlayer.Level >= ((PlayerState)PlayerState.Instance()).MaxLevel)
			{
				return true;
			}
			if (IConditionExtension.get_IsBetweenAreas(DalamudServiceExtension.Instance<ICondition>()) || IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()) || IConditionExtension.get_IsCasting(DalamudServiceExtension.Instance<ICondition>()) || !UIModuleExtension.IsScreenReady() || ((ActionManager)ActionManager.Instance()).GetActionStatus((ActionType)5, 2u, 3758096384uL, true, true, (uint*)null) != 0)
			{
				return false;
			}
			bool flag = localPlayer.ClassJob.Value.ToJobType() == ClassJobType.Gatherer;
			bool flag2 = localPlayer.ClassJob.Value.ToJobType() == ClassJobType.Crafter;
			if (!flag && !flag2)
			{
				return true;
			}
			StatusManager statusManager = ((BattleChara)localPlayer.ToStruct()).StatusManager;
			if (((StatusManager)(ref statusManager)).GetStatusIndex(flag ? 46u : 45u, 3758096384u) != -1)
			{
				return true;
			}
			uint num = 0u;
			if (flag && TryGetFirstValidItem(GathererManuals, out var itemID))
			{
				num = itemID;
			}
			if (flag2 && TryGetFirstValidItem(CrafterManuals, out var itemID2))
			{
				num = itemID2;
			}
			if (num == 0 || !LuminaGetter.TryGetRow<Item>(num, out Item item))
			{
				return true;
			}
			OmenServiceBase<UseActionManager>.Instance().UseActionLocation((ActionType)2, num, 3758096384uL, default(Vector3), 65535u, 0);
			if (config.SendNotification)
			{
				OmenServiceBase<NotifyHelper>.Instance().NotificationInfo(LanguageManager.Get("AutoUseCrafterGathererManual-Notification", ((object)((Item)(ref item)).Name/*cast due to constrained. prefix*/).ToString()));
			}
			return true;
		});
	}

	private unsafe static bool TryGetFirstValidItem(IEnumerable<uint> items, out uint itemID)
	{
		itemID = 0u;
		InventoryManager* ptr = InventoryManager.Instance();
		if (ptr == null)
		{
			return false;
		}
		foreach (uint item in items)
		{
			if (((InventoryManager)ptr).GetInventoryItemCount(item, false, true, true, (short)0) + ((InventoryManager)ptr).GetInventoryItemCount(item, true, true, true, (short)0) != 0)
			{
				itemID = item;
				return true;
			}
		}
		return false;
	}
}
