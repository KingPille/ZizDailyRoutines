using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Runtime.Hosts;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;

namespace DailyRoutines.Common.Interface.Windows;

public class OverlayConfig : Window
{
	private const ImGuiWindowFlags WINDOW_FLAGS = (ImGuiWindowFlags)8;

	private ModuleBase ModuleBase { get; init; }

	public OverlayConfig(ModuleBase moduleBase)
		: base(moduleBase.Info.Title + "###" + moduleBase.GetType().Name + "ConfigOverlay")
	{
		((Window)this).Flags = (ImGuiWindowFlags)8;
		((Window)this).RespectCloseHotkey = false;
		ModuleBase = moduleBase;
		ManagerHost.Current.AddWindow((Window)(object)this);
	}

	public override void Draw()
	{
		ModuleBase.PublicConfigUI();
	}

	public override bool DrawConditions()
	{
		return ModuleBase.IsEnabled;
	}
}
