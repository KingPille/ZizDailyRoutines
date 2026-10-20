using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Hooking;
using Lumina.Excel.Sheets;
using OmenTools.Global;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules;

public class ExpandActionRange : ModuleBase
{
	private delegate float HevZGulXWTXJoQGtiUG(uint actionID);

	private class NUy95qlYyPHFXTBuqZX : ModuleConfig
	{
		public float ExpandedRange;

		public NUy95qlYyPHFXTBuqZX()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			ExpandedRange = 2f;
			base._002Ector();
		}
	}

	private static readonly CompSig GetActionRangeSig;

	private Hook<HevZGulXWTXJoQGtiUG>? GetActionRangeHook;

	private NUy95qlYyPHFXTBuqZX config;

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected override void Init()
	{
		config = ModuleConfigExtension.Load<NUy95qlYyPHFXTBuqZX>(this) ?? new NUy95qlYyPHFXTBuqZX();
		if (GetActionRangeHook == null)
		{
			GetActionRangeHook = GetActionRangeSig.GetHook<HevZGulXWTXJoQGtiUG>(Xh9TYAwxI4);
		}
		GetActionRangeHook.Enable();
	}

	protected override void ConfigUI()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		ImGui.SetNextItemWidth(100f * Globals.GlobalUIScale);
		ImGui.InputFloat(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2DBD379A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5d8d924f1edf4ec39f023b3c7bb646cc))), ref config.ExpandedRange, 0.1f, 1f, ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--1470179789 ^ 0xB4212C0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3054c6e6ef664be696bb2ad005864818)), (ImGuiInputTextFlags)0);
		if (ImGui.IsItemDeactivatedAfterEdit())
		{
			config.Save(this);
		}
	}

	private float Xh9TYAwxI4(uint actionID)
	{
		float num = GetActionRangeHook.Original(actionID);
		if (actionID == 0 || num == 0f || !LuminaGetter.TryGetRow<Action>(actionID, out Action item) || ((Action)(ref item)).TargetArea)
		{
			return num;
		}
		return num + config.ExpandedRange;
	}

	public ExpandActionRange()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x9B4AF72 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2f1c0cb46b4440d499a69ed8afe22360)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x51E587C6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a90a92322bff42e180982e106200d4a3)),
			Category = ModuleCategory.Action,
			ModulesConflict = new string[1] { LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5CE33B95 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3054c6e6ef664be696bb2ad005864818) }
		};
		Permission = new ModulePermission
		{
			NeedAuth = true,
			AllDefaultEnabled = true
		};
		base._002Ector();
	}

	static ExpandActionRange()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		GetActionRangeSig = new CompSig(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x420CDB00 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_84ee025b19bb4145ba9b4b6b7acb91eb));
	}
}
