using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using OmenTools.Dalamud.Services.Graphics.Scene.Abstractions;

namespace OmenTools.Dalamud.Services.Graphics.Scene;

internal class DrawObject : SceneObject, IDrawObject, ISceneObject
{
	private new unsafe DrawObject* Struct => (DrawObject*)base.Address;

	public unsafe byte Flags => ((DrawObject)Struct).Flags;

	public unsafe byte OutlineFlags => ((DrawObject)Struct).OutlineFlags;

	public unsafe bool IsCoveredFromRain => ((DrawObject)Struct).IsCoveredFromRain;

	public unsafe byte LoadState => ((DrawObject)Struct).LoadState;

	public unsafe ObjectHighlightColor OutlineColor => ((DrawObject)Struct).OutlineColor;

	public unsafe bool IsVisible => ((DrawObject)Struct).IsVisible;

	public unsafe float Transparency => ((DrawObject)Struct).GetTransparency();

	public unsafe int TargetStatus => ((DrawObject)Struct).GetTargetStatus();

	public DrawObject(nint address)
		: base(address)
	{
	}

	public new unsafe DrawObject* ToStruct()
	{
		return Struct;
	}
}
