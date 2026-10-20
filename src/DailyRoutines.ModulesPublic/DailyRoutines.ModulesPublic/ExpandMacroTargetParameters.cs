using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Hooking;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Common.Math;
using FFXIVClientStructs.Interop;
using InteropGenerator.Runtime;
using Lumina.Excel.Sheets;
using OmenTools.Extensions;
using OmenTools.Info.Lumina;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class ExpandMacroTargetParameters : ModuleBase
{
	private unsafe delegate float? BattleCharaMetric(BattleChara* chara);

	private unsafe delegate bool BattleCharaPredicate(BattleChara* chara);

	private Hook<ResolvePlaceholder>? ResolvePlaceholderHook;

	private static readonly FrozenDictionary<string, (string Description, Func<nint> Handler)> Arguments = new Dictionary<string, (string, Func<nint>)>
	{
		["<lowhpmeandmember>"] = (LanguageManager.Get("ExpandMacroTargetParameters-Param-LowHpMeAndMember"), LowHPMeAndMemberHandler),
		["<lowhpmember>"] = (LanguageManager.Get("ExpandMacroTargetParameters-Param-LowHpMember"), LowHPMemberHandler),
		["<lowhpenemy>"] = (LanguageManager.Get("ExpandMacroTargetParameters-Param-LowHpEnemy"), LowHPEnemyHandler),
		["<deadmember>"] = (LanguageManager.Get("ExpandMacroTargetParameters-Param-DeadMember"), DeadMemberHandler),
		["<nearmember>"] = (LanguageManager.Get("ExpandMacroTargetParameters-Param-NearMember"), NearMemberHandler),
		["<farmember>"] = (LanguageManager.Get("ExpandMacroTargetParameters-Param-FarMember"), FarMemberHandler),
		["<nearenemy>"] = (LanguageManager.Get("ExpandMacroTargetParameters-Param-NearEnemy"), NearEnemyHandler),
		["<farenemy>"] = (LanguageManager.Get("ExpandMacroTargetParameters-Param-FarEnemy"), FarEnemyHandler),
		["<dispellablemeandmember>"] = (LanguageManager.Get("ExpandMacroTargetParameters-Param-DispellableMeAndMember"), DispellableMeAndMemberHandler),
		["<dispellablemember>"] = (LanguageManager.Get("ExpandMacroTargetParameters-Param-DispellableMember"), DispellableMemberHandler),
		["<lowesthateenemy>"] = (LanguageManager.Get("ExpandMacroTargetParameters-Param-LowestHateEnemy"), LowestHateEnemyHandler),
		["<lowesthateinlistenemy>"] = (LanguageManager.Get("ExpandMacroTargetParameters-Param-LowestHateInListEnemy"), LowestHateEnemyInListHandler)
	}.ToFrozenDictionary();

	private static readonly FrozenDictionary<string, (string Description, Func<uint, nint> Handler)> StartWithArguments = new Dictionary<string, (string, Func<uint, nint>)>
	{
		["<meandmemberstatus:"] = (LanguageManager.Get("ExpandMacroTargetParameters-Param-MeAndMemberStatus"), MeAndMemberStatusHandler),
		["<memberstatus:"] = (LanguageManager.Get("ExpandMacroTargetParameters-Param-MemberStatus"), MemberStatusHandler),
		["<enemystatus:"] = (LanguageManager.Get("ExpandMacroTargetParameters-Param-EnemyStatus"), EnemyStatusHandler)
	}.ToFrozenDictionary();

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("ExpandMacroTargetParametersTitle"),
		Description = LanguageManager.Get("ExpandMacroTargetParametersDescription"),
		Category = ModuleCategory.System
	};

	protected unsafe override void Init()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		ResolvePlaceholderHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromMemberFunction<ResolvePlaceholder>(typeof(MemberFunctionPointers), "ResolvePlaceholder", new ResolvePlaceholder(ResolvePlaceholderDetour));
		ResolvePlaceholderHook.Enable();
	}

	protected override void ConfigUI()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		TableDisposable val = ImRaii.Table(ImU8String.op_Implicit("ParametersTable"), 2, (ImGuiTableFlags)1920);
		try
		{
			if (!val)
			{
				return;
			}
			ImGui.TableSetupColumn(ImU8String.op_Implicit("参数"), (ImGuiTableColumnFlags)4, 20f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("描述"), (ImGuiTableColumnFlags)4, 50f, 0u);
			foreach (KeyValuePair<string, (string, Func<nint>)> argument in Arguments)
			{
				ImGui.TableNextRow();
				ImGui.TableNextColumn();
				Vector4 vector = KnownColor.Chocolate.ToVector4();
				ImGui.TextColored(ref vector, ImU8String.op_Implicit(argument.Key));
				if (ImGui.IsItemHovered())
				{
					ImGui.SetMouseCursor((ImGuiMouseCursor)7);
				}
				if (ImGui.IsItemClicked())
				{
					ImGui.SetClipboardText(ImU8String.op_Implicit(argument.Key));
					OmenServiceBase<NotifyHelper>.Instance().NotificationSuccess(LanguageManager.Get("CopiedToClipboard") + ": " + argument.Key);
				}
				ImGui.TableNextColumn();
				ImGui.TextUnformatted(ImU8String.op_Implicit(argument.Value.Item1));
			}
			foreach (KeyValuePair<string, (string, Func<uint, nint>)> startWithArgument in StartWithArguments)
			{
				ImGui.TableNextRow();
				ImGui.TableNextColumn();
				Vector4 vector = KnownColor.Chocolate.ToVector4();
				ImU8String val2 = new ImU8String(3, 1);
				((ImU8String)(ref val2)).AppendFormatted<string>(startWithArgument.Key);
				((ImU8String)(ref val2)).AppendLiteral("ID>");
				ImGui.TextColored(ref vector, val2);
				if (ImGui.IsItemHovered())
				{
					ImGui.SetMouseCursor((ImGuiMouseCursor)7);
				}
				if (ImGui.IsItemClicked())
				{
					ImU8String clipboardText = new ImU8String(3, 1);
					((ImU8String)(ref clipboardText)).AppendFormatted<string>(startWithArgument.Key);
					((ImU8String)(ref clipboardText)).AppendLiteral("ID>");
					ImGui.SetClipboardText(clipboardText);
					OmenServiceBase<NotifyHelper>.Instance().NotificationSuccess(LanguageManager.Get("CopiedToClipboard") + ": " + startWithArgument.Key);
				}
				ImGui.TableNextColumn();
				ImGui.TextUnformatted(ImU8String.op_Implicit(startWithArgument.Value.Item1));
			}
		}
		finally
		{
			((TableDisposable)(ref val)).Dispose();
		}
	}

	private unsafe GameObject* ResolvePlaceholderDetour(PronounModule* module, CStringPointer placeholder, byte unknown0, byte unknown1, bool unknown2)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		GameObject* ptr = ResolvePlaceholderHook.Original.Invoke(module, placeholder, unknown0, unknown1, unknown2);
		if (ptr != null)
		{
			return ptr;
		}
		string text = ((object)(*(CStringPointer*)(&placeholder))/*cast due to constrained. prefix*/).ToString();
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		if (Arguments.TryGetValue(text, out (string, Func<nint>) value))
		{
			return (GameObject*)value.Item2();
		}
		foreach (KeyValuePair<string, (string, Func<uint, nint>)> startWithArgument in StartWithArguments)
		{
			if (text.StartsWith(startWithArgument.Key) && uint.TryParse(text.Substring(startWithArgument.Key.Length).TrimEnd('>'), out var result))
			{
				return (GameObject*)startWithArgument.Value.Item2(result);
			}
		}
		return null;
	}

	private unsafe static nint LowHPMeAndMemberHandler()
	{
		return FindBestPartyMember(includeSelf: true, requireMultiple: false, (BattleChara* obj) => (!((BattleChara)obj).GetIsTargetable() || ((BattleChara)obj).IsDead() || ((BattleChara)obj).Health == ((BattleChara)obj).MaxHealth) ? ((float?)null) : new float?((float)((BattleChara)obj).Health / (float)((BattleChara)obj).MaxHealth));
	}

	private unsafe static nint LowHPMemberHandler()
	{
		return FindBestPartyMember(includeSelf: false, requireMultiple: true, (BattleChara* obj) => (!((BattleChara)obj).GetIsTargetable() || ((BattleChara)obj).IsDead() || ((BattleChara)obj).Health == ((BattleChara)obj).MaxHealth) ? ((float?)null) : new float?((float)((BattleChara)obj).Health / (float)((BattleChara)obj).MaxHealth));
	}

	private unsafe static nint LowHPEnemyHandler()
	{
		return FindBestEnemy((BattleChara* enemy) => (((BattleChara)enemy).IsDead() || ((BattleChara)enemy).Health == ((BattleChara)enemy).MaxHealth) ? ((float?)null) : new float?((float)((BattleChara)enemy).Health / (float)((BattleChara)enemy).MaxHealth));
	}

	private unsafe static nint DeadMemberHandler()
	{
		return FindBestPartyMember(includeSelf: false, requireMultiple: true, delegate(BattleChara* obj)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if (!((BattleChara)obj).GetIsTargetable() || !((BattleChara)obj).IsDead())
			{
				return (float?)null;
			}
			ClassJob rowOrDefault = LuminaGetter.GetRowOrDefault<ClassJob>((uint)((BattleChara)obj).ClassJob);
			byte role = ((ClassJob)(ref rowOrDefault)).Role;
			bool flag = ((role == 1 || role == 4) ? true : false);
			return flag ? 0f : 1f;
		});
	}

	private unsafe static nint NearMemberHandler()
	{
		BattleChara* localPlayer = Control.GetLocalPlayer();
		if (localPlayer == null)
		{
			return IntPtr.Zero;
		}
		return FindBestPartyMember(includeSelf: false, requireMultiple: true, (BattleChara* obj) => Vector3.DistanceSquared(Vector3.op_Implicit(((BattleChara)localPlayer).Position), Vector3.op_Implicit(((BattleChara)obj).Position)));
	}

	private unsafe static nint FarMemberHandler()
	{
		BattleChara* localPlayer = Control.GetLocalPlayer();
		if (localPlayer == null)
		{
			return IntPtr.Zero;
		}
		return FindBestPartyMember(includeSelf: false, requireMultiple: true, (BattleChara* obj) => 0f - Vector3.DistanceSquared(Vector3.op_Implicit(((BattleChara)localPlayer).Position), Vector3.op_Implicit(((BattleChara)obj).Position)));
	}

	private unsafe static nint NearEnemyHandler()
	{
		BattleChara* localPlayer = Control.GetLocalPlayer();
		if (localPlayer == null)
		{
			return IntPtr.Zero;
		}
		return FindBestEnemy((BattleChara* enemy) => Vector3.DistanceSquared(Vector3.op_Implicit(((BattleChara)localPlayer).Position), Vector3.op_Implicit(((BattleChara)enemy).Position)));
	}

	private unsafe static nint FarEnemyHandler()
	{
		BattleChara* localPlayer = Control.GetLocalPlayer();
		if (localPlayer == null)
		{
			return IntPtr.Zero;
		}
		return FindBestEnemy((BattleChara* enemy) => 0f - Vector3.DistanceSquared(Vector3.op_Implicit(((BattleChara)localPlayer).Position), Vector3.op_Implicit(((BattleChara)enemy).Position)));
	}

	private unsafe static nint DispellableMeAndMemberHandler()
	{
		return FindFirstPartyMember(includeSelf: true, requireMultiple: false, HasDispellableStatus);
	}

	private unsafe static nint DispellableMemberHandler()
	{
		return FindFirstPartyMember(includeSelf: false, requireMultiple: true, HasDispellableStatus);
	}

	private unsafe static nint MeAndMemberStatusHandler(uint id)
	{
		return FindFirstPartyMember(includeSelf: true, requireMultiple: false, (BattleChara* obj) => HasStatus(obj, id));
	}

	private unsafe static nint MemberStatusHandler(uint id)
	{
		return FindFirstPartyMember(includeSelf: false, requireMultiple: true, (BattleChara* obj) => HasStatus(obj, id));
	}

	private unsafe static nint EnemyStatusHandler(uint id)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		Span<Pointer<BattleChara>> battleCharas = ((CharacterManager)CharacterManager.Instance()).BattleCharas;
		for (int i = 0; i < battleCharas.Length; i++)
		{
			Pointer<BattleChara> val = battleCharas[i];
			if (!val.IsNull)
			{
				BattleChara* value = val.Value;
				if (CanUseActionOnEnemy((GameObject*)value) && HasStatus(value, id))
				{
					return (nint)value;
				}
			}
		}
		return IntPtr.Zero;
	}

	private unsafe static nint LowestHateEnemyHandler()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		Hater hater = ((UIState)UIState.Instance()).Hater;
		return FindBestEnemy(delegate(BattleChara* enemy)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < hater.HaterCount; i++)
			{
				HaterInfo val = ((Hater)(ref hater)).Haters[i];
				if (val.EntityId == ((BattleChara)enemy).EntityId)
				{
					return val.Enmity;
				}
			}
			return 0f;
		});
	}

	private unsafe static nint LowestHateEnemyInListHandler()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		Hater hater = ((UIState)UIState.Instance()).Hater;
		return FindBestEnemy(delegate(BattleChara* enemy)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < hater.HaterCount; i++)
			{
				HaterInfo val = ((Hater)(ref hater)).Haters[i];
				if (val.EntityId == ((BattleChara)enemy).EntityId)
				{
					return val.Enmity;
				}
			}
			return (float?)null;
		});
	}

	private unsafe static nint FindBestPartyMember(bool includeSelf, bool requireMultiple, BattleCharaMetric metric)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		AgentHUD* ptr = AgentHUD.Instance();
		if (ptr == null)
		{
			return IntPtr.Zero;
		}
		if (requireMultiple && ((AgentHUD)ptr).PartyMemberCount == 1)
		{
			return IntPtr.Zero;
		}
		BattleChara* ptr2 = null;
		float num = float.MaxValue;
		for (int i = 0; i < ((AgentHUD)ptr).PartyMemberCount; i++)
		{
			HudPartyMember val = ((AgentHUD)ptr).PartyMembers[i];
			if (!includeSelf && val.ContentId == LocalPlayerState.ContentID)
			{
				continue;
			}
			BattleChara* ptr3 = val.Object;
			if (ptr3 != null)
			{
				float? num2 = metric(ptr3);
				if (num2.HasValue && (ptr2 == null || num2.Value < num))
				{
					ptr2 = ptr3;
					num = num2.Value;
				}
			}
		}
		return (nint)ptr2;
	}

	private unsafe static nint FindFirstPartyMember(bool includeSelf, bool requireMultiple, BattleCharaPredicate predicate)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		AgentHUD* ptr = AgentHUD.Instance();
		if (ptr == null)
		{
			return IntPtr.Zero;
		}
		if (requireMultiple && ((AgentHUD)ptr).PartyMemberCount == 1)
		{
			return IntPtr.Zero;
		}
		for (int i = 0; i < ((AgentHUD)ptr).PartyMemberCount; i++)
		{
			HudPartyMember val = ((AgentHUD)ptr).PartyMembers[i];
			if (includeSelf || val.ContentId != LocalPlayerState.ContentID)
			{
				BattleChara* ptr2 = val.Object;
				if (ptr2 != null && predicate(ptr2))
				{
					return (nint)ptr2;
				}
			}
		}
		return IntPtr.Zero;
	}

	private unsafe static nint FindBestEnemy(BattleCharaMetric metric)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		CharacterManager* intPtr = CharacterManager.Instance();
		BattleChara* ptr = null;
		float num = float.MaxValue;
		Span<Pointer<BattleChara>> battleCharas = ((CharacterManager)intPtr).BattleCharas;
		for (int i = 0; i < battleCharas.Length; i++)
		{
			Pointer<BattleChara> val = battleCharas[i];
			if (val.IsNull)
			{
				continue;
			}
			BattleChara* value = val.Value;
			if (CanUseActionOnEnemy((GameObject*)value))
			{
				float? num2 = metric(value);
				if (num2.HasValue && (ptr == null || num2.Value < num))
				{
					ptr = value;
					num = num2.Value;
				}
			}
		}
		return (nint)ptr;
	}

	private unsafe static bool HasStatus(BattleChara* obj, uint id)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		Span<Status> status = ((StatusManager)((BattleChara)obj).GetStatusManager()).Status;
		for (int i = 0; i < status.Length; i++)
		{
			if (status[i].StatusId == id)
			{
				return true;
			}
		}
		return false;
	}

	private unsafe static bool HasDispellableStatus(BattleChara* obj)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		Span<Status> status = ((StatusManager)((BattleChara)obj).GetStatusManager()).Status;
		for (int i = 0; i < status.Length; i++)
		{
			Status val = status[i];
			if (Sheets.DispellableStatuses.ContainsKey(val.StatusId))
			{
				return true;
			}
		}
		return false;
	}

	private unsafe static bool CanUseActionOnEnemy(GameObject* target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		if ((int)ActionManager.ClassifyTarget((Character*)target) == 3 && ((GameObject)target).GetIsTargetable())
		{
			return ((GameObject)target).NextDistance <= 45;
		}
		return false;
	}
}
