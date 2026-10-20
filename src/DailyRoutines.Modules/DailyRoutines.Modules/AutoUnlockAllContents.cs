using System.Collections.Frozen;
using System.Runtime.InteropServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Hooking;
using OmenTools.Interop.Game.Models;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules;

public class AutoUnlockAllContents : ModuleBase
{
	[return: MarshalAs(UnmanagedType.U1)]
	private delegate bool zbXXntxP9dIqFac92v6(uint contentID);

	[return: MarshalAs(UnmanagedType.U1)]
	private delegate bool WIGx52xhHZGu8SrDPXf(nint a1, ushort a2, byte a3, nint a4);

	private static readonly CompSig IsInstanceContentUnlockedSig;

	private Hook<zbXXntxP9dIqFac92v6>? IsInstanceContentUnlockedHook;

	private static readonly CompSig IsPartyFinderCategoryUnlockedSig;

	private Hook<WIGx52xhHZGu8SrDPXf>? IsPartyFinderCategoryUnlockedHook;

	private static readonly FrozenSet<uint> SpecialContent;

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected override void Init()
	{
		if (IsInstanceContentUnlockedHook == null)
		{
			IsInstanceContentUnlockedHook = IsInstanceContentUnlockedSig.GetHook<zbXXntxP9dIqFac92v6>(huJMTLhDxS);
		}
		IsInstanceContentUnlockedHook.Enable();
		if (IsPartyFinderCategoryUnlockedHook == null)
		{
			IsPartyFinderCategoryUnlockedHook = IsPartyFinderCategoryUnlockedSig.GetHook<WIGx52xhHZGu8SrDPXf>(I9qM1NY9pw);
		}
		IsPartyFinderCategoryUnlockedHook.Enable();
	}

	private static bool huJMTLhDxS(uint content)
	{
		if (SpecialContent.Contains(content))
		{
			return false;
		}
		return true;
	}

	private static bool I9qM1NY9pw(nint a1, ushort a2, byte a3, nint a4)
	{
		return true;
	}

	public AutoUnlockAllContents()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2CE50A1B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c9dc773883f149b7befba3bdbeac55f7)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x76EBB639 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b988ddaee02846b69dd4e446b5da9de9)),
			Category = ModuleCategory.Recruitment
		};
		Permission = new ModulePermission
		{
			NeedAuth = true
		};
		base._002Ector();
	}

	static AutoUnlockAllContents()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		IsInstanceContentUnlockedSig = new CompSig(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3987B35B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_56ae6e3f2cdf4dd8a743f4d493ce49a5));
		IsPartyFinderCategoryUnlockedSig = new CompSig(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6854C171 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_23522a2a14314da28eb44187a09cd0b5));
		SpecialContent = FrozenSet.Create<uint>(20014u, 20015u, 20016u);
	}
}
