using System;
using System.CodeDom.Compiler;
using System.Text.RegularExpressions;
using System.Text.RegularExpressions.Generated;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Interop.Game.Models.Native;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.ModulesPublic;

public class AutoRefuseTrade : ModuleBase
{
	private class Config : ModuleConfig
	{
		public string ExtraCommands = string.Empty;

		public bool SendChat = true;

		public bool SendNotification = true;

		public uint DelayMS = 500u;
	}

	private Hook<AgentShowDelegate>? AgentTradeShowHook;

	private Hook<SendTradeRequest>? SendTradeRequestHook;

	private Config config;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoRefuseTradeTitle"),
		Description = LanguageManager.Get("AutoRefuseTradeDescription"),
		Category = ModuleCategory.System
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		NeedAuth = true
	};

	protected unsafe override void Init()
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		base.TaskHelper = new TaskHelper();
		AgentTradeShowHook = (*((AgentInterface)((AgentModule)AgentModule.Instance()).GetAgentByInternalId((AgentId)36)).VirtualTable).HookVFuncFromName<AgentInterfaceVirtualTable, AgentShowDelegate>("Show", (AgentShowDelegate)AgentTradeShowDetour);
		AgentTradeShowHook.Enable();
		SendTradeRequestHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromMemberFunction<SendTradeRequest>(typeof(MemberFunctionPointers), "SendTradeRequest", new SendTradeRequest(SendTradeRequestDetour));
		SendTradeRequestHook.Enable();
	}

	protected override void ConfigUI()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(5, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("Delay"));
		((ImU8String)(ref val)).AppendLiteral(" (ms)");
		if (ImGui.InputUInt(val, ref config.DelayMS, 0u, 0u, default(ImU8String), (ImGuiInputTextFlags)0))
		{
			config.DelayMS = Math.Max(0u, config.DelayMS);
		}
		if (ImGui.IsItemDeactivatedAfterEdit())
		{
			config.Save(this);
		}
		ImGui.NewLine();
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("SendChat")), ref config.SendChat))
		{
			config.Save(this);
		}
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("SendNotification")), ref config.SendNotification))
		{
			config.Save(this);
		}
		ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("AutoRefuseTrade-ExtraCommands")));
		ImGui.InputTextMultiline(ImU8String.op_Implicit("###ExtraCommandsInput"), ref config.ExtraCommands, 1024, Globals.ScaledVector2(300f, 200f), (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
		ImGuiOm.TooltipHover(config.ExtraCommands);
		if (ImGui.IsItemDeactivatedAfterEdit())
		{
			config.Save(this);
		}
	}

	private unsafe void SendTradeRequestDetour(InventoryManager* instance, uint entityID)
	{
		Throttler.Shared.Throttle("AutoRefuseTrade-Show", 3000u, reThrottle: true);
		SendTradeRequestHook.Original.Invoke(instance, entityID);
	}

	private unsafe void AgentTradeShowDetour(AgentInterface* agent)
	{
		if (Throttler.Shared.Check("AutoRefuseTrade-Show"))
		{
			base.TaskHelper.Abort();
			if (config.DelayMS != 0)
			{
				base.TaskHelper.DelayNext((int)config.DelayMS);
			}
			base.TaskHelper.Enqueue(delegate
			{
				((InventoryManager)InventoryManager.Instance()).RefuseTrade();
				NotifyTradeCancel();
			});
		}
		else
		{
			AgentTradeShowHook.Original(agent);
		}
	}

	private void NotifyTradeCancel()
	{
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		string text = LanguageManager.Get("AutoRefuseTrade-Notification");
		if (config.SendNotification)
		{
			OmenServiceBase<NotifyHelper>.Instance().NotificationInfo(text);
			NotifyHelper.Speak(text);
		}
		if (config.SendChat)
		{
			OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit($"{text}\n    ({LanguageManager.Get("Time")}: {OmenServiceBase<StandardTimeManager>.Instance().Now.ToShortTimeString()})"));
		}
		if (string.IsNullOrWhiteSpace(config.ExtraCommands))
		{
			return;
		}
		string[] array = config.ExtraCommands.Split('\n');
		foreach (string text2 in array)
		{
			string trimmed = text2.Trim();
			if (string.IsNullOrEmpty(trimmed))
			{
				continue;
			}
			if (trimmed.StartsWith("/wait ", StringComparison.OrdinalIgnoreCase))
			{
				if (int.TryParse(trimmed.Substring(6).Trim(), out var result) && result > 0)
				{
					base.TaskHelper.DelayNext(result);
				}
				continue;
			}
			Match match = WaitParamRegex().Match(trimmed);
			if (match.Success)
			{
				string command = match.Groups[1].Value.Trim();
				if (!string.IsNullOrEmpty(command))
				{
					base.TaskHelper.DelayNext(100);
					base.TaskHelper.Enqueue(delegate
					{
						OmenServiceBase<ChatManager>.Instance().SendMessage(command);
					});
				}
				if (int.TryParse(match.Groups[2].Value, out var result2) && result2 > 0)
				{
					base.TaskHelper.DelayNext(result2);
				}
			}
			else
			{
				base.TaskHelper.DelayNext(100);
				base.TaskHelper.Enqueue(delegate
				{
					OmenServiceBase<ChatManager>.Instance().SendMessage(trimmed);
				});
			}
		}
	}

	[GeneratedRegex("^(.*?)<wait\\.(\\d+)>\\s*$", RegexOptions.IgnoreCase, "zh-CN")]
	[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.42308")]
	private static Regex WaitParamRegex()
	{
		return _003CRegexGenerator_g_003EFDD7C65F98B3009022F7AC561E347A370A49D71D176A01AA07C89E66F21EBD08F__WaitParamRegex_4.Instance;
	}
}
