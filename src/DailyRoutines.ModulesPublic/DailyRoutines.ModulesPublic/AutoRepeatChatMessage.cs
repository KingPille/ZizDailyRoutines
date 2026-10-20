using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Internal;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Chat;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Shell;
using KamiToolKit.Classes;
using Lumina.Excel.Sheets;
using OmenTools.Extensions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class AutoRepeatChatMessage : ModuleBase
{
	private class Config : ModuleConfig
	{
		public bool AutoSwitchChannel = true;

		public bool AutoSwitchOrigChannel = true;

		public bool UseTrigger;
	}

	private Config config;

	private readonly Dictionary<uint, (int Channel, byte[] Message, string Sender)> savedPayload = new Dictionary<uint, (int, byte[], string)>();

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoRepeatChatMessageTitle"),
		Description = LanguageManager.Get("AutoRepeatChatMessageDescription", "\ue04e \ue090"),
		Category = ModuleCategory.System
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		NeedAuth = true
	};

	private static FrozenDictionary<XivChatType, int> ChatTypesToChannel { get; } = new Dictionary<XivChatType, int>
	{
		[(XivChatType)13] = 0,
		[(XivChatType)12] = 0,
		[(XivChatType)10] = 1,
		[(XivChatType)32] = 2,
		[(XivChatType)14] = 2,
		[(XivChatType)15] = 3,
		[(XivChatType)30] = 4,
		[(XivChatType)11] = 5,
		[(XivChatType)24] = 6,
		[(XivChatType)36] = 7,
		[(XivChatType)27] = 8,
		[(XivChatType)37] = 9,
		[(XivChatType)101] = 10,
		[(XivChatType)102] = 11,
		[(XivChatType)103] = 12,
		[(XivChatType)104] = 13,
		[(XivChatType)105] = 14,
		[(XivChatType)106] = 15,
		[(XivChatType)107] = 16,
		[(XivChatType)16] = 19,
		[(XivChatType)17] = 20,
		[(XivChatType)18] = 21,
		[(XivChatType)19] = 22,
		[(XivChatType)20] = 23,
		[(XivChatType)21] = 24,
		[(XivChatType)22] = 25,
		[(XivChatType)23] = 26
	}.ToFrozenDictionary();

	protected override void Init()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		DalamudServiceExtension.Instance<IChatGui>().ChatMessage += new OnHandleableChatMessageDelegate(OnChat);
	}

	protected override void Uninit()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IChatGui>().ChatMessage -= new OnHandleableChatMessageDelegate(OnChat);
		savedPayload.Clear();
	}

	protected override void ConfigUI()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("AutoRepeatChatMessage-AutoSwitchChannel")), ref config.AutoSwitchChannel))
		{
			config.Save(this);
		}
		if (config.AutoSwitchChannel)
		{
			Vector4 vector = KnownColor.Chocolate.ToVector4();
			ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("AutoRepeatChatMessage-ColorPreview")));
			IndentDisposable val = ImRaii.PushIndent(1, true);
			try
			{
				vector = ColorHelper.GetColor(34u);
				ImGui.TextColored(ref vector, ImU8String.op_Implicit("\ue04e \ue090"));
				ImGui.SameLine();
				ImU8String val2 = new ImU8String(2, 1);
				((ImU8String)(ref val2)).AppendLiteral(": ");
				((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get("AutoRepeatChatMessage-ColorAble"));
				ImGui.TextUnformatted(val2);
				vector = ColorHelper.GetColor(32u);
				ImGui.TextColored(ref vector, ImU8String.op_Implicit("\ue04e \ue090"));
				ImGui.SameLine();
				ImU8String val3 = new ImU8String(2, 1);
				((ImU8String)(ref val3)).AppendLiteral(": ");
				((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get("AutoRepeatChatMessage-ColorUnable"));
				ImGui.TextUnformatted(val3);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
			ImGui.Spacing();
			if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("AutoRepeatChatMessage-AutoSwitchOrigChannel")), ref config.AutoSwitchOrigChannel))
			{
				config.Save(this);
			}
		}
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("AutoRepeatChatMessage-UseTrigger")), ref config.UseTrigger))
		{
			config.Save(this);
		}
	}

	private void OnChat(IHandleableChatMessage message)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Expected O, but got Unknown
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Expected O, but got Unknown
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Expected O, but got Unknown
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Expected O, but got Unknown
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Expected O, but got Unknown
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Expected O, but got Unknown
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Expected O, but got Unknown
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Expected O, but got Unknown
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Expected O, but got Unknown
		if (((IChatMessage)message).IsHandled || !ChatTypesToChannel.TryGetValue(((IChatMessage)message).LogKind, out var value))
		{
			return;
		}
		string item = string.Empty;
		foreach (Payload payload in ((IMutableChatMessage)message).Sender.Payloads)
		{
			PlayerPayload val = (PlayerPayload)(object)((payload is PlayerPayload) ? payload : null);
			if (val != null)
			{
				string playerName = val.PlayerName;
				World value2 = val.World.Value;
				item = playerName + "@" + ((object)((World)(ref value2)).Name/*cast due to constrained. prefix*/).ToString();
			}
		}
		uint id;
		DalamudLinkPayload val2 = OmenServiceBase<LinkPayloadManager>.Instance().Reg(OnClickRepeat, out id);
		savedPayload.TryAdd(id, (value, ((IMutableChatMessage)message).Message.Encode(), item));
		((IMutableChatMessage)message).Message.Append((Payload)new UIForegroundPayload((ushort)24)).Append((Payload)new TextPayload(" [")).Append((Payload)new UIForegroundPayload((ushort)0))
			.Append((Payload)(object)RawPayload.LinkTerminator)
			.Append((Payload)(object)val2)
			.Append((Payload)new UIForegroundPayload((ushort)((value != -1) ? 34u : 32u)))
			.Append((Payload)new TextPayload("\ue04e \ue090"))
			.Append((Payload)new UIForegroundPayload((ushort)0))
			.Append((Payload)(object)RawPayload.LinkTerminator)
			.Append((Payload)new UIForegroundPayload((ushort)24))
			.Append((Payload)new TextPayload("]"))
			.Append((Payload)new UIForegroundPayload((ushort)0));
	}

	private unsafe void OnClickRepeat(uint id, SeString message)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Expected I4, but got Unknown
		if ((this.config.UseTrigger && !PluginConfig.Instance().ConflictKeyBinding.IsPressed()) || !savedPayload.TryGetValue(id, out (int, byte[], string) value))
		{
			return;
		}
		RaptureShellModule* ptr = RaptureShellModule.Instance();
		if (ptr == null)
		{
			return;
		}
		AgentChatLog* ptr2 = AgentChatLog.Instance();
		if (ptr2 == null)
		{
			return;
		}
		int num = (int)((AgentChatLog)ptr2).CurrentChannel;
		uint num2 = ChatChannelToLinkshellIndex((uint)num);
		uint num3 = ChatChannelToLinkshellIndex((uint)value.Item1);
		if (value.Item1 != -1 && this.config.AutoSwitchChannel)
		{
			if (value.Item1 == 0)
			{
				OmenServiceBase<ChatManager>.Instance().SendMessage("/tell " + value.Item3);
			}
			else
			{
				((RaptureShellModule)ptr).ChangeChatChannel(value.Item1, num3, Utf8String.FromString(string.Empty), true);
			}
		}
		OmenServiceBase<ChatManager>.Instance().SendCommand(value.Item2);
		if (value.Item1 != -1)
		{
			Config config = this.config;
			if (config != null && config.AutoSwitchChannel && config.AutoSwitchOrigChannel)
			{
				((RaptureShellModule)ptr).ChangeChatChannel(num, num2, Utf8String.FromString(string.Empty), false);
			}
		}
	}

	private static uint ChatChannelToLinkshellIndex(uint channel)
	{
		switch (channel)
		{
		case 9u:
		case 10u:
		case 11u:
		case 12u:
		case 13u:
		case 14u:
		case 15u:
		case 16u:
			return channel - 9;
		case 19u:
		case 20u:
		case 21u:
		case 22u:
		case 23u:
		case 24u:
		case 25u:
		case 26u:
			return channel - 19;
		default:
			return 0u;
		}
	}
}
