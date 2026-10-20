using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Common.Math;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.STD.Helper;
using InteropGenerator.Runtime;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Dalamud;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.OmenService;

public class ContextMenuManager : OmenServiceBase<ContextMenuManager>
{
	private enum ContextMenuType
	{
		AgentContext,
		AgentInventoryContext
	}

	private sealed class LocalMenuItemEntry(ContextMenuItem item) : ContextMenuEntry
	{
		public override string Identifier => "ContextMenuManager";

		public override ContextMenuItem Create(ContextMenuOpenedArgs args)
		{
			return item;
		}
	}

	public class ContextMenuManagerConfig : OmenServiceConfig
	{
		public bool ShowOpenMenuLog;

		public void Save()
		{
			this.Save(DService.Instance().GetOmenService<ContextMenuManager>());
		}
	}

	private Hook<OpenAddon>? OpenAddonHook;

	private Hook<FireCallback>? FireCallbackHook;

	private readonly ConcurrentDictionary<string, uint> addonNameIDs = new ConcurrentDictionary<string, uint>();

	private ContextMenuFrame? currentMenu;

	private readonly Stack<ContextMenuFrame> parentMenus = new Stack<ContextMenuFrame>();

	private bool isNavigating;

	private ContextMenuOpenedArgs? currentArgs;

	private readonly ContextMenuItemResolver itemResolver = new ContextMenuItemResolver();

	private readonly ConcurrentDictionary<ContextMenuEntry, byte> entries = new ConcurrentDictionary<ContextMenuEntry, byte>();

	public ReadOnlySeString? DefaultPrefix { get; set; }

	public ContextMenuManagerConfig Config { get; private set; }

	public void Reg(ContextMenuEntry entry)
	{
		ArgumentNullException.ThrowIfNull(entry, "entry");
		entries.TryAdd(entry, 0);
	}

	public void Unreg(ContextMenuEntry entry)
	{
		entries.TryRemove(entry, out var _);
	}

	protected unsafe override void Init()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected O, but got Unknown
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Expected O, but got Unknown
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Expected O, but got Unknown
		Config = LoadConfig<ContextMenuManagerConfig>() ?? new ContextMenuManagerConfig();
		itemResolver.Init();
		if (OpenAddonHook == null)
		{
			OpenAddonHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromAddress<OpenAddon>((IntPtr)(nint)MemberFunctionPointers.OpenAddon, new OpenAddon(OpenAddonDetour), (HookBackend)0);
		}
		OpenAddonHook.Enable();
		if (FireCallbackHook == null)
		{
			FireCallbackHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromAddress<FireCallback>((IntPtr)(nint)MemberFunctionPointers.FireCallback, new FireCallback(FireCallbackDetour), (HookBackend)0);
		}
		FireCallbackHook.Enable();
		string[] array = new string[3] { "ContextMenu", "AddonContextSub", "AddonContextMenuTitle" };
		foreach (string text in array)
		{
			DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)20, text, new AddonEventDelegate(OnMenuClosed));
			DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, text, new AddonEventDelegate(OnMenuClosed));
		}
	}

	protected override void Uninit()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		itemResolver.Dispose();
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnMenuClosed });
		CloseMenu();
		OpenAddonHook?.Dispose();
		OpenAddonHook = null;
		FireCallbackHook?.Dispose();
		FireCallbackHook = null;
		DefaultPrefix = null;
		ClearMenus();
	}

	private unsafe uint GetAddonNameID(string name)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (addonNameIDs.TryGetValue(name, out var value))
		{
			return value;
		}
		int num = 0;
		UnmanagedArrayEnumerator<Utf8String> enumerator = ((RaptureAtkModule)RaptureAtkModule.Instance()).AddonNames.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Utf8String current = enumerator.Current;
				if (((Utf8String)(ref current)).EqualToString(name))
				{
					addonNameIDs[name] = (uint)num;
					return (uint)num;
				}
				num++;
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
		throw new InvalidOperationException("找不到 " + name + " 的名称 ID");
	}

	private unsafe ContextMenuFrame CreateMenuFrame(uint addonNameID, ReadOnlySpan<AtkValue> nativeValues, AgentInterface* agent, ulong eventKind, ushort ownerAddonID, int depthLayer, List<ContextMenuItem> items, bool isSubmenu)
	{
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Invalid comparison between Unknown and I4
		int uInt = (int)nativeValues[0].UInt;
		int count = uInt + items.Count;
		bool flag = uInt > 0 && nativeValues.Length >= 8 + uInt * 2;
		bool hasDisabled = flag || items.Any((ContextMenuItem item) => !item.IsEnabled);
		ContextMenuFrame frame = new ContextMenuFrame(8 + count * ((!hasDisabled) ? 1 : 2))
		{
			AddonNameID = addonNameID,
			Agent = agent,
			EventKind = eventKind,
			OwnerAddonID = ownerAddonID,
			DepthLayer = depthLayer,
			IsSubmenu = isSubmenu,
			Items = items.ToArray(),
			CallbackIDs = new int[count]
		};
		try
		{
			for (int num = 0; num < 8; num++)
			{
				AtkValue val = nativeValues[num];
				frame.CopyValue(num, &val);
			}
			int num2 = (isSubmenu ? 6 : 2);
			int num3 = (isSubmenu ? 5 : 3);
			uint returnMask = 0u;
			uint submenuMask = 0u;
			int displayIndex = 0;
			for (int num4 = 0; num4 < items.Count; num4++)
			{
				if (items[num4].Priority < 0 && (!isSubmenu || !items[num4].IsReturn))
				{
					AddItem(num4);
				}
			}
			for (int num5 = 0; num5 < uInt; num5++)
			{
				AtkValue val2 = nativeValues[8 + num5];
				frame.CopyValue(8 + displayIndex, &val2);
				frame.CallbackIDs[displayIndex] = -num5 - 1;
				if (hasDisabled)
				{
					int num6 = (flag ? nativeValues[8 + uInt + num5].Int : 0);
					((AtkValue)((byte*)frame.Values + (nint)(8 + count + displayIndex) * (nint)Unsafe.SizeOf<AtkValue>())).SetInt(num6);
				}
				if ((nativeValues[num2].UInt & (uint)(1 << num5)) != 0)
				{
					returnMask |= (uint)(1 << displayIndex);
				}
				if ((nativeValues[num3].UInt & (uint)(1 << num5)) != 0)
				{
					submenuMask |= (uint)(1 << displayIndex);
				}
				displayIndex++;
			}
			for (int num7 = 0; num7 < items.Count; num7++)
			{
				if (items[num7].Priority >= 0 && (!isSubmenu || !items[num7].IsReturn))
				{
					AddItem(num7);
				}
			}
			if (isSubmenu)
			{
				for (int num8 = 0; num8 < items.Count; num8++)
				{
					if (items[num8].IsReturn)
					{
						AddItem(num8);
					}
				}
			}
			((AtkValue)frame.Values).SetUInt((uint)count);
			((AtkValue)((byte*)frame.Values + (nint)num2 * (nint)Unsafe.SizeOf<AtkValue>())).SetUInt(returnMask);
			((AtkValue)((byte*)frame.Values + (nint)num3 * (nint)Unsafe.SizeOf<AtkValue>())).SetUInt(submenuMask);
			if (!isSubmenu && (int)nativeValues[1].Type == 5)
			{
				int val3 = Array.IndexOf(frame.CallbackIDs, (int)(0 - nativeValues[1].UInt - 1));
				frame.SelectedIndex = Math.Max(0, val3);
				((AtkValue)((byte*)frame.Values + Unsafe.SizeOf<AtkValue>())).SetUInt((uint)frame.SelectedIndex);
			}
			return frame;
			unsafe void AddItem(int itemIndex)
			{
				//IL_004c: Unknown result type (might be due to invalid IL or missing references)
				ContextMenuItem contextMenuItem = items[itemIndex];
				frame.CallbackIDs[displayIndex] = itemIndex;
				SetManagedStringValue((AtkValue*)((byte*)frame.Values + (nint)8 * (nint)Unsafe.SizeOf<AtkValue>() + (nint)displayIndex * (nint)Unsafe.SizeOf<AtkValue>()), GetDisplayText(contextMenuItem, isSubmenu));
				if (hasDisabled)
				{
					((AtkValue)((byte*)frame.Values + (nint)(8 + count + displayIndex) * (nint)Unsafe.SizeOf<AtkValue>())).SetInt((!contextMenuItem.IsEnabled) ? 1 : 0);
				}
				if (contextMenuItem.IsReturn)
				{
					returnMask |= (uint)(1 << displayIndex);
				}
				else if (contextMenuItem.Submenu != null)
				{
					submenuMask |= (uint)(1 << displayIndex);
				}
				displayIndex++;
			}
		}
		catch
		{
			frame.Dispose();
			throw;
		}
	}

	private unsafe ushort OpenAddonDetour(RaptureAtkModule* module, uint addonNameID, uint valueCount, AtkValue* values, AtkEventInterface* eventInterface, ulong eventKind, ushort parentAddonID, int depthLayer)
	{
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		Span<Utf8String> span = ((RaptureAtkModule)module).AddonNames.AsSpan();
		if (addonNameID >= span.Length)
		{
			return OpenAddonHook.Original.Invoke(module, addonNameID, valueCount, values, eventInterface, eventKind, parentAddonID, depthLayer);
		}
		ReadOnlySpan<byte> span2 = ((Utf8String)(ref span[(int)addonNameID])).AsSpan();
		if (!span2.SequenceEqual("ContextMenu"u8) && !span2.SequenceEqual("AddonContextSub"u8) && !span2.SequenceEqual("AddonContextMenuTitle"u8))
		{
			return OpenAddonHook.Original.Invoke(module, addonNameID, valueCount, values, eventInterface, eventKind, parentAddonID, depthLayer);
		}
		ContextMenuFrame contextMenuFrame = currentMenu;
		if (contextMenuFrame != null && contextMenuFrame.IsSubmenu)
		{
			bool flag = isNavigating;
			isNavigating = true;
			try
			{
				AtkUnitBase* addonByID = GetAddonByID(contextMenuFrame.AddonID);
				if (addonByID != null)
				{
					((AtkUnitBase)addonByID).Hide(true, false, 0u);
				}
			}
			finally
			{
				isNavigating = flag;
			}
		}
		ClearMenus();
		ContextMenuType? value = ((eventInterface == AgentContext.Instance()) ? new ContextMenuType?(ContextMenuType.AgentContext) : ((eventInterface == AgentInventoryContext.Instance()) ? new ContextMenuType?(ContextMenuType.AgentInventoryContext) : ((ContextMenuType?)null)));
		if (!value.HasValue || !span2.SequenceEqual("ContextMenu"u8) || valueCount < 8 || ((AtkValue)values).UInt > 31 || valueCount < 8 + ((AtkValue)values).UInt)
		{
			return OpenAddonHook.Original.Invoke(module, addonNameID, valueCount, values, eventInterface, eventKind, parentAddonID, depthLayer);
		}
		ContextMenuFrame contextMenuFrame2;
		try
		{
			ContextMenuOpenedArgs args = (currentArgs = BuildArgs((AgentInterface*)eventInterface));
			itemResolver.Resolve(args, (int)((AtkValue)values).UInt);
			if (Config.ShowOpenMenuLog)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(202, 15);
				defaultInterpolatedStringHandler.AppendLiteral("[Context Menu Manager] 打开菜单\n");
				defaultInterpolatedStringHandler.AppendLiteral("类型：");
				defaultInterpolatedStringHandler.AppendFormatted(value);
				defaultInterpolatedStringHandler.AppendLiteral("\n");
				defaultInterpolatedStringHandler.AppendLiteral("Addon：");
				defaultInterpolatedStringHandler.AppendFormatted(currentArgs.AddonName ?? "[空]");
				defaultInterpolatedStringHandler.AppendLiteral(" (");
				defaultInterpolatedStringHandler.AppendFormatted(currentArgs.OwnerAddonID);
				defaultInterpolatedStringHandler.AppendLiteral(")\n");
				defaultInterpolatedStringHandler.AppendLiteral("目标名称：");
				defaultInterpolatedStringHandler.AppendFormatted(currentArgs.TargetName ?? "[空]");
				defaultInterpolatedStringHandler.AppendLiteral("\n");
				defaultInterpolatedStringHandler.AppendLiteral("目标 Object ID：0x");
				defaultInterpolatedStringHandler.AppendFormatted(currentArgs.TargetObjectID, "X");
				defaultInterpolatedStringHandler.AppendLiteral("\n");
				defaultInterpolatedStringHandler.AppendLiteral("目标 Content ID：");
				defaultInterpolatedStringHandler.AppendFormatted(currentArgs.TargetContentID);
				defaultInterpolatedStringHandler.AppendLiteral("\n");
				defaultInterpolatedStringHandler.AppendLiteral("目标 Home World ID：");
				defaultInterpolatedStringHandler.AppendFormatted(currentArgs.TargetHomeWorldID);
				defaultInterpolatedStringHandler.AppendLiteral("\n");
				defaultInterpolatedStringHandler.AppendLiteral("目标角色：0x");
				defaultInterpolatedStringHandler.AppendFormatted((nint)currentArgs.TargetCharacter, "X");
				defaultInterpolatedStringHandler.AppendLiteral("（玩家: ");
				defaultInterpolatedStringHandler.AppendFormatted(currentArgs.IsTargetPlayer);
				defaultInterpolatedStringHandler.AppendLiteral("）\n");
				defaultInterpolatedStringHandler.AppendLiteral("物品 ID：");
				defaultInterpolatedStringHandler.AppendFormatted(currentArgs.TargetItemID);
				defaultInterpolatedStringHandler.AppendLiteral("\n");
				defaultInterpolatedStringHandler.AppendLiteral("幻化物品 ID：");
				defaultInterpolatedStringHandler.AppendFormatted(currentArgs.TargetGlamourID);
				defaultInterpolatedStringHandler.AppendLiteral("\n");
				defaultInterpolatedStringHandler.AppendLiteral("Inventory Type：");
				InventoryType? targetInventoryType = currentArgs.TargetInventoryType;
				defaultInterpolatedStringHandler.AppendFormatted((targetInventoryType.HasValue ? ((object)targetInventoryType.GetValueOrDefault()/*cast due to constrained. prefix*/).ToString() : null) ?? "[空]");
				defaultInterpolatedStringHandler.AppendLiteral("\n");
				defaultInterpolatedStringHandler.AppendLiteral("Inventory Slot：");
				defaultInterpolatedStringHandler.AppendFormatted(currentArgs.TargetSlot?.ToString() ?? "[空]");
				defaultInterpolatedStringHandler.AppendLiteral("\n");
				defaultInterpolatedStringHandler.AppendLiteral("Agent Context：0x");
				defaultInterpolatedStringHandler.AppendFormatted((nint)currentArgs.DefaultAgentContext, "X");
				defaultInterpolatedStringHandler.AppendLiteral("\n");
				defaultInterpolatedStringHandler.AppendLiteral("Inventory Agent Context：0x");
				defaultInterpolatedStringHandler.AppendFormatted((nint)currentArgs.InventoryAgentContext, "X");
				DLog.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			List<ContextMenuItem> items = FixupMenuList(CreateItems(args, OrderedSnapshot()), (int)((AtkValue)values).UInt);
			contextMenuFrame2 = CreateMenuFrame(addonNameID, new ReadOnlySpan<AtkValue>(values, (int)valueCount), (AgentInterface*)eventInterface, eventKind, parentAddonID, depthLayer, items, isSubmenu: false);
		}
		catch (Exception ex)
		{
			ClearMenus();
			DLog.Error("[ContextMenuManager] 构建 ContextMenu 时发生错误", ex);
			return OpenAddonHook.Original.Invoke(module, addonNameID, valueCount, values, eventInterface, eventKind, parentAddonID, depthLayer);
		}
		currentMenu = contextMenuFrame2;
		contextMenuFrame2.Retain();
		try
		{
			contextMenuFrame2.AddonID = OpenAddonHook.Original.Invoke(module, addonNameID, (uint)contextMenuFrame2.ValueCount, contextMenuFrame2.Values, eventInterface, eventKind, parentAddonID, depthLayer);
			if (contextMenuFrame2.AddonID == 0 && currentMenu == contextMenuFrame2)
			{
				ClearMenus();
			}
			return contextMenuFrame2.AddonID;
		}
		catch
		{
			if (currentMenu == contextMenuFrame2)
			{
				ClearMenus();
			}
			throw;
		}
		finally
		{
			contextMenuFrame2.Dispose();
		}
	}

	private List<ContextMenuItem> FixupMenuList(List<ContextMenuItem> items, int nativeMenuSize, int reservedItems = 0)
	{
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		int num = 31 - nativeMenuSize - reservedItems;
		if (num <= 0)
		{
			return new List<ContextMenuItem>();
		}
		if (items.Count > num)
		{
			ContextMenuItem[] array = items.OrderBy((ContextMenuItem i) => i.Priority).ToArray();
			ContextMenuItem[] subArray = array[..(num - 1)];
			ContextMenuItem[] subArray2 = array[(num - 1)..];
			string addonText = LuminaWrapper.GetAddonText(1763u);
			LocalMenuItemEntry[] array2 = subArray2.Select((ContextMenuItem item) => new LocalMenuItemEntry(item)).ToArray();
			ContextMenuItem[] array3 = subArray;
			int num2 = 1 + array3.Length;
			List<ContextMenuItem> list = new List<ContextMenuItem>(num2);
			CollectionsMarshal.SetCount(list, num2);
			Span<ContextMenuItem> span = CollectionsMarshal.AsSpan(list);
			int num3 = 0;
			ReadOnlySpan<ContextMenuItem> readOnlySpan = new ReadOnlySpan<ContextMenuItem>(array3);
			readOnlySpan.CopyTo(span.Slice(num3, readOnlySpan.Length));
			num3 += readOnlySpan.Length;
			span[num3] = new ContextMenuItem
			{
				Name = ReadOnlySeString.op_Implicit(addonText),
				Prefix = DefaultPrefix,
				Priority = int.MaxValue,
				Submenu = new ContextMenuSubmenu
				{
					Title = ReadOnlySeString.op_Implicit(addonText),
					Entries = array2
				}
			};
			return list;
		}
		return items;
	}

	private unsafe void OpenSubmenu(ContextMenuFrame parent, ContextMenuSubmenu submenu)
	{
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		if (currentMenu != parent)
		{
			return;
		}
		AtkUnitBase* addonByID = GetAddonByID(parent.AddonID);
		if (addonByID == null || !((AtkUnitBase)addonByID).IsVisible)
		{
			return;
		}
		List<ContextMenuItem> list = new List<ContextMenuItem>();
		ContextMenuItem contextMenuItem = null;
		foreach (ContextMenuEntry entry in submenu.Entries)
		{
			IReadOnlyList<ContextMenuItem> readOnlyList = entry.CreateMultiple(currentArgs);
			if (readOnlyList != null)
			{
				foreach (ContextMenuItem item in readOnlyList)
				{
					if (!(entry is LocalMenuItemEntry))
					{
						item.Entry = entry;
					}
					if (item.IsReturn)
					{
						if (contextMenuItem == null)
						{
							contextMenuItem = item;
						}
					}
					else
					{
						list.Add(item);
					}
				}
			}
			ContextMenuItem contextMenuItem2 = entry.Create(currentArgs);
			if (contextMenuItem2 == null)
			{
				continue;
			}
			if (!(entry is LocalMenuItemEntry))
			{
				contextMenuItem2.Entry = entry;
			}
			if (contextMenuItem2.IsReturn)
			{
				if (contextMenuItem == null)
				{
					contextMenuItem = contextMenuItem2;
				}
			}
			else
			{
				list.Add(contextMenuItem2);
			}
		}
		if (currentMenu != parent)
		{
			return;
		}
		addonByID = GetAddonByID(parent.AddonID);
		if (addonByID == null || !((AtkUnitBase)addonByID).IsVisible)
		{
			return;
		}
		list = FixupMenuList(list, 0, 1);
		list.Add(contextMenuItem ?? new ContextMenuItem
		{
			Name = ReadOnlySeString.op_Implicit(LuminaWrapper.GetAddonText(2440u)),
			IsReturn = true
		});
		short num = default(short);
		short num2 = default(short);
		((AtkUnitBase)addonByID).GetPosition(&num, &num2);
		parent.X = num;
		parent.Y = num2;
		AtkValue* ptr = (AtkValue*)stackalloc AtkValue[8];
		new Span<AtkValue>(ptr, 8).Clear();
		ContextMenuFrame contextMenuFrame;
		try
		{
			((AtkValue)ptr).SetUInt(0u);
			SetManagedStringValue((AtkValue*)((byte*)ptr + Unsafe.SizeOf<AtkValue>()), submenu.Title);
			((AtkValue)((byte*)ptr + (nint)2 * (nint)Unsafe.SizeOf<AtkValue>())).SetInt((int)num);
			((AtkValue)((byte*)ptr + (nint)3 * (nint)Unsafe.SizeOf<AtkValue>())).SetInt((int)num2);
			((AtkValue)((byte*)ptr + (nint)4 * (nint)Unsafe.SizeOf<AtkValue>())).SetBool(false);
			((AtkValue)((byte*)ptr + (nint)5 * (nint)Unsafe.SizeOf<AtkValue>())).SetUInt(0u);
			((AtkValue)((byte*)ptr + (nint)6 * (nint)Unsafe.SizeOf<AtkValue>())).SetUInt(0u);
			((AtkValue)((byte*)ptr + (nint)7 * (nint)Unsafe.SizeOf<AtkValue>())).SetUInt(1u);
			contextMenuFrame = CreateMenuFrame(GetAddonNameID("AddonContextSub"), new ReadOnlySpan<AtkValue>(ptr, 8), parent.Agent, parent.EventKind, parent.OwnerAddonID, parent.DepthLayer, list, isSubmenu: true);
		}
		finally
		{
			for (int i = 0; i < 8; i++)
			{
				((AtkValue)((byte*)ptr + (nint)i * (nint)Unsafe.SizeOf<AtkValue>())).Dtor();
			}
		}
		contextMenuFrame.X = num;
		contextMenuFrame.Y = num2;
		try
		{
			if (!ShowMenu(contextMenuFrame, addonByID))
			{
				contextMenuFrame.Dispose();
				return;
			}
		}
		catch
		{
			contextMenuFrame.Dispose();
			throw;
		}
		parentMenus.Push(parent);
		currentMenu = contextMenuFrame;
	}

	private unsafe bool ShowMenu(ContextMenuFrame frame, AtkUnitBase* previousAddon)
	{
		ContextMenuFrame contextMenuFrame = currentMenu;
		bool flag = isNavigating;
		isNavigating = true;
		frame.Retain();
		try
		{
			if (frame.IsSubmenu)
			{
				((AtkValue)((byte*)frame.Values + (nint)2 * (nint)Unsafe.SizeOf<AtkValue>())).SetInt((int)frame.X);
				((AtkValue)((byte*)frame.Values + (nint)3 * (nint)Unsafe.SizeOf<AtkValue>())).SetInt((int)frame.Y);
			}
			else
			{
				((AtkValue)((byte*)frame.Values + Unsafe.SizeOf<AtkValue>())).SetUInt((uint)frame.SelectedIndex);
				((AtkValue)((byte*)frame.Values + (nint)4 * (nint)Unsafe.SizeOf<AtkValue>())).SetUInt(((AtkValue)((byte*)frame.Values + (nint)4 * (nint)Unsafe.SizeOf<AtkValue>())).UInt | 1);
			}
			ushort num = OpenAddonHook.Original.Invoke(RaptureAtkModule.Instance(), frame.AddonNameID, (uint)frame.ValueCount, frame.Values, &((AgentInterface)frame.Agent).AtkEventInterface, frame.EventKind, frame.OwnerAddonID, frame.DepthLayer);
			if (num == 0 || currentMenu != contextMenuFrame)
			{
				return false;
			}
			frame.AddonID = num;
			((AgentInterface)frame.Agent).AddonId = num;
			if (previousAddon != null && ((AtkUnitBase)previousAddon).Id != num)
			{
				((AtkUnitBase)previousAddon).Hide(true, false, 0u);
			}
			AtkUnitBase* addonByID = GetAddonByID(num);
			if (addonByID != null)
			{
				var (num2, num3) = GetScreenClampedPosition(addonByID, frame.X, frame.Y);
				((AtkUnitBase)addonByID).SetPosition(num2, num3);
			}
			return true;
		}
		finally
		{
			frame.Dispose();
			isNavigating = flag;
		}
	}

	private unsafe static (short X, short Y) GetScreenClampedPosition(AtkUnitBase* addon, short x, short y)
	{
		AtkStage* ptr = AtkStage.Instance();
		float num = ((Size)(&((AtkStage)ptr).ScreenSize)).Width;
		float num2 = ((Size)(&((AtkStage)ptr).ScreenSize)).Height;
		if (((AtkStage)ptr).IsScreenSizeScaled)
		{
			num *= ((AtkStage)ptr).ScreenSizeScale;
			num2 *= ((AtkStage)ptr).ScreenSizeScale;
		}
		float max = Math.Max(0f, num - ((AtkUnitBase)addon).GetScaledWidth(true));
		float max2 = Math.Max(0f, num2 - ((AtkUnitBase)addon).GetScaledHeight(true));
		return (X: (short)Math.Clamp(x, 0f, max), Y: (short)Math.Clamp(y, 0f, max2));
	}

	private unsafe bool ReturnToParent(AtkUnitBase* addon)
	{
		if (!parentMenus.TryPeek(out ContextMenuFrame result))
		{
			return CloseMenu();
		}
		if (ShowMenu(result, addon))
		{
			ContextMenuFrame? contextMenuFrame = currentMenu;
			currentMenu = parentMenus.Pop();
			contextMenuFrame?.Dispose();
		}
		return false;
	}

	private unsafe bool FireCallbackDetour(AtkUnitBase* addon, uint valueCount, AtkValue* values, bool close)
	{
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Invalid comparison between Unknown and I4
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Invalid comparison between Unknown and I4
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Invalid comparison between Unknown and I4
		bool flag = isNavigating;
		if (flag)
		{
			bool flag2;
			switch (((AtkUnitBase)addon).NameString)
			{
			case "ContextMenu":
			case "AddonContextSub":
			case "AddonContextMenuTitle":
				flag2 = true;
				break;
			default:
				flag2 = false;
				break;
			}
			flag = flag2;
		}
		if (flag)
		{
			return false;
		}
		ContextMenuFrame frame = currentMenu;
		if (frame == null || frame.AddonID != ((AtkUnitBase)addon).Id)
		{
			if (parentMenus.All((ContextMenuFrame parent) => parent.AddonID != ((AtkUnitBase)addon).Id))
			{
				return FireCallbackHook.Original.Invoke(addon, valueCount, values, close);
			}
			return false;
		}
		if (valueCount < 2 || (int)((AtkValue)values).Type != 3 || ((AtkValue)values).Int != 0 || (int)((AtkValue)((byte*)values + Unsafe.SizeOf<AtkValue>())).Type != 3)
		{
			return FireCallbackHook.Original.Invoke(addon, valueCount, values, close);
		}
		int num = ((AtkValue)((byte*)values + Unsafe.SizeOf<AtkValue>())).Int;
		if (num < 0)
		{
			if (!frame.IsSubmenu)
			{
				return FireCallbackHook.Original.Invoke(addon, valueCount, values, close);
			}
			if (valueCount <= 2 || (int)((AtkValue)((byte*)values + (nint)2 * (nint)Unsafe.SizeOf<AtkValue>())).Type != 5 || ((AtkValue)((byte*)values + (nint)2 * (nint)Unsafe.SizeOf<AtkValue>())).UInt == 0)
			{
				return CloseMenu();
			}
			try
			{
				flag = ReturnToParent(addon);
			}
			catch (Exception ex)
			{
				DLog.Error("[ContextMenuManager] 返回 ContextMenu 上级菜单时发生错误", ex);
				flag = CloseMenu();
			}
		}
		else
		{
			if (num >= frame.CallbackIDs.Length)
			{
				return FireCallbackHook.Original.Invoke(addon, valueCount, values, close);
			}
			int num2 = frame.CallbackIDs[num];
			if (num2 >= 0)
			{
				ContextMenuItem contextMenuItem = frame.Items[num2];
				if (!contextMenuItem.IsEnabled)
				{
					return false;
				}
				frame.SelectedIndex = num;
				try
				{
					if (contextMenuItem.IsReturn)
					{
						flag = ReturnToParent(addon);
						goto IL_02d5;
					}
					ContextMenuSubmenu submenu = contextMenuItem.Submenu;
					if (submenu != null)
					{
						OpenSubmenu(frame, submenu);
						flag = false;
						goto IL_02d5;
					}
					contextMenuItem.OnClicked?.Invoke(new ContextMenuItemClickedArgs(currentArgs.Clone(), delegate(ContextMenuSubmenu x)
					{
						OpenSubmenu(frame, x);
					}));
				}
				catch (Exception ex2)
				{
					DLog.Error("[ContextMenuManager] 处理 ContextMenu 点击时发生错误", ex2);
				}
				if (currentMenu == frame)
				{
					return CloseMenu();
				}
				return false;
			}
			((AtkValue)((byte*)values + Unsafe.SizeOf<AtkValue>())).Int = -num2 - 1;
			try
			{
				flag = FireCallbackHook.Original.Invoke(addon, valueCount, values, close);
			}
			finally
			{
				((AtkValue)((byte*)values + Unsafe.SizeOf<AtkValue>())).Int = num;
			}
		}
		goto IL_02d5;
		IL_02d5:
		return flag;
	}

	private unsafe bool CloseMenu()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		ContextMenuFrame contextMenuFrame = currentMenu;
		if (contextMenuFrame == null)
		{
			return false;
		}
		bool flag = isNavigating;
		isNavigating = true;
		try
		{
			AtkUnitBase* addonByID = GetAddonByID(contextMenuFrame.AddonID);
			if (addonByID == null)
			{
				return false;
			}
			AtkValue val = new AtkValue();
			val.Type = (AtkValueType)3;
			val.Int = -2;
			AtkValue val2 = val;
			return FireCallbackHook.Original.Invoke(addonByID, 1u, &val2, true);
		}
		finally
		{
			if (currentMenu == contextMenuFrame)
			{
				ClearMenus();
			}
			isNavigating = flag;
		}
	}

	private void OnMenuClosed(AddonEvent type, AddonArgs args)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (!isNavigating)
		{
			ContextMenuFrame contextMenuFrame = currentMenu;
			if (contextMenuFrame != null && ((AtkUnitBase)(nint)args.Addon.Address).Id == contextMenuFrame.AddonID)
			{
				ClearMenus();
			}
		}
	}

	private void ClearMenus()
	{
		currentMenu?.Dispose();
		currentMenu = null;
		ContextMenuFrame result;
		while (parentMenus.TryPop(out result))
		{
			result.Dispose();
		}
		currentArgs = null;
	}

	private unsafe ReadOnlySeString GetDisplayText(ContextMenuItem item, bool isSubmenu)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		if (item != null && item.IsReturn && item.Entry == null && !item.Prefix.HasValue)
		{
			return item.Name;
		}
		ReadOnlySeString? val = item.Prefix ?? item.Entry?.Prefix;
		if (!val.HasValue)
		{
			ContextMenuEntry? entry = item.Entry;
			if ((entry == null || !entry.OmitPrefix) && !isSubmenu)
			{
				val = DefaultPrefix;
			}
		}
		if (!val.HasValue)
		{
			return item.Name;
		}
		RentedSeStringBuilder val2 = default(RentedSeStringBuilder);
		((RentedSeStringBuilder)(ref val2))._002Ector();
		try
		{
			return ((RentedSeStringBuilder)(ref val2)).Builder.Append(val.Value).Append(' ').Append(item.Name)
				.ToReadOnlySeString();
		}
		finally
		{
			((IDisposable)(*(RentedSeStringBuilder*)(&val2))/*cast due to constrained. prefix*/).Dispose();
		}
	}

	private unsafe static void SetManagedStringValue(AtkValue* value, ReadOnlySeString text)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		byte[] array = text.Data.ToArray();
		byte[] array2 = new byte[array.Length + 1];
		array.CopyTo(array2, 0);
		fixed (byte* ptr = array2)
		{
			((AtkValue)value).SetManagedString(new CStringPointer(ptr));
		}
	}

	private List<ContextMenuEntry> OrderedSnapshot()
	{
		return entries.Keys.OrderBy<ContextMenuEntry, string>((ContextMenuEntry e) => e.Identifier, StringComparer.Ordinal).ThenBy((ContextMenuEntry e) => e.Priority.GetValueOrDefault()).ToList();
	}

	private static List<ContextMenuItem> CreateItems(ContextMenuOpenedArgs args, IEnumerable<ContextMenuEntry> entries)
	{
		List<ContextMenuItem> list = new List<ContextMenuItem>();
		foreach (ContextMenuEntry entry in entries)
		{
			IReadOnlyList<ContextMenuItem> readOnlyList = entry.CreateMultiple(args);
			if (readOnlyList != null)
			{
				foreach (ContextMenuItem item in readOnlyList)
				{
					item.Entry = entry;
					list.Add(item);
				}
			}
			ContextMenuItem contextMenuItem = entry.Create(args);
			if (contextMenuItem != null)
			{
				contextMenuItem.Entry = entry;
				list.Add(contextMenuItem);
			}
		}
		return list;
	}

	private unsafe static ContextMenuOpenedArgs BuildArgs(AgentInterface* agent)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		ContextMenuOpenedArgs contextMenuOpenedArgs = new ContextMenuOpenedArgs(agent);
		if (agent == AgentInventoryContext.Instance())
		{
			AgentInventoryContext* ptr = (contextMenuOpenedArgs.InventoryAgentContext = AgentInventoryContext.Instance());
			contextMenuOpenedArgs.OwnerAddonID = ((AgentInventoryContext)ptr).OwnerAddonId;
			contextMenuOpenedArgs.Addon = GetAddonByID(((AgentInventoryContext)ptr).OwnerAddonId);
			contextMenuOpenedArgs.TargetInventoryType = ((AgentInventoryContext)ptr).TargetInventoryId;
			contextMenuOpenedArgs.TargetSlot = ((AgentInventoryContext)ptr).TargetInventorySlotId;
			if (((AgentInventoryContext)ptr).TargetInventorySlot != null)
			{
				contextMenuOpenedArgs.TargetInventoryItem = *((AgentInventoryContext)ptr).TargetInventorySlot;
				contextMenuOpenedArgs.TargetItemID = ((InventoryItem)((AgentInventoryContext)ptr).TargetInventorySlot).GetBaseItemId();
				contextMenuOpenedArgs.TargetGlamourID = ((InventoryItem)((AgentInventoryContext)ptr).TargetInventorySlot).GetGlamourId();
			}
		}
		else if (agent == AgentContext.Instance())
		{
			AgentContext* ptr3 = (contextMenuOpenedArgs.DefaultAgentContext = AgentContext.Instance());
			contextMenuOpenedArgs.OwnerAddonID = ((AgentContext)ptr3).OwnerAddon;
			contextMenuOpenedArgs.Addon = GetAddonByID(((AgentContext)ptr3).OwnerAddon);
			contextMenuOpenedArgs.TargetObjectID = GameObjectId.op_Implicit(((AgentContext)ptr3).TargetObjectId);
			contextMenuOpenedArgs.TargetContentID = ((AgentContext)ptr3).TargetContentId;
			contextMenuOpenedArgs.TargetHomeWorldID = ((AgentContext)ptr3).TargetHomeWorldId;
			contextMenuOpenedArgs.TargetName = ((object)(*(Utf8String*)(&((AgentContext)ptr3).TargetName))/*cast due to constrained. prefix*/).ToString();
			contextMenuOpenedArgs.TargetCharacter = ((AgentContext)ptr3).CurrentContextMenuTarget;
		}
		contextMenuOpenedArgs.AddonName = ((contextMenuOpenedArgs.Addon == null) ? null : ((AtkUnitBase)contextMenuOpenedArgs.Addon).NameString);
		return contextMenuOpenedArgs;
	}

	private unsafe static AtkUnitBase* GetAddonByID(uint addonID)
	{
		if ((addonID != 0 && addonID != 65535) || 1 == 0)
		{
			return ((RaptureAtkUnitManager)RaptureAtkUnitManager.Instance()).GetAddonById((ushort)addonID);
		}
		return null;
	}

	public unsafe void Open(ContextMenuOpenedArgs args, IReadOnlyList<ContextMenuEntry>? entries = null)
	{
		ArgumentNullException.ThrowIfNull(args, "args");
		if (args.Agent == null)
		{
			throw new InvalidOperationException("主动打开 ContextMenu 需要指定 Agent");
		}
		CloseMenu();
		AtkValue* ptr = (AtkValue*)stackalloc AtkValue[8];
		new Span<AtkValue>(ptr, 8).Clear();
		ContextMenuFrame contextMenuFrame;
		try
		{
			currentArgs = args;
			Vector2 obj = args.Position ?? ImGui.GetMousePos();
			short x = (short)obj.X;
			short y = (short)obj.Y;
			AgentContext* defaultAgentContext = args.DefaultAgentContext;
			int depthLayer = ((defaultAgentContext != null) ? ((AgentContext)defaultAgentContext).ContextMenuDepthLayer : 0);
			List<ContextMenuEntry> list = new List<ContextMenuEntry>();
			if (entries != null)
			{
				list.AddRange(entries);
			}
			list.AddRange(OrderedSnapshot());
			List<ContextMenuItem> items = FixupMenuList(CreateItems(args, list), 0);
			contextMenuFrame = CreateMenuFrame(GetAddonNameID("ContextMenu"), new ReadOnlySpan<AtkValue>(ptr, 8), args.Agent, 0uL, (ushort)args.OwnerAddonID, depthLayer, items, isSubmenu: false);
			contextMenuFrame.X = x;
			contextMenuFrame.Y = y;
		}
		catch
		{
			ClearMenus();
			throw;
		}
		finally
		{
			for (int i = 0; i < 8; i++)
			{
				((AtkValue)((byte*)ptr + (nint)i * (nint)Unsafe.SizeOf<AtkValue>())).Dtor();
			}
		}
		try
		{
			if (!ShowMenu(contextMenuFrame, null))
			{
				contextMenuFrame.Dispose();
				ClearMenus();
				return;
			}
		}
		catch
		{
			contextMenuFrame.Dispose();
			ClearMenus();
			throw;
		}
		currentMenu = contextMenuFrame;
	}

	public unsafe void OpenItem(uint itemID, uint? parentID = null)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		if (!LuminaGetter.TryGetRow<Item>(itemID, out Item item))
		{
			return;
		}
		uint parentAddonID = parentID.GetValueOrDefault();
		ContextMenuOpenedArgs args = new ContextMenuOpenedArgs
		{
			InventoryAgentContext = AgentInventoryContext.Instance(),
			TargetItemID = itemID
		};
		List<ContextMenuItem> menus = new List<ContextMenuItem>();
		if (((Item)(ref item)).EquipSlotCategory.RowId != 0)
		{
			menus.AddRange(new global::_003C_003Ez__ReadOnlyArray<ContextMenuItem>(new ContextMenuItem[2]
			{
				new ContextMenuItem
				{
					Name = ReadOnlySeString.op_Implicit(LuminaWrapper.GetAddonText(2150u)),
					OnClicked = delegate
					{
						((AgentItemComp)AgentItemComp.Instance()).CompareItem((ushort)parentAddonID, itemID, (byte)0, (byte)0);
					}
				},
				new ContextMenuItem
				{
					Name = ReadOnlySeString.op_Implicit(LuminaWrapper.GetAddonText(2426u)),
					OnClicked = delegate
					{
						AgentTryon.TryOn(parentAddonID, itemID, (byte)0, (byte)0, 0u, false);
					}
				}
			}));
		}
		menus.AddRange(new global::_003C_003Ez__ReadOnlyArray<ContextMenuItem>(new ContextMenuItem[3]
		{
			new ContextMenuItem
			{
				Name = ReadOnlySeString.op_Implicit(LuminaWrapper.GetAddonText(4379u)),
				OnClicked = delegate
				{
					((ItemFinderModule)ItemFinderModule.Instance()).SearchForItem(itemID, true);
				}
			},
			new ContextMenuItem
			{
				Name = ReadOnlySeString.op_Implicit(LuminaWrapper.GetAddonText(4697u)),
				OnClicked = delegate
				{
					((AgentChatLog)AgentChatLog.Instance()).LinkItem(itemID);
				}
			},
			new ContextMenuItem
			{
				Name = ReadOnlySeString.op_Implicit(LuminaWrapper.GetAddonText(13439u)),
				OnClicked = delegate
				{
					((AgentRecipeProductList)AgentRecipeProductList.Instance()).SearchForRecipesUsingItem(itemID);
				}
			}
		}));
		Open(args, new _003C_003Ez__ReadOnlySingleElementList<ContextMenuEntry>(new ContextMenuEntryInfo("ContextMenuManager", (ContextMenuOpenedArgs _) => menus, null, omitPrefix: true)));
	}
}
