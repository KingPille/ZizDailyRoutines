using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.RemoteInteraction.Universalis.Models.Responses;
using itAnDoW4mgYVlrmtMor;

namespace sDR5tOLCj6misk13q98;

internal sealed class V7mTsSLftZnW02qKyMo : IEquatable<V7mTsSLftZnW02qKyMo>
{
	[CompilerGenerated]
	private Type EqualityContract
	{
		[CompilerGenerated]
		get
		{
			return typeof(V7mTsSLftZnW02qKyMo);
		}
	}

	public uint? WorldID { get; init; }

	public string DCName { get; init; }

	public string RegionName { get; init; }

	public string WorldName { get; init; }

	public V7mTsSLftZnW02qKyMo(uint? WorldID, string DCName, string RegionName, string WorldName)
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		this.WorldID = WorldID;
		this.DCName = DCName;
		this.RegionName = RegionName;
		this.WorldName = WorldName;
		base._002Ector();
	}

	public static V7mTsSLftZnW02qKyMo mlgLjP6iZc(UniversalisResponseBase response)
	{
		return new V7mTsSLftZnW02qKyMo(response.WorldID, response.DCName, response.RegionName, response.WorldName);
	}

	public void JBlLO7asIc(UniversalisResponseBase response, IReadOnlyCollection<uint> itemIDs, IReadOnlyCollection<uint> unresolvedItems)
	{
		response.ItemIds = itemIDs.ToList();
		response.UnresolvedItems = unresolvedItems.ToList();
		response.WorldID = WorldID;
		response.DCName = DCName;
		response.RegionName = RegionName;
		response.WorldName = WorldName;
	}

	[CompilerGenerated]
	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6E47BB2B ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ad43e19aac164f32ab06e23cbc10bc4e));
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x9DD10E0 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1cc312ba3ec946acba954139762d703f));
		if (SHoLKGg98Y(stringBuilder))
		{
			stringBuilder.Append(' ');
		}
		stringBuilder.Append('}');
		return stringBuilder.ToString();
	}

	[CompilerGenerated]
	private bool SHoLKGg98Y(StringBuilder builder)
	{
		RuntimeHelpers.EnsureSufficientExecutionStack();
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5369C910 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d4ce857df7b047629e7091bbc815dd94));
		builder.Append(WorldID.ToString());
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x329DEB24 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7042112693cd412186fee5da4a8f8e32));
		builder.Append((object?)DCName);
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2F0D459 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d964b0b1313049f490f2107d03a6ed3a));
		builder.Append((object?)RegionName);
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x28170932 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_dc626e3e84304b9d975c6d744befebcd));
		builder.Append((object?)WorldName);
		return true;
	}

	[CompilerGenerated]
	public static bool operator !=(V7mTsSLftZnW02qKyMo? left, V7mTsSLftZnW02qKyMo? right)
	{
		return !(left == right);
	}

	[CompilerGenerated]
	public static bool operator ==(V7mTsSLftZnW02qKyMo? left, V7mTsSLftZnW02qKyMo? right)
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
		return (((EqualityComparer<Type>.Default.GetHashCode(EqualityContract) * -1521134295 + EqualityComparer<uint?>.Default.GetHashCode(WorldID)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(DCName)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(RegionName)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(WorldName);
	}

	[CompilerGenerated]
	public override bool Equals(object? obj)
	{
		return Equals(obj as V7mTsSLftZnW02qKyMo);
	}

	[CompilerGenerated]
	public bool Equals(V7mTsSLftZnW02qKyMo? other)
	{
		if ((object)this != other)
		{
			if ((object)other != null && EqualityContract == other.EqualityContract && EqualityComparer<uint?>.Default.Equals(WorldID, other.WorldID) && EqualityComparer<string>.Default.Equals(DCName, other.DCName) && EqualityComparer<string>.Default.Equals(RegionName, other.RegionName))
			{
				return EqualityComparer<string>.Default.Equals(WorldName, other.WorldName);
			}
			return false;
		}
		return true;
	}

	[CompilerGenerated]
	public V7mTsSLftZnW02qKyMo iZ1LtwSPkr()
	{
		return new V7mTsSLftZnW02qKyMo(this);
	}

	[CompilerGenerated]
	private V7mTsSLftZnW02qKyMo(V7mTsSLftZnW02qKyMo original)
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
		WorldID = original.WorldID;
		DCName = original.DCName;
		RegionName = original.RegionName;
		WorldName = original.WorldName;
	}

	[CompilerGenerated]
	public void AC2LWcioXL(out uint? WorldID, out string DCName, out string RegionName, out string WorldName)
	{
		WorldID = this.WorldID;
		DCName = this.DCName;
		RegionName = this.RegionName;
		WorldName = this.WorldName;
	}
}
