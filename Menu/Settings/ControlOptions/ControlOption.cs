using Godot;
using System;

public partial class ControlOption : Control
{
	public override void _Ready()
	{
		Game Global=GetNode<Game>("/root/Global");
		GetNode<OptionButton>("ControlMode/Mode1").Selected=(int)Global.control_mode;
		GetNode<OptionButton>("ControlMode/Mode1").ResetSize();
		GetNode<Control>("ControlMode").Position=new Vector2(GetNode<Control>("ControlMode").Position.X,GetNode<Control>("ControlMode").Position.Y-(32+18)*4);
	}

	public override void _Process(double delta)
	{
		var scene=GetTree().CurrentScene;
		var mode1=GetNode<OptionButton>("ControlMode/Mode1");
		Game Global=GetNode<Game>("/root/Global");
		if (Global.control_mode!=(Game.CONTROL_MODE)mode1.Selected)
		{
			Global.control_mode=(Game.CONTROL_MODE)mode1.Selected;
		}
		if (GetNode<Button>("Button/Keybinds").ButtonPressed)
		{
			if (FileAccess.FileExists("user://GodotTemplate/Settings/keybinds_temp.ini"))
			{
				DirAccess.RemoveAbsolute("user://GodotTemplate/Settings/keybinds_temp.ini");
			}
			Visible=false;
			var keybinds=GetNode<Keybinds>("../Keybinds");
			GetNode<ControlOptions>("../").current=ControlOptions.SCREEN.KEYBINDS;
			keybinds.CreateActionList();
			Keybinds.SaveKeybindsToTemp();
			keybinds.Visible=true;
		}
		if ((GetNode<Button>("Button/Back").ButtonPressed||Input.IsActionJustPressed("pause"))&&(scene.SceneFilePath=="res://Menu/Settings/ControlOptions/ControlOptions.tscn"||Visible))
		{
			Global.control_mode=(Game.CONTROL_MODE)mode1.Selected;
			var file=new ConfigFile();
			file.SetValue("ControlOptions","control_mode",mode1.Selected);
			Error err=file.Save("user://GodotTemplate/Settings/control.ini");
			if (err != Error.Ok)
			{
				GD.PushError($"Settings(Controls) file save failed, error code: {(int)err}("+err.ToString()+")");
			}
			if (scene.SceneFilePath=="res://Menu/Settings/ControlOptions/ControlOptions.tscn")
			{
				GetTree().ChangeSceneToFile("res://Menu/Settings/Settings.tscn");
			}
			else
			{
				GetNode<Node2D>("../").Visible=false;
			}
		}
	}
}
