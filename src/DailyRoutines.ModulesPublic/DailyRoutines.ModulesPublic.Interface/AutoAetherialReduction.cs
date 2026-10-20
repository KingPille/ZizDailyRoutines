using System;
using System.Numerics;
using DailyRoutines.Common.Interface.Windows;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using Lumina.Text.ReadOnly;
using OmenTools.Dalamud.Attributes;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;

namespace DailyRoutines.ModulesPublic.Interface;

public class AutoAetherialReduction : ModuleBase
{
	private TextNode? lableNode;

	private TextButtonNode? startButtonNode;

	private TextButtonNode? stopButtonNode;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoAetherialReductionTitle"),
		Description = LanguageManager.Get("AutoAetherialReductionDescription"),
		Category = ModuleCategory.Interface,
		Author = new string[1] { "YLCHEN" }
	};

	[IPCProvider("DailyRoutines.Modules.AutoAetherialReduction.IsBusy")]
	private bool IsCurrentlyBusy => base.TaskHelper?.IsBusy ?? false;

	protected override void Init()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		if (base.TaskHelper == null)
		{
			TaskHelper taskHelper = (base.TaskHelper = new TaskHelper());
		}
		if (base.Overlay == null)
		{
			Overlay overlay = (base.Overlay = new Overlay(this));
		}
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)5, "PurifyItemSelector", new AddonEventDelegate(OnAddonList));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, "PurifyItemSelector", new AddonEventDelegate(OnAddonList));
	}

	protected override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddonList });
		ClearNodes();
	}

	private bool StartReduction()
	{
		if (base.TaskHelper == null)
		{
			return false;
		}
		base.TaskHelper.Enqueue((Func<bool>)StartAetherialReduction, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
		return true;
	}

	private unsafe bool StartAetherialReduction()
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (IsCurrentEnvironmentInvalid())
		{
			return true;
		}
		AgentPurify* ptr = AgentPurify.Instance();
		if (ptr == null || ((AgentPurify)ptr).ReducibleItems.Count == 0)
		{
			base.TaskHelper.Abort();
			return true;
		}
		InventoryManager* ptr2 = InventoryManager.Instance();
		if (ptr2 == null)
		{
			return false;
		}
		if (IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()))
		{
			return false;
		}
		PurifyItem* first = ((AgentPurify)ptr).ReducibleItems.First;
		if (first == null)
		{
			base.TaskHelper.Abort();
			return true;
		}
		InventoryItem* inventorySlot = ((InventoryManager)ptr2).GetInventorySlot(((PurifyItem)first).Inventory, ((PurifyItem)first).Slot);
		if (inventorySlot == null)
		{
			base.TaskHelper.Abort();
			return true;
		}
		((AgentPurify)ptr).ReduceItem(inventorySlot);
		base.TaskHelper.DelayNext(1000);
		base.TaskHelper.Enqueue((Func<bool>)StartAetherialReduction, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
		return true;
	}

	private unsafe void OnAddonList(AddonEvent type, AddonArgs? args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		if ((int)type != 5)
		{
			if ((int)type == 6)
			{
				ClearNodes();
				base.TaskHelper.Abort();
			}
		}
		else
		{
			if (Addons.PurifyItemSelector == null)
			{
				return;
			}
			if (lableNode == null)
			{
				lableNode = new TextNode
				{
					IsVisible = true,
					Position = new Vector2(135f, 8f),
					Size = new Vector2(150f, 28f),
					String = ReadOnlySeString.op_Implicit(Info.Title ?? ""),
					FontSize = 14u,
					AlignmentType = (AlignmentType)5,
					TextFlags = (TextFlags)9
				};
				lableNode.AttachNode(((AtkUnitBase)Addons.PurifyItemSelector).RootNode);
			}
			if (startButtonNode == null)
			{
				startButtonNode = new TextButtonNode
				{
					Position = new Vector2(295f, 10f),
					Size = new Vector2(100f, 28f),
					IsVisible = true,
					String = ReadOnlySeString.op_Implicit(LanguageManager.Get("Start")),
					OnClick = delegate
					{
						StartReduction();
					}
				};
				startButtonNode.AttachNode(((AtkUnitBase)Addons.PurifyItemSelector).RootNode);
			}
			startButtonNode.IsEnabled = !base.TaskHelper.IsBusy;
			if (stopButtonNode == null)
			{
				stopButtonNode = new TextButtonNode
				{
					Position = new Vector2(400f, 10f),
					Size = new Vector2(100f, 28f),
					IsVisible = true,
					String = ReadOnlySeString.op_Implicit(LanguageManager.Get("Stop")),
					OnClick = delegate
					{
						base.TaskHelper.Abort();
					}
				};
				stopButtonNode.AttachNode(((AtkUnitBase)Addons.PurifyItemSelector).RootNode);
			}
		}
	}

	private bool IsCurrentEnvironmentInvalid()
	{
		if (Inventories.Player.IsFull() || DalamudServiceExtension.Instance<ICondition>().Any((ConditionFlag[])(object)new ConditionFlag[2]
		{
			(ConditionFlag)4,
			(ConditionFlag)26
		}))
		{
			base.TaskHelper.Abort();
			return true;
		}
		return false;
	}

	private void ClearNodes()
	{
		lableNode?.Dispose();
		lableNode = null;
		startButtonNode?.Dispose();
		startButtonNode = null;
		stopButtonNode?.Dispose();
		stopButtonNode = null;
	}

	[IPCProvider("DailyRoutines.Modules.AutoAetherialReduction.StartReduction")]
	private bool StartReductionIPC()
	{
		return StartReduction();
	}
}
