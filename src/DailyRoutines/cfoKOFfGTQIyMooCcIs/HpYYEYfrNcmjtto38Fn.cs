using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines;
using DailyRoutines.Common.DataUploader.Abstractions;
using DailyRoutines.Common.DataUploader.Extensions;
using DailyRoutines.Manager;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using FFXIVClientStructs.FFXIV.Client.Game.InstanceContent;
using FFXIVClientStructs.Interop;
using FFXIVClientStructs.STD.Helper;
using Newtonsoft.Json;
using OmenTools.Dalamud;
using OmenTools.Extensions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using itAnDoW4mgYVlrmtMor;
using ycTZ7OSV787Z3a0g7JT;

namespace cfoKOFfGTQIyMooCcIs;

internal sealed class HpYYEYfrNcmjtto38Fn : DataUploaderBase
{
	private sealed class RXCnY5KNCk5v0s95oqU : IEquatable<RXCnY5KNCk5v0s95oqU>
	{
		[CompilerGenerated]
		private Type EqualityContract
		{
			[CompilerGenerated]
			get
			{
				return typeof(RXCnY5KNCk5v0s95oqU);
			}
		}

		public uint CurrentWorldID { get; init; }

		public uint DataCenterID { get; init; }

		public uint TerritoryID { get; init; }

		public uint ZoneServerID { get; init; }

		public long ObservedAt { get; init; }

		public EventType EventType { get; init; }

		public uint EventID { get; init; }

		public long SpawnedAt { get; init; }

		public string ObservedState { get; init; }

		public RXCnY5KNCk5v0s95oqU(uint CurrentWorldID, uint DataCenterID, uint TerritoryID, uint ZoneServerID, long ObservedAt, EventType EventType, uint EventID, long SpawnedAt, string ObservedState)
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			this.CurrentWorldID = CurrentWorldID;
			this.DataCenterID = DataCenterID;
			this.TerritoryID = TerritoryID;
			this.ZoneServerID = ZoneServerID;
			this.ObservedAt = ObservedAt;
			this.EventType = EventType;
			this.EventID = EventID;
			this.SpawnedAt = SpawnedAt;
			this.ObservedState = ObservedState;
			base._002Ector();
		}

		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1FFBCEDD ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_a46716ea783442acb36e95ced1c935fc));
			stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x34A3A576 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5e416caedcb6438f915024f390fd9079));
			if (wBQK79489Q(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		[CompilerGenerated]
		private bool wBQK79489Q(StringBuilder builder)
		{
			RuntimeHelpers.EnsureSufficientExecutionStack();
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x76DBD2E1 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f8c073ebdce5414baf147b1be2d2c2c2));
			builder.Append(CurrentWorldID.ToString());
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x611629F4 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_8883645b187042ce8990c01069a51402));
			builder.Append(DataCenterID.ToString());
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1E251DEB ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_86afda88005749b094dac2a7d6b27c25));
			builder.Append(TerritoryID.ToString());
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3D7D9E4C ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_cda76bbf50614e97b4ef38939106c571));
			builder.Append(ZoneServerID.ToString());
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1F4A7878 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_0d3e4cb7200c49c9943fa2d7c1024d32));
			builder.Append(ObservedAt.ToString());
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3922534C ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f6778e21204a4d43ae5bbed939acf8a2));
			builder.Append(EventType.ToString());
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x702B52C3 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ed1539c28915419fb405793dc0185c30));
			builder.Append(EventID.ToString());
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x768F9223 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_417cae18312d413c80505172551e7b46));
			builder.Append(SpawnedAt.ToString());
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x51335DD2 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_fc39b8b86ffd43198ab35059b444b39b));
			builder.Append((object?)ObservedState);
			return true;
		}

		[CompilerGenerated]
		public static bool operator !=(RXCnY5KNCk5v0s95oqU? left, RXCnY5KNCk5v0s95oqU? right)
		{
			return !(left == right);
		}

		[CompilerGenerated]
		public static bool operator ==(RXCnY5KNCk5v0s95oqU? left, RXCnY5KNCk5v0s95oqU? right)
		{
			if ((object)left != right)
			{
				return left?.Equals(right) ?? false;
			}
			return true;
		}

		[CompilerGenerated]
		public override int GetHashCode()
		{
			return ((((((((EqualityComparer<Type>.Default.GetHashCode(EqualityContract) * -1521134295 + EqualityComparer<uint>.Default.GetHashCode(CurrentWorldID)) * -1521134295 + EqualityComparer<uint>.Default.GetHashCode(DataCenterID)) * -1521134295 + EqualityComparer<uint>.Default.GetHashCode(TerritoryID)) * -1521134295 + EqualityComparer<uint>.Default.GetHashCode(ZoneServerID)) * -1521134295 + EqualityComparer<long>.Default.GetHashCode(ObservedAt)) * -1521134295 + EqualityComparer<EventType>.Default.GetHashCode(EventType)) * -1521134295 + EqualityComparer<uint>.Default.GetHashCode(EventID)) * -1521134295 + EqualityComparer<long>.Default.GetHashCode(SpawnedAt)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ObservedState);
		}

		[CompilerGenerated]
		public override bool Equals(object? obj)
		{
			return Equals(obj as RXCnY5KNCk5v0s95oqU);
		}

		[CompilerGenerated]
		public bool Equals(RXCnY5KNCk5v0s95oqU? other)
		{
			if ((object)this != other)
			{
				if ((object)other != null && EqualityContract == other.EqualityContract && EqualityComparer<uint>.Default.Equals(CurrentWorldID, other.CurrentWorldID) && EqualityComparer<uint>.Default.Equals(DataCenterID, other.DataCenterID) && EqualityComparer<uint>.Default.Equals(TerritoryID, other.TerritoryID) && EqualityComparer<uint>.Default.Equals(ZoneServerID, other.ZoneServerID) && EqualityComparer<long>.Default.Equals(ObservedAt, other.ObservedAt) && EqualityComparer<EventType>.Default.Equals(EventType, other.EventType) && EqualityComparer<uint>.Default.Equals(EventID, other.EventID) && EqualityComparer<long>.Default.Equals(SpawnedAt, other.SpawnedAt))
				{
					return EqualityComparer<string>.Default.Equals(ObservedState, other.ObservedState);
				}
				return false;
			}
			return true;
		}

		[CompilerGenerated]
		public RXCnY5KNCk5v0s95oqU nrgKdDmja9()
		{
			return new RXCnY5KNCk5v0s95oqU(this);
		}

		[CompilerGenerated]
		private RXCnY5KNCk5v0s95oqU(RXCnY5KNCk5v0s95oqU original)
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
			CurrentWorldID = original.CurrentWorldID;
			DataCenterID = original.DataCenterID;
			TerritoryID = original.TerritoryID;
			ZoneServerID = original.ZoneServerID;
			ObservedAt = original.ObservedAt;
			EventType = original.EventType;
			EventID = original.EventID;
			SpawnedAt = original.SpawnedAt;
			ObservedState = original.ObservedState;
		}

		[CompilerGenerated]
		public void JjlKI1L6V7(out uint CurrentWorldID, out uint DataCenterID, out uint TerritoryID, out uint ZoneServerID, out long ObservedAt, out EventType EventType, out uint EventID, out long SpawnedAt, out string ObservedState)
		{
			CurrentWorldID = this.CurrentWorldID;
			DataCenterID = this.DataCenterID;
			TerritoryID = this.TerritoryID;
			ZoneServerID = this.ZoneServerID;
			ObservedAt = this.ObservedAt;
			EventType = this.EventType;
			EventID = this.EventID;
			SpawnedAt = this.SpawnedAt;
			ObservedState = this.ObservedState;
		}
	}

	private sealed class pHYppQKUqRgjxppbhIE : DataUploaderConfig
	{
		public string InstallationID;

		public List<p0aAAkKuWHkq9jUXqb1> Outbox;

		public pHYppQKUqRgjxppbhIE()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			InstallationID = string.Empty;
			Outbox = new List<p0aAAkKuWHkq9jUXqb1>();
			base._002Ector();
		}
	}

	private sealed class p0aAAkKuWHkq9jUXqb1
	{
		public string EventKey;

		public uint CurrentWorldID;

		public uint DataCenterID;

		public uint TerritoryID;

		public uint ZoneServerID;

		public long ObservedAt;

		public EventType EventType;

		public uint EventID;

		public long SpawnedAt;

		public string ObservedState;

		public string ReporterEpochID;

		public long CreatedAt;

		public int AttemptCount;

		public long NextAttemptAt;

		public p0aAAkKuWHkq9jUXqb1()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			EventKey = string.Empty;
			ObservedState = string.Empty;
			ReporterEpochID = string.Empty;
			base._002Ector();
		}
	}

	private sealed class SamplingConfig
	{
		[JsonProperty("schemaVersion")]
		public int SchemaVersion { get; set; }

		[JsonProperty("enabled")]
		public bool Enabled { get; set; }

		[JsonProperty("samplingRate")]
		public double SamplingRate { get; set; }

		[JsonProperty("dataCenterRates")]
		public Dictionary<uint, double> DataCenterRates { get; set; }

		[JsonProperty("dailySalt")]
		public string DailySalt { get; set; }

		public SamplingConfig()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			SchemaVersion = 1;
			Enabled = true;
			SamplingRate = 1.0;
			DataCenterRates = new Dictionary<uint, double>();
			DailySalt = GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x75EEAA0F ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_11422b9d07f9498fb995b7dd8e417b5b);
			base._002Ector();
		}
	}

	private sealed class UploadResponse
	{
		[JsonProperty("samplingRate")]
		public double SamplingRate { get; set; }

		[JsonProperty("results")]
		public List<UploadResult> Results { get; set; }

		public UploadResponse()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			SamplingRate = -1.0;
			Results = new List<UploadResult>();
			base._002Ector();
		}
	}

	private sealed class pCZH0AKX74itrP0TXLU
	{
		public double SamplingRate;

		public long LastObservedAt;

		public bool IsBootstrapped;

		public pCZH0AKX74itrP0TXLU()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			SamplingRate = 1.0;
			base._002Ector();
		}
	}

	private sealed class UploadResult
	{
		[JsonProperty("eventType")]
		public EventType EventType { get; set; }

		[JsonProperty("eventID")]
		public uint EventID { get; set; }

		[JsonProperty("spawnedAt")]
		public long SpawnedAt { get; set; }

		[JsonProperty("status")]
		public string Status { get; set; }

		public UploadResult()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			Status = string.Empty;
			base._002Ector();
		}
	}

	private enum EventType : byte
	{
		CE,
		FATE
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass18_0
	{
		public long now;

		public p0aAAkKuWHkq9jUXqb1 first;

		public _003C_003Ec__DisplayClass18_0()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
		}

		internal bool ULpKY7E3sM(p0aAAkKuWHkq9jUXqb1 entry)
		{
			return entry.NextAttemptAt <= now;
		}

		internal bool taWKzaNUtj(p0aAAkKuWHkq9jUXqb1 entry)
		{
			if (entry.NextAttemptAt <= now && entry.CurrentWorldID == first.CurrentWorldID && entry.DataCenterID == first.DataCenterID && entry.TerritoryID == first.TerritoryID && entry.ZoneServerID == first.ZoneServerID)
			{
				return entry.ReporterEpochID == first.ReporterEpochID;
			}
			return false;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass18_1
	{
		public p0aAAkKuWHkq9jUXqb1 entry;

		public _003C_003Ec__DisplayClass18_1()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
		}

		internal bool MpQtnIVokM(UploadResult item)
		{
			if (item.EventType == entry.EventType && item.EventID == entry.EventID)
			{
				return item.SpawnedAt == entry.SpawnedAt;
			}
			return false;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass20_0
	{
		public long now;

		public _003C_003Ec__DisplayClass20_0()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
		}

		internal bool yrVty1OboK(KeyValuePair<string, long> entry)
		{
			return now - entry.Value >= 7200;
		}

		internal bool QVHtLS5sCB(KeyValuePair<string, pCZH0AKX74itrP0TXLU> entry)
		{
			return now - entry.Value.LastObservedAt >= 7200;
		}

		internal bool GLwtH6pvpm(p0aAAkKuWHkq9jUXqb1 entry)
		{
			if (now - entry.CreatedAt < 1200)
			{
				return entry.AttemptCount >= 3;
			}
			return true;
		}
	}

	private readonly Channel<RXCnY5KNCk5v0s95oqU> observationChannel;

	private readonly Dictionary<string, long> observedEvents;

	private readonly Dictionary<string, pCZH0AKX74itrP0TXLU> adaptiveSamplingStates;

	private pHYppQKUqRgjxppbhIE config;

	private SamplingConfig samplingConfig;

	private long lastSamplingRefresh;

	private bool isConfigDirty;

	private string territoryEpochSalt;

	private const uint SOUTH_HORN_TERRITORY_ID = 1252u;

	private const uint NORTH_HORN_TERRITORY_ID = 1346u;

	private const uint BOZJA_SOUTH_FRONT_TERRITORY_ID = 920u;

	private const uint ZADNOR_TERRITORY_ID = 975u;

	private const uint ANEMOS_TERRITORY_ID = 732u;

	private const uint PAGOS_TERRITORY_ID = 763u;

	private const uint PYROS_TERRITORY_ID = 795u;

	private const uint HYDATOS_TERRITORY_ID = 827u;

	private const long OBSERVED_EVENT_TTL_SECONDS = 7200L;

	private const long OUTBOX_TTL_SECONDS = 1200L;

	private const long SAMPLING_REFRESH_SECONDS = 900L;

	protected override Task Init()
	{
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Expected O, but got Unknown
		config = DataUploaderConfigExtension.Load<pHYppQKUqRgjxppbhIE>(this) ?? new pHYppQKUqRgjxppbhIE();
		if (config.Outbox.RemoveAll((p0aAAkKuWHkq9jUXqb1 entry) => entry.EventID == 0) > 0)
		{
			isConfigDirty = true;
		}
		if (string.IsNullOrWhiteSpace(config.InstallationID))
		{
			config.InstallationID = Convert.ToHexStringLower(RandomNumberGenerator.GetBytes(16));
			isConfigDirty = true;
		}
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += vmjf6PEkbN;
		OmenServiceBase<FrameworkManager>.Instance().Reg(new OnUpdateDelegate(YH3fk3xBla), 10000u);
		RunBackground(qETfasYmD8);
		return Task.CompletedTask;
	}

	protected override Task Stop()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= vmjf6PEkbN;
		OmenServiceBase<FrameworkManager>.Instance().Unreg(YH3fk3xBla);
		observationChannel.Writer.TryComplete();
		return Task.CompletedTask;
	}

	protected override Task Uninit()
	{
		if (isConfigDirty)
		{
			config.Save(this);
		}
		return Task.CompletedTask;
	}

	private void vmjf6PEkbN(uint zone)
	{
		territoryEpochSalt = Convert.ToHexStringLower(RandomNumberGenerator.GetBytes(16));
	}

	private unsafe void YH3fk3xBla(IFramework framework)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Invalid comparison between Unknown and I4
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Invalid comparison between Unknown and I4
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Invalid comparison between Unknown and I4
		uint territoryType = GameState.TerritoryType;
		if ((int)GameState.TerritoryIntendedUse == 41)
		{
			uint currentWorld = GameState.CurrentWorld;
			uint currentDataCenter = GameState.CurrentDataCenter;
			uint zoneServerID = GameState.ZoneServerID;
			long serverTimeUnix = GameState.ServerTimeUnix;
			if (currentWorld == 0 || currentDataCenter == 0 || zoneServerID == 0 || serverTimeUnix <= 0)
			{
				return;
			}
			Yvff22ITLm(territoryType, currentWorld, currentDataCenter, zoneServerID, serverTimeUnix);
		}
		if ((int)GameState.TerritoryIntendedUse == 48)
		{
			uint currentWorld2 = GameState.CurrentWorld;
			uint currentDataCenter2 = GameState.CurrentDataCenter;
			uint zoneServerID2 = GameState.ZoneServerID;
			long serverTimeUnix2 = GameState.ServerTimeUnix;
			if (currentWorld2 == 0 || currentDataCenter2 == 0 || zoneServerID2 == 0 || serverTimeUnix2 <= 0)
			{
				return;
			}
			PublicContentBozja* instance = PublicContentBozja.GetInstance();
			if (instance == null)
			{
				return;
			}
			fpwfZQ9Vbg(&((PublicContentBozja)instance).DynamicEventContainer, territoryType, currentWorld2, currentDataCenter2, zoneServerID2, serverTimeUnix2);
		}
		if ((int)GameState.TerritoryIntendedUse != 61)
		{
			return;
		}
		uint currentWorld3 = GameState.CurrentWorld;
		uint currentDataCenter3 = GameState.CurrentDataCenter;
		uint zoneServerID3 = GameState.ZoneServerID;
		long serverTimeUnix3 = GameState.ServerTimeUnix;
		if (currentWorld3 != 0 && currentDataCenter3 != 0 && zoneServerID3 != 0 && serverTimeUnix3 > 0)
		{
			PublicContentOccultCrescent* instance2 = PublicContentOccultCrescent.GetInstance();
			if (instance2 != null)
			{
				fpwfZQ9Vbg(&((PublicContentOccultCrescent)instance2).DynamicEventContainer, territoryType, currentWorld3, currentDataCenter3, zoneServerID3, serverTimeUnix3);
				Yvff22ITLm(territoryType, currentWorld3, currentDataCenter3, zoneServerID3, serverTimeUnix3);
			}
		}
	}

	private unsafe void Yvff22ITLm(uint territoryID, uint currentWorldID, uint dataCenterID, uint zoneServerID, long observedAt)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Invalid comparison between Unknown and I4
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Invalid comparison between Unknown and I4
		UnmanagedArrayEnumerator<Pointer<FateContext>> enumerator = ((FateManager)FateManager.Instance()).Fates.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Pointer<FateContext> current = enumerator.Current;
				FateContext* value = current.Value;
				if (value == null)
				{
					continue;
				}
				uint fateId = ((FateContext)value).FateId;
				if (iFxfIgpH0L(territoryID, EventType.FATE, fateId) && ((FateContext)value).StartTimeEpoch > 0)
				{
					FateState state = ((FateContext)value).State;
					string text = (((int)state == 3) ? GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2816A34C ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_dc626e3e84304b9d975c6d744befebcd) : (((int)state != 4) ? string.Empty : GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x70C28211 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c041f06f3e304b758cab1109bed434d0)));
					string text2 = text;
					if (text2.Length != 0)
					{
						observationChannel.Writer.TryWrite(new RXCnY5KNCk5v0s95oqU(currentWorldID, dataCenterID, territoryID, zoneServerID, observedAt, EventType.FATE, fateId, ((FateContext)value).StartTimeEpoch, text2));
					}
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
	}

	private unsafe void fpwfZQ9Vbg(DynamicEventContainer* container, uint territoryID, uint currentWorldID, uint dataCenterID, uint zoneServerID, long observedAt)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Invalid comparison between Unknown and I4
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		DynamicEvent[] array = ((DynamicEventContainer)container).Events.ToArray();
		foreach (DynamicEvent val in array)
		{
			uint dynamicEventId = val.DynamicEventId;
			if (iFxfIgpH0L(territoryID, EventType.CE, dynamicEventId) && (int)val.State == 1)
			{
				long num = (long)val.StartTimestamp - 180L;
				if (num > 0)
				{
					observationChannel.Writer.TryWrite(new RXCnY5KNCk5v0s95oqU(currentWorldID, dataCenterID, territoryID, zoneServerID, observedAt, EventType.CE, dynamicEventId, num, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2BF05A60 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_fc2313f9e2c04f099d4c47c5893e635d)));
				}
			}
		}
	}

	private async Task qETfasYmD8(CancellationToken cancellationToken)
	{
		using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromSeconds(1L));
		_ = 2;
		try
		{
			while (await timer.WaitForNextTickAsync(cancellationToken))
			{
				long now = OmenServiceBase<StandardTimeManager>.Instance().UTCNowOffset.ToUnixTimeSeconds();
				if (now - lastSamplingRefresh >= 900)
				{
					await MSAf5tNQJi(cancellationToken);
				}
				RXCnY5KNCk5v0s95oqU item;
				while (observationChannel.Reader.TryRead(out item))
				{
					s0rf0O8VB9(item, now);
				}
				await Hg5fRSTtNm(now, cancellationToken);
				LqXfeV0xZ0(now);
				if (isConfigDirty)
				{
					config.Save(this);
					isConfigDirty = false;
				}
			}
		}
		catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
		{
			DLog.Debug(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3F35F38C ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_128c630676db4a8ab2ed83db4c2b1862));
		}
	}

	private void s0rf0O8VB9(RXCnY5KNCk5v0s95oqU observation, long now)
	{
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(5, 6);
		defaultInterpolatedStringHandler.AppendFormatted(observation.DataCenterID);
		defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(-1003249140 ^ -960308207 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d964b0b1313049f490f2107d03a6ed3a));
		defaultInterpolatedStringHandler.AppendFormatted(observation.TerritoryID);
		defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6B304982 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ac6250342b784829976646d28955b117));
		defaultInterpolatedStringHandler.AppendFormatted(observation.ZoneServerID);
		defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4F7151EF ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_3782f2a028bd4046aa095423cd2526e0));
		defaultInterpolatedStringHandler.AppendFormatted(observation.EventType);
		defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x76DB9B51 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f8c073ebdce5414baf147b1be2d2c2c2));
		defaultInterpolatedStringHandler.AppendFormatted(observation.EventID);
		defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x46294C31 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_93e1cfc241b644019a4819c49327d6a7));
		defaultInterpolatedStringHandler.AppendFormatted(observation.SpawnedAt);
		string text = defaultInterpolatedStringHandler.ToStringAndClear();
		if (!observedEvents.ContainsKey(text))
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(1, 2);
			defaultInterpolatedStringHandler2.AppendFormatted(observation.DataCenterID);
			defaultInterpolatedStringHandler2.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x329C415A ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7042112693cd412186fee5da4a8f8e32));
			defaultInterpolatedStringHandler2.AppendFormatted(observation.ZoneServerID);
			string key = defaultInterpolatedStringHandler2.ToStringAndClear();
			if (!adaptiveSamplingStates.TryGetValue(key, out pCZH0AKX74itrP0TXLU value))
			{
				value = new pCZH0AKX74itrP0TXLU();
				adaptiveSamplingStates.Add(key, value);
			}
			value.LastObservedAt = now;
			double num = Math.Min(samplingConfig.DataCenterRates.GetValueOrDefault(observation.DataCenterID, samplingConfig.SamplingRate), value.IsBootstrapped ? value.SamplingRate : 1.0);
			if (samplingConfig.Enabled && !(num <= 0.0) && (!(num < 1.0) || DGRfNIHpeM(text, num)))
			{
				observedEvents.Add(text, now);
				ulong num2 = (value.IsBootstrapped ? (AatfddfPRh(text + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x41191A6D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_55c846b516154967bf0c321596d2dc52) + config.InstallationID + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x65408768 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_33b91e080b77482a8d7d3bd797a9d8fb)) % 60) : (AatfddfPRh(text + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x473376A9 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5f88650ee89f435aad988a8583bda5b5) + config.InstallationID + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4CDA04C9 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_2765950fccc24d96a1162dd6779cbee1)) % 16));
				config.Outbox.Add(new p0aAAkKuWHkq9jUXqb1
				{
					EventKey = text,
					CurrentWorldID = observation.CurrentWorldID,
					DataCenterID = observation.DataCenterID,
					TerritoryID = observation.TerritoryID,
					ZoneServerID = observation.ZoneServerID,
					ObservedAt = observation.ObservedAt,
					EventType = observation.EventType,
					EventID = observation.EventID,
					SpawnedAt = observation.SpawnedAt,
					ObservedState = observation.ObservedState,
					ReporterEpochID = oanf7f9SFq(),
					CreatedAt = now,
					NextAttemptAt = now + (long)num2
				});
				isConfigDirty = true;
			}
		}
	}

	private async Task MSAf5tNQJi(CancellationToken cancellationToken)
	{
		lastSamplingRefresh = OmenServiceBase<StandardTimeManager>.Instance().UTCNowOffset.ToUnixTimeSeconds();
		try
		{
			SamplingConfig samplingConfig = JsonConvert.DeserializeObject<SamplingConfig>((await PqtKlnSSKxdRRHSb7mo.ktqSjVMsCG(InternalAPIRequestType.RequestCECrowdsourceSampling, null, cancellationToken)) ?? string.Empty);
			if (samplingConfig == null || samplingConfig.SchemaVersion != 1)
			{
				return;
			}
			double samplingRate = samplingConfig.SamplingRate;
			if (samplingRate >= 0.0 && samplingRate <= 1.0)
			{
				Dictionary<uint, double> dataCenterRates = samplingConfig.DataCenterRates;
				if (dataCenterRates != null && dataCenterRates.Count > 0 && samplingConfig.DataCenterRates.Values.All((double rate) => rate >= 0.0 && rate <= 1.0) && !string.IsNullOrWhiteSpace(samplingConfig.DailySalt))
				{
					this.samplingConfig = samplingConfig;
				}
			}
		}
		catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
		{
			throw;
		}
		catch (Exception ex2)
		{
			DLog.Error(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(-224923271 ^ -1578225634 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_019a6a2bbee24ddb9dd8d9a1d1e1ca09), ex2);
		}
	}

	private async Task Hg5fRSTtNm(long now, CancellationToken cancellationToken)
	{
		_003C_003Ec__DisplayClass18_0 CS_0024_003C_003E8__locals26 = new _003C_003Ec__DisplayClass18_0();
		CS_0024_003C_003E8__locals26.now = now;
		CS_0024_003C_003E8__locals26.first = (from entry in config.Outbox
			where entry.NextAttemptAt <= CS_0024_003C_003E8__locals26.now
			orderby entry.NextAttemptAt
			select entry).FirstOrDefault();
		if (CS_0024_003C_003E8__locals26.first == null)
		{
			return;
		}
		List<p0aAAkKuWHkq9jUXqb1> batch = (from entry in config.Outbox
			where entry.NextAttemptAt <= CS_0024_003C_003E8__locals26.now && entry.CurrentWorldID == CS_0024_003C_003E8__locals26.first.CurrentWorldID && entry.DataCenterID == CS_0024_003C_003E8__locals26.first.DataCenterID && entry.TerritoryID == CS_0024_003C_003E8__locals26.first.TerritoryID && entry.ZoneServerID == CS_0024_003C_003E8__locals26.first.ZoneServerID && entry.ReporterEpochID == CS_0024_003C_003E8__locals26.first.ReporterEpochID
			orderby entry.NextAttemptAt
			select entry).Take(8).ToList();
		var data = new
		{
			schemaVersion = 2,
			currentWorldID = CS_0024_003C_003E8__locals26.first.CurrentWorldID,
			dataCenterID = CS_0024_003C_003E8__locals26.first.DataCenterID,
			territoryID = CS_0024_003C_003E8__locals26.first.TerritoryID,
			zoneServerID = CS_0024_003C_003E8__locals26.first.ZoneServerID,
			observedAt = batch.Max((p0aAAkKuWHkq9jUXqb1 entry) => entry.ObservedAt),
			reporterEpochID = CS_0024_003C_003E8__locals26.first.ReporterEpochID,
			pluginVersion = Plugin.Version.ToString(),
			events = batch.Select((p0aAAkKuWHkq9jUXqb1 entry) => new
			{
				eventType = entry.EventType.ToString(),
				eventID = entry.EventID,
				spawnedAt = entry.SpawnedAt,
				observedState = entry.ObservedState
			}).ToList()
		};
		try
		{
			using HttpResponseMessage response = await PqtKlnSSKxdRRHSb7mo.PostAsync(InternalAPIRequestType.ReportCECrowdsource, data, cancellationToken);
			if (response == null)
			{
				{
					foreach (p0aAAkKuWHkq9jUXqb1 item in batch)
					{
						XjsfDU8Rn1(item, CS_0024_003C_003E8__locals26.now, extendedDelay: false);
					}
					return;
				}
			}
			if (response.StatusCode == HttpStatusCode.TooManyRequests || response.StatusCode >= HttpStatusCode.InternalServerError)
			{
				foreach (p0aAAkKuWHkq9jUXqb1 item2 in batch)
				{
					XjsfDU8Rn1(item2, CS_0024_003C_003E8__locals26.now, extendedDelay: true);
				}
				return;
			}
			if (!response.IsSuccessStatusCode)
			{
				foreach (p0aAAkKuWHkq9jUXqb1 item3 in batch)
				{
					config.Outbox.Remove(item3);
				}
				isConfigDirty = true;
				return;
			}
			UploadResponse uploadResponse = JsonConvert.DeserializeObject<UploadResponse>(await response.Content.ReadAsStringAsync(cancellationToken));
			if (uploadResponse != null)
			{
				double samplingRate = uploadResponse.SamplingRate;
				if (samplingRate >= 0.0 && samplingRate <= 1.0)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
					defaultInterpolatedStringHandler.AppendFormatted(CS_0024_003C_003E8__locals26.first.DataCenterID);
					defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4F7151EF ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_3782f2a028bd4046aa095423cd2526e0));
					defaultInterpolatedStringHandler.AppendFormatted(CS_0024_003C_003E8__locals26.first.ZoneServerID);
					string key = defaultInterpolatedStringHandler.ToStringAndClear();
					if (!adaptiveSamplingStates.TryGetValue(key, out pCZH0AKX74itrP0TXLU value))
					{
						value = new pCZH0AKX74itrP0TXLU();
						adaptiveSamplingStates.Add(key, value);
					}
					value.SamplingRate = uploadResponse.SamplingRate;
					value.IsBootstrapped = true;
					value.LastObservedAt = CS_0024_003C_003E8__locals26.now;
				}
			}
			using (List<p0aAAkKuWHkq9jUXqb1>.Enumerator enumerator = batch.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					_003C_003Ec__DisplayClass18_1 CS_0024_003C_003E8__locals29 = new _003C_003Ec__DisplayClass18_1();
					CS_0024_003C_003E8__locals29.entry = enumerator.Current;
					string text = (uploadResponse?.Results.FirstOrDefault((UploadResult item) => item.EventType == CS_0024_003C_003E8__locals29.entry.EventType && item.EventID == CS_0024_003C_003E8__locals29.entry.EventID && item.SpawnedAt == CS_0024_003C_003E8__locals29.entry.SpawnedAt))?.Status;
					if ((text == GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x462907E5 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_93e1cfc241b644019a4819c49327d6a7) || text == GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(-1487420814 ^ -779349373 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f8c073ebdce5414baf147b1be2d2c2c2) || text == GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x411951E7 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_55c846b516154967bf0c321596d2dc52) || text == GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x35A9B7CF ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_95b251cdaa8148b7af54d7956ad9ed2a)) ? true : false)
					{
						config.Outbox.Remove(CS_0024_003C_003E8__locals29.entry);
					}
					else
					{
						XjsfDU8Rn1(CS_0024_003C_003E8__locals29.entry, CS_0024_003C_003E8__locals26.now, extendedDelay: false);
					}
				}
			}
			isConfigDirty = true;
		}
		catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
		{
			throw;
		}
		catch (Exception ex2)
		{
			DLog.Error(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x75EEAB11 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_11422b9d07f9498fb995b7dd8e417b5b), ex2);
			foreach (p0aAAkKuWHkq9jUXqb1 item4 in batch)
			{
				XjsfDU8Rn1(item4, CS_0024_003C_003E8__locals26.now, extendedDelay: false);
			}
		}
	}

	private void XjsfDU8Rn1(p0aAAkKuWHkq9jUXqb1 entry, long now, bool extendedDelay)
	{
		entry.AttemptCount++;
		if (entry.AttemptCount >= 3 || now - entry.CreatedAt >= 1200)
		{
			config.Outbox.Remove(entry);
		}
		else
		{
			int num = ((extendedDelay || entry.AttemptCount >= 2) ? 120 : 30);
			entry.NextAttemptAt = now + num + Random.Shared.Next(0, 16);
		}
		isConfigDirty = true;
	}

	private void LqXfeV0xZ0(long now)
	{
		_003C_003Ec__DisplayClass20_0 CS_0024_003C_003E8__locals4 = new _003C_003Ec__DisplayClass20_0();
		CS_0024_003C_003E8__locals4.now = now;
		string[] array = (from entry in observedEvents
			where CS_0024_003C_003E8__locals4.now - entry.Value >= 7200
			select entry.Key).ToArray();
		foreach (string key in array)
		{
			observedEvents.Remove(key);
		}
		array = (from entry in adaptiveSamplingStates
			where CS_0024_003C_003E8__locals4.now - entry.Value.LastObservedAt >= 7200
			select entry.Key).ToArray();
		foreach (string key2 in array)
		{
			adaptiveSamplingStates.Remove(key2);
		}
		if (config.Outbox.RemoveAll((p0aAAkKuWHkq9jUXqb1 entry) => CS_0024_003C_003E8__locals4.now - entry.CreatedAt >= 1200 || entry.AttemptCount >= 3) > 0)
		{
			isConfigDirty = true;
		}
	}

	private bool DGRfNIHpeM(string eventKey, double samplingRate)
	{
		string value = OmenServiceBase<StandardTimeManager>.Instance().UTCNow.ToString(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x35B4B77A ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_042249f573d4431982cd93672ca35bd3), CultureInfo.InvariantCulture);
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 4);
		defaultInterpolatedStringHandler.AppendFormatted(samplingConfig.DailySalt);
		defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5583A59C ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5603c9a2cf17495cb5da1dda759bb08d));
		defaultInterpolatedStringHandler.AppendFormatted(value);
		defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(--1317373447 ^ 0x32E9131D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d2f07ce64d464f30b1d7397ae7500cac));
		defaultInterpolatedStringHandler.AppendFormatted(config.InstallationID);
		defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(-1487420814 ^ -1953093934 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c02509fc64c64faaa7b59b967c4e5f83));
		defaultInterpolatedStringHandler.AppendFormatted(eventKey);
		return (double)AatfddfPRh(defaultInterpolatedStringHandler.ToStringAndClear()) / 1.8446744073709552E+19 < samplingRate;
	}

	private string oanf7f9SFq()
	{
		string value = OmenServiceBase<StandardTimeManager>.Instance().UTCNow.ToString(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x41191AAB ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_55c846b516154967bf0c321596d2dc52), CultureInfo.InvariantCulture);
		Encoding uTF = Encoding.UTF8;
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 4);
		defaultInterpolatedStringHandler.AppendFormatted(config.InstallationID);
		defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x70C28237 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c041f06f3e304b758cab1109bed434d0));
		defaultInterpolatedStringHandler.AppendFormatted(samplingConfig.DailySalt);
		defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x35B4B7BC ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_042249f573d4431982cd93672ca35bd3));
		defaultInterpolatedStringHandler.AppendFormatted(value);
		defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x53686286 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d4ce857df7b047629e7091bbc815dd94));
		defaultInterpolatedStringHandler.AppendFormatted(territoryEpochSalt);
		return Convert.ToBase64String(SHA256.HashData(uTF.GetBytes(defaultInterpolatedStringHandler.ToStringAndClear()))).TrimEnd('=').Replace('+', '-')
			.Replace('/', '_');
	}

	private static ulong AatfddfPRh(string value)
	{
		return BinaryPrimitives.ReadUInt64BigEndian(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
	}

	private static bool iFxfIgpH0L(uint territoryID, EventType eventType, uint eventID)
	{
		switch (territoryID)
		{
		case 1252u:
			switch (eventType)
			{
			case EventType.CE:
				return eventID >= 33 && eventID <= 48;
			case EventType.FATE:
				return eventID - 1976 <= 1;
			}
			break;
		case 1346u:
			switch (eventType)
			{
			case EventType.CE:
				return eventID >= 49 && eventID <= 64;
			case EventType.FATE:
				return eventID - 2072 <= 1;
			}
			break;
		case 920u:
			if (eventType != EventType.CE)
			{
				break;
			}
			return eventID >= 1 && eventID <= 16;
		case 975u:
			if (eventType != EventType.CE)
			{
				break;
			}
			return eventID >= 17 && eventID <= 32;
		case 732u:
			if (eventType != EventType.FATE)
			{
				break;
			}
			return eventID >= 1328 && eventID <= 1348 && eventID != 1330;
		case 763u:
			if (eventType != EventType.FATE)
			{
				break;
			}
			return eventID >= 1351 && eventID <= 1369;
		case 795u:
			if (eventType != EventType.FATE)
			{
				break;
			}
			return eventID >= 1388 && eventID <= 1408;
		case 827u:
			if (eventType != EventType.FATE)
			{
				break;
			}
			return eventID >= 1412 && eventID <= 1425;
		}
		return false;
	}

	public HpYYEYfrNcmjtto38Fn()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		observationChannel = Channel.CreateBounded<RXCnY5KNCk5v0s95oqU>(new BoundedChannelOptions(256)
		{
			FullMode = BoundedChannelFullMode.DropOldest,
			SingleReader = true,
			SingleWriter = true
		});
		observedEvents = new Dictionary<string, long>();
		adaptiveSamplingStates = new Dictionary<string, pCZH0AKX74itrP0TXLU>();
		samplingConfig = new SamplingConfig();
		territoryEpochSalt = Convert.ToHexStringLower(RandomNumberGenerator.GetBytes(16));
		base._002Ector();
	}
}
