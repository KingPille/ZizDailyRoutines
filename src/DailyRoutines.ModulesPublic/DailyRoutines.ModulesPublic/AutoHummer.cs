using System;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Text;
using OmenTools.Extensions;
using OmenTools.ImGuiOm;
using OmenTools.Info.Game.Data;
using OmenTools.Info.Game.Packets.Upstream;
using OmenTools.OmenService;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;

namespace DailyRoutines.ModulesPublic;

public class AutoHummer : ModuleBase
{
	private const uint EVENT_ID = 2359299u;

	private const uint ROUND_START_CATEGORY = 17235982u;

	private const uint PLAY_GAME_CATEGORY = 17301518u;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoHummerTitle"),
		Description = LanguageManager.Get("AutoHummerDescription"),
		Category = ModuleCategory.GoldSaucer
	};

	protected override void Init()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		base.TaskHelper = new TaskHelper();
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)1, "Hummer", new AddonEventDelegate(OnAddonSetup));
	}

	protected unsafe override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddonSetup });
		if ((*Addons.Hummer).IsAddonAndNodesReady())
		{
			SendRoundEnd();
		}
	}

	protected override void ConfigUI()
	{
		ImGuiOmExtension.ConflictKeyText();
		ImGui.NewLine();
		DisabledDisposable val = ImRaii.Disabled(GameState.TerritoryType != 144 || base.TaskHelper.IsBusy || IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()));
		try
		{
			if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61515, LanguageManager.Get("Start")))
			{
				SendInteractWithMachine();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		ImGui.SameLine();
		if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61517, LanguageManager.Get("Stop")))
		{
			base.TaskHelper.Abort();
			SendRoundEnd();
		}
	}

	private unsafe void OnAddonSetup(AddonEvent type, AddonArgs args)
	{
		if (!base.TaskHelper.AbortByConflictKey(this))
		{
			int currentMGP = 0;
			base.TaskHelper.Abort();
			base.TaskHelper.Enqueue((Func<bool>)WaitSelectStringAddon, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
			base.TaskHelper.Enqueue(delegate
			{
				UpdateSelectStringInfo(LanguageManager.Get("AutoHummer-StartingRound"));
				currentMGP = ((InventoryManager)InventoryManager.Instance()).GetInventoryItemCount(29u, false, true, true, (short)0);
				SendNewRound();
			});
			base.TaskHelper.Enqueue(() => ((InventoryManager)InventoryManager.Instance()).GetInventoryItemCount(29u, false, true, true, (short)0) != currentMGP);
			base.TaskHelper.DelayNext(1000);
			base.TaskHelper.Enqueue(delegate
			{
				UpdateSelectStringInfo(LanguageManager.Get("AutoHummer-WaitingForResult") + "......");
				SendPlayGame();
			});
			base.TaskHelper.DelayNext(5000);
			base.TaskHelper.Enqueue((Action)SendRoundEnd, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
			base.TaskHelper.Enqueue(() => !IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()));
			base.TaskHelper.Enqueue((Action)SendInteractWithMachine, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
		}
	}

	private unsafe static bool WaitSelectStringAddon()
	{
		if ((*Addons.SelectString).IsAddonAndNodesReady())
		{
			return (*Addons.Hummer).IsAddonAndNodesReady();
		}
		return false;
	}

	private static void SendInteractWithMachine()
	{
		new EventStartPackt(LocalPlayerState.EntityID, 2359299u).Send();
	}

	private static void SendNewRound()
	{
		new EventActionPacket(2359299u, 17235982u).Send();
	}

	private static void SendPlayGame()
	{
		new EventActionPacket(2359299u, 17301518u, 3u, 0u, 1u).Send();
	}

	private static void SendRoundEnd()
	{
		new EventCompletePackt(2359299u, 14u).Send();
	}

	private unsafe static void UpdateSelectStringInfo(string info)
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		if (!(*Addons.SelectString).IsAddonAndNodesReady() || !(*Addons.Hummer).IsAddonAndNodesReady())
		{
			return;
		}
		AtkComponentList* componentListById = ((AtkUnitBase)Addons.SelectString).GetComponentListById(3u);
		AtkTextNode* textNodeById = ((AtkUnitBase)Addons.SelectString).GetTextNodeById(2u);
		if (componentListById == null || textNodeById == null)
		{
			return;
		}
		((AtkComponentNode)((AtkComponentList)componentListById).OwnerNode).ToggleVisibility(false);
		((AtkComponentList)componentListById).SetEnabledState(false);
		((AtkTextNode)textNodeById).FontSize = 18;
		((AtkTextNode)textNodeById).AlignmentType = (AlignmentType)4;
		RentedSeStringBuilder val = default(RentedSeStringBuilder);
		((RentedSeStringBuilder)(ref val))._002Ector();
		try
		{
			SeStringBuilder builder = ((RentedSeStringBuilder)(ref val)).Builder;
			SeStringBuilder val2 = builder.PushColorType(28u);
			SeStringInterpolatedStringHandler val3 = new SeStringInterpolatedStringHandler(2, 1, val2);
			((SeStringInterpolatedStringHandler)(ref val3)).AppendLiteral("[");
			((SeStringInterpolatedStringHandler)(ref val3)).AppendFormatted<string>(LanguageManager.Get("AutoHummerTitle"));
			((SeStringInterpolatedStringHandler)(ref val3)).AppendLiteral("]");
			val2.Append(val3).PopColorType().AppendNewLine()
				.Append(info);
			((AtkTextNode)textNodeById).SetText(builder.GetViewAsSpan());
			((AtkTextNode)textNodeById).SetPositionFloat(20f, 60f);
		}
		finally
		{
			((IDisposable)(*(RentedSeStringBuilder*)(&val))/*cast due to constrained. prefix*/).Dispose();
		}
	}
}
