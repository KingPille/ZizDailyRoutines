using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using B2nED1Wmdu0xsDR7Tjv;
using itAnDoW4mgYVlrmtMor;

namespace DailyRoutines.RemoteInteraction.Blacklist.Models;

public sealed class UpdateBlacklistResult : IEquatable<UpdateBlacklistResult>
{
	[CompilerGenerated]
	private Type EqualityContract
	{
		[CompilerGenerated]
		get
		{
			return typeof(UpdateBlacklistResult);
		}
	}

	public bool Success { get; init; }

	public UpdateBlacklistResult(bool Success)
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		this.Success = Success;
		base._002Ector();
	}

	[CompilerGenerated]
	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(--1317373447 ^ 0x2BC4BD49 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_33b91e080b77482a8d7d3bd797a9d8fb));
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1F4B9914 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_0d3e4cb7200c49c9943fa2d7c1024d32));
		if (YZWL709Giw(stringBuilder))
		{
			stringBuilder.Append(' ');
		}
		stringBuilder.Append('}');
		return stringBuilder.ToString();
	}

	[CompilerGenerated]
	private bool YZWL709Giw(StringBuilder builder)
	{
		RuntimeHelpers.EnsureSufficientExecutionStack();
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2D3590BE ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_27ab9f3154924788ac627b1ee5fccf33));
		builder.Append(Success.ToString());
		return true;
	}

	[CompilerGenerated]
	public static bool operator !=(UpdateBlacklistResult? left, UpdateBlacklistResult? right)
	{
		return !(left == right);
	}

	[CompilerGenerated]
	public static bool operator ==(UpdateBlacklistResult? left, UpdateBlacklistResult? right)
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
		return EqualityComparer<Type>.Default.GetHashCode(EqualityContract) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(Success);
	}

	[CompilerGenerated]
	public override bool Equals(object? obj)
	{
		return Equals(obj as UpdateBlacklistResult);
	}

	[CompilerGenerated]
	public bool Equals(UpdateBlacklistResult? other)
	{
		if ((object)this != other)
		{
			if ((object)other != null && EqualityContract == other.EqualityContract)
			{
				return EqualityComparer<bool>.Default.Equals(Success, other.Success);
			}
			return false;
		}
		return true;
	}

	[CompilerGenerated]
	private UpdateBlacklistResult(UpdateBlacklistResult original)
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
		Success = original.Success;
	}

	[CompilerGenerated]
	public void Deconstruct(out bool Success)
	{
		Success = this.Success;
	}
}
