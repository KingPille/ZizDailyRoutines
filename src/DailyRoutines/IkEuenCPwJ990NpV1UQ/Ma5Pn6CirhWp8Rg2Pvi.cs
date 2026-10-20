using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.Common.DataUploader.Abstractions;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Manager;
using Dalamud.Plugin.Services;
using OmenTools.Extensions;
using ycTZ7OSV787Z3a0g7JT;

namespace IkEuenCPwJ990NpV1UQ;

internal class Ma5Pn6CirhWp8Rg2Pvi : DataUploaderBase
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass2_0
	{
		public _003C_003Ef__AnonymousType5<List<string>> data;

		public _003C_003Ec__DisplayClass2_0()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
		}

		internal Task JYbtpi3rIg(CancellationToken cancellationToken)
		{
			return PqtKlnSSKxdRRHSb7mo.xXNSfmClyl(InternalAPIRequestType.ReportModulesUsageByCID, data, cancellationToken);
		}
	}

	protected override Task Init()
	{
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += WinCh9OIIk;
		if (DalamudServiceExtension.Instance<IClientState>().IsLoggedIn)
		{
			WinCh9OIIk(0u);
		}
		return Task.CompletedTask;
	}

	protected override Task Stop()
	{
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= WinCh9OIIk;
		return Task.CompletedTask;
	}

	private void WinCh9OIIk(uint zone)
	{
		_003C_003Ec__DisplayClass2_0 CS_0024_003C_003E8__locals2 = new _003C_003Ec__DisplayClass2_0();
		if (!ManagerBase<ModuleManager>.Instance().IsBusy && ManagerBase<ModuleManager>.Instance().ModulesValidCount != 0 && ManagerBase<ModuleManager>.Instance().ModulesEnableCount != 0)
		{
			List<string> list = (from x in ManagerBase<ModuleManager>.Instance().Modules
				where x.IsEnabled
				select x.ModuleName).ToList();
			CS_0024_003C_003E8__locals2.data = new { list };
			RunBackground((CancellationToken cancellationToken) => PqtKlnSSKxdRRHSb7mo.xXNSfmClyl(InternalAPIRequestType.ReportModulesUsageByCID, CS_0024_003C_003E8__locals2.data, cancellationToken));
		}
	}

	public Ma5Pn6CirhWp8Rg2Pvi()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
	}
}
