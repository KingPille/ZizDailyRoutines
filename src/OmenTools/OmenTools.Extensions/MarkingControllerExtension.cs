using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Dalamud.Memory;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using OmenTools.Info.Game.Enums;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Extensions;

public static class MarkingControllerExtension
{
	extension(scoped ref MarkingController controller)
	{
		public unsafe byte SetFieldMarker(scoped in FieldMarkerPoint point, scoped in Vector3 pos, scoped in bool isActive)
		{
			ArgumentOutOfRangeException.ThrowIfGreaterThan((uint)point, 7u, "(uint)point");
			fixed (MarkingController* ptr = &controller)
			{
				ArgumentNullException.ThrowIfNull(ptr, "controllerPtr");
				fixed (Vector3* ptr2 = &pos)
				{
					if (!isActive)
					{
						return ((MarkingController)ptr).ClearFieldMarker((uint)point);
					}
					return ((MarkingController)ptr).PlaceFieldMarker((uint)point, ptr2);
				}
			}
		}

		public unsafe void SetFieldMarkerLocal(scoped in FieldMarkerPoint point, scoped in Vector3 pos, scoped in bool isActive)
		{
			ArgumentOutOfRangeException.ThrowIfGreaterThan((uint)point, 7u, "(uint)point");
			fixed (MarkingController* ptr = &controller)
			{
				ArgumentNullException.ThrowIfNull(ptr, "controllerPtr");
				FieldMarker* ptr2 = (FieldMarker*)Unsafe.AsPointer(in ((MarkingController)ptr).FieldMarkers[(int)point]);
				nint num = (nint)ptr2;
				if (!isActive)
				{
					MemoryHelper.Write<byte>((IntPtr)(num + 28), (byte)0);
					return;
				}
				MemoryHelper.Write<float>((IntPtr)num, pos.X);
				MemoryHelper.Write<float>((IntPtr)(num + 4), pos.Y);
				MemoryHelper.Write<float>((IntPtr)(num + 8), pos.Z);
				MemoryHelper.Write<int>((IntPtr)(num + 16), (int)(pos.X * 1000f));
				MemoryHelper.Write<int>((IntPtr)(num + 20), (int)(pos.Y * 1000f));
				MemoryHelper.Write<int>((IntPtr)(num + 24), (int)(pos.Z * 1000f));
				MemoryHelper.Write<byte>((IntPtr)(num + 28), (byte)1);
			}
		}

		public unsafe Vector3 GetFieldMarkerPosition(scoped in FieldMarkerPoint point)
		{
			ArgumentOutOfRangeException.ThrowIfGreaterThan((uint)point, 7u, "(uint)point");
			fixed (MarkingController* ptr = &controller)
			{
				ArgumentNullException.ThrowIfNull(ptr, "controllerPtr");
				return ((MarkingController)ptr).FieldMarkers[(int)point].Position;
			}
		}

		public unsafe bool IsFieldMarkerActive(scoped in FieldMarkerPoint point)
		{
			ArgumentOutOfRangeException.ThrowIfGreaterThan((uint)point, 7u, "(uint)point");
			fixed (MarkingController* ptr = &controller)
			{
				ArgumentNullException.ThrowIfNull(ptr, "controllerPtr");
				return ((MarkingController)ptr).FieldMarkers[(int)point].Active;
			}
		}

		public unsafe byte PlaceFieldMarker(scoped in FieldMarkerPoint point, scoped in Vector3 pos)
		{
			ArgumentOutOfRangeException.ThrowIfGreaterThan((uint)point, 7u, "(uint)point");
			fixed (MarkingController* ptr = &controller)
			{
				ArgumentNullException.ThrowIfNull(ptr, "controllerPtr");
				return (*ptr).SetFieldMarker(in point, in pos, true);
			}
		}

		public unsafe void PlaceFieldMarkerLocal(scoped in FieldMarkerPoint point, scoped in Vector3 pos)
		{
			ArgumentOutOfRangeException.ThrowIfGreaterThan((uint)point, 7u, "(uint)point");
			fixed (MarkingController* ptr = &controller)
			{
				ArgumentNullException.ThrowIfNull(ptr, "controllerPtr");
				(*ptr).SetFieldMarkerLocal(in point, in pos, true);
			}
		}

		public void PlaceFieldMarkerOnline(scoped in FieldMarkerPoint point, scoped in Vector3 pos)
		{
			ArgumentOutOfRangeException.ThrowIfGreaterThan((uint)point, 7u, "(uint)point");
			OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.PlaceFieldMarker, (uint)point, (uint)(int)(pos.X * 1000f), (uint)(int)(pos.Y * 1000f), (uint)(int)(pos.Z * 1000f));
		}

		public unsafe byte ClearFieldMarker(scoped in FieldMarkerPoint point)
		{
			ArgumentOutOfRangeException.ThrowIfGreaterThan((uint)point, 7u, "(uint)point");
			fixed (MarkingController* ptr = &controller)
			{
				ArgumentNullException.ThrowIfNull(ptr, "controllerPtr");
				return (*ptr).SetFieldMarker(in point, Vector3.Zero, false);
			}
		}

		public unsafe void ClearFieldMarkerLocal(scoped in FieldMarkerPoint point)
		{
			ArgumentOutOfRangeException.ThrowIfGreaterThan((uint)point, 7u, "(uint)point");
			fixed (MarkingController* ptr = &controller)
			{
				ArgumentNullException.ThrowIfNull(ptr, "controllerPtr");
				(*ptr).SetFieldMarkerLocal(in point, Vector3.Zero, false);
			}
		}

		public void ClearFieldMarkerOnline(scoped in FieldMarkerPoint point)
		{
			ArgumentOutOfRangeException.ThrowIfGreaterThan((uint)point, 7u, "(uint)point");
			OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RemoveFieldMarker, (uint)point);
		}

		public void ClearFieldMarkers()
		{
			Enum.GetValues<FieldMarkerPoint>().ForEach(delegate(FieldMarkerPoint x)
			{
				x.Clear();
			});
		}

		public void ClearFieldMarkersLocal()
		{
			Enum.GetValues<FieldMarkerPoint>().ForEach(delegate(FieldMarkerPoint x)
			{
				x.ClearLocal();
			});
		}

		public void ClearFieldMarkersOnline()
		{
			Enum.GetValues<FieldMarkerPoint>().ForEach(delegate(FieldMarkerPoint x)
			{
				x.ClearOnline();
			});
		}
	}

	extension(FieldMarkerPoint point)
	{
		public unsafe Vector3 GetPosition()
		{
			return (*MarkingController.Instance()).GetFieldMarkerPosition(in point);
		}

		public unsafe bool IsActive()
		{
			return (*MarkingController.Instance()).IsFieldMarkerActive(in point);
		}

		public unsafe byte Set(Vector3 pos, bool isActive)
		{
			return (*MarkingController.Instance()).SetFieldMarker(in point, in pos, in isActive);
		}

		public unsafe void SetLocal(Vector3 pos, bool isActive)
		{
			(*MarkingController.Instance()).SetFieldMarkerLocal(in point, in pos, in isActive);
		}

		public unsafe byte Place(Vector3 pos)
		{
			return (*MarkingController.Instance()).PlaceFieldMarker(in point, in pos);
		}

		public unsafe void PlaceLocal(Vector3 pos)
		{
			(*MarkingController.Instance()).PlaceFieldMarkerLocal(in point, in pos);
		}

		public unsafe void PlaceOnline(Vector3 pos)
		{
			(*MarkingController.Instance()).PlaceFieldMarkerOnline(in point, in pos);
		}

		public unsafe byte Clear()
		{
			return (*MarkingController.Instance()).ClearFieldMarker(in point);
		}

		public unsafe void ClearLocal()
		{
			(*MarkingController.Instance()).ClearFieldMarkerLocal(in point);
		}

		public unsafe void ClearOnline()
		{
			(*MarkingController.Instance()).ClearFieldMarkerOnline(in point);
		}
	}
}
