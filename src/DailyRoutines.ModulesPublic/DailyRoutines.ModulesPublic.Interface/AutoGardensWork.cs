using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using DailyRoutines.Common.Interface.Windows;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Common.Math;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Interop;
using Lumina.Excel.Sheets;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Info.Game.Data;
using OmenTools.Info.Game.Packets.Upstream;
using OmenTools.Info.Lumina;
using OmenTools.Info.Lumina.ExtraSheets;
using OmenTools.Interop.Game.AddonEvent;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;

namespace DailyRoutines.ModulesPublic.Interface;

public class AutoGardensWork : ModuleBase
{
	private class Config : ModuleConfig
	{
		public uint FertilizerSelected;

		public uint SeedSelected;

		public uint SoilSelected;
	}

	private Config config;

	private string searchSeed = string.Empty;

	private string searchSoil = string.Empty;

	private string searchFertilize = string.Empty;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoGardensWorkTitle"),
		Description = LanguageManager.Get("AutoGardensWorkDescription"),
		Category = ModuleCategory.Interface,
		ModulesPrerequisite = new string[1] { "AutoTalkSkip" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		NeedAuth = true
	};

	protected override void Init()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
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
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)1, "HousingGardening", new AddonEventDelegate(OnAddon));
		OmenServiceBase<TargetManager>.Instance().RegPostSetHardTarget(OnSetHardTarget);
	}

	protected override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddon });
		OmenServiceBase<TargetManager>.Instance().Unreg(OnSetHardTarget);
	}

	protected override void ConfigUI()
	{
		DrawAutoPlant();
		ImGui.NewLine();
		DrawAutoGather();
		ImGui.NewLine();
		DrawAutoFertilize();
		ImGui.NewLine();
		DrawAutoTend();
	}

	private void DrawAutoPlant()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		IdDisposable val = ImRaii.PushId(ImU8String.op_Implicit("AutoPlant"), true);
		try
		{
			Vector4 vector = KnownColor.Chocolate.ToVector4();
			ImU8String val2 = new ImU8String(0, 1);
			((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get("AutoGardensWork-AutoPlant"));
			ImGui.TextColored(ref vector, val2);
			IndentDisposable val3 = ImRaii.PushIndent(1, true);
			try
			{
				DisabledDisposable val4 = ImRaii.Disabled(base.TaskHelper?.IsBusy ?? true);
				try
				{
					ImU8String val5 = new ImU8String(1, 2);
					((ImU8String)(ref val5)).AppendFormatted<string>(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61515));
					((ImU8String)(ref val5)).AppendLiteral(" ");
					((ImU8String)(ref val5)).AppendFormatted<string>(LanguageManager.Get("Start"));
					if (ImGui.Button(val5, default(Vector2)))
					{
						StartPlant();
					}
				}
				finally
				{
					((IDisposable)val4)?.Dispose();
				}
				ImGui.SameLine();
				ImU8String val6 = new ImU8String(1, 2);
				((ImU8String)(ref val6)).AppendFormatted<string>(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61517));
				((ImU8String)(ref val6)).AppendLiteral(" ");
				((ImU8String)(ref val6)).AppendFormatted<string>(LanguageManager.Get("Stop"));
				if (ImGui.Button(val6, default(Vector2)))
				{
					base.TaskHelper.Abort();
				}
				ImGui.Spacing();
				ImGui.SetNextItemWidth(300f * Globals.GlobalUIScale);
				if (ImGuiOmExtension.SingleSelectCombo<Item>("SeedSelectCombo", (IDictionary<uint, Item>)Sheets.Seeds, ref config.SeedSelected, ref searchSeed, (Func<Item, string>)((Item x) => $"{((object)((Item)(ref x)).Name/*cast due to constrained. prefix*/).ToString()} ({((Item)(ref x)).RowId})"), new(string, ImGuiTableColumnFlags, float)[1] { (LuminaWrapper.GetAddonText(6412u), (ImGuiTableColumnFlags)4, 0f) }, new Func<Item, Action>[1]
				{
					(Item x) => delegate
					{
						//IL_0011: Unknown result type (might be due to invalid IL or missing references)
						//IL_0026: Unknown result type (might be due to invalid IL or missing references)
						//IL_002b: Unknown result type (might be due to invalid IL or missing references)
						if (ImGuiOm.SelectableImageWithText(ImageHelper.GetGameIcon(((Item)(ref x)).Icon).Handle, new Vector2(ImGui.GetTextLineHeightWithSpacing()), ((object)((Item)(ref x)).Name/*cast due to constrained. prefix*/).ToString(), ((Item)(ref x)).RowId == config.SeedSelected, (ImGuiSelectableFlags)3))
						{
							config.SeedSelected = ((Item)(ref x)).RowId;
						}
					}
				}, new Func<Item, string>[2]
				{
					(Item x) => ((object)((Item)(ref x)).Name/*cast due to constrained. prefix*/).ToString(),
					(Item x) => ((Item)(ref x)).RowId.ToString()
				}, true))
				{
					config.Save(this);
				}
				ImGui.SameLine();
				ImGui.TextUnformatted(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(6412u)));
				ImGui.SetNextItemWidth(300f * Globals.GlobalUIScale);
				IdDisposable val7 = ImRaii.PushId(ImU8String.op_Implicit("SoilSelectCombo"), true);
				try
				{
					if (ImGuiOmExtension.SingleSelectCombo<Item>("SoilSelectCombo", (IDictionary<uint, Item>)Sheets.Soils, ref config.SoilSelected, ref searchSoil, (Func<Item, string>)((Item x) => $"{((object)((Item)(ref x)).Name/*cast due to constrained. prefix*/).ToString()} ({((Item)(ref x)).RowId})"), new(string, ImGuiTableColumnFlags, float)[1] { (LuminaWrapper.GetAddonText(6411u), (ImGuiTableColumnFlags)4, 0f) }, new Func<Item, Action>[1]
					{
						(Item x) => delegate
						{
							//IL_0011: Unknown result type (might be due to invalid IL or missing references)
							//IL_0026: Unknown result type (might be due to invalid IL or missing references)
							//IL_002b: Unknown result type (might be due to invalid IL or missing references)
							if (ImGuiOm.SelectableImageWithText(ImageHelper.GetGameIcon(((Item)(ref x)).Icon).Handle, new Vector2(ImGui.GetTextLineHeightWithSpacing()), ((object)((Item)(ref x)).Name/*cast due to constrained. prefix*/).ToString(), ((Item)(ref x)).RowId == config.SeedSelected, (ImGuiSelectableFlags)1))
							{
								config.SoilSelected = ((Item)(ref x)).RowId;
							}
						}
					}, new Func<Item, string>[2]
					{
						(Item x) => ((object)((Item)(ref x)).Name/*cast due to constrained. prefix*/).ToString(),
						(Item x) => ((Item)(ref x)).RowId.ToString()
					}, true))
					{
						config.Save(this);
					}
				}
				finally
				{
					((IDisposable)val7)?.Dispose();
				}
				ImGui.SameLine();
				ImGui.TextUnformatted(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(6411u)));
			}
			finally
			{
				((IDisposable)val3)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void DrawAutoGather()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		IdDisposable val = ImRaii.PushId(ImU8String.op_Implicit("AutoGather"), true);
		try
		{
			Vector4 vector = KnownColor.Chocolate.ToVector4();
			ImU8String val2 = new ImU8String(0, 1);
			((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get("AutoGardensWork-AutoGather"));
			ImGui.TextColored(ref vector, val2);
			IndentDisposable val3 = ImRaii.PushIndent(1, true);
			try
			{
				DisabledDisposable val4 = ImRaii.Disabled(base.TaskHelper?.IsBusy ?? true);
				try
				{
					ImU8String val5 = new ImU8String(1, 2);
					((ImU8String)(ref val5)).AppendFormatted<string>(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61515));
					((ImU8String)(ref val5)).AppendLiteral(" ");
					((ImU8String)(ref val5)).AppendFormatted<string>(LanguageManager.Get("Start"));
					if (ImGui.Button(val5, default(Vector2)))
					{
						StartGather();
					}
				}
				finally
				{
					((IDisposable)val4)?.Dispose();
				}
				ImGui.SameLine();
				ImU8String val6 = new ImU8String(1, 2);
				((ImU8String)(ref val6)).AppendFormatted<string>(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61517));
				((ImU8String)(ref val6)).AppendLiteral(" ");
				((ImU8String)(ref val6)).AppendFormatted<string>(LanguageManager.Get("Stop"));
				if (ImGui.Button(val6, default(Vector2)))
				{
					base.TaskHelper.Abort();
				}
			}
			finally
			{
				((IDisposable)val3)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void DrawAutoFertilize()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		IdDisposable val = ImRaii.PushId(ImU8String.op_Implicit("AutoFertilize"), true);
		try
		{
			Vector4 vector = KnownColor.Chocolate.ToVector4();
			ImU8String val2 = new ImU8String(0, 1);
			((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get("AutoGardensWork-AutoFertilize"));
			ImGui.TextColored(ref vector, val2);
			IndentDisposable val3 = ImRaii.PushIndent(1, true);
			try
			{
				DisabledDisposable val4 = ImRaii.Disabled(base.TaskHelper?.IsBusy ?? true);
				try
				{
					ImU8String val5 = new ImU8String(1, 2);
					((ImU8String)(ref val5)).AppendFormatted<string>(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61515));
					((ImU8String)(ref val5)).AppendLiteral(" ");
					((ImU8String)(ref val5)).AppendFormatted<string>(LanguageManager.Get("Start"));
					if (ImGui.Button(val5, default(Vector2)))
					{
						StartFertilize();
					}
				}
				finally
				{
					((IDisposable)val4)?.Dispose();
				}
				ImGui.SameLine();
				ImU8String val6 = new ImU8String(1, 2);
				((ImU8String)(ref val6)).AppendFormatted<string>(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61517));
				((ImU8String)(ref val6)).AppendLiteral(" ");
				((ImU8String)(ref val6)).AppendFormatted<string>(LanguageManager.Get("Stop"));
				if (ImGui.Button(val6, default(Vector2)))
				{
					base.TaskHelper.Abort();
				}
				ImGui.Spacing();
				ImGui.SetNextItemWidth(300f * Globals.GlobalUIScale);
				if (ImGuiOmExtension.SingleSelectCombo<Item>("FertilizersSelectCombo", (IDictionary<uint, Item>)Sheets.Fertilizers, ref config.FertilizerSelected, ref searchFertilize, (Func<Item, string>)((Item x) => $"{((object)((Item)(ref x)).Name/*cast due to constrained. prefix*/).ToString()} ({((Item)(ref x)).RowId})"), new(string, ImGuiTableColumnFlags, float)[1] { (LuminaWrapper.GetAddonText(6417u), (ImGuiTableColumnFlags)4, 0f) }, new Func<Item, Action>[1]
				{
					(Item x) => delegate
					{
						//IL_0011: Unknown result type (might be due to invalid IL or missing references)
						//IL_0026: Unknown result type (might be due to invalid IL or missing references)
						//IL_002b: Unknown result type (might be due to invalid IL or missing references)
						if (ImGuiOm.SelectableImageWithText(ImageHelper.GetGameIcon(((Item)(ref x)).Icon).Handle, new Vector2(ImGui.GetTextLineHeightWithSpacing()), ((object)((Item)(ref x)).Name/*cast due to constrained. prefix*/).ToString(), ((Item)(ref x)).RowId == config.SeedSelected, (ImGuiSelectableFlags)3))
						{
							config.FertilizerSelected = ((Item)(ref x)).RowId;
						}
					}
				}, new Func<Item, string>[2]
				{
					(Item x) => ((object)((Item)(ref x)).Name/*cast due to constrained. prefix*/).ToString(),
					(Item x) => ((Item)(ref x)).RowId.ToString()
				}, true))
				{
					config.Save(this);
				}
				ImGui.SameLine();
				ImGui.TextUnformatted(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(6417u)));
			}
			finally
			{
				((IDisposable)val3)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void DrawAutoTend()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		IdDisposable val = ImRaii.PushId(ImU8String.op_Implicit("AutoTend"), true);
		try
		{
			Vector4 vector = KnownColor.Chocolate.ToVector4();
			ImU8String val2 = new ImU8String(0, 1);
			((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get("AutoGardensWork-AutoTend"));
			ImGui.TextColored(ref vector, val2);
			IndentDisposable val3 = ImRaii.PushIndent(1, true);
			try
			{
				DisabledDisposable val4 = ImRaii.Disabled(base.TaskHelper?.IsBusy ?? true);
				try
				{
					ImU8String val5 = new ImU8String(1, 2);
					((ImU8String)(ref val5)).AppendFormatted<string>(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61515));
					((ImU8String)(ref val5)).AppendLiteral(" ");
					((ImU8String)(ref val5)).AppendFormatted<string>(LanguageManager.Get("Start"));
					if (ImGui.Button(val5, default(Vector2)))
					{
						StartTend();
					}
				}
				finally
				{
					((IDisposable)val4)?.Dispose();
				}
				ImGui.SameLine();
				ImU8String val6 = new ImU8String(1, 2);
				((ImU8String)(ref val6)).AppendFormatted<string>(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61517));
				((ImU8String)(ref val6)).AppendLiteral(" ");
				((ImU8String)(ref val6)).AppendFormatted<string>(LanguageManager.Get("Stop"));
				if (ImGui.Button(val6, default(Vector2)))
				{
					base.TaskHelper.Abort();
				}
			}
			finally
			{
				((IDisposable)val3)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private unsafe void OnAddon(AddonEvent type, AddonArgs args)
	{
		if (config.SeedSelected == 0 || config.SoilSelected == 0 || !((IEnumerable<InventoryType>)Inventories.Player).TryGetFirstItem((Predicate<InventoryItem>)((InventoryItem x) => x.ItemId == config.SeedSelected), out InventoryItem* seedItem) || !((IEnumerable<InventoryType>)Inventories.Player).TryGetFirstItem((Predicate<InventoryItem>)((InventoryItem x) => x.ItemId == config.SoilSelected), out InventoryItem* soilItem))
		{
			return;
		}
		base.TaskHelper.Enqueue(delegate
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			AgentHousingPlant* ptr = AgentHousingPlant.Instance();
			if (ptr != null)
			{
				((AgentHousingPlant)ptr).SelectedItems[0] = new SelectedItem
				{
					ItemId = ((InventoryItem)soilItem).ItemId,
					InventoryType = ((InventoryItem)soilItem).Container,
					InventorySlot = (ushort)((InventoryItem)soilItem).Slot
				};
				((AgentHousingPlant)ptr).SelectedItems[1] = new SelectedItem
				{
					ItemId = ((InventoryItem)seedItem).ItemId,
					InventoryType = ((InventoryItem)seedItem).Container,
					InventorySlot = (ushort)((InventoryItem)seedItem).Slot
				};
				((AgentHousingPlant)ptr).ConfirmSeedAndSoilSelection();
			}
		}, null, 0, null, null, null, null, 2);
		base.TaskHelper.Enqueue(() => AddonSelectYesnoEvent.ClickYes(), null, 0, null, null, null, null, 2);
	}

	private unsafe void OnSetHardTarget(bool result, IGameObject? target, bool checkMode, bool a4, int a5)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Invalid comparison between Unknown and I4
		if (((HousingManager)HousingManager.Instance()).OutdoorTerritory == null)
		{
			return;
		}
		if (target != null)
		{
			if ((int)target.ObjectKind == 7 && target.DataID == 2003757)
			{
				ToggleOverlayConfig(true);
				return;
			}
		}
		else
		{
			OverlayConfig? overlayConfig = base.OverlayConfig;
			if (overlayConfig != null && ((Window)overlayConfig).IsOpen)
			{
				ToggleOverlayConfig(false);
				return;
			}
		}
		ToggleOverlayConfig(false);
	}

	private void StartAction(string entryKeyword, Action extraAction = null)
	{
		if (!IsEnvironmentValid(out List<ulong> objectIDs))
		{
			return;
		}
		base.TaskHelper.Enqueue(delegate
		{
			TargetManager.Target = null;
		}, "移除当前目标");
		foreach (ulong garden in objectIDs)
		{
			base.TaskHelper.Enqueue(delegate
			{
				new EventStartPackt(garden, 721047u).Send();
			}, $"交互园圃: {garden}");
			base.TaskHelper.Enqueue(() => ClickGardenEntryByText(entryKeyword), "点击");
			extraAction?.Invoke();
			base.TaskHelper.Enqueue(() => !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)32], "等待退出交互状态");
		}
	}

	private void StartGather()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		StartAction(((object)LuminaGetter.GetRowOrDefault<HousingGardeningPlant>(6u).Text/*cast due to constrained. prefix*/).ToString());
	}

	private void StartTend()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		StartAction(((object)LuminaGetter.GetRowOrDefault<HousingGardeningPlant>(4u).Text/*cast due to constrained. prefix*/).ToString());
	}

	private void StartPlant()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		StartAction(((object)LuminaGetter.GetRowOrDefault<HousingGardeningPlant>(2u).Text/*cast due to constrained. prefix*/).ToString(), delegate
		{
			base.TaskHelper.DelayNext(250);
		});
	}

	private void StartFertilize()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		StartAction(((object)LuminaGetter.GetRowOrDefault<HousingGardeningPlant>(3u).Text/*cast due to constrained. prefix*/).ToString(), delegate
		{
			base.TaskHelper.Enqueue((Func<bool>)CheckFertilizerState, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
			base.TaskHelper.Enqueue((Func<bool>)ClickFertilizer, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
			base.TaskHelper.Enqueue(() => !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)32]);
		});
	}

	private unsafe static bool IsEnvironmentValid(out List<ulong> objectIDs)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Invalid comparison between Unknown and I4
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		objectIDs = new List<ulong>();
		if (IOmenDalamudService<IObjectTable>.Instance().LocalPlayer == null)
		{
			return false;
		}
		HousingManager* ptr = HousingManager.Instance();
		if (ptr == null)
		{
			return false;
		}
		if (((HousingManager)ptr).OutdoorTerritory == null)
		{
			return false;
		}
		ulong num = HouseId.op_Implicit(((HousingManager)ptr).GetCurrentHouseId());
		if (num == 0L)
		{
			return false;
		}
		EstateType[] values = Enum.GetValues<EstateType>();
		int num2 = 0;
		while (true)
		{
			if (num2 < values.Length)
			{
				EstateType val = values[num2];
				if ((int)val == 4)
				{
					for (int i = 0; i < 2; i++)
					{
						if (HousingManager.GetOwnedHouseId(val, i) == HouseId.op_Implicit(num))
						{
							goto end_IL_0094;
						}
					}
				}
				else if (HousingManager.GetOwnedHouseId(val, -1) == HouseId.op_Implicit(num))
				{
					break;
				}
				num2++;
				continue;
			}
			return false;
			continue;
			end_IL_0094:
			break;
		}
		return TryObtainGardensAround(out objectIDs);
	}

	private unsafe static bool TryObtainGardensAround(out List<ulong> objectIDs)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Invalid comparison between Unknown and I4
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		objectIDs = new List<ulong>();
		OutdoorTerritory* outdoorTerritory = ((HousingManager)HousingManager.Instance()).OutdoorTerritory;
		if (outdoorTerritory == null)
		{
			return false;
		}
		List<(ulong GameObjectID, Vector3 Position)> gardenCenters = new List<(ulong, Vector3)>();
		Span<Pointer<GameObject>> objects = ((HousingObjectArray)(&((HousingObjectManager)(&((HousingFurnitureManager)(&((OutdoorTerritory)outdoorTerritory).FurnitureManager)).ObjectManager)).ObjectArray)).Objects;
		for (int i = 0; i < objects.Length; i++)
		{
			Pointer<GameObject> val = objects[i];
			if (!(val == Pointer<GameObject>.op_Implicit((GameObject*)null)) && val.Value != null && (int)((GameObject)val.Value).ObjectKind == 12 && ((GameObject)val.Value).BaseId == 131128 && !(LocalPlayerState.DistanceTo3D(Vector3.op_Implicit(((GameObject)val.Value).Position)) > 10f))
			{
				gardenCenters.Add((GameObjectId.op_Implicit(((GameObject)val.Value).GetGameObjectId()), Vector3.op_Implicit(((GameObject)val.Value).Position)));
			}
		}
		if (gardenCenters.Count == 0)
		{
			return false;
		}
		objectIDs = (from x in IOmenDalamudService<IObjectTable>.Instance()
			where x != null && (int)x.ObjectKind == 7 && x.DataID == 2003757 && gardenCenters.Any(((ulong GameObjectID, Vector3 Position) g) => Vector3.DistanceSquared(x.Position, g.Position) <= 25f)
			select x.GameObjectID).ToList();
		return objectIDs.Count > 0;
	}

	private unsafe static bool CheckFertilizerState()
	{
		if (Addons.SelectString != null)
		{
			return false;
		}
		if (!((AtkUnitBase)Addons.Inventory).IsVisible && !((AtkUnitBase)Addons.InventoryLarge).IsVisible && !((AtkUnitBase)Addons.InventoryExpansion).IsVisible)
		{
			return !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)32];
		}
		return true;
	}

	private unsafe bool ClickFertilizer()
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		if (Addons.SelectString != null)
		{
			return false;
		}
		if (!DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)32])
		{
			return true;
		}
		if (config.FertilizerSelected == 0 || !((IEnumerable<InventoryType>)Inventories.Player).TryGetFirstItem((Predicate<InventoryItem>)((InventoryItem x) => x.ItemId == config.FertilizerSelected), out InventoryItem* itemResult))
		{
			base.TaskHelper.Abort();
			return true;
		}
		((AgentInventoryContext)AgentInventoryContext.Instance()).OpenForItemSlot(((InventoryItem)itemResult).Container, (int)((InventoryItem)itemResult).Slot, 0, ((AgentInterface)((AgentModule)AgentModule.Instance()).GetAgentByInternalId((AgentId)6)).AddonId);
		base.TaskHelper.Enqueue(() => AddonContextMenuEvent.Select(((object)LuminaGetter.GetRowOrDefault<HousingGardeningPlant>(3u).Text/*cast due to constrained. prefix*/).ToString()), null, 0, null, null, null, null, 2);
		return true;
	}

	private unsafe static bool ClickGardenEntryByText(string text)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		if (!(*Addons.SelectString).IsAddonAndNodesReady())
		{
			return false;
		}
		if (!AddonSelectStringEvent.TryScanSelectStringText(text, out var index))
		{
			AddonSelectStringEvent.TryScanSelectStringText(((object)LuminaGetter.GetRowOrDefault<HousingGardeningPlant>(1u).Text/*cast due to constrained. prefix*/).ToString(), out index);
		}
		return AddonSelectStringEvent.Select(index);
	}
}
