using System.Numerics;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.DutyState;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using Lumina.Excel.Sheets;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Extensions;
using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.Helpers;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic.Duty;

public class AutoMovePetCenter : ModuleBase
{
	private unsafe delegate void ProcessPacketSpawnNPCDelegate(uint targetID, byte* packetData);

	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static DutyStartedDelegate _003C0_003E__OnDutyStarted;
	}

	private static readonly CompSig ProcessPacketSpawnNPCSig = new CompSig("48 89 5C 24 08 57 48 81 EC 30 04 00 00 48 8B DA 8B F9 E8 ?? ?? ?? ?? 3C 01 75 21 E8 ?? ?? ?? ?? 3C 01 75 18 80 BB 82 00 00 00 02 75 0F 8B 05 ?? ?? ?? ?? 39 43 54 0F 85 ?? ?? ?? ?? 0F B6 53 7E 48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 0F B6 53 7E 48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 8D 44 24 28 C7 44 24 20 02 00 00 00");

	private Hook<ProcessPacketSpawnNPCDelegate>? ProcessPacketSpawnNPCHook;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoMovePetCenterTitle"),
		Description = LanguageManager.Get("AutoMovePetCenterDescription"),
		Category = ModuleCategory.Duty,
		Author = new string[1] { "逆光" },
		ModulesConflict = new string[1] { "AutoMovePetPosition" }
	};

	protected unsafe override void Init()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected O, but got Unknown
		if (ProcessPacketSpawnNPCHook == null)
		{
			ProcessPacketSpawnNPCHook = ProcessPacketSpawnNPCSig.GetHook<ProcessPacketSpawnNPCDelegate>(ProcessPacketSpawnNPCDetour);
		}
		ProcessPacketSpawnNPCHook.Enable();
		IDutyState obj = DalamudServiceExtension.Instance<IDutyState>();
		object obj2 = _003C_003EO._003C0_003E__OnDutyStarted;
		if (obj2 == null)
		{
			DutyStartedDelegate val = OnDutyStarted;
			_003C_003EO._003C0_003E__OnDutyStarted = val;
			obj2 = (object)val;
		}
		obj.DutyStarted += (DutyStartedDelegate)obj2;
	}

	protected override void Uninit()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		IDutyState obj = DalamudServiceExtension.Instance<IDutyState>();
		object obj2 = _003C_003EO._003C0_003E__OnDutyStarted;
		if (obj2 == null)
		{
			DutyStartedDelegate val = OnDutyStarted;
			_003C_003EO._003C0_003E__OnDutyStarted = val;
			obj2 = (object)val;
		}
		obj.DutyStarted -= (DutyStartedDelegate)obj2;
	}

	private static void OnDutyStarted(IDutyStateEventArgs args)
	{
		MovePetToMapCenter(LocalPlayerState.EntityID);
	}

	private unsafe void ProcessPacketSpawnNPCDetour(uint targetID, byte* packetData)
	{
		ProcessPacketSpawnNPCHook.Original(targetID, packetData);
		uint* ptr = (uint*)packetData + 21;
		if (ptr != null)
		{
			MovePetToMapCenter(*ptr);
		}
	}

	private static void MovePetToMapCenter(uint npcEntityID)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		bool flag = GameState.ContentFinderCondition == 0 || GameState.Map == 0 || npcEntityID != LocalPlayerState.EntityID;
		if (!flag)
		{
			ContentFinderCondition contentFinderConditionData = GameState.ContentFinderConditionData;
			uint rowId = ((ContentFinderCondition)(ref contentFinderConditionData)).ContentType.RowId;
			bool flag2 = rowId - 4 <= 1;
			flag = !flag2;
		}
		if (!flag && IOmenDalamudService<IObjectTable>.Instance().LocalPlayer != null)
		{
			Vector3 location = PositionHelper.TextureToWorld(new Vector2(1024f), GameState.MapData).ToPlayerHeight();
			OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommandComplexLocation(ExecuteCommandComplexFlag.PetAction, location, 3u);
		}
	}
}
