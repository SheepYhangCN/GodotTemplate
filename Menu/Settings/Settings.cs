using Godot;
using System;

public partial class Settings : Control
{
	public override void _Ready()
	{
		if ((OS.GetName()=="Windows") || (OS.GetName()=="macOS") || (OS.GetName()=="Linux") || (OS.GetName()=="BSD"))
		{
			GetNode<Button>("Control/VBoxContainer/ScreenshotsFolder/ScreenshotsFolder").Disabled=false;
			GetNode<OptionButton>("Control/VBoxContainer/DisplayMode/DisplayMode1").Disabled=false;
			GetNode<OptionButton>("Control/VBoxContainer/Resolution/Resolution1").Disabled=false;
			GetNode<HBoxContainer>("Control/Buttons/ModsFolder").Visible=GetNode<Game>("/root/Global").mod;
			GetNode<HBoxContainer>("Control/Buttons/LocsFolder").Visible=GetNode<Game>("/root/Global").locale;
		}
		GetNode<HBoxContainer>("Control/ButtonsR/DebugMenu").Visible=GetNode<Game>("/root/Global").debug;
		GetNode<OptionButton>("Control/VBoxContainer/Language/Language1").Disabled = !(GetTree().CurrentScene.SceneFilePath == "res://Menu/Settings/Settings.tscn");
		GetNode<HBoxContainer>("Control/VBoxContainer/Language").TooltipText = ((GetTree().CurrentScene.SceneFilePath == "res://Menu/Settings/Settings.tscn") ? "" : "locOnlyMainMenu");
		ContainerUpdate();
	}
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("pause")&&(GetTree().CurrentScene.SceneFilePath=="res://Menu/Settings/Settings.tscn"||Visible))
		{
			_on_back_pressed();
		}
	}
	public void _on_screenshots_folder_pressed()
	{
		Game Global=GetNode<Game>("/root/Global");
		var err=OS.ShellOpen(OS.GetUserDataDir()+"/GodotTemplate/Screenshots");
		if (err == Error.Ok)
		{
			Global.CreatePopUpMessage(TranslationServer.Translate("locMessageFolder").ToString().Replace("{Directory}",OS.GetUserDataDir()+"/GodotTemplate/Screenshots"));
		}
		else
		{
			Global.CreatePopUpMessage(TranslationServer.Translate("locMessageFolderFailed").ToString().Replace("{Directory}",OS.GetUserDataDir()+"/GodotTemplate/Screenshots")+((int)err).ToString()+"("+err.ToString()+")",Message.TYPE.ERROR);
			GD.PushError($"Screenshot folder open failed, error code: {(int)err}("+err.ToString()+")");
		}
	}
	public void _on_control_pressed()
	{
		if (GetTree().CurrentScene.SceneFilePath=="res://Menu/Settings/Settings.tscn")
		{
			GetTree().ChangeSceneToFile("res://Menu/Settings/ControlOptions/ControlOptions.tscn");
		}
		else
		{
			Visible=false;
		}
	}
	public void _on_back_pressed()
	{
		var Global = GetNode<Game>("/root/Global");
		var file=new ConfigFile();
		file.SetValue("Settings","language_display",TranslationServer.GetLocale());
		file.SetValue("Settings","language_audio",(int)Global.lang_audio);
		file.SetValue("Settings","display_mode",(int)DisplayServer.WindowGetMode());
		file.SetValue("Settings","resolution",GetNode<OptionButton>("Control/VBoxContainer/Resolution/Resolution1").Selected);
		file.SetValue("Settings","fps_limit",GetNode<OptionButton>("Control/VBoxContainer/FpsLimit/FpsLimit1").Selected);
		file.SetValue("Settings","vsync",(int)DisplayServer.WindowGetVsyncMode());
		file.SetValue("Settings","volume",AudioServer.GetBusVolumeDb(AudioServer.GetBusIndex("Master")));
		file.SetValue("Settings","use_hdr",GetViewport().UseHdr2D);
		Error err=file.Save("user://GodotTemplate/Settings/settings.ini");
		if (err != Error.Ok)
		{
			GD.PushError($"Settings file save failed, error code: {(int)err}("+err.ToString()+")");
		}
		if (GetTree().CurrentScene.SceneFilePath=="res://Menu/Settings/Settings.tscn")
		{
			GetTree().ChangeSceneToFile("res://Menu/MainMenu/MainMenu.tscn");
		}
		else
		{
			Visible=false;
		}
	}
	internal void ContainerUpdate()
	{
		GetNode<VBoxContainer>("Control/VBoxContainer").Size=new Vector2(500f,GetNode<VBoxContainer>("Control/VBoxContainer").Size.Y);
		GetNode<VBoxContainer>("Control/VBoxContainer").Position=new Vector2(-GetNode<VBoxContainer>("Control/VBoxContainer").Size.X/2,GetNode<VBoxContainer>("Control/VBoxContainer").Position.Y);
	}
	public void _on_mods_folder_pressed()
	{
		Game Global=GetNode<Game>("/root/Global");
		var err=OS.ShellOpen(Game.GetGameDirPath("Mods"));
		if (err == Error.Ok)
		{
			Global.CreatePopUpMessage(TranslationServer.Translate("locMessageFolder").ToString().Replace("{Directory}",Game.GetGameDirPath("Mods")));
		}
		else
		{
			Global.CreatePopUpMessage(TranslationServer.Translate("locMessageFolderFailed").ToString().Replace("{Directory}",Game.GetGameDirPath("Mods"))+((int)err).ToString()+"("+err.ToString()+")",Message.TYPE.ERROR);
			GD.PushError($"Mods folder open failed, error code: {(int)err}("+err.ToString()+")");
		}
	}
	public void _on_locs_folder_pressed()
	{
		Game Global=GetNode<Game>("/root/Global");
		var err=OS.ShellOpen(Game.GetGameDirPath("Locales"));
		if (err == Error.Ok)
		{
			Global.CreatePopUpMessage(TranslationServer.Translate("locMessageFolder").ToString().Replace("{Directory}",Game.GetGameDirPath("Locales")));
		}
		else
		{
			Global.CreatePopUpMessage(TranslationServer.Translate("locMessageFolderFailed").ToString().Replace("{Directory}",Game.GetGameDirPath("Locales"))+((int)err).ToString()+"("+err.ToString()+")",Message.TYPE.ERROR);
			GD.PushError($"Locale folder open failed, error code: {(int)err}("+err.ToString()+")");
		}
	}
	public void _on_chars_folder_pressed()
	{
		Game Global=GetNode<Game>("/root/Global");
		var err=OS.ShellOpen(Game.GetGameDirPath("CustomChars"));
		if (err == Error.Ok)
		{
			Global.CreatePopUpMessage(TranslationServer.Translate("locMessageFolder").ToString().Replace("{Directory}",Game.GetGameDirPath("CustomChars")));
		}
		else
		{
			Global.CreatePopUpMessage(TranslationServer.Translate("locMessageFolderFailed").ToString().Replace("{Directory}",Game.GetGameDirPath("CustomChars"))+((int)err).ToString()+"("+err.ToString()+")",Message.TYPE.ERROR);
			GD.PushError($"Custom character packs folder open failed, error code: {(int)err}("+err.ToString()+")");
		}
	}
	public void _on_debug_menu_pressed()
	{
		GetTree().ChangeSceneToFile("res://Menu/Debug/DebugMenu.tscn");
	}
	public void _on_feedback_pressed()
	{
		GetTree().ChangeSceneToFile("res://Menu/Settings/Feedback/Feedback.tscn");
	}
}
