using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using OmenTools.Dalamud.Services.Game.UI.Abstractions;
using OmenTools.Extensions;

namespace OmenTools.Dalamud.Services.Game.UI;

internal readonly struct AetheryteEntry(TeleportInfo data) : IAetheryteEntry
{
	public uint AetheryteID => data.AetheryteId;

	public uint TerritoryID => data.TerritoryId;

	public byte SubIndex => data.SubIndex;

	public byte Ward => data.Ward;

	public byte Plot => data.Plot;

	public uint GilCost => data.GilCost;

	public bool IsFavourite => data.IsFavourite;

	public bool IsSharedHouse
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			TeleportInfo val = data;
			return ((TeleportInfo)(ref val)).IsSharedHouse;
		}
	}

	public bool IsApartment
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			TeleportInfo val = data;
			return ((TeleportInfo)(ref val)).IsApartment;
		}
	}

	public RowRef<Aetheryte> AetheryteData => AetheryteID.ToLuminaRowRef<Aetheryte>();
}
