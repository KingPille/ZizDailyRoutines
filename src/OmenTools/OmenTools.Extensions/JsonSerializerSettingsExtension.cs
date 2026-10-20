using Newtonsoft.Json;
using OmenTools.Info.Json.Converters;

namespace OmenTools.Extensions;

public static class JsonSerializerSettingsExtension
{
	extension(JsonSerializerSettings)
	{
		public static JsonSerializerSettings GetShared()
		{
			return SharedJSONSettings;
		}
	}

	private static JsonSerializerSettings SharedJSONSettings { get; } = new JsonSerializerSettings
	{
		Converters = { (JsonConverter)(object)new Vector2Converter() },
		Converters = { (JsonConverter)(object)new Vector3Converter() },
		Converters = { (JsonConverter)(object)new Vector4Converter() },
		Converters = { (JsonConverter)(object)new TimeSpanConverter() },
		Converters = { (JsonConverter)(object)new VersionConverter() }
	};
}
