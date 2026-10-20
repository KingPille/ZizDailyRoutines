using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.Lumina;
using OmenTools.KamiToolKit.Addons;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.ModulesPublic.Interface;

public class AutoStoreToCabinet : ModuleBase
{
	private sealed class AutoStoreToCabinetAddon(AutoStoreToCabinet module) : AttachedAddon("Cabinet")
	{
		private TextButtonNode? startButton;

		private TextButtonNode? stopButton;

		protected override AttachedAddonPosition AttachPosition => AttachedAddonPosition.LeftTop;

		protected unsafe override bool CanOpenAddon
		{
			get
			{
				if (Addons.CabinetAddon != null)
				{
					return (*Addons.CabinetAddon).IsAddonAndNodesReady();
				}
				return false;
			}
		}

		protected unsafe override bool CanCloseHostAddon(AtkUnitBase* hostAddon)
		{
			return false;
		}

		protected unsafe override void OnSetup(AtkUnitBase* addon, Span<AtkValue> atkValues)
		{
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			if (base.WindowNode is WindowNode windowNode)
			{
				windowNode.CloseButtonNode.IsVisible = false;
			}
			FlagHelper.UpdateFlag(ref ((AtkUnitBase)addon).Flags1A1, 4, enable: true);
			FlagHelper.UpdateFlag(ref ((AtkUnitBase)addon).Flags1A0, 128, enable: true);
			FlagHelper.UpdateFlag(ref ((AtkUnitBase)addon).Flags1A1, 64, enable: true);
			FlagHelper.UpdateFlag(ref ((AtkUnitBase)addon).Flags1A3, 1, enable: true);
			VerticalListNode verticalListNode = new VerticalListNode
			{
				IsVisible = true,
				Position = base.ContentStartPosition,
				ItemSpacing = 4f,
				Size = base.ContentSize,
				FitContents = true
			};
			startButton = new TextButtonNode
			{
				IsVisible = true,
				IsEnabled = true,
				Size = new Vector2(base.ContentSize.X - 4f, 32f),
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("Start")),
				OnClick = delegate
				{
					//IL_0011: Unknown result type (might be due to invalid IL or missing references)
					//IL_0016: Unknown result type (might be due to invalid IL or missing references)
					List<uint> itemsToStoreToCabinet = GetItemsToStoreToCabinet();
					Cabinet cabinet = ((UIState)UIState.Instance()).Cabinet;
					foreach (uint item in itemsToStoreToCabinet)
					{
						module.TaskHelper.Enqueue(() => (int)cabinet.State != 1);
						module.TaskHelper.Enqueue(() => ((Cabinet)(ref cabinet)).StoreCabinetItem(item));
					}
				}
			};
			stopButton = new TextButtonNode
			{
				IsVisible = true,
				IsEnabled = true,
				Size = new Vector2(base.ContentSize.X - 4f, 32f),
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("Stop")),
				OnClick = module.TaskHelper.Abort
			};
			verticalListNode.AddNode(startButton);
			verticalListNode.AddNode(stopButton);
			verticalListNode.AttachNode(this);
			verticalListNode.RecalculateLayout();
			SetWindowSize(base.Size.X, base.ContentStartPosition.Y + verticalListNode.Height + 16f);
			verticalListNode.Position = base.ContentStartPosition;
			verticalListNode.Height = verticalListNode.Height;
		}

		protected unsafe override void OnAttachedAddonUpdate(AtkUnitBase* addon, AtkUnitBase* hostAddon)
		{
			RefreshState();
		}

		protected override void OnHostAddon(AddonEvent type, AddonArgs? args)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Invalid comparison between Unknown and I4
			if ((int)type == 6)
			{
				module.TaskHelper.Abort();
			}
		}

		private void RefreshState()
		{
			bool isBusy = module.TaskHelper.IsBusy;
			if (startButton != null)
			{
				startButton.IsEnabled = !isBusy;
			}
			if (stopButton != null)
			{
				stopButton.IsEnabled = isBusy;
			}
		}
	}

	private AutoStoreToCabinetAddon? addon;

	private static readonly FrozenDictionary<uint, uint> CabinetItems = ((IEnumerable<Cabinet>)LuminaGetter.Get<Cabinet>()).Where((Cabinet x) => ((Cabinet)(ref x)).Item.RowId != 0).ToFrozenDictionary((Cabinet x) => ((Cabinet)(ref x)).Item.RowId, (Cabinet x) => ((Cabinet)(ref x)).RowId);

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoStoreToCabinetTitle"),
		Description = LanguageManager.Get("AutoStoreToCabinetDescription"),
		Category = ModuleCategory.Interface
	};

	protected override void Init()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		base.TaskHelper = new TaskHelper();
		if (addon == null)
		{
			addon = new AutoStoreToCabinetAddon(this)
			{
				InternalName = "DRAutoStoreToCabinet",
				Title = ReadOnlySeString.op_Implicit(Info.Title),
				Size = new Vector2(220f, 100f),
				RememberClosePosition = false
			};
		}
	}

	protected override void Uninit()
	{
		addon?.Dispose();
		addon = null;
	}

	private unsafe static List<uint> GetItemsToStoreToCabinet()
	{
		if (!((IEnumerable<InventoryType>)Inventories.Player).TryGetItems((Predicate<InventoryItem>)delegate(InventoryItem x)
		{
			uint baseItemId = ((InventoryItem)(ref x)).GetBaseItemId();
			if (baseItemId == 0)
			{
				return false;
			}
			uint value;
			return CabinetItems.TryGetValue(baseItemId, out value) && !((Cabinet)(&((UIState)UIState.Instance()).Cabinet)).IsItemInCabinet(value);
		}, out List<InventoryItem> itemResult))
		{
			return new List<uint>();
		}
		return itemResult.Select((InventoryItem x) => CabinetItems[((InventoryItem)(ref x)).GetBaseItemId()]).ToList();
	}
}
