using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Game;
using Dalamud.Game.Text.Evaluator;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using KamiToolKit.Controllers;
using KamiToolKit.MapOverlay;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Info.Game.AetheryteRecord;
using OmenTools.Interop.Game.Helpers;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.KamiToolKit.Addons.SelectYesno;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic.Interface;

public class ClickableAethernet : ModuleBase
{
	private AddonController<AddonAreaMap>? areaMapController;

	private MapOverlayController? mapOverlayController;

	private SelectYesnoAddon? drSelectYesno;

	private uint lastMapID;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("ClickableAethernetTitle"),
		Description = LanguageManager.Get("ClickableAethernetDescription"),
		Category = ModuleCategory.Interface
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected unsafe override void Init()
	{
		mapOverlayController = new MapOverlayController();
		mapOverlayController.Enable();
		areaMapController = new AddonController<AddonAreaMap>
		{
			AddonName = "AreaMap",
			OnUpdate = OnAddonUpdate
		};
		areaMapController.Enable();
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += OnZoneChanged;
	}

	protected override void Uninit()
	{
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChanged;
		areaMapController?.Dispose();
		areaMapController = null;
		mapOverlayController?.RemoveAllMarkers();
		mapOverlayController?.Dispose();
		mapOverlayController = null;
		drSelectYesno?.Dispose();
		drSelectYesno = null;
	}

	private void OnZoneChanged(uint zone)
	{
		lastMapID = 0u;
	}

	private unsafe void OnAddonUpdate(AddonAreaMap* addon)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		AgentMap* ptr = AgentMap.Instance();
		uint num = ((GameState.IsLoggedIn && ptr != null && ((AgentMap)ptr).IsAgentActive()) ? ((AgentMap)ptr).SelectedMapId : 0u);
		if (num == lastMapID)
		{
			return;
		}
		lastMapID = num;
		mapOverlayController.RemoveAllMarkers();
		if (num == 0)
		{
			return;
		}
		Map map = LuminaGetter.GetRowOrDefault<Map>(((AgentMap)ptr).SelectedMapId);
		List<MapMarker> mapMarkers = map.GetMapMarkers();
		Addon rowOrDefault = LuminaGetter.GetRowOrDefault<Addon>(3217u);
		ReadOnlySeString text = ((Addon)(ref rowOrDefault)).Text;
		string macroString = ((ReadOnlySeString)(ref text)).ToMacroString().Replace("Aetheryte,lnum1,8", "Aetheryte,lnum1,9").Replace("<kilo(lnum2,\\,)>", "<string(lstr2)>");
		foreach (MapMarker item in mapMarkers)
		{
			MapMarker marker = item;
			if (((MapMarker)(ref marker)).DataType != 4)
			{
				continue;
			}
			RowRef dataKey = ((MapMarker)(ref marker)).DataKey;
			if (!LuminaGetter.TryGetRow<PlaceName>(((RowRef)(ref dataKey)).RowId, out PlaceName placeNameRow))
			{
				continue;
			}
			AetheryteRecord aetheryteRecord = OmenServiceBase<AetheryteRecordManager>.Instance().AllRecords.FirstOrDefault(delegate(AetheryteRecord x)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0009: Unknown result type (might be due to invalid IL or missing references)
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				Aetheryte data = x.GetData();
				return ((Aetheryte)(ref data)).AethernetName.RowId == ((PlaceName)(ref placeNameRow)).RowId;
			});
			if ((object)aetheryteRecord == null)
			{
				continue;
			}
			AetheryteRecord nearestAetheryte = OmenServiceBase<AetheryteRecordManager>.Instance().GetNearestAetheryte(GameState.TerritoryType, Vector3.Zero);
			bool flag = (object)nearestAetheryte != null && nearestAetheryte.Group == aetheryteRecord.Group;
			uint cost = ((!flag) ? aetheryteRecord.Cost : 0u);
			mapOverlayController.AddMarker(new MapMarkerNode
			{
				MapId = ((AgentMap)ptr).SelectedMapId,
				Position = PositionHelper.TextureToWorld(marker.GetPosition(), map),
				IconId = ((MapMarker)(ref marker)).Icon,
				Size = new Vector2(32f),
				MarkerScale = 1f,
				TextTooltip = ReadOnlySeString.op_Implicit($"{aetheryteRecord.Name}\n({cost}\ue049)"),
				OnClick = delegate
				{
					//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
					//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
					//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
					//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
					//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
					if (((AddonAreaMap)addon).NumBlockingAddons == 0)
					{
						if (drSelectYesno != null && !AddonHelper.TryGetPtrByName("DRSelectYesno", out var _))
						{
							try
							{
								drSelectYesno?.Dispose();
								drSelectYesno = null;
							}
							catch
							{
							}
						}
						if (drSelectYesno == null)
						{
							ClickableAethernet clickableAethernet = this;
							SelectYesnoAddonOptions selectYesnoAddonOptions = new SelectYesnoAddonOptions();
							ISeStringEvaluator obj2 = DalamudServiceExtension.Instance<ISeStringEvaluator>();
							string text2 = macroString;
							InlineArray2<SeStringParameter> buffer = default(InlineArray2<SeStringParameter>);
							buffer[0] = SeStringParameter.op_Implicit(aetheryteRecord.RowID);
							buffer[1] = SeStringParameter.op_Implicit(cost.ToChineseString());
							selectYesnoAddonOptions.Prompt = obj2.EvaluateMacroString(text2, (Span<SeStringParameter>)buffer, (ClientLanguage?)null);
							selectYesnoAddonOptions.Callback = delegate(SelectYesnoAddon _, SelectYesnoAddonResult result)
							{
								drSelectYesno = null;
								if (result == SelectYesnoAddonResult.Yes)
								{
									aetheryteRecord.TeleportTo();
								}
							};
							selectYesnoAddonOptions.Position = new AddonPosition((*((AddonAreaMap)addon).RootNode).GetNodeState().Center, AddonPositionAlignment.TopCenter);
							selectYesnoAddonOptions.BlockedParentID = ((AddonAreaMap)addon).Id;
							selectYesnoAddonOptions.ParentID = ((AddonAreaMap)addon).Id;
							clickableAethernet.drSelectYesno = SelectYesnoAddon.Open(selectYesnoAddonOptions);
						}
					}
				}
			});
		}
	}
}
