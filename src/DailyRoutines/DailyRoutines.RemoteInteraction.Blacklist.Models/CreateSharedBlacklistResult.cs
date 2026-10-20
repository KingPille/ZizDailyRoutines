using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using B2nED1Wmdu0xsDR7Tjv;
using itAnDoW4mgYVlrmtMor;

namespace DailyRoutines.RemoteInteraction.Blacklist.Models;

public sealed class CreateSharedBlacklistResult : IEquatable<CreateSharedBlacklistResult>
{
	[CompilerGenerated]
	private Type EqualityContract
	{
		[CompilerGenerated]
		get
		{
			return typeof(CreateSharedBlacklistResult);
		}
	}

	public bool Success { get; init; }

	public string BlacklistID { get; init; }

	public string Message { get; init; }

	public CreateSharedBlacklistResult(bool Success, string BlacklistID, string Message)
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		this.Success = Success;
		this.BlacklistID = BlacklistID;
		this.Message = Message;
		base._002Ector();
	}

	[CompilerGenerated]
	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x371EFF6 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_a86a125fa3c94b2ca72995b652fdcc2e));
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5132BC38 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_fc39b8b86ffd43198ab35059b444b39b));
		if (R4nLepbFGw(stringBuilder))
		{
			stringBuilder.Append(' ');
		}
		stringBuilder.Append('}');
		return stringBuilder.ToString();
	}

	[CompilerGenerated]
	private bool R4nLepbFGw(StringBuilder builder)
	{
		RuntimeHelpers.EnsureSufficientExecutionStack();
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3923A676 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f6778e21204a4d43ae5bbed939acf8a2));
		builder.Append(Success.ToString());
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x28171EBA ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_dc626e3e84304b9d975c6d744befebcd));
		builder.Append((object?)BlacklistID);
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x768E6719 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_417cae18312d413c80505172551e7b46));
		builder.Append((object?)Message);
		return true;
	}

	[CompilerGenerated]
	public static bool operator !=(CreateSharedBlacklistResult? left, CreateSharedBlacklistResult? right)
	{
		return !(left == right);
	}

	[CompilerGenerated]
	public static bool operator ==(CreateSharedBlacklistResult? left, CreateSharedBlacklistResult? right)
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
		return ((EqualityComparer<Type>.Default.GetHashCode(EqualityContract) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(Success)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(BlacklistID)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Message);
	}

	[CompilerGenerated]
	public override bool Equals(object? obj)
	{
		return Equals(obj as CreateSharedBlacklistResult);
	}

	[CompilerGenerated]
	public bool Equals(CreateSharedBlacklistResult? other)
	{
		if ((object)this != other)
		{
			if ((object)other != null && EqualityContract == other.EqualityContract && EqualityComparer<bool>.Default.Equals(Success, other.Success) && EqualityComparer<string>.Default.Equals(BlacklistID, other.BlacklistID))
			{
				return EqualityComparer<string>.Default.Equals(Message, other.Message);
			}
			return false;
		}
		return true;
	}

	[CompilerGenerated]
	private CreateSharedBlacklistResult(CreateSharedBlacklistResult original)
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
		Success = original.Success;
		BlacklistID = original.BlacklistID;
		Message = original.Message;
	}

	[CompilerGenerated]
	public void Deconstruct(out bool Success, out string BlacklistID, out string Message)
	{
		Success = this.Success;
		BlacklistID = this.BlacklistID;
		Message = this.Message;
	}
}
