using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Interface.Windows;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Internal;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Component.GUI;
using InteropGenerator.Runtime;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.Helpers;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;

namespace DailyRoutines.ModulesPublic;

public class FastContentsFinderRegister : ModuleBase
{
	public class ContentFinderItemData
	{
		public uint NodeID { get; init; }

		public string Name { get; init; } = string.Empty;

		public string Level { get; init; } = string.Empty;

		public Vector2 Position { get; init; }

		public float Height { get; init; }

		public bool IsLocked { get; init; }

		public bool IsVisible { get; set; }

		public string CleanName { get; init; } = string.Empty;
	}

	public class ContentFinderCacheData
	{
		public uint CurrentTab { get; set; }

		public List<ContentFinderItemData> Items { get; set; } = new List<ContentFinderItemData>();

		public bool InDutyQueue { get; set; }

		public DateTime LastUpdateTime { get; set; } = DateTime.MinValue;
	}

	private sealed class ContentFinderDataManager
	{
		private ContentFinderCacheData? cachedData;

		public ContentFinderCacheData? GetCachedData()
		{
			if (cachedData != null && OmenServiceBase<StandardTimeManager>.Instance().Now - cachedData.LastUpdateTime > TimeSpan.FromSeconds(5L))
			{
				cachedData = null;
			}
			return cachedData;
		}

		public unsafe void UpdateCacheData()
		{
			if (Addons.ContentsFinder == null || ((AtkUnitBase)Addons.ContentsFinder).AtkValues == null || ((AtkValue)((byte*)((AtkUnitBase)Addons.ContentsFinder).AtkValues + Unsafe.SizeOf<AtkValue>())).Bool || ((AtkValue)((byte*)((AtkUnitBase)Addons.ContentsFinder).AtkValues + (nint)26 * (nint)Unsafe.SizeOf<AtkValue>())).UInt > 10)
			{
				return;
			}
			try
			{
				ContentFinderCacheData contentFinderCacheData = new ContentFinderCacheData
				{
					CurrentTab = ((AtkValue)((byte*)((AtkUnitBase)Addons.ContentsFinder).AtkValues + (nint)26 * (nint)Unsafe.SizeOf<AtkValue>())).UInt,
					InDutyQueue = DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)91],
					LastUpdateTime = OmenServiceBase<StandardTimeManager>.Instance().Now
				};
				AtkComponentNode* nodeById = (AtkComponentNode*)((AtkUnitBase)Addons.ContentsFinder).GetNodeById(52u);
				if (nodeById == null)
				{
					return;
				}
				AtkComponentTreeList* component = (AtkComponentTreeList*)((AtkComponentNode)nodeById).Component;
				if (component == null)
				{
					return;
				}
				AtkTextNode* nodeById2 = (AtkTextNode*)((AtkUnitBase)Addons.ContentsFinder).GetNodeById(57u);
				if (nodeById2 == null)
				{
					return;
				}
				int listLength = ((AtkComponentTreeList)component).ListLength;
				if (listLength == 0)
				{
					return;
				}
				List<ContentFinderItemData> list = new List<ContentFinderItemData>();
				for (int i = 0; (float)i < MathF.Min(listLength, 16f); i++)
				{
					int num = 3 + i;
					if (num >= ((AtkUldManager)(&((AtkComponentBase)((AtkComponentNode)nodeById).Component).UldManager)).NodeListCount)
					{
						break;
					}
					AtkComponentNode* ptr = (AtkComponentNode*)((AtkUldManager)(&((AtkComponentBase)((AtkComponentNode)nodeById).Component).UldManager)).NodeList[num];
					if (ptr == null || ((AtkComponentNode)ptr).Y >= 300f || ((AtkComponentNode)ptr).ScreenY < ((AtkComponentNode)nodeById).ScreenY || ((AtkComponentNode)ptr).ScreenY + 20f > ((AtkTextNode)nodeById2).ScreenY)
					{
						continue;
					}
					AtkTextNode* ptr2 = (AtkTextNode*)((AtkUldManager)(&((AtkComponentBase)((AtkComponentNode)ptr).Component).UldManager)).SearchNodeById(6u);
					if (ptr2 == null)
					{
						continue;
					}
					string text = (((CStringPointer)(&((Utf8String)(&((AtkTextNode)ptr2).NodeText)).StringPtr)).HasValue ? ((object)(*(Utf8String*)(&((AtkTextNode)ptr2).NodeText))/*cast due to constrained. prefix*/).ToString() : string.Empty);
					if (string.IsNullOrWhiteSpace(text))
					{
						continue;
					}
					AtkImageNode* ptr3 = (AtkImageNode*)((AtkUldManager)(&((AtkComponentBase)((AtkComponentNode)ptr).Component).UldManager)).SearchNodeById(3u);
					if (ptr3 == null)
					{
						continue;
					}
					AtkImageNode* ptr4 = (AtkImageNode*)((AtkUldManager)(&((AtkComponentBase)((AtkComponentNode)ptr).Component).UldManager)).SearchNodeById(4u);
					if (ptr4 == null)
					{
						continue;
					}
					AtkTextNode* ptr5 = (AtkTextNode*)((AtkUldManager)(&((AtkComponentBase)((AtkComponentNode)ptr).Component).UldManager)).SearchNodeById(19u);
					if (ptr5 != null)
					{
						string text2 = (((CStringPointer)(&((Utf8String)(&((AtkTextNode)ptr5).NodeText)).StringPtr)).HasValue ? ((object)(*(Utf8String*)(&((AtkTextNode)ptr5).NodeText))/*cast due to constrained. prefix*/).ToString() : string.Empty);
						if (!string.IsNullOrWhiteSpace(text2) && ((AtkUldManager)(&((AtkComponentBase)((AtkComponentNode)ptr).Component).UldManager)).SearchNodeById(14u) != null)
						{
							NodeState nodeState = ((AtkTextNode)ptr5).AtkResNode.GetNodeState();
							ContentFinderItemData item = new ContentFinderItemData
							{
								NodeID = ((AtkComponentNode)ptr).NodeId,
								Name = text,
								Level = text2,
								Position = nodeState.TopLeft - new Vector2(24f, 0f),
								Height = nodeState.Height / Globals.GlobalUIScale,
								IsLocked = (((AtkImageNode)ptr3).IsVisible() || ((AtkImageNode)ptr4).IsVisible()),
								IsVisible = ((AtkTextNode)ptr5).IsVisible(),
								CleanName = text.Replace(" ", string.Empty)
							};
							list.Add(item);
						}
					}
				}
				contentFinderCacheData.Items = list;
				cachedData = contentFinderCacheData;
			}
			catch
			{
			}
		}

		public void ClearCache()
		{
			cachedData = null;
		}
	}

	private readonly ContentFinderDataManager manager = new ContentFinderDataManager();

	private const ImGuiWindowFlags WINDOW_FLAGS = (ImGuiWindowFlags)2888175;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("FastContentsFinderRegisterTitle"),
		Description = LanguageManager.Get("FastContentsFinderRegisterDescription"),
		Category = ModuleCategory.Recruitment,
		ModulesPrerequisite = new string[1] { "ContentFinderCommand" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected unsafe override void Init()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Expected O, but got Unknown
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Expected O, but got Unknown
		if (base.Overlay == null)
		{
			Overlay overlay = (base.Overlay = new Overlay(this));
		}
		Overlay? overlay3 = base.Overlay;
		((Window)overlay3).Flags = (ImGuiWindowFlags)(((Window)overlay3).Flags | 0x80);
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)1, "ContentsFinder", new AddonEventDelegate(OnAddon));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, "ContentsFinder", new AddonEventDelegate(OnAddon));
		if (Addons.ContentsFinder != null)
		{
			OnAddon((AddonEvent)1, null);
		}
	}

	protected override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddon });
		manager.ClearCache();
	}

	protected unsafe override void OverlayUI()
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		if (Addons.ContentsFinder == null)
		{
			((Window)base.Overlay).IsOpen = false;
		}
		else
		{
			if (!(*Addons.ContentsFinder).IsAddonAndNodesReady() || ((AtkValue)((byte*)((AtkUnitBase)Addons.ContentsFinder).AtkValues + Unsafe.SizeOf<AtkValue>())).Bool)
			{
				return;
			}
			if (Throttler.Shared.Throttle("UpdateContentFinderData", 100u))
			{
				manager.UpdateCacheData();
			}
			ContentFinderCacheData cachedData = manager.GetCachedData();
			if (cachedData == null || cachedData.Items.Count == 0)
			{
				return;
			}
			AdjustOriginalNodes();
			ImGuiStylePtr style = ImGui.GetStyle();
			Vector2 itemSpacing = ((ImGuiStylePtr)(ref style)).ItemSpacing;
			ISharedImmediateTexture val3 = default(ISharedImmediateTexture);
			ISharedImmediateTexture val6 = default(ISharedImmediateTexture);
			ISharedImmediateTexture val7 = default(ISharedImmediateTexture);
			ISharedImmediateTexture val8 = default(ISharedImmediateTexture);
			foreach (ContentFinderItemData item in cachedData.Items)
			{
				Vector2 vector = ((cachedData.CurrentTab == 0) ? (item.Position + new Vector2(20f, 0f)) : item.Position);
				ImGui.SetNextWindowPos(vector - itemSpacing);
				ImU8String val = new ImU8String(34, 1);
				((ImU8String)(ref val)).AppendLiteral("FastContentsFinderRouletteOverlay-");
				((ImU8String)(ref val)).AppendFormatted<uint>(item.NodeID);
				if (!ImGui.Begin(val, (ImGuiWindowFlags)2888175))
				{
					continue;
				}
				if (cachedData.InDutyQueue)
				{
					ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
					GameIconLookup val2 = new GameIconLookup(61502u, false, true, (ClientLanguage?)null);
					if (obj.TryGetFromGameIcon(ref val2, ref val3))
					{
						ImGui.SetCursorScreenPos(vector);
						if (ImGui.ImageButton(val3.GetWrapOrEmpty().Handle, new Vector2(item.Height)))
						{
							ContentsFinderHelper.CancelDutyApply();
						}
						ImGuiOm.TooltipHover(LanguageManager.Get("Cancel") ?? "");
					}
				}
				else
				{
					string text = item.Level + " " + item.Name;
					GroupDisposable val4 = ImRaii.Group();
					try
					{
						DisabledDisposable val5 = ImRaii.Disabled(item.IsLocked);
						try
						{
							ITextureProvider obj2 = DalamudServiceExtension.Instance<ITextureProvider>();
							GameIconLookup val2 = new GameIconLookup(60081u, false, true, (ClientLanguage?)null);
							if (obj2.TryGetFromGameIcon(ref val2, ref val6))
							{
								ImGui.SetCursorScreenPos(vector);
								if (ImGui.ImageButton(val6.GetWrapOrEmpty().Handle, new Vector2(item.Height)))
								{
									OmenServiceBase<ChatManager>.Instance().SendMessage("/pdrduty " + ((cachedData.CurrentTab == 0) ? "r" : "n") + " " + item.CleanName);
									OmenServiceBase<ChatManager>.Instance().SendMessage("/pdrduty " + ((cachedData.CurrentTab != 0) ? "r" : "n") + " " + item.CleanName);
								}
								ImGuiOm.TooltipHover(text ?? "");
							}
							if (cachedData.CurrentTab != 0)
							{
								if (PluginConfig.Instance().ConflictKeyBinding.IsPressed())
								{
									ITextureProvider obj3 = DalamudServiceExtension.Instance<ITextureProvider>();
									val2 = new GameIconLookup(60648u, false, true, (ClientLanguage?)null);
									if (obj3.TryGetFromGameIcon(ref val2, ref val7))
									{
										ImGui.SameLine();
										if (ImGui.ImageButton(val7.GetWrapOrEmpty().Handle, new Vector2(item.Height)))
										{
											OmenServiceBase<ChatManager>.Instance().SendMessage("/pdrduty n " + item.CleanName + " explorer");
										}
										ImGuiOm.TooltipHover(text + " (" + LuminaWrapper.GetAddonText(13038u) + ")");
									}
								}
								else
								{
									ITextureProvider obj4 = DalamudServiceExtension.Instance<ITextureProvider>();
									val2 = new GameIconLookup(60641u, false, true, (ClientLanguage?)null);
									if (obj4.TryGetFromGameIcon(ref val2, ref val8))
									{
										ImGui.SameLine();
										if (ImGui.ImageButton(val8.GetWrapOrEmpty().Handle, new Vector2(item.Height)))
										{
											OmenServiceBase<ChatManager>.Instance().SendMessage("/pdrduty n " + item.CleanName + " unrest");
										}
										ImGuiOm.TooltipHover($"{text} ({LuminaWrapper.GetAddonText(10008u)})\n[{LanguageManager.Get("FastContentsFinderRegister-HoldConflictKeyToToggle")}]");
									}
								}
							}
						}
						finally
						{
							((IDisposable)val5)?.Dispose();
						}
					}
					finally
					{
						((GroupDisposable)(ref val4)).Dispose();
					}
				}
				ImGui.End();
			}
		}
	}

	private unsafe static void AdjustOriginalNodes()
	{
		if (Addons.ContentsFinder == null)
		{
			return;
		}
		try
		{
			AtkComponentNode* nodeById = (AtkComponentNode*)((AtkUnitBase)Addons.ContentsFinder).GetNodeById(52u);
			if (nodeById == null)
			{
				return;
			}
			AtkComponentTreeList* component = (AtkComponentTreeList*)((AtkComponentNode)nodeById).Component;
			if (component == null)
			{
				return;
			}
			int listLength = ((AtkComponentTreeList)component).ListLength;
			if (listLength == 0)
			{
				return;
			}
			for (int i = 0; (float)i < MathF.Min(listLength, 45f); i++)
			{
				int num = 3 + i;
				if (num >= ((AtkUldManager)(&((AtkComponentBase)((AtkComponentNode)nodeById).Component).UldManager)).NodeListCount)
				{
					break;
				}
				AtkComponentNode* ptr = (AtkComponentNode*)((AtkUldManager)(&((AtkComponentBase)((AtkComponentNode)nodeById).Component).UldManager)).NodeList[num];
				if (ptr == null)
				{
					continue;
				}
				AtkTextNode* ptr2 = (AtkTextNode*)((AtkUldManager)(&((AtkComponentBase)((AtkComponentNode)ptr).Component).UldManager)).SearchNodeById(19u);
				if (ptr2 != null)
				{
					if (((AtkTextNode)ptr2).IsVisible())
					{
						((AtkTextNode)ptr2).ToggleVisibility(false);
					}
					AtkResNode* ptr3 = ((AtkUldManager)(&((AtkComponentBase)((AtkComponentNode)ptr).Component).UldManager)).SearchNodeById(14u);
					if (ptr3 == null)
					{
						break;
					}
					((AtkResNode)ptr3).SetPositionFloat(322f, 1f);
				}
			}
		}
		catch
		{
		}
	}

	private void OnAddon(AddonEvent type, AddonArgs? args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Invalid comparison between Unknown and I4
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Invalid comparison between Unknown and I4
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Invalid comparison between Unknown and I4
		Overlay overlay = base.Overlay;
		bool isOpen = (int)type == 1 || ((int)type != 6 && ((Window)base.Overlay).IsOpen);
		((Window)overlay).IsOpen = isOpen;
		if ((int)type != 1)
		{
			if ((int)type == 6)
			{
				manager.ClearCache();
			}
		}
		else
		{
			manager.UpdateCacheData();
		}
	}
}
