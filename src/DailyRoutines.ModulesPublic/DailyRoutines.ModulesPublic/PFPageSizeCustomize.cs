using System;
using System.Runtime.InteropServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Hooking;
using OmenTools.Global;
using OmenTools.Interop.Game.Models;

namespace DailyRoutines.ModulesPublic;

public class PFPageSizeCustomize : ModuleBase
{
	private delegate byte PartyFinderDisplayAmountDelegate(nint a1, int a2);

	private class Config : ModuleConfig
	{
		public short PageSize = 100;
	}

	private static readonly CompSig PartyFinderDisplayAmountSig = new CompSig("48 89 5C 24 ?? 55 56 57 48 ?? ?? ?? ?? ?? ?? ?? 48 ?? ?? ?? ?? ?? ?? 48 ?? ?? ?? ?? ?? ?? 48 ?? ?? 48 89 85 ?? ?? ?? ?? 48 ?? ?? 0F");

	private Hook<PartyFinderDisplayAmountDelegate>? PartyFinderDisplayAmountHook;

	private Config config;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("PFPageSizeCustomizeTitle"),
		Description = LanguageManager.Get("PFPageSizeCustomizeDescription"),
		Category = ModuleCategory.Recruitment,
		Author = new string[1] { "逆光" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected override void Init()
	{
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		if (PartyFinderDisplayAmountHook == null)
		{
			PartyFinderDisplayAmountHook = PartyFinderDisplayAmountSig.GetHook<PartyFinderDisplayAmountDelegate>(PartyFinderDisplayAmountDetour);
		}
		PartyFinderDisplayAmountHook.Enable();
	}

	protected override void ConfigUI()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		ImGui.SetNextItemWidth(100f * Globals.GlobalUIScale);
		if (ImGui.InputShort(ImU8String.op_Implicit(LanguageManager.Get("PFPageSizeCustomize-DisplayAmount")), ref config.PageSize, (short)1, (short)10, default(ImU8String), (ImGuiInputTextFlags)0))
		{
			config.PageSize = Math.Clamp(config.PageSize, (short)1, (short)100);
		}
		if (ImGui.IsItemDeactivatedAfterEdit())
		{
			config.Save(this);
		}
	}

	private byte PartyFinderDisplayAmountDetour(nint a1, int a2)
	{
		Marshal.WriteInt16(a1 + 1152, config.PageSize);
		return PartyFinderDisplayAmountHook.Original(a1, a2);
	}
}
