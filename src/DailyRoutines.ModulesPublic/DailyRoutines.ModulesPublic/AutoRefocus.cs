using System.Collections.Generic;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class AutoRefocus : ModuleBase
{
	private ulong focusTarget = 3758096384uL;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoRefocusTitle"),
		Description = LanguageManager.Get("AutoRefocusDescription"),
		Category = ModuleCategory.System
	};

	protected override void Init()
	{
		focusTarget = 3758096384uL;
		OmenServiceBase<TargetManager>.Instance().RegPostSetFocusTarget(OnSetFocusTarget);
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += OnZoneChange;
		ManagerBase<PlayersManager>.Instance().ReceivePlayersAround += OnReceivePlayerAround;
	}

	protected override void Uninit()
	{
		ManagerBase<PlayersManager>.Instance().ReceivePlayersAround -= OnReceivePlayerAround;
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChange;
		OmenServiceBase<TargetManager>.Instance().Unreg(new TargetManager.PostSetFocusTargetDelegate[1] { OnSetFocusTarget });
	}

	private unsafe void OnReceivePlayerAround(IReadOnlyList<IPlayerCharacter> characters)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (GameState.ContentFinderCondition != 0 && focusTarget != 3758096384u && TargetManager.FocusTarget == null)
		{
			((TargetSystem)TargetManager.ToStruct()).SetFocusTargetByObjectId(GameObjectId.op_Implicit(focusTarget));
		}
	}

	private void OnSetFocusTarget(GameObjectId gameObjectID)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		focusTarget = GameObjectId.op_Implicit(gameObjectID);
	}

	private void OnZoneChange(uint u)
	{
		focusTarget = 3758096384uL;
	}
}
