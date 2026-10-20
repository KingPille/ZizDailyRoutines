using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Game.NativeWrapper;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using Lumina.Data;
using Lumina.Excel.Sheets;
using Newtonsoft.Json;
using OmenTools;
using OmenTools.Dalamud;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Info.DTOs.RisingStone;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Utils;

namespace DailyRoutines.ModulesPublic.Interface;

public class ExpandPlayerMenuSearch : ModuleBase
{
	private sealed class CharacterSearchInfo
	{
		public string Name { get; init; } = string.Empty;

		public string World { get; init; } = string.Empty;

		public uint WorldID { get; init; }
	}

	private sealed class Config : ModuleConfig
	{
		public Dictionary<string, bool> SearchMenuEnabledStates = new Dictionary<string, bool>();
	}

	private abstract class SearchMenuItemBase(ExpandPlayerMenuSearch module) : ContextMenuEntry
	{
		protected readonly ExpandPlayerMenuSearch module = module;

		public override string Identifier => "ExpandPlayerMenuSearch";

		public abstract string PlatformName { get; }

		public abstract string ConfigKey { get; }

		public virtual bool DefaultEnabled => false;

		protected CharacterSearchInfo? TargetChara => module.targetChara;

		protected static void NotifyPlayerNotFound()
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Expected O, but got Unknown
			string text = LanguageManager.Get("ExpandPlayerMenuSearch-Notification-PlayerInfoNotFound");
			NotifyHelper.ToastError(text);
			NotifyHelper.Chat(new XivChatEntry
			{
				Type = (XivChatType)60,
				Message = SeString.op_Implicit(text)
			});
		}

		protected void RunOnTick(Func<CharacterSearchInfo, Task> action)
		{
			CharacterSearchInfo targetChara = TargetChara;
			if (targetChara != null)
			{
				IFramework obj = DalamudServiceExtension.Instance<IFramework>();
				Func<Task> func = () => action(targetChara);
				CancellationToken token = module.cancelSource.Token;
				obj.RunOnTick(func, default(TimeSpan), 0, token);
			}
		}

		protected void RunOnTickImmediately(Func<CharacterSearchInfo, Task> action)
		{
			CharacterSearchInfo targetChara = TargetChara;
			if (targetChara != null)
			{
				DalamudServiceExtension.Instance<IFramework>().RunOnTick((Func<Task>)(() => action(targetChara)), TimeSpan.Zero, 0, module.cancelSource.Token);
			}
		}

		public abstract void OnClicked();

		public override ContextMenuItem Create(ContextMenuOpenedArgs args)
		{
			return null;
		}
	}

	private sealed class UpperContainerItem(ExpandPlayerMenuSearch module) : ContextMenuEntry
	{
		public override string Identifier => "ExpandPlayerMenuSearch";

		public override ContextMenuItem? Create(ContextMenuOpenedArgs args)
		{
			return null;
		}
	}

	private sealed class ClickAllItem(ExpandPlayerMenuSearch module) : ContextMenuEntry
	{
		public override string Identifier => "ExpandPlayerMenuSearch";

		public override int? Priority => 1000;

		public override ContextMenuItem Create(ContextMenuOpenedArgs args)
		{
			return null;
		}
	}

	private sealed class RisingStoneItem : SearchMenuItemBase
	{
		private const string SearchAPI = "hxxps://apiff14risingstones.web.sdo.com/api/common/search?type=6&keywords={0}&page={1}&limit=50";

		private const string PlayerInfoURL = "hxxps://ff14risingstones.web.sdo.com/pc/index.html#/me/info?uuid={0}";

		public override string PlatformName => "石之家";

		public override string ConfigKey => "RisingStoneItem";

		public override bool DefaultEnabled => GameState.IsCN;

		public RisingStoneItem(ExpandPlayerMenuSearch module)
			: base(module)
		{
		}

		public override void OnClicked()
		{
			RunOnTick(async delegate(CharacterSearchInfo targetChara)
			{
				int page = 1;
				bool isFound = false;
				while (!isFound)
				{
					string requestUri = $"https://apiff14risingstones.web.sdo.com/api/common/search?type=6&keywords={targetChara.Name}&page={page}&limit=50";
					RSPlayerSearchResult rSPlayerSearchResult = JsonConvert.DeserializeObject<RSPlayerSearchResult>(await OmenServiceBase<HTTPClientHelper>.Instance().Get().GetStringAsync(requestUri));
					if (rSPlayerSearchResult?.Data == null || rSPlayerSearchResult.Data.Count == 0)
					{
						SearchMenuItemBase.NotifyPlayerNotFound();
						break;
					}
					foreach (RSPlayerSearchResultData datum in rSPlayerSearchResult.Data)
					{
						if (!(datum.CharacterName != targetChara.Name) && !(datum.GroupName != targetChara.World))
						{
							Util.OpenLink($"https://ff14risingstones.web.sdo.com/pc/index.html#/me/info?uuid={datum.UUID}");
							isFound = true;
							break;
						}
					}
					if (isFound)
					{
						break;
					}
					await Task.Delay(1000, module.cancelSource.Token);
					page++;
				}
			});
		}
	}

	private sealed class TiebaItem : SearchMenuItemBase
	{
		private const string URL = "hxxps://tieba.baidu.com/f/search/res?ie=utf-8&kw=ff14&qw={0}";

		public override string PlatformName => "百度贴吧";

		public override string ConfigKey => "TiebaItem";

		public override bool DefaultEnabled => GameState.IsCN;

		public TiebaItem(ExpandPlayerMenuSearch module)
			: base(module)
		{
		}

		public override void OnClicked()
		{
			CharacterSearchInfo targetChara = base.TargetChara;
			if (targetChara != null)
			{
				Util.OpenLink(string.Format("https://tieba.baidu.com/f/search/res?ie=utf-8&kw=ff14&qw={0}", targetChara.Name + "@" + targetChara.World));
			}
		}
	}

	private sealed class FFLogsItem : SearchMenuItemBase
	{
		private const string URL = "hxxps://{3}.fflogs.com/character/{0}/{1}/{2}";

		public override string PlatformName => "FF Logs";

		public override string ConfigKey => "FFLogsItem";

		public override bool DefaultEnabled => true;

		public FFLogsItem(ExpandPlayerMenuSearch module)
			: base(module)
		{
		}

		public override void OnClicked()
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			CharacterSearchInfo targetChara = base.TargetChara;
			if (targetChara != null)
			{
				World rowOrDefault = LuminaGetter.GetRowOrDefault<World>(targetChara.WorldID);
				WorldDCGroupType value = ((World)(ref rowOrDefault)).DataCenter.Value;
				uint rowId = ((WorldDCGroupType)(ref value)).Region.RowId;
				InlineArray4<object> buffer = default(InlineArray4<object>);
				buffer[0] = RegionToFFLogsAbbvr(rowId);
				buffer[1] = targetChara.World;
				buffer[2] = targetChara.Name;
				buffer[3] = GetPrefix();
				Util.OpenLink(string.Format("https://{3}.fflogs.com/character/{0}/{1}/{2}", (ReadOnlySpan<object?>)buffer));
			}
		}

		private static string RegionToFFLogsAbbvr(uint region)
		{
			return region switch
			{
				1u => "JP", 
				2u => "NA", 
				3u => "EU", 
				4u => "OC", 
				5u => "CN", 
				6u => "KR", 
				_ => "CN", 
			};
		}

		private static string GetPrefix()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Expected I4, but got Unknown
			Language clientLanguage = GameState.ClientLanguage;
			return (clientLanguage - 1) switch
			{
				0 => "ja", 
				3 => "fr", 
				2 => "de", 
				6 => "ko", 
				4 => "cn", 
				5 => "cn", 
				_ => "www", 
			};
		}
	}

	private abstract class LodestoneSearchMenuItemBase : SearchMenuItemBase
	{
		protected LodestoneSearchMenuItemBase(ExpandPlayerMenuSearch module)
			: base(module)
		{
		}

		protected abstract string GetPlayerURL(string characterID, CharacterSearchInfo targetChara);

		public override void OnClicked()
		{
			RunOnTickImmediately(async delegate(CharacterSearchInfo targetChara)
			{
				CancellationToken cancellationToken = module.cancelSource.Token;
				try
				{
					string text = await module.lodestoneSearch.GetCharacterIDAsync(targetChara.Name, targetChara.World);
					cancellationToken.ThrowIfCancellationRequested();
					if (text == null)
					{
						SearchMenuItemBase.NotifyPlayerNotFound();
					}
					else
					{
						Util.OpenLink(GetPlayerURL(text, targetChara));
					}
				}
				catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
				{
				}
				catch (Exception ex2)
				{
					DLog.Error($"[{"ExpandPlayerMenuSearch"}] Lodestone: {targetChara.Name}@{targetChara.World}", ex2);
					NotifyHelper.ToastError("Lodestone " + LanguageManager.Get("Error") + ": " + ex2.Message);
				}
			});
		}
	}

	private sealed class LodestoneItem : LodestoneSearchMenuItemBase
	{
		public override string PlatformName => "Lodestone";

		public override string ConfigKey => "LodestoneItem";

		public override bool DefaultEnabled => GameState.IsGL;

		public LodestoneItem(ExpandPlayerMenuSearch module)
			: base(module)
		{
		}

		protected override string GetPlayerURL(string characterID, CharacterSearchInfo targetChara)
		{
			return $"https://{GetPrefix()}.finalfantasyxiv.com/lodestone/character/{characterID}/";
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

	private sealed class LalachievementsItem : LodestoneSearchMenuItemBase
	{
		public override string PlatformName => "Lalachievements";

		public override string ConfigKey => "LalachievementsItem";

		public override bool DefaultEnabled => GameState.IsGL;

		public LalachievementsItem(ExpandPlayerMenuSearch module)
			: base(module)
		{
		}

		protected override string GetPlayerURL(string characterID, CharacterSearchInfo targetChara)
		{
			return $"https://www.lalachievements.com{GetVariant()}/char/{characterID}/";
		}

		private static string GetVariant()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Expected I4, but got Unknown
			Language clientLanguage = GameState.ClientLanguage;
			return (clientLanguage - 1) switch
			{
				0 => "/ja", 
				3 => "/fr", 
				2 => "/de", 
				_ => string.Empty, 
			};
		}
	}

	private sealed class TomestoneItem : LodestoneSearchMenuItemBase
	{
		public override string PlatformName => "Tomestone";

		public override string ConfigKey => "TomestoneItem";

		public override bool DefaultEnabled => GameState.IsGL;

		public TomestoneItem(ExpandPlayerMenuSearch module)
			: base(module)
		{
		}

		protected override string GetPlayerURL(string characterID, CharacterSearchInfo targetChara)
		{
			return "https://tomestone.gg/character/" + characterID + "/" + Uri.EscapeDataString(targetChara.Name.ToLowerInvariant().Replace(' ', '-'));
		}
	}

	[CompilerGenerated]
	private SearchMenuItemBase[] _003CSearchMenuItems_003Ek__BackingField;

	private Config config;

	private readonly CancellationTokenSource cancelSource = new CancellationTokenSource();

	private CharacterSearchInfo? targetChara;

	private LodestoneSearcher lodestoneSearch;

	private UpperContainerItem menu;

	private ClickAllItem clickAllMenu;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("ExpandPlayerMenuSearchTitle"),
		Description = LanguageManager.Get("ExpandPlayerMenuSearchDescription"),
		Category = ModuleCategory.Interface
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
			return _003CSearchMenuItems_003Ek__BackingField = (from type in typeof(ExpandPlayerMenuSearch).GetNestedTypes(BindingFlags.NonPublic)
				where !type.IsAbstract && typeof(SearchMenuItemBase).IsAssignableFrom(type)
				select (SearchMenuItemBase)Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new object[1] { this }, null)).ToArray();
		}
	}

	protected override void Init()
	{
		menu = new UpperContainerItem(this);
		clickAllMenu = new ClickAllItem(this);
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		lodestoneSearch = new LodestoneSearcher(OmenServiceBase<HTTPClientHelper>.Instance().Get(), cancelSource.Token);
		OmenServiceBase<ContextMenuManager>.Instance().Reg(menu);
	}

	protected override void Uninit()
	{
		OmenServiceBase<ContextMenuManager>.Instance().Unreg(menu);
		cancelSource.Cancel();
		cancelSource.Dispose();
		targetChara = null;
	}

	protected override void ConfigUI()
	{
	}

	private unsafe static bool TryResolveTargetChara(ContextMenuOpenedArgs args, out CharacterSearchInfo? resolvedTarget)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		resolvedTarget = null;
		if (args.InventoryAgentContext != null)
		{
			return false;
		}
		AgentInterfacePtr val = DalamudServiceExtension.Instance<IGameGui>().FindAgentInterface("ChatLog");
		if (val != AgentInterfacePtr.op_Implicit((IntPtr)IntPtr.Zero) && *(uint*)((long)(nint)AgentInterfacePtr.op_Implicit(val) + 2376L + 8) == 3)
		{
			return false;
		}
		bool hasTargetCharacter = args.TargetCharacter != null;
		bool flag = !string.IsNullOrWhiteSpace(args.TargetName) && args.TargetHomeWorldID > 0;
		bool hasTargetObjectCharacter = args.TargetObjectID != 0L && IOmenDalamudService<IObjectTable>.Instance().SearchByID(args.TargetObjectID) is ICharacter && flag;
		string addonName = args.AddonName;
		World val2;
		if (!(addonName == "BlackList"))
		{
			if (!(addonName == "FreeCompany"))
			{
				if (!(addonName == "LinkShell") && !(addonName == "CrossWorldLinkshell"))
				{
					switch (addonName)
					{
					default:
						return false;
					case "LookingForGroup":
					case "PartyMemberList":
					case "FriendList":
					case "SocialList":
					case "_PartyList":
					case "ChatLog":
					case "ContactList":
					case "BeginnerChatList":
					case "ContentMemberList":
					case null:
						return TryResolveGeneralTarget(args, hasTargetCharacter, hasTargetObjectCharacter, flag, out resolvedTarget);
					}
				}
				if (args.TargetContentID != 0L)
				{
					return TryResolveGeneralTarget(args, hasTargetCharacter, hasTargetObjectCharacter, flag, out resolvedTarget);
				}
				return false;
			}
			if (args.TargetContentID == 0L)
			{
				return false;
			}
			CharacterSearchInfo obj = new CharacterSearchInfo
			{
				Name = (args.TargetName ?? string.Empty)
			};
			World? row = LuminaGetter.GetRow<World>((uint)args.TargetHomeWorldID);
			object obj2;
			if (!row.HasValue)
			{
				obj2 = null;
			}
			else
			{
				val2 = row.GetValueOrDefault();
				obj2 = ((object)((World)(ref val2)).Name/*cast due to constrained. prefix*/).ToString();
			}
			if (obj2 == null)
			{
				obj2 = string.Empty;
			}
			obj.World = (string)obj2;
			obj.WorldID = (uint)args.TargetHomeWorldID;
			resolvedTarget = obj;
			return true;
		}
		AgentBlacklist* ptr = AgentBlacklist.Instance();
		if (ptr == (AgentBlacklist*)IntPtr.Zero || !((AgentInterface)(&((AgentBlacklist)ptr).AgentInterface)).IsAgentActive())
		{
			return false;
		}
		string text = ((object)(*(Utf8String*)(&((AgentBlacklist)ptr).SelectedPlayerName))/*cast due to constrained. prefix*/).ToString();
		string text2 = ((object)(*(Utf8String*)(&((AgentBlacklist)ptr).SelectedPlayerFullName))/*cast due to constrained. prefix*/).ToString();
		string serverName = (text2.StartsWith(text, StringComparison.Ordinal) ? text2.Substring(text.Length) : string.Empty);
		CharacterSearchInfo obj3 = new CharacterSearchInfo
		{
			Name = text,
			World = serverName
		};
		val2 = ((IEnumerable<World>)LuminaGetter.Get<World>()).FirstOrDefault((World world) => ((object)((World)(ref world)).Name/*cast due to constrained. prefix*/).ToString().Contains(serverName, StringComparison.OrdinalIgnoreCase));
		obj3.WorldID = ((World)(ref val2)).RowId;
		resolvedTarget = obj3;
		return true;
	}

	private unsafe static bool TryResolveGeneralTarget(ContextMenuOpenedArgs args, bool hasTargetCharacter, bool hasTargetObjectCharacter, bool hasTargetNameAndWorld, out CharacterSearchInfo? resolvedTarget)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		resolvedTarget = null;
		World valueOrDefault;
		if (hasTargetCharacter)
		{
			CharacterData* targetCharacter = args.TargetCharacter;
			CharacterSearchInfo obj = new CharacterSearchInfo
			{
				Name = ((CharacterData)targetCharacter).NameString
			};
			World? row = LuminaGetter.GetRow<World>((uint)((CharacterData)targetCharacter).HomeWorld);
			object obj2;
			if (!row.HasValue)
			{
				obj2 = null;
			}
			else
			{
				valueOrDefault = row.GetValueOrDefault();
				obj2 = ((object)((World)(ref valueOrDefault)).Name/*cast due to constrained. prefix*/).ToString();
			}
			if (obj2 == null)
			{
				obj2 = string.Empty;
			}
			obj.World = (string)obj2;
			obj.WorldID = ((CharacterData)targetCharacter).HomeWorld;
			resolvedTarget = obj;
		}
		else
		{
			ICharacter character = IOmenDalamudService<IObjectTable>.Instance().SearchByID(args.TargetObjectID) as ICharacter;
			if (character != null && hasTargetNameAndWorld)
			{
				CharacterSearchInfo obj3 = new CharacterSearchInfo
				{
					Name = character.Name
				};
				World? row = LuminaGetter.GetRow<World>((uint)((Character)character.Address).HomeWorld);
				object obj4;
				if (!row.HasValue)
				{
					obj4 = null;
				}
				else
				{
					valueOrDefault = row.GetValueOrDefault();
					obj4 = ((object)((World)(ref valueOrDefault)).Name/*cast due to constrained. prefix*/).ToString();
				}
				if (obj4 == null)
				{
					obj4 = string.Empty;
				}
				obj3.World = (string)obj4;
				obj3.WorldID = ((Character)character.Address).HomeWorld;
				resolvedTarget = obj3;
			}
			else if (hasTargetNameAndWorld)
			{
				CharacterSearchInfo obj5 = new CharacterSearchInfo
				{
					Name = (args.TargetName ?? string.Empty)
				};
				World? row = LuminaGetter.GetRow<World>((uint)args.TargetHomeWorldID);
				object obj6;
				if (!row.HasValue)
				{
					obj6 = null;
				}
				else
				{
					valueOrDefault = row.GetValueOrDefault();
					obj6 = ((object)((World)(ref valueOrDefault)).Name/*cast due to constrained. prefix*/).ToString();
				}
				if (obj6 == null)
				{
					obj6 = string.Empty;
				}
				obj5.World = (string)obj6;
				obj5.WorldID = (uint)args.TargetHomeWorldID;
				resolvedTarget = obj5;
			}
		}
		return hasTargetCharacter || hasTargetObjectCharacter || hasTargetNameAndWorld;
	}
}
