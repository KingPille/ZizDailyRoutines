using System.Collections.Frozen;
using System.Collections.Generic;
using System.Drawing;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.InstanceContent;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Common.Math;
using FFXIVClientStructs.STD;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService.Abstractions;
using OmenTools.OmenService.ZoneIndicator;

namespace DailyRoutines.ModulesPublic.Duty;

public class HullbreakerIsleHelper : ModuleBase
{
	private ZoneIndicatorHandle? trapHandle;

	private ZoneIndicatorHandle? fakeTreasureHandle;

	private static readonly FrozenSet<uint> FakeTreasuresID = FrozenSet.Create<uint>(2004074u, 2004075u, 2004076u, 2004077u, 2004078u, 2004079u);

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("HullbreakerIsleHelperTitle"),
		Description = LanguageManager.Get("HullbreakerIsleHelperDescription", LuminaWrapper.GetContentName(23u), LuminaWrapper.GetBNPCName(2891u), LuminaWrapper.GetBNPCName(2896u)),
		Category = ModuleCategory.Duty
	};

	protected unsafe override void Init()
	{
		trapHandle = OmenServiceBase<ZoneIndicatorRenderer>.Instance().RegPermanent(361u, delegate
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			ContentDirector* contentDirector = ((EventFramework)EventFramework.Instance()).GetContentDirector();
			if (contentDirector == null)
			{
				return new List<nint>();
			}
			StdVector<DirectorTodo> directorTodos = ((ContentDirector)contentDirector).DirectorTodos;
			return ((int)directorTodos[1].Type != 0) ? new List<nint>() : (*CharacterManager.Instance()).FindAll((delegate*<BattleChara*, bool>)(&FindTrap));
		}, (nint ptr) => Vector3.op_Implicit(((GameObject)ptr).Position), new ZoneIndicatorOptions<nint>
		{
			TextGetter = (nint ptr) => new ZoneIndicatorText
			{
				Text = ReadOnlySeString.op_Implicit(((GameObject)ptr).NameString ?? ""),
				TextScale = 1.2f,
				TextColor = KnownColor.OrangeRed.ToVector4()
			},
			Surrounding = new ZoneIndicatorSurrounding
			{
				Type = ZoneIndicatorSurrounding.Shape.Circle,
				Radius = 1.5f,
				Thickness = 2f
			}
		});
		fakeTreasureHandle = OmenServiceBase<ZoneIndicatorRenderer>.Instance().RegPermanent(361u, delegate
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			ContentDirector* contentDirector = ((EventFramework)EventFramework.Instance()).GetContentDirector();
			if (contentDirector == null)
			{
				return new List<nint>();
			}
			StdVector<DirectorTodo> directorTodos = ((ContentDirector)contentDirector).DirectorTodos;
			return ((int)directorTodos[2].Type == 0 || directorTodos[2].CurrentCount == 3) ? new List<nint>() : (*EventObjectManager.Instance()).FindAll((delegate*<GameObject*, bool>)(&FindFakeTreasure));
		}, (nint ptr) => Vector3.op_Implicit(((GameObject)ptr).Position), new ZoneIndicatorOptions<nint>
		{
			TextGetter = (nint ptr) => new ZoneIndicatorText
			{
				Text = ReadOnlySeString.op_Implicit(((GameObject)ptr).NameString + " (" + LanguageManager.Get("HullbreakerIsleHelper-FakeTreasureTip") + ")"),
				TextScale = 1.2f,
				TextColor = KnownColor.DarkOrange.ToVector4()
			}
		});
		unsafe static bool FindFakeTreasure(GameObject* chara)
		{
			if (chara != null && FakeTreasuresID.Contains(((GameObject)chara).BaseId))
			{
				return ((GameObject)chara).GetIsTargetable();
			}
			return false;
		}
		unsafe static bool FindTrap(BattleChara* chara)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Invalid comparison between Unknown and I4
			if (chara != null && ((BattleChara)chara).NameId == 2891)
			{
				return (int)((BattleChara)chara).TargetableStatus == 248;
			}
			return false;
		}
	}

	protected override void Uninit()
	{
		trapHandle?.Unreg();
		trapHandle = null;
		fakeTreasureHandle?.Unreg();
		fakeTreasureHandle = null;
	}
}
