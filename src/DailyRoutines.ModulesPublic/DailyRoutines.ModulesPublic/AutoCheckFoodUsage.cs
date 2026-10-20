using System;
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
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Hooking;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using Lumina.Excel.Sheets;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.ImGuiOm.Widgets.Combos;
using OmenTools.Info.Lumina;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;

namespace DailyRoutines.ModulesPublic;

public class AutoCheckFoodUsage : ModuleBase
{
	public delegate nint CountdownInitDelegate(nint a1, nint a2);

	private class FoodUsagePreset : IEquatable<FoodUsagePreset>
	{
		public uint ItemID { get; set; }

		public bool IsHQ { get; set; } = true;

		public HashSet<uint> Zones { get; set; } = new HashSet<uint>();

		public HashSet<uint> ClassJobs { get; set; } = new HashSet<uint>();

		public bool Enabled { get; set; } = true;

		public FoodUsagePreset()
		{
		}

		public FoodUsagePreset(uint itemID)
		{
			ItemID = itemID;
		}

		public FoodUsagePreset(uint itemID, bool isHQ)
			: this(itemID)
		{
			IsHQ = isHQ;
		}

		public bool Equals(FoodUsagePreset? other)
		{
			if (other != null && ItemID == other.ItemID)
			{
				return IsHQ == other.IsHQ;
			}
			return false;
		}

		public override bool Equals(object? obj)
		{
			return Equals(obj as FoodUsagePreset);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(ItemID, IsHQ);
		}

		public static bool operator ==(FoodUsagePreset? left, FoodUsagePreset? right)
		{
			return EqualityComparer<FoodUsagePreset>.Default.Equals(left, right);
		}

		public static bool operator !=(FoodUsagePreset? left, FoodUsagePreset? right)
		{
			return !(left == right);
		}
	}

	private class Config : ModuleConfig
	{
		public HashSet<ConditionFlag> ConditionEnd = new HashSet<ConditionFlag>();

		public HashSet<ConditionFlag> ConditionStart = new HashSet<ConditionFlag>();

		public Dictionary<FoodCheckpoint, bool> EnabledCheckpoints = new Dictionary<FoodCheckpoint, bool>();

		public List<FoodUsagePreset> Presets = new List<FoodUsagePreset>();

		public int RefreshThreshold = 600;

		public bool SendChat = true;
	}

	private enum FoodCheckpoint
	{
		区域切换时,
		倒计时开始时,
		条件变更时
	}

	private CompSig countdownInitSig;

	private Hook<CountdownInitDelegate>? CountdownInitHook;

	private Config config;

	private JobSelectCombo jobSelectCombo;

	private uint selectedItem;

	private string selectItemSearch = string.Empty;

	private bool selectItemIsHQ = true;

	private string zoneSearchInput = string.Empty;

	private string conditionSearchInput = string.Empty;

	private readonly DateTime lastFoodUsageTime = DateTime.MinValue;

	private const int FOOD_USAGE_COOLDOWN_SECONDS = 10;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoCheckFoodUsageTitle"),
		Description = LanguageManager.Get("AutoCheckFoodUsageDescription"),
		Category = ModuleCategory.Combat
	};

	protected override void Init()
	{
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Expected O, but got Unknown
		countdownInitSig = new CompSig("48 89 5C 24 10 57 48 83 EC 40 48 8B DA 48 8B F9 48 8B 49 08");
		jobSelectCombo = new JobSelectCombo("Job");
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		FoodCheckpoint[] values = Enum.GetValues<FoodCheckpoint>();
		foreach (FoodCheckpoint key in values)
		{
			config.EnabledCheckpoints.TryAdd(key, value: false);
		}
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 60000
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		if (CountdownInitHook == null)
		{
			CountdownInitHook = countdownInitSig.GetHook<CountdownInitDelegate>(CountdownInitDetour);
		}
		CountdownInitHook.Enable();
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += OnZoneChanged;
		DalamudServiceExtension.Instance<ICondition>().ConditionChange += new ConditionChangeDelegate(OnConditionChanged);
	}

	protected override void Uninit()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		DalamudServiceExtension.Instance<ICondition>().ConditionChange -= new ConditionChangeDelegate(OnConditionChanged);
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChanged;
	}

	protected unsafe override void ConfigUI()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0504: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0538: Unknown result type (might be due to invalid IL or missing references)
		//IL_054c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0552: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_057d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0603: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_0604: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Invalid comparison between Unknown and I4
		//IL_0616: Unknown result type (might be due to invalid IL or missing references)
		//IL_062c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0642: Unknown result type (might be due to invalid IL or missing references)
		//IL_0658: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0691: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Invalid comparison between Unknown and I4
		//IL_06c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0700: Unknown result type (might be due to invalid IL or missing references)
		//IL_0705: Unknown result type (might be due to invalid IL or missing references)
		//IL_0724: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_0918: Unknown result type (might be due to invalid IL or missing references)
		//IL_091d: Unknown result type (might be due to invalid IL or missing references)
		//IL_095c: Unknown result type (might be due to invalid IL or missing references)
		//IL_040b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_043c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_097f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0802: Unknown result type (might be due to invalid IL or missing references)
		//IL_046d: Unknown result type (might be due to invalid IL or missing references)
		//IL_09aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_09af: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_09cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0832: Unknown result type (might be due to invalid IL or missing references)
		//IL_0481: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a0d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a26: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a2b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a30: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a32: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c3d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a4f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b6d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b72: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bde: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(1, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("AutoCheckFoodUsage-Checkpoint"));
		((ImU8String)(ref val)).AppendLiteral(":");
		ImGui.TextColored(ref vector, val);
		IndentDisposable val2 = ImRaii.PushIndent(1, true);
		try
		{
			ImGuiOm.ScaledDummy(2f);
			FoodCheckpoint[] values = Enum.GetValues<FoodCheckpoint>();
			for (int i = 0; i < values.Length; i++)
			{
				FoodCheckpoint key = values[i];
				ImGui.SameLine();
				bool value = config.EnabledCheckpoints[key];
				if (ImGui.Checkbox(ImU8String.op_Implicit(key.ToString()), ref value))
				{
					config.EnabledCheckpoints[key] = value;
					config.Save(this);
				}
			}
			if (config.EnabledCheckpoints[FoodCheckpoint.条件变更时])
			{
				IndentDisposable val3 = ImRaii.PushIndent(1, true);
				try
				{
					ImGui.AlignTextToFramePadding();
					vector = KnownColor.Chocolate.ToVector4();
					ImU8String val4 = new ImU8String(1, 1);
					((ImU8String)(ref val4)).AppendFormatted<string>(LanguageManager.Get("AutoCheckFoodUsage-WhenConditionBegin"));
					((ImU8String)(ref val4)).AppendLiteral(":");
					ImGui.TextColored(ref vector, val4);
					ImGui.SameLine();
					ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
					ComboDisposable val5 = ImRaii.Combo(ImU8String.op_Implicit("###ConditionBeginCombo"), ImU8String.op_Implicit(LanguageManager.Get("AutoCheckFoodUsage-SelectedAmount", config.ConditionStart.Count)), (ImGuiComboFlags)8);
					try
					{
						if (ComboDisposable.op_Implicit(val5))
						{
							if (ImGui.IsWindowAppearing())
							{
								conditionSearchInput = string.Empty;
							}
							ImGui.SetNextItemWidth(-1f);
							ImGui.InputTextWithHint(ImU8String.op_Implicit("###ConditionBeginSearch"), ImU8String.op_Implicit(LanguageManager.Get("PleaseSearch")), ref conditionSearchInput, 128, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
							ImGui.Separator();
							ConditionFlag[] values2 = Enum.GetValues<ConditionFlag>();
							for (int i = 0; i < values2.Length; i++)
							{
								ConditionFlag val6 = values2[i];
								if ((int)val6 <= 1)
								{
									continue;
								}
								string text = ((object)(*(ConditionFlag*)(&val6))/*cast due to constrained. prefix*/).ToString();
								if (!string.IsNullOrWhiteSpace(conditionSearchInput) && !text.Contains(conditionSearchInput, StringComparison.OrdinalIgnoreCase))
								{
									continue;
								}
								ImU8String val7 = new ImU8String(9, 2);
								((ImU8String)(ref val7)).AppendFormatted<string>(text);
								((ImU8String)(ref val7)).AppendLiteral("###");
								((ImU8String)(ref val7)).AppendFormatted<ConditionFlag>(val6);
								((ImU8String)(ref val7)).AppendLiteral("_Begin");
								if (ImGui.Selectable(val7, config.ConditionStart.Contains(val6), (ImGuiSelectableFlags)0, default(Vector2)))
								{
									if (!config.ConditionStart.Remove(val6))
									{
										config.ConditionStart.Add(val6);
									}
									config.Save(this);
								}
							}
						}
					}
					finally
					{
						((ComboDisposable)(ref val5)).Dispose();
					}
					ImGui.AlignTextToFramePadding();
					vector = KnownColor.Chocolate.ToVector4();
					ImU8String val8 = new ImU8String(1, 1);
					((ImU8String)(ref val8)).AppendFormatted<string>(LanguageManager.Get("AutoCheckFoodUsage-WhenConditionEnd"));
					((ImU8String)(ref val8)).AppendLiteral(":");
					ImGui.TextColored(ref vector, val8);
					ImGui.SameLine();
					ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
					ComboDisposable val9 = ImRaii.Combo(ImU8String.op_Implicit("###ConditionEndCombo"), ImU8String.op_Implicit(LanguageManager.Get("AutoCheckFoodUsage-SelectedAmount", config.ConditionEnd.Count)), (ImGuiComboFlags)8);
					try
					{
						if (ComboDisposable.op_Implicit(val9))
						{
							if (ImGui.IsWindowAppearing())
							{
								conditionSearchInput = string.Empty;
							}
							ImGui.SetNextItemWidth(-1f);
							ImGui.InputTextWithHint(ImU8String.op_Implicit("###ConditionEndSearch"), ImU8String.op_Implicit(LanguageManager.Get("PleaseSearch")), ref conditionSearchInput, 128, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
							ImGui.Separator();
							ConditionFlag[] values2 = Enum.GetValues<ConditionFlag>();
							for (int i = 0; i < values2.Length; i++)
							{
								ConditionFlag val10 = values2[i];
								if ((int)val10 <= 1)
								{
									continue;
								}
								string text2 = ((object)(*(ConditionFlag*)(&val10))/*cast due to constrained. prefix*/).ToString();
								if (!string.IsNullOrWhiteSpace(conditionSearchInput) && !text2.Contains(conditionSearchInput, StringComparison.OrdinalIgnoreCase))
								{
									continue;
								}
								ImU8String val11 = new ImU8String(7, 2);
								((ImU8String)(ref val11)).AppendFormatted<string>(text2);
								((ImU8String)(ref val11)).AppendLiteral("###");
								((ImU8String)(ref val11)).AppendFormatted<ConditionFlag>(val10);
								((ImU8String)(ref val11)).AppendLiteral("_End");
								if (ImGui.Selectable(val11, config.ConditionEnd.Contains(val10), (ImGuiSelectableFlags)0, default(Vector2)))
								{
									if (!config.ConditionEnd.Remove(val10))
									{
										config.ConditionEnd.Add(val10);
									}
									config.Save(this);
								}
							}
						}
					}
					finally
					{
						((ComboDisposable)(ref val9)).Dispose();
					}
				}
				finally
				{
					((IDisposable)val3)?.Dispose();
				}
			}
			ImGuiOm.ScaledDummy(2f);
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
		vector = KnownColor.Chocolate.ToVector4();
		ImU8String val12 = default(ImU8String);
		((ImU8String)(ref val12))._002Ector(1, 1);
		((ImU8String)(ref val12)).AppendFormatted<string>(LanguageManager.Get("Settings"));
		((ImU8String)(ref val12)).AppendLiteral(":");
		ImGui.TextColored(ref vector, val12);
		val2 = ImRaii.PushIndent(1, true);
		ImU8String val14;
		try
		{
			ImGui.Dummy(Vector2.One);
			ImGui.SetNextItemWidth(50f * Globals.GlobalUIScale);
			ImU8String val13 = ImU8String.op_Implicit(LanguageManager.Get("AutoCheckFoodUsage-RefreshThreshold"));
			ref int refreshThreshold = ref config.RefreshThreshold;
			val14 = default(ImU8String);
			ImGui.InputInt(val13, ref refreshThreshold, 0, 0, val14, (ImGuiInputTextFlags)0);
			if (ImGui.IsItemDeactivatedAfterEdit())
			{
				config.Save(this);
			}
			ImGui.SameLine();
			if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("SendChat")), ref config.SendChat))
			{
				config.Save(this);
			}
			ImGuiOm.HelpMarker(LanguageManager.Get("AutoCheckFoodUsage-RefreshThresholdHelp"), 40f, (FontAwesomeIcon)61530);
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
		Vector2 vector2 = ImGui.GetContentRegionAvail() - Globals.ScaledVector2(100f);
		vector2.Y = 0f;
		Vector2 vector3 = vector2;
		TableDisposable val15 = ImRaii.Table(ImU8String.op_Implicit("FoodPreset"), 4, (ImGuiTableFlags)1920, vector3);
		try
		{
			if (!val15)
			{
				return;
			}
			ImGui.TableSetupColumn(ImU8String.op_Implicit("添加"), (ImGuiTableColumnFlags)8, ImGui.GetTextLineHeightWithSpacing(), 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("名称"), (ImGuiTableColumnFlags)0, 30f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("地区"), (ImGuiTableColumnFlags)0, 30f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("职业"), (ImGuiTableColumnFlags)0, 30f, 0u);
			ImGui.TableNextRow((ImGuiTableRowFlags)1);
			ImGui.TableNextColumn();
			if (ImGuiOm.ButtonIconSelectable("AddNewPreset", (FontAwesomeIcon)61543))
			{
				ImGui.OpenPopup(ImU8String.op_Implicit("AddNewPresetPopup"), (ImGuiPopupFlags)0);
			}
			PopupDisposable val16 = ImRaii.Popup(ImU8String.op_Implicit("AddNewPresetPopup"));
			try
			{
				if (PopupDisposable.op_Implicit(val16))
				{
					vector = KnownColor.Chocolate.ToVector4();
					val14 = new ImU8String(1, 1);
					((ImU8String)(ref val14)).AppendFormatted<string>(LanguageManager.Get("AutoCheckFoodUsage-AddNewPreset"));
					((ImU8String)(ref val14)).AppendLiteral(":");
					ImGui.TextColored(ref vector, val14);
					val2 = ImRaii.PushIndent(1, true);
					try
					{
						ImGui.AlignTextToFramePadding();
						ImU8String val17 = new ImU8String(1, 1);
						((ImU8String)(ref val17)).AppendFormatted<string>(LanguageManager.Get("Food"));
						((ImU8String)(ref val17)).AppendLiteral(":");
						ImGui.TextUnformatted(val17);
						ImGui.SameLine();
						ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
						ImGuiOmExtension.SingleSelectCombo<Item>("FoodSelectCombo", (IDictionary<uint, Item>)Sheets.Food, ref selectedItem, ref selectItemSearch, (Func<Item, string>)((Item x) => $"{((object)((Item)(ref x)).Name/*cast due to constrained. prefix*/).ToString()} ({((Item)(ref x)).RowId})"), new(string, ImGuiTableColumnFlags, float)[1] { ("物品", (ImGuiTableColumnFlags)4, 0f) }, new Func<Item, Action>[1]
						{
							(Item x) => delegate
							{
								//IL_001b: Unknown result type (might be due to invalid IL or missing references)
								//IL_0030: Unknown result type (might be due to invalid IL or missing references)
								//IL_0035: Unknown result type (might be due to invalid IL or missing references)
								if (ImGuiOm.SelectableImageWithText(ImageHelper.GetGameIcon(((Item)(ref x)).Icon, selectItemIsHQ).Handle, Globals.ScaledVector2(20f), ((object)((Item)(ref x)).Name/*cast due to constrained. prefix*/).ToString(), ((Item)(ref x)).RowId == selectedItem, (ImGuiSelectableFlags)1))
								{
									selectedItem = ((selectedItem != ((Item)(ref x)).RowId) ? ((Item)(ref x)).RowId : 0u);
								}
							}
						}, new Func<Item, string>[2]
						{
							(Item x) => ((object)((Item)(ref x)).Name/*cast due to constrained. prefix*/).ToString(),
							(Item x) => ((Item)(ref x)).RowId.ToString()
						}, true);
						ImGui.SameLine();
						ImGui.Checkbox(ImU8String.op_Implicit("HQ"), ref selectItemIsHQ);
						ImGui.SameLine();
						DisabledDisposable val18 = ImRaii.Disabled(selectedItem == 0);
						try
						{
							if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("Add")), default(Vector2)))
							{
								FoodUsagePreset item = new FoodUsagePreset(selectedItem, selectItemIsHQ);
								config.Presets.Add(item);
								config.Save(this);
							}
						}
						finally
						{
							((IDisposable)val18)?.Dispose();
						}
					}
					finally
					{
						((IDisposable)val2)?.Dispose();
					}
				}
			}
			finally
			{
				((PopupDisposable)(ref val16)).Dispose();
			}
			ImGui.TableNextColumn();
			ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("Food")));
			ImGui.TableNextColumn();
			ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("AutoCheckFoodUsage-ZoneRestrictions")));
			ImGui.TableNextColumn();
			ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("AutoCheckFoodUsage-JobRestrictions")));
			for (int num = 0; num < config.Presets.Count; num++)
			{
				FoodUsagePreset foodUsagePreset = config.Presets[num];
				ImU8String val19 = new ImU8String(2, 3);
				((ImU8String)(ref val19)).AppendFormatted<uint>(foodUsagePreset.ItemID);
				((ImU8String)(ref val19)).AppendLiteral("_");
				((ImU8String)(ref val19)).AppendFormatted<bool>(foodUsagePreset.IsHQ);
				((ImU8String)(ref val19)).AppendLiteral("_");
				((ImU8String)(ref val19)).AppendFormatted<int>(num);
				IdDisposable val20 = ImRaii.PushId(val19, true);
				try
				{
					ImGui.TableNextRow();
					ImGui.TableNextColumn();
					bool enabled = foodUsagePreset.Enabled;
					if (ImGui.Checkbox(ImU8String.op_Implicit(""), ref enabled))
					{
						foodUsagePreset.Enabled = enabled;
						config.Save(this);
					}
					ImGui.TableNextColumn();
					ImU8String val21 = new ImU8String(1, 2);
					Item value2 = LuminaGetter.GetRow<Item>(foodUsagePreset.ItemID).Value;
					((ImU8String)(ref val21)).AppendFormatted<string>(((object)((Item)(ref value2)).Name/*cast due to constrained. prefix*/).ToString());
					((ImU8String)(ref val21)).AppendLiteral(" ");
					((ImU8String)(ref val21)).AppendFormatted<string>(foodUsagePreset.IsHQ ? "(HQ)" : "");
					ImGui.Selectable(val21, false, (ImGuiSelectableFlags)0, default(Vector2));
					PopupDisposable val22 = ImRaii.ContextPopupItem(ImU8String.op_Implicit("PresetContextMenu"));
					try
					{
						if (PopupDisposable.op_Implicit(val22))
						{
							if (num != 0 && ImGui.MenuItem(ImU8String.op_Implicit(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61698)), false, true))
							{
								List<FoodUsagePreset> presets = config.Presets;
								int i = num;
								List<FoodUsagePreset> presets2 = config.Presets;
								int index = num - 1;
								FoodUsagePreset value3 = config.Presets[num - 1];
								FoodUsagePreset value4 = config.Presets[num];
								presets[i] = value3;
								presets2[index] = value4;
								config.Save(this);
							}
							if (num != config.Presets.Count - 1 && ImGui.MenuItem(ImU8String.op_Implicit(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61699)), false, true))
							{
								List<FoodUsagePreset> presets = config.Presets;
								int index = num;
								List<FoodUsagePreset> presets3 = config.Presets;
								int i = num + 1;
								FoodUsagePreset value4 = config.Presets[num + 1];
								FoodUsagePreset value3 = config.Presets[num];
								presets[index] = value4;
								presets3[i] = value3;
								config.Save(this);
							}
							ImGui.Separator();
							ImU8String val23 = new ImU8String(1, 2);
							((ImU8String)(ref val23)).AppendFormatted<string>(LanguageManager.Get("AutoCheckFoodUsage-ChangeTo"));
							((ImU8String)(ref val23)).AppendLiteral(" ");
							((ImU8String)(ref val23)).AppendFormatted<string>(foodUsagePreset.IsHQ ? "NQ" : "HQ");
							if (ImGui.MenuItem(val23, false, true))
							{
								foodUsagePreset.IsHQ = !foodUsagePreset.IsHQ;
								config.Save(this);
							}
							if (ImGui.MenuItem(ImU8String.op_Implicit(LanguageManager.Get("Delete")), false, true))
							{
								config.Presets.Remove(foodUsagePreset);
								config.Save(this);
								break;
							}
						}
					}
					finally
					{
						((PopupDisposable)(ref val22)).Dispose();
					}
					ImGui.TableNextColumn();
					HashSet<uint> zones = foodUsagePreset.Zones;
					ImGui.SetNextItemWidth(-1f);
					IdDisposable val24 = ImRaii.PushId(ImU8String.op_Implicit("ZonesSelectCombo"), true);
					try
					{
						if (ImGuiOmExtension.MultiSelectCombo<TerritoryType>("ZoneSelectCombo", (IDictionary<uint, TerritoryType>)Sheets.Zones, ref zones, ref zoneSearchInput, new(string, ImGuiTableColumnFlags, float)[2]
						{
							("区域", (ImGuiTableColumnFlags)4, 0f),
							("副本", (ImGuiTableColumnFlags)4, 0f)
						}, new Func<TerritoryType, Action>[2]
						{
							(TerritoryType x) => delegate
							{
								//IL_0039: Unknown result type (might be due to invalid IL or missing references)
								ImU8String val25 = default(ImU8String);
								((ImU8String)(ref val25))._002Ector(2, 2);
								((ImU8String)(ref val25)).AppendFormatted<string>(x.ExtractPlaceName());
								((ImU8String)(ref val25)).AppendLiteral("##");
								((ImU8String)(ref val25)).AppendFormatted<uint>(((TerritoryType)(ref x)).RowId);
								if (ImGui.Selectable(val25, zones.Contains(((TerritoryType)(ref x)).RowId), (ImGuiSelectableFlags)3, default(Vector2)) && !zones.Remove(((TerritoryType)(ref x)).RowId))
								{
									zones.Add(((TerritoryType)(ref x)).RowId);
									config.Save(this);
								}
							},
							(TerritoryType x) => delegate
							{
								//IL_0006: Unknown result type (might be due to invalid IL or missing references)
								//IL_000b: Unknown result type (might be due to invalid IL or missing references)
								//IL_000e: Unknown result type (might be due to invalid IL or missing references)
								//IL_0013: Unknown result type (might be due to invalid IL or missing references)
								//IL_0016: Unknown result type (might be due to invalid IL or missing references)
								//IL_001b: Unknown result type (might be due to invalid IL or missing references)
								//IL_0032: Unknown result type (might be due to invalid IL or missing references)
								ContentFinderCondition value5 = ((TerritoryType)(ref x)).ContentFinderCondition.Value;
								ImGui.TextUnformatted(ImU8String.op_Implicit(((object)((ContentFinderCondition)(ref value5)).Name/*cast due to constrained. prefix*/).ToString() ?? ""));
							}
						}, new Func<TerritoryType, string>[2]
						{
							(TerritoryType x) => x.ExtractPlaceName(),
							delegate(TerritoryType x)
							{
								//IL_0002: Unknown result type (might be due to invalid IL or missing references)
								//IL_0007: Unknown result type (might be due to invalid IL or missing references)
								//IL_000a: Unknown result type (might be due to invalid IL or missing references)
								//IL_000f: Unknown result type (might be due to invalid IL or missing references)
								//IL_0012: Unknown result type (might be due to invalid IL or missing references)
								//IL_0017: Unknown result type (might be due to invalid IL or missing references)
								ContentFinderCondition value5 = ((TerritoryType)(ref x)).ContentFinderCondition.Value;
								return ((object)((ContentFinderCondition)(ref value5)).Name/*cast due to constrained. prefix*/).ToString() ?? "";
							}
						}, true))
						{
							foodUsagePreset.Zones = zones;
							config.Save(this);
						}
						ImGuiOm.TooltipHover(LanguageManager.Get("AutoCheckFoodUsage-NoZoneSelectHelp"));
					}
					finally
					{
						((IDisposable)val24)?.Dispose();
					}
					ImGui.TableNextColumn();
					ImGui.SetNextItemWidth(-1f);
					jobSelectCombo.SelectedIDs = foodUsagePreset.ClassJobs.ToHashSet();
					if (jobSelectCombo.DrawCheckbox())
					{
						foodUsagePreset.ClassJobs = jobSelectCombo.SelectedIDs.ToHashSet();
						config.Save(this);
					}
				}
				finally
				{
					((IDisposable)val20)?.Dispose();
				}
			}
		}
		finally
		{
			((TableDisposable)(ref val15)).Dispose();
		}
	}

	private bool EnqueueFoodRefresh()
	{
		if (!IsValidState())
		{
			return false;
		}
		if (!IsCooldownElapsed())
		{
			base.TaskHelper.Abort();
			return true;
		}
		List<FoodUsagePreset> validPresets = GetValidPresets();
		if (validPresets.Count == 0)
		{
			base.TaskHelper.Abort();
			return true;
		}
		if (TryGetWellFedParam(out var itemFood, out var remainingTime) && validPresets.FirstOrDefault((FoodUsagePreset x) => ToFoodRowID(x.ItemID) == itemFood) != null && !ShouldRefreshFood(remainingTime))
		{
			base.TaskHelper.Abort();
			return true;
		}
		FoodUsagePreset finalPreset = validPresets.FirstOrDefault();
		base.TaskHelper.Enqueue(() => TakeFood(finalPreset));
		return true;
	}

	private bool TakeFood(FoodUsagePreset preset)
	{
		return TakeFood(preset.ItemID, preset.IsHQ);
	}

	private bool TakeFood(uint itemID, bool isHQ)
	{
		if (!Throttler.Shared.Throttle("AutoCheckFoodUsage-TakeFood", 1000u))
		{
			return false;
		}
		if (!IsValidState())
		{
			return false;
		}
		base.TaskHelper.Enqueue(() => TakeFoodInternal(itemID, isHQ));
		return true;
	}

	private bool TakeFoodInternal(uint itemID, bool isHQ)
	{
		base.TaskHelper.Abort();
		if (TryGetWellFedParam(out var itemFoodRowID, out var remainingTime) && itemFoodRowID == ToFoodRowID(itemID) && remainingTime.TotalMinutes >= 25.0)
		{
			return true;
		}
		OmenServiceBase<UseActionManager>.Instance().UseActionLocation((ActionType)2, isHQ ? (itemID + 1000000) : itemID, 3758096384uL, default(Vector3), 65535u, 0);
		base.TaskHelper.DelayNext(3000);
		base.TaskHelper.Enqueue(() => CheckFoodState(itemID, isHQ));
		return true;
	}

	private bool CheckFoodState(uint itemID, bool isHQ)
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		base.TaskHelper.Abort();
		if (TryGetWellFedParam(out var itemFoodRowID, out var remainingTime) && itemFoodRowID == ToFoodRowID(itemID) && remainingTime.TotalMinutes >= 25.0)
		{
			OmenServiceBase<NotifyHelper>.Instance().Chat(LanguageManager.GetSe("AutoCheckFoodUsage-NoticeMessage", new SeStringBuilder().AddItemLink(itemID, isHQ, (string)null)));
			return true;
		}
		base.TaskHelper.DelayNext(3000);
		base.TaskHelper.Enqueue(() => TakeFoodInternal(itemID, isHQ));
		return false;
	}

	private unsafe static bool TryGetWellFedParam(out uint itemFoodRowID, out TimeSpan remainingTime)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		itemFoodRowID = 0u;
		remainingTime = TimeSpan.Zero;
		IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
		if (localPlayer == null)
		{
			return false;
		}
		StatusManager statusManager = ((BattleChara)localPlayer.ToStruct()).StatusManager;
		int statusIndex = ((StatusManager)(ref statusManager)).GetStatusIndex(48u, 3758096384u);
		if (statusIndex == -1)
		{
			return false;
		}
		Status val = ((StatusManager)(ref statusManager)).Status[statusIndex];
		itemFoodRowID = (uint)val.Param % 10000u;
		remainingTime = TimeSpan.FromSeconds(val.RemainingTime);
		return true;
	}

	private nint CountdownInitDetour(nint a1, nint a2)
	{
		nint result = CountdownInitHook.Original(a1, a2);
		if (config.EnabledCheckpoints[FoodCheckpoint.倒计时开始时] && !GameMain.IsInPvPArea())
		{
			base.TaskHelper.Abort();
			base.TaskHelper.Enqueue((Func<bool>)EnqueueFoodRefresh, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
		}
		return result;
	}

	private void OnZoneChanged(uint u)
	{
		if (config.EnabledCheckpoints[FoodCheckpoint.区域切换时] && !GameMain.IsInPvPArea())
		{
			base.TaskHelper.Abort();
			base.TaskHelper.Enqueue((Func<bool>)EnqueueFoodRefresh, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
		}
	}

	private void OnConditionChanged(ConditionFlag flag, bool value)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (config.EnabledCheckpoints[FoodCheckpoint.条件变更时] && !GameMain.IsInPvPArea() && ((value && config.ConditionStart.Contains(flag)) || (!value && config.ConditionEnd.Contains(flag))))
		{
			base.TaskHelper.Abort();
			base.TaskHelper.Enqueue((Func<bool>)EnqueueFoodRefresh, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
		}
	}

	private unsafe static bool IsValidState()
	{
		if (!IConditionExtension.get_IsBetweenAreas(DalamudServiceExtension.Instance<ICondition>()) && !IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()) && !IConditionExtension.get_IsCasting(DalamudServiceExtension.Instance<ICondition>()) && IOmenDalamudService<IObjectTable>.Instance().LocalPlayer != null && UIModuleExtension.IsScreenReady())
		{
			return ((ActionManager)ActionManager.Instance()).GetActionStatus((ActionType)5, 2u, 3758096384uL, true, true, (uint*)null) == 0;
		}
		return false;
	}

	private bool IsCooldownElapsed()
	{
		return (OmenServiceBase<StandardTimeManager>.Instance().Now - lastFoodUsageTime).TotalSeconds >= 10.0;
	}

	private static uint ToFoodRowID(uint id)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		Item rowOrDefault = LuminaGetter.GetRowOrDefault<Item>(id);
		ItemAction value = ((Item)(ref rowOrDefault)).ItemAction.Value;
		ItemFood? row = LuminaGetter.GetRow<ItemFood>((uint)((ItemAction)(ref value)).Data[1]);
		if (!row.HasValue)
		{
			return 0u;
		}
		ItemFood valueOrDefault = row.GetValueOrDefault();
		return ((ItemFood)(ref valueOrDefault)).RowId;
	}

	private unsafe List<FoodUsagePreset> GetValidPresets()
	{
		InventoryManager* instance = InventoryManager.Instance();
		uint zone = GameState.TerritoryType;
		if (instance == null || zone == 0)
		{
			return new List<FoodUsagePreset>();
		}
		return (from x in config.Presets
			where x.Enabled && (x.Zones.Count == 0 || x.Zones.Contains(zone)) && (x.ClassJobs.Count == 0 || x.ClassJobs.Contains(IOmenDalamudService<IObjectTable>.Instance().LocalPlayer.ClassJob.RowId)) && ((InventoryManager)instance).GetInventoryItemCount(x.ItemID, x.IsHQ, true, true, (short)0) > 0
			orderby x.Zones.Contains(zone) descending
			select x).ToList();
	}

	private bool ShouldRefreshFood(TimeSpan remainingTime)
	{
		if (remainingTime <= TimeSpan.FromSeconds(config.RefreshThreshold))
		{
			return remainingTime <= TimeSpan.FromMinutes(55L);
		}
		return false;
	}
}
