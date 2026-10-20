using System;
using System.Collections.Generic;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.RemoteInteraction.Universalis.Models.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using itAnDoW4mgYVlrmtMor;

namespace DailyRoutines.RemoteInteraction.Universalis.Helpers.JsonCoverters;

public class UniversalisMarketConverter : JsonConverter
{
	public override bool CanConvert(Type objectType)
	{
		return objectType == typeof(UniversalisMarketDataResponse);
	}

	public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
	{
		UniversalisMarketDataResponse universalisMarketDataResponse = new UniversalisMarketDataResponse();
		JObject val = JObject.Parse(reader.Value?.ToString() ?? ((object)JToken.ReadFrom(reader)).ToString());
		ymULcr0PMN(universalisMarketDataResponse, val);
		JToken obj = val[GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x371F3C8 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_a86a125fa3c94b2ca72995b652fdcc2e)];
		JObject val2 = (JObject)(object)((obj is JObject) ? obj : null);
		if (val2 != null)
		{
			foreach (JProperty item in val2.Properties())
			{
				if (uint.TryParse(item.Name, out var result))
				{
					universalisMarketDataResponse.Items[result] = item.Value.ToObject<UniversalisMarketItemData>() ?? new UniversalisMarketItemData();
				}
			}
		}
		else if (val[GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x7C6D3506 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d2f07ce64d464f30b1d7397ae7500cac)] != null)
		{
			uint key = val[GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2F0DE01 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d964b0b1313049f490f2107d03a6ed3a)].ToObject<uint>();
			UniversalisMarketItemData value = ((JToken)val).ToObject<UniversalisMarketItemData>() ?? new UniversalisMarketItemData();
			universalisMarketDataResponse.Items[key] = value;
		}
		return universalisMarketDataResponse;
	}

	public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
	{
		throw new NotImplementedException(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3F34183A ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_128c630676db4a8ab2ed83db4c2b1862));
	}

	private static void ymULcr0PMN(UniversalisResponseBase response, JObject jObject)
	{
		if (jObject[GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x70C323C9 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c041f06f3e304b758cab1109bed434d0)] != null)
		{
			response.ItemIds = jObject[GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2F0DFE3 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d964b0b1313049f490f2107d03a6ed3a)].ToObject<List<uint>>() ?? new List<uint>();
		}
		if (jObject[GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4FBFD8E1 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_881eb9df907345dea66a751ad6d65e76)] != null)
		{
			response.UnresolvedItems = jObject[GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(--367535148 ^ 0x392860E0 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c02509fc64c64faaa7b59b967c4e5f83)].ToObject<List<uint>>() ?? new List<uint>();
		}
		if (jObject[GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6CA62D73 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_86e8e55020514087b1fe097f879d6dff)] != null)
		{
			response.WorldID = jObject[GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5B208C62 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_435f54c92c5b40a9ad2757dd36deed0a)].ToObject<uint?>();
		}
		response.DCName = ((object)jObject[GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x27BB30AD ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7f9a8e3b1ea249bebe4aa86658ee9503)])?.ToString() ?? string.Empty;
		response.RegionName = ((object)jObject[GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x642B1C50 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7f11a41e9f664db18364eb0afc92a5df)])?.ToString() ?? string.Empty;
		response.WorldName = ((object)jObject[GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x227FBA1 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_e3177f14377e4fe58ddaaaaf90163f44)])?.ToString() ?? string.Empty;
	}

	public UniversalisMarketConverter()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		((JsonConverter)this)._002Ector();
	}
}
