using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Interface;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.Sheets;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm.Widgets.Combos;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class BetterMountRoulette : ModuleBase
{
	private class Config : ModuleConfig
	{
		public HashSet<uint> NormalRouletteMounts = new HashSet<uint>();

		public HashSet<uint> PVPRouletteMounts = new HashSet<uint>();

		public Dictionary<uint, HashSet<uint>> ZoneRouletteMounts = new Dictionary<uint, HashSet<uint>>();
	}

	private class MountListHandler(LuminaSearcher<Mount> searcher, HashSet<uint> selectedIDs)
	{
		public LuminaSearcher<Mount> Searcher { get; } = searcher;

		public HashSet<uint> SelectedIDs { get; } = selectedIDs;

		public string SearchText { get; set; } = string.Empty;

		public int DisplayCount { get; init; } = searcher.Data.Count;
	}

	private Config config;

	private ZoneSelectCombo zoneSelector;

	private LuminaSearcher<Mount>? masterMountsSearcher;

	private MountListHandler? pvpMounts;

	private MountListHandler? normalMounts;

	private Dictionary<uint, MountListHandler> zoneMountListHandlers = new Dictionary<uint, MountListHandler>();

	private HashSet<uint>? mountsListToUse;

	private static readonly HashSet<uint> MountRouletteActionIDs = new HashSet<uint> { 9u, 24u };

	private static readonly Vector4 ButtonNormalColor = ColorHelpers.WithAlpha(((ImGuiCol)21).ToVector4(), 0f);

	private static readonly Vector4 ButtonActiveColor = ColorHelpers.WithAlpha(((ImGuiCol)23).ToVector4(), 0.8f);

	private static readonly Vector4 ButtonHoveredColor = ColorHelpers.WithAlpha(((ImGuiCol)22).ToVector4(), 0.4f);

	private static readonly Vector4 ButtonSelectedColor = ColorHelpers.WithAlpha(((ImGuiCol)21).ToVector4(), 0.6f);

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("BetterMountRouletteTitle"),
		Description = LanguageManager.Get("BetterMountRouletteDescription"),
		Category = ModuleCategory.System,
		Author = new string[1] { "XSZYYS" }
	};

	protected override void Init()
	{
		zoneSelector = new ZoneSelectCombo("##BetterMountRouletteZoneSelector");
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		OmenServiceBase<UseActionManager>.Instance().RegPreUseAction(OnPreUseAction);
		DalamudServiceExtension.Instance<IClientState>().Login += OnLogin;
		if (DalamudServiceExtension.Instance<IClientState>().IsLoggedIn)
		{
			OnLogin();
		}
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += OnZoneChanged;
	}

	protected override void Uninit()
	{
		OmenServiceBase<UseActionManager>.Instance().Unreg(OnPreUseAction);
		DalamudServiceExtension.Instance<IClientState>().Login -= OnLogin;
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChanged;
		masterMountsSearcher = null;
		normalMounts = null;
		pvpMounts = null;
		zoneMountListHandlers.Clear();
		mountsListToUse = null;
	}

	protected override void ConfigUI()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (normalMounts == null || pvpMounts == null)
		{
			return;
		}
		TabBarDisposable val = ImRaii.TabBar(ImU8String.op_Implicit("##MountTabs"), (ImGuiTabBarFlags)1);
		try
		{
			if (!(!val))
			{
				DrawTab(LanguageManager.Get("General"), normalMounts);
				DrawTab("PVP", pvpMounts);
				DrawZoneTabs();
				HandleNewZoneTabAddition();
			}
		}
		finally
		{
			((TabBarDisposable)(ref val)).Dispose();
		}
	}

	private void DrawTab(string tabLabel, MountListHandler handler)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		TabItemDisposable val = ImRaii.TabItem(ImU8String.op_Implicit(tabLabel));
		try
		{
			if (!(!val))
			{
				DrawSearchAndMountsGrid(tabLabel, handler);
			}
		}
		finally
		{
			((TabItemDisposable)(ref val)).Dispose();
		}
	}

	private void DrawZoneTabs()
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		List<uint> list = new List<uint>();
		foreach (var (num2, handler) in zoneMountListHandlers)
		{
			if (!LuminaGetter.TryGetRow<TerritoryType>(num2, out TerritoryType item))
			{
				continue;
			}
			string text = item.ExtractPlaceName();
			if (string.IsNullOrEmpty(text))
			{
				continue;
			}
			bool flag = true;
			IdDisposable val = ImRaii.PushId((int)num2, true);
			try
			{
				TabItemDisposable val2 = ImRaii.TabItem(ImU8String.op_Implicit(text), ref flag);
				try
				{
					if (TabItemDisposable.op_Implicit(val2))
					{
						DrawSearchAndMountsGrid(text, handler);
					}
					if (!flag)
					{
						list.Add(num2);
					}
				}
				finally
				{
					((TabItemDisposable)(ref val2)).Dispose();
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		foreach (uint item2 in list)
		{
			zoneMountListHandlers.Remove(item2);
			config.ZoneRouletteMounts.Remove(item2);
			config.Save(this);
		}
	}

	private void HandleNewZoneTabAddition()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (zoneSelector == null)
		{
			return;
		}
		if (ImGui.TabItemButton(ImU8String.op_Implicit("+"), (ImGuiTabItemFlags)144))
		{
			ImGui.OpenPopup(ImU8String.op_Implicit("AddNewZonePopup"), (ImGuiPopupFlags)0);
		}
		PopupDisposable val = ImRaii.Popup(ImU8String.op_Implicit("AddNewZonePopup"));
		try
		{
			if (!(!val) && zoneSelector.DrawRadio())
			{
				HashSet<uint> hashSet = new HashSet<uint>();
				if (config.ZoneRouletteMounts.TryAdd(zoneSelector.SelectedID, hashSet))
				{
					zoneMountListHandlers[zoneSelector.SelectedID] = new MountListHandler(masterMountsSearcher, hashSet);
					config.Save(this);
				}
			}
		}
		finally
		{
			((PopupDisposable)(ref val)).Dispose();
		}
	}

	private void DrawSearchAndMountsGrid(string tabLabel, MountListHandler handler)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		string searchText = handler.SearchText;
		ImGui.SetNextItemWidth(-1f);
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(8, 1);
		((ImU8String)(ref val)).AppendLiteral("##Search");
		((ImU8String)(ref val)).AppendFormatted<string>(tabLabel);
		if (ImGui.InputTextWithHint(val, ImU8String.op_Implicit(LanguageManager.Get("Search")), ref searchText, 128, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null))
		{
			handler.SearchText = searchText;
			handler.Searcher.Search(searchText);
		}
		Vector2 vector = new Vector2(ImGui.GetContentRegionAvail().X, 400f * Globals.GlobalUIScale);
		ImU8String val2 = default(ImU8String);
		((ImU8String)(ref val2))._002Ector(12, 1);
		((ImU8String)(ref val2)).AppendLiteral("##MountsGrid");
		((ImU8String)(ref val2)).AppendFormatted<string>(tabLabel);
		ChildDisposable val3 = ImRaii.Child(val2, vector, true);
		try
		{
			if (!(!val3))
			{
				List<Mount> mountsToDraw = (string.IsNullOrWhiteSpace(handler.SearchText) ? handler.Searcher.Data.Where((Mount mount) => handler.SelectedIDs.Contains(((Mount)(ref mount)).RowId)).ToList() : handler.Searcher.SearchResult);
				DrawMountsGrid(mountsToDraw, handler);
			}
		}
		finally
		{
			((ChildDisposable)(ref val3)).Dispose();
		}
	}

	private void DrawMountsGrid(List<Mount> mountsToDraw, MountListHandler handler)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		if (mountsToDraw.Count == 0)
		{
			return;
		}
		float num = 150f * Globals.GlobalUIScale;
		float x = ImGui.GetContentRegionAvail().X;
		int num2 = Math.Max(1, (int)Math.Floor(x / num));
		float num3 = 3f * ImGui.GetTextLineHeightWithSpacing();
		TableDisposable val = ImRaii.Table(ImU8String.op_Implicit("##MountsGridTable"), num2, (ImGuiTableFlags)32768);
		try
		{
			if (!val)
			{
				return;
			}
			foreach (Mount item in mountsToDraw)
			{
				Mount current = item;
				if (!ImageHelper.TryGetGameIcon(((Mount)(ref current)).Icon, out IDalamudTextureWrap texture))
				{
					continue;
				}
				ImGui.TableNextColumn();
				Vector2 cursorPos = ImGui.GetCursorPos();
				Vector2 vector = new Vector2(ImGui.GetContentRegionAvail().X, 4f * ImGui.GetTextLineHeightWithSpacing());
				GroupDisposable val2 = ImRaii.Group();
				try
				{
					ImGui.SetCursorPosX(ImGui.GetCursorPosX() + (vector.X - num3) / 2f);
					ImGui.Image(texture.Handle, new Vector2(num3));
					string text = ((object)((Mount)(ref current)).Singular/*cast due to constrained. prefix*/).ToString();
					ImGui.SetCursorPosX(ImGui.GetCursorPosX() + (vector.X - ImGui.CalcTextSize(ImU8String.op_Implicit(text), false, -1f).X) / 2f);
					ImGui.TextUnformatted(ImU8String.op_Implicit(text));
				}
				finally
				{
					((GroupDisposable)(ref val2)).Dispose();
				}
				ImGui.SetCursorPos(cursorPos);
				ColorDisposable val3 = ImRaii.PushColor((ImGuiCol)21, ButtonNormalColor, true);
				try
				{
					ColorDisposable val4 = ImRaii.PushColor((ImGuiCol)23, ButtonActiveColor, true);
					try
					{
						ColorDisposable val5 = ImRaii.PushColor((ImGuiCol)22, ButtonHoveredColor, true);
						try
						{
							ColorDisposable val6 = ImRaii.PushColor((ImGuiCol)21, ButtonSelectedColor, handler.SelectedIDs.Contains(((Mount)(ref current)).RowId));
							try
							{
								ImU8String val7 = new ImU8String(3, 2);
								((ImU8String)(ref val7)).AppendLiteral("##");
								((ImU8String)(ref val7)).AppendFormatted<uint>(((Mount)(ref current)).RowId);
								((ImU8String)(ref val7)).AppendLiteral("_");
								((ImU8String)(ref val7)).AppendFormatted<Vector2>(cursorPos);
								if (ImGui.Button(val7, vector))
								{
									if (!handler.SelectedIDs.Add(((Mount)(ref current)).RowId))
									{
										handler.SelectedIDs.Remove(((Mount)(ref current)).RowId);
									}
									config.Save(this);
								}
								if (ImGui.IsItemHovered())
								{
									ImGui.SetMouseCursor((ImGuiMouseCursor)7);
								}
							}
							finally
							{
								((IDisposable)val6)?.Dispose();
							}
						}
						finally
						{
							((IDisposable)val5)?.Dispose();
						}
					}
					finally
					{
						((IDisposable)val4)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)val3)?.Dispose();
				}
			}
		}
		finally
		{
			((TableDisposable)(ref val)).Dispose();
		}
	}

	private void OnZoneChanged(uint u)
	{
		OnLogin();
	}

	private unsafe void OnLogin()
	{
		List<Mount> data = ((IEnumerable<Mount>)LuminaGetter.Get<Mount>()).Where((Mount mount) => ((PlayerState)PlayerState.Instance()).IsMountUnlocked(((Mount)(ref mount)).RowId) && ((Mount)(ref mount)).Icon != 0 && !string.IsNullOrEmpty(((object)((Mount)(ref mount)).Singular/*cast due to constrained. prefix*/).ToString())).ToList();
		masterMountsSearcher = new LuminaSearcher<Mount>(data, new Func<Mount, string>[1]
		{
			(Mount x) => ((object)((Mount)(ref x)).Singular/*cast due to constrained. prefix*/).ToString()
		});
		normalMounts = new MountListHandler(masterMountsSearcher, config.NormalRouletteMounts);
		pvpMounts = new MountListHandler(masterMountsSearcher, config.PVPRouletteMounts);
		zoneMountListHandlers.Clear();
		foreach (var (key, selectedIDs) in config.ZoneRouletteMounts)
		{
			zoneMountListHandlers[key] = new MountListHandler(masterMountsSearcher, selectedIDs);
		}
	}

	private void OnPreUseAction(ref bool isPrevented, ref ActionType actionType, ref uint actionID, ref ulong targetID, ref uint extraParam, ref UseActionMode queueState, ref uint comboRouteID)
	{
		if (!DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4] && (int)actionType == 5 && MountRouletteActionIDs.Contains(actionID))
		{
			mountsListToUse = null;
			uint territoryType = GameState.TerritoryType;
			if (config.ZoneRouletteMounts.TryGetValue(territoryType, out HashSet<uint> value) && value.Count > 0)
			{
				mountsListToUse = value;
			}
			else
			{
				mountsListToUse = (GameState.IsInPVPArea ? config.PVPRouletteMounts : config.NormalRouletteMounts);
			}
		}
		if (mountsListToUse == null || (int)actionType != 13)
		{
			return;
		}
		try
		{
			if (mountsListToUse.Count > 0)
			{
				List<uint> list = mountsListToUse.ToList();
				uint num = list[Random.Shared.Next(list.Count)];
				actionID = num;
			}
		}
		finally
		{
			mountsListToUse = null;
		}
	}
}
