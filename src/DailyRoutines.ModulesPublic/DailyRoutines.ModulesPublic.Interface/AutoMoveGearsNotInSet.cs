using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.Command;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using KamiToolKit.Nodes.Simplified;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic.Interface;

public class AutoMoveGearsNotInSet : ModuleBase
{
	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static HandlerDelegate _003C0_003E__OnCommand;
	}

	private TextButtonNode? button;

	private const string COMMAND = "retrievegears";

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoMoveGearsNotInSetTitle"),
		Description = LanguageManager.Get("AutoMoveGearsNotInSetDescription"),
		Category = ModuleCategory.Interface
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected override void Init()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected O, but got Unknown
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected O, but got Unknown
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Expected O, but got Unknown
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		CommandManager commandManager = OmenServiceBase<CommandManager>.Instance();
		object obj = _003C_003EO._003C0_003E__OnCommand;
		if (obj == null)
		{
			HandlerDelegate val = OnCommand;
			_003C_003EO._003C0_003E__OnCommand = val;
			obj = (object)val;
		}
		commandManager.AddSubCommand("retrievegears", new CommandInfo((HandlerDelegate)obj)
		{
			HelpMessage = LanguageManager.Get("AutoMoveGearsNotInSet-CommandHelp")
		});
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)5, "ArmouryBoard", new AddonEventDelegate(OnAddon));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, "ArmouryBoard", new AddonEventDelegate(OnAddon));
	}

	protected override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddon });
		button?.Dispose();
		button = null;
		OmenServiceBase<CommandManager>.Instance().RemoveSubCommand("retrievegears");
	}

	protected override void ConfigUI()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(1, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("Command"));
		((ImU8String)(ref val)).AppendLiteral(":");
		ImGui.TextColored(ref vector, val);
		ImGui.SameLine();
		ImU8String val2 = default(ImU8String);
		((ImU8String)(ref val2))._002Ector(8, 2);
		((ImU8String)(ref val2)).AppendLiteral("/pdr ");
		((ImU8String)(ref val2)).AppendFormatted<string>("retrievegears");
		((ImU8String)(ref val2)).AppendLiteral(" → ");
		((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get("AutoMoveGearsNotInSet-CommandHelp"));
		ImGui.TextUnformatted(val2);
		ImGui.Spacing();
		ImGui.AlignTextToFramePadding();
		vector = KnownColor.Chocolate.ToVector4();
		ImU8String val3 = default(ImU8String);
		((ImU8String)(ref val3))._002Ector(1, 1);
		((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get("AutoMoveGearsNotInSet-MannualRetrieve"));
		((ImU8String)(ref val3)).AppendLiteral(":");
		ImGui.TextColored(ref vector, val3);
		ImGui.SameLine();
		if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("Confirm")), default(Vector2)))
		{
			EnqueueRetrieve();
		}
	}

	private unsafe void OnAddon(AddonEvent type, AddonArgs args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		if ((int)type != 5)
		{
			if ((int)type == 6)
			{
				button = null;
			}
		}
		else if (Addons.ArmouryBoard != null && button == null)
		{
			button = new TextButtonNode
			{
				Size = new Vector2(48f),
				Position = new Vector2(12f, 500f),
				IsVisible = true,
				String = ReadOnlySeString.op_Implicit(new SeStringBuilder().AddIcon((BitmapFontIcon)72).Build().Encode()),
				TextTooltip = ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoMoveGearsNotInSet-Button")),
				OnClick = delegate
				{
					OmenServiceBase<ChatManager>.Instance().SendMessage("/pdr retrievegears");
				},
				IsEnabled = true
			};
			SimpleNineGridNode obj = (SimpleNineGridNode)button.BackgroundNode;
			obj.TexturePath = "ui/uld/partyfinder_hr1.tex";
			obj.TextureCoordinates = new Vector2(38f);
			obj.TextureSize = new Vector2(32f, 34f);
			obj.LeftOffset = 0f;
			obj.RightOffset = 0f;
			button.AttachNode(((AtkUnitBase)Addons.ArmouryBoard).RootNode);
		}
	}

	private static void OnCommand(string command, string args)
	{
		EnqueueRetrieve();
	}

	private unsafe static void EnqueueRetrieve()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		RaptureGearsetModule* intPtr = RaptureGearsetModule.Instance();
		InventoryManager* ptr = InventoryManager.Instance();
		HashSet<uint> hashSet = new HashSet<uint>();
		Span<GearsetEntry> entries = ((RaptureGearsetModule)intPtr).Entries;
		for (int i = 0; i < entries.Length; i++)
		{
			GearsetEntry val = entries[i];
			Span<GearsetItem> items = ((GearsetEntry)(ref val)).Items;
			for (int j = 0; j < items.Length; j++)
			{
				GearsetItem val2 = items[j];
				if (val2.ItemId != 0)
				{
					hashSet.Add(val2.ItemId);
				}
			}
		}
		int num = 0;
		foreach (InventoryType item in Inventories.Armory)
		{
			InventoryContainer* inventoryContainer = ((InventoryManager)ptr).GetInventoryContainer(item);
			for (int k = 0; k < ((InventoryContainer)inventoryContainer).Size; k++)
			{
				InventoryItem* inventorySlot = ((InventoryContainer)inventoryContainer).GetInventorySlot(k);
				if (inventorySlot == null || ((InventoryItem)inventorySlot).ItemId == 0)
				{
					continue;
				}
				uint num2 = ((InventoryItem)inventorySlot).ItemId;
				if (((Enum)((InventoryItem)inventorySlot).Flags).HasFlag((Enum)(object)(ItemFlags)1))
				{
					num2 += 1000000;
				}
				if (!hashSet.Contains(num2))
				{
					if (!((IEnumerable<InventoryType>)Inventories.Player).TryGetFirstItem((Predicate<InventoryItem>)((InventoryItem x) => x.ItemId == 0), out InventoryItem* itemResult))
					{
						goto end_IL_016a;
					}
					((InventoryManager)ptr).MoveItemSlot(item, (ushort)k, ((InventoryItem)itemResult).Container, (ushort)((InventoryItem)itemResult).Slot, true);
					num++;
				}
			}
			continue;
			end_IL_016a:
			break;
		}
		OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoMoveGearsNotInSet-Notification", num)));
	}
}
