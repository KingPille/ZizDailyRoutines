using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Hooking;
using Dalamud.Interface;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Memory;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using Lumina.Excel.Sheets;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Info.Game.Packets.Downstream;
using OmenTools.Info.Lumina;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules;

public class AutoBlockStatus : ModuleBase
{
	private unsafe delegate void glDaO6x9edRlh89RkNv(uint entityID, StatusEffectListUpdatePacket* packet, void* isReplayGroup, void* priority);

	private unsafe delegate void EhBmGhxf069pXqSYR1e(StatusManager* manager);

	private unsafe delegate void TOvkHOxCxMv3FwOTQAQ(StatusManager* manager, ushort statusID, ushort statusParam, ulong sourceID, ushort stackCount);

	private class er2e3mxj8rZ6mtyBwWO : ModuleConfig
	{
		public HashSet<uint> BlockedStatuses;

		public bool SendNotifications;

		public er2e3mxj8rZ6mtyBwWO()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			BlockedStatuses = new HashSet<uint>();
			SendNotifications = true;
			base._002Ector();
		}
	}

	private static readonly CompSig ProcessPacketStatusEffectListSig;

	private Hook<glDaO6x9edRlh89RkNv>? ProcessPacketStatusEffectListHook;

	private static readonly CompSig StatusManagerUpdateSig;

	private Hook<EhBmGhxf069pXqSYR1e>? StatusManagerUpdateHook;

	private static readonly CompSig StatusManagerOnLoseStatusSig;

	private TOvkHOxCxMv3FwOTQAQ? StatusManagerOnLoseStatus;

	private er2e3mxj8rZ6mtyBwWO config;

	private LuminaSearcher<Status>? statusSearcher;

	private string statusSearchInput;

	private string selectedStatusSearchInput;

	private static readonly FrozenSet<ConditionFlag> StatusFlags;

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected unsafe override void Init()
	{
		config = ModuleConfigExtension.Load<er2e3mxj8rZ6mtyBwWO>(this) ?? new er2e3mxj8rZ6mtyBwWO();
		if (statusSearcher == null)
		{
			statusSearcher = new LuminaSearcher<Status>(Sheets.Statuses.Values, new Func<Status, string>[3]
			{
				(Status x) => ((object)((Status)(ref x)).Name/*cast due to constrained. prefix*/).ToString(),
				(Status x) => ((Status)(ref x)).RowId.ToString(),
				(Status x) => ((object)((Status)(ref x)).Description/*cast due to constrained. prefix*/).ToString()
			});
		}
		if (StatusManagerOnLoseStatus == null)
		{
			StatusManagerOnLoseStatus = StatusManagerOnLoseStatusSig.GetDelegate<TOvkHOxCxMv3FwOTQAQ>();
		}
		if (ProcessPacketStatusEffectListHook == null)
		{
			ProcessPacketStatusEffectListHook = ProcessPacketStatusEffectListSig.GetHook<glDaO6x9edRlh89RkNv>(Uvu1KdIQbV);
		}
		ProcessPacketStatusEffectListHook.Enable();
		if (StatusManagerUpdateHook == null)
		{
			StatusManagerUpdateHook = StatusManagerUpdateSig.GetHook<EhBmGhxf069pXqSYR1e>(xfk1tgNWil);
		}
		StatusManagerUpdateHook.Enable();
	}

	protected override void ConfigUI()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_041a: Unknown result type (might be due to invalid IL or missing references)
		//IL_041f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0423: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_0438: Unknown result type (might be due to invalid IL or missing references)
		//IL_047b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0480: Unknown result type (might be due to invalid IL or missing references)
		//IL_049f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0526: Unknown result type (might be due to invalid IL or missing references)
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-1487420814 ^ -928630598 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5859f4ea3042459db2c9b610b3099912))), ref config.SendNotifications))
		{
			config.Save(this);
		}
		ImGui.Spacing();
		Vector2 contentRegionAvail = ImGui.GetContentRegionAvail();
		float x = Math.Max(200f * Globals.GlobalUIScale, (contentRegionAvail.X - 100f * Globals.GlobalUIScale) / 2f);
		Vector2 vector = new Vector2(x, 400f * Globals.GlobalUIScale);
		ChildDisposable val = ImRaii.Child(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6C3522DA ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88a2dcd1996b4a3fbefaf51a98b59def)), vector, true);
		try
		{
			if (ChildDisposable.op_Implicit(val))
			{
				ImGui.SetNextItemWidth(-1f);
				ImGui.InputTextWithHint(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x27D1AEDF ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_39d5d8025f5249e6b489bc374eefbb3c)), ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x9B4BCA2 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2f1c0cb46b4440d499a69ed8afe22360))), ref selectedStatusSearchInput, 100, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
				ImGui.Separator();
				foreach (uint blockedStatus in config.BlockedStatuses)
				{
					ImU8String val2 = new ImU8String(0, 1);
					((ImU8String)(ref val2)).AppendFormatted<uint>(blockedStatus);
					IdDisposable val3 = ImRaii.PushId(val2, true);
					try
					{
						if (!LuminaGetter.TryGetRow<Status>(blockedStatus, out Status item))
						{
							continue;
						}
						ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
						GameIconLookup val4 = new GameIconLookup(((Status)(ref item)).Icon, false, true, (ClientLanguage?)null);
						IDalamudTextureWrap wrapOrDefault = obj.GetFromGameIcon(ref val4).GetWrapOrDefault((IDalamudTextureWrap)null);
						if (wrapOrDefault != null && (string.IsNullOrWhiteSpace(selectedStatusSearchInput) || ((object)((Status)(ref item)).Name/*cast due to constrained. prefix*/).ToString().Contains(selectedStatusSearchInput, StringComparison.OrdinalIgnoreCase)))
						{
							if (ImGuiOm.SelectableImageWithText(wrapOrDefault.Handle, new Vector2(ImGui.GetTextLineHeightWithSpacing()), ((object)((Status)(ref item)).Name/*cast due to constrained. prefix*/).ToString(), selected: false, (ImGuiSelectableFlags)1))
							{
								config.BlockedStatuses.Remove(((Status)(ref item)).RowId);
								config.Save(this);
							}
							ImGuiOm.TooltipHover(wRT1OUqHAT(item));
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
			}
		}
		finally
		{
			((ChildDisposable)(ref val)).Dispose();
		}
		ImGui.SameLine();
		GroupDisposable val5 = ImRaii.Group();
		try
		{
			DisabledDisposable val6 = ImRaii.Disabled();
			try
			{
				ColorDisposable val7 = ImRaii.PushColor((ImGuiCol)21, new Vector4(0f), true);
				try
				{
					ImGui.SetCursorPosY(ImGui.GetCursorPosY() + vector.Y / 2f);
					ImGuiOm.ButtonIcon(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6E00B102 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c40146824d9043e0906f4e1889cf39aa), (FontAwesomeIcon)62306);
				}
				finally
				{
					((IDisposable)val7)?.Dispose();
				}
			}
			finally
			{
				((IDisposable)val6)?.Dispose();
			}
		}
		finally
		{
			((GroupDisposable)(ref val5)).Dispose();
		}
		ImGui.SameLine();
		ChildDisposable val8 = ImRaii.Child(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x45E427D7 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1904f95d2a394c929e1a6d4f72943ad7)), vector, true);
		try
		{
			if (!ChildDisposable.op_Implicit(val8))
			{
				return;
			}
			ImGui.SetNextItemWidth(-1f);
			if (ImGui.InputTextWithHint(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x109C603C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_44b9808560124617907f0048883b8948)), ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3D569428 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e9e296e241f44828a57c750102ce71c1))), ref statusSearchInput, 100, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null))
			{
				statusSearcher.Search(statusSearchInput);
			}
			ImGui.Separator();
			foreach (Status item2 in statusSearcher.SearchResult)
			{
				Status current2 = item2;
				ImU8String val9 = new ImU8String(0, 1);
				((ImU8String)(ref val9)).AppendFormatted<uint>(((Status)(ref current2)).RowId);
				IdDisposable val10 = ImRaii.PushId(val9, true);
				try
				{
					if (config.BlockedStatuses.Contains(((Status)(ref current2)).RowId))
					{
						continue;
					}
					ITextureProvider obj2 = DalamudServiceExtension.Instance<ITextureProvider>();
					GameIconLookup val4 = new GameIconLookup(((Status)(ref current2)).Icon, false, true, (ClientLanguage?)null);
					IDalamudTextureWrap wrapOrDefault2 = obj2.GetFromGameIcon(ref val4).GetWrapOrDefault((IDalamudTextureWrap)null);
					if (wrapOrDefault2 == null)
					{
						continue;
					}
					if (ImGuiOm.SelectableImageWithText(wrapOrDefault2.Handle, new Vector2(ImGui.GetTextLineHeightWithSpacing()), ((object)((Status)(ref current2)).Name/*cast due to constrained. prefix*/).ToString(), config.BlockedStatuses.Contains(((Status)(ref current2)).RowId), (ImGuiSelectableFlags)1))
					{
						if (!config.BlockedStatuses.Remove(((Status)(ref current2)).RowId))
						{
							config.BlockedStatuses.Add(((Status)(ref current2)).RowId);
						}
						config.Save(this);
					}
					ImGuiOm.TooltipHover(wRT1OUqHAT(current2));
				}
				finally
				{
					((IDisposable)val10)?.Dispose();
				}
			}
		}
		finally
		{
			((ChildDisposable)(ref val8)).Dispose();
		}
	}

	private static string wRT1OUqHAT(Status data)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		return LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x53518F78 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_9085260e6b1a40438d219e85182b5d08), ((Status)(ref data)).RowId, ((Status)(ref data)).LockActions, ((Status)(ref data)).LockMovement, ((Status)(ref data)).LockControl, ((object)((Status)(ref data)).Description/*cast due to constrained. prefix*/).ToString());
	}

	private unsafe void Uvu1KdIQbV(uint entityID, StatusEffectListUpdatePacket* packet, void* isReplayGroup, void* priority)
	{
		if (entityID != (IOmenDalamudService<IObjectTable>.Instance().LocalPlayer?.EntityID ?? 0))
		{
			ProcessPacketStatusEffectListHook.Original(entityID, packet, isReplayGroup, priority);
			return;
		}
		Span<StatusEffectListEntry> entries = packet->Entries;
		for (int i = 0; i < entries.Length; i++)
		{
			ref StatusEffectListEntry reference = ref entries[i];
			if (reference.StatusID != 0 && config.BlockedStatuses.Contains(reference.StatusID))
			{
				StatusEffectListEntry statusEffectListEntry = reference;
				reference = default(StatusEffectListEntry);
				drX1W7fQiu(statusEffectListEntry.StatusID, 0, statusEffectListEntry.SourceID, statusEffectListEntry.StackCount);
			}
		}
		ProcessPacketStatusEffectListHook.Original(entityID, packet, isReplayGroup, priority);
	}

	private unsafe void xfk1tgNWil(StatusManager* manager)
	{
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		if (((Character)((StatusManager)manager).Owner).EntityId != (IOmenDalamudService<IObjectTable>.Instance().LocalPlayer?.EntityID ?? 0))
		{
			StatusManagerUpdateHook.Original(manager);
			return;
		}
		Span<Status> status = ((StatusManager)manager).Status;
		for (int i = 0; i < status.Length; i++)
		{
			ref Status reference = ref status[i];
			if (reference.StatusId != 0 && config.BlockedStatuses.Contains(reference.StatusId))
			{
				Status val = reference;
				reference = default(Status);
				drX1W7fQiu(val.StatusId, val.Param, GameObjectId.op_Implicit(val.SourceObject), 0);
			}
		}
		StatusManagerUpdateHook.Original(manager);
	}

	private unsafe void drX1W7fQiu(ushort statusID, ushort statusParam, ulong sourceID = 0uL, ushort stackCount = 0)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
		if (localPlayer == null || !LuminaGetter.TryGetRow<Status>((uint)statusID, out Status item))
		{
			return;
		}
		StatusManagerOnLoseStatus(&((BattleChara)localPlayer.ToStruct()).StatusManager, statusID, statusParam, sourceID, stackCount);
		foreach (ConditionFlag statusFlag in StatusFlags)
		{
			MemoryHelper.Write<bool>((IntPtr)((nint)DalamudServiceExtension.Instance<ICondition>().Address + (nint)statusFlag), false);
		}
		if (config.SendNotifications)
		{
			Throttler<string> shared = Throttler.Shared;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(16, 1);
			defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6E00B04E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c40146824d9043e0906f4e1889cf39aa));
			defaultInterpolatedStringHandler.AppendFormatted(((Status)(ref item)).RowId);
			if (shared.Throttle(defaultInterpolatedStringHandler.ToStringAndClear(), 3000u))
			{
				OmenServiceBase<NotifyHelper>.Instance().NotificationInfo(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7C2F6B5C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_12c2d68ba4654622a4959ee73cacc6d7), ((object)((Status)(ref item)).Name/*cast due to constrained. prefix*/).ToString(), ((Status)(ref item)).RowId));
			}
		}
	}

	public AutoBlockStatus()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D751962 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8b51f8019ce49018b38e06104a97fc1)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6E00B3DA ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c40146824d9043e0906f4e1889cf39aa)),
			Category = ModuleCategory.Combat
		};
		Permission = new ModulePermission
		{
			NeedAuth = true,
			CNPremium = true,
			TCPremium = true
		};
		statusSearchInput = string.Empty;
		selectedStatusSearchInput = string.Empty;
		base._002Ector();
	}

	static AutoBlockStatus()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		ProcessPacketStatusEffectListSig = new CompSig(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-224923271 ^ -754736213 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cd1b319f326a44d1aaea645e95d8698b));
		StatusManagerUpdateSig = new CompSig(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-1487420814 ^ -821872405 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_23522a2a14314da28eb44187a09cd0b5));
		StatusManagerOnLoseStatusSig = new CompSig(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x591C0119 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_20724779fecb4a91959b8028d04d1118));
		StatusFlags = FrozenSet.Create(RuntimeHelpers.CreateSpan<ConditionFlag>((RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/));
	}
}
