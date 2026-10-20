using System;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;

namespace DailyRoutines.ModulesPublic.Interface;

public class AutoHideExpBar : ModuleBase
{
	private Hook<UpdateExp>? UpdateExpHook;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoHideExpBarTitle"),
		Description = LanguageManager.Get("AutoHideExpBarDescription"),
		Category = ModuleCategory.Interface
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected unsafe override void Init()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		UpdateExpHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromMemberFunction<UpdateExp>(typeof(MemberFunctionPointers), "UpdateExp", new UpdateExp(UpdateExpDetour));
		UpdateExpHook.Enable();
	}

	private unsafe void UpdateExpDetour(AgentHUD* agent, NumberArrayData* expNumberArray, StringArrayData* expStringArray, StringArrayData* characterStringArray)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		UpdateExpHook.Original.Invoke(agent, expNumberArray, expStringArray, characterStringArray);
		if (Addons.Exp != null)
		{
			((AtkUnitBase)Addons.Exp).IsVisible = !((Enum)((AgentHUD)agent).ExpFlags).HasFlag((Enum)(object)(AgentHudExpFlag)8);
		}
	}
}
