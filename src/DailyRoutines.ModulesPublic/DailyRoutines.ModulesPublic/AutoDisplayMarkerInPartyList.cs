using System;
using System.Collections.Generic;
using System.Numerics;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.ModulesPublic;

public class AutoDisplayMarkerInPartyList : ModuleBase
{
	private unsafe delegate void UpdateMarkerLocalDelegate(MarkingController* controller, uint marker, GameObjectId objectID, uint entityID);

	private class Config : ModuleConfig
	{
		public bool HidePartyListIndexNumber = true;

		public Vector2 IconOffset = new Vector2(0f, 0f);

		public int Size = 27;
	}

	private static readonly CompSig UpdateMarkerLocalSig = new CompSig("E8 ?? ?? ?? ?? 4C 8B C5 8B D7 48 8B CB E8");

	private Hook<UpdateMarkerLocalDelegate>? UpdateMarkerLocalHook;

	private Config config;

	private readonly (short X, short Y) basePosition = (X: 41, Y: 35);

	private readonly Dictionary<int, int> markedObjects = new Dictionary<int, int>(8);

	private readonly List<IconImageNode> nodes = new List<IconImageNode>(8);

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoDisplayMarkerInPartyListTitle"),
		Description = LanguageManager.Get("AutoDisplayMarkerInPartyListDescription"),
		Category = ModuleCategory.Combat,
		Author = new string[1] { "status102" }
	};

	protected unsafe override void Init()
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Expected O, but got Unknown
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		base.TaskHelper = new TaskHelper();
		UpdateMarkerLocalHook = UpdateMarkerLocalSig.GetHook<UpdateMarkerLocalDelegate>(UpdateMarkerLocalDetour);
		UpdateMarkerLocalHook.Enable();
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += OnZoneChanged;
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)5, "_PartyList", new AddonEventDelegate(OnPartyList));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, "_PartyList", new AddonEventDelegate(OnPartyList));
	}

	protected override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnPartyList });
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChanged;
		ModifyPartyMemberNumber(visible: true);
		foreach (IconImageNode node in nodes)
		{
			node.Dispose();
		}
		nodes.Clear();
	}

	protected unsafe override void ConfigUI()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		Vector2 iconOffset = config.IconOffset;
		ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
		ImGui.InputFloat2(ImU8String.op_Implicit(LanguageManager.Get("IconOffset")), ref iconOffset, 0f, 0f, ImU8String.op_Implicit("%.1f"), (ImGuiInputTextFlags)0);
		int value;
		int key;
		if (ImGui.IsItemDeactivatedAfterEdit())
		{
			config.IconOffset = iconOffset;
			config.Save(this);
			foreach (KeyValuePair<int, int> markedObject in markedObjects)
			{
				markedObject.Deconstruct(out value, out key);
				int value2 = value;
				int i = key;
				SetNode(i, value2);
			}
		}
		ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
		ImGui.InputInt(ImU8String.op_Implicit(LanguageManager.Get("IconScale")), ref config.Size, 0, 0, default(ImU8String), (ImGuiInputTextFlags)0);
		if (ImGui.IsItemDeactivatedAfterEdit())
		{
			config.Save(this);
			foreach (KeyValuePair<int, int> markedObject2 in markedObjects)
			{
				markedObject2.Deconstruct(out key, out value);
				int value3 = key;
				int i2 = value;
				SetNode(i2, value3);
			}
		}
		if (!ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("AutoDisplayMarkerInPartyList-HidePartyListIndexNumber")), ref config.HidePartyListIndexNumber))
		{
			return;
		}
		config.Save(this);
		bool flag = config.HidePartyListIndexNumber;
		for (int j = 0; j < 8; j++)
		{
			AtkResNode* nodeById = ((AtkUnitBase)Addons.PartyList).GetNodeById((uint)(10 + j));
			if (nodeById != null && ((AtkResNode)nodeById).IsVisible())
			{
				flag = flag && nodes[j].IsVisible;
			}
		}
		ModifyPartyMemberNumber(!flag);
	}

	private void OnZoneChanged(uint zone)
	{
		for (int i = 0; i < 8; i++)
		{
			SetNode(i, null);
		}
		markedObjects.Clear();
		ModifyPartyMemberNumber(visible: true);
	}

	private unsafe void UpdateMarkerLocalDetour(MarkingController* controller, uint marker, GameObjectId objectID, uint entityID)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		base.TaskHelper.Enqueue(delegate
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			ProcessMarkIconSetted(marker, (uint)GameObjectId.op_Implicit(objectID));
		});
		UpdateMarkerLocalHook.Original(controller, marker, objectID, entityID);
	}

	private unsafe void OnPartyList(AddonEvent type, AddonArgs args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		if ((int)type != 5)
		{
			if ((int)type == 6)
			{
				nodes.Clear();
			}
		}
		else
		{
			if (!(*Addons.PartyList).IsAddonAndNodesReady() || nodes.Count != 0)
			{
				return;
			}
			for (int i = 0; i < 8; i++)
			{
				IconImageNode iconImageNode = new IconImageNode
				{
					IconId = 61201u,
					NodeFlags = (NodeFlags)128,
					DrawFlags = DrawFlags.None,
					WrapMode = WrapMode.Stretch
				};
				iconImageNode.Priority = 5;
				nodes.Add(iconImageNode);
				iconImageNode.AttachNode(Addons.PartyList);
			}
			if (MarkingController.Instance() == null)
			{
				return;
			}
			Span<GameObjectId> markers = ((MarkingController)MarkingController.Instance()).Markers;
			for (int j = 0; j < markers.Length; j++)
			{
				uint gameObjectID = markers[j].ObjectId;
				uint num = gameObjectID;
				if ((num != 0 && num != 3758096384u) || 1 == 0)
				{
					uint index = (uint)j;
					base.TaskHelper.Enqueue(delegate
					{
						ProcessMarkIconSetted(index, gameObjectID);
					});
				}
			}
		}
	}

	private unsafe void ModifyPartyMemberNumber(bool visible)
	{
		if (!(*Addons.PartyList).IsAddonAndNodesReady() || (!config.HidePartyListIndexNumber && !visible))
		{
			return;
		}
		for (int i = 10; i <= 17; i++)
		{
			AtkResNode* nodeById = ((AtkUnitBase)Addons.PartyList).GetNodeById((uint)i);
			if (nodeById != null && ((AtkResNode)nodeById).GetComponent() != null && ((AtkResNode)nodeById).IsVisible())
			{
				AtkResNode* ptr = ((AtkUldManager)(&((AtkComponentBase)((AtkResNode)nodeById).GetComponent()).UldManager)).SearchNodeById(16u);
				if (ptr != null && ((AtkResNode)ptr).IsVisible() != visible)
				{
					((AtkResNode)ptr).ToggleVisibility(visible);
				}
			}
		}
	}

	private unsafe void ProcessMarkIconSetted(uint markIndex, uint entityID)
	{
		if (AgentHUD.Instance() == null || InfoProxyCrossRealm.Instance() == null || !(*Addons.PartyList).IsAddonAndNodesReady())
		{
			return;
		}
		int num = (int)(markIndex + 1);
		int index2;
		int value;
		if (num <= 0 || num > LuminaGetter.Get<Marker>().Count || !LuminaGetter.TryGetRow<Marker>((uint)num, out Marker item))
		{
			if (FindMember(entityID, out var index))
			{
				ClearMemberMark(index);
			}
		}
		else if ((entityID == 234881024 || entityID == 3758096384u) ? true : false)
		{
			ClearMark(((Marker)(ref item)).Icon);
		}
		else if (!FindMember(entityID, out index2))
		{
			ClearMark(((Marker)(ref item)).Icon);
		}
		else if (!markedObjects.TryGetValue(((Marker)(ref item)).Icon, out value) || value != index2)
		{
			ClearMemberMark(index2);
			markedObjects[((Marker)(ref item)).Icon] = index2;
			SetNode(index2, ((Marker)(ref item)).Icon);
			ModifyPartyMemberNumber(visible: false);
		}
	}

	private void ClearMemberMark(int memberIndex)
	{
		foreach (KeyValuePair<int, int> markedObject in markedObjects)
		{
			if (markedObject.Value == memberIndex)
			{
				markedObjects.Remove(markedObject.Key);
				SetNode(memberIndex, null);
				break;
			}
		}
		if (markedObjects.Count == 0)
		{
			ModifyPartyMemberNumber(visible: true);
		}
	}

	private void ClearMark(int markID)
	{
		if (markedObjects.Remove(markID, out var value))
		{
			SetNode(value, null);
		}
		if (markedObjects.Count == 0)
		{
			ModifyPartyMemberNumber(visible: true);
		}
	}

	private unsafe void SetNode(int i, int? iconID)
	{
		if (i >= 0 && i < nodes.Count && (*Addons.PartyList).IsAddonAndNodesReady())
		{
			IconImageNode iconImageNode = nodes[i];
			iconImageNode.IsVisible = iconID.HasValue;
			if (iconID.HasValue)
			{
				int valueOrDefault = iconID.GetValueOrDefault();
				iconImageNode.LoadIcon((uint)valueOrDefault);
				AtkResNode* nodeById = ((AtkUnitBase)Addons.PartyList).GetNodeById((uint)(10 + i));
				iconImageNode.Position = new Vector2(((AtkResNode)nodeById).X + (float)basePosition.X + config.IconOffset.X, ((AtkResNode)nodeById).Y + (float)basePosition.Y + config.IconOffset.Y);
				iconImageNode.TextureSize = iconImageNode.ActualTextureSize;
				iconImageNode.Size = new Vector2(config.Size);
			}
		}
	}

	private unsafe static bool FindMember(uint entityID, out int index)
	{
		AgentHUD* ptr = AgentHUD.Instance();
		for (int i = 0; i < ((AgentHUD)ptr).PartyMemberCount; i++)
		{
			if (entityID == ((AgentHUD)ptr).PartyMembers[i].EntityId)
			{
				index = i;
				return true;
			}
		}
		if (((InfoProxyCrossRealm)InfoProxyCrossRealm.Instance()).IsCrossRealm)
		{
			CrossRealmMember* memberByEntityId = InfoProxyCrossRealm.GetMemberByEntityId(LocalPlayerState.EntityID);
			CrossRealmMember* memberByEntityId2 = InfoProxyCrossRealm.GetMemberByEntityId(entityID);
			if (memberByEntityId != null && memberByEntityId2 != null && ((CrossRealmMember)memberByEntityId2).GroupIndex == ((CrossRealmMember)memberByEntityId).GroupIndex)
			{
				index = ((CrossRealmMember)memberByEntityId2).MemberIndex;
				return true;
			}
		}
		index = -1;
		return false;
	}
}
