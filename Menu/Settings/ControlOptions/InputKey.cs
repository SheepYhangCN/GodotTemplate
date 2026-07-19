using Godot;
using System;
using System.Linq;

public partial class InputKey : Control
{
	internal string input_action;
	internal bool edit=false;
	internal InputEvent edit_event;
	InputEvent ievent;
	internal double alarm=0;
	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
		if (!Singleton.Game.input_remapping)
		{
			if (alarm>0)
			{
				alarm-=delta;
			}
			if (GetNode<Button>("Button/No").ButtonPressed)
			{
				GetNode<Button>("Edit").Text="locWaitingForInput";
				Visible=false;
				Singleton.Game.input_remapping=false;
				edit=false;
				alarm=0;
			}
			if (GetNode<Button>("Button/Yes").ButtonPressed)
			{
				if (edit)
				{
					InputMap.ActionEraseEvent(input_action,edit_event);
				}
				InputMap.ActionAddEvent(input_action,ievent);
				Keybinds.SaveKeybindsToTemp();
				GetNode<Keybinds>("../").CreateActionListFromTemp();
				Visible=false;
				Singleton.Game.input_remapping=false;
				edit=false;
				alarm=0;
			}
			if (GetNode<Button>("Edit").ButtonPressed && !Singleton.Game.input_remapping && alarm==0)
			{
				GetNode<Button>("Edit").Text="locWaitingForInput";
				alarm=0.2;
			}
			if (alarm<0)
			{
				alarm=0;
				Singleton.Game.input_remapping=true;
			}
		}
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		if (Singleton.Game.input_remapping&&(@event is InputEventKey||@event is InputEventMouseButton))//||@event is InputEventJoypadButton||@event is InputEventJoypadMotion))
		{
			if (@event is InputEventMouseButton inpute && inpute.DoubleClick)
			{
				inpute.DoubleClick=false;
			}
			if (@event.IsReleased())
			{
				Singleton.Game.input_remapping=false;
			}
			else
			{
				GetNode<Button>("Edit").Text = TranslationServer.Translate(@event.AsText().TrimSuffix(" - Physical"), "Indentation");
				ievent=@event;
			}
		}
	}
}
