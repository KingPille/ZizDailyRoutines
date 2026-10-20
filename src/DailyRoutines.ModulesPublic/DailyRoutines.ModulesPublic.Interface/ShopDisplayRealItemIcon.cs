using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Threading;

namespace DailyRoutines.ModulesPublic.Interface;

public class ShopDisplayRealItemIcon : ModuleBase
{
	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static AddonEventDelegate _003C0_003E__OnShop;

		public static AddonEventDelegate _003C1_003E__OnInclusionShop;

		public static AddonEventDelegate _003C2_003E__OnGrandCompanyExchange;

		public static AddonEventDelegate _003C3_003E__OnShopExchange;

		public static AddonEventDelegate _003C4_003E__OnFreeShop;
	}

	private List<(uint ID, uint IconID, string Name)> collectablesShopItemDatas = new List<(uint, uint, string)>();

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("ShopDisplayRealItemIconTitle"),
		Description = LanguageManager.Get("ShopDisplayRealItemIconDescription"),
		Category = ModuleCategory.Interface
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
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Expected O, but got Unknown
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Expected O, but got Unknown
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Expected O, but got Unknown
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Expected O, but got Unknown
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Expected O, but got Unknown
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Expected O, but got Unknown
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Expected O, but got Unknown
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Expected O, but got Unknown
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Expected O, but got Unknown
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Expected O, but got Unknown
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Expected O, but got Unknown
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Expected O, but got Unknown
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Expected O, but got Unknown
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Expected O, but got Unknown
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Expected O, but got Unknown
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Expected O, but got Unknown
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Expected O, but got Unknown
		IAddonLifecycle obj = DalamudServiceExtension.Instance<IAddonLifecycle>();
		object obj2 = _003C_003EO._003C0_003E__OnShop;
		if (obj2 == null)
		{
			AddonEventDelegate val = OnShop;
			_003C_003EO._003C0_003E__OnShop = val;
			obj2 = (object)val;
		}
		obj.RegisterListener((AddonEvent)1, "Shop", (AddonEventDelegate)obj2);
		IAddonLifecycle obj3 = DalamudServiceExtension.Instance<IAddonLifecycle>();
		object obj4 = _003C_003EO._003C0_003E__OnShop;
		if (obj4 == null)
		{
			AddonEventDelegate val2 = OnShop;
			_003C_003EO._003C0_003E__OnShop = val2;
			obj4 = (object)val2;
		}
		obj3.RegisterListener((AddonEvent)9, "Shop", (AddonEventDelegate)obj4);
		IAddonLifecycle obj5 = DalamudServiceExtension.Instance<IAddonLifecycle>();
		object obj6 = _003C_003EO._003C0_003E__OnShop;
		if (obj6 == null)
		{
			AddonEventDelegate val3 = OnShop;
			_003C_003EO._003C0_003E__OnShop = val3;
			obj6 = (object)val3;
		}
		obj5.RegisterListener((AddonEvent)10, "Shop", (AddonEventDelegate)obj6);
		IAddonLifecycle obj7 = DalamudServiceExtension.Instance<IAddonLifecycle>();
		object obj8 = _003C_003EO._003C1_003E__OnInclusionShop;
		if (obj8 == null)
		{
			AddonEventDelegate val4 = OnInclusionShop;
			_003C_003EO._003C1_003E__OnInclusionShop = val4;
			obj8 = (object)val4;
		}
		obj7.RegisterListener((AddonEvent)1, "InclusionShop", (AddonEventDelegate)obj8);
		IAddonLifecycle obj9 = DalamudServiceExtension.Instance<IAddonLifecycle>();
		object obj10 = _003C_003EO._003C1_003E__OnInclusionShop;
		if (obj10 == null)
		{
			AddonEventDelegate val5 = OnInclusionShop;
			_003C_003EO._003C1_003E__OnInclusionShop = val5;
			obj10 = (object)val5;
		}
		obj9.RegisterListener((AddonEvent)9, "InclusionShop", (AddonEventDelegate)obj10);
		IAddonLifecycle obj11 = DalamudServiceExtension.Instance<IAddonLifecycle>();
		object obj12 = _003C_003EO._003C1_003E__OnInclusionShop;
		if (obj12 == null)
		{
			AddonEventDelegate val6 = OnInclusionShop;
			_003C_003EO._003C1_003E__OnInclusionShop = val6;
			obj12 = (object)val6;
		}
		obj11.RegisterListener((AddonEvent)10, "InclusionShop", (AddonEventDelegate)obj12);
		IAddonLifecycle obj13 = DalamudServiceExtension.Instance<IAddonLifecycle>();
		object obj14 = _003C_003EO._003C2_003E__OnGrandCompanyExchange;
		if (obj14 == null)
		{
			AddonEventDelegate val7 = OnGrandCompanyExchange;
			_003C_003EO._003C2_003E__OnGrandCompanyExchange = val7;
			obj14 = (object)val7;
		}
		obj13.RegisterListener((AddonEvent)1, "GrandCompanyExchange", (AddonEventDelegate)obj14);
		IAddonLifecycle obj15 = DalamudServiceExtension.Instance<IAddonLifecycle>();
		object obj16 = _003C_003EO._003C2_003E__OnGrandCompanyExchange;
		if (obj16 == null)
		{
			AddonEventDelegate val8 = OnGrandCompanyExchange;
			_003C_003EO._003C2_003E__OnGrandCompanyExchange = val8;
			obj16 = (object)val8;
		}
		obj15.RegisterListener((AddonEvent)9, "GrandCompanyExchange", (AddonEventDelegate)obj16);
		IAddonLifecycle obj17 = DalamudServiceExtension.Instance<IAddonLifecycle>();
		object obj18 = _003C_003EO._003C2_003E__OnGrandCompanyExchange;
		if (obj18 == null)
		{
			AddonEventDelegate val9 = OnGrandCompanyExchange;
			_003C_003EO._003C2_003E__OnGrandCompanyExchange = val9;
			obj18 = (object)val9;
		}
		obj17.RegisterListener((AddonEvent)10, "GrandCompanyExchange", (AddonEventDelegate)obj18);
		IAddonLifecycle obj19 = DalamudServiceExtension.Instance<IAddonLifecycle>();
		global::_003C_003Ez__ReadOnlyArray<string> obj20 = new global::_003C_003Ez__ReadOnlyArray<string>(new string[3] { "ShopExchangeCurrency", "ShopExchangeItem", "ShopExchangeCoin" });
		object obj21 = _003C_003EO._003C3_003E__OnShopExchange;
		if (obj21 == null)
		{
			AddonEventDelegate val10 = OnShopExchange;
			_003C_003EO._003C3_003E__OnShopExchange = val10;
			obj21 = (object)val10;
		}
		obj19.RegisterListener((AddonEvent)1, (IEnumerable<string>)obj20, (AddonEventDelegate)obj21);
		IAddonLifecycle obj22 = DalamudServiceExtension.Instance<IAddonLifecycle>();
		global::_003C_003Ez__ReadOnlyArray<string> obj23 = new global::_003C_003Ez__ReadOnlyArray<string>(new string[3] { "ShopExchangeCurrency", "ShopExchangeItem", "ShopExchangeCoin" });
		object obj24 = _003C_003EO._003C3_003E__OnShopExchange;
		if (obj24 == null)
		{
			AddonEventDelegate val11 = OnShopExchange;
			_003C_003EO._003C3_003E__OnShopExchange = val11;
			obj24 = (object)val11;
		}
		obj22.RegisterListener((AddonEvent)10, (IEnumerable<string>)obj23, (AddonEventDelegate)obj24);
		IAddonLifecycle obj25 = DalamudServiceExtension.Instance<IAddonLifecycle>();
		global::_003C_003Ez__ReadOnlyArray<string> obj26 = new global::_003C_003Ez__ReadOnlyArray<string>(new string[3] { "ShopExchangeCurrency", "ShopExchangeItem", "ShopExchangeCoin" });
		object obj27 = _003C_003EO._003C3_003E__OnShopExchange;
		if (obj27 == null)
		{
			AddonEventDelegate val12 = OnShopExchange;
			_003C_003EO._003C3_003E__OnShopExchange = val12;
			obj27 = (object)val12;
		}
		obj25.RegisterListener((AddonEvent)9, (IEnumerable<string>)obj26, (AddonEventDelegate)obj27);
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)5, "CollectablesShop", new AddonEventDelegate(OnCollectablesShop));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)9, "CollectablesShop", new AddonEventDelegate(OnCollectablesShop));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)10, "CollectablesShop", new AddonEventDelegate(OnCollectablesShop));
		IAddonLifecycle obj28 = DalamudServiceExtension.Instance<IAddonLifecycle>();
		object obj29 = _003C_003EO._003C4_003E__OnFreeShop;
		if (obj29 == null)
		{
			AddonEventDelegate val13 = OnFreeShop;
			_003C_003EO._003C4_003E__OnFreeShop = val13;
			obj29 = (object)val13;
		}
		obj28.RegisterListener((AddonEvent)1, "FreeShop", (AddonEventDelegate)obj29);
		IAddonLifecycle obj30 = DalamudServiceExtension.Instance<IAddonLifecycle>();
		object obj31 = _003C_003EO._003C4_003E__OnFreeShop;
		if (obj31 == null)
		{
			AddonEventDelegate val14 = OnFreeShop;
			_003C_003EO._003C4_003E__OnFreeShop = val14;
			obj31 = (object)val14;
		}
		obj30.RegisterListener((AddonEvent)9, "FreeShop", (AddonEventDelegate)obj31);
		IAddonLifecycle obj32 = DalamudServiceExtension.Instance<IAddonLifecycle>();
		object obj33 = _003C_003EO._003C4_003E__OnFreeShop;
		if (obj33 == null)
		{
			AddonEventDelegate val15 = OnFreeShop;
			_003C_003EO._003C4_003E__OnFreeShop = val15;
			obj33 = (object)val15;
		}
		obj32.RegisterListener((AddonEvent)10, "FreeShop", (AddonEventDelegate)obj33);
	}

	protected override void Uninit()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Expected O, but got Unknown
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Expected O, but got Unknown
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Expected O, but got Unknown
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Expected O, but got Unknown
		IAddonLifecycle obj = DalamudServiceExtension.Instance<IAddonLifecycle>();
		AddonEventDelegate[] array = new AddonEventDelegate[1];
		object obj2 = _003C_003EO._003C0_003E__OnShop;
		if (obj2 == null)
		{
			AddonEventDelegate val = OnShop;
			_003C_003EO._003C0_003E__OnShop = val;
			obj2 = (object)val;
		}
		array[0] = (AddonEventDelegate)obj2;
		obj.UnregisterListener((AddonEventDelegate[])(object)array);
		IAddonLifecycle obj3 = DalamudServiceExtension.Instance<IAddonLifecycle>();
		AddonEventDelegate[] array2 = new AddonEventDelegate[1];
		object obj4 = _003C_003EO._003C1_003E__OnInclusionShop;
		if (obj4 == null)
		{
			AddonEventDelegate val2 = OnInclusionShop;
			_003C_003EO._003C1_003E__OnInclusionShop = val2;
			obj4 = (object)val2;
		}
		array2[0] = (AddonEventDelegate)obj4;
		obj3.UnregisterListener((AddonEventDelegate[])(object)array2);
		IAddonLifecycle obj5 = DalamudServiceExtension.Instance<IAddonLifecycle>();
		AddonEventDelegate[] array3 = new AddonEventDelegate[1];
		object obj6 = _003C_003EO._003C2_003E__OnGrandCompanyExchange;
		if (obj6 == null)
		{
			AddonEventDelegate val3 = OnGrandCompanyExchange;
			_003C_003EO._003C2_003E__OnGrandCompanyExchange = val3;
			obj6 = (object)val3;
		}
		array3[0] = (AddonEventDelegate)obj6;
		obj5.UnregisterListener((AddonEventDelegate[])(object)array3);
		IAddonLifecycle obj7 = DalamudServiceExtension.Instance<IAddonLifecycle>();
		AddonEventDelegate[] array4 = new AddonEventDelegate[1];
		object obj8 = _003C_003EO._003C3_003E__OnShopExchange;
		if (obj8 == null)
		{
			AddonEventDelegate val4 = OnShopExchange;
			_003C_003EO._003C3_003E__OnShopExchange = val4;
			obj8 = (object)val4;
		}
		array4[0] = (AddonEventDelegate)obj8;
		obj7.UnregisterListener((AddonEventDelegate[])(object)array4);
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnCollectablesShop });
		IAddonLifecycle obj9 = DalamudServiceExtension.Instance<IAddonLifecycle>();
		AddonEventDelegate[] array5 = new AddonEventDelegate[1];
		object obj10 = _003C_003EO._003C4_003E__OnFreeShop;
		if (obj10 == null)
		{
			AddonEventDelegate val5 = OnFreeShop;
			_003C_003EO._003C4_003E__OnFreeShop = val5;
			obj10 = (object)val5;
		}
		array5[0] = (AddonEventDelegate)obj10;
		obj9.UnregisterListener((AddonEventDelegate[])(object)array5);
	}

	private unsafe static void OnFreeShop(AddonEvent type, AddonArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		AtkUnitBase* ptr = args.Addon.ToStruct();
		if (ptr == null)
		{
			return;
		}
		uint uInt = ((AtkValue)((byte*)((AtkUnitBase)ptr).AtkValues + (nint)76 * (nint)Unsafe.SizeOf<AtkValue>())).UInt;
		if (uInt == 0)
		{
			return;
		}
		for (int i = 0; i < uInt; i++)
		{
			uint uInt2 = ((AtkValue)((byte*)((AtkUnitBase)ptr).AtkValues + (nint)(138 + i) * (nint)Unsafe.SizeOf<AtkValue>())).UInt;
			if (uInt2 != 0 && LuminaGetter.TryGetRow<Item>(uInt2, out Item item))
			{
				((AtkValue)((byte*)((AtkUnitBase)ptr).AtkValues + (nint)(199 + i) * (nint)Unsafe.SizeOf<AtkValue>())).SetUInt((uint)((Item)(ref item)).Icon);
			}
		}
	}

	private unsafe void OnCollectablesShop(AddonEvent type, AddonArgs args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Invalid comparison between Unknown and I4
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		if ((int)type == 5 && !Throttler.Shared.Throttle("ShopDisplayRealItemIcon-OnCollectablesShop", 100u))
		{
			return;
		}
		AtkUnitBase* ptr = args.Addon.ToStruct();
		if (ptr == null)
		{
			return;
		}
		if ((int)type == 10)
		{
			uint uInt = ((AtkValue)((byte*)((AtkUnitBase)ptr).AtkValues + (nint)20 * (nint)Unsafe.SizeOf<AtkValue>())).UInt;
			if (uInt == 0)
			{
				return;
			}
			List<(uint, uint, string)> list = new List<(uint, uint, string)>();
			for (int i = 0; i < uInt; i++)
			{
				uint num = ((AtkValue)((byte*)((AtkUnitBase)ptr).AtkValues + (nint)(34 + 11 * i) * (nint)Unsafe.SizeOf<AtkValue>())).UInt % 500000;
				if (num != 0 && LuminaGetter.TryGetRow<Item>(num, out Item item))
				{
					list.Add((num, ((Item)(ref item)).Icon, ((object)((Item)(ref item)).Name/*cast due to constrained. prefix*/).ToString()));
				}
			}
			collectablesShopItemDatas = list;
		}
		if (collectablesShopItemDatas.Count == 0)
		{
			return;
		}
		AtkComponentNode* nodeById = (AtkComponentNode*)((AtkUnitBase)ptr).GetNodeById(28u);
		if (nodeById == null)
		{
			return;
		}
		for (int j = 0; j < 15; j++)
		{
			AtkComponentNode* ptr2 = (AtkComponentNode*)((AtkUldManager)(&((AtkComponentBase)((AtkComponentNode)nodeById).Component).UldManager)).NodeList[16 + j];
			if (ptr2 == null)
			{
				continue;
			}
			AtkTextNode* ptr3 = (AtkTextNode*)((AtkUldManager)(&((AtkComponentBase)((AtkComponentNode)ptr2).Component).UldManager)).SearchNodeById(4u);
			if (ptr3 == null)
			{
				break;
			}
			string name = ((object)new ReadOnlySeString(Utf8String.op_Implicit(ref ((AtkTextNode)ptr3).NodeText))/*cast due to constrained. prefix*/).ToString().SanitizeSEIcon();
			(uint, uint, string) tuple = collectablesShopItemDatas.FirstOrDefault<(uint, uint, string)>(((uint ID, uint IconID, string Name) x) => x.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
			(uint, uint, string) tuple2 = tuple;
			if (tuple2.Item1 != 0 || tuple2.Item2 != 0 || !(tuple2.Item3 == null))
			{
				AtkImageNode* ptr4 = (AtkImageNode*)((AtkUldManager)(&((AtkComponentBase)((AtkComponentNode)ptr2).Component).UldManager)).SearchNodeById(2u);
				if (ptr4 != null)
				{
					((AtkImageNode)ptr4).LoadIconTexture(tuple.Item2, 0);
				}
			}
		}
	}

	private unsafe static void OnShopExchange(AddonEvent type, AddonArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		AtkUnitBase* ptr = args.Addon.ToStruct();
		if (ptr == null)
		{
			return;
		}
		uint uInt = ((AtkValue)((byte*)((AtkUnitBase)ptr).AtkValues + (nint)4 * (nint)Unsafe.SizeOf<AtkValue>())).UInt;
		if (uInt == 0)
		{
			return;
		}
		for (int i = 0; i < uInt; i++)
		{
			uint uInt2 = ((AtkValue)((byte*)((AtkUnitBase)ptr).AtkValues + (nint)(1066 + i) * (nint)Unsafe.SizeOf<AtkValue>())).UInt;
			if (uInt2 != 0 && LuminaGetter.TryGetRow<Item>(uInt2, out Item item))
			{
				((AtkValue)((byte*)((AtkUnitBase)ptr).AtkValues + (nint)(212 + i) * (nint)Unsafe.SizeOf<AtkValue>())).SetUInt((uint)((Item)(ref item)).Icon);
			}
		}
	}

	private unsafe static void OnGrandCompanyExchange(AddonEvent type, AddonArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		AtkUnitBase* ptr = args.Addon.ToStruct();
		if (ptr == null)
		{
			return;
		}
		uint uInt = ((AtkValue)((byte*)((AtkUnitBase)ptr).AtkValues + Unsafe.SizeOf<AtkValue>())).UInt;
		if (uInt == 0)
		{
			return;
		}
		for (int i = 0; i < uInt; i++)
		{
			uint uInt2 = ((AtkValue)((byte*)((AtkUnitBase)ptr).AtkValues + (nint)(317 + i) * (nint)Unsafe.SizeOf<AtkValue>())).UInt;
			if (uInt2 != 0 && LuminaGetter.TryGetRow<Item>(uInt2, out Item item))
			{
				((AtkValue)((byte*)((AtkUnitBase)ptr).AtkValues + (nint)(167 + i) * (nint)Unsafe.SizeOf<AtkValue>())).SetUInt((uint)((Item)(ref item)).Icon);
			}
		}
	}

	private unsafe static void OnInclusionShop(AddonEvent type, AddonArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		AtkUnitBase* ptr = args.Addon.ToStruct();
		if (ptr == null)
		{
			return;
		}
		uint uInt = ((AtkValue)((byte*)((AtkUnitBase)ptr).AtkValues + (nint)298 * (nint)Unsafe.SizeOf<AtkValue>())).UInt;
		if (uInt == 0)
		{
			return;
		}
		for (int i = 0; i < uInt; i++)
		{
			uint uInt2 = ((AtkValue)((byte*)((AtkUnitBase)ptr).AtkValues + (nint)(300 + i * 18) * (nint)Unsafe.SizeOf<AtkValue>())).UInt;
			if (uInt2 != 0 && LuminaGetter.TryGetRow<Item>(uInt2, out Item item))
			{
				((AtkValue)((byte*)((AtkUnitBase)ptr).AtkValues + (nint)(301 + i * 18) * (nint)Unsafe.SizeOf<AtkValue>())).SetUInt((uint)((Item)(ref item)).Icon);
			}
		}
	}

	private unsafe static void OnShop(AddonEvent type, AddonArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		AtkUnitBase* ptr = args.Addon.ToStruct();
		if (ptr == null)
		{
			return;
		}
		uint uInt = ((AtkValue)((AtkUnitBase)ptr).AtkValues).UInt;
		uint uInt2 = ((AtkValue)((byte*)((AtkUnitBase)ptr).AtkValues + (nint)2 * (nint)Unsafe.SizeOf<AtkValue>())).UInt;
		if (uInt2 == 0)
		{
			return;
		}
		for (int i = 0; i < uInt2; i++)
		{
			uint num = 0u;
			bool flag = false;
			switch (uInt)
			{
			case 0u:
				num = ((AtkValue)((byte*)((AtkUnitBase)ptr).AtkValues + (nint)(441 + i) * (nint)Unsafe.SizeOf<AtkValue>())).UInt;
				break;
			case 1u:
			{
				BuybackItem val = ((ShopEventHandler)((AgentProxy)AgentProxy.Instance()).Handler).Buyback[i];
				flag = ((Enum)val.Flags).HasFlag((Enum)(object)(ItemFlags)1);
				num = val.ItemId;
				break;
			}
			}
			if (num != 0 && LuminaGetter.TryGetRow<Item>(num, out Item item))
			{
				((AtkValue)((byte*)((AtkUnitBase)ptr).AtkValues + (nint)(197 + i) * (nint)Unsafe.SizeOf<AtkValue>())).SetUInt((uint)(((Item)(ref item)).Icon + (flag ? 1000000 : 0)));
			}
		}
	}
}
