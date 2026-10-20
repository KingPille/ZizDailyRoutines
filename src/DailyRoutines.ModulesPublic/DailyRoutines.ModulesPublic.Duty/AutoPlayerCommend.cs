using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Party;
using Dalamud.Game.DutyState;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using FFXIVClientStructs.FFXIV.Component.GUI;
using InteropGenerator.Runtime;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm.Widgets.Combos;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.Helpers;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;

namespace DailyRoutines.ModulesPublic.Duty;

public class AutoPlayerCommend : ModuleBase
{
	private enum PlayerRole
	{
		Tank,
		Healer,
		MeleeDPS,
		RangedDPS,
		None
	}

	private class Config : ModuleConfig
	{
		public bool AutoIgnoreBlacklistPlayers = true;

		public HashSet<uint> BlacklistContents = new HashSet<uint>();
	}

	private sealed class AssignPlayerCommendationMenu(AutoPlayerCommend module) : ContextMenuEntry
	{
		public override string Identifier => "AutoPlayerCommend";

		public unsafe override ContextMenuItem? Create(ContextMenuOpenedArgs args)
		{
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			if (GameState.ContentFinderCondition == 0 || ((AgentHUD)AgentHUD.Instance()).PartyMemberCount < 2)
			{
				return null;
			}
			CharacterData* targetCharacter = args.TargetCharacter;
			if (targetCharacter == null || ((CharacterData)targetCharacter).ContentId == 0L)
			{
				return null;
			}
			ulong contentID = ((CharacterData)targetCharacter).ContentId;
			bool flag = contentID == LocalPlayerState.ContentID;
			if (module.assignedContentID == contentID)
			{
				return new ContextMenuItem
				{
					Name = ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoPlayerCommend-ContextMenu-Auto")),
					OnClicked = delegate
					{
						//IL_0023: Unknown result type (might be due to invalid IL or missing references)
						module.assignedContentID = 0uL;
						string text = LanguageManager.Get("AutoPlayerCommend-Notification-Auto");
						OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(text));
						NotifyHelper.Toast(text);
					}
				};
			}
			if (flag)
			{
				return new ContextMenuItem
				{
					Name = ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoPlayerCommend-ContextMenu-AssignNobody")),
					OnClicked = delegate
					{
						//IL_0026: Unknown result type (might be due to invalid IL or missing references)
						module.assignedContentID = LocalPlayerState.ContentID;
						string text = LanguageManager.Get("AutoPlayerCommend-Notification-AssignedNobody");
						OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(text));
						NotifyHelper.Toast(text);
					}
				};
			}
			return new ContextMenuItem
			{
				Name = ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoPlayerCommend-ContextMenu-AssignPlayer")),
				OnClicked = delegate
				{
					//IL_002e: Unknown result type (might be due to invalid IL or missing references)
					//IL_0033: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					//IL_0059: Unknown result type (might be due to invalid IL or missing references)
					//IL_005f: Expected O, but got Unknown
					//IL_005f: Unknown result type (might be due to invalid IL or missing references)
					//IL_0064: Unknown result type (might be due to invalid IL or missing references)
					//IL_006a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0071: Unknown result type (might be due to invalid IL or missing references)
					module.assignedContentID = contentID;
					ReadOnlySeString se = LanguageManager.GetSe("AutoPlayerCommend-Notification-AssignedPlayer", LuminaGetter.GetRowOrDefault<ClassJob>((uint)((CharacterData)targetCharacter).Job).ToBitmapFontIcon(), (object)new PlayerPayload(((CharacterData)targetCharacter).NameString, (uint)((CharacterData)targetCharacter).HomeWorld));
					OmenServiceBase<NotifyHelper>.Instance().Chat(se);
					NotifyHelper.Toast(se);
				}
			};
		}
	}

	private Config config;

	private AssignPlayerCommendationMenu menuItem;

	private ContentSelectCombo contentSelectCombo;

	private ulong assignedContentID;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoPlayerCommendTitle"),
		Description = LanguageManager.Get("AutoPlayerCommendDescription"),
		Category = ModuleCategory.Duty
	};

	private static uint MIPDisplayType
	{
		get
		{
			return DalamudServiceExtension.Instance<IGameConfig>().UiConfig.GetUInt("MipDispType");
		}
		set
		{
			DalamudServiceExtension.Instance<IGameConfig>().UiConfig.Set("MipDispType", value);
		}
	}

	protected override void Init()
	{
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Expected O, but got Unknown
		menuItem = new AssignPlayerCommendationMenu(this);
		contentSelectCombo = new ContentSelectCombo("Content");
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 10000
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		contentSelectCombo.SelectedIDs = config.BlacklistContents;
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += OnZoneChanged;
		DalamudServiceExtension.Instance<IDutyState>().DutyCompleted += new DutyCompletedDelegate(OnDutyComplete);
		OmenServiceBase<ContextMenuManager>.Instance().Reg(menuItem);
	}

	protected override void Uninit()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChanged;
		DalamudServiceExtension.Instance<IDutyState>().DutyCompleted -= new DutyCompletedDelegate(OnDutyComplete);
		OmenServiceBase<ContextMenuManager>.Instance().Unreg(menuItem);
		assignedContentID = 0uL;
	}

	protected override void ConfigUI()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(0, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("AutoPlayerCommend-BlacklistContents"));
		ImGui.TextColored(ref vector, val);
		IndentDisposable val2 = ImRaii.PushIndent(1, true);
		try
		{
			ImGui.SetNextItemWidth(300f * Globals.GlobalUIScale);
			if (contentSelectCombo.DrawCheckbox())
			{
				Config config = this.config;
				HashSet<uint> hashSet = new HashSet<uint>();
				foreach (uint selectedID in contentSelectCombo.SelectedIDs)
				{
					hashSet.Add(selectedID);
				}
				config.BlacklistContents = hashSet;
				this.config.Save(this);
			}
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
		ImGui.NewLine();
		ImU8String val3 = default(ImU8String);
		((ImU8String)(ref val3))._002Ector(0, 1);
		((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get("AutoPlayerCommend-BlockBlacklistPlayers"));
		if (ImGui.Checkbox(val3, ref this.config.AutoIgnoreBlacklistPlayers))
		{
			this.config.Save(this);
		}
	}

	private void OnZoneChanged(uint u)
	{
		assignedContentID = 0uL;
	}

	private void OnDutyComplete(IDutyStateEventArgs args)
	{
		if (!base.TaskHelper.AbortByConflictKey(this) && !config.BlacklistContents.Contains(GameState.ContentFinderCondition) && DalamudServiceExtension.Instance<IPartyList>().Length > 1)
		{
			uint orig = MIPDisplayType;
			base.TaskHelper.Enqueue(delegate
			{
				MIPDisplayType = 0u;
			}, "设置最优队员推荐不显示列表");
			base.TaskHelper.Enqueue((Func<bool>)OpenCommendWindow, "打开最优队员推荐列表", 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
			base.TaskHelper.Enqueue((Func<bool>)EnqueueCommendation, "给予最优队员推荐", 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
			base.TaskHelper.Enqueue(delegate
			{
				MIPDisplayType = orig;
			}, "还原原始最优队友推荐设置");
		}
	}

	private unsafe bool OpenCommendWindow()
	{
		AtkUnitBase* byName = AddonHelper.GetByName("_Notification");
		AtkUnitBase* byName2 = AddonHelper.GetByName("_NotificationIcMvp");
		if (byName == null && byName2 == null)
		{
			return true;
		}
		if (assignedContentID == LocalPlayerState.ContentID)
		{
			return true;
		}
		(*byName).Callback(0, 11);
		return true;
	}

	private unsafe bool EnqueueCommendation()
	{
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Expected O, but got Unknown
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		if (!(*Addons.VoteMvp).IsAddonAndNodesReady())
		{
			return false;
		}
		if (!((AgentInterface)((AgentModule)AgentModule.Instance()).GetAgentByInternalId((AgentId)131)).IsAgentActive())
		{
			return false;
		}
		if (assignedContentID == LocalPlayerState.ContentID)
		{
			return true;
		}
		if (Control.GetLocalPlayer() == null)
		{
			return false;
		}
		HudPartyMember[] source = ((AgentHUD)AgentHUD.Instance()).PartyMembers.ToArray();
		Dictionary<(string, uint, uint, uint, byte, PlayerRole, ulong), int> dictionary = new Dictionary<(string, uint, uint, uint, byte, PlayerRole, ulong), int>();
		ClassJob val;
		foreach (IPartyMember member in (IEnumerable<IPartyMember>)DalamudServiceExtension.Instance<IPartyList>())
		{
			if (member.ContentId != LocalPlayerState.ContentID)
			{
				int value = Math.Clamp(EnumerableExtension.IndexOf(source, (HudPartyMember x) => x.ContentId == member.ContentId) - 1, 0, 6);
				val = member.ClassJob.Value;
				byte role = ((ClassJob)(ref val)).Role;
				string? item = ((object)member.Name).ToString();
				uint rowId = member.World.RowId;
				uint rowId2 = member.ClassJob.RowId;
				val = member.ClassJob.Value;
				dictionary[(item, rowId, rowId2, ((ClassJob)(ref val)).ClassJobCategory.RowId, role, GetCharacterJobRole(role), member.ContentId)] = value;
			}
		}
		if (dictionary.Count == 0)
		{
			return true;
		}
		val = LocalPlayerState.ClassJobData;
		PlayerRole selfRole = GetCharacterJobRole(((ClassJob)(ref val)).Role);
		uint selfClassJob = LocalPlayerState.ClassJob;
		val = LocalPlayerState.ClassJobData;
		uint selfCategory = ((ClassJob)(ref val)).ClassJobCategory.RowId;
		Dictionary<uint, int> jobCounts = (from x in dictionary
			group x by x.Key.ClassJob).ToDictionary((IGrouping<uint, KeyValuePair<(string Name, uint HomeWorld, uint ClassJob, uint ClassJobCategory, byte RoleRaw, PlayerRole Role, ulong ContentID), int>> g) => g.Key, (IGrouping<uint, KeyValuePair<(string Name, uint HomeWorld, uint ClassJob, uint ClassJobCategory, byte RoleRaw, PlayerRole Role, ulong ContentID), int>> g) => g.Count());
		var list = Enumerable.Select(dictionary.Where<KeyValuePair<(string, uint, uint, uint, byte, PlayerRole, ulong), int>>((KeyValuePair<(string Name, uint HomeWorld, uint ClassJob, uint ClassJobCategory, byte RoleRaw, PlayerRole Role, ulong ContentID), int> x) => !config.AutoIgnoreBlacklistPlayers || (int)((InfoProxyBlacklist)InfoProxyBlacklist.Instance()).GetBlockResultType(x.Key.ContentID, 0uL) == 1).OrderByDescending<KeyValuePair<(string, uint, uint, uint, byte, PlayerRole, ulong), int>, int>(delegate(KeyValuePair<(string Name, uint HomeWorld, uint ClassJob, uint ClassJobCategory, byte RoleRaw, PlayerRole Role, ulong ContentID), int> x)
		{
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			if (assignedContentID != 0L && x.Key.ContentID == assignedContentID)
			{
				return 3;
			}
			if (selfClassJob == x.Key.ClassJob)
			{
				return 2;
			}
			PlayerRole playerRole = selfRole;
			bool flag = (uint)(playerRole - 2) <= 1u;
			bool flag2 = flag;
			if (flag2)
			{
				playerRole = x.Key.Role;
				bool flag3 = (uint)(playerRole - 2) <= 1u;
				flag2 = flag3;
			}
			if (flag2)
			{
				if (selfRole != x.Key.Role || selfCategory != x.Key.ClassJobCategory)
				{
					return 0;
				}
				return 1;
			}
			ClassJob classJobData = LocalPlayerState.ClassJobData;
			return (((ClassJob)(ref classJobData)).Role == x.Key.RoleRaw) ? 1 : 0;
		}).ThenByDescending<KeyValuePair<(string, uint, uint, uint, byte, PlayerRole, ulong), int>, int>(delegate(KeyValuePair<(string Name, uint HomeWorld, uint ClassJob, uint ClassJobCategory, byte RoleRaw, PlayerRole Role, ulong ContentID), int> x)
		{
			PlayerRole playerRole = selfRole;
			bool flag = (uint)(playerRole - 2) <= 1u;
			bool flag2 = flag;
			if (flag2)
			{
				playerRole = x.Key.Role;
				bool flag3 = (uint)(playerRole - 2) <= 1u;
				flag2 = flag3;
			}
			int value2;
			return (!flag2 || selfClassJob == x.Key.ClassJob || !jobCounts.TryGetValue(x.Key.ClassJob, out value2) || value2 < 2) ? 1 : 0;
		})
			.ThenByDescending<KeyValuePair<(string, uint, uint, uint, byte, PlayerRole, ulong), int>, int>(delegate(KeyValuePair<(string Name, uint HomeWorld, uint ClassJob, uint ClassJobCategory, byte RoleRaw, PlayerRole Role, ulong ContentID), int> x)
			{
				switch (selfRole)
				{
				case PlayerRole.Tank:
				case PlayerRole.Healer:
				{
					PlayerRole item3 = x.Key.Role;
					bool flag = (uint)item3 <= 1u;
					return flag ? 1 : 0;
				}
				case PlayerRole.MeleeDPS:
					return x.Key.Role switch
					{
						PlayerRole.MeleeDPS => 3, 
						PlayerRole.RangedDPS => 2, 
						PlayerRole.Healer => 1, 
						_ => 0, 
					};
				case PlayerRole.RangedDPS:
					return x.Key.Role switch
					{
						PlayerRole.RangedDPS => 3, 
						PlayerRole.MeleeDPS => 2, 
						PlayerRole.Healer => 1, 
						_ => 0, 
					};
				default:
					return 0;
				}
			}), (KeyValuePair<(string Name, uint HomeWorld, uint ClassJob, uint ClassJobCategory, byte RoleRaw, PlayerRole Role, ulong ContentID), int> x) => new
		{
			x.Key.Name,
			x.Key.ClassJob,
			x.Key.HomeWorld
		}).ToList();
		if (list.Count == 0)
		{
			return true;
		}
		foreach (var item4 in list)
		{
			if (TryFindPlayerIndex(item4.Name, item4.ClassJob, out var playerIndex) && LuminaGetter.TryGetRow<ClassJob>(item4.ClassJob, out ClassJob item2))
			{
				((AgentId)131).SendEvent(0uL, 0, playerIndex);
				ReadOnlySeString se = LanguageManager.GetSe("AutoPlayerCommend-Notification-Given", item2.ToBitmapFontIcon(), (object)new PlayerPayload(item4.Name, item4.HomeWorld));
				OmenServiceBase<NotifyHelper>.Instance().Chat(se);
				NotifyHelper.Toast(se);
				return true;
			}
		}
		string text = LanguageManager.Get("AutoPlayerCommend-Notification-ErrorWhenGave");
		OmenServiceBase<NotifyHelper>.Instance().ChatError(ReadOnlySeString.op_Implicit(text));
		NotifyHelper.ToastError(text);
		return true;
		unsafe static bool TryFindPlayerIndex(string playerName, uint playerJob, out int reference)
		{
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Invalid comparison between Unknown and I4
			reference = -1;
			uint uInt = ((AtkValue)((byte*)((AtkUnitBase)Addons.VoteMvp).AtkValues + Unsafe.SizeOf<AtkValue>())).UInt;
			for (int i = 0; i < uInt; i++)
			{
				if (((AtkValue)((byte*)((AtkUnitBase)Addons.VoteMvp).AtkValues + (nint)(16 + i) * (nint)Unsafe.SizeOf<AtkValue>())).UInt == 1)
				{
					AtkValue val2 = ((AtkValue*)((AtkUnitBase)Addons.VoteMvp).AtkValues)[9 + i];
					if ((int)val2.Type == 8 && ((CStringPointer)(ref val2.String)).HasValue)
					{
						string text2 = ((object)(*(CStringPointer*)(&val2.String))/*cast due to constrained. prefix*/).ToString();
						if (!string.IsNullOrWhiteSpace(text2) && !(text2 != playerName))
						{
							uint num = ((AtkValue)((byte*)((AtkUnitBase)Addons.VoteMvp).AtkValues + (nint)(2 + i) * (nint)Unsafe.SizeOf<AtkValue>())).UInt - 62100;
							if (num != 0 && num == playerJob)
							{
								reference = i;
								return true;
							}
						}
					}
				}
			}
			return false;
		}
	}

	private static PlayerRole GetCharacterJobRole(byte rawRole)
	{
		return rawRole switch
		{
			1 => PlayerRole.Tank, 
			2 => PlayerRole.MeleeDPS, 
			3 => PlayerRole.RangedDPS, 
			4 => PlayerRole.Healer, 
			_ => PlayerRole.None, 
		};
	}
}
