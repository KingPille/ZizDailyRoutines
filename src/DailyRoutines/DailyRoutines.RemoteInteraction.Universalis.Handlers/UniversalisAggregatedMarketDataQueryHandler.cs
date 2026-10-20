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
using DailyRoutines.RemoteInteraction.Universalis.Models.Responses;
using Newtonsoft.Json;
using itAnDoW4mgYVlrmtMor;

namespace DailyRoutines.RemoteInteraction.Universalis.Handlers;

public sealed class UniversalisAggregatedMarketDataQueryHandler : IRemoteQueryHandler<UniversalisAggregatedMarketDataKey, UniversalisAggregatedMarketDataResponse>
{
	private readonly struct maGYKbj8RergGIfQfl2 : IEquatable<maGYKbj8RergGIfQfl2>
	{
		public string WorldOrDC { get; init; }

		public maGYKbj8RergGIfQfl2(string WorldOrDC)
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			this.WorldOrDC = WorldOrDC;
		}

		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3922015E ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f6778e21204a4d43ae5bbed939acf8a2));
			stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x70C32B41 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c041f06f3e304b758cab1109bed434d0));
			if (BNAjvg41wa(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		[CompilerGenerated]
		private bool BNAjvg41wa(StringBuilder builder)
		{
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x75EEFB8D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_11422b9d07f9498fb995b7dd8e417b5b));
			builder.Append((object?)WorldOrDC);
			return true;
		}

		[CompilerGenerated]
		public static bool operator !=(maGYKbj8RergGIfQfl2 left, maGYKbj8RergGIfQfl2 right)
		{
			return !(left == right);
		}

		[CompilerGenerated]
		public static bool operator ==(maGYKbj8RergGIfQfl2 left, maGYKbj8RergGIfQfl2 right)
		{
			return left.Equals(right);
		}

		[CompilerGenerated]
		public override int GetHashCode()
		{
			return EqualityComparer<string>.Default.GetHashCode(WorldOrDC);
		}

		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			if (obj is maGYKbj8RergGIfQfl2)
			{
				return Equals((maGYKbj8RergGIfQfl2)obj);
			}
			return false;
		}

		[CompilerGenerated]
		public bool Equals(maGYKbj8RergGIfQfl2 other)
		{
			return EqualityComparer<string>.Default.Equals(WorldOrDC, other.WorldOrDC);
		}

		[CompilerGenerated]
		public void yoDjld5rY6(out string WorldOrDC)
		{
			WorldOrDC = this.WorldOrDC;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass7_0
	{
		public OA68AQLixwJkjkQM610<maGYKbj8RergGIfQfl2, UniversalisAggregatedMarketResult, byte>.JLryW2C7gXhkXuE0FyJ result;

		public _003C_003Ec__DisplayClass7_0()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
		}

		internal UniversalisAggregatedMarketResult uyIjxsfsjt(uint itemID)
		{
			return result.Items[itemID];
		}
	}

	private static readonly OA68AQLixwJkjkQM610<maGYKbj8RergGIfQfl2, UniversalisAggregatedMarketResult, byte> Batcher;

	public static TimeSpan TTL { get; }

	public static TimeSpan FailureTTL { get; }

	public static async ValueTask<UniversalisAggregatedMarketDataResponse> FetchAsync(UniversalisAggregatedMarketDataKey key, CancellationToken cancellationToken)
	{
		_003C_003Ec__DisplayClass7_0 CS_0024_003C_003E8__locals4 = new _003C_003Ec__DisplayClass7_0();
		List<uint> itemIds = ny1etPLTndeSqdSqyAx.oC4L4jmGZJ(key.ItemIDsCSV);
		CS_0024_003C_003E8__locals4.result = await Batcher.u6bLwF01uU(new maGYKbj8RergGIfQfl2(key.WorldOrDC), itemIds, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		return new UniversalisAggregatedMarketDataResponse
		{
			Results = (from itemID in itemIds.Where(CS_0024_003C_003E8__locals4.result.Items.ContainsKey)
				select CS_0024_003C_003E8__locals4.result.Items[itemID]).ToList(),
			FailedItems = CS_0024_003C_003E8__locals4.result.MissingItems.ToList()
		};
	}

	private static async ValueTask<OA68AQLixwJkjkQM610<maGYKbj8RergGIfQfl2, UniversalisAggregatedMarketResult, byte>.ppwNjbCa9sByBYVVAfh> usKLAtM4X5(maGYKbj8RergGIfQfl2 groupKey, IReadOnlyCollection<uint> itemIDs, CancellationToken cancellationToken)
	{
		UniversalisAggregatedMarketDataResponse universalisAggregatedMarketDataResponse = JsonConvert.DeserializeObject<UniversalisAggregatedMarketDataResponse>(await ny1etPLTndeSqdSqyAx.EU4Ll0t6Wd(ny1etPLTndeSqdSqyAx.KLTL8U9imE(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x702B00C1 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ed1539c28915419fb405793dc0185c30), groupKey.WorldOrDC, itemIDs, null), cancellationToken).ConfigureAwait(continueOnCapturedContext: false)) ?? throw new InvalidOperationException(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2D3437DE ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_27ab9f3154924788ac627b1ee5fccf33));
		return new OA68AQLixwJkjkQM610<maGYKbj8RergGIfQfl2, UniversalisAggregatedMarketResult, byte>.ppwNjbCa9sByBYVVAfh(universalisAggregatedMarketDataResponse.Results.ToDictionary((UniversalisAggregatedMarketResult result) => result.ItemID), universalisAggregatedMarketDataResponse.FailedItems, 0);
	}

	public UniversalisAggregatedMarketDataQueryHandler()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
	}

	static UniversalisAggregatedMarketDataQueryHandler()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		Batcher = new OA68AQLixwJkjkQM610<maGYKbj8RergGIfQfl2, UniversalisAggregatedMarketResult, byte>(usKLAtM4X5, ny1etPLTndeSqdSqyAx.AggregatedCacheTTL, ny1etPLTndeSqdSqyAx.BatchWindow, 100);
		TTL = ny1etPLTndeSqdSqyAx.AggregatedCacheTTL;
		FailureTTL = ny1etPLTndeSqdSqyAx.FailureTTL;
	}
}
