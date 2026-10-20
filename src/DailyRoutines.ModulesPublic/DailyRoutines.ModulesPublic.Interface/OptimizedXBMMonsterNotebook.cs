using System;
using System.Numerics;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic.Interface;

public class OptimizedXBMMonsterNotebook : ModuleBase
{
	private unsafe delegate void BuildNotebookDetailDelegate(void* agent, AtkValue* values);

	private static readonly CompSig BuildNotebookDetailSig = new CompSig("40 53 56 57 41 56 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 84 24 ?? ?? ?? ?? 48 8B DA");

	private Hook<BuildNotebookDetailDelegate>? BuildNotebookDetailHook;

	private TextButtonNode? smallButton;

	private TextButtonNode? mediumButton;

	private TextButtonNode? largeButton;

	private const int AGENT_UNLOCKED_PETS_OFFSET = 112;

	private const int AGENT_SELECTED_PET_ID_OFFSET = 156;

	private const int AGENT_LIST_DIRTY_OFFSET = 94;

	private const int AGENT_DETAIL_DIRTY_OFFSET = 95;

	private const uint MIN_PET_ID = 1u;

	private const uint MAX_PET_ID = 50u;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("OptimizedXBMMonsterNotebookTitle"),
		Description = LanguageManager.Get("OptimizedXBMMonsterNotebookDescription"),
		Category = ModuleCategory.Interface
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected unsafe override void Init()
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Expected O, but got Unknown
		BuildNotebookDetailHook = BuildNotebookDetailSig.GetHook<BuildNotebookDetailDelegate>(BuildNotebookDetailDetour);
		BuildNotebookDetailHook.Enable();
		if ((*Addons.XBMMonsterBookDetail).IsAddonAndNodesReady())
		{
			MarkAddonDirty();
		}
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)5, "XBMMonsterBookDetail", new AddonEventDelegate(OnAddon));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, "XBMMonsterBookDetail", new AddonEventDelegate(OnAddon));
	}

	protected override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddon });
		smallButton?.Dispose();
		smallButton = null;
		mediumButton?.Dispose();
		mediumButton = null;
		largeButton?.Dispose();
		largeButton = null;
	}

	private unsafe void OnAddon(AddonEvent type, AddonArgs args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		if ((int)type != 5)
		{
			if ((int)type == 6)
			{
				smallButton = null;
				mediumButton = null;
				largeButton = null;
			}
		}
		else
		{
			if (Addons.XBMMonsterBookDetail == null)
			{
				return;
			}
			if (smallButton == null)
			{
				smallButton = new TextButtonNode
				{
					Size = new Vector2(20f),
					Position = new Vector2(0f),
					OnClick = delegate
					{
						if (LuminaGetter.TryGetRow<XBMPet>(GetSelectedXBMPetID(), out XBMPet item))
						{
							OmenServiceBase<ChatManager>.Instance().SendCommand("/beastsize " + LuminaWrapper.GetPetName((uint)((XBMPet)(ref item)).Unknown4) + " small");
							MarkAddonDirty();
						}
					}
				};
				smallButton.BackgroundNode.IsVisible = false;
				smallButton.AttachNode(((AtkUnitBase)Addons.XBMMonsterBookDetail).GetNodeById(15u));
			}
			if (mediumButton == null)
			{
				mediumButton = new TextButtonNode
				{
					Size = new Vector2(20f),
					Position = new Vector2(22f, 0f),
					OnClick = delegate
					{
						if (LuminaGetter.TryGetRow<XBMPet>(GetSelectedXBMPetID(), out XBMPet item))
						{
							OmenServiceBase<ChatManager>.Instance().SendCommand("/beastsize " + LuminaWrapper.GetPetName((uint)((XBMPet)(ref item)).Unknown4) + " medium");
							MarkAddonDirty();
						}
					}
				};
				mediumButton.BackgroundNode.IsVisible = false;
				mediumButton.AttachNode(((AtkUnitBase)Addons.XBMMonsterBookDetail).GetNodeById(15u));
			}
			if (largeButton != null)
			{
				return;
			}
			largeButton = new TextButtonNode
			{
				Size = new Vector2(20f),
				Position = new Vector2(44f, 0f),
				OnClick = delegate
				{
					if (LuminaGetter.TryGetRow<XBMPet>(GetSelectedXBMPetID(), out XBMPet item))
					{
						OmenServiceBase<ChatManager>.Instance().SendCommand("/beastsize " + LuminaWrapper.GetPetName((uint)((XBMPet)(ref item)).Unknown4) + " large");
						MarkAddonDirty();
					}
				}
			};
			largeButton.BackgroundNode.IsVisible = false;
			largeButton.AttachNode(((AtkUnitBase)Addons.XBMMonsterBookDetail).GetNodeById(15u));
		}
	}

	private unsafe void BuildNotebookDetailDetour(void* agent, AtkValue* values)
	{
		uint selectedXBMPetID = GetSelectedXBMPetID();
		if ((selectedXBMPetID < 1 || selectedXBMPetID > 50) ? true : false)
		{
			BuildNotebookDetailHook.Original(agent, values);
			return;
		}
		uint num = selectedXBMPetID - 1 >> 3;
		byte b = (byte)(1 << (int)((selectedXBMPetID - 1) & 7));
		byte* ptr = (byte*)agent + 112;
		byte b2 = ptr[num];
		ptr[num] = (byte)(b2 | b);
		try
		{
			BuildNotebookDetailHook.Original(agent, values);
		}
		finally
		{
			ptr[num] = b2;
		}
	}

	private unsafe static void MarkAddonDirty()
	{
		AgentModule* ptr = AgentModule.Instance();
		if (ptr != null)
		{
			nint agentByInternalId = (nint)((AgentModule)ptr).GetAgentByInternalId((AgentId)500);
			if (agentByInternalId != IntPtr.Zero)
			{
				*(sbyte*)(agentByInternalId + 94) = 1;
				*(sbyte*)(agentByInternalId + 95) = 1;
			}
		}
	}

	private unsafe static uint GetSelectedXBMPetID()
	{
		AgentModule* ptr = AgentModule.Instance();
		if (ptr == null)
		{
			return 0u;
		}
		nint agentByInternalId = (nint)((AgentModule)ptr).GetAgentByInternalId((AgentId)500);
		if (agentByInternalId == IntPtr.Zero)
		{
			return 0u;
		}
		return *(uint*)(agentByInternalId + 156);
	}
}
