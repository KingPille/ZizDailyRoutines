using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.RemoteInteraction.Blacklist.Models.Responses;
using itAnDoW4mgYVlrmtMor;

namespace DailyRoutines.RemoteInteraction.Blacklist.Models;

public sealed class UpdateBlacklistArgs : IEquatable<UpdateBlacklistArgs>
{
	[CompilerGenerated]
	private Type EqualityContract
	{
		[CompilerGenerated]
		get
		{
			return typeof(UpdateBlacklistArgs);
		}
	}

	public string BlacklistID { get; init; }

	public string Password { get; init; }

	public IReadOnlyList<BlacklistPeopleInfo> Entries { get; init; }

	public UpdateBlacklistArgs(string BlacklistID, string Password, IReadOnlyList<BlacklistPeopleInfo> Entries)
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		this.BlacklistID = BlacklistID;
		this.Password = Password;
		this.Entries = Entries;
		base._002Ector();
	}

	[CompilerGenerated]
	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x339D3D90 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c48859e0be41429f8474d6e2fecfa0de));
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2F0D76B ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d964b0b1313049f490f2107d03a6ed3a));
		if (E83LNHgeAK(stringBuilder))
		{
			stringBuilder.Append(' ');
		}
		stringBuilder.Append('}');
		return stringBuilder.ToString();
	}

	[CompilerGenerated]
	private bool E83LNHgeAK(StringBuilder builder)
	{
		RuntimeHelpers.EnsureSufficientExecutionStack();
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x9DD1BE8 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1cc312ba3ec946acba954139762d703f));
		builder.Append((object?)BlacklistID);
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1E24F7F1 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_86afda88005749b094dac2a7d6b27c25));
		builder.Append((object?)Password);
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(--1470179789 ^ 0x1189C086 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_93e1cfc241b644019a4819c49327d6a7));
		builder.Append(Entries);
		return true;
	}

	[CompilerGenerated]
	public static bool operator !=(UpdateBlacklistArgs? left, UpdateBlacklistArgs? right)
	{
		return !(left == right);
	}

	[CompilerGenerated]
	public static bool operator ==(UpdateBlacklistArgs? left, UpdateBlacklistArgs? right)
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
		return ((EqualityComparer<Type>.Default.GetHashCode(EqualityContract) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(BlacklistID)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Password)) * -1521134295 + EqualityComparer<IReadOnlyList<BlacklistPeopleInfo>>.Default.GetHashCode(Entries);
	}

	[CompilerGenerated]
	public override bool Equals(object? obj)
	{
		return Equals(obj as UpdateBlacklistArgs);
	}

	[CompilerGenerated]
	public bool Equals(UpdateBlacklistArgs? other)
	{
		if ((object)this != other)
		{
			if ((object)other != null && EqualityContract == other.EqualityContract && EqualityComparer<string>.Default.Equals(BlacklistID, other.BlacklistID) && EqualityComparer<string>.Default.Equals(Password, other.Password))
			{
				return EqualityComparer<IReadOnlyList<BlacklistPeopleInfo>>.Default.Equals(Entries, other.Entries);
			}
			return false;
		}
		return true;
	}

	[CompilerGenerated]
	private UpdateBlacklistArgs(UpdateBlacklistArgs original)
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
		BlacklistID = original.BlacklistID;
		Password = original.Password;
		Entries = original.Entries;
	}

	[CompilerGenerated]
	public void Deconstruct(out string BlacklistID, out string Password, out IReadOnlyList<BlacklistPeopleInfo> Entries)
	{
		BlacklistID = this.BlacklistID;
		Password = this.Password;
		Entries = this.Entries;
	}
}
