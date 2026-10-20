using System.Numerics;
using Lumina.Excel.Sheets;
using OmenTools.Extensions;

namespace OmenTools.Interop.Game.Helpers;

public static class PositionHelper
{
	private const float PIXELS_PER_MAP_UNIT = 40.96f;

	private const float TEXTURE_CENTER = 1024f;

	private const float TEXTURE_SIZE = 2048f;

	private const int HEIGHT_OFFSET_SENTINEL = -10000;

	public static Vector2 WorldToTexture(Vector3 pos, Map map)
	{
		return WorldXZToTexture(new Vector2(pos.X, pos.Z), ((Map)(ref map)).SizeFactor, ((Map)(ref map)).OffsetX, ((Map)(ref map)).OffsetY);
	}

	public static Vector2 TextureToWorld(Vector2 pos, Map map)
	{
		return TextureToWorldXZ(pos, ((Map)(ref map)).SizeFactor, ((Map)(ref map)).OffsetX, ((Map)(ref map)).OffsetY);
	}

	public static Vector2 WorldToMap(Vector3 pos, Map map)
	{
		return TextureToMapXZ(WorldXZToTexture(pos.ToVector2(), ((Map)(ref map)).SizeFactor, ((Map)(ref map)).OffsetX, ((Map)(ref map)).OffsetY), ((Map)(ref map)).SizeFactor);
	}

	public static Vector2 WorldToMap(Vector2 pos, Map map)
	{
		return TextureToMapXZ(WorldXZToTexture(pos, ((Map)(ref map)).SizeFactor, ((Map)(ref map)).OffsetX, ((Map)(ref map)).OffsetY), ((Map)(ref map)).SizeFactor);
	}

	public static Vector3 WorldToMap(Vector3 pos, Map map, TerritoryTypeTransient territoryTransient, bool correctHeightOffset = false)
	{
		Vector2 vector = TextureToMapXZ(WorldXZToTexture(new Vector2(pos.X, pos.Z), ((Map)(ref map)).SizeFactor, ((Map)(ref map)).OffsetX, ((Map)(ref map)).OffsetY), ((Map)(ref map)).SizeFactor);
		return new Vector3(vector.X, WorldYToMap(pos.Y, ((TerritoryTypeTransient)(ref territoryTransient)).OffsetZ, correctHeightOffset), vector.Y);
	}

	public static Vector2 MapToWorld(Vector2 pos, Map map)
	{
		return TextureToWorldXZ(MapToTextureXZ(pos, ((Map)(ref map)).SizeFactor), ((Map)(ref map)).SizeFactor, ((Map)(ref map)).OffsetX, ((Map)(ref map)).OffsetY);
	}

	public static Vector3 MapToWorld(Vector3 pos, Map map, TerritoryTypeTransient territoryTransient, bool correctHeightOffset = false)
	{
		Vector2 vector = TextureToWorldXZ(MapToTextureXZ(new Vector2(pos.X, pos.Z), ((Map)(ref map)).SizeFactor), ((Map)(ref map)).SizeFactor, ((Map)(ref map)).OffsetX, ((Map)(ref map)).OffsetY);
		return new Vector3(vector.X, MapToWorldY(pos.Y, ((TerritoryTypeTransient)(ref territoryTransient)).OffsetZ, correctHeightOffset), vector.Y);
	}

	public static Vector2 TextureToMap(int x, int y, float sizeFactor)
	{
		return TextureToMapXZ(new Vector2(x, y), (uint)sizeFactor);
	}

	public static Vector2 TextureToMap(Vector2 pos, Map map)
	{
		return TextureToMapXZ(pos, ((Map)(ref map)).SizeFactor);
	}

	public static Vector2 MapToTexture(Vector2 pos, Map map)
	{
		return MapToTextureXZ(pos, ((Map)(ref map)).SizeFactor);
	}

	public static float WorldYToMap(float value, int heightOffset, bool correctHeightOffset = false)
	{
		if (!correctHeightOffset || heightOffset != -10000)
		{
			return (value - (float)heightOffset) / 100f;
		}
		return value / 100f;
	}

	public static float MapToWorldY(float mapValue, int heightOffset, bool correctHeightOffset = false)
	{
		if (!correctHeightOffset || heightOffset != -10000)
		{
			return mapValue * 100f + (float)heightOffset;
		}
		return mapValue * 100f;
	}

	private static Vector2 WorldXZToTexture(Vector2 worldXZ, uint sizeFactor, int offsetX, int offsetY)
	{
		float num = (float)sizeFactor / 100f;
		return (worldXZ + new Vector2(offsetX, offsetY)) * num + new Vector2(1024f);
	}

	private static Vector2 TextureToWorldXZ(Vector2 texture, uint sizeFactor, int offsetX, int offsetY)
	{
		float num = (float)sizeFactor / 100f;
		return (texture - new Vector2(1024f)) / num - new Vector2(offsetX, offsetY);
	}

	private static Vector2 TextureToMapXZ(Vector2 texture, uint sizeFactor)
	{
		float num = (float)sizeFactor / 100f;
		return texture / 2048f * (40.96f / num) + new Vector2(1f);
	}

	private static Vector2 MapToTextureXZ(Vector2 map, uint sizeFactor)
	{
		float num = (float)sizeFactor / 100f;
		return (map - new Vector2(1f)) * num / 40.96f * 2048f;
	}
}
