using System.Numerics;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.ModulesPublic.Interface;

public class FastBLUSpellbookSearchBar : ModuleBase
{
	private TextInputNode? searchBarNode;

	private string searchBarInput = string.Empty;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("FastBLUSpellbookSearchBarTitle"),
		Description = LanguageManager.Get("FastBLUSpellbookSearchBarDescription"),
		Category = ModuleCategory.Interface
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected override void Init()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		if (base.TaskHelper == null)
		{
			TaskHelper taskHelper = (base.TaskHelper = new TaskHelper());
		}
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)5, "AOZNotebook", new AddonEventDelegate(OnAddon));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, "AOZNotebook", new AddonEventDelegate(OnAddon));
	}

	protected override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddon });
		searchBarNode?.Dispose();
		searchBarNode = null;
	}

	private unsafe void OnAddon(AddonEvent type, AddonArgs args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		if ((int)type != 5)
		{
			if ((int)type == 6)
			{
				searchBarNode = null;
			}
		}
		else
		{
			if (Addons.AOZNotebook == null)
			{
				return;
			}
			if (searchBarNode == null)
			{
				ConductSearch(searchBarInput);
				AtkComponentNode* componentNodeById = ((AtkUnitBase)Addons.AOZNotebook).GetComponentNodeById(123u);
				if (componentNodeById == null)
				{
					return;
				}
				AtkResNode* ptr = ((AtkUldManager)(&((AtkComponentBase)((AtkComponentNode)componentNodeById).GetComponent()).UldManager)).SearchNodeById(3u);
				if (ptr != null)
				{
					((AtkResNode)ptr).ToggleVisibility(false);
				}
				AtkResNode* ptr2 = ((AtkUldManager)(&((AtkComponentBase)((AtkComponentNode)componentNodeById).GetComponent()).UldManager)).SearchNodeById(4u);
				if (ptr2 != null)
				{
					((AtkResNode)ptr2).ToggleVisibility(false);
				}
				searchBarNode = new TextInputNode
				{
					IsVisible = true,
					Position = new Vector2(40f, 35f),
					Size = new Vector2(200f, 35f),
					MaxCharacters = 20,
					ShowLimitText = true,
					OnInputReceived = delegate(ReadOnlySeString x)
					{
						searchBarInput = ((object)(*(ReadOnlySeString*)(&x))/*cast due to constrained. prefix*/).ToString();
						ConductSearch(searchBarInput);
					},
					OnInputComplete = delegate(ReadOnlySeString x)
					{
						searchBarInput = ((object)(*(ReadOnlySeString*)(&x))/*cast due to constrained. prefix*/).ToString();
						ConductSearch(searchBarInput);
					}
				};
				searchBarNode.CurrentTextNode.FontSize = 14u;
				searchBarNode.CurrentTextNode.Position += new Vector2(0f, 3f);
				searchBarNode.AttachNode(componentNodeById);
			}
			searchBarNode.IsVisible = ((AtkValue)((AtkUnitBase)Addons.AOZNotebook).AtkValues).Int < 9;
		}
	}

	private unsafe void ConductSearch(string input)
	{
		base.TaskHelper.Enqueue(delegate
		{
			AtkUnitBase* aOZNotebook = Addons.AOZNotebook;
			if (aOZNotebook == null)
			{
				base.TaskHelper.Abort();
				return true;
			}
			if (!(*aOZNotebook).IsAddonAndNodesReady())
			{
				return false;
			}
			if (((AtkValue)((AtkUnitBase)aOZNotebook).AtkValues).Int >= 9)
			{
				base.TaskHelper.Abort();
				return true;
			}
			((AgentId)325).SendEvent(2uL, 0, 0u, input);
			return true;
		});
	}
}
