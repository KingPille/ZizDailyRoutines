using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Game.Agent;
using Dalamud.Game.Agent.AgentArgTypes;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic.Interface;

public class FastCustomDeliveriesInfo : ModuleBase
{
	private record CustomDeliveryInfo(uint Index, string Name, uint Map, Vector3 Position);

	private sealed class CustomDeliveryContextMenu(CustomDeliveryInfo info) : ContextMenuEntry
	{
		public override string Identifier => "FastCustomDeliveriesInfo";

		public unsafe override IReadOnlyList<ContextMenuItem> CreateMultiple(ContextMenuOpenedArgs args)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			return new global::_003C_003Ez__ReadOnlyArray<ContextMenuItem>(new ContextMenuItem[3]
			{
				new ContextMenuItem
				{
					Name = ReadOnlySeString.op_Implicit(LanguageManager.Get("Teleport")),
					OnClicked = delegate
					{
						OmenServiceBase<MovementManager>.Instance().TPSmart_BetweenZone(LuminaWrapper.GetZoneFromMap(info.Map), info.Position);
					}
				},
				new ContextMenuItem
				{
					Name = ReadOnlySeString.op_Implicit(LanguageManager.Get("FastCustomDeliveriesInfo-TeleportToZone")),
					OnClicked = delegate
					{
						uint index = info.Index;
						if (index - 6 <= 1)
						{
							OmenServiceBase<MovementManager>.Instance().TPSmart_BetweenZone(LuminaWrapper.GetZoneFromMap(info.Map));
						}
						else
						{
							OmenServiceBase<AetheryteRecordManager>.Instance().GetNearestAetheryte(LuminaWrapper.GetZoneFromMap(info.Map), info.Position)?.TeleportTo();
						}
					}
				},
				new ContextMenuItem
				{
					Name = ReadOnlySeString.op_Implicit(LuminaWrapper.GetAddonText(8887u)),
					OnClicked = delegate
					{
						DalamudServiceExtension.Instance<IFramework>().RunOnTick((Action)delegate
						{
							(*AgentMap.Instance()).SetMapFlagAndOpen(info.Map, info.Position, info.Name);
						}, default(TimeSpan), 1, default(CancellationToken));
					}
				}
			});
		}
	}

	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static AgentEventDelegate _003C0_003E__OnAgent;
	}

	private static readonly FrozenDictionary<uint, CustomDeliveryInfo> Infos = new Dictionary<uint, CustomDeliveryInfo>
	{
		[12u] = new CustomDeliveryInfo(12u, "缇索加", 855u, new Vector3(90f, -14f, 59.7f)),
		[11u] = new CustomDeliveryInfo(11u, "尼托维凯", 860u, new Vector3(-355.7f, 19.6f, -108.7f)),
		[10u] = new CustomDeliveryInfo(10u, "玛格拉特", 695u, new Vector3(-52.8f, -29.5f, -61.5f)),
		[9u] = new CustomDeliveryInfo(9u, "安登", 494u, new Vector3(-241f, 51f, 615.7f)),
		[8u] = new CustomDeliveryInfo(8u, "阿梅莉安丝", 693u, new Vector3(223f, 25f, -193f)),
		[7u] = new CustomDeliveryInfo(7u, "狄兰达尔伯爵", 574u, new Vector3(-112f, 0f, -135f)),
		[6u] = new CustomDeliveryInfo(6u, "艾尔·图", 574u, new Vector3(110f, -20f, 0f)),
		[5u] = new CustomDeliveryInfo(5u, "凯·希尔", 555u, new Vector3(50f, 83f, -66f)),
		[4u] = new CustomDeliveryInfo(4u, "亚德基拉", 257u, new Vector3(-64f, 206.5f, 22f)),
		[3u] = new CustomDeliveryInfo(3u, "红", 371u, new Vector3(345f, -120f, -302f)),
		[2u] = new CustomDeliveryInfo(2u, "梅·娜格", 366u, new Vector3(162f, 13f, -88f)),
		[1u] = new CustomDeliveryInfo(1u, "熙洛·阿里亚珀", 257u, new Vector3(-72f, 206.5f, 28f))
	}.ToFrozenDictionary();

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("FastCustomDeliveriesInfoTitle"),
		Description = LanguageManager.Get("FastCustomDeliveriesInfoDescription"),
		Category = ModuleCategory.Interface
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		NeedAuth = true
	};

	protected override void Init()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		IAgentLifecycle obj = DalamudServiceExtension.Instance<IAgentLifecycle>();
		object obj2 = _003C_003EO._003C0_003E__OnAgent;
		if (obj2 == null)
		{
			AgentEventDelegate val = OnAgent;
			_003C_003EO._003C0_003E__OnAgent = val;
			obj2 = (object)val;
		}
		obj.RegisterListener((AgentEvent)0, (AgentId)273, (AgentEventDelegate)obj2);
	}

	protected override void Uninit()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		IAgentLifecycle obj = DalamudServiceExtension.Instance<IAgentLifecycle>();
		AgentEventDelegate[] array = new AgentEventDelegate[1];
		object obj2 = _003C_003EO._003C0_003E__OnAgent;
		if (obj2 == null)
		{
			AgentEventDelegate val = OnAgent;
			_003C_003EO._003C0_003E__OnAgent = val;
			obj2 = (object)val;
		}
		array[0] = (AgentEventDelegate)obj2;
		obj.UnregisterListener((AgentEventDelegate[])(object)array);
	}

	private static void ShowContextMenu(CustomDeliveryInfo? selectedInfo)
	{
		if (!(selectedInfo == null))
		{
			OmenServiceBase<ContextMenuManager>.Instance().Open(new ContextMenuOpenedArgs(), new global::_003C_003Ez__ReadOnlySingleElementList<ContextMenuEntry>(new CustomDeliveryContextMenu(selectedInfo)));
		}
	}

	private unsafe static void OnAgent(AgentEvent type, AgentArgs args)
	{
		AgentReceiveEventArgs e = (AgentReceiveEventArgs)(object)((args is AgentReceiveEventArgs) ? args : null);
		if (e.ValueCount < 2)
		{
			return;
		}
		AtkValue* atkValues = (AtkValue*)e.AtkValues;
		if (atkValues != null && ((AtkValue)atkValues).Int == 1)
		{
			uint uInt = ((AtkValue)((byte*)atkValues + Unsafe.SizeOf<AtkValue>())).UInt;
			if (uInt >= 1 && Infos.TryGetValue(uInt, out CustomDeliveryInfo value))
			{
				ShowContextMenu(value);
				((AgentArgs)e).PreventOriginal();
			}
		}
	}
}
