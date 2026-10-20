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
using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.Sheets;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules;

public class AutoRedirectDashActions : ModuleBase
{
	private class MDb11gl7MoAX6nXpelu : ModuleConfig
	{
		public Dictionary<uint, bool> ActionsEnabled;

		public MDb11gl7MoAX6nXpelu()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			ActionsEnabled = new Dictionary<uint, bool>
			{
				[7419u] = true,
				[24403u] = true,
				[29551u] = true,
				[41507u] = true
			};
			base._002Ector();
		}
	}

	private MDb11gl7MoAX6nXpelu config;

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected override void Init()
	{
		config = ModuleConfigExtension.Load<MDb11gl7MoAX6nXpelu>(this) ?? new MDb11gl7MoAX6nXpelu();
		OmenServiceBase<UseActionManager>.Instance().RegPreUseActionLocation(OnPreUseAction);
	}

	protected override void Uninit()
	{
		OmenServiceBase<UseActionManager>.Instance().Unreg(OnPreUseAction);
	}

	protected override void ConfigUI()
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		Vector2 vector = ImGui.GetContentRegionAvail() / 2f;
		vector.Y = 0f;
		Vector2 vector2 = vector;
		TableDisposable val = ImRaii.Table(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x109C77DA ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_44b9808560124617907f0048883b8948)), 2, (ImGuiTableFlags)128, vector2);
		try
		{
			if (!val)
			{
				return;
			}
			ImGui.TableSetupColumn(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x23105FA9 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a96fab9e97944b39a8afe3fb1109ba7d)), (ImGuiTableColumnFlags)8, ImGui.GetTextLineHeightWithSpacing(), 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x18CCA6EE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_33babcee9ecc460a9e530039fd18d361)), (ImGuiTableColumnFlags)4, 0f, 0u);
			ImGui.TableNextRow((ImGuiTableRowFlags)1);
			ImGui.TableNextColumn();
			ImGui.TableNextColumn();
			ImGuiOm.Text(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5E3BD156 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2d7d8a5df6744c51b35f486f03b53ded)));
			foreach (KeyValuePair<uint, bool> item2 in config.ActionsEnabled)
			{
				if (!LuminaGetter.TryGetRow<Action>(item2.Key, out Action item))
				{
					continue;
				}
				ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
				GameIconLookup val2 = new GameIconLookup((uint)((Action)(ref item)).Icon, false, true, (ClientLanguage?)null);
				IDalamudTextureWrap wrapOrDefault = obj.GetFromGameIcon(ref val2).GetWrapOrDefault((IDalamudTextureWrap)null);
				if (wrapOrDefault == null)
				{
					continue;
				}
				ImU8String val3 = new ImU8String(0, 1);
				((ImU8String)(ref val3)).AppendFormatted<uint>(item2.Key);
				IdDisposable val4 = ImRaii.PushId(val3, true);
				try
				{
					ImGui.TableNextRow();
					ImGui.TableNextColumn();
					bool value = item2.Value;
					ImU8String val5 = new ImU8String(3, 1);
					((ImU8String)(ref val5)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5357DAD8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_0676edd1b45a4ff096830fd6229bc1fb));
					((ImU8String)(ref val5)).AppendFormatted<uint>(item2.Key);
					if (ImGui.Checkbox(val5, ref value))
					{
						config.ActionsEnabled[item2.Key] = value;
						config.Save(this);
					}
					ImGui.TableNextColumn();
					ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 4f * Globals.GlobalUIScale);
					ImGui.Image(wrapOrDefault.Handle, new Vector2(ImGui.GetTextLineHeight()));
					ImGui.SameLine();
					ImGui.SetCursorPosY(ImGui.GetCursorPosY() - 2f * Globals.GlobalUIScale);
					ImU8String val6 = new ImU8String(0, 1);
					((ImU8String)(ref val6)).AppendFormatted<string>(((object)((Action)(ref item)).Name/*cast due to constrained. prefix*/).ToString());
					ImGui.TextUnformatted(val6);
				}
				finally
				{
					((IDisposable)val4)?.Dispose();
				}
			}
		}
		finally
		{
			((TableDisposable)(ref val)).Dispose();
		}
	}

	public unsafe void OnPreUseAction(ref bool isPrevented, ref ActionType actionType, ref uint actionID, ref ulong targetID, ref Vector3 location, ref uint extraParam, ref byte a7)
	{
		if ((int)actionType != 1)
		{
			return;
		}
		uint adjustedActionId = ((ActionManager)ActionManager.Instance()).GetAdjustedActionId(actionID);
		if (config.ActionsEnabled.TryGetValue(adjustedActionId, out var value) && value)
		{
			IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
			Vector3 target = default(Vector3);
			if (localPlayer != null && LuminaGetter.TryGetRow<Action>(adjustedActionId, out Action item) && ((Action)(ref item)).TargetArea && ((ActionManager)ActionManager.Instance()).GetActionStatus(actionType, adjustedActionId, 3758096384uL, true, true, (uint*)null) == 0 && DalamudServiceExtension.Instance<IGameGui>().ScreenToWorld(ImGui.GetMousePos(), ref target, 100000f))
			{
				target = AdjustTargetPosition(localPlayer.Position, target, ((Action)(ref item)).Range);
				location = target;
			}
		}
	}

	public static Vector3 AdjustTargetPosition(Vector3 origin, Vector3 target, float maxDistance)
	{
		Vector2 vector = origin.ToVector2();
		Vector2 vector2 = target.ToVector2();
		if (Vector2.DistanceSquared(vector, vector2) > maxDistance * maxDistance)
		{
			Vector2 vector3 = Vector2.Normalize(vector2 - vector);
			vector2 = vector + vector3 * maxDistance;
			return new Vector3(vector2.X, target.Y, vector2.Y);
		}
		return target;
	}

	public AutoRedirectDashActions()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x76EB991D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b988ddaee02846b69dd4e446b5da9de9)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6AAE2734 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_839f542231154d29b0562b687afa328a)),
			Category = ModuleCategory.Action
		};
		Permission = new ModulePermission
		{
			NeedAuth = true,
			CNPremium = true,
			TCPremium = true
		};
		base._002Ector();
	}
}
