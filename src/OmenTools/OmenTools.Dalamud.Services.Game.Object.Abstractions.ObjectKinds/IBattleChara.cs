using System;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using OmenTools.Dalamud.Services.Game.Object.ObjectKinds;

namespace OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;

public interface IBattleChara : ICharacter, IGameObject, IEquatable<IGameObject>
{
	StatusList StatusList { get; }

	bool IsCasting { get; }

	bool IsCastInterruptible { get; }

	ActionType CastActionType { get; }

	uint CastActionID { get; }

	ulong CastTargetObjectID { get; }

	IGameObject? CastTargetObject { get; }

	float CurrentCastTime { get; }

	float BaseCastTime { get; }

	float TotalCastTime { get; }

	uint CastSourceSequence { get; }

	Vector3 CastTargetLocation { get; }

	float CastInfoRotation { get; }

	uint ResponseSpellID { get; }

	ActionType ResponseActionType { get; }

	uint ResponseActionID { get; }

	uint ResponseGlobalSequence { get; }

	uint ResponseSourceSequence { get; }

	byte ResponseTargetCount { get; }

	byte ResponseFlags { get; }

	byte ForayLevel { get; }

	byte ForayElement { get; }

	string FreeCompanyName { get; }

	new unsafe BattleChara* ToStruct();

	new static IBattleChara Create(nint address)
	{
		return new BattleChara(address);
	}
}
