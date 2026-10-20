using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Internal;
using DailyRoutines.Manager;
using Dalamud.Game.Gui.ContextMenu;
using Dalamud.Plugin.Services;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.AddonEvent;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.ModulesPublic.Interface;

public class AutoInventoryTransfer : ModuleBase
{
	private static readonly List<string> MenuTexts;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoInventoryTransferTitle"),
		Description = LanguageManager.Get("AutoInventoryTransferDescription"),
		Category = ModuleCategory.Interface,
		Author = new string[1] { "Yangdoubao" }
	};

	protected override void Init()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 2000
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		DalamudServiceExtension.Instance<IContextMenu>().OnMenuOpened += new OnMenuOpenedDelegate(OnContextMenuOpened);
	}

	protected override void Uninit()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IContextMenu>().OnMenuOpened -= new OnMenuOpenedDelegate(OnContextMenuOpened);
	}

	protected override void ConfigUI()
	{
		ImGuiOmExtension.ConflictKeyText();
	}

	private unsafe void OnContextMenuOpened(IMenuOpenedArgs args)
	{
		if (PluginConfig.Instance().ConflictKeyBinding.IsPressed() && IsInventoryOpen())
		{
			base.TaskHelper.Enqueue(() => (*Addons.ContextMenuAddon).IsAddonAndNodesReady());
			base.TaskHelper.Enqueue(delegate
			{
				AddonContextMenuEvent.Select(MenuTexts);
			});
		}
		unsafe static bool IsInventoryOpen()
		{
			if (!(*Addons.Inventory).IsAddonAndNodesReady() && !(*Addons.InventoryLarge).IsAddonAndNodesReady() && !(*Addons.InventoryExpansion).IsAddonAndNodesReady() && !(*Addons.InventoryRetainer).IsAddonAndNodesReady())
			{
				return (*Addons.InventoryRetainerLarge).IsAddonAndNodesReady();
			}
			return true;
		}
	}

	static AutoInventoryTransfer()
	{
		int num = 4;
		List<string> list = new List<string>(num);
		CollectionsMarshal.SetCount(list, num);
		Span<string> span = CollectionsMarshal.AsSpan(list);
		span[0] = LuminaWrapper.GetAddonText(97u);
		span[1] = LuminaWrapper.GetAddonText(98u);
		span[2] = LuminaWrapper.GetAddonText(881u);
		span[3] = LuminaWrapper.GetAddonText(887u);
		MenuTexts = list;
	}
}
