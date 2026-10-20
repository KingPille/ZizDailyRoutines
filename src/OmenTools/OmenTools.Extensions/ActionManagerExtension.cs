using FFXIVClientStructs.FFXIV.Client.Enums;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;

namespace OmenTools.Extensions;

public static class ActionManagerExtension
{
	extension(scoped ref ActionManager manager)
	{
		public static uint GetAdjustSprintActionID()
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Invalid comparison between Unknown and I4
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Invalid comparison between Unknown and I4
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			if (!LuminaGetter.TryGetRow<GeneralAction>(4u, out GeneralAction item))
			{
				return 3u;
			}
			if (LocalPlayerState.ClassJob == 0)
			{
				return ((GeneralAction)(ref item)).Action.RowId;
			}
			TerritoryIntendedUse territoryIntendedUse = GameState.TerritoryIntendedUse;
			if ((int)territoryIntendedUse != 49)
			{
				if ((int)territoryIntendedUse == 60)
				{
					ClassJob classJobData = LocalPlayerState.ClassJobData;
					if (((ClassJob)(ref classJobData)).DohDolJobIndex > -1)
					{
						return 43357u;
					}
				}
				return ((GeneralAction)(ref item)).Action.RowId;
			}
			return 31314u;
		}

		public unsafe static bool IsActionUnlocked(uint actionID)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			if (!LuminaGetter.TryGetRow<Action>(actionID, out Action item))
			{
				return false;
			}
			RowRef unlockLink = ((Action)(ref item)).UnlockLink;
			uint rowId = ((RowRef)(ref unlockLink)).RowId;
			bool num = rowId == 0 || ((UIState)UIState.Instance()).IsUnlockLinkUnlockedOrQuestCompleted(rowId, (byte)0, true);
			byte classJobLevel = ((Action)(ref item)).ClassJobLevel;
			bool flag = classJobLevel == 0 || classJobLevel <= LocalPlayerState.GetClassJobLevel(((Action)(ref item)).ClassJob.RowId);
			return num && flag;
		}
	}
}
