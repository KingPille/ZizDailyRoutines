using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Helpers;

namespace OmenTools.KamiToolKit.Addons;

public abstract class AttachedAddon : NativeAddon
{
	public enum AttachedAddonPosition
	{
		LeftTop,
		LeftCenter,
		LeftBottom,
		TopLeft,
		TopCenter,
		TopRight,
		RightTop,
		RightCenter,
		RightBottom,
		BottomLeft,
		BottomCenter,
		BottomRight
	}

	private readonly string hostAddonName;

	private readonly bool runSetupForCurrentHostAddon;

	private bool isClosingAddonOnly;

	protected virtual AttachedAddonPosition AttachPosition => AttachedAddonPosition.LeftTop;

	protected virtual Vector2 PositionOffset => Vector2.Zero;

	protected virtual bool CanOpenAddon => true;

	protected unsafe AtkUnitBase* HostAddon => AddonHelper.GetByName(hostAddonName);

	protected unsafe AttachedAddon(string hostAddon, params AddonEvent[] hostAddonEvents)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Expected O, but got Unknown
		hostAddonName = hostAddon;
		runSetupForCurrentHostAddon = hostAddonEvents.Contains((AddonEvent)1, null);
		foreach (AddonEvent item in ((IEnumerable<AddonEvent>)(object)new AddonEvent[2]
		{
			(AddonEvent)5,
			(AddonEvent)6
		}).Concat(hostAddonEvents).Distinct())
		{
			DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener(item, hostAddon, new AddonEventDelegate(OnHostAddonLifecycle));
		}
		DalamudServiceExtension.Instance<IFramework>().RunOnTick((Action)delegate
		{
			if ((*HostAddon).IsAddonAndNodesReady())
			{
				if (runSetupForCurrentHostAddon)
				{
					OnHostAddon((AddonEvent)1, null);
				}
				if (CanOpenAddon)
				{
					OpenAddon();
				}
			}
		}, default(TimeSpan), 0, default(CancellationToken));
	}

	public override void Dispose()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnHostAddonLifecycle });
		isClosingAddonOnly = true;
		base.Dispose();
	}

	protected virtual void OnHostAddon(AddonEvent type, AddonArgs? args)
	{
	}

	protected unsafe virtual void OnAttachedAddonUpdate(AtkUnitBase* addon, AtkUnitBase* hostAddon)
	{
	}

	protected unsafe virtual void OnAttachedAddonFinalize(AtkUnitBase* addon)
	{
	}

	protected unsafe virtual bool CanCloseHostAddon(AtkUnitBase* hostAddon)
	{
		if (hostAddon != null)
		{
			return ((AtkUnitBase)hostAddon).IsVisible;
		}
		return false;
	}

	protected unsafe sealed override void OnUpdate(AtkUnitBase* addon)
	{
		AtkUnitBase* hostAddon = HostAddon;
		if (!(*HostAddon).IsAddonAndNodesReady())
		{
			CloseAddonOnly();
			return;
		}
		Vector2 vector = new Vector2(((AtkResNode)((AtkUnitBase)hostAddon).RootNode).ScreenX, ((AtkResNode)((AtkUnitBase)hostAddon).RootNode).ScreenY);
		Vector2 vector2 = new Vector2(((AtkUnitBase)hostAddon).GetScaledWidth(true), ((AtkUnitBase)hostAddon).GetScaledHeight(true));
		Vector2 vector3 = new Vector2(((AtkUnitBase)addon).GetScaledWidth(true), ((AtkUnitBase)addon).GetScaledHeight(true));
		Vector2 vector4;
		switch (AttachPosition)
		{
		case AttachedAddonPosition.LeftTop:
			vector4 = new Vector2(vector.X - vector3.X, vector.Y);
			break;
		case AttachedAddonPosition.LeftCenter:
			vector4 = new Vector2(vector.X - vector3.X, vector.Y + (vector2.Y - vector3.Y) / 2f);
			break;
		case AttachedAddonPosition.LeftBottom:
			vector4 = new Vector2(vector.X - vector3.X, vector.Y + vector2.Y - vector3.Y);
			break;
		case AttachedAddonPosition.TopLeft:
		{
			Vector2 vector5 = vector;
			vector5.Y = vector.Y - vector3.Y;
			vector4 = vector5;
			break;
		}
		case AttachedAddonPosition.TopCenter:
			vector4 = new Vector2(vector.X + (vector2.X - vector3.X) / 2f, vector.Y - vector3.Y);
			break;
		case AttachedAddonPosition.TopRight:
			vector4 = new Vector2(vector.X + vector2.X - vector3.X, vector.Y - vector3.Y);
			break;
		case AttachedAddonPosition.RightTop:
			vector4 = new Vector2(vector.X + vector2.X, vector.Y);
			break;
		case AttachedAddonPosition.RightCenter:
			vector4 = new Vector2(vector.X + vector2.X, vector.Y + (vector2.Y - vector3.Y) / 2f);
			break;
		case AttachedAddonPosition.RightBottom:
			vector4 = new Vector2(vector.X + vector2.X, vector.Y + vector2.Y - vector3.Y);
			break;
		case AttachedAddonPosition.BottomLeft:
		{
			Vector2 vector5 = vector;
			vector5.Y = vector.Y + vector2.Y;
			vector4 = vector5;
			break;
		}
		case AttachedAddonPosition.BottomCenter:
			vector4 = new Vector2(vector.X + (vector2.X - vector3.X) / 2f, vector.Y + vector2.Y);
			break;
		case AttachedAddonPosition.BottomRight:
			vector4 = new Vector2(vector.X + vector2.X - vector3.X, vector.Y + vector2.Y);
			break;
		default:
			vector4 = vector;
			break;
		}
		Vector2 vector6 = vector4;
		SetWindowPosition(vector6 + PositionOffset);
		OnAttachedAddonUpdate(addon, hostAddon);
	}

	protected unsafe sealed override void OnFinalize(AtkUnitBase* addon)
	{
		OnAttachedAddonFinalize(addon);
		if (isClosingAddonOnly)
		{
			isClosingAddonOnly = false;
			return;
		}
		AtkUnitBase* hostAddon = HostAddon;
		if (CanCloseHostAddon(hostAddon))
		{
			((AtkUnitBase)hostAddon).Close(true);
		}
	}

	private void OnHostAddonLifecycle(AddonEvent type, AddonArgs? args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Invalid comparison between Unknown and I4
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Invalid comparison between Unknown and I4
		OnHostAddon(type, args);
		if ((int)type != 5)
		{
			if ((int)type == 6)
			{
				CloseAddonOnly();
			}
		}
		else if (CanOpenAddon)
		{
			OpenAddon();
		}
	}

	protected void CloseAddonOnly()
	{
		if (base.IsOpen)
		{
			isClosingAddonOnly = true;
			Close();
		}
	}

	private unsafe void OpenAddon()
	{
		if (!base.IsOpen && (*HostAddon).IsAddonAndNodesReady())
		{
			Open();
		}
	}
}
