using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Component.GUI;
using InteropGenerator.Runtime;
using Lumina.Text.ReadOnly;

namespace OmenTools.Interop.Game.Models;

public class AtkValueArray : IDisposable
{
	public nint Address { get; }

	public unsafe AtkValue* Pointer { get; }

	public int Length { get; }

	public unsafe AtkValueArray(params object[] values)
	{
		Length = values.Length;
		Address = Marshal.AllocHGlobal(Length * Unsafe.SizeOf<AtkValue>());
		Pointer = (AtkValue*)Address;
		for (int i = 0; i < Length; i++)
		{
			EncodeValue(i, values[i]);
		}
	}

	public unsafe static implicit operator AtkValue*(AtkValueArray arr)
	{
		return arr.Pointer;
	}

	public unsafe void Dispose()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Invalid comparison between Unknown and I4
		for (int i = 0; i < Length; i++)
		{
			if ((int)((AtkValue)((byte*)Pointer + (nint)i * (nint)Unsafe.SizeOf<AtkValue>())).Type == 8)
			{
				Marshal.FreeHGlobal((nint)((CStringPointer)(&((AtkValue)((byte*)Pointer + (nint)i * (nint)Unsafe.SizeOf<AtkValue>())).String)).Value);
			}
		}
		Marshal.FreeHGlobal(Address);
	}

	private unsafe void EncodeValue(int index, object value)
	{
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		AtkValue val3 = default(AtkValue);
		if (!(value is uint uInt))
		{
			if (!(value is int num))
			{
				if (!(value is float num2))
				{
					if (!(value is bool value2))
					{
						if (!(value is string text))
						{
							if (value is ReadOnlySeString val)
							{
								RentedSeStringBuilder val2 = default(RentedSeStringBuilder);
								((RentedSeStringBuilder)(ref val2))._002Ector();
								try
								{
									byte[] array = ((RentedSeStringBuilder)(ref val2)).Builder.Append(val).GetViewAsSpan().ToArray();
									nint num3 = Marshal.AllocHGlobal(array.Length);
									Marshal.Copy(array, 0, num3, array.Length);
									byte* num4 = (byte*)Pointer + (nint)index * (nint)Unsafe.SizeOf<AtkValue>();
									val3 = new AtkValue
									{
										Type = (AtkValueType)8,
										String = CStringPointer.op_Implicit((byte*)num3)
									};
									Unsafe.Write(num4, val3);
									return;
								}
								finally
								{
									((IDisposable)(*(RentedSeStringBuilder*)(&val2))/*cast due to constrained. prefix*/).Dispose();
								}
							}
							if (!(value is AtkValue val4))
							{
								throw new ArgumentException($"Unable to convert type {value.GetType()} to AtkValue");
							}
							Unsafe.Write((byte*)Pointer + (nint)index * (nint)Unsafe.SizeOf<AtkValue>(), val4);
						}
						else
						{
							byte[] bytes = Encoding.UTF8.GetBytes(text + "\0");
							nint num5 = Marshal.AllocHGlobal(bytes.Length);
							Marshal.Copy(bytes, 0, num5, bytes.Length);
							byte* num6 = (byte*)Pointer + (nint)index * (nint)Unsafe.SizeOf<AtkValue>();
							((AtkValue)(ref val3))._002Ector();
							val3.Type = (AtkValueType)8;
							val3.String = CStringPointer.op_Implicit((byte*)num5);
							Unsafe.Write(num6, val3);
						}
					}
					else
					{
						byte* num7 = (byte*)Pointer + (nint)index * (nint)Unsafe.SizeOf<AtkValue>();
						((AtkValue)(ref val3))._002Ector();
						val3.Type = (AtkValueType)2;
						val3.Byte = Convert.ToByte(value2);
						Unsafe.Write(num7, val3);
					}
				}
				else
				{
					byte* num8 = (byte*)Pointer + (nint)index * (nint)Unsafe.SizeOf<AtkValue>();
					((AtkValue)(ref val3))._002Ector();
					val3.Type = (AtkValueType)7;
					val3.Float = num2;
					Unsafe.Write(num8, val3);
				}
			}
			else
			{
				byte* num9 = (byte*)Pointer + (nint)index * (nint)Unsafe.SizeOf<AtkValue>();
				((AtkValue)(ref val3))._002Ector();
				val3.Type = (AtkValueType)3;
				val3.Int = num;
				Unsafe.Write(num9, val3);
			}
		}
		else
		{
			byte* num10 = (byte*)Pointer + (nint)index * (nint)Unsafe.SizeOf<AtkValue>();
			((AtkValue)(ref val3))._002Ector();
			val3.Type = (AtkValueType)5;
			val3.UInt = uInt;
			Unsafe.Write(num10, val3);
		}
	}
}
