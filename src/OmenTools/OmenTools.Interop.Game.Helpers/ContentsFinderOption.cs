using System;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace OmenTools.Interop.Game.Helpers;

[StructLayout(LayoutKind.Explicit, Size = 10)]
public struct ContentsFinderOption : IEquatable<ContentsFinderOption>
{
	[FieldOffset(0)]
	public bool Supply;

	[FieldOffset(1)]
	public bool Config817to820;

	[FieldOffset(2)]
	public bool UnrestrictedParty;

	[FieldOffset(3)]
	public bool MinimalIL;

	[FieldOffset(4)]
	public bool LevelSync;

	[FieldOffset(5)]
	public bool SilenceEcho;

	[FieldOffset(6)]
	public bool ExplorerMode;

	[FieldOffset(7)]
	public LootRule LootRules;

	[FieldOffset(8)]
	public bool IsLimitedLevelingRoulette;

	[FieldOffset(9)]
	public bool Unknown9;

	public ContentsFinderOption Clone()
	{
		return this;
	}

	public override string ToString()
	{
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		return "ContentsFinderOption 配置:\n" + $"  中途加入: {Supply}\n" + $"  Config817to820: {Config817to820}\n" + $"  解除限制: {UnrestrictedParty}\n" + $"  最低装等同步: {MinimalIL}\n" + $"  等级同步: {LevelSync}\n" + $"  禁用超越之力: {SilenceEcho}\n" + $"  自由探索: {ExplorerMode}\n" + $"  分配方式: {LootRules}\n" + $"  限制随机任务：练级的目标: {IsLimitedLevelingRoulette}\n" + $"  Unknown9: {Unknown9}";
	}

	public unsafe static ContentsFinderOption Get()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		ContentsFinder contentsFinder = ((UIState)UIState.Instance()).ContentsFinder;
		return new ContentsFinderOption
		{
			Config817to820 = true,
			ExplorerMode = contentsFinder.IsExplorerMode,
			IsLimitedLevelingRoulette = contentsFinder.IsLimitedLevelingRoulette,
			LevelSync = contentsFinder.IsLevelSync,
			LootRules = contentsFinder.LootRules,
			MinimalIL = contentsFinder.IsMinimalIL,
			SilenceEcho = contentsFinder.IsSilenceEcho,
			UnrestrictedParty = contentsFinder.IsUnrestrictedParty
		};
	}

	public bool Equals(ContentsFinderOption other)
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		if (Supply == other.Supply && Config817to820 == other.Config817to820 && UnrestrictedParty == other.UnrestrictedParty && MinimalIL == other.MinimalIL && LevelSync == other.LevelSync && SilenceEcho == other.SilenceEcho && ExplorerMode == other.ExplorerMode && LootRules == other.LootRules && IsLimitedLevelingRoulette == other.IsLimitedLevelingRoulette)
		{
			return Unknown9 == other.Unknown9;
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is ContentsFinderOption other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Expected I4, but got Unknown
		HashCode hashCode = default(HashCode);
		hashCode.Add(Supply);
		hashCode.Add(Config817to820);
		hashCode.Add(UnrestrictedParty);
		hashCode.Add(MinimalIL);
		hashCode.Add(LevelSync);
		hashCode.Add(SilenceEcho);
		hashCode.Add(ExplorerMode);
		hashCode.Add((int)LootRules);
		hashCode.Add(IsLimitedLevelingRoulette);
		hashCode.Add(Unknown9);
		return hashCode.ToHashCode();
	}

	public static bool operator ==(ContentsFinderOption left, ContentsFinderOption right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(ContentsFinderOption left, ContentsFinderOption right)
	{
		return !left.Equals(right);
	}
}
