using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Command;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic.Interface;

public class FastMinimizeWindow : ModuleBase
{
	private class Config : ModuleConfig
	{
		public bool AlwaysAddTrayIcon;

		public bool DrawButton = true;

		public bool IsTransparentWhenNotHovered;

		public ClickBehavior LeftClickBehavior = ClickBehavior.MinimizeToTaskbar;

		public ButtonPosition Position = ButtonPosition.TopRight;

		public ClickBehavior RightClickBehavior = ClickBehavior.MinimizeToTray;

		public float Scale = 0.5f;
	}

	private enum ClickBehavior
	{
		None,
		MinimizeToTaskbar,
		MinimizeToTray
	}

	private enum ButtonPosition
	{
		TopLeft,
		TopRight,
		BottomLeft,
		BottomRight
	}

	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static HandlerDelegate _003C0_003E__OnMinimizeCommand;
	}

	private Config config;

	private NotifyIcon? trayIcon;

	private const string COMMAND_MINI = "mini";

	private const string COMMAND_TRAY = "tray";

	private const int SW_MINIMIZE = 6;

	private const int SW_HIDE = 0;

	private const int SW_SHOW = 5;

	private const int SW_RESTORE = 9;

	private const ImGuiWindowFlags BUTTON_WINDOW_FLAGS = (ImGuiWindowFlags)536797;

	private static readonly FrozenDictionary<ClickBehavior, string> BehaviorNames = new Dictionary<ClickBehavior, string>
	{
		[ClickBehavior.None] = LuminaWrapper.GetAddonText(7u),
		[ClickBehavior.MinimizeToTaskbar] = LanguageManager.Get("FastMinimizeWindow-MinimizeToTaskbar"),
		[ClickBehavior.MinimizeToTray] = LanguageManager.Get("FastMinimizeWindow-MinimizeToTray")
	}.ToFrozenDictionary();

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("FastMinimizeWindowTitle"),
		Description = LanguageManager.Get("FastMinimizeWindowDescription", "mini", "tray"),
		Category = ModuleCategory.Interface,
		Author = new string[1] { "Rorinnn" }
	};

	protected override void Init()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Expected O, but got Unknown
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Expected O, but got Unknown
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		CommandManager commandManager = OmenServiceBase<CommandManager>.Instance();
		object obj = _003C_003EO._003C0_003E__OnMinimizeCommand;
		if (obj == null)
		{
			HandlerDelegate val = OnMinimizeCommand;
			_003C_003EO._003C0_003E__OnMinimizeCommand = val;
			obj = (object)val;
		}
		commandManager.AddSubCommand("mini", new CommandInfo((HandlerDelegate)obj)
		{
			HelpMessage = LanguageManager.Get("FastMinimizeWindow-MinimizeToTaskbar")
		});
		OmenServiceBase<CommandManager>.Instance().AddSubCommand("tray", new CommandInfo(new HandlerDelegate(OnTrayCommand))
		{
			HelpMessage = LanguageManager.Get("FastMinimizeWindow-MinimizeToTray")
		});
		OmenServiceBase<WindowManager>.Instance().PostDraw += DrawMinimizeButton;
		if (config.AlwaysAddTrayIcon)
		{
			CreateTrayIcon();
		}
	}

	protected unsafe override void Uninit()
	{
		OmenServiceBase<WindowManager>.Instance().PostDraw -= DrawMinimizeButton;
		OmenServiceBase<CommandManager>.Instance().RemoveSubCommand("mini");
		OmenServiceBase<CommandManager>.Instance().RemoveSubCommand("tray");
		if (base.IsEnabled)
		{
			nint windowHandle = ((GameWindow)((Framework)Framework.Instance()).GameWindow).WindowHandle;
			if (windowHandle != IntPtr.Zero && !IsWindowVisible(windowHandle))
			{
				ShowWindow(windowHandle, 5);
				SetForegroundWindow(windowHandle);
			}
		}
		DisposeTrayIcon();
	}

	protected override void ConfigUI()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("Command")));
		IndentDisposable val = ImRaii.PushIndent(1, true);
		try
		{
			ImU8String val2 = new ImU8String(8, 2);
			((ImU8String)(ref val2)).AppendLiteral("/pdr ");
			((ImU8String)(ref val2)).AppendFormatted<string>("mini");
			((ImU8String)(ref val2)).AppendLiteral(" → ");
			((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get("FastMinimizeWindow-MinimizeToTaskbar"));
			ImGui.TextUnformatted(val2);
			ImU8String val3 = new ImU8String(8, 2);
			((ImU8String)(ref val3)).AppendLiteral("/pdr ");
			((ImU8String)(ref val3)).AppendFormatted<string>("tray");
			((ImU8String)(ref val3)).AppendLiteral(" → ");
			((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get("FastMinimizeWindow-MinimizeToTray"));
			ImGui.TextUnformatted(val3);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		ImGui.NewLine();
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("FastMinimizeWindow-AlwaysAddTrayIcon")), ref config.AlwaysAddTrayIcon))
		{
			if (config.AlwaysAddTrayIcon)
			{
				CreateTrayIcon();
			}
			else
			{
				DisposeTrayIcon();
			}
			config.Save(this);
		}
		ImGuiOm.HelpMarker(LanguageManager.Get("FastMinimizeWindow-AlwaysAddTrayIcon-Help"), 40f, (FontAwesomeIcon)61530);
		ImGui.NewLine();
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("FastMinimizeWindow-DrawButton")), ref config.DrawButton))
		{
			config.Save(this);
		}
		if (!config.DrawButton)
		{
			return;
		}
		ItemWidthDisposable val4 = ImRaii.ItemWidth(250f * Globals.GlobalUIScale);
		try
		{
			val = ImRaii.PushIndent(1, true);
			try
			{
				if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("FastMinimizeWindow-IsTransparentWhenNotHovered")), ref config.IsTransparentWhenNotHovered))
				{
					config.Save(this);
				}
				if (ImGui.SliderFloat(ImU8String.op_Implicit(LanguageManager.Get("Scale")), ref config.Scale, 0.1f, 2f, default(ImU8String), (ImGuiSliderFlags)0))
				{
					config.Scale = Math.Clamp(config.Scale, 0.1f, 2f);
				}
				if (ImGui.IsItemDeactivatedAfterEdit())
				{
					config.Save(this);
				}
				DrawBehaviorCombo(LanguageManager.Get("FastMinimizeWindow-LeftClickBehavior"), ref config.LeftClickBehavior);
				DrawBehaviorCombo(LanguageManager.Get("FastMinimizeWindow-RightClickBehavior"), ref config.RightClickBehavior);
				ComboDisposable val5 = ImRaii.Combo(ImU8String.op_Implicit(LanguageManager.Get("Position")), ImU8String.op_Implicit(LanguageManager.Get($"{config.Position}")));
				try
				{
					if (!ComboDisposable.op_Implicit(val5))
					{
						return;
					}
					ButtonPosition[] values = Enum.GetValues<ButtonPosition>();
					foreach (ButtonPosition buttonPosition in values)
					{
						if (ImGui.Selectable(ImU8String.op_Implicit(LanguageManager.Get($"{buttonPosition}", buttonPosition == config.Position)), false, (ImGuiSelectableFlags)0, default(Vector2)))
						{
							config.Position = buttonPosition;
							config.Save(this);
						}
					}
				}
				finally
				{
					((ComboDisposable)(ref val5)).Dispose();
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val4)?.Dispose();
		}
	}

	private void DrawBehaviorCombo(string label, ref ClickBehavior behavior)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		ComboDisposable val = ImRaii.Combo(ImU8String.op_Implicit(label), ImU8String.op_Implicit(BehaviorNames.GetValueOrDefault(behavior, LuminaWrapper.GetAddonText(7u))));
		try
		{
			if (!val)
			{
				return;
			}
			foreach (var (clickBehavior2, text2) in BehaviorNames)
			{
				if (ImGui.Selectable(ImU8String.op_Implicit(text2), behavior == clickBehavior2, (ImGuiSelectableFlags)0, default(Vector2)))
				{
					behavior = clickBehavior2;
					config.Save(this);
				}
			}
		}
		finally
		{
			((ComboDisposable)(ref val)).Dispose();
		}
	}

	private void DrawMinimizeButton()
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		if (!config.DrawButton)
		{
			return;
		}
		float num = 20f * config.Scale;
		Vector2 vector;
		ImGuiViewportPtr mainViewport;
		switch (config.Position)
		{
		case ButtonPosition.TopLeft:
			vector = new Vector2(0f, 0f);
			break;
		case ButtonPosition.TopRight:
			mainViewport = ImGuiHelpers.MainViewport;
			vector = new Vector2(((ImGuiViewportPtr)(ref mainViewport)).Size.X - num, 0f);
			break;
		case ButtonPosition.BottomLeft:
			mainViewport = ImGuiHelpers.MainViewport;
			vector = new Vector2(0f, ((ImGuiViewportPtr)(ref mainViewport)).Size.Y - num);
			break;
		case ButtonPosition.BottomRight:
			mainViewport = ImGuiHelpers.MainViewport;
			vector = ((ImGuiViewportPtr)(ref mainViewport)).Size - new Vector2(num);
			break;
		default:
			vector = Vector2.Zero;
			break;
		}
		Vector2 vector2 = vector;
		StyleDisposable val = ImRaii.PushStyle((ImGuiStyleVar)1, Vector2.Zero, true);
		try
		{
			StyleDisposable val2 = ImRaii.PushStyle((ImGuiStyleVar)4, Vector2.Zero, true);
			try
			{
				ImGuiHelpers.ForceNextWindowMainViewport();
				ImGuiHelpers.SetNextWindowPosRelativeMainViewport(vector2, (ImGuiCond)0, default(Vector2));
				if (!ImGui.Begin(ImU8String.op_Implicit("##DRMinimizeButton"), (ImGuiWindowFlags)536797))
				{
					return;
				}
				ColorDisposable val3 = (config.IsTransparentWhenNotHovered ? ImRaii.PushColor((ImGuiCol)21, 0u, true) : null);
				try
				{
					if (ImGui.Button(ImU8String.op_Implicit("##MinimizeBtn"), new Vector2(num, num)))
					{
						HandleClick(config.LeftClickBehavior);
					}
					if (ImGui.IsItemHovered() && ImGui.IsMouseReleased((ImGuiMouseButton)1))
					{
						HandleClick(config.RightClickBehavior);
					}
					ImGui.End();
				}
				finally
				{
					((IDisposable)val3)?.Dispose();
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void HandleClick(ClickBehavior behavior)
	{
		switch (behavior)
		{
		case ClickBehavior.MinimizeToTaskbar:
			TryMinimize();
			break;
		case ClickBehavior.MinimizeToTray:
			TryMinimizeToTray();
			break;
		}
	}

	private static void OnMinimizeCommand(string command, string args)
	{
		TryMinimize();
	}

	private void OnTrayCommand(string command, string args)
	{
		TryMinimizeToTray();
	}

	private unsafe static void TryMinimize()
	{
		nint windowHandle = ((GameWindow)((Framework)Framework.Instance()).GameWindow).WindowHandle;
		if (windowHandle != IntPtr.Zero)
		{
			ShowWindow(windowHandle, 6);
		}
	}

	private unsafe void TryMinimizeToTray()
	{
		nint windowHandle = ((GameWindow)((Framework)Framework.Instance()).GameWindow).WindowHandle;
		if (windowHandle != IntPtr.Zero && ((trayIcon?.Visible ?? false) || CreateTrayIcon()))
		{
			ShowWindow(windowHandle, 0);
		}
	}

	private static Icon? TryExtractGameIcon()
	{
		string text = Process.GetCurrentProcess().MainModule?.FileName;
		if (text == null)
		{
			return null;
		}
		return Icon.ExtractAssociatedIcon(text);
	}

	private bool CreateTrayIcon()
	{
		if (trayIcon?.Visible ?? false)
		{
			return true;
		}
		DisposeTrayIcon();
		try
		{
			trayIcon = new NotifyIcon
			{
				Icon = (TryExtractGameIcon() ?? SystemIcons.Application),
				Text = Info.Title,
				Visible = true
			};
			trayIcon.Click += OnTrayIconClick;
			return true;
		}
		catch
		{
			trayIcon = null;
			return false;
		}
	}

	private unsafe void OnTrayIconClick(object? sender, EventArgs e)
	{
		nint windowHandle = ((GameWindow)((Framework)Framework.Instance()).GameWindow).WindowHandle;
		if (windowHandle == IntPtr.Zero)
		{
			return;
		}
		if (!IsWindowVisible(windowHandle))
		{
			ShowWindow(windowHandle, 5);
			SetForegroundWindow(windowHandle);
			if (!config.AlwaysAddTrayIcon)
			{
				DisposeTrayIcon();
			}
		}
		else
		{
			ShowWindow(windowHandle, 9);
			SetForegroundWindow(windowHandle);
		}
	}

	private void DisposeTrayIcon()
	{
		if (trayIcon != null)
		{
			trayIcon.Click -= OnTrayIconClick;
			trayIcon.Visible = false;
			trayIcon.Dispose();
			trayIcon = null;
		}
	}

	[DllImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool ShowWindow(nint hWnd, int nCmdShow);

	[DllImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool IsWindowVisible(nint hWnd);

	[DllImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool SetForegroundWindow(nint hWnd);
}
