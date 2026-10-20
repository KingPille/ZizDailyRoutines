using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.Interop;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Dalamud.Services.Game.Object.ObjectKinds;
using OmenTools.Extensions;

namespace OmenTools.Dalamud.Services.Game.Object;

internal sealed class ObjectTable : IObjectTable, IEnumerable<IGameObject>, IEnumerable, IOmenDalamudService<IObjectTable>, IOmenDalamudService
{
	internal unsafe readonly struct CachedEntry(Pointer<GameObject>* gameObjectPtr)
	{
		private readonly PlayerCharacter playerCharacter = new PlayerCharacter(IntPtr.Zero);

		private readonly BattleNPC battleNPC = new BattleNPC(IntPtr.Zero);

		private readonly NPC npc = new NPC(IntPtr.Zero);

		private readonly EventObj eventObj = new EventObj(IntPtr.Zero);

		private readonly GameObject gameObject = new GameObject(IntPtr.Zero);

		public unsafe GameObject* Address
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return gameObjectPtr->Value;
			}
		}

		public unsafe GameObject? Update()
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Expected I4, but got Unknown
			GameObject* address = Address;
			if (address == null)
			{
				return null;
			}
			ObjectKind objectKind = (ObjectKind)((GameObject)address).ObjectKind;
			object obj = (objectKind - 1) switch
			{
				0 => playerCharacter, 
				1 => battleNPC, 
				2 => npc, 
				9 => npc, 
				6 => eventObj, 
				8 => npc, 
				7 => npc, 
				14 => npc, 
				_ => gameObject, 
			};
			((GameObject)obj).Address = (nint)address;
			return (GameObject?)obj;
		}
	}

	private struct Enumerator(ObjectTable owner) : IEnumerator<IGameObject>, IEnumerator, IDisposable
	{
		private int index = -1;

		public IGameObject Current { get; private set; } = null;

		object IEnumerator.Current => Current;

		public bool MoveNext()
		{
			Span<CachedEntry> span = owner.cachedObjectTable.AsSpan();
			while (++index < ObjectTableLength)
			{
				GameObject gameObject = span[index].Update();
				if ((object)gameObject != null)
				{
					Current = gameObject;
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

	private static int ObjectTableLength;

	private readonly CachedEntry[] cachedObjectTable;

	public unsafe nint Address => (nint)(&((GameObjectManager)GameObjectManager.Instance()).Objects);

	public int Length => ObjectTableLength;

	public IPlayerCharacter? LocalPlayer => this[0] as IPlayerCharacter;

	public IGameObject? this[int index]
	{
		get
		{
			if (index < ObjectTableLength && index >= 0)
			{
				return cachedObjectTable[index].Update();
			}
			return null;
		}
	}

	internal unsafe ObjectTable()
	{
		Span<Pointer<GameObject>> indexSorted = ((ObjectArrays)(&((GameObjectManager)GameObjectManager.Instance()).Objects)).IndexSorted;
		ObjectTableLength = indexSorted.Length;
		cachedObjectTable = new CachedEntry[ObjectTableLength];
		for (int i = 0; i < cachedObjectTable.Length; i++)
		{
			cachedObjectTable[i] = new CachedEntry(SpanExtensions.GetPointer<Pointer<GameObject>>(indexSorted, i));
		}
	}

	public IGameObject? SearchByID(ulong gameObjectID)
	{
		if (gameObjectID == 0L)
		{
			return null;
		}
		CachedEntry[] array = cachedObjectTable;
		foreach (CachedEntry cachedEntry in array)
		{
			GameObject gameObject = cachedEntry.Update();
			if ((object)gameObject != null && gameObject.GameObjectID == gameObjectID)
			{
				return gameObject;
			}
		}
		return null;
	}

	public unsafe IGameObject? SearchByID(ulong gameObjectID, Range range)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		if (gameObjectID == 0L)
		{
			return null;
		}
		(int Offset, int Length) offsetAndLength = range.GetOffsetAndLength(ObjectTableLength);
		int item = offsetAndLength.Offset;
		int item2 = offsetAndLength.Length;
		Span<CachedEntry> span = cachedObjectTable.AsSpan(item, item2);
		for (int i = 0; i < span.Length; i++)
		{
			ref CachedEntry reference = ref span[i];
			GameObject* address = reference.Address;
			if (address != null && ((GameObject)address).GetGameObjectId() == GameObjectId.op_Implicit(gameObjectID))
			{
				return reference.Update();
			}
		}
		return null;
	}

	public unsafe IGameObject? SearchByEntityID(uint entityID)
	{
		if ((entityID == 0 || entityID == 3758096384u) ? true : false)
		{
			return null;
		}
		Span<CachedEntry> span = cachedObjectTable.AsSpan();
		for (int i = 0; i < span.Length; i++)
		{
			ref CachedEntry reference = ref span[i];
			GameObject* address = reference.Address;
			if (address != null && ((GameObject)address).EntityId == entityID)
			{
				return reference.Update();
			}
		}
		return null;
	}

	public unsafe IGameObject? SearchByEntityID(uint entityID, Range range)
	{
		if ((entityID == 0 || entityID == 3758096384u) ? true : false)
		{
			return null;
		}
		(int Offset, int Length) offsetAndLength = range.GetOffsetAndLength(ObjectTableLength);
		int item = offsetAndLength.Offset;
		int item2 = offsetAndLength.Length;
		Span<CachedEntry> span = cachedObjectTable.AsSpan(item, item2);
		for (int i = 0; i < span.Length; i++)
		{
			ref CachedEntry reference = ref span[i];
			GameObject* address = reference.Address;
			if (address != null && ((GameObject)address).EntityId == entityID)
			{
				return reference.Update();
			}
		}
		return null;
	}

	public unsafe nint GetObjectAddress(int index)
	{
		if (index < ObjectTableLength && index >= 0)
		{
			return (nint)cachedObjectTable[index].Address;
		}
		return IntPtr.Zero;
	}

	public IGameObject? CreateObjectReference(nint address)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Expected I4, but got Unknown
		if (address == IntPtr.Zero)
		{
			return null;
		}
		if (!DalamudServiceExtension.Instance<IPlayerState>().IsLoaded)
		{
			return null;
		}
		ObjectKind objectKind = (ObjectKind)((GameObject)address).ObjectKind;
		return (objectKind - 1) switch
		{
			0 => new PlayerCharacter(address), 
			1 => new BattleNPC(address), 
			2 => new NPC(address), 
			9 => new NPC(address), 
			6 => new EventObj(address), 
			8 => new NPC(address), 
			7 => new NPC(address), 
			14 => new NPC(address), 
			_ => new GameObject(address), 
		};
	}

	public IEnumerable<IGameObject> SearchObjects(Predicate<IGameObject> predicate, Range range)
	{
		(int, int) offsetAndLength = range.GetOffsetAndLength(ObjectTableLength);
		int offset = offsetAndLength.Item1;
		int length = offsetAndLength.Item2;
		for (int i = 0; i < length; i++)
		{
			GameObject gameObject = cachedObjectTable[offset + i].Update();
			if ((object)gameObject != null && predicate(gameObject))
			{
				yield return gameObject;
			}
		}
	}

	public IEnumerable<IGameObject> SearchObjects(Predicate<IGameObject> predicate)
	{
		return SearchObjects(predicate, ..);
	}

	public IGameObject? SearchObject(Predicate<IGameObject> predicate, Range range)
	{
		(int Offset, int Length) offsetAndLength = range.GetOffsetAndLength(ObjectTableLength);
		int item = offsetAndLength.Offset;
		int item2 = offsetAndLength.Length;
		for (int i = 0; i < item2; i++)
		{
			GameObject gameObject = cachedObjectTable[item + i].Update();
			if ((object)gameObject != null && predicate(gameObject))
			{
				return gameObject;
			}
		}
		return null;
	}

	public IGameObject? SearchObject(Predicate<IGameObject> predicate)
	{
		return SearchObject(predicate, ..);
	}

	public IEnumerator<IGameObject> GetEnumerator()
	{
		return new Enumerator(this);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
