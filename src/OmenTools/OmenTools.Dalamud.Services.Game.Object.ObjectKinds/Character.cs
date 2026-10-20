using System;
using System.Numerics;
using Dalamud.Game.ClientState.Objects.Enums;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.Control.MoveControl;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Common.Math;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Dalamud.Services.Game.Object.Enums;
using OmenTools.Extensions;

namespace OmenTools.Dalamud.Services.Game.Object.ObjectKinds;

internal class Character : GameObject, ICharacter, IGameObject, IEquatable<IGameObject>
{
	protected internal new unsafe Character* Struct => (Character*)base.Address;

	public unsafe IBattleChara? ContainerOwner => IOmenDalamudService<IObjectTable>.Instance().CreateObjectReference((nint)((MoveController)(&((Character)Struct).MoveController)).OwnerObject) as IBattleChara;

	public unsafe short TransformationID => ((Character)Struct).TransformationId;

	public unsafe ushort StatusLoopVfxID => ((CharacterData)(&((Character)Struct).CharacterData)).StatusLoopVfxId;

	public unsafe byte SEPack => ((CharacterData)(&((Character)Struct).CharacterData)).SEPack;

	public unsafe float ModelScale => ((CharacterData)(&((Character)Struct).CharacterData)).ModelScale;

	public unsafe int ModelCharaID => ((ModelContainer)(&((Character)Struct).ModelContainer)).ModelCharaId;

	public unsafe int ModelSkeletonID => ((ModelContainer)(&((Character)Struct).ModelContainer)).ModelSkeletonId;

	public unsafe uint CurrentHp => ((CharacterData)(&((Character)Struct).CharacterData)).Health;

	public unsafe uint MaxHp => ((CharacterData)(&((Character)Struct).CharacterData)).MaxHealth;

	public unsafe uint CurrentMp => ((CharacterData)(&((Character)Struct).CharacterData)).Mana;

	public unsafe uint MaxMp => ((CharacterData)(&((Character)Struct).CharacterData)).MaxMana;

	public unsafe uint CurrentGp => ((CharacterData)(&((Character)Struct).CharacterData)).GatheringPoints;

	public unsafe uint MaxGp => ((CharacterData)(&((Character)Struct).CharacterData)).MaxGatheringPoints;

	public unsafe uint CurrentCp => ((CharacterData)(&((Character)Struct).CharacterData)).CraftingPoints;

	public unsafe uint MaxCp => ((CharacterData)(&((Character)Struct).CharacterData)).MaxCraftingPoints;

	public unsafe ushort TitleID => ((CharacterData)(&((Character)Struct).CharacterData)).TitleId;

	public unsafe byte Icon => ((CharacterData)(&((Character)Struct).CharacterData)).Icon;

	public unsafe byte ENPCMap => ((CharacterData)(&((Character)Struct).CharacterData)).Map;

	public unsafe BattalionFlags Battalion => (BattalionFlags)((CharacterData)(&((Character)Struct).CharacterData)).Battalion;

	public unsafe byte ShieldPercentage => ((CharacterData)(&((Character)Struct).CharacterData)).ShieldValue;

	public unsafe byte CharacterFlags => ((CharacterData)(&((Character)Struct).CharacterData)).Flags;

	public unsafe byte CombatTagType => ((CharacterData)(&((Character)Struct).CharacterData)).CombatTagType;

	public unsafe ulong CombatTaggerObjectID => GameObjectId.op_Implicit(((CharacterData)(&((Character)Struct).CharacterData)).CombatTaggerId);

	public unsafe RowRef<ClassJob> ClassJob => ((CharacterData)(&((Character)Struct).CharacterData)).ClassJob.ToLuminaRowRef<ClassJob>();

	public unsafe byte Level => ((CharacterData)(&((Character)Struct).CharacterData)).Level;

	public unsafe byte[] Customize => ((CustomizeData)(&((DrawDataContainer)(&((Character)Struct).DrawData)).CustomizeData)).Data.ToArray();

	public unsafe string CompanyTag => ((Character)Struct).FreeCompanyTagString;

	public unsafe float Alpha => ((Character)Struct).Alpha;

	public unsafe uint NameID => ((Character)Struct).NameId;

	public unsafe ulong AccountID => ((Character)Struct).AccountId;

	public unsafe ulong ContentID => ((Character)Struct).ContentId;

	public unsafe ulong SoftTargetObjectID => GameObjectId.op_Implicit(((Character)Struct).GetSoftTargetId());

	public IGameObject? SoftTargetObject => IOmenDalamudService<IObjectTable>.Instance().SearchByID(SoftTargetObjectID);

	public unsafe CharacterModes Mode => ((Character)Struct).Mode;

	public unsafe byte ModeParam => ((Character)Struct).ModeParam;

	public unsafe bool IsOffhandDrawn => ((LifeSkillContainer)(&((Character)Struct).LifeSkillContainer)).IsOffhandDrawn;

	public unsafe bool IsWeaponDrawn => ((Character)Struct).IsWeaponDrawn;

	public unsafe bool IsSwimming => ((MoveController)(&((Character)Struct).MoveController)).IsSwimming;

	public unsafe bool IsMounted => ((Character)Struct).IsMounted();

	public unsafe bool IsInPvP => ((Character)Struct).IsInPvP();

	public unsafe uint EventNPCInstanceID => ((Character)Struct).EventNpcInstanceId;

	public unsafe byte RelationFlags => ((Character)Struct).RelationFlags;

	public unsafe byte ActorControlFlags => ((Character)Struct).ActorControlFlags;

	public unsafe float CastRotation => ((Character)Struct).CastRotation;

	public unsafe ICharacter? ChildObject => IOmenDalamudService<IObjectTable>.Instance().CreateObjectReference((nint)((Character)Struct).ChildObject) as ICharacter;

	public unsafe uint CompanionOwnerID => ((Character)Struct).CompanionOwnerId;

	public unsafe ObjectType ObjectType => ((Character)Struct).ObjectType;

	public unsafe byte GMRank => ((Character)Struct).GMRank;

	public unsafe byte SoundVolumeCategory => ((Character)Struct).SoundVolumeCategory;

	public unsafe byte SoundVolumeCategoryOverride => ((Character)Struct).SoundVolumeCategoryOverride;

	public unsafe ushort MountID => ((MountContainer)(&((Character)Struct).Mount)).MountId;

	public unsafe float DismountTimer => ((MountContainer)(&((Character)Struct).Mount)).DismountTimer;

	public unsafe byte MountFlags => ((MountContainer)(&((Character)Struct).Mount)).Flags;

	public unsafe ICharacter? MountObject => IOmenDalamudService<IObjectTable>.Instance().CreateObjectReference((nint)((MountContainer)(&((Character)Struct).Mount)).MountObject) as ICharacter;

	public unsafe ushort CompanionID => ((CompanionContainer)(&((Character)Struct).CompanionData)).CompanionId;

	public unsafe ushort FollowMountID => ((CompanionContainer)(&((Character)Struct).CompanionData)).FollowMountId;

	public unsafe ICharacter? CompanionObject => IOmenDalamudService<IObjectTable>.Instance().CreateObjectReference((nint)((CompanionContainer)(&((Character)Struct).CompanionData)).CompanionObject) as ICharacter;

	public unsafe ushort OrnamentID => ((OrnamentContainer)(&((Character)Struct).OrnamentData)).OrnamentId;

	public unsafe ICharacter? OrnamentObject => IOmenDalamudService<IObjectTable>.Instance().CreateObjectReference((nint)((OrnamentContainer)(&((Character)Struct).OrnamentData)).OrnamentObject) as ICharacter;

	public unsafe int ModelCharaID2 => ((ModelContainer)(&((Character)Struct).ModelContainer)).ModelCharaId_2;

	public unsafe int ModelSkeletonID2 => ((ModelContainer)(&((Character)Struct).ModelContainer)).ModelSkeletonId_2;

	public unsafe byte ModelScaleID => ((ModelContainer)(&((Character)Struct).ModelContainer)).ModelScaleId;

	public unsafe byte ModelAttributeFlags => ((ModelContainer)(&((Character)Struct).ModelContainer)).ModeAttributeFlags;

	public unsafe float UnscaledRadius => ((ModelContainer)(&((Character)Struct).ModelContainer)).UnscaledRadius;

	public unsafe byte RepresentationNameType => ((RepresentationContainer)(&((Character)Struct).RepresentationContainer)).NameType;

	public unsafe string NameOverride
	{
		get
		{
			if (((RepresentationContainer)(&((Character)Struct).RepresentationContainer)).NameOverride != null)
			{
				return ((object)(*(Utf8String*)((RepresentationContainer)(&((Character)Struct).RepresentationContainer)).NameOverride)/*cast due to constrained. prefix*/).ToString();
			}
			return string.Empty;
		}
	}

	public unsafe float RepresentationUpdateTimer => ((RepresentationContainer)(&((Character)Struct).RepresentationContainer)).UpdateTimer;

	public unsafe ushort VoiceID => ((VfxContainer)(&((Character)Struct).Vfx)).VoiceId;

	public unsafe byte TimelineModelState => ((TimelineContainer)(&((Character)Struct).Timeline)).ModelState;

	public unsafe float OverallSpeed => ((TimelineContainer)(&((Character)Struct).Timeline)).OverallSpeed;

	public unsafe ushort BaseOverride => ((TimelineContainer)(&((Character)Struct).Timeline)).BaseOverride;

	public unsafe ushort LipsOverride => ((TimelineContainer)(&((Character)Struct).Timeline)).LipsOverride;

	public unsafe ushort BannerTimelineRowID => ((TimelineContainer)(&((Character)Struct).Timeline)).BannerTimelineRowId;

	public unsafe byte BannerFacialRowID => ((TimelineContainer)(&((Character)Struct).Timeline)).BannerFacialRowId;

	public unsafe uint BannerTimelineNameOffset => ((TimelineContainer)(&((Character)Struct).Timeline)).BannerTimelineNameOffset;

	public unsafe uint BannerTimelineAdditionalData => ((TimelineContainer)(&((Character)Struct).Timeline)).BannerTimelineAdditionalData;

	public unsafe int BannerTimelineIcon => ((TimelineContainer)(&((Character)Struct).Timeline)).BannerTimelineIcon;

	public unsafe ushort BannerTimelineUnlockCondition => ((TimelineContainer)(&((Character)Struct).Timeline)).BannerTimelineUnlockCondition;

	public unsafe ushort BannerTimelineSortKey => ((TimelineContainer)(&((Character)Struct).Timeline)).BannerTimelineSortKey;

	public unsafe byte BannerTimelineType => ((TimelineContainer)(&((Character)Struct).Timeline)).BannerTimelineType;

	public unsafe byte BannerTimelineAcceptClassJobCategory => ((TimelineContainer)(&((Character)Struct).Timeline)).BannerTimelineAcceptClassJobCategory;

	public unsafe byte BannerTimelineCategory => ((TimelineContainer)(&((Character)Struct).Timeline)).BannerTimelineCategory;

	public unsafe float BannerRequestStartTimestamp => ((TimelineContainer)(&((Character)Struct).Timeline)).BannerRequestStartTimestamp;

	public unsafe Vector3 CameraVector => Vector3.op_Implicit(((LookAtContainer)(&((Character)Struct).LookAt)).CameraVector);

	public unsafe bool IsFacingCamera => ((LookAtContainer)(&((Character)Struct).LookAt)).IsFacingCamera;

	public unsafe Vector2 BannerHeadDirection => Vector2.op_Implicit(((LookAtContainer)(&((Character)Struct).LookAt)).BannerHeadDirection);

	public unsafe Vector2 BannerEyeDirection => Vector2.op_Implicit(((LookAtContainer)(&((Character)Struct).LookAt)).BannerEyeDirection);

	public unsafe BannerCameraFollowFlags BannerCameraFollowFlag => ((LookAtContainer)(&((Character)Struct).LookAt)).BannerCameraFollowFlag;

	public unsafe bool IsHatHidden => ((DrawDataContainer)(&((Character)Struct).DrawData)).IsHatHidden;

	public unsafe bool IsWeaponHidden => ((DrawDataContainer)(&((Character)Struct).DrawData)).IsWeaponHidden;

	public unsafe bool IsVisorToggled => ((DrawDataContainer)(&((Character)Struct).DrawData)).IsVisorToggled;

	public unsafe bool VieraEarsHidden => ((DrawDataContainer)(&((Character)Struct).DrawData)).VieraEarsHidden;

	public unsafe byte FreeCompanyCrestBitfield => ((DrawDataContainer)(&((Character)Struct).DrawData)).FreeCompanyCrestBitfield;

	public unsafe ulong FreeCompanyCrestDataValue => ((CrestData)(&((DrawDataContainer)(&((Character)Struct).DrawData)).FreeCompanyCrestData)).Data;

	public unsafe byte FreeCompanyCrestCharge => ((CrestData)(&((DrawDataContainer)(&((Character)Struct).DrawData)).FreeCompanyCrestData)).Charge;

	public unsafe byte FreeCompanyCrestOrdinaryTinctures => ((CrestData)(&((DrawDataContainer)(&((Character)Struct).DrawData)).FreeCompanyCrestData)).OrdinaryTinctures;

	public unsafe byte TransformationStanceChangeID => ((TransformationContainer)(&((Character)Struct).Transformation)).StanceChangeId;

	public unsafe uint TransformationStanceChangeState => ((TransformationContainer)(&((Character)Struct).Transformation)).StanceChangeState;

	public unsafe float TransformationTimer => ((TransformationContainer)(&((Character)Struct).Transformation)).Timer;

	public unsafe ICharacter? TransformationCopyObject => IOmenDalamudService<IObjectTable>.Instance().CreateObjectReference((nint)((TransformationContainer)(&((Character)Struct).Transformation)).CopyObject) as ICharacter;

	public unsafe TransformationFlags TransformationFlags => ((TransformationContainer)(&((Character)Struct).Transformation)).Flags;

	public unsafe ushort TransformationNPCEquipID => ((TransformationContainer)(&((Character)Struct).Transformation)).NpcEquipId;

	public unsafe float CurrentFloatHeight => ((EffectContainer)(&((Character)Struct).Effects)).CurrentFloatHeight;

	public unsafe float TargetFloatHeight => ((EffectContainer)(&((Character)Struct).Effects)).TargetFloatHeight;

	public unsafe float FloatHeightChangeSpeed => ((EffectContainer)(&((Character)Struct).Effects)).FloatHeightChangeSpeed;

	public unsafe StatusEffect StatusEffects => ((EffectContainer)(&((Character)Struct).Effects)).StatusEffects;

	public unsafe int MountTiltSetupState1 => ((EffectContainer)(&((Character)Struct).Effects)).MountTiltSetupState1;

	public unsafe int MountTiltSetupState2 => ((EffectContainer)(&((Character)Struct).Effects)).MountTiltSetupState2;

	public unsafe TiltOrigin MountGroundTiltOrigin => ((EffectContainer)(&((Character)Struct).Effects)).MountGroundTiltOrigin;

	public unsafe float MountGroundTiltAngle => ((EffectContainer)(&((Character)Struct).Effects)).MountGroundTiltAngle;

	public unsafe float MountGroundTiltSpeed => ((EffectContainer)(&((Character)Struct).Effects)).MountGroundTiltSpeed;

	public unsafe TiltFlags MountGroundTiltFlags => ((EffectContainer)(&((Character)Struct).Effects)).MountGroundTiltFlags;

	public unsafe TiltOrigin MountFlightSwimTiltOrigin => ((EffectContainer)(&((Character)Struct).Effects)).MountFlightSwimTiltOrigin;

	public unsafe float MountFlightSwimTiltAngle => ((EffectContainer)(&((Character)Struct).Effects)).MountFlightSwimTiltAngle;

	public unsafe float MountFlightSwimTiltSpeed => ((EffectContainer)(&((Character)Struct).Effects)).MountFlightSwimTiltSpeed;

	public unsafe TiltFlags MountFlightSwimTiltFlags => ((EffectContainer)(&((Character)Struct).Effects)).MountFlightSwimTiltFlags;

	public unsafe TiltOrigin RiderGroundTiltOrigin => ((EffectContainer)(&((Character)Struct).Effects)).RiderGroundTiltOrigin;

	public unsafe float RiderGroundTiltAngle => ((EffectContainer)(&((Character)Struct).Effects)).RiderGroundTiltAngle;

	public unsafe float RiderGroundTiltSpeed => ((EffectContainer)(&((Character)Struct).Effects)).RiderGroundTiltSpeed;

	public unsafe TiltFlags RiderGroundTiltFlags => ((EffectContainer)(&((Character)Struct).Effects)).RiderGroundTiltFlags;

	public unsafe TiltOrigin RiderFlightSwimTiltOrigin => ((EffectContainer)(&((Character)Struct).Effects)).RiderFlightSwimTiltOrigin;

	public unsafe float RiderFlightSwimTiltAngle => ((EffectContainer)(&((Character)Struct).Effects)).RiderFlightSwimTiltAngle;

	public unsafe float RiderFlightSwimTiltSpeed => ((EffectContainer)(&((Character)Struct).Effects)).RiderFlightSwimTiltSpeed;

	public unsafe TiltFlags RiderFlightSwimReverseTilt => ((EffectContainer)(&((Character)Struct).Effects)).RiderFlightSwimReverseTilt;

	public unsafe ushort BalloonDefaultID => ((Balloon)(&((Character)Struct).Balloon)).DefaultBalloonId;

	public unsafe ushort BalloonCurrentID => ((Balloon)(&((Character)Struct).Balloon)).NowPlayingBalloonId;

	public unsafe float BalloonPlayTimer => ((Balloon)(&((Character)Struct).Balloon)).PlayTimer;

	public unsafe BalloonType BalloonType => ((Balloon)(&((Character)Struct).Balloon)).Type;

	public unsafe BalloonState BalloonState => ((Balloon)(&((Character)Struct).Balloon)).State;

	public unsafe float NPCYellPlayTimer => ((NpcYellBalloon)(&((Character)Struct).YellBalloon)).PlayTimer;

	public unsafe float NPCYellDelayTime => ((NpcYellBalloon)(&((Character)Struct).YellBalloon)).DelayTime;

	public unsafe NpcYellBalloonState NPCYellState => ((NpcYellBalloon)(&((Character)Struct).YellBalloon)).State;

	public unsafe NpcYellBalloonCloseType NPCYellCloseType => ((NpcYellBalloon)(&((Character)Struct).YellBalloon)).CloseType;

	public unsafe byte NPCYellParentBone => ((NpcYellBalloon)(&((Character)Struct).YellBalloon)).ParentBone;

	public unsafe NpcYellBalloonFlags NPCYellFlags => ((NpcYellBalloon)(&((Character)Struct).YellBalloon)).Flags;

	public unsafe ICharacter? NPCYellCharacter => IOmenDalamudService<IObjectTable>.Instance().CreateObjectReference((nint)((NpcYellBalloon)(&((Character)Struct).YellBalloon)).Character) as ICharacter;

	public unsafe RowRef<OnlineStatus> OnlineStatus => ((CharacterData)(&((Character)Struct).CharacterData)).OnlineStatus.ToLuminaRowRef<OnlineStatus>();

	public unsafe ulong EmoteTargetObjectID => GameObjectId.op_Implicit(((EmoteController)(&((Character)Struct).EmoteController)).Target);

	public IGameObject? EmoteTargetObject => IOmenDalamudService<IObjectTable>.Instance().SearchByID(EmoteTargetObjectID);

	public unsafe IBattleChara? EmoteOwnerObject => IOmenDalamudService<IObjectTable>.Instance().CreateObjectReference((nint)((EmoteController)(&((Character)Struct).EmoteController)).OwnerObject) as IBattleChara;

	public unsafe byte EmoteStance => ((EmoteController)(&((Character)Struct).EmoteController)).Stance;

	public unsafe PoseType CurrentPoseType => ((EmoteController)(&((Character)Struct).EmoteController)).CurrentPoseType;

	public unsafe byte CPoseState => ((EmoteController)(&((Character)Struct).EmoteController)).CPoseState;

	public unsafe bool IsEmoting => ((EmoteController)(&((Character)Struct).EmoteController)).IsEmoting();

	public unsafe bool IsInEmoteLoop => ((EmoteController)(&((Character)Struct).EmoteController)).IsInEmoteLoop();

	public unsafe bool IsWanderer => ((Character)Struct).IsWanderer();

	public unsafe bool IsTraveler => ((Character)Struct).IsTraveler();

	public unsafe bool IsVoyager => ((Character)Struct).IsVoyager();

	public unsafe RowRef<World> CurrentWorld => ((Character)Struct).CurrentWorld.ToLuminaRowRef<World>();

	public unsafe RowRef<World> HomeWorld => ((Character)Struct).HomeWorld.ToLuminaRowRef<World>();

	public unsafe override ulong TargetObjectID => GameObjectId.op_Implicit(((Character)Struct).TargetId);

	public unsafe StatusFlags StatusFlags => (StatusFlags)((((Character)Struct).IsHostile ? 1 : 0) | (((Character)Struct).InCombat ? 2 : 0) | (((Character)Struct).IsWeaponDrawn ? 4 : 0) | (((LifeSkillContainer)(&((Character)Struct).LifeSkillContainer)).IsOffhandDrawn ? 8 : 0) | (((Character)Struct).IsPartyMember ? 16 : 0) | (((Character)Struct).IsAllianceMember ? 32 : 0) | (((Character)Struct).IsFriend ? 64 : 0) | (((Character)Struct).IsCasting ? 128 : 0));

	public unsafe RowRef<Emote>? CurrentEmote
	{
		get
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			ushort emoteId = ((EmoteController)(&((Character)Struct).EmoteController)).EmoteId;
			if (emoteId != 0)
			{
				return emoteId.ToLuminaRowRef<Emote>();
			}
			return null;
		}
	}

	public unsafe RowRef<Mount>? CurrentMount
	{
		get
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			if (((Character)Struct).IsNotMounted())
			{
				return null;
			}
			ushort mountId = ((MountContainer)(&((Character)Struct).Mount)).MountId;
			if (mountId != 0)
			{
				return mountId.ToLuminaRowRef<Mount>();
			}
			return null;
		}
	}

	public unsafe RowRef<Ornament>? CurrentOrnament
	{
		get
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			ushort ornamentId = ((OrnamentContainer)(&((Character)Struct).OrnamentData)).OrnamentId;
			if (ornamentId != 0)
			{
				return ornamentId.ToLuminaRowRef<Ornament>();
			}
			return null;
		}
	}

	public unsafe RowRef<Companion>? CurrentMinion
	{
		get
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			if (((Character)Struct).ChildObject != null)
			{
				return ((Character)((Character)Struct).ChildObject).BaseId.ToLuminaRowRef<Companion>();
			}
			ushort companionId = ((CompanionContainer)(&((Character)Struct).CompanionData)).CompanionId;
			if (companionId != 0)
			{
				return companionId.ToLuminaRowRef<Companion>();
			}
			return null;
		}
	}

	public Character(nint address)
		: base(address)
	{
	}

	public new unsafe Character* ToStruct()
	{
		return Struct;
	}
}
