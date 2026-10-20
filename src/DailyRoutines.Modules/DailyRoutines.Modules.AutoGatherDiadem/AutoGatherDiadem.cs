using System;
using System.CodeDom.Compiler;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using AA2GHlTeHKUB9ArVZeZ;
using DailyRoutines.Common.Extensions;
using DailyRoutines.Common.Interface.Windows;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.LocalLifecycle;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Interface;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.InstanceContent;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Common.Math;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools;
using OmenTools.Dalamud;
using OmenTools.Dalamud.Abstractions;
using OmenTools.Dalamud.Attributes;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Info.Game.Data;
using OmenTools.Info.Game.Enums;
using OmenTools.Info.Game.Packets.Upstream;
using OmenTools.Interop.Game.AddonEvent;
using OmenTools.Interop.Game.ExecuteCommand.Implementations;
using OmenTools.Interop.Game.Helpers;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models.Native;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules.AutoGatherDiadem;

public class AutoGatherDiadem : ModuleBase
{
	private class kODftK3JogrJ0meUbml : ModuleConfig
	{
		public string AutoDiscardGroupName;

		public HashSet<string> BlacklistMonsters;

		public HashSet<uint> BlacklistWeathers;

		public JNX6T03ar0VQa8XFdl3? ExchangeItem;

		public bool IgnoreSkillPoints;

		public string ItemToGatherName;

		public List<uint> LevelThresholds;

		public GatheringMode Mode;

		public uint RoundMinute;

		public bool UseCordial;

		public kODftK3JogrJ0meUbml()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			AutoDiscardGroupName = string.Empty;
			BlacklistMonsters = new HashSet<string>();
			BlacklistWeathers = new HashSet<uint>();
			ItemToGatherName = string.Empty;
			LevelThresholds = new List<uint>();
			Mode = GatheringMode.冲分;
			RoundMinute = 40u;
			base._002Ector();
		}
	}

	private record a6gjlC3r34NOLGhTHo0
	{
		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass19_0
		{
			public AutoGatherDiadem module;

			public _003C_003Ec__DisplayClass19_0()
			{
				UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
				base._002Ector();
			}

			internal bool IXqP25VAHF((uint Index, uint ItemID, uint SuccessRate, string Name, uint Level, uint Rarity) x)
			{
				return x.Name.Contains(module.config.ItemToGatherName, StringComparison.OrdinalIgnoreCase);
			}
		}

		public uint Count { get; init; }

		public uint MaxCount { get; init; }

		public int Index { get; init; }

		public GatheringState State { get; init; }

		public a6gjlC3r34NOLGhTHo0(uint Count, uint MaxCount, int Index, GatheringState State)
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			this.Count = Count;
			this.MaxCount = MaxCount;
			this.Index = Index;
			this.State = State;
			base._002Ector();
		}

		public unsafe static a6gjlC3r34NOLGhTHo0 JGi3G2huHD(AtkUnitBase* addon, AutoGatherDiadem module)
		{
			_003C_003Ec__DisplayClass19_0 CS_0024_003C_003E8__locals4 = new _003C_003Ec__DisplayClass19_0();
			CS_0024_003C_003E8__locals4.module = module;
			AtkValue* atkValues = ((AtkUnitBase)addon).AtkValues;
			uint uInt = ((AtkValue)((byte*)atkValues + (nint)110 * (nint)Unsafe.SizeOf<AtkValue>())).UInt;
			if (uInt < 1)
			{
				return new a6gjlC3r34NOLGhTHo0(0u, uInt, -1, GatheringState.等待加载);
			}
			uint uInt2 = ((AtkValue)((byte*)atkValues + (nint)109 * (nint)Unsafe.SizeOf<AtkValue>())).UInt;
			if (uInt2 == 0)
			{
				return new a6gjlC3r34NOLGhTHo0(uInt2, uInt, -1, GatheringState.采集完毕);
			}
			List<(uint, uint, uint, string, uint, uint)> list = new List<(uint, uint, uint, string, uint, uint)>();
			for (uint num = 0u; num < 8; num++)
			{
				Dou360Bjxv(num, out uint itemID, out uint successRate, out string name, out uint level, out uint rarity);
				list.Add((num, itemID, successRate, name, level, rarity));
			}
			if (list.All<(uint, uint, uint, string, uint, uint)>(((uint Index, uint ItemID, uint SuccessRate, string Name, uint Level, uint Rarity) x) => x.ItemID == 0))
			{
				return new a6gjlC3r34NOLGhTHo0(uInt2, uInt, -1, GatheringState.等待加载);
			}
			if (list.All<(uint, uint, uint, string, uint, uint)>(((uint Index, uint ItemID, uint SuccessRate, string Name, uint Level, uint Rarity) x) => x.SuccessRate == 0))
			{
				return new a6gjlC3r34NOLGhTHo0(uInt2, uInt, -1, GatheringState.无法采集);
			}
			list = list.Except(list.Where<(uint, uint, uint, string, uint, uint)>(((uint Index, uint ItemID, uint SuccessRate, string Name, uint Level, uint Rarity) x) => x.SuccessRate == 0)).ToList();
			(uint, uint, uint, string, uint, uint) tuple = ((CS_0024_003C_003E8__locals4.module.config.Mode != GatheringMode.练级) ? ((!string.IsNullOrWhiteSpace(CS_0024_003C_003E8__locals4.module.config.ItemToGatherName)) ? (from x in list
				orderby x.Name.Contains(CS_0024_003C_003E8__locals4.module.config.ItemToGatherName, StringComparison.OrdinalIgnoreCase) descending, x.ItemID descending
				select x).FirstOrDefault() : list.OrderByDescending<(uint, uint, uint, string, uint, uint), uint>(((uint Index, uint ItemID, uint SuccessRate, string Name, uint Level, uint Rarity) x) => x.ItemID).FirstOrDefault()) : (from x in list
				where x.Level < 80
				orderby x.SuccessRate descending, x.Level descending
				select x).FirstOrDefault());
			(uint, uint, uint, string, uint, uint) tuple2 = tuple;
			if (tuple2.Item2 == 0 || tuple2.Item5 == 0)
			{
				return new a6gjlC3r34NOLGhTHo0(uInt2, uInt, -1, GatheringState.无法采集);
			}
			return new a6gjlC3r34NOLGhTHo0(uInt2, uInt, (int)tuple2.Item1, GatheringState.可以采集);
		}

		private unsafe static bool Dou360Bjxv(uint index, out uint itemID, out uint successRate, out string name, out uint level, out uint rarity)
		{
			itemID = 0u;
			successRate = 0u;
			name = string.Empty;
			level = 0u;
			rarity = 0u;
			uint num = 6 + index * 11;
			itemID = ((AtkValue)((byte*)((AtkUnitBase)Addons.Gathering).AtkValues + num * Unsafe.SizeOf<AtkValue>())).UInt;
			if (itemID == 0)
			{
				return false;
			}
			AtkComponentBase* componentByNodeId = ((AtkUnitBase)Addons.Gathering).GetComponentByNodeId(17 + index);
			if (componentByNodeId == null || !((AtkComponentNode)((AtkComponentBase)componentByNodeId).OwnerNode).IsVisible())
			{
				return false;
			}
			AtkTextNode* ptr = (AtkTextNode*)((AtkUldManager)(&((AtkComponentBase)componentByNodeId).UldManager)).SearchNodeById(16u);
			if (ptr == null || !((AtkTextNode)ptr).IsVisible() || !uint.TryParse(((object)(*(Utf8String*)(&((AtkTextNode)ptr).NodeText))/*cast due to constrained. prefix*/).ToString(), out successRate))
			{
				return false;
			}
			AtkTextNode* ptr2 = (AtkTextNode*)((AtkUldManager)(&((AtkComponentBase)componentByNodeId).UldManager)).SearchNodeById(23u);
			if (ptr2 != null && ((AtkTextNode)ptr2).IsVisible())
			{
				string text = ((object)(*(Utf8String*)(&((AtkTextNode)ptr2).NodeText))/*cast due to constrained. prefix*/).ToString();
				if (text != null)
				{
					name = text;
					AtkTextNode* ptr3 = (AtkTextNode*)((AtkUldManager)(&((AtkComponentBase)componentByNodeId).UldManager)).SearchNodeById(21u);
					if (ptr3 != null && ((AtkTextNode)ptr3).IsVisible())
					{
						string text2 = ((object)(*(Utf8String*)(&((AtkTextNode)ptr3).NodeText))/*cast due to constrained. prefix*/).ToString();
						if (text2 != null)
						{
							Match match = qh13kyHte8().Match(text2);
							if (!match.Success || !uint.TryParse(match.Value, out level))
							{
								return false;
							}
							AtkTextNode* ptr4 = (AtkTextNode*)((AtkUldManager)(&((AtkComponentBase)componentByNodeId).UldManager)).SearchNodeById(22u);
							if (ptr4 == null)
							{
								return false;
							}
							rarity = (((AtkTextNode)ptr4).IsVisible() ? ((uint)((object)(*(Utf8String*)(&((AtkTextNode)ptr4).NodeText))/*cast due to constrained. prefix*/).ToString().Count((char x) => x == '★')) : 0u);
							return true;
						}
					}
					return false;
				}
			}
			return false;
		}

		[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.42308")]
		[GeneratedRegex("\\d+")]
		private static Regex qh13kyHte8()
		{
			return vVWEW3TDrJdx8WHNH3d.Instance;
		}

		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xF20BC2B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_fea4ad1b13de4c4bbc22f571ceda7372));
			stringBuilder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x68AC8C9C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4722689f1a4d41dc9dc800bfe1a89c09));
			if (PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		[CompilerGenerated]
		protected virtual bool PrintMembers(StringBuilder builder)
		{
			RuntimeHelpers.EnsureSufficientExecutionStack();
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2194AF9C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cd1b319f326a44d1aaea645e95d8698b));
			builder.Append(Count.ToString());
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x64E5F2DA ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f53034f3efc64a309b10c7885bf5d9bd));
			builder.Append(MaxCount.ToString());
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2CE5CD43 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c9dc773883f149b7befba3bdbeac55f7));
			builder.Append(Index.ToString());
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4C4207F6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4643f8ec0000417ba22db1f06f6757bd));
			builder.Append(State.ToString());
			return true;
		}

		[CompilerGenerated]
		protected a6gjlC3r34NOLGhTHo0(a6gjlC3r34NOLGhTHo0 original)
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
			Count = original.Count;
			MaxCount = original.MaxCount;
			Index = original.Index;
			State = original.State;
		}

		[CompilerGenerated]
		public void teD32amfoD(out uint Count, out uint MaxCount, out int Index, out GatheringState State)
		{
			Count = this.Count;
			MaxCount = this.MaxCount;
			Index = this.Index;
			State = this.State;
		}
	}

	private struct OQSolR3ZF12HrPNe7VX
	{
		public uint ClassJob;

		public FrozenSet<string> GatherPointNames;

		public List<uint> ActionsAbove850;

		public List<uint> ActionsSingle;

		public uint DetectAction;

		public uint DetectStatus;

		public uint DetectPremiumAction;

		public uint DetectPremiumStatus;

		public uint BaseDetectAction;

		public uint BaseDetectStatus;

		public uint Achievement500K;
	}

	private record JNX6T03ar0VQa8XFdl3
	{
		public uint ShopID { get; init; }

		public string ItemName { get; init; }

		public uint ItemID { get; init; }

		public uint CostItemCount { get; init; }

		public JNX6T03ar0VQa8XFdl3(uint ShopID, string ItemName, uint ItemID, uint CostItemCount)
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			this.ShopID = ShopID;
			this.ItemName = ItemName;
			this.ItemID = ItemID;
			this.CostItemCount = CostItemCount;
			base._002Ector();
		}

		public static List<JNX6T03ar0VQa8XFdl3> e8o30PPBsC()
		{
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			int num = 3;
			List<uint> list = new List<uint>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<uint> span = CollectionsMarshal.AsSpan(list);
			span[0] = 1770041u;
			span[1] = 1770281u;
			span[2] = 1770301u;
			List<JNX6T03ar0VQa8XFdl3> list2 = new List<JNX6T03ar0VQa8XFdl3>();
			foreach (uint item in list)
			{
				SpecialShop valueOrDefault = LuminaGetter.GetRow<SpecialShop>(item).GetValueOrDefault();
				Enumerator<ItemStruct> enumerator2 = ((SpecialShop)(ref valueOrDefault)).Item.GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						ItemStruct current2 = enumerator2.Current;
						ReceiveItemsStruct val = ((ItemStruct)(ref current2)).ReceiveItems[0];
						uint rowId = ((ReceiveItemsStruct)(ref val)).Item.RowId;
						if (rowId != 0)
						{
							ItemCostsStruct val2 = ((ItemStruct)(ref current2)).ItemCosts[0];
							uint currencyCost = ((ItemCostsStruct)(ref val2)).CurrencyCost;
							if (currencyCost != 0)
							{
								val = ((ItemStruct)(ref current2)).ReceiveItems[0];
								Item value = ((ReceiveItemsStruct)(ref val)).Item.Value;
								list2.Add(new JNX6T03ar0VQa8XFdl3(item, ((object)((Item)(ref value)).Name/*cast due to constrained. prefix*/).ToString(), rowId, currencyCost));
							}
						}
					}
				}
				finally
				{
					((IDisposable)enumerator2/*cast due to constrained. prefix*/).Dispose();
				}
			}
			return list2;
		}

		public int ScN35uSHTD()
		{
			uint shopID = ShopID;
			switch (shopID)
			{
			case 1770041u:
				return 0;
			case 1770281u:
				return 1;
			case 1770301u:
				return 2;
			default:
			{
				global::_003CPrivateImplementationDetails_003E.ThrowSwitchExpressionException(shopID);
				int result = default(int);
				return result;
			}
			}
		}

		public unsafe bool Ewx3RUfdK4()
		{
			int num = ka13eWGiU9();
			if (num == 0)
			{
				return true;
			}
			int num2 = nP03DQCwMR();
			if (num2 == -1)
			{
				return false;
			}
			((AgentId)118).SendEvent(1uL, 0, num2, num, 0);
			return true;
		}

		public unsafe int nP03DQCwMR()
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			AgentShop* ptr = AgentShop.Instance();
			if (ptr == null || !((AgentShop)ptr).IsAgentActive() || ((AgentShop)ptr).ItemReceive == null)
			{
				return -1;
			}
			int num = -1;
			Span<ShopItem> itemReceiveSpan = ((AgentShop)ptr).ItemReceiveSpan;
			for (int i = 0; i < itemReceiveSpan.Length; i++)
			{
				ShopItem val = itemReceiveSpan[i];
				num++;
				if (val.ItemId == ItemID)
				{
					return num;
				}
			}
			return -1;
		}

		public unsafe int ka13eWGiU9()
		{
			return (int)(((InventoryManager)InventoryManager.Instance()).GetInventoryItemCount(28063u, false, true, true, (short)0) / CostItemCount);
		}

		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6AAECFE4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_839f542231154d29b0562b687afa328a));
			stringBuilder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x66B3044C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_762f55d0e7ef4ecd8f3f55c0f8a10fae));
			if (PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		[CompilerGenerated]
		protected virtual bool PrintMembers(StringBuilder builder)
		{
			RuntimeHelpers.EnsureSufficientExecutionStack();
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x66B30F12 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_762f55d0e7ef4ecd8f3f55c0f8a10fae));
			builder.Append(ShopID.ToString());
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6E004E28 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c40146824d9043e0906f4e1889cf39aa));
			builder.Append((object?)ItemName);
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x43D7D6BC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_974d377e1ed74986b81c34c95c2cc846));
			builder.Append(ItemID.ToString());
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x53573230 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_0676edd1b45a4ff096830fd6229bc1fb));
			builder.Append(CostItemCount.ToString());
			return true;
		}

		[CompilerGenerated]
		protected JNX6T03ar0VQa8XFdl3(JNX6T03ar0VQa8XFdl3 original)
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
			ShopID = original.ShopID;
			ItemName = original.ItemName;
			ItemID = original.ItemID;
			CostItemCount = original.CostItemCount;
		}

		[CompilerGenerated]
		public void UVh3N8x7iV(out uint ShopID, out string ItemName, out uint ItemID, out uint CostItemCount)
		{
			ShopID = this.ShopID;
			ItemName = this.ItemName;
			ItemID = this.ItemID;
			CostItemCount = this.CostItemCount;
		}
	}

	private class H4jDMP37AHjU9HEf5ti
	{
		private const float ACTIVATION_DISTANCE = 150f;

		private const float ACTIVATION_DISTANCE_SQUARED = 22500f;

		private const float USAGE_TIME = 5f;

		public Vector3 PointA { get; }

		public Vector3 PointB { get; }

		public H4jDMP37AHjU9HEf5ti(Vector3 pointA, Vector3 pointB)
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			PointA = pointA;
			PointB = pointB;
			base._002Ector();
		}

		public unsafe bool BIx3d0gI2Q(Vector3 currentPosition, Vector3 destination, out Vector3 entryPoint, bool ignoreDistance = false)
		{
			entryPoint = Vector3.Zero;
			float num = Vector3.DistanceSquared(currentPosition, PointA);
			float num2 = Vector3.DistanceSquared(currentPosition, PointB);
			bool flag = num <= 22500f;
			bool flag2 = num2 <= 22500f;
			if (!ignoreDistance && !flag && !flag2)
			{
				return false;
			}
			Vector3 vector = (flag ? PointA : PointB);
			Vector3 value = (flag ? PointB : PointA);
			float baseMovementSpeed = PlayerController.Instance()->MoveControllerWalk.BaseMovementSpeed;
			float num3 = Vector3.Distance(currentPosition, destination) / baseMovementSpeed;
			float num4 = Vector3.Distance(currentPosition, vector) / baseMovementSpeed;
			float num5 = Vector3.Distance(value, destination) / baseMovementSpeed;
			if (num4 + 5f + num5 < num3)
			{
				entryPoint = vector;
				return true;
			}
			return false;
		}

		public Vector3 nvO3I6dsaI(Vector3 entryPoint)
		{
			if (Vector3.DistanceSquared(entryPoint, PointA) < float.Epsilon)
			{
				return PointB;
			}
			if (Vector3.DistanceSquared(entryPoint, PointB) < float.Epsilon)
			{
				return PointA;
			}
			throw new ArgumentException(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x32D4AEB ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_86d75c161ced498ab6a767d2eb82d38e));
		}

		public unsafe static bool fAJ3USJCUR(H4jDMP37AHjU9HEf5ti[] teleporters, Vector3 currentPosition, Vector3 destination, out H4jDMP37AHjU9HEf5ti bestTeleporter, out Vector3 entryPoint, bool ignoreDistance = true)
		{
			bestTeleporter = null;
			entryPoint = Vector3.Zero;
			float num = 0f;
			float num2 = Vector3.Distance(currentPosition, destination);
			float baseMovementSpeed = PlayerController.Instance()->MoveControllerWalk.BaseMovementSpeed;
			float num3 = num2 / baseMovementSpeed;
			foreach (H4jDMP37AHjU9HEf5ti h4jDMP37AHjU9HEf5ti in teleporters)
			{
				if (h4jDMP37AHjU9HEf5ti.BIx3d0gI2Q(currentPosition, destination, out var entryPoint2, ignoreDistance))
				{
					Vector3 value = h4jDMP37AHjU9HEf5ti.nvO3I6dsaI(entryPoint2);
					float num4 = Vector3.Distance(currentPosition, entryPoint2) / baseMovementSpeed;
					float num5 = Vector3.Distance(value, destination) / baseMovementSpeed;
					float num6 = num4 + 5f + num5;
					float num7 = num3 - num6;
					if (num7 > num)
					{
						num = num7;
						bestTeleporter = h4jDMP37AHjU9HEf5ti;
						entryPoint = entryPoint2;
					}
				}
			}
			return bestTeleporter != null;
		}
	}

	private enum GatheringMode
	{
		练级,
		绿材,
		冲分
	}

	private enum GatheringState
	{
		可以采集,
		无法采集,
		采集完毕,
		等待加载
	}

	private enum GatheringNodeType
	{
		普通,
		园艺梦幻,
		采矿梦幻
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass34_0
	{
		public IPlayerCharacter localPlayer;

		public Vector3 targetPos;

		public _003C_003Ec__DisplayClass34_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal bool zO13umX0x1()
		{
			if (Vector2.DistanceSquared(localPlayer.Position.ToVector2(), targetPos.ToVector2()) > 6400f)
			{
				return false;
			}
			return true;
		}

		internal bool FkY3XpuP1v()
		{
			if (Vector3.DistanceSquared(localPlayer.Position, targetPos) <= 9f)
			{
				return vnavmeshIPC.GetPathfindWayPoints().Count <= 2;
			}
			return false;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass35_0
	{
		public Vector3 targetPos;

		public AutoGatherDiadem _003C_003E4__this;

		public bool appendTargetToPath;

		public float tolerance;

		public _003C_003Ec__DisplayClass35_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal async Task sZR3YUNGiZ(CancellationToken ct)
		{
			bool isOnMount = DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4];
			CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(ct);
			List<Vector3> list = await vnavmeshIPC.PathfindCancelable(LocalPlayerState.Object.Position, targetPos, isOnMount, cancellationTokenSource.Token);
			if (Vector2.DistanceSquared(list.Last().ToVector2(), targetPos.ToVector2()) > 64f)
			{
				string text = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3D578546 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e9e296e241f44828a57c750102ce71c1));
				OmenServiceBase<NotifyHelper>.Instance().ChatError(ReadOnlySeString.op_Implicit(text));
				NotifyHelper.ToastError(text);
				_003C_003E4__this.Xf68WqfRR8();
				_003C_003E4__this.dlAsRiL5Ej();
			}
			else
			{
				if (appendTargetToPath)
				{
					list.Add(targetPos);
				}
				vnavmeshIPC.SetPathfindTolerance(tolerance);
				vnavmeshIPC.PathfindWithPath(list, isOnMount);
			}
		}

		internal void uBp3zthlmg()
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x32D4A51 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_86d75c161ced498ab6a767d2eb82d38e))));
			_003C_003E4__this.Xf68WqfRR8();
			_003C_003E4__this.dlAsRiL5Ej();
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass36_0
	{
		public AutoGatherDiadem _003C_003E4__this;

		public IPlayerCharacter localPlayer;

		public _003C_003Ec__DisplayClass36_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal unsafe void RVCBnuawqH()
		{
			_003C_003Ec__DisplayClass36_1 CS_0024_003C_003E8__locals15 = new _003C_003Ec__DisplayClass36_1();
			CS_0024_003C_003E8__locals15.CS_0024_003C_003E8__locals1 = this;
			if (!_003C_003E4__this.KdA899cbTc(out CS_0024_003C_003E8__locals15.gatherPos, out CS_0024_003C_003E8__locals15.gatherObj, out GatheringNodeType _) || !(*EventFramework.Instance()).TryGetNearestEvent((EventHandlerInfo x) => (int)x.EventId.ContentId == 3, (GameObject x) => ((GameObject)(ref x)).NameString == CS_0024_003C_003E8__locals15.gatherObj.Name, CS_0024_003C_003E8__locals15.gatherPos, out CS_0024_003C_003E8__locals15.eventID, out CS_0024_003C_003E8__locals15.eventObjectID))
			{
				_003C_003E4__this.TaskHelper.Abort();
				_003C_003E4__this.TaskHelper.DelayNext(1000, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D75E390 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8b51f8019ce49018b38e06104a97fc1));
				_003C_003E4__this.TaskHelper.Enqueue((Action)_003C_003E4__this.rIlsudQLBS, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x12D6B980 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7fcf5a71a7ed445197f132979519c560), 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
				return;
			}
			vnavmeshIPC.StopPathfind();
			TargetManager.Target = CS_0024_003C_003E8__locals15.gatherObj;
			_003C_003E4__this.TaskHelper.Enqueue(delegate
			{
				if (!IConditionExtension.get_IsOnMount(DalamudServiceExtension.Instance<ICondition>()))
				{
					return true;
				}
				OmenServiceBase<MovementManager>.Instance().Dismount();
				return false;
			}, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x53517712 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_9085260e6b1a40438d219e85182b5d08), 5000, TaskAbortBehaviour.AbortCurrent, null, delegate
			{
				OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.DisableMounting, 1u);
			});
			_003C_003E4__this.TaskHelper.Enqueue(delegate
			{
				Vector3 position = CS_0024_003C_003E8__locals15.CS_0024_003C_003E8__locals1.localPlayer.Position;
				if (Vector3.DistanceSquared(position, CS_0024_003C_003E8__locals15.gatherPos) <= 12f && MathF.Abs(position.Y - CS_0024_003C_003E8__locals15.gatherPos.Y) <= 3f)
				{
					return true;
				}
				if (OmenServiceBase<MovementManager>.Instance().IsManagerBusy || !UIModuleExtension.IsScreenReady())
				{
					return false;
				}
				OmenServiceBase<MovementManager>.Instance().TPSmart_InZone(RaycastHelper.TryGetGroundPosition(CS_0024_003C_003E8__locals15.gatherPos, out var groundPosition) ? groundPosition : CS_0024_003C_003E8__locals15.gatherPos);
				return false;
			}, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--1470179789 ^ 0x99A16D1 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2d7d8a5df6744c51b35f486f03b53ded));
			_003C_003E4__this.TaskHelper.Enqueue(delegate
			{
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				if (Addons.Gathering != null)
				{
					return true;
				}
				new EventStartPackt(GameObjectId.op_Implicit(CS_0024_003C_003E8__locals15.eventObjectID), EventId.op_Implicit(CS_0024_003C_003E8__locals15.eventID)).Send();
				return false;
			}, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489F1A5A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b2294f2f9bb24a76b00cf1af75888414));
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass36_1
	{
		public IGameObject gatherObj;

		public Vector3 gatherPos;

		public GameObjectId eventObjectID;

		public EventId eventID;

		public _003C_003Ec__DisplayClass36_0 CS_0024_003C_003E8__locals1;

		public _003C_003Ec__DisplayClass36_1()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal bool VNvBydZQBs(GameObject x)
		{
			return ((GameObject)(ref x)).NameString == gatherObj.Name;
		}

		internal bool UkuBLsVEoW()
		{
			Vector3 position = CS_0024_003C_003E8__locals1.localPlayer.Position;
			if (Vector3.DistanceSquared(position, gatherPos) <= 12f && MathF.Abs(position.Y - gatherPos.Y) <= 3f)
			{
				return true;
			}
			if (OmenServiceBase<MovementManager>.Instance().IsManagerBusy || !UIModuleExtension.IsScreenReady())
			{
				return false;
			}
			OmenServiceBase<MovementManager>.Instance().TPSmart_InZone(RaycastHelper.TryGetGroundPosition(gatherPos, out var groundPosition) ? groundPosition : gatherPos);
			return false;
		}

		internal unsafe bool jclBHR5YUb()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			if (Addons.Gathering != null)
			{
				return true;
			}
			new EventStartPackt(GameObjectId.op_Implicit(eventObjectID), EventId.op_Implicit(eventID)).Send();
			return false;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass37_0
	{
		public JNX6T03ar0VQa8XFdl3 item;

		public _003C_003Ec__DisplayClass37_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal bool VYWBS1MYe8()
		{
			return AddonSelectStringEvent.Select(item.ScN35uSHTD());
		}

		internal bool XgeBVT6Sfi()
		{
			return item.ka13eWGiU9() == 0;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass38_0
	{
		public uint index;

		public _003C_003Ec__DisplayClass38_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal unsafe void aN5B9h96X2()
		{
			(*Addons.HWDGathereInspect).Callback(14, index % 2);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass42_0
	{
		public InventoryItem? cordialItem;

		public _003C_003Ec__DisplayClass42_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal unsafe void DHNBftfiPH()
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			AgentInventoryContext* intPtr = AgentInventoryContext.Instance();
			InventoryItem value = cordialItem.Value;
			uint itemId = ((InventoryItem)(ref value)).GetItemId();
			value = cordialItem.Value;
			InventoryType inventoryType = ((InventoryItem)(ref value)).GetInventoryType();
			value = cordialItem.Value;
			((AgentInventoryContext)intPtr).UseItem(itemId, inventoryType, (uint)((InventoryItem)(ref value)).GetSlot(), (short)0);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass43_0
	{
		public unsafe RecommendEquipModule* instance;

		public _003C_003Ec__DisplayClass43_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal unsafe bool vkXBC5L2tp()
		{
			return ((RecommendEquipModule)instance).SetupForClassJob((byte)LocalPlayerState.ClassJob);
		}

		internal unsafe bool jAtBjLyX8h()
		{
			if (((RecommendEquipModule)instance).IsUpdating)
			{
				return false;
			}
			((RecommendEquipModule)instance).EquipRecommendedGear();
			return true;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass51_0
	{
		public AutoGatherDiadem _003C_003E4__this;

		public OQSolR3ZF12HrPNe7VX jobInfo;

		public _003C_003Ec__DisplayClass51_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal unsafe bool J0YBOgAB8d()
		{
			GatheringState state = a6gjlC3r34NOLGhTHo0.JGi3G2huHD(Addons.Gathering, _003C_003E4__this).State;
			if (state != GatheringState.可以采集 && state == GatheringState.等待加载)
			{
				return false;
			}
			return true;
		}

		internal unsafe void iMWBKqpZBb()
		{
			BattleChara* localPlayer = Control.GetLocalPlayer();
			ushort gatheringPoints = ((BattleChara)localPlayer).GatheringPoints;
			ushort maxGatheringPoints = ((BattleChara)localPlayer).MaxGatheringPoints;
			a6gjlC3r34NOLGhTHo0 a6gjlC3r34NOLGhTHo1 = a6gjlC3r34NOLGhTHo0.JGi3G2huHD(Addons.Gathering, _003C_003E4__this);
			if (maxGatheringPoints >= 850)
			{
				if (gatheringPoints >= 850)
				{
					using (List<uint>.Enumerator enumerator = jobInfo.ActionsAbove850.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							_003C_003Ec__DisplayClass51_1 CS_0024_003C_003E8__locals13 = new _003C_003Ec__DisplayClass51_1();
							CS_0024_003C_003E8__locals13.action = enumerator.Current;
							if (ActionManagerExtension.IsActionUnlocked(CS_0024_003C_003E8__locals13.action))
							{
								_003C_003E4__this.TaskHelper.Enqueue(() => !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)42] && OmenServiceBase<UseActionManager>.Instance().UseActionLocation((ActionType)1, CS_0024_003C_003E8__locals13.action, 3758096384uL, default(Vector3), 0u, 0), LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x76EB776F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b988ddaee02846b69dd4e446b5da9de9) + LuminaWrapper.GetActionName(CS_0024_003C_003E8__locals13.action));
							}
						}
						return;
					}
				}
				if (_003C_003E4__this.IP08j7SYp5(out var _, out var _) || (_003C_003E4__this.config.UseCordial && cTK8Omm4j3()) || gatheringPoints + 6 * a6gjlC3r34NOLGhTHo1.Count + 50 < maxGatheringPoints)
				{
					return;
				}
				using List<uint>.Enumerator enumerator = jobInfo.ActionsSingle.GetEnumerator();
				while (enumerator.MoveNext())
				{
					_003C_003Ec__DisplayClass51_2 CS_0024_003C_003E8__locals14 = new _003C_003Ec__DisplayClass51_2();
					CS_0024_003C_003E8__locals14.actionSingle = enumerator.Current;
					if (ActionManagerExtension.IsActionUnlocked(CS_0024_003C_003E8__locals14.actionSingle))
					{
						_003C_003E4__this.TaskHelper.Enqueue(() => !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)42] && OmenServiceBase<UseActionManager>.Instance().UseActionLocation((ActionType)1, CS_0024_003C_003E8__locals14.actionSingle, 3758096384uL, default(Vector3), 0u, 0), LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x258F7474 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_dadeb2ab61424d31be7cc1966e675825) + LuminaWrapper.GetActionName(CS_0024_003C_003E8__locals14.actionSingle));
						break;
					}
				}
				return;
			}
			if (gatheringPoints < 100)
			{
				return;
			}
			int num = gatheringPoints;
			using IEnumerator<uint> enumerator2 = jobInfo.ActionsAbove850.Concat(jobInfo.ActionsSingle).GetEnumerator();
			while (enumerator2.MoveNext())
			{
				_003C_003Ec__DisplayClass51_3 CS_0024_003C_003E8__locals15 = new _003C_003Ec__DisplayClass51_3();
				CS_0024_003C_003E8__locals15.actionSingle = enumerator2.Current;
				if (LuminaGetter.TryGetRow<Action>(CS_0024_003C_003E8__locals15.actionSingle, out Action item2) && ActionManagerExtension.IsActionUnlocked(CS_0024_003C_003E8__locals15.actionSingle) && num - ((Action)(ref item2)).PrimaryCostValue >= 0)
				{
					num -= ((Action)(ref item2)).PrimaryCostValue;
					_003C_003E4__this.TaskHelper.Enqueue(() => !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)42] && OmenServiceBase<UseActionManager>.Instance().UseActionLocation((ActionType)1, CS_0024_003C_003E8__locals15.actionSingle, 3758096384uL, default(Vector3), 0u, 0), LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6854003D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_23522a2a14314da28eb44187a09cd0b5) + LuminaWrapper.GetActionName(CS_0024_003C_003E8__locals15.actionSingle));
				}
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass51_1
	{
		public uint action;

		public _003C_003Ec__DisplayClass51_1()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal bool x82BtlZFXE()
		{
			if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)42])
			{
				return false;
			}
			return OmenServiceBase<UseActionManager>.Instance().UseActionLocation((ActionType)1, action, 3758096384uL, default(Vector3), 0u, 0);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass51_2
	{
		public uint actionSingle;

		public _003C_003Ec__DisplayClass51_2()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal bool ESDBWJD5ek()
		{
			if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)42])
			{
				return false;
			}
			return OmenServiceBase<UseActionManager>.Instance().UseActionLocation((ActionType)1, actionSingle, 3758096384uL, default(Vector3), 0u, 0);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass51_3
	{
		public uint actionSingle;

		public _003C_003Ec__DisplayClass51_3()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal bool z3lBTo0Ine()
		{
			if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)42])
			{
				return false;
			}
			return OmenServiceBase<UseActionManager>.Instance().UseActionLocation((ActionType)1, actionSingle, 3758096384uL, default(Vector3), 0u, 0);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass55_0
	{
		public IGameObject monster;

		public _003C_003Ec__DisplayClass55_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal bool p6NB1Yy35U()
		{
			return OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)1, 19700u, monster.EntityID, 0u, (UseActionMode)0);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass56_0
	{
		public OQSolR3ZF12HrPNe7VX jobInfo;

		public _003C_003Ec__DisplayClass56_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal bool WScBMG1guZ()
		{
			return OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)1, jobInfo.BaseDetectAction, 3758096384uL, 0u, (UseActionMode)0);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass57_0
	{
		public uint targetJob;

		public OQSolR3ZF12HrPNe7VX jobInfo;

		public _003C_003Ec__DisplayClass57_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal bool M6bB4q5xfy()
		{
			return OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)1, jobInfo.DetectPremiumAction, 3758096384uL, 0u, (UseActionMode)0);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass57_1
	{
		public byte targetJobGearset;

		public _003C_003Ec__DisplayClass57_0 CS_0024_003C_003E8__locals1;

		public _003C_003Ec__DisplayClass57_1()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal bool riFBsuIi6H()
		{
			if (LocalPlayerState.ClassJob == CS_0024_003C_003E8__locals1.targetJob)
			{
				return true;
			}
			LocalPlayerState.SwitchGearset(targetJobGearset);
			return false;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass58_0
	{
		public uint currentJob;

		public OQSolR3ZF12HrPNe7VX jobInfo;

		public AutoGatherDiadem _003C_003E4__this;

		public _003C_003Ec__DisplayClass58_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal bool H1uB8gNgwi(KeyValuePair<uint, OQSolR3ZF12HrPNe7VX> x)
		{
			return x.Key != currentJob;
		}

		internal unsafe bool Ut6BvaJkwp()
		{
			if (LocalPlayerState.HasStatus(jobInfo.DetectStatus, out var _))
			{
				return true;
			}
			return ((ActionManager)ActionManager.Instance()).IsActionOffCooldown((ActionType)1, jobInfo.DetectAction);
		}

		internal bool HhhBlhN8r5()
		{
			if (LocalPlayerState.HasStatus(jobInfo.DetectStatus, out var _))
			{
				return true;
			}
			return OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)1, jobInfo.DetectAction, 3758096384uL, 0u, (UseActionMode)0);
		}

		internal bool sd4BxU8jNS(uint targetJob)
		{
			_003C_003Ec__DisplayClass58_2 CS_0024_003C_003E8__locals7 = new _003C_003Ec__DisplayClass58_2();
			CS_0024_003C_003E8__locals7.targetJob = targetJob;
			if (LocalPlayerState.GetClassJobLevel(CS_0024_003C_003E8__locals7.targetJob) < 10 || !LocalPlayerState.TryFindClassJobGearset(CS_0024_003C_003E8__locals7.targetJob, out CS_0024_003C_003E8__locals7.gearsetID))
			{
				return false;
			}
			_003C_003E4__this.TaskHelper.Enqueue(delegate
			{
				if (LocalPlayerState.ClassJob == CS_0024_003C_003E8__locals7.targetJob)
				{
					return true;
				}
				LocalPlayerState.SwitchGearset(CS_0024_003C_003E8__locals7.gearsetID);
				return false;
			}, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x454EEB95 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_09da05f368544962acb62b42d9155467) + LuminaWrapper.GetJobName(CS_0024_003C_003E8__locals7.targetJob));
			_003C_003E4__this.TaskHelper.Enqueue((Action)_003C_003E4__this.rIlsudQLBS, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2310B0A9 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a96fab9e97944b39a8afe3fb1109ba7d), 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
			return true;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass58_1
	{
		public ushort currentJobLevel;

		public ushort otherJobLevel;

		public _003C_003Ec__DisplayClass58_1()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal bool Ye0BpqcTlg(uint t)
		{
			return currentJobLevel >= t;
		}

		internal bool QFNB3ItEuS(uint t)
		{
			return otherJobLevel >= t;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass58_2
	{
		public uint targetJob;

		public byte gearsetID;

		public _003C_003Ec__DisplayClass58_2()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal bool XJSBBjaL6C()
		{
			if (LocalPlayerState.ClassJob == targetJob)
			{
				return true;
			}
			LocalPlayerState.SwitchGearset(gearsetID);
			return false;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass67_0
	{
		public OQSolR3ZF12HrPNe7VX info;

		public _003C_003Ec__DisplayClass67_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal unsafe bool lLCBmR7fOi(IGameObject x)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Invalid comparison between Unknown and I4
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			if ((int)x.ObjectKind == 6 && info.GatherPointNames.Contains(x.Name.ToString()))
			{
				return ((BattleChara)x.ToBCStruct()).TargetableStatus.IsSetAll<ObjectTargetableFlags>(new ReadOnlySpan<ObjectTargetableFlags>(Unsafe.AsPointer(ref global::_003CPrivateImplementationDetails_003E._8263A852B3877BAB05AE51AE58179841C08B22A25E0E964F5C157690B61F9B9C), 2));
			}
			return false;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass68_0
	{
		public AutoGatherDiadem _003C_003E4__this;

		public unsafe BattleChara* localPlayer;

		public _003C_003Ec__DisplayClass68_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal bool QfqBiDxbiB(IGameObject x)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Invalid comparison between Unknown and I4
			if (x != null && (int)x.ObjectKind == 2 && x.IsTargetable && !x.IsDead)
			{
				return !_003C_003E4__this.config.BlacklistMonsters.Contains(x.Name.ToString());
			}
			return false;
		}

		internal unsafe _003C_003Ef__AnonymousType1<IGameObject, float> iv7BPH8Dkw(IGameObject x)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			return new
			{
				Object = x,
				Distance = Vector3.DistanceSquared(x.Position, Vector3.op_Implicit(((BattleChara)localPlayer).Position))
			};
		}

		internal unsafe bool W38BhAMTvT(_003C_003Ef__AnonymousType1<IGameObject, float> x)
		{
			return ActionManager.GetActionInRangeOrLoS(19700u, (GameObject*)localPlayer, x.Object.ToStruct()) != 562;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass70_0
	{
		public uint cordial;

		public _003C_003Ec__DisplayClass70_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal bool qIsBoM3TQd(InventoryItem x)
		{
			return ((InventoryItem)(ref x)).GetBaseItemId() == cordial;
		}
	}

	private kODftK3JogrJ0meUbml config;

	private System.Timers.Timer? operationWatchdogTimer;

	private string addNewBlacklistMonsterInput;

	private uint addNewLevelThresholdInput;

	private long gatheringAddonFirstDisplayTime;

	private uint useAetherAugerCount;

	private uint lastLevel;

	private bool isOnAutoGather;

	private const uint ZONE_FIRMAMENT = 886u;

	private const uint ZONE_DIADEM = 939u;

	private static readonly Dictionary<GatheringMode, (string Title, string Description)> GatheringModeLoc;

	private static readonly List<JNX6T03ar0VQa8XFdl3> ExchangeItems;

	private static readonly Dictionary<uint, OQSolR3ZF12HrPNe7VX> JobInfos;

	private static readonly H4jDMP37AHjU9HEf5ti[] Teleporters;

	private static readonly Dictionary<uint, uint> CordialItems;

	private static readonly Dictionary<uint, uint> SpecialWeathers;

	private static readonly List<uint> SpecialItems;

	private static readonly HashSet<string> CloudedGatherPointNames;

	private static readonly Dictionary<string, Func<bool>> PreCondition;

	private static readonly Vector3 RedirectPosition;

	private static readonly Vector3 ZoneDefaultPosition;

	[IPCSubscriber("DailyRoutines.Modules.AutoDiscard.IsBusy", DefaultValue = "false")]
	private IPCSubscriber<bool>? IsAutoDiscardModuleBusy;

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected unsafe override void Init()
	{
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Expected O, but got Unknown
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Expected O, but got Unknown
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Expected O, but got Unknown
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Expected O, but got Unknown
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Expected O, but got Unknown
		TimerLifetime.Begin(this);
		config = ModuleConfigExtension.Load<kODftK3JogrJ0meUbml>(this) ?? new kODftK3JogrJ0meUbml
		{
			BlacklistMonsters = new HashSet<string> { LuminaWrapper.GetBNPCName(2250u) },
			ExchangeItem = ExchangeItems.First()
		};
		if (config.LevelThresholds.Count == 0)
		{
			kODftK3JogrJ0meUbml obj = config;
			int num = 3;
			List<uint> list = new List<uint>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<uint> span = CollectionsMarshal.AsSpan(list);
			span[0] = 50u;
			span[1] = 70u;
			span[2] = 100u;
			obj.LevelThresholds = list;
			config.Save(this);
		}
		if (base.TaskHelper == null)
		{
			TaskHelper obj2 = new TaskHelper
			{
				TimeoutMS = 180000,
				ShowDebug = true,
				TimeoutAction = delegate
				{
					if (isOnAutoGather)
					{
						Xf68WqfRR8();
						Nw6s2mwgSI();
					}
				},
				ExceptionAction = delegate
				{
					if (isOnAutoGather)
					{
						Xf68WqfRR8();
						Nw6s2mwgSI();
					}
				},
				RetryIntervalMS = 100,
				TaskIntervalMS = 100
			};
			TaskHelper taskHelper = obj2;
			base.TaskHelper = obj2;
		}
		if (base.Overlay == null)
		{
			Overlay overlay = (base.Overlay = new Overlay(this));
		}
		((Window)base.Overlay).WindowName = Info.Title + LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3AC92F44 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3932528062874ea4bb7e2e1185394e37);
		((Window)base.Overlay).Flags = (ImGuiWindowFlags)73;
		if ((*Addons.Gathering).IsAddonAndNodesReady())
		{
			gatheringAddonFirstDisplayTime = Environment.TickCount64;
		}
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)1, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7C2F11B0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_12c2d68ba4654622a4959ee73cacc6d7), new AddonEventDelegate(NdFs7xny57));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)5, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5A7FBCED ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5154ccd9fed0487f8fa0244531466009), new AddonEventDelegate(NdFs7xny57));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)1, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5E3BE2D0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2d7d8a5df6744c51b35f486f03b53ded), new AddonEventDelegate(mjmsdohhQn));
		DalamudServiceExtension.Instance<ICondition>().ConditionChange += new ConditionChangeDelegate(iE7se5LIWF);
		OmenServiceBase<FrameworkManager>.Instance().Reg(new OnUpdateDelegate(gOdsUMs2qF), 10000u);
		OmenServiceBase<GameState>.Instance().Login += h3isIc8wFO;
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += zVKsDoVnur;
	}

	protected override void Uninit()
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Expected O, but got Unknown
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Expected O, but got Unknown
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected O, but got Unknown
		TimerLifetime.End(this);
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= zVKsDoVnur;
		OmenServiceBase<GameState>.Instance().Login -= h3isIc8wFO;
		OmenServiceBase<FrameworkManager>.Instance().Unreg(gOdsUMs2qF);
		DalamudServiceExtension.Instance<ICondition>().ConditionChange -= new ConditionChangeDelegate(iE7se5LIWF);
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { NdFs7xny57 });
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { mjmsdohhQn });
		SAA8t57991();
		isOnAutoGather = false;
		lastLevel = 0u;
	}

	private void LBisJck0t6(Vector3 targetPos, bool isMarker)
	{
		_003C_003Ec__DisplayClass34_0 CS_0024_003C_003E8__locals12 = new _003C_003Ec__DisplayClass34_0();
		CS_0024_003C_003E8__locals12.targetPos = targetPos;
		CS_0024_003C_003E8__locals12.localPlayer = LocalPlayerState.Object;
		if (CS_0024_003C_003E8__locals12.localPlayer == null)
		{
			return;
		}
		if (isMarker)
		{
			Vector3? vector = vnavmeshIPC.QueryNearestPointOnMesh(CS_0024_003C_003E8__locals12.targetPos, 5f, 10000f);
			if (vector.HasValue)
			{
				Vector3 valueOrDefault = vector.GetValueOrDefault();
				CS_0024_003C_003E8__locals12.targetPos = valueOrDefault + new Vector3(0f, 20f, 0f);
			}
		}
		Vector3 position = CS_0024_003C_003E8__locals12.localPlayer.Position;
		if (H4jDMP37AHjU9HEf5ti.fAJ3USJCUR(Teleporters, position, CS_0024_003C_003E8__locals12.targetPos, out H4jDMP37AHjU9HEf5ti _, out Vector3 entryPoint))
		{
			base.TaskHelper.Abort();
			nw9srtqj6o(entryPoint, 0.1f, appendTargetToPath: true);
			base.TaskHelper.Enqueue(() => DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)48], LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x43D75266 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_974d377e1ed74986b81c34c95c2cc846));
			return;
		}
		nw9srtqj6o(CS_0024_003C_003E8__locals12.targetPos, 1.5f, !isMarker);
		if (isMarker)
		{
			base.TaskHelper.Enqueue(() => !(Vector2.DistanceSquared(CS_0024_003C_003E8__locals12.localPlayer.Position.ToVector2(), CS_0024_003C_003E8__locals12.targetPos.ToVector2()) > 6400f), LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7FD8E8F1 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1eab987c1db64b928220e27bd03141d3));
			base.TaskHelper.Enqueue((Action)rIlsudQLBS, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x369576E9 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_26e231ba68f74e8784503e099ddfc2d9), 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
		}
		else
		{
			base.TaskHelper.Enqueue(() => Vector3.DistanceSquared(CS_0024_003C_003E8__locals12.localPlayer.Position, CS_0024_003C_003E8__locals12.targetPos) <= 9f && vnavmeshIPC.GetPathfindWayPoints().Count <= 2, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5E3BBD2C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2d7d8a5df6744c51b35f486f03b53ded));
		}
	}

	private void nw9srtqj6o(Vector3 targetPos, float tolerance = 1.5f, bool appendTargetToPath = false)
	{
		_003C_003Ec__DisplayClass35_0 CS_0024_003C_003E8__locals15 = new _003C_003Ec__DisplayClass35_0();
		CS_0024_003C_003E8__locals15.targetPos = targetPos;
		CS_0024_003C_003E8__locals15._003C_003E4__this = this;
		CS_0024_003C_003E8__locals15.appendTargetToPath = appendTargetToPath;
		CS_0024_003C_003E8__locals15.tolerance = tolerance;
		if (!DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4] && !lrB8KAr7VI(CS_0024_003C_003E8__locals15.targetPos))
		{
			mktsaGpdpf();
		}
		TaskHelper? taskHelper = base.TaskHelper;
		Func<CancellationToken, Task> asyncTask = async delegate(CancellationToken ct)
		{
			bool isOnMount = DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4];
			CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(ct);
			List<Vector3> list = await vnavmeshIPC.PathfindCancelable(LocalPlayerState.Object.Position, CS_0024_003C_003E8__locals15.targetPos, isOnMount, cancellationTokenSource.Token);
			if (Vector2.DistanceSquared(list.Last().ToVector2(), CS_0024_003C_003E8__locals15.targetPos.ToVector2()) > 64f)
			{
				string text = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3D578546 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e9e296e241f44828a57c750102ce71c1));
				OmenServiceBase<NotifyHelper>.Instance().ChatError(ReadOnlySeString.op_Implicit(text));
				NotifyHelper.ToastError(text);
				CS_0024_003C_003E8__locals15._003C_003E4__this.Xf68WqfRR8();
				CS_0024_003C_003E8__locals15._003C_003E4__this.dlAsRiL5Ej();
			}
			else
			{
				if (CS_0024_003C_003E8__locals15.appendTargetToPath)
				{
					list.Add(CS_0024_003C_003E8__locals15.targetPos);
				}
				vnavmeshIPC.SetPathfindTolerance(CS_0024_003C_003E8__locals15.tolerance);
				vnavmeshIPC.PathfindWithPath(list, isOnMount);
			}
		};
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(8, 1);
		defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489F98B2 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b2294f2f9bb24a76b00cf1af75888414));
		defaultInterpolatedStringHandler.AppendFormatted(CS_0024_003C_003E8__locals15.targetPos);
		taskHelper.EnqueueAsync(asyncTask, defaultInterpolatedStringHandler.ToStringAndClear(), 30000, TaskAbortBehaviour.AbortAll, null, delegate
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x32D4A51 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_86d75c161ced498ab6a767d2eb82d38e))));
			CS_0024_003C_003E8__locals15._003C_003E4__this.Xf68WqfRR8();
			CS_0024_003C_003E8__locals15._003C_003E4__this.dlAsRiL5Ej();
		});
	}

	private unsafe void ydisGH3OJ3()
	{
		_003C_003Ec__DisplayClass36_0 CS_0024_003C_003E8__locals19 = new _003C_003Ec__DisplayClass36_0();
		CS_0024_003C_003E8__locals19._003C_003E4__this = this;
		CS_0024_003C_003E8__locals19.localPlayer = LocalPlayerState.Object;
		base.TaskHelper.Enqueue(delegate
		{
			_003C_003Ec__DisplayClass36_1 CS_0024_003C_003E8__locals27 = new _003C_003Ec__DisplayClass36_1();
			CS_0024_003C_003E8__locals27.CS_0024_003C_003E8__locals1 = CS_0024_003C_003E8__locals19;
			if (!CS_0024_003C_003E8__locals19._003C_003E4__this.KdA899cbTc(out CS_0024_003C_003E8__locals27.gatherPos, out CS_0024_003C_003E8__locals27.gatherObj, out GatheringNodeType _) || !(*EventFramework.Instance()).TryGetNearestEvent((EventHandlerInfo x) => (int)x.EventId.ContentId == 3, (GameObject x) => ((GameObject)(ref x)).NameString == CS_0024_003C_003E8__locals27.gatherObj.Name, CS_0024_003C_003E8__locals27.gatherPos, out CS_0024_003C_003E8__locals27.eventID, out CS_0024_003C_003E8__locals27.eventObjectID))
			{
				CS_0024_003C_003E8__locals19._003C_003E4__this.TaskHelper.Abort();
				CS_0024_003C_003E8__locals19._003C_003E4__this.TaskHelper.DelayNext(1000, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D75E390 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8b51f8019ce49018b38e06104a97fc1));
				CS_0024_003C_003E8__locals19._003C_003E4__this.TaskHelper.Enqueue((Action)CS_0024_003C_003E8__locals19._003C_003E4__this.rIlsudQLBS, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x12D6B980 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7fcf5a71a7ed445197f132979519c560), 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
			}
			else
			{
				vnavmeshIPC.StopPathfind();
				TargetManager.Target = CS_0024_003C_003E8__locals27.gatherObj;
				CS_0024_003C_003E8__locals19._003C_003E4__this.TaskHelper.Enqueue(delegate
				{
					if (!IConditionExtension.get_IsOnMount(DalamudServiceExtension.Instance<ICondition>()))
					{
						return true;
					}
					OmenServiceBase<MovementManager>.Instance().Dismount();
					return false;
				}, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x53517712 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_9085260e6b1a40438d219e85182b5d08), 5000, TaskAbortBehaviour.AbortCurrent, null, delegate
				{
					OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.DisableMounting, 1u);
				});
				CS_0024_003C_003E8__locals19._003C_003E4__this.TaskHelper.Enqueue(delegate
				{
					Vector3 position = CS_0024_003C_003E8__locals27.CS_0024_003C_003E8__locals1.localPlayer.Position;
					if (Vector3.DistanceSquared(position, CS_0024_003C_003E8__locals27.gatherPos) <= 12f && MathF.Abs(position.Y - CS_0024_003C_003E8__locals27.gatherPos.Y) <= 3f)
					{
						return true;
					}
					if (OmenServiceBase<MovementManager>.Instance().IsManagerBusy || !UIModuleExtension.IsScreenReady())
					{
						return false;
					}
					OmenServiceBase<MovementManager>.Instance().TPSmart_InZone(RaycastHelper.TryGetGroundPosition(CS_0024_003C_003E8__locals27.gatherPos, out var groundPosition) ? groundPosition : CS_0024_003C_003E8__locals27.gatherPos);
					return false;
				}, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--1470179789 ^ 0x99A16D1 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2d7d8a5df6744c51b35f486f03b53ded));
				CS_0024_003C_003E8__locals19._003C_003E4__this.TaskHelper.Enqueue(delegate
				{
					//IL_0016: Unknown result type (might be due to invalid IL or missing references)
					//IL_0021: Unknown result type (might be due to invalid IL or missing references)
					if (Addons.Gathering != null)
					{
						return true;
					}
					new EventStartPackt(GameObjectId.op_Implicit(CS_0024_003C_003E8__locals27.eventObjectID), EventId.op_Implicit(CS_0024_003C_003E8__locals27.eventID)).Send();
					return false;
				}, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489F1A5A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b2294f2f9bb24a76b00cf1af75888414));
			}
		});
	}

	private unsafe void nY6s6Go9WH()
	{
		_003C_003Ec__DisplayClass37_0 CS_0024_003C_003E8__locals5 = new _003C_003Ec__DisplayClass37_0();
		CS_0024_003C_003E8__locals5.item = config.ExchangeItem;
		if (!(CS_0024_003C_003E8__locals5.item == null))
		{
			base.TaskHelper.Enqueue(() => GameState.TerritoryType == 886 && !IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()) && Addons.HWDGathereInspect == null, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6C355920 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88a2dcd1996b4a3fbefaf51a98b59def));
			base.TaskHelper.Enqueue(delegate
			{
				new EventStartPackt(LocalPlayerState.EntityID, 721546u).Send();
			}, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4C428336 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4643f8ec0000417ba22db1f06f6757bd));
			base.TaskHelper.Enqueue(() => AddonSelectStringEvent.Select(CS_0024_003C_003E8__locals5.item.ScN35uSHTD()), LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2DBD5EF0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5d8d924f1edf4ec39f023b3c7bb646cc));
			base.TaskHelper.Enqueue(() => (*Addons.ShopExchangeCurrency).IsAddonAndNodesReady(), LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2310329F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a96fab9e97944b39a8afe3fb1109ba7d));
			base.TaskHelper.Enqueue((Func<bool>)CS_0024_003C_003E8__locals5.item.Ewx3RUfdK4, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4B0FA9C0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_8e002b0644ef421698ef6f63772929af), 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
			base.TaskHelper.Enqueue(() => CS_0024_003C_003E8__locals5.item.ka13eWGiU9() == 0, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5DD744E5 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c7355392ad9e43b6b82798cc42ea6c6f));
			base.TaskHelper.Enqueue(() => ((AtkUnitBase)Addons.ShopExchangeCurrency).Close(true), LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--367535148 ^ 0x50A64DB5 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_09da05f368544962acb62b42d9155467));
		}
	}

	private unsafe void wFdskD7uWO()
	{
		base.TaskHelper.Enqueue(() => GameState.TerritoryType == 886 && UIModuleExtension.IsScreenReady() && !IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()), LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5357B632 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_0676edd1b45a4ff096830fd6229bc1fb));
		base.TaskHelper.Enqueue(delegate
		{
			if ((*Addons.HWDGathereInspect).IsAddonAndNodesReady())
			{
				return true;
			}
			new EventStartPackt(LocalPlayerState.EntityID, 721556u).Send();
			return false;
		}, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6C3546AC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88a2dcd1996b4a3fbefaf51a98b59def));
		for (uint num = 0u; num < 4; num++)
		{
			_003C_003Ec__DisplayClass38_0 CS_0024_003C_003E8__locals2 = new _003C_003Ec__DisplayClass38_0();
			CS_0024_003C_003E8__locals2.index = num;
			base.TaskHelper.Enqueue(delegate
			{
				(*Addons.HWDGathereInspect).Callback(14, CS_0024_003C_003E8__locals2.index % 2);
			}, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x420CAC94 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_84ee025b19bb4145ba9b4b6b7acb91eb));
			base.TaskHelper.Enqueue(delegate
			{
				if (!((AtkComponentButton)((AtkUnitBase)Addons.HWDGathereInspect).GetComponentButtonById(8u)).IsEnabled)
				{
					return true;
				}
				((AgentId)371).SendEvent(1uL, 12);
				((AgentId)371).SendEvent(1uL, 11);
				return false;
			}, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2DBD4178 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5d8d924f1edf4ec39f023b3c7bb646cc));
		}
		base.TaskHelper.Enqueue(() => ((AtkUnitBase)Addons.HWDGathereInspect).Close(true), LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3987EF63 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_56ae6e3f2cdf4dd8a743f4d493ce49a5));
		if (!string.IsNullOrWhiteSpace(config.AutoDiscardGroupName) && ManagerBase<ModuleManager>.Instance().IsModuleEnabled(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--664844326 ^ 0x7EBCDEB3 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_20724779fecb4a91959b8028d04d1118)) == true)
		{
			base.TaskHelper.Enqueue(() => Addons.HWDGathereInspect == null && !IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()), LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x64E56992 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f53034f3efc64a309b10c7885bf5d9bd));
			base.TaskHelper.Enqueue(delegate
			{
				OmenServiceBase<ChatManager>.Instance().SendMessage(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D9C1267 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3badc14f3c904cf4809aa58db382c8ee) + config.AutoDiscardGroupName);
			}, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x423EDFA6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_6368ba44fa4749c09da09b5ebf853b71));
			base.TaskHelper.DelayNext(500, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x477E642D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_ba27244341a5414496a87eae29cf3dec));
			base.TaskHelper.Enqueue(delegate
			{
				IPCSubscriber<bool> isAutoDiscardModuleBusy = IsAutoDiscardModuleBusy;
				return isAutoDiscardModuleBusy == null || !isAutoDiscardModuleBusy;
			}, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5E3BA2FE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2d7d8a5df6744c51b35f486f03b53ded));
		}
	}

	private void Nw6s2mwgSI()
	{
		base.TaskHelper.Enqueue(() => !IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()), LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x12D6242C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7fcf5a71a7ed445197f132979519c560));
		base.TaskHelper.Enqueue(delegate
		{
			if (!Throttler.Shared.Throttle(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4B0F2A5E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_8e002b0644ef421698ef6f63772929af), 3000u))
			{
				return false;
			}
			if (GameState.TerritoryType == 939)
			{
				return true;
			}
			if (IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()) || !UIModuleExtension.IsScreenReady())
			{
				return false;
			}
			OmenServiceBase<ChatManager>.Instance().SendMessage(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x64E5F59A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f53034f3efc64a309b10c7885bf5d9bd));
			return false;
		}, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6FF19E42 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5859f4ea3042459db2c9b610b3099912));
		base.TaskHelper.Enqueue(() => UIModuleExtension.IsScreenReady(), LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x420CAC6C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_84ee025b19bb4145ba9b4b6b7acb91eb));
		base.TaskHelper.Enqueue((Action)rIlsudQLBS, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4CF25146 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a07aac30eb32484e94cb4d1e62117765), 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
	}

	private unsafe void vhrsZkGv1q()
	{
		base.TaskHelper.Enqueue(() => !((ActionManager)ActionManager.Instance()).IsActionOffCooldown((ActionType)1, 3u) || LocalPlayerState.HasStatus(50u, out var _) || ((ActionManager)ActionManager.Instance()).UseAction((ActionType)1, 3u, 3758096384uL, 0u, (UseActionMode)0, 0u, (bool*)null), LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xE09A3AB ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_aba6092d7cea44a49d1667540500e363));
	}

	private void mktsaGpdpf()
	{
		base.TaskHelper.Enqueue(delegate
		{
			if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4])
			{
				OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)5, 2u, 3758096384uL, 0u, (UseActionMode)0);
				iE7se5LIWF((ConditionFlag)4, value: true);
				return true;
			}
			if (IConditionExtension.get_IsCasting(DalamudServiceExtension.Instance<ICondition>()))
			{
				return false;
			}
			OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)5, 9u, 3758096384uL, 0u, (UseActionMode)0);
			return false;
		}, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3173A5F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_99910d0c1e594d99b604e0aab1195273));
	}

	private unsafe void SoNs0hMfWy()
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		_003C_003Ec__DisplayClass42_0 CS_0024_003C_003E8__locals5 = new _003C_003Ec__DisplayClass42_0();
		if (config.UseCordial && IP08j7SYp5(out CS_0024_003C_003E8__locals5.cordialItem, out var _))
		{
			base.TaskHelper.Enqueue(delegate
			{
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				//IL_002d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0038: Unknown result type (might be due to invalid IL or missing references)
				//IL_003d: Unknown result type (might be due to invalid IL or missing references)
				AgentInventoryContext* intPtr = AgentInventoryContext.Instance();
				InventoryItem value = CS_0024_003C_003E8__locals5.cordialItem.Value;
				uint itemId = ((InventoryItem)(ref value)).GetItemId();
				value = CS_0024_003C_003E8__locals5.cordialItem.Value;
				InventoryType inventoryType = ((InventoryItem)(ref value)).GetInventoryType();
				value = CS_0024_003C_003E8__locals5.cordialItem.Value;
				((AgentInventoryContext)intPtr).UseItem(itemId, inventoryType, (uint)((InventoryItem)(ref value)).GetSlot(), (short)0);
			}, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6E00D4E4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c40146824d9043e0906f4e1889cf39aa) + LuminaWrapper.GetItemName(CS_0024_003C_003E8__locals5.cordialItem.Value.ItemId));
			base.TaskHelper.DelayNext(1000, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5DD75A25 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c7355392ad9e43b6b82798cc42ea6c6f));
		}
	}

	private unsafe void Cxss5ZC0FX()
	{
		_003C_003Ec__DisplayClass43_0 CS_0024_003C_003E8__locals4 = new _003C_003Ec__DisplayClass43_0();
		CS_0024_003C_003E8__locals4.instance = RecommendEquipModule.Instance();
		base.TaskHelper.Enqueue(() => ((RecommendEquipModule)CS_0024_003C_003E8__locals4.instance).SetupForClassJob((byte)LocalPlayerState.ClassJob), LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489F867E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b2294f2f9bb24a76b00cf1af75888414));
		base.TaskHelper.Enqueue(delegate
		{
			if (((RecommendEquipModule)CS_0024_003C_003E8__locals4.instance).IsUpdating)
			{
				return false;
			}
			((RecommendEquipModule)CS_0024_003C_003E8__locals4.instance).EquipRecommendedGear();
			return true;
		}, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x23102C03 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a96fab9e97944b39a8afe3fb1109ba7d));
	}

	private void dlAsRiL5Ej()
	{
		if (GameState.TerritoryType == 939)
		{
			MQk8TZroB3();
			base.TaskHelper.Enqueue(() => GameState.TerritoryType == 886 && UIModuleExtension.IsScreenReady() && !IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()), LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xBA48336 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2edd08450a8b42e6997855e5217d6d55));
			Nw6s2mwgSI();
		}
	}

	private void zVKsDoVnur(uint zone)
	{
		if (isOnAutoGather)
		{
			vnavmeshIPC.StopPathfind();
		}
	}

	private unsafe void iE7se5LIWF(ConditionFlag flag, bool value)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Invalid comparison between Unknown and I4
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Invalid comparison between Unknown and I4
		if (!isOnAutoGather || GameState.TerritoryType != 939 || !JobInfos.ContainsKey(LocalPlayerState.ClassJob))
		{
			return;
		}
		if ((int)flag != 6)
		{
			if ((int)flag != 61)
			{
				return;
			}
			base.TaskHelper.Abort();
			if (value)
			{
				Xf68WqfRR8();
				base.TaskHelper.Enqueue(delegate
				{
					if (RaycastHelper.TryGetGroundPosition(LocalPlayerState.Object.Position, out var groundPosition))
					{
						OmenServiceBase<MovementManager>.Instance().TPSmart_InZone(groundPosition);
					}
					return false;
				}, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4B0FB792 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_8e002b0644ef421698ef6f63772929af));
			}
			else
			{
				base.TaskHelper.Abort();
				base.TaskHelper.Enqueue(() => PlayerController.Instance()->MoveControllerWalk.CurrentSpeed == 0f, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2CE5578D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c9dc773883f149b7befba3bdbeac55f7));
				base.TaskHelper.Enqueue((Action)rIlsudQLBS, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xB1ECCDE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_79d931aaf1c142a2a09f53e716260ad1), 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
			}
			return;
		}
		if (value)
		{
			SAA8t57991(createNew: true);
			return;
		}
		base.TaskHelper.Abort();
		SAA8t57991();
		if (config.Mode == GatheringMode.绿材)
		{
			dlAsRiL5Ej();
			return;
		}
		ushort expLevel = ((AgentHUD)AgentHUD.Instance()).ExpLevel;
		if (config.Mode == GatheringMode.练级 && lastLevel != expLevel)
		{
			Cxss5ZC0FX();
		}
		lastLevel = expLevel;
		base.TaskHelper.Enqueue((Action)rIlsudQLBS, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x258FE888 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_dadeb2ab61424d31be7cc1966e675825), 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
	}

	private void pOosNbuW6D(object? sender, ElapsedEventArgs e)
	{
		if (TimerLifetime.Receive((object)this, sender, (object?)operationWatchdogTimer, (ElapsedEventHandler)pOosNbuW6D, e, (Func<Action, Task>)DalamudServiceExtension.Instance<IFramework>().RunOnFrameworkThread) && isOnAutoGather && GameState.TerritoryType == 939 && JobInfos.ContainsKey(LocalPlayerState.ClassJob))
		{
			OmenServiceBase<NotifyHelper>.Instance().NotificationWarning(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x23102C9F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a96fab9e97944b39a8afe3fb1109ba7d)));
			Xf68WqfRR8();
			OmenServiceBase<ChatManager>.Instance().SendMessage(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5351E808 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_9085260e6b1a40438d219e85182b5d08));
		}
	}

	private unsafe void NdFs7xny57(AddonEvent type, AddonArgs args)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Invalid comparison between Unknown and I4
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Invalid comparison between Unknown and I4
		_003C_003Ec__DisplayClass51_0 CS_0024_003C_003E8__locals25 = new _003C_003Ec__DisplayClass51_0();
		CS_0024_003C_003E8__locals25._003C_003E4__this = this;
		if (!isOnAutoGather)
		{
			return;
		}
		AtkUnitBase* gathering = Addons.Gathering;
		if (gathering == null || GameState.TerritoryType != 939 || !JobInfos.TryGetValue(LocalPlayerState.ClassJob, out CS_0024_003C_003E8__locals25.jobInfo))
		{
			return;
		}
		if ((int)type != 1)
		{
			if ((int)type != 5 || !Throttler.Shared.Throttle(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3C14D1E9 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_d71d1e134fde413fa0bee3b2419181b6)))
			{
				return;
			}
			a6gjlC3r34NOLGhTHo0 a6gjlC3r34NOLGhTHo1 = a6gjlC3r34NOLGhTHo0.JGi3G2huHD(Addons.Gathering, this);
			if ((a6gjlC3r34NOLGhTHo1.MaxCount == a6gjlC3r34NOLGhTHo1.Count && Environment.TickCount64 - gatheringAddonFirstDisplayTime >= 15000) || Environment.TickCount64 - gatheringAddonFirstDisplayTime >= 45000)
			{
				OmenServiceBase<NotifyHelper>.Instance().NotificationWarning(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x469A5CA9 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_bb1eb496bad948e4a2baaa3e1edc7a92)));
				base.TaskHelper.Abort();
				OmenServiceBase<ChatManager>.Instance().SendMessage(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5CE34E33 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3054c6e6ef664be696bb2ad005864818));
			}
			else
			{
				if (base.TaskHelper.IsBusy || DalamudServiceExtension.Instance<ICondition>().Any((ConditionFlag[])(object)new ConditionFlag[1] { (ConditionFlag)42 }))
				{
					return;
				}
				AtkComponentCheckBox* component = (AtkComponentCheckBox*)((AtkComponentNode)((AtkUnitBase)gathering).GetComponentNodeById(31u)).Component;
				if (((AtkComponentCheckBox)component).IsEnabled && ((AtkComponentNode)((AtkComponentCheckBox)component).OwnerNode).IsVisible() && !((AtkComponentCheckBox)component).IsChecked)
				{
					(*Addons.Gathering).Callback(130, true);
					((AtkComponentCheckBox)component).SetChecked(true);
				}
				switch (a6gjlC3r34NOLGhTHo1.State)
				{
				case GatheringState.可以采集:
					if (!((AtkTextNode)((AtkUnitBase)gathering).GetTextNodeById(13u)).IsVisible())
					{
						((AtkValue)((AtkUnitBase)gathering).AtkValues).SetUInt(0u);
						(*Addons.Gathering).CallbackNoUpdate(a6gjlC3r34NOLGhTHo1.Index);
					}
					break;
				case GatheringState.无法采集:
					((AtkUnitBase)Addons.Gathering).Close(true);
					base.TaskHelper.Abort();
					base.TaskHelper.Enqueue(() => Addons.Gathering == null, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5CE34F59 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3054c6e6ef664be696bb2ad005864818));
					mktsaGpdpf();
					LBisJck0t6(RedirectPosition, isMarker: false);
					base.TaskHelper.Enqueue((Action)rIlsudQLBS, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x27D1CABF ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_39d5d8025f5249e6b489bc374eefbb3c), 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
					break;
				case GatheringState.采集完毕:
				case GatheringState.等待加载:
					break;
				}
			}
			return;
		}
		SAA8t57991();
		gatheringAddonFirstDisplayTime = Environment.TickCount64;
		Xf68WqfRR8();
		base.TaskHelper.Enqueue(() => (*Addons.Gathering).IsAddonAndNodesReady(), LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x1778D3F9 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f7cf93b006904393bb56d82c0a688fa1));
		base.TaskHelper.Enqueue(delegate
		{
			GatheringState state = a6gjlC3r34NOLGhTHo0.JGi3G2huHD(Addons.Gathering, CS_0024_003C_003E8__locals25._003C_003E4__this).State;
			return (state == GatheringState.可以采集 || state != GatheringState.等待加载) ? true : false;
		}, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x18CCD64C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_33babcee9ecc460a9e530039fd18d361));
		base.TaskHelper.Enqueue(delegate
		{
			BattleChara* localPlayer = Control.GetLocalPlayer();
			ushort gatheringPoints = ((BattleChara)localPlayer).GatheringPoints;
			ushort maxGatheringPoints = ((BattleChara)localPlayer).MaxGatheringPoints;
			a6gjlC3r34NOLGhTHo0 a6gjlC3r34NOLGhTHo2 = a6gjlC3r34NOLGhTHo0.JGi3G2huHD(Addons.Gathering, CS_0024_003C_003E8__locals25._003C_003E4__this);
			if (maxGatheringPoints >= 850)
			{
				if (gatheringPoints >= 850)
				{
					using (List<uint>.Enumerator enumerator = CS_0024_003C_003E8__locals25.jobInfo.ActionsAbove850.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							_003C_003Ec__DisplayClass51_1 CS_0024_003C_003E8__locals27 = new _003C_003Ec__DisplayClass51_1();
							CS_0024_003C_003E8__locals27.action = enumerator.Current;
							if (ActionManagerExtension.IsActionUnlocked(CS_0024_003C_003E8__locals27.action))
							{
								CS_0024_003C_003E8__locals25._003C_003E4__this.TaskHelper.Enqueue(() => !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)42] && OmenServiceBase<UseActionManager>.Instance().UseActionLocation((ActionType)1, CS_0024_003C_003E8__locals27.action, 3758096384uL, default(Vector3), 0u, 0), LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x76EB776F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b988ddaee02846b69dd4e446b5da9de9) + LuminaWrapper.GetActionName(CS_0024_003C_003E8__locals27.action));
							}
						}
						return;
					}
				}
				if (!CS_0024_003C_003E8__locals25._003C_003E4__this.IP08j7SYp5(out var _, out var _) && (!CS_0024_003C_003E8__locals25._003C_003E4__this.config.UseCordial || !cTK8Omm4j3()) && gatheringPoints + 6 * a6gjlC3r34NOLGhTHo2.Count + 50 >= maxGatheringPoints)
				{
					using (List<uint>.Enumerator enumerator = CS_0024_003C_003E8__locals25.jobInfo.ActionsSingle.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							_003C_003Ec__DisplayClass51_2 CS_0024_003C_003E8__locals28 = new _003C_003Ec__DisplayClass51_2();
							CS_0024_003C_003E8__locals28.actionSingle = enumerator.Current;
							if (ActionManagerExtension.IsActionUnlocked(CS_0024_003C_003E8__locals28.actionSingle))
							{
								CS_0024_003C_003E8__locals25._003C_003E4__this.TaskHelper.Enqueue(() => !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)42] && OmenServiceBase<UseActionManager>.Instance().UseActionLocation((ActionType)1, CS_0024_003C_003E8__locals28.actionSingle, 3758096384uL, default(Vector3), 0u, 0), LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x258F7474 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_dadeb2ab61424d31be7cc1966e675825) + LuminaWrapper.GetActionName(CS_0024_003C_003E8__locals28.actionSingle));
								break;
							}
						}
					}
				}
			}
			else if (gatheringPoints >= 100)
			{
				int num = gatheringPoints;
				using IEnumerator<uint> enumerator2 = CS_0024_003C_003E8__locals25.jobInfo.ActionsAbove850.Concat(CS_0024_003C_003E8__locals25.jobInfo.ActionsSingle).GetEnumerator();
				while (enumerator2.MoveNext())
				{
					_003C_003Ec__DisplayClass51_3 CS_0024_003C_003E8__locals29 = new _003C_003Ec__DisplayClass51_3();
					CS_0024_003C_003E8__locals29.actionSingle = enumerator2.Current;
					if (LuminaGetter.TryGetRow<Action>(CS_0024_003C_003E8__locals29.actionSingle, out Action item2) && ActionManagerExtension.IsActionUnlocked(CS_0024_003C_003E8__locals29.actionSingle) && num - ((Action)(ref item2)).PrimaryCostValue >= 0)
					{
						num -= ((Action)(ref item2)).PrimaryCostValue;
						CS_0024_003C_003E8__locals25._003C_003E4__this.TaskHelper.Enqueue(() => !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)42] && OmenServiceBase<UseActionManager>.Instance().UseActionLocation((ActionType)1, CS_0024_003C_003E8__locals29.actionSingle, 3758096384uL, default(Vector3), 0u, 0), LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6854003D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_23522a2a14314da28eb44187a09cd0b5) + LuminaWrapper.GetActionName(CS_0024_003C_003E8__locals29.actionSingle));
					}
				}
			}
		}, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xE09A139 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_aba6092d7cea44a49d1667540500e363));
	}

	private unsafe void mjmsdohhQn(AddonEvent type, AddonArgs args)
	{
		if (base.TaskHelper.IsBusy && GameState.TerritoryType == 886)
		{
			if ((*Addons.HWDGathereInspect).IsAddonAndNodesReady())
			{
				base.TaskHelper.Abort();
				((AtkUnitBase)Addons.HWDGathereInspect).Close(true);
				nY6s6Go9WH();
				wFdskD7uWO();
				Nw6s2mwgSI();
			}
			else if ((*Addons.ShopExchangeCurrency).IsAddonAndNodesReady())
			{
				AddonSelectYesnoEvent.ClickYes();
			}
		}
	}

	private void h3isIc8wFO()
	{
		if (isOnAutoGather && JobInfos.ContainsKey(LocalPlayerState.ClassJob))
		{
			base.TaskHelper.Abort();
			Nw6s2mwgSI();
		}
	}

	private unsafe void gOdsUMs2qF(IFramework framework)
	{
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		if (!isOnAutoGather || GameState.TerritoryType != 939 || (*Addons.Gathering).IsAddonAndNodesReady() || !UIModuleExtension.IsScreenReady() || LocalPlayerState.Object == null || ((*Addons.HWDAetherGauge).IsAddonAndNodesReady() && ((AtkValue)((byte*)((AtkUnitBase)Addons.HWDAetherGauge).AtkValues + Unsafe.SizeOf<AtkValue>())).Int / 200 > 0))
		{
			return;
		}
		ContentDirector* contentDirector = ((EventFramework)EventFramework.Instance()).GetContentDirector();
		if (contentDirector != null && ((ContentDirector)contentDirector).ContentTimeLeft <= (float)(10800 - config.RoundMinute * 60))
		{
			if (Throttler.Shared.Throttle(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5357AAAE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_0676edd1b45a4ff096830fd6229bc1fb), 30000u))
			{
				OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x36956A1D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_26e231ba68f74e8784503e099ddfc2d9), config.RoundMinute)));
			}
			MQk8TZroB3();
			wFdskD7uWO();
			nY6s6Go9WH();
			Nw6s2mwgSI();
		}
	}

	private unsafe void rIlsudQLBS()
	{
		base.TaskHelper.Abort();
		if (MeDsXgqaYc())
		{
			return;
		}
		((Window)base.Overlay).IsOpen = true;
		if ((*Addons.HWDAetherGauge).IsAddonAndNodesReady() && !IConditionExtension.get_IsOnMount(DalamudServiceExtension.Instance<ICondition>()) && useAetherAugerCount <= 2)
		{
			_003C_003Ec__DisplayClass55_0 CS_0024_003C_003E8__locals3 = new _003C_003Ec__DisplayClass55_0();
			if (((AtkValue)((byte*)((AtkUnitBase)Addons.HWDAetherGauge).AtkValues + Unsafe.SizeOf<AtkValue>())).Int / 200 > 0 && wUP8fhYn39(out CS_0024_003C_003E8__locals3.monster))
			{
				TargetManager.Target = CS_0024_003C_003E8__locals3.monster;
				useAetherAugerCount++;
				base.TaskHelper.Enqueue(() => OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)1, 19700u, CS_0024_003C_003E8__locals3.monster.EntityID, 0u, (UseActionMode)0), LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D9CE293 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3badc14f3c904cf4809aa58db382c8ee));
				base.TaskHelper.DelayNext(1500, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3AC935B2 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3932528062874ea4bb7e2e1185394e37));
				base.TaskHelper.Enqueue(() => !IConditionExtension.get_IsCasting(DalamudServiceExtension.Instance<ICondition>()), LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-864210836 ^ -526884259 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c9dc773883f149b7befba3bdbeac55f7));
				base.TaskHelper.DelayNext(1500, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6FF19A86 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5859f4ea3042459db2c9b610b3099912));
				base.TaskHelper.Enqueue((Action)rIlsudQLBS, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4753F084 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_790eb00575b3425495c924ac13469f4c), 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
				return;
			}
		}
		useAetherAugerCount = 0u;
		SoNs0hMfWy();
		switch (config.Mode)
		{
		case GatheringMode.绿材:
			if (MIdsYR7Jdg())
			{
				return;
			}
			break;
		case GatheringMode.练级:
		case GatheringMode.冲分:
			if (vMxsz66xst())
			{
				return;
			}
			break;
		default:
			Xf68WqfRR8();
			return;
		}
		isOnAutoGather = false;
		Xf68WqfRR8();
	}

	private bool MeDsXgqaYc()
	{
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		_003C_003Ec__DisplayClass56_0 CS_0024_003C_003E8__locals3 = new _003C_003Ec__DisplayClass56_0();
		if (!isOnAutoGather || GameState.TerritoryType != 939 || !JobInfos.TryGetValue(LocalPlayerState.ClassJob, out CS_0024_003C_003E8__locals3.jobInfo) || !DalamudServiceExtension.Instance().IsPluginEnabled(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x477E3901 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_ba27244341a5414496a87eae29cf3dec)))
		{
			Xf68WqfRR8();
			return true;
		}
		if (Inventories.Player.IsFull())
		{
			MQk8TZroB3();
			OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4CF255D8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a07aac30eb32484e94cb4d1e62117765))));
			return true;
		}
		if (!vnavmeshIPC.GetIsNavReady())
		{
			base.TaskHelper.DelayNext(1000, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x44277358 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8dfe57222f64fa68d9d16c8b6fe9d0b));
			base.TaskHelper.Enqueue((Action)rIlsudQLBS, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D757B8C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8b51f8019ce49018b38e06104a97fc1), 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
			return true;
		}
		if (IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()))
		{
			base.TaskHelper.DelayNext(1000, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6C3543B8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88a2dcd1996b4a3fbefaf51a98b59def));
			base.TaskHelper.Enqueue((Action)rIlsudQLBS, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3D56F5C2 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e9e296e241f44828a57c750102ce71c1), 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
			return true;
		}
		if (!LocalPlayerState.HasStatus(CS_0024_003C_003E8__locals3.jobInfo.BaseDetectStatus, out var _) && !KdA899cbTc(out Vector3 _, out IGameObject _, out GatheringNodeType _))
		{
			base.TaskHelper.Enqueue(() => OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)1, CS_0024_003C_003E8__locals3.jobInfo.BaseDetectAction, 3758096384uL, 0u, (UseActionMode)0), LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x9B4DD2E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2f1c0cb46b4440d499a69ed8afe22360));
			base.TaskHelper.DelayNext(1000, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3AC934EC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3932528062874ea4bb7e2e1185394e37));
			base.TaskHelper.Enqueue((Action)rIlsudQLBS, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x18CCD160 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_33babcee9ecc460a9e530039fd18d361), 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
			return true;
		}
		gOdsUMs2qF(DalamudServiceExtension.Instance<IFramework>());
		return false;
	}

	private unsafe bool MIdsYR7Jdg()
	{
		_003C_003Ec__DisplayClass57_0 CS_0024_003C_003E8__locals15 = new _003C_003Ec__DisplayClass57_0();
		uint classJob = LocalPlayerState.ClassJob;
		byte currentWeather = ((WeatherManager)WeatherManager.Instance()).GetCurrentWeather();
		bool num = SpecialWeathers.TryGetValue(currentWeather, out CS_0024_003C_003E8__locals15.targetJob);
		bool flag = LocalPlayerState.DistanceTo2DSquared(ZoneDefaultPosition.ToVector2()) < 2500f;
		if (!num)
		{
			if (!flag)
			{
				dlAsRiL5Ej();
				return true;
			}
			base.TaskHelper.DelayNext(10000, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6FF19B1E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5859f4ea3042459db2c9b610b3099912));
			base.TaskHelper.Enqueue((Action)rIlsudQLBS, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4B0FB3A0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_8e002b0644ef421698ef6f63772929af), 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
			return true;
		}
		if (classJob != CS_0024_003C_003E8__locals15.targetJob)
		{
			_003C_003Ec__DisplayClass57_1 CS_0024_003C_003E8__locals14 = new _003C_003Ec__DisplayClass57_1();
			CS_0024_003C_003E8__locals14.CS_0024_003C_003E8__locals1 = CS_0024_003C_003E8__locals15;
			if (LocalPlayerState.GetClassJobLevel(CS_0024_003C_003E8__locals14.CS_0024_003C_003E8__locals1.targetJob) >= 10 && LocalPlayerState.TryFindClassJobGearset(CS_0024_003C_003E8__locals14.CS_0024_003C_003E8__locals1.targetJob, out CS_0024_003C_003E8__locals14.targetJobGearset))
			{
				base.TaskHelper.Enqueue(delegate
				{
					if (LocalPlayerState.ClassJob == CS_0024_003C_003E8__locals14.CS_0024_003C_003E8__locals1.targetJob)
					{
						return true;
					}
					LocalPlayerState.SwitchGearset(CS_0024_003C_003E8__locals14.targetJobGearset);
					return false;
				}, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7EE7F2AB ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88432e96079448078bdf00e7b62f8ab6) + LuminaWrapper.GetJobName(CS_0024_003C_003E8__locals14.CS_0024_003C_003E8__locals1.targetJob));
				base.TaskHelper.Enqueue((Action)rIlsudQLBS, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3D56F502 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e9e296e241f44828a57c750102ce71c1), 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
				return true;
			}
			base.TaskHelper.DelayNext(10000, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4CF257BE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a07aac30eb32484e94cb4d1e62117765));
			base.TaskHelper.Enqueue((Action)rIlsudQLBS, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x27D1CC0B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_39d5d8025f5249e6b489bc374eefbb3c), 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
			return true;
		}
		CS_0024_003C_003E8__locals15.jobInfo = JobInfos[classJob];
		if (!YDa8Cue84J(out var position, out var type))
		{
			if (!flag && ((ActionManager)ActionManager.Instance()).IsActionOffCooldown((ActionType)1, CS_0024_003C_003E8__locals15.jobInfo.DetectPremiumAction))
			{
				base.TaskHelper.Enqueue(() => OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)1, CS_0024_003C_003E8__locals15.jobInfo.DetectPremiumAction, 3758096384uL, 0u, (UseActionMode)0), LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--1317373447 ^ 0x31961EE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3badc14f3c904cf4809aa58db382c8ee) + LuminaWrapper.GetActionName(CS_0024_003C_003E8__locals15.jobInfo.DetectPremiumAction));
				base.TaskHelper.Enqueue((Action)rIlsudQLBS, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x423ED9B0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_6368ba44fa4749c09da09b5ebf853b71), 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
				return true;
			}
			base.TaskHelper.DelayNext(1000, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3D56F656 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e9e296e241f44828a57c750102ce71c1));
			base.TaskHelper.Enqueue((Action)rIlsudQLBS, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6E00D33E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c40146824d9043e0906f4e1889cf39aa), 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
			return true;
		}
		if (type == GatheringNodeType.普通)
		{
			dlAsRiL5Ej();
			return true;
		}
		if (KdA899cbTc(out Vector3 position2, out IGameObject _, out GatheringNodeType type2) && type2 != GatheringNodeType.普通)
		{
			LBisJck0t6(position2, isMarker: false);
			ydisGH3OJ3();
			return true;
		}
		LBisJck0t6(position, isMarker: true);
		return true;
	}

	private unsafe bool vMxsz66xst()
	{
		_003C_003Ec__DisplayClass58_0 CS_0024_003C_003E8__locals24 = new _003C_003Ec__DisplayClass58_0();
		CS_0024_003C_003E8__locals24._003C_003E4__this = this;
		CS_0024_003C_003E8__locals24.currentJob = LocalPlayerState.ClassJob;
		uint key = JobInfos.FirstOrDefault((KeyValuePair<uint, OQSolR3ZF12HrPNe7VX> x) => x.Key != CS_0024_003C_003E8__locals24.currentJob).Key;
		_003C_003Ec__DisplayClass58_1 CS_0024_003C_003E8__locals20 = new _003C_003Ec__DisplayClass58_1();
		switch (config.Mode)
		{
		case GatheringMode.练级:
		{
			CS_0024_003C_003E8__locals20.currentJobLevel = LocalPlayerState.GetClassJobLevel(CS_0024_003C_003E8__locals24.currentJob);
			CS_0024_003C_003E8__locals20.otherJobLevel = LocalPlayerState.GetClassJobLevel(key);
			if (config.LevelThresholds.Count == 0)
			{
				return false;
			}
			uint num2 = config.LevelThresholds.Max();
			if (CS_0024_003C_003E8__locals20.currentJobLevel >= num2 && CS_0024_003C_003E8__locals20.otherJobLevel >= num2)
			{
				isOnAutoGather = false;
				MQk8TZroB3();
				return true;
			}
			uint num3 = config.LevelThresholds.Where((uint t) => CS_0024_003C_003E8__locals20.currentJobLevel >= t).DefaultIfEmpty(0u).Max();
			uint num4 = config.LevelThresholds.Where((uint t) => CS_0024_003C_003E8__locals20.otherJobLevel >= t).DefaultIfEmpty(0u).Max();
			if (num3 > num4)
			{
				return CS_0024_003C_003E8__locals24.sd4BxU8jNS(key);
			}
			break;
		}
		case GatheringMode.冲分:
		{
			if (config.IgnoreSkillPoints)
			{
				break;
			}
			OQSolR3ZF12HrPNe7VX oQSolR3ZF12HrPNe7VX = JobInfos[CS_0024_003C_003E8__locals24.currentJob];
			OQSolR3ZF12HrPNe7VX oQSolR3ZF12HrPNe7VX2 = JobInfos[key];
			if (!OmenServiceBase<AchievementManager>.Instance().TryGetAchievement(oQSolR3ZF12HrPNe7VX.Achievement500K, out AchievementInfo achievementInfo) || !OmenServiceBase<AchievementManager>.Instance().TryGetAchievement(oQSolR3ZF12HrPNe7VX2.Achievement500K, out AchievementInfo achievementInfo2))
			{
				base.TaskHelper.DelayNext(1000, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x36956F4F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_26e231ba68f74e8784503e099ddfc2d9));
				base.TaskHelper.Enqueue((Action)rIlsudQLBS, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D9CE673 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3badc14f3c904cf4809aa58db382c8ee), 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
				return true;
			}
			uint current = achievementInfo.Current;
			uint current2 = achievementInfo2.Current;
			bool num = current >= 400000;
			bool flag = current2 >= 400000;
			if (!num)
			{
				break;
			}
			if (flag)
			{
				bool isFinished = achievementInfo.IsFinished;
				bool isFinished2 = achievementInfo2.IsFinished;
				if (isFinished && isFinished2)
				{
					isOnAutoGather = false;
					MQk8TZroB3();
					return true;
				}
				if (!(!isFinished || isFinished2))
				{
					return CS_0024_003C_003E8__locals24.sd4BxU8jNS(key);
				}
			}
			else if (!flag)
			{
				return CS_0024_003C_003E8__locals24.sd4BxU8jNS(key);
			}
			break;
		}
		}
		CS_0024_003C_003E8__locals24.jobInfo = JobInfos[CS_0024_003C_003E8__locals24.currentJob];
		if (KdA899cbTc(out Vector3 position, out IGameObject _, out GatheringNodeType type))
		{
			if (lrB8KAr7VI(position) && !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4] && LocalPlayerState.DistanceTo3DSquared(position) >= 300f)
			{
				vhrsZkGv1q();
			}
			LBisJck0t6(position, isMarker: false);
			ydisGH3OJ3();
			return true;
		}
		if (YDa8Cue84J(out var position2, out type))
		{
			LBisJck0t6(position2, isMarker: true);
			return true;
		}
		if (!LocalPlayerState.HasStatus(CS_0024_003C_003E8__locals24.jobInfo.DetectStatus, out var _))
		{
			base.TaskHelper.Enqueue(() => LocalPlayerState.HasStatus(CS_0024_003C_003E8__locals24.jobInfo.DetectStatus, out var _) || ((ActionManager)ActionManager.Instance()).IsActionOffCooldown((ActionType)1, CS_0024_003C_003E8__locals24.jobInfo.DetectAction), LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--1976305562 ^ 0x31EB7734 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8dfe57222f64fa68d9d16c8b6fe9d0b) + LuminaWrapper.GetActionName(CS_0024_003C_003E8__locals24.jobInfo.DetectAction));
			base.TaskHelper.Enqueue(() => LocalPlayerState.HasStatus(CS_0024_003C_003E8__locals24.jobInfo.DetectStatus, out var _) || OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)1, CS_0024_003C_003E8__locals24.jobInfo.DetectAction, 3758096384uL, 0u, (UseActionMode)0), LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2194323A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cd1b319f326a44d1aaea645e95d8698b) + LuminaWrapper.GetActionName(CS_0024_003C_003E8__locals24.jobInfo.DetectAction));
			base.TaskHelper.DelayNext(1000, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-1487420814 ^ -479153484 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8dfe57222f64fa68d9d16c8b6fe9d0b));
			base.TaskHelper.Enqueue((Action)rIlsudQLBS, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x1C1709C5 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_accf3844f1c6464481df1849f0aee1bb), 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
			return true;
		}
		base.TaskHelper.DelayNext(1000, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x18CCD386 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_33babcee9ecc460a9e530039fd18d361));
		base.TaskHelper.Enqueue((Action)rIlsudQLBS, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x9B4DFDC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2f1c0cb46b4440d499a69ed8afe22360), 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
		return true;
	}

	protected unsafe override void OverlayUI()
	{
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_0479: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_047b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_051f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0524: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0541: Unknown result type (might be due to invalid IL or missing references)
		//IL_0546: Unknown result type (might be due to invalid IL or missing references)
		//IL_055c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0578: Unknown result type (might be due to invalid IL or missing references)
		//IL_057d: Unknown result type (might be due to invalid IL or missing references)
		//IL_05be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0603: Unknown result type (might be due to invalid IL or missing references)
		//IL_0409: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		bool flag = !isOnAutoGather;
		uint key;
		if (flag)
		{
			key = GameState.TerritoryType;
			bool flag2 = ((key == 886 || key == 939) ? true : false);
			flag = !flag2;
		}
		if (flag)
		{
			((Window)base.Overlay).IsOpen = false;
			return;
		}
		if (!GatheringModeLoc.TryGetValue(config.Mode, out (string, string) value))
		{
			((Window)base.Overlay).IsOpen = false;
			return;
		}
		using (OmenServiceBase<FontManager>.Instance().UIFont80.Push())
		{
			Jyh8ywW2Nl(isFromOverlay: true);
			ImGui.NewLine();
			using (ImRaiiExtension.Heading1(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7EE7F0AF ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88432e96079448078bdf00e7b62f8ab6)) + LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x50177EC4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e227921f7ee94b6aa1c28169395ba0cf) + value.Item1))
			{
				OQSolR3ZF12HrPNe7VX value2;
				switch (config.Mode)
				{
				case GatheringMode.练级:
				{
					TableDisposable val8 = ImRaii.Table(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x27D1CD93 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_39d5d8025f5249e6b489bc374eefbb3c)), 2);
					try
					{
						if (!TableDisposable.op_Implicit(val8))
						{
							break;
						}
						ImGui.TableSetupColumn(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6E00D210 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c40146824d9043e0906f4e1889cf39aa)), (ImGuiTableColumnFlags)4, 40f, 0u);
						ImGui.TableSetupColumn(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x109C0D0C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_44b9808560124617907f0048883b8948)), (ImGuiTableColumnFlags)4, 10f, 0u);
						ISharedImmediateTexture val9 = default(ISharedImmediateTexture);
						foreach (KeyValuePair<uint, OQSolR3ZF12HrPNe7VX> jobInfo in JobInfos)
						{
							jobInfo.Deconstruct(out key, out value2);
							uint num2 = key;
							if (LuminaGetter.TryGetRow<ClassJob>(num2, out ClassJob item3))
							{
								ITextureProvider obj3 = DalamudServiceExtension.Instance<ITextureProvider>();
								GameIconLookup val2 = GameIconLookup.op_Implicit(item3.GetIcon());
								if (obj3.TryGetFromGameIcon(ref val2, ref val9))
								{
									ImGui.TableNextRow();
									ImGui.TableNextColumn();
									ImGuiOm.TextImage(((object)((ClassJob)(ref item3)).Name/*cast due to constrained. prefix*/).ToString(), val9.GetWrapOrEmpty().Handle, new Vector2(ImGui.GetTextLineHeight()));
									ImGui.TableNextColumn();
									ImGui.AlignTextToFramePadding();
									ImU8String val10 = new ImU8String(0, 1);
									((ImU8String)(ref val10)).AppendFormatted<ushort>(LocalPlayerState.GetClassJobLevel(num2));
									ImGui.TextUnformatted(val10);
								}
							}
						}
						break;
					}
					finally
					{
						((TableDisposable)(ref val8)).Dispose();
					}
				}
				case GatheringMode.冲分:
				{
					TableDisposable val5 = ImRaii.Table(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xE09AB5F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_aba6092d7cea44a49d1667540500e363)), 2);
					try
					{
						if (!TableDisposable.op_Implicit(val5))
						{
							break;
						}
						ImGui.TableSetupColumn(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xF20212D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_fea4ad1b13de4c4bbc22f571ceda7372)), (ImGuiTableColumnFlags)4, 40f, 0u);
						ImGui.TableSetupColumn(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x44277F92 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8dfe57222f64fa68d9d16c8b6fe9d0b)), (ImGuiTableColumnFlags)4, 10f, 0u);
						ISharedImmediateTexture val6 = default(ISharedImmediateTexture);
						foreach (KeyValuePair<uint, OQSolR3ZF12HrPNe7VX> jobInfo2 in JobInfos)
						{
							jobInfo2.Deconstruct(out key, out value2);
							uint rowID = key;
							OQSolR3ZF12HrPNe7VX oQSolR3ZF12HrPNe7VX = value2;
							if (LuminaGetter.TryGetRow<ClassJob>(rowID, out ClassJob item2))
							{
								ITextureProvider obj2 = DalamudServiceExtension.Instance<ITextureProvider>();
								GameIconLookup val2 = GameIconLookup.op_Implicit(item2.GetIcon());
								if (obj2.TryGetFromGameIcon(ref val2, ref val6))
								{
									ImGui.TableNextRow();
									ImGui.TableNextColumn();
									ImGuiOm.TextImage(((object)((ClassJob)(ref item2)).Name/*cast due to constrained. prefix*/).ToString(), val6.GetWrapOrEmpty().Handle, new Vector2(ImGui.GetTextLineHeight()));
									AchievementInfo achievementInfo;
									uint num = (OmenServiceBase<AchievementManager>.Instance().TryGetAchievement(oQSolR3ZF12HrPNe7VX.Achievement500K, out achievementInfo) ? achievementInfo.Current : 0u);
									ImGui.TableNextColumn();
									ImGui.AlignTextToFramePadding();
									ImU8String val7 = new ImU8String(0, 1);
									((ImU8String)(ref val7)).AppendFormatted<uint>(num);
									ImGui.TextUnformatted(val7);
								}
							}
						}
						break;
					}
					finally
					{
						((TableDisposable)(ref val5)).Dispose();
					}
				}
				case GatheringMode.绿材:
				{
					InventoryManager* ptr = InventoryManager.Instance();
					TableDisposable val = ImRaii.Table(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6E00DDC6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c40146824d9043e0906f4e1889cf39aa)), 2);
					try
					{
						if (!TableDisposable.op_Implicit(val))
						{
							break;
						}
						ImGui.TableSetupColumn(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x27D1CDB5 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_39d5d8025f5249e6b489bc374eefbb3c)), (ImGuiTableColumnFlags)4, 40f, 0u);
						ImGui.TableSetupColumn(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x66B39CBC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_762f55d0e7ef4ecd8f3f55c0f8a10fae)), (ImGuiTableColumnFlags)4, 10f, 0u);
						ISharedImmediateTexture val3 = default(ISharedImmediateTexture);
						foreach (uint specialItem in SpecialItems)
						{
							if (!LuminaGetter.TryGetRow<Item>(specialItem, out Item item))
							{
								continue;
							}
							ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
							GameIconLookup val2 = new GameIconLookup((uint)((Item)(ref item)).Icon, false, true, (ClientLanguage?)null);
							if (obj.TryGetFromGameIcon(ref val2, ref val3))
							{
								ImGui.TableNextRow();
								ImGui.TableNextColumn();
								ImGuiOm.TextImage(((object)((Item)(ref item)).Name/*cast due to constrained. prefix*/).ToString(), val3.GetWrapOrEmpty().Handle, new Vector2(ImGui.GetTextLineHeight()));
								ImGui.TableNextColumn();
								ImU8String val4 = new ImU8String(3, 2);
								((ImU8String)(ref val4)).AppendFormatted<int>(((InventoryManager)ptr).GetInventoryItemCount(specialItem, false, true, true, (short)0));
								((ImU8String)(ref val4)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D750C74 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8b51f8019ce49018b38e06104a97fc1));
								((ImU8String)(ref val4)).AppendFormatted<uint>(specialItem);
								if (ImGui.Selectable(val4, false, (ImGuiSelectableFlags)2, default(Vector2)))
								{
									OmenServiceBase<ChatManager>.Instance().SendMessage(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x68AC5D62 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4722689f1a4d41dc9dc800bfe1a89c09) + ((object)((Item)(ref item)).Name/*cast due to constrained. prefix*/).ToString() + LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x258FA8C4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_dadeb2ab61424d31be7cc1966e675825));
								}
							}
						}
						break;
					}
					finally
					{
						((TableDisposable)(ref val)).Dispose();
					}
				}
				}
			}
		}
	}

	protected override void ConfigUI()
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0448: Unknown result type (might be due to invalid IL or missing references)
		//IL_049e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0472: Unknown result type (might be due to invalid IL or missing references)
		//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_062c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0671: Unknown result type (might be due to invalid IL or missing references)
		//IL_0676: Unknown result type (might be due to invalid IL or missing references)
		//IL_0681: Unknown result type (might be due to invalid IL or missing references)
		Q2L8niRgFh();
		ImGui.NewLine();
		Jyh8ywW2Nl();
		ImGui.NewLine();
		mqE8LQaVhb();
		ImGui.NewLine();
		ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
		ImU8String val = ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3987E76F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_56ae6e3f2cdf4dd8a743f4d493ce49a5)));
		ref uint roundMinute = ref config.RoundMinute;
		ImU8String val2 = default(ImU8String);
		if (ImGui.InputUInt(val, ref roundMinute, 0u, 0u, val2, (ImGuiInputTextFlags)0))
		{
			config.RoundMinute = Math.Clamp(config.RoundMinute, 5u, 175u);
		}
		if (ImGui.IsItemDeactivatedAfterEdit())
		{
			config.Save(this);
		}
		ImGuiOm.HelpMarker(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5CE34483 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3054c6e6ef664be696bb2ad005864818)), 40f, (FontAwesomeIcon)61530);
		ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
		ImGui.InputText(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6854957B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_23522a2a14314da28eb44187a09cd0b5))), ref config.AutoDiscardGroupName, 256, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
		if (ImGui.IsItemDeactivatedAfterEdit())
		{
			config.Save(this);
		}
		ImGuiOm.HelpMarker(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--664844326 ^ 0x7B43FD71 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3054c6e6ef664be696bb2ad005864818)), 40f, (FontAwesomeIcon)61530);
		ImGui.Spacing();
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--1317373447 ^ 0x61E466B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cade14e68bb84906959f3cda5c496848))), ref config.UseCordial))
		{
			config.Save(this);
		}
		ImGuiOm.HelpMarker(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xBA49514 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2edd08450a8b42e6997855e5217d6d55)), 40f, (FontAwesomeIcon)61530);
		ImGui.NewLine();
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x454E7C69 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_09da05f368544962acb62b42d9155467))));
		ImGuiOm.HelpMarker(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x591C6C93 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_20724779fecb4a91959b8028d04d1118)), 40f, (FontAwesomeIcon)61530);
		ComboDisposable val3 = ImRaii.Combo(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x44277D38 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8dfe57222f64fa68d9d16c8b6fe9d0b)), ImU8String.op_Implicit(config.ExchangeItem?.ItemName ?? string.Empty), (ImGuiComboFlags)8);
		try
		{
			if (ComboDisposable.op_Implicit(val3))
			{
				foreach (JNX6T03ar0VQa8XFdl3 exchangeItem in ExchangeItems)
				{
					ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
					Item value = LuminaGetter.GetRow<Item>(exchangeItem.ItemID).Value;
					GameIconLookup val4 = new GameIconLookup((uint)((Item)(ref value)).Icon, false, true, (ClientLanguage?)null);
					if (ImGuiOm.SelectableImageWithText(obj.GetFromGameIcon(ref val4).GetWrapOrEmpty().Handle, new Vector2(ImGui.GetTextLineHeightWithSpacing()), exchangeItem.ItemName, exchangeItem == config.ExchangeItem, (ImGuiSelectableFlags)0))
					{
						config.ExchangeItem = exchangeItem;
						config.Save(this);
					}
				}
			}
		}
		finally
		{
			((ComboDisposable)(ref val3)).Dispose();
		}
		ImGui.NewLine();
		vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--664844326 ^ 0xA1DF3BA ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5d8d924f1edf4ec39f023b3c7bb646cc))));
		ImGuiOm.HelpMarker(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6C354DA0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88a2dcd1996b4a3fbefaf51a98b59def)), 40f, (FontAwesomeIcon)61530);
		ImGui.SameLine(0f, 6f * Globals.GlobalUIScale);
		if (ImGui.SmallButton(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x36955995 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_26e231ba68f74e8784503e099ddfc2d9)))))
		{
			ImGui.OpenPopup(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xB1EC614 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_79d931aaf1c142a2a09f53e716260ad1)), (ImGuiPopupFlags)0);
		}
		PopupDisposable val5 = ImRaii.Popup(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x32DDDA5 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_86d75c161ced498ab6a767d2eb82d38e)));
		try
		{
			if (PopupDisposable.op_Implicit(val5))
			{
				ImGui.InputText(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x18CCDFB2 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_33babcee9ecc460a9e530039fd18d361)), ref addNewBlacklistMonsterInput, 512, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
				DisabledDisposable val6 = ImRaii.Disabled(string.IsNullOrWhiteSpace(addNewBlacklistMonsterInput) || config.BlacklistMonsters.Contains(addNewBlacklistMonsterInput));
				try
				{
					if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61543, LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3D56CAA0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e9e296e241f44828a57c750102ce71c1))))
					{
						config.BlacklistMonsters.Add(addNewBlacklistMonsterInput);
						config.Save(this);
						ImGui.CloseCurrentPopup();
						addNewBlacklistMonsterInput = string.Empty;
					}
				}
				finally
				{
					((IDisposable)val6)?.Dispose();
				}
			}
		}
		finally
		{
			((PopupDisposable)(ref val5)).Dispose();
		}
		foreach (string blacklistMonster in config.BlacklistMonsters)
		{
			val2 = new ImU8String(3, 2);
			((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7C2F3EB8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_12c2d68ba4654622a4959ee73cacc6d7)));
			((ImU8String)(ref val2)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D750C74 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8b51f8019ce49018b38e06104a97fc1));
			((ImU8String)(ref val2)).AppendFormatted<string>(blacklistMonster);
			if (ImGui.Button(val2, default(Vector2)))
			{
				config.BlacklistMonsters.Remove(blacklistMonster);
				config.Save(this);
			}
			ImGui.SameLine();
			ImGui.Bullet();
			ImGui.SameLine();
			ImU8String val7 = new ImU8String(0, 1);
			((ImU8String)(ref val7)).AppendFormatted<string>(blacklistMonster);
			ImGui.TextUnformatted(val7);
		}
	}

	private static void Q2L8niRgFh()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2DBD1C04 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5d8d924f1edf4ec39f023b3c7bb646cc))));
		IndentDisposable val = ImRaii.PushIndent(1, true);
		try
		{
			foreach (KeyValuePair<string, Func<bool>> item in PreCondition)
			{
				item.Deconstruct(out var key, out var value);
				string text = key;
				bool flag = value();
				vector = (flag ? KnownColor.GreenYellow.ToVector4() : KnownColor.Red.ToVector4());
				ImGui.TextColored(ref vector, ImU8String.op_Implicit(flag ? LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6E00883E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c40146824d9043e0906f4e1889cf39aa) : LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x32D8B07 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_86d75c161ced498ab6a767d2eb82d38e)));
				ImGui.SameLine();
				ImU8String val2 = new ImU8String(0, 1);
				((ImU8String)(ref val2)).AppendFormatted<string>(text);
				ImGui.TextUnformatted(val2);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private unsafe void Jyh8ywW2Nl(bool isFromOverlay = false)
	{
		using (ImRaiiExtension.Heading1(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xBA4A7C2 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2edd08450a8b42e6997855e5217d6d55))))
		{
			DisabledDisposable val = ImRaii.Disabled(isOnAutoGather);
			try
			{
				if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61515, LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5CE314AD ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3054c6e6ef664be696bb2ad005864818))))
				{
					isOnAutoGather = true;
					lastLevel = ((AgentHUD)AgentHUD.Instance()).ExpLevel;
					Nw6s2mwgSI();
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
			ImGui.SameLine();
			if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61517, LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-1487420814 ^ -2099743956 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_dadeb2ab61424d31be7cc1966e675825))))
			{
				Xf68WqfRR8();
				isOnAutoGather = false;
				lastLevel = 0u;
			}
			ImGui.SameLine();
			val = ImRaii.Disabled(GameState.TerritoryType != 886 || base.TaskHelper.IsBusy);
			try
			{
				if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)62306, LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x39516C6F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3aeae81f85f84848b273be7b2bfbf497))))
				{
					wFdskD7uWO();
					nY6s6Go9WH();
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
			if (isFromOverlay)
			{
				if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61459, LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xF202D31 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_fea4ad1b13de4c4bbc22f571ceda7372))))
				{
					OmenServiceBase<ChatManager>.Instance().SendCommand(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6C354D46 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88a2dcd1996b4a3fbefaf51a98b59def));
				}
				ImGui.SameLine();
				if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)62480, LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4B0FBA56 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_8e002b0644ef421698ef6f63772929af))))
				{
					((Window)base.Overlay).IsOpen = false;
				}
			}
		}
	}

	private void mqE8LQaVhb()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3172651 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_99910d0c1e594d99b604e0aab1195273))));
		IndentDisposable val = ImRaii.PushIndent(1, true);
		try
		{
			if (!GatheringModeLoc.TryGetValue(config.Mode, out (string, string) value))
			{
				return;
			}
			ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
			ComboDisposable val2 = ImRaii.Combo(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x1778DDE9 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f7cf93b006904393bb56d82c0a688fa1)), ImU8String.op_Implicit(value.Item1), (ImGuiComboFlags)8);
			try
			{
				if (ComboDisposable.op_Implicit(val2))
				{
					foreach (KeyValuePair<GatheringMode, (string, string)> item2 in GatheringModeLoc)
					{
						item2.Deconstruct(out var key, out var value2);
						(string, string) tuple = value2;
						GatheringMode gatheringMode = key;
						string item = tuple.Item1;
						object obj = tuple.Item2;
						ImU8String val3 = new ImU8String(2, 2);
						((ImU8String)(ref val3)).AppendFormatted<string>(item);
						((ImU8String)(ref val3)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3987A955 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_56ae6e3f2cdf4dd8a743f4d493ce49a5));
						((ImU8String)(ref val3)).AppendFormatted<GatheringMode>(gatheringMode);
						if (ImGui.Selectable(val3, false, (ImGuiSelectableFlags)0, default(Vector2)))
						{
							config.Mode = gatheringMode;
							config.Save(this);
						}
						if (obj == null)
						{
							obj = "";
						}
						ImGuiOm.TooltipHover((string)obj, 30f * Globals.GlobalUIScale);
					}
				}
			}
			finally
			{
				((ComboDisposable)(ref val2)).Dispose();
			}
			ImGui.NewLine();
			switch (config.Mode)
			{
			case GatheringMode.练级:
				OcK8HddliA();
				break;
			case GatheringMode.冲分:
				kxX8Vhqu5n();
				break;
			case GatheringMode.绿材:
				L4p8S26akd();
				break;
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void OcK8HddliA()
	{
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(3, 2);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x51E5A0D2 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a90a92322bff42e180982e106200d4a3)));
		((ImU8String)(ref val)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x109C791E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_44b9808560124617907f0048883b8948));
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x469A4369 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_bb1eb496bad948e4a2baaa3e1edc7a92)));
		((ImU8String)(ref val)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x45E43EC3 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1904f95d2a394c929e1a6d4f72943ad7));
		ImGui.TextColored(ref vector, val);
		ImGui.SameLine(0f, 6f * Globals.GlobalUIScale);
		if (ImGui.SmallButton(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5DD76B27 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c7355392ad9e43b6b82798cc42ea6c6f)))))
		{
			ImGui.OpenPopup(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4753F8B8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_790eb00575b3425495c924ac13469f4c)), (ImGuiPopupFlags)0);
		}
		PopupDisposable val2 = ImRaii.Popup(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5A7FAFB1 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5154ccd9fed0487f8fa0244531466009)));
		ImU8String val4;
		try
		{
			if (PopupDisposable.op_Implicit(val2))
			{
				ImGui.SetNextItemWidth(150f * Globals.GlobalUIScale);
				ImU8String val3 = ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3987E389 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_56ae6e3f2cdf4dd8a743f4d493ce49a5));
				ref uint reference = ref addNewLevelThresholdInput;
				val4 = default(ImU8String);
				ImGui.InputUInt(val3, ref reference, 0u, 0u, val4, (ImGuiInputTextFlags)0);
				DisabledDisposable val5 = ImRaii.Disabled(addNewLevelThresholdInput == 0);
				try
				{
					if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61452, LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x76EBD045 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b988ddaee02846b69dd4e446b5da9de9))) && !config.LevelThresholds.Contains(addNewLevelThresholdInput))
					{
						config.LevelThresholds.Add(addNewLevelThresholdInput);
						config.LevelThresholds.Sort();
						config.LevelThresholds = config.LevelThresholds.Distinct().ToList();
						config.Save(this);
						ImGui.CloseCurrentPopup();
						addNewLevelThresholdInput = 0u;
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
			((PopupDisposable)(ref val2)).Dispose();
		}
		ImGui.Spacing();
		bool flag = true;
		foreach (uint item in config.LevelThresholds.ToList())
		{
			if (!flag)
			{
				ImGui.SameLine();
			}
			flag = false;
			val4 = new ImU8String(0, 1);
			((ImU8String)(ref val4)).AppendFormatted<uint>(item);
			ImGui.TextUnformatted(val4);
			ImGui.SameLine();
			ImU8String val6 = new ImU8String(2, 2);
			((ImU8String)(ref val6)).AppendFormatted<string>(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)62189));
			((ImU8String)(ref val6)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x109C437C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_44b9808560124617907f0048883b8948));
			((ImU8String)(ref val6)).AppendFormatted<uint>(item);
			if (ImGui.SmallButton(val6))
			{
				config.LevelThresholds.Remove(item);
				config.LevelThresholds.Sort();
				config.Save(this);
			}
		}
	}

	private void L4p8S26akd()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--664844326 ^ 0x58784347 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1eab987c1db64b928220e27bd03141d3))));
		ImGuiOm.HelpMarker(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x36956491 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_26e231ba68f74e8784503e099ddfc2d9)), 40f, (FontAwesomeIcon)61530);
		string text = string.Join(',', config.BlacklistWeathers.Select(delegate(uint x)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			Weather? row = LuminaGetter.GetRow<Weather>(x);
			object obj2;
			if (!row.HasValue)
			{
				obj2 = null;
			}
			else
			{
				Weather valueOrDefault = row.GetValueOrDefault();
				obj2 = ((object)((Weather)(ref valueOrDefault)).Name/*cast due to constrained. prefix*/).ToString();
			}
			if (obj2 == null)
			{
				obj2 = string.Empty;
			}
			return (string)obj2;
		}).Distinct());
		ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
		ComboDisposable val = ImRaii.Combo(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x68AC1118 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4722689f1a4d41dc9dc800bfe1a89c09)), ImU8String.op_Implicit(text), (ImGuiComboFlags)8);
		try
		{
			if (ComboDisposable.op_Implicit(val))
			{
				foreach (var (num3, rowID) in SpecialWeathers)
				{
					if (!LuminaGetter.TryGetRow<Weather>(num3, out Weather item) || !LuminaGetter.TryGetRow<ClassJob>(rowID, out ClassJob item2))
					{
						continue;
					}
					ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
					GameIconLookup val2 = new GameIconLookup((uint)((Weather)(ref item)).Icon, false, true, (ClientLanguage?)null);
					if (ImGuiOm.SelectableImageWithText(obj.GetFromGameIcon(ref val2).GetWrapOrEmpty().Handle, new Vector2(ImGui.GetTextLineHeightWithSpacing()), ((object)((Weather)(ref item)).Name/*cast due to constrained. prefix*/).ToString() + LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x27D1C7E3 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_39d5d8025f5249e6b489bc374eefbb3c) + ((object)((ClassJob)(ref item2)).Name/*cast due to constrained. prefix*/).ToString() + LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x12D67DD0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7fcf5a71a7ed445197f132979519c560), config.BlacklistWeathers.Contains(num3), (ImGuiSelectableFlags)1))
					{
						if (!config.BlacklistWeathers.Add(num3))
						{
							config.BlacklistWeathers.Remove(num3);
						}
						config.Save(this);
					}
				}
			}
		}
		finally
		{
			((ComboDisposable)(ref val)).Dispose();
		}
		ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
		ImGui.InputText(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x50172A74 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e227921f7ee94b6aa1c28169395ba0cf))), ref config.ItemToGatherName, 256, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
		if (ImGui.IsItemDeactivatedAfterEdit())
		{
			config.Save(this);
		}
		ImGuiOm.HelpMarker(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xE09AEAF ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_aba6092d7cea44a49d1667540500e363)), 40f, (FontAwesomeIcon)61530);
	}

	private void kxX8Vhqu5n()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
		ImGui.InputText(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3AC93C0A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3932528062874ea4bb7e2e1185394e37))), ref config.ItemToGatherName, 256, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
		if (ImGui.IsItemDeactivatedAfterEdit())
		{
			config.Save(this);
		}
		ImGuiOm.HelpMarker(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5E3BAFAE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2d7d8a5df6744c51b35f486f03b53ded)), 40f, (FontAwesomeIcon)61530);
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(0, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x109C0B46 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_44b9808560124617907f0048883b8948)));
		if (ImGui.Checkbox(val, ref config.IgnoreSkillPoints))
		{
			config.Save(this);
		}
		ImGuiOm.HelpMarker(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x27D1C4DF ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_39d5d8025f5249e6b489bc374eefbb3c)), 40f, (FontAwesomeIcon)61530);
	}

	private unsafe bool KdA899cbTc(out Vector3 position, [NotNullWhen(true)] out IGameObject? obj, out GatheringNodeType type)
	{
		_003C_003Ec__DisplayClass67_0 CS_0024_003C_003E8__locals2 = new _003C_003Ec__DisplayClass67_0();
		position = Vector3.Zero;
		obj = null;
		type = GatheringNodeType.普通;
		if (!JobInfos.TryGetValue(LocalPlayerState.ClassJob, out CS_0024_003C_003E8__locals2.info))
		{
			return false;
		}
		List<IGameObject> list = IOmenDalamudService<IObjectTable>.Instance().SearchObjects((IGameObject x) => (int)x.ObjectKind == 6 && CS_0024_003C_003E8__locals2.info.GatherPointNames.Contains(x.Name.ToString()) && ((BattleChara)x.ToBCStruct()).TargetableStatus.IsSetAll<ObjectTargetableFlags>(new ReadOnlySpan<ObjectTargetableFlags>(Unsafe.AsPointer(ref global::_003CPrivateImplementationDetails_003E._8263A852B3877BAB05AE51AE58179841C08B22A25E0E964F5C157690B61F9B9C), 2)), IObjectTable.EventRange).ToList();
		if (list.Count == 0)
		{
			return false;
		}
		IGameObject gameObject = null;
		switch (config.Mode)
		{
		case GatheringMode.练级:
			gameObject = (from x in list
				where !CloudedGatherPointNames.Contains(x.Name.ToString())
				orderby LocalPlayerState.DistanceTo3DSquared(x.Position)
				select x).FirstOrDefault();
			break;
		case GatheringMode.冲分:
			gameObject = list.OrderBy((IGameObject x) => LocalPlayerState.DistanceTo3DSquared(x.Position)).FirstOrDefault();
			break;
		case GatheringMode.绿材:
			gameObject = (from x in list
				where CloudedGatherPointNames.Contains(x.Name.ToString())
				orderby LocalPlayerState.DistanceTo3DSquared(x.Position)
				select x).FirstOrDefault();
			break;
		}
		if (gameObject == null)
		{
			return false;
		}
		position = gameObject.Position;
		obj = gameObject;
		if (CloudedGatherPointNames.Contains(gameObject.Name))
		{
			type = ((LocalPlayerState.ClassJob != 16) ? GatheringNodeType.园艺梦幻 : GatheringNodeType.采矿梦幻);
		}
		else
		{
			type = GatheringNodeType.普通;
		}
		return true;
	}

	private unsafe bool wUP8fhYn39([NotNullWhen(true)] out IGameObject? gameObject)
	{
		_003C_003Ec__DisplayClass68_0 CS_0024_003C_003E8__locals6 = new _003C_003Ec__DisplayClass68_0();
		CS_0024_003C_003E8__locals6._003C_003E4__this = this;
		gameObject = null;
		CS_0024_003C_003E8__locals6.localPlayer = Control.GetLocalPlayer();
		if (CS_0024_003C_003E8__locals6.localPlayer == null)
		{
			return false;
		}
		var anon = (from x in IOmenDalamudService<IObjectTable>.Instance()
			where x != null && (int)x.ObjectKind == 2 && x.IsTargetable && !x.IsDead && !CS_0024_003C_003E8__locals6._003C_003E4__this.config.BlacklistMonsters.Contains(x.Name.ToString())
			select new
			{
				Object = x,
				Distance = Vector3.DistanceSquared(x.Position, Vector3.op_Implicit(((BattleChara)CS_0024_003C_003E8__locals6.localPlayer).Position))
			} into x
			where x.Distance <= 625f
			where ActionManager.GetActionInRangeOrLoS(19700u, (GameObject*)CS_0024_003C_003E8__locals6.localPlayer, x.Object.ToStruct()) != 562
			orderby x.Distance
			select x).FirstOrDefault();
		if (anon == null)
		{
			return false;
		}
		gameObject = anon.Object;
		return true;
	}

	private unsafe bool YDa8Cue84J(out Vector3 position, out GatheringNodeType type)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		type = GatheringNodeType.普通;
		position = Vector3.Zero;
		BattleChara* localPlayer = Control.GetLocalPlayer();
		if (localPlayer == null)
		{
			return false;
		}
		AgentMap* ptr = AgentMap.Instance();
		if (ptr == null)
		{
			return false;
		}
		MiniMapGatheringMarker val = ((AgentMap)ptr).MiniMapGatheringMarkers[0];
		MiniMapGatheringMarker val2 = ((AgentMap)ptr).MiniMapGatheringMarkers[1];
		MiniMapGatheringMarker val3 = ((AgentMap)ptr).MiniMapGatheringMarkers[2];
		if (val.ShouldRender == 0 && val2.ShouldRender == 0 && val3.ShouldRender == 0)
		{
			return false;
		}
		Vector2 zero = Vector2.Zero;
		if (config.Mode == GatheringMode.绿材)
		{
			if (val2.ShouldRender == 1 && LocalPlayerState.ClassJob == 16)
			{
				zero = new Vector2((float)val2.MapMarker.X / 16f, (float)val2.MapMarker.Y / 16f);
				type = GatheringNodeType.采矿梦幻;
			}
			else if (val3.ShouldRender == 1 && LocalPlayerState.ClassJob == 17)
			{
				zero = new Vector2((float)val3.MapMarker.X / 16f, (float)val3.MapMarker.Y / 16f);
				type = GatheringNodeType.园艺梦幻;
			}
			else
			{
				zero = new Vector2((float)val.MapMarker.X / 16f, (float)val.MapMarker.Y / 16f);
				type = GatheringNodeType.普通;
			}
		}
		else
		{
			zero = new Vector2((float)val.MapMarker.X / 16f, (float)val.MapMarker.Y / 16f);
			type = GatheringNodeType.普通;
		}
		if (zero == Vector2.Zero)
		{
			return false;
		}
		position = zero.ToVector3(in ((Vector3)(&((BattleChara)localPlayer).Position)).Y);
		return true;
	}

	private unsafe bool IP08j7SYp5([NotNullWhen(true)] out InventoryItem? item, out bool isHQ)
	{
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		item = null;
		isHQ = false;
		if (!config.UseCordial)
		{
			return false;
		}
		BattleChara* localPlayer = Control.GetLocalPlayer();
		if (localPlayer == null)
		{
			return false;
		}
		ushort gatheringPoints = ((BattleChara)localPlayer).GatheringPoints;
		ushort maxGatheringPoints = ((BattleChara)localPlayer).MaxGatheringPoints;
		if (gatheringPoints + 50 >= maxGatheringPoints)
		{
			return false;
		}
		using (Dictionary<uint, uint>.Enumerator enumerator = CordialItems.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				_003C_003Ec__DisplayClass70_0 CS_0024_003C_003E8__locals4 = new _003C_003Ec__DisplayClass70_0();
				uint num2;
				(CS_0024_003C_003E8__locals4.cordial, num2) = (KeyValuePair<uint, uint>)(ref enumerator.Current);
				if (((ActionManager)ActionManager.Instance()).IsActionOffCooldown((ActionType)2, CS_0024_003C_003E8__locals4.cordial) && ((IEnumerable<InventoryType>)Inventories.Player).TryGetItems((Predicate<InventoryItem>)((InventoryItem x) => ((InventoryItem)(ref x)).GetBaseItemId() == CS_0024_003C_003E8__locals4.cordial), out List<InventoryItem> itemResult))
				{
					itemResult = itemResult.OrderByDescending((InventoryItem x) => ((Enum)x.Flags).HasFlag((Enum)(object)(ItemFlags)1)).ToList();
					InventoryItem val = itemResult.First();
					bool flag = ((Enum)val.Flags).HasFlag((Enum)(object)(ItemFlags)1);
					if (((CS_0024_003C_003E8__locals4.cordial == 12699) ? num2 : (flag ? (num2 + 50) : num2)) + 50 + gatheringPoints < maxGatheringPoints)
					{
						item = val;
						isHQ = flag;
						return true;
					}
				}
			}
		}
		return false;
	}

	private unsafe static bool cTK8Omm4j3()
	{
		foreach (var (num3, _) in CordialItems)
		{
			if (((ActionManager)ActionManager.Instance()).IsActionOffCooldown((ActionType)2, num3))
			{
				return true;
			}
		}
		return false;
	}

	private unsafe static bool lrB8KAr7VI(Vector3 targetPos)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		BattleChara* localPlayer = Control.GetLocalPlayer();
		if (localPlayer == null)
		{
			return false;
		}
		if (MathF.Abs(targetPos.Y - ((Vector3)(&((BattleChara)localPlayer).Position)).Y) <= 5f)
		{
			return Vector3.DistanceSquared(Vector3.op_Implicit(((BattleChara)localPlayer).Position), targetPos) <= 900f;
		}
		return false;
	}

	private void SAA8t57991(bool createNew = false)
	{
		if (operationWatchdogTimer != null)
		{
			operationWatchdogTimer.Stop();
			operationWatchdogTimer.Dispose();
			operationWatchdogTimer = null;
		}
		if (createNew)
		{
			operationWatchdogTimer = new System.Timers.Timer(10000.0)
			{
				AutoReset = false
			};
			operationWatchdogTimer.Elapsed += pOosNbuW6D;
			operationWatchdogTimer.Start();
		}
	}

	private void Xf68WqfRR8()
	{
		base.TaskHelper.Abort();
		vnavmeshIPC.StopPathfind();
	}

	private void MQk8TZroB3()
	{
		base.TaskHelper.Abort();
		DutyCommand.Leave(DutyCommand.LeaveDutyKind.Inactive);
	}

	public AutoGatherDiadem()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7EE7F99F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88432e96079448078bdf00e7b62f8ab6)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x64E566EE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f53034f3efc64a309b10c7885bf5d9bd)),
			Category = ModuleCategory.Script,
			ModulesConflict = new string[3]
			{
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x109C0A46 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_44b9808560124617907f0048883b8948),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x66B39BC0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_762f55d0e7ef4ecd8f3f55c0f8a10fae),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x43D75382 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_974d377e1ed74986b81c34c95c2cc846)
			},
			ModulesPrerequisite = new string[8]
			{
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489BC66C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cade14e68bb84906959f3cda5c496848),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x420CACE8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_84ee025b19bb4145ba9b4b6b7acb91eb),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4753FB06 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_790eb00575b3425495c924ac13469f4c),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x50172832 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e227921f7ee94b6aa1c28169395ba0cf),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5CE3BC17 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3054c6e6ef664be696bb2ad005864818),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x68546DA3 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_23522a2a14314da28eb44187a09cd0b5),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4CF2A1FE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a07aac30eb32484e94cb4d1e62117765),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x68546DD9 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_23522a2a14314da28eb44187a09cd0b5)
			}
		};
		Permission = new ModulePermission
		{
			NeedAuth = true
		};
		addNewBlacklistMonsterInput = string.Empty;
		base._002Ector();
	}

	static AutoGatherDiadem()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		GatheringModeLoc = new Dictionary<GatheringMode, (string, string)>
		{
			[GatheringMode.练级] = (LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x53511A5A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_9085260e6b1a40438d219e85182b5d08)), LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x68ACECA6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4722689f1a4d41dc9dc800bfe1a89c09))),
			[GatheringMode.冲分] = (LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x32D27E9 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_86d75c161ced498ab6a767d2eb82d38e)), LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x27D13B25 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_39d5d8025f5249e6b489bc374eefbb3c))),
			[GatheringMode.绿材] = (LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7C2FFF54 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_12c2d68ba4654622a4959ee73cacc6d7)), LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xF20D4DD ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_fea4ad1b13de4c4bbc22f571ceda7372)))
		};
		ExchangeItems = JNX6T03ar0VQa8XFdl3.e8o30PPBsC();
		Dictionary<uint, OQSolR3ZF12HrPNe7VX> dictionary = new Dictionary<uint, OQSolR3ZF12HrPNe7VX>();
		OQSolR3ZF12HrPNe7VX value = new OQSolR3ZF12HrPNe7VX
		{
			ClassJob = 16u
		};
		int num = 4;
		List<uint> list = new List<uint>(num);
		CollectionsMarshal.SetCount(list, num);
		Span<uint> span = CollectionsMarshal.AsSpan(list);
		span[0] = 21177u;
		span[1] = 25589u;
		span[2] = 21203u;
		span[3] = 241u;
		value.ActionsAbove850 = list;
		num = 4;
		List<uint> list2 = new List<uint>(num);
		CollectionsMarshal.SetCount(list2, num);
		Span<uint> span2 = CollectionsMarshal.AsSpan(list2);
		span2[0] = 272u;
		span2[1] = 25589u;
		span2[2] = 4073u;
		span2[3] = 21177u;
		value.ActionsSingle = list2;
		value.DetectAction = 228u;
		value.DetectStatus = 234u;
		value.DetectPremiumAction = 291u;
		value.DetectPremiumStatus = 243u;
		value.BaseDetectAction = 227u;
		value.BaseDetectStatus = 225u;
		value.Achievement500K = 2515u;
		value.GatherPointNames = new string[12]
		{
			LuminaWrapper.GetGatheringPointName(1u),
			LuminaWrapper.GetGatheringPointName(2u),
			LuminaWrapper.GetGatheringPointName(5u),
			LuminaWrapper.GetGatheringPointName(6u),
			LuminaWrapper.GetGatheringPointName(9u),
			LuminaWrapper.GetGatheringPointName(10u),
			LuminaWrapper.GetGatheringPointName(13u),
			LuminaWrapper.GetGatheringPointName(14u),
			LuminaWrapper.GetGatheringPointName(17u),
			LuminaWrapper.GetGatheringPointName(18u),
			LuminaWrapper.GetGatheringPointName(23u),
			LuminaWrapper.GetGatheringPointName(24u)
		}.ToFrozenSet<string>(StringComparer.OrdinalIgnoreCase);
		dictionary[16u] = value;
		value = new OQSolR3ZF12HrPNe7VX
		{
			ClassJob = 17u
		};
		num = 4;
		List<uint> list3 = new List<uint>(num);
		CollectionsMarshal.SetCount(list3, num);
		Span<uint> span3 = CollectionsMarshal.AsSpan(list3);
		span3[0] = 21178u;
		span3[1] = 25590u;
		span3[2] = 21204u;
		span3[3] = 224u;
		value.ActionsAbove850 = list3;
		num = 4;
		List<uint> list4 = new List<uint>(num);
		CollectionsMarshal.SetCount(list4, num);
		Span<uint> span4 = CollectionsMarshal.AsSpan(list4);
		span4[0] = 273u;
		span4[1] = 25590u;
		span4[2] = 4087u;
		span4[3] = 21178u;
		value.ActionsSingle = list4;
		value.DetectAction = 211u;
		value.DetectStatus = 233u;
		value.DetectPremiumAction = 290u;
		value.DetectPremiumStatus = 242u;
		value.BaseDetectAction = 210u;
		value.BaseDetectStatus = 217u;
		value.Achievement500K = 2518u;
		value.GatherPointNames = new string[12]
		{
			LuminaWrapper.GetGatheringPointName(3u),
			LuminaWrapper.GetGatheringPointName(4u),
			LuminaWrapper.GetGatheringPointName(7u),
			LuminaWrapper.GetGatheringPointName(8u),
			LuminaWrapper.GetGatheringPointName(11u),
			LuminaWrapper.GetGatheringPointName(12u),
			LuminaWrapper.GetGatheringPointName(15u),
			LuminaWrapper.GetGatheringPointName(16u),
			LuminaWrapper.GetGatheringPointName(19u),
			LuminaWrapper.GetGatheringPointName(20u),
			LuminaWrapper.GetGatheringPointName(25u),
			LuminaWrapper.GetGatheringPointName(26u)
		}.ToFrozenSet<string>(StringComparer.OrdinalIgnoreCase);
		dictionary[17u] = value;
		JobInfos = dictionary;
		Teleporters = new H4jDMP37AHjU9HEf5ti[9]
		{
			new H4jDMP37AHjU9HEf5ti(new Vector3(-628.6f, 281f, -191.2f), new Vector3(-592.1f, 34f, -229.7f)),
			new H4jDMP37AHjU9HEf5ti(new Vector3(137.4f, -49.5f, -477f), new Vector3(174.8f, 295f, -737.5f)),
			new H4jDMP37AHjU9HEf5ti(new Vector3(434.9f, 290.4f, -735.5f), new Vector3(465.5f, 193f, -666f)),
			new H4jDMP37AHjU9HEf5ti(new Vector3(527f, 193f, -492.6f), new Vector3(602.4f, 252f, -411.1f)),
			new H4jDMP37AHjU9HEf5ti(new Vector3(634.6f, 252.1f, -388.3f), new Vector3(714.8f, 217.2f, -335.2f)),
			new H4jDMP37AHjU9HEf5ti(new Vector3(659.7f, 223.6f, -33.9f), new Vector3(466.4f, -24f, 185.3f)),
			new H4jDMP37AHjU9HEf5ti(new Vector3(339.1f, -4f, 471.9f), new Vector3(276f, 87f, 468.1f)),
			new H4jDMP37AHjU9HEf5ti(new Vector3(-145.5f, 113.8f, 519.8f), new Vector3(-288f, 318f, 559f)),
			new H4jDMP37AHjU9HEf5ti(new Vector3(-560.2f, 319f, 310f), new Vector3(-727.7f, 272.5f, -28f))
		};
		CordialItems = new Dictionary<uint, uint>
		{
			[12669u] = 400u,
			[6141u] = 300u,
			[16911u] = 150u
		};
		SpecialWeathers = new Dictionary<uint, uint>
		{
			[133u] = 16u,
			[134u] = 17u,
			[135u] = 16u,
			[136u] = 17u
		};
		num = 8;
		List<uint> list5 = new List<uint>(num);
		CollectionsMarshal.SetCount(list5, num);
		Span<uint> span5 = CollectionsMarshal.AsSpan(list5);
		span5[0] = 32001u;
		span5[1] = 32045u;
		span5[2] = 32002u;
		span5[3] = 32046u;
		span5[4] = 32003u;
		span5[5] = 32047u;
		span5[6] = 32004u;
		span5[7] = 32048u;
		SpecialItems = list5;
		CloudedGatherPointNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			LuminaWrapper.GetGatheringPointName(23u),
			LuminaWrapper.GetGatheringPointName(24u),
			LuminaWrapper.GetGatheringPointName(25u),
			LuminaWrapper.GetGatheringPointName(26u)
		};
		PreCondition = new Dictionary<string, Func<bool>>
		{
			[LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x68ACEE0E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4722689f1a4d41dc9dc800bfe1a89c09))] = () => JobInfos.ContainsKey(LocalPlayerState.ClassJob),
			[LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-1487420814 ^ -2075714151 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a96fab9e97944b39a8afe3fb1109ba7d))] = () => DalamudServiceExtension.Instance().IsPluginEnabled(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x76EBB7BB ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b988ddaee02846b69dd4e446b5da9de9))
		};
		RedirectPosition = new Vector3(468f, -23.7f, 213f);
		ZoneDefaultPosition = new Vector3(-656.5f, 285.3f, -157.4f);
	}

	[CompilerGenerated]
	private void xWE81AMbNG()
	{
		if (isOnAutoGather)
		{
			Xf68WqfRR8();
			Nw6s2mwgSI();
		}
	}

	[CompilerGenerated]
	private void tcK8M5Ukef()
	{
		if (isOnAutoGather)
		{
			Xf68WqfRR8();
			Nw6s2mwgSI();
		}
	}

	[CompilerGenerated]
	private void kmD841tmin()
	{
		OmenServiceBase<ChatManager>.Instance().SendMessage(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D9C1267 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3badc14f3c904cf4809aa58db382c8ee) + config.AutoDiscardGroupName);
	}

	[CompilerGenerated]
	private bool Q1v8sQpuKw()
	{
		IPCSubscriber<bool> isAutoDiscardModuleBusy = IsAutoDiscardModuleBusy;
		return isAutoDiscardModuleBusy == null || !isAutoDiscardModuleBusy;
	}

	[CompilerGenerated]
	private bool Kj488wMYvn()
	{
		if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4])
		{
			OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)5, 2u, 3758096384uL, 0u, (UseActionMode)0);
			iE7se5LIWF((ConditionFlag)4, value: true);
			return true;
		}
		if (IConditionExtension.get_IsCasting(DalamudServiceExtension.Instance<ICondition>()))
		{
			return false;
		}
		OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)5, 9u, 3758096384uL, 0u, (UseActionMode)0);
		return false;
	}
}
