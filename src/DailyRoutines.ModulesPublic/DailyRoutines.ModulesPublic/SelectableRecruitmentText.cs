using System;
using System.Numerics;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using InteropGenerator.Runtime;
using KamiToolKit.Nodes;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.KamiToolKit.Nodes;

namespace DailyRoutines.ModulesPublic;

public class SelectableRecruitmentText : ModuleBase
{
	private TextMultiLineInputNode? recruitmentTextNode;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("SelectableRecruitmentTextTitle"),
		Description = LanguageManager.Get("SelectableRecruitmentTextDescription"),
		Category = ModuleCategory.Recruitment,
		PreviewImageURL = new string[1] { "hxxps://gh.atmoomen.top/raw.githubusercontent.com/Dalamud-DailyRoutines/DailyRoutines/main/Resources/Modules/SelectableRecruitmentText/preview-1.png" }
	};

	protected override void Init()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)5, "LookingForGroupDetail", new AddonEventDelegate(OnAddon));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, "LookingForGroupDetail", new AddonEventDelegate(OnAddon));
	}

	protected override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddon });
		recruitmentTextNode?.Dispose();
		recruitmentTextNode = null;
	}

	private unsafe void OnAddon(AddonEvent type, AddonArgs? args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		if ((int)type != 5)
		{
			if ((int)type == 6)
			{
				recruitmentTextNode = null;
			}
		}
		else
		{
			if (!(*Addons.LookingForGroupDetail).IsAddonAndNodesReady())
			{
				return;
			}
			AgentLookingForGroup* ptr = AgentLookingForGroup.Instance();
			if (ptr == null)
			{
				return;
			}
			AtkTextNode* textNodeById = ((AtkUnitBase)Addons.LookingForGroupDetail).GetTextNodeById(20u);
			if (textNodeById == null)
			{
				return;
			}
			AtkComponentButton* componentButtonById = ((AtkUnitBase)Addons.LookingForGroupDetail).GetComponentButtonById(18u);
			if (componentButtonById == null)
			{
				return;
			}
			if (recruitmentTextNode != null)
			{
				recruitmentTextNode.Position = new Vector2(((AtkComponentNode)((AtkComponentButton)componentButtonById).OwnerNode).X, ((AtkComponentNode)((AtkComponentButton)componentButtonById).OwnerNode).Y) - new Vector2(10f, 8f);
				AddonLookingForGroupDetail* lookingForGroupDetail = (AddonLookingForGroupDetail*)Addons.LookingForGroupDetail;
				AtkTextNode* partyLeaderTextNode = ((AddonLookingForGroupDetail)lookingForGroupDetail).PartyLeaderTextNode;
				if (partyLeaderTextNode == null)
				{
					return;
				}
				Utf8String nodeText = ((AtkTextNode)partyLeaderTextNode).NodeText;
				if (nodeText.IsEmpty || !((CStringPointer)(ref nodeText.StringPtr)).HasValue || CStringExtensions.ExtractText(nodeText.StringPtr) != ((Detailed)(&((AgentLookingForGroup)ptr).LastViewedListing)).LeaderString)
				{
					return;
				}
				TextMultiLineInputNode textMultiLineInputNode = recruitmentTextNode;
				ReadOnlySeString val;
				if (textMultiLineInputNode != null && !textMultiLineInputNode.IsFocused)
				{
					val = textMultiLineInputNode.String;
					if (((ReadOnlySeString)(ref val)).IsEmpty)
					{
						ReadOnlySeString val2 = SeStringExtension.PraseAutoTranslate(new ReadOnlySeStringSpan((ReadOnlySpan<byte>)((Detailed)(&((AgentLookingForGroup)ptr).LastViewedListing)).Comment));
						recruitmentTextNode.String = val2;
					}
				}
				textMultiLineInputNode = recruitmentTextNode;
				if (textMultiLineInputNode != null && !textMultiLineInputNode.IsVisible)
				{
					val = textMultiLineInputNode.String;
					if (!((ReadOnlySeString)(ref val)).IsEmpty)
					{
						recruitmentTextNode.IsVisible = true;
					}
				}
			}
			else
			{
				AtkResNode* nodeById = ((AtkUnitBase)Addons.LookingForGroupDetail).GetNodeById(19u);
				if (nodeById != null)
				{
					((AtkComponentNode)((AtkComponentButton)componentButtonById).OwnerNode).ToggleVisibility(false);
					((AtkTextNode)textNodeById).ToggleVisibility(false);
					((AtkResNode)nodeById).ToggleVisibility(false);
					recruitmentTextNode = new TextMultiLineInputNode
					{
						AutoUpdateHeight = false,
						Size = new Vector2(520f, 60f),
						Position = new Vector2(((AtkComponentNode)((AtkComponentButton)componentButtonById).OwnerNode).X, ((AtkComponentNode)((AtkComponentButton)componentButtonById).OwnerNode).Y) - new Vector2(10f, 8f),
						ShowLimitText = false,
						IsVisible = false,
						MaxLines = 2u
					};
					recruitmentTextNode.TextLimitsNode.DetachNode();
					TextNode currentTextNode = recruitmentTextNode.CurrentTextNode;
					currentTextNode.TextFlags = (TextFlags)(currentTextNode.TextFlags | 0x40);
					recruitmentTextNode.AttachNode(Addons.LookingForGroupDetail);
				}
			}
		}
	}
}
