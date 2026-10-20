using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Lumina.Data;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic.Interface;

public class ExpandItemMenuSearch : ModuleBase
{
	private sealed class Config : ModuleConfig
	{
		public bool GlamourPrioritize = true;

		public Dictionary<string, bool> SearchMenuEnabledStates = new Dictionary<string, bool>();
	}

	private sealed class UpperContainerItem(ExpandItemMenuSearch module) : ContextMenuEntry
	{
		public override string Identifier => "ExpandItemMenuSearch";

		public override ContextMenuItem? Create(ContextMenuOpenedArgs args)
		{
			return null;
		}
	}

	private sealed class ClickAllItem(ExpandItemMenuSearch module) : ContextMenuEntry
	{
		public override string Identifier => "ExpandItemMenuSearch";

		public override int? Priority => 1000;

		public override ContextMenuItem Create(ContextMenuOpenedArgs args)
		{
			return null;
		}
	}

	private abstract class SearchMenuItemBase(ExpandItemMenuSearch module) : ContextMenuEntry
	{
		protected readonly ExpandItemMenuSearch module = module;

		public override string Identifier => "ExpandItemMenuSearch";

		public abstract string PlatformName { get; }

		public abstract string ConfigKey { get; }

		public virtual bool DefaultEnabled => false;

		protected Item GetSearchItem(ContextMenuOpenedArgs args)
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			if (module.config.GlamourPrioritize)
			{
				RowRef<Item> targetGlamourRow = args.TargetGlamourRow;
				if (targetGlamourRow.RowId != 0 && targetGlamourRow.IsValid)
				{
					return targetGlamourRow.Value;
				}
			}
			return args.TargetItemRow.Value;
		}

		public abstract void OnClicked(ContextMenuOpenedArgs args);

		public abstract bool IsDisplay(ContextMenuOpenedArgs args);

		public override ContextMenuItem Create(ContextMenuOpenedArgs args)
		{
			return null;
		}
	}

	private sealed class RisingStonesItem : SearchMenuItemBase
	{
		private const string URL = "hxxps://ff14risingstones.web.sdo.com/pc/index.html#/search?equipmentid={0}&section=glamour";

		public override string PlatformName => "石之家";

		public override string ConfigKey => "RisingStonesItem";

		public override bool DefaultEnabled => GameState.IsCN;

		public RisingStonesItem(ExpandItemMenuSearch module)
			: base(module)
		{
		}

		public override void OnClicked(ContextMenuOpenedArgs args)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			Item searchItem = GetSearchItem(args);
			_ = $"hxxps://ff14risingstones.web.sdo.com/pc/index.html#/search?equipmentid={((Item)(ref searchItem)).RowId}&section=glamour";
		}

		public override bool IsDisplay(ContextMenuOpenedArgs args)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			Item searchItem = GetSearchItem(args);
			return ((Item)(ref searchItem)).EquipSlotCategory.RowId != 0;
		}
	}

	private sealed class HuijiWikiItem : SearchMenuItemBase
	{
		private const string URL = "hxxps://ff14.huijiwiki.com/wiki/%E7%89%A9%E5%93%81:{0}";

		public override string PlatformName => "最终幻想 XIV 中文维基";

		public override string ConfigKey => "HuijiWikiItem";

		public override bool DefaultEnabled
		{
			get
			{
				if (!GameState.IsCN)
				{
					return GameState.IsTC;
				}
				return true;
			}
		}

		public HuijiWikiItem(ExpandItemMenuSearch module)
			: base(module)
		{
		}

		public override void OnClicked(ContextMenuOpenedArgs args)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			Item searchItem = GetSearchItem(args);
			_ = $"hxxps://ff14.huijiwiki.com/wiki/%E7%89%A9%E5%93%81:{((Item)(ref searchItem)).Name}";
		}

		public override bool IsDisplay(ContextMenuOpenedArgs args)
		{
			return true;
		}
	}

	private sealed class UniversalisItem : SearchMenuItemBase
	{
		private const string URL = "hxxps://universalis.app/market/{0}";

		public override string PlatformName => "Universalis";

		public override string ConfigKey => "UniversalisItem";

		public override bool DefaultEnabled => true;

		public UniversalisItem(ExpandItemMenuSearch module)
			: base(module)
		{
		}

		public override void OnClicked(ContextMenuOpenedArgs args)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			Item searchItem = GetSearchItem(args);
			_ = $"hxxps://universalis.app/market/{((Item)(ref searchItem)).RowId}";
		}

		public override bool IsDisplay(ContextMenuOpenedArgs args)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			Item searchItem = GetSearchItem(args);
			return ((Item)(ref searchItem)).ItemSearchCategory.RowId != 0;
		}
	}

	private sealed class TeamCraftItem : SearchMenuItemBase
	{
		private const string URL = "hxxps://ffxivteamcraft.com/db/{0}/item/{1}";

		public override string PlatformName => "TeamCraft";

		public override string ConfigKey => "TeamCraftItem";

		public override bool DefaultEnabled => true;

		public TeamCraftItem(ExpandItemMenuSearch module)
			: base(module)
		{
		}

		public override void OnClicked(ContextMenuOpenedArgs args)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			string variant = GetVariant();
			Item searchItem = GetSearchItem(args);
			_ = $"hxxps://ffxivteamcraft.com/db/{variant}/item/{((Item)(ref searchItem)).RowId}";
		}

		public override bool IsDisplay(ContextMenuOpenedArgs args)
		{
			return true;
		}

		private static string GetVariant()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Expected I4, but got Unknown
			Language clientLanguage = GameState.ClientLanguage;
			return (clientLanguage - 1) switch
			{
				0 => "ja", 
				1 => "en", 
				3 => "fr", 
				2 => "de", 
				4 => "zh", 
				5 => "zh", 
				7 => "tw", 
				6 => "ko", 
				_ => "en", 
			};
		}
	}

	private sealed class ConsoleGameWikiItem : SearchMenuItemBase
	{
		private const string URL = "hxxps://ffxiv.consolegameswiki.com/mediawiki/index.php?search={0}&title=Special%3ASearch&go=%E5%89%8D%E5%BE%80";

		public override string PlatformName => "Console Games Wiki";

		public override string ConfigKey => "ConsoleGameWikiItem";

		public override bool DefaultEnabled => GameState.IsGL;

		public ConsoleGameWikiItem(ExpandItemMenuSearch module)
			: base(module)
		{
		}

		public override void OnClicked(ContextMenuOpenedArgs args)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			Item searchItem = GetSearchItem(args);
			_ = $"hxxps://ffxiv.consolegameswiki.com/mediawiki/index.php?search={Uri.EscapeDataString(((object)((Item)(ref searchItem)).Name/*cast due to constrained. prefix*/).ToString())}&title=Special%3ASearch&go=%E5%89%8D%E5%BE%80";
		}

		public override bool IsDisplay(ContextMenuOpenedArgs args)
		{
			return true;
		}
	}

	private sealed class GarlandToolsCNItem : SearchMenuItemBase
	{
		private const string URL = "hxxps://www.garlandtools.cn/db/#item/{0}";

		public override string PlatformName => "Garland Tools 国服站";

		public override string ConfigKey => "GarlandToolsCNItem";

		public override bool DefaultEnabled
		{
			get
			{
				if (!GameState.IsCN)
				{
					return GameState.IsTC;
				}
				return true;
			}
		}

		public GarlandToolsCNItem(ExpandItemMenuSearch module)
			: base(module)
		{
		}

		public override void OnClicked(ContextMenuOpenedArgs args)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			Item searchItem = GetSearchItem(args);
			_ = $"hxxps://www.garlandtools.cn/db/#item/{((Item)(ref searchItem)).RowId}";
		}

		public override bool IsDisplay(ContextMenuOpenedArgs args)
		{
			return true;
		}
	}

	private sealed class GarlandToolsItem : SearchMenuItemBase
	{
		private const string URL = "hxxps://www.garlandtools.org/db/#item/{0}";

		public override string PlatformName => "Garland Tools";

		public override string ConfigKey => "GarlandToolsItem";

		public override bool DefaultEnabled => GameState.IsGL;

		public GarlandToolsItem(ExpandItemMenuSearch module)
			: base(module)
		{
		}

		public override void OnClicked(ContextMenuOpenedArgs args)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			Item searchItem = GetSearchItem(args);
			_ = $"hxxps://www.garlandtools.org/db/#item/{((Item)(ref searchItem)).RowId}";
		}

		public override bool IsDisplay(ContextMenuOpenedArgs args)
		{
			return true;
		}
	}

	private sealed class LodestoneItem : SearchMenuItemBase
	{
		private const string URL = "hxxps://{1}.finalfantasyxiv.com/lodestone/playguide/db//search/?patch=&db_search_category=&q={0}";

		public override string PlatformName => "Lodestone";

		public override string ConfigKey => "LodestoneItem";

		public override bool DefaultEnabled => GameState.IsGL;

		public LodestoneItem(ExpandItemMenuSearch module)
			: base(module)
		{
		}

		public override void OnClicked(ContextMenuOpenedArgs args)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			Item searchItem = GetSearchItem(args);
			string.Format("hxxps://{1}.finalfantasyxiv.com/lodestone/playguide/db//search/?patch=&db_search_category=&q={0}", ((Item)(ref searchItem)).Name, GetPrefix());
		}

		public override bool IsDisplay(ContextMenuOpenedArgs args)
		{
			return true;
		}

		private static string GetPrefix()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Expected I4, but got Unknown
			Language clientLanguage = GameState.ClientLanguage;
			return (clientLanguage - 1) switch
			{
				0 => "jp", 
				3 => "fr", 
				2 => "de", 
				_ => "na", 
			};
		}
	}

	private sealed class GamerEscapeItem : SearchMenuItemBase
	{
		private const string URL = "hxxps://ffxiv.gamerescape.com/?search={0}";

		public override string PlatformName => "Gamer Escape";

		public override string ConfigKey => "GamerEscapeItem";

		public override bool DefaultEnabled => GameState.IsGL;

		public GamerEscapeItem(ExpandItemMenuSearch module)
			: base(module)
		{
		}

		public override void OnClicked(ContextMenuOpenedArgs args)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			Item searchItem = GetSearchItem(args);
			_ = $"hxxps://ffxiv.gamerescape.com/?search={Uri.EscapeDataString(((object)((Item)(ref searchItem)).Name/*cast due to constrained. prefix*/).ToString())}";
		}

		public override bool IsDisplay(ContextMenuOpenedArgs args)
		{
			return true;
		}
	}

	private sealed class ERIONESItem : SearchMenuItemBase
	{
		private const string URL = "hxxps://{0}eriones.com/search?i={1}";

		public override string PlatformName => "ERIONES";

		public override string ConfigKey => "ERIONESItem";

		public override bool DefaultEnabled => GameState.IsGL;

		public ERIONESItem(ExpandItemMenuSearch module)
			: base(module)
		{
		}

		public override void OnClicked(ContextMenuOpenedArgs args)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			Item searchItem = GetSearchItem(args);
			string text = ((object)((Item)(ref searchItem)).Name/*cast due to constrained. prefix*/).ToString();
			if (!string.IsNullOrWhiteSpace(text))
			{
				if (text.Length > 25)
				{
					text = text.Substring(0, 25);
				}
				_ = $"hxxps://{GetPrefixByLang()}eriones.com/search?i={Uri.EscapeDataString(text)}";
			}
		}

		public override bool IsDisplay(ContextMenuOpenedArgs args)
		{
			return true;
		}

		private static string GetPrefixByLang()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Expected I4, but got Unknown
			Language clientLanguage = GameState.ClientLanguage;
			return (clientLanguage - 2) switch
			{
				0 => "en.", 
				2 => "fr.", 
				1 => "de.", 
				3 => "cn.", 
				4 => "cn.", 
				6 => "cn.", 
				5 => "ko.", 
				_ => string.Empty, 
			};
		}
	}

	private sealed class FFXIVItemSearchTCItem : SearchMenuItemBase
	{
		private const string URL = "hxxps://cycleapple.github.io/ffxiv-item-search-tc?selected={0}&q={1}";

		public override string PlatformName => "FFXIV 繁中物品搜尋站";

		public override string ConfigKey => "FFXIVItemSearchTCItem";

		public override bool DefaultEnabled => GameState.IsTC;

		public FFXIVItemSearchTCItem(ExpandItemMenuSearch module)
			: base(module)
		{
		}

		public override void OnClicked(ContextMenuOpenedArgs args)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			Item searchItem = GetSearchItem(args);
			_ = $"hxxps://cycleapple.github.io/ffxiv-item-search-tc?selected={((Item)(ref searchItem)).RowId}&q={Uri.EscapeDataString(((object)((Item)(ref searchItem)).Name/*cast due to constrained. prefix*/).ToString())}";
		}

		public override bool IsDisplay(ContextMenuOpenedArgs args)
		{
			return true;
		}
	}

	[CompilerGenerated]
	private SearchMenuItemBase[] _003CSearchMenuItems_003Ek__BackingField;

	private Config config;

	private readonly UpperContainerItem menu;

	private readonly ClickAllItem clickAllMenu;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("ExpandItemMenuSearchTitle"),
		Description = LanguageManager.Get("ExpandItemMenuSearchDescription"),
		Category = ModuleCategory.Interface,
		Author = new string[1] { "HSS" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	private SearchMenuItemBase[] SearchMenuItems
	{
		get
		{
			SearchMenuItemBase[] array = _003CSearchMenuItems_003Ek__BackingField;
			if (array != null && array.Length > 0)
			{
				return _003CSearchMenuItems_003Ek__BackingField;
			}
			return _003CSearchMenuItems_003Ek__BackingField = (from type in typeof(ExpandItemMenuSearch).GetNestedTypes(BindingFlags.NonPublic)
				where !type.IsAbstract && typeof(SearchMenuItemBase).IsAssignableFrom(type)
				select (SearchMenuItemBase)Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new object[1] { this }, null)).ToArray();
		}
	}

	public ExpandItemMenuSearch()
	{
		menu = new UpperContainerItem(this);
		clickAllMenu = new ClickAllItem(this);
	}

	protected override void Init()
	{
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		OmenServiceBase<ContextMenuManager>.Instance().Reg(menu);
	}

	protected override void Uninit()
	{
		OmenServiceBase<ContextMenuManager>.Instance().Unreg(menu);
	}

	protected override void ConfigUI()
	{
	}
}
