using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
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
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Common.Math;
using OmenTools;
using OmenTools.Dalamud.Attributes;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Info.Game.Packets.Upstream;
using OmenTools.Interop.Game.Helpers;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;

namespace DailyRoutines.ModulesPublic;

public class AutoFaceCameraDirection : ModuleBase
{
	private unsafe delegate void CameraUpdateRotationDelegate(Camera* camera);

	private unsafe delegate void* UpdateVisualRotationDelegate(GameObject* gameObject);

	private unsafe delegate void SetRotationDelegate(GameObject* gameObject, float rotation);

	private class Config : ModuleConfig
	{
		public bool WorkMode;
	}

	private static readonly CompSig cameraUpdateRotationSig = new CompSig("40 53 48 81 EC ?? ?? ?? ?? 8B 81 ?? ?? ?? ?? 48 8B D9 44 0F 29 54 24");

	private Hook<CameraUpdateRotationDelegate> CameraUpdateRotationHook;

	private static readonly CompSig updateVisualRotationSig = new CompSig("40 53 48 83 EC ?? 83 B9 ?? ?? ?? ?? ?? 48 8B D9 0F 85 ?? ?? ?? ?? F6 81");

	private UpdateVisualRotationDelegate UpdateVisualRotation;

	private static readonly CompSig setRotationSig = new CompSig("40 53 48 83 EC ?? F3 0F 10 81 ?? ?? ?? ?? 48 8B D9 0F 2E C1");

	private Hook<SetRotationDelegate>? SetRotationHook;

	private Config config;

	private float localPlayerRotationInput;

	private unsafe Camera* cacheCamera;

	private bool lockOn;

	private float lockOnRotation;

	private float cameraCharaRotation;

	private float lastSendedRotation;

	private bool isAllow;

	private long lastUpdateTick;

	private const string COMMAND = "/pdrface";

	private static readonly FrozenSet<int> ValidOpcodes;

	private static readonly FrozenSet<PositionUpdatePacket.MoveType> ValidMoveTypes;

	private static readonly FrozenSet<PositionUpdateInstancePacket.MoveType> ValidInstanceMoveTypes;

	private static readonly FrozenDictionary<string, Vector2> WorldDirectionToNormalizedDirection;

	private static readonly string GroundValuesString;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoFaceCameraDirectionTitle"),
		Description = LanguageManager.Get("AutoFaceCameraDirectionDescription"),
		Category = ModuleCategory.System,
		ModulesPair = new string[2] { "DisableGroundActionAutoFace", "IgnoreActionTargetBlocked" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		NeedAuth = true
	};

	protected unsafe override void Init()
	{
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected O, but got Unknown
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Expected O, but got Unknown
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		UpdateVisualRotation = updateVisualRotationSig.GetDelegate<UpdateVisualRotationDelegate>();
		if (CameraUpdateRotationHook == null)
		{
			CameraUpdateRotationHook = cameraUpdateRotationSig.GetHook<CameraUpdateRotationDelegate>(CameraUpdateRotationDetour);
		}
		CameraUpdateRotationHook.Enable();
		if (SetRotationHook == null)
		{
			SetRotationHook = setRotationSig.GetHook<SetRotationDelegate>(SetRotationDetour);
		}
		SetRotationHook.Enable();
		OmenServiceBase<GamePacketManager>.Instance().RegPreSendPacket(OnPreSendPacket);
		OmenServiceBase<FrameworkManager>.Instance().Reg(new OnUpdateDelegate(OnUpdate));
		OmenServiceBase<UseActionManager>.Instance().RegPostUseActionLocation(OnPostUseAction);
		CommandManager commandManager = OmenServiceBase<CommandManager>.Instance();
		CommandInfo val = new CommandInfo(new HandlerDelegate(OnCommand));
		val.HelpMessage = LanguageManager.Get("AutoFaceCameraDirection-CommandHelp", "/pdrface");
		commandManager.AddCommand("/pdrface", val);
	}

	protected override void Uninit()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Expected O, but got Unknown
		OmenServiceBase<UseActionManager>.Instance().Unreg(OnPostUseAction);
		OmenServiceBase<FrameworkManager>.Instance().Unreg(OnUpdate);
		OmenServiceBase<GamePacketManager>.Instance().Unreg(OnPreSendPacket);
		OmenServiceBase<CommandManager>.Instance().RemoveCommand("/pdrface");
	}

	protected unsafe override void ConfigUI()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Unknown result type (might be due to invalid IL or missing references)
		//IL_0447: Unknown result type (might be due to invalid IL or missing references)
		//IL_0466: Unknown result type (might be due to invalid IL or missing references)
		//IL_0479: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_050d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0512: Unknown result type (might be due to invalid IL or missing references)
		//IL_0553: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(0, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("Command"));
		ImGui.TextColored(ref vector, val);
		IndentDisposable val2 = ImRaii.PushIndent(1, true);
		try
		{
			ImU8String val3 = new ImU8String(3, 2);
			((ImU8String)(ref val3)).AppendFormatted<string>("/pdrface");
			((ImU8String)(ref val3)).AppendLiteral(" → ");
			((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get("AutoFaceCameraDirection-CommandHelp", "/pdrface"));
			ImGui.TextUnformatted(val3);
			vector = KnownColor.Chocolate.ToVector4();
			ImU8String val4 = new ImU8String(2, 1);
			((ImU8String)(ref val4)).AppendLiteral("<");
			((ImU8String)(ref val4)).AppendFormatted<string>(LanguageManager.Get("Type"));
			((ImU8String)(ref val4)).AppendLiteral(">");
			ImGui.TextColored(ref vector, val4);
			IndentDisposable val5 = ImRaii.PushIndent(1, true);
			try
			{
				ImU8String val6 = new ImU8String(9, 1);
				((ImU8String)(ref val6)).AppendLiteral("ground (");
				((ImU8String)(ref val6)).AppendFormatted<string>(LanguageManager.Get("AutoFaceCameraDirection-GroundDirection"));
				((ImU8String)(ref val6)).AppendLiteral(")");
				ImGui.TextUnformatted(val6);
				IndentDisposable val7 = ImRaii.PushIndent(1, true);
				try
				{
					ImU8String val8 = new ImU8String(2, 1);
					((ImU8String)(ref val8)).AppendLiteral("(");
					((ImU8String)(ref val8)).AppendFormatted<string>(GroundValuesString);
					((ImU8String)(ref val8)).AppendLiteral(")");
					ImGui.TextUnformatted(val8);
				}
				finally
				{
					((IDisposable)val7)?.Dispose();
				}
				ImU8String val9 = new ImU8String(8, 1);
				((ImU8String)(ref val9)).AppendLiteral("chara (");
				((ImU8String)(ref val9)).AppendFormatted<string>(LanguageManager.Get("AutoFaceCameraDirection-CharacterRotation"));
				((ImU8String)(ref val9)).AppendLiteral(")");
				ImGui.TextUnformatted(val9);
				ImU8String val10 = new ImU8String(9, 1);
				((ImU8String)(ref val10)).AppendLiteral("camera (");
				((ImU8String)(ref val10)).AppendFormatted<string>(LanguageManager.Get("AutoFaceCameraDirection-CameraRotation"));
				((ImU8String)(ref val10)).AppendLiteral(")");
				ImGui.TextUnformatted(val10);
			}
			finally
			{
				((IDisposable)val5)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
		ImGui.NewLine();
		ImGui.AlignTextToFramePadding();
		vector = KnownColor.Chocolate.ToVector4();
		ImU8String val11 = default(ImU8String);
		((ImU8String)(ref val11))._002Ector(0, 1);
		((ImU8String)(ref val11)).AppendFormatted<string>(LanguageManager.Get("WorkMode"));
		ImGui.TextColored(ref vector, val11);
		uint colorU = ImGui.GetColorU32((ImGuiCol)7);
		ImGui.SameLine();
		if (ImGuiOm.ToggleButton("WorkMode", ref config.WorkMode, 0f, colorU, colorU))
		{
			config.Save(this);
		}
		val2 = ImRaii.PushIndent(1, true);
		try
		{
			ImU8String val12 = new ImU8String(0, 1);
			((ImU8String)(ref val12)).AppendFormatted<string>(LanguageManager.Get($"AutoFaceCameraDirection-WorkMode{config.WorkMode}"));
			ImGui.TextWrapped(val12);
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
		IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
		if (localPlayer == null)
		{
			return;
		}
		ImGui.NewLine();
		vector = KnownColor.Chocolate.ToVector4();
		ImU8String val13 = default(ImU8String);
		((ImU8String)(ref val13))._002Ector(0, 1);
		((ImU8String)(ref val13)).AppendFormatted<string>(LanguageManager.Get("AutoFaceCameraDirection-GroundDirection"));
		ImGui.TextColored(ref vector, val13);
		val2 = ImRaii.PushIndent(1, true);
		try
		{
			foreach (KeyValuePair<string, Vector2> item in WorldDirectionToNormalizedDirection)
			{
				ImU8String val14 = new ImU8String(37, 1);
				((ImU8String)(ref val14)).AppendFormatted<string>(item.Key);
				((ImU8String)(ref val14)).AppendLiteral("##WorldDirectionToNormalizedDirection");
				if (ImGui.Button(val14, default(Vector2)))
				{
					SetLocalRotation((GameObject*)localPlayer.Address, RotationHelper.WorldDirHToChara(item.Value));
				}
				ImGui.SameLine();
			}
			ImGui.NewLine();
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
		vector = KnownColor.Chocolate.ToVector4();
		ImU8String val15 = default(ImU8String);
		((ImU8String)(ref val15))._002Ector(0, 1);
		((ImU8String)(ref val15)).AppendFormatted<string>(LanguageManager.Get("AutoFaceCameraDirection-CharacterRotation"));
		ImGui.TextColored(ref vector, val15);
		ItemWidthDisposable val16 = ImRaii.ItemWidth(200f * Globals.GlobalUIScale);
		try
		{
			val2 = ImRaii.PushIndent(1, true);
			try
			{
				ImU8String val17 = new ImU8String(18, 1);
				((ImU8String)(ref val17)).AppendFormatted<string>(LanguageManager.Get("Settings"));
				((ImU8String)(ref val17)).AppendLiteral("##SetCharaRotation");
				ImGui.InputFloat(val17, ref localPlayerRotationInput, 0f, 0f, ImU8String.op_Implicit("%.2f"), (ImGuiInputTextFlags)0);
				if (ImGui.IsItemDeactivatedAfterEdit())
				{
					SetLocalRotation((GameObject*)localPlayer.Address, localPlayerRotationInput);
				}
				float rotation = localPlayer.Rotation;
				ImU8String val18 = new ImU8String(22, 1);
				((ImU8String)(ref val18)).AppendFormatted<string>(LanguageManager.Get("Current"));
				((ImU8String)(ref val18)).AppendLiteral("##CurrentCharaRotation");
				ImGui.InputFloat(val18, ref rotation, 0f, 0f, ImU8String.op_Implicit("%.2f"), (ImGuiInputTextFlags)16384);
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val16)?.Dispose();
		}
		if (cacheCamera == null)
		{
			return;
		}
		vector = KnownColor.Chocolate.ToVector4();
		ImU8String val19 = default(ImU8String);
		((ImU8String)(ref val19))._002Ector(4, 2);
		((ImU8String)(ref val19)).AppendFormatted<string>(LanguageManager.Get("AutoFaceCameraDirection-CameraRotation"));
		((ImU8String)(ref val19)).AppendLiteral(" → ");
		((ImU8String)(ref val19)).AppendFormatted<string>(LanguageManager.Get("AutoFaceCameraDirection-CharacterRotation"));
		((ImU8String)(ref val19)).AppendLiteral(":");
		ImGui.TextColored(ref vector, val19);
		val2 = ImRaii.PushIndent(1, true);
		try
		{
			ImU8String val20 = new ImU8String(3, 2);
			((ImU8String)(ref val20)).AppendFormatted<float>(((Camera)cacheCamera).DirH, "F2");
			((ImU8String)(ref val20)).AppendLiteral(" → ");
			((ImU8String)(ref val20)).AppendFormatted<float>(RotationHelper.CameraDirHToChara(((Camera)cacheCamera).DirH), "F2");
			ImGui.TextUnformatted(val20);
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	private unsafe void OnCommand(string command, string args)
	{
		args = args.Trim().ToLowerInvariant();
		if (string.IsNullOrWhiteSpace(args))
		{
			NotifyCommandError();
			return;
		}
		string[] array = args.Split(' ');
		int num = array.Length;
		if ((uint)(num - 1) <= 1u)
		{
			IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
			if (localPlayer != null)
			{
				string obj = array[0];
				string text = ((array.Length == 2) ? array[1] : string.Empty);
				switch (obj)
				{
				case "ground":
				{
					if (WorldDirectionToNormalizedDirection.TryGetValue(text, out var value))
					{
						lockOnRotation = RotationHelper.WorldDirHToChara(value);
						lockOn = true;
						SetLocalRotation((GameObject*)localPlayer.Address, lockOnRotation);
						break;
					}
					goto default;
				}
				case "chara":
				{
					if (float.TryParse(text, out var result2))
					{
						lockOnRotation = result2;
						lockOn = true;
						break;
					}
					goto default;
				}
				case "camera":
				{
					if (float.TryParse(text, out var result))
					{
						lockOnRotation = RotationHelper.CameraDirHToChara(result);
						lockOn = true;
						SetLocalRotation((GameObject*)localPlayer.Address, lockOnRotation);
						break;
					}
					goto default;
				}
				case "off":
					lockOn = false;
					lockOnRotation = 0f;
					break;
				default:
					NotifyCommandError();
					return;
				}
				if (lockOn)
				{
					SetLocalRotation((GameObject*)localPlayer.Address, lockOnRotation);
					ushort currentZoneMoveState = OmenServiceBase<MovementManager>.Instance().CurrentZoneMoveState;
					if (GameState.ContentFinderCondition != 0)
					{
						PositionUpdateInstancePacket.MoveType moveType = (PositionUpdateInstancePacket.MoveType)(currentZoneMoveState << 16);
						new PositionUpdateInstancePacket(lockOnRotation, localPlayer.Position, moveType).Send();
					}
					else if (Throttler.Shared.Throttle("AutoFaceCameraDirection-UpdateRotation", 20u))
					{
						PositionUpdatePacket.MoveType moveType2 = (PositionUpdatePacket.MoveType)(currentZoneMoveState << 16);
						new PositionUpdatePacket(lockOnRotation, localPlayer.Position, moveType2).Send();
					}
				}
				return;
			}
		}
		NotifyCommandError();
		void NotifyCommandError()
		{
			OmenServiceBase<NotifyHelper>.Instance().NotificationError(LanguageManager.Get("Commands-InvalidArgs", command, args));
		}
	}

	private void OnPostUseAction(bool result, ActionType actionType, uint actionID, ulong targetID, Vector3 location, uint extraParam, byte a7)
	{
		OnUpdate(DalamudServiceExtension.Instance<IFramework>());
	}

	private unsafe void SetRotationDetour(GameObject* gameObject, float rotation)
	{
		if (gameObject == null || ((GameObject)gameObject).EntityId != LocalPlayerState.EntityID || ShouldSkipUpdate())
		{
			SetRotationHook.Original(gameObject, rotation);
			return;
		}
		((GameObject)gameObject).Rotation = rotation;
		isAllow = true;
	}

	private unsafe void OnUpdate(IFramework framework)
	{
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		if (MathF.Abs(lastSendedRotation - cameraCharaRotation) < 0.001f || cacheCamera == null)
		{
			return;
		}
		BattleChara* localPlayer = Control.GetLocalPlayer();
		if (localPlayer == null || ((BattleChara)localPlayer).Health == 0 || ShouldSkipUpdate() || DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)27])
		{
			return;
		}
		long tickCount = Environment.TickCount64;
		bool flag = GameState.ContentFinderCondition != 0;
		int num = (flag ? 33 : 100);
		SetLocalRotation((GameObject*)localPlayer, cameraCharaRotation);
		if (tickCount - lastUpdateTick >= num)
		{
			lastUpdateTick = tickCount;
			ushort currentZoneMoveState = OmenServiceBase<MovementManager>.Instance().CurrentZoneMoveState;
			if (flag)
			{
				PositionUpdateInstancePacket.MoveType moveType = (PositionUpdateInstancePacket.MoveType)(currentZoneMoveState << 16);
				new PositionUpdateInstancePacket(cameraCharaRotation, Vector3.op_Implicit(((BattleChara)localPlayer).Position), moveType).Send();
			}
			else
			{
				PositionUpdatePacket.MoveType moveType2 = (PositionUpdatePacket.MoveType)(currentZoneMoveState << 16);
				new PositionUpdatePacket(cameraCharaRotation, Vector3.op_Implicit(((BattleChara)localPlayer).Position), moveType2).Send();
			}
		}
	}

	private unsafe void CameraUpdateRotationDetour(Camera* camera)
	{
		CameraUpdateRotationHook.Original(camera);
		cacheCamera = camera;
		cameraCharaRotation = (lockOn ? lockOnRotation : RotationHelper.CameraDirHToChara(((Camera)camera).DirH));
	}

	private unsafe void OnPreSendPacket(ref bool isPrevented, int opcode, ref nint packet, ref bool isPrioritize)
	{
		if (cacheCamera == null || !ValidOpcodes.Contains(opcode) || ShouldSkipUpdate())
		{
			return;
		}
		if (opcode == UpstreamOpcode.PositionUpdateOpcode)
		{
			PositionUpdatePacket* ptr = (PositionUpdatePacket*)packet;
			if (ValidMoveTypes.Contains(ptr->Move))
			{
				if (!isAllow)
				{
					isPrevented = true;
					return;
				}
				isAllow = false;
				lastSendedRotation = ptr->Rotation;
			}
		}
		else
		{
			if (opcode != UpstreamOpcode.PositionUpdateInstanceOpcode)
			{
				return;
			}
			PositionUpdateInstancePacket* ptr2 = (PositionUpdateInstancePacket*)packet;
			if (ValidInstanceMoveTypes.Contains(ptr2->Move))
			{
				if (!isAllow)
				{
					isPrevented = true;
					return;
				}
				isAllow = false;
				lastSendedRotation = ptr2->RotationNew;
			}
		}
	}

	private unsafe void SetLocalRotation(GameObject* gameObject, float value)
	{
		if (!(MathF.Abs(((GameObject)gameObject).Rotation - value) < 0.001f))
		{
			((GameObject)gameObject).Rotation = value;
			UpdateVisualRotation(gameObject);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool ShouldSkipUpdate()
	{
		if (OmenServiceBase<MovementManager>.Instance().IsManagerBusy)
		{
			return true;
		}
		bool flag = PluginConfig.Instance().ConflictKeyBinding.IsPressed();
		if (!config.WorkMode)
		{
			return flag;
		}
		return !flag;
	}

	[IPCProvider("DailyRoutines.Modules.AutoFaceCameraDirection.SetWorkMode")]
	private void SetWorkModeIPC(bool workMode)
	{
		config.WorkMode = workMode;
	}

	[IPCProvider("DailyRoutines.Modules.AutoFaceCameraDirection.CancelLockOn")]
	private void CancelLockOnIPC()
	{
		lockOn = false;
		lockOnRotation = 0f;
	}

	[IPCProvider("DailyRoutines.Modules.AutoFaceCameraDirection.LockOnGround")]
	private bool LockOnGroundIPC(string rotation)
	{
		if (!WorldDirectionToNormalizedDirection.TryGetValue(rotation, out var value))
		{
			return false;
		}
		lockOnRotation = RotationHelper.WorldDirHToChara(value);
		lockOn = true;
		return true;
	}

	[IPCProvider("DailyRoutines.Modules.AutoFaceCameraDirection.LockOnChara")]
	private void LockOnCharaIPC(float rotation)
	{
		lockOnRotation = rotation;
		lockOn = true;
	}

	[IPCProvider("DailyRoutines.Modules.AutoFaceCameraDirection.LockOnCamera")]
	private void LockOnCameraIPC(float rotation)
	{
		lockOnRotation = RotationHelper.CameraDirHToChara(rotation);
		lockOn = true;
	}

	static AutoFaceCameraDirection()
	{
		InlineArray2<int> buffer = default(InlineArray2<int>);
		buffer[0] = UpstreamOpcode.PositionUpdateInstanceOpcode;
		buffer[1] = UpstreamOpcode.PositionUpdateOpcode;
		ValidOpcodes = FrozenSet.Create<int>(buffer);
		ValidMoveTypes = FrozenSet.Create<PositionUpdatePacket.MoveType>(PositionUpdatePacket.MoveType.NormalMove0, PositionUpdatePacket.MoveType.NormalMove1, PositionUpdatePacket.MoveType.NormalMove2, PositionUpdatePacket.MoveType.NormalMove3);
		ValidInstanceMoveTypes = FrozenSet.Create<PositionUpdateInstancePacket.MoveType>(PositionUpdateInstancePacket.MoveType.NormalMove0, PositionUpdateInstancePacket.MoveType.NormalMove1, PositionUpdateInstancePacket.MoveType.NormalMove2, PositionUpdateInstancePacket.MoveType.NormalMove3);
		WorldDirectionToNormalizedDirection = new Dictionary<string, Vector2>
		{
			["south"] = new Vector2(0f, 1f),
			["north"] = new Vector2(0f, -1f),
			["west"] = new Vector2(-1f, 0f),
			["east"] = new Vector2(1f, 0f),
			["northeast"] = new Vector2(0.707f, -0.707f),
			["southeast"] = new Vector2(0.707f, 0.707f),
			["northwest"] = new Vector2(-0.707f, -0.707f),
			["southwest"] = new Vector2(-0.707f, 0.707f)
		}.ToFrozenDictionary();
		GroundValuesString = string.Join(" / ", WorldDirectionToNormalizedDirection.Keys);
	}
}
