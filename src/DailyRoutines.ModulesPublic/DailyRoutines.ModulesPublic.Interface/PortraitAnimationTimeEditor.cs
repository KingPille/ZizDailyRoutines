using System;
using System.Numerics;
using System.Runtime.InteropServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Graphics.Render;
using FFXIVClientStructs.FFXIV.Client.System.Scheduler.Base;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Havok.Animation.Animation;
using FFXIVClientStructs.Havok.Animation.Playback;
using FFXIVClientStructs.Havok.Animation.Playback.Control;
using FFXIVClientStructs.Havok.Animation.Playback.Control.Default;
using FFXIVClientStructs.Havok.Common.Base.Container.Array;
using FFXIVClientStructs.Interop;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using Lumina.Text.ReadOnly;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.Lumina;
using OmenTools.KamiToolKit.Addons;

namespace DailyRoutines.ModulesPublic.Interface;

public class PortraitAnimationTimeEditor : ModuleBase
{
	private sealed class PortraitAnimationTimeEditorAddon(PortraitAnimationTimeEditor module) : AttachedAddon("BannerEditor")
	{
		private const float BUTTON_SIZE = 28f;

		private const float BUTTON_SPACING = 4f;

		private const float CONTROL_WIDTH = 92f;

		private TextNode frameLabel;

		private FloatSliderNode frameSlider;

		private CircleButtonNode playbackButton;

		private CircleButtonNode[] controlButtons = Array.Empty<CircleButtonNode>();

		private bool isSyncing;

		private bool? pendingPlaybackState;

		protected override AttachedAddonPosition AttachPosition => AttachedAddonPosition.RightTop;

		protected unsafe override void OnSetup(AtkUnitBase* addon, Span<AtkValue> atkValues)
		{
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			if (base.WindowNode is WindowNode windowNode)
			{
				windowNode.CloseButtonNode.IsVisible = false;
			}
			VerticalListNode verticalListNode = new VerticalListNode
			{
				FitContents = true,
				Position = base.ContentStartPosition
			};
			verticalListNode.AttachNode(this);
			HorizontalListNode horizontalListNode = new HorizontalListNode();
			Vector2 contentSize = base.ContentSize;
			contentSize.Y = 28f;
			horizontalListNode.Size = contentSize;
			horizontalListNode.FirstItemSpacing = (base.ContentSize.X - 92f) / 2f;
			horizontalListNode.ItemSpacing = 4f;
			HorizontalListNode horizontalListNode2 = horizontalListNode;
			controlButtons = new CircleButtonNode[3]
			{
				CreateControlButton(CircleButtonIcon.LeftArrow, "-1", delegate
				{
					module.SetCurrentFrame(MathF.Ceiling(module.currentFrame) - 1f);
				}),
				(playbackButton = CreateControlButton(CircleButtonIcon.MusicNote, LuminaWrapper.GetAddonText(4802u), TogglePlayback)),
				CreateControlButton(CircleButtonIcon.RightArrow, "+1", delegate
				{
					module.SetCurrentFrame(MathF.Floor(module.currentFrame) + 1f);
				})
			};
			horizontalListNode2.AddNode(controlButtons);
			horizontalListNode2.RecalculateLayout();
			verticalListNode.AddNode(horizontalListNode2);
			frameLabel = new TextNode
			{
				AlignmentType = (AlignmentType)4,
				TextFlags = (TextFlags)9,
				String = ReadOnlySeString.op_Implicit("000.00 / 000"),
				Size = new Vector2(base.Size.X - 24f, 28f)
			};
			verticalListNode.AddNode(frameLabel);
			frameSlider = new FloatSliderNode
			{
				Min = 0f,
				Max = 1f,
				Step = 1f,
				Size = new Vector2(base.Size.X + 20f, 28f),
				OnValueChanged = delegate(float value)
				{
					if (!isSyncing)
					{
						module.SetCurrentFrame(value);
					}
				}
			};
			frameSlider.FloatValueNode.FontSize = 0u;
			verticalListNode.AddNode(frameSlider);
		}

		protected unsafe override void OnAttachedAddonUpdate(AtkUnitBase* addon, AtkUnitBase* hostAddon)
		{
			if (((AtkUnitBase)hostAddon).GetNodeById(107u) != null)
			{
				SyncControls(module.UpdateAnimationState());
			}
		}

		private static CircleButtonNode CreateControlButton(CircleButtonIcon icon, string tooltip, Action onClick)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			return new CircleButtonNode
			{
				IsVisible = true,
				IsEnabled = true,
				Size = new Vector2(28f),
				Icon = icon,
				TextTooltip = ReadOnlySeString.op_Implicit(tooltip),
				OnClick = onClick
			};
		}

		private unsafe void TogglePlayback()
		{
			CharaViewPortrait* charaView = CharaView;
			hkaDefaultAnimationControl* animationControl = GetAnimationControl((charaView == null) ? null : ((CharaViewPortrait)charaView).GetCharacter());
			AddonBannerEditor* bannerEditor = (AddonBannerEditor*)Addons.BannerEditor;
			if (charaView != null && animationControl != null && bannerEditor != null)
			{
				bool flag = pendingPlaybackState ?? (((hkaDefaultAnimationControl)animationControl).PlaybackSpeed > 0f);
				((CharaViewPortrait)charaView).ToggleAnimationPlayback(flag);
				((AtkComponentButton)(&((AtkComponentCheckBox)((AddonBannerEditor)bannerEditor).PlayAnimationCheckbox).AtkComponentButton)).IsChecked = false;
				SetPlaybackState(!flag, refreshTooltip: true);
			}
		}

		private unsafe void SyncControls(hkaDefaultAnimationControl* animation)
		{
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			bool flag = animation != null;
			float num = MathF.Max(1f, module.frameCount);
			CircleButtonNode[] array = controlButtons;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].IsEnabled = flag;
			}
			frameSlider.IsEnabled = flag;
			bool flag2 = flag && ((hkaDefaultAnimationControl)animation).PlaybackSpeed > 0f;
			if (!flag || pendingPlaybackState == flag2)
			{
				pendingPlaybackState = null;
			}
			UpdatePlaybackButton(pendingPlaybackState ?? flag2);
			frameLabel.String = ReadOnlySeString.op_Implicit((module.frameCount < 100) ? $"{module.currentFrame:F3} / {module.frameCount}" : $"{module.currentFrame:F2} / {module.frameCount}");
			isSyncing = true;
			if (MathF.Abs(frameSlider.Max - num) > 0.001f)
			{
				frameSlider.Max = num;
			}
			frameSlider.Value = module.currentFrame;
			isSyncing = false;
		}

		public void SetPlaybackState(bool isPlaying, bool refreshTooltip = false)
		{
			pendingPlaybackState = isPlaying;
			UpdatePlaybackButton(isPlaying);
			if (refreshTooltip)
			{
				playbackButton.HideTooltip();
				playbackButton.ShowTooltip();
			}
		}

		private void UpdatePlaybackButton(bool isPlaying)
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			playbackButton.Icon = (isPlaying ? CircleButtonIcon.WavePulse : CircleButtonIcon.MusicNote);
			playbackButton.TextTooltip = ReadOnlySeString.op_Implicit(isPlaying ? LuminaWrapper.GetAddonText(13910u) : LuminaWrapper.GetAddonText(4802u));
		}
	}

	[StructLayout(LayoutKind.Explicit)]
	private struct Actor
	{
		[FieldOffset(256)]
		public unsafe ActorModel* Model;
	}

	[StructLayout(LayoutKind.Explicit)]
	private struct ActorModel
	{
		[FieldOffset(160)]
		public unsafe Skeleton* Skeleton;
	}

	private int frameCount;

	private float currentFrame;

	private PortraitAnimationTimeEditorAddon? addon;

	public override ModuleInfo Info => new ModuleInfo
	{
		Title = LanguageManager.Get("PortraitAnimationTimeEditorTitle"),
		Description = LanguageManager.Get("PortraitAnimationTimeEditorDescription"),
		Author = new string[1] { "Yarukon" },
		Category = ModuleCategory.Interface
	};

	private unsafe static AgentBannerEditorState* EditorState => ((AgentBannerEditor)AgentBannerEditor.Instance()).EditorState;

	private unsafe static CharaViewPortrait* CharaView
	{
		get
		{
			if (EditorState == null)
			{
				return null;
			}
			return ((AgentBannerEditorState)EditorState).CharaView;
		}
	}

	protected override void Init()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (addon == null)
		{
			addon = new PortraitAnimationTimeEditorAddon(this)
			{
				InternalName = "DRPortraitAnimationTimeEditor",
				Title = ReadOnlySeString.op_Implicit(LanguageManager.Get("PortraitAnimationTimeEditor-AddonTitle")),
				Size = new Vector2(300f, 136f),
				RememberClosePosition = false
			};
		}
	}

	protected override void Uninit()
	{
		addon?.Dispose();
		addon = null;
	}

	private void SetCurrentFrame(float frame)
	{
		currentFrame = Math.Clamp(frame, 0f, frameCount);
		UpdatePortraitCurrentFrame(currentFrame);
		addon?.SetPlaybackState(isPlaying: true);
	}

	private unsafe hkaDefaultAnimationControl* UpdateAnimationState()
	{
		CharaViewPortrait* charaView = CharaView;
		Character* ptr = ((charaView == null) ? null : ((CharaViewPortrait)charaView).GetCharacter());
		hkaDefaultAnimationControl* animationControl = GetAnimationControl(ptr);
		hkaAnimationBinding* ptr2 = ((animationControl == null) ? null : ((hkaAnimationControl)(&((hkaDefaultAnimationControl)animationControl).hkaAnimationControl)).Binding.ptr);
		hkaAnimation* ptr3 = ((ptr2 == null) ? null : ((hkaAnimationBinding)ptr2).Animation.ptr);
		SchedulerTimeline* ptr4 = ((ptr == null) ? null : ((ActionTimelineSequencer)(&((TimelineContainer)(&((Character)ptr).Timeline)).TimelineSequencer)).GetSchedulerTimeline(0u));
		if (charaView == null || ptr3 == null || ptr4 == null)
		{
			frameCount = 0;
			currentFrame = 0f;
			return null;
		}
		frameCount = Math.Max(0, (int)Math.Round(30f * (((hkaAnimation)ptr3).Duration - 0.5f)));
		currentFrame = Math.Clamp(((CharaViewPortrait)charaView).GetAnimationTime(), 0f, frameCount);
		return animationControl;
	}

	private unsafe static void UpdatePortraitCurrentFrame(float frame)
	{
		AgentBannerEditorState* editorState = EditorState;
		CharaViewPortrait* ptr = ((editorState == null) ? null : ((AgentBannerEditorState)editorState).CharaView);
		Character* ptr2 = ((ptr == null) ? null : ((CharaViewPortrait)ptr).GetCharacter());
		AddonBannerEditor* bannerEditor = (AddonBannerEditor*)Addons.BannerEditor;
		if (editorState == null || ptr == null || ptr2 == null || bannerEditor == null)
		{
			return;
		}
		SchedulerTimeline* schedulerTimeline = ((ActionTimelineSequencer)(&((TimelineContainer)(&((Character)ptr2).Timeline)).TimelineSequencer)).GetSchedulerTimeline(0u);
		if (schedulerTimeline != null)
		{
			float num = frame - ((TimelineController)(&((SchedulerTimeline)schedulerTimeline).TimelineController)).CurrentTimestamp;
			if (num < 0f)
			{
				((CharaViewPortrait)ptr).SetPoseTimed(((TimelineContainer)(&((Character)ptr2).Timeline)).BannerTimelineRowId, frame);
			}
			else
			{
				((SchedulerTimeline)schedulerTimeline).UpdateBanner(num, (byte)0);
			}
			((CharaViewPortrait)ptr).ToggleAnimationPlayback(true);
			((AtkComponentButton)(&((AtkComponentCheckBox)((AddonBannerEditor)bannerEditor).PlayAnimationCheckbox).AtkComponentButton)).IsChecked = false;
			if (!((AgentBannerEditorState)editorState).HasDataChanged)
			{
				((AgentBannerEditorState)editorState).SetHasChanged(true);
			}
		}
	}

	private unsafe static hkaDefaultAnimationControl* GetAnimationControl(Character* charaActor)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		if (charaActor == null)
		{
			return null;
		}
		ActorModel* model = ((Actor*)charaActor)->Model;
		Skeleton* ptr = ((model == null) ? null : model->Skeleton);
		PartialSkeleton* ptr2 = ((ptr == null) ? null : ((Skeleton)ptr).PartialSkeletons);
		hkaAnimatedSkeleton* ptr3 = ((ptr2 == null) ? null : ((PartialSkeleton)ptr2).GetHavokAnimatedSkeleton(0));
		if (ptr3 == null)
		{
			return null;
		}
		hkArray<Pointer<hkaDefaultAnimationControl>> animationControls = ((hkaAnimatedSkeleton)ptr3).AnimationControls;
		return Pointer<hkaDefaultAnimationControl>.op_Implicit((animationControls.Length == 0) ? Pointer<hkaDefaultAnimationControl>.op_Implicit((hkaDefaultAnimationControl*)null) : animationControls[0]);
	}
}
