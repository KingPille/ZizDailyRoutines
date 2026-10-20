using System;
using System.Collections.Generic;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.Gui.Dtr;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Hooking;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Enums;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Interop;
using InteropGenerator.Runtime;
using Lumina.Excel.Sheets;
using Lumina.Text;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic.Interface;

public class AutoDisplayIDInformation : ModuleBase
{
	private unsafe delegate CStringPointer GetStatusTooltipTextDelegate(AgentHUD* agent, Utf8String* output, uint statusID, uint param);

	private class Config : ModuleConfig
	{
		public bool ShowActionID = true;

		public bool ShowActionIDOriginal = true;

		public bool ShowActionIDResolved = true;

		public bool ShowItemID = true;

		public bool ShowTargetID = true;

		public bool ShowTargetIDBattleNPC = true;

		public bool ShowTargetIDCompanion = true;

		public bool ShowTargetIDEventNPC = true;

		public bool ShowTargetIDOthers = true;

		public bool ShowStatusID = true;

		public bool ShowWeatherID = true;

		public bool ShowZoneInfo = true;
	}

	private static readonly CompSig GetStatusTooltipTextSig = new CompSig("40 55 41 54 41 55 41 56 41 57 48 8D 6C 24 90 48 81 EC 70 01 00 00");

	private Hook<GetStatusTooltipTextDelegate>? GetStatusTooltipTextHook;

	private Config config;

	private IDtrBarEntry? zoneInfoEntry;

	private AtkEventWrapper? naviMapMouseOverEvent;

	private AtkEventWrapper? naviMapMouseOutEvent;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoDisplayIDInformationTitle"),
		Description = LanguageManager.Get("AutoDisplayIDInformationDescription"),
		Category = ModuleCategory.Interface,
		Author = new string[1] { "Middo" }
	};

	protected unsafe override void Init()
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Expected O, but got Unknown
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Expected O, but got Unknown
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		if (zoneInfoEntry == null)
		{
			zoneInfoEntry = DalamudServiceExtension.Instance<IDtrBar>().Get("AutoDisplayIDInformation-ZoneInfo", (SeString)null);
		}
		OmenServiceBase<TooltipManager>.Instance().RegItem(OnItemTooltip);
		OmenServiceBase<TooltipManager>.Instance().RegAction(OnActionTooltip);
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)7, "_TargetInfo", new AddonEventDelegate(OnAddonTarget));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)7, "_TargetInfoMainTarget", new AddonEventDelegate(OnAddonTarget));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)5, "_NaviMap", new AddonEventDelegate(OnAddonNaviMap));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, "_NaviMap", new AddonEventDelegate(OnAddonNaviMap));
		if (GetStatusTooltipTextHook == null)
		{
			GetStatusTooltipTextHook = GetStatusTooltipTextSig.GetHook<GetStatusTooltipTextDelegate>(GetStatusTooltipTextDetour);
		}
		GetStatusTooltipTextHook.Enable();
		DalamudServiceExtension.Instance<IClientState>().MapIdChanged += OnMapChanged;
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += OnZoneChanged;
		UpdateDTRInfo();
	}

	protected override void Uninit()
	{
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Expected O, but got Unknown
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IClientState>().MapIdChanged -= OnMapChanged;
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChanged;
		IDtrBarEntry? obj = zoneInfoEntry;
		if (obj != null)
		{
			obj.Remove();
		}
		zoneInfoEntry = null;
		OmenServiceBase<TooltipManager>.Instance().Unreg(OnItemTooltip);
		OmenServiceBase<TooltipManager>.Instance().Unreg(OnActionTooltip);
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[2] { OnAddonTarget, OnAddonNaviMap });
		OnAddonNaviMap((AddonEvent)6, null);
	}

	protected override void ConfigUI()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(3, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LuminaWrapper.GetAddonText(520u));
		((ImU8String)(ref val)).AppendLiteral(" ID");
		if (ImGui.Checkbox(val, ref config.ShowItemID))
		{
			config.Save(this);
		}
		ImGui.NewLine();
		ImU8String val2 = default(ImU8String);
		((ImU8String)(ref val2))._002Ector(3, 1);
		((ImU8String)(ref val2)).AppendFormatted<string>(LuminaWrapper.GetAddonText(1340u));
		((ImU8String)(ref val2)).AppendLiteral(" ID");
		if (ImGui.Checkbox(val2, ref config.ShowActionID))
		{
			config.Save(this);
		}
		if (config.ShowActionID)
		{
			IndentDisposable val3 = ImRaii.PushIndent(1, true);
			try
			{
				if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("Resolved")), ref config.ShowActionIDResolved))
				{
					config.Save(this);
				}
				if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("Original")), ref config.ShowActionIDOriginal))
				{
					config.Save(this);
				}
			}
			finally
			{
				((IDisposable)val3)?.Dispose();
			}
		}
		ImGui.NewLine();
		ImU8String val4 = default(ImU8String);
		((ImU8String)(ref val4))._002Ector(3, 1);
		((ImU8String)(ref val4)).AppendFormatted<string>(LuminaWrapper.GetAddonText(1030u));
		((ImU8String)(ref val4)).AppendLiteral(" ID");
		if (ImGui.Checkbox(val4, ref config.ShowTargetID))
		{
			config.Save(this);
		}
		if (config.ShowTargetID)
		{
			IndentDisposable val3 = ImRaii.PushIndent(1, true);
			try
			{
				if (ImGui.Checkbox(ImU8String.op_Implicit("BattleNPC"), ref config.ShowTargetIDBattleNPC))
				{
					config.Save(this);
				}
				if (ImGui.Checkbox(ImU8String.op_Implicit("EventNPC"), ref config.ShowTargetIDEventNPC))
				{
					config.Save(this);
				}
				if (ImGui.Checkbox(ImU8String.op_Implicit("Companion"), ref config.ShowTargetIDCompanion))
				{
					config.Save(this);
				}
				if (ImGui.Checkbox(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(832u)), ref config.ShowTargetIDOthers))
				{
					config.Save(this);
				}
			}
			finally
			{
				((IDisposable)val3)?.Dispose();
			}
		}
		ImGui.NewLine();
		ImU8String val5 = default(ImU8String);
		((ImU8String)(ref val5))._002Ector(3, 1);
		((ImU8String)(ref val5)).AppendFormatted<string>(LanguageManager.Get("Status"));
		((ImU8String)(ref val5)).AppendLiteral(" ID");
		if (ImGui.Checkbox(val5, ref config.ShowStatusID))
		{
			config.Save(this);
		}
		ImGui.NewLine();
		ImU8String val6 = default(ImU8String);
		((ImU8String)(ref val6))._002Ector(3, 1);
		((ImU8String)(ref val6)).AppendFormatted<string>(LuminaWrapper.GetAddonText(8555u));
		((ImU8String)(ref val6)).AppendLiteral(" ID");
		if (ImGui.Checkbox(val6, ref config.ShowWeatherID))
		{
			config.Save(this);
		}
		ImGui.NewLine();
		ImU8String val7 = default(ImU8String);
		((ImU8String)(ref val7))._002Ector(0, 1);
		((ImU8String)(ref val7)).AppendFormatted<string>(LuminaWrapper.GetAddonText(870u));
		if (ImGui.Checkbox(val7, ref config.ShowZoneInfo))
		{
			config.Save(this);
		}
	}

	private void OnMapChanged(uint obj)
	{
		UpdateDTRInfo();
	}

	private void OnZoneChanged(uint u)
	{
		UpdateDTRInfo();
	}

	private unsafe void OnAddonNaviMap(AddonEvent type, AddonArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Invalid comparison between Unknown and I4
		if ((int)type != 5)
		{
			if ((int)type == 6)
			{
				naviMapMouseOverEvent?.Dispose();
				naviMapMouseOverEvent = null;
				naviMapMouseOutEvent?.Dispose();
				naviMapMouseOutEvent = null;
			}
		}
		else
		{
			if (Addons.NaviMap == null || (naviMapMouseOutEvent != null && naviMapMouseOverEvent != null))
			{
				return;
			}
			AtkComponentBase* componentByNodeId = ((AtkUnitBase)Addons.NaviMap).GetComponentByNodeId(14u);
			if (componentByNodeId == null)
			{
				return;
			}
			AtkResNode* collisionNode = ((AtkUldManager)(&((AtkComponentBase)componentByNodeId).UldManager)).SearchNodeById(5u);
			if (collisionNode == null)
			{
				return;
			}
			((AtkResNode)collisionNode).ClearEvents();
			naviMapMouseOverEvent = new AtkEventWrapper(delegate(AtkEventType _, AtkUnitBase* addon, AtkEvent* _, AtkEventData* _)
			{
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_0039: Unknown result type (might be due to invalid IL or missing references)
				//IL_003e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				//IL_0074: Unknown result type (might be due to invalid IL or missing references)
				//IL_0087: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
				if (!LuminaGetter.TryGetRow<Weather>((uint)((WeatherManager)WeatherManager.Instance()).WeatherId, out Weather item))
				{
					return;
				}
				RentedSeStringBuilder val = default(RentedSeStringBuilder);
				((RentedSeStringBuilder)(ref val))._002Ector();
				try
				{
					RentedAtkValues val2 = new RentedAtkValues(1);
					try
					{
						ref AtkValue reference = ref ((RentedAtkValues)(ref val2))[0];
						SeStringBuilder builder = ((RentedSeStringBuilder)(ref val)).Builder;
						SeStringInterpolatedStringHandler val3 = new SeStringInterpolatedStringHandler(3, 2, builder);
						((SeStringInterpolatedStringHandler)(ref val3)).AppendFormatted(((Weather)(ref item)).Name);
						((SeStringInterpolatedStringHandler)(ref val3)).AppendLiteral(" [");
						((SeStringInterpolatedStringHandler)(ref val3)).AppendFormatted(((Weather)(ref item)).RowId);
						((SeStringInterpolatedStringHandler)(ref val3)).AppendLiteral("]");
						((AtkValue)(ref reference)).SetManagedString(builder.Append(val3).GetViewAsSpan());
						AtkTooltipArgs val4 = new AtkTooltipArgs
						{
							TextArgs = 
							{
								AtkArrayType = 0u
							},
							TextArgs = 
							{
								Text = ((RentedAtkValues)(ref val2))[0].String
							}
						};
						((AtkTooltipManager)(&((AtkStage)AtkStage.Instance()).TooltipManager)).ShowTooltip((AtkTooltipType)1, ((AtkUnitBase)addon).Id, collisionNode, &val4, (delegate* unmanaged[Stdcall]<float*, float*, AtkResNode*, void>)null, false, true);
					}
					finally
					{
						((RentedAtkValues)(ref val2)).Dispose();
					}
				}
				finally
				{
					((IDisposable)(*(RentedSeStringBuilder*)(&val))/*cast due to constrained. prefix*/).Dispose();
				}
			});
			naviMapMouseOverEvent.Add(Addons.NaviMap, collisionNode, (AtkEventType)6);
			naviMapMouseOutEvent = new AtkEventWrapper(delegate(AtkEventType _, AtkUnitBase* addon, AtkEvent* _, AtkEventData* _)
			{
				((AtkTooltipManager)(&((AtkStage)AtkStage.Instance()).TooltipManager)).HideTooltip(((AtkUnitBase)addon).Id, false);
			});
			naviMapMouseOutEvent.Add(Addons.NaviMap, collisionNode, (AtkEventType)7);
		}
	}

	private unsafe void OnAddonTarget(AddonEvent type, AddonArgs args)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Invalid comparison between Unknown and I4
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Invalid comparison between Unknown and I4
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Invalid comparison between Unknown and I4
		if (!config.ShowTargetID)
		{
			return;
		}
		IGameObject target = TargetManager.Target;
		if (target == null || target.DataID == 0)
		{
			return;
		}
		ObjectKind objectKind = target.ObjectKind;
		if (!(((int)objectKind == 2) ? config.ShowTargetIDBattleNPC : (((int)objectKind == 3) ? config.ShowTargetIDEventNPC : (((int)objectKind != 9) ? config.ShowTargetIDOthers : config.ShowTargetIDCompanion))))
		{
			return;
		}
		StringArrayData* stringArrayData = ((AtkStage)AtkStage.Instance()).GetStringArrayData((StringArrayType)9);
		if (stringArrayData == null)
		{
			return;
		}
		Utf8String val = default(Utf8String);
		((Utf8String)(ref val))._002Ector($"{target.Name} [{target.DataID}]");
		try
		{
			((StringArrayData)stringArrayData).SetValue(0, $"{target.Name} [{target.DataID}]", true, true, false);
		}
		finally
		{
			((IDisposable)(*(Utf8String*)(&val))/*cast due to constrained. prefix*/).Dispose();
		}
	}

	private unsafe void OnItemTooltip(ItemKind itemKind, uint itemID, ref List<TooltipItemModification> modifications)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		if (!config.ShowItemID)
		{
			return;
		}
		RentedSeStringBuilder val = default(RentedSeStringBuilder);
		((RentedSeStringBuilder)(ref val))._002Ector();
		try
		{
			((RentedSeStringBuilder)(ref val)).Builder.PushColorType(3u).Append(" [").Append(itemID)
				.Append(']')
				.PopColorType();
			modifications.Add(new TooltipItemModification
			{
				Target = TooltipItemType.UICategory,
				Type = TooltipModificationType.Append,
				Text = ((RentedSeStringBuilder)(ref val)).Builder.ToReadOnlySeString()
			});
		}
		finally
		{
			((IDisposable)(*(RentedSeStringBuilder*)(&val))/*cast due to constrained. prefix*/).Dispose();
		}
	}

	private unsafe void OnActionTooltip(DetailKind actionKind, uint actionID, ref List<TooltipActionModification> modifications)
	{
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		if (!this.config.ShowActionID)
		{
			return;
		}
		RentedSeStringBuilder val = default(RentedSeStringBuilder);
		((RentedSeStringBuilder)(ref val))._002Ector();
		try
		{
			uint originalId = ((AgentActionDetail)AgentActionDetail.Instance()).OriginalId;
			Config config = this.config;
			uint num = ((config != null && config.ShowActionIDResolved && !config.ShowActionIDOriginal) ? actionID : originalId);
			((RentedSeStringBuilder)(ref val)).Builder.PushColorType(3u).Append(" [").Append(num);
			config = this.config;
			if (config != null && config.ShowActionIDResolved && config.ShowActionIDOriginal && num != actionID)
			{
				SeStringBuilder builder = ((RentedSeStringBuilder)(ref val)).Builder;
				SeStringInterpolatedStringHandler val2 = new SeStringInterpolatedStringHandler(3, 1, builder);
				((SeStringInterpolatedStringHandler)(ref val2)).AppendLiteral(" → ");
				((SeStringInterpolatedStringHandler)(ref val2)).AppendFormatted(actionID);
				builder.Append(val2);
			}
			((RentedSeStringBuilder)(ref val)).Builder.Append("]").PopColorType();
			modifications.Add(new TooltipActionModification
			{
				Target = TooltipActionType.Category,
				Type = TooltipModificationType.Append,
				Text = ((RentedSeStringBuilder)(ref val)).Builder.ToReadOnlySeString()
			});
		}
		finally
		{
			((IDisposable)(*(RentedSeStringBuilder*)(&val))/*cast due to constrained. prefix*/).Dispose();
		}
	}

	private unsafe CStringPointer GetStatusTooltipTextDetour(AgentHUD* agent, Utf8String* output, uint statusID, uint param)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		CStringPointer result = GetStatusTooltipTextHook.Original(agent, output, statusID, param);
		if (!config.ShowStatusID || statusID == 0 || statusID == uint.MaxValue || !((CStringPointer)(ref result)).HasValue)
		{
			return result;
		}
		string text = ((object)(*(CStringPointer*)(&result))/*cast due to constrained. prefix*/).ToString();
		if (string.IsNullOrEmpty(text))
		{
			return result;
		}
		int num = text.IndexOf('\n');
		string text2 = ((num < 0) ? $"{text} [{statusID}]" : $"{text.Substring(0, num)} [{statusID}]{text.Substring(num)}");
		Utf8String val = default(Utf8String);
		((Utf8String)(ref val))._002Ector(text2);
		try
		{
			((Utf8String)output).Copy(&val);
			return ((Utf8String)output).StringPtr;
		}
		finally
		{
			((IDisposable)(*(Utf8String*)(&val))/*cast due to constrained. prefix*/).Dispose();
		}
	}

	private void UpdateDTRInfo()
	{
		if (config.ShowZoneInfo)
		{
			uint map = GameState.Map;
			uint territoryType = GameState.TerritoryType;
			if (map == 0 || territoryType == 0)
			{
				zoneInfoEntry.Shown = false;
				return;
			}
			zoneInfoEntry.Shown = true;
			zoneInfoEntry.Text = SeString.op_Implicit($"{LuminaWrapper.GetAddonText(870u)}: {territoryType} / {LuminaWrapper.GetAddonText(670u)}: {map}");
		}
		else
		{
			zoneInfoEntry.Shown = false;
		}
	}
}
