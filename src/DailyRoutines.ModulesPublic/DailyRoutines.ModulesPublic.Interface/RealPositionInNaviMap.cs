using System.Drawing;
using System.Numerics;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Interface;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using Lumina.Text.ReadOnly;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Info.Game.Data;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic.Interface;

public class RealPositionInNaviMap : ModuleBase
{
	private class Config : ModuleConfig
	{
		public string CopyFormat = "X:{0:F1} Y:{1:F1} Z:{2:F1}";
	}

	private Config config;

	private TextButtonNode? positionButton;

	private int lastX;

	private int lastY;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("RealPositionInNaviMapTitle"),
		Description = LanguageManager.Get("RealPositionInNaviMapDescription"),
		Category = ModuleCategory.Interface
	};

	protected override void Init()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)8, "_NaviMap", new AddonEventDelegate(OnAddon));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, "_NaviMap", new AddonEventDelegate(OnAddon));
	}

	protected unsafe override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddon });
		positionButton?.Dispose();
		positionButton = null;
		if (Addons.NaviMap != null)
		{
			AtkTextNode* textNodeById = ((AtkUnitBase)Addons.NaviMap).GetTextNodeById(6u);
			if (textNodeById != null)
			{
				((AtkTextNode)textNodeById).ToggleVisibility(true);
			}
		}
		lastX = (lastY = 0);
	}

	protected override void ConfigUI()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("RealPositionInNaviMap-CopyFormat")));
		ImGuiOm.HelpMarker(LanguageManager.Get("RealPositionInNaviMap-CopyFormatHelp"), 20f * Globals.GlobalUIScale, (FontAwesomeIcon)61530);
		ImGui.InputText(ImU8String.op_Implicit("###CopyFormat"), ref config.CopyFormat, 256, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
		if (ImGui.IsItemDeactivatedAfterEdit())
		{
			config.Save(this);
		}
	}

	private unsafe void OnAddon(AddonEvent type, AddonArgs args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		if ((int)type != 6)
		{
			if ((int)type != 8)
			{
				return;
			}
			NumberArrayData* numberArrayData = ((AtkStage)AtkStage.Instance()).GetNumberArrayData((NumberArrayType)25);
			if (numberArrayData == null)
			{
				return;
			}
			if (!DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)48])
			{
				if (*((NumberArrayData)numberArrayData).IntArray != lastX)
				{
					lastX = *((NumberArrayData)numberArrayData).IntArray;
				}
				else
				{
					if (((NumberArrayData)numberArrayData).IntArray[1] == lastY)
					{
						return;
					}
					lastY = ((NumberArrayData)numberArrayData).IntArray[1];
				}
			}
			if (positionButton == null)
			{
				AtkTextNode* textNodeById = ((AtkUnitBase)Addons.NaviMap).GetTextNodeById(6u);
				if (textNodeById == null)
				{
					return;
				}
				positionButton = new TextButtonNode
				{
					Position = new Vector2(0f),
					Size = new Vector2(130f, 18f),
					IsVisible = true,
					String = ReadOnlySeString.op_Implicit(string.Empty),
					OnClick = delegate
					{
						//IL_0078: Unknown result type (might be due to invalid IL or missing references)
						IPlayerCharacter localPlayer3 = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
						if (localPlayer3 != null)
						{
							((AgentMap)AgentMap.Instance()).SetFlagMapMarker(GameState.TerritoryType, GameState.Map, localPlayer3.Position, 60561u);
							string text = string.Format(config.CopyFormat, localPlayer3.Position.X, localPlayer3.Position.Y, localPlayer3.Position.Z);
							if (!string.IsNullOrWhiteSpace(text))
							{
								ImGui.SetClipboardText(ImU8String.op_Implicit(text));
								OmenServiceBase<NotifyHelper>.Instance().NotificationSuccess(LanguageManager.Get("CopiedToClipboard") + ": " + text);
							}
						}
					}
				};
				IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
				if (localPlayer != null)
				{
					positionButton.String = ReadOnlySeString.op_Implicit($"X:{localPlayer.Position.X:F1} Y:{localPlayer.Position.Y:F1} Z:{localPlayer.Position.Z:F1}");
				}
				positionButton.BackgroundNode.IsVisible = false;
				positionButton.LabelNode.TextFlags = (TextFlags)16;
				positionButton.LabelNode.TextColor = ((AtkTextNode)textNodeById).Color.ToVector4();
				positionButton.LabelNode.TextOutlineColor = ((AtkTextNode)textNodeById).EdgeColor.ToVector4();
				positionButton.AttachNode(((AtkUnitBase)Addons.NaviMap).GetNodeById(5u));
				((AtkTextNode)textNodeById).ToggleVisibility(false);
			}
			IPlayerCharacter localPlayer2 = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
			if (localPlayer2 != null)
			{
				positionButton.String = ReadOnlySeString.op_Implicit($"X:{localPlayer2.Position.X:F1} Y:{localPlayer2.Position.Y:F1} Z:{localPlayer2.Position.Z:F1}");
			}
		}
		else
		{
			positionButton = null;
			lastX = (lastY = 0);
		}
	}
}
