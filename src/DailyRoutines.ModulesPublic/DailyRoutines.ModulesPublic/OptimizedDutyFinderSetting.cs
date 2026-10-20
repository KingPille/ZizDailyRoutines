using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Plugin.Services;
using Dalamud.Utility.Numerics;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using KamiToolKit.Timelines;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Helpers;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;

namespace DailyRoutines.ModulesPublic;

public class OptimizedDutyFinderSetting : ModuleBase
{
	private unsafe delegate void SetContentsFinderSettingsInitDelegate(byte* data, UIModule* module);

	private enum DutyFinderSetting
	{
		Ja,
		En,
		De,
		Fr,
		LootRule,
		JoinPartyInProgress,
		UnrestrictedParty,
		LevelSync,
		MinimumIl,
		SilenceEcho,
		ExplorerMode,
		LimitedLevelingRoulette
	}

	private record DutyFinderSettingDisplay(DutyFinderSetting Setting)
	{
		public Func<uint>? GetIcon { get; init; }

		public Func<uint>? GetTooltip { get; init; }

		public DutyFinderSettingDisplay(DutyFinderSetting setting, uint icon, uint tooltip)
			: this(setting)
		{
			GetIcon = () => icon;
			GetTooltip = () => tooltip;
		}
	}

	private SetContentsFinderSettingsInitDelegate SetContentsFinderSettingsInit;

	private HorizontalListNode? layoutNode;

	private readonly Dictionary<DutyFinderSettingDisplay, (IconButtonNode? ButtonNode, IconImageNode? ImageNode)> nodes = new Dictionary<DutyFinderSettingDisplay, (IconButtonNode, IconImageNode)>();

	private static readonly DutyFinderSettingDisplay[] DutyFinderSettingIcons = new DutyFinderSettingDisplay[8]
	{
		new DutyFinderSettingDisplay(DutyFinderSetting.JoinPartyInProgress, 60644u, 2519u),
		new DutyFinderSettingDisplay(DutyFinderSetting.UnrestrictedParty, 60641u, 10008u),
		new DutyFinderSettingDisplay(DutyFinderSetting.LevelSync, 60649u, 12696u),
		new DutyFinderSettingDisplay(DutyFinderSetting.MinimumIl, 60642u, 10010u),
		new DutyFinderSettingDisplay(DutyFinderSetting.SilenceEcho, 60647u, 12691u),
		new DutyFinderSettingDisplay(DutyFinderSetting.ExplorerMode, 60648u, 13038u),
		new DutyFinderSettingDisplay(DutyFinderSetting.LimitedLevelingRoulette, 60640u, 13030u),
		new DutyFinderSettingDisplay(DutyFinderSetting.LootRule)
		{
			GetIcon = () => GetCurrentSettingValue(DutyFinderSetting.LootRule) switch
			{
				0 => 60645u, 
				1 => 60645u, 
				2 => 60646u, 
				_ => 0u, 
			},
			GetTooltip = () => GetCurrentSettingValue(DutyFinderSetting.LootRule) switch
			{
				0 => 10022u, 
				1 => 10023u, 
				2 => 10024u, 
				_ => 0u, 
			}
		}
	};

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("OptimizedDutyFinderSettingTitle"),
		Description = LanguageManager.Get("OptimizedDutyFinderSettingDescription"),
		Category = ModuleCategory.Recruitment,
		Author = new string[2] { "Mizami", "Cyf5119" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected override void Init()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Expected O, but got Unknown
		SetContentsFinderSettingsInit = new CompSig("E8 ?? ?? ?? ?? 49 8B 06 45 33 FF 49 8B CE 45 89 7E 20 FF 50 28 B0 01").GetDelegate<SetContentsFinderSettingsInitDelegate>();
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)5, (IEnumerable<string>)new global::_003C_003Ez__ReadOnlyArray<string>(new string[2] { "ContentsFinder", "RaidFinder" }), new AddonEventDelegate(OnAddon));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)10, (IEnumerable<string>)new global::_003C_003Ez__ReadOnlyArray<string>(new string[2] { "ContentsFinder", "RaidFinder" }), new AddonEventDelegate(OnAddon));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, (IEnumerable<string>)new global::_003C_003Ez__ReadOnlyArray<string>(new string[2] { "ContentsFinder", "RaidFinder" }), new AddonEventDelegate(OnAddon));
	}

	protected override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddon });
		foreach (var (iconButtonNode, iconImageNode) in nodes.Values)
		{
			iconButtonNode?.Dispose();
			iconImageNode?.Dispose();
		}
		nodes.Clear();
		layoutNode?.Dispose();
		layoutNode = null;
	}

	private unsafe void OnAddon(AddonEvent type, AddonArgs args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		//IL_06a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_056a: Unknown result type (might be due to invalid IL or missing references)
		if ((int)type != 5)
		{
			if ((int)type == 6)
			{
				nodes.Clear();
				layoutNode = null;
				return;
			}
			if ((int)type != 10)
			{
				return;
			}
		}
		AtkUnitBase* ptr = args.Addon.ToStruct();
		if (ptr == null)
		{
			return;
		}
		if (layoutNode == null)
		{
			AtkResNode* nodeById = ((AtkUnitBase)ptr).GetNodeById(6u);
			if (nodeById == null)
			{
				return;
			}
			((AtkResNode)nodeById).ToggleVisibility(false);
			AtkResNode* nodeById2 = ((AtkUnitBase)ptr).GetNodeById(4u);
			if (nodeById2 == null)
			{
				return;
			}
			layoutNode = new HorizontalListNode
			{
				IsVisible = true,
				Size = new Vector2((int)((AtkResNode)nodeById).Width, (int)((AtkResNode)nodeById).Height),
				Position = new Vector2(((AtkResNode)nodeById).X - 5f, ((AtkResNode)nodeById).Y),
				ItemSpacing = 0f
			};
			layoutNode.AttachNode(nodeById2);
			DutyFinderSettingDisplay[] dutyFinderSettingIcons = DutyFinderSettingIcons;
			foreach (DutyFinderSettingDisplay settingDetail in dutyFinderSettingIcons)
			{
				if (!nodes.ContainsKey(settingDetail))
				{
					IconButtonNode button = new IconButtonNode
					{
						IsVisible = true,
						Size = new Vector2(32f),
						Position = new Vector2(0f, -5f),
						TextTooltip = ReadOnlySeString.op_Implicit(LuminaWrapper.GetAddonText(settingDetail.GetTooltip()))
					};
					button.OnClick = delegate
					{
						//IL_0031: Unknown result type (might be due to invalid IL or missing references)
						ToggleSetting(settingDetail.Setting);
						button.TextTooltip = ReadOnlySeString.op_Implicit(LuminaWrapper.GetAddonText(settingDetail.GetTooltip()));
						button.HideTooltip();
						button.ShowTooltip();
					};
					button.BackgroundNode.IsVisible = false;
					button.ImageNode.IsVisible = false;
					Vector2 vector = new Vector2(4f, 5f);
					IconImageNode iconImageNode = new IconImageNode
					{
						IconId = settingDetail.GetIcon(),
						Size = new Vector2(24f),
						IsVisible = true,
						Position = vector,
						FitTexture = true
					};
					iconImageNode.AddTimeline(new TimelineBuilder().AddFrameSetWithFrame(1, 10, 1, vector).AddFrameSetWithFrame(11, 17, 11, vector).AddFrameSetWithFrame(18, 26, 18, vector + new Vector2(0f, 1f))
						.AddFrameSetWithFrame(27, 36, 27, vector)
						.AddFrameSetWithFrame(37, 46, 37, vector)
						.AddFrameSetWithFrame(47, 53, 47, vector)
						.Build());
					iconImageNode.AttachNode((NodeBase?)button, NodePosition.AsLastChild);
					nodes[settingDetail] = (button, iconImageNode);
					layoutNode.AddNode(button);
				}
			}
			List<DutyFinderSettingDisplay> languageButtons = GetLanguageButtons();
			if (languageButtons != null && languageButtons.Count > 0)
			{
				for (int num = 0; num < languageButtons.Count; num++)
				{
					AtkResNode* nodeById3 = ((AtkUnitBase)ptr).GetNodeById((uint)(17 + num));
					if (nodeById3 == null)
					{
						continue;
					}
					AtkResNode* parentNode = ((AtkResNode)nodeById3).ParentNode;
					if (parentNode == null)
					{
						continue;
					}
					DutyFinderSettingDisplay langSetting = languageButtons[num];
					IconButtonNode iconButtonNode = new IconButtonNode
					{
						IsVisible = true,
						Size = new Vector2(28f),
						Position = new Vector2(((AtkResNode)nodeById3).X - 7f, ((AtkResNode)nodeById3).Y - 5f),
						TextTooltip = ReadOnlySeString.op_Implicit(LuminaWrapper.GetAddonText((uint)(4266 + num))),
						OnClick = delegate
						{
							if (IsLangConfigReady())
							{
								ToggleSetting(langSetting.Setting);
							}
						}
					};
					iconButtonNode.BackgroundNode.IsVisible = false;
					iconButtonNode.ImageNode.IsVisible = false;
					iconButtonNode.AttachNode(parentNode);
					nodes[langSetting] = (iconButtonNode, null);
				}
			}
		}
		foreach (KeyValuePair<DutyFinderSettingDisplay, (IconButtonNode, IconImageNode)> node in nodes)
		{
			node.Deconstruct(out var key, out var value);
			(IconButtonNode, IconImageNode) tuple = value;
			DutyFinderSettingDisplay dutyFinderSettingDisplay = key;
			IconButtonNode item = tuple.Item1;
			IconImageNode item2 = tuple.Item2;
			byte currentSettingValue = GetCurrentSettingValue(dutyFinderSettingDisplay.Setting);
			if (item2 != null)
			{
				item2.IconId = dutyFinderSettingDisplay.GetIcon();
				if (dutyFinderSettingDisplay.Setting == DutyFinderSetting.LevelSync && GetCurrentSettingValue(DutyFinderSetting.UnrestrictedParty) == 0)
				{
					item2.Color = VectorExtensions.WithW(item.Color, (currentSettingValue != 0) ? 1f : 0.25f);
				}
				else
				{
					item2.Color = VectorExtensions.WithW(item.Color, (currentSettingValue != 0) ? 1f : 0.5f);
				}
			}
			item.TextTooltip = ReadOnlySeString.op_Implicit(LuminaWrapper.GetAddonText(dutyFinderSettingDisplay.GetTooltip()));
		}
	}

	private static byte GetCurrentSettingValue(DutyFinderSetting dutyFinderSetting)
	{
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Expected I4, but got Unknown
		ContentsFinderOption contentsFinderOption = ContentsFinderOption.Get();
		return dutyFinderSetting switch
		{
			DutyFinderSetting.Ja => (byte)(IsLangConfigReady() ? ((byte)DalamudServiceExtension.Instance<IGameConfig>().UiConfig.GetUInt("ContentsFinderUseLangTypeJA")) : 0), 
			DutyFinderSetting.En => (byte)(IsLangConfigReady() ? ((byte)DalamudServiceExtension.Instance<IGameConfig>().UiConfig.GetUInt("ContentsFinderUseLangTypeEN")) : 0), 
			DutyFinderSetting.De => (byte)(IsLangConfigReady() ? ((byte)DalamudServiceExtension.Instance<IGameConfig>().UiConfig.GetUInt("ContentsFinderUseLangTypeDE")) : 0), 
			DutyFinderSetting.Fr => (byte)(IsLangConfigReady() ? ((byte)DalamudServiceExtension.Instance<IGameConfig>().UiConfig.GetUInt("ContentsFinderUseLangTypeFR")) : 0), 
			DutyFinderSetting.JoinPartyInProgress => (byte)DalamudServiceExtension.Instance<IGameConfig>().UiConfig.GetUInt("ContentsFinderSupplyEnable"), 
			DutyFinderSetting.LootRule => (byte)(int)contentsFinderOption.LootRules, 
			DutyFinderSetting.UnrestrictedParty => contentsFinderOption.UnrestrictedParty ? ((byte)1) : ((byte)0), 
			DutyFinderSetting.LevelSync => contentsFinderOption.LevelSync ? ((byte)1) : ((byte)0), 
			DutyFinderSetting.MinimumIl => contentsFinderOption.MinimalIL ? ((byte)1) : ((byte)0), 
			DutyFinderSetting.SilenceEcho => contentsFinderOption.SilenceEcho ? ((byte)1) : ((byte)0), 
			DutyFinderSetting.ExplorerMode => contentsFinderOption.ExplorerMode ? ((byte)1) : ((byte)0), 
			DutyFinderSetting.LimitedLevelingRoulette => contentsFinderOption.IsLimitedLevelingRoulette ? ((byte)1) : ((byte)0), 
			_ => 0, 
		};
	}

	private unsafe void ToggleSetting(DutyFinderSetting setting)
	{
		bool flag = IsLangConfigReady();
		if (flag)
		{
			bool flag2 = (uint)setting <= 3u;
			flag = flag2;
		}
		if (!flag || GetCurrentSettingValue(DutyFinderSetting.Ja) + GetCurrentSettingValue(DutyFinderSetting.En) + GetCurrentSettingValue(DutyFinderSetting.De) + GetCurrentSettingValue(DutyFinderSetting.Fr) != 1 || GetCurrentSettingValue(setting) != 1)
		{
			byte[] currentSettingArray = GetCurrentSettingArray();
			byte b = ((setting != DutyFinderSetting.LootRule) ? ((currentSettingArray[(int)setting] == 0) ? ((byte)1) : ((byte)0)) : ((byte)((currentSettingArray[(int)setting] + 1) % 3)));
			currentSettingArray[(int)setting] = b;
			fixed (byte* data = currentSettingArray)
			{
				SetContentsFinderSettingsInit(data, UIModule.Instance());
			}
		}
	}

	private static byte[] GetCurrentSettingArray()
	{
		byte[] array = new byte[27];
		int num = Enum.GetValues<DutyFinderSetting>().Length;
		for (int i = 0; i < num; i++)
		{
			array[i] = GetCurrentSettingValue((DutyFinderSetting)i);
			array[i + num] = GetCurrentSettingValue((DutyFinderSetting)i);
		}
		array[26] = 1;
		return array;
	}

	private static List<DutyFinderSettingDisplay> GetLanguageButtons()
	{
		if (IsLangConfigReady())
		{
			int num = 4;
			List<DutyFinderSettingDisplay> list = new List<DutyFinderSettingDisplay>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<DutyFinderSettingDisplay> span = CollectionsMarshal.AsSpan(list);
			span[0] = new DutyFinderSettingDisplay(DutyFinderSetting.Ja, 0u, 10u);
			span[1] = new DutyFinderSettingDisplay(DutyFinderSetting.En, 0u, 11u);
			span[2] = new DutyFinderSettingDisplay(DutyFinderSetting.De, 0u, 12u);
			span[3] = new DutyFinderSettingDisplay(DutyFinderSetting.Fr, 0u, 13u);
			return list;
		}
		return new List<DutyFinderSettingDisplay>();
	}

	private static bool IsLangConfigReady()
	{
		try
		{
			uint num = default(uint);
			if (DalamudServiceExtension.Instance<IGameConfig>().UiConfig.TryGet("ContentsFinderUseLangTypeJA", ref num) && DalamudServiceExtension.Instance<IGameConfig>().UiConfig.TryGet("ContentsFinderUseLangTypeEN", ref num) && DalamudServiceExtension.Instance<IGameConfig>().UiConfig.TryGet("ContentsFinderUseLangTypeDE", ref num) && DalamudServiceExtension.Instance<IGameConfig>().UiConfig.TryGet("ContentsFinderUseLangTypeFR", ref num))
			{
				return true;
			}
		}
		catch
		{
		}
		return false;
	}
}
