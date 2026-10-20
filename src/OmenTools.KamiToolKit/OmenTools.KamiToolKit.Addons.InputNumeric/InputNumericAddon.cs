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

namespace OmenTools.KamiToolKit.Addons.InputNumeric;

public sealed class InputNumericAddon : NativeAddon
{
	private readonly InputNumericAddonOptions options;

	private Vector2? openPosition;

	private bool hasResult;

	public int Value => InputNode?.Value ?? 0;

	private TextNode? PromptNode { get; set; }

	private NumericInputNode? InputNode { get; set; }

	private TextButtonNode? ConfirmButton { get; set; }

	private TextButtonNode? CancelButton { get; set; }

	public static InputNumericAddon Open(InputNumericAddonOptions options)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		ArgumentNullException.ThrowIfNull(options, "options");
		ValidateOptions(options);
		InputNumericAddon obj = new InputNumericAddon(options)
		{
			InternalName = "DRInputNumeric",
			Title = ReadOnlySeString.op_Implicit(string.Empty),
			Size = new Vector2(280f, 130f),
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
			Size = new Vector2(224f, 0f),
			TextColor = ColorHelper.GetColor(8u),
			TextOutlineColor = ColorHelper.GetColor(7u),
			FontSize = 12u,
			FontType = (FontType)0,
			LineSpacing = 18u,
			AlignmentType = options.PromptAlignment
		};
		PromptNode.AddTextFlags((TextFlags)64, (TextFlags)128);
		PromptNode.AttachNode(this);
		InputNode = new NumericInputNode();
		InputNode.AttachNode(this);
		ConfirmButton = new TextButtonNode
		{
			Size = new Vector2(100f, 28f),
			OnClick = delegate
			{
				Select(InputNumericAddonResult.Confirmed);
			}
		};
		ConfirmButton.AttachNode(this);
		CancelButton = new TextButtonNode
		{
			Size = new Vector2(100f, 28f),
			OnClick = delegate
			{
				Select(InputNumericAddonResult.Cancelled);
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
				options.Callback?.Invoke(this, InputNumericAddonResult.Closed);
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
		InputNode = null;
		ConfirmButton = null;
		CancelButton = null;
	}

	private InputNumericAddon(InputNumericAddonOptions options)
	{
		this.options = options;
	}

	private unsafe void ApplyOptions(InputNumericAddonOptions dialogOptions)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		if (PromptNode != null && InputNode != null && ConfirmButton != null && CancelButton != null)
		{
			PromptNode.AlignmentType = dialogOptions.PromptAlignment;
			PromptNode.String = dialogOptions.Prompt;
			SetButtonText(ConfirmButton, dialogOptions.ConfirmButtonText, 572u);
			SetButtonText(CancelButton, dialogOptions.CancelButtonText, 2u);
			InputNode.Min = dialogOptions.Min;
			InputNode.Max = dialogOptions.Max;
			InputNode.Step = dialogOptions.Step;
			InputNode.Value = dialogOptions.Value;
			PromptNode.Size = new Vector2(224f, 0f);
			float y = PromptNode.GetTextDrawSize().Y;
			float num = 22f + y + 8f;
			float num2 = num + 28f + 10f;
			float height = num2 + 28f + 18f;
			SetWindowSize(280f, height);
			PromptNode.Size = new Vector2(224f, y);
			PromptNode.Position = new Vector2(28f, 22f);
			InputNode.Size = new Vector2(146f, 28f);
			InputNode.Position = new Vector2(67f, num);
			float num3 = 208f;
			float num4 = (280f - num3) / 2f;
			ConfirmButton.Position = new Vector2(num4, num2);
			CancelButton.Position = new Vector2(num4 + 100f + 8f, num2);
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

	private void Select(InputNumericAddonResult result)
	{
		if (!hasResult)
		{
			hasResult = true;
			Action<InputNumericAddon, InputNumericAddonResult>? callback = options.Callback;
			Close();
			callback?.Invoke(this, result);
		}
	}

	private static void ValidateOptions(InputNumericAddonOptions options)
	{
		if (options.Min > options.Max)
		{
			throw new ArgumentOutOfRangeException("Min");
		}
		if (options.Step <= 0)
		{
			throw new ArgumentOutOfRangeException("Step");
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
