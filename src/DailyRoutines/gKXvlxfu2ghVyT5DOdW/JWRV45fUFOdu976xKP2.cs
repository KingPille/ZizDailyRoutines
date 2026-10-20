using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.Common.DataUploader.Abstractions;
using DailyRoutines.Manager;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.Sheets;
using Newtonsoft.Json;
using OmenTools.Extensions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using itAnDoW4mgYVlrmtMor;
using ycTZ7OSV787Z3a0g7JT;

namespace gKXvlxfu2ghVyT5DOdW;

internal class JWRV45fUFOdu976xKP2 : DataUploaderBase
{
	private class ChaoticUptimeData
	{
		[JsonProperty("data_center")]
		public string DataCenter { get; set; }

		[JsonProperty("is_uptime")]
		public bool IsUptime { get; set; }

		[JsonProperty("last_bonus_starts")]
		public List<DateTime> LastBonusStartTimes { get; set; }

		[JsonProperty("last_bonus_ends")]
		public List<DateTime> LastBonusEndTimes { get; set; }

		public ChaoticUptimeData()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass2_0
	{
		public string dataCenterName;

		public bool isUptime;

		public _003C_003Ef__AnonymousType2<string, string, bool> data;

		public _003C_003Ec__DisplayClass2_0()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
		}

		internal async Task ghGtSOZ5it(CancellationToken cancellationToken)
		{
			bool? flag = await J63fYlcml0(dataCenterName, cancellationToken);
			if (flag.HasValue && flag != isUptime)
			{
				await PqtKlnSSKxdRRHSb7mo.xXNSfmClyl(InternalAPIRequestType.ReportChaoticBonus, data, cancellationToken);
			}
		}
	}

	protected override Task Init()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		OmenServiceBase<FrameworkManager>.Instance().Reg(new OnUpdateDelegate(FIvfX1jJEs), 60000u);
		return Task.CompletedTask;
	}

	protected override Task Stop()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		OmenServiceBase<FrameworkManager>.Instance().Unreg(FIvfX1jJEs);
		return Task.CompletedTask;
	}

	private unsafe void FIvfX1jJEs(IFramework framework)
	{
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		_003C_003Ec__DisplayClass2_0 CS_0024_003C_003E8__locals11 = new _003C_003Ec__DisplayClass2_0();
		uint num;
		switch (OmenServiceBase<StandardTimeManager>.Instance().Now.Minute)
		{
		case 16:
		case 17:
		case 18:
		case 19:
		case 20:
		case 21:
		case 22:
		case 23:
		case 24:
		case 25:
		case 26:
		case 27:
		case 28:
		case 29:
		case 30:
		case 31:
		case 32:
		case 33:
		case 34:
		case 35:
		case 36:
		case 37:
		case 38:
		case 39:
		case 40:
		case 41:
		case 42:
		case 43:
		case 44:
			return;
		default:
			num = 0u;
			break;
		case 6:
		case 7:
		case 8:
		case 9:
		case 10:
		case 11:
		case 12:
		case 13:
		case 14:
		case 15:
		case 45:
		case 46:
		case 47:
		case 48:
		case 49:
		case 50:
		case 51:
		case 52:
		case 53:
		case 54:
			num = 180000u;
			break;
		}
		if (num != 0 && !DataUploaderBase.Throttler.Throttle(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x7C6C95B4 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d2f07ce64d464f30b1d7397ae7500cac), num))
		{
			return;
		}
		CS_0024_003C_003E8__locals11.dataCenterName = string.Empty;
		CS_0024_003C_003E8__locals11.isUptime = false;
		BattleChara* localPlayer = Control.GetLocalPlayer();
		uint contentFinderCondition = GameState.ContentFinderCondition;
		if (localPlayer == null || IConditionExtension.get_IsBetweenAreas(DalamudServiceExtension.Instance<ICondition>()) || !UIModuleExtension.IsScreenReady() || contentFinderCondition != 0)
		{
			return;
		}
		WorldDCGroupType currentDataCenterData = GameState.CurrentDataCenterData;
		CS_0024_003C_003E8__locals11.dataCenterName = ((object)((WorldDCGroupType)(ref currentDataCenterData)).Name/*cast due to constrained. prefix*/).ToString();
		if (string.IsNullOrWhiteSpace(CS_0024_003C_003E8__locals11.dataCenterName))
		{
			return;
		}
		InstanceContent instanceContent = ((UIState)UIState.Instance()).InstanceContent;
		CS_0024_003C_003E8__locals11.isUptime = instanceContent.IsLimitedTimeBonusActive;
		CS_0024_003C_003E8__locals11.data = new
		{
			cid = LocalPlayerState.ContentID.ToString(),
			data_center = CS_0024_003C_003E8__locals11.dataCenterName,
			is_uptime = CS_0024_003C_003E8__locals11.isUptime
		};
		RunBackground(async delegate(CancellationToken cancellationToken)
		{
			bool? flag = await J63fYlcml0(CS_0024_003C_003E8__locals11.dataCenterName, cancellationToken);
			if (flag.HasValue && flag != CS_0024_003C_003E8__locals11.isUptime)
			{
				await PqtKlnSSKxdRRHSb7mo.xXNSfmClyl(InternalAPIRequestType.ReportChaoticBonus, CS_0024_003C_003E8__locals11.data, cancellationToken);
			}
		});
	}

	private static async Task<bool?> J63fYlcml0(string dcName, CancellationToken cancellationToken)
	{
		try
		{
			ChaoticUptimeData chaoticUptimeData = JsonConvert.DeserializeObject<ChaoticUptimeData>(await PqtKlnSSKxdRRHSb7mo.xXNSfmClyl(InternalAPIRequestType.RequestChaoticBonus, new
			{
				data_center = dcName
			}, cancellationToken));
			return chaoticUptimeData.IsUptime;
		}
		catch (Exception)
		{
		}
		return null;
	}

	public JWRV45fUFOdu976xKP2()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
	}
}
