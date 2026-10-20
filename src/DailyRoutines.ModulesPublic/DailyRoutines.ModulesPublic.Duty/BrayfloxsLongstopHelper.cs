using System;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.InstanceContent;
using OmenTools.Extensions;
using OmenTools.Info.Game.Packets.Upstream;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic.Duty;

public class BrayfloxsLongstopHelper : ModuleBase
{
	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static Action<uint> _003C0_003E__OnZoneChanged;

		public static OnUpdateDelegate _003C1_003E__OnUpdate;
	}

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("BrayfloxsLongstopHelperTitle"),
		Description = LanguageManager.Get("BrayfloxsLongstopHelperDescription", LuminaWrapper.GetContentName(8u), LuminaWrapper.GetBNPCName(1298u), LuminaWrapper.GetEventItemName(2000521u)),
		Category = ModuleCategory.Duty,
		ModulesPrerequisite = new string[1] { "AutoTalkSkip" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		NeedAuth = true
	};

	protected override void Init()
	{
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += OnZoneChanged;
		OnZoneChanged(0u);
	}

	protected override void Uninit()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChanged;
		FrameworkManager frameworkManager = OmenServiceBase<FrameworkManager>.Instance();
		OnUpdateDelegate[] array = new OnUpdateDelegate[1];
		object obj = _003C_003EO._003C1_003E__OnUpdate;
		if (obj == null)
		{
			OnUpdateDelegate val = OnUpdate;
			_003C_003EO._003C1_003E__OnUpdate = val;
			obj = (object)val;
		}
		array[0] = (OnUpdateDelegate)obj;
		frameworkManager.Unreg((OnUpdateDelegate[])(object)array);
	}

	private static void OnZoneChanged(uint zone)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Expected O, but got Unknown
		FrameworkManager frameworkManager = OmenServiceBase<FrameworkManager>.Instance();
		OnUpdateDelegate[] array = new OnUpdateDelegate[1];
		object obj = _003C_003EO._003C1_003E__OnUpdate;
		if (obj == null)
		{
			OnUpdateDelegate val = OnUpdate;
			_003C_003EO._003C1_003E__OnUpdate = val;
			obj = (object)val;
		}
		array[0] = (OnUpdateDelegate)obj;
		frameworkManager.Unreg((OnUpdateDelegate[])(object)array);
		if (GameState.TerritoryType == 1041)
		{
			FrameworkManager frameworkManager2 = OmenServiceBase<FrameworkManager>.Instance();
			object obj2 = _003C_003EO._003C1_003E__OnUpdate;
			if (obj2 == null)
			{
				OnUpdateDelegate val2 = OnUpdate;
				_003C_003EO._003C1_003E__OnUpdate = val2;
				obj2 = (object)val2;
			}
			frameworkManager2.Reg((OnUpdateDelegate)obj2, 100u);
		}
	}

	private unsafe static void OnUpdate(IFramework framework)
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		ContentDirector* contentDirector = ((EventFramework)EventFramework.Instance()).GetContentDirector();
		if (contentDirector == null)
		{
			return;
		}
		if (((ContentDirector)contentDirector).DirectorTodos[0].CurrentCount != 1)
		{
			BattleChara* ptr = (*CharacterManager.Instance()).FindFirst((delegate*<BattleChara*, bool>)(&FindNPC));
			if (ptr != null)
			{
				new EventStartPackt(((BattleChara)ptr).EntityId, 1638401u).Send();
			}
			return;
		}
		FrameworkManager frameworkManager = OmenServiceBase<FrameworkManager>.Instance();
		OnUpdateDelegate[] array = new OnUpdateDelegate[1];
		object obj = _003C_003EO._003C1_003E__OnUpdate;
		if (obj == null)
		{
			OnUpdateDelegate val = OnUpdate;
			_003C_003EO._003C1_003E__OnUpdate = val;
			obj = (object)val;
		}
		array[0] = (OnUpdateDelegate)obj;
		frameworkManager.Unreg((OnUpdateDelegate[])(object)array);
		unsafe static bool FindNPC(BattleChara* chara)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Invalid comparison between Unknown and I4
			if (chara != null && (int)((BattleChara)chara).ObjectKind == 3 && ((BattleChara)chara).BaseId == 1004346)
			{
				return ((BattleChara)chara).NextDistance <= 4;
			}
			return false;
		}
	}
}
