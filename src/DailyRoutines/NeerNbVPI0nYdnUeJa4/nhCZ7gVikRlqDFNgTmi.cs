using DailyRoutines.Common.Info;
using DailyRoutines.Internal;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using OmenTools.Extensions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using W5ek8T91VhslWK6xKNn;
using iGxwSFV6Y2uamybGI4O;
using mbL4CiVEWd3wtCCTP72;
using wUQLZCVUi4RNloOxE8c;

namespace NeerNbVPI0nYdnUeJa4;

internal static class nhCZ7gVikRlqDFNgTmi
{
	private static DrawScopesHandle WindowStylesHandle;

	public static void GcIVhc0oQg()
	{
		WindowManager windowManager = OmenServiceBase<WindowManager>.Instance();
		windowManager.AddWindow<HfWYPNVIio8M9ncZnm0>();
		windowManager.AddWindow<i3LqFS9TadSFeoumyfM>();
		windowManager.AddWindow<XOoANOVG4jDKQ7WvCb9>();
		windowManager.AddWindow<n2hMWbVwHSf2rAJHZyL>();
		WindowStylesHandle = windowManager.RegDrawScopes(() => OmenServiceBase<FontManager>.Instance().UIFont.Push(), delegate
		{
			PluginConfig pluginConfig = PluginConfig.Instance();
			if (pluginConfig.InterfaceStyleV2Index > Assets.Styles.Count - 1)
			{
				pluginConfig.InterfaceStyleV2Index = 0;
				pluginConfig.Save();
			}
			return Assets.Styles[pluginConfig.InterfaceStyleV2Index].Push();
		});
		IUiBuilder obj = DalamudLeftServiceExtension.Instance();
		obj.OpenMainUi += CI4VgyF76y;
		obj.OpenConfigUi += delegate
		{
			HfWYPNVIio8M9ncZnm0? hfWYPNVIio8M9ncZnm = OmenServiceBase<WindowManager>.Instance().Get<HfWYPNVIio8M9ncZnm0>();
			if (hfWYPNVIio8M9ncZnm != null)
			{
				hfWYPNVIio8M9ncZnm.SelectedTab = HfWYPNVIio8M9ncZnm0.TabPageID.设置;
				((Window)hfWYPNVIio8M9ncZnm).IsOpen = true;
			}
		};
	}

	public static void RWnVoJKMqc()
	{
		WindowManager windowManager = OmenServiceBase<WindowManager>.Instance();
		windowManager.RemoveWindow<HfWYPNVIio8M9ncZnm0>();
		windowManager.RemoveWindow<i3LqFS9TadSFeoumyfM>();
		windowManager.RemoveWindow<XOoANOVG4jDKQ7WvCb9>();
		windowManager.RemoveWindow<n2hMWbVwHSf2rAJHZyL>();
		windowManager.UnregDrawScopes(WindowStylesHandle);
		WindowStylesHandle = default(DrawScopesHandle);
		IUiBuilder obj = DalamudLeftServiceExtension.Instance();
		obj.OpenMainUi -= CI4VgyF76y;
		obj.OpenConfigUi -= delegate
		{
			HfWYPNVIio8M9ncZnm0? hfWYPNVIio8M9ncZnm = OmenServiceBase<WindowManager>.Instance().Get<HfWYPNVIio8M9ncZnm0>();
			if (hfWYPNVIio8M9ncZnm != null)
			{
				hfWYPNVIio8M9ncZnm.SelectedTab = HfWYPNVIio8M9ncZnm0.TabPageID.设置;
				((Window)hfWYPNVIio8M9ncZnm).IsOpen = true;
			}
		};
	}

	private static void CI4VgyF76y()
	{
		HfWYPNVIio8M9ncZnm0 hfWYPNVIio8M9ncZnm = OmenServiceBase<WindowManager>.Instance().Get<HfWYPNVIio8M9ncZnm0>();
		if (hfWYPNVIio8M9ncZnm != null)
		{
			((Window)hfWYPNVIio8M9ncZnm).IsOpen = !((Window)hfWYPNVIio8M9ncZnm).IsOpen;
		}
	}
}
