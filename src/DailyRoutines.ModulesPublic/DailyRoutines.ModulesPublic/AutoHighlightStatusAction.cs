using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.ImGuiOm.Widgets.Combos;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class AutoHighlightStatusAction : ModuleBase
{
	private class Config : ModuleConfig
	{
		public float Countdown = 4f;

		public bool KeepHighlight = true;

		public Dictionary<uint, StatusConfig> MonitoredStatus = new Dictionary<uint, StatusConfig>();
	}

	private class StatusConfig
	{
		public uint[] BindActions { get; set; } = Array.Empty<uint>();

		public float Countdown { get; set; } = 4f;

		public bool KeepHighlight { get; set; } = true;
	}

	private readonly record struct StatusKey(uint EntityID, ushort StatusID);

	private struct StatusState(bool active, float remainingTime)
	{
		public bool Active = active;

		public float RemainingTime = remainingTime;
	}

	private struct TrackedStatus(StatusKey key, StatusState state)
	{
		public StatusKey Key = key;

		public StatusState State = state;
	}

	private struct ActionCalcEntry(uint actionID, float remainingTime, float countdown, float score)
	{
		public readonly uint ActionID = actionID;

		public float RemainingTime = remainingTime;

		public float Countdown = countdown;

		public float Score = score;
	}

	private Hook<IsActionHighlighted>? IsActionHighlightedHook;

	private Config config;

	private StatusSelectCombo statusCombo;

	private ActionSelectCombo actionCombo;

	private readonly List<uint> actionsToHighlight = new List<uint>(8);

	private readonly List<(uint ActionID, uint[] Chain)> comboChainsCache = new List<(uint, uint[])>(16);

	private readonly List<ActionCalcEntry> actionCalculationCache = new List<ActionCalcEntry>(16);

	private readonly List<TrackedStatus> trackedStatuses = new List<TrackedStatus>(32);

	private long lastUpdateTicks;

	private readonly List<int> resyncIndices = new List<int>(16);

	private readonly List<int> removeIndices = new List<int>(16);

	private static readonly FrozenDictionary<uint, StatusConfig> StatusConfigs = new Dictionary<uint, StatusConfig>
	{
		[838u] = new StatusConfig
		{
			BindActions = new uint[1] { 3599u },
			Countdown = 4f,
			KeepHighlight = true
		},
		[843u] = new StatusConfig
		{
			BindActions = new uint[1] { 3608u },
			Countdown = 4f,
			KeepHighlight = true
		},
		[1881u] = new StatusConfig
		{
			BindActions = new uint[1] { 16554u },
			Countdown = 4f,
			KeepHighlight = true
		},
		[1248u] = new StatusConfig
		{
			BindActions = new uint[1] { 8324u },
			Countdown = 10f,
			KeepHighlight = false
		},
		[143u] = new StatusConfig
		{
			BindActions = new uint[1] { 121u },
			Countdown = 4f,
			KeepHighlight = true
		},
		[144u] = new StatusConfig
		{
			BindActions = new uint[1] { 132u },
			Countdown = 4f,
			KeepHighlight = true
		},
		[1871u] = new StatusConfig
		{
			BindActions = new uint[1] { 16532u },
			Countdown = 4f,
			KeepHighlight = true
		},
		[2614u] = new StatusConfig
		{
			BindActions = new uint[1] { 24290u },
			Countdown = 6f,
			KeepHighlight = true
		},
		[2615u] = new StatusConfig
		{
			BindActions = new uint[1] { 24290u },
			Countdown = 6f,
			KeepHighlight = true
		},
		[2616u] = new StatusConfig
		{
			BindActions = new uint[1] { 24290u },
			Countdown = 6f,
			KeepHighlight = true
		},
		[179u] = new StatusConfig
		{
			BindActions = new uint[1] { 17864u },
			Countdown = 4f,
			KeepHighlight = true
		},
		[189u] = new StatusConfig
		{
			BindActions = new uint[1] { 17865u },
			Countdown = 4f,
			KeepHighlight = true
		},
		[1895u] = new StatusConfig
		{
			BindActions = new uint[1] { 16540u },
			Countdown = 4f,
			KeepHighlight = true
		},
		[124u] = new StatusConfig
		{
			BindActions = new uint[1] { 100u },
			Countdown = 4f,
			KeepHighlight = true
		},
		[1200u] = new StatusConfig
		{
			BindActions = new uint[2] { 7406u, 3560u },
			Countdown = 4f,
			KeepHighlight = true
		},
		[129u] = new StatusConfig
		{
			BindActions = new uint[1] { 113u },
			Countdown = 4f,
			KeepHighlight = true
		},
		[1201u] = new StatusConfig
		{
			BindActions = new uint[2] { 7407u, 3560u },
			Countdown = 4f,
			KeepHighlight = true
		},
		[1299u] = new StatusConfig
		{
			BindActions = new uint[1] { 7485u },
			Countdown = 4f,
			KeepHighlight = true
		},
		[2719u] = new StatusConfig
		{
			BindActions = new uint[1] { 25772u },
			Countdown = 4f,
			KeepHighlight = true
		},
		[2677u] = new StatusConfig
		{
			BindActions = new uint[1] { 45u },
			Countdown = 4f,
			KeepHighlight = true
		}
	}.ToFrozenDictionary();

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoHighlightStatusActionTitle"),
		Description = LanguageManager.Get("AutoHighlightStatusActionDescription"),
		Category = ModuleCategory.Action,
		Author = new string[1] { "HaKu" }
	};

	protected unsafe override void Init()
	{
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Expected O, but got Unknown
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Expected O, but got Unknown
		statusCombo = new StatusSelectCombo("Status");
		actionCombo = new ActionSelectCombo("Action");
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		if (config.MonitoredStatus.Count == 0)
		{
			config.MonitoredStatus = StatusConfigs.ToDictionary<KeyValuePair<uint, StatusConfig>, uint, StatusConfig>((KeyValuePair<uint, StatusConfig> x) => x.Key, (KeyValuePair<uint, StatusConfig> x) => new StatusConfig
			{
				BindActions = x.Value.BindActions,
				Countdown = x.Value.Countdown,
				KeepHighlight = x.Value.KeepHighlight
			});
			config.Save(this);
		}
		RebuildComboChains();
		IsActionHighlightedHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromMemberFunction<IsActionHighlighted>(typeof(MemberFunctionPointers), "IsActionHighlighted", new IsActionHighlighted(IsActionHighlightedDetour));
		IsActionHighlightedHook.Enable();
		DalamudServiceExtension.Instance<ICondition>().ConditionChange += new ConditionChangeDelegate(OnConditionChanged);
		OnConditionChanged((ConditionFlag)26, DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)26]);
	}

	protected override void Uninit()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		DalamudServiceExtension.Instance<ICondition>().ConditionChange -= new ConditionChangeDelegate(OnConditionChanged);
		ExitCombat();
	}

	protected override void ConfigUI()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_051f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0524: Unknown result type (might be due to invalid IL or missing references)
		//IL_0560: Unknown result type (might be due to invalid IL or missing references)
		//IL_0565: Unknown result type (might be due to invalid IL or missing references)
		//IL_0572: Unknown result type (might be due to invalid IL or missing references)
		//IL_0577: Unknown result type (might be due to invalid IL or missing references)
		//IL_0580: Unknown result type (might be due to invalid IL or missing references)
		//IL_059b: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05af: Unknown result type (might be due to invalid IL or missing references)
		//IL_062a: Unknown result type (might be due to invalid IL or missing references)
		//IL_062f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0646: Unknown result type (might be due to invalid IL or missing references)
		//IL_0659: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cf: Unknown result type (might be due to invalid IL or missing references)
		TableDisposable val = ImRaii.Table(ImU8String.op_Implicit("PlayersInList"), 5, (ImGuiTableFlags)33556352, new Vector2(0f, 300f * Globals.GlobalUIScale));
		try
		{
			if (!val)
			{
				return;
			}
			ImGui.TableSetupColumn(ImU8String.op_Implicit("OPERATION"), (ImGuiTableColumnFlags)0, 0f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("STATUS"), (ImGuiTableColumnFlags)0, 0f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("ACTION"), (ImGuiTableColumnFlags)0, 0f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("COUNTDOWN"), (ImGuiTableColumnFlags)0, 0f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("KEEP_HIGHLIGHT"), (ImGuiTableColumnFlags)0, 0f, 0u);
			ImGui.TableNextRow((ImGuiTableRowFlags)1);
			ImGui.TableNextColumn();
			if (ImGuiOm.SelectableTextCentered($"{FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61543)}  {LanguageManager.Get("Add")} / {LanguageManager.Get("Edit")}", selected: false, (ImGuiSelectableFlags)0))
			{
				ImGui.OpenPopup(ImU8String.op_Implicit("ADD_NEW_PRESET_POPUP"), (ImGuiPopupFlags)0);
			}
			ItemWidthDisposable val2 = ImRaii.ItemWidth(200f * Globals.GlobalUIScale);
			try
			{
				PopupDisposable val3 = ImRaii.Popup(ImU8String.op_Implicit("ADD_NEW_PRESET_POPUP"));
				try
				{
					if (PopupDisposable.op_Implicit(val3))
					{
						ImU8String val4 = new ImU8String(19, 1);
						((ImU8String)(ref val4)).AppendFormatted<string>(LanguageManager.Get("AutoHighlightStatusAction-Countdown"));
						((ImU8String)(ref val4)).AppendLiteral("##ReminderThreshold");
						ImGui.SliderFloat(val4, ref config.Countdown, 2f, 10f, ImU8String.op_Implicit("%.1f"), (ImGuiSliderFlags)0);
						if (ImGui.IsItemDeactivatedAfterEdit())
						{
							config.Save(this);
						}
						ImGuiOm.HelpMarker(LanguageManager.Get("AutoHighlightStatusAction-Countdown-Help"), 40f, (FontAwesomeIcon)61530);
						ImU8String val5 = new ImU8String(26, 1);
						((ImU8String)(ref val5)).AppendFormatted<string>(LanguageManager.Get("AutoHighlightStatusAction-KeepHighlightAfterExpire"));
						((ImU8String)(ref val5)).AppendLiteral("##KeepHighlightAfterExpire");
						if (ImGui.Checkbox(val5, ref config.KeepHighlight))
						{
							config.Save(this);
						}
						ImGuiOm.HelpMarker(LanguageManager.Get("AutoHighlightStatusAction-KeepHighlightAfterExpire-Help"), 40f, (FontAwesomeIcon)61530);
						ImGui.Spacing();
						IdDisposable val6 = ImRaii.PushId(ImU8String.op_Implicit("Status"), true);
						try
						{
							statusCombo.DrawRadio();
						}
						finally
						{
							((IDisposable)val6)?.Dispose();
						}
						ImGui.SameLine();
						ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("Status")));
						val6 = ImRaii.PushId(ImU8String.op_Implicit("Action"), true);
						try
						{
							actionCombo.DrawCheckbox();
						}
						finally
						{
							((IDisposable)val6)?.Dispose();
						}
						ImGui.SameLine();
						ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("Action")));
						ImGui.Spacing();
						if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61543, LanguageManager.Get("Add")) && statusCombo.SelectedID != 0 && actionCombo.SelectedIDs.Count > 0)
						{
							config.MonitoredStatus[statusCombo.SelectedID] = new StatusConfig
							{
								BindActions = actionCombo.SelectedIDs.ToArray(),
								Countdown = config.Countdown,
								KeepHighlight = config.KeepHighlight
							};
							config.Save(this);
							RebuildComboChains();
						}
					}
				}
				finally
				{
					((PopupDisposable)(ref val3)).Dispose();
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
			ImGui.TableNextColumn();
			ImGuiOm.SelectableTextCentered(LanguageManager.Get("Status"), selected: false, (ImGuiSelectableFlags)0);
			ImGui.TableNextColumn();
			ImGuiOm.SelectableTextCentered(LanguageManager.Get("Action"), selected: false, (ImGuiSelectableFlags)0);
			ImGui.TableNextColumn();
			ImGuiOm.SelectableTextCentered(LanguageManager.Get("AutoHighlightStatusAction-Countdown"), selected: false, (ImGuiSelectableFlags)0);
			ImGui.TableNextColumn();
			ImGuiOm.SelectableTextCentered(LanguageManager.Get("AutoHighlightStatusAction-KeepHighlightAfterExpire"), selected: false, (ImGuiSelectableFlags)0);
			uint key = 0u;
			bool flag = false;
			ISharedImmediateTexture val10 = default(ISharedImmediateTexture);
			ISharedImmediateTexture val13 = default(ISharedImmediateTexture);
			foreach (KeyValuePair<uint, StatusConfig> item3 in config.MonitoredStatus)
			{
				item3.Deconstruct(out var key2, out var value);
				uint num = key2;
				StatusConfig statusConfig = value;
				ImU8String val7 = new ImU8String(0, 1);
				((ImU8String)(ref val7)).AppendFormatted<uint>(num);
				IdDisposable val8 = ImRaii.PushId(val7, true);
				try
				{
					if (!LuminaGetter.TryGetRow<Status>(num, out Status item))
					{
						continue;
					}
					ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
					GameIconLookup val9 = new GameIconLookup(((Status)(ref item)).Icon, false, true, (ClientLanguage?)null);
					if (!obj.TryGetFromGameIcon(ref val9, ref val10))
					{
						continue;
					}
					ImGui.TableNextRow();
					ImGui.TableNextColumn();
					if (ImGuiOm.ButtonStretch(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)62189) + "  " + LanguageManager.Get("Delete")))
					{
						key = num;
						flag = true;
					}
					ImGui.TableNextColumn();
					GroupDisposable val11 = ImRaii.Group();
					try
					{
						ImGui.Image(val10.GetWrapOrEmpty().Handle, new Vector2(ImGui.GetTextLineHeight()));
						ImGui.SameLine();
						ImGui.TextUnformatted(ImU8String.op_Implicit(((object)((Status)(ref item)).Name/*cast due to constrained. prefix*/).ToString()));
					}
					finally
					{
						((GroupDisposable)(ref val11)).Dispose();
					}
					if (ImGui.IsItemHovered())
					{
						ImGui.SetMouseCursor((ImGuiMouseCursor)7);
					}
					if (ImGui.IsItemClicked())
					{
						statusCombo.SelectedID = num;
					}
					ImGui.TableNextColumn();
					GroupDisposable val12 = ImRaii.Group();
					try
					{
						uint[] bindActions = statusConfig.BindActions;
						for (int i = 0; i < bindActions.Length; i++)
						{
							if (!LuminaGetter.TryGetRow<Action>(bindActions[i], out Action item2))
							{
								continue;
							}
							ITextureProvider obj2 = DalamudServiceExtension.Instance<ITextureProvider>();
							val9 = new GameIconLookup((uint)((Action)(ref item2)).Icon, false, true, (ClientLanguage?)null);
							if (obj2.TryGetFromGameIcon(ref val9, ref val13))
							{
								GroupDisposable val14 = ImRaii.Group();
								try
								{
									ImGui.Image(val13.GetWrapOrEmpty().Handle, new Vector2(ImGui.GetTextLineHeight()));
									ImGui.SameLine();
									ImGui.TextUnformatted(ImU8String.op_Implicit(((object)((Action)(ref item2)).Name/*cast due to constrained. prefix*/).ToString()));
								}
								finally
								{
									((GroupDisposable)(ref val14)).Dispose();
								}
								ImGui.SameLine();
							}
						}
					}
					finally
					{
						((GroupDisposable)(ref val12)).Dispose();
					}
					if (ImGui.IsItemHovered())
					{
						ImGui.SetMouseCursor((ImGuiMouseCursor)7);
					}
					if (ImGui.IsItemClicked())
					{
						actionCombo.SelectedIDs = statusConfig.BindActions.ToHashSet();
					}
					ImGui.TableNextColumn();
					float countdown = statusConfig.Countdown;
					ImGui.SetNextItemWidth(-1f);
					ImU8String val15 = new ImU8String(12, 1);
					((ImU8String)(ref val15)).AppendLiteral("##Countdown_");
					((ImU8String)(ref val15)).AppendFormatted<uint>(num);
					if (ImGui.InputFloat(val15, ref countdown, 0.1f, 0.5f, ImU8String.op_Implicit("%.1f"), (ImGuiInputTextFlags)0))
					{
						statusConfig.Countdown = Math.Clamp(countdown, 0.5f, 30f);
					}
					if (ImGui.IsItemDeactivatedAfterEdit())
					{
						config.Save(this);
						RebuildComboChains();
					}
					ImGui.TableNextColumn();
					bool keepHighlight = statusConfig.KeepHighlight;
					ImGuiOm.CenterAlignFor(ImGui.GetFrameHeight());
					ImU8String val16 = new ImU8String(16, 1);
					((ImU8String)(ref val16)).AppendLiteral("##KeepHighlight_");
					((ImU8String)(ref val16)).AppendFormatted<uint>(num);
					if (ImGui.Checkbox(val16, ref keepHighlight))
					{
						statusConfig.KeepHighlight = keepHighlight;
						config.Save(this);
					}
				}
				finally
				{
					((IDisposable)val8)?.Dispose();
				}
			}
			if (flag)
			{
				config.MonitoredStatus.Remove(key);
				config.Save(this);
				RebuildComboChains();
			}
		}
		finally
		{
			((TableDisposable)(ref val)).Dispose();
		}
	}

	private void OnConditionChanged(ConditionFlag flag, bool value)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Invalid comparison between Unknown and I4
		if ((int)flag == 26)
		{
			if (value && !GameState.IsInPVPArea)
			{
				EnterCombat();
			}
			else
			{
				ExitCombat();
			}
		}
	}

	private void EnterCombat()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		ExitCombat();
		lastUpdateTicks = Environment.TickCount64;
		SeedTrackedStatuses();
		OmenServiceBase<FrameworkManager>.Instance().Reg(new OnUpdateDelegate(OnUpdate), 500u);
		OmenServiceBase<UseActionManager>.Instance().RegPostUseActionLocation(OnPostUseActionLocation);
		OmenServiceBase<CharacterStatusManager>.Instance().RegGain(OnGainStatus);
		OmenServiceBase<CharacterStatusManager>.Instance().RegLose(OnLoseStatus);
	}

	private void ExitCombat()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		OmenServiceBase<FrameworkManager>.Instance().Unreg(OnUpdate);
		OmenServiceBase<UseActionManager>.Instance().Unreg(OnPostUseActionLocation);
		OmenServiceBase<CharacterStatusManager>.Instance().Unreg(OnGainStatus);
		OmenServiceBase<CharacterStatusManager>.Instance().Unreg(OnLoseStatus);
		actionsToHighlight.Clear();
		actionCalculationCache.Clear();
		trackedStatuses.Clear();
		resyncIndices.Clear();
		removeIndices.Clear();
		lastUpdateTicks = 0L;
	}

	[SkipLocalsInit]
	private unsafe void OnUpdate(IFramework _)
	{
		if (GameState.IsInPVPArea || !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)26])
		{
			ExitCombat();
			return;
		}
		long tickCount = Environment.TickCount64;
		float num = (float)(tickCount - lastUpdateTicks) / 1000f;
		lastUpdateTicks = tickCount;
		if (num <= 0f)
		{
			num = 1f;
		}
		if (num > 5f)
		{
			num = 5f;
		}
		resyncIndices.Clear();
		removeIndices.Clear();
		for (int i = 0; i < trackedStatuses.Count; i++)
		{
			ref TrackedStatus reference = ref CollectionsMarshal.AsSpan(trackedStatuses)[i];
			if (reference.State.Active)
			{
				float num2 = reference.State.RemainingTime - num;
				reference.State.RemainingTime = num2;
				float cutoff;
				if (num2 <= 0f)
				{
					resyncIndices.Add(i);
				}
				else if (TryGetStatusResyncCutoff(reference.Key.StatusID, out cutoff) && num2 <= cutoff)
				{
					resyncIndices.Add(i);
				}
			}
		}
		foreach (int resyncIndex in resyncIndices)
		{
			ref TrackedStatus reference2 = ref CollectionsMarshal.AsSpan(trackedStatuses)[resyncIndex];
			ref StatusConfig valueRefOrNullRef = ref CollectionsMarshal.GetValueRefOrNullRef(config.MonitoredStatus, reference2.Key.StatusID);
			float remaining;
			if (Unsafe.IsNullRef(in valueRefOrNullRef))
			{
				removeIndices.Add(resyncIndex);
			}
			else if (TryResyncStatus(reference2.Key.EntityID, reference2.Key.StatusID, out remaining))
			{
				reference2.State.Active = true;
				reference2.State.RemainingTime = remaining;
			}
			else if (valueRefOrNullRef.KeepHighlight)
			{
				reference2.State.Active = false;
				reference2.State.RemainingTime = 0f;
			}
			else
			{
				removeIndices.Add(resyncIndex);
			}
		}
		for (int num3 = removeIndices.Count - 1; num3 >= 0; num3--)
		{
			trackedStatuses.RemoveAt(removeIndices[num3]);
		}
		removeIndices.Clear();
		actionCalculationCache.Clear();
		uint num4 = 0u;
		IGameObject target = TargetManager.Target;
		if (target is IBattleNPC battleNPC && !target.IsDead)
		{
			num4 = battleNPC.EntityID;
		}
		uint entityID = LocalPlayerState.EntityID;
		for (int j = 0; j < trackedStatuses.Count; j++)
		{
			ref TrackedStatus reference3 = ref CollectionsMarshal.AsSpan(trackedStatuses)[j];
			if (reference3.Key.EntityID != num4 && reference3.Key.EntityID != entityID)
			{
				continue;
			}
			ref StatusConfig valueRefOrNullRef2 = ref CollectionsMarshal.GetValueRefOrNullRef(config.MonitoredStatus, reference3.Key.StatusID);
			if (Unsafe.IsNullRef(in valueRefOrNullRef2))
			{
				removeIndices.Add(j);
				continue;
			}
			float num5 = (reference3.State.Active ? reference3.State.RemainingTime : 0f);
			float countdown = valueRefOrNullRef2.Countdown;
			uint[] bindActions = valueRefOrNullRef2.BindActions;
			foreach (uint num6 in bindActions)
			{
				int num7 = FindComboChainIndex(num6);
				if (num7 < 0)
				{
					continue;
				}
				float num8 = num5 - countdown * (float)CollectionsMarshal.AsSpan(comboChainsCache)[num7].Chain.Length;
				bool flag = false;
				for (int l = 0; l < actionCalculationCache.Count; l++)
				{
					if (actionCalculationCache[l].ActionID == num6)
					{
						if (num8 < actionCalculationCache[l].Score)
						{
							actionCalculationCache[l] = new ActionCalcEntry(num6, num5, countdown, num8);
						}
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					actionCalculationCache.Add(new ActionCalcEntry(num6, num5, countdown, num8));
				}
			}
		}
		for (int num9 = removeIndices.Count - 1; num9 >= 0; num9--)
		{
			trackedStatuses.RemoveAt(removeIndices[num9]);
		}
		actionsToHighlight.Clear();
		foreach (ActionCalcEntry item in actionCalculationCache)
		{
			if (item.Score > 0f)
			{
				continue;
			}
			int num10 = FindComboChainIndex(item.ActionID);
			if (num10 < 0)
			{
				continue;
			}
			ref(uint, uint[]) reference4 = ref CollectionsMarshal.AsSpan(comboChainsCache)[num10];
			bool flag2 = true;
			uint[] bindActions = reference4.Item2;
			foreach (uint num11 in bindActions)
			{
				if (((ActionManager)ActionManager.Instance()).IsActionHighlighted((ActionType)1, num11))
				{
					flag2 = false;
					break;
				}
			}
			if (flag2)
			{
				actionsToHighlight.Add(reference4.Item2[0]);
			}
		}
	}

	private void OnPostUseActionLocation(bool result, ActionType actionType, uint actionID, ulong targetID, Vector3 location, uint extraParam, byte a7)
	{
		if (GameState.IsInPVPArea || !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)26])
		{
			ExitCombat();
		}
		else
		{
			actionsToHighlight.Remove(actionID);
		}
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private unsafe bool IsActionHighlightedDetour(ActionManager* actionManager, ActionType actionType, uint actionID)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		foreach (uint item in actionsToHighlight)
		{
			if (item == actionID)
			{
				return true;
			}
		}
		return IsActionHighlightedHook.Original.Invoke(actionManager, actionType, actionID);
	}

	private void OnGainStatus(IBattleChara player, ushort statusID, ushort param, ushort stackCount, TimeSpan remainingTime, ulong sourceID)
	{
		if (sourceID == LocalPlayerState.EntityID && !(remainingTime.TotalSeconds <= 0.0) && !Unsafe.IsNullRef(in CollectionsMarshal.GetValueRefOrNullRef(config.MonitoredStatus, statusID)))
		{
			AddOrUpdateTrackedStatus(player.EntityID, statusID, active: true, (float)remainingTime.TotalSeconds);
		}
	}

	private void OnLoseStatus(IBattleChara player, ushort statusID, ushort param, ushort stackCount, ulong sourceID)
	{
		if (sourceID == LocalPlayerState.EntityID)
		{
			ref StatusConfig valueRefOrNullRef = ref CollectionsMarshal.GetValueRefOrNullRef(config.MonitoredStatus, statusID);
			if (Unsafe.IsNullRef(in valueRefOrNullRef))
			{
				RemoveTrackedStatus(player.EntityID, statusID);
			}
			else if (valueRefOrNullRef.KeepHighlight)
			{
				AddOrUpdateTrackedStatus(player.EntityID, statusID, active: false, 0f);
			}
			else
			{
				RemoveTrackedStatus(player.EntityID, statusID);
			}
		}
	}

	private unsafe void SeedTrackedStatuses()
	{
		int num = -1;
		foreach (IGameObject item in IOmenDalamudService<IObjectTable>.Instance())
		{
			num++;
			if (num >= 200)
			{
				break;
			}
			if (!(item is IBattleChara battleChara) || item.IsDead)
			{
				continue;
			}
			BattleChara* ptr = battleChara.ToBCStruct();
			if (ptr == null)
			{
				continue;
			}
			Span<Status> status = ((StatusManager)(&((BattleChara)ptr).StatusManager)).Status;
			for (int i = 0; i < status.Length; i++)
			{
				ref Status reference = ref status[i];
				if (reference.StatusId != 0 && reference.SourceObject.ObjectId == LocalPlayerState.EntityID && !Unsafe.IsNullRef(in CollectionsMarshal.GetValueRefOrNullRef(config.MonitoredStatus, reference.StatusId)))
				{
					AddOrUpdateTrackedStatus(battleChara.EntityID, reference.StatusId, active: true, reference.RemainingTime);
				}
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private int FindComboChainIndex(uint actionID)
	{
		for (int i = 0; i < comboChainsCache.Count; i++)
		{
			if (comboChainsCache[i].ActionID == actionID)
			{
				return i;
			}
		}
		return -1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool TryGetStatusResyncCutoff(ushort statusID, out float cutoff)
	{
		ref StatusConfig valueRefOrNullRef = ref CollectionsMarshal.GetValueRefOrNullRef(config.MonitoredStatus, statusID);
		if (Unsafe.IsNullRef(in valueRefOrNullRef))
		{
			cutoff = 0f;
			return false;
		}
		int num = 0;
		uint[] bindActions = valueRefOrNullRef.BindActions;
		foreach (uint actionID in bindActions)
		{
			int num2 = FindComboChainIndex(actionID);
			if (num2 >= 0)
			{
				ref(uint, uint[]) reference = ref CollectionsMarshal.AsSpan(comboChainsCache)[num2];
				if (reference.Item2.Length > num)
				{
					num = reference.Item2.Length;
				}
			}
		}
		if (num == 0)
		{
			cutoff = 0f;
			return false;
		}
		cutoff = valueRefOrNullRef.Countdown * (float)num;
		return cutoff > 0f;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static bool TryResyncStatus(uint entityID, ushort statusID, out float remaining)
	{
		BattleChara* ptr = ((CharacterManager)CharacterManager.Instance()).LookupBattleCharaByEntityId(entityID);
		if (ptr == null || ((BattleChara)ptr).IsDead())
		{
			remaining = 0f;
			return false;
		}
		Span<Status> status = ((StatusManager)(&((BattleChara)ptr).StatusManager)).Status;
		for (int i = 0; i < status.Length; i++)
		{
			ref Status reference = ref status[i];
			if (reference.StatusId == statusID && reference.SourceObject.ObjectId == LocalPlayerState.EntityID)
			{
				remaining = reference.RemainingTime;
				return remaining > 0f;
			}
		}
		remaining = 0f;
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void AddOrUpdateTrackedStatus(uint entityID, ushort statusID, bool active, float remainingTime)
	{
		Span<TrackedStatus> span = CollectionsMarshal.AsSpan(trackedStatuses);
		for (int i = 0; i < span.Length; i++)
		{
			if (span[i].Key.EntityID == entityID && span[i].Key.StatusID == statusID)
			{
				span[i].State.Active = active;
				span[i].State.RemainingTime = remainingTime;
				return;
			}
		}
		trackedStatuses.Add(new TrackedStatus(new StatusKey(entityID, statusID), new StatusState(active, remainingTime)));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void RemoveTrackedStatus(uint entityID, ushort statusID)
	{
		for (int i = 0; i < trackedStatuses.Count; i++)
		{
			if (trackedStatuses[i].Key.EntityID == entityID && trackedStatuses[i].Key.StatusID == statusID)
			{
				trackedStatuses.RemoveAt(i);
				break;
			}
		}
	}

	private void RebuildComboChains()
	{
		comboChainsCache.Clear();
		foreach (StatusConfig value in config.MonitoredStatus.Values)
		{
			uint[] bindActions = value.BindActions;
			foreach (uint num in bindActions)
			{
				bool flag = false;
				for (int j = 0; j < comboChainsCache.Count; j++)
				{
					if (comboChainsCache[j].ActionID == num)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					comboChainsCache.Add((num, FetchComboChain(num)));
				}
			}
		}
		static uint[] FetchComboChain(uint actionID)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			List<uint> list = new List<uint>();
			uint num2 = actionID;
			Action item;
			while (num2 != 0 && LuminaGetter.TryGetRow<Action>(num2, out item))
			{
				list.Add(num2);
				RowRef<Action> val = ((Action)(ref item)).ActionCombo;
				if (val.RowId == 0)
				{
					break;
				}
				num2 = val.RowId;
			}
			list.Reverse();
			return list.ToArray();
		}
	}
}
