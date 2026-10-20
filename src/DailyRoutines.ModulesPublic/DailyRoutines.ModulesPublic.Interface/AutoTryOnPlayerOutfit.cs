using System;
using System.Collections.Generic;
using System.Numerics;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Threading;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;

namespace DailyRoutines.ModulesPublic.Interface;

public class AutoTryOnPlayerOutfit : ModuleBase
{
	private struct TryOnEntry
	{
		public int Slot;

		public uint TryOnItemID;

		public byte Stain0ID;

		public byte Stain1ID;
	}

	private TextButtonNode? tryOnButtonNode;

	private static readonly int[] TryOnSlots = new int[12]
	{
		0, 1, 2, 3, 4, 6, 7, 8, 9, 10,
		11, 12
	};

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoTryOnPlayerOutfitTitle"),
		Description = LanguageManager.Get("AutoTryOnPlayerOutfitDescription"),
		Category = ModuleCategory.Interface,
		Author = new string[1] { "ErxCharlotte" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected override void Init()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 10000
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)5, "CharacterInspect", new AddonEventDelegate(OnAddonList));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, "CharacterInspect", new AddonEventDelegate(OnAddonList));
	}

	protected override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddonList });
		tryOnButtonNode?.Dispose();
		tryOnButtonNode = null;
	}

	private unsafe void OnAddonList(AddonEvent type, AddonArgs? args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		if ((int)type != 5)
		{
			if ((int)type == 6)
			{
				tryOnButtonNode = null;
				base.TaskHelper?.Abort();
			}
		}
		else
		{
			if (Addons.CharacterInspect == null)
			{
				return;
			}
			AtkResNode* nodeById = ((AtkUnitBase)Addons.CharacterInspect).GetNodeById(6u);
			if (nodeById == null)
			{
				return;
			}
			if (tryOnButtonNode == null)
			{
				((AtkResNode)nodeById).SetHeight((ushort)24);
				tryOnButtonNode = new TextButtonNode
				{
					Size = new Vector2((int)((AtkResNode)nodeById).Width, (int)((AtkResNode)nodeById).Height),
					Position = new Vector2(((AtkResNode)nodeById).X, ((AtkResNode)nodeById).Y + (float)(int)((AtkResNode)nodeById).Height + 4f)
				};
				tryOnButtonNode.AttachNode(((AtkUnitBase)Addons.CharacterInspect).RootNode);
			}
			if (!Throttler.Shared.Throttle("AutoTryOnPlayerOutfit-PostDraw"))
			{
				return;
			}
			if (base.TaskHelper.IsBusy)
			{
				tryOnButtonNode.String = ReadOnlySeString.op_Implicit(LanguageManager.Get("Stop"));
				tryOnButtonNode.OnClick = delegate
				{
					base.TaskHelper.Abort();
				};
			}
			else
			{
				tryOnButtonNode.String = ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoTryOnPlayerOutfit-TryOnAll"));
				tryOnButtonNode.OnClick = StartTryOnAll;
			}
		}
	}

	private void StartTryOnAll()
	{
		if (!base.TaskHelper.IsBusy)
		{
			base.TaskHelper.Enqueue((Func<bool>)StartTryOn, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
		}
	}

	private unsafe bool StartTryOn()
	{
		if (IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()) || !(*Addons.CharacterInspect).IsAddonAndNodesReady() || AgentTryon.Instance() == null)
		{
			return false;
		}
		List<TryOnEntry> list = ReadInspectItems();
		if (list == null || list.Count == 0)
		{
			base.TaskHelper.Abort();
			return true;
		}
		((AgentTryon)AgentTryon.Instance()).SaveDeleteOutfit = true;
		foreach (TryOnEntry entry in list)
		{
			base.TaskHelper.Enqueue(delegate
			{
				AgentTryon.TryOn(0u, entry.TryOnItemID, entry.Stain0ID, entry.Stain1ID, 0u, false);
				return true;
			});
			base.TaskHelper.DelayNext(50);
		}
		return true;
	}

	private static List<TryOnEntry>? ReadInspectItems()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		if (!((InventoryType)2009).TryGetItems((InventoryItem x) => x.ItemId != 0 && TryOnSlots.Contains(x.Slot), out List<InventoryItem> itemResult))
		{
			return null;
		}
		List<TryOnEntry> list = new List<TryOnEntry>();
		foreach (InventoryItem item in itemResult)
		{
			InventoryItem current = item;
			uint itemId = current.ItemId;
			uint glamourId = current.GlamourId;
			uint num = ((glamourId != 0) ? glamourId : itemId);
			if (num != 0)
			{
				list.Add(new TryOnEntry
				{
					Slot = current.Slot,
					TryOnItemID = num,
					Stain0ID = ((InventoryItem)(ref current)).Stains[0],
					Stain1ID = ((InventoryItem)(ref current)).Stains[1]
				});
			}
		}
		return list;
	}
}
