using System;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace OmenTools.Interop.Game.Models;

public sealed class ManagedAtkEvent : IDisposable
{
	private sealed class EventDataSafeHandle : SafeHandle
	{
		public override bool IsInvalid => handle == IntPtr.Zero;

		public EventDataSafeHandle()
			: base(IntPtr.Zero, ownsHandle: true)
		{
			SetHandle(Marshal.AllocHGlobal(24));
		}

		protected override bool ReleaseHandle()
		{
			if (!IsInvalid)
			{
				Marshal.FreeHGlobal(handle);
			}
			return true;
		}
	}

	private readonly EventDataSafeHandle handle;

	private bool disposedValue;

	public unsafe void** Data { get; }

	public unsafe AtkEvent* AtkEvent => (AtkEvent*)Data;

	private unsafe ManagedAtkEvent()
	{
		handle = new EventDataSafeHandle();
		Data = (void**)handle.DangerousGetHandle();
		if (Data == null)
		{
			throw new ArgumentNullException();
		}
		*Data = null;
		Data[1] = null;
		Data[2] = null;
	}

	public unsafe static ManagedAtkEvent ForNormalTarget(void* target, void* listener)
	{
		ManagedAtkEvent managedAtkEvent = new ManagedAtkEvent();
		managedAtkEvent.Data[1] = target;
		managedAtkEvent.Data[2] = listener;
		return managedAtkEvent;
	}

	private void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				handle.Dispose();
			}
			disposedValue = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
	}
}
