using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Textures;

namespace OmenTools.ImGuiOm.Widgets.MapRenderer;

public class ImGuiMapMarker
{
	public string ID { get; set; } = string.Empty;

	public Vector3 Position { get; set; }

	public Vector2 Size { get; set; } = new Vector2(24f, 24f);

	public uint Color { get; set; } = uint.MaxValue;

	public ISharedImmediateTexture? Texture { get; set; }

	public uint? IconID { get; set; }

	public string Name { get; set; } = string.Empty;

	public string Description { get; set; } = string.Empty;

	public string Label { get; set; } = string.Empty;

	public bool ShowLabel { get; set; } = true;

	public uint LabelColor { get; set; } = uint.MaxValue;

	public string Hint { get; set; } = string.Empty;

	public uint HintColor { get; set; } = 4286611584u;

	public bool ShowTooltip { get; set; } = true;

	public string TooltipText { get; set; } = string.Empty;

	public bool PulseEffect { get; set; }

	public uint PulseColor { get; set; } = 2147548928u;

	public Action<ImGuiMapMarker, Vector2, ImDrawListPtr>? OnCustomDraw { get; set; }

	public Action<ImGuiMapMarker>? OnClick { get; set; }

	public Action<ImGuiMapMarker>? OnRightClick { get; set; }

	public Action<ImGuiMapMarker>? OnHover { get; set; }
}
