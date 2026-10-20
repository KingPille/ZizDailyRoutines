using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Internal;
using DailyRoutines.Manager;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Lumina.Text.ReadOnly;
using OmenTools.Dalamud;
using OmenTools.Extensions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using W5ek8T91VhslWK6xKNn;
using itAnDoW4mgYVlrmtMor;
using wUQLZCVUi4RNloOxE8c;

namespace scAmY5SuvRnwiSBZk0U;

internal static class TgWnIVSUWGDITG09VVO
{
	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static HandlerDelegate _003C0_003E__OnMainCommand;

		public static HandlerDelegate _003C1_003E__OnSearch;

		public static HandlerDelegate _003C2_003E__OnLoad;

		public static HandlerDelegate _003C3_003E__OnUnload;

		public static HandlerDelegate _003C4_003E__OnToggle;

		public static HandlerDelegate _003C5_003E__OnTTS;

		public static HandlerDelegate _003C6_003E__OnNotify;

		public static HandlerDelegate _003C7_003E__OnFavOn;

		public static HandlerDelegate _003C8_003E__OnFavOff;

		public static HandlerDelegate _003C9_003E__OnReloadAll;

		public static HandlerDelegate _003C10_003E__OnToggleConfig;

		public static HandlerDelegate _003C11_003E__OnDebug;
	}

	private const string MAIN_COMMAND = "/pdr";

	public static void MyvSX3JCpS()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Expected O, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected O, but got Unknown
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Expected O, but got Unknown
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Expected O, but got Unknown
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Expected O, but got Unknown
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Expected O, but got Unknown
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Expected O, but got Unknown
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Expected O, but got Unknown
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Expected O, but got Unknown
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Expected O, but got Unknown
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Expected O, but got Unknown
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Expected O, but got Unknown
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Expected O, but got Unknown
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Expected O, but got Unknown
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Expected O, but got Unknown
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Expected O, but got Unknown
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0402: Expected O, but got Unknown
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Expected O, but got Unknown
		//IL_043d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Expected O, but got Unknown
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_043d: Expected O, but got Unknown
		//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e1: Expected O, but got Unknown
		//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b0: Expected O, but got Unknown
		//IL_051c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0521: Unknown result type (might be due to invalid IL or missing references)
		//IL_052e: Expected O, but got Unknown
		//IL_0511: Unknown result type (might be due to invalid IL or missing references)
		//IL_0516: Unknown result type (might be due to invalid IL or missing references)
		//IL_051c: Expected O, but got Unknown
		CommandManager commandManager = OmenServiceBase<CommandManager>.Instance();
		string command = GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x227934D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_e3177f14377e4fe58ddaaaaf90163f44);
		object obj = _003C_003EO._003C0_003E__OnMainCommand;
		if (obj == null)
		{
			HandlerDelegate val = PDLSzFxU03;
			_003C_003EO._003C0_003E__OnMainCommand = val;
			obj = (object)val;
		}
		commandManager.MainCommand = new MainCommandInfo(command, new CommandInfo((HandlerDelegate)obj)
		{
			HelpMessage = LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1FFA4FB7 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_a46716ea783442acb36e95ced1c935fc))
		});
		string subCommand = GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5369AA32 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d4ce857df7b047629e7091bbc815dd94);
		object obj2 = _003C_003EO._003C1_003E__OnSearch;
		if (obj2 == null)
		{
			HandlerDelegate val2 = LCwVHp2E72;
			_003C_003EO._003C1_003E__OnSearch = val2;
			obj2 = (object)val2;
		}
		commandManager.AddSubCommand(subCommand, new CommandInfo((HandlerDelegate)obj2)
		{
			HelpMessage = LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2FDD9E47 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c6c2389b72fc4ab3828ae71fae281bf1))
		});
		string subCommand2 = GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x70C3497D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c041f06f3e304b758cab1109bed434d0);
		object obj3 = _003C_003EO._003C2_003E__OnLoad;
		if (obj3 == null)
		{
			HandlerDelegate val3 = saLVSNa5jy;
			_003C_003EO._003C2_003E__OnLoad = val3;
			obj3 = (object)val3;
		}
		commandManager.AddSubCommand(subCommand2, new CommandInfo((HandlerDelegate)obj3)
		{
			HelpMessage = LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x15FE5566 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_e279476822c444f9955c257cab427fb2))
		});
		string subCommand3 = GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4FBFB385 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_881eb9df907345dea66a751ad6d65e76);
		object obj4 = _003C_003EO._003C3_003E__OnUnload;
		if (obj4 == null)
		{
			HandlerDelegate val4 = K2hVVk45Rc;
			_003C_003EO._003C3_003E__OnUnload = val4;
			obj4 = (object)val4;
		}
		commandManager.AddSubCommand(subCommand3, new CommandInfo((HandlerDelegate)obj4)
		{
			HelpMessage = LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2FDD9DDB ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c6c2389b72fc4ab3828ae71fae281bf1))
		});
		string subCommand4 = GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x75EF2AA5 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_11422b9d07f9498fb995b7dd8e417b5b);
		object obj5 = _003C_003EO._003C4_003E__OnToggle;
		if (obj5 == null)
		{
			HandlerDelegate val5 = lV2VfRCI3L;
			_003C_003EO._003C4_003E__OnToggle = val5;
			obj5 = (object)val5;
		}
		commandManager.AddSubCommand(subCommand4, new CommandInfo((HandlerDelegate)obj5)
		{
			HelpMessage = LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x37CC8205 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_90dae8e50e3a4314867cef21f0bb1bb0))
		});
		string subCommand5 = GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1E249E3B ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_86afda88005749b094dac2a7d6b27c25);
		object obj6 = _003C_003EO._003C5_003E__OnTTS;
		if (obj6 == null)
		{
			HandlerDelegate val6 = wbJVy8aZ2k;
			_003C_003EO._003C5_003E__OnTTS = val6;
			obj6 = (object)val6;
		}
		commandManager.AddSubCommand(subCommand5, new CommandInfo((HandlerDelegate)obj6)
		{
			HelpMessage = LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(-1487420814 ^ -1828719541 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_95b251cdaa8148b7af54d7956ad9ed2a))
		});
		string subCommand6 = GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2BF19002 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_fc2313f9e2c04f099d4c47c5893e635d);
		object obj7 = _003C_003EO._003C6_003E__OnNotify;
		if (obj7 == null)
		{
			HandlerDelegate val7 = j2XVLjFL3e;
			_003C_003EO._003C6_003E__OnNotify = val7;
			obj7 = (object)val7;
		}
		commandManager.AddSubCommand(subCommand6, new CommandInfo((HandlerDelegate)obj7)
		{
			HelpMessage = LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3B5BF9E8 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1e38c41bbda54d9abc5b426283576a9a))
		});
		string subCommand7 = GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x34A3C6EC ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5e416caedcb6438f915024f390fd9079);
		object obj8 = _003C_003EO._003C7_003E__OnFavOn;
		if (obj8 == null)
		{
			HandlerDelegate val8 = qJPVC01x51;
			_003C_003EO._003C7_003E__OnFavOn = val8;
			obj8 = (object)val8;
		}
		commandManager.AddSubCommand(subCommand7, new CommandInfo((HandlerDelegate)obj8)
		{
			HelpMessage = LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4118D093 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_55c846b516154967bf0c321596d2dc52))
		});
		string subCommand8 = GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x339D4BCC ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c48859e0be41429f8474d6e2fecfa0de);
		object obj9 = _003C_003EO._003C8_003E__OnFavOff;
		if (obj9 == null)
		{
			HandlerDelegate val9 = vCNVjMe6Qw;
			_003C_003EO._003C8_003E__OnFavOff = val9;
			obj9 = (object)val9;
		}
		commandManager.AddSubCommand(subCommand8, new CommandInfo((HandlerDelegate)obj9)
		{
			HelpMessage = LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4FBFB23B ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_881eb9df907345dea66a751ad6d65e76))
		});
		string subCommand9 = GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2279EFD ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_e3177f14377e4fe58ddaaaaf90163f44);
		object obj10 = _003C_003EO._003C9_003E__OnReloadAll;
		if (obj10 == null)
		{
			HandlerDelegate val10 = xFBV9cpwLh;
			_003C_003EO._003C9_003E__OnReloadAll = val10;
			obj10 = (object)val10;
		}
		commandManager.AddSubCommand(subCommand9, new CommandInfo((HandlerDelegate)obj10)
		{
			HelpMessage = LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x642B794E ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7f11a41e9f664db18364eb0afc92a5df))
		});
		string subCommand10 = GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6CA6483F ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_86e8e55020514087b1fe097f879d6dff);
		object obj11 = _003C_003EO._003C10_003E__OnToggleConfig;
		if (obj11 == null)
		{
			HandlerDelegate val11 = pckVO90Z90;
			_003C_003EO._003C10_003E__OnToggleConfig = val11;
			obj11 = (object)val11;
		}
		commandManager.AddSubCommand(subCommand10, new CommandInfo((HandlerDelegate)obj11)
		{
			HelpMessage = LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2E95F881 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_6103a9bab63e42479916a134b07f1c75))
		});
		string subCommand11 = GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x28298847 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_95ac0b4adf764381bcdc75a9d241d184);
		object obj12 = _003C_003EO._003C11_003E__OnDebug;
		if (obj12 == null)
		{
			HandlerDelegate val12 = rbBVnluusg;
			_003C_003EO._003C11_003E__OnDebug = val12;
			obj12 = (object)val12;
		}
		commandManager.AddSubCommand(subCommand11, new CommandInfo((HandlerDelegate)obj12)
		{
			ShowInHelp = false
		});
	}

	public static void y8RSY1mjAH()
	{
		CommandManager commandManager = OmenServiceBase<CommandManager>.Instance();
		commandManager.RemoveSubCommand(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6E47D84F ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ad43e19aac164f32ab06e23cbc10bc4e));
		commandManager.RemoveSubCommand(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x768E11D5 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_417cae18312d413c80505172551e7b46));
		commandManager.RemoveSubCommand(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2E95F663 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_6103a9bab63e42479916a134b07f1c75));
		commandManager.RemoveSubCommand(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x38D3C6CE ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_59cab3a5f3464d2f8ff563ddb3d8bad1));
		commandManager.RemoveSubCommand(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x28176884 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_dc626e3e84304b9d975c6d744befebcd));
		commandManager.RemoveSubCommand(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x90753DA ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_444e42c3749640698bcaba67a1c5b5f3));
		commandManager.RemoveSubCommand(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x53784093 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_019a6a2bbee24ddb9dd8d9a1d1e1ca09));
		commandManager.RemoveSubCommand(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x537840D9 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_019a6a2bbee24ddb9dd8d9a1d1e1ca09));
		commandManager.RemoveSubCommand(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4118DF11 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_55c846b516154967bf0c321596d2dc52));
		commandManager.RemoveSubCommand(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x46288937 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_93e1cfc241b644019a4819c49327d6a7));
		commandManager.RemoveSubCommand(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2FDD9367 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c6c2389b72fc4ab3828ae71fae281bf1));
		commandManager.MainCommand = null;
	}

	private static void PDLSzFxU03(string command, string args)
	{
		DLog.Debug(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x15FE5B8E ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_e279476822c444f9955c257cab427fb2) + command + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x76CC44C3 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_417cae18312d413c80505172551e7b46) + args);
		HfWYPNVIio8M9ncZnm0 hfWYPNVIio8M9ncZnm = OmenServiceBase<WindowManager>.Instance().Get<HfWYPNVIio8M9ncZnm0>();
		if (hfWYPNVIio8M9ncZnm != null)
		{
			((Window)hfWYPNVIio8M9ncZnm).IsOpen = !((Window)hfWYPNVIio8M9ncZnm).IsOpen;
			hfWYPNVIio8M9ncZnm.sv49SfOr7P<HfWYPNVIio8M9ncZnm0.OMQ0W4KSvbgxtAOQfUK>().iuIK9HKYfw();
		}
	}

	private static void rbBVnluusg(string command, string args)
	{
		i3LqFS9TadSFeoumyfM i3LqFS9TadSFeoumyfM2 = OmenServiceBase<WindowManager>.Instance().Get<i3LqFS9TadSFeoumyfM>();
		if (i3LqFS9TadSFeoumyfM2 != null)
		{
			((Window)i3LqFS9TadSFeoumyfM2).IsOpen = !((Window)i3LqFS9TadSFeoumyfM2).IsOpen;
		}
	}

	private static void wbJVy8aZ2k(string command, string args)
	{
		args = args.Trim();
		if (!string.IsNullOrWhiteSpace(args))
		{
			DLog.Warning(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6541420C ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_33b91e080b77482a8d7d3bd797a9d8fb)));
			NotifyHelper.Speak(args);
		}
	}

	private static void j2XVLjFL3e(string command, string args)
	{
		args = args.Trim();
		if (!string.IsNullOrWhiteSpace(args))
		{
			OmenServiceBase<NotifyHelper>.Instance().NotificationInfo(args);
		}
	}

	private static void LCwVHp2E72(string command, string args)
	{
		HfWYPNVIio8M9ncZnm0 hfWYPNVIio8M9ncZnm = OmenServiceBase<WindowManager>.Instance().Get<HfWYPNVIio8M9ncZnm0>();
		if (hfWYPNVIio8M9ncZnm != null)
		{
			((Window)hfWYPNVIio8M9ncZnm).IsOpen = !((Window)hfWYPNVIio8M9ncZnm).IsOpen;
			hfWYPNVIio8M9ncZnm.sv49SfOr7P<HfWYPNVIio8M9ncZnm0.OMQ0W4KSvbgxtAOQfUK>().L59KV4Y5ei(args);
		}
	}

	private static void saLVSNa5jy(string command, string args)
	{
		bZkVKPRnZN(args, true);
	}

	private static void K2hVVk45Rc(string command, string args)
	{
		bZkVKPRnZN(args, false);
	}

	private static void xFBV9cpwLh(string command, string args)
	{
		if (!ManagerBase<ModuleManager>.Instance().IsInitializingAllModules && !ManagerBase<ModuleManager>.Instance().IsLoadingAllModules)
		{
			ManagerBase<ModuleManager>.Instance().ReloadAllAsync(CancellationToken.None);
		}
	}

	private static void lV2VfRCI3L(string command, string args)
	{
		bZkVKPRnZN(args);
	}

	private static void qJPVC01x51(string command, string args)
	{
		U2BVtfqmU4(args, isLoad: true);
	}

	private static void vCNVjMe6Qw(string command, string args)
	{
		U2BVtfqmU4(args, isLoad: false);
	}

	private static void pckVO90Z90(string command, string args)
	{
		args = args.Trim();
		if (!string.IsNullOrWhiteSpace(args))
		{
			ManagerBase<ModuleManager>.Instance().GetModuleByName(args)?.ToggleOverlayConfig();
		}
	}

	private static void bZkVKPRnZN(string moduleName, bool? isLoad = null)
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		if (string.IsNullOrWhiteSpace(moduleName))
		{
			return;
		}
		bool? flag = ManagerBase<ModuleManager>.Instance().LoadOrUnloadModuleByName(moduleName, isLoad);
		if (flag.HasValue)
		{
			if (flag == true)
			{
				OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2FDD929D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c6c2389b72fc4ab3828ae71fae281bf1), ManagerBase<ModuleManager>.Instance().GetModuleByName(moduleName).Info.Title)));
			}
			else
			{
				OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5E3995F5 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f9eb04af496747c5b32cddd51d0b3997), ManagerBase<ModuleManager>.Instance().GetModuleByName(moduleName).Info.Title)));
			}
		}
	}

	private static void U2BVtfqmU4(string favoriteName, bool isLoad)
	{
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		if (string.IsNullOrWhiteSpace(favoriteName))
		{
			return;
		}
		favoriteName = favoriteName.ToLowerInvariant().Trim();
		HashSet<string> value;
		HashSet<string> hashSet = ((!(favoriteName == GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2829891D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_95ac0b4adf764381bcdc75a9d241d184))) ? (PluginConfig.Instance().ModuleCustoms.TryGetValue(favoriteName, out value) ? value : new HashSet<string>()) : PluginConfig.Instance().ModuleFavorites);
		HashSet<string> hashSet2 = hashSet;
		if (hashSet2.Count == 0)
		{
			return;
		}
		ModuleBase[] array = hashSet2.Select(ManagerBase<ModuleManager>.Instance().GetModuleByName).ToArray();
		if (array.Length == 0)
		{
			return;
		}
		if (isLoad)
		{
			array.ForEach(delegate(ModuleBase x)
			{
				x.Load(affectConfig: true);
			});
		}
		else
		{
			array.ForEach(delegate(ModuleBase x)
			{
				x.Unload(affectConfig: true);
			});
		}
		OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(LanguageManager.Get(isLoad ? GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x76DA5FFB ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f8c073ebdce5414baf147b1be2d2c2c2) : GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4118DEA1 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_55c846b516154967bf0c321596d2dc52), favoriteName, array.Length)));
	}
}
