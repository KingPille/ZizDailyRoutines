using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Hooking;
using Dalamud.Interface;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic.Interface;

public class AutoHideBanners : ModuleBase
{
	private unsafe delegate void SetImageDelegate(AddonImage* addon, int bannerID, IconSubFolder folder, int soundEffectID);

	private class Config : ModuleConfig
	{
		public Dictionary<uint, bool> HiddenBanners = new Dictionary<uint, bool>();
	}

	private static readonly CompSig SetImageSig = new CompSig("48 89 5C 24 ?? 57 48 83 EC 30 48 8B D9 89 91");

	private Hook<SetImageDelegate>? SetImageHook;

	private Config config;

	private static readonly List<uint> BannersData;

	private static readonly FrozenSet<uint> WKSMissionChainBannerIDs;

	private static readonly FrozenSet<uint> DefaultEnabledBanners;

	private static readonly Vector4 ButtonNormalColor;

	private static readonly Vector4 ButtonActiveColor;

	private static readonly Vector4 ButtonHoveredColor;

	private static readonly Vector4 ButtonSelectedColor;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoHideBannersTitle"),
		Description = LanguageManager.Get("AutoHideBannersDescription"),
		Category = ModuleCategory.Interface,
		Author = new string[1] { "XSZYYS" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected unsafe override void Init()
	{
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		bool flag = false;
		foreach (uint bannersDatum in BannersData)
		{
			if (config.HiddenBanners.TryAdd(bannersDatum, DefaultEnabledBanners.Contains(bannersDatum)))
			{
				flag = true;
			}
		}
		if (flag)
		{
			config.Save(this);
		}
		SetImageHook = SetImageSig.GetHook<SetImageDelegate>(SetImageDetour);
		SetImageHook.Enable();
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)4, "_WKSMissionChain", new AddonEventDelegate(OnAddon));
	}

	protected override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddon });
	}

	protected override void ConfigUI()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		float x = ImGui.GetContentRegionAvail().X;
		ImGuiStylePtr style = ImGui.GetStyle();
		Vector2 vector = new Vector2(x - 2f * ((ImGuiStylePtr)(ref style)).ItemSpacing.X, 400f * Globals.GlobalUIScale);
		TableDisposable val = ImRaii.Table(ImU8String.op_Implicit("BannerList"), 2, (ImGuiTableFlags)33556416, vector);
		try
		{
			if (!val)
			{
				return;
			}
			ImGui.TableSetupColumn(ImU8String.op_Implicit("LeftColumn"), (ImGuiTableColumnFlags)4, 50f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("RightColumn"), (ImGuiTableColumnFlags)4, 50f, 0u);
			int num = (BannersData.Count + 1) / 2;
			for (int i = 0; i < num; i++)
			{
				ImGui.TableNextRow();
				ImGui.TableNextColumn();
				if (i < BannersData.Count)
				{
					uint bannerID = BannersData[i];
					RenderBannerButton(bannerID, vector);
				}
				ImGui.TableNextColumn();
				int num2 = i + num;
				if (num2 < BannersData.Count)
				{
					uint bannerID2 = BannersData[num2];
					RenderBannerButton(bannerID2, vector);
				}
			}
		}
		finally
		{
			((TableDisposable)(ref val)).Dispose();
		}
	}

	private void RenderBannerButton(uint bannerID, Vector2 tableSize)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		if (!OmenServiceBase<ImageHelper>.Instance().TryGetGameLangIcon(bannerID, out IDalamudTextureWrap texture))
		{
			return;
		}
		Vector2 vector = new Vector2(tableSize.X / 2f, 128f * Globals.GlobalUIScale);
		Vector2 cursorPos = ImGui.GetCursorPos();
		ImGui.Image(texture.Handle, vector);
		ImGui.SetCursorPos(cursorPos);
		ColorDisposable val = ImRaii.PushColor((ImGuiCol)21, ButtonNormalColor, true);
		try
		{
			ColorDisposable val2 = ImRaii.PushColor((ImGuiCol)23, ButtonActiveColor, true);
			try
			{
				ColorDisposable val3 = ImRaii.PushColor((ImGuiCol)22, ButtonHoveredColor, true);
				try
				{
					ColorDisposable val4 = ImRaii.PushColor((ImGuiCol)21, ButtonSelectedColor, config.HiddenBanners.GetValueOrDefault(bannerID));
					try
					{
						ImU8String val5 = new ImU8String(3, 2);
						((ImU8String)(ref val5)).AppendLiteral("##");
						((ImU8String)(ref val5)).AppendFormatted<uint>(bannerID);
						((ImU8String)(ref val5)).AppendLiteral("_");
						((ImU8String)(ref val5)).AppendFormatted<Vector2>(cursorPos);
						if (ImGui.Button(val5, vector))
						{
							Dictionary<uint, bool> hiddenBanners = config.HiddenBanners;
							hiddenBanners[bannerID] = !hiddenBanners[bannerID];
							config.Save(this);
						}
					}
					finally
					{
						((IDisposable)val4)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)val3)?.Dispose();
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private unsafe void OnAddon(AddonEvent type, AddonArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		AtkUnitBase* ptr = args.Addon.ToStruct();
		if (ptr != null)
		{
			((AtkResNode)((AtkUnitBase)ptr).RootNode).ToggleVisibility(!ShouldHideWKSMissionChain(ptr));
			((AtkUnitBase)ptr).Hide(true, true, 1u);
		}
	}

	private unsafe void SetImageDetour(AddonImage* addonImage, int bannerID, IconSubFolder folder, int soundEffectID)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (!IsWKSMissionChainBannerSelected((uint)bannerID) && !config.HiddenBanners.GetValueOrDefault((uint)bannerID))
		{
			SetImageHook.Original(addonImage, bannerID, folder, soundEffectID);
		}
	}

	private unsafe bool ShouldHideWKSMissionChain(AtkUnitBase* addon)
	{
		AtkImageNode* ptr = ((AtkUnitBase)addon).UldManager.SearchSimpleNodeByType<AtkImageNode>((NodeType)2);
		if (ptr == null)
		{
			return false;
		}
		uint iconID = (*ptr).GetIconID();
		if (iconID == 0)
		{
			return false;
		}
		return IsWKSMissionChainBannerSelected(iconID);
	}

	private bool IsWKSMissionChainBannerSelected(uint iconID)
	{
		if (WKSMissionChainBannerIDs.Contains(iconID))
		{
			return config.HiddenBanners.GetValueOrDefault(iconID);
		}
		return false;
	}

	static AutoHideBanners()
	{
		int num = 32;
		List<uint> list = new List<uint>(num);
		CollectionsMarshal.SetCount(list, num);
		Span<uint> span = CollectionsMarshal.AsSpan(list);
		span[0] = 120031u;
		span[1] = 120032u;
		span[2] = 120055u;
		span[3] = 120081u;
		span[4] = 120082u;
		span[5] = 120083u;
		span[6] = 120084u;
		span[7] = 120085u;
		span[8] = 120086u;
		span[9] = 120093u;
		span[10] = 120094u;
		span[11] = 120095u;
		span[12] = 120096u;
		span[13] = 120141u;
		span[14] = 120142u;
		span[15] = 121081u;
		span[16] = 121082u;
		span[17] = 121561u;
		span[18] = 121562u;
		span[19] = 121563u;
		span[20] = 128370u;
		span[21] = 128371u;
		span[22] = 128372u;
		span[23] = 128373u;
		span[24] = 128525u;
		span[25] = 128526u;
		span[26] = 128527u;
		span[27] = 128528u;
		span[28] = 128529u;
		span[29] = 128530u;
		span[30] = 128531u;
		span[31] = 128532u;
		BannersData = list;
		WKSMissionChainBannerIDs = FrozenSet.Create<uint>(128527u, 128528u, 128529u, 128530u, 128531u, 128532u);
		DefaultEnabledBanners = FrozenSet.Create<uint>(120031u, 120032u, 120055u, 120095u, 120096u, 120141u, 120142u);
		ButtonNormalColor = ColorHelpers.WithAlpha(ImGui.GetColorU32((ImGuiCol)21).ToVector4(), 0f);
		ButtonActiveColor = ColorHelpers.WithAlpha(ImGui.GetColorU32((ImGuiCol)23).ToVector4(), 0.8f);
		ButtonHoveredColor = ColorHelpers.WithAlpha(ImGui.GetColorU32((ImGuiCol)22).ToVector4(), 0.4f);
		ButtonSelectedColor = ColorHelpers.WithAlpha(ImGui.GetColorU32((ImGuiCol)21).ToVector4(), 0.6f);
	}
}
