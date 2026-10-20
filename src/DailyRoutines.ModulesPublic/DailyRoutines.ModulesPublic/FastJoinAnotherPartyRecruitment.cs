using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.Text.Evaluator;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using Lumina.Text;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.AddonEvent;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.KamiToolKit.Addons.SelectYesno;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.ModulesPublic;

public class FastJoinAnotherPartyRecruitment : ModuleBase
{
	private SelectYesnoAddon? confirmAddon;

	private TextButtonNode? button;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("FastJoinAnotherPartyRecruitmentTitle"),
		Description = LanguageManager.Get("FastJoinAnotherPartyRecruitmentDescription"),
		Category = ModuleCategory.Recruitment
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected unsafe override void Init()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Expected O, but got Unknown
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Expected O, but got Unknown
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 10000
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)4, "LookingForGroupDetail", new AddonEventDelegate(OnAddon));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)9, "LookingForGroupDetail", new AddonEventDelegate(OnAddon));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, "LookingForGroupDetail", new AddonEventDelegate(OnAddon));
		if ((*Addons.LookingForGroupDetail).IsAddonAndNodesReady())
		{
			OnAddon((AddonEvent)9, null);
		}
		if ((*Addons.LookingForGroup).IsAddonAndNodesReady())
		{
			((AgentId)130).SendEvent(1uL, 17);
		}
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)1, "SelectYesno", new AddonEventDelegate(OnAddonYesno));
	}

	protected override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[2] { OnAddon, OnAddonYesno });
		button?.Dispose();
		button = null;
		confirmAddon?.Dispose();
		confirmAddon = null;
	}

	private void OnAddonYesno(AddonEvent type, AddonArgs args)
	{
		if (base.TaskHelper.IsBusy)
		{
			AddonSelectYesnoEvent.ClickYes();
		}
	}

	private unsafe void OnAddon(AddonEvent type, AddonArgs? args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		if ((int)type != 4)
		{
			if ((int)type != 6)
			{
				if ((int)type == 9)
				{
					CreateButton(Addons.LookingForGroupDetail, base.TaskHelper);
				}
			}
			else
			{
				button = null;
			}
		}
		else
		{
			UpdateOtherButtons(Addons.LookingForGroupDetail);
		}
	}

	private unsafe void CreateButton(AtkUnitBase* addon, TaskHelper taskHelper)
	{
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		if (addon == null || button != null || ((AtkValue)((byte*)((AtkUnitBase)addon).AtkValues + (nint)19 * (nint)Unsafe.SizeOf<AtkValue>())).UInt != 1)
		{
			return;
		}
		AgentLookingForGroup* agent = AgentLookingForGroup.Instance();
		if (((AgentLookingForGroup)agent).ListingContentId == LocalPlayerState.ContentID)
		{
			return;
		}
		AtkResNode* nodeById = ((AtkUnitBase)addon).GetNodeById(108u);
		if (nodeById == null)
		{
			return;
		}
		button = new TextButtonNode
		{
			Size = new Vector2(140f, 28f),
			Position = new Vector2(100f, 0f),
			IsVisible = false,
			IsEnabled = LocalPlayerState.IsInAnyParty,
			String = ReadOnlySeString.op_Implicit(LanguageManager.Get("FastJoinAnotherPartyRecruitment-LeaveAndJoin")),
			OnClick = delegate
			{
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_003f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0079: Unknown result type (might be due to invalid IL or missing references)
				//IL_007e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0083: Unknown result type (might be due to invalid IL or missing references)
				//IL_0095: Unknown result type (might be due to invalid IL or missing references)
				//IL_0054: Unknown result type (might be due to invalid IL or missing references)
				confirmAddon?.Dispose();
				RentedSeStringBuilder val = default(RentedSeStringBuilder);
				((RentedSeStringBuilder)(ref val))._002Ector();
				try
				{
					SeStringBuilder builder = ((RentedSeStringBuilder)(ref val)).Builder;
					Detailed lastViewedListing = ((AgentLookingForGroup)agent).LastViewedListing;
					builder.Append(((Detailed)(ref lastViewedListing)).LeaderString);
					if (lastViewedListing.HomeWorld != GameState.HomeWorld)
					{
						builder.AppendIcon((BitmapFontIcon)88).Append(LuminaWrapper.GetWorldName(lastViewedListing.HomeWorld));
					}
					FastJoinAnotherPartyRecruitment fastJoinAnotherPartyRecruitment = this;
					SelectYesnoAddonOptions selectYesnoAddonOptions = new SelectYesnoAddonOptions();
					ISeStringEvaluator obj = DalamudServiceExtension.Instance<ISeStringEvaluator>();
					SeStringParameter reference = SeStringParameter.op_Implicit(builder.ToReadOnlySeString());
					selectYesnoAddonOptions.Prompt = obj.EvaluateFromAddon(120u, new Span<SeStringParameter>(ref reference), (ClientLanguage?)null);
					selectYesnoAddonOptions.BlockedParentID = ((AtkUnitBase)addon).Id;
					selectYesnoAddonOptions.ParentID = ((AtkUnitBase)addon).Id;
					selectYesnoAddonOptions.Position = new AddonPosition((*((AtkUnitBase)addon).RootNode).GetNodeState().Center, AddonPositionAlignment.TopCenter);
					selectYesnoAddonOptions.Callback = delegate(SelectYesnoAddon _, SelectYesnoAddonResult result)
					{
						confirmAddon = null;
						if (result == SelectYesnoAddonResult.Yes)
						{
							Enqueue(taskHelper);
						}
					};
					fastJoinAnotherPartyRecruitment.confirmAddon = SelectYesnoAddon.Open(selectYesnoAddonOptions);
				}
				finally
				{
					((IDisposable)(*(RentedSeStringBuilder*)(&val))/*cast due to constrained. prefix*/).Dispose();
				}
			}
		};
		button.AttachNode(nodeById);
	}

	private unsafe void UpdateOtherButtons(AtkUnitBase* addon)
	{
		if (addon == null || ((AtkValue)((byte*)((AtkUnitBase)addon).AtkValues + (nint)19 * (nint)Unsafe.SizeOf<AtkValue>())).UInt != 1 || ((AgentLookingForGroup)AgentLookingForGroup.Instance()).ListingContentId == LocalPlayerState.ContentID)
		{
			return;
		}
		AtkResNode* nodeById = ((AtkUnitBase)addon).GetNodeById(108u);
		if (nodeById == null)
		{
			return;
		}
		AtkComponentButton* componentButtonById = ((AtkUnitBase)addon).GetComponentButtonById(109u);
		AtkComponentButton* componentButtonById2 = ((AtkUnitBase)addon).GetComponentButtonById(110u);
		AtkComponentButton* componentButtonById3 = ((AtkUnitBase)addon).GetComponentButtonById(111u);
		if (componentButtonById != null && componentButtonById2 != null && componentButtonById3 != null)
		{
			DetachTimelineAndSetPosition(nodeById, 35f, 56f);
			DetachTimelineAndSetPosition((AtkResNode*)((AtkComponentButton)componentButtonById).OwnerNode, -50f, 0f);
			DetachTimelineAndSetPosition((AtkResNode*)((AtkComponentButton)componentButtonById2).OwnerNode, 250f, 0f);
			DetachTimelineAndSetPosition((AtkResNode*)((AtkComponentButton)componentButtonById3).OwnerNode, 400f, 0f);
			if (button != null)
			{
				button.IsEnabled = LocalPlayerState.IsInAnyParty;
				button.IsVisible = ((AtkComponentNode)((AtkComponentButton)componentButtonById3).OwnerNode).IsVisible();
			}
		}
	}

	private unsafe static void DetachTimelineAndSetPosition(AtkResNode* node, float x, float y)
	{
		((AtkResNode)node).Timeline = null;
		((AtkResNode)node).SetPositionFloat(x, y);
	}

	private unsafe static void Enqueue(TaskHelper taskHelper)
	{
		taskHelper.Abort();
		ulong currentCID = ((AgentLookingForGroup)AgentLookingForGroup.Instance()).ListingContentId;
		if (currentCID == 0L)
		{
			return;
		}
		if (LocalPlayerState.IsInAnyParty)
		{
			taskHelper.Enqueue(delegate
			{
				if (!Throttler.Shared.Throttle("FastJoinAnotherPartyRecruitment-Task", 100u))
				{
					return false;
				}
				if (!LocalPlayerState.IsInAnyParty)
				{
					return true;
				}
				OmenServiceBase<ChatManager>.Instance().SendMessage("/leave");
				OmenServiceBase<ChatManager>.Instance().SendMessage("/pcmd breakup");
				((AgentId)61).SendEvent(0uL, 2, 3);
				return !LocalPlayerState.IsInAnyParty;
			});
		}
		taskHelper.Enqueue(delegate
		{
			if (!Throttler.Shared.Throttle("FastJoinAnotherPartyRecruitment-Task"))
			{
				return false;
			}
			AgentLookingForGroup* ptr = AgentLookingForGroup.Instance();
			if (((AgentLookingForGroup)ptr).ListingContentId == currentCID)
			{
				return true;
			}
			((AgentLookingForGroup)ptr).OpenListingByContentId(currentCID);
			return ((AgentLookingForGroup)ptr).ListingContentId == currentCID;
		});
		taskHelper.Enqueue(delegate
		{
			if (!Throttler.Shared.Throttle("FastJoinAnotherPartyRecruitment-Task"))
			{
				return false;
			}
			if (!(*Addons.LookingForGroupDetail).IsAddonAndNodesReady())
			{
				return false;
			}
			AtkComponentButton* componentButtonById = ((AtkUnitBase)Addons.LookingForGroupDetail).GetComponentButtonById(109u);
			if (componentButtonById == null)
			{
				return false;
			}
			(*componentButtonById).Click();
			return true;
		});
		taskHelper.DelayNext(500);
	}
}
