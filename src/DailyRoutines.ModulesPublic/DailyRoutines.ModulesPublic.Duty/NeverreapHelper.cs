using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.InstanceContent;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Common.Math;
using FFXIVClientStructs.STD;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Info.Game.Packets.Upstream;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.OmenService.ZoneIndicator;

namespace DailyRoutines.ModulesPublic.Duty;

public class NeverreapHelper : ModuleBase
{
	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static Action<uint> _003C0_003E__OnZoneChanged;

		public static OnUpdateDelegate _003C1_003E__OnUpdate;
	}

	private ZoneIndicatorHandle? handle;

	private const uint STONE_NPC_DATA_ID = 1013331u;

	private const uint STONE_NPC_EVENT_ID = 1638407u;

	private const uint SHADOW_MONSTER_BNPC_NAME_ID = 3727u;

	private static readonly Vector2 FirstBossCenter = new Vector2(53.6f, 222.7f);

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("NeverreapHelperTitle"),
		Description = LanguageManager.Get("NeverreapHelperDescription"),
		Category = ModuleCategory.Duty,
		ModulesPrerequisite = new string[1] { "AutoTalkSkip" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected unsafe override void Init()
	{
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += OnZoneChanged;
		OnZoneChanged(0u);
		handle = OmenServiceBase<ZoneIndicatorRenderer>.Instance().RegPermanent(420u, delegate
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			ContentDirector* contentDirector = ((EventFramework)EventFramework.Instance()).GetContentDirector();
			if (contentDirector == null)
			{
				return new List<nint>();
			}
			if (!DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)26])
			{
				return new List<nint>();
			}
			StdVector<DirectorTodo> directorTodos = ((ContentDirector)contentDirector).DirectorTodos;
			if (directorTodos[1].CurrentCount != 1)
			{
				return new List<nint>();
			}
			if (LocalPlayerState.DistanceTo2DSquared(FirstBossCenter) > 625f)
			{
				return new List<nint>();
			}
			BattleChara* ptr = (*CharacterManager.Instance()).FindFirst((delegate*<BattleChara*, bool>)(&FindFirstShadow));
			if (ptr != null)
			{
				int num = 1;
				List<nint> list = new List<nint>(num);
				CollectionsMarshal.SetCount(list, num);
				CollectionsMarshal.AsSpan(list)[0] = (nint)ptr;
				return list;
			}
			return new List<nint>();
		}, (nint ptr) => Vector3.op_Implicit(((GameObject)ptr).Position), new ZoneIndicatorOptions<nint>
		{
			TextGetter = (nint _) => new ZoneIndicatorText
			{
				Text = ReadOnlySeString.op_Implicit(LuminaWrapper.GetBNPCName(3727u)),
				TextScale = 1.6f
			}
		});
		unsafe static bool FindFirstShadow(BattleChara* chara)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Invalid comparison between Unknown and I4
			if (chara != null && (int)((BattleChara)chara).ObjectKind == 2)
			{
				return ((BattleChara)chara).NameId == 3727;
			}
			return false;
		}
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
		handle?.Unreg();
		handle = null;
	}

	private static void OnZoneChanged(uint u)
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
		if (GameState.TerritoryType == 420)
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
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Invalid comparison between Unknown and I4
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		ContentDirector* contentDirector = ((EventFramework)EventFramework.Instance()).GetContentDirector();
		if (contentDirector == null)
		{
			return;
		}
		if ((int)((ContentDirector)contentDirector).DirectorTodos[0].Type != 2)
		{
			BattleChara* ptr = (*CharacterManager.Instance()).FindFirst((delegate*<BattleChara*, bool>)(&FindNPC));
			if (ptr != null)
			{
				new EventStartPackt(((BattleChara)ptr).EntityId, 1638407u).Send();
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
			if (chara != null && (int)((BattleChara)chara).ObjectKind == 3 && ((BattleChara)chara).BaseId == 1013331)
			{
				return ((BattleChara)chara).NextDistance <= 4;
			}
			return false;
		}
	}
}
