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

public class AutoMonsterToss : ModuleBase
{
	private const uint EVENT_ID = 2359297u;

	private const uint ROUND_START_CATEGORY = 17235982u;

	private const uint PLAY_GAME_CATEGORY = 17301518u;

	private const int GAME_TIME_SECONDS = 25;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoMonsterTossTitle"),
		Description = LanguageManager.Get("AutoMonsterTossDescription"),
		Category = ModuleCategory.GoldSaucer
	};

	protected override void Init()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		base.TaskHelper = new TaskHelper();
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)1, "BasketBall", new AddonEventDelegate(OnAddonSetup));
	}

	protected unsafe override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddonSetup });
		if ((*Addons.BasketBall).IsAddonAndNodesReady())
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
		if (base.TaskHelper.AbortByConflictKey(this))
		{
			return;
		}
		int currentMGP = 0;
		base.TaskHelper.Abort();
		base.TaskHelper.Enqueue((Func<bool>)WaitSelectStringAddon, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
		base.TaskHelper.Enqueue(delegate
		{
			UpdateSelectStringInfo(LanguageManager.Get("AutoMonsterToss-StartingGame"));
			currentMGP = ((InventoryManager)InventoryManager.Instance()).GetInventoryItemCount(29u, false, true, true, (short)0);
			SendNewRound();
		});
		base.TaskHelper.Enqueue(() => ((InventoryManager)InventoryManager.Instance()).GetInventoryItemCount(29u, false, true, true, (short)0) != currentMGP);
		base.TaskHelper.DelayNext(1000);
		base.TaskHelper.Enqueue((Action)SendPlayGame, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
		for (int num = 0; num < 25; num++)
		{
			int second = num;
			base.TaskHelper.DelayNext(1000);
			base.TaskHelper.Enqueue(delegate
			{
				UpdateSelectStringInfo(LanguageManager.Get("AutoMonsterToss-WaitingForResult", 25 - second));
			});
		}
		base.TaskHelper.Enqueue((Action)SendRoundEnd, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
		base.TaskHelper.Enqueue(() => !IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()));
		base.TaskHelper.Enqueue((Action)SendInteractWithMachine, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
	}

	private unsafe static bool WaitSelectStringAddon()
	{
		if ((*Addons.SelectString).IsAddonAndNodesReady())
		{
			return (*Addons.BasketBall).IsAddonAndNodesReady();
		}
		return false;
	}

	private static void SendInteractWithMachine()
	{
		new EventStartPackt(LocalPlayerState.EntityID, 2359297u).Send();
	}

	private static void SendNewRound()
	{
		new EventActionPacket(2359297u, 17235982u).Send();
	}

	private static void SendPlayGame()
	{
		new EventActionPacket(2359297u, 17301518u, 1u).Send();
		new EventActionPacket(2359297u, 17301518u, 1u).Send();
		new EventActionPacket(2359297u, 17301518u, 1u).Send();
		new EventActionPacket(2359297u, 17301518u, 1u).Send();
		new EventActionPacket(2359297u, 17301518u, 1u).Send();
	}

	private static void SendRoundEnd()
	{
		new EventCompletePackt(2359297u, 14u).Send();
	}

	private unsafe static void UpdateSelectStringInfo(string info)
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		if (!(*Addons.SelectString).IsAddonAndNodesReady() || !(*Addons.BasketBall).IsAddonAndNodesReady())
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
			((SeStringInterpolatedStringHandler)(ref val3)).AppendFormatted<string>(LanguageManager.Get("AutoMonsterTossTitle"));
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
