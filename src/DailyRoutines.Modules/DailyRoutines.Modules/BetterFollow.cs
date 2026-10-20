using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Info;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Internal;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Command;
using Dalamud.Hooking;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Control.MoveControl;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools;
using OmenTools.Dalamud;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Interop.Game;
using OmenTools.Interop.Game.Models;
using OmenTools.Interop.Game.Models.Native;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;
using OmenTools.Threading.TaskHelper;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules;

public class BetterFollow : ModuleBase
{
	private unsafe delegate byte F8D4vDpXifR3QlDY32b(PlayerController* playerController, ulong gameObjectID);

	private unsafe delegate bool RqOToHpYOtVuox2lnZ0(PlayerMoveControllerWalk* playerMoveControllerWalk);

	private class AI1o5HpzWTQWbHv3U1A : ModuleConfig
	{
		public bool FlyingWhenFollowFlying;

		public bool InCombatFollow;

		public bool MountWhenFollowMount;

		public MoveTypeList MoveType;

		public bool OnDuty;

		public bool OnlyInDuty;

		public float ReFollowDistance;

		public bool UnMountWhenFollowUnMount;

		public AI1o5HpzWTQWbHv3U1A()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			FlyingWhenFollowFlying = true;
			MountWhenFollowMount = true;
			OnDuty = true;
			ReFollowDistance = 3f;
			base._002Ector();
		}
	}

	private enum MoveTypeList
	{
		System,
		Navmesh
	}

	private enum FollowSessionState
	{
		Idle,
		Following,
		Paused,
		WaitingForTarget,
		BackendUnavailable
	}

	private enum FollowPauseReason
	{
		None,
		LocalPlayerUnavailable,
		Duty,
		NotInDuty,
		Dead,
		BetweenAreas,
		Cutscene,
		Casting,
		Combat,
		ManualInput,
		Action
	}

	private enum MountSyncState
	{
		Idle,
		Mounting,
		TakingOff,
		Dismounting
	}

	private class BxAdSW3n0tHhaNJEVuT : ContextMenuEntry
	{
		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass4_0
		{
			public ulong targetObjectID;

			public BxAdSW3n0tHhaNJEVuT _003C_003E4__this;

			public _003C_003Ec__DisplayClass4_0()
			{
				UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
				base._002Ector();
			}

			internal void KYEPkd71wp(ContextMenuItemClickedArgs _)
			{
				if (targetObjectID == 0L)
				{
					return;
				}
				if (_003C_003E4__this._003Cmodule_003EP.sessionState != FollowSessionState.Idle && _003C_003E4__this._003Cmodule_003EP.lastFollowObjectID == targetObjectID)
				{
					_003C_003E4__this._003Cmodule_003EP.oJb4kZf7Pq();
					return;
				}
				IGameObject gameObject = IOmenDalamudService<IObjectTable>.Instance().SearchByID(targetObjectID);
				if (gameObject != null)
				{
					_003C_003E4__this._003Cmodule_003EP.kV74GWrINY(gameObject);
				}
			}
		}

		public override string Identifier => LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x423E48A6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_6368ba44fa4749c09da09b5ebf853b71);

		public BxAdSW3n0tHhaNJEVuT(BetterFollow module)
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			_003Cmodule_003EP = module;
			base._002Ector();
		}

		public override ContextMenuItem? Create(ContextMenuOpenedArgs args)
		{
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			_003C_003Ec__DisplayClass4_0 CS_0024_003C_003E8__locals11 = new _003C_003Ec__DisplayClass4_0();
			CS_0024_003C_003E8__locals11._003C_003E4__this = this;
			if (!GameState.IsLoggedIn || args.TargetObjectID == 0L)
			{
				return null;
			}
			CS_0024_003C_003E8__locals11.targetObjectID = args.TargetObjectID;
			IGameObject gameObject = IOmenDalamudService<IObjectTable>.Instance().SearchByID(CS_0024_003C_003E8__locals11.targetObjectID);
			if (gameObject == null || gameObject.GameObjectID == 0 || gameObject.EntityID == LocalPlayerState.EntityID)
			{
				return null;
			}
			string text = ((_003Cmodule_003EP.sessionState != FollowSessionState.Idle && _003Cmodule_003EP.lastFollowObjectID == CS_0024_003C_003E8__locals11.targetObjectID) ? LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3951F05F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3aeae81f85f84848b273be7b2bfbf497)) : LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xE0934F7 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_aba6092d7cea44a49d1667540500e363)));
			return new ContextMenuItem
			{
				Name = ReadOnlySeString.op_Implicit(text),
				OnClicked = delegate
				{
					if (CS_0024_003C_003E8__locals11.targetObjectID != 0L)
					{
						if (CS_0024_003C_003E8__locals11._003C_003E4__this._003Cmodule_003EP.sessionState != FollowSessionState.Idle && CS_0024_003C_003E8__locals11._003C_003E4__this._003Cmodule_003EP.lastFollowObjectID == CS_0024_003C_003E8__locals11.targetObjectID)
						{
							CS_0024_003C_003E8__locals11._003C_003E4__this._003Cmodule_003EP.oJb4kZf7Pq();
						}
						else
						{
							IGameObject gameObject2 = IOmenDalamudService<IObjectTable>.Instance().SearchByID(CS_0024_003C_003E8__locals11.targetObjectID);
							if (gameObject2 != null)
							{
								CS_0024_003C_003E8__locals11._003C_003E4__this._003Cmodule_003EP.kV74GWrINY(gameObject2);
							}
						}
					}
				}
			};
		}
	}

	private static readonly CompSig SetFollowTargetSig;

	private Hook<F8D4vDpXifR3QlDY32b>? SetFollowTargetHook;

	private static readonly CompSig CheckMaxFollowDistanceSig;

	private Hook<RqOToHpYOtVuox2lnZ0>? CheckMaxFollowDistanceHook;

	private MemoryPatch? distanceCheckPatch;

	private MemoryPatch? checkObjectKindPatch;

	private AI1o5HpzWTQWbHv3U1A config;

	private BxAdSW3n0tHhaNJEVuT? contextMenuEntry;

	private FollowSessionState sessionState;

	private FollowPauseReason pauseReason;

	private MoveTypeList activeMoveType;

	private MountSyncState mountSyncState;

	private bool ownsVnavmeshMovement;

	private bool suppressNativeFollowMessage;

	private bool backendFallbackNotified;

	private long resumeAfterTimestamp;

	private long mountActionRetryTimestamp;

	private FollowPauseReason timedPauseReason;

	private ulong lastFollowObjectID;

	private string lastFollowObjectName;

	private const string COMMAND = "/pdrfollow";

	private static readonly FrozenDictionary<MoveTypeList, string> MoveTypeLoc;

	private static readonly FrozenDictionary<FollowSessionState, string> SessionStateLoc;

	private static readonly FrozenDictionary<FollowPauseReason, string> PauseReasonLoc;

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected unsafe override void Init()
	{
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Expected O, but got Unknown
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Expected O, but got Unknown
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 5000
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		config = ModuleConfigExtension.Load<AI1o5HpzWTQWbHv3U1A>(this) ?? new AI1o5HpzWTQWbHv3U1A();
		if (contextMenuEntry == null)
		{
			contextMenuEntry = new BxAdSW3n0tHhaNJEVuT(this);
		}
		OmenServiceBase<ContextMenuManager>.Instance().Reg(contextMenuEntry);
		if (SetFollowTargetHook == null)
		{
			SetFollowTargetHook = SetFollowTargetSig.GetHook<F8D4vDpXifR3QlDY32b>(X9g4zLFDmk);
		}
		SetFollowTargetHook.Enable();
		if (CheckMaxFollowDistanceHook == null)
		{
			CheckMaxFollowDistanceHook = CheckMaxFollowDistanceSig.GetHook<RqOToHpYOtVuox2lnZ0>(skRsnFWZa6);
		}
		CheckMaxFollowDistanceHook.Enable();
		distanceCheckPatch = new MemoryPatch((nint)SetFollowTargetHook.Address + 157, new global::_003C_003Ez__ReadOnlyArray<byte?>(new byte?[2]
		{
			(byte)144,
			(byte)144
		}));
		distanceCheckPatch.Enable();
		checkObjectKindPatch = new MemoryPatch((nint)SetFollowTargetHook.Address + 72, new global::_003C_003Ez__ReadOnlyArray<byte?>(new byte?[6]
		{
			(byte)144,
			(byte)144,
			(byte)144,
			(byte)144,
			(byte)144,
			(byte)144
		}));
		checkObjectKindPatch.Enable();
		config.ReFollowDistance = Math.Clamp(config.ReFollowDistance, 0f, 15f);
		if (!Enum.IsDefined(config.MoveType))
		{
			config.MoveType = MoveTypeList.System;
		}
		activeMoveType = config.MoveType;
		OmenServiceBase<UseActionManager>.Instance().RegPreUseAction(Sik4XypJuU);
		OmenServiceBase<LogMessageManager>.Instance().RegPre(g4D4Ymr5ZV);
		OmenServiceBase<CommandManager>.Instance().AddCommand(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x32DF2B3 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_86d75c161ced498ab6a767d2eb82d38e), new CommandInfo(new HandlerDelegate(kF74uRBpTN))
		{
			HelpMessage = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x9B4FCFC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2f1c0cb46b4440d499a69ed8afe22360))
		});
	}

	protected override void Uninit()
	{
		oJb4kZf7Pq(notify: false);
		if (contextMenuEntry != null)
		{
			OmenServiceBase<ContextMenuManager>.Instance().Unreg(contextMenuEntry);
		}
		contextMenuEntry = null;
		OmenServiceBase<CommandManager>.Instance().RemoveCommand(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3D56D49E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e9e296e241f44828a57c750102ce71c1));
		distanceCheckPatch?.Disable();
		distanceCheckPatch = null;
		checkObjectKindPatch?.Disable();
		checkObjectKindPatch = null;
		OmenServiceBase<UseActionManager>.Instance().Unreg(Sik4XypJuU);
		OmenServiceBase<LogMessageManager>.Instance().Unreg(g4D4Ymr5ZV);
	}

	protected override void ConfigUI()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0454: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_0516: Unknown result type (might be due to invalid IL or missing references)
		//IL_0562: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0630: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(0, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x44270C1E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8dfe57222f64fa68d9d16c8b6fe9d0b)));
		ImGui.TextColored(ref vector, val);
		IndentDisposable val2 = ImRaii.PushIndent(1, true);
		try
		{
			ImU8String val3 = new ImU8String(1, 2);
			((ImU8String)(ref val3)).AppendFormatted<string>(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4CF275CC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a07aac30eb32484e94cb4d1e62117765));
			((ImU8String)(ref val3)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2CE52DBB ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c9dc773883f149b7befba3bdbeac55f7));
			((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--1317373447 ^ 0x77D4C528 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3aeae81f85f84848b273be7b2bfbf497)));
			ImGui.TextUnformatted(val3);
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
		ImGui.NewLine();
		vector = KnownColor.Chocolate.ToVector4();
		ImU8String val4 = default(ImU8String);
		((ImU8String)(ref val4))._002Ector(1, 1);
		((ImU8String)(ref val4)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x64E54D26 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f53034f3efc64a309b10c7885bf5d9bd)));
		((ImU8String)(ref val4)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5357F3C6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_0676edd1b45a4ff096830fd6229bc1fb));
		ImGui.TextColored(ref vector, val4);
		ImGui.SameLine();
		ImU8String val5 = default(ImU8String);
		((ImU8String)(ref val5))._002Ector(0, 1);
		((ImU8String)(ref val5)).AppendFormatted<ConflictKey>(PluginConfig.Instance().ConflictKeyBinding);
		ImGui.TextUnformatted(val5);
		ImGui.NewLine();
		vector = KnownColor.Chocolate.ToVector4();
		ImU8String val6 = default(ImU8String);
		((ImU8String)(ref val6))._002Ector(0, 1);
		((ImU8String)(ref val6)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xE0987B5 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_aba6092d7cea44a49d1667540500e363)));
		ImGui.TextColored(ref vector, val6);
		ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
		val2 = ImRaii.PushIndent(1, true);
		try
		{
			ImU8String val7 = new ImU8String(31, 0);
			((ImU8String)(ref val7)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--664844326 ^ 0x6A3C7C51 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3badc14f3c904cf4809aa58db382c8ee));
			ImGui.SliderFloat(val7, ref config.ReFollowDistance, 0f, 15f, ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xE09D445 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_aba6092d7cea44a49d1667540500e363)), (ImGuiSliderFlags)0);
			if (ImGui.IsItemDeactivatedAfterEdit())
			{
				config.Save(this);
			}
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
		ImGui.NewLine();
		vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(mY24IfTYQ1()));
		ImGui.NewLine();
		vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x591C43AD ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_20724779fecb4a91959b8028d04d1118))));
		val2 = ImRaii.PushIndent(1, true);
		try
		{
			ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
			ComboDisposable val8 = ImRaii.Combo(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6C356332 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88a2dcd1996b4a3fbefaf51a98b59def)), ImU8String.op_Implicit(MoveTypeLoc[config.MoveType]));
			try
			{
				if (ComboDisposable.op_Implicit(val8))
				{
					foreach (KeyValuePair<MoveTypeList, string> item in MoveTypeLoc)
					{
						if (ImGui.Selectable(ImU8String.op_Implicit(item.Value), item.Key == config.MoveType, (ImGuiSelectableFlags)0, default(Vector2)))
						{
							config.MoveType = item.Key;
							config.Save(this);
							oSD42bPGim();
						}
					}
				}
			}
			finally
			{
				((ComboDisposable)(ref val8)).Dispose();
			}
			if (config.MoveType == MoveTypeList.Navmesh)
			{
				ImGuiOm.HelpMarker(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xB1EE8CA ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_79d931aaf1c142a2a09f53e716260ad1)), 40f, (FontAwesomeIcon)61530);
			}
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
		ImGui.NewLine();
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x258FCC14 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_dadeb2ab61424d31be7cc1966e675825))), ref config.MountWhenFollowMount))
		{
			config.Save(this);
		}
		if (config.MountWhenFollowMount)
		{
			val2 = ImRaii.PushIndent(1, true);
			try
			{
				if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x27D1EC75 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_39d5d8025f5249e6b489bc374eefbb3c))), ref config.FlyingWhenFollowFlying))
				{
					config.Save(this);
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x66B3B23A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_762f55d0e7ef4ecd8f3f55c0f8a10fae))), ref config.UnMountWhenFollowUnMount))
		{
			config.Save(this);
		}
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4B0F90C6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_8e002b0644ef421698ef6f63772929af))), ref config.InCombatFollow))
		{
			config.Save(this);
		}
		DisabledDisposable val9 = ImRaii.Disabled(config.OnlyInDuty);
		try
		{
			if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4427509C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8dfe57222f64fa68d9d16c8b6fe9d0b))), ref config.OnDuty))
			{
				config.Save(this);
			}
		}
		finally
		{
			((IDisposable)val9)?.Dispose();
		}
		val9 = ImRaii.Disabled(!config.OnDuty);
		try
		{
			if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3C14F427 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_d71d1e134fde413fa0bee3b2419181b6))), ref config.OnlyInDuty))
			{
				config.Save(this);
			}
		}
		finally
		{
			((IDisposable)val9)?.Dispose();
		}
	}

	private void kV74GWrINY(IGameObject gameObject, bool notify = true, bool stopMovement = true)
	{
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Expected O, but got Unknown
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Expected O, but got Unknown
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		if (stopMovement)
		{
			UxA46lhUrJ();
		}
		lastFollowObjectID = gameObject.GameObjectID;
		lastFollowObjectName = gameObject.Name;
		sessionState = FollowSessionState.Following;
		pauseReason = FollowPauseReason.None;
		activeMoveType = config.MoveType;
		backendFallbackNotified = false;
		resumeAfterTimestamp = 0L;
		timedPauseReason = FollowPauseReason.None;
		mountActionRetryTimestamp = 0L;
		OmenServiceBase<FrameworkManager>.Instance().Unreg(eDF4U8Zd4Q);
		OmenServiceBase<FrameworkManager>.Instance().Reg(new OnUpdateDelegate(eDF4U8Zd4Q));
		if (notify)
		{
			string text = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489BEE4C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cade14e68bb84906959f3cda5c496848), lastFollowObjectName);
			NotifyHelper.Toast(text);
			OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(text));
		}
	}

	private unsafe void UxA46lhUrJ()
	{
		PlayerController* ptr = PlayerController.Instance();
		if (ptr != null && ptr->MoveState == 4)
		{
			try
			{
				suppressNativeFollowMessage = true;
				ptr->InterruptFollow();
			}
			finally
			{
				suppressNativeFollowMessage = false;
			}
		}
		if (ownsVnavmeshMovement)
		{
			vnavmeshIPC.StopPathfind();
			vnavmeshIPC.CancelAllQueries();
			ownsVnavmeshMovement = false;
		}
		base.TaskHelper?.Abort();
		mountSyncState = MountSyncState.Idle;
		mountActionRetryTimestamp = 0L;
	}

	private void oJb4kZf7Pq(bool notify = true)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		bool flag = sessionState != FollowSessionState.Idle;
		UxA46lhUrJ();
		OmenServiceBase<FrameworkManager>.Instance().Unreg(eDF4U8Zd4Q);
		sessionState = FollowSessionState.Idle;
		pauseReason = FollowPauseReason.None;
		activeMoveType = config.MoveType;
		backendFallbackNotified = false;
		resumeAfterTimestamp = 0L;
		timedPauseReason = FollowPauseReason.None;
		lastFollowObjectID = 0uL;
		lastFollowObjectName = string.Empty;
		if (notify)
		{
			if (flag)
			{
				string text = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7FD8D079 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1eab987c1db64b928220e27bd03141d3));
				NotifyHelper.Toast(text);
				OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(text));
			}
			else
			{
				NotifyHelper.ToastError(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3D56D8E8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e9e296e241f44828a57c750102ce71c1)));
			}
		}
	}

	private void oSD42bPGim()
	{
		if (sessionState == FollowSessionState.Idle)
		{
			activeMoveType = config.MoveType;
			return;
		}
		UxA46lhUrJ();
		activeMoveType = config.MoveType;
		backendFallbackNotified = false;
		sessionState = FollowSessionState.Following;
		pauseReason = FollowPauseReason.None;
	}

	private FollowPauseReason q124ZtdAH7()
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Invalid comparison between Unknown and I4
		IPlayerCharacter playerCharacter = LocalPlayerState.Object;
		if (playerCharacter == null)
		{
			return FollowPauseReason.LocalPlayerUnavailable;
		}
		if (!config.OnDuty && IConditionExtension.get_IsBoundByDuty(DalamudServiceExtension.Instance<ICondition>()))
		{
			return FollowPauseReason.Duty;
		}
		if (config.OnlyInDuty && !IConditionExtension.get_IsBoundByDuty(DalamudServiceExtension.Instance<ICondition>()))
		{
			return FollowPauseReason.NotInDuty;
		}
		if (playerCharacter.IsDead)
		{
			return FollowPauseReason.Dead;
		}
		if (IConditionExtension.get_IsBetweenAreas(DalamudServiceExtension.Instance<ICondition>()))
		{
			return FollowPauseReason.BetweenAreas;
		}
		OnlineStatus? valueNullable = playerCharacter.OnlineStatus.ValueNullable;
		if (valueNullable.HasValue)
		{
			OnlineStatus valueOrDefault = valueNullable.GetValueOrDefault();
			if (((OnlineStatus)(ref valueOrDefault)).RowId == 15)
			{
				return FollowPauseReason.Cutscene;
			}
		}
		if (playerCharacter.IsCasting && (int)playerCharacter.CastActionType != 13)
		{
			return FollowPauseReason.Casting;
		}
		if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)26] && !config.InCombatFollow)
		{
			return FollowPauseReason.Combat;
		}
		long tickCount = Environment.TickCount64;
		if (G524aH0Z44())
		{
			resumeAfterTimestamp = tickCount + 100;
			timedPauseReason = FollowPauseReason.ManualInput;
			return timedPauseReason;
		}
		if (tickCount < resumeAfterTimestamp)
		{
			return timedPauseReason;
		}
		resumeAfterTimestamp = 0L;
		timedPauseReason = FollowPauseReason.None;
		return FollowPauseReason.None;
	}

	private unsafe bool G524aH0Z44()
	{
		PlayerController* ptr = PlayerController.Instance();
		if (ptr != null && ptr->MoveControllerWalk.IsMovementInput)
		{
			return true;
		}
		if (!sqT4D48idH())
		{
			return ((AgentMap)AgentMap.Instance()).IsPlayerMoving;
		}
		return false;
	}

	private void r9H40wvTOG(FollowPauseReason reason)
	{
		if (sessionState != FollowSessionState.Paused || pauseReason != reason)
		{
			UxA46lhUrJ();
		}
		sessionState = FollowSessionState.Paused;
		pauseReason = reason;
	}

	private void WKi45buNYx()
	{
		if (sessionState != FollowSessionState.WaitingForTarget)
		{
			UxA46lhUrJ();
		}
		sessionState = FollowSessionState.WaitingForTarget;
		pauseReason = FollowPauseReason.None;
	}

	private void mkO4R0VBsS(IGameObject followObject)
	{
		if (RFD4d7RhrP(followObject))
		{
			return;
		}
		float num = LocalPlayerState.DistanceTo3D(followObject.Position);
		bool flag = sqT4D48idH();
		if (num <= config.ReFollowDistance)
		{
			if (flag || ownsVnavmeshMovement)
			{
				UxA46lhUrJ();
			}
		}
		else
		{
			if (!flag && num <= config.ReFollowDistance + 2f)
			{
				return;
			}
			switch (activeMoveType)
			{
			case MoveTypeList.System:
				if (!flag)
				{
					OP24exktxq(followObject);
				}
				break;
			case MoveTypeList.Navmesh:
				fj34NShjXR(followObject);
				break;
			}
		}
	}

	private unsafe bool sqT4D48idH()
	{
		if (activeMoveType == MoveTypeList.System)
		{
			PlayerController* ptr = PlayerController.Instance();
			if (ptr != null)
			{
				return ptr->MoveState == 4;
			}
			return false;
		}
		if (ownsVnavmeshMovement)
		{
			if (!vnavmeshIPC.GetIsPathfindRunning())
			{
				return vnavmeshIPC.GetIsPathfindInProgress();
			}
			return true;
		}
		return false;
	}

	private unsafe bool OP24exktxq(IGameObject followObject)
	{
		PlayerController* ptr = PlayerController.Instance();
		if (ptr == null || SetFollowTargetHook == null)
		{
			sessionState = FollowSessionState.BackendUnavailable;
			return false;
		}
		try
		{
			suppressNativeFollowMessage = true;
			SetFollowTargetHook.Original(ptr, followObject.GameObjectID);
		}
		finally
		{
			suppressNativeFollowMessage = false;
		}
		sessionState = FollowSessionState.Following;
		return true;
	}

	private void fj34NShjXR(IGameObject followObject)
	{
		if (G524aH0Z44())
		{
			resumeAfterTimestamp = Environment.TickCount64 + 100;
			timedPauseReason = FollowPauseReason.ManualInput;
			r9H40wvTOG(FollowPauseReason.ManualInput);
			return;
		}
		if (!vnavmeshIPC.IsPluginEnabled() || !vnavmeshIPC.GetIsNavReady())
		{
			JCT4724hVM(followObject);
			return;
		}
		if (ownsVnavmeshMovement)
		{
			if (vnavmeshIPC.GetIsPathfindInProgress())
			{
				return;
			}
			if (vnavmeshIPC.GetIsPathfindRunning())
			{
				List<Vector3> pathfindWayPoints = vnavmeshIPC.GetPathfindWayPoints();
				if (pathfindWayPoints.Count > 0)
				{
					if (Vector3.DistanceSquared(pathfindWayPoints[pathfindWayPoints.Count - 1], followObject.Position) <= 9f)
					{
						return;
					}
				}
				vnavmeshIPC.StopPathfind();
				vnavmeshIPC.CancelAllQueries();
				ownsVnavmeshMovement = false;
			}
		}
		if (!vnavmeshIPC.PathfindAndMoveToClosely(followObject.Position, DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)77], config.ReFollowDistance))
		{
			JCT4724hVM(followObject);
			return;
		}
		ownsVnavmeshMovement = true;
		sessionState = FollowSessionState.Following;
	}

	private void JCT4724hVM(IGameObject followObject)
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		if (ownsVnavmeshMovement)
		{
			vnavmeshIPC.StopPathfind();
			vnavmeshIPC.CancelAllQueries();
			ownsVnavmeshMovement = false;
		}
		activeMoveType = MoveTypeList.System;
		if (!backendFallbackNotified)
		{
			string text = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x591C4EAD ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_20724779fecb4a91959b8028d04d1118));
			OmenServiceBase<NotifyHelper>.Instance().ChatError(ReadOnlySeString.op_Implicit(text));
			NotifyHelper.ToastError(text);
			backendFallbackNotified = true;
		}
		if (!OP24exktxq(followObject))
		{
			sessionState = FollowSessionState.BackendUnavailable;
		}
	}

	private unsafe bool RFD4d7RhrP(IGameObject followObject)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Invalid comparison between Unknown and I4
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Invalid comparison between Unknown and I4
		if ((int)followObject.ObjectKind != 1)
		{
			mountSyncState = MountSyncState.Idle;
			return false;
		}
		Character* address = (Character*)followObject.Address;
		bool flag = ((Character)address).IsMounted();
		bool flag2 = (int)((MoveController)(&((Character)address).MoveController)).MovementState == 1;
		bool flag3 = DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4];
		bool flag4 = DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)77];
		long tickCount = Environment.TickCount64;
		if ((mountSyncState == MountSyncState.Mounting && (flag3 || !flag)) || (mountSyncState == MountSyncState.TakingOff && (flag4 || !flag2)) || (mountSyncState == MountSyncState.Dismounting && (!flag3 || flag)))
		{
			mountSyncState = MountSyncState.Idle;
			mountActionRetryTimestamp = 0L;
		}
		if (mountSyncState != MountSyncState.Idle && tickCount < mountActionRetryTimestamp)
		{
			return true;
		}
		mountSyncState = MountSyncState.Idle;
		if (config.MountWhenFollowMount && flag && !flag3 && IConditionExtension.get_IsAbleToMount(DalamudServiceExtension.Instance<ICondition>()))
		{
			OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)5, 9u, 3758096384uL, 0u, (UseActionMode)0);
			mountSyncState = MountSyncState.Mounting;
			mountActionRetryTimestamp = tickCount + 3000;
			return true;
		}
		AI1o5HpzWTQWbHv3U1A aI1o5HpzWTQWbHv3U1A = config;
		if (aI1o5HpzWTQWbHv3U1A != null && aI1o5HpzWTQWbHv3U1A.MountWhenFollowMount && aI1o5HpzWTQWbHv3U1A.FlyingWhenFollowFlying && flag2 && flag3 && !flag4 && !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)27])
		{
			base.TaskHelper.Enqueue(() => OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)5, 2u, 3758096384uL, 0u, (UseActionMode)0));
			base.TaskHelper.DelayNext(50);
			base.TaskHelper.Enqueue(() => OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)5, 2u, 3758096384uL, 0u, (UseActionMode)0));
			mountSyncState = MountSyncState.TakingOff;
			mountActionRetryTimestamp = tickCount + 3000;
			return true;
		}
		if (config.UnMountWhenFollowUnMount && !flag && flag3 && LocalPlayerState.DistanceTo3D(followObject.Position) < config.ReFollowDistance + 20f)
		{
			OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)5, 9u, 3758096384uL, 0u, (UseActionMode)0);
			mountSyncState = MountSyncState.Dismounting;
			mountActionRetryTimestamp = tickCount + 3000;
			return true;
		}
		return false;
	}

	private string mY24IfTYQ1()
	{
		string text = (string.IsNullOrEmpty(lastFollowObjectName) ? LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3C14ACF7 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_d71d1e134fde413fa0bee3b2419181b6)) : lastFollowObjectName);
		string text2 = SessionStateLoc[sessionState];
		string text3 = PauseReasonLoc[pauseReason];
		string text4 = ((sessionState == FollowSessionState.Idle) ? LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x50175852 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e227921f7ee94b6aa1c28169395ba0cf)) : MoveTypeLoc[activeMoveType]);
		return LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6E00FD5C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c40146824d9043e0906f4e1889cf39aa), text, text2, text3, text4);
	}

	private void eDF4U8Zd4Q(IFramework _)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		if (sessionState == FollowSessionState.Idle)
		{
			OmenServiceBase<FrameworkManager>.Instance().Unreg(eDF4U8Zd4Q);
			return;
		}
		if (PluginConfig.Instance().ConflictKeyBinding.IsPressed())
		{
			base.TaskHelper?.Abort();
			oJb4kZf7Pq();
			return;
		}
		FollowPauseReason followPauseReason = q124ZtdAH7();
		if (followPauseReason != FollowPauseReason.None)
		{
			r9H40wvTOG(followPauseReason);
			return;
		}
		uint milliseconds = ((sessionState == FollowSessionState.WaitingForTarget) ? 500u : 50u);
		if (Throttler.Shared.Throttle(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x44275F70 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8dfe57222f64fa68d9d16c8b6fe9d0b), milliseconds))
		{
			IGameObject gameObject = IOmenDalamudService<IObjectTable>.Instance().SearchByID(lastFollowObjectID);
			if (gameObject == null || !gameObject.IsTargetable)
			{
				WKi45buNYx();
				return;
			}
			lastFollowObjectName = gameObject.Name;
			sessionState = FollowSessionState.Following;
			pauseReason = FollowPauseReason.None;
			mkO4R0VBsS(gameObject);
		}
	}

	private void kF74uRBpTN(string command, string args)
	{
		string text = args.Trim().ToLowerInvariant();
		if (!(text == LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7FD8DEB5 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1eab987c1db64b928220e27bd03141d3)))
		{
			if (text == null || text.Length != 0)
			{
				return;
			}
			IPlayerCharacter playerCharacter = LocalPlayerState.Object;
			if (playerCharacter != null)
			{
				IGameObject targetObject = playerCharacter.TargetObject;
				if (targetObject != null && targetObject.EntityID != LocalPlayerState.EntityID)
				{
					kV74GWrINY(targetObject);
					return;
				}
			}
			NotifyHelper.ToastError(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7EE7DE69 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88432e96079448078bdf00e7b62f8ab6)));
		}
		else
		{
			oJb4kZf7Pq();
		}
	}

	private void Sik4XypJuU(ref bool isPrevented, ref ActionType actionType, ref uint actionID, ref ulong targetID, ref uint extraParam, ref UseActionMode queueState, ref uint comboRouteID)
	{
		if ((int)actionType != 13 && (int)actionType != 5 && sessionState != FollowSessionState.Idle)
		{
			resumeAfterTimestamp = Environment.TickCount64 + 100;
			timedPauseReason = FollowPauseReason.Action;
			r9H40wvTOG(FollowPauseReason.Action);
		}
	}

	private void g4D4Ymr5ZV(ref bool isPrevented, ref uint logMessageID, ref LogMessageQueueItem values)
	{
		if (suppressNativeFollowMessage)
		{
			uint num = logMessageID;
			if (num - 52 <= 1)
			{
				isPrevented = true;
			}
		}
	}

	private unsafe byte X9g4zLFDmk(PlayerController* playerController, ulong gameObjectID)
	{
		UxA46lhUrJ();
		byte result = SetFollowTargetHook.Original(playerController, gameObjectID);
		IGameObject gameObject = IOmenDalamudService<IObjectTable>.Instance().SearchByID(gameObjectID);
		if (gameObject == null)
		{
			return result;
		}
		kV74GWrINY(gameObject, notify: false, stopMovement: false);
		if (config.MoveType == MoveTypeList.Navmesh)
		{
			UxA46lhUrJ();
		}
		return result;
	}

	private unsafe bool skRsnFWZa6(PlayerMoveControllerWalk* playerMoveControllerWalk)
	{
		if (activeMoveType == MoveTypeList.System && sessionState == FollowSessionState.Following)
		{
			return true;
		}
		return CheckMaxFollowDistanceHook.Original(playerMoveControllerWalk);
	}

	public BetterFollow()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x39514EE3 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3aeae81f85f84848b273be7b2bfbf497)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x1C172735 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_accf3844f1c6464481df1849f0aee1bb)),
			Category = ModuleCategory.System,
			Author = new string[2]
			{
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x21941D0C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cd1b319f326a44d1aaea645e95d8698b),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3D56D924 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e9e296e241f44828a57c750102ce71c1)
			}
		};
		Permission = new ModulePermission
		{
			NeedAuth = true
		};
		lastFollowObjectName = string.Empty;
		base._002Ector();
	}

	static BetterFollow()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		SetFollowTargetSig = new CompSig(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6FF1B740 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5859f4ea3042459db2c9b610b3099912));
		CheckMaxFollowDistanceSig = new CompSig(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3C14F939 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_d71d1e134fde413fa0bee3b2419181b6));
		MoveTypeLoc = new Dictionary<MoveTypeList, string>
		{
			[MoveTypeList.System] = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6E00FF46 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c40146824d9043e0906f4e1889cf39aa)),
			[MoveTypeList.Navmesh] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5357F470 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_0676edd1b45a4ff096830fd6229bc1fb)
		}.ToFrozenDictionary();
		SessionStateLoc = new Dictionary<FollowSessionState, string>
		{
			[FollowSessionState.Idle] = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x18CCFEF6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_33babcee9ecc460a9e530039fd18d361)),
			[FollowSessionState.Following] = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xE098841 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_aba6092d7cea44a49d1667540500e363)),
			[FollowSessionState.Paused] = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x68AC373E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4722689f1a4d41dc9dc800bfe1a89c09)),
			[FollowSessionState.WaitingForTarget] = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x477E4F3D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_ba27244341a5414496a87eae29cf3dec)),
			[FollowSessionState.BackendUnavailable] = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xF200D07 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_fea4ad1b13de4c4bbc22f571ceda7372))
		}.ToFrozenDictionary();
		PauseReasonLoc = new Dictionary<FollowPauseReason, string>
		{
			[FollowPauseReason.None] = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4B0FC986 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_8e002b0644ef421698ef6f63772929af)),
			[FollowPauseReason.LocalPlayerUnavailable] = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x18CCF80A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_33babcee9ecc460a9e530039fd18d361)),
			[FollowPauseReason.Duty] = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x68AC301A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4722689f1a4d41dc9dc800bfe1a89c09)),
			[FollowPauseReason.NotInDuty] = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x469A72D5 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_bb1eb496bad948e4a2baaa3e1edc7a92)),
			[FollowPauseReason.Dead] = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x68AC308C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4722689f1a4d41dc9dc800bfe1a89c09)),
			[FollowPauseReason.BetweenAreas] = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7C2F2380 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_12c2d68ba4654622a4959ee73cacc6d7)),
			[FollowPauseReason.Cutscene] = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6FF1B3DC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5859f4ea3042459db2c9b610b3099912)),
			[FollowPauseReason.Casting] = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2CE57B8B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c9dc773883f149b7befba3bdbeac55f7)),
			[FollowPauseReason.Combat] = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x32DFB01 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_86d75c161ced498ab6a767d2eb82d38e)),
			[FollowPauseReason.ManualInput] = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3987C2D9 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_56ae6e3f2cdf4dd8a743f4d493ce49a5)),
			[FollowPauseReason.Action] = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x454E584B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_09da05f368544962acb62b42d9155467))
		}.ToFrozenDictionary();
	}
}
