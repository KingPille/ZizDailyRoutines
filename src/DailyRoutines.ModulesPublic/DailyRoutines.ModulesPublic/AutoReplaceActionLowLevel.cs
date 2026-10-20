using System.Collections.Frozen;
using System.Collections.Generic;
using System.Numerics;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Hooking;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using Lumina.Excel.Sheets;
using OmenTools.Extensions;
using OmenTools.ImGuiOm;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;

namespace DailyRoutines.ModulesPublic;

public class AutoReplaceActionLowLevel : ModuleBase
{
	private delegate bool IsActionReplaceableDelegate(uint actionID);

	private unsafe delegate uint GetAdjustedActionIDDelegate(ActionManager* manager, uint actionID);

	private unsafe delegate uint GetIconIDForSlotDelegate(HotbarSlot* slot, HotbarSlotType type, uint actionID);

	private static readonly CompSig IsActionReplaceableSig = new CompSig("40 53 48 83 EC ?? 8B D9 48 8B 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 85 C0 74 ?? 48 8B 10 48 8B C8 FF 92 ?? ?? ?? ?? 8B D3");

	private Hook<IsActionReplaceableDelegate> IsActionReplaceableHook;

	private static readonly CompSig GetAdjustedActionIDSig = new CompSig("E8 ?? ?? ?? ?? 89 03 8B 03");

	private Hook<GetAdjustedActionIDDelegate> GetAdjustedActionIDHook;

	private static readonly CompSig GetIconIDForSlotSig = new CompSig("E8 ?? ?? ?? ?? 85 C0 89 83 ?? ?? ?? ?? 0F 94 C0");

	private Hook<GetIconIDForSlotDelegate> GetIconIDForSlotHook;

	private static readonly FrozenDictionary<uint, uint> ActionReplacements = new Dictionary<uint, uint>
	{
		[16534u] = 133u,
		[133u] = 124u,
		[16531u] = 135u,
		[135u] = 120u,
		[185u] = 190u,
		[3610u] = 3594u,
		[3601u] = 3600u,
		[16507u] = 7422u,
		[16481u] = 7496u,
		[162u] = 147u,
		[159u] = 25793u,
		[3577u] = 141u,
		[3576u] = 142u
	}.ToFrozenDictionary();

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoReplaceActionLowLevelTitle"),
		Description = LanguageManager.Get("AutoReplaceActionLowLevelDescription"),
		Category = ModuleCategory.Action
	};

	protected unsafe override void Init()
	{
		if (IsActionReplaceableHook == null)
		{
			IsActionReplaceableHook = IsActionReplaceableSig.GetHook<IsActionReplaceableDelegate>(IsActionReplaceableDetour);
		}
		IsActionReplaceableHook.Enable();
		if (GetAdjustedActionIDHook == null)
		{
			GetAdjustedActionIDHook = GetAdjustedActionIDSig.GetHook<GetAdjustedActionIDDelegate>(GetAdjustedActionIDDetour);
		}
		GetAdjustedActionIDHook.Enable();
		if (GetIconIDForSlotHook == null)
		{
			GetIconIDForSlotHook = GetIconIDForSlotSig.GetHook<GetIconIDForSlotDelegate>(GetIconIDForSlotDetour);
		}
		GetIconIDForSlotHook.Enable();
	}

	protected override void ConfigUI()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		TableDisposable val = ImRaii.Table(ImU8String.op_Implicit("ActionReplacementDisplayTable"), 3, (ImGuiTableFlags)24576, new Vector2(ImGui.GetContentRegionAvail().X / 2f, 0f));
		try
		{
			if (!val)
			{
				return;
			}
			ImGui.TableSetupColumn(ImU8String.op_Implicit("技能1"), (ImGuiTableColumnFlags)4, 4f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("箭头"), (ImGuiTableColumnFlags)4, 1f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("技能2"), (ImGuiTableColumnFlags)4, 4f, 0u);
			foreach (KeyValuePair<uint, uint> actionReplacement in ActionReplacements)
			{
				actionReplacement.Deconstruct(out var key, out var value);
				uint rowID = key;
				uint rowID2 = value;
				Action? row = LuminaGetter.GetRow<Action>(rowID);
				Action? row2 = LuminaGetter.GetRow<Action>(rowID2);
				if (row.HasValue && row2.HasValue)
				{
					ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
					Action value2 = row.Value;
					GameIconLookup val2 = new GameIconLookup((uint)((Action)(ref value2)).Icon, false, true, (ClientLanguage?)null);
					IDalamudTextureWrap wrapOrDefault = obj.GetFromGameIcon(ref val2).GetWrapOrDefault((IDalamudTextureWrap)null);
					ITextureProvider obj2 = DalamudServiceExtension.Instance<ITextureProvider>();
					value2 = row2.Value;
					val2 = new GameIconLookup((uint)((Action)(ref value2)).Icon, false, true, (ClientLanguage?)null);
					IDalamudTextureWrap wrapOrDefault2 = obj2.GetFromGameIcon(ref val2).GetWrapOrDefault((IDalamudTextureWrap)null);
					if (wrapOrDefault != null && wrapOrDefault2 != null)
					{
						ImGui.TableNextRow();
						ImGui.TableNextColumn();
						value2 = row.Value;
						ImGuiOm.TextImage(((object)((Action)(ref value2)).Name/*cast due to constrained. prefix*/).ToString(), wrapOrDefault.Handle, new Vector2(ImGui.GetTextLineHeightWithSpacing()));
						ImGui.TableNextColumn();
						ImGui.AlignTextToFramePadding();
						ImGui.TextUnformatted(ImU8String.op_Implicit("→"));
						ImGui.TableNextColumn();
						value2 = row2.Value;
						ImGuiOm.TextImage(((object)((Action)(ref value2)).Name/*cast due to constrained. prefix*/).ToString(), wrapOrDefault2.Handle, new Vector2(ImGui.GetTextLineHeightWithSpacing()));
					}
				}
			}
		}
		finally
		{
			((TableDisposable)(ref val)).Dispose();
		}
	}

	private unsafe uint GetAdjustedActionIDDetour(ActionManager* manager, uint actionID)
	{
		if (TryGetReplacement(actionID, out var adjustedActionID))
		{
			return adjustedActionID;
		}
		return GetAdjustedActionIDHook.Original(manager, actionID);
	}

	private unsafe uint GetIconIDForSlotDetour(HotbarSlot* slot, HotbarSlotType type, uint actionID)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if ((int)type != 1)
		{
			return GetIconIDForSlotHook.Original(slot, type, actionID);
		}
		if (TryGetReplacement(actionID, out var adjustedActionID))
		{
			if (!LuminaGetter.TryGetRow<Action>(adjustedActionID, out Action item))
			{
				return 0u;
			}
			return ((Action)(ref item)).Icon;
		}
		return GetIconIDForSlotHook.Original(slot, type, actionID);
	}

	private bool IsActionReplaceableDetour(uint actionID)
	{
		if (!ActionReplacements.ContainsKey(actionID))
		{
			return IsActionReplaceableHook.Original(actionID);
		}
		return true;
	}

	private static bool TryGetReplacement(uint actionID, out uint adjustedActionID)
	{
		uint value;
		while (true)
		{
			adjustedActionID = 0u;
			if (ActionManagerExtension.IsActionUnlocked(actionID))
			{
				return false;
			}
			if (!ActionReplacements.TryGetValue(actionID, out value))
			{
				return false;
			}
			if (ActionManagerExtension.IsActionUnlocked(value))
			{
				break;
			}
			actionID = value;
		}
		adjustedActionID = value;
		return true;
	}
}
