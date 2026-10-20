using System;
using System.Collections.Frozen;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Interface.Windows;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Game.Command;
using Dalamud.Game.Gui.Dtr;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using Dalamud.Utility.Numerics;
using FFXIVClientStructs.FFXIV.Client.Enums;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Interop;
using FFXIVClientStructs.STD;
using FFXIVClientStructs.STD.Helper;
using Lumina.Excel.Sheets;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Info.Game.Enums;
using OmenTools.Info.Game.Packets.Upstream;
using OmenTools.Interop.Game.Helpers;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.ModulesPublic;

public class FastInstanceZoneChange : ModuleBase
{
	private class Config : ModuleConfig
	{
		public bool AddDtrEntry = true;

		public bool CloseAfterUsage = true;

		public bool ConstantlyTry = true;

		public bool MountAfterChange = true;

		public bool TeleportIfNotNearAetheryte = true;
	}

	private Config config;

	private IDtrBarEntry? entry;

	private const string COMMAND = "insc";

	private unsafe static readonly FrozenSet<TerritoryIntendedUse> ValidUses = FrozenSet.Create(new ReadOnlySpan<TerritoryIntendedUse>(Unsafe.AsPointer(ref global::_003CPrivateImplementationDetails_003E._47DC540C94CEB704A23875C11273E16BB0B8A87AED84DE911F2133568115F254), 2));

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("FastInstanceZoneChangeTitle"),
		Description = LanguageManager.Get("FastInstanceZoneChangeDescription", "insc"),
		Category = ModuleCategory.System,
		Author = new string[2] { "ziz", "KirisameVanilla" },
		ModulesPair = new string[1] { "InstantTeleport" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected override void Init()
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected O, but got Unknown
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Expected O, but got Unknown
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 30000,
				ShowDebug = true
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		OmenServiceBase<CommandManager>.Instance().AddSubCommand("insc", new CommandInfo(new HandlerDelegate(OnCommand))
		{
			HelpMessage = LanguageManager.Get("FastInstanceZoneChange-CommandHelp")
		});
		if (config.AddDtrEntry)
		{
			HandleDtrEntry(isAdd: true);
			OnConditionChanged((ConditionFlag)45, value: false);
		}
		DalamudServiceExtension.Instance<ICondition>().ConditionChange += new ConditionChangeDelegate(OnConditionChanged);
	}

	protected override void Uninit()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		DalamudServiceExtension.Instance<ICondition>().ConditionChange -= new ConditionChangeDelegate(OnConditionChanged);
		HandleDtrEntry(isAdd: false);
		OmenServiceBase<CommandManager>.Instance().RemoveSubCommand("insc");
	}

	protected override void ConfigUI()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(1, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("Command"));
		((ImU8String)(ref val)).AppendLiteral(":");
		ImGui.TextColored(ref vector, val);
		IndentDisposable val2 = ImRaii.PushIndent(1, true);
		try
		{
			ImU8String val3 = new ImU8String(8, 2);
			((ImU8String)(ref val3)).AppendLiteral("/pdr ");
			((ImU8String)(ref val3)).AppendFormatted<string>("insc");
			((ImU8String)(ref val3)).AppendLiteral(" → ");
			((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get("FastInstanceZoneChange-CommandHelp"));
			ImGui.TextUnformatted(val3);
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
		ImGui.NewLine();
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("FastInstanceZoneChange-TeleportIfNotNearAetheryte")), ref config.TeleportIfNotNearAetheryte))
		{
			config.Save(this);
		}
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("FastInstanceZoneChange-ConstantlyTry")), ref config.ConstantlyTry))
		{
			config.Save(this);
		}
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("FastInstanceZoneChange-MountAfterChange")), ref config.MountAfterChange))
		{
			config.Save(this);
		}
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("FastInstanceZoneChange-AddDtrEntry")), ref config.AddDtrEntry))
		{
			config.Save(this);
			HandleDtrEntry(config.AddDtrEntry);
		}
		if (config.AddDtrEntry && ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("FastInstanceZoneChange-CloseAfterUsage")), ref config.CloseAfterUsage))
		{
			config.Save(this);
		}
	}

	protected unsafe override void OverlayUI()
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		if (!InstancesManager.IsInstancedArea)
		{
			((Window)base.Overlay).IsOpen = false;
			return;
		}
		if (DalamudServiceExtension.Instance<IKeyState>()[(VirtualKey)27])
		{
			((Window)base.Overlay).IsOpen = false;
			if (Addons.SystemMenu != null)
			{
				((AtkUnitBase)Addons.SystemMenu).Close(true);
			}
			return;
		}
		if (!ValidUses.Contains(GameState.TerritoryIntendedUse))
		{
			((Window)base.Overlay).IsOpen = false;
			return;
		}
		ImGuiViewportPtr mainViewport = ImGui.GetMainViewport();
		ImGui.SetWindowPos(((ImGuiViewportPtr)(ref mainViewport)).Size / 2f - new Vector2(0.5f), (ImGuiCond)0);
		int instancesCount = OmenServiceBase<InstancesManager>.Instance().GetInstancesCount();
		ImU8String val = default(ImU8String);
		for (uint num = 1u; num <= instancesCount; num++)
		{
			if (num == InstancesManager.CurrentInstance)
			{
				continue;
			}
			((ImU8String)(ref val))._002Ector(0, 1);
			((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("FastInstanceZoneChange-SwitchInstance", num.ToSESquareCount()));
			if ((ImGui.Button(val, default(Vector2)) | DalamudServiceExtension.Instance<IKeyState>()[(VirtualKey)(ushort)(48 + num)]) && !base.TaskHelper.IsBusy && !IConditionExtension.get_IsBetweenAreas(DalamudServiceExtension.Instance<ICondition>()) && !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)27])
			{
				OmenServiceBase<ChatManager>.Instance().SendMessage($"/pdr insc {num}");
				if (config.CloseAfterUsage)
				{
					((Window)base.Overlay).IsOpen = false;
				}
			}
		}
	}

	private void OnConditionChanged(ConditionFlag flag, bool value)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Invalid comparison between Unknown and I4
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		if (!((int)flag != 45 || value) && config.AddDtrEntry && entry != null)
		{
			entry.Text = SeString.op_Implicit((!InstancesManager.IsInstancedArea) ? string.Empty : LanguageManager.Get("AutoMarksFinder-RelayInstanceDisplay", InstancesManager.CurrentInstance.ToSESquareCount()));
			entry.Shown = InstancesManager.IsInstancedArea;
			entry.Tooltip = SeString.op_Implicit(ValidUses.Contains(GameState.TerritoryIntendedUse) ? LanguageManager.Get("FastInstanceZoneChange-DtrEntryTooltip") : string.Empty);
		}
	}

	private unsafe void OnCommand(string command, string args)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		args = args.Trim().ToLowerInvariant();
		PublicInstance publicInstance = ((UIState)UIState.Instance()).PublicInstance;
		if (args.Length == 0)
		{
			if (((PublicInstance)(ref publicInstance)).IsInstancedArea())
			{
				Overlay? overlay = base.Overlay;
				((Window)overlay).IsOpen = !((Window)overlay).IsOpen;
			}
			else
			{
				OmenServiceBase<NotifyHelper>.Instance().NotificationError(LanguageManager.Get("FastInstanceZoneChange-Notice-NoInstanceZones"));
			}
			return;
		}
		if (args == "abort")
		{
			base.TaskHelper.Abort();
			OmenServiceBase<NotifyHelper>.Instance().NotificationInfo(LanguageManager.Get("FastInstanceZoneChange-Notice-Aborted"));
			return;
		}
		if (!uint.TryParse(args, out var targetInstance))
		{
			OmenServiceBase<NotifyHelper>.Instance().NotificationError(LanguageManager.Get("FastInstanceZoneChange-Notice-InvalidArgs", args));
			return;
		}
		if (!((PublicInstance)(ref publicInstance)).IsInstancedArea())
		{
			OmenServiceBase<NotifyHelper>.Instance().NotificationError(LanguageManager.Get("FastInstanceZoneChange-Notice-NoInstanceZones"));
			return;
		}
		if (publicInstance.InstanceId == targetInstance)
		{
			OmenServiceBase<NotifyHelper>.Instance().NotificationError(LanguageManager.Get("FastInstanceZoneChange-Notice-CurrentlyInSameInstance", targetInstance));
			return;
		}
		base.TaskHelper.Abort();
		OmenServiceBase<NotifyHelper>.Instance().NotificationInfo(LanguageManager.Get("FastInstanceZoneChange-Notice-Change", targetInstance));
		uint eventID;
		bool flag = IsAnyAetheryteNearby(out eventID);
		if (config.TeleportIfNotNearAetheryte && !flag)
		{
			base.TaskHelper.Enqueue(() => OmenServiceBase<AetheryteRecordManager>.Instance().GetNearestAetheryte(GameState.TerritoryType, Vector3.Zero)?.TeleportTo(), "传送到目标区域最近以太之光", 0, null, null, null, null, 2);
			base.TaskHelper.DelayNext(500, "等待传送开始", 2);
			base.TaskHelper.Enqueue(() => Throttler.Shared.Throttle("FastInstanceZoneChange-WaitTeleportFinish") && IsAnyAetheryteNearby(out var _), "传送到目标区域最近以太之光", 0, null, null, null, null, 2);
		}
		uint currentMountID = 0u;
		if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4])
		{
			currentMountID = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer.CurrentMount?.RowId ?? 0;
		}
		base.TaskHelper.Enqueue(delegate
		{
			if (!DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4])
			{
				return true;
			}
			if (!Throttler.Shared.Throttle("FastInstanceZoneChange-WaitDismount", 100u))
			{
				return false;
			}
			OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.Dismount);
			if (RaycastHelper.TryGetGroundPosition(VectorExtensions.WithY(IOmenDalamudService<IObjectTable>.Instance().LocalPlayer.Position, 300f), out var groundPosition))
			{
				MovementManager movementManager = OmenServiceBase<MovementManager>.Instance();
				Vector3 pos = groundPosition;
				pos.Y = groundPosition.Y - 0.5f;
				movementManager.TPPlayerAddress(pos);
				OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)5, 9u, 3758096384uL, 0u, (UseActionMode)0);
			}
			return !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4];
		}, "下坐骑", 0, null, null, null, null, 2);
		if (config.ConstantlyTry)
		{
			base.TaskHelper.Enqueue(delegate
			{
				EnqueueInstanceChange(targetInstance, 0u);
			}, "开始持续尝试切换副本区", 0, null, null, null, null, 2);
		}
		else
		{
			base.TaskHelper.Enqueue(delegate
			{
				ChangeInstanceZone(targetInstance);
			}, "切换副本区", 0, null, null, null, null, 2);
		}
		base.TaskHelper.Enqueue(() => (InstancesManager.CurrentInstance == targetInstance && !IConditionExtension.get_IsBetweenAreas(DalamudServiceExtension.Instance<ICondition>())) ? true : false, "等待切换完毕, 更新副本区信息");
		if (!config.MountAfterChange)
		{
			return;
		}
		base.TaskHelper.Enqueue(delegate
		{
			if (!UIModuleExtension.IsScreenReady())
			{
				return false;
			}
			if (((ActionManager)ActionManager.Instance()).GetActionStatus((ActionType)5, 9u, 3758096384uL, true, true, (uint*)null) != 0)
			{
				return true;
			}
			if (currentMountID != 0)
			{
				OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)13, currentMountID, 3758096384uL, 0u, (UseActionMode)0);
			}
			else
			{
				OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)5, 9u, 3758096384uL, 0u, (UseActionMode)0);
			}
			return true;
		}, "切换完毕后上坐骑");
	}

	private void HandleDtrEntry(bool isAdd)
	{
		if (isAdd)
		{
			if (entry != null)
			{
				return;
			}
			if (entry == null)
			{
				entry = DalamudServiceExtension.Instance<IDtrBar>().Get("DailyRoutines-FastInstanceZoneChange", (SeString)null);
			}
			entry.OnClick = delegate
			{
				if (base.Overlay == null)
				{
					Overlay overlay = new Overlay(this);
					((Window)overlay).WindowName = LanguageManager.Get("FastInstanceZoneChangeTitle");
					Overlay overlay2 = overlay;
					base.Overlay = overlay;
				}
				Overlay? overlay3 = base.Overlay;
				((Window)overlay3).IsOpen = !((Window)overlay3).IsOpen;
			};
			entry.Shown = false;
			entry.Tooltip = SeString.op_Implicit(LanguageManager.Get("FastInstanceZoneChange-DtrEntryTooltip"));
		}
		else
		{
			IDtrBarEntry? obj = entry;
			if (obj != null)
			{
				obj.Remove();
			}
			entry = null;
		}
	}

	public unsafe void EnqueueInstanceChange(uint i, uint tryTimes)
	{
		base.TaskHelper.Enqueue(() => (*Addons.SelectString).IsAddonAndNodesReady() || !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)45], "等待上一次切换完毕", 0, null, null, null, null, 2);
		base.TaskHelper.Enqueue(delegate
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			if (!Throttler.Shared.Throttle("FastInstanceZoneChange-DetectInstances"))
			{
				return false;
			}
			if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)45])
			{
				OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.StartTerritoryTransport);
				return false;
			}
			PublicInstance publicInstance = ((UIState)UIState.Instance()).PublicInstance;
			if (!((PublicInstance)(ref publicInstance)).IsInstancedArea() || publicInstance.InstanceId == i)
			{
				base.TaskHelper.RemoveQueueTasks(2);
			}
			return true;
		}, "检测切换情况", 0, null, null, null, null, 2);
		base.TaskHelper.Enqueue(delegate
		{
			ChangeInstanceZone(i);
		}, "开始切换", 0, null, null, null, null, 2);
		if (tryTimes != 0)
		{
			base.TaskHelper.Enqueue(delegate
			{
				OmenServiceBase<NotifyHelper>.Instance().NotificationInfo(LanguageManager.Get("FastInstanceZoneChange-Notice-ChangeTimes", tryTimes));
			}, "发送提示信息", 0, null, null, null, null, 2);
		}
		base.TaskHelper.DelayNext(1500, "等待 1.5 秒后继续", 2);
		base.TaskHelper.Enqueue(delegate
		{
			EnqueueInstanceChange(i, tryTimes++);
		}, "开始新一轮切换", 0, null, null, null, null, 2);
	}

	internal unsafe static void ChangeInstanceZone(uint i)
	{
		if (((PublicInstance)(&((UIState)UIState.Instance()).PublicInstance)).IsInstancedArea() && IsAnyAetheryteNearby(out var eventID))
		{
			new EventStartPackt(LocalPlayerState.EntityID, eventID).Send();
			new EventCompletePackt(eventID, 33554432u, 7u, i + 1).Send();
		}
	}

	private unsafe static bool IsAnyAetheryteNearby(out uint eventID)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Invalid comparison between Unknown and I4
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		eventID = 0u;
		Enumerator<StdPair<uint, Pointer<EventHandler>>, uint, PairKeyExtractor<uint, Pointer<EventHandler>>> enumerator = ((EventHandlerModule)(&((EventFramework)EventFramework.Instance()).EventHandlerModule)).EventHandlerMap.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				StdPair<uint, Pointer<EventHandler>> current = enumerator.Current;
				if ((int)((EventId)(&((EventHandlerInfo)(&((EventHandler)current.Item2.Value).Info)).EventId)).ContentId != 5)
				{
					continue;
				}
				Enumerator<Pointer<GameObject>, Pointer<GameObject>, DefaultKeyExtractor<Pointer<GameObject>>> enumerator2 = ((EventHandler)current.Item2.Value).EventObjects.GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						Pointer<GameObject> current2 = enumerator2.Current;
						string nameString = ((GameObject)current2.Value).NameString;
						Aetheryte value = LuminaGetter.GetRow<Aetheryte>(0u).Value;
						if (nameString == ((object)((Aetheryte)(ref value)).Singular/*cast due to constrained. prefix*/).ToString())
						{
							eventID = EventId.op_Implicit(((EventHandlerInfo)(&((EventHandler)current.Item2.Value).Info)).EventId);
							return true;
						}
					}
				}
				finally
				{
					((IDisposable)enumerator2/*cast due to constrained. prefix*/).Dispose();
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
		return false;
	}
}
