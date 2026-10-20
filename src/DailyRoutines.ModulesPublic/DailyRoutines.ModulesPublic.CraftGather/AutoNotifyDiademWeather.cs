using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using DailyRoutines.Common.Extensions;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.ImGuiOm;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic.CraftGather;

public class AutoNotifyDiademWeather : ModuleBase
{
	private class Config : ModuleConfig
	{
		public uint[] Weathers = Array.Empty<uint>();
	}

	private Config config;

	private uint lastWeather;

	private static readonly uint[] SpecialWeathers = new uint[4] { 133u, 134u, 135u, 136u };

	private const uint DIADEM_ZONE = 939u;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoNotifyDiademWeatherTitle"),
		Description = LanguageManager.Get("AutoNotifyDiademWeatherDescription"),
		Category = ModuleCategory.CraftGather
	};

	protected override void Init()
	{
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += OnZoneChanged;
		OnZoneChanged(0u);
	}

	protected override void Uninit()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChanged;
		OmenServiceBase<FrameworkManager>.Instance().Unreg(OnUpdate);
		lastWeather = 0u;
	}

	protected override void ConfigUI()
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		using (ImRaiiExtension.Heading1(LuminaWrapper.GetAddonText(8555u)))
		{
			string text = string.Join(',', config.Weathers.Select(delegate(uint x)
			{
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				Weather? row = LuminaGetter.GetRow<Weather>(x);
				object obj2;
				if (!row.HasValue)
				{
					obj2 = null;
				}
				else
				{
					Weather valueOrDefault = row.GetValueOrDefault();
					obj2 = ((object)((Weather)(ref valueOrDefault)).Name/*cast due to constrained. prefix*/).ToString();
				}
				if (obj2 == null)
				{
					obj2 = string.Empty;
				}
				return (string)obj2;
			}).Distinct());
			ComboDisposable val = ImRaii.Combo(ImU8String.op_Implicit("###WeathersCombo"), ImU8String.op_Implicit(text), (ImGuiComboFlags)8);
			try
			{
				if (!ComboDisposable.op_Implicit(val))
				{
					return;
				}
				uint[] specialWeathers = SpecialWeathers;
				ISharedImmediateTexture val3 = default(ISharedImmediateTexture);
				foreach (uint num2 in specialWeathers)
				{
					if (!LuminaGetter.TryGetRow<Weather>(num2, out Weather item))
					{
						continue;
					}
					ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
					GameIconLookup val2 = new GameIconLookup((uint)((Weather)(ref item)).Icon, false, true, (ClientLanguage?)null);
					if (obj.TryGetFromGameIcon(ref val2, ref val3) && ImGuiOm.SelectableImageWithText(val3.GetWrapOrEmpty().Handle, new Vector2(ImGui.GetTextLineHeightWithSpacing()), ((object)((Weather)(ref item)).Name/*cast due to constrained. prefix*/).ToString() ?? "", config.Weathers.Contains(num2), (ImGuiSelectableFlags)1))
					{
						if (config.Weathers.Contains(num2))
						{
							config.Weathers.Remove(num2);
						}
						else
						{
							config.Weathers.Add(num2);
						}
						config.Save(this);
					}
				}
			}
			finally
			{
				((ComboDisposable)(ref val)).Dispose();
			}
		}
	}

	private void OnZoneChanged(uint u)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		OmenServiceBase<FrameworkManager>.Instance().Unreg(OnUpdate);
		if (GameState.TerritoryType == 939)
		{
			OmenServiceBase<FrameworkManager>.Instance().Reg(new OnUpdateDelegate(OnUpdate), 10000u);
		}
	}

	private unsafe void OnUpdate(IFramework framework)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		if (GameState.TerritoryType != 939)
		{
			OmenServiceBase<FrameworkManager>.Instance().Unreg(OnUpdate);
			return;
		}
		byte currentWeather = ((WeatherManager)WeatherManager.Instance()).GetCurrentWeather();
		if (lastWeather != currentWeather && LuminaGetter.TryGetRow<Weather>((uint)currentWeather, out Weather item))
		{
			lastWeather = currentWeather;
			if (config.Weathers.Contains(currentWeather))
			{
				string text = LanguageManager.Get("AutoNotifyDiademWeather-Notification", new Dictionary<string, object> { ["weather"] = ((Weather)(ref item)).Name });
				NotifyHelper.Toast(text);
				OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(text), useDefaultPrefix: false);
				OmenServiceBase<NotifyHelper>.Instance().TrayInfo(text);
			}
		}
	}
}
