using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using DailyRoutines.Common.Info;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Common.RemoteInteraction.Enums;
using DailyRoutines.Common.RemoteInteraction.Helpers;
using DailyRoutines.Common.RemoteInteraction.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using DailyRoutines.RemoteInteraction.PlayerInfo;
using DailyRoutines.RemoteInteraction.PlayerInfo.Models;
using DailyRoutines.RemoteInteraction.UsedNames;
using DailyRoutines.RemoteInteraction.UsedNames.Models;
using Dalamud.Game;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.Text.Evaluator;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using FFXIVClientStructs.FFXIV.Component.GUI;
using InteropGenerator.Runtime;
using KamiToolKit.BaseTypes;
using KamiToolKit.Enums;
using KamiToolKit.Interfaces;
using KamiToolKit.Nodes;
using Lumina.Data.Parsing.Uld;
using Lumina.Text;
using Lumina.Text.ReadOnly;
using OmenTools;
using OmenTools.Dalamud;
using OmenTools.Dalamud.Attributes;
using OmenTools.Dalamud.Services.Game.UI.Abstractions;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Info.Lumina;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.KamiToolKit.Addons.SelectYesno;
using OmenTools.KamiToolKit.Nodes;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;
using OmenTools.Threading.TaskHelper;
using TimeAgo;

namespace DailyRoutines.ModulesPublic.Interface;

public class OptimizedFriendList : ModuleBase
{
	private class Config : ModuleConfig
	{
		public ConcurrentDictionary<ulong, PlayerInfo> PlayerInfos = new ConcurrentDictionary<ulong, PlayerInfo>();

		public bool[] IgnoredGroup = new bool[8];

		public bool SearchName = true;

		public bool SearchNickname = true;

		public bool SearchRemark = true;
	}

	private sealed class QueryUsedNameMenuItem(OptimizedFriendList module) : ContextMenuEntry
	{
		public override string Identifier => "OptimizedFriendList";

		public unsafe override ContextMenuItem? Create(ContextMenuOpenedArgs args)
		{
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			if (args.AddonName != "FriendList")
			{
				return null;
			}
			ulong contentID = ((AgentFriendlist)AgentFriendlist.Instance()).SelectedContentId;
			if (contentID == 0L)
			{
				return null;
			}
			string name = args.TargetName;
			uint worldID = (uint)args.TargetHomeWorldID;
			if (string.IsNullOrEmpty(name) || !Sheets.Worlds.ContainsKey(worldID))
			{
				if (!TryGetPlayerInfoByContentID(contentID, out PlayerInfoValue playerInfo))
				{
					return null;
				}
				name = playerInfo.Name;
				worldID = playerInfo.WorldID;
			}
			return new ContextMenuItem
			{
				Name = ReadOnlySeString.op_Implicit(LanguageManager.Get("OptimizedFriendList-ContextMenu-QueryUsedNames")),
				OnClicked = delegate
				{
					module.usedNamesAddon?.Dispose();
					module.usedNamesAddon = DRFriendlistUsedNames.Open(contentID, name, LuminaWrapper.GetWorldName(worldID));
				}
			};
		}
	}

	private sealed class ModifyInfoMenuItem(OptimizedFriendList instance, TaskHelper taskHelper) : ContextMenuEntry
	{
		public override string Identifier => "OptimizedFriendList";

		public unsafe override ContextMenuItem? Create(ContextMenuOpenedArgs args)
		{
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			if (args.AddonName != "FriendList")
			{
				return null;
			}
			ulong contentID = ((AgentFriendlist)AgentFriendlist.Instance()).SelectedContentId;
			if (contentID == 0L)
			{
				return null;
			}
			string name = args.TargetName;
			uint worldID = (uint)args.TargetHomeWorldID;
			if (string.IsNullOrEmpty(name) || !Sheets.Worlds.ContainsKey(worldID))
			{
				if (!TryGetPlayerInfoByContentID(contentID, out PlayerInfoValue playerInfo))
				{
					return null;
				}
				name = playerInfo.Name;
				worldID = playerInfo.WorldID;
			}
			return new ContextMenuItem
			{
				Name = ReadOnlySeString.op_Implicit(LanguageManager.Get("OptimizedFriendList-ContextMenu-NicknameAndRemark")),
				OnClicked = delegate
				{
					instance.remarkEditAddon?.Dispose();
					instance.remarkEditAddon = DRFriendlistRemarkEdit.Open(instance, contentID, name, LuminaWrapper.GetWorldName(worldID));
					instance.ApplySearchFilter(instance.searchString, taskHelper);
				}
			};
		}
	}

	private sealed class TeleportFriendZoneMenuItem : ContextMenuEntry
	{
		public override string Identifier => "OptimizedFriendList";

		public unsafe override ContextMenuItem? Create(ContextMenuOpenedArgs args)
		{
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			if (args.AddonName != "FriendList")
			{
				return null;
			}
			CharacterData* targetCharacter = args.TargetCharacter;
			if (targetCharacter == null)
			{
				return null;
			}
			if (!TryGetAetheryteID(((CharacterData)targetCharacter).Location, out var aetheryteID))
			{
				return null;
			}
			return new ContextMenuItem
			{
				Name = ReadOnlySeString.op_Implicit(LanguageManager.Get("OptimizedFriendList-ContextMenu-TeleportToFriendZone")),
				OnClicked = delegate
				{
					((Telepo)Telepo.Instance()).Teleport(aetheryteID, (byte)0);
				}
			};
		}

		private static bool TryGetAetheryteID(uint zoneID, out uint aetheryteID)
		{
			aetheryteID = 0u;
			if (zoneID == 0 || zoneID == GameState.TerritoryType)
			{
				return false;
			}
			zoneID = zoneID switch
			{
				128u => 129u, 
				133u => 132u, 
				131u => 130u, 
				399u => 478u, 
				_ => zoneID, 
			};
			if (zoneID == GameState.TerritoryType)
			{
				return false;
			}
			aetheryteID = (from aetheryte in IOmenDalamudService<IAetheryteList>.Instance()
				where aetheryte.TerritoryID == zoneID
				select aetheryte.AetheryteID).FirstOrDefault();
			return aetheryteID != 0;
		}
	}

	private sealed class TeleportFriendWorldMenuItem : ContextMenuEntry
	{
		public override string Identifier => "OptimizedFriendList";

		public unsafe override ContextMenuItem? Create(ContextMenuOpenedArgs args)
		{
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			if (ManagerBase<ModuleManager>.Instance().IsModuleEnabled("FastWorldTravel") != true)
			{
				return null;
			}
			if (args.AddonName != "FriendList")
			{
				return null;
			}
			CharacterData* targetCharacter = args.TargetCharacter;
			if (targetCharacter == null)
			{
				return null;
			}
			uint targetWorldID = ((CharacterData)targetCharacter).CurrentWorld;
			if (targetWorldID == GameState.CurrentWorld)
			{
				return null;
			}
			return new ContextMenuItem
			{
				Name = ReadOnlySeString.op_Implicit(LanguageManager.Get("OptimizedFriendList-ContextMenu-TeleportToFriendWorld")),
				OnClicked = delegate
				{
					OmenServiceBase<ChatManager>.Instance().SendMessage("/pdr worldtravel " + LuminaWrapper.GetWorldName(targetWorldID));
				}
			};
		}
	}

	private class PlayerInfo
	{
		public ulong ContentID { get; set; }

		public string Name { get; set; } = string.Empty;

		public string Nickname { get; set; } = string.Empty;

		public string Remark { get; set; } = string.Empty;
	}

	private class DRFriendlistRemarkEdit(OptimizedFriendList instance) : NativeAddon
	{
		private TextButtonNode clearButtonNode;

		private TextButtonNode confirmButtonNode;

		private TextInputNode nicknameInputNode;

		private HorizontalLineNode headerLineNode;

		private TextNode nicknameNode;

		private TextNode playerNameNode;

		private TextMultiLineInputNode remarkInputNode;

		private TextNode remarkNode;

		private ulong ContentID { get; set; }

		private string Name { get; set; } = string.Empty;

		private string WorldName { get; set; } = string.Empty;

		private OptimizedFriendList Instance { get; } = instance;

		private SelectYesnoAddon? ClearConfirmationInstance { get; set; }

		public unsafe static DRFriendlistRemarkEdit Open(OptimizedFriendList owner, ulong contentID, string name, string worldName)
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			int num = ((Addons.FriendList != null) ? ((AtkUnitBase)Addons.FriendList).Id : 0);
			DRFriendlistRemarkEdit obj = new DRFriendlistRemarkEdit(owner)
			{
				InternalName = "DRFriendlistRemarkEdit",
				Title = ReadOnlySeString.op_Implicit(LanguageManager.Get("OptimizedFriendList-Addon-Title")),
				Size = new Vector2(460f, 304f),
				ContentID = contentID,
				Name = name,
				WorldName = worldName,
				ParentAddonId = num,
				BlockedParentAddonId = num
			};
			obj.Open();
			return obj;
		}

		protected unsafe override void OnSetup(AtkUnitBase* addon, Span<AtkValue> atkValues)
		{
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_037a: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
			if (ContentID == 0L || string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(WorldName))
			{
				Close();
				return;
			}
			string nickname = Instance.config.PlayerInfos.GetValueOrDefault(ContentID, new PlayerInfo()).Nickname;
			string remark = Instance.config.PlayerInfos.GetValueOrDefault(ContentID, new PlayerInfo()).Remark;
			RentedSeStringBuilder val = default(RentedSeStringBuilder);
			((RentedSeStringBuilder)(ref val))._002Ector();
			try
			{
				((RentedSeStringBuilder)(ref val)).Builder.Append(Name).AppendIcon(88u).Append(WorldName);
				playerNameNode = new TextNode
				{
					Position = new Vector2(12f, 42f),
					Size = new Vector2(436f, 26f),
					String = ((RentedSeStringBuilder)(ref val)).Builder.ToReadOnlySeString(),
					FontSize = 20u,
					AlignmentType = (AlignmentType)3,
					TextFlags = (TextFlags)2
				};
				playerNameNode.AttachNode(this);
				headerLineNode = new HorizontalLineNode
				{
					Position = new Vector2(12f, 72f),
					Size = new Vector2(436f, 2f)
				};
				headerLineNode.AttachNode(this);
				nicknameNode = new TextNode
				{
					Position = new Vector2(12f, 78f),
					Size = new Vector2(100f, 20f),
					TextId = 15207u,
					SheetType = (SheetType)0,
					FontSize = 14u,
					AlignmentType = (AlignmentType)3,
					TextFlags = (TextFlags)2
				};
				nicknameNode.AttachNode(this);
				nicknameInputNode = new TextInputNode
				{
					Position = new Vector2(12f, 100f),
					Size = new Vector2(436f, 26f),
					MaxCharacters = 64,
					ShowLimitText = true,
					AutoSelectAll = false,
					String = ReadOnlySeString.op_Implicit(nickname)
				};
				nicknameInputNode.AttachNode(this);
				remarkNode = new TextNode
				{
					Position = new Vector2(12f, 132f),
					Size = new Vector2(100f, 20f),
					String = ReadOnlySeString.op_Implicit(LanguageManager.Get("Remark")),
					FontSize = 14u,
					AlignmentType = (AlignmentType)3,
					TextFlags = (TextFlags)2
				};
				remarkNode.AttachNode(this);
				remarkInputNode = new TextMultiLineInputNode
				{
					Position = new Vector2(12f, 154f),
					MaxCharacters = 1024,
					MaxLines = 5u,
					ShowLimitText = true,
					AutoSelectAll = false,
					String = ReadOnlySeString.op_Implicit(remark)
				};
				remarkInputNode.Flags |= TextInputFlags.MultiLine;
				remarkInputNode.Size = new Vector2(436f, remarkInputNode.CurrentTextNode.LineSpacing * 5 + 26);
				remarkInputNode.AttachNode(this);
				confirmButtonNode = new TextButtonNode
				{
					Position = new Vector2(308f, 254f),
					Size = new Vector2(140f, 36f),
					TextureType = ButtonTextureType.ButtonB,
					String = ReadOnlySeString.op_Implicit(LanguageManager.Get("Confirm")),
					OnClick = delegate
					{
						//IL_0053: Unknown result type (might be due to invalid IL or missing references)
						//IL_0058: Unknown result type (might be due to invalid IL or missing references)
						//IL_0077: Unknown result type (might be due to invalid IL or missing references)
						//IL_007c: Unknown result type (might be due to invalid IL or missing references)
						Instance.config.PlayerInfos[ContentID] = new PlayerInfo
						{
							ContentID = ContentID,
							Name = Name,
							Nickname = ((object)nicknameInputNode.String/*cast due to constrained. prefix*/).ToString(),
							Remark = ((object)remarkInputNode.String/*cast due to constrained. prefix*/).ToString()
						};
						Instance.config.Save(Instance);
						((InfoProxyFriendList)InfoProxyFriendList.Instance()).RequestData();
						Close();
					}
				};
				confirmButtonNode.AttachNode(this);
				clearButtonNode = new TextButtonNode
				{
					Position = new Vector2(180f, 258f),
					Size = new Vector2(120f, 28f),
					String = ReadOnlySeString.op_Implicit(LanguageManager.Get("Clear")),
					OnClick = delegate
					{
						//IL_002c: Unknown result type (might be due to invalid IL or missing references)
						ClearConfirmationInstance?.Dispose();
						ClearConfirmationInstance = SelectYesnoAddon.Open(new SelectYesnoAddonOptions
						{
							Prompt = LanguageManager.GetSe("OptimizedFriendList-Addon-ConfirmClear"),
							Callback = delegate(SelectYesnoAddon _, SelectYesnoAddonResult result)
							{
								ClearConfirmationInstance = null;
								if (result == SelectYesnoAddonResult.Yes)
								{
									Instance.config.PlayerInfos.TryRemove(ContentID, out PlayerInfo _);
									Instance.config.Save(Instance);
									((InfoProxyFriendList)InfoProxyFriendList.Instance()).RequestData();
									Close();
								}
							},
							Position = new AddonPosition((*((AtkUnitBase)addon).RootNode).GetNodeState().Center, AddonPositionAlignment.TopCenter),
							BlockedParentID = ((AtkUnitBase)addon).Id,
							ParentID = ((AtkUnitBase)addon).Id
						});
					}
				};
				clearButtonNode.AttachNode(this);
			}
			finally
			{
				((IDisposable)(*(RentedSeStringBuilder*)(&val))/*cast due to constrained. prefix*/).Dispose();
			}
		}

		protected unsafe override void OnUpdate(AtkUnitBase* addon)
		{
			if (!(*Addons.FriendList).IsAddonAndNodesReady())
			{
				Close();
			}
		}

		protected unsafe override void OnFinalize(AtkUnitBase* addon)
		{
			ContentID = 0uL;
			Name = string.Empty;
			WorldName = string.Empty;
			ClearConfirmationInstance?.Dispose();
			ClearConfirmationInstance = null;
		}
	}

	private class DRFriendlistSearchSetting(OptimizedFriendList instance, TaskHelper taskHelper) : NativeAddon
	{
		private OptimizedFriendList Instance { get; init; } = instance;

		private TaskHelper TaskHelper { get; init; } = taskHelper;

		public unsafe static DRFriendlistSearchSetting Open(OptimizedFriendList owner, TaskHelper taskHelper)
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			int num = ((Addons.FriendList != null) ? ((AtkUnitBase)Addons.FriendList).Id : 0);
			DRFriendlistSearchSetting obj = new DRFriendlistSearchSetting(owner, taskHelper)
			{
				InternalName = "DRFriendlistSearchSetting",
				Title = ReadOnlySeString.op_Implicit(LanguageManager.Get("OptimizedFriendList-Addon-SearchSetting")),
				Size = new Vector2(230f, 350f),
				ParentAddonId = num,
				BlockedParentAddonId = num
			};
			obj.Open();
			return obj;
		}

		protected unsafe override void OnSetup(AtkUnitBase* addon, Span<AtkValue> atkValues)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02be: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			TextNode textNode = new TextNode
			{
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("OptimizedFriendList-SearchType")),
				FontSize = 16u,
				TextFlags = (TextFlags)1,
				Position = new Vector2(10f, 42f)
			};
			textNode.AttachNode(this);
			VerticalListNode verticalListNode = new VerticalListNode
			{
				Position = new Vector2(20f, textNode.Position.Y + 28f),
				Alignment = VerticalListAlignment.Left
			};
			CheckboxNode checkboxNode = new CheckboxNode
			{
				Size = new Vector2(80f, 20f),
				IsChecked = Instance.config.SearchName,
				IsEnabled = true,
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("Name")),
				OnClick = delegate(bool newState)
				{
					Instance.config.SearchName = newState;
					Instance.config.Save(Instance);
					Instance.ApplySearchFilter(Instance.searchString, TaskHelper);
				}
			};
			verticalListNode.Height += textNode.Height;
			CheckboxNode checkboxNode2 = new CheckboxNode
			{
				Size = new Vector2(80f, 20f),
				IsChecked = Instance.config.SearchNickname,
				IsEnabled = true,
				String = ReadOnlySeString.op_Implicit(LuminaWrapper.GetAddonText(15207u)),
				OnClick = delegate(bool newState)
				{
					Instance.config.SearchNickname = newState;
					Instance.config.Save(Instance);
					Instance.ApplySearchFilter(Instance.searchString, TaskHelper);
				}
			};
			verticalListNode.Height += checkboxNode2.Height;
			CheckboxNode checkboxNode3 = new CheckboxNode
			{
				Size = new Vector2(80f, 20f),
				IsChecked = Instance.config.SearchRemark,
				IsEnabled = true,
				String = ReadOnlySeString.op_Implicit(LuminaWrapper.GetAddonText(13294u).TrimEnd(':')),
				OnClick = delegate(bool newState)
				{
					Instance.config.SearchRemark = newState;
					Instance.config.Save(Instance);
					Instance.ApplySearchFilter(Instance.searchString, TaskHelper);
				}
			};
			verticalListNode.Height += checkboxNode3.Height;
			verticalListNode.AddNode(new global::_003C_003Ez__ReadOnlyArray<NodeBase>(new NodeBase[3] { checkboxNode, checkboxNode2, checkboxNode3 }));
			verticalListNode.AttachNode(this);
			TextNode textNode2 = new TextNode
			{
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("OptimizedFriendList-SearchIgnoreGroup")),
				FontSize = 16u,
				TextFlags = (TextFlags)1,
				Position = new Vector2(10f, verticalListNode.Position.Y + verticalListNode.Height + 12f)
			};
			textNode2.AttachNode(this);
			VerticalListNode verticalListNode2 = new VerticalListNode
			{
				Position = new Vector2(20f, textNode2.Position.Y + 28f),
				Alignment = VerticalListAlignment.Left
			};
			for (int num = 0; num < 8; num++)
			{
				int index = num;
				ISeStringEvaluator obj = DalamudServiceExtension.Instance<ISeStringEvaluator>();
				SeStringParameter reference = SeStringParameter.op_Implicit(index + 1);
				ReadOnlySeString val = obj.EvaluateFromAddon(12925u, new Span<SeStringParameter>(ref reference), (ClientLanguage?)null);
				CheckboxNode checkboxNode4 = new CheckboxNode
				{
					Size = new Vector2(80f, 20f),
					IsChecked = Instance.config.IgnoredGroup[num],
					IsEnabled = true,
					String = val,
					OnClick = delegate(bool newState)
					{
						Instance.config.IgnoredGroup[index] = newState;
						Instance.config.Save(Instance);
						Instance.ApplySearchFilter(Instance.searchString, TaskHelper);
					}
				};
				verticalListNode2.Height += checkboxNode4.Height;
				verticalListNode2.AddNode(checkboxNode4);
			}
			verticalListNode2.AttachNode(this);
		}

		protected unsafe override void OnUpdate(AtkUnitBase* addon)
		{
			if (Addons.FriendList == null)
			{
				Close();
			}
		}
	}

	private sealed class DRFriendlistUsedNames : NativeAddon
	{
		private sealed class UsedNamesEntry(string oldName, string newName, int year, int month, int day, int hour, int minute)
		{
			public string ChangedTimeText { get; } = new DateTime(year, month, day, hour, minute, 0).ToString("yyyy/MM/dd HH:mm");

			public string ChangedTimeTooltip { get; } = new DateTime(year, month, day, hour, minute, 0).TimeAgo();

			public string OldName { get; } = oldName;

			public string NewName { get; } = newName;

			public UsedNamesEntry(UsedNamesChange change)
				: this(change.BeforeName, change.AfterName, change.ChangedTime.Year, change.ChangedTime.Month, change.ChangedTime.Day, change.ChangedTime.Hour, change.ChangedTime.Minute)
			{
			}
		}

		private sealed class UsedNamesListNode : ListNode<UsedNamesEntry, UsedNamesItemNode>
		{
		}

		private sealed class UsedNamesItemNode : ListItemNode<UsedNamesEntry>, IListItemNode
		{
			private readonly TextNode oldNameNode;

			private readonly TextNode newNameNode;

			private readonly TextNode timeNode;

			public static float ItemHeight => 25f;

			public UsedNamesItemNode()
			{
				oldNameNode = new TextNode
				{
					Position = new Vector2(0f, 0f),
					Size = new Vector2(192f, ItemHeight),
					FontSize = 14u,
					TextFlags = (TextFlags)0
				};
				AtkColors.ListRow.ApplyTo(oldNameNode);
				oldNameNode.AttachNode(this);
				newNameNode = new TextNode
				{
					Position = new Vector2(192f, 0f),
					Size = new Vector2(192f, ItemHeight),
					FontSize = 14u,
					TextFlags = (TextFlags)0
				};
				AtkColors.ListRow.ApplyTo(newNameNode);
				newNameNode.AttachNode(this);
				timeNode = new TextNode
				{
					Position = new Vector2(384f, 0f),
					Size = new Vector2(128f, ItemHeight),
					FontSize = 14u,
					TextFlags = (TextFlags)0
				};
				AtkColors.ListRow.ApplyTo(timeNode);
				timeNode.AttachNode(this);
				base.EnableSelection = false;
			}

			protected override void OnSizeChanged()
			{
				base.OnSizeChanged();
				NineGridNode[] array = new NineGridNode[2] { base.SelectedBackgroundNode, base.HoveredBackgroundNode };
				foreach (NineGridNode obj in array)
				{
					obj.Position = new Vector2(3f, -1f);
					obj.Size = new Vector2(Width - 6f, ItemHeight - 3f);
					obj.TopOffset = 0f;
					obj.BottomOffset = 0f;
					obj.LeftOffset = 16f;
					obj.RightOffset = 1f;
				}
			}

			protected override void SetNodeData(UsedNamesEntry itemData)
			{
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				//IL_0038: Unknown result type (might be due to invalid IL or missing references)
				//IL_004e: Unknown result type (might be due to invalid IL or missing references)
				oldNameNode.String = ReadOnlySeString.op_Implicit(itemData.OldName);
				newNameNode.String = ReadOnlySeString.op_Implicit(itemData.NewName);
				timeNode.String = ReadOnlySeString.op_Implicit(itemData.ChangedTimeText);
				timeNode.TextTooltip = ReadOnlySeString.op_Implicit(itemData.ChangedTimeTooltip);
			}
		}

		public const float WINDOW_WIDTH = 560f;

		public const float WINDOW_HEIGHT = 480f;

		private const float HEADER_HEIGHT = 34f;

		private const float COLUMN_HEADER_HEIGHT = 20f;

		private const float DIVIDER_HEIGHT = 8f;

		private const float DIVIDER_SPACING = 2f;

		private const float COLUMN_OLD_NAME_WIDTH = 192f;

		private const float COLUMN_NEW_NAME_WIDTH = 192f;

		private const float COLUMN_TIME_WIDTH = 128f;

		private const float TIME_X = 384f;

		private static readonly (string TextKey, float X, float Width, AlignmentType Alignment)[] COLUMN_HEADERS = new(string, float, float, AlignmentType)[3]
		{
			("OptimizedFriendList-UsedNames-OldName", 0f, 192f, (AlignmentType)3),
			("OptimizedFriendList-UsedNames-NewName", 192f, 192f, (AlignmentType)3),
			("OptimizedFriendList-UsedNames-Time", 384f, 128f, (AlignmentType)5)
		};

		private IDisposable? observation;

		private UsedNamesListNode? listNode;

		public string PlayerName { get; private set; } = string.Empty;

		public string WorldName { get; private set; } = string.Empty;

		private ulong ContentID { get; set; }

		public unsafe static DRFriendlistUsedNames Open(ulong contentID, string playerName, string worldName)
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			int num = ((Addons.FriendList != null) ? ((AtkUnitBase)Addons.FriendList).Id : 0);
			DRFriendlistUsedNames obj = new DRFriendlistUsedNames
			{
				InternalName = "DRFriendlistUsedNames",
				Title = ReadOnlySeString.op_Implicit(LanguageManager.Get("OptimizedFriendList-Addon-UsedNames")),
				Size = new Vector2(560f, 480f),
				ContentID = contentID,
				PlayerName = playerName,
				WorldName = worldName,
				ParentAddonId = num,
				BlockedParentAddonId = num
			};
			obj.Open();
			return obj;
		}

		protected unsafe override void OnSetup(AtkUnitBase* addon, Span<AtkValue> atkValues)
		{
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			if (ContentID == 0L || string.IsNullOrWhiteSpace(PlayerName))
			{
				Close();
				return;
			}
			Vector2 contentStartPosition = base.ContentStartPosition;
			Vector2 contentSize = base.ContentSize;
			RentedSeStringBuilder val = default(RentedSeStringBuilder);
			((RentedSeStringBuilder)(ref val))._002Ector();
			try
			{
				((RentedSeStringBuilder)(ref val)).Builder.Append(PlayerName).AppendIcon(88u).Append(WorldName);
				TextNode obj = new TextNode
				{
					Position = contentStartPosition
				};
				Vector2 size = contentSize;
				size.Y = 28f;
				obj.Size = size;
				obj.FontSize = 20u;
				obj.String = ((RentedSeStringBuilder)(ref val)).Builder.ToReadOnlySeString();
				obj.AlignmentType = (AlignmentType)4;
				obj.TextFlags = (TextFlags)1024;
				obj.AttachNode(this);
				(string, float, float, AlignmentType)[] cOLUMN_HEADERS = COLUMN_HEADERS;
				for (int i = 0; i < cOLUMN_HEADERS.Length; i++)
				{
					(string, float, float, AlignmentType) tuple = cOLUMN_HEADERS[i];
					string item = tuple.Item1;
					float item2 = tuple.Item2;
					float item3 = tuple.Item3;
					AlignmentType item4 = tuple.Item4;
					TextNode textNode = new TextNode
					{
						Position = contentStartPosition + new Vector2(item2, 34f),
						Size = new Vector2(item3, 20f),
						FontSize = 12u,
						String = ReadOnlySeString.op_Implicit(LanguageManager.Get(item)),
						AlignmentType = item4,
						TextFlags = (TextFlags)0
					};
					AtkColors.ListHeader.ApplyTo(textNode);
					textNode.AttachNode(this);
				}
				HorizontalDashedLineNode obj2 = new HorizontalDashedLineNode
				{
					Position = contentStartPosition + new Vector2(0f, 54f)
				};
				size = contentSize;
				size.Y = 8f;
				obj2.Size = size;
				obj2.AttachNode(this);
				UsedNamesListNode obj3 = new UsedNamesListNode
				{
					Position = contentStartPosition + new Vector2(0f, 64f)
				};
				size = contentSize;
				size.Y = contentSize.Y - 64f;
				obj3.Size = size;
				obj3.AutoResetScroll = false;
				obj3.OptionsList = new List<UsedNamesEntry>();
				listNode = obj3;
				listNode.AttachNode(this);
				CircleButtonNode circleButtonNode = new CircleButtonNode();
				circleButtonNode.Position = new Vector2(490f, 5f);
				circleButtonNode.Size = new Vector2(28f);
				circleButtonNode.Icon = CircleButtonIcon.Refresh;
				circleButtonNode.TextTooltip = ReadOnlySeString.op_Implicit(LanguageManager.Get("Refresh"));
				circleButtonNode.OnClick = Refresh;
				circleButtonNode.AttachNode(this);
				WindowNode obj4 = (WindowNode)base.WindowNode;
				obj4.ShowConfigButton = false;
				obj4.ShowHelpButton = false;
				WorldRegion region = WorldRegionResolver.Resolve(GameState.HomeWorld);
				observation = RemoteUsedNames.Observe(ContentID, region, BuildEntries);
				RemoteUsedNames.GetOrRequest(ContentID, region);
				if (RemoteUsedNames.TryGet(ContentID, region, out List<UsedNamesChange> value))
				{
					BuildEntries(new RemoteSnapshot<List<UsedNamesChange>>(RemoteSnapshotStatus.Ready, value, DateTime.MinValue, null));
				}
			}
			finally
			{
				((IDisposable)(*(RentedSeStringBuilder*)(&val))/*cast due to constrained. prefix*/).Dispose();
			}
		}

		protected unsafe override void OnUpdate(AtkUnitBase* addon)
		{
			if (!(*Addons.FriendList).IsAddonAndNodesReady())
			{
				Close();
			}
		}

		protected unsafe override void OnFinalize(AtkUnitBase* addon)
		{
			observation?.Dispose();
			observation = null;
			listNode = null;
			ContentID = 0uL;
			PlayerName = string.Empty;
			WorldName = string.Empty;
		}

		private void Refresh()
		{
			WorldRegion region = WorldRegionResolver.Resolve(GameState.HomeWorld);
			RemoteUsedNames.Invalidate(ContentID, region);
			RemoteUsedNames.GetOrRequest(ContentID, region);
		}

		private void BuildEntries(RemoteSnapshot<List<UsedNamesChange>> snapshot)
		{
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			if (listNode != null)
			{
				if (snapshot.Error != null)
				{
					DLog.Error("获取好友曾用名时发生错误", snapshot.Error);
				}
				List<UsedNamesEntry> optionsList = (from change in snapshot.Value?.OrderByDescending((UsedNamesChange change) => change.ChangedTime)
					select new UsedNamesEntry(change)).ToList() ?? new List<UsedNamesEntry>();
				TextNode noResultsTextNode = listNode.NoResultsTextNode;
				string text;
				switch (snapshot.Status)
				{
				case RemoteSnapshotStatus.Empty:
				case RemoteSnapshotStatus.Loading:
					text = LanguageManager.Get("OptimizedFriendList-UsedNames-Querying");
					break;
				case RemoteSnapshotStatus.Failed:
					text = LanguageManager.Get("OptimizedFriendList-UsedNames-QueryFailed");
					break;
				default:
					text = LanguageManager.Get("OptimizedFriendList-UsedNames-NotFound");
					break;
				}
				noResultsTextNode.String = ReadOnlySeString.op_Implicit(text);
				listNode.OptionsList = optionsList;
				listNode.ResetScroll();
			}
		}
	}

	private Config config;

	private ModifyInfoMenuItem modifyInfoItem;

	private QueryUsedNameMenuItem queryUsedNameMenuItem;

	private TeleportFriendZoneMenuItem teleportZoneItem;

	private TeleportFriendWorldMenuItem teleportWorldItem;

	private TextInputNode? searchInputNode;

	private TextureButtonNode? searchSettingButtonNode;

	private DRFriendlistRemarkEdit? remarkEditAddon;

	private DRFriendlistSearchSetting? searchSettingAddon;

	private DRFriendlistUsedNames? usedNamesAddon;

	private string searchString = string.Empty;

	private readonly List<IDisposable> infoTokens = new List<IDisposable>();

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("OptimizedFriendListTitle"),
		Description = LanguageManager.Get("OptimizedFriendListDescription"),
		Category = ModuleCategory.Interface,
		ModulesPrerequisite = new string[1] { "FastWorldTravel" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected unsafe override void Init()
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		if (base.TaskHelper == null)
		{
			TaskHelper taskHelper = (base.TaskHelper = new TaskHelper());
		}
		modifyInfoItem = new ModifyInfoMenuItem(this, base.TaskHelper);
		queryUsedNameMenuItem = new QueryUsedNameMenuItem(this);
		teleportZoneItem = new TeleportFriendZoneMenuItem();
		teleportWorldItem = new TeleportFriendWorldMenuItem();
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)1, "FriendList", new AddonEventDelegate(OnAddon));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)7, "FriendList", new AddonEventDelegate(OnAddon));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, "FriendList", new AddonEventDelegate(OnAddon));
		if ((*Addons.FriendList).IsAddonAndNodesReady())
		{
			OnAddon((AddonEvent)1, null);
		}
		OmenServiceBase<ContextMenuManager>.Instance().Reg(modifyInfoItem);
		OmenServiceBase<ContextMenuManager>.Instance().Reg(queryUsedNameMenuItem);
		OmenServiceBase<ContextMenuManager>.Instance().Reg(teleportZoneItem);
		OmenServiceBase<ContextMenuManager>.Instance().Reg(teleportWorldItem);
	}

	protected unsafe override void Uninit()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected O, but got Unknown
		OmenServiceBase<ContextMenuManager>.Instance().Unreg(modifyInfoItem);
		OmenServiceBase<ContextMenuManager>.Instance().Unreg(queryUsedNameMenuItem);
		OmenServiceBase<ContextMenuManager>.Instance().Unreg(teleportZoneItem);
		OmenServiceBase<ContextMenuManager>.Instance().Unreg(teleportWorldItem);
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddon });
		searchInputNode?.Dispose();
		searchInputNode = null;
		searchSettingButtonNode?.Dispose();
		searchSettingButtonNode = null;
		foreach (IDisposable infoToken in infoTokens)
		{
			infoToken.Dispose();
		}
		infoTokens.Clear();
		remarkEditAddon?.Dispose();
		remarkEditAddon = null;
		searchSettingAddon?.Dispose();
		searchSettingAddon = null;
		usedNamesAddon?.Dispose();
		usedNamesAddon = null;
		if ((*Addons.FriendList).IsAddonAndNodesReady())
		{
			((InfoProxyFriendList)InfoProxyFriendList.Instance()).RequestData();
		}
	}

	private unsafe void OnAddon(AddonEvent type, AddonArgs? args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		if ((int)type != 1)
		{
			if ((int)type != 6)
			{
				if ((int)type == 7)
				{
					ApplySearchFilter(searchString, base.TaskHelper);
					ApplyDisplayModification(base.TaskHelper);
				}
			}
			else
			{
				searchInputNode = null;
				searchSettingButtonNode = null;
				infoTokens.Clear();
			}
			return;
		}
		if (Addons.FriendList != null)
		{
			if (searchInputNode == null)
			{
				searchInputNode = new TextInputNode
				{
					Position = new Vector2(10f, 425f),
					Size = new Vector2(200f, 35f),
					MaxCharacters = 20,
					ShowLimitText = true,
					OnInputReceived = delegate(ReadOnlySeString x)
					{
						searchString = ((object)(*(ReadOnlySeString*)(&x))/*cast due to constrained. prefix*/).ToString();
						ApplySearchFilter(searchString, base.TaskHelper);
					},
					OnInputComplete = delegate(ReadOnlySeString x)
					{
						searchString = ((object)(*(ReadOnlySeString*)(&x))/*cast due to constrained. prefix*/).ToString();
						ApplySearchFilter(searchString, base.TaskHelper);
					}
				};
			}
			searchInputNode.CursorNode.ScaleY = 1.4f;
			searchInputNode.CurrentTextNode.FontSize = 14u;
			searchInputNode.CurrentTextNode.Y += 3f;
			searchInputNode.AttachNode(((AtkUnitBase)Addons.FriendList).GetNodeById(20u));
			if (searchSettingButtonNode == null)
			{
				searchSettingButtonNode = new TextureButtonNode
				{
					Position = new Vector2(215f, 430f),
					Size = new Vector2(25f, 25f),
					IsChecked = config.SearchName,
					IsEnabled = true,
					TexturePath = "ui/uld/CircleButtons_hr1.tex",
					TextureSize = new Vector2(28f, 28f),
					OnClick = delegate
					{
						DRFriendlistSearchSetting dRFriendlistSearchSetting = searchSettingAddon;
						if (dRFriendlistSearchSetting != null && dRFriendlistSearchSetting.IsOpen)
						{
							dRFriendlistSearchSetting.Close();
						}
						else
						{
							DRFriendlistSearchSetting dRFriendlistSearchSetting2 = searchSettingAddon;
							if (dRFriendlistSearchSetting2 != null)
							{
								dRFriendlistSearchSetting2.Dispose();
								searchSettingAddon = null;
							}
							searchSettingAddon = DRFriendlistSearchSetting.Open(this, base.TaskHelper);
						}
					}
				};
			}
			searchSettingButtonNode.AttachNode(((AtkUnitBase)Addons.FriendList).GetNodeById(20u));
			searchString = string.Empty;
		}
		if (Throttler.Shared.Throttle("OptimizedFriendList-OnRequestFriendList", 10000u))
		{
			AgentFriendlist* agent = AgentFriendlist.Instance();
			if (agent == null)
			{
				return;
			}
			InfoProxyFriendList* ptr = InfoProxyFriendList.Instance();
			if (ptr == null || ((InfoProxyFriendList)ptr).EntryCount == 0)
			{
				return;
			}
			int num = 0;
			for (int num2 = 0; num2 < ((InfoProxyFriendList)ptr).CharDataSpan.Length; num2++)
			{
				CharacterData chara = ((InfoProxyFriendList)ptr).CharDataSpan.get_Item(num2);
				if (chara.ContentId == 0L)
				{
					continue;
				}
				DalamudServiceExtension.Instance<IFramework>().RunOnTick((Action)delegate
				{
					if (Addons.FriendList != null)
					{
						((AgentFriendlist)agent).RequestFriendInfo(chara.ContentId);
					}
				}, TimeSpan.FromMilliseconds(10 * num), 0, default(CancellationToken));
				num++;
			}
			if (num > 0)
			{
				DalamudServiceExtension.Instance<IFramework>().RunOnTick((Action)delegate
				{
					if (Addons.FriendList != null)
					{
						ApplyDisplayModification(base.TaskHelper);
					}
				}, TimeSpan.FromMilliseconds(10 * (num + 1)), 0, default(CancellationToken));
			}
		}
		ApplyDisplayModification(base.TaskHelper);
	}

	[IPCProvider("DailyRoutines.Modules.OptimizedFriendlist.GetRemarkByContentID")]
	private string GetRemarkByContentID(ulong contentID)
	{
		if (!config.PlayerInfos.TryGetValue(contentID, out PlayerInfo value))
		{
			return string.Empty;
		}
		if (string.IsNullOrWhiteSpace(value.Remark))
		{
			return string.Empty;
		}
		return value.Remark;
	}

	[IPCProvider("DailyRoutines.Modules.OptimizedFriendlist.GetNicknameByContentID")]
	private string GetNicknameByContentID(ulong contentID)
	{
		if (!config.PlayerInfos.TryGetValue(contentID, out PlayerInfo value))
		{
			return string.Empty;
		}
		if (string.IsNullOrWhiteSpace(value.Nickname))
		{
			return string.Empty;
		}
		return value.Nickname;
	}

	private unsafe static void ReplaceAtkString(int index, ReadOnlySeString str)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		Utf8String val = default(Utf8String);
		((Utf8String)(ref val))._002Ector(ReadOnlySeString.op_Implicit(str));
		try
		{
			((StringArrayData)((AtkStage)AtkStage.Instance()).GetStringArrayData((StringArrayType)16)).SetValue(index, val.StringPtr, true, true, false);
		}
		finally
		{
			((IDisposable)(*(Utf8String*)(&val))/*cast due to constrained. prefix*/).Dispose();
		}
	}

	private unsafe void ApplyDisplayModification(TaskHelper? taskHelper)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		if (!(*Addons.FriendList).IsAddonAndNodesReady())
		{
			return;
		}
		InfoProxyFriendList* ptr = InfoProxyFriendList.Instance();
		bool flag = false;
		RentedSeStringBuilder val2 = default(RentedSeStringBuilder);
		for (int i = 0; i < ((InfoProxyFriendList)ptr).EntryCount; i++)
		{
			CharacterData val = ((InfoProxyFriendList)ptr).CharDataSpan.get_Item(i);
			string text = ((object)(*(CStringPointer*)((byte*)((StringArrayData)((AtkStage)AtkStage.Instance()).GetStringArrayData((StringArrayType)16)).StringArray + (nint)(5 * i) * (nint)Unsafe.SizeOf<CStringPointer>()))/*cast due to constrained. prefix*/).ToString();
			if (text == LuminaWrapper.GetAddonText(964u))
			{
				flag = true;
				RestoreEntryData(i, val.ContentId, taskHelper);
			}
			if (!config.PlayerInfos.TryGetValue(val.ContentId, out PlayerInfo value))
			{
				continue;
			}
			if (!string.IsNullOrWhiteSpace(value.Nickname) && text != value.Nickname)
			{
				flag = true;
				((RentedSeStringBuilder)(ref val2))._002Ector();
				try
				{
					SeStringBuilder val3 = ((RentedSeStringBuilder)(ref val2)).Builder.PushColorType(37u);
					SeStringInterpolatedStringHandler val4 = new SeStringInterpolatedStringHandler(0, 1, val3);
					((SeStringInterpolatedStringHandler)(ref val4)).AppendFormatted<string>(value.Nickname);
					val3.Append(val4).PopColorType();
					ReplaceAtkString(5 * i, ((RentedSeStringBuilder)(ref val2)).Builder.ToReadOnlySeString());
				}
				finally
				{
					((IDisposable)(*(RentedSeStringBuilder*)(&val2))/*cast due to constrained. prefix*/).Dispose();
				}
			}
			string text2 = ((object)(*(CStringPointer*)((byte*)((StringArrayData)((AtkStage)AtkStage.Instance()).GetStringArrayData((StringArrayType)16)).StringArray + (nint)(3 + 5 * i) * (nint)Unsafe.SizeOf<CStringPointer>()))/*cast due to constrained. prefix*/).ToString();
			if (!string.IsNullOrWhiteSpace(value.Remark))
			{
				string text3 = LanguageManager.Get("Remark") + ": " + value.Remark + (string.IsNullOrWhiteSpace(value.Nickname) ? string.Empty : ("\n" + LuminaWrapper.GetAddonText(9818u) + ": " + ((CharacterData)(ref val)).NameString));
				if (!(text3 == text2))
				{
					flag = true;
					ReplaceAtkString(3 + 5 * i, ReadOnlySeString.op_Implicit(text3));
				}
			}
		}
		if (flag && taskHelper != null)
		{
			RequestInfoUpdate(taskHelper);
		}
	}

	private unsafe static void RequestInfoUpdate(TaskHelper taskHelper)
	{
		taskHelper.Abort();
		if (Addons.FriendList == null)
		{
			return;
		}
		taskHelper.Enqueue(delegate
		{
			if (Addons.FriendList != null)
			{
				((AtkUnitBase)Addons.FriendList).OnRequestedUpdate(((AtkStage)AtkStage.Instance()).GetNumberArrayData(), ((AtkStage)AtkStage.Instance()).GetStringArrayData());
			}
		});
		taskHelper.DelayNext(100);
		taskHelper.Enqueue(delegate
		{
			if (Addons.FriendList != null)
			{
				((AtkUnitBase)Addons.FriendList).OnRequestedUpdate(((AtkStage)AtkStage.Instance()).GetNumberArrayData(), ((AtkStage)AtkStage.Instance()).GetStringArrayData());
			}
		});
	}

	private bool MatchesSearch(string filter)
	{
		if (string.IsNullOrWhiteSpace(searchString))
		{
			return true;
		}
		if (string.IsNullOrWhiteSpace(filter))
		{
			return false;
		}
		if (searchString.StartsWith('^'))
		{
			return filter.StartsWith(searchString.Substring(1), StringComparison.InvariantCultureIgnoreCase);
		}
		if (searchString.EndsWith('$'))
		{
			string text = searchString;
			return filter.EndsWith(text.Substring(0, text.Length - 1), StringComparison.InvariantCultureIgnoreCase);
		}
		return filter.Contains(searchString, StringComparison.InvariantCultureIgnoreCase);
	}

	protected unsafe void ApplySearchFilter(string filter, TaskHelper? taskHelper)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Invalid comparison between Unknown and I4
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		InfoProxyFriendList* ptr = InfoProxyFriendList.Instance();
		if (string.IsNullOrWhiteSpace(filter))
		{
			((InfoProxyFriendList)ptr).ApplyFilters();
			return;
		}
		Dictionary<ulong, uint> dictionary = new Dictionary<ulong, uint>();
		DisplayGroup filterGroup = ((InfoProxyFriendList)ptr).FilterGroup;
		Unsafe.Write(&((InfoProxyFriendList)ptr).FilterGroup, (DisplayGroup)0);
		uint entryCount = ((InfoProxyFriendList)ptr).GetEntryCount();
		for (int i = 0; i < entryCount; i++)
		{
			CharacterData* entry = ((InfoProxyFriendList)ptr).GetEntry((uint)i);
			if (entry == null)
			{
				continue;
			}
			CharacterData val = ((InfoProxyFriendList)ptr).CharDataSpan.get_Item(i);
			dictionary.Add(((CharacterData)entry).ContentId, ((CharacterData)entry).ExtraFlags);
			if (config.IgnoredGroup[((CharacterData)entry).Group])
			{
				((CharacterData)entry).ExtraFlags = (((CharacterData)entry).ExtraFlags & 0xFFFF) | 0x10000;
				continue;
			}
			bool flag = false;
			PlayerInfo value = null;
			if (config.SearchName)
			{
				string entryNameString = ((CharacterData)entry).NameString;
				if (string.IsNullOrEmpty(((CharacterData)entry).NameString))
				{
					RestoreEntryData(i, val.ContentId, taskHelper, delegate(string name)
					{
						entryNameString = name;
					});
				}
				flag |= MatchesSearch(entryNameString);
			}
			if (config.SearchNickname && config.PlayerInfos.TryGetValue(val.ContentId, out value))
			{
				flag |= MatchesSearch(value.Nickname);
			}
			if (config.SearchRemark && config.PlayerInfos.TryGetValue(val.ContentId, out value))
			{
				flag |= MatchesSearch(value.Remark);
			}
			if (((int)filterGroup == -1 || ((CharacterData)entry).Group == filterGroup) && flag)
			{
				((CharacterData)entry).ExtraFlags &= 65535u;
			}
			else
			{
				((CharacterData)entry).ExtraFlags = (((CharacterData)entry).ExtraFlags & 0xFFFF) | 0x10000;
			}
		}
		((InfoProxyFriendList)ptr).ApplyFilters();
		Unsafe.Write(&((InfoProxyFriendList)ptr).FilterGroup, filterGroup);
		foreach (KeyValuePair<ulong, uint> item in dictionary)
		{
			((CharacterData)((InfoProxyFriendList)ptr).GetEntryByContentId(item.Key, 0u, (byte)0)).ExtraFlags = item.Value;
		}
	}

	private static bool TryGetPlayerInfoByContentID(ulong contentID, [NotNullWhen(true)] out PlayerInfoValue? playerInfo)
	{
		playerInfo = null;
		WorldRegion region = WorldRegionResolver.Resolve(GameState.HomeWorld);
		return RemotePlayerInfo.TryGet(contentID, region, out playerInfo);
	}

	private unsafe void RestoreEntryData(int index, ulong contentID, TaskHelper? taskHelper, Action<string>? onNameResolved = null)
	{
		WorldRegion region = WorldRegionResolver.Resolve(GameState.HomeWorld);
		RemotePlayerInfo.GetOrRequest(contentID, region);
		IDisposable item = RemotePlayerInfo.Observe(contentID, region, delegate(RemoteSnapshot<PlayerInfoValue> snapshot)
		{
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			if (snapshot.HasValue)
			{
				PlayerInfoValue value = snapshot.Value;
				if ((object)value != null && Addons.FriendList != null)
				{
					RentedSeStringBuilder val = default(RentedSeStringBuilder);
					((RentedSeStringBuilder)(ref val))._002Ector();
					try
					{
						((RentedSeStringBuilder)(ref val)).Builder.PushColorType(32u).Append(value.Name).PopColorType();
						ReplaceAtkString(5 * index, ((RentedSeStringBuilder)(ref val)).Builder.ToReadOnlySeString());
						RentedSeStringBuilder val2 = new RentedSeStringBuilder();
						try
						{
							((RentedSeStringBuilder)(ref val2)).Builder.Append(LuminaWrapper.GetWorldName(value.WorldID)).AppendIcon(88u).Append(LuminaWrapper.GetWorldDCName(value.WorldID));
							ReplaceAtkString(1 + 5 * index, ((RentedSeStringBuilder)(ref val2)).Builder.ToReadOnlySeString());
							ReplaceAtkString(3 + 5 * index, ReadOnlySeString.op_Implicit(LuminaWrapper.GetAddonText(1351u)));
							onNameResolved?.Invoke(value.Name);
							if (taskHelper != null)
							{
								RequestInfoUpdate(taskHelper);
							}
						}
						finally
						{
							((IDisposable)(*(RentedSeStringBuilder*)(&val2))/*cast due to constrained. prefix*/).Dispose();
						}
					}
					finally
					{
						((IDisposable)(*(RentedSeStringBuilder*)(&val))/*cast due to constrained. prefix*/).Dispose();
					}
				}
			}
		});
		infoTokens.Add(item);
	}
}
