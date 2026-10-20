using System;
using System.Collections.Generic;

namespace OmenTools.Dalamud;

public class RaphaelCaculationResponse(uint requestID, RaphaelCalculationStatus status, string error, List<uint> actions) : IEquatable<RaphaelCaculationResponse>
{
	public uint RequestID { get; init; } = requestID;

	public RaphaelCalculationStatus Status { get; init; } = status;

	public string ErrorMessage { get; init; } = error;

	public List<uint> Actions { get; init; } = actions;

	public bool Equals(RaphaelCaculationResponse? other)
	{
		if ((object)other == null)
		{
			return false;
		}
		if ((object)this == other)
		{
			return true;
		}
		return RequestID == other.RequestID;
	}

	public override bool Equals(object? obj)
	{
		return Equals(obj as RaphaelCaculationResponse);
	}

	public override int GetHashCode()
	{
		return (int)RequestID;
	}

	public static bool operator ==(RaphaelCaculationResponse? left, RaphaelCaculationResponse? right)
	{
		return object.Equals(left, right);
	}

	public static bool operator !=(RaphaelCaculationResponse? left, RaphaelCaculationResponse? right)
	{
		return !object.Equals(left, right);
	}
}
