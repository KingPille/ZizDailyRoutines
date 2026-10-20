using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Text.ReadOnly;
using OmenTools.Interop.Game.Models;

namespace OmenTools.KamiToolKit.Addons.SelectYesno;

public sealed record SelectYesnoAddonOptions
{
	public required ReadOnlySeString Prompt { get; init; }

	public SelectYesnoAddonButtons Buttons { get; init; } = SelectYesnoAddonButtons.Both;

	public ReadOnlySeString? YesButtonText { get; init; }

	public ReadOnlySeString? NoButtonText { get; init; }

	public AlignmentType PromptAlignment { get; init; } = (AlignmentType)3;

	public bool RespectCloseAll { get; init; } = true;

	public AddonPosition? Position { get; init; }

	public int OpenSoundEffectID { get; init; } = 23;

	public ushort ParentID { get; init; }

	public ushort BlockedParentID { get; init; }

	public Action<SelectYesnoAddon, SelectYesnoAddonResult>? Callback { get; init; }

	[CompilerGenerated]
	[SetsRequiredMembers]
	private SelectYesnoAddonOptions(SelectYesnoAddonOptions original)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		Prompt = original.Prompt;
		Buttons = original.Buttons;
		YesButtonText = original.YesButtonText;
		NoButtonText = original.NoButtonText;
		PromptAlignment = original.PromptAlignment;
		RespectCloseAll = original.RespectCloseAll;
		Position = original.Position;
		OpenSoundEffectID = original.OpenSoundEffectID;
		ParentID = original.ParentID;
		BlockedParentID = original.BlockedParentID;
		Callback = original.Callback;
	}
}
