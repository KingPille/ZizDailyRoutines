using System;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Memory;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.AddonEvent;
using OmenTools.Interop.Game.ExecuteCommand.Implementations;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules.Interface;

public class AutoConfirmMateriaWork : ModuleBase
{
	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static AddonEventDelegate _003C0_003E__OnAddonYesno;

		public static AddonEventDelegate _003C1_003E__OnAddonAttach;

		public static AddonEventDelegate _003C2_003E__OnAddonRetrive;
	}

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected override void Init()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Expected O, but got Unknown
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Expected O, but got Unknown
		IAddonLifecycle obj = DalamudServiceExtension.Instance<IAddonLifecycle>();
		string text = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5CE30C99 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3054c6e6ef664be696bb2ad005864818);
		object obj2 = _003C_003EO._003C0_003E__OnAddonYesno;
		if (obj2 == null)
		{
			AddonEventDelegate val = mm5vWDDckh;
			_003C_003EO._003C0_003E__OnAddonYesno = val;
			obj2 = (object)val;
		}
		obj.RegisterListener((AddonEvent)1, text, (AddonEventDelegate)obj2);
		IAddonLifecycle obj3 = DalamudServiceExtension.Instance<IAddonLifecycle>();
		string text2 = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2310C02B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a96fab9e97944b39a8afe3fb1109ba7d);
		object obj4 = _003C_003EO._003C1_003E__OnAddonAttach;
		if (obj4 == null)
		{
			AddonEventDelegate val2 = EdhvKkUD6g;
			_003C_003EO._003C1_003E__OnAddonAttach = val2;
			obj4 = (object)val2;
		}
		obj3.RegisterListener((AddonEvent)1, text2, (AddonEventDelegate)obj4);
		IAddonLifecycle obj5 = DalamudServiceExtension.Instance<IAddonLifecycle>();
		string text3 = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x45E4AF41 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1904f95d2a394c929e1a6d4f72943ad7);
		object obj6 = _003C_003EO._003C2_003E__OnAddonRetrive;
		if (obj6 == null)
		{
			AddonEventDelegate val3 = Twovti6hyG;
			_003C_003EO._003C2_003E__OnAddonRetrive = val3;
			obj6 = (object)val3;
		}
		obj5.RegisterListener((AddonEvent)1, text3, (AddonEventDelegate)obj6);
	}

	protected override void Uninit()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Expected O, but got Unknown
		IAddonLifecycle obj = DalamudServiceExtension.Instance<IAddonLifecycle>();
		AddonEventDelegate[] array = new AddonEventDelegate[1];
		object obj2 = _003C_003EO._003C1_003E__OnAddonAttach;
		if (obj2 == null)
		{
			AddonEventDelegate val = EdhvKkUD6g;
			_003C_003EO._003C1_003E__OnAddonAttach = val;
			obj2 = (object)val;
		}
		array[0] = (AddonEventDelegate)obj2;
		obj.UnregisterListener((AddonEventDelegate[])(object)array);
		IAddonLifecycle obj3 = DalamudServiceExtension.Instance<IAddonLifecycle>();
		AddonEventDelegate[] array2 = new AddonEventDelegate[1];
		object obj4 = _003C_003EO._003C0_003E__OnAddonYesno;
		if (obj4 == null)
		{
			AddonEventDelegate val2 = mm5vWDDckh;
			_003C_003EO._003C0_003E__OnAddonYesno = val2;
			obj4 = (object)val2;
		}
		array2[0] = (AddonEventDelegate)obj4;
		obj3.UnregisterListener((AddonEventDelegate[])(object)array2);
		IAddonLifecycle obj5 = DalamudServiceExtension.Instance<IAddonLifecycle>();
		AddonEventDelegate[] array3 = new AddonEventDelegate[1];
		object obj6 = _003C_003EO._003C2_003E__OnAddonRetrive;
		if (obj6 == null)
		{
			AddonEventDelegate val3 = Twovti6hyG;
			_003C_003EO._003C2_003E__OnAddonRetrive = val3;
			obj6 = (object)val3;
		}
		array3[0] = (AddonEventDelegate)obj6;
		obj5.UnregisterListener((AddonEventDelegate[])(object)array3);
	}

	private unsafe static void EdhvKkUD6g(AddonEvent type, AddonArgs args)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		if (Addons.MateriaAttachDialog != null && ((AtkValue)((byte*)((AtkUnitBase)Addons.MateriaAttachDialog).AtkValues + (nint)41 * (nint)Unsafe.SizeOf<AtkValue>())).Int != 0)
		{
			bool flag;
			try
			{
				AtkValue val = ((AtkValue*)((AtkUnitBase)Addons.MateriaAttachDialog).AtkValues)[49];
				flag = (int)val.Type != 0 && !string.IsNullOrWhiteSpace(SeString.Parse(val.String.Value).TextValue);
			}
			catch (Exception)
			{
				flag = false;
			}
			MemoryHelper.Write<bool>((IntPtr)((nint)DalamudServiceExtension.Instance<ICondition>().Address + 7), true);
			if (flag)
			{
				(*Addons.MateriaAttachDialog).Callback(0, 0, 1);
			}
			else
			{
				(*Addons.MateriaAttachDialog).Callback(0, 0, 0);
				MVTvTck79c();
			}
		}
	}

	private unsafe static void Twovti6hyG(AddonEvent type, AddonArgs args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		AtkUnitBase* ptr = args.Addon.ToStruct();
		if (ptr != null)
		{
			(*ptr).Callback(0);
			MVTvTck79c();
		}
	}

	private unsafe static void mm5vWDDckh(AddonEvent type, AddonArgs args)
	{
		if (Addons.MateriaAttachDialog != null || Addons.MateriaRetrieveDialog != null)
		{
			AddonSelectYesnoEvent.ClickYes();
			MVTvTck79c();
		}
	}

	private static void MVTvTck79c()
	{
		MemoryHelper.Write<bool>((IntPtr)((nint)DalamudServiceExtension.Instance<ICondition>().Address + 7), false);
		MateriaCommand.FinishAttachState();
		MateriaCommand.LeaveAttachState();
		InventoryCommand.Refresh();
	}

	public AutoConfirmMateriaWork()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6854704F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_23522a2a14314da28eb44187a09cd0b5)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x12D6CA96 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7fcf5a71a7ed445197f132979519c560)),
			Category = ModuleCategory.Interface
		};
		Permission = new ModulePermission
		{
			NeedAuth = true
		};
		base._002Ector();
	}
}
