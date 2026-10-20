using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.Helpers;
using OmenTools.Threading;

namespace DailyRoutines.ModulesPublic.Duty;

public class AutoShowFrontlineKillCount : ModuleBase
{
	private uint lastKillCount;

	private uint preview = 1u;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoShowFrontlineKillCountTitle"),
		Description = LanguageManager.Get("AutoShowFrontlineKillCountDescription"),
		Category = ModuleCategory.Duty
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected unsafe override void Init()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)5, "PvPFrontlineGauge", new AddonEventDelegate(OnAddon));
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += OnZoneChanged;
		if ((*Addons.PvPFrontlineGauge).IsAddonAndNodesReady())
		{
			try
			{
				lastKillCount = ((AtkValue)((byte*)((AtkUnitBase)Addons.PvPFrontlineGauge).AtkValues + (nint)6 * (nint)Unsafe.SizeOf<AtkValue>())).UInt;
			}
			catch
			{
			}
		}
	}

	protected override void Uninit()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChanged;
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddon });
		lastKillCount = 0u;
	}

	protected override void ConfigUI()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("Preview")));
		IndentDisposable val = ImRaii.PushIndent(1, true);
		try
		{
			if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("Confirm")), default(Vector2)))
			{
				DisplayKillCount(preview);
			}
			ImGui.SameLine();
			ImGui.SetNextItemWidth(100f * Globals.GlobalUIScale);
			if (ImGui.InputUInt(ImU8String.op_Implicit("###PreviewInput"), ref preview, 1u, 1u, default(ImU8String), (ImGuiInputTextFlags)0))
			{
				preview = Math.Clamp(preview, 1u, 99u);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private unsafe void OnAddon(AddonEvent type, AddonArgs args)
	{
		if (Addons.PvPFrontlineGauge != null && Throttler.Shared.Throttle("AutoShowFrontlineKillCount-OnUpdate", 100u))
		{
			uint num = 0u;
			try
			{
				num = ((AtkValue)((byte*)((AtkUnitBase)Addons.PvPFrontlineGauge).AtkValues + (nint)6 * (nint)Unsafe.SizeOf<AtkValue>())).UInt;
			}
			catch
			{
				num = lastKillCount;
			}
			if (lastKillCount != num)
			{
				DisplayKillCount(num);
				lastKillCount = num;
			}
		}
	}

	private void OnZoneChanged(uint u)
	{
		lastKillCount = 0u;
	}

	private unsafe static void DisplayKillCount(uint killCount)
	{
		if (AddonHelper.TryGetByName("_Streak", out var addonPtr))
		{
			((AtkUnitBase)addonPtr).IsVisible = false;
			((AtkUnitBase)addonPtr).Close(true);
		}
		((UIModule)UIModule.Instance()).ShowStreak((int)killCount, (killCount <= 2) ? 1 : 2);
	}
}
