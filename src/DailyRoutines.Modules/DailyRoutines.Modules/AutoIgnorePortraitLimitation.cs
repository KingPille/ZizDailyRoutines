using System;
using System.Collections.Frozen;
using System.Drawing;
using System.Numerics;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Hooking;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules;

public class AutoIgnorePortraitLimitation : ModuleBase
{
	private unsafe delegate int xOg0H1pW2P37qtw5vIT(CharaView* charaView);

	private static readonly CompSig GetCharaViewStateFlagSig;

	private Hook<xOg0H1pW2P37qtw5vIT> GetCharaViewStateFlagHook;

	private static readonly FrozenSet<uint> LogMessages;

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected unsafe override void Init()
	{
		if (GetCharaViewStateFlagHook == null)
		{
			GetCharaViewStateFlagHook = GetCharaViewStateFlagSig.GetHook<xOg0H1pW2P37qtw5vIT>(CJoMI9EWcW);
		}
		GetCharaViewStateFlagHook.Enable();
	}

	protected override void ConfigUI()
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(1, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x50176F6E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e227921f7ee94b6aa1c28169395ba0cf)));
		((ImU8String)(ref val)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D75256A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8b51f8019ce49018b38e06104a97fc1));
		ImGui.TextColored(ref vector, val);
		IndentDisposable val2 = ImRaii.PushIndent(1, true);
		try
		{
			foreach (uint logMessage in LogMessages)
			{
				ImGui.Bullet();
				ImGui.SameLine();
				ImGui.TextUnformatted(ImU8String.op_Implicit(LuminaWrapper.GetLogMessageText(logMessage)));
			}
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	private unsafe static int CJoMI9EWcW(CharaView* charaView)
	{
		return 0;
	}

	public AutoIgnorePortraitLimitation()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x68AC557E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4722689f1a4d41dc9dc800bfe1a89c09)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5E3BEB02 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2d7d8a5df6744c51b35f486f03b53ded)),
			Category = ModuleCategory.System
		};
		Permission = new ModulePermission
		{
			NeedAuth = true,
			AllDefaultEnabled = true
		};
		base._002Ector();
	}

	static AutoIgnorePortraitLimitation()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		GetCharaViewStateFlagSig = new CompSig(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x43D704F4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_974d377e1ed74986b81c34c95c2cc846));
		LogMessages = FrozenSet.Create<uint>(5861u, 5871u, 5872u, 5850u, 5882u);
	}
}
