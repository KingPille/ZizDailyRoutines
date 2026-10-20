using System;
using System.Collections.Frozen;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Gui.Dtr;
using Dalamud.Game.Gui.Toast;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Enums;
using Lumina.Text;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic.Duty;

public class AutoDisplayZoneServerID : ModuleBase
{
	private class Config : ModuleConfig
	{
		public bool IsEnabledChat = true;
	}

	private Config config;

	private IDtrBarEntry? entry;

	private unsafe static readonly FrozenSet<TerritoryIntendedUse> ContentMemberListValidZones = FrozenSet.Create(new ReadOnlySpan<TerritoryIntendedUse>(Unsafe.AsPointer(ref global::_003CPrivateImplementationDetails_003E._8F6275B642574298B74367E275EA70F430F855F9CE7FB17B406C8A67F876F829), 3));

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoDisplayZoneServerIDTitle"),
		Description = LanguageManager.Get("AutoDisplayZoneServerIDDescription"),
		Category = ModuleCategory.Duty
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected override void Init()
	{
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		entry = DalamudServiceExtension.Instance<IDtrBar>().Get("DailyRoutines-AutoDisplayZoneServerID", (SeString)null);
		entry.Tooltip = SeString.op_Implicit(LanguageManager.Get("AutoDisplayZoneServerID-DTR-Tooltip"));
		entry.OnClick = delegate(DtrInteractionEvent data)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Expected O, but got Unknown
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Invalid comparison between Unknown and I4
			MouseClickType clickType = data.ClickType;
			if ((int)clickType != 0)
			{
				if ((int)clickType == 1)
				{
					_ = $"hxxps://ce-crowdsource.atmoomen.top/dc/{GameState.CurrentDataCenter}/instance/{GameState.ZoneServerID}";
				}
			}
			else
			{
				NotifyHelper.ToastQuest($"{LanguageManager.Get("CopiedToClipboard")}: {GameState.ZoneServerID}", new QuestToastOptions
				{
					DisplayCheckmark = true
				});
			}
		};
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += OnZoneChanged;
		OnZoneChanged(0u);
	}

	protected override void Uninit()
	{
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChanged;
		IDtrBarEntry? obj = entry;
		if (obj != null)
		{
			obj.Remove();
		}
		entry = null;
	}

	protected override void ConfigUI()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("SendChat")), ref config.IsEnabledChat))
		{
			config.Save(this);
		}
	}

	private unsafe void OnZoneChanged(uint zone)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		bool flag = ContentMemberListValidZones.Contains(GameState.TerritoryIntendedUse);
		entry.Shown = flag;
		if (!flag)
		{
			return;
		}
		uint zoneServerID = GameState.ZoneServerID;
		entry.Text = SeString.op_Implicit($"{LanguageManager.Get("AutoDisplayZoneServerID-ServerID")}: {zoneServerID}");
		if (!config.IsEnabledChat)
		{
			return;
		}
		RentedSeStringBuilder val = default(RentedSeStringBuilder);
		((RentedSeStringBuilder)(ref val))._002Ector();
		try
		{
			SeStringBuilder builder = ((RentedSeStringBuilder)(ref val)).Builder;
			SeStringInterpolatedStringHandler val2 = new SeStringInterpolatedStringHandler(2, 1, builder);
			((SeStringInterpolatedStringHandler)(ref val2)).AppendFormatted<string>(LanguageManager.Get("AutoDisplayZoneServerID-ServerID"));
			((SeStringInterpolatedStringHandler)(ref val2)).AppendLiteral(": ");
			ReadOnlySeString message = builder.Append(val2).PushColorType(45u).Append(zoneServerID.ToString())
				.PopColorType()
				.ToReadOnlySeString();
			OmenServiceBase<NotifyHelper>.Instance().Chat(message);
		}
		finally
		{
			((IDisposable)(*(RentedSeStringBuilder*)(&val))/*cast due to constrained. prefix*/).Dispose();
		}
	}
}
