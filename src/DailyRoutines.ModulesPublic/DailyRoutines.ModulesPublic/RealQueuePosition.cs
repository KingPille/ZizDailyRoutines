using System;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game;
using Dalamud.Game.Text.Evaluator;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Enums;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class RealQueuePosition : ModuleBase
{
	private unsafe delegate bool AgentWorldTravelUpdateDelegate(AgentWorldTravel* agent, NumberArrayData* numberArray, StringArrayData* stringArray, bool a4);

	private delegate void UpdateWorldTravelDataDelegate(nint a1, nint a2);

	private unsafe delegate void ContentFinderQueuePositionDataDelegate(ContentsFinderQueueInfo* info, ContentsFinderQueueState state, QueueInfoState* infoState);

	private static readonly CompSig AgentWorldTravelUpdaterSig = new CompSig("E8 ?? ?? ?? ?? 40 0A F8 B9 ?? ?? ?? ??");

	private Hook<AgentWorldTravelUpdateDelegate> AgentWorldTravelUpdateHook;

	private static readonly CompSig UpdateWorldTravelDataSig = new CompSig("48 89 5C 24 ?? 57 48 83 EC 20 48 8B D9 48 8B FA 0F B6 4A 10");

	private Hook<UpdateWorldTravelDataDelegate> UpdateWorldTravelDataHook;

	private static readonly CompSig ContentFinderQueuePositionDataSig = new CompSig("40 53 56 57 41 57 48 83 EC ?? 0F B6 41");

	private Hook<ContentFinderQueuePositionDataDelegate>? ContentFinderQueuePositionDataHook;

	private DateTime eta = OmenServiceBase<StandardTimeManager>.Instance().Now;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("RealQueuePositionTitle"),
		Description = LanguageManager.Get("RealQueuePositionDescription"),
		Category = ModuleCategory.System,
		Author = new string[2] { "逆光", "Nukoooo" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected unsafe override void Init()
	{
		if (AgentWorldTravelUpdateHook == null)
		{
			AgentWorldTravelUpdateHook = AgentWorldTravelUpdaterSig.GetHook<AgentWorldTravelUpdateDelegate>(AgentWorldTravelUpdaterDetour);
		}
		AgentWorldTravelUpdateHook.Enable();
		if (UpdateWorldTravelDataHook == null)
		{
			UpdateWorldTravelDataHook = UpdateWorldTravelDataSig.GetHook<UpdateWorldTravelDataDelegate>(UpdateWorldTravelDataDetour);
		}
		UpdateWorldTravelDataHook.Enable();
		if (ContentFinderQueuePositionDataHook == null)
		{
			ContentFinderQueuePositionDataHook = ContentFinderQueuePositionDataSig.GetHook<ContentFinderQueuePositionDataDelegate>(ContentFinderQueuePositionDataDetour);
		}
		ContentFinderQueuePositionDataHook.Enable();
	}

	private unsafe void UpdateWorldTravelDataDetour(nint a1, nint a2)
	{
		if (*(byte*)(a2 + 16) == 1)
		{
			int position = *(int*)(a2 + 20);
			eta = OmenServiceBase<StandardTimeManager>.Instance().Now.AddSeconds(CalculateWaitTime(position));
		}
		UpdateWorldTravelDataHook.Original(a1, a2);
	}

	private unsafe bool AgentWorldTravelUpdaterDetour(AgentWorldTravel* agent, NumberArrayData* numberArray, StringArrayData* stringArray, bool a4)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		nint num = (nint)AgentWorldTravel.Instance();
		if (*(ushort*)(num + 288) == 0)
		{
			return AgentWorldTravelUpdateHook.Original(agent, numberArray, stringArray, a4);
		}
		if (!AgentWorldTravelUpdateHook.Original(agent, numberArray, stringArray, a4))
		{
			return false;
		}
		int num2 = 5;
		if (((NumberArrayData)numberArray).IntArray[5] > 0)
		{
			num2 = 6;
		}
		uint num3 = *(uint*)(num + 300);
		ISeStringEvaluator obj = DalamudServiceExtension.Instance<ISeStringEvaluator>();
		SeStringParameter reference = SeStringParameter.op_Implicit(num3);
		ReadOnlySeString val = obj.EvaluateFromAddon(10039u, new Span<SeStringParameter>(ref reference), (ClientLanguage?)null);
		RentedSeStringBuilder val2 = default(RentedSeStringBuilder);
		((RentedSeStringBuilder)(ref val2))._002Ector();
		try
		{
			((StringArrayData)stringArray).SetValue(num2, ((RentedSeStringBuilder)(ref val2)).Builder.Append(LuminaWrapper.GetAddonText(12522u)).Append(val).GetViewAsSpan(), true, true, false);
			TimeSpan value = TimeSpan.FromSeconds(*(int*)(num + 296));
			string text = LanguageManager.Get("RealQueuePosition-ETA", $"{value:mm\\:ss}", $"{eta - OmenServiceBase<StandardTimeManager>.Instance().Now:mm\\:ss}");
			((StringArrayData)stringArray).SetValue(num2 + 1, text, true, true, false);
			return true;
		}
		finally
		{
			((IDisposable)(*(RentedSeStringBuilder*)(&val2))/*cast due to constrained. prefix*/).Dispose();
		}
	}

	private unsafe void ContentFinderQueuePositionDataDetour(ContentsFinderQueueInfo* info, ContentsFinderQueueState state, QueueInfoState* infoState)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		sbyte b = (sbyte)((QueueInfoState)infoState).PositionInQueue;
		if (b != 0)
		{
			((ContentsFinderQueueInfo)info).PositionInQueue = b;
			((ContentsFinderQueueInfo)info).ClampedPositionInQueue = b;
		}
		ContentFinderQueuePositionDataHook.Original(info, state, infoState);
	}

	private static double CalculateWaitTime(int position)
	{
		if (position <= 0)
		{
			return 0.0;
		}
		float num = (float)((position - 1) / 4) * 10f;
		float num2 = (((position - 1) % 4 > 0) ? 10f : 0f);
		return num + num2;
	}
}
