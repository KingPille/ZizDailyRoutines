using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.Command;
using Dalamud.Game.Gui.Toast;
using Dalamud.Hooking;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Graphics.Environment;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using KamiToolKit.Nodes.Simplified;
using Lumina.Data;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Info.Game;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic.Interface;

public class FastSetWeatherTime : ModuleBase
{
	private unsafe delegate void* PlayWeatherSoundDelegate(void* manager, byte weatherID, void* a3, void* a4);

	private unsafe delegate void* UpdateBgmSituationDelegate(void* manager, ushort bgmSituationID, int column, void* a4, void* a5);

	private class Config : ModuleConfig
	{
		public Dictionary<uint, ZoneSetting> ZoneSettings = new Dictionary<uint, ZoneSetting>();
	}

	private class AddonDRFastSetWeather(FastSetWeatherTime module) : NativeAddon
	{
		private TextButtonNode? clearButtonNode;

		private NumericInputNode? hourInputNode;

		private NumericInputNode? minuteInputNode;

		private TextButtonNode? saveButtonNode;

		private NumericInputNode? secondInputNode;

		private SliderNode? timeNode;

		private Dictionary<byte, (IconButtonNode IconButton, SimpleNineGridNode EnabledIcon)> weatherButtons = new Dictionary<byte, (IconButtonNode, SimpleNineGridNode)>();

		public static AddonDRFastSetWeather? Addon { get; set; }

		protected unsafe override void OnSetup(AtkUnitBase* addon, Span<AtkValue> atkValues)
		{
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0625: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			weatherButtons.Clear();
			VerticalListNode verticalListNode = new VerticalListNode
			{
				IsVisible = true,
				Position = base.ContentStartPosition
			};
			float num = 125f;
			List<byte> list = ParseLVB((ushort)GameState.TerritoryType).WeatherList.Where((byte rowID) => LuminaGetter.TryGetRow<Weather>((uint)rowID, out Weather item) && !string.IsNullOrEmpty(((object)((Weather)(ref item)).Name/*cast due to constrained. prefix*/).ToString()) && ImageHelper.TryGetGameIcon((uint)((Weather)(ref item)).Icon, out IDalamudTextureWrap _)).ToList();
			if (list != null && list.Count > 0)
			{
				num += (float)(list.Count / 4) * 54f + (float)((list.Count / 4 - 1) * 5);
				SetWindowSize(base.Size.X, num);
				HorizontalFlexNode horizontalFlexNode = new HorizontalFlexNode
				{
					IsVisible = true,
					Size = new Vector2(0f, 54f)
				};
				int num2 = 0;
				foreach (byte weather in list)
				{
					Weather rowOrDefault = LuminaGetter.GetRowOrDefault<Weather>((uint)weather);
					if (num2 >= 4)
					{
						verticalListNode.Height += horizontalFlexNode.Height;
						verticalListNode.AddNode(horizontalFlexNode);
						verticalListNode.AddDummy(5f);
						horizontalFlexNode = new HorizontalFlexNode
						{
							IsVisible = true,
							Size = new Vector2(0f, 54f)
						};
						num2 = 0;
					}
					IconButtonNode iconButtonNode = new IconButtonNode
					{
						Size = new Vector2(54f),
						IsVisible = true,
						IsEnabled = true,
						IconId = (uint)((Weather)(ref rowOrDefault)).Icon,
						OnClick = delegate
						{
							if (module.IsWeatherCustom() && module.GetDisplayWeather() == weather)
							{
								module.ToggleWeather(isEnabled: false);
							}
							else
							{
								module.ToggleWeather(isEnabled: true, weather);
							}
							foreach (KeyValuePair<byte, (IconButtonNode, SimpleNineGridNode)> weatherButton in weatherButtons)
							{
								weatherButton.Deconstruct(out var key, out var value);
								(IconButtonNode, SimpleNineGridNode) tuple = value;
								byte b = key;
								tuple.Item2.IsVisible = module.IsWeatherCustom() && module.GetDisplayWeather() == b;
							}
						},
						TextTooltip = ReadOnlySeString.op_Implicit($"{((Weather)(ref rowOrDefault)).Name}")
					};
					SimpleNineGridNode simpleNineGridNode = new SimpleNineGridNode
					{
						TexturePath = "ui/uld/ContentsReplaySetting_hr1.tex",
						TextureCoordinates = new Vector2(36f, 44f),
						TextureSize = new Vector2(36f),
						Size = new Vector2(22f),
						Position = new Vector2(22f, 24f),
						IsVisible = (module.IsWeatherCustom() && module.GetDisplayWeather() == weather)
					};
					simpleNineGridNode.AttachNode((NodeBase?)iconButtonNode, NodePosition.AsLastChild);
					weatherButtons[weather] = (iconButtonNode, simpleNineGridNode);
					horizontalFlexNode.AddNode(iconButtonNode);
					horizontalFlexNode.AddDummy(5f);
					horizontalFlexNode.Width += iconButtonNode.Size.X + 4f;
					num2++;
				}
				if (num2 > 0)
				{
					verticalListNode.Height += horizontalFlexNode.Height;
					verticalListNode.AddNode(horizontalFlexNode);
				}
			}
			num += 85f;
			SetWindowSize(base.Size.X, num);
			verticalListNode.AddDummy(5f);
			CheckboxNode node = new CheckboxNode
			{
				IsVisible = true,
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("FastSetWeatherTime-Addon-ModifyTime")),
				Size = new Vector2(100f, 28f),
				IsChecked = module.IsTimeCustom(),
				OnClick = delegate(bool x)
				{
					module.ToggleTime(x, RealTime);
					timeNode.Value = (int)RealTime;
				}
			};
			verticalListNode.AddNode(node);
			timeNode = new SliderNode
			{
				Range = 0..86399,
				Value = (int)module.GetDisplayTime(),
				Size = new Vector2(base.Size.X - base.ContentStartPosition.X, 28f),
				OnValueChanged = delegate(int x)
				{
					if (module.IsTimeCustom())
					{
						module.ToggleTime(isEnabled: true, (uint)x);
					}
				}
			};
			timeNode.ValueNode.FontSize = 0u;
			timeNode.FloatValueNode.FontSize = 0u;
			verticalListNode.AddNode(timeNode);
			HorizontalListNode horizontalListNode = new HorizontalListNode
			{
				IsVisible = true,
				Size = new Vector2(100f, 35f)
			};
			hourInputNode = new NumericInputNode
			{
				Size = new Vector2(78f, 30f),
				OnValueUpdate = delegate(int hour)
				{
					if (module.IsTimeCustom())
					{
						TimeSpan timeSpan = TimeSpan.FromSeconds(module.GetDisplayTime());
						module.ToggleTime(isEnabled: true, (uint)(timeSpan.Minutes * 60 + timeSpan.Seconds + hour * 60 * 60));
						timeNode.Value = (int)module.GetDisplayTime();
					}
				}
			};
			horizontalListNode.Width += hourInputNode.Width;
			horizontalListNode.AddNode(hourInputNode);
			minuteInputNode = new NumericInputNode
			{
				Size = new Vector2(78f, 30f),
				OnValueUpdate = delegate(int minute)
				{
					if (module.IsTimeCustom())
					{
						TimeSpan timeSpan = TimeSpan.FromSeconds(module.GetDisplayTime());
						module.ToggleTime(isEnabled: true, (uint)(minute * 60 + timeSpan.Seconds + timeSpan.Hours * 60 * 60));
						timeNode.Value = (int)module.GetDisplayTime();
					}
				}
			};
			horizontalListNode.Width += minuteInputNode.Width;
			horizontalListNode.AddNode(minuteInputNode);
			secondInputNode = new NumericInputNode
			{
				Size = new Vector2(78f, 30f),
				OnValueUpdate = delegate(int second)
				{
					if (module.IsTimeCustom())
					{
						TimeSpan timeSpan = TimeSpan.FromSeconds(module.GetDisplayTime());
						module.ToggleTime(isEnabled: true, (uint)(timeSpan.Minutes * 60 + second + timeSpan.Hours * 60 * 60));
						timeNode.Value = (int)module.GetDisplayTime();
					}
				}
			};
			horizontalListNode.Width += secondInputNode.Width;
			horizontalListNode.AddNode(secondInputNode);
			verticalListNode.AddNode(horizontalListNode);
			num += 35f;
			SetWindowSize(base.Size.X, num);
			HorizontalFlexNode horizontalFlexNode2 = new HorizontalFlexNode
			{
				IsVisible = true,
				Size = new Vector2(base.Size.X - 2f * base.ContentStartPosition.X, 45f),
				Position = new Vector2(0f, -10f)
			};
			verticalListNode.AddNode(horizontalFlexNode2);
			saveButtonNode = new TextButtonNode
			{
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("Save")),
				Size = new Vector2(horizontalFlexNode2.Width / 2f - 5f, 30f),
				OnClick = delegate
				{
					//IL_0109: Unknown result type (might be due to invalid IL or missing references)
					//IL_010e: Unknown result type (might be due to invalid IL or missing references)
					//IL_011a: Expected O, but got Unknown
					if (module.IsTimeCustom() || module.IsWeatherCustom())
					{
						ZoneSetting value;
						ZoneSetting zoneSetting = (module.config.ZoneSettings.TryGetValue(GameState.TerritoryType, out value) ? value : new ZoneSetting());
						module.config.ZoneSettings[GameState.TerritoryType] = new ZoneSetting
						{
							IsTimeEnabled = (module.IsTimeCustom() || zoneSetting.IsTimeEnabled),
							IsWeatherEnabled = (module.IsWeatherCustom() || zoneSetting.IsWeatherEnabled),
							Time = (module.IsTimeCustom() ? module.GetDisplayTime() : zoneSetting.Time),
							WeatherID = (module.IsWeatherCustom() ? module.GetDisplayWeather() : zoneSetting.WeatherID)
						};
						module.config.Save(ManagerBase<ModuleManager>.Instance().GetModule<FastSetWeatherTime>());
						NotifyHelper.ToastQuest(LanguageManager.Get("FastSetWeatherTime-Notification-Saved"), new QuestToastOptions
						{
							DisplayCheckmark = true
						});
					}
				}
			};
			horizontalFlexNode2.AddNode(saveButtonNode);
			clearButtonNode = new TextButtonNode
			{
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("Clear")),
				Size = new Vector2(horizontalFlexNode2.Width / 2f - 5f, 30f),
				OnClick = delegate
				{
					if (module.config.ZoneSettings.Remove(GameState.TerritoryType))
					{
						module.config.Save(ManagerBase<ModuleManager>.Instance().GetModule<FastSetWeatherTime>());
						NotifyHelper.ToastQuest(LanguageManager.Get("FastSetWeatherTime-Notification-Cleard"));
					}
				}
			};
			horizontalFlexNode2.AddNode(clearButtonNode);
			verticalListNode.AttachNode(this);
		}

		protected unsafe override void OnFinalize(AtkUnitBase* addon)
		{
			weatherButtons.Clear();
			timeNode = null;
			hourInputNode = null;
			minuteInputNode = null;
			secondInputNode = null;
			saveButtonNode = null;
			clearButtonNode = null;
		}

		protected unsafe override void OnUpdate(AtkUnitBase* addon)
		{
			if (timeNode != null && hourInputNode != null && minuteInputNode != null && secondInputNode != null)
			{
				if (!module.IsTimeCustom())
				{
					timeNode.Value = (int)module.GetDisplayTime();
				}
				TimeSpan timeSpan = TimeSpan.FromSeconds(module.GetDisplayTime());
				hourInputNode.Value = timeSpan.Hours;
				minuteInputNode.Value = timeSpan.Minutes;
				secondInputNode.Value = timeSpan.Seconds;
			}
			if (saveButtonNode != null && clearButtonNode != null)
			{
				saveButtonNode.IsEnabled = GameState.TerritoryType != 0 && (module.IsTimeCustom() || module.IsWeatherCustom());
				clearButtonNode.IsEnabled = module.config.ZoneSettings.ContainsKey(GameState.TerritoryType);
			}
		}
	}

	private class ZoneSetting
	{
		public bool IsWeatherEnabled { get; set; }

		public byte WeatherID { get; set; } = byte.MaxValue;

		public bool IsTimeEnabled { get; set; }

		public uint Time { get; set; }
	}

	private class LVBFile : FileResource
	{
		public string ENVBFile;

		public ushort[] WeatherIDs;

		public override void LoadFile()
		{
			WeatherIDs = new ushort[32];
			int num = 12;
			if (((FileResource)this).Data[num] != 83 || ((FileResource)this).Data[num + 1] != 67 || ((FileResource)this).Data[num + 2] != 78 || ((FileResource)this).Data[num + 3] != 49)
			{
				num += 20;
			}
			int num2 = num;
			num += 16;
			int num3 = num2 + 8 + BitConverter.ToInt32(((FileResource)this).Data, num);
			num = num3 + 64;
			num = num3 + BitConverter.ToInt32(((FileResource)this).Data, num);
			for (int i = 0; i < 32; i++)
			{
				WeatherIDs[i] = BitConverter.ToUInt16(((FileResource)this).Data, num + i * 2);
			}
			if (((FileResource)this).Data.TryFindBytes("2E 65 6E 76 62 00", out num))
			{
				int num4 = num + 5;
				while (((FileResource)this).Data[num - 1] != 0 && num > 0)
				{
					num--;
				}
				ENVBFile = Encoding.UTF8.GetString(((FileResource)this).Data.Skip(num).Take(num4 - num).ToArray());
			}
		}
	}

	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static HandlerDelegate _003C0_003E__OnCommand;
	}

	private static readonly CompSig PlayWeatherSoundSig = new CompSig("48 89 5C 24 ?? 48 89 6C 24 ?? 56 57 41 56 48 83 EC ?? 45 33 F6 0F 29 74 24");

	private Hook<PlayWeatherSoundDelegate> PlayWeatherSoundHook;

	private static readonly CompSig UpdateBgmSituationSig = new CompSig("48 89 5C 24 ?? 57 48 83 EC 20 B8 ?? ?? ?? ?? 49 8B F9 41 8B D8");

	private Hook<UpdateBgmSituationDelegate> UpdateBgmSituationHook;

	private MemoryPatchWithPointer<uint> renderSunlightShadowPatch;

	private MemoryPatchWithPointer<byte> renderWeatherPatch;

	private MemoryPatchWithPointer<uint> renderTimePatch;

	private Config config;

	private TextButtonNode? openButton;

	private const uint MAX_TIME = 86400u;

	private const string COMMAND = "wt";

	private const string NAVI_MAP_IMAGE_URL = "hxxps://raw.githubusercontent.com/AtmoOmen/StaticAssets/refs/heads/main/DailyRoutines/image/FastSetWeatherTime-NaviMap.png";

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("FastSetWeatherTimeTitle"),
		Description = LanguageManager.Get("FastSetWeatherTimeDescription", "wt"),
		Category = ModuleCategory.Interface
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	private static uint RealTime
	{
		get
		{
			EorzeaDate time = EorzeaDate.GetTime();
			return (uint)(time.Second + 60 * time.Minute + 3600 * time.Hour);
		}
	}

	private unsafe static byte RealWeather => ((byte*)EnvManager.Instance())[38];

	private byte CustomWeather
	{
		get
		{
			return renderWeatherPatch.CurrentValue;
		}
		set
		{
			renderWeatherPatch.Set(value);
		}
	}

	private uint CustomTime
	{
		get
		{
			return renderTimePatch.CurrentValue;
		}
		set
		{
			renderTimePatch.Set(value);
		}
	}

	protected unsafe override void Init()
	{
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Expected O, but got Unknown
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Expected O, but got Unknown
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Expected O, but got Unknown
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		renderTimePatch = new MemoryPatchWithPointer<uint>("48 89 5C 24 ?? 57 48 83 EC 30 4C 8B 15", new global::_003C_003Ez__ReadOnlyArray<byte?>(new byte?[7]
		{
			(byte)73,
			(byte)199,
			(byte)193,
			0,
			0,
			0,
			0
		}), 25, 3);
		renderSunlightShadowPatch = new MemoryPatchWithPointer<uint>("49 0F BE 40 ?? 84 C0", new global::_003C_003Ez__ReadOnlyArray<byte?>(new byte?[5]
		{
			(byte)184,
			0,
			0,
			0,
			0
		}), 0, 1);
		renderWeatherPatch = new MemoryPatchWithPointer<byte>("48 89 5C 24 ?? 57 48 83 EC 30 80 B9 ?? ?? ?? ?? ?? 49 8B F8 0F 29 74 24 ?? 48 8B D9 0F 28 F1", new global::_003C_003Ez__ReadOnlyArray<byte?>(new byte?[4]
		{
			(byte)178,
			0,
			(byte)144,
			(byte)144
		}), 85, 1);
		if (PlayWeatherSoundHook == null)
		{
			PlayWeatherSoundHook = PlayWeatherSoundSig.GetHook<PlayWeatherSoundDelegate>(PlayWeatherSoundDetour);
		}
		PlayWeatherSoundHook.Enable();
		if (UpdateBgmSituationHook == null)
		{
			UpdateBgmSituationHook = UpdateBgmSituationSig.GetHook<UpdateBgmSituationDelegate>(UpdateBgmSituationDetour);
		}
		UpdateBgmSituationHook.Enable();
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += OnZoneChanged;
		AddonDRFastSetWeather.Addon = new AddonDRFastSetWeather(this)
		{
			InternalName = "DRFastSetWeather",
			Title = ReadOnlySeString.op_Implicit(LanguageManager.Get("Weather") + " & " + LanguageManager.Get("Time")),
			Size = new Vector2(254f, 50f)
		};
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)8, "_NaviMap", new AddonEventDelegate(OnAddon));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, "_NaviMap", new AddonEventDelegate(OnAddon));
		CommandManager commandManager = OmenServiceBase<CommandManager>.Instance();
		object obj = _003C_003EO._003C0_003E__OnCommand;
		if (obj == null)
		{
			HandlerDelegate val = OnCommand;
			_003C_003EO._003C0_003E__OnCommand = val;
			obj = (object)val;
		}
		commandManager.AddSubCommand("wt", new CommandInfo((HandlerDelegate)obj)
		{
			HelpMessage = LanguageManager.Get("FastSetWeatherTime-CommandHelp")
		});
	}

	protected override void Uninit()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChanged;
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddon });
		openButton?.Dispose();
		openButton = null;
		OmenServiceBase<CommandManager>.Instance().RemoveSubCommand("wt");
		AddonDRFastSetWeather.Addon?.Dispose();
		AddonDRFastSetWeather.Addon = null;
	}

	protected override void ConfigUI()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("FastSetWeatherTime-CommandHelp")));
		IndentDisposable val = ImRaii.PushIndent(1, true);
		try
		{
			ImU8String val2 = new ImU8String(8, 1);
			((ImU8String)(ref val2)).AppendLiteral("1. /pdr ");
			((ImU8String)(ref val2)).AppendFormatted<string>("wt");
			ImGui.TextUnformatted(val2);
			if (OmenServiceBase<ImageHelper>.Instance().TryGetImage("hxxps://raw.githubusercontent.com/AtmoOmen/StaticAssets/refs/heads/main/DailyRoutines/image/FastSetWeatherTime-NaviMap.png", out IDalamudTextureWrap texture))
			{
				ImU8String val3 = new ImU8String(3, 1);
				((ImU8String)(ref val3)).AppendLiteral("2. ");
				((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get("FastSetWeatherTime-OperationHelp-ClickNaviMap"));
				ImGui.TextUnformatted(val3);
				ImGui.Image(texture.Handle, texture.Size);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private unsafe void* PlayWeatherSoundDetour(void* manager, byte weatherID, void* a3, void* a4)
	{
		if (IsWeatherCustom())
		{
			weatherID = GetDisplayWeather();
		}
		return PlayWeatherSoundHook.Original(manager, weatherID, a3, a4);
	}

	private unsafe void* UpdateBgmSituationDetour(void* manager, ushort bgmSituationID, int column, void* a4, void* a5)
	{
		if (IsTimeCustom() && column != 3)
		{
			uint num = CustomTime % 86400;
			column = ((num >= 21600 && num < 64800) ? 1 : 2);
		}
		return UpdateBgmSituationHook.Original(manager, bgmSituationID, column, a4, a5);
	}

	private void OnZoneChanged(uint u)
	{
		config.ZoneSettings.TryGetValue(GameState.TerritoryType, out ZoneSetting value);
		if (value != null && value.IsWeatherEnabled && value.WeatherID != byte.MaxValue)
		{
			ToggleWeather(isEnabled: true, value.WeatherID);
		}
		else
		{
			ToggleWeather(isEnabled: false);
		}
		if (value != null && value.IsTimeEnabled)
		{
			ToggleTime(isEnabled: true, value.Time);
		}
		else
		{
			ToggleTime(isEnabled: false);
		}
	}

	private unsafe void OnAddon(AddonEvent type, AddonArgs args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		if ((int)type != 6)
		{
			if ((int)type != 8 || !(*Addons.NaviMap).IsAddonAndNodesReady())
			{
				return;
			}
			if (openButton == null)
			{
				openButton = new TextButtonNode
				{
					Position = new Vector2(158f, 24f),
					Size = new Vector2(36f),
					IsVisible = true,
					String = ReadOnlySeString.op_Implicit(string.Empty),
					OnClick = delegate
					{
						AddonDRFastSetWeather.Addon.Toggle();
					}
				};
				openButton.BackgroundNode.IsVisible = false;
				openButton.AttachNode(((AtkUnitBase)Addons.NaviMap).RootNode);
			}
			TextButtonNode? textButtonNode = openButton;
			Weather rowOrDefault = LuminaGetter.GetRowOrDefault<Weather>((uint)GetDisplayWeather());
			textButtonNode.TextTooltip = ((Weather)(ref rowOrDefault)).Name;
		}
		else
		{
			openButton = null;
		}
	}

	private static void OnCommand(string command, string args)
	{
		AddonDRFastSetWeather.Addon.Toggle();
	}

	private void ToggleWeather(bool isEnabled, byte weatherID = byte.MaxValue)
	{
		if (!isEnabled || weatherID == byte.MaxValue)
		{
			DisableCustomWeather();
			return;
		}
		EnableCustomWeather();
		CustomWeather = weatherID;
	}

	private void EnableCustomWeather()
	{
		if (!IsWeatherCustom())
		{
			renderWeatherPatch.Enable();
			renderSunlightShadowPatch.Enable();
		}
	}

	private void DisableCustomWeather()
	{
		if (IsWeatherCustom())
		{
			renderWeatherPatch.Disable();
			renderSunlightShadowPatch.Disable();
		}
	}

	private void ToggleTime(bool isEnabled, uint time = 0u)
	{
		if (!isEnabled)
		{
			DisableCustomTime();
			return;
		}
		EnableCustomTime();
		CustomTime = time;
	}

	private void EnableCustomTime()
	{
		if (!IsTimeCustom())
		{
			renderTimePatch.Enable();
		}
	}

	private void DisableCustomTime()
	{
		if (IsTimeCustom())
		{
			renderTimePatch.Disable();
		}
	}

	private byte GetDisplayWeather()
	{
		if (!IsWeatherCustom())
		{
			return RealWeather;
		}
		return CustomWeather;
	}

	private uint GetDisplayTime()
	{
		if (!IsTimeCustom())
		{
			return RealTime;
		}
		return CustomTime;
	}

	private bool IsWeatherCustom()
	{
		return renderWeatherPatch.IsEnabled;
	}

	private bool IsTimeCustom()
	{
		return renderTimePatch.IsEnabled;
	}

	private static (List<byte> WeatherList, string ENVBFile) ParseLVB(ushort zoneID)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		List<byte> list = new List<byte>();
		try
		{
			IDataManager obj = DalamudServiceExtension.Instance<IDataManager>();
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(7, 1);
			defaultInterpolatedStringHandler.AppendLiteral("bg/");
			TerritoryType rowOrDefault = LuminaGetter.GetRowOrDefault<TerritoryType>((uint)zoneID);
			defaultInterpolatedStringHandler.AppendFormatted<ReadOnlySeString>(((TerritoryType)(ref rowOrDefault)).Bg);
			defaultInterpolatedStringHandler.AppendLiteral(".lvb");
			LVBFile file = obj.GetFile<LVBFile>(defaultInterpolatedStringHandler.ToStringAndClear());
			if (file?.WeatherIDs == null || file.WeatherIDs.Length == 0)
			{
				return (WeatherList: new List<byte>(), ENVBFile: string.Empty);
			}
			ushort[] weatherIDs = file.WeatherIDs;
			foreach (ushort num in weatherIDs)
			{
				if (num > 0 && num < 255)
				{
					list.Add((byte)num);
				}
			}
			list.Sort();
			return (WeatherList: list, ENVBFile: file.ENVBFile);
		}
		catch
		{
		}
		return (WeatherList: new List<byte>(), ENVBFile: string.Empty);
	}
}
