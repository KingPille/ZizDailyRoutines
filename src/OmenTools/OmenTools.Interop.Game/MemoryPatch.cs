using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Dalamud;
using Dalamud.Plugin.Services;
using OmenTools.Extensions;

namespace OmenTools.Interop.Game;

public class MemoryPatch : IDisposable
{
	public nint Address { get; }

	public string Signature { get; }

	public byte[] NewBytes { get; }

	public byte[] OldBytes { get; }

	public bool IsEnabled { get; private set; }

	public bool IsValid => Address != IntPtr.Zero;

	public MemoryPatch(nint address, IReadOnlyCollection<byte?> bytes, bool startEnabled = false)
	{
		if (address != IntPtr.Zero)
		{
			(byte?[] TrimmedBytes, int Skip) tuple = TrimBytes(bytes);
			byte?[] item = tuple.TrimmedBytes;
			int item2 = tuple.Skip;
			Address = address + item2;
			byte[] array = default(byte[]);
			SafeMemory.ReadBytes((IntPtr)Address, item.Length, ref array);
			OldBytes = array;
			NewBytes = MergeBytes(item, OldBytes);
			DService.Instance().RegMemoryPatch(this);
			if (startEnabled)
			{
				Enable();
			}
		}
	}

	public MemoryPatch(nint address, string bytesString, bool startEnabled = false)
		: this(address, ParseByteString(bytesString), startEnabled)
	{
	}

	public MemoryPatch(string sig, IReadOnlyCollection<byte?> bytes, nint offset = 0, bool startEnabled = false)
		: this(Scan(sig) + offset, bytes, startEnabled)
	{
		Signature = sig;
	}

	public MemoryPatch(string sig, string bytesString, nint offset = 0, bool startEnabled = false)
		: this(sig, ParseByteString(bytesString), offset, startEnabled)
	{
	}

	public void Enable()
	{
		if (!IsEnabled && IsValid)
		{
			SafeMemory.WriteBytes((IntPtr)Address, NewBytes);
			IsEnabled = true;
		}
	}

	public void Disable()
	{
		if (IsEnabled && IsValid)
		{
			SafeMemory.WriteBytes((IntPtr)Address, OldBytes);
			IsEnabled = false;
		}
	}

	public void Toggle()
	{
		Set(!IsEnabled);
	}

	public void Set(bool enable)
	{
		(enable ? new Action(Enable) : new Action(Disable))();
	}

	public virtual void Dispose()
	{
		Disable();
		DService.Instance().UnregMemoryPatch(this);
		GC.SuppressFinalize(this);
	}

	private static nint Scan(string sig)
	{
		nint result = default(nint);
		if (DalamudServiceExtension.Instance<ISigScanner>().TryScanModule(sig, ref result))
		{
			return result;
		}
		DalamudServiceExtension.Instance<IPluginLog>().Warning("未能为 MemoryPatch 搜寻到签名 " + sig, Array.Empty<object>());
		return IntPtr.Zero;
	}

	private static byte?[] ParseByteString(string bytesString)
	{
		return (from chunk in bytesString.Replace(" ", "").Chunk(2)
			select (!chunk.SequenceEqual("??".AsSpan()) && !chunk.SequenceEqual("**".AsSpan())) ? new byte?(byte.Parse(new string(chunk), NumberStyles.HexNumber)) : ((byte?)null)).ToArray();
	}

	private static (byte?[] TrimmedBytes, int Skip) TrimBytes(IReadOnlyCollection<byte?> bytes)
	{
		byte?[] array = bytes.SkipWhile((byte? b) => !b.HasValue).ToArray();
		return (TrimmedBytes: array, Skip: bytes.Count - array.Length);
	}

	private static byte[] MergeBytes(byte?[] newBytes, byte[] oldBytes)
	{
		return newBytes.Select((byte? b, int i) => b ?? oldBytes[i]).ToArray();
	}
}
