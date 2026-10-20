using System;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Component.GUI;
using OmenTools.Global;
using OmenTools.Interop.Game.Models;

namespace DailyRoutines.ModulesPublic.Interface;

public class LargerIME : ModuleBase
{
	private unsafe delegate void TextInputReceiveEventDelegate(AtkComponentTextInput* component, AtkEventType eventType, int i, AtkEvent* atkEvent, AtkEventData* eventData);

	private class Config : ModuleConfig
	{
		public float Scale = 2f;
	}

	private static readonly CompSig TextInputReceiveEventSig = new CompSig("4C 8B DC 55 53 57 41 54 41 57 49 8D AB ?? ?? ?? ?? 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 85 ?? ?? ?? ?? 48 8B 9D ?? ?? ?? ??");

	private Hook<TextInputReceiveEventDelegate>? TextInputReceiveEventHook;

	private Config config;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("LargerIMETitle"),
		Description = LanguageManager.Get("LargerIMEDescription"),
		Category = ModuleCategory.Interface
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected unsafe override void Init()
	{
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		if (TextInputReceiveEventHook == null)
		{
			TextInputReceiveEventHook = TextInputReceiveEventSig.GetHook<TextInputReceiveEventDelegate>(TextInputReceiveEventDetour);
		}
		TextInputReceiveEventHook.Enable();
	}

	protected override void ConfigUI()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		ImGui.SetNextItemWidth(100f * Globals.GlobalUIScale);
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(17, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("Scale"));
		((ImU8String)(ref val)).AppendLiteral("###FontScaleInput");
		if (ImGui.InputFloat(val, ref config.Scale, 0.1f, 1f, ImU8String.op_Implicit("%.1f"), (ImGuiInputTextFlags)0))
		{
			config.Scale = MathF.Max(0.1f, config.Scale);
		}
		if (ImGui.IsItemDeactivatedAfterEdit())
		{
			config.Save(this);
		}
	}

	private unsafe void TextInputReceiveEventDetour(AtkComponentTextInput* component, AtkEventType eventType, int i, AtkEvent* atkEvent, AtkEventData* eventData)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		TextInputReceiveEventHook.Original(component, eventType, i, atkEvent, eventData);
		ModifyTextInputComponent(component);
	}

	private unsafe void ModifyTextInputComponent(AtkComponentTextInput* component)
	{
		if (component != null)
		{
			AtkResNode* ptr = ((AtkUldManager)(&((AtkComponentBase)(&((AtkComponentInputBase)(&((AtkComponentTextInput)component).AtkComponentInputBase)).AtkComponentBase)).UldManager)).SearchNodeById(4u);
			if (ptr != null)
			{
				((AtkResNode)ptr).SetScale(config.Scale, config.Scale);
			}
		}
	}
}
