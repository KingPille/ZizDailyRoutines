using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using B2nED1Wmdu0xsDR7Tjv;
using itAnDoW4mgYVlrmtMor;

namespace DailyRoutines.RemoteInteraction.Blacklist.Models;

public sealed class CreateBlacklistArgs : IEquatable<CreateBlacklistArgs>
{
	[CompilerGenerated]
	private Type EqualityContract
	{
		[CompilerGenerated]
		get
		{
			return typeof(CreateBlacklistArgs);
		}
	}

	public string Name { get; init; }

	public string Password { get; init; }

	public CreateBlacklistArgs(string Name, string Password)
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		this.Name = Name;
		this.Password = Password;
		base._002Ector();
	}

	[CompilerGenerated]
	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5132A874 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_fc39b8b86ffd43198ab35059b444b39b));
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4732DFDF ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5f88650ee89f435aad988a8583bda5b5));
		if (tSeL5X0SSI(stringBuilder))
		{
			stringBuilder.Append(' ');
		}
		stringBuilder.Append('}');
		return stringBuilder.ToString();
	}

	[CompilerGenerated]
	private bool tSeL5X0SSI(StringBuilder builder)
	{
		RuntimeHelpers.EnsureSufficientExecutionStack();
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x227FB35 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_e3177f14377e4fe58ddaaaaf90163f44));
		builder.Append((object?)Name);
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x37CCEBF5 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_90dae8e50e3a4314867cef21f0bb1bb0));
		builder.Append((object?)Password);
		return true;
	}

	[CompilerGenerated]
	public static bool operator !=(CreateBlacklistArgs? left, CreateBlacklistArgs? right)
	{
		return !(left == right);
	}

	[CompilerGenerated]
	public static bool operator ==(CreateBlacklistArgs? left, CreateBlacklistArgs? right)
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
		return (EqualityComparer<Type>.Default.GetHashCode(EqualityContract) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Password);
	}

	[CompilerGenerated]
	public override bool Equals(object? obj)
	{
		return Equals(obj as CreateBlacklistArgs);
	}

	[CompilerGenerated]
	public bool Equals(CreateBlacklistArgs? other)
	{
		if ((object)this != other)
		{
			if ((object)other != null && EqualityContract == other.EqualityContract && EqualityComparer<string>.Default.Equals(Name, other.Name))
			{
				return EqualityComparer<string>.Default.Equals(Password, other.Password);
			}
			return false;
		}
		return true;
	}

	[CompilerGenerated]
	private CreateBlacklistArgs(CreateBlacklistArgs original)
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
		Name = original.Name;
		Password = original.Password;
	}

	[CompilerGenerated]
	public void Deconstruct(out string Name, out string Password)
	{
		Name = this.Name;
		Password = this.Password;
	}
}
