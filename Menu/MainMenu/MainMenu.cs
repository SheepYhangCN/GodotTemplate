using Godot;
using System;

public partial class MainMenu : Control
{
	double timer=0;
	bool pressed=false;
	public override void _Ready()
	{
		Game Global=GetNode<Game>("/root/Global");
		var quit=GetNode<Button>("Control/Quit");
		var settings=GetNode<Button>("Control/Settings");
		var version=GetNode<Label>("Control/Version");
		quit.ResetSize();
		settings.ResetSize();
		version.ResetSize();
		var t="";
		if (Global.mod)
		{
			t+=TranslationServer.Translate("locInfoMod").ToString().Replace("{ModsLoaded}",Global.mods_loaded.Length.ToString());
		}
		if (Global.locale)
		{
			t+=TranslationServer.Translate("locLocInfo").ToString().Replace("{LocalesLoaded}",Global.locale_loaded.Length.ToString());
		}
		if (Global.mods_loaded.Length > 1)
		{
			t=t.Replace("{ModS}","mods");
		}
		else
		{
			t=t.Replace("{ModS}","mod");
		}
		if (Global.locale_loaded.Length > 1)
		{
			t=t.Replace("{fileS}","files");
		}
		else
		{
			t=t.Replace("{fileS}","file");
		}
		version.Text=t+"v"+Game.version+(Global.mod ? " - "+TranslationServer.Translate("locEnabledMod") : "");
	}

	public override void _Process(double delta)
	{
		var tree=GetTree();
		var quit=GetNode<Button>("Control/Quit");
		var settings=GetNode<Button>("Control/Settings");
		if (pressed)
		{
			timer+=delta;
			if (settings.ButtonPressed)
			{
				tree.ChangeSceneToFile("res://Menu/Settings/Settings.tscn");
			}
			if (quit.ButtonPressed)
			{
				tree.Quit();
			}
		}
	}
	public override void _Input(InputEvent @event)
	{
		var Global=GetNode<Game>("/root/Global");
		base._Input(@event);
		if (@event is InputEventKey || @event is InputEventMouseButton || @event is InputEventScreenTouch || @event is InputEventJoypadButton)
		{
			var e=true;
			if (!pressed)
			{
				var actions=InputMap.GetActions();
				var actions_dg=new Godot.Collections.Array<StringName>();
				for (var a=0;a<actions.Count;a+=1)
				{
					if (actions[a].ToString().StartsWith("debug_"))
					{
						actions_dg.Add(actions[a]);
					}
				}
				for (var a=0;a<actions_dg.Count;a+=1)
				{
					if (InputMap.EventIsAction(@event,actions_dg[a]))
					{
						e=false;
					}
				}
				if (e)
				{
					if ((!FileAccess.FileExists("user://GodotTemplate/Settings/control.ini"))&&Input.GetConnectedJoypads().Count>0)
					{
						/*for (var device=0;device<Input.GetConnectedJoypads().Count;device+=1)
						{
							for (var button=0;button<(int)JoyButton.Max;button+=1)
							{
								if (Input.IsJoyButtonPressed(device,(JoyButton)button))
								{
									//Global.control_mode=Game.CONTROL_MODE.CONTROLLER;
								}
							}
						}*/
						if (@event is InputEventJoypadButton)
						{
							//Global.control_mode=Game.CONTROL_MODE.CONTROLLER;
						}
					}
					pressed=true;
					Global.menu_pressed=true;
				}
			}
		}
	}
}
