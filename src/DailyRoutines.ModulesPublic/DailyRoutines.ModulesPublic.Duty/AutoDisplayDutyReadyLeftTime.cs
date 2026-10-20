using System;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.Text.Evaluator;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Text;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic.Duty;

public class AutoDisplayDutyReadyLeftTime : ModuleBase
{
	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static AddonEventDelegate _003C0_003E__OnAddon;
	}

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoDisplayDutyReadyLeftTimeTitle"),
		Description = LanguageManager.Get("AutoDisplayDutyReadyLeftTimeDescription"),
		Category = ModuleCategory.Duty,
		PreviewImageURL = new string[1] { "hxxps://gh.atmoomen.top/raw.githubusercontent.com/Dalamud-DailyRoutines/DailyRoutines/main/Resources/Modules/AutoDisplayDutyReadyLeftTime/preview-1.png" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected override void Init()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		IAddonLifecycle obj = DalamudServiceExtension.Instance<IAddonLifecycle>();
		object obj2 = _003C_003EO._003C0_003E__OnAddon;
		if (obj2 == null)
		{
			AddonEventDelegate val = OnAddon;
			_003C_003EO._003C0_003E__OnAddon = val;
			obj2 = (object)val;
		}
		obj.RegisterListener((AddonEvent)5, "ContentsFinderReady", (AddonEventDelegate)obj2);
	}

	protected override void Uninit()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		IAddonLifecycle obj = DalamudServiceExtension.Instance<IAddonLifecycle>();
		AddonEventDelegate[] array = new AddonEventDelegate[1];
		object obj2 = _003C_003EO._003C0_003E__OnAddon;
		if (obj2 == null)
		{
			AddonEventDelegate val = OnAddon;
			_003C_003EO._003C0_003E__OnAddon = val;
			obj2 = (object)val;
		}
		array[0] = (AddonEventDelegate)obj2;
		obj.UnregisterListener((AddonEventDelegate[])(object)array);
	}

	private unsafe static void OnAddon(AddonEvent type, AddonArgs args)
	{
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		if (!(*Addons.ContentsFinderReady).IsAddonAndNodesReady())
		{
			return;
		}
		AtkTextNode* textNodeById = ((AtkUnitBase)Addons.ContentsFinderReady).GetTextNodeById(3u);
		if (textNodeById == null)
		{
			return;
		}
		ContentsFinder* ptr = ContentsFinder.Instance();
		if (ptr == null)
		{
			return;
		}
		int queueReadyTimestamp = ((ContentsFinderQueueInfo)(&((ContentsFinder)ptr).QueueInfo)).QueueReadyTimestamp;
		if (queueReadyTimestamp == 0)
		{
			return;
		}
		TimeSpan timeSpan = queueReadyTimestamp.ToUTCDateTimeFromUnixSeconds() + TimeSpan.FromSeconds(45L) - OmenServiceBase<StandardTimeManager>.Instance().UTCNow;
		if (timeSpan.TotalSeconds < 0.0)
		{
			return;
		}
		RentedSeStringBuilder val = default(RentedSeStringBuilder);
		((RentedSeStringBuilder)(ref val))._002Ector();
		try
		{
			SeStringBuilder builder = ((RentedSeStringBuilder)(ref val)).Builder;
			SeStringBuilder val2 = builder;
			SeStringInterpolatedStringHandler val3 = new SeStringInterpolatedStringHandler(1, 1, val2);
			((SeStringInterpolatedStringHandler)(ref val3)).AppendFormatted<string>(LuminaWrapper.GetAddonText(2780u));
			((SeStringInterpolatedStringHandler)(ref val3)).AppendLiteral(" ");
			SeStringBuilder val4 = val2.Append(val3).PushColorType(32u);
			val3 = new SeStringInterpolatedStringHandler(2, 1, val4);
			((SeStringInterpolatedStringHandler)(ref val3)).AppendLiteral("[");
			ISeStringEvaluator obj = DalamudServiceExtension.Instance<ISeStringEvaluator>();
			SeStringParameter reference = SeStringParameter.op_Implicit((int)timeSpan.TotalSeconds);
			((SeStringInterpolatedStringHandler)(ref val3)).AppendFormatted(obj.EvaluateFromAddon(9169u, new Span<SeStringParameter>(ref reference), (ClientLanguage?)null));
			((SeStringInterpolatedStringHandler)(ref val3)).AppendLiteral("]");
			val4.Append(val3).PopColorType();
			((AtkTextNode)textNodeById).SetText(builder.GetViewAsSpan());
		}
		finally
		{
			((IDisposable)(*(RentedSeStringBuilder*)(&val))/*cast due to constrained. prefix*/).Dispose();
		}
	}
}
