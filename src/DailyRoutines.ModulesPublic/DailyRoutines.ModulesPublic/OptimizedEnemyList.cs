using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using DailyRoutines.Common.Extensions;
using DailyRoutines.Common.Info;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using Dalamud.Utility.Numerics;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Arrays;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using KamiToolKit.Nodes.Simplified;
using KamiToolKit.UiOverlay;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.Threading;

namespace DailyRoutines.ModulesPublic;

public class OptimizedEnemyList : ModuleBase
{
	private class Config : ModuleConfig
	{
		public bool DisplayStatus = true;

		public byte TextSize = 10;

		public Vector4 TextColor = Vector4.One;

		public Vector4 TextEdgeColor = new Vector4(0.6156863f, 0.5137255f, 0.3568628f, 1f);

		public Vector2 TextOffset = Vector2.Zero;

		public byte HealthTextSize = 16;

		public Vector4 HealthTextColor = Vector4.One;

		public Vector4 HealthTextEdgeColor = new Vector4(0.6156863f, 0.5137255f, 0.3568628f, 1f);

		public Vector2 HealthTextOffset = Vector2.Zero;

		public byte EnemityTextSize = 8;

		public Vector4 EnemityTextColor = AtkColors.Value.GetTextColor();

		public Vector4 EnemityTextEdgeColor = AtkColors.Value.GetEdgeColor();

		public Vector2 EnemityTextOffset = Vector2.Zero;

		public float StatusScale = 1f;

		public Vector2 StatusOffset = Vector2.Zero;

		public bool AlwaysUnlock = true;
	}

	private sealed class EnemyListNode(uint componentNodeID, TextNode castNode, TextNode heathNode, TextNode healthMarkerNode, NineGridNode castBackgroundNode, NineGridNode healthBackgroundNode, ProgressBarEnemyCastNode castBarNode, IconTextNodesRow statusNodes, TextNode enemityNode)
	{
		public uint ComponentNodeID { get; set; } = componentNodeID;

		public TextNode? CastNode { get; set; } = castNode;

		public TextNode? HeathNode { get; set; } = heathNode;

		public TextNode? HealthMarkerNode { get; set; } = healthMarkerNode;

		public NineGridNode? CastBackgroundNode { get; set; } = castBackgroundNode;

		public NineGridNode? HealthBackgroundNode { get; set; } = healthBackgroundNode;

		public ProgressBarEnemyCastNode? CastBarNode { get; set; } = castBarNode;

		public IconTextNodesRow? StatusNodes { get; set; } = statusNodes;

		public TextNode? EnemityNode { get; set; } = enemityNode;

		public void Deconstruct(out uint componentNodeID, out TextNode? castNode, out TextNode? heathNode, out TextNode? healthMarkerNode, out NineGridNode? castBackgroundNode, out NineGridNode? healthBackgroundNode, out ProgressBarEnemyCastNode? castBarNode, out IconTextNodesRow? statusNodes, out TextNode? enemityNode)
		{
			componentNodeID = ComponentNodeID;
			castNode = CastNode;
			heathNode = HeathNode;
			healthMarkerNode = HealthMarkerNode;
			castBackgroundNode = CastBackgroundNode;
			healthBackgroundNode = HealthBackgroundNode;
			castBarNode = CastBarNode;
			statusNodes = StatusNodes;
			enemityNode = EnemityNode;
		}
	}

	private class IconTextNodesRow : OverlayNode, IEnumerable<IconTextNode>, IEnumerable
	{
		public int Count { get; init; }

		public uint NodeID { get; init; }

		public int Index { get; init; }

		public override OverlayLayer OverlayLayer => OverlayLayer.Foreground;

		public override bool HideWithNativeUi => true;

		public bool ShouldBeVisible { get; set; }

		public List<IconTextNode> Nodes { get; init; } = new List<IconTextNode>();

		public IconTextNode this[int index] => Nodes[index];

		public IconTextNodesRow(int count, uint nodeID, int index)
		{
			ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(count, 0, "count");
			ArgumentOutOfRangeException.ThrowIfLessThan(index, 0, "index");
			Count = count;
			NodeID = nodeID;
			Index = index;
			for (int i = 0; i < count; i++)
			{
				IconTextNode iconTextNode = new IconTextNode
				{
					Size = new Vector2(25f, 41f),
					Position = new Vector2(27 * i, 0f)
				};
				iconTextNode.AttachNode(this);
				Nodes.Add(iconTextNode);
			}
			Size = new Vector2(25 + 27 * count, 41f);
		}

		public IEnumerator<IconTextNode> GetEnumerator()
		{
			return Nodes.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		protected unsafe override void OnUpdate()
		{
			IsVisible = ShouldBeVisible && !GameState.IsInPVPInstance && (*Addons.EnemyList).IsAddonAndNodesReady() && Index < ((EnemyListNumberArray)EnemyListNumberArray.Instance()).EnemyCount;
		}
	}

	private class IconTextNode : SimpleComponentNode
	{
		public readonly IconImageNode IconNode;

		public readonly TextNode TextNode;

		public IconTextNode()
		{
			IconNode = new IconImageNode
			{
				NodeId = 3u,
				Size = new Vector2(24f, 32f),
				ImageNodeFlags = (ImageNodeFlags)128
			};
			IconNode.TextureSize = new Vector2(24f, 32f);
			IconNode.AttachNode((NodeBase?)this, NodePosition.AsLastChild);
			TextNode = new TextNode
			{
				NodeId = 2u,
				Size = new Vector2(24f, 18f),
				Position = new Vector2(0f, 23f),
				TextFlags = (TextFlags)8,
				AlignmentType = (AlignmentType)4,
				FontType = (FontType)0,
				FontSize = 12u,
				TextColor = new Vector4(0.788f, 1f, 0.894f, 1f),
				TextOutlineColor = new Vector4(0.039f, 0.373f, 0.141f, 1f)
			};
			TextNode.AttachNode((NodeBase?)this, NodePosition.AsLastChild);
		}

		public void Update(Status status)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			if (LuminaGetter.TryGetRow<Status>((uint)status.StatusId, out Status item))
			{
				IconNode.IconId = ((Status)(ref item)).Icon;
				TextNode.SetNumber((int)status.RemainingTime);
				TextTooltip = ReadOnlySeString.op_Implicit($"{((Status)(ref item)).Name}\n{((Status)(ref item)).Description}");
			}
		}
	}

	private MemoryPatch enemyListIsCastInProgressPatch;

	private MemoryPatch enemyListClearSpellIDPatch;

	private MemoryPatch enemyListDisplayCastPatch;

	private Config config;

	private readonly List<EnemyListNode> nodes = new List<EnemyListNode>();

	private OverlayController? controller;

	private bool isUpdating;

	private static readonly Vector2 CastTextDefaultPosition = new Vector2(203f, 8f);

	private static readonly Vector2 CastBackgroundTextDefaultPosition = new Vector2(197f, 4f);

	private static readonly Vector2 HealthTextDefaultPosition = new Vector2(-60f, 8f);

	private static readonly Vector2 EnemityTextDefaultPosition = new Vector2(12f, 21f);

	private static readonly Vector2 StatusComponentOffset = new Vector2(12f, -1f);

	private const float HEALTH_TEXT_MARKER_PADDING = 1f;

	private const float CAST_TEXT_BACKGROUND_PADDING = 5f;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("OptimizedEnemyListTitle"),
		Description = LanguageManager.Get("OptimizedEnemyListDescription"),
		Category = ModuleCategory.Combat,
		PreviewImageURL = new string[1] { "hxxps://gh.atmoomen.top/raw.githubusercontent.com/Dalamud-DailyRoutines/DailyRoutines/main/Resources/Modules/OptimizedEnemyList/preview-1.png" },
		ModulesPair = new string[1] { "AutoDisplayHiddenCast" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected override void Init()
	{
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Expected O, but got Unknown
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Expected O, but got Unknown
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Expected O, but got Unknown
		enemyListIsCastInProgressPatch = new MemoryPatch("0F 84 ?? ?? ?? ?? 49 8B 04 24 49 8B CC FF 90 ?? ?? ?? ?? 48 85 C0", new global::_003C_003Ez__ReadOnlyArray<byte?>(new byte?[6]
		{
			(byte)144,
			(byte)144,
			(byte)144,
			(byte)144,
			(byte)144,
			(byte)144
		}));
		enemyListClearSpellIDPatch = new MemoryPatch("83 7E ?? ?? 0F 84 ?? ?? ?? ?? 8B B4 24", new global::_003C_003Ez__ReadOnlyArray<byte?>(new byte?[10]
		{
			(byte)144,
			(byte)144,
			(byte)144,
			(byte)144,
			(byte)144,
			(byte)144,
			(byte)144,
			(byte)144,
			(byte)144,
			(byte)144
		}));
		enemyListDisplayCastPatch = new MemoryPatch("74 ?? 49 8B 04 24 49 8B CC FF 90 ?? ?? ?? ?? 48 85 C0", new global::_003C_003Ez__ReadOnlyArray<byte?>(new byte?[2]
		{
			(byte)144,
			(byte)144
		}));
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		enemyListIsCastInProgressPatch.Enable();
		enemyListClearSpellIDPatch.Enable();
		enemyListDisplayCastPatch.Enable();
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)7, "_EnemyList", new AddonEventDelegate(OnAddon));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)5, "_EnemyList", new AddonEventDelegate(OnAddon));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, "_EnemyList", new AddonEventDelegate(OnAddon));
	}

	protected override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddon });
		foreach (var (_, textNode5, textNode6, textNode7, nineGridNode3, nineGridNode4, progressBarEnemyCastNode2, _, textNode8) in nodes)
		{
			textNode5?.Dispose();
			textNode6?.Dispose();
			textNode7?.Dispose();
			nineGridNode3?.Dispose();
			nineGridNode4?.Dispose();
			progressBarEnemyCastNode2?.Dispose();
			textNode8?.Dispose();
		}
		nodes.Clear();
		controller?.Dispose();
		controller = null;
	}

	protected override void ConfigUI()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_042e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0457: Unknown result type (might be due to invalid IL or missing references)
		//IL_0476: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		using (ImRaiiExtension.Heading1(LanguageManager.Get("Cast")))
		{
			ItemWidthDisposable val = ImRaii.ItemWidth(200f * Globals.GlobalUIScale);
			try
			{
				IdDisposable val2 = ImRaii.PushId(ImU8String.op_Implicit("CastText"), true);
				try
				{
					ImGui.InputFloat2(ImU8String.op_Implicit(LanguageManager.Get("Offset")), ref config.TextOffset, 0f, 0f, ImU8String.op_Implicit("%.1f"), (ImGuiInputTextFlags)0);
					if (ImGui.IsItemDeactivatedAfterEdit())
					{
						config.Save(this);
					}
					ImGui.InputByte(ImU8String.op_Implicit(LanguageManager.Get("FontSize")), ref config.TextSize, (byte)0, (byte)0, default(ImU8String), (ImGuiInputTextFlags)0);
					if (ImGui.IsItemDeactivatedAfterEdit())
					{
						config.Save(this);
					}
					ImGui.ColorEdit4(ImU8String.op_Implicit(LanguageManager.Get("TextColor")), ref config.TextColor, (ImGuiColorEditFlags)0);
					if (ImGui.IsItemDeactivatedAfterEdit())
					{
						config.Save(this);
					}
					ImGui.ColorEdit4(ImU8String.op_Implicit(LanguageManager.Get("EdgeColor")), ref config.TextEdgeColor, (ImGuiColorEditFlags)0);
					if (ImGui.IsItemDeactivatedAfterEdit())
					{
						config.Save(this);
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		ImGui.NewLine();
		using (ImRaiiExtension.Heading1(LuminaWrapper.GetAddonText(11274u)))
		{
			ItemWidthDisposable val = ImRaii.ItemWidth(200f * Globals.GlobalUIScale);
			try
			{
				IdDisposable val2 = ImRaii.PushId(ImU8String.op_Implicit("HealthText"), true);
				try
				{
					ImGui.InputFloat2(ImU8String.op_Implicit(LanguageManager.Get("Offset")), ref config.HealthTextOffset, 0f, 0f, ImU8String.op_Implicit("%.1f"), (ImGuiInputTextFlags)0);
					if (ImGui.IsItemDeactivatedAfterEdit())
					{
						config.Save(this);
					}
					ImGui.InputByte(ImU8String.op_Implicit(LanguageManager.Get("FontSize")), ref config.HealthTextSize, (byte)0, (byte)0, default(ImU8String), (ImGuiInputTextFlags)0);
					if (ImGui.IsItemDeactivatedAfterEdit())
					{
						config.Save(this);
					}
					ImGui.ColorEdit4(ImU8String.op_Implicit(LanguageManager.Get("TextColor")), ref config.HealthTextColor, (ImGuiColorEditFlags)0);
					if (ImGui.IsItemDeactivatedAfterEdit())
					{
						config.Save(this);
					}
					ImGui.ColorEdit4(ImU8String.op_Implicit(LanguageManager.Get("EdgeColor")), ref config.HealthTextEdgeColor, (ImGuiColorEditFlags)0);
					if (ImGui.IsItemDeactivatedAfterEdit())
					{
						config.Save(this);
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		ImGui.NewLine();
		using (ImRaiiExtension.Heading1(LuminaWrapper.GetAddonText(721u)))
		{
			ItemWidthDisposable val = ImRaii.ItemWidth(200f * Globals.GlobalUIScale);
			try
			{
				IdDisposable val2 = ImRaii.PushId(ImU8String.op_Implicit("EnemityText"), true);
				try
				{
					ImGui.InputFloat2(ImU8String.op_Implicit(LanguageManager.Get("Offset")), ref config.EnemityTextOffset, 0f, 0f, ImU8String.op_Implicit("%.1f"), (ImGuiInputTextFlags)0);
					if (ImGui.IsItemDeactivatedAfterEdit())
					{
						config.Save(this);
					}
					ImGui.InputByte(ImU8String.op_Implicit(LanguageManager.Get("FontSize")), ref config.EnemityTextSize, (byte)0, (byte)0, default(ImU8String), (ImGuiInputTextFlags)0);
					if (ImGui.IsItemDeactivatedAfterEdit())
					{
						config.Save(this);
					}
					ImGui.ColorEdit4(ImU8String.op_Implicit(LanguageManager.Get("TextColor")), ref config.EnemityTextColor, (ImGuiColorEditFlags)0);
					if (ImGui.IsItemDeactivatedAfterEdit())
					{
						config.Save(this);
					}
					ImGui.ColorEdit4(ImU8String.op_Implicit(LanguageManager.Get("EdgeColor")), ref config.EnemityTextEdgeColor, (ImGuiColorEditFlags)0);
					if (ImGui.IsItemDeactivatedAfterEdit())
					{
						config.Save(this);
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		ImGui.NewLine();
		using (ImRaiiExtension.Heading1(LanguageManager.Get("Status")))
		{
			ItemWidthDisposable val = ImRaii.ItemWidth(200f * Globals.GlobalUIScale);
			try
			{
				IdDisposable val2 = ImRaii.PushId(ImU8String.op_Implicit("Status"), true);
				try
				{
					ImGui.InputFloat2(ImU8String.op_Implicit(LanguageManager.Get("Offset")), ref config.StatusOffset, 0f, 0f, ImU8String.op_Implicit("%.1f"), (ImGuiInputTextFlags)0);
					if (ImGui.IsItemDeactivatedAfterEdit())
					{
						config.Save(this);
					}
					ImGui.InputFloat(ImU8String.op_Implicit(LanguageManager.Get("Scale")), ref config.StatusScale, 0f, 0f, ImU8String.op_Implicit("%.1f"), (ImGuiInputTextFlags)0);
					if (ImGui.IsItemDeactivatedAfterEdit())
					{
						config.Save(this);
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		ImGui.NewLine();
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("OptimizedEnemyList-AlwaysUnlock")), ref config.AlwaysUnlock))
		{
			config.Save(this);
		}
		ImGuiOm.HelpMarker(LanguageManager.Get("OptimizedEnemyList-AlwaysUnlock-Help"), 40f, (FontAwesomeIcon)61530);
	}

	private unsafe void OnAddon(AddonEvent type, AddonArgs args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Expected I4, but got Unknown
		switch (type - 5)
		{
		case 2:
			if (config.AlwaysUnlock)
			{
				EnemyListNumberArray* ptr = EnemyListNumberArray.Instance();
				for (int i = 0; i < ((EnemyListNumberArray)ptr).Enemies.Length; i++)
				{
					((EnemyListNumberArray)ptr).Enemies[i].LockedInList = false;
				}
			}
			if (!isUpdating)
			{
				try
				{
					UpdateNodes();
					break;
				}
				finally
				{
					isUpdating = false;
				}
			}
			break;
		case 0:
			if (!Throttler.Shared.Throttle("OptimizedEnemyList.UpdateEnemyList", 100u) || isUpdating)
			{
				break;
			}
			try
			{
				UpdateNodes();
				break;
			}
			finally
			{
				isUpdating = false;
			}
		case 1:
			nodes.Clear();
			controller?.Dispose();
			controller = null;
			break;
		}
	}

	private unsafe void UpdateNodes()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0481: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0781: Unknown result type (might be due to invalid IL or missing references)
		//IL_0883: Unknown result type (might be due to invalid IL or missing references)
		//IL_0888: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_090c: Unknown result type (might be due to invalid IL or missing references)
		EnemyListNumberArray* ptr = EnemyListNumberArray.Instance();
		if (ptr == null || ((EnemyListNumberArray)ptr).EnemyCount == 0)
		{
			return;
		}
		List<EnemyListNode> list = nodes;
		if (list == null || list.Count <= 0)
		{
			CreateNodes();
			return;
		}
		float num4 = default(float);
		for (int i = 0; (float)i < MathF.Min(((EnemyListNumberArray)ptr).EnemyCount, nodes.Count); i++)
		{
			EnemyListEnemyNumberArray val = ((EnemyListNumberArray)ptr).Enemies[i];
			nodes[i].Deconstruct(out uint componentNodeID, out TextNode castNode, out TextNode healthNode, out TextNode healthMarkerNode, out NineGridNode castBackgroundNode, out NineGridNode healthBackgroundNode, out ProgressBarEnemyCastNode castBarNode, out IconTextNodesRow statusNodes, out TextNode enemityNode);
			uint entityId = (uint)val.EntityId;
			if ((entityId == 0 || entityId == 3758096384u) ? true : false)
			{
				HideNodes();
				continue;
			}
			BattleChara* ptr2 = ((CharacterManager)CharacterManager.Instance()).LookupBattleCharaByEntityId(entityId);
			if (ptr2 == null)
			{
				HideNodes();
				continue;
			}
			AtkComponentNode* componentNodeById = ((AtkUnitBase)Addons.EnemyList).GetComponentNodeById(componentNodeID);
			if (componentNodeById == null)
			{
				HideNodes();
				continue;
			}
			AtkTextNode* asAtkTextNode = ((AtkResNode)((AtkUldManager)(&((AtkComponentBase)((AtkComponentNode)componentNodeById).Component).UldManager)).SearchNodeById(4u)).GetAsAtkTextNode();
			if (asAtkTextNode == null)
			{
				HideNodes();
				continue;
			}
			if (((AtkResNode)((AtkUldManager)(&((AtkComponentBase)((AtkComponentNode)componentNodeById).Component).UldManager)).SearchNodeById(6u)).GetAsAtkTextNode() == null)
			{
				HideNodes();
				continue;
			}
			AtkResNode* ptr3 = ((AtkUldManager)(&((AtkComponentBase)((AtkComponentNode)componentNodeById).Component).UldManager)).SearchNodeById(7u);
			AtkResNode* ptr4 = ((AtkUldManager)(&((AtkComponentBase)((AtkComponentNode)componentNodeById).Component).UldManager)).SearchNodeById(8u);
			if (ptr3 == null || ptr4 == null)
			{
				HideNodes();
				continue;
			}
			AtkResNode* ptr5 = ((AtkUldManager)(&((AtkComponentBase)((AtkComponentNode)componentNodeById).Component).UldManager)).SearchNodeById(5u);
			if (ptr5 == null)
			{
				HideNodes();
				continue;
			}
			AtkResNode* ptr6 = ((AtkUldManager)(&((AtkComponentBase)((AtkComponentNode)componentNodeById).Component).UldManager)).SearchNodeById(14u);
			if (ptr6 == null)
			{
				continue;
			}
			((AtkResNode)ptr3).SetAlpha((byte)0);
			((AtkResNode)ptr4).SetAlpha((byte)0);
			((AtkTextNode)asAtkTextNode).SetAlpha((byte)0);
			((AtkResNode)ptr5).SetAlpha((byte)0);
			statusNodes.Scale = ((*componentNodeById).GetScale() - new Vector2(0.1f)) * config.StatusScale;
			statusNodes.Alpha = (val.ActiveInList ? 1f : 0.5f);
			NodeState nodeState = (*componentNodeById).GetNodeState();
			statusNodes.Position = nodeState.TopRight - VectorExtensions.WithY((*ptr6).GetNodeState().Size, 0f) + StatusComponentOffset * statusNodes.Scale + config.StatusOffset;
			int num = 0;
			Span<Status> status = ((StatusManager)(&((BattleChara)ptr2).StatusManager)).Status;
			for (int j = 0; j < status.Length; j++)
			{
				Status val2 = status[j];
				if (num == 5)
				{
					break;
				}
				if (val2.StatusId != 0 && (uint)GameObjectId.op_Implicit(val2.SourceObject) == LocalPlayerState.EntityID)
				{
					IconTextNode iconTextNode = statusNodes[num];
					iconTextNode.IsVisible = true;
					iconTextNode.Update(val2);
					num++;
				}
			}
			if (num < 5)
			{
				for (int k = num; k < 5; k++)
				{
					statusNodes[k].IsVisible = false;
				}
			}
			statusNodes.ShouldBeVisible = num > 0;
			float value = (float)((BattleChara)ptr2).Health / (float)((BattleChara)ptr2).MaxHealth * 100f;
			if (!((BattleChara)ptr2).GetIsTargetable() && (((BattleChara)ptr2).Health == ((BattleChara)ptr2).MaxHealth || ((BattleChara)ptr2).Health == 0))
			{
				healthNode.IsVisible = false;
				healthMarkerNode.IsVisible = false;
				healthBackgroundNode.IsVisible = false;
			}
			else
			{
				healthNode.IsVisible = true;
				healthMarkerNode.IsVisible = true;
				healthBackgroundNode.IsVisible = true;
				healthNode.TextColor = config.HealthTextColor;
				healthNode.TextOutlineColor = config.HealthTextEdgeColor;
				healthNode.FontSize = config.HealthTextSize;
				healthMarkerNode.TextColor = config.HealthTextColor;
				healthMarkerNode.TextOutlineColor = config.HealthTextEdgeColor;
				healthMarkerNode.FontSize = (byte)Math.Max(0, config.HealthTextSize - 2);
				healthNode.String = ReadOnlySeString.op_Implicit($"{value:F1}");
				float x = healthNode.GetTextDrawSize(considerScale: false).X;
				float y = healthNode.GetTextDrawSize(considerScale: false).Y;
				float x2 = healthNode.GetTextDrawSize(ReadOnlySeString.op_Implicit("99.9"), considerScale: false).X;
				float x3 = healthMarkerNode.GetTextDrawSize(considerScale: false).X;
				float num2 = HealthTextDefaultPosition.X + config.HealthTextOffset.X + x2 - 3f + x3;
				healthNode.X = num2 - x - 1f - x3;
				healthNode.Y = HealthTextDefaultPosition.Y + config.HealthTextOffset.Y;
				healthMarkerNode.X = healthNode.X + x + 1f;
				healthMarkerNode.Y = HealthTextDefaultPosition.Y + config.HealthTextOffset.Y;
				healthBackgroundNode.X = healthNode.X - 5f;
				healthBackgroundNode.Y = 6f + config.HealthTextOffset.Y;
				healthBackgroundNode.Width = x + 1f + x3 + 11f;
				healthBackgroundNode.Height = y + 4f;
			}
			castNode.TextColor = config.TextColor;
			castNode.TextOutlineColor = config.TextEdgeColor;
			castNode.FontSize = config.TextSize;
			castNode.Position = CastTextDefaultPosition + config.TextOffset;
			float num3 = MathF.Max(((CastInfo)(&((BattleChara)ptr2).CastInfo)).TotalCastTime - ((CastInfo)(&((BattleChara)ptr2).CastInfo)).CurrentCastTime, 0f);
			if (num3 <= 0f && ((CastInfo)(&((BattleChara)ptr2).CastInfo)).ActionId == 0)
			{
				castNode.IsVisible = false;
				castBackgroundNode.IsVisible = false;
				castBarNode.IsVisible = false;
			}
			else
			{
				castNode.IsVisible = true;
				castBackgroundNode.IsVisible = true;
				castBarNode.IsVisible = true;
				castBarNode.Progress = ((CastInfo)(&((BattleChara)ptr2).CastInfo)).CurrentCastTime / ((CastInfo)(&((BattleChara)ptr2).CastInfo)).TotalCastTime;
				castBarNode.IsInterruptible = ((CastInfo)(&((BattleChara)ptr2).CastInfo)).Interruptible;
				Vector3 addColor = (((CastInfo)(&((BattleChara)ptr2).CastInfo)).Interruptible ? KnownColor.Red.ToVector4().ToVector3() : Vector3.Zero);
				castBarNode.AddColor = addColor;
				castNode.AddColor = addColor;
				string castInfoText = GetCastInfoText((ActionType)((CastInfo)(&((BattleChara)ptr2).CastInfo)).ActionType, ((CastInfo)(&((BattleChara)ptr2).CastInfo)).ActionId, num3);
				castNode.String = ReadOnlySeString.op_Implicit(castInfoText);
				uint uInt = DalamudServiceExtension.Instance<IGameConfig>().System.GetUInt("UiHighScale");
				switch (uInt)
				{
				case 0u:
					num4 = 3f;
					break;
				case 1u:
					num4 = 0f;
					break;
				case 2u:
					num4 = -3f;
					break;
				case 3u:
					num4 = -6f;
					break;
				default:
					global::_003CPrivateImplementationDetails_003E.ThrowSwitchExpressionException(uInt);
					break;
				}
				float num5 = num4;
				float num6 = castNode.GetTextDrawSize(considerScale: false).X + num5;
				NineGridNode nineGridNode = castBackgroundNode;
				Vector2 castBackgroundTextDefaultPosition = CastBackgroundTextDefaultPosition;
				castBackgroundTextDefaultPosition.X = castNode.Position.X - num6 - 2f;
				nineGridNode.Position = castBackgroundTextDefaultPosition;
				castBackgroundNode.Width = num6 + 5f * ((float)(int)config.TextSize / 10f);
			}
			int num7 = 0;
			int num8 = -1;
			Span<HaterInfo> haters = ((Hater)(&((UIState)UIState.Instance()).Hater)).Haters;
			for (int j = 0; j < haters.Length; j++)
			{
				HaterInfo val3 = haters[j];
				num7++;
				if (num7 > ((Hater)(&((UIState)UIState.Instance()).Hater)).HaterCount)
				{
					break;
				}
				if (val3.EntityId == ((BattleChara)ptr2).EntityId)
				{
					num8 = val3.Enmity;
					break;
				}
			}
			if (num8 > 0 && num8 != 100)
			{
				enemityNode.String = ReadOnlySeString.op_Implicit($"{num8}%");
				enemityNode.TextColor = config.EnemityTextColor;
				enemityNode.TextOutlineColor = config.EnemityTextEdgeColor;
				enemityNode.FontSize = config.EnemityTextSize;
				enemityNode.Position = EnemityTextDefaultPosition + config.EnemityTextOffset;
				enemityNode.IsVisible = true;
			}
			else
			{
				enemityNode.IsVisible = false;
			}
			void HideNodes()
			{
				castNode.IsVisible = false;
				healthNode.IsVisible = false;
				castBackgroundNode.IsVisible = false;
				healthBackgroundNode.IsVisible = false;
				castBarNode.IsVisible = false;
				statusNodes.ShouldBeVisible = false;
			}
		}
	}

	private unsafe void CreateNodes()
	{
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		if (Addons.EnemyList == null || !TryFindButtonNodes(out List<nint> list))
		{
			return;
		}
		if (controller == null)
		{
			controller = new OverlayController();
		}
		int num = -1;
		foreach (nint item in list)
		{
			if (((AtkResNode)((AtkUldManager)(&((AtkComponentBase)((AtkComponentNode)item).Component).UldManager)).SearchNodeById(4u)).GetAsAtkTextNode() != null)
			{
				num++;
				TextNode textNode = new TextNode
				{
					TextFlags = (TextFlags)9,
					AlignmentType = (AlignmentType)2
				};
				ProgressBarEnemyCastNode progressBarEnemyCastNode = new ProgressBarEnemyCastNode
				{
					IsVisible = true,
					Position = new Vector2(86f, 16f),
					Size = new Vector2(120f, 20f)
				};
				SimpleNineGridNode simpleNineGridNode = new SimpleNineGridNode
				{
					TexturePath = "ui/uld/EnemyList.tex",
					TextureCoordinates = new Vector2(96f, 80f),
					TextureSize = new Vector2(24f, 20f),
					Size = new Vector2(124f, 24f),
					Offsets = new Vector4(8f),
					Alpha = 1f
				};
				TextNode textNode2 = new TextNode
				{
					FontType = (FontType)2,
					TextFlags = (TextFlags)8,
					AlignmentType = (AlignmentType)0
				};
				TextNode textNode3 = new TextNode
				{
					String = ReadOnlySeString.op_Implicit("%"),
					TextFlags = (TextFlags)8,
					AlignmentType = (AlignmentType)0
				};
				SimpleNineGridNode simpleNineGridNode2 = new SimpleNineGridNode
				{
					TexturePath = "ui/uld/EnemyList.tex",
					TextureCoordinates = new Vector2(96f, 80f),
					TextureSize = new Vector2(24f, 20f),
					Size = new Vector2(60f, 24f),
					Position = new Vector2(-64f, 6f),
					Offsets = new Vector4(8f),
					Alpha = 0.6f
				};
				TextNode textNode4 = new TextNode
				{
					Position = EnemityTextDefaultPosition,
					FontSize = 8u,
					TextFlags = (TextFlags)9,
					AlignmentType = (AlignmentType)4
				};
				AtkColors.Value.ApplyTo(textNode4);
				simpleNineGridNode.AttachNode((AtkComponentNode*)item);
				textNode.AttachNode((AtkComponentNode*)item);
				simpleNineGridNode2.AttachNode((AtkComponentNode*)item);
				textNode2.AttachNode((AtkComponentNode*)item);
				textNode3.AttachNode((AtkComponentNode*)item);
				progressBarEnemyCastNode.AttachNode((AtkComponentNode*)item);
				textNode4.AttachNode((AtkComponentNode*)item);
				IconTextNodesRow iconTextNodesRow = new IconTextNodesRow(5, ((AtkComponentNode)item).NodeId, num);
				controller.AddNode(iconTextNodesRow);
				nodes.Add(new EnemyListNode(((AtkComponentNode)item).NodeId, textNode, textNode2, textNode3, simpleNineGridNode, simpleNineGridNode2, progressBarEnemyCastNode, iconTextNodesRow, textNode4));
			}
		}
	}

	private static string GetCastInfoText(ActionType type, uint actionID, float remainingTime)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		string text = string.Empty;
		if ((int)type == 1)
		{
			text = LuminaWrapper.GetActionName(actionID);
		}
		if (string.IsNullOrEmpty(text))
		{
			text = LuminaWrapper.GetAddonText(16482u);
		}
		string text2 = remainingTime.ToString("F1");
		return text + "：" + text2;
	}

	private unsafe static bool TryFindButtonNodes(out List<nint> nodes)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Invalid comparison between Unknown and I4
		nodes = new List<nint>();
		if (Addons.EnemyList == null)
		{
			return false;
		}
		for (int i = 4; i < ((AtkUldManager)(&((AtkUnitBase)Addons.EnemyList).UldManager)).NodeListCount; i++)
		{
			AtkResNode* ptr = ((AtkUldManager)(&((AtkUnitBase)Addons.EnemyList).UldManager)).NodeList[i];
			if (ptr != null && (int)((AtkResNode)ptr).Type == 1001 && ((AtkResNode)ptr).GetAsAtkComponentButton() != null)
			{
				nodes.Add((nint)ptr);
			}
		}
		nodes.Reverse();
		return nodes.Count > 0;
	}
}
