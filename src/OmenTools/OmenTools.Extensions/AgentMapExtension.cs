using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Excel.Sheets;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;

namespace OmenTools.Extensions;

public static class AgentMapExtension
{
	extension(scoped ref AgentMap agent)
	{
		public void SetMapByZoneAndOpen(uint territoryTypeID, string? mapTitle = null)
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			if (LuminaGetter.TryGetRow<TerritoryType>(territoryTypeID, out TerritoryType item))
			{
				if (((AgentMap)(ref agent)).IsAgentActive() && agent.SelectedMapId == ((TerritoryType)(ref item)).Map.RowId)
				{
					((AgentMap)(ref agent)).Hide();
				}
				((AgentMap)(ref agent)).OpenMapByMapId(((TerritoryType)(ref item)).Map.RowId, territoryTypeID, false);
				if (mapTitle != null)
				{
					((Utf8String)(ref agent.MapTitleString)).SetString(mapTitle);
				}
			}
		}

		public void SetMapAndOpen(uint mapID, string? mapTitle = null)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			if (LuminaGetter.TryGetRow<Map>(mapID, out Map item))
			{
				if (((AgentMap)(ref agent)).IsAgentActive() && agent.SelectedMapId == mapID)
				{
					((AgentMap)(ref agent)).Hide();
				}
				((AgentMap)(ref agent)).OpenMapByMapId(mapID, ((Map)(ref item)).TerritoryType.RowId, false);
				if (mapTitle != null)
				{
					((Utf8String)(ref agent.MapTitleString)).SetString(mapTitle);
				}
			}
		}

		public void SetMapFlagByZoneAndOpen(uint territoryTypeID, Vector3 worldPosition, string? mapTitle = null)
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			if (LuminaGetter.TryGetRow<TerritoryType>(territoryTypeID, out TerritoryType item))
			{
				if (((AgentMap)(ref agent)).IsAgentActive() && agent.SelectedMapId == ((TerritoryType)(ref item)).Map.RowId)
				{
					((AgentMap)(ref agent)).Hide();
				}
				((AgentMap)(ref agent)).SetFlagMapMarker(territoryTypeID, ((TerritoryType)(ref item)).Map.RowId, worldPosition, 60561u);
				((AgentMap)(ref agent)).OpenMap(((TerritoryType)(ref item)).Map.RowId, territoryTypeID, mapTitle, (MapType)1);
				if (mapTitle != null)
				{
					((Utf8String)(ref agent.MapTitleString)).SetString(mapTitle);
				}
			}
		}

		public void SetMapFlagAndOpen(uint mapID, Vector3 worldPosition, string? mapTitle = null)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			if (LuminaGetter.TryGetRow<Map>(mapID, out Map item))
			{
				if (((AgentMap)(ref agent)).IsAgentActive() && agent.SelectedMapId == mapID)
				{
					((AgentMap)(ref agent)).Hide();
				}
				((AgentMap)(ref agent)).SetFlagMapMarker(((Map)(ref item)).TerritoryType.RowId, mapID, worldPosition, 60561u);
				((AgentMap)(ref agent)).OpenMap(mapID, ((Map)(ref item)).TerritoryType.RowId, mapTitle, (MapType)1);
				if (mapTitle != null)
				{
					((Utf8String)(ref agent.MapTitleString)).SetString(mapTitle);
				}
			}
		}

		public void SetMapFlagAndOpen(Vector3 worldPosition, string? mapTitle = null)
		{
			agent.SetMapFlagAndOpen(GameState.Map, worldPosition, mapTitle);
		}
	}
}
