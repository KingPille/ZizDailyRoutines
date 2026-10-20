using System;
using System.Runtime.CompilerServices;
using Dalamud.Game.Addon.Events;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Lumina;

namespace OmenTools.KamiToolKit.Nodes;

public class ItemIconNode : IconNode
{
	[CompilerGenerated]
	private uint _003CItemID_003Ek__BackingField;

	public uint ItemID
	{
		[CompilerGenerated]
		get
		{
			return _003CItemID_003Ek__BackingField;
		}
		set
		{
			if (_003CItemID_003Ek__BackingField != value && LuminaGetter.TryGetRow<Item>(value, out Item item))
			{
				_003CItemID_003Ek__BackingField = value;
				base.IconId = ((Item)(ref item)).Icon;
				ItemTooltip = ((Item)(ref item)).RowId;
			}
		}
	}

	public ReceiveEvent? OnClick { get; set; }

	public unsafe ItemIconNode()
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		AddNodeFlags((NodeFlags)256);
		AddEvent((AtkEventType)6, (Action)delegate
		{
			DalamudServiceExtension.Instance<IAddonEventManager>().SetCursor((AddonCursorType)11);
		});
		AddEvent((AtkEventType)7, (Action)delegate
		{
			DalamudServiceExtension.Instance<IAddonEventManager>().ResetCursor();
		});
		AddEvent((AtkEventType)9, new ReceiveEvent(MouseClickHandler));
	}

	private unsafe void MouseClickHandler(AtkEventListener* thisPtr, AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		ReceiveEvent? onClick = OnClick;
		if (onClick != null)
		{
			onClick.Invoke(thisPtr, eventType, eventParam, atkEvent, atkEventData);
		}
	}
}
