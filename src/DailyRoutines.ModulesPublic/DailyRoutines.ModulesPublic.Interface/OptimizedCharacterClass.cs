using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.Addon.Events;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Sound;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Dalamud;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.ModulesPublic.Interface;

public class OptimizedCharacterClass : ModuleBase
{
	private readonly List<AtkEventWrapper> events = new List<AtkEventWrapper>();

	private static readonly FrozenDictionary<uint, uint> ClassJobComponentMap = new Dictionary<uint, uint>
	{
		[8u] = 19u,
		[10u] = 21u,
		[12u] = 32u,
		[14u] = 37u,
		[20u] = 24u,
		[22u] = 28u,
		[24u] = 33u,
		[26u] = 40u,
		[32u] = 20u,
		[34u] = 22u,
		[36u] = 30u,
		[38u] = 34u,
		[40u] = 39u,
		[42u] = 41u,
		[44u] = 43u,
		[50u] = 23u,
		[52u] = 31u,
		[54u] = 38u,
		[60u] = 25u,
		[62u] = 27u,
		[64u] = 35u,
		[66u] = 42u,
		[68u] = 36u,
		[73u] = 8u,
		[74u] = 9u,
		[75u] = 10u,
		[76u] = 11u,
		[77u] = 12u,
		[78u] = 13u,
		[79u] = 14u,
		[80u] = 15u,
		[86u] = 16u,
		[88u] = 17u,
		[90u] = 18u
	}.ToFrozenDictionary();

	private static readonly FrozenDictionary<uint, uint> PVPClassJobComponentMap = new Dictionary<uint, uint>
	{
		[14u] = 19u,
		[16u] = 21u,
		[18u] = 32u,
		[20u] = 37u,
		[26u] = 20u,
		[28u] = 22u,
		[30u] = 30u,
		[32u] = 34u,
		[34u] = 39u,
		[36u] = 41u,
		[42u] = 24u,
		[44u] = 28u,
		[46u] = 33u,
		[48u] = 40u,
		[54u] = 23u,
		[56u] = 31u,
		[58u] = 38u,
		[64u] = 25u,
		[66u] = 27u,
		[68u] = 35u,
		[70u] = 42u
	}.ToFrozenDictionary();

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("OptimizedCharacterClassTitle"),
		Description = LanguageManager.Get("OptimizedCharacterClassDescription"),
		Category = ModuleCategory.Interface,
		Author = new string[1] { "Middo" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected unsafe override void Init()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Expected O, but got Unknown
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Expected O, but got Unknown
		if (base.TaskHelper == null)
		{
			TaskHelper taskHelper = (base.TaskHelper = new TaskHelper());
		}
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)1, "CharacterClass", new AddonEventDelegate(OnAddon));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, "CharacterClass", new AddonEventDelegate(OnAddon));
		if ((*Addons.CharacterClass).IsAddonAndNodesReady())
		{
			OnAddon((AddonEvent)1, null);
		}
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)1, "PvPCharacter", new AddonEventDelegate(OnAddonPVP));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, "PvPCharacter", new AddonEventDelegate(OnAddonPVP));
		if ((*Addons.PvPCharacter).IsAddonAndNodesReady())
		{
			OnAddonPVP((AddonEvent)1, null);
		}
	}

	protected override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[2] { OnAddon, OnAddonPVP });
		ClearEvents();
	}

	private unsafe void AddCollisionEvent(AtkUnitBase* addon, AtkComponentNode* componentNode, uint classJobID)
	{
		if (!LuminaGetter.TryGetRow<ClassJob>(classJobID, out ClassJob classJob))
		{
			return;
		}
		AtkCollisionNode* ptr = ((AtkComponentBase)((AtkComponentNode)componentNode).Component).UldManager.SearchSimpleNodeByType<AtkCollisionNode>((NodeType)8);
		if (ptr == null)
		{
			return;
		}
		List<nint> list = ((AtkComponentBase)((AtkComponentNode)componentNode).Component).UldManager.SearchSimpleNodesByType((NodeType)2);
		if (list == null || list.Count != 0)
		{
			((AtkEventManager)(&((AtkCollisionNode)ptr).AtkEventManager)).ClearEvents();
			AtkImageNode* iconNode = (AtkImageNode*)list.Last();
			AtkEventWrapper atkEventWrapper = new AtkEventWrapper(delegate
			{
				//IL_0036: Unknown result type (might be due to invalid IL or missing references)
				DLog.Debug($"[{"OptimizedCharacterClass"}] 切换至职业 {((ClassJob)(ref classJob)).Name} ({classJobID})");
				LocalPlayerState.SwitchGearset(classJobID);
				UIGlobals.PlaySoundEffect(1u, (SoundData**)null, (SoundData**)null, (byte)0);
			});
			atkEventWrapper.Add(addon, (AtkResNode*)ptr, (AtkEventType)9);
			AtkEventWrapper atkEventWrapper2 = new AtkEventWrapper(delegate(AtkEventType _, AtkUnitBase* ownerAddon, AtkEvent* _, AtkEventData* _)
			{
				//IL_002d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0038: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Unknown result type (might be due to invalid IL or missing references)
				//IL_007f: Unknown result type (might be due to invalid IL or missing references)
				DalamudServiceExtension.Instance<IAddonEventManager>().SetCursor((AddonCursorType)11);
				UIGlobals.PlaySoundEffect(0u, (SoundData**)null, (SoundData**)null, (byte)0);
				((AtkTooltipManager)(&((AtkStage)AtkStage.Instance()).TooltipManager)).ShowTooltip(((AtkUnitBase)ownerAddon).Id, (AtkResNode*)iconNode, ReadOnlySeString.op_Implicit(new ReadOnlySeString(new SeStringBuilder().AddIcon(classJob.ToBitmapFontIcon()).AddText($" {((ClassJob)(ref classJob)).Name}").Build()
					.Encode())));
			});
			atkEventWrapper2.Add(addon, (AtkResNode*)ptr, (AtkEventType)6);
			AtkEventWrapper atkEventWrapper3 = new AtkEventWrapper(delegate(AtkEventType _, AtkUnitBase* ownerAddon, AtkEvent* _, AtkEventData* _)
			{
				DalamudServiceExtension.Instance<IAddonEventManager>().ResetCursor();
				((AtkTooltipManager)(&((AtkStage)AtkStage.Instance()).TooltipManager)).HideTooltip(((AtkUnitBase)ownerAddon).Id, false);
			});
			atkEventWrapper3.Add(addon, (AtkResNode*)ptr, (AtkEventType)7);
			events.Add(atkEventWrapper, atkEventWrapper2, atkEventWrapper3);
		}
	}

	private unsafe void OnAddon(AddonEvent type, AddonArgs args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		if ((int)type != 1)
		{
			if ((int)type == 6)
			{
				ClearEvents();
			}
		}
		else
		{
			if (Addons.CharacterClass == null)
			{
				return;
			}
			List<AtkEventWrapper> list = events;
			if (list == null || list.Count != 0)
			{
				return;
			}
			base.TaskHelper.Enqueue(delegate
			{
				if (!(*Addons.CharacterClass).IsAddonAndNodesReady())
				{
					return false;
				}
				foreach (KeyValuePair<uint, uint> item in ClassJobComponentMap)
				{
					item.Deconstruct(out var key, out var value);
					uint num = key;
					uint classJobID = value;
					AtkComponentNode* componentNodeById = ((AtkUnitBase)Addons.CharacterClass).GetComponentNodeById(num);
					if (componentNodeById != null)
					{
						AddCollisionEvent(Addons.CharacterClass, componentNodeById, classJobID);
					}
				}
				return true;
			});
		}
	}

	private unsafe void OnAddonPVP(AddonEvent type, AddonArgs args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		if ((int)type != 1)
		{
			if ((int)type == 6)
			{
				ClearEvents();
			}
		}
		else
		{
			if (Addons.PvPCharacter == null)
			{
				return;
			}
			List<AtkEventWrapper> list = events;
			if (list == null || list.Count != 0)
			{
				return;
			}
			base.TaskHelper.Enqueue(delegate
			{
				if (!(*Addons.PvPCharacter).IsAddonAndNodesReady())
				{
					return false;
				}
				foreach (KeyValuePair<uint, uint> item in PVPClassJobComponentMap)
				{
					item.Deconstruct(out var key, out var value);
					uint num = key;
					uint classJobID = value;
					AtkComponentNode* componentNodeById = ((AtkUnitBase)Addons.PvPCharacter).GetComponentNodeById(num);
					if (componentNodeById != null)
					{
						AddCollisionEvent(Addons.PvPCharacter, componentNodeById, classJobID);
					}
				}
				return true;
			});
		}
	}

	private void ClearEvents()
	{
		foreach (AtkEventWrapper item in events.ToList())
		{
			item.Dispose();
		}
		events.Clear();
	}
}
