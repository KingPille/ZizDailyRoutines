using System;
using System.Numerics;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using KamiToolKit.UiOverlay;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class AutoDisplayPlayerHitbox : ModuleBase
{
	private class PlayerDotImageNode : OverlayNode
	{
		private readonly Config moduleConfig;

		private readonly IconImageNode imageNode;

		public override OverlayLayer OverlayLayer => OverlayLayer.Foreground;

		public override bool HideWithNativeUi => false;

		public PlayerDotImageNode(Config config)
		{
			moduleConfig = config;
			imageNode = new IconImageNode
			{
				IconId = 60952u,
				FitTexture = true
			};
			imageNode.AttachNode(this);
		}

		protected override void OnSizeChanged()
		{
			base.OnSizeChanged();
			imageNode.Size = Size;
			imageNode.Origin = new Vector2(moduleConfig.Size / 2f);
		}

		protected override void OnUpdate()
		{
			Size = new Vector2(moduleConfig.Size);
			imageNode.Color = moduleConfig.Color;
			imageNode.IconId = moduleConfig.IconID;
			base.Timeline?.PlayAnimation(1);
			IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
			if (localPlayer == null)
			{
				IsVisible = false;
				return;
			}
			IsVisible = !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)38] && (!moduleConfig.OnlyInCombat || DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)26]) && (!moduleConfig.OnlyInDuty || DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)34]) && (!moduleConfig.OnlyUnsheathed || IsWeaponUnsheathed());
			if (IsVisible)
			{
				Vector3 offset = moduleConfig.Offset;
				float x = 0f - localPlayer.Rotation;
				float num = MathF.Cos(x);
				float num2 = MathF.Sin(x);
				Vector3 vector = new Vector3(num * offset.X - num2 * offset.Z, offset.Y, num2 * offset.X + num * offset.Z);
				Vector2 vector2 = default(Vector2);
				DalamudServiceExtension.Instance<IGameGui>().WorldToScreen(localPlayer.Position + vector, ref vector2);
				Position = vector2 - imageNode.Size / 2f;
			}
		}
	}

	private class Config : ModuleConfig
	{
		public Vector4 Color = new Vector4(1f, 1f, 1f, 1f);

		public uint IconID = 60422u;

		public Vector3 Offset = Vector3.Zero;

		public bool OnlyInCombat = true;

		public bool OnlyInDuty = true;

		public bool OnlyUnsheathed;

		public float Size = 96f;
	}

	private Config config;

	private OverlayController controller;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoDisplayPlayerHitboxTitle"),
		Description = LanguageManager.Get("AutoDisplayPlayerHitboxDescription"),
		Category = ModuleCategory.Combat,
		Author = new string[1] { "Due" }
	};

	protected override void Init()
	{
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		controller = new OverlayController();
		controller.AddNode(new PlayerDotImageNode(config));
	}

	protected override void Uninit()
	{
		controller?.Dispose();
		controller = null;
	}

	protected override void ConfigUI()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("OnlyInCombat")), ref config.OnlyInCombat))
		{
			config.Save(this);
		}
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("OnlyInDuty")), ref config.OnlyInDuty))
		{
			config.Save(this);
		}
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("OnlyUnsheathed")), ref config.OnlyUnsheathed))
		{
			config.Save(this);
		}
		ImGui.NewLine();
		ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
		ImGui.ColorPicker4(ImU8String.op_Implicit(LanguageManager.Get("Color")), ref config.Color, (ImGuiColorEditFlags)0);
		if (ImGui.IsItemDeactivatedAfterEdit())
		{
			config.Save(this);
		}
		ImGui.NewLine();
		ItemWidthDisposable val = ImRaii.ItemWidth(300f * Globals.GlobalUIScale);
		try
		{
			ImU8String val2 = ImU8String.op_Implicit(LanguageManager.Get("Size"));
			ref float size = ref config.Size;
			ImU8String val3 = default(ImU8String);
			if (ImGui.InputFloat(val2, ref size, 0f, 0f, val3, (ImGuiInputTextFlags)0))
			{
				config.Size = MathF.Max(1f, config.Size);
			}
			if (ImGui.IsItemDeactivatedAfterEdit())
			{
				config.Save(this);
			}
			ImU8String val4 = ImU8String.op_Implicit(LanguageManager.Get("Icon"));
			ref uint iconID = ref config.IconID;
			val3 = default(ImU8String);
			ImGui.InputUInt(val4, ref iconID, 0u, 0u, val3, (ImGuiInputTextFlags)0);
			if (ImGui.IsItemDeactivatedAfterEdit())
			{
				config.Save(this);
			}
			ImGui.SameLine();
			val3 = new ImU8String(0, 1);
			((ImU8String)(ref val3)).AppendFormatted<string>(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)63597));
			if (ImGui.Button(val3, default(Vector2)))
			{
				OmenServiceBase<ChatManager>.Instance().SendCommand("/xldata icon");
			}
			ImGuiOm.TooltipHover(LanguageManager.Get("IconBrowser") + "\n(" + LanguageManager.Get("IconBrowser-Suggestion") + ")");
			if (ImGui.InputFloat3(ImU8String.op_Implicit(LanguageManager.Get("Offset")), ref config.Offset, 0.1f, 1f, ImU8String.op_Implicit("%.1f"), (ImGuiInputTextFlags)0))
			{
				config.Save(this);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private unsafe static bool IsWeaponUnsheathed()
	{
		return ((WeaponState)(&((UIState)UIState.Instance()).WeaponState)).IsUnsheathed;
	}
}
