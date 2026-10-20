using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.Agent;
using Dalamud.Game.Agent.AgentArgTypes;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.ContextMenu;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using KamiToolKit.Nodes.Simplified;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.KamiToolKit.Addons;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.ModulesPublic.AutoRecordPartyFinderSettings;

public class AutoRecordPartyFinderSetting : ModuleBase
{
	private sealed class AutoRecordPartyFinderSettingAddon(AutoRecordPartyFinderSetting module) : AttachedAddon("LookingForGroupCondition", (AddonEvent)1)
	{
		private VerticalListNode? mainLayout;

		private HorizontalListNode? actionHeader;

		private VerticalListNode? presetListContainer;

		private HorizontalFlexNode? pagingLayout;

		private readonly List<PresetRowNode> presetRows = new List<PresetRowNode>();

		private TextButtonNode prevPageBtn;

		private TextButtonNode nextPageBtn;

		private TextNode pageLabel;

		public int CurrentPageIndex;

		private ContextMenu? contextMenu;

		protected override AttachedAddonPosition AttachPosition => AttachedAddonPosition.LeftTop;

		protected override Vector2 PositionOffset => new Vector2(0f, 6f);

		protected unsafe override bool CanOpenAddon
		{
			get
			{
				if (Addons.LookingForGroupCondition != null)
				{
					return (*Addons.LookingForGroupCondition).IsAddonAndNodesReady();
				}
				return false;
			}
		}

		public override void Dispose()
		{
			contextMenu?.Dispose();
			contextMenu = null;
			base.Dispose();
		}

		public unsafe void ShowContextMenu(PartyFinderSetting setting)
		{
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			contextMenu?.Dispose();
			contextMenu = new ContextMenu();
			contextMenu.AddItem(ReadOnlySeString.op_Implicit(LanguageManager.Get("Update")), delegate
			{
				if (Addons.LookingForGroupCondition != null && (*Addons.LookingForGroupCondition).IsAddonAndNodesReady())
				{
					string displayName = setting.DisplayName;
					PartyFinderSetting partyFinderSetting = module.config.Last.Copy();
					partyFinderSetting.DisplayName = displayName;
					int num = module.config.Slot.IndexOf(setting);
					if (num != -1)
					{
						module.config.Slot[num] = partyFinderSetting;
						module.config.Save(module);
						RefreshPresetList();
					}
				}
			});
			contextMenu.Open();
		}

		protected unsafe override bool CanCloseHostAddon(AtkUnitBase* hostAddon)
		{
			return false;
		}

		protected unsafe override void OnSetup(AtkUnitBase* addon, Span<AtkValue> atkValues)
		{
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
			if (base.WindowNode is WindowNode windowNode)
			{
				windowNode.CloseButtonNode.IsVisible = false;
			}
			FlagHelper.UpdateFlag(ref ((AtkUnitBase)addon).Flags1A1, 4, enable: true);
			FlagHelper.UpdateFlag(ref ((AtkUnitBase)addon).Flags1A0, 128, enable: true);
			FlagHelper.UpdateFlag(ref ((AtkUnitBase)addon).Flags1A1, 64, enable: true);
			FlagHelper.UpdateFlag(ref ((AtkUnitBase)addon).Flags1A3, 1, enable: true);
			mainLayout = new VerticalListNode
			{
				IsVisible = true,
				Position = base.ContentStartPosition,
				ItemSpacing = 6f,
				Size = base.ContentSize,
				FitContents = true
			};
			HorizontalListNode obj = new HorizontalListNode
			{
				IsVisible = true
			};
			Vector2 contentSize = base.ContentSize;
			contentSize.Y = 32f;
			obj.Size = contentSize;
			obj.ItemSpacing = 8f;
			actionHeader = obj;
			TextButtonNode textButtonNode = new TextButtonNode();
			contentSize = base.ContentSize;
			contentSize.Y = 32f;
			textButtonNode.Size = contentSize;
			textButtonNode.String = ReadOnlySeString.op_Implicit(LanguageManager.Get("Add"));
			textButtonNode.TextTooltip = ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoRecordPartyFinderSetting-Button-Save-Help"));
			textButtonNode.OnClick = delegate
			{
				//IL_0050: Unknown result type (might be due to invalid IL or missing references)
				//IL_0055: Unknown result type (might be due to invalid IL or missing references)
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				if ((*Addons.LookingForGroupCondition).IsAddonAndNodesReady())
				{
					PartyFinderSetting partyFinderSetting = module.config.Last.Copy();
					partyFinderSetting.DisplayName = ((object)Utf8StringExtensions.AsReadOnlySeString(((AtkTextNode)((AtkTextNode)((AtkComponentBase)((AtkComponentNode)((AtkResNode)((AtkUldManager)(&((AtkComponentBase)((AtkUnitBase)Addons.LookingForGroupCondition).GetComponentByNodeId(11u)).UldManager)).SearchNodeById(2u)).GetAsAtkComponentNode()).Component).GetTextNodeById(3u)).GetAsAtkTextNode()).NodeText)/*cast due to constrained. prefix*/).ToString();
					module.config.Slot.Add(partyFinderSetting);
					module.config.Save(module);
					RefreshPresetList();
				}
			};
			TextButtonNode textButtonNode2 = textButtonNode;
			SimpleNineGridNode obj2 = (SimpleNineGridNode)textButtonNode2.BackgroundNode;
			obj2.TexturePath = "ui/uld/img04/ButtonB_hr1.tex";
			obj2.TextureSize = new Vector2(80f, 36f);
			obj2.Offsets = new Vector4(16f);
			actionHeader.AddNode(textButtonNode2);
			presetListContainer = new VerticalListNode
			{
				IsVisible = true,
				ItemSpacing = 4f,
				FitContents = true,
				FitWidth = true,
				Size = base.ContentSize
			};
			presetRows.Clear();
			for (int num = 0; num < 10; num++)
			{
				PresetRowNode presetRowNode = new PresetRowNode(module, this);
				presetListContainer.AddNode(presetRowNode);
				presetRows.Add(presetRowNode);
			}
			HorizontalFlexNode obj3 = new HorizontalFlexNode
			{
				IsVisible = true
			};
			contentSize = base.ContentSize;
			contentSize.Y = 28f;
			obj3.Size = contentSize;
			obj3.AlignmentFlags = FlexFlags.CenterHorizontally;
			pagingLayout = obj3;
			prevPageBtn = new TextButtonNode
			{
				Size = new Vector2(40f, 24f),
				String = ReadOnlySeString.op_Implicit("<"),
				OnClick = delegate
				{
					if (CurrentPageIndex > 0)
					{
						CurrentPageIndex--;
						RefreshPresetList();
					}
				}
			};
			pageLabel = new TextNode
			{
				TextFlags = (TextFlags)1,
				String = ReadOnlySeString.op_Implicit("1 / 1"),
				AlignmentType = (AlignmentType)4,
				FontSize = 14u,
				Position = new Vector2(0f, 3f)
			};
			nextPageBtn = new TextButtonNode
			{
				Size = new Vector2(40f, 24f),
				String = ReadOnlySeString.op_Implicit(">"),
				OnClick = delegate
				{
					int count = module.config.Slot.Count;
					int num2 = Math.Max(1, (int)Math.Ceiling((double)count / 10.0));
					if (CurrentPageIndex < num2 - 1)
					{
						CurrentPageIndex++;
						RefreshPresetList();
					}
				}
			};
			pagingLayout.AddNode(prevPageBtn);
			pagingLayout.AddDummy(10f);
			pagingLayout.AddNode(pageLabel);
			pagingLayout.AddDummy(10f);
			pagingLayout.AddNode(nextPageBtn);
			mainLayout.AddNode(actionHeader);
			VerticalListNode? verticalListNode = mainLayout;
			HorizontalLineNode horizontalLineNode = new HorizontalLineNode();
			contentSize = base.ContentSize;
			contentSize.Y = 4f;
			horizontalLineNode.Size = contentSize;
			horizontalLineNode.Position = new Vector2(0f, -4f);
			verticalListNode.AddNode(horizontalLineNode);
			mainLayout.AddNode(presetListContainer);
			mainLayout.AddDummy();
			mainLayout.AddNode(pagingLayout);
			mainLayout.AttachNode(this);
			RefreshPresetList();
		}

		public void RefreshPresetList()
		{
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			int count = module.config.Slot.Count;
			int num = Math.Max(1, (int)Math.Ceiling((double)count / 10.0));
			if (CurrentPageIndex >= num)
			{
				CurrentPageIndex = num - 1;
			}
			if (CurrentPageIndex < 0)
			{
				CurrentPageIndex = 0;
			}
			List<PartyFinderSetting> list = module.config.Slot.Skip(CurrentPageIndex * 10).Take(10).ToList();
			for (int i = 0; i < 10; i++)
			{
				PresetRowNode presetRowNode = presetRows[i];
				if (i < list.Count)
				{
					PartyFinderSetting setting = list[i];
					presetRowNode.IsVisible = true;
					presetRowNode.Update(setting);
				}
				else
				{
					presetRowNode.IsVisible = false;
				}
			}
			prevPageBtn.IsEnabled = CurrentPageIndex > 0;
			nextPageBtn.IsEnabled = CurrentPageIndex < num - 1;
			pageLabel.String = ReadOnlySeString.op_Implicit($"{CurrentPageIndex + 1} / {num}");
			presetListContainer.RecalculateLayout();
			if (mainLayout != null)
			{
				mainLayout.RecalculateLayout();
				SetWindowSize(base.Size.X, base.ContentStartPosition.Y + mainLayout.Height + 16f);
				mainLayout.Position = base.ContentStartPosition;
			}
		}

		protected unsafe override void OnHostAddon(AddonEvent type, AddonArgs? args)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Invalid comparison between Unknown and I4
			if ((int)type == 1 && !module.isAppliedOnce && (*Addons.LookingForGroup).IsAddonAndNodesReady())
			{
				module.ApplyPreset(module.config.Last);
				module.isAppliedOnce = true;
			}
		}
	}

	private class PresetRowNode : HorizontalListNode
	{
		private readonly TextButtonNode titleButton;

		private readonly TextButtonNode deleteButton;

		public PartyFinderSetting Setting { get; set; }

		public unsafe PresetRowNode(AutoRecordPartyFinderSetting module, AutoRecordPartyFinderSettingAddon addon)
		{
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Expected O, but got Unknown
			PresetRowNode presetRowNode = this;
			IsVisible = true;
			Size = new Vector2(addon.ContentSize.X - 8f, 28f);
			base.ItemSpacing = 6f;
			titleButton = new TextButtonNode
			{
				IsVisible = true,
				Size = new Vector2(addon.ContentSize.X - 74f, 28f),
				String = ReadOnlySeString.op_Implicit(string.Empty)
			};
			TextNode labelNode = titleButton.LabelNode;
			labelNode.TextFlags = (TextFlags)(labelNode.TextFlags | 0x400);
			deleteButton = new TextButtonNode
			{
				IsVisible = true,
				Size = new Vector2(68f, 28f),
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("Delete")),
				OnClick = delegate
				{
					module.config.Slot.Remove(presetRowNode.Setting);
					module.config.Save(module);
					int num = Math.Max(1, (int)Math.Ceiling((double)module.config.Slot.Count / 10.0));
					if (addon.CurrentPageIndex >= num)
					{
						addon.CurrentPageIndex = num - 1;
					}
					addon.RefreshPresetList();
				},
				TextTooltip = ReadOnlySeString.op_Implicit(LanguageManager.Get("Delete"))
			};
			AddNode(titleButton);
			AddNode(deleteButton);
			titleButton.AddEvent((AtkEventType)9, (ReceiveEvent)delegate(AtkEventListener* _, AtkEventType _, int _, AtkEvent* _, AtkEventData* atkEventData)
			{
				if (AtkEventDataExtension.get_IsRightClick(in *atkEventData))
				{
					addon.ShowContextMenu(presetRowNode.Setting);
				}
				else if (AtkEventDataExtension.get_IsLeftClick(in *atkEventData))
				{
					module.ApplyPreset(presetRowNode.Setting);
				}
			});
		}

		public void Update(PartyFinderSetting setting)
		{
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			Setting = setting;
			ReadOnlySeString val = default(ReadOnlySeString);
			((ReadOnlySeString)(ref val))._002Ector(setting.DescriptionBytes ?? Array.Empty<byte>());
			string text = (string.IsNullOrEmpty(setting.DisplayName) ? ("[" + LanguageManager.Get("None") + "]") : setting.DisplayName);
			titleButton.String = val;
			ReadOnlySeString se = LanguageManager.GetSe("AutoRecordPartyFinderSetting-Message", text, val);
			titleButton.TextTooltip = se;
		}
	}

	private class PartyFinderSetting
	{
		public uint? Category = 0u;

		public uint? Duty = 0u;

		public bool? WelcomeNew = false;

		public uint? Goal = 1u;

		public uint? SetFinishRequirement = 1u;

		public uint? FinishRequirement = 2u;

		public bool? SetAllSlotsEveryone = false;

		public bool? OnlyClassJob = false;

		public uint? NoSameJob = 0u;

		public uint? IsLocal = 1u;

		public uint? IsSetPassword = 0u;

		public int? Password = 0;

		public bool? Unrestricted = false;

		public bool? MinIL = false;

		public bool? SilenceEcho = false;

		public uint? LootType = 0u;

		public uint? Type = 0u;

		public bool? NoAudiences = false;

		public byte[]? DescriptionBytes = new byte[1];

		public bool? IsEnableAvgIL = false;

		public uint? AvgIL = 1u;

		public string? DisplayName;

		[CompilerGenerated]
		private string? _003CName_003Ek__BackingField;

		[CompilerGenerated]
		private string? _003CDescription_003Ek__BackingField;

		public string? Name
		{
			[CompilerGenerated]
			get
			{
				return _003CName_003Ek__BackingField;
			}
			set
			{
				_003CName_003Ek__BackingField = null;
				if (!string.IsNullOrEmpty(value))
				{
					DisplayName = value;
				}
			}
		}

		public string? Description
		{
			[CompilerGenerated]
			get
			{
				return _003CDescription_003Ek__BackingField;
			}
			set
			{
				_003CDescription_003Ek__BackingField = null;
				if (!string.IsNullOrEmpty(value))
				{
					DescriptionBytes = Encoding.UTF8.GetBytes(value + "\0");
				}
			}
		}

		public PartyFinderSetting Copy()
		{
			return new PartyFinderSetting
			{
				DisplayName = DisplayName,
				Category = Category,
				Duty = Duty,
				AvgIL = AvgIL,
				IsEnableAvgIL = IsEnableAvgIL,
				WelcomeNew = WelcomeNew,
				Goal = Goal,
				SetFinishRequirement = SetFinishRequirement,
				FinishRequirement = FinishRequirement,
				SetAllSlotsEveryone = SetAllSlotsEveryone,
				OnlyClassJob = OnlyClassJob,
				NoSameJob = NoSameJob,
				IsLocal = IsLocal,
				IsSetPassword = IsSetPassword,
				Password = Password,
				Unrestricted = Unrestricted,
				MinIL = MinIL,
				SilenceEcho = SilenceEcho,
				LootType = LootType,
				Type = Type,
				NoAudiences = NoAudiences,
				DescriptionBytes = DescriptionBytes
			};
		}
	}

	private class Config : ModuleConfig
	{
		public PartyFinderSetting Last = new PartyFinderSetting();

		public List<PartyFinderSetting> Slot = new List<PartyFinderSetting>();

		public override string PreviousModuleName => "AutoRecordPartyFinderSetting";
	}

	private Config config;

	private bool isAppliedOnce;

	private AutoRecordPartyFinderSettingAddon? addon;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoRecordPartyFinderSettingTitle"),
		Description = LanguageManager.Get("AutoRecordPartyFinderSettingDescription"),
		Category = ModuleCategory.Recruitment,
		Author = new string[1] { "status102" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected override void Init()
	{
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Expected O, but got Unknown
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		if (base.TaskHelper == null)
		{
			TaskHelper taskHelper = (base.TaskHelper = new TaskHelper());
		}
		if (addon == null)
		{
			addon = new AutoRecordPartyFinderSettingAddon(this)
			{
				InternalName = "DRAutoRecordPartyFinderSetting",
				Title = ReadOnlySeString.op_Implicit(Info.Title),
				Size = new Vector2(340f, 410f),
				RememberClosePosition = false
			};
		}
		DalamudServiceExtension.Instance<IAgentLifecycle>().RegisterListener((AgentEvent)0, (AgentId)130, new AgentEventDelegate(OnAgent));
	}

	protected override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAgentLifecycle>().UnregisterListener((AgentEventDelegate[])(object)new AgentEventDelegate[1] { OnAgent });
		addon?.Dispose();
		addon = null;
	}

	private unsafe void OnAgent(AgentEvent type, AgentArgs args)
	{
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		AgentReceiveEventArgs e = (AgentReceiveEventArgs)(object)((args is AgentReceiveEventArgs) ? args : null);
		AtkValue* atkValues = (AtkValue*)e.AtkValues;
		if (atkValues == null || e.EventKind != 3)
		{
			return;
		}
		switch (((AtkValue)atkValues).Int)
		{
		default:
			return;
		case 11:
			config.Last.AvgIL = ((AtkValue)((byte*)atkValues + Unsafe.SizeOf<AtkValue>())).UInt;
			config.Last.IsEnableAvgIL = ((AtkValue)((byte*)atkValues + (nint)2 * (nint)Unsafe.SizeOf<AtkValue>())).Bool;
			break;
		case 12:
			config.Last.Category = ((AtkValue)((byte*)atkValues + Unsafe.SizeOf<AtkValue>())).UInt;
			config.Last.Duty = 0u;
			break;
		case 13:
			config.Last.Duty = ((AtkValue)((byte*)atkValues + Unsafe.SizeOf<AtkValue>())).UInt;
			break;
		case 15:
		{
			List<byte> list = CStringExtensions.AsReadOnlySeString(((AtkValue)((byte*)atkValues + Unsafe.SizeOf<AtkValue>())).String).Data.ToArray().ToList();
			list.Add(0);
			config.Last.DescriptionBytes = list.ToArray();
			break;
		}
		case 16:
			config.Last.Password = ((AtkValue)((byte*)atkValues + Unsafe.SizeOf<AtkValue>())).Int;
			break;
		case 18:
			switch (((AtkValue)((byte*)atkValues + (nint)2 * (nint)Unsafe.SizeOf<AtkValue>())).UInt)
			{
			case 1u:
				config.Last.WelcomeNew = true;
				break;
			case 3u:
				switch (((AtkValue)((byte*)atkValues + Unsafe.SizeOf<AtkValue>())).UInt)
				{
				case 0u:
					config.Last.Unrestricted = true;
					break;
				case 1u:
					config.Last.MinIL = true;
					break;
				case 2u:
					config.Last.SilenceEcho = true;
					break;
				}
				break;
			}
			config.Last.WelcomeNew = true;
			break;
		case 19:
			switch (((AtkValue)((byte*)atkValues + (nint)2 * (nint)Unsafe.SizeOf<AtkValue>())).UInt)
			{
			case 1u:
				config.Last.WelcomeNew = false;
				break;
			case 3u:
				switch (((AtkValue)((byte*)atkValues + Unsafe.SizeOf<AtkValue>())).UInt)
				{
				case 0u:
					config.Last.Unrestricted = false;
					break;
				case 1u:
					config.Last.MinIL = false;
					break;
				case 2u:
					config.Last.SilenceEcho = false;
					break;
				}
				break;
			}
			break;
		case 20:
			switch (((AtkValue)((byte*)atkValues + (nint)2 * (nint)Unsafe.SizeOf<AtkValue>())).UInt)
			{
			case 0u:
				config.Last.Goal = ((AtkValue)((byte*)atkValues + Unsafe.SizeOf<AtkValue>())).UInt;
				break;
			case 2u:
				if (((AtkValue)((byte*)atkValues + Unsafe.SizeOf<AtkValue>())).UInt < 3)
				{
					if (((AtkValue)((byte*)atkValues + Unsafe.SizeOf<AtkValue>())).UInt == 1)
					{
						config.Last.SetFinishRequirement = ((AtkValue)((byte*)atkValues + Unsafe.SizeOf<AtkValue>())).UInt;
						break;
					}
					if (((AtkValue)((byte*)atkValues + Unsafe.SizeOf<AtkValue>())).UInt == 3)
					{
						config.Last.FinishRequirement = ((AtkValue)((byte*)atkValues + Unsafe.SizeOf<AtkValue>())).UInt;
						break;
					}
					config.Last.SetFinishRequirement = ((AtkValue)((byte*)atkValues + Unsafe.SizeOf<AtkValue>())).UInt;
					config.Last.FinishRequirement = ((AtkValue)((byte*)atkValues + Unsafe.SizeOf<AtkValue>())).UInt;
				}
				else
				{
					config.Last.FinishRequirement = ((AtkValue)((byte*)atkValues + Unsafe.SizeOf<AtkValue>())).UInt;
				}
				break;
			case 4u:
				config.Last.LootType = ((AtkValue)((byte*)atkValues + Unsafe.SizeOf<AtkValue>())).UInt;
				break;
			}
			break;
		case 21:
			config.Last.IsSetPassword = ((AtkValue)((byte*)atkValues + Unsafe.SizeOf<AtkValue>())).UInt;
			break;
		case 22:
			config.Last.IsLocal = ((AtkValue)((byte*)atkValues + Unsafe.SizeOf<AtkValue>())).UInt;
			break;
		case 23:
			config.Last.NoSameJob = ((AtkValue)((byte*)atkValues + Unsafe.SizeOf<AtkValue>())).UInt;
			break;
		case 33:
			config.Last.SetAllSlotsEveryone = ((AtkValue)((byte*)atkValues + Unsafe.SizeOf<AtkValue>())).Bool;
			break;
		case 34:
			config.Last.OnlyClassJob = ((AtkValue)((byte*)atkValues + Unsafe.SizeOf<AtkValue>())).Bool;
			break;
		case 35:
			config.Last.Type = ((AtkValue)((byte*)atkValues + Unsafe.SizeOf<AtkValue>())).UInt;
			break;
		case 36:
			config.Last.NoAudiences = ((AtkValue)((byte*)atkValues + Unsafe.SizeOf<AtkValue>())).Bool;
			break;
		case 14:
		case 17:
		case 24:
		case 25:
		case 26:
		case 27:
		case 28:
		case 29:
		case 30:
		case 31:
		case 32:
			return;
		}
		config.Save(this);
	}

	private unsafe void ApplyPreset(PartyFinderSetting setting)
	{
		if (!(*Addons.LookingForGroup).IsAddonAndNodesReady() || !(*Addons.LookingForGroupCondition).IsAddonAndNodesReady())
		{
			return;
		}
		if (setting.Type.HasValue)
		{
			base.TaskHelper.Enqueue(delegate
			{
				SendEvent(new object[3]
				{
					35,
					setting.Type.Value,
					0
				});
			});
		}
		if (setting.Category.HasValue)
		{
			base.TaskHelper.Enqueue(delegate
			{
				SendEvent(new object[3]
				{
					12,
					setting.Category.Value,
					0
				});
			});
			if (setting.Category.Value != 0 && setting.Duty.HasValue)
			{
				base.TaskHelper.Enqueue(delegate
				{
					SendEvent(new object[3]
					{
						13,
						setting.Duty.Value,
						0
					});
				});
			}
		}
		if (setting.IsEnableAvgIL.HasValue)
		{
			base.TaskHelper.Enqueue(delegate
			{
				SendEvent(new object[3]
				{
					11,
					1,
					setting.IsEnableAvgIL.Value
				});
			});
			if (setting.IsEnableAvgIL.Value && setting.AvgIL.HasValue)
			{
				base.TaskHelper.Enqueue(delegate
				{
					SendEvent(new object[3]
					{
						11,
						setting.AvgIL.Value,
						setting.IsEnableAvgIL.Value
					});
				});
			}
		}
		if (setting.DescriptionBytes != null)
		{
			base.TaskHelper.Enqueue(delegate
			{
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				SendEvent(new object[3]
				{
					15,
					(object)new ReadOnlySeString(setting.DescriptionBytes),
					0
				});
			});
		}
		if (setting.IsSetPassword.HasValue)
		{
			base.TaskHelper.Enqueue(delegate
			{
				SendEvent(new object[3]
				{
					21,
					setting.IsSetPassword.Value,
					0
				});
			});
			if (setting.IsSetPassword.Value != 0 && setting.Password.HasValue)
			{
				base.TaskHelper.Enqueue(delegate
				{
					SendEvent(new object[3]
					{
						16,
						setting.Password.Value,
						0
					});
				});
			}
		}
		if (setting.WelcomeNew.HasValue)
		{
			base.TaskHelper.Enqueue(delegate
			{
				SendEvent(new object[3]
				{
					setting.WelcomeNew.Value ? 18 : 19,
					0,
					1
				});
			});
		}
		if (setting.Unrestricted.HasValue)
		{
			base.TaskHelper.Enqueue(delegate
			{
				SendEvent(new object[3]
				{
					setting.Unrestricted.Value ? 18 : 19,
					0,
					3
				});
			});
		}
		if (setting.MinIL.HasValue)
		{
			base.TaskHelper.Enqueue(delegate
			{
				SendEvent(new object[3]
				{
					setting.MinIL.Value ? 18 : 19,
					1,
					3
				});
			});
		}
		if (setting.SilenceEcho.HasValue)
		{
			base.TaskHelper.Enqueue(delegate
			{
				SendEvent(new object[3]
				{
					setting.SilenceEcho.Value ? 18 : 19,
					2,
					3
				});
			});
		}
		if (setting.Goal.HasValue)
		{
			base.TaskHelper.Enqueue(delegate
			{
				SendEvent(new object[3]
				{
					20,
					setting.Goal.Value,
					0
				});
			});
		}
		if (setting.SetFinishRequirement.HasValue)
		{
			base.TaskHelper.Enqueue(delegate
			{
				SendEvent(new object[3]
				{
					20,
					setting.SetFinishRequirement.Value,
					2
				});
			});
			if (setting.SetFinishRequirement.Value != 1 && setting.FinishRequirement.HasValue)
			{
				base.TaskHelper.Enqueue(delegate
				{
					SendEvent(new object[3]
					{
						20,
						setting.FinishRequirement.Value,
						2
					});
				});
			}
		}
		if (setting.LootType.HasValue)
		{
			base.TaskHelper.Enqueue(delegate
			{
				SendEvent(new object[3]
				{
					20,
					setting.LootType.Value,
					4
				});
			});
		}
		if (setting.IsLocal.HasValue)
		{
			base.TaskHelper.Enqueue(delegate
			{
				SendEvent(new object[3]
				{
					22,
					setting.IsLocal.Value,
					0
				});
			});
		}
		if (setting.NoSameJob.HasValue)
		{
			base.TaskHelper.Enqueue(delegate
			{
				SendEvent(new object[3]
				{
					23,
					setting.NoSameJob.Value,
					0
				});
			});
		}
		if (setting.SetAllSlotsEveryone.HasValue)
		{
			base.TaskHelper.Enqueue(delegate
			{
				SendEvent(new object[3]
				{
					33,
					setting.SetAllSlotsEveryone.Value,
					0
				});
			});
		}
		if (setting.OnlyClassJob.HasValue)
		{
			base.TaskHelper.Enqueue(delegate
			{
				SendEvent(new object[3]
				{
					34,
					setting.OnlyClassJob.Value,
					0
				});
			});
		}
		if (setting.NoAudiences.HasValue)
		{
			base.TaskHelper.Enqueue(delegate
			{
				SendEvent(new object[3]
				{
					36,
					setting.NoAudiences.Value,
					0
				});
			});
		}
		if (base.TaskHelper.IsBusy)
		{
			base.TaskHelper.Enqueue(() => ((AtkUnitBase)Addons.LookingForGroupCondition).Close(true));
			base.TaskHelper.Enqueue(delegate
			{
				(*Addons.LookingForGroup).Callback(14);
			});
		}
		unsafe static void SendEvent(params object[] args)
		{
			((AgentId)130).SendEvent(3uL, args);
		}
	}
}
