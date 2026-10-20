using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.RemoteInteraction.Blacklist.Models.Responses;
using itAnDoW4mgYVlrmtMor;

namespace DailyRoutines.RemoteInteraction.Blacklist.Models;

public sealed class CreateSharedBlacklistArgs : IEquatable<CreateSharedBlacklistArgs>
{
	[CompilerGenerated]
	private Type EqualityContract
	{
		[CompilerGenerated]
		get
		{
			return typeof(CreateSharedBlacklistArgs);
		}
	}

	public string Name { get; init; }

	public string Password { get; init; }

	public DateTime ExpiresAt { get; init; }

	public IReadOnlyList<SharedBlacklistEntry> Entries { get; init; }

	public CreateSharedBlacklistArgs(string Name, string Password, DateTime ExpiresAt, IReadOnlyList<SharedBlacklistEntry> Entries)
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		this.Name = Name;
		this.Password = Password;
		this.ExpiresAt = ExpiresAt;
		this.Entries = Entries;
		base._002Ector();
	}

	[CompilerGenerated]
	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2F0C277 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d964b0b1313049f490f2107d03a6ed3a));
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x200E07AD ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_062a9cfe150f4dc6a9fdd7b6d00434af));
		if (pMJLDeH24q(stringBuilder))
		{
			stringBuilder.Append(' ');
		}
		stringBuilder.Append('}');
		return stringBuilder.ToString();
	}

	[CompilerGenerated]
	private bool pMJLDeH24q(StringBuilder builder)
	{
		RuntimeHelpers.EnsureSufficientExecutionStack();
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2D358DC8 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_27ab9f3154924788ac627b1ee5fccf33));
		builder.Append((object?)Name);
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4118B833 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_55c846b516154967bf0c321596d2dc52));
		builder.Append((object?)Password);
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(-864210836 ^ -765863529 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_86afda88005749b094dac2a7d6b27c25));
		builder.Append(ExpiresAt.ToString());
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x32BCBA7 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_14783916c53948389577bd0e927537b1));
		builder.Append(Entries);
		return true;
	}

	[CompilerGenerated]
	public static bool operator !=(CreateSharedBlacklistArgs? left, CreateSharedBlacklistArgs? right)
	{
		return !(left == right);
	}

	[CompilerGenerated]
	public static bool operator ==(CreateSharedBlacklistArgs? left, CreateSharedBlacklistArgs? right)
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
		return (((EqualityComparer<Type>.Default.GetHashCode(EqualityContract) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Password)) * -1521134295 + EqualityComparer<DateTime>.Default.GetHashCode(ExpiresAt)) * -1521134295 + EqualityComparer<IReadOnlyList<SharedBlacklistEntry>>.Default.GetHashCode(Entries);
	}

	[CompilerGenerated]
	public override bool Equals(object? obj)
	{
		return Equals(obj as CreateSharedBlacklistArgs);
	}

	[CompilerGenerated]
	public bool Equals(CreateSharedBlacklistArgs? other)
	{
		if ((object)this != other)
		{
			if ((object)other != null && EqualityContract == other.EqualityContract && EqualityComparer<string>.Default.Equals(Name, other.Name) && EqualityComparer<string>.Default.Equals(Password, other.Password) && EqualityComparer<DateTime>.Default.Equals(ExpiresAt, other.ExpiresAt))
			{
				return EqualityComparer<IReadOnlyList<SharedBlacklistEntry>>.Default.Equals(Entries, other.Entries);
			}
			return false;
		}
		return true;
	}

	[CompilerGenerated]
	private CreateSharedBlacklistArgs(CreateSharedBlacklistArgs original)
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
		Name = original.Name;
		Password = original.Password;
		ExpiresAt = original.ExpiresAt;
		Entries = original.Entries;
	}

	[CompilerGenerated]
	public void Deconstruct(out string Name, out string Password, out DateTime ExpiresAt, out IReadOnlyList<SharedBlacklistEntry> Entries)
	{
		Name = this.Name;
		Password = this.Password;
		ExpiresAt = this.ExpiresAt;
		Entries = this.Entries;
	}
}
