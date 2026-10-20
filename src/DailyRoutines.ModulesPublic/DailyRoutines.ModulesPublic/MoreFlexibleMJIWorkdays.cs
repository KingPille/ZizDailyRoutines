using System;
using System.Collections.Generic;
using System.Numerics;
using DailyRoutines.Common.Interface.Windows;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.Info.Game.Data;
using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class MoreFlexibleMJIWorkdays : ModuleBase
{
	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("MoreFlexibleMJIWorkdaysTitle"),
		Description = LanguageManager.Get("MoreFlexibleMJIWorkdaysDescription"),
		Category = ModuleCategory.System
	};

	protected unsafe override void Init()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected O, but got Unknown
		if (base.Overlay == null)
		{
			Overlay overlay = (base.Overlay = new Overlay(this));
		}
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)1, "MJICraftSchedule", new AddonEventDelegate(OnAddon));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, "MJICraftSchedule", new AddonEventDelegate(OnAddon));
		if ((*Addons.MJICraftSchedule).IsAddonAndNodesReady())
		{
			OnAddon((AddonEvent)1, null);
		}
	}

	protected override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddon });
	}

	protected unsafe override void OverlayUI()
	{
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		AgentMJICraftSchedule* ptr = AgentMJICraftSchedule.Instance();
		AtkUnitBase* mJICraftSchedule = Addons.MJICraftSchedule;
		if (mJICraftSchedule == null || ptr == null || ((AgentMJICraftSchedule)ptr).Data == null)
		{
			((Window)base.Overlay).IsOpen = false;
		}
		else
		{
			if (((AtkUnitBase)mJICraftSchedule).GetNodeById(24u) == null)
			{
				return;
			}
			NodeState nodeState = (*((AtkUnitBase)mJICraftSchedule).WindowNode).GetNodeState();
			Vector2 topLeft = nodeState.TopLeft;
			topLeft.Y = nodeState.TopLeft.Y - ImGui.GetWindowSize().Y;
			ImGui.SetWindowPos(topLeft, (ImGuiCond)0);
			if (((ScheduleData)((AgentMJICraftSchedule)ptr).Data).NewRestCycles == 0)
			{
				((ScheduleData)((AgentMJICraftSchedule)ptr).Data).NewRestCycles = ((ScheduleData)((AgentMJICraftSchedule)ptr).Data).RestCycles;
			}
			List<bool> list = DecodeRestDays(((ScheduleData)((AgentMJICraftSchedule)ptr).Data).NewRestCycles);
			GroupDisposable val = ImRaii.Group();
			try
			{
				for (int i = 0; i < list.Count; i++)
				{
					bool value = list[i];
					ImU8String val2 = new ImU8String(5, 1);
					((ImU8String)(ref val2)).AppendLiteral("##Day");
					((ImU8String)(ref val2)).AppendFormatted<int>(i);
					if (ImGui.Checkbox(val2, ref value))
					{
						list[i] = value;
						uint num = EncodeRestDays(list);
						((ScheduleData)((AgentMJICraftSchedule)ptr).Data).RestCycles = num;
						((ScheduleData)((AgentMJICraftSchedule)ptr).Data).NewRestCycles = num;
						List<int> list2 = new List<int>();
						for (int j = 0; j < list.Count; j++)
						{
							if (list[j])
							{
								list2.Add(j);
							}
						}
						while (list2.Count < 4)
						{
							list2.Add(0);
						}
						OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.SetMJIWorkshopRest, (uint)list2[0], (uint)list2[1], (uint)list2[2], (uint)list2[3]);
						OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestMJIWorkshop, ((ScheduleData)((AgentMJICraftSchedule)ptr).Data).CycleDisplayed);
					}
					switch (i)
					{
					case 6:
						ImGui.SameLine(0f, 4f * Globals.GlobalUIScale);
						ImGui.TextUnformatted(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(15107u)));
						break;
					case 13:
						ImGui.SameLine(0f, 4f * Globals.GlobalUIScale);
						ImGui.TextUnformatted(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(15108u)));
						break;
					}
					if (i != 6)
					{
						ImGui.SameLine();
					}
				}
			}
			finally
			{
				((GroupDisposable)(ref val)).Dispose();
			}
		}
	}

	private void OnAddon(AddonEvent type, AddonArgs? args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Invalid comparison between Unknown and I4
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		Overlay overlay = base.Overlay;
		bool isOpen = (int)type == 1 || ((int)type != 6 && ((Window)base.Overlay).IsOpen);
		((Window)overlay).IsOpen = isOpen;
	}

	private static uint EncodeRestDays(List<bool> restDays)
	{
		if (restDays.Count != 14)
		{
			throw new ArgumentException("休息日列表长度必须为 14");
		}
		uint num = 0u;
		for (int i = 0; i < 7; i++)
		{
			if (restDays[i])
			{
				num |= (uint)(1 << i);
			}
		}
		for (int j = 7; j < 14; j++)
		{
			if (restDays[j])
			{
				num |= (uint)(1 << j);
			}
		}
		return num;
	}

	private static List<bool> DecodeRestDays(uint value)
	{
		List<bool> list = new List<bool>(14);
		for (int i = 0; i < 14; i++)
		{
			list.Add(item: false);
		}
		for (int j = 0; j < 14; j++)
		{
			list[j] = (value & (uint)(1 << j)) != 0;
		}
		return list;
	}
}
