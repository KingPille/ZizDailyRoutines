using System;
using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.AddonEvent.Abstractions;

namespace OmenTools.Interop.Game.AddonEvent;

public class AddonSelectYesnoEvent : AddonEventBase
{
	public static bool CheckConfirm(IReadOnlyList<string> text)
	{
		return text.Select(CheckConfirm).Any((bool x) => x);
	}

	public unsafe static bool CheckConfirm(string? textToContain = null)
	{
		if (!(*Addons.SelectYesno).IsAddonAndNodesReady())
		{
			return false;
		}
		if (!string.IsNullOrWhiteSpace(textToContain))
		{
			string text = ((object)(*(Utf8String*)(&((AtkTextNode)((AtkUnitBase)Addons.SelectYesno).GetTextNodeById(2u)).NodeText))/*cast due to constrained. prefix*/).ToString().Replace("\n", string.Empty);
			if (!string.IsNullOrWhiteSpace(text) && !text.Contains(textToContain.Replace("\n", string.Empty), StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
		}
		AddonSelectYesno* selectYesno = (AddonSelectYesno*)Addons.SelectYesno;
		(*((AddonSelectYesno)selectYesno).ConfirmCheckBox).Click(3, (AtkEventType)25);
		return true;
	}

	public static bool ClickNo(IReadOnlyList<string> text)
	{
		return text.Select(ClickNo).Any((bool x) => x);
	}

	public unsafe static bool ClickNo(string? textToContain = null)
	{
		if (!(*Addons.SelectYesno).IsAddonAndNodesReady())
		{
			return false;
		}
		if (!string.IsNullOrWhiteSpace(textToContain))
		{
			string text = ((object)(*(Utf8String*)(&((AtkTextNode)((AtkUnitBase)Addons.SelectYesno).GetTextNodeById(2u)).NodeText))/*cast due to constrained. prefix*/).ToString().Replace("\n", string.Empty);
			if (!string.IsNullOrWhiteSpace(text) && !text.Contains(textToContain.Replace("\n", string.Empty), StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
		}
		(*Addons.SelectYesno).Callback(1);
		return true;
	}

	public static bool ClickYes(IReadOnlyList<string> text)
	{
		return text.Select(ClickYes).Any((bool x) => x);
	}

	public unsafe static bool ClickYes(string? textToContain = null)
	{
		if (!(*Addons.SelectYesno).IsAddonAndNodesReady())
		{
			return false;
		}
		if (!string.IsNullOrWhiteSpace(textToContain))
		{
			string text = ((object)(*(Utf8String*)(&((AtkTextNode)((AtkUnitBase)Addons.SelectYesno).GetTextNodeById(2u)).NodeText))/*cast due to constrained. prefix*/).ToString().Replace("\n", string.Empty);
			if (!string.IsNullOrWhiteSpace(text) && !text.Contains(textToContain.Replace("\n", string.Empty), StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
		}
		(*Addons.SelectYesno).Callback(0);
		return true;
	}
}
