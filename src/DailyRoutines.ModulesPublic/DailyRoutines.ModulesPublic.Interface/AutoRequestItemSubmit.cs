using System.Collections.Frozen;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Internal;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.AddonEvent;
using OmenTools.Interop.Game.Lumina;

namespace DailyRoutines.ModulesPublic.Interface;

public class AutoRequestItemSubmit : ModuleBase
{
	private class Config : ModuleConfig
	{
		public bool IsSubmitHQItem = true;
	}

	private Config config;

	private static readonly FrozenSet<string> HQItemTexts;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoRequestItemSubmitTitle"),
		Description = LanguageManager.Get("AutoRequestItemSubmitDescription"),
		Category = ModuleCategory.Interface
	};

	protected override void Init()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)1, "Request", new AddonEventDelegate(OnAddonRequest));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)5, "Request", new AddonEventDelegate(OnAddonRequest));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, "Request", new AddonEventDelegate(OnAddonRequest));
	}

	protected override void ConfigUI()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		ImGuiOmExtension.ConflictKeyText();
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("AutoRequestItemSubmit-SubmitHQItem")), ref config.IsSubmitHQItem))
		{
			config.Save(this);
		}
	}

	private void OnAddonRequest(AddonEvent type, AddonArgs args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Expected O, but got Unknown
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Invalid comparison between Unknown and I4
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Expected O, but got Unknown
		if ((int)type != 1)
		{
			if ((int)type != 5)
			{
				if ((int)type == 6)
				{
					DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddonSelectYesno });
				}
			}
			else
			{
				OperateOnRequest();
			}
		}
		else
		{
			DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)1, "SelectYesno", new AddonEventDelegate(OnAddonSelectYesno));
		}
	}

	private unsafe void OnAddonSelectYesno(AddonEvent type, AddonArgs args)
	{
		if (config.IsSubmitHQItem)
		{
			string item = ((object)(*(Utf8String*)(&((AtkTextNode)((AddonSelectYesno)Addons.SelectYesno).PromptText).NodeText))/*cast due to constrained. prefix*/).ToString();
			if (HQItemTexts.Contains(item))
			{
				AddonSelectYesnoEvent.ClickYes();
			}
		}
	}

	private unsafe static void OperateOnRequest()
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		if (PluginConfig.Instance().ConflictKeyBinding.IsPressed())
		{
			return;
		}
		AddonRequest* request = (AddonRequest*)Addons.Request;
		if (request == null)
		{
			return;
		}
		if (((AtkComponentButton)((AddonRequest)request).HandOverButton).IsEnabled)
		{
			(*((AddonRequest)request).HandOverButton).Click();
			return;
		}
		AgentNpcTrade* ptr = AgentNpcTrade.Instance();
		if (ptr == null)
		{
			return;
		}
		InventoryManager* ptr2 = InventoryManager.Instance();
		if (ptr2 == null)
		{
			return;
		}
		InventoryContainer* inventoryContainer = ((InventoryManager)ptr2).GetInventoryContainer((InventoryType)2005);
		if (inventoryContainer == null)
		{
			return;
		}
		ItemRequests requests = ((NpcTrade)(&((UIState)UIState.Instance()).NpcTrade)).Requests;
		for (int i = 0; i < requests.Count; i++)
		{
			InventoryItem* inventorySlot = ((InventoryContainer)inventoryContainer).GetInventorySlot(i);
			Item val = ((ItemRequests)(ref requests)).Items[i];
			if (((InventoryItem)inventorySlot).ItemId == val.ItemId)
			{
				continue;
			}
			if (!(*Addons.ContextIconMenu).IsAddonAndNodesReady())
			{
				((AgentId)38).SendEvent(0uL, 2, i, 0, 0);
				break;
			}
			InventoryItem* value = ((AgentNpcTrade)ptr).SelectedTurnInSlotItemOptionValues[0].Value;
			if (value != null && ((InventoryItem)value).ItemId != 0)
			{
				((AgentId)38).SendEvent(1uL, 0, 0, ((InventoryItem)value).GetItemId(), 0u, 0);
			}
			break;
		}
	}

	protected override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddonRequest });
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddonSelectYesno });
	}

	static AutoRequestItemSubmit()
	{
		InlineArray3<string> buffer = default(InlineArray3<string>);
		buffer[0] = LuminaWrapper.GetAddonText(5450u);
		buffer[1] = LuminaWrapper.GetAddonText(11514u);
		buffer[2] = LuminaWrapper.GetAddonText(102434u);
		HQItemTexts = FrozenSet.Create<string>(buffer);
	}
}
