using System;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Extensions;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Command;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.Group;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Common.Math;
using FFXIVClientStructs.Interop;
using Lumina.Excel.Sheets;
using OmenTools.Extensions;
using OmenTools.Interop.Game.ExecuteCommand.Implementations;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class FastRidePillion : ModuleBase
{
	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static HandlerDelegate _003C0_003E__OnCommand;
	}

	private Hook<OpenContextMenu>? OpenContextMenuHook;

	private const string COMMAND = "ridepillion";

	private const float MAX_RIDE_DISTANCE_SQ = 25f;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("FastRidePillionTitle"),
		Description = LanguageManager.Get("FastRidePillionDescription", "ridepillion"),
		Category = ModuleCategory.System
	};

	protected unsafe override void Init()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Expected O, but got Unknown
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Expected O, but got Unknown
		OpenContextMenuHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromMemberFunction<OpenContextMenu>(typeof(MemberFunctionPointers), "OpenContextMenu", new OpenContextMenu(AgentOpenContextMenuDetour));
		OpenContextMenuHook.Enable();
		CommandManager commandManager = OmenServiceBase<CommandManager>.Instance();
		object obj = _003C_003EO._003C0_003E__OnCommand;
		if (obj == null)
		{
			HandlerDelegate val = OnCommand;
			_003C_003EO._003C0_003E__OnCommand = val;
			obj = (object)val;
		}
		commandManager.AddSubCommand("ridepillion", new CommandInfo((HandlerDelegate)obj)
		{
			HelpMessage = LanguageManager.Get("FastRidePillion-Command-Help")
		});
	}

	protected override void Uninit()
	{
		OmenServiceBase<CommandManager>.Instance().RemoveSubCommand("ridepillion");
	}

	protected override void ConfigUI()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		using (ImRaiiExtension.Heading1(LanguageManager.Get("Command")))
		{
			ImU8String val = new ImU8String(6, 2);
			((ImU8String)(ref val)).AppendLiteral("/pdr ");
			((ImU8String)(ref val)).AppendFormatted<string>("ridepillion");
			((ImU8String)(ref val)).AppendLiteral(" ");
			((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("FastRidePillion-Command-Help"));
			ImGui.TextUnformatted(val);
		}
	}

	private unsafe static void OnCommand(string command, string arguments)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Expected O, but got Unknown
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		BattleChara* localPlayer = ((Control)Control.Instance()).LocalPlayer;
		if (localPlayer == null || ((MountContainer)(&((BattleChara)localPlayer).Mount)).MountId != 0)
		{
			return;
		}
		Group* ptr = ((GroupManager)GroupManager.Instance()).GetGroup(false);
		if (ptr == null)
		{
			return;
		}
		for (int i = 0; i < ((Group)ptr).MemberCount; i++)
		{
			PartyMember val = ((Group)ptr).PartyMembers[i];
			BattleChara* ptr2 = ((CharacterManager)CharacterManager.Instance()).LookupBattleCharaByEntityId(val.EntityId);
			if (ptr2 != null)
			{
				Character* mountObject = ((MountContainer)(&((BattleChara)ptr2).Mount)).MountObject;
				if (((MountContainer)(&((BattleChara)ptr2).Mount)).MountId != 0 && mountObject != null && !(LocalPlayerState.DistanceTo3DSquared(Vector3.op_Implicit(((Character)mountObject).Position)) > 25f) && TryGetEmptySeat(ptr2, mountObject, out var seatIndex))
				{
					TargetCommand.Set(((BattleChara)ptr2).EntityId);
					MountCommand.RidePillion(((BattleChara)ptr2).EntityId, seatIndex);
					NotifyHelper.Toast(LanguageManager.GetSe("FastRidePillion-Notification-Rode", (object)new PlayerPayload(((BattleChara)ptr2).NameString, (uint)((BattleChara)ptr2).HomeWorld), seatIndex + 1));
					return;
				}
			}
		}
		NotifyHelper.ToastError(LanguageManager.Get("FastRidePillion-Notification-NoAvailable"));
	}

	private unsafe void AgentOpenContextMenuDetour(AgentContext* agent, bool bindToOwner, bool closeExisting)
	{
		if (!TryRidePillion(agent))
		{
			OpenContextMenuHook.Original.Invoke(agent, bindToOwner, closeExisting);
		}
	}

	private unsafe static bool TryRidePillion(AgentContext* agent)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Expected O, but got Unknown
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		BattleChara* localPlayer = ((Control)Control.Instance()).LocalPlayer;
		if (localPlayer == null || ((MountContainer)(&((BattleChara)localPlayer).Mount)).MountId != 0)
		{
			return false;
		}
		GameObjectId targetObjectId = ((AgentContext)agent).TargetObjectId;
		if (targetObjectId.ObjectId == 0 || targetObjectId.Type != 0)
		{
			return false;
		}
		Group* ptr = ((GroupManager)GroupManager.Instance()).GetGroup(false);
		if (ptr == null || !((Group)ptr).IsEntityIdInParty(targetObjectId.ObjectId))
		{
			return false;
		}
		BattleChara* ptr2 = ((CharacterManager)CharacterManager.Instance()).LookupBattleCharaByEntityId(targetObjectId.ObjectId);
		if (ptr2 == null)
		{
			return false;
		}
		Character* mountObject = ((MountContainer)(&((BattleChara)ptr2).Mount)).MountObject;
		if (((MountContainer)(&((BattleChara)ptr2).Mount)).MountId == 0 || mountObject == null)
		{
			return false;
		}
		if (LocalPlayerState.DistanceTo3DSquared(Vector3.op_Implicit(((Character)mountObject).Position)) > 25f)
		{
			return false;
		}
		if (!TryGetEmptySeat(ptr2, mountObject, out var seatIndex))
		{
			return false;
		}
		TargetCommand.Set(((BattleChara)ptr2).EntityId);
		MountCommand.RidePillion(((BattleChara)ptr2).EntityId, seatIndex);
		NotifyHelper.Toast(LanguageManager.GetSe("FastRidePillion-Notification-Rode", (object)new PlayerPayload(((BattleChara)ptr2).NameString, (uint)((BattleChara)ptr2).HomeWorld), seatIndex + 1));
		return true;
	}

	private unsafe static bool TryGetEmptySeat(BattleChara* owner, Character* mountObject, out uint seatIndex)
	{
		seatIndex = 0u;
		if (!LuminaGetter.TryGetRow<Mount>((uint)((MountContainer)(&((BattleChara)owner).Mount)).MountId, out Mount item) || ((Mount)(ref item)).ExtraSeats == 0)
		{
			return false;
		}
		Span<Pointer<BattleChara>> battleCharas = ((CharacterManager)CharacterManager.Instance()).BattleCharas;
		for (uint num = 0u; num < ((Mount)(ref item)).ExtraSeats; num++)
		{
			if (!IsSeatOccupied(battleCharas, mountObject, num))
			{
				seatIndex = num;
				return true;
			}
		}
		return false;
	}

	private unsafe static bool IsSeatOccupied(ReadOnlySpan<Pointer<BattleChara>> charas, Character* mountObject, uint seat)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Invalid comparison between Unknown and I4
		ReadOnlySpan<Pointer<BattleChara>> readOnlySpan = charas;
		for (int i = 0; i < readOnlySpan.Length; i++)
		{
			BattleChara* value = readOnlySpan.get_Item(i).Value;
			if (value != null && (int)((BattleChara)value).Mode == 10 && ((BattleChara)value).ChildObject == mountObject && ((BattleChara)value).ModeParam == seat)
			{
				return true;
			}
		}
		return false;
	}
}
