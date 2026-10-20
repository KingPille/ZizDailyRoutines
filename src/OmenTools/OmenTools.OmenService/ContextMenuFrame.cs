using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace OmenTools.OmenService;

internal unsafe sealed class ContextMenuFrame(int valueCount) : IDisposable
{
	private int references = 1;

	public unsafe AtkValue* Values { get; private set; } = (AtkValue*)NativeMemory.AllocZeroed((nuint)valueCount, (nuint)Unsafe.SizeOf<AtkValue>());

	public int ValueCount { get; } = valueCount;

	public uint AddonNameID { get; init; }

	public ushort AddonID { get; set; }

	public unsafe AgentInterface* Agent { get; init; }

	public ulong EventKind { get; init; }

	public ushort OwnerAddonID { get; init; }

	public int DepthLayer { get; init; }

	public bool IsSubmenu { get; init; }

	public int SelectedIndex { get; set; }

	public short X { get; set; }

	public short Y { get; set; }

	public ContextMenuItem[] Items { get; init; } = Array.Empty<ContextMenuItem>();

	public int[] CallbackIDs { get; init; } = Array.Empty<int>();

	public void Retain()
	{
		references++;
	}

	public unsafe void CopyValue(int index, AtkValue* source)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Invalid comparison between Unknown and I4
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Invalid comparison between Unknown and I4
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		AtkValueType val = (AtkValueType)(((AtkValue)source).Type & 0xF);
		if (((int)val == 8 || (int)val == 10) ? true : false)
		{
			((AtkValue)((byte*)Values + (nint)index * (nint)Unsafe.SizeOf<AtkValue>())).SetManagedString(((AtkValue)source).String);
		}
		else
		{
			((AtkValue)((byte*)Values + (nint)index * (nint)Unsafe.SizeOf<AtkValue>())).Copy(source);
		}
	}

	public unsafe void Dispose()
	{
		if (Values != null && --references <= 0)
		{
			for (int i = 0; i < ValueCount; i++)
			{
				((AtkValue)((byte*)Values + (nint)i * (nint)Unsafe.SizeOf<AtkValue>())).Dtor();
			}
			NativeMemory.Free(Values);
			Values = null;
		}
	}
}
