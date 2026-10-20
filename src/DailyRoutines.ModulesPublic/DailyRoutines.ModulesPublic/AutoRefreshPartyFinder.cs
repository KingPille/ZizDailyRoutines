using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Timers;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.LocalLifecycle;
using DailyRoutines.Manager;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.Classes;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;

namespace DailyRoutines.ModulesPublic;

public class AutoRefreshPartyFinder : ModuleBase
{
	private class Config : ModuleConfig
	{
		public bool OnlyInactive = true;

		public int RefreshInterval = 10;
	}

	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static Func<bool> _003C_003E9__14_0;

		internal unsafe bool _003COnRefreshTimer_003Eb__14_0()
		{
			return ((AgentLookingForGroup)AgentLookingForGroup.Instance()).RequestListingsUpdate();
		}
	}

	private Config config;

	private Timer? refreshTimer;

	private int cooldown;

	private NumericInputNode? refreshIntervalNode;

	private CheckboxNode? onlyInactiveNode;

	private TextNode? leftTimeNode;

	private HorizontalListNode? layoutNode;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoRefreshPartyFinderTitle"),
		Description = LanguageManager.Get("AutoRefreshPartyFinderDescription"),
		Category = ModuleCategory.Recruitment
	};

	protected unsafe override void Init()
	{
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Expected O, but got Unknown
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Expected O, but got Unknown
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Expected O, but got Unknown
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Expected O, but got Unknown
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Expected O, but got Unknown
		TimerLifetime.Begin(this);
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		if (refreshTimer == null)
		{
			refreshTimer = new Timer(1000.0);
		}
		refreshTimer.AutoReset = true;
		refreshTimer.Elapsed += OnRefreshTimer;
		cooldown = config.RefreshInterval;
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)1, "LookingForGroup", new AddonEventDelegate(OnAddonPF));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)10, "LookingForGroup", new AddonEventDelegate(OnAddonPF));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, "LookingForGroup", new AddonEventDelegate(OnAddonPF));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)1, "LookingForGroupDetail", new AddonEventDelegate(OnAddonLFGD));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, "LookingForGroupDetail", new AddonEventDelegate(OnAddonLFGD));
		if (Addons.LookingForGroup != null)
		{
			OnAddonPF((AddonEvent)1, null);
		}
	}

	protected override void Uninit()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Expected O, but got Unknown
		TimerLifetime.End(this);
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddonPF });
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddonLFGD });
		if (refreshTimer != null)
		{
			refreshTimer.Elapsed -= OnRefreshTimer;
			refreshTimer.Stop();
			refreshTimer.Dispose();
		}
		refreshTimer = null;
		CleanNodes();
	}

	private void OnAddonPF(AddonEvent type, AddonArgs? args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Invalid comparison between Unknown and I4
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		if ((int)type != 1)
		{
			if ((int)type != 6)
			{
				if ((int)type == 10 && config.OnlyInactive)
				{
					cooldown = config.RefreshInterval;
					UpdateNextRefreshTime(cooldown);
					refreshTimer.Restart();
				}
			}
			else
			{
				refreshTimer.Stop();
				CleanNodes();
			}
		}
		else
		{
			cooldown = config.RefreshInterval;
			CreateRefreshIntervalNode();
			refreshTimer.Restart();
		}
	}

	private void OnAddonLFGD(AddonEvent type, AddonArgs? args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		if ((int)type != 1)
		{
			if ((int)type == 6)
			{
				cooldown = config.RefreshInterval;
				refreshTimer.Restart();
			}
		}
		else
		{
			refreshTimer.Stop();
		}
	}

	private unsafe void OnRefreshTimer(object? sender, ElapsedEventArgs e)
	{
		if (TimerLifetime.Receive((object)this, sender, (object?)refreshTimer, (ElapsedEventHandler)OnRefreshTimer, e, (Func<Action, Task>)DalamudServiceExtension.Instance<IFramework>().RunOnFrameworkThread))
		{
			if (!(*Addons.LookingForGroup).IsAddonAndNodesReady() || (*Addons.LookingForGroupDetail).IsAddonAndNodesReady())
			{
				refreshTimer.Stop();
			}
			else if (cooldown > 1)
			{
				cooldown--;
				UpdateNextRefreshTime(cooldown);
			}
			else
			{
				cooldown = config.RefreshInterval;
				UpdateNextRefreshTime(cooldown);
				_003C_003Ec._003C_003E9._003COnRefreshTimer_003Eb__14_0();
			}
		}
	}

	private void CleanNodes()
	{
		TimerLifetime.Suspend(this);
		refreshIntervalNode?.Dispose();
		refreshIntervalNode = null;
		onlyInactiveNode?.Dispose();
		onlyInactiveNode = null;
		layoutNode?.Dispose();
		layoutNode = null;
		leftTimeNode?.Dispose();
		leftTimeNode = null;
	}

	private unsafe void CreateRefreshIntervalNode()
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		TimerLifetime.Resume(this);
		if (Addons.LookingForGroup == null)
		{
			return;
		}
		if (onlyInactiveNode == null)
		{
			onlyInactiveNode = new CheckboxNode
			{
				Size = new Vector2(150f, 28f),
				IsChecked = config.OnlyInactive,
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoRefreshPartyFinder-OnlyInactive")),
				OnClick = delegate(bool newState)
				{
					config.OnlyInactive = newState;
					config.Save(ManagerBase<ModuleManager>.Instance().GetModule<AutoRefreshPartyFinder>());
				},
				Position = new Vector2(0f, 1f)
			};
		}
		if (refreshIntervalNode == null)
		{
			refreshIntervalNode = new NumericInputNode
			{
				Size = new Vector2(150f, 30f),
				Position = new Vector2(0f, 2f),
				Min = 5,
				Max = 10000,
				Step = 5,
				OnValueUpdate = delegate(int newValue)
				{
					config.RefreshInterval = newValue;
					config.Save(ManagerBase<ModuleManager>.Instance().GetModule<AutoRefreshPartyFinder>());
					cooldown = config.RefreshInterval;
					refreshTimer.Restart();
				},
				Value = config.RefreshInterval
			};
		}
		refreshIntervalNode.Value = config.RefreshInterval;
		refreshIntervalNode.ValueTextNode.SetNumber(config.RefreshInterval);
		if (leftTimeNode == null)
		{
			leftTimeNode = new TextNode
			{
				String = ReadOnlySeString.op_Implicit($"({config.RefreshInterval})  "),
				FontSize = 12u,
				Size = new Vector2(0f, 28f),
				AlignmentType = (AlignmentType)5,
				Position = new Vector2(10f, 2f),
				TextColor = ColorHelper.GetColor(8u),
				TextOutlineColor = ColorHelper.GetColor(7u)
			};
		}
		layoutNode = new HorizontalListNode
		{
			Width = 270f,
			Position = new Vector2(770f, 630f),
			Alignment = HorizontalListAnchor.Right
		};
		layoutNode.AddNode(new global::_003C_003Ez__ReadOnlyArray<NodeBase>(new NodeBase[3] { onlyInactiveNode, refreshIntervalNode, leftTimeNode }));
		layoutNode.AttachNode(((AtkUnitBase)Addons.LookingForGroup).RootNode);
	}

	private void UpdateNextRefreshTime(int leftTime)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (leftTimeNode != null)
		{
			leftTimeNode.String = ReadOnlySeString.op_Implicit($"({leftTime})  ");
		}
	}
}
