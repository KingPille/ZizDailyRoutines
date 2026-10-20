using System;
using DailyRoutines.AnimationLockOptions;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using Lumina.Excel.Sheets;
using OmenTools.Global;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules;

public class AutoRemoveAnimationLockPunishment : ModuleBase
{
	private unsafe delegate void tJwKL5ldObyr11Vmn58(ActionManager* manager);

	private unsafe delegate void hf0tEIlIJMA8LvGOnnD(uint sourceID, nint sourceCharacter, nint pos, Header* effectHeader, Effect* effectArray, ulong* effectTrail);

	private class wyBPmblUT7C5mWd99NC : ModuleConfig
	{
		public float AnimationLock;

		public wyBPmblUT7C5mWd99NC()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}
	}

	private static readonly CompSig ActionManagerUpdateSig;

	private Hook<tJwKL5ldObyr11Vmn58>? ActionManagerUpdateHook;

	private static readonly CompSig ProcessPacketActionEffectSig;

	private Hook<hf0tEIlIJMA8LvGOnnD> ProcessPacketActionEffectHook;

	private wyBPmblUT7C5mWd99NC config;

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected unsafe override void Init()
	{
		config = ModuleConfigExtension.Load<wyBPmblUT7C5mWd99NC>(this) ?? new wyBPmblUT7C5mWd99NC();
		if (ActionManagerUpdateHook == null)
		{
			ActionManagerUpdateHook = ActionManagerUpdateSig.GetHook<tJwKL5ldObyr11Vmn58>(JcUTIYuJAS);
		}
		ActionManagerUpdateHook.Enable();
		if (ProcessPacketActionEffectHook == null)
		{
			ProcessPacketActionEffectHook = ProcessPacketActionEffectSig.GetHook<hf0tEIlIJMA8LvGOnnD>(Gn9TUAg9VU);
		}
		ProcessPacketActionEffectHook.Enable();
		Runtime.Initialize(this);
	}

	protected override void ConfigUI()
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(5, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xB1EBED8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_79d931aaf1c142a2a09f53e716260ad1)));
		((ImU8String)(ref val)).AppendLiteral(" (s)");
		if (ImGui.InputFloat(val, ref config.AnimationLock, 0f, 0f, default(ImU8String), (ImGuiInputTextFlags)0))
		{
			config.AnimationLock = MathF.Max(0f, config.AnimationLock);
		}
		if (ImGui.IsItemDeactivatedAfterEdit())
		{
			config.Save(this);
		}
		Runtime.Draw(this);
	}

	private unsafe void JcUTIYuJAS(ActionManager* manager)
	{
		ActionManagerUpdateHook.Original(manager);
	}

	private unsafe void Gn9TUAg9VU(uint sourceID, nint sourceCharacter, nint pos, Header* effectHeader, Effect* effectArray, ulong* effectTrail)
	{
		((Header)effectHeader).Flags = (byte)(((Header)effectHeader).Flags & -3);
		ProcessPacketActionEffectHook.Original(sourceID, sourceCharacter, pos, effectHeader, effectArray, effectTrail);
		Runtime.AfterPacket(this, sourceID, (nint)effectHeader);
	}

	private float L8DTuZ6pk6(float current)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		ClassJob classJobData = LocalPlayerState.ClassJobData;
		if (((ClassJob)(ref classJobData)).DohDolJobIndex != -1)
		{
			return MathF.Min(current, 400f);
		}
		return MathF.Min(current, config.AnimationLock);
	}

	public AutoRemoveAnimationLockPunishment()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5357DA06 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_0676edd1b45a4ff096830fd6229bc1fb)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5A7FD7D5 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5154ccd9fed0487f8fa0244531466009)),
			Category = ModuleCategory.Action
		};
		Permission = new ModulePermission
		{
			NeedAuth = true
		};
		base._002Ector();
	}

	static AutoRemoveAnimationLockPunishment()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		ActionManagerUpdateSig = new CompSig(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7C2F7AEC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_12c2d68ba4654622a4959ee73cacc6d7));
		ProcessPacketActionEffectSig = new CompSig(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6C35325A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88a2dcd1996b4a3fbefaf51a98b59def));
	}
}
