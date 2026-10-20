using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using B2nED1Wmdu0xsDR7Tjv;
using itAnDoW4mgYVlrmtMor;

namespace DailyRoutines.RemoteInteraction.Blacklist.Models;

public sealed class BlacklistContentValue : IEquatable<BlacklistContentValue>
{
	[CompilerGenerated]
	private Type EqualityContract
	{
		[CompilerGenerated]
		get
		{
			return typeof(BlacklistContentValue);
		}
	}

	public BlacklistInfo Info { get; init; }

	public List<BlacklistEntry> Entries { get; init; }

	public BlacklistContentValue(BlacklistInfo Info, List<BlacklistEntry> Entries)
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		this.Info = Info;
		this.Entries = Entries;
		base._002Ector();
	}

	[CompilerGenerated]
	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x35A85EBD ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_95b251cdaa8148b7af54d7956ad9ed2a));
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x70C32B41 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c041f06f3e304b758cab1109bed434d0));
		if (rbfLamLuwN(stringBuilder))
		{
			stringBuilder.Append(' ');
		}
		stringBuilder.Append('}');
		return stringBuilder.ToString();
	}

	[CompilerGenerated]
	private bool rbfLamLuwN(StringBuilder builder)
	{
		RuntimeHelpers.EnsureSufficientExecutionStack();
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5132B78E ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_fc39b8b86ffd43198ab35059b444b39b));
		builder.Append(Info);
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3F341DAC ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_128c630676db4a8ab2ed83db4c2b1862));
		builder.Append(Entries);
		return true;
	}

	[CompilerGenerated]
	public static bool operator !=(BlacklistContentValue? left, BlacklistContentValue? right)
	{
		return !(left == right);
	}

	[CompilerGenerated]
	public static bool operator ==(BlacklistContentValue? left, BlacklistContentValue? right)
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
		return (EqualityComparer<Type>.Default.GetHashCode(EqualityContract) * -1521134295 + EqualityComparer<BlacklistInfo>.Default.GetHashCode(Info)) * -1521134295 + EqualityComparer<List<BlacklistEntry>>.Default.GetHashCode(Entries);
	}

	[CompilerGenerated]
	public override bool Equals(object? obj)
	{
		return Equals(obj as BlacklistContentValue);
	}

	[CompilerGenerated]
	public bool Equals(BlacklistContentValue? other)
	{
		if ((object)this != other)
		{
			if ((object)other != null && EqualityContract == other.EqualityContract && EqualityComparer<BlacklistInfo>.Default.Equals(Info, other.Info))
			{
				return EqualityComparer<List<BlacklistEntry>>.Default.Equals(Entries, other.Entries);
			}
			return false;
		}
		return true;
	}

	[CompilerGenerated]
	private BlacklistContentValue(BlacklistContentValue original)
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
		Info = original.Info;
		Entries = original.Entries;
	}

	[CompilerGenerated]
	public void Deconstruct(out BlacklistInfo Info, out List<BlacklistEntry> Entries)
	{
		Info = this.Info;
		Entries = this.Entries;
	}
}
