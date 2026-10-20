using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using DailyRoutines.Common.Interface.Windows;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Gui.Dtr;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Interface.ImGuiSeStringRenderer;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using Newtonsoft.Json;
using OmenTools;
using OmenTools.Dalamud;
using OmenTools.Dalamud.Services.Game.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;

namespace DailyRoutines.ModulesPublic;

public class AutoDisplayMitigationInfo : ModuleBase
{
	private readonly record struct MitigationDefinition(float Physical, float Magical, bool OnMember)
	{
		public MitigationValue Value => new MitigationValue(Physical, Magical);
	}

	private readonly record struct MitigationValue(float Physical, float Magical)
	{
		public static MitigationValue Empty { get; } = new MitigationValue(0f, 0f);
	}

	private struct MitigationFactors(float physical, float magical)
	{
		private float physical = physical;

		private float magical = magical;

		public static MitigationFactors Full => new MitigationFactors(1f, 1f);

		public void Apply(ReadOnlySpan<ActiveMitigation> statuses)
		{
			ReadOnlySpan<ActiveMitigation> readOnlySpan = statuses;
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				ActiveMitigation activeMitigation = readOnlySpan.get_Item(i);
				Apply(activeMitigation.Value);
			}
		}

		public void Apply(MitigationValue value)
		{
			if (value.Physical > 0f)
			{
				physical *= 1f - value.Physical / 100f;
			}
			if (value.Magical > 0f)
			{
				magical *= 1f - value.Magical / 100f;
			}
		}

		public MitigationValue ToReduction()
		{
			return new MitigationValue(ReductionFromFactor(physical), ReductionFromFactor(magical));
		}

		private static float ReductionFromFactor(float factor)
		{
			if (!(factor >= 1f))
			{
				return (1f - factor) * 100f;
			}
			return 0f;
		}
	}

	private readonly record struct ActiveMitigation(uint StatusID, float RemainingTime, MitigationValue Value)
	{
		public float Physical => Value.Physical;

		public float Magical => Value.Magical;
	}

	private readonly record struct PartyMitigationSnapshot(uint EntityID, MitigationValue Value, float Shield)
	{
		public float Physical => Value.Physical;

		public float Magical => Value.Magical;
	}

	private sealed record MitigationSnapshot(ActiveMitigation[] LocalStatuses, ActiveMitigation[] TargetStatuses, PartyMitigationSnapshot[] PartyMembers, MitigationValue LocalSummary, float LocalShield)
	{
		public static MitigationSnapshot Empty { get; } = new MitigationSnapshot(Array.Empty<ActiveMitigation>(), Array.Empty<ActiveMitigation>(), new PartyMitigationSnapshot[9], MitigationValue.Empty, 0f);

		public bool IsLocalEmpty
		{
			get
			{
				if (LocalStatuses.Length == 0 && TargetStatuses.Length == 0)
				{
					return LocalShield == 0f;
				}
				return false;
			}
		}
	}

	private sealed class MitigationState
	{
		private readonly struct MemberStatus(uint statusID, uint sourceID)
		{
			public uint StatusID { get; } = statusID;

			public uint SourceID { get; } = sourceID;

			public static MemberStatus From(Status s)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				return new MemberStatus(s.StatusId, s.SourceObject.ObjectId);
			}
		}

		private MitigationSnapshot current = MitigationSnapshot.Empty;

		public float LocalShield => current.LocalShield;

		public float LocalPhysical => current.LocalSummary.Physical;

		public float LocalMagical => current.LocalSummary.Magical;

		public bool IsLocalEmpty => current.IsLocalEmpty;

		public ReadOnlySpan<ActiveMitigation> LocalActiveStatus => current.LocalStatuses;

		public ReadOnlySpan<ActiveMitigation> TargetActiveStatus => current.TargetStatuses;

		public unsafe ReadOnlySpan<PartyMitigationSnapshot> PartySnapshot => current.PartyMembers.AsSpan(0, Math.Min(((AgentHUD)AgentHUD.Instance()).PartyMemberCount, current.PartyMembers.Length));

		public void Clear()
		{
			current = MitigationSnapshot.Empty;
		}

		public unsafe void Update()
		{
			FrozenDictionary<uint, MitigationDefinition> statusDefinitions = RemoteRepoManager.GetStatusDefinitions();
			BattleChara* localPlayer = Control.GetLocalPlayer();
			if (localPlayer == null)
			{
				Clear();
				return;
			}
			ActiveMitigation[] array = CollectLocalStatuses(localPlayer, statusDefinitions);
			ActiveMitigation[] array2 = CollectTargetStatuses(statusDefinitions);
			MitigationValue localSummary = CalculateSummary(array, array2);
			float localShield = CalculateShield(localPlayer);
			PartyMitigationSnapshot[] partyMembers = BuildPartySnapshot(localPlayer, localSummary, localShield, array2, statusDefinitions);
			current = new MitigationSnapshot(array, array2, partyMembers, localSummary, localShield);
		}

		private unsafe static ActiveMitigation[] CollectLocalStatuses(BattleChara* localPlayer, FrozenDictionary<uint, MitigationDefinition> definitions)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			List<ActiveMitigation> list = new List<ActiveMitigation>();
			Span<Status> status = ((StatusManager)(&((BattleChara)localPlayer).StatusManager)).Status;
			for (int i = 0; i < status.Length; i++)
			{
				Status val = status[i];
				if (val.StatusId != 0 && TryGetMitigationValue(((BattleChara)localPlayer).EntityId, MemberStatus.From(val), definitions, out var mitigation))
				{
					AddOrUpdateActiveMitigation(list, val.StatusId, val.RemainingTime, mitigation);
				}
			}
			return list.ToArray();
		}

		private unsafe static ActiveMitigation[] CollectTargetStatuses(FrozenDictionary<uint, MitigationDefinition> definitions)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			List<ActiveMitigation> list = new List<ActiveMitigation>();
			if (!(TargetManager.Target is IBattleNPC battleNPC))
			{
				return Array.Empty<ActiveMitigation>();
			}
			Span<Status> status = ((StatusManager)(&((BattleChara)battleNPC.ToBCStruct()).StatusManager)).Status;
			for (int i = 0; i < status.Length; i++)
			{
				Status val = status[i];
				if (val.StatusId != 0 && definitions.TryGetValue(val.StatusId, out var value))
				{
					AddOrUpdateActiveMitigation(list, val.StatusId, val.RemainingTime, value.Value);
				}
			}
			return list.ToArray();
		}

		private static MitigationValue CalculateSummary(ReadOnlySpan<ActiveMitigation> localStatuses, ReadOnlySpan<ActiveMitigation> targetStatuses)
		{
			MitigationFactors full = MitigationFactors.Full;
			full.Apply(localStatuses);
			full.Apply(targetStatuses);
			return full.ToReduction();
		}

		private unsafe static PartyMitigationSnapshot[] BuildPartySnapshot(BattleChara* localPlayer, MitigationValue localSummary, float localShield, ReadOnlySpan<ActiveMitigation> targetStatuses, FrozenDictionary<uint, MitigationDefinition> definitions)
		{
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			PartyMitigationSnapshot[] array = new PartyMitigationSnapshot[9]
			{
				new PartyMitigationSnapshot(((BattleChara)localPlayer).EntityId, localSummary, localShield),
				default(PartyMitigationSnapshot),
				default(PartyMitigationSnapshot),
				default(PartyMitigationSnapshot),
				default(PartyMitigationSnapshot),
				default(PartyMitigationSnapshot),
				default(PartyMitigationSnapshot),
				default(PartyMitigationSnapshot),
				default(PartyMitigationSnapshot)
			};
			int num = 1;
			MitigationFactors full = MitigationFactors.Full;
			full.Apply(targetStatuses);
			Span<HudPartyMember> partyMembers = ((AgentHUD)AgentHUD.Instance()).PartyMembers;
			for (int i = 0; i < partyMembers.Length; i++)
			{
				HudPartyMember val = partyMembers[i];
				if (val.Index < 0 || val.Index >= array.Length || val.Index == 0)
				{
					continue;
				}
				if (val.Index + 1 > num)
				{
					num = val.Index + 1;
				}
				uint entityId = val.EntityId;
				if (entityId == 0 || val.Object == null)
				{
					array[val.Index] = new PartyMitigationSnapshot(entityId, MitigationValue.Empty, 0f);
					continue;
				}
				MitigationFactors mitigationFactors = full;
				Span<Status> status = ((StatusManager)(&((BattleChara)val.Object).StatusManager)).Status;
				for (int j = 0; j < status.Length; j++)
				{
					Status val2 = status[j];
					if (val2.StatusId != 0 && TryGetMitigationValue(entityId, MemberStatus.From(val2), definitions, out var mitigation))
					{
						mitigationFactors.Apply(mitigation);
					}
				}
				array[val.Index] = new PartyMitigationSnapshot(entityId, mitigationFactors.ToReduction(), CalculateShield(val.Object));
			}
			return array[..num];
		}

		private static bool TryGetMitigationValue(uint targetID, MemberStatus memberStatus, FrozenDictionary<uint, MitigationDefinition> definitions, out MitigationValue mitigation)
		{
			mitigation = MitigationValue.Empty;
			uint statusID = memberStatus.StatusID;
			switch (statusID)
			{
			case 2675u:
			{
				float num2 = ((memberStatus.SourceID == targetID) ? 15f : 10f);
				mitigation = new MitigationValue(num2, num2);
				return true;
			}
			case 1174u:
			{
				if (!(IOmenDalamudService<IObjectTable>.Instance().SearchByID(targetID) is IBattleChara battleChara))
				{
					return false;
				}
				float num = 10f;
				foreach (IStatus status in battleChara.StatusList)
				{
					uint statusID2 = status.StatusID;
					if ((statusID2 == 1191 || statusID2 == 3829) ? true : false)
					{
						num = 20f;
						break;
					}
				}
				mitigation = new MitigationValue(num, num);
				return true;
			}
			default:
			{
				if (!definitions.TryGetValue(statusID, out var value))
				{
					return false;
				}
				mitigation = value.Value;
				return true;
			}
			}
		}

		private static void AddOrUpdateActiveMitigation(List<ActiveMitigation> statuses, uint statusID, float remainingTime, MitigationValue value)
		{
			for (int i = 0; i < statuses.Count; i++)
			{
				ActiveMitigation activeMitigation = statuses[i];
				if (activeMitigation.StatusID == statusID)
				{
					if (remainingTime > activeMitigation.RemainingTime)
					{
						statuses[i] = new ActiveMitigation(statusID, remainingTime, value);
					}
					return;
				}
			}
			statuses.Add(new ActiveMitigation(statusID, remainingTime, value));
		}

		private unsafe static float CalculateShield(BattleChara* chara)
		{
			return (float)(int)((BattleChara)chara).ShieldValue / 100f * (float)((BattleChara)chara).Health;
		}
	}

	private static class RemoteRepoManager
	{
		private const string URI = "hxxps://assets.sumemo.dev";

		private static FrozenDictionary<uint, MitigationDefinition> StatusDefinitions = FrozenDictionary<uint, MitigationDefinition>.Empty;

		public static FrozenDictionary<uint, MitigationDefinition> GetStatusDefinitions()
		{
			return Volatile.Read(in StatusDefinitions);
		}

		public static async Task FetchMitigationStatusesAsync(CancellationToken ct)
		{
			try
			{
				MitigationInfoDto[] array = JsonConvert.DeserializeObject<MitigationInfoDto[]>(await OmenServiceBase<HTTPClientHelper>.Instance().Get().GetStringAsync("https://assets.sumemo.dev/mitigation.json", ct)
					.ConfigureAwait(continueOnCapturedContext: false));
				if (array == null)
				{
					DLog.Error("[AutoDisplayMitigationInfo] 远程减伤技能文件解析失败");
					return;
				}
				Dictionary<uint, MitigationDefinition> dictionary = new Dictionary<uint, MitigationDefinition>(array.Length);
				MitigationInfoDto[] array2 = array;
				foreach (MitigationInfoDto mitigationInfoDto in array2)
				{
					if (mitigationInfoDto != null && mitigationInfoDto.ID != 0)
					{
						StatusInfoDto mitigation = mitigationInfoDto.Mitigation;
						dictionary[mitigationInfoDto.ID] = new MitigationDefinition(mitigation?.Physical ?? 0f, mitigation?.Magical ?? 0f, mitigationInfoDto.OnMember);
					}
				}
				Volatile.Write(ref StatusDefinitions, dictionary.ToFrozenDictionary());
			}
			catch (OperationCanceledException)
			{
			}
			catch (Exception value)
			{
				DLog.Error($"[AutoDisplayMitigationInfo] 远程减伤技能文件获取失败: {value}");
			}
		}
	}

	private class Config : ModuleConfig
	{
		public bool MoveableOverlay = true;

		public bool ResizeableOverlay = true;

		public bool TransparentOverlay;
	}

	private sealed class MitigationInfoDto
	{
		[JsonProperty("id")]
		public uint ID { get; private set; }

		[JsonProperty("mitigation")]
		public StatusInfoDto? Mitigation { get; private set; }

		[JsonProperty("on_member")]
		public bool OnMember { get; private set; }
	}

	private sealed class StatusInfoDto
	{
		[JsonProperty("physical")]
		public float Physical { get; private set; }

		[JsonProperty("magical")]
		public float Magical { get; private set; }
	}

	private Config config;

	private IDtrBarEntry? barEntry;

	private readonly MitigationState state = new MitigationState();

	private readonly CancellationTokenSource? remoteFetchCancelSource = new CancellationTokenSource();

	private bool isCombatEventsRegistered;

	private bool isNeedToDrawOnPartyList;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoDisplayMitigationInfoTitle"),
		Description = LanguageManager.Get("AutoDisplayMitigationInfoDescription"),
		Category = ModuleCategory.Combat,
		Author = new string[1] { "HaKu" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	private static byte[] DamagePhysicalString { get; } = new SeString((Payload[])(object)new Payload[1] { (Payload)new IconPayload((BitmapFontIcon)117) }).Encode();

	private static byte[] DamageMagicalString { get; } = new SeString((Payload[])(object)new Payload[1] { (Payload)new IconPayload((BitmapFontIcon)118) }).Encode();

	protected override void Init()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		RemoteRepoManager.FetchMitigationStatusesAsync(remoteFetchCancelSource.Token);
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += OnZoneChanged;
		DalamudServiceExtension.Instance<ICondition>().ConditionChange += new ConditionChangeDelegate(OnConditionChanged);
		if (GameState.ContentFinderCondition != 0)
		{
			OnZoneChanged(0u);
		}
		if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)26])
		{
			OnConditionChanged((ConditionFlag)26, value: true);
		}
	}

	protected override void Uninit()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		DalamudServiceExtension.Instance<ICondition>().ConditionChange -= new ConditionChangeDelegate(OnConditionChanged);
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChanged;
		UnregCombatEvents();
		IDtrBarEntry? obj = barEntry;
		if (obj != null)
		{
			obj.Remove();
		}
		barEntry = null;
		remoteFetchCancelSource?.Cancel();
		remoteFetchCancelSource?.Dispose();
	}

	protected override void ConfigUI()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("TransparentOverlay")), ref config.TransparentOverlay))
		{
			config.Save(this);
			if (config.TransparentOverlay)
			{
				Overlay? overlay = base.Overlay;
				((Window)overlay).Flags = (ImGuiWindowFlags)(((Window)overlay).Flags | 0x80);
				Overlay? overlay2 = base.Overlay;
				((Window)overlay2).Flags = (ImGuiWindowFlags)(((Window)overlay2).Flags | 1);
			}
			else
			{
				Overlay? overlay3 = base.Overlay;
				((Window)overlay3).Flags = (ImGuiWindowFlags)(((Window)overlay3).Flags & -129);
				Overlay? overlay4 = base.Overlay;
				((Window)overlay4).Flags = (ImGuiWindowFlags)(((Window)overlay4).Flags & -2);
			}
		}
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("ResizeableOverlay")), ref config.ResizeableOverlay))
		{
			config.Save(this);
			if (config.ResizeableOverlay)
			{
				Overlay? overlay5 = base.Overlay;
				((Window)overlay5).Flags = (ImGuiWindowFlags)(((Window)overlay5).Flags & -3);
			}
			else
			{
				Overlay? overlay6 = base.Overlay;
				((Window)overlay6).Flags = (ImGuiWindowFlags)(((Window)overlay6).Flags | 2);
			}
		}
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("MoveableOverlay")), ref config.MoveableOverlay))
		{
			config.Save(this);
			if (!config.MoveableOverlay)
			{
				Overlay? overlay7 = base.Overlay;
				((Window)overlay7).Flags = (ImGuiWindowFlags)(((Window)overlay7).Flags | 4);
				Overlay? overlay8 = base.Overlay;
				((Window)overlay8).Flags = (ImGuiWindowFlags)(((Window)overlay8).Flags | 0xC0200);
			}
			else
			{
				Overlay? overlay9 = base.Overlay;
				((Window)overlay9).Flags = (ImGuiWindowFlags)(((Window)overlay9).Flags & -5);
				Overlay? overlay10 = base.Overlay;
				((Window)overlay10).Flags = (ImGuiWindowFlags)(((Window)overlay10).Flags & -786945);
			}
		}
	}

	protected override void OverlayUI()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		if (state.IsLocalEmpty)
		{
			return;
		}
		IDtrBarEntry? obj = barEntry;
		object obj2;
		if (obj == null)
		{
			obj2 = null;
		}
		else
		{
			SeString text = obj.Text;
			obj2 = ((text != null) ? text.Encode() : null);
		}
		if (obj2 == null)
		{
			obj2 = Array.Empty<byte>();
		}
		ReadOnlySpan<byte> readOnlySpan = (byte[]?)obj2;
		SeStringDrawParams val = default(SeStringDrawParams);
		ImGuiHelpers.SeStringWrapped(readOnlySpan, ref val, default(ImGuiId), (ImGuiButtonFlags)1);
		ImGui.Separator();
		TableDisposable val2 = ImRaii.Table(ImU8String.op_Implicit("StatusTable"), 3);
		try
		{
			if (!val2)
			{
				return;
			}
			ImGui.TableSetupColumn(ImU8String.op_Implicit("Icon"), (ImGuiTableColumnFlags)8, 24f * Globals.GlobalUIScale, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("Name"), (ImGuiTableColumnFlags)4, 20f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("Value"), (ImGuiTableColumnFlags)4, 20f, 0u);
			ITextureProvider obj3 = DalamudServiceExtension.Instance<ITextureProvider>();
			GameIconLookup val3 = new GameIconLookup(210405u, false, true, (ClientLanguage?)null);
			ISharedImmediateTexture val4 = default(ISharedImmediateTexture);
			if (!obj3.TryGetFromGameIcon(ref val3, ref val4))
			{
				return;
			}
			ReadOnlySpan<ActiveMitigation> localActiveStatus = state.LocalActiveStatus;
			for (int i = 0; i < localActiveStatus.Length; i++)
			{
				DrawStatusRow(localActiveStatus.get_Item(i));
			}
			ReadOnlySpan<ActiveMitigation> targetActiveStatus = state.TargetActiveStatus;
			for (int i = 0; i < targetActiveStatus.Length; i++)
			{
				DrawStatusRow(targetActiveStatus.get_Item(i));
			}
			if (state.LocalShield > 0f)
			{
				if (!state.IsLocalEmpty)
				{
					ImGui.TableNextRow();
				}
				ImGui.TableNextRow();
				ImGui.TableNextColumn();
				ImGui.Image(val4.GetWrapOrEmpty().Handle, Globals.ScaledVector2(24f));
				ImGui.TableNextColumn();
				ImGui.AlignTextToFramePadding();
				ImU8String val5 = new ImU8String(0, 1);
				((ImU8String)(ref val5)).AppendFormatted<string>(LanguageManager.Get("Shield"));
				ImGui.TextUnformatted(val5);
				ImGui.TableNextColumn();
				ImU8String val6 = new ImU8String(0, 1);
				((ImU8String)(ref val6)).AppendFormatted<float>(state.LocalShield);
				ImGui.TextUnformatted(val6);
			}
		}
		finally
		{
			((TableDisposable)(ref val2)).Dispose();
		}
	}

	private void SetOverlay()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		if (base.Overlay == null)
		{
			Overlay overlay = (base.Overlay = new Overlay(this));
		}
		((Window)base.Overlay).WindowName = LanguageManager.Get("AutoDisplayMitigationInfoTitle");
		Overlay? overlay3 = base.Overlay;
		((Window)overlay3).Flags = (ImGuiWindowFlags)(((Window)overlay3).Flags & -2);
		Overlay? overlay4 = base.Overlay;
		((Window)overlay4).Flags = (ImGuiWindowFlags)(((Window)overlay4).Flags & -65);
		if (config.TransparentOverlay)
		{
			Overlay? overlay5 = base.Overlay;
			((Window)overlay5).Flags = (ImGuiWindowFlags)(((Window)overlay5).Flags | 0x80);
			Overlay? overlay6 = base.Overlay;
			((Window)overlay6).Flags = (ImGuiWindowFlags)(((Window)overlay6).Flags | 1);
		}
		else
		{
			Overlay? overlay7 = base.Overlay;
			((Window)overlay7).Flags = (ImGuiWindowFlags)(((Window)overlay7).Flags & -129);
			Overlay? overlay8 = base.Overlay;
			((Window)overlay8).Flags = (ImGuiWindowFlags)(((Window)overlay8).Flags & -2);
		}
		if (config.ResizeableOverlay)
		{
			Overlay? overlay9 = base.Overlay;
			((Window)overlay9).Flags = (ImGuiWindowFlags)(((Window)overlay9).Flags & -3);
		}
		else
		{
			Overlay? overlay10 = base.Overlay;
			((Window)overlay10).Flags = (ImGuiWindowFlags)(((Window)overlay10).Flags | 2);
		}
		if (!config.MoveableOverlay)
		{
			Overlay? overlay11 = base.Overlay;
			((Window)overlay11).Flags = (ImGuiWindowFlags)(((Window)overlay11).Flags | 4);
			Overlay? overlay12 = base.Overlay;
			((Window)overlay12).Flags = (ImGuiWindowFlags)(((Window)overlay12).Flags | 0xC0200);
		}
		else
		{
			Overlay? overlay13 = base.Overlay;
			((Window)overlay13).Flags = (ImGuiWindowFlags)(((Window)overlay13).Flags & -5);
			Overlay? overlay14 = base.Overlay;
			((Window)overlay14).Flags = (ImGuiWindowFlags)(((Window)overlay14).Flags & -786945);
		}
	}

	private static void DrawStatusRow(ActiveMitigation status)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		if (LuminaGetter.TryGetRow<Status>(status.StatusID, out Status item))
		{
			ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
			GameIconLookup val = new GameIconLookup(((Status)(ref item)).Icon, false, true, (ClientLanguage?)null);
			ISharedImmediateTexture val2 = default(ISharedImmediateTexture);
			if (obj.TryGetFromGameIcon(ref val, ref val2))
			{
				ImGui.TableNextRow();
				ImGui.TableNextColumn();
				ImGui.Image(val2.GetWrapOrEmpty().Handle, Globals.ScaledVector2(24f));
				ImGui.TableNextColumn();
				ImGui.AlignTextToFramePadding();
				ImU8String val3 = default(ImU8String);
				((ImU8String)(ref val3))._002Ector(4, 2);
				((ImU8String)(ref val3)).AppendFormatted<ReadOnlySeString>(((Status)(ref item)).Name);
				((ImU8String)(ref val3)).AppendLiteral(" (");
				((ImU8String)(ref val3)).AppendFormatted<float>(status.RemainingTime, "F1");
				((ImU8String)(ref val3)).AppendLiteral("s)");
				ImGui.TextUnformatted(val3);
				ImGuiOm.TooltipHover($"{status.StatusID}");
				ImGui.TableNextColumn();
				ReadOnlySpan<byte> readOnlySpan = DamagePhysicalString;
				SeStringDrawParams val4 = default(SeStringDrawParams);
				ImGuiHelpers.SeStringWrapped(readOnlySpan, ref val4, default(ImGuiId), (ImGuiButtonFlags)1);
				ImGui.SameLine();
				ImU8String val5 = default(ImU8String);
				((ImU8String)(ref val5))._002Ector(2, 1);
				((ImU8String)(ref val5)).AppendFormatted<float>(status.Physical);
				((ImU8String)(ref val5)).AppendLiteral("% ");
				ImGui.TextUnformatted(val5);
				ImGui.SameLine();
				ReadOnlySpan<byte> readOnlySpan2 = DamageMagicalString;
				val4 = default(SeStringDrawParams);
				ImGuiHelpers.SeStringWrapped(readOnlySpan2, ref val4, default(ImGuiId), (ImGuiButtonFlags)1);
				ImGui.SameLine();
				ImU8String val6 = default(ImU8String);
				((ImU8String)(ref val6))._002Ector(2, 1);
				((ImU8String)(ref val6)).AppendFormatted<float>(status.Magical);
				((ImU8String)(ref val6)).AppendLiteral("% ");
				ImGui.TextUnformatted(val6);
			}
		}
	}

	private void RegCombatEvents()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Expected O, but got Unknown
		if (isCombatEventsRegistered)
		{
			return;
		}
		OmenServiceBase<WindowManager>.Instance().PostDraw += Draw;
		OmenServiceBase<FrameworkManager>.Instance().Reg(new OnUpdateDelegate(OnUpdate), 500u);
		if (barEntry == null)
		{
			barEntry = DalamudServiceExtension.Instance<IDtrBar>().Get("DailyRoutines-AutoDisplayMitigationInfo", (SeString)null);
		}
		barEntry.OnClick = delegate
		{
			if (base.Overlay == null)
			{
				SetOverlay();
			}
			Overlay? overlay = base.Overlay;
			((Window)overlay).IsOpen = !((Window)overlay).IsOpen;
		};
		isCombatEventsRegistered = true;
	}

	private void UnregCombatEvents()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		if (isCombatEventsRegistered)
		{
			OmenServiceBase<WindowManager>.Instance().PostDraw -= Draw;
			OmenServiceBase<FrameworkManager>.Instance().Unreg(OnUpdate);
			state.Clear();
			if (barEntry != null)
			{
				barEntry.Shown = false;
				barEntry.Tooltip = null;
				barEntry.Text = null;
			}
			isCombatEventsRegistered = false;
		}
	}

	private void UpdateStatusBar()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected O, but got Unknown
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Expected O, but got Unknown
		if (barEntry == null)
		{
			return;
		}
		if (state.IsLocalEmpty)
		{
			barEntry.Shown = false;
			barEntry.Tooltip = null;
			barEntry.Text = null;
			return;
		}
		SeStringBuilder builder = new SeStringBuilder();
		bool first = true;
		AppendSummary(ref builder, ref first, (BitmapFontIcon)117, state.LocalPhysical, suffixPercent: true);
		AppendSummary(ref builder, ref first, (BitmapFontIcon)118, state.LocalMagical, suffixPercent: true);
		AppendSummary(ref builder, ref first, (BitmapFontIcon)82, state.LocalShield, suffixPercent: false);
		barEntry.Text = builder.Build();
		SeStringBuilder val = new SeStringBuilder();
		bool flag = true;
		ReadOnlySpan<ActiveMitigation> localActiveStatus = state.LocalActiveStatus;
		for (int i = 0; i < localActiveStatus.Length; i++)
		{
			ActiveMitigation activeMitigation = localActiveStatus.get_Item(i);
			if (!flag)
			{
				val.Append("\n");
			}
			val.Append(LuminaWrapper.GetStatusName(activeMitigation.StatusID) + ":");
			val.AddIcon((BitmapFontIcon)117);
			val.Append($"{activeMitigation.Physical}% ");
			val.AddIcon((BitmapFontIcon)118);
			val.Append($"{activeMitigation.Magical}% ");
			flag = false;
		}
		if (state.LocalShield > 0f)
		{
			if (!flag)
			{
				val.Append("\n");
			}
			val.AddIcon((BitmapFontIcon)82);
			val.Append($"{LanguageManager.Get("Shield")}: {state.LocalShield}");
			flag = false;
		}
		ReadOnlySpan<ActiveMitigation> targetActiveStatus = state.TargetActiveStatus;
		for (int i = 0; i < targetActiveStatus.Length; i++)
		{
			ActiveMitigation activeMitigation2 = targetActiveStatus.get_Item(i);
			if (!flag)
			{
				val.Append("\n");
			}
			val.Append(LuminaWrapper.GetStatusName(activeMitigation2.StatusID) + ":");
			val.AddIcon((BitmapFontIcon)117);
			val.Append($"{activeMitigation2.Physical}% ");
			val.AddIcon((BitmapFontIcon)118);
			val.Append($"{activeMitigation2.Magical}% ");
			flag = false;
		}
		barEntry.Tooltip = val.Build();
		barEntry.Shown = true;
		static void AppendSummary(ref SeStringBuilder reference2, ref bool reference, BitmapFontIcon icon, float value, bool suffixPercent)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			if (!(value <= 0f))
			{
				if (!reference)
				{
					reference2.Append(" ");
				}
				reference2.AddIcon(icon);
				reference2.Append($"{value:0}" + (suffixPercent ? "%" : ""));
				reference = false;
			}
		}
	}

	private void OnZoneChanged(uint u)
	{
		UnregCombatEvents();
		if (GameState.ContentFinderCondition != 0)
		{
			RegCombatEvents();
		}
	}

	private void OnConditionChanged(ConditionFlag flag, bool value)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Invalid comparison between Unknown and I4
		if ((int)flag == 26)
		{
			if (value)
			{
				RegCombatEvents();
			}
			else
			{
				UnregCombatEvents();
			}
		}
	}

	private void OnUpdate(IFramework _)
	{
		if (GameState.IsInPVPArea)
		{
			UnregCombatEvents();
			return;
		}
		if (!DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)26] && GameState.ContentFinderCondition == 0)
		{
			UnregCombatEvents();
			return;
		}
		state.Update();
		UpdateStatusBar();
	}

	private unsafe void Draw()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		if (Throttler.Shared.Throttle("AutoDisplayMitigationInfo-OnUpdatePartyDrawCondition"))
		{
			isNeedToDrawOnPartyList = (*Addons.PartyList).IsAddonAndNodesReady() && !GameState.IsInPVPArea;
		}
		if (!isNeedToDrawOnPartyList)
		{
			return;
		}
		ImDrawListPtr backgroundDrawList = ImGui.GetBackgroundDrawList();
		AddonPartyList* partyList = (AddonPartyList*)Addons.PartyList;
		ReadOnlySpan<PartyMitigationSnapshot> partySnapshot = state.PartySnapshot;
		for (int i = 0; (float)i < MathF.Min(partySnapshot.Length, ((AgentHUD)AgentHUD.Instance()).PartyMemberCount); i++)
		{
			try
			{
				ref PartyListMemberStruct reference = ref ((AddonPartyList)partyList).PartyMembers[i];
				if (reference.HPGaugeComponent != null && ((AtkComponentNode)((AtkComponentBase)reference.HPGaugeComponent).OwnerNode).IsVisible())
				{
					ref readonly PartyMitigationSnapshot status = ref partySnapshot.get_Item(i);
					DrawMitigationNode(backgroundDrawList, ref reference, in status);
					DrawShieldNode(backgroundDrawList, ref reference, in status);
				}
			}
			catch
			{
			}
		}
	}

	private unsafe static void DrawMitigationNode(ImDrawListPtr drawList, ref PartyListMemberStruct partyMember, in PartyMitigationSnapshot status)
	{
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		float num = MathF.Max(status.Physical, status.Magical);
		if (num <= 0f)
		{
			return;
		}
		AtkResNode* nameAndBarsContainer = partyMember.NameAndBarsContainer;
		if (nameAndBarsContainer == null || !((AtkResNode)nameAndBarsContainer).IsVisible())
		{
			return;
		}
		AtkTextNode* name = partyMember.Name;
		if (name == null || !((AtkTextNode)name).IsVisible())
		{
			return;
		}
		AddonPartyList* partyList = (AddonPartyList*)Addons.PartyList;
		float scale = ((AddonPartyList)partyList).Scale;
		using (OmenServiceBase<FontManager>.Instance().MiedingerMidFont120.Push())
		{
			string text = $"{num:N0}%";
			Vector2 vector = ImGui.CalcTextSize(ImU8String.op_Implicit(text), false, -1f);
			float x = ((AtkResNode)nameAndBarsContainer).ScreenX + (float)(int)((AtkResNode)nameAndBarsContainer).GetWidth() * scale - vector.X - 5f * scale;
			float y = ((AtkResNode)nameAndBarsContainer).ScreenY + 2f * scale;
			Vector2 vector2 = new Vector2(x, y);
			((ImDrawListPtr)(ref drawList)).AddText(vector2 + new Vector2(1f, 1f), 2634064639u, ImU8String.op_Implicit(text));
			((ImDrawListPtr)(ref drawList)).AddText(vector2, uint.MaxValue, ImU8String.op_Implicit(text));
		}
	}

	private unsafe static void DrawShieldNode(ImDrawListPtr drawList, ref PartyListMemberStruct partyMember, in PartyMitigationSnapshot status)
	{
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		float shield = status.Shield;
		AtkComponentBase* hPGaugeComponent = partyMember.HPGaugeComponent;
		if (hPGaugeComponent == null || !((AtkComponentNode)((AtkComponentBase)hPGaugeComponent).OwnerNode).IsVisible())
		{
			return;
		}
		AtkTextNode* textNodeById = ((AtkComponentBase)hPGaugeComponent).GetTextNodeById(2u);
		if (textNodeById == null || !((AtkTextNode)textNodeById).IsVisible())
		{
			return;
		}
		AddonPartyList* partyList = (AddonPartyList*)Addons.PartyList;
		AtkTextNode* asAtkTextNode = ((AtkTextNode)((AtkComponentGaugeBar)partyMember.MPGaugeBar).GetTextNodeById(2u)).GetAsAtkTextNode();
		AtkTextNode* asAtkTextNode2 = ((AtkTextNode)((AtkComponentGaugeBar)partyMember.MPGaugeBar).GetTextNodeById(3u)).GetAsAtkTextNode();
		if ((double)shield >= 100000.0)
		{
			if (asAtkTextNode != null && ((AtkTextNode)asAtkTextNode).IsVisible())
			{
				((AtkTextNode)asAtkTextNode).SetAlpha((byte)0);
			}
			if (asAtkTextNode2 != null && ((AtkTextNode)asAtkTextNode2).IsVisible())
			{
				((AtkTextNode)asAtkTextNode2).SetAlpha((byte)0);
			}
		}
		else
		{
			if (asAtkTextNode != null && ((AtkTextNode)asAtkTextNode).IsVisible())
			{
				((AtkTextNode)asAtkTextNode).SetAlpha(byte.MaxValue);
			}
			if (asAtkTextNode2 != null && ((AtkTextNode)asAtkTextNode2).IsVisible())
			{
				((AtkTextNode)asAtkTextNode2).SetAlpha(byte.MaxValue);
			}
		}
		if (shield <= 0f)
		{
			return;
		}
		float scale = ((AddonPartyList)partyList).Scale;
		using (OmenServiceBase<FontManager>.Instance().MiedingerMidFont120.Push())
		{
			string text = $"{shield:F0}";
			float num = ((AtkTextNode)textNodeById).ScreenX + (float)(int)((AtkTextNode)textNodeById).GetWidth() * ((AddonPartyList)partyList).Scale + 3f * scale;
			float num2 = ((AtkTextNode)textNodeById).ScreenY + (float)(int)((AtkTextNode)textNodeById).GetHeight() * ((AddonPartyList)partyList).Scale / 2f - 3f * scale;
			((ImDrawListPtr)(ref drawList)).AddText(new Vector2(num + 1f, num2 + 1f), 2634064639u, ImU8String.op_Implicit(text));
			((ImDrawListPtr)(ref drawList)).AddText(new Vector2(num, num2), uint.MaxValue, ImU8String.op_Implicit(text));
		}
	}
}
