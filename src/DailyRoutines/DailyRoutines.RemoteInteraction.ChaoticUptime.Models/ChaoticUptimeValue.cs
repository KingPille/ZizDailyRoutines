using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using B2nED1Wmdu0xsDR7Tjv;
using itAnDoW4mgYVlrmtMor;

namespace DailyRoutines.RemoteInteraction.ChaoticUptime.Models;

public readonly struct ChaoticUptimeValue : IEquatable<ChaoticUptimeValue>
{
	public bool IsUptime { get; init; }

	public ChaoticUptimeValue(bool IsUptime)
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		this.IsUptime = IsUptime;
	}

	[CompilerGenerated]
	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x34A3AFCA ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5e416caedcb6438f915024f390fd9079));
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x75EF480D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_11422b9d07f9498fb995b7dd8e417b5b));
		if (rTyL2l7HLj(stringBuilder))
		{
			stringBuilder.Append(' ');
		}
		stringBuilder.Append('}');
		return stringBuilder.ToString();
	}

	[CompilerGenerated]
	private bool rTyL2l7HLj(StringBuilder builder)
	{
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x7C6D36B8 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d2f07ce64d464f30b1d7397ae7500cac));
		builder.Append(IsUptime.ToString());
		return true;
	}

	[CompilerGenerated]
	public static bool operator !=(ChaoticUptimeValue left, ChaoticUptimeValue right)
	{
		return !(left == right);
	}

	[CompilerGenerated]
	public static bool operator ==(ChaoticUptimeValue left, ChaoticUptimeValue right)
	{
		return left.Equals(right);
	}

	[CompilerGenerated]
	public override int GetHashCode()
	{
		return EqualityComparer<bool>.Default.GetHashCode(IsUptime);
	}

	[CompilerGenerated]
	public override bool Equals(object obj)
	{
		if (obj is ChaoticUptimeValue)
		{
			return Equals((ChaoticUptimeValue)obj);
		}
		return false;
	}

	[CompilerGenerated]
	public bool Equals(ChaoticUptimeValue other)
	{
		return EqualityComparer<bool>.Default.Equals(IsUptime, other.IsUptime);
	}

	[CompilerGenerated]
	public void Deconstruct(out bool IsUptime)
	{
		IsUptime = this.IsUptime;
	}
}
