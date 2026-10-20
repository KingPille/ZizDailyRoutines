using System;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.ManagedFontAtlas;
using Dalamud.Interface.Textures.TextureWraps;
using OmenTools.ImGuiOm.Markdown.Parsing;
using OmenTools.ImGuiOm.Markdown.Rendering;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.ImGuiOm.Markdown;

public sealed record MarkdownConfig
{
	public static IReadOnlyList<float> DefaultHeadingScales { get; } = new global::_003C_003Ez__ReadOnlyArray<float>(new float[6] { 1.6f, 1.4f, 1.2f, 1f, 1f, 0.9f });

	public float BaseFontScale { get; init; } = 1f;

	public Func<MarkdownFontRole, float>? FontScaleResolver { get; init; }

	public Action<string>? OnLinkClicked { get; init; }

	public Func<string, MarkdownImageResult?>? ImageResolver { get; init; }

	public string? BaseURL { get; init; }

	public Action<string?, string>? CodeBlockRenderer { get; init; }

	public IReadOnlyList<float> HeadingScales { get; init; } = DefaultHeadingScales;

	public float? WrapWidth { get; init; }

	public float ListIndentPixels { get; init; } = 20f;

	public Vector4? LinkColor { get; init; }

	public IFontHandle ResolveFont(MarkdownFontRole role, float headingScale = 1f)
	{
		return OmenServiceBase<FontManager>.Instance().GetUIFont(FontScaleResolver?.Invoke(role) ?? (BaseFontScale * headingScale));
	}

	public float ResolveHeadingScale(int level)
	{
		int num = Math.Clamp(level, 1, 6);
		if (num - 1 >= HeadingScales.Count)
		{
			return 1f;
		}
		return HeadingScales[num - 1];
	}

	internal static float ResolveGap(MarkdownBlockKind previous, MarkdownBlockKind next)
	{
		return ImGui.GetTextLineHeight() * BlockSpacing.Gap(previous, next);
	}

	public MarkdownImageResult? ResolveImage(string source)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (ImageResolver != null)
		{
			return ImageResolver(source);
		}
		IDalamudTextureWrap image = OmenServiceBase<ImageHelper>.Instance().GetImage(ResolveURL(source));
		if (image != null)
		{
			return new MarkdownImageResult(image.Handle, image.Size);
		}
		return null;
	}

	public string ResolveURL(string url)
	{
		string baseURL = BaseURL;
		if (baseURL == null || baseURL.Length <= 0 || !url.StartsWith('/'))
		{
			return url;
		}
		return BaseURL.TrimEnd('/') + url;
	}
}
