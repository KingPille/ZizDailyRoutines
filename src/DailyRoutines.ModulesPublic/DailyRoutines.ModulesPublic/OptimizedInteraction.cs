using System;
using System.Collections.Frozen;
using System.Linq;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Hooking;
using Dalamud.Memory;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.Sheets;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Info.Game.Packets.Upstream;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.ModulesPublic;

public class OptimizedInteraction : ModuleBase
{
	private delegate bool CameraObjectBlockedDelegate(nint a1, nint a2, nint a3);

	private unsafe delegate bool IsObjectInViewRangeDelegate(TargetSystem* system, GameObject* gameObject);

	private delegate bool InteractCheck0Delegate(nint a1, nint a2, nint a3, nint a4, bool a5);

	private delegate bool IsPlayerOnJumpingDelegate(nint a1);

	private unsafe delegate bool CheckTargetPositionDelegate(EventFramework* framework, GameObject* source, GameObject* target, ushort interactType, bool sendError, byte a6);

	private unsafe delegate float CheckTargetDistanceDelegate(GameObject* localPlayer, GameObject* target);

	private unsafe delegate ulong InteractWithObjectDelegate(TargetSystem* system, GameObject* obj, bool checkLOS = true);

	private unsafe delegate bool CheckCameraPositionDelegate(TargetSystem* system, Camera* activeCamera, GameObject* obj);

	private static readonly CompSig CameraObjectBlockedSig = new CompSig("E8 ?? ?? ?? ?? 84 C0 75 ?? B9 ?? ?? ?? ?? E8 ?? ?? ?? ?? EB ?? 40 B7");

	private Hook<CameraObjectBlockedDelegate>? CameraObjectBlockedHook;

	private static readonly CompSig IsObjectInViewRangeSig = new CompSig("E8 ?? ?? ?? ?? 84 C0 75 ?? 48 8B 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 8B C8 48 8B 10 FF 52 ?? 48 8B C8 BA ?? ?? ?? ?? E8 ?? ?? ?? ?? E9");

	private Hook<IsObjectInViewRangeDelegate>? IsObjectInViewRangeHook;

	private static readonly CompSig InteractCheck0Sig = new CompSig("48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC ?? 49 8B 00 49 8B C8 49 8B F8 48 8B F2");

	private Hook<InteractCheck0Delegate>? InteractCheck0Hook;

	private static readonly CompSig IsPlayerOnJumping0Sig = new CompSig("83 B9 ?? ?? ?? ?? ?? 0F 95 C0 C3 CC CC CC CC CC CC CC CC CC CC CC CC CC CC CC CC CC CC CC CC CC 83 B9 ?? ?? ?? ?? ?? 0F 94 C0 C3 CC CC CC CC CC CC CC CC CC CC CC CC CC CC CC CC CC CC CC CC CC 83 B9");

	private Hook<IsPlayerOnJumpingDelegate>? IsPlayerOnJumping0Hook;

	private static readonly CompSig IsPlayerOnJumping1Sig = new CompSig("E8 ?? ?? ?? ?? 84 C0 0F 85 ?? ?? ?? ?? 83 BF ?? ?? ?? ?? ?? 75 ?? 38 1D");

	private Hook<IsPlayerOnJumpingDelegate>? IsPlayerOnJumping1Hook;

	private static readonly CompSig IsPlayerOnJumping2Sig = new CompSig("40 53 48 83 EC ?? 48 8D 99 ?? ?? ?? ?? 48 8B CB E8 ?? ?? ?? ?? 84 C0 75");

	private Hook<IsPlayerOnJumpingDelegate>? IsPlayerOnJumping2Hook;

	private static readonly CompSig CheckTargetPositionSig = new CompSig("40 53 57 41 56 48 83 EC ?? 48 8B 02");

	private Hook<CheckTargetPositionDelegate>? CheckTargetPositionHook;

	private static readonly CompSig CheckTargetDistanceSig = new CompSig("E8 ?? ?? ?? ?? 0F 2F 05 ?? ?? ?? ?? 76 ?? 48 8B 03 48 8B CB FF 50 ?? 48 8B C8 BA ?? ?? ?? ?? E8 ?? ?? ?? ?? EB");

	private Hook<CheckTargetDistanceDelegate>? CheckTargetDistanceHook;

	private static readonly CompSig InteractWithObjectSig = new CompSig("48 89 5C 24 ?? 48 89 6C 24 ?? 56 48 83 EC ?? 48 8B E9 41 0F B6 F0");

	private Hook<InteractWithObjectDelegate>? InteractWithObjectHook;

	private static readonly CompSig CheckCameraPositionSig = new CompSig("E8 ?? ?? ?? ?? 84 C0 75 ?? B9 ?? ?? ?? ?? E8 ?? ?? ?? ?? EB");

	private Hook<CheckCameraPositionDelegate>? CheckCameraPositionHook;

	private static readonly FrozenSet<string> WhitelistObjectNames;

	private static readonly FrozenSet<uint> FirstQuest;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("OptimizedInteractionTitle"),
		Description = LanguageManager.Get("OptimizedInteractionDescription"),
		Category = ModuleCategory.System
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		NeedAuth = true
	};

	protected override void Init()
	{
		if (base.TaskHelper == null)
		{
			TaskHelper taskHelper = (base.TaskHelper = new TaskHelper());
		}
		InitHooks();
		SwitchHooks(isEnable: true);
	}

	private unsafe void InitHooks()
	{
		if (CameraObjectBlockedHook == null)
		{
			CameraObjectBlockedHook = CameraObjectBlockedSig.GetHook<CameraObjectBlockedDelegate>(CameraObjectBlockedDetour);
		}
		if (IsObjectInViewRangeHook == null)
		{
			IsObjectInViewRangeHook = IsObjectInViewRangeSig.GetHook<IsObjectInViewRangeDelegate>(IsObjectInViewRangeHookDetour);
		}
		if (InteractCheck0Hook == null)
		{
			InteractCheck0Hook = InteractCheck0Sig.GetHook<InteractCheck0Delegate>(InteractCheck0Detour);
		}
		if (IsPlayerOnJumping0Hook == null)
		{
			IsPlayerOnJumping0Hook = IsPlayerOnJumping0Sig.GetHook<IsPlayerOnJumpingDelegate>(IsPlayerOnJumpingDetour);
		}
		if (IsPlayerOnJumping1Hook == null)
		{
			IsPlayerOnJumping1Hook = IsPlayerOnJumping1Sig.GetHook<IsPlayerOnJumpingDelegate>(IsPlayerOnJumpingDetour);
		}
		if (IsPlayerOnJumping2Hook == null)
		{
			IsPlayerOnJumping2Hook = IsPlayerOnJumping2Sig.GetHook<IsPlayerOnJumpingDelegate>(IsPlayerOnJumpingDetour);
		}
		if (CheckTargetPositionHook == null)
		{
			CheckTargetPositionHook = CheckTargetPositionSig.GetHook<CheckTargetPositionDelegate>(CheckTargetPositionDetour);
		}
		if (CheckTargetDistanceHook == null)
		{
			CheckTargetDistanceHook = CheckTargetDistanceSig.GetHook<CheckTargetDistanceDelegate>(CheckTargetDistanceDetour);
		}
		if (InteractWithObjectHook == null)
		{
			InteractWithObjectHook = InteractWithObjectSig.GetHook<InteractWithObjectDelegate>(InteractWithObjectDetour);
		}
		if (CheckCameraPositionHook == null)
		{
			CheckCameraPositionHook = CheckCameraPositionSig.GetHook<CheckCameraPositionDelegate>(CheckCameraPositionDetour);
		}
	}

	private void SwitchHooks(bool isEnable)
	{
		CameraObjectBlockedHook.Toggle<CameraObjectBlockedDelegate>(isEnable);
		IsObjectInViewRangeHook.Toggle<IsObjectInViewRangeDelegate>(isEnable);
		InteractCheck0Hook.Toggle<InteractCheck0Delegate>(isEnable);
		IsPlayerOnJumping0Hook.Toggle<IsPlayerOnJumpingDelegate>(isEnable);
		IsPlayerOnJumping1Hook.Toggle<IsPlayerOnJumpingDelegate>(isEnable);
		IsPlayerOnJumping2Hook.Toggle<IsPlayerOnJumpingDelegate>(isEnable);
		CheckTargetPositionHook.Toggle<CheckTargetPositionDelegate>(isEnable);
		CheckTargetDistanceHook.Toggle<CheckTargetDistanceDelegate>(isEnable);
		InteractWithObjectHook.Toggle<InteractWithObjectDelegate>(isEnable);
		CheckCameraPositionHook.Toggle<CheckCameraPositionDelegate>(isEnable);
	}

	private static bool CameraObjectBlockedDetour(nint a1, nint a2, nint a3)
	{
		return true;
	}

	private unsafe static bool IsObjectInViewRangeHookDetour(TargetSystem* system, GameObject* gameObject)
	{
		return true;
	}

	private static bool InteractCheck0Detour(nint a1, nint a2, nint a3, nint a4, bool a5)
	{
		return true;
	}

	private static bool IsPlayerOnJumpingDetour(nint a1)
	{
		return false;
	}

	private unsafe static bool CheckTargetPositionDetour(EventFramework* framework, GameObject* source, GameObject* target, ushort interactType, bool sendError, byte a6)
	{
		return true;
	}

	private unsafe static float CheckTargetDistanceDetour(GameObject* localPlayer, GameObject* target)
	{
		return 0f;
	}

	private unsafe ulong InteractWithObjectDetour(TargetSystem* system, GameObject* obj, bool checkLOS)
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Invalid comparison between Unknown and I4
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		IPlayerCharacter localPlayer;
		if (obj != null)
		{
			localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
			if (localPlayer != null)
			{
				MemoryHelper.Write<bool>((IntPtr)((nint)DalamudServiceExtension.Instance<ICondition>().Address + 27), false);
				((ActionManager)ActionManager.Instance()).AnimationLock = 0f;
				if ((int)((GameObject)obj).ObjectKind == 5 && (*EventFramework.Instance()).TryGetNearestEventID((EventHandlerInfo x) => (int)x.EventId.ContentId == 5, (GameObject x) => ((GameObject)(ref x)).NameString == ((GameObject)obj).NameString, localPlayer.Position, out var eventID) && FirstQuest.Any((uint x) => ((UIState)UIState.Instance()).IsUnlockLinkUnlockedOrQuestCompleted(x, (byte)0, true)))
				{
					DismountAndSend(EventId.op_Implicit(eventID));
					return 1uL;
				}
				if (WhitelistObjectNames.ContainsAny<string>(new string[1] { ((GameObject)obj).NameString }))
				{
					if (((GameObject)obj).EventHandler != null)
					{
						EventHandlerInfo info = ((EventHandler)((GameObject)obj).EventHandler).Info;
						DismountAndSend(info.EventId.Id);
						return 1uL;
					}
					if ((*EventFramework.Instance()).TryGetNearestEventID((EventHandlerInfo _) => true, (GameObject x) => ((GameObject)(ref x)).NameString == ((GameObject)obj).NameString, localPlayer.Position, out var eventID2))
					{
						DismountAndSend(EventId.op_Implicit(eventID2));
						return 1uL;
					}
				}
				return InteractWithObjectHook.Original(system, obj, checkLOS: false);
			}
		}
		return 0uL;
		void DismountAndSend(uint eventID3)
		{
			OmenServiceBase<MovementManager>.Instance().Dismount();
			base.TaskHelper.Enqueue(delegate
			{
				if (OmenServiceBase<MovementManager>.Instance().IsManagerBusy || DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4] || DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)48])
				{
					return false;
				}
				new EventStartPackt(localPlayer.GameObjectID, eventID3).Send();
				return true;
			});
		}
	}

	private unsafe static bool CheckCameraPositionDetour(TargetSystem* system, Camera* activeCamera, GameObject* obj)
	{
		return true;
	}

	static OptimizedInteraction()
	{
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		InlineArray2<string> buffer = default(InlineArray2<string>);
		ref string reference = ref buffer[0];
		EObjName rowOrDefault = LuminaGetter.GetRowOrDefault<EObjName>(2000073u);
		reference = ((object)((EObjName)(ref rowOrDefault)).Singular/*cast due to constrained. prefix*/).ToString();
		ref string reference2 = ref buffer[1];
		rowOrDefault = LuminaGetter.GetRowOrDefault<EObjName>(2003395u);
		reference2 = ((object)((EObjName)(ref rowOrDefault)).Singular/*cast due to constrained. prefix*/).ToString();
		WhitelistObjectNames = FrozenSet.Create<string>(buffer);
		FirstQuest = FrozenSet.Create<uint>(65621u, 65644u, 65645u, 65659u, 65660u, 66104u, 66105u, 66106u);
	}
}
