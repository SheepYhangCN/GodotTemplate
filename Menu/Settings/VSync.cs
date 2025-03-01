using Godot;
using System;

public partial class VSync : HBoxContainer
{
	public override void _Ready()
	{
		GetNode<CheckButton>("VSync1").ButtonPressed=(DisplayServer.WindowGetVsyncMode()==DisplayServer.VSyncMode.Enabled);
	}

	public override void _Process(double delta)
	{
	}

	public void _on_v_sync_1_toggled(bool toggled_on)
	{
		DisplayServer.WindowSetVsyncMode((DisplayServer.VSyncMode)(toggled_on ? 1 : 0));
	}
}
