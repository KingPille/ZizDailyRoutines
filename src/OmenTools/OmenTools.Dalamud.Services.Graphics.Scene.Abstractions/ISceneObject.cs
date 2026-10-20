using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;

namespace OmenTools.Dalamud.Services.Graphics.Scene.Abstractions;

public interface ISceneObject
{
	nint Address { get; }

	ISceneObject? ParentObject { get; }

	ISceneObject? PreviousSiblingObject { get; }

	ISceneObject? NextSiblingObject { get; }

	ISceneObject? ChildObject { get; }

	ulong ObjectFlags { get; }

	bool IsTransformChanged { get; }

	Vector3 Position { get; }

	Quaternion Rotation { get; }

	Vector3 Scale { get; }

	ObjectType ObjectType { get; }

	unsafe Object* ToStruct();

	static ISceneObject Create(nint address)
	{
		return new SceneObject(address);
	}
}
