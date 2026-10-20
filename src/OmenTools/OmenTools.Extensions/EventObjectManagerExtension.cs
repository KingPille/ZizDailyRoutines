using System;
using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.Interop;

namespace OmenTools.Extensions;

public static class EventObjectManagerExtension
{
	extension(scoped ref EventObjectManager manager)
	{
		public unsafe GameObject* FindFirst(delegate*<GameObject*, bool> predicate)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			fixed (EventObjectManager* ptr = &manager)
			{
				if (ptr == null)
				{
					return null;
				}
				Span<Pointer<GameObject>> eventObjects = ((EventObjectManager)ptr).EventObjects;
				for (int i = 0; i < eventObjects.Length; i++)
				{
					Pointer<GameObject> val = eventObjects[i];
					if (val.Value != null && predicate(val.Value))
					{
						return val.Value;
					}
				}
			}
			return null;
		}

		public unsafe GameObject* FindFirst(Func<nint, bool> predicate)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			fixed (EventObjectManager* ptr = &manager)
			{
				if (ptr == null)
				{
					return null;
				}
				Span<Pointer<GameObject>> eventObjects = ((EventObjectManager)ptr).EventObjects;
				for (int i = 0; i < eventObjects.Length; i++)
				{
					Pointer<GameObject> val = eventObjects[i];
					if (val.Value != null && predicate((nint)val.Value))
					{
						return val.Value;
					}
				}
			}
			return null;
		}

		public unsafe void FindAll(delegate*<GameObject*, bool> predicate, List<nint> results)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			results.Clear();
			fixed (EventObjectManager* ptr = &manager)
			{
				if (ptr == null)
				{
					return;
				}
				Span<Pointer<GameObject>> eventObjects = ((EventObjectManager)ptr).EventObjects;
				for (int i = 0; i < eventObjects.Length; i++)
				{
					Pointer<GameObject> val = eventObjects[i];
					if (val.Value != null && predicate(val.Value))
					{
						results.Add((nint)val.Value);
					}
				}
			}
		}

		public unsafe void FindAll(Func<nint, bool> predicate, List<nint> results)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			results.Clear();
			fixed (EventObjectManager* ptr = &manager)
			{
				if (ptr == null)
				{
					return;
				}
				Span<Pointer<GameObject>> eventObjects = ((EventObjectManager)ptr).EventObjects;
				for (int i = 0; i < eventObjects.Length; i++)
				{
					Pointer<GameObject> val = eventObjects[i];
					if (val.Value != null && predicate((nint)val.Value))
					{
						results.Add((nint)val.Value);
					}
				}
			}
		}

		public unsafe List<nint> FindAll(delegate*<GameObject*, bool> predicate)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			List<nint> list = new List<nint>();
			fixed (EventObjectManager* ptr = &manager)
			{
				if (ptr == null)
				{
					return list;
				}
				Span<Pointer<GameObject>> eventObjects = ((EventObjectManager)ptr).EventObjects;
				for (int i = 0; i < eventObjects.Length; i++)
				{
					Pointer<GameObject> val = eventObjects[i];
					if (val.Value != null && predicate(val.Value))
					{
						list.Add((nint)val.Value);
					}
				}
			}
			return list;
		}

		public unsafe List<nint> FindAll(Func<nint, bool> predicate)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			List<nint> list = new List<nint>();
			fixed (EventObjectManager* ptr = &manager)
			{
				if (ptr == null)
				{
					return list;
				}
				Span<Pointer<GameObject>> eventObjects = ((EventObjectManager)ptr).EventObjects;
				for (int i = 0; i < eventObjects.Length; i++)
				{
					Pointer<GameObject> val = eventObjects[i];
					if (val.Value != null && predicate((nint)val.Value))
					{
						list.Add((nint)val.Value);
					}
				}
			}
			return list;
		}
	}
}
