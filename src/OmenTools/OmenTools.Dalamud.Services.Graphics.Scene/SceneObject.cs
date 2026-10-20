using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using FFXIVClientStructs.FFXIV.Common.Math;
using OmenTools.Dalamud.Services.Graphics.Scene.Abstractions;

namespace OmenTools.Dalamud.Services.Graphics.Scene;

internal class SceneObject(nint address) : ISceneObject
{
	protected unsafe Object* Struct => (Object*)Address;

	public nint Address { get; } = address;

	public unsafe ISceneObject? ParentObject => Create(((Object)Struct).ParentObject);

	public unsafe ISceneObject? PreviousSiblingObject => Create(((Object)Struct).PreviousSiblingObject);

	public unsafe ISceneObject? NextSiblingObject => Create(((Object)Struct).NextSiblingObject);

	public unsafe ISceneObject? ChildObject => Create(((Object)Struct).ChildObject);

	public unsafe ulong ObjectFlags => ((Object)Struct).ObjectFlags;

	public unsafe bool IsTransformChanged => ((Object)Struct).IsTransformChanged;

	public unsafe Vector3 Position => Vector3.op_Implicit(((Object)Struct).Position);

	public unsafe Quaternion Rotation => Quaternion.op_Implicit(((Object)Struct).Rotation);

	public unsafe Vector3 Scale => Vector3.op_Implicit(((Object)Struct).Scale);

	public unsafe ObjectType ObjectType => ((Object)Struct).GetObjectType();

	public unsafe Object* ToStruct()
	{
		return Struct;
	}

	private unsafe static ISceneObject? Create(Object* sceneObject)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		if (sceneObject != null)
		{
			if ((int)((Object)sceneObject).GetObjectType() != 0)
			{
				return new DrawObject((nint)sceneObject);
			}
			return new SceneObject((nint)sceneObject);
		}
		return null;
	}
}
