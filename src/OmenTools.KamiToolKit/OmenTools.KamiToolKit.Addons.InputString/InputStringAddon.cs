using System;
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

namespace OmenTools.KamiToolKit.Addons.InputString;

public sealed class InputStringAddon : NativeAddon
{
	private const float WINDOW_WIDTH = 320f;

	private const float PROMPT_X = 17f;

	private const float PROMPT_Y = 15f;

	private const float PROMPT_WIDTH = 276f;

	private const float LABEL_X = 18f;

	private const float LABEL_WIDTH = 284f;

	private const float LABEL_HEIGHT = 21f;

	private const float INPUT_X = 17f;

	private const float INPUT_WIDTH = 286f;

	private const float INPUT_HEIGHT = 28f;

	private const float BUTTON_WIDTH = 100f;

	private const float BUTTON_HEIGHT = 28f;

	private const float BUTTON_GAP = 8f;

	private const float PROMPT_TO_LABEL_GAP = 18f;

	private const float LABEL_TO_INPUT_GAP = 16f;

	private const float INPUT_TO_BUTTON_GAP = 10f;

	private const float BOTTOM_PADDING = 18f;

	private readonly InputStringAddonOptions options;

	private Vector2? openPosition;

	private bool hasResult;

	public string Value
	{
		get
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			TextInputNode? inputNode = InputNode;
			return ((inputNode != null) ? ((object)inputNode.String/*cast due to constrained. prefix*/).ToString() : null) ?? string.Empty;
		}
	}

	private TextNode? PromptNode { get; set; }

	private TextNode? LabelNode { get; set; }

	private TextInputNode? InputNode { get; set; }

	private TextButtonNode? ConfirmButton { get; set; }

	private TextButtonNode? CancelButton { get; set; }

	public static InputStringAddon Open(InputStringAddonOptions options)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		ArgumentNullException.ThrowIfNull(options, "options");
		ValidateOptions(options);
		InputStringAddon obj = new InputStringAddon(options)
		{
			InternalName = "DRInputString",
			Title = ReadOnlySeString.op_Implicit(string.Empty),
			Size = new Vector2(320f, 154f),
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
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
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
			Size = new Vector2(276f, 0f),
			TextColor = ColorHelper.GetColor(8u),
			TextOutlineColor = ColorHelper.GetColor(7u),
			FontSize = 14u,
			FontType = (FontType)0,
			LineSpacing = 21u,
			AlignmentType = options.PromptAlignment
		};
		PromptNode.AddTextFlags((TextFlags)64, (TextFlags)128);
		PromptNode.AttachNode(this);
		LabelNode = new TextNode
		{
			Size = new Vector2(284f, 21f),
			TextColor = ColorHelper.GetColor(3u),
			TextOutlineColor = ColorHelper.GetColor(7u),
			FontSize = 12u,
			FontType = (FontType)0
		};
		LabelNode.AttachNode(this);
		InputNode = new TextInputNode();
		InputNode.AttachNode(this);
		InputNode.OnInputComplete = delegate
		{
			Select(InputStringAddonResult.Confirmed);
		};
		InputNode.OnEscapeEntered = delegate
		{
			Select(InputStringAddonResult.Cancelled);
		};
		ConfirmButton = new TextButtonNode
		{
			Size = new Vector2(100f, 28f),
			OnClick = delegate
			{
				Select(InputStringAddonResult.Confirmed);
			}
		};
		ConfirmButton.AttachNode(this);
		CancelButton = new TextButtonNode
		{
			Size = new Vector2(100f, 28f),
			OnClick = delegate
			{
				Select(InputStringAddonResult.Cancelled);
			}
		};
		CancelButton.AttachNode(this);
		ApplyOptions(options);
	}

	protected unsafe override void OnHide(AtkUnitBase* addon)
	{
		if (!hasResult)
		{
			hasResult = true;
			DalamudServiceExtension.Instance<IFramework>().RunOnTick((Action)delegate
			{
				options.Callback?.Invoke(this, InputStringAddonResult.Closed);
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
		LabelNode = null;
		InputNode = null;
		ConfirmButton = null;
		CancelButton = null;
	}

	private InputStringAddon(InputStringAddonOptions options)
	{
		this.options = options;
	}

	private unsafe void ApplyOptions(InputStringAddonOptions dialogOptions)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		if (PromptNode != null && LabelNode != null && InputNode != null && ConfirmButton != null && CancelButton != null)
		{
			PromptNode.AlignmentType = dialogOptions.PromptAlignment;
			PromptNode.String = dialogOptions.Prompt;
			bool flag = dialogOptions.Label.HasValue;
			LabelNode.IsVisible = flag;
			LabelNode.String = (ReadOnlySeString)(((_003F?)dialogOptions.Label) ?? ReadOnlySeString.op_Implicit(string.Empty));
			SetButtonText(ConfirmButton, dialogOptions.ConfirmButtonText, 572u);
			SetButtonText(CancelButton, dialogOptions.CancelButtonText, 2u);
			InputNode.String = (ReadOnlySeString)(((_003F?)dialogOptions.Value) ?? ReadOnlySeString.op_Implicit(string.Empty));
			InputNode.PlaceholderString = dialogOptions.Placeholder;
			InputNode.MaxCharacters = dialogOptions.MaxCharacters;
			InputNode.ShowLimitText = dialogOptions.MaxCharacters > 0;
			PromptNode.Size = new Vector2(276f, 0f);
			float y = PromptNode.GetTextDrawSize().Y;
			float num = 15f + y + 18f;
			float num2 = (flag ? (num + 16f) : num);
			float num3 = num2 + 28f + 10f;
			float height = num3 + 28f + 18f;
			SetWindowSize(320f, height);
			PromptNode.Size = new Vector2(276f, y);
			PromptNode.Position = new Vector2(17f, 15f);
			LabelNode.Size = new Vector2(284f, 21f);
			LabelNode.Position = new Vector2(18f, num);
			InputNode.Size = new Vector2(286f, 28f);
			InputNode.Position = new Vector2(17f, num2);
			float num4 = 208f;
			float num5 = (320f - num4) / 2f;
			ConfirmButton.Position = new Vector2(num5, num3);
			CancelButton.Position = new Vector2(num5 + 100f + 8f, num3);
			ConfirmButton.NavIndex = 1;
			ConfirmButton.NavLeft = 2;
			ConfirmButton.NavRight = 2;
			CancelButton.NavIndex = 2;
			CancelButton.NavLeft = 1;
			CancelButton.NavRight = 1;
			if (base.InternalAddon != null)
			{
				((AtkUnitBase)base.InternalAddon).FocusNode = InputNode;
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

	private void Select(InputStringAddonResult result)
	{
		if (!hasResult)
		{
			hasResult = true;
			Action<InputStringAddon, InputStringAddonResult>? callback = options.Callback;
			Close();
			callback?.Invoke(this, result);
		}
	}

	private static void ValidateOptions(InputStringAddonOptions options)
	{
		if (options.MaxCharacters < 0)
		{
			throw new ArgumentOutOfRangeException("MaxCharacters");
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
