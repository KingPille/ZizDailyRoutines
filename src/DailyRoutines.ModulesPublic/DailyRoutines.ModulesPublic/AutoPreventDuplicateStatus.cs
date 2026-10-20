using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Game.Gui.Toast;
using Dalamud.Interface;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Excel.Sheets;
using Lumina.Text;
using Lumina.Text.ReadOnly;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Info.Lumina;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;

namespace DailyRoutines.ModulesPublic;

public class AutoPreventDuplicateStatus : ModuleBase
{
	private class Config : ModuleConfig
	{
		public Dictionary<uint, bool> EnabledActions = new Dictionary<uint, bool>();

		public float OverlapThreshold = 3.5f;

		public bool SendNotification = true;
	}

	private enum DetectType
	{
		Self,
		Member,
		Target
	}

	private sealed record DuplicatePreventInfo(DuplicateStatusInfo[] Statuses, DuplicateStatusInfo[]? SecondStatuses = null)
	{
		public bool ShouldPrevent(AutoPreventDuplicateStatus module, ulong gameObjectID)
		{
			DuplicateStatusInfo[] secondStatuses;
			if (SecondStatuses != null)
			{
				secondStatuses = SecondStatuses;
				foreach (DuplicateStatusInfo duplicateStatusInfo in secondStatuses)
				{
					if (!duplicateStatusInfo.IsReverse)
					{
						if (duplicateStatusInfo.HasStatus(module))
						{
							return false;
						}
					}
					else if (!duplicateStatusInfo.HasStatus(module))
					{
						return false;
					}
				}
			}
			secondStatuses = Statuses;
			foreach (DuplicateStatusInfo duplicateStatusInfo2 in secondStatuses)
			{
				if (!duplicateStatusInfo2.IsReverse)
				{
					if (duplicateStatusInfo2.HasStatus(module, gameObjectID))
					{
						return true;
					}
				}
				else if (!duplicateStatusInfo2.HasStatus(module, gameObjectID))
				{
					return true;
				}
			}
			return false;
		}
	}

	private sealed record DuplicateStatusInfo(uint StatusID, DetectType DetectType, bool IsReverse = false)
	{
		private bool IsPermanent
		{
			get
			{
				if (LuminaGetter.TryGetRow<Status>(StatusID, out Status item))
				{
					return ((Status)(ref item)).IsPermanent;
				}
				return false;
			}
		}

		public IDalamudTextureWrap? GetIcon()
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			if (LuminaGetter.TryGetRow<Status>(StatusID, out Status item))
			{
				ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
				GameIconLookup val = new GameIconLookup(((Status)(ref item)).Icon, false, true, (ClientLanguage?)null);
				return obj.GetFromGameIcon(ref val).GetWrapOrDefault((IDalamudTextureWrap)null);
			}
			return null;
		}

		public string? GetName()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			if (Sheets.Statuses.TryGetValue(StatusID, out var value))
			{
				return ((object)((Status)(ref value)).Name/*cast due to constrained. prefix*/).ToString();
			}
			return null;
		}

		public unsafe bool HasStatus(AutoPreventDuplicateStatus module)
		{
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Invalid comparison between Unknown and I4
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			switch (DetectType)
			{
			case DetectType.Self:
			{
				BattleChara* localPlayer = Control.GetLocalPlayer();
				if (localPlayer == null)
				{
					return false;
				}
				return HasStatus(module, ((BattleChara)localPlayer).GetStatusManager());
			}
			case DetectType.Target:
			{
				GameObject* target = ((TargetSystem)TargetSystem.Instance()).Target;
				if (target == null || !((GameObject)target).IsCharacter() || (int)((GameObject)target).ObjectKind != 2)
				{
					return false;
				}
				BattleChara* ptr = (BattleChara*)target;
				return HasStatus(module, ((BattleChara)ptr).GetStatusManager());
			}
			case DetectType.Member:
			{
				Span<HudPartyMember> partyMembers = ((AgentHUD)AgentHUD.Instance()).PartyMembers;
				for (int i = 0; i < partyMembers.Length; i++)
				{
					HudPartyMember val = partyMembers[i];
					if (val.Object != null && val.EntityId != LocalPlayerState.EntityID && HasStatus(module, ((BattleChara)val.Object).GetStatusManager()))
					{
						return true;
					}
				}
				return false;
			}
			default:
				return false;
			}
		}

		public unsafe bool HasStatus(AutoPreventDuplicateStatus module, ulong gameObjectID)
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
			if (localPlayer == null)
			{
				return false;
			}
			BattleChara* ptr = ((DetectType == DetectType.Self || gameObjectID == 3758096384u || gameObjectID == LocalPlayerState.EntityID) ? localPlayer.ToBCStruct() : ((BattleChara*)((ObjectArrays)(&((GameObjectManager)GameObjectManager.Instance()).Objects)).GetObjectByGameObjectId(GameObjectId.op_Implicit(gameObjectID))));
			if (ptr == null)
			{
				return false;
			}
			return HasStatus(module, ((BattleChara)ptr).GetStatusManager());
		}

		public unsafe bool HasStatus(AutoPreventDuplicateStatus module, StatusManager* statusManager)
		{
			if (statusManager == null)
			{
				return false;
			}
			int statusIndex = ((StatusManager)statusManager).GetStatusIndex(StatusID, 3758096384u);
			if (statusIndex == -1)
			{
				return false;
			}
			if (!IsPermanent)
			{
				return ((StatusManager)statusManager).Status[statusIndex].RemainingTime > module.config.OverlapThreshold;
			}
			return true;
		}
	}

	private Config config;

	private static readonly FrozenDictionary<DetectType, string> DetectTypeLoc = new Dictionary<DetectType, string>
	{
		[DetectType.Self] = LanguageManager.Get("AutoPreventDuplicateStatus-Self"),
		[DetectType.Member] = LanguageManager.Get("AutoPreventDuplicateStatus-Member"),
		[DetectType.Target] = LanguageManager.Get("Target")
	}.ToFrozenDictionary();

	private static readonly FrozenDictionary<uint, DuplicatePreventInfo> DuplicateActions = new Dictionary<uint, DuplicatePreventInfo>
	{
		[7549u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(1195u, DetectType.Target)
		}),
		[7560u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(1203u, DetectType.Target)
		}),
		[25857u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(2707u, DetectType.Self)
		}),
		[2887u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(860u, DetectType.Target)
		}),
		[16889u] = new DuplicatePreventInfo(new DuplicateStatusInfo[3]
		{
			new DuplicateStatusInfo(1951u, DetectType.Self),
			new DuplicateStatusInfo(1934u, DetectType.Self),
			new DuplicateStatusInfo(1826u, DetectType.Self)
		}),
		[16012u] = new DuplicatePreventInfo(new DuplicateStatusInfo[3]
		{
			new DuplicateStatusInfo(1951u, DetectType.Self),
			new DuplicateStatusInfo(1934u, DetectType.Self),
			new DuplicateStatusInfo(1826u, DetectType.Self)
		}),
		[7405u] = new DuplicatePreventInfo(new DuplicateStatusInfo[3]
		{
			new DuplicateStatusInfo(1951u, DetectType.Self),
			new DuplicateStatusInfo(1934u, DetectType.Self),
			new DuplicateStatusInfo(1826u, DetectType.Self)
		}),
		[7408u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(1202u, DetectType.Self)
		}),
		[7535u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(1193u, DetectType.Target)
		}),
		[7388u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(1457u, DetectType.Self)
		}),
		[3540u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(1362u, DetectType.Self)
		}),
		[7382u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(1174u, DetectType.Target)
		}),
		[25754u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(2682u, DetectType.Target)
		}),
		[7393u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(1178u, DetectType.Target)
		}),
		[16160u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(1839u, DetectType.Self)
		}),
		[25758u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(2683u, DetectType.Target)
		}),
		[16151u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(1835u, DetectType.Target)
		}),
		[7432u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(1218u, DetectType.Target)
		}),
		[25861u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(2708u, DetectType.Target)
		}),
		[7430u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(1217u, DetectType.Self)
		}),
		[25873u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(2717u, DetectType.Target)
		}),
		[188u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(1944u, DetectType.Self)
		}),
		[7863u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(2u, DetectType.Target)
		}),
		[7540u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(2u, DetectType.Target)
		}),
		[16u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(2u, DetectType.Target)
		}),
		[7546u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(1250u, DetectType.Self)
		}),
		[7548u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(2663u, DetectType.Self)
		}),
		[3557u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(786u, DetectType.Self)
		}),
		[83u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(116u, DetectType.Self)
		}),
		[69u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(110u, DetectType.Self)
		}),
		[7396u] = new DuplicatePreventInfo(new DuplicateStatusInfo[2]
		{
			new DuplicateStatusInfo(1182u, DetectType.Self),
			new DuplicateStatusInfo(1185u, DetectType.Self)
		}),
		[2248u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(638u, DetectType.Target)
		}),
		[7499u] = new DuplicatePreventInfo(new DuplicateStatusInfo[2]
		{
			new DuplicateStatusInfo(1233u, DetectType.Self),
			new DuplicateStatusInfo(3856u, DetectType.Self)
		}),
		[7421u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(1211u, DetectType.Self)
		}),
		[7518u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(1238u, DetectType.Self)
		}),
		[25801u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(2703u, DetectType.Self)
		}),
		[16508u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(304u, DetectType.Self)
		}),
		[16510u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(304u, DetectType.Self)
		}),
		[2876u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(851u, DetectType.Self)
		}),
		[34579u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(3643u, DetectType.Target)
		}),
		[34567u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(3712u, DetectType.Target)
		}),
		[118u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(141u, DetectType.Self)
		}),
		[7436u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(1221u, DetectType.Target)
		}),
		[16552u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(1878u, DetectType.Self)
		}),
		[3606u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(841u, DetectType.Self)
		}),
		[125u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(148u, DetectType.Target)
		}),
		[173u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(148u, DetectType.Target)
		}),
		[3603u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(148u, DetectType.Target)
		}),
		[24287u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(148u, DetectType.Target)
		}),
		[7523u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(148u, DetectType.Target)
		}),
		[18317u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(148u, DetectType.Target)
		}),
		[29057u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(1342u, DetectType.Self)
		}),
		[29234u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(3087u, DetectType.Target, IsReverse: true)
		}, new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(3089u, DetectType.Target)
		}),
		[3585u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(297u, DetectType.Target, IsReverse: true)
		}),
		[29415u] = new DuplicatePreventInfo(new DuplicateStatusInfo[3]
		{
			new DuplicateStatusInfo(3054u, DetectType.Target),
			new DuplicateStatusInfo(1302u, DetectType.Target),
			new DuplicateStatusInfo(3039u, DetectType.Target)
		}),
		[29228u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(3054u, DetectType.Target)
		}),
		[29081u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(3054u, DetectType.Target)
		}),
		[29258u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(3107u, DetectType.Self)
		}),
		[29264u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(2872u, DetectType.Target)
		}),
		[29395u] = new DuplicatePreventInfo(new DuplicateStatusInfo[2]
		{
			new DuplicateStatusInfo(3054u, DetectType.Target),
			new DuplicateStatusInfo(3248u, DetectType.Target)
		}),
		[29515u] = new DuplicatePreventInfo(new DuplicateStatusInfo[2]
		{
			new DuplicateStatusInfo(1302u, DetectType.Target),
			new DuplicateStatusInfo(3039u, DetectType.Target)
		}),
		[29414u] = new DuplicatePreventInfo(new DuplicateStatusInfo[1]
		{
			new DuplicateStatusInfo(3158u, DetectType.Self)
		})
	}.ToFrozenDictionary();

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoPreventDuplicateStatusTitle"),
		Description = LanguageManager.Get("AutoPreventDuplicateStatusDescription"),
		Category = ModuleCategory.Action
	};

	protected override void Init()
	{
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		DuplicateActions.Keys.Except(config.EnabledActions.Keys).ToList().ForEach(delegate(uint key)
		{
			config.EnabledActions[key] = true;
		});
		config.EnabledActions.Keys.Except(DuplicateActions.Keys).ToList().ForEach(delegate(uint key)
		{
			config.EnabledActions.Remove(key);
		});
		config.Save(this);
		OmenServiceBase<UseActionManager>.Instance().RegPreUseAction(OnPreUseAction);
	}

	protected override void Uninit()
	{
		OmenServiceBase<UseActionManager>.Instance().Unreg(OnPreUseAction);
	}

	protected override void ConfigUI()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		ImGui.AlignTextToFramePadding();
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(1, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("AutoPreventDuplicateStatus-OverlapThreshold"));
		((ImU8String)(ref val)).AppendLiteral(":");
		ImGui.TextColored(ref vector, val);
		ImGui.SameLine();
		ImGui.SetNextItemWidth(150f * Globals.GlobalUIScale);
		ImGui.InputFloat(ImU8String.op_Implicit("###OverlapThreshold"), ref config.OverlapThreshold, 0f, 0f, ImU8String.op_Implicit("%.1f"), (ImGuiInputTextFlags)0);
		if (ImGui.IsItemDeactivatedAfterEdit())
		{
			config.Save(this);
		}
		ImGuiOm.HelpMarker(LanguageManager.Get("AutoPreventDuplicateStatus-OverlapThresholdHelp"), 40f, (FontAwesomeIcon)61530);
		ImGui.AlignTextToFramePadding();
		vector = KnownColor.Chocolate.ToVector4();
		ImU8String val2 = default(ImU8String);
		((ImU8String)(ref val2))._002Ector(1, 1);
		((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get("SendNotification"));
		((ImU8String)(ref val2)).AppendLiteral(":");
		ImGui.TextColored(ref vector, val2);
		ImGui.SameLine();
		if (ImGui.Checkbox(ImU8String.op_Implicit("###SendNotification"), ref config.SendNotification))
		{
			config.Save(this);
		}
		ImGui.Spacing();
		ImU8String val3 = default(ImU8String);
		((ImU8String)(ref val3))._002Ector(5, 3);
		((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get("Action"));
		((ImU8String)(ref val3)).AppendLiteral(": ");
		((ImU8String)(ref val3)).AppendFormatted<int>(config.EnabledActions.Count((KeyValuePair<uint, bool> x) => x.Value));
		((ImU8String)(ref val3)).AppendLiteral(" / ");
		((ImU8String)(ref val3)).AppendFormatted<int>(config.EnabledActions.Count);
		TreeNodeDisposable val4 = ImRaii.TreeNode(val3);
		try
		{
			if (!val4)
			{
				return;
			}
			Vector2 vector2 = new Vector2(ImGui.GetContentRegionAvail().X - ImGui.GetTextLineHeightWithSpacing(), 0f);
			TableDisposable val5 = ImRaii.Table(ImU8String.op_Implicit("###ActionTable"), 3, (ImGuiTableFlags)1920, vector2);
			try
			{
				if (!val5)
				{
					return;
				}
				ImGui.TableSetupColumn(ImU8String.op_Implicit("名称"), (ImGuiTableColumnFlags)4, 30f, 0u);
				ImGui.TableSetupColumn(ImU8String.op_Implicit("一层状态"), (ImGuiTableColumnFlags)4, 30f, 0u);
				ImGui.TableSetupColumn(ImU8String.op_Implicit("二层状态"), (ImGuiTableColumnFlags)4, 30f, 0u);
				ImGui.TableNextRow((ImGuiTableRowFlags)1);
				ImGui.TableNextColumn();
				ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("Action")));
				ImGui.TableNextColumn();
				ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("AutoPreventDuplicateStatus-RelatedStatus")));
				ImGui.TableNextColumn();
				ImU8String val6 = new ImU8String(2, 1);
				((ImU8String)(ref val6)).AppendFormatted<string>(LanguageManager.Get("AutoPreventDuplicateStatus-RelatedStatus"));
				((ImU8String)(ref val6)).AppendLiteral(" 2");
				ImGui.TextUnformatted(val6);
				foreach (KeyValuePair<uint, DuplicatePreventInfo> duplicateAction in DuplicateActions)
				{
					if (!LuminaGetter.TryGetRow<Action>(duplicateAction.Key, out Action item))
					{
						continue;
					}
					ImGui.TableNextRow();
					ImGui.TableNextColumn();
					bool flag = config.EnabledActions[duplicateAction.Key];
					ImU8String val7 = new ImU8String(12, 1);
					((ImU8String)(ref val7)).AppendLiteral("###Is");
					((ImU8String)(ref val7)).AppendFormatted<uint>(duplicateAction.Key);
					((ImU8String)(ref val7)).AppendLiteral("Enabled");
					if (ImGui.Checkbox(val7, ref flag))
					{
						Dictionary<uint, bool> enabledActions = config.EnabledActions;
						uint key = duplicateAction.Key;
						enabledActions[key] = !enabledActions[key];
						config.Save(this);
					}
					ImGui.SameLine();
					ImGui.Spacing();
					ImGui.SameLine();
					ImGuiOm.TextImage(((object)((Action)(ref item)).Name/*cast due to constrained. prefix*/).ToString(), ImageHelper.GetGameIcon(((Action)(ref item)).Icon).Handle, Globals.ScaledVector2(20f));
					ImGui.TableNextColumn();
					ImGui.Spacing();
					DuplicateStatusInfo[] statuses = duplicateAction.Value.Statuses;
					for (int num = 0; num < statuses.Length; num++)
					{
						DrawDuplicateStatus(statuses[num]);
					}
					ImGui.TableNextColumn();
					if (duplicateAction.Value.SecondStatuses != null)
					{
						ImGui.Spacing();
						statuses = duplicateAction.Value.SecondStatuses;
						for (int num = 0; num < statuses.Length; num++)
						{
							DrawDuplicateStatus(statuses[num]);
						}
					}
				}
			}
			finally
			{
				((TableDisposable)(ref val5)).Dispose();
			}
		}
		finally
		{
			((TreeNodeDisposable)(ref val4)).Dispose();
		}
	}

	private static void DrawDuplicateStatus(DuplicateStatusInfo status)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		IDalamudTextureWrap icon = status.GetIcon();
		if (icon != null)
		{
			ImGui.SameLine();
			ImGui.Image(icon.Handle, new Vector2(ImGui.GetTextLineHeightWithSpacing()));
			ImGuiOm.TooltipHover($"{status.GetName()}\n{LanguageManager.Get("AutoPreventDuplicateStatus-DetectType")}: {DetectTypeLoc[status.DetectType]}");
		}
	}

	private unsafe void OnPreUseAction(ref bool isPrevented, ref ActionType actionType, ref uint actionID, ref ulong targetID, ref uint extraParam, ref UseActionMode queueState, ref uint comboRouteID)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Expected O, but got Unknown
		if ((int)actionType != 1)
		{
			return;
		}
		uint adjustedActionId = ((ActionManager)ActionManager.Instance()).GetAdjustedActionId(actionID);
		if (!DuplicateActions.TryGetValue(adjustedActionId, out DuplicatePreventInfo value) || !config.EnabledActions.TryGetValue(adjustedActionId, out var value2) || !value2 || ((ActionManager)ActionManager.Instance()).GetActionStatus(actionType, adjustedActionId, 3758096384uL, true, true, (uint*)null) != 0)
		{
			return;
		}
		Action? row = LuminaGetter.GetRow<Action>(adjustedActionId);
		if (!row.HasValue)
		{
			return;
		}
		Action value3 = row.Value;
		bool flag = ((Action)(ref value3)).CanTargetSelf;
		if (adjustedActionId == 7535)
		{
			flag = false;
		}
		IGameObject gameObject = IOmenDalamudService<IObjectTable>.Instance().SearchByID(targetID);
		ulong gameObjectID = targetID;
		if (flag && (gameObject == null || !ActionManager.CanUseActionOnTarget(adjustedActionId, gameObject.ToStruct())))
		{
			gameObjectID = LocalPlayerState.EntityID;
		}
		if (!value.ShouldPrevent(this, gameObjectID))
		{
			return;
		}
		if (config.SendNotification && Throttler.Shared.Throttle($"AutoPreventDuplicateStatus-Notification-{adjustedActionId}", 2500u))
		{
			RentedSeStringBuilder val = default(RentedSeStringBuilder);
			((RentedSeStringBuilder)(ref val))._002Ector();
			try
			{
				SeStringBuilder obj = ((RentedSeStringBuilder)(ref val)).Builder.PushEdgeColorType(32u);
				value3 = row.Value;
				obj.Append(((Action)(ref value3)).Name).PopEdgeColorType();
				ReadOnlySeString se = LanguageManager.GetSe("AutoPreventDuplicateStatus-Notification-Prevented", val);
				QuestToastOptions val2 = new QuestToastOptions();
				value3 = row.Value;
				val2.IconId = ((Action)(ref value3)).Icon;
				NotifyHelper.ToastQuest(se, val2);
			}
			finally
			{
				((IDisposable)(*(RentedSeStringBuilder*)(&val))/*cast due to constrained. prefix*/).Dispose();
			}
		}
		isPrevented = true;
	}
}
