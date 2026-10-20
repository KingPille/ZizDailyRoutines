using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Agent;
using Dalamud.Game.Agent.AgentArgTypes;
using Dalamud.Hooking;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Application.Network;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Helpers;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models.Native;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.ModulesPublic;

public class InstantLogout : ModuleBase
{
	private Hook<HandleMainCommandOperation>? HandleMainCommandOperationHook;

	private Hook<AgentShowDelegate>? AgentCloseMessageShowHook;

	private Hook<ExitFromWindow>? ExitFromWindowHook;

	private Hook<OnLogout>? OnLogoutHook;

	private static readonly TextCommand LogoutLine = LuminaGetter.GetRowOrDefault<TextCommand>(172u);

	private static readonly TextCommand ShutdownLine = LuminaGetter.GetRowOrDefault<TextCommand>(173u);

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("InstantLogoutTitle"),
		Description = LanguageManager.Get("InstantLogoutDescription"),
		Category = ModuleCategory.System
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected unsafe override void Init()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected O, but got Unknown
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Expected O, but got Unknown
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Expected O, but got Unknown
		base.TaskHelper = new TaskHelper();
		OnLogoutHook = (*((LogoutCallbackInterface)(&((AgentLobby)AgentLobby.Instance()).LogoutCallbackInterface)).VirtualTable).HookVFuncFromName<LogoutCallbackInterfaceVirtualTable, OnLogout>("OnLogout", new OnLogout(OnLogoutDetour));
		OnLogoutHook.Enable();
		HandleMainCommandOperationHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromMemberFunction<HandleMainCommandOperation>(typeof(MemberFunctionPointers), "HandleMainCommandOperation", new HandleMainCommandOperation(HandleMainCommandOperationDetour));
		HandleMainCommandOperationHook.Enable();
		AgentCloseMessageShowHook = (*((AgentInterface)((AgentModule)AgentModule.Instance()).GetAgentByInternalId((AgentId)112)).VirtualTable).HookVFuncFromName<AgentInterfaceVirtualTable, AgentShowDelegate>("Show", (AgentShowDelegate)AgentCloseMessageShowDetour);
		AgentCloseMessageShowHook.Enable();
		DalamudServiceExtension.Instance<IAgentLifecycle>().RegisterListener((AgentEvent)0, (AgentId)0, new AgentEventDelegate(OnAgentLobby));
		ExitFromWindowHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromMemberFunction<ExitFromWindow>(typeof(MemberFunctionPointers), "ExitFromWindow", new ExitFromWindow(ExitFromWindowDetour));
		ExitFromWindowHook.Enable();
		OmenServiceBase<ChatManager>.Instance().RegPreExecuteCommandInner(OnPreExecuteCommandInner);
	}

	protected override void Uninit()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		OmenServiceBase<ChatManager>.Instance().Unreg(OnPreExecuteCommandInner);
		DalamudServiceExtension.Instance<IAgentLifecycle>().UnregisterListener((AgentEvent)0, (AgentId)0, new AgentEventDelegate(OnAgentLobby));
	}

	protected override void ConfigUI()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		ImGui.AlignTextToFramePadding();
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(1, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("InstantLogout-ManualOperation"));
		((ImU8String)(ref val)).AppendLiteral(":");
		ImGui.TextColored(ref vector, val);
		IndentDisposable val2 = ImRaii.PushIndent(1, true);
		try
		{
			if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("InstantLogout-Logout")), default(Vector2)))
			{
				Logout(base.TaskHelper);
			}
			ImGui.SameLine();
			if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("InstantLogout-Shutdown")), default(Vector2)))
			{
				Shutdown(base.TaskHelper);
			}
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	private unsafe void OnLogoutDetour(LogoutCallbackInterface* thisPtr, LogoutParams* logoutParams)
	{
		if (base.TaskHelper.IsBusy)
		{
			((LogoutParams)logoutParams).Type = 0;
			((LogoutParams)logoutParams).Code = 10000;
		}
		OnLogoutHook.Original.Invoke(thisPtr, logoutParams);
	}

	private unsafe void ExitFromWindowDetour(Framework* framework)
	{
		Shutdown(base.TaskHelper);
	}

	private unsafe void OnAgentLobby(AgentEvent type, AgentArgs args)
	{
		AgentReceiveEventArgs e = (AgentReceiveEventArgs)(object)((args is AgentReceiveEventArgs) ? args : null);
		if (e.EventKind == 0L && e.ValueCount == 1)
		{
			AtkValue* atkValues = (AtkValue*)e.AtkValues;
			if (((AtkValue)atkValues).Int == 12)
			{
				args.PreventOriginal();
				Shutdown(base.TaskHelper);
			}
		}
	}

	private unsafe bool HandleMainCommandOperationDetour(AgentHUD* agent, MainCommandOperation operation, uint param1, int param2, byte* param3)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if ((int)operation == 1 && param2 == -1)
		{
			switch (param1)
			{
			case 23u:
				Logout(base.TaskHelper);
				return false;
			case 24u:
				Shutdown(base.TaskHelper);
				return false;
			}
		}
		return HandleMainCommandOperationHook.Original.Invoke(agent, operation, param1, param2, param3);
	}

	private unsafe void AgentCloseMessageShowDetour(AgentInterface* agent)
	{
		Shutdown(base.TaskHelper);
	}

	private void OnPreExecuteCommandInner(ref bool isPrevented, ref ReadOnlySeString message)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		string text = ((object)Unsafe.As<ReadOnlySeString, ReadOnlySeString>(ref message)/*cast due to constrained. prefix*/).ToString();
		if (!string.IsNullOrWhiteSpace(text) && text.StartsWith('/') && (CheckCommand(text, LogoutLine, base.TaskHelper, Logout) || CheckCommand(text, ShutdownLine, base.TaskHelper, Shutdown)))
		{
			isPrevented = true;
		}
	}

	private unsafe static void Logout(TaskHelper taskHelper)
	{
		taskHelper.Enqueue(() => ContentsFinderHelper.RequestDutyNormal(167u, ContentsFinderHelper.DefaultOption));
		taskHelper.Enqueue(delegate
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			LogoutParams val = new LogoutParams
			{
				Type = 0,
				Code = 10000
			};
			((LogoutCallbackInterface)(&((AgentLobby)AgentLobby.Instance()).LogoutCallbackInterface)).OnLogout(&val);
		});
		taskHelper.Enqueue(() => !GameState.IsLoggedIn);
	}

	private static void Shutdown(TaskHelper taskHelper)
	{
		Logout(taskHelper);
		taskHelper.Enqueue(delegate
		{
			if (GameState.IsLoggedIn)
			{
				return false;
			}
			OmenServiceBase<ChatManager>.Instance().SendMessage("/xlkill");
			return true;
		});
	}

	private static bool CheckCommand(string message, TextCommand command, TaskHelper taskHelper, Action<TaskHelper> action)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (message == ((object)((TextCommand)(ref command)).Command/*cast due to constrained. prefix*/).ToString() || message == ((object)((TextCommand)(ref command)).Alias/*cast due to constrained. prefix*/).ToString())
		{
			action(taskHelper);
			return true;
		}
		return false;
	}
}
