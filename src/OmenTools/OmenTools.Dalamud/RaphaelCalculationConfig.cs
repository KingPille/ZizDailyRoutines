using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmenTools.Dalamud;

public sealed class RaphaelCalculationConfig
{
	private static readonly JsonSerializerSettings JSONSettings = new JsonSerializerSettings
	{
		NullValueHandling = (NullValueHandling)1,
		ContractResolver = (IContractResolver)new CamelCasePropertyNamesContractResolver()
	};

	public bool? EnsureReliability { get; init; }

	public bool? BackloadProgress { get; init; }

	public bool? AllowHeartAndSoul { get; init; }

	public bool? AllowQuickInnovation { get; init; }

	public int? MaxThreads { get; init; }

	public int? TimeoutSeconds { get; init; }

	public int? TargetQuality { get; init; }

	public int? InitialQuality { get; init; }

	public uint? FoodItemID { get; init; }

	public bool? FoodHQ { get; init; }

	public uint? PotionItemID { get; init; }

	public bool? PotionHQ { get; init; }

	public int? StellarSteadyHand { get; init; }

	internal string ToJSON()
	{
		return JsonConvert.SerializeObject((object)this, JSONSettings);
	}
}
