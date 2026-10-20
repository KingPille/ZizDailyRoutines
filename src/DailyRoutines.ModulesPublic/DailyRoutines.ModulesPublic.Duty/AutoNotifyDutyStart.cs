using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.DutyState;
using Dalamud.Plugin.Services;
using Lumina.Excel.Sheets;
using OmenTools.Extensions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic.Duty;

public class AutoNotifyDutyStart : ModuleBase
{
	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static DutyStartedDelegate _003C0_003E__OnDutyStart;
	}

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoNotifyDutyStartTitle"),
		Description = LanguageManager.Get("AutoNotifyDutyStartDescription"),
		Category = ModuleCategory.Duty
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected override void Init()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		IDutyState obj = DalamudServiceExtension.Instance<IDutyState>();
		object obj2 = _003C_003EO._003C0_003E__OnDutyStart;
		if (obj2 == null)
		{
			DutyStartedDelegate val = OnDutyStart;
			_003C_003EO._003C0_003E__OnDutyStart = val;
			obj2 = (object)val;
		}
		obj.DutyStarted += (DutyStartedDelegate)obj2;
	}

	protected override void Uninit()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		IDutyState obj = DalamudServiceExtension.Instance<IDutyState>();
		object obj2 = _003C_003EO._003C0_003E__OnDutyStart;
		if (obj2 == null)
		{
			DutyStartedDelegate val = OnDutyStart;
			_003C_003EO._003C0_003E__OnDutyStart = val;
			obj2 = (object)val;
		}
		obj.DutyStarted -= (DutyStartedDelegate)obj2;
	}

	private static void OnDutyStart(IDutyStateEventArgs args)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		NotifyHelper notifyHelper = OmenServiceBase<NotifyHelper>.Instance();
		ContentFinderCondition contentFinderConditionData = GameState.ContentFinderConditionData;
		notifyHelper.TrayInfo(((object)((ContentFinderCondition)(ref contentFinderConditionData)).Name/*cast due to constrained. prefix*/).ToString(), LanguageManager.Get("AutoNotifyDutyStart-NotificationMessage"));
	}
}
