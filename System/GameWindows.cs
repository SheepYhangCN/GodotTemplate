using Godot;
using System;

public partial class GameWindows : Control
{
	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}

	public void _on_debug_menu_close_requested()
	{
		GetNode<Window>("DebugMenu").Visible = false;
	}
}
