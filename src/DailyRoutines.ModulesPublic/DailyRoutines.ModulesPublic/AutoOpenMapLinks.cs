using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Extensions;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Chat;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Info.Lumina;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class AutoOpenMapLinks : ModuleBase
{
	private class Config : ModuleConfig
	{
		public bool AutoOpenMap = true;

		public HashSet<XivChatType> WhitelistChannel = new HashSet<XivChatType>();

		public HashSet<string> WhitelistPlayer = new HashSet<string>();
	}

	private class AutoOpenMapLinksMenuItem(AutoOpenMapLinks module) : ContextMenuEntry
	{
		public override string Identifier => "AutoOpenMapLinks";

		public override ContextMenuItem? Create(ContextMenuOpenedArgs args)
		{
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			if (args.AddonName == null || ValidAddonNames.Contains(args.AddonName))
			{
				if (string.IsNullOrEmpty(args.TargetName) || !Sheets.Worlds.ContainsKey((uint)args.TargetHomeWorldID))
				{
					return null;
				}
				string player = args.TargetName + "@" + LuminaWrapper.GetWorldName((uint)args.TargetHomeWorldID);
				bool flag = module.config.WhitelistPlayer.Contains(player);
				return new ContextMenuItem
				{
					Name = ReadOnlySeString.op_Implicit(LanguageManager.Get((!flag) ? "AutoOpenMapLinks-ContextMenu-Subscribe" : "AutoOpenMapLinks-ContextMenu-Unsubscribe")),
					OnClicked = delegate
					{
						//IL_0016: Unknown result type (might be due to invalid IL or missing references)
						//IL_001c: Expected O, but got Unknown
						//IL_004d: Unknown result type (might be due to invalid IL or missing references)
						//IL_0052: Unknown result type (might be due to invalid IL or missing references)
						//IL_0058: Unknown result type (might be due to invalid IL or missing references)
						//IL_005f: Unknown result type (might be due to invalid IL or missing references)
						//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
						//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
						//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
						//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
						PlayerPayload val = new PlayerPayload(args.TargetName, (uint)args.TargetHomeWorldID);
						if (module.config.WhitelistPlayer.Add(player))
						{
							ReadOnlySeString se = LanguageManager.GetSe("AutoOpenMapLinks-Notification-PlayerAdded", val);
							OmenServiceBase<NotifyHelper>.Instance().Chat(se);
							NotifyHelper.Toast(se);
							module.config.Save(module);
						}
						else if (module.config.WhitelistPlayer.Remove(player))
						{
							ReadOnlySeString se2 = LanguageManager.GetSe("AutoOpenMapLinks-Notification-PlayerRemoved", val);
							OmenServiceBase<NotifyHelper>.Instance().Chat(se2);
							NotifyHelper.Toast(se2);
							module.config.Save(module);
						}
					}
				};
			}
			return null;
		}
	}

	private Config config;

	private AutoOpenMapLinksMenuItem autoOpenMapLinksItem;

	private static readonly FrozenSet<XivChatType> ValidChatTypes = FrozenSet.Create(new ReadOnlySpan<XivChatType>(XIVChatTypes.ChatTypeToAddonText.Keys.AsSpan().ToArray()));

	private static readonly FrozenSet<string> ValidAddonNames;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoOpenMapLinksTitle"),
		Description = LanguageManager.Get("AutoOpenMapLinksDescription"),
		Category = ModuleCategory.System,
		Author = new string[1] { "KirisameVanilla" }
	};

	protected override void Init()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		autoOpenMapLinksItem = new AutoOpenMapLinksMenuItem(this);
		DalamudServiceExtension.Instance<IChatGui>().ChatMessage += new OnHandleableChatMessageDelegate(HandleChatMessage);
		OmenServiceBase<ContextMenuManager>.Instance().Reg(autoOpenMapLinksItem);
	}

	protected override void Uninit()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IChatGui>().ChatMessage -= new OnHandleableChatMessageDelegate(HandleChatMessage);
		OmenServiceBase<ContextMenuManager>.Instance().Unreg(autoOpenMapLinksItem);
	}

	protected override void ConfigUI()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("AutoOpenMapLinks-AutoOpenMap")), ref config.AutoOpenMap))
		{
			config.Save(this);
		}
		ImGui.NewLine();
		using (ImRaiiExtension.Heading1(LanguageManager.Get("AutoOpenMapLinks-WhitelistChannels"), LanguageManager.Get("AutoOpenMapLinks-WhitelistChannels-Help")))
		{
			ComboDisposable val = ImRaii.Combo(ImU8String.op_Implicit("###WhitelistChannelCombo"), ImU8String.op_Implicit(LanguageManager.Get("AutoOpenMapLinks-AlreadyAddedChannelCount", config.WhitelistChannel.Count)), (ImGuiComboFlags)8);
			try
			{
				if (!ComboDisposable.op_Implicit(val))
				{
					return;
				}
				foreach (XivChatType validChatType in ValidChatTypes)
				{
					if (ImGui.Selectable(ImU8String.op_Implicit(XIVChatTypes.ChatTypeToAddonText.GetValueOrDefault(validChatType)), config.WhitelistChannel.Contains(validChatType), (ImGuiSelectableFlags)1, default(Vector2)))
					{
						if (!config.WhitelistChannel.Remove(validChatType))
						{
							config.WhitelistChannel.Add(validChatType);
						}
						config.Save(this);
					}
				}
			}
			finally
			{
				((ComboDisposable)(ref val)).Dispose();
			}
		}
	}

	private unsafe void HandleChatMessage(IHandleableChatMessage message)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		if (!ValidChatTypes.Contains(((IChatMessage)message).LogKind))
		{
			return;
		}
		MapLinkPayload val = ((IMutableChatMessage)message).Message.Payloads.OfType<MapLinkPayload>().FirstOrDefault();
		if (val == null)
		{
			return;
		}
		uint rowId = val.TerritoryType.RowId;
		uint rowId2 = val.Map.RowId;
		Vector3 vector = new Vector3((float)val.RawX / 1000f, 0f, (float)val.RawY / 1000f);
		if (config.WhitelistChannel.Contains(((IChatMessage)message).LogKind))
		{
			if (config.AutoOpenMap)
			{
				(*AgentMap.Instance()).SetMapFlagAndOpen(rowId2, vector);
			}
			else
			{
				((AgentMap)AgentMap.Instance()).SetFlagMapMarker(rowId, rowId2, vector, 60561u);
			}
		}
		else
		{
			if (config.WhitelistChannel.Count == 0 || ((IMutableChatMessage)message).Sender.Payloads.Count == 0)
			{
				return;
			}
			foreach (Payload payload in ((IMutableChatMessage)message).Sender.Payloads)
			{
				PlayerPayload val2 = (PlayerPayload)(object)((payload is PlayerPayload) ? payload : null);
				if (val2 == null)
				{
					continue;
				}
				string playerName = val2.PlayerName;
				World value = val2.World.Value;
				string item = playerName + "@" + ((object)((World)(ref value)).Name/*cast due to constrained. prefix*/).ToString();
				if (config.WhitelistPlayer.Contains(item))
				{
					if (config.AutoOpenMap)
					{
						(*AgentMap.Instance()).SetMapFlagAndOpen(rowId2, vector);
					}
					else
					{
						((AgentMap)AgentMap.Instance()).SetFlagMapMarker(rowId, rowId2, vector, 60561u);
					}
					break;
				}
			}
		}
	}

	static AutoOpenMapLinks()
	{
		InlineArray13<string> buffer = default(InlineArray13<string>);
		buffer[0] = "LookingForGroup";
		buffer[1] = "PartyMemberList";
		buffer[2] = "FriendList";
		buffer[3] = "FreeCompany";
		buffer[4] = "SocialList";
		buffer[5] = "ContactList";
		buffer[6] = "ChatLog";
		buffer[7] = "_PartyList";
		buffer[8] = "LinkShell";
		buffer[9] = "CrossWorldLinkshell";
		buffer[10] = "ContentMemberList";
		buffer[11] = "BeginnerChatList";
		buffer[12] = "CircleBook";
		ValidAddonNames = FrozenSet.Create<string>(buffer);
	}
}
