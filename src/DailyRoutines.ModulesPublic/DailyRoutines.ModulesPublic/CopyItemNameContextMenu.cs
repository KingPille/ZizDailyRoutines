using System;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class CopyItemNameContextMenu : ModuleBase
{
	private sealed class CopyItemNameMenuItem(string name, string nativeName, bool glamour) : ContextMenuEntry
	{
		public override string Identifier => "CopyItemNameContextMenu";

		public unsafe override ContextMenuItem? Create(ContextMenuOpenedArgs args)
		{
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			if (args.InventoryAgentContext == null)
			{
				if (string.IsNullOrWhiteSpace(args.AddonName) || args.AddonName == "FreeCompanyExchange")
				{
					return null;
				}
				AgentContext* defaultAgentContext = args.DefaultAgentContext;
				if (defaultAgentContext != null && ((AgentContext)defaultAgentContext).CurrentContextMenu != null)
				{
					Span<AtkValue> eventParams = ((ContextMenu)((AgentContext)defaultAgentContext).CurrentContextMenu).EventParams;
					int num = Math.Clamp(eventParams[0].Int, 0, eventParams.Length - 8);
					for (int i = 8; i < 8 + num; i++)
					{
						if (string.Equals(((AtkValue)(ref eventParams[i])).GetValueAsString(), nativeName, StringComparison.OrdinalIgnoreCase))
						{
							return null;
						}
					}
				}
			}
			RowRef<Item> item = (glamour ? args.TargetGlamourRow : args.TargetItemRow);
			if (item.RowId == 0)
			{
				return null;
			}
			object obj;
			if (glamour || item.RowId < 2000000)
			{
				Item? valueNullable = item.ValueNullable;
				if (!valueNullable.HasValue)
				{
					obj = null;
				}
				else
				{
					Item valueOrDefault = valueNullable.GetValueOrDefault();
					obj = ((object)((Item)(ref valueOrDefault)).Name/*cast due to constrained. prefix*/).ToString();
				}
			}
			else
			{
				EventItem? valueNullable2 = item.RowId.ToLuminaRowRef<EventItem>().ValueNullable;
				if (!valueNullable2.HasValue)
				{
					obj = null;
				}
				else
				{
					EventItem valueOrDefault2 = valueNullable2.GetValueOrDefault();
					obj = ((object)((EventItem)(ref valueOrDefault2)).Singular/*cast due to constrained. prefix*/).ToString();
				}
			}
			string itemName = (string)obj;
			if (string.IsNullOrWhiteSpace(itemName))
			{
				return null;
			}
			return new ContextMenuItem
			{
				Name = ReadOnlySeString.op_Implicit(name),
				OnClicked = delegate
				{
					//IL_0020: Unknown result type (might be due to invalid IL or missing references)
					((RaptureLogModule)RaptureLogModule.Instance()).ShowLogMessageUInt(1632u, item.RowId);
					ImGui.SetClipboardText(ImU8String.op_Implicit(itemName));
				}
			};
		}
	}

	private CopyItemNameMenuItem menuItem;

	private CopyItemNameMenuItem glamourMenuItem;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("CopyItemNameContextMenuTitle"),
		Description = LanguageManager.Get("CopyItemNameContextMenuDescription"),
		Category = ModuleCategory.System,
		Author = new string[1] { "Nukoooo" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected override void Init()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		string addonText = LuminaWrapper.GetAddonText(159u);
		CircleActivity rowOrDefault = LuminaGetter.GetRowOrDefault<CircleActivity>(18u);
		string text = ((object)((CircleActivity)(ref rowOrDefault)).Name/*cast due to constrained. prefix*/).ToString();
		menuItem = new CopyItemNameMenuItem(addonText, addonText, glamour: false);
		glamourMenuItem = new CopyItemNameMenuItem(addonText + " (" + text + ")", addonText, glamour: true);
		OmenServiceBase<ContextMenuManager>.Instance().Reg(menuItem);
		OmenServiceBase<ContextMenuManager>.Instance().Reg(glamourMenuItem);
	}

	protected override void Uninit()
	{
		OmenServiceBase<ContextMenuManager>.Instance().Unreg(menuItem);
		OmenServiceBase<ContextMenuManager>.Instance().Unreg(glamourMenuItem);
	}
}
