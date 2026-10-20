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

public sealed class UniversalisMarketDataQueryHandler : IRemoteQueryHandler<UniversalisMarketDataKey, UniversalisMarketDataResponse>
{
	private readonly struct GtIaDFjpCObfJl9mxP4 : IEquatable<GtIaDFjpCObfJl9mxP4>
	{
		public string WorldOrDC { get; init; }

		public UniversalisMarketDataRequestParams? Parameters { get; init; }

		public GtIaDFjpCObfJl9mxP4(string WorldOrDC, UniversalisMarketDataRequestParams? Parameters)
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			this.WorldOrDC = WorldOrDC;
			this.Parameters = Parameters;
		}

		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x70C298DD ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c041f06f3e304b758cab1109bed434d0));
			stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x38D3A466 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_59cab3a5f3464d2f8ff563ddb3d8bad1));
			if (icqj3lvW5S(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		[CompilerGenerated]
		private bool icqj3lvW5S(StringBuilder builder)
		{
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x51330FB8 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_fc39b8b86ffd43198ab35059b444b39b));
			builder.Append((object?)WorldOrDC);
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x75EF49B3 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_11422b9d07f9498fb995b7dd8e417b5b));
			builder.Append(Parameters);
			return true;
		}

		[CompilerGenerated]
		public static bool operator !=(GtIaDFjpCObfJl9mxP4 left, GtIaDFjpCObfJl9mxP4 right)
		{
			return !(left == right);
		}

		[CompilerGenerated]
		public static bool operator ==(GtIaDFjpCObfJl9mxP4 left, GtIaDFjpCObfJl9mxP4 right)
		{
			return left.Equals(right);
		}

		[CompilerGenerated]
		public override int GetHashCode()
		{
			return EqualityComparer<string>.Default.GetHashCode(WorldOrDC) * -1521134295 + EqualityComparer<UniversalisMarketDataRequestParams>.Default.GetHashCode(Parameters);
		}

		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			if (obj is GtIaDFjpCObfJl9mxP4)
			{
				return Equals((GtIaDFjpCObfJl9mxP4)obj);
			}
			return false;
		}

		[CompilerGenerated]
		public bool Equals(GtIaDFjpCObfJl9mxP4 other)
		{
			if (EqualityComparer<string>.Default.Equals(WorldOrDC, other.WorldOrDC))
			{
				return EqualityComparer<UniversalisMarketDataRequestParams>.Default.Equals(Parameters, other.Parameters);
			}
			return false;
		}

		[CompilerGenerated]
		public void YLTjBjg9k7(out string WorldOrDC, out UniversalisMarketDataRequestParams? Parameters)
		{
			WorldOrDC = this.WorldOrDC;
			Parameters = this.Parameters;
		}
	}

	private static readonly OA68AQLixwJkjkQM610<GtIaDFjpCObfJl9mxP4, UniversalisMarketItemData, V7mTsSLftZnW02qKyMo> Batcher;

	public static TimeSpan TTL { get; }

	public static TimeSpan FailureTTL { get; }

	public static async ValueTask<UniversalisMarketDataResponse> FetchAsync(UniversalisMarketDataKey key, CancellationToken cancellationToken)
	{
		List<uint> itemIds = ny1etPLTndeSqdSqyAx.oC4L4jmGZJ(key.ItemIDsCSV);
		OA68AQLixwJkjkQM610<GtIaDFjpCObfJl9mxP4, UniversalisMarketItemData, V7mTsSLftZnW02qKyMo>.JLryW2C7gXhkXuE0FyJ jLryW2C7gXhkXuE0FyJ = await Batcher.u6bLwF01uU(new GtIaDFjpCObfJl9mxP4(key.WorldOrDC, key.Parameters), itemIds, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		UniversalisMarketDataResponse universalisMarketDataResponse = new UniversalisMarketDataResponse
		{
			Items = jLryW2C7gXhkXuE0FyJ.Items.ToDictionary((KeyValuePair<uint, UniversalisMarketItemData> pair) => pair.Key, (KeyValuePair<uint, UniversalisMarketItemData> pair) => pair.Value)
		};
		jLryW2C7gXhkXuE0FyJ.Metadata.JBlLO7asIc(universalisMarketDataResponse, itemIds, jLryW2C7gXhkXuE0FyJ.MissingItems);
		return universalisMarketDataResponse;
	}

	private static async ValueTask<OA68AQLixwJkjkQM610<GtIaDFjpCObfJl9mxP4, UniversalisMarketItemData, V7mTsSLftZnW02qKyMo>.ppwNjbCa9sByBYVVAfh> I4TLQZD2xw(GtIaDFjpCObfJl9mxP4 groupKey, IReadOnlyCollection<uint> itemIDs, CancellationToken cancellationToken)
	{
		UniversalisMarketDataResponse universalisMarketDataResponse = JsonConvert.DeserializeObject<UniversalisMarketDataResponse>(await ny1etPLTndeSqdSqyAx.EU4Ll0t6Wd(ny1etPLTndeSqdSqyAx.KLTL8U9imE(string.Empty, groupKey.WorldOrDC, itemIDs, groupKey.Parameters), cancellationToken).ConfigureAwait(continueOnCapturedContext: false)) ?? throw new InvalidOperationException(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x9068DE2 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_444e42c3749640698bcaba67a1c5b5f3));
		return new OA68AQLixwJkjkQM610<GtIaDFjpCObfJl9mxP4, UniversalisMarketItemData, V7mTsSLftZnW02qKyMo>.ppwNjbCa9sByBYVVAfh(universalisMarketDataResponse.Items, universalisMarketDataResponse.UnresolvedItems, V7mTsSLftZnW02qKyMo.mlgLjP6iZc(universalisMarketDataResponse));
	}

	public UniversalisMarketDataQueryHandler()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
	}

	static UniversalisMarketDataQueryHandler()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		Batcher = new OA68AQLixwJkjkQM610<GtIaDFjpCObfJl9mxP4, UniversalisMarketItemData, V7mTsSLftZnW02qKyMo>(I4TLQZD2xw, ny1etPLTndeSqdSqyAx.MarketCacheTTL, ny1etPLTndeSqdSqyAx.BatchWindow, 100);
		TTL = ny1etPLTndeSqdSqyAx.MarketCacheTTL;
		FailureTTL = ny1etPLTndeSqdSqyAx.FailureTTL;
	}
}
