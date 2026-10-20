using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Text.ReadOnly;
using OmenTools.Interop.Game.Models;

namespace OmenTools.KamiToolKit.Addons.InputString;

public sealed record InputStringAddonOptions
{
	public required ReadOnlySeString Prompt { get; init; }

	public ReadOnlySeString? Label { get; init; }

	public ReadOnlySeString? Value { get; init; }

	public string? Placeholder { get; init; }

	public int MaxCharacters { get; init; }

	public AlignmentType PromptAlignment { get; init; } = (AlignmentType)3;

	public ReadOnlySeString? ConfirmButtonText { get; init; }

	public ReadOnlySeString? CancelButtonText { get; init; }

	public bool RespectCloseAll { get; init; } = true;

	public AddonPosition? Position { get; init; }

	public int OpenSoundEffectID { get; init; } = 23;

	public ushort ParentID { get; init; }

	public ushort BlockedParentID { get; init; }

	public Action<InputStringAddon, InputStringAddonResult>? Callback { get; init; }

	[CompilerGenerated]
	[SetsRequiredMembers]
	private InputStringAddonOptions(InputStringAddonOptions original)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		Prompt = original.Prompt;
		Label = original.Label;
		Value = original.Value;
		Placeholder = original.Placeholder;
		MaxCharacters = original.MaxCharacters;
		PromptAlignment = original.PromptAlignment;
		ConfirmButtonText = original.ConfirmButtonText;
		CancelButtonText = original.CancelButtonText;
		RespectCloseAll = original.RespectCloseAll;
		Position = original.Position;
		OpenSoundEffectID = original.OpenSoundEffectID;
		ParentID = original.ParentID;
		BlockedParentID = original.BlockedParentID;
		Callback = original.Callback;
	}
}
