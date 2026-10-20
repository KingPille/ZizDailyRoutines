using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines;
using DailyRoutines.Common.DataUploader.Abstractions;
using DailyRoutines.Common.DataUploader.Extensions;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Network;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Excel.Sheets;
using Newtonsoft.Json;
using OmenTools;
using OmenTools.Dalamud;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Extensions;
using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;
using itAnDoW4mgYVlrmtMor;

namespace EgsxXqC1AybnKVEtY52;

internal class tParGLCTvio61mFxVph : DataUploaderBase
{
	private unsafe delegate void Pc7OGZtC0J98wHJYirQ(AgentHousingPortal* agent, HousingPortalPacket* packet);

	private delegate void Ip9IITtj7nwLQp6hQZk(nint agent, HousingType housingType, ushort zoneID, ushort wardID, ushort plotID, short apartmentID, nint placardSaleInfo, nint a8);

	private record v9NQXitO7vvNEI69rDi : icl6wWtvfBfinAc5e9g
	{
		[JsonProperty("time")]
		public long Time { get; init; }

		[JsonProperty("server")]
		public int Server { get; init; }

		[JsonProperty("area")]
		public string Area { get; init; }

		[JsonProperty("slot")]
		public int Slot { get; init; }

		[JsonProperty("purchase_main")]
		public int PurchaseMain { get; init; }

		[JsonProperty("purchase_sub")]
		public int PurchaseSub { get; init; }

		[JsonProperty("region_main")]
		public int RegionMain { get; init; }

		[JsonProperty("region_sub")]
		public int RegionSub { get; init; }

		[JsonProperty("houses")]
		public vaRkTmttYUOHWicWRFt[] Houses { get; init; }

		public v9NQXitO7vvNEI69rDi(RiAcnEtsFGG3U69DXHt info)
		{
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
			Time = Framework.GetServerTime();
			Server = info.LandInfo.WorldID;
			HouseArea territoryTypeID = (HouseArea)info.LandInfo.TerritoryTypeID;
			Area = territoryTypeID.ToString();
			Slot = info.LandInfo.WardNumber;
			PurchaseMain = (int)info.PurchaseType0;
			PurchaseSub = (int)info.PurchaseType1;
			RegionMain = (int)info.TenantType0;
			RegionSub = (int)info.TenantType1;
			Houses = new vaRkTmttYUOHWicWRFt[info.HouseInfoEntries.Length];
			HousingLandSet? row = LuminaGetter.GetRow<HousingLandSet>(MOICpxgVVn((uint)info.LandInfo.TerritoryTypeID));
			for (int i = 0; i < info.HouseInfoEntries.Length; i++)
			{
				gybX9Qt1nOIuqMnulS1 gybX9Qt1nOIuqMnulS2 = info.HouseInfoEntries[i];
				vaRkTmttYUOHWicWRFt[] houses = Houses;
				int num = i;
				vaRkTmttYUOHWicWRFt obj = new vaRkTmttYUOHWicWRFt
				{
					ID = i + 1,
					Owner = gybX9Qt1nOIuqMnulS2.EstateOwnerName,
					Price = (int)gybX9Qt1nOIuqMnulS2.HousePrice
				};
				byte? b;
				if (!row.HasValue)
				{
					b = null;
				}
				else
				{
					HousingLandSet valueOrDefault = row.GetValueOrDefault();
					LandSetStruct val = ((HousingLandSet)(ref valueOrDefault)).LandSet[i];
					b = ((LandSetStruct)(ref val)).PlotSize;
				}
				byte? b2 = b;
				obj.Size = ((HouseSize)b2.Value/*cast due to constrained. prefix*/).ToString();
				obj.Tags = ((IEnumerable<sbyte>)gybX9Qt1nOIuqMnulS2.HouseAppeals).Select((Func<sbyte, int>)((sbyte x) => x)).Take(3).ToArray();
				obj.IsPersonal = gybX9Qt1nOIuqMnulS2.InfoFlags.HasFlag(HousingFlags.PlotOwned) && !gybX9Qt1nOIuqMnulS2.InfoFlags.HasFlag(HousingFlags.OwnedByFC);
				obj.IsEmpty = !gybX9Qt1nOIuqMnulS2.InfoFlags.HasFlag(HousingFlags.PlotOwned);
				obj.IsPublic = gybX9Qt1nOIuqMnulS2.InfoFlags.HasFlag(HousingFlags.VisitorsAllowed);
				obj.HasGreeting = gybX9Qt1nOIuqMnulS2.InfoFlags.HasFlag(HousingFlags.HasSearchComment);
				houses[num] = obj;
			}
		}

		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x473333DF ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5f88650ee89f435aad988a8583bda5b5));
			stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3B5B9AB8 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1e38c41bbda54d9abc5b426283576a9a));
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
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x9DCFCCC ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1cc312ba3ec946acba954139762d703f));
			builder.Append(Time.ToString());
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x7C6CD032 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d2f07ce64d464f30b1d7397ae7500cac));
			builder.Append(Server.ToString());
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x719D40BB ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5437ef93780b442a895af045b06fabbc));
			builder.Append((object?)Area);
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3F35FDCE ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_128c630676db4a8ab2ed83db4c2b1862));
			builder.Append(Slot.ToString());
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1FFBC295 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_a46716ea783442acb36e95ced1c935fc));
			builder.Append(PurchaseMain.ToString());
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(--664844326 ^ 0x143C7C90 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c48859e0be41429f8474d6e2fecfa0de));
			builder.Append(PurchaseSub.ToString());
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(-224923271 ^ -806722899 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_cda76bbf50614e97b4ef38939106c571));
			builder.Append(RegionMain.ToString());
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x27BADF2F ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7f9a8e3b1ea249bebe4aa86658ee9503));
			builder.Append(RegionSub.ToString());
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5379CE09 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_019a6a2bbee24ddb9dd8d9a1d1e1ca09));
			builder.Append(Houses);
			return true;
		}

		[CompilerGenerated]
		protected v9NQXitO7vvNEI69rDi(v9NQXitO7vvNEI69rDi original)
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
			Time = original.Time;
			Server = original.Server;
			Area = original.Area;
			Slot = original.Slot;
			PurchaseMain = original.PurchaseMain;
			PurchaseSub = original.PurchaseSub;
			RegionMain = original.RegionMain;
			RegionSub = original.RegionSub;
			Houses = original.Houses;
		}
	}

	private record wCmFiutKIsTsDOaaMUr : icl6wWtvfBfinAc5e9g
	{
		[JsonProperty("ServerId")]
		public int ServerID { get; init; }

		[JsonProperty("Area")]
		public int Area { get; init; }

		[JsonProperty("Slot")]
		public int Slot { get; init; }

		[JsonProperty("LandID")]
		public int LandID { get; init; }

		[JsonProperty("Time")]
		public long Time { get; init; }

		[JsonProperty("EndTime")]
		public uint EndTime { get; init; }

		[JsonProperty("State")]
		public int State { get; init; }

		[JsonProperty("Participate")]
		public uint Participate { get; init; }

		[JsonProperty("Winner")]
		public uint Winner { get; init; }

		public wCmFiutKIsTsDOaaMUr(cJd9KXtMoBK3CCMTaRy info, uint territoryID, int wardID, int plotID)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
			ServerID = (int)IOmenDalamudService<IObjectTable>.Instance().LocalPlayer.CurrentWorld.RowId;
			Area = qlgC3aifUh((HouseArea)territoryID);
			Slot = wardID;
			LandID = plotID + 1;
			Time = Framework.GetServerTime();
			EndTime = info.PhaseEndsAt;
			State = (int)info.LotteryState;
			Participate = info.EntryCount;
			Winner = info.Winner;
		}

		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4CDA408F ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_2765950fccc24d96a1162dd6779cbee1));
			stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5B20855E ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_435f54c92c5b40a9ad2757dd36deed0a));
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
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x282809D9 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_95ac0b4adf764381bcdc75a9d241d184));
			builder.Append(ServerID.ToString());
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x768F9F91 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_417cae18312d413c80505172551e7b46));
			builder.Append(Area.ToString());
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x32A2BC5 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_14783916c53948389577bd0e927537b1));
			builder.Append(Slot.ToString());
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6B300D82 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ac6250342b784829976646d28955b117));
			builder.Append(LandID.ToString());
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x7D150477 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1dfb87339e314237a8eb0a6453e2a275));
			builder.Append(Time.ToString());
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x339D3C3C ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c48859e0be41429f8474d6e2fecfa0de));
			builder.Append(EndTime.ToString());
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x768F9E6F ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_417cae18312d413c80505172551e7b46));
			builder.Append(State.ToString());
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x75EEA5A3 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_11422b9d07f9498fb995b7dd8e417b5b));
			builder.Append(Participate.ToString());
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x46290885 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_93e1cfc241b644019a4819c49327d6a7));
			builder.Append(Winner.ToString());
			return true;
		}

		[CompilerGenerated]
		protected wCmFiutKIsTsDOaaMUr(wCmFiutKIsTsDOaaMUr original)
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
			ServerID = original.ServerID;
			Area = original.Area;
			Slot = original.Slot;
			LandID = original.LandID;
			Time = original.Time;
			EndTime = original.EndTime;
			State = original.State;
			Participate = original.Participate;
			Winner = original.Winner;
		}
	}

	private record vaRkTmttYUOHWicWRFt
	{
		[JsonProperty("id")]
		public int ID { get; init; }

		[JsonProperty("owner")]
		public string Owner { get; init; }

		[JsonProperty("price")]
		public int Price { get; init; }

		[JsonProperty("size")]
		public string Size { get; init; }

		[JsonProperty("tags")]
		public int[] Tags { get; init; }

		[JsonProperty("isPersonal")]
		public bool IsPersonal { get; init; }

		[JsonProperty("isEmpty")]
		public bool IsEmpty { get; init; }

		[JsonProperty("isPublic")]
		public bool IsPublic { get; init; }

		[JsonProperty("hasGreeting")]
		public bool HasGreeting { get; init; }

		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x35A9B8C9 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_95b251cdaa8148b7af54d7956ad9ed2a));
			stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5E39F8B3 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f9eb04af496747c5b32cddd51d0b3997));
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
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4CDA43B3 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_2765950fccc24d96a1162dd6779cbee1));
			builder.Append(ID.ToString());
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x9DCFEE4 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1cc312ba3ec946acba954139762d703f));
			builder.Append((object?)Owner);
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4CDA4389 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_2765950fccc24d96a1162dd6779cbee1));
			builder.Append(Price.ToString());
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x15FFD91A ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_e279476822c444f9955c257cab427fb2));
			builder.Append((object?)Size);
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x70C2C53B ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c041f06f3e304b758cab1109bed434d0));
			builder.Append(Tags);
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6B300E94 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ac6250342b784829976646d28955b117));
			builder.Append(IsPersonal.ToString());
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x53682572 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d4ce857df7b047629e7091bbc815dd94));
			builder.Append(IsEmpty.ToString());
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(-1675397288 ^ -259736135 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_86e8e55020514087b1fe097f879d6dff));
			builder.Append(IsPublic.ToString());
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x37CD0E11 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_90dae8e50e3a4314867cef21f0bb1bb0));
			builder.Append(HasGreeting.ToString());
			return true;
		}

		[CompilerGenerated]
		protected vaRkTmttYUOHWicWRFt(vaRkTmttYUOHWicWRFt original)
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
			ID = original.ID;
			Owner = original.Owner;
			Price = original.Price;
			Size = original.Size;
			Tags = original.Tags;
			IsPersonal = original.IsPersonal;
			IsEmpty = original.IsEmpty;
			IsPublic = original.IsPublic;
			HasGreeting = original.HasGreeting;
		}

		public vaRkTmttYUOHWicWRFt()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			Owner = string.Empty;
			Size = string.Empty;
			Tags = Array.Empty<int>();
			base._002Ector();
		}
	}

	public class OP6xUItWdQUENQ6jOVR
	{
		public short LandID;

		public short TerritoryTypeID;

		public short WardNumber;

		public short WorldID;

		public static OP6xUItWdQUENQ6jOVR buUtTZRXxj(BinaryReader binaryReader)
		{
			return new OP6xUItWdQUENQ6jOVR
			{
				LandID = binaryReader.ReadInt16(),
				WardNumber = binaryReader.ReadInt16(),
				TerritoryTypeID = binaryReader.ReadInt16(),
				WorldID = binaryReader.ReadInt16()
			};
		}

		public OP6xUItWdQUENQ6jOVR()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
		}
	}

	private class gybX9Qt1nOIuqMnulS1
	{
		public string EstateOwnerName;

		public sbyte[] HouseAppeals;

		public uint HousePrice;

		public HousingFlags InfoFlags;

		public gybX9Qt1nOIuqMnulS1()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			EstateOwnerName = string.Empty;
			base._002Ector();
		}
	}

	private class cJd9KXtMoBK3CCMTaRy
	{
		public uint EntryCount;

		public LotteryState LotteryState;

		public uint PhaseEndsAt;

		public PurchaseType PurchaseType;

		public TenantType TenantType;

		public byte Unknown1;

		public uint Unknown2;

		public uint Unknown3;

		public uint Winner;

		public unsafe static cJd9KXtMoBK3CCMTaRy CfTt4853Cd(nint dataPtr)
		{
			using UnmanagedMemoryStream input = new UnmanagedMemoryStream((byte*)((IntPtr)dataPtr).ToPointer(), 32L);
			using BinaryReader binaryReader = new BinaryReader(input);
			return new cJd9KXtMoBK3CCMTaRy
			{
				PurchaseType = (PurchaseType)binaryReader.ReadByte(),
				TenantType = (TenantType)binaryReader.ReadByte(),
				LotteryState = (LotteryState)binaryReader.ReadByte(),
				Unknown1 = binaryReader.ReadByte(),
				Unknown2 = binaryReader.ReadUInt32(),
				PhaseEndsAt = binaryReader.ReadUInt32(),
				Unknown3 = binaryReader.ReadUInt32(),
				EntryCount = binaryReader.ReadUInt32(),
				Winner = binaryReader.ReadUInt32()
			};
		}

		public cJd9KXtMoBK3CCMTaRy()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
		}
	}

	private class RiAcnEtsFGG3U69DXHt
	{
		public gybX9Qt1nOIuqMnulS1[] HouseInfoEntries;

		public OP6xUItWdQUENQ6jOVR LandInfo;

		public PurchaseType PurchaseType0;

		public PurchaseType PurchaseType1;

		public TenantType TenantType0;

		public TenantType TenantType1;

		public unsafe static RiAcnEtsFGG3U69DXHt Bhqt85kKkD(nint dataPtr)
		{
			RiAcnEtsFGG3U69DXHt riAcnEtsFGG3U69DXHt = new RiAcnEtsFGG3U69DXHt();
			using UnmanagedMemoryStream input = new UnmanagedMemoryStream((byte*)((IntPtr)dataPtr).ToPointer(), 2664L);
			using BinaryReader binaryReader = new BinaryReader(input);
			riAcnEtsFGG3U69DXHt.LandInfo = OP6xUItWdQUENQ6jOVR.buUtTZRXxj(binaryReader);
			riAcnEtsFGG3U69DXHt.HouseInfoEntries = new gybX9Qt1nOIuqMnulS1[60];
			for (int i = 0; i < 60; i++)
			{
				gybX9Qt1nOIuqMnulS1 gybX9Qt1nOIuqMnulS2 = new gybX9Qt1nOIuqMnulS1
				{
					HousePrice = binaryReader.ReadUInt32(),
					InfoFlags = (HousingFlags)binaryReader.ReadByte(),
					HouseAppeals = new sbyte[3]
				};
				for (int j = 0; j < 3; j++)
				{
					gybX9Qt1nOIuqMnulS2.HouseAppeals[j] = binaryReader.ReadSByte();
				}
				gybX9Qt1nOIuqMnulS2.EstateOwnerName = Encoding.UTF8.GetString(binaryReader.ReadBytes(32)).TrimEnd('\0');
				riAcnEtsFGG3U69DXHt.HouseInfoEntries[i] = gybX9Qt1nOIuqMnulS2;
				if (!gybX9Qt1nOIuqMnulS2.InfoFlags.HasFlag(HousingFlags.PlotOwned))
				{
					gybX9Qt1nOIuqMnulS2.EstateOwnerName = string.Empty;
				}
			}
			riAcnEtsFGG3U69DXHt.PurchaseType0 = (PurchaseType)binaryReader.ReadByte();
			riAcnEtsFGG3U69DXHt.PurchaseType1 = (PurchaseType)binaryReader.ReadByte();
			riAcnEtsFGG3U69DXHt.TenantType0 = (TenantType)binaryReader.ReadByte();
			riAcnEtsFGG3U69DXHt.TenantType1 = (TenantType)binaryReader.ReadByte();
			return riAcnEtsFGG3U69DXHt;
		}

		public RiAcnEtsFGG3U69DXHt()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
		}
	}

	private interface icl6wWtvfBfinAc5e9g
	{
	}

	private enum HousingType : byte
	{
		OwnedHouse,
		UnownedHouse,
		FCChamber,
		Apartment
	}

	private enum LotteryState
	{
		Open = 1,
		Results,
		Closed
	}

	private enum PurchaseType
	{
		NotPurchasable,
		FirstComeFirstServe,
		Lottery
	}

	private enum HouseSize
	{
		S,
		M,
		L
	}

	private enum HouseArea
	{
		未知 = 0,
		海雾村 = 339,
		薰衣草苗圃 = 340,
		高脚孤丘 = 341,
		白银乡 = 641,
		穹顶皓天 = 979
	}

	[Flags]
	private enum HousingFlags : byte
	{
		PlotOwned = 1,
		VisitorsAllowed = 2,
		HasSearchComment = 4,
		HouseBuilt = 8,
		OwnedByFC = 0x10
	}

	private enum TenantType : byte
	{
		FreeCompany = 1,
		Personal
	}

	private class UhV0O7tlhrmvZqaajHr : DataUploaderConfig
	{
		public uint LastRequestDataCenter;

		public DateTime LastRequestTime;

		public UhV0O7tlhrmvZqaajHr()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			LastRequestTime = DateTime.MinValue;
			base._002Ector();
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass20_0
	{
		public HouseArea area;

		public _003C_003Ec__DisplayClass20_0()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass20_1
	{
		public int index;

		public _003C_003Ec__DisplayClass20_0 CS_0024_003C_003E8__locals1;

		public _003C_003Ec__DisplayClass20_1()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
		}

		internal void GyLtxKkj0t()
		{
			OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestHousingWard, (uint)CS_0024_003C_003E8__locals1.area, (uint)index);
		}
	}

	private static readonly CompSig AgentHousingPortalReadPacketSig;

	private Hook<Pc7OGZtC0J98wHJYirQ>? AgentHousingPortalReadPacketHook;

	private static readonly CompSig PlacardSaleInfoSig;

	private Hook<Ip9IITtj7nwLQp6hQZk>? PlacardSaleInfoHook;

	private Channel<v9NQXitO7vvNEI69rDi> wardEntryChannel;

	private Channel<wCmFiutKIsTsDOaaMUr> lotteryEntryChannel;

	private readonly TaskHelper taskHelper;

	private readonly Dictionary<(uint WorldID, uint AreaID, uint WardID), HashSet<uint>> ownerlessInfo;

	private UhV0O7tlhrmvZqaajHr config;

	private static HttpClient UploadClient => OmenServiceBase<HTTPClientHelper>.Instance().Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x200FADF7 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_062a9cfe150f4dc6a9fdd7b6d00434af));

	protected unsafe override Task Init()
	{
		if (!GameState.IsCN)
		{
			return Task.CompletedTask;
		}
		config = DataUploaderConfigExtension.Load<UhV0O7tlhrmvZqaajHr>(this) ?? new UhV0O7tlhrmvZqaajHr();
		wardEntryChannel = Channel.CreateUnbounded<v9NQXitO7vvNEI69rDi>();
		lotteryEntryChannel = Channel.CreateUnbounded<wCmFiutKIsTsDOaaMUr>();
		RunBackground((CancellationToken cancellationToken) => hZOCMwAs1c(wardEntryChannel.Reader, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(--367535148 ^ 0x5A996833 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_3782f2a028bd4046aa095423cd2526e0), cancellationToken));
		RunBackground((CancellationToken cancellationToken) => hZOCMwAs1c(lotteryEntryChannel.Reader, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x35B4AA78 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_042249f573d4431982cd93672ca35bd3), cancellationToken));
		HttpHeaderValueCollection<ProductInfoHeaderValue> userAgent = UploadClient.DefaultRequestHeaders.UserAgent;
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(33, 1);
		defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3B5A30FC ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1e38c41bbda54d9abc5b426283576a9a));
		defaultInterpolatedStringHandler.AppendFormatted(Plugin.Version);
		defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x27BA995B ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7f9a8e3b1ea249bebe4aa86658ee9503));
		userAgent.ParseAdd(defaultInterpolatedStringHandler.ToStringAndClear());
		UploadClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x38D20ED8 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_59cab3a5f3464d2f8ff563ddb3d8bad1), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x75EEE2A1 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_11422b9d07f9498fb995b7dd8e417b5b));
		AgentHousingPortalReadPacketHook = AgentHousingPortalReadPacketSig.GetHook<Pc7OGZtC0J98wHJYirQ>(wLSCsjkQcu);
		AgentHousingPortalReadPacketHook.Enable();
		PlacardSaleInfoHook = PlacardSaleInfoSig.GetHook<Ip9IITtj7nwLQp6hQZk>(v03C80lSjt);
		PlacardSaleInfoHook.Enable();
		OmenServiceBase<GameState>.Instance().Login += ysiC4npyES;
		if (GameState.IsLoggedIn)
		{
			ysiC4npyES();
		}
		return Task.CompletedTask;
	}

	protected override Task Stop()
	{
		if (!GameState.IsCN)
		{
			return Task.CompletedTask;
		}
		OmenServiceBase<GameState>.Instance().Login -= ysiC4npyES;
		wardEntryChannel.Writer.TryComplete();
		lotteryEntryChannel.Writer.TryComplete();
		taskHelper.Abort();
		AgentHousingPortalReadPacketHook?.Dispose();
		AgentHousingPortalReadPacketHook = null;
		PlacardSaleInfoHook?.Dispose();
		PlacardSaleInfoHook = null;
		return Task.CompletedTask;
	}

	protected override Task Uninit()
	{
		if (!GameState.IsCN)
		{
			return Task.CompletedTask;
		}
		taskHelper.Dispose();
		return Task.CompletedTask;
	}

	private static async Task hZOCMwAs1c<T>(ChannelReader<T> reader, string endpoint, CancellationToken ct) where T : icl6wWtvfBfinAc5e9g
	{
		_ = 1;
		try
		{
			while (await reader.WaitToReadAsync(ct))
			{
				List<T> list = new List<T>();
				T item;
				while (list.Count < 50 && reader.TryRead(out item))
				{
					list.Add(item);
				}
				if (list.Count > 0)
				{
					await QdQClGnhww(list, endpoint, ct);
				}
			}
		}
		catch
		{
		}
	}

	private void ysiC4npyES()
	{
		if (!KvtCxmiqi3() || (config.LastRequestDataCenter == GameState.CurrentDataCenter && OmenServiceBase<StandardTimeManager>.Instance().UTCNow - config.LastRequestTime <= TimeSpan.FromHours(3)))
		{
			return;
		}
		config.LastRequestDataCenter = GameState.CurrentDataCenter;
		config.LastRequestTime = OmenServiceBase<StandardTimeManager>.Instance().UTCNow;
		config.Save(this);
		HouseArea[] values = Enum.GetValues<HouseArea>();
		foreach (HouseArea houseArea in values)
		{
			if (houseArea != HouseArea.未知)
			{
				MktCvKZXn8(houseArea);
			}
		}
	}

	private unsafe void wLSCsjkQcu(AgentHousingPortal* agent, HousingPortalPacket* packet)
	{
		AgentHousingPortalReadPacketHook.Original(agent, packet);
		if (!KvtCxmiqi3())
		{
			taskHelper.Abort();
			return;
		}
		RiAcnEtsFGG3U69DXHt riAcnEtsFGG3U69DXHt = RiAcnEtsFGG3U69DXHt.Bhqt85kKkD((nint)packet);
		wardEntryChannel.Writer.TryWrite(new v9NQXitO7vvNEI69rDi(riAcnEtsFGG3U69DXHt));
		(uint, uint, uint) key = ((uint)riAcnEtsFGG3U69DXHt.LandInfo.WorldID, (uint)riAcnEtsFGG3U69DXHt.LandInfo.TerritoryTypeID, (uint)riAcnEtsFGG3U69DXHt.LandInfo.WardNumber);
		ownerlessInfo.TryAdd(key, new HashSet<uint>());
		for (uint num = 0u; num < riAcnEtsFGG3U69DXHt.HouseInfoEntries.Length; num++)
		{
			if (!riAcnEtsFGG3U69DXHt.HouseInfoEntries[num].InfoFlags.HasFlag(HousingFlags.PlotOwned))
			{
				ownerlessInfo[key].Add(num + 1);
			}
		}
	}

	private void v03C80lSjt(nint agent, HousingType housingType, ushort zoneID, ushort wardID, ushort plotID, short apartmentID, nint placardSaleInfo, nint a8)
	{
		PlacardSaleInfoHook.Original(agent, housingType, zoneID, wardID, plotID, apartmentID, placardSaleInfo, a8);
		if (!KvtCxmiqi3())
		{
			taskHelper.Abort();
		}
		else if (housingType == HousingType.UnownedHouse && placardSaleInfo != IntPtr.Zero)
		{
			cJd9KXtMoBK3CCMTaRy cJd9KXtMoBK3CCMTaRy2 = cJd9KXtMoBK3CCMTaRy.CfTt4853Cd(placardSaleInfo);
			if (cJd9KXtMoBK3CCMTaRy2 != null)
			{
				lotteryEntryChannel.Writer.TryWrite(new wCmFiutKIsTsDOaaMUr(cJd9KXtMoBK3CCMTaRy2, zoneID, wardID, plotID));
			}
		}
	}

	private void MktCvKZXn8(HouseArea area)
	{
		_003C_003Ec__DisplayClass20_0 _003C_003Ec__DisplayClass20_2 = new _003C_003Ec__DisplayClass20_0();
		_003C_003Ec__DisplayClass20_2.area = area;
		if (!Enum.IsDefined(_003C_003Ec__DisplayClass20_2.area))
		{
			return;
		}
		for (int i = 0; i < 30; i++)
		{
			_003C_003Ec__DisplayClass20_1 CS_0024_003C_003E8__locals6 = new _003C_003Ec__DisplayClass20_1();
			CS_0024_003C_003E8__locals6.CS_0024_003C_003E8__locals1 = _003C_003Ec__DisplayClass20_2;
			CS_0024_003C_003E8__locals6.index = i;
			TaskHelper obj = taskHelper;
			Action task = delegate
			{
				OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestHousingWard, (uint)CS_0024_003C_003E8__locals6.CS_0024_003C_003E8__locals1.area, (uint)CS_0024_003C_003E8__locals6.index);
			};
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(8, 2);
			defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5E385253 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f9eb04af496747c5b32cddd51d0b3997));
			defaultInterpolatedStringHandler.AppendFormatted(CS_0024_003C_003E8__locals6.CS_0024_003C_003E8__locals1.area);
			defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x76DB9935 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f8c073ebdce5414baf147b1be2d2c2c2));
			defaultInterpolatedStringHandler.AppendFormatted(i);
			obj.Enqueue(task, defaultInterpolatedStringHandler.ToStringAndClear());
			TaskHelper obj2 = taskHelper;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(7, 2);
			defaultInterpolatedStringHandler2.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x70C28049 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c041f06f3e304b758cab1109bed434d0));
			defaultInterpolatedStringHandler2.AppendFormatted(CS_0024_003C_003E8__locals6.CS_0024_003C_003E8__locals1.area);
			defaultInterpolatedStringHandler2.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x392219D0 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f6778e21204a4d43ae5bbed939acf8a2));
			defaultInterpolatedStringHandler2.AppendFormatted(i);
			obj2.DelayNext(100, defaultInterpolatedStringHandler2.ToStringAndClear());
		}
	}

	private static async Task QdQClGnhww<T>(IEnumerable<T> data, string endpoint, CancellationToken cancellationToken) where T : icl6wWtvfBfinAc5e9g
	{
		string[] array = new string[2]
		{
			GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x702B5D8D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ed1539c28915419fb405793dc0185c30),
			GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(--1976305562 ^ 0x4650C0A8 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c48859e0be41429f8474d6e2fecfa0de)
		};
		string[] array2 = array;
		foreach (string text in array2)
		{
			try
			{
				string text2 = JsonConvert.SerializeObject((object)data);
				DLog.Verbose(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x39225D5A ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f6778e21204a4d43ae5bbed939acf8a2) + text2);
				using StringContent content = new StringContent(JsonConvert.SerializeObject((object)data), Encoding.UTF8, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5379A1E1 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_019a6a2bbee24ddb9dd8d9a1d1e1ca09));
				_ = UploadClient;
				_ = text + endpoint;
				_ = content;
				_ = cancellationToken;
				throw null;
			}
			catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
			{
				break;
			}
			catch (Exception)
			{
			}
		}
	}

	private static bool KvtCxmiqi3()
	{
		if (DataUploaderBase.IsPlayerReady())
		{
			return !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)69];
		}
		return false;
	}

	private static uint MOICpxgVVn(uint zoneID)
	{
		return zoneID switch
		{
			641u => 3u, 
			979u => 4u, 
			_ => zoneID - 339, 
		};
	}

	private static int qlgC3aifUh(HouseArea area)
	{
		return area switch
		{
			HouseArea.未知 => -1, 
			HouseArea.海雾村 => 0, 
			HouseArea.薰衣草苗圃 => 1, 
			HouseArea.高脚孤丘 => 2, 
			HouseArea.白银乡 => 3, 
			HouseArea.穹顶皓天 => 4, 
			_ => -1, 
		};
	}

	public tParGLCTvio61mFxVph()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		taskHelper = new TaskHelper
		{
			TimeoutMS = 5000
		};
		ownerlessInfo = new Dictionary<(uint, uint, uint), HashSet<uint>>();
		base._002Ector();
	}

	static tParGLCTvio61mFxVph()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		AgentHousingPortalReadPacketSig = new CompSig(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2E943F25 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_6103a9bab63e42479916a134b07f1c75));
		PlacardSaleInfoSig = new CompSig(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3705122 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_a86a125fa3c94b2ca72995b652fdcc2e));
	}

	[CompilerGenerated]
	private Task JfVCBfV0hL(CancellationToken cancellationToken)
	{
		return hZOCMwAs1c(wardEntryChannel.Reader, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(--367535148 ^ 0x5A996833 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_3782f2a028bd4046aa095423cd2526e0), cancellationToken);
	}

	[CompilerGenerated]
	private Task GNCCmcXjeg(CancellationToken cancellationToken)
	{
		return hZOCMwAs1c(lotteryEntryChannel.Reader, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x35B4AA78 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_042249f573d4431982cd93672ca35bd3), cancellationToken);
	}
}
