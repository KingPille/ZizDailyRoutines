using System.Numerics;
using Lumina.Excel.Sheets;
using OmenTools.Interop.Game.Lumina;

namespace OmenTools.Info.Game.ItemSource.Models;

public class ShopNPCLocation
{
	public float X { get; }

	public float Y { get; }

	public uint TerritoryID { get; }

	public uint MapID { get; }

	public Vector2 TexturePosition => new Vector2(X, Y);

	public Vector2 MapPosition
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			Map rowOrDefault = LuminaGetter.GetRowOrDefault<Map>(MapID);
			return ToMapPos(new Vector2(X, Y), (int)((Map)(ref rowOrDefault)).SizeFactor, new Vector2(((Map)(ref rowOrDefault)).OffsetX, ((Map)(ref rowOrDefault)).OffsetY));
		}
	}

	public ShopNPCLocation(float x, float y, uint territoryID, uint? map = null)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		X = x;
		Y = y;
		TerritoryID = territoryID;
		uint? num = map;
		uint num2;
		if (!num.HasValue)
		{
			TerritoryType rowOrDefault = LuminaGetter.GetRowOrDefault<TerritoryType>(territoryID);
			num2 = ((TerritoryType)(ref rowOrDefault)).Map.RowId;
		}
		else
		{
			num2 = num.GetValueOrDefault();
		}
		MapID = num2;
		base._002Ector();
	}

	public TerritoryType GetTerritory()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return LuminaGetter.GetRowOrDefault<TerritoryType>(TerritoryID);
	}

	public Map GetMap()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return LuminaGetter.GetRowOrDefault<Map>(MapID);
	}

	private static Vector2 ToMapPos(Vector2 pos, float scale, Vector2 offset)
	{
		float x = ToMapPos(pos.X, scale, (short)offset.X);
		float y = ToMapPos(pos.Y, scale, (short)offset.Y);
		return new Vector2(x, y);
	}

	private static float ToMapPos(float val, float scale, short offset)
	{
		float num = scale / 100f;
		val = (val + (float)offset) * num;
		return 41f / num * ((val + 1024f) / 2048f) + 1f;
	}
}
