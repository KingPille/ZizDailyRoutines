using System;
using System.Collections.Generic;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.RemoteInteraction.Universalis.Models.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using itAnDoW4mgYVlrmtMor;

namespace DailyRoutines.RemoteInteraction.Universalis.Helpers.JsonCoverters;

public class UniversalisHistoryConverter : JsonConverter
{
	public override bool CanConvert(Type objectType)
	{
		return objectType == typeof(UniversalisMarketHistoryResponse);
	}

	public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
	{
		UniversalisMarketHistoryResponse universalisMarketHistoryResponse = new UniversalisMarketHistoryResponse();
		JObject val = JObject.Parse(reader.Value?.ToString() ?? ((object)JToken.ReadFrom(reader)).ToString());
		if (val[GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x35B51642 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_042249f573d4431982cd93672ca35bd3)] != null)
		{
			universalisMarketHistoryResponse.ItemIds = val[GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x642B1DF6 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7f11a41e9f664db18364eb0afc92a5df)].ToObject<List<uint>>() ?? new List<uint>();
		}
		if (val[GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x76DA3B3D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f8c073ebdce5414baf147b1be2d2c2c2)] != null)
		{
			universalisMarketHistoryResponse.UnresolvedItems = val[GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5E39F1A9 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f9eb04af496747c5b32cddd51d0b3997)].ToObject<List<uint>>() ?? new List<uint>();
		}
		if (val[GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1F4B9028 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_0d3e4cb7200c49c9943fa2d7c1024d32)] != null)
		{
			universalisMarketHistoryResponse.WorldID = val[GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2829EDA9 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_95ac0b4adf764381bcdc75a9d241d184)].ToObject<uint?>();
		}
		universalisMarketHistoryResponse.DCName = ((object)val[GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1E24FFBB ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_86afda88005749b094dac2a7d6b27c25)])?.ToString() ?? string.Empty;
		universalisMarketHistoryResponse.RegionName = ((object)val[GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2E959D33 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_6103a9bab63e42479916a134b07f1c75)])?.ToString() ?? string.Empty;
		universalisMarketHistoryResponse.WorldName = ((object)val[GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4628EC11 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_93e1cfc241b644019a4819c49327d6a7)])?.ToString() ?? string.Empty;
		JToken obj = val[GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(--1976305562 ^ 0x3ABCF67F ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_3782f2a028bd4046aa095423cd2526e0)];
		JObject val2 = (JObject)(object)((obj is JObject) ? obj : null);
		if (val2 != null)
		{
			foreach (JProperty item in val2.Properties())
			{
				if (uint.TryParse(item.Name, out var result))
				{
					universalisMarketHistoryResponse.Items[result] = item.Value.ToObject<UniversalisHistoryItemData>() ?? new UniversalisHistoryItemData();
				}
			}
		}
		else if (val[GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3D7C7670 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_cda76bbf50614e97b4ef38939106c571)] != null)
		{
			uint key = val[GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x7C6D3506 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d2f07ce64d464f30b1d7397ae7500cac)].ToObject<uint>();
			UniversalisHistoryItemData value = ((JToken)val).ToObject<UniversalisHistoryItemData>() ?? new UniversalisHistoryItemData();
			universalisMarketHistoryResponse.Items[key] = value;
		}
		return universalisMarketHistoryResponse;
	}

	public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
	{
		throw new NotImplementedException(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3D7C7680 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_cda76bbf50614e97b4ef38939106c571));
	}

	public UniversalisHistoryConverter()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		((JsonConverter)this)._002Ector();
	}
}
