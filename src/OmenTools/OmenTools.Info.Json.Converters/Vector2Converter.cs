using System;
using System.Numerics;
using Newtonsoft.Json;

namespace OmenTools.Info.Json.Converters;

public class Vector2Converter : JsonConverter<Vector2>
{
	public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existingValue, bool hasExistingValue, JsonSerializer serializer)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Invalid comparison between Unknown and I4
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Invalid comparison between Unknown and I4
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if ((int)reader.TokenType != 1)
		{
			throw new JsonException("Expected start of object");
		}
		float x = 0f;
		float y = 0f;
		while (reader.Read())
		{
			if ((int)reader.TokenType == 13)
			{
				return new Vector2(x, y);
			}
			if ((int)reader.TokenType != 4)
			{
				throw new JsonException("Expected property name");
			}
			string? obj = reader.Value?.ToString();
			reader.Read();
			string text = obj?.ToLower();
			if (!(text == "x"))
			{
				if (text == "y")
				{
					y = Convert.ToSingle(reader.Value);
				}
				else
				{
					reader.Skip();
				}
			}
			else
			{
				x = Convert.ToSingle(reader.Value);
			}
		}
		throw new JsonException("Expected end of object");
	}

	public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
	{
		writer.WriteStartObject();
		writer.WritePropertyName("x");
		writer.WriteValue(value.X);
		writer.WritePropertyName("y");
		writer.WriteValue(value.Y);
		writer.WriteEndObject();
	}
}
