using System.Collections.Frozen;
using System.Numerics;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Sound;
using FFXIVClientStructs.FFXIV.Client.UI;
using Lumina.Excel.Sheets;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class InstantPlaceLocationAction : ModuleBase
{
	private static readonly FrozenSet<uint> InvalidActions = FrozenSet.Create<uint>(3573u, 7419u, 24403u, 29551u, 25837u, 29669u, 34675u, 39215u, 41507u);

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("InstantPlaceLocationActionTitle"),
		Description = LanguageManager.Get("InstantPlaceLocationActionDescription"),
		Category = ModuleCategory.Action
	};

	protected override void Init()
	{
		OmenServiceBase<UseActionManager>.Instance().RegPreUseAction(OnPreUseAction);
	}

	protected override void Uninit()
	{
		OmenServiceBase<UseActionManager>.Instance().Unreg(OnPreUseAction);
	}

	public unsafe static void OnPreUseAction(ref bool isPrevented, ref ActionType actionType, ref uint actionID, ref ulong targetID, ref uint extraParam, ref UseActionMode queueState, ref uint comboRouteID)
	{
		if ((int)actionType != 1)
		{
			return;
		}
		uint adjustedActionId = ((ActionManager)ActionManager.Instance()).GetAdjustedActionId(actionID);
		if (!InvalidActions.Contains(adjustedActionId))
		{
			IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
			Vector3 target = default(Vector3);
			if (localPlayer != null && LuminaGetter.TryGetRow<Action>(adjustedActionId, out Action item) && ((Action)(ref item)).TargetArea && ((ActionManager)ActionManager.Instance()).GetActionStatus(actionType, adjustedActionId, 3758096384uL, true, true, (uint*)null) == 0 && DalamudServiceExtension.Instance<IGameGui>().ScreenToWorld(ImGui.GetMousePos(), ref target, 100000f))
			{
				target = AdjustTargetPosition(localPlayer.Position, target, ((Action)(ref item)).Range);
				((ActionManager)ActionManager.Instance()).UseActionLocation((ActionType)1, adjustedActionId, 3758096384uL, &target, extraParam, (byte)0);
				UIGlobals.PlaySoundEffect(24u, (SoundData**)null, (SoundData**)null, (byte)0);
				isPrevented = true;
			}
		}
	}

	public static Vector3 AdjustTargetPosition(Vector3 origin, Vector3 target, float maxDistance)
	{
		Vector2 vector = origin.ToVector2();
		Vector2 vector2 = target.ToVector2();
		if (Vector2.DistanceSquared(vector, vector2) > maxDistance * maxDistance)
		{
			Vector2 vector3 = Vector2.Normalize(vector2 - vector);
			vector2 = vector + vector3 * maxDistance;
			return new Vector3(vector2.X, target.Y, vector2.Y);
		}
		return target;
	}
}
