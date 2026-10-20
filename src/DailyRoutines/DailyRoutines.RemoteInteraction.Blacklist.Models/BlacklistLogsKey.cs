using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using B2nED1Wmdu0xsDR7Tjv;
using itAnDoW4mgYVlrmtMor;

namespace DailyRoutines.RemoteInteraction.Blacklist.Models;

public readonly struct BlacklistLogsKey : IEquatable<BlacklistLogsKey>
{
	public string BlacklistID { get; init; }

	public string Password { get; init; }

	public string? Player { get; init; }

	public string? Status { get; init; }

	public int Limit { get; init; }

	public string? StartTime { get; init; }

	public string? EndTime { get; init; }

	public BlacklistLogsKey(string BlacklistID, string Password, string? Player = null, string? Status = null, int Limit = 50, string? StartTime = null, string? EndTime = null)
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		this.BlacklistID = BlacklistID;
		this.Password = Password;
		this.Player = Player;
		this.Status = Status;
		this.Limit = Limit;
		this.StartTime = StartTime;
		this.EndTime = EndTime;
	}

	[CompilerGenerated]
	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x7D14E23F ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1dfb87339e314237a8eb0a6453e2a275));
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6E47B98D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ad43e19aac164f32ab06e23cbc10bc4e));
		if (r57L08IbhA(stringBuilder))
		{
			stringBuilder.Append(' ');
		}
		stringBuilder.Append('}');
		return stringBuilder.ToString();
	}

	[CompilerGenerated]
	private bool r57L08IbhA(StringBuilder builder)
	{
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1C78E52E ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_365ea33385244ba8b1c5da69c8292f00));
		builder.Append((object?)BlacklistID);
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2F0DC43 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d964b0b1313049f490f2107d03a6ed3a));
		builder.Append((object?)Password);
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x35B51536 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_042249f573d4431982cd93672ca35bd3));
		builder.Append((object?)Player);
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5582070C ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5603c9a2cf17495cb5da1dda759bb08d));
		builder.Append((object?)Status);
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2F0DC3D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d964b0b1313049f490f2107d03a6ed3a));
		builder.Append(Limit.ToString());
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x32BD3AB ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_14783916c53948389577bd0e927537b1));
		builder.Append((object?)StartTime);
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x9DD04C0 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1cc312ba3ec946acba954139762d703f));
		builder.Append((object?)EndTime);
		return true;
	}

	[CompilerGenerated]
	public static bool operator !=(BlacklistLogsKey left, BlacklistLogsKey right)
	{
		return !(left == right);
	}

	[CompilerGenerated]
	public static bool operator ==(BlacklistLogsKey left, BlacklistLogsKey right)
	{
		return left.Equals(right);
	}

	[CompilerGenerated]
	public override int GetHashCode()
	{
		return (((((EqualityComparer<string>.Default.GetHashCode(BlacklistID) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Password)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Player)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Status)) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(Limit)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(StartTime)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(EndTime);
	}

	[CompilerGenerated]
	public override bool Equals(object obj)
	{
		if (obj is BlacklistLogsKey)
		{
			return Equals((BlacklistLogsKey)obj);
		}
		return false;
	}

	[CompilerGenerated]
	public bool Equals(BlacklistLogsKey other)
	{
		if (EqualityComparer<string>.Default.Equals(BlacklistID, other.BlacklistID) && EqualityComparer<string>.Default.Equals(Password, other.Password) && EqualityComparer<string>.Default.Equals(Player, other.Player) && EqualityComparer<string>.Default.Equals(Status, other.Status) && EqualityComparer<int>.Default.Equals(Limit, other.Limit) && EqualityComparer<string>.Default.Equals(StartTime, other.StartTime))
		{
			return EqualityComparer<string>.Default.Equals(EndTime, other.EndTime);
		}
		return false;
	}

	[CompilerGenerated]
	public void Deconstruct(out string BlacklistID, out string Password, out string? Player, out string? Status, out int Limit, out string? StartTime, out string? EndTime)
	{
		BlacklistID = this.BlacklistID;
		Password = this.Password;
		Player = this.Player;
		Status = this.Status;
		Limit = this.Limit;
		StartTime = this.StartTime;
		EndTime = this.EndTime;
	}
}
