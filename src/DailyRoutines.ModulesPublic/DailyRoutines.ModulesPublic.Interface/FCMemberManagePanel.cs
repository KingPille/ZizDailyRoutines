using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using DailyRoutines.Common.Interface.Windows;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Interface;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models.Native;
using OmenTools.OmenService;
using OmenTools.Threading;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.ModulesPublic.Interface;

public class FCMemberManagePanel : ModuleBase
{
	private sealed class MemberRecord
	{
		public ulong ContentID { get; init; }

		public int Index { get; set; }

		public uint OnlineStatus { get; set; }

		public string Name { get; set; } = string.Empty;

		public string RankText { get; set; } = string.Empty;

		public byte RankIndex { get; set; }

		public bool IsOnline { get; set; }

		public int? OfflineMinutes { get; set; }

		public ISharedImmediateTexture? JobIcon { get; set; }

		public string JobText { get; set; } = string.Empty;

		public string LocationText { get; set; } = string.Empty;
	}

	private enum LastOnlineRange
	{
		All,
		Online,
		WithinHour,
		WithinDay,
		WithinWeek,
		WithinMonth,
		BeyondMonth,
		Unknown
	}

	private readonly Dictionary<ulong, MemberRecord> members = new Dictionary<ulong, MemberRecord>();

	private readonly HashSet<ulong> selectedContentIDs = new HashSet<ulong>();

	private readonly HashSet<byte> rankFilter = new HashSet<byte>();

	private uint totalMemberCount;

	private int currentPage;

	private bool isDescending;

	private string nameFilter = string.Empty;

	private LastOnlineRange lastOnlineFilter;

	private ulong[]? pendingTargets;

	private bool requestConfirmPopup;

	private const uint PAGE_SIZE = 200u;

	private const int PAGE_LIMIT = 3;

	private const string RANK_FILTER_POPUP_ID = "MemberRankFilter_Popup";

	private const string LAST_ONLINE_FILTER_POPUP_ID = "MemberLastOnlineFilter_Popup";

	private const byte FC_RANK_COUNT = 16;

	private const int MEMBER_RANK_INDEX_SHIFT = 12;

	private const ulong ONLINE_STATE_MASK = 141836999983104uL;

	private const string DIGIT_PLACEHOLDER = "@@NUM@@";

	private static readonly LastOnlineRange[] LastOnlineRanges = new LastOnlineRange[8]
	{
		LastOnlineRange.All,
		LastOnlineRange.Online,
		LastOnlineRange.WithinHour,
		LastOnlineRange.WithinDay,
		LastOnlineRange.WithinWeek,
		LastOnlineRange.WithinMonth,
		LastOnlineRange.BeyondMonth,
		LastOnlineRange.Unknown
	};

	private static readonly (uint RowID, int UnitMinutes)[] OfflineTimeAddons = new(uint, int)[4]
	{
		(39u, 5),
		(40u, 1),
		(41u, 60),
		(42u, 1440)
	};

	private static List<(Regex Pattern, int UnitMinutes)>? OfflineTimePatterns;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("FCMemberManagePanelTitle"),
		Description = LanguageManager.Get("FCMemberManagePanelDescription"),
		Category = ModuleCategory.Interface
	};

	protected unsafe override void Init()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Expected O, but got Unknown
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Expected O, but got Unknown
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 3000
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		if (base.Overlay == null)
		{
			Overlay overlay = (base.Overlay = new Overlay(this));
		}
		Overlay? overlay3 = base.Overlay;
		((Window)overlay3).Flags = (ImGuiWindowFlags)(((Window)overlay3).Flags & -2);
		Overlay? overlay4 = base.Overlay;
		((Window)overlay4).Flags = (ImGuiWindowFlags)(((Window)overlay4).Flags & -65);
		Overlay? overlay5 = base.Overlay;
		((Window)overlay5).Flags = (ImGuiWindowFlags)(((Window)overlay5).Flags & -3);
		((Window)base.Overlay).WindowName = LanguageManager.Get("FCMemberManagePanelTitle");
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)1, "FreeCompanyMember", new AddonEventDelegate(OnFreeCompanyMemberAddon));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, "FreeCompanyMember", new AddonEventDelegate(OnFreeCompanyMemberAddon));
		if (Addons.FreeCompanyMember != null && (*Addons.FreeCompanyMember).IsAddonAndNodesReady())
		{
			OnFreeCompanyMemberAddon((AddonEvent)1, null);
		}
	}

	protected override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnFreeCompanyMemberAddon });
		ResetMembers();
	}

	protected override void OverlayPreDraw()
	{
		if (DalamudServiceExtension.Instance<IClientState>().IsLoggedIn && Throttler.Shared.Throttle("FCMemberManagePanel-SyncMembers", 1000u))
		{
			SyncMembers();
		}
	}

	protected override void OverlayUI()
	{
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		int pageCount = GetPageCount();
		DisabledDisposable val = ImRaii.Disabled(currentPage <= 0);
		try
		{
			if (ImGuiOm.ButtonIcon("MemberPagePrev", (FontAwesomeIcon)61700, string.Empty, useStaticFont: true))
			{
				SwitchPage(currentPage - 1);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		ImGui.SameLine();
		ImGui.AlignTextToFramePadding();
		ImU8String val2 = default(ImU8String);
		((ImU8String)(ref val2))._002Ector(3, 2);
		((ImU8String)(ref val2)).AppendFormatted<int>(currentPage + 1);
		((ImU8String)(ref val2)).AppendLiteral(" / ");
		((ImU8String)(ref val2)).AppendFormatted<int>(pageCount);
		ImGui.TextUnformatted(val2);
		ImGui.SameLine();
		val = ImRaii.Disabled(currentPage >= pageCount - 1);
		try
		{
			if (ImGuiOm.ButtonIcon("MemberPageNext", (FontAwesomeIcon)61701, string.Empty, useStaticFont: true))
			{
				SwitchPage(currentPage + 1);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		ImGui.SameLine();
		ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
		ImGui.InputTextWithHint(ImU8String.op_Implicit("###MemberSearchInput"), ImU8String.op_Implicit(LanguageManager.Get("PleaseSearch")), ref nameFilter, 128, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
		List<MemberRecord> list = FilteredMembers();
		Vector2 contentRegionAvail = ImGui.GetContentRegionAvail();
		contentRegionAvail.Y = 0f;
		Vector2 vector = contentRegionAvail;
		TableDisposable val3 = ImRaii.Table(ImU8String.op_Implicit("FCMembersTable"), 6, (ImGuiTableFlags)1921, vector);
		try
		{
			if (!val3)
			{
				return;
			}
			float frameHeight = ImGui.GetFrameHeight();
			ImGui.TableSetupColumn(ImU8String.op_Implicit("序号"), (ImGuiTableColumnFlags)8, frameHeight, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("名称"), (ImGuiTableColumnFlags)4, 30f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("阶级"), (ImGuiTableColumnFlags)4, 25f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("职业"), (ImGuiTableColumnFlags)4, 25f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("位置"), (ImGuiTableColumnFlags)4, 25f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("勾选框"), (ImGuiTableColumnFlags)8, frameHeight, 0u);
			ImGui.TableNextRow((ImGuiTableRowFlags)1);
			DrawHeaderRow();
			foreach (MemberRecord item in list)
			{
				if (item.ContentID == LocalPlayerState.ContentID)
				{
					continue;
				}
				IdDisposable val4 = ImRaii.PushId(ImU8String.op_Implicit(item.ContentID.ToString()), true);
				try
				{
					bool flag = selectedContentIDs.Contains(item.ContentID);
					ImGui.TableNextRow();
					ImGui.TableNextColumn();
					ImU8String val5 = new ImU8String(0, 1);
					((ImU8String)(ref val5)).AppendFormatted<int>(item.Index);
					if (ImGui.Selectable(val5, flag, (ImGuiSelectableFlags)2, default(Vector2)) && !selectedContentIDs.Remove(item.ContentID))
					{
						selectedContentIDs.Add(item.ContentID);
					}
					DrawMemberContextMenu(item);
					ImGui.TableNextColumn();
					DrawOnlineStatus(item);
					ImGui.TextUnformatted(ImU8String.op_Implicit(item.Name));
					ImGui.TableNextColumn();
					ImGui.TextUnformatted(ImU8String.op_Implicit(item.RankText));
					ImGui.TableNextColumn();
					DrawJob(item);
					ImGui.TableNextColumn();
					ImGui.TextUnformatted(ImU8String.op_Implicit(item.LocationText));
					ImGui.TableNextColumn();
					val = ImRaii.Disabled();
					try
					{
						ImU8String val6 = new ImU8String(11, 1);
						((ImU8String)(ref val6)).AppendLiteral("##");
						((ImU8String)(ref val6)).AppendFormatted<ulong>(item.ContentID);
						((ImU8String)(ref val6)).AppendLiteral("_Checkbox");
						ImGui.Checkbox(val6, ref flag);
					}
					finally
					{
						((IDisposable)val)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)val4)?.Dispose();
				}
			}
			DrawConfirmPopup();
		}
		finally
		{
			((TableDisposable)(ref val3)).Dispose();
		}
	}

	private void DrawHeaderRow()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TableNextColumn();
		if (ImGui.Button(ImU8String.op_Implicit(isDescending ? FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61538) : FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61539)), default(Vector2)))
		{
			isDescending = !isDescending;
		}
		ImGui.TableNextColumn();
		ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("Name")));
		ImGui.TableNextColumn();
		if (ImGui.Selectable(ImU8String.op_Implicit(LanguageManager.Get("FCMemberManagePanel-Rank")), rankFilter.Count > 0, (ImGuiSelectableFlags)0, default(Vector2)))
		{
			ImGui.OpenPopup(ImU8String.op_Implicit("MemberRankFilter_Popup"), (ImGuiPopupFlags)0);
		}
		DrawRankFilterPopup();
		ImGui.TableNextColumn();
		ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("Job")));
		ImGui.TableNextColumn();
		if (ImGui.Selectable(ImU8String.op_Implicit(LanguageManager.Get("FCMemberManagePanel-PositionLastTime")), lastOnlineFilter != LastOnlineRange.All, (ImGuiSelectableFlags)0, default(Vector2)))
		{
			ImGui.OpenPopup(ImU8String.op_Implicit("MemberLastOnlineFilter_Popup"), (ImGuiPopupFlags)0);
		}
		DrawLastOnlineFilterPopup();
		ImGui.TableNextColumn();
		if (ImGuiOm.ButtonIcon("OpenMultiPopup", (FontAwesomeIcon)61761, string.Empty, useStaticFont: true))
		{
			ImGui.OpenPopup(ImU8String.op_Implicit("Multi_Popup"), (ImGuiPopupFlags)0);
		}
		DrawMultiContextMenu();
	}

	private void DrawRankFilterPopup()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		PopupDisposable val = ImRaii.ContextPopupItem(ImU8String.op_Implicit("MemberRankFilter_Popup"));
		try
		{
			if (!val)
			{
				return;
			}
			foreach (var rankOption in GetRankOptions())
			{
				byte item = rankOption.RankIndex;
				string item2 = rankOption.Name;
				bool flag = rankFilter.Contains(item);
				ImU8String val2 = new ImU8String(12, 2);
				((ImU8String)(ref val2)).AppendFormatted<string>(item2);
				((ImU8String)(ref val2)).AppendLiteral("##FilterRank");
				((ImU8String)(ref val2)).AppendFormatted<byte>(item);
				if (ImGui.Checkbox(val2, ref flag) && !rankFilter.Remove(item))
				{
					rankFilter.Add(item);
				}
			}
			ImGui.Spacing();
			if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("Reset")), new Vector2(-1f, 0f)))
			{
				rankFilter.Clear();
			}
		}
		finally
		{
			((PopupDisposable)(ref val)).Dispose();
		}
	}

	private void DrawLastOnlineFilterPopup()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		PopupDisposable val = ImRaii.ContextPopupItem(ImU8String.op_Implicit("MemberLastOnlineFilter_Popup"));
		try
		{
			if (!val)
			{
				return;
			}
			LastOnlineRange[] lastOnlineRanges = LastOnlineRanges;
			foreach (LastOnlineRange lastOnlineRange in lastOnlineRanges)
			{
				ImU8String val2 = new ImU8String(18, 1);
				((ImU8String)(ref val2)).AppendFormatted<string>(GetLastOnlineRangeName(lastOnlineRange));
				((ImU8String)(ref val2)).AppendLiteral("##FilterLastOnline");
				if (ImGui.RadioButton(val2, lastOnlineFilter == lastOnlineRange))
				{
					lastOnlineFilter = lastOnlineRange;
				}
			}
			ImGui.Spacing();
			if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("Reset")), new Vector2(-1f, 0f)))
			{
				lastOnlineFilter = LastOnlineRange.All;
			}
		}
		finally
		{
			((PopupDisposable)(ref val)).Dispose();
		}
	}

	private unsafe static List<(byte RankIndex, string Name)> GetRankOptions()
	{
		List<(byte, string)> list = new List<(byte, string)>();
		InfoProxyFreeCompany* ptr = InfoProxyFreeCompany.Instance();
		if (ptr == null)
		{
			return list;
		}
		for (byte b = 0; b < 16; b++)
		{
			string rankNameText = ptr->GetRankNameText(b);
			if (!string.IsNullOrWhiteSpace(rankNameText))
			{
				list.Add((b, rankNameText));
			}
		}
		return list;
	}

	private static string GetLastOnlineRangeName(LastOnlineRange range)
	{
		return range switch
		{
			LastOnlineRange.Online => LanguageManager.Get("FCMemberManagePanel-LastOnline-Online"), 
			LastOnlineRange.WithinHour => LanguageManager.Get("FCMemberManagePanel-LastOnline-WithinHour"), 
			LastOnlineRange.WithinDay => LanguageManager.Get("FCMemberManagePanel-LastOnline-WithinDay"), 
			LastOnlineRange.WithinWeek => LanguageManager.Get("FCMemberManagePanel-LastOnline-WithinWeek"), 
			LastOnlineRange.WithinMonth => LanguageManager.Get("FCMemberManagePanel-LastOnline-WithinMonth"), 
			LastOnlineRange.BeyondMonth => LanguageManager.Get("FCMemberManagePanel-LastOnline-BeyondMonth"), 
			LastOnlineRange.Unknown => LanguageManager.Get("Unknown"), 
			_ => LanguageManager.Get("All"), 
		};
	}

	private static void DrawOnlineStatus(MemberRecord member)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		if (member.OnlineStatus != 0 && LuminaGetter.TryGetRow<OnlineStatus>(member.OnlineStatus, out OnlineStatus item))
		{
			ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
			GameIconLookup val = new GameIconLookup(((OnlineStatus)(ref item)).Icon, false, true, (ClientLanguage?)null);
			IDalamudTextureWrap wrapOrDefault = obj.GetFromGameIcon(ref val).GetWrapOrDefault((IDalamudTextureWrap)null);
			if (wrapOrDefault != null)
			{
				float cursorPosY = ImGui.GetCursorPosY();
				ImGui.SetCursorPosY(cursorPosY + 2f * Globals.GlobalUIScale);
				ImGui.Image(wrapOrDefault.Handle, new Vector2(ImGui.GetTextLineHeight()));
				ImGui.SetCursorPosY(cursorPosY);
				ImGui.SameLine();
			}
		}
	}

	private static void DrawJob(MemberRecord member)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (member.JobIcon != null)
		{
			float cursorPosY = ImGui.GetCursorPosY();
			ImGui.SetCursorPosY(cursorPosY + 2f * Globals.GlobalUIScale);
			ImGui.Image(member.JobIcon.GetWrapOrEmpty().Handle, new Vector2(ImGui.GetTextLineHeight()));
			ImGui.SetCursorPosY(cursorPosY);
			ImGui.SameLine();
		}
		ImGui.TextUnformatted(ImU8String.op_Implicit(member.JobText));
	}

	private void DrawMemberContextMenu(MemberRecord member)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(6, 1);
		((ImU8String)(ref val)).AppendFormatted<ulong>(member.ContentID);
		((ImU8String)(ref val)).AppendLiteral("_Popup");
		PopupDisposable val2 = ImRaii.ContextPopupItem(val);
		try
		{
			if (!(!val2))
			{
				ImGui.TextUnformatted(ImU8String.op_Implicit(member.Name));
				ImGui.Separator();
				ImGui.Spacing();
				if (ImGui.MenuItem(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(15083u)), false, true))
				{
					OpenCharaCard(member.ContentID);
				}
				if (ImGui.MenuItem(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(51u)), false, true))
				{
					OpenCharacterDetail(member.ContentID);
				}
				if (ImGui.MenuItem(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(2807u)), false, true))
				{
					OpenFreeCompanyProfile(member.ContentID);
				}
				DrawRankMenu(new ulong[1] { member.ContentID });
				if (ImGui.MenuItem(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(2801u)), false, true))
				{
					RequestConfirm(new ulong[1] { member.ContentID });
				}
			}
		}
		finally
		{
			((PopupDisposable)(ref val2)).Dispose();
		}
	}

	private void DrawMultiContextMenu()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		PopupDisposable val = ImRaii.ContextPopupItem(ImU8String.op_Implicit("Multi_Popup"));
		try
		{
			if (!val)
			{
				return;
			}
			ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("FCMemberManagePanel-SelectedMembers", selectedContentIDs.Count)));
			ImGui.Separator();
			ImGui.Spacing();
			DisabledDisposable val2 = ImRaii.Disabled(selectedContentIDs.Count == 0);
			try
			{
				ulong[] targets = selectedContentIDs.ToArray();
				DrawRankMenu(targets);
				if (ImGui.MenuItem(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(2801u)), false, true))
				{
					RequestConfirm(targets);
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		finally
		{
			((PopupDisposable)(ref val)).Dispose();
		}
	}

	private unsafe void DrawRankMenu(ulong[] targets)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		MenuDisposable val = ImRaii.Menu(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(2656u)));
		try
		{
			if (!val)
			{
				return;
			}
			InfoProxyFreeCompany* ptr = InfoProxyFreeCompany.Instance();
			if (ptr == null)
			{
				return;
			}
			foreach (var assignableRank in ptr->GetAssignableRanks())
			{
				byte item = assignableRank.RankIndex;
				string item2 = assignableRank.Name;
				ImU8String val2 = new ImU8String(6, 2);
				((ImU8String)(ref val2)).AppendFormatted<string>(item2);
				((ImU8String)(ref val2)).AppendLiteral("##Rank");
				((ImU8String)(ref val2)).AppendFormatted<byte>(item);
				if (ImGui.MenuItem(val2, false, true))
				{
					EnqueueMemberAction(targets, AgentFreeCompany.MemberActionType.Promote, item);
				}
			}
		}
		finally
		{
			((MenuDisposable)(ref val)).Dispose();
		}
	}

	private void DrawConfirmPopup()
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		if (pendingTargets == null)
		{
			return;
		}
		using ImGuiOm.ModalPopupDisposable modalPopupDisposable = ImGuiOm.PopupModal(Info.Title + "##ConfirmKickPopup", ref requestConfirmPopup, (ImGuiWindowFlags)64);
		if (!(!modalPopupDisposable))
		{
			ImGui.TextUnformatted(ImU8String.op_Implicit((pendingTargets.Length == 1) ? LanguageManager.Get("FCMemberManagePanel-ConfirmKick", GetMemberName(pendingTargets[0])) : LanguageManager.Get("FCMemberManagePanel-ConfirmKickMulti", pendingTargets.Length)));
			ImGui.Separator();
			ImGui.Spacing();
			if (ImGui.Button(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(1u)), new Vector2(120f * Globals.GlobalUIScale, 0f)))
			{
				EnqueueMemberAction(pendingTargets, AgentFreeCompany.MemberActionType.Dismiss, 0);
				pendingTargets = null;
				requestConfirmPopup = false;
			}
			ImGui.SameLine();
			if (ImGui.Button(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(2u)), new Vector2(120f * Globals.GlobalUIScale, 0f)))
			{
				pendingTargets = null;
				requestConfirmPopup = false;
			}
		}
	}

	private void OnFreeCompanyMemberAddon(AddonEvent type, AddonArgs? args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Invalid comparison between Unknown and I4
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Invalid comparison between Unknown and I4
		ResetMembers();
		Overlay overlay = base.Overlay;
		bool isOpen = (int)type == 1 || ((int)type != 6 && ((Window)base.Overlay).IsOpen);
		((Window)overlay).IsOpen = isOpen;
	}

	private unsafe void SyncMembers()
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Invalid comparison between Unknown and I8
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		AgentFreeCompany* ptr = AgentFreeCompany.Instance();
		if (ptr == null)
		{
			return;
		}
		InfoProxyFreeCompanyMember* infoProxyFreeCompanyMember = ptr->InfoProxyFreeCompanyMember;
		if (infoProxyFreeCompanyMember == null)
		{
			return;
		}
		currentPage = ptr->CurrentMemberPageIndex;
		InfoProxyFreeCompany* ptr2 = InfoProxyFreeCompany.Instance();
		if (ptr2 != null)
		{
			totalMemberCount = ptr2->TotalMembers;
		}
		ReadOnlySpan<CharacterData> charDataSpan = ((InfoProxyFreeCompanyMember)infoProxyFreeCompanyMember).CharDataSpan;
		if (charDataSpan.Length == 0)
		{
			members.Clear();
			return;
		}
		for (int i = 0; i < charDataSpan.Length; i++)
		{
			CharacterData val = charDataSpan.get_Item(i);
			if (!string.IsNullOrWhiteSpace(((CharacterData)(ref val)).NameString))
			{
				ulong contentId = val.ContentId;
				MemberRecord value;
				MemberRecord memberRecord = (members.TryGetValue(contentId, out value) ? value : new MemberRecord
				{
					ContentID = contentId
				});
				memberRecord.Index = i;
				memberRecord.OnlineStatus = GetOnlineStatusIconID(val.State);
				memberRecord.Name = ((CharacterData)(ref val)).NameString;
				memberRecord.RankText = ((ptr2 == null) ? string.Empty : ptr2->GetMemberRankNameText(val.ExtraFlags));
				memberRecord.RankIndex = (byte)(val.ExtraFlags >> 12);
				memberRecord.IsOnline = (val.State & 0x810000000000L) > 0L;
				object jobIcon;
				if (val.Job != 0)
				{
					ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
					GameIconLookup val2 = new GameIconLookup((uint)(62100 + val.Job), false, true, (ClientLanguage?)null);
					jobIcon = obj.GetFromGameIcon(ref val2);
				}
				else
				{
					jobIcon = null;
				}
				memberRecord.JobIcon = (ISharedImmediateTexture?)jobIcon;
				object jobText;
				if (val.Job != 0)
				{
					ClassJob rowOrDefault = LuminaGetter.GetRowOrDefault<ClassJob>((uint)val.Job);
					jobText = ((object)((ClassJob)(ref rowOrDefault)).Name/*cast due to constrained. prefix*/).ToString() ?? string.Empty;
				}
				else
				{
					jobText = string.Empty;
				}
				memberRecord.JobText = (string)jobText;
				string memberLocationText = ptr->GetMemberLocationText(i);
				if (memberRecord.LocationText != memberLocationText)
				{
					memberRecord.LocationText = memberLocationText;
					memberRecord.OfflineMinutes = (memberRecord.IsOnline ? new int?(0) : ParseOfflineMinutes(memberLocationText));
				}
				members[contentId] = memberRecord;
			}
		}
	}

	private void ResetMembers()
	{
		members.Clear();
		selectedContentIDs.Clear();
		pendingTargets = null;
		requestConfirmPopup = false;
	}

	private int GetPageCount()
	{
		return Math.Min(3, Math.Max(1, (int)(totalMemberCount + 199) / 200));
	}

	private List<MemberRecord> FilteredMembers()
	{
		IEnumerable<MemberRecord> source = members.Values.AsEnumerable();
		if (!string.IsNullOrWhiteSpace(nameFilter))
		{
			source = source.Where((MemberRecord x) => x.Name.Contains(nameFilter, StringComparison.OrdinalIgnoreCase));
		}
		if (rankFilter.Count > 0)
		{
			source = source.Where((MemberRecord x) => rankFilter.Contains(x.RankIndex));
		}
		if (lastOnlineFilter != LastOnlineRange.All)
		{
			source = source.Where(MatchesLastOnlineFilter);
		}
		List<MemberRecord> list = source.ToList();
		list.Sort((MemberRecord a, MemberRecord b) => (!isDescending) ? a.Index.CompareTo(b.Index) : b.Index.CompareTo(a.Index));
		return list;
	}

	private bool MatchesLastOnlineFilter(MemberRecord member)
	{
		return lastOnlineFilter switch
		{
			LastOnlineRange.Online => member.IsOnline, 
			LastOnlineRange.Unknown => member != null && !member.IsOnline && !member.OfflineMinutes.HasValue, 
			LastOnlineRange.WithinHour => IsWithinOfflineMinutes(member, 60), 
			LastOnlineRange.WithinDay => IsWithinOfflineMinutes(member, 1440), 
			LastOnlineRange.WithinWeek => IsWithinOfflineMinutes(member, 10080), 
			LastOnlineRange.WithinMonth => IsWithinOfflineMinutes(member, 43200), 
			LastOnlineRange.BeyondMonth => IsBeyondOfflineMinutes(member, 43200), 
			_ => true, 
		};
	}

	private static bool IsWithinOfflineMinutes(MemberRecord member, int limit)
	{
		if (!member.IsOnline)
		{
			return member.OfflineMinutes <= limit;
		}
		return true;
	}

	private static bool IsBeyondOfflineMinutes(MemberRecord member, int limit)
	{
		if (!member.IsOnline)
		{
			return member.OfflineMinutes > limit;
		}
		return false;
	}

	private unsafe void SwitchPage(int page)
	{
		AgentFreeCompany* ptr = AgentFreeCompany.Instance();
		if (ptr != null)
		{
			ptr->SwitchMemberPage(page);
			currentPage = page;
			members.Clear();
			selectedContentIDs.Clear();
		}
	}

	private unsafe void EnqueueMemberAction(ulong[] targets, AgentFreeCompany.MemberActionType action, byte rank)
	{
		base.TaskHelper?.Abort();
		foreach (ulong target in targets)
		{
			base.TaskHelper?.Enqueue(delegate
			{
				AgentFreeCompany* ptr = AgentFreeCompany.Instance();
				if (ptr == null)
				{
					return true;
				}
				ptr->ExecuteMemberAction(target, action, rank);
				return true;
			});
			base.TaskHelper?.DelayNext(300);
		}
	}

	private void RequestConfirm(ulong[] targets)
	{
		if (targets.Length != 0)
		{
			pendingTargets = targets;
			requestConfirmPopup = true;
		}
	}

	private unsafe static void OpenCharaCard(ulong contentID)
	{
		AgentCharaCard* ptr = AgentCharaCard.Instance();
		if (ptr != null)
		{
			((AgentCharaCard)ptr).OpenCharaCard(contentID);
		}
	}

	private unsafe static void OpenCharacterDetail(ulong contentID)
	{
		AgentFreeCompany* ptr = AgentFreeCompany.Instance();
		if (ptr == null)
		{
			return;
		}
		InfoProxyFreeCompanyMember* infoProxyFreeCompanyMember = ptr->InfoProxyFreeCompanyMember;
		if (infoProxyFreeCompanyMember == null)
		{
			return;
		}
		CharacterData* entryByContentId = ((InfoProxyFreeCompanyMember)infoProxyFreeCompanyMember).GetEntryByContentId(contentID, 0u, (byte)0);
		if (entryByContentId != null)
		{
			AgentDetail* ptr2 = AgentDetail.Instance();
			if (ptr2 != null)
			{
				((AgentDetail)ptr2).OpenForCharacterData(entryByContentId, (DetailUpdateData*)null, 0);
			}
		}
	}

	private unsafe static void OpenFreeCompanyProfile(ulong contentID)
	{
		AgentFreeCompanyProfile* ptr = AgentFreeCompanyProfile.Instance();
		if (ptr != null)
		{
			ptr->ShowProfile(contentID);
		}
	}

	private string GetMemberName(ulong contentID)
	{
		if (!members.TryGetValue(contentID, out MemberRecord value))
		{
			return contentID.ToString();
		}
		return value.Name;
	}

	private static uint GetOnlineStatusIconID(OnlineStatus status)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Expected I8, but got Unknown
		if ((int)status == 0)
		{
			return 10u;
		}
		ulong num = (ulong)(long)(status & (~status + 1));
		uint num2 = 0u;
		while (num > 1)
		{
			num >>= 1;
			num2++;
		}
		return num2;
	}

	private static int? ParseOfflineMinutes(string locationText)
	{
		if (string.IsNullOrWhiteSpace(locationText))
		{
			return null;
		}
		foreach (var offlineTimePattern in GetOfflineTimePatterns())
		{
			Regex item = offlineTimePattern.Pattern;
			int item2 = offlineTimePattern.UnitMinutes;
			Match match = item.Match(locationText);
			if (match.Success)
			{
				if (match.Groups.Count < 2)
				{
					return item2;
				}
				int result;
				return int.TryParse(match.Groups[1].Value, out result) ? new int?(result * item2) : ((int?)null);
			}
		}
		return null;
	}

	private static List<(Regex Pattern, int UnitMinutes)> GetOfflineTimePatterns()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (OfflineTimePatterns != null)
		{
			return OfflineTimePatterns;
		}
		List<(Regex, int)> list = new List<(Regex, int)>();
		(uint, int)[] offlineTimeAddons = OfflineTimeAddons;
		for (int i = 0; i < offlineTimeAddons.Length; i++)
		{
			(uint, int) tuple = offlineTimeAddons[i];
			uint item = tuple.Item1;
			int item2 = tuple.Item2;
			ReadOnlySeString addonTextSeString = LuminaWrapper.GetAddonTextSeString(item);
			string text = ((ReadOnlySeString)(ref addonTextSeString)).ExtractText(false, "@@NUM@@");
			if (!string.IsNullOrWhiteSpace(text))
			{
				string text2 = Regex.Escape(text).Replace("@@NUM@@", "([0-9]+)");
				list.Add((new Regex("^" + text2 + "$", RegexOptions.Compiled), item2));
			}
		}
		return OfflineTimePatterns = list;
	}
}
