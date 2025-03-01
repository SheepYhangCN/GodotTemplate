using Godot;
using System;

public partial class HDR : HBoxContainer
{
	public override void _Ready()
	{
		GetNode<CheckButton>("Switch/HDR1").ButtonPressed=GetViewport().UseHdr2D;
	}

	public override void _Process(double delta)
	{
	}

	public void _on_hdr_1_toggled(bool toggled_on)
	{
		GetViewport().UseHdr2D=toggled_on;
	}
}
