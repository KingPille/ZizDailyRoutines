using System;
using System.Numerics;
using DailyRoutines.Common.Interface.Windows;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.Info.Game.Data;
using OmenTools.Info.Game.Packets.Upstream;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;

namespace DailyRoutines.ModulesPublic.Interface;

public class AutoCollectableExchange : ModuleBase
{
	private unsafe delegate nint HandInCollectablesDelegate(AgentInterface* agentCollectablesShop);

	private static readonly CompSig HandInCollectablesSig = new CompSig("48 89 6C 24 ?? 48 89 74 24 ?? 57 41 56 41 57 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 84 24 ?? ?? ?? ?? 48 8B F1 48 8B 49");

	private HandInCollectablesDelegate? handInCollectables;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoCollectableExchangeTitle"),
		Description = LanguageManager.Get("AutoCollectableExchangeDescription"),
		Category = ModuleCategory.Interface
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected unsafe override void Init()
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Expected O, but got Unknown
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected O, but got Unknown
		if (base.TaskHelper == null)
		{
			TaskHelper taskHelper = (base.TaskHelper = new TaskHelper());
		}
		if (base.Overlay == null)
		{
			Overlay overlay = (base.Overlay = new Overlay(this));
		}
		if (handInCollectables == null)
		{
			handInCollectables = HandInCollectablesSig.GetDelegate<HandInCollectablesDelegate>();
		}
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)1, "CollectablesShop", new AddonEventDelegate(OnAddon));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, "CollectablesShop", new AddonEventDelegate(OnAddon));
		if (Addons.CollectablesShopAddon != null)
		{
			OnAddon((AddonEvent)1, null);
		}
	}

	protected override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddon });
	}

	protected unsafe override void OverlayUI()
	{
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		AtkUnitBase* collectablesShopAddon = Addons.CollectablesShopAddon;
		if (collectablesShopAddon == null)
		{
			((Window)base.Overlay).IsOpen = false;
			return;
		}
		AtkResNode* nodeById = ((AtkUnitBase)Addons.CollectablesShopAddon).GetNodeById(51u);
		if (nodeById == null)
		{
			return;
		}
		if (((AtkResNode)nodeById).IsVisible())
		{
			((AtkResNode)nodeById).ToggleVisibility(false);
		}
		using (OmenServiceBase<FontManager>.Instance().UIFont80.Push())
		{
			ImGui.SetWindowPos(new Vector2((float)((AtkUnitBase)collectablesShopAddon).X + ((AtkUnitBase)collectablesShopAddon).GetScaledWidth(true), (float)((AtkUnitBase)collectablesShopAddon).Y + ((AtkUnitBase)collectablesShopAddon).GetScaledHeight(true)) - ImGui.GetWindowSize() - Globals.ScaledVector2(12f), (ImGuiCond)0);
			ImGui.AlignTextToFramePadding();
			Vector4 dalamudYellow = ImGuiColors.DalamudYellow;
			ImGui.TextColored(ref dalamudYellow, ImU8String.op_Implicit(LanguageManager.Get("AutoCollectableExchangeTitle")));
			DisabledDisposable val = ImRaii.Disabled(!((Enum)((AtkResNode)nodeById).NodeFlags).HasFlag((Enum)(object)(NodeFlags)32) || base.TaskHelper.IsBusy);
			try
			{
				if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("Start")), default(Vector2)))
				{
					EnqueueExchange();
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
			ImGui.SameLine();
			val = ImRaii.Disabled(!base.TaskHelper.IsBusy);
			try
			{
				if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("Stop")), default(Vector2)))
				{
					base.TaskHelper.Abort();
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
			ImGui.SameLine();
			ImGui.TextDisabled(ImU8String.op_Implicit("|"));
			val = ImRaii.Disabled(base.TaskHelper.IsBusy);
			try
			{
				ImGui.SameLine();
				DisabledDisposable val2 = ImRaii.Disabled(!((Enum)((AtkResNode)nodeById).NodeFlags).HasFlag((Enum)(object)(NodeFlags)32));
				try
				{
					if (ImGui.Button(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(531u)), default(Vector2)))
					{
						handInCollectables(((AgentModule)AgentModule.Instance()).GetAgentByInternalId((AgentId)382));
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				ImGui.SameLine();
				InclusionShop rowOrDefault = LuminaGetter.GetRowOrDefault<InclusionShop>(3801094u);
				if (!ImGui.Button(ImU8String.op_Implicit(((object)((InclusionShop)(ref rowOrDefault)).ShopName/*cast due to constrained. prefix*/).ToString()), default(Vector2)))
				{
					return;
				}
				base.TaskHelper.Enqueue(delegate
				{
					if ((*Addons.CollectablesShopAddon).IsAddonAndNodesReady())
					{
						((AtkUnitBase)Addons.CollectablesShopAddon).Close(true);
					}
				});
				base.TaskHelper.Enqueue(() => !IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()));
				base.TaskHelper.Enqueue(delegate
				{
					OmenServiceBase<GamePacketManager>.Instance().SendPackt(new EventStartPackt(IOmenDalamudService<IObjectTable>.Instance().LocalPlayer.GameObjectID, GetScriptEventID(GameState.TerritoryType)));
				});
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
	}

	private unsafe void EnqueueExchange()
	{
		base.TaskHelper.Enqueue(delegate
		{
			if (Addons.CollectablesShopAddon == null || (*Addons.SelectYesno).IsAddonAndNodesReady())
			{
				base.TaskHelper.Abort();
				return true;
			}
			AtkComponentList* asAtkComponentList = ((AtkComponentNode)((AtkUnitBase)Addons.CollectablesShopAddon).GetComponentNodeById(31u)).GetAsAtkComponentList();
			if (asAtkComponentList == null)
			{
				return false;
			}
			if (((AtkComponentList)asAtkComponentList).ListLength <= 0)
			{
				base.TaskHelper.Abort();
				return true;
			}
			handInCollectables(((AgentModule)AgentModule.Instance()).GetAgentByInternalId((AgentId)382));
			return true;
		}, "ClickExchange");
		base.TaskHelper.Enqueue((Action)EnqueueExchange, "EnqueueNewRound", 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
	}

	private static uint GetScriptEventID(uint zone)
	{
		return zone switch
		{
			478u => 3539065u, 
			635u => 3539064u, 
			820u => 3539063u, 
			963u => 3539062u, 
			1186u => 3539072u, 
			_ => 3539066u, 
		};
	}

	private unsafe void OnAddon(AddonEvent type, AddonArgs? args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Invalid comparison between Unknown and I4
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Invalid comparison between Unknown and I4
		if (args.Addon.ToStruct() != null)
		{
			Overlay overlay = base.Overlay;
			bool isOpen = (int)type == 1 || ((int)type != 6 && ((Window)base.Overlay).IsOpen);
			((Window)overlay).IsOpen = isOpen;
		}
	}
}
