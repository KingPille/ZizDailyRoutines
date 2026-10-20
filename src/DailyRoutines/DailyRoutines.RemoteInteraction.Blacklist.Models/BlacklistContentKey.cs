using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using B2nED1Wmdu0xsDR7Tjv;
using itAnDoW4mgYVlrmtMor;

namespace DailyRoutines.RemoteInteraction.Blacklist.Models;

public readonly struct BlacklistContentKey : IEquatable<BlacklistContentKey>
{
	public string BlacklistID { get; init; }

	public string Password { get; init; }

	public int Limit { get; init; }

	public int Offset { get; init; }

	public BlacklistContentKey(string BlacklistID, string Password, int Limit = 1000, int Offset = 0)
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		this.BlacklistID = BlacklistID;
		this.Password = Password;
		this.Limit = Limit;
		this.Offset = Offset;
	}

	[CompilerGenerated]
	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5B208FA0 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_435f54c92c5b40a9ad2757dd36deed0a));
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4628E547 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_93e1cfc241b644019a4819c49327d6a7));
		if (UESLZHaiFa(stringBuilder))
		{
			stringBuilder.Append(' ');
		}
		stringBuilder.Append('}');
		return stringBuilder.ToString();
	}

	[CompilerGenerated]
	private bool UESLZHaiFa(StringBuilder builder)
	{
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x32BCCA3 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_14783916c53948389577bd0e927537b1));
		builder.Append((object?)BlacklistID);
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2E959F35 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_6103a9bab63e42479916a134b07f1c75));
		builder.Append((object?)Password);
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3923B994 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f6778e21204a4d43ae5bbed939acf8a2));
		builder.Append(Limit.ToString());
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4628EE39 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_93e1cfc241b644019a4819c49327d6a7));
		builder.Append(Offset.ToString());
		return true;
	}

	[CompilerGenerated]
	public static bool operator !=(BlacklistContentKey left, BlacklistContentKey right)
	{
		return !(left == right);
	}

	[CompilerGenerated]
	public static bool operator ==(BlacklistContentKey left, BlacklistContentKey right)
	{
		return left.Equals(right);
	}

	[CompilerGenerated]
	public override int GetHashCode()
	{
		return ((EqualityComparer<string>.Default.GetHashCode(BlacklistID) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Password)) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(Limit)) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(Offset);
	}

	[CompilerGenerated]
	public override bool Equals(object obj)
	{
		if (obj is BlacklistContentKey)
		{
			return Equals((BlacklistContentKey)obj);
		}
		return false;
	}

	[CompilerGenerated]
	public bool Equals(BlacklistContentKey other)
	{
		if (EqualityComparer<string>.Default.Equals(BlacklistID, other.BlacklistID) && EqualityComparer<string>.Default.Equals(Password, other.Password) && EqualityComparer<int>.Default.Equals(Limit, other.Limit))
		{
			return EqualityComparer<int>.Default.Equals(Offset, other.Offset);
		}
		return false;
	}

	[CompilerGenerated]
	public void Deconstruct(out string BlacklistID, out string Password, out int Limit, out int Offset)
	{
		BlacklistID = this.BlacklistID;
		Password = this.Password;
		Limit = this.Limit;
		Offset = this.Offset;
	}
}
