using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using Lumina.Excel.Sheets;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Info.Game.Data.Icons;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;

namespace OmenTools.Extensions;

public static class GameObjectExtension
{
	extension(IGameObject? gameObject)
	{
		public unsafe bool TargetInteract()
		{
			if (gameObject == null)
			{
				return false;
			}
			TargetManager.Target = gameObject;
			return ((TargetSystem)TargetSystem.Instance()).InteractWithObject(gameObject.ToStruct(), true) != 0;
		}

		public unsafe bool Interact()
		{
			if (gameObject != null)
			{
				return ((TargetSystem)TargetSystem.Instance()).InteractWithObject(gameObject.ToStruct(), true) != 0;
			}
			return false;
		}

		public unsafe bool IsMTQ()
		{
			if (gameObject == null)
			{
				return false;
			}
			return (*gameObject.ToStruct()).IsMTQ();
		}

		public unsafe bool IsReachable()
		{
			if (gameObject == null)
			{
				return false;
			}
			return (*gameObject.ToStruct()).IsReachable();
		}
	}

	extension(scoped ref GameObject gameObject)
	{
		public unsafe bool IsMTQ()
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Invalid comparison between Unknown and I4
			fixed (GameObject* ptr = &gameObject)
			{
				if (ptr == null)
				{
					return false;
				}
				if (!((GameObject)ptr).TargetableStatus.IsSet<ObjectTargetableFlags>((ObjectTargetableFlags)64) || !ValidMTQObjectKinds.Contains(((GameObject)ptr).ObjectKind, null))
				{
					return false;
				}
				if (!QuestIcons.IsQuest(((GameObject)ptr).NamePlateIconId))
				{
					if ((int)((GameObject)ptr).ObjectKind == 7)
					{
						return ((GameObject)ptr).NextTargetStatus == 15;
					}
					return false;
				}
				return true;
			}
		}

		public unsafe bool IsReachable()
		{
			fixed (GameObject* ptr = &gameObject)
			{
				if (ptr == null)
				{
					return false;
				}
				BattleChara* localPlayer = Control.GetLocalPlayer();
				if (localPlayer == null)
				{
					return false;
				}
				return ((EventFramework)EventFramework.Instance()).CheckInteractRange((GameObject*)localPlayer, ptr, (byte)23, false);
			}
		}
	}

	extension(IEnumerable<IGameObject> gameObjects)
	{
		public IGameObject? FindNearest(Vector3 source, Func<IGameObject, bool> predicate)
		{
			return gameObjects.Where(predicate).MinBy((IGameObject x) => Vector3.DistanceSquared(source, x.Position));
		}
	}

	extension(scoped in Vector2 vector2)
	{
		public Vector3 ToPlayerHeight()
		{
			return vector2.ToVector3(IOmenDalamudService<IObjectTable>.Instance().LocalPlayer?.Position.Y ?? 0f);
		}
	}

	extension(uint objStrID)
	{
		public (ObjectKind Kind, uint DataID) FromObjStrID()
		{
			if (objStrID < 3000000)
			{
				if (objStrID < 1000000)
				{
					if (objStrID == 0)
					{
						return (Kind: (ObjectKind)0, DataID: 0u);
					}
					if (LuminaGetter.TryGetRow<BNpcBase>(objStrID, out BNpcBase item))
					{
						return (Kind: (ObjectKind)2, DataID: objStrID);
					}
					if (objStrID >= 100000)
					{
						uint num = objStrID + 900000;
						if (LuminaGetter.TryGetRow<BNpcBase>(num, out item))
						{
							return (Kind: (ObjectKind)2, DataID: num);
						}
					}
					return (Kind: (ObjectKind)0, DataID: 0u);
				}
				if (objStrID < 2000000)
				{
					if (LuminaGetter.TryGetRow<ENpcBase>(objStrID, out ENpcBase _))
					{
						return (Kind: (ObjectKind)3, DataID: objStrID);
					}
					return (Kind: (ObjectKind)0, DataID: 0u);
				}
				uint num2 = objStrID - 2000000;
				if (LuminaGetter.TryGetRow<Treasure>(num2, out Treasure _))
				{
					return (Kind: (ObjectKind)4, DataID: num2);
				}
				return (Kind: (ObjectKind)0, DataID: 0u);
			}
			if (objStrID < 5000000)
			{
				if (objStrID < 4000000)
				{
					uint num3 = objStrID - 3000000;
					if (LuminaGetter.TryGetRow<Aetheryte>(num3, out Aetheryte _))
					{
						return (Kind: (ObjectKind)5, DataID: num3);
					}
					return (Kind: (ObjectKind)14, DataID: num3);
				}
				uint num4 = objStrID - 4000000;
				if (LuminaGetter.TryGetRow<GatheringPoint>(num4, out GatheringPoint _))
				{
					return (Kind: (ObjectKind)6, DataID: num4);
				}
				return (Kind: (ObjectKind)0, DataID: 0u);
			}
			if (objStrID >= 10000000)
			{
				uint num5 = objStrID - 10000000;
				if (LuminaGetter.TryGetRow<HousingFurniture>(num5, out HousingFurniture _) || LuminaGetter.TryGetRow<HousingYardObject>(num5, out HousingYardObject _))
				{
					return (Kind: (ObjectKind)12, DataID: num5);
				}
			}
			if (objStrID >= 7000000)
			{
				uint num6 = objStrID - 7000000;
				if (LuminaGetter.TryGetRow<Companion>(num6, out Companion _))
				{
					return (Kind: (ObjectKind)9, DataID: num6);
				}
			}
			if (objStrID >= 5000000)
			{
				uint num7 = objStrID - 3000000;
				if (LuminaGetter.TryGetRow<EObj>(num7, out EObj _))
				{
					return (Kind: (ObjectKind)7, DataID: num7);
				}
			}
			return (Kind: (ObjectKind)0, DataID: 0u);
		}
	}

	private static readonly ObjectKind[] ValidMTQObjectKinds = (ObjectKind[])(object)new ObjectKind[2]
	{
		(ObjectKind)7,
		(ObjectKind)3
	};
}
