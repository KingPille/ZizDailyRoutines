using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.Common.DataUploader.Abstractions;
using DailyRoutines.Manager;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.OmenService;
using itAnDoW4mgYVlrmtMor;
using ycTZ7OSV787Z3a0g7JT;

namespace Y9rSlwCOF9pg1TybV4q;

internal class PowbIMCjfPtJeNojGZD : DataUploaderBase
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass3_0
	{
		public _003C_003Ef__AnonymousType6<string, string, bool, string, string, string, string, string, string> data;

		public _003C_003Ec__DisplayClass3_0()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
		}

		internal Task DQstfvfqdP(CancellationToken cancellationToken)
		{
			return PqtKlnSSKxdRRHSb7mo.xXNSfmClyl(InternalAPIRequestType.ReportCosmoSpecialWeather, data, cancellationToken);
		}
	}

	protected override Task Init()
	{
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += BElCKoIhFC;
		BElCKoIhFC(0u);
		return Task.CompletedTask;
	}

	protected override Task Stop()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= BElCKoIhFC;
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { hKuCtfNbWj });
		return Task.CompletedTask;
	}

	private void BElCKoIhFC(uint zone)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Invalid comparison between Unknown and I4
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { hKuCtfNbWj });
		if ((int)GameState.TerritoryIntendedUse == 60)
		{
			DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)10, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x34A20F46 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5e416caedcb6438f915024f390fd9079), new AddonEventDelegate(hKuCtfNbWj));
		}
	}

	private unsafe void hKuCtfNbWj(AddonEvent type, AddonArgs args)
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		_003C_003Ec__DisplayClass3_0 CS_0024_003C_003E8__locals2 = new _003C_003Ec__DisplayClass3_0();
		if (EvdCWpPCfa(out var progress))
		{
			CS_0024_003C_003E8__locals2.data = new
			{
				map_id = GameState.TerritoryType.ToString(),
				server_id = GameState.CurrentWorld.ToString(),
				is_emergency = (((AnnounceData)((AgentWKSAnnounce)AgentWKSAnnounce.Instance()).Data).EmergencyInfoRowId != 0),
				cid = LocalPlayerState.ContentID.ToString(),
				weather_id = GameState.Weather.ToString(),
				message = CStringExtensions.ExtractText(((Utf8String)(&((AnnounceData)((AgentWKSAnnounce)AgentWKSAnnounce.Instance()).Data).FormattedString)).StringPtr),
				timestamp = ((AnnounceData)((AgentWKSAnnounce)AgentWKSAnnounce.Instance()).Data).EndTime.ToString(),
				progress = progress.ToString(),
				instance = InstancesManager.CurrentInstance.ToString()
			};
			RunBackground((CancellationToken cancellationToken) => PqtKlnSSKxdRRHSb7mo.xXNSfmClyl(InternalAPIRequestType.ReportCosmoSpecialWeather, CS_0024_003C_003E8__locals2.data, cancellationToken));
		}
	}

	private unsafe static bool EvdCWpPCfa(out uint progress)
	{
		progress = 0u;
		if (!(*Addons.WKSAnnounce).IsAddonAndNodesReady() || ((AtkUnitBase)Addons.WKSAnnounce).AtkValuesCount < 5)
		{
			return false;
		}
		if (((AtkValue)((byte*)((AtkUnitBase)Addons.WKSAnnounce).AtkValues + Unsafe.SizeOf<AtkValue>())).UInt == 0)
		{
			return false;
		}
		if (GameState.ServerTimeUnix > ((AnnounceData)((AgentWKSAnnounce)AgentWKSAnnounce.Instance()).Data).EndTime)
		{
			return false;
		}
		progress = ((AtkValue)((byte*)((AtkUnitBase)Addons.WKSAnnounce).AtkValues + (nint)5 * (nint)Unsafe.SizeOf<AtkValue>())).UInt;
		return true;
	}

	public PowbIMCjfPtJeNojGZD()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
	}
}
