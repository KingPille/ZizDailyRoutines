using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Linq;
using System.Numerics;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Hooking;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.Classes;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Info.Game.Packets.Upstream;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.ModulesPublic;

public class AutoTenChiJin : ModuleBase
{
	private unsafe delegate byte IsSlotUsableDelegate(HotbarSlot* slot, HotbarSlotType type, uint id);

	private sealed class AddonDRNinJutsuActionsPreview : NativeAddon
	{
		protected unsafe override void OnSetup(AtkUnitBase* addon, Span<AtkValue> atkValues)
		{
			HorizontalFlexNode horizontalFlexNode = new HorizontalFlexNode
			{
				Width = 60 * NormalSequence.Count,
				AlignmentFlags = (FlexFlags.FitContentHeight | FlexFlags.CenterVertically | FlexFlags.CenterHorizontally),
				Position = new Vector2(20f, 40f),
				IsVisible = true
			};
			ImmutableArray<uint>.Enumerator enumerator = NormalSequence.Keys.GetEnumerator();
			while (enumerator.MoveNext())
			{
				uint current = enumerator.Current;
				if (LuminaGetter.TryGetRow<Action>(current, out Action item))
				{
					DragDropNode node = new DragDropNode
					{
						Size = new Vector2(44f),
						IsVisible = true,
						IconId = ((Action)(ref item)).Icon,
						AcceptedType = (DragDropType)2,
						IsDraggable = true,
						Payload = new DragDropPayload
						{
							Type = (DragDropType)7,
							Int2 = (int)current
						},
						IsClickable = false,
						ActionTooltip = current
					};
					horizontalFlexNode.AddNode(node);
					horizontalFlexNode.AddDummy();
				}
			}
			horizontalFlexNode.AttachNode(this);
		}
	}

	private sealed class Config : ModuleConfig
	{
		public TenChiJinMode Mode;

		public bool AutoCastNinJiTsu = true;

		public bool SendNotification = true;
	}

	private enum TenChiJinMode
	{
		Auto,
		Manual
	}

	private static readonly CompSig IsSlotUsableSig = new CompSig("48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC ?? 0F B6 F2 48 8B D9 41 8B F8");

	private Hook<IsSlotUsableDelegate>? IsSlotUsableHook;

	private Config cofig;

	private AddonDRNinJutsuActionsPreview? addon;

	private readonly HashSet<uint> usedMudraActions = new HashSet<uint>();

	private static readonly FrozenDictionary<uint, uint[]> NormalSequence = new Dictionary<uint, uint[]>
	{
		[2265u] = new uint[1] { 2259u },
		[2266u] = new uint[2] { 2261u, 18805u },
		[2267u] = new uint[2] { 2259u, 18806u },
		[2268u] = new uint[2] { 2259u, 18807u },
		[2269u] = new uint[3] { 2263u, 18806u, 18805u },
		[2270u] = new uint[3] { 2259u, 18807u, 18806u },
		[2271u] = new uint[3] { 2259u, 18806u, 18807u }
	}.ToFrozenDictionary();

	private static readonly FrozenDictionary<uint, uint> Kassatsu = new Dictionary<uint, uint>
	{
		[2266u] = 16491u,
		[2268u] = 16492u
	}.ToFrozenDictionary();

	private static readonly FrozenDictionary<uint, uint[]> TenChiJinSequence = new Dictionary<uint, uint[]>
	{
		[2269u] = new uint[3] { 18875u, 18877u, 18879u },
		[2270u] = new uint[3] { 18873u, 18878u, 18880u },
		[2271u] = new uint[3] { 18873u, 18877u, 18881u }
	}.ToFrozenDictionary();

	private static readonly FrozenSet<uint> NinjutsuActions = FrozenSet.Create<uint>(2265u, 2266u, 2267u, 2268u, 2269u, 2270u, 2271u, 16491u, 16492u);

	private static readonly FrozenSet<uint> MudraStartActions = FrozenSet.Create<uint>(2259u, 2261u, 2263u);

	private static readonly FrozenSet<uint> MudraProcessActions = FrozenSet.Create<uint>(18805u, 18806u, 18807u);

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoTenChiJinTitle"),
		Description = LanguageManager.Get("AutoTenChiJinDescription"),
		Category = ModuleCategory.Action
	};

	protected unsafe override void Init()
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		cofig = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 2000
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		if (addon == null)
		{
			addon = new AddonDRNinJutsuActionsPreview
			{
				InternalName = "DRNinJutsuActionsPreview",
				Title = ReadOnlySeString.op_Implicit(LuminaWrapper.GetActionName(2260u)),
				Size = new Vector2(430f, 110f)
			};
		}
		OmenServiceBase<UseActionManager>.Instance().RegPreUseAction(OnPreUseAction);
		OmenServiceBase<GamePacketManager>.Instance().RegPreSendPacket(OnPreSendActionPacket);
		if (IsSlotUsableHook == null)
		{
			IsSlotUsableHook = IsSlotUsableSig.GetHook<IsSlotUsableDelegate>(IsSlotUsableDetour);
		}
		IsSlotUsableHook.Enable();
	}

	protected override void Uninit()
	{
		ResetRuntimeState();
		OmenServiceBase<UseActionManager>.Instance().Unreg(OnPreUseAction);
		OmenServiceBase<GamePacketManager>.Instance().Unreg(OnPreSendActionPacket);
		addon?.Dispose();
		addon = null;
	}

	protected override void ConfigUI()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(1, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("Mode"));
		((ImU8String)(ref val)).AppendLiteral(":");
		ImGui.TextColored(ref vector, val);
		IndentDisposable val2 = ImRaii.PushIndent(1, true);
		try
		{
			DrawModeOption(TenChiJinMode.Auto, LanguageManager.Get("AutoMode"));
			ImGui.SameLine();
			DrawModeOption(TenChiJinMode.Manual, LanguageManager.Get("ManualMode"));
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
		if (cofig.Mode == TenChiJinMode.Auto)
		{
			ImGui.NewLine();
			if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("AutoTenChiJin-OpenNijutsuActionsAddon")), default(Vector2)))
			{
				addon.Toggle();
			}
			ImGui.NewLine();
			if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("SendNotification")), ref cofig.SendNotification))
			{
				cofig.Save(this);
			}
			if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("AutoTenChiJin-AutoCastNinJiTsu")), ref cofig.AutoCastNinJiTsu))
			{
				cofig.Save(this);
			}
		}
	}

	private void DrawModeOption(TenChiJinMode mode, string label)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (ImGui.RadioButton(ImU8String.op_Implicit(label), cofig.Mode == mode) && cofig.Mode != mode)
		{
			cofig.Mode = mode;
			ResetRuntimeState();
			cofig.Save(this);
		}
	}

	private unsafe byte IsSlotUsableDetour(HotbarSlot* slot, HotbarSlotType type, uint id)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Invalid comparison between Unknown and I4
		if (cofig.Mode != TenChiJinMode.Auto || (int)type != 1 || !NinjutsuActions.Contains(id))
		{
			return IsSlotUsableHook.Original(slot, type, id);
		}
		BattleChara* localPlayer = Control.GetLocalPlayer();
		if (localPlayer == null || ((BattleChara)localPlayer).ClassJob != 30)
		{
			return 0;
		}
		if (((StatusManager)(&((BattleChara)localPlayer).StatusManager)).HasStatus(1186u, 3758096384u))
		{
			return TenChiJinSequence.ContainsKey(id) ? ((byte)1) : ((byte)0);
		}
		ActionManager* ptr = ActionManager.Instance();
		uint currentCharges = ((ActionManager)ptr).GetCurrentCharges(2261u);
		float num = 21f - ((ActionManager)ptr).GetRecastTimeElapsed((ActionType)1, 2261u);
		((HotbarSlot)slot).CostType = 3;
		((HotbarSlot)slot).CostDisplayMode = 1;
		((HotbarSlot)slot).CostValue = (uint)((currentCharges != 0) ? ((float)currentCharges) : ((num <= 1f) ? 1f : num));
		return (byte)((currentCharges != 0 || ((StatusManager)(&((BattleChara)localPlayer).StatusManager)).HasStatus(497u, 3758096384u)) ? 1u : 0u);
	}

	private unsafe void OnPreUseAction(ref bool isPrevented, ref ActionType actionType, ref uint actionID, ref ulong targetID, ref uint extraParam, ref UseActionMode queueState, ref uint comboRouteID)
	{
		if (cofig.Mode != TenChiJinMode.Auto)
		{
			return;
		}
		BattleChara* localPlayer = Control.GetLocalPlayer();
		if (localPlayer == null || ((BattleChara)localPlayer).ClassJob != 30 || (int)actionType != 1 || !NinjutsuActions.Contains(actionID))
		{
			return;
		}
		if (base.TaskHelper.IsBusy)
		{
			if (cofig.AutoCastNinJiTsu)
			{
				isPrevented = true;
			}
			return;
		}
		ActionManager* ptr = ActionManager.Instance();
		if (ptr == null)
		{
			return;
		}
		uint actionStatus = ((ActionManager)ptr).GetActionStatus(actionType, actionID, 3758096384uL, true, true, (uint*)null);
		if (((StatusManager)(&((BattleChara)localPlayer).StatusManager)).HasStatus(1186u, 3758096384u))
		{
			if ((actionStatus == 579 || actionStatus == 582) ? true : false)
			{
				EnqueueAutomaticSequence(actionID);
			}
			return;
		}
		uint currentCharges = ((ActionManager)ptr).GetCurrentCharges(2261u);
		bool flag = ((ActionManager)ptr).IsActionOffCooldown((ActionType)1, 2261u);
		bool flag2 = ((StatusManager)(&((BattleChara)localPlayer).StatusManager)).HasStatus(497u, 3758096384u);
		if ((currentCharges != 0 || flag || flag2) && ((actionStatus == 572 || actionStatus == 582) ? true : false))
		{
			EnqueueAutomaticSequence(actionID);
		}
	}

	private unsafe void OnPreSendActionPacket(ref bool isPrevented, int opcode, ref nint packet, ref bool isPrioritize)
	{
		if (cofig.Mode != TenChiJinMode.Manual || opcode != UpstreamOpcode.UseActionOpcode || LocalPlayerState.ClassJob != 30)
		{
			return;
		}
		UseActionPacket* ptr = (UseActionPacket*)packet;
		if (MudraStartActions.Contains(ptr->ActionID))
		{
			usedMudraActions.Clear();
			usedMudraActions.Add(ptr->ActionID % 2259 / 2 + 18805);
		}
		else if (MudraProcessActions.Contains(ptr->ActionID))
		{
			if (!usedMudraActions.Add(ptr->ActionID))
			{
				isPrevented = true;
			}
		}
		else if (NinjutsuActions.Contains(ptr->ActionID))
		{
			usedMudraActions.Clear();
		}
	}

	private unsafe void EnqueueAutomaticSequence(uint actionID)
	{
		BattleChara* localPlayer = Control.GetLocalPlayer();
		if (localPlayer == null)
		{
			return;
		}
		if (((StatusManager)(&((BattleChara)localPlayer).StatusManager)).HasStatus(1186u, 3758096384u))
		{
			if (!TenChiJinSequence.TryGetValue(actionID, out uint[] value))
			{
				return;
			}
			NotifyNinjutsu(actionID);
			base.TaskHelper.Abort();
			uint[] array = value;
			foreach (uint ninjutsu in array)
			{
				base.TaskHelper.Enqueue(delegate
				{
					IGameObject target = TargetManager.Target;
					if (target == null)
					{
						return false;
					}
					if (((ActionManager)ActionManager.Instance()).GetActionStatus((ActionType)1, ninjutsu, 3758096384uL, true, true, (uint*)null) != 0)
					{
						return false;
					}
					OmenServiceBase<UseActionManager>.Instance().UseActionLocation((ActionType)1, ninjutsu, target.EntityID, default(Vector3), 0u, 0);
					return true;
				});
			}
		}
		else
		{
			if (!NormalSequence.TryGetValue(actionID, out uint[] value2) || !LuminaGetter.TryGetRow<Action>(actionID, out Action row))
			{
				return;
			}
			uint kassatsuActionID;
			bool hasKassatsuReplacement = Kassatsu.TryGetValue(actionID, out kassatsuActionID);
			bool isKassatsuActive = ((StatusManager)(&((BattleChara)localPlayer).StatusManager)).HasStatus(497u, 3758096384u);
			NotifyNinjutsu(actionID);
			base.TaskHelper.Abort();
			foreach (var item in value2.Select((uint item, int index) => (value: item, index: index)))
			{
				var (mudraActionID, _) = item;
				if (item.index == 0)
				{
					uint num = mudraActionID % 2259 / 2 + 18805;
					uint actualMudraActionID = (isKassatsuActive ? num : mudraActionID);
					base.TaskHelper.Enqueue(() => ((ActionManager)ActionManager.Instance()).GetActionStatus((ActionType)1, actualMudraActionID, 3758096384uL, true, true, (uint*)null) == 0);
					base.TaskHelper.Enqueue(() => SendMudraAction(localPlayer, actualMudraActionID));
				}
				else
				{
					base.TaskHelper.Enqueue(() => SendMudraAction(localPlayer, mudraActionID));
				}
				base.TaskHelper.DelayNext(300);
			}
			if (!cofig.AutoCastNinJiTsu)
			{
				return;
			}
			base.TaskHelper.Enqueue(delegate
			{
				uint num2 = ((!isKassatsuActive || !hasKassatsuReplacement) ? actionID : kassatsuActionID);
				return ((ActionManager)ActionManager.Instance()).GetAdjustedActionId(2260u) == num2;
			});
			base.TaskHelper.Enqueue(delegate
			{
				uint actionID2 = ((!isKassatsuActive || !hasKassatsuReplacement) ? actionID : kassatsuActionID);
				UseActionManager useActionManager = OmenServiceBase<UseActionManager>.Instance();
				uint num2;
				if (((Action)(ref row)).CanTargetHostile)
				{
					IGameObject target = TargetManager.Target;
					if (target != null)
					{
						num2 = target.EntityID;
						goto IL_004f;
					}
				}
				num2 = ((BattleChara)localPlayer).EntityId;
				goto IL_004f;
				IL_004f:
				return useActionManager.UseActionLocation((ActionType)1, actionID2, num2, default(Vector3), 0u, 0);
			});
		}
	}

	private void NotifyNinjutsu(uint actionID)
	{
		if (cofig.SendNotification)
		{
			OmenServiceBase<NotifyHelper>.Instance().NotificationInfo(LanguageManager.Get("AutoTenChiJin-Notification", LuminaWrapper.GetActionName(actionID)));
		}
	}

	private unsafe static bool SendMudraAction(BattleChara* localPlayer, uint actionID)
	{
		new UseActionPacket((ActionType)1, actionID, ((BattleChara)localPlayer).EntityId, ((BattleChara)localPlayer).Rotation).Send();
		((ActionManager)ActionManager.Instance()).StartCooldown((ActionType)1, actionID);
		return true;
	}

	private void ResetRuntimeState()
	{
		usedMudraActions.Clear();
		base.TaskHelper?.Abort();
	}
}
