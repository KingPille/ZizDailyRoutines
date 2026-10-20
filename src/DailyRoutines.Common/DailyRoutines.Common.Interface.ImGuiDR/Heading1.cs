using System;
using DailyRoutines.Common.Info;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using OmenTools.ImGuiOm;

namespace DailyRoutines.Common.Interface.ImGuiDR;

public ref struct Heading1 : IDisposable
{
	public bool Alive { get; private set; } = true;

	public Heading1(string text, string? help = null)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TextColored(ref Colors.Heading1, ImU8String.op_Implicit(text));
		if (help != null)
		{
			ImGuiOm.HelpMarker(help, 40f, (FontAwesomeIcon)61530);
		}
		ImGui.Indent();
	}

	public void Dispose()
	{
		if (Alive)
		{
			ImGui.Unindent();
			Alive = false;
		}
	}
}
