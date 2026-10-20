using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Component.GUI;
using OmenTools.Info.Game.Data;
using OmenTools.Info.Game.Packets.Upstream;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class CompanyCreditExchangeMore : ModuleBase
{
	[return: MarshalAs(UnmanagedType.U1)]
	private unsafe delegate bool AddonFreeCompanyCreditShopRefreshDelegate(AtkUnitBase* addon, uint atkValueCount, AtkValue* atkValues);

	private class Config : ModuleConfig
	{
		public bool OnlyActiveInWorkshop = true;
	}

	private static readonly CompSig AddonFreeCompanyCreditShopRefreshSig = new CompSig("41 56 41 57 48 83 EC ?? 0F B6 81 ?? ?? ?? ?? 4D 8B F8");

	private Hook<AddonFreeCompanyCreditShopRefreshDelegate> AddonFreeCompanyCreditShopRefreshHook;

	private Config config;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("CompanyCreditExchangeMoreTitle"),
		Description = LanguageManager.Get("CompanyCreditExchangeMoreDescription"),
		Category = ModuleCategory.System
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected unsafe override void Init()
	{
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		AddonFreeCompanyCreditShopRefreshHook = AddonFreeCompanyCreditShopRefreshSig.GetHook<AddonFreeCompanyCreditShopRefreshDelegate>(AddonRefreshDetour);
		AddonFreeCompanyCreditShopRefreshHook.Enable();
		OmenServiceBase<GamePacketManager>.Instance().RegPreSendPacket(OnPreSendPacket);
	}

	protected override void Uninit()
	{
		OmenServiceBase<GamePacketManager>.Instance().Unreg(OnPreSendPacket);
	}

	protected override void ConfigUI()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("CompanyCreditExchangeMore-OnlyActiveInWorkshop")), ref config.OnlyActiveInWorkshop))
		{
			config.Save(this);
		}
	}

	private unsafe bool AddonRefreshDetour(AtkUnitBase* addon, uint atkValueCount, AtkValue* atkValues)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Invalid comparison between Unknown and I4
		if (addon == null)
		{
			return false;
		}
		bool result = AddonFreeCompanyCreditShopRefreshHook.Original(addon, atkValueCount, atkValues);
		if (!config.OnlyActiveInWorkshop || ((HousingManager)HousingManager.Instance()).WorkshopTerritory != null)
		{
			for (int i = 110; i < 130; i++)
			{
				if ((int)((AtkValue)((byte*)((AtkUnitBase)addon).AtkValues + (nint)i * (nint)Unsafe.SizeOf<AtkValue>())).Type == 3)
				{
					((AtkValue)((byte*)((AtkUnitBase)addon).AtkValues + (nint)i * (nint)Unsafe.SizeOf<AtkValue>())).Int = 255;
				}
			}
		}
		return result;
	}

	private unsafe void OnPreSendPacket(ref bool isPrevented, int opcode, ref nint packet, ref bool isPrioritize)
	{
		if (opcode == UpstreamOpcode.HandOverItemOpcode && (!config.OnlyActiveInWorkshop || ((HousingManager)HousingManager.Instance()).WorkshopTerritory != null) && Addons.FreeCompanyCreditShop != null)
		{
			HandOverItemPacket* ptr = (HandOverItemPacket*)packet;
			if (ptr->Param0 >= 99)
			{
				ptr->Param0 = 255u;
			}
		}
	}
}
