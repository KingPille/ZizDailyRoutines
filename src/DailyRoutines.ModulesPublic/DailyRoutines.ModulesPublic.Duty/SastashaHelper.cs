using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.InstanceContent;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Common.Math;
using FFXIVClientStructs.STD;
using KamiToolKit.Classes;
using Lumina.Text.ReadOnly;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.OmenService.ZoneIndicator;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;

namespace DailyRoutines.ModulesPublic.Duty;

public class SastashaHelper : ModuleBase
{
	private uint correctBookID;

	private ZoneIndicatorHandle? handle;

	private static readonly FrozenDictionary<uint, (uint CoralDataID, ushort UIColor)> BookToCoral = new Dictionary<uint, (uint, ushort)>
	{
		[2000212u] = (2000213u, 37),
		[2001548u] = (2000214u, 17),
		[2001549u] = (2000215u, 45)
	}.ToFrozenDictionary();

	private static readonly Vector2 FirstBossCenter = new Vector2(75f, -45f);

	private const uint DUTY_ZONE_ID = 1036u;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("SastashaHelperTitle"),
		Description = LanguageManager.Get("SastashaHelperDescription", LuminaWrapper.GetContentName(4u)),
		Category = ModuleCategory.Duty
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected unsafe override void Init()
	{
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 30000
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += OnZoneChanged;
		OnZoneChanged(0u);
		handle = OmenServiceBase<ZoneIndicatorRenderer>.Instance().RegPermanent(1036u, delegate
		{
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			if (correctBookID == 0 || !BookToCoral.TryGetValue(correctBookID, out var data))
			{
				return new List<nint>();
			}
			ContentDirector* contentDirector = ((EventFramework)EventFramework.Instance()).GetContentDirector();
			if (contentDirector == null)
			{
				return new List<nint>();
			}
			StdVector<DirectorTodo> directorTodos = ((ContentDirector)contentDirector).DirectorTodos;
			if (directorTodos[0].CurrentCount != 0)
			{
				return new List<nint>();
			}
			if (LocalPlayerState.DistanceTo2DSquared(FirstBossCenter) > 625f)
			{
				return new List<nint>();
			}
			GameObject* ptr = (*EventObjectManager.Instance()).FindFirst((nint num2) => num2 != (nint)0 && (int)((GameObject)num2).ObjectKind == 7 && ((GameObject)num2).BaseId == data.CoralDataID);
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
			TextGetter = (nint ptr) => new ZoneIndicatorText
			{
				Text = ReadOnlySeString.op_Implicit("→ " + ((GameObject)ptr).NameString + " ←"),
				TextScale = 1.6f,
				TextColor = ColorHelper.GetColor(BookToCoral[correctBookID].UIColor)
			}
		});
	}

	protected override void Uninit()
	{
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChanged;
		handle?.Unreg();
		handle = null;
		correctBookID = 0u;
	}

	private void OnZoneChanged(uint u)
	{
		base.TaskHelper?.Abort();
		correctBookID = 0u;
		if (GameState.TerritoryType == 1036)
		{
			base.TaskHelper.Enqueue((Func<bool>)GetCorrectCoral, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
		}
	}

	private unsafe bool GetCorrectCoral()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		if (!UIModuleExtension.IsScreenReady())
		{
			return false;
		}
		ContentDirector* contentDirector = ((EventFramework)EventFramework.Instance()).GetContentDirector();
		if (contentDirector == null)
		{
			return false;
		}
		StdVector<DirectorTodo> directorTodos = ((ContentDirector)contentDirector).DirectorTodos;
		if (directorTodos[0].CurrentCount != 0)
		{
			return true;
		}
		IGameObject gameObject = IOmenDalamudService<IObjectTable>.Instance().SearchObject((IGameObject x) => x != null && x.IsTargetable && (int)x.ObjectKind == 7 && BookToCoral.ContainsKey(x.DataID), IObjectTable.EventRange);
		if (gameObject == null)
		{
			return false;
		}
		(uint, ushort) tuple = BookToCoral[gameObject.DataID];
		RentedSeStringBuilder val = default(RentedSeStringBuilder);
		((RentedSeStringBuilder)(ref val))._002Ector();
		try
		{
			OmenServiceBase<NotifyHelper>.Instance().Chat(LanguageManager.GetSe("SastashaHelper-Message", ((RentedSeStringBuilder)(ref val)).Builder.PushColorType((uint)tuple.Item2).Append(LuminaWrapper.GetEObjName(tuple.Item1)).PopColorType()
				.ToReadOnlySeString()));
			correctBookID = gameObject.DataID;
			return true;
		}
		finally
		{
			((IDisposable)(*(RentedSeStringBuilder*)(&val))/*cast due to constrained. prefix*/).Dispose();
		}
	}
}
