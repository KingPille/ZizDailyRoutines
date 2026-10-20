using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Command;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using Lumina.Text.ReadOnly;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Info.Lumina;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class AutoTrackPlayers : ModuleBase
{
	private sealed class TrackPlayerMenuItem(AutoTrackPlayers module) : ContextMenuEntry
	{
		public override string Identifier => "AutoTrackPlayers";

		public unsafe override ContextMenuItem? Create(ContextMenuOpenedArgs args)
		{
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Expected O, but got Unknown
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			if (args.DefaultAgentContext == null)
			{
				return null;
			}
			ulong contentID = args.TargetContentID;
			string playerName = args.TargetName;
			uint homeWorldID = (uint)args.TargetHomeWorldID;
			if (contentID == 0L || string.IsNullOrEmpty(playerName) || !Sheets.Worlds.ContainsKey(homeWorldID) || contentID == LocalPlayerState.ContentID)
			{
				return null;
			}
			bool num = module.trackedPlayers.ContainsKey(contentID);
			PlayerPayload playerPayload = new PlayerPayload(playerName, homeWorldID);
			if (num)
			{
				return new ContextMenuItem
				{
					Name = ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoTrackPlayers-ContextMenu-NotToTrack")),
					OnClicked = delegate
					{
						//IL_0075: Unknown result type (might be due to invalid IL or missing references)
						//IL_007a: Unknown result type (might be due to invalid IL or missing references)
						//IL_0080: Unknown result type (might be due to invalid IL or missing references)
						//IL_0087: Unknown result type (might be due to invalid IL or missing references)
						if (module.trackedPlayers.Remove(contentID))
						{
							if (!module.trackAddon.IsOpen)
							{
								module.trackAddon.Open();
							}
							else
							{
								module.trackAddon.RebuildList();
							}
							ReadOnlySeString se = LanguageManager.GetSe("AutoTrackPlayers-Notification-NotToTrack", playerPayload);
							OmenServiceBase<NotifyHelper>.Instance().Chat(se);
							NotifyHelper.Toast(se);
						}
					}
				};
			}
			return new ContextMenuItem
			{
				Name = ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoTrackPlayers-ContextMenu-ToTrack")),
				OnClicked = delegate
				{
					//IL_0094: Unknown result type (might be due to invalid IL or missing references)
					//IL_0099: Unknown result type (might be due to invalid IL or missing references)
					//IL_009f: Unknown result type (might be due to invalid IL or missing references)
					//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
					module.trackedPlayers.Add(contentID, (null, playerName, homeWorldID));
					if (!module.trackAddon.IsOpen)
					{
						module.trackAddon.Open();
					}
					else
					{
						module.trackAddon.RebuildList();
					}
					ReadOnlySeString se = LanguageManager.GetSe("AutoTrackPlayers-Notification-ToTrack", playerPayload, "/pdrtrack");
					OmenServiceBase<NotifyHelper>.Instance().Chat(se);
					NotifyHelper.Toast(se);
				}
			};
		}
	}

	private class AddonDRAutoTrackPlayers(AutoTrackPlayers module) : NativeAddon
	{
		private class PlayerEntryNode : HorizontalListNode
		{
			public unsafe PlayerEntryNode(ScrollingNode<VerticalListNode> parent, FieldMarkerPoint? currentMarker, ulong contentID, string name, string world, Action<FieldMarkerPoint?> onMarkerChange, Action onDelete, List<string> availableOptions)
			{
				//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
				//IL_0128: Unknown result type (might be due to invalid IL or missing references)
				base.ItemSpacing = 5f;
				Size = new Vector2(parent.Width, 30f);
				StringDropDownNode node = new StringDropDownNode
				{
					Size = new Vector2(120f, 30f),
					Options = availableOptions,
					SelectedOption = (currentMarker?.ToString() ?? LanguageManager.Get("AutoTrackPlayers-NoMarker")),
					OnOptionSelected = delegate(string selected)
					{
						FieldMarkerPoint result;
						if (selected == LanguageManager.Get("AutoTrackPlayers-NoMarker"))
						{
							onMarkerChange(null);
						}
						else if (Enum.TryParse<FieldMarkerPoint>(selected, out result))
						{
							onMarkerChange(result);
						}
					}
				};
				RentedSeStringBuilder val = default(RentedSeStringBuilder);
				((RentedSeStringBuilder)(ref val))._002Ector();
				try
				{
					ReadOnlySeString val2 = (string.IsNullOrEmpty(world) ? ReadOnlySeString.op_Implicit(contentID.ToString()) : ((RentedSeStringBuilder)(ref val)).Builder.Append(name).AppendIcon(88u).Append(world)
						.ToReadOnlySeString());
					TextNode textNode = new TextNode
					{
						String = val2,
						Size = new Vector2(200f, 30f),
						AlignmentType = (AlignmentType)3,
						FontSize = 14u
					};
					TextButtonNode textButtonNode = new TextButtonNode();
					textButtonNode.String = ReadOnlySeString.op_Implicit(string.Empty);
					textButtonNode.Size = new Vector2(200f, 30f);
					textButtonNode.OnClick = delegate
					{
						IPlayerCharacter playerCharacter = ManagerBase<PlayersManager>.Instance().PlayersAround.FirstOrDefault((IPlayerCharacter x) => x.ContentID == contentID);
						if (playerCharacter != null)
						{
							TargetManager.Target = playerCharacter;
						}
					};
					textButtonNode.BackgroundNode.IsVisible = false;
					IconButtonNode node2 = new IconButtonNode
					{
						Size = new Vector2(24f),
						IconId = 61502u,
						IsVisible = true,
						OnClick = onDelete
					};
					AddNode(node);
					AddNode(textNode);
					textButtonNode.AttachNode((NodeBase?)textNode, NodePosition.AsLastChild);
					AddNode(node2);
					parent.ContentNode.AddNode(this);
				}
				finally
				{
					((IDisposable)(*(RentedSeStringBuilder*)(&val))/*cast due to constrained. prefix*/).Dispose();
				}
			}
		}

		private ScrollingNode<VerticalListNode> scrollArea;

		private TextNode emptyHintText;

		private List<PlayerEntryNode> playerEntries = new List<PlayerEntryNode>();

		private bool isRebuildingList;

		protected unsafe override void OnSetup(AtkUnitBase* addon, Span<AtkValue> atkValues)
		{
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			scrollArea = new ScrollingNode<VerticalListNode>
			{
				Position = base.ContentStartPosition,
				Size = base.ContentSize,
				AutoHideScrollBar = true
			};
			scrollArea.ContentNode.FitContents = true;
			scrollArea.AttachNode(this);
			emptyHintText = new TextNode
			{
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoTrackPlayers-EmptyHint")),
				Size = base.ContentSize,
				Position = new Vector2(0f, 100f),
				AlignmentType = (AlignmentType)4,
				FontSize = 12u,
				IsVisible = true
			};
			scrollArea.ContentNode.AddNode(emptyHintText);
			RebuildList();
		}

		public void RebuildList()
		{
			if (this == null || !IsOpen || isRebuildingList)
			{
				return;
			}
			isRebuildingList = true;
			try
			{
				emptyHintText.IsVisible = module.trackedPlayers.Count == 0;
				foreach (PlayerEntryNode playerEntry in playerEntries)
				{
					playerEntry.DetachNode();
					playerEntry.Dispose();
				}
				playerEntries.Clear();
				foreach (KeyValuePair<ulong, (FieldMarkerPoint?, string, uint)> trackedPlayer in module.trackedPlayers)
				{
					trackedPlayer.Deconstruct(out var key, out var value);
					(FieldMarkerPoint?, string, uint) tuple = value;
					ulong contentID = key;
					FieldMarkerPoint? item = tuple.Item1;
					string name = tuple.Item2;
					uint world = tuple.Item3;
					PlayerEntryNode item2 = new PlayerEntryNode(scrollArea, item, contentID, name, LuminaWrapper.GetWorldName(world), delegate(FieldMarkerPoint? x)
					{
						module.trackedPlayers[contentID] = (x, name, world);
					}, delegate
					{
						//IL_0042: Unknown result type (might be due to invalid IL or missing references)
						//IL_0048: Expected O, but got Unknown
						//IL_0048: Unknown result type (might be due to invalid IL or missing references)
						//IL_004d: Unknown result type (might be due to invalid IL or missing references)
						//IL_0053: Unknown result type (might be due to invalid IL or missing references)
						//IL_005a: Unknown result type (might be due to invalid IL or missing references)
						if (module.trackedPlayers.Remove(contentID))
						{
							RebuildList();
							ReadOnlySeString se = LanguageManager.GetSe("AutoTrackPlayers-Notification-NotToTrack", (object)new PlayerPayload(name, world));
							OmenServiceBase<NotifyHelper>.Instance().Chat(se);
							NotifyHelper.Toast(se);
						}
					}, GetAvailableMarkers(contentID));
					playerEntries.Add(item2);
				}
				scrollArea.ContentNode.RecalculateLayout();
			}
			finally
			{
				isRebuildingList = false;
			}
		}

		private List<string> GetAvailableMarkers(ulong currentContentID)
		{
			HashSet<FieldMarkerPoint> hashSet = (from kvp in module.trackedPlayers
				where kvp.Key != currentContentID && kvp.Value.Marker.HasValue
				select kvp.Value.Marker.GetValueOrDefault()).ToHashSet();
			List<string> list = new List<string> { LanguageManager.Get("AutoTrackPlayers-NoMarker") };
			for (int num = 0; num < 8; num++)
			{
				FieldMarkerPoint item = (FieldMarkerPoint)num;
				if (!hashSet.Contains(item))
				{
					list.Add(item.ToString());
				}
			}
			return list;
		}
	}

	private readonly Dictionary<ulong, (FieldMarkerPoint? Marker, string Name, uint World)> trackedPlayers = new Dictionary<ulong, (FieldMarkerPoint?, string, uint)>();

	private TrackPlayerMenuItem trackPlayerMenuItem;

	private AddonDRAutoTrackPlayers trackAddon;

	private const string COMMAND = "/pdrtrack";

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoTrackPlayersTitle"),
		Description = LanguageManager.Get("AutoTrackPlayersDescription"),
		Category = ModuleCategory.System,
		Author = new string[1] { "KirisameVanilla" },
		ModulesConflict = new string[1] { "AutoHighlightFlagMarker" }
	};

	protected override void Init()
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Expected O, but got Unknown
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Expected O, but got Unknown
		trackPlayerMenuItem = new TrackPlayerMenuItem(this);
		ManagerBase<PlayersManager>.Instance().ReceivePlayersAround += OnReceivePlayers;
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += OnZoneChanged;
		OmenServiceBase<ContextMenuManager>.Instance().Reg(trackPlayerMenuItem);
		trackAddon = new AddonDRAutoTrackPlayers(this)
		{
			InternalName = "DRAutoTrackPlayers",
			Title = ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoTrackPlayersTitle")),
			Size = new Vector2(400f, 300f)
		};
		OmenServiceBase<CommandManager>.Instance().AddCommand("/pdrtrack", new CommandInfo(new HandlerDelegate(OnCommand))
		{
			HelpMessage = LanguageManager.Get("AutoTrackPlayers-CommandHelp")
		});
	}

	protected override void Uninit()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		ManagerBase<PlayersManager>.Instance().ReceivePlayersAround -= OnReceivePlayers;
		OmenServiceBase<FrameworkManager>.Instance().Unreg(OnUpdate);
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChanged;
		OmenServiceBase<ContextMenuManager>.Instance().Unreg(trackPlayerMenuItem);
		OmenServiceBase<CommandManager>.Instance().RemoveCommand("/pdrtrack");
		trackedPlayers.Clear();
		trackAddon?.Dispose();
		trackAddon = null;
	}

	protected override void ConfigUI()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(0, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("Command"));
		ImGui.TextColored(ref vector, val);
		IndentDisposable val2 = ImRaii.PushIndent(1, true);
		try
		{
			ImU8String val3 = new ImU8String(3, 2);
			((ImU8String)(ref val3)).AppendFormatted<string>("/pdrtrack");
			((ImU8String)(ref val3)).AppendLiteral(" → ");
			((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get("AutoTrackPlayers-CommandHelp"));
			ImGui.TextUnformatted(val3);
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	private void OnCommand(string command, string arguments)
	{
		trackAddon.Toggle();
	}

	private void OnZoneChanged(uint territoryType)
	{
		trackedPlayers.Clear();
	}

	private void OnReceivePlayers(IReadOnlyList<IPlayerCharacter> characters)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected O, but got Unknown
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		if (characters.Count == 0)
		{
			OmenServiceBase<FrameworkManager>.Instance().Unreg(OnUpdate);
		}
		else
		{
			OmenServiceBase<FrameworkManager>.Instance().Reg(new OnUpdateDelegate(OnUpdate));
		}
	}

	private unsafe void OnUpdate(IFramework framework)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Expected O, but got Unknown
		if (trackedPlayers.Count == 0 || !GameState.IsLoggedIn)
		{
			OmenServiceBase<FrameworkManager>.Instance().Unreg(OnUpdate);
			return;
		}
		if (ManagerBase<PlayersManager>.Instance().PlayersAroundCount == 0)
		{
			OmenServiceBase<FrameworkManager>.Instance().Unreg(OnUpdate);
			return;
		}
		List<(ulong, FieldMarkerPoint, Vector3)> list = new List<(ulong, FieldMarkerPoint, Vector3)>();
		foreach (KeyValuePair<ulong, (FieldMarkerPoint?, string, uint)> kvp in trackedPlayers)
		{
			if (kvp.Value.Item1.HasValue)
			{
				IPlayerCharacter playerCharacter = ManagerBase<PlayersManager>.Instance().PlayersAround.FirstOrDefault((IPlayerCharacter p) => p.ContentID == kvp.Key);
				if (playerCharacter != null)
				{
					list.Add((kvp.Key, kvp.Value.Item1.GetValueOrDefault(), playerCharacter.Position));
				}
			}
		}
		foreach (var item in list)
		{
			(ulong, FieldMarkerPoint, Vector3) current = item;
			(*MarkingController.Instance()).PlaceFieldMarkerLocal(in current.Item2, in current.Item3);
		}
	}
}
