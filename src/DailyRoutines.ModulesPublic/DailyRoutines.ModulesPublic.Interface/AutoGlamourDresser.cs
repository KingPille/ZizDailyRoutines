using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;

namespace DailyRoutines.ModulesPublic.Interface;

public class AutoGlamourDresser : ModuleBase
{
	private class Config : ModuleConfig
	{
		public bool AutoSwitchJobCategory = true;

		public bool OnlyRemoveSetItemsWhenComplete = true;

		public bool SkipItemsWithStain = true;
	}

	private readonly record struct PrismBoxItemInfo(uint Index, uint RawItemID, uint ItemID, uint Stain0ID, uint Stain1ID, bool IsGlamorous, ulong ModelMain, uint EquipSlotCategoryRowID)
	{
		public bool IsGlamourModelValid
		{
			get
			{
				if (IsGlamorous && ModelMain != 0L)
				{
					return EquipSlotCategoryRowID != 0;
				}
				return false;
			}
		}

		public bool IsStained
		{
			get
			{
				if (Stain0ID == 0)
				{
					return Stain1ID != 0;
				}
				return true;
			}
		}
	}

	private sealed record MirageItemSet((uint ID, string Name) Set, List<(uint ID, string Name)> SetItems)
	{
		public static MirageItemSet? Parse(MirageStoreSetItem row)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			if (!LuminaGetter.TryGetRow<Item>(((MirageStoreSetItem)(ref row)).RowId, out Item item))
			{
				return null;
			}
			string text = ((object)((Item)(ref item)).Name/*cast due to constrained. prefix*/).ToString();
			if (string.IsNullOrWhiteSpace(text))
			{
				return null;
			}
			int num = 11;
			List<uint> list = new List<uint>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<uint> span = CollectionsMarshal.AsSpan(list);
			span[0] = ((MirageStoreSetItem)(ref row)).Body.RowId;
			span[1] = ((MirageStoreSetItem)(ref row)).Bracelets.RowId;
			span[2] = ((MirageStoreSetItem)(ref row)).Earrings.RowId;
			span[3] = ((MirageStoreSetItem)(ref row)).Feet.RowId;
			span[4] = ((MirageStoreSetItem)(ref row)).Hands.RowId;
			span[5] = ((MirageStoreSetItem)(ref row)).Head.RowId;
			span[6] = ((MirageStoreSetItem)(ref row)).Legs.RowId;
			span[7] = ((MirageStoreSetItem)(ref row)).Necklace.RowId;
			span[8] = ((MirageStoreSetItem)(ref row)).Ring.RowId;
			span[9] = ((MirageStoreSetItem)(ref row)).MainHand.RowId;
			span[10] = ((MirageStoreSetItem)(ref row)).OffHand.RowId;
			List<(uint, string)> list2 = list.Where((uint x) => x > 1 && LuminaGetter.TryGetRow<Item>(x, out Item _)).Select(delegate(uint x)
			{
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				Item value = LuminaGetter.GetRow<Item>(x).Value;
				return (x: x, ((object)((Item)(ref value)).Name/*cast due to constrained. prefix*/).ToString());
			}).ToList();
			if (list2.Count == 0)
			{
				return null;
			}
			return new MirageItemSet((ID: ((MirageStoreSetItem)(ref row)).RowId, Name: text), list2);
		}

		public bool TryGetRestorableIndexes(IReadOnlyDictionary<uint, PrismBoxItemInfo> itemsByID, bool onlyRemoveWhenComplete, bool skipItemsWithStain, out List<uint> itemIndexes)
		{
			itemIndexes = new List<uint>();
			foreach (var setItem in SetItems)
			{
				if (!itemsByID.TryGetValue(setItem.ID, out var value))
				{
					if (onlyRemoveWhenComplete)
					{
						return false;
					}
				}
				else if (skipItemsWithStain && value.IsStained)
				{
					if (onlyRemoveWhenComplete)
					{
						return false;
					}
				}
				else
				{
					itemIndexes.Add(value.Index);
				}
			}
			if (itemIndexes.Count > 0)
			{
				if (onlyRemoveWhenComplete)
				{
					return itemIndexes.Count == SetItems.Count;
				}
				return true;
			}
			return false;
		}
	}

	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static AddonEventDelegate _003C0_003E__OnAddonMiragePrismPrismSetConvertC;

		public static Func<List<PrismBoxItemInfo>, List<uint>> _003C1_003E__FilterDuplicateItems;

		public static Func<List<PrismBoxItemInfo>, List<uint>> _003C2_003E__FilterDuplicateModelItems;
	}

	private Config config;

	private static readonly FrozenSet<uint> ArmoireAvailableItems = (from x in (IEnumerable<Cabinet>)LuminaGetter.Get<Cabinet>()
		where ((Cabinet)(ref x)).Item.RowId != 0
		select ((Cabinet)(ref x)).Item.RowId).ToFrozenSet();

	private static readonly FrozenSet<MirageItemSet> MirageItemSets = (from x in ((IEnumerable<MirageStoreSetItem>)LuminaGetter.Get<MirageStoreSetItem>()).Select(MirageItemSet.Parse)
		where x != null
		select x).OfType<MirageItemSet>().ToFrozenSet();

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoGlamourDresserTitle"),
		Description = LanguageManager.Get("AutoGlamourDresserDescription"),
		Category = ModuleCategory.Interface
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected unsafe override void Init()
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected O, but got Unknown
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Expected O, but got Unknown
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Expected O, but got Unknown
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Expected O, but got Unknown
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 5000
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		OmenServiceBase<LogMessageManager>.Instance().RegPost(OnReceiveLogMessage);
		IAddonLifecycle obj2 = DalamudServiceExtension.Instance<IAddonLifecycle>();
		obj2.RegisterListener((AddonEvent)0, "MiragePrismPrismBox", new AddonEventDelegate(OnMiragePrismPrismBox));
		obj2.RegisterListener((AddonEvent)10, "MiragePrismPrismSetConvert", new AddonEventDelegate(OnAddonMiragePrismPrismSetConvert));
		obj2.RegisterListener((AddonEvent)6, "MiragePrismPrismSetConvert", new AddonEventDelegate(OnAddonMiragePrismPrismSetConvert));
		object obj3 = _003C_003EO._003C0_003E__OnAddonMiragePrismPrismSetConvertC;
		if (obj3 == null)
		{
			AddonEventDelegate val = OnAddonMiragePrismPrismSetConvertC;
			_003C_003EO._003C0_003E__OnAddonMiragePrismPrismSetConvertC = val;
			obj3 = (object)val;
		}
		obj2.RegisterListener((AddonEvent)1, "MiragePrismPrismSetConvertC", (AddonEventDelegate)obj3);
		if ((*Addons.MiragePrismPrismSetConvert).IsAddonAndNodesReady())
		{
			OnAddonMiragePrismPrismSetConvert((AddonEvent)10, null);
		}
		if ((*Addons.MiragePrismPrismSetConvertC).IsAddonAndNodesReady())
		{
			OnAddonMiragePrismPrismSetConvertC((AddonEvent)1, null);
		}
	}

	protected override void Uninit()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Expected O, but got Unknown
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Expected O, but got Unknown
		OmenServiceBase<LogMessageManager>.Instance().Unreg(OnReceiveLogMessage);
		IAddonLifecycle obj = DalamudServiceExtension.Instance<IAddonLifecycle>();
		AddonEventDelegate[] obj2 = new AddonEventDelegate[3]
		{
			OnMiragePrismPrismBox,
			OnAddonMiragePrismPrismSetConvert,
			default(AddonEventDelegate)
		};
		object obj3 = _003C_003EO._003C0_003E__OnAddonMiragePrismPrismSetConvertC;
		if (obj3 == null)
		{
			AddonEventDelegate val = OnAddonMiragePrismPrismSetConvertC;
			_003C_003EO._003C0_003E__OnAddonMiragePrismPrismSetConvertC = val;
			obj3 = (object)val;
		}
		obj2[2] = (AddonEventDelegate)obj3;
		obj.UnregisterListener((AddonEventDelegate[])(object)obj2);
	}

	protected unsafe override void ConfigUI()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("AutoGlamourDresser-AutoSwitchJobCategory")), ref config.AutoSwitchJobCategory))
		{
			config.Save(this);
		}
		ImGui.NewLine();
		ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("Operation")));
		IndentDisposable val = ImRaii.PushIndent(1, true);
		try
		{
			DisabledDisposable val2 = ImRaii.Disabled(base.TaskHelper.IsBusy || !(*Addons.MiragePrismPrismBox).IsAddonAndNodesReady());
			try
			{
				if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("AutoGlamourDresser-RemoveDuplicateItems")), default(Vector2)))
				{
					StartRestoreOperation(FilterDuplicateItems);
				}
				if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("AutoGlamourDresser-RemoveDuplicateModels")), default(Vector2)))
				{
					StartRestoreOperation(FilterDuplicateModelItems);
				}
				if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("AutoGlamourDresser-RemoveArmoireItems")), default(Vector2)))
				{
					StartRestoreOperation(FilterArmoireItems);
				}
				if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("AutoGlamourDresser-RemoveSetItems")), default(Vector2)))
				{
					StartAttireItems();
				}
				IndentDisposable val3 = ImRaii.PushIndent(1, true);
				try
				{
					if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("AutoGlamourDresser-OnlyRemoveSetItemsWhenComplete")), ref config.OnlyRemoveSetItemsWhenComplete))
					{
						config.Save(this);
					}
					ImGui.SameLine();
					if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("AutoGlamourDresser-SkipItemsWithStain")), ref config.SkipItemsWithStain))
					{
						config.Save(this);
					}
				}
				finally
				{
					((IDisposable)val3)?.Dispose();
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		val = ImRaii.PushIndent(1, true);
		try
		{
			DisabledDisposable val2 = ImRaii.Disabled(!base.TaskHelper.IsBusy);
			try
			{
				ImGui.Spacing();
				if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("Stop")), default(Vector2)))
				{
					base.TaskHelper.Abort();
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private unsafe void OnMiragePrismPrismBox(AddonEvent type, AddonArgs? args)
	{
		if (config.AutoSwitchJobCategory)
		{
			AddonMiragePrismPrismBox* miragePrismPrismBox = (AddonMiragePrismPrismBox*)Addons.MiragePrismPrismBox;
			if (miragePrismPrismBox != null)
			{
				((AddonMiragePrismPrismBox)miragePrismPrismBox).Param = (int)LocalPlayerState.ClassJob;
			}
		}
	}

	private void StartRestoreOperation(Func<List<PrismBoxItemInfo>, List<uint>> filter)
	{
		base.TaskHelper.Abort();
		QueueRestoreOperation(filter);
	}

	private unsafe void QueueRestoreOperation(Func<List<PrismBoxItemInfo>, List<uint>> filter)
	{
		if (!TryGetLoadedMirageManager(out var _))
		{
			return;
		}
		List<uint> list = filter(GetPrismBoxItems());
		if (list.Count != 0)
		{
			EnqueueRestoreItems(list);
			base.TaskHelper.Enqueue(delegate
			{
				QueueRestoreOperation(filter);
			});
		}
	}

	private static List<uint> FilterDuplicateItems(List<PrismBoxItemInfo> items)
	{
		List<uint> list = new List<uint>();
		HashSet<uint> hashSet = new HashSet<uint>();
		foreach (PrismBoxItemInfo item in items)
		{
			if (!hashSet.Add(item.ItemID))
			{
				list.Add(item.Index);
			}
		}
		return list;
	}

	private static List<uint> FilterDuplicateModelItems(List<PrismBoxItemInfo> items)
	{
		List<uint> list = new List<uint>();
		HashSet<uint> hashSet = new HashSet<uint>();
		HashSet<string> hashSet2 = new HashSet<string>();
		foreach (PrismBoxItemInfo item2 in items)
		{
			if (item2.IsGlamourModelValid)
			{
				string item = $"{item2.ModelMain}_{item2.EquipSlotCategoryRowID}";
				if (!hashSet.Add(item2.ItemID) || !hashSet2.Add(item))
				{
					list.Add(item2.Index);
				}
			}
		}
		return list;
	}

	private List<uint> FilterArmoireItems(List<PrismBoxItemInfo> items)
	{
		Dictionary<uint, PrismBoxItemInfo> dictionary = new Dictionary<uint, PrismBoxItemInfo>();
		Dictionary<uint, PrismBoxItemInfo> itemsByIndex = new Dictionary<uint, PrismBoxItemInfo>();
		foreach (PrismBoxItemInfo item in items)
		{
			dictionary.TryAdd(item.ItemID, item);
			itemsByIndex[item.Index] = item;
		}
		List<uint> list = new List<uint>();
		HashSet<uint> hashSet = new HashSet<uint>();
		foreach (var (_, list3) in GetRestorableItemSets(dictionary, onlyRemoveWhenComplete: false))
		{
			if (!list3.Any((uint itemIndex) => ArmoireAvailableItems.Contains(itemsByIndex[itemIndex].ItemID)))
			{
				continue;
			}
			foreach (uint item2 in list3)
			{
				if (hashSet.Add(item2))
				{
					list.Add(item2);
				}
			}
		}
		foreach (PrismBoxItemInfo item3 in items)
		{
			if (ArmoireAvailableItems.Contains(item3.ItemID) && hashSet.Add(item3.Index))
			{
				list.Add(item3.Index);
			}
		}
		return list;
	}

	private void StartAttireItems()
	{
		base.TaskHelper.Abort();
		RestoreItemsFromPrismBox();
	}

	private unsafe void RestoreItemsFromPrismBox()
	{
		if (!TryGetLoadedMirageManager(out var _))
		{
			return;
		}
		List<PrismBoxItemInfo> prismBoxItems = GetPrismBoxItems();
		if (prismBoxItems.Count == 0)
		{
			return;
		}
		Dictionary<uint, PrismBoxItemInfo> dictionary = new Dictionary<uint, PrismBoxItemInfo>();
		foreach (PrismBoxItemInfo item in prismBoxItems)
		{
			dictionary.TryAdd(item.ItemID, item);
		}
		Dictionary<MirageItemSet, List<uint>> restorableItemSets = GetRestorableItemSets(dictionary, config.OnlyRemoveSetItemsWhenComplete);
		if (restorableItemSets.Count == 0)
		{
			return;
		}
		foreach (KeyValuePair<MirageItemSet, List<uint>> item2 in restorableItemSets)
		{
			var (mirageItemSet2, itemIndexes) = (KeyValuePair<MirageItemSet, List<uint>>)(ref item2);
			EnqueueRestoreItems(itemIndexes, abortWhenInventoryFull: true);
			base.TaskHelper.Enqueue(delegate
			{
				//IL_0024: Unknown result type (might be due to invalid IL or missing references)
				//IL_002a: Expected O, but got Unknown
				//IL_0055: Unknown result type (might be due to invalid IL or missing references)
				OmenServiceBase<NotifyHelper>.Instance().Chat(LanguageManager.GetSe("AutoGlamourDresser-SetItemsMessage", (object)new ItemPayload(mirageItemSet2.Set.ID, (ItemKind)0, (string)null), itemIndexes.Count, mirageItemSet2.SetItems.Count));
			});
		}
		base.TaskHelper.Enqueue((Action)RestoreItemsFromPrismBox, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
	}

	private Dictionary<MirageItemSet, List<uint>> GetRestorableItemSets(IReadOnlyDictionary<uint, PrismBoxItemInfo> itemsByID, bool onlyRemoveWhenComplete)
	{
		Dictionary<MirageItemSet, List<uint>> dictionary = new Dictionary<MirageItemSet, List<uint>>();
		foreach (MirageItemSet mirageItemSet in MirageItemSets)
		{
			if (mirageItemSet.TryGetRestorableIndexes(itemsByID, onlyRemoveWhenComplete, config.SkipItemsWithStain, out List<uint> itemIndexes))
			{
				dictionary[mirageItemSet] = itemIndexes;
			}
		}
		return dictionary;
	}

	private void OnReceiveLogMessage(uint logMessageID, LogMessageQueueItem values)
	{
		if (logMessageID == 4280)
		{
			base.TaskHelper.Abort();
		}
	}

	private unsafe void OnAddonMiragePrismPrismSetConvert(AddonEvent type, AddonArgs? args)
	{
		if (Addons.MiragePrismPrismSetConvert != null)
		{
			FillMiragePrismBoxSet();
		}
	}

	private unsafe static void OnAddonMiragePrismPrismSetConvertC(AddonEvent type, AddonArgs? args)
	{
		if (Addons.MiragePrismPrismSetConvertC != null)
		{
			(*Addons.MiragePrismPrismSetConvertC).Callback(0);
		}
	}

	private unsafe void FillMiragePrismBoxSet()
	{
		if (!(*Addons.MiragePrismPrismSetConvert).IsAddonAndNodesReady() || base.TaskHelper.IsBusy)
		{
			return;
		}
		uint uInt = ((AtkValue)((byte*)((AtkUnitBase)Addons.MiragePrismPrismSetConvert).AtkValues + (nint)20 * (nint)Unsafe.SizeOf<AtkValue>())).UInt;
		if (uInt == 0)
		{
			return;
		}
		List<int> list = new List<int>();
		for (int i = 0; i < uInt; i++)
		{
			if (((AtkValue)((byte*)((AtkUnitBase)Addons.MiragePrismPrismSetConvert).AtkValues + (nint)(25 + i * 7) * (nint)Unsafe.SizeOf<AtkValue>())).UInt == 9999)
			{
				uint uInt2 = ((AtkValue)((byte*)((AtkUnitBase)Addons.MiragePrismPrismSetConvert).AtkValues + (nint)(26 + i * 7) * (nint)Unsafe.SizeOf<AtkValue>())).UInt;
				uint uInt3 = ((AtkValue)((byte*)((AtkUnitBase)Addons.MiragePrismPrismSetConvert).AtkValues + (nint)(27 + i * 7) * (nint)Unsafe.SizeOf<AtkValue>())).UInt;
				if (uInt2 != uInt3 || uInt3 != 0)
				{
					list.Add(i);
				}
			}
		}
		if (list.Count == 0)
		{
			return;
		}
		foreach (int item in list)
		{
			int index = item;
			base.TaskHelper.Enqueue(delegate
			{
				if (!(*Addons.ContextIconMenu).IsAddonAndNodesReady())
				{
					(*Addons.MiragePrismPrismSetConvert).Callback(13, index);
					return false;
				}
				(*Addons.ContextIconMenu).Callback(0, 0, 1021003u, 0u, 0);
				return true;
			});
		}
		base.TaskHelper.DelayNext(100);
		base.TaskHelper.Enqueue(delegate
		{
			((AgentId)305).SendEvent(1uL, 14);
		});
	}

	private unsafe static bool TryGetLoadedMirageManager(out MirageManager* instance)
	{
		instance = MirageManager.Instance();
		if (instance != null && ((MirageManager)instance).PrismBoxRequested)
		{
			return ((MirageManager)instance).PrismBoxLoaded;
		}
		return false;
	}

	private unsafe static List<PrismBoxItemInfo> GetPrismBoxItems()
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetLoadedMirageManager(out var instance))
		{
			return new List<PrismBoxItemInfo>();
		}
		List<PrismBoxItemInfo> list = new List<PrismBoxItemInfo>();
		for (uint num = 0u; num < 800; num++)
		{
			uint num2 = ((MirageManager)instance).PrismBoxItemIds[(int)num];
			if (num2 != 0)
			{
				uint num3 = num2 % 1000000;
				byte stain0ID = ((MirageManager)instance).PrismBoxStain0Ids[(int)num];
				byte stain1ID = ((MirageManager)instance).PrismBoxStain1Ids[(int)num];
				if (LuminaGetter.TryGetRow<Item>(num3, out Item item))
				{
					list.Add(new PrismBoxItemInfo(num, num2, num3, stain0ID, stain1ID, ((Item)(ref item)).IsGlamorous, ((Item)(ref item)).ModelMain, ((Item)(ref item)).EquipSlotCategory.RowId));
				}
				else
				{
					list.Add(new PrismBoxItemInfo(num, num2, num3, stain0ID, stain1ID, IsGlamorous: false, 0uL, 0u));
				}
			}
		}
		return list;
	}

	private unsafe void EnqueueRestoreItems(IEnumerable<uint> itemIndexes, bool abortWhenInventoryFull = false)
	{
		if (!TryGetLoadedMirageManager(out var instance))
		{
			return;
		}
		foreach (uint itemIndex in itemIndexes)
		{
			if (abortWhenInventoryFull)
			{
				base.TaskHelper.Enqueue(delegate
				{
					if (Inventories.Player.IsFull())
					{
						base.TaskHelper.Abort();
					}
				});
			}
			base.TaskHelper.Enqueue(() => ((MirageManager)instance).RestorePrismBoxItem(itemIndex));
		}
	}
}
