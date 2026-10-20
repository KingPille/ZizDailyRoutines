using System;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Common.Math;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;

namespace OmenTools.Dalamud.Services.Game.Object.ObjectKinds;

internal class BattleChara : Character, IBattleChara, ICharacter, IGameObject, IEquatable<IGameObject>
{
	protected new unsafe BattleChara* Struct => (BattleChara*)base.Address;

	private unsafe CastInfo CastInfo => ((BattleChara)Struct).CastInfo;

	public unsafe StatusList StatusList => new StatusList(((BattleChara)Struct).GetStatusManager());

	public bool IsCasting
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			CastInfo castInfo = CastInfo;
			return ((CastInfo)(ref castInfo)).IsCasting;
		}
	}

	public bool IsCastInterruptible
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			CastInfo castInfo = CastInfo;
			return ((CastInfo)(ref castInfo)).Interruptible;
		}
	}

	public ActionType CastActionType => (ActionType)CastInfo.ActionType;

	public uint CastActionID => CastInfo.ActionId;

	public ulong CastTargetObjectID => GameObjectId.op_Implicit(CastInfo.TargetId);

	public IGameObject? CastTargetObject => IOmenDalamudService<IObjectTable>.Instance().SearchByID(CastTargetObjectID);

	public float CurrentCastTime
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			if (CastInfo.CurrentCastTime == 0f)
			{
				return -1f;
			}
			return CastInfo.CurrentCastTime;
		}
	}

	public float BaseCastTime
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			if (CastInfo.BaseCastTime == 0f)
			{
				return -1f;
			}
			return CastInfo.BaseCastTime;
		}
	}

	public float TotalCastTime
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			if (CastInfo.TotalCastTime == 0f)
			{
				return -1f;
			}
			return CastInfo.TotalCastTime;
		}
	}

	public uint CastSourceSequence => CastInfo.SourceSequence;

	public Vector3 CastTargetLocation => Vector3.op_Implicit(CastInfo.TargetLocation);

	public float CastInfoRotation => CastInfo.Rotation;

	public uint ResponseSpellID => CastInfo.ResponseSpellId;

	public ActionType ResponseActionType => CastInfo.ResponseActionType;

	public uint ResponseActionID => CastInfo.ResponseActionId;

	public uint ResponseGlobalSequence => CastInfo.ResponseGlobalSequence;

	public uint ResponseSourceSequence => CastInfo.ResponseSourceSequence;

	public byte ResponseTargetCount => CastInfo.ResponseTargetCount;

	public byte ResponseFlags => CastInfo.ResponseFlags;

	public unsafe byte ForayLevel => ((ForayInfo)(&((BattleChara)Struct).ForayInfo)).Level;

	public unsafe byte ForayElement => ((ForayInfo)(&((BattleChara)Struct).ForayInfo)).Element;

	public unsafe string FreeCompanyName => ((BattleChara)Struct).FreeCompanyTagString;

	public BattleChara(nint address)
		: base(address)
	{
	}

	public new unsafe BattleChara* ToStruct()
	{
		return Struct;
	}
}
