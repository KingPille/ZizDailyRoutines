using System;
using System.Numerics;
using Newtonsoft.Json;

namespace OmenTools.Info.Json.Converters;

public class Vector3Converter : JsonConverter<Vector3>
{
	public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 existingValue, bool hasExistingValue, JsonSerializer serializer)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Invalid comparison between Unknown and I4
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Invalid comparison between Unknown and I4
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if ((int)reader.TokenType != 1)
		{
			throw new JsonException("Expected start of object");
		}
		float x = 0f;
		float y = 0f;
		float z = 0f;
		while (reader.Read())
		{
			if ((int)reader.TokenType == 13)
			{
				return new Vector3(x, y, z);
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
			default:
				reader.Skip();
				break;
			}
		}
		throw new JsonException("Expected end of object");
	}

	public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
	{
		writer.WriteStartObject();
		writer.WritePropertyName("x");
		writer.WriteValue(value.X);
		writer.WritePropertyName("y");
		writer.WriteValue(value.Y);
		writer.WritePropertyName("z");
		writer.WriteValue(value.Z);
		writer.WriteEndObject();
	}
}
