using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace OmenTools.Interop.Game.Models;

public sealed class ManagedAtkInputData : IDisposable
{
	private sealed class InputDataSafeHandle : SafeHandle
	{
		public override bool IsInvalid => handle == IntPtr.Zero;

		public InputDataSafeHandle()
			: base(IntPtr.Zero, ownsHandle: true)
		{
			SetHandle(Marshal.AllocHGlobal(64));
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

	private readonly InputDataSafeHandle handle;

	private bool disposedValue;

	public unsafe void** Data { get; }

	public unsafe AtkEventData* AtkEventData => (AtkEventData*)Data;

	private unsafe ManagedAtkInputData()
	{
		handle = new InputDataSafeHandle();
		Data = (void**)handle.DangerousGetHandle();
		if (Data == null)
		{
			throw new ArgumentNullException();
		}
		*Data = null;
		Data[1] = null;
		Data[2] = null;
		Data[3] = null;
		Data[4] = null;
		Data[5] = null;
		Data[6] = null;
		Data[7] = null;
	}

	public static ManagedAtkInputData Empty()
	{
		return new ManagedAtkInputData();
	}

	public unsafe static ManagedAtkInputData ForPopupMenu(PopupMenu* popupMenu, ushort index)
	{
		ManagedAtkInputData managedAtkInputData = new ManagedAtkInputData();
		*managedAtkInputData.Data = ((ListItem)((byte*)((AtkComponentList)((PopupMenu)popupMenu).List).ItemRendererList + (nint)(int)index * (nint)Unsafe.SizeOf<ListItem>())).AtkComponentListItemRenderer;
		managedAtkInputData.Data[2] = (void*)(index | ((ulong)index << 48));
		return managedAtkInputData;
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
