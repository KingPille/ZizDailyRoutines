using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Enums;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using InteropGenerator.Runtime;
using Lumina.Text;
using Lumina.Text.ReadOnly;
using OmenTools.Dalamud;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.OmenService;

public class TooltipManager : OmenServiceBase<TooltipManager>
{
	public delegate void ItemTooltipUpdateDelegate(ItemKind itemKind, uint itemID, ref List<TooltipItemModification> modifications);

	public delegate void ActionTooltipUpdateDelegate(DetailKind actionKind, uint actionID, ref List<TooltipActionModification> modifications);

	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static AddonEventDelegate _003C0_003E__OnItemDetailPostUpdate;
	}

	private static readonly CompSig AgentItemDetailRefreshFlagOffsetSig = new CompSig("88 83 ?? ?? ?? ?? 48 8B 5C 24 ?? 48 8B 6C 24");

	private nint agentItemDetailRefreshFlagOffset;

	private (uint ID, ItemKind Kind) lastItemInfo;

	private ReadOnlySeString[] itemOriginalTexts = (ReadOnlySeString[])(object)new ReadOnlySeString[65];

	private (DetailKind Kind, uint ID) lastActionInfo;

	private ReadOnlySeString[] actionOriginalTexts = (ReadOnlySeString[])(object)new ReadOnlySeString[16];

	private readonly ConcurrentDictionary<Type, ImmutableList<Delegate>> methodsCollection = new ConcurrentDictionary<Type, ImmutableList<Delegate>>();

	public unsafe void TriggerItemDetailUpdate()
	{
		DalamudServiceExtension.Instance<IFramework>().RunOnFrameworkThread((Action)delegate
		{
			if ((*Addons.ItemDetail).IsAddonAndNodesReady())
			{
				((sbyte*)AgentItemDetail.Instance())[agentItemDetailRefreshFlagOffset] = 1;
				DLog.Verbose("TooltipManager: 触发更新物品工具信息界面");
			}
		});
	}

	public unsafe void TriggerActionDetailUpdate()
	{
		DalamudServiceExtension.Instance<IFramework>().RunOnFrameworkThread((Action)delegate
		{
			if ((*Addons.ActionDetail).IsAddonAndNodesReady())
			{
				((AtkUnitBase)Addons.ActionDetail).OnRequestedUpdate(((AtkStage)AtkStage.Instance()).GetNumberArrayData(), ((AtkStage)AtkStage.Instance()).GetStringArrayData());
				DLog.Verbose("TooltipManager: 触发更新技能工具信息界面");
			}
		});
	}

	public ReadOnlySeString GetOriginalItemTooltipText(TooltipItemType target)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return itemOriginalTexts[(uint)target];
	}

	public ReadOnlySeString GetOriginalActionTooltipText(TooltipActionType target)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return actionOriginalTexts[(int)target];
	}

	public void RegItem(ItemTooltipUpdateDelegate method, params ItemTooltipUpdateDelegate[] methods)
	{
		RegisterGeneric(method, methods);
	}

	public void RegAction(ActionTooltipUpdateDelegate method, params ActionTooltipUpdateDelegate[] methods)
	{
		RegisterGeneric(method, methods);
	}

	public void Unreg(params ItemTooltipUpdateDelegate[] methods)
	{
		UnregisterGeneric(methods);
	}

	public void Unreg(params ActionTooltipUpdateDelegate[] methods)
	{
		UnregisterGeneric(methods);
	}

	protected override void Init()
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected O, but got Unknown
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Expected O, but got Unknown
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Expected O, but got Unknown
		agentItemDetailRefreshFlagOffset = Marshal.ReadInt32(AgentItemDetailRefreshFlagOffsetSig.ScanText() + 2);
		DLog.Debug($"[{"TooltipManager"}] AgnetItemDetail 工具信息界面刷新标志偏移量: {agentItemDetailRefreshFlagOffset}");
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)7, "ItemDetail", new AddonEventDelegate(OnItemDetailUpdate));
		IAddonLifecycle obj = DalamudServiceExtension.Instance<IAddonLifecycle>();
		object obj2 = _003C_003EO._003C0_003E__OnItemDetailPostUpdate;
		if (obj2 == null)
		{
			AddonEventDelegate val = OnItemDetailPostUpdate;
			_003C_003EO._003C0_003E__OnItemDetailPostUpdate = val;
			obj2 = (object)val;
		}
		obj.RegisterListener((AddonEvent)8, "ItemDetail", (AddonEventDelegate)obj2);
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)7, "ActionDetail", new AddonEventDelegate(OnActionDetailUpdate));
	}

	protected override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected O, but got Unknown
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnItemDetailUpdate });
		IAddonLifecycle obj = DalamudServiceExtension.Instance<IAddonLifecycle>();
		AddonEventDelegate[] array = new AddonEventDelegate[1];
		object obj2 = _003C_003EO._003C0_003E__OnItemDetailPostUpdate;
		if (obj2 == null)
		{
			AddonEventDelegate val = OnItemDetailPostUpdate;
			_003C_003EO._003C0_003E__OnItemDetailPostUpdate = val;
			obj2 = (object)val;
		}
		array[0] = (AddonEventDelegate)obj2;
		obj.UnregisterListener((AddonEventDelegate[])(object)array);
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnActionDetailUpdate });
	}

	private unsafe void OnItemDetailUpdate(AddonEvent type, AddonArgs args)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0587: Unknown result type (might be due to invalid IL or missing references)
		//IL_0594: Unknown result type (might be due to invalid IL or missing references)
		//IL_0599: Unknown result type (might be due to invalid IL or missing references)
		//IL_0517: Unknown result type (might be due to invalid IL or missing references)
		//IL_0524: Unknown result type (might be due to invalid IL or missing references)
		//IL_0529: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0604: Unknown result type (might be due to invalid IL or missing references)
		//IL_0609: Unknown result type (might be due to invalid IL or missing references)
		StringArrayData* stringArrayData = ((AtkStage)AtkStage.Instance()).GetStringArrayData((StringArrayType)27);
		CStringPointer* stringArray = ((StringArrayData)stringArrayData).StringArray;
		NumberArrayData* numberArrayData = ((AtkStage)AtkStage.Instance()).GetNumberArrayData((NumberArrayType)30);
		TooltipItemGroupFlags itemTooltipGroupFlags = GetItemTooltipGroupFlags(numberArrayData);
		(uint, ItemKind) itemInfo = GetItemInfo(((AgentItemDetail)AgentItemDetail.Instance()).ItemId);
		(uint, ItemKind) tuple = itemInfo;
		(uint, ItemKind) tuple2 = lastItemInfo;
		if (tuple.Item1 != tuple2.Item1 || tuple.Item2 != tuple2.Item2)
		{
			lastItemInfo = itemInfo;
			itemOriginalTexts = (ReadOnlySeString[])(object)new ReadOnlySeString[65];
			DLog.Verbose($"[{"TooltipManager"}] 物品工具提示内容刷新: {lastItemInfo}");
			for (int i = 0; i < itemOriginalTexts.Length; i++)
			{
				if (!IsItemTooltipTextSet(itemTooltipGroupFlags, (TooltipItemType)i) || !((CStringPointer)((byte*)stringArray + (nint)i * (nint)Unsafe.SizeOf<CStringPointer>())).HasValue)
				{
					itemOriginalTexts[i] = default(ReadOnlySeString);
					continue;
				}
				ReadOnlySeString[] array = itemOriginalTexts;
				int num = i;
				CStringPointer val = new CStringPointer(((CStringPointer)((byte*)stringArray + (nint)i * (nint)Unsafe.SizeOf<CStringPointer>())).Value);
				array[num] = new ReadOnlySeString(((CStringPointer)(ref val)).AsSpan());
			}
		}
		AddonItemDetail* itemDetail = (AddonItemDetail*)Addons.ItemDetail;
		if ((*Addons.ItemDetail).IsAddonAndNodesReady())
		{
			TextFlags* textFlags = &((AtkTextNode)((AddonItemDetail)itemDetail).CategoryText).TextFlags;
			*textFlags = (TextFlags)((uint)(*textFlags) & 0xFBFFu);
		}
		DLog.Verbose($"[{"TooltipManager"}] 物品工具提示刷新: {lastItemInfo}");
		if (!methodsCollection.TryGetValue(typeof(ItemTooltipUpdateDelegate), out ImmutableList<Delegate> value))
		{
			return;
		}
		Dictionary<TooltipItemType, (List<TooltipItemModification>, List<TooltipItemModification>, List<TooltipItemModification>)> dictionary = new Dictionary<TooltipItemType, (List<TooltipItemModification>, List<TooltipItemModification>, List<TooltipItemModification>)>();
		foreach (ItemTooltipUpdateDelegate item in value)
		{
			List<TooltipItemModification> modifications = new List<TooltipItemModification>();
			item(itemInfo.Item2, itemInfo.Item1, ref modifications);
			foreach (TooltipItemModification item2 in modifications)
			{
				if (!dictionary.TryGetValue(item2.Target, out var value2))
				{
					value2 = (new List<TooltipItemModification>(), new List<TooltipItemModification>(), new List<TooltipItemModification>());
					dictionary[item2.Target] = value2;
				}
				switch (item2.Type)
				{
				case TooltipModificationType.Prepend:
					value2.Item1.Add(item2);
					break;
				case TooltipModificationType.Contribute:
					value2.Item2.Add(item2);
					break;
				case TooltipModificationType.Append:
					value2.Item3.Add(item2);
					break;
				default:
					throw new ArgumentOutOfRangeException("type");
				}
			}
		}
		List<TooltipItemType> list = new List<TooltipItemType>();
		RentedSeStringBuilder val2 = default(RentedSeStringBuilder);
		foreach (KeyValuePair<TooltipItemType, (List<TooltipItemModification>, List<TooltipItemModification>, List<TooltipItemModification>)> item3 in dictionary)
		{
			item3.Deconstruct(out var key, out var value3);
			TooltipItemType tooltipItemType = key;
			(List<TooltipItemModification>, List<TooltipItemModification>, List<TooltipItemModification>) tuple3 = value3;
			int num2 = (int)tooltipItemType;
			if ((uint)num2 >= (uint)itemOriginalTexts.Length)
			{
				continue;
			}
			((RentedSeStringBuilder)(ref val2))._002Ector();
			try
			{
				SeStringBuilder builder = ((RentedSeStringBuilder)(ref val2)).Builder;
				bool flag = false;
				ReadOnlySeString text;
				foreach (TooltipItemModification item4 in tuple3.Item1)
				{
					if (!item4.IsParagraph)
					{
						if (flag)
						{
							builder.Append(UITexts.Space);
						}
						builder.Append(item4.Text);
						text = item4.Text;
						if (!((ReadOnlySeString)(ref text)).IsEmpty)
						{
							flag = true;
						}
					}
				}
				foreach (TooltipItemModification item5 in tuple3.Item1)
				{
					if (item5.IsParagraph)
					{
						if (flag)
						{
							builder.AppendNewLine().AppendNewLine();
						}
						builder.Append(item5.Text);
						text = item5.Text;
						if (!((ReadOnlySeString)(ref text)).IsEmpty)
						{
							flag = true;
						}
					}
				}
				if (tuple3.Item2.Count <= 0)
				{
					if (!((ReadOnlySeString)(ref itemOriginalTexts[num2])).IsEmpty)
					{
						if (flag)
						{
							builder.Append(UITexts.Space);
						}
						builder.Append(itemOriginalTexts[num2]);
						flag = true;
					}
				}
				else
				{
					foreach (TooltipItemModification item6 in tuple3.Item2)
					{
						if (!item6.IsParagraph)
						{
							if (flag)
							{
								builder.Append(UITexts.Space);
							}
							builder.Append(item6.Text);
							text = item6.Text;
							if (!((ReadOnlySeString)(ref text)).IsEmpty)
							{
								flag = true;
							}
						}
					}
					foreach (TooltipItemModification item7 in tuple3.Item2)
					{
						if (item7.IsParagraph)
						{
							if (flag)
							{
								builder.AppendNewLine().AppendNewLine();
							}
							builder.Append(item7.Text);
							text = item7.Text;
							if (!((ReadOnlySeString)(ref text)).IsEmpty)
							{
								flag = true;
							}
						}
					}
				}
				foreach (TooltipItemModification item8 in tuple3.Item3)
				{
					if (!item8.IsParagraph)
					{
						if (flag)
						{
							builder.Append(UITexts.Space);
						}
						builder.Append(item8.Text);
						text = item8.Text;
						if (!((ReadOnlySeString)(ref text)).IsEmpty)
						{
							flag = true;
						}
					}
				}
				foreach (TooltipItemModification item9 in tuple3.Item3)
				{
					if (item9.IsParagraph)
					{
						if (flag)
						{
							builder.AppendNewLine().AppendNewLine();
						}
						builder.Append(item9.Text);
						text = item9.Text;
						if (!((ReadOnlySeString)(ref text)).IsEmpty)
						{
							flag = true;
						}
					}
				}
				ReadOnlySpan<byte> viewAsSpan = builder.GetViewAsSpan();
				((StringArrayData)stringArrayData).SetValue(num2, viewAsSpan, true, true, true);
				if (viewAsSpan.Length == 0 && !((ReadOnlySeString)(ref itemOriginalTexts[num2])).IsEmpty)
				{
					list.Add(tooltipItemType);
				}
			}
			finally
			{
				((IDisposable)(*(RentedSeStringBuilder*)(&val2))/*cast due to constrained. prefix*/).Dispose();
			}
		}
		SetItemTooltipGroupFlags(numberArrayData, dictionary.Keys);
		if (list.Count > 0)
		{
			ClearItemTooltipGroupFlags(numberArrayData, list);
			RecalculateItemDetailLayout(numberArrayData, stringArrayData);
		}
	}

	private unsafe static void OnItemDetailPostUpdate(AddonEvent type, AddonArgs args)
	{
		if ((*Addons.ItemDetail).IsAddonAndNodesReady())
		{
			AddonItemDetail* itemDetail = (AddonItemDetail*)Addons.ItemDetail;
			if (((AtkTextNode)((AddonItemDetail)itemDetail).DescriptionText).Width != 350)
			{
				((AtkTextNode)((AddonItemDetail)itemDetail).DescriptionText).Width = 350;
			}
			if (((AtkTextNode)((AddonItemDetail)itemDetail).MaterializeText).Width != 350)
			{
				((AtkTextNode)((AddonItemDetail)itemDetail).MaterializeText).Width = 350;
				((AtkTextNode)((AddonItemDetail)itemDetail).MaterializeText).TextFlags |= (TextFlags)192;
				((AtkTextNode)((AddonItemDetail)itemDetail).MaterializeText).LineSpacing = Math.Max(((AtkTextNode)((AddonItemDetail)itemDetail).MaterializeText).FontSize, (byte)14);
				((AtkTextNode)((AddonItemDetail)itemDetail).MaterializeText).ApplyTextFlow();
			}
			ushort num = default(ushort);
			ushort height = default(ushort);
			((AtkTextNode)((AddonItemDetail)itemDetail).MaterializeText).GetTextDrawSize(&num, &height, (byte*)null, 0, -1, false);
			((AtkTextNode)((AddonItemDetail)itemDetail).MaterializeText).SetHeight(height);
			((AtkResNode)((AddonItemDetail)itemDetail).CraftingAndRepairsGroup).SetHeight((ushort)(((AtkTextNode)((AddonItemDetail)itemDetail).MaterializeText).Y + (float)(int)((AtkTextNode)((AddonItemDetail)itemDetail).MaterializeText).Height));
			((AddonItemDetail)itemDetail).UpdateGroupPositions(((AtkStage)AtkStage.Instance()).GetNumberArrayData((NumberArrayType)30), ((AtkStage)AtkStage.Instance()).GetStringArrayData((StringArrayType)27));
		}
	}

	private unsafe void OnActionDetailUpdate(AddonEvent type, AddonArgs args)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0467: Unknown result type (might be due to invalid IL or missing references)
		//IL_046c: Unknown result type (might be due to invalid IL or missing references)
		//IL_053a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0547: Unknown result type (might be due to invalid IL or missing references)
		//IL_054c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
		StringArrayData* stringArrayData = ((AtkStage)AtkStage.Instance()).GetStringArrayData((StringArrayType)29);
		CStringPointer* stringArray = ((StringArrayData)stringArrayData).StringArray;
		(DetailKind, uint) tuple = (((AgentActionDetail)AgentActionDetail.Instance()).ActionKind, ((AgentActionDetail)AgentActionDetail.Instance()).ActionId);
		(DetailKind, uint) tuple2 = tuple;
		(DetailKind, uint) tuple3 = lastActionInfo;
		if (tuple2.Item1 != tuple3.Item1 || tuple2.Item2 != tuple3.Item2)
		{
			lastActionInfo = tuple;
			actionOriginalTexts = (ReadOnlySeString[])(object)new ReadOnlySeString[16];
			DLog.Verbose($"[{"TooltipManager"}] 技能工具提示内容刷新: {lastActionInfo}");
			for (int i = 0; i < actionOriginalTexts.Length; i++)
			{
				if (!((CStringPointer)((byte*)stringArray + (nint)i * (nint)Unsafe.SizeOf<CStringPointer>())).HasValue)
				{
					actionOriginalTexts[i] = default(ReadOnlySeString);
					continue;
				}
				ReadOnlySeString[] array = actionOriginalTexts;
				int num = i;
				CStringPointer val = new CStringPointer(((CStringPointer)((byte*)stringArray + (nint)i * (nint)Unsafe.SizeOf<CStringPointer>())).Value);
				array[num] = new ReadOnlySeString(((CStringPointer)(ref val)).AsSpan());
			}
		}
		DLog.Verbose($"[{"TooltipManager"}] 物品工具提示刷新: {lastItemInfo}");
		if (!methodsCollection.TryGetValue(typeof(ActionTooltipUpdateDelegate), out ImmutableList<Delegate> value))
		{
			return;
		}
		Dictionary<TooltipActionType, (List<TooltipActionModification>, List<TooltipActionModification>, List<TooltipActionModification>)> dictionary = new Dictionary<TooltipActionType, (List<TooltipActionModification>, List<TooltipActionModification>, List<TooltipActionModification>)>();
		foreach (ActionTooltipUpdateDelegate item in value)
		{
			List<TooltipActionModification> modifications = new List<TooltipActionModification>();
			item(tuple.Item1, tuple.Item2, ref modifications);
			foreach (TooltipActionModification item2 in modifications)
			{
				if (!dictionary.TryGetValue(item2.Target, out var value2))
				{
					value2 = (new List<TooltipActionModification>(), new List<TooltipActionModification>(), new List<TooltipActionModification>());
					dictionary[item2.Target] = value2;
				}
				switch (item2.Type)
				{
				case TooltipModificationType.Prepend:
					value2.Item1.Add(item2);
					break;
				case TooltipModificationType.Contribute:
					value2.Item2.Add(item2);
					break;
				case TooltipModificationType.Append:
					value2.Item3.Add(item2);
					break;
				default:
					throw new ArgumentOutOfRangeException("type");
				}
			}
		}
		RentedSeStringBuilder val2 = default(RentedSeStringBuilder);
		foreach (KeyValuePair<TooltipActionType, (List<TooltipActionModification>, List<TooltipActionModification>, List<TooltipActionModification>)> item3 in dictionary)
		{
			item3.Deconstruct(out var key, out var value3);
			TooltipActionType num2 = key;
			(List<TooltipActionModification>, List<TooltipActionModification>, List<TooltipActionModification>) tuple4 = value3;
			int num3 = (int)num2;
			if ((uint)num3 >= (uint)actionOriginalTexts.Length)
			{
				continue;
			}
			((RentedSeStringBuilder)(ref val2))._002Ector();
			try
			{
				SeStringBuilder builder = ((RentedSeStringBuilder)(ref val2)).Builder;
				bool flag = false;
				ReadOnlySeString text;
				foreach (TooltipActionModification item4 in tuple4.Item1)
				{
					if (!item4.IsParagraph)
					{
						if (flag)
						{
							builder.Append(UITexts.Space);
						}
						builder.Append(item4.Text);
						text = item4.Text;
						if (!((ReadOnlySeString)(ref text)).IsEmpty)
						{
							flag = true;
						}
					}
				}
				foreach (TooltipActionModification item5 in tuple4.Item1)
				{
					if (item5.IsParagraph)
					{
						if (flag)
						{
							builder.AppendNewLine().AppendNewLine();
						}
						builder.Append(item5.Text);
						text = item5.Text;
						if (!((ReadOnlySeString)(ref text)).IsEmpty)
						{
							flag = true;
						}
					}
				}
				if (tuple4.Item2.Count <= 0)
				{
					if (!((ReadOnlySeString)(ref actionOriginalTexts[num3])).IsEmpty)
					{
						if (flag)
						{
							builder.Append(UITexts.Space);
						}
						builder.Append(actionOriginalTexts[num3]);
						flag = true;
					}
				}
				else
				{
					foreach (TooltipActionModification item6 in tuple4.Item2)
					{
						if (!item6.IsParagraph)
						{
							if (flag)
							{
								builder.Append(UITexts.Space);
							}
							builder.Append(item6.Text);
							text = item6.Text;
							if (!((ReadOnlySeString)(ref text)).IsEmpty)
							{
								flag = true;
							}
						}
					}
					foreach (TooltipActionModification item7 in tuple4.Item2)
					{
						if (item7.IsParagraph)
						{
							if (flag)
							{
								builder.AppendNewLine().AppendNewLine();
							}
							builder.Append(item7.Text);
							text = item7.Text;
							if (!((ReadOnlySeString)(ref text)).IsEmpty)
							{
								flag = true;
							}
						}
					}
				}
				foreach (TooltipActionModification item8 in tuple4.Item3)
				{
					if (!item8.IsParagraph)
					{
						if (flag)
						{
							builder.Append(UITexts.Space);
						}
						builder.Append(item8.Text);
						text = item8.Text;
						if (!((ReadOnlySeString)(ref text)).IsEmpty)
						{
							flag = true;
						}
					}
				}
				foreach (TooltipActionModification item9 in tuple4.Item3)
				{
					if (item9.IsParagraph)
					{
						if (flag)
						{
							builder.AppendNewLine().AppendNewLine();
						}
						builder.Append(item9.Text);
						text = item9.Text;
						if (!((ReadOnlySeString)(ref text)).IsEmpty)
						{
							flag = true;
						}
					}
				}
				((StringArrayData)stringArrayData).SetValue(num3, builder.GetViewAsSpan(), true, true, true);
			}
			finally
			{
				((IDisposable)(*(RentedSeStringBuilder*)(&val2))/*cast due to constrained. prefix*/).Dispose();
			}
		}
	}

	private void RegisterGeneric<T>(T method, params T[] methods) where T : Delegate
	{
		Type typeFromHandle = typeof(T);
		methodsCollection.AddOrUpdate(typeFromHandle, delegate
		{
			ImmutableList<Delegate> immutableList = ImmutableList.Create((Delegate)method);
			return (methods.Length == 0) ? immutableList : immutableList.AddRange(methods);
		}, delegate(Type _, ImmutableList<Delegate> currentList)
		{
			ImmutableList<Delegate> immutableList = currentList.Add(method);
			return (methods.Length == 0) ? immutableList : immutableList.AddRange(methods);
		});
	}

	private void UnregisterGeneric<T>(params T[] methods) where T : Delegate
	{
		if (methods == null || methods.Length <= 0)
		{
			return;
		}
		Type typeFromHandle = typeof(T);
		ImmutableList<Delegate> value;
		while (methodsCollection.TryGetValue(typeFromHandle, out value))
		{
			ImmutableList<Delegate> immutableList = value.RemoveRange(methods);
			if (immutableList == value)
			{
				break;
			}
			if (immutableList.IsEmpty)
			{
				KeyValuePair<Type, ImmutableList<Delegate>> item = new KeyValuePair<Type, ImmutableList<Delegate>>(typeFromHandle, value);
				if (((ICollection<KeyValuePair<Type, ImmutableList<Delegate>>>)methodsCollection).Remove(item))
				{
					break;
				}
			}
			else if (methodsCollection.TryUpdate(typeFromHandle, immutableList, value))
			{
				break;
			}
		}
	}

	private static (uint ID, ItemKind Kind) GetItemInfo(uint itemID)
	{
		if (itemID > 1000000)
		{
			if (itemID > 2000000)
			{
				return (ID: itemID, Kind: (ItemKind)2000000);
			}
			itemID %= 1000000;
			return (ID: itemID, Kind: (ItemKind)1000000);
		}
		if (itemID > 500000)
		{
			itemID %= 500000;
			return (ID: itemID, Kind: (ItemKind)500000);
		}
		return (ID: itemID, Kind: (ItemKind)0);
	}

	private unsafe static TooltipItemGroupFlags GetItemTooltipGroupFlags(NumberArrayData* numberArrayData)
	{
		return (TooltipItemGroupFlags)((NumberArrayData)numberArrayData).IntArray[5];
	}

	private static bool IsItemTooltipTextSet(TooltipItemGroupFlags flags, TooltipItemType target)
	{
		bool flag = (flags & TooltipItemGroupFlags.HeaderStatsGroup) != 0;
		switch (target)
		{
		case TooltipItemType.Name:
		case TooltipItemType.GlamourName:
		case TooltipItemType.UICategory:
		case TooltipItemType.OwnedCount:
		case TooltipItemType.ItemLevel:
		case TooltipItemType.ControlHelp:
			return true;
		case TooltipItemType.MainParam0Name:
		case TooltipItemType.MainParam1Name:
		case TooltipItemType.MainParam2Name:
		case TooltipItemType.MainParam0Value:
		case TooltipItemType.MainParam1Value:
		case TooltipItemType.MainParam2Value:
		case TooltipItemType.MainParam0OffsetValue:
		case TooltipItemType.MainParam1OffsetValue:
		case TooltipItemType.MainParam2OffsetValue:
			return flag;
		case TooltipItemType.MarkerName:
			return (flags & TooltipItemGroupFlags.CrafterName) != 0;
		case TooltipItemType.Description:
			return (flags & TooltipItemGroupFlags.Description) != 0;
		case TooltipItemType.SellInfo:
			return (flags & TooltipItemGroupFlags.Marketable) != 0;
		case TooltipItemType.ClassJobCategory:
		case TooltipItemType.ClassJobLevel:
			return ((uint)flags & (uint)(flag ? 32768 : 8)) != 0;
		case TooltipItemType.EffectTitle:
		case TooltipItemType.Effect:
			return (flags & TooltipItemGroupFlags.Bonuses) != 0;
		case TooltipItemType.SpecialTitle:
		case TooltipItemType.SpecialParam0:
		case TooltipItemType.SpecialParam1:
		case TooltipItemType.SpecialParam2:
		case TooltipItemType.SpecialParam3:
		case TooltipItemType.SpecialParam4:
			return flag && (flags & TooltipItemGroupFlags.Effects) != 0;
		case TooltipItemType.AttachMateriaTitle:
		case TooltipItemType.AttachableGearCategory:
		case TooltipItemType.AttachableGearContent:
		case TooltipItemType.MateriaTitle:
		case TooltipItemType.AttachedMateria0:
		case TooltipItemType.AttachedMateria1:
		case TooltipItemType.AttachedMateria2:
		case TooltipItemType.AttachedMateria3:
		case TooltipItemType.AttachedMateria4:
		case TooltipItemType.AttachedMateria0Param:
		case TooltipItemType.AttachedMateria1Param:
		case TooltipItemType.AttachedMateria2Param:
		case TooltipItemType.AttachedMateria3Param:
		case TooltipItemType.AttachedMateria4Param:
			return (flags & TooltipItemGroupFlags.Materia) != 0;
		case TooltipItemType.DurabilityValue:
		case TooltipItemType.SpiritbondCategory:
		case TooltipItemType.SpiritbondValue:
		case TooltipItemType.RepairInfo:
		case TooltipItemType.RepairMaterial:
		case TooltipItemType.QuickRepairCost:
		case TooltipItemType.AttachMateriaInfo:
		case TooltipItemType.GearAbilityInfo:
			return (flags & TooltipItemGroupFlags.CraftingAndRepairs) != 0;
		case TooltipItemType.ShopInfo:
			return (flags & TooltipItemGroupFlags.ShopSellingPrice) != 0;
		default:
			return false;
		}
	}

	private unsafe static void SetItemTooltipGroupFlags(NumberArrayData* numberArrayData, IEnumerable<TooltipItemType> modifiedTargets)
	{
		TooltipItemGroupFlags tooltipItemGroupFlags = ResolveItemTooltipGroupFlags(numberArrayData, modifiedTargets);
		if (tooltipItemGroupFlags != TooltipItemGroupFlags.None)
		{
			((NumberArrayData)numberArrayData).IntArray[5] |= (int)tooltipItemGroupFlags;
		}
	}

	private unsafe static void ClearItemTooltipGroupFlags(NumberArrayData* numberArrayData, IEnumerable<TooltipItemType> emptiedTargets)
	{
		TooltipItemGroupFlags tooltipItemGroupFlags = ResolveItemTooltipGroupFlags(numberArrayData, emptiedTargets);
		if (tooltipItemGroupFlags != TooltipItemGroupFlags.None)
		{
			((NumberArrayData)numberArrayData).IntArray[5] &= (int)(~tooltipItemGroupFlags);
		}
	}

	private unsafe static TooltipItemGroupFlags ResolveItemTooltipGroupFlags(NumberArrayData* numberArrayData, IEnumerable<TooltipItemType> targets)
	{
		bool flag = (GetItemTooltipGroupFlags(numberArrayData) & TooltipItemGroupFlags.HeaderStatsGroup) != 0;
		TooltipItemGroupFlags tooltipItemGroupFlags = TooltipItemGroupFlags.None;
		foreach (TooltipItemType target in targets)
		{
			switch (target)
			{
			case TooltipItemType.MarkerName:
				tooltipItemGroupFlags |= TooltipItemGroupFlags.CrafterName;
				break;
			case TooltipItemType.Description:
				tooltipItemGroupFlags |= TooltipItemGroupFlags.Description;
				break;
			case TooltipItemType.SellInfo:
				tooltipItemGroupFlags |= TooltipItemGroupFlags.Marketable;
				break;
			case TooltipItemType.ClassJobCategory:
			case TooltipItemType.ClassJobLevel:
				tooltipItemGroupFlags = (TooltipItemGroupFlags)((int)tooltipItemGroupFlags | (flag ? 32768 : 8));
				break;
			case TooltipItemType.EffectTitle:
			case TooltipItemType.Effect:
				tooltipItemGroupFlags |= TooltipItemGroupFlags.Bonuses;
				break;
			case TooltipItemType.SpecialTitle:
			case TooltipItemType.SpecialParam0:
			case TooltipItemType.SpecialParam1:
			case TooltipItemType.SpecialParam2:
			case TooltipItemType.SpecialParam3:
			case TooltipItemType.SpecialParam4:
				if (flag)
				{
					tooltipItemGroupFlags |= TooltipItemGroupFlags.Effects;
				}
				break;
			case TooltipItemType.AttachMateriaTitle:
			case TooltipItemType.AttachableGearCategory:
			case TooltipItemType.AttachableGearContent:
			case TooltipItemType.MateriaTitle:
			case TooltipItemType.AttachedMateria0:
			case TooltipItemType.AttachedMateria1:
			case TooltipItemType.AttachedMateria2:
			case TooltipItemType.AttachedMateria3:
			case TooltipItemType.AttachedMateria4:
			case TooltipItemType.AttachedMateria0Param:
			case TooltipItemType.AttachedMateria1Param:
			case TooltipItemType.AttachedMateria2Param:
			case TooltipItemType.AttachedMateria3Param:
			case TooltipItemType.AttachedMateria4Param:
				tooltipItemGroupFlags |= TooltipItemGroupFlags.Materia;
				break;
			case TooltipItemType.DurabilityValue:
			case TooltipItemType.SpiritbondCategory:
			case TooltipItemType.SpiritbondValue:
			case TooltipItemType.RepairInfo:
			case TooltipItemType.RepairMaterial:
			case TooltipItemType.QuickRepairCost:
			case TooltipItemType.AttachMateriaInfo:
			case TooltipItemType.GearAbilityInfo:
				tooltipItemGroupFlags |= TooltipItemGroupFlags.CraftingAndRepairs;
				break;
			case TooltipItemType.ShopInfo:
				tooltipItemGroupFlags |= TooltipItemGroupFlags.ShopSellingPrice;
				break;
			}
		}
		return tooltipItemGroupFlags;
	}

	private unsafe static void RecalculateItemDetailLayout(NumberArrayData* numberArrayData, StringArrayData* stringArrayData)
	{
		AddonItemDetail* itemDetail = (AddonItemDetail*)Addons.ItemDetail;
		if (itemDetail != null && (*Addons.ItemDetail).IsAddonAndNodesReady())
		{
			((AddonItemDetail)itemDetail).GenerateItemTooltip(numberArrayData, stringArrayData);
			((AddonItemDetail)itemDetail).UpdateGroupPositions(numberArrayData, stringArrayData);
		}
	}
}
