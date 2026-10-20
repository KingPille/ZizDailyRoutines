using System;
using System.Collections.Generic;
using OmenTools.Dalamud.Abstractions;
using OmenTools.Dalamud.Attributes;

namespace OmenTools.Dalamud;

public static class RaphaelIPC
{
	public const string INTERNAL_NAME = "Raphael.Dalamud";

	[IPCSubscriber("Raphael.Dalamud.StartCalculation")]
	private static IPCSubscriber<uint> StartCalculationSubscriber;

	[IPCSubscriber("Raphael.Dalamud.StartCalculationWithRecipe")]
	private static IPCSubscriber<uint, uint> StartCalculationWithRecipeSubscriber;

	[IPCSubscriber("Raphael.Dalamud.StartCalculationWithConfig")]
	private static IPCSubscriber<uint, string, uint> StartCalculationWithConfigSubscriber;

	[IPCSubscriber("Raphael.Dalamud.GetCalculationStatus")]
	private static IPCSubscriber<uint, Tuple<uint, string, string, List<uint>>> GetCalculationStatusSubscriber;

	[IPCSubscriber("Raphael.Dalamud.GetCurrentRecipeID")]
	private static IPCSubscriber<uint> GetCurrentRecipeIDSubscriber;

	public static uint GetCurrentRecipeID()
	{
		return GetCurrentRecipeIDSubscriber.InvokeFunc();
	}

	public static uint StartCalculation()
	{
		return StartCalculationSubscriber.InvokeFunc();
	}

	public static uint StartCalculation(uint recipeID)
	{
		return StartCalculationWithRecipeSubscriber.InvokeFunc(recipeID);
	}

	public static uint StartCalculation(uint recipeID, RaphaelCalculationConfig config)
	{
		return StartCalculationWithConfigSubscriber.InvokeFunc(recipeID, config.ToJSON());
	}

	public static RaphaelCaculationResponse GetCalculationStatus(uint requestID)
	{
		Tuple<uint, string, string, List<uint>> tuple = GetCalculationStatusSubscriber.InvokeFunc(requestID);
		RaphaelCalculationStatus result;
		return new RaphaelCaculationResponse(tuple.Item1, Enum.TryParse<RaphaelCalculationStatus>(tuple.Item2, out result) ? result : RaphaelCalculationStatus.Failed, tuple.Item3, tuple.Item4);
	}
}
