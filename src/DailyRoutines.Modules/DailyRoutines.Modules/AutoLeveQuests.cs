using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Interface;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Application.Network.WorkDefinitions;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Info.Game.Data;
using OmenTools.Info.Game.Packets.Upstream;
using OmenTools.Info.Lumina.ExtraSheets;
using OmenTools.Interop.Game.AddonEvent;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules;

public class AutoLeveQuests : ModuleBase
{
	private class JISEppx0qoRRIL4rTwU : ModuleConfig
	{
		public uint Leve;

		public (ulong GameObjectID, string Name) LeveMete;

		public (ulong GameObjectID, string Name) LeveReceiver;

		public JISEppx0qoRRIL4rTwU()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			LeveMete = (GameObjectID: 0uL, Name: string.Empty);
			LeveReceiver = (GameObjectID: 0uL, Name: string.Empty);
			base._002Ector();
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass15_0
	{
		public CraftLeve craftLeveData;

		public _003C_003Ec__DisplayClass15_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal global::_003C_003Ef__AnonymousType2<RowRef<Item>, ushort> MyNx5INVH3(int x)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			return new
			{
				Obj = ((CraftLeve)(ref craftLeveData)).Item[x],
				ItemCountNeeded = ((CraftLeve)(ref craftLeveData)).ItemCount[x]
			};
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass18_0
	{
		public uint placeNameID;

		public _003C_003Ec__DisplayClass18_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal bool C6NxRQrjn4(Leve x)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			bool flag = ((Leve)(ref x)).PlaceNameIssued.RowId == placeNameID && !string.IsNullOrEmpty(((object)((Leve)(ref x)).Name/*cast due to constrained. prefix*/).ToString());
			if (flag)
			{
				bool flag2;
				switch (((Leve)(ref x)).ClassJobCategory.RowId)
				{
				case 9u:
				case 10u:
				case 11u:
				case 12u:
				case 13u:
				case 14u:
				case 15u:
				case 16u:
				case 19u:
					flag2 = true;
					break;
				default:
					flag2 = false;
					break;
				}
				flag = flag2;
			}
			return flag;
		}
	}

	private unsafe byte* leveAllowancesPtr;

	private JISEppx0qoRRIL4rTwU config;

	private Dictionary<string, List<Leve>> leveQuestsCurrentZone;

	private bool isItemTooltip;

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected unsafe override void Init()
	{
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Expected O, but got Unknown
		leveAllowancesPtr = new CompSig(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7C2F4E88 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_12c2d68ba4654622a4959ee73cacc6d7)).GetStatic<byte>();
		if (base.TaskHelper == null)
		{
			TaskHelper taskHelper = (base.TaskHelper = new TaskHelper());
		}
		config = ModuleConfigExtension.Load<JISEppx0qoRRIL4rTwU>(this) ?? new JISEppx0qoRRIL4rTwU();
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)1, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x1778913D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f7cf93b006904393bb56d82c0a688fa1), new AddonEventDelegate(zuAMosAWpH));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)5, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7FD8B721 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1eab987c1db64b928220e27bd03141d3), new AddonEventDelegate(SiBMgkZhOY));
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += kpiMhfFbAC;
		kpiMhfFbAC(0u);
	}

	protected unsafe override void Uninit()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= kpiMhfFbAC;
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { SiBMgkZhOY });
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { zuAMosAWpH });
		leveQuestsCurrentZone.Clear();
		if (isItemTooltip)
		{
			isItemTooltip = false;
			(*AtkStage.Instance()).HideTooltip(((AtkUnitBase)Addons.ScreenText).Id);
		}
	}

	protected unsafe override void ConfigUI()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0493: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_0503: Unknown result type (might be due to invalid IL or missing references)
		//IL_0508: Unknown result type (might be due to invalid IL or missing references)
		//IL_0530: Unknown result type (might be due to invalid IL or missing references)
		//IL_0535: Unknown result type (might be due to invalid IL or missing references)
		//IL_0766: Unknown result type (might be due to invalid IL or missing references)
		//IL_0778: Unknown result type (might be due to invalid IL or missing references)
		//IL_077d: Unknown result type (might be due to invalid IL or missing references)
		//IL_078d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0562: Unknown result type (might be due to invalid IL or missing references)
		//IL_0567: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0570: Unknown result type (might be due to invalid IL or missing references)
		//IL_058b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0590: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_061a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0688: Unknown result type (might be due to invalid IL or missing references)
		//IL_068d: Unknown result type (might be due to invalid IL or missing references)
		if (config.Leve == 0 || !LuminaGetter.TryGetRow<Leve>(config.Leve, out Leve item))
		{
			return;
		}
		RowRef dataId = ((Leve)(ref item)).DataId;
		if (!LuminaGetter.TryGetRow<CraftLeve>(((RowRef)(ref dataId)).RowId, out CraftLeve item2))
		{
			return;
		}
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(0, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LuminaWrapper.GetAddonText(574u));
		ImGui.TextColored(ref vector, val);
		IndentDisposable val2 = ImRaii.PushIndent(1, true);
		try
		{
			float x = ImGui.GetContentRegionAvail().X;
			ImGuiStylePtr style = ImGui.GetStyle();
			ImGui.SetNextItemWidth(x - 10f * ((ImGuiStylePtr)(ref style)).ItemSpacing.X);
			ImU8String val3 = ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5A7FE30B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5154ccd9fed0487f8fa0244531466009));
			ImU8String val4 = new ImU8String(8, 3);
			((ImU8String)(ref val4)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4B0FEF00 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_8e002b0644ef421698ef6f63772929af));
			ClassJobCategory value = ((Leve)(ref item)).ClassJobCategory.Value;
			((ImU8String)(ref val4)).AppendFormatted<string>(((object)((ClassJobCategory)(ref value)).Name/*cast due to constrained. prefix*/).ToString());
			((ImU8String)(ref val4)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x64E5290E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f53034f3efc64a309b10c7885bf5d9bd));
			((ImU8String)(ref val4)).AppendFormatted<ReadOnlySeString>(((Leve)(ref item)).Name);
			((ImU8String)(ref val4)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x469A1E85 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_bb1eb496bad948e4a2baaa3e1edc7a92));
			((ImU8String)(ref val4)).AppendFormatted<ushort>(((Leve)(ref item)).ClassJobLevel);
			((ImU8String)(ref val4)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7EE7AE3B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88432e96079448078bdf00e7b62f8ab6));
			ComboDisposable val5 = ImRaii.Combo(val3, val4, (ImGuiComboFlags)16);
			try
			{
				if (ComboDisposable.op_Implicit(val5))
				{
					ISharedImmediateTexture val8 = default(ISharedImmediateTexture);
					foreach (KeyValuePair<string, List<Leve>> item3 in leveQuestsCurrentZone)
					{
						TreeNodeDisposable val6 = ImRaii.TreeNode(ImU8String.op_Implicit(item3.Key));
						try
						{
							if (!val6)
							{
								continue;
							}
							foreach (Leve item4 in item3.Value)
							{
								Leve current2 = item4;
								string text = ((object)((Leve)(ref current2)).Name/*cast due to constrained. prefix*/).ToString();
								value = ((Leve)(ref current2)).ClassJobCategory.Value;
								string value2 = ((object)((ClassJobCategory)(ref value)).Name/*cast due to constrained. prefix*/).ToString();
								string text2 = text;
								if (text.Contains('：') || text.Contains(':'))
								{
									text2 = text.Substring(text.IndexOf('：') + 1);
								}
								text2 = text2.Trim();
								uint num = 62100 + ((((Leve)(ref current2)).ClassJobCategory.RowId == 0) ? 44 : (((Leve)(ref current2)).ClassJobCategory.RowId - 1));
								ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
								GameIconLookup val7 = new GameIconLookup(num, false, true, (ClientLanguage?)null);
								if (obj.TryGetFromGameIcon(ref val7, ref val8))
								{
									ImTextureID handle = val8.GetWrapOrEmpty().Handle;
									Vector2 imageSize = new Vector2(ImGui.GetTextLineHeightWithSpacing());
									DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(7, 3);
									defaultInterpolatedStringHandler.AppendFormatted(value2);
									defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xF207FD3 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_fea4ad1b13de4c4bbc22f571ceda7372));
									defaultInterpolatedStringHandler.AppendFormatted(text2);
									defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-1487420814 ^ -146758332 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e227921f7ee94b6aa1c28169395ba0cf));
									defaultInterpolatedStringHandler.AppendFormatted(((Leve)(ref current2)).ClassJobLevel);
									defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5DD70241 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c7355392ad9e43b6b82798cc42ea6c6f));
									if (ImGuiOm.SelectableImageWithText(handle, imageSize, defaultInterpolatedStringHandler.ToStringAndClear(), config.Leve == ((Leve)(ref current2)).RowId, (ImGuiSelectableFlags)0))
									{
										config.Leve = ((Leve)(ref current2)).RowId;
										config.Save(this);
									}
								}
							}
						}
						finally
						{
							((TreeNodeDisposable)(ref val6)).Dispose();
						}
					}
				}
			}
			finally
			{
				((ComboDisposable)(ref val5)).Dispose();
			}
			if (ImGui.IsItemClicked())
			{
				kpiMhfFbAC((ushort)GameState.TerritoryType);
			}
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
		vector = KnownColor.Chocolate.ToVector4();
		ImU8String val9 = default(ImU8String);
		((ImU8String)(ref val9))._002Ector(0, 1);
		((ImU8String)(ref val9)).AppendFormatted<string>(LuminaWrapper.GetAddonText(12563u));
		ImGui.TextColored(ref vector, val9);
		val2 = ImRaii.PushIndent(1, true);
		try
		{
			GroupDisposable val10 = ImRaii.Group();
			try
			{
				ImGui.Spacing();
				bool flag = false;
				uint itemID = 0u;
				ISharedImmediateTexture val12 = default(ISharedImmediateTexture);
				for (int i = 0; i < 4; i++)
				{
					RowRef<Item> val11 = ((CraftLeve)(ref item2)).Item[i];
					ushort num2 = ((CraftLeve)(ref item2)).ItemCount[i];
					if (!val11.ValueNullable.HasValue || num2 == 0)
					{
						continue;
					}
					Item value3 = val11.Value;
					int num3 = iauMmVvw0M(val11.RowId);
					ITextureProvider obj2 = DalamudServiceExtension.Instance<ITextureProvider>();
					GameIconLookup val7 = new GameIconLookup((uint)((Item)(ref value3)).Icon, false, true, (ClientLanguage?)null);
					if (!obj2.TryGetFromGameIcon(ref val7, ref val12))
					{
						continue;
					}
					ImGui.SameLine();
					ColorDisposable val13 = ImRaii.PushColor((ImGuiCol)0, KnownColor.Red.ToVector4(), num3 < num2);
					try
					{
						GroupDisposable val14 = ImRaii.Group();
						try
						{
							ImGui.Image(val12.GetWrapOrEmpty().Handle, new Vector2(ImGui.GetTextLineHeight()));
							ImGui.SameLine();
							string? text3 = ((object)((Item)(ref value3)).Name/*cast due to constrained. prefix*/).ToString();
							string text4;
							if (num2 <= 1)
							{
								text4 = string.Empty;
							}
							else
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(4, 1);
								defaultInterpolatedStringHandler2.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-1003249140 ^ -2015056302 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_974d377e1ed74986b81c34c95c2cc846));
								defaultInterpolatedStringHandler2.AppendFormatted(num2);
								defaultInterpolatedStringHandler2.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6AAE2874 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_839f542231154d29b0562b687afa328a));
								text4 = defaultInterpolatedStringHandler2.ToStringAndClear();
							}
							ImGui.TextUnformatted(ImU8String.op_Implicit(text3 + text4));
						}
						finally
						{
							((GroupDisposable)(ref val14)).Dispose();
						}
					}
					finally
					{
						((IDisposable)val13)?.Dispose();
					}
					if (ImGui.IsItemHovered())
					{
						ImGui.SetMouseCursor((ImGuiMouseCursor)7);
						flag = true;
						itemID = ((Item)(ref value3)).RowId;
					}
					if (ImGui.IsItemClicked())
					{
						OmenServiceBase<ChatManager>.Instance().SendMessage(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x12D66598 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7fcf5a71a7ed445197f132979519c560) + ((object)((Item)(ref value3)).Name/*cast due to constrained. prefix*/).ToString() + LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x9B4994A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2f1c0cb46b4440d499a69ed8afe22360));
					}
				}
				if (flag)
				{
					(*AtkStage.Instance()).ShowItemTooltip(((AtkUnitBase)Addons.ScreenText).RootNode, itemID);
					isItemTooltip = true;
				}
				else if (isItemTooltip)
				{
					isItemTooltip = false;
					(*AtkStage.Instance()).HideTooltip(((AtkUnitBase)Addons.ScreenText).Id);
				}
			}
			finally
			{
				((GroupDisposable)(ref val10)).Dispose();
			}
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
		vector = KnownColor.Chocolate.ToVector4();
		ImU8String val15 = default(ImU8String);
		((ImU8String)(ref val15))._002Ector(0, 1);
		((ImU8String)(ref val15)).AppendFormatted<string>(LuminaWrapper.GetAddonText(456u));
		ImGui.TextColored(ref vector, val15);
		val2 = ImRaii.PushIndent(1, true);
		try
		{
			ImU8String val16 = new ImU8String(0, 1);
			((ImU8String)(ref val16)).AppendFormatted<byte>(*leveAllowancesPtr);
			ImGui.TextUnformatted(val16);
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
		ImGui.NewLine();
		GroupDisposable val17 = ImRaii.Group();
		try
		{
			WPeMv5vZ8C();
		}
		finally
		{
			((GroupDisposable)(ref val17)).Dispose();
		}
		Vector2 itemRectSize = ImGui.GetItemRectSize();
		ImGui.SameLine(0f, 20f * Globals.GlobalUIScale);
		GroupDisposable val18 = ImRaii.Group();
		try
		{
			DisabledDisposable val19 = ImRaii.Disabled(config.LeveMete.GameObjectID == 0L || config.LeveReceiver.GameObjectID == 0L || config.LeveMete.GameObjectID == config.LeveReceiver.GameObjectID || config.Leve == 0 || base.TaskHelper.IsBusy);
			Vector2 buttonSize;
			try
			{
				string text5 = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x469A06C3 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_bb1eb496bad948e4a2baaa3e1edc7a92));
				buttonSize = itemRectSize;
				buttonSize.X = 100f * Globals.GlobalUIScale;
				if (ImGuiOm.ButtonIconWithTextVertical((FontAwesomeIcon)61515, text5, buttonSize))
				{
					weSMloVy4n();
				}
			}
			finally
			{
				((IDisposable)val19)?.Dispose();
			}
			ImGui.SameLine();
			string text6 = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--664844326 ^ 0x74F74952 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_0676edd1b45a4ff096830fd6229bc1fb));
			buttonSize = itemRectSize;
			buttonSize.X = 100f * Globals.GlobalUIScale;
			if (ImGuiOm.ButtonIconWithTextVertical((FontAwesomeIcon)61516, text6, buttonSize) && IOmenDalamudService<IObjectTable>.Instance().SearchByID(config.LeveReceiver.GameObjectID) != null && IOmenDalamudService<IObjectTable>.Instance().SearchByID(config.LeveMete.GameObjectID) != null)
			{
				base.TaskHelper.Abort();
			}
		}
		finally
		{
			((GroupDisposable)(ref val18)).Dispose();
		}
	}

	private void WPeMv5vZ8C()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		DisabledDisposable val = ImRaii.Disabled(base.TaskHelper.IsBusy);
		try
		{
			if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7C2F4FBE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_12c2d68ba4654622a4959ee73cacc6d7))), default(Vector2)))
			{
				IGameObject target = TargetManager.Target;
				if (target != null)
				{
					config.LeveMete = (GameObjectID: target.GameObjectID, Name: target.Name);
					config.Save(this);
				}
			}
			ImGui.SameLine();
			ImGui.TextUnformatted(ImU8String.op_Implicit((config.LeveMete.GameObjectID == 0L) ? LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6E009482 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c40146824d9043e0906f4e1889cf39aa)) : (config.LeveMete.Name ?? "")));
			ImGuiOm.ScaledDummy(2f);
			if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5357E854 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_0676edd1b45a4ff096830fd6229bc1fb))), default(Vector2)))
			{
				IGameObject target2 = TargetManager.Target;
				if (target2 != null)
				{
					config.LeveReceiver = (GameObjectID: target2.GameObjectID, Name: target2.Name);
					config.Save(this);
				}
			}
			ImGui.SameLine();
			ImGui.TextUnformatted(ImU8String.op_Implicit((config.LeveReceiver.GameObjectID == 0L) ? LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--1976305562 ^ 0x3D53C1C8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b2294f2f9bb24a76b00cf1af75888414)) : (config.LeveReceiver.Name ?? "")));
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private unsafe void weSMloVy4n()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		base.TaskHelper.Abort();
		bool flag = ((QuestManager)QuestManager.Instance()).LeveQuests.ToArray().FirstOrDefault((LeveWork x) => x.LeveId == config.Leve).LeveId != 0;
		if (flag && (*Addons.SelectString).IsAddonAndNodesReady())
		{
			string text = ((object)LuminaGetter.GetRowOrDefault<CraftLeveClient>(1u).Text/*cast due to constrained. prefix*/).ToString();
			if (AddonSelectStringEvent.TryScanSelectStringText(text, out var _))
			{
				AddonSelectStringEvent.Select(text);
			}
			base.TaskHelper.Enqueue((Action)weSMloVy4n, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x36952857 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_26e231ba68f74e8784503e099ddfc2d9), 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
		}
		else if (flag && !IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()))
		{
			base.TaskHelper.Enqueue((Func<bool>)hUOM3guLOx, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x109C44F4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_44b9808560124617907f0048883b8948), 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
			base.TaskHelper.Enqueue((Action)weSMloVy4n, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4427359E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8dfe57222f64fa68d9d16c8b6fe9d0b), 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
		}
		else if (!flag)
		{
			if (!QFTMxqMk1F())
			{
				base.TaskHelper.Abort();
				string message = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7C2F4CA4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_12c2d68ba4654622a4959ee73cacc6d7));
				OmenServiceBase<NotifyHelper>.Instance().NotificationInfo(message);
				NotifyHelper.Speak(message);
			}
			else
			{
				base.TaskHelper.Enqueue((Func<bool>)XCfMpNmfWZ, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x18CC967E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_33babcee9ecc460a9e530039fd18d361), 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
				base.TaskHelper.Enqueue((Action)weSMloVy4n, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xE09E1DD ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_aba6092d7cea44a49d1667540500e363), 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
			}
		}
		else
		{
			base.TaskHelper.Enqueue((Action)weSMloVy4n, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-1675397288 ^ -1956912053 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f7cf93b006904393bb56d82c0a688fa1), 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
		}
	}

	private unsafe bool QFTMxqMk1F()
	{
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		_003C_003Ec__DisplayClass15_0 CS_0024_003C_003E8__locals3 = new _003C_003Ec__DisplayClass15_0();
		if (config.Leve == 0)
		{
			return false;
		}
		if (config.LeveMete.GameObjectID == 0L || config.LeveReceiver.GameObjectID == 0L)
		{
			return false;
		}
		if (*leveAllowancesPtr <= 0)
		{
			return false;
		}
		if (!LuminaGetter.TryGetRow<Leve>(config.Leve, out Leve item))
		{
			return false;
		}
		RowRef dataId = ((Leve)(ref item)).DataId;
		if (!LuminaGetter.TryGetRow<CraftLeve>(((RowRef)(ref dataId)).RowId, out CS_0024_003C_003E8__locals3.craftLeveData))
		{
			return false;
		}
		return !(from x in Enumerable.Range(0, 4)
			select new
			{
				Obj = ((CraftLeve)(ref CS_0024_003C_003E8__locals3.craftLeveData)).Item[x],
				ItemCountNeeded = ((CraftLeve)(ref CS_0024_003C_003E8__locals3.craftLeveData)).ItemCount[x]
			} into x
			where x.Obj.ValueNullable.HasValue && x.ItemCountNeeded > 0
			select x).Any(x => iauMmVvw0M(x.Obj.RowId) < x.ItemCountNeeded);
	}

	private unsafe bool XCfMpNmfWZ()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		if (!q63Miuv0S8(out var eventID))
		{
			base.TaskHelper.Abort();
			return true;
		}
		if (((QuestManager)QuestManager.Instance()).LeveQuests.ToArray().FirstOrDefault((LeveWork x) => x.LeveId == config.Leve).LeveId != 0 && (*Addons.SelectString).IsAddonAndNodesReady())
		{
			AddonSelectStringEvent.Select(LuminaWrapper.GetAddonText(581u));
			return true;
		}
		if (!(*Addons.SelectString).IsAddonAndNodesReady())
		{
			new EventStartPackt(LocalPlayerState.EntityID, EventId.op_Implicit(eventID)).Send();
		}
		new EventActionPacket(EventId.op_Implicit(eventID), 16842752u, config.Leve).Send();
		return false;
	}

	private bool hUOM3guLOx()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()))
		{
			return true;
		}
		if (!rhNMPXPQIP(out var objectID, out var eventID))
		{
			base.TaskHelper.Abort();
			return true;
		}
		new EventStartPackt(GameObjectId.op_Implicit(objectID), EventId.op_Implicit(eventID)).Send();
		return false;
	}

	private void lTMMBHCLGl()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		_003C_003Ec__DisplayClass18_0 CS_0024_003C_003E8__locals3 = new _003C_003Ec__DisplayClass18_0();
		if (GameState.TerritoryType == 0 || GameState.ContentFinderCondition != 0)
		{
			return;
		}
		TerritoryType territoryTypeData = GameState.TerritoryTypeData;
		CS_0024_003C_003E8__locals3.placeNameID = ((TerritoryType)(ref territoryTypeData)).PlaceName.RowId;
		if (CS_0024_003C_003E8__locals3.placeNameID == 0)
		{
			return;
		}
		leveQuestsCurrentZone = ((IEnumerable<Leve>)LuminaGetter.Get<Leve>()).Where(delegate(Leve x)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			bool flag = ((Leve)(ref x)).PlaceNameIssued.RowId == CS_0024_003C_003E8__locals3.placeNameID && !string.IsNullOrEmpty(((object)((Leve)(ref x)).Name/*cast due to constrained. prefix*/).ToString());
			if (flag)
			{
				bool flag2;
				switch (((Leve)(ref x)).ClassJobCategory.RowId)
				{
				case 9u:
				case 10u:
				case 11u:
				case 12u:
				case 13u:
				case 14u:
				case 15u:
				case 16u:
				case 19u:
					flag2 = true;
					break;
				default:
					flag2 = false;
					break;
				}
				flag = flag2;
			}
			return flag;
		}).GroupBy(delegate(Leve x)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			ClassJobCategory value = ((Leve)(ref x)).ClassJobCategory.Value;
			return ((object)((ClassJobCategory)(ref value)).Name/*cast due to constrained. prefix*/).ToString();
		}).ToDictionary((IGrouping<string, Leve> x) => x.Key, (IGrouping<string, Leve> x) => x.ToList());
	}

	private unsafe static int iauMmVvw0M(uint itemID)
	{
		InventoryManager* ptr = InventoryManager.Instance();
		if (ptr == null)
		{
			return 0;
		}
		return ((InventoryManager)ptr).GetInventoryItemCount(itemID, false, true, true, (short)0) + ((InventoryManager)ptr).GetInventoryItemCount(itemID, true, true, true, (short)0);
	}

	private unsafe bool q63Miuv0S8(out EventId eventID)
	{
		GameObjectId eventObjectID;
		return (*EventFramework.Instance()).TryGetNearestEvent((EventHandlerInfo x) => (int)x.EventId.ContentId == 6, (GameObject x) => ((GameObject)(ref x)).NameString == config.LeveMete.Name, default(Vector3), out eventID, out eventObjectID);
	}

	private unsafe bool rhNMPXPQIP(out GameObjectId objectID, out EventId eventID)
	{
		return (*EventFramework.Instance()).TryGetNearestEvent((EventHandlerInfo x) => (int)x.EventId.ContentId == 14, (GameObject x) => ((GameObject)(ref x)).NameString == config.LeveReceiver.Name, default(Vector3), out eventID, out objectID);
	}

	private void kpiMhfFbAC(uint zone)
	{
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		base.TaskHelper.Abort();
		leveQuestsCurrentZone.Clear();
		lTMMBHCLGl();
		if (leveQuestsCurrentZone.Count != 0 && config.Leve == 0)
		{
			JISEppx0qoRRIL4rTwU jISEppx0qoRRIL4rTwU = config;
			Leve val = leveQuestsCurrentZone.FirstOrDefault<KeyValuePair<string, List<Leve>>>((KeyValuePair<string, List<Leve>> x) => x.Value.Count > 0).Value.FirstOrDefault();
			jISEppx0qoRRIL4rTwU.Leve = ((Leve)(ref val)).RowId;
			config.Save(this);
		}
	}

	private void zuAMosAWpH(AddonEvent type, AddonArgs args)
	{
		if (base.TaskHelper.IsBusy)
		{
			AddonSelectYesnoEvent.ClickYes();
		}
	}

	private unsafe void SiBMgkZhOY(AddonEvent type, AddonArgs args)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		if (base.TaskHelper.IsBusy && Addons.SelectIconString != null && LuminaGetter.TryGetRow<Leve>(config.Leve, out Leve item))
		{
			AddonSelectIconStringEvent.Select(((object)((Leve)(ref item)).Name/*cast due to constrained. prefix*/).ToString());
		}
	}

	public unsafe AutoLeveQuests()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x51E5B24E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a90a92322bff42e180982e106200d4a3)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x32D9427 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_86d75c161ced498ab6a767d2eb82d38e)),
			Category = ModuleCategory.Script,
			ModulesPrerequisite = new string[3]
			{
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5351A902 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_9085260e6b1a40438d219e85182b5d08),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xE09E007 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_aba6092d7cea44a49d1667540500e363),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x27D18921 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_39d5d8025f5249e6b489bc374eefbb3c)
			}
		};
		Permission = new ModulePermission
		{
			NeedAuth = true
		};
		leveAllowancesPtr = null;
		leveQuestsCurrentZone = new Dictionary<string, List<Leve>>();
		base._002Ector();
	}

	[CompilerGenerated]
	private bool qVjMw8eu3s(LeveWork x)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return x.LeveId == config.Leve;
	}

	[CompilerGenerated]
	private bool bylMES7P49(LeveWork x)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return x.LeveId == config.Leve;
	}

	[CompilerGenerated]
	private bool rl1MbnHnPf(GameObject x)
	{
		return ((GameObject)(ref x)).NameString == config.LeveMete.Name;
	}

	[CompilerGenerated]
	private bool gKWMFj4oZ9(GameObject x)
	{
		return ((GameObject)(ref x)).NameString == config.LeveReceiver.Name;
	}
}
