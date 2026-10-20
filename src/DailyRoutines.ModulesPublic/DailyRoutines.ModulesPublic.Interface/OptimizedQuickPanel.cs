using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Hooking;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.Classes;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using KamiToolKit.Nodes.Simplified;
using KamiToolKit.Timelines;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic.Interface;

public class OptimizedQuickPanel : ModuleBase
{
	private unsafe delegate void AddonControlReceiveEventDelegate(AtkAddonControl* control, ushort eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData);

	[return: MarshalAs(UnmanagedType.U1)]
	private unsafe delegate bool IsMoveHandleNodeDelegate(AtkUnitBase* addon, AtkResNode* node);

	private unsafe delegate void StartDraggingAddonDelegate(AtkUnitManager* manager, AtkUnitBase* addon);

	private unsafe delegate void QuickPanelReceiveEventDelegate(AtkUnitBase* addon, ushort eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData);

	private class Config : ModuleConfig
	{
		public bool IsLock = true;
	}

	private static readonly CompSig AddonControlReceiveEventSig = new CompSig("40 53 56 41 56 48 81 EC ?? ?? ?? ?? 48 8B F1");

	private Hook<AddonControlReceiveEventDelegate>? AddonControlReceiveEventHook;

	private static readonly CompSig IsMoveHandleNodeSig = new CompSig("48 3B 91 ?? ?? ?? ?? 74 ?? 45 33 C0");

	private Hook<IsMoveHandleNodeDelegate>? IsMoveHandleNodeHook;

	private static readonly CompSig StartDraggingAddonSig = new CompSig("40 53 48 83 EC 20 80 A2 A1 01 00 00 EF");

	private Hook<StartDraggingAddonDelegate>? StartDraggingAddonHook;

	private static readonly CompSig QuickPanelReceiveEventSig = new CompSig("40 55 53 56 57 41 56 48 8D 6C 24 ?? 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 45 ?? 48 8B 7D");

	private Hook<QuickPanelReceiveEventDelegate>? QuickPanelReceiveEventHook;

	private Config config;

	private CheckboxNode? lockCheckBoxNode;

	private const ushort ATK_EVENT_TYPE_MOUSE_MOVE = 5;

	private const int QUICK_PANEL_SLOT_ARRAY_OFFSET = 576;

	private const int QUICK_PANEL_SLOT_ARRAY_COUNT = 25;

	private const int QUICK_PANEL_SLOT_SIZE = 48;

	private static readonly TextCommand QuickPanelLine = LuminaGetter.GetRowOrDefault<TextCommand>(50u);

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("OptimizedQuickPanelTitle"),
		Description = LanguageManager.Get("OptimizedQuickPanelDescription", ((TextCommand)(ref QuickPanelLine)).Command, ((TextCommand)(ref QuickPanelLine)).Alias),
		Category = ModuleCategory.Interface
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected unsafe override void Init()
	{
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Expected O, but got Unknown
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Expected O, but got Unknown
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Expected O, but got Unknown
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		OmenServiceBase<ChatManager>.Instance().RegPreExecuteCommandInner(OnPreExecuteCommandInner);
		StartDraggingAddonHook = StartDraggingAddonSig.GetHook<StartDraggingAddonDelegate>(StartDraggingAddonDetour);
		StartDraggingAddonHook.Enable();
		AddonControlReceiveEventHook = AddonControlReceiveEventSig.GetHook<AddonControlReceiveEventDelegate>(AddonControlReceiveEventDetour);
		AddonControlReceiveEventHook.Enable();
		IsMoveHandleNodeHook = IsMoveHandleNodeSig.GetHook<IsMoveHandleNodeDelegate>(IsMoveHandleNodeDetour);
		IsMoveHandleNodeHook.Enable();
		QuickPanelReceiveEventHook = QuickPanelReceiveEventSig.GetHook<QuickPanelReceiveEventDelegate>(QuickPanelReceiveEventDetour);
		QuickPanelReceiveEventHook.Enable();
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)5, "QuickPanel", new AddonEventDelegate(OnAddon));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)5, "QuickPanel", new AddonEventDelegate(OnAddon));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)2, "QuickPanel", new AddonEventDelegate(OnAddon));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, "QuickPanel", new AddonEventDelegate(OnAddon));
	}

	protected override void Uninit()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		OmenServiceBase<ChatManager>.Instance().Unreg(OnPreExecuteCommandInner);
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddon });
		lockCheckBoxNode?.Dispose();
		lockCheckBoxNode = null;
	}

	protected override void ConfigUI()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("Command")));
		IndentDisposable val = ImRaii.PushIndent(1, true);
		try
		{
			ImU8String val2 = new ImU8String(17, 4);
			((ImU8String)(ref val2)).AppendFormatted<ReadOnlySeString>(((TextCommand)(ref QuickPanelLine)).Command);
			((ImU8String)(ref val2)).AppendLiteral(" <");
			((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get("OptimizedQuickPanel-CommandArgs"));
			((ImU8String)(ref val2)).AppendLiteral(" / close> → ");
			((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get("OptimizedQuickPanel-CommandArgs-Help"));
			((ImU8String)(ref val2)).AppendLiteral(" / ");
			((ImU8String)(ref val2)).AppendFormatted<string>(LuminaWrapper.GetAddonText(2366u));
			ImGui.TextUnformatted(val2);
			ImU8String val3 = new ImU8String(17, 4);
			((ImU8String)(ref val3)).AppendFormatted<ReadOnlySeString>(((TextCommand)(ref QuickPanelLine)).Alias);
			((ImU8String)(ref val3)).AppendLiteral(" <");
			((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get("OptimizedQuickPanel-CommandArgs"));
			((ImU8String)(ref val3)).AppendLiteral(" / close> → ");
			((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get("OptimizedQuickPanel-CommandArgs-Help"));
			((ImU8String)(ref val3)).AppendLiteral(" / ");
			((ImU8String)(ref val3)).AppendFormatted<string>(LuminaWrapper.GetAddonText(2366u));
			ImGui.TextUnformatted(val3);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private unsafe void OnAddon(AddonEvent type, AddonArgs args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected I4, but got Unknown
		switch (type - 2)
		{
		case 4:
			lockCheckBoxNode = null;
			break;
		case 0:
			UpdateAddonFlags();
			UpdateSlotLockState();
			break;
		case 3:
			if (Addons.QuickPanel != null && lockCheckBoxNode == null)
			{
				lockCheckBoxNode = new CheckboxNode
				{
					Position = new Vector2(238f, 34f),
					Size = new Vector2(20f, 24f),
					Scale = new Vector2(0.9f),
					IsChecked = config.IsLock
				};
				lockCheckBoxNode.OnClick = delegate(bool x)
				{
					config.IsLock = x;
					config.Save(this);
					UpdateAddonFlags();
					UpdateSlotLockState();
				};
				lockCheckBoxNode.BoxBackground.IsVisible = false;
				lockCheckBoxNode.BoxForeground.IsVisible = false;
				lockCheckBoxNode.Label.IsVisible = false;
				SimpleImageNode simpleImageNode = new SimpleImageNode
				{
					Size = new Vector2(20f, 24f),
					TexturePath = "ui/uld/ActionBar_hr1.tex"
				};
				simpleImageNode.AddPart(new Part
				{
					Size = new Vector2(20f, 24f),
					TexturePath = "ui/uld/ActionBar_hr1.tex",
					TextureCoordinates = new Vector2(48f, 0f),
					Id = 1u
				}, new Part
				{
					Size = new Vector2(20f, 24f),
					TexturePath = "ui/uld/ActionBar_hr1.tex",
					TextureCoordinates = new Vector2(68f, 0f),
					Id = 2u
				}, new Part
				{
					Size = new Vector2(20f, 24f),
					TexturePath = "ui/uld/ActionBar_hr1.tex",
					TextureCoordinates = new Vector2(88f, 0f),
					Id = 3u
				});
				simpleImageNode.AttachNode((NodeBase?)lockCheckBoxNode, NodePosition.AsLastChild);
				FrameSetBuilder frameSetBuilder = new TimelineBuilder().BeginFrameSet(1, 10);
				Vector3? addColor = new Vector3(0f, 0f, 0f);
				Vector3? multiplyColor = new Vector3(100f, 100f, 100f);
				FrameSetBuilder frameSetBuilder2 = frameSetBuilder.AddFrame(1, null, null, addColor, multiplyColor);
				uint? partId = 1u;
				FrameSetBuilder frameSetBuilder3 = frameSetBuilder2.AddFrame(1, null, null, null, null, null, null, null, null, partId).EndFrameSet().BeginFrameSet(11, 20);
				Vector3? addColor2 = new Vector3(0f, 0f, 0f);
				Vector3? multiplyColor2 = new Vector3(100f, 100f, 100f);
				FrameSetBuilder frameSetBuilder4 = frameSetBuilder3.AddFrame(11, null, null, addColor2, multiplyColor2);
				Vector3? addColor3 = new Vector3(40f, 40f, 40f);
				Vector3? multiplyColor3 = new Vector3(100f, 100f, 100f);
				FrameSetBuilder frameSetBuilder5 = frameSetBuilder4.AddFrame(13, null, null, addColor3, multiplyColor3);
				uint? partId2 = 1u;
				FrameSetBuilder frameSetBuilder6 = frameSetBuilder5.AddFrame(11, null, null, null, null, null, null, null, null, partId2);
				uint? partId3 = 1u;
				FrameSetBuilder frameSetBuilder7 = frameSetBuilder6.AddFrame(13, null, null, null, null, null, null, null, null, partId3).EndFrameSet().BeginFrameSet(21, 30);
				Vector3? addColor4 = new Vector3(60f, 60f, 60f);
				Vector3? multiplyColor4 = new Vector3(100f, 100f, 100f);
				FrameSetBuilder frameSetBuilder8 = frameSetBuilder7.AddFrame(21, null, null, addColor4, multiplyColor4);
				uint? partId4 = 2u;
				FrameSetBuilder frameSetBuilder9 = frameSetBuilder8.AddFrame(21, null, null, null, null, null, null, null, null, partId4).EndFrameSet().BeginFrameSet(31, 40);
				Vector3? addColor5 = new Vector3(0f, 0f, 0f);
				Vector3? multiplyColor5 = new Vector3(50f, 50f, 50f);
				FrameSetBuilder frameSetBuilder10 = frameSetBuilder9.AddFrame(31, null, null, addColor5, multiplyColor5);
				uint? partId5 = 1u;
				FrameSetBuilder frameSetBuilder11 = frameSetBuilder10.AddFrame(31, null, null, null, null, null, null, null, null, partId5).EndFrameSet().BeginFrameSet(41, 50);
				Vector3? addColor6 = new Vector3(60f, 60f, 60f);
				Vector3? multiplyColor6 = new Vector3(100f, 100f, 100f);
				FrameSetBuilder frameSetBuilder12 = frameSetBuilder11.AddFrame(41, null, null, addColor6, multiplyColor6);
				Vector3? addColor7 = new Vector3(0f, 0f, 0f);
				Vector3? multiplyColor7 = new Vector3(100f, 100f, 100f);
				FrameSetBuilder frameSetBuilder13 = frameSetBuilder12.AddFrame(43, null, null, addColor7, multiplyColor7);
				uint? partId6 = 1u;
				FrameSetBuilder frameSetBuilder14 = frameSetBuilder13.AddFrame(41, null, null, null, null, null, null, null, null, partId6);
				uint? partId7 = 1u;
				FrameSetBuilder frameSetBuilder15 = frameSetBuilder14.AddFrame(43, null, null, null, null, null, null, null, null, partId7).EndFrameSet().BeginFrameSet(51, 60);
				Vector3? addColor8 = new Vector3(40f, 40f, 40f);
				Vector3? multiplyColor8 = new Vector3(100f, 100f, 100f);
				FrameSetBuilder frameSetBuilder16 = frameSetBuilder15.AddFrame(51, null, null, addColor8, multiplyColor8);
				Vector3? addColor9 = new Vector3(0f, 0f, 0f);
				Vector3? multiplyColor9 = new Vector3(100f, 100f, 100f);
				FrameSetBuilder frameSetBuilder17 = frameSetBuilder16.AddFrame(53, null, null, addColor9, multiplyColor9);
				uint? partId8 = 1u;
				FrameSetBuilder frameSetBuilder18 = frameSetBuilder17.AddFrame(51, null, null, null, null, null, null, null, null, partId8);
				uint? partId9 = 1u;
				FrameSetBuilder frameSetBuilder19 = frameSetBuilder18.AddFrame(53, null, null, null, null, null, null, null, null, partId9).EndFrameSet().BeginFrameSet(61, 70);
				Vector3? addColor10 = new Vector3(0f, 0f, 0f);
				Vector3? multiplyColor10 = new Vector3(100f, 100f, 100f);
				FrameSetBuilder frameSetBuilder20 = frameSetBuilder19.AddFrame(61, null, null, addColor10, multiplyColor10);
				uint? partId10 = 3u;
				FrameSetBuilder frameSetBuilder21 = frameSetBuilder20.AddFrame(61, null, null, null, null, null, null, null, null, partId10).EndFrameSet().BeginFrameSet(71, 80);
				Vector3? addColor11 = new Vector3(0f, 0f, 0f);
				Vector3? multiplyColor11 = new Vector3(100f, 100f, 100f);
				FrameSetBuilder frameSetBuilder22 = frameSetBuilder21.AddFrame(71, null, null, addColor11, multiplyColor11);
				Vector3? addColor12 = new Vector3(40f, 40f, 40f);
				Vector3? multiplyColor12 = new Vector3(100f, 100f, 100f);
				FrameSetBuilder frameSetBuilder23 = frameSetBuilder22.AddFrame(73, null, null, addColor12, multiplyColor12);
				uint? partId11 = 3u;
				FrameSetBuilder frameSetBuilder24 = frameSetBuilder23.AddFrame(71, null, null, null, null, null, null, null, null, partId11);
				uint? partId12 = 3u;
				FrameSetBuilder frameSetBuilder25 = frameSetBuilder24.AddFrame(73, null, null, null, null, null, null, null, null, partId12).EndFrameSet().BeginFrameSet(81, 90);
				Vector3? addColor13 = new Vector3(60f, 60f, 60f);
				Vector3? multiplyColor13 = new Vector3(100f, 100f, 100f);
				FrameSetBuilder frameSetBuilder26 = frameSetBuilder25.AddFrame(81, null, null, addColor13, multiplyColor13);
				uint? partId13 = 2u;
				FrameSetBuilder frameSetBuilder27 = frameSetBuilder26.AddFrame(81, null, null, null, null, null, null, null, null, partId13).EndFrameSet().BeginFrameSet(91, 100);
				Vector3? addColor14 = new Vector3(0f, 0f, 0f);
				Vector3? multiplyColor14 = new Vector3(50f, 50f, 50f);
				FrameSetBuilder frameSetBuilder28 = frameSetBuilder27.AddFrame(91, null, null, addColor14, multiplyColor14);
				uint? partId14 = 3u;
				FrameSetBuilder frameSetBuilder29 = frameSetBuilder28.AddFrame(91, null, null, null, null, null, null, null, null, partId14).EndFrameSet().BeginFrameSet(101, 110);
				Vector3? addColor15 = new Vector3(60f, 60f, 60f);
				Vector3? multiplyColor15 = new Vector3(100f, 100f, 100f);
				FrameSetBuilder frameSetBuilder30 = frameSetBuilder29.AddFrame(101, null, null, addColor15, multiplyColor15);
				Vector3? addColor16 = new Vector3(0f, 0f, 0f);
				Vector3? multiplyColor16 = new Vector3(100f, 100f, 100f);
				FrameSetBuilder frameSetBuilder31 = frameSetBuilder30.AddFrame(103, null, null, addColor16, multiplyColor16);
				uint? partId15 = 3u;
				FrameSetBuilder frameSetBuilder32 = frameSetBuilder31.AddFrame(101, null, null, null, null, null, null, null, null, partId15);
				uint? partId16 = 3u;
				FrameSetBuilder frameSetBuilder33 = frameSetBuilder32.AddFrame(103, null, null, null, null, null, null, null, null, partId16).EndFrameSet().BeginFrameSet(111, 120);
				Vector3? addColor17 = new Vector3(40f, 40f, 40f);
				Vector3? multiplyColor17 = new Vector3(100f, 100f, 100f);
				FrameSetBuilder frameSetBuilder34 = frameSetBuilder33.AddFrame(111, null, null, addColor17, multiplyColor17);
				Vector3? addColor18 = new Vector3(0f, 0f, 0f);
				Vector3? multiplyColor18 = new Vector3(100f, 100f, 100f);
				FrameSetBuilder frameSetBuilder35 = frameSetBuilder34.AddFrame(113, null, null, addColor18, multiplyColor18);
				uint? partId17 = 3u;
				FrameSetBuilder frameSetBuilder36 = frameSetBuilder35.AddFrame(111, null, null, null, null, null, null, null, null, partId17);
				uint? partId18 = 3u;
				simpleImageNode.AddTimeline(frameSetBuilder36.AddFrame(113, null, null, null, null, null, null, null, null, partId18).EndFrameSet().Build());
				lockCheckBoxNode.AttachNode(Addons.QuickPanel);
			}
			break;
		case 1:
		case 2:
			break;
		}
	}

	private unsafe static void OnPreExecuteCommandInner(ref bool isPrevented, ref ReadOnlySeString message)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		string text = ((object)Unsafe.As<ReadOnlySeString, ReadOnlySeString>(ref message)/*cast due to constrained. prefix*/).ToString();
		if (!text.StartsWith('/'))
		{
			return;
		}
		string[] array = text.Split(' ');
		if (array != null && array.Length == 2 && (!(array[0] != ((object)((TextCommand)(ref QuickPanelLine)).Command/*cast due to constrained. prefix*/).ToString()) || !(array[0] != ((object)((TextCommand)(ref QuickPanelLine)).Alias/*cast due to constrained. prefix*/).ToString())))
		{
			int result;
			if (array[1].Equals("close"))
			{
				((AgentQuickPanel)AgentQuickPanel.Instance()).Hide();
				isPrevented = true;
			}
			else if (int.TryParse(array[1], out result) && result > 0 && result < 5)
			{
				((AgentQuickPanel)AgentQuickPanel.Instance()).OpenPanel((uint)(result - 1), true, false);
				isPrevented = true;
			}
		}
	}

	private unsafe void StartDraggingAddonDetour(AtkUnitManager* manager, AtkUnitBase* addon)
	{
		if (addon != Addons.QuickPanel || !config.IsLock)
		{
			StartDraggingAddonHook.Original(manager, addon);
		}
	}

	private unsafe void AddonControlReceiveEventDetour(AtkAddonControl* control, ushort eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData)
	{
		if (eventType != 5 || ((AtkAddonControl)control).ParentAddon != Addons.QuickPanel || !config.IsLock)
		{
			AddonControlReceiveEventHook.Original(control, eventType, eventParam, atkEvent, atkEventData);
		}
	}

	private unsafe bool IsMoveHandleNodeDetour(AtkUnitBase* addon, AtkResNode* node)
	{
		if (addon == Addons.QuickPanel && config.IsLock)
		{
			return false;
		}
		return IsMoveHandleNodeHook.Original(addon, node);
	}

	private unsafe void QuickPanelReceiveEventDetour(AtkUnitBase* addon, ushort eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData)
	{
		if (eventType != 58 || ((AtkDragDropData)(&((AtkEventData)atkEventData).DragDropData)).MouseButtonId == 0 || !config.IsLock)
		{
			QuickPanelReceiveEventHook.Original(addon, eventType, eventParam, atkEvent, atkEventData);
		}
	}

	private unsafe void UpdateAddonFlags()
	{
		if (Addons.QuickPanel != null)
		{
			((AtkUnitBase)Addons.QuickPanel).DisableFocusability = config.IsLock;
			if (config.IsLock)
			{
				((AtkUnitBase)Addons.QuickPanel).Flags1B4 |= 16u;
			}
			else
			{
				((AtkUnitBase)Addons.QuickPanel).Flags1B4 &= 4294967279u;
			}
		}
	}

	private unsafe void UpdateSlotLockState()
	{
		if (Addons.QuickPanel == null)
		{
			return;
		}
		byte* ptr = (byte*)Addons.QuickPanel + 576;
		int num = 0;
		while (num < 25)
		{
			AtkComponentDragDrop* ptr2 = *(AtkComponentDragDrop**)ptr;
			if (ptr2 != null)
			{
				if (config.IsLock)
				{
					((AtkComponentDragDrop)ptr2).Flags |= (DragDropFlag)2;
				}
				else
				{
					DragDropFlag* flags = &((AtkComponentDragDrop)ptr2).Flags;
					*flags = (DragDropFlag)((uint)(*flags) & 0xFDu);
				}
			}
			num++;
			ptr += 48;
		}
	}
}
