using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;
using InteropGenerator.Runtime;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Models;

namespace DailyRoutines.ModulesPublic.Interface;

public class AutoSharpenInterfaceText : ModuleBase
{
	private unsafe delegate void AtkTextNodeSetTextDelegate(AtkTextNode* node, CStringPointer text);

	private static readonly CompSig AtkTextNodeSetTextSig = new CompSig("48 85 C9 0F 84 ?? ?? ?? ?? 4C 8B DC 53 56");

	private Hook<AtkTextNodeSetTextDelegate> AtkTextNodeSetTextHook;

	private const TextFlags FLAG_TO_REMOVE = (TextFlags)4096;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoSharpenInterfaceTextTitle"),
		Description = LanguageManager.Get("AutoSharpenInterfaceTextDescription"),
		Category = ModuleCategory.Interface
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	private static uint UIHighScaleMode => DalamudServiceExtension.Instance<IGameConfig>().System.GetUInt("UiHighScale");

	protected unsafe override void Init()
	{
		AtkTextNodeSetTextHook = AtkTextNodeSetTextSig.GetHook<AtkTextNodeSetTextDelegate>(AtkTextNodeSetTextDetour);
		AtkTextNodeSetTextHook.Enable();
	}

	private unsafe void AtkTextNodeSetTextDetour(AtkTextNode* node, CStringPointer text)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		AtkTextNodeSetTextHook.Original(node, text);
		if (node == null || UIHighScaleMode == 0)
		{
			return;
		}
		AtkUnitBase* ownerAddon = (*(AtkResNode*)node).GetOwnerAddon();
		if (ownerAddon != null && !(((AtkUnitBase)ownerAddon).NameString == "NamePlate"))
		{
			TextFlags textFlags = ((AtkTextNode)node).TextFlags;
			if (textFlags.IsSet<TextFlags>((TextFlags)4096))
			{
				textFlags = (TextFlags)(textFlags & 0xEFFF);
				Unsafe.Write(&((AtkTextNode)node).TextFlags, textFlags);
			}
		}
	}
}
