using System;
using System.Numerics;
using Newtonsoft.Json;

namespace OmenTools.Info.Json.Converters;

public class Vector4Converter : JsonConverter<Vector4>
{
	public override Vector4 ReadJson(JsonReader reader, Type objectType, Vector4 existingValue, bool hasExistingValue, JsonSerializer serializer)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Invalid comparison between Unknown and I4
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Invalid comparison between Unknown and I4
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if ((int)reader.TokenType != 1)
		{
			throw new JsonException("Expected start of object");
		}
		float x = 0f;
		float y = 0f;
		float z = 0f;
		float w = 0f;
		while (reader.Read())
		{
			if ((int)reader.TokenType == 13)
			{
				return new Vector4(x, y, z, w);
			}
			if ((int)reader.TokenType != 4)
			{
				throw new JsonException("Expected property name");
			}
			string? obj = reader.Value?.ToString();
			reader.Read();
			switch (obj?.ToLower())
			{
			case "x":
				x = Convert.ToSingle(reader.Value);
				break;
			case "y":
				y = Convert.ToSingle(reader.Value);
				break;
			case "z":
				z = Convert.ToSingle(reader.Value);
				break;
			case "w":
				w = Convert.ToSingle(reader.Value);
				break;
			default:
				reader.Skip();
				break;
			}
		}
		throw new JsonException("Expected end of object");
	}

	public override void WriteJson(JsonWriter writer, Vector4 value, JsonSerializer serializer)
	{
		writer.WriteStartObject();
		writer.WritePropertyName("x");
		writer.WriteValue(value.X);
		writer.WritePropertyName("y");
		writer.WriteValue(value.Y);
		writer.WritePropertyName("z");
		writer.WriteValue(value.Z);
		writer.WritePropertyName("w");
		writer.WriteValue(value.W);
		writer.WriteEndObject();
	}
}
