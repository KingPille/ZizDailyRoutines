using System;
using System.Collections.Frozen;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using OmenTools.Extensions;
using OmenTools.Info.Game.Enums;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class FasterTerritoryTransport : ModuleBase
{
	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static ConditionChangeDelegate _003C0_003E__OnConditionChanged;
	}

	private static readonly FrozenSet<WarpType> InvalidWarpTypes = FrozenSet.Create(RuntimeHelpers.CreateSpan<WarpType>((RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/));

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("FasterTerritoryTransportTitle"),
		Description = LanguageManager.Get("FasterTerritoryTransportDescription"),
		Category = ModuleCategory.System,
		ModulesPair = new string[1] { "NoUIFade" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		NeedAuth = true
	};

	protected override void Init()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		ICondition obj = DalamudServiceExtension.Instance<ICondition>();
		object obj2 = _003C_003EO._003C0_003E__OnConditionChanged;
		if (obj2 == null)
		{
			ConditionChangeDelegate val = OnConditionChanged;
			_003C_003EO._003C0_003E__OnConditionChanged = val;
			obj2 = (object)val;
		}
		obj.ConditionChange += (ConditionChangeDelegate)obj2;
	}

	protected override void Uninit()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		ICondition obj = DalamudServiceExtension.Instance<ICondition>();
		object obj2 = _003C_003EO._003C0_003E__OnConditionChanged;
		if (obj2 == null)
		{
			ConditionChangeDelegate val = OnConditionChanged;
			_003C_003EO._003C0_003E__OnConditionChanged = val;
			obj2 = (object)val;
		}
		obj.ConditionChange -= (ConditionChangeDelegate)obj2;
	}

	private unsafe static void OnConditionChanged(ConditionFlag flag, bool value)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Invalid comparison between Unknown and I4
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if ((int)flag == 45 && !InvalidWarpTypes.Contains(((WarpInfo)WarpInfo.Instance()).WarpType))
		{
			OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.StartTerritoryTransport);
			((WarpInfo)WarpInfo.Instance()).CompleteWarp(0, 0);
		}
	}
}
