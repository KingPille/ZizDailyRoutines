using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using B2nED1Wmdu0xsDR7Tjv;
using itAnDoW4mgYVlrmtMor;

namespace DailyRoutines.RemoteInteraction.ChaoticUptime.Models;

public readonly struct ChaoticUptimeKey : IEquatable<ChaoticUptimeKey>
{
	public string DataCenter { get; init; }

	public ChaoticUptimeKey(string DataCenter)
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		this.DataCenter = DataCenter;
	}

	[CompilerGenerated]
	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1FFA247B ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_a46716ea783442acb36e95ced1c935fc));
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4732DFDF ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5f88650ee89f435aad988a8583bda5b5));
		if (C0JLksXpa7(stringBuilder))
		{
			stringBuilder.Append(' ');
		}
		stringBuilder.Append('}');
		return stringBuilder.ToString();
	}

	[CompilerGenerated]
	private bool C0JLksXpa7(StringBuilder builder)
	{
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x200E0D33 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_062a9cfe150f4dc6a9fdd7b6d00434af));
		builder.Append((object?)DataCenter);
		return true;
	}

	[CompilerGenerated]
	public static bool operator !=(ChaoticUptimeKey left, ChaoticUptimeKey right)
	{
		return !(left == right);
	}

	[CompilerGenerated]
	public static bool operator ==(ChaoticUptimeKey left, ChaoticUptimeKey right)
	{
		return left.Equals(right);
	}

	[CompilerGenerated]
	public override int GetHashCode()
	{
		return EqualityComparer<string>.Default.GetHashCode(DataCenter);
	}

	[CompilerGenerated]
	public override bool Equals(object obj)
	{
		if (obj is ChaoticUptimeKey)
		{
			return Equals((ChaoticUptimeKey)obj);
		}
		return false;
	}

	[CompilerGenerated]
	public bool Equals(ChaoticUptimeKey other)
	{
		return EqualityComparer<string>.Default.Equals(DataCenter, other.DataCenter);
	}

	[CompilerGenerated]
	public void Deconstruct(out string DataCenter)
	{
		DataCenter = this.DataCenter;
	}
}
