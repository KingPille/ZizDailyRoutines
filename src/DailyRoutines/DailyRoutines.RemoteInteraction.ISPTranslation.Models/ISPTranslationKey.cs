using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using B2nED1Wmdu0xsDR7Tjv;
using itAnDoW4mgYVlrmtMor;

namespace DailyRoutines.RemoteInteraction.ISPTranslation.Models;

public readonly struct ISPTranslationKey : IEquatable<ISPTranslationKey>
{
	public string Text { get; init; }

	public string LocaleName { get; init; }

	public ISPTranslationKey(string Text, string LocaleName)
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		this.Text = Text;
		this.LocaleName = LocaleName;
	}

	[CompilerGenerated]
	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x37CCEACD ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_90dae8e50e3a4314867cef21f0bb1bb0));
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x35B51ECA ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_042249f573d4431982cd93672ca35bd3));
		if (FktL6fjLMN(stringBuilder))
		{
			stringBuilder.Append(' ');
		}
		stringBuilder.Append('}');
		return stringBuilder.ToString();
	}

	[CompilerGenerated]
	private bool FktL6fjLMN(StringBuilder builder)
	{
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(--367535148 ^ 0x35E629B7 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_062a9cfe150f4dc6a9fdd7b6d00434af));
		builder.Append((object?)Text);
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x32BCDF3 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_14783916c53948389577bd0e927537b1));
		builder.Append((object?)LocaleName);
		return true;
	}

	[CompilerGenerated]
	public static bool operator !=(ISPTranslationKey left, ISPTranslationKey right)
	{
		return !(left == right);
	}

	[CompilerGenerated]
	public static bool operator ==(ISPTranslationKey left, ISPTranslationKey right)
	{
		return left.Equals(right);
	}

	[CompilerGenerated]
	public override int GetHashCode()
	{
		return EqualityComparer<string>.Default.GetHashCode(Text) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(LocaleName);
	}

	[CompilerGenerated]
	public override bool Equals(object obj)
	{
		if (obj is ISPTranslationKey)
		{
			return Equals((ISPTranslationKey)obj);
		}
		return false;
	}

	[CompilerGenerated]
	public bool Equals(ISPTranslationKey other)
	{
		if (EqualityComparer<string>.Default.Equals(Text, other.Text))
		{
			return EqualityComparer<string>.Default.Equals(LocaleName, other.LocaleName);
		}
		return false;
	}

	[CompilerGenerated]
	public void Deconstruct(out string Text, out string LocaleName)
	{
		Text = this.Text;
		LocaleName = this.LocaleName;
	}
}
