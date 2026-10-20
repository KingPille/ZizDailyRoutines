using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Plugin.Services;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic.Interface;

public class AutoConfirmPortraitUpdate : ModuleBase
{
	private class Config : ModuleConfig
	{
		public bool SendChat = true;

		public bool SendNotification = true;
	}

	private Config config;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoConfirmPortraitUpdateTitle"),
		Description = LanguageManager.Get("AutoConfirmPortraitUpdateDescription"),
		Category = ModuleCategory.Interface
	};

	protected unsafe override void Init()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)1, "BannerPreview", new AddonEventDelegate(OnAddon));
		if (Addons.BannerPreview != null)
		{
			OnAddon((AddonEvent)1, null);
		}
	}

	protected override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddon });
	}

	protected override void ConfigUI()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("SendNotification")), ref config.SendNotification))
		{
			config.Save(this);
		}
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("SendChat")), ref config.SendChat))
		{
			config.Save(this);
		}
	}

	private unsafe void OnAddon(AddonEvent type, AddonArgs? args)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		(*Addons.BannerPreview).Callback(0);
		if (config.SendNotification)
		{
			OmenServiceBase<NotifyHelper>.Instance().NotificationSuccess(LanguageManager.Get("AutoConfirmPortraitUpdate-Notification"));
		}
		if (config.SendChat)
		{
			OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoConfirmPortraitUpdate-Notification")));
		}
	}
}
