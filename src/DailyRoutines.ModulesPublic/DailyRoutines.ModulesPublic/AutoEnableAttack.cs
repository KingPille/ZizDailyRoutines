using System;
using System.Collections.Frozen;
using System.Numerics;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.Sheets;
using OmenTools.Interop.Game;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class AutoEnableAttack : ModuleBase
{
	private MemoryPatch autoAttackPatch;

	private static readonly FrozenSet<byte> BehavioursToModify = FrozenSet.Create((ReadOnlySpan<byte>)new byte[3] { 1, 2, 4 });

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoEnableAttackTitle"),
		Description = LanguageManager.Get("AutoEnableAttackDescription"),
		Category = ModuleCategory.Combat
	};

	protected override void Init()
	{
		autoAttackPatch = new MemoryPatch("41 0F B6 46 ?? FF C8", new global::_003C_003Ez__ReadOnlyArray<byte?>(new byte?[5]
		{
			(byte)184,
			(byte)2,
			0,
			0,
			0
		}));
		OmenServiceBase<UseActionManager>.Instance().RegPreUseActionLocation(OnPreUseAction);
		OmenServiceBase<UseActionManager>.Instance().RegPostUseActionLocation(OnPostUseAction);
	}

	protected override void Uninit()
	{
		OmenServiceBase<UseActionManager>.Instance().Unreg(OnPreUseAction);
		OmenServiceBase<UseActionManager>.Instance().Unreg(OnPostUseAction);
	}

	private void OnPreUseAction(ref bool isPrevented, ref ActionType type, ref uint actionID, ref ulong targetID, ref Vector3 location, ref uint extraParam, ref byte a7)
	{
		if ((int)type != 1 || !LuminaGetter.TryGetRow<Action>(actionID, out Action item))
		{
			autoAttackPatch.Disable();
		}
		else
		{
			autoAttackPatch.Set(BehavioursToModify.Contains(((Action)(ref item)).AutoAttackBehaviour));
		}
	}

	private void OnPostUseAction(bool result, ActionType actionType, uint actionID, ulong targetID, Vector3 location, uint extraParam, byte a7)
	{
		autoAttackPatch.Disable();
	}
}
