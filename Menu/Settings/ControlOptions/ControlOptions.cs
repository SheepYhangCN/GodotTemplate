using Godot;
using System;

public partial class ControlOptions : Control
{
	internal enum SCREEN//画面
	{
		CONTROL,//控制选项
		KEYBINDS//按键绑定
	}
	internal SCREEN current=SCREEN.CONTROL;
	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
		if (current==SCREEN.CONTROL)
		{
			if (GetNode<Control>("Keybinds").Visible)
			{
				GetNode<Control>("Keybinds").Visible=false;
			}
			if (!GetNode<Control>("ControlOption").Visible)
			{
				GetNode<Control>("ControlOption").Visible=true;
			}
		}
		if (current==SCREEN.KEYBINDS)
		{
			if (GetNode<Control>("ControlOption").Visible)
			{
				GetNode<Control>("ControlOption").Visible=false;
			}
			if (!GetNode<Control>("Keybinds").Visible)
			{
				GetNode<Control>("Keybinds").Visible=true;
			}
		}
	}
}
