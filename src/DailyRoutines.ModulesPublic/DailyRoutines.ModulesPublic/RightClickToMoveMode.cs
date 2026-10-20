using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Internal;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.System.Input;
using OmenTools;
using OmenTools.Dalamud;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.Interop.Game;
using OmenTools.Interop.Game.Helpers;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class RightClickToMoveMode : ModuleBase
{
	private sealed class Config : ModuleConfig
	{
		public VirtualKey ComboKey = (VirtualKey)16;

		public ControlMode ControlMode;

		public IndicatorStyle IndicatorStyle = IndicatorStyle.Pulse;

		public MoveMode MoveMode = MoveMode.Smart;

		public bool WASDToInterrupt = true;
	}

	private static class SessionManager
	{
		public static MoveSession? Current { get; private set; }

		public static void Start(RightClickToMoveMode module, Vector3 targetPosition)
		{
			IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
			if (localPlayer != null)
			{
				Stop(module);
				if (OmenServiceBase<LocalPlayerState>.Instance().IsMoving)
				{
					OmenServiceBase<ChatManager>.Instance().SendMessage("/automove off");
				}
				switch (module.ResolveMoveDriver(localPlayer.Position, targetPosition))
				{
				case MoveDriver.Game:
					StartGame(module, targetPosition);
					break;
				case MoveDriver.Navmesh:
					StartNavmesh(module, targetPosition);
					break;
				}
			}
		}

		public static void Stop(RightClickToMoveMode module)
		{
			MoveSession current = Current;
			Current = null;
			if (current != null)
			{
				module.movementController.Enabled = false;
				module.movementController.DesiredPosition = default(Vector3);
				vnavmeshIPC.StopPathfind();
				if (current != null && current.Driver == MoveDriver.Game)
				{
					OmenServiceBase<MovementManager>.Instance().SetCurrentControlMode(current.PreviousControlMode);
				}
			}
		}

		public static void UpdateGame(RightClickToMoveMode module, MoveSession session, Vector3 localPlayerPosition)
		{
			if (Vector2.DistanceSquared(session.Target.ToVector2(), localPlayerPosition.ToVector2()) <= 2.25f)
			{
				Stop(module);
			}
		}

		public static void UpdateNavmesh(RightClickToMoveMode module, MoveSession session, Vector3 localPlayerPosition)
		{
			if (Vector2.DistanceSquared(session.Target.ToVector2(), localPlayerPosition.ToVector2()) <= 2.25f)
			{
				Stop(module);
			}
			else if (!vnavmeshIPC.GetIsPathfindRunning() && !vnavmeshIPC.GetIsPathfindInProgress() && !vnavmeshIPC.GetIsNavPathfindInProgress() && session.ElapsedSeconds >= 0.15f)
			{
				Stop(module);
			}
		}

		private static void StartGame(RightClickToMoveMode module, Vector3 targetPosition)
		{
			MovementControlMode currentControlMode = OmenServiceBase<MovementManager>.Instance().CurrentControlMode;
			OmenServiceBase<MovementManager>.Instance().SetCurrentControlMode(MovementControlMode.Normal);
			module.movementController.DesiredPosition = targetPosition;
			module.movementController.Enabled = true;
			Current = new MoveSession(targetPosition, MoveDriver.Game, currentControlMode);
			TargetIndicatorRenderer.Trigger(module, targetPosition);
		}

		private static void StartNavmesh(RightClickToMoveMode module, Vector3 targetPosition)
		{
			module.movementController.Enabled = false;
			module.movementController.DesiredPosition = default(Vector3);
			bool fly = DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)77] || DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)81];
			if (vnavmeshIPC.PathfindAndMoveTo(targetPosition, fly))
			{
				Current = new MoveSession(targetPosition, MoveDriver.Navmesh, OmenServiceBase<MovementManager>.Instance().CurrentControlMode);
				TargetIndicatorRenderer.Trigger(module, targetPosition);
			}
		}
	}

	private sealed class MoveSession(Vector3 target, MoveDriver driver, MovementControlMode previousControlMode)
	{
		public Vector3 Target { get; } = target;

		public MoveDriver Driver { get; } = driver;

		public MovementControlMode PreviousControlMode { get; } = previousControlMode;

		public long StartedAtTicks { get; } = Environment.TickCount64;

		public float ElapsedSeconds => (float)(Environment.TickCount64 - StartedAtTicks) / 1000f;
	}

	private static class ClickTriggerEvaluator
	{
		public static bool ShouldHandle(Config config)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			return config.ControlMode switch
			{
				ControlMode.RightClick => true, 
				ControlMode.LeftRightClick => (GetAsyncKeyState(1) & 0x8000) != 0, 
				ControlMode.KeyRightClick => DalamudServiceExtension.Instance<IKeyState>()[config.ComboKey], 
				_ => false, 
			};
		}
	}

	private static class ClickPointResolver
	{
		public static bool TryResolve(Vector2 clientPosition, out Vector3 targetPosition)
		{
			targetPosition = default(Vector3);
			Vector3 vector = default(Vector3);
			if (!DalamudServiceExtension.Instance<IGameGui>().ScreenToWorld(clientPosition, ref vector, 100000f))
			{
				return false;
			}
			if (IsNavmeshAvailable())
			{
				Vector3? vector2 = vnavmeshIPC.QueryNearestPointOnMesh(vector, 3f, 10f);
				if (vector2.HasValue)
				{
					Vector3 valueOrDefault = vector2.GetValueOrDefault();
					targetPosition = valueOrDefault;
					return true;
				}
			}
			if (RaycastHelper.TryGetGroundPosition(vector, out var groundPosition))
			{
				targetPosition = groundPosition;
				return true;
			}
			return false;
		}
	}

	private static class TargetIndicatorRenderer
	{
		private static Vector3 PulseTarget;

		private static long PulseStartedAtTicks;

		private static bool IsPulseActive;

		public static void Trigger(RightClickToMoveMode module, Vector3 targetPosition)
		{
			if (module.moduleConfig.IndicatorStyle != IndicatorStyle.Pulse)
			{
				ResetPulse();
				return;
			}
			PulseTarget = targetPosition;
			PulseStartedAtTicks = Environment.TickCount64;
			IsPulseActive = true;
		}

		public static void Draw(RightClickToMoveMode module, MoveSession? session)
		{
			if (module.moduleConfig.IndicatorStyle == IndicatorStyle.None)
			{
				return;
			}
			switch (module.moduleConfig.IndicatorStyle)
			{
			case IndicatorStyle.Pulse:
				DrawPulse();
				break;
			case IndicatorStyle.Marker:
				if (session != null)
				{
					DrawMarker(session.Target);
				}
				break;
			}
		}

		public static void Reset()
		{
			ResetPulse();
		}

		private static void DrawPulse()
		{
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			if (!IsPulseActive)
			{
				return;
			}
			Vector2 vector = default(Vector2);
			if (!DalamudServiceExtension.Instance<IGameGui>().WorldToScreen(PulseTarget, ref vector))
			{
				ResetPulse();
				return;
			}
			float num = (float)(Environment.TickCount64 - PulseStartedAtTicks) / 1000f;
			if (num >= 0.45f)
			{
				ResetPulse();
				return;
			}
			float num2 = Math.Clamp(num / 0.45f, 0f, 1f);
			float num3 = 0.85f * (1f - num2);
			float num4 = (14f + 30f * num2) * Globals.GlobalUIScale;
			float num5 = MathF.Max(1.75f * Globals.GlobalUIScale, 4f * Globals.GlobalUIScale * (1f - num2 * 0.6f));
			ImDrawListPtr foregroundDrawList = ImGui.GetForegroundDrawList();
			((ImDrawListPtr)(ref foregroundDrawList)).AddCircle(vector, num4, ColorHelpers.WithAlpha(IndicatorColor, num3).ToUInt(), 32, num5);
			((ImDrawListPtr)(ref foregroundDrawList)).AddCircleFilled(vector, 4f * Globals.GlobalUIScale, ColorHelpers.WithAlpha(IndicatorInnerColor, 0.25f + num3 * 0.25f).ToUInt(), 16);
		}

		private static void DrawMarker(Vector3 targetPosition)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			Vector2 vector = default(Vector2);
			if (DalamudServiceExtension.Instance<IGameGui>().WorldToScreen(targetPosition, ref vector))
			{
				float num = 11f * Globals.GlobalUIScale;
				ImDrawListPtr foregroundDrawList = ImGui.GetForegroundDrawList();
				uint num2 = ColorHelpers.WithAlpha(IndicatorColor, 0.95f).ToUInt();
				uint num3 = ColorHelpers.WithAlpha(IndicatorInnerColor, 0.45f).ToUInt();
				float num4 = num * 0.65f;
				((ImDrawListPtr)(ref foregroundDrawList)).AddCircle(vector, num, num2, 24, 2.5f * Globals.GlobalUIScale);
				((ImDrawListPtr)(ref foregroundDrawList)).AddCircleFilled(vector, 4f * Globals.GlobalUIScale, num3, 16);
				((ImDrawListPtr)(ref foregroundDrawList)).AddLine(vector + new Vector2(0f - num4, 0f), vector + new Vector2(num4, 0f), num2, 2f * Globals.GlobalUIScale);
				((ImDrawListPtr)(ref foregroundDrawList)).AddLine(vector + new Vector2(0f, 0f - num4), vector + new Vector2(0f, num4), num2, 2f * Globals.GlobalUIScale);
			}
		}

		private static void ResetPulse()
		{
			PulseTarget = default(Vector3);
			PulseStartedAtTicks = 0L;
			IsPulseActive = false;
		}
	}

	private enum ControlMode
	{
		RightClick,
		LeftRightClick,
		KeyRightClick
	}

	private enum MoveMode
	{
		Game,
		Navmesh,
		Smart
	}

	private enum MoveDriver
	{
		Game,
		Navmesh
	}

	private enum IndicatorStyle
	{
		None,
		Pulse,
		Marker
	}

	private Config moduleConfig;

	private MovementInputController movementController;

	private const float GAME_ARRIVAL_DISTANCE_SQ = 2.25f;

	private const float NAVMESH_ARRIVAL_DISTANCE_SQ = 2.25f;

	private const float SMART_GAME_DISTANCE_SQ = 144f;

	private const float SMART_GAME_HEIGHT_DELTA = 1.5f;

	private const float PULSE_DURATION_SECONDS = 0.45f;

	private const float MARKER_RADIUS = 11f;

	private const float PULSE_START_RADIUS = 14f;

	private const float PULSE_EXPAND_RADIUS = 30f;

	private static readonly Vector4 IndicatorColor = KnownColor.Chocolate.ToVector4();

	private static readonly Vector4 IndicatorInnerColor = KnownColor.Chocolate.ToVector4();

	private static readonly FrozenDictionary<MoveMode, string> MoveModeTitles = new Dictionary<MoveMode, string>
	{
		[MoveMode.Game] = LanguageManager.Get("RightClickToMoveMode-MoveMode-Game"),
		[MoveMode.Navmesh] = LanguageManager.Get("RightClickToMoveMode-MoveMode-Navmesh"),
		[MoveMode.Smart] = LanguageManager.Get("RightClickToMoveMode-MoveMode-Smart")
	}.ToFrozenDictionary();

	private static readonly FrozenDictionary<MoveMode, string> MoveModeDescriptions = new Dictionary<MoveMode, string>
	{
		[MoveMode.Game] = LanguageManager.Get("RightClickToMoveMode-MoveMode-Game-Desc"),
		[MoveMode.Navmesh] = LanguageManager.Get("RightClickToMoveMode-MoveMode-Navmesh-Desc"),
		[MoveMode.Smart] = LanguageManager.Get("RightClickToMoveMode-MoveMode-Smart-Desc")
	}.ToFrozenDictionary();

	private static readonly FrozenDictionary<ControlMode, string> ControlModeTitles = new Dictionary<ControlMode, string>
	{
		[ControlMode.RightClick] = LanguageManager.Get("RightClickToMoveMode-RightClickMode-Title"),
		[ControlMode.LeftRightClick] = LanguageManager.Get("RightClickToMoveMode-LeftRightClickMode-Title"),
		[ControlMode.KeyRightClick] = LanguageManager.Get("RightClickToMoveMode-KeyRightClickMode-Title")
	}.ToFrozenDictionary();

	private static readonly FrozenDictionary<ControlMode, string> ControlModeDescriptions = new Dictionary<ControlMode, string>
	{
		[ControlMode.RightClick] = LanguageManager.Get("RightClickToMoveMode-RightClickMode-Desc"),
		[ControlMode.LeftRightClick] = LanguageManager.Get("RightClickToMoveMode-LeftRightClickMode-Desc"),
		[ControlMode.KeyRightClick] = LanguageManager.Get("RightClickToMoveMode-KeyRightClickMode-Desc")
	}.ToFrozenDictionary();

	private static readonly FrozenDictionary<IndicatorStyle, string> IndicatorStyleTitles = new Dictionary<IndicatorStyle, string>
	{
		[IndicatorStyle.None] = LanguageManager.Get("RightClickToMoveMode-IndicatorStyle-None"),
		[IndicatorStyle.Pulse] = LanguageManager.Get("RightClickToMoveMode-IndicatorStyle-Pulse"),
		[IndicatorStyle.Marker] = LanguageManager.Get("RightClickToMoveMode-IndicatorStyle-Marker")
	}.ToFrozenDictionary();

	private static readonly FrozenDictionary<IndicatorStyle, string> IndicatorStyleDescriptions = new Dictionary<IndicatorStyle, string>
	{
		[IndicatorStyle.None] = LanguageManager.Get("RightClickToMoveMode-IndicatorStyle-None-Desc"),
		[IndicatorStyle.Pulse] = LanguageManager.Get("RightClickToMoveMode-IndicatorStyle-Pulse-Desc"),
		[IndicatorStyle.Marker] = LanguageManager.Get("RightClickToMoveMode-IndicatorStyle-Marker-Desc")
	}.ToFrozenDictionary();

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("RightClickToMoveModeTitle"),
		Description = LanguageManager.Get("RightClickToMoveModeDescription"),
		Category = ModuleCategory.System
	};

	protected override void Init()
	{
		movementController = new MovementInputController
		{
			Precision = 0.15f,
			IsAutoMove = true
		};
		moduleConfig = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += OnZoneChanged;
		OmenServiceBase<InputIDManager>.Instance().RegPostPressed(OnPostPressed);
		OmenServiceBase<WindowManager>.Instance().PostDraw += OnDraw;
	}

	protected override void Uninit()
	{
		OmenServiceBase<InputIDManager>.Instance().UnregPostPressed(OnPostPressed);
		SessionManager.Stop(this);
		TargetIndicatorRenderer.Reset();
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChanged;
		OmenServiceBase<WindowManager>.Instance().PostDraw -= OnDraw;
		movementController.Dispose();
	}

	private void OnPostPressed(bool result, InputId id)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Invalid comparison between Unknown and I4
		if ((int)id == 37 && result)
		{
			OnMouseClickCaptured(ImGui.GetMousePos());
		}
	}

	private void OnDraw()
	{
		IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
		if (localPlayer == null)
		{
			SessionManager.Stop(this);
			TargetIndicatorRenderer.Draw(this, null);
			return;
		}
		if (IsInterruptKeysPressed())
		{
			SessionManager.Stop(this);
		}
		MoveSession current = SessionManager.Current;
		if (current != null)
		{
			switch (current.Driver)
			{
			case MoveDriver.Game:
				SessionManager.UpdateGame(this, current, localPlayer.Position);
				break;
			case MoveDriver.Navmesh:
				SessionManager.UpdateNavmesh(this, current, localPlayer.Position);
				break;
			}
		}
		TargetIndicatorRenderer.Draw(this, SessionManager.Current);
	}

	private void OnZoneChanged(uint u)
	{
		SessionManager.Stop(this);
		TargetIndicatorRenderer.Reset();
	}

	private void OnMouseClickCaptured(Vector2 clientPosition)
	{
		if (IOmenDalamudService<IObjectTable>.Instance().LocalPlayer != null && ClickTriggerEvaluator.ShouldHandle(moduleConfig) && ClickPointResolver.TryResolve(clientPosition, out var targetPosition))
		{
			SessionManager.Start(this, targetPosition);
		}
	}

	protected override void ConfigUI()
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		ImGuiOmExtension.ConflictKeyText();
		ImGui.NewLine();
		DrawMoveModeSection();
		ImGui.NewLine();
		DrawControlModeSection();
		ImGui.NewLine();
		DrawIndicatorSection();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(18, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("RightClickToMoveMode-WASDToInterrupt"));
		((ImU8String)(ref val)).AppendLiteral("###WASDToInterrupt");
		if (ImGui.Checkbox(val, ref moduleConfig.WASDToInterrupt))
		{
			moduleConfig.Save(this);
		}
	}

	private void DrawMoveModeSection()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		bool flag = IsNavmeshAvailable();
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("RightClickToMoveMode-MoveMode")));
		IndentDisposable val = ImRaii.PushIndent(1, true);
		try
		{
			ImGui.Spacing();
			MoveMode[] values = Enum.GetValues<MoveMode>();
			foreach (MoveMode moveMode in values)
			{
				bool flag2 = (uint)(moveMode - 1) <= 1u;
				DisabledDisposable val2 = ImRaii.Disabled((flag2 && !flag) || moveMode == moduleConfig.MoveMode);
				try
				{
					ImGui.SameLine();
					if (ImGui.RadioButton(ImU8String.op_Implicit(MoveModeTitles[moveMode]), moveMode == moduleConfig.MoveMode))
					{
						moduleConfig.MoveMode = moveMode;
						moduleConfig.Save(ManagerBase<ModuleManager>.Instance().GetModule<RightClickToMoveMode>());
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
			}
			ImGui.TextUnformatted(ImU8String.op_Implicit(MoveModeDescriptions[moduleConfig.MoveMode]));
			if (!flag)
			{
				ImGui.TextDisabled(ImU8String.op_Implicit(LanguageManager.Get("RightClickToMoveMode-NavmeshUnavailable")));
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void DrawControlModeSection()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("RightClickToMoveMode-ControlMode")));
		IndentDisposable val = ImRaii.PushIndent(1, true);
		try
		{
			ImGui.Spacing();
			ControlMode[] values = Enum.GetValues<ControlMode>();
			foreach (ControlMode controlMode in values)
			{
				DisabledDisposable val2 = ImRaii.Disabled(controlMode == moduleConfig.ControlMode);
				try
				{
					ImGui.SameLine();
					if (ImGui.RadioButton(ImU8String.op_Implicit(ControlModeTitles[controlMode]), controlMode == moduleConfig.ControlMode))
					{
						moduleConfig.ControlMode = controlMode;
						moduleConfig.Save(ManagerBase<ModuleManager>.Instance().GetModule<RightClickToMoveMode>());
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
			}
			ImGui.TextUnformatted(ImU8String.op_Implicit(ControlModeDescriptions[moduleConfig.ControlMode]));
			if (moduleConfig.ControlMode != ControlMode.KeyRightClick)
			{
				return;
			}
			ImGui.AlignTextToFramePadding();
			ImU8String val3 = new ImU8String(1, 1);
			((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get("RightClickToMoveMode-ComboKey"));
			((ImU8String)(ref val3)).AppendLiteral(":");
			ImGui.TextUnformatted(val3);
			ImGui.SameLine();
			ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
			ComboDisposable val4 = ImRaii.Combo(ImU8String.op_Implicit("###ComboKeyCombo"), ImU8String.op_Implicit(VirtualKeyExtensions.GetFancyName(moduleConfig.ComboKey)));
			try
			{
				if (!val4)
				{
					return;
				}
				foreach (VirtualKey validVirtualKey in DalamudServiceExtension.Instance<IKeyState>().GetValidVirtualKeys())
				{
					DisabledDisposable val5 = ImRaii.Disabled(PluginConfig.Instance().ConflictKeyBinding.Keyboard == validVirtualKey);
					try
					{
						if (ImGui.Selectable(ImU8String.op_Implicit(VirtualKeyExtensions.GetFancyName(validVirtualKey)), false, (ImGuiSelectableFlags)0, default(Vector2)))
						{
							moduleConfig.ComboKey = validVirtualKey;
							moduleConfig.Save(ManagerBase<ModuleManager>.Instance().GetModule<RightClickToMoveMode>());
						}
					}
					finally
					{
						((IDisposable)val5)?.Dispose();
					}
				}
			}
			finally
			{
				((ComboDisposable)(ref val4)).Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void DrawIndicatorSection()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("RightClickToMoveMode-IndicatorStyle")));
		IndentDisposable val = ImRaii.PushIndent(1, true);
		try
		{
			ImGui.Spacing();
			IndicatorStyle[] values = Enum.GetValues<IndicatorStyle>();
			foreach (IndicatorStyle indicatorStyle in values)
			{
				DisabledDisposable val2 = ImRaii.Disabled(indicatorStyle == moduleConfig.IndicatorStyle);
				try
				{
					ImGui.SameLine();
					if (ImGui.RadioButton(ImU8String.op_Implicit(IndicatorStyleTitles[indicatorStyle]), indicatorStyle == moduleConfig.IndicatorStyle))
					{
						moduleConfig.IndicatorStyle = indicatorStyle;
						moduleConfig.Save(ManagerBase<ModuleManager>.Instance().GetModule<RightClickToMoveMode>());
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
			}
			ImGui.TextUnformatted(ImU8String.op_Implicit(IndicatorStyleDescriptions[moduleConfig.IndicatorStyle]));
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private static bool ShouldUseGameMove(Vector3 localPlayerPosition, Vector3 targetPosition)
	{
		if (!IsNavmeshAvailable())
		{
			return true;
		}
		if (MathF.Abs(localPlayerPosition.Y - targetPosition.Y) > 1.5f)
		{
			return false;
		}
		if (RaycastHelper.HasLineOfSight(localPlayerPosition, targetPosition))
		{
			return Vector2.DistanceSquared(localPlayerPosition.ToVector2(), targetPosition.ToVector2()) <= 144f;
		}
		return false;
	}

	private MoveDriver ResolveMoveDriver(Vector3 localPlayerPosition, Vector3 targetPosition)
	{
		return moduleConfig.MoveMode switch
		{
			MoveMode.Game => MoveDriver.Game, 
			MoveMode.Navmesh => IsNavmeshAvailable() ? MoveDriver.Navmesh : MoveDriver.Game, 
			MoveMode.Smart => (!ShouldUseGameMove(localPlayerPosition, targetPosition)) ? MoveDriver.Navmesh : MoveDriver.Game, 
			_ => MoveDriver.Game, 
		};
	}

	private bool IsInterruptKeysPressed()
	{
		if (PluginConfig.Instance().ConflictKeyBinding.IsPressed())
		{
			return true;
		}
		if (moduleConfig.WASDToInterrupt)
		{
			if (!DalamudServiceExtension.Instance<IKeyState>()[(VirtualKey)87] && !DalamudServiceExtension.Instance<IKeyState>()[(VirtualKey)65] && !DalamudServiceExtension.Instance<IKeyState>()[(VirtualKey)83])
			{
				return DalamudServiceExtension.Instance<IKeyState>()[(VirtualKey)68];
			}
			return true;
		}
		return false;
	}

	private static bool IsNavmeshAvailable()
	{
		if (DalamudServiceExtension.Instance().IsPluginEnabled("vnavmesh"))
		{
			return vnavmeshIPC.GetIsNavReady();
		}
		return false;
	}

	[DllImport("user32.dll")]
	private static extern short GetAsyncKeyState(int vKey);
}
