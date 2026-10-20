using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using Dalamud.Game;
using Dalamud.Game.Text.Evaluator;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Common.Math;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Models;

namespace OmenTools.KamiToolKit.Addons.SelectYesno;

public sealed class SelectYesnoAddon : NativeAddon
{
	private readonly SelectYesnoAddonOptions options;

	private Vector2? openPosition;

	private bool hasResult;

	private TextNode? PromptNode { get; set; }

	private TextButtonNode? PrimaryButton { get; set; }

	private TextButtonNode? SecondaryButton { get; set; }

	public static SelectYesnoAddon Open(SelectYesnoAddonOptions options)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		ArgumentNullException.ThrowIfNull(options, "options");
		ValidateOptions(options);
		SelectYesnoAddon obj = new SelectYesnoAddon(options)
		{
			InternalName = "DRSelectYesno",
			Title = ReadOnlySeString.op_Implicit(string.Empty),
			Size = new Vector2(400f, 96f),
			OpenInBounds = true,
			OpenWindowSoundEffectId = options.OpenSoundEffectID,
			RespectCloseAll = options.RespectCloseAll,
			DisableClamping = false,
			EnableContextMenu = false,
			DisableScaleContextOption = true,
			RememberClosePosition = false,
			ParentAddonId = options.ParentID,
			BlockedParentAddonId = options.BlockedParentID,
			CreateWindowNode = delegate
			{
				WindowNode windowNode = new WindowNode();
				windowNode.ShowCloseButton = true;
				windowNode.ShowConfigButton = false;
				windowNode.ShowHelpButton = false;
				windowNode.HeaderContainerNode.IsVisible = true;
				windowNode.HeaderCollisionNode.IsVisible = true;
				windowNode.TitleNode.IsVisible = false;
				windowNode.SubtitleNode.IsVisible = false;
				windowNode.DividingLineNode.IsVisible = false;
				return windowNode;
			}
		};
		obj.Open();
		return obj;
	}

	protected unsafe override void OnSetup(AtkUnitBase* addon, Span<AtkValue> atkValueSpan)
	{
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		hasResult = false;
		if (base.WindowNode is WindowNode windowNode)
		{
			windowNode.ShowCloseButton = true;
			windowNode.HeaderContainerNode.IsVisible = true;
			windowNode.HeaderCollisionNode.IsVisible = true;
			windowNode.TitleNode.IsVisible = false;
			windowNode.SubtitleNode.IsVisible = false;
			windowNode.DividingLineNode.IsVisible = false;
		}
		PromptNode = new TextNode
		{
			Size = new Vector2(344f, 0f),
			TextColor = ColorHelper.GetColor(1u),
			FontSize = 14u,
			FontType = (FontType)0,
			LineSpacing = 18u,
			AlignmentType = options.PromptAlignment
		};
		PromptNode.AddTextFlags((TextFlags)64, (TextFlags)128);
		PromptNode.AttachNode(this);
		PrimaryButton = new TextButtonNode
		{
			Size = new Vector2(100f, 28f),
			OnClick = delegate
			{
				Select(SelectYesnoAddonResult.Yes);
			}
		};
		PrimaryButton.AttachNode(this);
		SecondaryButton = new TextButtonNode
		{
			Size = new Vector2(100f, 28f),
			OnClick = delegate
			{
				Select(SelectYesnoAddonResult.No);
			}
		};
		SecondaryButton.AttachNode(this);
		ApplyOptions(options);
	}

	protected unsafe override void OnHide(AtkUnitBase* addon)
	{
		if (!hasResult)
		{
			hasResult = true;
			DalamudServiceExtension.Instance<IFramework>().RunOnTick((Action)delegate
			{
				options.Callback?.Invoke(this, SelectYesnoAddonResult.Closed);
			}, default(TimeSpan), 1, default(CancellationToken));
		}
	}

	protected unsafe override void OnUpdate(AtkUnitBase* addon)
	{
		if (openPosition.HasValue)
		{
			SetWindowPosition(openPosition.Value);
			openPosition = null;
		}
	}

	protected unsafe override void OnFinalize(AtkUnitBase* addon)
	{
		PromptNode = null;
		PrimaryButton = null;
		SecondaryButton = null;
	}

	private SelectYesnoAddon(SelectYesnoAddonOptions options)
	{
		this.options = options;
	}

	private unsafe void ApplyOptions(SelectYesnoAddonOptions dialogOptions)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		if (PromptNode != null && PrimaryButton != null && SecondaryButton != null)
		{
			PromptNode.AlignmentType = dialogOptions.PromptAlignment;
			PromptNode.String = dialogOptions.Prompt;
			SetButtonText(PrimaryButton, dialogOptions.YesButtonText, 3u);
			SetButtonText(SecondaryButton, dialogOptions.NoButtonText, 4u);
			SelectYesnoAddonButtons buttons = dialogOptions.Buttons;
			bool flag = (buttons & SelectYesnoAddonButtons.Yes) != 0;
			bool flag2 = (buttons & SelectYesnoAddonButtons.No) != 0;
			PrimaryButton.IsVisible = flag;
			SecondaryButton.IsVisible = flag2;
			PromptNode.Size = new Vector2(344f, 0f);
			float y = PromptNode.GetTextDrawSize().Y;
			int num = (flag ? 1 : 0) + (flag2 ? 1 : 0);
			float num2 = ((num == 0) ? 0f : 28f);
			float num3 = ((num == 0) ? 0f : 8f);
			float num4 = MathF.Max(96f, 20f + y + num3 + num2 + 18f);
			SetWindowSize(400f, num4);
			PromptNode.Size = new Vector2(344f, y);
			PromptNode.Position = new Vector2(28f, 20f);
			float num5 = ((num == 2) ? 208f : 100f);
			float num6 = (400f - num5) / 2f;
			float y2 = num4 - num2 - 18f;
			List<TextButtonNode> list = new List<TextButtonNode>(num);
			if (flag)
			{
				list.Add(PrimaryButton);
			}
			if (flag2)
			{
				list.Add(SecondaryButton);
			}
			for (int i = 0; i < list.Count; i++)
			{
				TextButtonNode textButtonNode = list[i];
				int navIndex = i + 1;
				int navLeft = ((i == 0) ? list.Count : i);
				int navRight = ((i == list.Count - 1) ? 1 : (i + 2));
				textButtonNode.Position = new Vector2(num6 + (float)i * 108f, y2);
				textButtonNode.NavIndex = navIndex;
				textButtonNode.NavLeft = navLeft;
				textButtonNode.NavRight = navRight;
			}
			if (!flag)
			{
				PrimaryButton.NavIndex = 0;
			}
			if (!flag2)
			{
				SecondaryButton.NavIndex = 0;
			}
			if (base.InternalAddon != null)
			{
				((AtkUnitBase)base.InternalAddon).FocusNode = (flag ? ((AtkResNode*)PrimaryButton) : (flag2 ? ((AtkResNode*)SecondaryButton) : ((AtkResNode*)base.RootNode)));
			}
			Vector2 vector = Size.op_Implicit(((AtkStage)AtkStage.Instance()).ScreenSize);
			Vector2 vector2 = Vector2.Max(Vector2.Zero, vector - base.Size);
			AddonPosition position = options.Position;
			Vector2 value = (((object)position != null) ? Vector2.Clamp(GetWindowPosition(position, (*base.RootNode.Node).GetNodeState().Size), Vector2.Zero, vector2) : (vector2 / 2f));
			openPosition = value;
		}
	}

	private static Vector2 GetWindowPosition(AddonPosition addonPosition, Vector2 addonSize)
	{
		Vector2 vector;
		switch (addonPosition.Alignment)
		{
		case AddonPositionAlignment.TopLeft:
			vector = Vector2.Zero;
			break;
		case AddonPositionAlignment.TopCenter:
			vector = new Vector2(addonSize.X / 2f, 0f);
			break;
		case AddonPositionAlignment.TopRight:
		{
			Vector2 vector2 = addonSize;
			vector2.Y = 0f;
			vector = vector2;
			break;
		}
		case AddonPositionAlignment.RightCenter:
		{
			Vector2 vector2 = addonSize;
			vector2.Y = addonSize.Y / 2f;
			vector = vector2;
			break;
		}
		case AddonPositionAlignment.BottomRight:
			vector = new Vector2(addonSize.X, addonSize.Y);
			break;
		case AddonPositionAlignment.BottomCenter:
		{
			Vector2 vector2 = addonSize;
			vector2.X = addonSize.X / 2f;
			vector = vector2;
			break;
		}
		case AddonPositionAlignment.BottomLeft:
		{
			Vector2 vector2 = addonSize;
			vector2.X = 0f;
			vector = vector2;
			break;
		}
		case AddonPositionAlignment.LeftCenter:
			vector = new Vector2(0f, addonSize.Y / 2f);
			break;
		case AddonPositionAlignment.Center:
			vector = new Vector2(addonSize.X / 2f, addonSize.Y / 2f);
			break;
		default:
			vector = Vector2.Zero;
			break;
		}
		Vector2 vector3 = vector;
		return addonPosition.Position - vector3;
	}

	private static void SetButtonText(TextButtonNode button, ReadOnlySeString? text, uint defaultTextID)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		button.TextId = 0u;
		button.String = (ReadOnlySeString)(((_003F?)text) ?? DalamudServiceExtension.Instance<ISeStringEvaluator>().EvaluateFromAddon(defaultTextID, default(Span<SeStringParameter>), (ClientLanguage?)null));
	}

	private void Select(SelectYesnoAddonResult result)
	{
		if (!hasResult)
		{
			hasResult = true;
			Action<SelectYesnoAddon, SelectYesnoAddonResult>? callback = options.Callback;
			Close();
			callback?.Invoke(this, result);
		}
	}

	private static void ValidateOptions(SelectYesnoAddonOptions options)
	{
		if ((options.Buttons & ~SelectYesnoAddonButtons.Both) != SelectYesnoAddonButtons.None)
		{
			throw new ArgumentOutOfRangeException("Buttons");
		}
		Vector2? vector = options.Position?.Position;
		if (vector.HasValue)
		{
			Vector2 valueOrDefault = vector.GetValueOrDefault();
			if (!float.IsFinite(valueOrDefault.X) || !float.IsFinite(valueOrDefault.Y))
			{
				throw new ArgumentOutOfRangeException("Position");
			}
		}
	}
}
