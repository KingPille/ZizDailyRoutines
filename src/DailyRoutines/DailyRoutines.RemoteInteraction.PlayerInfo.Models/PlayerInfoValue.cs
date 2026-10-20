using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using B2nED1Wmdu0xsDR7Tjv;
using itAnDoW4mgYVlrmtMor;

namespace DailyRoutines.RemoteInteraction.PlayerInfo.Models;

public sealed class PlayerInfoValue : IEquatable<PlayerInfoValue>
{
	[CompilerGenerated]
	private Type EqualityContract
	{
		[CompilerGenerated]
		get
		{
			return typeof(PlayerInfoValue);
		}
	}

	public string Name { get; init; }

	public uint WorldID { get; init; }

	public PlayerInfoValue(string Name, uint WorldID)
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		this.Name = Name;
		this.WorldID = WorldID;
		base._002Ector();
	}

	[CompilerGenerated]
	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3D7C76BA ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_cda76bbf50614e97b4ef38939106c571));
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x75EF480D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_11422b9d07f9498fb995b7dd8e417b5b));
		if (vjwLGo7LCs(stringBuilder))
		{
			stringBuilder.Append(' ');
		}
		stringBuilder.Append('}');
		return stringBuilder.ToString();
	}

	[CompilerGenerated]
	private bool vjwLGo7LCs(StringBuilder builder)
	{
		RuntimeHelpers.EnsureSufficientExecutionStack();
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5E39F171 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f9eb04af496747c5b32cddd51d0b3997));
		builder.Append((object?)Name);
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x9DD1914 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1cc312ba3ec946acba954139762d703f));
		builder.Append(WorldID.ToString());
		return true;
	}

	[CompilerGenerated]
	public static bool operator !=(PlayerInfoValue? left, PlayerInfoValue? right)
	{
		return !(left == right);
	}

	[CompilerGenerated]
	public static bool operator ==(PlayerInfoValue? left, PlayerInfoValue? right)
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
		return (EqualityComparer<Type>.Default.GetHashCode(EqualityContract) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name)) * -1521134295 + EqualityComparer<uint>.Default.GetHashCode(WorldID);
	}

	[CompilerGenerated]
	public override bool Equals(object? obj)
	{
		return Equals(obj as PlayerInfoValue);
	}

	[CompilerGenerated]
	public bool Equals(PlayerInfoValue? other)
	{
		if ((object)this != other)
		{
			if ((object)other != null && EqualityContract == other.EqualityContract && EqualityComparer<string>.Default.Equals(Name, other.Name))
			{
				return EqualityComparer<uint>.Default.Equals(WorldID, other.WorldID);
			}
			return false;
		}
		return true;
	}

	[CompilerGenerated]
	private PlayerInfoValue(PlayerInfoValue original)
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
		Name = original.Name;
		WorldID = original.WorldID;
	}

	[CompilerGenerated]
	public void Deconstruct(out string Name, out uint WorldID)
	{
		Name = this.Name;
		WorldID = this.WorldID;
	}
}
