using System;

namespace OmenTools.OmenService;

public sealed class ContextMenuItemClickedArgs
{
	private readonly Action<ContextMenuSubmenu> openSubmenu;

	public ContextMenuOpenedArgs Source { get; }

	internal ContextMenuItemClickedArgs(ContextMenuOpenedArgs source, Action<ContextMenuSubmenu> openSubmenu)
	{
		Source = source;
		this.openSubmenu = openSubmenu;
	}

	public void OpenSubmenu(ContextMenuSubmenu submenu)
	{
		openSubmenu(submenu);
	}
}
