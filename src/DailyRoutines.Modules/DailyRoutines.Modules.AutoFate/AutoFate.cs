using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DailyRoutines.Common.Extensions;
using DailyRoutines.Common.Interface.Windows;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Fates;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Interface;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Common.Math;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using Newtonsoft.Json;
using OmenTools;
using OmenTools.Dalamud;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.ImGuiOm.Widgets.Combos;
using OmenTools.Info.Game.AetheryteRecord;
using OmenTools.Info.Game.Data;
using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.AddonEvent;
using OmenTools.Interop.Game.ExecuteCommand.Implementations;
using OmenTools.Interop.Game.Helpers;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules.AutoFate;

public class AutoFate : ModuleBase
{
	private class Tts4FKBgnLI6N0adTwv : ModuleConfig
	{
		public string ActiveAttackCommand;

		public string DeactiveAttackCommand;

		public HashSet<uint> BlacklistedFates;

		public FateMode CurrentMode;

		public Tx5DWgBFgaOdVwt3xBP? ExchangeItem;

		public Vector3 ExchangePosition;

		public uint ExchangeThreshold;

		public uint ExchangeZone;

		public bool IgnoreFarLevelFates;

		public float LowHealthThreshold;

		public uint MainZone;

		public HashSet<uint> MultiZones;

		public uint SingleZone;

		public uint SubZone;

		public bool RestEnabled;

		public uint RestIntervalMinutes;

		public uint RestJitterPercent;

		public uint RestMinutes;

		public bool RestRandomZone;

		public uint RestZone;

		[JsonProperty("ItemPresets")]
		public List<CustomPreset> CustomPresets;

		[JsonProperty("SelectedItemPresetIndex")]
		public int SelectedCustomPresetIndex;

		public Tts4FKBgnLI6N0adTwv()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			ActiveAttackCommand = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489F192A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b2294f2f9bb24a76b00cf1af75888414);
			DeactiveAttackCommand = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x9B44646 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2f1c0cb46b4440d499a69ed8afe22360);
			BlacklistedFates = new HashSet<uint>();
			ExchangeThreshold = 1200u;
			IgnoreFarLevelFates = true;
			LowHealthThreshold = 20f;
			MultiZones = new HashSet<uint>();
			RestIntervalMinutes = 120u;
			RestJitterPercent = 5u;
			RestMinutes = 20u;
			RestRandomZone = true;
			CustomPresets = new List<CustomPreset>();
			base._002Ector();
		}
	}

	private readonly struct hsIAJpBwayeFO4nLLSg : IEquatable<hsIAJpBwayeFO4nLLSg>
	{
		public ZoneSwitchAction Action { get; init; }

		public uint Zone { get; init; }

		public hsIAJpBwayeFO4nLLSg(ZoneSwitchAction Action, uint Zone)
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			this.Action = Action;
			this.Zone = Zone;
		}

		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x53573788 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_0676edd1b45a4ff096830fd6229bc1fb));
			stringBuilder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x68AC8C9C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4722689f1a4d41dc9dc800bfe1a89c09));
			if (Y9iBEKajeB(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		[CompilerGenerated]
		private bool Y9iBEKajeB(StringBuilder builder)
		{
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x591CF8A7 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_20724779fecb4a91959b8028d04d1118));
			builder.Append(Action.ToString());
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7FD86699 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1eab987c1db64b928220e27bd03141d3));
			builder.Append(Zone.ToString());
			return true;
		}

		[CompilerGenerated]
		public static bool operator !=(hsIAJpBwayeFO4nLLSg left, hsIAJpBwayeFO4nLLSg right)
		{
			return !(left == right);
		}

		[CompilerGenerated]
		public static bool operator ==(hsIAJpBwayeFO4nLLSg left, hsIAJpBwayeFO4nLLSg right)
		{
			return left.Equals(right);
		}

		[CompilerGenerated]
		public override int GetHashCode()
		{
			return EqualityComparer<ZoneSwitchAction>.Default.GetHashCode(Action) * -1521134295 + EqualityComparer<uint>.Default.GetHashCode(Zone);
		}

		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			if (obj is hsIAJpBwayeFO4nLLSg)
			{
				return Equals((hsIAJpBwayeFO4nLLSg)obj);
			}
			return false;
		}

		[CompilerGenerated]
		public bool Equals(hsIAJpBwayeFO4nLLSg other)
		{
			if (EqualityComparer<ZoneSwitchAction>.Default.Equals(Action, other.Action))
			{
				return EqualityComparer<uint>.Default.Equals(Zone, other.Zone);
			}
			return false;
		}

		[CompilerGenerated]
		public void hkHBbmmRpb(out ZoneSwitchAction Action, out uint Zone)
		{
			Action = this.Action;
			Zone = this.Zone;
		}
	}

	private record Tx5DWgBFgaOdVwt3xBP
	{
		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass19_0
		{
			public uint itemID;

			public _003C_003Ec__DisplayClass19_0()
			{
				UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
				base._002Ector();
			}

			internal bool dkiPaJa11f(Tx5DWgBFgaOdVwt3xBP x)
			{
				return x.ItemID == itemID;
			}
		}

		public uint ShopID { get; init; }

		public string ItemName { get; init; }

		public uint ItemID { get; init; }

		public uint CostItemCount { get; init; }

		public Tx5DWgBFgaOdVwt3xBP(uint ShopID, string ItemName, uint ItemID, uint CostItemCount)
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			this.ShopID = ShopID;
			this.ItemName = ItemName;
			this.ItemID = ItemID;
			this.CostItemCount = CostItemCount;
			base._002Ector();
		}

		public static List<Tx5DWgBFgaOdVwt3xBP> dyHBqQwroc()
		{
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			List<uint> list = (from x in (IEnumerable<SpecialShop>)LuminaGetter.Get<SpecialShop>()
				where ((IEnumerable<ItemStruct>)(object)((SpecialShop)(ref x)).Item).Any((ItemStruct a) => ((IEnumerable<ItemCostsStruct>)(object)((ItemStruct)(ref a)).ItemCosts).Any((ItemCostsStruct b) => ((ItemCostsStruct)(ref b)).ItemCost.RowId == 26807))
				select ((SpecialShop)(ref x)).RowId).ToList();
			List<Tx5DWgBFgaOdVwt3xBP> list2 = new List<Tx5DWgBFgaOdVwt3xBP>();
			foreach (uint item in list)
			{
				SpecialShop valueOrDefault = LuminaGetter.GetRow<SpecialShop>(item).GetValueOrDefault();
				Enumerator<ItemStruct> enumerator2 = ((SpecialShop)(ref valueOrDefault)).Item.GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						ItemStruct current2 = enumerator2.Current;
						_003C_003Ec__DisplayClass19_0 CS_0024_003C_003E8__locals4 = new _003C_003Ec__DisplayClass19_0();
						ReceiveItemsStruct val = ((ItemStruct)(ref current2)).ReceiveItems[0];
						CS_0024_003C_003E8__locals4.itemID = ((ReceiveItemsStruct)(ref val)).Item.RowId;
						if (CS_0024_003C_003E8__locals4.itemID != 0)
						{
							ItemCostsStruct val2 = ((ItemStruct)(ref current2)).ItemCosts[0];
							uint currencyCost = ((ItemCostsStruct)(ref val2)).CurrencyCost;
							if (currencyCost != 0 && !list2.Any((Tx5DWgBFgaOdVwt3xBP x) => x.ItemID == CS_0024_003C_003E8__locals4.itemID))
							{
								val = ((ItemStruct)(ref current2)).ReceiveItems[0];
								Item value = ((ReceiveItemsStruct)(ref val)).Item.Value;
								list2.Add(new Tx5DWgBFgaOdVwt3xBP(item, ((object)((Item)(ref value)).Name/*cast due to constrained. prefix*/).ToString(), CS_0024_003C_003E8__locals4.itemID, currencyCost));
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

		public unsafe bool pIHBcbk2bB()
		{
			int num = XY6BQxDJjQ();
			if (num == 0)
			{
				return true;
			}
			int num2 = wIfBAmbbug();
			if (num2 == -1)
			{
				return false;
			}
			((AgentId)118).SendEvent(1uL, 0, num2, num, 0);
			return true;
		}

		public unsafe int wIfBAmbbug()
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

		public unsafe int XY6BQxDJjQ()
		{
			return (int)(((InventoryManager)InventoryManager.Instance()).GetInventoryItemCount(26807u, false, true, true, (short)0) / CostItemCount);
		}

		public override string ToString()
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(18, 2);
			defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x68540307 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_23522a2a14314da28eb44187a09cd0b5));
			defaultInterpolatedStringHandler.AppendFormatted(ShopID);
			defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7EE7A7B7 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88432e96079448078bdf00e7b62f8ab6));
			defaultInterpolatedStringHandler.AppendFormatted(ItemID);
			return defaultInterpolatedStringHandler.ToStringAndClear();
		}

		[CompilerGenerated]
		protected virtual bool PrintMembers(StringBuilder builder)
		{
			RuntimeHelpers.EnsureSufficientExecutionStack();
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x477EFF17 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_ba27244341a5414496a87eae29cf3dec));
			builder.Append(ShopID.ToString());
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x17784A7B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f7cf93b006904393bb56d82c0a688fa1));
			builder.Append((object?)ItemName);
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x398774C7 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_56ae6e3f2cdf4dd8a743f4d493ce49a5));
			builder.Append(ItemID.ToString());
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xF20BD2D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_fea4ad1b13de4c4bbc22f571ceda7372));
			builder.Append(CostItemCount.ToString());
			return true;
		}

		[CompilerGenerated]
		protected Tx5DWgBFgaOdVwt3xBP(Tx5DWgBFgaOdVwt3xBP original)
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
			ShopID = original.ShopID;
			ItemName = original.ItemName;
			ItemID = original.ItemID;
			CostItemCount = original.CostItemCount;
		}

		[CompilerGenerated]
		public void AVMBJBPkQX(out uint ShopID, out string ItemName, out uint ItemID, out uint CostItemCount)
		{
			ShopID = this.ShopID;
			ItemName = this.ItemName;
			ItemID = this.ItemID;
			CostItemCount = this.CostItemCount;
		}
	}

	public class CustomPreset : IEquatable<CustomPreset>
	{
		public string Name { get; set; }

		public Dictionary<uint, CustomZoneRule> Zones { get; set; }

		[JsonProperty("Data")]
		public Dictionary<uint, Dictionary<uint, uint>>? Data
		{
			set
			{
				if (value == null)
				{
					return;
				}
				foreach (var (key, dictionary2) in value)
				{
					Zones[key] = new CustomZoneRule
					{
						Items = new Dictionary<uint, uint>(dictionary2)
					};
				}
			}
		}

		public bool Equals(CustomPreset? other)
		{
			if (other == null)
			{
				return false;
			}
			if (this == other)
			{
				return true;
			}
			return Name == other.Name;
		}

		public override bool Equals(object? obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (this == obj)
			{
				return true;
			}
			if (obj.GetType() != GetType())
			{
				return false;
			}
			return Equals((CustomPreset)obj);
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}

		public CustomPreset()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			Name = string.Empty;
			Zones = new Dictionary<uint, CustomZoneRule>();
			base._002Ector();
		}
	}

	public class CustomZoneRule
	{
		public Dictionary<uint, uint> Items { get; set; }

		public uint RequiredFateCount { get; set; }

		public uint MinionID { get; set; }

		public string Macro { get; set; }

		public bool IsValid
		{
			get
			{
				if (RequiredFateCount == 0)
				{
					return Items.Count > 0;
				}
				return true;
			}
		}

		public CustomZoneRule()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			Items = new Dictionary<uint, uint>();
			Macro = string.Empty;
			base._002Ector();
		}
	}

	private class wQgpjiBrB2dwArZO9DQ : CustomPreset
	{
		public wQgpjiBrB2dwArZO9DQ(string nameKey)
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
			base.Name = LanguageManager.Get(nameKey);
		}
	}

	private sealed class IQGRQ4BGy50rFrjwhAw : IDisposable
	{
		private CancellationTokenSource? cts;

		private Task<List<Vector3>>? task;

		private Vector3 requestedTarget;

		private Vector3 navTarget;

		private bool hasCachedNavTarget;

		private Vector3 handedOffTarget;

		private long handoffTick;

		private Vector3 lastStartTarget;

		private long lastStartTick;

		private Vector3 pendingChangedTarget;

		private long pendingChangedSince;

		public Vector3 Target { get; private set; }

		public DateTime StartTime { get; private set; }

		public bool IsRunning
		{
			get
			{
				Task<List<Vector3>> task = this.task;
				if (task != null)
				{
					return !task.IsCompleted;
				}
				return false;
			}
		}

		public bool IsCompleted => task?.IsCompleted ?? false;

		public static bool oJtB66IZtM(Vector3 position)
		{
			if (position != Vector3.Zero)
			{
				return position.X != 0f || position.Z != 0f;
			}
			return false;
		}

		public Vector3 af3BkOmId7(Vector3 target)
		{
			if (hasCachedNavTarget && Vector3.DistanceSquared(requestedTarget, target) <= 9f)
			{
				return navTarget;
			}
			Vector3 vector = vnavmeshIPC.QueryNearestPointOnMesh(target, 10f, 10000f) ?? target;
			if (!oJtB66IZtM(vector))
			{
				vector = target;
			}
			requestedTarget = target;
			navTarget = vector;
			hasCachedNavTarget = true;
			return vector;
		}

		public bool fTbB2S9WYW(Vector3 target, long currentTick)
		{
			if (handedOffTarget != Vector3.Zero && Vector3.DistanceSquared(handedOffTarget, target) <= 9f)
			{
				if (vnavmeshIPC.GetIsPathfindRunning() || currentTick - handoffTick < 1000)
				{
					return true;
				}
				handedOffTarget = default(Vector3);
			}
			if (vnavmeshIPC.GetIsPathfindRunning())
			{
				List<Vector3> pathfindWayPoints = vnavmeshIPC.GetPathfindWayPoints();
				if (pathfindWayPoints != null && pathfindWayPoints.Count > 0 && !(Vector3.DistanceSquared(pathfindWayPoints.Last(), target) > 9f))
				{
					XrwB0kVBQO(target, currentTick);
					return true;
				}
			}
			return false;
		}

		public bool GggBZie02f(Vector3 target)
		{
			bool num = task != null && Vector3.DistanceSquared(Target, target) > 9f;
			bool flag = handedOffTarget != Vector3.Zero && Vector3.DistanceSquared(handedOffTarget, target) > 9f;
			if (!num && !flag)
			{
				return false;
			}
			NHhBRVwW04();
			handedOffTarget = default(Vector3);
			handoffTick = 0L;
			return true;
		}

		public bool lKNBaP79ti(Vector3 target, long currentTick)
		{
			Vector3 vector = ((task != null) ? Target : handedOffTarget);
			if (vector == Vector3.Zero || Vector3.DistanceSquared(vector, target) <= 9f)
			{
				pendingChangedTarget = default(Vector3);
				pendingChangedSince = 0L;
				return false;
			}
			if (pendingChangedTarget == Vector3.Zero || Vector3.DistanceSquared(pendingChangedTarget, target) > 9f)
			{
				pendingChangedTarget = target;
				pendingChangedSince = currentTick;
				return true;
			}
			return currentTick - pendingChangedSince < 1000;
		}

		public void XrwB0kVBQO(Vector3 target, long currentTick)
		{
			handedOffTarget = target;
			handoffTick = currentTick;
		}

		public List<Vector3>? vydB5jRvBu()
		{
			Task<List<Vector3>> task = this.task;
			if (task == null || !task.IsCompleted)
			{
				return null;
			}
			if (!this.task.IsCompletedSuccessfully)
			{
				NHhBRVwW04();
				return null;
			}
			List<Vector3> result = this.task.Result;
			NHhBRVwW04();
			return result;
		}

		public void NHhBRVwW04()
		{
			cts?.Cancel();
			cts?.Dispose();
			cts = null;
			task = null;
		}

		public bool btTBDXf0cZ(Vector3 target, DateTime startTime, long currentTick, Func<CancellationToken, Task<List<Vector3>>?> factory)
		{
			if (Vector3.DistanceSquared(lastStartTarget, target) <= 9f && currentTick - lastStartTick < 1000)
			{
				return false;
			}
			NHhBRVwW04();
			cts = new CancellationTokenSource();
			Target = target;
			StartTime = startTime;
			lastStartTarget = target;
			lastStartTick = currentTick;
			task = factory(cts.Token);
			return true;
		}

		public void nfiBe1Rra6()
		{
			NHhBRVwW04();
			Target = default(Vector3);
			StartTime = default(DateTime);
			requestedTarget = default(Vector3);
			navTarget = default(Vector3);
			hasCachedNavTarget = false;
			handedOffTarget = default(Vector3);
			handoffTick = 0L;
			pendingChangedTarget = default(Vector3);
			pendingChangedSince = 0L;
			lastStartTarget = default(Vector3);
			lastStartTick = 0L;
		}

		public void Dispose()
		{
			nfiBe1Rra6();
		}

		public IQGRQ4BGy50rFrjwhAw()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}
	}

	private enum DirectPathfindResult
	{
		Running,
		InRange,
		Invalid
	}

	private enum FateRule
	{
		Kill = 1,
		Collect
	}

	private enum FateMode
	{
		SingleZone,
		MainSubZone,
		MultiZone,
		Custom
	}

	private enum DutyEscapeStep
	{
		None,
		WaitingForEvent,
		WaitingToEnter,
		WaitingToExit,
		CorrectingPosition
	}

	private enum ExchangeStep
	{
		None,
		Moving,
		Interacting,
		Purchasing,
		Closing
	}

	private enum Phase
	{
		CombatEmergency,
		CollectComplete,
		PluginsOrModulesNotReady,
		ManagedMovement,
		InFate,
		FateEnding,
		Exchange,
		ZoneSwitch,
		SearchAndMove,
		Rest
	}

	private enum ZoneSwitchAction
	{
		Teleport,
		InstanceSwitch,
		Sanctuary
	}

	private sealed class uGeIvcBNLWfsqCPBrl2
	{
		private readonly Dictionary<uint, long> zoneLastVisitTicks;

		private readonly Dictionary<uint, uint> customFateCounts;

		private uint pendingEntryZone;

		public DutyEscapeStep DutyStep { get; set; }

		public ExchangeStep ExchangeStage { get; set; }

		public uint DutyEscapeZone { get; set; }

		public Vector3 DutyEscapePosition { get; set; }

		public long StepStartedTick { get; set; }

		public long ReviveResumeTick { get; set; }

		public Phase CurrentPhase { get; set; }

		public hsIAJpBwayeFO4nLLSg? PendingZoneSwitch { get; set; }

		public ulong AttackTargetID { get; set; }

		public uint PullTargetID { get; set; }

		public long NextRestTick { get; set; }

		public long RestUntilTick { get; set; }

		public long RestStartedTick { get; set; }

		public uint RestZone { get; set; }

		public bool RestDismounted { get; set; }

		public long o1FB76Xwho(uint zone)
		{
			return zoneLastVisitTicks.GetValueOrDefault(zone);
		}

		public uint GLZBdHAZpr(uint zone)
		{
			return customFateCounts.GetValueOrDefault(zone);
		}

		public void E1bBIfKAsA(uint zone)
		{
			customFateCounts[zone] = customFateCounts.GetValueOrDefault(zone) + 1;
		}

		public void nnbBUCfrhJ(uint zone)
		{
			pendingEntryZone = zone;
		}

		public bool iK7BupbpFK(out uint zone)
		{
			zone = pendingEntryZone;
			pendingEntryZone = 0u;
			return zone != 0;
		}

		public void f8LBXu2uHx(uint zone)
		{
			zoneLastVisitTicks[zone] = Environment.TickCount64;
		}

		public void nQQBYsju3h()
		{
			DutyStep = DutyEscapeStep.None;
			DutyEscapeZone = 0u;
			DutyEscapePosition = default(Vector3);
			StepStartedTick = 0L;
		}

		public void EjBBzEs8ev()
		{
			nQQBYsju3h();
			ExchangeStage = ExchangeStep.None;
		}

		public void C9qmnWPknt()
		{
			EjBBzEs8ev();
			zoneLastVisitTicks.Clear();
			customFateCounts.Clear();
			pendingEntryZone = 0u;
			ReviveResumeTick = 0L;
			CurrentPhase = Phase.CombatEmergency;
			PendingZoneSwitch = null;
			AttackTargetID = 0uL;
			PullTargetID = 0u;
			NextRestTick = 0L;
			RestUntilTick = 0L;
			RestStartedTick = 0L;
			RestZone = 0u;
			RestDismounted = false;
		}

		public uGeIvcBNLWfsqCPBrl2()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			zoneLastVisitTicks = new Dictionary<uint, long>();
			customFateCounts = new Dictionary<uint, uint>();
			base._002Ector();
		}
	}

	private class sJrcoAmyE6mj74ckFMl
	{
		public required uint FateID { get; init; }

		public required string Name { get; init; }

		public required Vector3 Position { get; set; }

		public required float Radius { get; set; }

		public required FateRule Rule { get; init; }

		public required bool IsSpecial { get; init; }

		public required uint MaxLevel { get; init; }

		public required uint Level { get; init; }

		public Vector3 StartNPCPosition { get; set; }

		public byte Progress { get; set; }

		public FateState GameState { get; set; }

		public long TimeRemaining { get; set; }

		public bool HasBonus { get; set; }

		public bool CanStart { get; set; }

		public long ParticipationStartedTimestamp { get; set; }

		public bool CompletionNotified { get; set; }

		public float Score { get; set; }

		public sJrcoAmyE6mj74ckFMl()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}
	}

	private class b90cEomL5Jl8rVYNdZJ
	{
		private readonly struct LEr52xP0DKuvOXoX5xJ : IEquatable<LEr52xP0DKuvOXoX5xJ>
		{
			public uint Progress { get; init; }

			public uint ItemCount { get; init; }

			public long StartedTick { get; init; }

			public LEr52xP0DKuvOXoX5xJ(uint Progress, uint ItemCount, long StartedTick)
			{
				UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
				this.Progress = Progress;
				this.ItemCount = ItemCount;
				this.StartedTick = StartedTick;
			}

			[CompilerGenerated]
			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x76EA9FFB ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b988ddaee02846b69dd4e446b5da9de9));
				stringBuilder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x1778414B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f7cf93b006904393bb56d82c0a688fa1));
				if (oFoP5KJgSl(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			[CompilerGenerated]
			private bool oFoP5KJgSl(StringBuilder builder)
			{
				builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5A7ED5E9 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5154ccd9fed0487f8fa0244531466009));
				builder.Append(Progress.ToString());
				builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489ABFF6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cade14e68bb84906959f3cda5c496848));
				builder.Append(ItemCount.ToString());
				builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x1C1678EB ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_accf3844f1c6464481df1849f0aee1bb));
				builder.Append(StartedTick.ToString());
				return true;
			}

			[CompilerGenerated]
			public static bool operator !=(LEr52xP0DKuvOXoX5xJ left, LEr52xP0DKuvOXoX5xJ right)
			{
				return !(left == right);
			}

			[CompilerGenerated]
			public static bool operator ==(LEr52xP0DKuvOXoX5xJ left, LEr52xP0DKuvOXoX5xJ right)
			{
				return left.Equals(right);
			}

			[CompilerGenerated]
			public override int GetHashCode()
			{
				return (EqualityComparer<uint>.Default.GetHashCode(Progress) * -1521134295 + EqualityComparer<uint>.Default.GetHashCode(ItemCount)) * -1521134295 + EqualityComparer<long>.Default.GetHashCode(StartedTick);
			}

			[CompilerGenerated]
			public override bool Equals(object obj)
			{
				if (obj is LEr52xP0DKuvOXoX5xJ)
				{
					return Equals((LEr52xP0DKuvOXoX5xJ)obj);
				}
				return false;
			}

			[CompilerGenerated]
			public bool Equals(LEr52xP0DKuvOXoX5xJ other)
			{
				if (EqualityComparer<uint>.Default.Equals(Progress, other.Progress) && EqualityComparer<uint>.Default.Equals(ItemCount, other.ItemCount))
				{
					return EqualityComparer<long>.Default.Equals(StartedTick, other.StartedTick);
				}
				return false;
			}

			[CompilerGenerated]
			public void PmfPRU2QU6(out uint Progress, out uint ItemCount, out long StartedTick)
			{
				Progress = this.Progress;
				ItemCount = this.ItemCount;
				StartedTick = this.StartedTick;
			}
		}

		private readonly Dictionary<uint, sJrcoAmyE6mj74ckFMl> trackedFates;

		private readonly HashSet<uint> gameFateIDs;

		private readonly List<uint> staleFateIDs;

		private uint currentFateID;

		private uint lastActiveFateID;

		private sJrcoAmyE6mj74ckFMl? bestFate;

		private LEr52xP0DKuvOXoX5xJ? pendingCollectSubmission;

		private uint endingThrottledID;

		public uint Zone { get; private set; }

		public uint Instance { get; private set; }

		public float CollectItemProgress { get; private set; }

		public uint FinishedCount { get; private set; }

		public string? CompletedFateName { get; private set; }

		public TimeSpan CompletedFateDuration { get; private set; }

		public bool IsCollectSubmitPending => pendingCollectSubmission.HasValue;

		public bool IsWaitingForFateRemoval
		{
			get
			{
				if (!awRmWMiyYo(currentFateID))
				{
					return awRmWMiyYo(lastActiveFateID);
				}
				return true;
			}
		}

		public void WvVmHkOR1f(uint zone, uint instance)
		{
			if (Zone != zone || Instance != instance)
			{
				pLjmSIgpim();
				Zone = zone;
				Instance = instance;
			}
		}

		public void pLjmSIgpim()
		{
			Zone = 0u;
			Instance = 0u;
			trackedFates.Clear();
			gameFateIDs.Clear();
			staleFateIDs.Clear();
			bestFate = null;
			FinishedCount = 0u;
			CompletedFateName = null;
			CompletedFateDuration = TimeSpan.Zero;
			currentFateID = 0u;
			lastActiveFateID = 0u;
			endingThrottledID = 0u;
			sL3mKwGPoV();
		}

		public unsafe bool DKDmVZ0ArM(FateContext* currentFate)
		{
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Invalid comparison between Unknown and I4
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0317: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0377: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ce: Invalid comparison between Unknown and I4
			//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d6: Invalid comparison between Unknown and I4
			CompletedFateName = null;
			CompletedFateDuration = TimeSpan.Zero;
			uint num = currentFateID;
			uint num2 = (uint)((currentFate != null) ? ((FateContext)currentFate).FateId : 0);
			if (num2 != currentFateID)
			{
				if (currentFateID != 0)
				{
					lastActiveFateID = (trackedFates.ContainsKey(currentFateID) ? currentFateID : 0u);
				}
				currentFateID = num2;
				endingThrottledID = 0u;
				sL3mKwGPoV();
			}
			gameFateIDs.Clear();
			sJrcoAmyE6mj74ckFMl value2;
			uint key;
			foreach (IFate item2 in (IEnumerable<IFate>)DalamudServiceExtension.Instance<IFateTable>())
			{
				gameFateIDs.Add(item2.FateId);
				bool canStart = false;
				if ((int)item2.State == 3)
				{
					FateContext* fateById = ((FateManager)FateManager.Instance()).GetFateById(item2.FateId);
					bool flag = fateById != null;
					if (flag)
					{
						uint motivationNpc = ((FateContext)fateById).MotivationNpc;
						bool flag2 = ((motivationNpc == 0 || motivationNpc == 3758096384u) ? true : false);
						flag = !flag2;
					}
					canStart = flag;
				}
				Fate item;
				if (trackedFates.TryGetValue(item2.FateId, out sJrcoAmyE6mj74ckFMl value))
				{
					bool flag3 = WINmTA1QGv(value);
					if (item2.FateId == currentFateID && value.ParticipationStartedTimestamp == 0L && !flag3)
					{
						value.ParticipationStartedTimestamp = Stopwatch.GetTimestamp();
					}
					if (IQGRQ4BGy50rFrjwhAw.oJtB66IZtM(item2.Position))
					{
						value.Position = item2.Position;
						value.Radius = item2.Radius;
					}
					value.Progress = item2.Progress;
					value.GameState = item2.State;
					value.TimeRemaining = item2.TimeRemaining;
					value.HasBonus = item2.HasBonus;
					value.CanStart = canStart;
					if ((item2.FateId == currentFateID || item2.FateId == lastActiveFateID) && value.ParticipationStartedTimestamp != 0L && !value.CompletionNotified && !flag3 && WINmTA1QGv(value))
					{
						value.CompletionNotified = true;
						CompletedFateName = value.Name;
						CompletedFateDuration = Stopwatch.GetElapsedTime(value.ParticipationStartedTimestamp);
					}
				}
				else if (IQGRQ4BGy50rFrjwhAw.oJtB66IZtM(item2.Position) && LuminaGetter.TryGetRow<Fate>((uint)item2.FateId, out item))
				{
					Dictionary<uint, sJrcoAmyE6mj74ckFMl> dictionary = trackedFates;
					uint motivationNpc = item2.FateId;
					value2 = new sJrcoAmyE6mj74ckFMl
					{
						FateID = item2.FateId,
						Name = ((object)item2.Name).ToString(),
						Position = item2.Position,
						Radius = item2.Radius
					};
					sJrcoAmyE6mj74ckFMl obj = value2;
					FateRule rule = ((((Fate)(ref item)).Rule != 2 || ((Fate)(ref item)).EventItem.RowId == 0) ? FateRule.Kill : FateRule.Collect);
					obj.Rule = rule;
					sJrcoAmyE6mj74ckFMl sJrcoAmyE6mj74ckFMl2 = value2;
					key = ((Fate)(ref item)).Music.RowId;
					bool flag = key - 153 <= 1;
					sJrcoAmyE6mj74ckFMl2.IsSpecial = flag;
					value2.MaxLevel = item2.MaxLevel;
					value2.Level = item2.Level;
					value2.Progress = item2.Progress;
					value2.GameState = item2.State;
					value2.TimeRemaining = item2.TimeRemaining;
					value2.HasBonus = item2.HasBonus;
					value2.CanStart = canStart;
					sJrcoAmyE6mj74ckFMl sJrcoAmyE6mj74ckFMl3 = value2;
					bool flag2 = item2.FateId == currentFateID;
					if (flag2)
					{
						FateState state = item2.State;
						bool flag4 = (((int)state == 5 || (int)state == 7) ? true : false);
						flag2 = !flag4;
					}
					sJrcoAmyE6mj74ckFMl3.ParticipationStartedTimestamp = ((flag2 && item2.Progress < 100) ? Stopwatch.GetTimestamp() : 0);
					value2.Score = 0f;
					dictionary[motivationNpc] = value2;
				}
			}
			staleFateIDs.Clear();
			foreach (KeyValuePair<uint, sJrcoAmyE6mj74ckFMl> trackedFate in trackedFates)
			{
				trackedFate.Deconstruct(out key, out value2);
				uint num3 = key;
				sJrcoAmyE6mj74ckFMl fate = value2;
				if (!gameFateIDs.Contains(num3))
				{
					if ((num3 == currentFateID || num3 == lastActiveFateID) && WINmTA1QGv(fate))
					{
						key = FinishedCount++;
						sL3mKwGPoV();
					}
					staleFateIDs.Add(num3);
				}
			}
			foreach (uint staleFateID in staleFateIDs)
			{
				sJrcoAmyE6mj74ckFMl? obj2 = bestFate;
				if (obj2 != null && obj2.FateID == staleFateID)
				{
					bestFate = null;
				}
				trackedFates.Remove(staleFateID);
				if (lastActiveFateID == staleFateID)
				{
					lastActiveFateID = 0u;
				}
			}
			if (currentFate != null)
			{
				LEr52xP0DKuvOXoX5xJ? lEr52xP0DKuvOXoX5xJ = pendingCollectSubmission;
				if (lEr52xP0DKuvOXoX5xJ.HasValue)
				{
					LEr52xP0DKuvOXoX5xJ valueOrDefault = lEr52xP0DKuvOXoX5xJ.GetValueOrDefault();
					if (!IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()))
					{
						if (((FateContext)currentFate).Progress > valueOrDefault.Progress)
						{
							uint num4 = ((FateContext)currentFate).Progress - valueOrDefault.Progress;
							CollectItemProgress = (float)num4 / (float)valueOrDefault.ItemCount;
							pendingCollectSubmission = null;
						}
						else if (valueOrDefault.Progress >= 100 && LocalPlayerState.GetItemCount(((FateContext)currentFate).EventItem) < valueOrDefault.ItemCount)
						{
							pendingCollectSubmission = null;
						}
						else if (Environment.TickCount64 - valueOrDefault.StartedTick >= 2000)
						{
							pendingCollectSubmission = null;
						}
					}
				}
			}
			if (num != 0)
			{
				return num2 == 0;
			}
			return false;
		}

		public void wGjm9k88c2(Vector3 playerPos, uint playerLevel, HashSet<uint> blacklist, bool ignoreFarLevelFates)
		{
			bestFate = null;
			foreach (sJrcoAmyE6mj74ckFMl value in trackedFates.Values)
			{
				value.Score = rykmtZNsa6(value, playerPos, playerLevel, blacklist, ignoreFarLevelFates);
				if (value.Score != 0f && (bestFate == null || value.Score > bestFate.Score))
				{
					bestFate = value;
				}
			}
		}

		public bool VnDmfLR0qH([NotNullWhen(true)] out sJrcoAmyE6mj74ckFMl? fate)
		{
			return (fate = bestFate) != null;
		}

		public void q3BmCsM1m5(uint progress, uint itemCount)
		{
			if (itemCount != 0)
			{
				pendingCollectSubmission = new LEr52xP0DKuvOXoX5xJ(progress, itemCount, Environment.TickCount64);
			}
		}

		public uint xu6mj0FRwF(uint currentProgress)
		{
			if (CollectItemProgress > 0f)
			{
				uint num = 100 - currentProgress;
				if (num == 0)
				{
					return 1u;
				}
				uint val = (uint)Math.Ceiling((float)num / CollectItemProgress);
				return Math.Max(1u, Math.Min(val, 20u) - 1);
			}
			return 2u;
		}

		public bool cjjmOjbBnc(uint fateID)
		{
			if (endingThrottledID == fateID)
			{
				return false;
			}
			endingThrottledID = fateID;
			return true;
		}

		public void sL3mKwGPoV()
		{
			CollectItemProgress = 0f;
			pendingCollectSubmission = null;
		}

		private static float rykmtZNsa6(sJrcoAmyE6mj74ckFMl fate, Vector3 playerPos, uint playerLevel, HashSet<uint> blacklist, bool ignoreFarLevelFates)
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Invalid comparison between Unknown and I4
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Invalid comparison between Unknown and I4
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Invalid comparison between Unknown and I4
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Invalid comparison between Unknown and I4
			if (blacklist.Contains(fate.FateID))
			{
				return 0f;
			}
			if (ignoreFarLevelFates && fate.Level > playerLevel + 8)
			{
				return 0f;
			}
			FateState gameState = fate.GameState;
			if (((int)gameState == 5 || gameState - 7 <= 1) ? true : false)
			{
				return 0f;
			}
			if (fate != null && (int)fate.GameState == 3 && !fate.CanStart)
			{
				return 0f;
			}
			if (!IQGRQ4BGy50rFrjwhAw.oJtB66IZtM(fate.Position))
			{
				return 0f;
			}
			if (fate.Progress >= 80)
			{
				return 0f;
			}
			if (fate != null && fate.IsSpecial && fate.Progress <= 20)
			{
				return 0f;
			}
			float num = Vector3.Distance(playerPos, fate.Position);
			if ((int)fate.GameState != 3)
			{
				float num2 = num / 12f;
				float num3 = ((fate.Rule != FateRule.Collect) ? 60f : 120f);
				float num4 = num3;
				float num5 = num2 + num4 + 30f;
				if ((float)fate.TimeRemaining < 180f)
				{
					return 0f;
				}
				if ((float)fate.TimeRemaining < num5)
				{
					return 0f;
				}
			}
			float num6 = 0f;
			num6 += 500f;
			if (fate.HasBonus)
			{
				num6 += 300f;
			}
			if (fate.MaxLevel >= playerLevel)
			{
				num6 += 300f;
			}
			num6 -= num * 0.5f;
			return num6 - (float)(int)fate.Progress * 10f;
		}

		private bool awRmWMiyYo(uint fateID)
		{
			if (fateID != 0 && trackedFates.TryGetValue(fateID, out sJrcoAmyE6mj74ckFMl value))
			{
				return WINmTA1QGv(value);
			}
			return false;
		}

		private static bool WINmTA1QGv(sJrcoAmyE6mj74ckFMl fate)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Invalid comparison between Unknown and I4
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Invalid comparison between Unknown and I4
			FateState gameState = fate.GameState;
			if (((int)gameState != 5 && (int)gameState != 7) || 1 == 0)
			{
				return fate.Progress == 100;
			}
			return true;
		}

		public b90cEomL5Jl8rVYNdZJ()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			trackedFates = new Dictionary<uint, sJrcoAmyE6mj74ckFMl>();
			gameFateIDs = new HashSet<uint>();
			staleFateIDs = new List<uint>();
			base._002Ector();
		}
	}

	private sealed class mEuFRom1N7yRwDnCHna
	{
		public string ExchangeItemSearchInput;

		public FateSelectCombo FateCombo { get; }

		public ZoneSelectCombo SingleZoneCombo { get; }

		public ZoneSelectCombo MainZoneCombo { get; }

		public ZoneSelectCombo SubZoneCombo { get; }

		public ZoneSelectCombo MultiZoneCombo { get; }

		public ZoneSelectCombo ExchangeZoneCombo { get; }

		public ZoneSelectCombo RestZoneCombo { get; }

		public ZoneSelectCombo CustomModeZoneCombo { get; }

		public ItemSelectCombo CustomModeItemCombo { get; }

		public CompanionCombo CustomModeCompanionCombo { get; }

		public int LastSelectedCustomPresetZoneIndex { get; set; }

		public uint SelectedCustomZone { get; set; }

		public uint CachedBicolorGemstoneCount { get; set; }

		public sJrcoAmyE6mj74ckFMl? CachedFate { get; set; }

		public Dictionary<uint, uint> CachedItemCounts { get; }

		public mEuFRom1N7yRwDnCHna(IEnumerable<TerritoryType> fateZones, IEnumerable<TerritoryType> exchangeZones, IEnumerable<TerritoryType> cityZones)
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			LastSelectedCustomPresetZoneIndex = -1;
			ExchangeItemSearchInput = string.Empty;
			CachedItemCounts = new Dictionary<uint, uint>();
			base._002Ector();
			TerritoryType[] zones = fateZones.ToArray();
			FateCombo = new FateSelectCombo(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6FF17EAE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5859f4ea3042459db2c9b610b3099912));
			SingleZoneCombo = new ZoneSelectCombo(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x45E4DC7D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1904f95d2a394c929e1a6d4f72943ad7), zones);
			MainZoneCombo = new ZoneSelectCombo(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x12D6BA2A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7fcf5a71a7ed445197f132979519c560), zones);
			SubZoneCombo = new ZoneSelectCombo(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5A7F3D1B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5154ccd9fed0487f8fa0244531466009), zones);
			MultiZoneCombo = new ZoneSelectCombo(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6FF1006E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5859f4ea3042459db2c9b610b3099912), zones);
			ExchangeZoneCombo = new ZoneSelectCombo(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x64E5F71A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f53034f3efc64a309b10c7885bf5d9bd), exchangeZones);
			RestZoneCombo = new ZoneSelectCombo(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5DD7C413 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c7355392ad9e43b6b82798cc42ea6c6f), cityZones);
			CustomModeZoneCombo = new ZoneSelectCombo(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x68AC8344 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4722689f1a4d41dc9dc800bfe1a89c09), zones);
			CustomModeItemCombo = new ItemSelectCombo(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x32D4983 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_86d75c161ced498ab6a767d2eb82d38e));
			CustomModeCompanionCombo = new CompanionCombo(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2310B231 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a96fab9e97944b39a8afe3fb1109ba7d));
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass113_0
	{
		public HashSet<uint> selected;

		public Func<uint, bool> _003C_003E9__0;

		public _003C_003Ec__DisplayClass113_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal bool SArmMu9iYV(uint z)
		{
			return !selected.Contains(z);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass113_1
	{
		public HashSet<uint> selected;

		public Func<uint, bool> _003C_003E9__1;

		public _003C_003Ec__DisplayClass113_1()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal bool nAvm4xg2fZ(uint z)
		{
			return !selected.Contains(z);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass117_0
	{
		public Vector3 navTarget;

		public bool fly;

		public _003C_003Ec__DisplayClass117_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal Task<List<Vector3>>? kVumsl1hTh(CancellationToken token)
		{
			return vnavmeshIPC.PathfindCancelable(LocalPlayerState.Object.Position, navTarget, fly, token);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass129_0
	{
		public unsafe FateContext* currentFate;

		public _003C_003Ec__DisplayClass129_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal unsafe bool A1wm8IJ19u(IGameObject x)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Invalid comparison between Unknown and I4
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			if (x is IBattleNPC battleNPC && (int)x.ObjectKind == 2 && battleNPC.NameID == 6737 && x != null && x.IsTargetable && !x.IsDead && ((BattleChara)battleNPC.ToBCStruct()).FateId == ((FateContext)currentFate).FateId)
			{
				return ((Enum)battleNPC.BattleNPCKind).HasFlag((Enum)(object)(BattleNpcSubKind)5);
			}
			return false;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass130_0
	{
		public bool allowOutOfRange;

		public unsafe BattleChara* localPlayer;

		public Predicate<IGameObject> _003C_003E9__0;

		public _003C_003Ec__DisplayClass130_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal unsafe bool Jwqmvad8e3(IGameObject x)
		{
			if (x is IBattleNPC && x.IsTargetable && !x.IsDead)
			{
				if (!allowOutOfRange)
				{
					if (ActionManager.CanUseActionOnTarget(142u, x.ToStruct()))
					{
						return ActionManager.GetActionInRangeOrLoS(19700u, (GameObject*)localPlayer, x.ToStruct()) != 562;
					}
					return false;
				}
				return true;
			}
			return false;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass132_0
	{
		public unsafe FateContext* currentFate;

		public Predicate<IGameObject> _003C_003E9__0;

		public _003C_003Ec__DisplayClass132_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal unsafe bool yWHmlV6qyg(IGameObject x)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Invalid comparison between Unknown and I4
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			if ((int)x.ObjectKind == 7 && ((GameObject)x.ToStruct()).FateId == ((FateContext)currentFate).FateId && x.IsTargetable)
			{
				return ((GameObject)x.ToStruct()).TargetableStatus.IsSetAll<ObjectTargetableFlags>(new ReadOnlySpan<ObjectTargetableFlags>(Unsafe.AsPointer(ref global::_003CPrivateImplementationDetails_003E._8263A852B3877BAB05AE51AE58179841C08B22A25E0E964F5C157690B61F9B9C), 2));
			}
			return false;
		}
	}

	private Tts4FKBgnLI6N0adTwv config;

	private b90cEomL5Jl8rVYNdZJ tracker;

	private mEuFRom1N7yRwDnCHna uiContext;

	private bool isOnAutoFate;

	private const uint BGM_FATE_ODIN = 153u;

	private const uint BGM_FATE_HMN = 154u;

	private const uint BNPC_FOLORN_MAIDEN = 6737u;

	private const uint ACTION_CHECK_CAN_USE_ON_TARGET = 142u;

	private const uint ACTION_CHECK_RANGE_OR_LOS = 19700u;

	private const uint ITEM_BICOLOR_GEMSTONE = 26807u;

	private const uint STATUS_GEM_BONUS_1 = 1288u;

	private const uint STATUS_GEM_BONUS_2 = 1289u;

	private const int ACTION_STATUS_OUT_OF_RANGE = 562;

	private const uint INVALID_GAME_OBJECT_ID = 3758096384u;

	private const float MOUNTED_SPEED = 12f;

	private const float AETHERYTE_OVERHEAD_SEC = 10f;

	private const byte FATE_PROGRESS_CAP = 80;

	private const byte FATE_SINGLE_TARGET_PROGRESS = 80;

	private const int MAX_ENGAGED_MONSTERS = 3;

	private const float PULL_HEALTH_THRESHOLD = 60f;

	private const byte SPECIAL_FATE_MIN_PROGRESS = 20;

	private const float FATE_MIN_TIME_REMAINING = 180f;

	private const float FATE_TIME_BUFFER = 30f;

	private const uint COLLECT_SUBMIT_MIN_HOLD = 20u;

	private const uint COLLECT_SUBMIT_INITIAL_MIN = 2u;

	private const float DEFAULT_TELEPORT_DISTANCE = 100f;

	private const uint EXCHANGE_THRESHOLD_MAX = 1400u;

	private const uint EXCHANGE_THRESHOLD_DEFAULT = 1200u;

	private const float LOW_HEALTH_THRESHOLD_DEFAULT = 20f;

	private const float DISTANCE_SQ_NEAR = 9f;

	private const float DISTANCE_SQ_CENTER = 400f;

	private const float DISTANCE_SQ_WALKABLE = 1600f;

	private const float DISTANCE_SQ_INTERACT = 9f;

	private const float DISTANCE_SQ_FATE_DISMOUNT = 25f;

	private const float DISTANCE_ABS_VALID_GROUND = 5f;

	private const float DISTANCE_ABS_CLOSED_GROUND = 3f;

	private const float DISTANCE_SQ_ARRIVED = 3600f;

	private const float ATTACK_RANGE_SQ_MELEE = 9f;

	private const float ATTACK_RANGE_SQ_RANGED = 400f;

	private const float PULL_RANGE_SQ = 400f;

	private const float PATH_VALIDATION_SQ = 900f;

	private const int PATHFIND_TIMEOUT_MS = 30000;

	private const int PATHFIND_RETRY_INTERVAL_MS = 1000;

	private const int PATHFIND_TARGET_CHANGE_CONFIRM_MS = 1000;

	private const int COLLECT_SUBMIT_CONFIRM_TIMEOUT_MS = 2000;

	private const int REST_TRAVEL_TIMEOUT_MS = 60000;

	private const int REST_RETRY_INTERVAL_MS = 300000;

	private const float FOLORN_MAIDEN_KILL_TIME = 60f;

	private static readonly FrozenDictionary<FateMode, (string Title, string Description)> FateModeLoc;

	private static FrozenSet<uint> FateZones;

	private static FrozenSet<uint> RestCities;

	private static FrozenSet<Tx5DWgBFgaOdVwt3xBP> ExchangeItems;

	private static FrozenSet<uint> ShopNPCDataIDs;

	private static readonly FrozenSet<FateState> InvalidFateStates;

	private static readonly wQgpjiBrB2dwArZO9DQ[] BuiltInCustomPresets;

	private readonly uGeIvcBNLWfsqCPBrl2 machine;

	private readonly IQGRQ4BGy50rFrjwhAw pathfind;

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	private bool IsSelectedCustomPresetBuiltIn
	{
		get
		{
			if (config.SelectedCustomPresetIndex >= 0)
			{
				return config.SelectedCustomPresetIndex < BuiltInCustomPresets.Length;
			}
			return false;
		}
	}

	protected unsafe override void Init()
	{
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Expected O, but got Unknown
		FateZones = (from x in (IEnumerable<TerritoryType>)LuminaGetter.Get<TerritoryType>()
			where ((TerritoryType)(ref x)).TerritoryIntendedUse.RowId == 1 && !((TerritoryType)(ref x)).IsPvpZone && !string.IsNullOrEmpty(x.ExtractPlaceName())
			select ((TerritoryType)(ref x)).RowId).ToFrozenSet();
		ExchangeItems = Tx5DWgBFgaOdVwt3xBP.dyHBqQwroc().ToFrozenSet();
		ShopNPCDataIDs = (from x in ((IEnumerable<ENpcResident>)LuminaGetter.Get<ENpcResident>()).Where(delegate(ENpcResident x)
			{
				//IL_0009: Unknown result type (might be due to invalid IL or missing references)
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0031: Unknown result type (might be due to invalid IL or missing references)
				//IL_0036: Unknown result type (might be due to invalid IL or missing references)
				string? text = ((object)((ENpcResident)(ref x)).Title/*cast due to constrained. prefix*/).ToString();
				ENpcResident value = LuminaGetter.GetRow<ENpcResident>(1027385u).Value;
				return text == ((object)((ENpcResident)(ref value)).Title/*cast due to constrained. prefix*/).ToString();
			})
			select ((ENpcResident)(ref x)).RowId).ToFrozenSet();
		tracker = new b90cEomL5Jl8rVYNdZJ();
		TerritoryType[] fateZones = FateZones.Select((uint x) => LuminaGetter.GetRow<TerritoryType>(x).GetValueOrDefault()).ToArray();
		IEnumerable<TerritoryType> exchangeZones = ((IEnumerable<TerritoryType>)LuminaGetter.Get<TerritoryType>()).Where(delegate(TerritoryType x)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			uint rowId = ((TerritoryType)(ref x)).TerritoryIntendedUse.RowId;
			bool flag = rowId <= 1;
			return flag && !((TerritoryType)(ref x)).IsPvpZone && !string.IsNullOrWhiteSpace(x.ExtractPlaceName());
		});
		TerritoryType[] array = ((IEnumerable<TerritoryType>)LuminaGetter.Get<TerritoryType>()).Where((TerritoryType x) => ((TerritoryType)(ref x)).TerritoryIntendedUse.RowId == 0 && !((TerritoryType)(ref x)).IsPvpZone && !string.IsNullOrWhiteSpace(x.ExtractPlaceName())).ToArray();
		RestCities = array.Select((TerritoryType x) => ((TerritoryType)(ref x)).RowId).ToFrozenSet();
		uiContext = new mEuFRom1N7yRwDnCHna(fateZones, exchangeZones, array);
		config = ModuleConfigExtension.Load<Tts4FKBgnLI6N0adTwv>(this) ?? new Tts4FKBgnLI6N0adTwv
		{
			BlacklistedFates = new HashSet<uint>(),
			ExchangeItem = ExchangeItems.First()
		};
		uiContext.FateCombo.SelectedIDs = config.BlacklistedFates;
		uiContext.SingleZoneCombo.SelectedID = config.SingleZone;
		uiContext.MainZoneCombo.SelectedID = config.MainZone;
		uiContext.SubZoneCombo.SelectedID = config.SubZone;
		uiContext.MultiZoneCombo.SelectedIDs = config.MultiZones;
		uiContext.ExchangeZoneCombo.SelectedID = config.ExchangeZone;
		uiContext.RestZoneCombo.SelectedID = config.RestZone;
		if (base.Overlay == null)
		{
			Overlay overlay = (base.Overlay = new Overlay(this));
		}
		((Window)base.Overlay).WindowName = Info.Title + LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x45E4B1F1 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1904f95d2a394c929e1a6d4f72943ad7);
		((Window)base.Overlay).Flags = (ImGuiWindowFlags)73;
		if (GameState.IsLoggedIn)
		{
			for (byte b = 0; b < FateAchievements.VersionCount; b++)
			{
				((Achievement)(&((UIState)UIState.Instance()).Achievement)).RequestFateProgressTab(b);
			}
		}
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += GV58xiWYBs;
		OmenServiceBase<ExecuteCommandManager>.Instance().RegPost(iPw8lRQ0cr);
		OmenServiceBase<FrameworkManager>.Instance().Reg(new OnUpdateDelegate(hH68pY9jH6), 100u);
		OmenServiceBase<UseActionManager>.Instance().RegPostUseActionLocation(Wvg8vTiNME);
	}

	protected override void Uninit()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected O, but got Unknown
		OmenServiceBase<UseActionManager>.Instance().Unreg(Wvg8vTiNME);
		OmenServiceBase<FrameworkManager>.Instance().Unreg(hH68pY9jH6);
		OmenServiceBase<ExecuteCommandManager>.Instance().Unreg(iPw8lRQ0cr);
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= GV58xiWYBs;
		q5q8UWZLbj();
	}

	private void Wvg8vTiNME(bool result, ActionType actionType, uint actionID, ulong targetID, Vector3 location, uint extraParam, byte a7)
	{
		if (isOnAutoFate && result)
		{
			Throttler.Shared.Throttle(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489B3A88 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cade14e68bb84906959f3cda5c496848), 2500u, reThrottle: true);
		}
	}

	private void iPw8lRQ0cr(ExecuteCommandFlag command, uint param1, uint param2, uint param3, uint param4)
	{
		if (isOnAutoFate && command == ExecuteCommandFlag.Teleport)
		{
			QO98N7kSGo();
			machine.PendingZoneSwitch = null;
		}
	}

	private void GV58xiWYBs(uint obj)
	{
		bool num = machine.DutyStep != DutyEscapeStep.None;
		if (machine.DutyStep == DutyEscapeStep.WaitingToEnter && obj != machine.DutyEscapeZone)
		{
			machine.DutyStep = DutyEscapeStep.WaitingToExit;
		}
		else if (machine.DutyStep == DutyEscapeStep.WaitingToExit && obj == machine.DutyEscapeZone)
		{
			machine.DutyStep = DutyEscapeStep.CorrectingPosition;
		}
		machine.PendingZoneSwitch = null;
		machine.f8LBXu2uHx(obj);
		if (isOnAutoFate && config.CurrentMode == FateMode.Custom)
		{
			machine.nnbBUCfrhJ(obj);
		}
		if (!num)
		{
			tracker.pLjmSIgpim();
		}
		i2yv995ltv();
		QO98N7kSGo();
		Throttler.Shared.Throttle(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-1487420814 ^ -1393316400 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2edd08450a8b42e6997855e5217d6d55), 3000u, reThrottle: true);
	}

	private unsafe void hH68pY9jH6(IFramework framework)
	{
		if (!isOnAutoFate || An08mmru2g() || GameState.ContentFinderCondition != 0)
		{
			return;
		}
		BattleChara* localPlayer = Control.GetLocalPlayer();
		if (localPlayer == null || !UIModuleExtension.IsScreenReady())
		{
			return;
		}
		if (machine.iK7BupbpFK(out var zone))
		{
			UeR8cuXfZw(zone);
			return;
		}
		SPX83ixi2d(localPlayer);
		if (Environment.TickCount64 < machine.ReviveResumeTick)
		{
			return;
		}
		if (((BattleChara)localPlayer).IsDead())
		{
			qJd8PhxiyM();
		}
		else if (!xea8JAw864())
		{
			RKO8hLnxpq(localPlayer);
			FateContext* currentFate = ((FateManager)FateManager.Instance()).CurrentFate;
			switch (machine.CurrentPhase = Wgq8BhMN5d(localPlayer, currentFate))
			{
			case Phase.CombatEmergency:
				B688o1tCTh(localPlayer);
				break;
			case Phase.CollectComplete:
				xXg8gaAL7Z(localPlayer);
				break;
			case Phase.PluginsOrModulesNotReady:
			case Phase.ManagedMovement:
				break;
			case Phase.InFate:
				sh18AbuhaR(currentFate);
				break;
			case Phase.FateEnding:
				NpW8w0qqfX(currentFate);
				break;
			case Phase.Exchange:
				QlT8EBA6JK();
				break;
			case Phase.ZoneSwitch:
				ufY8F9iDx3();
				break;
			case Phase.SearchAndMove:
				vta8QMUe6W();
				break;
			}
		}
	}

	private unsafe void SPX83ixi2d(BattleChara* localPlayer)
	{
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		switch (config.CurrentMode)
		{
		case FateMode.SingleZone:
			if (GameState.TerritoryType == config.SingleZone)
			{
				break;
			}
			return;
		case FateMode.MainSubZone:
			if (GameState.TerritoryType == config.MainZone || GameState.TerritoryType == config.SubZone)
			{
				break;
			}
			return;
		case FateMode.MultiZone:
			if (config.MultiZones.Contains(GameState.TerritoryType))
			{
				break;
			}
			return;
		case FateMode.Custom:
		{
			CustomPreset? customPreset = f9x87jJkaf();
			if (customPreset == null || !customPreset.Zones.TryGetValue(GameState.TerritoryType, out CustomZoneRule value) || !value.IsValid)
			{
				return;
			}
			break;
		}
		}
		tracker.WvVmHkOR1f(GameState.TerritoryType, InstancesManager.CurrentInstance);
		if (tracker.DKDmVZ0ArM(((FateManager)FateManager.Instance()).CurrentFate))
		{
			Throttler.Shared.Throttle(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D758CFE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8b51f8019ce49018b38e06104a97fc1), 15000u, reThrottle: true);
		}
		if (tracker.CompletedFateName != null)
		{
			if (config.CurrentMode == FateMode.Custom)
			{
				machine.E1bBIfKAsA(GameState.TerritoryType);
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
			defaultInterpolatedStringHandler.AppendFormatted(tracker.CompletedFateDuration, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3C1420A1 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_d71d1e134fde413fa0bee3b2419181b6));
			string text = defaultInterpolatedStringHandler.ToStringAndClear();
			OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4CF2A262 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a07aac30eb32484e94cb4d1e62117765), tracker.CompletedFateName, text)));
		}
		tracker.wGjm9k88c2(Vector3.op_Implicit(((BattleChara)localPlayer).Position), ((BattleChara)localPlayer).Level, config.BlacklistedFates, config.IgnoreFarLevelFates);
	}

	private unsafe Phase Wgq8BhMN5d(BattleChara* localPlayer, FateContext* currentFate)
	{
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		if (!vnavmeshIPC.IsPluginEnabled() || !vnavmeshIPC.GetIsNavReady() || !BossModIPC.IsPluginEnabled() || !MD08urmiRQ())
		{
			return Phase.PluginsOrModulesNotReady;
		}
		if (OmenServiceBase<MovementManager>.Instance().IsManagerBusy)
		{
			return Phase.ManagedMovement;
		}
		float num = (float)((BattleChara)localPlayer).Health / (float)((BattleChara)localPlayer).MaxHealth * 100f;
		bool flag = DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)26];
		bool flag2 = DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)77];
		AetheryteRecord nearestAetheryte = OmenServiceBase<AetheryteRecordManager>.Instance().GetNearestAetheryte(GameState.TerritoryType, Vector3.Zero, excludeAethernet: true);
		bool flag3 = gBMvVKactb();
		if (flag && !flag2 && num <= config.LowHealthThreshold && Throttler.Shared.Check(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-224923271 ^ -1321185347 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_974d377e1ed74986b81c34c95c2cc846)))
		{
			return Phase.CombatEmergency;
		}
		if (flag && !flag2 && !flag3 && currentFate == null && nearestAetheryte != null && Throttler.Shared.Check(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xE0950CD ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_aba6092d7cea44a49d1667540500e363)))
		{
			return Phase.CombatEmergency;
		}
		if (flag && !flag2 && !flag3 && currentFate != null && ((FateContext)currentFate).EventItem != 0 && LocalPlayerState.GetItemCount(((FateContext)currentFate).EventItem) == 0 && ((FateContext)currentFate).Progress == 100)
		{
			return Phase.CollectComplete;
		}
		if (machine.ExchangeStage != ExchangeStep.None)
		{
			return Phase.Exchange;
		}
		if (currentFate != null)
		{
			if (((FateContext)currentFate).EventItem != 0 && tracker.IsCollectSubmitPending)
			{
				return Phase.InFate;
			}
			if (((FateContext)currentFate).EventItem != 0)
			{
				if (((FateContext)currentFate).Progress >= 100 && LocalPlayerState.GetItemCount(((FateContext)currentFate).EventItem) == 0)
				{
					goto IL_0210;
				}
				if (((FateContext)currentFate).Progress >= 100)
				{
					return Phase.InFate;
				}
			}
			if (((FateContext)currentFate).Progress >= 100 || InvalidFateStates.Contains(((FateContext)currentFate).State))
			{
				return Phase.FateEnding;
			}
			return Phase.InFate;
		}
		goto IL_0210;
		IL_0210:
		if (!tracker.IsWaitingForFateRemoval && config.ExchangeItem != null && LocalPlayerState.GetItemCount(26807u) >= config.ExchangeThreshold)
		{
			return Phase.Exchange;
		}
		if (E6H8YwDrd1() && Throttler.Shared.Check(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2194C7D6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cd1b319f326a44d1aaea645e95d8698b)) && !tracker.IsWaitingForFateRemoval)
		{
			machine.PendingZoneSwitch = qy18bYhMca();
			if (machine.PendingZoneSwitch.HasValue)
			{
				return Phase.ZoneSwitch;
			}
		}
		return Phase.SearchAndMove;
	}

	private bool An08mmru2g()
	{
		switch (machine.DutyStep)
		{
		case DutyEscapeStep.None:
			return false;
		case DutyEscapeStep.WaitingForEvent:
			if (IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()) || !UIModuleExtension.IsScreenReady() || !Throttler.Shared.Throttle(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x591C92DB ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_20724779fecb4a91959b8028d04d1118), 10000u))
			{
				return true;
			}
			OmenServiceBase<ChatManager>.Instance().SendMessage(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-864210836 ^ -1884649100 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_974d377e1ed74986b81c34c95c2cc846));
			machine.DutyStep = DutyEscapeStep.WaitingToEnter;
			machine.StepStartedTick = Environment.TickCount64;
			return true;
		case DutyEscapeStep.WaitingToEnter:
			if (GameState.ContentFinderCondition != 0)
			{
				machine.DutyStep = DutyEscapeStep.WaitingToExit;
				machine.StepStartedTick = Environment.TickCount64;
				return true;
			}
			if (Environment.TickCount64 - machine.StepStartedTick >= 15000)
			{
				machine.DutyStep = DutyEscapeStep.WaitingForEvent;
			}
			return true;
		case DutyEscapeStep.WaitingToExit:
			return true;
		case DutyEscapeStep.CorrectingPosition:
			if (!UIModuleExtension.IsScreenReady())
			{
				if (machine.DutyEscapePosition != Vector3.Zero && LocalPlayerState.Object != null)
				{
					OmenServiceBase<MovementManager>.Instance().TPPlayerAddress(machine.DutyEscapePosition);
				}
				return true;
			}
			if (GameState.ContentFinderCondition != 0)
			{
				return true;
			}
			machine.nQQBYsju3h();
			return false;
		default:
			return false;
		}
	}

	private void OaA8iSjulu(Vector3? position = null)
	{
		if (machine.DutyStep == DutyEscapeStep.None)
		{
			machine.ExchangeStage = ExchangeStep.None;
			machine.DutyStep = DutyEscapeStep.WaitingForEvent;
			machine.DutyEscapeZone = GameState.TerritoryType;
			machine.DutyEscapePosition = position.GetValueOrDefault();
			machine.StepStartedTick = Environment.TickCount64;
		}
	}

	private void qJd8PhxiyM()
	{
		machine.EjBBzEs8ev();
		i2yv995ltv();
		QO98N7kSGo();
		ContentsFinderHelper.CancelDutyApply();
		tracker.sL3mKwGPoV();
		if (Throttler.Shared.Throttle(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D9C1503 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3badc14f3c904cf4809aa58db382c8ee), 5000u))
		{
			OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.Revive, 8u);
			machine.ReviveResumeTick = Environment.TickCount64 + 5000;
		}
	}

	private unsafe static void RKO8hLnxpq(BattleChara* localPlayer)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		CompanionInfo companionInfo = ((Buddy)(&((UIState)UIState.Instance()).Buddy)).CompanionInfo;
		if (!(companionInfo.TimeLeft <= 0f))
		{
			byte b = (byte)(((float)((BattleChara)localPlayer).Health / (float)((BattleChara)localPlayer).MaxHealth * 100f > 60f) ? 6u : 7u);
			if (companionInfo.ActiveCommand != b && ((ActionManager)ActionManager.Instance()).GetActionStatus((ActionType)6, (uint)b, 3758096384uL, false, false, (uint*)null) == 0)
			{
				OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)6, b, 3758096384uL, 0u, (UseActionMode)0);
			}
		}
	}

	private unsafe void B688o1tCTh(BattleChara* localPlayer)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if ((float)((BattleChara)localPlayer).Health / (float)((BattleChara)localPlayer).MaxHealth * 100f <= config.LowHealthThreshold)
		{
			QO98N7kSGo();
			OaA8iSjulu();
			return;
		}
		AetheryteRecord nearestAetheryte = OmenServiceBase<AetheryteRecordManager>.Instance().GetNearestAetheryte(GameState.TerritoryType, Vector3.op_Implicit(((BattleChara)localPlayer).Position), excludeAethernet: true);
		if (nearestAetheryte != null)
		{
			QO98N7kSGo();
			OaA8iSjulu(nearestAetheryte.Position);
		}
	}

	private unsafe void xXg8gaAL7Z(BattleChara* localPlayer)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		AetheryteRecord nearestAetheryte = OmenServiceBase<AetheryteRecordManager>.Instance().GetNearestAetheryte(GameState.TerritoryType, Vector3.op_Implicit(((BattleChara)localPlayer).Position), excludeAethernet: true);
		if (nearestAetheryte != null)
		{
			QO98N7kSGo();
			OaA8iSjulu(nearestAetheryte.Position);
		}
	}

	private unsafe void NpW8w0qqfX(FateContext* currentFate)
	{
		i2yv995ltv();
		QO98N7kSGo();
		if (((FateManager)FateManager.Instance()).IsSyncedToFate(currentFate))
		{
			FateCommand.UnsyncLevel(((FateContext)currentFate).FateId);
		}
		TargetManager.Target = null;
		if (tracker.cjjmOjbBnc(((FateContext)currentFate).FateId))
		{
			Throttler.Shared.Throttle(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x12D6D7F8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7fcf5a71a7ed445197f132979519c560), 2500u, reThrottle: true);
			Throttler.Shared.Throttle(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--1470179789 ^ 0x1AD4A533 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8b51f8019ce49018b38e06104a97fc1), 15000u, reThrottle: true);
		}
	}

	private unsafe void QlT8EBA6JK()
	{
		if (config.ExchangeItem == null)
		{
			machine.ExchangeStage = ExchangeStep.None;
			return;
		}
		if (GameState.TerritoryType != config.ExchangeZone)
		{
			QO98N7kSGo();
			if (E6H8YwDrd1() && Throttler.Shared.Throttle(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x76EB1E8D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b988ddaee02846b69dd4e446b5da9de9), 10000u))
			{
				OmenServiceBase<AetheryteRecordManager>.Instance().GetNearestAetheryte(config.ExchangeZone, Vector3.Zero, excludeAethernet: true)?.TeleportTo();
			}
			return;
		}
		if (machine.ExchangeStage == ExchangeStep.None)
		{
			machine.ExchangeStage = ExchangeStep.Moving;
		}
		switch (machine.ExchangeStage)
		{
		case ExchangeStep.Moving:
			if (H9f8eTbREx(config.ExchangePosition) == DirectPathfindResult.InRange)
			{
				B1svjugXkl();
				if (!IConditionExtension.get_IsOnMount(DalamudServiceExtension.Instance<ICondition>()))
				{
					machine.ExchangeStage = ExchangeStep.Interacting;
				}
			}
			break;
		case ExchangeStep.Interacting:
			if ((*Addons.ShopExchangeCurrency).IsAddonAndNodesReady())
			{
				machine.ExchangeStage = ExchangeStep.Purchasing;
			}
			else if (Throttler.Shared.Throttle(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x44278356 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8dfe57222f64fa68d9d16c8b6fe9d0b)))
			{
				IOmenDalamudService<IObjectTable>.Instance().FirstOrDefault((IGameObject x) => (int)x.ObjectKind == 3 && ShopNPCDataIDs.Contains(x.DataID))?.TargetInteract();
			}
			break;
		case ExchangeStep.Purchasing:
			if ((*Addons.SelectYesno).IsAddonAndNodesReady())
			{
				if (Throttler.Shared.Throttle(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x258F1CEE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_dadeb2ab61424d31be7cc1966e675825)))
				{
					AddonSelectYesnoEvent.ClickYes();
				}
			}
			else if (config.ExchangeItem.XY6BQxDJjQ() == 0)
			{
				machine.ExchangeStage = ExchangeStep.Closing;
			}
			else if (Throttler.Shared.Throttle(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x47530182 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_790eb00575b3425495c924ac13469f4c)))
			{
				config.ExchangeItem.pIHBcbk2bB();
			}
			break;
		case ExchangeStep.Closing:
			if (Addons.ShopExchangeCurrency == null)
			{
				machine.ExchangeStage = ExchangeStep.None;
			}
			else if (Throttler.Shared.Throttle(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6FF16BFA ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5859f4ea3042459db2c9b610b3099912)))
			{
				((AtkUnitBase)Addons.ShopExchangeCurrency).Close(true);
			}
			break;
		}
	}

	private hsIAJpBwayeFO4nLLSg? qy18bYhMca()
	{
		sJrcoAmyE6mj74ckFMl fate;
		TerritoryType item;
		switch (config.CurrentMode)
		{
		case FateMode.SingleZone:
			if (GameState.TerritoryType != config.SingleZone)
			{
				return new hsIAJpBwayeFO4nLLSg(ZoneSwitchAction.Teleport, config.SingleZone);
			}
			if (tracker.VnDmfLR0qH(out fate))
			{
				return null;
			}
			if (!InstancesManager.IsInstancedArea)
			{
				return null;
			}
			return new hsIAJpBwayeFO4nLLSg(ZoneSwitchAction.InstanceSwitch, 0u);
		case FateMode.MainSubZone:
			if (GameState.TerritoryType == config.SubZone)
			{
				if (tracker.FinishedCount != 0)
				{
					return new hsIAJpBwayeFO4nLLSg(ZoneSwitchAction.Teleport, config.MainZone);
				}
				if (tracker.VnDmfLR0qH(out fate))
				{
					return null;
				}
				if (!InstancesManager.IsInstancedArea)
				{
					return null;
				}
				return new hsIAJpBwayeFO4nLLSg(ZoneSwitchAction.InstanceSwitch, 0u);
			}
			if (GameState.TerritoryType == config.MainZone)
			{
				if (tracker.VnDmfLR0qH(out fate))
				{
					return null;
				}
				if (InstancesManager.IsInstancedArea)
				{
					return new hsIAJpBwayeFO4nLLSg(ZoneSwitchAction.InstanceSwitch, 0u);
				}
				return new hsIAJpBwayeFO4nLLSg(ZoneSwitchAction.Teleport, config.SubZone);
			}
			return new hsIAJpBwayeFO4nLLSg(ZoneSwitchAction.Teleport, config.MainZone);
		case FateMode.MultiZone:
		{
			if (config.MultiZones.Count <= 0)
			{
				break;
			}
			bool flag3 = false;
			bool flag4 = false;
			uint num6 = 0u;
			long num7 = long.MaxValue;
			foreach (uint multiZone in config.MultiZones)
			{
				if (!FateAchievements.ZoneToAchievement.TryGetValue(multiZone, out var value) || !OmenServiceBase<AchievementManager>.Instance().TryGetAchievement(value, out AchievementInfo achievementInfo) || achievementInfo.IsFinished)
				{
					continue;
				}
				flag3 = true;
				if (multiZone == GameState.TerritoryType)
				{
					flag4 = true;
					continue;
				}
				long num8 = machine.o1FB76Xwho(multiZone);
				if (num8 < num7)
				{
					num6 = multiZone;
					num7 = num8;
				}
			}
			if (!flag3)
			{
				return null;
			}
			if (flag4 && tracker.VnDmfLR0qH(out fate))
			{
				return null;
			}
			if (num6 == 0 || !LuminaGetter.TryGetRow<TerritoryType>(num6, out item))
			{
				return new hsIAJpBwayeFO4nLLSg(ZoneSwitchAction.Sanctuary, 0u);
			}
			return new hsIAJpBwayeFO4nLLSg(ZoneSwitchAction.Teleport, num6);
		}
		case FateMode.Custom:
		{
			CustomPreset customPreset = f9x87jJkaf();
			if (customPreset != null)
			{
				Dictionary<uint, CustomZoneRule> zones = customPreset.Zones;
				if (zones != null && zones.Count > 0)
				{
					bool flag = false;
					bool flag2 = false;
					uint num = 0u;
					long num2 = long.MaxValue;
					foreach (var (num4, customZoneRule2) in customPreset.Zones)
					{
						if (!customZoneRule2.IsValid || nC98qKeA1J(num4, customZoneRule2))
						{
							continue;
						}
						flag = true;
						if (num4 == GameState.TerritoryType)
						{
							flag2 = true;
							continue;
						}
						long num5 = machine.o1FB76Xwho(num4);
						if (num5 < num2)
						{
							num = num4;
							num2 = num5;
						}
					}
					if (!flag)
					{
						return null;
					}
					if (flag2 && tracker.VnDmfLR0qH(out fate))
					{
						return null;
					}
					if (num == 0 || !LuminaGetter.TryGetRow<TerritoryType>(num, out item))
					{
						return new hsIAJpBwayeFO4nLLSg(ZoneSwitchAction.Sanctuary, 0u);
					}
					return new hsIAJpBwayeFO4nLLSg(ZoneSwitchAction.Teleport, num);
				}
			}
			return null;
		}
		}
		return null;
	}

	private unsafe void ufY8F9iDx3()
	{
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		QO98N7kSGo();
		hsIAJpBwayeFO4nLLSg? pendingZoneSwitch = machine.PendingZoneSwitch;
		if (!pendingZoneSwitch.HasValue)
		{
			return;
		}
		hsIAJpBwayeFO4nLLSg valueOrDefault = pendingZoneSwitch.GetValueOrDefault();
		switch (valueOrDefault.Action)
		{
		case ZoneSwitchAction.Teleport:
			if (GameState.TerritoryType != valueOrDefault.Zone && E6H8YwDrd1() && Throttler.Shared.Throttle(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4B0F4388 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_8e002b0644ef421698ef6f63772929af), 10000u))
			{
				OmenServiceBase<AetheryteRecordManager>.Instance().GetNearestAetheryte(valueOrDefault.Zone, Vector3.Zero, excludeAethernet: true)?.TeleportTo();
			}
			break;
		case ZoneSwitchAction.InstanceSwitch:
		{
			uint num = ((InstancesManager.CurrentInstance >= InstancesManager.InstanceAmount) ? 1u : (InstancesManager.CurrentInstance + 1));
			if (InstancesManager.CurrentInstance != num && E6H8YwDrd1() && Throttler.Shared.Throttle(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x66B36170 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_762f55d0e7ef4ecd8f3f55c0f8a10fae), 10000u))
			{
				OmenServiceBase<ChatManager>.Instance().SendMessage(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x395191BD ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3aeae81f85f84848b273be7b2bfbf497));
				ChatManager chatManager = OmenServiceBase<ChatManager>.Instance();
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 1);
				defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x475302E2 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_790eb00575b3425495c924ac13469f4c));
				defaultInterpolatedStringHandler.AppendFormatted(num);
				chatManager.SendMessage(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			break;
		}
		case ZoneSwitchAction.Sanctuary:
		{
			BattleChara* localPlayer = Control.GetLocalPlayer();
			if (localPlayer == null)
			{
				break;
			}
			AetheryteRecord nearestAetheryte = OmenServiceBase<AetheryteRecordManager>.Instance().GetNearestAetheryte(GameState.TerritoryType, Vector3.op_Implicit(((BattleChara)localPlayer).Position), excludeAethernet: true);
			if ((object)nearestAetheryte != null)
			{
				if (LocalPlayerState.DistanceTo2DSquared(nearestAetheryte.Position.ToVector2()) <= 3600f)
				{
					HypvCF3peJ();
				}
				else if (E6H8YwDrd1() && Throttler.Shared.Throttle(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x1C17F88B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_accf3844f1c6464481df1849f0aee1bb), 10000u))
				{
					nearestAetheryte.TeleportTo();
				}
			}
			break;
		}
		}
	}

	private bool nC98qKeA1J(uint zone, CustomZoneRule rule)
	{
		if (rule.RequiredFateCount != 0 && machine.GLZBdHAZpr(zone) < rule.RequiredFateCount)
		{
			return false;
		}
		return rule.Items.All((KeyValuePair<uint, uint> item) => LocalPlayerState.GetItemCount(item.Key) >= item.Value);
	}

	private unsafe void UeR8cuXfZw(uint zone)
	{
		if (config.CurrentMode != FateMode.Custom)
		{
			return;
		}
		CustomPreset? customPreset = f9x87jJkaf();
		if (customPreset == null || !customPreset.Zones.TryGetValue(zone, out CustomZoneRule value) || !value.IsValid)
		{
			return;
		}
		if (value.MinionID != 0 && ((UIState)UIState.Instance()).IsCompanionUnlocked(value.MinionID))
		{
			MinionCommand.Summon(value.MinionID);
		}
		if (!string.IsNullOrWhiteSpace(value.Macro))
		{
			string[] array = value.Macro.Split(new string[3]
			{
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6E00239C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c40146824d9043e0906f4e1889cf39aa),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x51E506F4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a90a92322bff42e180982e106200d4a3),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x423E29A4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_6368ba44fa4749c09da09b5ebf853b71)
			}, StringSplitOptions.None);
			if (array.Length <= 15)
			{
				OmenServiceBase<ChatManager>.Instance().ExecuteMacro(array);
			}
		}
	}

	private unsafe void sh18AbuhaR(FateContext* currentFate)
	{
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		BattleChara* localPlayer = Control.GetLocalPlayer();
		if (localPlayer == null)
		{
			return;
		}
		if (tracker.IsCollectSubmitPending)
		{
			QO98N7kSGo();
			return;
		}
		float num = fDc8zVrbeL();
		AQ1vy4kpWs(((FateContext)currentFate).FateId, ((FateContext)currentFate).Location, ((FateContext)currentFate).Radius, ((FateContext)currentFate).Progress, ((FateContext)currentFate).EventItem != 0, allowOutOfRange: false, out IGameObject monster);
		if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)77])
		{
			float groundY;
			if (((FateContext)currentFate).EventItem != 0)
			{
				if (!PqkvHQ4sf0(out IGameObject obj) || H9f8eTbREx(obj.Position, obj.HitboxRadius * obj.HitboxRadius + 9f) != DirectPathfindResult.InRange)
				{
					return;
				}
			}
			else if ((monster != null && H9f8eTbREx(monster.Position, monster.HitboxRadius * monster.HitboxRadius + 25f) != DirectPathfindResult.InRange) || !RaycastHelper.TryGetGroundHeight(Vector3.op_Implicit(((BattleChara)localPlayer).Position), out groundY) || MathF.Abs(groundY - monster.Position.Y) > 5f)
			{
				return;
			}
			QO98N7kSGo();
		}
		if (IConditionExtension.get_IsOnMount(DalamudServiceExtension.Instance<ICondition>()))
		{
			B1svjugXkl();
		}
		if (!((FateManager)FateManager.Instance()).IsSyncedToFate(currentFate) && Throttler.Shared.Throttle(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x64E59F6A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f53034f3efc64a309b10c7885bf5d9bd)))
		{
			FateCommand.SyncLevel(((FateContext)currentFate).FateId);
		}
		if (((FateContext)currentFate).EventItem != 0 && N5QvLWvM7R(out IBattleNPC obj2) && !IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()))
		{
			uint itemCount = LocalPlayerState.GetItemCount(((FateContext)currentFate).EventItem);
			if ((itemCount != 0 && ((FateContext)currentFate).Progress == 100) || itemCount >= tracker.xu6mj0FRwF(((FateContext)currentFate).Progress))
			{
				TargetManager.Target = obj2;
				i2yv995ltv();
				if (H9f8eTbREx(obj2.Position, obj2.HitboxRadius * obj2.HitboxRadius + 9f) == DirectPathfindResult.InRange && !IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()) && Throttler.Shared.Throttle(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6AAEA2DE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_839f542231154d29b0562b687afa328a)))
				{
					tracker.q3BmCsM1m5(((FateContext)currentFate).Progress, itemCount);
					obj2.TargetInteract();
				}
				return;
			}
		}
		IGameObject gameObject = null;
		bool flag = false;
		if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)26] && monster != null)
		{
			gameObject = monster;
		}
		if (((FateContext)currentFate).EventItem == 0 && u9hvnHw4lY(out IBattleNPC obj3) && (float)(((FateContext)currentFate).StartTimeEpoch + ((FateContext)currentFate).Duration - OmenServiceBase<StandardTimeManager>.Instance().NowOffset.ToUnixTimeSeconds()) > 120f)
		{
			gameObject = obj3;
			flag = true;
		}
		IGameObject obj4;
		if (gameObject != null)
		{
			AYVvfjtdrb(gameObject);
			if (!flag && DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)26])
			{
				QO98N7kSGo();
			}
			else if (H9f8eTbREx(gameObject.Position, gameObject.HitboxRadius * gameObject.HitboxRadius + num) == DirectPathfindResult.InRange)
			{
				QO98N7kSGo();
			}
		}
		else if (((FateContext)currentFate).EventItem != 0 && PqkvHQ4sf0(out obj4))
		{
			if (H9f8eTbREx(obj4.Position, obj4.HitboxRadius * obj4.HitboxRadius + 9f) == DirectPathfindResult.InRange && !IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()) && Throttler.Shared.Throttle(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x591C9011 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_20724779fecb4a91959b8028d04d1118)))
			{
				obj4.TargetInteract();
			}
		}
		else if (monster != null)
		{
			TargetManager.Target = monster;
			AYVvfjtdrb(monster);
			if (IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()) || DalamudServiceExtension.Instance<ICondition>().Any((ConditionFlag[])(object)new ConditionFlag[1] { (ConditionFlag)26 }))
			{
				QO98N7kSGo();
			}
			if (!IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()) && !DalamudServiceExtension.Instance<ICondition>().Any((ConditionFlag[])(object)new ConditionFlag[1] { (ConditionFlag)26 }) && H9f8eTbREx(monster.Position, monster.HitboxRadius * monster.HitboxRadius + num) == DirectPathfindResult.InRange)
			{
				QO98N7kSGo();
			}
		}
		else
		{
			Vector3 vector = vnavmeshIPC.QueryNearestPointOnMesh(((FateContext)currentFate).Location, ((FateContext)currentFate).Radius, 10000f) ?? ((FateContext)currentFate).Location;
			if (!DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)26] && LocalPlayerState.DistanceTo2DSquared(vector.ToVector2()) > 400f)
			{
				H9f8eTbREx(vector, 9f);
			}
		}
	}

	private unsafe void vta8QMUe6W()
	{
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Invalid comparison between Unknown and I4
		if (Control.GetLocalPlayer() == null)
		{
			return;
		}
		FateContext* currentFate = ((FateManager)FateManager.Instance()).CurrentFate;
		if (currentFate != null && (((FateContext)currentFate).EventItem == 0 || ((FateContext)currentFate).Progress < 100))
		{
			QO98N7kSGo();
			return;
		}
		if (!tracker.VnDmfLR0qH(out sJrcoAmyE6mj74ckFMl fate))
		{
			QO98N7kSGo();
			if (!DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4] && !DalamudServiceExtension.Instance<ICondition>().Any((ConditionFlag[])(object)new ConditionFlag[2]
			{
				(ConditionFlag)27,
				(ConditionFlag)26
			}))
			{
				HypvCF3peJ();
			}
			return;
		}
		Vector3 vector = vnavmeshIPC.QueryNearestPointOnMesh(fate.Position + new Vector3(0f, 5f, 0f), fate.Radius, 10000f) ?? fate.Position;
		if (!IQGRQ4BGy50rFrjwhAw.oJtB66IZtM(vector))
		{
			vector = fate.Position;
		}
		if ((int)fate.GameState == 3)
		{
			if (Vn6vSJIrjq(fate.FateID, out IGameObject obj))
			{
				fate.StartNPCPosition = obj.Position;
				TargetManager.Target = obj;
				if (H9f8eTbREx(obj.Position, obj.HitboxRadius * obj.HitboxRadius + 9f) == DirectPathfindResult.InRange)
				{
					B1svjugXkl();
					if (Throttler.Shared.Throttle(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x68546A8F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_23522a2a14314da28eb44187a09cd0b5)))
					{
						FateCommand.Start(obj.FateID, obj.EntityID);
					}
				}
				return;
			}
			if (fate.StartNPCPosition != default(Vector3))
			{
				H9f8eTbREx(fate.StartNPCPosition, 13f);
				return;
			}
		}
		if (LocalPlayerState.DistanceTo3DSquared(vector) > 10000f && E6H8YwDrd1())
		{
			AetheryteRecord nearestAetheryte = OmenServiceBase<AetheryteRecordManager>.Instance().GetNearestAetheryte(GameState.TerritoryType, vector, excludeAethernet: true);
			if ((object)nearestAetheryte != null && UcG8XgdQGn(vector, nearestAetheryte.Position))
			{
				QO98N7kSGo();
				if (Throttler.Shared.Throttle(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489F7012 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b2294f2f9bb24a76b00cf1af75888414), 10000u))
				{
					nearestAetheryte.TeleportTo();
				}
				return;
			}
		}
		if (fate.Rule == FateRule.Kill && AQ1vy4kpWs(fate.FateID, fate.Position, fate.Radius, fate.Progress, isCollectFate: false, allowOutOfRange: true, out IGameObject monster))
		{
			TargetManager.Target = monster;
			if (H9f8eTbREx(monster.Position, 25f) == DirectPathfindResult.InRange)
			{
				B1svjugXkl();
			}
		}
		else
		{
			H9f8eTbREx(vector, 9f);
		}
	}

	private unsafe bool xea8JAw864()
	{
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		if (!config.RestEnabled)
		{
			machine.RestUntilTick = 0L;
			machine.RestStartedTick = 0L;
			machine.RestZone = 0u;
			machine.RestDismounted = false;
			return false;
		}
		if (machine.RestUntilTick > 0)
		{
			machine.CurrentPhase = Phase.Rest;
			if (machine.RestZone != GameState.TerritoryType)
			{
				QO98N7kSGo();
				i2yv995ltv();
				if (Environment.TickCount64 - machine.RestStartedTick < 60000)
				{
					return true;
				}
				machine.RestUntilTick = 0L;
				machine.RestStartedTick = 0L;
				machine.RestZone = 0u;
				machine.RestDismounted = false;
				machine.NextRestTick = Environment.TickCount64 + 300000;
				return false;
			}
			if (Environment.TickCount64 < machine.RestUntilTick)
			{
				QO98N7kSGo();
				i2yv995ltv();
				TargetManager.Target = null;
				if (!machine.RestDismounted)
				{
					if (IConditionExtension.get_IsOnMount(DalamudServiceExtension.Instance<ICondition>()))
					{
						B1svjugXkl();
					}
					else
					{
						machine.RestDismounted = true;
					}
				}
				return true;
			}
			machine.RestUntilTick = 0L;
			machine.RestStartedTick = 0L;
			machine.RestZone = 0u;
			machine.RestDismounted = false;
			machine.NextRestTick = Environment.TickCount64 + A2S8GsoRRH();
			OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4753039A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_790eb00575b3425495c924ac13469f4c))));
			return false;
		}
		if (machine.NextRestTick == 0L)
		{
			machine.NextRestTick = Environment.TickCount64 + A2S8GsoRRH();
			return false;
		}
		if (Environment.TickCount64 < machine.NextRestTick)
		{
			return false;
		}
		if (machine.DutyStep != DutyEscapeStep.None)
		{
			return false;
		}
		if (machine.ExchangeStage != ExchangeStep.None)
		{
			return false;
		}
		if (((FateManager)FateManager.Instance()).CurrentFate != null)
		{
			return false;
		}
		if (config.ExchangeItem != null && LocalPlayerState.GetItemCount(26807u) >= config.ExchangeThreshold)
		{
			return false;
		}
		TerritoryType territoryTypeData = GameState.TerritoryTypeData;
		if (((TerritoryType)(ref territoryTypeData)).TerritoryIntendedUse.RowId != 0 && !E6H8YwDrd1())
		{
			return false;
		}
		ryn8r3LHb3();
		return true;
	}

	private void ryn8r3LHb3()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		QO98N7kSGo();
		i2yv995ltv();
		TargetManager.Target = null;
		TerritoryType territoryTypeData = GameState.TerritoryTypeData;
		if (((TerritoryType)(ref territoryTypeData)).TerritoryIntendedUse.RowId == 0)
		{
			machine.RestZone = GameState.TerritoryType;
			goto IL_0102;
		}
		uint num = ((config.RestRandomZone || config.RestZone == 0) ? oh9825RA8b() : config.RestZone);
		if (num != 0)
		{
			AetheryteRecord nearestAetheryte = OmenServiceBase<AetheryteRecordManager>.Instance().GetNearestAetheryte(num, Vector3.Zero, excludeAethernet: true);
			if ((object)nearestAetheryte != null)
			{
				if (!Throttler.Shared.Throttle(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xBA46B54 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2edd08450a8b42e6997855e5217d6d55), 10000u))
				{
					return;
				}
				nearestAetheryte.TeleportTo();
				machine.RestZone = num;
				goto IL_0102;
			}
		}
		machine.NextRestTick = Environment.TickCount64 + 300000;
		return;
		IL_0102:
		uint num2 = zG5869sOan();
		machine.RestUntilTick = Environment.TickCount64 + (long)num2 * 60000L;
		machine.RestStartedTick = Environment.TickCount64;
		machine.RestDismounted = false;
		OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x64E59E28 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f53034f3efc64a309b10c7885bf5d9bd), num2, LuminaWrapper.GetZonePlaceName(machine.RestZone))));
	}

	private long A2S8GsoRRH()
	{
		return Cjq8khOBHv((long)config.RestIntervalMinutes * 60000L);
	}

	private uint zG5869sOan()
	{
		return (uint)Math.Clamp(Cjq8khOBHv((long)Math.Max(1u, config.RestMinutes) * 60000L) / 60000, 1L, 4294967295L);
	}

	private long Cjq8khOBHv(long baseMs)
	{
		uint num = Math.Clamp(config.RestJitterPercent, 0u, 100u);
		if (num == 0)
		{
			return baseMs;
		}
		int num2 = (int)Math.Min(baseMs * num / 100, 2147483647L);
		return baseMs + Random.Shared.Next(-num2, num2 + 1);
	}

	private uint oh9825RA8b()
	{
		uint[] array = RestCities.Where((uint z) => OmenServiceBase<AetheryteRecordManager>.Instance().GetNearestAetheryte(z, Vector3.Zero, excludeAethernet: true) != null).ToArray();
		if (array.Length != 0)
		{
			return array[Random.Shared.Next(array.Length)];
		}
		return 0u;
	}

	protected override void OverlayUI()
	{
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_040c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Unknown result type (might be due to invalid IL or missing references)
		//IL_0459: Unknown result type (might be due to invalid IL or missing references)
		//IL_04df: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_053c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0686: Unknown result type (might be due to invalid IL or missing references)
		//IL_0698: Unknown result type (might be due to invalid IL or missing references)
		//IL_069d: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0734: Unknown result type (might be due to invalid IL or missing references)
		//IL_0786: Unknown result type (might be due to invalid IL or missing references)
		//IL_078b: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c7: Unknown result type (might be due to invalid IL or missing references)
		if (!isOnAutoFate && !FateZones.Contains(GameState.TerritoryType))
		{
			((Window)base.Overlay).IsOpen = false;
			return;
		}
		using (OmenServiceBase<FontManager>.Instance().UIFont80.Push())
		{
			using (ImRaiiExtension.Heading1(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xF201DDB ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_fea4ad1b13de4c4bbc22f571ceda7372))))
			{
				pdR8Z18XuC(isFromOverlay: true);
			}
			ImGui.NewLine();
			using (ImRaiiExtension.Heading1(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x18CC9298 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_33babcee9ecc460a9e530039fd18d361))))
			{
				TableDisposable val = ImRaii.Table(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x454E7E59 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_09da05f368544962acb62b42d9155467)), 2);
				try
				{
					if (TableDisposable.op_Implicit(val))
					{
						ImGui.TableSetupColumn(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x45E44521 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1904f95d2a394c929e1a6d4f72943ad7)), (ImGuiTableColumnFlags)4, 0f, 0u);
						ImGui.TableSetupColumn(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x27D1C24D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_39d5d8025f5249e6b489bc374eefbb3c)), (ImGuiTableColumnFlags)4, 0f, 0u);
						ImGui.TableNextRow();
						ImGui.TableNextColumn();
						ImU8String val2 = new ImU8String(0, 1);
						((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2CE5AE17 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c9dc773883f149b7befba3bdbeac55f7)));
						ImGui.TextUnformatted(val2);
						ImGui.TableNextColumn();
						ImU8String val3 = new ImU8String(0, 1);
						((ImU8String)(ref val3)).AppendFormatted<Phase>(machine.CurrentPhase);
						ImGui.TextUnformatted(val3);
						ImGui.TableNextRow();
						if (Throttler.Shared.Throttle(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x76EB121B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b988ddaee02846b69dd4e446b5da9de9), 2000u))
						{
							uiContext.CachedFate = (tracker.VnDmfLR0qH(out sJrcoAmyE6mj74ckFMl fate) ? fate : null);
						}
						ImGui.TableNextColumn();
						ImU8String val4 = new ImU8String(0, 1);
						((ImU8String)(ref val4)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x18CC2C5A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_33babcee9ecc460a9e530039fd18d361)));
						ImGui.TextUnformatted(val4);
						ImGui.TableNextColumn();
						ImU8String val5 = new ImU8String(0, 1);
						((ImU8String)(ref val5)).AppendFormatted<string>(uiContext.CachedFate?.Name);
						ImGui.TextUnformatted(val5);
					}
				}
				finally
				{
					((TableDisposable)(ref val)).Dispose();
				}
			}
			ImGui.NewLine();
			using (ImRaiiExtension.Heading1(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7FD8F06F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1eab987c1db64b928220e27bd03141d3)) + LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x32D8FCD ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_86d75c161ced498ab6a767d2eb82d38e) + FateModeLoc.GetValueOrDefault(config.CurrentMode, default((string, string))).Title))
			{
				TableDisposable val6 = ImRaii.Table(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7FD8FF93 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1eab987c1db64b928220e27bd03141d3)), 2);
				try
				{
					if (!TableDisposable.op_Implicit(val6))
					{
						return;
					}
					ImGui.TableSetupColumn(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6E00D210 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c40146824d9043e0906f4e1889cf39aa)), (ImGuiTableColumnFlags)4, 0f, 0u);
					ImGui.TableSetupColumn(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x1C1706B3 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_accf3844f1c6464481df1849f0aee1bb)), (ImGuiTableColumnFlags)4, 0f, 0u);
					ImGui.TableNextRow();
					if (Throttler.Shared.Throttle(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x47530CB4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_790eb00575b3425495c924ac13469f4c), 2000u))
					{
						uiContext.CachedBicolorGemstoneCount = LocalPlayerState.GetItemCount(26807u);
					}
					ImGui.TableNextColumn();
					ImGui.TextUnformatted(ImU8String.op_Implicit(LuminaWrapper.GetItemName(26807u)));
					ImGui.TableNextColumn();
					ImU8String val7 = new ImU8String(3, 2);
					((ImU8String)(ref val7)).AppendFormatted<uint>(uiContext.CachedBicolorGemstoneCount);
					((ImU8String)(ref val7)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6854C35D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_23522a2a14314da28eb44187a09cd0b5));
					((ImU8String)(ref val7)).AppendFormatted<uint>(config.ExchangeThreshold);
					ImGui.TextUnformatted(val7);
					switch (config.CurrentMode)
					{
					case FateMode.MultiZone:
						ImGui.TableNextRow();
						{
							foreach (uint multiZone in config.MultiZones)
							{
								if (FateAchievements.ZoneToAchievement.TryGetValue(multiZone, out var value3) && OmenServiceBase<AchievementManager>.Instance().TryGetAchievement(value3, out AchievementInfo achievementInfo))
								{
									ImGui.TableNextRow();
									ImGui.TableNextColumn();
									ImGui.TextUnformatted(ImU8String.op_Implicit(LuminaWrapper.GetZonePlaceName(multiZone)));
									ImGui.TableNextColumn();
									ImU8String val12 = new ImU8String(3, 2);
									((ImU8String)(ref val12)).AppendFormatted<uint>(achievementInfo.Current);
									((ImU8String)(ref val12)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x591C3817 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_20724779fecb4a91959b8028d04d1118));
									((ImU8String)(ref val12)).AppendFormatted<uint>(achievementInfo.Max);
									ImGui.TextUnformatted(val12);
								}
							}
							break;
						}
					case FateMode.Custom:
					{
						CustomPreset customPreset = f9x87jJkaf();
						if (customPreset == null)
						{
							break;
						}
						uint key;
						uint value;
						if (Throttler.Shared.Throttle(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x43D7B5E4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_974d377e1ed74986b81c34c95c2cc846), 2000u))
						{
							foreach (CustomZoneRule value4 in customPreset.Zones.Values)
							{
								foreach (KeyValuePair<uint, uint> item in value4.Items)
								{
									item.Deconstruct(out key, out value);
									uint num = key;
									uiContext.CachedItemCounts[num] = LocalPlayerState.GetItemCount(num);
								}
							}
						}
						{
							foreach (KeyValuePair<uint, CustomZoneRule> zone in customPreset.Zones)
							{
								zone.Deconstruct(out value, out var value2);
								uint num2 = value;
								CustomZoneRule customZoneRule = value2;
								ImGui.TableNextRow();
								ImGui.TableNextColumn();
								ImGui.TextUnformatted(ImU8String.op_Implicit(LuminaWrapper.GetZonePlaceName(num2)));
								ImGui.TableNextColumn();
								ImU8String val8 = new ImU8String(3, 2);
								((ImU8String)(ref val8)).AppendFormatted<uint>(machine.GLZBdHAZpr(num2));
								((ImU8String)(ref val8)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3C148DBF ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_d71d1e134fde413fa0bee3b2419181b6));
								((ImU8String)(ref val8)).AppendFormatted<uint>(customZoneRule.RequiredFateCount);
								ImGui.TextUnformatted(val8);
								foreach (KeyValuePair<uint, uint> item2 in customZoneRule.Items)
								{
									item2.Deconstruct(out value, out key);
									uint num3 = value;
									uint num4 = key;
									ImGui.TableNextRow();
									ImGui.TableNextColumn();
									IndentDisposable val9 = ImRaii.PushIndent(1, true);
									try
									{
										ImGui.TextUnformatted(ImU8String.op_Implicit(LuminaWrapper.GetItemName(num3)));
									}
									finally
									{
										((IDisposable)val9)?.Dispose();
									}
									uint valueOrDefault = uiContext.CachedItemCounts.GetValueOrDefault(num3);
									ImGui.TableNextColumn();
									ColorDisposable val10 = ImRaii.PushColor((ImGuiCol)0, KnownColor.LimeGreen.ToUInt(), valueOrDefault >= num4);
									try
									{
										ImU8String val11 = new ImU8String(1, 2);
										((ImU8String)(ref val11)).AppendFormatted<uint>(valueOrDefault);
										((ImU8String)(ref val11)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3C14CD35 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_d71d1e134fde413fa0bee3b2419181b6));
										((ImU8String)(ref val11)).AppendFormatted<uint>(num4);
										ImGui.TextUnformatted(val11);
									}
									finally
									{
										((IDisposable)val10)?.Dispose();
									}
								}
							}
							break;
						}
					}
					}
				}
				finally
				{
					((TableDisposable)(ref val6)).Dispose();
				}
			}
		}
	}

	protected unsafe override void ConfigUI()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_0540: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_0427: Unknown result type (might be due to invalid IL or missing references)
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_077f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0784: Unknown result type (might be due to invalid IL or missing references)
		//IL_0789: Unknown result type (might be due to invalid IL or missing references)
		//IL_0582: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_078b: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_085f: Unknown result type (might be due to invalid IL or missing references)
		//IL_08cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_091f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0951: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0603: Unknown result type (might be due to invalid IL or missing references)
		//IL_063f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a7e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b15: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b1a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b9f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bc6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bcb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c16: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c38: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c3e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c43: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c45: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c77: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c9c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e51: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e56: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ea8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eb9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ebf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d1e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d23: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d38: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d3d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0faa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fe4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1000: Unknown result type (might be due to invalid IL or missing references)
		//IL_1006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d54: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d68: Unknown result type (might be due to invalid IL or missing references)
		//IL_10dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_1124: Unknown result type (might be due to invalid IL or missing references)
		//IL_1129: Unknown result type (might be due to invalid IL or missing references)
		//IL_1180: Unknown result type (might be due to invalid IL or missing references)
		//IL_1092: Unknown result type (might be due to invalid IL or missing references)
		//IL_11cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_11d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_121d: Unknown result type (might be due to invalid IL or missing references)
		//IL_122e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1234: Unknown result type (might be due to invalid IL or missing references)
		//IL_1276: Unknown result type (might be due to invalid IL or missing references)
		//IL_127b: Unknown result type (might be due to invalid IL or missing references)
		//IL_12cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_12dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_12e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1325: Unknown result type (might be due to invalid IL or missing references)
		//IL_132a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1376: Unknown result type (might be due to invalid IL or missing references)
		//IL_1387: Unknown result type (might be due to invalid IL or missing references)
		//IL_138d: Unknown result type (might be due to invalid IL or missing references)
		//IL_14a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_14c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_14c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1518: Unknown result type (might be due to invalid IL or missing references)
		BattleChara* localPlayer = Control.GetLocalPlayer();
		if (localPlayer == null)
		{
			return;
		}
		TabBarDisposable val = ImRaii.TabBar(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7C2FF67C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_12c2d68ba4654622a4959ee73cacc6d7)));
		try
		{
			TabItemDisposable val2 = ImRaii.TabItem(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6E002D16 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c40146824d9043e0906f4e1889cf39aa))));
			try
			{
				if (TabItemDisposable.op_Implicit(val2))
				{
					ImGui.Spacing();
					Vector4 vector = KnownColor.Chocolate.ToVector4();
					ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x535750C2 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_0676edd1b45a4ff096830fd6229bc1fb))));
					IndentDisposable val3 = ImRaii.PushIndent(1, true);
					try
					{
						ImGui.Bullet();
						ImGui.SameLine();
						ImU8String val4 = new ImU8String(10, 1);
						((ImU8String)(ref val4)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6C35BF8C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88a2dcd1996b4a3fbefaf51a98b59def));
						((ImU8String)(ref val4)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3AC9C8F6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3932528062874ea4bb7e2e1185394e37)));
						ImGui.TextUnformatted(val4);
						ImGui.Bullet();
						ImGui.SameLine();
						ImU8String val5 = new ImU8String(17, 1);
						((ImU8String)(ref val5)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6854641F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_23522a2a14314da28eb44187a09cd0b5));
						((ImU8String)(ref val5)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x39871699 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_56ae6e3f2cdf4dd8a743f4d493ce49a5)));
						ImGui.TextUnformatted(val5);
						ImGui.Bullet();
						ImGui.SameLine();
						ImU8String val6 = new ImU8String(2, 2);
						((ImU8String)(ref val6)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2194CEB6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cd1b319f326a44d1aaea645e95d8698b)));
						((ImU8String)(ref val6)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D752662 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8b51f8019ce49018b38e06104a97fc1));
						((ImU8String)(ref val6)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--1470179789 ^ 0x4577F73D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7fcf5a71a7ed445197f132979519c560)));
						ImGui.TextUnformatted(val6);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					ImGui.NewLine();
					vector = KnownColor.Chocolate.ToVector4();
					ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3C1429A5 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_d71d1e134fde413fa0bee3b2419181b6))));
					val3 = ImRaii.PushIndent(1, true);
					try
					{
						ImGui.Bullet();
						ImGui.SameLine();
						ImU8String val7 = new ImU8String(17, 1);
						((ImU8String)(ref val7)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xF20DF4F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_fea4ad1b13de4c4bbc22f571ceda7372));
						((ImU8String)(ref val7)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x12D6DF9C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7fcf5a71a7ed445197f132979519c560)));
						ImGui.TextWrapped(val7);
						ImGui.Bullet();
						ImGui.SameLine();
						ImU8String val8 = new ImU8String(16, 1);
						((ImU8String)(ref val8)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x258F128A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_dadeb2ab61424d31be7cc1966e675825));
						((ImU8String)(ref val8)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D7584E8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8b51f8019ce49018b38e06104a97fc1)));
						ImGui.TextWrapped(val8);
						ImGui.Bullet();
						ImGui.SameLine();
						ImU8String val9 = new ImU8String(12, 1);
						((ImU8String)(ref val9)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5E3B59AA ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2d7d8a5df6744c51b35f486f03b53ded));
						((ImU8String)(ref val9)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x17782DA3 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f7cf93b006904393bb56d82c0a688fa1)));
						ImGui.TextWrapped(val9);
						ImGui.Bullet();
						ImGui.SameLine();
						ImU8String val10 = new ImU8String(10, 1);
						((ImU8String)(ref val10)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--367535148 ^ 0x440D2824 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a90a92322bff42e180982e106200d4a3));
						((ImU8String)(ref val10)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x44278B0A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8dfe57222f64fa68d9d16c8b6fe9d0b)));
						ImGui.TextWrapped(val10);
						ImGui.Bullet();
						ImGui.SameLine();
						ImU8String val11 = new ImU8String(2, 2);
						((ImU8String)(ref val11)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-224923271 ^ -56709422 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_aba6092d7cea44a49d1667540500e363)));
						((ImU8String)(ref val11)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6AAE0D68 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_839f542231154d29b0562b687afa328a));
						((ImU8String)(ref val11)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x76EB1739 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b988ddaee02846b69dd4e446b5da9de9)));
						ImGui.TextWrapped(val11);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
			}
			finally
			{
				((TabItemDisposable)(ref val2)).Dispose();
			}
			TabItemDisposable val12 = ImRaii.TabItem(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x66B3D2C4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_762f55d0e7ef4ecd8f3f55c0f8a10fae))));
			try
			{
				if (TabItemDisposable.op_Implicit(val12))
				{
					ImGui.Spacing();
					pdR8Z18XuC();
					ImGui.NewLine();
					Vector4 vector = KnownColor.Chocolate.ToVector4();
					ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x423ED8F0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_6368ba44fa4749c09da09b5ebf853b71))));
					DisabledDisposable val13 = ImRaii.Disabled(isOnAutoFate);
					try
					{
						ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
						ImU8String val14 = ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6FF16332 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5859f4ea3042459db2c9b610b3099912));
						FrozenDictionary<FateMode, (string Title, string Description)> fateModeLoc = FateModeLoc;
						FateMode currentMode = config.CurrentMode;
						(string, string) value = default((string, string));
						ComboDisposable val15 = ImRaii.Combo(val14, ImU8String.op_Implicit(fateModeLoc.GetValueOrDefault(currentMode, value).Title), (ImGuiComboFlags)8);
						try
						{
							if (ComboDisposable.op_Implicit(val15))
							{
								foreach (KeyValuePair<FateMode, (string, string)> item3 in FateModeLoc)
								{
									item3.Deconstruct(out var key, out value);
									(string, string) tuple = value;
									FateMode fateMode = key;
									string item = tuple.Item1;
									string item2 = tuple.Item2;
									ImU8String val16 = new ImU8String(2, 2);
									((ImU8String)(ref val16)).AppendFormatted<string>(item);
									((ImU8String)(ref val16)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x591C20BF ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_20724779fecb4a91959b8028d04d1118));
									((ImU8String)(ref val16)).AppendFormatted<FateMode>(fateMode);
									if (ImGui.Selectable(val16, config.CurrentMode == fateMode, (ImGuiSelectableFlags)0, default(Vector2)))
									{
										config.CurrentMode = fateMode;
										config.Save(this);
									}
									ImGuiOm.TooltipHover(item2, 20f * Globals.GlobalUIScale);
								}
							}
						}
						finally
						{
							((ComboDisposable)(ref val15)).Dispose();
						}
					}
					finally
					{
						((IDisposable)val13)?.Dispose();
					}
					IndentDisposable val3 = ImRaii.PushIndent(1, true);
					try
					{
						val13 = ImRaii.Disabled(isOnAutoFate);
						try
						{
							switch (config.CurrentMode)
							{
							case FateMode.SingleZone:
								Uq28aTFPj2();
								break;
							case FateMode.MainSubZone:
								aDB80j9K9b();
								break;
							case FateMode.MultiZone:
								BFG855REJk();
								break;
							case FateMode.Custom:
								dee8RSwpXU();
								break;
							}
						}
						finally
						{
							((IDisposable)val13)?.Dispose();
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
			}
			finally
			{
				((TabItemDisposable)(ref val12)).Dispose();
			}
			TabItemDisposable val17 = ImRaii.TabItem(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489F8CDC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b2294f2f9bb24a76b00cf1af75888414))));
			try
			{
				if (!TabItemDisposable.op_Implicit(val17))
				{
					return;
				}
				ImGui.Spacing();
				ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
				if (ImGui.InputText(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x66B36962 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_762f55d0e7ef4ecd8f3f55c0f8a10fae))), ref config.ActiveAttackCommand, 1028, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null))
				{
					config.Save(this);
				}
				ImGuiOm.TooltipHover(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x12D6DA8C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7fcf5a71a7ed445197f132979519c560)), 20f * Globals.GlobalUIScale);
				ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
				if (ImGui.InputText(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5351147A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_9085260e6b1a40438d219e85182b5d08))), ref config.DeactiveAttackCommand, 1028, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null))
				{
					config.Save(this);
				}
				ImGuiOm.TooltipHover(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x420C522C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_84ee025b19bb4145ba9b4b6b7acb91eb)), 20f * Globals.GlobalUIScale);
				ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
				ImU8String val18 = new ImU8String(4, 1);
				((ImU8String)(ref val18)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x43D7B3B8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_974d377e1ed74986b81c34c95c2cc846)));
				((ImU8String)(ref val18)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x423E203A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_6368ba44fa4749c09da09b5ebf853b71));
				if (ImGui.SliderFloat(val18, ref config.LowHealthThreshold, 5f, 30f, ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6FF161AC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5859f4ea3042459db2c9b610b3099912)), (ImGuiSliderFlags)0))
				{
					config.Save(this);
				}
				ImGuiOm.TooltipHover(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2194CB8C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cd1b319f326a44d1aaea645e95d8698b)), 20f * Globals.GlobalUIScale);
				ImU8String val19 = new ImU8String(21, 1);
				((ImU8String)(ref val19)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x1C17F1C7 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_accf3844f1c6464481df1849f0aee1bb)));
				((ImU8String)(ref val19)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x68ACE3DC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4722689f1a4d41dc9dc800bfe1a89c09));
				if (ImGui.Checkbox(val19, ref config.IgnoreFarLevelFates))
				{
					config.Save(this);
				}
				ImGuiOm.TooltipHover(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x477E9B5B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_ba27244341a5414496a87eae29cf3dec)), 20f * Globals.GlobalUIScale);
				ImGui.NewLine();
				Vector4 vector = KnownColor.Chocolate.ToVector4();
				ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x68ACFC4A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4722689f1a4d41dc9dc800bfe1a89c09))));
				ImGui.SetNextItemWidth(300f * Globals.GlobalUIScale);
				if (uiContext.FateCombo.DrawCheckbox())
				{
					config.BlacklistedFates = uiContext.FateCombo.SelectedIDs;
					config.Save(this);
				}
				ImGuiOm.TooltipHover(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x12D6C4AE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7fcf5a71a7ed445197f132979519c560)), 20f * Globals.GlobalUIScale);
				ImGui.NewLine();
				vector = KnownColor.Chocolate.ToVector4();
				ImU8String val20 = new ImU8String(3, 2);
				((ImU8String)(ref val20)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--1470179789 ^ 0x6134A010 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_26e231ba68f74e8784503e099ddfc2d9)));
				((ImU8String)(ref val20)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x51E58C9A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a90a92322bff42e180982e106200d4a3));
				((ImU8String)(ref val20)).AppendFormatted<string>(LuminaWrapper.GetItemName(26807u));
				((ImU8String)(ref val20)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5E3BDE4C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2d7d8a5df6744c51b35f486f03b53ded));
				ImGui.TextColored(ref vector, val20);
				Tx5DWgBFgaOdVwt3xBP exchangeItem = config.ExchangeItem;
				ImGui.SetNextItemWidth(300f * Globals.GlobalUIScale);
				ImU8String val21 = new ImU8String(20, 1);
				((ImU8String)(ref val21)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x18CC348C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_33babcee9ecc460a9e530039fd18d361)));
				((ImU8String)(ref val21)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6AAE5EC4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_839f542231154d29b0562b687afa328a));
				ComboDisposable val22 = ImRaii.Combo(val21, ImU8String.op_Implicit(exchangeItem?.ItemName ?? string.Empty), (ImGuiComboFlags)8);
				try
				{
					if (ComboDisposable.op_Implicit(val22))
					{
						ImGui.SetNextItemWidth(-1f);
						ImGui.InputTextWithHint(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-1003249140 ^ -1142180585 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1eab987c1db64b928220e27bd03141d3)), ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x23104997 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a96fab9e97944b39a8afe3fb1109ba7d))), ref uiContext.ExchangeItemSearchInput, 256, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
						bool flag = string.IsNullOrEmpty(uiContext.ExchangeItemSearchInput);
						ISharedImmediateTexture val24 = default(ISharedImmediateTexture);
						foreach (Tx5DWgBFgaOdVwt3xBP exchangeItem2 in ExchangeItems)
						{
							if (!flag && !exchangeItem2.ItemName.Contains(uiContext.ExchangeItemSearchInput, StringComparison.OrdinalIgnoreCase))
							{
								continue;
							}
							ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
							Item value2 = LuminaGetter.GetRow<Item>(exchangeItem2.ItemID).Value;
							GameIconLookup val23 = new GameIconLookup((uint)((Item)(ref value2)).Icon, false, true, (ClientLanguage?)null);
							if (!obj.TryGetFromGameIcon(ref val23, ref val24))
							{
								continue;
							}
							IdDisposable val25 = ImRaii.PushId(ImU8String.op_Implicit(exchangeItem2.ToString()), true);
							try
							{
								if (ImGuiOm.SelectableImageWithText(val24.GetWrapOrEmpty().Handle, new Vector2(ImGui.GetTextLineHeightWithSpacing()), exchangeItem2.ItemName, exchangeItem2 == exchangeItem, (ImGuiSelectableFlags)0))
								{
									config.ExchangeItem = exchangeItem2;
									config.Save(this);
								}
							}
							finally
							{
								((IDisposable)val25)?.Dispose();
							}
						}
					}
				}
				finally
				{
					((ComboDisposable)(ref val22)).Dispose();
				}
				ImGui.SameLine();
				if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)62189, LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x109CE5B4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_44b9808560124617907f0048883b8948))))
				{
					config.ExchangeItem = null;
					config.Save(this);
				}
				ImGui.SetNextItemWidth(300f * Globals.GlobalUIScale);
				ImU8String val26 = new ImU8String(10, 1);
				((ImU8String)(ref val26)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--1317373447 ^ 0x41A5F088 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_fea4ad1b13de4c4bbc22f571ceda7372)));
				((ImU8String)(ref val26)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6AAEB4A4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_839f542231154d29b0562b687afa328a));
				ImU8String val27 = val26;
				ref uint exchangeThreshold = ref config.ExchangeThreshold;
				ImU8String val28 = default(ImU8String);
				if (ImGui.InputUInt(val27, ref exchangeThreshold, 0u, 0u, val28, (ImGuiInputTextFlags)0))
				{
					config.ExchangeThreshold = Math.Clamp(config.ExchangeThreshold, 0u, 1400u);
				}
				if (ImGui.IsItemDeactivatedAfterEdit())
				{
					config.Save(this);
				}
				ImGui.SetNextItemWidth(300f * Globals.GlobalUIScale);
				if (uiContext.ExchangeZoneCombo.DrawRadio())
				{
					config.ExchangeZone = uiContext.ExchangeZoneCombo.SelectedID;
					config.Save(this);
				}
				ImGuiOm.TooltipHover(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489B292C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cade14e68bb84906959f3cda5c496848)));
				ImGui.SameLine();
				ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x21947AC4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cd1b319f326a44d1aaea645e95d8698b))));
				ImGui.SetNextItemWidth(300f * Globals.GlobalUIScale);
				ImU8String val29 = ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x23103367 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a96fab9e97944b39a8afe3fb1109ba7d)));
				ref Vector3 exchangePosition = ref config.ExchangePosition;
				val28 = default(ImU8String);
				ImGui.InputFloat3(val29, ref exchangePosition, 0f, 0f, val28, (ImGuiInputTextFlags)0);
				if (ImGui.IsItemDeactivatedAfterEdit())
				{
					config.Save(this);
				}
				ImGuiOm.TooltipHover(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7FD8169F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1eab987c1db64b928220e27bd03141d3)));
				ImGui.SameLine();
				if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61732, LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x535748A2 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_0676edd1b45a4ff096830fd6229bc1fb))))
				{
					config.ExchangePosition = Vector3.op_Implicit(((BattleChara)localPlayer).Position);
					config.Save(this);
				}
				ImGui.NewLine();
				vector = KnownColor.Chocolate.ToVector4();
				ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x317C00B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_99910d0c1e594d99b604e0aab1195273))));
				ImGuiOm.HelpMarker(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5DD7BAAD ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c7355392ad9e43b6b82798cc42ea6c6f)), 20f * Globals.GlobalUIScale, (FontAwesomeIcon)61530);
				val28 = new ImU8String(6, 1);
				((ImU8String)(ref val28)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7FD81661 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1eab987c1db64b928220e27bd03141d3)));
				((ImU8String)(ref val28)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x53510BC8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_9085260e6b1a40438d219e85182b5d08));
				if (ImGui.Checkbox(val28, ref config.RestEnabled))
				{
					config.Save(this);
				}
				ItemWidthDisposable val30 = ImRaii.ItemWidth(300f * Globals.GlobalUIScale);
				try
				{
					DisabledDisposable val13 = ImRaii.Disabled(!config.RestEnabled);
					try
					{
						ImU8String val31 = new ImU8String(15, 1);
						((ImU8String)(ref val31)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489B2832 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cade14e68bb84906959f3cda5c496848)));
						((ImU8String)(ref val31)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D759D48 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8b51f8019ce49018b38e06104a97fc1));
						ImU8String val32 = val31;
						ref uint restIntervalMinutes = ref config.RestIntervalMinutes;
						ImU8String val33 = default(ImU8String);
						if (ImGui.InputUInt(val32, ref restIntervalMinutes, 0u, 0u, val33, (ImGuiInputTextFlags)0))
						{
							config.RestIntervalMinutes = Math.Max(1u, config.RestIntervalMinutes);
						}
						if (ImGui.IsItemDeactivatedAfterEdit())
						{
							config.Save(this);
						}
						val33 = new ImU8String(15, 1);
						((ImU8String)(ref val33)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5017C590 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e227921f7ee94b6aa1c28169395ba0cf)));
						((ImU8String)(ref val33)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x535108A6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_9085260e6b1a40438d219e85182b5d08));
						ImU8String val34 = val33;
						ref uint restMinutes = ref config.RestMinutes;
						ImU8String val35 = default(ImU8String);
						if (ImGui.InputUInt(val34, ref restMinutes, 0u, 0u, val35, (ImGuiInputTextFlags)0))
						{
							config.RestMinutes = Math.Max(1u, config.RestMinutes);
						}
						if (ImGui.IsItemDeactivatedAfterEdit())
						{
							config.Save(this);
						}
						val35 = new ImU8String(13, 1);
						((ImU8String)(ref val35)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4CF2B314 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a07aac30eb32484e94cb4d1e62117765)));
						((ImU8String)(ref val35)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x39870DE9 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_56ae6e3f2cdf4dd8a743f4d493ce49a5));
						ImU8String val36 = val35;
						ref uint restJitterPercent = ref config.RestJitterPercent;
						ImU8String val37 = default(ImU8String);
						if (ImGui.InputUInt(val36, ref restJitterPercent, 0u, 0u, val37, (ImGuiInputTextFlags)0))
						{
							config.RestJitterPercent = Math.Clamp(config.RestJitterPercent, 0u, 100u);
						}
						if (ImGui.IsItemDeactivatedAfterEdit())
						{
							config.Save(this);
						}
						ImGuiOm.HelpMarker(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3C143189 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_d71d1e134fde413fa0bee3b2419181b6)), 20f * Globals.GlobalUIScale, (FontAwesomeIcon)61530);
						ImGui.Spacing();
						DisabledDisposable val38 = ImRaii.Disabled(config.RestRandomZone);
						try
						{
							ImGui.SetNextItemWidth(300f * Globals.GlobalUIScale);
							if (uiContext.RestZoneCombo.DrawRadio())
							{
								config.RestZone = uiContext.RestZoneCombo.SelectedID;
								config.Save(this);
							}
						}
						finally
						{
							((IDisposable)val38)?.Dispose();
						}
						ImGui.SameLine();
						ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--664844326 ^ 0x24B77A85 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_99910d0c1e594d99b604e0aab1195273))));
						ImGui.SameLine(0f, 4f * Globals.GlobalUIScale);
						val37 = new ImU8String(6, 1);
						((ImU8String)(ref val37)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x177832F3 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f7cf93b006904393bb56d82c0a688fa1)));
						((ImU8String)(ref val37)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x423E3ECE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_6368ba44fa4749c09da09b5ebf853b71));
						if (ImGui.Checkbox(val37, ref config.RestRandomZone))
						{
							config.Save(this);
						}
					}
					finally
					{
						((IDisposable)val13)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)val30)?.Dispose();
				}
			}
			finally
			{
				((TabItemDisposable)(ref val17)).Dispose();
			}
		}
		finally
		{
			((TabBarDisposable)(ref val)).Dispose();
		}
	}

	private void pdR8Z18XuC(bool isFromOverlay = false)
	{
		DisabledDisposable val = ImRaii.Disabled(isOnAutoFate);
		try
		{
			if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61515, LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6FF1C63A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5859f4ea3042459db2c9b610b3099912))))
			{
				machine.C9qmnWPknt();
				tracker.pLjmSIgpim();
				machine.f8LBXu2uHx(GameState.TerritoryType);
				machine.nnbBUCfrhJ(GameState.TerritoryType);
				if (config.RestEnabled)
				{
					machine.NextRestTick = Environment.TickCount64 + A2S8GsoRRH();
				}
				isOnAutoFate = true;
				((Window)base.Overlay).IsOpen = true;
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		ImGui.SameLine();
		if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61517, LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6854C539 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_23522a2a14314da28eb44187a09cd0b5))))
		{
			q5q8UWZLbj();
			TargetManager.Target = null;
			isOnAutoFate = false;
		}
		if (isFromOverlay)
		{
			if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61459, LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xE09A8B3 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_aba6092d7cea44a49d1667540500e363))))
			{
				OmenServiceBase<ChatManager>.Instance().SendCommand(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x469ABD29 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_bb1eb496bad948e4a2baaa3e1edc7a92));
			}
			ImGui.SameLine();
			if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)62480, LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3172DC3 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_99910d0c1e594d99b604e0aab1195273))))
			{
				((Window)base.Overlay).IsOpen = false;
			}
		}
	}

	private void Uq28aTFPj2()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3AC97FC0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3932528062874ea4bb7e2e1185394e37))));
		ImGui.SetNextItemWidth(300f * Globals.GlobalUIScale);
		if (uiContext.SingleZoneCombo.DrawRadio())
		{
			config.SingleZone = uiContext.SingleZoneCombo.SelectedID;
			config.Save(this);
		}
	}

	private void aDB80j9K9b()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2DBDA2DA ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5d8d924f1edf4ec39f023b3c7bb646cc))));
		ImGui.SetNextItemWidth(300f * Globals.GlobalUIScale);
		uiContext.MainZoneCombo.SelectedID = config.MainZone;
		if (uiContext.MainZoneCombo.DrawRadio())
		{
			config.MainZone = uiContext.MainZoneCombo.SelectedID;
			config.Save(this);
		}
		ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4C427F64 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4643f8ec0000417ba22db1f06f6757bd))));
		ImGui.SetNextItemWidth(300f * Globals.GlobalUIScale);
		uiContext.SubZoneCombo.SelectedID = config.SubZone;
		if (uiContext.SubZoneCombo.DrawRadio())
		{
			config.SubZone = uiContext.SubZoneCombo.SelectedID;
			config.Save(this);
		}
	}

	private void BFG855REJk()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x469A100D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_bb1eb496bad948e4a2baaa3e1edc7a92))));
		ImGui.SetNextItemWidth(300f * Globals.GlobalUIScale);
		if (uiContext.MultiZoneCombo.DrawCheckbox())
		{
			config.MultiZones = uiContext.MultiZoneCombo.SelectedIDs;
			config.Save(this);
		}
	}

	private void dee8RSwpXU()
	{
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0463: Unknown result type (might be due to invalid IL or missing references)
		//IL_0468: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0579: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_066a: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0901: Unknown result type (might be due to invalid IL or missing references)
		//IL_0906: Unknown result type (might be due to invalid IL or missing references)
		//IL_0916: Unknown result type (might be due to invalid IL or missing references)
		//IL_094b: Unknown result type (might be due to invalid IL or missing references)
		//IL_098c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0997: Unknown result type (might be due to invalid IL or missing references)
		//IL_099d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a06: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c21: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c26: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c54: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c6b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c86: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c8b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c9b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cdc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ced: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d86: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e2d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e6a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f08: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f0d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f40: Unknown result type (might be due to invalid IL or missing references)
		int num = BuiltInCustomPresets.Length + config.CustomPresets.Count;
		if (config.SelectedCustomPresetIndex < 0 || config.SelectedCustomPresetIndex >= num)
		{
			config.SelectedCustomPresetIndex = 0;
		}
		CustomPreset customPreset = f9x87jJkaf();
		bool isSelectedCustomPresetBuiltIn = IsSelectedCustomPresetBuiltIn;
		if (customPreset != null && (uiContext.SelectedCustomZone == 0 || !customPreset.Zones.ContainsKey(uiContext.SelectedCustomZone)))
		{
			uiContext.SelectedCustomZone = customPreset.Zones.Keys.FirstOrDefault();
		}
		TableDisposable val = ImRaii.Table(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x685479B7 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_23522a2a14314da28eb44187a09cd0b5)), 2, (ImGuiTableFlags)513);
		try
		{
			if (!val)
			{
				return;
			}
			ImGui.TableSetupColumn(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5E3B460A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2d7d8a5df6744c51b35f486f03b53ded)), (ImGuiTableColumnFlags)8, 220f * Globals.GlobalUIScale, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-1003249140 ^ -442010934 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cd1b319f326a44d1aaea645e95d8698b)), (ImGuiTableColumnFlags)4, 0f, 0u);
			ImGui.TableNextRow();
			ImGui.TableNextColumn();
			ImGui.TextDisabled(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x454E92EF ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_09da05f368544962acb62b42d9155467))));
			ImGui.Separator();
			for (int i = 0; i < BuiltInCustomPresets.Length; i++)
			{
				ImU8String val2 = new ImU8String(10, 2);
				((ImU8String)(ref val2)).AppendFormatted<string>(BuiltInCustomPresets[i].Name);
				((ImU8String)(ref val2)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4B0F52CC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_8e002b0644ef421698ef6f63772929af));
				((ImU8String)(ref val2)).AppendFormatted<int>(i);
				if (ImGui.Selectable(val2, config.SelectedCustomPresetIndex == i, (ImGuiSelectableFlags)0, default(Vector2)))
				{
					config.SelectedCustomPresetIndex = i;
					uiContext.SelectedCustomZone = 0u;
					config.Save(this);
				}
				if (xpe8DDoah9(BuiltInCustomPresets[i], i))
				{
					return;
				}
			}
			ImGui.NewLine();
			ImGui.AlignTextToFramePadding();
			ImGui.TextDisabled(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--367535148 ^ 0x4F976315 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5154ccd9fed0487f8fa0244531466009))));
			ImGui.SameLine();
			ImGuiOm.RightAlignFor(ImGui.GetFrameHeightWithSpacing() + ImGui.GetFrameHeight());
			GroupDisposable val3 = ImRaii.Group();
			try
			{
				if (ImGuiOm.ButtonIcon(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3C143651 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_d71d1e134fde413fa0bee3b2419181b6), (FontAwesomeIcon)61543))
				{
					List<CustomPreset> customPresets = config.CustomPresets;
					CustomPreset customPreset2 = new CustomPreset();
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
					defaultInterpolatedStringHandler.AppendFormatted(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x9B43C9E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2f1c0cb46b4440d499a69ed8afe22360)));
					defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x9B4A3CE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2f1c0cb46b4440d499a69ed8afe22360));
					defaultInterpolatedStringHandler.AppendFormatted(config.CustomPresets.Count + 1);
					customPreset2.Name = defaultInterpolatedStringHandler.ToStringAndClear();
					customPresets.Add(customPreset2);
					config.SelectedCustomPresetIndex = BuiltInCustomPresets.Length + config.CustomPresets.Count - 1;
					config.Save(this);
				}
				ImGui.SameLine();
				if (ImGuiOm.ButtonIcon(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x423E3A7C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_6368ba44fa4749c09da09b5ebf853b71), (FontAwesomeIcon)62831, LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x420C9B1E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_84ee025b19bb4145ba9b4b6b7acb91eb))))
				{
					CustomPreset customPreset3 = ModuleBase.ImportFromClipboard<CustomPreset>();
					if (customPreset3 != null)
					{
						config.CustomPresets.Add(customPreset3);
						config.SelectedCustomPresetIndex = BuiltInCustomPresets.Length + config.CustomPresets.Count - 1;
						config.Save(this);
					}
				}
			}
			finally
			{
				((GroupDisposable)(ref val3)).Dispose();
			}
			ImGui.Separator();
			for (int j = 0; j < config.CustomPresets.Count; j++)
			{
				int num2 = BuiltInCustomPresets.Length + j;
				ImU8String val4 = new ImU8String(7, 2);
				((ImU8String)(ref val4)).AppendFormatted<string>(config.CustomPresets[j].Name);
				((ImU8String)(ref val4)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-1003249140 ^ -1934835524 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b2294f2f9bb24a76b00cf1af75888414));
				((ImU8String)(ref val4)).AppendFormatted<int>(j);
				if (ImGui.Selectable(val4, config.SelectedCustomPresetIndex == num2, (ImGuiSelectableFlags)0, default(Vector2)))
				{
					config.SelectedCustomPresetIndex = num2;
					uiContext.SelectedCustomZone = 0u;
					config.Save(this);
				}
				if (xpe8DDoah9(config.CustomPresets[j], j))
				{
					return;
				}
			}
			ImGui.TableNextColumn();
			if (customPreset == null)
			{
				return;
			}
			ImGui.TextColored(KnownColor.Chocolate.ToUInt(), ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489B8D5A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cade14e68bb84906959f3cda5c496848))));
			IndentDisposable val5 = ImRaii.PushIndent(1, true);
			try
			{
				if (isSelectedCustomPresetBuiltIn)
				{
					ImGui.TextUnformatted(ImU8String.op_Implicit(BuiltInCustomPresets[config.SelectedCustomPresetIndex].Name));
				}
				else
				{
					string name = customPreset.Name;
					ImGui.SetNextItemWidth(-1f);
					ImGui.InputText(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x12D6C108 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7fcf5a71a7ed445197f132979519c560)), ref name, 256, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
					if (ImGui.IsItemDeactivatedAfterEdit())
					{
						customPreset.Name = name;
						config.Save(this);
					}
				}
			}
			finally
			{
				((IDisposable)val5)?.Dispose();
			}
			ImGui.Spacing();
			DisabledDisposable val6 = ImRaii.Disabled(isSelectedCustomPresetBuiltIn);
			try
			{
				ImGui.TextColored(KnownColor.Chocolate.ToUInt(), ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x369527C9 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_26e231ba68f74e8784503e099ddfc2d9))));
				val5 = ImRaii.PushIndent(1, true);
				uint key;
				try
				{
					if (uiContext.LastSelectedCustomPresetZoneIndex != config.SelectedCustomPresetIndex)
					{
						uiContext.LastSelectedCustomPresetZoneIndex = config.SelectedCustomPresetIndex;
						ZoneSelectCombo customModeZoneCombo = uiContext.CustomModeZoneCombo;
						HashSet<uint> hashSet = new HashSet<uint>();
						foreach (uint key2 in customPreset.Zones.Keys)
						{
							key = key2;
							hashSet.Add(key);
						}
						customModeZoneCombo.SelectedIDs = hashSet;
					}
					ImGui.SetNextItemWidth(-1f);
					if (uiContext.CustomModeZoneCombo.DrawCheckbox())
					{
						_003C_003Ec__DisplayClass113_0 CS_0024_003C_003E8__locals6 = new _003C_003Ec__DisplayClass113_0();
						CS_0024_003C_003E8__locals6.selected = uiContext.CustomModeZoneCombo.SelectedIDs;
						foreach (uint item in customPreset.Zones.Keys.Where((uint z) => !CS_0024_003C_003E8__locals6.selected.Contains(z)).ToList())
						{
							customPreset.Zones.Remove(item);
						}
						foreach (uint item2 in CS_0024_003C_003E8__locals6.selected.Except(customPreset.Zones.Keys))
						{
							customPreset.Zones[item2] = new CustomZoneRule();
						}
						if (uiContext.SelectedCustomZone != 0 && !customPreset.Zones.ContainsKey(uiContext.SelectedCustomZone))
						{
							uiContext.SelectedCustomZone = customPreset.Zones.Keys.FirstOrDefault();
						}
						config.Save(this);
					}
				}
				finally
				{
					((IDisposable)val5)?.Dispose();
				}
				foreach (KeyValuePair<uint, CustomZoneRule> zone in customPreset.Zones)
				{
					zone.Deconstruct(out key, out var value);
					uint num3 = key;
					CustomZoneRule customZoneRule = value;
					ImU8String val7 = new ImU8String(5, 1);
					((ImU8String)(ref val7)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xF20C36F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_fea4ad1b13de4c4bbc22f571ceda7372));
					((ImU8String)(ref val7)).AppendFormatted<uint>(num3);
					IdDisposable val8 = ImRaii.PushId(val7, true);
					try
					{
						ImU8String val9 = new ImU8String(0, 1);
						((ImU8String)(ref val9)).AppendFormatted<string>(LuminaWrapper.GetZonePlaceName(num3));
						if (!ImGui.CollapsingHeader(val9, (ImGuiTreeNodeFlags)0))
						{
							continue;
						}
						ImGui.TextColored(KnownColor.Chocolate.ToUInt(), ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x53510FE0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_9085260e6b1a40438d219e85182b5d08))));
						val5 = ImRaii.PushIndent(1, true);
						ImU8String val11;
						try
						{
							uint requiredFateCount = customZoneRule.RequiredFateCount;
							ImGui.SetNextItemWidth(-1f);
							ImU8String val10 = ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5017C22E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e227921f7ee94b6aa1c28169395ba0cf));
							val11 = default(ImU8String);
							ImGui.InputUInt(val10, ref requiredFateCount, 0u, 0u, val11, (ImGuiInputTextFlags)0);
							if (ImGui.IsItemDeactivatedAfterEdit())
							{
								customZoneRule.RequiredFateCount = requiredFateCount;
								config.Save(this);
							}
						}
						finally
						{
							((IDisposable)val5)?.Dispose();
						}
						ImGui.TextColored(KnownColor.Chocolate.ToUInt(), ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x258F0978 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_dadeb2ab61424d31be7cc1966e675825))));
						val5 = ImRaii.PushIndent(1, true);
						ImU8String val15;
						try
						{
							ItemSelectCombo customModeItemCombo = uiContext.CustomModeItemCombo;
							HashSet<uint> hashSet = new HashSet<uint>();
							foreach (uint key3 in customZoneRule.Items.Keys)
							{
								key = key3;
								hashSet.Add(key);
							}
							customModeItemCombo.SelectedIDs = hashSet;
							ImGui.SetNextItemWidth(-1f);
							if (uiContext.CustomModeItemCombo.DrawCheckbox())
							{
								_003C_003Ec__DisplayClass113_1 CS_0024_003C_003E8__locals7 = new _003C_003Ec__DisplayClass113_1();
								CS_0024_003C_003E8__locals7.selected = uiContext.CustomModeItemCombo.SelectedIDs;
								foreach (uint item3 in customZoneRule.Items.Keys.Where((uint z) => !CS_0024_003C_003E8__locals7.selected.Contains(z)).ToList())
								{
									customZoneRule.Items.Remove(item3);
								}
								foreach (uint item4 in CS_0024_003C_003E8__locals7.selected.Except(customZoneRule.Items.Keys))
								{
									customZoneRule.Items[item4] = 0u;
								}
								if (uiContext.SelectedCustomZone != 0 && !customPreset.Zones.ContainsKey(uiContext.SelectedCustomZone))
								{
									uiContext.SelectedCustomZone = customPreset.Zones.Keys.FirstOrDefault();
								}
								config.Save(this);
							}
							ImGui.Spacing();
							foreach (KeyValuePair<uint, uint> item5 in customZoneRule.Items)
							{
								item5.Deconstruct(out key, out var value2);
								uint num4 = key;
								uint num5 = value2;
								val11 = new ImU8String(5, 1);
								((ImU8String)(ref val11)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x66B372B6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_762f55d0e7ef4ecd8f3f55c0f8a10fae));
								((ImU8String)(ref val11)).AppendFormatted<uint>(num4);
								IdDisposable val12 = ImRaii.PushId(val11, true);
								try
								{
									ImGui.Image(ImageHelper.GetGameIcon(LuminaWrapper.GetItemIconID(num4)).Handle, new Vector2(ImGui.GetTextLineHeight()));
									ImGui.SameLine();
									ImU8String val13 = new ImU8String(0, 1);
									((ImU8String)(ref val13)).AppendFormatted<string>(LuminaWrapper.GetItemName(num4));
									ImGui.TextUnformatted(val13);
									uint value3 = num5;
									float num6 = 100f * Globals.GlobalUIScale;
									ImGui.SameLine();
									ImGui.SetNextItemWidth(num6);
									ImGuiOm.RightAlignFor(num6);
									ImU8String val14 = ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x109CE318 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_44b9808560124617907f0048883b8948));
									val15 = default(ImU8String);
									ImGui.InputUInt(val14, ref value3, 0u, 0u, val15, (ImGuiInputTextFlags)0);
									if (ImGui.IsItemDeactivatedAfterEdit())
									{
										customZoneRule.Items[num4] = value3;
										config.Save(this);
									}
								}
								finally
								{
									((IDisposable)val12)?.Dispose();
								}
							}
						}
						finally
						{
							((IDisposable)val5)?.Dispose();
						}
						ImGui.TextColored(KnownColor.Chocolate.ToUInt(), ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6C35A08C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88a2dcd1996b4a3fbefaf51a98b59def))));
						val5 = ImRaii.PushIndent(1, true);
						try
						{
							ImGui.SetNextItemWidth(-1f);
							uiContext.CustomModeCompanionCombo.SelectedID = customZoneRule.MinionID;
							if (uiContext.CustomModeCompanionCombo.DrawRadio())
							{
								customZoneRule.MinionID = uiContext.CustomModeCompanionCombo.SelectedID;
								config.Save(this);
							}
						}
						finally
						{
							((IDisposable)val5)?.Dispose();
						}
						ImGui.TextColored(KnownColor.Chocolate.ToUInt(), ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5CE3AA7D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3054c6e6ef664be696bb2ad005864818))));
						val5 = ImRaii.PushIndent(1, true);
						try
						{
							string macro = customZoneRule.Macro;
							ImGui.InputTextMultiline(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7C2FE81A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_12c2d68ba4654622a4959ee73cacc6d7)), ref macro, 4096, new Vector2(-1f, 100f * Globals.GlobalUIScale), (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
							int num7 = macro.Split(new string[3]
							{
								LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x68ACEA18 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4722689f1a4d41dc9dc800bfe1a89c09),
								LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-1003249140 ^ -2102851071 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_bb1eb496bad948e4a2baaa3e1edc7a92),
								LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x76EB1C85 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b988ddaee02846b69dd4e446b5da9de9)
							}, StringSplitOptions.None).Length;
							val15 = new ImU8String(3, 1);
							((ImU8String)(ref val15)).AppendFormatted<int>(num7);
							((ImU8String)(ref val15)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5CE3AAB3 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3054c6e6ef664be696bb2ad005864818));
							ImGui.TextDisabled(val15);
							if (ImGui.IsItemDeactivatedAfterEdit() && num7 <= 15)
							{
								customZoneRule.Macro = macro;
								config.Save(this);
							}
						}
						finally
						{
							((IDisposable)val5)?.Dispose();
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
				((IDisposable)val6)?.Dispose();
			}
		}
		finally
		{
			((TableDisposable)(ref val)).Dispose();
		}
	}

	private bool xpe8DDoah9(CustomPreset preset, int index)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		bool flag = preset is wQgpjiBrB2dwArZO9DQ;
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(1, 2);
		((ImU8String)(ref val)).AppendFormatted<bool>(flag);
		((ImU8String)(ref val)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x109C55C6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_44b9808560124617907f0048883b8948));
		((ImU8String)(ref val)).AppendFormatted<int>(index);
		PopupDisposable val2 = ImRaii.ContextPopupItem(val);
		try
		{
			if (!val2)
			{
				return false;
			}
			if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61637, LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x477E8203 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_ba27244341a5414496a87eae29cf3dec))))
			{
				CustomPreset item = yqE8dHnO9r(preset, preset.Name + LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x18CC32D0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_33babcee9ecc460a9e530039fd18d361) + LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5E3B449A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2d7d8a5df6744c51b35f486f03b53ded)));
				config.CustomPresets.Add(item);
				config.SelectedCustomPresetIndex = BuiltInCustomPresets.Length + config.CustomPresets.Count - 1;
				uiContext.SelectedCustomZone = 0u;
				config.Save(this);
				return true;
			}
			if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)62830, LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x68547A95 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_23522a2a14314da28eb44187a09cd0b5))))
			{
				ModuleBase.ExportToClipboard(yqE8dHnO9r(preset, preset.Name));
			}
			if (!flag && ImGuiOm.ButtonIconWithText((FontAwesomeIcon)62189, LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4B0F8638 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_8e002b0644ef421698ef6f63772929af))))
			{
				config.CustomPresets.RemoveAt(index);
				int num = BuiltInCustomPresets.Length + index;
				if (config.SelectedCustomPresetIndex > num)
				{
					config.SelectedCustomPresetIndex--;
				}
				else if (config.SelectedCustomPresetIndex == num)
				{
					config.SelectedCustomPresetIndex = Math.Min(num, BuiltInCustomPresets.Length + config.CustomPresets.Count - 1);
				}
				uiContext.SelectedCustomZone = 0u;
				config.Save(this);
				return true;
			}
			return false;
		}
		finally
		{
			((PopupDisposable)(ref val2)).Dispose();
		}
	}

	private unsafe DirectPathfindResult H9f8eTbREx(Vector3 targetPos, float radiusSquared = 1f)
	{
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		_003C_003Ec__DisplayClass117_0 CS_0024_003C_003E8__locals12 = new _003C_003Ec__DisplayClass117_0();
		if (!IQGRQ4BGy50rFrjwhAw.oJtB66IZtM(targetPos))
		{
			return DirectPathfindResult.Invalid;
		}
		if (IConditionExtension.get_IsBetweenAreas(DalamudServiceExtension.Instance<ICondition>()) || DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)27])
		{
			return DirectPathfindResult.Invalid;
		}
		if (Control.GetLocalPlayer() == null)
		{
			return DirectPathfindResult.Invalid;
		}
		if (LocalPlayerState.DistanceTo3DSquared(targetPos) <= radiusSquared)
		{
			QO98N7kSGo();
			return DirectPathfindResult.InRange;
		}
		bool flag = DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4];
		bool flag2 = DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)26];
		if (!RdNvO4bUFE(GameState.TerritoryTypeData, targetPos) && !flag)
		{
			if (!flag2)
			{
				HypvCF3peJ();
			}
			return DirectPathfindResult.Invalid;
		}
		CS_0024_003C_003E8__locals12.fly = flag && Control.CanFly;
		CS_0024_003C_003E8__locals12.navTarget = pathfind.af3BkOmId7(targetPos);
		long tickCount = Environment.TickCount64;
		if (pathfind.lKNBaP79ti(CS_0024_003C_003E8__locals12.navTarget, tickCount))
		{
			return DirectPathfindResult.Running;
		}
		if (pathfind.GggBZie02f(CS_0024_003C_003E8__locals12.navTarget) && (vnavmeshIPC.GetIsPathfindRunning() || vnavmeshIPC.GetIsPathfindInProgress()))
		{
			vnavmeshIPC.StopPathfind();
		}
		if (pathfind.fTbB2S9WYW(CS_0024_003C_003E8__locals12.navTarget, tickCount))
		{
			O9u8InXG1u();
			return DirectPathfindResult.Running;
		}
		if (pathfind.IsCompleted)
		{
			List<Vector3> list = pathfind.vydB5jRvBu();
			if (list != null && list.Count > 0 && Vector2.DistanceSquared(list.Last().ToVector2(), CS_0024_003C_003E8__locals12.navTarget.ToVector2()) <= 900f)
			{
				list[0] = LocalPlayerState.Object.Position;
				vnavmeshIPC.SetPathfindTolerance(1f);
				vnavmeshIPC.PathfindWithPath(list, CS_0024_003C_003E8__locals12.fly);
				pathfind.XrwB0kVBQO(CS_0024_003C_003E8__locals12.navTarget, tickCount);
				O9u8InXG1u();
				return DirectPathfindResult.Running;
			}
		}
		DateTime uTCNow = OmenServiceBase<StandardTimeManager>.Instance().UTCNow;
		if (pathfind.IsRunning && (uTCNow - pathfind.StartTime).TotalMilliseconds >= 30000.0)
		{
			pathfind.NHhBRVwW04();
			vnavmeshIPC.StopPathfind();
		}
		if (pathfind.IsRunning && Vector3.DistanceSquared(pathfind.Target, CS_0024_003C_003E8__locals12.navTarget) <= 9f)
		{
			return DirectPathfindResult.Running;
		}
		pathfind.btTBDXf0cZ(CS_0024_003C_003E8__locals12.navTarget, uTCNow, tickCount, (CancellationToken token) => vnavmeshIPC.PathfindCancelable(LocalPlayerState.Object.Position, CS_0024_003C_003E8__locals12.navTarget, CS_0024_003C_003E8__locals12.fly, token));
		return DirectPathfindResult.Running;
	}

	private void QO98N7kSGo()
	{
		pathfind.nfiBe1Rra6();
		if (vnavmeshIPC.GetIsPathfindRunning() || vnavmeshIPC.GetIsPathfindInProgress())
		{
			vnavmeshIPC.StopPathfind();
		}
	}

	private CustomPreset? f9x87jJkaf()
	{
		if (config.SelectedCustomPresetIndex < 0)
		{
			return null;
		}
		if (config.SelectedCustomPresetIndex < BuiltInCustomPresets.Length)
		{
			return BuiltInCustomPresets[config.SelectedCustomPresetIndex];
		}
		int num = config.SelectedCustomPresetIndex - BuiltInCustomPresets.Length;
		if (num >= config.CustomPresets.Count)
		{
			return null;
		}
		return config.CustomPresets[num];
	}

	private static CustomPreset yqE8dHnO9r(CustomPreset source, string name)
	{
		return new CustomPreset
		{
			Name = name,
			Zones = source.Zones.ToDictionary<KeyValuePair<uint, CustomZoneRule>, uint, CustomZoneRule>((KeyValuePair<uint, CustomZoneRule> kvp) => kvp.Key, (KeyValuePair<uint, CustomZoneRule> kvp) => new CustomZoneRule
			{
				Items = new Dictionary<uint, uint>(kvp.Value.Items),
				RequiredFateCount = kvp.Value.RequiredFateCount,
				MinionID = kvp.Value.MinionID,
				Macro = kvp.Value.Macro
			})
		};
	}

	private unsafe static void O9u8InXG1u()
	{
		if (!DalamudServiceExtension.Instance<ICondition>().Any((ConditionFlag[])(object)new ConditionFlag[2]
		{
			(ConditionFlag)4,
			(ConditionFlag)26
		}) && ((ActionManager)ActionManager.Instance()).IsActionOffCooldown((ActionType)1, 3u) && !LocalPlayerState.HasStatus(50u, out var _))
		{
			OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)1, 3u, 3758096384uL, 0u, (UseActionMode)0);
		}
	}

	private void q5q8UWZLbj()
	{
		i2yv995ltv();
		machine.C9qmnWPknt();
		OmenServiceBase<MovementManager>.Instance().AbortTasks();
		QO98N7kSGo();
	}

	private static bool MD08urmiRQ()
	{
		if (ManagerBase<ModuleManager>.Instance().IsModuleEnabled(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x45E4A5F9 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1904f95d2a394c929e1a6d4f72943ad7)) == true)
		{
			return ManagerBase<ModuleManager>.Instance().IsModuleEnabled(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7FD810F3 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1eab987c1db64b928220e27bd03141d3)) == true;
		}
		return false;
	}

	private unsafe static bool UcG8XgdQGn(Vector3 finalPos, Vector3 aetherytePos)
	{
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		BattleChara* localPlayer = Control.GetLocalPlayer();
		if (localPlayer == null)
		{
			return false;
		}
		if (LocalPlayerState.DistanceTo2DSquared(aetherytePos.ToVector2()) <= 3600f)
		{
			return false;
		}
		if (LocalPlayerState.DistanceTo3DSquared(finalPos) <= 10000f)
		{
			return false;
		}
		Vector3? vector = vnavmeshIPC.QueryNearestPointOnMesh(finalPos, 40f, 10000f);
		Vector3? vector2 = vnavmeshIPC.QueryNearestPointOnMesh(aetherytePos, 40f, 10000f);
		if (vector.HasValue)
		{
			Vector3 valueOrDefault = vector.GetValueOrDefault();
			if (IQGRQ4BGy50rFrjwhAw.oJtB66IZtM(valueOrDefault))
			{
				finalPos = valueOrDefault;
			}
		}
		if (vector2.HasValue)
		{
			Vector3 valueOrDefault2 = vector2.GetValueOrDefault();
			if (IQGRQ4BGy50rFrjwhAw.oJtB66IZtM(valueOrDefault2))
			{
				aetherytePos = valueOrDefault2;
			}
		}
		float num = Vector3.Distance(finalPos, Vector3.op_Implicit(((BattleChara)localPlayer).Position)) / 12f;
		float num2 = Vector3.Distance(finalPos, aetherytePos) / 12f + 10f;
		return num > num2;
	}

	private unsafe static bool E6H8YwDrd1()
	{
		if ((((FateManager)FateManager.Instance()).CurrentFate == null || ((FateContext)((FateManager)FateManager.Instance()).CurrentFate).Progress == 100) && !DalamudServiceExtension.Instance<ICondition>().Any((ConditionFlag[])(object)new ConditionFlag[2]
		{
			(ConditionFlag)26,
			(ConditionFlag)27
		}) && ((ActionManager)ActionManager.Instance()).GetActionStatus((ActionType)5, 7u, 3758096384uL, true, true, (uint*)null) == 0 && !(*Addons.EnemyList).IsAddonAndNodesReady() && !((AgentMap)AgentMap.Instance()).IsPlayerMoving && Throttler.Shared.Check(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x39871C63 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_56ae6e3f2cdf4dd8a743f4d493ce49a5)))
		{
			return Throttler.Shared.Check(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-1675397288 ^ -121180716 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f53034f3efc64a309b10c7885bf5d9bd));
		}
		return false;
	}

	private static float fDc8zVrbeL()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		ClassJob classJobData = LocalPlayerState.ClassJobData;
		switch (((ClassJob)(ref classJobData)).Role)
		{
		case 1:
		case 2:
			return 9f;
		case 3:
		case 4:
			return 400f;
		default:
			return 0f;
		}
	}

	private unsafe static bool u9hvnHw4lY([NotNullWhen(true)] out IBattleNPC? obj)
	{
		_003C_003Ec__DisplayClass129_0 CS_0024_003C_003E8__locals3 = new _003C_003Ec__DisplayClass129_0();
		obj = null;
		CS_0024_003C_003E8__locals3.currentFate = ((FateManager)FateManager.Instance()).CurrentFate;
		if (CS_0024_003C_003E8__locals3.currentFate == null)
		{
			return false;
		}
		obj = IOmenDalamudService<IObjectTable>.Instance().FirstOrDefault((IGameObject x) => x is IBattleNPC battleNPC && (int)x.ObjectKind == 2 && battleNPC.NameID == 6737 && x != null && x.IsTargetable && !x.IsDead && ((BattleChara)battleNPC.ToBCStruct()).FateId == ((FateContext)CS_0024_003C_003E8__locals3.currentFate).FateId && ((Enum)battleNPC.BattleNPCKind).HasFlag((Enum)(object)(BattleNpcSubKind)5)) as IBattleNPC;
		return obj != null;
	}

	private unsafe bool AQ1vy4kpWs(uint fateID, Vector3 fateLocation, float fateRadius, byte fateProgress, bool isCollectFate, bool allowOutOfRange, [NotNullWhen(true)] out IGameObject? monster)
	{
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		_003C_003Ec__DisplayClass130_0 CS_0024_003C_003E8__locals11 = new _003C_003Ec__DisplayClass130_0();
		CS_0024_003C_003E8__locals11.allowOutOfRange = allowOutOfRange;
		monster = null;
		CS_0024_003C_003E8__locals11.localPlayer = Control.GetLocalPlayer();
		if (CS_0024_003C_003E8__locals11.localPlayer == null)
		{
			return false;
		}
		int num = Math.Min(((Hater)(&((UIState)UIState.Instance()).Hater)).HaterCount, ((Hater)(&((UIState)UIState.Instance()).Hater)).Haters.Length);
		Span<uint> span = stackalloc uint[num];
		int num2 = 0;
		Span<HaterInfo> span2 = ((Hater)(&((UIState)UIState.Instance()).Hater)).Haters.Slice(0, num);
		for (int i = 0; i < span2.Length; i++)
		{
			HaterInfo val = span2[i];
			uint entityId = val.EntityId;
			if ((entityId != 0 && entityId != 3758096384u) || 1 == 0)
			{
				span[num2++] = val.EntityId;
			}
		}
		span = span.Slice(0, num2);
		IGameObject target = TargetManager.Target;
		uint num3 = target?.EntityID ?? 0;
		bool flag = DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)26];
		float num4 = ((((BattleChara)CS_0024_003C_003E8__locals11.localPlayer).MaxHealth == 0) ? 0f : ((float)((BattleChara)CS_0024_003C_003E8__locals11.localPlayer).Health / (float)((BattleChara)CS_0024_003C_003E8__locals11.localPlayer).MaxHealth * 100f));
		bool flag2 = fateProgress >= 80;
		bool flag3 = !isCollectFate && num4 > 60f && num2 < (flag2 ? 1 : 3);
		int num5 = -1;
		float num6 = float.MaxValue;
		uint num7 = uint.MaxValue;
		float num8 = float.MaxValue;
		float num9 = fateRadius * fateRadius;
		Vector3 value = fateLocation;
		if (flag)
		{
			value = Vector3.op_Implicit(((BattleChara)CS_0024_003C_003E8__locals11.localPlayer).Position);
		}
		if (flag3 && target is IBattleNPC && !target.IsDead && target.FateID == fateID)
		{
			value = target.Position;
		}
		foreach (IGameObject item in IOmenDalamudService<IObjectTable>.Instance().SearchObjects((IGameObject x) => x is IBattleNPC && x.IsTargetable && !x.IsDead && (CS_0024_003C_003E8__locals11.allowOutOfRange || (ActionManager.CanUseActionOnTarget(142u, x.ToStruct()) && ActionManager.GetActionInRangeOrLoS(19700u, (GameObject*)CS_0024_003C_003E8__locals11.localPlayer, x.ToStruct()) != 562)), IObjectTable.CharactersRange))
		{
			bool flag4 = ((ReadOnlySpan<uint>)span).Contains(item.EntityID);
			if ((((GameObject)item.ToStruct()).FateId == fateID || !(!flag4 | CS_0024_003C_003E8__locals11.allowOutOfRange)) && (flag4 || flag3) && ((BattleChara)item.ToBCStruct()).NameId != 6737 && !(Vector2.DistanceSquared(item.Position.ToVector2(), fateLocation.ToVector2()) >= num9) && (!(!flag4 && flag) || !(Vector3.DistanceSquared(Vector3.op_Implicit(((BattleChara)CS_0024_003C_003E8__locals11.localPlayer).Position), item.Position) > 400f)))
			{
				bool flag5 = num3 == item.EntityID;
				bool flag6 = !flag4 && machine.PullTargetID == item.EntityID;
				int num10 = (flag2 ? 1 : ((!flag3) ? ((!flag5) ? 1 : 2) : ((!flag4) ? (flag6 ? 5 : (flag5 ? 4 : 3)) : ((!flag5) ? 1 : 2))));
				IBattleChara battleChara = (IBattleChara)item;
				float num11 = ((battleChara.MaxHp == 0) ? 1f : ((float)battleChara.CurrentHp / (float)battleChara.MaxHp));
				float num12 = Vector3.DistanceSquared(value, item.Position);
				if (monster == null || (num10 >= num5 && (num10 != num5 || !(flag2 ? (battleChara.CurrentHp > num7) : (num11 > num6))) && (num10 != num5 || !(flag2 ? (battleChara.CurrentHp == num7) : (num11 == num6)) || !(num12 >= num8))))
				{
					monster = item;
					num5 = num10;
					num6 = num11;
					num7 = battleChara.CurrentHp;
					num8 = num12;
				}
			}
		}
		machine.PullTargetID = ((monster != null && flag3 && !((ReadOnlySpan<uint>)span).Contains(monster.EntityID)) ? monster.EntityID : 0u);
		return monster != null;
	}

	private unsafe static bool N5QvLWvM7R([NotNullWhen(true)] out IBattleNPC? obj)
	{
		obj = null;
		FateContext* currentFate = ((FateManager)FateManager.Instance()).CurrentFate;
		bool flag = currentFate == null;
		if (!flag)
		{
			uint objectiveNpc = ((FateContext)currentFate).ObjectiveNpc;
			bool flag2 = ((objectiveNpc == 0 || objectiveNpc == 3758096384u) ? true : false);
			flag = flag2;
		}
		if (flag)
		{
			return false;
		}
		obj = IOmenDalamudService<IObjectTable>.Instance().SearchByID(((FateContext)currentFate).ObjectiveNpc, IObjectTable.CharactersRange) as IBattleNPC;
		return obj?.IsTargetable ?? false;
	}

	private unsafe static bool PqkvHQ4sf0([NotNullWhen(true)] out IGameObject? obj)
	{
		_003C_003Ec__DisplayClass132_0 CS_0024_003C_003E8__locals3 = new _003C_003Ec__DisplayClass132_0();
		obj = null;
		CS_0024_003C_003E8__locals3.currentFate = ((FateManager)FateManager.Instance()).CurrentFate;
		if (CS_0024_003C_003E8__locals3.currentFate == null)
		{
			return false;
		}
		float num = float.MaxValue;
		foreach (IGameObject item in IOmenDalamudService<IObjectTable>.Instance().SearchObjects((IGameObject x) => (int)x.ObjectKind == 7 && ((GameObject)x.ToStruct()).FateId == ((FateContext)CS_0024_003C_003E8__locals3.currentFate).FateId && x.IsTargetable && ((GameObject)x.ToStruct()).TargetableStatus.IsSetAll<ObjectTargetableFlags>(new ReadOnlySpan<ObjectTargetableFlags>(Unsafe.AsPointer(ref global::_003CPrivateImplementationDetails_003E._8263A852B3877BAB05AE51AE58179841C08B22A25E0E964F5C157690B61F9B9C), 2)), IObjectTable.EventRange))
		{
			float num2 = LocalPlayerState.DistanceTo3DSquared(item.Position);
			if (!(num2 >= num))
			{
				obj = item;
				num = num2;
			}
		}
		return obj != null;
	}

	private unsafe static bool Vn6vSJIrjq(uint fateID, [NotNullWhen(true)] out IGameObject? obj)
	{
		obj = null;
		FateContext* fateById = ((FateManager)FateManager.Instance()).GetFateById((ushort)fateID);
		bool flag = fateById == null;
		if (!flag)
		{
			uint motivationNpc = ((FateContext)fateById).MotivationNpc;
			bool flag2 = ((motivationNpc == 0 || motivationNpc == 3758096384u) ? true : false);
			flag = flag2;
		}
		if (flag)
		{
			return false;
		}
		obj = IOmenDalamudService<IObjectTable>.Instance().SearchByID(((FateContext)fateById).MotivationNpc, IObjectTable.CharactersRange);
		return obj?.IsTargetable ?? false;
	}

	private unsafe static bool gBMvVKactb()
	{
		BattleChara* localPlayer = Control.GetLocalPlayer();
		if (localPlayer == null)
		{
			return false;
		}
		if (!((StatusManager)(&((BattleChara)localPlayer).StatusManager)).HasStatus(1288u, 3758096384u))
		{
			return ((StatusManager)(&((BattleChara)localPlayer).StatusManager)).HasStatus(1289u, 3758096384u);
		}
		return true;
	}

	private unsafe void i2yv995ltv()
	{
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Invalid comparison between Unknown and I4
		if (machine.AttackTargetID != 0L)
		{
			if (!string.IsNullOrWhiteSpace(config.DeactiveAttackCommand) && config.DeactiveAttackCommand.StartsWith('/'))
			{
				OmenServiceBase<ChatManager>.Instance().SendMessage(config.DeactiveAttackCommand);
			}
			AutoAttackCommand.Disable();
			machine.AttackTargetID = 0uL;
			machine.PullTargetID = 0u;
			if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)27] && (int)((ActionManager)ActionManager.Instance()).CastActionType == 1 && ((ActionManager)ActionManager.Instance()).CastActionId > 8)
			{
				OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.CancelCast);
			}
		}
	}

	private void AYVvfjtdrb(IGameObject obj)
	{
		TargetManager.Target = obj;
		if (machine.AttackTargetID != obj.EntityID)
		{
			if (!string.IsNullOrWhiteSpace(config.ActiveAttackCommand) && config.ActiveAttackCommand.StartsWith('/'))
			{
				OmenServiceBase<ChatManager>.Instance().SendMessage(config.ActiveAttackCommand);
			}
			AutoAttackCommand.Enable(obj.EntityID);
			machine.AttackTargetID = obj.EntityID;
		}
	}

	private static void HypvCF3peJ()
	{
		if (!IConditionExtension.get_IsOnMount(DalamudServiceExtension.Instance<ICondition>()) && Throttler.Shared.Check(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--367535148 ^ 0x7DBC4AEF ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_23522a2a14314da28eb44187a09cd0b5)) && Throttler.Shared.Check(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2DBDB224 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5d8d924f1edf4ec39f023b3c7bb646cc)) && !IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()) && !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)27])
		{
			OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)5, 9u, 3758096384uL, 0u, (UseActionMode)0);
		}
	}

	private unsafe static void B1svjugXkl()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		BattleChara* localPlayer = Control.GetLocalPlayer();
		if (localPlayer != null && DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4])
		{
			if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)77])
			{
				OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommandComplexLocation(ExecuteCommandComplexFlag.Dismount, Vector3.op_Implicit(((BattleChara)localPlayer).Position), RotationHelper.CharaToPacket(((BattleChara)localPlayer).Rotation));
			}
			else
			{
				OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.Dismount);
			}
		}
	}

	public AutoFate()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x64E58E9C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f53034f3efc64a309b10c7885bf5d9bd)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--1976305562 ^ 0xB2B177F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88432e96079448078bdf00e7b62f8ab6)),
			Category = ModuleCategory.Script,
			Author = new string[2]
			{
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x469AB9B7 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_bb1eb496bad948e4a2baaa3e1edc7a92),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6C35A16A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88a2dcd1996b4a3fbefaf51a98b59def)
			},
			ModulesPrerequisite = new string[7]
			{
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-1003249140 ^ -417084301 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a96fab9e97944b39a8afe3fb1109ba7d),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6FF1DDD8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5859f4ea3042459db2c9b610b3099912),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x64E58EC2 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f53034f3efc64a309b10c7885bf5d9bd),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x454E919F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_09da05f368544962acb62b42d9155467),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7EE7F88D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88432e96079448078bdf00e7b62f8ab6),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x45E4AAD5 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1904f95d2a394c929e1a6d4f72943ad7),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2194D3F6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cd1b319f326a44d1aaea645e95d8698b)
			},
			ModulesConflict = new string[2]
			{
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6C35AE8C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88a2dcd1996b4a3fbefaf51a98b59def),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x43D74214 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_974d377e1ed74986b81c34c95c2cc846)
			},
			ModulesPair = new string[1] { LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6FF176C6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5859f4ea3042459db2c9b610b3099912) }
		};
		Permission = new ModulePermission
		{
			NeedAuth = true
		};
		machine = new uGeIvcBNLWfsqCPBrl2();
		pathfind = new IQGRQ4BGy50rFrjwhAw();
		base._002Ector();
	}

	unsafe static AutoFate()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		FateModeLoc = new Dictionary<FateMode, (string, string)>
		{
			[FateMode.SingleZone] = (LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x109CED72 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_44b9808560124617907f0048883b8948)), LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x317C941 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_99910d0c1e594d99b604e0aab1195273))),
			[FateMode.MainSubZone] = (LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x47531DD0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_790eb00575b3425495c924ac13469f4c)), LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6E003CB0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c40146824d9043e0906f4e1889cf39aa))),
			[FateMode.MultiZone] = (LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-1003249140 ^ -2146157524 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8dfe57222f64fa68d9d16c8b6fe9d0b)), LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xE094AA3 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_aba6092d7cea44a49d1667540500e363))),
			[FateMode.Custom] = (LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x477E8EF3 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_ba27244341a5414496a87eae29cf3dec)), LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x45E4A853 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1904f95d2a394c929e1a6d4f72943ad7)))
		}.ToFrozenDictionary();
		FateZones = null;
		RestCities = null;
		ExchangeItems = null;
		ShopNPCDataIDs = null;
		InvalidFateStates = FrozenSet.Create(new ReadOnlySpan<FateState>(Unsafe.AsPointer(ref global::_003CPrivateImplementationDetails_003E._3D6553FB8B273C39BC1468F74E97D1684AC50997367154EDE37497C4D63C712A), 3));
		BuiltInCustomPresets = new wQgpjiBrB2dwArZO9DQ[9]
		{
			new wQgpjiBrB2dwArZO9DQ(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3AC9DB6A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3932528062874ea4bb7e2e1185394e37))
			{
				Data = new Dictionary<uint, Dictionary<uint, uint>>
				{
					[1187u] = new Dictionary<uint, uint> { [47744u] = 3u },
					[1188u] = new Dictionary<uint, uint> { [47745u] = 3u },
					[1189u] = new Dictionary<uint, uint> { [47746u] = 3u },
					[1190u] = new Dictionary<uint, uint> { [47747u] = 3u },
					[1191u] = new Dictionary<uint, uint> { [47748u] = 3u },
					[1192u] = new Dictionary<uint, uint> { [47749u] = 3u }
				}
			},
			new wQgpjiBrB2dwArZO9DQ(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x420C4790 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_84ee025b19bb4145ba9b4b6b7acb91eb))
			{
				Data = new Dictionary<uint, Dictionary<uint, uint>>
				{
					[1187u] = new Dictionary<uint, uint> { [50059u] = 1200u },
					[1188u] = new Dictionary<uint, uint> { [50059u] = 1200u },
					[1189u] = new Dictionary<uint, uint> { [50059u] = 1200u },
					[1190u] = new Dictionary<uint, uint> { [50059u] = 1200u },
					[1191u] = new Dictionary<uint, uint> { [50059u] = 1200u },
					[1192u] = new Dictionary<uint, uint> { [50059u] = 1200u }
				}
			},
			new wQgpjiBrB2dwArZO9DQ(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x64E5828C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f53034f3efc64a309b10c7885bf5d9bd))
			{
				Zones = new Dictionary<uint, CustomZoneRule>
				{
					[1187u] = new CustomZoneRule
					{
						RequiredFateCount = 5u
					},
					[1188u] = new CustomZoneRule
					{
						RequiredFateCount = 5u
					},
					[1189u] = new CustomZoneRule
					{
						RequiredFateCount = 5u
					},
					[1190u] = new CustomZoneRule
					{
						RequiredFateCount = 5u
					},
					[1191u] = new CustomZoneRule
					{
						RequiredFateCount = 5u
					},
					[1192u] = new CustomZoneRule
					{
						RequiredFateCount = 5u
					}
				}
			},
			new wQgpjiBrB2dwArZO9DQ(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x27D121F7 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_39d5d8025f5249e6b489bc374eefbb3c))
			{
				Data = new Dictionary<uint, Dictionary<uint, uint>>
				{
					[134u] = new Dictionary<uint, uint> { [7856u] = 1u },
					[135u] = new Dictionary<uint, uint> { [7859u] = 1u },
					[138u] = new Dictionary<uint, uint> { [7862u] = 1u },
					[139u] = new Dictionary<uint, uint> { [7853u] = 1u },
					[180u] = new Dictionary<uint, uint> { [7858u] = 1u },
					[148u] = new Dictionary<uint, uint> { [7851u] = 1u },
					[152u] = new Dictionary<uint, uint> { [7854u] = 1u },
					[154u] = new Dictionary<uint, uint> { [7860u] = 1u },
					[140u] = new Dictionary<uint, uint> { [7857u] = 1u },
					[141u] = new Dictionary<uint, uint> { [7861u] = 1u },
					[145u] = new Dictionary<uint, uint> { [7855u] = 1u },
					[146u] = new Dictionary<uint, uint> { [7852u] = 1u }
				}
			},
			new wQgpjiBrB2dwArZO9DQ(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x39870327 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_56ae6e3f2cdf4dd8a743f4d493ce49a5))
			{
				Data = new Dictionary<uint, Dictionary<uint, uint>>
				{
					[402u] = new Dictionary<uint, uint> { [13571u] = 1u },
					[401u] = new Dictionary<uint, uint> { [13570u] = 1u },
					[400u] = new Dictionary<uint, uint> { [13573u] = 1u },
					[397u] = new Dictionary<uint, uint> { [13569u] = 1u },
					[398u] = new Dictionary<uint, uint> { [13572u] = 1u },
					[399u] = new Dictionary<uint, uint> { [13574u] = 1u }
				}
			},
			new wQgpjiBrB2dwArZO9DQ(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xB1E213E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_79d931aaf1c142a2a09f53e716260ad1))
			{
				Data = new Dictionary<uint, Dictionary<uint, uint>>
				{
					[397u] = new Dictionary<uint, uint> { [31573u] = 20u },
					[401u] = new Dictionary<uint, uint> { [31573u] = 20u },
					[398u] = new Dictionary<uint, uint> { [31574u] = 20u },
					[400u] = new Dictionary<uint, uint> { [31574u] = 20u },
					[399u] = new Dictionary<uint, uint> { [31575u] = 20u },
					[402u] = new Dictionary<uint, uint> { [31575u] = 20u }
				}
			},
			new wQgpjiBrB2dwArZO9DQ(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x27D126C1 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_39d5d8025f5249e6b489bc374eefbb3c))
			{
				Data = new Dictionary<uint, Dictionary<uint, uint>>
				{
					[612u] = new Dictionary<uint, uint> { [32957u] = 18u },
					[620u] = new Dictionary<uint, uint> { [32957u] = 18u },
					[621u] = new Dictionary<uint, uint> { [32957u] = 18u },
					[613u] = new Dictionary<uint, uint> { [32958u] = 18u },
					[614u] = new Dictionary<uint, uint> { [32958u] = 18u },
					[622u] = new Dictionary<uint, uint> { [32958u] = 18u }
				}
			},
			new wQgpjiBrB2dwArZO9DQ(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x1C17E267 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_accf3844f1c6464481df1849f0aee1bb))
			{
				Data = new Dictionary<uint, Dictionary<uint, uint>>
				{
					[134u] = new Dictionary<uint, uint> { [15167u] = 50u },
					[135u] = new Dictionary<uint, uint> { [15167u] = 50u },
					[138u] = new Dictionary<uint, uint> { [15167u] = 50u },
					[139u] = new Dictionary<uint, uint> { [15167u] = 50u },
					[140u] = new Dictionary<uint, uint> { [15167u] = 50u },
					[141u] = new Dictionary<uint, uint> { [15167u] = 50u },
					[145u] = new Dictionary<uint, uint> { [15167u] = 50u },
					[146u] = new Dictionary<uint, uint> { [15167u] = 50u },
					[148u] = new Dictionary<uint, uint> { [15167u] = 50u },
					[152u] = new Dictionary<uint, uint> { [15167u] = 50u },
					[153u] = new Dictionary<uint, uint> { [15167u] = 50u },
					[154u] = new Dictionary<uint, uint> { [15167u] = 50u },
					[180u] = new Dictionary<uint, uint> { [15167u] = 50u },
					[397u] = new Dictionary<uint, uint> { [15167u] = 50u },
					[398u] = new Dictionary<uint, uint> { [15167u] = 50u },
					[399u] = new Dictionary<uint, uint> { [15167u] = 50u },
					[400u] = new Dictionary<uint, uint> { [15167u] = 50u },
					[401u] = new Dictionary<uint, uint> { [15167u] = 50u },
					[402u] = new Dictionary<uint, uint> { [15167u] = 50u },
					[612u] = new Dictionary<uint, uint> { [15167u] = 50u },
					[613u] = new Dictionary<uint, uint> { [15167u] = 50u },
					[614u] = new Dictionary<uint, uint> { [15167u] = 50u },
					[620u] = new Dictionary<uint, uint> { [15167u] = 50u },
					[621u] = new Dictionary<uint, uint> { [15167u] = 50u },
					[622u] = new Dictionary<uint, uint> { [15167u] = 50u }
				}
			},
			new wQgpjiBrB2dwArZO9DQ(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3AC9DCA4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3932528062874ea4bb7e2e1185394e37))
			{
				Zones = new Dictionary<uint, CustomZoneRule>
				{
					[148u] = new CustomZoneRule
					{
						Items = new Dictionary<uint, uint> { [15168u] = 10u },
						MinionID = 200u
					},
					[152u] = new CustomZoneRule
					{
						Items = new Dictionary<uint, uint> { [15169u] = 10u },
						MinionID = 201u
					},
					[153u] = new CustomZoneRule
					{
						Items = new Dictionary<uint, uint> { [15170u] = 10u },
						MinionID = 202u
					},
					[154u] = new CustomZoneRule
					{
						Items = new Dictionary<uint, uint> { [15171u] = 10u },
						MinionID = 203u
					},
					[140u] = new CustomZoneRule
					{
						Items = new Dictionary<uint, uint> { [15172u] = 10u },
						MinionID = 204u
					},
					[141u] = new CustomZoneRule
					{
						Items = new Dictionary<uint, uint> { [15173u] = 10u },
						MinionID = 205u
					},
					[145u] = new CustomZoneRule
					{
						Items = new Dictionary<uint, uint> { [15174u] = 10u },
						MinionID = 206u
					},
					[146u] = new CustomZoneRule
					{
						Items = new Dictionary<uint, uint> { [15175u] = 10u },
						MinionID = 207u
					},
					[134u] = new CustomZoneRule
					{
						Items = new Dictionary<uint, uint> { [15176u] = 10u },
						MinionID = 208u
					},
					[135u] = new CustomZoneRule
					{
						Items = new Dictionary<uint, uint> { [15177u] = 10u },
						MinionID = 209u
					},
					[138u] = new CustomZoneRule
					{
						Items = new Dictionary<uint, uint> { [15178u] = 10u },
						MinionID = 210u
					},
					[139u] = new CustomZoneRule
					{
						Items = new Dictionary<uint, uint> { [15179u] = 10u },
						MinionID = 211u
					},
					[180u] = new CustomZoneRule
					{
						Items = new Dictionary<uint, uint> { [15180u] = 10u },
						MinionID = 212u
					},
					[612u] = new CustomZoneRule
					{
						Items = new Dictionary<uint, uint> { [30805u] = 10u },
						MinionID = 390u
					},
					[397u] = new CustomZoneRule
					{
						Items = new Dictionary<uint, uint> { [30804u] = 10u },
						MinionID = 391u
					},
					[613u] = new CustomZoneRule
					{
						Items = new Dictionary<uint, uint> { [30806u] = 10u },
						MinionID = 393u
					},
					[398u] = new CustomZoneRule
					{
						Items = new Dictionary<uint, uint> { [30803u] = 10u },
						MinionID = 392u
					}
				}
			}
		};
	}

	[CompilerGenerated]
	internal unsafe static bool RdNvO4bUFE(TerritoryType zone, Vector3 targetPos)
	{
		if (((FateManager)FateManager.Instance()).CurrentFate != null && ((FateContext)((FateManager)FateManager.Instance()).CurrentFate).Progress != 100)
		{
			return true;
		}
		if (!((TerritoryType)(ref zone)).Mount)
		{
			return true;
		}
		return LocalPlayerState.DistanceTo3DSquared(targetPos) <= 1600f;
	}
}
