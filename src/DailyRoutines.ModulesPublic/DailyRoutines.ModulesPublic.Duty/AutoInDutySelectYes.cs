using System.Collections.Generic;
using System.Linq;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;
using OmenTools.Extensions;
using OmenTools.Info.Algorithms;
using OmenTools.Interop.Game.AddonEvent;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;

namespace DailyRoutines.ModulesPublic.Duty;

public class AutoInDutySelectYes : ModuleBase
{
	private AhoCorasick Whitelist;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoInDutySelectYesTitle"),
		Description = LanguageManager.Get("AutoInDutySelectYesDescription"),
		Category = ModuleCategory.Duty
	};

	protected override void Init()
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		Whitelist = new AhoCorasick(from x in (IEnumerable<GimmickYesNo>)LuminaGetter.Get<GimmickYesNo>()
			select ((object)((GimmickYesNo)(ref x)).Message/*cast due to constrained. prefix*/).ToString() into x
			where !string.IsNullOrWhiteSpace(x)
			select x);
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)1, "SelectYesno", new AddonEventDelegate(OnAddonSelectYesno));
	}

	protected override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddonSelectYesno });
	}

	private unsafe void OnAddonSelectYesno(AddonEvent type, AddonArgs args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		if (GameState.ContentFinderCondition == 0)
		{
			return;
		}
		AddonSelectYesno* address = (AddonSelectYesno*)args.Addon.Address;
		if (address != null)
		{
			string text = ((object)(*(Utf8String*)(&((AtkTextNode)((AddonSelectYesno)address).PromptText).NodeText))/*cast due to constrained. prefix*/).ToString();
			if (!string.IsNullOrWhiteSpace(text) && Whitelist.ContainsAny(text))
			{
				AddonSelectYesnoEvent.ClickYes();
			}
		}
	}
}
