using Godot;
using System;

public partial class DisplayMode : HBoxContainer
{
	public override void _Ready()
	{
		var option=GetNode<OptionButton>("DisplayMode1");
		if (DisplayServer.WindowGetMode()==DisplayServer.WindowMode.Windowed)
		{
			option.Selected=0;
		}
		if (DisplayServer.WindowGetMode()==DisplayServer.WindowMode.ExclusiveFullscreen)
		{
			option.Selected=1;
		}
		if (DisplayServer.WindowGetMode()==DisplayServer.WindowMode.Fullscreen)
		{
			option.Selected=2;
		}
	}

	public override void _Process(double delta)
	{
		var option=GetNode<OptionButton>("DisplayMode1");
		if (DisplayServer.WindowGetMode()==DisplayServer.WindowMode.Windowed&&option.Selected!=0)
		{
			option.Selected=0;
		}
		if (DisplayServer.WindowGetMode()==DisplayServer.WindowMode.ExclusiveFullscreen&&option.Selected!=1)
		{
			option.Selected=1;
		}
		if (DisplayServer.WindowGetMode()==DisplayServer.WindowMode.Fullscreen&&option.Selected!=2)
		{
			option.Selected=2;
		}
	}
	public void _on_display_mode_1_item_selected(int selected)
	{
		if (selected==0)
		{
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
			GetNode<Resolution>("../../VBoxContainer/Resolution")._on_resolution_1_item_selected(GetNode<OptionButton>("../../VBoxContainer/Resolution/Resolution1").Selected);
		}
		if (selected==1)
		{
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
		}
		if (selected==2)
		{
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
		}
	}
}
