using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Hooking;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.Interop;
using OmenTools.Extensions;
using OmenTools.ImGuiOm;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.ModulesPublic;

public class AutoHideGameObjects : ModuleBase
{
	private unsafe delegate void* UpdateObjectArraysDelegate(GameObjectManager* objectManager);

	private class Config : ModuleConfig
	{
		public FilterConfig DefaultConfig = new FilterConfig();
	}

	private class FilterConfig
	{
		public bool HideChocobo = true;

		public bool HidePet = true;

		public bool HidePlayer = true;

		public bool HideUnimportantENPC = true;
	}

	private enum RenderFlag
	{
		Invisible = 0x100
	}

	private static readonly CompSig UpdateObjectArraysSig = new CompSig("40 57 48 83 EC ?? 48 89 5C 24 ?? 33 DB");

	private Hook<UpdateObjectArraysDelegate> UpdateObjectArraysHook;

	private Config config;

	private readonly HashSet<nint> processedObjects = new HashSet<nint>();

	private int zoneUpdateCount;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoHideGameObjectsTitle"),
		Description = LanguageManager.Get("AutoHideGameObjectsDescription"),
		Category = ModuleCategory.System
	};

	protected unsafe override void Init()
	{
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 30000
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		if (UpdateObjectArraysHook == null)
		{
			UpdateObjectArraysHook = UpdateObjectArraysSig.GetHook<UpdateObjectArraysDelegate>(UpdateObjectArraysDetour);
		}
		UpdateObjectArraysHook.Enable();
		UpdateAllObjects(GameObjectManager.Instance());
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += OnZoneChanged;
		OmenServiceBase<FrameworkManager>.Instance().Reg(new OnUpdateDelegate(OnUpdate), 1000u);
	}

	protected override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		OmenServiceBase<FrameworkManager>.Instance().Unreg(OnUpdate);
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChanged;
		ResetAllObjects();
	}

	protected override void ConfigUI()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("Default")));
		IdDisposable val = ImRaii.PushId(ImU8String.op_Implicit("Default"), true);
		try
		{
			IndentDisposable val2 = ImRaii.PushIndent(1, true);
			try
			{
				if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("AutoHideGameObjects-HidePlayer")), ref config.DefaultConfig.HidePlayer))
				{
					config.Save(this);
				}
				ImGuiOm.TooltipHover(LanguageManager.Get("AutoHideGameObjects-HidePlayerHelp"));
				if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("AutoHideGameObjects-HideUnimportantENPC")), ref config.DefaultConfig.HideUnimportantENPC))
				{
					config.Save(this);
				}
				ImGuiOm.TooltipHover(LanguageManager.Get("AutoHideGameObjects-HideUnimportantENPCHelp"));
				if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("AutoHideGameObjects-HidePet")), ref config.DefaultConfig.HidePet))
				{
					config.Save(this);
				}
				ImGuiOm.TooltipHover(LanguageManager.Get("AutoHideGameObjects-HidePetHelp"));
				if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("AutoHideGameObjects-HideChocobo")), ref config.DefaultConfig.HideChocobo))
				{
					config.Save(this);
				}
				ImGuiOm.TooltipHover(LanguageManager.Get("AutoHideGameObjects-HideChocoboHelp"));
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

	private unsafe void* UpdateObjectArraysDetour(GameObjectManager* objectManager)
	{
		void* result = UpdateObjectArraysHook.Original(objectManager);
		UpdateAllObjects(objectManager);
		return result;
	}

	private unsafe void UpdateAllObjects(GameObjectManager* manager)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Invalid comparison between Unknown and I4
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Invalid comparison between Unknown and I4
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Invalid comparison between Unknown and I4
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Invalid comparison between Unknown and I4
		if (manager == null || !GameState.IsLoggedIn)
		{
			return;
		}
		if ((int)GameState.TerritoryIntendedUse != 61)
		{
			if (GameState.ContentFinderCondition != 0 || GameState.IsInPVPArea || (int)GameState.TerritoryIntendedUse == 49)
			{
				return;
			}
		}
		else if ((LocalPlayerState.Object?.Position.Y ?? (-100f)) < 0f)
		{
			return;
		}
		int playerCount = 0;
		nint targetAddress = TargetManager.Target?.Address ?? IntPtr.Zero;
		if ((int)GameState.TerritoryIntendedUse == 61 && (LocalPlayerState.Object?.Position.Y ?? (-100f)) < 0f)
		{
			return;
		}
		for (int i = 0; i < ((ObjectArrays)(&((GameObjectManager)manager).Objects)).IndexSorted.Length && i <= 629; i++)
		{
			if (i > 200 && i < 489)
			{
				i = 488;
				continue;
			}
			Pointer<GameObject> val = ((ObjectArrays)(&((GameObjectManager)manager).Objects)).IndexSorted[i];
			if ((int)GameState.TerritoryIntendedUse == 61)
			{
				if (!ShouldFilterOccultCrescent(val.Value, targetAddress, ref playerCount, (uint)i))
				{
					continue;
				}
			}
			else if (!ShouldFilter(config.DefaultConfig, val.Value, (uint)i))
			{
				continue;
			}
			((GameObject)val.Value).RenderFlags |= (VisibilityFlags)256;
			processedObjects.Add((nint)val.Value);
		}
	}

	private unsafe static bool ShouldFilter(FilterConfig config, GameObject* gameObject, uint index)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Invalid comparison between Unknown and I4
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Invalid comparison between Unknown and I4
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Invalid comparison between Unknown and I4
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Invalid comparison between Unknown and I4
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		if (gameObject == null)
		{
			return false;
		}
		if (((GameObject)gameObject).EntityId == LocalPlayerState.EntityID)
		{
			return false;
		}
		if (((RenderFlag)((GameObject)gameObject).RenderFlags).IsSet(RenderFlag.Invisible))
		{
			return false;
		}
		if (((GameObject)gameObject).NamePlateIconId != 0)
		{
			return false;
		}
		if (config.HidePlayer && index <= 200 && index % 2 == 0 && (int)((GameObject)gameObject).ObjectKind == 1)
		{
			if (((BattleChara)gameObject).IsFriend)
			{
				return false;
			}
			if (LocalPlayerState.IsInParty && (((BattleChara)gameObject).IsPartyMember || ((BattleChara)gameObject).IsAllianceMember))
			{
				return false;
			}
			return true;
		}
		if (config.HidePet && index <= 200 && index % 2 == 1 && (int)((GameObject)gameObject).ObjectKind != 8 && ((GameObject)gameObject).OwnerId != LocalPlayerState.EntityID)
		{
			return true;
		}
		if (config.HidePet && index <= 200 && index % 2 == 0 && (int)((GameObject)gameObject).ObjectKind == 2 && ((GameObject)gameObject).SubKind == 2 && ((GameObject)gameObject).OwnerId != LocalPlayerState.EntityID)
		{
			return true;
		}
		if (config.HideChocobo && index <= 200 && index % 2 == 0 && (int)((GameObject)gameObject).ObjectKind == 2 && ((GameObject)gameObject).SubKind == 3 && ((GameObject)gameObject).OwnerId != LocalPlayerState.EntityID)
		{
			return true;
		}
		if (config.HideUnimportantENPC && index >= 489 && index <= 629 && !((GameObject)gameObject).TargetableStatus.IsSet<ObjectTargetableFlags>((ObjectTargetableFlags)2) && ((GameObject)gameObject).EventHandler == null)
		{
			return true;
		}
		return false;
	}

	private unsafe bool ShouldFilterOccultCrescent(GameObject* gameObject, nint targetAddress, ref int playerCount, uint index)
	{
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Invalid comparison between Unknown and I4
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Invalid comparison between Unknown and I4
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Invalid comparison between Unknown and I4
		if (gameObject == null)
		{
			return false;
		}
		if (((GameObject)gameObject).EntityId == LocalPlayerState.EntityID)
		{
			return false;
		}
		if (((GameObject)gameObject).NamePlateIconId != 0)
		{
			return false;
		}
		if (index <= 200 && index % 2 == 0 && (int)((GameObject)gameObject).ObjectKind == 1)
		{
			playerCount++;
			if (((BattleChara)gameObject).IsDead() || gameObject == (GameObject*)targetAddress)
			{
				((GameObject)gameObject).RenderFlags &= (VisibilityFlags)(-257);
				processedObjects.Remove((nint)gameObject);
				return false;
			}
			if (((BattleChara)gameObject).IsFriend)
			{
				return false;
			}
			if (LocalPlayerState.IsInParty && (((BattleChara)gameObject).IsPartyMember || ((BattleChara)gameObject).IsAllianceMember))
			{
				return false;
			}
			return playerCount >= 10;
		}
		if (index >= 489 && index <= 629 && !((GameObject)gameObject).TargetableStatus.IsSet<ObjectTargetableFlags>((ObjectTargetableFlags)2) && ((GameObject)gameObject).EventHandler == null)
		{
			return true;
		}
		if ((int)((GameObject)gameObject).ObjectKind == 2 && index <= 200 && index % 2 == 0 && (int)((GameObject)gameObject).ObjectKind == 2 && ((GameObject)gameObject).OwnerId != LocalPlayerState.EntityID && ((GameObject)gameObject).OwnerId != 0 && ((GameObject)gameObject).OwnerId != 3758096384u)
		{
			return true;
		}
		return false;
	}

	private unsafe void ResetAllObjects()
	{
		if (!DalamudServiceExtension.Instance<IClientState>().IsLoggedIn || processedObjects.Count == 0 || GameObjectManager.Instance() == null)
		{
			return;
		}
		Span<Pointer<GameObject>> indexSorted = ((ObjectArrays)(&((GameObjectManager)GameObjectManager.Instance()).Objects)).IndexSorted;
		for (int i = 0; i < indexSorted.Length; i++)
		{
			ref Pointer<GameObject> reference = ref indexSorted[i];
			if (reference.Value != null && reference.Value != (GameObject*)(LocalPlayerState.Object?.Address ?? IntPtr.Zero) && processedObjects.Contains((nint)reference.Value))
			{
				((GameObject)reference.Value).RenderFlags &= (VisibilityFlags)(-257);
			}
		}
		processedObjects.Clear();
		zoneUpdateCount = 0;
	}

	private void OnZoneChanged(uint u)
	{
		zoneUpdateCount = 0;
		processedObjects.Clear();
	}

	private unsafe void OnUpdate(IFramework _)
	{
		if (zoneUpdateCount <= 3 && !IConditionExtension.get_IsBetweenAreas(DalamudServiceExtension.Instance<ICondition>()))
		{
			zoneUpdateCount++;
			UpdateAllObjects(GameObjectManager.Instance());
		}
	}
}
