using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Interface.Textures;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.Interop;
using FFXIVClientStructs.STD;
using FFXIVClientStructs.STD.Helper;
using Lumina.Text.ReadOnly;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.OmenService.Abstractions;
using OmenTools.OmenService.ZoneIndicator;

namespace DailyRoutines.ModulesPublic.Interface;

public class AutoDisplayMTQObject : ModuleBase
{
	private static readonly List<IGameObject> QuestObjects = new List<IGameObject>();

	private ZoneIndicatorHandle? handle;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoDisplayMTQObjectTitle"),
		Description = LanguageManager.Get("AutoDisplayMTQObjectDescription"),
		Category = ModuleCategory.Interface
	};

	protected override void Init()
	{
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += OnTerritoryChanged;
		OnTerritoryChanged(0u);
	}

	protected override void Uninit()
	{
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnTerritoryChanged;
		handle?.Unreg();
		handle = null;
	}

	private void OnTerritoryChanged(uint territoryType)
	{
		handle?.Unreg();
		handle = OmenServiceBase<ZoneIndicatorRenderer>.Instance().RegTemporary(GetQuestObjects, (IGameObject x) => x.Position, new ZoneIndicatorOptions<IGameObject>
		{
			TextGetter = (IGameObject x) => new ZoneIndicatorText
			{
				Text = ReadOnlySeString.op_Implicit(x.Name),
				TextColor = KnownColor.LawnGreen.ToVector4(),
				Image = GetIcon(x.NamePlateIconID)
			}
		});
	}

	private unsafe static List<IGameObject> GetQuestObjects()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Invalid comparison between Unknown and I4
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		QuestObjects.Clear();
		QuestManager* ptr = QuestManager.Instance();
		if (ptr == null)
		{
			return QuestObjects;
		}
		if (IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()))
		{
			return QuestObjects;
		}
		IObjectTable objectTable = IOmenDalamudService<IObjectTable>.Instance();
		Enumerator<StdPair<uint, Pointer<EventHandler>>, uint, PairKeyExtractor<uint, Pointer<EventHandler>>> enumerator = ((EventHandlerModule)(&((EventFramework)EventFramework.Instance()).EventHandlerModule)).EventHandlerMap.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				EventHandler* value = enumerator.Current.Item2.Value;
				if (value == null || (int)((EventId)(&((EventHandlerInfo)(&((EventHandler)value).Info)).EventId)).ContentId != 1 || ((QuestManager)ptr).GetQuestById(((EventId)(&((EventHandlerInfo)(&((EventHandler)value).Info)).EventId)).EntryId) == null)
				{
					continue;
				}
				Enumerator<Pointer<GameObject>, Pointer<GameObject>, DefaultKeyExtractor<Pointer<GameObject>>> enumerator2 = ((EventHandler)value).EventObjects.GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						Pointer<GameObject> current = enumerator2.Current;
						GameObject* value2 = current.Value;
						if (value2 != null && ((GameObject)value2).GetIsTargetable() && ((EventHandler)value).IsActive(value2) && (*value2).IsMTQ())
						{
							IGameObject gameObject = objectTable[((GameObject)value2).ObjectIndex];
							if (gameObject != null && gameObject.ToStruct() == value2 && !QuestObjects.Contains(gameObject))
							{
								QuestObjects.Add(gameObject);
							}
						}
					}
				}
				finally
				{
					((IDisposable)enumerator2/*cast due to constrained. prefix*/).Dispose();
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
		return QuestObjects;
	}

	private static ZoneIndicatorText.TextImage? GetIcon(uint iconID)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
		GameIconLookup val = new GameIconLookup(iconID, false, true, (ClientLanguage?)null);
		ISharedImmediateTexture texture = default(ISharedImmediateTexture);
		if (!obj.TryGetFromGameIcon(ref val, ref texture))
		{
			return null;
		}
		return new ZoneIndicatorText.TextImage
		{
			Texture = texture,
			SizeGetter = () => new Vector2(ImGui.GetTextLineHeightWithSpacing())
		};
	}
}
