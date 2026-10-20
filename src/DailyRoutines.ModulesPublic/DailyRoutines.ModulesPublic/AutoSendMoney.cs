using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Hooking;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;
using OmenTools;
using OmenTools.Dalamud;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.Info.Game.Data;
using OmenTools.Info.Lumina;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;

namespace DailyRoutines.ModulesPublic;

public class AutoSendMoney : ModuleBase
{
	private class Config : ModuleConfig
	{
		public int Delay1 = 200;

		public int Delay2 = 500;

		public int Step1 = 50;

		public int Step2 = 100;
	}

	private class Member
	{
		public uint EntityID;

		public string FirstName;

		public int GroupIndex = -1;

		public string World;

		public string FullName => FirstName + "@" + World;

		public Member()
		{
		}

		public unsafe Member(ICharacter gameObject)
		{
			EntityID = gameObject.EntityID;
			FirstName = gameObject.Name;
			ushort homeWorld = ((BattleChara)gameObject.ToBCStruct()).HomeWorld;
			World = LuminaWrapper.GetWorldName(homeWorld) ?? "???";
		}
	}

	private readonly record struct PreCheckState(bool SelfConfirmed, bool OtherConfirmed);

	private sealed class SendMoneyRuntime : IDisposable
	{
		private unsafe delegate nint TradeRequestDelegate(InventoryManager* manager, uint entityID);

		private unsafe delegate nint TradeStatusUpdateDelegate(InventoryManager* manager, nint entityID, nint a3);

		private static readonly CompSig TradeRequestSig = new CompSig("48 89 6C 24 ?? 56 57 41 56 48 83 EC ?? 48 8B E9 44 8B F2 48 8D 0D");

		private Hook<TradeRequestDelegate>? TradeRequestHook;

		private static readonly CompSig TradeStatusUpdateSig = new CompSig("E9 ?? ?? ?? ?? CC CC CC CC CC CC CC CC CC CC CC CC CC CC CC 4C 8B C2 8B D1 48 8D 0D ?? ?? ?? ?? E9 ?? ?? ?? ?? CC CC CC CC CC CC CC CC CC CC CC CC CC CC CC 48 8D 0D");

		private Hook<TradeStatusUpdateDelegate>? TradeStatusUpdateHook;

		private readonly AutoSendMoney owner;

		private readonly HashSet<uint> pendingTradeRequests = new HashSet<uint>();

		private readonly Dictionary<uint, long> tradePlan;

		private PreCheckState checkState;

		private uint currentMoney;

		private bool isDisposed;

		private bool isTrading;

		private uint lastTradeEntityID;

		private static readonly FrozenSet<uint> TradeFinishLogMessages = FrozenSet.Create<uint>(10920u, 10921u, 10922u, 10923u);

		public unsafe SendMoneyRuntime(AutoSendMoney owner)
		{
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Expected O, but got Unknown
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Expected O, but got Unknown
			this.owner = owner;
			Dictionary<uint, long> collection = new Dictionary<uint, long>();
			foreach (KeyValuePair<uint, long> item in owner.editPlan.Where((KeyValuePair<uint, long> i) => i.Value > 0))
			{
				collection.Add(item);
			}
			tradePlan = collection;
			TradeRequestHook = TradeRequestSig.GetHook<TradeRequestDelegate>(OnTradeRequest);
			TradeRequestHook.Enable();
			TradeStatusUpdateHook = TradeStatusUpdateSig.GetHook<TradeStatusUpdateDelegate>(OnTradeStatusUpdate);
			TradeStatusUpdateHook.Enable();
			DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)1, "Trade", new AddonEventDelegate(OnTradeAddonSetup));
			OmenServiceBase<FrameworkManager>.Instance().Reg(new OnUpdateDelegate(OnFrameworkTick), 1000u);
			OmenServiceBase<LogMessageManager>.Instance().RegPost(OnLogMessage);
			TryQueueNextTrade();
		}

		public void Dispose()
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Expected O, but got Unknown
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Expected O, but got Unknown
			if (!isDisposed)
			{
				isDisposed = true;
				OmenServiceBase<FrameworkManager>.Instance().Unreg(OnFrameworkTick);
				DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEvent)1, "Trade", new AddonEventDelegate(OnTradeAddonSetup));
				OmenServiceBase<LogMessageManager>.Instance().Unreg(OnLogMessage);
				TradeRequestHook?.Dispose();
				TradeRequestHook = null;
				TradeStatusUpdateHook?.Dispose();
				TradeStatusUpdateHook = null;
				owner.TaskHelper?.Abort();
			}
		}

		public long GetRemaining(uint entityID)
		{
			return tradePlan.GetValueOrDefault(entityID, 0L);
		}

		private unsafe void OnLogMessage(uint logMessageID, LogMessageQueueItem item)
		{
			if (TradeFinishLogMessages.Contains(logMessageID) && Addons.Trade != null)
			{
				((AtkUnitBase)Addons.Trade).Close(true);
				CompleteTrade();
			}
		}

		private void OnTradeAddonSetup(AddonEvent type, AddonArgs args)
		{
			owner.TaskHelper?.Abort();
			pendingTradeRequests.Clear();
			if (!tradePlan.TryGetValue(lastTradeEntityID, out var value))
			{
				owner.TaskHelper?.DelayNext(owner.GetRandomDelayMS());
				owner.TaskHelper?.Enqueue((Action)CancelTradeAddon, "CancelTradeAddon", 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
				return;
			}
			owner.TaskHelper?.DelayNext(owner.GetRandomDelayMS());
			owner.TaskHelper?.Enqueue(() => SetTradeGil((uint)Math.Min(value, 1000000L)), "SetTradeGil");
			owner.TaskHelper?.DelayNext(owner.GetRandomDelayMS());
			owner.TaskHelper?.Enqueue((Func<bool>)ConfirmPreCheck, "ConfirmPreCheck", 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
		}

		private unsafe nint OnTradeRequest(InventoryManager* manager, uint entityID)
		{
			if (TradeRequestHook == null)
			{
				return 0;
			}
			nint num = TradeRequestHook.Original(manager, entityID);
			if (num == 0)
			{
				BeginTrade(entityID);
				return num;
			}
			pendingTradeRequests.Remove(entityID);
			return num;
		}

		private unsafe nint OnTradeStatusUpdate(InventoryManager* manager, nint entityID, nint a3)
		{
			switch (Marshal.ReadByte(a3 + 4))
			{
			case 1:
				BeginTrade((uint)Marshal.ReadInt32(a3 + 40));
				break;
			case 16:
				switch (Marshal.ReadByte(a3 + 5))
				{
				case 3:
					UpdatePreCheck((uint)Marshal.ReadInt32(a3 + 40), confirm: false);
					break;
				case 4:
				case 5:
					UpdatePreCheck((uint)Marshal.ReadInt32(a3 + 40), confirm: true);
					break;
				}
				break;
			case 5:
				ConfirmFinal();
				break;
			case 7:
				CancelTrade();
				break;
			}
			if (TradeStatusUpdateHook != null)
			{
				return TradeStatusUpdateHook.Original(manager, entityID, a3);
			}
			return 0;
		}

		private void BeginTrade(uint entityID)
		{
			currentMoney = 0u;
			checkState = default(PreCheckState);
			isTrading = true;
			lastTradeEntityID = entityID;
			pendingTradeRequests.Remove(entityID);
		}

		private void UpdatePreCheck(uint objectID, bool confirm)
		{
			if (objectID == LocalPlayerState.EntityID)
			{
				checkState = checkState with
				{
					SelfConfirmed = confirm
				};
			}
			else if (objectID == lastTradeEntityID)
			{
				checkState = checkState with
				{
					OtherConfirmed = confirm
				};
			}
			if (!tradePlan.TryGetValue(lastTradeEntityID, out var value))
			{
				owner.TaskHelper?.DelayNext(owner.GetRandomDelayMS());
				owner.TaskHelper?.Enqueue((Action)CancelTradeAddon, "CancelTradeAddon", 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
			}
			else if (currentMoney <= value)
			{
				PreCheckState preCheckState = checkState;
				if (!preCheckState.SelfConfirmed && preCheckState.OtherConfirmed)
				{
					owner.TaskHelper?.DelayNext(owner.GetRandomDelayMS());
					owner.TaskHelper?.Enqueue((Func<bool>)ConfirmPreCheck, "ConfirmPreCheck", 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
				}
			}
		}

		private void ConfirmFinal()
		{
			if (tradePlan.TryGetValue(lastTradeEntityID, out var value) && currentMoney <= value)
			{
				owner.TaskHelper?.DelayNext(owner.GetRandomDelayMS());
				owner.TaskHelper?.Enqueue(delegate
				{
					FinalCheckTradeAddon();
				}, "FinalCheckTradeAddon");
			}
		}

		private void CancelTrade()
		{
			isTrading = false;
			pendingTradeRequests.Clear();
			checkState = default(PreCheckState);
			owner.TaskHelper?.Abort();
		}

		private void CompleteTrade()
		{
			isTrading = false;
			pendingTradeRequests.Clear();
			checkState = default(PreCheckState);
			if (!tradePlan.ContainsKey(lastTradeEntityID))
			{
				DLog.Warning(LanguageManager.Get("AutoSendMoney-NoPlan"));
			}
			else
			{
				tradePlan[lastTradeEntityID] -= currentMoney;
				if (tradePlan[lastTradeEntityID] <= 0)
				{
					tradePlan.Remove(lastTradeEntityID);
					owner.editPlan.Remove(lastTradeEntityID);
				}
			}
			if (tradePlan.Count == 0)
			{
				StopSelf();
			}
		}

		private unsafe void IssueTradeRequest(uint entityID, GameObject* gameObjectAddress)
		{
			((TargetSystem)TargetSystem.Instance()).Target = gameObjectAddress;
			((InventoryManager)InventoryManager.Instance()).SendTradeRequest(entityID);
		}

		private unsafe bool SetTradeGil(uint money)
		{
			if (!(*Addons.Trade).IsAddonAndNodesReady())
			{
				return false;
			}
			((InventoryManager)InventoryManager.Instance()).SetTradeGilAmount(money);
			currentMoney = money;
			return true;
		}

		private unsafe bool ConfirmPreCheck()
		{
			if (!(*Addons.Trade).IsAddonAndNodesReady())
			{
				return false;
			}
			if (checkState.SelfConfirmed)
			{
				return true;
			}
			(*((AtkUnitBase)Addons.Trade).GetComponentButtonById(33u)).Click();
			checkState = checkState with
			{
				SelfConfirmed = true
			};
			return true;
		}

		private void OnFrameworkTick(IFramework framework)
		{
			TryQueueNextTrade();
		}

		private unsafe void TryQueueNextTrade()
		{
			if (tradePlan.Count == 0)
			{
				StopSelf();
				return;
			}
			TaskHelper taskHelper = owner.TaskHelper;
			if (taskHelper != null && !isTrading && !taskHelper.IsBusy && TrySelectTarget(out var entityID, out var targetAddress) && pendingTradeRequests.Add(entityID))
			{
				taskHelper.DelayNext(owner.GetRandomDelayMS(), "AutoSendMoney-IssueTradeRequest");
				taskHelper.Enqueue(delegate
				{
					IssueTradeRequest(entityID, (GameObject*)targetAddress);
				}, $"{"IssueTradeRequest"}({entityID})");
			}
		}

		private void StopSelf()
		{
			owner.runtime = null;
			Dispose();
		}

		private bool TrySelectTarget(out uint entityID, out nint address)
		{
			entityID = 0u;
			address = 0;
			if (lastTradeEntityID != 0 && tradePlan.ContainsKey(lastTradeEntityID) && !pendingTradeRequests.Contains(lastTradeEntityID))
			{
				IGameObject gameObject = IOmenDalamudService<IObjectTable>.Instance().SearchByEntityID(lastTradeEntityID);
				if (gameObject != null && IsWithinTradeDistance(gameObject.Position))
				{
					entityID = lastTradeEntityID;
					address = gameObject.Address;
					return true;
				}
			}
			foreach (uint key in tradePlan.Keys)
			{
				if (!pendingTradeRequests.Contains(key))
				{
					IGameObject gameObject2 = IOmenDalamudService<IObjectTable>.Instance().SearchByEntityID(key);
					if (gameObject2 != null && IsWithinTradeDistance(gameObject2.Position))
					{
						entityID = key;
						address = gameObject2.Address;
						return true;
					}
				}
			}
			return false;
		}

		private unsafe static void CancelTradeAddon()
		{
			if (Addons.Trade != null)
			{
				(*Addons.Trade).Callback(1, 0);
			}
		}

		private unsafe static void FinalCheckTradeAddon(bool confirm = true)
		{
			if (Addons.SelectYesno != null)
			{
				(*Addons.SelectYesno).Callback((!confirm) ? 1 : 0);
			}
		}
	}

	private Config config;

	private int[] moneyButtons = Array.Empty<int>();

	private readonly List<Member> members = new List<Member>();

	private readonly Dictionary<uint, long> editPlan = new Dictionary<uint, long>();

	private float nameLength = -1f;

	private double planAll;

	private long currentChange;

	private SendMoneyRuntime? runtime;

	private const uint MAXIMUM_GIL_PER_TRADE = 1000000u;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoSendMoneyTitle"),
		Description = LanguageManager.Get("AutoSendMoneyDescription"),
		Category = ModuleCategory.General,
		Author = new string[1] { "status102" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		NeedAuth = true
	};

	private bool IsRunning => runtime != null;

	protected override void Init()
	{
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		ValidateConfigChanges();
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 5000
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
	}

	protected override void Uninit()
	{
		Stop();
	}

	protected override void ConfigUI()
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		if (nameLength < 0f)
		{
			nameLength = ImGui.CalcTextSize(ImU8String.op_Implicit(LanguageManager.Get("All")), false, -1f).X;
		}
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(0, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("Settings"));
		ImGui.TextColored(ref vector, val);
		IndentDisposable val2 = ImRaii.PushIndent(1, true);
		ImU8String val12;
		try
		{
			ItemWidthDisposable val3 = ImRaii.ItemWidth(100f * Globals.GlobalUIScale);
			try
			{
				ImU8String val4 = new ImU8String(12, 1);
				((ImU8String)(ref val4)).AppendFormatted<string>(LanguageManager.Get("AutoSendMoney-Step", 1));
				((ImU8String)(ref val4)).AppendLiteral("##Step1Input");
				ImU8String val5 = val4;
				ref int step = ref config.Step1;
				ImU8String val6 = default(ImU8String);
				ImGui.InputInt(val5, ref step, 0, 0, val6, (ImGuiInputTextFlags)1);
				if (ImGui.IsItemDeactivatedAfterEdit())
				{
					ValidateConfigChanges();
					config.Save(this);
				}
				val6 = new ImU8String(12, 1);
				((ImU8String)(ref val6)).AppendFormatted<string>(LanguageManager.Get("AutoSendMoney-Step", 2));
				((ImU8String)(ref val6)).AppendLiteral("##Step2Input");
				ImU8String val7 = val6;
				ref int step2 = ref config.Step2;
				ImU8String val8 = default(ImU8String);
				ImGui.InputInt(val7, ref step2, 0, 0, val8, (ImGuiInputTextFlags)1);
				if (ImGui.IsItemDeactivatedAfterEdit())
				{
					ValidateConfigChanges();
					config.Save(this);
				}
				val8 = new ImU8String(22, 1);
				((ImU8String)(ref val8)).AppendFormatted<string>(LanguageManager.Get("AutoSendMoney-DelayLowerLimit"));
				((ImU8String)(ref val8)).AppendLiteral("##DelayLowerLimitInput");
				ImU8String val9 = val8;
				ref int delay = ref config.Delay1;
				ImU8String val10 = default(ImU8String);
				ImGui.InputInt(val9, ref delay, 0, 0, val10, (ImGuiInputTextFlags)1);
				if (ImGui.IsItemDeactivatedAfterEdit())
				{
					ValidateConfigChanges();
					config.Save(this);
				}
				val10 = new ImU8String(22, 1);
				((ImU8String)(ref val10)).AppendFormatted<string>(LanguageManager.Get("AutoSendMoney-DelayUpperLimit"));
				((ImU8String)(ref val10)).AppendLiteral("##DelayUpperLimitInput");
				ImU8String val11 = val10;
				ref int delay2 = ref config.Delay2;
				val12 = default(ImU8String);
				ImGui.InputInt(val11, ref delay2, 0, 0, val12, (ImGuiInputTextFlags)1);
				if (ImGui.IsItemDeactivatedAfterEdit())
				{
					ValidateConfigChanges();
					config.Save(this);
				}
			}
			finally
			{
				((IDisposable)val3)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
		ImGui.NewLine();
		vector = KnownColor.Chocolate.ToVector4();
		((ImU8String)(ref val12))._002Ector(0, 1);
		((ImU8String)(ref val12)).AppendFormatted<string>(LanguageManager.Get("Control"));
		ImGui.TextColored(ref vector, val12);
		val2 = ImRaii.PushIndent(1, true);
		try
		{
			DisabledDisposable val13 = ImRaii.Disabled(IsRunning);
			try
			{
				ImU8String val14 = new ImU8String(1, 2);
				((ImU8String)(ref val14)).AppendFormatted<string>(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61726));
				((ImU8String)(ref val14)).AppendLiteral(" ");
				((ImU8String)(ref val14)).AppendFormatted<string>(LanguageManager.Get("Start"));
				if (ImGui.Button(val14, default(Vector2)))
				{
					Start();
				}
			}
			finally
			{
				((IDisposable)val13)?.Dispose();
			}
			ImGui.SameLine();
			ImU8String val15 = new ImU8String(1, 2);
			((ImU8String)(ref val15)).AppendFormatted<string>(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61517));
			((ImU8String)(ref val15)).AppendLiteral(" ");
			((ImU8String)(ref val15)).AppendFormatted<string>(LanguageManager.Get("Stop"));
			if (ImGui.Button(val15, default(Vector2)))
			{
				Stop();
			}
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
		ImGui.NewLine();
		vector = KnownColor.Chocolate.ToVector4();
		ImU8String val16 = default(ImU8String);
		((ImU8String)(ref val16))._002Ector(0, 1);
		((ImU8String)(ref val16)).AppendFormatted<string>(LanguageManager.Get("AutoSendMoney-ListToTrade"));
		ImGui.TextColored(ref vector, val16);
		val2 = ImRaii.PushIndent(1, true);
		try
		{
			if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("AutoSendMoney-AddPartyList")), default(Vector2)))
			{
				AddPartyMembers();
			}
			ImGui.SameLine();
			if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("AutoSendMoney-AddTarget")), default(Vector2)))
			{
				AddCurrentTarget();
			}
			IdDisposable val17 = ImRaii.PushId(ImU8String.op_Implicit("All"), true);
			try
			{
				DrawGlobalPlan();
			}
			finally
			{
				((IDisposable)val17)?.Dispose();
			}
			foreach (Member member in members)
			{
				val17 = ImRaii.PushId(ImU8String.op_Implicit(member.EntityID.ToString()), true);
				try
				{
					DrawMemberPlan(member);
				}
				finally
				{
					((IDisposable)val17)?.Dispose();
				}
			}
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	private void DrawGlobalPlan()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		GroupDisposable val = ImRaii.Group();
		try
		{
			bool flag = editPlan.Count > 0;
			if (ImGui.Checkbox(ImU8String.op_Implicit("##AllHasPlan"), ref flag))
			{
				if (flag)
				{
					foreach (Member member in members)
					{
						if (!editPlan.ContainsKey(member.EntityID))
						{
							editPlan.Add(member.EntityID, (long)(planAll * 10000.0));
						}
					}
				}
				else
				{
					editPlan.Clear();
				}
			}
			ImGui.SameLine();
			ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("All")));
			DisabledDisposable val2 = ImRaii.Disabled(IsRunning);
			try
			{
				ImGui.SameLine(nameLength + 60f);
				ImGui.SetNextItemWidth(80f * Globals.GlobalUIScale);
				ImU8String val3 = new ImU8String(10, 1);
				((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get("Wan"));
				((ImU8String)(ref val3)).AppendLiteral("##AllMoney");
				ImGui.InputDouble(val3, ref planAll, 0.0, 0.0, ImU8String.op_Implicit("%.1lf"), (ImGuiInputTextFlags)1);
				if (ImGui.IsItemDeactivatedAfterEdit())
				{
					uint[] array = editPlan.Keys.ToArray();
					foreach (uint key in array)
					{
						editPlan[key] = (long)(planAll * 10000.0);
					}
				}
				currentChange = 0L;
				int[] array2 = moneyButtons;
				foreach (int num in array2)
				{
					ImGui.SameLine();
					ImGui.SetNextItemWidth(15f * Globals.GlobalUIScale);
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 2);
					defaultInterpolatedStringHandler.AppendFormatted((num < 0) ? string.Empty : ((object)'+'));
					defaultInterpolatedStringHandler.AppendFormatted(num);
					string text = defaultInterpolatedStringHandler.ToStringAndClear();
					ImU8String val4 = new ImU8String(5, 1);
					((ImU8String)(ref val4)).AppendFormatted<string>(text);
					((ImU8String)(ref val4)).AppendLiteral("##All");
					if (ImGui.Button(val4, default(Vector2)))
					{
						currentChange = num * 10000;
					}
				}
				if (currentChange != 0L)
				{
					planAll += (double)currentChange / 10000.0;
					foreach (Member member2 in members)
					{
						if (!editPlan.TryAdd(member2.EntityID, currentChange))
						{
							editPlan[member2.EntityID] += currentChange;
						}
					}
				}
				ImGui.SameLine();
				ImU8String val5 = new ImU8String(11, 1);
				((ImU8String)(ref val5)).AppendFormatted<string>(LanguageManager.Get("Reset"));
				((ImU8String)(ref val5)).AppendLiteral("###ResetAll");
				if (ImGui.Button(val5, default(Vector2)))
				{
					planAll = 0.0;
					uint[] array = editPlan.Keys.ToArray();
					foreach (uint key2 in array)
					{
						editPlan[key2] = 0L;
					}
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		finally
		{
			((GroupDisposable)(ref val)).Dispose();
		}
	}

	private void DrawMemberPlan(Member p)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		GroupDisposable val = ImRaii.Group();
		try
		{
			bool flag = editPlan.ContainsKey(p.EntityID);
			DisabledDisposable val2 = ImRaii.Disabled(IsRunning);
			try
			{
				ImU8String val3 = new ImU8String(11, 1);
				((ImU8String)(ref val3)).AppendLiteral("##");
				((ImU8String)(ref val3)).AppendFormatted<string>(p.FullName);
				((ImU8String)(ref val3)).AppendLiteral("-CheckBox");
				if (ImGui.Checkbox(val3, ref flag))
				{
					if (flag)
					{
						editPlan.Add(p.EntityID, (long)(planAll * 10000.0));
					}
					else
					{
						editPlan.Remove(p.EntityID);
					}
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
			if (p.GroupIndex >= 0)
			{
				ImGui.SameLine();
				ImU8String val4 = new ImU8String(1, 1);
				((ImU8String)(ref val4)).AppendFormatted<char>((char)(65 + p.GroupIndex));
				((ImU8String)(ref val4)).AppendLiteral("-");
				ImGui.TextUnformatted(val4);
			}
			ImGui.SameLine();
			ImGui.TextUnformatted(ImU8String.op_Implicit(p.FullName));
			ImGui.SameLine(nameLength + 60f);
			if (!flag)
			{
				return;
			}
			if (IsRunning)
			{
				ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("AutoSendMoney-Count", runtime?.GetRemaining(p.EntityID) ?? 0)));
				return;
			}
			ImGui.SetNextItemWidth(80f * Globals.GlobalUIScale);
			long value;
			double num = (editPlan.TryGetValue(p.EntityID, out value) ? ((double)value / 10000.0) : 0.0);
			ImU8String val5 = new ImU8String(8, 2);
			((ImU8String)(ref val5)).AppendFormatted<string>(LanguageManager.Get("Wan"));
			((ImU8String)(ref val5)).AppendLiteral("##");
			((ImU8String)(ref val5)).AppendFormatted<uint>(p.EntityID);
			((ImU8String)(ref val5)).AppendLiteral("-Money");
			ImGui.InputDouble(val5, ref num, 0.0, 0.0, ImU8String.op_Implicit("%.1lf"), (ImGuiInputTextFlags)1);
			if (ImGui.IsItemDeactivatedAfterEdit())
			{
				editPlan[p.EntityID] = (long)(num * 10000.0);
			}
			currentChange = 0L;
			int[] array = moneyButtons;
			foreach (int num2 in array)
			{
				ImGui.SameLine();
				ImGui.SetNextItemWidth(15f * Globals.GlobalUIScale);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 2);
				defaultInterpolatedStringHandler.AppendFormatted((num2 < 0) ? string.Empty : ((object)'+'));
				defaultInterpolatedStringHandler.AppendFormatted(num2);
				string text = defaultInterpolatedStringHandler.ToStringAndClear();
				ImU8String val6 = new ImU8String(9, 2);
				((ImU8String)(ref val6)).AppendFormatted<string>(text);
				((ImU8String)(ref val6)).AppendLiteral("##Single_");
				((ImU8String)(ref val6)).AppendFormatted<uint>(p.EntityID);
				if (ImGui.Button(val6, default(Vector2)))
				{
					currentChange = num2 * 10000;
				}
			}
			if (currentChange != 0L)
			{
				editPlan[p.EntityID] = (long)(num * 10000.0) + currentChange;
			}
			ImGui.SameLine();
			ImU8String val7 = new ImU8String(15, 2);
			((ImU8String)(ref val7)).AppendFormatted<string>(LanguageManager.Get("Reset"));
			((ImU8String)(ref val7)).AppendLiteral("###ResetSingle-");
			((ImU8String)(ref val7)).AppendFormatted<uint>(p.EntityID);
			if (ImGui.Button(val7, default(Vector2)))
			{
				editPlan[p.EntityID] = 0L;
			}
		}
		finally
		{
			((GroupDisposable)(ref val)).Dispose();
		}
	}

	public unsafe void AddPartyMembers()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		members.Clear();
		InfoProxyCrossRealm* ptr = InfoProxyCrossRealm.Instance();
		if (((InfoProxyCrossRealm)ptr).IsCrossRealm)
		{
			byte groupIndex = ((CrossRealmMember)InfoProxyCrossRealm.GetMemberByEntityId((uint)GameObjectId.op_Implicit(((BattleChara)Control.GetLocalPlayer()).GetGameObjectId()))).GroupIndex;
			AddCrossRealmGroupMembers(((InfoProxyCrossRealm)ptr).CrossRealmGroups[groupIndex], groupIndex);
			for (int i = 0; i < ((InfoProxyCrossRealm)ptr).CrossRealmGroups.Length; i++)
			{
				if (i != groupIndex)
				{
					AddCrossRealmGroupMembers(((InfoProxyCrossRealm)ptr).CrossRealmGroups[i], i);
				}
			}
		}
		else
		{
			AgentHUD* ptr2 = AgentHUD.Instance();
			for (int j = 0; j < ((AgentHUD)ptr2).PartyMemberCount; j++)
			{
				HudPartyMember val = ((AgentHUD)ptr2).PartyMembers[j];
				string textValue = SeString.Parse(val.Name.Value).TextValue;
				AddMember(val.EntityId, textValue, ((BattleChara)val.Object).HomeWorld);
			}
		}
		uint[] array = editPlan.Keys.Where((uint k) => members.All((Member m) => m.EntityID != k)).ToArray();
		foreach (uint key in array)
		{
			editPlan.Remove(key);
		}
		foreach (Member member in members)
		{
			editPlan.TryAdd(member.EntityID, 0L);
		}
		nameLength = members.Select((Member p) => ImGui.CalcTextSize(ImU8String.op_Implicit(p.FullName), false, -1f).X).Append(ImGui.CalcTextSize(ImU8String.op_Implicit(LanguageManager.Get("All")), false, -1f).X).Max();
	}

	private void AddCrossRealmGroupMembers(CrossRealmGroup crossRealmGroup, int groupIndex)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < crossRealmGroup.GroupMemberCount; i++)
		{
			CrossRealmMember val = ((CrossRealmGroup)(ref crossRealmGroup)).GroupMembers[i];
			AddMember(val.EntityId, SeString.Parse((ReadOnlySpan<byte>)((CrossRealmMember)(ref val)).Name).TextValue, (ushort)val.HomeWorld, groupIndex);
		}
	}

	private void AddMember(uint entityID, string fullName, ushort worldID, int groupIndex = -1)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (!string.IsNullOrWhiteSpace(fullName) && Sheets.Worlds.TryGetValue(worldID, out var value))
		{
			members.Add(new Member
			{
				EntityID = entityID,
				FirstName = fullName,
				World = ((object)((World)(ref value)).Name/*cast due to constrained. prefix*/).ToString(),
				GroupIndex = groupIndex
			});
		}
	}

	private unsafe void AddCurrentTarget()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Invalid comparison between Unknown and I4
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		GameObject* targetObject = ((TargetSystem)TargetSystem.Instance()).GetTargetObject();
		if (targetObject == null)
		{
			return;
		}
		IGameObject gameObject = IOmenDalamudService<IObjectTable>.Instance().SearchByEntityID(((GameObject)targetObject).EntityId);
		ICharacter player = gameObject as ICharacter;
		if (player != null && (int)gameObject.ObjectKind == 1 && !members.Any((Member p) => p.EntityID == player.EntityID))
		{
			members.Add(new Member(player));
			editPlan.TryAdd(player.EntityID, 0L);
			nameLength = members.Select((Member p) => ImGui.CalcTextSize(ImU8String.op_Implicit(p.FullName), false, -1f).X).Append(ImGui.CalcTextSize(ImU8String.op_Implicit(LanguageManager.Get("All")), false, -1f).X).Max();
		}
	}

	private void Start()
	{
		if (runtime == null)
		{
			ValidateConfigChanges();
			runtime = new SendMoneyRuntime(this);
		}
	}

	private void Stop()
	{
		runtime?.Dispose();
		runtime = null;
	}

	private static bool IsWithinTradeDistance(Vector3 pos2)
	{
		IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
		if (localPlayer == null)
		{
			return false;
		}
		Vector3 vector = localPlayer.Position - pos2;
		return vector.X * vector.X + vector.Z * vector.Z < 16f;
	}

	private int GetRandomDelayMS()
	{
		int num = Math.Max(0, config.Delay1);
		int num2 = Math.Max(0, config.Delay2);
		if (num2 <= num)
		{
			return num;
		}
		return Random.Shared.Next(num, num2);
	}

	private void ValidateConfigChanges()
	{
		this.config.Step1 = Math.Abs(this.config.Step1);
		this.config.Step2 = Math.Abs(this.config.Step2);
		if (this.config.Step1 == 0)
		{
			this.config.Step1 = 50;
		}
		if (this.config.Step2 == 0)
		{
			this.config.Step2 = 100;
		}
		if (this.config.Step2 < this.config.Step1)
		{
			Config obj = this.config;
			Config config = this.config;
			int step = this.config.Step2;
			int step2 = this.config.Step1;
			obj.Step1 = step;
			config.Step2 = step2;
		}
		this.config.Delay1 = Math.Max(0, this.config.Delay1);
		this.config.Delay2 = Math.Max(0, this.config.Delay2);
		if (this.config.Delay2 < this.config.Delay1)
		{
			Config obj2 = this.config;
			Config config = this.config;
			int step2 = this.config.Delay2;
			int step = this.config.Delay1;
			obj2.Delay1 = step2;
			config.Delay2 = step;
		}
		if (this.config.Delay2 == this.config.Delay1)
		{
			this.config.Delay2 = this.config.Delay1 + 1;
		}
		moneyButtons = new int[4]
		{
			-this.config.Step2,
			-this.config.Step1,
			this.config.Step1,
			this.config.Step2
		};
	}
}
