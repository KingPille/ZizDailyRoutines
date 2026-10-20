using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using Lumina.Text.ReadOnly;
using OmenTools.Dalamud.Abstractions;
using OmenTools.Dalamud.Attributes;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.AddonEvent;
using OmenTools.Interop.Game.Models;
using OmenTools.KamiToolKit.Addons;
using OmenTools.KamiToolKit.Nodes;
using OmenTools.OmenService;
using OmenTools.Threading.TaskHelper;
using TinyPinyin;

namespace DailyRoutines.ModulesPublic.Interface;

public class OptimizedLetter : ModuleBase
{
	private class AddonDROptimizedLetter(TaskHelper taskHelper) : AttachedAddon("LetterList")
	{
		private static AtkEventWrapper? FireRequestEvent;

		protected unsafe override void OnSetup(AtkUnitBase* addon, Span<AtkValue> atkValues)
		{
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			if ((*Addons.LetterList).IsAddonAndNodesReady())
			{
				AtkComponentButton* componentButtonById = ((AtkUnitBase)Addons.LetterList).GetComponentButtonById(4u);
				if (componentButtonById != null)
				{
					((AtkComponentNode)((AtkComponentButton)componentButtonById).OwnerNode).ClearEvents();
					FireRequestEvent = new AtkEventWrapper(delegate
					{
						if ((*Addons.LetterList).IsAddonAndNodesReady())
						{
							NotifyHelper.Toast(LanguageManager.Get("OptimizedLetter-Notification-RewardDeliveryRequested"));
							((InfoProxyLetter)InfoProxyLetter.Instance()).RequestRewardDelivery();
						}
					});
					FireRequestEvent.Add(addon, (AtkResNode*)((AtkComponentButton)componentButtonById).OwnerNode, (AtkEventType)25);
				}
			}
			VerticalListNode verticalListNode = new VerticalListNode
			{
				Position = base.ContentStartPosition + new Vector2(0f, 2f),
				ItemSpacing = 1f,
				Size = new Vector2(275f, 28f),
				FitContents = true
			};
			HoldButtonNode node = new HoldButtonNode
			{
				UnlockAfterClick = true,
				Size = new Vector2(verticalListNode.Size.X - 10f, 38f),
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("OptimizedLetter-Button-Delete-All")),
				OnClick = delegate
				{
					if (TryFindLetters((Letter _) => true, out List<(int, Letter)> letters))
					{
						InfoProxyLetter* ptr = InfoProxyLetter.Instance();
						foreach (var item2 in letters)
						{
							int item = item2.Item1;
							((InfoProxyLetter)InfoProxyLetter.Instance()).DeleteLetter((uint)item);
						}
						((InfoProxyLetter)ptr).RequestData();
					}
				}
			};
			verticalListNode.AddNode(node);
			verticalListNode.AddDummy(5f);
			HoldButtonNode node2 = new HoldButtonNode
			{
				UnlockAfterClick = true,
				Size = new Vector2(verticalListNode.Size.X - 10f, 38f),
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("OptimizedLetter-Button-Delete-AllButPlayers")),
				OnClick = delegate
				{
					if (TryFindLetters((Letter x) => x.SenderContentId < 100000000000L, out List<(int, Letter)> letters))
					{
						InfoProxyLetter* ptr = InfoProxyLetter.Instance();
						foreach (var item3 in letters)
						{
							int item = item3.Item1;
							((InfoProxyLetter)ptr).DeleteLetter((uint)item);
						}
						((InfoProxyLetter)ptr).RequestData();
					}
				}
			};
			verticalListNode.AddNode(node2);
			verticalListNode.AddDummy(5f);
			HoldButtonNode node3 = new HoldButtonNode
			{
				UnlockAfterClick = true,
				Size = new Vector2(verticalListNode.Size.X - 10f, 38f),
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("OptimizedLetter-Button-Claim")),
				OnClick = delegate
				{
					if (!TryFindLetters((Letter x) => ((Letter)(ref x)).Attachments.ToArray().Any((ItemAttachment d) => d.Count != 0), out List<(int, Letter)> letters))
					{
						return;
					}
					InfoProxyLetter* info = InfoProxyLetter.Instance();
					foreach (var item4 in letters)
					{
						var (index, _) = item4;
						taskHelper.Enqueue(() => ((InfoProxyLetter)info).TakeAttachments((uint)index, -1));
						taskHelper.Enqueue(() => ((NumberArrayData)((AtkStage)AtkStage.Instance()).GetNumberArrayData((NumberArrayType)20)).IntArray[136] == 0);
					}
				}
			};
			verticalListNode.AddNode(node3);
			verticalListNode.AttachNode(this);
		}

		protected unsafe override void OnAttachedAddonFinalize(AtkUnitBase* addon)
		{
			FireRequestEvent?.Dispose();
			FireRequestEvent = null;
		}

		private unsafe static bool TryFindLetters(Predicate<Letter> predicate, out List<(int Index, Letter)> letters)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			letters = new List<(int, Letter)>();
			InfoProxyLetter* ptr = InfoProxyLetter.Instance();
			if (ptr == null)
			{
				return false;
			}
			for (int i = 0; i < ((InfoProxyLetter)ptr).Letters.Length; i++)
			{
				Letter val = ((InfoProxyLetter)ptr).Letters[i];
				if (val.Timestamp != 0 && predicate(val))
				{
					letters.Add((i, val));
				}
			}
			return letters.Count > 0;
		}
	}

	private AddonDROptimizedLetter? addon;

	private TextInputNode? textInputButton;

	private TextButtonListNode? listNode;

	[IPCSubscriber("DailyRoutines.Modules.OptimizedFriendlist.GetRemarkByContentID", DefaultValue = "")]
	private IPCSubscriber<ulong, string> GetRemarkByContentID;

	[IPCSubscriber("DailyRoutines.Modules.OptimizedFriendlist.GetNicknameByContentID", DefaultValue = "")]
	private IPCSubscriber<ulong, string> GetNicknameByContentID;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("OptimizedLetterTitle"),
		Description = LanguageManager.Get("OptimizedLetterDescription"),
		Category = ModuleCategory.Interface
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected override void Init()
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Expected O, but got Unknown
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Expected O, but got Unknown
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Expected O, but got Unknown
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Expected O, but got Unknown
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (base.TaskHelper == null)
		{
			TaskHelper taskHelper = (base.TaskHelper = new TaskHelper());
		}
		if (addon == null)
		{
			addon = new AddonDROptimizedLetter(base.TaskHelper)
			{
				InternalName = "DROptimizedLetter",
				Title = ReadOnlySeString.op_Implicit(Info.Title),
				Size = new Vector2(290f, 190f)
			};
		}
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)1, "SelectYesno", new AddonEventDelegate(OnAddonSelectYesNo));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)5, "LetterAddress", new AddonEventDelegate(OnAddonLetterAddress));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, "LetterAddress", new AddonEventDelegate(OnAddonLetterAddress));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)5, "LetterList", new AddonEventDelegate(OnAddon));
	}

	protected override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddonSelectYesNo });
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddon });
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddonLetterAddress });
		textInputButton?.Dispose();
		textInputButton = null;
		listNode?.Dispose();
		listNode = null;
		addon?.Dispose();
		addon = null;
	}

	private unsafe void OnAddon(AddonEvent type, AddonArgs? args)
	{
		if (!addon.IsOpen && (*Addons.LetterList).IsAddonAndNodesReady())
		{
			addon.Open();
		}
	}

	private unsafe void OnAddonLetterAddress(AddonEvent type, AddonArgs args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		if ((int)type != 5)
		{
			if ((int)type == 6)
			{
				textInputButton = null;
				listNode = null;
			}
		}
		else
		{
			if (Addons.LetterAddress == null || textInputButton != null)
			{
				return;
			}
			AtkTextNode* textNodeById = ((AtkUnitBase)Addons.LetterAddress).GetTextNodeById(3u);
			if (textNodeById != null)
			{
				((AtkTextNode)textNodeById).ToggleVisibility(false);
			}
			textInputButton = new TextInputNode
			{
				Size = new Vector2(200f, 30f),
				Position = new Vector2(12f, 32f),
				OnInputReceived = delegate(ReadOnlySeString name)
				{
					//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
					//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
					//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
					//IL_00df: Unknown result type (might be due to invalid IL or missing references)
					//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
					if (listNode == null)
					{
						listNode = new TextButtonListNode
						{
							IsVisible = false,
							Position = new Vector2(16f, 68f),
							Size = new Vector2(316f, 192f),
							OnOptionSelected = delegate(string option)
							{
								((AgentId)67).SendEvent(8uL, 2, 1, option);
								((AgentId)67).SendEvent(8uL, -1);
								if (Addons.LetterEditor != null)
								{
									((AtkComponentButton)((AtkUnitBase)Addons.LetterEditor).GetComponentButtonById(3u)).SetText(option);
								}
								if (Addons.LetterAddress != null)
								{
									((AtkUnitBase)Addons.LetterAddress).Close(true);
								}
							}
						};
						listNode.BackgroundNode.IsVisible = false;
						listNode.ScrollingListNode.AutoHideScrollBar = true;
						listNode.AttachNode(Addons.LetterAddress);
					}
					List<string> list = new List<string>();
					ReadOnlySpan<CharacterData> charDataSpan = ((InfoProxyFriendList)InfoProxyFriendList.Instance()).CharDataSpan;
					for (int num = 0; num < charDataSpan.Length; num++)
					{
						CharacterData val = charDataSpan.get_Item(num);
						if (val.HomeWorld == GameState.HomeWorld)
						{
							string text = GetRemarkByContentID.TryInvokeFunc(val.ContentId) ?? string.Empty;
							string text2 = GetNicknameByContentID.TryInvokeFunc(val.ContentId) ?? string.Empty;
							string pinyin = PinyinHelper.GetPinyin(((CharacterData)(ref val)).NameString, string.Empty);
							string pinyin2 = PinyinHelper.GetPinyin(text, string.Empty);
							string pinyin3 = PinyinHelper.GetPinyin(text2, string.Empty);
							if (((CharacterData)(ref val)).NameString.Contains(((object)(*(ReadOnlySeString*)(&name))/*cast due to constrained. prefix*/).ToString(), StringComparison.OrdinalIgnoreCase) || pinyin.Contains(((object)(*(ReadOnlySeString*)(&name))/*cast due to constrained. prefix*/).ToString(), StringComparison.OrdinalIgnoreCase) || text.Contains(((object)(*(ReadOnlySeString*)(&name))/*cast due to constrained. prefix*/).ToString(), StringComparison.OrdinalIgnoreCase) || pinyin2.Contains(((object)(*(ReadOnlySeString*)(&name))/*cast due to constrained. prefix*/).ToString(), StringComparison.OrdinalIgnoreCase) || text2.Contains(((object)(*(ReadOnlySeString*)(&name))/*cast due to constrained. prefix*/).ToString(), StringComparison.OrdinalIgnoreCase) || pinyin3.Contains(((object)(*(ReadOnlySeString*)(&name))/*cast due to constrained. prefix*/).ToString(), StringComparison.OrdinalIgnoreCase))
							{
								list.Add(((CharacterData)(ref val)).NameString);
							}
						}
					}
					bool flag = string.IsNullOrWhiteSpace(((object)(*(ReadOnlySeString*)(&name))/*cast due to constrained. prefix*/).ToString());
					listNode.IsVisible = !flag;
					AtkComponentList* componentListById = ((AtkUnitBase)Addons.LetterAddress).GetComponentListById(7u);
					if (componentListById != null)
					{
						((AtkComponentNode)((AtkComponentList)componentListById).OwnerNode).ToggleVisibility(flag);
					}
					listNode.MaxButtons = (int)MathF.Min(list.Count, 8f);
					listNode.Options = list;
				}
			};
			textInputButton.AttachNode(((AtkUnitBase)Addons.LetterAddress).RootNode);
			if (listNode != null)
			{
				bool isVisible = !string.IsNullOrWhiteSpace(((object)textInputButton.String/*cast due to constrained. prefix*/).ToString());
				listNode.IsVisible = isVisible;
			}
		}
	}

	private void OnAddonSelectYesNo(AddonEvent type, AddonArgs args)
	{
		if (base.TaskHelper.IsBusy)
		{
			AddonSelectYesnoEvent.ClickYes();
		}
	}
}
