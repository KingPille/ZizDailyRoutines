using System;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;

namespace OmenTools.Dalamud.Services.Game.Object.ObjectKinds;

internal class PlayerCharacter : BattleChara, IPlayerCharacter, IBattleChara, ICharacter, IGameObject, IEquatable<IGameObject>
{
	public unsafe override ulong TargetObjectID => GameObjectId.op_Implicit(((CharacterLookAtController)(&((LookAtContainer)(&((BattleChara)base.Struct).LookAt)).Controller)).Params[0].TargetParam.TargetId);

	public PlayerCharacter(nint address)
		: base(address)
	{
	}
}
