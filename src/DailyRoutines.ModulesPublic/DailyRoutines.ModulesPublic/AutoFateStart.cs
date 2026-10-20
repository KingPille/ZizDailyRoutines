using System;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.STD.Helper;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;

namespace DailyRoutines.ModulesPublic;

public class AutoFateStart : ModuleBase
{
	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static OnUpdateDelegate _003C0_003E__OnUpdate;
	}

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoFateStartTitle"),
		Description = LanguageManager.Get("AutoFateStartDescription"),
		Category = ModuleCategory.Combat
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
		FrameworkManager frameworkManager = OmenServiceBase<FrameworkManager>.Instance();
		object obj = _003C_003EO._003C0_003E__OnUpdate;
		if (obj == null)
		{
			OnUpdateDelegate val = OnUpdate;
			_003C_003EO._003C0_003E__OnUpdate = val;
			obj = (object)val;
		}
		frameworkManager.Reg((OnUpdateDelegate)obj, 2000u);
	}

	protected override void Uninit()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		FrameworkManager frameworkManager = OmenServiceBase<FrameworkManager>.Instance();
		OnUpdateDelegate[] array = new OnUpdateDelegate[1];
		object obj = _003C_003EO._003C0_003E__OnUpdate;
		if (obj == null)
		{
			OnUpdateDelegate val = OnUpdate;
			_003C_003EO._003C0_003E__OnUpdate = val;
			obj = (object)val;
		}
		array[0] = (OnUpdateDelegate)obj;
		frameworkManager.Unreg((OnUpdateDelegate[])(object)array);
	}

	private unsafe static void OnUpdate(IFramework framework)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		if ((int)GameState.TerritoryIntendedUse != 1 || GameState.IsInPVPArea)
		{
			return;
		}
		ClassJob classJobData = LocalPlayerState.ClassJobData;
		if (((ClassJob)(ref classJobData)).DohDolJobIndex != -1 || DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)26] || ((FateManager)FateManager.Instance()).CurrentFate != null)
		{
			return;
		}
		UnmanagedArrayEnumerator<GameObjectId> enumerator = ((FateManager)FateManager.Instance()).FateStartNPCs.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				uint objectId = enumerator.Current.ObjectId;
				BattleChara* ptr = ((CharacterManager)CharacterManager.Instance()).LookupBattleCharaByEntityId(objectId);
				if (ptr != null && ((BattleChara)ptr).FateId != 0)
				{
					OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.StartFate, ((BattleChara)ptr).FateId, objectId);
					if (Throttler.Shared.Throttle($"AutoFateStart-Fate-{((BattleChara)ptr).FateId}", 60000u))
					{
						OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoFateStart-StartNotice", LuminaWrapper.GetFateName(((BattleChara)ptr).FateId))));
					}
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
	}
}
