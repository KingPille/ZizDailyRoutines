using System;
using System.Numerics;
using Dalamud.Game.ClientState.Objects.Enums;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Common.Math;
using OmenTools.Dalamud.Services.Game.Event;
using OmenTools.Dalamud.Services.Game.Event.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Dalamud.Services.Graphics.Scene;
using OmenTools.Dalamud.Services.Graphics.Scene.Abstractions;
using OmenTools.Dalamud.Services.LayoutEngine.Group;
using OmenTools.Dalamud.Services.LayoutEngine.Group.Abstractions;
using OmenTools.OmenService;

namespace OmenTools.Dalamud.Services.Game.Object.ObjectKinds;

internal class GameObject(nint address) : IGameObject, IEquatable<IGameObject>
{
	protected internal unsafe GameObject* Struct => (GameObject*)Address;

	public unsafe string Name => ((GameObject)Struct).NameString;

	public unsafe ulong GameObjectID => GameObjectId.op_Implicit(((GameObject)Struct).GetGameObjectId());

	public unsafe uint EntityID => ((GameObject)Struct).EntityId;

	public unsafe byte EventState => ((GameObject)Struct).EventState;

	public unsafe uint LayoutID => ((GameObject)Struct).LayoutId;

	public unsafe uint GimmickID => ((GameObject)Struct).GimmickId;

	public unsafe uint DataID => ((GameObject)Struct).BaseId;

	public unsafe uint OwnerID => ((GameObject)Struct).OwnerId;

	public unsafe ushort ObjectIndex => ((GameObject)Struct).ObjectIndex;

	public unsafe ObjectKind ObjectKind => (ObjectKind)((GameObject)Struct).ObjectKind;

	public unsafe byte SubKind => ((GameObject)Struct).SubKind;

	public unsafe BattleNpcSubKind BattleNPCSubKind => ((GameObject)Struct).BattleNpcSubKind;

	public unsafe byte CurrentTargetStatus => ((GameObject)Struct).CurrentTargetStatus;

	public unsafe byte CurrentDistance => ((GameObject)Struct).CurrentDistance;

	public unsafe byte NextTargetStatus => ((GameObject)Struct).NextTargetStatus;

	public unsafe byte NextDistance => ((GameObject)Struct).NextDistance;

	public unsafe byte Visibility => ((GameObject)Struct).Visibility;

	public unsafe Vector3 Position => new Vector3(((Vector3)(&((GameObject)Struct).Position)).X, ((Vector3)(&((GameObject)Struct).Position)).Y, ((Vector3)(&((GameObject)Struct).Position)).Z);

	public unsafe float Rotation => ((GameObject)Struct).Rotation;

	public unsafe float HitboxRadius => ((GameObject)Struct).HitboxRadius;

	public unsafe uint NamePlateIconID => ((GameObject)Struct).NamePlateIconId;

	public unsafe ushort FateID => ((GameObject)Struct).FateId;

	public unsafe EventId EventID => ((GameObject)Struct).EventId;

	public unsafe IDrawObject? DrawObject
	{
		get
		{
			if (((GameObject)Struct).DrawObject != null)
			{
				return new DrawObject((nint)((GameObject)Struct).DrawObject);
			}
			return null;
		}
	}

	public unsafe ISharedGroupLayoutInstance? SharedGroupLayoutInstance
	{
		get
		{
			if (((GameObject)Struct).SharedGroupLayoutInstance != null)
			{
				return new SharedGroupLayoutInstance((nint)((GameObject)Struct).SharedGroupLayoutInstance);
			}
			return null;
		}
	}

	public unsafe ILuaActor? LuaActor
	{
		get
		{
			if (((GameObject)Struct).LuaActor != null)
			{
				return new LuaActor((nint)((GameObject)Struct).LuaActor);
			}
			return null;
		}
	}

	public unsafe IEventHandler? EventHandler
	{
		get
		{
			if (((GameObject)Struct).EventHandler != null)
			{
				return new OmenTools.Dalamud.Services.Game.Event.EventHandler((nint)((GameObject)Struct).EventHandler);
			}
			return null;
		}
	}

	public unsafe float Scale => ((GameObject)Struct).Scale;

	public unsafe float VfxScale => ((GameObject)Struct).VfxScale;

	public unsafe ObjectTargetableFlags TargetableStatus => ((GameObject)Struct).TargetableStatus;

	public unsafe ObjectUpdateFlags UpdateFlags => ((GameObject)Struct).UpdateFlags;

	public unsafe byte TargetStatus => ((GameObject)Struct).NextTargetStatus;

	public unsafe Vector3 DrawOffset => Vector3.op_Implicit(((GameObject)Struct).DrawOffset);

	public unsafe float Height => ((GameObject)Struct).Height;

	public unsafe byte Sex => ((GameObject)Struct).Sex;

	public unsafe VisibilityFlags RenderFlags => ((GameObject)Struct).RenderFlags;

	public unsafe Vector3 DefaultPosition => Vector3.op_Implicit(((GameObject)Struct).DefaultPosition);

	public unsafe float DefaultRotation => ((GameObject)Struct).DefaultRotation;

	public unsafe Vector3 NameplateOffset => Vector3.op_Implicit(((GameObject)Struct).NameplateOffset);

	public unsafe Vector3 CameraOffset => Vector3.op_Implicit(((GameObject)Struct).CameraOffset);

	public unsafe float NameplateOffsetScaleMultiplier => ((GameObject)Struct).NameplateOffsetScaleMultiplier;

	public unsafe Vector3 NameplateOffsetTarget => Vector3.op_Implicit(((GameObject)Struct).NameplateOffsetTarget);

	public unsafe Vector3 CameraOffsetTarget => Vector3.op_Implicit(((GameObject)Struct).CameraOffsetTarget);

	public unsafe bool IsDead => ((GameObject)Struct).IsDead();

	public unsafe bool IsTargetable => ((GameObject)Struct).GetIsTargetable();

	public unsafe byte Distance => ((GameObject)Struct).NextDistance;

	public virtual ulong TargetObjectID => 0uL;

	public virtual IGameObject? TargetObject => IOmenDalamudService<IObjectTable>.Instance().SearchByID(TargetObjectID);

	public nint Address { get; internal set; } = address;

	public bool IsValid()
	{
		return IsValid(this);
	}

	public unsafe GameObject* ToStruct()
	{
		return Struct;
	}

	public unsafe BattleChara* ToBCStruct()
	{
		return (BattleChara*)Struct;
	}

	bool IEquatable<IGameObject>.Equals(IGameObject? other)
	{
		return GameObjectID == other?.GameObjectID;
	}

	public static implicit operator bool(GameObject? gameObject)
	{
		return IsValid(gameObject);
	}

	public static bool operator ==(GameObject? gameObject1, GameObject? gameObject2)
	{
		if ((object)gameObject1 == null || (object)gameObject2 == null)
		{
			return object.Equals(gameObject1, gameObject2);
		}
		return gameObject1.Equals(gameObject2);
	}

	public static bool operator !=(GameObject? actor1, GameObject? actor2)
	{
		return !(actor1 == actor2);
	}

	public static bool IsValid(IGameObject? actor)
	{
		if (actor != null && LocalPlayerState.ContentID != 0L && actor.Address != IntPtr.Zero)
		{
			return actor.Address != (nint)0;
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		return ((IEquatable<IGameObject>)this).Equals(obj as IGameObject);
	}

	public override int GetHashCode()
	{
		return GameObjectID.GetHashCode();
	}

	public override string ToString()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		return $"{GameObjectID:X}({Name} - {ObjectKind}) Address: {Address:X}";
	}
}
