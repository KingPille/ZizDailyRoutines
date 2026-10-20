using System;
using System.Collections.Generic;
using Dalamud;

namespace OmenTools.Interop.Game;

public class MemoryPatchWithPointer<T> : MemoryPatch where T : unmanaged
{
	private bool isPatched;

	public nint PointerAddress { get; }

	public T OriginalValue { get; private set; }

	public T CurrentValue { get; private set; }

	public void Set(T value)
	{
		if (!base.IsEnabled)
		{
			return;
		}
		if (!isPatched)
		{
			T originalValue = default(T);
			if (!SafeMemory.Read<T>((IntPtr)PointerAddress, ref originalValue))
			{
				return;
			}
			OriginalValue = originalValue;
			isPatched = true;
		}
		if (SafeMemory.Write<T>((IntPtr)PointerAddress, value))
		{
			CurrentValue = value;
		}
	}

	public void Reset()
	{
		if (isPatched && base.IsEnabled && SafeMemory.Write<T>((IntPtr)PointerAddress, OriginalValue))
		{
			CurrentValue = OriginalValue;
			isPatched = false;
		}
	}

	public override void Dispose()
	{
		Reset();
		base.Dispose();
		GC.SuppressFinalize(this);
	}

	public MemoryPatchWithPointer(nint address, IReadOnlyCollection<byte?> bytes, nint pointerOffset = 0, bool startEnabled = false)
		: base(address, bytes, startEnabled)
	{
		PointerAddress = address + pointerOffset;
	}

	public MemoryPatchWithPointer(nint address, string bytesString, nint pointerOffset = 0, bool startEnabled = false)
		: base(address, bytesString, startEnabled)
	{
		PointerAddress = address + pointerOffset;
	}

	public MemoryPatchWithPointer(string sig, IReadOnlyCollection<byte?> bytes, nint scanOffset = 0, nint pointerOffset = 0, bool startEnabled = false)
		: base(sig, bytes, scanOffset, startEnabled)
	{
		PointerAddress = base.Address + pointerOffset;
	}

	public MemoryPatchWithPointer(string sig, string bytesString, nint scanOffset = 0, nint pointerOffset = 0, bool startEnabled = false)
		: base(sig, bytesString, scanOffset, startEnabled)
	{
		PointerAddress = base.Address + pointerOffset;
	}
}
