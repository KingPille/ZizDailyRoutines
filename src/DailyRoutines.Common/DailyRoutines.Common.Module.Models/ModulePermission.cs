using System.Runtime.CompilerServices;
using OmenTools.OmenService;

namespace DailyRoutines.Common.Module.Models;

public sealed class ModulePermission
{
	[CompilerGenerated]
	private readonly bool _003CNeedAuth_003Ek__BackingField;

	[CompilerGenerated]
	private readonly bool _003CCNPremium_003Ek__BackingField;

	[CompilerGenerated]
	private readonly bool _003CTCPremium_003Ek__BackingField;

	public bool NeedAuth
	{
		[CompilerGenerated]
		get
		{
			return false;
		}
		[CompilerGenerated]
		init
		{
			_003CNeedAuth_003Ek__BackingField = false;
		}
	}

	public bool CNOnly { get; init; }

	public bool TCOnly { get; init; }

	public bool KROnly { get; init; }

	public bool GLOnly { get; init; }

	public bool CNPremium
	{
		[CompilerGenerated]
		get
		{
			return false;
		}
		[CompilerGenerated]
		init
		{
			_003CCNPremium_003Ek__BackingField = false;
		}
	}

	public bool GLPremium { get; init; }

	public bool TCPremium
	{
		[CompilerGenerated]
		get
		{
			return false;
		}
		[CompilerGenerated]
		init
		{
			_003CTCPremium_003Ek__BackingField = false;
		}
	}

	public bool KRPremium { get; init; }

	public bool AllDefaultEnabled { get; init; }

	public bool CNDefaultEnabled { get; init; }

	public bool GLDefaultEnabled { get; init; }

	public bool TCDefaultEnabled { get; init; }

	public bool KRDefaultEnabled { get; init; }

	public bool IsSkip()
	{
		if (!CNOnly && !TCOnly && !KROnly && !GLOnly)
		{
			return false;
		}
		return (!CNOnly || !GameState.IsCN) && (!TCOnly || !GameState.IsTC) && (!KROnly || !GameState.IsKR) && (!GLOnly || !GameState.IsGL);
	}

	public bool IsHide(bool isAuth, bool isPremium)
	{
		bool num = IsSkip();
		bool flag = NeedAuth && !isAuth;
		bool flag2 = CNPremium && GameState.IsCN && !isPremium;
		bool flag3 = GLPremium && GameState.IsGL && !isPremium;
		bool flag4 = TCPremium && GameState.IsTC && !isPremium;
		bool flag5 = KRPremium && GameState.IsKR && !isPremium;
		return num || flag || flag2 || flag3 || flag4 || flag5;
	}

	public bool IsDefaultEnabled()
	{
		if (AllDefaultEnabled)
		{
			return true;
		}
		bool result = false;
		if (GameState.IsCN && CNDefaultEnabled)
		{
			result = true;
		}
		if (GameState.IsGL && GLDefaultEnabled)
		{
			result = true;
		}
		if (GameState.IsTC && TCDefaultEnabled)
		{
			result = true;
		}
		if (GameState.IsKR && KRDefaultEnabled)
		{
			result = true;
		}
		return result;
	}
}
