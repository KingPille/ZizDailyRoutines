using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;

namespace DailyRoutines.ModulesPublic.Interface;

public class AutoQuestComplete : ModuleBase
{
	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static AddonEventDelegate _003C0_003E__OnAddonJournalResultSetup;

		public static AddonEventDelegate _003C1_003E__OnAddonSatisfactionSupplyResultSetup;
	}

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoQuestCompleteTitle"),
		Description = LanguageManager.Get("AutoQuestCompleteDescription"),
		Category = ModuleCategory.Interface
	};

	protected override void Init()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected O, but got Unknown
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Expected O, but got Unknown
		IAddonLifecycle obj = DalamudServiceExtension.Instance<IAddonLifecycle>();
		object obj2 = _003C_003EO._003C0_003E__OnAddonJournalResultSetup;
		if (obj2 == null)
		{
			AddonEventDelegate val = OnAddonJournalResultSetup;
			_003C_003EO._003C0_003E__OnAddonJournalResultSetup = val;
			obj2 = (object)val;
		}
		obj.RegisterListener((AddonEvent)1, "JournalResult", (AddonEventDelegate)obj2);
		IAddonLifecycle obj3 = DalamudServiceExtension.Instance<IAddonLifecycle>();
		object obj4 = _003C_003EO._003C0_003E__OnAddonJournalResultSetup;
		if (obj4 == null)
		{
			AddonEventDelegate val2 = OnAddonJournalResultSetup;
			_003C_003EO._003C0_003E__OnAddonJournalResultSetup = val2;
			obj4 = (object)val2;
		}
		obj3.RegisterListener((AddonEvent)5, "JournalResult", (AddonEventDelegate)obj4);
		IAddonLifecycle obj5 = DalamudServiceExtension.Instance<IAddonLifecycle>();
		object obj6 = _003C_003EO._003C1_003E__OnAddonSatisfactionSupplyResultSetup;
		if (obj6 == null)
		{
			AddonEventDelegate val3 = OnAddonSatisfactionSupplyResultSetup;
			_003C_003EO._003C1_003E__OnAddonSatisfactionSupplyResultSetup = val3;
			obj6 = (object)val3;
		}
		obj5.RegisterListener((AddonEvent)1, "SatisfactionSupplyResult", (AddonEventDelegate)obj6);
		IAddonLifecycle obj7 = DalamudServiceExtension.Instance<IAddonLifecycle>();
		object obj8 = _003C_003EO._003C1_003E__OnAddonSatisfactionSupplyResultSetup;
		if (obj8 == null)
		{
			AddonEventDelegate val4 = OnAddonSatisfactionSupplyResultSetup;
			_003C_003EO._003C1_003E__OnAddonSatisfactionSupplyResultSetup = val4;
			obj8 = (object)val4;
		}
		obj7.RegisterListener((AddonEvent)5, "SatisfactionSupplyResult", (AddonEventDelegate)obj8);
	}

	protected override void Uninit()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		IAddonLifecycle obj = DalamudServiceExtension.Instance<IAddonLifecycle>();
		AddonEventDelegate[] array = new AddonEventDelegate[1];
		object obj2 = _003C_003EO._003C1_003E__OnAddonSatisfactionSupplyResultSetup;
		if (obj2 == null)
		{
			AddonEventDelegate val = OnAddonSatisfactionSupplyResultSetup;
			_003C_003EO._003C1_003E__OnAddonSatisfactionSupplyResultSetup = val;
			obj2 = (object)val;
		}
		array[0] = (AddonEventDelegate)obj2;
		obj.UnregisterListener((AddonEventDelegate[])(object)array);
		IAddonLifecycle obj3 = DalamudServiceExtension.Instance<IAddonLifecycle>();
		AddonEventDelegate[] array2 = new AddonEventDelegate[1];
		object obj4 = _003C_003EO._003C0_003E__OnAddonJournalResultSetup;
		if (obj4 == null)
		{
			AddonEventDelegate val2 = OnAddonJournalResultSetup;
			_003C_003EO._003C0_003E__OnAddonJournalResultSetup = val2;
			obj4 = (object)val2;
		}
		array2[0] = (AddonEventDelegate)obj4;
		obj3.UnregisterListener((AddonEventDelegate[])(object)array2);
	}

	private unsafe static void OnAddonJournalResultSetup(AddonEvent type, AddonArgs args)
	{
		AtkUnitBase* journalResult = Addons.JournalResult;
		if (journalResult != null)
		{
			uint uInt = ((AtkValue)((byte*)((AtkUnitBase)journalResult).AtkValues + (nint)82 * (nint)Unsafe.SizeOf<AtkValue>())).UInt;
			if (uInt == 0)
			{
				(*journalResult).Callback(0, 0);
			}
			else
			{
				(*journalResult).Callback(0, uInt);
			}
		}
	}

	private unsafe static void OnAddonSatisfactionSupplyResultSetup(AddonEvent type, AddonArgs args)
	{
		AtkUnitBase* satisfactionSupplyResult = Addons.SatisfactionSupplyResult;
		if (satisfactionSupplyResult != null)
		{
			(*satisfactionSupplyResult).Callback(1);
		}
	}
}
