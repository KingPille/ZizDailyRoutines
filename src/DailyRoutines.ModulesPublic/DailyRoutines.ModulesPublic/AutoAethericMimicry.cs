using System;
using System.Linq;
using System.Numerics;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;
using Lumina.Text;
using Lumina.Text.ReadOnly;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;

namespace DailyRoutines.ModulesPublic;

public class AutoAethericMimicry : ModuleBase
{
	private class AddonDRAutoAethericMimicry : NativeAddon
	{
		public static AddonDRAutoAethericMimicry? Addon;

		private IconButtonNode dpsButton;

		private IconButtonNode healerButton;

		private IconButtonNode tankButton;

		public static void OpenWithNewInstance()
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			Addon?.Dispose();
			Addon = new AddonDRAutoAethericMimicry
			{
				InternalName = "DRAutoAethericMimicry",
				Title = ReadOnlySeString.op_Implicit(LanguageManager.Get("SelectTarget")),
				Size = new Vector2(230f, 120f),
				CreateWindowNode = () => new WindowNode()
			};
			AddonDRAutoAethericMimicry? addon = Addon;
			Vector2 mousePos = ImGui.GetMousePos();
			Vector2 size = Addon.Size;
			size.X = Addon.Size.X / 1.5f;
			addon.SetWindowPosition(mousePos - size);
			Addon.Open();
		}

		protected unsafe override void OnSetup(AtkUnitBase* addon, Span<AtkValue> atkValues)
		{
			HorizontalListNode horizontalListNode = new HorizontalListNode
			{
				Size = new Vector2(base.ContentSize.X + 10f, 64f),
				Position = base.ContentStartPosition,
				ItemSpacing = 16f
			};
			tankButton = CreateButton(62581u, new byte[1] { 1 }, 1082u);
			horizontalListNode.AddNode(tankButton);
			healerButton = CreateButton(62582u, new byte[1] { 4 }, 1083u);
			horizontalListNode.AddNode(healerButton);
			dpsButton = CreateButton(62583u, new byte[2] { 2, 3 }, 1084u);
			horizontalListNode.AddNode(dpsButton);
			tankButton.IsEnabled = TryGetChara(new byte[1] { 1 }, out IPlayerCharacter chara);
			healerButton.IsEnabled = TryGetChara(new byte[1] { 4 }, out chara);
			dpsButton.IsEnabled = TryGetChara(new byte[2] { 2, 3 }, out chara);
			horizontalListNode.AttachNode(this);
			unsafe static IconButtonNode CreateButton(uint iconID, byte[] roles, uint addonTextID)
			{
				//IL_0056: Unknown result type (might be due to invalid IL or missing references)
				//IL_0083: Unknown result type (might be due to invalid IL or missing references)
				IconButtonNode iconButtonNode = new IconButtonNode
				{
					Size = new Vector2(58f),
					IconId = iconID,
					OnClick = delegate
					{
						if (TryGetChara(roles, out IPlayerCharacter chara2))
						{
							OmenServiceBase<UseActionManager>.Instance().UseActionLocation((ActionType)1, 18322u, chara2.EntityID, default(Vector3), 0u, 0);
						}
						Notify(chara2);
						Addon.Close();
					},
					TextTooltip = ReadOnlySeString.op_Implicit(LuminaWrapper.GetActionName(18322u) + "：" + LuminaWrapper.GetAddonText(addonTextID))
				};
				RentedSeStringBuilder val = default(RentedSeStringBuilder);
				((RentedSeStringBuilder)(ref val))._002Ector();
				try
				{
					SeStringBuilder builder = ((RentedSeStringBuilder)(ref val)).Builder;
					builder.AppendIcon(163u);
					TextNode textNode = new TextNode();
					textNode.String = builder.ToReadOnlySeString();
					textNode.Size = new Vector2(16f);
					textNode.Position = new Vector2(0f, iconButtonNode.Height - 16f);
					textNode.AttachNode((NodeBase?)iconButtonNode, NodePosition.AsLastChild);
					return iconButtonNode;
				}
				finally
				{
					((IDisposable)(*(RentedSeStringBuilder*)(&val))/*cast due to constrained. prefix*/).Dispose();
				}
			}
		}

		protected unsafe override void OnUpdate(AtkUnitBase* addon)
		{
			if (LocalPlayerState.ClassJob != 36 || DalamudServiceExtension.Instance<IKeyState>()[(VirtualKey)27])
			{
				Close();
				if (Addons.SystemMenu != null)
				{
					((AtkUnitBase)Addons.SystemMenu).Close(true);
				}
			}
			else if (Throttler.Shared.Throttle("AutoAethericMimicry.OnUpdateButtons"))
			{
				tankButton.IsEnabled = TryGetChara(new byte[1] { 1 }, out IPlayerCharacter chara);
				healerButton.IsEnabled = TryGetChara(new byte[1] { 4 }, out chara);
				dpsButton.IsEnabled = TryGetChara(new byte[2] { 2, 3 }, out chara);
			}
		}

		private static bool TryGetChara(byte[] roles, out IPlayerCharacter? chara)
		{
			chara = null;
			chara = (from x in IOmenDalamudService<IObjectTable>.Instance().Where(delegate(IGameObject x)
				{
					//IL_0023: Unknown result type (might be due to invalid IL or missing references)
					//IL_0028: Unknown result type (might be due to invalid IL or missing references)
					//IL_002b: Unknown result type (might be due to invalid IL or missing references)
					//IL_0030: Unknown result type (might be due to invalid IL or missing references)
					if (x is IPlayerCharacter playerCharacter && playerCharacter.EntityID != LocalPlayerState.EntityID)
					{
						ReadOnlySpan<byte> span = roles;
						ClassJob value = playerCharacter.ClassJob.Value;
						return span.Contains(((ClassJob)(ref value)).Role);
					}
					return false;
				})
				where x != null && x.Distance <= 25
				orderby x.Distance
				select x).OfType<IPlayerCharacter>().FirstOrDefault();
			return chara != null;
		}

		private unsafe static void Notify(IPlayerCharacter? chara)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Expected I4, but got Unknown
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			if (chara == null)
			{
				((RaptureLogModule)RaptureLogModule.Instance()).ShowLogMessage(563u);
				return;
			}
			RentedSeStringBuilder val = default(RentedSeStringBuilder);
			((RentedSeStringBuilder)(ref val))._002Ector();
			try
			{
				SeStringBuilder builder = ((RentedSeStringBuilder)(ref val)).Builder;
				builder.AppendIcon((uint)(int)chara.ClassJob.Value.ToBitmapFontIcon()).Append(chara.Name);
				if (chara.HomeWorld.RowId != GameState.HomeWorld)
				{
					SeStringBuilder obj = builder.AppendIcon(88u);
					World value = chara.HomeWorld.Value;
					obj.Append(((World)(ref value)).Name);
				}
				NotifyHelper.Toast(LanguageManager.GetSe("AutoAethericMimicry-Notification-MimickedTarget", builder.ToReadOnlySeString()));
			}
			finally
			{
				((IDisposable)(*(RentedSeStringBuilder*)(&val))/*cast due to constrained. prefix*/).Dispose();
			}
		}
	}

	private static readonly uint[] Status = new uint[3] { 2124u, 2125u, 2126u };

	private const uint AethericMimicryActionID = 18322u;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoAethericMimicryTitle"),
		Description = LanguageManager.Get("AutoAethericMimicryDescription"),
		Category = ModuleCategory.Action
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected override void Init()
	{
		OmenServiceBase<UseActionManager>.Instance().RegPreUseAction(OnPreUseAction);
	}

	protected override void Uninit()
	{
		OmenServiceBase<UseActionManager>.Instance().Unreg(OnPreUseAction);
		AddonDRAutoAethericMimicry.Addon?.Dispose();
		AddonDRAutoAethericMimicry.Addon = null;
	}

	private static void OnPreUseAction(ref bool isPrevented, ref ActionType actionType, ref uint actionID, ref ulong targetID, ref uint extraParam, ref UseActionMode queueState, ref uint comboRouteID)
	{
		if ((int)actionType == 1 && actionID == 18322 && (targetID == 3758096384u || targetID == LocalPlayerState.EntityID) && !Status.Any((uint x) => LocalPlayerState.HasStatus(x, out var _)))
		{
			AddonDRAutoAethericMimicry.OpenWithNewInstance();
			isPrevented = true;
		}
	}
}
