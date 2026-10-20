using System;
using System.Collections.Generic;
using System.Numerics;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Interface;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using Dalamud.Utility.Numerics;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using Lumina.Excel.Sheets;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Info.Game.Packets.Upstream;
using OmenTools.Interop.Game.Helpers;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class AutoActionAlignCamera : ModuleBase
{
	private class Config : ModuleConfig
	{
		public Dictionary<uint, bool> ActionEnabled = new Dictionary<uint, bool>
		{
			[94u] = true,
			[29494u] = true,
			[24401u] = true,
			[29550u] = true,
			[24402u] = true,
			[34684u] = true,
			[39210u] = true,
			[16010u] = true,
			[29430u] = true,
			[7418u] = true,
			[37008u] = true,
			[7385u] = true,
			[41506u] = true,
			[11390u] = true,
			[11414u] = true,
			[11403u] = true,
			[11383u] = true,
			[11399u] = true,
			[11388u] = true,
			[11422u] = true,
			[11428u] = true,
			[11430u] = true,
			[18296u] = true,
			[18297u] = true,
			[18299u] = true,
			[18323u] = true,
			[23283u] = true,
			[23288u] = true,
			[23289u] = true,
			[34567u] = true,
			[34571u] = true,
			[34572u] = true,
			[34581u] = true
		};

		public HashSet<uint> ActionReversed = new HashSet<uint>();
	}

	private Config config;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoActionAlignCameraTitle"),
		Description = LanguageManager.Get("AutoActionAlignCameraDescription"),
		Category = ModuleCategory.Action
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		NeedAuth = true
	};

	protected override void Init()
	{
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config
		{
			ActionReversed = new HashSet<uint> { 94u, 29494u, 24402u }
		};
		OmenServiceBase<UseActionManager>.Instance().RegPreUseActionLocation(OnPreUseAction);
	}

	protected override void Uninit()
	{
		OmenServiceBase<UseActionManager>.Instance().Unreg(OnPreUseAction);
	}

	protected override void ConfigUI()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		TableDisposable val = ImRaii.Table(ImU8String.op_Implicit("ActionEnabled"), 3, (ImGuiTableFlags)128, VectorExtensions.WithY(ImGui.GetContentRegionAvail() / 1.75f, 0f));
		try
		{
			if (!val)
			{
				return;
			}
			ImGui.TableSetupColumn(ImU8String.op_Implicit("选框"), (ImGuiTableColumnFlags)8, ImGui.GetTextLineHeightWithSpacing(), 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("技能"), (ImGuiTableColumnFlags)4, 0f, 0u);
			ImU8String val2 = ImU8String.op_Implicit("反转");
			float textLineHeightWithSpacing = ImGui.GetTextLineHeightWithSpacing();
			ImGuiStylePtr style = ImGui.GetStyle();
			ImGui.TableSetupColumn(val2, (ImGuiTableColumnFlags)8, textLineHeightWithSpacing + ((ImGuiStylePtr)(ref style)).ItemSpacing.X, 0u);
			ImGui.TableNextRow((ImGuiTableRowFlags)1);
			ImGui.TableNextColumn();
			ImGui.TableNextColumn();
			ImGuiOm.Text(LanguageManager.Get("Action"));
			ImGui.TableNextColumn();
			ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 4f * Globals.GlobalUIScale);
			ImGuiOm.Text(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61666));
			ImGuiOm.TooltipHover(LanguageManager.Get("AutoActionAlignCamera-ReverseDirection"));
			foreach (KeyValuePair<uint, bool> item2 in config.ActionEnabled)
			{
				if (!LuminaGetter.TryGetRow<Action>(item2.Key, out Action item))
				{
					continue;
				}
				ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
				GameIconLookup val3 = new GameIconLookup((uint)((Action)(ref item)).Icon, false, true, (ClientLanguage?)null);
				IDalamudTextureWrap wrapOrDefault = obj.GetFromGameIcon(ref val3).GetWrapOrDefault((IDalamudTextureWrap)null);
				if (wrapOrDefault == null)
				{
					continue;
				}
				ImU8String val4 = new ImU8String(0, 1);
				((ImU8String)(ref val4)).AppendFormatted<uint>(item2.Key);
				IdDisposable val5 = ImRaii.PushId(val4, true);
				try
				{
					ImGui.TableNextRow();
					ImGui.TableNextColumn();
					bool value = item2.Value;
					ImU8String val6 = new ImU8String(3, 1);
					((ImU8String)(ref val6)).AppendLiteral("###");
					((ImU8String)(ref val6)).AppendFormatted<uint>(item2.Key);
					if (ImGui.Checkbox(val6, ref value))
					{
						config.ActionEnabled[item2.Key] = value;
						config.Save(this);
					}
					ImGui.TableNextColumn();
					ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 4f * Globals.GlobalUIScale);
					ImGui.Image(wrapOrDefault.Handle, new Vector2(ImGui.GetTextLineHeight()));
					ImGui.SameLine();
					ImGui.SetCursorPosY(ImGui.GetCursorPosY() - 2f * Globals.GlobalUIScale);
					ImU8String val7 = new ImU8String(0, 1);
					((ImU8String)(ref val7)).AppendFormatted<string>(((object)((Action)(ref item)).Name/*cast due to constrained. prefix*/).ToString());
					ImGui.TextUnformatted(val7);
					ImGui.TableNextColumn();
					bool flag = config.ActionReversed.Contains(item2.Key);
					ImU8String val8 = new ImU8String(14, 1);
					((ImU8String)(ref val8)).AppendLiteral("###");
					((ImU8String)(ref val8)).AppendFormatted<uint>(item2.Key);
					((ImU8String)(ref val8)).AppendLiteral("-IsReversed");
					if (ImGui.Checkbox(val8, ref flag))
					{
						if (!config.ActionReversed.Remove(item2.Key))
						{
							config.ActionReversed.Add(item2.Key);
						}
						config.Save(this);
					}
				}
				finally
				{
					((IDisposable)val5)?.Dispose();
				}
			}
		}
		finally
		{
			((TableDisposable)(ref val)).Dispose();
		}
	}

	private unsafe void OnPreUseAction(ref bool isPrevented, ref ActionType type, ref uint actionID, ref ulong targetID, ref Vector3 location, ref uint extraParam, ref byte a7)
	{
		if ((int)type != 1)
		{
			return;
		}
		uint adjustedActionId = ((ActionManager)ActionManager.Instance()).GetAdjustedActionId(actionID);
		if (!config.ActionEnabled.TryGetValue(adjustedActionId, out var value) || !value)
		{
			return;
		}
		IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
		if (localPlayer != null)
		{
			float num = RotationHelper.CameraDirHToChara(((Camera)((CameraManager)CameraManager.Instance()).Camera).DirH);
			if (config.ActionReversed.Contains(adjustedActionId))
			{
				num = RotationHelper.CharaSymmetricTransform(num);
			}
			if (GameState.ContentFinderCondition != 0)
			{
				PositionUpdateInstancePacket.MoveType moveType = (PositionUpdateInstancePacket.MoveType)(OmenServiceBase<MovementManager>.Instance().CurrentZoneMoveState << 16);
				new PositionUpdateInstancePacket(num, localPlayer.Position, moveType).Send();
			}
			else
			{
				PositionUpdatePacket.MoveType moveType2 = (PositionUpdatePacket.MoveType)(OmenServiceBase<MovementManager>.Instance().CurrentZoneMoveState << 16);
				new PositionUpdatePacket(num, localPlayer.Position, moveType2).Send();
			}
			((BattleChara)localPlayer.ToStruct()).SetRotation(num);
		}
	}
}
