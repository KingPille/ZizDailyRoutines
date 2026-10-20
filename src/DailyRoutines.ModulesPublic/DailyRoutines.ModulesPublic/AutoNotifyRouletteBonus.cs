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
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Hooking;
using Dalamud.Interface;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.ImGuiOm;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.ModulesPublic;

public class AutoNotifyRouletteBonus : ModuleBase
{
	private unsafe delegate void SetContentRouletteRoleBonusDelegate(AgentContentsFinder* instance, void* data, uint bonusIndex);

	private class Config : ModuleConfig
	{
		public Dictionary<uint, RouletteConfig> Roulettes = new Dictionary<uint, RouletteConfig>();

		public bool SendChat;

		public bool SendNotification;

		public bool SendTTS;
	}

	private class RouletteConfig
	{
		public bool DPS;

		public bool Healer;

		public bool OnlyIncomplete;

		public bool Tank;
	}

	private static readonly CompSig SetContentRouletteRoleBonusSig = new CompSig("48 89 4C 24 ?? 55 41 56 48 83 EC ?? ?? ?? ?? 4C 8B F1");

	private Hook<SetContentRouletteRoleBonusDelegate>? SetContentRouletteRoleBonusHook;

	private Config config;

	private ContentsRouletteRole[] lastKnownRoles = Array.Empty<ContentsRouletteRole>();

	private readonly Dictionary<uint, DalamudLinkPayload> rouletteLinkPayloads = new Dictionary<uint, DalamudLinkPayload>();

	private readonly Dictionary<uint, byte> rouletteLinkPayloadIDs = new Dictionary<uint, byte>();

	private const int ROULETTE_BONUS_ARRAY_SIZE = 11;

	private static readonly FrozenDictionary<byte, uint> RouletteIndexToRowID = new Dictionary<byte, uint>
	{
		[1] = 1u,
		[2] = 2u,
		[3] = 3u,
		[4] = 4u,
		[5] = 5u,
		[6] = 6u,
		[7] = 8u,
		[8] = 9u,
		[9] = 15u,
		[10] = 17u
	}.ToFrozenDictionary();

	private static readonly FrozenDictionary<ContentsRouletteRole, BitmapFontIcon> RoleIcons = new Dictionary<ContentsRouletteRole, BitmapFontIcon>
	{
		[(ContentsRouletteRole)0] = (BitmapFontIcon)82,
		[(ContentsRouletteRole)1] = (BitmapFontIcon)83,
		[(ContentsRouletteRole)2] = (BitmapFontIcon)84
	}.ToFrozenDictionary();

	private static readonly ContentRoulette[] CachedRoulettes = (from x in (IEnumerable<ContentRoulette>)LuminaGetter.Get<ContentRoulette>()
		where ((ContentRoulette)(ref x)).RowId != 0 && ((ContentRoulette)(ref x)).ContentRouletteRoleBonus.RowId != 0
		orderby ((ContentRoulette)(ref x)).ContentRouletteRoleBonus.RowId
		select x).ToArray();

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoNotifyRouletteBonusTitle"),
		Description = LanguageManager.Get("AutoNotifyRouletteBonusDescription"),
		Category = ModuleCategory.Recruitment,
		Author = new string[1] { "BoxingBunny" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected unsafe override void Init()
	{
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		if (base.TaskHelper == null)
		{
			TaskHelper taskHelper = (base.TaskHelper = new TaskHelper());
		}
		if (lastKnownRoles.Length != 11)
		{
			lastKnownRoles = (ContentsRouletteRole[])(object)new ContentsRouletteRole[11];
			Array.Fill(lastKnownRoles, (ContentsRouletteRole)3);
		}
		if (SetContentRouletteRoleBonusHook == null)
		{
			SetContentRouletteRoleBonusHook = SetContentRouletteRoleBonusSig.GetHook<SetContentRouletteRoleBonusDelegate>(SetContentRouletteRoleBonusDetour);
		}
		SetContentRouletteRoleBonusHook.Enable();
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += OnZoneChanged;
	}

	protected override void Uninit()
	{
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChanged;
		foreach (DalamudLinkPayload value in rouletteLinkPayloads.Values)
		{
			OmenServiceBase<LinkPayloadManager>.Instance().Unreg(value.CommandId);
		}
		rouletteLinkPayloads.Clear();
		rouletteLinkPayloadIDs.Clear();
	}

	protected override void ConfigUI()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_046d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0472: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_040b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("SendNotification")), ref config.SendNotification))
		{
			config.Save(this);
		}
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("SendChat")), ref config.SendChat))
		{
			config.Save(this);
		}
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("SendTTS")), ref config.SendTTS))
		{
			config.Save(this);
		}
		ImGui.NewLine();
		TableDisposable val = ImRaii.Table(ImU8String.op_Implicit("RouletteConfigTable"), 6, (ImGuiTableFlags)26560);
		try
		{
			if (!val)
			{
				return;
			}
			ImGui.TableSetupColumn(ImU8String.op_Implicit("##Roulette"), (ImGuiTableColumnFlags)0, 0.3f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("##Tank"), (ImGuiTableColumnFlags)0, 0.1f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("##Healer"), (ImGuiTableColumnFlags)0, 0.1f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("##Dps"), (ImGuiTableColumnFlags)0, 0.1f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("##OnlyIncomplete"), (ImGuiTableColumnFlags)131072, 0.1f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("##RoleBonus"), (ImGuiTableColumnFlags)131072, 0.1f, 0u);
			GetTextureInfo("ui/uld/ContentsFinder_hr1.tex", out IDalamudTextureWrap texture, out Vector2 invTexSize);
			GetTextureInfo("ui/uld/Journal_Detail_hr1.tex", out IDalamudTextureWrap texture2, out Vector2 invTexSize2);
			ImGui.TableNextRow((ImGuiTableRowFlags)1);
			ImGui.TableNextColumn();
			ImGui.TextUnformatted(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(8605u)));
			ImGui.TableNextColumn();
			ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
			GameIconLookup val2 = new GameIconLookup(62581u, false, true, (ClientLanguage?)null);
			ISharedImmediateTexture val3 = default(ISharedImmediateTexture);
			if (obj.TryGetFromGameIcon(ref val2, ref val3))
			{
				ImGui.Image(val3.GetWrapOrEmpty().Handle, new Vector2(ImGui.GetTextLineHeightWithSpacing()));
			}
			else
			{
				ImGui.TextUnformatted(ImU8String.op_Implicit("-"));
			}
			ImGui.TableNextColumn();
			ITextureProvider obj2 = DalamudServiceExtension.Instance<ITextureProvider>();
			val2 = new GameIconLookup(62582u, false, true, (ClientLanguage?)null);
			ISharedImmediateTexture val4 = default(ISharedImmediateTexture);
			if (obj2.TryGetFromGameIcon(ref val2, ref val4))
			{
				ImGui.Image(val4.GetWrapOrEmpty().Handle, new Vector2(ImGui.GetTextLineHeightWithSpacing()));
			}
			else
			{
				ImGui.TextUnformatted(ImU8String.op_Implicit("-"));
			}
			ImGui.TableNextColumn();
			ITextureProvider obj3 = DalamudServiceExtension.Instance<ITextureProvider>();
			val2 = new GameIconLookup(62583u, false, true, (ClientLanguage?)null);
			ISharedImmediateTexture val5 = default(ISharedImmediateTexture);
			if (obj3.TryGetFromGameIcon(ref val2, ref val5))
			{
				ImGui.Image(val5.GetWrapOrEmpty().Handle, new Vector2(ImGui.GetTextLineHeightWithSpacing()));
			}
			else
			{
				ImGui.TextUnformatted(ImU8String.op_Implicit("-"));
			}
			ImGui.TableNextColumn();
			ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("OnlyIncomplete")));
			ImGuiOm.HelpMarker(LanguageManager.Get("AutoNotifyRouletteBonus-OnlyIncompleteHelp"), 40f, (FontAwesomeIcon)61530);
			ImGui.TableNextColumn();
			DrawRoleBonusHeaderIcon(texture2, invTexSize2);
			ContentRoulette[] cachedRoulettes = CachedRoulettes;
			bool flag2 = default(bool);
			for (int i = 0; i < cachedRoulettes.Length; i++)
			{
				ContentRoulette val6 = cachedRoulettes[i];
				uint rowId = ((ContentRoulette)(ref val6)).RowId;
				if (!config.Roulettes.TryGetValue(rowId, out RouletteConfig value))
				{
					value = new RouletteConfig();
					config.Roulettes[rowId] = value;
					config.Save(this);
				}
				bool flag = value.Tank || value.Healer || value.DPS;
				ImGui.TableNextRow();
				ColorDisposable val7 = ImRaii.PushColor((ImGuiCol)0, KnownColor.DarkGray.ToVector4(), !flag);
				try
				{
					ImGui.TableNextColumn();
					ImGui.TextUnformatted(ImU8String.op_Implicit(((object)((ContentRoulette)(ref val6)).Name/*cast due to constrained. prefix*/).ToString()));
					for (int j = 0; j <= 2; j++)
					{
						ImGui.TableNextColumn();
						switch (j)
						{
						case 0:
							flag2 = value.Tank;
							break;
						case 1:
							flag2 = value.Healer;
							break;
						case 2:
							flag2 = value.DPS;
							break;
						default:
							global::_003CPrivateImplementationDetails_003E.ThrowSwitchExpressionException(j);
							break;
						}
						bool flag3 = flag2;
						ImU8String val8 = new ImU8String(8, 2);
						((ImU8String)(ref val8)).AppendLiteral("##Role_");
						((ImU8String)(ref val8)).AppendFormatted<uint>(rowId);
						((ImU8String)(ref val8)).AppendLiteral("_");
						((ImU8String)(ref val8)).AppendFormatted<int>(j);
						if (ImGui.Checkbox(val8, ref flag3))
						{
							switch (j)
							{
							case 0:
								value.Tank = flag3;
								break;
							case 1:
								value.Healer = flag3;
								break;
							case 2:
								value.DPS = flag3;
								break;
							}
							config.Save(this);
						}
					}
					ImGui.TableNextColumn();
					ImU8String val9 = new ImU8String(13, 1);
					((ImU8String)(ref val9)).AppendLiteral("##Incomplete_");
					((ImU8String)(ref val9)).AppendFormatted<uint>(rowId);
					if (ImGui.Checkbox(val9, ref value.OnlyIncomplete))
					{
						config.Save(this);
					}
					ImGui.TableNextColumn();
					uint rowId2 = ((ContentRoulette)(ref val6)).ContentRouletteRoleBonus.RowId;
					if (rowId2 != 0 && rowId2 < 11)
					{
						ContentsRouletteRole role = lastKnownRoles[rowId2];
						DrawRoleBonusCellIcon(texture, invTexSize, role);
					}
					else
					{
						ImGui.TextUnformatted(ImU8String.op_Implicit("-"));
					}
				}
				finally
				{
					((IDisposable)val7)?.Dispose();
				}
			}
		}
		finally
		{
			((TableDisposable)(ref val)).Dispose();
		}
	}

	private unsafe void SetContentRouletteRoleBonusDetour(AgentContentsFinder* instance, void* data, uint bonusIndex)
	{
		SetContentRouletteRoleBonusHook.Original(instance, data, bonusIndex);
		OnRoleBonusUpdated();
	}

	private unsafe void OnZoneChanged(uint u)
	{
		base.TaskHelper.Abort();
		base.TaskHelper.Enqueue(() => UIModuleExtension.IsScreenReady() && !IConditionExtension.get_IsBetweenAreas(DalamudServiceExtension.Instance<ICondition>()));
		base.TaskHelper.Enqueue(delegate
		{
			AgentContentsFinder* ptr = AgentContentsFinder.Instance();
			if (ptr == null)
			{
				return false;
			}
			((AgentContentsFinder)ptr).Refresh();
			return true;
		});
	}

	private unsafe void OnClickRouletteLinkPayload(uint id, SeString _)
	{
		if (rouletteLinkPayloadIDs.TryGetValue(id, out var value) && GameState.ContentFinderCondition == 0)
		{
			AgentContentsFinder* ptr = AgentContentsFinder.Instance();
			if (ptr != null)
			{
				((AgentContentsFinder)ptr).OpenRouletteDuty(value, false);
			}
		}
	}

	private unsafe void OnRoleBonusUpdated()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected I4, but got Unknown
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Invalid comparison between Unknown and I4
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Invalid comparison between Unknown and I4
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Expected I4, but got Unknown
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		if (!GameState.IsLoggedIn || GameState.ContentFinderCondition != 0)
		{
			return;
		}
		AgentContentsFinder* ptr = AgentContentsFinder.Instance();
		if (ptr == null)
		{
			return;
		}
		ContentsRouletteRole[] array = ((AgentContentsFinder)ptr).ContentRouletteRoleBonuses.ToArray();
		List<(uint, ContentsRouletteRole)> list = new List<(uint, ContentsRouletteRole)>();
		bool flag = default(bool);
		for (byte b = 1; b < 11; b++)
		{
			ContentsRouletteRole val = array[b];
			ContentsRouletteRole val2 = lastKnownRoles[b];
			if (val != val2)
			{
				lastKnownRoles[b] = (ContentsRouletteRole)(int)val;
				if ((int)val2 != 3 && RouletteIndexToRowID.TryGetValue(b, out var value) && config.Roulettes.TryGetValue(value, out RouletteConfig value2) && (int)val <= 2)
				{
					switch ((int)val)
					{
					case 0:
						flag = value2.Tank;
						break;
					case 1:
						flag = value2.Healer;
						break;
					case 2:
						flag = value2.DPS;
						break;
					default:
						global::_003CPrivateImplementationDetails_003E.ThrowSwitchExpressionException(val);
						break;
					}
					if (flag && (!value2.OnlyIncomplete || !IsRouletteComplete(value)))
					{
						list.Add((value, val));
					}
				}
			}
		}
		NotifyRoleBonuses(list);
	}

	private void NotifyRoleBonuses(List<(uint RowID, ContentsRouletteRole Role)> bonuses)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Invalid comparison between Unknown and I4
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Expected O, but got Unknown
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		if (bonuses.Count == 0)
		{
			return;
		}
		Dictionary<ContentsRouletteRole, List<(uint, string)>> dictionary = new Dictionary<ContentsRouletteRole, List<(uint, string)>>();
		foreach (var bonus in bonuses)
		{
			if (LuminaGetter.TryGetRow<ContentRoulette>(bonus.RowID, out ContentRoulette item) && (int)bonus.Role <= 2)
			{
				if (!dictionary.TryGetValue(bonus.Role, out var value))
				{
					value = new List<(uint, string)>();
					dictionary[bonus.Role] = value;
				}
				if (!value.Any<(uint, string)>(((uint RowID, string RouletteName) x) => x.RowID == bonus.RowID))
				{
					value.Add((bonus.RowID, ((object)((ContentRoulette)(ref item)).Name/*cast due to constrained. prefix*/).ToString()));
				}
			}
		}
		if (dictionary.Count == 0)
		{
			return;
		}
		ContentsRouletteRole key;
		List<(uint, string)> value2;
		if (config.SendChat)
		{
			SeStringBuilder val = new SeStringBuilder();
			val.AddText(LanguageManager.Get("AutoNotifyRouletteBonusTitle"));
			foreach (KeyValuePair<ContentsRouletteRole, List<(uint, string)>> item2 in dictionary)
			{
				item2.Deconstruct(out key, out value2);
				ContentsRouletteRole val2 = key;
				List<(uint, string)> list = value2;
				val.Add((Payload)(object)NewLinePayload.Payload);
				val.AddText(GetRoleBonusText(val2));
				if (RoleIcons.TryGetValue(val2, out var value3))
				{
					val.AddText(" ").AddIcon(value3);
				}
				val.Add((Payload)(object)NewLinePayload.Payload).AddText(LanguageManager.Get("Operation") + ": ");
				foreach (var (rowID, text) in list)
				{
					val.Add((Payload)(object)RawPayload.LinkTerminator).Add((Payload)(object)GetRouletteLinkPayload(rowID)).AddText("[")
						.AddUiForeground(text, (ushort)35)
						.AddText("]")
						.Add((Payload)(object)RawPayload.LinkTerminator);
				}
			}
			OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(val.Build().Encode()));
		}
		List<string> list2 = new List<string>();
		foreach (KeyValuePair<ContentsRouletteRole, List<(uint, string)>> item3 in dictionary)
		{
			item3.Deconstruct(out key, out value2);
			ContentsRouletteRole role = key;
			List<(uint, string)> source = value2;
			list2.Add(string.Concat(GetRoleBonusText(role), " ", string.Concat(source.Select<(uint, string), string>(((uint RowID, string RouletteName) x) => "[" + x.RouletteName + "]"))));
		}
		if (list2.Count != 0)
		{
			string message = string.Join('\n', list2);
			if (config.SendNotification)
			{
				OmenServiceBase<NotifyHelper>.Instance().NotificationInfo(message, LanguageManager.Get("AutoNotifyRouletteBonusTitle"));
			}
			if (config.SendTTS)
			{
				NotifyHelper.Speak(message);
			}
		}
	}

	private unsafe static bool IsRouletteComplete(uint rowID)
	{
		if (rowID > 255)
		{
			return false;
		}
		return ((InstanceContent)InstanceContent.Instance()).IsRouletteComplete((byte)rowID);
	}

	private static string GetRoleBonusText(ContentsRouletteRole role)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected I4, but got Unknown
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		uint rowID = default(uint);
		switch ((int)role)
		{
		case 0:
			rowID = 10997u;
			break;
		case 1:
			rowID = 10998u;
			break;
		case 2:
			rowID = 10999u;
			break;
		default:
			global::_003CPrivateImplementationDetails_003E.ThrowSwitchExpressionException(role);
			break;
		}
		return LuminaWrapper.GetAddonText(rowID);
	}

	private DalamudLinkPayload GetRouletteLinkPayload(uint rowID)
	{
		if (rouletteLinkPayloads.TryGetValue(rowID, out DalamudLinkPayload value))
		{
			return value;
		}
		uint id;
		DalamudLinkPayload val = OmenServiceBase<LinkPayloadManager>.Instance().Reg(OnClickRouletteLinkPayload, out id);
		rouletteLinkPayloads[rowID] = val;
		rouletteLinkPayloadIDs[id] = (byte)rowID;
		return val;
	}

	private static void DrawRoleBonusHeaderIcon(IDalamudTextureWrap? texture, Vector2 invTexSize)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (texture == null)
		{
			ImGui.TextUnformatted(ImU8String.op_Implicit("-"));
		}
		else
		{
			DrawIcon(texture, invTexSize, new Vector2(888f, 0f), new Vector2(56f, 56f));
		}
	}

	private static void DrawRoleBonusCellIcon(IDalamudTextureWrap? texture, Vector2 invTexSize, ContentsRouletteRole role)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Invalid comparison between Unknown and I4
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (texture == null || (int)role > 2)
		{
			ImGui.TextUnformatted(ImU8String.op_Implicit("-"));
		}
		else
		{
			DrawIcon(texture, invTexSize, new Vector2(40f * (float)role, 216f), new Vector2(40f, 40f));
		}
	}

	private static void DrawIcon(IDalamudTextureWrap texture, Vector2 invTexSize, Vector2 iconPosPx, Vector2 sizePx)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		Vector2 vector = iconPosPx * invTexSize;
		Vector2 vector2 = (iconPosPx + sizePx) * invTexSize;
		ImGui.Image(texture.Handle, new Vector2(ImGui.GetTextLineHeightWithSpacing()), vector, vector2);
	}

	private static void GetTextureInfo(string path, out IDalamudTextureWrap? texture, out Vector2 invTexSize)
	{
		texture = DalamudServiceExtension.Instance<ITextureProvider>().GetFromGame(path).GetWrapOrDefault((IDalamudTextureWrap)null);
		if (texture == null)
		{
			invTexSize = default(Vector2);
		}
		else
		{
			invTexSize = new Vector2(1f / (float)texture.Width, 1f / (float)texture.Height);
		}
	}
}
