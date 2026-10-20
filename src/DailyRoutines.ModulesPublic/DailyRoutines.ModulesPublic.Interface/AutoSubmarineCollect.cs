using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Command;
using Dalamud.Game.Text.Evaluator;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Hooking;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Component.GUI;
using InteropGenerator.Runtime;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using Lumina.Text;
using Lumina.Text.ReadOnly;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.Info.Game.Data;
using OmenTools.Info.Game.Enums;
using OmenTools.Info.Game.Packets.Upstream;
using OmenTools.Info.Lumina.ExtraSheets;
using OmenTools.Interop.Game.AddonEvent;
using OmenTools.Interop.Game.Helpers;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;
using TimeAgo;

namespace DailyRoutines.ModulesPublic.Interface;

public class AutoSubmarineCollect : ModuleBase
{
	private unsafe delegate nint SubmarineReturnTimeDelegate(SubmarineReturnTimePacket* packet);

	private class Config : ModuleConfig
	{
		public uint AutoCollectCount;

		public uint NotifyCount = 4u;

		public DateTimeOffset LastNotifyTime = DateTimeOffset.MinValue;

		public ulong LastNotifyContentID;
	}

	private class ItemDisplayNode : HorizontalListNode
	{
		public uint ItemID { get; init; }

		public IconImageNode? IconNode { get; private set; }

		public TextNode? NameNode { get; private set; }

		public TextNode? CountNode { get; private set; }

		public ItemDisplayNode(uint itemID, float width)
		{
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Expected O, but got Unknown
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			ItemID = itemID;
			IconNode = new IconImageNode
			{
				Size = new Vector2(32f),
				IsVisible = true,
				IconId = LuminaWrapper.GetItemIconID(ItemID),
				FitTexture = true
			};
			AddNode(IconNode);
			AddDummy(5f);
			NameNode = new TextNode
			{
				IsVisible = true,
				Position = new Vector2(0f, 6f),
				TextFlags = (TextFlags)1,
				FontSize = 14u,
				String = ReadOnlySeString.op_Implicit(LuminaWrapper.GetItemName(ItemID))
			};
			AddNode(NameNode);
			uint itemCount = LocalPlayerState.GetItemCount(ItemID);
			SeStringBuilder val = new SeStringBuilder();
			if (itemCount <= 20)
			{
				val.AddIcon((BitmapFontIcon)101).AddText(" ");
			}
			val.AddText($"{itemCount}");
			CountNode = new TextNode
			{
				IsVisible = true,
				TextFlags = (TextFlags)41,
				FontType = (FontType)1,
				AlignmentType = (AlignmentType)2,
				Position = new Vector2(width - 20f, 4f),
				TextColor = ColorHelper.GetColor(50u),
				TextOutlineColor = ColorHelper.GetColor((itemCount > 20) ? 28u : 17u),
				FontSize = 16u,
				String = ReadOnlySeString.op_Implicit(val.Build().Encode())
			};
			CountNode.AttachNode(this);
		}

		public void UpdateItemCount()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Expected O, but got Unknown
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			uint itemCount = LocalPlayerState.GetItemCount(ItemID);
			SeStringBuilder val = new SeStringBuilder();
			if (itemCount <= 20)
			{
				val.AddIcon((BitmapFontIcon)101).AddText(" ");
			}
			val.AddText($"{itemCount}");
			CountNode.String = ReadOnlySeString.op_Implicit(val.Build().Encode());
			CountNode.TextOutlineColor = ColorHelper.GetColor((itemCount > 20) ? 28u : 17u);
		}

		protected override void Dispose(bool disposing, bool isNativeDestructor)
		{
			IconNode?.Dispose();
			IconNode = null;
			NameNode?.Dispose();
			NameNode = null;
			CountNode?.Dispose();
			CountNode = null;
			base.Dispose(disposing, isNativeDestructor);
		}
	}

	[StructLayout(LayoutKind.Explicit, Size = 144)]
	private struct SubmarineReturnTimePacket
	{
		[FieldOffset(0)]
		public int ReturnTime1;

		[FieldOffset(36)]
		public int ReturnTime2;

		[FieldOffset(72)]
		public int ReturnTime3;

		[FieldOffset(108)]
		public int ReturnTime4;

		public int GetFinishCount()
		{
			return new List<int> { ReturnTime1, ReturnTime2, ReturnTime3, ReturnTime4 }.Count((int x) => x != 0 && x <= OmenServiceBase<StandardTimeManager>.Instance().UTCNowOffset.ToUnixTimeSeconds());
		}

		public int GetAvailableCount()
		{
			return new List<int> { ReturnTime1, ReturnTime2, ReturnTime3, ReturnTime4 }.Count((int x) => x != 0);
		}

		public DateTime GetLatestReturnTime()
		{
			return new List<int> { ReturnTime1, ReturnTime2, ReturnTime3, ReturnTime4 }.Where((int x) => x != 0).Max().ToUTCDateTimeFromUnixSeconds()
				.ToLocalTime();
		}
	}

	private record FreeCompanyWorkshopInfo
	{
		public Vector3 Position { get; init; }

		public uint TerritoryType { get; init; }

		public uint Map { get; init; }

		public uint WardIndex { get; init; }

		public uint PlotIndex { get; init; }

		public FreeCompanyWorkshopInfo(uint territoryType, uint wardIndex, uint plotIndex)
		{
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			TerritoryType = territoryType;
			WardIndex = wardIndex;
			PlotIndex = plotIndex;
			HousingMapMarkerInfo val = (from x in ((IEnumerable<SubrowCollection<HousingMapMarkerInfo>>)LuminaGetter.GetSub<HousingMapMarkerInfo>()).SelectMany((SubrowCollection<HousingMapMarkerInfo> x) => (IEnumerable<HousingMapMarkerInfo>)(object)x)
				where ((HousingMapMarkerInfo)(ref x)).Map.IsValid
				select x).Where(delegate(HousingMapMarkerInfo x)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				Map value = ((HousingMapMarkerInfo)(ref x)).Map.Value;
				return ((Map)(ref value)).TerritoryType.RowId == TerritoryType;
			}).FirstOrDefault((HousingMapMarkerInfo x) => ((HousingMapMarkerInfo)(ref x)).SubrowId == PlotIndex);
			Position = new Vector3(((HousingMapMarkerInfo)(ref val)).X, ((HousingMapMarkerInfo)(ref val)).Y, ((HousingMapMarkerInfo)(ref val)).Z);
			Map = ((HousingMapMarkerInfo)(ref val)).Map.RowId;
		}

		public static bool TryGet([NotNullWhen(true)] out FreeCompanyWorkshopInfo? workshopInfo)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			workshopInfo = null;
			HouseId ownedHouseId = HousingManager.GetOwnedHouseId((EstateType)0, -1);
			if (ownedHouseId.Id == 0L || ownedHouseId.WorldId != GameState.CurrentWorld)
			{
				return false;
			}
			workshopInfo = new FreeCompanyWorkshopInfo(ownedHouseId.TerritoryTypeId, ((HouseId)(ref ownedHouseId)).WardIndex, ((HouseId)(ref ownedHouseId)).PlotIndex);
			return true;
		}
	}

	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static Action<uint, SeString> _003C0_003E__OnClickCollectSubmarinePayload;

		public static LogMessageManager.PreLogMessageDelegate _003C1_003E__OnPreSendLogMessage;

		public static OnUpdateDelegate _003C2_003E__OnUpdate;

		public static Func<bool> _003C3_003E__TeleportToRoomSelect;

		public static Func<bool> _003C4_003E__TeleportToHouseEntry;

		public static Func<bool> _003C5_003E__TeleportToHouseZone;

		public static Func<bool> _003C6_003E__TeleportToPanel;

		public static Func<bool> _003C7_003E__IsOnValidSubmarineList;
	}

	private static readonly CompSig CurrentSubmarineIndexSig = new CompSig("48 8D 0D ?? ?? ?? ?? 80 A3");

	private static readonly CompSig SubmarineReturnTimeSig = new CompSig("40 53 48 83 EC ?? 48 8B D9 E8 ?? ?? ?? ?? 84 C0 74 ?? E8 ?? ?? ?? ?? 48 8B D3 48 8D 48 ?? 48 83 C4 ?? 5B E9 ?? ?? ?? ?? 48 83 C4 ?? 5B C3 CC CC CC CC CC CC CC CC CC CC CC CC CC CC CC CC CC CC 40 53 48 83 EC ?? 48 8B D9 E8 ?? ?? ?? ?? 84 C0 74 ?? E8 ?? ?? ?? ?? 48 8B D3");

	private Hook<SubmarineReturnTimeDelegate>? SubmarineReturnTimeHook;

	private Config config;

	private DalamudLinkPayload? collectSubmarinePayload;

	private VerticalListNode? itemListLayout;

	private readonly List<ItemDisplayNode> itemRenderers = new List<ItemDisplayNode>();

	private TextButtonNode? autoCollectNode;

	private bool isNextNotifyIgnoreTime;

	private const string COMMAND = "submarine";

	private const ulong INVALID_HOUSE_ID = ulong.MaxValue;

	private static readonly uint[] SubmarineItems = new uint[2] { 10155u, 10373u };

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoSubmarineCollectTitle"),
		Description = LanguageManager.Get("AutoSubmarineCollectDescription"),
		Category = ModuleCategory.Interface,
		ModulesPrerequisite = new string[1] { "AutoCutsceneSkip" },
		ModulesPair = new string[1] { "OptimizedInteraction" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		NeedAuth = true
	};

	private static List<string> VoyageListTitleText { get; } = (from text in SeStringExtensions.ToDalamudString(LuminaGetter.GetRowOrDefault<HouFixCompanySubmarine>(2u).Text).Payloads
		where (int)text.Type == 4
		select SantisizeText(((TextPayload)((text is TextPayload) ? text : null)).Text) into x
		where !string.IsNullOrWhiteSpace(x)
		select x).ToList().ToList();

	protected unsafe override void Init()
	{
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Expected O, but got Unknown
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Expected O, but got Unknown
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Expected O, but got Unknown
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Expected O, but got Unknown
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Expected O, but got Unknown
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Expected O, but got Unknown
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 30000
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		if (collectSubmarinePayload == null)
		{
			collectSubmarinePayload = OmenServiceBase<LinkPayloadManager>.Instance().Reg(OnClickCollectSubmarinePayload, out var _);
		}
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)1, "SelectYesno", new AddonEventDelegate(OnAddonSelectYesno));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)1, "AirShipExplorationResult", new AddonEventDelegate(OnExplorationResult));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)5, "SelectString", new AddonEventDelegate(OnAddonSelectString));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, "SelectString", new AddonEventDelegate(OnAddonSelectString));
		OmenServiceBase<CommandManager>.Instance().AddSubCommand("submarine", new CommandInfo(new HandlerDelegate(OnCommand))
		{
			HelpMessage = LanguageManager.Get("AutoSubmarineCollect-CommandHelp")
		});
		OmenServiceBase<LogMessageManager>.Instance().RegPre(OnPreSendLogMessage);
		if (SubmarineReturnTimeHook == null)
		{
			SubmarineReturnTimeHook = SubmarineReturnTimeSig.GetHook<SubmarineReturnTimeDelegate>(SubmarineReturnTimeDetour);
		}
		SubmarineReturnTimeHook.Enable();
		FrameworkManager frameworkManager = OmenServiceBase<FrameworkManager>.Instance();
		object obj2 = _003C_003EO._003C2_003E__OnUpdate;
		if (obj2 == null)
		{
			OnUpdateDelegate val = OnUpdate;
			_003C_003EO._003C2_003E__OnUpdate = val;
			obj2 = (object)val;
		}
		frameworkManager.Reg((OnUpdateDelegate)obj2, 600000u);
		OmenServiceBase<GameState>.Instance().Login += OnLogin;
	}

	protected override void Uninit()
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Expected O, but got Unknown
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Expected O, but got Unknown
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Expected O, but got Unknown
		OmenServiceBase<LogMessageManager>.Instance().Unreg(OnPreSendLogMessage);
		OmenServiceBase<CommandManager>.Instance().RemoveSubCommand("submarine");
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnExplorationResult });
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddonSelectYesno });
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddonSelectString });
		itemListLayout?.Dispose();
		itemListLayout = null;
		autoCollectNode?.Dispose();
		autoCollectNode = null;
		foreach (ItemDisplayNode itemRenderer in itemRenderers)
		{
			itemRenderer.Dispose();
		}
		itemRenderers.Clear();
		FrameworkManager frameworkManager = OmenServiceBase<FrameworkManager>.Instance();
		OnUpdateDelegate[] array = new OnUpdateDelegate[1];
		object obj = _003C_003EO._003C2_003E__OnUpdate;
		if (obj == null)
		{
			OnUpdateDelegate val = OnUpdate;
			_003C_003EO._003C2_003E__OnUpdate = val;
			obj = (object)val;
		}
		array[0] = (OnUpdateDelegate)obj;
		frameworkManager.Unreg((OnUpdateDelegate[])(object)array);
		OmenServiceBase<GameState>.Instance().Login -= OnLogin;
		if (collectSubmarinePayload != null)
		{
			OmenServiceBase<LinkPayloadManager>.Instance().Unreg(collectSubmarinePayload.CommandId);
		}
		collectSubmarinePayload = null;
	}

	protected override void ConfigUI()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(1, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("Command"));
		((ImU8String)(ref val)).AppendLiteral(":");
		ImGui.TextColored(ref vector, val);
		IndentDisposable val2 = ImRaii.PushIndent(1, true);
		try
		{
			ImU8String val3 = new ImU8String(8, 2);
			((ImU8String)(ref val3)).AppendLiteral("/pdr ");
			((ImU8String)(ref val3)).AppendFormatted<string>("submarine");
			((ImU8String)(ref val3)).AppendLiteral(" → ");
			((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get("AutoSubmarineCollect-CommandHelp"));
			ImGui.TextUnformatted(val3);
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
		ImGui.NewLine();
		ItemWidthDisposable val4 = ImRaii.ItemWidth(100f * Globals.GlobalUIScale);
		try
		{
			ImU8String val5 = new ImU8String(19, 1);
			((ImU8String)(ref val5)).AppendFormatted<string>(LanguageManager.Get("AutoSubmarineCollect-NotifyCount", config.NotifyCount));
			((ImU8String)(ref val5)).AppendLiteral("###NotifyCountInput");
			ImU8String val6 = val5;
			ref uint notifyCount = ref config.NotifyCount;
			ImU8String val7 = default(ImU8String);
			if (ImGui.InputUInt(val6, ref notifyCount, 0u, 0u, val7, (ImGuiInputTextFlags)0))
			{
				config.NotifyCount = Math.Clamp(config.NotifyCount, 0u, 4u);
			}
			if (ImGui.IsItemDeactivatedAfterEdit())
			{
				config.Save(this);
			}
			val7 = new ImU8String(19, 1);
			((ImU8String)(ref val7)).AppendFormatted<string>(LanguageManager.Get("AutoSubmarineCollect-AutoCollectCount", config.AutoCollectCount));
			((ImU8String)(ref val7)).AppendLiteral("###AutoCollectCount");
			if (ImGui.InputUInt(val7, ref config.AutoCollectCount, 0u, 0u, default(ImU8String), (ImGuiInputTextFlags)0))
			{
				config.NotifyCount = Math.Clamp(config.NotifyCount, 0u, 4u);
			}
			if (ImGui.IsItemDeactivatedAfterEdit())
			{
				config.Save(this);
			}
		}
		finally
		{
			((IDisposable)val4)?.Dispose();
		}
	}

	private unsafe void EnqueueTeleportTasks()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		base.TaskHelper.Abort();
		if (!FreeCompanyWorkshopInfo.TryGet(out FreeCompanyWorkshopInfo workshopInfo))
		{
			return;
		}
		HousingManager* ptr = HousingManager.Instance();
		List<Func<bool>> list = new List<Func<bool>>();
		if (((HousingManager)ptr).WorkshopTerritory != null)
		{
			HouseId houseId = ((WorkshopTerritory)((HousingManager)ptr).WorkshopTerritory).HouseId;
			if (houseId.TerritoryTypeId == workshopInfo.TerritoryType && ((HouseId)(ref houseId)).PlotIndex == workshopInfo.PlotIndex && ((HouseId)(ref houseId)).WardIndex == workshopInfo.WardIndex)
			{
				goto IL_01b9;
			}
		}
		if (((HousingManager)ptr).IndoorTerritory != null)
		{
			HouseId houseId2 = ((IndoorTerritory)((HousingManager)ptr).IndoorTerritory).HouseId;
			if (houseId2.TerritoryTypeId == workshopInfo.TerritoryType && ((HouseId)(ref houseId2)).PlotIndex == workshopInfo.PlotIndex && ((HouseId)(ref houseId2)).WardIndex == workshopInfo.WardIndex)
			{
				list.Add(TeleportToRoomSelect);
				goto IL_01b9;
			}
		}
		if (((HousingManager)ptr).OutdoorTerritory != null)
		{
			HouseId houseId3 = ((OutdoorTerritory)((HousingManager)ptr).OutdoorTerritory).HouseId;
			if (houseId3.TerritoryTypeId == workshopInfo.TerritoryType && GameState.Map == workshopInfo.Map && ((HouseId)(ref houseId3)).WardIndex == workshopInfo.WardIndex)
			{
				list.Add(TeleportToHouseEntry);
				list.Add(TeleportToRoomSelect);
				goto IL_01b9;
			}
		}
		list.Add(TeleportToHouseZone);
		list.Add(TeleportToHouseEntry);
		list.Add(TeleportToRoomSelect);
		goto IL_01b9;
		IL_01b9:
		list.Add(TeleportToPanel);
		list.Add(EnqueueSubmarineCollect);
		foreach (Func<bool> item in list)
		{
			base.TaskHelper.Enqueue(item);
		}
	}

	private unsafe static bool TeleportToHouseZone()
	{
		if (!Throttler.Shared.Throttle("AutoSubmarineCollect-TeleportToHouseZone"))
		{
			return false;
		}
		return ((Telepo)Telepo.Instance()).Teleport(96u, (byte)0);
	}

	private unsafe static bool TeleportToHouseEntry()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		if (Throttler.Shared.Throttle("AutoSubmarineCollect-TeleportToHouseEntry", 100u) && FreeCompanyWorkshopInfo.TryGet(out FreeCompanyWorkshopInfo workshopInfo) && ((HousingManager)HousingManager.Instance()).OutdoorTerritory != null)
		{
			HouseId houseId = ((OutdoorTerritory)((HousingManager)HousingManager.Instance()).OutdoorTerritory).HouseId;
			if (((HouseId)(ref houseId)).WardIndex == workshopInfo.WardIndex && GameState.Map == workshopInfo.Map)
			{
				IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
				if (localPlayer != null)
				{
					if (((HouseUnit)(&((OutdoorTerritory)((HousingManager)HousingManager.Instance()).OutdoorTerritory).HouseUnit)).PlotIndex == workshopInfo.PlotIndex)
					{
						IGameObject gameObject = (from x in IOmenDalamudService<IObjectTable>.Instance()
							where x != null && (int)x.ObjectKind == 7 && x.DataID == 2002737
							orderby Vector2.DistanceSquared(x.Position.ToVector2(), workshopInfo.Position.ToVector2())
							select x).FirstOrDefault();
						if (gameObject != null)
						{
							Vector3 vector = gameObject.Position;
							if (RaycastHelper.TryGetGroundPosition(vector, out var groundPosition))
							{
								vector = groundPosition;
							}
							if (IConditionExtension.get_IsOnMount(DalamudServiceExtension.Instance<ICondition>()))
							{
								if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)77])
								{
									OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommandComplexLocation(ExecuteCommandComplexFlag.Dismount, localPlayer.Position, RotationHelper.CharaToPacket(localPlayer.Rotation));
								}
								else
								{
									OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.Dismount);
								}
								return false;
							}
							if (LocalPlayerState.DistanceTo3D(vector) > 2f)
							{
								OmenServiceBase<MovementManager>.Instance().TPSmart_InZone(vector);
								return false;
							}
							if (!UIModuleExtension.IsScreenReady())
							{
								return false;
							}
							return gameObject.TargetInteract();
						}
					}
					OmenServiceBase<MovementManager>.Instance().TPSmart_InZone(workshopInfo.Position);
					return false;
				}
			}
		}
		return false;
	}

	private unsafe static bool TeleportToRoomSelect()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		if (Throttler.Shared.Throttle("AutoSubmarineCollect-TeleportToRoomSelect", 100u) && FreeCompanyWorkshopInfo.TryGet(out FreeCompanyWorkshopInfo workshopInfo) && ((HousingManager)HousingManager.Instance()).IndoorTerritory != null)
		{
			HouseId houseId = ((IndoorTerritory)((HousingManager)HousingManager.Instance()).IndoorTerritory).HouseId;
			if (((HouseId)(ref houseId)).WardIndex == workshopInfo.WardIndex && ((HouseId)(ref houseId)).PlotIndex == workshopInfo.PlotIndex && IOmenDalamudService<IObjectTable>.Instance().LocalPlayer != null)
			{
				if (!(*EventFramework.Instance()).IsEventIDNearby(EventId.op_Implicit(721074u)))
				{
					OmenServiceBase<MovementManager>.Instance().TPSmart_InZone(Vector3.Zero);
					return false;
				}
				if (!UIModuleExtension.IsScreenReady())
				{
					return false;
				}
				if (!IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()))
				{
					new EventStartPackt(LocalPlayerState.EntityID, 721074u).Send();
				}
				return AddonSelectStringEvent.Select(((object)LuminaGetter.GetRowOrDefault<HousingPersonalRoomEntrance>(11u).Text/*cast due to constrained. prefix*/).ToString());
			}
		}
		return false;
	}

	private unsafe static bool TeleportToPanel()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		if (Throttler.Shared.Throttle("AutoSubmarineCollect-TeleportToPanel", 100u) && FreeCompanyWorkshopInfo.TryGet(out FreeCompanyWorkshopInfo workshopInfo) && ((HousingManager)HousingManager.Instance()).WorkshopTerritory != null)
		{
			HouseId houseId = ((WorkshopTerritory)((HousingManager)HousingManager.Instance()).WorkshopTerritory).HouseId;
			if (((HouseId)(ref houseId)).WardIndex == workshopInfo.WardIndex && ((HouseId)(ref houseId)).PlotIndex == workshopInfo.PlotIndex && IOmenDalamudService<IObjectTable>.Instance().LocalPlayer != null)
			{
				if ((*EventFramework.Instance()).TryGetNearestEvent((EventHandlerInfo x) => x.EventId == EventId.op_Implicit(3276843u), (GameObject _) => true, default(Vector3), out var _, out var eventObjectID, out var _, out var _))
				{
					IGameObject gameObject = IOmenDalamudService<IObjectTable>.Instance().SearchByID(GameObjectId.op_Implicit(eventObjectID));
					if (gameObject != null)
					{
						Vector3 nearestPointToObject = LocalPlayerState.GetNearestPointToObject(gameObject);
						if (LocalPlayerState.DistanceToObject3D(gameObject, ignoreRadius: false) > 1f)
						{
							OmenServiceBase<MovementManager>.Instance().TPSmart_InZone(RaycastHelper.TryGetNearestVerticalHit(nearestPointToObject, out var hitInfo) ? hitInfo.Point : nearestPointToObject);
							return false;
						}
						if (!UIModuleExtension.IsScreenReady())
						{
							return false;
						}
						if (!IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()))
						{
							gameObject.TargetInteract();
							return false;
						}
						CustomTalk rowOrDefault = LuminaGetter.GetRowOrDefault<CustomTalk>(721343u);
						return AddonSelectStringEvent.Select(((object)((CustomTalk)(ref rowOrDefault)).MainOption/*cast due to constrained. prefix*/).ToString());
					}
				}
				OmenServiceBase<MovementManager>.Instance().TPSmart_InZone(Vector3.Zero);
				return false;
			}
		}
		return false;
	}

	private unsafe void OnExplorationResult(AddonEvent type, AddonArgs args)
	{
		if (Addons.AirShipExplorationResult != null && (*Addons.AirShipExplorationResult).IsAddonAndNodesReady())
		{
			(*Addons.AirShipExplorationResult).Callback(1);
			if (base.TaskHelper.IsBusy)
			{
				((AtkUnitBase)Addons.AirShipExplorationResult).IsVisible = false;
			}
		}
	}

	private unsafe void OnAddonSelectString(AddonEvent type, AddonArgs args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		if ((int)type != 5)
		{
			if ((int)type == 6)
			{
				itemListLayout = null;
				autoCollectNode = null;
				itemRenderers.Clear();
			}
		}
		else
		{
			if (Addons.SelectString == null)
			{
				return;
			}
			if (itemListLayout == null && IsOnValidSubmarineList())
			{
				int num = ((AtkResNode)((AtkUnitBase)Addons.SelectString).RootNode).Width - 25;
				itemListLayout = new VerticalListNode
				{
					IsVisible = true,
					Position = new Vector2(22f, 15f)
				};
				itemListLayout.AttachNode(((AtkUnitBase)Addons.SelectString).RootNode);
				autoCollectNode = new TextButtonNode
				{
					IsVisible = true,
					Position = new Vector2(-10f, 0f),
					Size = new Vector2(num - 15, 30f),
					String = ReadOnlySeString.op_Implicit(Info.Title),
					OnClick = delegate
					{
						base.TaskHelper.Abort();
						EnqueueSubmarineCollect();
					}
				};
				itemListLayout.AddNode(autoCollectNode);
				itemListLayout.AddDummy(5f);
				uint[] submarineItems = SubmarineItems;
				foreach (uint num3 in submarineItems)
				{
					ItemDisplayNode itemDisplayNode = new ItemDisplayNode(num3, num)
					{
						IsVisible = true,
						Size = new Vector2(num, 38f),
						ItemTooltip = num3
					};
					itemDisplayNode.AddNodeFlags((NodeFlags)256);
					itemListLayout.AddNode(itemDisplayNode);
					itemRenderers.Add(itemDisplayNode);
				}
				AtkTextNode* textNodeById = ((AtkUnitBase)Addons.SelectString).GetTextNodeById(2u);
				if (textNodeById != null)
				{
					((AtkTextNode)textNodeById).ToggleVisibility(false);
				}
				((AtkResNode)((AtkUnitBase)Addons.SelectString).RootNode).SetHeight((ushort)271);
				((AtkComponentNode)((AtkUnitBase)Addons.SelectString).WindowNode).SetHeight((ushort)271);
				for (int num4 = 0; num4 < ((AtkUldManager)(&((AtkComponentBase)((AtkComponentNode)((AtkUnitBase)Addons.SelectString).WindowNode).Component).UldManager)).NodeListCount; num4++)
				{
					AtkResNode* ptr = ((AtkUldManager)(&((AtkComponentBase)((AtkComponentNode)((AtkUnitBase)Addons.SelectString).WindowNode).Component).UldManager)).NodeList[num4];
					if (ptr != null)
					{
						if (((AtkResNode)ptr).Height == 231)
						{
							((AtkResNode)ptr).SetHeight((ushort)271);
						}
						if (((AtkResNode)ptr).Height == 215)
						{
							((AtkResNode)ptr).SetHeight((ushort)255);
						}
					}
				}
				AtkComponentList* componentListById = ((AtkUnitBase)Addons.SelectString).GetComponentListById(3u);
				if (componentListById != null)
				{
					((AtkComponentNode)((AtkComponentList)componentListById).OwnerNode).SetYFloat(132f);
				}
			}
			if (itemListLayout == null || !Throttler.Shared.Throttle("AutoSubmarineCollect-UpdateCount"))
			{
				return;
			}
			foreach (ItemDisplayNode itemRenderer in itemRenderers)
			{
				itemRenderer.UpdateItemCount();
			}
		}
	}

	private void OnAddonSelectYesno(AddonEvent type, AddonArgs args)
	{
		if (base.TaskHelper.IsBusy)
		{
			AddonSelectYesnoEvent.ClickYes();
		}
	}

	private unsafe nint SubmarineReturnTimeDetour(SubmarineReturnTimePacket* submarineReturnTimePacket)
	{
		NotifyFinishCount(submarineReturnTimePacket);
		return SubmarineReturnTimeHook.Original(submarineReturnTimePacket);
	}

	private void OnLogin()
	{
		isNextNotifyIgnoreTime = true;
		SendRefreshSubmarineInfo();
	}

	private static void OnClickCollectSubmarinePayload(uint arg1, SeString arg2)
	{
		OmenServiceBase<ChatManager>.Instance().SendMessage("/pdr submarine");
	}

	private static void OnUpdate(IFramework _)
	{
		SendRefreshSubmarineInfo();
	}

	private void OnCommand(string command, string arguments)
	{
		EnqueueTeleportTasks();
	}

	private static void OnPreSendLogMessage(ref bool isPrevented, ref uint logMessageID, ref LogMessageQueueItem values)
	{
		if (logMessageID == 4109)
		{
			isPrevented = true;
		}
	}

	private bool EnqueueSubmarineCollect()
	{
		base.TaskHelper.Enqueue(() => !DalamudServiceExtension.Instance<ICondition>().Any((ConditionFlag[])(object)new ConditionFlag[2]
		{
			(ConditionFlag)35,
			(ConditionFlag)78
		}), "等待过场动画结束");
		base.TaskHelper.Enqueue((Func<bool>)IsOnValidSubmarineList, "等待潜水艇列表界面出现", 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
		base.TaskHelper.Enqueue(delegate
		{
			if (IsLackOfSubmarineItems() || !IsAnySubmarinesAvailable(out var submarineIndex))
			{
				base.TaskHelper.Abort();
			}
			else
			{
				base.TaskHelper.Enqueue(() => AddonSelectStringEvent.Select(submarineIndex), $"收取 {submarineIndex} 号潜艇", 0, null, null, null, null, 1);
				base.TaskHelper.DelayNext(2000, "延迟 2 秒, 等待远航结果确认", 1);
				base.TaskHelper.Enqueue((Action)EnqueueSubmarineStateCheck, "确认潜艇信息, 准备修理和再次出航", 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 1);
			}
		}, "检测是否有潜艇待收取");
		return true;
	}

	private unsafe void EnqueueSubmarineStateCheck()
	{
		base.TaskHelper.Enqueue(() => (*Addons.AirShipExplorationDetail).IsAddonAndNodesReady(), "等待出航信息确认界面出现");
		base.TaskHelper.Enqueue(delegate
		{
			if (!Throttler.Shared.Throttle("AutoSubmarineCollect-RepairSubmarine", 100u))
			{
				return false;
			}
			if (!IsAnySubmarinePartWaitForRepair(out List<int> parts))
			{
				return true;
			}
			int currentSubmarineIndex = *CurrentSubmarineIndexSig.GetStatic<int>();
			parts.ForEach(delegate(int index)
			{
				OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RepairSubmarinePart, (uint)currentSubmarineIndex, (uint)index);
			});
			return false;
		}, "检测并修理潜水艇部件");
		base.TaskHelper.Enqueue(delegate
		{
			if (!(*Addons.AirShipExplorationDetail).IsAddonAndNodesReady())
			{
				return false;
			}
			(*Addons.AirShipExplorationDetail).Callback(0);
			((AtkUnitBase)Addons.AirShipExplorationDetail).Close(true);
			return true;
		}, "再次确认出航");
		base.TaskHelper.DelayNext(2000, "等待出航动画结束");
		base.TaskHelper.Enqueue((Func<bool>)EnqueueSubmarineCollect, "开始新一轮", 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
	}

	private unsafe static bool IsAnySubmarinePartWaitForRepair(out List<int> parts)
	{
		parts = new List<int>();
		int num = *CurrentSubmarineIndexSig.GetStatic<int>();
		if ((num < 0 || num > 3) ? true : false)
		{
			return false;
		}
		InventoryManager* ptr = InventoryManager.Instance();
		if (ptr == null)
		{
			return false;
		}
		InventoryContainer* inventoryContainer = ((InventoryManager)ptr).GetInventoryContainer((InventoryType)25004);
		if (inventoryContainer == null || !((InventoryContainer)inventoryContainer).IsLoaded)
		{
			return false;
		}
		int num2 = 5 * num;
		for (int i = 0; i < 4; i++)
		{
			InventoryItem* inventorySlot = ((InventoryContainer)inventoryContainer).GetInventorySlot(i + num2);
			if (inventorySlot != null && ((InventoryItem)inventorySlot).Condition <= 0)
			{
				parts.Add(i);
			}
		}
		return parts.Count > 0;
	}

	private unsafe static bool IsLackOfSubmarineItems()
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		InventoryManager* intPtr = InventoryManager.Instance();
		uint num = 0u;
		if (((InventoryManager)intPtr).GetInventoryItemCount(10373u, false, true, true, (short)0) < 20)
		{
			num = 10373u;
		}
		if (((InventoryManager)intPtr).GetInventoryItemCount(10155u, false, true, true, (short)0) < 15)
		{
			num = 10155u;
		}
		if (num != 0)
		{
			ReadOnlySeString se = LanguageManager.GetSe("AutoSubmarineCollect-LackSpecificItems", SeString.CreateItemLink(num, (ItemKind)0, (string)null));
			OmenServiceBase<NotifyHelper>.Instance().Chat(se);
			NotifyHelper.ToastError(se);
			return true;
		}
		return false;
	}

	private unsafe static bool IsAnySubmarinesAvailable(out int index)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		index = -1;
		if (!IsOnValidSubmarineList())
		{
			return false;
		}
		Span<HousingWorkshopSubmersibleSubData> data = ((HousingWorkshopSubmersibleData)(&((WorkshopTerritory)((HousingManager)HousingManager.Instance()).WorkshopTerritory).Submersible)).Data;
		for (int i = 0; i < data.Length; i++)
		{
			HousingWorkshopSubmersibleSubData val = data[i];
			if (val.RankId != 0 && !((((HousingWorkshopSubmersibleSubData)(ref val)).GetReturnTime() - OmenServiceBase<StandardTimeManager>.Instance().Now.ToUniversalTime()).TotalSeconds > 0.0))
			{
				index = i;
				return true;
			}
		}
		return false;
	}

	private unsafe static bool IsOnValidSubmarineList()
	{
		if (((HousingManager)HousingManager.Instance()).WorkshopTerritory == null)
		{
			return false;
		}
		if (!(*Addons.SelectString).IsAddonAndNodesReady())
		{
			return false;
		}
		string text = ((object)(*(CStringPointer*)(&((AtkValue)((byte*)((AtkUnitBase)Addons.SelectString).AtkValues + (nint)2 * (nint)Unsafe.SizeOf<AtkValue>())).String))/*cast due to constrained. prefix*/).ToString();
		if (string.IsNullOrEmpty(text) || !VoyageListTitleText.All(text.Contains))
		{
			return false;
		}
		return true;
	}

	private unsafe void NotifyFinishCount(SubmarineReturnTimePacket* packet)
	{
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		int availableCount = packet->GetAvailableCount();
		int finishCount = packet->GetFinishCount();
		if (availableCount == 0)
		{
			return;
		}
		if (config.AutoCollectCount != 0 && finishCount >= Math.Min(availableCount, config.AutoCollectCount))
		{
			OmenServiceBase<ChatManager>.Instance().SendMessage("/pdr submarine");
		}
		if (config.NotifyCount != 0 && finishCount < config.NotifyCount)
		{
			return;
		}
		if (config.LastNotifyContentID == LocalPlayerState.ContentID)
		{
			TimeSpan timeSpan = OmenServiceBase<StandardTimeManager>.Instance().UTCNowOffset - config.LastNotifyTime;
			if (!isNextNotifyIgnoreTime && availableCount != finishCount && timeSpan < TimeSpan.FromHours(4))
			{
				return;
			}
		}
		isNextNotifyIgnoreTime = false;
		config.LastNotifyContentID = LocalPlayerState.ContentID;
		config.LastNotifyTime = OmenServiceBase<StandardTimeManager>.Instance().UTCNowOffset;
		config.Save(this);
		DateTime latestReturnTime = packet->GetLatestReturnTime();
		RentedSeStringBuilder val = default(RentedSeStringBuilder);
		((RentedSeStringBuilder)(ref val))._002Ector();
		try
		{
			SeStringBuilder builder = ((RentedSeStringBuilder)(ref val)).Builder;
			if (availableCount == finishCount)
			{
				ISeStringEvaluator obj = DalamudServiceExtension.Instance<ISeStringEvaluator>();
				SeStringParameter reference = SeStringParameter.op_Implicit(LanguageManager.Get("AutoSubmarineCollect-Notification-AllReturned", latestReturnTime.ToString("yyyy/MM/dd HH:mm:ss"), latestReturnTime.TimeAgo()));
				ReadOnlySeString val2 = obj.EvaluateFromAddon(371u, new Span<SeStringParameter>(ref reference), (ClientLanguage?)null);
				builder.AppendDalamudSeString((Payload)(object)collectSubmarinePayload).Append(val2).AppendDalamudSeString((Payload)(object)RawPayload.LinkTerminator);
				OmenServiceBase<NotifyHelper>.Instance().Chat(builder.ToReadOnlySeString());
			}
			else
			{
				string text = LanguageManager.Get("AutoSubmarineCollect-Notification-SomeExploring", availableCount - finishCount, latestReturnTime.ToString("yyyy/MM/dd HH:mm:ss"));
				OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(text));
			}
		}
		finally
		{
			((IDisposable)(*(RentedSeStringBuilder*)(&val))/*cast due to constrained. prefix*/).Dispose();
		}
	}

	private static void SendRefreshSubmarineInfo()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (LocalPlayerState.ContentID != 0L && GameState.ContentFinderCondition == 0 && !(HousingManager.GetOwnedHouseId((EstateType)0, -1) == HouseId.op_Implicit(ulong.MaxValue)))
		{
			OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestSubmarine, 1u);
		}
	}

	private static string SantisizeText(string text)
	{
		char[] array = new char[6] { '(', '.', ')', ']', ':', '/' };
		foreach (char oldChar in array)
		{
			text = text.Replace(oldChar, ' ');
		}
		return text.Trim();
	}
}
