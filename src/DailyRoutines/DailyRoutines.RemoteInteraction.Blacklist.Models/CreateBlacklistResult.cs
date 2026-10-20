using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using B2nED1Wmdu0xsDR7Tjv;
using itAnDoW4mgYVlrmtMor;

namespace DailyRoutines.RemoteInteraction.Blacklist.Models;

public sealed class CreateBlacklistResult : IEquatable<CreateBlacklistResult>
{
	[CompilerGenerated]
	private Type EqualityContract
	{
		[CompilerGenerated]
		get
		{
			return typeof(CreateBlacklistResult);
		}
	}

	public bool Success { get; init; }

	public string BlacklistID { get; init; }

	public string Message { get; init; }

	public CreateBlacklistResult(bool Success, string BlacklistID, string Message)
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
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x27BB27A9 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7f9a8e3b1ea249bebe4aa86658ee9503));
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x702AB36D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ed1539c28915419fb405793dc0185c30));
		if (RYoLRAMexS(stringBuilder))
		{
			stringBuilder.Append(' ');
		}
		stringBuilder.Append('}');
		return stringBuilder.ToString();
	}

	[CompilerGenerated]
	private bool RYoLRAMexS(StringBuilder builder)
	{
		RuntimeHelpers.EnsureSufficientExecutionStack();
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(-1487420814 ^ -387446979 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_881eb9df907345dea66a751ad6d65e76));
		builder.Append(Success.ToString());
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2829F049 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_95ac0b4adf764381bcdc75a9d241d184));
		builder.Append((object?)BlacklistID);
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x9DD0410 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1cc312ba3ec946acba954139762d703f));
		builder.Append((object?)Message);
		return true;
	}

	[CompilerGenerated]
	public static bool operator !=(CreateBlacklistResult? left, CreateBlacklistResult? right)
	{
		return !(left == right);
	}

	[CompilerGenerated]
	public static bool operator ==(CreateBlacklistResult? left, CreateBlacklistResult? right)
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
		return Equals(obj as CreateBlacklistResult);
	}

	[CompilerGenerated]
	public bool Equals(CreateBlacklistResult? other)
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
	private CreateBlacklistResult(CreateBlacklistResult original)
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
