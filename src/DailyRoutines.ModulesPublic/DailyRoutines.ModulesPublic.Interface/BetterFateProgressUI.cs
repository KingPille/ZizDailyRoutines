using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using DailyRoutines.Common.Interface.Windows;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;

namespace DailyRoutines.ModulesPublic.Interface;

public class BetterFateProgressUI : ModuleBase
{
	private class ZoneFateProgressInfo(uint version, uint counter, uint achievementID, uint zoneID, uint aetheryteID)
	{
		private IDalamudTextureWrap? backgroundTexture;

		public uint Version { get; init; } = version;

		public uint Counter { get; init; } = counter;

		public uint AchievementID { get; init; } = achievementID;

		public uint ZoneID { get; init; } = zoneID;

		public uint AetheryteID { get; init; } = aetheryteID;

		public void SetBackgroundTexture(IDalamudTextureWrap texture)
		{
			backgroundTexture = texture;
		}

		public void Draw()
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			if (!LuminaGetter.TryGetRow<TerritoryType>(ZoneID, out TerritoryType item))
			{
				return;
			}
			ImU8String val = default(ImU8String);
			((ImU8String)(ref val))._002Ector(0, 1);
			((ImU8String)(ref val)).AppendFormatted<string>(ToString());
			ChildDisposable val2 = ImRaii.Child(val, ChildSize, true, (ImGuiWindowFlags)24);
			try
			{
				if (ChildDisposable.op_Implicit(val2))
				{
					DrawBackgroundImage();
					DrawZoneName(item.ExtractPlaceName());
					DrawFateProgress();
				}
			}
			finally
			{
				((ChildDisposable)(ref val2)).Dispose();
			}
			HandleInteraction(item);
		}

		private void DrawBackgroundImage()
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			if (backgroundTexture != null)
			{
				Vector2 cursorPos = ImGui.GetCursorPos();
				ImGui.SetCursorPos(cursorPos - Globals.ScaledVector2(10f, 4f));
				ImGui.Image(backgroundTexture.Handle, ImGui.GetWindowSize() + Globals.ScaledVector2(10f, 4f));
				ImGui.SetCursorPos(cursorPos);
			}
		}

		private static void DrawZoneName(string name)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			ImGui.SetWindowFontScale(1.05f);
			ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 8f * Globals.GlobalUIScale);
			ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 4f * Globals.GlobalUIScale);
			ImGui.TextUnformatted(ImU8String.op_Implicit(name));
			ImGui.SetWindowFontScale(1f);
		}

		private void DrawFateProgress()
		{
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			if (OmenServiceBase<AchievementManager>.Instance().TryGetAchievement(AchievementID, out AchievementInfo achievementInfo))
			{
				uint current = achievementInfo.Current;
				ImGui.SetWindowFontScale(0.8f);
				string text = ((current > 6) ? $"{current - 6}/60" : $"{current}/6");
				ImGui.SetCursorPosY(ImGui.GetContentRegionMax().Y - ImGui.CalcTextSize(ImU8String.op_Implicit(text), false, -1f).Y);
				ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 4f * Globals.GlobalUIScale);
				ImGui.TextUnformatted(ImU8String.op_Implicit(text));
				DrawFinalProgress(current);
			}
		}

		private static void DrawFinalProgress(uint fateProgress)
		{
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			uint num = 66 - fateProgress;
			string text = ((fateProgress == 66) ? LuminaWrapper.GetAddonText(3930u) : LanguageManager.Get("BetterFateProgressUI-LeftFateAmount", num));
			ImGui.SetWindowFontScale(0.95f);
			ImGui.SameLine();
			ImGui.SetCursorPosX(ImGui.GetContentRegionMax().X - ImGui.CalcTextSize(ImU8String.op_Implicit(text), false, -1f).X);
			Vector4 parsedGold = ImGuiColors.ParsedGold;
			ImGui.TextColored(ref parsedGold, ImU8String.op_Implicit(text));
		}

		private unsafe void HandleInteraction(TerritoryType zoneSheetRow)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			if (ImGui.IsItemHovered())
			{
				ImGui.SetMouseCursor((ImGuiMouseCursor)7);
			}
			if (ImGui.IsItemClicked((ImGuiMouseButton)0))
			{
				(*AgentMap.Instance()).SetMapAndOpen(((TerritoryType)(ref zoneSheetRow)).Map.RowId);
			}
			if (ImGui.IsItemClicked((ImGuiMouseButton)1) && AetheryteID != 0)
			{
				((Telepo)Telepo.Instance()).Teleport(AetheryteID, (byte)0);
			}
		}

		public override string ToString()
		{
			return $"ZoneFateProgressInfo_Version{Version + 5}.0_{ZoneID}_{Counter}";
		}
	}

	private readonly CancellationTokenSource cancelSource = new CancellationTokenSource();

	private int bicolorGemAmount;

	private Vector2 bicolorGemComponentSize;

	private bool isWindowUnlock;

	[CompilerGenerated]
	private static FrozenDictionary<uint, List<ZoneFateProgressInfo>> _003CVersionToZoneInfos_003Ek__BackingField;

	private static readonly Vector2 ChildSize = Globals.ScaledVector2(450f, 150f);

	private static readonly uint BicolorGemCap;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("BetterFateProgressUITitle"),
		Description = LanguageManager.Get("BetterFateProgressUIDescription"),
		Category = ModuleCategory.Interface
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	private static FrozenDictionary<uint, List<ZoneFateProgressInfo>> VersionToZoneInfos
	{
		get
		{
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			if (_003CVersionToZoneInfos_003Ek__BackingField != null)
			{
				return _003CVersionToZoneInfos_003Ek__BackingField;
			}
			Dictionary<uint, List<ZoneFateProgressInfo>> dictionary = new Dictionary<uint, List<ZoneFateProgressInfo>>();
			int num = 0;
			uint num2 = 0u;
			uint key2;
			foreach (KeyValuePair<uint, uint> item3 in FateAchievements.AchievementToZone)
			{
				item3.Deconstruct(out var key, out key2);
				uint num3 = key;
				uint num4 = key2;
				if (LuminaGetter.TryGetRow<TerritoryType>(num4, out TerritoryType item))
				{
					uint num5 = ((num3 >= 3559) ? 2u : ((num3 >= 3022) ? 1u : 0u));
					if (num2 != num5)
					{
						num = 0;
						num2 = num5;
					}
					uint rowId = ((TerritoryType)(ref item)).Aetheryte.RowId;
					ZoneFateProgressInfo item2 = new ZoneFateProgressInfo(num5, (uint)num, num3, num4, rowId);
					dictionary.TryAdd(num5, new List<ZoneFateProgressInfo>());
					dictionary[num5].Add(item2);
					num++;
				}
			}
			foreach (KeyValuePair<uint, List<ZoneFateProgressInfo>> item4 in dictionary)
			{
				item4.Deconstruct(out key2, out var value);
				uint num6 = key2;
				List<ZoneFateProgressInfo> list = value;
				string text = $"ui/uld/FlyingPermission{num6 + 3}_hr1.tex";
				for (int i = 0; i < list.Count; i++)
				{
					IDalamudTextureWrap backgroundTexture = DalamudServiceExtension.Instance().UiBuilder.LoadUld("ui/uld/FateProgress.uld").LoadTexturePart(text, i);
					list[i].SetBackgroundTexture(backgroundTexture);
				}
			}
			return _003CVersionToZoneInfos_003Ek__BackingField = dictionary.ToFrozenDictionary();
		}
	}

	protected override void Init()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Expected O, but got Unknown
		if (GameState.IsLoggedIn)
		{
			ObtainAllFateProgress();
		}
		if (base.Overlay == null)
		{
			Overlay overlay = (base.Overlay = new Overlay(this));
		}
		Overlay? overlay3 = base.Overlay;
		((Window)overlay3).Flags = (ImGuiWindowFlags)(((Window)overlay3).Flags & -2);
		Overlay? overlay4 = base.Overlay;
		((Window)overlay4).Flags = (ImGuiWindowFlags)(((Window)overlay4).Flags | 0x1A);
		Overlay? overlay5 = base.Overlay;
		WindowSizeConstraints value = default(WindowSizeConstraints);
		((WindowSizeConstraints)(ref value))._002Ector();
		((WindowSizeConstraints)(ref value)).MinimumSize = ChildSize;
		((Window)overlay5).SizeConstraints = value;
		((Window)base.Overlay).WindowName = LuminaWrapper.GetAddonText(3933u) + "###BetterFateProgressUI";
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)1, "FateProgress", new AddonEventDelegate(OnAddon));
	}

	protected override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddon });
		cancelSource.Cancel();
		cancelSource.Dispose();
	}

	protected override void ConfigUI()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("BetterFateProgressUI-UnlockWindow")), ref isWindowUnlock))
		{
			if (isWindowUnlock)
			{
				Overlay? overlay = base.Overlay;
				((Window)overlay).Flags = (ImGuiWindowFlags)(((Window)overlay).Flags & -65);
				Overlay? overlay2 = base.Overlay;
				((Window)overlay2).Flags = (ImGuiWindowFlags)(((Window)overlay2).Flags & -3);
			}
			else
			{
				Overlay? overlay3 = base.Overlay;
				((Window)overlay3).Flags = (ImGuiWindowFlags)(((Window)overlay3).Flags | 0x42);
			}
		}
	}

	protected unsafe override void OverlayPreDraw()
	{
		if (Throttler.Shared.Throttle("BetterFateProgress-Refresh", 10000u))
		{
			ObtainAllFateProgress();
			bicolorGemAmount = ((InventoryManager)InventoryManager.Instance()).GetInventoryItemCount(26807u, false, true, true, (short)0);
		}
	}

	protected override void OverlayOnOpen()
	{
		ObtainAllFateProgress();
	}

	protected override void OverlayUI()
	{
		using (OmenServiceBase<FontManager>.Instance().UIFont120.Push())
		{
			DrawBicolorGemComponent();
			DrawFateProgressTabs();
		}
	}

	private void DrawBicolorGemComponent()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		Vector2 cursorPos = ImGui.GetCursorPos();
		float num = ImGui.GetWindowSize().X - bicolorGemComponentSize.X;
		ImGuiStylePtr style = ImGui.GetStyle();
		ImGui.SetCursorPosX(num - ((ImGuiStylePtr)(ref style)).ItemSpacing.X * 2f);
		GroupDisposable val = ImRaii.Group();
		try
		{
			Item rowOrDefault = LuminaGetter.GetRowOrDefault<Item>(26807u);
			ImGui.Image(ImageHelper.GetGameIcon(((Item)(ref rowOrDefault)).Icon).Handle, new Vector2(ImGui.GetFrameHeight()));
			ImGui.SameLine();
			ImGui.AlignTextToFramePadding();
			ImU8String val2 = new ImU8String(1, 2);
			((ImU8String)(ref val2)).AppendFormatted<int>(bicolorGemAmount);
			((ImU8String)(ref val2)).AppendLiteral("/");
			((ImU8String)(ref val2)).AppendFormatted<uint>(BicolorGemCap);
			ImGui.TextUnformatted(val2);
		}
		finally
		{
			((GroupDisposable)(ref val)).Dispose();
		}
		bicolorGemComponentSize = ImGui.GetItemRectSize();
		ImGui.SetCursorPos(cursorPos);
	}

	private static void DrawFateProgressTabs()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		GroupDisposable val = ImRaii.Group();
		try
		{
			TabBarDisposable val2 = ImRaii.TabBar(ImU8String.op_Implicit("FateProgressTab"));
			try
			{
				if (!(!val2))
				{
					ImmutableArray<uint>.Enumerator enumerator = VersionToZoneInfos.Keys.GetEnumerator();
					while (enumerator.MoveNext())
					{
						DrawFateProgressTabItem(enumerator.Current);
					}
				}
			}
			finally
			{
				((TabBarDisposable)(ref val2)).Dispose();
			}
		}
		finally
		{
			((GroupDisposable)(ref val)).Dispose();
		}
	}

	private static void DrawFateProgressTabItem(uint version)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (!VersionToZoneInfos.TryGetValue(version, out List<ZoneFateProgressInfo> value))
		{
			return;
		}
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(2, 1);
		((ImU8String)(ref val)).AppendFormatted<uint>(version + 5);
		((ImU8String)(ref val)).AppendLiteral(".0");
		TabItemDisposable val2 = ImRaii.TabItem(val);
		try
		{
			if (!val2)
			{
				return;
			}
			int num = 0;
			foreach (ZoneFateProgressInfo item in value)
			{
				item.Draw();
				if (num % 2 == 0)
				{
					ImGui.SameLine();
				}
				num++;
			}
		}
		finally
		{
			((TabItemDisposable)(ref val2)).Dispose();
		}
	}

	private unsafe static void ObtainAllFateProgress()
	{
		for (byte b = 0; b < FateAchievements.VersionCount; b++)
		{
			((Achievement)(&((UIState)UIState.Instance()).Achievement)).RequestFateProgressTab(b);
		}
	}

	private unsafe void OnAddon(AddonEvent type, AddonArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		AtkUnitBase* address = (AtkUnitBase*)args.Addon.Address;
		if (address != null)
		{
			((AtkUnitBase)address).Close(true);
			Overlay? overlay = base.Overlay;
			((Window)overlay).IsOpen = !((Window)overlay).IsOpen;
		}
	}

	static BetterFateProgressUI()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		Item rowOrDefault = LuminaGetter.GetRowOrDefault<Item>(26807u);
		BicolorGemCap = ((Item)(ref rowOrDefault)).StackSize;
	}
}
