using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using Dalamud.Hooking;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.OmenService;

public class ControllerManager : OmenServiceBase<ControllerManager>
{
	private unsafe delegate byte WriteHIDDelegate(int device, void* report, ushort length);

	public class Presets
	{
		public List<Vector2> Pulsing(float duration, float maxIntensity = 1f)
		{
			List<Vector2> list = new List<Vector2>();
			if (duration <= 0f)
			{
				return list;
			}
			int num = (int)(duration / 0.5f);
			maxIntensity = Math.Clamp(maxIntensity, 0f, 1f);
			for (int i = 0; i <= num; i++)
			{
				float num2 = (float)i * 0.5f;
				float num3 = ((duration > 0f) ? (num2 / duration) : 0f);
				float num4 = 4f * (num3 - num3 * num3);
				float num5 = MathF.Abs(MathF.Sin(num2 * (float)Math.PI));
				float num6 = maxIntensity * num4 * num5;
				float x = 0.6f * num6;
				float y = 0.6f * num6;
				list.Add(new Vector2(x, y));
			}
			return list;
		}

		public List<Vector2> SmoothUp(float duration, float maxIntensity = 1f, float power = 2f)
		{
			List<Vector2> list = new List<Vector2>();
			if (duration <= 0f)
			{
				return list;
			}
			int num = (int)(duration / 0.5f);
			maxIntensity = Math.Clamp(maxIntensity, 0f, 1f);
			for (int i = 0; i <= num; i++)
			{
				float num2 = (float)i * 0.5f;
				float x = ((duration > 0f) ? (num2 / duration) : 0f);
				float num3 = maxIntensity * MathF.Pow(x, power);
				float x2 = 0.8f * num3;
				float y = 0.6f * num3;
				list.Add(new Vector2(x2, y));
			}
			return list;
		}
	}

	[StructLayout(LayoutKind.Explicit, Size = 48)]
	private struct Report
	{
		[FieldOffset(0)]
		public byte ID;

		[FieldOffset(1)]
		public Context Detail;
	}

	[StructLayout(LayoutKind.Explicit, Size = 47)]
	private struct Context
	{
		[FieldOffset(0)]
		public byte Flag0;

		[FieldOffset(1)]
		public byte Flag1;

		[FieldOffset(2)]
		public byte MotorRight;

		[FieldOffset(3)]
		public byte MotorLeft;

		[FieldOffset(4)]
		public unsafe fixed byte Reserved[4];

		[FieldOffset(8)]
		public byte MicButtonLed;

		[FieldOffset(9)]
		public byte PowerSaveControl;

		[FieldOffset(10)]
		public unsafe fixed byte TriggerR2[10];

		[FieldOffset(20)]
		public byte Unk;

		[FieldOffset(21)]
		public unsafe fixed byte TriggerL2[10];

		[FieldOffset(30)]
		public unsafe fixed byte Unk2[8];

		[FieldOffset(38)]
		public byte Flag2;

		[FieldOffset(39)]
		public unsafe fixed byte Reserved3[2];

		[FieldOffset(41)]
		public byte LightBarSetup;

		[FieldOffset(42)]
		public byte LedBrightness;

		[FieldOffset(43)]
		public byte PlayerLeds;

		[FieldOffset(44)]
		public byte LightBarColourRed;

		[FieldOffset(45)]
		public byte LightBarColourGreen;

		[FieldOffset(46)]
		public byte LightBarColourBlue;
	}

	[Flags]
	private enum ControlFlag0 : byte
	{
		None = 0,
		CompatibleVibration = 1,
		HapticsSelect = 2,
		AdapterTriggerR2Select = 4,
		AdapterTriggerL2Select = 8
	}

	[Flags]
	private enum ControlFlag1 : byte
	{
		None = 0,
		MicMuteLedControlEnable = 1,
		PowerSaveControlEnable = 2,
		LightbarControlEnable = 4,
		ReleaseLeds = 8,
		PlayerIndicatorControlEnable = 0x10
	}

	[Flags]
	private enum ControlFlag2 : byte
	{
		None = 0,
		LightbarSetupControlEnable = 2,
		CompatibleVibration2 = 4
	}

	private readonly CompSig WriteHidSig = new CompSig("E8 ?? ?? ?? ?? 8B 4B 1C 8B F8");

	private Hook<WriteHIDDelegate>? WriteHidHook;

	private readonly Queue<Vector2> MotorQueue = new Queue<Vector2>();

	private Vector3 Color;

	private Vector2 MotorCustom;

	public void SetColor(Vector3 rgb)
	{
		Color = rgb;
	}

	public void SetMotor(Vector2 motor)
	{
		MotorCustom = motor;
	}

	public void StartMotorQueue(List<Vector2> motors)
	{
		MotorQueue.Clear();
		foreach (Vector2 motor in motors)
		{
			MotorQueue.Enqueue(motor);
		}
	}

	public void AbortMotorQueue()
	{
		MotorQueue.Clear();
	}

	protected unsafe override void Init()
	{
		if (WriteHidHook == null)
		{
			WriteHidHook = WriteHidSig.GetHook<WriteHIDDelegate>(WriteHIDDetour);
		}
		WriteHidHook.Enable();
	}

	protected override void Uninit()
	{
		WriteHidHook?.Dispose();
		WriteHidHook = null;
	}

	private unsafe byte WriteHIDDetour(int device, void* reportPtr, ushort length)
	{
		if (length != 48)
		{
			return WriteHidHook.Original(device, reportPtr, length);
		}
		if (Color != Vector3.Zero)
		{
			((Report*)reportPtr)->Detail.Flag1 = 4;
			((Report*)reportPtr)->Detail.LightBarColourRed = (byte)Color.X;
			((Report*)reportPtr)->Detail.LightBarColourGreen = (byte)Color.Y;
			((Report*)reportPtr)->Detail.LightBarColourBlue = (byte)Color.Z;
		}
		if (MotorQueue.Count > 0)
		{
			((Report*)reportPtr)->Detail.Flag0 = 2;
			((Report*)reportPtr)->Detail.Flag2 = 4;
			Vector2 vector = MotorQueue.Dequeue();
			((Report*)reportPtr)->Detail.MotorLeft = (byte)(vector.X * 255f);
			((Report*)reportPtr)->Detail.MotorRight = (byte)(vector.Y * 255f);
		}
		if (MotorCustom != Vector2.Zero)
		{
			((Report*)reportPtr)->Detail.Flag0 = 2;
			((Report*)reportPtr)->Detail.Flag2 = 4;
			((Report*)reportPtr)->Detail.MotorLeft = (byte)(MotorCustom.X * 255f);
			((Report*)reportPtr)->Detail.MotorRight = (byte)(MotorCustom.Y * 255f);
		}
		return WriteHidHook.Original(device, reportPtr, length);
	}
}
