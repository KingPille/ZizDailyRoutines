using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using OmenTools.Extensions;
using OmenTools.Interop.Game;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.ModulesPublic;

public class AutoRefreshMarketSearchResult : ModuleBase
{
	private static readonly CompSig WaitMessageSig = new CompSig("BA ?? ?? ?? ?? E8 ?? ?? ?? ?? 4C 8B C0 BA ?? ?? ?? ?? 48 8B CE E8 ?? ?? ?? ?? 45 33 C9");

	private MemoryPatch waitMessagePatch;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoRefreshMarketSearchResultTitle"),
		Description = LanguageManager.Get("AutoRefreshMarketSearchResultDescription"),
		Category = ModuleCategory.System
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected override void Init()
	{
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 2000
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		waitMessagePatch = new MemoryPatch(WaitMessageSig.Get(), new global::_003C_003Ez__ReadOnlyArray<byte?>(new byte?[5]
		{
			(byte)186,
			(byte)185,
			(byte)26,
			0,
			0
		}));
		waitMessagePatch.Enable();
		OmenServiceBase<GameState>.Instance().MarketListingsStuck += OnMarketListingsStuck;
		OmenServiceBase<GameState>.Instance().Logout += OnLogout;
	}

	protected override void Uninit()
	{
		OmenServiceBase<GameState>.Instance().Logout -= OnLogout;
		OmenServiceBase<GameState>.Instance().MarketListingsStuck -= OnMarketListingsStuck;
	}

	private unsafe void OnLogout()
	{
		base.TaskHelper.Abort();
		InfoProxyItemSearch* ptr = InfoProxyItemSearch.Instance();
		if (ptr != null)
		{
			((InfoProxyItemSearch)ptr).ClearListData();
		}
	}

	private unsafe void OnMarketListingsStuck(int errorCode)
	{
		if (!DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)69])
		{
			base.TaskHelper.Abort();
			base.TaskHelper.DelayNext(500, 1000);
			base.TaskHelper.Enqueue(() => ((InfoProxyItemSearch)InfoProxyItemSearch.Instance()).RequestData());
		}
	}
}
