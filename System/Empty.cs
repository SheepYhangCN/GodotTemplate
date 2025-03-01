using Godot;
using System;

public partial class Empty : Node2D
{
	public override void _Ready()
	{
		var tree = GetTree();
		if (!FileAccess.FileExists("user://GodotTemplate/Settings/settings.ini"))
		{
			tree.ChangeSceneToFile("res://Menu/InitLang/InitLang.tscn");
		}
		else
		{
			tree.ChangeSceneToFile("res://Menu/MainMenu/MainMenu.tscn");
		}
	}

	public override void _Process(double delta)
	{
	}
}
