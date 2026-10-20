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
using FFXIVClientStructs.FFXIV.Client.System.Input;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using KamiToolKit.Timelines;
using KamiToolKit.UiOverlay;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic.Interface;

public class AutoHighlightCursor : ModuleBase
{
	private class Config : ModuleConfig
	{
		public Vector4 Color = Vector4.One;

		public bool HideOnCameraMove = true;

		public uint IconID = 60498u;

		public bool OnlyShowInCombat = true;

		public bool OnlyShowInDuty;

		public bool PlayAnimation = true;

		public float Size = 96f;
	}

	private class CursorImageNode : OverlayNode
	{
		private readonly Config moduleConfig;

		private readonly IconImageNode imageNode;

		public override OverlayLayer OverlayLayer => OverlayLayer.Foreground;

		public override bool HideWithNativeUi => true;

		public CursorImageNode(Config config)
		{
			moduleConfig = config;
			imageNode = new IconImageNode
			{
				IconId = 60498u,
				FitTexture = true
			};
			imageNode.AttachNode(this);
			AddTimeline(new TimelineBuilder().BeginFrameSet(1, 120).AddLabel(1, 1, (AtkTimelineJumpBehavior)0, 0).AddLabel(60, 0, (AtkTimelineJumpBehavior)2, 1)
				.AddLabel(61, 2, (AtkTimelineJumpBehavior)0, 0)
				.AddLabel(120, 0, (AtkTimelineJumpBehavior)2, 2)
				.EndFrameSet()
				.Build());
			IconImageNode iconImageNode = imageNode;
			FrameSetBuilder frameSetBuilder = new TimelineBuilder().BeginFrameSet(1, 60);
			Vector2? scale = new Vector2(1f, 1f);
			FrameSetBuilder frameSetBuilder2 = frameSetBuilder.AddFrame(1, null, null, null, null, null, scale);
			Vector2? scale2 = new Vector2(0.75f, 0.75f);
			FrameSetBuilder frameSetBuilder3 = frameSetBuilder2.AddFrame(30, null, null, null, null, null, scale2);
			Vector2? scale3 = new Vector2(1f, 1f);
			FrameSetBuilder frameSetBuilder4 = frameSetBuilder3.AddFrame(60, null, null, null, null, null, scale3).EndFrameSet().BeginFrameSet(61, 120);
			Vector2? scale4 = new Vector2(1f, 1f);
			iconImageNode.AddTimeline(frameSetBuilder4.AddFrame(61, null, null, null, null, null, scale4).EndFrameSet().Build());
		}

		protected override void OnSizeChanged()
		{
			base.OnSizeChanged();
			imageNode.Size = Size;
			imageNode.Origin = new Vector2(moduleConfig.Size / 2f);
		}

		protected unsafe override void OnUpdate()
		{
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Invalid comparison between Unknown and I4
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Invalid comparison between Unknown and I4
			Size = new Vector2(moduleConfig.Size);
			imageNode.Color = moduleConfig.Color;
			imageNode.IconId = moduleConfig.IconID;
			base.Timeline?.PlayAnimation(moduleConfig.PlayAnimation ? 1 : 2);
			ref CursorInputData reference = ref ((UIInputData)UIInputData.Instance()).CursorInputs;
			Position = new Vector2(reference.PositionX, reference.PositionY) - imageNode.Size / 2f;
			bool flag = (reference.MouseButtonHeldFlags & 1) > 0;
			bool flag2 = (reference.MouseButtonHeldFlags & 4) > 0;
			Config config = moduleConfig;
			if ((config != null && (config.OnlyShowInCombat || config.OnlyShowInDuty)) ? true : false)
			{
				bool flag3 = true;
				flag3 &= !moduleConfig.OnlyShowInCombat || DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)26];
				flag3 &= !moduleConfig.OnlyShowInDuty || IConditionExtension.get_IsBoundByDuty(DalamudServiceExtension.Instance<ICondition>());
				flag3 &= !moduleConfig.HideOnCameraMove || (!flag && !flag2);
				IsVisible = flag3;
			}
			else
			{
				IsVisible = (!flag && !flag2) || !moduleConfig.HideOnCameraMove;
			}
		}
	}

	private Config config;

	private OverlayController? controller;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoHighlightCursorTitle"),
		Description = LanguageManager.Get("AutoHighlightCursorDescription"),
		Category = ModuleCategory.Interface
	};

	protected override void Init()
	{
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		if (controller == null)
		{
			controller = new OverlayController();
		}
		controller.AddNode(new CursorImageNode(config));
	}

	protected override void Uninit()
	{
		controller?.Dispose();
		controller = null;
	}

	protected override void ConfigUI()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(0, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("AutoHighlightCursor-PlayAnimation"));
		if (ImGui.Checkbox(val, ref config.PlayAnimation))
		{
			config.Save(this);
		}
		ImGuiOm.HelpMarker(LanguageManager.Get("AutoHighlightCursor-PlayAnimation-Help"), 40f, (FontAwesomeIcon)61530);
		ImU8String val2 = default(ImU8String);
		((ImU8String)(ref val2))._002Ector(0, 1);
		((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get("AutoHighlightCursor-HideOnCameraMove"));
		if (ImGui.Checkbox(val2, ref config.HideOnCameraMove))
		{
			config.Save(this);
		}
		ImGuiOm.HelpMarker(LanguageManager.Get("AutoHighlightCursor-HideOnCameraMove-Help"), 40f, (FontAwesomeIcon)61530);
		ImGui.NewLine();
		ItemWidthDisposable val3 = ImRaii.ItemWidth(200f * Globals.GlobalUIScale);
		try
		{
			ImGui.ColorPicker4(ImU8String.op_Implicit(LanguageManager.Get("Color")), ref config.Color, (ImGuiColorEditFlags)0);
			if (ImGui.IsItemDeactivatedAfterEdit())
			{
				config.Save(this);
			}
			ImU8String val4 = ImU8String.op_Implicit(LanguageManager.Get("Size"));
			ref float size = ref config.Size;
			ImU8String val5 = default(ImU8String);
			if (ImGui.InputFloat(val4, ref size, 0f, 0f, val5, (ImGuiInputTextFlags)0))
			{
				config.Size = MathF.Max(1f, config.Size);
			}
			if (ImGui.IsItemDeactivatedAfterEdit())
			{
				config.Save(this);
			}
			ImU8String val6 = ImU8String.op_Implicit(LanguageManager.Get("Icon"));
			ref uint iconID = ref config.IconID;
			val5 = default(ImU8String);
			ImGui.InputUInt(val6, ref iconID, 0u, 0u, val5, (ImGuiInputTextFlags)0);
			if (ImGui.IsItemDeactivatedAfterEdit())
			{
				config.Save(this);
			}
			ImGui.SameLine();
			val5 = new ImU8String(0, 1);
			((ImU8String)(ref val5)).AppendFormatted<string>(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)63597));
			if (ImGui.Button(val5, default(Vector2)))
			{
				OmenServiceBase<ChatManager>.Instance().SendMessage("/xldata icon");
			}
			ImGuiOm.TooltipHover(LanguageManager.Get("IconBrowser") + "\n(" + LanguageManager.Get("IconBrowser-Suggestion") + ")");
		}
		finally
		{
			((IDisposable)val3)?.Dispose();
		}
		ImGui.NewLine();
		ImU8String val7 = default(ImU8String);
		((ImU8String)(ref val7))._002Ector(0, 1);
		((ImU8String)(ref val7)).AppendFormatted<string>(LanguageManager.Get("OnlyInCombat"));
		if (ImGui.Checkbox(val7, ref config.OnlyShowInCombat))
		{
			config.Save(this);
		}
		ImU8String val8 = default(ImU8String);
		((ImU8String)(ref val8))._002Ector(0, 1);
		((ImU8String)(ref val8)).AppendFormatted<string>(LanguageManager.Get("OnlyInDuty"));
		if (ImGui.Checkbox(val8, ref config.OnlyShowInDuty))
		{
			config.Save(this);
		}
	}
}
