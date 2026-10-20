using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.Common.DataUploader.Abstractions;
using Dalamud.Game.Gui.PartyFinder.Types;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Excel.Sheets;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using itAnDoW4mgYVlrmtMor;

namespace soRfOUCgAQHRYCPWEqB;

internal class tso1jlCoGyhNFmSXU03 : DataUploaderBase
{
	[Serializable]
	[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
	internal class UploadableListing
	{
		public ulong ID { get; }

		public uint ContentIDLower { get; }

		public byte[] Name { get; }

		public byte[] Description { get; }

		public ushort CreatedWorld { get; }

		public ushort HomeWorld { get; }

		public ushort CurrentWorld { get; }

		public DutyCategory Category { get; }

		public ushort Duty { get; }

		public DutyType DutyType { get; }

		public bool BeginnersWelcome { get; }

		public ushort SecondsRemaining { get; }

		public ushort MinItemLevel { get; }

		public byte NumParties { get; }

		public byte SlotsAvailable { get; }

		public int LastServerRestart { get; }

		public ObjectiveFlags Objective { get; }

		public ConditionFlags Conditions { get; }

		public DutyFinderSettingsFlags DutyFinderSettings { get; }

		public LootRuleFlags LootRules { get; }

		public SearchAreaFlags SearchArea { get; }

		public List<UploadableSlot> Slots { get; }

		public List<byte> JobsPresent { get; }

		internal UploadableListing(IPartyFinderListing listing)
		{
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
			ID = listing.Id;
			ContentIDLower = (uint)listing.ContentId;
			Name = listing.Name.Encode();
			Description = listing.Description.Encode();
			World value = listing.World.Value;
			CreatedWorld = (ushort)((World)(ref value)).RowId;
			value = listing.HomeWorld.Value;
			HomeWorld = (ushort)((World)(ref value)).RowId;
			value = listing.CurrentWorld.Value;
			CurrentWorld = (ushort)((World)(ref value)).RowId;
			Category = listing.Category;
			Duty = listing.RawDuty;
			DutyType = listing.DutyType;
			BeginnersWelcome = listing.BeginnersWelcome;
			SecondsRemaining = listing.SecondsRemaining;
			MinItemLevel = listing.MinimumItemLevel;
			NumParties = listing.Parties;
			SlotsAvailable = listing.SlotsAvailable;
			LastServerRestart = listing.LastPatchHotfixTimestamp;
			Objective = listing.Objective;
			Conditions = listing.Conditions;
			DutyFinderSettings = listing.DutyFinderSettings;
			LootRules = listing.LootRules;
			SearchArea = listing.SearchArea;
			Slots = listing.Slots.Select((PartyFinderSlot slot) => new UploadableSlot(slot)).ToList();
			JobsPresent = listing.RawJobsPresent.ToList();
		}
	}

	[Serializable]
	[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
	internal class UploadableSlot
	{
		public uint Accepting { get; }

		internal UploadableSlot(PartyFinderSlot slot)
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
			Accepting = slot.Accepting.Aggregate(0u, (uint agg, JobFlags flag) => agg | (uint)flag);
		}
	}

	private readonly System.Timers.Timer uploadTimer;

	private readonly ConcurrentDictionary<int, List<IPartyFinderListing>> data;

	private DateTime lastDataArrivalTime;

	private static HttpClient UploadClient => OmenServiceBase<HTTPClientHelper>.Instance().Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6E47D405 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ad43e19aac164f32ab06e23cbc10bc4e));

	protected override Task Init()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Expected O, but got Unknown
		uploadTimer.Elapsed += eMZCEdv3E2;
		uploadTimer.AutoReset = true;
		uploadTimer.Start();
		DalamudServiceExtension.Instance<IPartyFinderGui>().ReceiveListing += new PartyFinderListingEventDelegate(xw2CwhwF7Q);
		return Task.CompletedTask;
	}

	protected override Task Stop()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IPartyFinderGui>().ReceiveListing -= new PartyFinderListingEventDelegate(xw2CwhwF7Q);
		uploadTimer.Elapsed -= eMZCEdv3E2;
		uploadTimer.Stop();
		return Task.CompletedTask;
	}

	protected override Task Uninit()
	{
		uploadTimer.Dispose();
		data.Clear();
		return Task.CompletedTask;
	}

	private void xw2CwhwF7Q(IPartyFinderListing listing, IPartyFinderListingEventArgs args)
	{
		data.GetOrAdd(args.BatchNumber, (int _) => new List<IPartyFinderListing>()).Add(listing);
		lastDataArrivalTime = OmenServiceBase<StandardTimeManager>.Instance().Now;
	}

	private void eMZCEdv3E2(object? sender, ElapsedEventArgs e)
	{
		RunBackground(DYTCbB3B0j);
	}

	private unsafe async Task DYTCbB3B0j(CancellationToken cancellationToken)
	{
		if (data.IsEmpty)
		{
			if (!(OmenServiceBase<StandardTimeManager>.Instance().Now - lastDataArrivalTime > TimeSpan.FromMinutes(10L)) || !dJaCqy0n9I())
			{
				return;
			}
			AgentLookingForGroup* ptr = AgentLookingForGroup.Instance();
			if (ptr != null && !(*Addons.LookingForGroup).IsAddonAndNodesReady())
			{
				(*(AgentInterface*)ptr).SendEvent(1uL, 17);
			}
		}
		using List<KeyValuePair<int, List<IPartyFinderListing>>>.Enumerator enumerator = data.ToList().GetEnumerator();
		if (enumerator.MoveNext())
		{
			enumerator.Current.Deconstruct(out var key, out var value);
			int key2 = key;
			List<IPartyFinderListing> source = value;
			data.TryRemove(key2, out value);
			string content = JsonConvert.SerializeObject((object)source.Select((IPartyFinderListing listing) => new UploadableListing(listing)).ToList());
			_ = UploadClient;
			NGoCF1lm4l();
			new StringContent(content).Headers.ContentType = MediaTypeHeaderValue.Parse(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x34A2279E ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5e416caedcb6438f915024f390fd9079));
			_ = cancellationToken;
			throw null;
		}
	}

	private static string NGoCF1lm4l()
	{
		if (!GameState.IsGL)
		{
			return GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x75EEFCA9 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_11422b9d07f9498fb995b7dd8e417b5b);
		}
		return GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x65409BDA ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_33b91e080b77482a8d7d3bd797a9d8fb);
	}

	private unsafe static bool dJaCqy0n9I()
	{
		if (DataUploaderBase.IsPlayerReady() && !(*Addons.LookingForGroup).IsAddonAndNodesReady())
		{
			return ((UIModule)UIModule.Instance()).IsMainCommandUnlocked(57u);
		}
		return false;
	}

	public tso1jlCoGyhNFmSXU03()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		uploadTimer = new System.Timers.Timer(10000.0);
		data = new ConcurrentDictionary<int, List<IPartyFinderListing>>();
		lastDataArrivalTime = DateTime.MinValue;
		base._002Ector();
	}
}
