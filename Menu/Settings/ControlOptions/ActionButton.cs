using Godot;
using System;

public partial class ActionButton : Control
{
	internal InputEvent eventa;
	internal string action;
	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
		var funcs=GetNode<Keybinds>("../../../../../../../../../../");
		Game Global=GetNode<Game>("/root/Global");
		if (GetNode<TextureButton>("MarginContainer/HBoxContainer/Edit").ButtonPressed)
		{
			var input_key=GetNode<InputKey>("../../../../../../../../../../InputKey");
			input_key.input_action=action;
			input_key.edit=true;
			input_key.edit_event=eventa;
			input_key.Visible=true;
			input_key.alarm=0.2;
			GetNode<Button>("../../../../../../../../../../InputKey/Edit").Text="locWaitingForInput";
			Keybinds.SaveKeybindsToTemp();
			funcs.CreateActionListFromTemp();
		}
		if (GetNode<TextureButton>("MarginContainer/HBoxContainer/Remove").ButtonPressed)
		{
			InputMap.ActionEraseEvent(action,eventa);
			Keybinds.SaveKeybindsToTemp();
			funcs.CreateActionListFromTemp();
		}
	}
}
