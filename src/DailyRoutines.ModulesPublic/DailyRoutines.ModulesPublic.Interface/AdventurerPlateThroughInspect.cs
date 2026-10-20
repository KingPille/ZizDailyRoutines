using System.Numerics;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Info.Game.Packets.Upstream;
using OmenTools.Interop.Game.Lumina;

namespace DailyRoutines.ModulesPublic.Interface;

public class AdventurerPlateThroughInspect : ModuleBase
{
	private IconButtonNode? openButton;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AdventurerPlateThroughInspectTitle"),
		Description = LanguageManager.Get("AdventurerPlateThroughInspectDescription"),
		Category = ModuleCategory.Interface
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected unsafe override void Init()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)5, "CharacterInspect", new AddonEventDelegate(OnAddon));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, "CharacterInspect", new AddonEventDelegate(OnAddon));
		if ((*Addons.CharacterInspect).IsAddonAndNodesReady())
		{
			OnAddon((AddonEvent)1, null);
		}
	}

	protected override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddon });
		openButton?.Dispose();
		openButton = null;
	}

	private unsafe void OnAddon(AddonEvent type, AddonArgs? args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		if ((int)type != 5)
		{
			if ((int)type == 6)
			{
				openButton = null;
			}
		}
		else if (Addons.CharacterInspect != null && openButton == null)
		{
			openButton = new IconButtonNode
			{
				Size = new Vector2(36f),
				IsVisible = true,
				IsEnabled = true,
				IconId = 66469u,
				OnClick = delegate
				{
					new CharaCardOpenPacket(((AgentInspect)AgentInspect.Instance()).CurrentEntityId).Send();
				},
				TextTooltip = ReadOnlySeString.op_Implicit(LuminaWrapper.GetAddonText(15083u)),
				Position = new Vector2(298f, 86f)
			};
			openButton.AttachNode(((AtkUnitBase)Addons.CharacterInspect).RootNode);
		}
	}
}
