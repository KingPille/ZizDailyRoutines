using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.DutyState;
using Dalamud.Game.Gui.Toast;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.Control.MoveControl;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Common.Math;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using Newtonsoft.Json;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.ImGuiOm.Widgets.Combos;
using OmenTools.Info.Lumina;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class HealerHelper : ModuleBase
{
	private class AutoPlayCardManager(HealerHelper module, AutoPlayCardManager.Storage config)
	{
		public class Storage
		{
			public readonly PlayCardOrder CustomCardOrder = new PlayCardOrder();

			public AutoPlayCardStatus AutoPlayCard = AutoPlayCardStatus.Default;

			public PlayCardOrder DefaultCardOrder = new PlayCardOrder();
		}

		public class PlayCardOrder
		{
			[JsonProperty("melee")]
			public Dictionary<string, string[]> Melee { get; private set; } = new Dictionary<string, string[]>();

			[JsonProperty("range")]
			public Dictionary<string, string[]> Range { get; private set; } = new Dictionary<string, string[]>();
		}

		public enum AutoPlayCardStatus
		{
			Disable,
			Default,
			Custom
		}

		public enum DutySection
		{
			Enter,
			Start
		}

		private readonly List<(uint id, double priority)> meleeCandidateOrder = new List<(uint, double)>();

		private readonly List<(uint id, double priority)> rangeCandidateOrder = new List<(uint, double)>();

		public static readonly FrozenSet<uint> PlayCardActions = FrozenSet.Create<uint>(37023u, 37026u);

		public DutySection CurrentDutySection { get; set; }

		public DateTime StartTimeUTC { get; set; }

		public bool DefaultCardOrderLoaded
		{
			get
			{
				if (config.DefaultCardOrder.Melee.Count > 0)
				{
					return config.DefaultCardOrder.Range.Count > 0;
				}
				return false;
			}
		}

		public bool CustomCardOrderLoaded
		{
			get
			{
				if (config.CustomCardOrder.Melee.Count > 0)
				{
					return config.CustomCardOrder.Range.Count > 0;
				}
				return false;
			}
		}

		public bool IsOpener => (OmenServiceBase<StandardTimeManager>.Instance().UTCNow - StartTimeUTC).TotalSeconds > 90.0;

		public void InitDefaultCardOrder(PlayCardOrder order)
		{
			config.DefaultCardOrder = order;
			OrderCandidates();
		}

		public void InitCustomCardOrder(string role = "All", string section = "All")
		{
			if ((role == "Melee" || role == "All") ? true : false)
			{
				if ((section == "opener" || section == "All") ? true : false)
				{
					config.CustomCardOrder.Melee["opener"] = config.DefaultCardOrder.Melee["opener"].ToArray();
				}
				if ((section == "2m+" || section == "All") ? true : false)
				{
					config.CustomCardOrder.Melee["2m+"] = config.DefaultCardOrder.Melee["2m+"].ToArray();
				}
			}
			if ((role == "Range" || role == "All") ? true : false)
			{
				if ((section == "opener" || section == "All") ? true : false)
				{
					config.CustomCardOrder.Range["opener"] = config.DefaultCardOrder.Range["opener"].ToArray();
				}
				if ((section == "2m+" || section == "All") ? true : false)
				{
					config.CustomCardOrder.Range["2m+"] = config.DefaultCardOrder.Range["2m+"].ToArray();
				}
			}
			OrderCandidates();
		}

		public unsafe void OrderCandidates()
		{
			meleeCandidateOrder.Clear();
			rangeCandidateOrder.Clear();
			HudPartyMember[] array = ((AgentHUD)AgentHUD.Instance()).PartyMembers.ToArray();
			bool flag = LocalPlayerState.ClassJob == 33;
			if (!GameState.IsInPVPArea && array.Length >= 2 && flag && config.AutoPlayCard != AutoPlayCardStatus.Disable)
			{
				string key = (IsOpener ? "opener" : "2m+");
				PlayCardOrder playCardOrder = ((config.AutoPlayCard == AutoPlayCardStatus.Custom) ? config.CustomCardOrder : config.DefaultCardOrder);
				ProcessRoleCandidates(array, playCardOrder.Melee[key], meleeCandidateOrder, 3);
				ProcessRoleCandidates(array, playCardOrder.Range[key], rangeCandidateOrder, 2);
				meleeCandidateOrder.Sort(((uint id, double priority) a, (uint id, double priority) b) => b.priority.CompareTo(a.priority));
				rangeCandidateOrder.Sort(((uint id, double priority) a, (uint id, double priority) b) => b.priority.CompareTo(a.priority));
			}
		}

		private unsafe static void ProcessRoleCandidates(HudPartyMember[] partyList, string[] order, List<(uint id, double priority)> candidates, int fallbackRole)
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			int idx;
			for (idx = 0; idx < order.Length; idx++)
			{
				HudPartyMember member = partyList.FirstOrDefault(delegate(HudPartyMember m)
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_000a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0015: Unknown result type (might be due to invalid IL or missing references)
					//IL_001a: Unknown result type (might be due to invalid IL or missing references)
					//IL_001d: Unknown result type (might be due to invalid IL or missing references)
					//IL_0022: Unknown result type (might be due to invalid IL or missing references)
					//IL_0025: Unknown result type (might be due to invalid IL or missing references)
					//IL_003c: Unknown result type (might be due to invalid IL or missing references)
					if (m.Object != null)
					{
						ClassJob value = ((BattleChara)m.Object).ClassJob.ToLuminaRowRef<ClassJob>().Value;
						return ((ClassJob)(ref value)).NameEnglish == ReadOnlySeString.op_Implicit(order[idx]);
					}
					return false;
				});
				if (member.EntityId != 0 && candidates.All(((uint id, double priority) m) => m.id != member.EntityId))
				{
					candidates.Add((member.EntityId, 5.0 - (double)idx * 0.1));
				}
			}
			if (candidates.Count != 0)
			{
				return;
			}
			HudPartyMember val = partyList.FirstOrDefault(delegate(HudPartyMember m)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				if (m.Object != null)
				{
					ClassJob value = ((BattleChara)m.Object).ClassJob.ToLuminaRowRef<ClassJob>().Value;
					return ((ClassJob)(ref value)).Role == fallbackRole;
				}
				return false;
			});
			if (val.EntityId != 0)
			{
				candidates.Add((val.EntityId, 1.0));
			}
		}

		private unsafe BattleChara* FetchCandidateObject(string role)
		{
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			List<(uint id, double priority)> obj = ((role == "Melee") ? meleeCandidateOrder : rangeCandidateOrder);
			BattleChara* result = null;
			double num = 0.0;
			float num2 = MathF.Pow(ActionManager.GetActionRange(37023u), 2f);
			foreach (var member in obj)
			{
				HudPartyMember val = ((AgentHUD)AgentHUD.Instance()).PartyMembers.ToArray().FirstOrDefault((HudPartyMember m) => m.EntityId == member.id);
				BattleChara* ptr = val.Object;
				if (val.EntityId != 0 && ptr != null && !((BattleChara)ptr).IsDead() && ((BattleChara)ptr).Health != 0 && !(Vector3.DistanceSquared(LocalPlayerState.Object.Position, Vector3.op_Implicit(((BattleChara)ptr).Position)) >= num2))
				{
					if (((BattleChara)ptr).IsMounted() || (int)((MoveController)(&((BattleChara)ptr).MoveController)).MovementState != 0 || ((StatusManager)(&((BattleChara)ptr).StatusManager)).HasStatus(43u, 3758096384u) || ((StatusManager)(&((BattleChara)ptr).StatusManager)).HasStatus(44u, 3758096384u))
					{
						result = val.Object;
						num = member.priority;
					}
					else if (member.priority >= num - 2.0)
					{
						return val.Object;
					}
				}
			}
			return result;
		}

		public unsafe void OnPrePlayCard(ref ulong targetID, ref uint actionID)
		{
			if (targetID == 3758096384u)
			{
				BattleChara* ptr = ((actionID == 37023) ? FetchCandidateObject("Melee") : FetchCandidateObject("Range"));
				if (ptr != null && ((BattleChara)ptr).EntityId != targetID)
				{
					targetID = ((BattleChara)ptr).EntityId;
					module.NotifyTargetChange(IBattleChara.Create((nint)ptr), actionID);
				}
			}
		}
	}

	private class EasyHealManager(HealerHelper module, EasyHealManager.Storage config)
	{
		public class Storage
		{
			public HashSet<uint> ActiveHealActions = new HashSet<uint>();

			public DispelOrderStatus DispelOrder;

			public EasyDispelStatus EasyDispel = EasyDispelStatus.Enable;

			public EasyHealStatus EasyHeal = EasyHealStatus.Enable;

			public EasyRaiseStatus EasyRaise = EasyRaiseStatus.Enable;

			public float NeedHealThreshold = 0.92f;

			public OverhealTarget OverhealTarget;

			public RaiseOrderStatus RaiseOrder;

			public Dictionary<uint, HealAction> TargetHealActions = new Dictionary<uint, HealAction>();
		}

		public class HealAction
		{
			[JsonProperty("id")]
			public uint ID;

			[JsonProperty("name")]
			public string Name;

			[JsonProperty("on")]
			public bool On;
		}

		public enum DispelOrderStatus
		{
			Order,
			Reverse
		}

		public enum EasyDispelStatus
		{
			Disable,
			Enable
		}

		public enum EasyHealStatus
		{
			Disable,
			Enable
		}

		public enum EasyRaiseStatus
		{
			Disable,
			Enable
		}

		public enum OverhealTarget
		{
			Local,
			FirstTank,
			Prevent
		}

		public enum RaiseOrderStatus
		{
			Order,
			Reverse
		}

		public static readonly FrozenSet<uint> RaiseActions = FrozenSet.Create<uint>(125u, 173u, 3603u, 24287u, 7670u, 7523u, 64556u);

		public bool TargetHealActionsLoaded => config.TargetHealActions.Count > 0;

		public bool ActiveHealActionsLoaded => config.ActiveHealActions.Count > 0;

		public Dictionary<uint, HealAction> TargetHealActions => config.TargetHealActions;

		public void InitTargetHealActions(Dictionary<uint, HealAction> actions)
		{
			config.TargetHealActions = actions;
		}

		public void InitActiveHealActions()
		{
			config.ActiveHealActions = (from act in config.TargetHealActions
				where act.Value.On
				select act.Key).ToHashSet();
		}

		private unsafe BattleChara* TargetNeedHealObject(uint actionID)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			float num = 2f;
			BattleChara* result = null;
			Span<HudPartyMember> partyMembers = ((AgentHUD)AgentHUD.Instance()).PartyMembers;
			for (int i = 0; i < partyMembers.Length; i++)
			{
				HudPartyMember val = partyMembers[i];
				if (val.EntityId == 0 || val.Object == null)
				{
					continue;
				}
				BattleChara* ptr = val.Object;
				if (!((BattleChara)ptr).IsDead() && ((BattleChara)ptr).Health != 0 && ActionManager.GetActionInRangeOrLoS(actionID, (GameObject*)Control.GetLocalPlayer(), (GameObject*)val.Object) == 0)
				{
					float num2 = (float)((BattleChara)ptr).Health / (float)((BattleChara)ptr).MaxHealth;
					if (num2 < num && num2 <= config.NeedHealThreshold)
					{
						num = num2;
						result = val.Object;
					}
				}
			}
			return result;
		}

		private unsafe static BattleChara* FindTarget(uint actionID, Func<HudPartyMember, bool> predicate, bool reverse = false)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			HudPartyMember[] array = ((AgentHUD)AgentHUD.Instance()).PartyMembers.ToArray();
			IEnumerable<HudPartyMember> enumerable2;
			if (!reverse)
			{
				IEnumerable<HudPartyMember> enumerable = array;
				enumerable2 = enumerable;
			}
			else
			{
				enumerable2 = array.Reverse();
			}
			foreach (HudPartyMember item in enumerable2)
			{
				if (item.EntityId != 0 && item.Object != null && ActionManager.GetActionInRangeOrLoS(actionID, (GameObject*)Control.GetLocalPlayer(), (GameObject*)item.Object) == 0 && predicate(item))
				{
					return item.Object;
				}
			}
			return null;
		}

		public unsafe void OnPreHeal(ref ulong targetID, ref uint actionID, ref bool isPrevented)
		{
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			if (targetID != 3758096384u && IsHealable(IOmenDalamudService<IObjectTable>.Instance().SearchByID(targetID)))
			{
				return;
			}
			BattleChara* ptr = TargetNeedHealObject(actionID);
			if (ptr != null && ((BattleChara)ptr).EntityId != targetID)
			{
				targetID = ((BattleChara)ptr).EntityId;
				module.NotifyTargetChange(IBattleChara.Create((nint)ptr), actionID);
				return;
			}
			switch (config.OverhealTarget)
			{
			case OverhealTarget.Prevent:
				isPrevented = true;
				break;
			case OverhealTarget.Local:
				targetID = LocalPlayerState.EntityID;
				module.NotifyTargetChange(LocalPlayerState.Object, actionID);
				break;
			case OverhealTarget.FirstTank:
			{
				foreach (HudPartyMember item in (from x in ((AgentHUD)AgentHUD.Instance()).PartyMembers.ToArray()
					where x.Object != null
					select x).OrderByDescending(delegate(HudPartyMember x)
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_000b: Unknown result type (might be due to invalid IL or missing references)
					//IL_0010: Unknown result type (might be due to invalid IL or missing references)
					//IL_0013: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					ClassJob value = ((BattleChara)x.Object).ClassJob.ToLuminaRowRef<ClassJob>().Value;
					return ((ClassJob)(ref value)).Role == 1;
				}).ToList())
				{
					if (item.Object != null && ActionManager.GetActionInRangeOrLoS(actionID, (GameObject*)Control.GetLocalPlayer(), (GameObject*)item.Object) == 0)
					{
						targetID = item.EntityId;
						module.NotifyTargetChange(IBattleChara.Create((nint)item.Object), actionID);
						break;
					}
				}
				break;
			}
			}
		}

		public unsafe void OnPreDispel(ref ulong targetID)
		{
			if (targetID != 3758096384u)
			{
				return;
			}
			if (LocalPlayerState.Object.StatusList.Any((IStatus s) => Sheets.DispellableStatuses.ContainsKey(s.StatusID)))
			{
				targetID = LocalPlayerState.EntityID;
				module.NotifyTargetChange(LocalPlayerState.Object, 7568u);
				return;
			}
			BattleChara* ptr = FindTarget(7568u, delegate(HudPartyMember m)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				//IL_003a: Unknown result type (might be due to invalid IL or missing references)
				//IL_003f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				if (((BattleChara)m.Object).IsDead() || ((BattleChara)m.Object).Health == 0)
				{
					return false;
				}
				Span<Status> status = ((StatusManager)(&((BattleChara)m.Object).StatusManager)).Status;
				for (int i = 0; i < status.Length; i++)
				{
					Status val = status[i];
					if (Sheets.DispellableStatuses.ContainsKey(val.StatusId))
					{
						return true;
					}
				}
				return false;
			}, config.DispelOrder == DispelOrderStatus.Reverse);
			if (ptr != null)
			{
				targetID = ((BattleChara)ptr).EntityId;
				module.NotifyTargetChange(IBattleChara.Create((nint)ptr), 7568u);
			}
		}

		public unsafe void OnPreRaise(ref ulong targetID, ref uint actionID)
		{
			if (targetID == 3758096384u)
			{
				BattleChara* ptr = FindTarget(actionID, delegate(HudPartyMember m)
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					BattleChara* ptr2 = m.Object;
					return (((BattleChara)ptr2).IsDead() || ((BattleChara)ptr2).Health == 0) && !((StatusManager)(&((BattleChara)ptr2).StatusManager)).HasStatus(148u, 3758096384u);
				}, config.RaiseOrder == RaiseOrderStatus.Reverse);
				if (ptr != null && ((BattleChara)ptr).EntityId != targetID)
				{
					targetID = ((BattleChara)ptr).EntityId;
					module.NotifyTargetChange(IBattleChara.Create((nint)ptr), actionID);
				}
			}
		}

		public unsafe static bool IsHealable(IGameObject? gameObject)
		{
			return ActionManager.CanUseActionOnTarget(3595u, gameObject.ToStruct());
		}
	}

	private class Config : ModuleConfig
	{
		public AutoPlayCardManager.Storage AutoPlayCardStorage = new AutoPlayCardManager.Storage();

		public EasyHealManager.Storage EasyHealStorage = new EasyHealManager.Storage();

		public bool SendChat;

		public bool SendNotification = true;
	}

	private Config config;

	private EasyHealManager easyHealService;

	private AutoPlayCardManager autoPlayCardService;

	private ActionSelectCombo? actionSelect;

	private static int? CustomCardOrderDragIndex;

	private const string REMOTE_URI = "hxxps://assets.sumemo.dev";

	private const uint UNSPECIFIC_TARGET_ID = 3758096384u;

	private const uint DISPELL_ACTION_ID = 7568u;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("HealerHelperTitle"),
		Description = LanguageManager.Get("HealerHelperDescription"),
		Category = ModuleCategory.Action,
		Author = new string[1] { "HaKu" }
	};

	private static FrozenDictionary<ReadOnlySeString, ReadOnlySeString> JobNameMap { get; } = ((IEnumerable<ClassJob>)LuminaGetter.Get<ClassJob>()).DistinctBy((ClassJob x) => ((ClassJob)(ref x)).NameEnglish).ToFrozenDictionary((ClassJob s) => ((ClassJob)(ref s)).NameEnglish, (ClassJob s) => ((ClassJob)(ref s)).Name);

	protected override void Init()
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Expected O, but got Unknown
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Expected O, but got Unknown
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		easyHealService = new EasyHealManager(this, config.EasyHealStorage);
		autoPlayCardService = new AutoPlayCardManager(this, config.AutoPlayCardStorage);
		Task.Run(async delegate
		{
			await FetchAll();
		});
		OmenServiceBase<UseActionManager>.Instance().RegPreUseActionLocation(OnPreUseAction);
		DalamudServiceExtension.Instance<IDutyState>().DutyRecommenced += new DutyRecommencedDelegate(OnDutyRecommenced);
		DalamudServiceExtension.Instance<IDutyState>().DutyStarted += new DutyStartedDelegate(OnDutyStarted);
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += OnZoneChanged;
		DalamudServiceExtension.Instance<ICondition>().ConditionChange += new ConditionChangeDelegate(OnConditionChanged);
		if (GameState.ContentFinderCondition != 0 && DalamudServiceExtension.Instance<IDutyState>().IsDutyStarted)
		{
			OnDutyStarted(null);
		}
	}

	protected override void Uninit()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected O, but got Unknown
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Expected O, but got Unknown
		OmenServiceBase<UseActionManager>.Instance().Unreg(OnPreUseAction);
		DalamudServiceExtension.Instance<IDutyState>().DutyRecommenced -= new DutyRecommencedDelegate(OnDutyRecommenced);
		DalamudServiceExtension.Instance<IDutyState>().DutyStarted -= new DutyStartedDelegate(OnDutyStarted);
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChanged;
		DalamudServiceExtension.Instance<ICondition>().ConditionChange -= new ConditionChangeDelegate(OnConditionChanged);
	}

	private unsafe void NotifyTargetChange(IBattleChara gameObject, uint actionID)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Expected O, but got Unknown
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Expected O, but got Unknown
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Expected O, but got Unknown
		ClassJob job = gameObject.ClassJob.Value;
		RentedSeStringBuilder val = default(RentedSeStringBuilder);
		((RentedSeStringBuilder)(ref val))._002Ector();
		try
		{
			((RentedSeStringBuilder)(ref val)).Builder.PushColorType(32u).Append(LuminaWrapper.GetActionName(actionID)).PopColorType();
			if (config.SendChat)
			{
				OmenServiceBase<NotifyHelper>.Instance().Chat(LanguageManager.GetSe("HealerHelper-Message-TargetRedirected", val, (object)new PlayerPayload(gameObject.Name, gameObject.HomeWorld.RowId), job.ToBitmapFontIcon(), ((ClassJob)(ref job)).Name));
			}
			if (config.SendNotification)
			{
				NotifyHelper.ToastQuest(LanguageManager.GetSe("HealerHelper-Notification-TargetRedirected", (object)new PlayerPayload(gameObject.Name, gameObject.HomeWorld.RowId), job.ToBitmapFontIcon(), ((ClassJob)(ref job)).Name), new QuestToastOptions
				{
					IconId = LuminaWrapper.GetActionIconID(actionID)
				});
			}
		}
		finally
		{
			((IDisposable)(*(RentedSeStringBuilder*)(&val))/*cast due to constrained. prefix*/).Dispose();
		}
	}

	private async Task FetchPlayCardOrder()
	{
		if (autoPlayCardService.DefaultCardOrderLoaded)
		{
			return;
		}
		try
		{
			AutoPlayCardManager.PlayCardOrder playCardOrder = JsonConvert.DeserializeObject<AutoPlayCardManager.PlayCardOrder>(await OmenServiceBase<HTTPClientHelper>.Instance().Get().GetStringAsync("https://assets.sumemo.dev/card-order.json"));
			if (playCardOrder != null)
			{
				autoPlayCardService.InitDefaultCardOrder(playCardOrder);
				if (!autoPlayCardService.CustomCardOrderLoaded)
				{
					autoPlayCardService.InitCustomCardOrder();
				}
			}
		}
		catch
		{
		}
	}

	private async Task FetchHealActions()
	{
		if (easyHealService.TargetHealActionsLoaded)
		{
			return;
		}
		try
		{
			Dictionary<string, List<EasyHealManager.HealAction>> dictionary = JsonConvert.DeserializeObject<Dictionary<string, List<EasyHealManager.HealAction>>>(await OmenServiceBase<HTTPClientHelper>.Instance().Get().GetStringAsync("https://assets.sumemo.dev/heal-action.json"));
			if (dictionary != null)
			{
				easyHealService.InitTargetHealActions(dictionary.SelectMany((KeyValuePair<string, List<EasyHealManager.HealAction>> kv) => kv.Value).ToDictionary((EasyHealManager.HealAction act) => act.ID, (EasyHealManager.HealAction act) => act));
				if (!easyHealService.ActiveHealActionsLoaded)
				{
					easyHealService.InitActiveHealActions();
				}
			}
		}
		catch
		{
		}
	}

	private async Task FetchAll()
	{
		try
		{
			InlineArray2<Task> buffer = default(InlineArray2<Task>);
			buffer[0] = FetchPlayCardOrder();
			buffer[1] = FetchHealActions();
			await Task.WhenAll(buffer);
		}
		catch
		{
		}
		finally
		{
			if (actionSelect == null)
			{
				actionSelect = new ActionSelectCombo("##ActionSelect", ((IEnumerable<Action>)LuminaGetter.Get<Action>()).Where((Action x) => easyHealService.TargetHealActions.ContainsKey(((Action)(ref x)).RowId)));
			}
			if (config.EasyHealStorage.ActiveHealActions.Count == 0)
			{
				easyHealService.InitActiveHealActions();
			}
			actionSelect.SelectedIDs = config.EasyHealStorage.ActiveHealActions;
		}
	}

	protected override void ConfigUI()
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		AutoPlayCardUI();
		ImGui.NewLine();
		EasyHealUI();
		ImGui.NewLine();
		EasyDispelUI();
		ImGui.NewLine();
		EasyRaiseUI();
		ImGui.NewLine();
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("Notification")));
		ImGui.Spacing();
		IndentDisposable val = ImRaii.PushIndent(1, true);
		try
		{
			if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("SendChat")), ref config.SendChat))
			{
				config.Save(this);
			}
			if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("SendNotification")), ref config.SendNotification))
			{
				config.Save(this);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void AutoPlayCardUI()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		AutoPlayCardManager.Storage playCardStorage = config.AutoPlayCardStorage;
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("HealerHelper-AutoPlayCardTitle")));
		ImGuiOm.HelpMarker(LanguageManager.Get("HealerHelper-EasyRedirectDescription", LuminaWrapper.GetActionName(17055u)), 40f, (FontAwesomeIcon)61530);
		ImGui.Spacing();
		IndentDisposable val = ImRaii.PushIndent(1, true);
		try
		{
			DrawConfigRadio(LanguageManager.Get("Disable") + "##autocard", playCardStorage.AutoPlayCard, AutoPlayCardManager.AutoPlayCardStatus.Disable, delegate(AutoPlayCardManager.AutoPlayCardStatus v)
			{
				playCardStorage.AutoPlayCard = v;
			});
			DrawConfigRadio(LanguageManager.Get("Common") + " (" + LanguageManager.Get("HealerHelper-AutoPlayCard-CommonDescription") + ")", playCardStorage.AutoPlayCard, AutoPlayCardManager.AutoPlayCardStatus.Default, delegate(AutoPlayCardManager.AutoPlayCardStatus v)
			{
				playCardStorage.AutoPlayCard = v;
			});
			DrawConfigRadio(LanguageManager.Get("Custom") + " (" + LanguageManager.Get("HealerHelper-AutoPlayCard-CustomDescription") + ")", playCardStorage.AutoPlayCard, AutoPlayCardManager.AutoPlayCardStatus.Custom, delegate(AutoPlayCardManager.AutoPlayCardStatus v)
			{
				playCardStorage.AutoPlayCard = v;
			});
			if (playCardStorage.AutoPlayCard == AutoPlayCardManager.AutoPlayCardStatus.Custom)
			{
				ImGui.Spacing();
				CustomCardUI();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void CustomCardUI()
	{
		AutoPlayCardManager.Storage autoPlayCardStorage = config.AutoPlayCardStorage;
		DrawCustomCardSection("HealerHelper-AutoPlayCard-MeleeOpener", autoPlayCardStorage.CustomCardOrder.Melee["opener"], "Melee", "opener", "meleeopener");
		DrawCustomCardSection("HealerHelper-AutoPlayCard-Melee2Min", autoPlayCardStorage.CustomCardOrder.Melee["2m+"], "Melee", "2m+", "melee2m");
		DrawCustomCardSection("HealerHelper-AutoPlayCard-RangeOpener", autoPlayCardStorage.CustomCardOrder.Range["opener"], "Range", "opener", "rangeopener");
		DrawCustomCardSection("HealerHelper-AutoPlayCard-Range2Min", autoPlayCardStorage.CustomCardOrder.Range["2m+"], "Range", "2m+", "range2m");
		config.Save(this);
	}

	private void DrawCustomCardSection(string titleKey, string[] order, string role, string section, string resetKeySuffix)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		ImGui.AlignTextToFramePadding();
		Vector4 vector = KnownColor.LightYellow.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get(titleKey)));
		if (CustomCardOrderUI(order))
		{
			config.Save(this);
			autoPlayCardService.OrderCandidates();
		}
		ImGui.SameLine();
		ImGuiOm.ScaledDummy(5f, 0f);
		ImGui.SameLine();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(2, 2);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("Reset"));
		((ImU8String)(ref val)).AppendLiteral("##");
		((ImU8String)(ref val)).AppendFormatted<string>(resetKeySuffix);
		if (ImGui.Button(val, default(Vector2)))
		{
			autoPlayCardService.InitCustomCardOrder(role, section);
			config.Save(this);
		}
		ImGui.Spacing();
	}

	private static bool CustomCardOrderUI(string[] cardOrder)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		ImU8String val = default(ImU8String);
		for (int i = 0; i < cardOrder.Length; i++)
		{
			((ImU8String)(ref val))._002Ector(0, 1);
			((ImU8String)(ref val)).AppendFormatted<int>(i);
			IdDisposable val2 = ImRaii.PushId(val, true);
			try
			{
				string? text = ((object)Unsafe.As<ReadOnlySeString, ReadOnlySeString>(ref JobNameMap[ReadOnlySeString.op_Implicit(cardOrder[i])])/*cast due to constrained. prefix*/).ToString();
				Vector2 vector = ImGui.CalcTextSize(ImU8String.op_Implicit(text), false, -1f);
				ImGui.Button(ImU8String.op_Implicit(text), new Vector2(vector.X + 20f, 0f));
				if (i != cardOrder.Length - 1)
				{
					ImGui.SameLine();
				}
				DragDropSourceDisposable val3 = ImRaii.DragDropSource();
				try
				{
					if (DragDropSourceDisposable.op_Implicit(val3))
					{
						CustomCardOrderDragIndex = i;
						ImGui.SetDragDropPayload(ImU8String.op_Implicit("##CustomCardOrder"), default(ReadOnlySpan<byte>), (ImGuiCond)0);
					}
				}
				finally
				{
					((DragDropSourceDisposable)(ref val3)).Dispose();
				}
				DragDropTargetDisposable val4 = ImRaii.DragDropTarget();
				try
				{
					if (DragDropTargetDisposable.op_Implicit(val4))
					{
						ImGui.AcceptDragDropPayload(ImU8String.op_Implicit("##CustomCardOrder"), (ImGuiDragDropFlags)0);
						if (ImGui.IsMouseReleased((ImGuiMouseButton)0) && CustomCardOrderDragIndex.HasValue)
						{
							ref string reference = ref cardOrder[i];
							ref string reference2 = ref cardOrder[CustomCardOrderDragIndex.Value];
							string text2 = cardOrder[CustomCardOrderDragIndex.Value];
							string text3 = cardOrder[i];
							reference = text2;
							reference2 = text3;
							result = true;
						}
					}
				}
				finally
				{
					((DragDropTargetDisposable)(ref val4)).Dispose();
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		return result;
	}

	private void EasyHealUI()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		EasyHealManager.Storage easyHealStorage = config.EasyHealStorage;
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("HealerHelper-EasyHealTitle")));
		ImGuiOm.HelpMarker(LanguageManager.Get("HealerHelper-EasyRedirectDescription", LanguageManager.Get("HealerHelper-SingleTargetHeal")), 40f, (FontAwesomeIcon)61530);
		ImGui.Spacing();
		IndentDisposable val = ImRaii.PushIndent(1, true);
		try
		{
			DrawConfigRadio(LanguageManager.Get("Disable") + "##easyheal", easyHealStorage.EasyHeal, EasyHealManager.EasyHealStatus.Disable, delegate(EasyHealManager.EasyHealStatus v)
			{
				easyHealStorage.EasyHeal = v;
			});
			DrawConfigRadio(LanguageManager.Get("Enable") + " (" + LanguageManager.Get("HealerHelper-EasyHeal-EnableDescription") + ")", easyHealStorage.EasyHeal, EasyHealManager.EasyHealStatus.Enable, delegate(EasyHealManager.EasyHealStatus v)
			{
				easyHealStorage.EasyHeal = v;
			});
			if (easyHealStorage.EasyHeal == EasyHealManager.EasyHealStatus.Enable)
			{
				ImGui.Spacing();
				ActiveHealActionsSelect();
				ImGui.Spacing();
				vector = KnownColor.LightGreen.ToVector4();
				ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("HealerHelper-EasyHeal-HealThreshold")));
				ImGuiOm.HelpMarker(LanguageManager.Get("HealerHelper-EasyHeal-HealThresholdHelp"), 40f, (FontAwesomeIcon)61530);
				ImGui.Spacing();
				if (ImGui.SliderFloat(ImU8String.op_Implicit("##HealThreshold"), ref easyHealStorage.NeedHealThreshold, 0f, 1f, ImU8String.op_Implicit("%.2f"), (ImGuiSliderFlags)0))
				{
					config.Save(this);
				}
				if (easyHealStorage.NeedHealThreshold > 0.92f)
				{
					ImGui.Spacing();
					vector = KnownColor.Orange.ToVector4();
					ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("HealerHelper-EasyHeal-OverhealWarning")));
				}
				ImGui.Spacing();
				vector = KnownColor.LightYellow.ToVector4();
				ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("HealerHelper-EasyHeal-OverhealTargetDescription")));
				ImGui.Spacing();
				DrawConfigRadio(LanguageManager.Get("HealerHelper-EasyHeal-OverhealTarget-Prevent") + "##overhealtarget", easyHealStorage.OverhealTarget, EasyHealManager.OverhealTarget.Prevent, delegate(EasyHealManager.OverhealTarget v)
				{
					easyHealStorage.OverhealTarget = v;
				});
				ImGui.SameLine();
				ImGuiOm.ScaledDummy(5f, 0f);
				ImGui.SameLine();
				DrawConfigRadio(LanguageManager.Get("HealerHelper-EasyHeal-OverhealTarget-Local") + "##overhealtarget", easyHealStorage.OverhealTarget, EasyHealManager.OverhealTarget.Local, delegate(EasyHealManager.OverhealTarget v)
				{
					easyHealStorage.OverhealTarget = v;
				});
				ImGui.SameLine();
				ImGuiOm.ScaledDummy(5f, 0f);
				ImGui.SameLine();
				DrawConfigRadio(LanguageManager.Get("HealerHelper-EasyHeal-OverhealTarget-FirstTank") + "##overhealtarget", easyHealStorage.OverhealTarget, EasyHealManager.OverhealTarget.FirstTank, delegate(EasyHealManager.OverhealTarget v)
				{
					easyHealStorage.OverhealTarget = v;
				});
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void ActiveHealActionsSelect()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.YellowGreen.ToVector4();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(0, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("HealerHelper-EasyHeal-ActiveHealAction"));
		ImGui.TextColored(ref vector, val);
		ImGui.Spacing();
		if (actionSelect.DrawCheckbox())
		{
			config.EasyHealStorage.ActiveHealActions = actionSelect.SelectedIDs;
			config.Save(this);
		}
		ImGui.SameLine();
		ImGuiOm.ScaledDummy(5f, 0f);
		ImGui.SameLine();
		ImU8String val2 = default(ImU8String);
		((ImU8String)(ref val2))._002Ector(19, 1);
		((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get("Reset"));
		((ImU8String)(ref val2)).AppendLiteral("##activehealactions");
		if (ImGui.Button(val2, default(Vector2)))
		{
			easyHealService.InitActiveHealActions();
			config.Save(this);
		}
	}

	private void EasyDispelUI()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		EasyHealManager.Storage easyHealStorage = config.EasyHealStorage;
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("HealerHelper-EasyDispelTitle")));
		ImGuiOm.HelpMarker(LanguageManager.Get("HealerHelper-EasyRedirectDescription", LuminaWrapper.GetActionName(7568u)), 40f, (FontAwesomeIcon)61530);
		ImGui.Spacing();
		IndentDisposable val = ImRaii.PushIndent(1, true);
		try
		{
			DrawConfigRadio(LanguageManager.Get("Disable") + "##easydispel", easyHealStorage.EasyDispel, EasyHealManager.EasyDispelStatus.Disable, delegate(EasyHealManager.EasyDispelStatus v)
			{
				easyHealStorage.EasyDispel = v;
			});
			GroupDisposable val2 = ImRaii.Group();
			try
			{
				ImU8String val3 = new ImU8String(15, 2);
				((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get("Enable"));
				((ImU8String)(ref val3)).AppendLiteral(" [");
				((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get("InOrder"));
				((ImU8String)(ref val3)).AppendLiteral("]##easydispel");
				if (ImGui.RadioButton(val3, easyHealStorage != null && easyHealStorage.EasyDispel == EasyHealManager.EasyDispelStatus.Enable && easyHealStorage.DispelOrder == EasyHealManager.DispelOrderStatus.Order))
				{
					easyHealStorage.EasyDispel = EasyHealManager.EasyDispelStatus.Enable;
					easyHealStorage.DispelOrder = EasyHealManager.DispelOrderStatus.Order;
					config.Save(this);
				}
				ImU8String val4 = new ImU8String(15, 2);
				((ImU8String)(ref val4)).AppendFormatted<string>(LanguageManager.Get("Enable"));
				((ImU8String)(ref val4)).AppendLiteral(" [");
				((ImU8String)(ref val4)).AppendFormatted<string>(LanguageManager.Get("InReverseOrder"));
				((ImU8String)(ref val4)).AppendLiteral("]##easydispel");
				if (ImGui.RadioButton(val4, easyHealStorage != null && easyHealStorage.EasyDispel == EasyHealManager.EasyDispelStatus.Enable && easyHealStorage.DispelOrder == EasyHealManager.DispelOrderStatus.Reverse))
				{
					easyHealStorage.EasyDispel = EasyHealManager.EasyDispelStatus.Enable;
					easyHealStorage.DispelOrder = EasyHealManager.DispelOrderStatus.Reverse;
					config.Save(this);
				}
			}
			finally
			{
				((GroupDisposable)(ref val2)).Dispose();
			}
			ImGuiOm.TooltipHover(LanguageManager.Get("HealerHelper-OrderHelp"), 20f * Globals.GlobalUIScale);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void EasyRaiseUI()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		EasyHealManager.Storage easyHealStorage = config.EasyHealStorage;
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("HealerHelper-EasyRaiseTitle")));
		ImGui.Spacing();
		IndentDisposable val = ImRaii.PushIndent(1, true);
		try
		{
			DrawConfigRadio(LanguageManager.Get("Disable") + "##easyraise", easyHealStorage.EasyRaise, EasyHealManager.EasyRaiseStatus.Disable, delegate(EasyHealManager.EasyRaiseStatus v)
			{
				easyHealStorage.EasyRaise = v;
			});
			GroupDisposable val2 = ImRaii.Group();
			try
			{
				ImU8String val3 = new ImU8String(14, 2);
				((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get("Enable"));
				((ImU8String)(ref val3)).AppendLiteral(" [");
				((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get("InOrder"));
				((ImU8String)(ref val3)).AppendLiteral("]##easyraise");
				if (ImGui.RadioButton(val3, easyHealStorage != null && easyHealStorage.EasyRaise == EasyHealManager.EasyRaiseStatus.Enable && easyHealStorage.RaiseOrder == EasyHealManager.RaiseOrderStatus.Order))
				{
					easyHealStorage.EasyRaise = EasyHealManager.EasyRaiseStatus.Enable;
					easyHealStorage.RaiseOrder = EasyHealManager.RaiseOrderStatus.Order;
					config.Save(this);
				}
				ImU8String val4 = new ImU8String(14, 2);
				((ImU8String)(ref val4)).AppendFormatted<string>(LanguageManager.Get("Enable"));
				((ImU8String)(ref val4)).AppendLiteral(" [");
				((ImU8String)(ref val4)).AppendFormatted<string>(LanguageManager.Get("InReverseOrder"));
				((ImU8String)(ref val4)).AppendLiteral("]##easyraise");
				if (ImGui.RadioButton(val4, easyHealStorage != null && easyHealStorage.EasyRaise == EasyHealManager.EasyRaiseStatus.Enable && easyHealStorage.RaiseOrder == EasyHealManager.RaiseOrderStatus.Reverse))
				{
					easyHealStorage.EasyRaise = EasyHealManager.EasyRaiseStatus.Enable;
					easyHealStorage.RaiseOrder = EasyHealManager.RaiseOrderStatus.Reverse;
					config.Save(this);
				}
			}
			finally
			{
				((GroupDisposable)(ref val2)).Dispose();
			}
			ImGuiOm.TooltipHover(LanguageManager.Get("HealerHelper-OrderHelp"), 20f * Globals.GlobalUIScale);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void DrawConfigRadio<T>(string label, T currentValue, T targetValue, Action<T> setter) where T : Enum
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (ImGui.RadioButton(ImU8String.op_Implicit(label), currentValue.Equals(targetValue)))
		{
			setter(targetValue);
			config.Save(this);
		}
	}

	private unsafe void OnPreUseAction(ref bool isPrevented, ref ActionType type, ref uint actionID, ref ulong targetID, ref Vector3 location, ref uint extraParam, ref byte a7)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		if ((int)type != 1 || GameState.IsInPVPArea || ((AgentHUD)AgentHUD.Instance()).PartyMemberCount < 2)
		{
			return;
		}
		ClassJob classJobData = LocalPlayerState.ClassJobData;
		bool flag = ((ClassJob)(ref classJobData)).Role == 4;
		uint classJob = LocalPlayerState.ClassJob;
		bool flag2 = ((classJob == 27 || classJob == 35) ? true : false);
		bool flag3 = flag2;
		if (!flag && !flag3)
		{
			return;
		}
		EasyHealManager.Storage easyHealStorage = config.EasyHealStorage;
		IGameObject gameObject = IOmenDalamudService<IObjectTable>.Instance().SearchByID(targetID, IObjectTable.CharactersRange);
		if (flag)
		{
			if (LocalPlayerState.ClassJob == 33 && AutoPlayCardManager.PlayCardActions.Contains(actionID) && config.AutoPlayCardStorage.AutoPlayCard != AutoPlayCardManager.AutoPlayCardStatus.Disable)
			{
				if (!(gameObject is IBattleChara battleChara) || !ActionManager.CanUseActionOnTarget(37023u, (GameObject*)battleChara.ToStruct()))
				{
					targetID = 3758096384uL;
				}
				autoPlayCardService.OnPrePlayCard(ref targetID, ref actionID);
			}
			else if (easyHealStorage.EasyHeal == EasyHealManager.EasyHealStatus.Enable && easyHealStorage.ActiveHealActions.Contains(actionID))
			{
				if (!(gameObject is IBattleChara battleChara2) || !ActionManager.CanUseActionOnTarget(3595u, (GameObject*)battleChara2.ToStruct()))
				{
					targetID = 3758096384uL;
				}
				easyHealService.OnPreHeal(ref targetID, ref actionID, ref isPrevented);
			}
			else if (easyHealStorage.EasyDispel == EasyHealManager.EasyDispelStatus.Enable && actionID == 7568)
			{
				if (!(gameObject is IBattleChara battleChara3) || !ActionManager.CanUseActionOnTarget(7568u, (GameObject*)battleChara3.ToStruct()))
				{
					targetID = 3758096384uL;
				}
				easyHealService.OnPreDispel(ref targetID);
			}
		}
		if (easyHealStorage.EasyRaise == EasyHealManager.EasyRaiseStatus.Enable && EasyHealManager.RaiseActions.Contains(actionID))
		{
			if (!(gameObject is IBattleChara battleChara4) || battleChara4.StatusFlags.IsSetAny<StatusFlags>(new ReadOnlySpan<StatusFlags>(Unsafe.AsPointer(ref global::_003CPrivateImplementationDetails_003E._4BF5122F344554C53BDE2EBB8CD2B7E3D1600AD631C385A5D7CCE23C7785459A), 1)))
			{
				targetID = 3758096384uL;
			}
			easyHealService.OnPreRaise(ref targetID, ref actionID);
		}
	}

	private void OnZoneChanged(uint u)
	{
		autoPlayCardService.CurrentDutySection = AutoPlayCardManager.DutySection.Enter;
	}

	private void OnDutyRecommenced(IDutyStateEventArgs args)
	{
		autoPlayCardService.CurrentDutySection = AutoPlayCardManager.DutySection.Enter;
		autoPlayCardService.OrderCandidates();
	}

	private void OnDutyStarted(IDutyStateEventArgs args)
	{
		autoPlayCardService.CurrentDutySection = AutoPlayCardManager.DutySection.Enter;
		autoPlayCardService.OrderCandidates();
	}

	private void OnConditionChanged(ConditionFlag flag, bool value)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Invalid comparison between Unknown and I4
		if ((int)flag == 26 && autoPlayCardService.CurrentDutySection == AutoPlayCardManager.DutySection.Enter)
		{
			autoPlayCardService.CurrentDutySection = AutoPlayCardManager.DutySection.Start;
			autoPlayCardService.StartTimeUTC = OmenServiceBase<StandardTimeManager>.Instance().UTCNow;
		}
	}
}
