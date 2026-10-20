using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Hooking;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Interop.Game;
using OmenTools.Interop.Game.Models;

namespace DailyRoutines.ModulesPublic;

public class CustomizeSightDistance : ModuleBase
{
	private unsafe delegate void SetActiveCameraDelegate(CameraManager* manager, int cameraIndex, void* a3);

	private delegate float CameraCurrentSightDistanceDelegate(nint a1, float minValue, float maxValue, float upperBound, float lowerBound, int mode, float currentValue, float targetValue);

	private unsafe delegate void SetCameraDistanceLimitDelegate(Camera* camera, float distance, float minDistance, float maxDistance);

	private class Config : ModuleConfig
	{
		public float FoV = 0.78f;

		public bool IgnoreCollision = true;

		public float MaxDistance = 80f;

		public float MaxFoV = 0.78f;

		public float MaxRotation = 1.569f;

		public float MinDistance;

		public float MinFoV = 0.69f;

		public float MinRotation = -1.569f;
	}

	private static readonly CompSig SetActiveCameraSig = new CompSig("40 57 41 54 41 57 48 83 EC ?? 4C 63 FA");

	private Hook<SetActiveCameraDelegate> SetActiveCameraHook;

	private static readonly CompSig CameraCurrentSightDistanceSig = new CompSig("40 53 48 83 EC ?? 48 8B 15 ?? ?? ?? ?? 48 8B D9 0F 29 74 24");

	private Hook<CameraCurrentSightDistanceDelegate>? CameraCurrentSightDistanceHook;

	private static readonly CompSig CameraCollisionBaseSig = new CompSig("84 C0 0F 84 ?? ?? ?? ?? F3 0F 10 44 24 ?? 41 B7");

	private MemoryPatch cameraCollisionPatch;

	private static readonly CompSig SetCameraDistanceLimitSig = new CompSig("48 8D 81 2C 01 00 00 F3 0F 11 4C 24 10 0F 2F D9");

	private Hook<SetCameraDistanceLimitDelegate> SetCameraDistanceLimitHook;

	private Config config;

	private static readonly FrozenDictionary<string, float> OriginalData = new Dictionary<string, float>
	{
		["CustomizeSightDistance-MaxDistanceInput"] = 20f,
		["CustomizeSightDistance-MinDistanceInput"] = 1.5f,
		["CustomizeSightDistance-MaxRotationInput"] = 0.785398f,
		["CustomizeSightDistance-MinRotationInput"] = -1.48353f,
		["CustomizeSightDistance-MaxFoVInput"] = 0.78f,
		["CustomizeSightDistance-MinFoVInput"] = 0.69f,
		["CustomizeSightDistance-ManualFoVInput"] = 0.78f
	}.ToFrozenDictionary();

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("CustomizeSightDistanceTitle"),
		Description = LanguageManager.Get("CustomizeSightDistanceDescription"),
		Category = ModuleCategory.System
	};

	protected unsafe override void Init()
	{
		cameraCollisionPatch = new MemoryPatch(CameraCollisionBaseSig.Get(), new global::_003C_003Ez__ReadOnlyArray<byte?>(new byte?[8]
		{
			(byte)144,
			(byte)144,
			(byte)233,
			(byte)167,
			(byte)1,
			0,
			0,
			(byte)144
		}));
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		SetActiveCameraHook = SetActiveCameraSig.GetHook<SetActiveCameraDelegate>(SetActiveCameraDetour);
		SetActiveCameraHook.Enable();
		if (CameraCurrentSightDistanceHook == null)
		{
			CameraCurrentSightDistanceHook = CameraCurrentSightDistanceSig.GetHook<CameraCurrentSightDistanceDelegate>(CameraCurrentSightDistanceDetour);
		}
		CameraCurrentSightDistanceHook.Enable();
		SetCameraDistanceLimitHook = SetCameraDistanceLimitSig.GetHook<SetCameraDistanceLimitDelegate>(SetCameraDistanceLimitDetour);
		SetCameraDistanceLimitHook.Enable();
		if (config.IgnoreCollision)
		{
			cameraCollisionPatch.Enable();
		}
		UpdateCameraByConfig();
	}

	protected unsafe override void Uninit()
	{
		if (base.IsEnabled)
		{
			UpdateCamera(((CameraManager)CameraManager.Instance()).Camera, 20f, 1.5f, 0.785398f, -1.48353f, 0.78f, 0.69f, 0.78f);
		}
	}

	protected override void ConfigUI()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		TableDisposable val = ImRaii.Table(ImU8String.op_Implicit("##SightTable"), 2, (ImGuiTableFlags)2048);
		try
		{
			if (!val)
			{
				return;
			}
			ImGui.TableSetupColumn(ImU8String.op_Implicit("Parameter"), (ImGuiTableColumnFlags)8, 0f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("Value"), (ImGuiTableColumnFlags)4, 0f, 0u);
			AddSlider("CustomizeSightDistance-MaxDistanceInput", ref config.MaxDistance, (config.MinDistance > 1f) ? config.MinDistance : 1f, 80f, "%.1f");
			AddSlider("CustomizeSightDistance-MinDistanceInput", ref config.MinDistance, 0f, config.MaxDistance, "%.1f");
			AddSlider("CustomizeSightDistance-MaxRotationInput", ref config.MaxRotation, config.MinRotation, 1.569f, "%.3f");
			AddSlider("CustomizeSightDistance-MinRotationInput", ref config.MinRotation, -1.569f, config.MaxRotation, "%.3f");
			AddSlider("CustomizeSightDistance-MaxFoVInput", ref config.MaxFoV, config.MinFoV, 3f, "%.3f");
			AddSlider("CustomizeSightDistance-MinFoVInput", ref config.MinFoV, 0.01f, config.MaxFoV, "%.3f");
			AddSlider("CustomizeSightDistance-ManualFoVInput", ref config.FoV, config.MinFoV, config.MaxFoV, "%.3f");
			ImGui.TableNextRow();
			ImGui.TableNextColumn();
			ImU8String val2 = new ImU8String(2, 1);
			((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get("CustomizeSightDistance-IgnoreCollision"));
			((ImU8String)(ref val2)).AppendLiteral(": ");
			ImGui.TextUnformatted(val2);
			ImGui.TableNextColumn();
			if (ImGui.Checkbox(ImU8String.op_Implicit("###IgnoreCollision"), ref config.IgnoreCollision))
			{
				config.Save(this);
				if (config.IgnoreCollision)
				{
					cameraCollisionPatch.Enable();
				}
				else
				{
					cameraCollisionPatch.Disable();
				}
			}
		}
		finally
		{
			((TableDisposable)(ref val)).Dispose();
		}
	}

	private unsafe void AddSlider(string label, ref float value, float min, float max, string format)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TableNextRow();
		ImGui.TableNextColumn();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(1, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get(label));
		((ImU8String)(ref val)).AppendLiteral(":");
		ImGui.TextUnformatted(val);
		ImGui.TableNextColumn();
		ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
		ImU8String val2 = default(ImU8String);
		((ImU8String)(ref val2))._002Ector(2, 1);
		((ImU8String)(ref val2)).AppendLiteral("##");
		((ImU8String)(ref val2)).AppendFormatted<string>(label);
		ImGui.SliderFloat(val2, ref value, min, max, ImU8String.op_Implicit(format), (ImGuiSliderFlags)0);
		if (ImGui.IsItemDeactivatedAfterEdit())
		{
			config.Save(this);
			UpdateCamera(((CameraManager)CameraManager.Instance()).Camera, config.MaxDistance, config.MinDistance, config.MaxRotation, config.MinRotation, config.MaxFoV, config.MinFoV, config.FoV);
		}
		ImGui.SameLine();
		if (ImGuiOm.ButtonIcon("##reset" + label, (FontAwesomeIcon)62186, LanguageManager.Get("Reset")))
		{
			value = OriginalData[label];
			config.Save(this);
			UpdateCamera(((CameraManager)CameraManager.Instance()).Camera, config.MaxDistance, config.MinDistance, config.MaxRotation, config.MinRotation, config.MaxFoV, config.MinFoV, config.FoV);
		}
	}

	private unsafe void SetCameraDistanceLimitDetour(Camera* camera, float distance, float minDistance, float maxDistance)
	{
		SetCameraDistanceLimitHook.Original(camera, ((Camera)camera).Distance, config.MinDistance, config.MaxDistance);
	}

	private unsafe void SetActiveCameraDetour(CameraManager* manager, int cameraIndex, void* a3)
	{
		SetActiveCameraHook.Original(manager, cameraIndex, a3);
		UpdateCamera(((CameraManager)manager).Camera, config.MaxDistance, config.MinDistance, config.MaxRotation, config.MinRotation, config.MaxFoV, config.MinFoV, config.FoV);
	}

	private unsafe float CameraCurrentSightDistanceDetour(nint a1, float minValue, float maxValue, float upperBound, float lowerBound, int mode, float currentValue, float targetValue)
	{
		Framework* framework = Framework.Instance();
		float num = Math.Min(upperBound - 0.001f, maxValue);
		float target = Math.Min(lowerBound - 0.001f, maxValue);
		float num2;
		switch (mode)
		{
		case 1:
			num2 = Math.Min(num, Interpolate(target, 0.3f));
			break;
		case 2:
			num2 = Interpolate(num, 0.3f);
			break;
		case 3:
			num2 = num;
			break;
		case 0:
		case 4:
		case 5:
			num2 = Interpolate(num, 0.07f);
			break;
		default:
			num2 = currentValue;
			break;
		}
		float val = num2;
		return Math.Max(Math.Min(targetValue, val), config.MinDistance);
		unsafe float Interpolate(float num3, float multiplier)
		{
			if (Math.Abs(num3 - currentValue) < 0.001f)
			{
				return num3;
			}
			float num4 = Math.Min(((Framework)framework).FrameDeltaTime * 60f * multiplier, 1f);
			if (currentValue < num3 && num3 > targetValue)
			{
				return Math.Min(currentValue + num4 * (num3 - currentValue), targetValue);
			}
			return currentValue + num4 * (num3 - currentValue);
		}
	}

	private unsafe void UpdateCameraByConfig()
	{
		UpdateCamera(((CameraManager)CameraManager.Instance()).Camera, config.MaxDistance, config.MinDistance, config.MaxRotation, config.MinRotation, config.MaxFoV, config.MinFoV, config.FoV);
	}

	private unsafe static void UpdateCamera(Camera* camera, float maxDistance, float minDistance, float maxRotation, float minRotation, float maxFoV, float minFoV, float FoV)
	{
		((Camera)camera).MinDistance = minDistance;
		((Camera)camera).MaxDistance = maxDistance;
		((float*)camera)[86] = minRotation;
		((float*)camera)[87] = maxRotation;
		((Camera)camera).MinFoV = minFoV;
		((Camera)camera).MaxFoV = maxFoV;
		((Camera)camera).FoV = FoV;
	}
}
