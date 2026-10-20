using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Client.Game;
using OmenTools.Dalamud.Services.Game.Abstractions;
using OmenTools.OmenService;

namespace OmenTools.Dalamud.Services.Game;

public sealed class StatusList : IReadOnlyCollection<IStatus>, IEnumerable<IStatus>, IEnumerable, ICollection
{
	private struct Enumerator(StatusList statusList) : IEnumerator<IStatus>, IEnumerator, IDisposable
	{
		private int index = -1;

		public IStatus Current { get; private set; } = null;

		object IEnumerator.Current => Current;

		public bool MoveNext()
		{
			while (++index < statusList.Length)
			{
				IStatus status = statusList[index];
				if (status != null && status.StatusID != 0)
				{
					Current = status;
					return true;
				}
			}
			Current = null;
			return false;
		}

		public void Reset()
		{
			index = -1;
		}

		public void Dispose()
		{
		}
	}

	public nint Address { get; }

	public unsafe int Length => ((StatusManager)Struct).NumValidStatuses;

	private static int StatusSize { get; } = Marshal.SizeOf<Status>();

	private unsafe StatusManager* Struct => (StatusManager*)Address;

	public IStatus? this[int index]
	{
		get
		{
			if (index < 0 || index > Length)
			{
				return null;
			}
			return CreateStatusReference(GetStatusAddress(index));
		}
	}

	int ICollection.Count => Length;

	bool ICollection.IsSynchronized => false;

	object ICollection.SyncRoot => this;

	int IReadOnlyCollection<IStatus>.Count => Length;

	internal StatusList(nint address)
	{
		Address = address;
	}

	internal unsafe StatusList(void* pointer)
		: this((nint)pointer)
	{
	}

	public unsafe bool HasStatus(uint statusID)
	{
		for (int i = 0; i < Length; i++)
		{
			nint statusAddress = GetStatusAddress(i);
			if (statusAddress != IntPtr.Zero)
			{
				Status* ptr = (Status*)statusAddress;
				if (((Status)ptr).StatusId == statusID)
				{
					return true;
				}
			}
		}
		return false;
	}

	public unsafe bool TryGetStatus(uint statusID, out IStatus? status, out int index)
	{
		status = null;
		index = -1;
		for (int i = 0; i < Length; i++)
		{
			nint statusAddress = GetStatusAddress(i);
			if (statusAddress != IntPtr.Zero)
			{
				Status* ptr = (Status*)statusAddress;
				if (((Status)ptr).StatusId == statusID)
				{
					status = CreateStatusReference(statusAddress);
					index = i;
					return true;
				}
			}
		}
		return false;
	}

	public static StatusList? CreateStatusListReference(nint address)
	{
		if (address == IntPtr.Zero)
		{
			return null;
		}
		if (LocalPlayerState.ContentID == 0L)
		{
			return null;
		}
		if (address == 0)
		{
			return null;
		}
		return new StatusList(address);
	}

	public unsafe static IStatus? CreateStatusReference(nint address)
	{
		if (address == IntPtr.Zero)
		{
			return null;
		}
		if (address == 0)
		{
			return null;
		}
		return new Status((Status*)address);
	}

	public unsafe nint GetStatusAddress(int index)
	{
		if (index < 0 || index >= Length)
		{
			return 0;
		}
		return (nint)Unsafe.AsPointer(in ((StatusManager)Struct).Status[index]);
	}

	void ICollection.CopyTo(Array array, int index)
	{
		for (int i = 0; i < Length; i++)
		{
			array.SetValue(this[i], index);
			index++;
		}
	}

	public IEnumerator<IStatus> GetEnumerator()
	{
		return new Enumerator(this);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
