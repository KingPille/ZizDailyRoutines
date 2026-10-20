using System.Windows.Forms;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.System.Input;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Component.GUI;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Models;

namespace DailyRoutines.ModulesPublic;

public class ClipboardMultiLineToOneLine : ModuleBase
{
	private unsafe delegate Utf8String* GetClipboardDataDelegate(ClipBoard* clipBoard);

	private static readonly CompSig GetClipboardDataSig = new CompSig("40 53 56 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 84 24 ?? ?? ?? ?? 48 8B F1 BA");

	private Hook<GetClipboardDataDelegate>? GetClipboardDataHook;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("ClipboardMultiLineToOneLineTitle"),
		Description = LanguageManager.Get("ClipboardMultiLineToOneLineDescription"),
		Category = ModuleCategory.System
	};

	protected unsafe override void Init()
	{
		if (GetClipboardDataHook == null)
		{
			GetClipboardDataHook = GetClipboardDataSig.GetHook<GetClipboardDataDelegate>(GetClipboardDataDetour);
		}
		GetClipboardDataHook.Enable();
	}

	private unsafe Utf8String* GetClipboardDataDetour(ClipBoard* clipBoard)
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		if (((Framework)Framework.Instance()).WindowInactive || !AtkComponentExtension.TryGetActive(out var component, out var _))
		{
			return InvokeOriginal();
		}
		string text = Clipboard.GetText();
		if (string.IsNullOrWhiteSpace(text))
		{
			return InvokeOriginal();
		}
		string text2 = text;
		text2 = ((((AtkUldComponentDataTextInput)(&((AtkComponentTextInput)component).ComponentTextData)).MaxLine <= 1 && !((AtkUldComponentDataTextInput)(&((AtkComponentTextInput)component).ComponentTextData)).Flags2.IsSet<TextInputFlags2>((TextInputFlags2)8)) ? text.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ") : text.Replace("\r\n", "\r").Replace("\n", "\r"));
		if (text2 == text)
		{
			return InvokeOriginal();
		}
		Utf8String* result = &((ClipBoard)clipBoard).SystemClipboardText;
		((Utf8String)(&((ClipBoard)clipBoard).SystemClipboardText)).Clear();
		((Utf8String)(&((ClipBoard)clipBoard).SystemClipboardText)).SetString(text2);
		return result;
		unsafe Utf8String* InvokeOriginal()
		{
			return GetClipboardDataHook.Original(clipBoard);
		}
	}
}
