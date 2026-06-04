using Godot;
using System;

public partial class DebugMenu : Control
{
	public override void _Ready()
	{
		Game Global=GetNode<Game>("/root/Global");
		if (!Global.debug)
		{
			Global.Crashed("Debug Menu is debug mode only\nAdd \"-debug\" launch option");
		}
		GetNode<CheckButton>("ScrollContainer/VBoxContainer/HBoxContainer2/DebugOverlay").ButtonPressed = Global.debug_overlay;
		GetNode<CheckButton>("ScrollContainer/VBoxContainer/HBoxContainer2/DebugOverlayRight").ButtonPressed = Global.debug_overlay_r;
		GetNode<CheckButton>("ScrollContainer/VBoxContainer/HBoxContainer2/DebugOverlayRight").Disabled = !Global.debug_overlay;
	}

	public override void _Process(double delta)
	{
		Game Global=GetNode<Game>("/root/Global");
		if (GetNode<CheckButton>("ScrollContainer/VBoxContainer/HBoxContainer2/DebugOverlay").ButtonPressed!=Global.debug_overlay)
		{
			GetNode<CheckButton>("ScrollContainer/VBoxContainer/HBoxContainer2/DebugOverlay").ButtonPressed=Global.debug_overlay;
		}
		if (GetNode<CheckButton>("ScrollContainer/VBoxContainer/HBoxContainer2/DebugOverlayRight").ButtonPressed!=Global.debug_overlay_r)
		{
			GetNode<CheckButton>("ScrollContainer/VBoxContainer/HBoxContainer2/DebugOverlayRight").ButtonPressed=Global.debug_overlay_r;
		}
		if (GetNode<CheckButton>("ScrollContainer/VBoxContainer/HBoxContainer2/DebugOverlayRight").Disabled==Global.debug_overlay)
		{
			GetNode<CheckButton>("ScrollContainer/VBoxContainer/HBoxContainer2/DebugOverlayRight").Disabled=!Global.debug_overlay;
		}
	}

	public void _on_check_button_toggled(bool toggle)
	{
		GetNode<Label>("ScrollContainer/HBoxContainer/VBoxContainer/Keybinds").Visible = toggle;
	}

	public void _on_back_2_settings_pressed()
	{
		GetTree().ChangeSceneToFile("res://Menu/Settings/Settings.tscn");
	}

	public void _on_change_scene_to_file_b_pressed()
	{
		GetTree().ChangeSceneToFile(GetNode<LineEdit>("ScrollContainer/VBoxContainer/ChangeSceneToFile/ChangeSceneToFile").Text);
	}

	public void _on_debug_overlay_toggled(bool a)
	{
		GetNode<Game>("/root/Global").debug_overlay=a;
	}

	public void _on_debug_overlay_right_toggled(bool a)
	{
		GetNode<Game>("/root/Global").debug_overlay_r=a;
	}

	public void _on_restart_game_pressed()
	{
		if (OS.IsStdOutVerbose())
		{
			GD.Print($"[{Time.GetDatetimeStringFromSystem(false,true)}] Restarted game with debug menu.");
		}
		GetNode<Game>("/root/Global").GameInit();
	}
	public void _on_restart_scene_pressed()
	{
		var tree = GetTree();
		if (OS.IsStdOutVerbose())
		{
			GD.Print($"[{Time.GetDatetimeStringFromSystem(false,true)}] Restarted scene with debug menu: {tree.CurrentScene.Name} ({tree.CurrentScene.SceneFilePath})");
		}
		tree.ReloadCurrentScene();
	}

	public void _on_create_pop_up_message_pressed()
	{
		var message = GetNode<TextEdit>("ScrollContainer/VBoxContainer/CreatePopUpMessage/args/message").Text;
		var type = (Message.TYPE)GetNode<OptionButton>("ScrollContainer/VBoxContainer/CreatePopUpMessage/args/type").Selected;
		var copy_button = GetNode<CheckButton>("ScrollContainer/VBoxContainer/CreatePopUpMessage/args/copy_button").ButtonPressed;
		var shown_time = GetNode<SpinBox>("ScrollContainer/VBoxContainer/CreatePopUpMessage/args/shown_time").Value;
		var close_button = GetNode<CheckButton>("ScrollContainer/VBoxContainer/CreatePopUpMessage/args/close_button").ButtonPressed;
		var custom_button = GetNode<CheckButton>("ScrollContainer/VBoxContainer/CreatePopUpMessage/args/custom_button").ButtonPressed;
		var custom_button_text = GetNode<TextEdit>("ScrollContainer/VBoxContainer/CreatePopUpMessage/args/custom_button_text").Text;
		var custom_method_name = GetNode<TextEdit>("ScrollContainer/VBoxContainer/CreatePopUpMessage/args/custom_method_name").Text;
		GetNode<Game>("/root/Global").CreatePopUpMessage(message, type, copy_button, shown_time, close_button, custom_button, (custom_button_text == "" ? null : custom_button_text), (custom_method_name == "" ? null : custom_method_name));
	}

	public void _on_clear_message_list_pressed()
	{
		GetNode<Game>("/root/Global").ClearMessageList();
	}

	public void _on_color_picker_color_changed(Color color)
	{
		GetNode<Game>("/root/Global").fader_color = color;
	}
}