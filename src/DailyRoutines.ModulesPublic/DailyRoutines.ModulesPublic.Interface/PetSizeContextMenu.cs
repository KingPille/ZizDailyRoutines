using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using Lumina.Text.ReadOnly;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic.Interface;

public class PetSizeContextMenu : ModuleBase
{
	private sealed class UpperContainerItem : ContextMenuEntry
	{
		public override string Identifier => "PetSizeContextMenu";

		public unsafe override ContextMenuItem? Create(ContextMenuOpenedArgs args)
		{
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			BattleChara* localPlayer = Control.GetLocalPlayer();
			if (localPlayer == null)
			{
				return null;
			}
			byte classJob = ((BattleChara)localPlayer).ClassJob;
			if (((uint)(classJob - 26) > 1u && classJob != 43) || 1 == 0)
			{
				return null;
			}
			BattleChara* ptr = ((CharacterManager)CharacterManager.Instance()).LookupPetByOwnerObject(localPlayer);
			if (ptr == null)
			{
				return null;
			}
			string text = null;
			if (args.TargetObjectID == GameObjectId.op_Implicit(((BattleChara)ptr).GetGameObjectId()))
			{
				text = LuminaWrapper.GetAddonText(17709u);
			}
			if (args.TargetContentID == LocalPlayerState.ContentID)
			{
				text = LanguageManager.Get((LocalPlayerState.ClassJob == 43) ? "PetSizeContextMenu-ContextMenu-Self-Beast" : "PetSizeContextMenu-ContextMenu-Self-Pet");
			}
			if (!string.IsNullOrEmpty(text))
			{
				return new ContextMenuItem
				{
					Name = ReadOnlySeString.op_Implicit(text),
					Submenu = new ContextMenuSubmenu
					{
						Title = ReadOnlySeString.op_Implicit(text),
						Entries = new global::_003C_003Ez__ReadOnlyArray<ContextMenuEntry>(new ContextMenuEntry[3]
						{
							new PetSizeSmallItem(((BattleChara)ptr).NameString),
							new PetSizeMediumItem(((BattleChara)ptr).NameString),
							new PetSizeLargeItem(((BattleChara)ptr).NameString)
						})
					}
				};
			}
			return null;
		}
	}

	private abstract class PetSizeAdjustItem(string petName) : ContextMenuEntry
	{
		public override string Identifier => "PetSizeContextMenu";

		protected abstract uint AddonTextID { get; }

		protected abstract uint BeastMasterAddonTextID { get; }

		protected abstract string TextCommandParam { get; }

		public override ContextMenuItem Create(ContextMenuOpenedArgs args)
		{
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			bool flag = LocalPlayerState.ClassJob == 43;
			return new ContextMenuItem
			{
				Name = ReadOnlySeString.op_Implicit(flag ? LuminaWrapper.GetAddonText(BeastMasterAddonTextID) : LanguageManager.Get("PetSizeContextMenu-ContextMenu-Sub-Pet", LuminaWrapper.GetAddonText(AddonTextID))),
				OnClicked = delegate
				{
					if (LocalPlayerState.ClassJob == 43)
					{
						OmenServiceBase<ChatManager>.Instance().SendMessage("/beastsize " + petName + " " + TextCommandParam);
					}
					else
					{
						OmenServiceBase<ChatManager>.Instance().SendMessage("/petsize all " + TextCommandParam);
					}
				}
			};
		}
	}

	private sealed class PetSizeSmallItem : PetSizeAdjustItem
	{
		protected override uint AddonTextID => 6373u;

		protected override uint BeastMasterAddonTextID => 17689u;

		protected override string TextCommandParam => "small";

		public PetSizeSmallItem(string petName)
			: base(petName)
		{
		}
	}

	private sealed class PetSizeMediumItem : PetSizeAdjustItem
	{
		protected override uint AddonTextID => 6372u;

		protected override uint BeastMasterAddonTextID => 17690u;

		protected override string TextCommandParam => "medium";

		public PetSizeMediumItem(string petName)
			: base(petName)
		{
		}
	}

	private sealed class PetSizeLargeItem : PetSizeAdjustItem
	{
		protected override uint AddonTextID => 6371u;

		protected override uint BeastMasterAddonTextID => 17691u;

		protected override string TextCommandParam => "large";

		public PetSizeLargeItem(string petName)
			: base(petName)
		{
		}
	}

	private UpperContainerItem containerItem;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("PetSizeContextMenuTitle"),
		Description = LanguageManager.Get("PetSizeContextMenuDescription"),
		Category = ModuleCategory.Interface
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected override void Init()
	{
		containerItem = new UpperContainerItem();
		OmenServiceBase<ContextMenuManager>.Instance().Reg(containerItem);
	}

	protected override void Uninit()
	{
		OmenServiceBase<ContextMenuManager>.Instance().Unreg(containerItem);
	}
}
