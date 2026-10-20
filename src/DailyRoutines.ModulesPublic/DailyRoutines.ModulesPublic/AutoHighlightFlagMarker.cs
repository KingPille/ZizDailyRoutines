using System;
using System.Linq;
using System.Numerics;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using OmenTools;
using OmenTools.Dalamud.Helpers;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Models.Native;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.ModulesPublic;

public class AutoHighlightFlagMarker : ModuleBase
{
	private unsafe delegate void SetFlagMarkerDelegate(AgentMap* agent, uint zoneID, uint mapID, float worldX, float worldZ, uint iconID = 60561u);

	private class Config : ModuleConfig
	{
		public bool ConstantlyUpdate;
	}

	private Hook<SetFlagMarkerDelegate>? SetFlagMarkerHook;

	private Hook<AgentReceiveEventDelegate>? AgentMapReceiveEventHook;

	private Config config;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoHighlightFlagMarkerTitle"),
		Description = LanguageManager.Get("AutoHighlightFlagMarkerDescription"),
		Category = ModuleCategory.System,
		ModulesConflict = new string[1] { "MultiTargetTracker" }
	};

	protected unsafe override void Init()
	{
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 15000
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		if (SetFlagMarkerHook == null)
		{
			SetFlagMarkerHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromAddress<SetFlagMarkerDelegate>((IntPtr)DalamudReflector.GetMemberFuncByName(typeof(MemberFunctionPointers), "SetFlagMapMarker"), (SetFlagMarkerDelegate)SetFlagMarkerDetour, (HookBackend)0);
		}
		SetFlagMarkerHook.Enable();
		if (AgentMapReceiveEventHook == null)
		{
			AgentMapReceiveEventHook = (*((AgentMap)AgentMap.Instance()).VirtualTable).HookVFuncFromName<AgentMapVirtualTable, AgentReceiveEventDelegate>("ReceiveEvent", (AgentReceiveEventDelegate)AgentMapReceiveEventDetour);
		}
		AgentMapReceiveEventHook.Enable();
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += OnZoneChanged;
		OmenServiceBase<FrameworkManager>.Instance().Reg(new OnUpdateDelegate(OnUpdate), 3000u);
	}

	protected override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		OmenServiceBase<FrameworkManager>.Instance().Unreg(OnUpdate);
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChanged;
	}

	protected override void ConfigUI()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("AutoHighlightFlagMarker-ConstantlyUpdate")), ref config.ConstantlyUpdate))
		{
			config.Save(this);
		}
	}

	private unsafe void SetFlagMarkerDetour(AgentMap* agent, uint zoneID, uint mapID, float worldX, float worldZ, uint iconID = 60561u)
	{
		SetFlagMarkerHook.Original(agent, zoneID, mapID, worldX, worldZ, iconID);
		if (mapID == DalamudServiceExtension.Instance<IClientState>().MapId && iconID == 60561)
		{
			OnZoneChanged(0u);
		}
	}

	private unsafe AtkValue* AgentMapReceiveEventDetour(AgentInterface* agent, AtkValue* returnValues, AtkValue* values, uint valueCount, ulong eventKind)
	{
		AtkValue* result = AgentMapReceiveEventHook.Original(agent, returnValues, values, valueCount, eventKind);
		if (eventKind == 0L && valueCount != 0 && ((AtkValue)values).Int == 10)
		{
			OnZoneChanged(0u);
		}
		return result;
	}

	private unsafe void OnZoneChanged(uint u)
	{
		if (!IsFlagMarkerValid())
		{
			return;
		}
		base.TaskHelper.Abort();
		base.TaskHelper.Enqueue(() => IOmenDalamudService<IObjectTable>.Instance().LocalPlayer != null && !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)45]);
		base.TaskHelper.Enqueue(delegate
		{
			if (!IsFlagMarkerValid())
			{
				base.TaskHelper.Abort();
			}
		});
		base.TaskHelper.Enqueue(delegate
		{
			AgentMap* ptr = AgentMap.Instance();
			Vector2 vector = new Vector2(((AgentMap)ptr).FlagMapMarkers[0].XFloat, ((AgentMap)ptr).FlagMapMarkers[0].YFloat);
			float num = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer?.Position.Y ?? 0f;
			int num2 = 0;
			FieldMarkerPoint[] values = Enum.GetValues<FieldMarkerPoint>();
			for (int i = 0; i < values.Length; i++)
			{
				FieldMarkerPoint point = values[i];
				Vector3 value = vector.ToVector3(num - 2f + (float)(num2 * 5));
				Vector3 position = point.GetPosition();
				if (!(Vector3.DistanceSquared(value, position) <= 9f))
				{
					(*MarkingController.Instance()).PlaceFieldMarkerLocal(in point, vector.ToVector3(num - 2f + (float)(num2 * 5)));
					num2++;
				}
			}
		});
	}

	private unsafe static void ClearMarkers()
	{
		MarkingController* ptr = MarkingController.Instance();
		if (ptr != null)
		{
			FieldMarker[] source = ((MarkingController)ptr).FieldMarkers.ToArray();
			if (Enumerable.Count(source, (FieldMarker x) => x.Active) == 8 && source.Select((FieldMarker x) => x.Position.ToVector2()).ToHashSet().Count == 1)
			{
				((MarkingController)ptr).FieldMarkers.Clear();
			}
		}
	}

	private unsafe void OnUpdate(IFramework _)
	{
		if (!config.ConstantlyUpdate)
		{
			return;
		}
		if (!IsFlagMarkerValid())
		{
			ClearMarkers();
		}
		else
		{
			if (base.TaskHelper.IsBusy)
			{
				return;
			}
			int num = 0;
			FieldMarkerPoint[] values = Enum.GetValues<FieldMarkerPoint>();
			for (int i = 0; i < values.Length; i++)
			{
				FieldMarkerPoint point = values[i];
				AgentMap* ptr = AgentMap.Instance();
				Vector2 vector = new Vector2(((AgentMap)ptr).FlagMapMarkers[0].XFloat, ((AgentMap)ptr).FlagMapMarkers[0].YFloat);
				float num2 = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer?.Position.Y ?? 0f;
				Vector3 value = vector.ToVector3(num2 - 2f + (float)(num * 5));
				Vector3 position = point.GetPosition();
				if (!(Vector3.DistanceSquared(value, position) <= 9f) || !((MarkingController)MarkingController.Instance()).FieldMarkers[(int)point].Active)
				{
					(*MarkingController.Instance()).PlaceFieldMarkerLocal(in point, vector.ToVector3(num2 - 2f + (float)(num * 5)));
					num++;
				}
			}
		}
	}

	private static bool IsFlagMarkerValid()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (!GameState.IsFlagMarkerSet)
		{
			return false;
		}
		FlagMapMarker flagMarker = GameState.FlagMarker;
		if (flagMarker.TerritoryId == 0 || flagMarker.MapId == 0 || flagMarker.TerritoryId != GameState.TerritoryType)
		{
			return false;
		}
		return true;
	}
}
