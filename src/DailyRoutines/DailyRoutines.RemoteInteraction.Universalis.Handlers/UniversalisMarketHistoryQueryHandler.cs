using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using B2nED1Wmdu0xsDR7Tjv;
using BgEQrMLgIuIWcZIR7aG;
using CVg02HL1lNJ30jq90OI;
using DailyRoutines.Common.RemoteInteraction.Abstractions;
using DailyRoutines.RemoteInteraction.Universalis.Models;
using DailyRoutines.RemoteInteraction.Universalis.Models.Requests;
using DailyRoutines.RemoteInteraction.Universalis.Models.Responses;
using Newtonsoft.Json;
using itAnDoW4mgYVlrmtMor;
using sDR5tOLCj6misk13q98;

namespace DailyRoutines.RemoteInteraction.Universalis.Handlers;

public sealed class UniversalisMarketHistoryQueryHandler : IRemoteQueryHandler<UniversalisMarketHistoryKey, UniversalisMarketHistoryResponse>
{
	private readonly struct RxmaWkjmjZmgdJiF8hu : IEquatable<RxmaWkjmjZmgdJiF8hu>
	{
		public string WorldOrDC { get; init; }

		public UniversalisMarketHistoryRequestParams? Parameters { get; init; }

		public RxmaWkjmjZmgdJiF8hu(string WorldOrDC, UniversalisMarketHistoryRequestParams? Parameters)
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			this.WorldOrDC = WorldOrDC;
			this.Parameters = Parameters;
		}

		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x9068366 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_444e42c3749640698bcaba67a1c5b5f3));
			stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2CC04DD6 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c02509fc64c64faaa7b59b967c4e5f83));
			if (df2jisiZin(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		[CompilerGenerated]
		private bool df2jisiZin(StringBuilder builder)
		{
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(-1003249140 ^ -1435129855 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ad43e19aac164f32ab06e23cbc10bc4e));
			builder.Append((object?)WorldOrDC);
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2FDDFE0B ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c6c2389b72fc4ab3828ae71fae281bf1));
			builder.Append(Parameters);
			return true;
		}

		[CompilerGenerated]
		public static bool operator !=(RxmaWkjmjZmgdJiF8hu left, RxmaWkjmjZmgdJiF8hu right)
		{
			return !(left == right);
		}

		[CompilerGenerated]
		public static bool operator ==(RxmaWkjmjZmgdJiF8hu left, RxmaWkjmjZmgdJiF8hu right)
		{
			return left.Equals(right);
		}

		[CompilerGenerated]
		public override int GetHashCode()
		{
			return EqualityComparer<string>.Default.GetHashCode(WorldOrDC) * -1521134295 + EqualityComparer<UniversalisMarketHistoryRequestParams>.Default.GetHashCode(Parameters);
		}

		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			if (obj is RxmaWkjmjZmgdJiF8hu)
			{
				return Equals((RxmaWkjmjZmgdJiF8hu)obj);
			}
			return false;
		}

		[CompilerGenerated]
		public bool Equals(RxmaWkjmjZmgdJiF8hu other)
		{
			if (EqualityComparer<string>.Default.Equals(WorldOrDC, other.WorldOrDC))
			{
				return EqualityComparer<UniversalisMarketHistoryRequestParams>.Default.Equals(Parameters, other.Parameters);
			}
			return false;
		}

		[CompilerGenerated]
		public void iAjjPZKgcq(out string WorldOrDC, out UniversalisMarketHistoryRequestParams? Parameters)
		{
			WorldOrDC = this.WorldOrDC;
			Parameters = this.Parameters;
		}
	}

	private static readonly OA68AQLixwJkjkQM610<RxmaWkjmjZmgdJiF8hu, UniversalisHistoryItemData, V7mTsSLftZnW02qKyMo> Batcher;

	public static TimeSpan TTL { get; }

	public static TimeSpan FailureTTL { get; }

	public static async ValueTask<UniversalisMarketHistoryResponse> FetchAsync(UniversalisMarketHistoryKey key, CancellationToken cancellationToken)
	{
		List<uint> itemIds = ny1etPLTndeSqdSqyAx.oC4L4jmGZJ(key.ItemIDsCSV);
		OA68AQLixwJkjkQM610<RxmaWkjmjZmgdJiF8hu, UniversalisHistoryItemData, V7mTsSLftZnW02qKyMo>.JLryW2C7gXhkXuE0FyJ jLryW2C7gXhkXuE0FyJ = await Batcher.u6bLwF01uU(new RxmaWkjmjZmgdJiF8hu(key.WorldOrDC, key.Parameters), itemIds, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		UniversalisMarketHistoryResponse universalisMarketHistoryResponse = new UniversalisMarketHistoryResponse
		{
			Items = jLryW2C7gXhkXuE0FyJ.Items.ToDictionary((KeyValuePair<uint, UniversalisHistoryItemData> pair) => pair.Key, (KeyValuePair<uint, UniversalisHistoryItemData> pair) => pair.Value)
		};
		jLryW2C7gXhkXuE0FyJ.Metadata.JBlLO7asIc(universalisMarketHistoryResponse, itemIds, jLryW2C7gXhkXuE0FyJ.MissingItems);
		return universalisMarketHistoryResponse;
	}

	private static async ValueTask<OA68AQLixwJkjkQM610<RxmaWkjmjZmgdJiF8hu, UniversalisHistoryItemData, V7mTsSLftZnW02qKyMo>.ppwNjbCa9sByBYVVAfh> wtuLJaDn6d(RxmaWkjmjZmgdJiF8hu groupKey, IReadOnlyCollection<uint> itemIDs, CancellationToken cancellationToken)
	{
		UniversalisMarketHistoryResponse universalisMarketHistoryResponse = JsonConvert.DeserializeObject<UniversalisMarketHistoryResponse>(await ny1etPLTndeSqdSqyAx.EU4Ll0t6Wd(ny1etPLTndeSqdSqyAx.KLTL8U9imE(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x37CD5DEB ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_90dae8e50e3a4314867cef21f0bb1bb0), groupKey.WorldOrDC, itemIDs, groupKey.Parameters), cancellationToken).ConfigureAwait(continueOnCapturedContext: false)) ?? throw new InvalidOperationException(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4629581F ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_93e1cfc241b644019a4819c49327d6a7));
		return new OA68AQLixwJkjkQM610<RxmaWkjmjZmgdJiF8hu, UniversalisHistoryItemData, V7mTsSLftZnW02qKyMo>.ppwNjbCa9sByBYVVAfh(universalisMarketHistoryResponse.Items, universalisMarketHistoryResponse.UnresolvedItems, V7mTsSLftZnW02qKyMo.mlgLjP6iZc(universalisMarketHistoryResponse));
	}

	public UniversalisMarketHistoryQueryHandler()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
	}

	static UniversalisMarketHistoryQueryHandler()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		Batcher = new OA68AQLixwJkjkQM610<RxmaWkjmjZmgdJiF8hu, UniversalisHistoryItemData, V7mTsSLftZnW02qKyMo>(wtuLJaDn6d, ny1etPLTndeSqdSqyAx.HistoryCacheTTL, ny1etPLTndeSqdSqyAx.BatchWindow, 100);
		TTL = ny1etPLTndeSqdSqyAx.HistoryCacheTTL;
		FailureTTL = ny1etPLTndeSqdSqyAx.FailureTTL;
	}
}
