using System.Collections.Frozen;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.System.Input;
using FFXIVClientStructs.FFXIV.Component.GUI;
using OmenTools.Extensions;
using OmenTools.Interop.Windows.Helpers;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;

namespace DailyRoutines.ModulesPublic.Duty;

public class AutoQTE : ModuleBase
{
	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static InputIDManager.PrePressedDelegate _003C0_003E__OnPreIsInputIDPressed;

		public static AddonEventDelegate _003C1_003E__OnQTEAddon;
	}

	private static readonly FrozenSet<string> QTETypes;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoQTETitle"),
		Description = LanguageManager.Get("AutoQTEDescription"),
		Category = ModuleCategory.Duty
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected override void Init()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
		OmenServiceBase<InputIDManager>.Instance().RegPrePressed(OnPreIsInputIDPressed);
		IAddonLifecycle obj = DalamudServiceExtension.Instance<IAddonLifecycle>();
		FrozenSet<string> qTETypes = QTETypes;
		object obj2 = _003C_003EO._003C1_003E__OnQTEAddon;
		if (obj2 == null)
		{
			AddonEventDelegate val = OnQTEAddon;
			_003C_003EO._003C1_003E__OnQTEAddon = val;
			obj2 = (object)val;
		}
		obj.RegisterListener((AddonEvent)5, (IEnumerable<string>)qTETypes, (AddonEventDelegate)obj2);
	}

	protected override void Uninit()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		OmenServiceBase<InputIDManager>.Instance().UnregPrePressed(OnPreIsInputIDPressed);
		IAddonLifecycle obj = DalamudServiceExtension.Instance<IAddonLifecycle>();
		AddonEventDelegate[] array = new AddonEventDelegate[1];
		object obj2 = _003C_003EO._003C1_003E__OnQTEAddon;
		if (obj2 == null)
		{
			AddonEventDelegate val = OnQTEAddon;
			_003C_003EO._003C1_003E__OnQTEAddon = val;
			obj2 = (object)val;
		}
		array[0] = (AddonEventDelegate)obj2;
		obj.UnregisterListener((AddonEventDelegate[])(object)array);
	}

	private static void OnPreIsInputIDPressed(ref bool? overrideResult, ref InputId id)
	{
		if (GameState.ContentFinderCondition != 0 && !Throttler.Shared.Check("AutoQTE-QTE"))
		{
			overrideResult = false;
		}
	}

	private unsafe static void OnQTEAddon(AddonEvent type, AddonArgs args)
	{
		Throttler.Shared.Throttle("AutoQTE-QTE", 1000u, reThrottle: true);
		KeyEmulationHelper.SendKeypress(Keys.Space);
		((AtkStage)AtkStage.Instance()).ClearFocus();
	}

	static AutoQTE()
	{
		InlineArray4<string> buffer = default(InlineArray4<string>);
		buffer[0] = "_QTEKeep";
		buffer[1] = "_QTEMash";
		buffer[2] = "_QTEKeepTime";
		buffer[3] = "_QTEButton";
		QTETypes = FrozenSet.Create<string>(buffer);
	}
}
