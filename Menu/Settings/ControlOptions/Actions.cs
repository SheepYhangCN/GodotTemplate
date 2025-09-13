using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class Actions : Control
{
	internal int num=0;
	public override void _Ready()
	{
		var action_list=GetNode<VBoxContainer>("MarginContainer/VBoxContainer/List");
		GetNode<Control>("MarginContainer/VBoxContainer/List/ActionButton").QueueFree();
		var scene=GD.Load<PackedScene>("res://Menu/Settings/ControlOptions/ActionButton.tscn");
		var events=InputMap.ActionGetEvents(Game.actions.Keys.ToArray()[num]);
		var b=0;
		for (var a=0;a<events.Count;a+=1)
		{
			if (!(events[a].AsText().Contains("Controller")||events[a].AsText().Contains("Joypad")))
			{
				b+=1;
				var button=scene.Instantiate<ActionButton>();
				button.GetNode<Label>("MarginContainer/HBoxContainer/Name").Text="　"+TranslationServer.Translate(events[a].AsText().TrimSuffix(" (Physical)"), "Indentation");
				button.action=Game.actions.Keys.ToArray()[num];
				button.eventa=events[a];
				action_list.AddChild(button);
			}
		}
		if (CustomMinimumSize.Y!=40+40*b)
		{
			CustomMinimumSize=new Vector2(0,40+40*b);
		}
	}
	public override void _Process(double delta)
	{
		var funcs=GetNode<Keybinds>("../../../../../../");
		if (GetNode<TextureButton>("MarginContainer/VBoxContainer/List/HBoxContainer/+").ButtonPressed)
		{
			var input_keys=GetNode<InputKey>("../../../../../../InputKey");
			input_keys.input_action=Game.actions.Keys.ToArray()[System.Array.IndexOf(Game.actions.Values.ToArray(),GetNode<Label>("MarginContainer/VBoxContainer/List/HBoxContainer/Name").Text)];
			input_keys.edit=false;
			input_keys.Visible=true;
			input_keys.alarm=0.2;
			GetNode<Button>("../../../../../../InputKey/Edit").Text="locWaitingForInput";
			Keybinds.SaveKeybindsToTemp();
			funcs.CreateActionListFromTemp();
		}
		if (GetNode<TextureButton>("MarginContainer/VBoxContainer/List/HBoxContainer/Reset").ButtonPressed)
		{
			InputMap.LoadFromProjectSettings();
			var fk=new ConfigFile();
			Error erk=fk.Load("user://GodotTemplate/Settings/keybinds_temp.ini");
			if (erk == Error.Ok)
			{
				for (var action=0;action<Game.actions.Count;action+=1)
				{
					if (Game.actions.Values.ToArray()[action]!=GetNode<Label>("MarginContainer/VBoxContainer/List/HBoxContainer/Name").Text)
					{
						InputMap.ActionEraseEvents(Game.actions.Keys.ToArray()[action]);
						var array=(Array<InputEvent>)fk.GetValue("Keybinds",Game.actions.Keys.ToArray()[action],new Array<InputEvent>());
						for (var key=0;key<array.Count;key+=1)
						{
							InputMap.ActionAddEvent(Game.actions.Keys.ToArray()[action],array[key]);
						}
					}
				}
			}
			else
			{
				GD.PushError($"Settings(KeybindsTemp) file load failed, error code: {(int)erk}("+erk.ToString()+")");
			}
			Keybinds.SaveKeybindsToTemp();
			funcs.CreateActionListFromTemp();
		}
	}
}
