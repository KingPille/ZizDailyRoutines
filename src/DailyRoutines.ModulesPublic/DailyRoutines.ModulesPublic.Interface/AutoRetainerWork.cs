using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using DailyRoutines.Common.Extensions;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Internal;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Network.Structures;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Hooking;
using Dalamud.Interface;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.BaseTypes.ComponentNode;
using KamiToolKit.Classes;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Dalamud.Abstractions;
using OmenTools.Dalamud.Attributes;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.ImGuiOm.Widgets.Combos;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.AddonEvent;
using OmenTools.Interop.Game.Helpers;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.Interop.Game.Models.Native;
using OmenTools.KamiToolKit.Addons;
using OmenTools.KamiToolKit.Nodes.Collasping;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;

namespace DailyRoutines.ModulesPublic.Interface;

public class AutoRetainerWork : ModuleBase
{
	private class DRAutoRetainerWork(AutoRetainerWork module) : AttachedAddon("RetainerList")
	{
		private CollaspingNode? treeListNode;

		protected override Vector2 PositionOffset => new Vector2(0f, 6f);

		protected override bool CanOpenAddon => !module.IsAnyWorkerBusy();

		protected unsafe override void OnSetup(AtkUnitBase* addon, Span<AtkValue> atkValues)
		{
			if (base.WindowNode is WindowNode windowNode)
			{
				windowNode.CloseButtonNode.IsVisible = false;
			}
			FlagHelper.UpdateFlag(ref ((AtkUnitBase)addon).Flags1A1, 4, enable: true);
			FlagHelper.UpdateFlag(ref ((AtkUnitBase)addon).Flags1A0, 128, enable: true);
			FlagHelper.UpdateFlag(ref ((AtkUnitBase)addon).Flags1A1, 64, enable: true);
			FlagHelper.UpdateFlag(ref ((AtkUnitBase)addon).Flags1A3, 1, enable: true);
			float x = base.ContentSize.X;
			treeListNode = new CollaspingNode
			{
				IsVisible = true,
				Position = base.ContentStartPosition,
				Size = new Vector2(x, 0f),
				CategoryVerticalSpacing = 4f,
				OnLayoutUpdate = delegate(float height)
				{
					SetWindowSize(base.Size.X, base.ContentStartPosition.Y + height + 16f);
					if (treeListNode != null)
					{
						treeListNode.Position = base.ContentStartPosition;
						treeListNode.Height = height;
					}
				}
			};
			RetainerWorkerBase[] workers = module.workers;
			for (int num = 0; num < workers.Length; num++)
			{
				CollaspingCategoryNode collaspingCategoryNode = workers[num].CreateOverlayCategory(x);
				if (collaspingCategoryNode != null)
				{
					treeListNode.AddCategoryNode(collaspingCategoryNode);
				}
			}
			treeListNode.AttachNode(addon);
			treeListNode.RefreshLayout();
			ApplyControllerNavigation(addon);
		}

		protected unsafe override bool CanCloseHostAddon(AtkUnitBase* hostAddon)
		{
			return false;
		}

		private unsafe void ApplyControllerNavigation(AtkUnitBase* addon)
		{
			if (treeListNode == null)
			{
				return;
			}
			List<ComponentNode> list = new List<ComponentNode>();
			foreach (CollaspingCategoryNode categoryNode in treeListNode.CategoryNodes)
			{
				NavFocusNode navFocusNode = new NavFocusNode
				{
					Position = new Vector2(2f, 14f),
					OnSelected = delegate
					{
						categoryNode.IsCollapsed = !categoryNode.IsCollapsed;
					},
					OnHoverStart = delegate
					{
						categoryNode.Timeline?.PlayAnimation(categoryNode.IsCollapsed ? 2 : 9);
					},
					OnHoverEnd = delegate
					{
						categoryNode.Timeline?.PlayAnimation(categoryNode.IsCollapsed ? 1 : 8);
					}
				};
				navFocusNode.AttachNode(categoryNode);
				list.Add(navFocusNode);
				foreach (NodeBase item2 in categoryNode.Children.OfType<VerticalListNode>().SelectMany((VerticalListNode x) => x.Nodes))
				{
					if (!(item2 is CheckboxNode item))
					{
						if (item2 is HorizontalFlexNode horizontalFlexNode)
						{
							List<TextButtonNode> list2 = horizontalFlexNode.Nodes.OfType<TextButtonNode>().ToList();
							int count = list.Count;
							list.AddRange(list2);
							for (int num = 0; num < list2.Count; num++)
							{
								list2[num].NavLeft = count + ((num == 0) ? list2.Count : num);
								list2[num].NavRight = count + ((num == list2.Count - 1) ? 1 : (num + 2));
							}
						}
					}
					else
					{
						list.Add(item);
					}
				}
			}
			if (list.Count != 0)
			{
				for (int num2 = 0; num2 < list.Count; num2++)
				{
					list[num2].NavIndex = num2 + 1;
					list[num2].NavUp = ((num2 == 0) ? list.Count : num2);
					list[num2].NavDown = ((num2 == list.Count - 1) ? 1 : (num2 + 2));
				}
				((AtkUnitBase)addon).FocusNode = list[0];
			}
		}
	}

	private class CollectWorker : RetainerWorkerBase
	{
		private TaskHelper? taskHelper;

		private static readonly string[] VentureCompleteTexts = new string[7] { "结束", "結束", "Complete", "完了", "완료", "Abgeschlossen", "Terminée" };

		public CollectWorker(AutoRetainerWork module)
			: base(module)
		{
		}

		public override bool DrawConfigCondition()
		{
			return false;
		}

		public override bool IsWorkerBusy()
		{
			return taskHelper?.IsBusy ?? false;
		}

		public override void Init()
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Expected O, but got Unknown
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Expected O, but got Unknown
			if (taskHelper == null)
			{
				taskHelper = new TaskHelper
				{
					TimeoutMS = 15000,
					ShowDebug = true
				};
			}
			DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)1, "RetainerList", new AddonEventDelegate(OnRetainerList));
			DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)5, "RetainerList", new AddonEventDelegate(OnRetainerList));
		}

		public override void Uninit()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Expected O, but got Unknown
			DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnRetainerList });
			taskHelper?.Abort();
			taskHelper?.Dispose();
			taskHelper = null;
		}

		public override CollaspingCategoryNode CreateOverlayCategory(float width)
		{
			return RetainerWorkerBase.CreateOverlayCategory(LanguageManager.Get("AutoRetainerWork-Collect-Title"), width, RetainerWorkerBase.CreateOverlayCheckbox(LanguageManager.Get("AutoRetainerWork-Collect-AutoCollect"), Module.config.AutoRetainerCollect, delegate(bool isChecked)
			{
				Module.config.AutoRetainerCollect = isChecked;
				if (Module.config.AutoRetainerCollect)
				{
					EnqueueRetainersCollect();
				}
				Module.config.Save(Module);
			}, width), RetainerWorkerBase.CreateOverlayCheckbox(LanguageManager.Get("AutoRetainerWork-Collect-AutoPriceAdjustAfterCollect"), Module.config.AutoPriceAdjustAfterCollect, delegate(bool isChecked)
			{
				Module.config.AutoPriceAdjustAfterCollect = isChecked;
				Module.config.Save(Module);
			}, width), RetainerWorkerBase.CreateOverlayButtonRow(EnqueueRetainersCollect, delegate
			{
				taskHelper?.Abort();
			}, width));
		}

		private void OnRetainerList(AddonEvent type, AddonArgs args)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Invalid comparison between Unknown and I4
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Invalid comparison between Unknown and I4
			if (Module.IsAnyOtherWorkerBusy(typeof(CollectWorker)))
			{
				return;
			}
			if ((int)type != 1)
			{
				if ((int)type != 5 || !Module.config.AutoRetainerCollect || !Module.retainerThrottler.Throttle("AutoRetainerCollect-AFK", 5000u))
				{
					return;
				}
				DalamudServiceExtension.Instance<IFramework>().RunOnTick((Action)delegate
				{
					if (!taskHelper.IsBusy)
					{
						EnqueueRetainersCollect();
					}
				}, TimeSpan.FromSeconds(1L), 0, default(CancellationToken));
			}
			else
			{
				Module.ObtainPlayerRetainers();
				if (!taskHelper.IsBusy && Module.config.AutoRetainerCollect && !taskHelper.AbortByConflictKey(Module))
				{
					EnqueueRetainersCollect();
				}
			}
		}

		private unsafe void EnqueueRetainersCollect()
		{
			if (taskHelper.AbortByConflictKey(Module))
			{
				return;
			}
			long serverTime = Framework.GetServerTime();
			if (GetValidRetainerCount((Retainer x) => x.VentureId != 0 && x.VentureComplete != 0 && x.VentureComplete + 1 <= serverTime, out List<uint> validRetainers) == 0)
			{
				if (!taskHelper.IsBusy)
				{
					return;
				}
				taskHelper.Enqueue((Func<bool>)LeaveRetainer, "确保所有雇员均已返回", 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
				if (!Module.config.AutoPriceAdjustAfterCollect)
				{
					return;
				}
				taskHelper.Enqueue(delegate
				{
					if (taskHelper.AbortByConflictKey(Module))
					{
						return true;
					}
					DalamudServiceExtension.Instance<IFramework>().RunOnTick((Action)delegate
					{
						if (Module.config.AutoPriceAdjustAfterCollect)
						{
							(Array.Find(Module.workers, (RetainerWorkerBase w) => w is PriceAdjustWorker) as PriceAdjustWorker)?.EnqueuePriceAdjustAllRetainers();
						}
					}, default(TimeSpan), 0, default(CancellationToken));
					return true;
				}, "收取完成后触发自动改价");
				return;
			}
			foreach (uint index in validRetainers)
			{
				taskHelper.Enqueue(() => taskHelper.AbortByConflictKey(Module) || Module.EnterRetainer(index), $"选择进入 {index} 号雇员");
				taskHelper.Enqueue(delegate
				{
					if (taskHelper.AbortByConflictKey(Module))
					{
						return true;
					}
					if (!(*Addons.SelectString).IsAddonAndNodesReady())
					{
						return false;
					}
					if (Addons.RetainerList != null)
					{
						return false;
					}
					if (!AddonSelectStringEvent.TryScanSelectStringText(VentureCompleteTexts, out var index2))
					{
						taskHelper.Abort();
						taskHelper.Enqueue((Func<bool>)LeaveRetainer, "回到雇员列表", 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
						return true;
					}
					return AddonSelectStringEvent.Select(index2);
				}, "确认雇员探险完成");
				taskHelper.Enqueue(delegate
				{
					if (taskHelper.AbortByConflictKey(Module))
					{
						return true;
					}
					if (!(*Addons.RetainerTaskResult).IsAddonAndNodesReady())
					{
						return false;
					}
					(*Addons.RetainerTaskResult).Callback(14);
					return true;
				}, "重新派遣雇员探险");
				taskHelper.Enqueue(delegate
				{
					if (taskHelper.AbortByConflictKey(Module))
					{
						return true;
					}
					if (!(*Addons.RetainerTaskAsk).IsAddonAndNodesReady())
					{
						return false;
					}
					(*Addons.RetainerTaskAsk).Callback(12);
					return true;
				}, "确认派遣雇员探险");
				taskHelper.Enqueue(() => taskHelper.AbortByConflictKey(Module) || LeaveRetainer(), "回到雇员列表");
			}
			taskHelper.Enqueue((Action)EnqueueRetainersCollect, "重新检查是否有其他雇员需要收取", 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
		}
	}

	private enum AdjustBehavior
	{
		固定值,
		百分比
	}

	[Flags]
	private enum AbortCondition
	{
		无 = 1,
		低于最小值 = 2,
		低于预期值 = 4,
		低于收购价 = 8,
		大于可接受降价值 = 0x10,
		高于预期值 = 0x20,
		高于最大值 = 0x40
	}

	private enum AbortBehavior
	{
		无,
		收回至雇员,
		收回至背包,
		出售至系统商店,
		改价至最小值,
		改价至预期值,
		改价至最高值
	}

	private enum SortOrder
	{
		上架顺序,
		物品ID,
		物品类型
	}

	private abstract class RetainerWorkerBase(AutoRetainerWork module)
	{
		protected AutoRetainerWork Module = module;

		public abstract bool IsWorkerBusy();

		public virtual bool DrawConfigCondition()
		{
			return true;
		}

		public abstract void Init();

		public virtual CollaspingCategoryNode? CreateOverlayCategory(float width)
		{
			return null;
		}

		public virtual void DrawConfig()
		{
		}

		public abstract void Uninit();

		protected static CollaspingCategoryNode CreateOverlayCategory(string title, float width, params NodeBase[] nodes)
		{
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			VerticalListNode verticalListNode = new VerticalListNode
			{
				IsVisible = true,
				Size = new Vector2(width, 0f),
				FitContents = true,
				FitWidth = true,
				FirstItemSpacing = 4f,
				ItemSpacing = 4f
			};
			verticalListNode.AddNode(nodes);
			CollaspingCategoryNode collaspingCategoryNode = new CollaspingCategoryNode();
			collaspingCategoryNode.IsVisible = true;
			collaspingCategoryNode.Size = new Vector2(width, 28f);
			collaspingCategoryNode.String = ReadOnlySeString.op_Implicit(title);
			collaspingCategoryNode.AddNode(verticalListNode);
			collaspingCategoryNode.IsCollapsed = true;
			return collaspingCategoryNode;
		}

		protected static HorizontalFlexNode CreateOverlayButtonRow(Action startAction, Action stopAction, float width)
		{
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			HorizontalFlexNode horizontalFlexNode = new HorizontalFlexNode
			{
				IsVisible = true,
				Size = new Vector2(width, 28f),
				AlignmentFlags = (FlexFlags.FitWidth | FlexFlags.FitContentHeight),
				ItemSpacing = 4f
			};
			horizontalFlexNode.AddNode(new global::_003C_003Ez__ReadOnlyArray<NodeBase>(new NodeBase[2]
			{
				new TextButtonNode
				{
					IsVisible = true,
					IsEnabled = true,
					Size = new Vector2(100f, 28f),
					String = ReadOnlySeString.op_Implicit(LanguageManager.Get("Start")),
					OnClick = startAction
				},
				new TextButtonNode
				{
					IsVisible = true,
					IsEnabled = true,
					Size = new Vector2(100f, 28f),
					String = ReadOnlySeString.op_Implicit(LanguageManager.Get("Stop")),
					OnClick = stopAction
				}
			}));
			return horizontalFlexNode;
		}

		protected static CheckboxNode CreateOverlayCheckbox(string title, bool isChecked, Action<bool> onClick, float width, string? tooltip = null)
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			CheckboxNode checkboxNode = new CheckboxNode
			{
				IsVisible = true,
				IsEnabled = true,
				Size = new Vector2(width, 24f),
				IsChecked = isChecked,
				String = ReadOnlySeString.op_Implicit(title),
				OnClick = onClick
			};
			if (!string.IsNullOrWhiteSpace(tooltip))
			{
				checkboxNode.TextTooltip = ReadOnlySeString.op_Implicit(tooltip);
			}
			return checkboxNode;
		}

		protected static TextNode CreateOverlayText(string text, float width)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			TextNode obj = new TextNode
			{
				IsVisible = true,
				Size = new Vector2(width, 24f),
				FontSize = 14u,
				String = ReadOnlySeString.op_Implicit(text),
				AlignmentType = (AlignmentType)3
			};
			obj.AutoAdjustTextSize();
			return obj;
		}
	}

	private class PriceCheckCondition(AbortCondition condition, Func<ItemConfig, uint, uint, uint, bool> predicate)
	{
		public AbortCondition Condition { get; } = condition;

		public Func<ItemConfig, uint, uint, uint, bool> Predicate { get; } = predicate;
	}

	private static class PriceCheckConditions
	{
		private static readonly PriceCheckCondition[] Conditions = new PriceCheckCondition[6]
		{
			new PriceCheckCondition(AbortCondition.高于最大值, (ItemConfig cfg, uint _, uint modified, uint _) => modified > cfg.PriceMaximum),
			new PriceCheckCondition(AbortCondition.高于预期值, (ItemConfig cfg, uint _, uint modified, uint _) => modified > cfg.PriceExpected),
			new PriceCheckCondition(AbortCondition.大于可接受降价值, (ItemConfig cfg, uint orig, uint modified, uint _) => cfg.PriceMaxReduction != 0 && orig != 999999999 && orig - modified != 0 && orig - modified > cfg.PriceMaxReduction),
			new PriceCheckCondition(AbortCondition.低于收购价, (ItemConfig cfg, uint _, uint modified, uint _) => LuminaGetter.TryGetRow<Item>(cfg.ItemID, out Item item) && modified <= ((Item)(ref item)).PriceMid),
			new PriceCheckCondition(AbortCondition.低于最小值, (ItemConfig cfg, uint _, uint modified, uint _) => modified < cfg.PriceMinimum),
			new PriceCheckCondition(AbortCondition.低于预期值, (ItemConfig cfg, uint _, uint modified, uint _) => modified < cfg.PriceExpected)
		};

		public static IEnumerable<PriceCheckCondition> GetAll()
		{
			return Conditions;
		}

		public static PriceCheckCondition Get(AbortCondition condition)
		{
			return Conditions.FirstOrDefault((PriceCheckCondition x) => x.Condition == condition);
		}
	}

	private class Config : ModuleConfig
	{
		public bool AutoPriceAdjustWhenNewOnSale = true;

		public bool AutoOnSale = true;

		public bool AutoRetainerCollect = true;

		public bool AutoPriceAdjustAfterCollect;

		public Dictionary<string, ItemConfig> ItemConfigs = new Dictionary<string, ItemConfig>
		{
			{
				new ItemKey(0u, isHQ: false).ToString(),
				new ItemConfig(0u, isHQ: false)
			},
			{
				new ItemKey(0u, isHQ: true).ToString(),
				new ItemConfig(0u, isHQ: true)
			}
		};

		public SortOrder MarketItemsSortOrder;

		public float MarketItemsWindowFontScale = 0.8f;

		public bool SendPriceAdjustProcessMessage = true;
	}

	private class ItemKey : IEquatable<ItemKey>
	{
		public uint ItemID { get; set; }

		public bool IsHQ { get; set; }

		public ItemKey()
		{
		}

		public ItemKey(uint itemID, bool isHQ)
		{
			ItemID = itemID;
			IsHQ = isHQ;
		}

		public bool Equals(ItemKey? other)
		{
			if ((object)other == null || GetType() != other.GetType())
			{
				return false;
			}
			if (ItemID == other.ItemID)
			{
				return IsHQ == other.IsHQ;
			}
			return false;
		}

		public override string ToString()
		{
			return $"{ItemID}_{(IsHQ ? "HQ" : "NQ")}";
		}

		public override bool Equals(object? obj)
		{
			return Equals(obj as ItemKey);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(ItemID, IsHQ);
		}

		public static bool operator ==(ItemKey? lhs, ItemKey? rhs)
		{
			return lhs?.Equals(rhs) ?? ((object)rhs == null);
		}

		public static bool operator !=(ItemKey lhs, ItemKey rhs)
		{
			return !(lhs == rhs);
		}
	}

	private class ItemConfig : IEquatable<ItemConfig>
	{
		public uint ItemID { get; set; }

		public bool IsHQ { get; set; }

		public string ItemName { get; set; } = string.Empty;

		public AdjustBehavior AdjustBehavior { get; set; }

		public Dictionary<AdjustBehavior, int> AdjustValues { get; set; } = new Dictionary<AdjustBehavior, int>
		{
			{
				AdjustBehavior.固定值,
				1
			},
			{
				AdjustBehavior.百分比,
				10
			}
		};

		public int PriceMinimum { get; set; } = 100;

		public int PriceMaximum { get; set; } = 100000000;

		public int PriceExpected { get; set; } = 200;

		public int PriceMaxReduction { get; set; }

		public int UpshelfCount { get; set; }

		public Dictionary<AbortCondition, AbortBehavior> AbortLogic { get; set; } = new Dictionary<AbortCondition, AbortBehavior>();

		public ItemConfig()
		{
		}

		public ItemConfig(uint itemID, bool isHQ)
		{
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			ItemID = itemID;
			IsHQ = isHQ;
			object obj;
			if (itemID != 0)
			{
				Item? row = LuminaGetter.GetRow<Item>(ItemID);
				if (!row.HasValue)
				{
					obj = null;
				}
				else
				{
					Item valueOrDefault = row.GetValueOrDefault();
					obj = ((object)((Item)(ref valueOrDefault)).Name/*cast due to constrained. prefix*/).ToString();
				}
				if (obj == null)
				{
					obj = string.Empty;
				}
			}
			else
			{
				obj = LanguageManager.Get("AutoRetainerWork-PriceAdjust-CommonItemPreset");
			}
			ItemName = (string)obj;
		}

		public bool Equals(ItemConfig? other)
		{
			if ((object)other == null || GetType() != other.GetType())
			{
				return false;
			}
			if (ItemID == other.ItemID)
			{
				return IsHQ == other.IsHQ;
			}
			return false;
		}

		public override bool Equals(object? obj)
		{
			return Equals(obj as ItemConfig);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(ItemID, IsHQ);
		}

		public static bool operator ==(ItemConfig? lhs, ItemConfig? rhs)
		{
			return lhs?.Equals(rhs) ?? ((object)rhs == null);
		}

		public static bool operator !=(ItemConfig lhs, ItemConfig rhs)
		{
			return !(lhs == rhs);
		}
	}

	private class EntrustDupsWorker : RetainerWorkerBase
	{
		private TaskHelper? taskHelper;

		private static readonly string[] ItemEntrustWithdrawTexts = new string[6] { "道具管理", "Entrust or withdraw items", "アイテムの受け渡し", "아이템 주고받기", "Gegenstände übergeben oder entnehmen", "Échanger des objets" };

		public EntrustDupsWorker(AutoRetainerWork module)
			: base(module)
		{
		}

		public override bool DrawConfigCondition()
		{
			return false;
		}

		public override bool IsWorkerBusy()
		{
			return taskHelper?.IsBusy ?? false;
		}

		public override void Init()
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Expected O, but got Unknown
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Expected O, but got Unknown
			if (taskHelper == null)
			{
				taskHelper = new TaskHelper
				{
					TimeoutMS = 15000
				};
			}
			DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)1, "RetainerItemTransferList", new AddonEventDelegate(OnEntrustDupsAddons));
			DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)1, "RetainerItemTransferProgress", new AddonEventDelegate(OnEntrustDupsAddons));
		}

		public override void Uninit()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Expected O, but got Unknown
			DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnEntrustDupsAddons });
			taskHelper?.Abort();
			taskHelper?.Dispose();
			taskHelper = null;
		}

		public override CollaspingCategoryNode CreateOverlayCategory(float width)
		{
			return RetainerWorkerBase.CreateOverlayCategory(LanguageManager.Get("AutoRetainerWork-EntrustDups-Title"), width, RetainerWorkerBase.CreateOverlayButtonRow(EnqueueRetainersEntrust, delegate
			{
				taskHelper?.Abort();
			}, width));
		}

		private unsafe void EnqueueRetainersEntrust()
		{
			if (taskHelper.AbortByConflictKey(Module) || Module.IsAnyOtherWorkerBusy(typeof(EntrustDupsWorker)) || GetValidRetainerCount((Retainer x) => x.ItemCount > 0, out List<uint> validRetainers) == 0)
			{
				return;
			}
			validRetainers.ForEach(delegate(uint index)
			{
				taskHelper.Enqueue(() => taskHelper.AbortByConflictKey(Module) || Module.EnterRetainer(index), $"选择进入 {index} 号雇员");
				taskHelper.Enqueue(() => taskHelper.AbortByConflictKey(Module) || AddonSelectStringEvent.Select(ItemEntrustWithdrawTexts), "选择道具管理");
				taskHelper.Enqueue(delegate
				{
					if (!Module.retainerThrottler.Throttle("AutoRetainerEntrustDups", 100u))
					{
						return false;
					}
					if (taskHelper.AbortByConflictKey(Module))
					{
						return true;
					}
					AgentInterface* agentByInternalId = ((AgentModule)AgentModule.Instance()).GetAgentByInternalId((AgentId)70);
					if (agentByInternalId == null || !((AgentInterface)agentByInternalId).IsAgentActive())
					{
						return false;
					}
					((AgentId)70).SendEvent(0uL, 0);
					return true;
				}, "选择同类道具合并提交");
				taskHelper.DelayNext(500, "等待同类道具合并提交开始");
				taskHelper.Enqueue(() => taskHelper.AbortByConflictKey(Module) || ExitRetainerInventory(), "离开雇员背包界面");
				taskHelper.Enqueue(() => taskHelper.AbortByConflictKey(Module) || LeaveRetainer(), "回到雇员列表");
			});
		}

		private unsafe void OnEntrustDupsAddons(AddonEvent type, AddonArgs args)
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			if (!taskHelper.IsBusy)
			{
				return;
			}
			string addonName = args.AddonName;
			if (!(addonName == "RetainerItemTransferList"))
			{
				if (!(addonName == "RetainerItemTransferProgress"))
				{
					return;
				}
				taskHelper.Enqueue(delegate
				{
					if (taskHelper.AbortByConflictKey(Module))
					{
						return true;
					}
					AtkUnitBase* byName = AddonHelper.GetByName("RetainerItemTransferProgress");
					if (!(*byName).IsAddonAndNodesReady())
					{
						return false;
					}
					if (((AtkValue)((byte*)((AtkUnitBase)byName).AtkValues + (nint)2 * (nint)Unsafe.SizeOf<AtkValue>())).Float == 1f)
					{
						(*byName).Callback(-2);
						((AtkUnitBase)byName).Close(true);
						return true;
					}
					return false;
				}, "等待同类道具合并提交完成", 0, null, null, null, null, 2);
			}
			else
			{
				(*args.Addon.ToStruct()).Callback(1);
			}
		}
	}

	private class GilsShareWorker : RetainerWorkerBase
	{
		private TaskHelper? taskHelper;

		private const uint MAX_PLAYER_GIL = 999999999u;

		private static readonly string[] GilManageTexts = new string[7] { "金币管理", "Gil管理", "Entrust or withdraw gil", "ギルの受け渡し", "길 주고받기", "Gil geben oder nehmen", "Confier ou récupérer de l'argent" };

		public GilsShareWorker(AutoRetainerWork module)
			: base(module)
		{
		}

		public override bool DrawConfigCondition()
		{
			return false;
		}

		public override bool IsWorkerBusy()
		{
			return taskHelper?.IsBusy ?? false;
		}

		public override void Init()
		{
			if (taskHelper == null)
			{
				taskHelper = new TaskHelper
				{
					TimeoutMS = 15000
				};
			}
		}

		public override void Uninit()
		{
			taskHelper?.Abort();
			taskHelper?.Dispose();
			taskHelper = null;
		}

		public override CollaspingCategoryNode CreateOverlayCategory(float width)
		{
			return RetainerWorkerBase.CreateOverlayCategory(LanguageManager.Get("AutoRetainerWork-GilsShare-Title"), width, RetainerWorkerBase.CreateOverlayButtonRow(EnqueueRetainersGilShare, delegate
			{
				taskHelper?.Abort();
			}, width));
		}

		private unsafe void EnqueueRetainersGilShare()
		{
			if (taskHelper.AbortByConflictKey(Module) || Module.IsAnyOtherWorkerBusy(typeof(GilsShareWorker)))
			{
				return;
			}
			uint num = LocalPlayerState.GetItemCount(1u);
			if (num >= 999999999)
			{
				OmenServiceBase<NotifyHelper>.Instance().NotificationWarning(LanguageManager.Get("AutoRetainerWork-GilsShare-PlayerGilFull"), Module.Info.Title);
				return;
			}
			RetainerManager* ptr = RetainerManager.Instance();
			byte retainerCount = ((RetainerManager)ptr).GetRetainerCount();
			uint num2 = 0u;
			List<uint> validRetainers;
			for (uint num3 = 0u; num3 < GetValidRetainerCount((Retainer _) => true, out validRetainers); num3++)
			{
				num2 += ((Retainer)((RetainerManager)ptr).GetRetainerBySortedIndex(num3)).Gil;
			}
			uint num4 = (uint)Math.Floor((double)num2 / (double)(int)retainerCount);
			if (num4 <= 1)
			{
				OmenServiceBase<NotifyHelper>.Instance().NotificationInfo(LanguageManager.Get("AutoRetainerWork-GilsShare-NoNeedToShare"), Module.Info.Title);
				return;
			}
			List<(uint, uint)> list = new List<(uint, uint)>();
			List<(uint, uint)> list2 = new List<(uint, uint)>();
			for (uint num5 = 0u; num5 < retainerCount; num5++)
			{
				uint gil = ((Retainer)((RetainerManager)ptr).GetRetainerBySortedIndex(num5)).Gil;
				if (gil > num4)
				{
					list.Add((num5, gil - num4));
				}
				else if (gil < num4)
				{
					list2.Add((num5, num4 - gil));
				}
			}
			if (list.Count == 0)
			{
				OmenServiceBase<NotifyHelper>.Instance().NotificationInfo(LanguageManager.Get("AutoRetainerWork-GilsShare-NoNeedToShare"), Module.Info.Title);
				return;
			}
			List<(uint, uint, bool)> list3 = new List<(uint, uint, bool)>();
			int num6 = 0;
			int num7 = 0;
			uint num8 = list[0].Item2;
			uint num9 = ((list2.Count > 0) ? list2[0].Item2 : 0u);
			while (num6 < list.Count || num7 < list2.Count)
			{
				bool flag = false;
				while (num7 < list2.Count && num != 0 && num9 != 0)
				{
					uint num10 = Math.Min(num, num9);
					list3.Add((list2[num7].Item1, num10, false));
					num -= num10;
					num9 -= num10;
					flag = true;
					if (num9 == 0)
					{
						num7++;
						if (num7 < list2.Count)
						{
							num9 = list2[num7].Item2;
						}
					}
				}
				if (num6 < list.Count && num8 != 0)
				{
					uint num11 = 999999999 - num;
					if (num11 != 0)
					{
						uint num12 = Math.Min(num8, num11);
						list3.Add((list[num6].Item1, num12, true));
						num += num12;
						num8 -= num12;
						flag = true;
						if (num8 == 0)
						{
							num6++;
							if (num6 < list.Count)
							{
								num8 = list[num6].Item2;
							}
						}
					}
				}
				if (!flag)
				{
					break;
				}
			}
			foreach (var (index, amount, isWithdraw) in list3)
			{
				EnqueueRetainerGilOperation(index, amount, isWithdraw);
			}
			taskHelper.Enqueue(delegate
			{
				OmenServiceBase<NotifyHelper>.Instance().NotificationSuccess(LanguageManager.Get("AutoRetainerWork-GilsShare-Complete"), Module.Info.Title);
				return true;
			}, "发送完成通知");
		}

		private unsafe void EnqueueRetainerGilOperation(uint index, uint amount, bool isWithdraw)
		{
			taskHelper.Enqueue(() => taskHelper.AbortByConflictKey(Module) || Module.EnterRetainer(index), $"选择进入 {index} 号雇员");
			taskHelper.Enqueue(() => taskHelper.AbortByConflictKey(Module) || AddonSelectStringEvent.Select(GilManageTexts), "选择进入金币管理");
			taskHelper.Enqueue(delegate
			{
				if (taskHelper.AbortByConflictKey(Module))
				{
					return true;
				}
				if (!(*Addons.Bank).IsAddonAndNodesReady())
				{
					return false;
				}
				if (!isWithdraw)
				{
					AddonBankEvent.SwitchMode();
				}
				AddonBankEvent.SetNumber(amount);
				AddonBankEvent.ClickConfirm();
				((AtkUnitBase)Addons.Bank).Close(true);
				return true;
			}, $"{(isWithdraw ? "取出" : "存入")} {amount} 金币 ({index} 号雇员)");
			taskHelper.Enqueue(() => taskHelper.AbortByConflictKey(Module) || LeaveRetainer(), "回到雇员列表");
		}
	}

	private class GilsWithdrawWorker : RetainerWorkerBase
	{
		private TaskHelper? TaskHelper;

		private static readonly string[] GilManageTexts = new string[7] { "金币管理", "Gil管理", "Entrust or withdraw gil", "ギルの受け渡し", "길 주고받기", "Gil geben oder nehmen", "Confier ou récupérer de l'argent" };

		public GilsWithdrawWorker(AutoRetainerWork module)
			: base(module)
		{
		}

		public override bool DrawConfigCondition()
		{
			return false;
		}

		public override bool IsWorkerBusy()
		{
			return TaskHelper?.IsBusy ?? false;
		}

		public override void Init()
		{
			if (TaskHelper == null)
			{
				TaskHelper = new TaskHelper
				{
					TimeoutMS = 15000
				};
			}
		}

		public override void Uninit()
		{
			TaskHelper?.Abort();
			TaskHelper?.Dispose();
			TaskHelper = null;
		}

		public override CollaspingCategoryNode CreateOverlayCategory(float width)
		{
			return RetainerWorkerBase.CreateOverlayCategory(LanguageManager.Get("AutoRetainerWork-GilsWithdraw-Title"), width, RetainerWorkerBase.CreateOverlayButtonRow(EnqueueRetainersGilWithdraw, delegate
			{
				TaskHelper?.Abort();
			}, width));
		}

		private unsafe void EnqueueRetainersGilWithdraw()
		{
			if (TaskHelper.AbortByConflictKey(Module) || Module.IsAnyOtherWorkerBusy(typeof(GilsWithdrawWorker)) || GetValidRetainerCount((Retainer x) => x.Gil != 0, out List<uint> validRetainers) == 0)
			{
				return;
			}
			validRetainers.ForEach(delegate(uint index)
			{
				TaskHelper.Enqueue(() => TaskHelper.AbortByConflictKey(Module) || Module.EnterRetainer(index), $"选择进入 {index} 号雇员");
				TaskHelper.Enqueue(() => TaskHelper.AbortByConflictKey(Module) || AddonSelectStringEvent.Select(GilManageTexts), "选择进入金币管理");
				TaskHelper.Enqueue(delegate
				{
					if (TaskHelper.AbortByConflictKey(Module))
					{
						return true;
					}
					if (!(*Addons.Bank).IsAddonAndNodesReady())
					{
						return false;
					}
					int retainerGilAmount = AddonBankEvent.RetainerGilAmount;
					if (retainerGilAmount <= 0)
					{
						AddonBankEvent.ClickCancel();
					}
					else
					{
						AddonBankEvent.SetNumber((uint)retainerGilAmount);
						AddonBankEvent.ClickConfirm();
					}
					((AtkUnitBase)Addons.Bank).Close(true);
					return true;
				}, "取出所有的金币");
				TaskHelper.Enqueue(() => TaskHelper.AbortByConflictKey(Module) || LeaveRetainer(), "回到雇员列表");
			});
		}
	}

	private class PriceAdjustWorker(AutoRetainerWork module) : RetainerWorkerBase(module)
	{
		private class PriceAdjustContextMenuEntry(PriceAdjustWorker worker) : ContextMenuEntry
		{
			public override string Identifier => "AutoRetainerWork";

			public unsafe override IReadOnlyList<ContextMenuItem>? CreateMultiple(ContextMenuOpenedArgs args)
			{
				//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
				//IL_0124: Unknown result type (might be due to invalid IL or missing references)
				if (args.AddonName != "RetainerSellList")
				{
					return null;
				}
				AgentRetainer* ptr = AgentRetainer.Instance();
				if (ptr->ContextMenuIndex < 0 || ptr->SellListEntryCount == 0 || ptr->ContextMenuIndex >= ptr->SellListEntryCount)
				{
					return null;
				}
				InventoryManager* ptr2 = InventoryManager.Instance();
				AgentRetainer.SellListEntry sellListEntry = ptr->SellListEntries[ptr->ContextMenuIndex];
				InventoryItem* inventoryItem = ((InventoryManager)ptr2).GetInventorySlot((InventoryType)12002, (int)sellListEntry.InventorySlot);
				if (inventoryItem == null)
				{
					return null;
				}
				uint itemID = ((InventoryItem)inventoryItem).GetBaseItemId();
				if (!LuminaGetter.TryGetRow<Item>(itemID, out Item _))
				{
					return null;
				}
				return new global::_003C_003Ez__ReadOnlyArray<ContextMenuItem>(new ContextMenuItem[3]
				{
					new ContextMenuItem
					{
						Name = ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoRetainerWork-PriceAdjust-AutoAdjustPrice")),
						OnClicked = delegate
						{
							worker.EnqueuePriceAdjustSlot(((InventoryItem)inventoryItem).GetSlot());
						}
					},
					new ContextMenuItem
					{
						Name = ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoRetainerWork-PriceAdjust-ManualAdjustPrice-AllSame")),
						OnClicked = delegate
						{
							//IL_001c: Unknown result type (might be due to invalid IL or missing references)
							worker.isPriceAdjustAllSameItems = true;
							AgentRetainer.Instance()->OpenRetainerSell(((InventoryItem)inventoryItem).GetInventoryType(), ((InventoryItem)inventoryItem).GetSlot());
						}
					},
					new ContextMenuItem
					{
						Name = ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoRetainerWork-PriceAdjust-AutoAdjustPrice-AllSame")),
						OnClicked = delegate
						{
							if (TryGetSameItemSlots(itemID, out List<ushort> slots))
							{
								foreach (ushort item2 in slots)
								{
									worker.EnqueuePriceAdjustSlot(item2);
								}
							}
						}
					}
				});
			}
		}

		private class PriceAdjustAddon(PriceAdjustWorker worker) : AttachedAddon("RetainerSellList")
		{
			protected override bool CanOpenAddon => DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)50];

			public TextButtonNode? PriceAdjustButton { get; private set; }

			public CheckboxNode? AutoAdjustPriceCheckbox { get; private set; }

			public CheckboxNode? AutoOnSaleCheckbox { get; private set; }

			public CheckboxNode? NotifyPriceAdjustCheckbox { get; private set; }

			protected unsafe override void OnSetup(AtkUnitBase* addon, Span<AtkValue> atkValues)
			{
				//IL_006e: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
				//IL_015b: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
				//IL_0257: Unknown result type (might be due to invalid IL or missing references)
				//IL_026c: Unknown result type (might be due to invalid IL or missing references)
				//IL_02db: Unknown result type (might be due to invalid IL or missing references)
				//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
				//IL_035f: Unknown result type (might be due to invalid IL or missing references)
				HorizontalListNode horizontalListNode = new HorizontalListNode();
				horizontalListNode.Alignment = HorizontalListAnchor.Right;
				horizontalListNode.Position = base.ContentStartPosition + new Vector2(base.ContentSize.X - 1f, 0f);
				horizontalListNode.FitToContentWidth = true;
				horizontalListNode.AttachNode(this);
				CircleButtonNode node = new CircleButtonNode
				{
					Icon = CircleButtonIcon.GearCog,
					Size = new Vector2(28f),
					TextTooltip = ReadOnlySeString.op_Implicit(LanguageManager.Get("Settings")),
					OnClick = delegate
					{
						OmenServiceBase<ChatManager>.Instance().SendCommand("/pdr search AutoRetainerWork");
					}
				};
				horizontalListNode.AddNode(node);
				CircleButtonNode node2 = new CircleButtonNode
				{
					Icon = CircleButtonIcon.Refresh,
					TextTooltip = ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoRetainerWork-PriceAdjust-ClearCache")),
					Size = new Vector2(28f),
					OnClick = delegate
					{
						PriceCacheManager.ClearCache();
						NotifyHelper.Toast(LanguageManager.Get("AutoRetainerWork-PriceAdjust-CacheCleared"));
					}
				};
				horizontalListNode.AddNode(node2);
				VerticalListNode verticalListNode = new VerticalListNode
				{
					ItemSpacing = 5f,
					FirstItemSpacing = 30f,
					Position = base.ContentStartPosition,
					Width = base.ContentSize.X,
					FitContents = true
				};
				verticalListNode.AttachNode(this);
				PriceAdjustButton = new TextButtonNode
				{
					String = ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoRetainerWork-PriceAdjust-AutoAdjustPrice-Batch")),
					Size = new Vector2(verticalListNode.Width, 36f),
					TextureType = ButtonTextureType.ButtonB,
					OnClick = delegate
					{
						if (worker.taskHelper.IsBusy)
						{
							worker.taskHelper.Abort();
						}
						else
						{
							worker.EnqueuePriceAdjustRetainer();
						}
					}
				};
				verticalListNode.AddNode(PriceAdjustButton);
				TextButtonNode node3 = new TextButtonNode
				{
					String = ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoRetainerWork-PriceAdjust-ReturnAllToInventory")),
					Size = new Vector2(verticalListNode.Width, 28f),
					OnClick = delegate
					{
						InventoryContainer* inventoryContainer = ((InventoryManager)InventoryManager.Instance()).GetInventoryContainer((InventoryType)12002);
						for (int i = 0; i < ((InventoryContainer)inventoryContainer).Size; i++)
						{
							int index = i;
							worker.taskHelper.Enqueue(() => worker.ReturnRetainerMarketItemToInventory((ushort)index, isInventory: true), $"将市场中的第{index}栏物品收回至自己");
						}
					}
				};
				verticalListNode.AddNode(node3);
				TextButtonNode node4 = new TextButtonNode
				{
					String = ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoRetainerWork-PriceAdjust-ReturnAllToRetainer")),
					Size = new Vector2(verticalListNode.Width, 28f),
					OnClick = delegate
					{
						InventoryContainer* inventoryContainer = ((InventoryManager)InventoryManager.Instance()).GetInventoryContainer((InventoryType)12002);
						for (int i = 0; i < ((InventoryContainer)inventoryContainer).Size; i++)
						{
							int index = i;
							worker.taskHelper.Enqueue(() => worker.ReturnRetainerMarketItemToInventory((ushort)index, isInventory: false), $"将市场中的第{index}栏物品收回至雇员");
						}
					}
				};
				verticalListNode.AddNode(node4);
				verticalListNode.AddDummy(2f);
				AutoAdjustPriceCheckbox = new CheckboxNode
				{
					String = ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoRetainerWork-PriceAdjust-AutoAdjustWhenNewOnSale")),
					TextTooltip = ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoRetainerWork-PriceAdjust-AutoAdjustWhenNewOnSale-Help")),
					Size = new Vector2(verticalListNode.Width, 28f),
					IsChecked = worker.Module.config.AutoPriceAdjustWhenNewOnSale,
					OnClick = delegate(bool value)
					{
						worker.Module.config.AutoPriceAdjustWhenNewOnSale = value;
						worker.Module.config.Save(worker.Module);
					}
				};
				verticalListNode.AddNode(AutoAdjustPriceCheckbox);
				AutoOnSaleCheckbox = new CheckboxNode
				{
					String = ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoRetainerWork-PriceAdjust-AutoOnSale")),
					TextTooltip = ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoRetainerWork-PriceAdjust-AutoOnSale-Help")),
					Size = new Vector2(verticalListNode.Width, 28f),
					IsChecked = worker.Module.config.AutoOnSale,
					OnClick = delegate(bool value)
					{
						worker.Module.config.AutoOnSale = value;
						worker.Module.config.Save(worker.Module);
					}
				};
				verticalListNode.AddNode(AutoOnSaleCheckbox);
				NotifyPriceAdjustCheckbox = new CheckboxNode
				{
					String = ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoRetainerWork-PriceAdjust-SendProcessMessage")),
					Size = new Vector2(verticalListNode.Width, 28f),
					IsChecked = worker.Module.config.SendPriceAdjustProcessMessage,
					OnClick = delegate(bool value)
					{
						worker.Module.config.SendPriceAdjustProcessMessage = value;
						worker.Module.config.Save(worker.Module);
					}
				};
				verticalListNode.AddNode(NotifyPriceAdjustCheckbox);
				verticalListNode.RecalculateLayout();
				SetWindowSize(base.Size.X, base.ContentStartPosition.Y + verticalListNode.Height + 20f);
				verticalListNode.Position = base.ContentStartPosition;
			}

			protected unsafe override void OnAttachedAddonUpdate(AtkUnitBase* addon, AtkUnitBase* hostAddon)
			{
				//IL_003a: Unknown result type (might be due to invalid IL or missing references)
				TextButtonNode? priceAdjustButton = PriceAdjustButton;
				if (priceAdjustButton != null)
				{
					TaskHelper? taskHelper = worker.taskHelper;
					priceAdjustButton.String = ReadOnlySeString.op_Implicit((taskHelper != null && taskHelper.IsBusy) ? LanguageManager.Get("Stop") : LanguageManager.Get("AutoRetainerWork-PriceAdjust-AutoAdjustPrice-Batch"));
				}
			}
		}

		public static class PriceCacheManager
		{
			private static class CacheKeys
			{
				public static string Create(uint itemID, bool isHQ)
				{
					return $"{itemID}_{(isHQ ? "HQ" : "NQ")}";
				}
			}

			private const int CACHE_EXPIRATION_MINUTES = 10;

			private static readonly PriceCache CurrentPriceCache = new PriceCache();

			private static readonly PriceCache HistoryPriceCache = new PriceCache();

			public static void UpdateCache<T>(AutoRetainerWork module, PriceCache cache, uint itemID, IEnumerable<T> listings, Func<T, bool> isHQSelector, Func<T, bool> onMannequinSelector, Func<T, uint> priceSelector, Func<T, ulong> retainerSelector = null)
			{
				ILookup<bool, T> lookup = listings.Where((T x) => !onMannequinSelector(x)).ToLookup(isHQSelector);
				bool[] array = new bool[2] { false, true };
				foreach (bool flag in array)
				{
					IEnumerable<T> enumerable = lookup[flag];
					if (retainerSelector != null)
					{
						enumerable = enumerable.Where((T x) => !module.playerRetainers.Contains(retainerSelector(x)));
					}
					T[] array2 = (enumerable as T[]) ?? enumerable.ToArray();
					uint num2 = ((array2.Length != 0) ? array2.Min(priceSelector) : 0u);
					if (num2 != 0)
					{
						string key = CacheKeys.Create(itemID, flag);
						if (!cache.TryGetPrice(key, out var price) || num2 < price)
						{
							cache.SetPrice(key, num2);
						}
					}
				}
			}

			public static void UpdateHistoryCache<T>(PriceCache cache, uint itemID, IEnumerable<T> listings, Func<T, bool> isHQSelector, Func<T, bool> onMannequinSelector, Func<T, uint> priceSelector)
			{
				ILookup<bool, T> lookup = listings.Where((T x) => !onMannequinSelector(x)).ToLookup(isHQSelector);
				bool[] array = new bool[2] { false, true };
				foreach (bool flag in array)
				{
					IEnumerable<T> enumerable = lookup[flag];
					T[] array2 = (enumerable as T[]) ?? enumerable.ToArray();
					uint num2 = ((array2.Length != 0) ? array2.Max(priceSelector) : 0u);
					if (num2 != 0)
					{
						string key = CacheKeys.Create(itemID, flag);
						if (!cache.TryGetPrice(key, out var price) || num2 > price)
						{
							cache.SetPrice(key, num2);
						}
					}
				}
			}

			public static void OnOfferingReceived(AutoRetainerWork module, IMarketBoardCurrentOfferings data)
			{
				if (data.ItemListings.Any())
				{
					UpdateCache(module, CurrentPriceCache, data.ItemListings[0].ItemId, data.ItemListings, (IMarketBoardItemListing x) => x.IsHq, (IMarketBoardItemListing x) => x.OnMannequin, (IMarketBoardItemListing x) => x.PricePerUnit, (IMarketBoardItemListing x) => x.RetainerId);
				}
			}

			public static void OnHistoryReceived(IMarketBoardHistory history)
			{
				if (history.HistoryListings.Any())
				{
					UpdateHistoryCache(HistoryPriceCache, history.ItemId, history.HistoryListings, (IMarketBoardHistoryListing x) => x.IsHq, (IMarketBoardHistoryListing x) => x.OnMannequin, (IMarketBoardHistoryListing x) => x.SalePrice);
				}
			}

			public static bool TryGetPriceCache(uint itemID, bool isHQ, out uint price)
			{
				price = 0u;
				string key = CacheKeys.Create(itemID, isHQ);
				string key2 = CacheKeys.Create(itemID, !isHQ);
				CurrentPriceCache.RemoveExpiredEntries(TimeSpan.FromMinutes(10L));
				HistoryPriceCache.RemoveExpiredEntries(TimeSpan.FromMinutes(10L));
				if (CurrentPriceCache.TryGetPrice(key, out price) || CurrentPriceCache.TryGetPrice(key2, out price) || HistoryPriceCache.TryGetPrice(key, out price) || HistoryPriceCache.TryGetPrice(key2, out price))
				{
					return price != 0;
				}
				return false;
			}

			public static (DateTime Current, DateTime History) GetCacheTimes()
			{
				return (Current: CurrentPriceCache.LastUpdateTime, History: HistoryPriceCache.LastUpdateTime);
			}

			public static void ClearCache(bool clearCurrent = true, bool clearHistory = true)
			{
				if (clearCurrent)
				{
					CurrentPriceCache.Clear();
				}
				if (clearHistory)
				{
					HistoryPriceCache.Clear();
				}
			}
		}

		public sealed class PriceCache
		{
			private class CacheEntry
			{
				public uint Price { get; init; }

				public DateTime LastUpdateTime { get; init; }
			}

			private readonly Dictionary<string, CacheEntry> data = new Dictionary<string, CacheEntry>();

			public DateTime LastUpdateTime { get; private set; } = DateTime.MinValue;

			public void RemoveExpiredEntries(TimeSpan expirationTime)
			{
				DateTime now = OmenServiceBase<StandardTimeManager>.Instance().Now;
				foreach (string item in (from kvp in data
					where now - kvp.Value.LastUpdateTime > expirationTime
					select kvp.Key).ToList())
				{
					data.Remove(item);
				}
				if (!data.Any())
				{
					LastUpdateTime = DateTime.MinValue;
				}
			}

			public bool TryGetPrice(string key, out uint price)
			{
				price = 0u;
				if (data.TryGetValue(key, out CacheEntry value))
				{
					price = value.Price;
					return true;
				}
				return false;
			}

			public void SetPrice(string key, uint price)
			{
				data[key] = new CacheEntry
				{
					Price = price,
					LastUpdateTime = OmenServiceBase<StandardTimeManager>.Instance().Now
				};
				LastUpdateTime = OmenServiceBase<StandardTimeManager>.Instance().Now;
			}

			public void Clear()
			{
				data.Clear();
				LastUpdateTime = DateTime.MinValue;
			}
		}

		[CompilerGenerated]
		private static class _003C_003EO
		{
			public static HistoryReceivedDelegate _003C0_003E__OnHistoryReceived;
		}

		private Hook<MoveToRetainerMarket>? MoveToRetainerMarketHook;

		private TaskHelper? taskHelper;

		private ItemSelectCombo itemSelectCombo;

		private ItemConfig? selectedItemConfig;

		private readonly Vector2 childSizeLeft = Globals.ScaledVector2(200f, 400f);

		private Vector2 childSizeRight = Globals.ScaledVector2(450f, 400f);

		private string presetSearchInput = string.Empty;

		private bool newConfigItemHQ;

		private AbortCondition conditionInput = AbortCondition.低于最小值;

		private AbortBehavior behaviorInput;

		private PriceAdjustContextMenuEntry? contextMenuEntry;

		private PriceAdjustAddon? priceAdjustAddon;

		private AtkEventWrapper? openMarketEvent;

		private AtkEventWrapper? priceAdjustAllSameEvent;

		private ResNode? autoPriceAdjustWarningNode;

		private bool isPriceAdjustAllSameItems;

		private static readonly string[] SellInventoryItemsText = new string[6] { "玩家所持物品", "Sell items in your inventory", "プレイヤー所持品から", "플레이어 소지품에서 선택", "Gegenstände aus dem eigenen Inventar verkaufen", "Mettre en vente un objet de votre inventaire" };

		public override bool IsWorkerBusy()
		{
			return taskHelper?.IsBusy ?? false;
		}

		public unsafe override void Init()
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Expected O, but got Unknown
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Expected O, but got Unknown
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Expected O, but got Unknown
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Expected O, but got Unknown
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Expected O, but got Unknown
			itemSelectCombo = new ItemSelectCombo("AddNewItem");
			if (MoveToRetainerMarketHook == null)
			{
				MoveToRetainerMarketHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromMemberFunction<MoveToRetainerMarket>(typeof(MemberFunctionPointers), "MoveToRetainerMarket", new MoveToRetainerMarket(MoveToRetainerMarketDetour));
			}
			MoveToRetainerMarketHook.Enable();
			if (taskHelper == null)
			{
				taskHelper = new TaskHelper
				{
					TimeoutMS = 30000,
					ShowDebug = true
				};
			}
			taskHelper.EnterBusyAction = delegate
			{
				ToggleOverlayIPC.TryInvokeFunc(true);
			};
			taskHelper.LeaveBusyAction = delegate
			{
				ToggleOverlayIPC.TryInvokeFunc(false);
			};
			contextMenuEntry = new PriceAdjustContextMenuEntry(this);
			priceAdjustAddon = new PriceAdjustAddon(this)
			{
				InternalName = "DRAutoRetainerWorkPriceAdjust",
				Title = ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoRetainerWork-PriceAdjust-Title")),
				Size = new Vector2(260f, 320f)
			};
			IMarketBoard obj = DalamudServiceExtension.Instance<IMarketBoard>();
			object obj2 = _003C_003EO._003C0_003E__OnHistoryReceived;
			if (obj2 == null)
			{
				HistoryReceivedDelegate val = OnHistoryReceived;
				_003C_003EO._003C0_003E__OnHistoryReceived = val;
				obj2 = (object)val;
			}
			obj.HistoryReceived += (HistoryReceivedDelegate)obj2;
			DalamudServiceExtension.Instance<IMarketBoard>().OfferingsReceived += new OfferingsReceivedDelegate(OnOfferingReceived);
			DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)1, "RetainerSell", new AddonEventDelegate(OnRetainerSell));
			DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, "RetainerSell", new AddonEventDelegate(OnRetainerSell));
			if ((*Addons.RetainerSell).IsAddonAndNodesReady())
			{
				OnRetainerSell((AddonEvent)1, null);
			}
			OmenServiceBase<ContextMenuManager>.Instance().Reg(contextMenuEntry);
		}

		public override void Uninit()
		{
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Expected O, but got Unknown
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Expected O, but got Unknown
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Expected O, but got Unknown
			OmenServiceBase<ContextMenuManager>.Instance().Unreg(contextMenuEntry);
			priceAdjustAddon?.Dispose();
			priceAdjustAddon = null;
			openMarketEvent?.Dispose();
			openMarketEvent = null;
			priceAdjustAllSameEvent?.Dispose();
			priceAdjustAllSameEvent = null;
			MoveToRetainerMarketHook?.Dispose();
			MoveToRetainerMarketHook = null;
			DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnRetainerSell });
			autoPriceAdjustWarningNode?.Dispose();
			autoPriceAdjustWarningNode = null;
			IMarketBoard obj = DalamudServiceExtension.Instance<IMarketBoard>();
			object obj2 = _003C_003EO._003C0_003E__OnHistoryReceived;
			if (obj2 == null)
			{
				HistoryReceivedDelegate val = OnHistoryReceived;
				_003C_003EO._003C0_003E__OnHistoryReceived = val;
				obj2 = (object)val;
			}
			obj.HistoryReceived -= (HistoryReceivedDelegate)obj2;
			DalamudServiceExtension.Instance<IMarketBoard>().OfferingsReceived -= new OfferingsReceivedDelegate(OnOfferingReceived);
			PriceCacheManager.ClearCache();
			contextMenuEntry = null;
			taskHelper?.Abort();
			taskHelper?.Dispose();
			taskHelper = null;
		}

		public override void DrawConfig()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			Vector4 vector = KnownColor.Chocolate.ToVector4();
			ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("AutoRetainerWork-PriceAdjust-Title")));
			ItemConfigSelector();
			ImGui.SameLine();
			ItemConfigEditor();
		}

		public override CollaspingCategoryNode CreateOverlayCategory(float width)
		{
			return RetainerWorkerBase.CreateOverlayCategory(LanguageManager.Get("AutoRetainerWork-PriceAdjust-Title"), width, RetainerWorkerBase.CreateOverlayText(LanguageManager.Get("AutoRetainerWork-PriceAdjust-AutoAdjustPrice-AllRetainers"), width), RetainerWorkerBase.CreateOverlayButtonRow(delegate
			{
				TaskHelper taskHelper = this.taskHelper;
				if (taskHelper != null && !taskHelper.IsBusy)
				{
					EnqueuePriceAdjustAllRetainers();
				}
			}, delegate
			{
				taskHelper?.Abort();
			}, width), RetainerWorkerBase.CreateOverlayCheckbox(LanguageManager.Get("AutoRetainerWork-PriceAdjust-SendProcessMessage"), Module.config.SendPriceAdjustProcessMessage, delegate(bool isChecked)
			{
				Module.config.SendPriceAdjustProcessMessage = isChecked;
				Module.config.Save(Module);
			}, width));
		}

		private void ItemConfigSelector()
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0382: Unknown result type (might be due to invalid IL or missing references)
			//IL_0387: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_03af: Unknown result type (might be due to invalid IL or missing references)
			//IL_0351: Unknown result type (might be due to invalid IL or missing references)
			//IL_0356: Unknown result type (might be due to invalid IL or missing references)
			//IL_0377: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			ChildDisposable val = ImRaii.Child(ImU8String.op_Implicit("ItemConfigSelectorChild"), childSizeLeft, true);
			try
			{
				if (!val)
				{
					return;
				}
				if (ImGuiOm.ButtonIcon("AddNewConfig", (FontAwesomeIcon)61543, LanguageManager.Get("Add")))
				{
					ImGui.OpenPopup(ImU8String.op_Implicit("AddNewPreset"), (ImGuiPopupFlags)0);
				}
				ImGui.SameLine();
				if (ImGuiOm.ButtonIcon("ImportConfig", (FontAwesomeIcon)62831, LanguageManager.Get("ImportFromClipboard")))
				{
					ItemConfig itemConfig = ModuleBase.ImportFromClipboard<ItemConfig>();
					if (itemConfig != null)
					{
						string key = new ItemKey(itemConfig.ItemID, itemConfig.IsHQ).ToString();
						Module.config.ItemConfigs[key] = itemConfig;
					}
				}
				PopupDisposable val2 = ImRaii.Popup(ImU8String.op_Implicit("AddNewPreset"));
				try
				{
					if (PopupDisposable.op_Implicit(val2))
					{
						AddNewConfigItemPopup(delegate
						{
							string key2 = new ItemKey(itemSelectCombo.SelectedID, newConfigItemHQ).ToString();
							ItemConfig value2 = new ItemConfig(itemSelectCombo.SelectedID, newConfigItemHQ);
							if (Module.config.ItemConfigs.TryAdd(key2, value2))
							{
								Module.config.Save(Module);
								ImGui.CloseCurrentPopup();
							}
						});
					}
				}
				finally
				{
					((PopupDisposable)(ref val2)).Dispose();
				}
				ImGui.SameLine();
				ImGui.SetNextItemWidth(-1f);
				ImGui.InputTextWithHint(ImU8String.op_Implicit("###PresetSearchInput"), ImU8String.op_Implicit(LanguageManager.Get("PleaseSearch")), ref presetSearchInput, 100, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
				ImGui.Separator();
				foreach (KeyValuePair<string, ItemConfig> itemConfig2 in Module.config.ItemConfigs.ToList())
				{
					if (!string.IsNullOrWhiteSpace(presetSearchInput) && !itemConfig2.Value.ItemName.Contains(presetSearchInput))
					{
						continue;
					}
					ImU8String val3 = new ImU8String(1, 2);
					((ImU8String)(ref val3)).AppendFormatted<string>(itemConfig2.Value.ItemName);
					((ImU8String)(ref val3)).AppendLiteral(" ");
					((ImU8String)(ref val3)).AppendFormatted<string>(itemConfig2.Value.IsHQ ? "(HQ)" : "");
					if (ImGui.Selectable(val3, itemConfig2.Value == selectedItemConfig, (ImGuiSelectableFlags)0, default(Vector2)))
					{
						selectedItemConfig = itemConfig2.Value;
					}
					bool flag = false;
					ImU8String val4 = new ImU8String(2, 3);
					((ImU8String)(ref val4)).AppendFormatted<ItemConfig>(itemConfig2.Value);
					((ImU8String)(ref val4)).AppendLiteral("_");
					((ImU8String)(ref val4)).AppendFormatted<string>(itemConfig2.Key);
					((ImU8String)(ref val4)).AppendLiteral("_");
					((ImU8String)(ref val4)).AppendFormatted<uint>(itemConfig2.Value.ItemID);
					PopupDisposable val5 = ImRaii.ContextPopupItem(val4);
					try
					{
						if (PopupDisposable.op_Implicit(val5))
						{
							if (ImGui.MenuItem(ImU8String.op_Implicit(LanguageManager.Get("ExportToClipboard")), false, true))
							{
								ModuleBase.ExportToClipboard(itemConfig2.Value);
							}
							if (ImGui.MenuItem(ImU8String.op_Implicit(LanguageManager.Get("AutoRetainerWork-PriceAdjust-CreateNewBaseOnExisted")), false, true))
							{
								flag = true;
							}
							if (itemConfig2.Value.ItemID != 0 && ImGui.MenuItem(ImU8String.op_Implicit(LanguageManager.Get("Delete")), false, true))
							{
								Module.config.ItemConfigs.Remove(itemConfig2.Key);
								Module.config.Save(Module);
								selectedItemConfig = null;
							}
						}
					}
					finally
					{
						((PopupDisposable)(ref val5)).Dispose();
					}
					if (flag)
					{
						ImU8String val6 = new ImU8String(27, 1);
						((ImU8String)(ref val6)).AppendLiteral("AddNewPresetBasedOnExisted_");
						((ImU8String)(ref val6)).AppendFormatted<string>(itemConfig2.Key);
						ImGui.OpenPopup(val6, (ImGuiPopupFlags)0);
					}
					ImU8String val7 = new ImU8String(27, 1);
					((ImU8String)(ref val7)).AppendLiteral("AddNewPresetBasedOnExisted_");
					((ImU8String)(ref val7)).AppendFormatted<string>(itemConfig2.Key);
					PopupDisposable val8 = ImRaii.Popup(val7);
					try
					{
						if (PopupDisposable.op_Implicit(val8))
						{
							AddNewConfigItemPopup(delegate
							{
								//IL_005e: Unknown result type (might be due to invalid IL or missing references)
								//IL_0063: Unknown result type (might be due to invalid IL or missing references)
								//IL_0066: Unknown result type (might be due to invalid IL or missing references)
								//IL_006b: Unknown result type (might be due to invalid IL or missing references)
								string key2 = new ItemKey(itemSelectCombo.SelectedID, newConfigItemHQ).ToString();
								ItemConfig obj = new ItemConfig
								{
									ItemID = itemSelectCombo.SelectedID,
									IsHQ = newConfigItemHQ
								};
								Item selectedItem = itemSelectCombo.SelectedItem;
								obj.ItemName = ((object)((Item)(ref selectedItem)).Name/*cast due to constrained. prefix*/).ToString() ?? string.Empty;
								obj.AbortLogic = itemConfig2.Value.AbortLogic;
								obj.AdjustBehavior = itemConfig2.Value.AdjustBehavior;
								obj.AdjustValues = itemConfig2.Value.AdjustValues;
								obj.PriceExpected = itemConfig2.Value.PriceExpected;
								obj.PriceMaximum = itemConfig2.Value.PriceMaximum;
								obj.PriceMaxReduction = itemConfig2.Value.PriceMaxReduction;
								obj.PriceMinimum = itemConfig2.Value.PriceMinimum;
								ItemConfig value2 = obj;
								if (Module.config.ItemConfigs.TryAdd(key2, value2))
								{
									Module.config.Save(Module);
									ImGui.CloseCurrentPopup();
								}
							});
						}
					}
					finally
					{
						((PopupDisposable)(ref val8)).Dispose();
					}
					ItemConfig value = itemConfig2.Value;
					if ((object)value != null && value.ItemID == 0 && value.IsHQ)
					{
						ImGui.Separator();
					}
				}
			}
			finally
			{
				((ChildDisposable)(ref val)).Dispose();
			}
		}

		private void AddNewConfigItemPopup(Action confirmAction)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
			itemSelectCombo.DrawRadio();
			ImGui.SameLine();
			ImGui.Checkbox(ImU8String.op_Implicit("HQ"), ref newConfigItemHQ);
			ImGui.SameLine();
			if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("Confirm")), default(Vector2)))
			{
				confirmAction();
			}
		}

		private void ItemConfigEditor()
		{
			//Discarded unreachable code: IL_056d
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03be: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0491: Unknown result type (might be due to invalid IL or missing references)
			//IL_049c: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0506: Unknown result type (might be due to invalid IL or missing references)
			//IL_0511: Unknown result type (might be due to invalid IL or missing references)
			//IL_0517: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0603: Unknown result type (might be due to invalid IL or missing references)
			//IL_0663: Unknown result type (might be due to invalid IL or missing references)
			//IL_066e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0674: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_06dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0791: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0726: Unknown result type (might be due to invalid IL or missing references)
			//IL_0874: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0925: Unknown result type (might be due to invalid IL or missing references)
			//IL_092a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0941: Unknown result type (might be due to invalid IL or missing references)
			//IL_0984: Unknown result type (might be due to invalid IL or missing references)
			//IL_0989: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_095d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0962: Unknown result type (might be due to invalid IL or missing references)
			//IL_0979: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aa2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ad7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0adc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b39: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b3e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b55: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b57: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b5c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b12: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b17: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b2e: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b5e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b86: Unknown result type (might be due to invalid IL or missing references)
			ref Vector2 reference = ref childSizeRight;
			float x = ImGui.GetContentRegionAvail().X;
			ImGuiStylePtr style = ImGui.GetStyle();
			reference.X = x - ((ImGuiStylePtr)(ref style)).ItemSpacing.X;
			ChildDisposable val = ImRaii.Child(ImU8String.op_Implicit("ItemConfigEditorChild"), childSizeRight, true);
			try
			{
				if (selectedItemConfig == null || !LuminaGetter.TryGetRow<Item>(selectedItemConfig.ItemID, out Item item))
				{
					return;
				}
				string text = ((selectedItemConfig.ItemID == 0) ? LanguageManager.Get("AutoRetainerWork-PriceAdjust-CommonItemPreset") : (((object)((Item)(ref item)).Name/*cast due to constrained. prefix*/).ToString() ?? string.Empty));
				ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
				GameIconLookup val2 = new GameIconLookup((uint)((selectedItemConfig.ItemID == 0) ? 65002 : ((Item)(ref item)).Icon), selectedItemConfig.IsHQ, true, (ClientLanguage?)null);
				IDalamudTextureWrap wrapOrDefault = obj.GetFromGameIcon(ref val2).GetWrapOrDefault((IDalamudTextureWrap)null);
				if (wrapOrDefault == null)
				{
					return;
				}
				uint val3 = ((selectedItemConfig.ItemID == 0) ? 1u : ((Item)(ref item)).PriceLow);
				if (!val)
				{
					return;
				}
				ImGui.Image(wrapOrDefault.Handle, Globals.ScaledVector2(48f));
				ImGui.SameLine();
				using (OmenServiceBase<FontManager>.Instance().UIFont140.Push())
				{
					ImGui.TextUnformatted(ImU8String.op_Implicit(text));
				}
				ImGui.SameLine();
				ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 6f * Globals.GlobalUIScale);
				ImGui.TextUnformatted(ImU8String.op_Implicit(selectedItemConfig.IsHQ ? ("(" + LanguageManager.Get("HQ") + ")") : string.Empty));
				ImGui.Separator();
				GroupDisposable val4 = ImRaii.Group();
				try
				{
					AdjustBehavior[] values = Enum.GetValues<AdjustBehavior>();
					foreach (AdjustBehavior adjustBehavior in values)
					{
						if (ImGui.RadioButton(ImU8String.op_Implicit(AdjustBehaviorLoc.GetValueOrDefault(adjustBehavior)), adjustBehavior == selectedItemConfig.AdjustBehavior))
						{
							selectedItemConfig.AdjustBehavior = adjustBehavior;
							Module.config.Save(Module);
						}
					}
				}
				finally
				{
					((GroupDisposable)(ref val4)).Dispose();
				}
				ImGui.SameLine();
				GroupDisposable val5 = ImRaii.Group();
				ImU8String val7;
				try
				{
					if (selectedItemConfig.AdjustBehavior == AdjustBehavior.固定值)
					{
						int value = selectedItemConfig.AdjustValues[AdjustBehavior.固定值];
						ImGui.SetNextItemWidth(100f * Globals.GlobalUIScale);
						ImU8String val6 = ImU8String.op_Implicit(LanguageManager.Get("AutoRetainerWork-PriceAdjust-ValueReduction"));
						val7 = default(ImU8String);
						ImGui.InputInt(val6, ref value, 0, 0, val7, (ImGuiInputTextFlags)0);
						if (ImGui.IsItemDeactivatedAfterEdit())
						{
							selectedItemConfig.AdjustValues[AdjustBehavior.固定值] = value;
							Module.config.Save(Module);
						}
					}
					else
					{
						ImGui.Dummy(new Vector2(ImGui.GetTextLineHeightWithSpacing()));
					}
					if (selectedItemConfig.AdjustBehavior == AdjustBehavior.百分比)
					{
						int value2 = selectedItemConfig.AdjustValues[AdjustBehavior.百分比];
						ImGui.SetNextItemWidth(100f * Globals.GlobalUIScale);
						ImU8String val8 = ImU8String.op_Implicit(LanguageManager.Get("AutoRetainerWork-PriceAdjust-PercentageReduction"));
						val7 = default(ImU8String);
						ImGui.InputInt(val8, ref value2, 0, 0, val7, (ImGuiInputTextFlags)0);
						if (ImGui.IsItemDeactivatedAfterEdit())
						{
							selectedItemConfig.AdjustValues[AdjustBehavior.百分比] = Math.Clamp(value2, -99, 99);
							Module.config.Save(Module);
						}
					}
					else
					{
						ImGui.Dummy(new Vector2(ImGui.GetTextLineHeightWithSpacing()));
					}
				}
				finally
				{
					((GroupDisposable)(ref val5)).Dispose();
				}
				ImGuiOm.ScaledDummy(10f);
				int priceMinimum = selectedItemConfig.PriceMinimum;
				ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
				ImU8String val9 = ImU8String.op_Implicit(LanguageManager.Get("AutoRetainerWork-PriceAdjust-PriceMinimum"));
				val7 = default(ImU8String);
				ImGui.InputInt(val9, ref priceMinimum, 0, 0, val7, (ImGuiInputTextFlags)0);
				if (ImGui.IsItemDeactivatedAfterEdit())
				{
					selectedItemConfig.PriceMinimum = Math.Max(1, priceMinimum);
					Module.config.Save(Module);
				}
				ImGui.SameLine();
				DisabledDisposable val10 = ImRaii.Disabled(selectedItemConfig.ItemID == 0);
				try
				{
					if (ImGuiOm.ButtonIcon("ObtainBuyingPrice", (FontAwesomeIcon)62798, LanguageManager.Get("AutoRetainerWork-PriceAdjust-ObtainBuyingPrice")))
					{
						selectedItemConfig.PriceMinimum = Math.Max(1, (int)val3);
						Module.config.Save(Module);
					}
				}
				finally
				{
					((IDisposable)val10)?.Dispose();
				}
				int priceMaximum = selectedItemConfig.PriceMaximum;
				ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
				ImU8String val11 = ImU8String.op_Implicit(LanguageManager.Get("AutoRetainerWork-PriceAdjust-PriceMaximum"));
				val7 = default(ImU8String);
				ImGui.InputInt(val11, ref priceMaximum, 0, 0, val7, (ImGuiInputTextFlags)0);
				if (ImGui.IsItemDeactivatedAfterEdit())
				{
					selectedItemConfig.PriceMaximum = Math.Min(int.MaxValue, priceMaximum);
					Module.config.Save(Module);
				}
				int priceExpected = selectedItemConfig.PriceExpected;
				ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
				ImU8String val12 = ImU8String.op_Implicit(LanguageManager.Get("AutoRetainerWork-PriceAdjust-PriceExpected"));
				val7 = default(ImU8String);
				ImGui.InputInt(val12, ref priceExpected, 0, 0, val7, (ImGuiInputTextFlags)0);
				if (ImGui.IsItemDeactivatedAfterEdit())
				{
					selectedItemConfig.PriceExpected = Math.Max(priceMinimum + 1, priceExpected);
					Module.config.Save(Module);
				}
				int priceMaxReduction = selectedItemConfig.PriceMaxReduction;
				ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
				ImU8String val13 = ImU8String.op_Implicit(LanguageManager.Get("AutoRetainerWork-PriceAdjust-PriceMaxReduction"));
				val7 = default(ImU8String);
				ImGui.InputInt(val13, ref priceMaxReduction, 0, 0, val7, (ImGuiInputTextFlags)0);
				if (ImGui.IsItemDeactivatedAfterEdit())
				{
					selectedItemConfig.PriceMaxReduction = Math.Max(0, priceMaxReduction);
					Module.config.Save(Module);
				}
				int upshelfCount = selectedItemConfig.UpshelfCount;
				ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
				ImU8String val14 = ImU8String.op_Implicit(LanguageManager.Get("AutoRetainerWork-PriceAdjust-UpshelfCount"));
				val7 = default(ImU8String);
				ImGui.InputInt(val14, ref upshelfCount, 0, 0, val7, (ImGuiInputTextFlags)0);
				if (ImGui.IsItemDeactivatedAfterEdit())
				{
					selectedItemConfig.UpshelfCount = upshelfCount;
					Module.config.Save(Module);
				}
				ImGuiOm.ScaledDummy(10f);
				GroupDisposable val15 = ImRaii.Group();
				try
				{
					ImGui.SetNextItemWidth(250f * Globals.GlobalUIScale);
					ComboDisposable val16 = ImRaii.Combo(ImU8String.op_Implicit("###AddNewLogicConditionCombo"), ImU8String.op_Implicit(GetAbortConditionName(conditionInput)), (ImGuiComboFlags)8);
					try
					{
						if (ComboDisposable.op_Implicit(val16))
						{
							AbortCondition[] abortConditions = AbortConditions;
							foreach (AbortCondition abortCondition in abortConditions)
							{
								if (abortCondition != AbortCondition.无)
								{
									bool flag = (conditionInput & abortCondition) == abortCondition;
									if (ImGui.Selectable(ImU8String.op_Implicit(AbortConditionLoc.GetValueOrDefault(abortCondition)), flag, (ImGuiSelectableFlags)1, default(Vector2)))
									{
										AbortCondition abortCondition2 = conditionInput;
										abortCondition2 = ((!flag) ? (abortCondition2 | abortCondition) : (abortCondition2 & ~abortCondition));
										conditionInput = abortCondition2;
									}
								}
							}
						}
					}
					finally
					{
						((ComboDisposable)(ref val16)).Dispose();
					}
					ImGui.SetNextItemWidth(250f * Globals.GlobalUIScale);
					ComboDisposable val17 = ImRaii.Combo(ImU8String.op_Implicit("###AddNewLogicBehaviorCombo"), ImU8String.op_Implicit(AbortBehaviorLoc.GetValueOrDefault(behaviorInput)), (ImGuiComboFlags)8);
					try
					{
						if (ComboDisposable.op_Implicit(val17))
						{
							foreach (AbortBehavior value4 in Enum.GetValues(typeof(AbortBehavior)))
							{
								if (ImGui.Selectable(ImU8String.op_Implicit(AbortBehaviorLoc.GetValueOrDefault(value4)), behaviorInput == value4, (ImGuiSelectableFlags)1, default(Vector2)))
								{
									behaviorInput = value4;
								}
							}
						}
					}
					finally
					{
						((ComboDisposable)(ref val17)).Dispose();
					}
				}
				finally
				{
					((GroupDisposable)(ref val15)).Dispose();
				}
				Vector2 itemRectSize = ImGui.GetItemRectSize();
				ImGui.SameLine();
				string text2 = LanguageManager.Get("Add");
				Vector2 buttonSize = itemRectSize;
				buttonSize.X = ImGui.CalcTextSize(ImU8String.op_Implicit(LanguageManager.Get("Add")), false, -1f).X * 2f;
				if (ImGuiOm.ButtonIconWithTextVertical((FontAwesomeIcon)61543, text2, buttonSize) && conditionInput != AbortCondition.无)
				{
					selectedItemConfig.AbortLogic.TryAdd(conditionInput, behaviorInput);
					Module.config.Save(Module);
				}
				ImGui.Separator();
				foreach (KeyValuePair<AbortCondition, AbortBehavior> item2 in selectedItemConfig.AbortLogic.ToList())
				{
					string abortConditionName = GetAbortConditionName(item2.Key);
					ImGui.SetNextItemWidth(300f * Globals.GlobalUIScale);
					val7 = new ImU8String(13, 1);
					((ImU8String)(ref val7)).AppendLiteral("###Condition_");
					((ImU8String)(ref val7)).AppendFormatted<string>(abortConditionName);
					ImGui.InputText(val7, ref abortConditionName, 100, (ImGuiInputTextFlags)16384, (ImGuiInputTextCallbackDelegate)null);
					if (ImGui.IsItemClicked())
					{
						ImU8String val18 = new ImU8String(24, 1);
						((ImU8String)(ref val18)).AppendLiteral("###ConditionSelectPopup_");
						((ImU8String)(ref val18)).AppendFormatted<string>(abortConditionName);
						ImGui.OpenPopup(val18, (ImGuiPopupFlags)0);
					}
					ImU8String val19 = new ImU8String(24, 1);
					((ImU8String)(ref val19)).AppendLiteral("###ConditionSelectPopup_");
					((ImU8String)(ref val19)).AppendFormatted<string>(abortConditionName);
					PopupDisposable val20 = ImRaii.Popup(val19);
					try
					{
						if (PopupDisposable.op_Implicit(val20))
						{
							AbortCondition[] abortConditions = AbortConditions;
							foreach (AbortCondition abortCondition3 in abortConditions)
							{
								bool flag2 = (item2.Key & abortCondition3) == abortCondition3;
								if (ImGui.Selectable(ImU8String.op_Implicit(AbortConditionLoc.GetValueOrDefault(abortCondition3)), flag2, (ImGuiSelectableFlags)0, default(Vector2)))
								{
									AbortCondition key = item2.Key;
									key = ((!flag2) ? (key | abortCondition3) : (key & ~abortCondition3));
									if (!selectedItemConfig.AbortLogic.ContainsKey(key))
									{
										AbortBehavior value3 = item2.Value;
										selectedItemConfig.AbortLogic[key] = value3;
										selectedItemConfig.AbortLogic.Remove(item2.Key);
										Module.config.Save(Module);
									}
								}
							}
						}
					}
					finally
					{
						((PopupDisposable)(ref val20)).Dispose();
					}
					ImGui.SameLine();
					ImGui.TextUnformatted(ImU8String.op_Implicit("→"));
					string valueOrDefault = AbortBehaviorLoc.GetValueOrDefault(item2.Value);
					ImGui.SameLine();
					ImGui.SetNextItemWidth(300f * Globals.GlobalUIScale);
					ImU8String val21 = new ImU8String(12, 1);
					((ImU8String)(ref val21)).AppendLiteral("###Behavior_");
					((ImU8String)(ref val21)).AppendFormatted<string>(valueOrDefault);
					ImGui.InputText(val21, ref valueOrDefault, 128, (ImGuiInputTextFlags)16384, (ImGuiInputTextCallbackDelegate)null);
					if (ImGui.IsItemClicked())
					{
						ImU8String val22 = new ImU8String(23, 1);
						((ImU8String)(ref val22)).AppendLiteral("###BehaviorSelectPopup_");
						((ImU8String)(ref val22)).AppendFormatted<string>(valueOrDefault);
						ImGui.OpenPopup(val22, (ImGuiPopupFlags)0);
					}
					ImU8String val23 = new ImU8String(23, 1);
					((ImU8String)(ref val23)).AppendLiteral("###BehaviorSelectPopup_");
					((ImU8String)(ref val23)).AppendFormatted<string>(valueOrDefault);
					PopupDisposable val24 = ImRaii.Popup(val23);
					try
					{
						if (PopupDisposable.op_Implicit(val24))
						{
							AbortBehavior[] values2 = Enum.GetValues<AbortBehavior>();
							foreach (AbortBehavior abortBehavior2 in values2)
							{
								if (ImGui.Selectable(ImU8String.op_Implicit(AbortBehaviorLoc.GetValueOrDefault(abortBehavior2)), abortBehavior2 == item2.Value, (ImGuiSelectableFlags)0, default(Vector2)))
								{
									selectedItemConfig.AbortLogic[item2.Key] = abortBehavior2;
									Module.config.Save(Module);
								}
							}
						}
					}
					finally
					{
						((PopupDisposable)(ref val24)).Dispose();
					}
					ImGui.SameLine();
					if (ImGuiOm.ButtonIcon($"Delete_{item2.Key}_{item2.Value}", (FontAwesomeIcon)62189, LanguageManager.Get("Delete")))
					{
						selectedItemConfig.AbortLogic.Remove(item2.Key);
					}
				}
			}
			finally
			{
				((ChildDisposable)(ref val)).Dispose();
			}
		}

		private unsafe void OnRetainerSell(AddonEvent type, AddonArgs args)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Invalid comparison between Unknown and I4
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Invalid comparison between Unknown and I4
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Invalid comparison between Unknown and I4
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			if (!DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)50])
			{
				return;
			}
			if ((int)type != 1)
			{
				if ((int)type == 6)
				{
					if (!taskHelper.IsBusy)
					{
						ToggleOverlayIPC.TryInvokeFunc(false);
					}
					autoPriceAdjustWarningNode = null;
					openMarketEvent?.Dispose();
					openMarketEvent = null;
					priceAdjustAllSameEvent?.Dispose();
					priceAdjustAllSameEvent = null;
					isPriceAdjustAllSameItems = false;
				}
				return;
			}
			InventoryItem* slot = ((InventoryManager)InventoryManager.Instance()).GetInventorySlot(AgentRetainer.Instance()->SellItemInventoryType, (int)AgentRetainer.Instance()->SellItemInventorySlot);
			if (slot == null)
			{
				return;
			}
			if ((int)AgentRetainer.Instance()->SellItemInventoryType != 12002)
			{
				ItemConfig itemConfigByItemKey = GetItemConfigByItemKey(new ItemKey(((InventoryItem)slot).GetBaseItemId(), ((InventoryItem)slot).IsHighQuality()));
				if (itemConfigByItemKey.UpshelfCount > 0)
				{
					AtkComponentNumericInput* componentByNodeId = (AtkComponentNumericInput*)((AtkUnitBase)Addons.RetainerSell).GetComponentByNodeId(14u);
					if (componentByNodeId != null)
					{
						((AtkComponentNumericInput)componentByNodeId).InnerSetValue(itemConfigByItemKey.UpshelfCount, true, false);
					}
				}
				if (Module.config.AutoOnSale)
				{
					(*Addons.RetainerSell).Callback(0);
					return;
				}
				if (Module.config.AutoPriceAdjustWhenNewOnSale)
				{
					AtkComponentNumericInput* componentByNodeId2 = (AtkComponentNumericInput*)((AtkUnitBase)Addons.RetainerSell).GetComponentByNodeId(14u);
					AtkComponentNumericInput* componentByNodeId3 = (AtkComponentNumericInput*)((AtkUnitBase)Addons.RetainerSell).GetComponentByNodeId(10u);
					if (componentByNodeId2 != null && componentByNodeId3 != null)
					{
						((AtkComponentNumericInput)componentByNodeId3).SetEnabledState(false);
						AtkComponentNode* ownerNode = ((AtkComponentNumericInput)componentByNodeId3).OwnerNode;
						if (ownerNode == null)
						{
							return;
						}
						AtkResNode* parentNode = ((AtkComponentNode)ownerNode).ParentNode;
						if (parentNode == null)
						{
							return;
						}
						autoPriceAdjustWarningNode = new ResNode
						{
							Size = new Vector2((int)((AtkComponentNode)ownerNode).Width, (int)((AtkComponentNode)ownerNode).Height),
							Position = new Vector2(((AtkComponentNode)ownerNode).X, ((AtkComponentNode)ownerNode).Y),
							TextTooltip = ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoRetainerWork-PriceAdjust-AutoAdjustWhenNewOnSale-Warning"))
						};
						autoPriceAdjustWarningNode.AttachNode(parentNode);
					}
				}
			}
			AtkComponentButton* componentButtonById = ((AtkUnitBase)Addons.RetainerSell).GetComponentButtonById(4u);
			if (componentButtonById != null)
			{
				((AtkComponentNode)((AtkComponentButton)componentButtonById).OwnerNode).ClearEvents();
				openMarketEvent = new AtkEventWrapper(delegate
				{
					RequestMarketItemData(((InventoryItem)slot).GetBaseItemId(), openOverlay: true);
				});
				openMarketEvent.Add(Addons.RetainerSell, (AtkResNode*)((AtkComponentButton)componentButtonById).OwnerNode, (AtkEventType)25);
			}
			if (!isPriceAdjustAllSameItems)
			{
				return;
			}
			AtkComponentButton* componentButtonById2 = ((AtkUnitBase)Addons.RetainerSell).GetComponentButtonById(21u);
			if (componentButtonById2 == null)
			{
				return;
			}
			((AtkComponentNode)((AtkComponentButton)componentButtonById2).OwnerNode).ClearEvents();
			priceAdjustAllSameEvent = new AtkEventWrapper(delegate
			{
				if (TryGetSameItemSlots(((InventoryItem)slot).GetBaseItemId(), out List<ushort> slots))
				{
					foreach (ushort item in slots)
					{
						EnqueuePriceAdjustSlot(item, (uint)AgentRetainer.Instance()->SellItemUnitPrice);
					}
				}
				((AtkUnitBase)Addons.RetainerSell).Close(true);
				isPriceAdjustAllSameItems = false;
			});
			priceAdjustAllSameEvent.Add(Addons.RetainerSell, (AtkResNode*)((AtkComponentButton)componentButtonById2).OwnerNode, (AtkEventType)25);
		}

		private void OnOfferingReceived(IMarketBoardCurrentOfferings data)
		{
			PriceCacheManager.OnOfferingReceived(Module, data);
		}

		private static void OnHistoryReceived(IMarketBoardHistory history)
		{
			PriceCacheManager.OnHistoryReceived(history);
		}

		private unsafe void MoveToRetainerMarketDetour(InventoryManager* manager, InventoryType srcInv, ushort srcSlot, InventoryType dstInv, ushort dstSlot, uint quantity, uint unitPrice)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			if (((InventoryManager)manager).GetInventorySlot(srcInv, (int)srcSlot) == null)
			{
				InvokeOriginal();
			}
			else if (Module.config.AutoPriceAdjustWhenNewOnSale && !PluginConfig.Instance().ConflictKeyBinding.IsPressed())
			{
				MoveToRetainerMarketHook.Original.Invoke(manager, srcInv, srcSlot, dstInv, dstSlot, quantity, 999999999u);
				EnqueuePriceAdjustSlot(dstSlot);
			}
			else
			{
				InvokeOriginal();
			}
			unsafe void InvokeOriginal()
			{
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				MoveToRetainerMarketHook.Original.Invoke(manager, srcInv, srcSlot, dstInv, dstSlot, quantity, unitPrice);
			}
		}

		internal unsafe void EnqueuePriceAdjustAllRetainers()
		{
			if (taskHelper.AbortByConflictKey(Module) || Module.IsAnyOtherWorkerBusy(typeof(PriceAdjustWorker)) || GetValidRetainerCount((Retainer x) => x.Available && x.MarketItemCount > 0, out List<uint> validRetainers) == 0)
			{
				return;
			}
			validRetainers.ForEach(delegate(uint index)
			{
				taskHelper.Enqueue(() => taskHelper.AbortByConflictKey(Module) || Module.EnterRetainer(index), $"选择进入 {index} 号雇员");
				taskHelper.Enqueue(delegate
				{
					if (taskHelper.AbortByConflictKey(Module))
					{
						return true;
					}
					return (*Addons.SelectString).IsAddonAndNodesReady() && ((RetainerManager)RetainerManager.Instance()).GetActiveRetainer() != null;
				}, $"等待接收 {index} 号雇员的数据");
				taskHelper.Enqueue(() => taskHelper.AbortByConflictKey(Module) || AddonSelectStringEvent.Select(SellInventoryItemsText), "点击进入出售玩家所持物品列表");
				taskHelper.Enqueue(delegate
				{
					if (!taskHelper.AbortByConflictKey(Module))
					{
						EnqueuePriceAdjustRetainer();
					}
				}, "由单一雇员商品改价接管后续逻辑");
				taskHelper.Enqueue(delegate
				{
					if (!taskHelper.AbortByConflictKey(Module) && (*Addons.RetainerSellList).IsAddonAndNodesReady())
					{
						(*Addons.RetainerSellList).Callback(-1);
					}
				}, "单一雇员改价完成, 退出出售品列表界面");
				taskHelper.Enqueue(() => taskHelper.AbortByConflictKey(Module) || LeaveRetainer(), "单一雇员改价完成, 返回至雇员列表界面");
			});
		}

		private unsafe void EnqueuePriceAdjustRetainer()
		{
			if (taskHelper.AbortByConflictKey(Module) || Module.IsAnyOtherWorkerBusy(typeof(PriceAdjustWorker)))
			{
				return;
			}
			Retainer* activeRetainer = ((RetainerManager)RetainerManager.Instance()).GetActiveRetainer();
			if (activeRetainer == null || ((Retainer)activeRetainer).MarketItemCount <= 0)
			{
				return;
			}
			InventoryContainer* inventoryContainer = ((InventoryManager)InventoryManager.Instance()).GetInventoryContainer((InventoryType)12002);
			if (inventoryContainer != null && ((InventoryContainer)inventoryContainer).IsLoaded)
			{
				for (ushort num = 0; num < ((InventoryContainer)inventoryContainer).Size; num++)
				{
					EnqueuePriceAdjustSlot(num);
				}
			}
		}

		private unsafe void EnqueuePriceAdjustSlot(ushort slotIndex, uint forcePrice = 0u)
		{
			if (taskHelper.AbortByConflictKey(Module) || Module.IsAnyOtherWorkerBusy(typeof(PriceAdjustWorker)))
			{
				return;
			}
			taskHelper.Enqueue(delegate
			{
				//IL_0087: Unknown result type (might be due to invalid IL or missing references)
				//IL_008c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0090: Unknown result type (might be due to invalid IL or missing references)
				//IL_0080: Unknown result type (might be due to invalid IL or missing references)
				//IL_0095: Unknown result type (might be due to invalid IL or missing references)
				//IL_0099: Unknown result type (might be due to invalid IL or missing references)
				//IL_013d: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
				Retainer* activeRetainer = ((RetainerManager)RetainerManager.Instance()).GetActiveRetainer();
				if (activeRetainer != null)
				{
					InventoryContainer* inventoryContainer = ((InventoryManager)InventoryManager.Instance()).GetInventoryContainer((InventoryType)12002);
					if (inventoryContainer != null && ((InventoryContainer)inventoryContainer).IsLoaded)
					{
						InventoryItem* inventorySlot = ((InventoryContainer)inventoryContainer).GetInventorySlot((int)slotIndex);
						uint itemID = ((InventoryItem)inventorySlot).ItemId;
						if (inventorySlot != null && ((InventoryItem)inventorySlot).ItemId != 0)
						{
							Item? row = LuminaGetter.GetRow<Item>(itemID);
							ReadOnlySeString val;
							if (!row.HasValue)
							{
								val = ReadOnlySeString.op_Implicit(string.Empty);
							}
							else
							{
								Item valueOrDefault = row.GetValueOrDefault();
								val = ((Item)(ref valueOrDefault)).Name;
							}
							ReadOnlySeString value = val;
							bool isItemHQ = ((Enum)((InventoryItem)inventorySlot).Flags).HasFlag((Enum)(object)(ItemFlags)1);
							if (!PriceCacheManager.TryGetPriceCache(itemID, isItemHQ, out var price))
							{
								bool num = ((InfoProxyItemSearch)InfoProxyItemSearch.Instance()).SearchItemId == 0;
								taskHelper.Enqueue(delegate
								{
									if (!taskHelper.AbortByConflictKey(Module))
									{
										RequestMarketItemData(itemID, openOverlay: false);
									}
								}, $"请求雇员 {((Retainer)activeRetainer).NameString} {slotIndex} 号位置处 {value} 的市场价格数据", 0, null, null, null, null, 2);
								if (num)
								{
									taskHelper.DelayNext(1000, "初始无数据, 等待 1 秒", 2);
								}
								taskHelper.Enqueue(() => taskHelper.AbortByConflictKey(Module) || IsMarketItemDataReady(itemID), $"等待 {value} 市场价格数据完全到达", 0, null, null, null, null, 2);
								taskHelper.Enqueue(delegate
								{
									if (!taskHelper.AbortByConflictKey(Module))
									{
										if (!PriceCacheManager.TryGetPriceCache(itemID, isItemHQ, out price))
										{
											price = 0u;
										}
										EnqueuePriceAdjustSingleItem(slotIndex, price, forcePrice);
									}
								}, "由单一物品改价接管后续逻辑", 0, null, null, null, null, 2);
							}
							else
							{
								taskHelper.Enqueue(delegate
								{
									EnqueuePriceAdjustSingleItem(slotIndex, price, forcePrice);
								}, "由单一物品改价接管后续逻辑", 0, null, null, null, null, 2);
							}
						}
					}
				}
			}, $"检查当前市场第 {slotIndex} 栏的物品数据, 强制价格: {forcePrice}", 0, null, null, null, null, 1);
		}

		private void EnqueuePriceAdjustSingleItem(ushort slot, uint marketPrice, uint forcePrice = 0u)
		{
			if (taskHelper.AbortByConflictKey(Module) || Module.IsAnyOtherWorkerBusy(typeof(PriceAdjustWorker)))
			{
				return;
			}
			(ItemKey Item, uint Price)? itemMarketData = GetRetainerMarketItem(slot);
			if (!itemMarketData.HasValue)
			{
				return;
			}
			ItemConfig itemConfig = GetItemConfigByItemKey(itemMarketData.Value.Item);
			uint num = ((forcePrice != 0) ? forcePrice : GetModifiedPrice(itemConfig, marketPrice));
			if (num != 0 && num != itemMarketData.Value.Price)
			{
				if (IsAnyAbortConditionsMet(itemConfig, itemMarketData.Value.Price, num, marketPrice, out var conditionMet, out var behaviorNeeded))
				{
					NotifyAbortCondition(itemMarketData.Value.Item.ItemID, itemMarketData.Value.Item.IsHQ, conditionMet);
					EnqueueAbortBehavior(behaviorNeeded);
				}
				else
				{
					SetRetainerMarketItemPrice(slot, num);
					NotifyPriceAdjustSuccessfully(itemMarketData.Value.Item.ItemID, itemMarketData.Value.Item.IsHQ, itemMarketData.Value.Price, num);
				}
			}
			unsafe void EnqueueAbortBehavior(AbortBehavior behavior)
			{
				//IL_0024: Unknown result type (might be due to invalid IL or missing references)
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_0046: Unknown result type (might be due to invalid IL or missing references)
				//IL_004c: Unknown result type (might be due to invalid IL or missing references)
				if (Module.config.SendPriceAdjustProcessMessage)
				{
					ReadOnlySeString se = LanguageManager.GetSe("AutoRetainerWork-PriceAdjust-ConductAbortBehavior", new SeStringBuilder().AddUiForeground(AbortBehaviorLoc.GetValueOrDefault(behavior), (ushort)67).Build());
					OmenServiceBase<NotifyHelper>.Instance().Chat(se);
				}
				switch (behavior)
				{
				case AbortBehavior.改价至最小值:
					SetRetainerMarketItemPrice(slot, (uint)itemConfig.PriceMinimum);
					NotifyPriceAdjustSuccessfully(itemMarketData.Value.Item.ItemID, itemMarketData.Value.Item.IsHQ, itemMarketData.Value.Price, (uint)itemConfig.PriceMinimum);
					break;
				case AbortBehavior.改价至预期值:
					SetRetainerMarketItemPrice(slot, (uint)itemConfig.PriceExpected);
					NotifyPriceAdjustSuccessfully(itemMarketData.Value.Item.ItemID, itemMarketData.Value.Item.IsHQ, itemMarketData.Value.Price, (uint)itemConfig.PriceExpected);
					break;
				case AbortBehavior.改价至最高值:
					SetRetainerMarketItemPrice(slot, (uint)itemConfig.PriceMaximum);
					NotifyPriceAdjustSuccessfully(itemMarketData.Value.Item.ItemID, itemMarketData.Value.Item.IsHQ, itemMarketData.Value.Price, (uint)itemConfig.PriceMaximum);
					break;
				case AbortBehavior.收回至雇员:
					ReturnRetainerMarketItemToInventory(slot, isInventory: false);
					break;
				case AbortBehavior.收回至背包:
					ReturnRetainerMarketItemToInventory(slot, isInventory: true);
					break;
				case AbortBehavior.出售至系统商店:
				{
					taskHelper.Enqueue(() => ReturnRetainerMarketItemToInventory(slot, isInventory: true), "将物品收回背包, 以待出售", 0, null, null, null, null, 3);
					taskHelper.Enqueue(() => TrySearchItemInInventory(itemMarketData.Value.Item.ItemID, itemMarketData.Value.Item.IsHQ, out List<InventoryItem> foundItem) && foundItem != null && foundItem.Count > 0 && foundItem.FirstOrDefault().OpenContext(), "找到物品并打开其右键菜单", 0, null, null, null, null, 3);
					taskHelper.Enqueue(() => (*Addons.ContextMenuAddon).IsAddonAndNodesReady(), "等待右键菜单出现", 0, null, null, null, null, 3);
					taskHelper.Enqueue(() => AddonContextMenuEvent.Select(LuminaWrapper.GetAddonText(5480u)), "出售物品至系统商店", 0, null, null, null, null, 3);
					break;
				}
				}
			}
		}

		private ItemConfig GetItemConfigByItemKey(ItemKey key)
		{
			if (!Module.config.ItemConfigs.TryGetValue(key.ToString(), out ItemConfig value))
			{
				return Module.config.ItemConfigs[new ItemKey(0u, key.IsHQ).ToString()];
			}
			return value;
		}

		private unsafe bool ReturnRetainerMarketItemToInventory(ushort slot, bool isInventory)
		{
			if (!Module.retainerThrottler.Throttle("ReturnMarketItemToInventory", 100u))
			{
				return false;
			}
			InventoryManager* ptr = InventoryManager.Instance();
			if (ptr == null)
			{
				return false;
			}
			InventoryContainer* inventoryContainer = ((InventoryManager)ptr).GetInventoryContainer((InventoryType)12002);
			if (inventoryContainer == null || !((InventoryContainer)inventoryContainer).IsLoaded)
			{
				return false;
			}
			InventoryItem* inventorySlot = ((InventoryContainer)inventoryContainer).GetInventorySlot((int)slot);
			if (inventorySlot == null || ((InventoryItem)inventorySlot).ItemId == 0)
			{
				return true;
			}
			if (isInventory)
			{
				((InventoryManager)InventoryManager.Instance()).MoveFromRetainerMarketToPlayerInventory((InventoryType)12002, slot, (uint)((InventoryItem)inventorySlot).Quantity);
			}
			else
			{
				((InventoryManager)InventoryManager.Instance()).MoveFromRetainerMarketToRetainerInventory((InventoryType)12002, slot, (uint)((InventoryItem)inventorySlot).Quantity);
			}
			return false;
		}

		private unsafe static bool SetRetainerMarketItemPrice(ushort slot, uint price)
		{
			if (slot >= 20)
			{
				return false;
			}
			InventoryManager* ptr = InventoryManager.Instance();
			if (ptr == null)
			{
				return false;
			}
			((InventoryManager)ptr).SetRetainerMarketPrice((short)slot, price);
			((RaptureAtkModule)RaptureAtkModule.Instance()).AgentUpdateFlag |= (AgentUpdateFlags)4;
			return true;
		}

		private unsafe static (ItemKey Item, uint Price)? GetRetainerMarketItem(ushort slot)
		{
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			if (slot >= 20)
			{
				return null;
			}
			InventoryManager* ptr = InventoryManager.Instance();
			if (ptr == null)
			{
				return null;
			}
			InventoryContainer* inventoryContainer = ((InventoryManager)ptr).GetInventoryContainer((InventoryType)12002);
			if (inventoryContainer == null || !((InventoryContainer)inventoryContainer).IsLoaded)
			{
				return null;
			}
			InventoryItem* inventorySlot = ((InventoryContainer)inventoryContainer).GetInventorySlot((int)slot);
			if (inventorySlot == null)
			{
				return null;
			}
			return (new ItemKey(((InventoryItem)inventorySlot).ItemId, ((Enum)((InventoryItem)inventorySlot).Flags).HasFlag((Enum)(object)(ItemFlags)1)), GetRetainerMarketPrice(slot));
		}

		private unsafe static uint GetRetainerMarketPrice(ushort slot)
		{
			if (slot >= 20)
			{
				return 0u;
			}
			InventoryManager* ptr = InventoryManager.Instance();
			if (ptr == null)
			{
				return 0u;
			}
			return (uint)((InventoryManager)ptr).GetRetainerMarketPrice((short)slot);
		}

		private unsafe static void RequestMarketItemData(uint itemID, bool openOverlay)
		{
			if (((InfoProxyItemSearch)InfoProxyItemSearch.Instance()).SearchItemId != itemID)
			{
				SearchItemIPC.InvokeFunc(itemID);
			}
			if (openOverlay)
			{
				ToggleOverlayIPC.InvokeFunc(true);
			}
		}

		private unsafe static bool IsMarketItemDataReady(uint itemID)
		{
			InfoProxyItemSearch* ptr = InfoProxyItemSearch.Instance();
			if (ptr == null)
			{
				return false;
			}
			return (*ptr).IsFullyReceived(itemID);
		}

		private static bool IsAnyAbortConditionsMet(ItemConfig config, uint origPrice, uint modifiedPrice, uint marketPrice, out AbortCondition conditionMet, out AbortBehavior behaviorNeeded)
		{
			conditionMet = AbortCondition.无;
			behaviorNeeded = AbortBehavior.无;
			foreach (PriceCheckCondition item in PriceCheckConditions.GetAll())
			{
				bool flag = false;
				foreach (KeyValuePair<AbortCondition, AbortBehavior> item2 in config.AbortLogic)
				{
					if ((item2.Key & item.Condition) == item.Condition)
					{
						behaviorNeeded = item2.Value;
						flag = true;
						break;
					}
				}
				if (flag && item.Predicate(config, origPrice, modifiedPrice, marketPrice))
				{
					conditionMet = item.Condition;
					return true;
				}
			}
			return false;
		}

		private static uint GetModifiedPrice(ItemConfig config, uint marketPrice)
		{
			return (uint)(config.AdjustBehavior switch
			{
				AdjustBehavior.固定值 => (ulong)Math.Max(0L, marketPrice - config.AdjustValues[AdjustBehavior.固定值]), 
				AdjustBehavior.百分比 => (ulong)Math.Max(0L, marketPrice * (1 - config.AdjustValues[AdjustBehavior.百分比] / 100)), 
				_ => marketPrice, 
			});
		}

		private unsafe void NotifyPriceAdjustSuccessfully(uint itemID, bool isHQ, uint origPrice, uint modifiedPrice)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			if (Module.config.SendPriceAdjustProcessMessage)
			{
				SeString val = new SeStringBuilder().AddItemLink(itemID, isHQ, (string)null).Build();
				long num = (long)modifiedPrice - (long)origPrice;
				string text = num.ToChineseString();
				if (!text.StartsWith('-'))
				{
					text = "+" + text;
				}
				string text2 = ((origPrice == 0) ? 0.0 : ((double)num / (double)origPrice * 100.0)).ToString("+0.##;-0.##") + "%";
				OmenServiceBase<NotifyHelper>.Instance().Chat(LanguageManager.GetSe("AutoRetainerWork-PriceAdjust-PriceAdjustSuccessfully", val, ((Retainer)((RetainerManager)RetainerManager.Instance()).GetActiveRetainer()).NameString, origPrice.ToChineseString(), modifiedPrice.ToChineseString(), text, text2));
			}
		}

		private unsafe void NotifyAbortCondition(uint itemID, bool isHQ, AbortCondition condition)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			if (Module.config.SendPriceAdjustProcessMessage)
			{
				SeString val = new SeStringBuilder().AddItemLink(itemID, isHQ, (string)null).Build();
				OmenServiceBase<NotifyHelper>.Instance().Chat(LanguageManager.GetSe("AutoRetainerWork-PriceAdjust-DetectAbortCondition", val, ((Retainer)((RetainerManager)RetainerManager.Instance()).GetActiveRetainer()).NameString, new SeStringBuilder().AddUiForeground(GetAbortConditionName(condition), (ushort)60).Build()));
			}
		}

		private unsafe static bool TryGetSameItemSlots(uint itemID, out List<ushort> slots)
		{
			slots = new List<ushort>();
			InventoryManager* ptr = InventoryManager.Instance();
			if (ptr == null)
			{
				return false;
			}
			InventoryContainer* inventoryContainer = ((InventoryManager)ptr).GetInventoryContainer((InventoryType)12002);
			if (inventoryContainer == null || !((InventoryContainer)inventoryContainer).IsLoaded)
			{
				return false;
			}
			for (int i = 0; i < ((InventoryContainer)inventoryContainer).Size; i++)
			{
				InventoryItem* inventorySlot = ((InventoryContainer)inventoryContainer).GetInventorySlot(i);
				if (inventorySlot != null && ((InventoryItem)inventorySlot).ItemId == itemID)
				{
					slots.Add((ushort)i);
				}
			}
			return slots.Count > 0;
		}
	}

	private class RefreshWorker : RetainerWorkerBase
	{
		private TaskHelper? taskHelper;

		public RefreshWorker(AutoRetainerWork module)
			: base(module)
		{
		}

		public override bool DrawConfigCondition()
		{
			return false;
		}

		public override bool IsWorkerBusy()
		{
			return taskHelper?.IsBusy ?? false;
		}

		public override void Init()
		{
			if (taskHelper == null)
			{
				taskHelper = new TaskHelper
				{
					TimeoutMS = 15000
				};
			}
		}

		public override void Uninit()
		{
			taskHelper?.Abort();
			taskHelper?.Dispose();
			taskHelper = null;
		}

		public override CollaspingCategoryNode CreateOverlayCategory(float width)
		{
			return RetainerWorkerBase.CreateOverlayCategory(LanguageManager.Get("AutoRetainerWork-Refresh-Title"), width, RetainerWorkerBase.CreateOverlayButtonRow(EnqueueRetainersRefresh, delegate
			{
				taskHelper?.Abort();
			}, width));
		}

		private void EnqueueRetainersRefresh()
		{
			if (Module.IsAnyOtherWorkerBusy(typeof(RefreshWorker)) || GetValidRetainerCount((Retainer _) => true, out List<uint> validRetainers) == 0)
			{
				return;
			}
			validRetainers.ForEach(delegate(uint index)
			{
				taskHelper.Enqueue(() => taskHelper.AbortByConflictKey(Module) || Module.EnterRetainer(index), $"选择进入 {index} 号雇员");
				taskHelper.Enqueue(() => taskHelper.AbortByConflictKey(Module) || LeaveRetainer(), "回到雇员列表");
			});
		}
	}

	private class TownDispatchWorker : RetainerWorkerBase
	{
		private TaskHelper? TaskHelper;

		public TownDispatchWorker(AutoRetainerWork module)
			: base(module)
		{
		}

		public override bool DrawConfigCondition()
		{
			return true;
		}

		public override bool IsWorkerBusy()
		{
			return TaskHelper?.IsBusy ?? false;
		}

		public override void Init()
		{
			if (TaskHelper == null)
			{
				TaskHelper = new TaskHelper
				{
					TimeoutMS = 15000
				};
			}
		}

		public override void Uninit()
		{
			TaskHelper?.Abort();
			TaskHelper?.Dispose();
			TaskHelper = null;
		}

		public override void DrawConfig()
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			ImGui.AlignTextToFramePadding();
			Vector4 vector = KnownColor.Chocolate.ToVector4();
			ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("AutoRetainerWork-Dispatch-Title")));
			IDalamudTextureWrap texture;
			bool flag = OmenServiceBase<ImageHelper>.Instance().TryGetImage("hxxps://gh.atmoomen.top/StaticAssets/main/DailyRoutines/image/AutoRetainersDispatch-1.png", out texture);
			ImGui.SameLine();
			ImGui.TextDisabled(ImU8String.op_Implicit(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61530)));
			if (ImGui.IsItemHovered())
			{
				TooltipDisposable val = ImRaii.Tooltip();
				try
				{
					ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("AutoRetainerWork-Dispatch-Description")));
					if (flag)
					{
						ImGui.Image(texture.Handle, texture.Size * 0.8f);
					}
				}
				finally
				{
					((TooltipDisposable)(ref val)).Dispose();
				}
			}
			IndentDisposable val2 = ImRaii.PushIndent(1, true);
			try
			{
				if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("Start")), default(Vector2)))
				{
					EnqueueRetainersDispatch();
				}
				ImGui.SameLine();
				if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("Stop")), default(Vector2)))
				{
					TaskHelper.Abort();
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}

		private unsafe void EnqueueRetainersDispatch()
		{
			if (TaskHelper.AbortByConflictKey(Module) || Module.IsAnyOtherWorkerBusy(typeof(TownDispatchWorker)))
			{
				return;
			}
			AddonSelectString* selectString = (AddonSelectString*)Addons.SelectString;
			if (selectString == null)
			{
				return;
			}
			int entryCount = ((PopupMenu)(&((PopupMenuDerive)(&((AddonSelectString)selectString).PopupMenu)).PopupMenu)).EntryCount;
			if (entryCount - 1 <= 0)
			{
				return;
			}
			for (int i = 0; i < entryCount - 1; i++)
			{
				int tempI = i;
				TaskHelper.Enqueue(() => TaskHelper.AbortByConflictKey(Module) || AddonSelectStringEvent.Select(tempI), $"点击第 {tempI} 位雇员, 拉起市场变更请求");
				TaskHelper.Enqueue(() => TaskHelper.AbortByConflictKey(Module) || AddonSelectYesnoEvent.ClickYes(), "确认市场变更");
			}
		}
	}

	private Config config;

	private readonly Throttler<string> retainerThrottler = new Throttler<string>();

	private readonly HashSet<ulong> playerRetainers = new HashSet<ulong>();

	private DRAutoRetainerWork? addon;

	private RetainerWorkerBase[] workers;

	private static readonly AbortCondition[] AbortConditions = Enum.GetValues<AbortCondition>();

	private static readonly FrozenDictionary<AdjustBehavior, string> AdjustBehaviorLoc = new Dictionary<AdjustBehavior, string>
	{
		[AdjustBehavior.固定值] = LanguageManager.Get("AutoRetainerWork-AdjustBehavior-FixedValue"),
		[AdjustBehavior.百分比] = LanguageManager.Get("AutoRetainerWork-AdjustBehavior-Percentage")
	}.ToFrozenDictionary();

	private static readonly FrozenDictionary<AbortCondition, string> AbortConditionLoc = new Dictionary<AbortCondition, string>
	{
		[AbortCondition.无] = LanguageManager.Get("None"),
		[AbortCondition.低于最小值] = LanguageManager.Get("AutoRetainerWork-AbortCondition-BelowMinimum"),
		[AbortCondition.低于预期值] = LanguageManager.Get("AutoRetainerWork-AbortCondition-BelowExpected"),
		[AbortCondition.低于收购价] = LanguageManager.Get("AutoRetainerWork-AbortCondition-BelowVendorPrice"),
		[AbortCondition.大于可接受降价值] = LanguageManager.Get("AutoRetainerWork-AbortCondition-ExceedsMaximumReduction"),
		[AbortCondition.高于预期值] = LanguageManager.Get("AutoRetainerWork-AbortCondition-AboveExpected"),
		[AbortCondition.高于最大值] = LanguageManager.Get("AutoRetainerWork-AbortCondition-AboveMaximum")
	}.ToFrozenDictionary();

	private static readonly FrozenDictionary<AbortBehavior, string> AbortBehaviorLoc = new Dictionary<AbortBehavior, string>
	{
		[AbortBehavior.无] = LanguageManager.Get("None"),
		[AbortBehavior.收回至雇员] = LanguageManager.Get("AutoRetainerWork-AbortBehavior-ReturnToRetainer"),
		[AbortBehavior.收回至背包] = LanguageManager.Get("AutoRetainerWork-AbortBehavior-ReturnToInventory"),
		[AbortBehavior.出售至系统商店] = LanguageManager.Get("AutoRetainerWork-AbortBehavior-SellToVendor"),
		[AbortBehavior.改价至最小值] = LanguageManager.Get("AutoRetainerWork-AbortBehavior-AdjustToMinimum"),
		[AbortBehavior.改价至预期值] = LanguageManager.Get("AutoRetainerWork-AbortBehavior-AdjustToExpected"),
		[AbortBehavior.改价至最高值] = LanguageManager.Get("AutoRetainerWork-AbortBehavior-AdjustToMaximum")
	}.ToFrozenDictionary();

	private static readonly FrozenDictionary<SortOrder, string> SortOrderLoc = new Dictionary<SortOrder, string>
	{
		[SortOrder.上架顺序] = LanguageManager.Get("AutoRetainerWork-SortOrder-Listing"),
		[SortOrder.物品ID] = LanguageManager.Get("AutoRetainerWork-SortOrder-ItemID"),
		[SortOrder.物品类型] = LanguageManager.Get("AutoRetainerWork-SortOrder-ItemType")
	}.ToFrozenDictionary();

	[IPCSubscriber("DailyRoutines.Modules.BetterMarketBoard.SearchItem")]
	private static IPCSubscriber<uint, bool> SearchItemIPC;

	[IPCSubscriber("DailyRoutines.Modules.BetterMarketBoard.ToggleOverlay")]
	private static IPCSubscriber<bool?, bool> ToggleOverlayIPC;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoRetainerWorkTitle"),
		Description = LanguageManager.Get("AutoRetainerWorkDescription"),
		Category = ModuleCategory.Interface,
		ModulesPrerequisite = new string[3] { "AutoTalkSkip", "AutoRefreshMarketSearchResult", "BetterMarketBoard" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		NeedAuth = true
	};

	protected override void Init()
	{
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		workers = new RetainerWorkerBase[7]
		{
			new CollectWorker(this),
			new EntrustDupsWorker(this),
			new GilsShareWorker(this),
			new GilsWithdrawWorker(this),
			new RefreshWorker(this),
			new TownDispatchWorker(this),
			new PriceAdjustWorker(this)
		};
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		RetainerWorkerBase[] array = workers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Init();
		}
		if (addon == null)
		{
			addon = new DRAutoRetainerWork(this)
			{
				InternalName = "DRAutoRetainerWork",
				Title = ReadOnlySeString.op_Implicit(Info.Title),
				Size = new Vector2(260f, 320f)
			};
		}
	}

	protected override void Uninit()
	{
		addon?.Dispose();
		addon = null;
		RetainerWorkerBase[] array = workers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Uninit();
		}
	}

	protected override void ConfigUI()
	{
		RetainerWorkerBase[] array = workers;
		foreach (RetainerWorkerBase retainerWorkerBase in array)
		{
			if (retainerWorkerBase.DrawConfigCondition())
			{
				retainerWorkerBase.DrawConfig();
				ImGui.NewLine();
			}
		}
	}

	private static string GetAbortConditionName(AbortCondition condition)
	{
		string valueOrDefault = AbortConditionLoc.GetValueOrDefault(condition);
		if (valueOrDefault != null)
		{
			return valueOrDefault;
		}
		StringBuilder stringBuilder = new StringBuilder();
		AbortCondition[] abortConditions = AbortConditions;
		foreach (AbortCondition abortCondition in abortConditions)
		{
			if (abortCondition != AbortCondition.无 && (condition & abortCondition) == abortCondition)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(" / ");
				}
				stringBuilder.Append(AbortConditionLoc.GetValueOrDefault(abortCondition));
			}
		}
		return stringBuilder.ToString();
	}

	private unsafe bool EnterRetainer(uint index)
	{
		if (!retainerThrottler.Throttle("EnterRetainer", 100u))
		{
			return false;
		}
		if (!(*Addons.RetainerList).IsAddonAndNodesReady())
		{
			return false;
		}
		(*Addons.RetainerList).Callback(2, (int)index, 0, 0);
		return true;
	}

	private unsafe static bool LeaveRetainer()
	{
		if ((*Addons.SelectYesno).IsAddonAndNodesReady())
		{
			(*Addons.SelectYesno).Callback(0);
			return false;
		}
		if ((*Addons.SelectString).IsAddonAndNodesReady())
		{
			(*Addons.SelectString).Callback(-1);
			return false;
		}
		return (*Addons.RetainerList).IsAddonAndNodesReady();
	}

	private unsafe static uint GetValidRetainerCount(Func<Retainer, bool> predicateFunc, out List<uint> validRetainers)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		validRetainers = new List<uint>();
		RetainerManager* ptr = RetainerManager.Instance();
		if (ptr == null)
		{
			return 0u;
		}
		uint num = 0u;
		for (uint num2 = 0u; num2 < ((RetainerManager)ptr).GetRetainerCount(); num2++)
		{
			Retainer* retainerBySortedIndex = ((RetainerManager)ptr).GetRetainerBySortedIndex(num2);
			if (retainerBySortedIndex != null && predicateFunc(*retainerBySortedIndex))
			{
				validRetainers.Add(num2);
				num++;
			}
		}
		return num;
	}

	private unsafe static bool ExitRetainerInventory()
	{
		AgentInterface* agentByInternalId = ((AgentModule)AgentModule.Instance()).GetAgentByInternalId((AgentId)70);
		AgentInterface* agentByInternalId2 = ((AgentModule)AgentModule.Instance()).GetAgentByInternalId((AgentId)6);
		if (agentByInternalId == null || agentByInternalId2 == null || !((AgentInterface)agentByInternalId).IsAgentActive())
		{
			return false;
		}
		AtkUnitBase* addonById = ((RaptureAtkUnitManager)RaptureAtkUnitManager.Instance()).GetAddonById((ushort)((AgentInterface)agentByInternalId).GetAddonId());
		AtkUnitBase* addonById2 = ((RaptureAtkUnitManager)RaptureAtkUnitManager.Instance()).GetAddonById((ushort)((AgentInterface)agentByInternalId2).GetAddonId());
		if (addonById != null)
		{
			((AtkUnitBase)addonById).Close(true);
		}
		if (addonById2 != null)
		{
			(*addonById2).Callback(-1);
		}
		((AgentId)70).SendEvent(0uL, -1);
		return true;
	}

	private unsafe static bool TrySearchItemInInventory(uint itemID, bool isHQ, out List<InventoryItem> foundItem)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		foundItem = new List<InventoryItem>();
		InventoryManager* ptr = InventoryManager.Instance();
		if (ptr == null)
		{
			return false;
		}
		foreach (InventoryType item in Inventories.Player)
		{
			InventoryContainer* inventoryContainer = ((InventoryManager)ptr).GetInventoryContainer(item);
			if (inventoryContainer == null)
			{
				return false;
			}
			for (int i = 0; i < ((InventoryContainer)inventoryContainer).Size; i++)
			{
				InventoryItem* inventorySlot = ((InventoryContainer)inventoryContainer).GetInventorySlot(i);
				if (inventorySlot != null && ((InventoryItem)inventorySlot).ItemId != 0 && ((InventoryItem)inventorySlot).ItemId == itemID && (!isHQ || (isHQ && ((Enum)((InventoryItem)inventorySlot).Flags).HasFlag((Enum)(object)(ItemFlags)1))))
				{
					foundItem.Add(*inventorySlot);
				}
			}
		}
		return foundItem.Count > 0;
	}

	private unsafe void ObtainPlayerRetainers()
	{
		RetainerManager* ptr = RetainerManager.Instance();
		if (ptr == null)
		{
			return;
		}
		for (uint num = 0u; num < ((RetainerManager)ptr).GetRetainerCount(); num++)
		{
			Retainer* retainerBySortedIndex = ((RetainerManager)ptr).GetRetainerBySortedIndex(num);
			if (retainerBySortedIndex != null)
			{
				playerRetainers.Add(((Retainer)retainerBySortedIndex).RetainerId);
				continue;
			}
			break;
		}
	}

	private bool IsAnyOtherWorkerBusy(Type current)
	{
		RetainerWorkerBase[] array = workers;
		foreach (RetainerWorkerBase retainerWorkerBase in array)
		{
			if (retainerWorkerBase.IsWorkerBusy() && !(current == retainerWorkerBase.GetType()))
			{
				return true;
			}
		}
		return false;
	}

	private bool IsAnyWorkerBusy()
	{
		RetainerWorkerBase[] array = workers;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].IsWorkerBusy())
			{
				return true;
			}
		}
		return false;
	}
}
