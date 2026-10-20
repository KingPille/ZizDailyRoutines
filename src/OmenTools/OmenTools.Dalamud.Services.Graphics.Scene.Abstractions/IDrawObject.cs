using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;

namespace OmenTools.Dalamud.Services.Graphics.Scene.Abstractions;

public interface IDrawObject : ISceneObject
{
	byte Flags { get; }

	byte OutlineFlags { get; }

	bool IsCoveredFromRain { get; }

	byte LoadState { get; }

	ObjectHighlightColor OutlineColor { get; }

	bool IsVisible { get; }

	float Transparency { get; }

	int TargetStatus { get; }

	new unsafe DrawObject* ToStruct();

	new static IDrawObject Create(nint address)
	{
		return new DrawObject(address);
	}
}
