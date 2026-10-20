using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using B2nED1Wmdu0xsDR7Tjv;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.InstanceContent;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Common.Math;
using FFXIVClientStructs.Interop;
using FFXIVClientStructs.STD;
using FFXIVClientStructs.STD.Helper;
using GO3ln7f4pExIcYCK2q3;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.ImGuiOm;
using OmenTools.Info.Game.Packets.Upstream;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using P67IJbfls2Hy3Xtb45R;
using itAnDoW4mgYVlrmtMor;

namespace j6WMLU97bhEtYpJPds3;

internal sealed class O9jj2h9N8tqnIkxI6Cq : ExINkNfv21TTK3yJ2Cx
{
	private readonly struct nJIlYmKAed9DfsXqOAq
	{
		public nint Address { get; }

		public unsafe EventHandler* Handler { get; }

		public string Label { get; }

		public int ObjectCount { get; }

		public uint EventID { get; }

		public string ContentType { get; }

		public unsafe nJIlYmKAed9DfsXqOAq(nint address, EventHandler* handler, string label, int objectCount, uint eventID, string contentType)
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			Address = address;
			Handler = handler;
			Label = label;
			ObjectCount = objectCount;
			EventID = eventID;
			ContentType = contentType;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass7_0
	{
		public VejoYhfM1yiPckJhcdm context;

		public _003C_003Ec__DisplayClass7_0()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
		}

		internal bool J3SKQLgcdq(nJIlYmKAed9DfsXqOAq handler)
		{
			return handler.Address == context.ViewState.SelectedInteractHandlerAddress;
		}
	}

	public override string SectionID => GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4CDA08FD ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_2765950fccc24d96a1162dd6779cbee1);

	public override string Title => GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x702B1621 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ed1539c28915419fb405793dc0185c30);

	protected override void DrawContent(VejoYhfM1yiPckJhcdm context)
	{
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		List<nJIlYmKAed9DfsXqOAq> list = GB59uTEVjy();
		if (list.Count == 0)
		{
			ImGui.TextDisabled(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3D7DDA5E ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_cda76bbf50614e97b4ef38939106c571)));
			return;
		}
		nJIlYmKAed9DfsXqOAq handler = r4o9URR3oR(context, list);
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(7, 1);
		((ImU8String)(ref val)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5B21203C ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_435f54c92c5b40a9ad2757dd36deed0a));
		((ImU8String)(ref val)).AppendFormatted<int>(list.Count);
		ImGui.TextDisabled(val);
		ImGui.Spacing();
		TableDisposable val2 = ImRaii.Table(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2FDC5A21 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c6c2389b72fc4ab3828ae71fae281bf1)), 2, (ImGuiTableFlags)25089);
		try
		{
			if (!(!val2))
			{
				ImGui.TableSetupColumn(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2FDC5A1B ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c6c2389b72fc4ab3828ae71fae281bf1)), (ImGuiTableColumnFlags)4, 10f, 0u);
				ImGui.TableSetupColumn(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x75EEEDDF ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_11422b9d07f9498fb995b7dd8e417b5b)), (ImGuiTableColumnFlags)4, 50f, 0u);
				ImGui.TableNextRow();
				ImGui.TableNextColumn();
				Ser9dr2ZS8(context, list);
				ImGui.TableNextColumn();
				VUK9Ix35et(handler);
			}
		}
		finally
		{
			((TableDisposable)(ref val2)).Dispose();
		}
	}

	private static void Ser9dr2ZS8(VejoYhfM1yiPckJhcdm context, IReadOnlyList<nJIlYmKAed9DfsXqOAq> handlers)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		ChildDisposable val = ImRaii.Child(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x75EEEDC9 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_11422b9d07f9498fb995b7dd8e417b5b)), ImGui.GetContentRegionAvail(), true);
		try
		{
			if (!val)
			{
				return;
			}
			foreach (nJIlYmKAed9DfsXqOAq handler in handlers)
			{
				bool flag = context.ViewState.SelectedInteractHandlerAddress == handler.Address;
				ImU8String val2 = new ImU8String(2, 2);
				((ImU8String)(ref val2)).AppendFormatted<string>(handler.Label);
				((ImU8String)(ref val2)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6B30BE6C ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ac6250342b784829976646d28955b117));
				((ImU8String)(ref val2)).AppendFormatted<nint>(handler.Address);
				if (ImGui.Selectable(val2, flag, (ImGuiSelectableFlags)0, default(Vector2)))
				{
					context.ViewState.SelectedInteractHandlerAddress = handler.Address;
				}
				if (ImGui.IsItemHovered())
				{
					ImU8String tooltip = new ImU8String(8, 1);
					((ImU8String)(ref tooltip)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x34A20368 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5e416caedcb6438f915024f390fd9079));
					((ImU8String)(ref tooltip)).AppendFormatted<int>(handler.ObjectCount);
					ImGui.SetTooltip(tooltip);
				}
			}
		}
		finally
		{
			((ChildDisposable)(ref val)).Dispose();
		}
	}

	private unsafe static void VUK9Ix35et(nJIlYmKAed9DfsXqOAq handler)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_049d: Unknown result type (might be due to invalid IL or missing references)
		using (OmenServiceBase<FontManager>.Instance().UIFont120.Push())
		{
			ImGui.TextUnformatted(ImU8String.op_Implicit(handler.Label));
		}
		ImGuiStylePtr style = ImGui.GetStyle();
		ColorDisposable val = ImRaii.PushColor((ImGuiCol)0, ((ImGuiStylePtr)(ref style)).Colors[1], true);
		try
		{
			ImU8String val2 = new ImU8String(26, 3);
			((ImU8String)(ref val2)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x90696F8 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_444e42c3749640698bcaba67a1c5b5f3));
			((ImU8String)(ref val2)).AppendFormatted<string>(handler.ContentType);
			((ImU8String)(ref val2)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x53686DC2 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d4ce857df7b047629e7091bbc815dd94));
			((ImU8String)(ref val2)).AppendFormatted<uint>(handler.EventID);
			((ImU8String)(ref val2)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x34A2032C ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5e416caedcb6438f915024f390fd9079));
			((ImU8String)(ref val2)).AppendFormatted<int>(handler.ObjectCount);
			ImGui.TextWrapped(val2);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		ImGui.Spacing();
		ImGui.Separator();
		ImGui.Spacing();
		TableDisposable val3 = ImRaii.Table(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x90696B8 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_444e42c3749640698bcaba67a1c5b5f3)), 4, (ImGuiTableFlags)24769);
		try
		{
			if (!val3)
			{
				return;
			}
			ImGui.TableSetupColumn(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x51331AA6 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_fc39b8b86ffd43198ab35059b444b39b)), (ImGuiTableColumnFlags)4, 0f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x35B4B85C ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_042249f573d4431982cd93672ca35bd3)), (ImGuiTableColumnFlags)4, 0f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(-1003249140 ^ -954612951 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_14783916c53948389577bd0e927537b1)), (ImGuiTableColumnFlags)4, 0f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x32A611F ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_14783916c53948389577bd0e927537b1)), (ImGuiTableColumnFlags)4, 0f, 0u);
			ImGui.TableHeadersRow();
			Enumerator<Pointer<GameObject>, Pointer<GameObject>, DefaultKeyExtractor<Pointer<GameObject>>> enumerator = ((EventHandler)handler.Handler).EventObjects.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Pointer<GameObject> current = enumerator.Current;
					if (current.Value == null)
					{
						continue;
					}
					ImGui.TableNextRow();
					ImGui.TableNextColumn();
					ImU8String val4 = new ImU8String(0, 1);
					((ImU8String)(ref val4)).AppendFormatted<string>(((GameObject)current.Value).NameString);
					ImGui.TextUnformatted(val4);
					ImGui.TableNextColumn();
					ImU8String val5 = new ImU8String(0, 1);
					((ImU8String)(ref val5)).AppendFormatted<ObjectKind>(((GameObject)current.Value).ObjectKind);
					ImGui.TextUnformatted(val5);
					ImGui.TableNextColumn();
					ImU8String val6 = new ImU8String(0, 1);
					((ImU8String)(ref val6)).AppendFormatted<ulong>(GameObjectId.op_Implicit(((GameObject)current.Value).GetGameObjectId()));
					ImGui.TextUnformatted(val6);
					ImGui.TableNextColumn();
					ImU8String val7 = new ImU8String(1, 2);
					((ImU8String)(ref val7)).AppendFormatted<nint>(handler.Address);
					((ImU8String)(ref val7)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x38919346 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_59cab3a5f3464d2f8ff563ddb3d8bad1));
					((ImU8String)(ref val7)).AppendFormatted<nint>((nint)current.Value);
					IdDisposable val8 = ImRaii.PushId(val7, true);
					try
					{
						if (ImGuiOm.ButtonIcon(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4CDA0B09 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_2765950fccc24d96a1162dd6779cbee1), (FontAwesomeIcon)62021, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x35A9F389 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_95b251cdaa8148b7af54d7956ad9ed2a)))
						{
							new EventStartPackt(LocalPlayerState.EntityID, EventId.op_Implicit(((EventHandlerInfo)(&((EventHandler)handler.Handler).Info)).EventId)).Send();
						}
						ImGui.SameLine();
						if (ImGuiOm.ButtonIcon(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x46294399 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_93e1cfc241b644019a4819c49327d6a7), (FontAwesomeIcon)63692, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x61166E36 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_8883645b187042ce8990c01069a51402)))
						{
							new EventStartPackt(GameObjectId.op_Implicit(((GameObject)current.Value).GetGameObjectId()), EventId.op_Implicit(((EventHandlerInfo)(&((EventHandler)handler.Handler).Info)).EventId)).Send();
						}
						ImGui.SameLine();
						if (ImGuiOm.ButtonIcon(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6CA7828F ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_86e8e55020514087b1fe097f879d6dff), (FontAwesomeIcon)61505, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2F17193 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d964b0b1313049f490f2107d03a6ed3a)))
						{
							AgentMap* intPtr = AgentMap.Instance();
							((AgentMap)intPtr).SetFlagMapMarker(GameState.TerritoryType, DalamudServiceExtension.Instance<IClientState>().MapId, Vector3.op_Implicit(((GameObject)current.Value).Position), 60561u);
							((AgentMap)intPtr).OpenMap(DalamudServiceExtension.Instance<IClientState>().MapId, GameState.TerritoryType, ((GameObject)current.Value).NameString ?? "", (MapType)1);
						}
					}
					finally
					{
						((IDisposable)val8)?.Dispose();
					}
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
			}
		}
		finally
		{
			((TableDisposable)(ref val3)).Dispose();
		}
	}

	private static nJIlYmKAed9DfsXqOAq r4o9URR3oR(VejoYhfM1yiPckJhcdm context, IReadOnlyList<nJIlYmKAed9DfsXqOAq> handlers)
	{
		_003C_003Ec__DisplayClass7_0 CS_0024_003C_003E8__locals3 = new _003C_003Ec__DisplayClass7_0();
		CS_0024_003C_003E8__locals3.context = context;
		nJIlYmKAed9DfsXqOAq result = handlers.FirstOrDefault((nJIlYmKAed9DfsXqOAq handler) => handler.Address == CS_0024_003C_003E8__locals3.context.ViewState.SelectedInteractHandlerAddress);
		if (result.Address != 0)
		{
			return result;
		}
		result = handlers[0];
		CS_0024_003C_003E8__locals3.context.ViewState.SelectedInteractHandlerAddress = result.Address;
		return result;
	}

	private unsafe static List<nJIlYmKAed9DfsXqOAq> GB59uTEVjy()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		List<nJIlYmKAed9DfsXqOAq> list = new List<nJIlYmKAed9DfsXqOAq>();
		Enumerator<StdPair<uint, Pointer<EventHandler>>, uint, PairKeyExtractor<uint, Pointer<EventHandler>>> enumerator = ((EventHandlerModule)(&((EventFramework)EventFramework.Instance()).EventHandlerModule)).EventHandlerMap.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				StdPair<uint, Pointer<EventHandler>> current = enumerator.Current;
				EventHandler* value = current.Item2.Value;
				if (value != null)
				{
					int num = ((IEnumerable<Pointer<GameObject>>)(object)((EventHandler)value).EventObjects).Count((Pointer<GameObject> obj) => obj.Value != null);
					if (num > 0)
					{
						string label = pGQ9XJG2Xc(value);
						list.Add(new nJIlYmKAed9DfsXqOAq((nint)value, value, label, num, ((EventId)(&((EventHandlerInfo)(&((EventHandler)value).Info)).EventId)).Id, ((object)(*(EventHandlerContent*)(&((EventId)(&((EventHandlerInfo)(&((EventHandler)value).Info)).EventId)).ContentId))/*cast due to constrained. prefix*/).ToString()));
					}
				}
			}
			return list;
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
	}

	private unsafe static string pGQ9XJG2Xc(EventHandler* handler)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Invalid comparison between Unknown and I4
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Invalid comparison between Unknown and I4
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Invalid comparison between Unknown and I4
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Invalid comparison between Unknown and I4
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Invalid comparison between Unknown and I4
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Invalid comparison between Unknown and I4
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		EventId eventId = ((EventHandlerInfo)(&((EventHandler)handler).Info)).EventId;
		string text = null;
		EventHandlerContent contentId = eventId.ContentId;
		if ((int)contentId <= 4)
		{
			if ((int)contentId != 1)
			{
				if ((int)contentId == 4)
				{
					text = ((object)new ReadOnlySeStringSpan(((Utf8String)(&((ShopEventHandler)handler).ShopName)).AsSpan())/*cast due to constrained. prefix*/).ToString();
				}
			}
			else
			{
				Quest? row = LuminaGetter.GetRow<Quest>(eventId.Id);
				object obj;
				if (!row.HasValue)
				{
					obj = null;
				}
				else
				{
					Quest valueOrDefault = row.GetValueOrDefault();
					obj = ((object)((Quest)(ref valueOrDefault)).Name/*cast due to constrained. prefix*/).ToString();
				}
				text = (string)obj;
			}
		}
		else if ((int)contentId != 11)
		{
			if ((int)contentId != 32771)
			{
				if ((int)contentId == 32772)
				{
					text = ((object)(*(PublicContentDirectorType*)(&((PublicContentDirector)handler).Type))/*cast due to constrained. prefix*/).ToString();
				}
			}
			else
			{
				text = ((object)(*(InstanceContentType*)(&((InstanceContentDirector)handler).InstanceContentType))/*cast due to constrained. prefix*/).ToString();
			}
		}
		else
		{
			text = ((object)new ReadOnlySeStringSpan(((Utf8String)(&((LuaEventHandler)handler).LuaClass)).AsSpan())/*cast due to constrained. prefix*/).ToString();
		}
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 2);
		defaultInterpolatedStringHandler.AppendFormatted(eventId.Id);
		defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x702B159D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ed1539c28915419fb405793dc0185c30));
		defaultInterpolatedStringHandler.AppendFormatted<EventHandlerContent>(eventId.ContentId);
		defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x53686D18 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d4ce857df7b047629e7091bbc815dd94));
		return defaultInterpolatedStringHandler.ToStringAndClear() + (string.IsNullOrWhiteSpace(text) ? string.Empty : (GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1E24CBA9 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_86afda88005749b094dac2a7d6b27c25) + text + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4732E8B7 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5f88650ee89f435aad988a8583bda5b5)));
	}

	public O9jj2h9N8tqnIkxI6Cq()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
	}
}
